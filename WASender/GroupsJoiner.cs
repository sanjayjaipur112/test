using FluentValidation.Results;
using MaterialSkin;
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
    public partial class GroupsJoiner : MaterialForm
    {

        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        WaSenderForm waSenderForm;
        CampaignStatusEnum campaignStatusEnum;
        System.Windows.Forms.Timer timerRunner;
        private bool IsStopped = true;
        private bool IsPaused = false;
        Logger logger;

        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public GroupsJoiner(WaSenderForm _waSenderForm, List<string> links = null)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            logger = new Logger("GroupJoiner");
            generalSettingsModel = Config.GetSettings();
            waSenderForm = _waSenderForm;
            if (links != null)
            {
                ImportLinks(links);
            }
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

        private void GroupsJoiner_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        public void ImportLinks(List<string> links)
        {
            foreach (var item in links)
            {
                DataGridViewRow row = (DataGridViewRow)gridTargetsGroup.Rows[0].Clone();
                row.Cells[0].Value = item;
                gridTargetsGroup.Rows.Add(row);
            }
            lblCount.Text = links.Count().ToString();
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

                            gridTargetsGroup.Rows.Add();
                            gridTargetsGroup.Rows[globalCounter].Cells[0].Value = worksheet.Cells[i + 1, 1].Value.ToString();
                            globalCounter++;

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

        private void init()
        {
            logger.WriteLog("init");
            ChangeInitStatus(InitStatusEnum.NotInitialised);
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
            materialCheckbox1.Checked = true;
            materialCheckbox2.Checked = true;
        }


        private void initBackgroundWorker()
        {
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
        }


        private bool isValidLink(string link)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(link, UriKind.Absolute, out uriResult)
                && uriResult.Scheme == Uri.UriSchemeHttps;
            if (result == true)
            {
                if (link.Contains("https://chat.whatsapp.com/"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return result;
        }

        bool isWPPijected = false;
  


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

        private  void btnSTart_Click(object sender, EventArgs e)
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
                ValidateControlsGroup();
            }
        }
        WASenderGroupTransModel wASenderGroupTransModel;
        private async void ValidateControlsGroup()
        {
            wASenderGroupTransModel = new WASenderGroupTransModel();
            wASenderGroupTransModel.groupList = new List<GroupModel>();
            GroupModel group = new GroupModel();
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleaseFollowStepnoThree, Alerts.Alert.enmType.Error);
                return;
            }

            for (int i = 0; i < gridTargetsGroup.Rows.Count; i++)
            {
                if (!(gridTargetsGroup.Rows[i].Cells[0].Value == null))
                {
                    group = new GroupModel
                    {
                        Name = gridTargetsGroup.Rows[i].Cells[0].Value == null ? "" : gridTargetsGroup.Rows[i].Cells[0].Value.ToString(),
                        sendStatusModel = new SendStatusModel { isDone = false }
                    };

                    group.validationFailures = new GroupModelValidator().Validate(group).Errors;
                    wASenderGroupTransModel.groupList.Add(group);
                }
            }

            wASenderGroupTransModel.validationFailures = new WASenderGroupTransModelValidator(true).Validate(wASenderGroupTransModel).Errors;


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
                    //worker.RunWorkerAsync();
                    ChangeCampStatus(CampaignStatusEnum.Running);
                    startProgressBar();

                    try
                    {
                        bool s = await startJoining();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    ChangeCampStatus(CampaignStatusEnum.Finish);
                    stopProgressbar();
                    //initTimer();
                }
            }
            else
            {

            }

        }

        private void addtoGrid(GroupModel item)
        {
            var globalCounter = gridStatus.Rows.Count - 1;
            gridStatus.Rows.Add();
            gridStatus.Rows[globalCounter].Cells[0].Value = iCounter;
            gridStatus.Rows[globalCounter].Cells[1].Value = item.Name;
            gridStatus.Rows[globalCounter].Cells[2].Value = item.sendStatusModel.sendStatusEnum + " " + (item.reason != "" && item.reason != null ? "- " + item.reason : "");

            gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
            item.logged = true;
            iCounter = iCounter + 1;
        }
        private async Task<bool> startJoining()
        {
            WebView2 wv = null;
            int counter = 0;
            int totalCounter = 0;
            foreach (GroupModel item in wASenderGroupTransModel.groupList)
            {
                if (IsPaused)
                {
                    while (IsPaused)
                    {
                        await Task.Delay(500);
                    }
                }
                if (IsStopped)
                {
                    return true;
                }


                try
                {
                    if (!isValidLink(item.Name))
                    {

                        item.sendStatusModel.isDone = true;
                        item.sendStatusModel.sendStatusEnum = SendStatusEnum.NotValidLink;
                        totalCounter++;
                        addtoGrid(item);
                        continue;
                    }

                    string Groupcode = item.Name.Replace("https://chat.whatsapp.com/", "");
                    Groupcode = Groupcode.Replace("invite/", "");
                    if (Groupcode.Contains("?"))
                    {
                        var splitter = Groupcode.Split('?');
                        Groupcode = splitter[0];
                    }

                  
                    if (generalSettingsModel.browserType == 1)
                    {
                        if (isWPPijected == false)
                        {
                            if (!WAPIHelper.IsWAPIInjected(driver))
                            {
                                ProjectCommon.injectWapi(driver);
                                await Task.Delay(1000);
                            }
                            isWPPijected = true;
                        }
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        try
                        {
                            wv = Utils.GetActiveWebView(browser);
                            if (isWPPijected == false)
                            {
                                if (!await WPPHelper.isWPPinjected(wv))
                                {
                                    await WPPHelper.InjectWapiSync(wv, Config.GetSysFolderPath());
                                    await Task.Delay(1000);
                                }
                                isWPPijected = true;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    bool isJoined = false;
                    string outParam = "";

                    if (generalSettingsModel.browserType == 1)
                    {
                        isJoined =  WAPIHelper.JoinGroup(driver, Groupcode.Trim(), out outParam);
                        if (isJoined == false)
                        {
                            await Task.Delay(1000);
                            isJoined = WAPIHelper.JoinGroup(driver, Groupcode.Trim(), out outParam);
                        }
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        isJoined = await WPPHelper.JoinGroupAsync((WebView2)wv, Groupcode.Trim());
                        if (isJoined == false)
                        {
                            await Task.Delay(1000);
                            isJoined = await WPPHelper.JoinGroupAsync((WebView2)wv, Groupcode.Trim());
                            outParam = WPPHelper.GroupJoinFaildReson;
                        }
                    }

                   
                    item.sendStatusModel.isDone = true;
                    if (isJoined == true)
                    {
                        item.sendStatusModel.sendStatusEnum = SendStatusEnum.Success;
                        counter++;
                    }
                    else
                    {
                        item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                        item.reason = outParam;
                    }


                    await Task.Delay(Utils.getRandom(wASenderGroupTransModel.settings.delayAfterEveryMessageFrom * 1000, wASenderGroupTransModel.settings.delayAfterEveryMessageTo * 1000));

                    if (wASenderGroupTransModel.settings.delayAfterMessages == counter)
                    {
                        counter = 0;
                        int sleeper = Utils.getRandom(wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000, wASenderGroupTransModel.settings.delayAfterMessagesFrom * 1000);
                        await Task.Delay(sleeper);
                    }
                    totalCounter++;
                    addtoGrid(item);

                }
                catch (Exception ex)
                {
                    item.sendStatusModel.isDone = true;
                    item.sendStatusModel.sendStatusEnum = SendStatusEnum.Failed;
                    totalCounter++;
                    addtoGrid(item);
                    continue;
                }
                int persentage = totalCounter * 100 / wASenderGroupTransModel.groupList.Count();
                lblPersentage.Text = persentage.ToString() + "% " + Strings.Completed;
            }
            ChangeCampStatus(CampaignStatusEnum.Finish);
            stopProgressbar();
            return true;
        }


        int iCounter = 1;
       
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
        private bool showValidationErrorIfAnyGroup()
        {
            bool validationFail = true;
            if (CheckValidationMessage(wASenderGroupTransModel.validationFailures))
            {
                if (CheckValidationMessage(wASenderGroupTransModel.settings.validationFailures))
                {
                    for (int i = 0; i < wASenderGroupTransModel.groupList.Count(); i++)
                    {
                        if (CheckValidationMessage(wASenderGroupTransModel.groupList[i].validationFailures, Strings.RowNo + "- " + Convert.ToString(i + 1)))
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

        private void GroupsJoiner_Load(object sender, EventArgs e)
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
            stopProgressbar();
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
            this.Text = Strings.GroupsJoiner;
            materialButton1.Text = Strings.UploadSampleExcel;
            De.Text = Strings.DelaySettings;
            materialLabel3.Text = Strings.Wait;
            materialLabel9.Text = Strings.Wait;
            materialLabel4.Text = Strings.to;
            materialLabel8.Text = Strings.to;
            materialLabel5.Text = Strings.secondsafterevery;
            materialLabel6.Text = Strings.GroupJoin;
            materialLabel7.Text = Strings.secondsbeforeeveryGroupJoin;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourrmobile;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            btnInitWA.Text = Strings.ClicktoInitiate;
            btnSTart.Text = Strings.Start;
            materialButton2.Text = Strings.Pause;
            materialButton3.Text = Strings.Stop;

            gridTargetsGroup.Columns[0].HeaderText = Strings.GroupLink;
            gridStatus.Columns[0].HeaderText = Strings.ChatName;
            gridStatus.Columns[1].HeaderText = Strings.Status;
            label8.Text = Strings.Log;
        }

        private void GroupsJoiner_FormClosing(object sender, FormClosingEventArgs e)
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

        private void materialButton2_Click(object sender, EventArgs e)
        {
            IsPaused = true;
            ChangeCampStatus(CampaignStatusEnum.Paused);
            stopProgressbar();
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            stopProgressbar();
            IsStopped = true;
            IsPaused = false;

        }

        private void gridTargetsGroup_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
        }

        private void gridTargetsGroup_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
        }

    }
}
