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
    public  class _Config
    {
      
        public static void KillChromeDriverProcess()
        {
            if (Utils.Driver != null)
            {
                try
                {
                    Utils.Driver.Close();
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


        }

        public static AISettingsModel GetAISettings()
        {
            try
            {
                String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
                String settingsFilePath = Path.Combine(WaSenderFolderpath, "ai_settings.json");
                
                if (!File.Exists(settingsFilePath))
                {
                    return new AISettingsModel();
                }
                
                string json = File.ReadAllText(settingsFilePath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<AISettingsModel>(json);
            }
            catch (Exception)
            {
                return new AISettingsModel();
            }
        }

        public static void SaveAISettings(AISettingsModel settings)
        {
            try
            {
                String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                String WaSenderFolderpath = Path.Combine(FolderPath, Config.WaSenderFolderName);
                
                if (!Directory.Exists(WaSenderFolderpath))
                {
                    Directory.CreateDirectory(WaSenderFolderpath);
                }
                
                String settingsFilePath = Path.Combine(WaSenderFolderpath, "ai_settings.json");
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
                File.WriteAllText(settingsFilePath, json);
            }
            catch (Exception ex)
            {
                Logger.WriteLog("Error saving AI settings: " + ex.Message);
            }
        }
    }
}
