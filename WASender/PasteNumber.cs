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
    public partial class PasteNumber : MyMaterialPopOp
    {

        WaSenderForm waSenderForm;
        NumberFilter numberFilter;
        public PasteNumber(WaSenderForm _waSenderForm)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            waSenderForm = _waSenderForm;
        }

        public PasteNumber(NumberFilter _numberFilter)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            numberFilter = _numberFilter;
        }

        private void PasteNumber_Load(object sender, EventArgs e)
        {
            initLanguage();
        }

        private void initLanguage()
        {
            this.Text = Strings.CopyPasteNumber;
            materialButton1.Text = Strings.Import;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            List<string> splits = textBox1.Text.Split('\n').ToList();

            List<string> finalList = new List<string>();

            for (var i = 0; i < splits.Count();i++ )
            {
                if (splits[i].Contains(","))
                {
                    List<string> byQWuama = splits[i].Split(',').ToList();
                    finalList.AddRange(byQWuama);
                }
                else
                {
                    if (splits[i] != "")
                    {
                        finalList.Add(splits[i]);
                    }
                }
            }
            for (var i = 0; i < finalList.Count(); i++)
            {
                finalList[i] = finalList[i].Replace("\r", "");
                finalList[i] = finalList[i].Replace("\t", "");
                finalList[i] = finalList[i].Replace("\n", "");
            }
            if (this.waSenderForm != null)
            {
                this.waSenderForm.ReturnPasteNumber(finalList);
                this.Close();
            }
            if (this.numberFilter != null)
            {
                this.numberFilter.ReturnPasteNumber(finalList);
                this.Close();
            }
        }

    }
}
