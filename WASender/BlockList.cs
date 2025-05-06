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
    public partial class BlockList : MyMaterialPopOp
    {
        public BlockList()
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
        }

        private void BlockList_Load(object sender, EventArgs e)
        {
            initLanguage();
            LoadExisting();
        }

        private void LoadExisting()
        {
            try
            {
                string BlockListFilePath = Config.getBlocklistFile();
                string text = File.ReadAllText(BlockListFilePath);
                textBox1.Text = text;
            }
            catch (Exception ex)
            {

            }
        }

        private void initLanguage()
        {
            this.Text = Strings.BlockList;
            materialButton1.Text = Strings.Save;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string BlockListFilePath = Config.getBlocklistFile();
                File.WriteAllText(BlockListFilePath, textBox1.Text);

                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Done 👍👍👍👍", Strings.OK, true);
                SnackBarMessage.Show(this);
            }
            catch (Exception ex)
            {
                MaterialSnackBar SnackBarMessage = new MaterialSnackBar("Not Done 👎👎" + ex.Message, Strings.OK, true);
                SnackBarMessage.Show(this);
            }
        }

       
    }
}
