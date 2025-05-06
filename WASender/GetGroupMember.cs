using MaterialSkin;
using MaterialSkin.Controls;
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
using Microsoft.Web.WebView2.WinForms;

namespace WASender
{
    public partial class GetGroupMember : MaterialForm
    {
        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        CampaignStatusEnum campaignStatusEnum;
        WaSenderForm waSenderForm;
        Logger logger;
        List<WAPI_GroupModel> wAPI_GroupModel;

        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public GetGroupMember(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            waSenderForm = _waSenderForm;
            logger = new Logger("GteGroupMembers");
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


        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }

        private void init()
        {
            ChangeInitStatus(InitStatusEnum.NotInitialised);
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
                        Activate();
                    }
                }
                catch (Exception ex)
                {
                    ChangeInitStatus(InitStatusEnum.Unable);
                    timerInitChecker.Stop();
                }
            }
            if (generalSettingsModel.browserType == 2)
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


        List<string> chatNames;

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

        private void GetGroupMember_Load(object sender, EventArgs e)
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
            this.Text = Strings.GetGroupMember;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourmobile;
            materialLabel1.Text = Strings.OpenanyGroupchatClickbuttonbellow;
            btnInitWA.Text = Strings.ClicktoInitiate;
            materialButton1.Text = Strings.GetGroupMember;
            materialButton2.Text = Strings.GetCommunityMembers;
            label5.Text = Strings.Status;
        }




        string DownloaType = "";


        public async Task<List<WAPI_GroupModel>> getGroups()
        {
            List<WAPI_GroupModel> _wAPI_GroupModel = new List<WAPI_GroupModel>();
            if (generalSettingsModel.browserType == 1)
            {

                {
                    if (!WAPIHelper.IsWAPIInjected(driver))
                    {
                        ProjectCommon.injectWapi(driver);
                    }

                    pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
                    await WAPIHelper.isListReady(driver);
                    pgbar.materialLabel1.Text = Strings.fetchingdata;

                    _wAPI_GroupModel = await WAPIHelper.getMyGroups(driver);
                    return _wAPI_GroupModel;
                }
            }
            else if (generalSettingsModel.browserType == 2)
            {
                WebView2 wv = Utils.GetActiveWebView(browser);

                if (!await WPPHelper.isWPPinjected(wv))
                {
                    await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                    await Task.Delay(500);
                }

                pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
                await WPPHelper.isListReady(wv);
                pgbar.materialLabel1.Text = Strings.fetchingdata;
                _wAPI_GroupModel = await WPPHelper.getMyGroups(wv);
                return _wAPI_GroupModel;
            }
            return _wAPI_GroupModel;

        }

        private async void materialButton1_Click(object sender, EventArgs e)
        {

            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                logger.WriteLog("!InitStatusEnum.Initialised");
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {
                DownloaType = "Group";
                if (generalSettingsModel.browserType == 1)
                {
                    pgbar = new Progressbar();
                    pgbar.Show();

                    wAPI_GroupModel = await getGroups();
                    ChooseGroup ghooseGroup = new ChooseGroup(this, wAPI_GroupModel, true);
                    pgbar.Hide();
                    ghooseGroup.Show();
                }
                else if (generalSettingsModel.browserType == 2)
                {
                    pgbar = new Progressbar();
                    pgbar.Show();
                    try
                    {
                        wAPI_GroupModel = await getGroups();
                        ChooseGroup ghooseGroup = new ChooseGroup(this, wAPI_GroupModel, true);
                        pgbar.Hide();
                        ghooseGroup.Show();
                    }
                    catch (Exception ex)
                    {
                        pgbar.Hide();
                    }

                }

            }
            else
            {
                Utils.showAlert(Strings.Processisalreadyrunning, Alerts.Alert.enmType.Info);
            }
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
                Utils.Driver = null;
                try
                {
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
                else
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void GetGroupMember_FormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Complete();
            waSenderForm.formReturn(true);
        }



        internal async void ReturnBack(List<WAPI_GroupModel> p)
        {
            List<GroupMembersModel> members = new List<GroupMembersModel>();

            if (generalSettingsModel.browserType == 1)
            {

                if (!WAPIHelper.IsWAPIInjected(driver))
                {
                    ProjectCommon.injectWapi(driver);
                }
                foreach (var group in p)
                {
                    members.AddRange(WAPIHelper.GetGroupMembers(group, driver));
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
                foreach (var group in p)
                {
                    try
                    {
                        members.AddRange(await WPPHelper.GetGroupMembers(group, wv));
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

            }


            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "Multiple_" + DownloaType + "_Members" + Guid.NewGuid().ToString() + ".xlsx");
            string NewFileName = file.ToString();

            File.Copy("MemberListTemplate.xlsx", NewFileName, true);


            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];
                ws.Cells[ 1, 1].Value = Strings.Number;
                ws.Cells[1, 2].Value = Strings.CountryShortCode;
                for (int i = 0; i < members.Count(); i++)
                {
                    ws.Cells[i + 2,1].Value = members[i].number;
                    ws.Cells[i + 2, 2].Value = members[i].CountryShortCode;
                }
                xlPackage.Save();
            }


            savesampleExceldialog.FileName = "Multiple_" + DownloaType + "_Members.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                    Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private async void materialButton2_Click(object sender, EventArgs e)
        {
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                logger.WriteLog("!InitStatusEnum.Initialised");
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {
                var confirmResult = MessageBox.Show(Strings.Afterobtainingthelistofcommunitymembers,
                                      Strings.PleaseConfirm + "!!",
                                     MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.OK)
                {
                    pgbar = new Progressbar();
                    pgbar.Show();


                    DownloaType = "Community";
                    if (generalSettingsModel.browserType == 1)
                    {

                        if (Config.SendingType == 1)
                        {
                            if (!WAPIHelper.IsWAPIInjected(driver))
                            {
                                ProjectCommon.injectWapi(driver);

                            }

                            pgbar.materialLabel1.Text = Strings.WaitingforyourWhatsappAccountReadiness;
                            await WAPIHelper.isListReady(driver);

                            try
                            {
                                wAPI_GroupModel = WAPIHelper.getMyCommunities(driver);
                                ChooseGroup ghooseGroup = new ChooseGroup(this, wAPI_GroupModel, true, true);
                                ghooseGroup.Show();
                            }
                            catch (Exception ffffe)
                            { }
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

                        wAPI_GroupModel = await WPPHelper.getMyCommunities(wv);
                        ChooseGroup ghooseGroup = new ChooseGroup(this, wAPI_GroupModel, true, true);
                        ghooseGroup.Show();
                    }

                    pgbar.Hide();
                }



            }
            else
            {
                Utils.showAlert(Strings.Processisalreadyrunning, Alerts.Alert.enmType.Info);
            }
        }
    }
}
