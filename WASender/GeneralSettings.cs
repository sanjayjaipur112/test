using MaterialSkin.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;
using WASender.Models;
using System.Web.Script.Serialization;
using System.Collections;
using System.Net;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
namespace WASender
{
    public partial class GeneralSettings : MyMaterialPopOp
    {
        WaSenderForm waSenderForm;
        GeneralSettingsModel generalSettingsModel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        Progressbar pgbar;
        public GeneralSettings(WaSenderForm _waSenderForm)
        {
            this.waSenderForm = _waSenderForm;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            init();
            initLanguages();
        }

        public GeneralSettings()
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            init();
            initLanguages();
        }

        private void initLanguages()
        {
            this.Text = Strings.GeneralSettings;
            materialButton2.Text = Strings.BlockList;
            btnSave.Text = Strings.Save;
            materialButton6.Text = Strings.CheckforInternalUpdate;

            materialButton5.Text = Strings.clearsessions;
            materialButton3.Text = Strings.ClearProfileCache;
            materialButton4.Text = Strings.ClearBOTCache;
            txtChromePath.Hint = Strings.ChromeEXEpath;

            materialButton7.Text = Strings.AboutLicence;
            materialButton10.Text = Strings.UpdateChromeDriver;
            
            materialCheckbox2.Text = Strings.Dontlinkpreviewwhilesendingmessage;
            materialCheckbox3.Text = Strings.FilterNumbersBeforeSendingMessages;
            materialLabel1.Text = Strings.Browser;
        }

        private void init()
        {
            getData();
        }

        private void getData()
        {

            Dictionary<string, string> test = new Dictionary<string, string>();
            test.Add("1", "Chrome");
            test.Add("2", "Built In Browser");

            materialComboBox1.DataSource = new BindingSource(test, null);
            materialComboBox1.DisplayMember = "Value";
            materialComboBox1.ValueMember = "Key";

            String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();

            if (File.Exists(GetGeneralSettingsFilePath))
            {
                string json = File.ReadAllText(GetGeneralSettingsFilePath);
                generalSettingsModel = JsonConvert.DeserializeObject<GeneralSettingsModel>(json);
                if (generalSettingsModel == null)
                {
                    generalSettingsModel = new GeneralSettingsModel();
                    generalSettingsModel.filterNumbersBeforeSendingMessage = false;
                }
                if (generalSettingsModel.ChromeProfilePath != null && generalSettingsModel.ChromeProfilePath != "")
                {
                    txtChromePath.Text = generalSettingsModel.ChromeProfilePath;
                }
                //if (generalSettingsModel.CheckNumberBeforeSending != null)
                //{
                //    materialCheckbox1.Checked = generalSettingsModel.CheckNumberBeforeSending;
                //}
                if (generalSettingsModel.filterNumbersBeforeSendingMessage != null)
                {
                    //MessageBox.Show(generalSettingsModel.filterNumbersBeforeSendingMessage.ToString());
                    materialCheckbox3.Checked = generalSettingsModel.filterNumbersBeforeSendingMessage;
                }
                if (generalSettingsModel.browserType == 0)
                {
                    generalSettingsModel.browserType = 2;
                }
                if (generalSettingsModel.disableLinkPreview != null)
                {
                    materialCheckbox2.Checked = generalSettingsModel.disableLinkPreview;
                }

                materialComboBox1.SelectedValue = generalSettingsModel.browserType.ToString();

            }


        }

        private void GeneralSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void materialButton15_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();

            if (!File.Exists(GetGeneralSettingsFilePath))
            {
                File.Create(GetGeneralSettingsFilePath).Close();
            }


            if (this.txtChromePath.Text != "")
            {

                if (!Directory.Exists(txtChromePath.Text))
                {
                    MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.InputPathisnotCorrectfolderpath, Strings.OK, true);
                    SnackBarMessage.Show(this);
                    return;
                }
            }
            generalSettingsModel.filterNumbersBeforeSendingMessage = materialCheckbox3.Checked;
            generalSettingsModel.ChromeProfilePath = txtChromePath.Text == null ? "" : txtChromePath.Text;
            generalSettingsModel.browserType = Convert.ToInt32(materialComboBox1.SelectedValue);
            generalSettingsModel.disableLinkPreview = materialCheckbox2.Checked;
            string Json = JsonConvert.SerializeObject(generalSettingsModel, Formatting.Indented);

            File.WriteAllText(GetGeneralSettingsFilePath, Json);

            MaterialSnackBar SnackBarMessage1 = new MaterialSnackBar(Strings.SettingsSavedSuccessfully, Strings.OK, true);
            SnackBarMessage1.Show(this);
            if (this.waSenderForm != null)
            {
                this.waSenderForm.checkBrowserType();
            }
        }


        private void materialButton2_Click(object sender, EventArgs e)
        {
            BlockList blockList = new BlockList();
            blockList.ShowDialog();

        }

        private void GeneralSettings_Load(object sender, EventArgs e)
        {
          
        }
       
       
        private void ClearCHromeSessions()
        {
            if (Utils.Driver != null)
            {
                try
                {

                    Utils.Driver.Dispose();
                    Utils.Driver.Quit();
                    Utils.Driver = null;
                }
                catch (Exception ex)
                {


                }
            }
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {

                }

            }
            if (Utils.waSenderBrowser != null)
            {
                Utils.waSenderBrowser.Close();
                Utils.waSenderBrowser = null;
            }
        }
        private void materialButton5_Click_1(object sender, EventArgs e)
        {
            ClearCHromeSessions();
            MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Done 👍👍👍👍", Strings.OK, true);
            SnackBarMessage.Show(this);
        }


        private void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(directory);
                }
                Directory.Delete(path);
            }
        }
        private void materialButton3_Click_1(object sender, EventArgs e)
        {
            try
            {
                string ProfileFolderpath = Config.GetChromeProfileFolder();
                DeleteDirectory(ProfileFolderpath);
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Done 👍👍👍👍", Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            catch (Exception exc)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Not Done 👎👎 -" + exc.Message, Strings.OK, true);
                SnackBarMessage.Show(this);
            }
        }

        private void materialButton4_Click_1(object sender, EventArgs e)
        {
            try
            {
                string ProfileFolderpath = Config.GetChromeProfileFolderBot();
                DeleteDirectory(ProfileFolderpath);
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Done 👍👍👍👍", Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            catch (Exception exc)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Not Done 👎👎 -" + exc.Message, Strings.OK, true);
                SnackBarMessage.Show(this);
            }

        }

        private void materialButton6_Click(object sender, EventArgs e)
        {
            try
            {
                string UpdateFIle = Config.GetTempFolderPath() + "\\" + Guid.NewGuid().ToString() + ".zip";
                string extractPath = Config.WAPIFolderFolder();

                WebClient webClient = new WebClient();
                webClient.DownloadFile("http://shivjagar.in/update.zip", UpdateFIle);

                var archive = ZipFile.Open(UpdateFIle, ZipArchiveMode.Read);

                DirectoryInfo di = Directory.CreateDirectory(extractPath);
                string destinationDirectoryFullPath = di.FullName;

                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string completeFileName = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, file.FullName));

                    if (!completeFileName.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new IOException("Trying to extract file outside of destination directory. See this link for more info: https://snyk.io/research/zip-slip-vulnerability");
                    }

                    if (file.Name == "")
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                        continue;
                    }
                    file.ExtractToFile(completeFileName, true);

                    try
                    {
                        string readText = File.ReadAllText(completeFileName);
                        File.WriteAllText(completeFileName, readText + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }

                archive.Dispose();
                File.Delete(UpdateFIle);
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Done 👍👍👍👍", Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Not Done 👎👎 -" + ex.Message);
            }
        }

        public void ProductDeActivated()
        {
            if (this.waSenderForm != null)
            {
                this.Hide();
                this.waSenderForm.CHeckForActivation();

            }
        }
        private void materialButton7_Click(object sender, EventArgs e)
        {
            About about = new About(this);
            about.ShowDialog();
        }

      

        public chromeDriverresultModel EdgeDriverUpdate()
        {
            chromeDriverresultModel _chromeDriverresultModel = new chromeDriverresultModel();
            {
                try
                {
                    
                    #region edgeVersion
                    string edgeVersion = string.Empty;
                    const string edgeRegistryKey = @"SOFTWARE\Microsoft\Edge\BLBeacon";
                    const string edgeRegistryValue = "version";

                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(edgeRegistryKey))
                    {
                        if (key != null)
                        {
                            edgeVersion = key.GetValue(edgeRegistryValue).ToString();
                        }
                    }
                    #endregion

                    var chromeDrvPath = Config.GetChromeDriverFolder() + "\\msedgedriver.exe";
                    if (!string.IsNullOrEmpty(edgeVersion))
                    {
                        string chromversion = edgeVersion;
                        chromversion = chromversion.Split('.')[0];

                        WebClient webClient = new WebClient();
                        webClient.DownloadFile("http://shivjagar.in/edgedriver/" + chromversion + ".exe", chromeDrvPath);

                        _chromeDriverresultModel.isDone = true;
                        _chromeDriverresultModel.message = "Done 👍👍👍👍";
                    }
                    else
                    {
                        string message = "It seems that you do not have Google Chrome installed in your Program Files in C Drive. This software requires the latest version of Google Chrome installed.";
                        _chromeDriverresultModel.isDone = false;
                        _chromeDriverresultModel.message = message;
                    }
                }
                catch (Exception ex)
                {

                    try
                    {
                        #region edgeVersion
                        string edgeVersion = string.Empty;
                        const string edgeRegistryKey = @"SOFTWARE\Microsoft\Edge\BLBeacon";
                        const string edgeRegistryValue = "version";

                        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(edgeRegistryKey))
                        {
                            if (key != null)
                            {
                                edgeVersion = key.GetValue(edgeRegistryValue).ToString();
                            }
                        }
                        #endregion
                        var isdownloadchrome = false;
                        var chromeDrvPath = Config.GetChromeDriverFolder() + "\\msedgedriver.exe";
                        System.IO.FileInfo fi = new System.IO.FileInfo(chromeDrvPath);
                        string chromePath = System.Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", null, null));
                        if (!string.IsNullOrEmpty(chromePath))
                        {
                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                            string chromversion = FileVersionInfo.GetVersionInfo(chromePath).FileVersion;
                            chromversion = chromversion.Split('.')[0];

                            WebClient webClient = new WebClient();
                            webClient.DownloadFile("https://github.com/bracketsApps007/NewRepo/releases/download/1.0.0/ms" + chromversion + ".exe", chromeDrvPath);

                            _chromeDriverresultModel.isDone = true;
                            _chromeDriverresultModel.message = "Done 👍👍👍👍";
                        }
                        else
                        {
                            string message = "It seems that you do not have Google Chrome installed in your Program Files in C Drive. This software requires the latest version of Google Chrome installed.";
                            _chromeDriverresultModel.isDone = false;
                            _chromeDriverresultModel.message = message;
                        }
                    }
                    catch (Exception eex)
                    {
                        _chromeDriverresultModel.isDone = false;
                        _chromeDriverresultModel.message = ex.Message;
                    }

                    
                }
            }

            return _chromeDriverresultModel;
        }


        public async Task<chromeDriverresultModel> ChromeDriverUpdate()
        {
            chromeDriverresultModel _chromeDriverresultModel = new chromeDriverresultModel();
            {
                bool useSecondMethod = false;
                try
                {
                    var chromeDrvPath = Config.GetChromeDriverFolder() + "\\chromedriver.exe";
                    System.IO.FileInfo fi = new System.IO.FileInfo(chromeDrvPath);
                    string chromePath = System.Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", null/* TODO Change to default(_) if this is not a reference type */, null/* TODO Change to default(_) if this is not a reference type */));
                    if (!string.IsNullOrEmpty(chromePath))
                    {
                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        string chromversion = FileVersionInfo.GetVersionInfo(chromePath).FileVersion;
                        chromversion = chromversion.Split('.')[0];

                        Uri u = new Uri("https://github.com/bracketsApps007/NewRepo/releases/download/1.0.0/" + chromversion + ".exe");
                        WebClient webClient = new WebClient();
                        await webClient.DownloadFileTaskAsync(u, chromeDrvPath);

                        _chromeDriverresultModel.isDone = true;
                        _chromeDriverresultModel.message = "Done 👍👍👍👍";
                    }
                    else
                    {
                         var chromeVersiontxt= Microsoft.VisualBasic.Interaction.InputBox("What Is Your Chrome Version ?", "Chrome Version", "");
                         try
                         {
                             ServicePointManager.Expect100Continue = true;
                             ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                             int chromversion = Convert.ToInt32(chromeVersiontxt);
                             WebClient webClient = new WebClient();

                             Uri u = new Uri("https://github.com/bracketsApps007/NewRepo/releases/download/1.0.0/" + chromversion + ".exe");

                             await webClient.DownloadFileTaskAsync(u, chromeDrvPath);

                             _chromeDriverresultModel.isDone = true;
                             _chromeDriverresultModel.message = "Done 👍👍👍👍";
                         }
                         catch (Exception ex)
                         {
                             _chromeDriverresultModel.isDone = false;
                             _chromeDriverresultModel.message = "Not Done 👎👎 ! Version  " + chromeVersiontxt.ToString() + " Not found !";
                         }
                    }

                    
                }
                catch (Exception exx)
                {
                    useSecondMethod = true;

                }

                if (useSecondMethod)
                {
                    try
                    {
                        var chromeDrvPath = Config.GetChromeDriverFolder() + "\\chromedriver.exe";
                        System.IO.FileInfo fi = new System.IO.FileInfo(chromeDrvPath);
                        string chromePath = System.Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", null, null));
                        if (!string.IsNullOrEmpty(chromePath))
                        {
                            string chromversion = FileVersionInfo.GetVersionInfo(chromePath).FileVersion;
                            chromversion = chromversion.Split('.')[0];

                            WebClient webClient = new WebClient();
                            await webClient.DownloadFileTaskAsync("http://shivjagar.in/driver/" + chromversion + ".exe", chromeDrvPath);
                            _chromeDriverresultModel.isDone = true;
                            _chromeDriverresultModel.message = "Done 👍👍👍👍";
                        }
                        else
                        {
                            string message = "It seems that you do not have Google Chrome installed in your Program Files in C Drive. This software requires the latest version of Google Chrome installed.";
                            _chromeDriverresultModel.isDone = false;
                            _chromeDriverresultModel.message = message;
                        }
                    }
                    catch (Exception ex)
                    {
                        _chromeDriverresultModel.isDone = false;
                        _chromeDriverresultModel.message = ex.Message;
                    }
                }
            }
            return _chromeDriverresultModel;

        }

        private async void materialButton10_Click(object sender, EventArgs e)
        {

            try
            {
                ClearCHromeSessions();
                pgbar = new Progressbar();
                pgbar.Show();

                chromeDriverresultModel mode =await ChromeDriverUpdate();

                pgbar.Hide();
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar(mode.message, Strings.OK, true);
                SnackBarMessage.Show(this); 
            }
            catch (Exception ex)
            {

            }
        }

     
        
       

    }


    public class chromeDriverresultModel
    {
        public bool isDone { get; set; }
        public string message { get; set; }
    }
}
