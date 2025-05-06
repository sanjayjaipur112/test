using MaterialSkin.Controls;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WASender.Model
{
    public class BaseClass : MaterialForm
    {
        public IWebDriver driver { get; set; }
    }
}
