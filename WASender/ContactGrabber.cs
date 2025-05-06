using MaterialSkin.Controls;
using Microsoft.Web.WebView2.WinForms;
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.enums;
using WASender.Models;

namespace WASender
{
    public partial class ContactGrabber : MaterialForm
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

        public ContactGrabber(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            logger = new Logger("ContactGrabber");
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
        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }

        private void ContactGrabber_Load(object sender, EventArgs e)
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
            if (generalSettingsModel.browserType == 1)
            {

                try
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1XkO3"), driver);

                    if (isElementDisplayed == true)
                    {
                        logger.WriteLog("_1XkO3 ElementDisplayed");
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
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
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1jJ70"), driver);
                    if (isElementDisplayed == true)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                    }
                }
                catch (Exception ex)
                {
                    ChangeInitStatus(InitStatusEnum.Unable);
                    timerInitChecker.Stop();
                }

                try
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
                catch (Exception ex)
                {
                    ChangeInitStatus(InitStatusEnum.Unable);
                    timerInitChecker.Stop();
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

        private void initBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        List<string> chatNames;
        List<WAPI_ContactModel> wAPI_ContactModel;
        List<WAPI_GroupModel> wAPI_GroupModel;


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool isEnd = false;
            chatNames = new List<string>();
            logger.WriteLog("Started Grabbing chat list");
            int nonINsertedCount = 0;
            try
            {
                By newChatButonBy = By.XPath("//*[@id=\"side\"]/header/div[2]/div/span/div[2]/div/span");
                if (AutomationCommon.IsElementPresent(newChatButonBy, driver))
                {
                    driver.FindElement(newChatButonBy).Click();
                    while (isEnd == false)
                    {

                        logger.WriteLog("Not End");
                        By HolderBy = By.ClassName("KPJpj");
                        if (AutomationCommon.IsElementPresent(HolderBy, driver))
                        {
                            IWebElement Holder = driver.FindElement(HolderBy);

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
                                {
                                    try
                                    {
                                        string ss = chat.GetAttribute("innerHTML");
                                        string ChatName = chat.FindElement(By.ClassName("_3q9s6")).Text;
                                        if ((chatNames.Where(_ => _ == ChatName).Count()) == 0)
                                        {
                                            chatNames.Add(ChatName);
                                            IsInserted = true;
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                            }
                            if (IsInserted == false)
                            {
                                nonINsertedCount++;
                            }
                            else
                            {
                                nonINsertedCount = 0;
                            }
                        }


                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                        var res = (bool)js.ExecuteScript("return document.getElementsByClassName('KPJpj')[0].offsetHeight + document.getElementsByClassName('KPJpj')[0].scrollTop >= document.getElementsByClassName('KPJpj')[0].scrollHeight");
                        Thread.Sleep(600);
                        isEnd = res;
                        if (isEnd == false && nonINsertedCount > 30)
                        {
                            isEnd = true;
                        }
                        driver.Manage().Window.Maximize();
                        js.ExecuteScript("document.getElementsByClassName('KPJpj')[0].scrollBy(0,100)");
                        logger.WriteLog("Js Executed");
                    }
                }
                else
                {
                    logger.WriteLog("New CHat Element is not Ready");
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(ex.Message);
                logger.WriteLog(ex.StackTrace);
            }


        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "ContactList_" + Guid.NewGuid().ToString() + ".xlsx");

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


            savesampleExceldialog.FileName = "ContactLIstList.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx");
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
            ChangeCampStatus(CampaignStatusEnum.Finish);
        }

        private void InitLanguage()
        {
            this.Text = Strings.ContactListGrabber;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourmobile;
            btnInitWA.Text = Strings.ClicktoInitiate;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            materialButton1.Text = Strings.GrabAllSavedContacts;
            materialButton2.Text = Strings.GrabAllGroups;

        }

        private void ContactGrabber_FormClosed(object sender, FormClosedEventArgs e)
        {

            logger.Complete();
            waSenderForm.formReturn(true);
            
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
                    DialogResult dr = MessageBox.Show("Your Chrome Driver and Google Chrome Version Is not same, Click 'Yes botton' to Update it from Settings", "Error ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
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

        private void checkQRScanDoneBrowser()
        {
            Thread.Sleep(1000);
            logger.WriteLog("checkQRScanDone");
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
        }

        private async void materialButton1_Click(object sender, EventArgs e)
        {
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
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
                wAPI_ContactModel = WAPIHelper.getMyContacts(driver);
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
                wAPI_ContactModel = await WPPHelper.getMyContacts(wv);

            }



            pgbar.Hide();
            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "ContactList_" + Guid.NewGuid().ToString() + ".xlsx");

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
                ws.Cells[1, 4].Value = "Country Short Code";

                for (int i = 0; i < wAPI_ContactModel.Count(); i++)
                {
                    ws.Cells[i + 2, 1].Value = wAPI_ContactModel[i].number;
                    ws.Cells[i + 2, 2].Value = wAPI_ContactModel[i].Name;
                    ws.Cells[i + 2, 3].Value = wAPI_ContactModel[i].Labels;
                    ws.Cells[i + 2, 4].Value = wAPI_ContactModel[i].countryShortCode;
                }
                xlPackage.Save();
            }


            savesampleExceldialog.FileName = "ContactList.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
            ChangeCampStatus(CampaignStatusEnum.Finish);

        }

        private async void materialButton2_Click(object sender, EventArgs e)
        {
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
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
                wAPI_GroupModel = await WAPIHelper.getMyGroups(driver);

                if (wAPI_GroupModel.Count() == 0)
                {
                    await Task.Delay(1000);
                    wAPI_GroupModel = await WAPIHelper.getMyGroups(driver);
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
                wAPI_GroupModel = await WPPHelper.getMyGroups(wv);
                if (wAPI_GroupModel.Count() == 0)
                {
                    await Task.Delay(1000);
                    wAPI_GroupModel = await WPPHelper.getMyGroups(wv);
                }
            }
            pgbar.Hide();

            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "GroupList_" + Guid.NewGuid().ToString() + ".xlsx");

            string NewFileName = file.ToString();

            File.Copy("ChatListTemplate.xlsx", NewFileName, true);


            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];

                ws.Cells[1, 1].Value = "GroupName";
                ws.Cells[1, 2].Value = "GroupId";
                ws.Cells[1, 3].Value = "CanSend";
                ws.Cells[1, 4].Value = "GroupLInk";


                for (int i = 0; i < wAPI_GroupModel.Count(); i++)
                {
                    ws.Cells[i + 2, 1].Value = wAPI_GroupModel[i].GroupName;
                    ws.Cells[i + 2, 2].Value = wAPI_GroupModel[i].GroupId;
                    ws.Cells[i + 2, 3].Value = wAPI_GroupModel[i].CanSend;
                    ws.Cells[i + 2, 4].Value = wAPI_GroupModel[i].GroupLink;
                }
                xlPackage.Save();
            }


            savesampleExceldialog.FileName = "GroupList.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
            ChangeCampStatus(CampaignStatusEnum.Finish);

        }
    }
}
