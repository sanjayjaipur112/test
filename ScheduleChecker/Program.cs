using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleChecker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            ///["\"Green\"","\"Schedule Checker\"","\"Running\"","\"Trying to run the Schedule, But \"","\"already running\"","\"I will stay here until all your schedules are completed\"","\"Exit\"","\"Days\"","\"Hours\"","\"Minutes\"","\"Next Schedule in\""]
            ///

            //args = new string[1];
            //args[0] = "[\"Green\",\"Schedule Checker\",\"Running\",\"Trying to run the Schedule, But \",\"already running\",\"I will stay here until all your schedules are completed\",\"Exit\",\"Days\",\"Hours\",\"Minutes\",\"Next Schedule in\",\"D:\\ProjectFiles\\WASender\\CodeCanyon\\CodeHere\\WASender\\bin\\Debug\"]";

            Application.Run(new ScheduleChecker(args));
        }
    }
}
