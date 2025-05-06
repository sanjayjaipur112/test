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
    public partial class QuickFilter : MyMaterialPopOp
    {
        CampaignStatusEnum campaignStatusEnum;
        WASenderSingleTransModel wASenderSingleTransModel;
        WASenderSingleTransModel wASenderSingleTransModelOriginal;
        WaSenderBrowser browser;
        RunSingle runSingle;
        GeneralSettingsModel generalSettingsModel;
        private TestClass _testClass;
        IWebDriver driver;
        System.Windows.Forms.Timer timerInitChecker;
        BackgroundWorker worker;
        bool isStop = false;
        public QuickFilter(WASenderSingleTransModel _WASenderSingleTransModel, RunSingle _runSingle)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            wASenderSingleTransModel = _WASenderSingleTransModel;
            wASenderSingleTransModelOriginal = _WASenderSingleTransModel;
            runSingle = _runSingle;
            generalSettingsModel = Config.GetSettings();
            stopProgressbar();
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

            preparGoodNumbers();

        }

        private void preparGoodNumbers()
        {
            foreach (ContactModel item in this.wASenderSingleTransModel.contactList)
            {
                if (!item.number.Contains("@uid"))
                {
                    item.number = item.number.SanitizeNumber();
                }
            }
        }

        void _testClass_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            ChangeCampStatus(CampaignStatusEnum.Stopped);
            stopProgressbar();
        }

        public void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            this.campaignStatusEnum = _campaignStatus;
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
        }

        int retryAttempt = 0;
        private void initWABrowser()
        {
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
            timerInitChecker = new System.Windows.Forms.Timer();
            timerInitChecker.Interval = 1000;
            timerInitChecker.Tick += timerInitChecker_Tick;
            timerInitChecker.Start();
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
                        timerInitChecker.Stop();
                        Activate();
                        ChangeCampStatus(CampaignStatusEnum.NotStarted);
                        startFiltaring();
                    }
                }
                catch (Exception ex)
                {

                    timerInitChecker.Stop();
                }

                try
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_1jJ70"), driver);
                    if (isElementDisplayed == true)
                    {
                        timerInitChecker.Stop();
                        ChangeCampStatus(CampaignStatusEnum.NotStarted);
                        Activate();
                        startFiltaring();
                    }
                }
                catch (Exception ex)
                {
                    timerInitChecker.Stop();
                }

                try
                {
                    bool isElementDisplayed = AutomationCommon.IsElementPresent(By.ClassName("_aigu"), driver);
                    if (isElementDisplayed == true)
                    {
                        timerInitChecker.Stop();
                        ChangeCampStatus(CampaignStatusEnum.NotStarted);
                        Activate();
                        startFiltaring();
                    }
                }
                catch (Exception ex)
                {
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
                        timerInitChecker.Stop();
                        ChangeCampStatus(CampaignStatusEnum.NotStarted);
                        Activate();
                        startFiltaring();
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
        public async void startFiltaring()
        {
            ChangeCampStatus(CampaignStatusEnum.Running);
            startProgressBar();
            try
            {
                bool isDOne = await startChecking();
                ChangeCampStatus(CampaignStatusEnum.Finish);
                stopProgressbar();
                if (isStop == false)
                    SaveAndReturn();
            }
            catch (Exception ex)
            {

            }
        }
        private void initWA()
        {

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
                    Utils.SetDriver();
                    this.driver = Utils.Driver;

                }
                checkQRScanDone();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("session not created"))
                {
                    DialogResult dr = MessageBox.Show("Your Chrome Driver and Google Chrome Version Is not same, Click 'Yes botton' to Update it from Settings", "Error ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if (dr == DialogResult.Yes)
                    {
                        this.Hide();
                        //this.numberFilter.Show();
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

        private void QuickFilter_Load(object sender, EventArgs e)
        {
            initLanguages();
        }

        


        private async Task<bool> startChecking()
        {
            WebView2 wv = new WebView2();
            if (generalSettingsModel.browserType == 1)
            {
                try
                {

                    WAPIHelper.IsWAPIInjected(driver);
                }
                catch (Exception ex)
                {
                    string ss = "";

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

                foreach (var item in wASenderSingleTransModel.contactList)
                {
                    if (!item.number.Contains("@uid"))
                    {
                        item.number = Utils.sanitizeNumber(item.number);
                    }
                    else
                    {
                        item.sendStatusModel.isDone = true;
                        item.sendStatusModel.sendStatusEnum = SendStatusEnum.Available;
                        item.isBusiness = false;
                    }
                }

                var ss_ = LinqHelpers.Chunk(wASenderSingleTransModel.contactList.Where(x => !x.number.Contains("@uid")), Convert.ToInt32(wASenderSingleTransModel.settings.delayAfterMessages)).ToList();

                foreach (var chunk in ss_)
                {

                    if (isStop)
                    {
                        break;
                    }
                    List<ContactModel> lisr = chunk.ToList();
                    List<NumberFilterFReturnModel> _ss = new List<NumberFilterFReturnModel>();

                    if (generalSettingsModel.browserType == 1)
                    {
                        _ss = await WAPIHelper.validateNumber3(driver, lisr, Convert.ToInt32(wASenderSingleTransModel.settings.delayAfterMessagesFrom), Convert.ToInt32(wASenderSingleTransModel.settings.delayAfterMessagesTo));
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        _ss = await WPPHelper.validateNumber_ThreeSync(lisr, Convert.ToInt32(wASenderSingleTransModel.settings.delayAfterMessagesFrom), Convert.ToInt32(wASenderSingleTransModel.settings.delayAfterMessagesTo), wv);
                    }


                    foreach (var item in _ss)
                    {
                        try
                        {
                            if (item.numberExists)
                            {
                                wASenderSingleTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.isDone = true;
                                wASenderSingleTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.sendStatusEnum = SendStatusEnum.Available;
                                wASenderSingleTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().isBusiness = item.isBusiness;
                            }
                            else
                            {
                                wASenderSingleTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.isDone = true;
                                wASenderSingleTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().sendStatusModel.sendStatusEnum = SendStatusEnum.NotAvailable;
                                wASenderSingleTransModel.contactList.Where(x => x.number == item.id).FirstOrDefault().isBusiness = false;
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }



                    int DoneCount = wASenderSingleTransModel.contactList.Where(x => x.sendStatusModel.isDone == true).Count();
                    int totalCount = wASenderSingleTransModel.contactList.Count();
                    int per = ((int)Math.Round((double)(100 * DoneCount) / totalCount));

                    updateGrid();
                    lblPersentage.Text = per.ToString() + "% " + Strings.Completed;


                    //worker.ReportProgress(per);
                    if (totalCount < DoneCount)
                    {
                        try
                        {
                            int _rand = Utils.getRandom(Convert.ToInt32(wASenderSingleTransModel.settings.delayAfterMessagesFrom), Convert.ToInt32(wASenderSingleTransModel.settings.delayAfterMessagesTo));
                            await Task.Delay(TimeSpan.FromSeconds(_rand));
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                    

                }
            }
            catch (Exception ex)
            {

                throw;
            }


            return true;
        }


        private void updateGrid()
        {
            int increment = 1;
            try
            {
                foreach (var item in wASenderSingleTransModel.contactList)
                {
                    if (item.sendStatusModel.isDone == true && item.logged == false)
                    {
                        var globalCounter = dataGridView1.Rows.Count;
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[globalCounter].Cells[0].Value = item.number;
                        dataGridView1.Rows[globalCounter].Cells[1].Value = item.sendStatusModel.sendStatusEnum;
                        dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
                        item.logged = true;
                        increment = increment + 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }



        private void initLanguages()
        {
            this.Text = Strings.QuickFilter;
            dataGridView1.Columns[0].HeaderText = Strings.Number;
            dataGridView1.Columns[1].HeaderText = Strings.Status;
            label7.Text = Strings.Status;
            lblRunStatus.Text = Strings.NotInitialised;
            linkLabel1.Text = Strings.SkipFiltaringandsendMessagewithoutcheckingnumber;
        }


        private void QuickFilter_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                worker.CancelAsync();
                worker.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            isStop = true;
            this.wASenderSingleTransModel = wASenderSingleTransModelOriginal;
            dataGridView1.Rows.Clear();
            lblPersentage.Text = "";

            foreach (ContactModel item in this.wASenderSingleTransModel.contactList)
            {
                item.sendStatusModel.sendStatusEnum = SendStatusEnum.NotStarted;
            }

            stopProgressbar();
            ChangeCampStatus(CampaignStatusEnum.Stopped);
            SaveAndReturn(true);
        }
        private void SaveAndReturn(bool isSkiped = false)
        {
            foreach (var item in wASenderSingleTransModel.contactList)
            {
                item.logged = false;
                item.sendStatusModel.isDone = false;
            }

            this.runSingle.returnFromQuickFilter(wASenderSingleTransModel, isSkiped);
            this.Close();
        }
    }
}
