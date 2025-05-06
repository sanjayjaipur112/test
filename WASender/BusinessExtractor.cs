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
using WaAutoReplyBot;
using WASender;
using WASender.enums;
using WASender.Models;
using Models;
using System.IO;
using OfficeOpenXml;
using OpenQA.Selenium;
using System.Threading;
using Microsoft.Web.WebView2.WinForms;
namespace WASender
{
    public partial class BusinessExtractor : MyMaterialPopOp
    {
        CampaignStatusEnum campaignStatusEnum;
        WaSenderBrowser browser;
        WASenderSingleTransModel wASenderGroupTransModel;
        NumberFilter numberFilter;
        GeneralSettingsModel generalSettingsModel;
        private TestClass _testClass;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;

        public BusinessExtractor(WASenderSingleTransModel _wASenderGroupTransModel, NumberFilter _numberFilter)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            wASenderGroupTransModel = _wASenderGroupTransModel;
            numberFilter = _numberFilter;
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
            ChangeCampStatus(CampaignStatusEnum.Stopped);
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
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("session not created"))
                {
                    DialogResult dr = MessageBox.Show("Your Chrome Driver and Google Chrome Version Is not same, Click 'Yes botton' to Update it from Settings", "Error ", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if (dr == DialogResult.Yes)
                    {
                        this.Hide();
                        this.numberFilter.Show();
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
        }

      

      

        private void BusinessExtractor_Load(object sender, EventArgs e)
        {
            initLanguages();
            ChangeCampStatus(CampaignStatusEnum.NotStarted);
            materialCheckbox1.Checked = true;
            label2.Text = wASenderGroupTransModel.contactList.Where(x => x.isBusiness == true).ToList().Select(x => x.number).ToList().Count().ToString();

            if (generalSettingsModel.browserType == 1)
            {
                initWA();
            }
            else if (generalSettingsModel.browserType == 2)
            {
                initWABrowser();
            }
        }

        private void initLanguages()
        {
            this.Text = Strings.BusinessProfileExtractor;
            De.Text = Strings.DelaySettings;
            materialLabel3.Text = Strings.Wait;
            materialLabel5.Text = Strings.secondsafterevery;
            materialLabel6.Text = Strings.NumberCheck;
            btnSTart.Text = Strings.Start;
            label3.Text = Strings.TotalBusinessProfilestoExtract;
            label7.Text = Strings.Status;
            materialButton1.Text = Strings.Export;
            label1.Text = Strings.BusinessInstruction_1;

            gridStatus.Columns[0].HeaderText = "#";
            gridStatus.Columns[1].HeaderText = Strings.Number;
            gridStatus.Columns[2].HeaderText = Strings.BusinessName;
            gridStatus.Columns[3].HeaderText = Strings.Catagory;
            gridStatus.Columns[4].HeaderText = Strings.Description;
            gridStatus.Columns[5].HeaderText = Strings.EmailId;
            gridStatus.Columns[6].HeaderText = Strings.Website;
        }

        public void ChangeCampStatus(CampaignStatusEnum _campaignStatus)
        {
            this.campaignStatusEnum = _campaignStatus;
            AutomationCommon.ChangeCampStatus(_campaignStatus, lblRunStatus);
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
        private async void btnSTart_Click(object sender, EventArgs e)
        {
            if (campaignStatusEnum == CampaignStatusEnum.NotStarted)
            {
                startProgressBar();
                ChangeCampStatus(CampaignStatusEnum.Running);
                List<string> numbers = wASenderGroupTransModel.contactList.Where(x => x.isBusiness == true).ToList().Select(x => x.number).ToList();
                var chunks = LinqHelpers.Chunk(numbers, Convert.ToInt32(txtdelayAfterMessages.Text)).ToList();
                WebView2 wv = new WebView2();
                {
                    try
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            WAPIHelper.IsWAPIInjected(driver);
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            browser.Invoke((MethodInvoker)delegate
                            {
                                wv = Utils.GetActiveWebView(browser);
                            });
                            
                        }

                    }
                    catch (Exception ex)
                    {
                        
                    }
                    if (generalSettingsModel.browserType == 1)
                    {
                        if (!WAPIHelper.IsWAPIInjected(driver))
                        {
                            ProjectCommon.injectWapi(driver);
                            await Task.Delay(1000);
                        }
                    }
                    else if (generalSettingsModel.browserType == 2)
                    {
                        browser.Invoke((MethodInvoker)delegate
                        {
                            wv = Utils.GetActiveWebView(browser);
                        });

                        if (!await WPPHelper.isWPPinjected(wv))
                        {
                            await WPPHelper.InjectWapi(wv, Config.GetSysFolderPath());
                            await Task.Delay(1000);
                        }
                    }



                    int i = 1;
                    foreach (var chunk in chunks)
                    {
                        if (generalSettingsModel.browserType == 1)
                        {
                            bool isDone = WAPIHelper.OpenAllChat(driver, chunk.ToList());
                            await Task.Delay(1000);
                            isDone = WAPIHelper.OpenAllChat(driver, chunk.ToList());
                            await Task.Delay(1000);
                        }
                        else if (generalSettingsModel.browserType == 2)
                        {
                            bool isDone = await WPPHelper.openAllchat(wv, chunk.ToList());
                            await Task.Delay(1000);
                            isDone = await WPPHelper.openAllchat(wv, chunk.ToList());
                            await Task.Delay(1000);
                        }


                        foreach (var item in chunk.ToList())
                        {
                            try
                            {
                                BusinessProfileModel profile = null;
                                if (generalSettingsModel.browserType == 1)
                                {
                                    profile = WAPIHelper.getBusinessProfileDetails(driver, item);
                                }
                                else if (generalSettingsModel.browserType == 2)
                                {
                                    profile = await WPPHelper.getBusinessProfile(wv, item);
                                }

                                if (profile != null)
                                {
                                    try
                                    {
                                        gridStatus.Rows.Add(new object[]{
                                            i.ToString(),
                                            profile.number,
                                            profile.name,
                                            profile.catagory,
                                            profile.desc,
                                            profile.email,
                                            profile.website
                                        });

                                        i = i + 1;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }

                        }

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
                stopProgressbar();
                ChangeCampStatus(CampaignStatusEnum.Finish);

            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (wASenderGroupTransModel != null)
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
                    ws.Cells[1, 1].Value = "Id";
                    ws.Cells[1, 2].Value = "Number";
                    ws.Cells[1, 3].Value = "Business Name";
                    ws.Cells[1, 4].Value = "Cagagory";
                    ws.Cells[1, 5].Value = "Description";
                    ws.Cells[1, 6].Value = "Email Id";
                    ws.Cells[1, 7].Value = "Website";

                    int j = 0;
                    foreach (DataGridViewRow row in gridStatus.Rows)
                    {

                        ws.Cells[j + 2, 1].Value = row.Cells[0].Value != null ? row.Cells[0].Value.ToString() : "";
                        ws.Cells[j + 2, 2].Value = row.Cells[1].Value != null ? row.Cells[1].Value.ToString() : "";
                        ws.Cells[j + 2, 3].Value = row.Cells[2].Value != null ? row.Cells[2].Value.ToString() : "";
                        ws.Cells[j + 2, 4].Value = row.Cells[3].Value != null ? row.Cells[3].Value.ToString() : "";
                        ws.Cells[j + 2, 5].Value = row.Cells[4].Value != null ? row.Cells[4].Value.ToString() : "";
                        ws.Cells[j + 2, 6].Value = row.Cells[5].Value != null ? row.Cells[5].Value.ToString() : "";
                        ws.Cells[j + 2, 7].Value = row.Cells[6].Value != null ? row.Cells[6].Value.ToString() : "";
                        j++;
                    }

                    xlPackage.Save();
                }


                savesampleExceldialog.FileName = "BusinessProfiles.xlsx";
                savesampleExceldialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
                if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
                {
                    File.Copy(NewFileName, savesampleExceldialog.FileName.EndsWith(".xlsx") ? savesampleExceldialog.FileName : savesampleExceldialog.FileName + ".xlsx", true);
                    Utils.showAlert(Strings.Filedownloadedsuccessfully, WASender.Alerts.Alert.enmType.Success);
                }
            }
        }

    }
}
