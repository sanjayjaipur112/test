using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WASender.Models;

namespace WASender
{
    public class Config
    {

        public static readonly string AppName = "WASender"; // Replace WaSender here to your own Software Name

        #region "dontchange"
        public static readonly string WaSenderFolderName = "WaSender";
        public static readonly string KeyMarkersFile = "KeyMarkers.txt";
        public static readonly string GeneralSettingsFile = "GeneralSettings.txt";
        public static readonly string BlockListFile = "BlockList.txt";
        public static readonly string SCParams = "SCParams.txt";
        public static readonly string FriendlyListFile = "FriendlyList.txt";
        public static readonly string ChromeProfileFolder = "ChromeProfile";
        public static readonly string ChromeProfileBotFolder = "ChromeProfileBot";
        public static readonly string ChromeProfileGMAPFolder = "ChromeProfileGMAP";
        public static readonly string ActivationFile = "Activation.txt";
        public static readonly string ProcessLoggerFolderName = "ProcessLogger";
        public static readonly string ErrorLoggerFolderName = "ErrorLogger";
        public static readonly string TempFolderName = "temp";
        public static readonly string MasterKeyName = "masterkey";
        public static readonly string SysFilesFolder = "SysFiles";
        public static readonly string ProfilesFolder = "Profiles";
        public static readonly string ChromeDriverFolder = SysFilesFolder;
        public static readonly string WAPIFolder = SysFilesFolder;
        #endregion

        public static readonly string PurchaseCode = "";

        public static readonly int SendingType = 1;
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }


        public static string GetSysFolderPath()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String returnableFolder = Path.Combine(WaSenderFolderpath, Config.SysFilesFolder);

            if (!Directory.Exists(returnableFolder))
            {
                Directory.CreateDirectory(returnableFolder);
            }



            if (!File.Exists(returnableFolder + "\\db.db"))
            {
                File.Copy("db.db", returnableFolder + "\\db.db");
            }

            if (!File.Exists(returnableFolder + "\\chatTemplate.csv"))
            {
                File.Copy("chatTemplate.csv", returnableFolder + "\\chatTemplate.csv", true);
            }
            else
            {
                DateTime ftime = File.GetLastWriteTime(returnableFolder + "\\chatTemplate.csv");
                DateTime ftime2 = File.GetLastWriteTime("chatTemplate.csv");
                if (ftime < ftime2)
                {
                    File.Copy("chatTemplate.csv", returnableFolder + "\\chatTemplate.csv", true);
                }
            }


            return returnableFolder;
        }
        public static string GetProfilesFolderPath()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String returnableFolder = Path.Combine(WaSenderFolderpath, Config.ProfilesFolder);

            if (!Directory.Exists(returnableFolder))
            {
                Directory.CreateDirectory(returnableFolder);
            }

            return returnableFolder;
        }

        public static string getSCParamsFile()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.SCParams);
            if (!File.Exists(keyMarkersTxtFilepath))
            {
                File.Create(keyMarkersTxtFilepath).Close();
            }


            return keyMarkersTxtFilepath;
        }

        public static string getBlocklistFile()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.BlockListFile);
            if (!File.Exists(keyMarkersTxtFilepath))
            {
                File.Create(keyMarkersTxtFilepath).Close();
            }


            return keyMarkersTxtFilepath;
        }

        public static string getFriendlylistFile()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.FriendlyListFile);
            if (!File.Exists(keyMarkersTxtFilepath))
            {
                File.Create(keyMarkersTxtFilepath).Close();
            }


            return keyMarkersTxtFilepath;
        }

        public static List<string> getAllBlockListes()
        {
            List<string> blocklists = new List<string>();
            try
            {
                string BlockListFilePath = Config.getBlocklistFile();

                string text = File.ReadAllText(BlockListFilePath);
                foreach (string item in text.Split('\n'))
                {
                    string number = item.Replace("\r", "");
                    blocklists.Add(number);
                }
            }
            catch (Exception ex)
            {
            }

            return blocklists;
        }
        public static GeneralSettingsModel GetSettings()
        {
            String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();
            GeneralSettingsModel generalSettingsModel;
            try
            {

                if (File.Exists(GetGeneralSettingsFilePath))
                {
                    string json = File.ReadAllText(GetGeneralSettingsFilePath);
                    generalSettingsModel = JsonConvert.DeserializeObject<GeneralSettingsModel>(json);
                    if (generalSettingsModel == null)
                    {
                        generalSettingsModel = new GeneralSettingsModel();
                    }
                    //generalSettingsModel.browserType = 2;
                    if (generalSettingsModel.browserType == 0)
                    {
                        generalSettingsModel.browserType = 2;
                    }
                    return generalSettingsModel;
                }
                else
                {
                    return new GeneralSettingsModel();
                }
            }
            catch (Exception ex)
            {
                return new GeneralSettingsModel();
            }
        }


        public static DateTime? getEndDate()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ActivationFile);
            if (File.Exists(keyMarkersTxtFilepath))
            {
                string DecodedText = File.ReadAllText(keyMarkersTxtFilepath);
                string encodedJson = Config.Base64Decode(DecodedText);

                try
                {
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<WASender.Models.ActivationModel>(encodedJson);

                    string keyCode = Base64Decode(obj.ActivationCode);
                    return obj.EndDate;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }

        public static bool IsProductActive()
        {
            try
            {
                return KeySecurity.KeySecurity.IsProductActive(Config.WaSenderFolderName, Config.ActivationFile);
            }
            catch (Exception ex)
            {
                Config.deActivateProduct();
                return false;
            }
            //String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            //String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            //if (!Directory.Exists(WaSenderFolderpath))
            //{
            //    Directory.CreateDirectory(WaSenderFolderpath);
            //}
            //String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ActivationFile);


            //if (File.Exists(keyMarkersTxtFilepath))
            //{


            //    string DecodedText = File.ReadAllText(keyMarkersTxtFilepath);
            //    string encodedJson = Config.Base64Decode(DecodedText);

            //    try
            //    {
            //        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<WASender.Models.ActivationModel>(encodedJson);

            //        string keyCode = Base64Decode(obj.ActivationCode);

            //        // if (Security.FingerPrint.Value() == keyCode || keyCode == "masterkey")
            //        {
            //            if (obj.EndDate <= DateTime.Now)
            //            {
            //                return false;
            //            }
            //            if (obj.purchasecode == "" || obj.purchasecode == null)
            //            {
            //                DateTime creation = File.GetCreationTime(keyMarkersTxtFilepath);
            //                var today = DateTime.Now;
            //                var diffOfDates = today - creation;


            //                if (diffOfDates.Days < 5)
            //                {
            //                    Config.deActivateProduct();
            //                    return false;
            //                }
            //            }

            //            string AppDataa = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //            WaSenderFolderpath = Path.Combine(AppDataa, Config.WaSenderFolderName);
            //            if (!Directory.Exists(WaSenderFolderpath))
            //            {
            //                Directory.CreateDirectory(WaSenderFolderpath);
            //            }
            //            string keyMarkersTxtFilepathSecond = Path.Combine(WaSenderFolderpath, Config.ActivationFile);
            //            if (!File.Exists(keyMarkersTxtFilepathSecond))
            //            {
            //                File.Copy(keyMarkersTxtFilepath, keyMarkersTxtFilepathSecond);
            //            }
            //            else
            //            {
            //                string firstFile, secondFile;

            //                firstFile = File.ReadAllText(keyMarkersTxtFilepath);
            //                secondFile = File.ReadAllText(keyMarkersTxtFilepathSecond);
            //                if (firstFile == secondFile)
            //                {
            //                    return true;
            //                }
            //                else
            //                {
            //                    return false;
            //                }
            //            }

            //            return true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //}
            //return false;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }


        public static string GetTempFolderPath()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String returnableFolder = Path.Combine(WaSenderFolderpath, Config.TempFolderName);

            if (!Directory.Exists(returnableFolder))
            {
                Directory.CreateDirectory(returnableFolder);
            }

            return returnableFolder;
        }

        //public static void KillChromeDriverProcess()
        //{
        //    if (Utils.Driver != null)
        //    {
        //        try
        //        {
        //            Utils.Driver.Close();
        //            Utils.Driver.Dispose();
        //            Utils.Driver.Quit();
        //            Utils.Driver = null;
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //    }
        //    foreach (var process in Process.GetProcessesByName("chromedriver"))
        //    {
        //        try
        //        {
        //            process.Kill();
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }


        //}

        public static string GetProcessLoggerFolderPath()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String returnableFolder = Path.Combine(WaSenderFolderpath, Config.ProcessLoggerFolderName);

            if (!Directory.Exists(returnableFolder))
            {
                Directory.CreateDirectory(returnableFolder);
            }

            return returnableFolder;
        }


        public static void deActivateProduct()
        {

            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ActivationFile);
            if (File.Exists(keyMarkersTxtFilepath))
            {
                File.Delete(keyMarkersTxtFilepath);
            }

            string AppDataa = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            WaSenderFolderpath = Path.Combine(AppDataa, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            string keyMarkersTxtFilepathSecond = Path.Combine(WaSenderFolderpath, Config.ActivationFile);
            if (File.Exists(keyMarkersTxtFilepathSecond))
            {
                File.Delete(keyMarkersTxtFilepathSecond);
            }
        }

        public static void ActivateProduct(string ActivationKey)
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ActivationFile);
            if (!File.Exists(keyMarkersTxtFilepath))
            {
                File.WriteAllText(keyMarkersTxtFilepath, ActivationKey);
            }
            else
            {
                File.Delete(keyMarkersTxtFilepath);
                File.WriteAllText(keyMarkersTxtFilepath, ActivationKey);
            }

            string AppDataa = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            WaSenderFolderpath = Path.Combine(AppDataa, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            string keyMarkersTxtFilepathSecond = Path.Combine(WaSenderFolderpath, Config.ActivationFile);
            //if (!File.Exists(keyMarkersTxtFilepathSecond))
            {
                File.Copy(keyMarkersTxtFilepath, keyMarkersTxtFilepathSecond, true);
            }
        }

        public static string GetKeyMarkersFilePath()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.KeyMarkersFile);
            return keyMarkersTxtFilepath;
        }

        public static string GetGeneralSettingsFilePath()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.GeneralSettingsFile);
            return keyMarkersTxtFilepath;
        }

        public static string GetChromeProfileFolder()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ChromeProfileFolder);
            if (!Directory.Exists(keyMarkersTxtFilepath))
            {
                Directory.CreateDirectory(keyMarkersTxtFilepath);
            }
            return keyMarkersTxtFilepath;
        }
        //ChromeDriverFolder


        public static string GetEdgeDriver()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ChromeDriverFolder);
            if (!Directory.Exists(keyMarkersTxtFilepath))
            {
                Directory.CreateDirectory(keyMarkersTxtFilepath);
            }
            if (!File.Exists(keyMarkersTxtFilepath + "\\msedgedriver.exe"))
            {
                File.Copy("msedgedriver.exe", keyMarkersTxtFilepath + "\\msedgedriver.exe");
            }
            else
            {
                DateTime ftime = File.GetLastWriteTime(keyMarkersTxtFilepath + "\\msedgedriver.exe");
                DateTime ftime2 = File.GetLastWriteTime("msedgedriver.exe");
                if (ftime < ftime2)
                {
                    File.Copy("msedgedriver.exe", keyMarkersTxtFilepath + "\\msedgedriver.exe", true);
                }
            }
            return keyMarkersTxtFilepath;
        }

        public static string GetChromeDriverFolder()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ChromeDriverFolder);
            if (!Directory.Exists(keyMarkersTxtFilepath))
            {
                Directory.CreateDirectory(keyMarkersTxtFilepath);
            }
            if (!File.Exists(keyMarkersTxtFilepath + "\\chromedriver.exe"))
            {
                File.Copy("chromedriver.exe", keyMarkersTxtFilepath + "\\chromedriver.exe");
            }
            else
            {
                DateTime ftime = File.GetLastWriteTime(keyMarkersTxtFilepath + "\\chromedriver.exe");
                DateTime ftime2 = File.GetLastWriteTime("chromedriver.exe");
                if (ftime < ftime2)
                {
                    File.Copy("chromedriver.exe", keyMarkersTxtFilepath + "\\chromedriver.exe", true);
                }
            }
            return keyMarkersTxtFilepath;
        }

        //WAPIFolder

        public static string WAPIFolderFolder()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ChromeDriverFolder);
            if (!Directory.Exists(keyMarkersTxtFilepath))
            {
                Directory.CreateDirectory(keyMarkersTxtFilepath);
            }
            if (!File.Exists(keyMarkersTxtFilepath + "\\chatTemplate.csv"))
            {
                File.Copy("chatTemplate.csv", keyMarkersTxtFilepath + "\\chatTemplate.csv", true);
            }
            else
            {
                DateTime ftime = File.GetLastWriteTime(keyMarkersTxtFilepath + "\\chatTemplate.csv");
                DateTime ftime2 = File.GetLastWriteTime("chatTemplate.csv");
                if (ftime < ftime2)
                {
                    File.Copy("chatTemplate.csv", keyMarkersTxtFilepath + "\\chatTemplate.csv", true);
                }
            }

            try
            {

                if (!File.Exists(keyMarkersTxtFilepath + "\\wapi.js"))
                {
                    File.Copy("wapi.js", keyMarkersTxtFilepath + "\\wapi.js", true);
                }
            }
            catch (Exception ex)
            {

            }
            return keyMarkersTxtFilepath;
        }

        public static string GetChromeProfileFolderBot()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ChromeProfileBotFolder);
            if (!Directory.Exists(keyMarkersTxtFilepath))
            {
                Directory.CreateDirectory(keyMarkersTxtFilepath);
            }
            return keyMarkersTxtFilepath;
        }

        public static string GetChromeProfileFolderGMAP()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }
            String keyMarkersTxtFilepath = Path.Combine(WaSenderFolderpath, Config.ChromeProfileGMAPFolder);
            if (!Directory.Exists(keyMarkersTxtFilepath))
            {
                Directory.CreateDirectory(keyMarkersTxtFilepath);
            }
            return keyMarkersTxtFilepath;
        }

        public static ChromeOptions GetChromeOptionsGMAP()
        {
            ChromeOptions options = new ChromeOptions();


            String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();

            if (File.Exists(GetGeneralSettingsFilePath))
            {
                string json = File.ReadAllText(GetGeneralSettingsFilePath);
                GeneralSettingsModel generalSettingsModel = JsonConvert.DeserializeObject<GeneralSettingsModel>(json);
                try
                {
                    if (generalSettingsModel.ChromeProfilePath != null && generalSettingsModel.ChromeProfilePath != "")
                    {
                        options.BinaryLocation = generalSettingsModel.ChromeProfilePath + "\\chrome.exe";
                    }
                }
                catch (Exception ex)
                {

                }
            }

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddArgument("user-data-dir=" + Config.GetChromeProfileFolderGMAP());
            return options;
        }
        public static ChromeOptions GetChromeOptionsBot()
        {
            ChromeOptions options = new ChromeOptions();


            String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();

            if (File.Exists(GetGeneralSettingsFilePath))
            {
                string json = File.ReadAllText(GetGeneralSettingsFilePath);
                GeneralSettingsModel generalSettingsModel = JsonConvert.DeserializeObject<GeneralSettingsModel>(json);
                try
                {
                    if (generalSettingsModel.ChromeProfilePath != null && generalSettingsModel.ChromeProfilePath != "")
                    {
                        options.BinaryLocation = generalSettingsModel.ChromeProfilePath + "\\chrome.exe";
                    }
                }
                catch (Exception ex)
                {

                }
            }

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddArgument("user-data-dir=" + Config.GetChromeProfileFolderBot());
            return options;
        }


        public static ChromeOptions GetChromeOptions()
        {
            ChromeOptions options = new ChromeOptions();


            String GetGeneralSettingsFilePath = Config.GetGeneralSettingsFilePath();

            if (File.Exists(GetGeneralSettingsFilePath))
            {
                string json = File.ReadAllText(GetGeneralSettingsFilePath);
                GeneralSettingsModel generalSettingsModel = JsonConvert.DeserializeObject<GeneralSettingsModel>(json);
                try
                {
                    if (generalSettingsModel.ChromeProfilePath != null && generalSettingsModel.ChromeProfilePath != "")
                    {
                        options.BinaryLocation = generalSettingsModel.ChromeProfilePath + "\\chrome.exe";
                    }
                }
                catch (Exception ex)
                {

                }
            }

            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalCapability("useAutomationExtension", false);
            options.AddArgument("user-data-dir=" + Config.GetChromeProfileFolder());
            return options;
        }



    }
}
