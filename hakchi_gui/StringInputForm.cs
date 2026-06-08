using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace com.clusterrr.hakchi_gui
{
    public partial class StringInputForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Value { get => textBox.Text; set => textBox.Text = value; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Comments { get => labelComments.Text; set => labelComments.Text = value; }
        public StringInputForm() => InitializeComponent();

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
