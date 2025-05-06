using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WASender.Models
{
    public class WarmerModel
    {
        public List<WarmerContactModel> SelectedAccountNames { get; set; }
        public string selectedText { get; set; }
        public int delayFrom { get; set; }
        public int delayTo { get; set; }

        public int warmmingMethod { get; set; }
    }

    
}
