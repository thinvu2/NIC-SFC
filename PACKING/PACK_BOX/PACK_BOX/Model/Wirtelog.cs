using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACK_BOX.Model
{
    public class Writelog
    {
        public static void WriteLog(string Log)
        {
            string strAppPath = System.IO.Directory.GetCurrentDirectory() + @"\LOG_PACK_BOX";

            //Server Log
            if (!System.IO.Directory.Exists(@strAppPath))
            {
                System.IO.Directory.CreateDirectory(@strAppPath);
            }

            FileInfo fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"\LOG_PACK_BOX\" + DateTime.Now.ToString("yyyyMMdd") + ".txt");

            if (!fi.Exists)
            {
                //Create a file to write to.
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ": ->" + Log);
                }
            }
            else
            {
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine("\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " : ->" + Log);
                }
            }
        }
    }
}
