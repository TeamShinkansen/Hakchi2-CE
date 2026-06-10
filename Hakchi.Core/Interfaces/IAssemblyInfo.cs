using System.Reflection;

namespace Hakchi.Core.Interfaces
{
    public interface IAssemblyInfo
    {
        public Assembly EntryAssembly { get; }
        public string EntryAssemblyLocation { get; }
        public string EntryAssemblyDirectory { get; }
        public AssemblyName EntryAssemblyName { get; }
        public Version EntryAssemblyVersion { get; }
    }
}
