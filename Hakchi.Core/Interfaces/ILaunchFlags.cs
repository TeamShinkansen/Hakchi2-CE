using System;
using System.Collections.Generic;
using System.Text;

namespace Hakchi.Core.Interfaces
{
    public interface ILaunchFlags
    {
        public bool IsPortable { get; }
        public bool IsDebug { get; }
        public string? VersionFormat { get; }
        public string? VersionFile { get; }
    }
}
