using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WASender
{
    public class TestClass
    {
        public delegate void StatusUpdateHandler(object sender, ProgressEventArgs e);
        public event StatusUpdateHandler OnUpdateStatus;

        public void UpdateStatus(string status)
        {
            // Make sure someone is listening to event
            if (OnUpdateStatus == null) return;

            ProgressEventArgs args = new ProgressEventArgs(status);
            OnUpdateStatus(this, args);
        }
    }


    public class ProgressEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public ProgressEventArgs(string status)
        {
            Status = status;
        }

        
    }
}
