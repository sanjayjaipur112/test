using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WASender
{
    public partial class MainUC : UserControl
    {
        string profileName;
        string ProfileId;
        public bool _isWPPIJected { get; set; }
        public MainUC(string _ProfileId)
        {
            ProfileId = _ProfileId;
            InitializeComponent();
        }

        private void MainUC_Load(object sender, EventArgs e)
        {
            try
            {
                string ProfilesFolderPath = Config.GetProfilesFolderPath();   
                profileName = ProfilesFolderPath + "\\" + ProfileId;
                if (!Directory.Exists(profileName))
                {
                    Directory.CreateDirectory(profileName);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("compatible Webview2 Runtime"))
                {
                    MessageBox.Show(
                    Strings.YourComputerdonthaveCompatiblewebviewinstallation,
                    Strings.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    0,
                    "https://developer.microsoft.com/en-us/microsoft-edge/webview2/consumer/",
                    "");
                }
                else
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
            InitBrowser();
        }

        public async void InitBrowser()
        {
            await initizated();
            //var cookie = webView21.CoreWebView2.CookieManager.CreateCookie("wa_build", "w", ".web.whatsapp.com", "C:\\ProgramData\\WaSender\\SysFiles\\c");
            //cookie.IsSecure = false;
            //webView21.CoreWebView2.CookieManager.AddOrUpdateCookie(cookie);       
            webView21.CoreWebView2.Navigate("https://web.whatsapp.com/");
        }

        private async Task initizated()
        {
            try
            {
                webView21.CreationProperties = new Microsoft.Web.WebView2.WinForms.CoreWebView2CreationProperties();
                webView21.CreationProperties.UserDataFolder = profileName;
                await webView21.EnsureCoreWebView2Async(null);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("compatible Webview2 Runtime"))
                {
                    MessageBox.Show(
                    Strings.YourComputerdonthaveCompatiblewebviewinstallation,
                    Strings.Error,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    0,
                    "https://developer.microsoft.com/en-us/microsoft-edge/webview2/consumer/",
                    "");
                }
                else
                {
                    MessageBox.Show(ex.Message, "Error");
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                   Strings.YourComputerdonthaveCompatiblewebviewinstallation,
                   Strings.Error,
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Information,
                   MessageBoxDefaultButton.Button1,
                   0,
                   "https://developer.microsoft.com/en-us/microsoft-edge/webview2/consumer/",
                   "keyword");
        }

    }
}
