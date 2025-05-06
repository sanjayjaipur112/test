using MaterialSkin;
using MaterialSkin.Controls;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
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

namespace WASender
{
    public partial class GrabGroupLinks : MaterialForm
    {

        InitStatusEnum initStatusEnum;
        System.Windows.Forms.Timer timerInitChecker;
        IWebDriver driver;
        BackgroundWorker worker;
        CampaignStatusEnum campaignStatusEnum;
        WaSenderForm waSenderForm;

        GeneralSettingsModel generalSettingsModel;
        private System.ComponentModel.BackgroundWorker backgroundWorker_productChecker;
        Progressbar pgbar;

        public GrabGroupLinks(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            waSenderForm = _waSenderForm;
            generalSettingsModel = Config.GetSettings();
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





        List<string> chatNames;

        private void btnInitWA_Click(object sender, EventArgs e)
        {

            ChangeInitStatus(InitStatusEnum.Initialising);
            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddExcludedArgument("enable-automation");
                options.AddAdditionalCapability("useAutomationExtension", false);
                var chromeDriverService = ChromeDriverService.CreateDefaultService(Config.GetChromeDriverFolder());
                chromeDriverService.HideCommandPromptWindow = true;


                driver = new ChromeDriver(chromeDriverService, options);
                driver.Url = "https://www.google.com/search?q=whatsapp+group+links&oq=whatsapp+group+links&aqs=chrome.0.69i59j0i433i512j0i512j0i457i512j0i402j69i60l3.2696j0j7&sourceid=chrome&ie=UTF-8";

                ChangeInitStatus(InitStatusEnum.Initialised);
            }
            catch (Exception ex)
            {
                ChangeInitStatus(InitStatusEnum.Unable);
                string ss = "";
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
                if (ex.Message.Contains("The specified executable is not a valid application for this OS platform"))
                {
                    MessageBox.Show(Strings.ChromeDriversarenotDownloadedProperly, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        private void GrabGroupLinks_Load(object sender, EventArgs e)
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
            //
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
            this.Text = Strings.GrabGroupLinks;
            this.materialLabel2.Text = Strings.Clickbellowbuttontoopenbrowser;
            this.label5.Text = Strings.Status;
            this.materialLabel1.Text = Strings.Navigatetoanywebsitewherelistedgrouplinkstheclickbellowbellowbuton;
            this.materialButton1.Text = Strings.StartGrabbing;
            this.btnInitWA.Text = Strings.OpenBrowser;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (initStatusEnum != InitStatusEnum.Initialised)
            {
                Utils.showAlert(Strings.PleasefollowStepNo1FirstInitialiseWhatsapp, Alerts.Alert.enmType.Error);
                return;
            }
            if (campaignStatusEnum != CampaignStatusEnum.Running)
            {
                chatNames = new List<string>();
                var links = driver.FindElements(By.XPath("//a[contains(@href,'https://chat.whatsapp.com/')]"));
                int globalCounter = 0;
                foreach (var item in links)
                {
                    try
                    {
                        
                        string Link = item.GetAttribute("href").ToString();
                        chatNames.Add(Link);
                        globalCounter++;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (links.Count() == 0)
                {
                    Utils.showAlert(Strings.NoGroupLinkfoundincurrentPage, Alerts.Alert.enmType.Error);
                }
                else
                {
                    String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    String file = Path.Combine(FolderPath, "GroupLinks__" + Guid.NewGuid().ToString() + ".xlsx");
                    string NewFileName = file.ToString();

                    File.Copy("MemberListTemplate.xlsx", NewFileName, true);


                    var newFile = new FileInfo(NewFileName);
                    ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                    using (ExcelPackage xlPackage = new ExcelPackage(newFile))
                    {
                        var ws = xlPackage.Workbook.Worksheets[0];

                        for (int i = 0; i < chatNames.Count(); i++)
                        {
                            ws.Cells[i + 1, 1].Value = chatNames[i];
                        }
                        xlPackage.Save();
                    }

                    savesampleExceldialog.Filter = "Excel spreadsheet (*.xlsx)|*.xlsx|Comma-separated values file (*.csv)|*.csv";
                    savesampleExceldialog.FileName = "GroupLinks.xlsx";
                    if (savesampleExceldialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(NewFileName, savesampleExceldialog.FileName, true);
                        Utils.showAlert(Strings.Filedownloadedsuccessfully, Alerts.Alert.enmType.Success);
                    }
                }



            }
        }

        private void GrabGroupLinks_FormClosed(object sender, FormClosedEventArgs e)
        {

            try
            {
                driver.Quit();
            }
            catch (Exception ex)
            {

            }
            waSenderForm.formReturn(true);
        }


    }
}
