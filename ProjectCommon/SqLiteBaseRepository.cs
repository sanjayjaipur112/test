using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WASender
{
    public class SqLiteBaseRepository
    {

        SQLiteConnection conn;
        SQLiteCommand cmd;
        SQLiteDataAdapter adapter;
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        int id;
        bool isDoubleClick = false;
        String connectString = @"Data Source=" + Config.GetSysFolderPath() + @"\db.db;version=3";

        public static void createTable()
        {

        }

        public DataTable getBySessionId(string SessionId)
        {
            try
            {
                if (isDefaultColumnExist() == 0)
                {
                    CreateDefautultColumn();
                    markasDefault();
                }

                conn = new SQLiteConnection(connectString);
                cmd = new SQLiteCommand();
                cmd.CommandType = CommandType.Text;
                String sql = "SELECT * FROM Sessions where sesionID=@SessionId";
                cmd.Connection = conn;
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SQLiteParameter("@SessionId", SessionId));
                conn.Open();

                adapter = new SQLiteDataAdapter(cmd);

                ds.Reset();
                adapter.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public DataTable ReadData(bool getOnlyDefault = false)
        {
            try
            {
                if (isDefaultColumnExist() == 0)
                {
                    CreateDefautultColumn();
                    markasDefault();
                }

                conn = new SQLiteConnection(connectString);
                conn.Open();
                cmd = new SQLiteCommand();
                String sql = "SELECT * FROM Sessions";
                if (getOnlyDefault == true)
                {
                    sql = "SELECT * FROM Sessions where isDefault=1";
                }
                adapter = new SQLiteDataAdapter(sql, conn);
                ds.Reset();
                adapter.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public DataTable ReadDataExists(string sessionName)
        {
            try
            {
                conn = new SQLiteConnection(connectString);
                conn.Open();
                cmd = new SQLiteCommand();
                String sql = "SELECT * FROM Sessions where sessionName='" + sessionName + "'";
                adapter = new SQLiteDataAdapter(sql, conn);
                ds.Reset();
                adapter.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public void updatescheduleStatus(string scheduleID, string status)
        {
            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"update schedules set status=@status where Id=@scheduleID";
            cmd.Connection = conn;
            cmd.Parameters.Add(new SQLiteParameter("@scheduleID", scheduleID));
            cmd.Parameters.Add(new SQLiteParameter("@status", status));
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            if (i == 1)
            {
                //MessageBox.Show("Successfully Created!");
            }
        }
        public int DeleteSession(string SessionId)
        {
            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            if (SessionId == "")
            {
                cmd.CommandText = @"Delete from Sessions where  sesionID is null";
            }
            else
            {
                cmd.CommandText = @"Delete from Sessions where  sesionID=@sesionID";
            }

            cmd.Connection = conn;
            cmd.Parameters.Add(new SQLiteParameter("@sesionID", SessionId == null ? (object)DBNull.Value : SessionId));
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            return i;
        }


        public void checkAndCreate_Schedule_tableIfnotExist()
        {
            if (!isSchedule_Table_Available())
            {
                try
                {
                    string strim = " create table schedules ";
                    strim += "( 	Id				text, ";
                    strim += " 	ScheduleName	text, ";
                    strim += " 	JsonString		text, ";
                    strim += " 	day				int, ";
                    strim += " 	year				int, ";
                    strim += " 	month				int, ";
                    strim += " 	hour				int, ";
                    strim += " 	minutes				int, ";
                    strim += " 	status			text, ";
                    strim += " 	Type			text, ";
                    strim += " 	result text )";
                    strim += "  ";
                    strim += "  ";
                    strim += "  ";
                    strim += "  ";
                    conn = new SQLiteConnection(connectString);
                    cmd = new SQLiteCommand();
                    cmd.CommandText = strim;
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {

                }
                conn.Close();
            }
        }

        public bool isSchedule_Table_Available()
        {
            conn = new SQLiteConnection(connectString);
            try
            {
                cmd = new SQLiteCommand();
                cmd.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND name='schedules'";
                cmd.Connection = conn;
                conn.Open();
                var si = cmd.ExecuteScalar();

                if (si == null || si == "null")
                {
                    conn.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            conn.Close();
            return true;
        }


        public int DeleteSchedule(string ScheduleId)
        {
            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"Delete from schedules where  Id=@ScheduleId";
            cmd.Connection = conn;
            cmd.Parameters.Add(new SQLiteParameter("@ScheduleId", ScheduleId));
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            return i;
        }



        public DataTable getAllSchedules()
        {
            try
            {
                checkAndCreate_Schedule_tableIfnotExist();
            }
            catch (Exception ex)
            {

            }

            try
            {
                conn = new SQLiteConnection(connectString);
                conn.Open();
                cmd = new SQLiteCommand();
                String sql = "select * from schedules";
                adapter = new SQLiteDataAdapter(sql, conn);
                ds.Reset();
                adapter.Fill(ds);
                dt = ds.Tables[0];
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public DataTable getSchedulesById(string Id)
        {
            try
            {
                conn = new SQLiteConnection(connectString);
                conn.Open();
                cmd = new SQLiteCommand();
                String sql = "select * from schedules where Id='" + Id + "'";
                adapter = new SQLiteDataAdapter(sql, conn);
                ds.Reset();
                adapter.Fill(ds);
                dt = ds.Tables[0];
                conn.Close();
                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public void UpdateScheduleCompleted(string scheduleId, string camp_json_string, string resultString)
        {
            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"update schedules set  JsonString=@JsonString,status=@status,result=@resultString where  Id=@Id ";
            cmd.Connection = conn;
            cmd.Parameters.Add(new SQLiteParameter("@Id", scheduleId));
            cmd.Parameters.Add(new SQLiteParameter("@JsonString", camp_json_string));
            cmd.Parameters.Add(new SQLiteParameter("@status", "COMPLETED"));
            cmd.Parameters.Add(new SQLiteParameter("@resultString", resultString));

            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            if (i == 1)
            {
                //MessageBox.Show("Successfully Created!");
            }
        }

        public void InsertSchedule(string ScheduleName, DateTime sessionDatetime, string camp_json_string, string Type)
        {
            checkAndCreate_Schedule_tableIfnotExist();

            int _day = sessionDatetime.Day;
            int _month = sessionDatetime.Month;
            int _year = sessionDatetime.Year;
            int _hours = sessionDatetime.Hour;
            int _minutes = sessionDatetime.Minute;
            string newID = Guid.NewGuid().ToString();

            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"INSERT INTO schedules (Id,ScheduleName,JsonString,day,year,month,hour,minutes,status,Type) VALUES(@Id,@ScheduleName,@JsonString,@day,@year,@month,@hour,@minutes,@status,@Type)";
            cmd.Connection = conn;
            cmd.Parameters.Add(new SQLiteParameter("@Id", newID));
            cmd.Parameters.Add(new SQLiteParameter("@ScheduleName", ScheduleName));

            cmd.Parameters.Add(new SQLiteParameter("@day", _day));
            cmd.Parameters.Add(new SQLiteParameter("@year", _year));
            cmd.Parameters.Add(new SQLiteParameter("@month", _month));
            cmd.Parameters.Add(new SQLiteParameter("@hour", _hours));
            cmd.Parameters.Add(new SQLiteParameter("@minutes", _minutes));
            cmd.Parameters.Add(new SQLiteParameter("@Type", Type));

            cmd.Parameters.Add(new SQLiteParameter("@JsonString", camp_json_string));
            cmd.Parameters.Add(new SQLiteParameter("@status", "PENDING"));
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            if (i == 1)
            {
                //MessageBox.Show("Successfully Created!");
            }
        }


        public void UpdateSchedule(string scheduleId, string ScheduleName, DateTime sessionDatetime, string camp_json_string, string Type)
        {
            int _day = sessionDatetime.Day;
            int _month = sessionDatetime.Month;
            int _year = sessionDatetime.Year;
            int _hours = sessionDatetime.Hour;
            int _minutes = sessionDatetime.Minute;
            //string newID = Guid.NewGuid().ToString();

            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"update schedules set  ScheduleName=@ScheduleName,JsonString=@JsonString,day=@day,year=@year,month=@month,hour=@hour,minutes=@minutes,status=@status,Type=@Type where  Id=@Id ";
            cmd.Connection = conn;
            cmd.Parameters.Add(new SQLiteParameter("@Id", scheduleId));
            cmd.Parameters.Add(new SQLiteParameter("@ScheduleName", ScheduleName));

            cmd.Parameters.Add(new SQLiteParameter("@day", _day));
            cmd.Parameters.Add(new SQLiteParameter("@year", _year));
            cmd.Parameters.Add(new SQLiteParameter("@month", _month));
            cmd.Parameters.Add(new SQLiteParameter("@hour", _hours));
            cmd.Parameters.Add(new SQLiteParameter("@minutes", _minutes));
            cmd.Parameters.Add(new SQLiteParameter("@Type", Type));

            cmd.Parameters.Add(new SQLiteParameter("@JsonString", camp_json_string));
            cmd.Parameters.Add(new SQLiteParameter("@status", "PENDING"));
            conn.Open();
            int i = cmd.ExecuteNonQuery();
            conn.Close();
            if (i == 1)
            {
                //MessageBox.Show("Successfully Created!");
            }
        }

        public void setPrimaryAccount(string SessionId)
        {
            makeIsDefaultNull();
            try
            {
                conn = new SQLiteConnection(connectString);
                cmd = new SQLiteCommand();
                cmd.CommandText = @"update Sessions set IsDefault=1 where ID=@sesionID;";
                cmd.Connection = conn;
                cmd.Parameters.Add(new SQLiteParameter("@sesionID", SessionId));
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }
        private void makeIsDefaultNull()
        {
            try
            {
                conn = new SQLiteConnection(connectString);
                cmd = new SQLiteCommand();
                cmd.CommandText = @"update Sessions set IsDefault=null;";
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void markasDefault()
        {
            try
            {
                cmd = new SQLiteCommand();
                cmd.CommandText = @"update Sessions set IsDefault=1 where sessionName='Profile1'";
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }
        public void CreateDefautultColumn()
        {
            try
            {
                cmd = new SQLiteCommand();
                cmd.CommandText = @"alter table Sessions add isDefault INTEGER";
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {

            }
        }


        public int isDefaultColumnExist()
        {
            GenerateDatabase();
            int i = 0;
            conn = new SQLiteConnection(connectString);
            try
            {
                cmd = new SQLiteCommand();
                cmd.CommandText = @"SELECT EXISTS (SELECT * FROM sqlite_master WHERE tbl_name = 'Sessions' AND sql LIKE '%isDefault%') as 'ss'; ";
                cmd.Connection = conn;
                conn.Open();
                i = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {

            }
            conn.Close();
            return i;
        }
        public string AddSession(string sessionName, string sessionId = "", bool setDefault = false)
        {
            int _isDefault = 0;
            if (setDefault == true)
            {
                _isDefault = 1;
            }
            if (sessionId == "")
            {
                sessionId = Guid.NewGuid().ToString();
            }
            //connectString = @"Data Source=" + Config.GetSysFolderPath() + @"\db.db;version=3";
            GenerateDatabase();
            conn = new SQLiteConnection(connectString);
            cmd = new SQLiteCommand();
            cmd.CommandText = @"INSERT INTO Sessions (sessionName,sesionID,isDefault) VALUES(@sessionName,@sesionID,@_isDefault )";
            cmd.Connection = conn;
            cmd.Parameters.Add(new SQLiteParameter("@sessionName", sessionName));
            cmd.Parameters.Add(new SQLiteParameter("@sesionID", sessionId));
            cmd.Parameters.Add(new SQLiteParameter("@_isDefault", _isDefault));
            conn.Open();
            int i = cmd.ExecuteNonQuery();

            if (i == 1)
            {
                //MessageBox.Show("Successfully Created!");
            }
            return sessionId;
        }



        private void GenerateDatabase()
        {
            conn = new SQLiteConnection(connectString);
            conn.Open();
            string sql = "CREATE TABLE if not exists Sessions (ID INTEGER PRIMARY KEY AUTOINCREMENT, sessionName TEXT,sesionID TEXT)";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }

    }
}
