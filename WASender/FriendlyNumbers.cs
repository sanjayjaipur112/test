using MaterialSkin.Controls;
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
    public partial class FriendlyNumbers : MyMaterialPopOp
    {
        SingleLauncher singleLauncher;
        int count = 0;

        public FriendlyNumbers(SingleLauncher _singleLauncher, int _count=0)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            this.singleLauncher = _singleLauncher;
            this.count = _count;
        }

        private void FriendlyNumbers_Load(object sender, EventArgs e)
        {
            initLanguages();
            LoadExisting();
        }

        private void LoadExisting()
        {
            try
            {
                string BlockListFilePath = Config.getFriendlylistFile();
                string text = File.ReadAllText(BlockListFilePath);
                if (text != "")
                    textBox1.Text = text;
                
            }
            catch (Exception ex)
            {

            }
            if (this.count > 0)
            {
                materialTextBox22.Text = this.count.ToString();
            }
        }

        private void initLanguages()
        {
            this.Text = Strings.FriendlyNumbers;
            materialTextBox22.Hint = Strings.SendmessagetothisNumbersaftermessages;
            materialButton1.Text = Strings.Save;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Strings.AspartoftheAntibanSetting + Environment.NewLine + Environment.NewLine + Strings.Recommendedtotalis3to7numbers + Environment.NewLine + Environment.NewLine + Strings.Ifyouareusingmultipleaccountstosendmessages + Environment.NewLine + Environment.NewLine + Strings.Entereachnumberinoneline, Strings.WhatIs + " " + Strings.FriendlyNumbers, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void materialTextBox22_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {

            try
            {
                int _count= Convert.ToInt32(materialTextBox22.Text);

                var numbers = textBox1.Text.Split('\n');
                List<string> allnumbers = numbers.Select(x => x.Replace("\r", "")).Where(x => x != "").ToList();

                if (allnumbers.Count == 0)
                {
                    throw new Exception("");
                }

                try
                {
                    string BlockListFilePath = Config.getFriendlylistFile();
                    File.WriteAllText(BlockListFilePath, textBox1.Text);
                }
                catch (Exception ex)
                {

                }

                this.singleLauncher.receiveFriendlyListData(allnumbers, _count);
                this.Close();

            }
            catch (Exception ex)
            {
                MaterialSnackBar SnackBarMessage1 = new MaterialSnackBar(Strings.Pleasecheckinputs, Strings.OK, true);
                SnackBarMessage1.Show(this);
            }
        }
    }
}
