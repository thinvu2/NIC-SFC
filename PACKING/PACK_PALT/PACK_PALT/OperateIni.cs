using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace MES.OpINI
{
    # region Ini reader
    /// <summary>
    /// INI reader
    /// </summary> 
    public class IniUtil
    {

        //Ini API
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
            string key, string def, StringBuilder retVal,
            int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);


        public IniUtil()
        {
            //
            // TODO: 
            //
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iniFileName">
        /// <param name="sectionName">¬q</param>
        /// <param name="keyName">key¦W</param>
        /// <returns></returns>
        public static string ReadINI(string iniFileName, string sectionName, string keyName)
        {
            try
            {
                StringBuilder keyValue = new StringBuilder(5000);
                int i = GetPrivateProfileString(sectionName, keyName, "", keyValue, 5000, iniFileName);
                return (keyValue.ToString().Trim());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="iniFileName">ini</param>
        /// <param name="sectionName">¬q</param>
        /// <param name="keyName">key¦W</param>
        /// <param name="isCipherText"></param>
        /// <returns></returns>
        public static string ReadINI(string iniFileName, string sectionName, string keyName, bool isCipherText)
        {
            IniUtil iniut = new IniUtil();
            StringBuilder keyValue = new StringBuilder(255);
            int i = GetPrivateProfileString(sectionName, keyName, "", keyValue, 255, iniFileName);
            if (isCipherText)
            {
                string strkeyValue = iniut.ProclaimText(keyValue.ToString().Trim());
                return (strkeyValue.ToString().Trim());
            }
            else
            {
                return (keyValue.ToString().Trim());
            }

        }

        /// <summary>
        /// Write Ini
        /// </summary>
        /// <param name="iniFileName"></param>
        /// <param name="sectionName"></param>
        /// <param name="keyName">key¦W</param>
        /// <param name="keyValue">key­</param>
        public static void WriteINI(string iniFileName, string sectionName, string keyName, string keyValue)
        {
            WritePrivateProfileString(sectionName, keyName, keyValue, iniFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCipherText"></param>
        /// <returns></returns>
        private string ProclaimText(string strCipherText)
        {
            string str1 = "";
            string str2 = "";
            try
            {
                for (short i = 0; i < strCipherText.Length / 3; i++)
                {
                    str2 = Convert.ToChar(Convert.ToInt32(strCipherText.Substring(i * 3, 3))).ToString();
                    str1 += str2.ToString();
                }
                return str1;
            }
            catch
            {
                return "";
            }
        }

        public static void WriteLog(string Log)
        {
            string strAppPath = System.IO.Directory.GetCurrentDirectory() + "\\LOG";
            string strYYYYMMDD = DateTime.Now.ToString("yyyyMMdd");

            //Server Log
            strAppPath = strAppPath + "\\LOG";
            if (!System.IO.Directory.Exists(@strAppPath))
            {
                System.IO.Directory.CreateDirectory(@strAppPath);
            }

            FileInfo fi = new FileInfo(@strAppPath + "\\" + strYYYYMMDD + ".TXT");


            if (!fi.Exists)
            {
                //Create a file to write to.
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine("\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "   \n" + Log);
                }
            }
            else
            {
                using (StreamWriter sw = fi.AppendText())
                {
                    sw.WriteLine("\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "   \n" + Log);
                }
            }

        }
        /// <summary>
        /// 
        /// </summary>
        ///Server Log

    }
    # endregion
}
