using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADD_RWMAC
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool ownmutex;
            using (Mutex mutex = new Mutex(true, "ADD_RWMAC", out ownmutex))
            {
                if (ownmutex)
                {
                    Application.Run(new Main_Prog());
                    mutex.ReleaseMutex();
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }
        }
    }
}
