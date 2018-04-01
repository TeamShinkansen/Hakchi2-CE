using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using com.clusterrr.hakchi_gui.Properties;

namespace com.clusterrr.hakchi_gui.module_library
{
    public class ModStoreManager
    {
        public List<Module> AvailableModules = new List<Module>();
        public List<Module> InstalledModules = new List<Module>();
        public DateTime LastUpdate = new DateTime();
        public string ConfigPath { get { return Path.Combine(Program.BaseDirectoryExternal, "config\\ModStoreConfig.xml"); } }

        public void DownloadModule(Module module)
        {
            try
            {
                var installedModule = GetInstalledModule(module);
                //If module is installed remove it
                if (installedModule != null)
                {
                    if (installedModule.Type == ModuleType.hmod)
                        File.Delete(installedModule.Path);
                    else
                        Directory.Delete(installedModule.Path, true);

                    InstalledModules.Remove(installedModule);
                    installedModule = null;
                }
                string userModDir = Path.Combine(Program.BaseDirectoryExternal, "user_mods");
                switch (module.Type)
                {
                    case ModuleType.hmod:
                        {
                            string fileLocation = Path.Combine(userModDir, module.Path.Substring(module.Path.LastIndexOf('/') + 1));

                            using (var wc = new WebClient())
                            {
                                wc.DownloadFile(module.Path, fileLocation);
                                installedModule = module.Clone();
                                installedModule.Path = fileLocation;
                                InstalledModules.Add(installedModule);
                            }
                        }
                        break;
                        //case ModuleType.compressedFile:
                        //    SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                        //    using (var wc = new WebClient())
                        //    {
                        //        var tempFileName = Path.GetTempFileName();
                        //        wc.DownloadFile(module.Path, tempFileName);
                        //        using (var szExtractor = new SevenZipExtractor(tempFileName))
                        //        {
                        //            installedModule = module.Clone();
                        //            installedModule.Path = Path.Combine(userModDir, installedModule.IdHmod);
                        //            if (Directory.Exists(installedModule.Path)) Directory.Delete(installedModule.Path, true);
                        //            if (module.Type == ModuleType.zippedfolder)
                        //            {
                        //                var tempPath = Path.Combine(Path.GetTempPath(), "hmod");
                        //                if (szExtractor.ArchiveFileData[0].IsDirectory == false)
                        //                    throw new Exception("Cannot find folder in module zip.");
                        //                szExtractor.ExtractArchive(tempPath);
                        //                Directory.Move(Path.Combine(tempPath, szExtractor.ArchiveFileData[0].FileName), installedModule.Path);
                        //                Directory.Delete(tempPath, true);
                        //            }
                        //            else
                        //            {
                        //                Directory.CreateDirectory(installedModule.Path);
                        //                szExtractor.ExtractArchive(installedModule.Path);
                        //            }
                        //            config.InstalledModules.Add(installedModule);
                        //        }
                        //        File.Delete(tempFileName);
                        //    }
                        //    break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Module GetInstalledModule(Module repoModule)
        {
            foreach (var module in InstalledModules)
            {
                if (module.Id == repoModule.Id)
                    return module;
            }
            return null;
        }

        public void SaveConfig()
        {
            if (!Directory.Exists(Path.GetDirectoryName(ConfigPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            XmlSerializer x = new XmlSerializer(typeof(ModStoreManager));
            using (var fs = new FileStream(ConfigPath, FileMode.Create))
            {
                x.Serialize(fs, this);
            }
        }
    }
}
