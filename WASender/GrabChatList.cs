using MaterialSkin;
using MaterialSkin.Controls;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.enums;
using System.Threading;
using System.Diagnostics;
using System.IO;
using OfficeOpenXml;
using WASender.Models;
using Microsoft.Web.WebView2.WinForms;
using Models;

namespace WASender
{


    public partial class GrabChatList : MaterialForm
    {
        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        CampaignStatusEnum campaignStatusEnum;
        WaSenderForm waSenderForm;
        Logger logger;
        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;


        public GrabChatList(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            logger = new Logger("GrabChatList");
            waSenderForm = _waSenderForm;
            generalSettingsModel = Config.GetSettings();
            if (Utils.Driver != null)
            {
                if (generalSettingsModel.browserType == 1)
                {
                    Utils.SetDriver();
                    driver = Utils.Driver;
                    initWA();
                }
            }

            if (Utils.waSenderBrowser != null)
            {
                browser = Utils.waSenderBrowser;
                initWABrowser();
            }
            _testClass = Utils.testClass;
            _testClass.OnUpdateStatus += _testClass_OnUpdateStatus;
        }

        void _testClass_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            ChangeInitStatus(InitStatusEnum.Stopped);
            ChangeCampStatus(CampaignStatusEnum.Stopped);
        }

        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            logger.WriteLog("ChangeInitStatus = " + _initStatus.ToString());
            this.initStatusEnum = _initStatus;
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }


        private void init()
        {
            ChangeInitStatus(InitStatusEnum.NotInitialised);
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
        }

        private void checkQRScanDoneBrowser()
        {
            Thread.Sleep(1000);
            logger.WriteLog("checkQRScanDone");
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
        }

        private void checkQRScanDone()
        {
            logger.WriteLog("checkQRScanDone");
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
        }

        int retryAttempt = 0;

        public async void timerInitChecker_Tick(object sender, EventArgs e)
        {
            try
            {
                if (generalSettingsModel.browserType == 1)
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1XkO3"), driver);

                    if (isElementDisplayed == true)
                    {
                        logger.WriteLog("_1XkO3 ElementDisplayed");
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                    }
                }
                else if (generalSettingsModel.browserType == 2)
                {

                    try
                    {
                        WebView2 vw = Utils.GetActiveWebView(browser);

                        bool isInitiated = await WPPHelper.isWaInited(vw);
                        if (isInitiated)
                        {
                            ChangeInitStatus(InitStatusEnum.Initialised);
                            timerInitChecker.Stop();
                            Activate();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (retryAttempt == 5)
                        {
                            retryAttempt = 0;
                            timerInitChecker.Stop();
                        }
                        else
                        {
                            retryAttempt++;
                            Debug.WriteLine("Retry attempt ." + retryAttempt.ToString());
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex.Message);
                logger.WriteLog(ex.StackTrace);
                ChangeInitStatus(InitStatusEnum.Unable);
                timerInitChecker.Stop();
            }

            try
            {
                if (generalSettingsModel.browserType == 1)
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1jJ70"), driver);
                    if (isElementDisplayed == true)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                    }
                }

            }
            catch (Exception ex)
            {
                ChangeInitStatus(InitStatusEnum.Unable);
                timerInitChecker.Stop();
            }

            try
            {
                if (generalSettingsModel.browserType == 1)
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_aigu"), driver);
                    if (isElementDisplayed == true)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                    }
                }

            }
            catch (Exception ex)
            {
                ChangeInitStatus(InitStatusEnum.Unable);
                timerInitChecker.Stop();
            }
        }


        private void initBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        List<string> chatNames;

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool isEnd = false;
            chatNames = new List<string>();
            logger.WriteLog("Started Grabbing chat list");
            int nonINsertedCount = 0;
            while (isEnd == false)
            {
                logger.WriteLog("Not End");
                var list = driver.FindElements(By.XPath("//span[contains(@class,'_3m_Xw')] | //span[contains(@class,'_3q9s6')] | //div[contains(@class,'zoWT4')] "));
                logger.WriteLog("list count = " + list.Count());
                bool IsInserted = false;
                foreach (var chat in list)
                {
                    try
                    {
                        string ChatName = chat.FindElement(By.ClassName("ggj6brxn")).Text;
                        if ((chatNames.Where(_ => _ == ChatName).Count()) == 0)
                        {
                            chatNames.Add(ChatName);
                            IsInserted = true;
                        }
                    }
                    catch (Exception ex)
                    { }
                }
                if (IsInserted == false)
                {
                    nonINsertedCount++;
                }
                else
                {
                    nonINsertedCount = 0;
                }
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                driver.Manage().Window.Maximize();
                var res = (bool)js.ExecuteScript("return document.getElementById('pane-side').offsetHeight + document.getElementById('pane-side').scrollTop >= document.getElementById('pane-side').scrollHeight");
                Thread.Sleep(600);
                isEnd = res;
                if (isEnd == false && nonINsertedCount > 30)
                {
                    isEnd = true;
                }

                js.ExecuteScript("document.getElementById('pane-side').scrollBy(0,100)");
                logger.WriteLog("Js Executed");
            }



        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            String file = Path.Combine(FolderPath, "ChatList_" + Guid.NewGuid().ToString() + ".xlsx");

            string NewFileName = file.ToString();

            File.Copy("ChatListTemplate.xlsx", NewFileName, true);


            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];

                for (int i = 0; i < chatNames.Count(); i++)
                {
                    ws.Cells[i + 1, 1].Value = chatNames[i];
                }
                xlPackage.Save();
            }


            savesampleExceldialog.FileName = "ChatList.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
            ChangeCampStatus(CampaignStatusEnum.Finish);
        }

        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }
        List<WAPI_ContactModel> wAPI_ContactModel;
        private async void materialButton1_Click(object sender, EventArgs e)
        {
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {

                
                ChangeCampStatus(CampaignStatusEnum.Running);

                pgbar = new Progressbar();
                pgbar.Show();

                if (generalSettingsModel.browserType == 1)
                {
                    if (!WAPIHelper.IsWAPIInjected(driver))
                    {
                        ProjectCommon.injectWapi(driver);
                    }

                    pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
                    await WAPIHelper.isListReady(driver);
                    pgbar.materialLabel1.Text = Strings.fetchingdata;

                    wAPI_ContactModel = WAPIHelper.getAlChats(driver);
                }
                else if (generalSettingsModel.browserType == 2)
                {
                    WebView2 wv = Utils.GetActiveWebView(browser);

                    if (!await WPPHelper.isWPPinjected(wv))
                    {
                        await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                        Thread.Sleep(1000);
                    }
                    pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
                    await WPPHelper.isListReady(wv);
                    pgbar.materialLabel1.Text = Strings.fetchingdata;
                    try
                    {
                        wAPI_ContactModel = await WPPHelper.getAllChats(wv);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

                pgbar.Hide();


                String FolderPath = Config.GetTempFolderPath();
                String file = Path.Combine(FolderPath, "ChatList_" + Guid.NewGuid().ToString() + ".xlsx");

                string NewFileName = file.ToString();

                File.Copy("ChatListTemplate.xlsx", NewFileName, true);


                var newFile = new FileInfo(NewFileName);
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage xlPackage = new ExcelPackage(newFile))
                {
                    var ws = xlPackage.Workbook.Worksheets[0];

                    ws.Cells[1, 1].Value = "Number";
                    ws.Cells[1, 2].Value = "Name";
                    ws.Cells[1, 3].Value = "Labels";

                    for (int i = 0; i < wAPI_ContactModel.Count(); i++)
                    {
                        ws.Cells[i + 2, 1].Value = wAPI_ContactModel[i].number;
                        ws.Cells[i + 2, 2].Value = wAPI_ContactModel[i].Name;
                        ws.Cells[i + 2, 3].Value = wAPI_ContactModel[i].Labels;
                    }
                    xlPackage.Save();
                }


                savesampleExceldialog.FileName = "ChatstList.xlsx";
                savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                    Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
                }
                ChangeCampStatus(CampaignStatusEnum.Finish);

            }
            else
            {
                Utils.showAlert(Strings.Processisalreadyrunning, Alerts.Alert.enmType.Info);
            }
        }

        private void initWABrowser()
        {
            ChangeInitStatus(InitStatusEnum.Initialising);
            retryAttempt = 0;
            if (Utils.waSenderBrowser != null)
            {
                browser = Utils.waSenderBrowser;
            }
            else
            {
                browser = new WaSenderBrowser();
                Utils.waSenderBrowser = browser;
                browser.Show();
            }
            checkQRScanDoneBrowser();
        }
        private void initWA()
        {
            ChangeInitStatus(InitStatusEnum.Initialising);

            try
            {
                var s = driver.WindowHandles;
            }
            catch (Exception ex)
            {
                try
                {
                    Utils.Driver = null;
                    Utils.SetDriver();
                    this.driver = Utils.Driver;
                }
                catch (Exception eex)
                {
                    if (eex.Message.Contains("The specified executable is not a valid application for this OS platform"))
                    {
                        if (generalSettingsModel.browserType == 2)
                        {
                            MessageBox.Show(Strings.MSEdgeDriversarenotDownloadedProperly, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        if (generalSettingsModel.browserType == 1)
                        {
                            MessageBox.Show(Strings.ChromeDriversarenotDownloadedProperly, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }


            try
            {
                if (driver == null)
                {
                    Utils.SetDriver();
                    this.driver = Utils.Driver;
                }


                checkQRScanDone();
            }
            catch (Exception ex)
            {
                ChangeInitStatus(InitStatusEnum.Unable);
                if (ex.Message.Contains("session not created"))
                {
                    DialogResult dr = MessageBox.Show("Your Chrome Driver and Google Chrome Version Is not same, Click 'Yes botton' to Update it from Settings ", "Error ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if (dr == DialogResult.Yes)
                    {
                        this.Hide();
                        this.waSenderForm.Show();
                        GeneralSettings generalSettings = new GeneralSettings();
                        generalSettings.ShowDialog();
                    }
                }

                else if (ex.Message.Contains("invalid argument: user data directory is already in use"))
                {
                    _Config.KillChromeDriverProcess();
                    MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Please Close All Previous Sessions and Browsers if open, Then try again", Strings.OK, true);
                    SnackBarMessage.Show(this);
                }
            }
        }

        private void btnInitWA_Click(object sender, EventArgs e)
        {
            if (generalSettingsModel.browserType == 1)
            {
                initWA();
            }
            else if (generalSettingsModel.browserType == 2)
            {
                initWABrowser();
            }
        }

        private void GrabGroups_Load(object sender, EventArgs e)
        {
            init();
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
            this.Text = Strings.GrabChatList;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourmobile;
            btnInitWA.Text = Strings.ClicktoInitiate;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            materialButton1.Text = Strings.GrabAll;
            materialButton2.Text = Strings.GrabByLabel;

        }

        private void GrabGroups_FormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Complete();
            waSenderForm.formReturn(true);
        }

        
        public async void SelectLabelReturl(string selectedLebel)
        {
            if (generalSettingsModel.browserType == 1)
            {
                if (!WAPIHelper.IsWAPIInjected(driver))
                {
                    ProjectCommon.injectWapi(driver);
                }
                wAPI_ContactModel = WAPIHelper.getAlChatsWithLabel(driver, selectedLebel);
            }
            else if (generalSettingsModel.browserType == 2)
            {
                WebView2 wv = Utils.GetActiveWebView(browser);

                if (!await WPPHelper.isWPPinjected(wv))
                {
                    await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                    Thread.Sleep(1000);
                }
                wAPI_ContactModel = await WPPHelper.getAlChatsWithLabel(wv, selectedLebel);
            }

            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "ChatList_" + selectedLebel + "_" + Guid.NewGuid().ToString() + ".xlsx");

            string NewFileName = file.ToString();

            File.Copy("ChatListTemplate.xlsx", NewFileName, true);


            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];

                ws.Cells[1, 1].Value = "Number";
                ws.Cells[1, 2].Value = "Name";
                ws.Cells[1, 3].Value = "Label";

                for (int i = 0; i < wAPI_ContactModel.Count(); i++)
                {
                    ws.Cells[i + 2, 1].Value = wAPI_ContactModel[i].number;
                    ws.Cells[i + 2, 2].Value = wAPI_ContactModel[i].Name;
                    ws.Cells[i + 2, 3].Value = wAPI_ContactModel[i].Labels;
                }
                xlPackage.Save();
            }


            savesampleExceldialog.FileName = "ChatstList_" + selectedLebel + "_.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
            ChangeCampStatus(CampaignStatusEnum.Finish);
        }

        private async void materialButton2_Click_1(object sender, EventArgs e)
        {
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {
                ChangeCampStatus(CampaignStatusEnum.Running);


                pgbar = new Progressbar();
                pgbar.Show();

                if (generalSettingsModel.browserType == 1)
                {
                    if (!WAPIHelper.IsWAPIInjected(driver))
                    {
                        ProjectCommon.injectWapi(driver);
                    }

                    pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
                    await WAPIHelper.isListReady(driver);
                    pgbar.materialLabel1.Text = Strings.fetchingdata;

                    List<LableModel> sss = WAPIHelper.getAllLabels(driver);

                    pgbar.Hide();

                    if (sss.Count == 0)
                    {
                        MaterialSnackBar SnackBarMessage1 = new MaterialSnackBar(Strings.YouDonthaveanyLabeltoselect, Strings.OK, true);
                        SnackBarMessage1.Show(this);
                    }
                    else
                    {
                        SelectLabel form = new SelectLabel(this, sss);
                        form.ShowDialog();
                    }

                }
                else if (generalSettingsModel.browserType == 2)
                {
                    WebView2 wv = Utils.GetActiveWebView(browser);

                    if (!await WPPHelper.isWPPinjected(wv))
                    {
                        await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                        Thread.Sleep(1000);
                    }

                    pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
                    await WPPHelper.isListReady(wv);
                    pgbar.materialLabel1.Text = Strings.fetchingdata;

                    List<LableModel> sss = await WPPHelper.getAllLabels(wv);

                    pgbar.Hide();

                    if (sss.Count == 1)
                    {
                        if (sss[0].name == null)
                        {
                            MaterialSnackBar SnackBarMessage1 = new MaterialSnackBar(Strings.YouDonthaveanyLabeltoselect, Strings.OK, true);
                            SnackBarMessage1.Show(this);
                            return;
                        }
                    }

                    SelectLabel form = new SelectLabel(this, sss);
                    form.ShowDialog();
                }
            }

        }

    }
}
