using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WASender.Models
{
     public class WarmerContactModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }

        public string toAccountId { get; set; }

        public WebView2 webview { get; set; }

        public System.Windows.Forms.TabPage tabPage{ get; set; }

        internal void Invoke(System.Windows.Forms.MethodInvoker methodInvoker)
        {
            throw new NotImplementedException();
        }
    }
}
