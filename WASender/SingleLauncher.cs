using MaterialSkin.Controls;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;
using WASender.Models;

namespace WASender
{
    public partial class SingleLauncher : MyMaterialPopOp
    {
        WASenderSingleTransModel wASenderSingleTransModel;
        WaSenderForm waSenderForm;
        GeneralSettingsModel generalSettingsModel;
        SchedulesModel schedulesModel;
        List<string> friendlyList;
        int friendlyListSendAfter = 0;
        public SingleLauncher(WASenderSingleTransModel _wASenderSingleTransModel, WaSenderForm _waSenderForm, SchedulesModel _schedulesModel = null)
        {
            InitializeComponent();
            wASenderSingleTransModel = _wASenderSingleTransModel;
            waSenderForm = _waSenderForm;
            this.Icon = Strings.AppIcon;
            generalSettingsModel = Config.GetSettings();

            materialTextBox21.Text = wASenderSingleTransModel.CampaignName;
            schedulesModel = _schedulesModel;
        }

        private void SingleLauncher_Load(object sender, EventArgs e)
        {
            initLanguages();
            init();

            if (wASenderSingleTransModel.messages.Where(x => x != null).Count() >= 2)
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
                wASenderSingleTransModel.IsRotateMessages = false;
            }

            filldetals();
            checkforCheckboxes();
            if (schedulesModel != null)
            {
                materialButton1.Enabled = false;
            }
        }

        private void filldetals()
        {
            materialTextBox21.Text = wASenderSingleTransModel.CampaignName;

            if (wASenderSingleTransModel.selectedAccounts != null)
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataRow _tag = (DataRow)row.Tag;
                    string _sesionId = _tag["sesionID"].ToString();
                    if (wASenderSingleTransModel.selectedAccounts.Where(x => x.sesionID == _sesionId).Count() > 0)
                    {
                        row.Cells[0].Value = true;
                    }

                }
            }

            if (wASenderSingleTransModel.IsRotateMessages)
            {
                materialComboBox1.SelectedValue = "2";
            }
            if (wASenderSingleTransModel.swipeAccountAfterMessages != null && wASenderSingleTransModel.swipeAccountAfterMessages > 0)
            {
                materialTextBox22.Text = wASenderSingleTransModel.swipeAccountAfterMessages.ToString();
            }
            if (wASenderSingleTransModel.sendTofriendlyNumbersAfterMessages > 0)
            {
                materialCheckbox3.Checked = true;

                friendlyList = wASenderSingleTransModel.friendlyNumbers;
                friendlyListSendAfter = wASenderSingleTransModel.sendTofriendlyNumbersAfterMessages;
            }

        }

        private void init()
        {
            if (generalSettingsModel.browserType == 1)
            {
                dataGridView1.Hide();
                materialTextBox22.Hide();
            }
            else
            {
                dataGridView1.Show();
                materialTextBox22.Show();

                dataGridView1.Rows.Clear();
                DataTable dt = new SqLiteBaseRepository().ReadData();
                try
                {
                    dt.DefaultView.Sort = "isDefault desc";
                    dt = dt.DefaultView.ToTable();
                }
                catch (Exception ex)
                {

                }

                foreach (DataRow item in dt.Rows)
                {
                    dataGridView1.Rows.Add(new object[]{
                        item["isDefault"].ToString()=="1" ?true:false,
                    item["sessionName"].ToString() + (   item["isDefault"].ToString()=="1"? " ("+ Strings.Primary+")" : "" )
                });
                    dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
                }

                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if ((bool)item.Cells["Column1"].Value == true)
                    {
                    }
                }
            }


        }

        private void initLanguages()
        {
            this.Text = Strings.Launch;
            materialTextBox21.Hint = Strings.CampaignName;
            materialButton1.Text = Strings.StartNow;
            dataGridView1.Columns[0].HeaderText = Strings.Select;
            dataGridView1.Columns[1].HeaderText = Strings.AccountName;
            materialTextBox22.Hint = Strings.SwipeAccountaftermessages;
            groupBox1.Text = Strings.SendingMode;
            materialCheckbox1.Text = Strings.SafeMode;
            materialCheckbox2.Text = Strings.UnSafeMode;
            groupBox2.Text = Strings.MultiMessagingMode;
            materialButton4.Text = Strings.Schedule;
            materialCheckbox3.Text = Strings.EnableFriendlyNumbers;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (!prepareCampaign())
            {
                RunSingle run = new RunSingle(wASenderSingleTransModel, waSenderForm);
                waSenderForm.Hide();
                this.Hide();
                run.Show();
            }

        }

        private bool prepareCampaign()
        {
            if (Utils.waSenderBrowser != null)
            {
                Utils.waSenderBrowser.Close();
                Utils.waSenderBrowser = null;
            }

            if (friendlyList != null && friendlyList.Count() > 0 && friendlyListSendAfter > 0 && materialCheckbox3.Checked)
            {
                this.waSenderForm.receiveFriendlyNumbersData(friendlyList, friendlyListSendAfter);

                var chunks = wASenderSingleTransModel.contactList.Chunk<ContactModel>(friendlyListSendAfter).ToList();

                wASenderSingleTransModel.contactList = new List<ContactModel>();

                foreach (var chunk in chunks)
                {
                    List<ContactModel> lisr = chunk.ToList();
                    wASenderSingleTransModel.contactList.AddRange(lisr);
                    ContactModel contact;
                    foreach (string fNumber in friendlyList)
                    {
                        contact = new ContactModel();
                        contact.number = fNumber;
                        contact.isFriendly = true;
                        contact.sendStatusModel = new SendStatusModel { isDone = false };

                        wASenderSingleTransModel.contactList.Add(contact);

                    }

                }
            }

            List<ConnectedAccountModel> ConnectedAccountModelList = new List<ConnectedAccountModel>();
            if (generalSettingsModel.browserType == 2)
            {
                ConnectedAccountModel connectedAccountModel;
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    if (Convert.ToBoolean(item.Cells[0].Value) == true)
                    {
                        DataRow dr = (DataRow)item.Tag;
                        string sesionID = dr["sesionID"].ToString();
                        string ID = dr["ID"].ToString();
                        string sessionName = dr["sessionName"].ToString();

                        connectedAccountModel = new ConnectedAccountModel();
                        connectedAccountModel.ID = ID;
                        connectedAccountModel.sessionName = sessionName;
                        connectedAccountModel.sesionID = sesionID;
                        ConnectedAccountModelList.Add(connectedAccountModel);
                    }
                }
                if (ConnectedAccountModelList.Count() == 0)
                {
                    MaterialDialog materialDialog = new MaterialDialog(this, Strings.Select, Strings.Pleaseselectatleastoneaccount, Strings.OK, false, Strings.Cancel);
                    DialogResult result = materialDialog.ShowDialog(this);
                    {
                        return true;
                    }

                }
            }


            try
            {
                if (wASenderSingleTransModel.messages.Where(x => x != null).Count() >= 2)
                {
                    if (materialComboBox1.SelectedValue == "1")
                    {
                        wASenderSingleTransModel.IsRotateMessages = false;
                    }
                    else if (materialComboBox1.SelectedValue == "2")
                    {
                        wASenderSingleTransModel.IsRotateMessages = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }

            wASenderSingleTransModel.selectedAccounts = ConnectedAccountModelList;
            try
            {
                wASenderSingleTransModel.swipeAccountAfterMessages = Convert.ToInt32(materialTextBox22.Text);
            }
            catch (Exception ex)
            {
                wASenderSingleTransModel.swipeAccountAfterMessages = 2;
            }

            if (materialCheckbox1.Checked)
            {
                wASenderSingleTransModel.isSafeMode = true;
            }
            else
            {
                wASenderSingleTransModel.isSafeMode = false;
            }

            wASenderSingleTransModel.CampaignName = materialTextBox21.Text;




            return false;
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);
                ControlPaint.DrawCheckBox(e.Graphics, e.CellBounds.X + 1, e.CellBounds.Y + 1,
                    e.CellBounds.Width - 2, e.CellBounds.Height - 2,
                    (bool)e.FormattedValue ? ButtonState.Checked : ButtonState.Normal);
                e.Handled = true;
            }
        }



        private void materialButton2_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(Strings.SafeModeData1 + "\n\n" + Strings.SafeModeData2 + "\n\n" + Strings.SafeModeData3, Strings.SafeMode, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private void checkforCheckboxes()
        {
            int totalAccountSelected = 0;
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(item.Cells[0].Value) == true)
                {
                    totalAccountSelected++;
                }
            }
            if (totalAccountSelected > 1)
            {
                groupBox1.Enabled = false;
                materialTextBox22.Enabled = true;
                materialCheckbox1.Checked = false;
                materialCheckbox2.Checked = true;
            }
            else if (totalAccountSelected == 1)
            {
                groupBox1.Enabled = true;
                materialTextBox22.Enabled = false;
            }

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0)
                this.dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            checkforCheckboxes();
        }

        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckbox1.Checked)
            {
                materialCheckbox2.Checked = false;
            }
        }

        private void materialCheckbox2_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckbox2.Checked)
            {
                materialCheckbox1.Checked = false;
            }
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1) " + Strings.SendAllMessagestoeachnumber + "\n2) " + Strings.RotateMessages + " " + Strings.Randomly + "\n" + Strings.Thisfieldstasdesabledifyouenteredonlyonemessage, Strings.MultiMessagingMode, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            if (!prepareCampaign())
            {
                ScheduleSingle scheduler = new ScheduleSingle(wASenderSingleTransModel, this, waSenderForm, this.schedulesModel == null ? null : this.schedulesModel.Id, this.schedulesModel == null ? null : this.schedulesModel.ScheduleName);
                scheduler.ShowDialog();
                if (waSenderForm != null)
                {
                    waSenderForm.clearAll();
                    this.Close();
                }
            }
        }



        public void receiveFriendlyListData(List<string> numbers, int count)
        {
            materialCheckbox3.Checked = true;
            friendlyList = numbers;
            friendlyListSendAfter = count;
            this.waSenderForm.receiveFriendlyNumbersData(friendlyList, friendlyListSendAfter);
        }

        private void materialCheckbox3_Click(object sender, EventArgs e)
        {
            if (materialCheckbox3.Checked)
            {
                materialCheckbox3.Checked = false;
                FriendlyNumbers form = new FriendlyNumbers(this, wASenderSingleTransModel.sendTofriendlyNumbersAfterMessages);
                form.Show();
            }

        }
    }
}
