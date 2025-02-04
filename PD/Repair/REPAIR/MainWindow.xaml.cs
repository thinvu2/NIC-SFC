using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Net.NetworkInformation;
using System.IO;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Text.RegularExpressions;
using System.Diagnostics;
using REPAIR.Models;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Data;
using MES.OpINI;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Timers;
using System.Windows.Threading;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SfcHttpClient sfcHttpClient;
        public string[] RESArray = { "NULL" };
        public static Oracle oracle = null;
        public static string inputLogin, checkSum, baseUrl, loginDB, empNo, empName, strMACHINE, empPass, prgName, appVer, MACAddress, IP, DataInput
                            , iniLeadFree, strINIPath, strsql, DataReasonClose = "", RepairStatus, RepairType, EC_ERROR_CODE, F_GROUP_NEXT = "", ReasonCode, ExcelFile="N";
        public static bool CheckLogin = false, rgrpAllErr = false, Modify_Status, fAddEC ;
        public int M_iIF_Repair, Repair_CNT;
        private char charSub = IniUtil.CharSub;
        DataTable dtMain;
        DateTime dateTime = DateTime.UtcNow.Date;

        public MainWindow()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            var _timer = new System.Windows.Forms.Timer();
            _timer.Tick += new EventHandler(ComponentDispatcher_ThreadIdle);
            _timer.Interval = 1000;
            _timer.Start();

        }
        private void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
        {
            var idleTime = IdleTimeDetector.GetIdleTimeInfo();
            if (idleTime.IdleTime.Seconds >= 600)
            {
                Environment.Exit(0);
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!await AccessAPI())
                {
                    Environment.Exit(0);
                }
                MACAddress = GetMacAddress();
                IP = GetLocalIPAddress();
                lblIP.Content = IP;
                lblMAC.Content = MACAddress;
                strMACHINE = "IP: " + IP + ";MAC: " + MACAddress;
                //btnMain.Background = Brushes.OrangeRed;
                //btnMain.Foreground = Brushes.White;
                loadIniFile();

                if (empNo == "PD")
                {
                    Login frmLogin = new Login();
                    frmLogin.sfcHttpClient = sfcHttpClient;
                    frmLogin.ShowDialog();
                    lblEmpName.Content = empName;
                }
                else
                {
                    if (!await Login())
                    {
                        this.Close();
                    }
                }

                if (!await getSN())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    
  
        public void loadIniFile()
        {
            try
            {
                strINIPath = "C:\\Windows\\SFIS.INI";

                if (!File.Exists(strINIPath))
                {
                    File.Create(strINIPath);
                }

                iniLeadFree = MES.OpINI.IniUtil.ReadINI(strINIPath, "REPAIR", "LEAD_FREE", "");
                if (iniLeadFree == "")
                {
                    lblLeadFree.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public async Task<Boolean> Login()
        {
            try
            {
                sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                await sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                oracle = new Oracle(sfcHttpClient);
                EmpModel emp = await oracle.GetObj<EmpModel>($"select * from SFIS1.c_emp_desc_t where emp_no = '" + empNo + "' ");
                if (emp != null)
                {
                    if (await checkPrivilege(empPass, "REPAIR_NEW"))
                    {
                        lblEmpName.Content = emp.emp_name;
                        return true;
                    }
                    else
                    {
                        showError("Employee No PRIVILEGE !\n  GROUP_NAME = REPAIR", "Tài khoản không có quyền !", true);
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


                //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
                //{
                //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                //    Environment.Exit(0);
                //}

                sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                await sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                oracle = new Oracle(sfcHttpClient);
                prgName = "REPAIR";
                appVer = getRunningVersion().ToString();
                mainWindow.Title = "Repair ver: " + appVer;
                lblVerson.Content = appVer;
                lblDBName.Content = loginDB.ToUpper();

                string sql = "SELECT VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='REPAIR' AND VR_CLASS='SUBSTRING'";
                var checkChar = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (checkChar.Data != null)
                {
                    IniUtil.CharSub = (checkChar.Data["vr_value"].ToString())[0];
                }
                else
                {
                    IniUtil.CharSub = '#';
                }
                charSub = IniUtil.CharSub;

                //if (!await CheckAppVersion(prgName, appVer))
                //{
                //    Environment.Exit(0);
                //}
                return true;
            }
            catch (Exception ex)
            {
                showError("AccessAPI have exception !!", ex.Message, true);
                return false;
            }
        }

        private string GetMacAddress()
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "";
            }
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
            try
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        private void showError(string strVN, string strEng, bool Flag)
        {
            MessageError FrmMessage = new MessageError();
            FrmMessage.sfcHttpClient = sfcHttpClient;
            if (Flag)
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.MessageVietNam = strVN;
                FrmMessage.MessageEnglish = strEng;
            }
            else
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.errorcode = strEng;
            }

            FrmMessage.ShowDialog();
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
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            for (i = 0; i <= macAddress.Length - 2; i = i + 2)
            {
                if (i == macAddress.Length - 2)
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

        public async Task<bool> checkPrivilege(string EmpPass, string FuntionCheck)
        {
            var logInfo = new
            {
                OPTION = "CHECKPRIVE",
                PASS = EmpPass,
                FUNTION = FuntionCheck
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            try
            {
                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPAIR_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
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
                    RESArray = RES.Split(charSub);
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
            catch (Exception aa)
            {
                return false;
            }

        }
        private bool _altModifierPressed = false;
        private void mainWindow_Keydown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.M:
                    btnModify_Click(sender, e);
                    break;
                //case Key.I:
                //    itemInputSNbyXLS_Click(sender, e);
                //    break;

            }

            _altModifierPressed = (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) ;

            if (_altModifierPressed && Keyboard.IsKeyDown(Key.I))
            {
                itemInputSNbyXLS_Click(sender, e);
            }
        }

        private async Task<Boolean> getSN()
        {
            DataInput = "";
            while (DataInput == "")
            {
                frmInputData frmInput = new frmInputData();
                var funtionInput = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='REPAIR' AND VR_VALUE='YES' ",
                    SfcCommandType = SfcCommandType.Text
                });
                if (funtionInput.Data != null && funtionInput.Data.Count() > 0)
                {
                    frmInput.cbbFuntion.Visibility = Visibility.Visible;
                }
                else
                {
                    frmInput.cbbFuntion.Visibility = Visibility.Hidden;
                }
                frmInput.ShowDialog();

                if (DataInput.Length > 5 && DataInput.Substring(0, 5) == "CHECK")
                {
                    frmCheckInOut frmInOut = new frmCheckInOut();
                    frmInOut.sfcHttpClient = sfcHttpClient;
                    frmInOut.tblStatus.Text = DataInput.Replace("_", " ");
                    frmInOut.C_Emp_No = empNo;
                    frmInOut.ShowDialog();
                }
                else
                {
                    if (DataInput.Length == 0)
                    {
                        return true;
                    }
                    DataInput = await checkDataSN(DataInput);
                }
            }
            return true;

        }

        public async Task<string> checkDataSN(string SN)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "CheckDataInput",
                    LEAD_FREE = iniLeadFree,
                    DATA = SN,
                    EMP = empNo
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPAIR_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
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
                    RESArray = RES.Split(charSub);
                    if (RESArray[0] == "OK")
                    {
                        if (RESArray[1] == "unknown")
                        {
                            return "";
                        }
                        else
                        {
                            Repair_CNT = int.Parse(RESArray[3].ToString());
                            strsql = "SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + RESArray[1] + "' ";
                            var result107 = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strsql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (result107.Data != null && result107.Data.Count() > 0)
                            {
                                tblSN.Text = result107.Data["serial_number"]?.ToString() ?? "";
                                tblS_SN.Text = result107.Data["shipping_sn"]?.ToString() ?? "";
                                tblMO.Text = result107.Data["mo_number"]?.ToString() ?? "";
                                tblLine.Text = result107.Data["line_name"]?.ToString() ?? "";
                                tblLine.Text = result107.Data["line_name"]?.ToString() ?? "";
                                tblModel.Text = result107.Data["model_name"]?.ToString() ?? "";
                                tblPartNO.Text = result107.Data["key_part_no"]?.ToString() ?? "";
                                tblVersion.Text = result107.Data["version_code"]?.ToString() ?? "";
                                tblGroup.Text = result107.Data["group_name"]?.ToString() ?? "";
                                tblInLine.Text = result107.Data["in_line_time"]?.ToString() ?? "";
                                tblStation.Text = result107.Data["in_station_time"]?.ToString() ?? "";
                                tblPMCC.Text = result107.Data["pmcc"]?.ToString() ?? "";
                                tblWipGroup.Text = result107.Data["wip_group"]?.ToString() ?? "";
                                tblRoute.Text = result107.Data["special_route"]?.ToString() ?? "";
                            }
                            else
                            {
                                showError("SN: " + RESArray[1] + " Không tìm thấy dữ liệu trong R107 ", "Not found data in R107", true);
                                return "";
                            }

                            if (!await getErrorList())
                            {
                                return "";

                            }
                            else
                            {
                                return SN;
                            }
                        }
                    }
                    else
                    {
                        if (RESArray[1] == "MS")
                        {
                            showError("3.SFIS1.REPAIR_API_EXECUTE / CheckDataInput", RESArray[2], true);
                            return "";
                        }
                        else
                        {
                            showError("4.SFIS1.REPAIR_API_EXECUTE / CheckDataInput", RESArray[2], false);
                           return  "";
                        }
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                showError("1.Get repair information have exception ", ex.Message, true);
                return "";
            }
        }


        private async Task<Boolean> getAndCheckRepairData(string SN ,string error ,string station , string time)
        {
            try
            {
                strsql = string.Format(sqlStr.getDataRepair, tblSN.Text,error,station,time);
                var result109 = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strsql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result109.Data != null && result109.Data.Count() > 0)
                {
                    string json = JsonConvert.SerializeObject(result109.Data);
                    dtMain = JsonConvert.DeserializeObject<DataTable>(json);

                    gridDataRepair.DataContext = dtMain;

                    dynamic data3 = result109.Data;
                    gridDataRepair.SelectedIndex = 0;
                }
                else
                {
                    btnAdd.IsEnabled = false;
                    btnRemove.IsEnabled = false;
                    btnModify.IsEnabled = false;
                    itemAddErrorCode.IsEnabled = false;
                    itemDeleteErrorCode.IsEnabled = false;
                    itemModifyWip.IsEnabled = false;
                }

                return true;
            }
            catch (Exception ex)
            {
                showError("2.Get repair data have exception !", ex.Message, true);
                return false;
            }
        }
        private void dgr_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = " " +  e.PropertyName.ToUpper().Replace("_", " ") + "  ";
      
        }
        private bool  closeCheck ()
        {
            if (tblSN.Text != "" && (lblQtyError.Content.ToString() != "0" || lblQtyError.Content.ToString() == ""))
            {
                showError("Not finish all errors! If you want Exit, you must Press Close button first.", "Chưa kết thúc tất cả các lỗi! Nếu muốt thoát , bạn cần nhấn vào nút ấn CLOSE trước khi thoát ", true);
                return false;
            }
            return true;

        }

        private void btnExit_Click_1(object sender, RoutedEventArgs e)
        {
            if (! closeCheck())
            {
                return;
            }  
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private async Task<Boolean> getErrorDesc(string error_code )
        {
            var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT * FROM SFIS1.C_ERROR_CODE_T WHERE ERROR_CODE ='" + error_code + "' ",
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                tbDescription.Text = result.Data["error_desc"].ToString();
            }
            return true;
        }

        private async void itemChangeKeyPart_Click(object sender, RoutedEventArgs e)
        {
            if (tblSN.Text == "")
            {
                RemoveKP frmRemoveKP = new RemoveKP();
                frmRemoveKP.EMP_NO = empNo;
                frmRemoveKP.MACIP = lblIP.Content.ToString() + "," +  lblMAC.Content.ToString();
                frmRemoveKP.sfcHttpClient = sfcHttpClient;
                frmRemoveKP.ShowDialog();
            }
            else
            {
                string sqlstr = string.Format(sqlStr.qryR108, tblSN.Text);
                var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlstr,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null && result.Data.Count() > 0)
                {
                    RemoveKP frmRemoveKP = new RemoveKP();
                    frmRemoveKP.tbInput.Text = tblSN.Text;
                    frmRemoveKP.EMP_NO = empNo;
                    frmRemoveKP.MACIP = "IP: " + lblIP.Content.ToString() + ", MAC: " + lblMAC.Content.ToString();
                    frmRemoveKP.sfcHttpClient = sfcHttpClient;
                    frmRemoveKP.ShowDialog();
                }
                else
                {
                    showError("This SN: " + tblSN.Text + " no have KeyParts", "", true);
                    return;
                }
            }
        }

        private async void itemInputSNbyXLS_Click(object sender, RoutedEventArgs e)
        {
            string SN, Result;
            int rCnt;
            int rw = 0;
            int cl = 0;
            itemInputSNbyXLS.IsChecked = true;
            ExcelFile = "Y";
            OpenFileDialog file = new OpenFileDialog();
            if (file.ShowDialog() == true)
            {
                string fileExt = System.IO.Path.GetExtension(file.FileName);
                if (fileExt.CompareTo(".xls") == 0 || fileExt.CompareTo(".xlsx") == 0)
                {
                    try
                    {
                        Excel.Application xlApp;
                        Excel.Workbook xlWorkBook;
                        Excel.Worksheet xlWorkSheet;
                        Excel.Range range;

                        xlApp = new Excel.Application();
                        xlWorkBook = xlApp.Workbooks.Open(file.FileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                        range = xlWorkSheet.UsedRange;
                        rw = range.Rows.Count;
                        cl = range.Columns.Count;


                        for (rCnt = 1; rCnt <= rw; rCnt++)
                        {
                            SN = (string)(range.Cells[rCnt, 1] as Excel.Range).Value2;
                            ReasonCode = (string)(range.Cells[rCnt, 2] as Excel.Range).Value2;
                            if (!string.IsNullOrEmpty(SN))
                            {
                                Result = await checkDataSN(SN);
                                if (Result != "")
                                {
                                    if (!string.IsNullOrEmpty(ReasonCode))
                                    {
                                        await funModify();

                                    }
                                    else
                                    {
                                        MessageBox.Show("Không tìm thấy mã lỗi tại vị trí: Cột 2 - Dòng " + rCnt + " - "+ SN + "","ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                    }

                                }
                            }
                        }

                        xlWorkBook.Close(true, null, null);
                        xlApp.Quit();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Please choose .xls or .xlsx file only.", "Warning", MessageBoxButton.OK, MessageBoxImage.Error); //custom messageBox to show error
                }
            }
            itemInputSNbyXLS.IsChecked = false;
            ExcelFile = "N";
            if (!await getSN())
            {
            }
        }

        private async void listErrorCode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int i;

            string ER = listErrorCode.SelectedValue?.ToString() ?? "";
            if (ER != "")
            {
                tblDefectTime.Text = listTestTime.Items[listErrorCode.SelectedIndex].ToString();
                tblDfstation.Text = listTestStation.Items[listErrorCode.SelectedIndex].ToString();
                tbErroCode.Text = ER.Replace("System.Windows.Controls.ListBoxItem: ", "");
                if (!await getErrorDesc(tbErroCode.Text))
                {

                }
                if (!await getAndCheckRepairData(tblSN.Text, tbErroCode.Text, tblDfstation.Text, tblDefectTime.Text))
                {
                    DataInput = "";
                }
            }
        }

        private void itemUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            frmUpdateStatus frmStatus = new frmUpdateStatus();
            frmStatus.sfcHttpClient = sfcHttpClient;
            frmStatus.charSub = charSub;
            frmStatus.ShowDialog();
        }

        private async void itemInputSN_Click(object sender, RoutedEventArgs e)
        {
            if (!await getSN())
            {

            }
        }

        private void listErrorCode_MouseLeave(object sender, MouseEventArgs e)
        {
            listErrorCode.SelectedIndex = -1;
        }

        private async Task<Boolean> getErrorList()
        {
            try
            {
                listErrorCode.Items.Clear();
                listRecordType.Items.Clear();
                listTestTime.Items.Clear();
                listTestStation.Items.Clear();
                if (rgrpAllErr)
                {
                    strsql = string.Format(sqlStr.qryAllError, tblSN.Text);
                }
                else
                {
                    strsql = string.Format(sqlStr.qryListNormalError, tblSN.Text, tblWipGroup.Text, tblRoute.Text);
                }
                var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strsql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null && result.Data.Count() > 0)
                {
                    foreach (var row in result.Data)
                    {
                        listRecordType.Items.Add(row["record_type"]?.ToString() ?? "" );
                        listTestTime.Items.Add(row["test_time"]?.ToString() ?? "" );
                        listTestStation.Items.Add(row["test_station"]?.ToString() ?? "");
                        if ((row["reason_code"]?.ToString() ?? "") == "")
                        {

                            listErrorCode.Items.Add(new ListBoxItem { Content = row["test_code"].ToString(), Background = Brushes.Red, Foreground = Brushes.Yellow });

                        }
                        else
                        {
                            
                            listErrorCode.Items.Add( new ListBoxItem { Content = row["test_code"]?.ToString(), Background = Brushes.Green, Foreground = Brushes.Yellow });
                            //listErrorCode.SelectedValue = row["test_code"]?.ToString();
                        }

                    }
                    listErrorCode.SelectedIndex = listErrorCode.Items.Count - 1;
                    lblQtyError.Content = listErrorCode.Items.Count.ToString();

                    int i;

                    string ER = listErrorCode.SelectedValue?.ToString() ?? "";
                    if (ER != "")
                    {
                        tblDefectTime.Text = listTestTime.Items[listErrorCode.SelectedIndex].ToString();
                        tblDfstation.Text = listTestStation.Items[listErrorCode.SelectedIndex].ToString();
                        tbErroCode.Text = ER.Replace("System.Windows.Controls.ListBoxItem: ", "");
                        if (!await getErrorDesc(tbErroCode.Text))
                        {

                        }
                        if (!await getAndCheckRepairData(tblSN.Text, tbErroCode.Text, tblDfstation.Text, tblDefectTime.Text))
                        {
                            DataInput = "";
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                showError("Get list error code have exception", ex.Message, true);
                return false;
            }
        }

        private async void listErrorCode_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //int i;
      
            //string ER = listErrorCode.SelectedValue?.ToString() ?? "";
            //if (ER != "")
            //{
            //    tblDefectTime.Text = listTestTime.Items[listErrorCode.SelectedIndex].ToString();
            //    tblDfstation.Text = listTestStation.Items[listErrorCode.SelectedIndex].ToString();
            //    tbErroCode.Text = ER.Replace("System.Windows.Controls.ListBoxItem: ", "");
            //    if (!await getErrorDesc(tbErroCode.Text))
            //    {

            //    }
            //    if (!await getAndCheckRepairData(tblSN.Text, tbErroCode.Text, tblDfstation.Text, tblDefectTime.Text))
            //    {
            //        DataInput = "";
            //    }
            //}
        }

        private void gridDataRepair_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var index = e.Row.GetIndex();
            var row = e.Row;
            int i = 0;
            var bc = new BrushConverter();
            foreach (DataRow rows in dtMain.Rows )
            {
                if (i == index)
                {
                    string repairer = rows["repairer"]?.ToString() ?? "";
                    if (rows["reason_code"] is null || rows["reason_code"].ToString() == "")
                    {
                        row.Background = Brushes.OrangeRed;  //(Brush)bc.ConvertFrom("#FFECC287");  //(Brush)bc.ConvertFrom("#FFFFA0");
                        row.Foreground = Brushes.Yellow; 
                    }
                    else
                    {
                        row.Background = Brushes.Green;  //FF369B5B
                        row.Foreground = Brushes.White; //(Brush)bc.ConvertFrom("#FFB00404");
                    }
                    break;
                }
                i++;
            }
       
            // string gg = gridDataRepair.I
            //var person = row.DataContext;

            //if (sender == gridDataRepair)
            //{
            //       row.Background = new SolidColorBrush(Colors.Red);

            //}
        }

        private void itemModifyWip_Click(object sender, RoutedEventArgs e)
        {
            btnModify_Click(sender, e);
        }

        private void itemRepairReport_Click(object sender, RoutedEventArgs e)
        {
            formReport frmReport = new formReport();
            frmReport.sfcHttpClient = sfcHttpClient;
            frmReport.ShowDialog();
        }

        private void itemCheckIn_Click(object sender, RoutedEventArgs e)
        {
            frmCheckInOut frmCheckIN = new frmCheckInOut();
            frmCheckIN.sfcHttpClient = sfcHttpClient;
            frmCheckIN.tblStatus.Text = "CHECK IN";
            frmCheckIN.itemCheckIn.IsChecked = true;
            frmCheckIN.ShowDialog();
        }

        private void itemCheckOut_Click(object sender, RoutedEventArgs e)
        {
            frmCheckInOut frmCheckOut = new frmCheckInOut();
            frmCheckOut.sfcHttpClient = sfcHttpClient;
            frmCheckOut.tblStatus.Text = "CHECK OUT";
            frmCheckOut.itemCheckIn.IsChecked = true;
            frmCheckOut.ShowDialog();
        }

        public class Person
        {
            public string REPAIRER { get; set; }
        }

 

        private async void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (!await FinishRepair())
            {
                return ;
            }

        }

        private async Task<Boolean> FinishRepair()
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "FinishRepair",
                    DATA = tblSN.Text,
                    GROUP_NEXT = F_GROUP_NEXT,
                    TEST_TIME = tblDefectTime.Text,
                    TEST_STATION = tblDfstation.Text,
                    TEST_CODE = tbErroCode.Text
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
               
                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPAIR_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
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
                    RESArray = RES.Split(charSub);
                    if (RESArray[0] == "OK")
                    {
                        MessageBox.Show("This machine has repaired OK! Transfer to " + RESArray[1]?.ToString() ?? "", "Thông báo" , MessageBoxButton.OK,MessageBoxImage.Information);
                        F_GROUP_NEXT = "";
                        RefreshAll();
                        if (itemInputSNbyXLS.IsChecked  == false)
                        {
                            if (!await getSN())
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        if (RESArray[1] == "RETURN_STATION")
                        {
                            frmSelectGroupNext frmGroupNext = new frmSelectGroupNext();
                            frmGroupNext.sfcHttpClient = sfcHttpClient;
                            frmGroupNext.group = RESArray[2].ToString();
                            frmGroupNext.route = RESArray[3].ToString();
                            frmGroupNext.C_SN = tblSN.Text;
                            frmGroupNext.ShowDialog();
                            if (!await FinishRepair())
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if (RESArray[1] == "MS")
                        {
                            showError("1.SFIS1.REPAIR_API_EXECUTE / FinishRepair", RESArray[2], true);
                            DataInput = "";
                        }
                        else
                        {
                            showError("2.SFIS1.REPAIR_API_EXECUTE / FinishRepair", RESArray[2], false);
                            DataInput = "";
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        private async Task<bool> CheckFinishReapair (string data)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "CheckFinishAll",
                    DATA = data
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPAIR_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
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
                    RESArray = RES.Split(charSub);
                    if (RESArray[0] == "OK")
                    {
                        return true;
                    }
                }  
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = gridDataRepair.SelectedItem as DataRowView;
                if (row != null)
                {
                    if ((row.Row.ItemArray[9]?.ToString() ?? "") != "T")
                    {
                        MessageBoxResult dlr = MessageBox.Show("Do you want to delete this repair record? ", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (dlr == MessageBoxResult.OK)
                        {
                            strsql = "Delete from sfism4.r_repair_t where serial_number ='" + tblSN.Text + "' AND  ROWID ='" + row.Row.ItemArray[22]?.ToString() + "' ";
                            var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = strsql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (result.Result.ToString() == "OK")
                            {
                                MessageBox.Show("Delete success !!", "Information", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                return;
                            }
                        }
                    }
                    else
                    {
                        showError("Can not delete this record,because it was record by test station?", "Không thể xóa lỗi này !!", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                showError("btnDelete have exception!!", ex.ToString(), true);
                return;
            }
        }

        private async void btnNew_Click(object sender, RoutedEventArgs e)
        {
            formAddError frmAddEC = new formAddError();
            frmAddEC.sfcHttpClient = sfcHttpClient;
            frmAddEC.ShowDialog();
            if (fAddEC)
            {
                var logInfo = new
                {
                    OPTION = "AddEC",
                    DATA = tblSN.Text,
                    ERROR_CODE = EC_ERROR_CODE,
                    EMP = empNo
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
                try
                {
                    var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.REPAIR_API_EXECUTE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
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
                        RESArray = RES.Split(charSub);

                        if (RESArray[0] == "OK")
                        {
                            if (!await getErrorList())
                             return ;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return ;
                    }
                }
                catch
                {
                    return ;
                }
            }
        }

        private void gridDataRepair_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridDataRepair.Items.Count > 0 )
            {
                if (!CheckRepairData())
                {
                    return;
                }
            }
        }
        private bool CheckRepairData ()
        {
            try
            {
                string Repairer = "", RepairTime = "", ReasonCode = "", RepairStatus = "";
                DataRowView row = gridDataRepair.SelectedItem as DataRowView;
                if (row != null)
                {
                    RepairTime = row.Row.ItemArray[0]?.ToString() ?? "";
                    ReasonCode = row.Row.ItemArray[1]?.ToString() ?? "";
                    Repairer = row.Row.ItemArray[21]?.ToString() ?? "";
                    RepairStatus = row.Row.ItemArray[8]?.ToString() ?? "";

                    if ((RepairTime == "") || (DateTime.Parse(RepairTime) >= DateTime.Now))
                    {
                        if (gridDataRepair.Items.Count == 1)
                        {
                            if ((ReasonCode is null) || (ReasonCode == ""))
                            {
                                btnAdd.IsEnabled = false;
                                btnDelete.IsEnabled = false;
                                itemAddErrorCode.IsEnabled = false;
                                itemDeleteErrorCode.IsEnabled = false;
                            }
                            else
                            {
                                btnAdd.IsEnabled = true;
                                btnDelete.IsEnabled = false;
                                itemAddErrorCode.IsEnabled = true;
                                itemDeleteErrorCode.IsEnabled = false;
                            }

                            if ((Repairer == "")
                                || (Repairer == empNo)
                                || (Repairer == empPass))
                            {
                                itemModifyWip.IsEnabled = true;
                                btnModify.IsEnabled = true;
                            }
                            else
                            {
                                itemModifyWip.IsEnabled = false;
                                btnModify.IsEnabled = false;
                            }
                        }
                        else
                        {
                            if (RepairStatus == "N")
                            {
                                btnAdd.IsEnabled = true;
                                btnDelete.IsEnabled = false;
                                itemAddErrorCode.IsEnabled = true;
                                itemDeleteErrorCode.IsEnabled = false;
                                if ((Repairer == "")
                                    || (Repairer == empNo)
                                    || (Repairer == empPass))
                                {
                                    itemModifyWip.IsEnabled = true;
                                    btnModify.IsEnabled = true;
                                }
                                else
                                {
                                    itemModifyWip.IsEnabled = false;
                                    btnModify.IsEnabled = false;
                                }
                            }
                            else if ((RepairStatus is null) || (RepairStatus == "") || (RepairStatus == "D"))
                            {
                                btnAdd.IsEnabled = true;
                                btnDelete.IsEnabled = true;
                                itemAddErrorCode.IsEnabled = true;
                                itemDeleteErrorCode.IsEnabled = true;

                                if ((Repairer == "")
                                    || (Repairer == empNo)
                                    || (Repairer == empPass))
                                {
                                    itemModifyWip.IsEnabled = true;
                                    btnModify.IsEnabled = true;
                                }
                                else
                                {
                                    itemModifyWip.IsEnabled = false;
                                    btnModify.IsEnabled = false;
                                }
                            }
                            else
                            {
                                btnAdd.IsEnabled = false;
                                btnDelete.IsEnabled = false;
                                itemAddErrorCode.IsEnabled = false;
                                itemDeleteErrorCode.IsEnabled = false;
                                itemModifyWip.IsEnabled = false;
                                btnModify.IsEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        btnAdd.IsEnabled = false;
                        btnDelete.IsEnabled = false;
                        btnModify.IsEnabled = false;
                        itemAddErrorCode.IsEnabled = false;
                        itemDeleteErrorCode.IsEnabled = false;
                        itemModifyWip.IsEnabled = false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                showError("5.Check Repair Data have Exception !", ex.Message, true);
                return false;
            }

        }

        private async void btnModify_Click(object sender, RoutedEventArgs e)
        {
            await funModify();
        }

        public async Task funModify ()
        {
            try
            {
                if (gridDataRepair.Items.Count == 0)
                {
                    return;
                }
                RepairStatus = "";
                if (gridDataRepair.SelectedIndex < 0)
                {
                    gridDataRepair.SelectedIndex = 0;
                }
                DataRowView row = gridDataRepair.SelectedItem as DataRowView;
                frmModify fModify = new frmModify();
                fModify.sfcHttpClient = sfcHttpClient;
                fModify.tblLocation.Text = row.Row.ItemArray[3]?.ToString() ?? "";
                fModify.tblDescription2.Text = row.Row.ItemArray[4]?.ToString() ?? "";
                fModify.lblErrorCode.Content = tbErroCode.Text;
                fModify.lblErrorDesc.Content = tbDescription.Text;
                fModify.MODEL_NAME = tblModel.Text;
                fModify.lbGroup = tblGroup.Text;
                fModify.sRepairType = row.Row.ItemArray[8]?.ToString() ?? "";
                fModify.REPAIR_GROUP = tblWipGroup.Text;
                fModify.LINE_NAME = tblLine.Text;
                fModify.SN = tblSN.Text;
                fModify.F_ROWID = row.Row.ItemArray[22]?.ToString() ?? "";

                RepairType = row.Row.ItemArray[8]?.ToString() ?? "";
                if (RepairType == "")
                {
                    RepairStatus = "N";
                }
                else
                {
                    RepairStatus = RepairType;
                }
                fModify.tbFixReasonCode.Text = row.Row.ItemArray[1]?.ToString() ?? "";
                if ((row.Row.ItemArray[1]?.ToString() ?? "") != "")
                {
                    fModify.tblFixTime.Text = row.Row.ItemArray[0]?.ToString() ?? "";
                }
                else
                {
                    fModify.tblFixTime.Text = DateTime.Now.ToString();
                    M_iIF_Repair = 1;
                }

                fModify.tblFixDescription.Text = row.Row.ItemArray[2]?.ToString() ?? "";
                fModify.tblLocation.Text = row.Row.ItemArray[3]?.ToString() ?? "";
                fModify.tblFixDescription.Text = row.Row.ItemArray[4]?.ToString() ?? "";
                fModify.tblDutyType.Text = row.Row.ItemArray[5]?.ToString() ?? "";
                fModify.cbbDutyStation.Text = row.Row.ItemArray[7]?.ToString() ?? "";
                fModify.tbMemo.Text = row.Row.ItemArray[11]?.ToString() ?? "";
                fModify.cbbVender.Text = row.Row.ItemArray[12]?.ToString() ?? "";
                fModify.tbEcExt.Text = row.Row.ItemArray[13]?.ToString() ?? "";
                fModify.tbDC.Text = row.Row.ItemArray[23]?.ToString() ?? "";
                fModify.SECTION = row.Row.ItemArray[16]?.ToString() ?? "";
                //fModify.tblLC.Text = row.Row.ItemArray[24]?.ToString() ?? "";
                //fModify.tblPartNumber.Text = row.Row.ItemArray[25]?.ToString() ?? "";
                fModify.tReasonCode = ReasonCode;

                fModify.ShowDialog();
                if (Modify_Status)
                {
                    if (!await getAndCheckRepairData(tblSN.Text, tbErroCode.Text, tblDfstation.Text, tblDefectTime.Text))
                    {

                    }
                    if (!await getErrorList())
                    {

                    }

                    if (await CheckFinishReapair(tblSN.Text))
                    {
                        if (!await FinishRepair())
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                showError("Modify have exception!!", ex.ToString(), true);
                return;
            }
        }

        private async void listErrorCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private async void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmInputReason frmReason = new frmInputReason();
                frmReason.ShowDialog();
                if (DataReasonClose != "")
                {
                    //MessageBox.Show("Reason close can not null , Please input reason!!","Error", MessageBoxButton.OK,MessageBoxImage.Error );

                    strsql = " Insert into sfism4.r_repair_reason_t " +
                           " (serial_number, repair_cnt, reapir_reason) " +
                           " Values('" + tblSN.Text + "','" + (Repair_CNT + 1).ToString() + "','" + DataReasonClose + "' ) ";
                    var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (result.Result.ToString() == "OK")
                    {

                        RefreshAll();
                        if (!await getSN())
                        {
                            return;
                        }
                    }
                    else
                    {
                        showError("Save reason have error!!", result.Result.ToString(), true);
                        return;
                        // MessageBox.Show("Save reason have error!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                showError("Close have exception!!", ex.ToString(), true);
                return;
            }
        }

        private void RefreshAll()
        {
            listErrorCode.Items.Clear();
            gridDataRepair.DataContext = null;
            tblSN.Text = "";
            tblS_SN.Text = "";
            tblMO.Text = "";
            tblLine.Text = "";
            tblLine.Text = "";
            tblModel.Text = "";
            tblPartNO.Text = "";
            tblVersion.Text = "";
            tblGroup.Text = "";
            tblInLine.Text = "";
            tblStation.Text = "";
            tblPMCC.Text = "";
            tblWipGroup.Text = "";
            tblRoute.Text = "";
            lblQtyError.Content = "0";
            tblDefectTime.Text = "";
            tblDfstation.Text = "";
            tbErroCode.Text = "";
            tbDescription.Text = "";
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmModify fModify = new frmModify();
                fModify.sfcHttpClient = sfcHttpClient;
                fModify.lblErrorCode.Content = tbErroCode.Text;
                fModify.lblErrorDesc.Content = tbDescription.Text;
                fModify.MODEL_NAME = tblModel.Text;
                fModify.lbGroup = tblGroup.Text;
                fModify.REPAIR_GROUP = tblWipGroup.Text;
                fModify.LINE_NAME = tblLine.Text;
                fModify.SN = tblSN.Text;
                RepairStatus = "D";
                fModify.tblFixTime.Text = DateTime.Now.ToString();
                fModify.tbFixReasonCode.Text = "";
                fModify.tblFixDescription.Text = "";
                fModify.tblLocation.Text = "";
                fModify.tblDutyType.Text = "";
                fModify.cbbDutyDesc.Text = "";
                fModify.cbbDutyStation.Text = "";
                fModify.cbbVender.Items.Clear();
                fModify.tbEcExt.Text = "";
                fModify.tbMemo.Text = "";

                fModify.ShowDialog();
                if (Modify_Status)
                {
                    if (!await getAndCheckRepairData(tblSN.Text, tbErroCode.Text, tblDfstation.Text, tblDefectTime.Text))
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                showError("btnAdd have exception!!", ex.ToString(), true);
                return;
            }
        }

        private async void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string RecordType = "";

                string ER = listErrorCode.SelectedValue?.ToString() ?? "";
                ER = ER.Replace("System.Windows.Controls.ListBoxItem: ", "");
                if (ER != "")
                {
                    RecordType = listRecordType.Items[listErrorCode.SelectedIndex].ToString();

                    if (RecordType != "T")
                    {
                        MessageBoxResult dlr = MessageBox.Show("Do you want to delete this repair record? ", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (dlr == MessageBoxResult.OK)
                        {
                            strsql = "Delete from sfism4.r_repair_t where serial_number ='" + tblSN.Text + "' AND  test_code ='" + ER + "' AND test_station = '" + tblDfstation.Text + "' AND test_time = to_date('" + tblDefectTime.Text + "' ,'YYYY/MM/DD HH24:MI:SS') ";
                            var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = strsql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (result.Result.ToString() == "OK")
                            {
                                MessageBox.Show("Delete success !!", "Information", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                                if (!await getErrorList())
                                    return;
                            }
                        }
                    }
                    else
                    {
                        showError("Can not delete this record,because it was record by test station?", "Không thể xóa lỗi này !!", true);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                showError("btnRemove have exception!!", ex.ToString(), true);
                return;
            }
        }


    }
}
