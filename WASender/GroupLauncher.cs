using Models;
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
using WASender.Models;

namespace WASender
{
    public partial class GroupLauncher : MyMaterialPopOp
    {
        WaSenderForm waSenderForm;
        GeneralSettingsModel generalSettingsModel;
        WASenderGroupTransModel wASenderGroupTransModel;
        SchedulesModel schedulesModel;
        public GroupLauncher(WASenderGroupTransModel _wASenderGroupTransModel, WaSenderForm _waSenderForm, SchedulesModel _schedulesModel = null)
        {
            InitializeComponent();
            this.wASenderGroupTransModel = _wASenderGroupTransModel;
            this.waSenderForm = _waSenderForm;
            this.Icon = Strings.AppIcon;
            schedulesModel = _schedulesModel;
        }

        private void GroupLauncher_Load(object sender, EventArgs e)
        {
            initLanguages();

            if (wASenderGroupTransModel.messages.Where(x => x != null).Count() >= 2)
            {
                Dictionary<string, string> test = new Dictionary<string, string>();
                test.Add("1", Strings.SendAllMessagestoeachnumber);
                test.Add("2", Strings.RotateMessages);

                materialComboBox1.DataSource = new BindingSource(test, null);
                materialComboBox1.DisplayMember = "Value";
                materialComboBox1.ValueMember = "Key";

                materialComboBox1.Enabled = true;
            }
            else
            {
                materialComboBox1.Enabled = false;
                wASenderGroupTransModel.IsRotateMessages = false;
            }

            fillData();
        }

        private void fillData()
        {
            materialTextBox21.Text = wASenderGroupTransModel.CampaignName;
            if (wASenderGroupTransModel.IsRotateMessages)
            {
                materialComboBox1.SelectedValue = "2";
            }
            if (schedulesModel != null)
            {
                materialButton1.Enabled = false;
            }
        }

        private void initLanguages()
        {
            this.Text = Strings.Launch;
            materialTextBox21.Hint = Strings.CampaignName;
            groupBox2.Text = Strings.MultiMessagingMode;
            materialButton1.Text = Strings.StartNow;
            //materialCheckbox1.Text = Strings.TagAllMemberswithmessage;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            try
            {
                wASenderGroupTransModel.CampaignName = materialTextBox21.Text;
                if (wASenderGroupTransModel.messages.Where(x => x != null).Count() >= 2)
                {
                    if (materialComboBox1.SelectedValue == "1")
                    {
                        wASenderGroupTransModel.IsRotateMessages = false;
                    }
                    else if (materialComboBox1.SelectedValue == "2")
                    {
                        wASenderGroupTransModel.IsRotateMessages = true;
                    }
                }
                wASenderGroupTransModel.tagAll = false;
            }
            catch (Exception ex)
            {

            }

            RunGroup run = new RunGroup(wASenderGroupTransModel, waSenderForm);
            run.Show();
            this.Hide();
            waSenderForm.Hide();
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1) " + Strings.SendAllMessagestoeachnumber + "\n2) " + Strings.RotateMessages + " " + Strings.Randomly + "\n" + Strings.Thisfieldstasdesabledifyouenteredonlyonemessage, Strings.MultiMessagingMode, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            try
            {
                wASenderGroupTransModel.CampaignName = materialTextBox21.Text;
                if (wASenderGroupTransModel.messages.Where(x => x != null).Count() >= 2)
                {
                    if (materialComboBox1.SelectedValue == "1")
                    {
                        wASenderGroupTransModel.IsRotateMessages = false;
                    }
                    else if (materialComboBox1.SelectedValue == "2")
                    {
                        wASenderGroupTransModel.IsRotateMessages = true;
                    }
                }
                wASenderGroupTransModel.tagAll = false;
            }
            catch (Exception ex)
            {

            }

            wASenderGroupTransModel.generalSettingsModel = Config.GetSettings();

            DataTable dt= new SqLiteBaseRepository().ReadData(true);
            var sessionId = dt.Rows[0]["sesionId"].ToString();
            wASenderGroupTransModel.sessionId = sessionId;

            ScheduleSingle scheduler = new ScheduleSingle(wASenderGroupTransModel, this, this.waSenderForm, schedulesModel == null ? null : schedulesModel.Id, schedulesModel == null ? null : schedulesModel.ScheduleName);
            scheduler.ShowDialog();
            if (waSenderForm != null)
            {
                waSenderForm.clearAllGroup();
                this.Close();
            }
            
        }
    }
}
