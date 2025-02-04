using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
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
using LabelManager2;
using LOT_REPRINT.Resource;
using System.Management;
using System.Windows.Threading;
using System.Threading;
using System.Security.Cryptography;
using System.Media;
using System.Diagnostics;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Deployment.Application;

namespace LOT_REPRINT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer;
        private int count;
        public LabelManager2.Application labApp = null;
        public LabelManager2.Document doc = null;
        string labname = string.Empty;
        //ApplicationClass labApp = null;
        //Document doc = null;
        string COM_PORT;
        private SerialPort serialPort1 = new SerialPort();
        private SerialPort serialPort3 = new SerialPort();
        bool logic,check_roku_info;
        System.DateTime timelive;
        bool _ChkMD5Flag = true;
        public static string local_strIP, mo_date, time_string,prgVer;
        public string G_sLabl_Name, SPID, RECIPIENT_NAME, LOCATION, MMC_FLAG, DDR_NAME, SOC_NAME, CTN_NAME, ADD_CODE, SSN_INFO;
        public string paramData = "";
        string sql, model_type, M_MODEL_NAME, model_name, sTable = "", sFindData = "";
        string rmaMac, rmaSn, macID;
        Boolean notFull, PrintOK = false, printflag, err = false;
        private string data_com;
        public Boolean reprint_flag = false, locationlabel = false;
        string checkSum, loginApiUrl, loginDB, empNo, empPass, inputLogin;
        string SSN1, SSN2, CUST_SSN1, MO, Model_name, mo1, mo2, model1, model2, pubPo_no;
        string G_passEmp_No, FTP_label_path, G_update_Date, sLabelFileName, sMessage, M_sTRACK_NO;
        string sFileName = string.Empty;
        string My_LabelFileName, LabelFileName_ftp, pubftppath, publicfilepath = "";
        string M_VERSION_CODE = "", c_ssn = "";
        string My_MoNumber = "", MGROUP_NAME = "", PCMAC = "", tmp_msn = "";
        string SourceString, ftphost;
        string M_PMCC, M_PART_NO, M_CUST_CODE, M_sUPCEANData, M_sCustModelDesc, M_sCustModelName, sCarton, sPallet;
        int ISPRINTCOUNT, M_iCartonCapacity, M_iCSVersion;
        int PageQTY;
        //string MD5_STR, SSID1, Password1, Channel1;
        public string My_Section, M_Station, M_Group, M_Line;
        public static DataTable dtParams = new DataTable();
        SfcHttpClient sfcClient;
        public Boolean saveOK = false;
        

        public MainWindow()
        {
            checkConnect();
            DispatcherTimerSample();
            InitializeComponent();
            try
            {
                COM_PORT = GetSet.COMRECEIVE;
                serialPort1.BaudRate = 9600;
                serialPort1.Parity = Parity.None;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.PortName = COM_PORT;
                serialPort1.Handshake = Handshake.None;
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                M_Line = ini.IniReadValue("LOTREPRINT", "M_Line");
                if (!string.IsNullOrEmpty(M_Line))
                {
                    txtLine.Text = M_Line;
                    lbl3.Text = " - ";
                    M_Station = ini.IniReadValue("LOTREPRINT", "M_Station");
                    M_Group = ini.IniReadValue("LOTREPRINT", "M_Group");
                    txtStation.Text = M_Group;
                    txtLine.Visibility = Visibility.Visible;
                    lbl3.Visibility = Visibility.Visible;
                    txtStation.Visibility = Visibility.Visible;
                    My_Section = ini.IniReadValue("LOTREPRINT", "My_Section");
                }
                InputSN.IsChecked = true;
                prgVer = getRunningVersion().ToString();
            }
            catch(Exception ex)
            {
                showMessage(ex.Message, ex.Message, true);
            }
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
        public string GetHostMacAddress()
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
        public static IPAddress GetIPAddress()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses("");

            foreach (IPAddress hostAddress in hostAddresses)
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(hostAddress) &&
                    !hostAddress.ToString().StartsWith("169.254."))
                    return hostAddress;
            }
            return null;
        }

        public async Task<bool> checkPalletfull(string cData)
        {
            string sModel, sVer, sImei;
            int sQty, palletQty, sn_Qty, car_qty;
            sql = "SELECT * FROM SFISM4.R107 WHERE IMEI='" + cData + "' or serial_number='" + cData + "' or shipping_sn='" + cData + "'"
                + "or mcarton_no='" + cData + "' or carton_no='" + cData + "' or pallet_no='" + cData + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                sImei = _result.Data["imei"]?.ToString() ?? "";
                sModel = _result.Data["model_name"]?.ToString() ?? "";
                sVer = _result.Data["version_code"]?.ToString() ?? "";
                if (SSNLABEL.IsChecked)
                {
                    sql = "SELECT * FROM SFISM4.R107 WHERE IMEI='" + sImei + "'";
                }
                else
                {
                    sql = "SELECT DISTINCT MCARTON_NO FROM SFISM4.R107 WHERE IMEI='" + sImei + "'";
                }
                var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    sQty = result.Data.Count();
                    sql = "SELECT PALLET_QTY,CARTON_QTY FROM SFIS1.C_PACK_PARAM_T WHERE MODEL_NAME='" + sModel + "' AND VERSION_CODE='" + sVer + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        palletQty = Int32.Parse(_result.Data["pallet_qty"]?.ToString() ?? "");
                        car_qty = Int32.Parse(_result.Data["carton_qty"]?.ToString() ?? "");
                        sn_Qty = palletQty * car_qty;
                        if (SSNLABEL.IsChecked)
                        {
                            if (sQty < sn_Qty)
                            {
                                return true;
                            }
                            else
                            {
                                if (sQty < palletQty)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void showParam_Click(object sender, RoutedEventArgs e)
        {
            frmParam frmParam = new frmParam();
            frmParam.dataGridcsParam.DataContext = dtParams;
            if (frmParam.dataGridcsParam.DataContext == null)
            {
                return;
            }
            frmParam.sUrlFile = SourceString;
            frmParam.ShowDialog();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dir = "C:/NIC/LOT_Reprint";
                string[] listlink = Directory.GetFiles(dir, "*.lab");
                foreach (string link in listlink)
                {
                    File.Delete(link);
                }

                labApp.Quit();
            }
            catch
            { }
            killprocess();
            System.Windows.Application.Current.Shutdown();
        }
        private async Task btnOK_Click()
        {
            //Z: add for NicMode
            if (!NicMode.IsChecked) return;
            logic = true;
            string line="N/A";
            if (!string.IsNullOrEmpty(txtStation.Text))
            {
                line = txtLine.Text.Trim();
            }
            if (txtInput1.Text != "")
            {
                if (!printBoxLabelForEnskyRework.IsChecked)
                {
                    var regexItem = new Regex("^[A-Z0-9 --%/@:]*$");
                    if (!regexItem.IsMatch(txtInput1.Text))
                    {
                        showMessage("00340", "00340", false);
                        return;
                    }

                }
                if (Kind.SelectedItem == chkBoxLabel)
                {
                    string labelType = "LabelType:LT" + txtSeditLabelQty.Text + txtSeditfCount.Text + txtSedprtnow.Text;
                    string indata = labelType + "|SN:" + txtInput1.Text + "|SSN:" + txtInput2.Text + "|JUSTPRINT:" + txtSedprtnow.Text + "|LINE:" + line;
                    await call_SP(indata, "EXECUTE");
                    string _RES = _ads[1]["res"];
                    string _Param = _ads[0]["out_data"];

                    if (_RES.StartsWith("OK"))
                    {
                        if (_Param is null || _Param.ToString() == "")
                        {
                            showMessage("No param to label file,Call IT check");
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                            return;
                        }
                        dtParams.Clear();
                        foreach (var rows in _Param.Split('|'))
                        {
                            if (rows.IndexOf(':') != -1)
                            {
                                AddParam(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "", true);
                            }
                        }
                    }
                    else
                    {
                        showMessage(_RES);
                        txtInput1.SelectAll();
                        txtInput1.Focus();
                        return;
                    }

                    string ztemp = txtSeditLabelQty.Text.Trim() + txtSeditfCount.Text.Trim() + txtSedprtnow.Text.Trim();
                    //-----------------------------------------------------------
                    sql = "SELECT PO_NO,SHIPPING_SN,EMP_NO,BILL_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                    var _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result1.Data != null && !PrintTest.IsChecked)
                    {
                        tmp_msn = _result1.Data["bill_no"]?.ToString() ?? "";
                        if (tmp_msn.IndexOf("LT" + ztemp) != -1 )
                        {
                            frmLogin frmLogin = new frmLogin();
                            frmLogin.sfcClient = sfcClient;
                            frmLogin.type = "MSG";
                            if (frmLogin.emp_input != txtInput1.Text)
                            {
                                frmLogin.ShowDialog();
                                await saveReprintRd(frmLogin.emp_input, txtInput1.Text, ztemp);
                                if (!saveOK)
                                {
                                    return;
                                }
                            }
                        }
                    }
                    //-------------------------------------------------------------------------------

                    G_sLabl_Name = txtModelName.Text.Replace(".", "_") + "_BX.LAB";
                    if (txtSedprtnow.Text == "0" && txtSeditfCount.Text == "1")
                    {
                        sFindData = paramData;
                        await P_PrintToCodeSoft("C", G_sLabl_Name);

                    }
                    if (txtSeditfCount.Text != "1")
                    {
                        if (txtSedprtnow.Text == "0")
                        {
                            for (int m = 1; m <= Int32.Parse(txtSeditfCount.Text) - 1; m++)
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + m + ".Lab";
                                await P_PrintToCodeSoft("C", labname);
                            }
                        }
                        else
                        {
                            labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                            await P_PrintToCodeSoft("C", labname);
                        }
                    }
                    if (PrintOK)
                    {
                        sql = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "' and instr(bill_no,'LT"+ ztemp + "') > 0";
                        var _result2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result2.Data == null)
                        {
                            sql = "UPDATE SFISM4.R_WIP_TRACKING_T SET bill_no=bill_no||'LT" + ztemp + "' WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                            var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (query_update.Result.ToString() != "OK")
                            {
                                showMessage("Call IT check exception: " + query_update.Message.ToString() + "\nsql: " + sql);
                                return ;
                            }
                        }
                    }
                }
                else if(Kind.SelectedItem == chkCartonLabel)
                {
                    if (string.IsNullOrEmpty(M_Group))
                    {
                        showMessage("Need setup station first|Cần thiết lập tên trạm trước");
                        txtInput1.SelectAll();
                        txtInput1.Focus();
                        return;
                    }
                    line = txtLine.Text.Trim();

                    sql = "select * from sfism4.r107 where mcarton_no='" + txtInput1.Text + "'";
                    var ctnResult = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if(ctnResult.Data==null)
                    {
                        showMessage("CartonID not found|Không tìm thấy mã carton ");
                        return;
                    }

                    sql = "select * from sfism4.r117 where serial_number in(select serial_number from sfism4.r107 where mcarton_no= '" + txtInput1.Text + "')";
                    sql += " and mcarton_no= '" + txtInput1.Text + "' and group_name ='"+M_Group+"'";
                    ctnResult = null;
                    ctnResult = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (ctnResult.Data != null &&!PrintTest.IsChecked)
                    {
                        frmLogin frmLogin = new frmLogin();
                        frmLogin.sfcClient = sfcClient;
                        frmLogin.type = "MSG";
                        frmLogin.ShowDialog();
                        await saveReprintRd(frmLogin.emp_input, txtInput1.Text, "CTN");
                        if (!saveOK)
                        {
                            return;
                        }
                    }
                    string indata = "CARTONID:" + txtInput1.Text + "|LINE:" + line;
                    await call_SP( indata, "CARTON");
                    string _RES = _ads[1]["res"];
                    string _Param = _ads[0]["out_data"];

                    if (_RES.StartsWith("OK"))
                    {
                        if (_Param is null || _Param.ToString() == "")
                        {
                            showMessage("No param to label file,Call IT check");
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                            return;
                        }
                        dtParams.Clear();
                        foreach (var rows in _Param.Split('|'))
                        {
                            if (rows.IndexOf(':') != -1)
                            {
                                AddParam(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "", true);
                            }
                        }
                        if (!string.IsNullOrEmpty(G_sLabl_Name))
                        {
                            await P_PrintToCodeSoft("C", G_sLabl_Name);
                            txtInput1.Clear();
                            txtInput1.Focus();
                            return;
                        }
                        else
                        {
                            showMessage("Not found LabelName, call IT check");
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                            return;
                        }
                    }
                    else
                    {
                        showMessage(_RES);
                        txtInput1.SelectAll();
                        txtInput1.Focus();
                        return;
                    }
                }
                else if (Kind.SelectedItem == chkCombine)
                {
                    if (string.IsNullOrEmpty(M_Group))
                    {
                        showMessage("Need setup station first|Cần thiết lập tên trạm trước");
                        txtInput1.SelectAll();
                        txtInput1.Focus();
                        return;
                    }
                    line = txtLine.Text.Trim();

                    string indata = "SN:" + txtInput1.Text + "|LINE:" + line + "|SUB_FUNC:GET";
                    await call_SP(indata, "PrintCombine");
                    string _RES = _ads[1]["res"];
                    string _Param = _ads[0]["out_data"];

                    if (_RES.StartsWith("OK"))
                    {
                        if (_Param is null || _Param.ToString() == "")
                        {
                            showMessage("No param to label file,Call IT check");
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                            return;
                        }
                        dtParams.Clear();
                        foreach (var rows in _Param.Split('|'))
                        {
                            if (rows.IndexOf(':') != -1)
                            {
                                AddParam(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "", true);
                            }
                        }

                        sql = "select * from SFISM4.R_NIC_SN_T where mac1='"+ txtInput1.Text + "' and mac1<>ssn1 and print_flag='Y'";

                        var reprintResult = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (reprintResult.Data != null && !PrintTest.IsChecked)
                        {
                            frmLogin frmLogin = new frmLogin();
                            frmLogin.sfcClient = sfcClient;
                            frmLogin.type = "MSG";
                            frmLogin.ShowDialog();
                            await saveReprintRd(frmLogin.emp_input, txtInput1.Text, "Combine");
                            if (!saveOK)
                            {
                                return;
                            }
                        }

                        if (!string.IsNullOrEmpty(G_sLabl_Name))
                        {
                            await P_PrintToCodeSoft("C", G_sLabl_Name);
                            if (PrintOK && !PrintTest.IsChecked)
                            {
                                //Qua tram, reprint thi van phai rework va qua tram lai
                                indata = "SN:" + txtInput1.Text + "|LINE:" + line + "|SUB_FUNC:PASS";
                                await call_SP(indata, "PrintCombine");
                                _RES = _ads[1]["res"];
                                _Param = _ads[0]["out_data"];
                                if (_RES.StartsWith("OK"))
                                {
                                    txtInput1.Clear();
                                }
                                else
                                {
                                    showMessage(_RES);
                                    txtInput1.SelectAll();
                                    txtInput1.Focus();
                                    return;
                                }
                            }
                            else
                            {
                                txtInput1.SelectAll();
                            }
                            txtInput1.Focus();
                            return;
                        }
                        else
                        {
                            showMessage("Not found LabelName, call IT check");
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                            return;
                        }
                    }
                    else
                    {
                        showMessage(_RES);
                        txtInput1.SelectAll();
                        txtInput1.Focus();
                        return;
                    }
                }
                else
                {
                    showMessage("NicMode not apply for this function|NicMode không áp dụng cho chức năng này");
                }
            }
            SystemSounds.Beep.Play();
            txtInput2.Clear();
            txtInput1.Clear();
            txtInput1.Focus();
        }
        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            showMessage("Function closed by IT");
            return;
            logic = true;
            int InputQty;
            
            string c_model_seri = "";
            string c_serial = "";
            string c_special_route = "";
            string c_wip_group = "";
            string c_mo_number = "", c_ship_no, LOCATION, RECIPIENT_NAME, CTN_NAME, ADD_CODE, Result;
            string c_step_sequence, T_RES, c_step_sequence1, sMO_TYPE, c_invoice, po, tcom, SO_QTY, SHIP_TYPE,S_SHIP, batch, haveBatch, BATCH_TYPE, SO_PRE, endrange, TRUCK_NUMBER, sysdate, PO_CURRENT, PO_MAX;
            int i_step_sequence, i_step_sequence1, intX, i, j, iLBLQTY, iRQTY, a, b, k, c, h, total_ctn, total_pcs;
            bool sFindMcartonflag, ispass;

            txtInput1.Text = txtInput1.Text.ToUpper().Trim();
            #region oldCode temp close
            /*
            if (check_roku_info)  //(loginDB=="TEST") 
            {
                if(txtInput1.Text.Length == 65)
                {
                    txtInput1.Text = txtInput1.Text.Substring(0, 24).Trim();
                }

                if (txtInput1.Text.Length == 78)
                {
                    txtInput1.Text = txtInput1.Text.Substring(0, 24).Trim();
                }

                if (txtInput1.Text.IndexOf(":")!=-1)
                {
                    txtInput1.Text = txtInput1.Text.Substring(1, txtInput1.Text.IndexOf(":") - 1);
                }

                sql = "select * from sfism4.R_CUSTSN_T where SSN1 ='"+ txtInput1.Text + "'";
                 var _result_data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result_data.Data!=null)
                {
                    txtInput1.Text = _result_data.Data["serial_number"]?.ToString() ?? "";
                }

                sql = "select * from sfism4.R_CUSTSN_T where serial_number ='" + txtInput1.Text + "'";
                 _result_data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result_data.Data != null)
                {
                    SSN_INFO = _result_data.Data["ssn1"]?.ToString() ?? "";
                }


                sql = "select * from sfism4.r_wip_tracking_t where serial_number ='" + txtInput1.Text + "' or shipping_sn ='" + txtInput1.Text + "'";
                _result_data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result_data.Data != null)
                {
                    txtInput1.Text = _result_data.Data["serial_number"]?.ToString() ?? "";
                    txtModelName.Text = _result_data.Data["model_name"]?.ToString() ?? "";

                }
                else
                {
                    Result = "FAIL";
                    txtInput1.Focus();
                    return;
                }

                try
                {
                    sql= "select * from sfism4.r_SN_DETAIL_t where group_name='"+ txtStation.Text + "' AND SUBSTR(WIP_GROUP,1,2)<>'R_' AND serial_number ='"+ txtInput1 .Text+ "'";
                    _result_data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType=SfcCommandType.Text
                    });

                    if (_result_data.Data!=null)
                    {
                        if (rePrint.IsChecked)
                        {
                             ispass = true;
                        }
                        else
                        {
                            showMessage("SN PRINTED, MUST CHOSEN REPRINT STATION " + M_Group + " SERIAL NUMBER " + txtInput1.Text + "----"+SSN_INFO, "BAN NAY DA IN, PHAI CHON IN BU " + M_Group + " SERIAL NUMBER " + txtInput1.Text + "----" + SSN_INFO, true);
                            txtInput1.Focus();
                            return;
                        }
                    }
                    else
                    {
                        if (rePrint.IsChecked)
                        {
                            rePrint.IsChecked = false;
                        }
                    }

                    sql = "select * from sfism4.R_CUSTSN_T where serial_number ='" + txtInput1.Text + "'";
                    _result_data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result_data.Data != null)
                    {

                        sql="UPDATE SFISM4.R_WIP_TRACKING_T SET SHIPPING_SN='"+_result_data.Data["ssn1"].ToString()+ "',ATE_STATION_NO='AUTO_PRINT' WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                        var _result_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText=sql,
                            SfcCommandType=SfcCommandType.Text
                        });
                    }

                }
                catch (Exception ex)
                {
                   Result="FAIL";
                    return;
                }

            }
            //Format: WIFI:S:NETGEAR03;T:WPA2;P:modernnest452;SN:58YF1871A15C9;SK:R6700-100NAS;MAC:C89E435BB944;
            if (txtInput1.Text.Length > 4 && txtInput1.Text.Substring(0, 4) == "WIFI" && txtInput1.Text.IndexOf("SN:") != -1 && txtInput1.Text.IndexOf("SK:") != -1)
            {
                txtInput1.Text = txtInput1.Text.Substring(txtInput1.Text.IndexOf("SN:") + 3, txtInput1.Text.IndexOf(";SK:") - txtInput1.Text.IndexOf("SN:") - 3);
            }

            //Format: WPS PIN:76954252;SN:5X45197VA0288;SK:EAX80-100NAS;MAC:C89E436BC51F;
            if (txtInput1.Text.IndexOf("WPS PIN") != -1)
            {
                txtInput1.Text = txtInput1.Text.Substring(txtInput1.Text.IndexOf("SN:") + 3, 13);
            }

            //FORMAT: SN:W6RD2201010231,SSID:IDU0231,PWD:BtXbAh5900,MAC:3498B5F4CDCE
            txtInput1.Text = txtInput1.Text.ToUpper();
            if ((txtInput1.Text.IndexOf("SN:") != -1) && (txtInput1.Text.Length == 62))
            {
                txtInput1.Text = txtInput1.Text.Substring(txtInput1.Text.IndexOf(":") + 1, 14);
            }

            //Không biết format. chưa test được
            if (txtInput1.Text.IndexOf("</EAN>") != -1)
            {
                txtInput1.Text = txtInput1.Text.Substring(txtInput1.Text.IndexOf("<SRNO>") + 6, txtInput1.Text.IndexOf("</SRNO>") - txtInput1.Text.IndexOf("</SRNO>") - 6);
            }
            else
            {
                if (txtInput1.Text.IndexOf(",") != -1)
                {
                    txtInput1.Text = txtInput1.Text.Substring(0, txtInput1.Text.IndexOf(","));
                }
            }

            //FORMAT: {Serial Number: "5V429571A0001", Mac Address: "123456ABCDEF", Model: "GC108PP", Default IP Address: "192.168.0.239", User: "admin", Default Password: "password"}
            txtInput1.Text = txtInput1.Text.ToUpper();
            if (txtInput1.Text.IndexOf("SERIAL NUMBER:") != -1)
            {
                txtInput1.Text = txtInput1.Text.Substring(txtInput1.Text.IndexOf('"') + 1, 13);
            }

            //FORMAT hàng RBK: @ZY81INU                    PASS
            if (txtInput1.Text.IndexOf("                    PASS") != -1)
            {
                //txtInput1.Text = txtInput1.Text.Substring(0, 8);
                sql = "SELECT SHIPPING_SN FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER IN ( '" + txtInput1.Text.Substring(0, 8) + "')";
                var result_RBK = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_RBK.Data != null)
                {
                    txtInput1.Text = result_RBK.Data["shipping_sn"]?.ToString() ?? "";
                }
            }

            //FORMAT hàng VSC: 2DBTBTP5GW-MD-CD212022210 210809 24:c3:f9:0a:8d:84
            if ((txtInput1.Text.Length == 53 && txtInput1.Text.IndexOf(":") != -1) || txtInput1.Text.Length == 54)
            {
                if (txtInput1.Text.Length == 53)
                {
                    txtInput1.Text = txtInput1.Text.Substring(36);
                    txtInput1.Text = txtInput1.Text.Replace(":", "");
                }
                else
                {
                    txtInput1.Text = txtInput1.Text.Substring(37);
                    txtInput1.Text = txtInput1.Text.Replace(":", "");
                }
                sql = "SELECT SHIPPING_SN VALUE_1 FROM SFISM4.R_NETG_PRIN_ALL_T WHERE  PRINT_FLAG <> 'R' AND MACID = LOWER('" + txtInput1.Text + "') AND ROWNUM=1";
                txtInput1.Text = await DoSelectSingleValueQueryString(sql);
            }

            if (txtInput1.Text.IndexOf(":") != -1)
            {
                txtInput1.Text = txtInput1.Text.Replace(":", "");
            }
            */
                #endregion
            
            if (Z107.IsChecked == false && txtMoNumber.Text.Length == 0)
            {
                sql = "SELECT MODEL_TYPE,model_name,MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE " +
                      "MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.R107 WHERE " +
                      "SERIAL_NUMBER='" + txtInput1.Text + "'  OR SHIPPING_SN='" + txtInput1.Text + "'  OR imei='" + txtInput1.Text + "'" +
                      "OR SHIPPING_SN2='" + txtInput1.Text + "') and rownum=1";
            }
            else if (txtMoNumber.Text.Length > 0)
            {
                sql = "SELECT MODEL_TYPE,model_name,MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE " +
                      "MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.R105 WHERE  mo_number ='" + txtMoNumber.Text + "')";
            }
            else
            {
                sql = "  SELECT MODEL_TYPE,model_name,MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE " +
                    "  MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.z107 WHERE " +
                    "  SERIAL_NUMBER='" + txtInput1.Text + "'  OR SHIPPING_SN='" + txtInput1.Text + "' OR imei='" + txtInput1.Text + "' " +
                                " OR SHIPPING_SN2='" + txtInput1.Text + "') and rownum=1";
            }

            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                model_type = _result.Data["model_type"]?.ToString() ?? "";
                M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                c_model_seri = _result.Data["model_serial"]?.ToString() ?? "";
            }
            c_serial = txtInput1.Text;

            //XUAN QUY SUA DE SELECT TEN HANG DONG GOI THEO MA DEVICE_SN _ EERO
            if (!string.IsNullOrEmpty(M_MODEL_NAME))
            {
                if (M_MODEL_NAME.Substring(0, 4) == "830-")
                {
                    sql = "SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE (SERIAL_NUMBER='" + txtInput1.Text + "' OR SHIPPING_SN='" + txtInput1.Text + "' OR SHIPPING_SN2='" + txtInput1.Text + "') AND MODEL_NAME LIKE'810-%'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";

                        sql = "SELECT MODEL_TYPE,model_name,MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.R107 WHERE "
                            + "SERIAL_NUMBER='" + txtInput1.Text + "')";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            model_type = _result.Data["model_type"]?.ToString() ?? "";
                            M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                            c_model_seri = _result.Data["model_serial"]?.ToString() ?? "";
                        }
                    }
                }

                if (M_MODEL_NAME.Substring(0, 4) == "810-")
                {
                    sql = "SELECT*FROM SFISM4.R107 WHERE (SERIAL_NUMBER='" + c_serial + "' OR SHIPPING_SN='" + c_serial + "' OR SHIPPING_SN2='" + c_serial + "') AND MODEL_NAME IN" +
                          "(SELECT MODEL_NAME FROM SFIS1.C_PACK_PARAM_T)";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";

                        sql = "  SELECT MODEL_TYPE,model_name,MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE " +
                          "  MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.R107 WHERE " +
                          "  SERIAL_NUMBER='" + txtInput1.Text + "'  OR SHIPPING_SN='" + txtInput1.Text + "' " +
                          " OR SHIPPING_SN2='" + txtInput1.Text + "') and rownum=1";

                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            model_type = _result.Data["model_type"]?.ToString() ?? "";
                            M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                            c_model_seri = _result.Data["model_serial"]?.ToString() ?? "";
                        }
                    }
                    else
                    {
                        sql = "SELECT*FROM SFISM4.R107 WHERE SERIAL_NUMBER IN (SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_SN = '" + c_serial + "') AND MODEL_NAME IN "
                            + "(SELECT MODEL_NAME FROM SFIS1.C_PACK_PARAM_T WHERE MODEL_NAME NOT LIKE '810-%')";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";
                            sql = "  SELECT MODEL_TYPE,model_name,MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE " +
                              "  MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.R107 WHERE " +
                              "  SERIAL_NUMBER='" + txtInput1.Text + "') and rownum=1";

                            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result.Data != null)
                            {
                                model_type = _result.Data["model_type"]?.ToString() ?? "";
                                M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                                c_model_seri = _result.Data["model_serial"]?.ToString() ?? "";
                            }
                        }
                    }
                }
            }

            //Auto HOLD
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G60") != -1 && string.IsNullOrEmpty(txtStation.Text))
            {
                showMessage("You need choose station!", "Bạn cần chọn trạm!", true);
                return;
            }
            // KHONG DUOC IN BU SAU RC - TIEN LUAT 27/01
            if (txtSedprtnow.Text == "4")
            {
                sql = "SELECT SPECIAL_ROUTE,WIP_GROUP,GROUP_NAME,MO_NUMBER,SHIP_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SHIPPING_SN='" + txtInput1.Text + "' "
                    + " OR SHIPPING_SN2='" + txtInput1.Text + "' OR SERIAL_NUMBER='" + txtInput1.Text + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    c_ship_no = _result.Data["ship_no"]?.ToString() ?? "";
                    c_special_route = _result.Data["special_route"]?.ToString() ?? "";
                    c_wip_group = _result.Data["wip_group"]?.ToString() ?? "";
                    c_mo_number = _result.Data["mo_number"]?.ToString() ?? "";
                    if (!string.IsNullOrEmpty(c_ship_no) && c_ship_no != "N/A")
                    {
                        sql = "SELECT SPECIAL_ROUTE,WIP_GROUP,GROUP_NAME,MO_NUMBER,SHIP_NO FROM SFISM4.R_WIP_TRACKING_T "
                            + "WHERE  SERIAL_NUMBER='" + c_ship_no + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            c_ship_no = _result.Data["ship_no"]?.ToString() ?? "";
                            c_special_route = _result.Data["special_route"]?.ToString() ?? "";
                            c_wip_group = _result.Data["wip_group"]?.ToString() ?? "";
                            c_mo_number = _result.Data["mo_number"]?.ToString() ?? "";
                        }
                    }
                }

                sql = "select MIN(STEP_SEQUENCE) STEP from sfis1.c_route_control_t where route_code ='" + c_special_route + "' and group_name ='RC' AND state_flag ='0'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    if (!string.IsNullOrEmpty(_result.Data["step"]?.ToString() ?? ""))
                    {
                        c_step_sequence = _result.Data["step"]?.ToString() ?? "";
                        i_step_sequence = Int32.Parse(c_step_sequence);

                        sql = "select MIN(STEP_SEQUENCE) STEP from sfis1.c_route_control_t where route_code ='" + c_special_route + "' and group_name ='" + c_wip_group + "' AND state_flag ='0'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            c_step_sequence1 = _result.Data["step"]?.ToString() ?? "";
                            i_step_sequence1 = Int32.Parse(c_step_sequence1);
                            if (!string.IsNullOrEmpty(c_step_sequence1))
                            {
                                sql = "SELECT * FROM SFISM4.R105 WHERE MO_NUMBER='" + c_mo_number + "' AND END_GROUP='" + c_wip_group + "'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null && c_wip_group != "RC" && txtMoNumber.Text.Length == 0)
                                {
                                    showMessage("This station cannot reprint,need rework to RC!", "Trạm này không thể in lại, cần rework đến RC!", true);
                                    txtInput1.SelectAll();
                                    return;
                                }
                            }
                            if (i_step_sequence < i_step_sequence1 && txtMoNumber.Text.Length == 0)
                            {
                                showMessage("This station cannot reprint,need rework to RC!", "Trạm này không thể in lại, cần rework đến RC!", true);
                                txtInput1.SelectAll();
                                return;
                            }
                        }
                    }
                }
            }
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G74") != -1)
            {
                CancelWhenMacisNA.IsChecked = false;
                checkMac.IsChecked = false;
            }

            // AUTO HOLD
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G15") != -1)
            {
                sql = "SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER='" + txtInput1.Text + "'AND GROUP_NAME IN ('PACK_BOX','CHECK_BOX_WEIGHT','PACK_CTN','F-FQA','OBA_CHECK_WEIGHT','PACK_PALT','FQA','CHK_PALT','STOCKIN')";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.SYSTEM_HOLD",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SN",Value=txtInput1.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MAIN_DESC",Value=M_MODEL_NAME,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="EMP_NO",Value="LOT_REPRINT HOLD",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="REASON",Value="LOT_REPRINT AUTO HOLD WHEN DEM BAN SAU TRAM PACK_BOX VE IN LAI",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="PROGRAM_NAME",Value="LOT_REPRINT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="FLAG",Value="0",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                    });
                    dynamic _ads = result.Data;
                    string _RES = _ads[0]["res"];

                    if (_RES == "OK INSERT")
                    {
                        sql = "update SFISM4.R107 SET GROUP_NAME='HOLD-'||GROUP_NAME,WIP_GROUP='HOLD-'||WIP_GROUP,NEXT_STATION='HOLD-'||NEXT_STATION,EMP_NO='LOT_REPRINT' WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                        var insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (insert.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + insert.Message.ToString() + "\nsql: " + sql, "Exception: " + insert.Message.ToString() + "\nsql: " + sql, true);
                            return;
                        }
                    }
                    else
                    {
                        showMessage(txtInput1.Text + _RES, txtInput1.Text + _RES, true);
                        //MessageBox.Show(txtInput1.Text + " " + _RES);
                        return;
                    }
                }
            }

            //Cpei scan SN
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G01") != -1)
            {
                InputSN.IsChecked = true;
                noCheckSSN.IsChecked = true;
                checkMac.IsChecked = false;
            }
            //Cpei scan MAC
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G02") != -1)
            {
                InputSN.IsChecked = true;
                noCheckSSN.IsChecked = true;
            }
            //Cpeii scan SN
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G03") != -1)
            {
                InputSN.IsChecked = true;
            }
            //Cpeii scan MAC
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G04") != -1)
            {
                InputSN.IsChecked = true;
                noCheckSSN.IsChecked = true;
                BB.IsChecked = true;
            }
            //EERO auto chose
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G37") != -1)
            {
                InputSN.IsChecked = true;
                noCheckSSN.IsChecked = true;
                checkMac.IsChecked = false;
            }
            txtInput2.Text = txtInput2.Text.ToUpper();

            if (printPalt.IsChecked || printCtn.IsChecked)
            {
                if (await checkPalletFull(txtInput1.Text, "SFISM4.R_WIP_TRACKING_T"))
                {
                    notFull = true;
                }
                else
                {
                    notFull = false;
                }
            }
            if (notFull && !rePrint.IsChecked)
            {
                showMessage("YOU MUST CHOOSE TO REPRINT", "BAN PHAI CHON IN LAI (REPRINT)", true);
                txtInput1.Focus();
                txtInput1.SelectAll();
                return;
            }

            sql = "SELECT SERIAL_NUMBER,MO_NUMBER,SPECIAL_ROUTE,WIP_GROUP,MODEL_NAME FROM SFISM4.R107 WHERE (SERIAL_NUMBER = '" + txtInput1.Text + "' OR SHIPPING_SN='" + txtInput1.Text + "' OR SHIPPING_SN2='" + txtInput1.Text + "') AND MODEL_NAME='" + M_MODEL_NAME + "'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null && txtMoNumber.Text.Length == 0)
            {
                c_special_route = _result.Data["special_route"]?.ToString() ?? "";
                c_wip_group = _result.Data["wip_group"]?.ToString() ?? "";
                M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                c_mo_number = _result.Data["mo_number"]?.ToString() ?? "";
                

                if (!string.IsNullOrEmpty(txtStation.Text))
                {
                    txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";
                }
                sql = "select model_type from sfis1.c_model_desc_t where model_name='" + M_MODEL_NAME + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    model_type = _result.Data["model_type"]?.ToString() ?? "";
                }
                if (c_wip_group.Substring(0, 2) == "R_")
                {
                    showMessage("THIS STATION R_ CAN'T PRINT", "BẢN BỊ R_ KHÔNG THỂ IN", true);
                    txtInput1.Focus();
                    txtInput1.SelectAll();
                    return;
                }
                if (M_MODEL_NAME.Substring(0, 4) == "810-")
                {
                    sql = "select MIN(STEP_SEQUENCE) STEP from sfis1.c_route_control_t where route_code ='" + c_special_route + "' and group_name ='VI' AND state_flag ='0'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        c_step_sequence = _result.Data["step"]?.ToString() ?? "";
                        i_step_sequence = Int32.Parse(c_step_sequence);
                        sql = "select MIN(STEP_SEQUENCE) STEP from sfis1.c_route_control_t where route_code ='" + c_special_route + "' and group_name ='" + c_wip_group + "' AND state_flag ='0'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            c_step_sequence1 = _result.Data["step"]?.ToString() ?? "";
                            if (c_step_sequence1 == "")
                            {
                                sql = "SELECT * FROM SFISM4.R105 WHERE MO_NUMBER='" + c_mo_number + "' AND END_GROUP='" + c_wip_group + "'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null && c_wip_group != "VI" && txtMoNumber.Text.Length == 0)
                                {
                                    showMessage("THIS STATION CANNOT REPRINT,NEED REWORK TO VI!", "TRẠM NÀY KHÔNG THỂ IN LẠI, HÃY REWORK VỀ VI", true);
                                    txtInput1.Focus();
                                    txtInput1.SelectAll();
                                    return;
                                }
                            }
                            i_step_sequence1 = Int32.Parse(c_step_sequence1);
                            if (i_step_sequence < i_step_sequence1)
                            {
                                showMessage("THIS STATION CANNOT REPRINT,NEED REWORK TO VI!", "TRẠM NÀY KHÔNG THỂ IN LẠI, HÃY REWORK VỀ VI", true);
                                txtInput1.Focus();
                                txtInput1.SelectAll();
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("201") != -1)
                    {
                        sql = "select STEP_SEQUENCE from sfis1.c_route_control_t where route_code ='" + c_special_route + "' and group_next ='PACK_BOX' AND state_flag ='0' ";
                    }
                    else
                    {
                        sql = "select STEP_SEQUENCE from sfis1.c_route_control_t where route_code ='" + c_special_route + "' and group_next ='PACK_CTN' AND state_flag ='0'";
                    }
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        c_step_sequence = _result.Data["step_sequence"]?.ToString() ?? "";
                        i_step_sequence = Int32.Parse(c_step_sequence);
                        sql = "select STEP_SEQUENCE from sfis1.c_route_control_t where route_code ='" + c_special_route + "' and group_next ='" + c_wip_group + "' AND state_flag ='0' order by STEP_SEQUENCE";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            c_step_sequence1 = _result.Data["step_sequence"]?.ToString() ?? "";
                            i_step_sequence1 = Int32.Parse(c_step_sequence1);
                            if ((i_step_sequence >= i_step_sequence1 || printCtn.IsChecked) && txtMoNumber.Text.Length == 0)
                            {

                            }
                            else
                            {
                                showMessage("THIS STATION CANNOT REPRINT,NEED REWORK TO VI!", "TRẠM NÀY KHÔNG THỂ IN LẠI, HÃY REWORK VỀ VI", true);
                                txtInput1.Focus();
                                txtInput1.SelectAll();
                                return;
                            }
                        }
                    }
                }
            }
            if (InputSNT.IsChecked)
            {
                if (string.IsNullOrEmpty(await getDotSerialNumber(txtInput1.Text.Trim().ToUpper())))
                {
                    txtInput1.Text = await getDotSerialNumber(txtInput1.Text.Trim().ToUpper());
                    lbl2.Text = txtInput1.Text;
                }
                else
                {
                    txtInput1.Text = txtInput1.Text.Trim().ToUpper();
                    lbl2.Text = txtInput1.Text.Trim().ToUpper();
                }
            }
            if (await checkReprint() && string.IsNullOrEmpty(txtStation.Text) && txtMoNumber.Text.Length == 0)
            {
                if (!reprint_flag)
                {/*
                    if (Z107.IsChecked != true)
                    {
                        if (await findR107(txtInput1.Text))
                        {
                            frmLogin frmLogin = new frmLogin();
                            frmLogin.sfcClient = sfcClient;
                            frmLogin.type = "MSG";
                            frmLogin.ShowDialog();
                        }
                    }*/
                }
                else
                {
                    reprint_flag = false;
                }
            }
            /*if pl_model.Checked then
              begin
                if editinput.Focused then
                begin
                  ClearSendListBox;
                  mo1:='';mo2:='';model1:='';model2:='';
                  pl_model_check(editinput.Text,1);
                  exit;
                end;
                if editinput2.Focused then
                begin
                  if editinput.Text='' then
                  begin
                    PasswdForm.panlMessage.Caption := GetPubMessage('00339',dbReprint);
                    PasswdForm.Showmodal;
                    editinput.SelectAll;
                    editinput.SetFocus;
                    exit;
                  end;
                  pl_model_check(editinput2.Text,2);
                  pl_model_print;
                  exit;
                end;
              end;*/
            //khong biet focused de lam gi
            if (plModel.IsChecked)
            {
                if (txtInput1.Focus())
                {

                }
            }


            if (txtInput1.Text != "")
            {
                if (!printBoxLabelForEnskyRework.IsChecked)
                {
                    var regexItem = new Regex("^[A-Z0-9 --%/@:]*$");
                    if (!regexItem.IsMatch(txtInput1.Text))
                    {
                        showMessage("00340", "00340", false);
                        return;
                    }

                }
                if (chkMac2.IsChecked == true)
                {
                    if (txtInput2.Text == "")
                    {
                        sql = "select serial_number from sfism4.r_CUSTSN_T where  MAC2= '" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";
                        }
                        else
                        {
                            showMessage("00341", "00341", false);
                            return;
                        }
                    }
                }
                if (chkMac.IsChecked == true)
                {
                    if (txtInput2.Text == "")
                    {
                        sql = "select serial_number,key_part_sn from sfism4.r_wip_keyparts_t where  key_part_sn='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";
                        }
                        else
                        {
                            showMessage("00341", "00341", false);
                            return;
                        }
                    }
                }
                if (chkSSN.IsChecked == true)
                {
                    if (txtInput2.Text == "")
                    {
                        sql = "select serial_number from sfism4.r_wip_tracking_t where  shipping_sn='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";
                        }
                        else
                        {
                            showMessage("00341", "00341", false);
                            return;
                        }
                    }
                }

                //XUANQUY ADD CHECK_ROUTE
                if (!string.IsNullOrEmpty(txtStation.Text))
                {
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.CHECK_ROUTE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="LINE",Value=txtLine.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value=M_Group,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="DATA",Value=txtInput1.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                    });
                    dynamic _ads = result.Data;
                    string _RES = _ads[0]["res"];

                    if (_RES != "OK")
                    {
                        showMessage("00022", "00022", false);
                        return;
                    }
                }
                //END XUANQUY ADD CHECK_ROUTE

                if (printBoxLabelForEnskyRework.IsChecked && txtInput2.Text != "")
                {
                    if (!await p_ensky_rework_boxlabel(txtInput2.Text))
                    {
                        return;
                    }
                }
                if (printPalt.IsChecked && txtMoNumber.Text.Length == 0)
                {
                    sql = "select model_type,model_name from sfis1.c_model_desc_t where model_name =(SELECT MODEL_NAME FROM SFISM4.R107 WHERE IMEI= '" + txtInput1.Text + "' AND ROWNUM=1 )";
                }
                else if (txtMoNumber.Text.Length > 0)
                {
                    sql = "select * from sfis1.c_model_desc_t where model_name IN (SELECT MODEL_NAME FROM SFISM4.R105 WHERE mo_number = '" + txtMoNumber.Text + "')";
                }
                else
                {
                    sql = "select * from sfis1.c_model_desc_t where model_name IN (SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER= '" + txtInput1.Text + "' OR " +
                        "SHIPPING_SN='" + txtInput1.Text + "' or SHIPPING_SN2='" + txtInput1.Text + "' )";
                }
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    model_type = _result.Data["model_type"]?.ToString() ?? "";
                    M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                }
                else
                {
                    sql = "Select * From SFIS1.C_Model_Desc_T Where Model_Name IN (Select A.Model_Name From SFISM4.R_Mo_Base_T A, SFISM4.R_Wip_Keyparts_T B " +
                        "Where A.Mo_number=B.Mo_Number AND B.Key_Part_Sn='" + txtInput1.Text + "' AND B.Key_Part_No='MACID') AND RowNum=1";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        model_type = _result.Data["model_type"]?.ToString() ?? "";
                        M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                        M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                    }
                }

                sql = "select model_serial from sfis1.c_model_desc_t where model_name='SFISSITE'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    if (_result.Data["model_serial"].ToString() == "N")
                    {
                        if (!await getAllModelValue(model_type, txtInput1.Text)) return;
                    }
                }

                if (string.IsNullOrEmpty(model_type))
                {
                    sql = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME IN(SELECT MODEL_NAME FROM SFISM4.R107 WHERE SERIAL_NUMBER='" + txtInput1.Text + "')";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        model_type = _result.Data["model_type"]?.ToString() ?? "";
                        model_name = _result.Data["model_name"]?.ToString() ?? "";
                        M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                    }
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("035") != -1 && rework.IsChecked)
                {
                    if (txtInput2.Text == "")
                    {
                        txtInput2.Focus();
                        return;
                    }
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                        txtInput1.Text = "";
                        txtInput2.Text = "";
                        txtInput3.Text = "";
                        txtInput1.Focus();
                        return;
                    }
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("051") != -1)
                {
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                        txtInput1.Text = "";
                        txtInput2.Text = "";
                        txtInput3.Text = "";
                        txtInput1.Focus();
                        return;
                    }
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("084") != -1)
                {
                    if (InputSN.IsChecked)
                    {
                        sql = "select data2 from sfism4.r_undp_data_t  where serial_number='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                            txtInput2.Text = _result.Data["data2"]?.ToString() ?? "";
                    }
                    if (!string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    else
                    {
                        if (txtInput2.Text.Trim().Length != 11)
                        {
                            if (txtInput2.Text.Trim().Length != 12)
                            {
                                txtInput2.Focus();
                                showMessage("00342", "00342", false);
                                return;
                            }
                        }
                    }
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                        txtInput1.Text = "";
                        txtInput2.Text = "";
                        txtInput3.Text = "";
                        txtInput1.Focus();
                        return;
                    }
                }
                //for T77N081
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("083") != -1)
                {
                    txtInput2.Text = txtInput1.Text;
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    txtInput1.Text = "";
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    txtInput1.Focus();
                    return;
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("071") != -1)
                {
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    txtInput1.Text = "";
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    txtInput1.Focus();
                    return;
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("079") != -1)
                {
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    txtInput1.Text = "";
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    txtInput1.Focus();
                    return;
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("045") != -1)
                {
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    txtInput1.Text = "";
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    txtInput1.Focus();
                    return;
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("047") != -1)
                {
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    if (txtInput1.Text != txtInput2.Text)
                    {
                        showMessage("00343", "00343", false);
                        txtInput2.Focus();
                        txtInput2.SelectAll();
                        return;
                    }
                    sMO_TYPE = await findMOType();
                    if (!string.IsNullOrEmpty(sMO_TYPE))
                    {
                        if (txtSeditfCount.Text == "1")
                            if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                            {
                                return;
                            }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text != "0")
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    txtInput1.Text = "";
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    txtInput1.Focus();
                    return;
                }
                if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("030") != -1 || model_type.IndexOf("042") != -1 || model_type.IndexOf("043") != -1 || model_type.IndexOf("044") != -1 || model_type.IndexOf("052") != -1 || model_type.IndexOf("070") != -1))
                {
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    else
                    {
                        if (txtInput1.Text != txtInput2.Text)
                        {
                            showMessage("00344", "00344", false);
                            txtInput2.Focus();
                            txtInput2.SelectAll();
                            return;
                        }
                    }
                    sql = "select SN,SSN1,SSN2 From sfism4.R_SIEMENS_DATA_T WHERE SN='" + txtInput1.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        showMessage("00345", "00345", false);
                        txtInput1.Focus();
                        txtInput1.SelectAll();
                        return;
                    }
                    else
                    {
                        dtParams.Clear();
                        await SM_model_printAsync();
                        return;
                    }
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("110") != -1)
                {
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        txtInput2.Focus();
                        return;
                    }
                    else
                    {
                        if (txtInput1.Text != txtInput2.Text)
                        {
                            showMessage("00344", "00344", false);
                            txtInput1.Clear();
                            txtInput2.Clear();
                            txtInput1.Focus();
                            return;
                        }
                    }
                    sql = "select SN,SSN1,mac1 From sfism4.R_SIEMENS_DATA_T WHERE SN='" + txtInput1.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        showMessage("00345", "00345", false);
                        txtInput1.Focus();
                        txtInput1.SelectAll();
                        return;
                    }
                    else
                    {
                        dtParams.Clear();
                        await iDTAprint();
                    }
                }
                sql = "select * from SFIS1.C_PRGSCAN_SEQUENCE_T where model_name IN(SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER= '" + txtInput1.Text + "'  or shipping_sn = '" + txtInput1.Text + "' OR SHIPPING_SN2='" + txtInput1.Text + "' )   AND  CHECK_CUSTSN_RULE ='NOCHK'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    CHECK_PONO.IsChecked = true;
                    CHECK_PONO.IsEnabled = false;
                }
                if (PCBLabel.IsChecked)
                {
                    if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("027") != -1)
                    {
                        sql = "SELECT PO_NO,SHIPPING_SN,MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data == null)
                        {
                            showMessage("00347", "00347", false);
                            txtInput1.SelectAll();
                            return;
                        }
                        M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                        sMO_TYPE = await findMOType();
                        if (!string.IsNullOrEmpty(sMO_TYPE))
                        {
                            if (!await P_PrintToCodeSoft("C", M_MODEL_NAME + "_BX.Lab"))
                            {
                                return;
                            }
                        }
                        txtInput2.Clear();
                        txtInput3.Clear();
                        txtInput1.SelectAll();
                        return;
                    }
                    else
                    {
                        showMessage("00346", "00346", false);
                        txtInput1.SelectAll();
                        return;
                    }
                }
                if (STBLabel.IsChecked)
                {
                    if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("027") != -1)
                    {
                        sql = "SELECT PO_NO,SHIPPING_SN,MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data == null)
                        {
                            showMessage("00347", "00347", false);
                            txtInput1.SelectAll();
                            return;
                        }
                        M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                        sMO_TYPE = await findMOType();
                        if (!string.IsNullOrEmpty(sMO_TYPE))
                        {
                            if (!await P_PrintToCodeSoft("C", M_MODEL_NAME + "_BX.Lab"))
                            {
                                return;
                            }
                        }
                        txtInput2.Clear();
                        txtInput3.Clear();
                        txtInput1.SelectAll();
                        return;
                    }
                    else
                    {
                        showMessage("00346", "00346", false);
                        txtInput1.SelectAll();
                        return;
                    }
                }
                if (PinCode.IsChecked)
                {
                    if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("011") != -1)
                    {
                        if (string.IsNullOrEmpty(txtInput2.Text))
                        {
                            txtInput2.Focus();
                            return;
                        }
                        sMO_TYPE = await findMOType();
                        if (!string.IsNullOrEmpty(sMO_TYPE))
                        {
                            if (!await P_PrintToCodeSoft("C", M_MODEL_NAME + "BX1.Lab"))
                            {
                                return;
                            }
                        }
                        txtInput2.Clear();
                        txtInput3.Clear();
                        txtInput1.Clear();
                        txtInput1.Focus();
                        return;
                    }
                }
                if (BPinCode.IsChecked)
                {
                    if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("016") != -1 || model_type.IndexOf("032") != -1))
                    {
                        if (string.IsNullOrEmpty(txtInput2.Text))
                        {
                            txtInput2.Focus();
                            return;
                        }
                        sMO_TYPE = await findMOType();
                        if (!string.IsNullOrEmpty(sMO_TYPE))
                        {
                            if (txtSeditfCount.Text == "1")
                                if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                                {
                                    return;
                                }
                            if (txtSeditfCount.Text != "1")
                            {
                                if (txtSedprtnow.Text != "0")
                                {
                                    labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                    if (!await P_PrintToCodeSoft("C", labname))
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        showMessage("00346", "00346", false);
                        txtInput1.SelectAll();
                        return;
                    }
                    txtInput2.Clear();
                    txtInput3.Clear();
                    txtInput1.Clear();
                    txtInput1.Focus();
                    return;
                }
                if (printPalt.IsChecked)
                {
                    if (Kind.SelectedItem == chkPalletLabel)
                    {
                        if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("080") != -1)
                        {
                            await Shipping_Pallet(txtInput1.Text);
                            return;
                        }
                        if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("999") != -1)
                        {
                            if (!await CheckExistZ107(txtInput1.Text))
                            {
                                txtInput1.SelectAll();
                                return;
                            }
                            if (!await Shipping_PDF417(txtInput1.Text))
                            {
                                showMessage("Data not found", "Không tìm thấy dữ liệu", true);
                                txtInput1.SelectAll();
                                return;
                            }
                        }
                        if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("103") != -1)
                        {
                            sFindMcartonflag = await FindMcarton_DCI(txtInput1.Text.Trim());
                            return;
                        }
                        if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("168") != -1)
                        {
                            txtInput1.SelectAll();
                            sFindMcartonflag = await FindShippingData168(txtInput1.Text.Trim());
                            return;
                        }
                        if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("027") != -1)
                        {
                            sFindMcartonflag = await FindMcarton_DCI(txtInput1.Text.Trim());
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("039") != -1 && SSNLABEL.IsChecked)
                        {
                            sFindMcartonflag = await FindMcarton_I01_34(txtInput1.Text.Trim());
                        }
                        else
                        {
                            sFindMcartonflag = await FindMcarton(txtInput1.Text.Trim());
                        }
                        if (sFindMcartonflag)
                        {
                            if (SSNLABEL.IsChecked)
                            {
                                intX = M_MODEL_NAME.IndexOf(".");
                                if (intX == -1)
                                {
                                    labname = M_MODEL_NAME + "_PMSN.Lab";
                                }
                                else
                                {
                                    labname = M_MODEL_NAME.Substring(0, intX - 1) + M_MODEL_NAME.Substring(intX, M_MODEL_NAME.Length - intX) + "_PMSN.Lab";
                                }
                            }
                            else
                            {
                                intX = M_MODEL_NAME.IndexOf(".");
                                if (intX == -1)
                                {
                                    if (txtSeditfCount.Text == "2")
                                    {
                                        labname = M_MODEL_NAME + "_PMCTN1.Lab";
                                    }
                                    else
                                    {
                                        labname = M_MODEL_NAME + "_PMCTN.Lab";
                                    }
                                }
                                else
                                {
                                    if (txtSeditfCount.Text == "2")
                                    {
                                        labname = M_MODEL_NAME.Substring(0, intX - 1) + M_MODEL_NAME.Substring(intX, M_MODEL_NAME.Length - intX) + "_PMCTN1.Lab";
                                    }
                                    else
                                    {
                                        labname = M_MODEL_NAME.Substring(0, intX - 1) + M_MODEL_NAME.Substring(intX, M_MODEL_NAME.Length - intX) + "_PMCTN.Lab";
                                    }
                                }
                            }
                            if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("G54") != -1 && Z107.IsChecked == true))
                            {
                                sql = "select po_number,po_line from sfism4.R_PO_T where UPC_CODE IS NOT NULL AND customer in (select invoice from sfism4.R_BPCS_INVOICE_T " +
                                    " where tcom in (select ship_no from sfism4.z107 where imei ='" + txtInput1.Text + "' and rownum=1) )";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    AddParams("PO_SAP", _result.Data["po_number"]?.ToString() ?? "");
                                    AddParams("PO_LINE", _result.Data["po_line"]?.ToString() ?? "");
                                }
                                else
                                {
                                    showMessage("PM NOT SET CONGIF 35 OR PQE NOT PASS CALL PM", "PM NOT SET CONGIF 35 OR PQE NOT PASS CALL PM", true);
                                    return;
                                }
                            }
                            if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("G64") != -1 && Z107.IsChecked == true))
                            {
                                sql = "select po_number,po_line from sfism4.R_PO_T where UPC_CODE IS NOT NULL AND customer in (select invoice from sfism4.R_BPCS_INVOICE_T " +
                                    " where tcom in (select ship_no from sfism4.z107 where imei ='" + txtInput1.Text + "' and rownum=1) )";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    AddParams("PO_SAP", _result.Data["po_number"]?.ToString() ?? "");
                                    AddParams("PO_LINE", _result.Data["po_line"]?.ToString() ?? "");
                                }
                                else
                                {
                                    showMessage("PM NOT SET CONGIF 35 OR PQE NOT PASS CALL PM", "PM NOT SET CONGIF 35 OR PQE NOT PASS CALL PM", true);
                                    return;
                                }
                            }
                            if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("G80") != -1 && Z107.IsChecked == true))
                            {
                                sql = "SELECT * FROM SFISM4.z107 WHERE imei='" + txtInput1.Text + "' and wip_group='SHIPPING'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    sql = "select * from SFISM4.R_BPCS_INVOICE_T where tcom in (select ship_no from sfism4.z107 where imei  ='" + txtInput1.Text + "')";
                                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (_result.Data != null)
                                    {
                                        c_invoice = _result.Data["invoice"]?.ToString() ?? "";
                                        AddParams("FOXCONN_DN", c_invoice);
                                        po = _result.Data["cust_po"]?.ToString() ?? "";
                                        AddParams("POLINE", po);
                                        tcom = _result.Data["tcom"]?.ToString() ?? "";
                                        a = Int32.Parse(_result.Data["so_qty"].ToString());
                                        SO_QTY = _result.Data["so_qty"]?.ToString() ?? "";
                                        AddParams("SO_QTY", SO_QTY);

                                        sql = "SELECT DISTINCT IMEI FROM SFISM4.Z107 WHERE SHIP_NO ='" + tcom + "'";
                                        var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                        if (result_list.Data != null)
                                        {
                                            i = 0;
                                            foreach (var row in result_list.Data)
                                            {
                                                i++;
                                                sql = "select *  from SFISM4.R_PO_SN_DETAIL_T WHERE PO_ORDER='" + i + "' AND PO_NO='" + M_MODEL_NAME + "' AND MO_NUMBER='" + po + "' AND MODEL_NAME='PO_ORDER' AND WIP_GROUP='" + tcom + "'";
                                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                                if (_result.Data == null)
                                                {
                                                    sql = "Insert into SFISM4.R_PO_SN_DETAIL_T(PO_ORDER, PO_NO, MO_NUMBER, MODEL_NAME,WIP_GROUP) Values   ('" + i + "', '" + M_MODEL_NAME + "', '" + po + "', 'PO_ORDER','" + tcom + "')";
                                                    var query_insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                    {
                                                        CommandText = sql,
                                                        SfcCommandType = SfcCommandType.Text
                                                    });
                                                    if (query_insert.Result.ToString() != "OK")
                                                    {
                                                        showMessage("Exception: " + query_insert.Message.ToString() + "\nsql: " + sql, "Exception: " + query_insert.Message.ToString() + "\nsql: " + sql, true);
                                                        return;
                                                    }
                                                }
                                            }
                                            sql = "update SFISM4.R_PO_SN_DETAIL_T a set imei =(select imei from(select rownum mynumber,imei from( " +
                                                " select distinct imei from sfism4.z107  where ship_no ='" + tcom + "'  order by imei asc)) b  where a.po_order = b.mynumber) " +
                                                " where  a.mo_number='" + po + "' AND a.wip_group='" + tcom + "'";
                                            var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = sql,
                                                SfcCommandType = SfcCommandType.Text
                                            });
                                            if (query_update.Result.ToString() != "OK")
                                            {
                                                showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            showMessage("DN nay khong co pallet nao ton tai", "DN nay khong co pallet nao ton tai", true);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        showMessage("khong co du lieu cua Tcom cua DN", "khong co du lieu cua Tcom cua DN", true);
                                        return;
                                    }
                                }
                                else
                                {
                                    showMessage("THIS IS PALLET NOT SCAN SHIPPING", "pallet nay chua sao Shipping", true);
                                    return;
                                }
                                sql = "select PRG_NAME from sfis1.c_parameter_ini where prg_name='SHIP_TYPE' and vr_class='Netgear'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    sql = "select INVOICE,K1,K2,K3,K4,K5,K6,K7,K8,K9 from  SFISM4.R_EDI_K14_T  where invoice='" + c_invoice + "'";
                                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (_result.Data == null)
                                    {
                                        showMessage("80420", "80420", false);
                                        txtInput1.SelectAll();
                                        return;
                                    }
                                    else
                                    {
                                        SHIP_TYPE = _result.Data["k7"]?.ToString() ?? "";
                                        AddParams("SHIP_TYPE", SHIP_TYPE);
                                    }
                                }
                                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G24") != -1 && Z107.IsChecked == true)
                                {
                                    sql = "select distinct A.SHIPPING_SN shipping_sn,A.model_name model_name,A.CONTAINER_NO BATCH,A.SHIP_NO SHIP_NO  from " +
                                        " SFISM4.Z107 A WHERE A.imei='" + txtInput1.Text + "'  AND A.WIP_GROUP='SHIPPING'";
                                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (_result.Data != null)
                                    {
                                        M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                                        S_SHIP = _result.Data["ship_no"]?.ToString() ?? "";
                                        batch = "";
                                        haveBatch = "";
                                        BATCH_TYPE = "";

                                        sql = "SELECT DISTINCT CONTAINER_NO FROM SFISM4.Z107 WHERE SHIP_NO='" + S_SHIP + "' AND CONTAINER_NO IS NOT NULL AND CONTAINER_NO<>'N/A'";
                                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                        if (_result.Data != null)
                                        {
                                            haveBatch = _result.Data["container_no"]?.ToString() ?? "";
                                            batch = haveBatch;
                                        }
                                        else
                                        {
                                            sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE UPPER(PRG_NAME)='LOT_REPRINT' AND  VR_NAME='" + M_MODEL_NAME + "'" +
                                                "AND vr_class='TRUCK_NUMBER'";
                                            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                            if (_result.Data == null)
                                            {
                                                showMessage("TRUCK_NUMBER range in C_PARAMETER_INI not exist", "TRUCK_NUMBER range in C_PARAMETER_INI not exist", true);
                                                txtInput1.SelectAll();
                                                return;
                                            }
                                            else
                                            {
                                                BATCH_TYPE = _result.Data["vr_item"]?.ToString() ?? "";
                                                SO_PRE = _result.Data["vr_desc"]?.ToString() ?? "";
                                                endrange = _result.Data["vr_value"]?.ToString() ?? "";
                                                TRUCK_NUMBER = getnextSN(BATCH_TYPE, "0123456789", 1);
                                                if (string.IsNullOrEmpty(endrange.Trim()))
                                                {
                                                    sql = "select to_char(sysdate,'mmddyy')date_now from dual";
                                                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                                    if (_result.Data == null)
                                                    {
                                                        sysdate = _result.Data["date_now"]?.ToString() ?? "";
                                                        if (endrange == sysdate)
                                                        {
                                                            batch = SO_PRE + endrange + TRUCK_NUMBER;
                                                        }

                                                        sql = "UPDATE SFIS1.C_PARAMETER_INI SET VR_VALUE='" + sysdate + "',VR_ITEM='00' WHERE UPPER(PRG_NAME)='LOT_REPRINT' AND vr_class='TRUCK_NUMBER' AND  VR_NAME='" + M_MODEL_NAME + "'";
                                                        var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                        {
                                                            CommandText = sql,
                                                            SfcCommandType = SfcCommandType.Text
                                                        });
                                                        if (query_update.Result.ToString() != "OK")
                                                        {
                                                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                                                            return;
                                                        }

                                                        sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE UPPER(PRG_NAME)='LOT_REPRINT' AND  VR_NAME='" + M_MODEL_NAME + "' AND vr_class='TRUCK_NUMBER'";
                                                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                                        if (_result.Data == null)
                                                        {
                                                            BATCH_TYPE = _result.Data["vr_item"]?.ToString() ?? "";
                                                            TRUCK_NUMBER = getnextSN(BATCH_TYPE, "0123456789", 1);
                                                            batch = SO_PRE + endrange + TRUCK_NUMBER;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    batch = SO_PRE + TRUCK_NUMBER;
                                                }

                                                sql = "UPDATE SFISM4.Z107 SET CONTAINER_NO='" + batch + "' WHERE SHIP_NO='" + S_SHIP + "'";
                                                var queryUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                {
                                                    CommandText = sql,
                                                    SfcCommandType = SfcCommandType.Text
                                                });
                                                if (queryUpdate.Result.ToString() != "OK")
                                                {
                                                    showMessage("Exception: " + queryUpdate.Message.ToString() + "\nsql: " + sql, "Exception: " + queryUpdate.Message.ToString() + "\nsql: " + sql, true);
                                                    return;
                                                }

                                                sql = "UPDATE SFIS1.C_PARAMETER_INI SET VR_ITEM='" + TRUCK_NUMBER + "' WHERE UPPER(PRG_NAME)='LOT_REPRINT' AND vr_class='TRUCK_NUMBER' AND  VR_NAME='" + M_MODEL_NAME + "'";
                                                queryUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                {
                                                    CommandText = sql,
                                                    SfcCommandType = SfcCommandType.Text
                                                });
                                                if (queryUpdate.Result.ToString() != "OK")
                                                {
                                                    showMessage("Exception: " + queryUpdate.Message.ToString() + "\nsql: " + sql, "Exception: " + queryUpdate.Message.ToString() + "\nsql: " + sql, true);
                                                    return;
                                                }
                                            }
                                        }
                                        AddParams("BATCH", batch);
                                    }
                                }
                                sql = "select *  from SFISM4.R_PO_SN_DETAIL_T WHERE IMEI='" + txtInput1.Text + "' AND PO_NO='" + M_MODEL_NAME + "' AND MO_NUMBER='" + po + "' AND MODEL_NAME='PO_ORDER' AND WIP_GROUP='" + tcom + "'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    PO_CURRENT = _result.Data["po_order"]?.ToString() ?? "";
                                    AddParams("PO_CURRENT", PO_CURRENT);
                                }

                                sql = "select MAX(PO_ORDER) MAXORDER   from SFISM4.R_PO_SN_DETAIL_T WHERE  PO_NO='" + M_MODEL_NAME + "' AND MO_NUMBER='" + po + "' AND MODEL_NAME='PO_ORDER' AND WIP_GROUP='" + tcom + "'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    PO_MAX = _result.Data["maxorder"]?.ToString() ?? "";
                                    AddParams("PO_MAX", PO_MAX);
                                }
                                if (M_MODEL_NAME.IndexOf("/") != -1)
                                    M_MODEL_NAME = M_MODEL_NAME.Replace("/", "_");
                                labname = M_MODEL_NAME + "_PMCTN.Lab";

                                sql = "select mcarton_no,count(serial_number) as count from sfism4.z_wip_tracking_t where imei = '" + txtInput1.Text + "' group by mcarton_no order by mcarton_no";
                                var result_list1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_list1.Data != null)
                                {
                                    intX = 1;
                                    total_ctn = 0;
                                    total_pcs = 0;
                                    foreach (var row in result_list1.Data)
                                    {
                                        AddParams("Carton_No" + intX, row["mcarton_no"]?.ToString() ?? "");
                                        AddParams("Qty" + intX, row["count"]?.ToString() ?? "");
                                        i++;
                                        total_ctn++;
                                        total_pcs = total_pcs + Int32.Parse(row["count"]?.ToString() ?? "");
                                    }
                                    AddParams("Total_carton", total_ctn.ToString());
                                    AddParams("Total_Pcs", total_pcs.ToString());
                                }
                                sql = "SELECT SSN3 FROM SFISM4.R_CUSTSN_T WHERE SSN1 IN (SELECT SHIPPING_SN FROM SFISM4.z107 WHERE imei='" + txtInput1.Text + "' and wip_group='SHIPPING')";
                                result_list1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_list1.Data != null)
                                {
                                    i = 0;
                                    foreach (var row in result_list1.Data)
                                    {
                                        i++;
                                        AddParams("MSNK", row["ssn3"]?.ToString() ?? "");
                                    }
                                }
                            }
                            if (!await P_PrintToCodeSoft("C", labname))
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("039") != -1)
                            {
                                showMessage("00248", "00248", false);
                            }
                        }
                        txtInput1.SelectAll();
                        return;
                    }
                }
                //tai hai them cho hang muon in carton label
                if (printCtn.IsChecked)
                {
                    dtParams.Clear();
                    if (Z107.IsChecked == true)
                    {
                        sql = "SELECT MCARTON_NO, MODEL_NAME,VERSION_CODE, COUNT (MCARTON_NO) CARTON_QTY FROM SFISM4.Z_WIP_TRACKING_T WHERE MCARTON_NO='" + txtInput1.Text.ToUpper().Trim() + "' GROUP BY MCARTON_NO, MODEL_NAME,VERSION_CODE";
                    }
                    else
                    {
                        sql = "SELECT MCARTON_NO, MODEL_NAME,VERSION_CODE, COUNT (MCARTON_NO) CARTON_QTY FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO='" + txtInput1.Text.ToUpper().Trim() + "' GROUP BY MCARTON_NO, MODEL_NAME,VERSION_CODE";
                    }
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        showMessage("Not found MCARTON_NO " + txtInput1.Text.ToUpper(), "Không tìm thấy MCARTON_NO " + txtInput1.Text.ToUpper(), true);
                        txtInput1.SelectAll();
                        return;
                    }
                    M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                    if (M_MODEL_NAME != "26-2452-0451")
                    {
                        AddParams("ModelName", M_MODEL_NAME);
                        AddParams("MCartonNO", (_result.Data["mcarton_no"]?.ToString() ?? ""));
                        AddParams("Version", (_result.Data["version_code"]?.ToString() ?? ""));
                        AddParams("CartonQty", (_result.Data["carton_qty"]?.ToString() ?? ""));

                        labname = M_MODEL_NAME.Replace(".", "_") + "_CARTON.Lab";
                        if (Z107.IsChecked == true)
                        {
                            sql = "SELECT SERIAL_NUMBER FROM SFISM4.Z_WIP_TRACKING_T WHERE MCARTON_NO='" + txtInput1.Text.ToUpper().Trim() + "'";
                        }
                        else
                        {
                            sql = "SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO='" + txtInput1.Text.ToUpper().Trim() + "'";
                        }
                        var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_list.Data != null)
                        {
                            i = 0;
                            foreach (var row in result_list.Data)
                            {
                                i++;
                                AddParams("MSNB" + i, row["serial_number"]?.ToString() ?? "");
                            }
                        }
                        await P_PrintToCodeSoft("C", labname);
                        txtInput1.SelectAll();
                        return;
                    }
                    else
                    {
                        AddParams("ModelName", M_MODEL_NAME);
                        AddParams("MCartonNO", (_result.Data["mcarton_no"]?.ToString() ?? ""));
                        AddParams("Version", (_result.Data["version_code"]?.ToString() ?? ""));
                        AddParams("CartonQty", (_result.Data["carton_qty"]?.ToString() ?? ""));

                        labname = M_MODEL_NAME.Replace(".", "_") + "_CARTON.Lab";
                        if (Z107.IsChecked == true)
                        {
                            sql = "SELECT TO_NUMBER(SERIAL_NUMBER,'XXXXXXXXXX') AS SERIAL_NUMBER FROM SFISM4.Z_WIP_TRACKING_T WHERE MCARTON_NO='" + txtInput1.Text.ToUpper().Trim() + "'";
                        }
                        else
                        {
                            sql = "SELECT TO_NUMBER(SERIAL_NUMBER,'XXXXXXXXXX') AS SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO='" + txtInput1.Text.ToUpper().Trim() + "'";
                        }
                        var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_list.Data != null)
                        {
                            i = 0;
                            foreach (var row in result_list.Data)
                            {
                                i++;
                                AddParams("MSNB" + i, row["serial_number"]?.ToString() ?? "");
                            }
                        }
                        await P_PrintToCodeSoft("C", labname);
                        txtInput1.SelectAll();
                        return;
                    }
                    
                }
                //END tai hai them cho hang muon in carton label
                if (Kind.SelectedItem == chkBoxLabel)
                {
                    if (InputSN.IsChecked && txtMoNumber.Text.Length == 0)
                    {
                        txtInput2.Text = txtInput1.Text;
                    }
                    if (txtMoNumber.Text.Length != 0)
                    {
                        sql = "SELECT serial_number,mac2 FROM SFISM4.R_custsn_t  WHERE serial_number ='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            txtInput2.Text = _result.Data["mac2"]?.ToString() ?? "";
                        }
                    }
                    if (BB.IsChecked)
                    {
                        txtInput2.Text = txtInput1.Text;
                    }
                    if (string.IsNullOrEmpty(txtInput2.Text))
                    {
                        if (chkSSN.IsChecked == true)
                        {
                            sql = "SELECT serial_number FROM SFISM4.R_WIP_TRACKING_T  WHERE shipping_sn='" + txtInput1.Text + "'";
                            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result.Data != null)
                            {
                                txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";
                            }
                        }
                        txtInput2.Focus();
                        return;
                    }
                    if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("005") != -1)
                    {
                        if (string.IsNullOrEmpty(txtInput3.Text))
                        {
                            txtInput3.Focus();
                            return;
                        }
                    }

                    if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("093") != -1)
                    {
                        if (string.IsNullOrEmpty(txtInput2.Text))
                        {
                            txtInput2.Focus();
                            return;
                        }
                        sMO_TYPE = await findMOType();
                        if (string.IsNullOrEmpty(sMO_TYPE))
                        {
                            if (txtSeditfCount.Text == "1")
                            {
                                if (!await P_PrintToCodeSoft("C", G_sLabl_Name))
                                {
                                    return;
                                }
                            }
                            else
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow + ".Lab";
                                if (!await P_PrintToCodeSoft("C", labname))
                                    return;
                            }
                        }
                        txtInput1.Text = "";
                        txtInput2.Text = "";
                        txtInput3.Text = "";
                        txtInput1.Focus();
                        return;
                    }
                    if (rmaDataInput.IsChecked) //Chức năng này chưa có
                    {
                        if (string.IsNullOrEmpty(txtInput3.Text))
                        {
                            txtInput3.Focus();
                            return;
                        }
                        if (!string.IsNullOrEmpty(txtInput3.Text))
                        {
                            txtInput3.Focus();
                            return;
                        }
                    }
                    sMO_TYPE = await findMOType();

                    //lien sua
                    sql = "SELECT* FROM SFIS1.C_RECIPIENT_T A  WHERE MO_NUMBER IN(SELECT MO_NUMBER FROM SFISM4.R107 WHERE serial_number = '" + txtInput1.Text + "' AND CONTAINER_NO = A.RECIPIENT_NAME)";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        LOCATION = _result.Data["location"]?.ToString() ?? "";
                        RECIPIENT_NAME = _result.Data["recipient_name"]?.ToString() ?? "";
                        CTN_NAME = _result.Data["data1"]?.ToString() ?? "";
                        ADD_CODE = _result.Data["data2"]?.ToString() ?? "";

                    }
                    //lien sua

                    if (!printflag)
                    {
                        txtInput1.SelectAll();
                        return;
                    }
                    if (string.IsNullOrEmpty(sMO_TYPE))
                    {
                        return;
                    }
                    txtModelName.Text = M_MODEL_NAME;

                    //sql = "select * from sfis1.c_parameter_ini where prg_name='HH_PrintLabel' and vr_class='ROKU' and vr_value='Y' ";
                    //_result_check_param = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    //{
                    //    CommandText = sql,
                    //    SfcCommandType = SfcCommandType.Text
                    //});

                    if (check_roku_info) // (loginDB == "TEST")
                    {
                        if (sMO_TYPE == "SFIS")
                        { 
                            if (txtSedprtnow.Text == "0")
                            {
                                G_sLabl_Name = G_sLabl_Name;
                            }
                            else
                            {
                                G_sLabl_Name = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                            }
                            try
                            {
                                serialPort1.Write(data_com + "PASS\r\n");
                                await printBoxLabel_CodeSoft(sFindData, G_sLabl_Name);
                                Result = "OK";

                            }
                            catch (Exception ex)
                            {
                                showMessage("Can't open COM 6", "Không thể mở cổng COM 6", true);
                                return;
                            }
                        }
                        else
                        {
                            Result = "FAIL";
                            serialPort1.Write(data_com + "ERROR\r\n");
                            return;
                        }
                    }
                    else
                    {
                        if (sMO_TYPE == "SFIS" && txtSedprtnow.Text == "0" && txtSeditfCount.Text == "1")
                        {
                            sFindData = paramData;
                            await printBoxLabel_CodeSoft(sFindData, G_sLabl_Name);
                        }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text == "0")
                            {
                                for (int m = 1; m <= Int32.Parse(txtSeditfCount.Text) - 1; m++)
                                {
                                    labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + m + ".Lab";
                                    await printBoxLabel_CodeSoft(sFindData, labname);
                                }
                            }
                            else
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                await printBoxLabel_CodeSoft(sFindData, labname);
                            }
                        }
                    }
                }
                else
                {
                    sMO_TYPE = await findMOType();
                    if (sMO_TYPE == "SFIS")
                    {
                        if (Define.SelectedItem == chkCustomer && Kind.SelectedItem == chkCartonLabel)
                        {
                            if (!string.IsNullOrEmpty(G_sLabl_Name.Trim()) && G_sLabl_Name.Trim() != "N/A")
                            {
                                await printBoxLabel_CodeSoft(sFindData, G_sLabl_Name);
                            }
                        }
                        else
                        {
                            await GetAmbitData(sFindData);
                            await printToCodeSoft();
                        }
                    }
                    else if (sMO_TYPE == "LOT")
                    {
                        InputQty = Int32.Parse(inputBox("Khong tim thay du lieu Carton trong R107"));
                        if (M_iCartonCapacity == 0)
                        {
                            showMessage("00111", "00111", false);
                            return;
                        }
                        decimal deci = InputQty / M_iCartonCapacity;
                        iLBLQTY = Convert.ToInt32(Math.Truncate(deci));
                        iRQTY = InputQty % M_iCartonCapacity;
                        dtParams.Clear();
                        AddParams("Mo_Number", My_MoNumber);
                        AddParams("ModelName", M_MODEL_NAME);
                        if (string.IsNullOrEmpty(M_PART_NO))
                        {
                            AddParams("Part_No", M_MODEL_NAME);
                        }
                        else
                        {
                            AddParams("Part_No", M_PART_NO);
                        }
                        if (string.IsNullOrEmpty(sCarton))
                        {
                            sCarton = "Khong tim thay du lieu Carton trong R107";
                            AddParams("Carton_No", sCarton);
                            AddParams("Version", M_VERSION_CODE);
                            AddParams("PALLET_NO", sPallet);
                            AddParams("Carton_Qty", M_iCartonCapacity.ToString());
                        }
                        for (int m = 1; m <= iLBLQTY; m++)
                        {
                            await printToCodeSoft();
                        }
                        dtParams.Clear();
                        AddParams("Mo_Number", My_MoNumber);
                        AddParams("Mo", My_MoNumber);
                        AddParams("ModelName", M_MODEL_NAME);
                        if (string.IsNullOrEmpty(M_PART_NO))
                        {
                            AddParams("Part_No", M_MODEL_NAME);
                        }
                        else
                        {
                            AddParams("Part_No", M_PART_NO);
                            AddParams("Carton_No", sCarton);
                            AddParams("Version", M_VERSION_CODE);
                            AddParams("PALLET_NO", sPallet);
                            AddParams("Carton_Qty", iRQTY.ToString());
                        }
                        if (iRQTY > 0)
                            await printToCodeSoft();
                    }
                }
                //XUANQUY ADD PASS STATION ONLINE ROUTING
                if (!string.IsNullOrEmpty(txtStation.Text))
                {
                    T_RES = await F_Call_TestInput("N/A", txtInput1.Text);
                }
            }
            SystemSounds.Beep.Play();
            if (Define.SelectedItem == chkAmbit)
            {
                if (Kind.SelectedItem == chkCartonLabel)
                {
                    showMessage("00171", "00171", false);
                }
                else
                {
                    showMessage("00350", "00350", false);
                }
            }
            if (rmaDataInput.IsChecked)
            {
                //mmo_rmalist.Lines.Clear;
            }
            txtInput2.Clear();
            txtInput3.Clear();
            txtInput1.Clear();
            txtInput1.Focus();
            if (autoMation.IsChecked)
            {
                autoMation.IsChecked = true;
                frmCheckAcce frmCheckAcce = new frmCheckAcce();
                frmCheckAcce.sfcClient = sfcClient;
                frmCheckAcce.serialPort3 = serialPort3;
                frmCheckAcce.ShowDialog();
            }
        }
        private async Task<string> F_Call_TestInput(string inputEC, string inputSN)
        {
            sql = "Select SysDate DateTime,To_Char(SysDate,'yyyymmdd') Mo_Date, To_Char(SysDate, 'hh24mi') Time From Dual";
            var _result_time = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result_time.Data != null)
            {
                timelive = System.DateTime.Parse(_result_time.Data["datetime"].ToString());
                mo_date = _result_time.Data["mo_date"]?.ToString() ?? "";
                time_string = _result_time.Data["time"]?.ToString() ?? "";
            }
            string lineNew;
            if (!string.IsNullOrEmpty(txtLine.Text))
            {
                lineNew = txtLine.Text;
            }
            else
            {
                lineNew = "Default";
            }
            sql = "Select WORK_SECTION WRKSec From SFIS1.C_WORK_DESC_T WHERE START_TIME <='" + time_string + "' AND END_TIME >'" + time_string + "' AND LINE_NAME ='" + lineNew + "' AND SECTION_NAME = 'Default' AND SHIFT = '1'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

            var result_pro = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.TEST_INPUT",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="EMP",Value="Lot_reprint",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="DATA",Value=inputSN,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="LINE",Value=lineNew,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="SECTION",Value=My_Section,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="W_STATION",Value=M_Station,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="DATETIME",Value=timelive,SfcParameterDataType=SfcParameterDataType.Date,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="EC",Value=inputEC,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MO_DATE",Value=mo_date,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="W_SECTION",Value=Int32.Parse(_result.Data["wrksec"].ToString()),SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value=M_Group,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
            });
            dynamic ads = result_pro.Data;
            string Ok = ads[0]["res"];
            return Ok;
        }
        
        private string inputBox(string title)
        {
            string _inputBox;
            frmInputBox frmInputBox = new frmInputBox(title);
            frmInputBox.ShowDialog();
            frmInputBox.Title = title;
            _inputBox = frmInputBox.inputBox;
            return _inputBox;
        }
        private async Task printToCodeSoft()
        {
            if (Define.SelectedItem == chkAmbit)
            {
                if (Kind.SelectedItem == chkCartonLabel)
                {
                    sFileName = await labellocation() + "\\" + "C_Default.Lab";
                    G_sLabl_Name = "C_Default.Lab";
                }
                else
                {
                    sFileName = await labellocation() + "\\" + "P_Default.Lab";
                    G_sLabl_Name = "P_Default.Lab";
                }
            }
            else
            {
                if (Kind.SelectedItem == chkCartonLabel)
                {
                    sFileName = await labellocation() + "\\" + M_MODEL_NAME.Substring(0, 7) + "_" + M_MODEL_NAME.Substring(8, 2) + ".Lab";
                    G_sLabl_Name = M_MODEL_NAME.Substring(0, 7) + "_" + M_MODEL_NAME.Substring(8, 2) + ".Lab";
                }
                else
                {
                    sFileName = await labellocation() + "\\" + G_sLabl_Name;
                    G_sLabl_Name = M_MODEL_NAME.Substring(0, 7) + "_" + M_MODEL_NAME.Substring(8, 2) + ".Lab";
                }
            }
            if (publicfilepath != sFileName || !File.Exists(@sFileName))
            {
                if (locationlabel)
                {
                    if (!await downloadlabel(pubftppath, G_sLabl_Name))
                    {
                        showMessage("Download label file fail!", "Tải xuống tệp label lỗi!", true);
                        return;
                    }
                }
            }
            publicfilepath = sFileName;

            if (!File.Exists(@sFileName))
            {
                SystemSounds.Beep.Play();
                showMessage("The label file could not find " + @sFileName, "Tim khong thay File Label " + @sFileName, true);
                return;
            }
            if (string.IsNullOrEmpty(G_sLabl_Name))
            {
                G_sLabl_Name = LabelFileName_ftp;
                if (string.IsNullOrEmpty(G_sLabl_Name))
                {
                    showMessage("00057", "00057", false);
                    return;
                }
            }
            if (await isCheckLabel())
            {
                if (!await P_PrintToCodeSoft("C", sFileName))
                    return;
            }
            else
            {
                if (!await P_PrintToCodeSoft("C", sFileName))
                    return;
            }
        }
        private async Task GetAmbitData(string FindData)
        {
            string table;
            if (Z107.IsChecked == true)
            {
                table = "SFISM4.Z107";
            }
            else
            {
                table = "SFISM4.R107";
            }
            sql = "select count(ROWID) aa from " + table;
            if (Kind.SelectedItem == chkCartonLabel)
            {
                sql += " where carton_no = '" + FindData + "'";
            }
            else
            {
                sql += " where pallet_no = '" + FindData + "'";
            }
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                AddParams("Carton_Qty", _result.Data["aa"]?.ToString() ?? "");
            }
        }
        private async Task printBoxLabel_CodeSoft(string paramBox, string paramLabelFile)
        {

            string sMACID, sGetPoNo;

            if (txtMoNumber.Text.Length == 0)
            {
                AddParams("SN", txtInput1.Text);
                AddParams("SN1", txtInput1.Text);
                AddParams("Serial", txtInput1.Text);
                AddParams("MSN", txtInput2.Text);
                AddParams("MSN1", txtInput2.Text);
                AddParams("MSN2", txtInput3.Text);
                AddParams("BoxSN", txtInput2.Text);

                //Label box CPEII Sunny confirm
                sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LOT REPRINT' AND VR_CLASS='SCAN MAC'";
                var _result_time = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result_time.Data != null)
                {
                    sql = "SELECT SHIPPING_SN FROM SFISM4.R_WIP_TRACKING_T WHERE SHIPPING_SN2 = '" + txtInput2.Text + "'";
                    _result_time = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result_time.Data != null && (_result_time.Data["shipping_sn"]?.ToString() ?? "") != "N/A" && !string.IsNullOrEmpty((_result_time.Data["shipping_sn"]?.ToString() ?? "")))
                        AddParams("BoxSN", (_result_time.Data["shipping_sn"]?.ToString() ?? ""));
                }
                //END Label box CPEII Sunny confirm

                sql = "select wpa_key from sfism4.r_r06_ssnkey_t where ssn='" + txtInput2.Text.ToUpper().Trim() + "'";
                _result_time = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result_time.Data != null)
                {
                    AddParams("WAP", (_result_time.Data["wpa_key"]?.ToString() ?? ""));
                }
            }

            AddParams("SPID", SPID); //GET RECIPIENT_NAME
            AddParams("RECIPIENT_NAME", RECIPIENT_NAME);
            AddParams("LOCATION", LOCATION);
            AddParams("MMC_FLAG", MMC_FLAG);
            AddParams("DDR_NAME", DDR_NAME);
            AddParams("SOC_NAME", SOC_NAME);
            AddParams("CTN_NAME", CTN_NAME);
            AddParams("ADD_CODE", ADD_CODE);

            sql = "SELECT * FROM SFISM4.R_wip_keyparts_t where key_part_sn='" + txtInput1.Text.Trim() + "' and rownum=1 ";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                AddParams("D_SN", _result.Data["serial_number"]?.ToString() ?? "");
            }

            int i = 0;
            sql = "SELECT * FROM SFISM4.r108 WHERE serial_number='" + txtInput1.Text + "' and key_part_no <>'MACID' ";
            var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data != null)
            {
                foreach (var row in result_list.Data)
                {
                    AddParams("T_SN" + i, (row["key_part_sn"]?.ToString() ?? ""));
                    i = i + 1;
                    string sql1 = "SELECT * FROM SFISM4.R_wip_keyparts_t where SERIAL_NUMBER='" + row["key_part_sn"].ToString() + "' AND KEY_PART_NO<>'MACID' ";
                    var result_keypart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql1,
                        SfcCommandType = SfcCommandType.Text
                    });

                    foreach (var row1 in result_keypart.Data)
                    {
                        AddParams("TT_SN" + i, (row1["key_part_sn"]?.ToString() ?? ""));
                    }
                }

                sql = "SELECT * FROM SFISM4.R_wip_keyparts_t where SERIAL_NUMBER='" + txtInput1.Text + "' and key_part_no<>'MACID' AND length(key_part_sn)<>'7' AND rownum=1 ";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (_result.Data != null)
                {
                    AddParams("T_SSN", _result.Data["key_part_sn"]?.ToString() ?? "");
                }

            }

            //XUANQUY ADD PROC_DATE 20211021
            sql = "SELECT to_char(SYSDATE,'Month') PRODATE FROM DUAL";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                AddParams("SHORT_MON", _result.Data["prodate"]?.ToString() ?? "");
            }

            sql = "SELECT TO_CHAR(SYSDATE,'MM') PRODATE FROM DUAL";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                AddParams("PROC_MON", _result.Data["prodate"]?.ToString() ?? "");
            }

            sql = "SELECT TO_CHAR(SYSDATE,'YYYY') PRODATE FROM DUAL";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                AddParams("PROC_YEAR", _result.Data["prodate"]?.ToString() ?? "");
            }

            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("110") != -1)
                await iDTAprint();

            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("C") != -1)
            {
                if (!await FINDNORTELMACID(txtInput1.Text))
                {
                    AddParams("MAC", "N/A");
                    AddParams("MAC1", "N/A");
                }
                else
                {
                    AddParams("MAC", macID);
                }
            }
            else
            {
                if (!await findMacID(txtInput1.Text))
                {
                    if (!await FINDHMACID(txtInput1.Text) && txtMoNumber.Text.Length == 0)
                    {
                        sql = "select MAC1, SERIAL_NUMBER from sfism4.R_CUSTSN_T where serial_number='" + txtInput1.Text.Trim() + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            AddParams("MAC", _result.Data["mac1"]?.ToString() ?? "");
                            AddParams("SSN", _result.Data["serial_number"]?.ToString() ?? "");
                        }
                        AddParams("MAC1", "N/A");
                    }
                    else
                    {
                        sMACID = macID;
                        if (!string.IsNullOrEmpty(sMACID))
                        {
                            AddParams("MAC", sMACID);
                            AddParams("MAC1", sMACID);
                            AddParams("MTA_MAC", await add2tomacid(sMACID));
                            AddParams("USB_MAC", await add1tomacid(sMACID));
                            AddParams("NETWORK", "Thomson" + (await add1tomacid(sMACID)).Substring(6, 6));
                            AddParams("WIFI_MAC", await add6tomacid(sMACID));
                        }
                    }
                }
                else
                {
                    sMACID = macID;
                    AddParams("MAC", sMACID);
                    AddParams("MAC1", sMACID);
                    AddParams("MTA_MAC", await add2tomacid(sMACID));
                    AddParams("USB_MAC", await add1tomacid(sMACID));
                    AddParams("ETH_MAC", red1tomacid(sMACID));
                    AddParams("NETWORK", "Thomson" + (await add1tomacid(sMACID)).Substring(6, 6));
                    AddParams("WIFI_MAC", await add6tomacid(sMACID));
                }
            }
            sMACID = macID;
            if (CancelWhenMacisNA.IsChecked && Kind.SelectedItem == chkBoxLabel && !String.IsNullOrEmpty(model_type) && model_type.IndexOf("G76") == -1)
            {
                if ((sMACID == "N/A" || string.IsNullOrEmpty(sMACID)) && txtMoNumber.Text.Length == 0)
                {
                    if (checkMac.IsChecked)
                    {
                        showMessage("00310", "00310", false);
                        return;
                    }
                }
            }
            if (DCI5211.IsChecked)
            {
                if (!await P_PrintToCodeSoft("C", paramLabelFile))
                    return;
            }
            else
            {
                if (!logic)
                {
                    return;
                }
                if (!await P_PrintToCodeSoft("C", paramLabelFile))
                    return;
            }
            try
            {
                sql = "SELECT count(ROWID) aa FROM   SFIS1.C_MODELTYPE_MATCH_T WHERE MODEL_NAME='" + M_MODEL_NAME + "'  and MODEL_DESC='REPRINT' and MODEL_TYPE<>'REPRINT'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (Int32.Parse(_result.Data["aa"].ToString()) > 0)
                {
                    sql = "update sfism4.r_wip_tracking_t set BILL_NO='" + (tmp_msn + "LT" + txtSeditLabelQty.Text + txtSeditfCount.Text + txtSedprtnow.Text).Substring(0, 25) + "' where serial_number = '" + txtInput1.Text + "'";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return;
                    }
                }
                if (LinkSNSSN.IsChecked && Kind.SelectedItem == chkBoxLabel && !String.IsNullOrEmpty(model_type) && model_type.IndexOf("011") == -1)
                {
                    sql = "update sfism4.r_wip_tracking_t set Po_No='" + txtInput2.Text + "' where (serial_number = '" + txtInput1.Text + "'  or  shipping_sn = '" + txtInput1.Text + "') and " +
                        " ((Po_No='') or (Po_No='N/A') or (Po_No is NULL)) ";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return;
                    }

                    sql = "SELECT count(model_name) aa FROM   SFIS1.C_MODELTYPE_MATCH_T WHERE MODEL_NAME='" + M_MODEL_NAME + "'  and MODEL_DESC='REPRINT'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (Int32.Parse(_result.Data["aa"].ToString()) > 0)
                    {
                        if (tmp_msn.Length >= 22)
                        {
                            sql = "update sfism4.r_wip_tracking_t set BILL_NO='" + (tmp_msn + "LT" + txtSeditLabelQty.Text + txtSeditfCount.Text + txtSedprtnow.Text).Substring(0, 25) + "' where serial_number = '" + txtInput1.Text + "'";
                        }
                        else
                        {
                            sql = "update sfism4.r_wip_tracking_t set BILL_NO='" + (tmp_msn + "LT" + txtSeditLabelQty.Text + txtSeditfCount.Text + txtSedprtnow.Text) + "' where serial_number = '" + txtInput1.Text + "'";
                        }
                        query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async Task<string> GetPoNo()
        {
            sql = "Select Po_No From SFISM4.R_Wip_Tracking_T Where Serial_Number='" + txtInput1.Text + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                return "";
            }
            else
            {
                string po_no = _result.Data["po_no"]?.ToString() ?? ""; ;
                if (po_no == "N/A")
                {
                    return "";
                }
                else
                {
                    return po_no;
                }
            }
        }
        private async Task<bool> FINDHMACID(string sn)
        {
            bool ContinueFindMACID = true;
            string LastKPSN = string.Empty;

            sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_NO = 'MACID' AND SERIAL_NUMBER = '" + sn + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                sql = "SELECT key_part_sn FROM SFISM4.R_WIP_KEYPARTS_T WHERE GROUP_NAME = 'ASSY' AND SERIAL_NUMBER = '" + sn + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    LastKPSN = _result.Data["key_part_sn"]?.ToString() ?? "";
                }

                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("B") != -1)
                {
                    sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE Group_name<>'ASSY' AND SERIAL_NUMBER =:sn";
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_NO <> KEY_PART_SN AND SERIAL_NUMBER =:sn";
                }
            }
            while (ContinueFindMACID)
            {
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "sn", Value = sn }
                    }
                });
                if (_result.Data == null)
                {
                    ContinueFindMACID = false;
                    return false;
                }
                else
                {
                    LastKPSN = _result.Data["key_part_sn"]?.ToString() ?? "";
                    if (_result.Data["key_part_no"].ToString() == "MACID")
                    {
                        macID = LastKPSN;
                        ContinueFindMACID = false;
                        return true;
                    }
                    else
                    {
                        sn = LastKPSN;
                    }
                }
            }
            return true;
        }
        private async Task<bool> FINDNORTELMACID(string sn)
        {
            string LastKPSN, M_BOM = string.Empty;
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("C") != -1)
            {
                sql = "SELECT BOM_NO FROM SFISM4.R_WIP_TRACKING_T WHERE  SERIAL_NUMBER = '" + sn + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    M_BOM = _result.Data["bom_no"]?.ToString() ?? "";
                }

                sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE  SERIAL_NUMBER = '" + sn + "' AND KEY_PART_NO= " +
                    " (select key_part_no from SFIS1.C_BOM_KEYPART_T where bom_no='" + M_MODEL_NAME + "' and type='MACID')";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    LastKPSN = _result.Data["key_part_sn"]?.ToString() ?? "";

                    sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE  SERIAL_NUMBER = '" + LastKPSN + "' and KEY_PART_NO='MACID'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        macID = _result.Data["key_part_sn"]?.ToString() ?? "";
                    }
                    else
                    {
                        sql = "SELECT * FROM SFISM4.H_WIP_KEYPARTS_T WHERE  SERIAL_NUMBER = '" + LastKPSN + "' and KEY_PART_NO='MACID'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            macID = _result.Data["key_part_sn"]?.ToString() ?? "";
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T  A,SFISM4.R_WIP_TRACKING_T B WHERE  A.SERIAL_NUMBER = '" + sn + "' AND  B.SERIAL_NUMBER=A.SERIAL_NUMBER AND " +
                        " A.KEY_PART_NO='MACID'  AND   B.MODEL_NAME=(select key_part_no from SFIS1.C_BOM_KEYPART_T where bom_no='" + M_BOM + "' and type='MACID')";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        macID = _result.Data["key_part_sn"]?.ToString() ?? "";
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private async Task<bool> FindMcarton(string strdata)
        {
            int intX, intY;
            string labname, Modeldesc, ModelName, myPALLET = string.Empty, sssQTY = string.Empty, PO_NO = string.Empty, PONUMBER, PO_LINE, CUST_PALLET = string.Empty, tocom, TA_VERSION = string.Empty;
            //KHOA.PD ADD IN 2018-01-05
            sql = "SELECT DISTINCT SSN2 FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER IN(SELECT SERIAL_NUMBER FROM SFISM4.Z_WIP_TRACKING_T " +
                " WHERE IMEI='" + txtInput1.Text + "' OR MCARTON_NO='" + txtInput1.Text + "' OR SHIPPING_SN='" + txtInput1.Text + "' " +
                " OR SHIPPING_SN2='" + txtInput1.Text + "' OR SERIAL_NUMBER='" + txtInput1.Text + "')";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                TA_VERSION = _result.Data["ssn2"]?.ToString() ?? "";
            }
            //END KHOA.PD ADD IN 2018-01-05
            if (Z107.IsChecked == false)
            {
                sql = "select count(ROWID) QTY from sfism4.r_wip_tracking_t where imei= '" + strdata + "'";
            }
            else
            {
                sql = "select count(ROWID) QTY from sfism4.z_wip_tracking_t where imei= '" + strdata + "'";
            }
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                sssQTY = _result.Data["qty"]?.ToString() ?? "";
            }
            if (Z107.IsChecked == false)
            {
                sql = "SELECT PO_NO FROM SFIS1.C_PO_CONFIG_t WHERE MODEL_NAME IN(select distinct MODEL_NAME from sfism4.r_wip_tracking_t where imei='" + strdata + "' AND ROWNUM=1)";
            }
            else
            {
                sql = "SELECT PO_NO FROM SFIS1.C_PO_CONFIG_t WHERE MODEL_NAME IN(select distinct MODEL_NAME from sfism4.z_wip_tracking_t where imei='" + strdata + "' AND ROWNUM=1)";
            }
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                PO_NO = _result.Data["po_no"]?.ToString() ?? "";
            }
            intX = 0;
            if (SSNLABEL.IsChecked)
            {
                if (Z107.IsChecked == false)
                {
                    sql = "select distinct SHIPPING_SN,model_name from sfism4.r_wip_tracking_t where imei= '" + strdata + "'";
                }
                else
                {
                    sql = "select distinct SHIPPING_SN,model_name from sfism4.z_wip_tracking_t where imei= '" + strdata + "'";
                }
                var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data != null)
                {
                    dtParams.Clear();
                    foreach (var row in result_list.Data)
                    {
                        intX = intX + 1;
                        AddParams("MSN" + intX, row["shipping_sn"]?.ToString() ?? "");
                        if (intX == 36)
                        {
                            intX = 0;
                            M_MODEL_NAME = row["model_name"]?.ToString() ?? "";
                            intY = M_MODEL_NAME.IndexOf(".");
                            if (intY == -1)
                            {
                                labname = M_MODEL_NAME + "_PMSN.Lab";
                            }
                            else
                            {
                                labname = M_MODEL_NAME.Substring(0, intY - 1) + M_MODEL_NAME.Substring(intY, M_MODEL_NAME.Length - intY) + "_PMSN.Lab";
                            }
                            //Cần xem lại đoạn này call print trong for
                            if (!await P_PrintToCodeSoft("C", labname))
                                break;
                        }
                    }
                }
            }
            else
            {
                if (Z107.IsChecked == false)
                {
                    if (M_MODEL_NAME.Substring(0, 4) == "T77W")
                    {
                        sql = "select distinct model_name,carton_no,mcarton_no,pallet_no,IMEI from sfism4.r_wip_tracking_t " +
                            " where imei =(select imei from sfism4.r_wip_tracking_t where (imei='" + strdata + "')  group by imei) " +
                            " group by model_name,carton_no,mcarton_no,pallet_no,IMEI ORDER BY MCARTON_NO";
                    }
                    else
                    {
                        sql = "select distinct model_name,carton_no,mcarton_no,pallet_no,IMEI from sfism4.r_wip_tracking_t " +
                            " where imei =(select imei from sfism4.r_wip_tracking_t where (imei='" + strdata + "') or (mcarton_no='" + strdata + "') group by imei) " +
                            " group by model_name,carton_no,mcarton_no,pallet_no,IMEI ORDER BY MCARTON_NO";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(M_MODEL_NAME) && M_MODEL_NAME.Substring(0, 4) == "T77W")
                    {
                        sql = "select distinct model_name,carton_no,mcarton_no,pallet_no,IMEI from sfism4.z_wip_tracking_t " +
                            " where imei =(select imei from sfism4.z_wip_tracking_t where (imei='" + strdata + "')  group by imei) " +
                            " group by model_name,carton_no,mcarton_no,pallet_no,IMEI ORDER BY MCARTON_NO";
                    }
                    else
                    {
                        sql = "select distinct model_name,carton_no,mcarton_no,pallet_no,IMEI from sfism4.z_wip_tracking_t " +
                            " where imei =(select imei from sfism4.z_wip_tracking_t where (imei='" + strdata + "') or (mcarton_no='" + strdata + "') group by imei) " +
                            " group by model_name,carton_no,mcarton_no,pallet_no,IMEI ORDER BY MCARTON_NO";
                    }
                }
                var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data != null)
                {
                    dtParams.Clear();
                    foreach (var row in result_list.Data)
                    {
                        intX = intX + 1;
                        M_MODEL_NAME = row["model_name"]?.ToString() ?? "";
                        myPALLET = row["imei"]?.ToString() ?? "";
                        CUST_PALLET = row["imei"]?.ToString() ?? "";
                        AddParams("MCTN" + intX, row["mcarton_no"]?.ToString() ?? "");
                        AddParams("CUST_PALLET_NO" + intX, row["imei"]?.ToString() ?? "");
                        if (Z107.IsChecked == false)
                        {
                            AddParams("CTN" + intX, row["carton_no"]?.ToString() ?? "");
                        }
                    }
                }
            }
            if (M_MODEL_NAME.Substring(0, 3) == "I01")
            {
                if (Z107.IsChecked == false)
                {
                    sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=( SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) AND " +
                        " VERSION_CODE=(SELECT VERSION_CODE FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) ";
                }
                else
                {
                    sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=( SELECT MODEL_NAME FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) AND " +
                        " VERSION_CODE=(SELECT VERSION_CODE FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) ";
                }
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    tocom = _result.Data["upceandata"]?.ToString() ?? "";
                    Modeldesc = _result.Data["cust_model_desc"]?.ToString() ?? "";
                    AddParams("UPCEACDATA", tocom);
                    AddParams("PART_NO", M_MODEL_NAME);
                    AddParams("MODELDESC", Modeldesc);
                    AddParams("CUST_PART_NO", myPALLET);
                    AddParams("QTY", intX.ToString("000"));
                    AddParams("SN_QTY", sssQTY);
                    AddParams("CUST_PALLET_NO", CUST_PALLET);
                    AddParams("TA_VERSION", TA_VERSION);
                }
            }
            else
            {
                if (Z107.IsChecked == false)
                {
                    sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=( SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) AND " +
                        " VERSION_CODE=(SELECT VERSION_CODE FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) ";
                }
                else
                {
                    sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=( SELECT MODEL_NAME FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) AND " +
                        " VERSION_CODE=(SELECT VERSION_CODE FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) ";
                }
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    tocom = _result.Data["upceandata"]?.ToString() ?? "";
                    Modeldesc = _result.Data["cust_model_desc"]?.ToString() ?? "";
                    ModelName = _result.Data["model_name"]?.ToString() ?? "";
                    AddParams("PART_NO", M_MODEL_NAME);
                    AddParams("ModelName", ModelName);
                    AddParams("MODELDESC", Modeldesc);
                    AddParams("UPCEACDATA", tocom);
                    AddParams("PALLET_NO", myPALLET);
                    AddParams("CUST_PART_NO", CUST_PALLET);
                    AddParams("IMEI", CUST_PALLET);
                    AddParams("QTY", intX.ToString());
                    AddParams("SN_QTY", sssQTY);
                    AddParams("CUST_PALLET_NO", CUST_PALLET);
                    AddParams("TA_VERSION", TA_VERSION);
                    AddParams("PO_NO", PO_NO);
                }
                if (poLine.IsChecked)
                {
                    if (Z107.IsChecked == true)
                    {
                        sql = "SELECT COUNT(CUST_PO) C_COUNT FROM SFISM4.R_BPCS_INVOICE_T WHERE TCOM IN (SELECT SHIP_NO FROM SFISM4.Z107 WHERE IMEI='" + strdata + "')";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if(_result.Data["c_count"].ToString() != "1")
                        {
                            showMessage("Pallet nay ton tai " + _result.Data["c_count"].ToString() + " PO", "Pallet nay ton tai " + _result.Data["c_count"].ToString() + " PO", true);
                            return false;
                        }

                        sql = "SELECT CUST_PO FROM SFISM4.R_BPCS_INVOICE_T WHERE TCOM IN (SELECT SHIP_NO FROM SFISM4.Z107 WHERE IMEI='" + strdata + "')";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            PONUMBER = _result.Data["cust_po"]?.ToString() ?? "";
                            AddParams("POLINE", PONUMBER);
                        }
                    }
                }
            }
            return true;
        }
        private async Task<bool> FindMcarton_I01_34(string strdata)
        {
            string labname, sQTY = string.Empty, Modeldesc = string.Empty, tmpssn, page, R107_TABLE;
            int intX, inty, intQty = 0, allqty, tmppage, allpageqty = 0, i, tmpqty;
            bool PRINTOTHERFLAG;
            intX = 0;
            if (Z107.IsChecked == true)
            {
                R107_TABLE = "SFISM4.Z107";
            }
            else
            {
                R107_TABLE = "SFISM4.R107";
            }
            sql = "select count(rowid) QTY from " + R107_TABLE + " where imei='" + strdata + "' ";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                sQTY = _result.Data["qty"]?.ToString() ?? "";
                intQty = Int32.Parse(sQTY);
            }

            sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=(SELECT MODEL_NAME FROM " + R107_TABLE + " WHERE IMEI='" + strdata + "' AND ROWNUM=1) AND " +
                "VERSION_CODE=(SELECT VERSION_CODE FROM " + R107_TABLE + " WHERE IMEI='" + strdata + "' AND ROWNUM=1)";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                Modeldesc = _result.Data["cust_model_desc"]?.ToString() ?? "";
                M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
            }

            sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='" + M_MODEL_NAME + "' AND VR_CLASS='LOT_PRINT' AND VR_ITEM='PALTLIST'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                PRINTOTHERFLAG = true;
                tmpssn = _result.Data["vr_name"]?.ToString() ?? "";
            }
            else
            {
                PRINTOTHERFLAG = false;
            }
            if (SSNLABEL.IsChecked)
            {
                if (PRINTOTHERFLAG)
                {
                    sql = "select a.serial_number,b.ssn4 SHIPPING_SN,A.MODEL_NAME from " + R107_TABLE + " a,sfism4.r_custsn_t b " +
                        " where a.imei='" + strdata + "' and a.serial_number=b.serial_number ";
                }
                else
                {
                    sql = "select distinct SHIPPING_SN,model_name from " + R107_TABLE + " where imei='" + strdata + "'";
                }
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    dtParams.Clear();
                    AddParams("CUST_PALLET_NO", strdata);
                    AddParams("QTY", sQTY);
                    AddParams("MODELDESC", Modeldesc);
                    tmpqty = intQty;
                    int ceiling = Int32.Parse(sQTY) / PageQTY;
                    decimal cei = ceiling;
                    page = Math.Ceiling(cei).ToString();
                    AddParams("PAGE", page);
                    if (PageQTY == 0)
                    {
                        PageQTY = intQty;
                        allpageqty = 1;
                    }
                    else
                    {
                        for (int j = 1; j <= 10; j++)
                        {
                            tmpqty = tmpqty - PageQTY;
                            if (tmpqty < 0)
                            {
                                break;
                            }
                            allpageqty = allpageqty + 1;
                        }
                    }
                    allqty = 0;
                    tmppage = 0;
                    var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_list.Data != null)
                    {
                        foreach (var row in result_list.Data)
                        {
                            intX = intX + 1;
                            allqty = allqty + 1;
                            AddParams("MSN" + intX, row["shipping_sn"]?.ToString() ?? "");
                            if (intX == PageQTY || allqty.ToString() == sQTY)
                            {
                                tmppage = tmppage + 1;
                                M_MODEL_NAME = row["model_name"]?.ToString() ?? "";
                                inty = M_MODEL_NAME.IndexOf(".");
                                AddParams("CUST_PALLET_NO", strdata);
                                AddParams("QTY", sQTY);
                                AddParams("MODELDESC", Modeldesc);
                                AddParams("ModelName", M_MODEL_NAME);
                                AddParams("Allpage", page);
                                AddParams("Pageqty", tmppage.ToString());
                                if (inty == -1)
                                {
                                    labname = M_MODEL_NAME + "_PMSN.Lab";
                                }
                                else
                                {
                                    labname = M_MODEL_NAME.Substring(0, inty - 1) + M_MODEL_NAME.Substring(inty, M_MODEL_NAME.Length - inty) + "_PMSN.Lab";
                                }
                                //Chỗ này gọi lệnh in trong for cần xem lại
                                if (!await P_PrintToCodeSoft("C", labname))
                                    break;
                            }
                        }
                    }
                    else
                    {
                        sql = "select distinct model_name,carton_no,mcarton_no,IMEI from " + R107_TABLE + " where imei =(select imei from " + R107_TABLE +
                            " where (imei='" + strdata + "') or (mcarton_no='" + strdata + "') group by imei) group by model_name,carton_no,mcarton_no,IMEI";
                        result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_list.Data != null)
                        {
                            dtParams.Clear();
                            foreach (var row in result_list.Data)
                            {
                                M_MODEL_NAME = row["model_name"]?.ToString() ?? "";
                                intX = intX + 1;
                                AddParams("MCTN" + intX, row["mcarton_no"]?.ToString() ?? "");
                                AddParams("CTN" + intX, row["carton_no"]?.ToString() ?? "");
                                AddParams("CUST_PALLET_NO" + intX, row["imei"]?.ToString() ?? "");
                            }
                        }
                    }
                }
            }
            return true;
        }
        private async Task<bool> FindShippingData168(string IMEI)
        {
            string tcom, invoice, inv_no, finish, pallid, model, sequid, shipqty = string.Empty, vercode, labname;
            int i, inty;
            sql = "select ship_sequence,pallet_id,so_number,cust_po from SFISM4.R_SHIP_PALLET_T where pallet_no='" + IMEI + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                sql = "select version_code from sfism4.z107 where imei='" + IMEI + "' and rownum=1";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("UnKnow IMEI in z107: " + IMEI, "UnKnow IMEI in z107: " + IMEI, true);
                    return false;
                }
                vercode = _result.Data["version_code"]?.ToString() ?? "";

                sql = "select tcom,invoice,inv_no,model_name,FINISH_DATE from sfism4.r_bpcs_invoice_t where tcom is not null and tcom=( " +
                    " select ship_no from sfism4.z107 a where imei='" + IMEI + "' and wip_group='SHIPPING' and rownum=1 ) order by invoice";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("This IMEI: " + IMEI + " not shipping, places scann shipping first!", "This IMEI: " + IMEI + " not shipping, places scann shipping first!", true);
                    return false;
                }
                tcom = _result.Data["tcom"]?.ToString() ?? "";
                invoice = _result.Data["invoice"]?.ToString() ?? "";
                inv_no = _result.Data["inv_no"]?.ToString() ?? "";
                model = _result.Data["model_name"]?.ToString() ?? "";
                finish = _result.Data["finish_date"]?.ToString() ?? "";

                sql = "select ship_sequence,max(pallet_id) pallet_id  from SFISM4.R_SHIP_PALLET_T where invoice='" + invoice + "' group by invoice,ship_sequence";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    sequid = _result.Data["ship_sequence"]?.ToString() ?? "";
                    pallid = Int32.Parse(_result.Data["pallet_id"].ToString()).ToString("000");
                }
                else
                {
                    sql = "select max(ship_sequence) ship_sequence from SFISM4.R_SHIP_PALLET_T";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        i = 0;
                    }
                    else
                    {
                        i = Int32.Parse(_result.Data["ship_sequence"].ToString());
                    }
                    i = i + 1;
                    sequid = i.ToString("0000");
                    pallid = i.ToString("000");
                }
                sql = "select count(distinct imei) bb from sfism4.z107 where ship_no='" + tcom + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    shipqty = Int32.Parse(_result.Data["bb"].ToString()).ToString("000");
                }
                sql = "insert into SFISM4.R_SHIP_PALLET_T(pallet_id,pallet_no,inv_no,invoice,ship_date,emp_no,cust_po,tcom,model_name,ship_sequence,so_number) " +
                    "values('" + pallid + "' , '" + IMEI + "','" + inv_no + "','" + invoice + "',sysdate,'lotPrint','" + vercode + "','" + tcom + "','" + model + "','" + sequid + "','" + shipqty + "')";
                var query_insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (query_insert.Result.ToString() != "OK")
                {
                    showMessage("Exception: " + query_insert.Message.ToString() + "\nsql: " + sql, "Exception: " + query_insert.Message.ToString() + "\nsql: " + sql, true);
                    return false;
                }
            }
            else
            {
                sequid = _result.Data["ship_sequence"]?.ToString() ?? "";
                pallid = _result.Data["pallet_id"]?.ToString() ?? "";
                shipqty = _result.Data["so_number"]?.ToString() ?? "";
                vercode = _result.Data["cust_po"]?.ToString() ?? "";
            }
            return true;
        }
        private async Task<bool> FindMcarton_DCI(string strdata)
        {
            string batch = string.Empty, firstrange = string.Empty, pallet = string.Empty, Modeldesc = string.Empty, tocom = string.Empty, QTY = string.Empty, page = string.Empty, INSERT_PO, haveBatch, S_Table, S_SHIP, S_BATCHMAX, BATCH_TYPE, endrange, SO_PRE, labname;
            int intX, inty, qtyindex, pageindex, i_batch, a, b, k, i, j;

            if (Z107.IsChecked == true)
            {
                S_Table = "sfism4.z107";
            }
            else
            {
                S_Table = "sfism4.r107";
            }
            intX = 0;
            qtyindex = 0;
            pageindex = 1;

            if (SSNLABEL.IsChecked)
            {
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("133") != -1)
                {
                    if (Z107.IsChecked != true)
                    {
                        sql = "select imei from sfism4.r_wip_tracking_t where (serial_number ='" + strdata + "' or shipping_sn ='" + strdata + "' or mcarton_no='" + strdata + "' or carton_no='" + strdata + "' or imei='" + strdata + "') and rownum=1";
                    }
                    else
                    {
                        sql = "select imei from sfism4.z_wip_tracking_t where (serial_number ='" + strdata + "' or shipping_sn ='" + strdata + "' or mcarton_no='" + strdata + "' or carton_no='" + strdata + "' or imei='" + strdata + "') and rownum=1";
                    }
                    var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        strdata = _result.Data["imei"]?.ToString() ?? "";
                        pallet = strdata;
                    }
                    if (Z107.IsChecked != true)
                    {
                        sql = "select distinct A.SHIPPING_SN shipping_sn,A.model_name model_name,B.KEY_PART_SN key_part_sn,C.SSN2 SSN2 from " +
                            " sfism4.r_wip_tracking_t A,SFISM4.R108 B,SFISM4.R_CUSTSN_T C where A.SERIAL_NUMBER=C.SERIAL_NUMBER AND A.SERIAL_NUMBER=B.SERIAL_NUMBER " +
                            " AND B.KEY_PART_NO='MACID' AND  A.imei='" + strdata + "'";
                    }
                    else
                    {
                        sql = "select distinct A.SHIPPING_SN shipping_sn,A.model_name model_name,B.KEY_PART_SN key_part_sn from " +
                            " sfism4.z_wip_tracking_t A,SFISM4.R108 B where A.SERIAL_NUMBER=B.SERIAL_NUMBER " +
                            " AND B.KEY_PART_NO=''MACID'' AND  A.imei='" + strdata + "' ";
                    }
                    var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_list.Data == null)
                    {
                        return false;
                    }
                    else
                    {
                        foreach (var row in result_list.Data)
                        {
                            M_MODEL_NAME = row["model_name"]?.ToString() ?? "";
                            intX = intX + 1;
                            qtyindex = qtyindex + 1;

                            if (intX == 1)
                            {
                                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("133") == -1)
                                {
                                    if (string.IsNullOrEmpty(batch))
                                    {
                                        //thieu form do chua design
                                        /*if batch='' then  batch:=inputbox(GetPubMessage('00337',dbReprint),GetPubMessage('00337',dbReprint),'');
                                        batch:=trim(StrUpper(PChar(batch)));*/
                                    }
                                    if (string.IsNullOrEmpty(pallet))
                                    {
                                        /*if pallet='' then pallet:=inputbox(GetPubMessage('00338',dbReprint),GetPubMessage('00338',dbReprint),'');
                                        pallet:=trim(StrUpper(PChar(pallet)));*/
                                    }
                                }
                                sql = "SELECT COUNT(ROWID) QTY FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + strdata + "'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    if (PageQTY == 0)
                                    {
                                        PageQTY = Int32.Parse(_result.Data["qty"].ToString());
                                    }
                                    if (Int32.Parse(_result.Data["qty"].ToString()) > PageQTY)
                                        page = "2";
                                    if (Int32.Parse(_result.Data["qty"].ToString()) < PageQTY + 1)
                                        page = "1";
                                }
                                sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=(SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI=:SN AND ROWNUM=1) AND " +
                                    "VERSION_CODE=(SELECT VERSION_CODE FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1)";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    tocom = _result.Data["upceandata"]?.ToString() ?? "";
                                    Modeldesc = _result.Data["cust_model_desc"]?.ToString() ?? "";
                                }

                                dtParams.Clear();
                                AddParams("BATCH", batch);
                                AddParams("PALLET", pallet);
                                AddParams("TOCOM", tocom);
                                AddParams("MODELDESC", Modeldesc);
                                AddParams("BATCH", batch);
                                AddParams("SN_QTY", QTY);
                                AddParams("PAGE", pageindex.ToString() + "/" + page);
                            }
                            if (M_MODEL_NAME.Substring(0, 3) == "I01")
                            {
                                AddParams("MSN" + intX, row["shipping_sn"]?.ToString() ?? "");
                            }
                            else
                            {
                                AddParams("MSN" + intX, row["ssn2"]?.ToString() ?? "");
                            }
                            AddParams("MSNB" + intX, row["shipping_sn"]?.ToString() ?? "");
                            AddParams("CMAC" + intX, row["key_part_sn"]?.ToString() ?? "");
                            AddParams("QTY" + intX, qtyindex.ToString() + "/" + QTY);

                            if (intX == PageQTY)
                            {
                                intX = 0;
                                pageindex = pageindex + 1;
                                AddParams("QTY", QTY);
                                AddParams("CUST_PALLET_NO", strdata);
                                inty = M_MODEL_NAME.IndexOf(".");
                                if (inty == -1)
                                {
                                    labname = M_MODEL_NAME + "_PMSN.Lab";
                                }
                                else
                                {
                                    labname = M_MODEL_NAME.Substring(0, inty - 1) + M_MODEL_NAME.Substring(inty, M_MODEL_NAME.Length - inty) + "_PMSN.Lab";
                                }
                                if (!await P_PrintToCodeSoft("C", labname))
                                    return false;
                            }
                        }
                        if (intX != 0)
                        {
                            intX = 0;
                            pageindex = pageindex + 1;
                            AddParams("QTY", QTY);
                            AddParams("CUST_PALLET_NO", strdata);
                            inty = M_MODEL_NAME.IndexOf(".");
                            if (inty == -1)
                            {
                                labname = M_MODEL_NAME + "_PMSN.Lab";
                            }
                            else
                            {
                                labname = M_MODEL_NAME.Substring(0, inty - 1) + M_MODEL_NAME.Substring(inty, M_MODEL_NAME.Length - inty) + "_PMSN.Lab";
                            }
                            if (!await P_PrintToCodeSoft("C", labname))
                                return false;
                        }
                    }
                }
            }
            else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("103") != -1)
            {
                dtParams.Clear();
                sql = "select distinct A.SHIPPING_SN shipping_sn,A.model_name model_name,A.CONTAINER_NO BATCH,A.SHIP_NO SHIP_NO ,B.KEY_PART_SN key_part_sn from " + S_Table +
                    " A,SFISM4.R108 B where A.SERIAL_NUMBER=B.SERIAL_NUMBER AND B.KEY_PART_NO='MACID' AND  A.imei='" + strdata + "'  AND A.WIP_GROUP='SHIPPING'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    return false;
                }
                else
                {
                    M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";
                    S_SHIP = _result.Data["ship_no"]?.ToString() ?? "";
                    batch = "";
                    haveBatch = "";
                    BATCH_TYPE = "";

                    //xac dinh dn sample hay mass by akira
                    sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE UPPER(PRG_NAME)='LOT_REPRINT' AND VR_NAME='" + M_MODEL_NAME + "'" +
                        "AND VR_DESC IN(SELECT SUBSTR(SO_NUMBER,1,3) FROM SFISM4.R_BPCS_INVOICE_T WHERE TCOM='" + S_SHIP + "')";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        showMessage("Batch no range in C_PARAMETER_INI not exist", "Batch no range in C_PARAMETER_INI not exist", true);
                        txtInput1.SelectAll();
                        return false;
                    }
                    else
                    {
                        BATCH_TYPE = _result.Data["vr_class"]?.ToString() ?? "";
                        SO_PRE = _result.Data["vr_desc"]?.ToString() ?? "";
                        firstrange = _result.Data["vr_item"]?.ToString() ?? "";
                        endrange = _result.Data["vr_value"]?.ToString() ?? "";
                        if (BATCH_TYPE != "MASS" && BATCH_TYPE != "SAMPLE")
                        {
                            showMessage("DN IS NOT MASS OR SAMPLE", "DN IS NOT MASS OR SAMPLE", true);
                            txtInput1.SelectAll();
                            return false;
                        }
                    }
                }
                sql = "'SELECT DISTINCT CONTAINER_NO FROM SFISM4.Z107 WHERE SHIP_NO='" + S_SHIP + "' AND CONTAINER_NO IS NOT NULL AND CONTAINER_NO<>'N/A'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    haveBatch = _result.Data["container_no"]?.ToString() ?? "";
                    batch = haveBatch;
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.Z107  WHERE  MODEL_NAME='" + M_MODEL_NAME + "'  AND CONTAINER_NO IS NOT NULL AND CONTAINER_NO<>'N/A' " +
                        " AND (CONTAINER_NO>='" + firstrange + "' AND CONTAINER_NO<='" + endrange + "')";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LOT_REPRINT' AND VR_CLASS='" + BATCH_TYPE + "' " +
                        " AND VR_NAME='" + M_MODEL_NAME + "'AND VR_DESC='" + SO_PRE + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data == null)
                        {
                            showMessage("Batch no range in C_PARAMETER_INI not exist", "Batch no range in C_PARAMETER_INI not exist", true);
                            txtInput1.SelectAll();
                            return false;
                        }
                        else
                        {
                            batch = _result.Data["vr_item"]?.ToString() ?? "";
                        }
                    }
                    else
                    {
                        sql = "SELECT MAX(CONTAINER_NO)SMAX FROM SFISM4.Z107  WHERE  MODEL_NAME='" + M_MODEL_NAME + "'  AND CONTAINER_NO IS NOT NULL AND CONTAINER_NO<>'N/A' " +
                        " AND (CONTAINER_NO>='" + firstrange + "' AND CONTAINER_NO<='" + endrange + "')";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            S_BATCHMAX = _result.Data["smax"]?.ToString() ?? "";
                            batch = getnextSN(S_BATCHMAX, "0123456789", 1);
                        }
                    }
                }
                if (BATCH_TYPE == "MASS")
                {
                    try
                    {
                        i_batch = Int32.Parse(batch);
                    }
                    catch
                    {
                        showMessage("BATCH FORMAT ERROR", "BATCH FORMAT ERROR", true);
                        return false;
                    }
                }
                if (BATCH_TYPE == "SAMPLE")
                {
                    try
                    {
                        i_batch = Int32.Parse(batch.Substring(1));
                    }
                    catch
                    {
                        showMessage("BATCH FORMAT ERROR", "BATCH FORMAT ERROR", true);
                        return false;
                    }
                }
                sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LOT_REPRINT' AND VR_CLASS='" + BATCH_TYPE + "' " +
                    " AND VR_NAME='" + M_MODEL_NAME + "' AND VR_DESC='" + SO_PRE + "' AND '" + batch + "' BETWEEN VR_ITEM AND VR_VALUE";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("Batch no range error: " + batch, "Batch no range error: " + batch, true);
                    return false;
                }
                sql = "SELECT COUNT(ROWID) QTY FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + strdata + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    QTY = _result.Data["qty"]?.ToString() ?? "";
                }
                dtParams.Clear();
                AddParams("CUST_PALLET_NO", strdata);
                AddParams("QTY", QTY);
                AddParams("SN_QTY", QTY);

                sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=( SELECT MODEL_NAME FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1) AND " +
                    " VERSION_CODE=(SELECT VERSION_CODE FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + strdata + "' AND ROWNUM=1)";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    tocom = _result.Data["upceandata"]?.ToString() ?? "";
                    Modeldesc = _result.Data["cust_model_desc"]?.ToString() ?? "";
                }
                AddParams("BATCH", batch);
                AddParams("PALLET", pallet);
                AddParams("TOCOM", tocom);
                AddParams("MODELDESC", Modeldesc);
                AddParams("PAGE", pageindex.ToString() + "/" + page);

                sql = "SELECT DISTINCT MCARTON_NO FROM SFISM4.Z107 WHERE IMEI='" + strdata + "' ORDER BY MCARTON_NO ASC";
                var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data != null)
                {
                    i = 1;
                    foreach (var row in result_list.Data)
                    {
                        AddParams("MCTN" + i, row["mcarton_no"]?.ToString() ?? "");
                        i++;
                    }
                }
                inty = M_MODEL_NAME.IndexOf(".");
                if (inty == -1)
                {
                    labname = M_MODEL_NAME + "_PMCTN.Lab";
                }
                else
                {
                    labname = M_MODEL_NAME.Substring(0, inty - 1) + M_MODEL_NAME.Substring(inty, M_MODEL_NAME.Length - inty) + "_PMCTN.Lab";
                }
                if (!await P_PrintToCodeSoft("C", labname))
                    return false;
                if (PrintOK && string.IsNullOrEmpty(haveBatch))
                {
                    sql = "UPDATE SFISM4.Z107 SET CONTAINER_NO='" + batch + "' WHERE SHIP_NO='" + S_SHIP + "'";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return false;
                    }
                }
            }
            else
            {
                sql = "select distinct model_name,carton_no,mcarton_no,IMEI from sfism4.r_wip_tracking_t " +
                    " where imei =(select imei from sfism4.r_wip_tracking_t where (imei='" + strdata + "') or (mcarton_no='" + strdata + "') group by imei) " +
                    " group by model_name,carton_no,mcarton_no,IMEI";
                var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data != null)
                {
                    i = 1;
                    foreach (var row in result_list.Data)
                    {
                        if (i == 1)
                        {
                            dtParams.Clear();
                            M_MODEL_NAME = row["model_name"]?.ToString() ?? "";
                        }
                        AddParams("MCTN" + i, row["mcarton_no"]?.ToString() ?? "");
                        AddParams("CTN" + i, row["carton_no"]?.ToString() ?? "");
                        AddParams("CUST_PALLET_NO" + i, row["imei"]?.ToString() ?? "");
                        i++;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<bool> Shipping_PDF417(string ctrdata)
        {
            string batch, pallet, Modeldesc = string.Empty, tocom, QTY = string.Empty, page, ctn, SNQTY = string.Empty, strdata, labname;
            int intX, inty, qtyindex, pageindex, CtnK;
            intX = 0;
            qtyindex = 0;
            pageindex = 1;

            sql = "select model_name,imei,mcarton_no,shipping_sn from sfism4.r107 where (serial_number ='" + ctrdata + "' or shipping_sn ='" + ctrdata + "' or mcarton_no='" + ctrdata + "' or carton_no='" + ctrdata + "' or imei='" + ctrdata + "') order by  mcarton_no,shipping_sn";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                strdata = _result.Data["imei"]?.ToString() ?? "";
                pallet = strdata;
            }
            else
            {
                return false;
            }
            sql = "select model_name,imei,mcarton_no,shipping_sn from sfism4.r107 where (serial_number ='" + strdata + "' " +
                " or shipping_sn ='" + strdata + "' or mcarton_no='" + strdata + "' or carton_no='" + strdata + "' or imei='" + strdata + "') order by  mcarton_no,shipping_sn ";
            var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data != null)
            {
                int i = 1;
                ctn = "";
                CtnK = 0;
                foreach (var row in result_list.Data)
                {
                    if (i == 1)
                    {
                        M_MODEL_NAME = row["model_name"]?.ToString() ?? "";
                        dtParams.Clear();

                        //for print SN in pallet model_type 999
                        sql = "SELECT COUNT(SERIAL_NUMBER) SNQTY FROM sfism4.r107  WHERE IMEI='" + strdata + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            SNQTY = _result.Data["snqty"]?.ToString() ?? "";
                        }
                        //END for print SN in pallet model_type 999
                        sql = "SELECT COUNT(distinct mcarton_no) QTY FROM sfism4.r107  WHERE IMEI='" + strdata + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            QTY = _result.Data["qty"]?.ToString() ?? "";
                        }

                        sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME=(SELECT MODEL_NAME FROM sfism4.r107 WHERE IMEI='" + strdata + "' AND ROWNUM=1) AND VERSION_CODE=(SELECT VERSION_CODE FROM sfism4.r107 WHERE IMEI='" + strdata + "' AND ROWNUM=1)";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            Modeldesc = _result.Data["cust_model_desc"]?.ToString() ?? "";
                        }
                        AddParams("SN_QTY", SNQTY);
                        AddParams("CUST_PALLET_NO", strdata);
                        AddParams("QTY", QTY);
                        AddParams("ModelDesc", Modeldesc);
                    }
                    AddParams("MSN" + i, row["shipping_sn"]?.ToString() ?? "");
                    if (ctn != row["mcarton_no"].ToString())
                    {
                        CtnK = CtnK + 1;
                        ctn = row["mcarton_no"]?.ToString() ?? "";
                        AddParams("MCTN" + CtnK, ctn);
                    }
                    i++;
                }
            }
            else
            {
                return false;
            }
            if (!string.IsNullOrEmpty(M_MODEL_NAME) && M_MODEL_NAME.IndexOf(".") == -1)
            {
                labname = M_MODEL_NAME + "_PMSN.lab";
            }
            else
            {
                labname = M_MODEL_NAME.Substring(0, M_MODEL_NAME.IndexOf(".") - 1) + M_MODEL_NAME.Substring(M_MODEL_NAME.IndexOf("."), M_MODEL_NAME.Length - M_MODEL_NAME.IndexOf(".")) + "_PMSN.Lab";
            }
            if (!await P_PrintToCodeSoft("C", labname))
                return false;
            return true;
        }
        private async Task<bool> CheckExistZ107(string sData)
        {
            if (!string.IsNullOrEmpty(txtInput1.Text) && txtInput1.Text.IndexOf("</EAN>") != -1)
            {
                sData = txtInput1.Text.Substring(txtInput1.Text.IndexOf("<SRNO>") + 5, txtInput1.Text.IndexOf("</SRNO>") - txtInput1.Text.IndexOf("</SRNO>") - 6);
            }
            else
            {
                if (!string.IsNullOrEmpty(txtInput1.Text) && txtInput1.Text.IndexOf(",") != -1)
                {
                    sData = txtInput1.Text.Substring(txtInput1.Text.IndexOf(","), txtInput1.Text.IndexOf(",") - txtInput1.Text.IndexOf(",") - 1);
                }
            }
            sql = "select * from sfism4.z107 where serial_number in (select serial_number from  SFISM4.r107 " +
                " where (serial_number = '" + sData + "'  shipping_sn = '" + sData + "' or  carton_no = '" + sData + "' or  pallet_no = '" + sData + "' or  Mcarton_no = '" + sData + "' or  IMEI = '" + sData + "') and  rownum = 1 " +
                " union all " +
                " select serial_number from  SFISM4.Z107 where (serial_number = '" + sData + "' or  shipping_sn = '" + sData + "' or  carton_no = '" + sData + "' or  pallet_no = '" + sData + "' or  Mcarton_no = '" + sData + "' or  IMEI = '" + sData + "') and  rownum = 1 ) ";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                if (Z107.IsChecked == false)
                {
                    showMessage("Please chose Z107", "Vui lòng chọn Z107", true);
                    return false;
                }
                else
                {
                    sql = "select * from  SFISM4.Z107 where (IMEI = '" + sData + "' or mcarton_no = '" + sData + "' ) and  rownum = 1";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        if (Z107.IsChecked == true && Kind.SelectedItem == chkPalletLabel)
                        {
                            showMessage("Please scan IMEI", "Vui lòng quyét mã IMEI", true);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private async Task<bool> Shipping_Pallet(string imei)
        {
            string s_ship_no, ShippingQty, IMEIQTY, CartonQty, s_model_name, s_version, PERIMEIQTY;
            sql = "SELECT * FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI='" + imei + "' AND ROWNUM=1";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                showMessage("00249", "00249", false);
                return false;
            }
            s_ship_no = _result.Data["ship_no"]?.ToString() ?? "";
            if (string.IsNullOrEmpty(s_ship_no))
            {
                showMessage("00364", "00364", false);
                return false;
            }
            s_model_name = _result.Data["model_name"]?.ToString() ?? "";
            s_version = _result.Data["version_code"]?.ToString() ?? "";

            sql = "SELECT SERIAL_NUMBER FROM SFISM4.Z_WIP_TRACKING_T WHERE PMCC IS NULL AND SHIP_NO='" + s_ship_no + "' AND ROWNUM=1";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                //ShippingQty:=inttostr(qurytemp.recordcount);
                ShippingQty = "1";
                sql = "SELECT DISTINCT IMEI FROM SFISM4.Z_WIP_TRACKING_T WHERE SHIP_NO='" + s_ship_no + "'";
                var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data != null)
                {
                    IMEIQTY = result_list.Data.Count().ToString();
                    int i = 1;
                    foreach (var row in result_list.Data)
                    {
                        sql = "UPDATE SFISM4.Z_WIP_TRACKING_T SET PMCC='" + i + "' WHERE IMEI='" + row["imei"].ToString() + "'";
                        var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return false;
                        }
                        i++;
                    }
                }
            }

            sql = "SELECT COUNT(ROWID) C_COUNT FROM SFISM4.Z_WIP_TRACKING_T WHERE SHIP_NO='" + s_ship_no + "'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                CartonQty = _result.Data["c_count"]?.ToString() ?? "";
            }

            sql = "SELECT * FROM SFIS1.C_PACK_PARAM_T WHERE MODEL_NAME='" + s_model_name + "' AND VERSION_CODE='" + s_version + "'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                PERIMEIQTY = (Int32.Parse(_result.Data["tray_qty"].ToString()) * Int32.Parse(_result.Data["carton_qty"].ToString()) * Int32.Parse(_result.Data["pallet_qty"].ToString())).ToString();
            }

            return true;
        }
        private async Task iDTAprint()
        {
            string SN, MFG_DATE, MAC1, model_nmae, model_serial, slabname;
            sql = "select SN,SSN1,MAC1 From sfism4.R_SIEMENS_DATA_T where SN = '" + txtInput1.Text + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                SN = _result.Data["sn"]?.ToString() ?? "";
                MFG_DATE = _result.Data["ssn1"]?.ToString() ?? "";
                MAC1 = _result.Data["mac1"]?.ToString() ?? "";
                AddParams("SN", SN);
                AddParams("MFG_DATE", MFG_DATE);
                AddParams("MAC1", MAC1);
            }
            else
            {
                showMessage("00345", "00345", false);
                txtInput1.Clear();
                txtInput2.Clear();
                txtInput1.Focus();
                return;
            }
        }
        private async Task SM_model_printAsync()
        {
            string SN, SSID, DV, MV, WPA, model_serial, MAC, slabname;
            sql = "select SN,SSN1,SSN2,MAC1 From sfism4.R_SIEMENS_DATA_T where SN='" + txtInput1.Text + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                SN = _result.Data["sn"]?.ToString() ?? "";
                SSID = _result.Data["ssn2"]?.ToString() ?? "";
                WPA = _result.Data["ssn1"]?.ToString() ?? "";
                MAC = _result.Data["mac1"]?.ToString() ?? "";
                DV = txtInput1.Text;
                MV = DV.Substring(3, 1);
                switch (MV)
                {
                    case "1":
                        MV = "01";
                        break;
                    case "2":
                        MV = "02";
                        break;
                    case "3":
                        MV = "03";
                        break;
                    case "4":
                        MV = "04";
                        break;
                    case "5":
                        MV = "05";
                        break;
                    case "6":
                        MV = "06";
                        break;
                    case "7":
                        MV = "07";
                        break;
                    case "8":
                        MV = "08";
                        break;
                    case "9":
                        MV = "09";
                        break;
                    case "O":
                        MV = "0O";
                        break;
                    case "N":
                        MV = "0N";
                        break;
                    default:
                        MV = "0D";
                        break;
                }
                AddParams("SN", SN);
                AddParams("SSID", SSID);
                AddParams("WPA", WPA);
                AddParams("MV", MV);
                AddParams("MAC", MAC);

                sql = "SELECT MODEL_NAME,MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T where model_name" +
                    "IN(SELECT  MODEL_NAME FROM SFISM4.R107 WHERE SERIAL_NUMBER='" + txtInput1.Text + "' )";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    model_name = _result.Data["model_name"]?.ToString() ?? "";
                    model_serial = _result.Data["model_serial"]?.ToString() ?? "";
                    if (txtSeditfCount.Text == "1")
                        if (!await P_PrintToCodeSoft("C", model_name + "_BX.Lab"))
                            return;
                    if (txtSeditfCount.Text != "1")
                    {
                        if (txtSedprtnow.Text != "0")
                        {
                            slabname = model_name + "_BX" + txtSedprtnow + ".Lab";
                            if (!await P_PrintToCodeSoft("C", slabname))
                            {
                                return;
                            }
                        }
                    }
                    txtInput1.Text = "";
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    txtInput1.Focus();
                    return;
                }
                else
                {
                    showMessage("00345", "00345", false);
                    txtInput1.Text = "";
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    txtInput1.Focus();
                    return;
                }
            }
        }

        private void labelQTY_Click(object sender, RoutedEventArgs e)
        {
            frmLabelQTY labelQTY = new frmLabelQTY();
            labelQTY.ShowDialog();
        }

        private void Visible_Click(object sender, RoutedEventArgs e)
        {
            string labelfile = My_LabelFileName;
            frmLogin frmLogin = new frmLogin();
            frmLogin.sfcClient = sfcClient;
            frmLogin.type = "Visible";
            frmLogin.ShowDialog();
            if (frmLogin.ok == System.Windows.Forms.DialogResult.OK)
            {
                if (File.Exists(labelfile))
                {
                    if (labApp == null)
                    {
                        labApp = new LabelManager2.Application();
                    }
                    try
                    {
                        labApp.Documents.Open(labelfile, false);
                        Document doc = labApp.ActiveDocument;
                        doc.Application.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        showMessage(ex.Message, ex.Message, true);
                        return;
                    }
                }
                else
                {
                    showMessage("Can not find Label file!", "Không tìm thấy tệp label!", true);
                    return;
                }
            }
        }

        private void LH_Click(object sender, RoutedEventArgs e)
        {
            LH.IsChecked = true;
            GL.IsChecked = false;
            Local.IsChecked = false;
        }

        private void GL_Click(object sender, RoutedEventArgs e)
        {
            LH.IsChecked = false;
            GL.IsChecked = true;
            Local.IsChecked = false;
        }

        private void Local_Click(object sender, RoutedEventArgs e)
        {
            string labelfile = My_LabelFileName;
            frmLogin frmLogin = new frmLogin();
            frmLogin.sfcClient = sfcClient;
            frmLogin.type = "Visible";
            frmLogin.ShowDialog();
            if (frmLogin.ok == System.Windows.Forms.DialogResult.OK)
            {
                LH.IsChecked = false;
                GL.IsChecked = false;
                Local.IsChecked = true;
            }
        }
        void cmbItem_PreviewMouseDown(object s, MouseButtonEventArgs e)
        {
            //...do your item selection code here...
            string myItem = (e.OriginalSource as FrameworkElement).DataContext.ToString() ;
            if (myItem == "Sequans SN")
            {
                mText.Content = "Panel :";
                txtInput2.Visibility = Visibility.Hidden;
                txtInput3.Visibility = Visibility.Hidden;
                InputGroup.Visibility = Visibility.Hidden;
                InputGroup.Visibility = Visibility.Hidden;
            }
            else if (myItem == "Carton Label")
            {
                mText.Content = "CartonID : ";
                txtInput2.Visibility = Visibility.Hidden;
                txtInput3.Visibility = Visibility.Hidden;
                InputGroup.Visibility = Visibility.Hidden;
                InputGroup.Visibility = Visibility.Hidden;
            }
            else if (myItem == "Combine Label")
            {
                mText.Content = "SN : ";
                txtInput2.Visibility = Visibility.Hidden;
                txtInput3.Visibility = Visibility.Hidden;
                InputGroup.Visibility = Visibility.Hidden;
                InputGroup.Visibility = Visibility.Hidden;
            }
            else
            {
                mText.Content = "SN/SSN:";
                txtInput2.Visibility = Visibility.Visible;
                txtInput3.Visibility = Visibility.Visible;
                InputGroup.Visibility = Visibility.Visible;
                InputGroup.Visibility = Visibility.Visible;
            }
        }
        private async void txtInput1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtSeditLabelQty.Text = "1";
                var aaaa = txtInput1.Text;
                 if(!NicMode.IsChecked)
                    btnOK_Click(sender, e);
                else
                {
                    #region Print Sequans SN from Panel
                    if (Kind.SelectedItem == chkSequansSN)
                    {
                        string inputdata = "SUB_FUNC:PRINT|PANEL:" + txtInput1.Text.Trim();
                        if (rePrint.IsChecked) inputdata = "SUB_FUNC:REPRINT|SN:" + txtInput1.Text.Trim() + "|EMP:" + frmLogin.emp_no + "|IP:" + GetIPAddress();
                        await call_SP(inputdata, "SEQUANS_SN");
                        string _RES = _ads[1]["res"];
                        string _Param = _ads[0]["out_data"];

                        if (_RES.StartsWith("OK"))
                        {
                            if (_Param is null || _Param.ToString() == "")
                            {
                                showMessage("No param to label file,Call IT check");
                                txtInput1.SelectAll();
                                txtInput1.Focus();
                                return;
                            }
                            G_sLabl_Name = "";
                            dtParams.Clear();
                            foreach (var rows in _Param.Split('|'))
                            {
                                if (rows.IndexOf(':') != -1) 
                                {
                                    AddParam(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "", true);
                                }
                            }
                            if (G_sLabl_Name == "")
                            {
                                showMessage("Label name is null,Call IT check");
                                txtInput1.SelectAll();
                                txtInput1.Focus();
                                return;
                            }
                            if (_RES != "OK,Next Panel")
                            {
                                PrintOK = false;
                                await P_PrintToCodeSoft("C", G_sLabl_Name);
                                if (PrintOK) txtInput1_KeyDown(sender, e);
                            }
                            else
                            {
                                await P_PrintToCodeSoft("C", G_sLabl_Name);
                                txtInput1.SelectAll();
                                txtInput1.Focus();
                                return;
                            }
                        }
                        else
                        {
                            showMessage(_RES);
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                            return;
                        }
                        return;
                    }
                    #endregion Print Sequans SN from Panel
                    //------------------------------------------------------------------------------
                    #region Print Carton label 
                    if (Kind.SelectedItem == chkCartonLabel|| Kind.SelectedItem == chkCombine)
                    {
                        await btnOK_Click();
                        return;
                    }
                    #endregion Print Carton label
                    //------------------------------------------------------------------------------
                    #region Print BX label 
                    //Dev for inputQR,inputSN,inputSSN(SuperCap),input 2MAC
                    if (ScanQR.IsChecked)
                    {
                        var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.SP_GET_QRSTRING",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="IN_GROUP",Value="LotReprint",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=txtInput1.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                        });
                        dynamic _ads = result.Data;
                        string _RES = _ads[1]["res"];
                        if (_RES.StartsWith("OK"))
                        {
                            string strOut = _ads[0]["out_data"];
                            txtInput1.Text = strOut.Split('|')[0].ToString();
                            txtInput2.Text = strOut.Split('|')[1].ToString();
                            txtInput2_KeyDown(sender, e);
                            return;
                        }
                        else
                        {
                            showMessage(_RES);
                            return;
                        }
                    }
                    //------------------------------------------------------------------
                    if (InputSN.IsChecked||Input2MAC.IsChecked)
                    {
                        if (Input2MAC.IsChecked)
                        {
                            //2MAC:25,3MAC:38,4MAC:51
                            if (txtInput1.Text.Length == 25 || txtInput1.Text.Length == 38 || txtInput1.Text.Length == 51)
                                txtInput1.Text = txtInput1.Text.Substring(0, 12).Trim();
                            else
                            {
                                showMessage("Label length error,Just apply for 2-3-4 MAC|Lỗi độ dài label,Chỉ áp dụng cho label 2 hoặc 3 hoặc 4 MAC");
                                return;
                            }
                        }
                        await call_SP("SN:" + txtInput1.Text, "INPUT1");
                        string _RES = _ads[1]["res"];
                        if (_RES.StartsWith("OK")) 
                        {
                            if (_RES.StartsWith("OK,PRINT"))
                            {
                                txtInput2_KeyDown(sender, e);
                            }
                            else
                            {
                                txtInput2.SelectAll();
                                txtInput2.Focus();
                            }
                        } 
                        else
                        {
                            showMessage(_RES);
                            return;
                        }
                    }
                    //------------------------------------------------------------------
                    if (InputSSN.IsChecked)
                    {
                        await call_SP("SSN:" + txtInput1.Text, "INPUT1");
                        string _RES = _ads[1]["res"];
                        if (_RES.StartsWith("OK"))
                        {
                            string strOut = _ads[0]["out_data"];
                            txtInput1.Text = strOut.Split('|')[0].ToString();
                            txtInput2.Text = strOut.Split('|')[1].ToString();
                            txtInput2_KeyDown(sender, e);
                        }
                        else
                        {
                            showMessage(_RES);
                            return;
                        }
                    }
                    //------------------------------------------------------------------
                    if(InputASN.IsChecked)
                    {
                        txtInput1.Text = "@"+ txtInput1.Text.Trim();
                        txtInput2.Text = "N/A";
                        txtInput2_KeyDown(sender, e);
                        return;
                    }

                    if(InputBoxID.IsChecked)
                    {
                        string line = "N/A";
                        if (!string.IsNullOrEmpty(txtStation.Text))
                        {
                            line = txtLine.Text.Trim();
                        }
                        string labelType = "LabelType:LT" + txtSeditLabelQty.Text + txtSeditfCount.Text + txtSedprtnow.Text;
                        string indata = labelType + "|BOXID:" + txtInput1.Text + "|JUSTPRINT:" + txtSedprtnow.Text + "|LINE:" + line;
                        await call_SP(indata, "INPUTBOX");
                        string _RES = _ads[1]["res"];
                        string _Param = _ads[0]["out_data"];

                        if (_RES.StartsWith("OK"))
                        {
                            if (_Param is null || _Param.ToString() == "")
                            {
                                showMessage("No param to label file,Call IT check");
                                txtInput1.SelectAll();
                                txtInput1.Focus();
                                return;
                            }
                            dtParams.Clear();
                            foreach (var rows in _Param.Split('|'))
                            {
                                if (rows.IndexOf(':') != -1)
                                {
                                    AddParam(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "", true);
                                }
                            }
                        }
                        else
                        {
                            showMessage(_RES);
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                            return;
                        }

                        string ztemp = txtSeditLabelQty.Text.Trim() + txtSeditfCount.Text.Trim() + txtSedprtnow.Text.Trim();
                        //-----------------------------------------------------------
                        sql = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='" + txtInput1.Text + "'";
                        sql += "AND INSTR(BILL_NO,'"+ ztemp + "') > 0";
                        var _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result1.Data != null && !PrintTest.IsChecked)
                        {
                            frmLogin frmLogin = new frmLogin();
                            frmLogin.sfcClient = sfcClient;
                            frmLogin.type = "MSG";
                            if (frmLogin.emp_input != txtInput1.Text)
                            {
                                frmLogin.ShowDialog();
                                await saveReprintRd(frmLogin.emp_input, txtInput1.Text,"BOX"+ztemp);
                                if (!saveOK)
                                {
                                    return;
                                }
                            }
                        }
                        //-------------------------------------------------------------------------------

                        G_sLabl_Name = txtModelName.Text.Replace(".", "_") + "_BX.LAB";
                        if (txtSedprtnow.Text == "0" && txtSeditfCount.Text == "1")
                        {
                            sFindData = paramData;
                            await P_PrintToCodeSoft("C", G_sLabl_Name);

                        }
                        if (txtSeditfCount.Text != "1")
                        {
                            if (txtSedprtnow.Text == "0")
                            {
                                for (int m = 1; m <= Int32.Parse(txtSeditfCount.Text) - 1; m++)
                                {
                                    labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + m + ".Lab";
                                    await P_PrintToCodeSoft("C", labname);
                                }
                            }
                            else
                            {
                                labname = G_sLabl_Name.Substring(0, G_sLabl_Name.Length - 4) + txtSedprtnow.Text + ".Lab";
                                await P_PrintToCodeSoft("C", labname);
                            }
                        }
                        if (PrintOK)
                        {
                            sql = "UPDATE SFISM4.R_WIP_TRACKING_T SET bill_no=bill_no||'LT" + ztemp + "' WHERE tray_no='" + txtInput1.Text + "' and instr(bill_no,'"+ztemp+"')=0";
                            var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = sql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (query_update.Result.ToString() != "OK")
                            {
                                showMessage("Call IT check exception: " + query_update.Message.ToString() + "\nsql: " + sql);
                                return;
                            }
                        }
                    }
                    #endregion Print BX label 
                }
            }
        }

        private void setupStation_Click(object sender, RoutedEventArgs e)
        {
            frmSetupStation frmSetup = new frmSetupStation(this);
            frmSetup.sfcClient = sfcClient;
            if (autoMation.IsChecked)
            {
                frmSetup.AutomationChecked = true;
            }
            else
            {
                frmSetup.AutomationChecked = false;
            }
            frmSetup.ShowDialog();
            if (frmSetup.ok == System.Windows.Forms.DialogResult.OK)
            {
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                M_Line = frmSetup.M_Line;
                ini.IniWriteValue("LOTREPRINT", "M_Line", M_Line);
                txtLine.Text = M_Line;
                lbl3.Text = " - ";
                M_Station = frmSetup.M_Station;
                ini.IniWriteValue("LOTREPRINT", "M_Station", M_Station);
                M_Group = frmSetup.M_Group;
                ini.IniWriteValue("LOTREPRINT", "M_Group", M_Group);
                txtStation.Text = M_Group;
                txtLine.Visibility = Visibility.Visible;
                lbl3.Visibility = Visibility.Visible;
                txtStation.Visibility = Visibility.Visible;
                M_Group = frmSetup.M_Group;
                My_Section = frmSetup.My_Section;
                ini.IniWriteValue("LOTREPRINT", "My_Section", My_Section);
            }
            if (frmSetup.ok == System.Windows.Forms.DialogResult.None)
            {
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                M_Line = "";
                ini.IniWriteValue("LOTREPRINT", "M_Line", M_Line);
                txtLine.Text = M_Line;
                lbl3.Text = "";
                M_Group = "";
                ini.IniWriteValue("LOTREPRINT", "M_Group", M_Group);
                txtStation.Text = M_Group;
                txtLine.Visibility = Visibility.Hidden;
                lbl3.Visibility = Visibility.Hidden;
                txtStation.Visibility = Visibility.Hidden;
                My_Section = "";
                ini.IniWriteValue("LOTREPRINT", "My_Section", My_Section);
            }
        }

        private async void txtInput2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtInput1.Text = txtInput1.Text.Trim();
                txtInput2.Text = txtInput2.Text.Trim();
                if (txtInput1.Text == "")
                {
                    txtInput1.Focus();
                    return;
                }
                if(!NicMode.IsChecked) btnOK_Click(sender, e);
                else
                {
                    string indata = "SN:" + txtInput1.Text + "|SSN:" + txtInput2.Text;
                    await call_SP(indata, "INPUT2");
                    string _RES = _ads[1]["res"];
                    string _OUT = _ads[0]["out_data"];
                    if (_RES.StartsWith("OK"))
                    {
                        txtModelName.Text = _OUT;
                        await btnOK_Click();
                    }
                    else
                    {
                        showMessage(_RES);
                        if(InputSSN.IsChecked||ScanQR.IsChecked)
                        {
                            txtInput1.SelectAll();
                            txtInput1.Focus();
                        }
                        if(InputSN.IsChecked)
                        {
                            txtInput2.SelectAll();
                            txtInput2.Focus();
                        }
                        return;
                    }
                }
            }
        }

        private void txtInput3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtInput1.Text == "")
                    txtInput1.Focus();
                btnOK_Click(sender, e);
            }
        }

        public async Task<string> findMOType()
        {
            string tmpssn1, tmpssn2, tmpssn3, tmpssn5, tmpmac1, tmpmac2;
            string result = "";
            int Cust_sn_len, i;
            string Cust_sn_Prefix, Cust_sn_prefix2, Cust_sn_postfix, MACID_PREFIX, CUSTSN_STRING, LAST_SN, SYEAR4, ssn_prefix, last_sn_flag;
            string PRINTDATE, MAC1, WEP1, SN1, prt_mac = "", H235, CHECKSUM, TEMP, PING = "", SSN, P_SSN, SN, SSID2, WEP2, PINCODE, CSN, S_SN;
            string SSN1, SSN2, SSN3, SSN4, SSN5, SSN9, SSN13, SSN15, SSN14, SSN12, SSN16, SSN7,SSN6,SSN8,MAC4,MAC5;
            string PRINT_SSN1, PRINT_SSN2, PRINT_SSN3, PRINT_SSN4, PRINT_SSN5, PRINT_MAC1, PRINT_MAC2, PRINT_MAC3, PRINT_MAC4, PRINT_MAC5;
            string PRINTMODELFLAG, MoType, BlnCheckSSN, PoNo_String, str_group_name = "", str_model_name, FINISH_GOOD2, sCustModel_Desc1;
            string MAC2, MAC3, MAC10, checkssnrule1, checkssnrule2, Customer_sn, wo, CUSTO, key_part, sroute, keypart;
            string str_serial_number = "", C_MAXSSN1, prefix, TRUCK_NUMBER, length1;
            string sLine, sYear, supc, smodel, s071_PONO, s071_MONUmber, tmp_str1, IMEI, MCN, MEID, C_MAC, SQLSTR, SNEXT, item_1;
            string sDay, sWeek, sShift, ssDay, sSQE, sVer, sVer1, sfis_model, T_ssn, temp_ssn, Tmac, C_BOM, C_KEYPART;
            string sSN_Barcode, sSN_Text, sCustModel_Name, sCustModel_Desc2, sCustModel_Desc3, sCustModel_Desc4, sCustModel_Desc5, sCustomer_Name;
            string s_079Mac, s_079monum, s_079wepkey, S_035IMEI, S_035MEID, S_035VER, S_035PRODUCT, S_035SCAN, vender_part = "";
            dtParams.Clear();
            printflag = true;
            checkssnrule1 = "SSN1";
            checkssnrule2 = "SSN1";
            if (plModel.IsChecked)
            {
                pl_model_print();
                return null;
            }

            if (txtMoNumber.Text.IndexOf("21") != -1)
            {
                sql = "select model_type,model_name from sfis1.c_model_desc_t WHERE MODEL_NAME =(select model_name from sfism4.R105  where MO_NUMBER ='" + txtMoNumber.Text + "')";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("80430", "80430", false);
                    txtInput1.SelectAll();
                    txtInput1.Focus();
                    return null;
                }
            }
            //model_type = "201,301";
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("102") != -1)
            {
                sql = "SELECT * FROM SFISM4.R_INVENTEL_PRINT_T WHERE  MAC='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    P_SSN = _result.Data["ssn"]?.ToString() ?? "";
                    SSN1 = _result.Data["ssn"]?.ToString() ?? "";
                    AddParams("P_SSN", P_SSN);
                    AddParams("SSN1", SSN1);
                    AddParams("BOXSN", SSN1);
                }
            }

            sql = "SELECT TO_CHAR(sysdate, 'YYYYMMDD') date_time FROM dual ";
             var _result_date = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result_date.Data!=null)
            {
               string  DATE_TIME = _result_date.Data["date_time"]?.ToString() ?? "";
                AddParams("DATE_TIME", DATE_TIME);
            }

            if (check_roku_info) //(loginDB=="TEST")
            {
                sql= "SELECT * FROM SFISM4.R_PRINT_INPUT_T WHERE  SSN1='"+txtInput1.Text+"'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                     PRINT_SSN1 = _result.Data["ssn1"]?.ToString() ?? "";
                     PRINT_SSN2 = _result.Data["ssn2"]?.ToString() ?? "";
                     PRINT_SSN3 = _result.Data["ssn3"]?.ToString() ?? "";
                     PRINT_SSN4 = _result.Data["ssn4"]?.ToString() ?? "";
                     PRINT_SSN5 = _result.Data["ssn5"]?.ToString() ?? "";
                     PRINT_MAC1 = _result.Data["mac1"]?.ToString() ?? "";
                     PRINT_MAC2 = _result.Data["mac2"]?.ToString() ?? "";
                     PRINT_MAC3 = _result.Data["mac3"]?.ToString() ?? "";
                     PRINT_MAC4 = _result.Data["mac4"]?.ToString() ?? "";
                     PRINT_MAC5 = _result.Data["mac5"]?.ToString() ?? "";

                    AddParams("P_SSN1", PRINT_SSN1);
                    AddParams("P_SSN2", PRINT_SSN2);
                    AddParams("P_SSN3", PRINT_SSN3);
                    AddParams("P_SSN4", PRINT_SSN4);
                    AddParams("P_SSN5", PRINT_SSN5);
                    AddParams("P_MAC1", PRINT_MAC1);
                    AddParams("P_MAC2", PRINT_MAC2);
                    AddParams("P_MAC3", PRINT_MAC3);
                    AddParams("P_MAC4", PRINT_MAC4);
                    AddParams("P_MAC5", PRINT_MAC5);
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.R_PRINT_INPUT_T WHERE  MAC1='" + txtInput1.Text + "'";
                     _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                         PRINT_SSN1 = _result.Data["ssn1"]?.ToString() ?? "";
                         PRINT_SSN2 = _result.Data["ssn2"]?.ToString() ?? "";
                         PRINT_SSN3 = _result.Data["ssn3"]?.ToString() ?? "";
                         PRINT_SSN4 = _result.Data["ssn4"]?.ToString() ?? "";
                         PRINT_SSN5 = _result.Data["ssn5"]?.ToString() ?? "";
                         PRINT_MAC1 = _result.Data["mac1"]?.ToString() ?? "";
                         PRINT_MAC2 = _result.Data["mac2"]?.ToString() ?? "";
                         PRINT_MAC3 = _result.Data["mac3"]?.ToString() ?? "";
                         PRINT_MAC4 = _result.Data["mac4"]?.ToString() ?? "";
                         PRINT_MAC5 = _result.Data["mac5"]?.ToString() ?? "";

                        AddParams("P_SSN1", PRINT_SSN1);
                        AddParams("P_SSN2", PRINT_SSN2);
                        AddParams("P_SSN3", PRINT_SSN3);
                        AddParams("P_SSN4", PRINT_SSN4);
                        AddParams("P_SSN5", PRINT_SSN5);
                        AddParams("P_MAC1", PRINT_MAC1);
                        AddParams("P_MAC2", PRINT_MAC2);
                        AddParams("P_MAC3", PRINT_MAC3);
                        AddParams("P_MAC4", PRINT_MAC4);
                        AddParams("P_MAC5", PRINT_MAC5);
                    }
                    else
                    {
                        sql = "SELECT * FROM SFISM4.R_PRINT_INPUT_T WHERE  SERIAL_NUMBER='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            PRINT_SSN1 = _result.Data["ssn1"]?.ToString() ?? "";
                            PRINT_SSN2 = _result.Data["ssn2"]?.ToString() ?? "";
                            PRINT_SSN3 = _result.Data["ssn3"]?.ToString() ?? "";
                            PRINT_SSN4 = _result.Data["ssn4"]?.ToString() ?? "";
                            PRINT_SSN5 = _result.Data["ssn5"]?.ToString() ?? "";
                            PRINT_MAC1 = _result.Data["mac1"]?.ToString() ?? "";
                            PRINT_MAC2 = _result.Data["mac2"]?.ToString() ?? "";
                            PRINT_MAC3 = _result.Data["mac3"]?.ToString() ?? "";
                            PRINT_MAC4 = _result.Data["mac4"]?.ToString() ?? "";
                            PRINT_MAC5 = _result.Data["mac5"]?.ToString() ?? "";

                            AddParams("P_SSN1", PRINT_SSN1);
                            AddParams("P_SSN2", PRINT_SSN2);
                            AddParams("P_SSN3", PRINT_SSN3);
                            AddParams("P_SSN4", PRINT_SSN4);
                            AddParams("P_SSN5", PRINT_SSN5);
                            AddParams("P_MAC1", PRINT_MAC1);
                            AddParams("P_MAC2", PRINT_MAC2);
                            AddParams("P_MAC3", PRINT_MAC3);
                            AddParams("P_MAC4", PRINT_MAC4);
                            AddParams("P_MAC5", PRINT_MAC5);
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(model_type) && model_type.Contains("102"))
            {
                sql = "SELECT * FROM SFISM4.R_INVENTEL_PRINT_T WHERE  MAC='" + txtInput1.Text + "' ";
                 _result_date = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result_date.Data != null)
                {
                    P_SSN = _result_date.Data["ssn"]?.ToString() ?? "";
                    SSN1= _result_date.Data["ssn"]?.ToString() ?? "";
                    AddParams("P_SSN", P_SSN);
                    AddParams("SSN1", SSN1);
                    AddParams("BOXSN", SSN1);
                }
            }

            sql = "SELECT * FROM SFISM4.r108 WHERE serial_number='" + txtInput1.Text + "'";
            var result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data != null)
            {
                foreach (var row in result_list.Data)
                {
                    AddParams("B" + (row["key_part_no"]?.ToString() ?? ""), (row["key_part_sn"]?.ToString() ?? ""));
                }
            }
            if (printCtn.IsChecked) await prepareCtnParam(txtInput1.Text.Trim());

            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("035") != -1 && rework.IsChecked)
            {
                sql = "SELECT * FROM SFISM4.R_INVENTEL_DATA_T WHERE  MAC='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    S_035IMEI = _result.Data["ssn"]?.ToString() ?? "";
                    S_035MEID = _result.Data["mac"]?.ToString() ?? "";
                    S_035PRODUCT = _result.Data["h235"]?.ToString() ?? "";
                    S_035PRODUCT = S_035PRODUCT.Substring(S_035PRODUCT.Length - 7, 7);
                    S_035SCAN = txtInput2.Text.Substring(txtInput2.Text.Length - 7, 7);
                    if (S_035PRODUCT != S_035SCAN)
                    {
                        showMessage("CT NOT MATCH!", "CT không khớp!", true);
                        return null;
                    }
                    sql = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE  SERIAL_NUMBER='" + txtInput1.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        S_035VER = _result.Data["version_code"]?.ToString() ?? "";
                        AddParams("IMEI", S_035IMEI);
                        AddParams("MEID", S_035MEID);
                        AddParams("REV", S_035VER);
                        AddParams("PRODUCT", txtInput2.Text);
                    }
                }
                else
                {
                    showMessage("MEID NOT FOUND IN INVENTEL DATA!", "MEID NOT FOUND IN INVENTEL DATA!", true);
                    return null;
                }

            }
            if (!BPinCode.IsChecked && txtMoNumber.Text.Length == 0)
            {
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("093") == -1)
                {
                    sql = "select * from SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "' OR SSN1='" + txtInput1.Text + "' or mac1='" + txtInput1.Text + "'";
                    var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        //string sql_check = "select * from sfis1.c_parameter_ini where prg_name='HH_PrintLabel' and vr_class='ROKU' and vr_value='Y' ";
                        // _result_check_param = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        //{
                        //    CommandText = sql_check,
                        //    SfcCommandType = SfcCommandType.Text
                        //});

                        if (check_roku_info==false) //(loginDB != "TEST")
                        {
                            SSN1 = _result.Data["ssn1"]?.ToString() ?? "";
                            SSN2 = _result.Data["ssn2"]?.ToString() ?? "";
                            SSN3 = _result.Data["ssn3"]?.ToString() ?? "";
                            SSN4 = _result.Data["ssn4"]?.ToString() ?? "";
                            SSN9 = _result.Data["ssn9"]?.ToString() ?? "";
                            SSN12 = _result.Data["ssn12"]?.ToString() ?? "";
                            SSN13 = _result.Data["ssn13"]?.ToString() ?? "";
                            SSN14 = _result.Data["ssn14"]?.ToString() ?? "";
                            SSN15 = _result.Data["ssn15"]?.ToString() ?? "";
                            SSN16 = _result.Data["ssn16"]?.ToString() ?? "";
                            MAC2 = _result.Data["mac2"]?.ToString() ?? "";
                            MAC3 = _result.Data["mac3"]?.ToString() ?? "";
                            MAC10 = _result.Data["mac10"]?.ToString() ?? "";


                            AddParams("SSN1", SSN1);
                            AddParams("SSN2", SSN2);
                            AddParams("SSN3", SSN3);
                            AddParams("SSN4", SSN4);
                            AddParams("SSN9", SSN9);
                            AddParams("SSN12", SSN12);
                            AddParams("SSN14", SSN14);
                            AddParams("SSN16", SSN16);
                            AddParams("SSN13", SSN13);
                            AddParams("SSN15", SSN15);
                            AddParams("MAC2", MAC2);
                            AddParams("MAC3", MAC3);
                            AddParams("SERIAL_NUMBER", txtInput1.Text);
                            AddParams("BoxSN", txtInput2.Text);
                            AddParams("MAC10", MAC10);

                            //Label box CPEII Sunny confirm
                            sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LOT REPRINT' AND VR_CLASS='SCAN MAC'";
                            var _result_time = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result_time.Data != null)
                            {
                                sql = "SELECT SHIPPING_SN FROM SFISM4.R_WIP_TRACKING_T WHERE SHIPPING_SN2 = '" + txtInput2.Text + "'";
                                _result_time = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result_time.Data != null && (_result_time.Data["shipping_sn"]?.ToString() ?? "") != "N/A" && !string.IsNullOrEmpty((_result_time.Data["shipping_sn"]?.ToString() ?? "")))
                                    AddParams("BoxSN", (_result_time.Data["shipping_sn"]?.ToString() ?? ""));
                            }
                            //END Label box CPEII Sunny confirm
                        }
                        else
                        {
                            SSN1 = _result.Data["ssn1"]?.ToString() ?? "";
                            SSN2 = _result.Data["ssn2"]?.ToString() ?? "";
                            SSN3 = _result.Data["ssn3"]?.ToString() ?? "";
                            SSN4 = _result.Data["ssn4"]?.ToString() ?? "";
                            SSN5 = _result.Data["ssn5"]?.ToString() ?? "";
                            SSN6 = _result.Data["ssn6"]?.ToString() ?? "";
                            SSN7 = _result.Data["ssn7"]?.ToString() ?? "";
                            SSN8 = _result.Data["ssn8"]?.ToString() ?? "";
                            SSN9 = _result.Data["ssn9"]?.ToString() ?? "";
                            MAC1 = _result.Data["mac1"]?.ToString() ?? "";
                            MAC2 = _result.Data["mac2"]?.ToString() ?? "";
                            MAC3 = _result.Data["mac3"]?.ToString() ?? "";
                            MAC4 = _result.Data["mac4"]?.ToString() ?? "";
                            MAC5 = _result.Data["mac5"]?.ToString() ?? "";
                            MAC10 = _result.Data["mac10"]?.ToString() ?? "";


                            AddParams("SSN1", SSN1);
                            AddParams("SSN2", SSN2);
                            AddParams("SSN3", SSN3);
                            AddParams("SSN4", SSN4);
                            AddParams("SSN5", SSN5);
                            AddParams("SSN6", SSN6);
                            AddParams("SSN7", SSN7);
                            AddParams("SSN8", SSN8);
                            AddParams("SSN9", SSN9);
                            AddParams("MAC1", MAC1);
                            AddParams("MAC2", MAC2);
                            AddParams("MAC3", MAC3);
                            AddParams("MAC4", MAC4);
                            AddParams("MAC5", MAC5);
                            AddParams("SERIAL_NUMBER", txtInput1.Text);
                            AddParams("SN", txtInput1.Text);
                        }
                    }
                    sql= "select * from sfism4.r_wip_tracking_t where SERIAL_NUMBER='"+txtInput1.Text+ "' ";
                     _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        AddParams("MSN", _result.Data["shipping_sn"]?.ToString() ?? "");
                        AddParams("SERIAL_NUMBER", _result.Data["serial_number"]?.ToString() ?? "");
                        AddParams("SN", txtInput1.Text);
                    }

                    sql = "select * from sfism4.r_wip_KEYPARTS_t where SERIAL_NUMBER='" + txtInput1.Text + "'  and key_part_no='MACID' ";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        AddParams("MAC", _result.Data["key_part_sn"]?.ToString() ?? "");
                    }
                }
            }


            if (DCI5211.IsChecked)
            {
                dtParams.Clear();
                sql = "select * from SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    SSN1 = _result.Data["ssn1"]?.ToString() ?? "";
                    SSN2 = _result.Data["ssn2"]?.ToString() ?? "";
                    SSN3 = _result.Data["ssn3"]?.ToString() ?? "";
                    SSN4 = _result.Data["ssn4"]?.ToString() ?? "";
                    SSN15 = _result.Data["ssn15"]?.ToString() ?? "";
                    MAC2 = _result.Data["mac2"]?.ToString() ?? "";
                    MAC3 = _result.Data["mac3"]?.ToString() ?? "";

                    AddParams("SSN1", txtInput1.Text.Trim());
                    AddParams("SSN2", txtInput3.Text.Trim());
                    AddParams("SSN3", SSN3);
                    AddParams("SSN4", SSN4);
                    AddParams("SSN15", SSN15);
                    AddParams("MAC2", MAC2);
                    AddParams("MAC3", MAC3);
                }
            }

            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("113") != -1)
            {
                sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T A ,SFISM4.R_CUSTSN_T@E5ASFC B WHERE A.KEY_PART_SN=B.SERIAL_NUMBER AND A.SERIAL_NUMBER='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    MEID = _result.Data["ssn2"]?.ToString() ?? "";
                    IMEI = _result.Data["ssn1"]?.ToString() ?? "";
                    MCN = _result.Data["ssn5"]?.ToString() ?? "";

                    AddParams("MEID", MEID);
                    AddParams("IMEI", IMEI);
                    AddParams("MCN", MCN);
                }
                else
                {
                    showMessage("00654", "00654", false);
                    return null;
                }
            }

            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("084") != -1)
            {
                result = "";
                sql = "select ssn1,ssn2,ssn3,mac1,mac2,mo_number from sfism4.r_custsn_t where SERIAL_NUMBER='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    tmpssn1 = _result.Data["ssn1"]?.ToString() ?? "";
                    tmpssn2 = _result.Data["ssn2"]?.ToString() ?? "";
                    tmpssn3 = _result.Data["ssn3"]?.ToString() ?? "";
                    tmpmac1 = _result.Data["mac1"]?.ToString() ?? "";
                    tmpmac2 = _result.Data["mac2"]?.ToString() ?? "";

                    if (tmpssn3.Length < 4 || tmpssn3 == txtInput3.Text)
                    {
                        if (tmpssn3.Length < 4)
                        {

                            if (txtInput2.Text.Length == 12)
                            {
                                sql = "select substr(data2,-12,12) bb from sfism4.r_undp_data_t  where serial_number='" + txtInput1.Text + "'";
                            }
                            else
                            {
                                sql = "select substr(data2,-11,11) bb from sfism4.r_undp_data_t  where serial_number='" + txtInput1.Text + "'";
                            }
                            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result.Data != null)
                            {
                                if (_result.Data["bb"].ToString() != txtInput2.Text)
                                {
                                    showMessage("SSN3 is wrong: " + txtInput2.Text + " <> " + _result.Data["bb"].ToString(), "Lỗi SSN3: " + txtInput2.Text + " <> " + _result.Data["bb"].ToString(), true);
                                    return null;
                                }
                                sql = "select 1 from sfism4.r_custsn_t where serial_number<>'" + txtInput1.Text + "' and ssn3='" + txtInput1.Text + "'";
                                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result.Data != null)
                                {
                                    showMessage("00272", "00272", false);
                                    return null;
                                }
                                else
                                {
                                    sql = "update sfism4.r_custsn_t set ssn3='" + txtInput3.Text + "' where serial_number='" + txtInput1.Text + "'";
                                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = sql,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (query_update.Result.ToString() != "OK")
                                    {
                                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                                        return null;
                                    }
                                }
                            }
                            else
                            {
                                showMessage("00270", "00270", false);
                                return null;
                            }
                        }
                        AddParams("SSN1", tmpssn1);
                        AddParams("SSN2", tmpssn2);
                        AddParams("PO_NO", txtInput2.Text);
                        AddParams("MAC1", tmpmac1);
                        AddParams("MAC2", tmpmac2);
                        G_sLabl_Name = model_name.Substring(0, 7) + "_" + model_name.Substring(8, 2) + "_BX.LAB";
                        result = "OK";
                        return result;
                    }
                    else
                    {
                        showMessage("has printed Label, the first sn is: " + tmpssn3 + " different from the input", "Đã in Label, trước đây SN nhập vào là: " + tmpssn3 + " Không giống với dữ liệu nhập hiện tại", true);
                        return null;
                    }
                }
                else
                {
                    showMessage("00289", "00289", false);
                    return null;
                }
            }

            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("093") != -1)
            {
                result = "";
                sql = "SELECT A.CUSTSN_PREFIX,C.MO_NUMBER FROM SFIS1.C_CUSTSN_RULE_T A,SFISM4.R_MO_BASE_T B,SFISM4.R_WIP_TRACKING_T C  WHERE C.SERIAL_NUMBER='" + txtInput1.Text + "' " +
                    " AND A.MO_TYPE=B.MO_TYPE AND B.MO_NUMBER=C.MO_NUMBER AND C.MODEL_NAME=A.MODEL_NAME " +
                    " AND B.MODEL_NAME=C.MODEL_NAME AND C.VERSION_CODE=A.VERSION_CODE AND CUSTSN_CODE='SSN4'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00290", "00290", false);
                    return null;
                }
                Cust_sn_Prefix = _result.Data["custsn_prefix"]?.ToString() ?? "";
                wo = _result.Data["mo_number"]?.ToString() ?? "";

                sql = "SELECT A.KEY_PART_SN FROM SFISM4.R_WIP_KEYPARTS_T A,SFISM4.R_WIP_KEYPARTS_T B " +
                    " WHERE  A.KEY_PART_NO='MACID' AND A.SERIAL_NUMBER=B.KEY_PART_SN  AND B.SERIAL_NUMBER='" + txtInput1.Text + "' AND B.GROUP_NAME = 'ASSY'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    sql = "SELECT A.KEY_PART_SN FROM SFISM4.H_WIP_KEYPARTS_T A,SFISM4.R_WIP_KEYPARTS_T B " +
                    " WHERE  A.KEY_PART_NO='MACID' AND A.SERIAL_NUMBER=B.KEY_PART_SN  AND B.SERIAL_NUMBER='" + txtInput1.Text + "' AND B.GROUP_NAME = 'ASSY'";
                }
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00291", "00291", false);
                    return null;
                }

                SSN1 = _result.Data["key_part_sn"]?.ToString() ?? "";
                SSN2 = SSN1;
                SSN3 = txtInput3.Text.Trim();

                sql = "SELECT TO_CHAR(SYSDATE,''MMYY'') SDAY FROM DUAL";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                sDay = _result.Data["sday"]?.ToString() ?? "";
                SSN4 = Cust_sn_Prefix + sDay + txtInput3.Text.Trim();

                AddParams("SSN1", SSN1);
                AddParams("SSN2", SSN2);
                AddParams("SSN3", SSN3);
                AddParams("SSN4", SSN4);
                result = "OK";

                sql = "SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='" + txtInput1.Text.Trim() + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    sql = "UPDATE SFISM4.R_CUSTSN_T SET SSN1=:SSN1,SSN2=:SSN2,SSN3=:SSN3,SSN4=:SSN4,MO_NUMBER=:MO_NUMBER WHERE SERIAL_NUMBER=:SERIAL_NUMBER";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter{Name="SSN1",Value=SSN1,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SSN2",Value=SSN2,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SSN3",Value=SSN3,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SSN4",Value=SSN4,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="MO_NUMBER",Value=wo,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SERIAL_NUMBER",Value=txtInput1.Text,SfcParameterDataType=SfcParameterDataType.Varchar2}
                        }
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return null;
                    }
                }
                else
                {
                    sql = "INSERT INTO SFISM4.R_CUSTSN_T (SERIAL_NUMBER,SSN1,SSN2,SSN3,SSN4,MO_NUMBER) VALUES(:SERIAL_NUMBER,:SSN1,:SSN2,:SSN3,:SSN4,:MO_NUMBER)";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter{Name="SSN1",Value=SSN1,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SSN2",Value=SSN2,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SSN3",Value=SSN3,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SSN4",Value=SSN4,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="MO_NUMBER",Value=wo,SfcParameterDataType=SfcParameterDataType.Varchar2},
                            new SfcParameter{Name="SERIAL_NUMBER",Value=txtInput1.Text,SfcParameterDataType=SfcParameterDataType.Varchar2}
                        }
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return null;
                    }
                }
            }
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("171") != -1)
            {
                result = await tg799nextcustsn(txtInput1.Text.Trim());
                if (result != "OK")
                {
                    return null;
                }
            }
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("083") != -1)
            {
                sql = "select ssn1,ssn3,ssn5 from sfism4.r_custsn_t where SERIAL_NUMBER='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00289", "00289", false);
                    return null;
                }
                else
                {
                    tmpssn1 = _result.Data["ssn1"]?.ToString() ?? "";
                    tmpssn3 = _result.Data["ssn3"]?.ToString() ?? "";
                    tmpssn5 = _result.Data["ssn5"]?.ToString() ?? "";
                    if (tmpssn3.Length > 4)
                    {
                        //Do nothing
                    }
                    else
                    {
                        tmpssn3 = await gett77n081sequence();
                        if (tmpssn3 == "")
                        {
                            showMessage("00292", "00292", false);
                            return null;
                        }
                    }
                    AddParams("SSN1", tmpssn1);
                    AddParams("SSN3", tmpssn3);
                    AddParams("SSN5", tmpssn5);
                    result = "OK";
                    G_sLabl_Name = model_name.Substring(0, 7) + "_" + model_name.Substring(8, 2) + "_BX.LAB";
                    return result;
                }
            }
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("076") != -1)
            {
                result = "";
                sql = "select ssn1,ssn3,ssn5 from sfism4.r_custsn_t where SERIAL_NUMBER='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    tmpssn1 = _result.Data["ssn1"]?.ToString() ?? "";
                    tmpssn3 = _result.Data["ssn3"]?.ToString() ?? "";
                    tmpssn5 = _result.Data["ssn5"]?.ToString() ?? "";

                    AddParams("SSN1", tmpssn1);
                    AddParams("SSN3", tmpssn3);
                    AddParams("SSN5", tmpssn5);
                    result = "OK";
                    G_sLabl_Name = model_name.Substring(0, 7) + "_" + model_name.Substring(8, 2) + "_BX.LAB";
                    return result;
                }
                else
                {
                    showMessage("00289", "00289", false);
                    return null;
                }
            }

            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("189") != -1)
            {
                sql = "select distinct vender_part_no from sfism4.r105 where mo_number in(select distinct  mo_number from sfism4.r107 where " +
                    " (serial_number='" + txtInput1.Text + "' or shipping_sn='" + txtInput1.Text + "'or po_no = '" + txtInput1.Text + "' or shipping_sn2='" + txtInput1.Text + "'))";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null || _result.Data["vender_part_no"].ToString() == "" || _result.Data["vender_part_no"].ToString() == "N/A")
                {
                    showMessage("80389", "80389", false);
                    return null;
                }
                else
                {
                    vender_part = _result.Data["vender_part_no"].ToString().Trim();
                }
            }
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("074") != -1)
            {
                sql = "SELECT SERIAL_NUMBER FROM SFISM4.R_CUSTSN_T WHERE MAC1='" + txtInput1.Text + "' OR SSN1='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00293", "00293", false);
                    return null;
                }
                S_SN = _result.Data["serial_number"]?.ToString() ?? "";
                sql = "SELECT TRACK_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + S_SN + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    if (_result.Data["track_no"].ToString().Length > 10)
                    {
                        CUSTO = _result.Data["track_no"]?.ToString() ?? "";
                        AddParams("CUSTO", CUSTO);

                        sql = "UPDATE SFISM4.R_CUSTSN_T SET SSN4='" + CUSTO + "' WHERE SERIAL_NUMBER='" + S_SN + "'";
                        var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }
                    }
                    else
                    {
                        sql = "select a.model_name model_name,a.cust_sn_prefix ssn_prefix,a.version_code ver,a.cust_version_code hard,a.cust_last_sn last_sn, " +
                            " a.cust_model_desc model,a.upceandata upc,b.mo_number mo_number from sfis1.c_cust_snrule_t a ,sfism4.r107 b where " +
                            " a.model_name=b.model_name and a.version_code=b.version_code and b.serial_number='" + S_SN + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data == null)
                        {
                            showMessage("00084", "00084", false);
                            return null;
                        }
                        LAST_SN = _result.Data["last_sn"]?.ToString() ?? "";
                        smodel = _result.Data["model"]?.ToString() ?? "";
                        supc = _result.Data["upc"]?.ToString() ?? "";
                        wo = _result.Data["mo_number"]?.ToString() ?? "";
                        sVer = _result.Data["ver"]?.ToString() ?? "";
                        sfis_model = _result.Data["model_name"]?.ToString() ?? "";

                        sql = "SELECT to_char(SYSDATE,'MMYY') as YYMM FROM DUAL";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        sYear = _result.Data["yymm"]?.ToString() ?? "";

                        if (LAST_SN.Trim() == "" || LAST_SN.Substring(16, 7) != sYear)
                        {
                            CUSTO = supc + sYear + "0000001";
                        }
                        else
                        {
                            sSQE = (Int32.Parse(LAST_SN.Substring(16, 7)) + 1).ToString();
                            switch (sSQE.Length)
                            {
                                case 6:
                                    sSQE = "0" + sSQE;
                                    break;
                                case 5:
                                    sSQE = "00" + sSQE;
                                    break;
                                case 4:
                                    sSQE = "000" + sSQE;
                                    break;
                                case 3:
                                    sSQE = "0000" + sSQE;
                                    break;
                                case 2:
                                    sSQE = "00000" + sSQE;
                                    break;
                                case 1:
                                    sSQE = "000000" + sSQE;
                                    break;
                            }
                            CUSTO = supc + sYear + sSQE;
                        }

                        sql = "UPDATE SFISM4.R_WIP_TRACKING_T SET TRACK_NO='" + CUSTO + "' WHERE SERIAL_NUMBER='" + S_SN + "'";
                        var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }

                        sql = "UPDATE SFISM4.R_CUSTSN_T SET SSN4='" + CUSTO + "' WHERE SERIAL_NUMBER='" + S_SN + "'";
                        query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }
                        AddParams("CUSTO", CUSTO);

                        sql = "UPDATE SFIS1.C_CUST_SNRULE_T SET CUST_LAST_SN='" + CUSTO + "' WHERE MODEL_NAME='" + sfis_model + "'";
                        query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }
                    }
                }
            }
            //FOR U46H184.XX
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("111") != -1)
            {
                sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_NO=( " +
                    "SELECT KEY_PART_NO FROM SFIS1.C_BOM_KEYPART_T WHERE TYPE='MACID' AND " +
                    "BOM_NO=(SELECT BOM_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "') " +
                    ") AND SERIAL_NUMBER='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00652", "00652", false);
                    return null;
                }
                Tmac = _result.Data["key_part_sn"]?.ToString() ?? "";
                AddParams("TMAC", Tmac);

                sql = "SELECT PO_NO,SHIPPING_SN FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("Not found data in table R107 " + txtInput1.Text, "Không có dữ liệu trong R107 " + txtInput1.Text, true);
                    return null;
                }
                if (_result.Data["po_no"].ToString().Length > 0 || _result.Data["shipping_sn"].ToString().Length > 10)
                {
                    SSN = _result.Data["shipping_sn"]?.ToString() ?? "";
                    AddParams("SSN", SSN);
                }
                else
                {
                    sql = "select a.model_name model_name,a.cust_sn_prefix ssn_prefix,a.version_code ver,a.cust_version_code hard,a.cust_last_sn last_sn," +
                        " a.cust_model_desc model,a.upceandata upc,b.mo_number mo_number from sfis1.c_cust_snrule_t a ,sfism4.r107 b where " +
                        "a.model_name=b.model_name and a.version_code=b.version_code and b.serial_number='" + txtInput1.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        LAST_SN = _result.Data["last_sn"]?.ToString() ?? "";
                        smodel = _result.Data["model"]?.ToString() ?? "";
                        supc = _result.Data["upc"]?.ToString() ?? "";
                        wo = _result.Data["mo_number"]?.ToString() ?? "";
                        sVer = _result.Data["ver"]?.ToString() ?? "";
                        sfis_model = _result.Data["model_name"]?.ToString() ?? "";
                        ssn_prefix = _result.Data["ssn_prefix"]?.ToString() ?? "";

                        sql = "SELECT SYSDATE SSYSDATE,TO_CHAR(SYSDATE,'YYYY') SYEAR,TO_CHAR(SYSDATE, 'D') DOW, TO_CHAR(SYSDATE, 'DDD') DOY, TO_CHAR(SYSDATE,'WW') WOY FROM DUAL";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        sYear = _result.Data["syear"]?.ToString() ?? "";
                        SYEAR4 = sYear.Substring(0, 4);
                        sYear = sYear.Substring(3, 1);
                        sDay = _result.Data["doy"]?.ToString() ?? "";
                        ssDay = _result.Data["dow"]?.ToString() ?? "";
                        sWeek = _result.Data["woy"]?.ToString() ?? "";

                        sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE CUST_LAST_SN LIKE '" + smodel + sYear + sWeek + supc + "%'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data == null)
                        {
                            LAST_SN = "";
                        }
                        else
                        {
                            LAST_SN = _result.Data["cust_last_sn"]?.ToString() ?? "";
                        }

                        if (LAST_SN == "")
                        {
                            last_sn_flag = "0";
                            SSN = smodel + sYear + sWeek + supc + "000001";
                        }
                        else
                        {
                            last_sn_flag = "1";
                            sSQE = (Int32.Parse(LAST_SN.Substring(LAST_SN.Length - 6, 6)) + 1).ToString();

                            switch (sSQE.Length)
                            {
                                case 1:
                                    sSQE = "00000" + sSQE;
                                    break;
                                case 2:
                                    sSQE = "0000" + sSQE;
                                    break;
                                case 3:
                                    sSQE = "000" + sSQE;
                                    break;
                                case 4:
                                    sSQE = "00" + sSQE;
                                    break;
                                case 5:
                                    sSQE = "0" + sSQE;
                                    break;
                            }
                            SSN = smodel + sYear + sWeek + supc + sSQE;
                        }
                        sql = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SHIPPING_SN='" + SSN + "' OR PO_NO='" + SSN + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            showMessage("00297", "00297", false);
                            return null;
                        }

                        sql = "UPDATE SFISM4.R_WIP_TRACKING_T SET SHIPPING_SN='" + SSN + "',PO_NO='" + SSN + "'  WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                        var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }

                        sql = "UPDATE SFIS1.C_CUST_SNRULE_T SET CUST_LAST_SN='" + SSN + "' WHERE MODEL_NAME='" + sfis_model + "'";
                        query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }

                        AddParams("SSN", SSN);
                    }
                    else
                    {
                        showMessage("00084", "00084", false);
                        return null;
                    }
                }
            }
            //begin by vtp 20190510
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G37") != -1)
            {
                result = "";
                sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE  SERIAL_NUMBER IN ( SELECT SERIAL_NUMBER" +
                    " FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "' OR SHIPPING_SN='" + txtInput1.Text + "' OR SHIPPING_SN2='" + txtInput1.Text + "') " +
                    " AND KEY_PART_NO LIKE '810%' AND ROWNUM =1";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00652", "00652", false);
                    return null;
                }
                Tmac = _result.Data["key_part_sn"]?.ToString() ?? "";
                AddParams("TMAC", Tmac);

                sql = "SELECT PO_NO,SHIPPING_SN FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("Not found data R107", "Không tìm thấy dữ liệu trong R107", true);
                    return null;
                }

                if (_result.Data["po_no"].ToString().Length > 10 || _result.Data["shipping_sn"].ToString().Length > 10)
                {
                    SSN = _result.Data["shipping_sn"]?.ToString() ?? "";
                    AddParams("SSN", SSN);
                }
                else
                {
                    sql = "select a.model_name model_name,a.cust_sn_prefix ssn_prefix,a.version_code ver,a.cust_version_code hard,a.cust_last_sn last_sn," +
                        " a.cust_model_desc model,a.upceandata upc,b.mo_number mo_number from sfis1.c_cust_snrule_t a ,sfism4.r107 b where " +
                        "a.model_name=b.model_name and a.version_code=b.version_code and (b.serial_number='" + txtInput1.Text + "' or b.shipping_sn='" + txtInput1.Text + "' )";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        //showMessage("Chị Phương comment lại nên không làm", "Chị Phương comment lại nên không làm", true);
                        //return null;
                    }
                }
            }
            //begin by DXQ 20190516
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G39") != -1)
            {
                result = "";
                sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T A WHERE  SERIAL_NUMBER IN ( SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T " +
                    " WHERE SERIAL_NUMBER='" + txtInput1.Text + "' OR SHIPPING_SN='" + txtInput1.Text + "' OR SHIPPING_SN2='" + txtInput1.Text + "') " +
                    " AND KEY_PART_NO = (SELECT VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='LOT_REPRINT' AND VR_CLASS='PRINTASSY' AND " +
                    " VR_NAME = (SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T  WHERE SERIAL_NUMBER= A.SERIAL_NUMBER) " +
                    " AND VR_DESC ='Y' AND VR_ITEM= A.GROUP_NAME )";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00652", "00652", false);
                    return null;
                }
                Tmac = _result.Data["key_part_sn"]?.ToString() ?? "";
                AddParams("TKEYPART", Tmac);
            }
            //For Han CPEI
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G62") != -1 && txtMoNumber.Text.Length > 9 && !rePrint.IsChecked)
            {
                sql = "select * from SFISM4.R_MO_BASE_T where CLOSE_FLAG ='2' and mo_number ='" + txtMoNumber.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00307", "00307", false);
                    return null;
                }
                C_BOM = _result.Data["bom_no"]?.ToString() ?? "";
                keypart = _result.Data["key_part_no"]?.ToString() ?? "";
                smodel = _result.Data["model_name"]?.ToString() ?? "";
                sVer = _result.Data["version_code"]?.ToString() ?? "";
                sroute = _result.Data["route_code"]?.ToString() ?? "";

                sql = "select*from SFIS1.C_BOM_KEYPART_T where  BOM_NO = '" + C_BOM + "'  and group_name='ASSY'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("80433", "80433", false);
                    return null;
                }
                C_KEYPART = _result.Data["key_part_no"]?.ToString() ?? "";

                sql = "select*from SFISM4.R_MO_EXT_T where  mo_number  ='" + txtMoNumber.Text + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("80431", "80431", false);
                    return null;
                }

                sql = "select mac2,serial_number  from SFISM4.R_CUSTSN_T  where  serial_number  ='" + txtInput1.Text + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("80431", "80431", false);
                    return null;
                }
                C_MAC = _result.Data["mac2"]?.ToString() ?? "";
                txtInput2.Text = C_MAC;

                sql = "select STANDARD,MODEL_NAME,MODEL_RANGE2  from SFIS1.C_MODEL_DESC_T2 where  model_name = '" + smodel + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("No data in MODEL_DESC_T2, Call Labelroom", "Không có dữ liệu trong MODEL_DESC_T2, Gọi Labelroom", true);
                    return null;
                }
                else
                {
                    prefix = _result.Data["standard"]?.ToString() ?? "";
                    length1 = _result.Data["model_range2"]?.ToString() ?? "";

                    sql = "select to_char(sysdate,'IWYY')date_now from dual";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    sDay = _result.Data["date_now"]?.ToString() ?? "";

                    sql = "select count(*) as count from sfism4.R_DATA_INPUT_T  where  mo_number ='" + txtMoNumber.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data["count"].ToString() == "0")
                    {
                        sql = "select item_1 from SFISM4.R_MO_EXT_T where  mo_number  ='" + txtMoNumber.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        item_1 = _result.Data["item_1"]?.ToString() ?? "";
                        txtInput3.Text = item_1;
                    }
                    else
                    {
                        sql = "select MAX(SSN1) AS SSN1_MAX from sfism4.R_DATA_INPUT_T  where  mo_number ='" + txtMoNumber.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        C_MAXSSN1 = _result.Data["ssn1_max"]?.ToString() ?? "";
                        TRUCK_NUMBER = getnextSN(C_MAXSSN1, "0123456789ABCDEF", 1);
                        txtInput3.Text = TRUCK_NUMBER;

                        sql = "select*from SFISM4.R_MO_EXT_T where  mo_number  ='" + txtMoNumber.Text + "' and '" + txtInput3.Text + "' between item_1 and item_2";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data == null)
                        {
                            showMessage("80431", "80431", false);
                            return null;
                        }
                    }
                    sql = "select*from SFISm4.r108 where key_part_sn = '" + txtInput1.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        showMessage("Tồn tại dữ liệu link", "Have link data in table R108", false);
                        return null;
                    }

                    sql = "select*from SFISm4.r107 where  serial_number  = '" + txtInput1.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    key_part = _result.Data["key_part_no"]?.ToString() ?? "";
                    sVer1 = _result.Data["version_code"]?.ToString() ?? "";

                    if (C_KEYPART != key_part)
                    {
                        showMessage("80418", "80418", false);
                        return null;
                    }
                }
                sql = "select*from SFISm4.r107 where  serial_number  = '" + txtInput3.Text + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    sql = "Insert into SFISM4.R108 (EMP_NO, SERIAL_NUMBER, KEY_PART_NO, KEY_PART_SN, KP_RELATION, GROUP_NAME, CARTON_NO, VERSION, KP_CODE, MO_NUMBER,WORK_TIME) " +
                        " Values('LOTREPRINT','" + txtInput3.Text + "', '" + C_KEYPART + "', '" + txtInput1.Text + "', '1','ASSY', 'N/A',  '" + sVer1 + "', 'N/A', '" + txtMoNumber.Text + "',SYSDATE)";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return null;
                    }

                    sql = "select*from SFISm4.R_DATA_INPUT_T where  ssn1  = '" + txtInput3.Text + "' or mac1 ='" + txtInput2.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        sql = "Insert into SFISM4.R_DATA_INPUT_T(MO_NUMBER,MAC1, SSN1, SSN9, SSN10, PRINT_FLAG,INPUT_TIME, PRINT_TIME) " +
                            " Values('" + txtMoNumber.Text + "','" + txtInput2.Text + "', '" + txtInput3.Text + "', '" + smodel + "', '" + sVer + "', 'Y', sysdate, sysdate)";
                        query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }
                    }

                    sql = "select*from SFISm4.R_CUSTSN_T  where  ssn1  = '" + txtInput3.Text + "' or mac1 ='" + txtInput2.Text + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        sql = "Insert into SFISM4.R_CUSTSN_T(SERIAL_NUMBER, SSN1, MO_NUMBER, MAC1, IN_STATION_TIME, GROUP_NAME, SSN7) " +
                            " Values('" + txtInput3.Text + "', '" + txtInput3.Text + "', '" + txtMoNumber.Text + "', '" + txtInput2.Text + "', SYSDATE, 'ASSY', '" + txtInput1.Text + "')";
                        query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }
                    }
                    else
                    {
                        sql = "update SFISM4.R_CUSTSN_T set MAC1 = '" + txtInput2.Text + "' where serial_number= '" + txtInput3.Text + "'";
                        query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }
                    }

                    sql = "SELECT GROUP_NEXT FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE = '" + sroute + "' AND STEP_SEQUENCE = " +
                        "(SELECT MAX (STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T  WHERE   GROUP_NAME = 'ASSY'  AND ROUTE_CODE = '" + sroute + "' )";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    SNEXT = _result.Data["group_next"]?.ToString() ?? "";

                    sql = "Insert into SFISM4.R107 (SERIAL_NUMBER, MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME,SECTION_NAME, GROUP_NAME, STATION_NAME, STATION_SEQ," +
                        "ERROR_FLAG,IN_STATION_TIME, SHIPPING_SN, WORK_FLAG, FINISH_FLAG, ENC_CNT," +
                        "SPECIAL_ROUTE,  SCRAP_FLAG, NEXT_STATION, CUSTOMER_NO, BOM_NO, KEY_PART_NO, CARTON_NO, REPAIR_CNT, PALLET_FULL_FLAG,WIP_GROUP,SHIPPING_SN2) " +
                        " Values('" + txtInput3.Text + "','" + txtMoNumber.Text + "', '" + smodel + "', '" + sVer + "', 'L6','SI', 'ASSY', 'ASSY_1_043', '1', '0',SYSDATE, 'N/A', '0', '0', '0'," +
                        "'" + sroute + "','0', 'N/A', '80190', '" + C_BOM + "', '" + keypart + "','N/A', '0', 'N','" + SNEXT + "','" + txtInput2.Text + "')";
                    query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return null;
                    }

                    sql = "Insert into SFISM4.R117 SELECT*FROM SFISM4.R107 WHERE SERIAL_NUMBER ='" + txtInput3.Text + "' AND GROUP_NAME ='ASSY' AND WIP_GROUP IS NOT NULL AND ROWNUM='1'";
                    query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return null;
                    }
                }
                else
                {
                    showMessage("Bản đã tồn tại trong R107", "Bản đã tồn tại trong R107", true);
                    return null;
                }
                sql = "SELECT *  FROM SFISM4.R_CUSTSN_T where serial_number ='" + txtInput3.Text + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                SSN = _result.Data["ssn1"]?.ToString() ?? "";
                C_MAC = _result.Data["mac1"]?.ToString() ?? "";
                AddParams("SSN", SSN);
                AddParams("MAC", C_MAC);
            }
            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G62") != -1 && txtMoNumber.Text.Length > 9 && rePrint.IsChecked)
            {
                sql = "SELECT SSN1,MAC1,SSN7  FROM SFISM4.R_CUSTSN_T where SSN7 ='" + txtInput1.Text + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    SSN = _result.Data["ssn1"]?.ToString() ?? "";
                    C_MAC = _result.Data["mac1"]?.ToString() ?? "";
                    AddParams("SSN", SSN);
                    AddParams("MAC", C_MAC);
                    txtInput2.Text = C_MAC;
                    txtInput3.Text = SSN;
                }
            }
            //DXQ ADD 20200417
            sql = "SELECT * FROM SFISM4.R_WIP_TRACKING_T  WHERE SERIAL_NUMBER='" + txtInput1.Text + "' AND ROWNUM=1";
            var _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result1.Data != null)
            {
                AddParams("SHIPPING_SN", _result1.Data["shipping_sn"]?.ToString() ?? "");
            }
            if (STBLabel.IsChecked)
            {
                result = "";
                sql = "SELECT PO_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER IN(SELECT KEY_PART_SN FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_NO='U46X095T02' AND SERIAL_NUMBER='" + txtInput1.Text + "')";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    if (_result.Data["po_no"].ToString().Length > 10)
                    {
                        SSN = _result.Data["po_no"]?.ToString() ?? "";
                        sql = "select a.model_name model_name,a.version_code ver,a.cust_version_code hard,a.cust_last_sn last_sn,a.cust_model_desc model,a.upceandata upc from " +
                            "sfis1.c_cust_snrule_t a ,sfism4.r107 b where  a.model_name=b.model_name and b.serial_number='" + txtInput1.Text + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            supc = _result.Data["upc"]?.ToString() ?? "";
                            SSN = SSN.Substring(0, 14) + supc + SSN.Substring(22, 14);
                        }
                        else
                        {
                            showMessage("00299", "00299", false);
                            return null;
                        }
                        AddParams("SSN", SSN);
                        result = "OK";
                        return result;
                    }
                    else
                    {
                        showMessage("00300", "00300", false);
                        return null;
                    }
                }
                else
                {
                    showMessage("00301", "00301", false);
                    return null;
                }
            }

            if (BB.IsChecked)
            {
                if (txtInput2.Text.Trim().Length == 12)
                {
                    sql = "select * from SFISM4.R_CUSTSN_T WHERE MAC1='" + txtInput2.Text + "'";
                }
                else
                {
                    sql = "select * from SFISM4.R_CUSTSN_T WHERE ssn1='" + txtInput2.Text + "'";
                }
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    SN = _result.Data["serial_number"]?.ToString() ?? "";
                    SSN1 = _result.Data["ssn1"]?.ToString() ?? "";
                    SSN2 = _result.Data["ssn2"]?.ToString() ?? "";
                    SSN3 = _result.Data["ssn3"]?.ToString() ?? "";
                    SSN4 = _result.Data["ssn4"]?.ToString() ?? "";
                    SSN9 = _result.Data["ssn9"]?.ToString() ?? "";
                    MAC2 = _result.Data["mac2"]?.ToString() ?? "";
                    MAC3 = _result.Data["mac3"]?.ToString() ?? "";
                    MAC4 = _result.Data["mac4"]?.ToString() ?? "";

                    AddParams("SSN1", SSN1);
                    AddParams("SSN2", SSN2);
                    AddParams("SSN3", SSN3);
                    AddParams("SSN4", SSN4);
                    AddParams("SSN9", SSN9);
                    AddParams("MAC2", MAC2);
                    AddParams("MAC3", MAC3);
                    AddParams("MAC3", MAC4);
                }
                else
                {
                    showMessage("00293", "00293", false);
                    return null;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("047") != -1)
                {
                    sql = "select * from SFISM4.R_Wip_Keyparts_T WHERE Key_Part_SN='" + txtInput1.Text + "'";
                    var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        SN = _result.Data["serial_number"]?.ToString() ?? "";
                    }
                    else
                    {
                        showMessage("40111", "40111", false);
                        return null;
                    }
                }
                else
                {
                    if (txtMoNumber.Text.Length > 0)
                    {
                        SN = txtInput3.Text;
                    }
                    else
                    {
                        SN = txtInput1.Text;
                    }
                }
            }

            result = "SFIS";
            if (shipPing.IsChecked)
            {
                sTable = "SFISM4.Z_WIP_TRACKING_T";
            }
            else
            {
                sTable = "SFISM4.R_WIP_TRACKING_T";
            }
            sql = "select * from " + sTable + " where (serial_number = '" + SN + "' or  shipping_sn = '" + SN + "' or shipping_sn2='" + SN + "' or po_no='" + SN + "' )";
            var _result_check = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result_check.Data != null)
            {
                str_group_name = _result_check.Data["group_name"]?.ToString() ?? "";
                str_model_name = _result_check.Data["model_name"]?.ToString() ?? "";
                str_serial_number = _result_check.Data["serial_number"]?.ToString() ?? "";
                M_sTRACK_NO = _result_check.Data["track_no"]?.ToString() ?? "";
            }
            else
            {
                sTable = "SFISM4.Z_WIP_TRACKING_T";
                sql = "select * from " + sTable + " where (serial_number = '" + SN + "' or  shipping_sn = '" + SN + "' or shipping_sn2='" + SN + "')";
                if (inputCustData.IsChecked)
                {
                    sql += " or ( mcarton_no = '" + SN + "' or imei = '" + SN + "'";
                }
                else
                {
                    sql += " or ( carton_no = '" + SN + "' or  pallet_no = '" + SN + "'";
                }
                sql += " or mo_number = '" + SN + "')";
                _result_check = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            }
            if (_result_check.Data == null)
            {
                showMessage("00316", "00316", false);
                txtInput1.Focus();
                txtInput1.SelectAll();
                return null;
            }
            else
            {
                if (!string.IsNullOrEmpty(UI_Route.Content.ToString().Replace("Route: ", "")) && str_group_name != UI_Route.Content.ToString().Replace("Route: ", ""))
                {
                    if (BPinCode.IsChecked || PinCode.IsChecked)
                    {
                        result = "";
                    }
                    printflag = false;
                    txtInput2.Text = "";
                    txtInput3.Text = "";
                    showMessage("00247", "00247", false);
                    return null;
                }
                if (_result_check.Data["mo_number"].ToString() == "N/A" || string.IsNullOrEmpty(_result_check.Data["mo_number"].ToString()))
                {
                    showMessage("00010", "00010", false);
                    return null;
                }
                else
                {
                    My_MoNumber = _result_check.Data["mo_number"]?.ToString() ?? "";
                    M_CUST_CODE = _result_check.Data["customer_no"]?.ToString() ?? "";
                    model_name = _result_check.Data["model_name"]?.ToString() ?? "";
                    M_VERSION_CODE = _result_check.Data["version_code"]?.ToString() ?? "";
                    M_PART_NO = _result_check.Data["key_part_no"]?.ToString() ?? "";
                    PoNo_String = _result_check.Data["po_no"]?.ToString() ?? "";
                    if (Kind.SelectedItem == chkBoxLabel && txtMoNumber.Text.Length == 0)
                    {
                        txtInput1.Text = _result_check.Data["serial_number"]?.ToString() ?? "";
                    }
                }
            }
            sql = "select * from sfism4.r_custsn_t where serial_number= '" + str_serial_number + "'";
            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result1.Data != null)
            {
                AddParams("PSSN1", _result1.Data["ssn1"]?.ToString() ?? "");
                AddParams("PSSN2", _result1.Data["ssn2"]?.ToString() ?? "");
                AddParams("PSSN3", _result1.Data["ssn3"]?.ToString() ?? "");
                AddParams("PSSN4", _result1.Data["ssn4"]?.ToString() ?? "");
                AddParams("PSSN5", _result1.Data["ssn5"]?.ToString() ?? "");
                AddParams("PSSN7", _result1.Data["ssn7"]?.ToString() ?? "");
                AddParams("PMAC1", _result1.Data["mac1"]?.ToString() ?? "");
                AddParams("PMAC2", _result1.Data["mac2"]?.ToString() ?? "");
                AddParams("PMAC3", _result1.Data["mac3"]?.ToString() ?? "");
                AddParams("PMAC4", _result1.Data["mac4"]?.ToString() ?? "");
                AddParams("PMAC5", _result1.Data["mac5"]?.ToString() ?? "");
                AddParams("PMAC6", _result1.Data["mac6"]?.ToString() ?? "");

                sql = "select * from SFIS1.C_PARAMETER_INI where prg_name ='LOT_Reprint'  and vr_item  ='TRUE'";
                var check = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (check.Data != null)
                {
                    AddParams("PSSN31", _result1.Data["ssn31"]?.ToString() ?? "");
                }

                //string sql_check = "select * from sfis1.c_parameter_ini where prg_name='HH_PrintLabel' and vr_class='ROKU' and vr_value='Y' ";
                //_result_check_param = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                //{
                //    CommandText = sql_check,
                //    SfcCommandType = SfcCommandType.Text
                //});

                if (check_roku_info==false)//(loginDB != "TEST")
                {
                    AddParams("PMAC7", _result1.Data["macid7"]?.ToString() ?? "");
                }
            }

            sql = "select * from SFISM4.R_NETG_PRIN_ALL_T where SHIPPING_SN = '" + txtInput2.Text + "'";
            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result1.Data != null)
            {
                AddParams("CMMAC", _result1.Data["cm_mac"]?.ToString() ?? "");
                AddParams("PASSWORD", _result1.Data["password"]?.ToString() ?? "");
            }

            sql = "select lot_flag,version_code,pmcc,mo_type from sfism4.r_mo_base_t where mo_number = '" + My_MoNumber + "'";
            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result1.Data == null)
            {
                sql = "select lot_flag,version_code,pmcc,mo_type from sfism4.H_mo_base_t where mo_number = '" + My_MoNumber + "'";
                _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            }
            if (_result1.Data == null)
            {
                showMessage("00316", "00316", false);
                txtInput1.Focus();
                txtInput1.SelectAll();
                return null;
            }
            else
            {
                M_PMCC = _result1.Data["pmcc"]?.ToString() ?? "";
                MoType = _result1.Data["mo_type"]?.ToString() ?? "";
                if ((_result1.Data["lot_flag"]?.ToString() ?? "") == "LOT")
                {
                    result = "LOT";
                    M_VERSION_CODE = _result1.Data["version_code"]?.ToString() ?? "";
                    if (Kind.SelectedItem == chkPalletLabel)
                    {
                        G_sLabl_Name = M_MODEL_NAME.Substring(0, 7) + "_" + M_MODEL_NAME.Substring(8, 2) + ".LAB";
                        smodel = M_MODEL_NAME;
                        paramData = My_MoNumber;
                    }
                }
                else
                {
                    result = "SFIS";
                    //Box
                    if (Kind.SelectedItem == chkBoxLabel)
                    {
                        paramData = txtInput2.Text.ToUpper();
                    }
                    //Carton
                    else if (Kind.SelectedItem == chkCartonLabel)
                    {
                        if (inputCustData.IsChecked)
                        {
                            if ((_result_check.Data["mcarton_no"]?.ToString() ?? "") == "N/A" || (_result_check.Data["mcarton_no"]?.ToString() ?? "") == "")
                            {
                                showMessage("00070", "00070", false);
                                return null;
                            }
                            else
                            {
                                paramData = _result_check.Data["mcarton_no"]?.ToString() ?? "";
                            }
                        }
                        else
                        {
                            if ((_result_check.Data["carton_no"]?.ToString() ?? "") == "N/A" || (_result_check.Data["carton_no"]?.ToString() ?? "") == "")
                            {
                                showMessage("00070", "00070", false);
                                return null;
                            }
                            else
                            {
                                paramData = _result_check.Data["carton_no"]?.ToString() ?? "";
                            }
                        }
                        AddParams("CartonNO", paramData);
                        AddParams("Carton_NO", paramData);
                        AddParams("MCartonNO", paramData);
                    }
                    //Pallet
                    else if (Kind.SelectedItem == chkPalletLabel)
                    {
                        if (inputCustData.IsChecked)
                        {
                            if (_result_check.Data["imei"].ToString() == "N/A" || string.IsNullOrEmpty(_result_check.Data["imei"].ToString()))
                            {
                                showMessage("00279", "00279", false);
                                return null;
                            }
                            else
                            {
                                paramData = _result_check.Data["imei"]?.ToString() ?? "";
                            }
                        }
                        else
                        {
                            if (_result_check.Data["pallet_no"].ToString() == "N/A" || string.IsNullOrEmpty(_result_check.Data["pallet_no"].ToString()))
                            {
                                showMessage("00279", "00279", false);
                                return null;
                            }
                            else
                            {
                                paramData = _result_check.Data["pallet_no"]?.ToString() ?? "";
                            }
                        }
                        AddParams("PART_NO", M_PART_NO);
                        AddParams("CUST_PALLET_NO", _result_check.Data["imei"]?.ToString() ?? "");
                        if (inputCustData.IsChecked)
                        {
                            AddParams("PALLET_NO", _result_check.Data["imei"]?.ToString() ?? "");
                        }
                        else
                        {
                            AddParams("PALLET_NO", _result_check.Data["pallet_no"]?.ToString() ?? "");
                        }
                        AddParams("AMBIT_PALLET_NO", _result_check.Data["pallet_no"]?.ToString() ?? "");
                    }
                    AddParams("PackLotNO", M_PMCC);
                    AddParams("Mo_Number", My_MoNumber);
                    AddParams("Part_No", M_PART_NO);
                    AddParams("ModelName", M_MODEL_NAME);
                    AddParams("Version", M_VERSION_CODE);

                    sql = "select * from SFIS1.C_PACK_PARAM_T where model_name = '" + M_MODEL_NAME + "'";
                    _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result1.Data != null)
                    {
                        if (Kind.SelectedItem == chkBoxLabel || Kind.SelectedItem == chkPalletLabel)
                        {
                            sql = "select * from sfis1.c_cust_snrule_t where model_name = '" + M_MODEL_NAME + "' and version_code = '" + M_VERSION_CODE + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data == null)
                            {
                                showMessage("00147", "00147", false);
                                return null;
                            }
                            M_sCustModelName = _result1.Data["cust_model_name"]?.ToString() ?? "";
                            M_sCustModelDesc = _result1.Data["cust_model_desc"]?.ToString() ?? "";
                            M_sUPCEANData = _result1.Data["upceandata"]?.ToString() ?? "";
                            G_sLabl_Name = _result1.Data["carton_lab_name"]?.ToString() ?? "";

                            AddParams("ModelDesc", M_sCustModelDesc);
                            AddParams("Remark", M_sCustModelDesc);
                            AddParams("CUST_PART_NO", M_sCustModelName);

                           if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("074") == -1)
                            {
                                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("189") != -1)
                                {
                                    if (M_sTRACK_NO.Trim() == "" || M_sTRACK_NO.Trim() == "N/A")
                                    {
                                        AddParams("UPCEACDATA", vender_part);
                                    }
                                    else
                                    {
                                        if (M_sTRACK_NO.Trim() == vender_part.Trim())
                                        {
                                            AddParams("UPCEACDATA", vender_part);
                                        }
                                        else
                                        {
                                            showMessage("00303", "00303", false);
                                            return null;
                                        }
                                    }
                                }
                                else
                                {
                                    if (M_sTRACK_NO.Trim() == "" || M_sTRACK_NO == "N/A")
                                    {
                                        AddParams("UPCEACDATA", M_sUPCEANData);
                                    }
                                    else
                                    {
                                        if (M_sTRACK_NO.Trim() == M_sUPCEANData.Trim())
                                        {
                                            AddParams("UPCEACDATA", M_sUPCEANData);
                                        }
                                        else
                                        {
                                            showMessage("00303", "00303", false);
                                            return null;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (M_sTRACK_NO.Trim() == "" || M_sTRACK_NO.Trim() == "N/A")
                    {
                        if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("189") != -1)
                        {
                            sql = "update sfism4.r_wip_tracking_t set track_no= '" + vender_part + "' where (serial_number = '" + SN + "' or  shipping_sn = '" + SN + "')";
                        }
                        else
                        {
                            sql = "update sfism4.r_wip_tracking_t set track_no= '" + M_sUPCEANData + "' where (serial_number = '" + SN + "' or  shipping_sn = '" + SN + "')";
                        }
                        var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sql,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (query_update.Result.ToString() != "OK")
                        {
                            showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                            return null;
                        }
                    }
                    //Box label
                    if (Kind.SelectedItem == chkBoxLabel)
                    {
                        if (!String.IsNullOrEmpty(M_MODEL_NAME) && M_MODEL_NAME.IndexOf(".") != -1)
                        {
                            //Nếu sai bỏ comment câu dưới
                            //G_sLabl_Name = M_MODEL_NAME.Substring(0, M_MODEL_NAME.IndexOf(".")) + "_" + M_MODEL_NAME.Substring(M_MODEL_NAME.IndexOf(".") + 1, M_MODEL_NAME.Length - M_MODEL_NAME.IndexOf(".") - 1) + "_BX.LAB";
                            G_sLabl_Name = M_MODEL_NAME.Replace(".", "_") + "_BX.LAB";
                        }
                        else
                        {
                            sql = "SELECT*FROM SFISM4.R_MO_BASE_T A, SFIS1.C_PARAMETER_INI B WHERE A.MO_NUMBER='" + My_MoNumber + "' AND B.PRG_NAME='PACKING' AND B.VR_CLASS='CHECK_RMA' AND B.VR_ITEM='MO_PREFIX' AND SUBSTR(A.MO_NUMBER,0,B.VR_VALUE)=B.VR_NAME";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data != null)
                            {
                                G_sLabl_Name = M_MODEL_NAME + "_RMA_BX.LAB";
                            }
                            else
                            {
                                G_sLabl_Name = M_MODEL_NAME + "_BX.LAB";
                            }

                        }

                        Cust_sn_Prefix = "";
                        Cust_sn_prefix2 = "";
                        Cust_sn_postfix = "";
                        CUSTSN_STRING = "";
                        BlnCheckSSN = "N";
                        Cust_sn_len = 0;
                        MACID_PREFIX = "";

                        if (CHECK_PONO.IsChecked)
                        {
                            checkssnrule1 = "SSN1";
                            checkssnrule2 = "SSN1";

                            sql = "SELECT * FROM  SFIS1.C_PRGSCAN_SEQUENCE_T WHERE MODEL_NAME='" + M_MODEL_NAME + "' AND GROUP_NAME='" + MGROUP_NAME + "'";
                            result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_list.Data != null)
                            {
                                foreach (var row in result_list.Data)
                                {
                                    if (row["scan_seq"].ToString() == "2")
                                    {
                                        checkssnrule1 = row["check_custsn_rule"]?.ToString() ?? "";
                                    }
                                    if (row["scan_seq"].ToString() == "3")
                                    {
                                        checkssnrule2 = row["check_custsn_rule"]?.ToString() ?? "";
                                    }
                                }
                            }

                            sql = "SELECT * FROM SFIS1.C_CUSTSN_RULE_T WHERE MODEL_NAME='" + M_MODEL_NAME + "' AND VERSION_CODE='" + M_VERSION_CODE + "' AND Mo_Type='" + MoType + "'";
                            result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_list.Data != null)
                            {
                                foreach (var row in result_list.Data)
                                {
                                    if (row["custsn_code"].ToString() == checkssnrule1)
                                    {
                                        Cust_sn_Prefix = row["custsn_prefix"]?.ToString() ?? "";
                                        Cust_sn_prefix2 = "";
                                        Cust_sn_postfix = row["custsn_postfix"]?.ToString() ?? "";
                                        Cust_sn_len = Int32.Parse(row["custsn_leng"]?.ToString() ?? "");
                                        CUSTSN_STRING = row["custsn_str"]?.ToString() ?? "";
                                        BlnCheckSSN = row["check_ssn"]?.ToString() ?? "";
                                    }
                                    if ((row["custsn_code"]?.ToString() ?? "") == "MAC1")
                                    {
                                        MACID_PREFIX = row["custsn_prefix"]?.ToString() ?? "";
                                    }
                                }
                            }
                        }
                        else
                        {
                            checkssnrule1 = "SSN1";
                            sql = "SELECT * FROM  SFIS1.C_PRGSCAN_SEQUENCE_T WHERE MODEL_NAME='" + M_MODEL_NAME + "' AND GROUP_NAME='" + MGROUP_NAME + "'";
                            result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_list.Data != null)
                            {
                                foreach (var row in result_list.Data)
                                {
                                    if (row["scan_seq"].ToString() == "2")
                                    {
                                        checkssnrule1 = row["check_custsn_rule"]?.ToString() ?? "";
                                    }
                                    if (row["scan_seq"].ToString() == "3")
                                    {
                                        checkssnrule2 = row["check_custsn_rule"]?.ToString() ?? "";
                                    }
                                }
                            }

                            sql = "SELECT * FROM SFIS1.C_CUSTSN_RULE_T WHERE MODEL_NAME='" + M_MODEL_NAME + "' AND VERSION_CODE='" + M_VERSION_CODE + "' AND Mo_Type='" + MoType + "'";
                            result_list = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_list.Data != null)
                            {
                                foreach (var row in result_list.Data)
                                {
                                    if (row["custsn_code"].ToString() == checkssnrule1)
                                    {
                                        Cust_sn_Prefix = row["custsn_prefix"]?.ToString() ?? "";
                                        Cust_sn_len = Cust_sn_Prefix.Length + Int32.Parse(row["custsn_leng"].ToString());
                                        CUSTSN_STRING = row["custsn_str"]?.ToString() ?? "";
                                    }
                                }
                            }
                        }

                        sql = "select model_type from sfis1.c_model_desc_t where model_name = '" + M_MODEL_NAME + "'";
                        _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result1.Data != null)
                        {
                            model_type = _result1.Data["model_type"]?.ToString() ?? "";
                        }

                        sql = "SELECT * FROM   SFIS1.C_MODELTYPE_MATCH_T WHERE MODEL_NAME='" + M_MODEL_NAME + "'  and MODEL_DESC='REPRINT' and rownum=1";
                        _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result1.Data != null)
                        {
                            if (_result1.Data["model_type"].ToString() != "REPRINT")
                            {
                                sql = "SELECT * FROM SFIS1.C_MODELTYPE_MATCH_T WHERE MODEL_NAME='" + M_MODEL_NAME + "' and MODEL_DESC='REPRINT' and model_type='" + PCMAC + "' and rownum=1";
                                _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result1.Data != null)
                                {
                                    sql = "SELECT PO_NO,SHIPPING_SN,EMP_NO,BILL_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                                    _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (_result1.Data != null)
                                    {
                                        string ztemp = txtSeditLabelQty.Text.Trim() + txtSeditfCount.Text.Trim() + txtSedprtnow.Text.Trim();
                                        tmp_msn = _result1.Data["bill_no"]?.ToString() ?? "";
                                        if (("LT" + ztemp).IndexOf(tmp_msn) != -1 &&!PrintTest.IsChecked)
                                        {
                                            frmLogin frmLogin = new frmLogin();
                                            frmLogin.sfcClient = sfcClient;
                                            frmLogin.type = "MSG";
                                            if (frmLogin.emp_input != txtInput1.Text)
                                            {
                                                frmLogin.ShowDialog();
                                                await saveReprintRd(frmLogin.emp_input, txtInput1.Text, ztemp);
                                            }
                                            else
                                            {
                                                /*MSGFORM.SNNUMBER:='123';
                                                  tmp_str1:=  FieldByName('EMP_NO').AsString;
                                                  tmp_str1:=tmp_str1+'+'+MSGFORM.lbl1.Caption;
                                                  tmp_str1:=Copy(tmp_str1,1,25)  ;
                                                  Close;
                                                  sql.Clear;
                                                  sql.Add('UPDATE SFISM4.R107 SET EMP_NO=:MEMP_NO WHERE SERIAL_NUMBER=:MSERIAL_NUMBER  ');
                                                  ParamByName('MEMP_NO').AsString:=tmp_str1;
                                                  ParamByName('MSERIAL_NUMBER').AsString:=editInput.Text;
                                                  EXECSQL;*/


                                                //frmLogin.emp_input = "123";
                                                //tmp_str1 = _result1.Data["emp_no"]?.ToString() ?? "";
                                                //tmp_str1 = tmp_str1 + 
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sql = "SELECT PO_NO,SHIPPING_SN,EMP_NO,BILL_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + txtInput1.Text + "'";
                                _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result1.Data != null)
                                {
                                    string ztemp = txtSeditLabelQty.Text.Trim() + txtSeditfCount.Text.Trim() + txtSedprtnow.Text.Trim();
                                    tmp_msn = _result1.Data["bill_no"]?.ToString() ?? "";
                                    if (tmp_msn.IndexOf(("LT" + ztemp)) != -1 && !PrintTest.IsChecked)
                                    {
                                        frmLogin frmLogin = new frmLogin();
                                        frmLogin.sfcClient = sfcClient;
                                        frmLogin.type = "MSG";
                                        if (frmLogin.emp_input != txtInput1.Text)
                                        {
                                            frmLogin.ShowDialog();
                                            if (frmLogin.ok != System.Windows.Forms.DialogResult.OK)
                                            {
                                                return null;
                                            }
                                            await saveReprintRd(frmLogin.emp_input, txtInput1.Text, ztemp);
                                        }
                                        else
                                        {
                                            /*MSGFORM.SNNUMBER:='123';
                                              tmp_str1:=  FieldByName('EMP_NO').AsString;
                                              tmp_str1:=tmp_str1+'+'+MSGFORM.lbl1.Caption;
                                              tmp_str1:=Copy(tmp_str1,1,25)  ;
                                              Close;
                                              sql.Clear;
                                              sql.Add('UPDATE SFISM4.R107 SET EMP_NO=:MEMP_NO WHERE SERIAL_NUMBER=:MSERIAL_NUMBER  ');
                                              ParamByName('MEMP_NO').AsString:=tmp_str1;
                                              ParamByName('MSERIAL_NUMBER').AsString:=editInput.Text;
                                              EXECSQL;*/


                                            //frmLogin.emp_input = "123";
                                            //tmp_str1 = _result1.Data["emp_no"]?.ToString() ?? "";
                                            //tmp_str1 = tmp_str1 + 
                                        }
                                    }
                                }
                            }
                        }

                        sql = "select FINISH_GOOD2,Cust_Model_Name,CustModel_Desc1,CustModel_Desc2,CustModel_Desc3,CustModel_Desc4,CustModel_Desc5,Customer_Name " +
                            " from sfis1.C_CUST_MODEL_T where model_name = '" + M_MODEL_NAME + "'";
                        _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result1.Data != null)
                        {
                            FINISH_GOOD2 = _result1.Data["finish_good2"]?.ToString() ?? "";
                            sCustModel_Desc1 = _result1.Data["custmodel_desc1"]?.ToString() ?? "";
                            AddParams("FINISHGOOD", FINISH_GOOD2);
                            AddParams("CMODELDESC1", sCustModel_Desc1);

                            sCustModel_Name = _result1.Data["cust_model_name"]?.ToString() ?? "";
                            AddParams("CModelName", sCustModel_Name);

                            sCustModel_Desc2 = _result1.Data["custmodel_desc2"]?.ToString() ?? "";
                            AddParams("CModelName", sCustModel_Name);

                            sCustModel_Desc3 = _result1.Data["custmodel_desc3"]?.ToString() ?? "";
                            AddParams("CMODELDESC3", sCustModel_Desc3);

                            sCustModel_Desc4 = _result1.Data["custmodel_desc4"]?.ToString() ?? "";
                            if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("071") != -1)
                            {
                                sCustModel_Desc4 = sCustModel_Desc4.Substring(0, sCustModel_Desc4.Length - 1);
                            }
                            AddParams("CMODELDESC4", sCustModel_Desc4);

                            sCustModel_Desc5 = _result1.Data["custmodel_desc5"]?.ToString() ?? "";
                            AddParams("CMODELDESC5", sCustModel_Desc5);

                            sCustomer_Name = _result1.Data["customer_name"]?.ToString() ?? "";
                            AddParams("CCUSTOMERNAME", sCustomer_Name);
                        }

                        //if (POS('B',M_model_type)>0) OR (POS('C',M_model_type)>0) then
                        if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("B") != -1 || model_type.IndexOf("C") != -1))
                        {
                            if (noCheckSSN.IsChecked == false)
                            {
                                if (txtInput1.Text != txtInput2.Text && noCheckSSN.IsChecked == false)
                                {
                                    showMessage("00304", "00304", false);
                                    return "";
                                }
                                if (txtInput2.Text.Length != Cust_sn_len)
                                {
                                    showMessage("40005", "40005", false);
                                    return "";
                                }
                                if (txtInput2.Text.Substring(0, (Cust_sn_Prefix + Cust_sn_prefix2).Length) != Cust_sn_Prefix + Cust_sn_prefix2) //chuc nang se khong bao gio xay ra
                                {
                                    showMessage("00048", "00048", false);
                                    return "";
                                }
                                if (txtInput2.Text.Substring(Cust_sn_len - Cust_sn_postfix.Length + 1, Cust_sn_postfix.Length) != Cust_sn_postfix)
                                {
                                    showMessage("00222", "00222", false);
                                    return "";
                                }
                                for (i = 0; i <= (Cust_sn_len - Cust_sn_postfix.Length); i++)
                                {
                                    if (!(txtInput2.Text.Substring(i, 1).IndexOf(CUSTSN_STRING) == -1))
                                    {
                                        showMessage("00032", "00032", false);
                                        return "";
                                    }
                                }
                                if (PoNo_String != "" && PoNo_String != "N/A")
                                {
                                    if (PoNo_String != txtInput2.Text && CHECK_PONO.IsChecked)
                                    {
                                        if (checkssnrule1 == "SSN1")
                                        {
                                            showMessage("00309", "00309", false);
                                            return "";
                                        }
                                        else
                                        {
                                            showMessage("00336", "00336", false);
                                            return "";
                                        }
                                    }
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("V") != -1)
                        {
                            if (!await findMacID(SN))
                            {
                                showMessage("00310", "00310", false);
                                return "";
                            }
                            else
                            {
                                prt_mac = macID;
                                if (prt_mac != txtInput2.Text)
                                {
                                    sql = "select ssn1 from sfism4.r_custsn_t where serial_number='" + SN + "'";
                                    _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (_result1.Data != null)
                                    {
                                        showMessage("00311", "00311", false);
                                        return "";
                                    }
                                    if (InputSN.IsChecked)
                                    {
                                        txtInput2.Text = _result1.Data["ssn1"]?.ToString() ?? "";
                                    }
                                    else
                                    {
                                        if (_result1.Data["ssn1"].ToString() != txtInput2.Text)
                                        {
                                            showMessage("00313", "00313", false);
                                            return "";
                                        }
                                    }
                                }
                            }
                            sql = "SELECT SFISM4.R_INVENTEL_DATA_T.*,TO_CHAR(IN_STATION_TIME,'MMDDYYYY') AS PRINT_TIME FROM  SFISM4.R_INVENTEL_DATA_T where MAC = '" + txtInput2.Text + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data != null && _result1.Data["ssn"].ToString() != "")
                            {
                                PRINTDATE = _result1.Data["print_time"]?.ToString() ?? "";
                                MAC1 = _result1.Data["mac"]?.ToString() ?? "";
                                WEP1 = _result1.Data["wepkey"]?.ToString() ?? "";
                                SN1 = _result1.Data["ssn"]?.ToString() ?? "";
                                H235 = _result1.Data["h235"]?.ToString() ?? "";
                                CHECKSUM = _result1.Data["checksum"]?.ToString() ?? "";
                                PRINTMODELFLAG = _result1.Data["model_flag"]?.ToString() ?? "";
                                TEMP = WEP1.Substring(0, 4);
                                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("G74") == -1)
                                {
                                    TEMP = chenge16TO10(TEMP).ToString();
                                }
                                if (TEMP.Length >= 5)
                                {
                                    PING = TEMP.Substring(TEMP.Length - 4, 4);
                                }
                                if (TEMP.Length < 4)
                                {
                                    if (TEMP.Length == 3)
                                    {
                                        PING = "0" + TEMP;
                                    }
                                    if (TEMP.Length == 2)
                                    {
                                        PING = "00" + TEMP;
                                    }
                                    if (TEMP.Length == 1)
                                    {
                                        PING = "000" + TEMP;
                                    }
                                }
                                if (TEMP.Length == 4)
                                {
                                    PING = TEMP;
                                }
                                if (MAC1.Length != 12)
                                {
                                    showMessage("40110", "40110", false);
                                    return "";
                                }
                                if (WEP1 == "")
                                {
                                    showMessage("00314", "00314", false);
                                    return "";
                                }
                                if (SN1 == "")
                                {
                                    showMessage("00315", "00315", false);
                                    return "";
                                }
                                if (H235 == "")
                                {
                                    showMessage("00317", "00317", false);
                                    return "";
                                }
                                if (PING == "")
                                {
                                    showMessage("00317", "00317", false);
                                    return "";
                                }
                                AddParams("MAC", MAC1);
                                AddParams("WEP", WEP1);
                                AddParams("SSN", SN1);
                                AddParams("PIN", PING);
                                AddParams("H235", H235);
                                AddParams("CHECKSUM", CHECKSUM);
                                AddParams("PRINTDATE", PRINTDATE);
                            }
                            else
                            {
                                sql = "select * from sfism4.r_custsn_t where MAC1='" + txtInput2.Text.Trim() + "' or ssn1='" + txtInput2.Text.Trim() + "'";
                                _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result1.Data != null)
                                {
                                    SSN = _result1.Data["ssn1"]?.ToString() ?? "";
                                    sql = "select * from SFISM4.R_INVENTEL_PRINT_T where ssn='" + SSN + "'";
                                    _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (_result1.Data != null)
                                    {
                                        MAC1 = _result1.Data["mac"]?.ToString() ?? "";
                                        WEP1 = _result1.Data["wepkey"]?.ToString() ?? "";
                                        SN1 = _result1.Data["ssn"]?.ToString() ?? "";
                                        H235 = _result1.Data["h235"]?.ToString() ?? "";
                                        CHECKSUM = _result1.Data["checksum"]?.ToString() ?? "";
                                        WEP2 = _result1.Data["wepkey2"]?.ToString() ?? "";
                                        SSID2 = _result1.Data["ssid2"]?.ToString() ?? "";
                                        PINCODE = _result1.Data["pincode"]?.ToString() ?? "";
                                        PRINTMODELFLAG = _result1.Data["model_flag"]?.ToString() ?? "";

                                        sql = "select * from SFISM4.R_INVENTEL_PRINT_T where ssn='" + SSN + "'";
                                        _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                        if (_result1.Data == null)
                                        {
                                            showMessage("00318", "00318", false);
                                            return "";
                                        }
                                        if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("108") != -1)
                                        {
                                            if (_result1.Data["customer_sn"].ToString() == "" || _result1.Data["checksum"].ToString() == "")
                                            {
                                                showMessage("00651", "00651", false);
                                                return "";
                                            }
                                        }
                                        Customer_sn = _result1.Data["customer_sn"]?.ToString() ?? "";
                                        CHECKSUM = _result1.Data["checksum"]?.ToString() ?? "";

                                        if (MAC1 == "")
                                        {
                                            showMessage("00319", "00319", false);
                                            return "";
                                        }
                                        if (WEP1 == "")
                                        {
                                            showMessage("00320", "00320", false);
                                            return "";
                                        }
                                        if (SN1 == "")
                                        {
                                            showMessage("00321", "00321", false);
                                            return "";
                                        }
                                        if (H235 == "")
                                        {
                                            showMessage("00322", "00322", false);
                                            return "";
                                        }
                                        if (model_type.Substring(0, 7) == "U46L121")
                                        {
                                            if (SSID2 == "")
                                            {
                                                showMessage("00323", "00323", false);
                                                return "";
                                            }
                                        }

                                        AddParams("SSID", MAC1);
                                        AddParams("MAC", prt_mac);
                                        AddParams("SSN", SN1);
                                        AddParams("CHK", H235);
                                        AddParams("CHECKSUM", CHECKSUM);
                                        AddParams("WEP2", WEP2);
                                        AddParams("SSID2", SSID2);
                                        AddParams("PINCODE", PINCODE);
                                        AddParams("CUSTOMER_SN", Customer_sn);
                                        AddParams("CHECKSUM", CHECKSUM);
                                        AddParams("SSID", MAC1);

                                        if (PRINTMODELFLAG == "W")
                                        {
                                            WEP2 = _result1.Data["wepkey2"]?.ToString() ?? "";
                                            SSID2 = _result1.Data["ssid2"]?.ToString() ?? "";
                                            if (WEP2 == "")
                                            {
                                                showMessage("00325", "00325", false);
                                                return "";
                                            }
                                            if (SSID2 == "")
                                            {
                                                showMessage("00324", "00324", false);
                                                return "";
                                            }
                                            AddParams("WEP2", WEP2);
                                            AddParams("SSID2", SSID2);
                                        }
                                    }
                                    else
                                    {
                                        showMessage("00318", "00318", false);
                                        return "";
                                    }
                                }
                                else
                                {
                                    showMessage("00326", "00326", false);
                                    return "";
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("071") != -1)
                        {
                            sql = "SELECT PO_NO,Mo_Number FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='" + txtInput1.Text + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data == null)
                            {
                                showMessage("40001", "40001", false);
                                return "";
                            }
                            else
                            {
                                s071_PONO = _result1.Data["po_no"]?.ToString() ?? "";
                                s071_MONUmber = _result1.Data["mo_number"]?.ToString() ?? "";
                                if (s071_PONO != "" && s071_PONO != "N/A" && s071_PONO != txtInput2.Text)
                                {
                                    showMessage("00327", "00327", false);
                                    return "";
                                }
                                if (txtInput2.Text.Length != 7)
                                {
                                    showMessage("40005", "40005", false);
                                    return "";
                                }
                                else
                                {
                                    var regexItem = new Regex("^[0-9]*$");
                                    if (!regexItem.IsMatch(txtInput2.Text))
                                    {
                                        showMessage("40006", "40006", false);
                                        return "";
                                    }
                                }

                                sql = "Select * From SFISM4.R107 Where Serial_Number<>'" + txtInput1.Text + "' AND PO_NO='" + txtInput1.Text + "' AND Mo_Number='" + s071_MONUmber + "'";
                                _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result1.Data != null)
                                {
                                    showMessage("00328", "00328", false);
                                    return "";
                                }
                                sql = "Update SFISM4.R107 Set Po_No='" + txtInput2.Text + "' where Serial_Number='" + txtInput1.Text + "'";
                                var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                {
                                    CommandText = sql,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (query_update.Result.ToString() != "OK")
                                {
                                    showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                                    return null;
                                }

                                sSN_Barcode = "S01" + txtInput2.Text;
                                AddParams("SNBARCODE", sSN_Barcode);
                                sSN_Text = "S01-" + txtInput2.Text + "-C";
                                AddParams("SNTEXT", sSN_Text); ;
                                AddParams("PONO_SN", txtInput2.Text);
                                result = "OK"; ;
                            }

                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("079") != -1)
                        {
                            if (!await findMacID(SN))
                            {
                                showMessage("MAC not found!", "Không tìm thấy MAC", true);
                                return "";
                            }
                            else
                            {
                                prt_mac = macID;
                                sql = "select * from sfism4.r_inventel_data_t where mo_number='" + My_MoNumber + "' and SSN='" + txtInput1.Text + "' and MAC='" + prt_mac + "'";
                                _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (_result1.Data != null)
                                {
                                    s_079wepkey = _result1.Data["wepkey"]?.ToString() ?? "";
                                    AddParams("SN", txtInput1.Text);
                                    AddParams("MAC", prt_mac);
                                    AddParams("SKEY", s_079wepkey);
                                }
                                else
                                {
                                    showMessage("WEPKEY not found!", "Không tìm thấy WEPKEY", true);
                                    return "";
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("016") != -1 || model_type.IndexOf("032") != -1))
                        {
                            if (!await findMacID(SN))
                            {
                                showMessage("00310", "00310", false);
                                return "";
                            }
                            else
                            {
                                prt_mac = macID;
                                if (prt_mac != txtInput2.Text)
                                {
                                    showMessage("00329", "00329", false);
                                    return "";
                                }
                            }
                            sql = "SELECT *  FROM  SFISM4.R_INVENTEL_DATA_T where MAC = '" + txtInput2.Text + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data != null)
                            {
                                WEP1 = _result1.Data["wepkey"]?.ToString() ?? "";
                                MAC1 = _result1.Data["mac"]?.ToString() ?? "";
                                MAC2 = await add1tomacid(MAC1);
                                MAC3 = await add1tomacid(MAC2);
                                SSN1 = _result1.Data["ssn"]?.ToString() ?? "";
                                if (WEP1 == "" || MAC1 == "" || SSN1 == "")
                                {
                                    showMessage("00314", "00314", false);
                                    return "";
                                }
                                AddParams("PinCode", WEP1);
                                AddParams("MAC1", MAC1);
                                AddParams("MAC2", MAC2);
                                AddParams("MAC3", MAC3);
                                AddParams("SSN1", SSN1);
                            }
                            else
                            {
                                showMessage("00314", "00314", false);
                                return "";
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("051") != -1)
                        {
                            sql = "SELECT *  FROM  SFISM4.R_WIP_TRACKING_T where SHIPPING_SN = '" + SN + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data == null)
                            {
                                showMessage("00330", "00330", false);
                                return "";
                            }
                            else
                            {
                                SN1 = _result1.Data["serial_number"]?.ToString() ?? "";
                            }
                            if (!await findMacID(SN1))
                            {
                                showMessage("00310", "00310", false);
                                return "";
                            }
                            else
                            {
                                prt_mac = macID;
                                if (txtInput2.Text.Substring(4, 1) == ".")
                                {
                                    txtInput2.Text = txtInput2.Text.Substring(0, 4) + txtInput2.Text.Substring(5, 4) + txtInput2.Text.Substring(10, 4);
                                }
                                if (prt_mac != txtInput2.Text)
                                {
                                    showMessage("00329", "00329", false);
                                    return "";
                                }
                                if (SN.Length != 17)
                                {
                                    showMessage("40005", "40005", false);
                                    return "";
                                }
                                sDay = SN.Substring(2, 2);
                                ssDay = SN.Substring(10, 1);
                                if (ssDay == "A")
                                {
                                    ssDay = "20" + sDay + ".10";
                                }
                                else if (ssDay == "B")
                                {
                                    ssDay = "20" + sDay + ".11";
                                }
                                else if (ssDay == "C")
                                {
                                    ssDay = "20" + sDay + ".12";
                                }
                                else
                                {
                                    ssDay = "20" + sDay + ".0" + ssDay;
                                }
                                AddParams("SN", SN1);
                                AddParams("SSN1", SN);
                                AddParams("MAC1", prt_mac);
                                AddParams("MV", ssDay);
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("045") != -1)
                        {
                            if (!await findMacID(SN))
                            {
                                showMessage("00310", "00310", false);
                                return "";
                            }
                            else
                            {
                                prt_mac = macID;
                                if (prt_mac != txtInput2.Text)
                                {
                                    showMessage("00331", "00331", false);
                                    return "";
                                }
                            }
                            MAC2 = "";
                            sql = "SELECT *  FROM  SFISM4.R_CUSTSN_T where Serial_Number = '" + SN + "' AND Mac1='" + prt_mac + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data == null)
                            {
                                showMessage("00332", "00332", false);
                                return "";
                            }
                            else
                            {
                                MAC2 = _result1.Data["mac2"]?.ToString() ?? "";
                            }
                            AddParams("SN", SN);
                            AddParams("MAC1", prt_mac);
                            AddParams("MAC2", MAC2);
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("047") != -1)
                        {
                            if (!await findMacID(SN))
                            {
                                showMessage("00310", "00310", false);
                                return "";
                            }
                            prt_mac = macID;
                            MAC2 = "";
                            sql = "SELECT *  FROM  SFISM4.R_CUSTSN_T where Serial_Number = '" + SN + "' AND Mac1='" + prt_mac + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data == null)
                            {
                                showMessage("00332", "00332", false);
                                return "";
                            }
                            else
                            {
                                MAC2 = _result1.Data["mac2"]?.ToString() ?? "";
                                MAC3 = _result1.Data["mac3"]?.ToString() ?? "";
                            }
                            if (MAC2 == "")
                            {
                                showMessage("00332", "00332", false);
                                return "";
                            }
                            if (MAC3 == "")
                            {
                                showMessage("00333", "00333", false);
                                return "";
                            }
                            AddParams("SN", SN);
                            AddParams("MAC1", prt_mac);
                            AddParams("MAC2", MAC2);
                            AddParams("MAC3", MAC3);
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("J") != -1)
                        {
                            if (MACID_PREFIX != txtInput2.Text.Substring(0, 6) || txtInput2.Text.Length != 12)
                            {
                                showMessage("00334", "00334", false);
                                return "";
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("U") != -1)
                        {
                            if (NetgearRule(txtInput2.Text))
                            {
                                showMessage("00335", "00335", false);
                                txtInput1.SelectAll();
                                txtInput1.Focus();
                                return "";
                            }
                            if (txtInput2.Text.Length != Cust_sn_len)
                            {
                                showMessage("40005", "40005", false);
                                return "";
                            }
                            if (txtInput2.Text.Substring(0, (Cust_sn_Prefix + Cust_sn_prefix2).Length) != Cust_sn_Prefix + Cust_sn_prefix2)
                            {
                                showMessage("00048", "00048", false);
                                return "";
                            }
                            if (txtInput2.Text.Substring(Cust_sn_len - Cust_sn_postfix.Length, Cust_sn_postfix.Length) != Cust_sn_postfix)
                            {
                                showMessage("00222", "00222", false);
                                return "";
                            }
                            for (int j = Cust_sn_postfix.Length; j < Cust_sn_len - Cust_sn_postfix.Length; j++)
                            {
                                if ((CUSTSN_STRING.IndexOf(txtInput2.Text.Substring(j, 1))) == -1)
                                {
                                    showMessage("00032", "00032", false);
                                    return "";
                                }
                            }
                            if (PoNo_String != "" && PoNo_String != "N/A")
                            {
                                if (PoNo_String != txtInput2.Text && CHECK_PONO.IsChecked)
                                {
                                    if (checkssnrule1 == "SSN1")
                                    {
                                        showMessage("00309", "00309", false);
                                        return "";
                                    }
                                    else
                                    {
                                        showMessage("00336", "00336", false);
                                        return "";
                                    }
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("Z") != -1)
                        {
                            sql = "select * from sfisM4.R_WIP_TRACKING_T where SERIAL_NUMBER = '" + txtInput1.Text.ToUpper().Trim() + "' and PO_NO = '" + txtInput2.Text.ToUpper().Trim() + "'";
                            _result1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (_result1.Data == null)
                            {
                                showMessage("00334", "00334", false);
                                txtInput2.SelectAll();
                                txtInput2.Focus();
                                return "";
                            }
                            if (txtInput2.Text.Length != Cust_sn_len)
                            {
                                showMessage("40005", "40005", false);
                                return "";
                            }
                            if (txtInput2.Text.Substring(0, (Cust_sn_Prefix + Cust_sn_prefix2).Length) != Cust_sn_Prefix + Cust_sn_prefix2)
                            {
                                showMessage("00048", "00048", false);
                                return "";
                            }
                            if (txtInput2.Text.Substring(Cust_sn_len - Cust_sn_postfix.Length, Cust_sn_postfix.Length) != Cust_sn_postfix)
                            {
                                showMessage("00222", "00222", false);
                                return "";
                            }
                            for (int j = Cust_sn_postfix.Length; j < Cust_sn_len - Cust_sn_postfix.Length; j++)
                            {
                                if ((CUSTSN_STRING.IndexOf(txtInput2.Text.Substring(j, 1))) == -1)
                                {
                                    showMessage("00032", "00032", false);
                                    return "";
                                }
                            }
                            if (PoNo_String != "" && PoNo_String != "N/A")
                            {
                                if (PoNo_String != txtInput2.Text && CHECK_PONO.IsChecked)
                                {
                                    showMessage("00309", "00309", false);
                                    return "";
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("005") != -1)
                        {
                            if (!await findMacID(txtInput1.Text.ToUpper()))
                            {
                                showMessage("00310", "00310", false);
                                return "";
                            }
                            prt_mac = macID;
                            if (prt_mac != txtInput3.Text.ToUpper())
                            {
                                showMessage("00329", "00329", false);
                                return "";
                            }
                            if (paramData.Length != Cust_sn_len && !BB.IsChecked)
                            {
                                showMessage("40005", "40005", false);
                                return "";
                            }
                            if (paramData.Substring(0, (Cust_sn_postfix + Cust_sn_prefix2).Length) != Cust_sn_postfix + Cust_sn_prefix2 && !BB.IsChecked)
                            {
                                showMessage("00048", "00048", false);
                                return "";
                            }
                            if (paramData.Substring(Cust_sn_len - Cust_sn_postfix.Length, Cust_sn_postfix.Length) != Cust_sn_postfix)
                            {
                                showMessage("00222", "00222", false);
                                return "";
                            }
                            for (int j = Cust_sn_postfix.Length; j < Cust_sn_len - Cust_sn_postfix.Length; j++)
                            {
                                if ((CUSTSN_STRING.IndexOf(txtInput2.Text.Substring(j, 1))) == -1)
                                {
                                    showMessage("00032", "00032", false);
                                    return "";
                                }
                            }
                            if (PoNo_String != "" && PoNo_String != "N/A")
                            {
                                if (PoNo_String != paramData && CHECK_PONO.IsChecked)
                                {
                                    showMessage("00309", "00309", false);
                                    return "";
                                }
                            }
                        }
                        else if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("074") != -1)
                        {
                            if (CHECK_PONO.IsChecked && !noCheckSSN.IsChecked)
                            {
                                if (Cust_sn_len != 99)
                                {
                                    if (paramData.Length != Cust_sn_len && !BB.IsChecked && txtMoNumber.Text.Length == 0)
                                    {
                                        showMessage("40005", "40005", false);
                                        return "";
                                    }
                                }
                            }
                            if (paramData.Substring(0, (Cust_sn_postfix + Cust_sn_prefix2).Length) != Cust_sn_Prefix + Cust_sn_prefix2 && !BB.IsChecked)
                            {
                                showMessage("00048", "00048", false);
                                return "";
                            }
                            if (paramData.Substring(Cust_sn_len - Cust_sn_postfix.Length, Cust_sn_postfix.Length) != Cust_sn_postfix)
                            {
                                showMessage("00222", "00222", false);
                                return "";
                            }
                            for (int j = Cust_sn_postfix.Length; j < paramData.Length; j++)
                            {
                                if (CUSTSN_STRING.IndexOf(paramData.Substring(j, 1)) != -1 && txtMoNumber.Text.Length == 0)
                                {
                                    showMessage("00222", "00222", false);
                                    return "";
                                }
                            }
                            if (PoNo_String != "" && PoNo_String != "N/A")
                            {
                                if (PoNo_String != paramData && CHECK_PONO.IsChecked)
                                {
                                    showMessage("00309", "00309", false);
                                    return "";
                                }
                            }
                        }
                    }
                }
                M_iCartonCapacity = await FGetCartonCapacityByModelVer(M_MODEL_NAME, M_VERSION_CODE);
                if (M_iCartonCapacity == 0)
                {
                    showMessage("00111", "00111", false);
                    return "";
                }
            }
            if (await dm2.checkModelpo(M_MODEL_NAME))
            {
                pubPo_no = await dm2.getPOBySn(SN);
                if (pubPo_no == "0")
                {
                    showMessage("40001", "40001", false);
                    return "";
                }
                else if (pubPo_no == "1")
                {
                    showMessage("00468", "00468", false);
                    return "";
                }
                else
                {
                    if (!await dm2.checkModelpo(M_MODEL_NAME))
                    {
                        if (!await dm2.chkPOValid(pubPo_no, SN, M_MODEL_NAME))
                        {
                            showMessage("PO not valid,please call IT or PM", "PO not valid,please call IT or PM", true);
                            return "";
                        }
                        AddParams("PO", pubPo_no);
                        await dm2.UpdateSninPO(SN, pubPo_no);
                    }
                }
            }
            //if(frmLabelQTY.)
            /*IF LabelQtyForm.PalletQty.Value = 10 THEN
              BEGIN
                Pack2_Ini:=TIniFile.Create('C:\PACKING\PACKING.Ini');
                LabelQtyForm.SNQty.value:=Pack2_Ini.ReadInteger('SLabelQTY','Default',10);
                LabelQtyForm.CartonQty.value:=Pack2_Ini.ReadInteger('CLabelQTY','Default',1);
                LabelQtyForm.PalletQty.value:=Pack2_Ini.ReadInteger('PLabelQTY','Default',15);
                Pack2_Ini.Free;
              END;
              LabelQtyForm.PalletQty.value:=M_iCartonCapacity;*/

            return result;
        }
        public async Task<int> FGetCartonCapacityByModelVer(string i_model_name, string i_version)
        {
            sql = "select c.* from sfis1.c_pack_param_t c where c.model_name = '" + i_model_name + "' and c.version_code = '" + i_version + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                return -1;
            }
            else
            {
                if (_result.Data["tray_qty"].ToString() == "" || _result.Data["carton_qty"].ToString() == "")
                {
                    return -1;
                }
                else
                {
                    return Int32.Parse(_result.Data["tray_qty"].ToString()) * Int32.Parse(_result.Data["carton_qty"].ToString());
                }
            }

        }
        private void InputSN_Checked(object sender, RoutedEventArgs e)
        {
            mText.Content = "SN/SSN:";
            InputSSN.IsChecked = false;
            ScanQR.IsChecked = false;
            Input2MAC.IsChecked = false;
            InputASN.IsChecked = false;

            txtInput1.IsEnabled = true;
            txtInput2.IsEnabled = true;
            txtInput3.IsEnabled = false;
        }

        private void InputSSN_Checked(object sender, RoutedEventArgs e)
        {
            mText.Content = "SN/SSN:";
            InputSN.IsChecked = false;
            ScanQR.IsChecked = false;
            Input2MAC.IsChecked = false;
            InputASN.IsChecked = false;

            txtInput1.IsEnabled = true;
            txtInput2.IsEnabled = false;
            txtInput3.IsEnabled = false;
        }
        private void InputSNT_Checked(object sender, RoutedEventArgs e)
        {
            
        }
        private void ScanQR_Checked(object sender, RoutedEventArgs e)
        {
            mText.Content = "SN/SSN:";
            InputSN.IsChecked = false;
            InputSSN.IsChecked = false;
            Input2MAC.IsChecked = false;
            InputASN.IsChecked = false;

            txtInput1.IsEnabled = true;
            txtInput2.IsEnabled = false;
            txtInput3.IsEnabled = false;
        }
        private void Input2MAC_Checked(object sender, RoutedEventArgs e)
        {
            mText.Content = "SN/SSN:";
            InputSN.IsChecked = false;
            InputSSN.IsChecked = false;
            ScanQR.IsChecked = false;
            InputASN.IsChecked = false;

            txtInput1.IsEnabled = true;
            txtInput2.IsEnabled = true;
            txtInput3.IsEnabled = false;
        }
        private void InputASN_Checked(object sender, RoutedEventArgs e)
        {
            mText.Content = "SN/SSN:";
            InputSN.IsChecked = false;
            InputSSN.IsChecked = false;
            ScanQR.IsChecked = false;
            Input2MAC.IsChecked = false;

            txtInput1.IsEnabled = true;
            txtInput2.IsEnabled = false;
            txtInput3.IsEnabled = false;
        }

        private void InputBoxID_Checked(object sender, RoutedEventArgs e)
        {
            mText.Content = "Reel/Tray/Box:";
            InputSN.IsChecked = false;
            InputSSN.IsChecked = false;
            ScanQR.IsChecked = false;
            Input2MAC.IsChecked = false;
            InputASN.IsChecked = false;

            txtInput1.IsEnabled = true;
            txtInput2.IsEnabled = false;
            txtInput3.IsEnabled = false;
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                serialPort1.Open();
                lbl2.Text = COM_PORT + " open!";
                lbl2.Foreground = new SolidColorBrush(Colors.White);
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            }
            catch
            {
                lbl2.Text = COM_PORT + " close!";
                lbl2.Foreground = new SolidColorBrush(Colors.Red);
            }
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            UI_Route.Content = "Route: " + ini.IniReadValue("LOTREPRINT", "CHECK_ROUTENAME");
            killprocess();
            try
            {
                labApp = new LabelManager2.Application();
            }
            catch(Exception ex)
            {
                showMessage("Chưa cài codesoft", ex.Message, true);
            }

            try
            {
                string sql = "select * from sfis1.c_parameter_ini where prg_name='HH_PrintLabel' and vr_class='ROKU' and vr_value='Y' ";
                var _result_check_param = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (_result_check_param.Data.Count() != 0)
                {
                    check_roku_info = true;
                }
                else
                {
                    check_roku_info = false;
                }

            }
            catch (Exception EX)
            {
                MessageBox.Show("" + EX + "");

            }

        }

        private void btnDownSedit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSeditLabelQty.Text))
            {
                txtSeditLabelQty.Text = "1";
            }
            int valueCheck = Int32.Parse(txtSeditLabelQty.Text);
            if (valueCheck > 1)
                txtSeditLabelQty.Text = (valueCheck - 1).ToString();
        }

        

        private void btnUpSedit_Click(object sender, RoutedEventArgs e)
        {
            showMessage("No privilege to change print qty","Không có quyền hạn thay đổi số lượng in",true);
            return;
            if (string.IsNullOrEmpty(txtSeditLabelQty.Text))
            {
                txtSeditLabelQty.Text = "0";
            }
            txtSeditLabelQty.Text = (Int32.Parse(txtSeditLabelQty.Text) + 1).ToString();
        }

        private void btnUpSeditCount_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSeditfCount.Text))
            {
                txtSeditfCount.Text = "0";
            }
            txtSeditfCount.Text = (Int32.Parse(txtSeditfCount.Text) + 1).ToString();
        }

        private void btnDownSeditCount_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSeditfCount.Text))
            {
                txtSeditfCount.Text = "1";
            }
            int valueCheck = Int32.Parse(txtSeditfCount.Text);
            if (valueCheck > 1)
                txtSeditfCount.Text = (valueCheck - 1).ToString();
        }

        private void btnUpSedprtnow_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSedprtnow.Text))
            {
                txtSedprtnow.Text = "0";
            }
            txtSedprtnow.Text = (Int32.Parse(txtSedprtnow.Text) + 1).ToString();
        }

        

        private void btnDownSedprtnow_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSedprtnow.Text))
            {
                txtSedprtnow.Text = "0";
            }
            int valueCheck = Int32.Parse(txtSedprtnow.Text);
            if (valueCheck > 0)
                txtSedprtnow.Text = (valueCheck - 1).ToString();
        }

        

        void serialPort1_DataReceived(object obj, SerialDataReceivedEventArgs e)
        {
             data_com = serialPort1.ReadExisting().Replace("\r\n", "");
            this.Dispatcher.Invoke(new SetTextDeleg(si_DataReceived), new object[] { data_com });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string dir = "C:/NIC/LOT_Reprint";
                string[] listlink = Directory.GetFiles(dir, "*.lab");
                foreach (string link in listlink)
                {
                    File.Delete(link);
                }

                labApp.Quit();
            }
            catch
            { }
            killprocess();
        }

        private void autoMation_Click(object sender, RoutedEventArgs e)
        {
            if (autoMation.IsChecked)
            {
                string COM_PORT3 = GetSet.COMSEND;
                serialPort3.BaudRate = 9600;
                serialPort3.Parity = Parity.None;
                serialPort3.DataBits = 8;
                serialPort3.StopBits = StopBits.One;
                serialPort3.PortName = COM_PORT3;
                serialPort3.Handshake = Handshake.None;
                try
                {
                    serialPort3.Open();
                    frmCheckAcce checkAcce = new frmCheckAcce();
                    checkAcce.sfcClient = sfcClient;
                    checkAcce.serialPort3 = serialPort3;
                    checkAcce.ShowDialog();
                }
                catch
                {
                    showMessage("Can't open COM 3", "Không thể mở cổng COM 3", true);
                }
            }
            else
            {
                serialPort3.Close();
            }
        }

        private delegate void SetTextDeleg(string text);
        private void si_DataReceived(string data)
        {
            txtInput1.Text = data.Trim();
            btnOK_Click(new object(), new RoutedEventArgs());
        }

        private void testComport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                serialPort1.Open();
                lbl2.Text = COM_PORT + " open!";
                lbl2.Foreground = new SolidColorBrush(Colors.White);
            }
            catch
            {
                lbl2.Text = COM_PORT + " close!";
                lbl2.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        public bool NetgearRule(string input)
        {
            int I, Sub1, SUBTOT, ChkPos;
            string Str1, ChkDig, tmpStr, tmp_str1;
            if (input.Length != 13)
            {
                showMessage("20101", "20101", false);
                return false;
            }
            var regexItem = new Regex("^[A-Z0-9]*$");
            if (!regexItem.IsMatch(input))
            {
                showMessage("00356", "00356", false);
                return false;
            }
            Str1 = input.Substring(0, 7) + input.Substring(8, 5);
            tmpStr = "123456789ABCDEFGHJKLMNPRSTUVWXY0";
            ChkPos = 0;
            SUBTOT = 0;
            for (int i = 0; i < Str1.Length; i++)
            {
                if (Str1.Substring(i, 1) != "0")
                {
                    Sub1 = (tmpStr.IndexOf(Str1.Substring(i, 1)) + 1) * (Str1.Length - i);
                    SUBTOT = SUBTOT + Sub1;
                }
            }
            ChkPos = (SUBTOT % 32);
            if (ChkPos == 0)
            {
                ChkPos = 32;
            }
            ChkDig = tmpStr.Substring(ChkPos - 1, 1);
            if (input.Substring(7, 1) == ChkDig)
            {
                return false;
            }
            return true;
        }
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        public string GetSSID(string input)
        {
            string s16 = "0123456789ABCDEF";
            string a, b, c, d, e;
            a = ((s16.IndexOf(input.Substring(0, 1)) * 16 + s16.IndexOf(input.Substring(1, 1))) % 10).ToString().Trim();
            b = ((s16.IndexOf(input.Substring(2, 1)) * 16 + s16.IndexOf(input.Substring(3, 1))) % 10).ToString().Trim();
            c = ((s16.IndexOf(input.Substring(4, 1)) * 16 + s16.IndexOf(input.Substring(5, 1))) % 10).ToString().Trim();
            d = ((s16.IndexOf(input.Substring(6, 1)) * 16 + s16.IndexOf(input.Substring(7, 1))) % 10).ToString().Trim();
            e = ((s16.IndexOf(input.Substring(8, 1)) * 16 + s16.IndexOf(input.Substring(9, 1))) % 10).ToString().Trim();
            return a + b + c + d + e;
        }
        public string GetPassword(string input)
        {
            string s16 = "0123456789ABCDEF";
            string s26 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string a, b, c, d, e, f, g, h;
            a = s26.Substring((s16.IndexOf(input.Substring(0, 1)) * 16 + s16.IndexOf(input.Substring(1, 1))) % 26, 1);
            b = s26.Substring((s16.IndexOf(input.Substring(2, 1)) * 16 + s16.IndexOf(input.Substring(3, 1))) % 26, 1);
            c = s26.Substring((s16.IndexOf(input.Substring(4, 1)) * 16 + s16.IndexOf(input.Substring(5, 1))) % 26, 1);
            d = s26.Substring((s16.IndexOf(input.Substring(6, 1)) * 16 + s16.IndexOf(input.Substring(7, 1))) % 26, 1);
            e = s26.Substring((s16.IndexOf(input.Substring(8, 1)) * 16 + s16.IndexOf(input.Substring(9, 1))) % 26, 1);
            f = s26.Substring((s16.IndexOf(input.Substring(10, 1)) * 16 + s16.IndexOf(input.Substring(11, 1))) % 26, 1);
            g = s26.Substring((s16.IndexOf(input.Substring(12, 1)) * 16 + s16.IndexOf(input.Substring(13, 1))) % 26, 1);
            h = s26.Substring((s16.IndexOf(input.Substring(14, 1)) * 16 + s16.IndexOf(input.Substring(15, 1))) % 26, 1);
            return a + b + c + d + e + f + g + h;
        }
        public string GetChannel(string input)
        {
            string s16 = "0123456789ABCDEF";
            string tmpCH = "";
            int a = ((s16.IndexOf(input.Substring(0, 1)) * 16) + s16.IndexOf(input.Substring(1, 1))) % 3;
            switch (a)
            {
                case 0:
                    tmpCH = "1";
                    break;
                case 1:
                    tmpCH = "6";
                    break;
                case 2:
                    tmpCH = "11";
                    break;
            }
            return tmpCH;
        }
        //CHENGE16TO10
        public int chenge16TO10(string data)
        {
            int tmpj = 0;
            var regexItem = new Regex("^[A-F0-9]*$");
            if (!regexItem.IsMatch(data.Substring(data.Length - 1, 1)))
            {
                showMessage("00032", "00032", false);
                return tmpj;
            }

            regexItem = new Regex("^[A-F0-9]*$");
            if (!regexItem.IsMatch(data))
            {
                showMessage("00032", "00032", false);
                return tmpj;
            }
            return tmpj;
            //thieu chuc nang
        }
        public async Task saveReprintRd(string emp_bc, string m_sn,string labCode)
        {
            saveOK = false;
            string tmp_emp = "";
            string sql = "select emp_no from sfis1.c_emp_desc_t where emp_bc ='" + emp_bc + "' or emp_pass ='" + emp_bc + "' and rownum=1";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                tmp_emp = _result.Data["emp_no"]?.ToString() ?? "";
                if (labCode == "CTN")
                {
                    sql = "insert into sfism4.r_system_log_t(emp_no,prg_name,action_type,action_desc) values('" + tmp_emp + "','LotReprint','REPRINT','Carton: " + m_sn + " ,IP: " + GetIPAddress() + ",Label:" + labCode + "')";
                }
                else if(labCode.StartsWith("BOX"))
                {
                    sql = "insert into sfism4.r_system_log_t(emp_no,prg_name,action_type,action_desc) values('" + tmp_emp + "','LotReprint','REPRINT','BoxID: " + m_sn + " ,IP: " + GetIPAddress() + ",Label:" + labCode + "')";
                }
                else 
                {
                    sql = "insert into sfism4.r_system_log_t(emp_no,prg_name,action_type,action_desc) values('" + tmp_emp + "','LotReprint','REPRINT','Serial_Number: " + m_sn + " ,IP: " + GetIPAddress() + ",Label:" + labCode + "')";
                }
                var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (query_update.Result.ToString() != "OK")
                {
                    showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                    return ;
                }
                else
                {
                    saveOK = true;
                }
            }
            else
            {
                showMessage("EMP_BC not found, please check again!", "Không tìm thấy EMP_BC, Kiểm tra lại!" + SourceString, true);
            }
        }
        public async Task<string> gett77n081sequence()
        {
            //string MonthStr = "1234567890AB";
            //string DateStr = "1234567890ABCDEFGHIJKLMNOPQRSTU";
            string tmp, tmpint, tmpPrefixCCarton, TY, TM, TD, tmpBB, MC;
            //int qty;

            tmpBB = txtInput1.Text;
            sql = "select a.UPCEANDATA,to_char(sysdate,'Y') TY,to_char(sysdate,'MM') TM,to_char(sysdate,'DD') TD from sfis1.c_cust_snrule_t  a " +
                " where (model_name,VERSION_CODE) IN " +
                " (SELECT MODEL_NAME,VERSION_CODE FROM SFISM4.R105 WHERE MO_NUMBER IN (SELECT MO_NUMBER FROM SFISM4.R107 WHERE SERIAL_NUMBER='" + tmpBB + "'))";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                tmpPrefixCCarton = _result.Data["upceandata"]?.ToString() ?? "";
                TY = _result.Data["ty"]?.ToString() ?? "";
                TM = _result.Data["tm"]?.ToString() ?? "";
                TD = _result.Data["td"]?.ToString() ?? "";
                tmp = tmpPrefixCCarton + TY + TM + TD;

                sql = "select nvl(max(SSN3),'" + tmp + "000000') MC from sfism4.r_CUSTSN_T where SSN3 like '" + tmp + "%'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    MC = _result.Data["mc"]?.ToString() ?? "";
                    tmpint = MC.Substring(tmp.Length);
                    tmp = tmp + Int32.Parse(tmpint);   //Cần check lại đoạn này TMP:=TMP+SysUtils.FORMAT('%.6D',[strtoint(tmpint)+1]);

                    sql = "update sfism4.r_CUSTSN_t set SSN3='" + tmp + "' where SERIAL_NUMBER='" + tmpBB + "'";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return null;
                    }
                    return tmp;
                }
            }
            return null;
        }

        private void checkRoute_Click(object sender, RoutedEventArgs e)
        {
            frmLogin frmLogin = new frmLogin();
            frmLogin.sfcClient = sfcClient;
            frmLogin.type = "CHECKROUTE";
            frmLogin.ShowDialog();
            if (frmLogin.ok == System.Windows.Forms.DialogResult.OK)
            {
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                UI_Route.Content = "Route: " + ini.IniReadValue("LOTREPRINT", "CHCEK_ROUTENAME");
            }
        }

        public async Task<string> tg799nextcustsn(string data)
        {
            string result = "";
            string s_ver, s_ssn_pre, s_custsn = "", s_date, C_SSN, C_SN = "";
            int i, i_ssn_len;
            sql = "select to_char(sysdate,'MMYY' ) S_DATE from dual";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            s_date = _result.Data["s_date"]?.ToString() ?? "";
            if (data.Length == 12)
            {
                sql = "select serial_number from sfism4.r_wip_keyparts_t where key_part_sn='" + data + "'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    C_SN = _result.Data["serial_number"]?.ToString() ?? "";
                }
            }
            else
            {
                C_SN = data;
            }

            sql = "select version_code from sfism4.r_wip_tracking_t where serial_number='" + C_SN + "'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                M_VERSION_CODE = _result.Data["version_code"]?.ToString() ?? "";
            }

            sql = "select ssn1 from sfism4.r_custsn_t where serial_number='" + C_SN + "'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                c_ssn = _result.Data["ssn1"]?.ToString() ?? "";
            }

            sql = "select * from sfis1.c_cust_snrule_t where version_code='" + M_VERSION_CODE + "' and model_name='" + model_name + "'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                showMessage("Please setup config 19. Call IT!", "Chưa thiết lập config 19. Gọi IT", true);
                result = "Please set config19";
                return result;
            }
            s_ssn_pre = _result.Data["cust_sn_prefix"]?.ToString() ?? "";
            i_ssn_len = Int32.Parse(_result.Data["cust_sn_leng"].ToString());

            if (s_ssn_pre.Length < 12)
            {
                showMessage("ssn prefix length<12!", "ssn prefix length<12", true);
                result = "ssn prefix length<12";
                return result;
            }
            sql = "select max(customer_sn) maxcustsn from sfism4.r_inventel_print_t where customer_sn like '" + s_ssn_pre.Substring(0, 12) + "____" + s_ssn_pre.Substring(16, 1) + "%' and model_flag='195'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                if (_result.Data["maxcustsn"].ToString() == "")
                {
                    if (s_ssn_pre.Substring(18, 1) == "X")
                    {
                        s_custsn = s_ssn_pre.Substring(0, 12) + s_date + s_ssn_pre.Substring(17, 1) + "X0000";
                    }
                    else
                    {
                        s_custsn = s_ssn_pre.Substring(0, 12) + s_date + s_ssn_pre.Substring(17, 1) + "00000";
                    }
                }
                else
                {
                    s_custsn = _result.Data["maxcustsn"]?.ToString() ?? "";
                    if (s_date == s_custsn.Substring(12, 4))
                    {
                        s_custsn = getnextSN(s_custsn, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", 1);
                    }
                    else
                    {
                        if (s_ssn_pre.Substring(17, 1) == "X")
                        {
                            s_custsn = s_ssn_pre.Substring(0, 12) + s_date + s_ssn_pre.Substring(17, 1) + "X0000";
                        }
                        else
                        {
                            s_custsn = s_ssn_pre.Substring(0, 12) + s_date + s_ssn_pre.Substring(17, 1) + "00000";
                        }
                    }
                }
            }
            sql = "select * from sfism4.r_inventel_print_t where ssn = '" + c_ssn + "'";
            _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                showMessage("inventel_print no data!", "inventel_print no data!", true);
                result = "inventel_print no data.";
                return result;
            }
            else
            {
                if (_result.Data["customer_sn"].ToString().Length < 5)
                {
                    sql = "update sfism4.r_inventel_print_t set customer_sn='" + s_custsn + "' where ssn='" + c_ssn + "'";
                    var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_update.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                        return null;
                    }
                }
            }
            result = "OK";
            return result;
        }
        private async Task prepareCtnParam(string sn)
        {
            string table;
            string ctnqty;
            if (Z107.IsChecked == true)
            {
                table = "SFISM4.Z107";
            }
            else
            {
                table = "SFISM4.R107";
            }
            sql = "select count(1) aa from " + table + " where mcarton_no in (select mcarton_no from " + table + " where serial_number='" + sn + "')";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                ctnqty = _result.Data["aa"]?.ToString() ?? "";
                AddParams("CTNQTY", ctnqty);
            }
        }

        private void txtSeditLabelQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            ini.IniWriteValue("CLabelQTY", "Default", txtSeditLabelQty.Text);
        }

        public async void pl_model_print()
        {
            string labname;

            txtInput1.Text = txtInput1.Text.ToUpper();
            txtInput1.Text = txtInput1.Text.ToUpper();

            if (txtInput1.Text == txtInput2.Text)
            {
                showMessage("00360", "00360", false);
                txtInput2.SelectAll();
                txtInput2.Focus();
                return;
            }
            PL_model_check(txtInput1.Text, 1);
            if (err)
            {
                txtInput1.SelectAll();
                txtInput1.Focus();
                return;
            }
            PL_model_check(txtInput2.Text, 2);
            if (err)
            {
                txtInput2.SelectAll();
                txtInput2.Focus();
                return;
            }
            if (!err)
            {
                AddParams("CUST_SSN1", txtInput1.Text);
                AddParams("CUST_SSN2", txtInput2.Text);
                AddParams("MODELDESC", CUST_SSN1);
                if (txtSeditfCount.Text == "1")
                {
                    if (!await P_PrintToCodeSoft("C", model_name + "_BX.LAB"))
                    {
                        return;
                    }
                }
                else
                {
                    if (txtSedprtnow.Text != "0")
                    {
                        labname = model_name + "_BX" + txtSedprtnow.Text + ".Lab";
                        if (!await P_PrintToCodeSoft("C", labname))
                            return;
                    }
                }
            }
            txtInput1.Text = "";
            txtInput2.Text = "";
            txtInput1.Focus();
        }
        //'S','C','P'
        public async Task<bool> P_PrintToCodeSoft(string ParamKind, string paramLabelFile)
        {
            PrintOK = false;
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            //Loc_Num = ini.IniReadValue(ParamKind + "LabelQTY", "Default");
            LabelFileName_ftp = paramLabelFile;

            My_LabelFileName = await labellocation() + @"\" + paramLabelFile;
            if (!File.Exists(My_LabelFileName))
            {
                if (!await downloadlabel(pubftppath, paramLabelFile))
                {
                    showMessage("Download label file fail!", "Tải xuống tệp label lỗi!", true);
                    return false;
                }
            }
            publicfilepath = My_LabelFileName;

            if (!File.Exists(My_LabelFileName))
            {
                showMessage("00092", "00092", false);
                return false;
            }

            if (string.IsNullOrEmpty(G_sLabl_Name))
            {
                G_sLabl_Name = LabelFileName_ftp;
                if (G_sLabl_Name == "")
                {
                    showMessage("00057", "00057", false);
                    return false;
                }
            }

            if (await isCheckLabel())
            {
                if (labApp == null)
                {
                    labApp = new LabelManager2.Application();
                }
                labApp.Documents.Open(My_LabelFileName, false);
                doc = labApp.ActiveDocument;
                bool _chkprint = await CallCodesoftToPrint(LabelFileName_ftp, dtParams);
            }
            else
            {
                if (labApp == null)
                {
                    labApp = new LabelManager2.Application();
                }
                labApp.Documents.Open(My_LabelFileName, false);
                doc = labApp.ActiveDocument;
                bool _chkprint = await CallCodesoftToPrint(LabelFileName_ftp, dtParams);
            }

            if (await checkReprint() && string.IsNullOrEmpty(txtStation.Text))
            {
                if (Z107.IsChecked == false)
                {
                    await updater107(txtInput1.Text);
                }
            }
            PrintOK = true;
            return PrintOK;
        }
        private bool killprocess()
        {
            Process[] processes = Process.GetProcessesByName("lppa");
            if (processes.Length > 0)
            {
                foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                        showMessage("Disconnect codesoft error", "Ngắt kết nối với codesoft lỗi", true);
                        return false;
                    }
                }
            }
            foreach (string sFile in System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "*.LAB"))
            {
                try
                {
                    File.Delete(sFile);
                }
                catch (Exception ex_e)
                {
                    //showMessage("Cannot delete label file because: " + ex_e, "Cannot delete label file because: " + ex_e, true);
                    return false;
                }
            }
            foreach (string sFile in System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "*.bak"))
            {
                try
                {
                    File.Delete(sFile);
                }
                catch (Exception ex_e)
                {
                    //showMessage("Cannot delete label file because: " + ex_e, "Cannot delete label file because: " + ex_e, true);
                    return false;
                }
            }
            return true;
        }
        public async Task updater107(string sn)
        {
            string sql_string = "update sfism4.r_wip_tracking_t set work_flag='" + txtSeditfCount.Text + "' where serial_number ='" + sn + "'";
            var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = sql_string,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_update.Result.ToString() != "OK")
            {
                showMessage("Exception: " + query_update.Message.ToString() + "\nsql: " + sql, "Exception: " + query_update.Message.ToString() + "\nsql: " + sql, true);
                return;
            }
        }
        public async Task<bool> CallCodesoftToPrint(string LabelName, DataTable dtParams)
        {
            string _param_Name = string.Empty;

            try
            {
                LabelTracking _lbt = new LabelTracking();
                _lbt.sfcHttpClient = sfcClient;
                string MD5Label = await _lbt.getWipLabelMD5(LabelName, 0);
                sql = "";
                int d = 0;
                //Set param for label file
                for (int i = 1; i < doc.Variables.FormVariables.Count+1; i++)
                {
                    doc.Variables.FormVariables.Item(i).Value = "";
                }
                doc.PrintDocument(0);
                if (!PrintTest.IsChecked)
                {
                    var _param_Name1 = "";
                    dtParams.AsEnumerable().Select(r => r).AsParallel().ForAll(t =>
                    {
                        try
                        {
                            _param_Name1 = t["Name"]?.ToString() ?? "";
                            doc.Variables.FormVariables.Item(_param_Name1).CounterUse = 0;
                            doc.Variables.FormVariables.Item(_param_Name1).Value = t["Value"]?.ToString() ?? "";
                            sql += "  INSERT INTO SFISM4.R_FQA_CHECKLABEL_T (LABEL_NAME,GROUP_NAME,PASS_DATE,UPD_DATE,EMP_NO,PASS_FLAG,CHECKSUM,MEMO) " +
                                                      " VALUES ('" + LabelName + "','H' || to_char(sysdate,'YYYYMMDDHH24MISS'),sysdate,null,'" + _param_Name1 + "','0','" + MD5Label + "','" + (t["Value"]?.ToString() ?? "") + "') ; \n";
                        d++;
                        }
                        catch (Exception ex)
                        {

                        }
                        
                    });
                    //foreach (DataRow dr in dtParams.Rows)
                    //{
                    //    try
                    //    {
                    //        _param_Name1 = dr[0]?.ToString();
                    //        doc.Variables.FormVariables.Item(_param_Name1).CounterUse = 0;
                    //        doc.Variables.FormVariables.Item(_param_Name1).Value = dr[1]?.ToString();
                    //    }
                    //    catch (Exception ex)
                    //    {

                    //    }
                    //    sql += "  INSERT INTO SFISM4.R_FQA_CHECKLABEL_T (LABEL_NAME,GROUP_NAME,PASS_DATE,UPD_DATE,EMP_NO,PASS_FLAG,CHECKSUM,MEMO) " +
                    //                                  " VALUES ('" + LabelName + "','H' || to_char(sysdate,'YYYYMMDDHH24MISS'),sysdate,null,'" + _param_Name1 + "','0','" + MD5Label + "','" + (dr[1]?.ToString()) + "') ; \n";
                    //    d++;
                    //}

                    if (d == 0)
                    {
                        showMessage("No param data for label file", "Không có dữ liệu biến cho file label", true);
                        return false;
                    }
                    sql = "BEGIN \n" + sql + "END;";
                    var query_insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query_insert.Result.ToString() != "OK")
                    {
                        showMessage("Exception: " + query_insert.Message.ToString() + "\nsql: " + sql, "Exception: " + query_insert.Message.ToString() + "\nsql: " + sql, true);
                        return false;
                    }
                }
                else
                {
                    dtParams.AsEnumerable().Select(r => r).AsParallel().ForAll(t =>
                    {
                        try
                        {
                            var _param_Name1 = t["Name"]?.ToString() ?? "";
                            doc.Variables.FormVariables.Item(_param_Name1).CounterUse = 0;
                            doc.Variables.FormVariables.Item(_param_Name1).Value = t["Value"]?.ToString() ?? "";
                            d++;
                        }
                        catch (Exception ex)
                        {

                        }

                    });
                    if (d == 0)
                    {
                        showMessage("No param data for label file", "Không có dữ liệu biến cho file label", true);
                        return false;
                    }
                }

                //Call label tracking
                if (!rePrint.IsChecked && !PrintTest.IsChecked)
                {
                    try
                    {
                        if (!await _lbt.doLabelTracking(LabelName, 0, labApp, doc))
                        {
                            showMessage(_lbt.LastError, _lbt.LastError, true);
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        showMessage("Check label tracking have exception: " + ex.Message, "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message, true);
                        return false;
                    }
                }
                int printQTY = Int32.Parse(txtSeditLabelQty.Text);
                if (printQTY <= 0)
                    printQTY = 1;
                for (int i = 0; i < printQTY; i++)
                {
                    doc.PrintDocument(1);
                }
            }
            catch (Exception ex)
            {
                showMessage("Have exceptions when print label: " + ex.Message, "Phát sinh lỗi khi in label: " + ex.Message, true);
                return false;
            }
            finally
            {
                doc = null;
            }
            return true;
        }
        public async Task<bool> isCheckLabel()
        {
            sql = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='Z_CHECKSUM_LH'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                ISPRINTCOUNT = Int32.Parse(_result.Data["model_type"].ToString());
                ftphost = _result.Data["model_serial"]?.ToString() ?? "";
                if (_result.Data["checkflag"].ToString() == "Y")
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> downloadlabel(string ftplabelpath, string labelname)
        {
            string ftpaddress, TMPDIR, ftpuser, ftppassword, LocalDir;
            string DestinationString, sModel_name = "";
            LocalDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + labelname;


            try
            {
                sql = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME =: MODEL_NAME";
                if (LH.IsChecked) sModel_name = "Z_BOX_LH";
                if (GL.IsChecked) sModel_name = "Z_BOX_GL";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "MODEL_NAME", Value = sModel_name }
                    }
                });
                if (_result.Data != null)
                {
                    ftpaddress = _result.Data["model_serial"]?.ToString() ?? "";
                    SourceString = "http://" + ftpaddress + ftplabelpath + "//" + labelname;
                    if (!await DownLoadInternetFile(SourceString, LocalDir))
                    {
                        showMessage("Error during Download " + SourceString, "Lỗi khi tải xuống " + SourceString, true);
                        return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> DownLoadInternetFile(string Source, string Dest)
        {
            if (!File.Exists(Dest))
            {
                try
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFile(Source, Dest);
                    return true;
                }
                catch (Exception ex)
                {
                    showMessage(ex.Message, ex.Message, true);
                    return false;
                }
            }
            return false;
        }
        public async Task<string> labellocation()
        {
            string TEMPSTR;
            string sModel_name = "";
            sql = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME =: MODEL_NAME";
            if (LH.IsChecked) sModel_name = "Z_BOX_LH";
            if (GL.IsChecked) sModel_name = "Z_BOX_GL";
            if (Local.IsChecked)
            {
                return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "MODEL_NAME", Value = sModel_name }
                    }
            });
            if (_result.Data != null)
            {
                if (_result.Data["standard"].ToString() == "Y")
                {
                    locationlabel = true;
                    pubftppath = _result.Data["customer"]?.ToString() ?? "";
                    return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                }
                TEMPSTR = @"\\" + _result.Data["model_serial"].ToString() + "\\Lot_Reprint_CS6-BOX";
                return TEMPSTR;
            }
            showMessage("00172", "00172", false);
            return null;
        }
        public async void PL_model_check(string sn, int i)
        {
            if (sn != "")
            {
                var regexItem = new Regex("^[A-Z0-9 %/@:-]*$");
                if (!regexItem.IsMatch(sn))
                {
                    showMessage("00340", "00340", false);
                    return;
                }
                if (i == 1)
                {
                    txtInput1.SelectAll();
                    txtInput1.Focus();
                    return;
                }
                if (i == 2)
                {
                    txtInput2.SelectAll();
                    txtInput2.Focus();
                    return;
                }
            }
            SSN1 = sn;
            sql = "select model_name,mo_number from sfism4.r107 where serial_number='" + SSN1 + "' or shipping_sn= '" + SSN1 + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                MO = _result.Data["mo_number"]?.ToString() ?? "";
                model_name = _result.Data["model_name"]?.ToString() ?? "";
                if (i == 1)
                {
                    if (i == 1)
                    {
                        mo1 = MO;
                        model1 = model_name;
                    }
                    else
                    {
                        mo2 = MO;
                        model2 = model_name;
                    }
                    sql = "select * from sfism4.r105 where mo_number = '" + MO + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        err = true;
                        showMessage("00004", "00004", false);
                        return;
                    }
                    sql = "select cust_model_desc from sfis1.c_cust_snrule_t where model_name = '" + model_name + "'";
                    if (_result.Data == null)
                    {
                        err = true;
                        showMessage("00357", "00357", false);
                        return;
                    }
                    CUST_SSN1 = _result.Data["cust_model_desc"]?.ToString() ?? "";
                    if (CUST_SSN1 == "")
                    {
                        err = true;
                        showMessage("00358", "00358", false);
                        return;
                    }
                }
            }
            else
            {
                err = true;
                showMessage("40001", "40001", false);
                if (i == 1)
                {
                    txtInput1.SelectAll();
                    txtInput1.Focus();
                }
                if (i == 2)
                {
                    txtInput2.SelectAll();
                    txtInput2.Focus();
                }
                return;
            }

            if (i == 2)
            {
                if (mo1 != mo2 || model1 != model2)
                {
                    err = true;
                    showMessage("00359", "00359", false);
                    txtInput2.SelectAll();
                    txtInput2.Focus();
                    return;
                }
            }
        }

        public async Task<bool> getAllModelValue(string model_type, string s_sn)
        {
            sql = "select model_serial from sfis1.c_model_desc_t where model_name='SFISSITE'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                return true;
            }
            else
            {
                if (_result.Data["model_serial"].ToString() != "N")
                {
                    return false;
                }
                if (rmaDataInput.IsChecked)
                {
                    if (await getRmaData(s_sn))
                    {
                        return false;
                    }

                    txtInput1.Focus();
                    txtInput1.SelectAll();
                    txtInput2.Clear();
                    return false;
                }

                if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("071") != -1 || model_type.IndexOf("024") != -1))
                {
                    return true;
                }
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("047") != -1)
                {
                    txtInput2.Text = txtInput1.Text;
                    return true;
                }
                else
                {
                    if (!String.IsNullOrEmpty(model_type) && (model_type.IndexOf("016") != -1 || model_type.IndexOf("032") != -1 || model_type.IndexOf("045") != -1 || model_type.IndexOf("051") != -1))
                    {
                        if (!await findMacID(s_sn))
                        {
                            showMessage("00310", "00310", false);
                            return false;
                        }
                        else
                        {
                            txtInput2.Text = macID;
                        }
                    }
                    else
                    {
                        txtInput2.Text = txtInput1.Text;
                    }
                }
            }
            return true;
        }

        public async Task<bool> findMacID(string sn)
        {
            string lastKPSN;
            bool ContinueFindMACID = true;
            sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T " +
                "WHERE (SERIAL_NUMBER,WORK_TIME) IN( " +
                "SELECT SERIAL_NUMBER, MAX(WORK_TIME) FROM SFISM4.R_WIP_KEYPARTS_T " +
                "WHERE KEY_PART_NO = 'MACID' AND SERIAL_NUMBER =:sn GROUP BY SERIAL_NUMBER ) " +
                "AND KEY_PART_NO = 'MACID' AND SERIAL_NUMBER =:sn";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "sn", Value = sn }
                    }
            });
            if (_result.Data == null)
            {
                if (!String.IsNullOrEmpty(model_type) && model_type.IndexOf("B") != -1)
                {
                    sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE Group_name<>'ASSY' AND SERIAL_NUMBER =:sn";
                }
                else
                {
                    sql = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_NO <> KEY_PART_SN AND SERIAL_NUMBER =:sn";
                }
            }
            while (ContinueFindMACID)
            {
                //LastKPSN := FieldByName('KEY_PART_SN').AsString;
                //sql = sql;
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "sn", Value = sn }
                    }
                });
                if (_result.Data == null)
                {
                    ContinueFindMACID = false;
                    return false;
                }
                else
                {
                    lastKPSN = _result.Data["key_part_sn"]?.ToString() ?? "";
                    if (_result.Data["key_part_no"].ToString() == "MACID")
                    {
                        macID = lastKPSN;
                        ContinueFindMACID = false;
                        return true;
                    }
                    else
                    {
                        sn = lastKPSN;
                    }
                }
            }
            return false;
        }

        public async Task<bool> getRmaData(string sn)
        {
            string p_mo, t_str, ssn1, mac1, mac2, mac3, wep1;
            sql = "select mo_number,model_name from sfism4.r_wip_tracking_t where serial_number='" + sn + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                showMessage("40001", "40001", false);
                return false;
            }
            else
            {
                p_mo = _result.Data["mo_number"]?.ToString() ?? "";
                t_str = p_mo.Substring(1, 1);
                sql = "select mo_number,model_name from sfism4.r105 where job_mo_option like '%" + p_mo + "%'";
                _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data == null)
                {
                    showMessage("00639", "00639", false);
                    return false;
                }
                G_sLabl_Name = _result.Data["model_name"].ToString() + "_BX.LAB";
                sql = "select VR_VALUE from sfis1.C_PARAMETER_INI where vr_item='X5_MO' and vr_name='PREFIX_SECOND' AND PRG_NAME='RMA'";
                if (_result.Data == null)
                {
                    return false;
                }
                p_mo = _result.Data["vr_value"]?.ToString() ?? "";
                if (t_str.IndexOf(p_mo) != -1)
                {
                    if (!await getRmaDataSN(sn))
                    {
                        return false;
                    }
                    if (!await getRmaDataMAC(sn))
                    {
                        return false;
                    }
                    if (rmaSn == "" || rmaMac == "")
                    {
                        return false;
                    }

                    ssn1 = rmaSn;
                    mac1 = rmaMac;
                    mac2 = await add1tomacid(mac1);
                    mac3 = await add1tomacid(mac2);
                    dtParams.Clear();
                    AddParams("MAC1", mac1);
                    AddParams("MAC2", mac2);
                    AddParams("MAC3", mac3);
                    AddParams("SSN1", ssn1);

                    sql = "SELECT *  FROM  SFISM4.R_INVENTEL_DATA_T where MAC = '" + rmaMac + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data == null)
                    {
                        wep1 = _result.Data["wepkey"]?.ToString() ?? "";
                        AddParams("PinCode", wep1);
                    }
                    else
                    {
                        sql = "SELECT *  FROM  SFISM4.R_INVENTEL_DATA_T@sfcodbh where MAC = '" + rmaMac + "'";
                        _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data == null)
                        {
                            wep1 = _result.Data["wepkey"]?.ToString() ?? "";
                            AddParams("PinCode", wep1);
                        }
                    }
                    /*IF   SEditf_count.Text ='1'  THEN
                    begin
                      P_PrintToCodeSoft('C', G_sLabl_Name);
                    end;
                    //¦C¦L¨ä¥¦Label
                    if SEditf_count.Text<>'1'   then
                    begin
                      if sedprtnow.Text <>'0'  then
                      begin
                        labname:=copy(G_sLabl_Name,1,length(G_sLabl_Name)-4)+ sedprtnow.Text+'.Lab'  ;
                        P_PrintToCodeSoft('C', labname);
                      end;
                    end;*/
                    if (txtSeditfCount.Text == "1")
                    {

                    }
                    if (txtSeditfCount.Text != "1")
                    {
                        if (txtSedprtnow.Text != "0")
                        {

                        }
                    }
                }
            }
            return false;
        }

        private async Task<string> add1tomacid(string serial_number)
        {
            if (string.IsNullOrEmpty(serial_number))
                return "";
            string hexData = "0123456789ABCDEF";
            return getnextSN(serial_number, hexData, 1);
        }
        private string red1tomacid(string serial_number)
        {
            if (string.IsNullOrEmpty(serial_number))
                return "";
            string hexData = "FEDCBA9876543210";
            return getnextSN(serial_number, hexData, 1);
        }
        private async Task<string> add6tomacid(string serial_number)
        {
            if (string.IsNullOrEmpty(serial_number))
                return "";
            string hexData = "0123456789ABCDEF";
            string tmp = getnextSN(serial_number, hexData, 1);
            tmp = getnextSN(tmp, hexData, 1);
            tmp = getnextSN(tmp, hexData, 1);
            tmp = getnextSN(tmp, hexData, 1);
            tmp = getnextSN(tmp, hexData, 1);
            tmp = getnextSN(tmp, hexData, 1);
            return tmp;
        }
        private async Task<string> add2tomacid(string serial_number)
        {
            if (string.IsNullOrEmpty(serial_number))
                return "";
            string hexData = "0123456789ABCDEF";
            string tmp = getnextSN(serial_number, hexData, 1);
            tmp = getnextSN(tmp, hexData, 1);
            return tmp;
        }
        public async Task<bool> getRmaDataSN(string sn)
        {
            sql = "select * from sfism4.r_rma_link_t  where serial_number='" + sn + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                rmaSn = _result.Data["ssn1"]?.ToString() ?? "";
                return true;
            }
            return false;
        }

        public async Task<bool> getRmaDataMAC(string sn)
        {
            sql = "select * from sfism4.r_rma_link_t  where serial_number='" + sn + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                rmaMac = _result.Data["mac1"]?.ToString() ?? "";
                return true;
            }
            return false;
        }

        public async Task<bool> p_ensky_rework_boxlabel(string serial_number)
        {
            if (txtSeditfCount.Text != "1")
            {
                serial_number = serial_number.Substring(3, 7);
            }
            else if (serial_number.Trim().Length == 11)
            {
                serial_number = serial_number.Substring(3, 7);
            }
            txtInput2.Text = serial_number;
            sql = "select serial_number from sfism4.r_wip_tracking_t where po_no='" + serial_number + "' and mo_number=' + mo_label.Caption +'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                txtInput1.Text = _result.Data["serial_number"]?.ToString() ?? "";
                return true;
            }
            showMessage("00365", "00365", false);
            return false;
        }

        public async Task<bool> findR107(string serial_number)
        {
            sql = "select * from sfism4.r_wip_tracking_t where serial_number='" + serial_number + "' and rownum=1";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                if (_result.Data["work_flag"].ToString() == txtSeditfCount.Text.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<string> getDotSerialNumber(string serial_number)
        {
            sql = "SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE SERIAL_NUMBER IN" +
                "(SELECT SERIAL_NUMBER FROM SFISM4.R108 WHERE KEY_PART_SN='" + serial_number + "' AND UPPER(KEY_PART_NO) IN(SELECT UPPER(MODEL_NAME) FROM SFISM4.R107 WHERE SERIAL_NUMBER='" + serial_number + "'))";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data != null)
            {
                return _result.Data["serial_number"]?.ToString() ?? "";
            }
            else
            {
                return "";
            }
        }

        private async Task<Boolean> checkPalletFull(string data, string table)
        {
            string cImei = "";
            string cModel = "";
            string cVer = "";
            int cQty, palletQty, cartonQty, snQty;
            sql = "SELECT * FROM " + table + " WHERE IMEI='" + data + "' or serial_number='" + data + "' or shipping_sn='" + data + "'" +
                  "or mcarton_no='" + data + "' or carton_no='" + data + "' or pallet_no='" + data + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                return false;
            }
            else
            {
                cImei = _result.Data["imei"]?.ToString() ?? "";
                cModel = _result.Data["model_name"]?.ToString() ?? "";
                cVer = _result.Data["version_code"]?.ToString() ?? "";
                M_MODEL_NAME = _result.Data["model_name"]?.ToString() ?? "";

                sql = "SELECT MODEL_TYPE FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='" + M_MODEL_NAME + "'";
                var _result_modelType = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result_modelType.Data != null)
                {
                    model_type = _result_modelType.Data["model_type"]?.ToString() ?? "";
                }

                if (SSNLABEL.IsChecked)
                {
                    sql = "SELECT * FROM " + table + " WHERE IMEI='" + cImei + "'";
                }
                else
                {
                    sql = "SELECT DISTINCT MCARTON_NO FROM " + table + " WHERE IMEI='" + cImei + "'";
                }
                var _result1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result1.Data != null)
                {
                    cQty = _result1.Data.Count();
                    sql = "SELECT PALLET_QTY,CARTON_QTY FROM SFIS1.C_PACK_PARAM_T WHERE MODEL_NAME='" + cModel + "' AND VERSION_CODE='" + cVer + "'";
                    _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (_result.Data != null)
                    {
                        palletQty = Int32.Parse(_result.Data["pallet_qty"].ToString());
                        cartonQty = Int32.Parse(_result.Data["carton_qty"].ToString());
                        snQty = palletQty * cartonQty;
                        if (SSNLABEL.IsChecked)
                        {
                            if (cQty < snQty)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (cQty < palletQty)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public async void checkConnect()
        {
            string[] Args = Environment.GetCommandLineArgs();

            if (Args.Length == 1)
            {
                showMessage("ONLY OPEN FROM SFIS_5.0 PROGRAM", "CHỈ MỞ TRÊN HỆ THỐNG SFIS_5.0", true);
                //Environment.Exit(0);
            }

            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }

            string[] argsInfor = Regex.Split(inputLogin, @";");
            checkSum = argsInfor[0]?.ToString() ?? "";
            loginApiUrl = argsInfor[1]?.ToString() ?? "";
            loginDB = argsInfor[2]?.ToString() ?? "";
            empNo = argsInfor[3]?.ToString() ?? "";
            empPass = argsInfor[4]?.ToString() ?? "";

            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OK);
            //    Environment.Exit(0);
            //}

            var loginInfo = new
            {
                PRG_NAME = "LOT_Reprint"
            };
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            sfcClient = DBApi.sfcClient(loginDB, loginApiUrl);
            try
            {
                await sfcClient.GetAccessTokenAsync(empNo, empPass);
                dm2.sfcClient = sfcClient;
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.LOT_REPRINT_PROGRAM",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="TYPE",Value="CONNECT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic ads = result.Data;
                string Ok = ads[0]["output"];

                if (Ok.Substring(0, 2) != "OK")
                {
                    MessageBox.Show(Ok, "Error", MessageBoxButton.OK);
                    this.Close();
                }
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                UI_Route.Content = "Route: " + ini.IniReadValue("LOTREPRINT", "CHCEK_ROUTENAME");
                PCMAC = GetHostMacAddress();
            }
            catch (Exception ex)
            {
                //ReportError(ex.Message.ToString());
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OK);
                this.Close();
            }
        }
        private async Task<bool> checkReprint()
        {
            sql = "SELECT COUNT(PRG_NAME) COUNT FROM SFIS1.C_PARAMETER_INI WHERE VR_VALUE='YES' AND VR_ITEM='REPRINT' AND  PRG_NAME='REPRINT_LABEL'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data["count"].ToString() != "0")
            {
                return true;
            }
            return false;
        }

        private void RadioButton_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void rePrint_Click(object sender, RoutedEventArgs e)
        {
            if (reprint_flag == false)
            {
                frmLogin frmLogin = new frmLogin();
                frmLogin.sfcClient = sfcClient;
                frmLogin.type = "REPRINT";
                frmLogin.ShowDialog();
                if (frmLogin.ok == System.Windows.Forms.DialogResult.OK)
                {
                    reprint_flag = true;
                    rePrint.IsChecked = true;
                }
                else
                {
                    reprint_flag = false;
                    rePrint.IsChecked = false;
                }
            }
            else
            {
                reprint_flag = false;
                rePrint.IsChecked = false;
            }
        }

        public async Task<string> DoSelectSingleValueQueryString(string sql)
        {
            //int qty = 0;
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (_result.Data != null)
            {
                var obj = _result.Data;
                //qty = int.Parse(_result.Data["qty"].ToString());
                return obj["value_1"]?.ToString() ?? "";
            }
            else
            {
                return null;
            }
        }

        //Get IP
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

        //Add Param
        public void AddParams(string _name, string _value)
        {
            return;//Closed old func
            if (dtParams.Columns.Count == 0)
            {
                dtParams.Columns.Add("Name");
                dtParams.Columns.Add("Value");
            }
            dtParams.Rows.Add(new object[] { _name, _value });
        }
        public void AddParam(string _name, string _value,bool newFlag)
        {
            if (_name is null) return;
            if (dtParams.Columns.Count == 0)
            {
                dtParams.Columns.Add("Name");
                dtParams.Columns.Add("Value");
            }

            if (_name == "LabelName") G_sLabl_Name = _value;
            else if (_name == "LabelQty") txtSeditLabelQty.Text = _value.ToString();
            else
            {
                if (_name.ToUpper() == "MODELNAME") txtModelName.Text = _value;
                if (_name.ToUpper() == "MONUMBER") txtMoNumber.Text = _value;
                if (PrintTest.IsChecked)
                {
                    string pattern = "[a-zA-Z0-9]";
                    Regex regEx = new Regex(pattern);
                    _value = Regex.Replace(regEx.Replace(_value, "X"), @"\s+", " ");
                    dtParams.Rows.Add(new object[] { _name, _value });
                    return;
                }
                else
                    dtParams.Rows.Add(new object[] { _name, _value });
            } 
        }
        //Show message
        private void showMessage(string MessageEnglish, string MessageVietNam, bool CustomFlag)
        {
            frmMessage frmMessage = new frmMessage();
            frmMessage.sfcClient = sfcClient;
            if (CustomFlag)
            {
                frmMessage.MessageEnglish = MessageEnglish;
                frmMessage.MessageVietNam = MessageVietNam;
                frmMessage.CustomFlag = CustomFlag;
            }
            else
            {
                frmMessage.errorcode = MessageEnglish;
                frmMessage.CustomFlag = CustomFlag;
            }
            frmMessage.ShowDialog();
        }
        private void showMessage(string content)
        {
            string MessageEnglish="", MessageVietNam = "";
            if (content.IndexOf("|") > 0)
            {
                MessageEnglish = content.Split('|')[0].ToString().Trim();
                MessageVietNam = content.Split('|')[1].ToString().Trim();
            }
            else
            {
                MessageEnglish = content;
                MessageVietNam = content;
            }
            frmMessage frmMessage = new frmMessage();
            frmMessage.sfcClient = sfcClient;
            frmMessage.MessageEnglish = MessageEnglish;
            frmMessage.MessageVietNam = MessageVietNam;
            frmMessage.CustomFlag = true;
            frmMessage.ShowDialog();
        }
        public static string getnextSN(string current, string strSN, int number = 1)
        {
            int _num = number;
            string _nextssn = current;
            while (_num > 0)
            {
                _nextssn = MygetnextMac(_nextssn, strSN);
                _num--;
            }
            return _nextssn;
        }

        public static string MygetnextMac(string current, string strSN)
        {
            int _length = current.Length;
            char _last = current.Last();
            if (getadd(ref _last, strSN) == false)
            {
                if (_length != 1)
                {
                    return MygetnextMac(current.Substring(0, current.Length - 1), strSN) + _last;
                }
                else
                {
                    return "_";
                }
            }
            else
            {
                return current.Substring(0, current.Length - 1) + _last;
            }
        }

        public static bool getadd(ref char last, string strSN)
        {
            int _length = strSN.Length;
            int _num = strSN.IndexOf(last);
            if (_num < 0)
            {
                return true;
            }
            if (_num == _length - 1)
            {
                last = strSN[0];
                return false;
            }

            last = strSN[_num + 1];
            return true;
        }
        private SerialPort comport = new SerialPort();
        private void disconnect()
        {
            //sfcClient
        }
        public void DispatcherTimerSample()
        {
            count = 5;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //count--;
            //if (count == 0)
            //{
            //    disconnect();
            //    dispatcherTimer.Stop();
            //    showMessage("Disconnect!\nAfter 5 minutes, You can't use the program.", "Ngắt kết nối!\nSau 5 phút, bạn không thể sử dụng chương trình.", true);
            //    this.Close();
            //}
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            count = 5;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            count = 5;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
        public dynamic _ads;
        private async Task call_SP(string in_Data,string in_func)
        {
            in_Data= in_Data+ "|IP:" + GetIPAddress()+"|EMP:"+ empNo;
            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_LotReprint",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="AP_VER",Value=prgVer,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MYGROUP",Value=M_Group,SfcParameterDataType =SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="IN_DATA",Value=in_Data,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="IN_FUNC",Value=in_func,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                    new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                }
            });
            _ads = result.Data;
        }
    }
}
