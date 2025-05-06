using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaAutoReplyBot;
using WASender;

namespace WASender
{
    public partial class ManageAccounts : MyMaterialPopOp
    {
        public ManageAccounts()
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
        }

        private void ManageAccounts_Load(object sender, EventArgs e)
        {
            initLanguage();
            loadData();
        }

        public void loadData()
        {
            dataGridView1.Rows.Clear();
            DataTable dt = new SqLiteBaseRepository().ReadData();

            foreach (DataRow item in dt.Rows)
            {
                dataGridView1.Rows.Add(new object[]{
                    item["sessionName"].ToString(),
                    item["isDefault"].ToString()=="1" ? Properties.Resources.Icon_Yes:Properties.Resources.Icon_No,
                    Strings.Delete,
                    Strings.Load,
                });
                dataGridView1.Rows[dataGridView1.RowCount - 1].Tag = item;
            }
            if (selectedIndex != 0 && selectedIndex != -1)
            {
                try
                {
                    dataGridView1.Rows[selectedIndex].Selected = true;
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void initLanguage()
        {
            this.Text = Strings.ManageAccounts;
            dataGridView1.Columns[0].HeaderText = Strings.AccountName;

            dataGridView1.Columns[1].HeaderText = Strings.IsDefault;
            dataGridView1.Columns[2].HeaderText = Strings.Delete;
            dataGridView1.Columns[3].HeaderText = Strings.Load;

            materialButton1.Text = Strings.AddNewAccount;
            contextMenuStrip1.Items[0].Text = Strings.SetasDefaultAccount;
            label1.Text = Strings.NoteRightClicktoanyaccounttosentitasprimary;

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                DataRow dr = (DataRow)dataGridView1.CurrentRow.Tag;
                string Id = dr["sesionID"].ToString();
                string isDefault = dr["isDefault"].ToString();

                if (isDefault == "1")
                {
                    MaterialDialog materialDialog = new MaterialDialog(this, Strings.Delete, Strings.CantDeleteDefaultAccount, Strings.OK);
                    materialDialog.ShowDialog(this);
                }
                else
                {
                    MaterialDialog materialDialog = new MaterialDialog(this, Strings.Delete, Strings.AreYouSuretodeletethisAccount, Strings.Yes, true, Strings.Cancel);
                    DialogResult result = materialDialog.ShowDialog(this);
                    if (result == DialogResult.OK)
                    {
                        int _res = new SqLiteBaseRepository().DeleteSession(Id);
                        if (_res == 1)
                        {
                            MaterialSnackBar SnackBarMessage = new MaterialSnackBar(Strings.AccountDeleted, 7500, Strings.OK);
                            SnackBarMessage.Show(this);
                            loadData();
                        }
                    }
                }

            }
            else if (e.ColumnIndex == 3)
            {
                if (Utils.waSenderBrowser != null)
                {
                    Utils.waSenderBrowser.Close();
                }

                DataRow dr = (DataRow)dataGridView1.CurrentRow.Tag;
                string Id = dr["sesionID"].ToString();
                new WaSenderBrowser(Id).ShowDialog();
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            AddAccount form = new AddAccount(this);
            form.ShowDialog();
        }

        public static int selectedIndex = 0;

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    var hti = dataGridView1.HitTest(e.X, e.Y);
                    dataGridView1.ClearSelection();
                    selectedIndex = hti.RowIndex;
                    dataGridView1.Rows[hti.RowIndex].Selected = true;
                    contextMenuStrip1.Show(dataGridView1, new Point(e.X, e.Y));
                }
                catch (Exception ex)
                {

                }

            }
            if (e.Button == MouseButtons.Left) {
                var hti = dataGridView1.HitTest(e.X, e.Y);
                selectedIndex = hti.RowIndex;
            }
        }

        private void markAsDefaultAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedIndex == 0)
            {
                DataRow dr = (DataRow)dataGridView1.CurrentRow.Tag;
                string Id = dr["ID"].ToString();
                new SqLiteBaseRepository().setPrimaryAccount(Id);
                loadData();
            }
            else
            {
                DataRow dr = (DataRow)dataGridView1.Rows[selectedIndex].Tag;
                string Id = dr["ID"].ToString();
                new SqLiteBaseRepository().setPrimaryAccount(Id);
                loadData();
            }

            if (Utils.waSenderBrowser != null)
            {
                Utils.waSenderBrowser.Close();
            }
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged != DataGridViewElementStates.Selected)
            {
                string ss = "";
            }
        }
    }
}
