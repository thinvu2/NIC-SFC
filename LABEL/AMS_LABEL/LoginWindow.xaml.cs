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
using System.Windows.Shapes;
using AMSLabel;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;

namespace AMSLabel
{

    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
   
    public partial class LoginWindow : Window
    {
        private string checkSum;
        SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUri = "";
        public string loginDB = "";
        public string empNo = "";
        public string empPass = "";
        public string inputLogin = "";
        public static string PCIP, PCMAC, PrgName = "AMSLABEL";

        public LoginWindow()
        {
            InitializeComponent();
            PCIP = GetIPAddress().ToString();
            PCMAC = GetMACAddress();
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
            loginApiUri =argsInfor[1].ToString();
            loginDB=argsInfor[2].ToString();
            empNo= argsInfor[3].ToString();
            empPass= argsInfor[4].ToString();
            string temp = GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (checkSum != temp)
            {
                MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                //Environment.Exit(0);
            }
            Button_Click(new object(), new RoutedEventArgs());
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
        public static string GetMACAddress()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    string mac = AddressBytesToString(nic.GetPhysicalAddress().GetAddressBytes()).Trim();
                    if (string.IsNullOrEmpty(mac))
                    {
                        return "";
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var loginInfo = new
            {
                TYPE="LOGIN",
                PRG_NAME ="AMSLABEL",
                UserName = empNo,
                Password = empPass
            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            sfcClient = DBApi.sfcClient(loginDB, loginApiUri);
            try
            {
                await sfcClient.GetAccessTokenAsync(empNo, empPass);

                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE ",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}

                }

                });

                dynamic ads = result.Data;
                string Ok = ads[0]["output"];
                //--------------------------------------------------------------------------------------------------------
                var Z1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "select SFIS1.CHECKPC('" + PrgName + "','" + PCIP + "','" + PCMAC + "') as res from dual",
                    SfcCommandType = SfcCommandType.Text,

                });
                if (Z1.Data["res"]?.ToString() != "OK")
                {
                    MessageBox.Show(Z1.Data["res"]?.ToString(), "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    txt_Password.Clear();
                    this.Close();
                }
                //--------------------------------------------------------------------------------------------------------
                if (Ok.Substring(0,2) == "OK")
                {
                    loginInfor = Ok.Substring(3, Ok.Length-3).Trim();
                    MainWindow main = new MainWindow(loginInfor, sfcClient);
                    main.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show(Ok, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    txt_Password.Clear();
                    this.Close();

                }
                
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                txt_Password.Clear();
                this.Close();
            }
        }
    }
}
