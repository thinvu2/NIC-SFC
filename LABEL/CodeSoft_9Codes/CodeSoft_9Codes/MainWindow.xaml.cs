using CodeSoft_9Codes.View;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using Sfc.Library.HttpClient;

namespace CodeSoft_9Codes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string checkSum;
        public static SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUri = "";
        public string loginDB = "";
        public static string empNo = "",PCIP,PCMAC,APVersion;
        public string empPass = "";
        public string Plant = "";
        public string inputLogin = "";
        public string sEmpName = "";
        public static string UrlLabelFile = "";
        public MainWindow()
        {
            InitializeComponent();
            string[] Args = Environment.GetCommandLineArgs();

            if (Args.Length == 1)
            {
                MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }
            string[] argsInfor = Regex.Split(inputLogin, @";");
            checkSum = argsInfor[0].ToString();
            loginApiUri = argsInfor[1].ToString();
            loginDB = argsInfor[2].ToString();
            empNo = argsInfor[3].ToString();
            empPass = argsInfor[4].ToString();
            PCIP = GetLocalIPAddress();
            PCMAC = GetMACAddress();
            APVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //    Environment.Exit(0);
            //}
            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "CODESOFT",
                UserName = empNo,
                Password = empPass
            };
            //Tranform it to Json object
            ApiConnect(loginInfo);
        }
        public async void ApiConnect(object loginInfo)
        {
            sfcClient = new SfcHttpClient(loginApiUri, loginDB, "helloApp", "123456");
            await sfcClient.GetAccessTokenAsync(empNo, empPass);
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            try
            {
                var result1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                });

                dynamic ads = result1.Data;
                string Ok = ads[0]["output"];

                if (Ok.Substring(0, 2) == "OK")
                {
                    loginInfor = Ok.Substring(3, Ok.Length - 3).Trim();
                    string[] digits = Regex.Split(loginInfor, @";");
                    lblDB.Content = "Database connect: " + loginDB;
                    //string emppass = digits[1].Substring(10, digits[1].ToString().Length - 10).ToString();
                    lblName.Content = "User: " + digits[1].ToString();
                    sEmpName = digits[1].ToString();
                    lblVersion.Content = "Code Soft 9 Codes Version: " + APVersion;
                    lblIP.Content = "IP address: " + PCIP + "-" + PCMAC;
                    UrlLabelFile = digits[2].ToString();
                    //panelLoad.Children.Add(new BackGround());
                    
                }
                else
                {
                    MessageBox.Show(Ok);
                    this.Close();
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                this.Close();
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

        public static string GetChecksum(HashingAlgoTypes hashingAlgoType, Stream fileStream)
        {
            using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString()))
            {
                using (var stream = fileStream)
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

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set tooltip visibility

            if (Tg_Btn.IsChecked == true)
            {
                tt_home.Visibility = Visibility.Collapsed;
                tt_vn.Visibility = Visibility.Collapsed;
                tt_nn.Visibility = Visibility.Collapsed;
                tt_signout.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_home.Visibility = Visibility.Visible;
                tt_vn.Visibility = Visibility.Visible;
                tt_nn.Visibility = Visibility.Visible;
                tt_signout.Visibility = Visibility.Visible;
            }
        }

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 1;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 0.3;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Close();
        }


        private void PtintAmbit_click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You clicked me");
        }

        
        private async void mainWindow_Initialized(object sender, EventArgs e)
        {
            //sfcconnect conn = new sfcconnect();
            //sfcClient = await conn.AccessAPI();
        }

        private void Exit_Click(object sender, MouseButtonEventArgs e)
        {
            if (LabelHHVN.isPrinting)
            {
                MessageBox.Show("Printing!!!", "Warning!!!");
                return;
            }
            if (MessageBox.Show("Do you want to close this window?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            
        }
        private void NewProduct_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            panelLoad.Children.Clear();
            //panelLoad.Children.Add(new UC_Newproduct(sfcClient, empNo, Plant));
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {

            }
        }

        

        private void HHVN_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (LabelHHVN.isPrinting)
            {
                MessageBox.Show("Printing!!!", "Warning!!!");
                return;
            }
            panelLoad.Children.Clear();
            lblContent.Content = "PRINT LABEL HON HAI";
            panelLoad.Children.Add(new LabelHHVN(sfcClient,sEmpName,empNo));
        }

        public static string GetMACAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    string mac = AddressBytesToString(nic.GetPhysicalAddress().GetAddressBytes()).Trim();
                    if (string.IsNullOrEmpty(mac))
                    {
                        return GetLocalIPAddress();
                    }
                    else
                    {
                        return mac;
                    }
                }
            }

            return string.Empty;
        }

        private static string AddressBytesToString(byte[] addressBytes)
        {
            return string.Join(":", (from b in addressBytes
                                     select b.ToString("X2")).ToArray());
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

        private void ReleaseHH_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if (LabelHHVN.isPrinting)
            //{
            //    MessageBox.Show("Printing!!!", "Warning!!!");
            //    return;
            //}
            panelLoad.Children.Clear();
            lblContent.Content = "RELEASE LABEL HON HAI";
            panelLoad.Children.Add(new ReleaseLabel(sfcClient, sEmpName, empNo));
        }
        private void Home_Mouse_Click(object sender, MouseButtonEventArgs e)
        {
            if (LabelHHVN.isPrinting)
            {
                MessageBox.Show("Printing!!!", "Warning!!!");
                return;
            }
            panelLoad.Children.Clear();
            lblContent.Content = "CodeSoft NIC";
            panelLoad.Children.Add(new CodeSoft_NIC());
        }
        private void Content_Main_Loaded(object sender, RoutedEventArgs e)
        {
            panelLoad.Children.Clear();
            lblContent.Content = "CodeSoft NIC";
            panelLoad.Children.Add(new CodeSoft_NIC());
        }
    }
}
