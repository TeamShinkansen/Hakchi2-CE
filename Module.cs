using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.clusterrr.hakchi_gui.module_library
{
    public enum ModuleType { hmod = 0, zippedfolder, zippedfiles, list }
    public class Module
    {
        public string Name;
        public string Id;
        public string Path;
        public string Description;
        public string Version;
        public ModuleType Type;
        public string IdHmod { get { return (Id.EndsWith(".hmod")) ? Id : Id + ".hmod"; } }
        public Module Clone()
        {
            var newObject = (Module)this.MemberwiseClone();
            newObject.Description = null;
            return newObject;
        }
    }
    public class Repository
    {
        public string Author;
        public string Url;
        public List<Module> Modules = new List<Module>();
    }
}
