using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GemaltoRoast
{
    class OperateINI
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);//¼gINIªºAPI¨ç¼ÆªºÁn©ú

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);//ÅªINIªºAPI¨ç¼ÆªºÁn©ú
        public string ReadINI(string iniFileName, string sectionName, string keyName)
        {
            StringBuilder keyValue = new StringBuilder(255);
            int i = GetPrivateProfileString(sectionName, keyName, "", keyValue, 255, iniFileName);
            return (keyValue.ToString().Trim());
        }

        public void WriteINI(string iniFileName, string sectionName, string keyName, string keyValue)
        {
            WritePrivateProfileString(sectionName, keyName, keyValue, iniFileName);
        }
    }
}
