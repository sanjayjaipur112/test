using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;
using WASender.Models;

namespace WASender
{
    public partial class Warmer : Form
    {
        WarmerModel warmerModel;
        WaSenderForm waSenderForm;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public Warmer(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            intLanguages();
            warmerModel = new WarmerModel();
            this.Icon = Strings.AppIcon;
            stepWizardControl1.TitleIcon = Strings.AppIcon;
            waSenderForm = _waSenderForm;
        }

        private void intLanguages()
        {
            this.Text = Strings.WhatsAppWarmer;
            stepWizardControl1.Text = Strings.WhatsAppWarmer;
            stepWizardControl1.BackButtonToolTipText = Strings.Returnstoapreviouspage;
            stepWizardControl1.CancelButtonText = "&" + Strings.Cancel;
            stepWizardControl1.FinishButtonText = "&" + Strings.Finish;
            stepWizardControl1.NextButtonText = "&" + Strings.Next;
            stepWizardControl1.Pages[0].Text = Strings.SelectAccounts;
            stepWizardControl1.Pages[1].Text = Strings.Conversessions;
            stepWizardControl1.Pages[2].Text = Strings.DelaySettings;
            stepWizardControl1.SetStepText(stepWizardControl1.Pages[0], Strings.SelectAccounts);
            stepWizardControl1.SetStepText(stepWizardControl1.Pages[1], Strings.Conversessions);
            stepWizardControl1.SetStepText(stepWizardControl1.Pages[2], Strings.DelaySettings);    
        }

        private void stepWizardControl1_SelectedPageChanged(object sender, EventArgs e)
        {

        }



        private void Warmer_Load(object sender, EventArgs e)
        {
            initlanguages();
            bindData();
            string ss = File.ReadAllText("dataset.txt");
            string ds = Regex.Replace(ss, @"\r\n?|\n", Environment.NewLine);
            textBox1.Text = ds;
            stepWizardControl1.Location = new Point(
                 10,
                 this.stepWizardControl1.Location.Y
             );
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                this.Invoke(new Action(() =>
                    CheckForActivation()));
            });


            Dictionary<string, string> test = new Dictionary<string, string>();
            test.Add("1", Strings.OnetoOne);
            test.Add("2", Strings.OnetoMany);

            materialComboBox1.DataSource = new BindingSource(test, null);
            materialComboBox1.DisplayMember = "Value";
            materialComboBox1.ValueMember = "Key";
        }

        private void CheckForActivation()
        {
            pgbar = new Progressbar();
            this.backgroundWorker_productChecker = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker_productChecker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_productChecker_DoWork);
            this.backgroundWorker_productChecker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker2_RunWorkerCompleted); ;
            this.backgroundWorker_productChecker.RunWorkerAsync();
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            pgbar.Close();
            if (e.Cancelled)
            {
                MessageBox.Show("Operation was canceled");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Operation was canceled");
            }
            else
            {
                try
                {
                    bool mode = (bool)e.Result;
                    if (mode == false)
                    {
                        MessageBox.Show(Strings.ProductIsNotActivated, Strings.ProductIsNotActivated, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch (Exception ex)
                {

                }

            }
        }

        private void backgroundWorker_productChecker_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = CheckForActivationInternal();
        }

        private bool CheckForActivationInternal()
        {
            try
            {
                WPPHelper.CheckExecutingAssembly();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private void initlanguages()
        {
            this.Text = Strings.WhatsAppWarmer;
            stepWizardControl1.Text = Strings.Prepare + " " + Strings.WhatsAppWarmer;
            stepWizardControl1.Title = Strings.WhatsAppWarmer;
            wizardPage1.Text = Strings.SelectAccounts;
            wizardPage2.Text = Strings.Conversessions;
            wizardPage3.Text = Strings.DelaySettings;

            stepWizardControl1.Pages[0].Text = Strings.SelectAccounts;
            stepWizardControl1.Pages[1].Text = Strings.Conversessions;
            stepWizardControl1.Pages[2].Text = Strings.DelaySettings;


            materialButton1.Text = Strings.Clear;
            materialButton2.Text = Strings.Selecttextfile;

            label1.Text = Strings.Delaybetween;
            label2.Text = Strings.and;
            label3.Text = Strings.Seconds;

            stepWizardControl1.FinishButtonText = "&" + Strings.Finish;
            stepWizardControl1.CancelButtonText = "&" + Strings.Cancel;
            stepWizardControl1.NextButtonText = "&" + Strings.Next;

            dataGridView1.Columns[0].HeaderText = Strings.Select;
            dataGridView1.Columns[1].HeaderText = Strings.AccountName;
            label4.Text = Strings.WarmingMethod;

        }

        private async void bindData()
        {
            dataGridView1.Rows.Clear();
            DataTable dt = new SqLiteBaseRepository().ReadData();

            foreach (DataRow item in dt.Rows)
            {
                List<AccountReadyNessModel> accountReadyNessModelList = new List<AccountReadyNessModel>();
                
                dataGridView1.Rows.Add(new object[]{
                    false,
                    item["sessionName"].ToString(),
                });
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            bindData();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt;";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string selectedFileName = openFileDialog1.FileName;
            string ss = File.ReadAllText(selectedFileName);
            textBox1.Text = ss;
        }

       
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        private void wizardPage1_Commit(object sender, AeroWizard.WizardPageConfirmEventArgs e)
        {
            int selectedCount = 0;

            

            warmerModel.SelectedAccountNames = new List<WarmerContactModel>();


            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(item.Cells[0].Value) == true)
                {
                    DataRow dr = (DataRow)item.Tag;
                    string Id = dr["ID"].ToString();
                    warmerModel.SelectedAccountNames.Add(new WarmerContactModel
                    {
                        Name = item.Cells[1].Value.ToString(),
                        Number = "",
                        ID = Id
                    });
                    selectedCount++;

                }
            }
            if (selectedCount < 2)
            {
                e.Cancel = true;
                MessageBox.Show(Strings.PleaseSelectTWOormoreaccounts);
                return;
            }

        }
        private void wizardPage2_Commit(object sender, AeroWizard.WizardPageConfirmEventArgs e)
        {
            if (textBox1.Text == "")
            {
                e.Cancel = true;
                MessageBox.Show(Strings.PleaseselctyourInputfile);
                return;
            }
            else
            {
                warmerModel.selectedText = textBox1.Text;
            }
        }
        private void wizardPage3_Commit(object sender, AeroWizard.WizardPageConfirmEventArgs e)
        {
            if (textBox2.Text == "" || textBox3.Text == "")
            {
                e.Cancel = true;
                MessageBox.Show(Strings.PleaseenteryourDelaysettings);
                return;
            }
            else
            {
                warmerModel.delayFrom = Convert.ToInt32(textBox2.Text);
                warmerModel.delayTo = Convert.ToInt32(textBox3.Text);
                warmerModel.warmmingMethod = Convert.ToInt32(materialComboBox1.SelectedValue);
                this.Hide();
                RunWarmer form = new RunWarmer(warmerModel);
                form.ShowDialog();

            }
        }

        private void materialButton1_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void materialButton2_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt;";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string selectedFileName = openFileDialog1.FileName;
            string ss = File.ReadAllText(selectedFileName);
            textBox1.Text = ss;
        }

        private void Warmer_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Warmer_FormClosing(object sender, FormClosingEventArgs e)
        {
            waSenderForm.Show();
        }

        private void materialButton3_Click_2(object sender, EventArgs e)
        {
            MessageBox.Show(Strings.Onetoonemeans + Environment.NewLine + "* " + Strings.RecommendedIfyouhavenewwhatsappaccounts + Environment.NewLine + Environment.NewLine + Strings.OnetoManymeans + Environment.NewLine + "* " + Strings.RecommendedifyouhaveOldWhatsAppAccounts, Strings.WhatIs + " " + Strings.WarmingMethod, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
