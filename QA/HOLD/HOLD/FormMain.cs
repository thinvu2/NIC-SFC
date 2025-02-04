using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.OracleClient;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using System.Text.RegularExpressions;
using System.Deployment.Application;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace HOLD
{
    public partial class FormMain : Form
    {
        //public EmployeeInfomation _empInfo = new EmployeeInfomation();
        //public OracleClientDBHelper dbsfis = null;
        public static string inputLogin, checkSum, baseUrl, loginDB, empNo, empPass, empName, prgName, appVer, MACAddress, IP;

        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public static string lang="VI";

        public FormMain()
        {
            InitializeComponent();
            appVer = getRunningVersion().ToString();
            this.Text = "Hold System -- " + "Version: " + appVer;
            AccessAPI();
            
        }

        private async void AccessAPI()
        {
            string[] Args = Environment.GetCommandLineArgs();
            if (Args.Length == 1)
            {
                MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
                Environment.Exit(0);
            }
            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }
            string[] argsInfor = Regex.Split(inputLogin, @";");
            checkSum = argsInfor[0].ToString();
            baseUrl = argsInfor[1].ToString();
            loginDB = argsInfor[2].ToString();
            empNo = argsInfor[3].ToString();
            empPass = argsInfor[4].ToString();
            MACAddress = GetMacAddress().ToString();
            IP = GetIPAddress().ToString();

            if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            {
                //MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButtons.OK);
                //Environment.Exit(0);
            }
            sfcClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
            try
            {
                await sfcClient.GetAccessTokenAsync(empNo, empPass);
                var loginInfo = new
                {
                    TYPE = "LOGIN",
                    PRG_NAME = "HOLD",
                    UserName = empNo,
                    Password = empPass,
                    IP = IP,
                    MAC = MACAddress
                };

                //Tranform it to Json object
                string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();

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
                        string[] digits = Regex.Split(loginInfor, @";");
                        status_connectedDb.Text = digits[0].ToString();
                        status_loginInfo.Text = digits[1].ToString();
                        empNo = Regex.Split(status_loginInfo.Text, @"-")[0].Trim();
                    var resultAutoHold = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP=:EMP AND FUN='HOLD_AUTO_HOLD' AND PRG_NAME ='HOLD'",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "EMP", Value = empNo }
                    }

                    });
                    if (resultAutoHold.Data.ToList().Count != 0 && resultAutoHold.Data != null)
                    {
                        this.btnAutoHold.Enabled = true;
                    }
                    else
                    {
                        this.btnAutoHold.Enabled = false;
                    }

                    var resultHandHold = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP=:EMP AND FUN='HOLD_HAND_HOLD' AND PRG_NAME ='HOLD'",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "EMP", Value = empNo }
                    }
                    });
                    if (resultHandHold.Data.ToList().Count != 0)
                    {
                        this.btnHandHold.Enabled = true;
                    }
                    else
                    {
                        this.btnHandHold.Enabled = true;
                    }

                    var resultQuery = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP=:EMP AND FUN='HOLD_QUERY' AND PRG_NAME ='HOLD'",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "EMP", Value = empNo }
                    }
                    });
                    if (resultQuery.Data.ToList().Count != 0)
                    {
                        this.btnHoldQuery.Enabled = true;
                        this.btnLogQuery.Enabled = true;
                    }
                    else
                    {
                        this.btnLogQuery.Enabled = false;
                        this.btnHoldQuery.Enabled = false;
                    }
                }
                    else
                    {
                        showMessage(Ok);
                        //MessageBox.Show(Ok, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        this.Close();
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
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
        public static PhysicalAddress GetMacAddress()
        {
            var myInterfaceAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .OrderByDescending(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Select(n => n.GetPhysicalAddress())
                .FirstOrDefault();

            return myInterfaceAddress;
        }
        public static string GetChecksum(HashingAlgoTypes hashingAlgoType, string filename)
        {
            using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString()))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = hasher.ComputeHash(stream);
                    string sss = BitConverter.ToString(hash).Replace("-", "");
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }

        private void tiếngViệtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lang = "VI";
            tiếngViệtToolStripMenuItem.Checked = true;
            englishToolStripMenuItem.Checked = false;
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lang = "EN";
            tiếngViệtToolStripMenuItem.Checked = false;
            englishToolStripMenuItem.Checked = true;
        }

        public enum HashingAlgoTypes
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }
        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private void showMessage(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
        }

        //private async void FormMain_Load(object sender, EventArgs e)
        //{           

            //try
            //{
            //    CommonString.checkFileTNSName(true, true);
            //    dbsfis = new OracleClientDBHelper(CommonString.getConnStrAlias_SFC("SFIS.WORLD", "1235"));
            //    if (!dbsfis.TestConnect(lblerror))
            //    {
            //        SFCMessage.Show("Connect SFIS DB error | Lỗi kết nối tới SFIS DB", "Exception:" + Environment.NewLine + lblerror.Text + Environment.NewLine + CommonString.host + CommonString.port + CommonString.service, "Ngoại lệ:" + Environment.NewLine + lblerror.Text);
            //        Application.Exit();
            //    }
            //    else
            //    {
            //        status_connectedDb.Text = "Connected to: " + CommonString.host;
            //    }

            //    string _productname = Application.ProductName;
            //    dbsfis.CheckAPPVersion(_productname, lblversion);

            //    _empInfo = SFCLogin.Login(false, _productname);
            //    if (!_empInfo.CheckPrivilege(_productname, "Login"))
            //    {
            //        SFCMessage.Show("Login error | Lỗi đăng nhập", "You not have privilege to login!", "Bạn không có quyền để đăng nhập!");
            //        Application.Exit();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        //}

        private void btnHandHold_Click(object sender, EventArgs e)
        {
            HandHold frmHandHold = new HandHold(this);
            //HandHold frmHandHold = new HandHold(loginInfor, sfcClient);

            frmHandHold.loginInfor = this.loginInfor;
            frmHandHold.sfcClient = this.sfcClient;

            this.Hide();
            frmHandHold.ShowDialog();
            this.Show();
        }

        private void btnAutoHold_Click(object sender, EventArgs e)
        {
            AutoHold frmAutoHold = new AutoHold(this);

            //frmAutoHold._empInfo = this._empInfo;
            //frmAutoHold.dbsfis = this.dbsfis;

            frmAutoHold.loginInfor = this.loginInfor;
            frmAutoHold.sfcClient = this.sfcClient;

            this.Hide();
            frmAutoHold.ShowDialog();
            this.Show();
        }

        private void btnHoldQuery_Click(object sender, EventArgs e)
        {
            HoldQuery frmHoldQuery = new HoldQuery(this);

            //frmHoldQuery._empInfo = this._empInfo;
            //frmHoldQuery.dbsfis = this.dbsfis;

            frmHoldQuery.loginInfor = this.loginInfor;
            frmHoldQuery.sfcClient = this.sfcClient;

            this.Hide();
            frmHoldQuery.ShowDialog();
            this.Show();
        }

        private void btnLogQuery_Click(object sender, EventArgs e)
        {
            LogQuery frmLogQuery = new LogQuery(this);

            //frmLogQuery._empInfo = this._empInfo;
            //frmLogQuery.dbsfis = this.dbsfis;

            frmLogQuery.loginInfor = this.loginInfor;
            frmLogQuery.sfcClient = this.sfcClient;

            this.Hide();
            frmLogQuery.ShowDialog();
            this.Show();
        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ChangePass frmChangePass = new ChangePass();
            //this.Hide();
            //frmChangePass.ShowDialog();
            //this.Show();
        }

        private async void frmMain_Shown(object sender, EventArgs e) 
        {
            
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
