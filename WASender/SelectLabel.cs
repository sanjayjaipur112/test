using MaterialSkin.Controls;
using Models;
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
    public partial class SelectLabel : MyMaterialPopOp
    {
        GrabChatList grabChatList;
        List<LableModel> lableList;
        public SelectLabel(GrabChatList _grabChatList, List<LableModel> _lableList)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.grabChatList = _grabChatList;
            this.lableList = _lableList;
            materialListView1.Columns[0].Text = Strings.LebelName;
            materialListView1.Columns[1].Text = Strings.ChatCount;
            materialButton1.Text = Strings.Select;
        }

        private void SelectLabel_Load(object sender, EventArgs e)
        {
            initLanguages();
            LoadData();
        }

        private void LoadData()
        {
            foreach (LableModel item in lableList)
            {
                ListViewItem lItem = new ListViewItem(item.name.ToString());
                lItem.SubItems.Add(item.count.ToString());
                materialListView1.Items.Add(lItem);
            }
            
        }

        private void initLanguages()
        {
            this.Text = Strings.SelectLebel;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialListView1.SelectedItems.Count == 0)
            {
                MaterialSnackBar SnackBarMessage1 = new MaterialSnackBar(Strings.PleaseSelectAnyOneLebel, Strings.OK, true);
                SnackBarMessage1.Show(this);
            }
            else
            {
                var ss = materialListView1.SelectedItems;
                string text = materialListView1.SelectedItems[0].Text;
                grabChatList.SelectLabelReturl(text);
                this.Close();
            }
        }
    }
}
