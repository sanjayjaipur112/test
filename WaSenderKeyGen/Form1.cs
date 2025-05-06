using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static readonly string WaSenderFolderName = "WaSender";


        public static string GetTempFolderPath()
        {
            String FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            String WaSenderFolderpath = Path.Combine(FolderPath, WaSenderFolderName);
            if (!Directory.Exists(WaSenderFolderpath))
            {
                Directory.CreateDirectory(WaSenderFolderpath);
            }


            return WaSenderFolderpath;
        }


        string loc = "info.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            txtKey.Text = "";
            try
            {
                string inform = textBox1.Text;
                File.WriteAllText(GetTempFolderPath() + "\\"+loc, inform);
            }
            catch (Exception ex)
            {

            }

         

            if (txtActivationCode.Text != "" && txtDays.Text != "" && textBox1.Text != "")
            {
                try
                {
                    txtKey.Text = KeySecurity.KeySecurity.GenerateKeyActivate(txtActivationCode.Text, Convert.ToInt32(txtDays.Text), textBox1.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please fill all required fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void hideLinkLabels()
        {
            linkLabel1.Hide();
            linkLabel2.Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            try
            {
                string text = File.ReadAllText(GetTempFolderPath() + "\\"+loc);
                textBox1.Text = text;
            }
            catch (Exception ex)
            {
                
            }
            try
            {
                if (File.Exists(loc))
                {
                    hideLinkLabels();
                }
            }
            catch (Exception ex)
            {

            }
        }


        public string Base64Encode(string plainText)
        {



            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        class ActivationModel
        {

            public string ActivationCode { get; set; }
            public int validDays { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        private void txtActivationCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://wasenderhelp.wordpress.com/2023/08/09/wasender-3-0-and-above-how-to-activate/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            txtKey.Text = "";
            try
            {

                string inform = "anything";
                File.WriteAllText(loc, inform);
                hideLinkLabels();
            }
            catch (Exception ex)
            {

            }

        }
    }
}
