using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;
using System.Net.NetworkInformation;
using System.IO;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Text.RegularExpressions;
using System.Diagnostics;
using REWORK.UserControlRework;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using Path = System.IO.Path;
using REWORK.Models;
using PACK_CTN.Models;
using PACK_CTN;

namespace REWORK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SfcHttpClient sfcHttpClient;
        public string[] RESArray = { "NULL" };
        public static Oracle oracle = null;
        public static string inputLogin, checkSum, baseUrl, loginDB, empNo, empPass , empName , prgName , appVer ,MACAddress, IP ;
        public static Boolean ChkLogin = false ;


        public MainWindow()
        {
            InitializeComponent();
           // Login main = new Login();
           //sfcHttpClient =   main._sfcHttpClient;
           
        }
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
           
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                foreach (var item in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)))
                {
                    WindowHelper.BringProcessToFront(item);
                }
                Process.GetCurrentProcess().Kill();
            }
            InitializeComponent();
            Process currentProcess = new Process();

            if (!await  AccessAPI())
            {
                return;
            }
            MACAddress = GetMacAddress();
            IP = GetLocalIPAddress();
            lblIP.Content = IP;
            lblMAC.Content = MACAddress;
            btnMain.Background = Brushes.OrangeRed;
            btnMain.Foreground = Brushes.White;

            if (!await Login())
                {
                    return;
                }
                panelMain.Children.Add(new ucMainForm());
            
        }

        public async Task<Boolean> Login()
        {
            try
            {
                var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { 
                    CommandText = "select * from SFIS1.c_emp_desc_t where emp_pass ='"+ empPass + "'",
                    SfcCommandType = SfcCommandType.Text
                });

             
                if (result.Data != null)
                {
                    if (await checkPrivilege(empNo, "SFCREWORK"))
                    {
                        lblEmpName.Content = result.Data["emp_name"];
                        itemNameEmp.Content = "Hi: " + result.Data["emp_name"];
                        return true;
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "Employee No PRIVILEGE !";
                        FrmMessage.MessageEnglish = "Tài khoản không có quyền !";
                        FrmMessage.ShowDialog();
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = ex.Message;
                FrmMessage.MessageEnglish = " Login error !";
                FrmMessage.ShowDialog();
                return false;
            }
        }

        private async Task<bool> AccessAPI()
        {
            string[] Args = Environment.GetCommandLineArgs();
            if (Args.Length == 1)
            {
                MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                Environment.Exit(0);
                return false;
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

            itemDbConnected.Content = "DB: " + loginDB.ToUpper();

            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //    Environment.Exit(0);
            //}

            sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
            await sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

            oracle = new Oracle(sfcHttpClient);
            prgName = "Rework";
            appVer = getRunningVersion().ToString();
            lblVerson.Content = appVer;
            lblDBName.Content = loginDB.ToUpper();
            return true;

        }

        private string GetMacAddress()
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
            int i;
            string mac = "";
            for (i = 0; i <= macAddresses.Length - 2; i = i + 2)
            {
                if (i == macAddresses.Length - 2)
                {
                    mac = mac + macAddresses.Substring(i, 2);
                }
                else
                    mac = mac + macAddresses.Substring(i, 2) + ':';
            }
            return mac;
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

        private async Task<bool> CheckAppVersion(string AppName, string AppVersion)
        {

            VersionModel versionModel = await oracle.GetObj<VersionModel>($"select ap_version from SFISM4.AMS_AP where upper(AP_NAME) = upper('{AppName}') ");
            if (versionModel != null)
            {
                string _AppVerAMS = versionModel.ap_version;

                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

                var resultCompare = fvi.FileVersion.CompareTo(_AppVerAMS);

                if (resultCompare < 0)
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = string.Format("Chương trình {0} đã có phiên bản mới. Vui lòng cập nhật bằng SFIS_AMS. Phiên bản mới: {1} - Phiên bản hiện tại: {2}. Nhập 9999 để đóng thông báo.", AppName, _AppVerAMS, AppVersion);
                    FrmMessage.MessageEnglish = string.Format("Program {0} have new version. Please upgrade by SFIS_AMS. New version: {1} - Current version: {2}.  Input 9999 to close message box.", AppName, _AppVerAMS, AppVersion);
                    FrmMessage.ShowDialog();
                    return false;
                }
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = string.Format("Chương trình {0} không có trên hệ thống SFIS_AMS. Vui lòng liên hệ IT để xử lí! Nhập 9999 để đóng thông báo.", AppName);
                FrmMessage.MessageEnglish = string.Format("Program {0} not found on the SFIS_AMS system. Please find IT to resolve! Input 9999 to close message box.", AppName);
                FrmMessage.ShowDialog();
                return false;
            }
            //  MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
           // MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
            return true;
        }

        private Version getRunningVersion()
        {
            try
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        private string getMacAddress()
        {
            string macAddress = string.Empty;
            int i;
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up )
                {
                    macAddress += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            for (i=0;i <= macAddress.Length -2; i = i + 2 )
            {
                if (i== macAddress.Length - 2)
                {
                    mac = mac + macAddress.Substring(i, 2);
                }
                else
                {
                    mac = mac + macAddress.Substring(i, 2);
                }
            }
            return mac;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
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

        public enum HashingAlgoTypes
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }
        public async Task<bool> checkPrivilege (string EmpPass , string FuntionCheck)
        {
           
            var logInfo = new
            {
                OPTION = "CHECKPRIVE" ,
                PASS = EmpPass ,
                FUNTION = FuntionCheck
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            try
            {
                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType .StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter {Name = "DATA" ,Value = jsonData, SfcParameterDataType = SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name = "OUTPUT" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                    }
                });
                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "OK")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch 
            {
                return false;
            }
            
        }
        private async void btnMain_Click(object sender, RoutedEventArgs e)
        {
            funtionClick(1);
            panelMain.Children.Clear();
            if (await checkPrivilege(empNo, "SFCREWORK"))
                {
                    panelMain.Children.Clear();
                    panelMain.Children.Add(new ucMainForm());
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "MÃ THẺ KHÔNG CÓ QUYÊN REWORK ";
                    FrmMessage.MessageEnglish = "EMP NO PRIVILEGE";
                    FrmMessage.ShowDialog();
                    return;
                }
            
        }

        private  void btnTRework_Click(object sender, RoutedEventArgs e)
        {
           funtionClick(2);
           panelMain.Children.Clear();
           // panelMain.Children.Add(new ucTReworkForm());

        }
        private void btnSystem_Click(object sender, RoutedEventArgs e)
        {
            funtionClick(3);
        }

        private async void btnRepair_Click(object sender, RoutedEventArgs e)
        {
            funtionClick(4);
            if (await checkPrivilege(empNo, "DELETE_R_"))
            {
                panelMain.Children.Clear();
                panelMain.Children.Add(new ucTReworkForm());
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "MÃ THẺ KHÔNG CÓ QUYÊN XÓA CĂN TẠI TRẠM ";
                FrmMessage.MessageEnglish = "";
                FrmMessage.ShowDialog();
                return;
            }

        }
        private async void btnUpFW_Click(object sender, RoutedEventArgs e)
        {
            funtionClick(5);
            if (await checkPrivilege(empNo, "CUTIN_FW"))
            {
                panelMain.Children.Clear();
                panelMain.Children.Add(new ucCutInFW());
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "MA THE NAY KHONG CO QUYEN ";
                FrmMessage.MessageEnglish = "";
                FrmMessage.ShowDialog();
                return;
            }
        }
        private async void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            funtionClick(6);
            if (await checkPrivilege(empNo, "PROCESS_ISSUE"))
            {
                panelMain.Children.Clear();
                panelMain.Children.Add(new ucControlProcess());
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "MA THE NAY KHONG CO QUYEN ";
                FrmMessage.MessageEnglish = "";
                FrmMessage.ShowDialog();
                return;
            }
        }
        private void funtionClick (int funt)
        {
            btnMain.Background = Brushes.White;
            btnTRework.Background = Brushes.White;
           // btnSystem.Background = Brushes.White;
            btnRepair.Background = Brushes.White;
            btnUpFW.Background = Brushes.White;
            btnProcess.Background = Brushes.White;
            btnMain.Foreground = Brushes.Black;
            btnTRework.Foreground = Brushes.Black;
            //btnSystem.Foreground = Brushes.Black;
            btnRepair.Foreground = Brushes.Black;
            btnUpFW.Foreground = Brushes.Black;
            btnProcess.Foreground = Brushes.Black;
            if (funt == 1 )
            {
                btnMain.Background = Brushes.OrangeRed;
                btnMain.Foreground = Brushes.White;
            }
            else if (funt == 2)
            {
                btnTRework.Background = Brushes.OrangeRed;
                btnTRework.Foreground = Brushes.White;
            }
            else if (funt == 3)
            {
               // btnSystem.Background = Brushes.OrangeRed;
                //btnSystem.Foreground = Brushes.White;
            }
            else if (funt == 4)
            {
                btnRepair.Background = Brushes.OrangeRed;
                btnRepair.Foreground = Brushes.White;
            }
            else if (funt == 5)
            {
                btnUpFW.Background = Brushes.OrangeRed;
                btnUpFW.Foreground = Brushes.White;
            }
            else if (funt == 6)
            {
                btnProcess.Background = Brushes.OrangeRed;
                btnProcess.Foreground = Brushes.White;
            }
        }
       
    }
}
