using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.module_library;
using SevenZip;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace com.clusterrr.hakchi_gui
{
    public partial class ModStore : Form
    {
        #region Form Initialisation

        public class LibraryConfig
        {
            public List<Module> AvailableModules = new List<Module>();
            public List<Module> InstalledModules = new List<Module>();
            public DateTime LastUpdate = new DateTime();
        }

        private LibraryConfig config = new LibraryConfig();
        private string configPath { get { return Path.Combine(Program.BaseDirectoryExternal, "config\\libraryConfig.xml"); } }
        private Module currentModule { get; set; }

        
        public ModStore()
        {
            InitializeComponent();
            this.Icon = Resources.icon;
            PoweredByLinkS.Text = "Powered By HakchiResources.com";
        }

        private void ModStore_Load(object sender, EventArgs e)
        {
            //Load Config
            XmlSerializer xs = new XmlSerializer(typeof(LibraryConfig));
            if (File.Exists(configPath))
            {
                using (var fs = File.Open(configPath, FileMode.Open))
                {
                    config = (LibraryConfig)xs.Deserialize(fs);
                }
            }

            if (config.AvailableModules.Count == 0 || (DateTime.Now - config.LastUpdate).TotalDays >= 1.0)
            {
                //Ask user to update repository information
                updateModuleList();
            }
            loadModuleList();
        }
        #endregion

        #region Non Essential GUI code
        private void PoweredByLinkS_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.hakchiresources.com");
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.hakchiresources.com");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //Web Browser Navigation Fix
            webBrowser1.AllowNavigation = false;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var modStoreAbout = new ModStoreAbout();
            modStoreAbout.ShowDialog();
        }
        #endregion

        #region Main Mod Store Code
        private void updateModuleList()
        {
            try
            {
                JObject json;
                using (var wc = new WebClient())
                {
                    json = JObject.Parse(wc.DownloadString("https://hakchiresources.com/api/get_posts/?count=10000"));
                }
                config.AvailableModules.Clear();
                foreach (var post in json["posts"])
                {
                    bool skip = false;
                    foreach (var tag in post["tags"])
                    {
                        if (tag["slug"].ToString().Equals("non_hmod"))
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (skip)
                        continue;

                    try
                    {
                        Module module = new Module
                        {
                            Name = System.Web.HttpUtility.HtmlDecode(post["title"].ToString()),
                            Id = System.Web.HttpUtility.HtmlDecode(post["title"].ToString()), //Temporary ID need to replace
                            Author = post["custom_fields"]["user_submit_name"][0].ToString(),
                            Description = post["url"].ToString() + "?mode=mod_store",
                            Version = post["custom_fields"]["usp_custom_field"][0].ToString(),
                            Path = post["custom_fields"]["user_submit_url"][0].ToString()
                        };
                        var extention = module.Path.Substring(module.Path.LastIndexOf('.') + 1).ToLower();
                        if (extention.Equals("hmod"))
                            module.Type = ModuleType.hmod;
                        else if (extention.Equals("zip") || extention.Equals("7z") || extention.Equals("rar"))
                            module.Type = ModuleType.compressedFile;
                        else
                            continue; //Unknown File Type
                        config.AvailableModules.Add(module);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            config.LastUpdate = DateTime.Now;
            updateModules();
        }

        private void updateModules()
        {
            List<Module> modulesToUpdate = new List<Module>();
            //For each installed module find the matching repo entry
            foreach (var module in config.InstalledModules)
            {
                Module repoModule = null;
                foreach (var rModule in config.AvailableModules)
                {
                    if (rModule.Id == module.Id)
                    {
                        repoModule = rModule;
                        break;
                    }
                }
                if (repoModule != null && repoModule.Version != module.Version)
                    modulesToUpdate.Add(repoModule);
            }
            if (modulesToUpdate.Count != 0)
            {
                var updateMsgBox = MessageBox.Show("Do you want to update all out of date modules?", "Update Modules", MessageBoxButtons.YesNo);
                if (updateMsgBox == DialogResult.Yes)
                {
                    var progressBarForm = new ProgressBarForm("Updating Modules", modulesToUpdate.Count);
                    progressBarForm.Run(() =>
                    {
                        for (int i = 0; i < modulesToUpdate.Count; ++i)
                        {
                            downloadModule(modulesToUpdate[i]);
                            progressBarForm.UpdateProgress();
                        }
                    });
                    MessageBox.Show(this, "Finished updating modules.");
                }
            }
        }

        private Module getInstalledModule(Module repoModule)
        {
            foreach (var module in config.InstalledModules)
            {
                if (module.Id == repoModule.Id)
                    return module;
            }
            return null;
        }

        private void loadModuleList()
        {
            moduleListBox.Items.Clear();
            foreach (var module in config.AvailableModules)
            {
                moduleListBox.Items.Add(module.Name);
            }
            moduleListBox.SelectedIndex = 0;
        }

        private void moduleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = moduleListBox.SelectedIndex;
            if (index != -1)
            {
                if (currentModule != config.AvailableModules[index])
                {
                    currentModule = config.AvailableModules[index];
                    loadModuleDescription();
                }
            }
            else
                currentModule = null;
        }

        private void loadModuleDescription()
        {
            webBrowser1.AllowNavigation = true;
            webBrowser1.Url = new Uri(currentModule.Description, UriKind.Absolute);
            var installedModule = getInstalledModule(currentModule);
            if (installedModule != null)
            {
                if (installedModule.Version != currentModule.Version)
                {
                    moduleDownloadButton.Enabled = true;
                    moduleDownloadButton.Text = "Update Module";
                }
                else
                {
                    moduleDownloadButton.Enabled = false;
                    moduleDownloadButton.Text = "Module Up-To-Date";
                }
                //moduleLabel.Text = "Downloaded version: " + installedModule.Version;
            }
            else
            {
                moduleDownloadButton.Enabled = true;
                moduleDownloadButton.Text = "Download Module";
                //moduleLabel.Text = "";
            }
        }

        private void moduleDownloadButton_Click(object sender, EventArgs e)
        {
            downloadModule(currentModule);
            loadModuleDescription();
            saveConfig();
        }

        private void downloadModule(Module module)
        {
            try
            {
                var installedModule = getInstalledModule(module);
                //If module is installed remove it
                if (installedModule != null)
                {
                    if (installedModule.Type == ModuleType.hmod)
                        File.Delete(installedModule.Path);
                    else
                        Directory.Delete(installedModule.Path, true);

                    config.InstalledModules.Remove(installedModule);
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
                                config.InstalledModules.Add(installedModule);
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
                MessageBox.Show(this, "Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModuleLibraryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveConfig();
        }

        private void saveConfig()
        {
            if (!Directory.Exists(Path.GetDirectoryName(configPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(configPath));
            XmlSerializer x = new XmlSerializer(typeof(LibraryConfig));
            using (var fs = new FileStream(configPath, FileMode.Create))
            {
                x.Serialize(fs, config);
            }
        }
        #endregion

    }
}