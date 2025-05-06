
using FluentValidation.Results;
using MaterialSkin;
using MaterialSkin.Controls;
using Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.Model;
using WASender.Models;
using WASender.Validators;


namespace WASender
{
    public partial class WaSenderForm : MaterialForm
    {
        MaterialSkin.MaterialSkinManager materialSkinManager;
        public WASenderSingleTransModel wASenderSingleTransModel;
        public WASenderGroupTransModel wASenderGroupTransModel;
        GeneralSettingsModel generalSettingsModel;
        //List<ButtonsModel> buttonsModelList1, buttonsModelList2, buttonsModelList3, buttonsModelList4, buttonsModelList5, buttonsModelList6, buttonsModelList7, buttonsModelList8, buttonsModelList9, buttonsModelList10;

        List<PollModel> pollModelList1, pollModelList2, pollModelList3, pollModelList4, pollModelList5, pollModelList6, pollModelList7, pollModelList8, pollModelList9, pollModelList10;
        WebBrowser _browser1, _browser2, _browser3, _browser4, _browser5, _browser6, _browser7, _browser8, _browser9, _browser10;
        string ScheduleId = "";
        public SchedulesModel schedulesModel;
        Progressbar pgbar;
        public List<string> tmpfriendlyNumbers { get; set; }
        public int tmpsendTofriendlyNumbersAfterMessages { get; set; }

        List<ButtonHolderModel> buttonsModelList1;
        List<ButtonHolderModel> buttonsModelList2;
        List<ButtonHolderModel> buttonsModelList3;
        List<ButtonHolderModel> buttonsModelList4;
        List<ButtonHolderModel> buttonsModelList5;

        List<ButtonHolderModel> buttonsModelList6;
        List<ButtonHolderModel> buttonsModelList7;
        List<ButtonHolderModel> buttonsModelList8;
        List<ButtonHolderModel> buttonsModelList9;
        List<ButtonHolderModel> buttonsModelList10;

        WebBrowser _btnBrowser1, _btnBrowser2, _btnBrowser3, _btnBrowser4, _btnBrowser5;

        public WaSenderForm(string[] args)
        {

            InitializeComponent();
            this.Icon = Strings.AppIcon;
            if (Strings.Allow_Users_to_Change_Language == false)
            {
                comboBox1.Visible = false;
            }
            checkBrowserType();

            dataGridView1.Columns[2].Visible = false;
            dataGridView2.Columns[2].Visible = false;
            dataGridView3.Columns[2].Visible = false;
            dataGridView4.Columns[2].Visible = false;
            dataGridView5.Columns[2].Visible = false;
            dataGridView6.Columns[2].Visible = false;
            dataGridView7.Columns[2].Visible = false;
            dataGridView8.Columns[2].Visible = false;
            dataGridView9.Columns[2].Visible = false;
            dataGridView10.Columns[2].Visible = false;

            materialSkinManager = Utils.SetColorScheme(materialSkinManager, this, Model.ColorSchemeenum.Green);

            buttonsModelList1 = new List<ButtonHolderModel>();
            buttonsModelList2 = new List<ButtonHolderModel>();
            buttonsModelList3 = new List<ButtonHolderModel>();
            buttonsModelList4 = new List<ButtonHolderModel>();
            buttonsModelList5 = new List<ButtonHolderModel>();

            buttonsModelList6 = new List<ButtonHolderModel>();
            buttonsModelList7 = new List<ButtonHolderModel>();
            buttonsModelList8 = new List<ButtonHolderModel>();
            buttonsModelList9 = new List<ButtonHolderModel>();
            buttonsModelList10 = new List<ButtonHolderModel>();

            _btnBrowser1 = webBrowser12;
            _btnBrowser2 = webBrowser13;
            _btnBrowser3 = webBrowser14;
            _btnBrowser4 = webBrowser15;
            _btnBrowser5 = webBrowser16;

            pollModelList1 = new List<PollModel>();
            pollModelList2 = new List<PollModel>();
            pollModelList3 = new List<PollModel>();
            pollModelList4 = new List<PollModel>();
            pollModelList5 = new List<PollModel>();
            pollModelList6 = new List<PollModel>();
            pollModelList7 = new List<PollModel>();
            pollModelList8 = new List<PollModel>();
            pollModelList9 = new List<PollModel>();
            pollModelList10 = new List<PollModel>();

            _browser1 = webBrowser1;
            _browser2 = webBrowser2;
            _browser3 = webBrowser3;
            _browser4 = webBrowser4;
            _browser5 = webBrowser5;

            _browser6 = webBrowser6;
            _browser7 = webBrowser7;
            _browser8 = webBrowser8;
            _browser9 = webBrowser9;
            _browser10 = webBrowser10;

            setBrowserDefaultHtml(webBrowser1);
            setBrowserDefaultHtml(webBrowser2);
            setBrowserDefaultHtml(webBrowser3);
            setBrowserDefaultHtml(webBrowser4);
            setBrowserDefaultHtml(webBrowser5);
            setBrowserDefaultHtml(webBrowser6);
            setBrowserDefaultHtml(webBrowser7);
            setBrowserDefaultHtml(webBrowser8);
            setBrowserDefaultHtml(webBrowser9);
            setBrowserDefaultHtml(webBrowser10);

            setBrowserDefaultHtml(webBrowser12);
            setBrowserDefaultHtml(webBrowser13);
            setBrowserDefaultHtml(webBrowser14);
            setBrowserDefaultHtml(webBrowser15);
            setBrowserDefaultHtml(webBrowser16);
            


            this._browser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser1_DocumentCompleted);
            this._browser2.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser2_DocumentCompleted);
            this._browser3.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser3_DocumentCompleted);
            this._browser4.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser4_DocumentCompleted);
            this._browser5.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser5_DocumentCompleted);

            this._browser6.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser6_DocumentCompleted);
            this._browser7.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser7_DocumentCompleted);
            this._browser8.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser8_DocumentCompleted);
            this._browser9.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser9_DocumentCompleted);
            this._browser10.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(browser10_DocumentCompleted);

            try
            {
                if (args != null && args.Count() > 0)
                {
                    ScheduleId = args[0].ToString();
                }

            }
            catch (Exception ex)
            {

            }


            this._btnBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_btnBrowser1_DocumentCompleted);
            this._btnBrowser2.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_btnBrowser2_DocumentCompleted);
            this._btnBrowser3.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_btnBrowser3_DocumentCompleted);
            this._btnBrowser4.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_btnBrowser4_DocumentCompleted);
            this._btnBrowser5.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_btnBrowser5_DocumentCompleted);
            
            

        }

        private void _btnBrowser5_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._btnBrowser5.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown16);
        }

        private void Body_MouseDown16(object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._btnBrowser5.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            ButtonHolderModel b = buttonsModelList5.Where(x => x.Id == btnId).FirstOrDefault();
                            b.editMode = true;
                            ButtonHolder form = new ButtonHolder(this, b);
                            form.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
            }
        }

        private void _btnBrowser4_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._btnBrowser4.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown15);
        }

        private void Body_MouseDown15(object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._btnBrowser4.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            ButtonHolderModel b = buttonsModelList4.Where(x => x.Id == btnId).FirstOrDefault();
                            b.editMode = true;
                            ButtonHolder form = new ButtonHolder(this, b);
                            form.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
            }
        }

        private void _btnBrowser3_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._btnBrowser3.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown14);
        }

        private void Body_MouseDown14(object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._btnBrowser3.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            ButtonHolderModel b = buttonsModelList3.Where(x => x.Id == btnId).FirstOrDefault();
                            b.editMode = true;
                            ButtonHolder form = new ButtonHolder(this, b);
                            form.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
            }
        }

        private void _btnBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._btnBrowser2.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown13);
        }

        private void Body_MouseDown13(object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._btnBrowser2.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            ButtonHolderModel b = buttonsModelList2.Where(x => x.Id == btnId).FirstOrDefault();
                            b.editMode = true;
                            ButtonHolder form = new ButtonHolder(this, b);
                            form.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
            }
        }

        private void _btnBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._btnBrowser1.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown12);
        }

        private void Body_MouseDown12(object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._btnBrowser1.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            ButtonHolderModel b = buttonsModelList1.Where(x => x.Id == btnId).FirstOrDefault();
                            b.editMode = true;
                            ButtonHolder form = new ButtonHolder(this, b);
                            form.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    break;
            }
        }

        private void setBrowserDefaultHtml(WebBrowser _WebBrowser)
        {
            _WebBrowser.DocumentText = Storage.DocumentHtmlString;
        }



        private void browser1_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser1.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown1);
        }

        private void browser2_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser2.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown2);
        }
        private void browser3_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser3.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown3);
        }
        private void browser4_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser4.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown4);
        }
        private void browser5_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser5.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown5);
        }


        private void browser6_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser6.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown6);
        }
        private void browser7_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser7.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown7);
        }
        private void browser8_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser8.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown8);
        }
        private void browser9_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser9.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown9);
        }
        private void browser10_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this._browser10.Document.Body.MouseDown += new HtmlElementEventHandler(Body_MouseDown10);
        }

        private void Body_MouseDown1(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser1.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList1.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }

        private void Body_MouseDown2(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser2.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList2.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                    break;
            }
        }

        private void Body_MouseDown3(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser3.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList3.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }
        private void Body_MouseDown4(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser4.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList4.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }

        private void Body_MouseDown5(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser5.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList5.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }


        private void Body_MouseDown6(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser6.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList6.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                    break;
            }
        }

        private void Body_MouseDown7(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser7.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList7.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }

        private void Body_MouseDown8(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser8.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList8.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }

        private void Body_MouseDown9(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser9.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList9.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }

        private void Body_MouseDown10(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this._browser10.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("id");
                    if (btnId != "")
                    {
                        try
                        {
                            PollModel b = pollModelList10.Where(x => x.id == btnId).FirstOrDefault();
                            b.editMode = true;
                            AddPoll addButton = new AddPoll(b, this);
                            addButton.ShowDialog();
                        }
                        catch (Exception ex)
                        {

                        }

                    }


                    break;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            _Config.KillChromeDriverProcess();
            init();
            setTooltips();
            CHeckForActivation();
            lblSection.Hide();
            webBrowser11.ScriptErrorsSuppressed = true;
            LoadDocument();
            if (ScheduleId == "")
            {
                checkForPendingSchedules(false);
            }
            else
            {
                runSchedule();
            }

            checkForInternalUpdate();


        }

        private void ShowPGBar(string msg = "")
        {
            pgbar = new Progressbar();
            pgbar.Show();
            if (msg != "" && msg != null)
            {
                pgbar.materialLabel1.Text = msg;
            }
        }

        private void hidePGBar()
        {
            pgbar.Close();
        }

        private async void checkForInternalUpdate()
        {

            try
            {
                await Task.Delay(1000);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Uri u = new System.Uri("https://github.com/bracketsApps007/NewRepo/releases/download/1.0.0/internalupdateversion.txt");

                WebClient client = new WebClient();
                string stream = await client.DownloadStringTaskAsync(u);

                if (generalSettingsModel.internalUpdateVersion == 0)
                {
                    generalSettingsModel.internalUpdateVersion = Strings.CurrentInternalUpdateVersion;
                }

                if (generalSettingsModel.internalUpdateVersion < Convert.ToInt32(stream))
                {
                    try
                    {
                        ShowPGBar(Strings.Takinganinternalupdates + ", " + Strings.PleaseWait);
                        string UpdateFIle = Config.GetTempFolderPath() + "\\" + Guid.NewGuid().ToString() + ".zip";
                        string extractPath = Config.WAPIFolderFolder();

                        WebClient webClient = new WebClient();
                        Uri uas = new System.Uri("http://shivjagar.in/update.zip");
                        await webClient.DownloadFileTaskAsync(uas, UpdateFIle);

                        var archive = ZipFile.Open(UpdateFIle, ZipArchiveMode.Read);

                        DirectoryInfo di = Directory.CreateDirectory(extractPath);
                        string destinationDirectoryFullPath = di.FullName;

                        foreach (ZipArchiveEntry file in archive.Entries)
                        {
                            string completeFileName = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, file.FullName));

                            if (!completeFileName.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new IOException("Trying to extract file outside of destination directory. See this link for more info: https://snyk.io/research/zip-slip-vulnerability");
                            }

                            if (file.Name == "")
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                                continue;
                            }
                            file.ExtractToFile(completeFileName, true);
                        }

                        archive.Dispose();
                        File.Delete(UpdateFIle);
                    }
                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        generalSettingsModel.internalUpdateVersion = Convert.ToInt32(stream);
                        string Json = JsonConvert.SerializeObject(generalSettingsModel, Formatting.Indented);

                        String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();

                        if (!File.Exists(GetGeneralSettingsFilePath))
                        {
                            File.Create(GetGeneralSettingsFilePath).Close();
                        }

                        File.WriteAllText(GetGeneralSettingsFilePath, Json);

                        MaterialSnackBar SnackBarMessage1 = new MaterialSnackBar("Done 👍👍👍👍", Strings.OK, true);
                        SnackBarMessage1.Show(this);
                    }
                    catch (Exception exx)
                    {

                    }

                    hidePGBar();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }


        }

        public void runSceduleById(string _ScheduleId)
        {
            this.ScheduleId = _ScheduleId;
            runSchedule(true);
        }

        private void runSchedule(bool forceScheduleRun = false)
        {
            schedulesModel = PCUtils.getScheduleById(ScheduleId);
            if (schedulesModel.Type == "SINGLE")
            {
                wASenderSingleTransModel = JsonConvert.DeserializeObject<WASenderSingleTransModel>(schedulesModel.JsonString);

                Task.Delay(1000).ContinueWith(t => bar());
                RunSingle run = new RunSingle(wASenderSingleTransModel, this, schedulesModel, forceScheduleRun);
                run.Show();

            }
            if (schedulesModel.Type == "GROUP")
            {
                wASenderGroupTransModel = JsonConvert.DeserializeObject<WASenderGroupTransModel>(schedulesModel.JsonString);
                Task.Delay(1000).ContinueWith(t => bar());
                RunGroup run = new RunGroup(wASenderGroupTransModel, this, schedulesModel, forceScheduleRun);
                run.Show();
            }
        }


        private void bar()
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
            }));
        }

        private void LoadDocument()
        {
            webBrowser11.Visible = false;
            pictureBox1.Visible = true;
            string curDir = Directory.GetCurrentDirectory();
            var uri = new Uri(String.Format("file:///{0}/HelpDocs/Dashboard.html", curDir));
            webBrowser11.DocumentCompleted += browser11_DocumentCompleted;
            webBrowser11.Url = uri;

        }

        private async void browser11_DocumentCompleted(Object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            setPrimaryColor(materialSkinManager.ColorScheme.PrimaryColor.R, materialSkinManager.ColorScheme.PrimaryColor.G, materialSkinManager.ColorScheme.PrimaryColor.B);
            await Task.Delay(1000);
            setLanguages();
            setEnableDesableTools();
            webBrowser11.Visible = true;
            pictureBox1.Visible = false;
            this.webBrowser11.Document.Body.MouseUp += new HtmlElementEventHandler(Body_MouseDown11);
        }

        private void setEnableDesableTools()
        {
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_GetGroupMembers", Strings.Get_Group_Member.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_GetWhatsAppGroupLinksfromweb", Strings.Grab_WhatsApp_Group_Links_from_web.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_WhatsAppAutoreplyBot", Strings.WhatsApp_Auto_Responder_Bot.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_ContactListGrabber", Strings.Contact_List_Grabber.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_GoogleMagDataExtractor", Strings.Google_Map_Data_Extractor.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_AutoGroupJoiner", Strings.Auto_Group_Joiner.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_WhatsAppNumberFilter", Strings.WhatsApp_Number_Filter.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_GrabActiveGroupMembers", Strings.Grab_Active_Group_Members.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_GrabChatList", Strings.Grab_Chat_List.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_BulkAddGroupMembers", Strings.Bulk_Add_Group_Members.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_GroupFinder", Strings.Group_Finder.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_BulkGroupGenerator", Strings.Bulk_Group_Generator.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_GoogleContactCSVGenerator", Strings.Google_Contact_CSV_Generator.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_WebsiteEmailMobileExtractor", Strings.Website_Email_Mobile_Extractor.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_WhatsAppWarmer", Strings.WhatsApp_Warmer.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_PollReport", Strings.Poll_Report.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_SocialMediaExtractor", Strings.Social_Media_Data_Extractor.ToString() });
            webBrowser11.Document.InvokeScript("setEnableDesable", new string[] { "Box_Scheduler", Strings.All_Schedules.ToString() });
        }

        private void Body_MouseDown11(Object sender, HtmlElementEventArgs e)
        {
            switch (e.MouseButtonsPressed)
            {
                case MouseButtons.Left:
                    HtmlElement element = this.webBrowser11.Document.GetElementFromPoint(e.ClientMousePosition);
                    var btnId = element.GetAttribute("data-id");
                    if (btnId != "")
                    {
                        if (btnId == "GrabGroupMembers")
                        {
                            GetGroupMember getGroupMember = new GetGroupMember(this);
                            this.Hide();
                            getGroupMember.Show();
                        }
                        else if (btnId == "GetWhatsAppGroupLinksfromweb")
                        {
                            GrabGroupLinks form = new GrabGroupLinks(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "WhatsAppAutoreplyBot")
                        {
                            WaAutoReplyBot.WaAutoReplyForm form = new WaAutoReplyBot.WaAutoReplyForm(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "ContactListGrabber")
                        {
                            ContactGrabber form = new ContactGrabber(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "GoogleMagDataExtractor")
                        {
                            GMapExtractor form = new GMapExtractor(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "AutoGroupJoiner")
                        {
                            GroupsJoiner form = new GroupsJoiner(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "WhatsAppNumberFilter")
                        {
                            NumberFilter form = new NumberFilter(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "GrabActiveGroupMembers")
                        {
                            GrabGroupActiveMembers form = new GrabGroupActiveMembers(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "GrabChatList")
                        {
                            GrabChatList form = new GrabChatList(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "BulkAddGroupMembers")
                        {
                            GroupMemberAdder form = new GroupMemberAdder(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "GroupFinder")
                        {
                            GroupFinder form = new GroupFinder(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "BulkGroupGenerator")
                        {
                            GroupGenerator form = new GroupGenerator(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "GoogleContactCSVGenerator")
                        {
                            GoogleCSVGenerator form = new GoogleCSVGenerator(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "WebsiteEmailMobileExtractor")
                        {
                            WebScrapper form = new WebScrapper(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "WhatsAppWarmer")
                        {
                            Warmer form = new Warmer(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "PollReport")
                        {
                            GetPollResults form = new GetPollResults(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "SocialMediaExtractor")
                        {
                            SocialMediaExtractor form = new SocialMediaExtractor(this);
                            this.Hide();
                            form.Show();
                        }
                        else if (btnId == "Scheduler")
                        {
                            AllSchedules form = new AllSchedules(this);
                            this.Hide();
                            form.Show();
                        }
                    }
                    break;
            }
        }

        private void setLanguages()
        {
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtGetGroupMembers", Strings.GetGroupMember });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtGetWhatsAppGroupLinksfromweb", Strings.GrabWhatsAppGroupLinksfromwebpage });

            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtWhatsAppAutoreplyBot", Strings.WhatsAppAutoResponderBot });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtGoogleMagDataExtractor", Strings.GoogleMapDataEExtractor });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtAutoGroupJoiner", Strings.AutoGroupJoiner });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtWhatsAppNumberFilter", Strings.WhatsAppNumberFilter });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtGrabActiveGroupMembers", Strings.GrabActiveGroupMembers });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtContactListGrabber", Strings.ContactListGrabber });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtGrabChatList", Strings.GrabChatList });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtBulkAddGroupMembers", Strings.BulkAddGroupMembers });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtGroupFinder", Strings.GroupFinder });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtBulkGroupGenerator", Strings.BulkGroupGenerator });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtGoogleContactCSVGenerator", Strings.GoogleContactsCSVGenerator });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtWebsiteEmailMobileExtractor", Strings.WebsiteEMailMobileExtractor });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtWhatsAppWarmer", Strings.WhatsAppWarmer });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtPollReport", Strings.GetPollResults });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtSocialMediaExtractor", Strings.SocialMediaDataExtractor });
            webBrowser11.Document.InvokeScript("changeLabel", new string[] { "txtScheduler", Strings.AllSchedules });

            webBrowser11.Document.InvokeScript("changeLabelByClass", new string[] { "grabnow", Strings.GrabNow });
            webBrowser11.Document.InvokeScript("changeLabelByClass", new string[] { "startnow", Strings.StartNow });
        }

        private void setPrimaryColor(int R, int G, int B)
        {
            addNewCss(@" .btn-primary {background-color:rgb(" + R + ", " + G + ", " + B + "); };");
            addNewCss(@" .btn-primary.active, .btn-primary:active, .btn-primary:focus, .btn-primary:hover{background-color:rgb(" + R + ", " + G + ", " + B + ")!important}; ");
            addNewCss(@" .btn-primary.disabled, .btn-primary:disabled {background-color:rgb(" + R + ", " + G + ", " + B + "); };");
            addNewCss(@" .badge-primary {background-color:rgb(" + R + ", " + G + ", " + B + "); };");
        }

        private void addNewCss(string CSS)
        {
            System.Windows.Forms.HtmlElement head = webBrowser11.Document.GetElementsByTagName("head")[0];
            System.Windows.Forms.HtmlElement styleEl = webBrowser11.Document.CreateElement("style");
            styleEl.InnerHtml = CSS;
            head.AppendChild(styleEl);
        }
        public void CHeckForActivation()
        {
            if (!Config.IsProductActive())
            {
                Activate activate = new Activate(this);
                this.Hide();
                activate.ShowDialog();
            }
        }

        private void setTooltips()
        {
            Utils.setTooltiop(btnDownloadSample, Strings.DownloadSample);
            Utils.setTooltiop(btnUploadExcel, Strings.UploadSampleExcel);

            Utils.setTooltiop(btnDownloadSampleGroup, Strings.DownloadSample);
            Utils.setTooltiop(btnUploadExcelGroup, Strings.UploadSampleExcel);
        }

        private void init()
        {
            materialCheckbox1.Checked = true;
            materialCheckbox2.Checked = true;
            materialCheckbox4.Checked = true;
            materialCheckbox3.Checked = true;
            defaultColorSchime();
            getSelectedLanguage();
            lblSection.Text = Strings.ContectSender;
            chkDarkMode.Visible = false;
            SetLanguagesDropdown();
            comboBox1.SelectedText = generalSettingsModel.selectedLanguage;
            initLanguage();
        }

        public void CountryCOdeAdded(string code)
        {
            foreach (DataGridViewRow item in gridTargets.Rows)
            {
                if (item.Cells[0].Value != "" && item.Cells[0].Value != null)
                {
                    item.Cells[0].Value = code + item.Cells[0].Value;
                }
            }
        }
        private void SetLanguagesDropdown()
        {
            DirectoryInfo d = new DirectoryInfo("languages");
            FileInfo[] Files = d.GetFiles("*.json");
            string str = "";

            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            foreach (FileInfo file in Files)
            {

                str = str + ", " + file.Name;

                try
                {
                    comboBox1.Items.Add(new { Text = file.Name.Split('.')[0], Value = file.Name });
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void getSelectedLanguage()
        {
            string settingPath = Config.GetGeneralSettingsFilePath();

            if (!File.Exists(settingPath))
            {
                File.Create(settingPath).Close();
            }
            generalSettingsModel = new GeneralSettingsModel();
            generalSettingsModel.selectedLanguage = Strings.selectedLanguage;
            try
            {
                string GeneralSettingJson = "";
                using (StreamReader r = new StreamReader(settingPath))
                {
                    GeneralSettingJson = r.ReadToEnd();
                }
                var dict = JsonConvert.DeserializeObject<GeneralSettingsModel>(GeneralSettingJson);
                if (dict != null)
                {
                    generalSettingsModel = dict;
                }
                if (generalSettingsModel.selectedLanguage == null || generalSettingsModel.selectedLanguage == "")
                {
                    generalSettingsModel.selectedLanguage = Strings.selectedLanguage;
                }
                Strings.selectedLanguage = generalSettingsModel.selectedLanguage;
            }
            catch (Exception ex)
            {

            }

        }

        private void initLanguage()
        {
            this.Text = Strings.AppName;
            label1.Text = Strings.SoftwareVersion;
            tabMain.TabPages[0].Text = Strings.ContectSender;
            tabMain.TabPages[1].Text = Strings.GroupSender;
            tabMain.TabPages[2].Text = Strings.Tools;
            materialLabel1.Text = Strings.Target;
            materialLabel19.Text = Strings.Target;
            materialLabel10.Text = Strings.Target;
            btnUploadExcel.Text = Strings.UploadSampleExcel;
            btnDownloadSample.Text = Strings.DownloadSample;
            materialLabel2.Text = Strings.Messages;
            materialTabControl2.TabPages[0].Text = Strings.MessageOne;
            materialTabControl2.TabPages[1].Text = Strings.MessageTwo;
            materialTabControl2.TabPages[2].Text = Strings.MessageThree;
            materialTabControl2.TabPages[3].Text = Strings.MessageFour;
            materialTabControl2.TabPages[4].Text = Strings.MessageFive;
            groupBox1.Text = Strings.Attachments;
            groupBox2.Text = Strings.Attachments;
            groupBox3.Text = Strings.Attachments;
            groupBox4.Text = Strings.Attachments;
            groupBox5.Text = Strings.Attachments;

            groupBox11.Text = Strings.Polls;
            groupBox12.Text = Strings.Polls;
            groupBox13.Text = Strings.Polls;
            groupBox14.Text = Strings.Polls;
            groupBox15.Text = Strings.Polls;
            groupBox16.Text = Strings.Polls;
            groupBox17.Text = Strings.Polls;
            groupBox18.Text = Strings.Polls;
            groupBox19.Text = Strings.Polls;
            groupBox20.Text = Strings.Polls;

            btnAddFileOne.Text = Strings.Addfile;
            btnAddFileTwo.Text = Strings.Addfile;
            btnAddFileThree.Text = Strings.Addfile;
            btnAddFileFour.Text = Strings.Addfile;
            btnAddFileFive.Text = Strings.Addfile;

            txtMsgOne.Hint = Strings.Yourfirstmessage;
            txtMsgTwo.Hint = Strings.YourSecondmessage;
            txtMsgThree.Hint = Strings.YourThirdmessage;
            txtMsgFour.Hint = Strings.YourFourthmessage;
            txtMsgFive.Hint = Strings.YourFifthmessage;


            txtMsgOneGroup.Hint = Strings.Yourfirstmessage;
            txtMsgTwoGroup.Hint = Strings.YourSecondmessage;
            txtMsgTHreeGroup.Hint = Strings.YourThirdmessage;
            txtMsgFourGroup.Hint = Strings.YourFourthmessage;
            txtMsgFiveGroup.Hint = Strings.YourFifthmessage;

            De.Text = Strings.DelaySettings;
            materialLabel3.Text = Strings.Wait;
            materialLabel9.Text = Strings.Wait;
            materialLabel5.Text = Strings.secondsafterevery;
            materialLabel4.Text = Strings.to;
            materialLabel8.Text = Strings.to;
            materialLabel6.Text = Strings.Messages;
            materialLabel7.Text = Strings.secondsbeforeeverymessage;

            materialButton1.Text = Strings.Clear;
            btnStart.Text = Strings.StartCampaign;

            contextMenuStrip1.Items[0].Text = Strings.AddKeyMarkers;
            contextMenuStrip1.Items[1].Text = Strings.RandomNumber;
            contextMenuStrip1.Items[2].Text = Strings.BuiltInVariable;


            btnUploadExcelGroup.Text = Strings.UploadSampleExcel;
            materialLabel1.Text = Strings.Target;
            gridTargetsGroup.Columns[0].HeaderText = Strings.GroupNames;

            gridTargets.Columns[0].HeaderText = Strings.Number;
            gridTargets.Columns[1].HeaderText = Strings.Name;

            btnDownloadSampleGroup.Text = Strings.DownloadSample;
            materialLabel18.Text = Strings.Message;
            materialLabel2.Text = Strings.Message;

            materialTabControl1.TabPages[0].Text = Strings.MessageOne;
            materialTabControl1.TabPages[1].Text = Strings.MessageTwo;
            materialTabControl1.TabPages[2].Text = Strings.MessageThree;
            materialTabControl1.TabPages[3].Text = Strings.MessageFour;
            materialTabControl1.TabPages[4].Text = Strings.MessageFive;

            groupBox6.Text = Strings.Attachments;
            groupBox7.Text = Strings.Attachments;
            groupBox8.Text = Strings.Attachments;
            groupBox9.Text = Strings.Attachments;
            groupBox10.Text = Strings.Attachments;

            materialButton4.Text = Strings.Addfile;
            materialButton5.Text = Strings.Addfile;
            materialButton6.Text = Strings.Addfile;
            materialButton7.Text = Strings.Addfile;
            materialButton8.Text = Strings.Addfile;

            materialLabel17.Text = Strings.DelaySettings;
            materialLabel16.Text = Strings.Wait;
            materialLabel12.Text = Strings.Wait;

            materialLabel15.Text = Strings.to;
            materialLabel11.Text = Strings.to;
            materialLabel14.Text = Strings.secondsafterevery;
            materialLabel13.Text = Strings.Messages;
            materialLabel10.Text = Strings.secondsbeforeeverymessage;
            materialButton2.Text = Strings.Clear;
            btnStartGroup.Text = Strings.StartCampaign;



            addCountryCodeToolStripMenuItem.Text = Strings.AddCountryCode;
            importNumbersToolStripMenuItem.Text = Strings.CopyPasteNumber;
            removeDuplicatesToolStripMenuItem.Text = Strings.RemoveDuplicateNumbers;
            deleteAllRowsToolStripMenuItem.Text = Strings.DeleteAllRows;

            materialButton19.Text = Strings.AddPoll;
            materialButton20.Text = Strings.AddPoll;
            materialButton21.Text = Strings.AddPoll;
            materialButton22.Text = Strings.AddPoll;
            materialButton23.Text = Strings.AddPoll;

            materialButton26.Text = Strings.AddPoll;
            materialButton27.Text = Strings.AddPoll;
            materialButton28.Text = Strings.AddPoll;
            materialButton29.Text = Strings.AddPoll;
            materialButton30.Text = Strings.AddPoll;


            contextMenuStrip3.Items[0].Text = Strings.AddCaption;



            materialButton34.Text = Strings.Accounts;
            changeGridHeaders(dataGridView1);
            changeGridHeaders(dataGridView2);
            changeGridHeaders(dataGridView3);
            changeGridHeaders(dataGridView4);
            changeGridHeaders(dataGridView5);
            changeGridHeaders(dataGridView6);
            changeGridHeaders(dataGridView7);
            changeGridHeaders(dataGridView8);
            changeGridHeaders(dataGridView9);
            changeGridHeaders(dataGridView10);

            groupBox21.Text = groupBox22.Text =groupBox23.Text= groupBox24.Text =groupBox25.Text= Strings.Buttons;
            materialButton3.Text = materialButton9.Text = materialButton10.Text = materialButton11.Text = materialButton12 .Text= Strings.AddButton;

            if (!Strings.EnableButtons)
            {
                remove_buttonsPanel(tableLayoutPanel1);
                remove_buttonsPanel(tableLayoutPanel2);
                remove_buttonsPanel(tableLayoutPanel3);
                remove_buttonsPanel(tableLayoutPanel4);
                remove_buttonsPanel(tableLayoutPanel5);
            }
        }

        private static void remove_buttonsPanel(TableLayoutPanel tableLayoutPanel2)
        {
            List<Control> Col_2_Stuff = tableLayoutPanel2.Controls.OfType<Control>().Where(x => tableLayoutPanel2.GetPositionFromControl(x).Column == 1).ToList();
            Col_2_Stuff.Select(c => { c.Visible = false; c = null; return c; }).ToList();

            tableLayoutPanel2.ColumnStyles[0].SizeType = SizeType.Percent;
            tableLayoutPanel2.ColumnStyles[0].Width = 100;
        }

        private void changeGridHeaders(DataGridView dg)
        {
            dg.Columns[0].HeaderText = Strings._File;
            dg.Columns[1].HeaderText = Strings.Caption;
            dg.Columns[2].HeaderText = Strings.AttachWithMainMessage;
        }

        private void defaultColorSchime()
        {
            lblSection.BackColor = System.Drawing.Color.Transparent;
            lblSection.ForeColor = System.Drawing.Color.White;
            label1.BackColor = System.Drawing.Color.Transparent;
            label1.ForeColor = System.Drawing.Color.White;
        }

        private void tabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMain.SelectedIndex == 0)
            {
                lblSection.Text = Strings.ContectSender;
            }
            if (tabMain.SelectedIndex == 1)
            {
                lblSection.Text = Strings.GroupSender;
            }
            if (tabMain.SelectedIndex == 2)
            {
                lblSection.Text = Strings.Tools;
            }

        }

        private void chkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            defaultColorSchime();
        }

        private void btnDownloadSample_Click(object sender, EventArgs e)
        {
            savesampleExceldialog.FileName = "SingleSenderTemplate.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy("templets/SingleSenderTemplate.xlsx", savesampleExceldialog.FileName, true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }
        public void ReturnPasteNumber(List<string> numbers)
        {
            var globalCounter = gridTargets.Rows.Count - 1;
            for (int i = 0; i < numbers.Count(); i++)
            {
                try
                {
                    gridTargets.Rows.Add();
                    string MobileNumber = numbers[i];
                    MobileNumber = MobileNumber.Replace("+", "");
                    MobileNumber = MobileNumber.Replace(" ", "");
                    MobileNumber = MobileNumber.Replace(" ", "");
                    gridTargets.Rows[globalCounter].Cells[0].Value = MobileNumber;
                    globalCounter++;
                }
                catch (Exception ec)
                {

                }
            }
            tabMain.SelectedIndex = 0;
        }
        private void btnUploadExcel_Click(object sender, EventArgs e)
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
                if ((fi.Extension != ".xlsx"))
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

                        var globalCounter = gridTargets.Rows.Count - 1;
                        var ColumnsCOunt = worksheet.Dimension.Columns;
                        if (ColumnsCOunt > 2)
                        {
                            for (int i = 3; i <= ColumnsCOunt; i++)
                            {
                                try
                                {
                                    string Header = worksheet.Cells[1, i].Value.ToString();
                                    gridTargets.Columns.Add("NewColumn" + i, Header);
                                }
                                catch (Exception ex)
                                {
                                    string exs = "";
                                }
                            }
                        }


                        for (int i = 1; i < worksheet.Dimension.Rows; i++)
                        {

                            try
                            {
                                gridTargets.Rows.Add();

                                string MobileNumber = worksheet.Cells[i + 1, 1].Value.ToString();
                                try
                                {
                                    MobileNumber = MobileNumber.Replace("+", "");
                                    MobileNumber = MobileNumber.Replace(" ", "");
                                    MobileNumber = MobileNumber.Replace(" ", "");
                                    Int64 temp = Convert.ToInt64(MobileNumber);
                                }
                                catch (Exception ex)
                                {

                                }


                                string name = "";
                                try
                                {
                                    name = worksheet.Cells[i + 1, 2].Value.ToString();
                                }
                                catch (Exception ex)
                                {

                                }

                                gridTargets.Rows[globalCounter].Cells[0].Value = MobileNumber;
                                gridTargets.Rows[globalCounter].Cells[1].Value = name;

                                try
                                {
                                    if (ColumnsCOunt > 1)
                                    {
                                        for (int j = 2; j <= ColumnsCOunt; j++)
                                        {
                                            try
                                            {
                                                string CelValue = worksheet.Cells[i + 1, j].Value.ToString();
                                                gridTargets.Rows[globalCounter].Cells[j - 1].Value = CelValue;
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            catch (Exception ex)
                            {
                                string ss = "";
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

        private void setCounter()
        {
            lblCount.Text = (gridTargets.Rows.Count - 1).ToString();
        }

        private void setCounterGroup()
        {
            lblCountGroup.Text = (gridTargetsGroup.Rows.Count - 1).ToString();
        }

        private void gridTargets_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            setCounter();
        }

        private void gridTargets_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            setCounter();
        }

        private void btnAddFileOne_Click_1(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView1);
        }

        private void btnAddFileTwo_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView2);
        }

        private void btnAddFileThree_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView3);
        }

        private void btnAddFileFour_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView4);
        }

        private void btnAddFileFive_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView5);
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            clearAll();
        }

        public void clearAll()
        {
            gridTargets.Rows.Clear();

            if (gridTargets.Columns.Count > 2)
            {
                try
                {
                    while (gridTargets.Columns.Count > 2)
                    {
                        gridTargets.Columns.Remove(gridTargets.Columns[gridTargets.Columns.Count - 1]);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            txtMsgOne.Text = "";
            txtMsgTwo.Text = "";
            txtMsgThree.Text = "";
            txtMsgFour.Text = "";
            txtMsgFive.Text = "";

            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            dataGridView4.Rows.Clear();
            dataGridView5.Rows.Clear();



            webBrowser1.DocumentText = Storage.DocumentHtmlString;

            pollModelList1 = new List<PollModel>();

            webBrowser2.DocumentText = Storage.DocumentHtmlString;

            pollModelList2 = new List<PollModel>();

            webBrowser3.DocumentText = Storage.DocumentHtmlString;

            pollModelList3 = new List<PollModel>();

            webBrowser4.DocumentText = Storage.DocumentHtmlString;

            pollModelList4 = new List<PollModel>();

            webBrowser5.DocumentText = Storage.DocumentHtmlString;

            pollModelList5 = new List<PollModel>();


            this.schedulesModel = null;
            this.wASenderSingleTransModel = null;

            this.tmpfriendlyNumbers = null;
            this.tmpsendTofriendlyNumbersAfterMessages = 0;


            _btnBrowser1.DocumentText = Storage.DocumentHtmlString;
            buttonsModelList1 = new List<ButtonHolderModel>();
            materialButton3.Enabled = true;

            _btnBrowser2.DocumentText = Storage.DocumentHtmlString;
            buttonsModelList2 = new List<ButtonHolderModel>();
            materialButton9.Enabled = true;


            _btnBrowser3.DocumentText = Storage.DocumentHtmlString;
            buttonsModelList3 = new List<ButtonHolderModel>();
            materialButton10.Enabled = true;

            _btnBrowser4.DocumentText = Storage.DocumentHtmlString;
            buttonsModelList4 = new List<ButtonHolderModel>();
            materialButton11.Enabled = true;

            _btnBrowser5.DocumentText = Storage.DocumentHtmlString;
            buttonsModelList5 = new List<ButtonHolderModel>();
            materialButton12.Enabled = true;

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            ValidateControls();
        }


        private void ValidateControlsGroup(bool isValidateOnly = false)
        {
            if (wASenderGroupTransModel == null)
            {
                wASenderGroupTransModel = new WASenderGroupTransModel();
            }

            wASenderGroupTransModel.groupList = new List<GroupModel>();
            GroupModel group = new GroupModel();

            for (int i = 0; i < gridTargetsGroup.Rows.Count; i++)
            {
                if (!(gridTargetsGroup.Rows[i].Cells[0].Value == null))
                {
                    group = new GroupModel();
                    group.Name = gridTargetsGroup.Rows[i].Cells[0].Value == null ? "" : gridTargetsGroup.Rows[i].Cells[0].Value.ToString();
                    group.GroupId = gridTargetsGroup.Rows[i].Cells[1].Value == null ? "" : gridTargetsGroup.Rows[i].Cells[1].Value.ToString();

                    try
                    {
                        if (gridTargetsGroup.Rows[i].Cells[2].Value.ToString() == "True")
                        {
                            group.CanSend = true;
                        }
                        else if (gridTargetsGroup.Rows[i].Cells[2].Value.ToString() == "False")
                        {
                            group.CanSend = false;
                        }
                        else
                        {
                            group.CanSend = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        group.CanSend = null;
                    }

                    group.sendStatusModel = new SendStatusModel { isDone = false };
                    //group = new GroupModel
                    //{
                    //    Name = gridTargetsGroup.Rows[i].Cells[0].Value == null ? "" : gridTargetsGroup.Rows[i].Cells[0].Value.ToString(),
                    //    GroupId = gridTargetsGroup.Rows[i].Cells[1].Value == null ? "" : gridTargetsGroup.Rows[i].Cells[1].Value.ToString(),
                    //    CanSend = gridTargetsGroup.Rows[i].Cells[2].Value == null ? null : (gridTargetsGroup.Rows[i].Cells[2].Value.ToString()=="True"?true:false),
                    //    sendStatusModel = new SendStatusModel { isDone = false }
                    //};

                    group.validationFailures = new GroupModelValidator().Validate(group).Errors;
                    wASenderGroupTransModel.groupList.Add(group);
                }
            }


            wASenderGroupTransModel.settings = new SingleSettingModel();
            wASenderGroupTransModel.settings.delayAfterMessages = Convert.ToInt32(txtdelayAfterMessagesGroup.Text);
            wASenderGroupTransModel.settings.delayAfterMessagesFrom = Convert.ToInt32(txtdelayAfterMessagesFromGroup.Text);
            wASenderGroupTransModel.settings.delayAfterMessagesTo = Convert.ToInt32(txtdelayAfterMessagesToGroup.Text);
            wASenderGroupTransModel.settings.delayAfterEveryMessageFrom = Convert.ToInt32(txtdelayAfterEveryMessageFromGroup.Text);
            wASenderGroupTransModel.settings.delayAfterEveryMessageTo = Convert.ToInt32(txtdelayAfterEveryMessageToGroup.Text);

            wASenderGroupTransModel.settings.validationFailures = new SingleSettingModelValidator().Validate(wASenderGroupTransModel.settings).Errors;


            wASenderGroupTransModel.messages = new List<MesageModel>();
            wASenderGroupTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgOneGroup, dataGridView6, pollModelList6));
            wASenderGroupTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgTwoGroup, dataGridView7, pollModelList7));
            wASenderGroupTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgTHreeGroup, dataGridView8, pollModelList8));
            wASenderGroupTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgFourGroup, dataGridView9, pollModelList9));
            wASenderGroupTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgFiveGroup, dataGridView10, pollModelList10));
            wASenderGroupTransModel.validationFailures = new WASenderGroupTransModelValidator().Validate(wASenderGroupTransModel).Errors;

            if (showValidationErrorIfAnyGroup())
            {
                if (isValidateOnly == false)
                {
                    GroupLauncher form = new GroupLauncher(wASenderGroupTransModel, this, this.schedulesModel);
                    form.ShowDialog();
                }

            }

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        public void receiveFriendlyNumbersData(List<string> friendlyNumbers, int sendTofriendlyNumbersAfterMessages)
        {
            this.wASenderSingleTransModel.friendlyNumbers = friendlyNumbers;
            this.wASenderSingleTransModel.sendTofriendlyNumbersAfterMessages = sendTofriendlyNumbersAfterMessages;
        }

        private void ValidateControls(bool onlySave = false)
        {
            if (wASenderSingleTransModel == null)
            {
                wASenderSingleTransModel = new WASenderSingleTransModel();
            }

            wASenderSingleTransModel.contactList = new List<ContactModel>();
            ContactModel contact;

            for (int i = 0; i < gridTargets.Rows.Count; i++)
            {
                if (!(gridTargets.Rows[i].Cells[0].Value == null && gridTargets.Rows[i].Cells[1].Value == null))
                {
                    contact = new ContactModel
                    {
                        number = gridTargets.Rows[i].Cells[0].Value == null ? "" : gridTargets.Rows[i].Cells[0].Value.ToString(),
                        name = gridTargets.Rows[i].Cells[1].Value == null ? "" : gridTargets.Rows[i].Cells[1].Value.ToString(),
                        sendStatusModel = new SendStatusModel { isDone = false }
                    };

                    if (gridTargets.ColumnCount > 1)
                    {
                        contact.parameterModelList = new List<ParameterModel>();
                        ParameterModel parameterModel;
                        for (int j = 2; j <= gridTargets.ColumnCount; j++)
                        {
                            try
                            {
                                string cellValue = gridTargets.Rows[i].Cells[j - 1].Value.ToString();
                                string cellHeader = gridTargets.Columns[j - 1].HeaderText;
                                parameterModel = new ParameterModel();
                                parameterModel.ParameterName = cellHeader;
                                parameterModel.ParameterValue = cellValue;
                                contact.parameterModelList.Add(parameterModel);
                            }
                            catch (Exception ex)
                            {


                            }
                        }
                    }


                    contact.validationFailures = new ContactModelValidator().Validate(contact).Errors;
                    wASenderSingleTransModel.contactList.Add(contact);
                }
            }

            wASenderSingleTransModel.settings = new SingleSettingModel();
            wASenderSingleTransModel.settings.delayAfterMessages = Convert.ToInt32(txtdelayAfterMessages.Text);
            wASenderSingleTransModel.settings.delayAfterMessagesFrom = Convert.ToInt32(txtdelayAfterMessagesFrom.Text);
            wASenderSingleTransModel.settings.delayAfterMessagesTo = Convert.ToInt32(txtdelayAfterMessagesTo.Text);
            wASenderSingleTransModel.settings.delayAfterEveryMessageFrom = Convert.ToInt32(txtdelayAfterEveryMessageFrom.Text);
            wASenderSingleTransModel.settings.delayAfterEveryMessageTo = Convert.ToInt32(txtdelayAfterEveryMessageTo.Text);

            wASenderSingleTransModel.settings.validationFailures = new SingleSettingModelValidator().Validate(wASenderSingleTransModel.settings).Errors;

            wASenderSingleTransModel.messages = new List<MesageModel>();
            wASenderSingleTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgOne, dataGridView1, pollModelList1, buttonsModelList1));
            wASenderSingleTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgTwo, dataGridView2, pollModelList2, buttonsModelList2));
            wASenderSingleTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgThree, dataGridView3, pollModelList3, buttonsModelList3));
            wASenderSingleTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgFour, dataGridView4, pollModelList4, buttonsModelList4));
            wASenderSingleTransModel.messages.Add(CheckAndSendtoMessageModel(txtMsgFive, dataGridView5, pollModelList5, buttonsModelList5));

            foreach (MesageModel mesageModel in wASenderSingleTransModel.messages)
            {
                if (mesageModel != null)
                    mesageModel.validationFailures = new MesageModelValidator().Validate(mesageModel).Errors;
            }

            wASenderSingleTransModel.validationFailures = new WASenderSingleTransModelValidator().Validate(wASenderSingleTransModel).Errors;

            wASenderSingleTransModel.friendlyNumbers = tmpfriendlyNumbers;
            wASenderSingleTransModel.sendTofriendlyNumbersAfterMessages = tmpsendTofriendlyNumbersAfterMessages;

            if (showValidationErrorIfAny())
            {
                if (onlySave == false)
                {
                    SingleLauncher launcher = new SingleLauncher(wASenderSingleTransModel, this, this.schedulesModel);
                    launcher.ShowDialog();
                }
            }
        }


        public void formReturn(bool success)
        {
            this.Show();
        }

        public void ImportNumbers(List<string> numbers)
        {
            gridTargets.Rows.Clear();
            int j = 0;
            foreach (string number in numbers)
            {
                gridTargets.Rows.Add();
                gridTargets.Rows[j].Cells[0].Value = number;
                j++;
            }
            tabMain.SelectedIndex = 0;
        }
        public void gmapDataReturn(List<GMapModel> gmapModel)
        {
            this.Show();
            gridTargets.Rows.Add();
            for (int i = 0; i < gmapModel.Where(x => x.mobilenumber != "" && x.mobilenumber != null).Count(); i++)
            {
                gridTargets.Rows.Add();
            }

            int j = 0;
            foreach (var item in gmapModel.Where(x => x.mobilenumber != "" && x.mobilenumber != null))
            {
                try
                {
                    string MobileNumber = item.mobilenumber;
                    MobileNumber = MobileNumber.Replace("+", "");
                    MobileNumber = MobileNumber.Replace(" ", "");
                    MobileNumber = MobileNumber.Replace(" ", "");
                    gridTargets.Rows[j].Cells[0].Value = MobileNumber;
                    j++;
                }
                catch (Exception)
                {

                }
            }

            tabMain.SelectedIndex = 0;
        }

        private bool showValidationErrorIfAnyGroup()
        {
            bool validationFail = true;
            if (CheckValidationMessage(wASenderGroupTransModel.validationFailures))
            {
                if (CheckValidationMessage(wASenderGroupTransModel.settings.validationFailures))
                {
                    for (int i = 0; i < wASenderGroupTransModel.groupList.Count(); i++)
                    {
                        if (CheckValidationMessage(wASenderGroupTransModel.groupList[i].validationFailures, Strings.RowNo + " - " + Convert.ToString(i + 1)))
                        {
                            string ss = "";
                        }
                        else
                        {
                            i = wASenderGroupTransModel.groupList.Count;
                            validationFail = false;
                        }
                    }
                }
                else
                {
                    validationFail = false;
                }
            }
            else
            {
                validationFail = false;
            }
            return validationFail;
        }

        private bool showValidationErrorIfAny()
        {
            bool validationFail = true;
            if (CheckValidationMessage(wASenderSingleTransModel.validationFailures))
            {
                if (CheckValidationMessage(wASenderSingleTransModel.settings.validationFailures))
                {

                    foreach (MesageModel message in wASenderSingleTransModel.messages)
                    {
                        if (message != null)
                        {
                            if (!CheckValidationMessage(message.validationFailures))
                            {
                                validationFail = false;
                            }
                        }
                    }

                    for (int i = 0; i < wASenderSingleTransModel.contactList.Count(); i++)
                    {
                        if (CheckValidationMessage(wASenderSingleTransModel.contactList[i].validationFailures, Strings.RowNo + "- " + Convert.ToString(i + 1)))
                        {
                            string ss = "";
                        }
                        else
                        {
                            i = wASenderSingleTransModel.contactList.Count;
                            validationFail = false;
                        }
                    }
                }
                else
                {
                    validationFail = false;
                }
            }
            else
            {
                validationFail = false;
            }
            return validationFail;
        }

        private bool CheckValidationMessage(IList<ValidationFailure> validationFailures, string AdditionalMessage = "")
        {
            string Messages = "";
            if (validationFailures != null && validationFailures.Count() > 0)
            {
                foreach (var item in validationFailures)
                {
                    Messages = Messages + item.ErrorMessage + "\n\n";
                }
            }
            if (Messages == "")
            {
                return true;
            }
            else
            {
                MessageBox.Show(AdditionalMessage + " " + Messages, Strings.Errors, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private MesageModel CheckAndSendtoMessageModel(MaterialMultiLineTextBox2 txtMsg, DataGridView list, List<PollModel> _pollModel = null, List<ButtonHolderModel> _buttons = null)
        {
            MesageModel mesageModel;

            List<FilesModel> files = new List<FilesModel>();

            foreach (DataGridViewRow item in list.Rows)
            {
                try
                {
                    FilesModel filesModel;
                    bool IsAttachwithMainMessage = false;
                    if (item.Cells[2].Value != null)
                    {
                        if (item.Cells[2].Value.ToString() == "True")
                        {
                            IsAttachwithMainMessage = true;
                        }
                    }

                    if (item.Cells[1].Value.ToString() != null && item.Cells[1].Value.ToString() != "")
                    {
                        filesModel = new FilesModel
                        {
                            filePath = item.Cells[0].Value.ToString(),
                            attachWithMainMessage = false,
                            Caption = item.Cells[1].Value.ToString()
                        };
                        files.Add(filesModel);
                    }
                    else if (IsAttachwithMainMessage == true)
                    {
                        filesModel = new FilesModel
                        {
                            filePath = item.Cells[0].Value.ToString(),
                            attachWithMainMessage = true
                        };
                        files.Add(filesModel);
                    }
                    else
                    {
                        files.Add(new FilesModel { filePath = item.Cells[0].Value.ToString(), Caption = item.Cells[1].Value.ToString() });
                    }
                }
                catch (Exception ex)
                {

                }
            }

            if ((txtMsg.Text != null && txtMsg.Text != "") || (files.Count() > 0))
            {
                mesageModel = new MesageModel();
                mesageModel.longMessage = txtMsg.Text;

                mesageModel.files = files;

                //mesageModel.buttons = _buttonsModel;

                if (_pollModel != null)
                {
                    mesageModel.polls = _pollModel;
                }
                if (_buttons != null)
                {
                    mesageModel.buttons = _buttons;
                }

                return mesageModel;
            }

            else
            {
                return null;
            }
        }

        private void btnGroupDownloadExcel_Click(object sender, EventArgs e)
        {
            savesampleExceldialog.FileName = "GroupSenderTemplate.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy("GroupSenderTemplate.xlsx", savesampleExceldialog.FileName, true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }

        private void btnUploadExcelGroup_Click(object sender, EventArgs e)
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
                if ((fi.Extension != ".xlsx"))
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

                        var globalCounter = gridTargetsGroup.Rows.Count - 1;
                        for (int i = 1; i < worksheet.Dimension.Rows; i++)
                        {

                            gridTargetsGroup.Rows.Add();
                            gridTargetsGroup.Rows[globalCounter].Cells[0].Value = worksheet.Cells[i + 1, 1].Value.ToString();
                            gridTargetsGroup.Rows[globalCounter].Cells[1].Value = worksheet.Cells[i + 1, 2].Value.ToString();

                            try
                            {
                                gridTargetsGroup.Rows[globalCounter].Cells[2].Value = worksheet.Cells[i + 1, 3].Value.ToString();
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

        private void gridTargetsGroup_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            setCounterGroup();
        }

        private void gridTargetsGroup_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            setCounterGroup();
        }

        private void materialButton8_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView10);
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView6);
        }

        private void materialButton5_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView7);
        }

        private void materialButton6_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView8);
        }

        private void materialButton7_Click(object sender, EventArgs e)
        {
            Utils.selectFileForMessage(dataGridView9);
        }

        private void btnStartGroup_Click(object sender, EventArgs e)
        {
            ValidateControlsGroup();
        }


        private void materialButton2_Click(object sender, EventArgs e)
        {
            clearAllGroup();
        }

        public void clearAllGroup()
        {
            gridTargetsGroup.Rows.Clear();
            dataGridView6.Rows.Clear();
            dataGridView7.Rows.Clear();
            dataGridView8.Rows.Clear();
            dataGridView9.Rows.Clear();
            dataGridView10.Rows.Clear();

            txtMsgOneGroup.Text = "";
            txtMsgTwoGroup.Text = "";
            txtMsgTHreeGroup.Text = "";
            txtMsgFourGroup.Text = "";
            txtMsgFiveGroup.Text = "";

            wASenderGroupTransModel = null;
            this.schedulesModel = null;
        }



        private void materialButton13_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(materialButton13, new Point(0, materialButton13.Height));
        }

        private void keyMarkersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            KeyMarker keyMarker = new KeyMarker(this);
            keyMarker.ShowDialog();
        }

        public void AddKeyMarker(string KeyMarker)
        {
            SingleInsertKeyParams(KeyMarker);
        }

        private void SingleInsertKeyParams(string paramVals)
        {

            int MainTabIndex = tabMain.SelectedIndex;
            if (MainTabIndex == 0)
            {
                int tabIndex = materialTabControl2.SelectedIndex;
                if (tabIndex == 0)
                {
                    txtMsgOne.Text += paramVals;
                }
                else if (tabIndex == 1)
                {
                    txtMsgTwo.Text += paramVals;
                }
                else if (tabIndex == 2)
                {
                    txtMsgThree.Text += paramVals;
                }
                else if (tabIndex == 3)
                {
                    txtMsgFour.Text += paramVals;
                }
                else if (tabIndex == 4)
                {
                    txtMsgFive.Text += paramVals;
                }
            }
            else if (MainTabIndex == 1)
            {
                int tabIndex = materialTabControl1.SelectedIndex;
                if (tabIndex == 0)
                {
                    txtMsgOneGroup.Text += paramVals;
                }
                else if (tabIndex == 1)
                {
                    txtMsgTwoGroup.Text += paramVals;
                }
                else if (tabIndex == 2)
                {
                    txtMsgTHreeGroup.Text += paramVals;
                }
                else if (tabIndex == 3)
                {
                    txtMsgFourGroup.Text += paramVals;
                }
                else if (tabIndex == 4)
                {
                    txtMsgFiveGroup.Text += paramVals;
                }
            }

        }

        private void randomNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SingleInsertKeyParams("{{ RANDOM }}");
        }

        private void materialButton14_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(materialButton13, new Point(0, materialButton13.Height));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            generalSettingsModel.selectedLanguage = comboBox1.Text;

            String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();
            if (!File.Exists(GetGeneralSettingsFilePath))
            {
                File.Create(GetGeneralSettingsFilePath).Close();
            }

            string Json = JsonConvert.SerializeObject(generalSettingsModel, Formatting.Indented);
            File.WriteAllText(GetGeneralSettingsFilePath, Json);

            MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.LanguageIsSet, Strings.OK, true);
            SnackBarMessage.Show(this);
        }


        private void gridTargets_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(gridTargets, new Point(e.Location.X, e.Location.Y));
            }
        }

        private void addCountryCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CountryCodeInput countryCodeInput = new CountryCodeInput(this);
            countryCodeInput.ShowDialog();
        }

        public void reEnableAutoReply()
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
            WaAutoReplyBot.WaAutoReplyForm waAutoReplyForm = new WaAutoReplyBot.WaAutoReplyForm(this, true);
            this.Hide();
            waAutoReplyForm.ShowDialog();
        }

        private void ShowAddPollDialog()
        {
            PollModel model = new PollModel();
            AddPoll form = new AddPoll(model, this);
            form.ShowDialog();
        }


        private void commonAddPollList(PollModel _pollModel, List<PollModel> _buttonsModelList)
        {
            if (_pollModel.editMode == true)
            {
                int index = _buttonsModelList.FindIndex(x => x.id == _pollModel.id);
                _pollModel.editMode = false;
                _buttonsModelList[index] = _pollModel;
            }
            else
            {
                _buttonsModelList.Add(_pollModel);
            }
        }
        private void commonAddButtonList(ButtonsModel _buttonsModel, List<ButtonsModel> _buttonsModelList)
        {
            if (_buttonsModel.editMode == true)
            {
                int index = _buttonsModelList.FindIndex(x => x.id == _buttonsModel.id);
                _buttonsModel.editMode = false;
                _buttonsModelList[index] = _buttonsModel;
            }
            else
            {
                _buttonsModelList.Add(_buttonsModel);
            }
        }


        private void commonRemovePollList(PollModel _buttonsModel, List<PollModel> _buttonsModelList)
        {
            int index = _buttonsModelList.FindIndex(x => x.id == _buttonsModel.id);
            _buttonsModelList.Remove(_buttonsModelList[index]);

        }

        private void commonRemoveButtonList(ButtonsModel _buttonsModel, List<ButtonsModel> _buttonsModelList)
        {
            int index = _buttonsModelList.FindIndex(x => x.id == _buttonsModel.id);
            _buttonsModelList.Remove(_buttonsModelList[index]);

        }

        public void RemovePoll(PollModel buttonsModel)
        {

            int TabIndex = tabMain.SelectedIndex;
            if (TabIndex == 0)
            {
                int MainTabIndex = materialTabControl2.SelectedIndex;
                if (MainTabIndex == 0)
                {
                    commonRemovePollList(buttonsModel, pollModelList1);
                }
                if (MainTabIndex == 1)
                {
                    commonRemovePollList(buttonsModel, pollModelList2);
                }
                if (MainTabIndex == 2)
                {
                    commonRemovePollList(buttonsModel, pollModelList3);
                }
                if (MainTabIndex == 3)
                {
                    commonRemovePollList(buttonsModel, pollModelList4);
                }
                if (MainTabIndex == 4)
                {
                    commonRemovePollList(buttonsModel, pollModelList5);
                }

                geteratePolls();
            }
            else if (TabIndex == 1)
            {
                int MainTabIndex = materialTabControl1.SelectedIndex;
                if (MainTabIndex == 0)
                {
                    commonRemovePollList(buttonsModel, pollModelList6);
                }
                if (MainTabIndex == 1)
                {
                    commonRemovePollList(buttonsModel, pollModelList7);
                }
                if (MainTabIndex == 2)
                {
                    commonRemovePollList(buttonsModel, pollModelList8);
                }
                if (MainTabIndex == 3)
                {
                    commonRemovePollList(buttonsModel, pollModelList9);
                }
                if (MainTabIndex == 4)
                {
                    commonRemovePollList(buttonsModel, pollModelList10);
                }
                geteratePollsGroup();
            }


        }



        public void RecievPoll(PollModel pollModel, int? _MainTabIndex = null)
        {
            int topTabIndex = tabMain.SelectedIndex;
            if (topTabIndex == 0)
            {
                int MainTabIndex = materialTabControl2.SelectedIndex;
                if (_MainTabIndex != null)
                {
                    MainTabIndex = (int)_MainTabIndex;
                }
                if (MainTabIndex == 0)
                {
                    commonAddPollList(pollModel, pollModelList1);
                }
                else if (MainTabIndex == 1)
                {
                    commonAddPollList(pollModel, pollModelList2);
                }
                else if (MainTabIndex == 2)
                {
                    commonAddPollList(pollModel, pollModelList3);
                }
                else if (MainTabIndex == 3)
                {
                    commonAddPollList(pollModel, pollModelList4);
                }
                else if (MainTabIndex == 4)
                {
                    commonAddPollList(pollModel, pollModelList5);
                }

                geteratePolls(_MainTabIndex);

            }
            else if (topTabIndex == 1)
            {
                int MainTabIndex = materialTabControl1.SelectedIndex;
                if (_MainTabIndex != null)
                {
                    MainTabIndex = (int)_MainTabIndex;
                }
                if (MainTabIndex == 0)
                {
                    commonAddPollList(pollModel, pollModelList6);
                }
                if (MainTabIndex == 1)
                {
                    commonAddPollList(pollModel, pollModelList7);
                }
                if (MainTabIndex == 2)
                {
                    commonAddPollList(pollModel, pollModelList8);
                }
                if (MainTabIndex == 3)
                {
                    commonAddPollList(pollModel, pollModelList9);
                }
                if (MainTabIndex == 4)
                {
                    commonAddPollList(pollModel, pollModelList10);
                }
                geteratePollsGroup(_MainTabIndex);
            }
        }


        private void CommonGeneratePolls(WebBrowser webBrowser, List<PollModel> buttonsModelList)
        {
            string buttontext = Storage.DocumentHtmlString;
            string cssStyle = Storage.DocumentButtonStypeStrig;

            foreach (var item in buttonsModelList)
            {
                string txt = "🗳️";


                txt = "🗳️ " + item.PollName;

                buttontext += "<button style='margin:5px;" + cssStyle + "' type='button' id='" + item.id + "' >" + txt + "</button>";
            }
            webBrowser.DocumentText = buttontext + "</body></html>";
        }
        private void CommonGenerateButtons(WebBrowser webBrowser, List<ButtonsModel> buttonsModelList)
        {
            string buttontext = Storage.DocumentHtmlString;
            string cssStyle = Storage.DocumentButtonStypeStrig;

            foreach (var item in buttonsModelList)
            {
                string txt = "";

                if (item.buttonTypeEnum == enums.ButtonTypeEnum.PHONE_NUMBER)
                {
                    txt = "📞" + item.text;
                }
                else if (item.buttonTypeEnum == enums.ButtonTypeEnum.URL)
                {
                    txt = "🔗 " + item.text;
                }
                else
                {
                    txt = item.text;
                }
                buttontext += "<button style='margin:5px;" + cssStyle + "' type='button' id='" + item.id + "' >" + txt + "</button>";
            }
            webBrowser.DocumentText = buttontext + "</body></html>";
        }


        private void geteratePollsGroup(int? _MainTabIndex = null)
        {
            int MainTabIndex = materialTabControl1.SelectedIndex;
            if (_MainTabIndex != null)
            {
                MainTabIndex = (int)_MainTabIndex;
            }

            if (MainTabIndex == 0)
            {
                CommonGeneratePolls(webBrowser6, pollModelList6);
            }
            else if (MainTabIndex == 1)
            {
                CommonGeneratePolls(webBrowser7, pollModelList7);
            }
            else if (MainTabIndex == 2)
            {
                CommonGeneratePolls(webBrowser8, pollModelList8);
            }
            else if (MainTabIndex == 3)
            {
                CommonGeneratePolls(webBrowser9, pollModelList9);
            }
            else if (MainTabIndex == 4)
            {
                CommonGeneratePolls(webBrowser10, pollModelList10);
            }
        }


        private void geteratePolls(int? _MainTabIndex = null)
        {
            int MainTabIndex = materialTabControl2.SelectedIndex;
            if (_MainTabIndex != null)
            {
                MainTabIndex = (int)_MainTabIndex;
            }
            if (MainTabIndex == 0)
            {
                CommonGeneratePolls(webBrowser1, pollModelList1);
            }
            if (MainTabIndex == 1)
            {
                CommonGeneratePolls(webBrowser2, pollModelList2);
            }
            if (MainTabIndex == 2)
            {
                CommonGeneratePolls(webBrowser3, pollModelList3);
            }
            if (MainTabIndex == 3)
            {
                CommonGeneratePolls(webBrowser4, pollModelList4);
            }
            if (MainTabIndex == 4)
            {
                CommonGeneratePolls(webBrowser5, pollModelList5);
            }
        }


        private void materialButton19_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void importNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteNumber pasteNumber = new PasteNumber(this);
            pasteNumber.ShowDialog();

        }


        private void materialButton20_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void materialButton21_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void materialButton22_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void materialButton23_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }


        public void AddCaptionFReturn(string text, bool isAttachwithMainMessage)
        {
            int GlobalTabINdex = tabMain.SelectedIndex;

            if (GlobalTabINdex == 0)
            {
                int MainTabIndex = materialTabControl2.SelectedIndex;
                if (MainTabIndex == 0)
                {
                    foreach (DataGridViewRow item in dataGridView1.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;

                    }
                }
                if (MainTabIndex == 1)
                {
                    foreach (DataGridViewRow item in dataGridView2.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
                if (MainTabIndex == 2)
                {
                    foreach (DataGridViewRow item in dataGridView3.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
                if (MainTabIndex == 3)
                {
                    foreach (DataGridViewRow item in dataGridView4.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
                if (MainTabIndex == 4)
                {
                    foreach (DataGridViewRow item in dataGridView5.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
            }
            else if (GlobalTabINdex == 1)
            {
                int MainTabIndex = materialTabControl1.SelectedIndex;
                if (MainTabIndex == 0)
                {
                    foreach (DataGridViewRow item in dataGridView6.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
                if (MainTabIndex == 1)
                {
                    foreach (DataGridViewRow item in dataGridView7.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
                if (MainTabIndex == 2)
                {
                    foreach (DataGridViewRow item in dataGridView8.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
                if (MainTabIndex == 3)
                {
                    foreach (DataGridViewRow item in dataGridView9.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
                if (MainTabIndex == 4)
                {
                    foreach (DataGridViewRow item in dataGridView10.SelectedRows)
                    {
                        item.Cells[1].Value = text;
                        item.Cells[2].Value = isAttachwithMainMessage;
                    }
                }
            }

        }


        private void addCaptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ss = GetSelectedCaptionModel();

            AddCaption addCaption = new AddCaption(this, GetSelectedCaptionModel());
            addCaption.ShowDialog();
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

        }


        private void LoadContaxtforFIles(DataGridView _dataGridView, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (_dataGridView.SelectedRows.Count > 0)
                {
                    if (_dataGridView.SelectedRows[0].Cells[0].Value != null && _dataGridView.SelectedRows[0].Cells[0].Value.ToString() != "")
                    {
                        contextMenuStrip3.Show(_dataGridView, new Point(e.Location.X, e.Location.Y));
                    }
                }

            }
        }
        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView1, e);
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView2, e);
        }

        private void dataGridView3_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView3, e);
        }

        private void dataGridView4_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView4, e);
        }

        private void dataGridView5_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView5, e);
        }

        private void dataGridView6_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView6, e);
        }

        private void dataGridView7_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView7, e);
        }

        private void dataGridView8_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView8, e);
        }

        private void dataGridView9_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView9, e);
        }

        private void dataGridView10_MouseClick(object sender, MouseEventArgs e)
        {
            LoadContaxtforFIles(dataGridView10, e);
        }


        private void WaSenderForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void fillSingleDetails(WASenderSingleTransModel _tmpwASenderSingleTransModel)
        {
            #region GridColumns
            var globalCounter = gridTargets.Rows.Count - 1;
            var ColumnsCOunt = _tmpwASenderSingleTransModel.contactList[0].parameterModelList.Count();
            if (ColumnsCOunt > 2)
            {
                for (int i = 1; i <= ColumnsCOunt; i++)
                {
                    try
                    {
                        string Header = _tmpwASenderSingleTransModel.contactList[0].parameterModelList[i].ParameterName;
                        gridTargets.Columns.Add("NewColumn" + i, Header);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            #endregion

            #region Grid
            foreach (var item in _tmpwASenderSingleTransModel.contactList)
            {

                try
                {
                    gridTargets.Rows.Add();
                    string MobileNumber = item.number;
                    try
                    {
                        MobileNumber = MobileNumber.Replace("+", "");
                        MobileNumber = MobileNumber.Replace(" ", "");
                        MobileNumber = MobileNumber.Replace(" ", "");
                        Int64 temp = Convert.ToInt64(MobileNumber);
                    }
                    catch (Exception ex)
                    {
                    }


                    string name = "";
                    try
                    {
                        name = item.name;
                    }
                    catch (Exception ex)
                    {

                    }

                    gridTargets.Rows[globalCounter].Cells[0].Value = MobileNumber;
                    gridTargets.Rows[globalCounter].Cells[1].Value = name;

                    try
                    {
                        if (ColumnsCOunt > 1)
                        {
                            for (int j = 1; j <= ColumnsCOunt; j++)
                            {
                                try
                                {
                                    string CelValue = item.parameterModelList[j].ParameterValue;
                                    gridTargets.Rows[globalCounter].Cells[j + 1].Value = CelValue;
                                }
                                catch (Exception rrrrex)
                                {

                                }
                            }
                        }
                    }
                    catch (Exception eex)
                    {

                    }
                    globalCounter++;

                }
                catch (Exception vex)
                {

                }

            }
            #endregion

            #region Messages

            if (_tmpwASenderSingleTransModel.messages[0] != null)
            {
                if (_tmpwASenderSingleTransModel.messages[0].longMessage != null && _tmpwASenderSingleTransModel.messages[0].longMessage != "")
                {
                    txtMsgOne.Text = _tmpwASenderSingleTransModel.messages[0].longMessage;
                }
            }

            if (_tmpwASenderSingleTransModel.messages[1] != null)
            {
                if (_tmpwASenderSingleTransModel.messages[1].longMessage != null && _tmpwASenderSingleTransModel.messages[1].longMessage != "")
                {
                    txtMsgTwo.Text = _tmpwASenderSingleTransModel.messages[1].longMessage;
                }
            }
            if (_tmpwASenderSingleTransModel.messages[2] != null)
            {
                if (_tmpwASenderSingleTransModel.messages[2].longMessage != null && _tmpwASenderSingleTransModel.messages[2].longMessage != "")
                {
                    txtMsgThree.Text = _tmpwASenderSingleTransModel.messages[2].longMessage;
                }
            }
            if (_tmpwASenderSingleTransModel.messages[3] != null)
            {
                if (_tmpwASenderSingleTransModel.messages[3].longMessage != null && _tmpwASenderSingleTransModel.messages[3].longMessage != "")
                {
                    txtMsgFour.Text = _tmpwASenderSingleTransModel.messages[3].longMessage;
                }
            }
            if (_tmpwASenderSingleTransModel.messages[4] != null)
            {
                if (_tmpwASenderSingleTransModel.messages[4].longMessage != null && _tmpwASenderSingleTransModel.messages[4].longMessage != "")
                {
                    txtMsgFive.Text = _tmpwASenderSingleTransModel.messages[4].longMessage;
                }
            }

            #endregion

            #region files


            if (_tmpwASenderSingleTransModel.messages[0] != null && _tmpwASenderSingleTransModel.messages[0].files != null && _tmpwASenderSingleTransModel.messages[0].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderSingleTransModel.messages[0].files)
                {
                    dataGridView1.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }

            if (_tmpwASenderSingleTransModel.messages[1] != null && _tmpwASenderSingleTransModel.messages[1].files != null && _tmpwASenderSingleTransModel.messages[1].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderSingleTransModel.messages[1].files)
                {
                    dataGridView2.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderSingleTransModel.messages[2] != null && _tmpwASenderSingleTransModel.messages[2].files != null && _tmpwASenderSingleTransModel.messages[2].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderSingleTransModel.messages[2].files)
                {
                    dataGridView3.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderSingleTransModel.messages[3] != null && _tmpwASenderSingleTransModel.messages[3].files != null && _tmpwASenderSingleTransModel.messages[3].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderSingleTransModel.messages[3].files)
                {
                    dataGridView4.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderSingleTransModel.messages[4] != null && _tmpwASenderSingleTransModel.messages[4].files != null && _tmpwASenderSingleTransModel.messages[4].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderSingleTransModel.messages[4].files)
                {
                    dataGridView5.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            #endregion

            #region polls

            try
            {
                if (_tmpwASenderSingleTransModel.messages[0] != null)
                {

                    if (_tmpwASenderSingleTransModel.messages[0].polls != null && _tmpwASenderSingleTransModel.messages[0].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[0].polls)
                        {
                            try
                            {
                                RecievPoll(item, 0);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderSingleTransModel.messages[1] != null)
                {


                    if (_tmpwASenderSingleTransModel.messages[1].polls != null && _tmpwASenderSingleTransModel.messages[1].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[1].polls)
                        {
                            try
                            {
                                RecievPoll(item, 1);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderSingleTransModel.messages[2] != null)
                {
                    if (_tmpwASenderSingleTransModel.messages[2].polls != null && _tmpwASenderSingleTransModel.messages[2].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[2].polls)
                        {
                            try
                            {
                                RecievPoll(item, 2);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                if (_tmpwASenderSingleTransModel.messages[3] != null)
                {

                    if (_tmpwASenderSingleTransModel.messages[3].polls != null && _tmpwASenderSingleTransModel.messages[3].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[3].polls)
                        {
                            try
                            {
                                RecievPoll(item, 3);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                if (_tmpwASenderSingleTransModel.messages[4] != null)
                {

                    if (_tmpwASenderSingleTransModel.messages[4].polls != null && _tmpwASenderSingleTransModel.messages[4].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[4].polls)
                        {
                            try
                            {
                                RecievPoll(item, 4);
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

            #endregion

            #region Buttons

            if (Strings.EnableButtons)
            {
                if (_tmpwASenderSingleTransModel.messages[0] != null)
                {
                    if (_tmpwASenderSingleTransModel.messages[0].buttons != null && _tmpwASenderSingleTransModel.messages[0].buttons.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[0].buttons)
                        {
                            try
                            {
                                RecievButton(item, 0);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderSingleTransModel.messages[1] != null)
                {
                    if (_tmpwASenderSingleTransModel.messages[1].buttons != null && _tmpwASenderSingleTransModel.messages[1].buttons.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[1].buttons)
                        {
                            try
                            {
                                RecievButton(item, 1);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderSingleTransModel.messages[2] != null)
                {
                    if (_tmpwASenderSingleTransModel.messages[2].buttons != null && _tmpwASenderSingleTransModel.messages[2].buttons.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[2].buttons)
                        {
                            try
                            {
                                RecievButton(item, 2);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderSingleTransModel.messages[3] != null)
                {
                    if (_tmpwASenderSingleTransModel.messages[3].buttons != null && _tmpwASenderSingleTransModel.messages[3].buttons.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[3].buttons)
                        {
                            try
                            {
                                RecievButton(item, 3);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderSingleTransModel.messages[4] != null)
                {
                    if (_tmpwASenderSingleTransModel.messages[4].buttons != null && _tmpwASenderSingleTransModel.messages[4].buttons.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderSingleTransModel.messages[4].buttons)
                        {
                            try
                            {
                                RecievButton(item, 4);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }

            #endregion

            #region settingss
            txtdelayAfterMessagesFrom.Text = _tmpwASenderSingleTransModel.settings.delayAfterMessagesFrom.ToString();
            txtdelayAfterMessagesTo.Text = _tmpwASenderSingleTransModel.settings.delayAfterMessagesTo.ToString();
            txtdelayAfterMessages.Text = _tmpwASenderSingleTransModel.settings.delayAfterMessages.ToString();
            txtdelayAfterEveryMessageFrom.Text = _tmpwASenderSingleTransModel.settings.delayAfterEveryMessageFrom.ToString();
            txtdelayAfterEveryMessageTo.Text = _tmpwASenderSingleTransModel.settings.delayAfterEveryMessageTo.ToString();
            #endregion

            #region firndlySettings
            this.tmpsendTofriendlyNumbersAfterMessages = _tmpwASenderSingleTransModel.sendTofriendlyNumbersAfterMessages;
            this.tmpfriendlyNumbers = _tmpwASenderSingleTransModel.friendlyNumbers;

            #endregion
        }



        private void fillGroupDetails(WASenderGroupTransModel _tmpwASenderGroupTransModel)
        {
            #region Grid

            var globalCounter = gridTargetsGroup.Rows.Count - 1;
            for (var i = 0; i < _tmpwASenderGroupTransModel.groupList.Count(); i++)
            {
                try
                {
                    gridTargetsGroup.Rows.Add();
                    gridTargetsGroup.Rows[globalCounter].Cells[0].Value = _tmpwASenderGroupTransModel.groupList[i].Name;
                    gridTargetsGroup.Rows[globalCounter].Cells[1].Value = _tmpwASenderGroupTransModel.groupList[i].GroupId;
                    globalCounter++;
                }
                catch (Exception ex)
                {

                }
            }

            #endregion

            #region Messages
            if (_tmpwASenderGroupTransModel.messages[0] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[0].longMessage != null && _tmpwASenderGroupTransModel.messages[0].longMessage != "")
                {
                    txtMsgOneGroup.Text = _tmpwASenderGroupTransModel.messages[0].longMessage;
                }
            }
            if (_tmpwASenderGroupTransModel.messages[1] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[1].longMessage != null && _tmpwASenderGroupTransModel.messages[1].longMessage != "")
                {
                    txtMsgTwoGroup.Text = _tmpwASenderGroupTransModel.messages[1].longMessage;
                }
            }
            if (_tmpwASenderGroupTransModel.messages[2] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[2].longMessage != null && _tmpwASenderGroupTransModel.messages[2].longMessage != "")
                {
                    txtMsgTHreeGroup.Text = _tmpwASenderGroupTransModel.messages[2].longMessage;
                }
            }
            if (_tmpwASenderGroupTransModel.messages[3] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[3].longMessage != null && _tmpwASenderGroupTransModel.messages[3].longMessage != "")
                {
                    txtMsgFourGroup.Text = _tmpwASenderGroupTransModel.messages[3].longMessage;
                }
            }
            if (_tmpwASenderGroupTransModel.messages[4] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[4].longMessage != null && _tmpwASenderGroupTransModel.messages[4].longMessage != "")
                {
                    txtMsgFiveGroup.Text = _tmpwASenderGroupTransModel.messages[4].longMessage;
                }
            }
            #endregion

            #region files
            if (_tmpwASenderGroupTransModel.messages[0] != null && _tmpwASenderGroupTransModel.messages[0].files != null && _tmpwASenderGroupTransModel.messages[0].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderGroupTransModel.messages[0].files)
                {
                    dataGridView6.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }

            if (_tmpwASenderGroupTransModel.messages[1] != null && _tmpwASenderGroupTransModel.messages[1].files != null && _tmpwASenderGroupTransModel.messages[1].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderGroupTransModel.messages[1].files)
                {
                    dataGridView7.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderGroupTransModel.messages[2] != null && _tmpwASenderGroupTransModel.messages[2].files != null && _tmpwASenderGroupTransModel.messages[2].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderGroupTransModel.messages[2].files)
                {
                    dataGridView8.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderGroupTransModel.messages[3] != null && _tmpwASenderGroupTransModel.messages[3].files != null && _tmpwASenderGroupTransModel.messages[3].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderGroupTransModel.messages[3].files)
                {
                    dataGridView9.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderGroupTransModel.messages[4] != null && _tmpwASenderGroupTransModel.messages[4].files != null && _tmpwASenderGroupTransModel.messages[4].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderGroupTransModel.messages[4].files)
                {
                    dataGridView10.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            #endregion

            #region polls

            try
            {
                if (_tmpwASenderGroupTransModel.messages[0] != null)
                {

                    if (_tmpwASenderGroupTransModel.messages[0].polls != null && _tmpwASenderGroupTransModel.messages[0].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderGroupTransModel.messages[0].polls)
                        {
                            try
                            {
                                RecievPoll(item, 0);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderGroupTransModel.messages[1] != null)
                {


                    if (_tmpwASenderGroupTransModel.messages[1].polls != null && _tmpwASenderGroupTransModel.messages[1].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderGroupTransModel.messages[1].polls)
                        {
                            try
                            {
                                RecievPoll(item, 1);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                if (_tmpwASenderGroupTransModel.messages[2] != null)
                {
                    if (_tmpwASenderGroupTransModel.messages[2].polls != null && _tmpwASenderGroupTransModel.messages[2].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderGroupTransModel.messages[2].polls)
                        {
                            try
                            {
                                RecievPoll(item, 2);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                if (_tmpwASenderGroupTransModel.messages[3] != null)
                {

                    if (_tmpwASenderGroupTransModel.messages[3].polls != null && _tmpwASenderGroupTransModel.messages[3].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderGroupTransModel.messages[3].polls)
                        {
                            try
                            {
                                RecievPoll(item, 3);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
                if (_tmpwASenderGroupTransModel.messages[4] != null)
                {

                    if (_tmpwASenderGroupTransModel.messages[4].polls != null && _tmpwASenderGroupTransModel.messages[4].polls.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderGroupTransModel.messages[4].polls)
                        {
                            try
                            {
                                RecievPoll(item, 4);
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

            #endregion

            #region Buttons

            if (Strings.EnableButtons)
            {
                if (_tmpwASenderGroupTransModel.messages[0] != null)
                {
                    if (_tmpwASenderGroupTransModel.messages[0].buttons != null && _tmpwASenderGroupTransModel.messages[0].buttons.Count() > 0)
                    {
                        foreach (var item in _tmpwASenderGroupTransModel.messages[0].buttons)
                        {
                            try
                            {
                                RecievButton(item, 0);
                            }
                foreach (var fl in _tmpwASenderGroupTransModel.messages[2].files)
                {
                    dataGridView8.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderGroupTransModel.messages[3] != null && _tmpwASenderGroupTransModel.messages[3].files != null && _tmpwASenderGroupTransModel.messages[3].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderGroupTransModel.messages[3].files)
                {
                    dataGridView9.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            if (_tmpwASenderGroupTransModel.messages[4] != null && _tmpwASenderGroupTransModel.messages[4].files != null && _tmpwASenderGroupTransModel.messages[4].files.Count() > 0)
            {
                foreach (var fl in _tmpwASenderGroupTransModel.messages[4].files)
                {
                    dataGridView10.Rows.Add(fl.filePath, fl.Caption == null ? "" : fl.Caption, fl.attachWithMainMessage);
                }
            }
            #endregion

            #region settingss
            txtdelayAfterMessagesFromGroup.Text = _tmpwASenderGroupTransModel.settings.delayAfterMessagesFrom.ToString();
            txtdelayAfterMessagesToGroup.Text = _tmpwASenderGroupTransModel.settings.delayAfterMessagesTo.ToString();
            txtdelayAfterMessagesGroup.Text = _tmpwASenderGroupTransModel.settings.delayAfterMessages.ToString();
            txtdelayAfterEveryMessageFromGroup.Text = _tmpwASenderGroupTransModel.settings.delayAfterEveryMessageFrom.ToString();
            txtdelayAfterEveryMessageToGroup.Text = _tmpwASenderGroupTransModel.settings.delayAfterEveryMessageTo.ToString();
            #endregion



            if (_tmpwASenderGroupTransModel.messages[0] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[0].polls != null && _tmpwASenderGroupTransModel.messages[0].polls.Count() > 0)
                {
                    foreach (var item in _tmpwASenderGroupTransModel.messages[0].polls)
                    {
                        try
                        {
                            RecievPoll(item, 0);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }

            if (_tmpwASenderGroupTransModel.messages[1] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[1].polls != null && _tmpwASenderGroupTransModel.messages[1].polls.Count() > 0)
                {
                    foreach (var item in _tmpwASenderGroupTransModel.messages[1].polls)
                    {
                        try
                        {
                            RecievPoll(item, 1);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            if (_tmpwASenderGroupTransModel.messages[2] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[2].polls != null && _tmpwASenderGroupTransModel.messages[2].polls.Count() > 0)
                {
                    foreach (var item in _tmpwASenderGroupTransModel.messages[2].polls)
                    {
                        try
                        {
                            RecievPoll(item, 2);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            if (_tmpwASenderGroupTransModel.messages[3] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[2].polls != null && _tmpwASenderGroupTransModel.messages[3].polls.Count() > 0)
                {
                    foreach (var item in _tmpwASenderGroupTransModel.messages[3].polls)
                    {
                        try
                        {
                            RecievPoll(item, 3);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            if (_tmpwASenderGroupTransModel.messages[4] != null)
            {
                if (_tmpwASenderGroupTransModel.messages[4].polls != null && _tmpwASenderGroupTransModel.messages[4].polls.Count() > 0)
                {
                    foreach (var item in _tmpwASenderGroupTransModel.messages[4].polls)
                    {
                        try
                        {
                            RecievPoll(item, 4);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }
        private void OpenCampaign()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Strings.SelectExcel;
            openFileDialog.DefaultExt = "json";
            openFileDialog.Filter = "JSON Files|*.json;";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                string fileData = File.ReadAllText(file);
                try
                {
                    int MainTabIndex = tabMain.SelectedIndex;


                    if (MainTabIndex == 0)
                    {
                        clearAll();

                        #region SingleSender
                        try
                        {
                            WASenderSingleTransModel _tmpwASenderSingleTransModel = JsonConvert.DeserializeObject<WASenderSingleTransModel>(fileData);

                            fillSingleDetails(_tmpwASenderSingleTransModel);

                        }
                        catch (Exception ex)
                        {

                        }
                        #endregion
                    }
                    else if (MainTabIndex == 1)
                    {

                        clearAllGroup();
                        WASenderGroupTransModel _tmpwASenderGroupTransModel = JsonConvert.DeserializeObject<WASenderGroupTransModel>(fileData);
                        fillGroupDetails(_tmpwASenderGroupTransModel);

                    }


                }
                catch (Exception ex)
                {

                }

            }
        }
        private void saveCampaign()
        {
            int MainTabIndex = tabMain.SelectedIndex;
            String tmpFolderPath = Config.GetTempFolderPath();
            string FtmpName = "Campaign_" + Guid.NewGuid();
            string Json = "";
            if (MainTabIndex == 0)
            {
                #region SingleSender
                ValidateControls(true);
                Json = JsonConvert.SerializeObject(wASenderSingleTransModel, Formatting.Indented);
                File.WriteAllText(tmpFolderPath + "\\" + FtmpName, Json);
                savesampleExceldialog.FileName = "SingleSender.json";
                savesampleExceldialog.Filter = "JSON Files (*.json)|*.json";

                #endregion
            }
            else if (MainTabIndex == 1)
            {
                #region Group
                ValidateControlsGroup(true);
                Json = JsonConvert.SerializeObject(wASenderGroupTransModel, Formatting.Indented);
                File.WriteAllText(tmpFolderPath + "\\" + FtmpName, Json);
                savesampleExceldialog.FileName = "GroupSender.json";
                savesampleExceldialog.Filter = "JSON Files (*.json)|*.json";
                #endregion
            }
            else
            {
                return;
            }
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(tmpFolderPath + "\\" + FtmpName, savesampleExceldialog.FileName.EndsWith(".json") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".json", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }


        }

        private void WaSenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.ToString() == "S, Control")
            {
                saveCampaign();
            }
            else if (e.KeyData.ToString() == "O, Control")
            {
                OpenCampaign();
            }

        }

        private void materialButton26_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void materialButton27_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void materialButton28_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void materialButton29_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void materialButton30_Click(object sender, EventArgs e)
        {
            ShowAddPollDialog();
        }

        private void WaSenderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Utils.Driver != null)
            {
                try
                {
                    Utils.Driver.Quit();
                }
                catch (Exception ex)
                {
                }
            }
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {

                }
            }
        }
        public void checkBrowserType()
        {
            try
            {
                GeneralSettingsModel settings = Config.GetSettings();
                if (settings.browserType == 1)
                {
                    materialButton34.Hide();
                    materialButton35.Hide();
                }
                else
                {
                    materialButton34.Show();
                    materialButton35.Show();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void materialButton31_Click_1(object sender, EventArgs e)
        {
            GeneralSettings generalSettings = new GeneralSettings(this);
            generalSettings.ShowDialog();
        }

        private void btnAIMessage_Click(object sender, EventArgs e)
        {
            AIMessageGenerator aiGenerator = new AIMessageGenerator();
            if (aiGenerator.ShowDialog() == DialogResult.OK)
            {
                string generatedMessage = aiGenerator.GeneratedMessage;
                if (!string.IsNullOrEmpty(generatedMessage))
                {
                    txtLongMessage.Text = generatedMessage;
                }
            }
        }

        private void InitializeAIButton()
        {
            btnAIMessage = new MaterialSkin.Controls.MaterialButton();
            btnAIMessage.AutoSize = false;
            btnAIMessage.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            btnAIMessage.Depth = 0;
            btnAIMessage.DrawShadows = true;
            btnAIMessage.HighEmphasis = true;
            btnAIMessage.Icon = global::WASender.Properties.Resources.robot;
            btnAIMessage.Location = new System.Drawing.Point(650, 120);
            btnAIMessage.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            btnAIMessage.MouseState = MaterialSkin.MouseState.HOVER;
            btnAIMessage.Name = "btnAIMessage";
            btnAIMessage.Size = new System.Drawing.Size(180, 36);
            btnAIMessage.TabIndex = 15;
            btnAIMessage.Text = "AI Message";
            btnAIMessage.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            btnAIMessage.UseAccentColor = true;
            btnAIMessage.UseVisualStyleBackColor = true;
            btnAIMessage.Click += new System.EventHandler(this.btnAIMessage_Click);
            
            // Add to the form
            this.Controls.Add(btnAIMessage);
        }

        private void InitializeAIMenu()
        {
            // Create AI menu item
            ToolStripMenuItem aiMenuItem = new ToolStripMenuItem();
            aiMenuItem.Text = "AI Tools";
            
            // Create submenu items
            ToolStripMenuItem aiSettingsItem = new ToolStripMenuItem();
            aiSettingsItem.Text = "AI Settings";
            aiSettingsItem.Click += new EventHandler(aiSettings_Click);
            
            ToolStripMenuItem aiGenerateMessageItem = new ToolStripMenuItem();
            aiGenerateMessageItem.Text = "Generate Message";
            aiGenerateMessageItem.Click += new EventHandler(aiGenerateMessage_Click);
            
            // Add submenu items to AI menu
            aiMenuItem.DropDownItems.Add(aiSettingsItem);
            aiMenuItem.DropDownItems.Add(aiGenerateMessageItem);
            
            // Add AI menu to main menu
            menuStrip1.Items.Add(aiMenuItem);
        }

        private void aiSettings_Click(object sender, EventArgs e)
        {
            AISettingsForm settingsForm = new AISettingsForm();
            settingsForm.ShowDialog();
        }

        private void aiGenerateMessage_Click(object sender, EventArgs e)
        {
            AIMessageGenerator aiGenerator = new AIMessageGenerator();
            if (aiGenerator.ShowDialog() == DialogResult.OK)
            {
                string generatedMessage = aiGenerator.GeneratedMessage;
                if (!string.IsNullOrEmpty(generatedMessage))
                {
                    txtLongMessage.Text = generatedMessage;
                }
            }
        }

        private CaptionModel GetCurrent(DataGridView dataGridView1)
        {
            CaptionModel captionModel = new CaptionModel();

            foreach (DataGridViewRow item in dataGridView1.SelectedRows)
            {
                try
                {
                    captionModel.Caption = item.Cells[1].Value.ToString();
                }
                catch (Exception ex)
                {

                }
                if (item.Cells[2].Value == null)
                {
                    captionModel.IsAttachwithMainMessage = false;
                }
                else
                {
                    string s = item.Cells[2].Value.ToString();
                    if (item.Cells[2].Value.ToString() == "True")
                    {
                        captionModel.IsAttachwithMainMessage = true;
                    }
                }
            }

            return captionModel;

        }
        private CaptionModel GetSelectedCaptionModel()
        {
            CaptionModel captionModel = new CaptionModel();
            int GlobalTabINdex = tabMain.SelectedIndex;

            if (GlobalTabINdex == 0)
            {
                int MainTabIndex = materialTabControl2.SelectedIndex;
                if (MainTabIndex == 0)
                {
                    captionModel = GetCurrent(dataGridView1);
                }
                if (MainTabIndex == 1)
                {
                    captionModel = GetCurrent(dataGridView2);
                }
                if (MainTabIndex == 2)
                {
                    captionModel = GetCurrent(dataGridView3);
                }
                if (MainTabIndex == 3)
                {
                    captionModel = GetCurrent(dataGridView4);
                }
                if (MainTabIndex == 4)
                {
                    captionModel = GetCurrent(dataGridView5);
                }
            }
            else if (GlobalTabINdex == 1)
            {
                int MainTabIndex = materialTabControl1.SelectedIndex;
                if (MainTabIndex == 0)
                {
                    captionModel = GetCurrent(dataGridView6);
                }
                if (MainTabIndex == 1)
                {
                    captionModel = GetCurrent(dataGridView7);
                }
                if (MainTabIndex == 2)
                {
                    captionModel = GetCurrent(dataGridView8);
                }
                if (MainTabIndex == 3)
                {
                    captionModel = GetCurrent(dataGridView9);
                }
                if (MainTabIndex == 4)
                {
                    captionModel = GetCurrent(dataGridView10);
                }
            }
            return captionModel;
        }


        private void materialButton34_Click(object sender, EventArgs e)
        {
            ManageAccounts form = new ManageAccounts();
            form.ShowDialog();
        }

        private void materialButton35_Click(object sender, EventArgs e)
        {
            if (Utils.waSenderBrowser != null)
            {

            }
            else
            {
                var browser = new WaSenderBrowser();
                Utils.waSenderBrowser = browser;
                browser.Show();
            }
        }


        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> AllNumbers = new List<string>();
            foreach (DataGridViewRow row in gridTargets.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    AllNumbers.Add(row.Cells[0].Value.ToString());
                }

            }
            List<string> newList = AllNumbers.Distinct().ToList();
            gridTargets.Rows.Clear();

            foreach (string item in newList)
            {
                gridTargets.Rows.Add(new object[]{
                    item,
                    "",
                });
            }

        }

        public void OpenGeneralSettings()
        {
            try
            {
                GeneralSettings form = new GeneralSettings();
                form.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }


        public void checkForPendingSchedules(bool forceRetart = false)
        {
            var schedules = WASender.PCUtils.checkSchedule();
            var pendings = schedules.Where(x => x.scheduleDatetime >= DateTime.Now && x.status == "PENDING").ToList();
            if (pendings.Count() > 0)
            {
                scheduleAdded(forceRetart);
            }

        }

        public void scheduleDeleted()
        {
            scheduleAdded(true);
        }


        private void runScheduleChecker()
        {
            string Actualpath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string path = Actualpath.Replace("\\", "~");

            string param = Utils.currentColorscheme.ToString();
            List<string> cParams = new List<string>();
            cParams.Add("\"" + param + "\"");
            cParams.Add("\"" + Strings.ScheduleChecker + "\"");
            cParams.Add("\"" + Strings.Running + "\"");
            cParams.Add("\"" + Strings.TryingtoruntheScheduleBut + "\"");
            cParams.Add("\"" + Strings.alreadyrunning + "\"");
            cParams.Add("\"" + Strings.IwillstayhereutillyourallSchedulesarecompleted + "\"");
            cParams.Add("\"" + Strings.Exit + "\"");
            cParams.Add("\"" + Strings.Days + "\"");
            cParams.Add("\"" + Strings.Hours + "\"");
            cParams.Add("\"" + Strings.Minutes + "\"");
            cParams.Add("\"" + Strings.NextSchedulein + "\"");
            cParams.Add("\"" + path + "\"");
            string sParams = JsonConvert.SerializeObject(cParams);

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo(Actualpath + "\\ScheduleChecker.exe", sParams)
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = Actualpath,
                    //UseShellExecute =true,
                    //Verb = "runas",
                }
            };

            process.Start();
        }

        public void scheduleAdded(bool forceRetart = false)
        {
            Process[] processes = Process.GetProcessesByName("ScheduleChecker");
            if (forceRetart == true)
            {
                if (processes.Count() > 0)
                {
                    processes[0].Kill();
                }

                runScheduleChecker();
            }

            else
            {
                if (processes.Count() == 0)
                {
                    runScheduleChecker();
                }
            }
        }

        public void EditSchedule(SchedulesModel _schedulesModel)
        {
            this.schedulesModel = _schedulesModel;

            if (schedulesModel.Type == "SINGLE")
            {
                wASenderSingleTransModel = JsonConvert.DeserializeObject<WASenderSingleTransModel>(schedulesModel.JsonString);
                gridTargets.Rows.Clear();
                tabMain.SelectedIndex = 0;
                fillSingleDetails(wASenderSingleTransModel);
            }
            if (schedulesModel.Type == "GROUP")
            {
                wASenderGroupTransModel = JsonConvert.DeserializeObject<WASenderGroupTransModel>(schedulesModel.JsonString);
                gridTargetsGroup.Rows.Clear();
                tabMain.SelectedIndex = 1;
                fillGroupDetails(wASenderGroupTransModel);
            }
        }

        private void builtInVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = tabMain.SelectedIndex;
            BuiltInVariable form = new BuiltInVariable(this, tabMain.SelectedIndex == 1 ? true : false);
            form.ShowDialog();
        }

        private void deleteAllRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridTargets.Rows.Clear();
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            ButtonHolder holder = new ButtonHolder(this, new ButtonHolderModel());
            holder.ShowDialog();
        }

        public void RecievButton(ButtonHolderModel buttonsModel, int? _MainTabIndex = null)
        {
            int topTabIndex = tabMain.SelectedIndex;
            if (topTabIndex == 0)
            {
                int MainTabIndex = materialTabControl2.SelectedIndex;
                if (_MainTabIndex != null)
                {
                    MainTabIndex = (int)_MainTabIndex;
                }

                if (MainTabIndex == 0)
                {
                    commonAddButtonList(buttonsModel, buttonsModelList1);
                }
                if (MainTabIndex == 1)
                {
                    commonAddButtonList(buttonsModel, buttonsModelList2);
                }
                if (MainTabIndex == 2)
                {
                    commonAddButtonList(buttonsModel, buttonsModelList3);
                }
                if (MainTabIndex == 3)
                {
                    commonAddButtonList(buttonsModel, buttonsModelList4);
                }
                if (MainTabIndex == 4)
                {
                    commonAddButtonList(buttonsModel, buttonsModelList5);
                }
                geterateButtons(_MainTabIndex);
            }
        }


        private void geterateButtons(int? _MainTabIndex = null)
        {
            int MainTabIndex = materialTabControl2.SelectedIndex;
            if (_MainTabIndex != null)
            {
                MainTabIndex = (int)_MainTabIndex;
            }

            if (MainTabIndex == 0)
            {
                CommonGenerateButtons(_btnBrowser1, buttonsModelList1, materialButton3);
            }
            else if (MainTabIndex == 1)
            {
                CommonGenerateButtons(_btnBrowser2, buttonsModelList2, materialButton9);
            }
            else if (MainTabIndex == 2)
            {
                CommonGenerateButtons(_btnBrowser3, buttonsModelList3, materialButton10);
            }
            else if (MainTabIndex == 3)
            {
                CommonGenerateButtons(_btnBrowser4, buttonsModelList4, materialButton11);
            }
            else if (MainTabIndex == 4)
            {
                CommonGenerateButtons(_btnBrowser5, buttonsModelList5, materialButton12);
            }
        }

        private static void CommonGenerateButtons(WebBrowser webBrowser, List<ButtonHolderModel> buttonsModelList, MaterialButton materialButton)
        {
            string buttontext = Storage.DocumentHtmlString;
            string cssStyle = Storage.DocumentButtonStypeStrig;

            if (buttonsModelList.Count() >= 1)
            {
                materialButton.Enabled = false;
            }
            else
            {
                materialButton.Enabled = true;
            }

            foreach (var item in buttonsModelList)
            {
                string txt = "";
                txt = item.title;
                buttontext += "<button style='margin:5px;" + cssStyle + "' type='button' id='" + item.Id + "' >" + txt + "</button>";
            }
            webBrowser.DocumentText = buttontext + "</body></html>";
        }
        public void RemoveButton(ButtonHolderModel buttonsModel)
        {
            int TabIndex = tabMain.SelectedIndex;
            if (TabIndex == 0)
            {
                int MainTabIndex = materialTabControl2.SelectedIndex;
                if (MainTabIndex == 0)
                {
                    commonRemoveButtonList(buttonsModel, buttonsModelList1);
                }
                if (MainTabIndex == 1)
                {
                    commonRemoveButtonList(buttonsModel, buttonsModelList2);
                }
                if (MainTabIndex == 2)
                {
                    commonRemoveButtonList(buttonsModel, buttonsModelList3);
                }
                if (MainTabIndex == 3)
                {
                    commonRemoveButtonList(buttonsModel, buttonsModelList4);
                }
                if (MainTabIndex == 4)
                {
                    commonRemoveButtonList(buttonsModel, buttonsModelList5);
                }
                geterateButtons(MainTabIndex);
            }
        }

        private void commonRemoveButtonList(ButtonHolderModel _buttonsModel, List<ButtonHolderModel> _buttonsModelList)
        {
            int index = _buttonsModelList.FindIndex(x => x.Id == _buttonsModel.Id);
            _buttonsModelList.Remove(_buttonsModelList[index]);



        }
        private void commonAddButtonList(ButtonHolderModel _buttonsModel, List<ButtonHolderModel> _buttonsModelList)
        {
            if (_buttonsModel.editMode == true)
            {
                int index = _buttonsModelList.FindIndex(x => x.Id == _buttonsModel.Id);
                _buttonsModel.editMode = false;
                _buttonsModelList[index] = _buttonsModel;
            }
            else
            {
                _buttonsModelList.Add(_buttonsModel);
            }
        }

        private void materialButton9_Click(object sender, EventArgs e)
        {
            ButtonHolder holder = new ButtonHolder(this, new ButtonHolderModel());
            holder.ShowDialog();
        }

        private void materialButton10_Click(object sender, EventArgs e)
        {
            ButtonHolder holder = new ButtonHolder(this, new ButtonHolderModel());
            holder.ShowDialog();
        }

        private void materialButton11_Click(object sender, EventArgs e)
        {
            ButtonHolder holder = new ButtonHolder(this, new ButtonHolderModel());
            holder.ShowDialog();
        }

        private void materialButton12_Click(object sender, EventArgs e)
        {
            ButtonHolder holder = new ButtonHolder(this, new ButtonHolderModel());
            holder.ShowDialog();
        }
    }
}
