using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.module_library
{
    public enum ModuleType { unknown = 0, hmod, compressedFile }
    public class Module
    {
        public string Id; // Need to develop some sort of website id system
        public string Name;
        public string Author;
        public string Path;
        public string Description;
        public string Version;
        public List<string> Categories = new List<string>();
        public ModuleType Type;

        public InstalledModule CreateInstalledModule()
        {
            return new InstalledModule { Id = Id, Name = Name, Version = Version};
        }
    }
    public class InstalledModule
    {
        public string Id;
        public string Name; //Temporary - incase of ID issues
        public string Version;
        public List<string> InstalledFiles = new List<string>();
    }
}
