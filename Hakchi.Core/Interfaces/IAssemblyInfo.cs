using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hakchi.Core.Interfaces
{
    public interface IAssemblyInfo
    {
        public Assembly EntryAssembly { get; }
        public string EntryAssemblyLocation { get; }
        public string EntryAssemblyDirectory { get; }
    }
}
