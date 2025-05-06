using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WASender.Models;

namespace WASender
{
    public static class Utils
    {

        private static OpenQA.Selenium.IWebDriver _driver;
        private static WaSenderBrowser _waSenderBrowser { get; set; }

        private static TestClass _TestClass;
        public static WASender.Model.ColorSchemeenum? currentColorscheme;

        public static TestClass testClass
        {
            get
            {
                if (_TestClass == null)
                    _TestClass = new TestClass();

                return _TestClass;
            }
            set { _TestClass = value; }
        }


        public static WaSenderBrowser waSenderBrowser
        {
            get
            {
                return _waSenderBrowser;
            }
            set { _waSenderBrowser = value; }
        }

        public static OpenQA.Selenium.IWebDriver Driver
        {
            get
            {
                return _driver;
            }
            set { _driver = value; }
        }


        public static TabPage GetTabPageById(WaSenderBrowser browser, string id)
        {
            TabPage tp = null;
            foreach (TabPage page in browser.tabControl1.TabPages)
            {
                var _tag = (System.Data.DataRow)page.Tag;
                if (_tag["ID"].ToString() == id)
                {
                    tp = page;
                }
            }
            return tp;
        }

        public static void showError(string messgae)
        {
            MessageBox.Show(messgae, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string sanitizeNumber(string number)
        {
            string newNumber = number.Replace(" ","").Replace(Environment.NewLine,"").Replace("+","").Trim().Replace("-","");
            return newNumber;
        }

        public static WebView2 GetWebViewById(WaSenderBrowser browser, string id)
        {
            WebView2 vw=null;
            foreach (TabPage page in browser.tabControl1.TabPages)
            {
                var _tag = (System.Data.DataRow)page.Tag;
                if (_tag["ID"].ToString() == id)
                {
                    vw = (WebView2)page.Controls.Find("webView21", true)[0];
                    
                }
            }
            return vw;
        }

        public static MainUC GetMainUCId(WaSenderBrowser browser, string id)
        {
            MainUC vw = null;
            foreach (TabPage page in browser.tabControl1.TabPages)
            {
                var _tag = (System.Data.DataRow)page.Tag;
                if (_tag["ID"].ToString() == id)
                {
                    vw = (MainUC)page.Controls.Find("MainUC", true)[0];

                }
            }
            return vw;
        }


        public static WebView2 GetActiveWebView(WaSenderBrowser browser)
        {
            try
            {
                int index = browser.tabControl1.SelectedIndex;
                string tabName = browser.tabControl1.TabPages[index].Name;
                TabPage tp = browser.tabControl1.SelectedTab;
                WebView2 vw = (WebView2)tp.Controls.Find("webView21", true)[0];

                return vw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SetDriver()
        {
            if (_driver == null)
            {

                var chromeDriverService = OpenQA.Selenium.Chrome.ChromeDriverService.CreateDefaultService(Config.GetChromeDriverFolder());
                chromeDriverService.HideCommandPromptWindow = true;

                _driver = new OpenQA.Selenium.Chrome.ChromeDriver(chromeDriverService, Config.GetChromeOptions());
                try
                {
                    _driver.Url = "https://web.whatsapp.com";
                }
                catch (Exception ex)
                {

                }
            }

        }


        public static void showAlert(string message, Alerts.Alert.enmType alertType)
        {
            Alerts.Alert alert = new Alerts.Alert();
            alert.showAlert(message, alertType);
        }

        public static string ExtractBetweenTwoStrings(string FullText, string StartString, string EndString, bool IncludeStartString, bool IncludeEndString)
        {
            try
            {
                int Pos1 = FullText.IndexOf(StartString) + StartString.Length;
                int Pos2 = FullText.IndexOf(EndString, Pos1);
                return ((IncludeStartString) ? StartString : "") + FullText.Substring(Pos1, Pos2 - Pos1) + ((IncludeEndString) ? EndString : "");
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static int getRandom(int min, int max)
        {
            try
            {
                int delay = new Random().Next(min, max);
                return delay;
            }
            catch (Exception ex)
            {

                return min;
            }
        }


        public static string SanitizeNumber(this string number)
        {
            number = number.Replace("+", "");
            number = number.Replace(" ", "");
            number = number.Replace(",", "");
            number = number.Replace("-", "");
            number = number.Replace("(", "");
            number = number.Replace(")", "");
            number = Regex.Replace(number, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
            return number;
        }

        public static string exactDatetime()
        {
            string date = "";

            var datr = DateTime.Now;

            date = datr.Year + "-" + datr.Month + "-" + datr.Day + " " + datr.Hour + ":" + datr.Minute + ":" + datr.Second;

            return date;
        }
        public static void setTooltiop(Control control, string caption)
        {
            System.Windows.Forms.ToolTip ToolTip1 = new System.Windows.Forms.ToolTip();
            ToolTip1.BackColor = System.Drawing.Color.Black;
            ToolTip1.ForeColor = System.Drawing.Color.Wheat;
            ToolTip1.SetToolTip(control, caption);
        }

        public static void selectFileForMessage(MaterialListView lstView)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Strings.SelectExcel;
            openFileDialog.DefaultExt = "";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog.FileName;
                string[] itemsss = new string[] { file, "[No Caption]" };
                ListViewItem i = new ListViewItem(itemsss);
                lstView.Items.Add(i);
            }
        }

        public static void selectFileForMessage(DataGridView lstView)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = Strings.SelectExcel;
            openFileDialog.DefaultExt = "";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in openFileDialog.FileNames)
                {
                    long length = new System.IO.FileInfo(file).Length;
                    double fileSize = (length / 1024f) / 1024f;

                    if (fileSize > 12)
                    {
                        MessageBox.Show(Strings.YouraretryingtosendLargefile, Strings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    string[] itemsss = new string[] { file, "[No Caption]" };
                    lstView.Rows.Add(file, "");
                }
                
            }
        }

        public static void removeListViewItem(KeyEventArgs e, MaterialListView lstView)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var removables = new List<ListViewItem>();
                foreach (ListViewItem item in lstView.SelectedItems)
                {
                    removables.Add(item);
                }
                foreach (var item in removables)
                {
                    lstView.Items.Remove(item);
                }
            }
        }

        public static MaterialSkin.MaterialSkinManager SetColorScheme(MaterialSkin.MaterialSkinManager materialSkinManager, MaterialForm materialForm, Model.ColorSchemeenum? colorSchemeenum = null)
        {
            currentColorscheme = colorSchemeenum;
            
            materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(materialForm);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
            if (colorSchemeenum == null)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Green700, Primary.Green400, Primary.Green900, Accent.Green700, TextShade.WHITE);
                return materialSkinManager;
            }
            if (colorSchemeenum == Model.ColorSchemeenum.Green)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Green700, Primary.Green400, Primary.Green900, Accent.Green700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Red)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Red700, Primary.Red400, Primary.Red900, Accent.Red700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Amber)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Amber700, Primary.Amber400, Primary.Amber900, Accent.Amber700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Blue)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue700, Primary.Blue400, Primary.Blue900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.BlueGrey)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey700, Primary.BlueGrey400, Primary.BlueGrey900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Brown)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Brown700, Primary.Brown400, Primary.Brown900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Cyan)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Cyan700, Primary.Cyan400, Primary.Cyan900, Accent.Cyan700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.DeepOrange)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepOrange700, Primary.DeepOrange400, Primary.DeepOrange900, Accent.DeepOrange700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.DeepPurple)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.DeepPurple700, Primary.DeepPurple400, Primary.DeepPurple900, Accent.DeepPurple700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Grey)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey700, Primary.Grey400, Primary.Grey900, Accent.Blue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Indigo)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo700, Primary.Indigo400, Primary.Indigo900, Accent.Indigo700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.LightBlue)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue700, Primary.LightBlue400, Primary.LightBlue900, Accent.LightBlue700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Lime)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Lime700, Primary.Lime400, Primary.Lime900, Accent.Lime700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Orange)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Orange700, Primary.Orange400, Primary.Orange900, Accent.Orange700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Pink)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Pink700, Primary.Pink400, Primary.Pink900, Accent.Pink700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Purple)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Purple700, Primary.Purple400, Primary.Purple900, Accent.Purple700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Teal)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Teal700, Primary.Teal400, Primary.Teal900, Accent.Teal700, TextShade.WHITE);
            }
            else if (colorSchemeenum == Model.ColorSchemeenum.Yellow)
            {
                materialSkinManager.ColorScheme = new ColorScheme(Primary.Yellow700, Primary.Yellow400, Primary.Yellow900, Accent.Yellow700, TextShade.WHITE);
            }

            return materialSkinManager;
        }

    }
}
