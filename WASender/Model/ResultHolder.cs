using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WASender.Model
{
    public class ResultHolder
    {
        public string MainLink { get; set; }

        public List<string> subLinks { get; set; }

        public List<WebItemModel> mobiles { get; set; }

        public List<WebItemModel> emailIds { get; set; }
    }
}
