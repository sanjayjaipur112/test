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
    public partial class Progressbar : MyMaterialPopOp
    {
        public Progressbar()
        {
            InitializeComponent();
        }

        private void Progressbar_Load(object sender, EventArgs e)
        {
            this.Icon = Strings.AppIcon;
            initLanguages();
            progressBar1.Show();
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;
        }

        private void initLanguages()
        {
            this.Text = Strings.Loading;
            materialLabel1.Text = Strings.PleaseWait;
        }
    }
}
