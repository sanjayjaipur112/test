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
    public partial class GenerateRandomNames : MyMaterialPopOp
    {

        GoogleCSVGenerator googleCSVGenerator;
        public GenerateRandomNames(GoogleCSVGenerator _googleCSVGenerator)
        {
            this.googleCSVGenerator = _googleCSVGenerator;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
        }

        private void GenerateRandomNames_Load(object sender, EventArgs e)
        {
            initLanguages();
        }

        private void initLanguages()
        {
            this.Text = Strings.GenrateRandomName;
            materialTextBox21.Hint = Strings.NamePrefix;
            materialTextBox22.Hint = Strings.Increment;
            materialTextBox23.Hint = Strings.NameSufix;
            materialButton1.Text = Strings.GenerateNow;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (materialTextBox21.Text != "" && materialTextBox22.Text != "")
            {
                try
                {
                    int increment = Convert.ToInt32(materialTextBox22.Text);
                    this.googleCSVGenerator.GenerateNames(materialTextBox21.Text, increment, materialTextBox23.Text);
                    this.Hide();
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
