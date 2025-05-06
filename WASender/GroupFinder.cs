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
using WASender.Validators;
using FluentValidation.Results;
using MaterialSkin.Controls;
using System.Threading;
using RestSharp;
using OfficeOpenXml;
using System.IO;
using System.Net;


namespace WASender
{
    public partial class GroupFinder : MaterialForm
    {
        WaSenderForm waSenderForm;
        private static int group_no = 0;
        private static int cntr = 0;
        private static bool IsRunning = false;
        private string Searchturm = "";
        private static int attempts = 0;


        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;
        GeneralSettingsModel generalSettingsModel;

        public GroupFinder(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.waSenderForm = _waSenderForm;
            group_no = 0;
            cntr = 0;
            IsRunning = false;
            Searchturm = "";
            generalSettingsModel = Config.GetSettings();

        }

        private void GroupFinder_Load(object sender, EventArgs e)
        {
            initLanguages();
            attempts = 0;
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
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


        private void initLanguages()
        {
            this.Text = Strings.GroupFinder;
            materialTextBox21.Hint = Strings.GroupSubject;
            materialTextBox21.HelperText = Strings.GroupSubject;
            materialButton1.Text = Strings.Start;
            materialButton2.Text = Strings.Stop;
            materialButton3.Text = Strings.ImportInGroupJoiner;
            dataGridView1.Columns[0].HeaderText = Strings.GroupName;
            dataGridView1.Columns[1].HeaderText = Strings.GroupLink;
            materialButton4.Text = Strings.Export;
            label2.Text = Strings.Note;
            label3.Text = Strings.Thissoftwaredoesnothaveitsowngroupdatabase;
            label4.Text = Strings.ItisrecommendedtotrygenerickeywordsinEnglishsuchas;
        }



        private void materialButton1_Click(object sender, EventArgs e)
        {
            startProgressBar();

            IsRunning = true;
            Searchturm = materialTextBox21.Text;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            try
            {
                backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helperBW = sender as BackgroundWorker;
            e.Result = BackgroundProcessLogicMethod(helperBW);
            e.Cancel = true;

        }



        private async Task<int> BackgroundProcessLogicMethod(BackgroundWorker bw)
        {
            group_no = 0;
            while (IsRunning)
            {
                try
                {
                    if (attempts == 0)
                    {
                        ServicePointManager.Expect100Continue = true;
                        Searchturm = Searchturm.Replace(" ", "-");
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        var client = new RestClient("https://groupsor.link/group/searchmore/" + Searchturm + "?group_no=" + group_no);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        request.AddHeader("Cookie", "groupda=odfdn40d8jp0vo68io532oo09iknrqo6");
                        request.AddParameter("group_no", group_no);
                        IRestResponse response = client.Execute(request);
                        string res = response.Content;
                        HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        htmlDoc.LoadHtml(res);
                        var ss = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'joinbtn')]");
                        if (ss != null)
                        {
                            foreach (var item in ss.ToList())
                            {
                                try
                                {
                                    string ttl = item.Attributes["title"].Value;
                                    ttl = ttl.Replace("Click here to join ", "").Replace(" Whatsapp group", "");
                                    var link = item.Attributes["href"].Value;
                                    var waLink = link.Split('/')[link.Split('/').Length - 1];
                                    string fullLink = "https://chat.whatsapp.com/invite/" + waLink;

                                    dataGridView1.Invoke(new Action(() =>
                                    {
                                        DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                                        row.Cells[0].Value = ttl;
                                        row.Cells[1].Value = fullLink;
                                        dataGridView1.Rows.Add(row);
                                    }));

                                    cntr = cntr + 1;
                                    label1.Invoke(new Action(() =>
                                    {
                                        label1.Text = cntr.ToString();
                                    }));
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            group_no = group_no + 1;
                        }
                        else
                        {

                            attempts = attempts + 1;
                            group_no = 1;
                        }
                    }

                    if (attempts == 1)
                    {
                        if (IsRunning)
                        {

                            ServicePointManager.Expect100Continue = true;
                            Searchturm = Searchturm.Replace(" ", "-");
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            var client = new RestClient("https://linkdegrupo.com.br/page/" + group_no + "/?s=" + Searchturm);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            request.AddHeader("Cookie", "groupda=odfdn40d8jp0vo68io532oo09iknrqo6");
                            request.AddParameter("group_no", group_no);
                            IRestResponse response = client.Execute(request);
                            string res = response.Content;
                            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                            htmlDoc.LoadHtml(res);
                            var ss = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'aneTemaQdb4dfc7_C3ae7d')]");

                            if (ss != null)
                            {
                                foreach (var item in ss.ToList())
                                {
                                    try
                                    {
                                        if (IsRunning)
                                        {
                                            string _link = item.Attributes["href"].Value;
                                            WebClient wc = new WebClient();
                                            string detailPageResult = wc.DownloadString(_link);
                                            HtmlAgilityPack.HtmlDocument htmlDocDetail = new HtmlAgilityPack.HtmlDocument();
                                            htmlDocDetail.LoadHtml(detailPageResult);
                                            var _title = htmlDocDetail.DocumentNode.SelectNodes("//h1[contains(@class, 'aneTemaQdb4dfc7_21f5bc')]");
                                            var _Grouplink = htmlDocDetail.DocumentNode.SelectNodes("//a[contains(@class, 'btn-success')]");

                                            if ((_title != null) && (_title.Count() > 0) && (_Grouplink != null) && (_Grouplink.Count() > 0))
                                            {
                                                string ttl = _title[0].InnerText;

                                                string fullLink = _Grouplink[0].Attributes["href"].Value;

                                                dataGridView1.Invoke(new Action(() =>
                                                {
                                                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                                                    row.Cells[0].Value = ttl;
                                                    row.Cells[1].Value = fullLink;
                                                    dataGridView1.Rows.Add(row);
                                                }));

                                                cntr = cntr + 1;
                                                label1.Invoke(new Action(() =>
                                                {
                                                    label1.Text = cntr.ToString();
                                                }));
                                            }
                                        }


                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                group_no = group_no + 1;
                            }
                            else
                            {
                                attempts = attempts + 1;
                                group_no = 1;
                            }
                        }


                    }

                    if (attempts == 2)
                    {
                        if (IsRunning)
                        {

                            ServicePointManager.Expect100Continue = true;
                            Searchturm = Searchturm.Replace(" ", "-");
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            var client = new RestClient("https://centraldelinkswhats.com.br/page/" + group_no + "/?s=" + Searchturm);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            request.AddHeader("Cookie", "groupda=odfdn40d8jp0vo68io532oo09iknrqo6");
                            request.AddParameter("group_no", group_no);
                            IRestResponse response = client.Execute(request);
                            string res = response.Content;
                            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                            htmlDoc.LoadHtml(res);
                            var ss = htmlDoc.DocumentNode.SelectNodes("//a[contains(@itemprop, 'URL')]");

                            if (ss != null)
                            {
                                foreach (var item in ss.ToList())
                                {
                                    try
                                    {
                                        if (IsRunning)
                                        {
                                            string _link = item.Attributes["href"].Value;
                                            WebClient wc = new WebClient();
                                            string detailPageResult = wc.DownloadString(_link);
                                            HtmlAgilityPack.HtmlDocument htmlDocDetail = new HtmlAgilityPack.HtmlDocument();
                                            htmlDocDetail.LoadHtml(detailPageResult);
                                            var _title = htmlDocDetail.DocumentNode.SelectNodes("//h1[contains(@class, 'grupoTitulo')]");
                                            var _Grouplink = htmlDocDetail.DocumentNode.SelectNodes("//a[contains(@class, 'btn-entrar')]");

                                            if ((_title != null) && (_title.Count() > 0) && (_Grouplink != null) && (_Grouplink.Count() > 0))
                                            {
                                                string ttl = _title[0].InnerText;

                                                string fullLink = _Grouplink[0].Attributes["href"].Value;

                                                dataGridView1.Invoke(new Action(() =>
                                                {
                                                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                                                    row.Cells[0].Value = ttl;
                                                    row.Cells[1].Value = fullLink;
                                                    dataGridView1.Rows.Add(row);
                                                }));

                                                cntr = cntr + 1;
                                                label1.Invoke(new Action(() =>
                                                {
                                                    label1.Text = cntr.ToString();
                                                }));
                                            }
                                        }


                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                group_no = group_no + 1;
                            }
                            else
                            {
                                attempts = attempts + 1;
                                group_no = 1;
                            }
                        }
                    }

                    if (attempts == 3)
                    {
                        if (IsRunning)
                        {

                            ServicePointManager.Expect100Continue = true;
                            Searchturm = Searchturm.Replace(" ", "-");
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            var client = new RestClient("https://linkdegrupo.com/page/" + group_no + "/?s=" + Searchturm);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            request.AddHeader("Cookie", "groupda=odfdn40d8jp0vo68io532oo09iknrqo6");
                            request.AddParameter("group_no", group_no);
                            IRestResponse response = client.Execute(request);
                            string res = response.Content;
                            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                            htmlDoc.LoadHtml(res);
                            var ss = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'aneTemaM37f5149_38d78a')]");

                            if (ss != null)
                            {
                                foreach (var item in ss.ToList())
                                {
                                    try
                                    {
                                        if (IsRunning)
                                        {
                                            string _link = item.Attributes["href"].Value;
                                            WebClient wc = new WebClient();
                                            string detailPageResult = wc.DownloadString(_link);
                                            HtmlAgilityPack.HtmlDocument htmlDocDetail = new HtmlAgilityPack.HtmlDocument();
                                            htmlDocDetail.LoadHtml(detailPageResult);
                                            var _title = htmlDocDetail.DocumentNode.SelectNodes("//h1[contains(@class, 'aneTemaM37f5149_C8f9ec')]");
                                            var _Grouplink = htmlDocDetail.DocumentNode.SelectNodes("//a[contains(@class, 'btn-success')]");

                                            if ((_title != null) && (_title.Count() > 0) && (_Grouplink != null) && (_Grouplink.Count() > 0))
                                            {
                                                string ttl = _title[0].InnerText;

                                                string fullLink = _Grouplink[0].Attributes["href"].Value;
                                                if (fullLink.StartsWith("https://chat.whatsapp.com"))
                                                {
                                                    dataGridView1.Invoke(new Action(() =>
                                                    {
                                                        DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                                                        row.Cells[0].Value = ttl;
                                                        row.Cells[1].Value = fullLink;
                                                        dataGridView1.Rows.Add(row);
                                                    }));

                                                    cntr = cntr + 1;
                                                    label1.Invoke(new Action(() =>
                                                    {
                                                        label1.Text = cntr.ToString();
                                                    }));
                                                }

                                            }
                                        }


                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                group_no = group_no + 1;
                            }
                            else
                            {
                                attempts = attempts + 1;
                                group_no = 1;
                            }
                        }
                    }
                    if (attempts == 4)
                    {
                        if (IsRunning)
                        {

                            ServicePointManager.Expect100Continue = true;
                            Searchturm = Searchturm.Replace(" ", "-");
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            var client = new RestClient("https://www.igrupos.com/buscar-ajax.php?categoria=" + Searchturm);
                            client.Timeout = -1;
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            request.AddHeader("Cookie", "groupda=odfdn40d8jp0vo68io532oo09iknrqo6");
                            request.AddParameter("group_no", group_no);
                            IRestResponse response = client.Execute(request);
                            string res = response.Content;
                            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                            htmlDoc.LoadHtml(res);
                            var ss = htmlDoc.DocumentNode.SelectNodes("//a");

                            var sss = ss.Where(x => x.Attributes["href"].Value.Contains("grupo/")).ToList();
                            if (sss != null)
                            {
                                foreach (var item in sss.ToList())
                                {
                                    var A_link = item.Attributes["href"].Value;
                                    A_link = "https://www.igrupos.com/" + A_link;
                                    try
                                    {
                                        if (IsRunning)
                                        {
                                            string _link = A_link;
                                           
                                            WebClient wc = new WebClient();
                                            string detailPageResult = wc.DownloadString(_link);
                                            HtmlAgilityPack.HtmlDocument htmlDocDetail = new HtmlAgilityPack.HtmlDocument();
                                            htmlDocDetail.LoadHtml(detailPageResult);
                                            var _title = htmlDocDetail.DocumentNode.SelectNodes("//h1");
                                            var _ParentDiv = htmlDocDetail.DocumentNode.SelectNodes("//div[contains(@class, 'apuntarse')]");

                                            string ssss = _ParentDiv.FirstOrDefault().OuterHtml;
                                            string _Grouplink = null;
                                            try
                                            {
                                                var _llink = _ParentDiv.FirstOrDefault().SelectNodes("//a");
                                                foreach (var ___Links in _llink)
                                                {
                                                    if (___Links.Attributes["onclick"] != null)
                                                    {
                                                        string onLick = ___Links.Attributes["onclick"].Value ;
                                                        _Grouplink = onLick.Replace("window.open('", "").Replace("','_blank')", "");
                                                    }
                                                    
                                                }
                                                
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            
                                        

                                            if ((_title != null) && (_title.Count() > 0) && (_Grouplink != null))
                                            {
                                                string ttl = _title[0].InnerText;

                                                string fullLink = _Grouplink;
                                                if (fullLink.StartsWith("https://chat.whatsapp.com"))
                                                {
                                                    dataGridView1.Invoke(new Action(() =>
                                                    {
                                                        DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                                                        row.Cells[0].Value = ttl;
                                                        row.Cells[1].Value = fullLink;
                                                        dataGridView1.Rows.Add(row);
                                                    }));

                                                    cntr = cntr + 1;
                                                    label1.Invoke(new Action(() =>
                                                    {
                                                        label1.Text = cntr.ToString();
                                                    }));
                                                }

                                            }
                                        }


                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                group_no = group_no + 1;
                            }
                            else
                            {
                                attempts = attempts + 1;
                                group_no = 1;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    IsRunning = false;
                    bw.CancelAsync();
                }


            }

          


            return 1;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            stopProgressbar();
            IsRunning = false;

        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            stopProgressbar();
            IsRunning = false;
        }



        private void materialButton3_Click(object sender, EventArgs e)
        {
            List<string> links = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                try
                {
                    links.Add(row.Cells[1].Value.ToString());
                }
                catch (Exception ex)
                {

                }
            }

            GroupsJoiner groupsJoiner = new GroupsJoiner(this.waSenderForm, links);
            this.Hide();
            groupsJoiner.ShowDialog();


        }

        private void GroupFinder_FormClosing(object sender, FormClosingEventArgs e)
        {
            waSenderForm.formReturn(true);
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.Cells[1, 1].Value = Strings.GroupName;
            workSheet.Cells[1, 2].Value = Strings.Link;
            int recordIndex = 2;


            List<NVModel> links = new List<NVModel>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                try
                {
                    links.Add(new NVModel
                    {
                        Name = row.Cells[0].Value.ToString(),
                        Value = row.Cells[1].Value.ToString()
                    });
                }
                catch (Exception ex)
                {

                }
            }
            foreach (var item in links)
            {
                workSheet.Cells[recordIndex, 1].Value = item.Name;
                workSheet.Cells[recordIndex, 2].Value = item.Value;
                recordIndex++;
            }
            workSheet.Column(1).AutoFit();
            workSheet.Column(2).AutoFit();

            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, materialTextBox21.Text + "_Group_Links" + Guid.NewGuid().ToString() + ".xlsx");
            string NewFileName = file.ToString();

            FileStream objFileStrm = File.Create(NewFileName);
            objFileStrm.Close();

            File.WriteAllBytes(NewFileName, excel.GetAsByteArray());
            excel.Dispose();
            savesampleExceldialog.FileName = materialTextBox21.Text + "_Group_Links" + "" + ".xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }

        }
        private void startProgressBar()
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
        }
        private void stopProgressbar()
        {
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.MarqueeAnimationSpeed = 0;
        }



    }

    class NVModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
