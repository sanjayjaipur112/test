using MaterialSkin.Controls;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;

namespace WASender
{
    public partial class AllSchedules : MyMaterialPopOp
    {
        List<SchedulesModel> schedules;
        WaSenderForm waSenderForm;
        public AllSchedules(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            waSenderForm = _waSenderForm;
        }

        private void AllSchedules_Load(object sender, EventArgs e)
        {
            initLanguages();
            loadData();
        }

        private string getStatusString(string hcStatus)
        {
            string returnVal = "";
            try
            {
                returnVal = Strings.GetValue(hcStatus);
            }
            catch (Exception ex)
            {
                returnVal = hcStatus;
            }
            if (returnVal == "")
            {
                returnVal = hcStatus;
            }
            return returnVal;
        }
        private void loadData()
        {
            dataGridView1.Rows.Clear();
            schedules = PCUtils.checkSchedule();

            foreach (SchedulesModel schedule in schedules)
            {
                dataGridView1.Rows.Add(new object[]{
                    schedule.ScheduleName,
                    schedule.Type=="GROUP"? Strings.Group: Strings.individual,
                    schedule.year + "-" + schedule.month + "-" + schedule.day + " " + schedule.hour + ":" + schedule.minutes,
                    getStatusString(schedule.status),
                    schedule.status=="PENDING"?Strings.Edit:"-",
                    schedule.status=="COMPLETED"? Strings.Report: "-",
                    Strings.Clone,
                    Strings.SendNow,
                    Strings.Delete
                });
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = schedule;
            }

            foreach (DataGridViewRow item in dataGridView1.Rows.OfType<DataGridViewRow>())
            {
                SchedulesModel model = (SchedulesModel)item.Tag;
                if (model.status != "PENDING")
                {
                    var cell = new DataGridViewTextBoxCell();
                    cell.Value = "-";
                    cell.Style.BackColor = Color.FromKnownColor(KnownColor.Control);
                    item.Cells[4] = cell;
                }
                if (model.status != "COMPLETED")
                {
                    DataGridViewCell btn = (DataGridViewCell)item.Cells[3];
                    btn.ReadOnly = true;
                    var cell = new DataGridViewTextBoxCell();
                    cell.Value = "-";
                    cell.Style.BackColor = Color.FromKnownColor(KnownColor.Control);
                    item.Cells[5] = cell;
                }
                if (model.status == "MISSED")
                {

                }
                else
                {
                    DataGridViewCell btn = (DataGridViewCell)item.Cells[3];
                    btn.ReadOnly = true;
                    var cell = new DataGridViewTextBoxCell();
                    cell.Value = "-";
                    cell.Style.BackColor = Color.FromKnownColor(KnownColor.Control);
                    item.Cells[7] = cell;
                }

            }
        }



        private void initLanguages()
        {
            this.Text = Strings.AllSchedules;
            dataGridView1.Columns[0].HeaderText = Strings.Name;
            dataGridView1.Columns[1].HeaderText = Strings.Type;
            dataGridView1.Columns[2].HeaderText = Strings.ScheduleDate;
            dataGridView1.Columns[3].HeaderText = Strings.Status;
            dataGridView1.Columns[4].HeaderText = Strings.Edit;
            dataGridView1.Columns[5].HeaderText = Strings.Report;
            dataGridView1.Columns[6].HeaderText = Strings.Clone;
            dataGridView1.Columns[7].HeaderText = Strings.StartNow;
            dataGridView1.Columns[8].HeaderText = Strings.Delete;

            label1.MaximumSize = new Size(this.Size.Width, 0);
            label1.AutoSize = true;
            label1.Text = Strings.Note + ": " + Strings.TocreatenewSchedulePrepareyourCampainwithyourmessageandnumbers + ". " + Strings.Clickon + " \"" + Strings.StartCampaign + "\" " + Strings.Button + ". \"" + Strings.Schedule + "\" " + Strings.OptionwillbeavailableonLauncherwindow;
            
            
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SchedulesModel schedule = (SchedulesModel)dataGridView1.CurrentRow.Tag;
            //MessageBox.Show(schedule.ScheduleName);
            if (e.ColumnIndex == 4 && schedule.status == "PENDING")
            { 
                // Edit Schedule
                this.waSenderForm.EditSchedule(schedule);
                this.Close();
            }
            else if (e.ColumnIndex == 5 && schedule.status=="COMPLETED")
            { 
                // Generate Report
                string _newFileName = Guid.NewGuid().ToString() + ".html";
                string tmpfile = Config.GetTempFolderPath() + "\\" + _newFileName;
                using (FileStream fs = File.Create(tmpfile))
                {
                    
                    byte[] author = new UTF8Encoding(true).GetBytes(schedule.result);
                    fs.Write(author, 0, schedule.result.Length);
                }
                System.Diagnostics.Process.Start(tmpfile);

            }
            else if (e.ColumnIndex == 6)
            {
                schedule.Id = null;
                schedule.ScheduleName = schedule.ScheduleName + " " + Strings.Clone;
                this.waSenderForm.EditSchedule(schedule);
                this.Close();
            }
            else if (e.ColumnIndex == 7 && schedule.status == "MISSED")
            {
                this.waSenderForm.runSceduleById(schedule.Id);
                this.Close();
            }
            else if (e.ColumnIndex == 8)
            {
                MaterialDialog materialDialog = new MaterialDialog(this, Strings.Delete, Strings.AreyousuretodeletethisSchedule, Strings.Yes, true, Strings.Cancel);
                DialogResult result = materialDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    new SqLiteBaseRepository().DeleteSchedule(schedule.Id);
                    loadData();
                    waSenderForm.scheduleDeleted();
                }
            }
        }

        private void AllSchedules_FormClosed(object sender, FormClosedEventArgs e)
        {
            waSenderForm.formReturn(true);
        }
    }
}
