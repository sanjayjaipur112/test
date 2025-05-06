using Models;
using Newtonsoft.Json;
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
using WASender.Models;

namespace WASender
{
    public partial class ScheduleSingle : MyMaterialPopOp
    {
        WASenderSingleTransModel wASenderSingleTransModel;
        WASenderGroupTransModel wASenderGroupTransModel;
        SingleLauncher singleLauncher;
        WaSenderForm waSenderForm;
        string jsonString;
        string scheduleId = null;
        string scheduleName = "";
        GroupLauncher groupLauncher;
        SchedulesModel schedulesModel;
        public ScheduleSingle(WASenderSingleTransModel _wASenderSingleTransModel, SingleLauncher _singleLauncher, WaSenderForm _waSenderForm, string _scheduleId = null, string _scheduleName = null)
        {
            InitializeComponent();
            wASenderSingleTransModel = _wASenderSingleTransModel;
            scheduleId = _scheduleId;
            scheduleName = _scheduleName;
            singleLauncher = _singleLauncher;
            waSenderForm = _waSenderForm;
            this.Icon = Strings.AppIcon;
            wASenderSingleTransModel.generalSettingsModel = Config.GetSettings();
            this.materialTextBox21.Text = scheduleName;
        }
        public ScheduleSingle(WASenderGroupTransModel _wASenderGroupTransModel, GroupLauncher _groupLauncher, WaSenderForm _waSenderForm, string _scheduleId = null, string _scheduleName = null)
        {
            InitializeComponent();
            wASenderGroupTransModel = _wASenderGroupTransModel;
            this.Icon = Strings.AppIcon;
            waSenderForm = _waSenderForm;
            scheduleId = _scheduleId;
            scheduleName = _scheduleName;
            this.materialTextBox21.Text = scheduleName;
        }


        private void ScheduleSingle_Load(object sender, EventArgs e)
        {
            initLanguages();
            if (scheduleName != null)
            {
                materialTextBox21.Text = scheduleName;
            }
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy/MM/dd HH:mm";
        }

        private void initLanguages()
        {
            this.Text = Strings.ScheduleCampaign;
            label3.Text = Strings.MakesureyourPCwillbeturnONattheScheduleDateandtime;
            materialTextBox21.Hint = Strings.ScheduleName;
            materialLabel1.Text = Strings.ScheduleDateandTime;
            label1.Text = Strings.timein24Hoursformat;
            materialButton1.Text = Strings.ScheduleNow;
            label2.Text = Strings.PleaseNote;
            label4.Text = Strings.Makesure + " " + Config.AppName + " " + Strings.isnotRunningonScheduletime;
            label5.Text = Strings.PleasemaintainTenminutesgapbetweenschedules;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            List<SchedulesModel> allSchedules = PCUtils.checkSchedule();
            var datetime = dateTimePicker1.Value;
            bool isEarly = false;
            string ScheduleName="";
            string scheduleTime="";
            

            foreach (var item in allSchedules.Where(x=>x.Id !=this.scheduleId).ToList())
            {
                var diffOfDates = datetime - item.scheduleDatetime;
                if (diffOfDates.TotalMinutes < 10 && diffOfDates.TotalMinutes > 0)
                {
                    isEarly = true;
                }
                ScheduleName=item.ScheduleName;
                scheduleTime = item.scheduleDatetime.ToShortDateString() + " : " + item.scheduleDatetime.ToShortTimeString();
            }

            if (isEarly == true)
            {
               Utils.showAlert(Strings.PleasemaintainTenminutesgapbetweenschedules + "\n "+Strings.Schedule+" '" + ScheduleName + "' - @ " + scheduleTime, Alerts.Alert.enmType.Error);
                return;
            }
            if (materialTextBox21.Text == "")
            {
                Utils.showAlert(Strings.Pleasegiveanyname, Alerts.Alert.enmType.Error);
                return;
            }
            
            if (datetime < DateTime.Now)
            {
                Utils.showAlert(Strings.Dateandtimeisnotvalid, Alerts.Alert.enmType.Error);
                return;
            }

            string ScheduleType = "";

            if (wASenderSingleTransModel != null)
            {
                jsonString = JsonConvert.SerializeObject(wASenderSingleTransModel);
                ScheduleType = "SINGLE";
            }

            if (wASenderGroupTransModel != null)
            {
                jsonString = JsonConvert.SerializeObject(wASenderGroupTransModel);
                ScheduleType = "GROUP";
            }
            
            if (scheduleId != null)
            {
                new SqLiteBaseRepository().UpdateSchedule(scheduleId, materialTextBox21.Text, datetime, jsonString, ScheduleType);
                MessageBox.Show(Strings.ScheduleUpdatedSuccessfully, Strings.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                waSenderForm.scheduleAdded(true);
                this.Close();

            }
            else
            {
                new SqLiteBaseRepository().InsertSchedule(materialTextBox21.Text, datetime, jsonString, ScheduleType);
                MessageBox.Show(Strings.CampaignScheduledSuccessfully, Strings.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                waSenderForm.scheduleAdded(true);
                this.Close();
            }
        }
    }
}
