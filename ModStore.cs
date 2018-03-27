using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using com.clusterrr.hakchi_gui.Properties;
using System.Diagnostics;

namespace com.clusterrr.hakchi_gui
{
    public partial class ModStore : Form
    {
        public ModStore()
        {
            InitializeComponent();
            this.Icon = Resources.icon;
            this.Text = String.Format("About {0}", AssemblyTitle);
            PoweredByLinkS.Text = "Powered By HakchiResources.com";
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        #endregion

        private void PoweredByLinkS_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.hakchiresources.com");
        }

        private void visitWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.hakchiresources.com");
        }
    }
}