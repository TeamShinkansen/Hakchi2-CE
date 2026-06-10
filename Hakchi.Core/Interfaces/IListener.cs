using Hakchi.Core.SshClient;

namespace Hakchi.Core.Interfaces
{
    public interface IListener : IDisposable
    {
        IList<Device> Available
        {
            get;
        }

        void Cycle();
    }
}
