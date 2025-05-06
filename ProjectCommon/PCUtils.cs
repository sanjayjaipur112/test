using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WASender;

namespace WASender
{
    public class PCUtils
    {
        

        public static SchedulesModel getScheduleById(String Id)
        {
            SchedulesModel scheduleModel = new SchedulesModel();
            DataTable dt = new SqLiteBaseRepository().getSchedulesById(Id);

            var item = dt.Rows[0];

            scheduleModel.Id = item["Id"].ToString();

            scheduleModel.ScheduleName = item["ScheduleName"].ToString();
            scheduleModel.JsonString = item["JsonString"].ToString();
            scheduleModel.day = Convert.ToInt32(item["day"].ToString());
            scheduleModel.year = Convert.ToInt32(item["year"].ToString());
            scheduleModel.month = Convert.ToInt32(item["month"].ToString());
            scheduleModel.hour = Convert.ToInt32(item["hour"].ToString());
            scheduleModel.minutes = Convert.ToInt32(item["minutes"].ToString());
            scheduleModel.status = item["status"].ToString();
            scheduleModel.Type = item["Type"].ToString();
            scheduleModel.result = item["result"].ToString();

            DateTime d = new DateTime(scheduleModel.year, scheduleModel.month, scheduleModel.day, scheduleModel.hour, scheduleModel.minutes, 0);
            scheduleModel.scheduleDatetime = d;

            return scheduleModel;

        }

        public static List<SchedulesModel> checkSchedule()
        {
            List<SchedulesModel> schedules = new List<SchedulesModel>();

            DataTable dt = new SqLiteBaseRepository().getAllSchedules();

            SchedulesModel scheduleModel;

            foreach (DataRow item in dt.Rows)
            {
                scheduleModel = new SchedulesModel();
                scheduleModel.Id = item["Id"].ToString();

                scheduleModel.ScheduleName = item["ScheduleName"].ToString();
                scheduleModel.JsonString = item["JsonString"].ToString();
                scheduleModel.day = Convert.ToInt32(item["day"].ToString());
                scheduleModel.year = Convert.ToInt32(item["year"].ToString());
                scheduleModel.month = Convert.ToInt32(item["month"].ToString());
                scheduleModel.hour = Convert.ToInt32(item["hour"].ToString());
                scheduleModel.minutes = Convert.ToInt32(item["minutes"].ToString());
                scheduleModel.status = item["status"].ToString();
                scheduleModel.Type = item["Type"].ToString();
                scheduleModel.result = item["result"].ToString();

                DateTime d = new DateTime(scheduleModel.year, scheduleModel.month, scheduleModel.day, scheduleModel.hour, scheduleModel.minutes, 0);
                scheduleModel.scheduleDatetime = d;

                schedules.Add(scheduleModel);
            }

            foreach (SchedulesModel item in schedules)
            {

                DateTime today = DateTime.Now;
                var diffOfDates = today - item.scheduleDatetime;
                var mins= diffOfDates.TotalMinutes;

                if ((mins > 3) && (item.status == "PENDING"))
                {
                    new SqLiteBaseRepository().updatescheduleStatus(item.Id, "MISSED");
                    item.status = "MISSED";
                    item.justUpdated = true;
                }
            }

            return schedules;
        }
    }
}
