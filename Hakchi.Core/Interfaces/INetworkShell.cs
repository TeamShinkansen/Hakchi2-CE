namespace Hakchi.Core.Interfaces
{
    public interface INetworkShell
    {
        string IPAddress
        {
            get;
        }
        int Ping();
    }
}
