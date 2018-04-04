using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using com.clusterrr.hakchi_gui.module_library;

namespace com.clusterrr.hakchi_gui
{
    public partial class ModStoreTabControl : UserControl
    {
        [Category("Data")]
        public string Category { get; set; }
        public ModStoreManager Manager { set { manager = value; loadModuleList(); } }

        private Module currentModule { get; set; }
        private ModStoreManager manager;
        private List<Module> moduleList = new List<Module>();

        public ModStoreTabControl()
        {
            InitializeComponent();
        }

        #region GUI
        private void loadModuleDescription()
        {
            var installedModule = manager.GetInstalledModule(currentModule);
            webBrowser1.AllowNavigation = true;
            webBrowser1.Url = new Uri(currentModule.Description, UriKind.Absolute);

            moduleDescriptionBrowser.DocumentText = String.Format("<html style='background-color:#d20014;color:#ffffff;'>" +
                                                                    "<body background='https://hakchiresources.com/wp-content/uploads/2018/04/bg-1.png' style='width:273px;'>" +
                                                                         "<span style='font-family: Arial, Helvetica, sans-serif;'>" +
                                                                              "<b>Module Name:</b><br /><span style='font-size:75%;'>{0}</span><br />" +
                                                                              "<b>Author:</b><br /><span style='font-size:75%;'>{1}</span><br />" +
                                                                              "<b>Latest Version:</b><br /><span style='font-size:75%;'>{2}</span><br />" +
                                                                              "<b>Installed Version:</b><br /><span style='font-size:75%;'>{3}</span>" +
                                                                          "</span>" +
                                                                    "</body>" +
                                                                  "</html>",
                                                                  currentModule.Name,
                                                                  currentModule.Author,
                                                                  currentModule.Version,
                                                                  (installedModule != null) ? installedModule.Version : "N/A");
            
            if (installedModule != null)
            {
                if (installedModule.Version != currentModule.Version)
                {
                    moduleDownloadButton.Enabled = true;
                    moduleDownloadButton.Text = "Update Module";
                    moduleDownloadInstallButton.Text = "Update and Install Module";
                }
                else
                {
                    moduleDownloadButton.Enabled = false;
                    moduleDownloadButton.Text = "Module Up-To-Date";
                    moduleDownloadInstallButton.Text = "Install Module";
                }
            }
            else
            {
                moduleDownloadButton.Enabled = true;
                moduleDownloadButton.Text = "Download Module";
                moduleDownloadInstallButton.Text = "Download and Install Module";
            }
        }

        private void moduleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = moduleListBox.SelectedIndex;
            if (index != -1)
            {
                if (currentModule != moduleList[index])
                {
                    webBrowser1.Navigate(new Uri("about:blank"));
                    currentModule = moduleList[index];
                    loadModuleDescription();
                }
            }
            else
                currentModule = null;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.AllowNavigation = false;
        }
        #endregion

        #region Mod Manager Code
        private void loadModuleList()
        {
            moduleListBox.Items.Clear();
            moduleList.Clear();
            foreach (var module in manager.AvailableModules)
            {
                if (module.Categories.Contains(Category))
                {
                    moduleListBox.Items.Add(module.Name);
                    moduleList.Add(module);
                }
            }
            moduleListBox.SelectedIndex = 0;
        }

        private void moduleDownloadButton_Click(object sender, EventArgs e)
        {
            manager.DownloadModule(currentModule);
            loadModuleDescription();
        }

        private void moduleDownloadInstallButton_Click(object sender, EventArgs e)
        {
            InstalledModule installedModule = manager.GetInstalledModule(currentModule);

            //Download or update module
            if(installedModule == null || installedModule.Version != currentModule.Version)
            {
                moduleDownloadButton_Click(this, new EventArgs());
                installedModule = manager.GetInstalledModule(currentModule);
            }

            if(installedModule != null)
            {
                List<string> mods = new List<string>();
                foreach (var file in installedModule.InstalledFiles)
                {
                    if(file.EndsWith(".hmod"))
                    {
                        mods.Add(file.Substring(0, file.Length - 5));
                    }
                    else if(file.EndsWith(".hmod\\"))
                    {
                        mods.Add(file.Substring(0, file.Length - 6));
                    }
                }
                MainForm mainForm = Application.OpenForms[0] as MainForm;
                mainForm.InstallMods(mods.ToArray());
            }
        }
        #endregion
    }
}
