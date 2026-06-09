using Hakchi.Core.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Xml;

namespace Hakchi.Core.Services
{
    public class HakchiPaths : IHakchiPaths
    {
        public string BaseDirectoryInternal { get; private set; }
        public string BaseDirectoryExternal { get; private set; }
        public HakchiPaths(IAssemblyInfo assemblyInfo, ILaunchFlags launchFlags, ISpecialLocations specialLocations)
        {
            BaseDirectoryInternal = assemblyInfo.EntryAssemblyDirectory;
            BaseDirectoryExternal = launchFlags.IsPortable ? BaseDirectoryInternal : Path.Combine(specialLocations.Documents, "hakchi2");
        }
    }
}
