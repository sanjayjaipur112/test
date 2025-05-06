using FluentValidation.Results;
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
using WASender.Validators;

namespace WASender
{
    public partial class GroupMemberAdder : MaterialForm
    {
        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        WaSenderForm waSenderForm;
        CampaignStatusEnum campaignStatusEnum;
        System.Windows.Forms.Timer timerRunner;
        Logger logger;
        public List<WAPI_GroupModel> wAPI_GroupModel;
        WAPI_GroupModel wAPI_SelectedGroup;
        private bool IsStopped = true;
        private bool IsPaused = false;

        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public GroupMemberAdder(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            materialCheckbox3.Checked = true;
            this.Icon = Strings.AppIcon;
            this.waSenderForm = _waSenderForm;
            logger = new Logger("GroupAdder");
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
            IsStopped = true;
            ChangeCampStatus(CampaignStatusEnum.Stopped);
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

        private void materialButton1_Click(object sender, EventArgs e)
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
                if (fi.Extension != ".xlsx")
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
                            try
                            {
                                if (worksheet.Cells[i + 1, 1].Value.ToString() != "")
                                {
                                    gridTargetsGroup.Rows.Add();
                                    gridTargetsGroup.Rows[globalCounter].Cells[0].Value = worksheet.Cells[i + 1, 1].Value.ToString();
                                    globalCounter++;    
                                }
                            }
                            catch (Exception ex)
                            {
                                
                            }
                            

                        }
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog(ex.Message);
                        logger.WriteLog(ex.StackTrace);
                        Utils.showAlert(ex.Message, Alerts.Alert.enmType.Error);
                    }
                }
            }
        }

        private void GroupMemberAdder_Load(object sender, EventArgs e)
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

        public void init()
        {
            logger.WriteLog("init");
            ChangeInitStatus(InitStatusEnum.NotInitialised);
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
            materialCheckbox1.Checked = true;
            materialCheckbox2.Checked = true;
        }

        private void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            logger.WriteLog(_campaignStatus.ToString());
            this.campaignStatusEnum = _campaignStatus;
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }

        private void ChangeInitStatus(InitStatusEnum _initStatus)
        {
            this.initStatusEnum = _initStatus;
            logger.WriteLog(_initStatus.ToString());
            AutomationCommon.ChangeInitStatus(_initStatus, lblInitStatus);
        }
        private void InitLanguage()
        {
            this.Text = Strings.BulkAddGroupMembers;
            materialButton1.Text = Strings.UploadSampleExcel;
            De.Text = Strings.DelaySettings;
            materialLabel3.Text = Strings.Wait;
            materialLabel9.Text = Strings.Wait;
            materialLabel4.Text = Strings.to;
            materialLabel8.Text = Strings.to;
            materialLabel5.Text = Strings.secondsafterevery;
            materialLabel6.Text = Strings.NumberAdd;
            materialLabel7.Text = Strings.secondsbeforeeveryNumberCheck;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourrmobile;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            btnInitWA.Text = Strings.ClicktoInitiate;
            btnSTart.Text = Strings.Start;
            materialButton3.Text = Strings.Pause;
            materialButton2.Text = Strings.Stop;

            gridTargetsGroup.Columns[0].HeaderText = Strings.Number;
            gridStatus.Columns[1].HeaderText = Strings.Number;
            gridStatus.Columns[2].HeaderText = Strings.Status;

            label1.Text = Strings.SendGroupInvitationcodeiffail;
            label8.Text = Strings.Log;
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

        static string msg = "";

        WASenderSingleTransModel wASenderGroupTransModel;

        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;
            int totalCounter = 0;

            logger.WriteLog("Started adding");

            if (Config.SendingType == 0)
            {

            }
            else if (Config.SendingType == 1)
            {

                WebView2 wv = new WebView2();
                if (generalSettingsModel.browserType == 1)
                {

                    if (!WAPIHelper.IsWAPIInjected(driver))
                    {
                        ProjectCommon.injectWapi(driver);
                        Thread.Sleep(100);
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


                foreach (var item in wASenderGroupTransModel.contactList)
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
                        try
                        {
                            WAPIHelper.OpenChatopenChatBottom(driver, item.number);
                            string isJoiner = WAPIHelper.addParticipants(driver, wAPI_SelectedGroup.GroupId, item.number, materialCheckbox3.Checked, "Follow this link to join my WhatsApp group:");
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.message = isJoiner;

                        }
                        catch (Exception ex)
                        {
                            logger.WriteLog("Error -- " + ex.Message);
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                        }
                    }
                    if (generalSettingsModel.browserType == 2)
                    {
                        try
                        {
                            string isJoiner = WPPHelper.addParticipants(wv, wAPI_SelectedGroup.GroupId, item.number, materialCheckbox3.Checked, "Follow this link to join my WhatsApp group:");
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.message = isJoiner;
                        }
                        catch (Exception ex)
                        {

                            logger.WriteLog("Error -- " + ex.Message);
                            item.sendStatusModel.isDone = true;
                            item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                        }
                    }

                    counter++;
                    totalCounter++;

                    var _count = wASenderGroupTransModel.contactList.Count();
                    var percentage = totalCounter * 100 / _count;
                    worker.ReportProgress(percentage);


                    Thread.Sleep(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderGroupTransModel.settings.delayAfterEveryMessageTo * 1000));
                    counter++;

                    if (wASenderGroupTransModel.settings.delayAfterMessages == counter)
                    {
                        counter = 0;
                        Thread.Sleep(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000, wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000));
                    }
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
           

            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, wAPI_SelectedGroup.GroupName + "_Group_Adder_" + Guid.NewGuid().ToString() + ".xlsx");
            string NewFileName = file.ToString();
            File.Copy("MemberListTemplate.xlsx", NewFileName, true);
            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];

                for (int i = 0; i < wASenderGroupTransModel.contactList.Count(); i++)
                {
                    ws.Cells[i + 1, 1].Value = wASenderGroupTransModel.contactList[i].number;
                    ws.Cells[i + 1, 2].Value = wASenderGroupTransModel.contactList[i].sendStatusModel.message;
                }
                xlPackage.Save();
            }


            savesampleExceldialog.FileName = wAPI_SelectedGroup.GroupName + "_Group_Adder_Result.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }

        private async void btnSTart_Click(object sender, EventArgs e)
        {
            if (campaignStatusEnum == CampaignStatusEnum.Finish)
            {
                gridStatus.Rows.Clear();
            }
            if (campaignStatusEnum == CampaignStatusEnum.Paused)
            {
                IsPaused = false;
                ChangeCampStatus(CampaignStatusEnum.Running);
                startProgressBar();
            }
            else
            {
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
                    wAPI_GroupModel = await WAPIHelper.getMyOwnedGroups(driver);
                }
                else if (generalSettingsModel.browserType == 2)
                {
                    try
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
                        wAPI_GroupModel = await WPPHelper.getMyOwnedGroups(wv);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                pgbar.Hide();

                ChooseGroup ghooseGroup = new ChooseGroup(this, wAPI_GroupModel);
                ghooseGroup.Show();




            }
        }

        public void ReturnBack(WAPI_GroupModel index)
        {
            wAPI_SelectedGroup = index;

            if (wAPI_GroupModel != null && wAPI_GroupModel.Count() > 0)
            {
                ValidateControlsGroup();
            }
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

        private bool showValidationErrorIfAnyGroup()
        {
            bool validationFail = true;
            if (CheckValidationMessage(wASenderGroupTransModel.validationFailures))
            {
                if (CheckValidationMessage(wASenderGroupTransModel.settings.validationFailures))
                {
                    for (int i = 0; i < wASenderGroupTransModel.contactList.Count(); i++)
                    {
                        if (CheckValidationMessage(wASenderGroupTransModel.contactList[i].validationFailures, Strings.RowNo + "- " + Convert.ToString(i + 1)))
                        {
                            string ss = "";
                        }
                        else
                        {
                            i = wASenderGroupTransModel.contactList.Count;
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

        private void ValidateControlsGroup()
        {
            wASenderGroupTransModel = new WASenderSingleTransModel();
            wASenderGroupTransModel.contactList = new List<ContactModel>();
            ContactModel contact;
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleaseFollowStepnoThree, Alerts.Alert.enmType.Error);
                return;
            }

            for (int i = 0; i < gridTargetsGroup.Rows.Count; i++)
            {
                if (!(gridTargetsGroup.Rows[i].Cells[0].Value == null))
                {
                    contact = new ContactModel
                    {
                        number = gridTargetsGroup.Rows[i].Cells[0].Value == null ? "" : gridTargetsGroup.Rows[i].Cells[0].Value.ToString(),
                        sendStatusModel = new SendStatusModel { isDone = false }
                    };

                    contact.validationFailures = new ContactModelValidator().Validate(contact).Errors;
                    wASenderGroupTransModel.contactList.Add(contact);
                }
            }

            wASenderGroupTransModel.messages = new List<MesageModel>();
            wASenderGroupTransModel.messages.Add(new MesageModel
            {
                longMessage = "Message"
            });
            wASenderGroupTransModel.validationFailures = new WASenderSingleTransModelValidator().Validate(wASenderGroupTransModel).Errors;

            wASenderGroupTransModel.settings = new SingleSettingModel();
            wASenderGroupTransModel.settings.delayAfterMessages = Convert.ToInt32(txtdelayAfterMessages.Text);
            wASenderGroupTransModel.settings.delayAfterMessagesFrom = Convert.ToInt32(txtdelayAfterMessagesFrom.Text);
            wASenderGroupTransModel.settings.delayAfterMessagesTo = Convert.ToInt32(txtdelayAfterMessagesTo.Text);
            wASenderGroupTransModel.settings.delayAfterEveryMessageFrom = Convert.ToInt32(txtdelayAfterEveryMessageFrom.Text);
            wASenderGroupTransModel.settings.delayAfterEveryMessageTo = Convert.ToInt32(txtdelayAfterEveryMessageTo.Text);

            wASenderGroupTransModel.settings.validationFailures = new SingleSettingModelValidator().Validate(wASenderGroupTransModel.settings).Errors;

            if (showValidationErrorIfAnyGroup())
            {
                if (campaignStatusEnum != CampaignStatusEnum.Running)
                {
                    IsStopped = false;
                    worker.RunWorkerAsync();
                    ChangeCampStatus(CampaignStatusEnum.Running);
                    startProgressBar();
                    initTimer();
                }
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
                foreach (var item in wASenderGroupTransModel.contactList)
                {
                    if (item.sendStatusModel.isDone == true && item.logged == false)
                    {
                        var globalCounter = gridStatus.Rows.Count - 1;
                        gridStatus.Rows.Add();
                        gridStatus.Rows[globalCounter].Cells[0].Value = gridStatus.Rows.Count.ToString();
                        gridStatus.Rows[globalCounter].Cells[1].Value = item.number;
                        gridStatus.Rows[globalCounter].Cells[2].Value = item.sendStatusModel.message;

                        gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
                        item.logged = true;
                        i = i + 1;
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void GroupMemberAdder_FormClosed(object sender, FormClosedEventArgs e)
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

            waSenderForm.formReturn(true);

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

        private void materialButton3_Click(object sender, EventArgs e)
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

        private void gridTargetsGroup_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
        }

        private void gridTargetsGroup_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
        }


    }
}
