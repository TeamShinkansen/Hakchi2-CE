using System;
using System.Collections.Generic;
using System.Text;

namespace Hakchi.Core.Interfaces
{
    public interface ILaunchArguments
    {
        public string[] Arguments { get; init; }
    }
}
