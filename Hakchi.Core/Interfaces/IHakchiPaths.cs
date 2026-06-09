using System;
using System.Collections.Generic;
using System.Text;

namespace Hakchi.Core.Interfaces
{
    public interface IHakchiPaths
    {
        public string BaseDirectoryInternal { get; }
        public string BaseDirectoryExternal { get; }
    }
}
