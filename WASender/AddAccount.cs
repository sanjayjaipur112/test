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
    public partial class AddAccount : MyMaterialPopOp
    {
        ManageAccounts manageAccounts;
        public AddAccount(ManageAccounts _manageAccounts)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            manageAccounts = _manageAccounts;
        }

        private void AddAccount_Load(object sender, EventArgs e)
        {
            initLanguages();
        }

        private void initLanguages()
        {
            this.Text = Strings.AddNewAccount;
            materialTextBox21.Hint = Strings.AccountName;
            materialButton1.Text = Strings.Add;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialTextBox21.Text != "")
            {
                 if (Utils.waSenderBrowser != null)
            {
                Utils.waSenderBrowser.Close();
            }

                try
                {
                    DataTable dt = new SqLiteBaseRepository().ReadDataExists(materialTextBox21.Text);
                    if (dt.Rows.Count > 0)
                    {
                        MaterialSkin.Controls.MaterialMessageBox.Show("Error - " + Strings.SameNameAlreadyExists, false, MaterialSkin.Controls.FlexibleMaterialForm.ButtonsPosition.Right);

                    }
                    else
                    {
                        new SqLiteBaseRepository().AddSession(materialTextBox21.Text);
                        MaterialSkin.Controls.MaterialMessageBox.Show(Strings.AccountAddedSuccessfully, false, MaterialSkin.Controls.FlexibleMaterialForm.ButtonsPosition.Right);
                        manageAccounts.loadData();
                        this.Close();
                    }
                    
                }
                catch (Exception ex)
                {
                    MaterialSkin.Controls.MaterialMessageBox.Show("Error - " + ex.Message, false, MaterialSkin.Controls.FlexibleMaterialForm.ButtonsPosition.Right);
                }
            }
        }
    }
}
