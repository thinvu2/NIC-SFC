using LabelManager2;
using Newtonsoft.Json;
using PACK_CTN.Models;
using PACK_CTN.Resource;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using Label_Tracking;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing.Printing;
using Sfc.Library.HttpClient.Helpers;
using System.Windows.Controls;
using Image = System.Windows.Controls.Image;
using System.Windows.Threading;
using System.IO.Ports;
using System.Windows.Documents;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string MCARTON_NO, CartonNo, M_sUPCEANData, _TxtQtyLable, Z_LABEL, MODEL_NAME, MO_NUMBER, VERSION_CODE, LabelName, MODEL_TYPE,
            sCustModel_Name, sCustModel_Desc1, sCustModel_Desc2, sCustModel_Desc3, sCustModel_Desc4, sCustModel_Desc5, sCustomer_Name, CustModelDesc
            , ITEMS_PRINT, MACIP , IP;
        public static string strINIPath = "C:\\PACKING\\PACKING.INI";
        public static bool IsUpdateR105, CheckKp, itemOrderbySSN, RB_Carton, RprintZ_WIP, _ChkMD5Flag = true;
        public static bool csvt_not_full = false, _CheckLabel, _itemReprint, IsPass , CheckLogin, CheckPri;
        public static int KPSNLength, SSNLength, KPQTY, iCarton_LabelQty, itemLable, itemScanInput , lbIndex;
        public static string FilePath, FrmSender, UrlLabelFile , printerName ;
        public static DataTable dtParams = new DataTable();
        public static DataTable dtItemsLabel = new DataTable();
        public static string loginDB, empNo, empPass, EMP_NAME, inputLogin;
        private string checkSum, erromessage, strsql, StrbMessage, ITEM_SETUP_CLICK, My_PrintMethod;
        public string[] _RESArray = { "NULL" };
        private bool isPaper, IsPrintCartonLabel ;
        public string ModelSerial, PassInput , Appver , wipLabelMd5 ,LocalMd5 , strParamName = "NG" , EMP_PRG,IsPallet="0" ;
        public static Double CartonQTY , CTN_QTY_CONFIG , TARGET_QTY_MODEL , ScreensizeM;
        public int PHAN_NGUYEN, PHAN_DU ;
        float  currentWeight;
        string displayWeightStr = "0" , DataWeight;
        public  LabelManager2.Application LabApp;
        Document doc = null;



        public static Oracle oracle = null;

        SerialPort SP = new SerialPort();
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        FlowDocument mcFlowDoc = new FlowDocument();
        Paragraph para = new Paragraph();
        public string output = "";

        private string prgname = "PACK_CTN";
        private GridLength _gridImage;



        // public string Password { get { return _Password; } set { _Password = value; } }
        public ICommand PasswordChangedCommand { get; set; }
        public object ApplicationDeployment { get; private set; }


        public static SfcHttpClient _sfcHttpClient;
        private string baseUrl = "http://10.224.81.51/sfcwebapi";

        MessageError FrmMessage = new MessageError();


        public MainWindow()
        {
            InitializeComponent();
            PACKING();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            GetWeight();
            itemWeight.IsChecked = false;

        }
        public void PACKING()
        {
            string program = AppDomain.CurrentDomain.FriendlyName;
            int index = program.IndexOf(".exe");
            if (index != -1)
            {
                program = program.Remove(index, 4);
            }

            Process[] processes = Process.GetProcesses();
            List<dynamic> LIST = new List<dynamic>();

            foreach (Process process in processes)
            {
                LIST.Add(process.ProcessName);
                if (process.ProcessName.ToUpper().Contains("PACK"))
                {
                    if (process.ProcessName != program)
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "Chương trình " + process.ProcessName + " đang bật!";
                        FrmMessage.MessageEnglish = "This Program " + process.ProcessName + " in operation! ";
                        FrmMessage.ShowDialog();
                        Environment.Exit(0);
                    }
                }
            }
        }
        void size()
        {
            double Screensize, controlsize, imageWidth;
            Screensize = (SystemParameters.PrimaryScreenWidth);
       
            if (Screensize > 1200)
            {
                imageWidth = (Screensize / 6);
                if (Screensize - imageWidth < 1200)
                {
                    imageWidth = Screensize - 1200;
                }
            }
            else
            {
                imageWidth = 0;
            }
            controlsize = (Screensize / 100) * 1.35;

            System.Windows.Application.Current.Resources.Remove("FontSizeVal");
            System.Windows.Application.Current.Resources.Add("FontSizeVal", controlsize);

            System.Windows.Application.Current.Resources.Add("widthImage", imageWidth);
        }

       
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Appver = getRunningVersion().ToString();
                mainWindow.Title = "PACK_CTN - Version:" + Appver;
                // InitializeComponent();

                // 
                size();
                // 

                if (! await AccessAPI())
                {
                    return;
                }
                if (empNo == "PD")
                {
                    LoginWindow frmLogin = new LoginWindow();
                    frmLogin.ShowDialog();
                    if (!CheckLogin)
                    {
                        this.Close();
                    }
                    lblEmpName.Content = EMP_NAME;
                    itemNameEmp.Content = "Hi: " + EMP_NAME;


                    _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                    await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                    oracle = new Oracle(_sfcHttpClient);
                    INI Infor = await oracle.GetObj<INI>(" SELECT * FROM SFIS1.C_PARAMETER_INI   WHERE  PRG_NAME  ='PACK_CTN' AND VR_CLASS ='INFOR' ");
                    if (Infor != null)
                    {
                        lblPhoneNumber.Content = Infor.VR_ITEM;
                        lblMailInfor.Content = Infor.VR_NAME;
                    }
                    txtInputSN.Focus();
                }
                else
                {
                    Login();
                }

                MACIP = GetMacAddress();
                IP = GetIPAddress().ToString();
                lblMacAddress.Content = MACIP;
                itemVerPrgram.Content = "Ver: " + Appver;
                var bc = new BrushConverter();
                loadIniFile();

                while (tbLineName.Text == "")
                {
                    SetupStation frmStation = new SetupStation();
                    frmStation.ShowDialog();
                    tbLineName.Text = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK2", "LINE", "");
                }

                try
                {
                    LabApp = new LabelManager2.Application();
                }
                catch (Exception ex)
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Connect Codesosft error: " + ex.ToString();
                    FrmMessage.MessageEnglish = ex.Message;
                    FrmMessage.ShowDialog();
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Loaded program have Exception ";
                FrmMessage.MessageEnglish = ex.Message;
                FrmMessage.ShowDialog();
                return ;
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
        private async Task<bool> CheckAppVersion(string AppName, string AppVersion)
        {

            VersionModel versionModel = await oracle.GetObj<VersionModel>($"SELECT ap_version FROM SFIS1.C_AMS_PATTERN_T WHERE AP_NAME= '{AppName}' ");            
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
            return true;
        }

        private async Task<Boolean> AccessAPI()
        {
            try
            {
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
                baseUrl = argsInfor[1].ToString();
                loginDB = argsInfor[2].ToString();
                empNo = argsInfor[3].ToString();
                empPass = argsInfor[4].ToString();

              //  baseUrl = "http://10.220.130.99/sfcwebapi";


                itemDbConnected.Content = "DB: " + loginDB.ToUpper();
                //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
                //{
                //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                //    Environment.Exit(0);
                //}

                _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                oracle = new Oracle(_sfcHttpClient);

                if (!await CheckAppVersion(prgname, Appver))
                {
                    Environment.Exit(0);
                }


               if ((tbLineName.Text == null) || (tbLineName.Text == ""))
                {
                    SetupStation SetStation = new SetupStation();
                    SetStation.ShowDialog();
                    tbLineName.Text = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK2", "LINE", "");
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "AccessAPI have Exception ";
                FrmMessage.MessageEnglish = ex.Message;
                FrmMessage.ShowDialog();
                return false;
            }
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

        public async void Login()
        {
            try
            {
                _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                oracle = new Oracle(_sfcHttpClient);
                EmpModel emp = await oracle.GetObj<EmpModel>($"select * from SFIS1.c_emp_desc_t where emp_pass ='{empPass}'");
                if (emp != null)
                {
                    lblEmpName.Content = emp.emp_name;
                    itemNameEmp.Content = "Hi: " + emp.emp_name;
                    txtInputSN.Focus();
                }

                INI Infor = await oracle.GetObj<INI>(" SELECT * FROM SFIS1.C_PARAMETER_INI   WHERE  PRG_NAME  ='PACK_CTN' AND VR_CLASS ='INFOR' ");
                if (Infor != null)
                {
                    lblPhoneNumber.Content = Infor.VR_ITEM;
                    lblMailInfor.Content = Infor.VR_NAME;
                }

                Console.WriteLine("Finished!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }



        #region  CLICK SETUP
        private void StationName_Click(object sender, RoutedEventArgs e)
        {
            SetupStation StationSetup = new SetupStation();
            StationSetup.ShowDialog();
            tbLineName.Text = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK2", "LINE", "");
        }

        private void Flipper_OnIsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (e.NewValue == true)
            {
                Grid_menu.Width = 140;
            }
            else
            {
                Grid_menu.Width = 0;
            }

        }

        private void MenuItem_MouseMove(object sender, MouseEventArgs e)
        {

            itemCodesoft.Background = Brushes.OrangeRed;
        }

        private void ShowParam_MouseMove(object sender, MouseEventArgs e)
        {

            ShowParam.Background = Brushes.OrangeRed;
        }

        private void ShowParam_MouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            ShowParam.Background = (Brush)bc.ConvertFrom("#0889a6");
        }
        private void ItemCodesoft_MouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            itemCodesoft.Background = (Brush)bc.ConvertFrom("#0889a6");
        }
        private void Print_MouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            Print.Background = (Brush)bc.ConvertFrom("#0889a6");
        }

        private void Pass_MouseMove(object sender, MouseEventArgs e)
        {
            Pass.Background = Brushes.OrangeRed;
        }

        private void Pass_MouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            Pass.Background = (Brush)bc.ConvertFrom("#0889a6");
        }

        private void Option_MouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            Option.Background = (Brush)bc.ConvertFrom("#0889a6");
        }

        private void Option_MouseMove(object sender, MouseEventArgs e)
        {
            Option.Background = Brushes.OrangeRed;
        }

        private void Setup_MouseLeave(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            Setup.Background = (Brush)bc.ConvertFrom("#0889a6");
        }

        private void Setup_MouseMove(object sender, MouseEventArgs e)
        {
            Setup.Background = Brushes.OrangeRed;
        }

        private void ItemCodesoft_MouseMove(object sender, MouseEventArgs e)
        {
            itemCodesoft.Background = Brushes.OrangeRed;
        }


        private void Print_MouseMove(object sender, MouseEventArgs e)
        {
            Print.Background = Brushes.OrangeRed;
        }

        private async void Reprint_Click(object sender, RoutedEventArgs e)
        {
            if (empNo == "")
            {
                FrmMessage.errorcode = "00086";
                FrmMessage.CustomFlag = false;
                FrmMessage.ShowDialog();
                return;
            }

            if (itemScanMac2.IsChecked)
            {
                ITEMS_PRINT = "1";
            }
            else
            {
                ITEMS_PRINT = "0";
            }

            if (itemLHcartonMac.IsChecked)
            {
                ITEMS_PRINT = ITEMS_PRINT + "1";
            }
            else
            {
                ITEMS_PRINT = ITEMS_PRINT + "0";
            }

            if (itemScanTrayNo.IsChecked)
            {
                ITEMS_PRINT = ITEMS_PRINT + "1";
            }
            else
            {
                ITEMS_PRINT = ITEMS_PRINT + "0";
            }

            if (await CheckParamaterINI("YES", "REPRINT_LABEL"))
            {
                PassInput = "";
                InputBox inputBOX = new InputBox();
                inputBOX.ShowDialog();
                inputBOX.Activate();
                if (inputBOX.PASS != "")

                {
                    if (await CheckPrivilege(inputBOX.PASS, "PACK_TRAY", "REPRINT"))
                    {
                        itemReprint.IsChecked = true;
                        _itemReprint = true;
                        Reprint FrmReprint = new Reprint();
                        FrmReprint.EMP_NO = EMP_PRG;
                        FrmReprint.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Check Validation Failed, No Authority!", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                    }
                }
                else
                {
                    MessageBox.Show("Please input password !", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                }
            }
            else
            {
                if (loginDB == "ROKU")
                {
                    itemReprint.IsChecked = true;
                    _itemReprint = true;
                    Reprint FrmReprint = new Reprint();
                    FrmReprint.ShowDialog();
                }
            }
            LabApp = null;
            _itemReprint = false;
            itemReprint.IsChecked = false;
            txtInputSN.SelectAll();
            txtInputSN.Focus();
        }

        public async Task<bool> CheckPrivilege(string EMPBC, string FUN, string PRG_NAME)
        {
            string SQL = "SELECT * FROM SFIS1.C_PRIVILEGE WHERE  FUN= '" + FUN + "' " +
                            "AND PRG_NAME = '" + PRG_NAME + "' AND EMP = (SELECT EMP_NO FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC = '" + EMPBC + "') AND ROWNUM = 1";

            var _PRIVILEGE = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = SQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (_PRIVILEGE.Data != null)
            {
                dynamic _ads = _PRIVILEGE.Data;

                if (_ads["privilege"] == 2)
                {
                    EMP_PRG = _ads["emp"];
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

        public async Task<bool> CheckParamaterINI(string value1, string value2)
        {
            var _paramaterINI = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE vr_value =:VL1 AND prg_name=:VL2  ",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter{ Name = "VL1", Value = value1 },
                    new SfcParameter{ Name = "VL2", Value = value2 }
                }
            });
            if (_paramaterINI.Data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void ButtonPopUp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void mainWindow_Keydown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F1:
                    StationName_Click(sender, e);
                    break;
                case Key.F9:
                    Reprint_Click(sender, e);
                    break;
                case Key.F8:
                    lbQTY_Click(sender, e);
                    break;
            }
        }

        private void PasswordBox_KeyUp(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Enter)
            //{
            //    Login();
            //}
        }

        private void SetupPrinter_Click(object sender, RoutedEventArgs e)
        {
            if (itemSetupPrinter.IsChecked == true)
            {
                SetupPrinter setupPrinter = new SetupPrinter();
                setupPrinter.ShowDialog();
                printerName = MES.OpINI.IniUtil.ReadINI(strINIPath, "PRINTER", "PrinterName", "");
            }
        }

        private void LblModelName_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MessageBox.Show("loisdfabsdfsdhgh");
        }

        private void ScanMac2_click(object sender, RoutedEventArgs e)
        {
            itemScanReset();
            itemScanMac2.IsChecked = true;
            lbSN.Content = "MAC2";
            lblistSNitem1.Content = "SN";
            lblistSNitem2.Content = "MAC2";
            lbError.Content = "Please input MAC2 !";
            var bc = new BrushConverter();
            lbError.Foreground = (Brush)bc.ConvertFrom("#0889a6");
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "Scan_Input", "4");

        }


        private void ScanMac_click(object sender, RoutedEventArgs e)
        {
            itemScanReset();
            itemScanMac.IsChecked = true;
            lbSN.Content = "MAC";
            lbSSN.Content = "SSN";
            lblistSNitem1.Content = "SN";
            lblistSNitem2.Content = "MAC";
            lbError.Content = "Please input MAC !";
            var bc = new BrushConverter();
            lbError.Foreground = (Brush)bc.ConvertFrom("#0889a6");
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "Scan_Input", "5");
          
        }
        void itemScanReset ()
        {
            itemScanSN.IsChecked = false;
            itemScanTrayNo.IsChecked = false;
            itemScanShippingSN.IsChecked = false;
            itemScanMac2.IsChecked = false;
            itemScanMac.IsChecked = false;
        }

        private void checkMaid_Click(object sender, RoutedEventArgs e)
        {
            itemCheckMac.IsChecked = true;
        }

        private void ItemTest_Click(object sender, RoutedEventArgs e)
        {
            // input model name
            if (lblModelName.Content == null)
            {
                MODEL_NAME = "";
            }
            // input carton no
            CartonNo = "";

            CheckCartonLabel frmCheckCartonlabel = new CheckCartonLabel();
            frmCheckCartonlabel.lblCartonNo.Content = CartonNo;
            frmCheckCartonlabel.MODELNAME = MODEL_NAME ;
            frmCheckCartonlabel.LineName = tbLineName.Text;
            frmCheckCartonlabel.EmpNo = empNo;
            frmCheckCartonlabel.ShowDialog();
            txtInputSN.SelectAll();
            txtInputSN.Focus();
        }

        private void ItemPrintCartonLabel_Click(object sender, RoutedEventArgs e)
        {
            itemPrintCartonLabel.IsChecked = true;
        }

        private void ItemCheckRoute_Click(object sender, RoutedEventArgs e)
        {
            itemCheckRoute.IsChecked = true;
        }

        private void itemClose_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private async void itemPass_Click(object sender, RoutedEventArgs e)
        {
            InputBox inputBOX = new InputBox();
            inputBOX.tbMess.Text = "Input password for check pass label !!";
            inputBOX.ShowDialog();
            if (await CheckPrivilege(inputBOX.PASS, "CHECK_PASS", "CHECK_LABEL"))
            {
      
                String ERR = await GetPubMessage("00159");
                MessageBoxResult dlr = MessageBox.Show(ERR, "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (dlr == MessageBoxResult.OK)
                {
                  
                }
            }
        }

        private async void fqaPassLabel ()
        {
            if ( (LabelName.Length == 0 )|| (tbLineName.Text.Length == 0) )
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = false;
                FrmMessage.errorcode = "00160";
                FrmMessage.ShowDialog();
                return;
            }
            try
            {
                var Result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " SELECT * FROM SFISM4.R_FQA_CHECKLABEL_T  WHERE UPD_DATE= (SELECT   MAX(UPD_DATE)  " +
                                        " FROM SFISM4.R_FQA_CHECKLABEL_T    WHERE LABEL_NAME = '"+LabelName+ "' AND GROUP_NAME= '" + tbStationName + "' ) " +
                                        " AND LABEL_NAME = '" + LabelName + "'  AND  GROUP_NAME= '"+tbStationName+"' ",
                    SfcCommandType = SfcCommandType.Text 
                });
                if (Result.Data != null)
                {

                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = ex.Message.ToString();
                FrmMessage.MessageEnglish = "Have exceptions:";
                FrmMessage.ShowDialog();
                return;
            }
        }

        private void ScanTrayNo_click(object sender, RoutedEventArgs e)
        {
            itemScanReset();
            itemScanTrayNo.IsChecked = true;
            lblistSNitem1.Content = "TRAY NO";
            lblistSNitem2.Content = "QTY";

            lbSN.Content = "TRAY NO";
            lbError.Content = "Please input TRAY NO !";
            var bc = new BrushConverter();
            lbError.Foreground = (Brush)bc.ConvertFrom("#0889a6");  //0889a6
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "Scan_Input", "3");
         
        }

        private void lbQTY_Click(object sender, RoutedEventArgs e)
        {
            SetLabelQTY _lbqty = new SetLabelQTY();
            _lbqty.ShowDialog();
        }

        private void ItemVisible_Click(object sender, RoutedEventArgs e)
        {
            CheckPri = false;
            CheckPrivilege chkpri = new CheckPrivilege();
            chkpri.ShowDialog();
            if (CheckPri)
            {
                VisibleLabel();
            }
            
        }

        private  void mainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                killprocess();
                string[] listlink = Directory.GetFiles(System.IO.Directory.GetCurrentDirectory(), "*.lab");
                foreach (string link in listlink)
                {
                    File.Delete(link);
                }
            }
            catch(Exception ex)
            {
                
            }
        }

        private void Item_LH_Carton_Click(object sender, RoutedEventArgs e)
        {
            SetItemsLable();
            item_LH_Carton.IsChecked = true;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LB_Carton", "1");
        }

        private void ItemLHcartonMac_Click(object sender, RoutedEventArgs e)
        {
            SetItemsLable();
            itemLHcartonMac.IsChecked = true;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LB_Carton", "2");
        }

        private void ItemGLcarton_Click(object sender, RoutedEventArgs e)
        {

            SetItemsLable();
            itemGLcarton.IsChecked = true;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LB_Carton", "3");
         
        }

        private void ItemGLcartonMac_Click(object sender, RoutedEventArgs e)
        {
            SetItemsLable();
            itemGLcartonMac.IsChecked = true;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LB_Carton", "4");
        }
        private void SetItemsLable ()
        {
            item_LH_Carton.IsChecked = false;
            itemLHcartonMac.IsChecked = false;
            itemGLcarton.IsChecked = false;
            itemLocalCarton.IsChecked = false;
            itemGLcartonMac.IsChecked = false;
        }

        private void CheckCTN_Click(object sender, RoutedEventArgs e)
        {
            CheckCarton CheckCTN = new CheckCarton();
            CheckCTN.ShowDialog();
        }

        private void EmpName_TouchEnter(object sender, TouchEventArgs e)
        {

        }

        private void btnCheckLabel_Click(object sender, RoutedEventArgs e)
        {
            _CheckLabel = false;
            CheckLabel _CheckLB = new CheckLabel();
            _CheckLB.MoNumber = lblMoNumber.Content.ToString();
            _CheckLB.LineName = tbLineName.Text.ToString();
            _CheckLB.ShowDialog();
            if (_CheckLB._CheckLabel == true)
            {
                gridCheckLabel.Visibility = Visibility.Hidden;
            }

        }

        private async void itemCloseCarton_Click(object sender, RoutedEventArgs e)
        {
            string SQL = "SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP = '" + empNo + "' AND FUN= 'CLOSE_CARTON'";
            var _PRIVILEGE = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = SQL,
                SfcCommandType = SfcCommandType.Text
            });

            if (_PRIVILEGE.Data == null)
            {
                MessageBox.Show("EMP NO PRIVILEGE CLOSE CARTON!", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                return;
            }

            CloseCarton frmCloseCarton = new CloseCarton();
            if (lblCount.Content == null)
            {
                frmCloseCarton.MainLbCartonCount = "";
            }
            else
            {
                frmCloseCarton.MainLbCartonCount = lblCount.Content.ToString();
            }
            frmCloseCarton.ShowDialog();
        }

        private void ItemLocalCarton_Click(object sender, RoutedEventArgs e)
        {
            SetItemsLable();
            itemLocalCarton.IsChecked = true;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LB_Carton", "5");
        }


        private async void txtInputshipping_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                dtParams.Clear();  // Xoa toan bo bien cu
                getItemSetup();

                _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                if (txtInputSN.Text == "UNDO")
                {
                    LabApp = null;
                    Refresh();
                    lbError.Content = txtInputSN.Text + "  OK";
                    txtInputSN.Text = "";

                }
                else
                {
                    var logInfo = new
                    {
                        OPTION = "SSN_EXECUTE" ,
                        DATA = txtInputSN.Text,
                        SSN = txtInputshipping.Text,
                        LINE_NAME = tbLineName.Text,
                        LABEL_MO = lblMoNumber.Content,
                        EMP = empNo,
                        LABEL_MODEL = lblModelName.Content,
                        LABEL_VERSION = lblVersionCode.Content,
                        LABEL_GROUP = tbStationName.Text,
                        LABEL_CARTON = lblCartonNo.Content,
                        COUNT_ITEM_SN = "0",
                        ITEM_SETUP = ITEM_SETUP_CLICK,
                        IDMAC = lblMacAddress.Content,
                        MODELSERIAL = ModelSerial
                    };
                    //Tranform it to Json object

                    string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
                    try
                    {
                        var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                        });
                        dynamic ads = result.Data;
                        string _RES = ads[0]["output"];
                        _RESArray = _RES.Split('#');

                        if (_RESArray[0] == "OK")
                        {
                            lblCartonNo.Content = _RESArray[1];
                            lblMCartonNo.Content = _RESArray[2];
                            MCARTON_NO = _RESArray[2];
                            lblCount.Content = _RESArray[3];
                            lblModelName.Content = _RESArray[6];
                            lblVersionCode.Content = _RESArray[8];
                            lblMoNumber.Content = _RESArray[7];
                            lblCartonQTY.Content = _RESArray[9];
                            lblMoQTY.Content = _RESArray[10];
                            lblPackQTY.Content = _RESArray[11];
                            MODEL_TYPE = _RESArray[12];

                            if (! await getSNinCarton())
                            {
                                return;
                            }

                            btnCloseCarton.IsEnabled = true;

                            txtInputSN.SelectAll();
                            txtInputSN.Focus();
                            txtInputshipping.Text = "";

                            if (lblCount.Content == lblCartonQTY.Content && !itemWeight.IsChecked)
                            {
                                //btnCloseCarton_Click(sender, e);
                                //check Bracket
                                strsql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE VR_CLASS='ISBRACKET' AND PRG_NAME ='PACK_CTN' AND VR_VALUE ='" + lblModelName.Content + "'";
                                var result1 = await _sfcHttpClient.QuerySingleAsync (new QuerySingleParameterModel
                                {
                                    CommandText = strsql,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (result1.Data != null)
                                {
                                    IsBracket frmIsBracket = new IsBracket();
                                    frmIsBracket.KP = "BRACKET";
                                    frmIsBracket.RES = "NA";
                                    frmIsBracket.lblCartonNo = lblMCartonNo;
                                    frmIsBracket.ShowDialog();
                                    if (frmIsBracket.RES != "OK")
                                        return;
                                }
                                //end
                                //link RFID_label
                                var logInfo1 = new
                                {
                                    OPTION = "RFID_CONFIG",
                                    DATA = MCARTON_NO
                                };
                                string jsonData1 = JsonConvert.SerializeObject(logInfo1).ToString();

                                var result2 = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                                {
                                    CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                                    SfcCommandType = SfcCommandType.StoredProcedure,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                    new SfcParameter{Name="DATA",Value=jsonData1,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                    }
                                });
                                dynamic ads1 = result2.Data;
                                string _RES1 = ads1[0]["output"];
                                _RESArray = _RES1.Split('#');
                                if (_RESArray[0] == "OK")
                                {
                                    if (_RESArray[1] == "Y")
                                    {
                                        RfidForm rfidform = new RfidForm();
                                        rfidform.ShowDialog();

                                        //check
                                        strsql = "SELECT * FROM SFISM4.R_SEC_LIC_LINK_T WHERE CARTON_NO ='" + MCARTON_NO + "' AND RFID IS NOT NULL";
                                        result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strsql,
                                            SfcCommandType = SfcCommandType.Text
                                        });

                                        if (result1.Data == null)
                                        {
                                            MessageError FrmMessage = new MessageError();
                                            FrmMessage.CustomFlag = true;
                                            FrmMessage.MessageEnglish = string.Format("Link RFID is null ");
                                            FrmMessage.MessageVietNam = string.Format("Link RFID rỗng ");
                                            FrmMessage.ShowDialog();
                                            return;
                                        }
                                    }
                                }
                                else if (_RESArray[0] == "NG")
                                {
                                    MessageError FrmMessage = new MessageError();
                                    erromessage = _RESArray[1];
                                    FrmMessage.errorcode = _RESArray[1];
                                    FrmMessage.CustomFlag = true;
                                    FrmMessage.MessageEnglish = string.Format(erromessage);
                                    FrmMessage.MessageVietNam = " SFIS1.PACK_CTN_API_EXECUTE / RFID_CONFIG ";
                                    txtInputshipping.Focus();
                                    txtInputshipping.SelectAll();
                                    FrmMessage.ShowDialog();
                                }

                                await closeCarton();
                            }

                            InitializeComponent();
                        }
                        else if (_RESArray[0] == "CHECK_CARTON")
                        {
                            CheckCarton frmCheckCarton = new CheckCarton();

                        }
                        else
                        {
                            if (_RESArray[1] == "ER")
                            {
                                MessageError FrmMessage = new MessageError();

                                FrmMessage.errorcode = _RESArray[2];
                                FrmMessage.CustomFlag = false;
                                FrmMessage.ShowDialog();
                                txtInputshipping.Focus();
                                txtInputshipping.SelectAll();
                                return;
                            }
                            else if (_RESArray[1] == "MS")
                            {
                                MessageError FrmMessage = new MessageError();
                                erromessage = _RESArray[2];
                                FrmMessage.errorcode = _RESArray[2];
                                FrmMessage.CustomFlag = true;
                                FrmMessage.MessageEnglish = string.Format(erromessage);
                                FrmMessage.MessageVietNam = " SFIS1.PACK_CTN_API_EXECUTE / SSN_EXECUTE ";
                                txtInputshipping.Focus();
                                txtInputshipping.SelectAll();
                                FrmMessage.ShowDialog();
                                return;
                            }
                            else if (_RESArray[1] == "PASSLABEL")
                            {
                                String ERR = await GetPubMessage(_RESArray[2]);
                                gridCheckLabel.Visibility = Visibility.Visible;
                                txtCheckLabel.Text = ERR;

                                lblMoNumber.Content = _RESArray[3];
                                lblModelName.Content = _RESArray[4];
                                lblVersionCode.Content = _RESArray[5];
                                MessageError FrmMessage = new MessageError();
                                FrmMessage.errorcode = _RESArray[2];
                                FrmMessage.CustomFlag = false;
                                FrmMessage.ShowDialog();
                                txtInputshipping.Focus();
                                txtInputshipping.SelectAll();
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = ex.Message.ToString();
                        FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                        FrmMessage.ShowDialog();
                        txtInputshipping.Focus();
                        txtInputshipping.SelectAll();
                        return;
                    }
                }
            }
        }

        

        private void itemA3_Click(object sender, RoutedEventArgs e)
        {
            if (itemA3.IsChecked)
            {
                itemA4.IsChecked = false;
                itemA5.IsChecked = false;
            }
            if ((itemA3.IsChecked == false) && (itemA4.IsChecked == false) && (itemA5.IsChecked) == false)
            {
                isPaper = false;
            }
            else
            {
                isPaper = true;
            }
        }

        private void itemA4_Click(object sender, RoutedEventArgs e)
        {
            if (itemA4.IsChecked)
            {
                itemA3.IsChecked = false;
                itemA5.IsChecked = false;
            }
            if ((itemA3.IsChecked == false) && (itemA4.IsChecked == false) && (itemA5.IsChecked) == false)
            {
                isPaper = false;
            }
            else
            {
                isPaper = true;
            }
        }

        private void itemA5_Click(object sender, RoutedEventArgs e)
        {
            if (itemA5.IsChecked)
            {
                itemA4.IsChecked = false;
                itemA3.IsChecked = false;
            }
            if ((itemA3.IsChecked == false) && (itemA4.IsChecked == false) && (itemA5.IsChecked) == false)
            {
                isPaper = false;
            }
            else
            {
                isPaper = true;
            }
        }

        private void ScanShippingSN_click(object sender, RoutedEventArgs e)
        {
          
            itemScanReset();
            itemScanShippingSN.IsChecked = true;
            lblistSNitem1.Content = "SN";
            lblistSNitem2.Content = "SHIPPING SN";

            lbSN.Content = "SSN";
            lbSSN.Content = "SN";
            lbError.Content = "Please input shipping sn !";
            var bc = new BrushConverter();
            lbError.Foreground = (Brush)bc.ConvertFrom("#0889a6");
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "Scan_Input", "2");
          
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CSParam CS = new CSParam();
            CS.ShowDialog();
        }

        private void ScanSN_click(object sender, RoutedEventArgs e)
        {
            itemScanReset();
            itemScanSN.IsChecked = true;
            lblistSNitem1.Content = "SN";
            lblistSNitem2.Content = "SHIPPING SN";
            lbSN.Content = "SN";
            lbSSN.Content = "SSN";
            lbError.Content = " Please input serial_number !";
            var bc = new BrushConverter();
            lbError.Foreground = (Brush)bc.ConvertFrom("#0889a6");
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "Scan_Input", "1");
          

        }
        #endregion 
        private void InputMAC_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private async Task<bool>  WeigthInfor (string carton , string weight ,string FSender)
        {
            var logInfo = new
            {
                OPTION = "CheckWeight",
                CARTON = carton,
                WEIGHT = weight,
                SENDER = FSender,
                MODEL = MODEL_NAME,
                MO = MO_NUMBER 
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

            var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                }

            });

            if (result.Data != null)
            {
                dynamic ads = result.Data;
                string _RES = ads[0]["output"];
                _RESArray = _RES.Split('#');

                if (_RESArray[0] != "OK")
                {
                    if (_RESArray[1] == "MS")
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = _RESArray[2];
                        FrmMessage.MessageVietNam = "SFIS1.PACK_CTN_API_EXECUTE /funtion: CheckWeight ";
                        FrmMessage.ShowDialog();
                        return false;
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.errorcode = _RESArray[2];
                        FrmMessage.CustomFlag = false;
                        FrmMessage.ShowDialog();
                        txtInputSN.SelectAll();
                        txtInputSN.Focus();
                        return false;
                    }
                }
                else
                {
                    DataWeight = _RESArray[1].ToString();
                }
                return true;
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = "Execute Procedures result null"; 
                FrmMessage.MessageVietNam = "SFIS1.PACK_CTN_API_EXECUTE /funtion: CheckWeight ";
                FrmMessage.ShowDialog();
                return false;

            }
        }

        private void getItemSetup()
        {
            if (itemScanShippingSN.IsChecked)  // 1
            {
                ITEM_SETUP_CLICK = "1";
            }
            else
            {
                ITEM_SETUP_CLICK = "0";
            }

            if (itemScanSN.IsChecked)    // 2
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "1";
            }
            else
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "0";
            }

            if (itemPackByBox.IsChecked)   // 3
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "1";
            }
            else
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "0";
            }
            if (itemScanTrayNo.IsChecked)   // 4
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "1";
            }
            else
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "0";
            }
            if (itemScanMac2.IsChecked)   // 5
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "1";
            }
            else
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "0";
            }

            if (itemCheckLabel.IsChecked)   // 6
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "1";
            }
            else
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "0";
            }

            if (itemScanMac.IsChecked)  // 7
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "1";
            }
            else
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "0";
            }
            if (itemNoPackPalt.IsChecked)  // 8
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "1";
            }
            else
            {
                ITEM_SETUP_CLICK = ITEM_SETUP_CLICK + "0";
            }
        }
        public void Refresh ()
        {
            lblModelName.Content = "";
            lblMoNumber.Content = "";
            lblVersionCode.Content = "";
            lblCartonNo.Content = "";
            lblCartonQTY.Content = "";
            lblCount.Content = "";
            viewListSN.ItemsSource = null;
        }
        private async void txtInputSN_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtInputSN.Text != string.Empty)
            {
                if (itemScanTrayNo.IsChecked)   // 4
                {
                    //CHECK SN/SSN TRONG BOX/TRAY
                    strsql = "SELECT * FROM SFIS1.C_MODEL_DESC_T where MODEL_SERIAL = 'ECD' AND MODEL_NAME = (" +
                        "select model_name from sfism4.r107 where rownum = 1 and tray_no ='" + txtInputSN.Text + "')";
                    var res = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (res.Data != null)
                    {
                        IsCheckBox_Tray frmIsCheckBox_Tray = new IsCheckBox_Tray();
                        frmIsCheckBox_Tray.RES = "NA";
                        frmIsCheckBox_Tray.lblCartonNoSN_SSN.Content = txtInputSN.Text;
                        frmIsCheckBox_Tray.lblCartonNoSN_SSN1.Content = txtInputSN.Text;
                        frmIsCheckBox_Tray.ShowDialog();
                        if (frmIsCheckBox_Tray.RES != "OK")
                            return;
                    }
                    //else
                    //{
                    //    MessageError FrmMessage = new MessageError();
                    //    FrmMessage.CustomFlag = true;
                    //    FrmMessage.MessageEnglish = "TRAY_NO NOT FOUND!";
                    //    FrmMessage.MessageVietNam = "TRAY_NO KHÔNG TỒN TẠI!";
                    //    FrmMessage.ShowDialog();
                    //    txtInputSN.SelectAll();
                    //    txtInputSN.Focus();
                    //    return;
                    //}
                }

                if (itemAddA.IsChecked) txtInputSN.Text = "@" + txtInputSN.Text;
                if (itemQRMode.IsChecked)
                {
                    var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.SP_GET_QRSTRING",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="IN_GROUP",Value="PACK_CTN",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=txtInputSN.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                    });
                    dynamic ads = result.Data;
                    string _RES = ads[1]["res"];
                    if (_RES.StartsWith("OK")) txtInputSN.Text = ads[0]["out_data"];
                }
                dtParams.Clear();  // Xoa toan bo bien cu
                getItemSetup();
                if (itemUpper.IsChecked == true)
                {
                    txtInputSN.Text = txtInputSN.Text.ToUpper();
                }

                _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                if (txtInputSN.Text == "UNDO")
                {
                    Refresh();
                    LabApp = null;
                    lbError.Content = txtInputSN.Text + "  OK";
                    txtInputSN.Text = "";
                    txtInputSN.Focus();
                }
                else
                {
                    //Check KP 
                    strsql = "select TYPE_VALUE from SFIS1.C_MODEL_ATTR_CONFIG_T where ATTRIBUTE_NAME='PACKCTN_CONFIRM_KP' and TYPE_VALUE in (select model_name from sfism4.r107 where serial_number='" + txtInputSN.Text+ "' or Shipping_sn='" + txtInputSN.Text + "')";
                    var result0 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (result0.Data != null)
                    {
                        IsBracket frmIsBracket = new IsBracket();
                        frmIsBracket.KP = "ESD";
                        frmIsBracket.RES = "NA";
                        frmIsBracket.lblCartonNo.Content = result0.Data["type_value"];
                        frmIsBracket.ShowDialog();
                        if (frmIsBracket.RES != "OK")
                            return;
                    }

                    //Check DES
                    strsql = "select TYPE_VALUE,VERSION from SFIS1.C_MODEL_ATTR_CONFIG_T " +
                        "where ATTRIBUTE_NAME='PACKCTN_CONFIRM_DES'" +
                        " and TYPE_VALUE in (select model_name from sfism4.r107 " +
                        "where serial_number='" + txtInputSN.Text + "' or Shipping_sn='" + txtInputSN.Text + "')" +
                        " and (VERSION in (select VERSION_CODE from sfism4.r107 where serial_number='" + txtInputSN.Text + "' or Shipping_sn='" + txtInputSN.Text + "' ) OR VERSION = 'ALL') ";
                    var SQL = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (SQL.Data != null)
                    {
                        IsBracket frmIsBracket = new IsBracket();
                        frmIsBracket.KP = "DES";
                        frmIsBracket.RES = "NA";
                        frmIsBracket.lblCartonNo.Content = SQL.Data["type_value"];
                        IsBracket.version = SQL.Data["version"]?.ToString() ?? "";
                        frmIsBracket.ShowDialog();
                        if (frmIsBracket.RES != "OK")
                            return;
                    }

                    if (itemScanSSN.IsChecked==true && itemScanSN.IsChecked==true)
                    {
                        strsql = "select * from sfism4.r107 where serial_number = '" + txtInputSN.Text + "'";
                        result0 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strsql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (result0.Data != null)
                        {
                            txtInputshipping.Focus();
                            return;
                        }
                    }

                    //nputSN_void(InputSN.Text); 
                    var logInfo = new
                    {
                        OPTION = "SN_EXECUTE",
                        DATA = txtInputSN.Text,
                        LINE_NAME = tbLineName.Text,
                        LABEL_MO = lblMoNumber.Content?.ToString() ?? "",
                        EMP = empNo,
                        LABEL_MODEL = lblModelName.Content?.ToString() ?? "",
                        LABEL_VERSION = lblVersionCode.Content?.ToString() ?? "",
                        LABEL_GROUP = tbStationName.Text,
                        LABEL_CARTON = lblCartonNo.Content?.ToString() ?? "",
                        COUNT_ITEM_SN = "0",
                        ITEM_SETUP = ITEM_SETUP_CLICK,
                        IDMAC = lblMacAddress.Content?.ToString().Replace(":","") ?? ""
                    };
                    //Tranform it to Json object

                    string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
                    try
                    {
                        var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                        });
                        dynamic ads = result.Data;
                        string _RES = ads[0]["output"];
                        _RESArray = _RES.Split('#');

                        if (_RESArray[0] == "OK")
                        {
                            lblCartonNo.Content = _RESArray[1];
                            lblMCartonNo.Content = _RESArray[2];
                            MCARTON_NO = _RESArray[2];
                            lblCount.Content = _RESArray[3];
                            if (itemScanSN.IsChecked)
                            {
                                txtInputSN.Text = _RESArray[4];
                                txtInputshipping.Text = _RESArray[5];
                            }
                            else if (itemScanShippingSN.IsChecked)
                            {
                                txtInputSN.Text = _RESArray[5];
                                txtInputshipping.Text = _RESArray[4];
                            }
                            else if (itemScanMac.IsChecked)
                            {
                                txtInputshipping.Text = _RESArray[5]; 
                            }
                         
                            lblModelName.Content = _RESArray[6];
                            lblVersionCode.Content = _RESArray[8];
                            lblMoNumber.Content = _RESArray[7];
                            lblCartonQTY.Content = _RESArray[9];
                            lblMoQTY.Content = _RESArray[10];
                            lblPackQTY.Content = _RESArray[11];
                            MODEL_TYPE = _RESArray[12];
                            ModelSerial = _RESArray[13];

                            if (!await getSNinCarton())
                            {
                                return;
                            }

                            btnCloseCarton.IsEnabled = true;
                            txtInputSN.SelectAll();
                            txtInputSN.Focus();

                            if (lblCount.Content.ToString() == lblCartonQTY.Content.ToString() && !itemWeight.IsChecked)
                            {
                                //btnCloseCarton_Click(sender, e);
                                //check Bracket
                                strsql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE VR_CLASS='ISBRACKET' AND PRG_NAME ='PACK_CTN' AND VR_VALUE ='" + lblModelName.Content + "'";
                                var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strsql,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (result1.Data != null)
                                {
                                    IsBracket frmIsBracket = new IsBracket();
                                    frmIsBracket.KP = "BRACKET";
                                    frmIsBracket.RES = "NA";
                                    frmIsBracket.lblCartonNo = lblMCartonNo;
                                    frmIsBracket.ShowDialog();
                                    if (frmIsBracket.RES != "OK")
                                        return;
                                }
                                //end
                                //link RFID_label
                                var logInfo1 = new
                                {
                                    OPTION = "RFID_CONFIG",
                                    DATA = MCARTON_NO
                                };
                                string jsonData1 = JsonConvert.SerializeObject(logInfo1).ToString();

                                var result2 = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                                {
                                    CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                                    SfcCommandType = SfcCommandType.StoredProcedure,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                    new SfcParameter{Name="DATA",Value=jsonData1,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                    }
                                });
                                dynamic ads1 = result2.Data;
                                string _RES1 = ads1[0]["output"];
                                _RESArray = _RES1.Split('#');
                                if (_RESArray[0] == "OK")
                                {
                                    if (_RESArray[1] == "Y")
                                    {
                                        RfidForm rfidform = new RfidForm();
                                        rfidform.ShowDialog();

                                        //check
                                        strsql = "SELECT * FROM SFISM4.R_SEC_LIC_LINK_T WHERE CARTON_NO ='" + MCARTON_NO + "' AND RFID IS NOT NULL";
                                        result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strsql,
                                            SfcCommandType = SfcCommandType.Text
                                        });

                                        if (result1.Data == null)
                                        {
                                            MessageError FrmMessage = new MessageError();
                                            FrmMessage.CustomFlag = true;
                                            FrmMessage.MessageEnglish = string.Format("Link RFID is null ");
                                            FrmMessage.MessageVietNam = string.Format("Link RFID rỗng ");
                                            FrmMessage.ShowDialog();
                                            return;
                                        }
                                    }
                                }
                                else if (_RESArray[0] == "NG")
                                {
                                    MessageError FrmMessage = new MessageError();
                                    erromessage = _RESArray[1];
                                    FrmMessage.errorcode = _RESArray[1];
                                    FrmMessage.CustomFlag = true;
                                    FrmMessage.MessageEnglish = string.Format(erromessage);
                                    FrmMessage.MessageVietNam = " SFIS1.PACK_CTN_API_EXECUTE / RFID_CONFIG ";
                                    txtInputshipping.Focus();
                                    txtInputshipping.SelectAll();
                                    FrmMessage.ShowDialog();
                                }
                                //dong du carton
                                await closeCarton();
                            }

                            InitializeComponent();
                            //List<ListSN> items = new List<ListSN>();
                            //items.Add(new ListSN() { SN = _RESArray[4], SHIPPING_SN = _RESArray[5] });
                            //lvListSN.ItemsSource = items;
                        }
                        else if (_RESArray[0] == "CHECK_CARTON")
                        {

                            CheckCartonLabel frmCheckCartonlabel = new CheckCartonLabel();
                            frmCheckCartonlabel.lblCartonNo.Content = _RESArray[1];
                            frmCheckCartonlabel.MODELNAME = _RESArray[2];
                            frmCheckCartonlabel.LineName = tbLineName.Text;
                            frmCheckCartonlabel.EmpNo = empNo;
                            frmCheckCartonlabel.ShowDialog();
                            txtInputSN.SelectAll();
                            txtInputSN.Focus();
                            return;
                        }
                        else if (_RESArray[0] == "WAIT")
                        {
                            txtInputshipping.Focus();
                            lbError.Content = _RESArray[1];
                            lblModelName.Content = _RESArray[2];
                            lblMoNumber.Content = _RESArray[3];
                            lblVersionCode.Content = _RESArray[4];
                            ModelSerial  = _RESArray[5];

                        }
                        else
                        {
                            if (_RESArray[0] == "NG")
                            {
                                if (_RESArray[1] == "ER")
                                {
                                    MessageError FrmMessage = new MessageError();

                                    FrmMessage.errorcode = _RESArray[2];
                                    FrmMessage.CustomFlag = false;
                                    FrmMessage.ShowDialog();
                                    txtInputSN.SelectAll();
                                    txtInputSN.Focus();
                                    return;

                                }
                                else if (_RESArray[1] == "MS")
                                {
                                    MessageError FrmMessage = new MessageError();
                                    erromessage = _RESArray[2];
                                    FrmMessage.errorcode = _RESArray[2];
                                    FrmMessage.CustomFlag = true;
                                    FrmMessage.MessageEnglish = string.Format(erromessage);
                                    FrmMessage.MessageVietNam = " SFIS1.PACK_CTN_API_EXECUTE / SN_EXECUTE";
                                    FrmMessage.ShowDialog();
                                    txtInputSN.SelectAll();
                                    txtInputSN.Focus();
                                    return;
                                }
                                else if (_RESArray[1] == "PASSLABEL")
                                {
                                    String ERR = await GetPubMessage(_RESArray[2]);
                                    gridCheckLabel.Visibility = Visibility.Visible;
                                    txtCheckLabel.Text = ERR;

                                    lblMoNumber.Content = _RESArray[3];
                                    lblModelName.Content = _RESArray[4];
                                    lblVersionCode.Content = _RESArray[5];
                                    MessageError FrmMessage = new MessageError();
                                    FrmMessage.errorcode = _RESArray[2];
                                    FrmMessage.CustomFlag = false;
                                    FrmMessage.ShowDialog();
                                    txtInputSN.SelectAll();
                                    txtInputSN.Focus();
                                    return;
                                }
                                else
                                {
                                    MessageError FrmMessage = new MessageError();
                                    erromessage = _RESArray[1];
                                    FrmMessage.CustomFlag = true;
                                    FrmMessage.MessageEnglish = string.Format(erromessage);
                                    FrmMessage.MessageVietNam = " SFIS1.PACK_CTN_API_EXECUTE / SN_EXECUTE";
                                    FrmMessage.ShowDialog();
                                    txtInputSN.SelectAll();
                                    txtInputSN.Focus();
                                    return;
                                }
                            }
                            else
                            {
                                MessageError FrmMessage = new MessageError();
                                FrmMessage.CustomFlag = true;
                                FrmMessage.MessageVietNam = _RESArray[0];
                                FrmMessage.MessageEnglish = "Excute procedure have error:";
                                FrmMessage.ShowDialog();
                                txtInputSN.SelectAll();
                                txtInputSN.Focus();
                                return;

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = ex.Message.ToString();
                        FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                        FrmMessage.ShowDialog();
                        txtInputSN.SelectAll();
                        txtInputSN.Focus();
                        return;
                    }
                }
            }
        }

        private async Task<bool> SNInformationForCodeSoft(string carton_no)
        {
            strsql = "SELECT SERIAL_NUMBER, SHIPPING_SN, PALLET_NO, MCARTON_NO FROM SFISM4.R_WIP_TRACKING_T WHERE CARTON_NO =:CARTON ";
            if (itemOrderbySSN)
            {
                if (MODEL_TYPE.Contains("U"))
                {
                    strsql = strsql + " ORDER BY SUBSTR(SHIPPING_SN,LENGTH(SHIPPING_SN)-4,5) ";
                }
                else
                {
                    if (MODEL_TYPE.Contains("G50"))
                    {
                        strsql = strsql + " order by serial_number ";
                    }
                    else
                    {
                        strsql = strsql + " order by shipping_sn ";
                    }
                }
            }

            var _R107 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strsql,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "CARTON", Value = carton_no }
                    }
            });
            if (_R107.Data != null)
            {
                int i = 0;
                foreach (var row in _R107.Data)
                {
                    i = i + 1;

                }
            }
            return true;
        }

        #region Lay khoi luong thung carton 

        private void cbbComPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemWeight.IsChecked == true)
            {
                GetWeight();
            }
            if ( (cbbComPort.SelectedValue?.ToString() ?? "" ) != "")
            {
                setupPort(cbbComPort.SelectedValue.ToString());
            }
        }
          
        private void itemWeight_Click(object sender, RoutedEventArgs e)
        {

            if (itemWeight.IsChecked == true)
            {
                cbbComPort.Items.Clear();
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    cbbComPort.Items.Add(port);
                }
                cbbComPort.SelectedIndex = 0;
               // setupPort("COM1");
                dataGridKeypart.Visibility = Visibility.Hidden;
                MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "WEIGHT", "TRUE");                
            }
            else
            {
                SP.Close();
                dataGridKeypart.Visibility = Visibility.Visible;
                MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "WEIGHT", "FALSE");
            }
        }

        public void setupPort (string portName)
        {
            try
            {
                SP.Close();
                SP.PortName = portName; //Com Port Name                
                SP.BaudRate = Convert.ToInt32(9600); //COM Port Sp
                SP.Handshake = System.IO.Ports.Handshake.None;
                SP.Parity = Parity.None;
                SP.DataBits = 8;
                SP.StopBits = StopBits.One;
                SP.ReadTimeout = 200;
                SP.WriteTimeout = 50;
                SP.Open();
            }
            catch (Exception exc)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Không thể mở kết nối cổng COM [" + portName + "] của cân. Kiểm tra kết nối cổng COM !";
                FrmMessage.MessageEnglish = "Can't open scale's COM port [" + portName + "]\nERROR: " + exc.Message + "";
                FrmMessage.ShowDialog();
                return;
            }
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1/2);
            dispatcherTimer.Start();

        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (SP.IsOpen)
            {
                try
                {
                    lblWeight.Content = displayWeightStr;
                }
                catch { }
            }
        }
        void GetWeight()
        {
            SP.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            //lblWeight.Content = displayWeightStr;
        }

        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = SP.ReadLine().Trim();
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.StartsWith("OL"))
                    {
                        currentWeight = 0;
                        displayWeightStr = "O";
                    }
                    else
                    {
                        string type = data.ToUpper().Contains("KG") ? " kg" : data.ToUpper().Contains("G") ? " g" : "";
                        data = data.ToUpper().Replace("KG", "").Replace("G", "").Trim().Split(' ').Last();

                        displayWeightStr = data + type;
                        float.TryParse(data, out currentWeight);
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:Lỗi truyền dữ liệu trọng lượng cân: " + exc.Message + "");
                return;
            }
        }


        #endregion

        private async Task closeCarton()
        {
            string Weight_CTN;
            if (itemWeight.IsChecked)
            {
                currentWeight = 0;
                Weight_CTN = lblWeight.Content.ToString();
                if (!await zCheckWeight(CartonNo,MCARTON_NO,Weight_CTN, lblModelName.Content.ToString(),lblMoNumber.Content.ToString())) return;
            }

            if (lblCartonQTY.Content.ToString() != lblCount.Content.ToString())
            {
                String ERR = await GetPubMessage("00062");
                MessageBoxResult dlr = MessageBox.Show(ERR, "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (dlr == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            if ((lblCartonQTY.Content != lblCount.Content)
                 || (lblCount.Content.ToString() != "0"))
            {
                try
                {
                    btnCloseCarton.IsEnabled = false;
                    /// changeMcarton
                    var logInfo = new
                    {
                        OPTION = "CHANGEMCARTON",
                        MODEL_NAME = lblModelName.Content,
                        CARTON = lblCartonNo.Content,
                        MCARTON = MCARTON_NO,
                        C_VERSION = lblVersionCode.Content
                    };
                    string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

                    var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                    });
                    if (result.Data != null)
                    {
                        dynamic ads = result.Data;
                        string _RES = ads[0]["output"];
                        _RESArray = _RES.Split('#');

                        if (_RESArray[0] != "OK")
                        {
                            MessageError FrmMessage = new MessageError();
                            string _erro = _RESArray[2];
                            FrmMessage.CustomFlag = true;
                            FrmMessage.MessageEnglish = string.Format("change Mcarton erro : " + _erro);
                            FrmMessage.MessageVietNam = string.Format("Thay đổi mã mcarton phát sinh lỗi  ");
                            FrmMessage.ShowDialog();
                            return;
                        }
                        else if (_RESArray[1] == "Carton_le")
                        {
                            MCARTON_NO = _RESArray[2];
                        }
                    }

                    // end changeMcarton
                    iCarton_LabelQty = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "CLabelQTY", "Default", 1);

                    if (itemPrintCartonLabel.IsChecked)
                    {
                        if (!await getCartonInformation(MCARTON_NO, " SFISM4.R107 ", " MCARTON_NO"))
                        {
                            return;
                        }
                        if (!await PrintToCodeSoft(LabelName, false))
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (!await zCloseCarton(MCARTON_NO)) return;
                    }
                    string ACTION_DESC = lblCartonNo.Content + "; MAC: " + lblMacAddress.Content + "; IP:" + IP;

                    String sql = " Insert into SFISM4.R_SYSTEM_PRGLOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                                + " Values ('" + empNo + "', 'PACK_CTN', 'CLOSECART', '" + ACTION_DESC + "') ";
                    var _insert = _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql
                    });



                    txtInputSN.SelectAll();
                    txtInputSN.Focus();
                    btnCloseCarton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    showMessage("Close carton error , carton no: " + lblCartonNo.Content.ToString(), ex.Message.ToString(), true);
                    return;
                }
            }
        }
        private async void btnCloseCarton_Click(object sender, RoutedEventArgs e)
        {
            string SQL = "SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP = '" + empNo + "' AND FUN= 'CLOSE_CARTON'";
            var _PRIVILEGE = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = SQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (_PRIVILEGE.Data == null)
            {
                MessageBox.Show("EMP NO PRIVILEGE CLOSE CARTON!", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                return;
            }
            //check Bracket
            strsql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE VR_CLASS='ISBRACKET' AND PRG_NAME ='PACK_CTN' AND VR_VALUE ='" + lblModelName.Content + "'";
            var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strsql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result1.Data != null)
            {
                IsBracket frmIsBracket = new IsBracket();
                frmIsBracket.KP = "BRACKET";
                frmIsBracket.RES = "NA";
                frmIsBracket.lblCartonNo = lblMCartonNo;
                frmIsBracket.ShowDialog();
                if (frmIsBracket.RES != "OK")
                    return;
            }
            //end
            //link RFID_label
            var logInfo1 = new
            {
                OPTION = "RFID_CONFIG",
                DATA = MCARTON_NO
            };
            string jsonData1 = JsonConvert.SerializeObject(logInfo1).ToString();

            var result2 = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="DATA",Value=jsonData1,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                }
            });
            dynamic ads1 = result2.Data;
            string _RES1 = ads1[0]["output"];
            _RESArray = _RES1.Split('#');
            if (_RESArray[0] == "OK")
            {
                if (_RESArray[1] == "Y")
                {
                    RfidForm rfidform = new RfidForm();
                    rfidform.ShowDialog();

                    //check
                    strsql = "SELECT * FROM SFISM4.R_SEC_LIC_LINK_T WHERE CARTON_NO ='" + MCARTON_NO + "' AND RFID IS NOT NULL";
                    result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (result1.Data == null)
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("Link RFID is null ");
                        FrmMessage.MessageVietNam = string.Format("Link RFID rỗng ");
                        FrmMessage.ShowDialog();
                        return;
                    }
                }
            }
            else if (_RESArray[0] == "NG")
            {
                MessageError FrmMessage = new MessageError();
                erromessage = _RESArray[1];
                FrmMessage.errorcode = _RESArray[1];
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = string.Format(erromessage);
                FrmMessage.MessageVietNam = " SFIS1.PACK_CTN_API_EXECUTE / RFID_CONFIG ";
                txtInputshipping.Focus();
                txtInputshipping.SelectAll();
                FrmMessage.ShowDialog();
            }
            ///dong le carton
            await closeCarton();
        }
        public async Task<bool> zCloseCarton(string mCarton)
        {
            string CARTON_NO = "";
            var logInfo = new
            {
                OPTION = "F_CLOSE_CARTON",
                DATA = mCarton
            };
            string Jsdata = JsonConvert.SerializeObject(logInfo).ToString();

            var result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value= Jsdata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

            });
            if (result.Data != null)
            {
                dynamic ads = result.Data;
                string _RES = ads[0]["output"];
                _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    CARTON_NO = _RESArray[1];
                    MainWindow.LabelName = _RESArray[3];
                    MainWindow.MODEL_TYPE = _RESArray[4];

                    MessageBox.Show("This Carton Already Closed!!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    loadIniFile();
                    return true;

                }
                else
                {
                    if (_RESArray[1] == "ER")
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.errorcode = _RESArray[2];
                        FrmMessage.CustomFlag = false;
                        FrmMessage.ShowDialog();
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.MessageEnglish = " Close carton error !! ";
                        FrmMessage.MessageVietNam = _RESArray[2];
                        FrmMessage.CustomFlag = false;
                        FrmMessage.ShowDialog();
                    }
                    return false;
                }
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Data null";
                FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                FrmMessage.ShowDialog();
                return false;
            }
        }
        public async Task<bool> zCheckWeight(string CartonNo,string mCarton,string weight_CTN,string model,string mo)
        {
            string CARTON_NO = "";
            var logInfo = new
            {
                OPTION = "CheckWeight",
                DATA = mCarton,
                CARTON = CartonNo,
                WEIGHT = weight_CTN,
                MODEL = model,
                EMP = empNo,
                MO = mo
            };
            string Jsdata = JsonConvert.SerializeObject(logInfo).ToString();

            var result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value= Jsdata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

            });
            if (result.Data != null)
            {
                dynamic ads = result.Data;
                string _RES = ads[0]["output"];
                _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    return true;

                }
                else
                {
                    if (_RESArray[1] == "ER")
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = " CTN_WEIGHT error !! ";
                        FrmMessage.MessageVietNam = _RESArray[2];
                        FrmMessage.ShowDialog();
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = " CTN_WEIGHT error !! ";
                        FrmMessage.MessageVietNam = _RESArray[2];
                        FrmMessage.ShowDialog();
                    }
                    return false;
                }
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Data null";
                FrmMessage.MessageEnglish = "Call procedure have exceptions: ";
                FrmMessage.ShowDialog();
                return false;
            }
        }
        public void showMessage (String strVN , String strEN , Boolean flag)
        {
            if(strEN.IndexOf('|') > 0)
            {
                strVN = strVN + strEN.Split('|')[0];
                strEN= strEN.Split('|')[1];
            }
            if (strVN.IndexOf('|') > 0)
            {
                strEN = strEN + strVN.Split('|')[0];
                strVN = strVN.Split('|')[1];
            }
            MessageError FrmMessage = new MessageError();
            FrmMessage.CustomFlag = flag;
            FrmMessage.MessageVietNam = strVN;
            FrmMessage.MessageEnglish = strEN;
            FrmMessage.ShowDialog();
        }
        public async Task<String> getParamNameItem(string name)
        {
            try
            {
                itemLable = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "LB_Carton", 1);

                if ((item_LH_Carton.IsChecked == true) || (itemLable == 1))
                {
                    lbIndex = 1;
                }
                else if ((itemLHcartonMac.IsChecked == true) || (itemLable == 2))
                {
                    lbIndex = 2;
                }

                strParamName = "NG";
                string sql1 = " SELECT distinct NAME FROM SFISM4.R_LABEL_VAR_T WHERE MD5 = ( SELECT MD5 FROM SFIS1.C_LABEL_WIP_T WHERE LABEL_NAME  ='" + name + "' AND  PATH_INDEX ='" + lbIndex + "' ) ";
                var resultVarName = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql1,
                    SfcCommandType = SfcCommandType.Text
                });

                if (resultVarName.Data != null && resultVarName.Data.Count() > 0 )
                {
                    strParamName = "OK,";
                    foreach (var row in resultVarName.Data)
                    {
                        strParamName = strParamName +  row["name"].ToString() + "," ;
                        AddItemsLabel(row["name"].ToString(), "");
                    } 
                    return strParamName;
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("Can not get param name item label , check r_label_var ");
                    FrmMessage.MessageVietNam = string.Format("Không tìm thấy tên bien label , kiểm tra r_label_var " );
                    FrmMessage.ShowDialog();
                    return strParamName;
                }
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = ex.Message.ToString();
                FrmMessage.MessageEnglish = "GET param name label For addpram ERROR ";
                FrmMessage.ShowDialog();
                return strParamName;
            }
        }

        public async Task<bool> getLabelName(string model, string ver)
        {
            try
            {
                var _Label_name = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " SELECT LOWER (CARTON_LAB_NAME) CARTON_LAB_NAME , CUST_MODEL_DESC  FROM  SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME =:MODEL AND VERSION_CODE  =:VERSION ",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "MODEL", Value = model },
                        new SfcParameter{ Name = "VERSION", Value = ver}
                    }
                });
                if (_Label_name.Data != null && _Label_name.Data.Count() != 0)
                {
                    dynamic _ads = _Label_name.Data;
                    LabelName = _ads["carton_lab_name"];
                    CustModelDesc = _ads["cust_model_desc"];
                    return true;
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("Can not get label name , check config 19 ");
                    FrmMessage.MessageVietNam = string.Format("Không tìm thấy tên label , kiểm config 19 ");
                    FrmMessage.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = ex.Message.ToString();
                FrmMessage.MessageEnglish = "Get Label Name error ,Model: " + model + " ,ver: " + ver;
                FrmMessage.ShowDialog();
                return false;
            }
        }

        public async Task<Boolean> printCartonLabel_CodeSoft(string paramCarton , String tb107)
        {
            string My_MoNumber1, My_ModelName1, ship_addr, macid, USB_MAC = null, SN;

            int i = 0;
            try
            {
                if ("T".Contains(MODEL_TYPE))
                {

                }
                if (itemScanTrayNo.IsChecked == true)
                {
                    strsql = "SELECT DISTINCT TRAY_NO  , COUNT(*)  FROM  " + tb107 + " WHERE ( MCARTON_NO =:CARTON OR CARTON_NO =:CARTON) " +
                            " GROUP BY TRAY_NO";
                }
                else
                {

                    
                    strsql = "SELECT * FROM " + tb107 + " WHERE MCARTON_NO =:CARTON OR CARTON_NO =:CARTON ";
                }

                if (MODEL_TYPE.Contains("U"))
                {
                    strsql = strsql + " ORDER BY SUBSTR(SHIPPING_SN,LENGTH(SHIPPING_SN)-4,5) ";
                }
                else
                {
                    if (MODEL_TYPE.Contains("G50"))
                    {
                        strsql = strsql + " order by serial_number ";
                    }
                    else
                    {
                        if (itemScanTrayNo.IsChecked == true)
                        {
                            strsql = strsql + " order by TRAY_NO ";
                        }
                        else
                        {
                            strsql = strsql + " order by shipping_sn ";
                        }
                          
                    }
                }


                var _R107 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strsql,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "CARTON", Value =paramCarton}
                    }
                });
                if (_R107.Data != null)
                {
                    if (_R107.Data.Count() == 0)
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("Can not found data : " + paramCarton + " in " + tb107);
                        FrmMessage.MessageVietNam = string.Format("Khong tim thay du lieu: " + paramCarton + " trong " + tb107);
                        FrmMessage.ShowDialog();
                        return false;
                    }
                    dynamic ads = _R107.Data;
                    My_MoNumber1 = ads[0]["mo_number"];
                    My_ModelName1 = ads[0]["model_name"];
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("Can not found data : " + paramCarton + " in " + tb107);
                    FrmMessage.MessageVietNam = string.Format("Khong tim thay du lieu: " + paramCarton + " trong " + tb107);
                    FrmMessage.ShowDialog();
                    return false;
                }

                var _ship_addr = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "SELECT * FROM SFIS1.C_SHIP_ADDR_T WHERE SHIP_INDEX IN (SELECT OPTION_DESC FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER =: MO )",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "MO", Value = My_MoNumber1}
                    }
                });
                if (_ship_addr.Data != null && _ship_addr.Data.Count() != 0)
                {
                    dynamic ads = _ship_addr.Data;
                    ship_addr = ads["ship_address"];
                }

                var _CUST_MODEL = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "SELECT FINISH_GOOD2,CUST_MODEL_NAME,CUSTMODEL_DESC1,CUSTMODEL_DESC2,"
                                + "CUSTMODEL_DESC3,CUSTMODEL_DESC4,CUSTMODEL_DESC5,CUSTOMER_NAME "
                                + "FROM SFIS1.C_CUST_MODEL_T WHERE MODEL_NAME =:MODEL ",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter{ Name = "MODEL", Value = My_ModelName1}
                }
                });
                if (_CUST_MODEL.Data != null && _CUST_MODEL.Data.Count() != 0)
                {
                    dynamic ads = _CUST_MODEL.Data;
                    ship_addr = ads["ship_address"];
                    if ((ship_addr != null) || (ship_addr != ""))
                    {
                        AddParams("SHIP_TO", ship_addr);
                    }
                    // findCartonInfo( paramCarton); 
                    // AddParams("PAGENO", TRIM(INTTOSTR(J)));
                    sCustModel_Name = ads["cust_model_name"];
                    sCustModel_Desc1 = ads["custmodel_desc1"];
                    sCustModel_Desc2 = ads["custmodel_desc2"];
                    sCustModel_Desc3 = ads["custmodel_desc3"];
                    sCustModel_Desc4 = ads["custmodel_desc4"];
                    sCustModel_Desc5 = ads["custmodel_desc5"];
                    sCustomer_Name = ads["customer_name"];

                }
                if (itemScanTrayNo.IsChecked)
                {
                    if (!await findCartonInfo(paramCarton, ""))
                    {
                        return false;
                    }
                    AddParams("ModelName", sCustModel_Name);
                    AddParams("MODELDESC1", sCustModel_Desc1);
                    AddParams("MODELDESC2", sCustModel_Desc2);
                    AddParams("MODELDESC3", sCustModel_Desc3);
                    AddParams("MODELDESC4", sCustModel_Desc4);
                    AddParams("MODELDESC5", sCustModel_Desc5);
                    AddParams("CUSTOMERNAME", sCustomer_Name);


                    if (! await PrintToCodeSoft(LabelName, false))
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    if (!await findCartonInfo(paramCarton, "mainWindow"))
                    {
                        return false;
                    }

                    AddParams("CModelName", sCustModel_Name);
                    AddParams("CMODELDESC1", sCustModel_Desc1);
                    AddParams("CMODELDESC2", sCustModel_Desc2);
                    AddParams("CMODELDESC3", sCustModel_Desc3);
                    AddParams("CMODELDESC4", sCustModel_Desc4);
                    AddParams("CMODELDESC5", sCustModel_Desc5);
                    AddParams("CCUSTOMERNAME", sCustomer_Name);

                    i = 0;
                    foreach (var row in _R107.Data)
                    {
                        i = i + 1;
                        SN = row["serial_number"].ToString();
                        // AddParams("SN" + i.ToString(), SN);
                        //AddParams("MSN" + i.ToString(), row["shipping_sn"].ToString());
                        //  macid = await FindMac(SN, row["model_name"].ToString());
                        // AddParams("MAC" + i.ToString(), macid);
                        // USB_MAC = NEXTMAC(macid);
                        AddParams("USB_MAC" + i.ToString(), USB_MAC);

                        // get data in R_CUST
                        var _R_CUSTSN = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER  =: SN ",
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "SN", Value = SN }
                    }
                        });
                        if (_R_CUSTSN.Data != null && _R_CUSTSN.Data.Count() != 0)
                        {
                            dynamic ads = _R_CUSTSN.Data;
                            AddParams("MSNA" + i.ToString(), ads["ssn1"]);
                            AddParams("MSNB" + i.ToString(), ads["ssn2"]);
                            AddParams("MSNC" + i.ToString(), ads["ssn3"]);
                            AddParams("MSND" + i.ToString(), ads["ssn4"]);
                            AddParams("MSNE" + i.ToString(), ads["ssn5"]);
                            AddParams("MSNF" + i.ToString(), ads["ssn6"]);
                            AddParams("MSNG" + i.ToString(), ads["ssn7"]);
                            AddParams("MSNH" + i.ToString(), ads["ssn8"]);
                            AddParams("MSNL" + i.ToString(), ads["ssn12"]);
                            if (loginDB != "ROKU")
                            {
                                AddParams("MSNK" + i.ToString(), ads["ssn13"]);
                                AddParams("MSNM" + i.ToString(), ads["ssn14"]);
                                AddParams("MSNN" + i.ToString(), ads["ssn15"]);
                                AddParams("MSNO" + i.ToString(), ads["ssn16"]);
                                AddParams("MSNU" + i.ToString(), ads["ssn17"]);
                                AddParams("MSNP" + i.ToString(), ads["ssn18"]);
                                AddParams("MSNQ" + i.ToString(), ads["ssn19"]);
                                AddParams("MSNR" + i.ToString(), ads["ssn20"]);
                            }
                            AddParams("MACA" + i.ToString(), ads["mac1"]);
                            AddParams("MACB" + i.ToString(), ads["mac2"]);
                            AddParams("MACC" + i.ToString(), ads["mac3"]);
                            AddParams("MACD" + i.ToString(), ads["mac4"]);
                            AddParams("MACE" + i.ToString(), ads["mac5"]);
                        }
                        if ("D".Contains(MODEL_TYPE))
                        {
                            // MTAMAC 
                            // DEC_MSN
                        }
                        if ("E".Contains(MODEL_TYPE))
                        {

                        }

                    }

                    if (! await PrintToCodeSoft(LabelName, false))
                    {
                        return false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                showMessage("print Carton Label CodeSoft error", ex.Message, true);
                return false;
            }
        }

        private async Task<bool> findCartonInfo (string paramCarton, string FormSender)
        {
            try
            {
                int i;
                Process currentp = Process.GetCurrentProcess();
                FrmSender = currentp.MainWindowTitle;

                RprintZ_WIP = MES.OpINI.IniUtil.ReadINI_B(strINIPath, "PACK2", "RB_Carton", false); // z_wip reprint from  
                itemScanInput = MES.OpINI.IniUtil.ReadINI_Int(MainWindow.strINIPath, "PACK2", "Scan_Input", 0);

                

                if (RprintZ_WIP && FrmSender == "Reprint")
                {
                    if (itemScanInput == 3)
                    {
                        strsql = " select count(*) QTY FROM (  Select distinct tray_no from sfism4.Z_wip_tracking_t a " +
                                      " where( MCARTON_NO = '" + paramCarton + "' OR A.CARTON_NO = '" + paramCarton + "' )) ";
                    }
                    else
                    {
                        strsql = " Select COUNT(*) QTY from sfism4.Z_wip_tracking_t a " +
                                      " where( MCARTON_NO = '" + paramCarton + "' OR A.CARTON_NO = '" + paramCarton + "' ) ";
                    }

                    var _Z107 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (_Z107.Data != null)
                    {
                        dynamic ads = _Z107.Data;

                        CartonQTY = ads["qty"];
                    }
                    else
                    {
                        MessageBox.Show("Get qty in carton result null , carton_no: " + paramCarton, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }


                    _Z107 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = " Select b.MODEL_NAME,b.MO_NUMBER, b.VERSION_CODE,a.track_no,a.carton_no ,a.mcarton_no, b.pmcc ,b.VENDER_PART_NO ,COUNT(*) QTY from sfism4.Z_wip_tracking_t a, sfism4.r105 b " +
                               " where(A.MCARTON_NO = '" + paramCarton + "' OR A.CARTON_NO = '" + paramCarton + "'  ) and a.mo_number = b.mo_number " +
                               " group by b.MODEL_NAME,b.MO_NUMBER, b.VERSION_CODE, b.pmcc,a.track_no ,b.VENDER_PART_NO ,a.carton_no ,a.mcarton_no ",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (_Z107.Data != null)
                    {
                        dynamic ads = _Z107.Data;

                        AddParams("PackLotNO", ads["pmcc"]);
                        AddParams("MO", ads["mo_number"]);
                        AddParams("Version", ads["version_code"]);
                        AddParams("TRACK_NO", ads["track_no"]);

                        MODEL_NAME = ads["model_name"];
                        VERSION_CODE = ads["version_code"];
                        M_sUPCEANData = ads["vender_part_no"];
                        MCARTON_NO = ads["mcarton_no"];
                        CartonNo = ads["carton_no"];

                        if (MODEL_TYPE.Contains("065") || MODEL_TYPE.Contains("W") || MODEL_TYPE.Contains("051"))
                        {
                            // PAddSendListBox('TSQ', TSQ)
                        }
                    }
                }
                else
                {
                    if (itemScanTrayNo.IsChecked == false)
                    {
                        var _R108 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = " SELECT KEY_PART_NO , KEY_PART_SN FROM SFISM4.R108 WHERE SERIAL_NUMBER  IN ( " +
                                        " SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO = '" + paramCarton + "' OR CARTON_NO = '" + paramCarton + "' ) ",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (_R108.Data != null)
                        {
                            i = 1;
                            foreach (var row in _R108.Data)
                            {
                                AddParams(row["key_part_no"]?.ToString() ?? "" + i, row["key_part_sn"]?.ToString() ?? "");
                                i = i + 1;
                            }
                        }
                    }

                    try
                    {
                        if (itemScanInput == 3)
                        {
                            strsql = "  Select COUNT(*) QTY from (select distinct tray_no from sfism4.r_wip_tracking_t a " +
                                " where( MCARTON_NO = '" + paramCarton + "' OR A.CARTON_NO = '" + paramCarton + "' )) ";
                        }  
                        else
                        {
                            strsql = " Select COUNT(*) QTY from sfism4.r_wip_tracking_t a " +
                            " where( MCARTON_NO = '" + paramCarton + "' OR A.CARTON_NO = '" + paramCarton + "' ) ";
                        }
                        var _R107 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strsql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (_R107.Data != null)
                        {
                            dynamic ads = _R107.Data;
                            CartonQTY = ads["qty"];
                        }
                        else
                        {
                            MessageBox.Show("Get qty in carton result null , carton_no: " + paramCarton, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }


                        _R107 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = " Select b.MODEL_NAME,b.MO_NUMBER, b.VERSION_CODE,a.track_no,a.carton_no ,a.mcarton_no, b.pmcc ,b.VENDER_PART_NO  from sfism4.r_wip_tracking_t a, sfism4.r105 b " +
                                 " where(A.MCARTON_NO = '" + paramCarton + "' OR A.CARTON_NO = '" + paramCarton + "'  ) and a.mo_number = b.mo_number " +
                                 " group by b.MODEL_NAME,b.MO_NUMBER, b.VERSION_CODE, b.pmcc,a.track_no ,b.VENDER_PART_NO ,a.carton_no ,a.mcarton_no ",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (_R107.Data != null)
                        {
                            dynamic ads = _R107.Data;

                            AddParams("PackLotNO", ads["pmcc"]);
                            AddParams("MO", ads["mo_number"]);
                            AddParams("Version", ads["version_code"]);
                            AddParams("TRACK_NO", ads["track_no"]);

                            MODEL_NAME = ads["model_name"];
                            VERSION_CODE = ads["version_code"];
                            M_sUPCEANData = ads["vender_part_no"];
                            MCARTON_NO = ads["mcarton_no"];
                            CartonNo = ads["carton_no"];
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = ex.Message.ToString();
                        FrmMessage.MessageEnglish = "GET SN Information For CodeSoft ERROR ";
                        FrmMessage.ShowDialog();
                        return false;
                    }
                }

                if (!await getLabelName(MODEL_NAME, VERSION_CODE))
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Error";
                    FrmMessage.MessageEnglish = "Get cust_model_desc error !!";
                    FrmMessage.ShowDialog();
                    return false;
                }

                AddParams("ModelName", MODEL_NAME);
                AddParams("MCartonNO", MCARTON_NO);
                AddParams("CartonNO", CartonNo);
                AddParams("CartonQty", CartonQTY.ToString());
                AddParams("UPCEACDATA", M_sUPCEANData);
                AddParams("ModelDesc", CustModelDesc);

          
                DataWeight = currentWeight.ToString();
               
                if (await WeigthInfor(MCARTON_NO, DataWeight,FrmSender))
                {
                    AddParams("CTNWeight", DataWeight);
                    lblOldCarton.Content = MCARTON_NO;
                    lblOldWeight.Content = DataWeight;
                }
                else
                {
                    return false;
                }
                
                /*
                if (FrmSender == "Reprint")
                {
                    var _Weight = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SELECT  * FROM SFISM4.R_WEIGHT_T WHERE  CARTON_NO = '" + MCARTON_NO + "' ",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if ((_Weight.Data != null) && (_Weight.Data.Count != 0))
                    {
                        AddParams("CTNWeight", _Weight.Data["carton_weight"]?.ToString() ?? "0");
                    }
                }
                */

                var config15 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " SELECT  DISTINCT  A.CARTON_QTY ,B.CUST_VENDER_CODE  FROM SFIS1.C_PACK_PARAM_T A , SFIS1.C_CUST_SNRULE_T B  WHERE  A.MODEL_NAME = '" + MODEL_NAME + "' AND  A.VERSION_CODE = '" + VERSION_CODE + "' AND A.MODEL_NAME  =B.MODEL_NAME  AND A.VERSION_CODE = B.VERSION_CODE AND ROWNUM = 1  ",
                    SfcCommandType = SfcCommandType.Text
                });
                if ((config15.Data != null) && (config15.Data.Count != 0))
                {
                    dynamic data = config15.Data;
                    string VenderCode = data["cust_vender_code"];
                    Double QTYConfig15 = data["carton_qty"];

                    AddParams("PackParamQTY", QTYConfig15.ToString());
                    AddParams("VenderCode", VenderCode);
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = false;
                    FrmMessage.errorcode = "00111";
                    FrmMessage.ShowDialog();
                    return false;
                }

                // TAI HAI
                if (MODEL_TYPE.Contains("G38"))
                {
                    var Result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = " SELECT  * FROM SFIS1.C_PACK_PARAM_T  WHERE MODEL_NAME = '" + MODEL_NAME + "' AND VERSION_CODE = '" + VERSION_CODE + "'  ",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if ((Result.Data != null) && (Result.Data.Count != 0))
                    {
                        dynamic data = Result.Data;
                        CTN_QTY_CONFIG = data["carton_qty"];
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = false;
                        FrmMessage.errorcode = "00111";
                        FrmMessage.ShowDialog();
                        return false;
                    }

                    var targetQty = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = "  SELECT  SUM(TARGET_QTY) QTY FROM sfism4.r105  WHERE MODEL_NAME ='" + MODEL_NAME + "' ",
                        SfcCommandType = SfcCommandType.Text
                    });

                    if ((targetQty.Data != null) && (targetQty.Data.Count != 0))
                    {
                        dynamic data = targetQty.Data;
                        TARGET_QTY_MODEL = data["qty"];
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = "Model name : " + MODEL_NAME + " not exist in R105 , Please check !! ";
                        FrmMessage.MessageVietNam = "Tên hàng : " + MODEL_NAME + " không tồn tại trong R105 !!";
                        FrmMessage.ShowDialog();
                        return false;
                    }
                    PHAN_NGUYEN = int.Parse(TARGET_QTY_MODEL.ToString()) / int.Parse(CTN_QTY_CONFIG.ToString());
                    PHAN_DU = int.Parse(TARGET_QTY_MODEL.ToString()) % int.Parse(CTN_QTY_CONFIG.ToString());
                    if (PHAN_DU > 0)
                    {
                        PHAN_NGUYEN = PHAN_NGUYEN + 1;
                    }
                    AddParams("TOTAL_CARTON", PHAN_NGUYEN.ToString());

                    if (MODEL_TYPE.Contains("G41"))
                    {
                        strsql = "    SELECT COUNT (*) COUNT FROM (  SELECT  DISTINCT MCARTON_NO FROM SFISM4.R107  WHERE MODEL_NAME= '" + MODEL_NAME + "' AND VERSION_CODE='" + VERSION_CODE + " ' " +
                               " AND SUBSTR(MCARTON_NO,1,6 )= (select C_CUST_BOX_PREFIX || 'CUL' FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME " +
                               " = '" + MODEL_NAME + "' AND VERSION_CODE = '" + VERSION_CODE + "' ))";
                    }
                    else
                    {
                        strsql = " SELECT COUNT (*) COUNT FROM ( SELECT  distinct mcarton_no FROM sfism4.r107  WHERE MODEL_NAME='" + MODEL_NAME + "' " +
                                 " AND VERSION_CODE='" + VERSION_CODE + "' and SUBSTR(mcarton_no,1,3 )= 'CUL' ) ";
                    }

                    var snRule = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (snRule.Data != null)
                    {
                        dynamic DATA = snRule.Data;
                        if (int.Parse(DATA["count"]) == 0)
                        {
                            MessageError FrmMessage = new MessageError();
                            FrmMessage.CustomFlag = true;
                            FrmMessage.MessageEnglish = " No data in R107, Please check!! (Model_type G38) ";
                            FrmMessage.MessageVietNam = " Khong co du lieu trong R107 (Model_type G38) ";
                            FrmMessage.ShowDialog();
                            return false;
                        }
                        else
                        {
                            AddParams("NUMBER_CARTON", DATA["count"]);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageEnglish = " find Carton Info have error ";
                FrmMessage.MessageVietNam = ex.Message;
                FrmMessage.ShowDialog();
                return false;
            }
        }

        public void loadIniFile()
        {
            strINIPath = "C:\\PACKING\\PACKING.INI";

            if (!File.Exists("C:\\PACKING"))
            {
                System.IO.Directory.CreateDirectory("C:\\PACKING");
            }
            if (!File.Exists(strINIPath))
            {
                File.Create(strINIPath);
            }

            tbLineName.Text = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK2", "LINE", "");

            SSNLength = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "SSNLength", 12);
            KPSNLength = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "KPSNLength", 12);
            KPQTY = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "KPQTY", 0);
            IsUpdateR105 = MES.OpINI.IniUtil.ReadINI_B(strINIPath, "PACK2", "OutputLine", false);

            CheckKp = MES.OpINI.IniUtil.ReadINI_B(strINIPath, "PACK2", "Checkkp", false);
            _TxtQtyLable = MES.OpINI.IniUtil.ReadINI(strINIPath, "CLabelQTY", "Default", "1");

            itemOrderbySSN = MES.OpINI.IniUtil.ReadINI_B(strINIPath, "PACK2", "OrderBySSN", false);

            // = MES.OpINI.IniUtil.ReadINI_Int(strINIPath,"PRINT_TOP", "Default", 0);
            IsPrintCartonLabel = MES.OpINI.IniUtil.ReadINI_B(strINIPath, "PACK2", "IsPrintCartonLabel", true);
            My_PrintMethod = MES.OpINI.IniUtil.ReadINI(strINIPath, "CONFIGURATION", "My_PrintMethod", "CODESOFT");
            if (My_PrintMethod == "SENDCOMMAND")
            {
                itemCODESOFT.IsChecked = false;
                itemSendcommand.IsChecked = true;
            }
            itemLable = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "LB_Carton", 1);
            if (itemLable == 1)
            {
                item_LH_Carton.IsChecked = true;
            }
            else if (itemLable == 2)
            {
                itemLHcartonMac.IsChecked = true;
            }
            else if (itemLable == 3)
            {
                itemGLcarton.IsChecked = true;
            }
            else if (itemLable == 4)
            {
                itemGLcartonMac.IsChecked = true;
            }
            else if (itemLable == 5)
            {
                itemLocalCarton.IsChecked = true;
            }

            itemScanInput = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "Scan_Input", 0);
            if (itemScanInput == 1)
            {
                itemScanSN.IsChecked = true;
                lbError.Content = "Please input serial number !";
                lblistSNitem1.Content = "SN";
                lblistSNitem2.Content = "SHIPPING SN";

            }
            else if (itemScanInput == 3)
            {
                lblistSNitem1.Content = "TRAY NO";
                lblistSNitem2.Content = "QTY";
                lbSN.Content = "TRAY NO";
                itemScanTrayNo.IsChecked = true;
                lbError.Content = "Please input TRAY NO !";
        
            }
            else if (itemScanInput == 4)
            {

                lbSN.Content = "MAC2";
                itemScanMac2.IsChecked = true;
                lbError.Content = "Please input MAC2 !";
                lblistSNitem1.Content = "SN";
                lblistSNitem2.Content = "MAC2";
            }
            else if (itemScanInput == 5)
            {
                lbSN.Content = "MAC";
                lbSSN.Content = "SSN";
                itemScanMac.IsChecked = true;
                lbError.Content = "Please input MAC !";
                lblistSNitem1.Content = "SN";
                lblistSNitem2.Content = "MAC";
            }
            else 
            {
                lbSN.Content = "SSN";
                lbSSN.Content = "SN";
                itemScanShippingSN.IsChecked = true;
                lbError.Content = "Please input shipping sn !";
                lblistSNitem1.Content = "SN";
                lblistSNitem2.Content = "SHIPPING SN";
            }
            if (itemScanInput != 0)
            {
                var bc = new BrushConverter();
                lbError.Foreground = (Brush)bc.ConvertFrom("#0889a6");
            }
            if (MES.OpINI.IniUtil.ReadINI_B(strINIPath, "PACK2", "WEIGHT", false))
            {
                dataGridKeypart.Visibility = Visibility.Visible;
                itemWeight.IsChecked = false;
            }
        }

        private async Task<bool> deleteCarton(string _carton)
        {
            var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = " delete sfis1.c_pallet_t " +
                       " where pallet_no =:CARTON  and close_flag = 'Carton' ",
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="CARTON",Value= _carton}
                }
            });
            if (result.Result.ToString() != "OK")
            {
                return false;
            }

            if ("R".Contains(MODEL_TYPE))
            {
                var _result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "  Delete SFISM4.R_Pallet_T   Where MCARTON_NO IN  " +
                         " (SELECT CUST_PALLET_NO FROM SFIS1.C_PALLET_T  WHERE PALLET_NO =:CARTON  AND   CLOSE_FLAG='Carton' ) ",
                    SfcParameters = new List<SfcParameter>()
                            {
                             new SfcParameter{Name="CARTON",Value= _carton}
                            }
                });
                if (_result.ToString() != "OK")
                {
                    return false;
                }
            }

            return true;
        }


        private void lblCartonNo_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //  getSNinCarton();
        }


        public async Task<string> GetPubMessage(string PROMPT_CODE)
        {
            string SLANGUAGE = "";
            SLANGUAGE = MES.OpINI.IniUtil.ReadINI(strINIPath, "LANGUAGES", "LANGUAGE", "");
            if (!string.IsNullOrWhiteSpace(SLANGUAGE)) PROMPT_CODE = SLANGUAGE + PROMPT_CODE;
            var result_procedure = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.GET_PROMPT_MESSAGE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="P_PROMPT_CODE",Value=PROMPT_CODE,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                }
            });
            dynamic ads = result_procedure.Data;

            StrbMessage = ads[0]["res"];
            return StrbMessage;
        }

        public async Task<bool> CartonInformationForCodeSoft(String CARTON , String tb107 ,String colName)
        {
            string mac;
            Process currentp = Process.GetCurrentProcess();
            FrmSender = currentp.MainWindowTitle;
            try
            {
                strsql = "SELECT SERIAL_NUMBER, MODEL_NAME  ,VERSION_CODE,MO_NUMBER , SHIPPING_SN,CARTON_NO, KEY_PART_NO, TRACK_NO, MCARTON_NO  FROM "+ tb107 + " WHERE "+ colName + " =:CARTON ";
                //if (itemOrderbySSN)
                //{
                if (MODEL_TYPE.Contains("U"))
                {
                    strsql = strsql + " ORDER BY SUBSTR(SHIPPING_SN,LENGTH(SHIPPING_SN)-4,5) ";
                }
                else
                {
                    if (MODEL_TYPE.Contains("G50"))
                    {
                        strsql = strsql + " order by serial_number ";
                    }
                    else
                    {
                        strsql = strsql + " order by shipping_sn ";
                    }
                }
                //}

                var SnInformation = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strsql,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "CARTON", Value = CARTON }
                    }
                });

                if (SnInformation.Data != null)
                {
                    string qty = SnInformation.Data.Count().ToString();
                    dtParams.Clear();
                    dynamic ads = SnInformation.Data;
                    MODEL_NAME = ads[0]["model_name"];
                    MO_NUMBER = ads[0]["mo_number"];
                    VERSION_CODE = ads[0]["version_code"];
                    MCARTON_NO = ads[0]["mcarton_no"];
                    CartonNo = ads[0]["mcarton_no"];

                    AddParams("Carton_Qty", qty);
                    if (FrmSender == "Reprint")
                    {
                        //AddParams("Model_Name", MODEL_NAME);
                        //AddParams("CartonNO", ads[0]["carton_no"]);
                        //AddParams("MCartonNO", MCARTON_NO);
                        //AddParams("Mo_Number", ads[0]["mo_number"]);
                        //AddParams("Version", VERSION_CODE);
                        //AddParams("Part_No", ads[0]["key_part_no"]);
                    }
                    int i = 0;
                    foreach (var row in SnInformation.Data)
                    {
                        i = i + 1;
                        AddParams("SN" + i.ToString(), row["serial_number"].ToString());
                        AddParams("MSN" + i.ToString(), row["shipping_sn"].ToString());
                        AddParams("Serial_Number_" + i.ToString(), row["serial_number"].ToString());
                        AddParams("Shipping_SN_" + i.ToString(), row["shipping_sn"].ToString());
                      
                        mac = await FindMac(row["serial_number"].ToString(), MODEL_NAME);

                        AddParams("MAC" + i.ToString(), mac); // Hang ko co mac => MAC = '' 
                        
                        if ("D".Contains(MODEL_TYPE))
                        {
                            /// Need check
                        }
                        if ("E".Contains(MODEL_TYPE))
                        {
                            /// Need check
                        }
                    }
                    if ((i != 11) || (i < 11))
                    {
                        for (int k = i + 1; k <= 10; k++)
                        {
                            AddParams("SN" + k.ToString(), null);
                            AddParams("MSN" + k.ToString(), null);
                            AddParams("MAC" + k.ToString(), null);
                            if ("D".Contains(MODEL_TYPE))
                            {
                                AddParams("MTAMAC" + k.ToString(), null);
                                AddParams("DEC_MSN" + k.ToString(), null);
                            }
                            if ("E".Contains(MODEL_TYPE))
                            {
                                AddParams("MTAMAC" + k.ToString(), null);
                                AddParams("MTAMACC" + k.ToString(), null);
                            }
                        }
                    }

                    var _PackLotNO = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SELECT pmcc FROM SFISM4.R105 WHERE MO_NUMBER =:MO ",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                        {
                        new SfcParameter{ Name = "MO", Value = MO_NUMBER }
                        }
                    });

                    if (_PackLotNO.Data != null)
                    {
                        dynamic pmcc = _PackLotNO.Data;
                        AddParams("PackLotNO", pmcc["pmcc"]);
                    }
                    else
                    {
                        AddParams("PackLotNO", null);
                    }

                    var _count = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = "Select COUNT(SERIAL_NUMBER) Total from "+ tb107 + " where mcarton_no =: mcarton ",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                        {
                        new SfcParameter{ Name = "mcarton", Value = MCARTON_NO }
                        }
                    });

                    if (_count.Data != null)
                    {
                        dynamic _total = _count.Data;
                        Double tt = _total["total"];
                        AddParams("CustCartonQty", tt.ToString());
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageEnglish = string.Format("Get total carton error");
                        FrmMessage.MessageVietNam = string.Format("Không tìm thấy qty carton");
                        FrmMessage.ShowDialog();
                        return false;
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("Get information in "+ tb107 + " by carton " + CARTON + " error");
                    FrmMessage.MessageVietNam = string.Format("Không tìm thấy dữ liệu carton " + CARTON + " trong "+ tb107 );
                    FrmMessage.ShowDialog();
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = ex.Message.ToString();
                FrmMessage.MessageEnglish = "GET SN Information For CodeSoft ERROR ";
                FrmMessage.ShowDialog();
                return false;
            }
        }

        public async Task<bool> Reprint(string carton_no ,string tb107 ,string label_name ,string colName )
        {

            try
            {
                dtParams.Clear();
                if (!await getCartonInformation(carton_no, tb107, colName))
                {
                    return false;
                }

                if (!await PrintToCodeSoft(LabelName, false))
                {
                    return false;
                }

                return true;
           
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Reprint error";
                FrmMessage.MessageEnglish = ex.Message;
                FrmMessage.ShowDialog();
                return false;
            }
        }

        public async Task<string> FindMac(string sn, string MODEL)
        {
            string MAC="";
            var logInfo = new
            {
                DATA = sn,
                OPTION = "FINDMAC",
                MODEL_NAME = MODEL
            };
            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

            var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

            });
            dynamic ads = result.Data;
            string _RES = ads[0]["output"];
            _RESArray = _RES.Split('#');

            if (_RESArray[0] == "OK")
            {
                if (_RESArray[1] == "1")
                {
                    itemCheckMac.IsChecked = true;
                }
                else
                {
                    itemCheckMac.IsChecked = false;
                }
                MAC = _RESArray[2];
            }
            return MAC;
        }
        public async Task<Boolean> PrintToCodeSoft( string FileName , Boolean print)
        {
            try
            {
                itemLable = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "LB_Carton", 1);

                if ((item_LH_Carton.IsChecked == true) || (itemLable == 1))
                {
                    lbIndex = 1;
                }
                else if ((itemLHcartonMac.IsChecked == true) || (itemLable == 2))
                {
                    lbIndex = 2;
                }
                wipLabelMd5 = await this.getWipLabelMD5(LabelName, lbIndex); ;
                string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory;
                FilePath = _DirPath + FileName;



                if (File.Exists(FilePath))
                {
                    string str1 = this.CRC322(FilePath);
                    if (wipLabelMd5 != str1)
                    {
                        try
                        {
                           // LabApp = null;
                            File.Delete(FilePath);
                        }

                        catch (Exception ex)
                        {
                            List<Process> lstProcs = new List<Process>();
                            lstProcs = FileUtil.WhoIsLocking(FileName);

                            foreach (Process p in lstProcs)
                            {
                                try
                                {
                                    ProcessHandler.localProcessKill(p.ProcessName);
                                }
                                catch (Exception exc)
                                {
                                    string errorMessage = "Cannot kill process " + p.ProcessName + " Exception: " + exc.Message + Environment.NewLine + "Please Close program and try again";
                                    MessageError _sh = new MessageError();
                                    _sh.CustomFlag = true;
                                    _sh.MessageVietNam = "Không thể xóa đóng process { 0}. Lỗi ngoại lệ: { 1} Hãy đóng chương trình và thử lại ";
                                    _sh.MessageEnglish = errorMessage + ex.Message;
                                    _sh.ShowDialog();
                                    return false;
                                }
                            }
                            try
                            {
                                File.Delete(FilePath);
                            }
                            catch (Exception ex_e)
                            {
                                string errorMessage = "Cannot delete file " + FilePath + " Exception: " + ex_e.Message;
                                MessageError _sh = new MessageError();
                                _sh.CustomFlag = true;
                                _sh.MessageVietNam = string.Format("Không thể xóa file {0}. Lỗi ngoại lệ: {1}", FilePath, ex_e.Message);
                                _sh.MessageEnglish = string.Format("Cannot delete file {0}. Exception: {1}", FilePath, ex_e.Message);
                                _sh.ShowDialog();
                                return false;
                            }
                        }
                    }
                }
                //down file label
                UrlLabelFile = await GetUrlLabelFile();

                if (!File.Exists(FilePath))
                {
                    try
                    {
                        //LabApp = null;
                        WebClient wc = new WebClient();
                        wc.DownloadFile(UrlLabelFile + FileName, FilePath);

                        string str2 = this.CRC322(FilePath);
                        if (wipLabelMd5 != str2)
                        {
                            MessageError _sh = new MessageError();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "Label file Version error.\n\r File name=" + LabelName + ",\n\rpathIndex= " + lbIndex + " ,\n\rCurrentMD5=" + str2 + ",\n\rWipMD5=" + wipLabelMd5;
                            _sh.MessageEnglish = "MD5 khong chinh xac .\n\r MD5 he thong : " + wipLabelMd5 + ",\n\r MD5 file: " + str2;
                            _sh.ShowDialog();
                            return false;

                        }
                    }
                    catch (Exception exc)
                    {
                        if (exc.Message.Equals("The remote server returned an error: (404) Not Found."))
                        {
                            MessageError _sh = new MessageError();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "Không tìm thấy file label, gọi Labelroom kiểm tra.";
                            _sh.MessageEnglish = "Label file not found, call Labelroom check." + Environment.NewLine + "Url: " + Environment.NewLine + UrlLabelFile + LabelName;
                            _sh.ShowDialog();
                            return false;
                        }
                        else
                        {
                            MessageError _sh = new MessageError();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "Không tìm thấy file label, gọi Labelroom kiểm tra.";
                            _sh.MessageEnglish = "Can not download label file. Url: " + UrlLabelFile + exc.Message;
                            _sh.ShowDialog();
                            return false;
                        }
                    }
                }
                // xoa chuoi luu ten bien label 

                strParamName = null;
                bool _chkprint = await CallCodesoftToPrint(FileName, dtParams, wipLabelMd5, print);
                if (!_chkprint)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                showMessage("Print To CodeSoft have exception", ex.Message, true);
                return false; 
            }
        }
        private void ShowParam_Click(object sender, RoutedEventArgs e)
        {

            CSParam _sp = new CSParam();
            _sp.dataGridcsParam.DataContext = dtParams;
            if (_sp.dataGridcsParam.DataContext == null)
            {
                return;
            }
            _sp.urFile.Text = UrlLabelFile + LabelName;
            _sp.ShowDialog();
        }

        public async Task<Boolean> getCartonInformation(String CARTON, String tb107, String colName)
        {
            try
            {
                var logInfo = new
                {
                    DATA = CARTON, 
                    OPTION = "CartonInformation",
                    TABLE = tb107,
                    COLNAME = colName,
                    EMP = empNo
                };

                //Tranform it to Json object
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

                var result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.Pack_ctn_api_execute",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                });
                dynamic ads = result.Data;
                string _RES = ads[0]["output"];
                _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    LabelName = _RESArray[1].ToString();

                    strParamName = await getParamNameItem(LabelName);
                    if (strParamName.Substring(0, 2) != "OK")
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "Error";
                        FrmMessage.MessageEnglish = "Get param item label name: " + LabelName + " error !!";
                        FrmMessage.ShowDialog();
                        return false;
                    }
                    foreach (var rows in _RESArray[2].Split('|'))
                    {
                        AddParams(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "");
                    }
                    return true;
                }
                else
                {
                    showMessage("SFIS1.PACK_CTN_API_EXECUTE / CartonInformation:", _RESArray[2], true);
                    return false;
                }
            }
            catch (Exception ex)
            {
                showMessage(" SFIS1.PACK_CTN_API_EXECUTE / CartonInformation ", ex.Message, true);
                return false;
            }
        }


        private void VisibleLabel()
        {
            //ReprintForm _rep = new ReprintForm();
            string _param_Name = "";
            if (File.Exists(MainWindow.FilePath))
            {
                //ApplicationClass LabApp = new ApplicationClass();
                try
                {
                    if (LabApp == null)
                    {
                        try
                        {
                            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
                            {
                                process.Kill();
                            }
                        }
                        catch
                        {
                        }

                        LabApp = new LabelManager2.ApplicationClass();

                    }
                    LabApp.Documents.Open(MainWindow.FilePath, false);
                    Document doc = LabApp.ActiveDocument;
                    foreach (DataRow param in dtParams.Rows)
                    {
                        _param_Name = param["Name"].ToString();
                        try
                        {
                            doc.Variables.FormVariables.Item(_param_Name).Value = param["Value"].ToString();
                        }
                        catch
                        { }
                    }
                    doc.Application.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageError _sh = new MessageError();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Mở file label có lỗi ngoại lệ: " + ex.Message;
                    _sh.MessageEnglish = "Have exception when open label file: " + ex.Message;
                    _sh.ShowDialog();
                }
            }
            else if (File.Exists(FilePath))
            {
                try
                {
                    LabApp.Documents.Open(FilePath, false);
                    Document doc = LabApp.ActiveDocument;
                    doc.Application.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageError _sh = new MessageError();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Mở file label có lỗi ngoại lệ: " + ex.Message;
                    _sh.MessageEnglish = "Have exception when open label file: " + ex.Message;
                    _sh.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Can not find Label file!", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public async Task<bool> CallCodesoftToPrint( string LabelName, DataTable dtParams, string MD5 , Boolean Print )
        {
            try
            {
                string _pVareg = string.Empty;
                string _param_Name = "", LastError = "", strDate;
                try
                {
                    if (LabApp == null)
                    {
                        try
                        {
                            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
                            {
                                process.Kill();
                            }
                        }
                        catch
                        {

                        }

                        LabApp = new LabelManager2.ApplicationClass();
                    }
                    LabApp.Documents.Open(FilePath, false);
                    LabApp.ActiveDocument.ViewMode = enumViewMode.lppxViewModeName;
                    LabApp.ActiveDocument.ViewMode = enumViewMode.lppxViewModeValue;
                    doc = LabApp.ActiveDocument;
                    //doc.ViewMode = LabApp.ActiveDocument.ViewMode;
                    //doc.ViewMode = LabApp.ActiveDocument.WriteVariables;
                }
                catch (Exception ex)
                {
                    MessageError frmError = new MessageError();
                    frmError.CustomFlag = true;
                    frmError.MessageVietNam = "Open label have error";
                    frmError.MessageEnglish = ex.Message;
                    frmError.ShowDialog();
                    return false;
                }

                foreach (DataRow param in dtItemsLabel.Rows)
                {
                    _param_Name = param["Name"].ToString();
                    try
                    {
                        doc.Variables.FormVariables.Item(_param_Name).Value = param["Value"].ToString();
                    }
                    catch
                    {

                    }
                }
                
                strDate = System.DateTime.Now.ToString("yyyymmddHHmmss");

                strsql = "";

                foreach (DataRow param in dtParams.Rows)
                {
                    _param_Name = param["Name"].ToString();
                    try
                    {
                        doc.Variables.FormVariables.Item(_param_Name).Value = param["Value"].ToString();
                        strsql = strsql + "  INSERT INTO SFISM4.R_FQA_CHECKLABEL_T (LABEL_NAME,GROUP_NAME,PASS_DATE,UPD_DATE,EMP_NO,PASS_FLAG,CHECKSUM,MEMO) " +
                                                  " VALUES ('" + LabelName + "','C" + strDate + "',sysdate,null,'" + _param_Name + "','" + lbIndex + "','" + MD5 + "','" + param["Value"].ToString() + "') ; \n";
                    }
                    catch
                    { }
                }

                strsql = "BEGIN \n" + strsql + "END;";
                var _insert = _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = strsql
                });
                
                if ((!itemReprint.IsChecked) && (!_itemReprint))
                {
                    doc.PrintDocument(0);//Them vao de xu ly loi khi formula ko update gia tri
                    DataTable dt1 = new DataTable();
                    dt1.Columns.Add("VAR_NAME", typeof(string));
                    dt1.Columns.Add("VALUE", typeof(string));
                    dt1.Columns.Add("TYPE", typeof(string));

                    //Check PQE pass label file
                    string labeStatus = await this.getLabeStatus(LabelName, lbIndex);
                    if (labeStatus != "11")
                    {
                        MessageError frmError = new MessageError();
                        frmError.CustomFlag = true;
                        frmError.MessageVietNam = "Check PQE pass label file";
                        frmError.MessageEnglish = "Label file not confirmed!" + (labeStatus.Length == 2 && !(labeStatus.Substring(1, 1) != "1") ? "Need PQE Confirm" : " Need PE Confirm") + "\n\rCurrentFileName=" + LabelName + "\n\rMd5=" + wipLabelMd5 + "\n\rpathIndex=" + lbIndex + "\n\rLabel Flag=" + labeStatus + "\n\r";
                        frmError.ShowDialog();
                        return false;
                    }

                    //Check labeltracking values
                    try
                    {
                        int count1 = (int)((IDocument)doc).DocObjects.Barcodes.Count;
                        int count2 = (int)((IDocument)doc).DocObjects.Texts.Count;
                        int count3 = (int)((IDocument)doc).Variables.FormVariables.Count;
                        int count4 = (int)((IDocument)doc).Variables.FreeVariables.Count;
                        int count5 = (int)((IDocument)doc).Variables.Formulas.Count;
                        for (int index = 1; index <= count1; ++index)
                        {
                            string name = ((IDocument)doc).DocObjects.Barcodes.Item((object)index)._Name;
                            if (name != null)
                            {
                                ((IDocument)doc).DocObjects.Barcodes.Item((object)index);
                                string variableName = ((IDocument)doc).DocObjects.Barcodes.Item((object)index).VariableName;
                                string str3 = ((IDocument)doc).DocObjects.Barcodes.Item((object)index).Symbology.ToString().Replace("lppx", "").Trim();
                                string str4 = ((IDocument)doc).DocObjects.Barcodes.Item((object)index).Value;
                                dt1.Rows.Add((object)name, (object)str4, (object)str3);
                            }
                        }

                        for (int index = 1; index <= count2; ++index)
                        {
                            string name1 = ((IDocument)doc).DocObjects.Texts.Item((object)index)._Name;
                            if (name1 != null)
                            {
                                string variableName = ((IDocument)doc).DocObjects.Texts.Item((object)index).VariableName;
                                string str2 = ((IDocument)doc).DocObjects.Texts.Item((object)index).Value;
                                string name2 = ((IDocument)doc).DocObjects.Texts.Item((object)index).Font.Name;
                                dt1.Rows.Add((object)name1, (object)str2, (object)name2);
                            }
                        }

                        //DataTable result = dt1;
                        DataTable dataTable1 = new DataTable();
                        DataTable pqeSetup = await this.getPQESetup(LabelName, lbIndex);
                        int count6 = dt1.Rows.Count;
                        int count7 = pqeSetup.Rows.Count;
                        int num = int.Parse(await this.getLabelSetupPercent());
                        if ((int)Math.Floor((double)count7 / (double)count6 * 100.0) < num)
                        {
                            DataTable dataTable2 = this.DatatableMinus(dt1, pqeSetup, "VAR_NAME");
                            LastError = LastError + "\n\rPQE setup label not enough " + num.ToString() + "%.\n\rCall PQE continue setup on label tracking! Label name:" + LabelName + "\n\rList Object not setup:\n\r";
                            foreach (DataRow row in (InternalDataCollectionBase)dataTable2.Rows)
                                LastError = LastError + row["VAR_NAME"].ToString() + "\n\r";
                            MessageError _sh = new MessageError();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "";
                            _sh.MessageEnglish = LastError;
                            _sh.ShowDialog();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageError _sh = new MessageError();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "";
                        _sh.MessageEnglish = ex.ToString();
                        _sh.ShowDialog();
                        return false;
                    }

                    DataTable dataTable3 = new DataTable("PQECONFIG");
                    strsql = "SELECT VAR_NAME, VAR_REG, FLAG   FROM SFIS1.C_LABEL_CONFIG_T  WHERE VAR_REG NOT LIKE '@%' AND LABEL_NAME ='" + LabelName + "' AND PATH_NAME = " + lbIndex + " UNION SELECT A.VAR_NAME, B.REG_EXP, B.MEMO   FROM SFIS1.C_LABEL_CONFIG_T A, SFIS1.C_LABEL_REGEXP_T B  WHERE   A.LABEL_NAME ='" + LabelName + "' AND A.PATH_NAME = " + lbIndex + "   AND A.VAR_REG LIKE '@%'    AND A.VAR_REG = '@' || B.REG_NAME";
                    var result = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    DataTable dataTable4 = new DataTable();
                    Resource.ResouceClass rc = new Resource.ResouceClass();
                    if (result.Data != null)
                    {
                        dataTable4 = rc.ToDataTable<LabelConfigNEW>(result.Data.ToListObject<LabelConfigNEW>());
                    }
                    //result1 = dataTable4;
                    
                    string _obName = "";
                    string _obValue = "";
                    string _obtype = "";
                    string _pVarname = "";
                    string _pFlag = "";
                    LastError = "";
                    _pVareg = "";
                    try
                    {
                        foreach (DataRow row1 in (InternalDataCollectionBase)dt1.Rows)
                        {
                            _obName = row1["VAR_NAME"].ToString().Trim();
                            _obValue =  row1["VALUE"].ToString().Trim();
                            _obtype = row1["TYPE"].ToString().Trim();
                            if (_obValue.StartsWith("#"))
                            {
                                MessageError frmError = new MessageError();
                                frmError.CustomFlag = true;
                                frmError.MessageVietNam = "Error first character,can not start with character '#' ";
                                frmError.MessageEnglish = "Lỗi ký tự đầu tiên,không thẻ bắt đầu với ký tự '#'" ;
                                frmError.ShowDialog();
                                return false;

                            }
                            foreach (DataRow row2 in (InternalDataCollectionBase)dataTable4.Rows)
                            {
                                _pVarname = row2["VAR_NAME"].ToString();
                                _pVareg = row2["VAR_REG"].ToString();
                                _pFlag = row2["FLAG"].ToString();
                                if (_obName.Equals(_pVarname) && _pVareg.Substring(0, 1) != "=")
                                {
                                    if (_pFlag == "LINKBARCODE")
                                    {
                                        var _datarow = dt1.AsEnumerable().Where(x => x.Field<string>("VAR_NAME") == _pVareg).FirstOrDefault();
                                        string _linkvalue = _datarow["VALUE"].ToString();
                                        if (_obValue != _linkvalue)
                                            LastError = LastError + "[ERRO]" + _obName + ":" + _obValue + "," + _linkvalue + "," + _pFlag + "," + _pVareg + "\n\r";
                                    }
                                    else
                                    {
                                        string pattern = "^" + _pVareg + "$";
                                        if (!new Regex(pattern, RegexOptions.Multiline).GetMatch(_obValue))
                                            LastError = LastError + "[ERRO]" + _obName + ":" + _obValue + "," + pattern + "," + _pFlag + "\n\r";
                                    }
                                }
                                else if (_obName.Equals(_pVarname) && _pVareg.Substring(0, 1) == "=" && _obtype != "")
                                {
                                    if (_obName.Substring(0, 4) == "Barc" && _pVarname.Substring(0, 4) == "Barc")
                                    {
                                        if (_pVareg.Replace("=", "") != _obtype)
                                            LastError = LastError + "[BARCODE ERROR]" + _obName + ":" + _obValue + "," + _obtype + "," + _pFlag + "\n\r";
                                    }
                                    else if (_obName.Substring(0, 4) == "Text" && _pVarname.Substring(0, 4) == "Text" && _pVareg.Replace("=", "") != _obtype)
                                        LastError = LastError + "[FONT ERROR]" + _obName + ":" + _obValue + "," + _obtype + "," + _pFlag + "\n\r";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LastError = ex.Message;
                    }

                    if (LastError != "")
                    {
                        MessageError _sh = new MessageError();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "Label ";
                        _sh.MessageEnglish = LastError;
                        _sh.ShowDialog();
                        return false;
                    }

                }

                _TxtQtyLable = MES.OpINI.IniUtil.ReadINI(strINIPath, "CLabelQTY", "Default", "1");

                //Get label QTY
                try
                {
                    strsql = "select to_number(ATTRIBUTE_DESC1) from SFIS1.C_MODEL_ATTR_CONFIG_T where " +
                        "ATTRIBUTE_NAME = 'LABEL_QTY' and ATTRIBUTE_VALUE = 'PACK_CTN' and TYPE_VALUE = '" + lblModelName.Content + "' " +
                        "and version = '" + lblVersionCode.Content + "' and MODEL_NAME = 'CONFIRM'";
                    var SQL = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (SQL.Result == "ERROR")
                    {
                        MessageError _sh = new MessageError();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "TE Label thiết lập sai số lượng in label: " + SQL.Message;
                        _sh.MessageEnglish = "TE Label setup label qty wrong: " + SQL.Message;
                        _sh.ShowDialog();
                        return false;
                    }
                    else
                    {
                        if (SQL.Data != null)
                        {
                            _TxtQtyLable = SQL.Data["to_number(attribute_desc1)"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageError _sh = new MessageError();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "GET label QTY ERROR: " + ex.Message;
                    _sh.MessageEnglish = "GET label QTY ERROR: " + ex.Message;
                    _sh.ShowDialog();
                    return false;
                }
                
                doc.PrintDocument(Int32.Parse(_TxtQtyLable));
                return true;
            }
            catch (Exception ex)
            {
                MessageError _sh = new MessageError();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = "Phát sinh lỗi khi in label: " + ex.Message;
                _sh.MessageEnglish = "Have exceptions when pint document: " + ex.Message;
                _sh.ShowDialog();
                return false;
            }
        }
        public async Task<string> getWipLabelMD5(string LabelName, int PathIndex)
        {

            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "select md5 from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex,
                SfcCommandType = SfcCommandType.Text
            });
            string _md5 = "NULL";
            if (result.Data != null)
            {
                _md5 = result.Data["md5"].ToString();
            }
            return _md5;
        }
        public async Task<string> getLabeStatus(string LabelName, int PathIndex)
        {
            string sql = PathIndex == 6 ? "select NVL(PD,'0')*10+NVL(IPQC,'0') bb from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex.ToString() : "select 1*10+NVL(IPQC,'0') bb from sfis1.c_label_wip_t where UPPER(label_name)='" + LabelName.ToUpper().Trim() + "' and path_index=" + PathIndex.ToString();
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null)
            {
                return result.Data["bb"].ToString();
            }
            else
            {
                return "NULL";
            }
        }
        public async Task<DataTable> getPQESetup(string LabelName, int PathIndex)
        {
            var result = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT DISTINCT VAR_NAME FROM SFIS1.C_LABEL_CONFIG_T WHERE UPPER(LABEL_NAME)='" + LabelName.ToUpper().Trim() + "' AND PATH_NAME=" + PathIndex,
                SfcCommandType = SfcCommandType.Text
            });
            DataTable dt = new DataTable();
            Resource.ResouceClass rc = new Resource.ResouceClass();
            if (result.Data != null)
            {
                dt = rc.ToDataTable<Resource.LabelConfig>(result.Data.ToListObject<Resource.LabelConfig>());
            }
            return dt;
        }
        public async Task<string> getLabelSetupPercent()
        {
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT NVL(VR_VALUE,0) VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LABELTRACKING'",
                SfcCommandType = SfcCommandType.Text
            });
            return result.Data["vr_value"].ToString();
        }
        public DataTable DatatableMinus(DataTable dt1, DataTable dt2, string field) => dt1.AsEnumerable().GroupJoin((IEnumerable<DataRow>)dt2.AsEnumerable(), (Func<DataRow, string>)(a => a[field].ToString()), (Func<DataRow, string>)(b => b[field].ToString()), (a, g) => new
        {
            a = a,
            g = g
        }).Where(_param1 => _param1.g.Count<DataRow>() == 0).Select(_param1 => _param1.a).CopyToDataTable<DataRow>();
        public string CRC322(string FilePath)
        {
            uint[] numArray = new uint[256]
            {
                0U,
                1996959894U,
                3993919788U,
                2567524794U,
                124634137U,
                1886057615U,
                3915621685U,
                2657392035U,
                249268274U,
                2044508324U,
                3772115230U,
                2547177864U,
                162941995U,
                2125561021U,
                3887607047U,
                2428444049U,
                498536548U,
                1789927666U,
                4089016648U,
                2227061214U,
                450548861U,
                1843258603U,
                4107580753U,
                2211677639U,
                325883990U,
                1684777152U,
                4251122042U,
                2321926636U,
                335633487U,
                1661365465U,
                4195302755U,
                2366115317U,
                997073096U,
                1281953886U,
                3579855332U,
                2724688242U,
                1006888145U,
                1258607687U,
                3524101629U,
                2768942443U,
                901097722U,
                1119000684U,
                3686517206U,
                2898065728U,
                853044451U,
                1172266101U,
                3705015759U,
                2882616665U,
                651767980U,
                1373503546U,
                3369554304U,
                3218104598U,
                565507253U,
                1454621731U,
                3485111705U,
                3099436303U,
                671266974U,
                1594198024U,
                3322730930U,
                2970347812U,
                795835527U,
                1483230225U,
                3244367275U,
                3060149565U,
                1994146192U,
                31158534U,
                2563907772U,
                4023717930U,
                1907459465U,
                112637215U,
                2680153253U,
                3904427059U,
                2013776290U,
                251722036U,
                2517215374U,
                3775830040U,
                2137656763U,
                141376813U,
                2439277719U,
                3865271297U,
                1802195444U,
                476864866U,
                2238001368U,
                4066508878U,
                1812370925U,
                453092731U,
                2181625025U,
                4111451223U,
                1706088902U,
                314042704U,
                2344532202U,
                4240017532U,
                1658658271U,
                366619977U,
                2362670323U,
                4224994405U,
                1303535960U,
                984961486U,
                2747007092U,
                3569037538U,
                1256170817U,
                1037604311U,
                2765210733U,
                3554079995U,
                1131014506U,
                879679996U,
                2909243462U,
                3663771856U,
                1141124467U,
                855842277U,
                2852801631U,
                3708648649U,
                1342533948U,
                654459306U,
                3188396048U,
                3373015174U,
                1466479909U,
                544179635U,
                3110523913U,
                3462522015U,
                1591671054U,
                702138776U,
                2966460450U,
                3352799412U,
                1504918807U,
                783551873U,
                3082640443U,
                3233442989U,
                3988292384U,
                2596254646U,
                62317068U,
                1957810842U,
                3939845945U,
                2647816111U,
                81470997U,
                1943803523U,
                3814918930U,
                2489596804U,
                225274430U,
                2053790376U,
                3826175755U,
                2466906013U,
                167816743U,
                2097651377U,
                4027552580U,
                2265490386U,
                503444072U,
                1762050814U,
                4150417245U,
                2154129355U,
                426522225U,
                1852507879U,
                4275313526U,
                2312317920U,
                282753626U,
                1742555852U,
                4189708143U,
                2394877945U,
                397917763U,
                1622183637U,
                3604390888U,
                2714866558U,
                953729732U,
                1340076626U,
                3518719985U,
                2797360999U,
                1068828381U,
                1219638859U,
                3624741850U,
                2936675148U,
                906185462U,
                1090812512U,
                3747672003U,
                2825379669U,
                829329135U,
                1181335161U,
                3412177804U,
                3160834842U,
                628085408U,
                1382605366U,
                3423369109U,
                3138078467U,
                570562233U,
                1426400815U,
                3317316542U,
                2998733608U,
                733239954U,
                1555261956U,
                3268935591U,
                3050360625U,
                752459403U,
                1541320221U,
                2607071920U,
                3965973030U,
                1969922972U,
                40735498U,
                2617837225U,
                3943577151U,
                1913087877U,
                83908371U,
                2512341634U,
                3803740692U,
                2075208622U,
                213261112U,
                2463272603U,
                3855990285U,
                2094854071U,
                198958881U,
                2262029012U,
                4057260610U,
                1759359992U,
                534414190U,
                2176718541U,
                4139329115U,
                1873836001U,
                414664567U,
                2282248934U,
                4279200368U,
                1711684554U,
                285281116U,
                2405801727U,
                4167216745U,
                1634467795U,
                376229701U,
                2685067896U,
                3608007406U,
                1308918612U,
                956543938U,
                2808555105U,
                3495958263U,
                1231636301U,
                1047427035U,
                2932959818U,
                3654703836U,
                1088359270U,
                936918000U,
                2847714899U,
                3736837829U,
                1202900863U,
                817233897U,
                3183342108U,
                3401237130U,
                1404277552U,
                615818150U,
                3134207493U,
                3453421203U,
                1423857449U,
                601450431U,
                3009837614U,
                3294710456U,
                1567103746U,
                711928724U,
                3020668471U,
                3272380065U,
                1510334235U,
                755167117U
            };
            FileStream fileStream = (FileStream)null;
            byte[] buffer = (byte[])null;
            try
            {
                fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                fileStream.Read(buffer, 0, length);
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                fileStream?.Close();
            }
            uint num = uint.MaxValue;
            int length1 = buffer.Length;
            for (int index = 0; index < length1; ++index)
                num = numArray[((int)num ^ (int)buffer[index]) & (int)byte.MaxValue] ^ num >> 8;
            return (num ^ uint.MaxValue).ToString();
        }

        public void AddParams(string _name, string _value)
        {
             // doan nay dung db thuong , loc bot bien de in nhanh hon
            try
            {
                if (dtParams.Columns.Count == 0)
                {
                    dtParams.Columns.Add("Name");
                    dtParams.Columns.Add("Value");
                }
                dtParams.Rows.Add(new object[] { _name, _value });
                /*
                if (strParamName.Contains(_name))
                {
                    try
                    {
                        dtParams.Rows.Add(new object[] { _name, _value });
                    }
                    catch
                    {

                    }
                }*/

            }
            catch (Exception ex)
            {
                MessageBox.Show("Add value to table Param error : " + ex.Message,"Error" , MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } 
       
        }

        public void AddItemsLabel(string _name, string _value)
        {
            if (dtItemsLabel.Columns.Count == 0)
            {
                dtItemsLabel.Columns.Add("Name");
                dtItemsLabel.Columns.Add("Value");
            }

            try
            {
                dtItemsLabel.Rows.Add(new object[] { _name, _value });
            }
            catch
            {
            }
        }
        public async Task<string> GetUrlLabelFile()
        {
            itemLable = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PACK2", "LB_Carton", 1);

            if ((item_LH_Carton.IsChecked == true) || (itemLable == 1))
            {
                Z_LABEL = "Z_CARTON_LH";
            }
            else if ((itemLHcartonMac.IsChecked == true) || (itemLable == 2))
            {
                Z_LABEL = "Z_CARTONM_LH";
            }
            else if ((itemGLcarton.IsChecked == true) || (itemLable == 3))
            {
                Z_LABEL = "Z_CARTON_GL";
            }
            else if ((itemGLcartonMac.IsChecked == true) || (itemLable == 4))
            {
                Z_LABEL = "Z_CARTONM_GL";
            }
            else
            {
                item_LH_Carton.IsChecked = true;
                Z_LABEL = "Z_CARTON_LH";
            }

            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT MODEL_SERIAL || CUSTOMER  URL FROM SFIS1.C_MODEL_DESC_T  WHERE MODEL_NAME =:MODEL_NAME",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter{ Name = "MODEL_NAME", Value = Z_LABEL}  //PasswordBox.Password
                }
            });
            if (result.Data != null)
            {
                dynamic _ads = result.Data;
                string _RES = _ads["url"];
                return "http://" + _RES + "/";
            }
            else
            {
                return "";
            }

        }
        public void killprocess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
            {
                process.Kill();
            }
            //Process.GetCurrentProcess().Kill();
        }

        /// SNInformationForCodeSoft

        private async Task<bool> getSNinCarton()
        {
            try
            {
                //lvListSN.Items.Clear();

                string sql_str;
                if (itemScanMac2.IsChecked == true)
                {
                    sql_str = " SELECT SERIAL_NUMBER , MAC2 FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER IN ( select  SERIAL_NUMBER  from sfism4.r_wip_tracking_t "
                             + " WHERE CARTON_NO = '" + lblCartonNo.Content + "' ) ";
                }
                else if (itemScanTrayNo.IsChecked == true)
                {
                    sql_str = " SELECT  distinct TRAY_NO ,COUNT(*) QTY from sfism4.r_wip_tracking_t "
                           + " WHERE CARTON_NO = '" + lblCartonNo.Content + "'" +
                           " GROUP BY TRAY_NO ";
                }
                else if (itemScanMac.IsChecked == true)
                {
                    sql_str = " SELECT  distinct SERIAL_NUMBER ,SHIPPING_SN2 from sfism4.r_wip_tracking_t "
                            + " WHERE CARTON_NO = '" + lblCartonNo.Content + "' ";
                }
                else
                {
                    sql_str = " SELECT  distinct SERIAL_NUMBER ,NVL(SHIPPING_SN,'N/A') as SHIPPING_SN from sfism4.r_wip_tracking_t "
                            + " WHERE CARTON_NO = '" + lblCartonNo.Content + "' ";
                }
                var SnInCarton = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql_str,
                    SfcCommandType = SfcCommandType.Text
                });

                if (SnInCarton.Data != null)
                {
                    lblCount.Content = SnInCarton.Data.Count();
                    List<ListSN> items = new List<ListSN>();

                    foreach (var row in SnInCarton.Data)
                    {
                        if (itemScanMac2.IsChecked == true)
                        {
                            items.Add(new ListSN() { SN = row["serial_number"].ToString(), SHIPPING_SN = row["mac2"].ToString() });
                        }
                        else if (itemScanMac.IsChecked == true)
                        {
                            items.Add(new ListSN() { SN = row["serial_number"].ToString(), SHIPPING_SN = row["shipping_sn2"].ToString() });
                        }
                        else if (itemScanTrayNo.IsChecked == true)
                        {
                            items.Add(new ListSN() { SN = row["tray_no"].ToString(), SHIPPING_SN = row["qty"].ToString() });
                        }
                        else
                        {
                            items.Add(new ListSN() { SN = row["serial_number"].ToString(), SHIPPING_SN = row["shipping_sn"].ToString()});
                        }    
           
                    }
                    viewListSN.ItemsSource = items;
                    return true;
                    
                }
                else
                {
                    MessageError _sh = new MessageError();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Get sn in carton error: " + lblCartonNo.Content;
                    _sh.MessageEnglish =  "No data SN in carton: " + lblCartonNo.Content;
                    _sh.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError _sh = new MessageError();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = "Get sn in carton error: " + lblCartonNo.Content;
                _sh.MessageEnglish = ex.Message;
                _sh.ShowDialog();
                return false;
            }
        }
        
    }
}
