using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace com.clusterrr.ssh
{
    /// <summary>
    /// Minimal one-shot mDNS "A record" lookup (RFC 6762), used on Linux where Tmds.MDns cannot run under Mono.
    /// The query is sent on every IPv4 interface from an ephemeral port, so responders answer by unicast.
    /// </summary>
    public static class MdnsHostResolver
    {
        static readonly IPEndPoint mdnsGroup = new IPEndPoint(IPAddress.Parse("224.0.0.251"), 5353);

        public static List<IPAddress> Resolve(string hostName, int timeoutMs = 1000)
        {
            var found = new List<IPAddress>();
            var query = buildQuery(hostName);
            foreach (var local in localIPv4Addresses())
            {
                try
                {
                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        socket.Bind(new IPEndPoint(local, 0));
                        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, local.GetAddressBytes());
                        socket.ReceiveTimeout = timeoutMs;
                        socket.SendTo(query, mdnsGroup);

                        var buffer = new byte[9000];
                        var deadline = DateTime.Now.AddMilliseconds(timeoutMs);
                        while (DateTime.Now < deadline)
                        {
                            int len;
                            try
                            {
                                len = socket.Receive(buffer);
                            }
                            catch (SocketException)
                            {
                                break; // timeout
                            }
                            foreach (var address in parseARecords(buffer, len, hostName))
                                if (!found.Contains(address))
                                    found.Add(address);
                            if (found.Count > 0)
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"mDNS query via {local} failed: {ex.Message}");
                }
            }
            return found;
        }

        static IEnumerable<IPAddress> localIPv4Addresses()
        {
            // GetIPProperties()/UnicastAddresses work on Mono; only the IPv6-specific properties are unimplemented
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .SelectMany(n => n.GetIPProperties().UnicastAddresses)
                .Select(a => a.Address)
                .Where(a => a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a))
                .Distinct()
                .ToList();
        }

        static byte[] buildQuery(string hostName)
        {
            var packet = new List<byte>();
            packet.AddRange(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 }); // id, flags, 1 question
            foreach (var label in hostName.TrimEnd('.').Split('.'))
            {
                var bytes = Encoding.UTF8.GetBytes(label);
                packet.Add((byte)bytes.Length);
                packet.AddRange(bytes);
            }
            packet.Add(0);
            packet.AddRange(new byte[] { 0, 1, 0x80, 1 }); // type A, class IN with the unicast-response bit
            return packet.ToArray();
        }

        static IEnumerable<IPAddress> parseARecords(byte[] data, int length, string hostName)
        {
            var results = new List<IPAddress>();
            if (length < 12 || (data[2] & 0x80) == 0) // not a response
                return results;
            int questions = (data[4] << 8) | data[5];
            int records = ((data[6] << 8) | data[7]) + ((data[8] << 8) | data[9]) + ((data[10] << 8) | data[11]);
            int pos = 12;
            try
            {
                for (int i = 0; i < questions; i++)
                {
                    readName(data, length, ref pos);
                    pos += 4;
                }
                for (int i = 0; i < records && pos + 10 <= length; i++)
                {
                    var name = readName(data, length, ref pos);
                    int type = (data[pos] << 8) | data[pos + 1];
                    int rdLength = (data[pos + 8] << 8) | data[pos + 9];
                    pos += 10;
                    if (pos + rdLength > length)
                        break;
                    if (type == 1 && rdLength == 4 && string.Equals(name.TrimEnd('.'), hostName.TrimEnd('.'), StringComparison.OrdinalIgnoreCase))
                        results.Add(new IPAddress(new byte[] { data[pos], data[pos + 1], data[pos + 2], data[pos + 3] }));
                    pos += rdLength;
                }
            }
            catch (IndexOutOfRangeException)
            {
                // malformed packet; keep what was parsed
            }
            return results;
        }

        static string readName(byte[] data, int length, ref int pos)
        {
            var labels = new List<string>();
            int cursor = pos;
            bool jumped = false;
            for (int hops = 0; hops < 64; hops++)
            {
                int len = data[cursor];
                if (len == 0)
                {
                    cursor++;
                    break;
                }
                if ((len & 0xC0) == 0xC0) // compression pointer
                {
                    int target = ((len & 0x3F) << 8) | data[cursor + 1];
                    if (!jumped)
                        pos = cursor + 2;
                    jumped = true;
                    cursor = target;
                    continue;
                }
                if (cursor + 1 + len > length)
                    throw new IndexOutOfRangeException();
                labels.Add(Encoding.UTF8.GetString(data, cursor + 1, len));
                cursor += 1 + len;
            }
            if (!jumped)
                pos = cursor;
            return string.Join(".", labels);
        }
    }
}
