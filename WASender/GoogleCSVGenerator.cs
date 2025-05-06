using MaterialSkin.Controls;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.Models;

namespace WASender
{
    public partial class GoogleCSVGenerator : MaterialForm
    {
        WaSenderForm waSenderForm;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;
        GeneralSettingsModel generalSettingsModel;

        public GoogleCSVGenerator(WaSenderForm _waSenderForm)
        {
            this.waSenderForm = _waSenderForm;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            generalSettingsModel = Config.GetSettings();
        }

        private void GoogleCSVGenerator_Load(object sender, EventArgs e)
        {
            InitLanguage();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                this.Invoke(new Action(() =>
                    CheckForActivation()));
            });
            
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
                if (generalSettingsModel.browserType == 1)
                {
                    WAPIHelper.CheckExecutingAssembly();
                    return true;
                }
                else
                {
                    WPPHelper.CheckExecutingAssembly();
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private void InitLanguage()
        {
            this.Text = Strings.GoogleContactsCSVGenerator;
            materialButton1.Text = Strings.UploadNumbersExcel;

            dataGridView1.Columns[0].HeaderText = Strings.Number;
            dataGridView1.Columns[1].HeaderText = Strings.Name;
            materialButton2.Text = Strings.GenrateRandomName;
            materialButton3.Text = Strings.ExportasGoogleContactCSV;

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Strings.SelectExcel;
            openFileDialog.DefaultExt = "xlsx";
            openFileDialog.Filter = "Excel Files|*.xlsx;";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog.FileName;

                FileInfo fi = new FileInfo(file);
                if (fi.Extension != ".xlsx")
                {
                    Utils.showAlert(Strings.PleaseselectExcelfilesformatonly, Alerts.Alert.enmType.Error);
                    return;
                }

                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(fi))
                {
                    try
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                        var globalCounter = dataGridView1.Rows.Count - 1;
                        for (int i = 1; i < worksheet.Dimension.Rows; i++)
                        {
                            try
                            {
                               

                                string _Number = worksheet.Cells[i + 1, 1].Value.ToString();
                                dataGridView1.Rows.Add();
                                dataGridView1.Rows[globalCounter].Cells[0].Value = _Number;

                                try
                                {
                                    string _Name= worksheet.Cells[i + 1, 2].Value.ToString();
                                    if (_Name != "")
                                    {
                                        dataGridView1.Rows[globalCounter].Cells[1].Value = _Name;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                            globalCounter++;

                        }
                    }
                    catch (Exception ex)
                    {
                        Utils.showAlert(ex.Message, Alerts.Alert.enmType.Error);
                    }
                }
            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            GenerateRandomNames form = new GenerateRandomNames(this);
            form.ShowDialog();
        }

        public void GenerateNames(string prefix, int increment, string suffix)
        {
            int globalIncrement = 1;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != "")
                {
                    if (dataGridView1.Rows[i].Cells[1].Value == null)
                    {
                        dataGridView1.Rows[i].Cells[1].Value = prefix + "" + globalIncrement.ToString() + suffix;
                        globalIncrement = globalIncrement + increment;
                    }
                }
            }
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            string MainString = "Name,Given Name,Additional Name,Family Name,Yomi Name,Given Name Yomi,Additional Name Yomi,Family Name Yomi,Name Prefix,Name Suffix,Initials,Nickname,Short Name,Maiden Name,Birthday,Gender,Location,Billing Information,Directory Server,Mileage,Occupation,Hobby,Sensitivity,Priority,Subject,Notes,Language,Photo,Group Membership,Phone 1 - Type,Phone 1 - Value" + Environment.NewLine;

            string Template = "{NAME},{NAME},,,,,,,,,,,,,,,,,,,,,,,,,,,,Mobile,{NUMBER}" + Environment.NewLine;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                try
                {
                    if (dataGridView1.Rows[i].Cells[0].Value != "")
                    {
                        string newLine = Template.Replace("{NAME}", dataGridView1.Rows[i].Cells[1].Value.ToString());
                        newLine = newLine.Replace("{NUMBER}", dataGridView1.Rows[i].Cells[0].Value.ToString());
                        MainString = MainString + newLine + Environment.NewLine;
                    }
                }
                catch (Exception)
                {

                }
            }

            string fileName = "contacts_" + Guid.NewGuid().ToString();
            string path = Config.GetTempFolderPath() + "\\" + fileName + ".csv";
            File.AppendAllLines(path, new[] { MainString });

            savesampleExceldialog.FileName = "NewContacts.csv";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(path, savesampleExceldialog.FileName.EndsWith(".csv") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".csv", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }

        private void GoogleCSVGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.waSenderForm.Show();
        }
    }
}
