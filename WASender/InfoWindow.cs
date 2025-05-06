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
    public partial class InfoWindow : MyMaterialPopOp
    {
        private string readmoreImageBase = "";
        int infoRowNumbr = 0;
        public InfoWindow(int _infoRowNumbr)
        {
            InitializeComponent();
            this.Icon = Strings.AppIcon;
            infoRowNumbr = _infoRowNumbr;
        }

        private void InfoWindow_Load(object sender, EventArgs e)
        {
            initLanguages();
            setText();
        }

        private void setText()
        {
            if (infoRowNumbr==0)
            {
                StringBuilder s = new StringBuilder("");
                s.AppendLine("<html>");
                s.AppendLine("<body style='font-family: monospace;'>");
                s.AppendLine("<p>" + Strings.NAMEvariableinsertsausersnameintoamessageautomaticallywhilesendingmessages + "</p>");
                s.AppendLine("<p>" + Strings.Donottranslatethisvariableintoanyotherlanguage + ".</p>");
                s.AppendLine("<p style='color:red'>" + Strings.ImportantNotethisvariableonlyworksforthosenumberswho + ".</p>");
                s.AppendLine("</body></html>");
                webBrowser1.DocumentText = s.ToString();
            }
            else if (infoRowNumbr == 1)
            {
                StringBuilder s = new StringBuilder("");
                s.AppendLine("<html>");
                s.AppendLine("<body style='font-family: monospace;'>");
                s.AppendLine("" + Strings.SPOILERthisvariableSendWhatsAppmessageswithspoilertags + ".");
                s.AppendLine("<img src='https://i.imgur.com/QxY9jxR.png' />");
                s.AppendLine("<img src='https://i.ibb.co/qg1DsgG/QxY9jxR.png' />");
                s.AppendLine("<p style='color:red'>" + Strings.NoteSpoilerswillnotworkforiOS + ".</p>");
                s.AppendLine("</body></html>");
                webBrowser1.DocumentText = s.ToString();
            }
        }

        private void initLanguages()
        {
            this.Text = Strings.Info;
            materialButton1.Text = Strings.Close;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
