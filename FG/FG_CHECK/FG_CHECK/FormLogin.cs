using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOLD
{
    public partial class FormLogin : Form
    {
        private string checkSum;
        SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUrl = "";
        public string loginDB = "";
        public string empNo = "";
        public string empPass = "";
        public string inputLogin = "";
        public string MAC = "",IP="";
        public FormLogin()
        {
            InitializeComponent();

            string[] Args = Environment.GetCommandLineArgs();
            //if (Args.Length == 1)
            //{
            //    MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
            //    Environment.Exit(0);
            //}
            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }
            string[] argsInfor = Regex.Split(inputLogin, @";");
            checkSum = argsInfor[0].ToString();
            //loginApiUrl = argsInfor[1].ToString();
            //loginDB = argsInfor[2].ToString();
            //empNo = argsInfor[3].ToString();
            //empPass = argsInfor[4].ToString();
            //empPass = "TOP12345";
            MAC = GetMacAddress().ToString();
            IP = GetIPAddress().ToString();
            //string neww = GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location);

            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            //    Environment.Exit(0);
            //}
            
           // CheckLogin();
        }

        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }

        public static string GetChecksum(HashingAlgoTypes hashingAlgoType, string filename)
        {
            using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString()))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }

        public enum HashingAlgoTypes
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        public static IPAddress GetIPAddress()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses("");

            foreach (IPAddress hostAddress in hostAddresses)
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(hostAddress) &&  // ignore loopback addresses
                    !hostAddress.ToString().StartsWith("169.254."))  // ignore link-local addresses
                    return hostAddress;
            }
            return null; // or IPAddress.None if you prefer
        }
        public async Task connect()
        {
            sfcconnect sfc = new sfcconnect();

            sfcClient = await sfc.AccessAPI();
            if (sfcClient.AutoRefreshToken == null)
            {
                MessageBox.Show("Connect SFC fail! plz call IT");
                System.Windows.Forms.Application.Exit();
            }
        }
        public static PhysicalAddress GetMacAddress()
        {
            var myInterfaceAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Select(n => n.GetPhysicalAddress())
                .FirstOrDefault();

            return myInterfaceAddress;
        }
        public async void CheckLogin()
        {
            //empNo = txtEmp.Text;
            //empPass = txtPassword.Text;
            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "HOLD",
                UserName = txtEmp.Text,
                Password = txtPassword.Text,
                IP = IP,
                MAC = MAC
            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
           // sfcClient = DBApi.sfcClient(loginDB, loginApiUrl);
            try
            {
             //   await sfcClient.GetAccessTokenAsync(empNo, empPass);

                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic ads = result.Data;
                string Ok = ads[0]["output"];

                if (Ok.Substring(0, 2) == "OK")
                {
                    loginInfor = Ok.Substring(3, Ok.Length - 3).Trim();
                    this.Hide();
                    this.Show();
                }
                else
                {
                    ReportError(Ok);
                    //MessageBox.Show(Ok, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ReportError(ex.Message.ToString());
                //MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);                
                this.Close();
            }
        }

        private async void FormLogin_Load(object sender, EventArgs e)
        {
         //   status_connectDB.Text = "Trying to connect to SFIS server...";
            sfcconnect sfc = new sfcconnect();
            sfcClient = await sfc.AccessAPI();
        }              

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //LoginFun();
            CheckLogin();
        }

        private void txtEmp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //LoginFun();
                CheckLogin();
            }
        }        
    }
}
