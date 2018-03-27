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
        public ModuleType Type;
        public Module Clone()
        {
            var newObject = (Module)this.MemberwiseClone();
            newObject.Description = null;
            return newObject;
        }
    }
}
