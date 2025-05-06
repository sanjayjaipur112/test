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
using WASender.Model;

namespace WASender
{
    public partial class InputDialog : MyMaterialPopOp
    {
        GMapExtractor gMapExtractor;
        List<GMapGlobal> GMapGlobalList;
        public InputDialog(GMapExtractor _gMapExtractor)
        {
            gMapExtractor = _gMapExtractor;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
        }

        private void InputDialog_Load(object sender, EventArgs e)
        {
            GMapGlobalList = new List<GMapGlobal>();
            initLang();
            this.Icon = Strings.AppIcon;
        }

        private void initLang()
        {
            this.Text = Strings.YourSearchterm;
            materialMaskedTextBox1.Text = Strings.Softwarecompaniesintexas;
            materialButton1.Text = Strings.Add;
            materialButton2.Text = Strings.Search;
            materialMultiLineTextBox21.Hint = Strings.Searches;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem(materialMaskedTextBox1.Text);

            materialMultiLineTextBox21.Text = materialMultiLineTextBox21.Text + materialMaskedTextBox1.Text + Environment.NewLine;
            materialMaskedTextBox1.Text = "";

        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            List<string> searchers = materialMultiLineTextBox21.Text.Split('\n').ToList();

            foreach (string item in searchers)
            {
                string Newitem = item.Replace("\r", "");
                if (item != "")
                {
                    GMapGlobalList.Add(new GMapGlobal
                    {
                        searchQuery = Newitem,
                        isDone = false
                    });
                }
            }



            if (GMapGlobalList.Count() > 0)
            {
                try
                {
                    gMapExtractor.InputReturnList(GMapGlobalList);
                    this.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
