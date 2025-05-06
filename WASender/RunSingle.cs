using MaterialSkin;
using MaterialSkin.Controls;
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
using WASender.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Web;
using Microsoft.Web.WebView2.WinForms;
using Models;
using Newtonsoft.Json;

namespace WASender
{
    public partial class RunSingle : MaterialForm
    {
        WASenderSingleTransModel wASenderSingleTransModel;
        WaSenderForm waSenderForm;
        InitStatusEnum initStatusEnum;
        CampaignStatusEnum campaignStatusEnum;
        IWebDriver driver;
        System.Windows.Forms.Timer timerInitChecker;
        System.Windows.Forms.Timer timerRunner;
        BackgroundWorker worker;
        Logger logger;
        private bool IsStopped = true;
        private bool IsPaused = false;
        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;
        SchedulesModel schedulesModel;
        bool forceScheduleRun = false;
        bool isSkippedNumberCHecking = false;
        public RunSingle(WASenderSingleTransModel _wASenderSingleTransModel, WaSenderForm _waSenderForm, SchedulesModel _schedulesModel = null, bool _forceScheduleRun = false)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            logger = new Logger("RunSingle");
            this.waSenderForm = _waSenderForm;
            this.wASenderSingleTransModel = _wASenderSingleTransModel;
            this.Text = _wASenderSingleTransModel.CampaignName;
            forceScheduleRun = _forceScheduleRun;
            if (_schedulesModel == null)
            {
                generalSettingsModel = Config.GetSettings();
            }
            else
            {
                generalSettingsModel = _wASenderSingleTransModel.generalSettingsModel;
            }


            schedulesModel = _schedulesModel;
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
                Utils.waSenderBrowser.Close();
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
                browser = new WaSenderBrowser(wASenderSingleTransModel.selectedAccounts);
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
            IsStopped = true;
            ChangeCampStatus(CampaignStatusEnum.Stopped);
        }

        private void RunForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            logger.Complete();
            try
            {
                IsStopped = true;
                worker.CancelAsync();
                worker.Dispose();
            }
            catch (Exception ex)
            {

            }
            if (Utils.waSenderBrowser != null)
            {
                Utils.waSenderBrowser.Close();
            }
            waSenderForm.formReturn(true);
        }

        private void RunSingle_Load(object sender, EventArgs e)
        {
            init();
            initLanguage();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                this.Invoke(new Action(() =>
                    CheckForActivation()));
            });


            semulate();
        }

        private async void semulate()
        {
            if (schedulesModel != null)
            {
                await Task.Delay(2000);
                initBaseWA();
            }
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

        private void initLanguage()
        {
            this.Text = Strings.Run;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourrmobile;
            btnInitWA.Text = Strings.ClicktoInitiate;
            label5.Text = Strings.Status;
            btnSTart.Text = Strings.Start;
            materialButton1.Text = Strings.Pause;
            materialButton2.Text = Strings.Stop;
            label7.Text = Strings.Status;
            label8.Text = Strings.Log;
            gridStatus.Columns[0].HeaderText = Strings.ChatName;
            gridStatus.Columns[1].HeaderText = Strings.Status;
            label4.Text = Strings.ImportentNotes;
            label1.Text = Strings.Keepapplicationopenwhilesendingmessagesanduntilallmessagesaresentfromyourmobile;
            label2.Text = Strings.DontuseyourAccountforbulkSendingifitsbrandnew;
            label3.Text = Strings.WaSendertendstosubmitmessagestoyourphoneisnotresponsiblefordeliveryofthemessage;
        }

        private void init()
        {
            label4.ForeColor = Color.Red;
            ChangeInitStatus(InitStatusEnum.NotInitialised);
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
        }

        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            this.campaignStatusEnum = _campaignStatus;
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }

        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }

        private void checkQRScanDone()
        {
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
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
                        this.waSenderForm.Show();
                        this.waSenderForm.OpenGeneralSettings();
                        this.Close();
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
            initBaseWA();
        }

        private void initBaseWA()
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

        private async void startAutoRun()
        {
            if (schedulesModel != null)
            {
                await Task.Delay(2000);
                if (generalSettingsModel.filterNumbersBeforeSendingMessage == true)
                {
                    oppUpFilter();
                }
                else
                {
                    btnStartClick();
                }
            }

        }
        int retryAttempt = 0;
        bool isAllFocused = false;

        private void doneInitialisation()
        {

        }
        public async void timerInitChecker_Tick(object sender, EventArgs e)
        {
            if (generalSettingsModel.browserType == 1)
            {
                try
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1XkO3"), driver);
                    if (isElementDisplayed == true)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                        startAutoRun();
                    }
                }
                catch (Exception ex)
                {
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
                        startAutoRun();
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
                        startAutoRun();
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
                    bool IsAllinitiated = true;
                    foreach (TabPage tab in browser.tabControl1.TabPages)
                    {
                        WebView2 vw = (WebView2)tab.Controls.Find("webView21", true)[0];
                        string name = tab.Text;
                        bool isInitiated = await WPPHelper.isWaInited(vw);

                        IsAllinitiated = isInitiated;
                    }
                    if (IsAllinitiated)
                    {
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        startAutoRun();
                    }

                    if (schedulesModel != null && isAllFocused == false)
                    {
                        foreach (TabPage tab in browser.tabControl1.TabPages)
                        {
                            browser.tabControl1.SelectedTab = tab;
                            Thread.Sleep(500);
                        }
                        isAllFocused = true;
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
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            ChangeCampStatus(CampaignStatusEnum.NotStarted);
        }



        private static bool CanStopWaitingForDelevetry = false;

        private void SendMessage(MesageModel mesageModel, bool isFirstMessage, ContactModel item, WebView2 wv)
        {
            try
            {
                bool AutoSend = false;
                bool isAlreadySent = false;
                if (mesageModel.buttons != null && mesageModel.buttons.Count() > 0)
                {
                    AutoSend = true;
                }

                isFirstMessage = false;
                Thread.Sleep(500);
                string NewMessage = ProjectCommon.ReplaceKeyMarker(mesageModel.longMessage, item.parameterModelList);

                ButtonHolderModel holder = null;
                if (mesageModel.buttons.Count() > 0)
                {
                    holder = mesageModel.buttons[0];
                }

                foreach (var file in mesageModel.files)
                {
                    file.fileName = Path.GetFileName(file.filePath);
                    Byte[] bytes = File.ReadAllBytes(file.filePath);
                    String filebase64 = Convert.ToBase64String(bytes);

                    string ext = Path.GetExtension(file.filePath);

                    string contentType = MimeMapping.GetMimeMapping(file.filePath);
                    if (ext == ".mp4")
                    {
                        contentType = "video/mp4";
                    }
                    string fullBase64 = "data:" + contentType + ";base64," + filebase64;
                    string FileName = file.filePath.Split('\\')[file.filePath.Split('\\').Length - 1];

                    if (ext == ".mp4")
                    {
                        try
                        {
                            if (file.Caption != null)
                            {
                                file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption, item.parameterModelList);
                            }


                            if (file.attachWithMainMessage == true)
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendVideo(driver, item.number, fullBase64, NewMessage, file.fileName,holder);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    WPPHelper.SendVideoAsync(wv, item.number, fullBase64, NewMessage, "",holder);
                                }

                                AutoSend = false;
                                isAlreadySent = true;
                            }
                            else
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendVideo(driver, item.number, fullBase64, file.Caption, file.fileName);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    WPPHelper.SendVideoAsync(wv, item.number, fullBase64, file.Caption);
                                }

                            }


                        }
                        catch (Exception eeex)
                        {
                            logger.WriteLog("Is Number Valid-" + eeex.Message);
                        }

                    }
                    else if (ext == ".ogg")
                    {
                        try
                        {
                            if (file.Caption != null)
                            {
                                file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption, item.parameterModelList);
                            }

                            fullBase64 = fullBase64.Replace("data:application/octet-stream;", "data:audio/mp3;");

                            if (file.attachWithMainMessage == true)
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendAudio(driver, item.number, fullBase64, NewMessage, holder);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    WPPHelper.SendAudioAsync(wv, item.number, fullBase64, NewMessage,holder);
                                }

                                AutoSend = false;
                                isAlreadySent = true;
                            }
                            else
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendAudio(driver, item.number, fullBase64, file.Caption);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    WPPHelper.SendAudioAsync(wv, item.number, fullBase64, file.Caption,holder);
                                }
                            }
                        }
                        catch (Exception eeex)
                        {
                            logger.WriteLog("Is Number Valid-" + eeex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            string OriginalCaption = file.Caption;
                            if (file.Caption != null)
                            {
                                file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption, item.parameterModelList);
                            }

                            if (file.attachWithMainMessage == true)
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendVideo(driver, item.number, fullBase64, NewMessage, file.fileName,holder);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    WPPHelper.SendVideoAsync(wv, item.number, fullBase64, NewMessage, file.fileName,holder);
                                }

                                AutoSend = false;
                                isAlreadySent = true;
                            }
                            else
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendAttachment(driver, item.number, fullBase64, FileName, file.Caption);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    WPPHelper.SendAttachmentAsync(wv, item.number, fullBase64, FileName, file.Caption);
                                }

                            }
                            file.Caption = OriginalCaption;

                        }
                        catch (Exception eex)
                        {
                            logger.WriteLog("Is Number Valid-" + eex.Message);
                        }
                    }

                    if (isFirstMessage == true)
                    {
                        Thread.Sleep(2000);
                    }
                    Thread.Sleep(Utils.getRandom(wASenderSingleTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderSingleTransModel.settings.delayAfterEveryMessageTo * 1000));
                }


                if (AutoSend == false)
                {

                    try
                    {
                        if (isAlreadySent == false)
                        {
                            if (NewMessage.Contains("http://") || NewMessage.Contains("https://"))
                            {
                                var rand = Utils.getRandom(500, 1000);
                                if (generalSettingsModel.browserType == 1)
                                {
                                    try
                                    {
                                        if (!generalSettingsModel.disableLinkPreview)
                                        {
                                            WAPIHelper.sendTextMessageWithPreview(driver, item.number, NewMessage, false);
                                        }
                                        else
                                        {
                                            WAPIHelper.SendMessage(driver, item.number, NewMessage);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        WAPIHelper.SendMessage(driver, item.number, NewMessage);
                                    }
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    try
                                    {
                                        if (!generalSettingsModel.disableLinkPreview)
                                        {
                                            WPPHelper.sendTextMessageWithPreview(wv, item.number, NewMessage);
                                        }
                                        else
                                        {
                                            WPPHelper.SendMessage(wv, item.number, NewMessage);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        WPPHelper.SendMessage(wv, item.number, NewMessage);
                                    }
                                }

                            }
                            else
                            {
                                var rand = Utils.getRandom(500, 1000);

                                if (generalSettingsModel.browserType == 1)
                                {
                                    if (NewMessage != "")
                                    {
                                        WAPIHelper.SendMessage(driver, item.number, NewMessage);
                                    }

                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    if (NewMessage != "")
                                    {
                                        WPPHelper.SendMessage(wv, item.number, NewMessage);
                                    }

                                }


                            }
                        }
                        if (mesageModel.polls != null && mesageModel.polls.Count() > 0)
                        {
                            foreach (var poll in mesageModel.polls)
                            {
                                try
                                {
                                    if (generalSettingsModel.browserType == 1)
                                    {
                                        WAPIHelper.sendCreatePollMessageToNumber(driver, item.number, poll);
                                        Thread.Sleep(500);
                                    }
                                    else if (generalSettingsModel.browserType == 2)
                                    {
                                        WPPHelper.sendCreatePollMessageToNumber(wv, item.number, poll);
                                        Thread.Sleep(500);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }

                        }
                        else if (holder !=null)
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                //WAPIHelper.Sen(driver, item.number, fullBase64, FileName, file.Caption);
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendDirectMessageAsync(wv, item.number, NewMessage,holder);
                            }
                        }

                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }

                }
                else
                {
                    try
                    {
                        if (isAlreadySent == false)
                        {
                            var rand = Utils.getRandom(500, 1000);

                            if (generalSettingsModel.browserType == 1)
                            {

                            }
                            else if (generalSettingsModel.browserType == 2)
                            {

                            }



                        }

                    }
                    catch (Exception ex)
                    {

                        logger.WriteLog("Is Number Valid-" + ex.Message);
                    }
                }

                Thread.Sleep(Utils.getRandom(wASenderSingleTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderSingleTransModel.settings.delayAfterEveryMessageTo * 1000));



            }
            catch (Exception ex)
            {
                logger.WriteLog("1309=" + ex.Message);
            }
        }


        private async Task<bool> SendMessageSync(MesageModel mesageModel, bool isFirstMessage, ContactModel item, WebView2 wv)
        {
            try
            {
                bool AutoSend = false;
                bool isAlreadySent = false;
                if (mesageModel.buttons != null && mesageModel.buttons.Count() > 0)
                {
                    AutoSend = false;
                }

                isFirstMessage = false;
                //await Task.Delay(500);
                string NewMessage = ProjectCommon.ReplaceKeyMarker(mesageModel.longMessage, item.parameterModelList);

                ButtonHolderModel holder = null;
                if (mesageModel.buttons.Count() > 0)
                {
                    holder = mesageModel.buttons[0];
                }

                foreach (var file in mesageModel.files)
                {
                    file.fileName = Path.GetFileName(file.filePath);
                    Byte[] bytes = File.ReadAllBytes(file.filePath);
                    String filebase64 = Convert.ToBase64String(bytes);

                    string ext = Path.GetExtension(file.filePath);

                    string contentType = MimeMapping.GetMimeMapping(file.filePath);
                    if (ext == ".mp4")
                    {
                        contentType = "video/mp4";
                    }
                    string fullBase64 = "data:" + contentType + ";base64," + filebase64;
                    string FileName = file.filePath.Split('\\')[file.filePath.Split('\\').Length - 1];

                    if (ext == ".mp4")
                    {
                        try
                        {
                            if (file.Caption != null)
                            {
                                file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption, item.parameterModelList);
                            }

                           


                            if (file.attachWithMainMessage == true)
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendVideo(driver, item.number, fullBase64, NewMessage, file.fileName,holder);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    bool isDOne=await WPPHelper.SendVideosync(wv, item.number, fullBase64, NewMessage,"",holder);
                                }

                                AutoSend = false;
                                isAlreadySent = true;
                            }
                            else
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendVideo(driver, item.number, fullBase64, file.Caption, file.fileName);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    bool isDOne = await WPPHelper.SendVideosync(wv, item.number, fullBase64, file.Caption);
                                }

                            }


                        }
                        catch (Exception eeex)
                        {
                            logger.WriteLog("Is Number Valid-" + eeex.Message);
                        }

                    }
                    else if (ext == ".ogg")
                    {
                        try
                        {
                            if (file.Caption != null)
                            {
                                file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption, item.parameterModelList);
                            }

                          
                            fullBase64 = fullBase64.Replace("data:application/octet-stream;", "data:audio/mp3;");

                            if (file.attachWithMainMessage == true)
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendAudio(driver, item.number, fullBase64, NewMessage,holder);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    bool isDone = await WPPHelper.SendVideosync(wv, item.number, fullBase64, NewMessage, "", holder);
                                }

                                AutoSend = false;
                                isAlreadySent = true;
                            }
                            else
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendAudio(driver, item.number, fullBase64, file.Caption);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    bool isDone = await WPPHelper.SendVideosync(wv, item.number, fullBase64, file.Caption);
                                }
                            }
                        }
                        catch (Exception eeex)
                        {
                            logger.WriteLog("Is Number Valid-" + eeex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            string OriginalCaption = file.Caption;
                            if (file.Caption != null)
                            {
                                file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption, item.parameterModelList);
                            }

                            
                            if (file.attachWithMainMessage == true)
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendVideo(driver, item.number, fullBase64, NewMessage, file.fileName,holder);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    bool isDone = await WPPHelper.SendVideosync(wv, item.number, fullBase64, NewMessage, file.fileName,holder);
                                }

                                AutoSend = false;
                                isAlreadySent = true;
                            }
                            else
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    WAPIHelper.SendAttachment(driver, item.number, fullBase64, FileName, file.Caption);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    bool isDOne=await WPPHelper.SendAttachmentsync(wv, item.number, fullBase64, FileName, file.Caption);
                                }

                            }
                            file.Caption = OriginalCaption;

                        }
                        catch (Exception eex)
                        {
                            logger.WriteLog("Is Number Valid-" + eex.Message);
                        }
                    }

                    await Task.Delay(Utils.getRandom(wASenderSingleTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderSingleTransModel.settings.delayAfterEveryMessageTo * 1000));
                }


                if (AutoSend == false)
                {

                    try
                    {
                        if (isAlreadySent == false)
                        {
                            if (NewMessage.Contains("http://") || NewMessage.Contains("https://"))
                            {
                                var rand = Utils.getRandom(500, 1000);
                                if (generalSettingsModel.browserType == 1)
                                {
                                    try
                                    {
                                        if (!generalSettingsModel.disableLinkPreview && holder==null)
                                        {
                                            WAPIHelper.sendTextMessageWithPreview(driver, item.number, NewMessage, false);
                                        }
                                        else
                                        {
                                            WAPIHelper.SendMessage(driver, item.number, NewMessage,false,false,holder);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        WAPIHelper.SendMessage(driver, item.number, NewMessage, false, false, holder);
                                    }
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    try
                                    {
                                        if (!generalSettingsModel.disableLinkPreview && holder ==null)
                                        {
                                            bool isDone=await WPPHelper.sendTextMessageWithPreviewSync(wv, item.number, NewMessage);
                                        }
                                        else
                                        {
                                            bool isDOne = await WPPHelper.SendMessageSync(wv, item.number, NewMessage,false,false,holder);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }

                            }
                            else
                            {
                                
                                if (generalSettingsModel.browserType == 1)
                                {
                                    if (NewMessage != "")
                                    {
                                        WAPIHelper.SendMessage(driver, item.number, NewMessage,false,false,holder);
                                    }

                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    if (NewMessage != "")
                                    {
                                        bool isDOne = await WPPHelper.SendMessageSync(wv, item.number, NewMessage,false,false,holder);
                                    }

                                }


                            }
                        }
                        if (mesageModel.polls != null && mesageModel.polls.Count() > 0)
                        {
                            foreach (var poll in mesageModel.polls)
                            {
                                try
                                {
                                    if (generalSettingsModel.browserType == 1)
                                    {
                                        WAPIHelper.sendCreatePollMessageToNumber(driver, item.number, poll);
                                        await Task.Delay(500);
                                    }
                                    else if (generalSettingsModel.browserType == 2)
                                    {
                                        bool isDone= await WPPHelper.sendCreatePollMessageToNumberSync(wv, item.number, poll);
                                        await Task.Delay(500);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            }

                        }

                    }
                    catch (Exception ex)
                    {

                        throw new Exception(ex.Message);
                    }

                }

                await Task.Delay(Utils.getRandom(wASenderSingleTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderSingleTransModel.settings.delayAfterEveryMessageTo * 1000));



            }
            catch (Exception ex)
            {
                logger.WriteLog("1309=" + ex.Message);
            }

            return true;
        }

        private async Task<bool> doStartWork()
        {
            int counter = 0;
            int totalCounter = 0;
            if (Config.SendingType == 1)
            {

                try
                {
                    logger.WriteLog("Checking IsWAPIInjected");
                    bool isFirstMessage = true;

                    if (generalSettingsModel.browserType == 2)
                    {
                        int accountSwitchNumber = wASenderSingleTransModel.swipeAccountAfterMessages;
                        int accountSwitchCounter = 0;
                        int MinimumNumber = 0;
                        ConnectedAccountModel assignableAccount;
                        foreach (var contact in wASenderSingleTransModel.contactList)
                        {
                            MinimumNumber = wASenderSingleTransModel.selectedAccounts.Min(x => x.usedCount);
                            assignableAccount = wASenderSingleTransModel.selectedAccounts.OrderBy(x => Convert.ToInt32(x.ID)).Where(x => x.usedCount == MinimumNumber).FirstOrDefault();

                            if (accountSwitchCounter < accountSwitchNumber)
                            {
                                contact.senderName = assignableAccount.sessionName;
                                contact.senderId = assignableAccount.ID;
                                accountSwitchCounter++;
                            }
                            else
                            {
                                MinimumNumber = 0;
                                assignableAccount.usedCount = assignableAccount.usedCount + 1;

                                MinimumNumber = wASenderSingleTransModel.selectedAccounts.Min(x => x.usedCount);
                                assignableAccount = wASenderSingleTransModel.selectedAccounts.OrderBy(x => Convert.ToInt32(x.ID)).Where(x => x.usedCount == MinimumNumber).FirstOrDefault();
                                contact.senderName = assignableAccount.sessionName;
                                contact.senderId = assignableAccount.ID;
                                accountSwitchCounter = 1;
                            }
                        }
                    }

                    List<string> blockList = Config.getAllBlockListes();

                    foreach (var item in wASenderSingleTransModel.contactList)
                    {
                        bool isBlockListed = false;

                        if (IsPaused)
                        {
                            while (IsPaused) ;
                        }
                        if (IsStopped)
                        {
                            return true;
                        }

                        WebView2 wv = new WebView2();
                        MainUC mainUC = null;
                        if (generalSettingsModel.browserType == 1)
                        {
                            try
                            {
                                if (!WAPIHelper.IsWAPIInjected(driver))
                                {
                                    ProjectCommon.injectWapi(driver);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLog("in Each-" + ex.Message);
                            }
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            try
                            {
                                wv = Utils.GetWebViewById(browser, item.senderId);
                                TabPage tp = Utils.GetTabPageById(browser, item.senderId);
                                if (tp != null)
                                {
                                    browser.tabControl1.SelectedTab = tp;
                                }

                                mainUC = Utils.GetMainUCId(browser, item.senderId);

                                if (!await WPPHelper.isWPPinjected(wv))
                                {
                                    if (mainUC._isWPPIJected == false)
                                    {
                                        await WPPHelper.InjectWapiSync(wv, Config.GetSysFolderPath());
                                        mainUC._isWPPIJected = true;
                                        await Task.Delay(1000);
                                    }

                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }


                        bool isNumberValid = false;
                        if (generalSettingsModel.filterNumbersBeforeSendingMessage == false || isSkippedNumberCHecking == true)
                        {
                            isNumberValid = true;
                        }
                        else
                        {
                            isNumberValid = item.sendStatusModel.sendStatusEnum == SendStatusEnum.Available ? true : false;
                        }
                        bool checkNumberststus = true;
                        try
                        {
                            item.number = item.number.Replace("+", "");
                            item.number = item.number.Replace(" ", "");
                            item.number = item.number.Replace("-", "");
                        }
                        catch (Exception ex)
                        {

                        }
                        if (blockList.Where(z => z == item.number).Count() > 0)
                        {
                            isBlockListed = true;
                        }

                        try
                        {

                            if (item.number.EndsWith("@uid"))
                            {
                                checkNumberststus = false;
                                isNumberValid = true;
                            }

                            if (item.number.StartsWith("55"))
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    item.number = WAPIHelper.queryExists(driver, item.number);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    item.number = await WPPHelper.queryExistsSync(wv, item.number);
                                }


                            }
                            if (item.number.StartsWith("225"))
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    item.number = WAPIHelper.queryExists(driver, item.number);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    item.number = await WPPHelper.queryExistsSync(wv, item.number);
                                }


                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        bool IsAvailableinchat = true;

                        if (isNumberValid == true && wASenderSingleTransModel.isSafeMode == true && isBlockListed == false)
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                IsAvailableinchat = WAPIHelper.getChat(driver, item.number);
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                IsAvailableinchat = await WPPHelper.getChatSync(wv, item.number);
                            }
                        }

                        if (isBlockListed == true)
                        {
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.BlockListed_Ignored;
                            counter++;
                            totalCounter++;
                            var _count = wASenderSingleTransModel.contactList.Count();
                            var percentage = totalCounter * 100 / _count;
                            //worker.ReportProgress(percentage);
                        }
                        else if (IsAvailableinchat == false)
                        {
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.SafeMode_Ignored;
                            counter++;
                            totalCounter++;

                            var _count = wASenderSingleTransModel.contactList.Count();
                            var percentage = totalCounter * 100 / _count;
                            // worker.ReportProgress(percentage);
                        }
                        else if (!isNumberValid)
                        {
                            item.sendStatusModel.isDone = true;

                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.ContactNotFound;
                            counter++;
                            totalCounter++;

                            var _count = wASenderSingleTransModel.contactList.Count();
                            var percentage = totalCounter * 100 / _count;
                            // worker.ReportProgress(percentage);
                        }
                        else
                        {
                            bool IsError = false;
                            if (wASenderSingleTransModel.IsRotateMessages)
                            {
                                var random = new Random();
                                var list = wASenderSingleTransModel.messages.Where(x => x != null).ToList();
                                int index = random.Next(list.Count);
                                MesageModel mesageModel = list[index];

                                if (generalSettingsModel.browserType == 2)
                                {
                                    bool result = await WPPHelper.findChatLongSync(wv, item.number);
                                    if (result == false)
                                    {
                                        result = await WPPHelper.findChatLongSync(wv, item.number);
                                    }
                                }


                                try
                                {
                                    bool isDOne = await SendMessageSync(mesageModel, isFirstMessage, item, wv);
                                }
                                catch (Exception ex)
                                {
                                    IsError = true;
                                }
                            }
                            else
                            {
                                foreach (MesageModel mesageModel in wASenderSingleTransModel.messages.Where(x => x != null).ToList())
                                {
                                    if (generalSettingsModel.browserType == 2)
                                    {
                                        bool result = await WPPHelper.findChatLongSync(wv, item.number);
                                        if (result == false)
                                        {
                                            result = await WPPHelper.findChatLongSync(wv, item.number);
                                        }
                                    }


                                    //await Task.Delay(500);
                                    try
                                    {
                                        bool isDOne = await SendMessageSync(mesageModel, isFirstMessage, item, wv);
                                    }
                                    catch (Exception ex)
                                    {
                                        IsError = true;
                                    }
                                }
                            }

                            try
                            {
                                counter++;

                                if (wASenderSingleTransModel.settings.delayAfterMessages == counter)
                                {
                                    counter = 0;
                                    await Task.Delay(Utils.getRandom(wASenderSingleTransModel.settings.delayAfterMessagesFrom * 1000, wASenderSingleTransModel.settings.delayAfterMessagesFrom * 1000));
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLog("1324=" + ex.Message);
                            }
                            totalCounter++;

                            if (IsError != true)
                            {
                                var __count = wASenderSingleTransModel.contactList.Count();
                                var _percentage = totalCounter * 100 / __count;
                                item.sendStatusModel.isDone = true;
                                item.sendStatusModel.sendStatusEnum = SendStatusEnum.Success;
                                //worker.ReportProgress(_percentage);
                            }
                            else
                            {
                                var __count = wASenderSingleTransModel.contactList.Count();
                                var _percentage = totalCounter * 100 / __count;
                                item.sendStatusModel.isDone = true;
                                item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                                //worker.ReportProgress(_percentage);
                            }
                        }

                        updateGrid();
                    }
                }
                catch (Exception ex)
                {
                    logger.WriteLog("1339=" + ex.Message);
                }
            }

            return true;
        }

        private void updateGrid()
        {
            foreach (var item in wASenderSingleTransModel.contactList)
            {
                if (item.sendStatusModel.isDone == true && item.logged == false)
                {
                    var globalCounter = gridStatus.Rows.Count - 1;
                    gridStatus.Rows.Add();
                    gridStatus.Rows[globalCounter].Cells[0].Value = item.number;
                    gridStatus.Rows[globalCounter].Cells[1].Value = item.sendStatusModel.sendStatusEnum + (item.isFriendly ? (" - " + Strings.FriendlyNumber) : "");

                    gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
                    item.logged = true;
                }
            }
            int total= wASenderSingleTransModel.contactList.Count();
            int Completed = wASenderSingleTransModel.contactList.Where(x => x.sendStatusModel.isDone == true).Count();

            var value = ((double)Completed / total) * 100;
            var percentage = Convert.ToInt32(Math.Round(value, 0));

            lblPersentage.Text = percentage.ToString() + "% " + Strings.Completed + " -- [" + Completed + "/" + total + "]";

        }



        [STAThread]
        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;
            int totalCounter = 0;
            logger.WriteLog("Starting");
            if (Config.SendingType == 0)
            {

            }
            else if (Config.SendingType == 1)
            {

                try
                {
                    logger.WriteLog("Checking IsWAPIInjected");
                    bool isFirstMessage = true;

                    if (generalSettingsModel.browserType == 2)
                    {
                        int accountSwitchNumber = wASenderSingleTransModel.swipeAccountAfterMessages;
                        int accountSwitchCounter = 0;
                        int MinimumNumber = 0;
                        ConnectedAccountModel assignableAccount;
                        foreach (var contact in wASenderSingleTransModel.contactList)
                        {
                            MinimumNumber = wASenderSingleTransModel.selectedAccounts.Min(x => x.usedCount);
                            assignableAccount = wASenderSingleTransModel.selectedAccounts.OrderBy(x => Convert.ToInt32(x.ID)).Where(x => x.usedCount == MinimumNumber).FirstOrDefault();

                            if (accountSwitchCounter < accountSwitchNumber)
                            {
                                contact.senderName = assignableAccount.sessionName;
                                contact.senderId = assignableAccount.ID;
                                accountSwitchCounter++;
                            }
                            else
                            {
                                MinimumNumber = 0;
                                assignableAccount.usedCount = assignableAccount.usedCount + 1;

                                MinimumNumber = wASenderSingleTransModel.selectedAccounts.Min(x => x.usedCount);
                                assignableAccount = wASenderSingleTransModel.selectedAccounts.OrderBy(x => Convert.ToInt32(x.ID)).Where(x => x.usedCount == MinimumNumber).FirstOrDefault();
                                contact.senderName = assignableAccount.sessionName;
                                contact.senderId = assignableAccount.ID;
                                accountSwitchCounter = 1;
                            }
                        }
                    }

                    List<string> blockList = Config.getAllBlockListes();
                    bool isFirst = true;
                    foreach (var item in wASenderSingleTransModel.contactList)
                    {
                        bool isBlockListed = false;

                        if (IsPaused)
                        {
                            while (IsPaused) ;
                        }
                        if (IsStopped)
                        {
                            return;
                        }

                        WebView2 wv = new WebView2();
                        MainUC mainUC = null;

                        if (generalSettingsModel.browserType == 1)
                        {
                            try
                            {
                                if (!WAPIHelper.IsWAPIInjected(driver))
                                {
                                    ProjectCommon.injectWapi(driver);
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLog("in Each-" + ex.Message);
                            }
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            try
                            {
                                browser.Invoke((MethodInvoker)delegate
                                {
                                    wv = Utils.GetWebViewById(browser, item.senderId);
                                    TabPage tp = Utils.GetTabPageById(browser, item.senderId);
                                    if (tp != null)
                                    {
                                        browser.tabControl1.SelectedTab = tp;
                                    }
                                });

                                browser.Invoke((MethodInvoker)delegate
                                {
                                    mainUC = Utils.GetMainUCId(browser, item.senderId);
                                });


                                if (!await WPPHelper.isWPPinjectedAsync(wv))
                                {
                                    if (mainUC._isWPPIJected == false)
                                    {
                                        await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                                        mainUC._isWPPIJected = true;
                                        Thread.Sleep(1000);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }


                        bool isNumberValid = false;
                        if (generalSettingsModel.filterNumbersBeforeSendingMessage == false || isSkippedNumberCHecking == true)
                        {
                            isNumberValid = true;
                        }
                        else
                        {
                            isNumberValid = item.sendStatusModel.sendStatusEnum == SendStatusEnum.Available ? true : false;
                        }
                        bool checkNumberststus = true;
                        try
                        {
                            item.number = item.number.Replace("+", "");
                            item.number = item.number.Replace(" ", "");
                        }
                        catch (Exception ex)
                        {

                        }
                        if (blockList.Where(z => z == item.number).Count() > 0)
                        {
                            isBlockListed = true;
                        }

                        try
                        {

                            if (item.number.EndsWith("@uid"))
                            {
                                checkNumberststus = false;
                                isNumberValid = true;
                            }

                            if (item.number.StartsWith("55"))
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    item.number = WAPIHelper.queryExists(driver, item.number);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    item.number = WPPHelper.queryExists(wv, item.number);
                                }


                            }
                            if (item.number.StartsWith("225"))
                            {
                                if (generalSettingsModel.browserType == 1)
                                {
                                    item.number = WAPIHelper.queryExists(driver, item.number);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    item.number = WPPHelper.queryExists(wv, item.number);
                                }


                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        bool IsAvailableinchat = true;

                        if (isNumberValid == true && wASenderSingleTransModel.isSafeMode == true && isBlockListed == false)
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                IsAvailableinchat = WAPIHelper.getChat(driver, item.number);
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                IsAvailableinchat = WPPHelper.getChat(wv, item.number);
                            }
                        }

                        if (isBlockListed == true)
                        {
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.BlockListed_Ignored;
                            counter++;
                            totalCounter++;
                            var _count = wASenderSingleTransModel.contactList.Count();
                            var percentage = totalCounter * 100 / _count;
                            worker.ReportProgress(percentage);
                        }
                        else if (IsAvailableinchat == false)
                        {
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.SafeMode_Ignored;
                            counter++;
                            totalCounter++;

                            var _count = wASenderSingleTransModel.contactList.Count();
                            var percentage = totalCounter * 100 / _count;
                            worker.ReportProgress(percentage);
                        }
                        else if (!isNumberValid)
                        {
                            item.sendStatusModel.isDone = true;

                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.ContactNotFound;
                            counter++;
                            totalCounter++;

                            var _count = wASenderSingleTransModel.contactList.Count();
                            var percentage = totalCounter * 100 / _count;
                            worker.ReportProgress(percentage);
                        }
                        else
                        {
                            bool IsError = false;
                            if (wASenderSingleTransModel.IsRotateMessages)
                            {
                                var random = new Random();
                                var list = wASenderSingleTransModel.messages.Where(x => x != null).ToList();
                                int index = random.Next(list.Count);
                                MesageModel mesageModel = list[index];

                                if (generalSettingsModel.browserType == 2)
                                {
                                    bool result = WPPHelper.findChatLong(wv, item.number);
                                    if (result == false)
                                    {
                                        WPPHelper.findChatLong(wv, item.number);
                                    }
                                }

                                try
                                {
                                    SendMessage(mesageModel, isFirstMessage, item, wv);
                                }
                                catch (Exception ex)
                                {
                                    IsError = true;
                                }
                            }
                            else
                            {
                                foreach (MesageModel mesageModel in wASenderSingleTransModel.messages.Where(x => x != null).ToList())
                                {

                                    if (generalSettingsModel.browserType == 2)
                                    {
                                        bool result = WPPHelper.findChatLong(wv, item.number);
                                        if (result == false)
                                        {
                                            WPPHelper.findChatLong(wv, item.number);
                                        }
                                    }



                                    try
                                    {
                                        SendMessage(mesageModel, isFirstMessage, item, wv);
                                    }
                                    catch (Exception ex)
                                    {
                                        IsError = true;
                                    }
                                }
                            }

                            try
                            {
                                counter++;

                                if (wASenderSingleTransModel.settings.delayAfterMessages == counter)
                                {
                                    counter = 0;
                                    Thread.Sleep(Utils.getRandom(wASenderSingleTransModel.settings.delayAfterMessagesFrom * 1000, wASenderSingleTransModel.settings.delayAfterMessagesFrom * 1000));
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.WriteLog("1324=" + ex.Message);
                            }
                            totalCounter++;

                            if (IsError != true)
                            {
                                var __count = wASenderSingleTransModel.contactList.Count();
                                var _percentage = totalCounter * 100 / __count;
                                item.sendStatusModel.isDone = true;
                                item.sendStatusModel.sendStatusEnum = SendStatusEnum.Success;
                                worker.ReportProgress(_percentage);
                            }
                            else
                            {
                                var __count = wASenderSingleTransModel.contactList.Count();
                                var _percentage = totalCounter * 100 / __count;
                                item.sendStatusModel.isDone = true;
                                item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                                worker.ReportProgress(_percentage);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.WriteLog("1339=" + ex.Message);
                }
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            campaign_completed();
        }

        private void campaign_completed()
        {
            ChangeCampStatus(CampaignStatusEnum.Finish);
            stopProgressbar();
            string report = AutomationCommon.GenerateReport(this.wASenderSingleTransModel);
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                string NavigateJS = "";
                NavigateJS += report;
                string _newFileName = Guid.NewGuid().ToString() + ".html";
                string tmpfile = Config.GetTempFolderPath() + "\\" + _newFileName;

                File.Create(tmpfile).Dispose();

                using (TextWriter tw = new StreamWriter(tmpfile))
                {
                    tw.WriteLine(report);
                }
                System.Diagnostics.Process.Start(tmpfile);
            }
            catch (Exception ex)
            {
                string ss = "";
            }

            if (schedulesModel != null)
            {
                string jsonString = JsonConvert.SerializeObject(this.wASenderSingleTransModel);
                new SqLiteBaseRepository().UpdateScheduleCompleted(schedulesModel.Id, jsonString, report);
                this.waSenderForm.checkForPendingSchedules(true);
                this.Close();
                if (forceScheduleRun == false)
                {

                    pgbar = new Progressbar();
                    pgbar.Show();
                    pgbar.materialLabel1.Text = Strings.PleaseWait;
                    Thread.Sleep(1000);
                    pgbar.Close();
                    this.waSenderForm.Close();
                }
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                lblPersentage.Text = e.ProgressPercentage + "% " + Strings.Completed;
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSTart_Click(object sender, EventArgs e)
        {
            if (generalSettingsModel.filterNumbersBeforeSendingMessage == true)
            {
                oppUpFilter();
            }
            else
            {
                btnStartClick();
            }

        }

        private void oppUpFilter()
        {
            QuickFilter q = new QuickFilter(wASenderSingleTransModel, this);
            q.Show();
        }

        public void returnFromQuickFilter(WASenderSingleTransModel _wASenderSingleTransModel, bool isSkiped = false)
        {
            string jsString = Newtonsoft.Json.JsonConvert.SerializeObject(_wASenderSingleTransModel);
            this.wASenderSingleTransModel = Newtonsoft.Json.JsonConvert.DeserializeObject<WASenderSingleTransModel>(jsString);
            isSkippedNumberCHecking = isSkiped;
            btnStartClick();
        }

        private async void btnStartClick()
        {
            logger.WriteLog("CLicked");
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum == CampaignStatusEnum.Finish)
            {
                gridStatus.Rows.Clear();
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running && campaignStatusEnum != CampaignStatusEnum.Paused)
            {
                try
                {
                    IsStopped = false;
                    logger.WriteLog("worker.RunWorkerAsync");
                    // worker.RunWorkerAsync();
                    ChangeCampStatus(CampaignStatusEnum.Running);
                    startProgressBar();
                    //initTimer();

                    bool isDOne = await doStartWork();

                    campaign_completed();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (campaignStatusEnum == CampaignStatusEnum.Paused)
            {
                IsPaused = false;
                ChangeCampStatus(CampaignStatusEnum.Running);
                startProgressBar();
            }
            else
            {
                Utils.showAlert(Strings.Processisalreadyrunning, Alerts.Alert.enmType.Info);
            }
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
            try
            {
                foreach (var item in wASenderSingleTransModel.contactList)
                {
                    if (item.sendStatusModel.isDone == true && item.logged == false)
                    {


                        var globalCounter = gridStatus.Rows.Count - 1;
                        gridStatus.Rows.Add();
                        gridStatus.Rows[globalCounter].Cells[0].Value = item.number;
                        gridStatus.Rows[globalCounter].Cells[1].Value = item.sendStatusModel.sendStatusEnum + (item.isFriendly ? (" - " + Strings.FriendlyNumber) : "");

                        gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
                        item.logged = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        private void startProgressBar()
        {
            progressBar1.Show();
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
        }
        private void stopProgressbar()
        {
            progressBar1.Hide();
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.MarqueeAnimationSpeed = 0;
        }
        private void materialButton1_Click(object sender, EventArgs e)
        {
            IsPaused = true;
            ChangeCampStatus(CampaignStatusEnum.Paused);
            stopProgressbar();
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            stopProgressbar();
            IsStopped = true;
            IsPaused = false;
        }
    }
}
