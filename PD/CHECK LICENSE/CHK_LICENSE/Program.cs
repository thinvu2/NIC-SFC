using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace CHK_LICENSE
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool ownmutex;
            using (Mutex mutex = new Mutex(true, "CHK_LICENSE", out ownmutex))
            {
                if (ownmutex)
                {
                    System.Windows.Forms.Application.Run(new Form1());
                    mutex.ReleaseMutex();
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }

}
