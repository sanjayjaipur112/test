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
    public partial class Activate : MyMaterialPopOp
    {

        Logger logger;
        WaSenderForm waSenderForm;
        public Activate(WaSenderForm _waSenderForm)
        {
            this.waSenderForm = _waSenderForm;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            logger = new Logger("Activator");
        }

        private void Activate_Load(object sender, EventArgs e)
        {
            init();
        }

        private void init()
        {
            this.Text = Strings.ActivateAppName +" "  + Strings.AppName + " " + Strings.SoftwareVersion;
            lblActivationCode.Text = Strings.YourActivationCodeis;
            label1.Text = Strings.ProvideYourActivationKeyHere;
            btnActivate.Text = Strings.ActivateNow;
            try
            {
                logger.WriteLog("FingerPrint_Value=" + Security.FingerPrint.Value());
                txtActivationCode.Text = Security.FingerPrint.Value();
            }
            catch (Exception ex)
            {
                txtActivationCode.Text = "C4E8-DA5B-1FD4-128D-6CAB-1BA1-55CC-EB5D";

            }
            materialButton1.Text = Strings.Exit;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            logger.Complete();
            Environment.Exit(1);
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            try
            {
                WASender.Models.ActivationModel obj = KeySecurity.KeySecurity.VerifyActivationCode(txtKey.Text);
                if (Strings.PurchaseCode != "")
                {
                    if (obj.purchasecode != Strings.PurchaseCode)
                    {
                        MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.InvalidPurchaseCode + ". " + Strings.DiffrentPurchaseCode, Strings.OK, true);
                        SnackBarMessage.Show(this);
                        return;
                    }
                }
                string keyCode = Config.Base64Decode(obj.ActivationCode);
                if (txtActivationCode.Text == keyCode || keyCode == "masterkey")
                {
                    if (obj.EndDate < DateTime.Now)
                    {
                        MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.InvalidActivationKey, Strings.OK, true);
                        SnackBarMessage.Show(this);
                    }
                    else
                    {
                        MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.ActivationSuccessfull, Strings.OK, true);
                        SnackBarMessage.Show(this);
                        var NewjsonString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                        Config.ActivateProduct(Config.Base64Encode(NewjsonString));
                        this.Hide();
                        waSenderForm.Show();
                        logger.Complete();
                    }
                }
                else
                {
                    MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.InvalidActivationKey, Strings.OK, true);
                    SnackBarMessage.Show(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

        }



        private void Activate_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(1);
            logger.Complete();
        }
    }
}
