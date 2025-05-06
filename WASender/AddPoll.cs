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
    public partial class AddPoll : MyMaterialPopOp
    {
        WaSenderForm waSenderForm;
        AddMessage addMessage; 
        PollModel pollModel;

        public AddPoll(PollModel model, AddMessage _addMessage)
        {
            pollModel = model;
            addMessage = _addMessage;
            InitializeComponent();
        }
        
        public AddPoll(PollModel model, WaSenderForm _waSenderForm)
        {
            pollModel = model;
            waSenderForm = _waSenderForm;
            InitializeComponent();
        }

        private void AddPoll_Load(object sender, EventArgs e)
        {
            this.Icon = Strings.AppIcon;
            initLanguages();
            loadIfedit();

            Dictionary<string, string> test = new Dictionary<string, string>();
            test.Add("1", Strings.SingleSelect);
            test.Add("2", Strings.MultiSelect);

            materialComboBox1.DataSource = new BindingSource(test, null);
            materialComboBox1.DisplayMember = "Value";
            materialComboBox1.ValueMember = "Key";

            if (pollModel.selectableCount != 1)
            {
                materialComboBox1.SelectedValue = "2";
            }
        }

        private void loadIfedit()
        {
            if (pollModel.editMode == true)
            {
                btnDelete.Show();

                materialTextBox21.Text = pollModel.PollName;
                int counter = 0;
                foreach (string item in pollModel.Options)
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[counter].Cells[0].Value = item;
                    counter++;
                }
            }
            else
            {
                btnDelete.Hide();
            }
        }

        private void initLanguages()
        {
            this.Text = Strings.AddPoll;
            materialTextBox21.Hint = Strings.PollName;
            btnSave.Text = Strings.Save;
            btnDelete.Text = Strings.Delete;
            dataGridView1.Columns[0].HeaderText = Strings.Options;
            materialLabel2.Text= Strings.selectableCount;
            materialLabel1.Text = Strings.Options;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            GenerateModel();
        }

        private void GenerateModel()
        {
            if (materialTextBox21.Text != "" && dataGridView1.Rows.Count > 0)
            {
                pollModel.PollName = materialTextBox21.Text;
                pollModel.id = Guid.NewGuid().ToString();
                pollModel.Options = new List<string>();

                try
                {
                    if (materialComboBox1.SelectedValue == "1")
                    {
                        pollModel.selectableCount = 1;
                    }
                    else
                    {
                        pollModel.selectableCount = 2;
                    }
                }
                catch (Exception ex)
                {
                    
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null)
                    {
                        string option = row.Cells[0].Value.ToString();
                        if (option != "")
                        {
                            pollModel.Options.Add(option);
                        }
                    }
                }

                var duplicateKeys = pollModel.Options.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();

                if (duplicateKeys.Count() > 0)
                {
                    MessageBox.Show(duplicateKeys.FirstOrDefault(), Strings.DuplicateValue);
                }
                else
                {
                    if (waSenderForm != null)
                    {
                        this.waSenderForm.RecievPoll(pollModel);
                    }
                    else if (addMessage != null)
                    {
                        addMessage.RecievPolls(pollModel);
                    }
                    this.Close();
                }
            }
            
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (waSenderForm != null)
            {
                waSenderForm.RemovePoll(pollModel);
            }
            else if (addMessage != null)
            {
                addMessage.RemovePoll(pollModel);
            }
            
            this.Close();
        }
    }
}
