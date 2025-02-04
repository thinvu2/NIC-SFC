using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;
using CHECK_SYS.DataObject;
using System.IO.Ports;
using System.Windows.Threading;
using LabelManager2;
using System.IO;
namespace CHECK_SYS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow openWD { get; private set; }
        SfcHttpClient _sfcHttpClient;
        SfcHttpClient _sfcHttpClientAllparts;
        public DAL dal;
        string loginApiUri = "";
        string loginDB = "";
        string empNo = "";
        string empPass = "";
        string inputLogin = "";
        string checkSum;
        private string _res;
        dynamic I_stuff;
        string mac = "", ip = "";
        string empPW = "",license = "",qty = "",modelName = "";
        public static int check_role = 0;
        string ap_version_server = "";
        string apname ="", localVer = "",apver = "";
        string cartonNo = "";
        public static string lang = "EN";
        string sql = "";
        List<ResultTable> lst = new List<ResultTable>();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Load;
        }
                   

        private void dtgRS_OnAutoGenerating(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ExtensionData")
            {
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }
       
       
        private void clearError()
        {
            txtError.Text = "";
        }
        private void clearAll()
        {
            txtEmpPwd.Clear();
            txtCartonNo.Clear();
            txtLicense.Clear();
            txtQty.Clear();
        }

       
        private void txtEmpNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            check_role = 0;
            clearAll();
        }

        public void setMessage(string ms)
        {
            if ("EN".Equals(lang) || lang  == "EN")
            {
                txtError.Text = ms.Split('|')[0];               
            }
            else
            {
                txtError.Text = ms.Split('|')[1];

            }
        }

        private async void txtCatxtCartonNo_Keypress(object sender, KeyEventArgs e)
        {
            string inData = "";
            clearError();
            if (check_role == 0)
            {
                setMessage("Input Emp No and Emp Pass first!|Nhập Emp No và Emp Pass trước!");
                return;
            }
            else
            {
                if (e.Key == Key.Return)
                {
                    cartonNo = txtCartonNo.Text.ToString().Trim();
                    if ("".Equals(cartonNo) || cartonNo == "")
                    {
                        txtError.Foreground = Brushes.Red;
                        setMessage("Carton No cannot be blank|Carton No không được để trống");
                        return;
                    }
                    else
                    {
                        clearError();
                        cartonNo = cartonNo.ToUpper();
                        txtCartonNo.Text = cartonNo;

                        ///Avoid case scan dup
                        txtModelName.Text = "";
                        txtLicense.Text = "";
                        txtQty.Text = "";

                        inData = "APVER:" + localVer + " |MYGROUP:CHK_SYS|FUNC:CHECK_CARTON|EMP:" + empNo + "|PWD:" + empPW + "|CARTON:" +cartonNo+ "|PN:" + "|LICENSE:" + "|QTY:";
                        string res = await dal.CheckCartonNo(inData);
                         if (res.StartsWith("OK") || res.Contains("OK"))
                         {
                            txtModelName.IsEnabled = true;
                            txtModelName.Focus();
                            setMessage(res);
                            txtError.Foreground = Brushes.Green;
                        }
                         else
                         {
                            setMessage(res);
                            txtError.Foreground = Brushes.Red;
                            txtCartonNo.SelectAll();
                            txtCartonNo.Focus();                         
                            return;
                         }
                        
                    }
                    
                }
            }
        }

        private void txtEmpNO_Keypress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                empPW = txtEmpPwd.Password.ToString();
                empNo = txtEmpNo.Text.ToString().Trim();
                if (empNo.ToUpper().Trim()=="PD")
                {
                    setMessage("EmpNo:PD no privilege|Mã thẻ PD không có quyền hạn");
                    return;
                }
                if ("".Equals(empNo) || empNo.Length == 0)
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("Emp No cannot null|Mã thẻ không được để trống");
                    return;
                }
                if (empPW.Length == 0 || "".Equals(empPW))
                {
                    txtEmpPwd.Focus();
                    return;
                }
            }
        }


        private async void txtLicense_Keypress(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Return)
            {
                string inData = "";
                modelName = txtModelName.Text.ToString().Trim();
                license = txtLicense.Text.ToString().Trim();
                cartonNo = txtCartonNo.Text.Trim();
                if ("".Equals(cartonNo) || cartonNo.Length == 0)
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("Carton No cannot be blank|Mã carton không được để trống");
                    txtCartonNo.Focus();
                    return;
                }
                if ("".Equals(modelName) || modelName.Length == 0)
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("P/Ncannot be blank|P/N không được để trống");
                    txtModelName.Focus();
                    return;
                }
                if ("".Equals(license) || license.Length == 0 )
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("License No cannot be blank|Mã License không được để trống");
                    txtLicense.Focus();
                    return;
                }
                inData = "APVER:" + localVer + " |MYGROUP:CHK_SYS|FUNC:CHECK_LICENSE|EMP:" + empNo + "|PWD:" + empPW + "|CARTON:" + cartonNo + "|PN:" +modelName+ "|LICENSE:" + license+" |QTY:";
                string res = await dal.CheckCartonNo(inData);
                if (res.StartsWith("OK") || res.Contains("OK"))
                {
                    txtQty.IsEnabled = true;
                    txtQty.SelectAll();
                    txtQty.Focus();
                    setMessage(res);
                    txtError.Foreground = Brushes.Green;
                }
                else
                {
                    setMessage(res);
                    txtError.Foreground = Brushes.Red;
                    txtLicense.SelectAll();
                    txtLicense.Focus();
                    return;
                }

            }
        }

        private async void txtQty_Keypress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string inData = "";
                modelName = txtModelName.Text.ToString().Trim();
                license = txtLicense.Text.ToString().Trim();
                cartonNo = txtCartonNo.Text.Trim();
                string strQty = txtQty.Text.ToString().Trim();
                int Intqty = Int32.Parse(strQty.Substring(1, strQty.Length - 1).ToString());
                qty = Intqty.ToString();
                if ("".Equals(cartonNo) || cartonNo.Length == 0)
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("Carton No cannot be blank|Mã carton không được để trống");
                    txtCartonNo.Focus();
                    return;
                }
                if ("".Equals(modelName) || modelName.Length == 0)
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("P/N cannot be blank|P/N không được để trống");
                    txtModelName.Focus();
                    return;
                }
                if ("".Equals(license) || license.Length == 0 )
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("License No cannot be blank|Mã License không được để trống");
                    txtLicense.Focus();
                    return;
                }
                if ("".Equals(qty) || qty.Length == 0 )
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("Qty cannot be blank|Số lượng không được để trống");
                    txtQty.Focus();
                    return;
                }
                inData = "APVER:" + localVer + " |MYGROUP:CHK_SYS|FUNC:CHECK_QTY|EMP:" + empNo + "|PWD:" + empPW + " |CARTON:" + cartonNo + "|PN:" + modelName + "|LICENSE:" + license + "|QTY:"+qty;
                string res = await dal.CheckCartonNo(inData);
                ResultTable rs = new ResultTable();
                rs.CARTON_NO = cartonNo;
                rs.MODEL_NAME = modelName;
                rs.LICENSE = license;
                rs.QTY = qty;
                rs.NOTE = res.ToString().Trim();
                if (res.StartsWith("OK") || res.Contains("OK"))
                {                    
                    txtCartonNo.IsEnabled = true;
                    txtCartonNo.Text="";
                    txtModelName.Text = "";
                    txtLicense.Text = "";
                    txtQty.Text = "";
                    txtCartonNo.Focus();
                    setMessage(res);
                    txtError.Foreground = Brushes.Green;
                    rs.RESULT = "PASS";
                    lst.Add(rs);
                    dtgRS.ItemsSource = null;
                    dtgRS.ItemsSource = lst;
                }
                else
                {
                    setMessage(res);
                    txtError.Foreground = Brushes.Red;
                    txtQty.SelectAll();
                    txtQty.Focus();
                    rs.RESULT = "FAIL";
                    lst.Add(rs);
                    dtgRS.ItemsSource = null;
                    dtgRS.ItemsSource = lst;
                    return;
                }
            }
        }

        private void txtModelName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async void txtModelName_Keypress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string inData = "";
                modelName = txtModelName.Text.ToString().Trim();
                cartonNo = txtCartonNo.Text.Trim();
                if ("".Equals(cartonNo) || cartonNo.Length == 0)
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("Carton No cannot be blank|Mã carton không được để trống");
                    txtCartonNo.Focus();
                    return;
                }
                if ("".Equals(modelName) || modelName.Length == 0 )
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("P/N cannot be blank|P/N không được để trống");
                    txtModelName.Focus();
                    return;
                }
                inData = "APVER:" + localVer + "|MYGROUP:CHK_SYS|FUNC:CHECK_PN|EMP:" + empNo + "|PWD:" + empPW + "|CARTON:" + cartonNo + "|PN:" + modelName + "|LICENSE:" + "|QTY:";
                string res = await dal.CheckCartonNo(inData);
                if (res.StartsWith("OK") || res.Contains("OK"))
                {
                    txtLicense.IsEnabled = true;
                    txtLicense.Focus();
                    setMessage(res);
                    txtError.Foreground = Brushes.Green;
                }
                else
                {
                    setMessage(res);
                    txtError.Foreground = Brushes.Red;
                    txtModelName.SelectAll();
                    txtModelName.Focus();
                    return;
                }

            }
        }

        private async void txtEmpPwd_Keypress(object sender, KeyEventArgs e)
        {
            empPW = txtEmpPwd.Password.ToString().Trim();
            empNo = txtEmpNo.Text.Trim();
            string inData = "";
            if (e.Key == Key.Return)
            {
                clearError();
                if ("".Equals(empPW) || empPW.Length == 0 || empNo.Length == 0)
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage("EmpNo and Emp pass cannot be blank|Mã thẻ và mật khẩu không được để trống");
                    return;
                }
                empNo = txtEmpNo.Text.Trim();
                inData = "APVER:" + localVer + " |MYGROUP:CHK_SYS|FUNC:CHECK_EMP|EMP:" + empNo + "|PWD:" + empPW + "|CARTON:" + "|PN:" + "|LICENSE:" + " |QTY:";
                _res = await dal.CheckLogin(inData);
                if (_res.ToString().Contains("OK"))
                {
                    Status.Text = "Employee: " + empNo + "       |       " + "IP: " + ip + "       |       " + "MAC: " + mac;
                    check_role = 1;
                    setMessage(_res.ToString());
                    txtCartonNo.IsEnabled = true;
                    txtCartonNo.Focus();
                    txtError.Foreground = Brushes.Green;
                }
                else
                {
                    txtError.Foreground = Brushes.Red;
                    setMessage(_res.ToString());
                    txtCartonNo.IsEnabled = false;
                    txtEmpPwd.SelectAll();
                    txtEmpPwd.Focus();
                    check_role = 0;
                    return;
                }                   
            }                     
        }



        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
     

        private void EN_Click(object sender, RoutedEventArgs e)
        {
            if (EN.IsChecked == true)
            {
                VI.IsChecked = false;
                lang = "EN";
            }
        }

        private void VI_Click(object sender, RoutedEventArgs e)
        {
            if (VI.IsChecked == true)
            {
                EN.IsChecked = false;
                lang = "VI";
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public async void MainWindow_Load(object sender, EventArgs e)
        {
            try
            {
                if (openWD != null)
                {
                    MessageBox.Show("Application is already running !", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    setMessage("Application is already running!|");
                    Environment.Exit(0);
                }
                else
                {
                    openWD = this;
                }

                string[] Args = Environment.GetCommandLineArgs();
                if (Args.Length == 1)
                {
                    //MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    //closePort();
                    //Environment.Exit(0);
                }
                foreach (string s in Args)
                {
                    inputLogin = s.ToString();
                }
                string[] argsInfor = Regex.Split(inputLogin, @";");
                checkSum = argsInfor[0].ToString();
                loginApiUri = argsInfor[1].ToString();
                //loginApiUri = "http://10.220.96.223:8080/sfcapi";
                loginDB = argsInfor[2].ToString();
                //loginDB = "NIC";
                empNo = argsInfor[3].ToString();
                //empNo = "PD";
                empPass = argsInfor[4].ToString();
                //empPass = "PD";
                string fun = lblTitleFunc.Text.Trim();
                _sfcHttpClient = new SfcHttpClient(loginApiUri, loginDB, "helloApp", "123456");
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
                dal = new DAL(_sfcHttpClient);

                apname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
               // apname = Regex.Replace(apname, "[_]", string.Empty);
                _res = await dal.GetVersionMatch(apname);
                if (!_res.Contains("Program not exist"))
                {
                    ap_version_server = _res;
                }
                else
                {
                    MessageBox.Show(_res, "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                    Environment.Exit(0);
                }
                string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var ver_local = new Version(ver);
                var ver_server = new Version(ap_version_server);
                var result = ver_local.CompareTo(ver_server);
                if (result < 0)
                {
                    MessageBox.Show("Application is running have new version on AMS-SERVER is: " + ap_version_server + " ,Please use SFC_AMS program download new version!", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    Environment.Exit(0);
                }
                localVer = ver_local.ToString().Trim();
                apver = apname + "|" + localVer;              
                string FilePath = System.AppDomain.CurrentDomain.BaseDirectory;              
                #region Check Login , get link api sfc         


                    IPHostEntry IPHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                    foreach (var ipAddress in IPHost.AddressList)
                    {
                        ip = ipAddress.ToString();
                    }
                    foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        // Only consider Ethernet network interfaces
                        if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                            nic.OperationalStatus == OperationalStatus.Up)
                        {
                            mac =  nic.GetPhysicalAddress().ToString();
                        }
                    }
                    if (!"PD".Equals(empNo))
                    {
                        txtEmpNo.Text = empNo;
                    }
                    else
                    {
                        txtEmpNo.Text = "";
                    }
                    openWD.Title = apname + " (Version: " + ver_local + " )";
                    lblTitle.Text = apname + " (Version: " + ver_local + " )";
                     Status.Text = "Employee: " + empNo + "       |       " + "IP: " + ip + "       |       " + "MAC: "+mac;
                    openWD.Show();
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                Environment.Exit(0);
            }

        }
    }
}
