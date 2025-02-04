using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PACK_PALT;
using System.Net;
using MES.OpINI;
using System.Text.RegularExpressions;
using System.Data;
using PACK_PALT.Resource;
using PACK_PALT.Model;
using System.IO;
using LabelManager2;
using Newtonsoft.Json;
using PACK_PALT.ViewModel;
using System.Management;
using PACK_PALT_NEW;
using PACK_PALT_NEW.Model;

namespace PACK_PALT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _Password;
        public string Password { get { return _Password; } set { _Password = value; } }
        public ICommand PasswordChangedCommand { get; set; }
        public static SfcHttpClient _sfcHttpClient;
        public static string str_sql_query = "";
        public string str_sql_insert = "";

        public static string strINIPath = "C:\\Windows\\SFIS.ini";
        public string emp_name { get; set; }
        public static string macpc { get; set; }
        public static string G_sMyIP { get; set; }
        public static string txt_mcarton_no { get; set; }
        public string local_strIP;
        public static string G_sBuildDate, G_sPrgName, G_sLine_Name, M_sThisSection, My_Station, M_sThisGroup, M_thisresult, M_thismo,
                      M_ModelType, M_standardcount, G_sMo, G_sModel, G_sTA, G_sPartNo, G_sVersion_Code, CHECKMO46, CHECKMO45, CHECKDOA,
                        G_sCust_No, ADB_CUSTOMER, M_flag = "", tmp_pallet_code, G_sCust_Prefix, G_sCust_PNo_Next, G_sLabl_Name, G_sCUSTPartNo = "",
            MO_Status, ship_way = "", G_sCHECKTA, G_sCust_PartNo, Z_pallet_no, M_sThisEmpNo, Cust_Pallet, ctnList, PO_PALLETQTY, sLabelFileName;
        public int G_iNum;
        public bool M_bChangeLine, CHECK_MO_FLAG;
        public string loginInfor = "";
        public string loginApiUrl = "";
        public string loginDB = "";
        public string empNo = "";
        public string empPass = "";
        public string inputLogin = "";
        private string checkSum;
        public bool Result, Result_fintcarton, Result_checkdoamo, IsPackingByMoElseByModel, rma_flag, h_flag, h_flag1, MA400_flag, notfull;
        public int M_setcount, M_iPalletCount, csvt_plt_qty;
        public int M_currentcount = 0, Cust_Prefix_Len = 0, G_cartonnolength = 0, iPalletCapacity = 0, G_PALLET_QTY = 0;
        public static string str_GetPubMessage = "", query_sql = "", str_getCust_No, G_sCust_Pno_Last = "", G_sCustPalletLength = "", G_SCust_temp_PreFix = "", M_sLocation = "",
            G_sCust_Pno_Last_prefix = "", G_PALLET_QTY1 = "", sMCARTON_NO = "", iCurrNewCustPalt = "", NOINSERT117 = "", PMCC, s_pcount, PO_TARGET;
        public static DataTable dtParams = new DataTable();
        private bool _ChkMD5Flag = true;
        public static string UrlLabelFile = "";
        public static string LabelName = "";
        public static string FilePath = "";
        private string qtylabel = "";
        public static int int_label_qty = 1;

        public static string check_plot_run = "", check_n46 = "", check_n45 = "";
        public static bool zFlag = false;
        LicenseForm lsc;


        private string _appver = System.Windows.Forms.Application.ProductVersion;//lay version cua chuong trinh.

        public MainWindow()
        {

            InitializeComponent();

            string[] Args = Environment.GetCommandLineArgs();
            if (Args.Length == 1)
            {
                MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OK, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }
            string[] argsInfor = Regex.Split(inputLogin, @";");
            checkSum = argsInfor[0].ToString();
            loginApiUrl = argsInfor[1].ToString();
            loginDB = argsInfor[2].ToString();
            empNo = argsInfor[3].ToString();
            empPass = argsInfor[4].ToString();

            _sfcHttpClient = DBApi.sfcClient(loginApiUrl, loginDB);


            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            //    Environment.Exit(0);
            //}

            AccessAPI();

            //loadparameterini();

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        }
        private async void AccessAPI()
        {
            string temp, changeline, sPrinter;
            try
            {
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }


            G_sBuildDate = System.DateTime.Now.ToString("yyyy/MM/dd");
            //G_sPrgName = "Pack_PALT";
            G_sPrgName = System.Windows.Forms.Application.ProductName;
            G_sMyIP = localIP();
            G_sLine_Name = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "LINE");

            if (await CheckAppVersion(G_sPrgName, _appver) == false)
            {
                Environment.Exit(0);
            }
            Int32.TryParse(MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "QTYLABEL"), out int_label_qty);
            if (int_label_qty < 1) int_label_qty = 1;
            if (!string.IsNullOrWhiteSpace(MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "NUM")))
            {
                temp = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "NUM");
                if (temp == "1")
                    G_iNum = 1;
                else
                    G_iNum = 0;
            }
            changeline = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "IsChangeLine");
            if (changeline == "1")
            {
                M_bChangeLine = true;
            }
            else
            {
                M_bChangeLine = false;
            }
            if (MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "Pallet Type") == "Ambit")
            {
                item_ambit_pallet.IsChecked = true;
            }
            else if (MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "Pallet Type") == "Custom")
            {
                item_custom_pallet.IsChecked = true;
            }
            else if (MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "Pallet Type") == "A4page")
            {
                A4Label.IsChecked = true;
            }
            else if (MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "Pallet Type") == "Label3")
            {
                Pallet_Label3.IsChecked = true;
            }
            else if (MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "Pallet Type") == "Label4")
            {
                Pallet_label4.IsChecked = true;
            }
            else if (MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "Pallet Type") == "Label3_4")
            {
                Pallet_label3_4.IsChecked = true;
            }
            check_plot_run = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "CHECK_PILOT_RUN");
            //check pilot run
            if (check_plot_run == "1")
            {
                DOA1.IsChecked = true;
            }
            else
            {
                if (check_plot_run == "0")
                {
                    MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "CHECK_PILOT_RUN", "1");
                    DOA1.IsChecked = true;
                }
                else if (string.IsNullOrWhiteSpace(check_plot_run))
                {
                    MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "CHECK_PILOT_RUN", "1");
                    DOA1.IsChecked = true;
                }
            }
            check_n45 = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "CHECK_N45");
            if (check_n45 == "1")
            {
                N451.IsChecked = true;
            }
            else
            {
                if (check_n45 == "0")
                {
                    MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "CHECK_N45", "1");
                    N451.IsChecked = true;
                }
                else if (string.IsNullOrWhiteSpace(check_plot_run))
                {
                    MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "CHECK_N45", "1");
                    N451.IsChecked = true;
                }
            }
            check_n46 = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "CHECK_N46");
            if (check_n46 == "1")
            {
                N461.IsChecked = true;
            }
            else
            {
                if (check_n46 == "0")
                {
                    MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "CHECK_N46", "1");
                    N461.IsChecked = true;
                }
                else if (string.IsNullOrWhiteSpace(check_plot_run))
                {
                    MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "CHECK_N46", "1");
                    N461.IsChecked = true;
                }
            }
            // end check pilot run
            sPrinter = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "PRINTER");
            if (sPrinter == "PRINTER")
            {
                itemPrinter.IsChecked = true;
            }
            else if (sPrinter == "DONTPRINT")
            {
                itemDontPrint.IsChecked = true;
            }
            else
            {
                BarCodeItem.IsChecked = true;
            }
            CHECK_MO_FLAG = true;
            //Login();
            if (!await Login())
            {

                Environment.Exit(0);
            }
            loadC_Model_Desc_T();
            loadparameterini();
            macpc = GetHostMacAddress();//lay mac cua may tinh.
            version.Content = _appver;
            ip.Content = local_strIP;
            mac.Content = macpc;
        }
        #region lay mac cua may tinh
        public string GetHostMacAddress()
        {
            // su dung thu vien System.Management
            string result = "";
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection instances = managementClass.GetInstances();
            foreach (ManagementObject item in instances)
            {
                if (item["IPEnabled"].ToString() == "True")
                {
                    result = item["MacAddress"].ToString();
                }
            }
            return result;
        }
        #endregion
        private async Task<bool> CheckAppVersion(string AppName, string AppVersion)
        {
            var _result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT*FROM SFIS1.C_AMS_PATTERN_T WHERE AP_NAME=:app_name",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter{ Name = "app_name", Value = AppName }
                }
            });

            if (_result.Data != null)
            {
                string _AppVerAMS = _result.Data["ap_version"].ToString();
                if (!AppVersion.Equals(_AppVerAMS))
                {
                    string[] _localVersionArray = AppVersion.Split('.');
                    string[] _serverVersionArray = _AppVerAMS.Split('.');
                    if (_localVersionArray.Length == _serverVersionArray.Length)
                    {
                        for (int i = 0; i < _localVersionArray.Length; i++)
                        {
                            if (_localVersionArray[i].Length < _serverVersionArray[i].Length)
                                _localVersionArray[i] = _localVersionArray[i].PadLeft(_serverVersionArray[i].Length, '0');
                            else
                                if (_serverVersionArray[i].Length < _localVersionArray[i].Length)
                                _serverVersionArray[i] = _serverVersionArray[i].PadLeft(_localVersionArray[i].Length, '0');
                        }
                    }
                    else
                    {
                        if (AppVersion.Length < _AppVerAMS.Length) AppVersion = AppVersion.PadRight(_AppVerAMS.Length, '0');
                        if (_AppVerAMS.Length < AppVersion.Length) _AppVerAMS = _AppVerAMS.PadRight(AppVersion.Length, '0');
                        _localVersionArray = AppVersion.Split('.');
                        _serverVersionArray = _AppVerAMS.Split('.');
                    }
                    int local = Convert.ToInt16(string.Join("", _localVersionArray));// localVersion.Replace(".", ""));
                    int server = Convert.ToInt16(string.Join("", _serverVersionArray));//serverVersion.Replace(".", ""));
                    if (server > local)
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = string.Format("Chương trình {0} đã có phiên bản mới. Vui lòng cập nhật bằng SFIS_AMS. Phiên bản mới: {1} - Phiên bản hiện tại: {2}. Nhập 9999 để đóng thông báo.", AppName, _AppVerAMS, AppVersion);
                        _sh.MessageEnglish = string.Format("Program {0} have new version. Please upgrade by SFIS_AMS. New version: {1} - Current version: {2}.  Input 9999 to close message box.", AppName, _AppVerAMS, AppVersion);
                        _sh.ShowDialog();
                        return false;
                    }
                }
            }
            else
            {
                //ShowMessageForm _sh = new ShowMessageForm();
                //_sh.CustomFlag = true;
                //_sh.MessageVietNam = string.Format("Chương trình {0} không có trên hệ thống SFIS_AMS. Vui lòng liên hệ IT để xử lí! Nhập 9999 để đóng thông báo.", AppName);
                //_sh.MessageEnglish = string.Format("Program {0} not found on the SFIS_AMS system. Please find IT to resolve! Input 9999 to close message box.", AppName);
                //_sh.ShowDialog();
                //return false;
            }
            return true;
        }

        private void Pallet_label3_4_Click(object sender, RoutedEventArgs e)
        {
            item_ambit_pallet.IsChecked = false;
            item_custom_pallet.IsChecked = false;
            Pallet_Label3.IsChecked = false;
            Pallet_label4.IsChecked = false;
            Pallet_label3_4.IsChecked = true;
            A4Label.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "Label3_4");
        }

        private void Cut_Barcode_Click(object sender, RoutedEventArgs e)
        {
            Cut_BarcodeForm cutbarcodefrom = new PACK_PALT_NEW.Cut_BarcodeForm(this, _sfcHttpClient);
            cutbarcodefrom.ShowDialog();
        }

        public string localIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    local_strIP = ip.ToString();
                }
            }

            return local_strIP;
        }
        public async Task<bool> Login()
        {

            var _data = new
            {
                TYPE = "CHECKEMP",
                PRG_NAME = "PACK_PALT",
                EMP_PASS = empPass,
                FUN = "LOGIN"
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    PasswordBox.Visibility = Visibility.Collapsed;
                    EmpName.Text = _RESArray[1];
                    EmpName.Visibility = Visibility.Visible;
                    EmpName.IsEnabled = false;
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.httpclient = _sfcHttpClient;
                        _sh.errorcode = _RESArray[1];
                        _sh.ShowDialog();
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.httpclient = _sfcHttpClient;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error:";
                        _sh.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions:";
                _sh.ShowDialog();
                return false;
            }
        }
        private async void loadC_Model_Desc_T()
        {
            string sql_str = "Select * " +
                " From Sfis1.C_Model_Desc_T " +
                " Where Model_Name='SFISSITE'";
            var query_select = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_select.Data != null)
            {
                M_sLocation = query_select.Data["model_serial"].ToString().Trim();
                if (M_sLocation.Length != 1 || string.IsNullOrWhiteSpace(M_sLocation))
                {
                    MessageBox.Show(GetPubMessage("00156"));
                    Label_error.Content = GetPubMessage("00156");
                    Input_Carton.IsEnabled = false;
                    PasswordBox.Visibility = Visibility.Visible;
                    EmpName.Visibility = Visibility.Collapsed;
                    PasswordBox.Focus();
                    PasswordBox.SelectAll();
                    return;
                }

            }
            else
            {
                MessageBox.Show(GetPubMessage("00155"));
                Label_error.Content = GetPubMessage("00155");
                Input_Carton.IsEnabled = false;
                PasswordBox.Visibility = Visibility.Visible;
                EmpName.Visibility = Visibility.Collapsed;
                PasswordBox.Focus();
                PasswordBox.SelectAll();
                return;
            }
        }
        public async void loadparameterini()
        {
            try
            {

                //await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
                string sql = "select * from sfis1.c_parameter_ini where prg_name = '" + G_sPrgName + "'  and vr_class =  '" + G_sMyIP + "' and rownum=1 ";
                var result_parameterini = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text

                });
                if (result_parameterini.Data != null)
                {
                    //foreach (var row in result_parameterini.Data)
                    //{
                    //    M_sThisSection = row["vr_item"].ToString();
                    //    M_sThisGroup = row["vr_name"].ToString();
                    //    My_Station = row["vr_value"].ToString();
                    //}


                    M_sThisSection = result_parameterini.Data["vr_item"].ToString();
                    M_sThisGroup = result_parameterini.Data["vr_name"].ToString();
                    My_Station = result_parameterini.Data["vr_value"].ToString();
                    Input_Carton.Focus();
                }
                else
                {
                    //MessageBox.Show("Please Setup Station", "WARRING", MessageBoxButton.OK, MessageBoxImage.Stop);
                    loadformsetupstation();
                }

                txt_name_station_and_line.Text = M_sThisGroup + " : " + G_sLine_Name;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void loadformsetupstation()
        {
            Setup_Station ssetupstation = new PACK_PALT.Setup_Station(this, _sfcHttpClient);
            ssetupstation.ShowDialog();
        }

        private void Input_Carton_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //string tmp_str, CheckMacPrefix, CheckBomAndOSFW, CheckFcd, ModelCheckMacPrefix, ModelCheckBomAndOSFW, ModelCheckFcd;
                string model_serial = "";
                Label_error.Content = "";
                string Place_flag = "", datenow = "";
                txt_mcarton_no = Input_Carton.Text;
                Input_Carton.Text = "";
                if (string.IsNullOrWhiteSpace(txt_mcarton_no)) return;
                if (string.IsNullOrWhiteSpace(G_sLine_Name) || string.IsNullOrWhiteSpace(M_sThisGroup))
                {
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.CustomFlag = true;
                    _smf.MessageEnglish = "PELASE CHOOE LINE OR STATION";
                    _smf.MessageVietNam = "VUI LONG CHON LINE HOAC TRAM";
                    _smf.ShowDialog();
                    Input_Carton.Text = "";
                    return;
                }
                txt_mcarton_no = txt_mcarton_no.ToUpper();
                if (Input_2D.IsChecked || txt_mcarton_no.Contains(":"))
                {
                    if (txt_mcarton_no.Contains(":"))
                    {
                        string[] arrListCarton = txt_mcarton_no.Split(new char[] { ':' });
                        txt_mcarton_no = arrListCarton[0].ToString();
                    }
                }

                if (Cut_Barcode.IsChecked == true)
                {
                    string val = "SN";
                    txt_mcarton_no = getInputdataWithINI(txt_mcarton_no, val);
                }

                //if(txt_mcarton_no.Length>30 && txt_mcarton_no.Contains(@"%"))
                //{
                //    txt_mcarton_no = txt_mcarton_no.Substring(0, 11);
                //}

                if (ChkInput_data_valid(txt_mcarton_no))
                {
                    Label_error.Content = "Input data is invalid-->" + txt_mcarton_no;
                    return;
                }
                if (!getMcarton_no())
                {
                    MessageBox.Show("MCARTON/CARTON không tồn tại. Xem lại dữ liệu đã sảo?", "WARRING", MessageBoxButton.OK, MessageBoxImage.Stop);
                    Input_Carton.SelectAll();
                    return;
                }

                str_sql_query = "SELECT * FROM SFIS1.C_MODEL_DESC_T "
                              + " WHERE MODEL_NAME =(SELECT distinct(MODEL_NAME) FROM SFISM4.R_WIP_TRACKING_T "
                              + " WHERE MCARTON_NO= '" + txt_mcarton_no + "' "
                              + " OR  CARTON_NO='" + txt_mcarton_no + "' )";
                try
                {
                    var result_query_MODEL_SERIAL = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (result_query_MODEL_SERIAL.Data != null)  ///(result_query_MODEL_SERIAL.Data != null)
                    {
                        model_serial = result_query_MODEL_SERIAL.Data["model_serial"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("MCARTON NOT IN DB. PLEASE! CHECK MCARTON_NO", "WARRING", MessageBoxButton.OK, MessageBoxImage.Stop);
                        Input_Carton.SelectAll();
                        return;
                    }


                }
                catch (Exception ex)
                {
                    return;

                }
                if (checkdb() == false)
                {
                    return;
                }
                //await checkweight(Input_Carton.Text);
                //if (await checkweight(txt_mcarton_no) == false)
                //{
                //    return;
                //}
                try
                {
                    if (FindCarton() == false)
                        return;
                }
                catch (Exception ex)
                {
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.CustomFlag = true;
                    _smf.MessageEnglish = ex.Message;
                    _smf.MessageVietNam = "FindCarton error";
                    _smf.ShowDialog();
                    txt_mcarton_no = "";
                    return;
                }


                //if(await Check_C_ModelType(M_flag))
                if (listbCarton.Items.Count == 0)
                {
                    query_sql = "select * "
                        + " from sfism4.r_wip_tracking_t "
                        + " where (MCARTON_NO = '" + txt_mcarton_no + "' or CARTON_NO = '" + txt_mcarton_no + "'  ) AND IN_STATION_TIME BETWEEN TO_DATE('0601', 'MMDD')AND TO_DATE('0930', 'MMDD')";

                    var query_date = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                    {
                        CommandText = query_sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_date.Data != null)
                    {
                        // MessageBox.Show("xac nhan san pham gan the nhiet do chua");
                    }
                }

                if (M_flag.IndexOf("098") > 0)
                {
                    if (h_flag == false)
                        return;
                }

                MO_Status = "";

                //if(await CheckMo()==false)
                //{

                //    Input_Carton.Focus();
                //    Input_Carton.SelectAll();
                //    txt_mcarton_no = "";
                //    return;
                //}
                if (!string.IsNullOrWhiteSpace(editMO.Text))
                {
                    if (editMO.Text != G_sMo)
                    {
                        if (IsPackingByMoElseByModel)
                        {
                            ShowMessageForm _smf = new ShowMessageForm();
                            _smf.httpclient = _sfcHttpClient;
                            _smf.errorcode = "00002";
                            _smf.ShowDialog();

                            Label_error.Content = GetPubMessage("00002");
                            Input_Carton.SelectAll();
                            Input_Carton.Focus();
                            return;
                        }
                    }
                    if (Edit_model.Text != G_sModel)
                    {
                        if (IsPackingByMoElseByModel == false || Edit_Version.Text != G_sVersion_Code)
                        {
                            ShowMessageForm _smf = new ShowMessageForm();
                            _smf.httpclient = _sfcHttpClient;
                            _smf.errorcode = "80084";
                            _smf.ShowDialog();
                            Label_error.Content = GetPubMessage("80084");
                            Input_Carton.SelectAll();
                            Input_Carton.Focus();
                            return;
                        }
                    }

                }
                Place_flag = IsCPE3();
                if (Place_flag == "N")
                {
                    iPalletCapacity = getCapacitysec(G_sMo);

                    //tam thoi chua chuyen cai nay dong 2542-2562 in delphi
                    //if (h_flag1)
                    //{
                    //    if(ship_way=="N" || string.IsNullOrWhiteSpace(ship_way))
                    //    {

                    //    }
                    //}
                }
                else
                {
                    iPalletCapacity = getCapacity(G_sMo);
                }

                if (iPalletCapacity <= 0)
                {
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80085";
                    _smf.ShowDialog();
                    Label_error.Content = GetPubMessage("80085");
                    Input_Carton.Focus();
                    Input_Carton.SelectAll();
                    return;
                }

                //if (M_flag.IndexOf("D") > 0)
                //{
                datenow = getSysDate();
                datenow = datenow.Substring(2, 4);
                if ((UPDATENECCUSTSNCONFIG(datenow)) == false)
                {
                    MessageBox.Show("Model: " + G_sModel + " Pallet difine  error");
                    return;
                }
                //}
                //vi khong co thu tuc nen khong viet tiep 2021/08/04
                if (M_flag.IndexOf("R") > 0)
                {
                    query_sql = "select *  "
                        + " from sfis1.C_Pallet_T "
                        + " where Model_Name='" + G_sModel + "' AND CLOSE_FLAG='Pallet' ";

                    var query_select = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = query_sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (string.IsNullOrWhiteSpace(query_select.Data["pallet_no"].ToString()) ||
                       string.IsNullOrWhiteSpace(query_select.Data["cust_pallet_no"].ToString()) ||
                       query_select.Data.Count == 0)
                    {
                        var query_procedure = _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.FIND_ORDER_SN_RULE",
                            SfcCommandType = SfcCommandType.StoredProcedure
                        });
                    }
                }
                if (M_flag.IndexOf("336") > 0)
                {
                    if (Pa_PalletNo.Text != "")
                    {
                        query_sql = "select max(ssn2) as MAXSSN2 from SFISM4.R_CUSTSN_T where serial_number in  "
                                + " (select serial_number  from sfism4.r107 where imei = '" + Pa_PalletNo.Text + "' ";
                        var query_select = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                        {
                            CommandText = query_sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        string ssn2, c_ssn2;
                        ssn2 = query_select.Data["maxssn2"].ToString();
                        c_ssn2 = (Int32.Parse(ssn2.Substring(6, 5)) + 1).ToString("00000"); //// 00001

                        string dfdfd = "SELECT SSN2,MO_NUMBER,serial_number FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER IN(SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE (MCARTON_NO='" + txt_mcarton_no + "' or CARTON_NO='" + txt_mcarton_no + "') GROUP BY SERIAL_NUMBER) ORDER BY SSN2 ";
                        var query_ssn = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                        {
                            CommandText = dfdfd,
                            SfcCommandType = SfcCommandType.Text
                        });
                        ssn2 = query_ssn.Data["ssn2"].ToString();

                        if (Int32.Parse(ssn2.Substring(6, 5)) != Int32.Parse(c_ssn2))
                        {
                            MessageBox.Show("scan theo thu tu teho ssn2 Rakuten 5 so cuoi : " + c_ssn2);
                            Label_error.Content = "Check lai ma RAKUTEN ";
                            return;
                        }
                    }
                }
                //end vi khong co thu tuc nen khong viet tiep 2021/08/04

                //for netgear
                if (txt_mcarton_no.Substring(0, 7) == "FXCYYMM")
                {
                    MessageBox.Show("FXCYYMMDD prefix error! call SFIS!");
                    Label_error.Content = "FXCYYMMDD prefix error! call SFIS!";
                    return;
                }



                if (M_flag.IndexOf("I") > 0)
                {
                    sMCARTON_NO = txt_mcarton_no;
                    if ((checkI01Carton(txt_mcarton_no)) == false)
                    {
                        Input_Carton.Focus();
                        Input_Carton.SelectAll();
                        return;
                    }
                }
                if (listbCarton.Items.Count != 0 || M_flag.IndexOf("U") > 0)
                {
                    if (model_serial != "Cinterion")
                    {
                        if (G_sTA != G_sCHECKTA)
                        {
                            ShowMessageForm _smf = new ShowMessageForm();
                            _smf.httpclient = _sfcHttpClient;
                            _smf.errorcode = "80091";
                            _smf.ShowDialog();
                            return;
                        }
                    }
                }



                if (M_flag.IndexOf("T") > 0)
                {
                    if (checkDellCarton(txt_mcarton_no) == false) return;


                }
                if (M_flag.IndexOf("L") > 0)
                    if (checkcpeiiiCarton(txt_mcarton_no) == false) return;
                //tam thoi k chuyen sang c#
                //if(M_flag.IndexOf("202")>0)
                //    if (await checkMBDCarton(Input_Carton.Text) == false) return;


                Ed_PalletQty.Content = iPalletCapacity.ToString();
                Pa_PalletNo.Text = getPalletNo();//TAO PALLET NO//sitinh
                pa_custom_pallet.Content = G_sCust_PartNo;
                Z_pallet_no = Pa_PalletNo.Text;

                if (string.IsNullOrWhiteSpace(Pa_PalletNo.Text))
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageEnglish = "GET PALLET NO ERROR";
                    _sh.MessageVietNam = "TAO MA PALLET ERROR";
                    _sh.ShowDialog();
                    txt_mcarton_no = "";
                    return;
                }
                if (G_sCust_Prefix.ToUpper() == "AMBIT" || M_flag.IndexOf("007") > 0)
                {
                    pa_custom_pallet.Content = Pa_PalletNo.Text;
                }

                if (pa_custom_pallet.Content.ToString().Substring(0, 7) == "PLFXCYY")
                {
                    MessageBox.Show("PLFXCYYMMB prefix error! call SFIS!");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Pa_PalletNo.Text)) return;

                try
                {
                    if (Check_carton_with_Pallet(txt_mcarton_no, pa_custom_pallet.Content.ToString()) == false)// HAM CHECK CARTON VOI PALLET!
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = ex.Message;
                    _sh.MessageEnglish = "CALL Check_carton_with_Pallet ERROR";
                    _sh.ShowDialog();
                    return;
                }

                if (N461.IsChecked)
                {
                    if (FCheckPIRUN(Pa_PalletNo.Text, "") == false)
                    {
                        return;
                    }
                }
                if (N451.IsChecked)
                {
                    if (FCheckPIRUN45(Pa_PalletNo.Text, "") == false) return;
                }
                if (DOA1.IsChecked)
                {
                    if (FCheckDOA(Pa_PalletNo.Text, "") == false) return;
                }


                if (M_flag.IndexOf("K") > 0)
                {
                    if (CheckCtnSequenceInPalt(txt_mcarton_no, G_sMo) == false)
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = "80092";
                        _smf.ShowDialog();
                        Input_Carton.SelectAll();
                    }
                }

                if (M_flag.IndexOf("D") > 0 && G_sModel == "J01C021.00")
                {
                    if (checkctnsequence(txt_mcarton_no, G_sMo))
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = "80093";
                        _smf.ShowDialog();
                        txt_mcarton_no = "";
                        return;
                    }
                }


                if (M_flag.IndexOf("119") > 0)
                {
                    if (MA400_flag == false) return;
                    pa_custom_pallet.Content = iCurrNewCustPalt;
                }

                string MT = "";
                string MS = "";

                str_sql_query = "select * from sfis1.c_model_desc_t where model_name in "
                    + " (select model_name from sfism4.r107 where mcarton_no='" + txt_mcarton_no + "') ";
                var select_c_model_desc_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = str_sql_query,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_c_model_desc_t.Data != null)
                {
                    MT = select_c_model_desc_t.Data["model_type"].ToString();
                    MS = select_c_model_desc_t.Data["model_serial"].ToString();
                }
                if (IsPackingByMoElseByModel)
                {
                    str_sql_query = "select distinct mo_number from sfism4.r107 where mcarton_no='" + txt_mcarton_no + "' or carton_no='" + txt_mcarton_no + "'  or pallet_no='" + Pa_PalletNo.Text + "'";
                    var select_r107 = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_r107.Data != null)
                    {
                        int i = 0;
                        foreach (var row in select_r107.Data)
                        {
                            i++;
                        }
                        if (i > 1)
                        {
                            MessageBox.Show("MO DIFF With Pallet: " + Pa_PalletNo.Text, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }

                try
                {
                    //check license_no
                    if (MS == "NIC")
                    {
                        zFlag = false;
                        lsc = new PACK_PALT_NEW.LicenseForm(this, _sfcHttpClient);
                        lsc.ShowDialog();
                        lsc.Close();
                        if (!zFlag) return;
                    }

                    str_sql_query = "select a.* from sfism4.r107 a,SFIS1.C_MODEL_DESC_T b where a.mcarton_no = '" + txt_mcarton_no + "' and a.model_name = b.model_name and model_serial in('NIC', 'ECD', 'SUPERCAP') and not exists(SELECT * FROM SFIS1.C_MODEL_ATTR_CONFIG_T WHERE  ATTRIBUTE_NAME = 'NO_RFID_LABEL' and TYPE_VALUE = a.model_Name)";
                    //link RFID_label
                    //str_sql_query = "SELECT * FROM SFIS1.C_MODEL_ATTR_CONFIG_T WHERE  ATTRIBUTE_NAME = 'RFID_LABEL' AND TYPE_VALUE = (SELECT DISTINCT model_name FROM SFISM4.R107 WHERE  MCARTON_NO ='" + txt_mcarton_no + "') ";
                    var sl_config_rfid = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (sl_config_rfid.Data != null)
                    {
                        zFlag = false;
                        RFIDForm rfidform = new PACK_PALT_NEW.RFIDForm(this, _sfcHttpClient);
                        rfidform.ShowDialog();

                        //check lai data
                        str_sql_query = "SELECT * FROM SFISM4.R_SEC_LIC_LINK_T WHERE CARTON_NO ='" + txt_mcarton_no + "' AND RFID IS NOT NULL";
                        var sl_config_rfid1 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                        {
                            CommandText = str_sql_query,
                            SfcCommandType = SfcCommandType.Text
                        });

                        if (sl_config_rfid1.Data == null)
                        {
                            txt_mcarton_no = "";
                            return;
                        }
                        if (!zFlag) return;
                    }

                    if (updateR107() == false)  /// update pallet_no va imei
                    {
                        txt_mcarton_no = "";
                        return;
                    }
                    if (updateToDB() == false) /// update wip, r102
                    {
                        txt_mcarton_no = "";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80094";
                    _smf.ShowDialog();
                    return;
                }

                Pa_PalletCount.Content = M_iPalletCount.ToString();//SO LUONG CARTON DA SCAN
                bbtnClosePallet.IsEnabled = true;// NUT CLOSE PALLET

                editMO.Text = G_sMo;
                G_sCHECKTA = G_sTA;
                Edit_model.Text = G_sModel;
                Edit_Version.Text = G_sVersion_Code;
                editCustomer.Text = getCustomer();

                getCartonInPallet();

                if (M_flag.IndexOf("098") > 0 || Check_C_ModelType(M_flag))
                {
                    Pa_PalletNo.Text = G_sCust_PartNo;
                }
                if (M_flag.IndexOf("119") > 0)
                {
                    Pa_PalletNo.Text = iCurrNewCustPalt;
                }
                if (Int32.Parse(Pa_PalletCount.Content.ToString()) >= Int32.Parse(Ed_PalletQty.Content.ToString()))
                {
                    closePallet();

                    Label_error.Content = GetPubMessage("80095");
                    listbCarton.Items.Clear();
                }
                else
                {
                    if (IsPackingByMoElseByModel)
                    {
                        if (checkMoTarget())
                        {
                            closePallet();
                            Label_error.Content = GetPubMessage("80096");
                            listbCarton.Items.Clear();
                        }
                    }

                }


                Input_Carton.Focus();
            }
        }

        public bool Check_carton_with_Pallet(string carton, string pallet)
        {

            if (!string.IsNullOrEmpty(pallet))
            {
                var _data = new
                {
                    TYPE = "CHECK_CARTON_WITH_PALLET",
                    PRG_NAME = "PACK_PALT",
                    DB_TYPE = loginDB,
                    IMEI = pallet,
                    CARTON = carton
                };
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                try
                {
                    var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                        }
                    });

                    dynamic _ads = _result.Data;
                    string _RES = _ads[0]["output"];
                    string[] _RESArray = _RES.Split('#');
                    if (_RESArray[0] == "OK")
                    {
                        return true;
                    }
                    else
                    {
                        if (_RESArray[0] == "NG")
                        {
                            ShowMessageForm _sh = new ShowMessageForm();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = _RESArray[1];
                            _sh.MessageEnglish = _RESArray[2];
                            _sh.ShowDialog();
                            return false;
                        }
                        else
                        {
                            ShowMessageForm _sh = new ShowMessageForm();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = _RESArray[0];
                            _sh.MessageEnglish = "Excute procedure have error:CHECK_CARTON_WITH_PALLET";
                            _sh.ShowDialog();
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = ex.Message.ToString();
                    _sh.MessageEnglish = "Call procedure have exceptions:CHECK_CARTON_WITH_PALLET";
                    _sh.ShowDialog();
                    return false;
                }
            }
            return true;
        }

        //CLASS DONG PALLET BANG THU TUC
        public async void closePallet()
        {
            txt_mcarton_no = "";
            string temp1 = "0", temp2 = "0", temp3 = "0";
            if (A4Label.IsChecked)
                temp1 = "1";
            if (SSNLABEL1.IsChecked) temp2 = "1";
            if (CARTONLABEL1.IsChecked) temp3 = "1";

            var _data = new
            {
                TYPE = "CLOSEPALT",
                PRG_NAME = "PACK_PALT",
                IMEI = pa_custom_pallet.Content.ToString(),
                PALLETNO = Pa_PalletNo.Text,
                MODELNAME = Edit_model.Text.Trim(),
                A4LABEL = temp1,
                SSNLABEL1 = temp2,
                CARTONLABEL1 = temp3
            };
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    string slabelname = _RESArray[1];
                    LabelName = _RESArray[2];
                    string str_sql = "SELECT EMP_NO,MEMO FROM SFISM4.R_FQA_CHECKLABEL_T WHERE LABEL_NAME='" + slabelname + "' ";
                    var _result_param = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = str_sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    dtParams.Rows.Clear();
                    if (_result_param.Data != null)
                    {
                        foreach (var rows in _result_param.Data)
                        {
                            if (!string.IsNullOrEmpty(rows["memo"].ToString()))
                            {
                                AddParams(rows["emp_no"].ToString(), rows["memo"].ToString());
                            }
                        }
                    }
                    if (A4Label.IsChecked)
                        LabelName = "P_" + Edit_model.Text.Trim() + "_PMCTN_4.LAB";
                    else if (A4Label.IsChecked && SSNLABEL1.IsChecked)
                        LabelName = "P_" + Edit_model.Text.Trim() + "PMSN_4.LAB";
                    else if (A4Label.IsChecked && CARTONLABEL1.IsChecked)
                        LabelName = "P_" + Edit_model.Text.Trim() + "_PMCTN_4.LAB";
                    else if (Pallet_Label3.IsChecked)
                        LabelName = "P_" + Edit_model.Text.Trim() + "_3.LAB";
                    else if (Pallet_label4.IsChecked)
                        LabelName = "P_" + Edit_model.Text.Trim() + "_4.LAB";

                    rma_flag = await FindRmaMO(G_sMo);//check hang rma de lay ra file label rma.

                    if (rma_flag == true)
                    {
                        //LabelName = "P_" + Edit_model.Text.Trim() + "_RMA.lab";
                        LabelName = LabelName.ToUpper();
                        int charPos = LabelName.IndexOf(".LAB");
                        LabelName = LabelName.Replace(LabelName.Substring(charPos), "_RMA.LAB");
                    }

                    if (itemDontPrint.IsChecked == false)
                    {
                        Label_error.Content = GetPubMessage("80098");
                        if (A4Label.IsChecked && (SSNLABEL1.IsChecked || CARTONLABEL1.IsChecked))
                        {
                            //PrintForm.PageA4LabelPrint(pa_custom_pallet.Content.ToString());
                            PageA4LabelPrint();
                        }
                        else if (M_flag.IndexOf("202") > 0)
                        {
                            //PrintToCodeSoftForMBD();
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(LabelName))
                            {
                                LabelName = "P_" + Edit_model.Text.Trim() + ".LAB";
                            }

                            if (!itemDontPrint.IsChecked)
                            {
                                PrintToCodeSoft("P", Edit_model.Text.Trim());
                            }


                        }

                    }
                    ClearTextBox();
                    listbCarton.Items.Clear();
                    bbtnClosePallet.IsEnabled = false;

                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {

                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = false;
                        _sh.httpclient = _sfcHttpClient;
                        _sh.errorcode = _RESArray[2];
                        _sh.ShowDialog();
                        listbCarton.Items.Clear();
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error:closePallet";
                        _sh.ShowDialog();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions:closePallet";
                _sh.ShowDialog();
                return;
            }
        }


        private void N461_Click(object sender, RoutedEventArgs e)
        {
            if (N461.IsChecked)
            {
                N461.IsChecked = false;
            }
            else
            {
                N461.IsChecked = true;
            }
        }

        private void N451_Click(object sender, RoutedEventArgs e)
        {
            if (N451.IsChecked)
            {
                N451.IsChecked = false;
            }
            else
            {
                N451.IsChecked = true;
            }
        }

        private void DOA1_Click(object sender, RoutedEventArgs e)
        {
            //if (DOA1.IsChecked)
            //{
            //    DOA1.IsChecked = false;
            //}
            //else
            //{
            //    DOA1.IsChecked = true;
            //}
            DOA1.IsChecked = false;
        }

        private void Item_ambit_pallet_Click(object sender, RoutedEventArgs e)
        {
            item_ambit_pallet.IsChecked = true;
            item_custom_pallet.IsChecked = false;
            Pallet_Label3.IsChecked = false;
            Pallet_label4.IsChecked = false;
            A4Label.IsChecked = false;
            Pallet_label3_4.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "Ambit");
        }

        private void Item_custom_pallet_Click(object sender, RoutedEventArgs e)
        {
            item_ambit_pallet.IsChecked = false;
            item_custom_pallet.IsChecked = true;
            Pallet_Label3.IsChecked = false;
            Pallet_label4.IsChecked = false;
            A4Label.IsChecked = false;
            Pallet_label3_4.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "Custom");
        }

        private void Pallet_Label3_Click(object sender, RoutedEventArgs e)
        {
            item_ambit_pallet.IsChecked = false;
            item_custom_pallet.IsChecked = false;
            Pallet_Label3.IsChecked = true;
            Pallet_label4.IsChecked = false;
            A4Label.IsChecked = false;
            Pallet_label3_4.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "Label3");
        }

        private void Pallet_label4_Click(object sender, RoutedEventArgs e)
        {
            item_ambit_pallet.IsChecked = false;
            item_custom_pallet.IsChecked = false;
            Pallet_Label3.IsChecked = false;
            Pallet_label4.IsChecked = true;
            A4Label.IsChecked = false;
            Pallet_label3_4.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "Label4");
        }

        private void A4Label_Click(object sender, RoutedEventArgs e)
        {
            item_ambit_pallet.IsChecked = false;
            item_custom_pallet.IsChecked = false;
            Pallet_Label3.IsChecked = false;
            Pallet_label4.IsChecked = false;
            A4Label.IsChecked = true;
            Pallet_label3_4.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "A4page");
        }

        private void ClearTextBox()
        {
            editCustomer.Text = "";
            editMO.Text = "";
            Edit_model.Text = "";
            Edit_Version.Text = "";
            Input_Carton.Text = "";
            txt_mcarton_no = "";
            listbCarton.Items.Clear();
            //dtParams.Clear();

        }
        public void PrintToCodeSoft(string ParamKind, string model_name)
        {
            killprocess();
            string Loc_num = MES.OpINI.IniUtil.ReadINI(strINIPath, ParamKind + "LabelQTY", "Default");
            if (string.IsNullOrWhiteSpace(Loc_num))
            {
                Loc_num = "1";
            }

            //string _labelNameBak = ParamKind + "_" + model_name + ".BAK";
            string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\";
            FilePath = _DirPath + LabelName;
            //string FilePathBak = _DirPath + _labelNameBak;
            //remove old file label
            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch (Exception ex)
                {
                    List<Process> lstProcs = new List<Process>();
                    lstProcs = FileUtil.WhoIsLocking(LabelName);

                    foreach (Process p in lstProcs)
                    {
                        try
                        {
                            ProcessHandler.localProcessKill(p.ProcessName);
                        }
                        catch (Exception exc)
                        {
                            string errorMessage = "Cannot kill process " + p.ProcessName + " Exception: " + exc.Message + Environment.NewLine + "Please Close program and try again";
                            ShowMessageForm _sh = new ShowMessageForm();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "Không thể xóa đóng process { 0}. Lỗi ngoại lệ: { 1} Hãy đóng chương trình và thử lại ";
                            _sh.MessageEnglish = errorMessage + ex.Message;
                            _sh.ShowDialog();
                            return;

                        }
                    }
                    try
                    {
                        File.Delete(FilePath);
                    }
                    catch (Exception ex_e)
                    {
                        string errorMessage = "Cannot delete file " + FilePath + " Exception: " + ex_e.Message;
                        //MessageBoxHelper.Display(errorMessage);
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = string.Format("Không thể xóa file {0}. Lỗi ngoại lệ: {1}", FilePath, ex_e.Message);
                        _sh.MessageEnglish = string.Format("Cannot delete file {0}. Exception: {1}", FilePath, ex_e.Message);
                        _sh.ShowDialog();
                        return;
                    }

                }
            }
            //down file label
            if (!File.Exists(FilePath))
            {
                try
                {
                    WebClient wc = new WebClient();
                    UrlLabelFile = GetUrlLabelFile();
                    wc.DownloadFile(UrlLabelFile + LabelName, FilePath);
                }
                catch (Exception exc)
                {
                    if (exc.Message.Equals("The remote server returned an error: (404) Not Found."))
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "Không tìm thấy file label, gọi Labelroom kiểm tra.";
                        _sh.MessageEnglish = "Label file not found, call Labelroom check." + Environment.NewLine + "Url: " + Environment.NewLine + UrlLabelFile + LabelName;
                        _sh.ShowDialog();
                        return;
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "Không tìm thấy file label, gọi Labelroom kiểm tra.";
                        _sh.MessageEnglish = "Can not download label file. Url: " + UrlLabelFile + exc.Message;
                        _sh.ShowDialog();
                        return;
                    }
                }
            }

            //ApplicationClass LabApp = new ApplicationClass();
            //LabApp.Documents.Open(FilePath, false);
            //Document doc = LabApp.ActiveDocument;

            bool _chkprint = CallCodesoftToPrint(/*doc,*/ LabelName, dtParams);
            if (!_chkprint)
            {
                listbCarton.Items.Clear();
                return;
            }
            listbCarton.Items.Clear();
        }
        public bool CallCodesoftToPrint(/*Document doctemp,*/ string LabelName, DataTable dtParams)
        {
            string _param_Name = "";

            try
            {
                LabelTracking _lbt = new LabelTracking();
                _lbt.sfcHttpClient = _sfcHttpClient;
                ApplicationClass LabApp = new ApplicationClass();
                LabApp.Documents.Open(FilePath, false);
                Document doctemp = LabApp.ActiveDocument;
                //Check MD5 of label file
                if (_ChkMD5Flag)
                {
                    if (!_lbt.doMD5Label(LabelName, 4, true))
                    {
                        return false;
                    }
                    _ChkMD5Flag = false;
                }

                // Set value into label file
                foreach (DataRow param in dtParams.Rows)
                {
                    _param_Name = param["Name"].ToString();
                    try
                    {
                        doctemp.Variables.FormVariables.Item(_param_Name).CounterUse = 0;
                        doctemp.Variables.FormVariables.Item(_param_Name).Value = param["Value"].ToString();
                    }
                    catch
                    { }

                }

                // Get value of label items to compare with label tracking
                DataTable dtbVarName = new DataTable();
                dtbVarName.Columns.Add("VAR_NAME", typeof(string));
                dtbVarName.Columns.Add("VALUE", typeof(string));
                dtbVarName.Columns.Add("TYPE", typeof(string));
                int TotalBarcode = doctemp.DocObjects.Barcodes.Count;
                int TotalText = doctemp.DocObjects.Texts.Count;
                int TotalVar = doctemp.Variables.FormVariables.Count;
                int TotalFreeVar = doctemp.Variables.FreeVariables.Count;
                int TotalFomula = doctemp.Variables.Formulas.Count;

                for (int i = 1; i <= TotalText; i++)
                {
                    var _name = doctemp.DocObjects.Texts.Item(i).Name;
                    if (_name != null)
                    {
                        var _var = doctemp.DocObjects.Texts.Item(i).VariableName;
                        var _data = doctemp.DocObjects.Texts.Item(i).Value;
                        var _font = doctemp.DocObjects.Texts.Item(i).Font.Name;
                        dtbVarName.Rows.Add(new object[] { _name, _data, _font });
                    }
                }

                for (int i = 1; i <= TotalBarcode; i++)
                {
                    var _name = doctemp.DocObjects.Barcodes.Item(i).Name;
                    if (_name != null)
                    {
                        var _var = doctemp.DocObjects.Barcodes.Item(i).VariableName;
                        var _bar = doctemp.DocObjects.Barcodes.Item(i).Symbology.ToString().Replace("lppx", "").Trim();
                        var _data = doctemp.DocObjects.Barcodes.Item(i).Value;
                        dtbVarName.Rows.Add(new object[] { _name, _data, _bar });
                    }
                }
                //Call label tracking

                try
                {
                    doctemp.Save();
                    doctemp.Close();
                    LabApp.Documents.Open(FilePath, false);
                    doctemp = LabApp.ActiveDocument;
                    if (!_lbt.doLabelTracking_AddParamValue(LabelName, 4, dtParams, doctemp, dtbVarName, true))
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message;
                    _sh.MessageEnglish = "Check label tracking have exception: " + ex.Message;
                    _sh.ShowDialog();
                    LabApp.Documents.CloseAll();
                    LabApp.Quit();
                    doctemp = null;
                    LabApp = null;
                    return false;
                }

                doctemp.PrintDocument(int_label_qty);
                doctemp.FormFeed();
                LabApp.Documents.CloseAll();
                LabApp.Quit();
                doctemp = null;
                LabApp = null;
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = "Phát sinh lỗi khi in label: " + ex.Message;
                _sh.MessageEnglish = "Have exceptions when pint document: " + ex.Message;
                _sh.ShowDialog();
                return false;
            }
            return true;
        }

        #region print A4 LABEL
        public void PageA4LabelPrint()
        {
            int pageSum = 0;
            bool PRINTOTHERFLAG;
            string tmpssn = "";
            MainWindow mw = new MainWindow();
            var getmainVM = mw.DataContext as MainViewModel;
            if (!string.IsNullOrWhiteSpace(MES.OpINI.IniUtil.ReadINI(strINIPath, "PALLETPRINT", "PAGEQTY")))
                pageSum = Int32.Parse(MES.OpINI.IniUtil.ReadINI(strINIPath, "PALLETPRINT", "PAGEQTY"));


            // mtable = "SFISM4.R_WIP_TRACKING_T";

            string query_sql = "SELECT COUNT(*) QTY FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + pa_custom_pallet.Content.ToString() + "'";

            var _result = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            int sQTY = Int32.Parse(_result.Data["qty"].ToString());
            query_sql = "ELECT COUNT(DISTINCT MCARTON_NO) QTY  FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + pa_custom_pallet.Content.ToString() + "'";
            var _result_count = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            int sQTY_SN = Int32.Parse(_result_count.Data["qty"].ToString());
            query_sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME= "
                + "SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + pa_custom_pallet.Content.ToString() + "' AND ROWNUM=1) AND "
                + " VERSION_CODE=(SELECT VERSION_CODE FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI=:SN AND ROWNUM=1) ";
            var _result_cust = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            string Modeldesc = _result_cust.Data["cust_model_desc"].ToString();
            string M_MODEL_NAME = _result_count.Data["model_name"].ToString();
            query_sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE "
                + "PRG_NAME='" + M_MODEL_NAME + "' AND VR_CLASS='LOT_PRINT' AND VR_ITEM='PALTLIST' AND ROWNUM=1";
            var _result_parameter = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (_result_parameter.Data != null)
            {
                PRINTOTHERFLAG = true;
                tmpssn = _result_parameter.Data["vr_name"].ToString();
            }
            else
            {
                PRINTOTHERFLAG = false;
            }
            if (getmainVM._SsnLabelCheck)
            {
                if (PRINTOTHERFLAG)
                {

                    query_sql = "select a.serial_number,a.pallet_no,a.track_no,b.ssn4 SHIPPING_SN,a.MODEL_NAME,b.mac1,b.ssn3 from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b"
                        + " where a.imei='" + pa_custom_pallet.Content.ToString() + "' and a.serial_number=b.serial_number";

                }
                else
                {

                    query_sql = "select distinct a.serial_number, a.SHIPPING_SN,a.model_name,a.pallet_no,a.track_no,b.mac1,b.ssn3 from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b "
                        + " where imei='" + pa_custom_pallet.Content.ToString() + "' and a.serial_number=b.serial_number ";

                }
                var _result_wip_track = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                {
                    CommandText = query_sql,
                    SfcCommandType = SfcCommandType.Text
                });
                ResouceClass rc = new ResouceClass();
                DataTable dt = new DataTable();
                if (_result_wip_track.Data != null)
                {
                    dtParams.Clear();
                    dt = rc.ToDataTable<List_infomation_pallet>(_result_wip_track.Data.ToListObject<List_infomation_pallet>());
                    AddParams("AMBIT_PALLET_NO", dt.Rows[0]["pallet_no"].ToString());
                    AddParams("CUST_PALLET_NO", pa_custom_pallet.Content.ToString());
                    AddParams("SN_QTY", sQTY.ToString());
                    AddParams("PMODELDESC", Modeldesc);
                    AddParams("MODELNAME", M_MODEL_NAME);
                    AddParams("P_NO", dt.Rows[0]["track_no"].ToString());
                    AddParams("PAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                }
                else return;

                if (pageSum == 0)
                {
                    pageSum = sQTY_SN;
                }

                int allqty = 0;
                int tmppage = 0;
                int IntX = 0;
                foreach (var row in _result_wip_track.Data)
                {
                    IntX = IntX + 1;
                    allqty = allqty + 1;
                    AddParams("SN" + IntX, row["serial_number"].ToString());
                    //AddParams("PMSN" + IntX, row["serial_number"].ToString());
                    //AddParams("MSN" + IntX, row["shipping_sn"].ToString());
                    //AddParams("MAC" + IntX, row["mac1"].ToString());
                    //AddParams("MSNK", row["ssn3"].ToString());
                    if (IntX == pageSum || allqty == sQTY)
                    {
                        tmppage = tmppage + 1;
                        AddParams("ALLPAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                        AddParams("PAGEQTY", tmppage.ToString());
                        LabelName = "P_" + M_MODEL_NAME + "_PMSN_4.LAB";
                        PrintToCodeSoft("P", M_MODEL_NAME);
                        dtParams.Clear();
                        IntX = 0;
                    }
                }
            }
            else if (getmainVM._CartonLabelCheck)
            {
                if (PRINTOTHERFLAG)
                {

                    query_sql = "select distinct a.MCARTON_NO,a.model_name,a.pallet_no,a.track_no from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b"
                        + " where a.imei='" + pa_custom_pallet.Content.ToString() + "' and a.serial_number=b.serial_number";

                }
                else
                {

                    query_sql = "select distinct a.MCARTON_NO,a.model_name,a.pallet_no,a.track_no from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b "
                        + " where imei='" + pa_custom_pallet.Content.ToString() + "' and a.serial_number=b.serial_number ";

                }
                var _result_wip_track = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                {
                    CommandText = query_sql,
                    SfcCommandType = SfcCommandType.Text
                });
                ResouceClass rc = new ResouceClass();
                DataTable dt = new DataTable();
                if (_result_wip_track.Data != null)
                {
                    dtParams.Clear();
                    dt = rc.ToDataTable<List_info_carton_check>(_result_wip_track.Data.ToListObject<List_info_carton_check>());
                    AddParams("AMBIT_PALLET_NO", dt.Rows[0]["pallet_no"].ToString());
                    AddParams("CUST_PALLET_NO", pa_custom_pallet.Content.ToString());
                    AddParams("SN_QTY", sQTY.ToString());
                    AddParams("PMODELDESC", Modeldesc);
                    AddParams("MODELNAME", M_MODEL_NAME);
                    AddParams("P_NO", dt.Rows[0]["track_no"].ToString());
                    AddParams("PAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                }
                else return;

                if (pageSum == 0)
                {
                    pageSum = sQTY_SN;
                }

                int allqty = 0;
                int tmppage = 0;
                int IntX = 0;
                foreach (var row in _result_wip_track.Data)
                {
                    IntX = IntX + 1;
                    allqty = allqty + 1;
                    //AddParams("MCTN" + IntX, row["mcarton_no"].ToString());
                    if (IntX == pageSum || allqty == sQTY)
                    {
                        tmppage = tmppage + 1;
                        AddParams("ALLPAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                        AddParams("PAGEQTY", tmppage.ToString());
                        LabelName = "P_" + M_MODEL_NAME + "_PMSN_4.LAB";
                        PrintToCodeSoft("P", M_MODEL_NAME);
                        dtParams.Rows.Clear();
                        IntX = 0;
                    }
                }
            }
            else
            {
                query_sql = "select distinct a.serial_number, a.SHIPPING_SN,a.model_name,a.pallet_no,a.track_no,b.mac1,b.ssn3 from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b "
                            + " where imei='" + pa_custom_pallet.Content.ToString() + "' and a.serial_number=b.serial_number ";
                var _result_wip_track = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                {
                    CommandText = query_sql,
                    SfcCommandType = SfcCommandType.Text
                });
                ResouceClass rc = new ResouceClass();
                DataTable dt = new DataTable();
                int IntX = 0;
                foreach (var row in _result_wip_track.Data)
                {
                    IntX = IntX + 1;
                    //AddParams("MCTN" + IntX, row["mcarton_no"].ToString());
                    AddParams("SN" + IntX, row["serial_number"].ToString());
                    //AddParams("PMSN" + IntX, row["serial_number"].ToString());
                    //AddParams("MSN" + IntX, row["shipping_sn"].ToString());
                    //AddParams("MAC" + IntX, row["mac1"].ToString());
                }
            }
        }
        #endregion print A4 LABEL

        #region LAY DUONG DAN DOWNLOAD FILE LABEL
        public string GetUrlLabelFile()
        {
            var _data = new
            {
                TYPE = "GETURL",
                PRG_NAME = "PACK_PALT"
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    return "http://" + _RESArray[1] + "/";
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        return "";
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error:";
                        _sh.ShowDialog();
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions:";
                _sh.ShowDialog();
                return "";
            }
        }
        #endregion LAY DUONG DAN DOWNLOAD FILE LABEL

        private async Task<bool> killprocess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
            {
                process.Kill();
            }
            return true;
        }
        #region dong pallet thu cong
        private void ButtomClose_Click(object sender, RoutedEventArgs e)
        {
            string str_sql = "", TMP_local_PalletNo = "", TMP_cust_PalletNo = "";
            if (MessageBox.Show(GetPubMessage("00062"), "Warning", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
            {
                return;
            }
            txt_mcarton_no = "";
            findflag();
            if (M_flag.IndexOf("098") >= 0)
            {
                str_sql = "select * from sfis1.c_pallet_t where cust_pallet_no ='" + Pa_PalletNo.Text + "' and close_flag='Pallet'";
                var _result_c_pallet = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = str_sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (_result_c_pallet.Data != null)
                {
                    TMP_local_PalletNo = _result_c_pallet.Data["pallet_no"].ToString();
                    TMP_cust_PalletNo = _result_c_pallet.Data["cust_pallet_no"].ToString();
                }
                Pa_PalletNo.Text = TMP_local_PalletNo;
            }
            if (!string.IsNullOrWhiteSpace(Pa_PalletNo.Text))
            {
                closePallet();
                Label_error.Content = GetPubMessage("00236");
            }
            if (M_flag.IndexOf("098") >= 0)
            {
                Pa_PalletNo.Text = TMP_cust_PalletNo;
            }
            Input_Carton.SelectAll();
            Input_Carton.Focus();
        }
        #endregion dong pallet thu cong

        public bool locationlabel = false;



        #region class add du lieu vao datatabel
        public void AddParams(string _name, string _value)
        {
            if (dtParams.Columns.Count == 0)
            {
                dtParams.Columns.Add("Name");
                dtParams.Columns.Add("Value");
            }
            dtParams.Rows.Add(new object[] { _name, _value });
        }
        #endregion class add du lieu vao datatabel


        private async void getCartonInPallet()
        {
            string TMPCARTON, TMP1, TMP2;
            listbCarton.Items.Clear();
            string sql_str = "select distinct MCARTON_NO from sfism4.r_wip_tracking_t "
                + " WHERE pallet_no = '" + Pa_PalletNo.Text + "' ";
            var select_r_wip_tracking_t_mcarton = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            if (select_r_wip_tracking_t_mcarton.Data != null)
            {
                foreach (var row in select_r_wip_tracking_t_mcarton.Data)
                {
                    listbCarton.Items.Add(row["mcarton_no"].ToString());
                    sql_str = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='TMPCHECK' AND VR_NAME='" + G_sModel + "' ";
                    var select_c_parameter_ini = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql_str,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_c_parameter_ini.Data != null)
                    {
                        TMPCARTON = row["mcarton_no"].ToString();
                        if (TMPCARTON != txt_mcarton_no)
                        {
                            TMP1 = TMPCARTON.Substring(TMPCARTON.Length - 6, 8);
                            TMP2 = txt_mcarton_no.Substring(txt_mcarton_no.Length - 6, 8);
                            if (TMP1.CompareTo("44400986") <= 0)
                            {
                                if (TMP2.CompareTo("44400986") > 0)
                                {
                                    MessageBox.Show("Khong the dong chung,goi pqe xac nhan", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    Input_Carton.Focus();
                                    Input_Carton.SelectAll();
                                    return;
                                }
                            }
                            else
                            {
                                if (TMP2.CompareTo("44400987") < 0)
                                {
                                    MessageBox.Show("Khong the dong chung,goi pqe xac nhan", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    Input_Carton.Focus();
                                    Input_Carton.SelectAll();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        private bool checkMoTarget()
        {
            int iTarget, iPalletByMo;
            bool check = false;
            string sql_str = "select TARGET_QTY, OUTPUT_QTY, CLOSE_FLAG from sfism4.r_mo_base_t "
                + " where mo_number = '" + G_sMo + "' ";
            var select_r_mo_base_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            iTarget = Int32.Parse(select_r_mo_base_t.Data["target_qty"].ToString());
            sql_str = " select count(serial_number) SNCount from sfism4.r_wip_tracking_t "
                + " where mo_number = '" + G_sMo + "' "
                + " and pallet_no <> 'N/A' ";
            var select_r_wip_tracking_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            iPalletByMo = Int32.Parse(select_r_wip_tracking_t.Data["sncount"].ToString());
            if (iTarget == iPalletByMo)
                check = true;
            return check;
        }
        private string getCustomer()
        {
            string customer;
            string sql_str = " select c.customer customer "
                + " from sfis1.c_customer_t c, sfism4.r_mo_base_t r "
                + " where c.cust_code = r.cust_code "
                + " and r.mo_number = '" + G_sMo + "' and rownum=1 ";
            var select_customer = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            customer = select_customer.Data["customer"].ToString();
            return customer;
        }
        private bool updateToDB()
        {
            var _data = new
            {
                TYPE = "UPDATETODB",
                PRG_NAME = "PACK_PALT",
                LINE = G_sLine_Name,
                MCARTON_NO = txt_mcarton_no,
                C_EMP_NO = empNo,
                SECTION = M_sThisSection,
                W_STATION = My_Station,
                MYGROUP = M_sThisGroup
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[1];
                        _sh.MessageEnglish = _RESArray[1];
                        _sh.ShowDialog();
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error: updateToDB";
                        _sh.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions: updateToDB";
                _sh.ShowDialog();
                return false;
            }
        }

        private bool updateR107()
        {
            var _data = new
            {
                TYPE = "UPDATER107",
                PRG_NAME = "PACK_PALT",
                MODELNAME = G_sModel,
                MCARTON_NO = txt_mcarton_no,
                IMEI = pa_custom_pallet.Content.ToString(),
                PALETLNO = Pa_PalletNo.Text
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    return true;
                }
                else
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = _RESArray[0];
                    _sh.MessageEnglish = "Excute procedure have error: UPDATER107";
                    _sh.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions: UPDATER107";
                _sh.ShowDialog();
                return false;
            }
        }
        public static bool LINKRFID(string rfid_no)
        {
            var _data = new
            {
                TYPE = "RFID_LABEL",
                PRG_NAME = "PACK_PALT",
                MODELNAME = G_sModel,
                MCARTON_NO = txt_mcarton_no,
                VERSION_CODE = G_sVersion_Code,
                IMEI = G_sCust_PartNo,
                PALETLNO = Z_pallet_no,
                RFIDNO = rfid_no
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] != "OK")
                {
                    MessageBox.Show(_RESArray[1], "error", MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions: RFID_LABEL";
                _sh.ShowDialog();
                return false;
            }
        }
        private bool checkctnsequence(string mctn, string mo_number)
        {
            string sSql, tmp_str1, maximei;
            int i, tmp_int1, tmp_int2;
            bool check_ctn = false;

            sSql = "SELECT MAX(IMEI) imei FROM  SFISM4.R_WIP_TRACKING_T  WHERE  MO_NUMBER=  '" + mo_number + "' AND  IMEI <>'N/A'";

            var select_r_wip_tracking_t_max_imei = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sSql,
                SfcCommandType = SfcCommandType.Text
            });
            if (select_r_wip_tracking_t_max_imei.Data != null)
            {
                if (string.IsNullOrWhiteSpace(select_r_wip_tracking_t_max_imei.Data["imei"].ToString()) || select_r_wip_tracking_t_max_imei.Data["imei"].ToString() == "N/A")
                {
                    sSql = "SELECT MIN(MCARTON_NO) carton FROM  SFISM4.R_WIP_TRACKING_T  WHERE  MO_NUMBER=  '" + mo_number + "' AND MCARTON_NO <>'N/A'";
                    var select_r_wip_tracking_t_min_carton = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = sSql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_r_wip_tracking_t_min_carton.Data != null)
                    {
                        if (!string.IsNullOrWhiteSpace(select_r_wip_tracking_t_min_carton.Data["carton"].ToString()))
                        {
                            if (txt_mcarton_no != select_r_wip_tracking_t_min_carton.Data["carton"].ToString())
                            {
                                MessageBox.Show("NOT FIRST CARTON " + select_r_wip_tracking_t_min_carton.Data["carton"].ToString());
                                check_ctn = false;
                                return check_ctn;
                            }
                        }
                    }
                }
                else
                {
                    maximei = select_r_wip_tracking_t_max_imei.Data["imei"].ToString();
                    sSql = "SELECT  *  FROM  SFIS1.C_PALLET_T "
                        + " WHERE  MO_NUMBER=  '" + mo_number + "' AND  CUST_PALLET_NO = '" + maximei + "' AND CLOSE_FLAG= 'Pallet' ";
                    var select_c_pallet_t = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                    {
                        CommandText = sSql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_c_pallet_t.Data != null)
                    {
                        sSql = "SELECT MIN(MCARTON_NO) carton FROM  SFISM4.R_WIP_TRACKING_T  WHERE  MO_NUMBER=  '" + mo_number + "' AND  IMEI = '" + maximei + "'";
                        var select_r_wip_tracking_t_min_carton = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                        {
                            CommandText = sSql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (select_r_wip_tracking_t_min_carton.Data != null)
                        {
                            tmp_str1 = select_r_wip_tracking_t_min_carton.Data["carton"].ToString();
                            tmp_int1 = Int32.Parse(tmp_str1.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));
                            tmp_int2 = Int32.Parse(mctn.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));

                            if ((tmp_int2 - tmp_int1) > (G_PALLET_QTY - 1))
                            {
                                MessageBox.Show("CARTON  SEQUENCE ERROR", "error", MessageBoxButton.OK);
                                check_ctn = false;
                                return check_ctn;
                            }

                        }
                    }
                    else
                    {
                        sSql = "SELECT MIN(MCARTON_NO) carton FROM  SFISM4.R_WIP_TRACKING_T  WHERE  MO_NUMBER=  '" + mo_number + "' AND  IMEI = '" + maximei + "'";
                        var select_r_wip_tracking_min_carton = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                        {
                            CommandText = sSql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (select_r_wip_tracking_min_carton.Data != null)
                        {
                            tmp_str1 = select_r_wip_tracking_min_carton.Data["carton"].ToString();
                            tmp_int1 = Int32.Parse(tmp_str1.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));
                            tmp_int2 = Int32.Parse(mctn.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));

                            if (tmp_int2 - tmp_int1 != G_PALLET_QTY)
                            {
                                MessageBox.Show("CARTON  SEQUENCE ERROR", "ERROR", MessageBoxButton.OK);
                                return check_ctn = false;
                            }

                        }
                    }
                }
            }

            return check_ctn = true;
        }
        private bool CheckCtnSequenceInPalt(string mctn, string mo_number)
        {
            string sSql, tmp_str1 = "", tmp_str3, tmp_str_MINCARTON;
            int i, tmp_int1 = 0, tmp_int2 = 0, tmp_int3, tmp_int1_MINCARTON, tmp_int2_MINCARTON;
            bool check_ctn = false;

            sSql = " SELECT MAX(IMEI) imei"
                + " FROM  SFISM4.R_WIP_TRACKING_T "
                + " WHERE  MO_NUMBER=  '" + mo_number + "' AND  IMEI <>'N/A'";
            var select_wip_tracking_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sSql,
                SfcCommandType = SfcCommandType.Text
            });
            if (select_wip_tracking_t.Data["imei"].ToString() == "N/A" || string.IsNullOrWhiteSpace(select_wip_tracking_t.Data["imei"].ToString()))
            {
                sSql = " select * "
                    + " From Sfism4.R_Wip_Tracking_T "
                    + " Where Mo_Number='" + mo_number + "' AND "
                    + " MCarton_No='" + mctn + "' AND "
                    + " Shipping_SN=(Select Min(Item_1) "
                    + " From Sfism4.R_Mo_Ext3_t "
                    + " Where Mo_Number='" + mo_number + "' )";
                var select_wip_tracking_t_1 = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                {
                    CommandText = sSql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_wip_tracking_t_1.Data == null)
                {
                    MessageBox.Show("NOT FIRST CARTON");
                    check_ctn = false;
                    return check_ctn;
                }
            }
            else
            {
                sSql = "Select * "
                    + " From sfis1.C_Pallet_T "
                    + " Where Mo_Number='" + mo_number + "' AND "
                    + " Line_Name='" + G_sLine_Name + "' AND ROWNUM=1 ";
                var select_c_pallet_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = sSql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_c_pallet_t.Data != null)
                {
                    sSql = "SELECT MAX(MCARTON_NO) maxcarton "
                        + " FROM  SFISM4.R_WIP_TRACKING_T "
                        + " WHERE  MO_NUMBER=  '" + mo_number + "' AND "
                        + " IMEI = '" + select_c_pallet_t.Data[""] + "'";
                }
                var select_r_wip_tracking_t_max_carton = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = sSql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_r_wip_tracking_t_max_carton.Data != null)
                {
                    tmp_str1 = select_r_wip_tracking_t_max_carton.Data["maxcarton"].ToString();
                }

                if (!string.IsNullOrWhiteSpace(tmp_str1))
                {
                    tmp_int1 = Int32.Parse(tmp_str1.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));
                    tmp_int2 = Int32.Parse(mctn.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));

                    sSql = "SELECT MIN(MCarton_No) mincarton "
                        + " FROM  SFISM4.R_Wip_Tracking_T "
                        + " WHERE  MO_number=  '" + mo_number + "' AND "
                        + " IMEI = 'N/A' AND MCARTON_NO<>'N/A' ";
                    var select_wip_tracking_t_min_carton = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = sSql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    tmp_str_MINCARTON = select_wip_tracking_t_min_carton.Data["mincarton"].ToString();
                    tmp_int1_MINCARTON = Int32.Parse(tmp_str_MINCARTON.Substring(tmp_str_MINCARTON.Length - G_cartonnolength, G_cartonnolength));
                    tmp_int2_MINCARTON = Int32.Parse(tmp_str_MINCARTON.Substring(tmp_str_MINCARTON.Length - G_cartonnolength, G_cartonnolength));

                    if (tmp_int2_MINCARTON < tmp_int2)
                    {
                        check_ctn = false;
                        return check_ctn;
                    }
                }
                else
                {
                    sSql = "SELECT Max(MCarton_No) maxcarton "
                        + " FROM  SFISM4.R_Wip_Tracking_T "
                        + " WHERE  MO_NUMBER=  '" + mo_number + "' AND "
                        + " IMEI <> ''N/A'' AND MCARTON_NO<>'K13C0134260000012' ";
                    var select_r_wip_tracking_t_max_carton_1 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = sSql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_r_wip_tracking_t_max_carton_1.Data != null)
                    {
                        tmp_str1 = select_r_wip_tracking_t_max_carton_1.Data["maxcarton"].ToString();
                        tmp_int1 = Int32.Parse(tmp_str1.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));
                        tmp_int2 = Int32.Parse(mctn.Substring(tmp_str1.Length - G_cartonnolength, G_cartonnolength));

                        if ((tmp_int2 - tmp_int1) >= 0)
                        {
                            sSql = "SELECT MIN(MCarton_No) mincarton "
                                + " FROM  SFISM4.R_Wip_Tracking_T "
                                + " WHERE  MO_number=  '" + mo_number + "' AND "
                                + " IMEI = 'N/A' AND MCARTON_NO<>'N/A' ";
                            var select_r_wip_tracking_t_min_carton = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                            {
                                CommandText = sSql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            tmp_str_MINCARTON = select_r_wip_tracking_t_min_carton.Data["mincarton"].ToString();
                            tmp_int1_MINCARTON = Int32.Parse(tmp_str_MINCARTON.Substring(tmp_str_MINCARTON.Length - G_cartonnolength, G_cartonnolength));
                            tmp_int2_MINCARTON = Int32.Parse(tmp_str_MINCARTON.Substring(tmp_str_MINCARTON.Length - G_cartonnolength, G_cartonnolength));

                            if (tmp_int2_MINCARTON < tmp_int2)
                            {
                                check_ctn = false;
                                return check_ctn;
                            }


                        }
                        else
                        {
                            tmp_int3 = tmp_int2 - 1;
                            tmp_str3 = mctn.Substring(0, tmp_str1.Length - G_cartonnolength) + tmp_int3.ToString();
                            sSql = " SELECT *  "
                                + " FROM  SFISM4.R_WIP_TRACKING_T "
                                + " WHERE  MO_NUMBER=  '" + mo_number + "' AND "
                                + " MCarton_No='" + tmp_str3 + "' AND  "
                                + " IMEI<>'N/A'";
                            var select_r_wip_traking_t_1 = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                            {
                                CommandText = sSql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (select_r_wip_traking_t_1.Data == null)
                            {
                                sSql = "Select *  "
                                    + " From Sfism4.R_Wip_Tracking_T "
                                    + " Where Mo_Number='" + mo_number + "' AND "
                                    + " MCarton_No='" + mctn + "' AND "
                                    + " Shipping_SN  BETWEEN (Select Item_1 "
                                    + " From Sfism4.R_Mo_Ext3_t  "
                                    + " Where Mo_Number='" + mo_number + "') AND (Select Item_2 "
                                    + " From Sfism4.R_Mo_Ext3_t "
                                    + " Where Mo_Number='" + mo_number + "')";
                                var select_r_wip_tracking_t_2 = _sfcHttpClient.QueryList(new QuerySingleParameterModel
                                {
                                    CommandText = sSql,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (select_r_wip_tracking_t_2.Data == null)
                                {
                                    MessageBox.Show("CARTON  SEQUENCE ERROR, " + tmp_str3 + "HAS NOT IMEI ");
                                    check_ctn = false;
                                    return check_ctn;
                                }
                            }
                        }

                    }
                }

                //dang sua doan nay.
            }
            return check_ctn;
        }
        private bool FCheckDOA(string paramPalletNo, string flag)
        {
            bool check = true;
            string mo_number1, mo_number2, mo_number3;
            if (CHECKDOA == "0")
            {
                string sql_str = "SELECT MO_NUMBER FROM  SFISM4.R107 WHERE pallet_no='" + paramPalletNo + "'  AND ROWNUM=1";
                var select_data = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = sql_str,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_data.Data == null)
                {
                    check = true;
                    return check;
                }
                else
                {
                    mo_number1 = select_data.Data["mo_number"].ToString();
                    str_sql_query = "SELECT * FROM SFISM4.R_BPCS_moplan_T WHERE MO_NUMBER='" + mo_number1 + "' AND    SAP_MO_TYPE='ZA13'";
                    var select_bpcs_moplan_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_bpcs_moplan_t.Data != null)
                    {
                        check = true;
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = "80104";
                        _smf.ShowDialog();
                        return check;
                    }
                    str_sql_query = "SELECT  MO_NUMBER FROM SFISM4.R107 WHERE pallet_no='" + paramPalletNo + "' and MO_NUMBER>'" + mo_number1 + "'  AND ROWNUM=1";
                    var select_r107 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_r107.Data != null)
                    {
                        mo_number2 = select_r107.Data["mo_number"].ToString();
                        sql_str = "SELECT * FROM SFISM4.R_BPCS_moplan_T WHERE MO_NUMBER='" + mo_number2 + "' AND  SAP_MO_TYPE='ZA13'";
                        var select_bpct_moplan_t_2 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                        {
                            CommandText = sql_str,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (select_bpct_moplan_t_2.Data != null)
                        {
                            check = false;
                            ShowMessageForm _smf = new ShowMessageForm();
                            _smf.httpclient = _sfcHttpClient;
                            _smf.errorcode = "80104";
                            _smf.ShowDialog();
                            return check;
                        }
                    }
                    sql_str = "SELECT  MO_NUMBER FROM SFISM4.R107 WHERE pallet_no='" + paramPalletNo + "' and MO_NUMBER<'" + mo_number1 + "'  AND ROWNUM=1";
                    var select_r107_2 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = sql_str,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_r107_2.Data != null)
                    {
                        mo_number3 = select_r107_2.Data["mo_number"].ToString();
                        sql_str = "SELECT * FROM SFISM4.R_BPCS_moplan_T WHERE MO_NUMBER='" + mo_number3 + "' AND    SAP_MO_TYPE='ZA13'";
                        var select_bpcs_moplan_t_3 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                        {
                            CommandText = sql_str,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (select_bpcs_moplan_t_3.Data != null)
                        {
                            check = false;
                            ShowMessageForm _smf = new ShowMessageForm();
                            _smf.httpclient = _sfcHttpClient;
                            _smf.errorcode = "80104";
                            _smf.ShowDialog();
                            return check;
                        }
                    }

                }
            }
            else if (CHECKDOA == "1")
            {
                str_sql_query = "SELECT MO_NUMBER FROM  SFISM4.R107 WHERE pallet_no='" + paramPalletNo + "'  AND ROWNUM=1";
                var select_r107 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = str_sql_query,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_r107.Data == null)
                {
                    check = true;
                    return check;
                }
                mo_number1 = select_r107.Data["mo_number"].ToString();
                str_sql_query = "SELECT * FROM SFISM4.R_BPCS_moplan_T WHERE MO_NUMBER='" + mo_number1 + "' AND    SAP_MO_TYPE<>'ZA13'";
                var select_bpcs_moplan_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = str_sql_query,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_bpcs_moplan_t.Data != null)
                {
                    check = false;
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80105";
                    _smf.ShowDialog();
                    return check;
                }
                str_sql_query = "SELECT  MO_NUMBER FROM SFISM4.R107 WHERE pallet_no='" + paramPalletNo + "' and MO_NUMBER<>'" + mo_number1 + "'  AND ROWNUM=1";
                var select_r107_1 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = str_sql_query,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_r107_1.Data != null)
                {
                    mo_number2 = select_r107_1.Data["mo_number"].ToString();
                    str_sql_query = "SELECT * FROM SFISM4.R_BPCS_moplan_T WHERE MO_NUMBER='" + mo_number2 + "' AND    SAP_MO_TYPE<>'ZA13'";
                    var select_bps_moplan_t_1 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_bps_moplan_t_1.Data != null)
                    {
                        check = false;
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = "80105";
                        _smf.ShowDialog();
                        return check;
                    }
                }
                if (check)
                {
                    str_sql_query = "select * from sfism4.r107 where pallet_no='" + paramPalletNo + "' and mo_number<>'" + txt_mcarton_no + "' and rownum=1";
                    var select_r107_2 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_r107_2.Data != null)
                    {
                        check = false;
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = "80105";
                        _smf.ShowDialog();
                        return check;
                    }
                }
            }
            return check;
        }
        private bool FCheckPIRUN45(string paramPalletNo, string flag)
        {
            bool check = true;
            if (CHECKMO45 == "0")
            {
                string sql_str = "select * from sfism4.r107 where  pallet_no='" + paramPalletNo + "' and mo_number LIKE '46%' and rownum=1";
                var select_data = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = sql_str,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_data.Data != null)
                {
                    check = false;
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80102";
                    _smf.ShowDialog();
                }
            }
            else if (CHECKMO45 == "1")
            {
                string sql_str = "select * from sfism4.r107 where  pallet_no='" + paramPalletNo + "' and mo_number not LIKE '45%' and rownum=1";
                var select_data = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = sql_str,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_data.Data != null)
                {
                    check = false;
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80102";
                    _smf.ShowDialog();
                }
                if (check)
                {
                    sql_str = "select * from sfism4.r107 where pallet_no='" + paramPalletNo + "' and mo_number<>'" + editMO.Text + "' and rownum=1";

                    var select_data_r107 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = sql_str,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_data_r107.Data != null)
                    {
                        check = false;
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = "80103";
                        _smf.ShowDialog();
                    }
                }
            }
            return check;
        }
        private bool FCheckPIRUN(string paramPalletNo, string flag)
        {
            bool check = true;
            if (CHECKMO46 == "0")
            {
                string sql_str = "select * from sfism4.r107 where  pallet_no='" + paramPalletNo + "' and mo_number LIKE '46%' and rownum=1";
                var select_data = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = sql_str,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_data.Data != null)
                {
                    check = false;
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80102";
                    _smf.ShowDialog();
                }
            }
            else if (CHECKMO46 == "1")
            {
                string sql_str = "select * from sfism4.r107 where  pallet_no='" + paramPalletNo + "' and mo_number not LIKE '46%' and rownum=1";
                var select_data = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = sql_str,
                    SfcCommandType = SfcCommandType.Text
                });
                if (select_data.Data != null)
                {
                    check = false;
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80102";
                    _smf.ShowDialog();
                }
                if (check)
                {
                    sql_str = "select * from sfism4.r107 where pallet_no='" + paramPalletNo + "' and mo_number<>'" + editMO.Text + "' and rownum=1";

                    var select_data_r107 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = sql_str,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_data_r107.Data != null)
                    {
                        check = false;
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = "80103";
                        _smf.ShowDialog();
                    }
                }
            }
            return check;
        }

        private string getPalletNo()
        {
            string temp = "";
            if (rma_flag == false)
            {
                temp = "N";
            }
            string temp1 = "";
            if (IsPackingByMoElseByModel)
            {
                temp1 = "1";
            }
            var _data = new
            {
                TYPE = "GETPALLETNO",
                PRG_NAME = "PACK_PALT",
                LINE = G_sLine_Name,
                MODEL = G_sModel,
                VERSION_CODE = G_sVersion_Code,
                MO_NUMBER = G_sMo,
                PALLETQTY = Ed_PalletQty.Content.ToString(),
                STATUS = temp1,
                MACPC = macpc,
                RMA_FLAG = temp
            };
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    string sPalletNO = _RESArray[1].ToString();
                    G_sCust_PartNo = _RESArray[2].ToString();
                    M_iPalletCount = Int32.Parse(_RESArray[3]) + 1;

                    return sPalletNO;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.httpclient = _sfcHttpClient;
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[1];
                        _sh.MessageEnglish = _RESArray[1] + " trung lap ma pallet";
                        _sh.ShowDialog();
                        return "";
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error: PACK_PALT_API_EXE_10-TYPE: GETPALLETNO";
                        _sh.ShowDialog();
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions: PACK_PALT_API_EXE_10-TYPE: GETPALLETNO";
                _sh.ShowDialog();
                return "";
            }
        }


        private bool checkcpeiiiCarton(string carton)
        {
            bool bol_cpeii = true;
            string ssql, sVER = "", sMODEL_NAME = "";
            int Ctnlength = 0;
            ssql = "SELECT * FROM SFISM4.R107 WHERE MCARTON_NO='" + carton + "' AND ROWNUM=1";
            var query_temp = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_temp.Data != null)
            {
                sVER = query_temp.Data["version_code"].ToString();
                sMODEL_NAME = query_temp.Data["model_name"].ToString();
            }
            else
            {
                bol_cpeii = false;
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = _sfcHttpClient;
                _smf.errorcode = "80109";
                _smf.ShowDialog();

                return bol_cpeii;
            }
            ssql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME='" + sMODEL_NAME + "' AND VERSION_CODE='" + sVER + "'";
            var quryTemp = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data != null)
            {
                Ctnlength = quryTemp.Data["cust_carton_prefix"].ToString().Trim().Length + Int32.Parse(quryTemp.Data["CUST_CARTON_LENG"].ToString());
                if (carton.Length != Ctnlength)
                {
                    bol_cpeii = false;
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.httpclient = _sfcHttpClient;
                    _smf.errorcode = "80110";
                    _smf.ShowDialog();
                    return bol_cpeii;
                }
            }
            else
            {
                bol_cpeii = false;
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = _sfcHttpClient;
                _smf.errorcode = "00084";
                _smf.ShowDialog();
                return bol_cpeii;
            }
            if (Int32.Parse(carton.Substring(8, 4)) < 1000)
            {
                bol_cpeii = false;
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = _sfcHttpClient;
                _smf.errorcode = "80113";
                _smf.ShowDialog();
                return bol_cpeii;
            }

            return bol_cpeii;
        }

        private bool checkDellCarton(string carton)
        {
            bool bol_dell = true;
            string ssql, sVER = "", sMODEL_NAME = "", MYDI = "", MYTEMP = "";
            int Ctnlength = 0, sfcqty = 0;

            ssql = "SELECT * FROM SFISM4.R107 WHERE MCARTON_NO='" + carton + "'";
            var quryTemp = _sfcHttpClient.QueryList(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data != null)
            {
                int i = 1;
                foreach (var row in quryTemp.Data)
                {
                    sVER = row["version_code"].ToString();
                    sMODEL_NAME = row["model_name"].ToString();
                    i = i + 1;
                }
                sfcqty = i;
            }
            else
            {
                bol_dell = false;
                MessageBox.Show(GetPubMessage("80109"));
                return bol_dell;
            }

            ssql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME='" + sMODEL_NAME + "' AND VERSION_CODE='" + sVER + "'";
            var query_temp = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_temp.Data != null)
            {
                MYDI = query_temp.Data["d1"].ToString();
                if (M_flag.IndexOf("091") > 0)
                {
                    Ctnlength = query_temp.Data["cust_carton_prefix"].ToString().Length + query_temp.Data["cust_carton_leng"].ToString().Length + 11;
                }
                else
                {
                    if (MYDI.Length == 4)
                    {
                        Ctnlength = query_temp.Data["cust_carton_prefix"].ToString().Length + query_temp.Data["cust_carton_leng"].ToString().Length + 13;
                    }
                    else
                    {
                        Ctnlength = query_temp.Data["cust_carton_prefix"].ToString().Length + query_temp.Data["cust_carton_leng"].ToString().Length + 12;
                    }
                }
            }
            else
            {
                bol_dell = false;
                MessageBox.Show(GetPubMessage("00084"));
                return bol_dell;
            }
            //MYTEMP = carton.Substring(27, carton.Length - 27);
            //if (Int32.Parse(MYTEMP) != sfcqty)
            //{
            //    bol_dell = false;
            //    MessageBox.Show( GetPubMessage("80112") + carton.Substring(27, carton.Length - 28));
            //    return bol_dell;
            //}


            return bol_dell;
        }
        public bool checkI01Carton(string carton)
        {
            bool bo_check = true;
            int Ctnlength = 0;
            string sVER, sMODEL_NAME;
            string sql_str = "SELECT * FROM SFISM4.R107 WHERE (MCARTON_NO='" + carton + "' or CARTON_NO='" + carton + "') AND ROWNUM=1";
            var query_r107 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_r107.Data != null)
            {
                sVER = query_r107.Data["version_code"].ToString();
                sMODEL_NAME = query_r107.Data["model_name"].ToString();
            }
            else
            {
                MessageBox.Show(GetPubMessage("80109"));
                bo_check = false;
                return bo_check;
            }

            sql_str = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME='" + sMODEL_NAME + "' AND VERSION_CODE='" + sVER + "'";
            var query_table = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_table.Data != null)
            {
                Ctnlength = query_table.Data["cust_carton_prefix"].ToString().Length + Int32.Parse(query_table.Data["CUST_CARTON_LENG"].ToString()) + 2;

                if (carton.Length != Ctnlength)
                {
                    bo_check = false;
                    MessageBox.Show(GetPubMessage("80110"));
                    return bo_check;
                }
            }
            else
            {
                MessageBox.Show(GetPubMessage("00084"));
                bo_check = false;
                return bo_check;
            }
            if (carton.Substring(19, 2) == "00")
            {
                bo_check = false;
                MessageBox.Show(GetPubMessage("00099"));
            }

            return bo_check;

        }
        private bool UPDATENECCUSTSNCONFIG(string date)
        {
            string pallet_pre_old = "";
            bool check_bool = true;

            string sql_str = " select *  from SFIS1.C_CUST_SNRULE_T  WHERE  MODEL_NAME= '" + G_sModel + "' "
                + " and   VERSION_CODE  ='" + G_sVersion_Code + "'";
            var query_table = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = sql_str,
                SfcCommandType = SfcCommandType.Text
            });
            pallet_pre_old = query_table.Data["cust_pallet_prefix"].ToString();

            //if (pallet_pre_old.Length == 5)
            //{
            //    try
            //    {
            //        pallet_pre_old = pallet_pre_old.Substring(0, 4);
            //        if (Int32.Parse(pallet_pre_old) < 100 && Int32.Parse(pallet_pre_old) > 1232)
            //        {
            //            MessageBox.Show("PRE  CONFIG  ERROR!");
            //            check_bool = false;
            //            return check_bool;
            //        }
            //    }
            //    catch
            //    {
            //        MessageBox.Show("PRE  CONFIG  ERROR!");
            //        check_bool = false;
            //        return check_bool;
            //    }

            //}
            //else
            //{
            //    MessageBox.Show("IMEI PRE LENGTH CONFIG ERROR!");
            //    check_bool = false;
            //    return check_bool;
            //}
            //if (string.Compare(date, pallet_pre_old) > 0)
            //{
            //    string sql_string = "update SFIS1.C_CUST_SNRULE_T "
            //        + " set CUST_PALLET_PREFIX='3'||datenow, "
            //        + " CUST_LAST_PALLET='' "
            //        + " where cust_code='"+ G_sCust_No + "' and model_name='"+ G_sModel + "' "
            //        + " AND VERSION_CODE='"+ G_sVersion_Code+"' ";
            //    var query_update = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            //    {
            //        CommandText = sql_string,
            //        SfcCommandType = SfcCommandType.Text
            //    });

            //}

            str_sql_query = " select *  from SFIS1.C_CUST_SNRULE_T  "
                + " WHERE   cust_code= '" + G_sCust_No + "' and MODEL_NAME= '" + G_sModel + "' and "
                + " VERSION_CODE  ='" + G_sVersion_Code + "'";

            var query_dt1 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = str_sql_query,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_dt1.Data != null)
            {
                if (M_flag.IndexOf("I") > 0)
                {
                    G_sCust_PNo_Next = inc_no(query_dt1.Data["cust_last_pallet"].ToString().Substring(query_dt1.Data["cust_pallet_prefix"].ToString().Length + 3, Int32.Parse(query_dt1.Data["cust_pallet_leng"].ToString())),
                        query_dt1.Data["cust_pallet_str"].ToString(),
                        Int32.Parse(query_dt1.Data["cust_pallet_leng"].ToString()));
                    G_sCust_Prefix = query_dt1.Data["cust_pallet_prefix"].ToString();
                    //G_sCust_PNo_Next = G_sCust_Prefix + G_sCust_PNo_Next + query_dt1.Data["cust_pallet_postfix"].ToString();
                }
                else
                {
                    string ad = query_dt1.Data["cust_last_pallet"].ToString();
                    int leg = query_dt1.Data["cust_pallet_prefix"].ToString().Length;
                    string asd = query_dt1.Data["cust_pallet_leng"].ToString();
                    string dff = query_dt1.Data["cust_pallet_str"].ToString();

                    string G_sCust_PNo_Next1 = inc_no(ad.Substring(leg, Int32.Parse(asd)), dff, Int32.Parse(asd));

                    G_sCust_PNo_Next = inc_no(query_dt1.Data["cust_last_pallet"].ToString().Substring(query_dt1.Data["cust_pallet_prefix"].ToString().Length, Int32.Parse(query_dt1.Data["cust_pallet_leng"].ToString())),
                       query_dt1.Data["cust_pallet_str"].ToString(), Int32.Parse(query_dt1.Data["cust_pallet_leng"].ToString()));
                    G_sCust_Prefix = query_dt1.Data["cust_pallet_prefix"].ToString();
                    //G_sCust_PNo_Next = G_sCust_Prefix + G_sCust_PNo_Next + query_dt1.Data["cust_pallet_postfix"].ToString();
                }
            }
            return check_bool;
        }
        public string getSysDate()
        {
            string date = "";
            string str_sql = "select to_char(sysdate, 'YYMMDD') sysstr from dual";
            var query_date = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = str_sql,
                SfcCommandType = SfcCommandType.Text
            });
            date = query_date.Data["sysstr"].ToString();
            return date;
        }
        private int getCapacity(string mo_number)
        {
            int data = 0;
            string str_sql = "select c.*  "
                + " from sfis1.c_pack_param_t c, sfism4.r_mo_base_t r"
                + " where c.model_name = r.model_name "
                + " and c.version_code = r.version_code "
                + " and r.mo_number = '" + mo_number + "' ";

            var query_dt = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = str_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_dt.Data != null)
            {
                G_PALLET_QTY = Int32.Parse(query_dt.Data["pallet_qty"].ToString());
                data = Int32.Parse(query_dt.Data["pallet_qty"].ToString());
            }
            return data;
        }
        private int getCapacitysec(string mo_number)
        {
            h_flag1 = true;
            int data = 0;
            string str_sql = "SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + mo_number + "'";
            var dt_query = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = str_sql,
                SfcCommandType = SfcCommandType.Text
            });
            //if (dt_query.Data != null)
            //{
            //    ship_way = dt_query.Data["imei_mo_option"].ToString();
            //}
            str_sql = "select c.*  "
                + " from sfis1.c_pack_param_t c, sfism4.r_mo_base_t r"
                + " where c.model_name = r.model_name "
                + " and c.version_code = r.version_code "
                + " and r.mo_number = '" + mo_number + "' ";

            var dt_query_1 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = str_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (dt_query_1.Data != null)
            {
                if (ship_way == "S" || string.IsNullOrWhiteSpace(ship_way))
                {
                    G_PALLET_QTY = Int32.Parse(dt_query_1.Data["pallet_qty"].ToString());
                    data = Int32.Parse(dt_query_1.Data["pallet_qty"].ToString());
                }
                else
                {
                    G_PALLET_QTY1 = dt_query_1.Data["coo"].ToString();
                    if (string.IsNullOrWhiteSpace(G_PALLET_QTY1))
                    {
                        MessageBox.Show(GetPubMessage("00111"));
                        Label_error.Content = GetPubMessage("00111");
                        h_flag1 = false;
                        return data;
                    }
                    G_PALLET_QTY = Int32.Parse(dt_query_1.Data["coo"].ToString());
                    data = Int32.Parse(dt_query_1.Data["coo"].ToString());
                }
            }
            return data;
        }
        private string IsCPE3()
        {
            string data = "N/A";
            string str_sql = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME ='SFISSITE'";
            var dt_query = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = str_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (dt_query.Data != null)
            {
                data = dt_query.Data["model_serial"].ToString();
            }
            return data;
        }

        public async Task<bool> CheckMo()
        {
            var _data = new
            {
                TYPE = "CHECKMO",
                PRG_NAME = "PACK_PALT",
                CARTON = txt_mcarton_no
            };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');
                if (_RESArray[0] == "OK")
                {

                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = _RESArray[1].ToString();
                        _smf.ShowDialog();
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.CustomFlag = true;
                        _smf.MessageVietNam = _RESArray[0].ToString();
                        _smf.MessageEnglish = "procedure have exceptions:";
                        _smf.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = _sfcHttpClient;
                _smf.CustomFlag = true;
                _smf.MessageVietNam = ex.Message;
                _smf.MessageEnglish = "Call procedure have exceptions:";
                _smf.ShowDialog();
                return false;
            }

        }
        private bool Check_C_ModelType(string model_type)
        {
            bool check = false;
            string s_type = "";
            str_sql_query = "select vr_name from sfis1.C_PARAMETER_INI  where VR_ITEM='MODEL_TYPE' AND prg_name='CISCO_RANGE'";

            var query_para = _sfcHttpClient.QueryList(new QuerySingleParameterModel
            {
                CommandText = str_sql_query,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_para.Data != null)
            {
                foreach (var row in query_para.Data)
                {
                    s_type = row["vr_name"].ToString();
                    if (model_type.IndexOf(s_type) > 0)
                    {
                        check = true;
                        return check;
                    }
                }
            }
            else
            {
                return check;
            }
            return check;
        }
        public bool checkdb()
        {
            Result = true;


            return Result;
        }//chua viet

        #region kiem tra ma carton
        public bool FindCarton()
        {
            Result = true;
            if (listbCarton.Items.IndexOf(txt_mcarton_no) != -1)
            {
                str_GetPubMessage = "";

                MessageBox.Show(GetPubMessage("80100"));
                Result = false;
            }
            else
            {

                if (findCartonDetail())
                {
                    if (CheckRoute() == false)
                    {
                        Label_error.Content = GetPubMessage("80101");
                        MessageBox.Show(GetPubMessage("80101"));
                        txt_mcarton_no = "";
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("The Carton No " + txt_mcarton_no + " not found or Close not yet!!");
                    Result = false;
                    return Result;
                }
            }

            return Result;
        }
        #endregion kiem tra ma carton

        #region check route cua carton
        public bool CheckRoute()
        {
            query_sql = "select serial_number from sfism4.r_wip_tracking_t"
                + " where MCARTON_NO = '" + txt_mcarton_no + "' or CARTON_NO = '" + txt_mcarton_no + "'";

            var query_route = _sfcHttpClient.QueryList(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_route.Data != null)
            {


                foreach (var rows in query_route.Data)
                {
                    var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.CHECK_ROUTE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="LINE",Value=G_sLine_Name,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="DATA",Value=rows["serial_number"].ToString(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                    });
                    dynamic _ads = _result.Data;
                    string _RES = _ads[0]["res"];

                    if (_RES != "OK")
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.MessageEnglish = rows["serial_number"].ToString() + " - " + _RES;
                        _smf.MessageVietNam = rows["serial_number"].ToString() + " - " + _RES;
                        _smf.CustomFlag = true;
                        _smf.ShowDialog();
                        txt_mcarton_no = "";
                        Input_Carton.Text = "";
                        return false;

                    }
                    else { return true; }
                }
                return true;
            }
            else
            {
                return false;
            }


        }
        #endregion check route cua carton

        #region du lieu cua thung carton
        private bool findCartonDetail()
        {
            var _data = new
            {
                TYPE = "FINDCARTONDETAIL",
                PRG_NAME = "PACK_PALT",
                CARTON_NO = txt_mcarton_no,
                LINE = G_sLine_Name
            };
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = _sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    G_sMo = _RESArray[1];
                    G_sModel = _RESArray[2];
                    G_sTA = _RESArray[3];
                    G_sPartNo = _RESArray[4];
                    G_sVersion_Code = _RESArray[5];
                    //G_sCust_Prefix = _RESArray[6];
                    G_sCust_No = getCust_No(G_sMo);
                    if (G_sMo.Substring(0, 2) == "46")
                    {
                        CHECKMO46 = "1";
                    }
                    else
                    {
                        CHECKMO46 = "0";
                    }
                    if (G_sMo.Substring(0, 2) == "45")
                    {
                        CHECKMO45 = "1";
                    }
                    else
                    {
                        CHECKMO45 = "0";
                    }
                    if (checkdoamo(G_sMo))
                    {
                        CHECKDOA = "1";
                    }
                    else
                    {
                        CHECKDOA = "0";
                    }
                    findflag();
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[1];
                        _sh.MessageEnglish = _RESArray[1];
                        _sh.ShowDialog();
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error:";
                        _sh.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions:";
                _sh.ShowDialog();
                return false;
            }
        }
        #endregion du lieu cua thung carton



        private string inc_no(string custom_no, string custom_kind, int custom_length)
        {
            string str_result_temp = "";
            bool bl_add;
            int i_pos = 0;
            if (custom_kind[1] != 0 && string.IsNullOrWhiteSpace(custom_no))
                bl_add = false;
            else
                bl_add = true;

            if (custom_no.Length > custom_length)
            {
                str_result_temp = "0";
                return str_result_temp;
            }

            for (int i = 0; i < custom_length; i++)
            {
                if (custom_no.Length > 0)
                {
                    i_pos = custom_kind.IndexOf(custom_no.Substring(custom_no.Length - 1, 1));
                    if (i_pos < 0)
                    {
                        str_result_temp = "0";
                        return str_result_temp;
                    }
                    else if (bl_add == false)
                    {
                        str_result_temp = custom_kind[i_pos] + str_result_temp;
                    }
                    else if (i_pos == custom_kind.Length)
                    {
                        str_result_temp = custom_kind[1] + str_result_temp;
                    }
                    else if (i_pos == custom_kind.Length - 1)
                    {
                        str_result_temp = custom_kind[0] + str_result_temp;
                    }
                    else
                    {
                        str_result_temp = custom_kind[i_pos + 1] + str_result_temp;
                        bl_add = false;
                    }
                    //Delete(custom_no, custom_no.Length, 1);
                    string temp = custom_no.Remove(custom_no.Length - 1, 1);
                    custom_no = temp;
                }
                else
                {
                    if (bl_add)
                    {
                        str_result_temp = custom_kind[2] + str_result_temp;
                        bl_add = false;
                    }
                    else
                    {
                        str_result_temp = custom_kind[1] + str_result_temp;
                    }
                }
            }
            if (bl_add)
            {
                str_result_temp = "0";
            }
            return str_result_temp;

        }



        public async Task<bool> FindRmaMO(string mo_number)
        {
            //rma_flag = false;
            //string sqlstr, rma_mo_pre;
            //sqlstr = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE VR_NAME='PREFIX' AND VR_ITEM='MO' AND PRG_NAME='RMA'";
            //var query_rma_mo = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            //{
            //    CommandText = sqlstr,
            //    SfcCommandType = SfcCommandType.Text
            //});

            //if (query_rma_mo.Data != null)
            //{
            //    rma_mo_pre = query_rma_mo.Data["vr_value"].ToString();
            //    if (!string.IsNullOrWhiteSpace(rma_mo_pre))
            //    {
            //        if (mo_number.Substring(0, 4) == rma_mo_pre) rma_flag = true;
            //    }
            //}

            var _data = new
            {
                TYPE = "FINDRMAMO",
                PRG_NAME = "PACK_PALT",
                MO_NUMBER = mo_number
            };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');
                if (_RESArray[0] == "TRUE")
                {
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "FAIL")
                    {
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.CustomFlag = true;
                        _smf.MessageVietNam = _RESArray[0].ToString();
                        _smf.MessageEnglish = "procedure have exceptions:";
                        _smf.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = _sfcHttpClient;
                _smf.CustomFlag = true;
                _smf.MessageVietNam = ex.Message;
                _smf.MessageEnglish = "Call procedure have exceptions:";
                _smf.ShowDialog();
                return false;
            }
        }
        private bool findflag()
        {
            query_sql = "SELECT MODEL_TYPE FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='" + G_sModel + "' ";

            var query_model_desc_t = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_model_desc_t.Data != null)
            {
                M_flag = query_model_desc_t.Data["model_type"].ToString().Trim();

                if (M_flag.Contains("A"))
                {
                    IsPackingByMoElseByModel = false;
                }
                else if (M_flag.Contains("002"))
                {
                    IsPackingByMoElseByModel = false;
                }
                else if (M_flag.Contains("008"))
                {
                    IsPackingByMoElseByModel = false;
                }
                else
                {
                    IsPackingByMoElseByModel = true;
                }

                if (CHECK_MO_FLAG == false)
                {
                    IsPackingByMoElseByModel = false;
                }
            }
            return true;

        }
        private string getCust_No(string mo_number)
        {
            query_sql = "select * from sfism4.r_mo_base_t "
                       + " where mo_number='" + mo_number + "' ";

            var result_r105 = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result_r105.Data != null)
            {
                str_getCust_No = result_r105.Data["cust_code"].ToString();
            }
            return str_getCust_No;
        }

        private bool checkdoamo(string mo_number)
        {
            Result_checkdoamo = true;

            query_sql = "select sap_mo_type from sfism4.r_bpcs_moplan_t where mo_number='" + mo_number + "'";
            var result_bpcs = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result_bpcs.Data != null)
            {
                if (result_bpcs.Data["sap_mo_type"].ToString() == "ZA13")
                    Result_checkdoamo = true;
                else
                    Result_checkdoamo = false;
            }
            return Result_checkdoamo;
        }
        public async Task<bool> checkweight(string mcarton_no)
        {
            var _data = new
            {
                TYPE = "CHECKWEIGHT",
                PRG_NAME = "PACK_PALT",
                CARTON = mcarton_no
            };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');
                if (_RESArray[0] == "OK")
                {

                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.errorcode = _RESArray[1].ToString();
                        _smf.ShowDialog();
                        return false;
                    }
                    else if ((_RESArray[0] == "NG1"))
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.CustomFlag = true;
                        _smf.MessageVietNam = _RESArray[1].ToString();
                        _smf.MessageEnglish = _RESArray[1].ToString();
                        _smf.ShowDialog();
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = _sfcHttpClient;
                        _smf.CustomFlag = true;
                        _smf.MessageVietNam = _RESArray[1].ToString();
                        _smf.MessageEnglish = "procedure have exceptions:";
                        _smf.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = _sfcHttpClient;
                _smf.CustomFlag = true;
                _smf.MessageVietNam = ex.Message;
                _smf.MessageEnglish = "Call procedure have exceptions:";
                _smf.ShowDialog();
                return false;
            }

        }

        public string GetPubMessage(string PROMPT_CODE)
        {
            string SLANGUAGE = "";
            SLANGUAGE = MES.OpINI.IniUtil.ReadINI(strINIPath, "LANGUAGES", "LANGUAGE");
            if (!string.IsNullOrWhiteSpace(SLANGUAGE)) PROMPT_CODE = SLANGUAGE + PROMPT_CODE;
            var result_procedure = _sfcHttpClient.Execute(new QuerySingleParameterModel
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

            str_GetPubMessage = ads[0]["res"];
            return str_GetPubMessage;
        }
        public bool ChkInput_data_valid(string str)
        {
            string[] arr = { "NA", "N/A", "" };
            bool check = arr.Contains(str);
            return check;
        }
        public bool getMcarton_no()
        {
            str_sql_query = "SELECT distinct(MCARTON_NO) as MCARTON_NO FROM SFISM4.R_WIP_TRACKING_T "
                              + " WHERE MCARTON_NO= '" + txt_mcarton_no + "' "
                              + " OR  CARTON_NO='" + txt_mcarton_no + "'";
            try
            {
                var result_query_MODEL_SERIAL = _sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                {
                    CommandText = str_sql_query,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (result_query_MODEL_SERIAL.Data != null)  ///(result_query_MODEL_SERIAL.Data != null)
                {
                    txt_mcarton_no = result_query_MODEL_SERIAL.Data["mcarton_no"].ToString();
                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                return false;

            }
        }
        private void itemPrinter_Click(object sender, RoutedEventArgs e)
        {
            itemPrinter.IsChecked = true;
            itemDontPrint.IsChecked = false;
            BarCodeItem.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "PRINTER", "PRINTER");
        }

        private void BarCodeItem_Click(object sender, RoutedEventArgs e)
        {
            itemPrinter.IsChecked = false;
            itemDontPrint.IsChecked = false;
            BarCodeItem.IsChecked = true;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "PRINTER", "BarCode");
        }

        private void itemDontPrint_Click(object sender, RoutedEventArgs e)
        {
            itemPrinter.IsChecked = false;
            itemDontPrint.IsChecked = true;
            BarCodeItem.IsChecked = false;
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "PRINTER", "DontPrint");
        }

        private void ButtonPopUp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Setup_Station_Click(object sender, RoutedEventArgs e)
        {
            Input_Carton.IsEnabled = false;
            loadformsetupstation();
        }

        public static string SubStringPlus(string input, int start, int length)
        {
            if (start == 0) throw new Exception("index start begin with 1 -- vi tri bat dau phai tu 1");
            start = start - 1;
            if (length > input.Length) length = input.Length;
            input = input.Substring(start, length);
            return input;
        }
        public static string getInputdataWithINI(string INPUTDATA, string SN_NAME)
        {
            string result = INPUTDATA;
            INIFile ini = new INIFile("C:\\Windows\\SFIS.ini");
            string tmpSnstart = Cut_BarcodeForm.SN;
            string tmpsnend = Cut_BarcodeForm.SNto;
            int intStart, intEnd;
            if (tmpSnstart == "0" || tmpSnstart == "" || tmpsnend == "0" || tmpsnend == "") return result;
            if (tmpsnend != "")
            {
                if (!int.TryParse(tmpSnstart, out intStart) || !int.TryParse(tmpsnend, out intEnd))
                {
                    throw new ArgumentException("Set SN Loction error , please set again -- SET SN location loi, vui long cai dat lai");
                }
                result = SubStringPlus(INPUTDATA, intStart, intEnd);
            }
            return result;
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

    }
    public class LISTCARTON
    {
        public String mcarton { get; set; }
    }
}
