using Hakchi.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hakchi.Core.Services
{
    public class AssemblyInfo: IAssemblyInfo
    {
        public Assembly EntryAssembly => field ??= Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Unable to determine the entry assembly.");
        public string EntryAssemblyLocation => field ??= EntryAssembly.Location ?? throw new InvalidOperationException("Unable to determine the entry assembly location.");

        public string EntryAssemblyDirectory => field ??= Path.GetDirectoryName(EntryAssemblyLocation) ?? throw new InvalidOperationException("Unable to determine the entry assembly directory.");
        
    }
}
