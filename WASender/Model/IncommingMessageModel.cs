using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WASender.Model
{
    public class IncommingMessageModel
    {
        public string body { get; set; }
        public string from { get; set; }
        public string notifyName { get; set; }
        public string self { get; set; }

        public IDModel id { get; set; }
        public string author { get; set; }
    }

    public class IDModel
    {
        public bool fromMe { get; set; }
        public string id { get; set; }
        public string _serialized { get; set; }
    }
}
