using MaterialSkin.Controls;
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

namespace WASender
{
    public partial class About : MyMaterialPopOp
    {
        GeneralSettings generalSettings;
        public About(GeneralSettings _generalSettings)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.generalSettings = _generalSettings;
            lblSoftwarename.Text = Strings.AppName;
            materialLabel2.Text = Strings.SoftwareVersion;
            DateTime? _date = Config.getEndDate();
            if (_date == null)
            {
                materialLabel5.Text = "Never";
            }
            else
            {
                materialLabel5.Text = _date.Value.Day.ToString() + "-" + _date.Value.ToString("MMM") + "-" + _date.Value.Year + " " + _date.Value.Hour + ":" + _date.Value.Minute;
            }

            materialButton2.Text = Strings.DeActivateLicence;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            MaterialDialog materialDialog = new MaterialDialog(this, Strings.Delete, Strings.AreyouSure, Strings.Yes, true, Strings.Cancel);
            DialogResult result = materialDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                Config.deActivateProduct();
                this.Hide();
                this.generalSettings.ProductDeActivated();
                
            }
        }

        private void About_Load(object sender, EventArgs e)
        {
            initLanguages();
        }

        private void initLanguages()
        {
            this.Text = Strings.AboutLicence;
            materialLabel1.Text = Strings.Version;
            materialLabel3.Text = Strings.LicenceInformation;
            materialLabel4.Text = Strings.Activated;
            materialLabel6.Text = Strings.LicenceExpiresOn;
            materialButton1.Text = Strings.OK;
        }
    }
}
