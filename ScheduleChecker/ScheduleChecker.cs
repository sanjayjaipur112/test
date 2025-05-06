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
using WASender;
using Newtonsoft.Json;
using Models;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace ScheduleChecker
{
    public partial class ScheduleChecker : MaterialForm
    {

        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupValue = "RUnONStartUpScheduleChecker";

        MaterialSkin.MaterialSkinManager materialSkinManager;
        List<SchedulesModel> schedules;
        Timer schedulerTimer;
        List<string> strings = new List<string>();


        public ScheduleChecker(string[] args)
        {
            InitializeComponent();

            try
            {
                if (args.Count() > 0)
                {
                    List<string> _params = JsonConvert.DeserializeObject<List<string>>(args[0]);
                    strings = _params;
                    string paramfiles = JsonConvert.SerializeObject(strings);
                    string SCParamFiles = Config.getSCParamsFile();
                    File.WriteAllText(SCParamFiles, paramfiles);
                }
                else
                {
                    bool isFIleCanRead = false;

                    try
                    {
                        string SCParamFiles = Config.getSCParamsFile();
                        string text = File.ReadAllText(SCParamFiles);
                        List<string> _param = JsonConvert.DeserializeObject<List<string>>(text);
                        if (_param.Count > 0)
                        {
                            strings = _param;
                            isFIleCanRead = true;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    if (isFIleCanRead == false)
                    {
                        strings.Add("Green");                                   //---0
                        strings.Add("Schedule Checker");                        //---1
                        strings.Add("Running");                                 //---2
                        strings.Add("Trying to run the Schedule, But ");        //---3
                        strings.Add("already running");                         //---4
                        strings.Add("I will stay here utill your all Schedules are completed");                         //---5
                        strings.Add("Exit");                                    //---6
                        strings.Add("Days");                                    //---7
                        strings.Add("Hours");                                   //---8
                        strings.Add("Minutes");                                 //---9
                        strings.Add("Next Schedule in");                         //---10
                        strings.Add("C:\\Program Files (x86)\\TrendingApps\\WaSenderSetUp");                         //---11
                        //strings.Add("Running");
                    }



                }
                materialSkinManager = Utils.SetColorScheme(materialSkinManager, this, strings[0]);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void initLanguages()
        {
            this.Text = Config.AppName + " - " + strings[1];
            notifyIcon1.Text = Config.AppName + " - " + strings[1];
            label1.Text = strings[2];
            label2.Text = "";
        }

        private void ScheduleChecker_Load(object sender, EventArgs e)
        {
            initLanguages();
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
            TrayMenuContext();
            checkforSchedules();



            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
                key.SetValue(StartupValue, Application.ExecutablePath.ToString());
            }
            catch (Exception ex)
            {
                try
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey(StartupKey, true);
                    key.SetValue(StartupValue, Application.ExecutablePath.ToString());
                }
                catch (Exception exx)
                {
                    MessageBox.Show("Error While setting the Schedule chaecker on STartUp, you may need to open " + Config.AppName + " after PC Restart to run the schedules", Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }




            try
            {
                int res = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);
                if (res == 0)
                {
                    notifyIcon1.Icon = Properties.Resources.icons8_schedule_Light;
                    this.Icon = Properties.Resources.icons8_schedule_Light;
                }
                else
                {
                    notifyIcon1.Icon = Properties.Resources.icons8_schedule_dark;
                    this.Icon = Properties.Resources.icons8_schedule_dark;
                }


            }
            catch (Exception ex)
            {

            }
        }

        private void TrayMenuContext()
        {
            this.notifyIcon1.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            this.notifyIcon1.ContextMenuStrip.Items.Add(strings[6], null, this.MenuTest1_Click);
        }

        bool fromnotifyIcon = false;
        void MenuTest1_Click(object sender, EventArgs e)
        {
            fromnotifyIcon = true;
            Application.Exit();
        }


        private void ScheduleChecker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!fromnotifyIcon)
            {
                e.Cancel = true;
                takeYourPosition();
            }


        }

        public void checkforSchedules()
        {
            startScheduleTimer();
        }

        private void startScheduleTimer()
        {
            schedulerTimer = new Timer();
            schedulerTimer.Interval = (1000);
            schedulerTimer.Tick += new EventHandler(schedulerTimer_Tick);
            schedulerTimer.Start();
        }

        bool WarningShowm = false;
        private void schedulerTimer_Tick(object sender, EventArgs e)
        {
            if (schedules == null)
            {
                schedules = WASender.PCUtils.checkSchedule();
            }

            List<SchedulesModel> Allpendings = schedules.Where(x => x.scheduleDatetime <= DateTime.Now && x.status == "PENDING").ToList();

            List<SchedulesModel> AllpendingsOnly = schedules.Where(x => x.scheduleDatetime > DateTime.Now && x.status == "PENDING").ToList();

            DateTime fileDate, closestDate;
            long min = long.MaxValue;
            fileDate = DateTime.Now;
            closestDate = DateTime.MinValue;


            foreach (SchedulesModel date in AllpendingsOnly)
            {
                if (Math.Abs(date.scheduleDatetime.Ticks - fileDate.Ticks) < min)
                {
                    min = Math.Abs(date.scheduleDatetime.Ticks - fileDate.Ticks);
                    closestDate = date.scheduleDatetime;
                }
            }

            if (closestDate > DateTime.MinValue)
            {
                DateTime today = DateTime.Now;
                var diffOfDates = closestDate - today;

                string nextScheduleText = "";
                if (Convert.ToInt32(diffOfDates.TotalDays) > 0)
                {
                    nextScheduleText = nextScheduleText + diffOfDates.Days.ToString() + " " + strings[7] + ", ";
                }
                if (Convert.ToInt32(diffOfDates.TotalHours) > 0)
                {
                    nextScheduleText = nextScheduleText + diffOfDates.Hours.ToString() + " " + strings[8] + ", ";
                }
                if (Convert.ToInt32(diffOfDates.Minutes) > 0)
                {
                    nextScheduleText = nextScheduleText + diffOfDates.Minutes.ToString() + " " + strings[9] + ", ";
                }
                label2.Text = strings[10] + " " + (nextScheduleText == "" ? "less than a Minute" : nextScheduleText);
            }
            if (Allpendings.Count() == 0)
            {
                WarningShowm = false;
            }

            if (Allpendings.Count() >= 1)
            {
                Process[] processes = Process.GetProcessesByName("WaSender");
                if (processes.Count() == 0)
                {
                    try
                    {
                        Allpendings.FirstOrDefault().status = "RUNNING";

                        string sParams = Allpendings.FirstOrDefault().Id;
                        string path = strings[11];
                        path = path.Replace("~", "\\");
                        //Process.Start(path + "\\WaSender.exe", sParams);

                        Process process = new Process()
                        {
                            StartInfo = new ProcessStartInfo(path + "\\WaSender.exe", sParams)
                            {
                                WindowStyle = ProcessWindowStyle.Normal,
                                WorkingDirectory = path,
                                //UseShellExecute =true,
                                //Verb = "runas",
                            }
                        };

                        process.Start();

                        label2.Text = "";
                    }
                    catch (Exception ex)
                    {
                        StringBuilder sb = new StringBuilder();
                        // flush every 20 seconds as you do it
                        //File.AppendAllText("D:\\ttttmp\\vCard\\log.txt", ex.Message + "------" + ex.StackTrace);
                        sb.Clear();
                        MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace, Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (WarningShowm == false)
                    {
                        WarningShowm = true;
                        MessageBox.Show(strings[3] + " " + Config.AppName + "  " + strings[4], Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }

            if (Allpendings.Count == 0 && AllpendingsOnly.Count == 0)
            {
                fromnotifyIcon = true;
                Application.Exit();
            }



        }

        private void takeYourPosition()
        {
            this.Hide();
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipText = strings[5];
            notifyIcon1.BalloonTipTitle = Config.AppName;
            notifyIcon1.ShowBalloonTip(1000);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void ScheduleChecker_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                takeYourPosition();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<SchedulesModel> AllpendingsOnly = schedules.Where(x => x.scheduleDatetime > DateTime.Now && x.status == "PENDING").ToList();
            if (AllpendingsOnly.Count() >= 1)
            {
                Process[] processes = Process.GetProcessesByName("WaSender");
                if (processes.Count() == 0)
                {
                    try
                    {
                        AllpendingsOnly.FirstOrDefault().status = "RUNNING";

                        string sParams = AllpendingsOnly.FirstOrDefault().Id;
                        Process.Start("WaSender.exe", sParams);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace, Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (WarningShowm == false)
                    {
                        WarningShowm = true;
                        MessageBox.Show(strings[3] + " " + Config.AppName + "  " + strings[4], Config.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }
    }
}
