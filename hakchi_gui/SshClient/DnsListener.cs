using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace com.clusterrr.ssh
{
    public class DnsListener : IListener
    {
        public const int TTL = 60;
        public const int TTR = 15;

        public IList<Device> Available
        {
            get
            {
                if (DateTime.Now.Subtract(lastChecked) > TimeSpan.FromSeconds(TTR))
                {
                    lastChecked = DateTime.Now;
                    query();
                }
                return devices;
            }
        }

        private string serviceName;
        private List<Device> devices;
        private DateTime lastUpdated;
        private DateTime lastChecked;

        private void query()
        {
            if (DateTime.Now.Subtract(lastUpdated) > TimeSpan.FromSeconds(TTL))
            {
                devices.Clear();
                // Linux resolvers usually only answer mDNS names (avahi/systemd-resolved) with the .local suffix
                var names = com.clusterrr.hakchi_gui.Shared.isWindows ? new[] { serviceName } : new[] { serviceName, serviceName + ".local" };
                foreach (var name in names)
                {
                    try
                    {
                        IPHostEntry ihe = Dns.GetHostEntry(name);
                        if (ihe.AddressList != null && ihe.AddressList.Length > 0)
                        {
                            Trace.WriteLine($"DNS Resolution returned IPs: " + string.Join(", ", (IEnumerable<IPAddress>)ihe.AddressList));
                            lastUpdated = DateTime.Now;
                            foreach (var address in ihe.AddressList)
                                devices.Add(new Device() { Addresses = new List<IPAddress>() { address }, Port = SshClientWrapper.DEFAULT_SSH_PORT });
                            break;
                        }
                    }
                    catch
                    {
                        // no-op (dns resolution fail causes exception)
                    }
                }

                // No system mDNS resolver (avahi/nss-mdns, systemd-resolved): ask the network directly
                if (devices.Count == 0 && !com.clusterrr.hakchi_gui.Shared.isWindows)
                {
                    var addresses = MdnsHostResolver.Resolve(serviceName + ".local");
                    if (addresses.Count > 0)
                    {
                        Trace.WriteLine("mDNS lookup returned IPs: " + string.Join(", ", addresses));
                        lastUpdated = DateTime.Now;
                        foreach (var address in addresses)
                            devices.Add(new Device() { Addresses = new List<IPAddress>() { address }, Port = SshClientWrapper.DEFAULT_SSH_PORT });
                    }
                }

                // hakchi's USB gadget always uses this link-local address (S92rndis), and its tinysvcmdns responder
                // may not answer direct queries, so probe it as a last resort.
                if (devices.Count == 0 && !com.clusterrr.hakchi_gui.Shared.isWindows && sshPortOpen(hakchiUsbAddress))
                {
                    Trace.WriteLine("Found hakchi at its default USB address " + hakchiUsbAddress);
                    lastUpdated = DateTime.Now;
                    devices.Add(new Device() { Addresses = new List<IPAddress>() { hakchiUsbAddress }, Port = SshClientWrapper.DEFAULT_SSH_PORT });
                }
            }
        }

        private static readonly IPAddress hakchiUsbAddress = IPAddress.Parse("169.254.13.37");

        private static bool sshPortOpen(IPAddress address)
        {
            try
            {
                using (var client = new System.Net.Sockets.TcpClient())
                {
                    var connect = client.BeginConnect(address, SshClientWrapper.DEFAULT_SSH_PORT, null, null);
                    if (!connect.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500)))
                        return false;
                    client.EndConnect(connect);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public DnsListener(string name)
        {
            serviceName = name;
            devices = new List<Device>();
            lastUpdated = DateTime.Now.Subtract(TimeSpan.FromSeconds(TTL));
            lastChecked = DateTime.Now;
        }

        public void Cycle()
        {
            devices.Clear();
        }

        public void Dispose()
        {
            devices.Clear();
            devices = null;
        }
    }
}
