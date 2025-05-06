using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Web.WebView2.WinForms;
using Models;
using Newtonsoft.Json;
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
using System.Web;
using System.Windows.Forms;
using WASender.enums;
using WASender.Models;

namespace WASender
{
    public partial class RunGroup : MaterialForm
    {
        WASenderGroupTransModel wASenderGroupTransModel;
        WaSenderForm waSenderForm;
        InitStatusEnum initStatusEnum;
        CampaignStatusEnum campaignStatusEnum;
        IWebDriver driver;
        System.Windows.Forms.Timer timerInitChecker;
        System.Windows.Forms.Timer timerRunner;
        BackgroundWorker worker;
        private bool IsStopped = true;
        private bool IsPaused = false;
        Logger logger;
        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;
        SchedulesModel schedulesModel;
        bool forceScheduleRun = false;
        public RunGroup(WASenderGroupTransModel _wASenderGroupTransModel, WaSenderForm _waSenderForm, SchedulesModel _schedulesModel = null, bool _forceScheduleRun = false)
        {
            logger = new Logger("RunGroup");
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.waSenderForm = _waSenderForm;
            this.wASenderGroupTransModel = _wASenderGroupTransModel;
            this.Text = _wASenderGroupTransModel.CampaignName;
            forceScheduleRun = _forceScheduleRun;
            if (_schedulesModel == null)
            {
                generalSettingsModel = Config.GetSettings();
            }
            else
            {
                generalSettingsModel = _wASenderGroupTransModel.generalSettingsModel;
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
                if (schedulesModel == null)
                {
                    browser = new WaSenderBrowser();
                    Utils.waSenderBrowser = browser;
                    browser.Show();
                }
                else
                {
                    browser = new WaSenderBrowser(wASenderGroupTransModel.sessionId);
                    Utils.waSenderBrowser = browser;
                    browser.Show();
                }

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
        private void RunGroup_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                IsStopped = true;
                worker.CancelAsync();
                worker.Dispose();
            }
            catch (Exception ex)
            {

            }
            waSenderForm.formReturn(true);
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
                    WebView2 vw = Utils.GetActiveWebView(browser);
                    bool isInitiated = await WPPHelper.isWaInited(vw);
                    if (isInitiated)
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


        private void SendMessagetoGroup(MesageModel mesageModel, GroupModel item, WebView2 wv, bool isFirstMessage)
        {
            bool AutoSend = false;
            bool isAlreadySent = false;
            if (mesageModel.buttons != null && mesageModel.buttons.Count() > 0)
            {
                AutoSend = true;
            }

            isFirstMessage = false;

            Thread.Sleep(500);

            //var messages = mesageModel.longMessage.Split('\n');
            string NewMessage = ProjectCommon.ReplaceKeyMarker(mesageModel.longMessage);



            foreach (var file in mesageModel.files)
            {
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
                file.fileName = FileName;
                if (ext == ".mp4")
                {
                    try
                    {

                        if (file.Caption != null)
                        {
                            file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption);
                        }

                        var rand = Utils.getRandom(500, 1000);

                        if (generalSettingsModel.browserType == 1)
                        {
                            WAPIHelper.markIsComposing(driver, item.GroupId, rand);
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            WPPHelper.markIsComposing(wv, item.GroupId, rand);
                        }



                        if (file.attachWithMainMessage == true)
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                string outParam = "";
                                WAPIHelper.SendVideoToGroup(driver, item.GroupId, fullBase64, NewMessage, file.fileName, out outParam, wASenderGroupTransModel.tagAll);
                                item.reason = outParam;
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendVideoToGroup2(wv, item.GroupId, fullBase64, NewMessage, file.fileName, wASenderGroupTransModel.tagAll);
                            }

                            AutoSend = false;
                        }
                        else
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                string outParam = "";
                                WAPIHelper.SendVideoToGroup(driver, item.GroupId, fullBase64, file.Caption, "", out outParam, wASenderGroupTransModel.tagAll);
                                item.reason = outParam;
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendVideoToGroup2(wv, item.GroupId, fullBase64, file.Caption, file.fileName, wASenderGroupTransModel.tagAll);
                            }

                        }


                    }
                    catch (Exception)
                    { }
                }
                else if (ext == ".ogg")
                {
                    try
                    {

                        if (file.Caption != null)
                        {
                            file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption);
                        }

                        var rand = Utils.getRandom(500, 1000);
                        WAPIHelper.markIsComposing(driver, item.GroupId, rand);

                        fullBase64 = fullBase64.Replace("data:application/octet-stream;", "data:audio/mp3;");

                        if (file.attachWithMainMessage == true)
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                string outParam = "";
                                WAPIHelper.SendVideoToGroup(driver, item.GroupId, fullBase64, NewMessage, "", out outParam, wASenderGroupTransModel.tagAll);
                                item.reason = outParam;
                            }
                            if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendVideoToGroup2(wv, item.GroupId, fullBase64, NewMessage, null, wASenderGroupTransModel.tagAll);
                            }

                            AutoSend = false;
                        }
                        else
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                string outParam = "";
                                WAPIHelper.SendVideoToGroup(driver, item.GroupId, fullBase64, file.Caption, "", out outParam, wASenderGroupTransModel.tagAll);
                                item.reason = outParam;
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendVideoToGroup2(wv, item.GroupId, fullBase64, file.Caption, null, wASenderGroupTransModel.tagAll);
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

                    if (file.Caption != null)
                    {
                        file.Caption = ProjectCommon.ReplaceKeyMarker(file.Caption);
                    }

                    var rand = Utils.getRandom(500, 1000);
                    if (generalSettingsModel.browserType == 1)
                    {
                        WAPIHelper.markIsComposing(driver, item.GroupId, rand);
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        WPPHelper.markIsComposing(wv, item.GroupId, rand);
                    }


                    if (file.attachWithMainMessage == true)
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            string outParam = "";
                            WAPIHelper.SendVideoToGroup(driver, item.GroupId, fullBase64, NewMessage, file.fileName, out outParam, wASenderGroupTransModel.tagAll);
                            item.reason = outParam;
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            WPPHelper.SendVideoToGroup2(wv, item.GroupId, fullBase64, NewMessage, file.fileName, wASenderGroupTransModel.tagAll);
                        }

                        AutoSend = false;
                        isAlreadySent = true;
                    }
                    else
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            string outParam = "";
                            WAPIHelper.SendAttachmentToGroup(driver, item.GroupId, fullBase64, FileName, file.Caption, out outParam); ////REMIND HERE
                            item.reason = outParam;
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            WPPHelper.SendAttachmentToGroup(wv, item.GroupId, fullBase64, FileName, file.Caption);////REMIND HERE
                        }

                    }

                }

                if (isFirstMessage == true)
                {
                    Thread.Sleep(2000);
                }
                Thread.Sleep(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderGroupTransModel.settings.delayAfterEveryMessageTo * 1000));
            }


            if (AutoSend == false)
            {
                if (isAlreadySent == false)
                {
                    if (NewMessage != "")
                    {
                        var rand = Utils.getRandom(500, 1000);
                        WAPIHelper.markIsComposing(driver, item.GroupId, rand);
                        if (NewMessage.Contains("http://") || NewMessage.Contains("https://"))
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                try
                                {
                                    WAPIHelper.sendTextMessageWithPreview(driver, item.GroupId, NewMessage, true); /// REMIND
                                }
                                catch (Exception ex)
                                {
                                    WAPIHelper.SendMessage(driver, item.GroupId, NewMessage, true); /// REMIND
                                }
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                try
                                {
                                    WPPHelper.sendTextMessageWithPreview(wv, item.GroupId, NewMessage, true, wASenderGroupTransModel.tagAll);
                                }
                                catch (Exception ex)
                                {
                                    WPPHelper.SendMessage(wv, item.GroupId, NewMessage, true, wASenderGroupTransModel.tagAll);
                                }
                            }

                        }
                        else
                        {
                            if (generalSettingsModel.browserType == 1)
                            {
                                WAPIHelper.SendMessage(driver, item.GroupId, NewMessage, true);
                            }
                            else if (generalSettingsModel.browserType == 2)
                            {
                                WPPHelper.SendMessage(wv, item.GroupId, NewMessage, true, wASenderGroupTransModel.tagAll);
                            }

                        }
                    }


                }
            }
            else
            {
                var rand = Utils.getRandom(500, 1000);

                WAPIHelper.markIsComposing(driver, item.GroupId, rand);
                WAPIHelper.sendButtonWithMessageToGroup(driver, mesageModel, item.GroupId, NewMessage);
            }

            if (mesageModel.polls != null && mesageModel.polls.Count() > 0)
            {
                foreach (var poll in mesageModel.polls)
                {
                    try
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            WAPIHelper.sendCreatePollMessageToGroup(driver, item.GroupId, poll);
                            Thread.Sleep(500);
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            WPPHelper.sendCreatePollMessageToGroup(wv, item.GroupId, poll);
                            Thread.Sleep(500);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }





        }

        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;
            int totalCounter = 0;
            string BaseMessageId = null;

            if (Config.SendingType == 0)
            {

            }
            else if (Config.SendingType == 1)
            {
                bool isFirstMessage = true;

                WebView2 wv = new WebView2();

                if (generalSettingsModel.browserType == 1)
                {
                    if (!WAPIHelper.IsWAPIInjected(driver))
                    {
                        ProjectCommon.injectWapi(driver);
                    }
                }
                else if (generalSettingsModel.browserType == 2)
                {
                    try
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
                    }
                    catch (Exception ex)
                    {

                    }
                }



                foreach (var item in wASenderGroupTransModel.groupList)
                {
                    if (IsPaused)
                    {
                        while (IsPaused) ;
                    }
                    if (IsStopped)
                    {
                        return;
                    }

                    if (generalSettingsModel.browserType == 1)
                    {
                        if (!WAPIHelper.IsWAPIInjected(driver))
                        {
                            ProjectCommon.injectWapi(driver);
                        }
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        try
                        {
                            browser.Invoke((MethodInvoker)delegate
                            {
                                wv = Utils.GetActiveWebView(browser);
                            });


                            //if (!await WPPHelper.isWPPinjectedAsync(wv))
                            //{
                            //    await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                            //    Thread.Sleep(1000);
                            //}
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    bool canSend = false;
                    if (item.CanSend == null)
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            canSend = WAPIHelper.GetGroup__x_canSend(driver, item.GroupId);
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            canSend = WPPHelper.GetGroup__x_canSend(wv, item.GroupId);
                        }
                    }
                    else
                    {
                        canSend =(bool) item.CanSend;
                    }




                    if (!canSend)
                    {
                        item.sendStatusModel.isDone = true;
                        item.sendStatusModel.sendStatusEnum = SendStatusEnum.CantSend_Group_Admin_Only_Or_Removed;
                        counter++;
                        totalCounter++;

                        var _count = wASenderGroupTransModel.groupList.Count();
                        var percentage = totalCounter * 100 / _count;
                        worker.ReportProgress(percentage);
                    }
                    else
                    {

                        if (wASenderGroupTransModel.IsRotateMessages == true)
                        {
                            var random = new Random();
                            var list = wASenderGroupTransModel.messages.Where(x => x != null).ToList();
                            int index = random.Next(list.Count);
                            MesageModel mesageModel = list[index];
                            try
                            {
                                SendMessagetoGroup(mesageModel, item, wv, isFirstMessage);
                                isFirstMessage = false;
                            }
                            catch (Exception ex)
                            {

                            }

                            Thread.Sleep(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderGroupTransModel.settings.delayAfterEveryMessageTo * 1000));
                            try
                            {
                                counter++;
                                if (wASenderGroupTransModel.settings.delayAfterMessages == counter)
                                {
                                    counter = 0;
                                    Thread.Sleep(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000, wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000));
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                            totalCounter++;

                            var __count = wASenderGroupTransModel.groupList.Count();
                            var _percentage = totalCounter * 100 / __count;
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.Success;
                            worker.ReportProgress(_percentage);
                        }

                        else
                        {
                            foreach (MesageModel mesageModel in wASenderGroupTransModel.messages.Where(x => x != null).ToList())
                            {
                                try
                                {
                                    SendMessagetoGroup(mesageModel, item, wv, isFirstMessage);
                                    isFirstMessage = false;
                                }
                                catch (Exception ex)
                                {

                                }

                                Thread.Sleep(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderGroupTransModel.settings.delayAfterEveryMessageTo * 1000));
                                try
                                {
                                    counter++;
                                    if (wASenderGroupTransModel.settings.delayAfterMessages == counter)
                                    {
                                        counter = 0;
                                        Thread.Sleep(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000, wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000));
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                totalCounter++;

                                var __count = wASenderGroupTransModel.groupList.Count();
                                var _percentage = totalCounter * 100 / __count;
                                item.sendStatusModel.isDone = true;
                                item.sendStatusModel.sendStatusEnum = SendStatusEnum.Success;
                                worker.ReportProgress(_percentage);
                            }
                        }


                    }
                }
            }

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChangeCampStatus(CampaignStatusEnum.Finish);
            stopProgressbar();
            string report = AutomationCommon.GenerateReport(this.wASenderGroupTransModel);
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                string NavigateJS = "";
                NavigateJS += report;
                string _newFileName = Guid.NewGuid().ToString() + ".html";
                string tmpfile = Config.GetTempFolderPath() + "\\" + _newFileName;

                using (FileStream fs = File.Create(tmpfile))
                {
                    byte[] author = new UTF8Encoding(true).GetBytes(NavigateJS);
                    fs.Write(author, 0, NavigateJS.Length);
                }
                System.Diagnostics.Process.Start(tmpfile);
            }
            catch (Exception ex)
            {
                string ss = "";
            }
            if (e.Error != null)
            {
                logger.WriteLog(e.Error.Message);
            }

            if (schedulesModel != null)
            {
                string jsonString = JsonConvert.SerializeObject(this.wASenderGroupTransModel);
                new SqLiteBaseRepository().UpdateScheduleCompleted(schedulesModel.Id, jsonString, report);
                this.waSenderForm.checkForPendingSchedules(true);
                this.Close();
                if (forceScheduleRun == false)
                {
                    this.waSenderForm.Close();
                }
            }


        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblPersentage.Text = e.ProgressPercentage + "% " + Strings.Completed;
        }

        private async void semulate()
        {
            if (schedulesModel != null)
            {
                await Task.Delay(2000);
                initBaseWA();
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
        private void btnSTart_Click(object sender, EventArgs e)
        {
            btnStartClick();
        }


        private async void startAutoRun()
        {
            if (schedulesModel != null)
            {
                await Task.Delay(2000);
                btnStartClick();
            }

        }

        private void btnStartClick()
        {
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

                IsStopped = false;
                worker.RunWorkerAsync();
                ChangeCampStatus(CampaignStatusEnum.Running);
                startProgressBar();
                initTimer();
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
                int i = 1;
                foreach (var item in wASenderGroupTransModel.groupList)
                {
                    if (item.sendStatusModel.isDone == true && item.logged == false)
                    {
                        var globalCounter = gridStatus.Rows.Count - 1;
                        gridStatus.Rows.Add();
                        gridStatus.Rows[globalCounter].Cells[0].Value = item.Name;
                        gridStatus.Rows[globalCounter].Cells[1].Value = item.sendStatusModel.sendStatusEnum;

                        gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
                        item.logged = true;

                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void RunGroup_Load(object sender, EventArgs e)
        {
            InitLanguage();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                this.Invoke(new Action(() =>
                    CheckForActivation()));
            });


            semulate();
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
            this.Text = Strings.RunGroup;
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
            label2.Text = Strings.ClearWhatsAppchathistoryafter5001000150020000messagesasperyourphoneconfiguration;
            label3.Text = Strings.WaSendertendstosubmitmessagestoyourphoneisnotresponsiblefordeliveryofthemessage;
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
