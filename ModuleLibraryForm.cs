using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.module_library;
using SevenZip;

namespace com.clusterrr.hakchi_gui
{
    public partial class ModuleLibraryForm : Form
    {
        public class LibraryConfig
        {
            public List<Repository> Repositories = new List<Repository>();
            public List<Module> InstalledModules = new List<Module>();
            public DateTime LastUpdate = new DateTime();
        }

        private LibraryConfig config = new LibraryConfig();
        private string configPath { get { return Path.Combine(Program.BaseDirectoryExternal, "config\\libraryConfig.xml"); } }
        private Repository currentRepo { get; set; }
        private Module currentModule { get; set; }

        public ModuleLibraryForm()
        {
            InitializeComponent();
        }

        private void ModuleLibraryForm_Load(object sender, EventArgs e)
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

            if (config.Repositories.Count == 0)
            {
                //Ask user if they want to add default repos
                var addDefRepoMsgBox = MessageBox.Show("Do you want to add the default repositories?", "Add Default Repositories", MessageBoxButtons.YesNo);
                if (addDefRepoMsgBox == DialogResult.Yes)
                    addRepo("https://raw.githubusercontent.com/CompCom/hmrepo/master/repo.xml");
            }
            else if ((DateTime.Now - config.LastUpdate).TotalDays >= 1.0)
            {
                //Ask user to update repository information
                var updateMsgBox = MessageBox.Show("Do you want to update the repositories?", "Update Repository Information", MessageBoxButtons.YesNo);
                if (updateMsgBox == DialogResult.Yes)
                    updateRepos();
            }
            loadRepositoryList();
        }

        private void updateRepos()
        {
            for(int i = 0; i < config.Repositories.Count; ++i)
            {
                var updatedRepo = getRepoFromUrl(config.Repositories[i].Url);
                if(updatedRepo != null)
                    config.Repositories[i] = updatedRepo;
            }
            config.LastUpdate = DateTime.Now;
            MessageBox.Show(this, "Finished updating repository information.", "Update complete");
            updateModules();
        }

        private void updateModules()
        {
            List<Module> modulesToUpdate = new List<Module>();
            foreach(var module in config.InstalledModules)
            {
                Module repoModule = null;
                foreach(var repo in config.Repositories)
                {
                    foreach(var rModule in repo.Modules)
                    {
                        if(rModule.Id == module.Id)
                        {
                            repoModule = rModule;
                            break;
                        }
                    }
                    if (repoModule != null) break;
                }
                if (repoModule != null && repoModule.Version != module.Version)
                    modulesToUpdate.Add(repoModule);
            }
            if(modulesToUpdate.Count != 0)
            {
                var updateMsgBox = MessageBox.Show("Do you want to update all out of date modules?", "Update Modules", MessageBoxButtons.YesNo);
                if (updateMsgBox == DialogResult.Yes)
                {
                    foreach(var module in modulesToUpdate)
                    {
                        downloadModule(module);
                    }
                }
                MessageBox.Show(this, "Finished updating modules.");
            }
        }

        private void loadRepositoryList()
        {
            repositoryListBox.Items.Clear();
            foreach (var repo in config.Repositories)
            {
                repositoryListBox.Items.Add(repo.Author + "'s Repository");
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

        private void addRepoButton_Click(object sender, EventArgs e)
        {
            var form = new StringInputForm();
            form.Text = "Add repository";
            form.labelComments.Text = "Enter repository url:";
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (addRepo(form.textBox.Text))
                {
                    repositoryListBox.SelectedIndex = config.Repositories.Count - 1;
                    saveConfig();
                }
            }
        }

        private Repository getRepoFromUrl(string url)
        {
            Repository r = null;
            try
            {
                using (var wc = new WebClient())
                {
                    using (var ws = wc.OpenRead(url))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(Repository));
                        r = (Repository)xs.Deserialize(ws);
                    }
                }
            }
            catch
            {
                MessageBox.Show(this, "Error could not download repository at: " + url, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return r;
        }

        private bool addRepo(string url)
        {
            try
            {
                var newRepo = getRepoFromUrl(url);
                if (newRepo == null)
                    return false;
                config.Repositories.Add(newRepo);
                loadRepositoryList();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Critical error: " + ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void repositoryListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = repositoryListBox.SelectedIndex;
            if (index != -1)
            {
                if (currentRepo != config.Repositories[index])
                {
                    currentRepo = config.Repositories[index];
                    loadModuleList();
                }
            }
            else
            {
                currentRepo = null;
                currentModule = null;
            }
        }

        private void loadModuleList()
        {
            moduleListBox.Items.Clear();
            foreach (var module in currentRepo.Modules)
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
                if (currentModule != currentRepo.Modules[index])
                {
                    currentModule = currentRepo.Modules[index];
                    loadModuleDescription();
                }
            }
            else
                currentModule = null;
        }

        private void loadModuleDescription()
        {
            moduleDescriptionBrowser.DocumentText = String.Format(
                "<b>Module Name:</b> {0}<br /><b>Author:</b> {1}<br /><b>Version:</b> {2} <br /><b>Description:</b><br />{3}",
                currentModule.Name, currentRepo.Author, currentModule.Version, currentModule.Description);
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
            }
            else
            {
                moduleDownloadButton.Enabled = true;
                moduleDownloadButton.Text = "Download Module";
            }
        }

        private void moduleDownloadButton_Click(object sender, EventArgs e)
        {
            downloadModule(currentModule);
            saveConfig();
        }

        private void downloadModule(Module module)
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
                        string fileLocation = Path.Combine(userModDir, module.IdHmod);

                        using (var wc = new WebClient())
                        {
                            wc.DownloadFile(module.Path, fileLocation);
                            installedModule = module.Clone();
                            installedModule.Path = fileLocation;
                            config.InstalledModules.Add(installedModule);
                        }
                    }
                    break;
                case ModuleType.zippedfolder:
                case ModuleType.zippedfiles:
                    SevenZipExtractor.SetLibraryPath(Path.Combine(Program.BaseDirectoryInternal, IntPtr.Size == 8 ? @"tools\7z64.dll" : @"tools\7z.dll"));
                    using (var wc = new WebClient())
                    {
                        var tempFileName = Path.GetTempFileName();
                        wc.DownloadFile(module.Path, tempFileName);
                        using (var szExtractor = new SevenZipExtractor(tempFileName))
                        {
                            installedModule = module.Clone();
                            installedModule.Path = Path.Combine(userModDir, installedModule.IdHmod);
                            if (Directory.Exists(installedModule.Path)) Directory.Delete(installedModule.Path, true);
                            if (module.Type == ModuleType.zippedfolder)
                            {
                                var tempPath = Path.Combine(Path.GetTempPath(), "hmod");
                                if (szExtractor.ArchiveFileData[0].IsDirectory == false)
                                    throw new Exception("Cannot find folder in module zip.");
                                szExtractor.ExtractArchive(tempPath);
                                Directory.Move(Path.Combine(tempPath, szExtractor.ArchiveFileData[0].FileName), installedModule.Path);
                                Directory.Delete(tempPath, true);
                            }
                            else
                            {
                                Directory.CreateDirectory(installedModule.Path);
                                szExtractor.ExtractArchive(installedModule.Path);
                            }
                            config.InstalledModules.Add(installedModule);
                        }
                        File.Delete(tempFileName);
                    }
                    break;
                case ModuleType.list:
                    using (var wc = new WebClient())
                    {
                        installedModule = module.Clone();
                        installedModule.Path = Path.Combine(userModDir, installedModule.IdHmod);
                        if (Directory.Exists(installedModule.Path)) Directory.Delete(installedModule.Path, true);
                        Directory.CreateDirectory(installedModule.Path);
                        var fileList = wc.DownloadString(module.Path).Replace("\r","").Split(new char[] { '\n' });
                        var downloadFolder = fileList[0];
                        for(int i = 1; i < fileList.Length; ++i)
                        {
                            var destFile = Path.Combine(installedModule.Path, fileList[i]);
                            if (!Directory.Exists(Path.GetDirectoryName(destFile)))
                                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                            wc.DownloadFile(Path.Combine(downloadFolder, fileList[i]), destFile);
                        }
                        config.InstalledModules.Add(installedModule);
                    }
                    break;
            }
        }

        private void ModuleLibraryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveConfig();
        }

        private void saveConfig()
        {
            XmlSerializer x = new XmlSerializer(typeof(LibraryConfig));
            using (var fs = new FileStream(configPath, FileMode.Create))
            {
                x.Serialize(fs, config);
            }
        }

        private void updateRepoButton_Click(object sender, EventArgs e)
        {
            int selectedRepo = repositoryListBox.SelectedIndex;
            updateRepos();
            loadRepositoryList();
            repositoryListBox.SelectedIndex = selectedRepo;
        }
    }
}
