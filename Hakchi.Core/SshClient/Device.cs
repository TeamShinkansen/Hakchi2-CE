using System.Net;

namespace Hakchi.Core.SshClient
{
    public class Device
    {
        public IList<IPAddress> Addresses;
        public ushort Port;
        public string UniqueID;
        public string ConsoleType;
        public string ConsoleRegion;
    }
}
