using MaterialSkin.Controls;
using Microsoft.Web.WebView2.WinForms;
using Models;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaSender.Models;
using WASender.enums;
using WASender.Models;
using System.IO;
using Newtonsoft.Json;
namespace WASender
{
    public partial class GetPollResults : MaterialForm
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
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        Progressbar pgbar;

        public GetPollResults(WaSenderForm _waSenderForm)
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

        int retryAttempt = 0;
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

        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }
        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            this.campaignStatusEnum = _campaignStatus;
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
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
                        Thread.Sleep(1000);

                    }

                }

            }

        }

       
        void _testClass_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            ChangeInitStatus(InitStatusEnum.Stopped);
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
                    else
                    {
                        MessageBox.Show(eex.Message, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void GetPollResults_Load(object sender, EventArgs e)
        {
            initLanguages();
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        private void initLanguages()
        {
            this.Text = Strings.GetPollResults;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourmobile;
            materialLabel1.Text = Strings.OpenanyGroupchatClickbuttonbellow;
            btnInitWA.Text = Strings.ClicktoInitiate;
            materialButton1.Text = Strings.StartGrabbing;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            label1.Text = Strings.Result;
            materialButton2.Text = Strings.Export;
            lblInitStatus.Text = Strings.NotInitialised;
            lblRunStatus.Text = Strings.NotInitialised;
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

        private async void materialButton1_Click(object sender, EventArgs e)
        {
            webBrowser1.DocumentText = "";

            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                logger.WriteLog("!InitStatusEnum.Initialised");
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {
                ChangeCampStatus(CampaignStatusEnum.Running);
                startProgressBar();


                bool isDOne = await BackgroundProcessLogicMethod();

                stopProgressbar();
                ChangeCampStatus(CampaignStatusEnum.Finish);
                materialButton2.Enabled = true;
                //this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
                //this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
                //this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
                //try
                //{
                //    backgroundWorker1.RunWorkerAsync();
                //}
                //catch (Exception ex)
                //{

                //}
            }
            else
            {
                Utils.showAlert(Strings.Processisalreadyrunning, Alerts.Alert.enmType.Info);
            }


        }

        private async Task<bool> BackgroundProcessLogicMethod()
        {

            List<PollReportModel> pollReportModelList = new List<global::Models.PollReportModel>();
            if (generalSettingsModel.browserType == 1)
            {
                if (!WAPIHelper.IsWAPIInjected(driver))
                {
                    ProjectCommon.injectWapi(driver);
                }
                await Task.Delay(500);
                pollReportModelList = WAPIHelper.GetPollResults(driver);
            }
            else if (generalSettingsModel.browserType == 2)
            {
                WebView2 wv = new WebView2();
                wv = wv = Utils.GetActiveWebView(browser);



                if (await WPPHelper.isWPPinjected(wv))
                {

                }
                else
                {
                    await WPPHelper.InjectWapiSync(wv, Config.GetSysFolderPath());
                    Thread.Sleep(1000);
                }

                try
                {
                    pollReportModelList = await WPPHelper.GetPollResults(wv);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.InnerException != null ? ex.InnerException.Message : "", Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            string strim = "";
            strim += "<table style='width:100%;border:1px solid black;' border='1' >";
            strim += "  <thead>";
            strim += "      <tr>";
            strim += "          <th style='border:1px solid black;'>";
            strim += "          " + Strings.PollName;
            strim += "          </th>";
            strim += "          <th style='border:1px solid black;'>";
            strim += "          " + Strings.PollOptions;
            strim += "          </th>";
            strim += "          <th style='border:1px solid black;'>";
            strim += "          " + Strings.NonReactedNumbers;
            strim += "          </th>";
            strim += "      </tr>";
            strim += "  </thead>";
            strim += "  <tbody>";

            foreach (var poll in pollReportModelList)
            {



                strim += "      <tr>";
                strim += "          <td style='border:1px solid black;'>";
                strim += "      " + poll.PollName;
                strim += "          </td>";

                strim += "          <td style='border:1px solid black;'>";

                strim += "<table border='1' style='width:100%;border:1px solid black;'>";

                strim += "  <thead>";
                strim += "      <tr>";
                strim += "          <th style='border:1px solid black;'>";
                strim += "          " + Strings.Option;
                strim += "          </th>";
                strim += "          <th style='border:1px solid black;'>";
                strim += "          " + Strings.VotedNumbers;
                strim += "          </th>";

                strim += "      </tr>";
                strim += "  </thead>";
                strim += "  <tbody>";
                foreach (var option in poll.options)
                {

                    strim += "      <tr>";
                    strim += "          <td style='width:50%;border:1px solid black;'>";
                    strim += "      " + option.name + "<br>";
                    strim += "          </td>";
                    strim += "          <td style='border:1px solid black;'>";

                    foreach (var selecteds in option.selectedVotesList)
                    {
                        strim += "      " + selecteds.sender.Replace("@c.us", "") + "<br>";
                    }

                    strim += "          </td>";
                    strim += "      </tr>";
                }
                strim += "  </tbody>";
                strim += "</table>";
                strim += "          </td>";
                strim += "      <td>";


                foreach (var item in poll.unreactedList)
                {
                    strim += item.chatId.Replace("@c.us", "") + "<br>";
                }

                strim += "      </td>";
                strim += "      </tr>";
            }
            strim += "      ";
            strim += "";
            strim += "";
            strim += "";
            strim += "  <tbody>";
            strim += "";
            strim += "";
            strim += "";
            strim += "";
            strim += "";
            strim += "</table>";

            webBrowser1.DocumentText = strim;

            return true;
        }

        private int BackgroundProcessLogicMethod(BackgroundWorker bw)
        {
            List<PollReportModel> pollReportModelList = new List<global::Models.PollReportModel>();
            if (generalSettingsModel.browserType == 1)
            {
                if (!WAPIHelper.IsWAPIInjected(driver))
                {
                    ProjectCommon.injectWapi(driver);
                }
                Thread.Sleep(500);
                pollReportModelList = WAPIHelper.GetPollResults(driver);
            }
            else if (generalSettingsModel.browserType == 2)
            {
                WebView2 wv=new WebView2();
                browser.Invoke((MethodInvoker)delegate
                {
                    wv =wv = Utils.GetActiveWebView(browser);
                    
                });

                

                if (!WPPHelper.isWPPinjected(wv).Result)
                {
                    WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                    Thread.Sleep(1000);
                }

                try
                {
                    pollReportModelList = WPPHelper.GetPollResults(wv).Result;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.InnerException !=null?ex.InnerException.Message :"",Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            string strim = "";
            strim += "<table style='width:100%;border:1px solid black;' border='1' >";
            strim += "  <thead>";
            strim += "      <tr>";
            strim += "          <th style='border:1px solid black;'>";
            strim += "          " + Strings.PollName;
            strim += "          </th>";
            strim += "          <th style='border:1px solid black;'>";
            strim += "          " + Strings.PollOptions;
            strim += "          </th>";
            strim += "          <th style='border:1px solid black;'>";
            strim += "          " + Strings.NonReactedNumbers;
            strim += "          </th>";
            strim += "      </tr>";
            strim += "  </thead>";
            strim += "  <tbody>";

            foreach (var poll in pollReportModelList)
            {



                strim += "      <tr>";
                strim += "          <td style='border:1px solid black;'>";
                strim += "      " + poll.PollName;
                strim += "          </td>";

                strim += "          <td style='border:1px solid black;'>";

                strim += "<table border='1' style='width:100%;border:1px solid black;'>";

                strim += "  <thead>";
                strim += "      <tr>";
                strim += "          <th style='border:1px solid black;'>";
                strim += "          " + Strings.Option;
                strim += "          </th>";
                strim += "          <th style='border:1px solid black;'>";
                strim += "          " + Strings.VotedNumbers;
                strim += "          </th>";

                strim += "      </tr>";
                strim += "  </thead>";
                strim += "  <tbody>";
                foreach (var option in poll.options)
                {

                    strim += "      <tr>";
                    strim += "          <td style='width:50%;border:1px solid black;'>";
                    strim += "      " + option.name + "<br>";
                    strim += "          </td>";
                    strim += "          <td style='border:1px solid black;'>";

                    foreach (var selecteds in option.selectedVotesList)
                    {
                        strim += "      " + selecteds.sender.Replace("@c.us", "") + "<br>";
                    }

                    strim += "          </td>";
                    strim += "      </tr>";
                }
                strim += "  </tbody>";
                strim += "</table>";
                strim += "          </td>";
                strim += "      <td>";


                foreach (var item in poll.unreactedList)
                {
                    strim += item.chatId.Replace("@c.us", "") + "<br>";
                }

                strim += "      </td>";
                strim += "      </tr>";
            }
            strim += "      ";
            strim += "";
            strim += "";
            strim += "";
            strim += "  <tbody>";
            strim += "";
            strim += "";
            strim += "";
            strim += "";
            strim += "";
            strim += "</table>";

            webBrowser1.DocumentText = strim;

            return 1;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helperBW = sender as BackgroundWorker;
            e.Result = BackgroundProcessLogicMethod(helperBW);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopProgressbar();
            ChangeCampStatus(CampaignStatusEnum.Finish);
            materialButton2.Enabled = true;

        }
        private void startProgressBar()
        {
            progressBar1.Visible = true;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
        }

        private void stopProgressbar()
        {
            progressBar1.Visible = false;
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.MarqueeAnimationSpeed = 0;
        }

        private void GetPollResults_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                worker.CancelAsync();
                worker.Dispose();
            }
            catch (Exception ex)
            {

            }
            waSenderForm.formReturn(true);
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "PollResult_" + Guid.NewGuid().ToString() + ".html");
            string _jtnl = webBrowser1.DocumentText;
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.WriteLine(_jtnl);
                }
            }

            savesampleExceldialog.FileName = "PollResult.html";
            savesampleExceldialog.Filter = "Excel Files (*.html)|*.html";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(file, savesampleExceldialog.FileName.EndsWith(".html") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".html", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }

        }


    }
}
