namespace com.clusterrr.hakchi_gui
{
    partial class ModStore
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.refreshContentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.visitWebsiteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.PoweredByLinkS = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.moduleListBox = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.moduleDownloadButton = new System.Windows.Forms.Button();
            this.moduleDescriptionBrowser = new System.Windows.Forms.WebBrowser();
            this.moduleDownloadInstallButton = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.menuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshContentToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(752, 24);
            this.menuStrip.TabIndex = 2;
            // 
            // refreshContentToolStripMenuItem
            // 
            this.refreshContentToolStripMenuItem.Name = "refreshContentToolStripMenuItem";
            this.refreshContentToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.refreshContentToolStripMenuItem.Text = "Refresh";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.visitWebsiteToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            // 
            // visitWebsiteToolStripMenuItem
            // 
            this.visitWebsiteToolStripMenuItem.Name = "visitWebsiteToolStripMenuItem";
            this.visitWebsiteToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.visitWebsiteToolStripMenuItem.Text = "Visit Website";
            this.visitWebsiteToolStripMenuItem.Click += new System.EventHandler(this.visitWebsiteToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PoweredByLinkS});
            this.statusStrip1.Location = new System.Drawing.Point(0, 439);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip1.Size = new System.Drawing.Size(752, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // PoweredByLinkS
            // 
            this.PoweredByLinkS.IsLink = true;
            this.PoweredByLinkS.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.PoweredByLinkS.Name = "PoweredByLinkS";
            this.PoweredByLinkS.Size = new System.Drawing.Size(94, 17);
            this.PoweredByLinkS.Text = "PoweredByLinkS";
            this.PoweredByLinkS.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(0)))), ((int)(((byte)(20)))));
            this.PoweredByLinkS.Click += new System.EventHandler(this.PoweredByLinkS_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(752, 414);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.moduleListBox);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.webBrowser1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(744, 388);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            // 
            // moduleListBox
            // 
            this.moduleListBox.FormattingEnabled = true;
            this.moduleListBox.Location = new System.Drawing.Point(3, 4);
            this.moduleListBox.Name = "moduleListBox";
            this.moduleListBox.Size = new System.Drawing.Size(183, 381);
            this.moduleListBox.TabIndex = 0;
            this.moduleListBox.SelectedIndexChanged += new System.EventHandler(this.moduleListBox_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.moduleDownloadButton);
            this.panel1.Controls.Add(this.moduleDescriptionBrowser);
            this.panel1.Controls.Add(this.moduleDownloadInstallButton);
            this.panel1.Location = new System.Drawing.Point(190, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(215, 381);
            this.panel1.TabIndex = 2;
            // 
            // moduleDownloadButton
            // 
            this.moduleDownloadButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.moduleDownloadButton.Enabled = false;
            this.moduleDownloadButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.moduleDownloadButton.Location = new System.Drawing.Point(0, 287);
            this.moduleDownloadButton.Name = "moduleDownloadButton";
            this.moduleDownloadButton.Size = new System.Drawing.Size(213, 46);
            this.moduleDownloadButton.TabIndex = 7;
            this.moduleDownloadButton.Text = "Download Module";
            this.moduleDownloadButton.UseVisualStyleBackColor = true;
            this.moduleDownloadButton.Click += new System.EventHandler(this.moduleDownloadButton_Click);
            // 
            // moduleDescriptionBrowser
            // 
            this.moduleDescriptionBrowser.AllowWebBrowserDrop = false;
            this.moduleDescriptionBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moduleDescriptionBrowser.IsWebBrowserContextMenuEnabled = false;
            this.moduleDescriptionBrowser.Location = new System.Drawing.Point(0, 0);
            this.moduleDescriptionBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.moduleDescriptionBrowser.Name = "moduleDescriptionBrowser";
            this.moduleDescriptionBrowser.ScriptErrorsSuppressed = true;
            this.moduleDescriptionBrowser.ScrollBarsEnabled = false;
            this.moduleDescriptionBrowser.Size = new System.Drawing.Size(213, 333);
            this.moduleDescriptionBrowser.TabIndex = 6;
            this.moduleDescriptionBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // moduleDownloadInstallButton
            // 
            this.moduleDownloadInstallButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.moduleDownloadInstallButton.Enabled = false;
            this.moduleDownloadInstallButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.moduleDownloadInstallButton.Location = new System.Drawing.Point(0, 333);
            this.moduleDownloadInstallButton.Name = "moduleDownloadInstallButton";
            this.moduleDownloadInstallButton.Size = new System.Drawing.Size(213, 46);
            this.moduleDownloadInstallButton.TabIndex = 5;
            this.moduleDownloadInstallButton.Text = "Download and Install Module";
            this.moduleDownloadInstallButton.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowNavigation = false;
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Right;
            this.webBrowser1.Location = new System.Drawing.Point(409, 3);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(332, 382);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.Url = new System.Uri("https://hakchiresources.com/2018/03/19/hakchi-advanced-music-hack/?mode=mod_store" +
        "", System.UriKind.Absolute);
            this.webBrowser1.WebBrowserShortcutsEnabled = false;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(744, 388);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ModStore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 461);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModStore";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ModStore";
            this.Load += new System.EventHandler(this.ModStore_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem visitWebsiteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshContentToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel PoweredByLinkS;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.ListBox moduleListBox;
        private System.Windows.Forms.Button moduleDownloadInstallButton;
        private System.Windows.Forms.WebBrowser moduleDescriptionBrowser;
        private System.Windows.Forms.Button moduleDownloadButton;
    }
}