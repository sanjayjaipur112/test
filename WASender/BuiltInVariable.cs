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

namespace WASender
{
    public partial class BuiltInVariable : MyMaterialPopOp
    {
        WaSenderForm waSenderForm;
        AddCaption addCaption;
        bool freezeName = false;
        public BuiltInVariable(WaSenderForm _waSenderForm,bool _freezeName = false)
        {
            InitializeComponent();
            this.waSenderForm = _waSenderForm;
            this.Icon = Strings.AppIcon;
            freezeName = _freezeName;
            init();
        }

        private void init()
        {
            this.Select.Text = "<< " + Strings.Add;

        }

        public void LoadVariables()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(Strings.Name, typeof(string));

            dt.Rows.Add(Strings.Name);
            dt.Rows.Add(Strings.Spoiler);

            gridMarker.DataSource = dt;
            gridMarker.Columns[1].Width = 250;

            DataGridViewButtonColumn uninstallButtonColumn = new DataGridViewButtonColumn();
            uninstallButtonColumn.Name = "info_column";
            uninstallButtonColumn.Text = Strings.Info + " ℹ️ ";
            uninstallButtonColumn.HeaderText = Strings.Info;
            uninstallButtonColumn.UseColumnTextForButtonValue = true;

            int columnIndex = 2;
            if (gridMarker.Columns["info_column"] == null)
            {
                gridMarker.Columns.Insert(columnIndex, uninstallButtonColumn);
            }
            if (freezeName)
            {
                gridMarker.Rows[0].Cells[0]= new DataGridViewTextBoxCell { Value =  "<< " + Strings.Add };
                gridMarker.Rows[0].Cells[0].ReadOnly = true;
                gridMarker.Rows[0].Frozen = true;
            }
        }

        private void BuiltInVariable_Load(object sender, EventArgs e)
        {
            initLanguages();
            LoadVariables();
        }

        private void initLanguages()
        {
            this.Text = Strings.BuiltInVariable;
            materialButton1.Text = Strings.Close;
        }

        private void gridMarker_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == gridMarker.Columns["info_column"].Index)
                {
                    InfoWindow info = new InfoWindow(e.RowIndex);
                    info.ShowDialog();
                }

                if (gridMarker.Columns[e.ColumnIndex].Name == "Select")
                {
                    string return_text = "";
                    if (e.RowIndex == 0)
                    {
                        if (freezeName)
                        {
                            return;
                        }
                        else
                        {
                            return_text = "[NAME]";
                        }
                        
                    }
                    else if (e.RowIndex == 1)
                    {
                        return_text = "[SPOILER]";
                    }

                    if (waSenderForm != null)
                    {
                        waSenderForm.AddKeyMarker(return_text);
                    }
                    else if (addCaption != null)
                    {
                        addCaption.AddKeyMarker(return_text);
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
