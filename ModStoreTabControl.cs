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
            webBrowser1.AllowNavigation = true;
            webBrowser1.Url = new Uri(currentModule.Description, UriKind.Absolute);
            var installedModule = manager.GetInstalledModule(currentModule);
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

        private void moduleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = moduleListBox.SelectedIndex;
            if (index != -1)
            {
                if (currentModule != moduleList[index])
                {
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
            manager.SaveConfig();
        }
        #endregion
    }
}
