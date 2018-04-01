using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using com.clusterrr.hakchi_gui.Properties;
using com.clusterrr.hakchi_gui.module_library;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace com.clusterrr.hakchi_gui
{
    public partial class ModStore : Form
    {
        private ModStoreManager config = new ModStoreManager();
        private Module currentModule { get; set; }

        #region Form Initialisation
        public ModStore()
        {
            InitializeComponent();
            this.Icon = Resources.icon;
            PoweredByLinkS.Text = "Powered By HakchiResources.com";
        }

        private void ModStore_Load(object sender, EventArgs e)
        {
            //Load Config
            XmlSerializer xs = new XmlSerializer(typeof(ModStoreManager));
            if (File.Exists(config.ConfigPath))
            {
                using (var fs = File.Open(config.ConfigPath, FileMode.Open))
                {
                    config = (ModStoreManager)xs.Deserialize(fs);
                }
            }

            if (config.AvailableModules.Count == 0 || (DateTime.Now - config.LastUpdate).TotalDays >= 1.0)
            {
                //Ask user to update repository information
                updateModuleList();
            }

            //Set Control Config
            foreach(TabPage tabPage in tabControl1.Controls)
            {
                ((ModStoreTabControl)tabPage.Controls[0]).Manager = config;
            }
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

                        //Set Module Type
                        var extention = module.Path.Substring(module.Path.LastIndexOf('.') + 1).ToLower();
                        if (extention.Equals("hmod"))
                            module.Type = ModuleType.hmod;
                        else if (extention.Equals("zip") || extention.Equals("7z") || extention.Equals("rar"))
                            module.Type = ModuleType.compressedFile;
                        else
                            continue; //Unknown File Type

                        //Set Categories
                        foreach(var category in post["categories"])
                        {
                            module.Categories.Add(category["slug"].ToString());
                        }

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
                            config.DownloadModule(modulesToUpdate[i]);
                            progressBarForm.UpdateProgress();
                        }
                    });
                    MessageBox.Show(this, "Finished updating modules.");
                }
            }
        }

        private void ModStore_FormClosing(object sender, FormClosingEventArgs e)
        {
            config.SaveConfig();
        }
        #endregion

    }
}