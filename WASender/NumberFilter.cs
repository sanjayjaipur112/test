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
    public partial class NumberFilter : MaterialForm
    {

        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        WaSenderForm waSenderForm;
        CampaignStatusEnum campaignStatusEnum;
        System.Windows.Forms.Timer timerRunner;
        Logger logger;
        private bool IsStopped = true;
        private bool IsPaused = false;
        WASenderSingleTransModel wASenderGroupTransModel;
        GeneralSettingsModel generalSettingsModel;
        WaSenderBrowser browser;
        private TestClass _testClass;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public NumberFilter(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            logger = new Logger("NumberFilter");
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

        public void init()
        {
            logger.WriteLog("init");
            ChangeInitStatus(InitStatusEnum.NotInitialised);
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
            materialCheckbox1.Checked = true;
        }

        private void InitLanguage()
        {
            this.Text = Strings.WhatsAppNumberFilter;
            materialButton1.Text = Strings.UploadSampleExcel;
            De.Text = Strings.DelaySettings;
            materialLabel3.Text = Strings.Wait;
            materialLabel4.Text = Strings.to;
            materialLabel5.Text = Strings.secondsafterevery;
            materialLabel6.Text = Strings.NumberCheck;
            materialLabel2.Text = Strings.InitiateWhatsAppScaneQRCodefromyourrmobile;
            label5.Text = Strings.Status;
            label7.Text = Strings.Status;
            btnInitWA.Text = Strings.ClicktoInitiate;
            btnSTart.Text = Strings.Start;
            //materialButton3.Text = Strings.Stop;

            gridTargetsGroup.Columns[0].HeaderText = Strings.Number;
            gridStatus.Columns[1].HeaderText = Strings.Number;
            gridStatus.Columns[2].HeaderText = Strings.Status;
            gridStatus.Columns[3].HeaderText = Strings.BusinessAccount;
            materialButton4.Text = Strings.GenerateNumbers;
            materialButton3.Text = Strings.ExportAll;
            materialButton5.Text = Strings.ExportAvailable;
            materialButton2.Text = Strings.BusinessProfileExtractor;
            label8.Text = Strings.Log;

            addCountryCodeToolStripMenuItem.Text = Strings.AddCountryCode;
            importNumbersToolStripMenuItem.Text = Strings.CopyPasteNumber;
            removeDuplicatesToolStripMenuItem.Text = Strings.RemoveDuplicateNumbers;
            deleteAllRowsToolStripMenuItem.Text = Strings.DeleteAllRows;
        }

        private void NumberFilter_FormClosing(object sender, FormClosingEventArgs e)
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

        private void NumberFilter_Load(object sender, EventArgs e)
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


                                string _Number = worksheet.Cells[i + 1, 1].Value.ToString();
                                gridTargetsGroup.Rows.Add();
                                gridTargetsGroup.Rows[globalCounter].Cells[0].Value = _Number;

                            }
                            catch (Exception ex)
                            {

                            }
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
            //lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
        }

        public void NumberGeneratorReturn(List<string> numbers)
        {
            var globalCounter = gridTargetsGroup.Rows.Count - 1;
            for (int i = 1; i < numbers.Count(); i++)
            {
                gridTargetsGroup.Rows.Add();
                gridTargetsGroup.Rows[globalCounter].Cells[0].Value = numbers[i].ToString();
                globalCounter++;
            }
            //lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
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


        private async Task<bool> startChecking()
        {
            WebView2 wv = new WebView2();
            if (generalSettingsModel.browserType == 1)
            {
                try
                {
                    logger.WriteLog("initing WAPI");
                    WAPIHelper.IsWAPIInjected(driver);
                }
                catch (Exception ex)
                {
                    string ss = "";
                    msg = ex.Message;
                }

                if (!WAPIHelper.IsWAPIInjected(driver))
                {
                    ProjectCommon.injectWapi(driver);
                }
                await Task.Delay(1000);
            }

            else if (generalSettingsModel.browserType == 2)
            {
                try
                {
                    wv = Utils.GetActiveWebView(browser);


                    if (!await WPPHelper.isWPPinjected(wv))
                    {
                        await WPPHelper.InjectWapiSync(wv, Config.GetSysFolderPath());
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            try
            {

                foreach (var item in wASenderGroupTransModel.contactList)
                {
                    item.number = Utils.sanitizeNumber(item.number);
                }

                var ss_ = LinqHelpers.Chunk(wASenderGroupTransModel.contactList, Convert.ToInt32(txtdelayAfterMessages.Text)).ToList();

                foreach (var chunk in ss_)
                {
                    List<ContactModel> lisr = chunk.ToList();
                    List<NumberFilterFReturnModel> _ss = new List<NumberFilterFReturnModel>();

                    if (generalSettingsModel.browserType == 1)
                    {
                        _ss = await WAPIHelper.validateNumber_threeSync(driver, lisr, Convert.ToInt32(txtdelayAfterMessagesFrom.Text), Convert.ToInt32(txtdelayAfterMessagesTo.Text));
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        _ss = await WPPHelper.validateNumber_ThreeSync(lisr, Convert.ToInt32(txtdelayAfterMessagesFrom.Text), Convert.ToInt32(txtdelayAfterMessagesTo.Text), wv);
                    }


                    foreach (var item in _ss)
                    {
                        try
                        {
                            if (item.numberExists)
                            {
                                wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.isDone = true;
                                wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.sendStatusEnum = SendStatusEnum.Available;
                                wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().isBusiness = item.isBusiness;
                            }
                            else
                            {
                                wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.isDone = true;
                                wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.sendStatusEnum = SendStatusEnum.NotAvailable;
                                wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().isBusiness = false;
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }



                    int DoneCount = wASenderGroupTransModel.contactList.Where(x => x.sendStatusModel.isDone == true).Count();
                    int totalCount = wASenderGroupTransModel.contactList.Count();
                    int per = ((int)Math.Round((double)(100 * DoneCount) / totalCount));

                    updateGrid();
                    lblPersentage.Text = per.ToString() + "% " + Strings.Completed;


                    //worker.ReportProgress(per);
                    try
                    {
                        int _rand = Utils.getRandom(Convert.ToInt32(txtdelayAfterMessagesFrom.Text), Convert.ToInt32(txtdelayAfterMessagesTo.Text));
                        await Task.Delay(TimeSpan.FromSeconds(_rand));
                    }
                    catch (Exception)
                    {

                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }


            return true;
        }

        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int counter = 0;
            int totalCounter = 0;

            logger.WriteLog("Started Checking");


            if (Config.SendingType == 0)
            {

            }
            else if (Config.SendingType == 1)
            {

                WebView2 wv = new WebView2();

                if (generalSettingsModel.browserType == 1)
                {
                    try
                    {
                        logger.WriteLog("initing WAPI");
                        WAPIHelper.IsWAPIInjected(driver);
                    }
                    catch (Exception ex)
                    {
                        string ss = "";
                        logger.WriteLog(ex.Message);
                        msg = ex.Message;
                    }

                    if (!WAPIHelper.IsWAPIInjected(driver))
                    {
                        ProjectCommon.injectWapi(driver);
                    }
                    Thread.Sleep(1000);
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



                try
                {

                    foreach (var item in wASenderGroupTransModel.contactList)
                    {
                        item.number = Utils.sanitizeNumber(item.number);
                    }

                    var ss_ = LinqHelpers.Chunk(wASenderGroupTransModel.contactList, Convert.ToInt32(txtdelayAfterMessages.Text)).ToList();


                    foreach (var chunk in ss_)
                    {
                        List<ContactModel> lisr = chunk.ToList();
                        List<NumberFilterFReturnModel> _ss = new List<NumberFilterFReturnModel>();

                        if (generalSettingsModel.browserType == 1)
                        {
                            _ss = await WAPIHelper.validateNumber3(driver, lisr, Convert.ToInt32(txtdelayAfterMessagesFrom.Text), Convert.ToInt32(txtdelayAfterMessagesTo.Text));
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            _ss = await WPPHelper.validateNumber3(lisr, Convert.ToInt32(txtdelayAfterMessagesFrom.Text), Convert.ToInt32(txtdelayAfterMessagesTo.Text), wv);
                        }


                        foreach (var item in _ss)
                        {
                            try
                            {
                                if (item.numberExists)
                                {
                                    wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.isDone = true;
                                    wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.sendStatusEnum = SendStatusEnum.Available;
                                    wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().isBusiness = item.isBusiness;
                                }
                                else
                                {
                                    wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.isDone = true;
                                    wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.sendStatusEnum = SendStatusEnum.NotAvailable;
                                    wASenderGroupTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().isBusiness = false;
                                }

                            }
                            catch (Exception)
                            {

                            }
                        }



                        int DoneCount = wASenderGroupTransModel.contactList.Where(x => x.sendStatusModel.isDone == true).Count();
                        int totalCount = wASenderGroupTransModel.contactList.Count();
                        int per = ((int)Math.Round((double)(100 * DoneCount) / totalCount));

                        worker.ReportProgress(per);
                        try
                        {
                            int _rand = Utils.getRandom(Convert.ToInt32(txtdelayAfterMessagesFrom.Text), Convert.ToInt32(txtdelayAfterMessagesTo.Text));
                            Thread.Sleep(TimeSpan.FromSeconds(_rand));
                        }
                        catch (Exception)
                        {

                        }

                    }
                }
                catch (Exception ex)
                {

                }


            }





        }


        private void updateGrid()
        {
            int increment = 1;
            foreach (var item in wASenderGroupTransModel.contactList)
            {
                if (item.sendStatusModel.isDone == true && item.logged == false)
                {
                    var globalCounter = gridStatus.Rows.Count - 1;
                    gridStatus.Rows.Add();
                    gridStatus.Rows[globalCounter].Cells[0].Value = (gridStatus.Rows.Count - 1).ToString();
                    gridStatus.Rows[globalCounter].Cells[1].Value = item.number;
                    gridStatus.Rows[globalCounter].Cells[2].Value = item.sendStatusModel.sendStatusEnum;
                    gridStatus.Rows[globalCounter].Cells[3].Value = item.isBusiness;

                    gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
                    item.logged = true;
                    increment = increment + 1;
                }
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {


            lblPersentage.Text = e.ProgressPercentage + "% " + Strings.Completed;

            int increment = 1;
            foreach (var item in wASenderGroupTransModel.contactList)
            {
                if (item.sendStatusModel.isDone == true && item.logged == false)
                {
                    var globalCounter = gridStatus.Rows.Count - 1;
                    gridStatus.Rows.Add();
                    gridStatus.Rows[globalCounter].Cells[0].Value = (gridStatus.Rows.Count - 1).ToString();
                    gridStatus.Rows[globalCounter].Cells[1].Value = item.number;
                    gridStatus.Rows[globalCounter].Cells[2].Value = item.sendStatusModel.sendStatusEnum;
                    gridStatus.Rows[globalCounter].Cells[3].Value = item.isBusiness;

                    gridStatus.FirstDisplayedScrollingRowIndex = gridStatus.RowCount - 1;
                    item.logged = true;
                    increment = increment + 1;
                }
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChangeCampStatus(CampaignStatusEnum.Finish);
            lblPersentage.Text = "100% " + Strings.Completed;
            stopProgressbar();

        }



        private void btnSTart_Click(object sender, EventArgs e)
        {
            lblPersentage.Text = "0% " + Strings.Completed;
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

        private async void ValidateControlsGroup()
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
                    string _number = gridTargetsGroup.Rows[i].Cells[0].Value == null ? "" : gridTargetsGroup.Rows[i].Cells[0].Value.ToString();
                    _number = _number.SanitizeNumber();
                    contact = new ContactModel
                    {
                        number = _number,
                        sendStatusModel = new SendStatusModel { isDone = false }
                    };

                    contact.validationFailures = new ContactModelValidator().Validate(contact).Errors;
                    wASenderGroupTransModel.contactList.Add(contact);
                }
            }

            wASenderGroupTransModel.messages = new List<MesageModel>();
            wASenderGroupTransModel.messages.Add(new MesageModel
            {
                longMessage = "dfg"
            });
            wASenderGroupTransModel.validationFailures = new WASenderSingleTransModelValidator().Validate(wASenderGroupTransModel).Errors;

            wASenderGroupTransModel.settings = new SingleSettingModel();
            wASenderGroupTransModel.settings.delayAfterMessages = Convert.ToInt32(txtdelayAfterMessages.Text);
            wASenderGroupTransModel.settings.delayAfterMessagesFrom = Convert.ToInt32(txtdelayAfterMessagesFrom.Text);
            wASenderGroupTransModel.settings.delayAfterMessagesTo = Convert.ToInt32(txtdelayAfterMessagesTo.Text);

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
                        bool isDOne = await startChecking();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    ChangeCampStatus(CampaignStatusEnum.Finish);
                    stopProgressbar();
                    //initTimer();
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




        private void materialButton3_Click_1(object sender, EventArgs e)
        {
            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "Number_Filter_" + Guid.NewGuid().ToString() + ".xlsx");
            string NewFileName = file.ToString();
            File.Copy("MemberListTemplate.xlsx", NewFileName, true);
            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];

                ws.Cells[1, 1].Value = Strings.Number;
                ws.Cells[1, 2].Value = Strings.Status;
                ws.Cells[1, 3].Value = Strings.BusinessAccount;

                for (int i = 0; i < wASenderGroupTransModel.contactList.Count(); i++)
                {
                    ws.Cells[i + 2, 1].Value = wASenderGroupTransModel.contactList[i].number;
                    ws.Cells[i + 2, 2].Value = wASenderGroupTransModel.contactList[i].sendStatusModel.sendStatusEnum;
                    ws.Cells[i + 2, 3].Value = wASenderGroupTransModel.contactList[i].isBusiness;
                }
                xlPackage.Save();
            }
            savesampleExceldialog.FileName = "NumberFilter.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }

        private void materialButton4_Click(object sender, EventArgs e)
        {
            GenerateNumbers generateNumbers = new GenerateNumbers(this);
            generateNumbers.ShowDialog();
        }


        private void materialButton2_Click_1(object sender, EventArgs e)
        {
            if (campaignStatusEnum == CampaignStatusEnum.Finish)
            {
                BusinessExtractor form = new BusinessExtractor(wASenderGroupTransModel, this);
                form.ShowDialog();
            }
        }

        private void materialButton5_Click(object sender, EventArgs e)
        {
            String FolderPath = Config.GetTempFolderPath();
            String file = Path.Combine(FolderPath, "Number_Filter_Available_" + Guid.NewGuid().ToString() + ".xlsx");
            string NewFileName = file.ToString();
            File.Copy("MemberListTemplate.xlsx", NewFileName, true);
            var newFile = new FileInfo(NewFileName);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


            using (ExcelPackage xlPackage = new ExcelPackage(newFile))
            {
                var ws = xlPackage.Workbook.Worksheets[0];

                ws.Cells[1, 1].Value = Strings.Number;
                ws.Cells[1, 2].Value = Strings.Status;
                ws.Cells[1, 3].Value = Strings.BusinessAccount;

                List<ContactModel> availables = wASenderGroupTransModel.contactList.Where(x => x.sendStatusModel.sendStatusEnum == SendStatusEnum.Available).ToList();
                for (int i = 0; i < availables.Count(); i++)
                {
                    ws.Cells[i + 2, 1].Value = availables[i].number;
                    ws.Cells[i + 2, 2].Value = availables[i].sendStatusModel.sendStatusEnum;
                    ws.Cells[i + 2, 3].Value = availables[i].isBusiness;
                }
                xlPackage.Save();
            }
            savesampleExceldialog.FileName = "NumberFilter_Available.xlsx";
            savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
            }
        }

        private void addCountryCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CountryCodeInput countryCodeInput = new CountryCodeInput(this);
            countryCodeInput.ShowDialog();
        }

        public void CountryCOdeAdded(string code)
        {
            foreach (DataGridViewRow item in gridTargetsGroup.Rows)
            {
                if (item.Cells[0].Value != "" && item.Cells[0].Value != null)
                {
                    item.Cells[0].Value = code + item.Cells[0].Value;
                }
            }
        }


        public void ReturnPasteNumber(List<string> numbers)
        {
            var globalCounter = gridTargetsGroup.Rows.Count - 1;
            for (int i = 0; i < numbers.Count(); i++)
            {
                try
                {
                    gridTargetsGroup.Rows.Add();
                    string MobileNumber = numbers[i];
                    MobileNumber = MobileNumber.Replace("+", "");
                    MobileNumber = MobileNumber.Replace(" ", "");
                    MobileNumber = MobileNumber.Replace(" ", "");
                    gridTargetsGroup.Rows[globalCounter].Cells[0].Value = MobileNumber;
                    globalCounter++;
                }
                catch (Exception ec)
                {

                }
            }
        }
        private void importNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteNumber pasteNumber = new PasteNumber(this);
            pasteNumber.ShowDialog();
        }

        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> AllNumbers = new List<string>();
            foreach (DataGridViewRow row in gridTargetsGroup.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    AllNumbers.Add(row.Cells[0].Value.ToString());
                }

            }
            List<string> newList = AllNumbers.Distinct().ToList();
            gridTargetsGroup.Rows.Clear();

            foreach (string item in newList)
            {
                gridTargetsGroup.Rows.Add(new object[]{
                    item,
                    "",
                });
            }
        }

        private void gridTargetsGroup_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
        }

        private void gridTargetsGroup_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            lblCount.Text = gridTargetsGroup.Rows.Count.ToString();
        }

        private void gridTargetsGroup_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(gridTargetsGroup, new Point(e.Location.X, e.Location.Y));
            }
        }

        private void deleteAllRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridTargetsGroup.Rows.Clear();
        }
    }
}
