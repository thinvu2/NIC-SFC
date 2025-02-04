﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BRCM_B2B.mail
{
    class INIFile
    {
        private string filePath;
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpszReturnBuffer, int nSize, string lpFileName);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
        string key,
        string val,
        string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
        string key,
        string def,
        StringBuilder retVal,
        int size,
        string filePath);
        public INIFile(string filePath)
        {
            this.filePath = filePath;
        }
        public List<string> GetKeys(string section)
        {

            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(section, buffer, 2048, this.filePath);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');

            List<string> result = new List<string>();

            foreach (String entry in tmp)
            {
                // result.Add(entry.Substring(0, entry.IndexOf("=")));
                result.Add(entry);
            }

            return result;
        }

        public List<string> GetDB(string section)
        {

            byte[] buffer = new byte[2048];

            GetPrivateProfileSection(section, buffer, 2048, this.filePath);
            String[] tmp = Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');

            List<string> result = new List<string>();

            foreach (String entry in tmp)
            {
                result.Add(entry.Substring(0, entry.IndexOf("=")));
            }

            return result;
        }
        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, this.filePath);
        }

        public string Read(string section, string key)
        {
            StringBuilder SB = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", SB, 255, this.filePath);
            return SB.ToString();
        }
        public void Lock_forders(string folderPath)
        {
            try
            {
                string adminUserName = Environment.UserName;// getting your adminUserName
                DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.FullControl, AccessControlType.Deny);
                ds.AddAccessRule(fsa);
                Directory.SetAccessControl(folderPath, ds);
            }
            catch
            {
                // MessageBox.Show(ex.Message);
            }
        }
        public void UnLock_forders(string folderPath)
        {
            try
            {
                string adminUserName = Environment.UserName;// getting your adminUserName
                DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                FileSystemAccessRule fsa = new FileSystemAccessRule(adminUserName, FileSystemRights.FullControl, AccessControlType.Deny);
                ds.RemoveAccessRule(fsa);
                Directory.SetAccessControl(folderPath, ds);
            }
            catch
            {
                //  MessageBox.Show(ex.Message);
            }


        }
        public string FilePath
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static string GetMacAddress()
        {
            string macAddresses = string.Empty;

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            return macAddresses;
        }
    }
}
