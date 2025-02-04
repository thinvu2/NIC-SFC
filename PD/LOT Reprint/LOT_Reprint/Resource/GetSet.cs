using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOT_REPRINT.Resource
{
    class GetSet
    {
        public static string COMSEND
        {
            get
            {
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                if(string.IsNullOrEmpty(ini.IniReadValue("LOTREPRINT", "COMSEND")))
                {
                    ini.IniWriteValue("LOTREPRINT", "COMSEND", "COM3");
                }
                return ini.IniReadValue("LOTREPRINT", "COMSEND");
            }
        }
        public static string COMRECEIVE
        {
            get
            {
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                if (string.IsNullOrEmpty(ini.IniReadValue("LOTREPRINT", "COMRECEIVE")))
                {
                    ini.IniWriteValue("LOTREPRINT", "COMRECEIVE", "COM2");
                }
                return ini.IniReadValue("LOTREPRINT", "COMRECEIVE");
            }
        }
    }
}
