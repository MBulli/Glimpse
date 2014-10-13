using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Glimpse
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // TODO: http://msdn.microsoft.com/en-us/library/bb546085(v=vs.110).aspx
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form = new Form1();
            if (args != null)
            {
                form.LaunchWithArguments(args);
            }
            Application.Run(form);
        }
    }
}
