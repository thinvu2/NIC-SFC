using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PRINT_ALLPARTS.Class
{
	/// <summary>
	/// Description of TIniFile.
	/// </summary>
	public static class TIniFile
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
        public static string ReadINI(string iniFileName, string sectionName, string keyName)
        {
            StringBuilder keyValue = new StringBuilder(255);
            int i = GetPrivateProfileString(sectionName, keyName, "", keyValue, 255, iniFileName);
            return (keyValue.ToString().Trim());
        }

        public static void WriteINI(string iniFileName, string sectionName, string keyName, string keyValue)
        {
            WritePrivateProfileString(sectionName, keyName, keyValue, iniFileName);
        }
    }
}
