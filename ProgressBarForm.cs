using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    public partial class ProgressBarForm : Form
    {
        public ProgressBarForm()
        {
            InitializeComponent();
        }

        public ProgressBarForm(string title, int progressBarSize)
        {
            InitializeComponent();
            Text = title;
            progressBar1.Maximum = progressBarSize;
        }

        public void UpdateProgress(int increment = 1)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { UpdateProgress(increment); });
            else
                progressBar1.Value += increment;
        }

        public void CloseForm()
        {
            if (InvokeRequired)
                Invoke((MethodInvoker)delegate { Close(); });
            else
                Close();

        }

        public void Run(System.Threading.ThreadStart function)
        {
            System.Threading.Thread thread = new System.Threading.Thread(() => { function(); CloseForm(); });
            thread.Start();
            ShowDialog();
        }
    }
}
