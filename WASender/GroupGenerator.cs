using MaterialSkin.Controls;
using Microsoft.Web.WebView2.WinForms;
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
using WaAutoReplyBot;
using WASender.enums;
using WASender.Models;

namespace WASender
{
    public partial class GroupGenerator : MyMaterialPopOp
    {
        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        WaSenderForm waSenderForm;
        CampaignStatusEnum campaignStatusEnum;
        System.Windows.Forms.Timer timerRunner;
        Logger logger;

        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public GroupGenerator(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.waSenderForm = _waSenderForm;
            logger = new Logger("GroupGenerator");
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

        private void initWABrowser()
        {
            ChangeInitStatus(InitStatusEnum.Initialising);

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
                this.driver = Utils.Driver;
            }


            try
            {
                if (driver == null)
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

        private void checkQRScanDone()
        {
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
                        logger.WriteLog("isElementDisplayed = true");
                        ChangeInitStatus(InitStatusEnum.Initialised);
                        timerInitChecker.Stop();
                        initBackgroundWorker();
                        Activate();
                    }
                }
                catch (Exception ex)
                {
                    logger.WriteLog(ex.Message);
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
                        initBackgroundWorker();
                        Activate();
                    }
                }
                catch (Exception ex)
                {
                    timerInitChecker.Stop();
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

        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.WriteLog("Started");
            WebView2 wv = new WebView2();

            if (generalSettingsModel.browserType == 1)
            {
                if (!WAPIHelper.IsWAPIInjected(driver))
                {
                    ProjectCommon.injectWapi(driver);
                    Thread.Sleep(1000);
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





            int chunkSeperator = Convert.ToInt32(txtdelayAfterMessages.Text);
            if (chunkSeperator >= groupGeneratorModel.contactList.Count())
            {
                chunkSeperator = groupGeneratorModel.contactList.Count();
            }

            var ss_ = LinqHelpers.Chunk(groupGeneratorModel.contactList, chunkSeperator).ToList();

            foreach (var chunk in ss_)
            {
                List<ContactModel> lisr = chunk.ToList();

                foreach (var item in lisr)
                {

                    if (generalSettingsModel.browserType == 1)
                    {
                        logger.WriteLog("creating " + item.name);
                        try
                        {
                            WAPIHelper.CreateGroup(driver, item.name, groupGeneratorModel.DefaultNumberAdd);
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.Success;
                            item.sendStatusModel.isDone = true;
                        }
                        catch (Exception ex)
                        {
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                            item.sendStatusModel.isDone = true;
                            logger.WriteLog("Error " + ex.Message);
                        }
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        logger.WriteLog("creating " + item.name);
                        try
                        {
                            bool _result = WPPHelper.CreateGroup(wv, item.name, groupGeneratorModel.DefaultNumberAdd);

                            if (_result == true)
                            {
                                item.sendStatusModel.sendStatusEnum = SendStatusEnum.Success;
                                item.sendStatusModel.isDone = true;
                            }
                            else
                            {
                                item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                                item.sendStatusModel.isDone = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                            item.sendStatusModel.isDone = true;
                            logger.WriteLog("Error - " + ex.Message);
                        }
                    }



                    try
                    {
                        int _rand = Utils.getRandom(Convert.ToInt32(txtdelayAfterEveryMessageFrom.Text), Convert.ToInt32(txtdelayAfterEveryMessageTo.Text));
                        Thread.Sleep(TimeSpan.FromSeconds(_rand));
                    }
                    catch (Exception ex)
                    {

                    }
                }

                try
                {
                    int _rand = Utils.getRandom(Convert.ToInt32(txtdelayAfterMessagesFrom.Text), Convert.ToInt32(txtdelayAfterMessagesTo.Text));
                    Thread.Sleep(TimeSpan.FromSeconds(_rand));
                }
                catch (Exception ex)
                {

                }

            }



        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblPersentage.Text = e.ProgressPercentage + "% " + Strings.Completed;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChangeCampStatus(CampaignStatusEnum.Finish);
            stopProgressbar();
        }
        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            logger.WriteLog(_initStatus.ToString());
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }

        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            logger.WriteLog(_campaignStatus.ToString());
            this.campaignStatusEnum = _campaignStatus;
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }
        private void GroupGenerator_Load(object sender, EventArgs e)
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
            this.Text = Strings.BulkGroupGenerator;
            materialLabel1.Text = Strings.GroupNameSettings;
            materialTextBox21.Hint = Strings.GroupNamePrefix;
            materialTextBox22.Hint = Strings.Increment;
            materialTextBox23.Hint = Strings.GroupNameSuffix;
            materialTextBox24.Hint = Strings.DefaultNumberAdd;
            materialTextBox25.Hint = Strings.GenerateTotalGroups;
            materialButton1.Text = Strings.Validate;
            De.Text = Strings.DelaySettings;
            materialLabel3.Text = Strings.Wait;
            materialLabel5.Text = Strings.secondsafterevery;
            materialLabel6.Text = Strings.GroupCreate;
            materialLabel9.Text = Strings.Wait;
            materialLabel7.Text = Strings.secondsbeforeeveryGroupCreate;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourmobile;
            btnInitWA.Text = Strings.ClicktoInitiate;
            btnSTart.Text = Strings.Start;

            gridStatus.Columns[1].HeaderText = Strings.GroupName;
            gridStatus.Columns[2].HeaderText = Strings.Status;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            label8.Text = Strings.Log;
        }
        public void init()
        {
            logger.WriteLog("init");
            ChangeInitStatus(InitStatusEnum.NotInitialised);
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
            materialCheckbox1.Checked = true;
            materialCheckbox2.Checked = true;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            ValidateControl();
        }

        private bool ValidateControl()
        {

            string Message = "";
            bool returnValue = true;
            if (materialTextBox21.Text == "")
            {
                Message = Strings.GroupNamePrefix + " " + Strings.ShouldNotbeempty;
                returnValue = false;
            }
            if (materialTextBox22.Text == "")
            {
                Message = Strings.Increment + " " + Strings.ShouldNotbeempty;
                returnValue = false;
            }

            try
            {
                Convert.ToInt32(materialTextBox22.Text);
            }
            catch (Exception ex)
            {
                Message = Strings.Increment + " " + Strings.IsNotValid;
                returnValue = false;
            }

            if (materialTextBox24.Text == "")
            {
                Message = Strings.DefaultNumberAdd + " " + Strings.ShouldNotbeempty;
                returnValue = false;
            }

            if (materialTextBox25.Text == "")
            {
                Message = Strings.GenerateTotalGroups + " " + Strings.ShouldNotbeempty;
                returnValue = false;
            }

            try
            {
                Convert.ToInt32(materialTextBox25.Text);
            }
            catch (Exception ex)
            {
                Message = Strings.GenerateTotalGroups + " " + Strings.IsNotValid;
                returnValue = false;
            }
            if (Message != "")
            {
                MessageBox.Show(Message, Strings.Errors, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            returnValue = SetFocusIfEmpty(txtdelayAfterMessagesFrom);
            returnValue = SetFocusIfEmpty(txtdelayAfterMessagesTo);
            returnValue = SetFocusIfEmpty(txtdelayAfterMessages);
            returnValue = SetFocusIfEmpty(txtdelayAfterEveryMessageFrom);
            returnValue = SetFocusIfEmpty(txtdelayAfterEveryMessageTo);

            return returnValue;
        }


        private bool SetFocusIfEmpty(TextBox txt)
        {
            bool returnValue = true;
            if (txt.Text == "")
            {
                txt.Focus();
                returnValue = false;
            }
            try
            {
                Convert.ToInt32(txt.Text);
            }
            catch (Exception ex)
            {
                txt.Focus();
                returnValue = false;
            }
            return returnValue;
        }
        private void btnInitWA_Click(object sender, EventArgs e)
        {
            if (ValidateControl() == true)
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

        }

        GroupGeneratorModel groupGeneratorModel;
        private void btnSTart_Click(object sender, EventArgs e)
        {
            if (campaignStatusEnum == CampaignStatusEnum.Finish)
            {
                gridStatus.Rows.Clear();
            }

            if (campaignStatusEnum == CampaignStatusEnum.Paused)
            {

                ChangeCampStatus(CampaignStatusEnum.Running);
                startProgressBar();
            }
            else
            {
                ValidateControlsGroup();
            }
        }

        private void ValidateControlsGroup()
        {
            groupGeneratorModel = new GroupGeneratorModel();
            groupGeneratorModel.contactList = new List<ContactModel>();

            ContactModel contact;
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleaseFollowStepnoThree, Alerts.Alert.enmType.Error);
                return;
            }

            int totalGrouptoCreate = Convert.ToInt32(materialTextBox25.Text);

            int currentIncrement = Convert.ToInt32(materialTextBox22.Text);

            groupGeneratorModel.contactList = new List<ContactModel>();
            for (int i = 0; i < totalGrouptoCreate; i++)
            {
                contact = new ContactModel();
                contact.name = materialTextBox21.Text + currentIncrement.ToString() + materialTextBox23.Text;
                contact.sendStatusModel = new SendStatusModel { isDone = false };
                groupGeneratorModel.contactList.Add(contact);
                currentIncrement = currentIncrement + Convert.ToInt32(materialTextBox22.Text);
            }
            groupGeneratorModel.DefaultNumberAdd = materialTextBox24.Text;
            groupGeneratorModel.singleSettingModel = new SingleSettingModel();
            groupGeneratorModel.singleSettingModel.delayAfterMessages = Convert.ToInt32(txtdelayAfterMessages.Text);
            groupGeneratorModel.singleSettingModel.delayAfterMessagesFrom = Convert.ToInt32(txtdelayAfterMessagesFrom.Text);
            groupGeneratorModel.singleSettingModel.delayAfterMessagesTo = Convert.ToInt32(txtdelayAfterMessagesTo.Text);
            groupGeneratorModel.singleSettingModel.delayAfterEveryMessageFrom = Convert.ToInt32(txtdelayAfterEveryMessageFrom.Text);
            groupGeneratorModel.singleSettingModel.delayAfterEveryMessageTo = Convert.ToInt32(txtdelayAfterEveryMessageTo.Text);
            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {
                worker.RunWorkerAsync();
                ChangeCampStatus(CampaignStatusEnum.Running);
                startProgressBar();
                initTimer();
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
            int i = 1;
            foreach (var item in groupGeneratorModel.contactList)
            {
                if (item.sendStatusModel.isDone == true && item.logged == false)
                {
                    var globalCounter = gridStatus.Rows.Count - 1;
                    gridStatus.Rows.Add();
                    gridStatus.Rows[globalCounter].Cells[0].Value = globalCounter + 1;
                    gridStatus.Rows[globalCounter].Cells[1].Value = item.name;
                    gridStatus.Rows[globalCounter].Cells[2].Value = item.sendStatusModel.sendStatusEnum;
                    gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
                    item.logged = true;
                    i = i + 1;
                }
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

        private void GroupGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.Complete();
            waSenderForm.Show();
        }
    }
}
