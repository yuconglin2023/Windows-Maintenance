using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows_Maintenance;

namespace Windows_Maintenance
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            DateTime now = DateTime.Now;
            if (now.Hour >= 9 && now.Hour < 17)
            {
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form2 form2 = new Form2();
            form2.Show();
            Form1 form1 = new Form1();
            form1.Show();
            form1.Activate();
            Application.Run(form1);
        }
    }
}
