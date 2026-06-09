using Hakchi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
