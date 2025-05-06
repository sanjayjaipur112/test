using HtmlAgilityPack;
using MaterialSkin.Controls;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.Model;
using WASender.Models;

namespace WASender
{
    public partial class WebScrapper : MaterialForm
    {
        BackgroundWorker backgroundWorker;
        public static List<ResultHolder> resultHolderList { get; set; }
        List<string> importLists;
        bool isRunning = false;
        

        public static bool checSublinks = true;
        public static int depth = 0;
        WaSenderForm waSenderForm;

        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;
        GeneralSettingsModel generalSettingsModel;
        public WebScrapper(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.waSenderForm = _waSenderForm;
            generalSettingsModel = Config.GetSettings();
        }

        public WebScrapper(WaSenderForm _waSenderForm, List<string> _importLists)
        {
            InitializeComponent();
            this.importLists = _importLists;
            this.waSenderForm = _waSenderForm;
            if (this.importLists.Count() > 1)
            {
                foreach (var item in this.importLists)
                {
                    this.textBox1.Text = this.textBox1.Text + item + Environment.NewLine;
                }
            }
            generalSettingsModel = Config.GetSettings();
        }

        private void WebScrapper_Load(object sender, EventArgs e)
        {
            initLanguages();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                this.Invoke(new Action(() =>
                    CheckForActivation()));
            });
            
            //backgroundWorker = new BackgroundWorker();

            //backgroundWorker.DoWork += BackgroundWorkerDoWork;
            //backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
            //backgroundWorker.WorkerSupportsCancellation = true;
            //backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
        }

        private void CheckForActivation()
        {
            pgbar = new Progressbar();
            //pgbar.Show();
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


        void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            ScrapData();
        }

        void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "Completed";
        }

        void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }


        private void ScrapData()
        {
            int mainLinkCount = 0;
            foreach (var item in resultHolderList)
            {
                mainLinkCount++;
                if (isRunning == true)
                {
                    if (!(item.MainLink.StartsWith("https://") || item.MainLink.StartsWith("http://")))
                    {
                        return;
                    }
                    try
                    {
                        ServicePointManager.Expect100Continue = true;
                        if (item.MainLink.Contains("?"))
                        {
                            var splitter = item.MainLink.Split('?');
                            item.MainLink = splitter[0];
                        }
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        HtmlWeb hw = new HtmlWeb();
                        HtmlAgilityPack.HtmlDocument doc = hw.Load(item.MainLink);

                        item.subLinks.Add(item.MainLink);
                        var AllNodes = doc.DocumentNode.SelectNodes("//a[@href]");

                        if (checSublinks == true && isRunning == true)
                        {
                            if (AllNodes != null)
                            {
                                
                                foreach (HtmlNode link in AllNodes)
                                {
                                    HtmlAttribute att = link.Attributes["href"];
                                    string v = att.Value;
                                    if (att.Value.Contains(item.MainLink.Replace("https://", "").Replace("http://", "")))
                                    {
                                        item.subLinks.Add(v);
                                    }
                                    if (!att.Value.Contains("https://") && !att.Value.Contains("http://"))
                                    {
                                        string ssfff = "";
                                        if (att.Value.StartsWith("./"))
                                        {
                                            string _subLink = att.Value.Replace("./", "");
                                            item.subLinks.Add(item.MainLink + _subLink);
                                        }
                                        else
                                        {
                                            item.subLinks.Add(item.MainLink + att.Value);
                                        }
                                    }
                                }
                            }

                        }

                        int sublinkCount = 1;
                        if (item.subLinks != null)
                        {
                            List<string> filteredLinks = item.subLinks;
                            if (checSublinks == true)
                            {
                                if (depth != 0)
                                {
                                    if (item.subLinks.Count() > depth)
                                    {
                                        filteredLinks = item.subLinks.Take(depth).ToList();
                                    }
                                }
                            }
                            else
                            {
                                // filteredLinks.Add(filteredLinks.FirstOrDefault());
                            }

                            foreach (string subLink in filteredLinks)
                            {
                                if (isRunning == true)
                                {
                                    try
                                    {
                                        HtmlWeb detailPage = new HtmlWeb();
                                        HtmlAgilityPack.HtmlDocument DetailDoc = detailPage.Load(subLink);

                                        label1.Invoke((MethodInvoker)delegate
                                        {
                                            label1.Text = "Running... Checking in " + item.MainLink + "(" + mainLinkCount + ") out of (" + resultHolderList.Count() + ") - Sub Page (" + sublinkCount + ") Out of (" + filteredLinks.Count().ToString() + ")";
                                        });

                                        List<HtmlNode> allnodes = DetailDoc.DocumentNode.Descendants().ToList();
                                        foreach (HtmlNode node in allnodes)
                                        {

                                            string ss = node.OuterHtml;
                                            ss = ss.Replace("(", "")
                                                                .Replace(")", "")
                                                                .Replace(" ...", ".")
                                                                .Replace("Tel: ", "Tel: +")
                                                                .Replace("Tel.: ", "Tel.: +")
                                                                .Replace("call ", "call +")
                                                                .Replace("Phone :", "")
                                                                .Replace("++", "+");

                                            Match match = new Regex("(?:\\+)[0-9\\s.\\/-]{10,17}").Match(ss);
                                            if (match.Success && match.Value.Count(char.IsDigit) > 7)
                                            {
                                                string __match = match.Value.Replace("/", "").Replace(" ", "").Replace(".", "")
                                                                .Replace("-", "");

                                                WebItemModel webItemModel = new WebItemModel();
                                                webItemModel.Value = match.Value;
                                                webItemModel.SubLink = subLink;
                                                webItemModel.type = typeEnum.mobilenumber;
                                                if (item.mobiles.Where(x => x.Value == match.Value).Count() == 0)
                                                {
                                                    item.mobiles.Add(webItemModel);
                                                    dataGridView1.Invoke((MethodInvoker)delegate
                                                    {
                                                        dataGridView1.Rows.Add(new object[]{
                                                            subLink,
                                                            match.Value
                                                     });

                                                    });
                                                }

                                            }

                                            match = new Regex("(?:[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[A-Za-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])").Match(ss);
                                            if (match.Success)
                                            {
                                                WebItemModel webItemModel = new WebItemModel();
                                                webItemModel.Value = match.Value;
                                                webItemModel.SubLink = subLink;
                                                webItemModel.type = typeEnum.emailId;
                                                if (item.emailIds.Where(x => x.Value == match.Value).Count() == 0)
                                                {
                                                    item.emailIds.Add(webItemModel);
                                                    dataGridView2.Invoke((MethodInvoker)delegate
                                                    {
                                                        dataGridView2.Rows.Add(new object[]{
                                                            subLink,
                                                            match.Value
                                                     });
                                                    });
                                                }

                                            }
                                        }
                                        sublinkCount++;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }


                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
        }


        private void initLanguages()
        {
            this.Text = Strings.WebsiteEMailMobileExtractor;
            materialLabel5.Text = Strings.WebSiteUrls;
            materialButton1.Text = Strings.Start;
            materialLabel1.Text = Strings.MobileNumber;
            materialLabel2.Text = Strings.EmailId;
            materialLabel4.Text = Strings.Status;
            label1.Text = Strings.NotStarted;
            dataGridView1.Columns[0].HeaderText = Strings.Link;
            dataGridView1.Columns[1].HeaderText = Strings.Number;

            dataGridView2.Columns[0].HeaderText = Strings.Link;
            dataGridView2.Columns[1].HeaderText = Strings.EmailId;
            materialCheckbox1.Text = Strings.DeepCheck;
            materialCheckbox1.Checked = true;
            materialButton4.Text = Strings.Stop;
            materialButton2.Text = Strings.Export;
            materialButton3.Text = Strings.ImportNumbers;
            materialButton5.Text = Strings.Export;
            materialTextBox21.Hint = Strings.depth;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                var splitter = textBox1.Text.Split('\n');
                resultHolderList = new List<ResultHolder>();
                ResultHolder resultHolder;
                foreach (var item in splitter)
                {
                    string newitem = item.Replace("\r", "");
                    if (!newitem.EndsWith("/"))
                    {
                        newitem = newitem + "/";
                    }
                    resultHolder = new ResultHolder();
                    resultHolder.MainLink = newitem;
                    resultHolder.emailIds = new List<WebItemModel>();
                    resultHolder.mobiles = new List<WebItemModel>();
                    resultHolder.subLinks = new List<string>();
                    resultHolderList.Add(resultHolder);
                }
                checSublinks = materialCheckbox1.Checked;
                try
                {
                    depth = Convert.ToInt32(materialTextBox21.Text);
                }
                catch (Exception ex)
                {
                    depth = 30;
                }
                backgroundWorker = new BackgroundWorker();

                backgroundWorker.DoWork += BackgroundWorkerDoWork;
                backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
                isRunning = true;
                backgroundWorker.RunWorkerAsync();
                label1.Text = Strings.Running;
            }
        }

        private void WebScrapper_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.waSenderForm.Show();
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
            isRunning = false;
            label1.Text = Strings.Stopped;

        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            List<string> allList = new List<string>();

            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                try
                {
                    string number = item.Cells[1].Value.ToString();
                    number = ProjectCommon.sanitiseNumber(number);
                    if (allList.Where(x => x == number).Count() == 0)
                    {
                        allList.Add(number);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            this.waSenderForm.ReturnPasteNumber(allList);
            this.Close();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "GMapData" + Guid.NewGuid().ToString() + ".xlsx");
            string NewFileName = file.ToString();
            File.Copy("ChatListTemplate.xlsx", NewFileName, true);
            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];
                ws.Cells[1, 1].Value = Strings.Link;
                ws.Cells[1, 2].Value = Strings.Number;

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if ((dataGridView1.Rows[i].Cells[0].Value != null) && (dataGridView1.Rows[i].Cells[1].Value != null))
                    {
                        string number=dataGridView1.Rows[i].Cells[0].Value.ToString();
                        number=ProjectCommon.sanitiseNumber(number);
                        ws.Cells[i + 2, 1].Value = number;
                        ws.Cells[i + 2, 2].Value = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    }
                }
                xlPackage.Save();
            }
            

            savesampleExceldialog.FileName = "WebExtract_Numbers.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }

        private void materialButton5_Click(object sender, EventArgs e)
        {
            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "GMapData" + Guid.NewGuid().ToString() + ".xlsx");
            string NewFileName = file.ToString();
            File.Copy("ChatListTemplate.xlsx", NewFileName, true);
            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];
                ws.Cells[1, 1].Value = Strings.Link;
                ws.Cells[1, 2].Value = Strings.EmailId;

                for (int i = 0; i < dataGridView2.Rows.Count; i++)
                {
                    if ((dataGridView2.Rows[i].Cells[0].Value != null) && (dataGridView2.Rows[i].Cells[1].Value != null))
                    {
                        ws.Cells[i + 2, 1].Value = dataGridView2.Rows[i].Cells[0].Value.ToString();
                        ws.Cells[i + 2, 2].Value = dataGridView2.Rows[i].Cells[1].Value.ToString();
                    }
                }
                xlPackage.Save();
            }


            savesampleExceldialog.FileName = "WebExtract_Emails.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }

        private void materialButton6_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Strings.CheckSubLinks, Strings.DeepCheck, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialCheckbox1_CheckedChanged(object sender, EventArgs e)
        {
            if (materialCheckbox1.Checked)
            {
                materialTextBox21.Enabled = true;
            }
            else {
                materialTextBox21.Enabled = false;
            }
        }

        private void materialButton7_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Strings.LimitofSublinksunderanywebsite + ", \n" + Strings.Put0asunlimited, Strings.depth, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialButton8_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void materialButton9_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
        }
    }
}
