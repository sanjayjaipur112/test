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
    public partial class CountryCodeInput : MyMaterialPopOp
    {
        WaSenderForm waSenderForm;
        NumberFilter numberFilter;
        public CountryCodeInput(WaSenderForm _WaSenderForm)
        {
            waSenderForm = _WaSenderForm;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            init();
        }

        public CountryCodeInput(NumberFilter _numberFilter)
        {
            numberFilter = _numberFilter;
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            init();
        }

        private void init()
        {
            this.Text = Strings.EnterCountryCode;
            materialButton1.Text = Strings.OK;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (waSenderForm != null)
                {
                    int cc = Convert.ToInt32(materialMaskedTextBox1.Text);
                    waSenderForm.CountryCOdeAdded(materialMaskedTextBox1.Text);
                    this.Close();
                }
                if (numberFilter != null)
                {
                    int cc = Convert.ToInt32(materialMaskedTextBox1.Text);
                    numberFilter.CountryCOdeAdded(materialMaskedTextBox1.Text);
                    this.Close();
                }
                
            }
            catch (Exception ex)
            {
                
            }
            
        }
    }
}
