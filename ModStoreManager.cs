using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;
using com.clusterrr.hakchi_gui.Properties;
using SevenZip;

namespace com.clusterrr.hakchi_gui.module_library
{
    public class ModStoreManager
    {
        public List<Module> AvailableModules = new List<Module>();
        public List<InstalledModule> InstalledModules = new List<InstalledModule>();
        public DateTime LastUpdate = new DateTime();
        public string ConfigPath { get { return Path.Combine(Program.BaseDirectoryExternal, "config\\ModStoreConfig.xml"); } }

        public void DownloadModule(Module module)
        {
            try
            {
                string userModDir = Path.Combine(Program.BaseDirectoryExternal, "user_mods");
                var installedModule = GetInstalledModule(module);
                //If module is installed remove it
                if (installedModule != null)
                {
                    foreach(var file in installedModule.InstalledFiles)
                    {
                        try
                        {
                            if (file.EndsWith("\\"))
                                Directory.Delete(Path.Combine(userModDir, file), true);
                            else
                                File.Delete(Path.Combine(userModDir, file));
                        }
                        catch { }
                    }

                    InstalledModules.Remove(installedModule);
                    installedModule = null;
                }
                switch (module.Type)
                {
                    case ModuleType.hmod:
                        {
                            string fileLocation = Path.Combine(userModDir, module.Path.Substring(module.Path.LastIndexOf('/') + 1));

                            using (var wc = new WebClient())
                            {
                                wc.DownloadFile(module.Path, fileLocation);
                                installedModule = module.CreateInstalledModule();
                                installedModule.InstalledFiles.Add(module.Path.Substring(module.Path.LastIndexOf('/') + 1));
                                InstalledModules.Add(installedModule);
                            }
                        }
                        break;
                    case ModuleType.compressedFile:
                        SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                        using (var wc = new WebClient())
                        {
                            var tempFileName = Path.GetTempFileName();
                            wc.DownloadFile(module.Path, tempFileName);
                            using (var szExtractor = new SevenZipExtractor(tempFileName))
                            {
                                installedModule = module.CreateInstalledModule();
                                var data = szExtractor.ArchiveFileData;
                                foreach(var file in data)
                                {
                                    int index = file.FileName.IndexOf('\\');
                                    if (index != -1)
                                    {
                                        var folder = file.FileName.Substring(0, index + 1);
                                        if (!installedModule.InstalledFiles.Contains(folder))
                                        {
                                            installedModule.InstalledFiles.Add(folder);
                                            var localFolder = Path.Combine(userModDir, folder);
                                            if (Directory.Exists(localFolder))
                                                Directory.Delete(localFolder, true);
                                        }
                                    }
                                    else
                                        installedModule.InstalledFiles.Add(file.FileName);
                                }
                                szExtractor.ExtractArchive(userModDir);
                                InstalledModules.Add(installedModule);
                            }
                            File.Delete(tempFileName);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public InstalledModule GetInstalledModule(Module repoModule)
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
