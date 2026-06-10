using Hakchi.Core.Interfaces;

namespace Hakchi.Core.Services
{
    internal class LaunchArguments: ILaunchArguments
    {
        public string[] Arguments { get; init; }

        public LaunchArguments(string[] args)
        {
            Arguments = args;
        }
    }
}
