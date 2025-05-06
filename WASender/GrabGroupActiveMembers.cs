using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Web.WebView2.WinForms;
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.enums;
using WASender.Models;

namespace WASender
{
    public partial class GrabGroupActiveMembers : MaterialForm
    {
        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        CampaignStatusEnum campaignStatusEnum;
        WaSenderForm waSenderForm;
        Logger logger;
        System.Windows.Forms.Timer timerRunner;
        private static List<Models.ActiveMemberModel> activeMembersMain = new List<Models.ActiveMemberModel>();

        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;

        List<WAPI_GroupModel> wAPI_GroupModel;
        WAPI_GroupModel wAPI_SelectedGroup;

        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public GrabGroupActiveMembers(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            waSenderForm = _waSenderForm;
            generalSettingsModel = Config.GetSettings();
            logger = new Logger("GrabAcriveMembers");
            if (Utils.Driver != null)
            {
                if (generalSettingsModel.browserType == 1)
                {
                    Utils.SetDriver();
                    driver = Utils.Driver;
                    initWA();
                }
            }
            activeMembersMain = new List<ActiveMemberModel>();
            if (Utils.waSenderBrowser != null)
            {
                browser = Utils.waSenderBrowser;
                initWABrowser();
            }
            _testClass = Utils.testClass;
            _testClass.OnUpdateStatus += _testClass_OnUpdateStatus;
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

        void _testClass_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            ChangeInitStatus(InitStatusEnum.Stopped);
            ChangeCampStatus(CampaignStatusEnum.Stopped);
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
                //checkQRScanDone();
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


        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }

        private void init()
        {
            ChangeInitStatus(InitStatusEnum.NotInitialised);
        }


        private void initTimer()
        {
            timerRunner = new System.Windows.Forms.Timer();
            timerRunner.Interval = 1000;
            timerRunner.Tick += timerRunnerChecker_Tick;
            timerRunner.Start();
        }

        public void timerRunnerChecker_Tick(object sender, EventArgs e)
        {
            if (isStoped == false)
            {
                lbltotalfoundCount.Text = (activeMembersMain.Count() + activeMembers.Count()).ToString();
            }
        }

        private void checkQRScanDone()
        {
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
                        Activate();
                    }
                }
                catch (Exception ex)
                {
                    ChangeInitStatus(InitStatusEnum.Unable);
                    logger.WriteLog(ex.Message);
                    logger.WriteLog(ex.StackTrace);
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
                        initBackgroundWorker();
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




        private void btnInitWA_Click(object sender, EventArgs e)
        {

            if (generalSettingsModel.browserType == 1)
            {
                ChangeInitStatus(InitStatusEnum.Initialising);
                logger.WriteLog("ChangeInitStatus");
                try
                {
                    initWA();

                    checkQRScanDone();
                }
                catch (Exception ex)
                {
                    logger.WriteLog(ex.Message);
                    logger.WriteLog(ex.StackTrace);
                    ChangeInitStatus(InitStatusEnum.Unable);
                    string ss = "";
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
            else if (generalSettingsModel.browserType == 2)
            {
                initWABrowser();
            }


        }

        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }

        public async Task<List<WAPI_GroupModel>> getGroups()
        {
            List<WAPI_GroupModel> _WAPI_GroupModel = new List<WAPI_GroupModel>();
            WebView2 wv = Utils.GetActiveWebView(browser);
            
            if (!await WPPHelper.isWPPinjected(wv))
            {
                await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                Thread.Sleep(1000);
            }
            pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
            await WPPHelper.isListReady(wv);
            pgbar.materialLabel1.Text = Strings.fetchingdata;
            _WAPI_GroupModel = await WPPHelper.getMyGroups(wv);

            return _WAPI_GroupModel;
        }
        private async void materialButton1_Click(object sender, EventArgs e)
        {
            isStoped = false;
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                logger.WriteLog("!InitStatusEnum.Initialised");
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }

            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {



                if (generalSettingsModel.browserType == 1)
                {
                    activeMembersMain = new List<ActiveMemberModel>();
                    pgbar = new Progressbar();
                    pgbar.Show();

                    wAPI_GroupModel = await getGroupsChrome();
                    pgbar.Hide();
                    ChooseGroup ghooseGroup = new ChooseGroup(this, wAPI_GroupModel);
                    ghooseGroup.Show();
                }
                else if (generalSettingsModel.browserType == 2)
                {
                    activeMembersMain = new List<ActiveMemberModel>();
                    pgbar = new Progressbar();
                    pgbar.Show();

                    wAPI_GroupModel = await getGroups();
                    pgbar.Hide();
                    ChooseGroup ghooseGroup = new ChooseGroup(this, wAPI_GroupModel);
                    ghooseGroup.Show();
                }

            }

        }

        private async Task<List<WAPI_GroupModel>> getGroupsChrome()
        {
            List<WAPI_GroupModel> _WAPI_GroupModel = new List<WAPI_GroupModel>();
            //WebView2 wv = Utils.GetActiveWebView(browser);

            if (!WAPIHelper.IsWAPIInjected(driver))
            {
                ProjectCommon.injectWapi(driver);
            }

            pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
            await WAPIHelper.isListReady(driver);
            pgbar.materialLabel1.Text = Strings.fetchingdata;
            _WAPI_GroupModel = await WAPIHelper.getMyGroups(driver);

            return _WAPI_GroupModel;
        }
        public void ReturnBack(WAPI_GroupModel index)
        {
            wAPI_SelectedGroup = index;
            if (wAPI_SelectedGroup != null)
            {
                initBackgroundWorker();
                worker.RunWorkerAsync();
                initTimer();
                ChangeCampStatus(CampaignStatusEnum.Running);
                campaignStatusEnum = CampaignStatusEnum.Running;
            }
        }

        private void GrabGroupActiveMembers_Load(object sender, EventArgs e)
        {
            init();
            InitLanguage();
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
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
            this.Text = Strings.GrabActiveGroupMembers;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourmobile;
            materialLabel1.Text = Strings.OpenanyGroupchatClickbuttonbellow;
            btnInitWA.Text = Strings.ClicktoInitiate;
            materialButton1.Text = Strings.StartGrabbing;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            materialButton2.Text = Strings.Stop;
            label1.Text = Strings.TotalFound;
            materialButton3.Text = Strings.Export;
        }

        private void GrabGroupActiveMembers_FormClosing(object sender, FormClosingEventArgs e)
        {
            waSenderForm.formReturn(true);
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {

            if (campaignStatusEnum == CampaignStatusEnum.Running)
            {
                isStoped = true;
                timerRunner.Stop();
                ChangeCampStatus(CampaignStatusEnum.Stopped);
                campaignStatusEnum = CampaignStatusEnum.Stopped;
                activeMembersMain.AddRange(activeMembers);
                activeMembers = new List<Models.ActiveMemberModel>();
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

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (campaignStatusEnum == CampaignStatusEnum.Running)
            {
                isStoped = true;
                timerRunner.Stop();
                ChangeCampStatus(CampaignStatusEnum.Finish);
                campaignStatusEnum = CampaignStatusEnum.Finish;
                activeMembersMain.AddRange(activeMembers);
                lbltotalfoundCount.Text = activeMembersMain.Count().ToString();
                activeMembers = new List<Models.ActiveMemberModel>();
            }
        }


        private static bool isStoped = false;
        private static List<Models.ActiveMemberModel> activeMembers = new List<Models.ActiveMemberModel>();

        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            WebView2 wv = new WebView2();
            if (generalSettingsModel.browserType == 1)
            {
                try
                {
                    activeMembers = await WAPIHelper.getChatAndCount(driver, wAPI_SelectedGroup.GroupId);
                }
                catch (Exception ex)
                {

                }
                
            }
            else if (generalSettingsModel.browserType == 2)
            {
                browser.Invoke((MethodInvoker)delegate
                {
                    wv = Utils.GetActiveWebView(browser);
                });

                if (!await WPPHelper.isWPPinjectedAsync(wv))
                {
                    await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                    Thread.Sleep(1000);
                }
                try
                {
                    activeMembers = await WPPHelper.getChatAndCount(wv, wAPI_SelectedGroup.GroupId);
                }
                catch (Exception ex)
                {
                    
                }
            }   
        }


        private void materialButton3_Click(object sender, EventArgs e)
        {

            if (campaignStatusEnum == CampaignStatusEnum.Stopped || campaignStatusEnum == CampaignStatusEnum.Finish)
            {
                if (activeMembersMain.Count() == 0)
                {
                    Utils.showAlert(Strings.Nothingtoexport, Alerts.Alert.enmType.Warning);
                    logger.WriteLog("Nothing to export");
                }
                else
                {
                    string GroupName = "Multiple_Group";

                    String FolderPath = Config.GetTempFolderPath();
                    String file = Path.Combine(FolderPath, "" + GroupName + "_Members_" + Guid.NewGuid().ToString() + ".xlsx");
                    string NewFileName = file.ToString();
                    File.Copy("MemberListTemplate.xlsx", NewFileName, true);
                    var newFile = new FileInfo(NewFileName);

                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (ExcelPackage xlPackage = new ExcelPackage(newFile))
                    {
                        var ws = xlPackage.Workbook.Worksheets[0];
                        ws.Cells[1, 1].Value = "Contact Name";
                        ws.Cells[1, 2].Value = "Total Messages";

                        int i = 1;

                        var nonduplicates = activeMembersMain.GroupBy(x => x.number).Select(y => y.First());

                        foreach (var item in nonduplicates.OrderByDescending(x => x.count))
                        {
                            ws.Cells[i + 1, 1].Value = AutomationCommon.GetNumbers(item.number);
                            ws.Cells[i + 1, 2].Value = activeMembersMain.Where(x => x.number == item.number).Count();
                            
                            i++;
                        }
                        xlPackage.Save();
                    }
                    savesampleExceldialog.FileName = GroupName + "_Active_Members.xlsx";
                    savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                    if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                        Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
                    }
                }
            }

        }
    }
}
