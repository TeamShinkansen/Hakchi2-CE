using Hakchi.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hakchi.Core.Services
{
    [RegisterService(ServiceLifetime.Singleton, typeof(IAssemblyInfo))]
    internal class AssemblyInfo: IAssemblyInfo
    {
        public Assembly EntryAssembly => field ??= Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Unable to determine the entry assembly.");
        public string EntryAssemblyLocation => field ??= EntryAssembly.Location ?? throw new InvalidOperationException("Unable to determine the entry assembly location.");
        public string EntryAssemblyDirectory => field ??= Path.GetDirectoryName(EntryAssemblyLocation) ?? throw new InvalidOperationException("Unable to determine the entry assembly directory.");
        public AssemblyName EntryAssemblyName => field ??= EntryAssembly.GetName() ?? throw new InvalidOperationException("Unable to determine the entry assembly name.");
        public Version EntryAssemblyVersion => field ??= EntryAssemblyName.Version ?? throw new InvalidOperationException("Unable to determine the entry assembly version.");
    }
}
