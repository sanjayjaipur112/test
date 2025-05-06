using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;
using WASender.Model;

namespace WASender
{
    public partial class AddCaption : MyMaterialPopOp
    {

        WaSenderForm waSenderForm;
        CaptionModel captionModel;
        public AddCaption(WaSenderForm _waSenderForm, CaptionModel _captionModel)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.waSenderForm = _waSenderForm;
            this.captionModel = _captionModel;
        }

        private void AddCaption_Load(object sender, EventArgs e)
        {
            initLanguages();
            if (captionModel.IsAttachwithMainMessage == true)
            {
                materialCheckbox1.Checked = true;
                txtLongMessage.Text = "";
            }
            txtLongMessage.Text = captionModel.Caption;
        }

        private void initLanguages()
        {
            this.Text = Strings.AddCaption;
            txtLongMessage.Hint = Strings.TypeYourMessagehere;
            materialButton2.Text = Strings.Save;
            contextMenuStrip1.Items[0].Text = Strings.AddKeyMarkers;
            contextMenuStrip1.Items[1].Text = Strings.RandomNumber;
            materialCheckbox1.Text = Strings.AttachWithMainMessage;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            if (materialCheckbox1.Checked)
                txtLongMessage.Text = "";

            this.waSenderForm.AddCaptionFReturn(txtLongMessage.Text, materialCheckbox1.Checked);
            this.Close();

        }

        private void materialButton13_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(materialButton13, new Point(0, materialButton13.Height));
        }

        private void keyMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeyMarker keyMarker = new KeyMarker(this);
            keyMarker.ShowDialog();
        }
        public void AddKeyMarker(string KeyMarker)
        {
            txtLongMessage.Text += KeyMarker;
        }

        private void randomNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtLongMessage.Text += "{{ RANDOM }}";
        }

        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckbox1.Checked == true)
            {
                txtLongMessage.Enabled = false;
                materialButton13.Enabled = false;
            }
            else
            {
                txtLongMessage.Enabled = true;
                materialButton13.Enabled = true;
            }
        }

    }
}
