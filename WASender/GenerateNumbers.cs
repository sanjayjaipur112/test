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
    public partial class GenerateNumbers : MyMaterialPopOp
    {
        NumberFilter numberFilter;
        List<string> list;
        public GenerateNumbers(NumberFilter _numberFilter)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            numberFilter = _numberFilter;
            initLanguages();
        }

        private void initLanguages()
        {
            this.Text = Strings.GenerateNumbers;
            materialTextBox1.Hint = Strings.CountryCode;
            materialTextBox2.Hint = Strings.PhoneNumber;
            materialTextBox3.Hint = Strings.Increment;
            materialTextBox4.Hint = Strings.Quentity;
            materialButton1.Text = Strings.Generate;
            materialButton2.Text = Strings.Import;
            dataGridView1.Columns[0].HeaderText = Strings.Number;

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            try
            {
                int countryCode = Convert.ToInt32(materialTextBox1.Text);
                long phoneNumber = Convert.ToInt64(materialTextBox2.Text);
                int increment = Convert.ToInt32(materialTextBox3.Text);
                int quentity = Convert.ToInt32(materialTextBox4.Text);

                long num = long.Parse(materialTextBox1.Text + materialTextBox2.Text);
                list = new List<string>();
                for (int i = 0; i < int.Parse(materialTextBox4.Text); i++)
                {
                    list.Add(num.ToString());

                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                    row.Cells[0].Value = num.ToString();
                    dataGridView1.Rows.Add(row);
                    num += int.Parse(materialTextBox3.Text);


                }

            }
            catch (Exception ex)
            {

            }
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            numberFilter.NumberGeneratorReturn(list);
            this.Hide();
        }

        private void GenerateNumbers_Load(object sender, EventArgs e)
        {

        }
    }
}
