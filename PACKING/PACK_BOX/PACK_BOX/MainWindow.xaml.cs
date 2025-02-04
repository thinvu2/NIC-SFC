using PACK_BOX.Model;
using Sfc.Library.HttpClient;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Net;
using Sfc.Core.Parameters;
using System.Reflection;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using System.Deployment.Application;
using System.Windows.Controls;
using Sfc.Library.HttpClient.Helpers;
using System.Linq;
using System.Diagnostics;

namespace PACK_BOX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string checkSum;
        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUri = "";
        public string loginDB = "";
        public string empNo = "";
        public string empPass = "";
        public string inputLogin = "";

        public delegate void getFormScan(object input);
        public static string _Inputdata = "";
        public static string _CUSTSN_STR = "";
        public string local_strIP, G_sBuildDate, sPrgName;
        public string M_sThisSection, M_sThisGroup, M_sThisStation,M_sThisLine, MACID;
        public bool sCAPS_LOCK, BOXID_FLAG;
        public string sMyGroup, chk_ssn6, chk_ssn7;
        public string Next_Step, sCUSTSN_CODE, CTNSCAN_SHIPPINGSN, tmp_CUSTSN_NAME, SCAN_SSN_STR;
        public string Bnext_group, BOXID;
        public int SCAN_POS;
        public string M_modeltype, CHECKFLAG, sMO_TYPE, M_sSSN1, RESULT;
        public bool CHECK_MO_FLAG, CHECKSN_OK, SSN_OK, Keypart_OK, is_check_orbi_ax;
        public string res, sLine_name, comsnchar;
        public string Err_Msg, Mes_err;
        public string SSNSTRING, last_orbi_ax_val;
        public string MODELSTR, Wo_type;
        public string _SSN, _MAC, sn_temp,zTemp1,zTemp2;
        public string MO_Status;

        public string inPN, inSN, inSSN,inQR;
        Boolean inputFlag = false;

        //TexBox
        TextBox editSerialNumber = new TextBox();

        //Array
        TSSN_RULE[] sLOAD_SSN = new TSSN_RULE[31];
        TMAC_RULE[] sLOAD_MAC = new TMAC_RULE[6];
        OTHER_USE_ASSN[] sOTHER_USE_ASSN = new OTHER_USE_ASSN[5];
        R_CUSTDATE[] sSSNR_CUSTDATE = new R_CUSTDATE[16];
        R_CUSTDATE[] sMACR_CUSTDATE = new R_CUSTDATE[16];
        TMAC_MODEL[] SMAC = new TMAC_MODEL[11];
        List<SOURCE> sequenlist = new List<SOURCE>();

        //Menuitems
        MenuItem CheckMacID1 = new MenuItem();
        //ListBox
        ListBox LB_2D = new ListBox();
        public MainWindow()
        {
            InitializeComponent();
            PACKING();
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
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "This Program " + process.ProcessName + " in operation! ";
                        _er.MessageEnglish = "Chương trình " + process.ProcessName + " đang bật!";
                        _er.ShowDialog();
                        Environment.Exit(0);
                    }
                }
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
        public enum HashingAlgoTypes
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }
        // Get DateTime
        public async Task GetSysDateTime(string DateTimeT, string Mo_Date, string Time)
        {
            string strGetDateTim = "Select SysDate DateTime,To_Char(SysDate,'yyyymmdd') Mo_Date,To_Char(SysDate, 'hh24mi') Time From Dual  ";
            var qry_DateTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDateTim,
                SfcCommandType = SfcCommandType.Text
            });
            DateTime DateTime = Convert.ToDateTime(qry_DateTime.Data["SysDate"].ToString());
            Mo_Date = qry_DateTime.Data["Mo_Date"].ToString();
            Time = qry_DateTime.Data["Time"].ToString();
        }

        // Get localIP addresses
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
        public void Receiver(object input)
        {
            _Inputdata = (string)input;
        }


        public void editEmployeeChange(object sender, TextChangedEventArgs e)
        {
            editSerialNumber.IsEnabled = false;
            for (int i = 1; i <= 30; i++)
            {
                sLOAD_SSN[i] = new TSSN_RULE();
                sLOAD_SSN[i].SSN = "";
                sLOAD_SSN[i].cSSN_PREFIX = "";
                sLOAD_SSN[i].cSSN_POSTFIX = "";
                sLOAD_SSN[i].cSSN_STR = "";
                sLOAD_SSN[i].sSHIPPINGSN_CODE = "";
                sLOAD_SSN[i].sCHECK_SSN_RULE = "";
                sLOAD_SSN[i].sCHECK_SSN_RANGE = "";
                sLOAD_SSN[i].sCHECK_SSN = "";
                sLOAD_SSN[i].nSSN_LENGTH = 0;
                sLOAD_SSN[i].nSSN_PREFIX_LEN = 0;
                sLOAD_SSN[i].nSSN_POSTFIX_LEN = 0;
                sLOAD_SSN[i].sCOMPARE_SSN = "";
                sLOAD_SSN[i].nSSN_Self_StartDigit = 0;
                sLOAD_SSN[i].nSSN_Self_FlowNO = 0;
                sLOAD_SSN[i].nSSN_Compare_StartDigit = 0;
                sLOAD_SSN[i].nSSN_Compare_FlowNO = 0;
            }

            for (int i = 1; i <= 5; i++)
            {
                sLOAD_MAC[i] = new TMAC_RULE();
                sLOAD_MAC[i].MAC = "";
                sLOAD_MAC[i].cMAC_PREFIX = "";
                sLOAD_MAC[i].cMAC_POSTFIX = "";
                sLOAD_MAC[i].cMAC_STR = "";
                sLOAD_MAC[i].sMACID_CODE = "";
                sLOAD_MAC[i].sCHECK_MAC_RULE = "";
                sLOAD_MAC[i].sCHECK_MAC = "";
                sLOAD_MAC[i].sCHECK_MAC_RANGE = "";
                sLOAD_MAC[i].nMAC_LENGTH = 0;
                sLOAD_MAC[i].nMAC_PREFIX_LEN = 0;
                sLOAD_MAC[i].nMAC_POSTFIX_LEN = 0;
                sLOAD_MAC[i].sCOMPARE_MAC = "";
                sLOAD_MAC[i].nMAC_Self_StartDigit = 0;
                sLOAD_MAC[i].nMAC_Self_FlowNO = 0;
                sLOAD_MAC[i].nMAC_Compare_StartDigit = 0;
                sLOAD_MAC[i].nMAC_Compare_FlowNO = 0;
            }

            for (int i = 1; i <= 10; i++)
            {
                SMAC[i] = new TMAC_MODEL();
                SMAC[i].CUSTSN_MAC = "";
                SMAC[i].MODEL_NAME = "";
                SMAC[i].MAC = "";
            }

            for (int i = 1; i <= 15; i++)
            {
                sSSNR_CUSTDATE[i] = new R_CUSTDATE();
                sSSNR_CUSTDATE[i].THE_LENGTH = 0;
            }

            for (int i = 1; i <= 15; i++)
            {
                sMACR_CUSTDATE[i] = new R_CUSTDATE();
                sMACR_CUSTDATE[i].THE_LENGTH = 0;
            }

            InputData.Text = "";
            InputData.IsEnabled = true;
            InputData.Focus();
            lbError.Text = "000 - Please Input Data";
            for (int i = 1; i <= 12; i++)
            {
                sSSNR_CUSTDATE[i] = new R_CUSTDATE();
                sSSNR_CUSTDATE[i].THE_LENGTH = 0;
            }

            for (int i = 1; i <= 5; i++)
            {
                sMACR_CUSTDATE[i] = new R_CUSTDATE();
                sMACR_CUSTDATE[i].THE_LENGTH = 0;
            }
        }
        public async Task updateR107()
        {
            string mssn1, mssn2, mssn3, mssn4, mssn5, mssn6, mssn7, mssn8, mssn9, mssn10, mssn11, mssn12, mssn13, mssn14, mssn15, mssn16;
            string mssn17, mssn18, mssn19, mssn20, mssn21, mssn22, mssn23, mssn24, mssn25, mssn26, mssn27, mssn28, mssn29, mssn30;
            string mmac1, mmac2, mmac3, mmac4, mmac5;
            string shippingsn, mgroup;

            mgroup = lbTitle.Content.ToString();
           

            string strGetSerialNumber = $"select * from SFISM4.R_CUSTSN_T where SERIAL_NUMBER ='{editSerialNumber.Text}'";
            var qry_SerialNumber = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetSerialNumber,
                SfcCommandType = SfcCommandType.Text
            });

            if (qry_SerialNumber.Data == null)
            {
                try
                {
                    string strInsert = "Insert into SFISM4.R_CUSTSN_T (MO_NUMBER,SERIAL_NUMBER,SSN1,SSN2,SSN3,SSN4,SSN5,SSN6,SSN7,SSN8,SSN9,SSN10,SSN11,SSN12, SSN13,MAC1,MAC2,MAC3,MAC4,MAC5,GROUP_NAME)"
                   + $"VALUES ('{ Edt_moNumber.Text}','{editSerialNumber.Text}',"
                   + $"'{sLOAD_SSN[1].SSN}',"
                   + $"'{sLOAD_SSN[2].SSN}',"
                   + $"'{sLOAD_SSN[3].SSN}',"
                   + $"'{sLOAD_SSN[4].SSN}',"
                   + $"'{sLOAD_SSN[5].SSN}',"
                   + $"'{sLOAD_SSN[6].SSN}',"
                   + $"'{sLOAD_SSN[7].SSN}',"
                   + $"'{sLOAD_SSN[8].SSN}',"
                   + $"'{sLOAD_SSN[9].SSN}',"
                   + $"'{sLOAD_SSN[10].SSN}',"
                   + $"'{sLOAD_SSN[11].SSN}',"
                   + $"'{sLOAD_SSN[12].SSN}',"
                   //add new ssn13
                 + $"'{sLOAD_SSN[13].SSN}',"
                   + $"'{sLOAD_MAC[1].MAC}',"
                   + $"'{sLOAD_MAC[2].MAC}',"
                   + $"'{sLOAD_MAC[3].MAC}',"
                   + $"'{sLOAD_MAC[4].MAC}',"
                   + $"'{sLOAD_MAC[5].MAC}',"
                   + $"'{mgroup}')";
                    var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strInsert,
                        SfcCommandType = SfcCommandType.Text
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
                var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strChkROKU,
                    SfcCommandType = SfcCommandType.Text
                });              
                if (qry_ChkROKU.Data.Count() > 0)
                {
                    mssn1 = qry_SerialNumber.Data["ssn1"]?.ToString() ?? "";
                    mssn2 = qry_SerialNumber.Data["ssn2"]?.ToString() ?? "";
                    mssn3 = qry_SerialNumber.Data["ssn3"]?.ToString() ?? "";
                    mssn4 = qry_SerialNumber.Data["ssn4"]?.ToString() ?? "";
                    mssn5 = qry_SerialNumber.Data["ssn5"]?.ToString() ?? "";
                    mssn6 = qry_SerialNumber.Data["ssn6"]?.ToString() ?? "";
                    mssn7 = qry_SerialNumber.Data["ssn7"]?.ToString() ?? "";
                    mssn8 = qry_SerialNumber.Data["ssn8"]?.ToString() ?? "";
                    mssn9 = qry_SerialNumber.Data["ssn9"]?.ToString() ?? "";
                    mssn10 = qry_SerialNumber.Data["ssn10"]?.ToString() ?? "";
                    mssn11 = qry_SerialNumber.Data["ssn11"]?.ToString() ?? "";
                    mssn12 = qry_SerialNumber.Data["ssn12"]?.ToString() ?? "";
                    mmac1 = qry_SerialNumber.Data["mac1"]?.ToString() ?? "";
                    mmac2 = qry_SerialNumber.Data["mac2"]?.ToString() ?? "";
                    mmac3 = qry_SerialNumber.Data["mac3"]?.ToString() ?? "";
                    mmac4 = qry_SerialNumber.Data["mac4"]?.ToString() ?? "";
                    mmac5 = qry_SerialNumber.Data["mac5"]?.ToString() ?? "";
                    StringBuilder sb_Roku = new StringBuilder();
                    sb_Roku.Append("UPDATE");
                    sb_Roku.Append($" SFISM4.R_CUSTSN_T set in_station_time=sysdate, group_name = '{mgroup}', MO_NUMBER = '{Edt_moNumber.Text}'");
                    if (BOXID_FLAG == true)
                    {
                        if (mssn1.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn1 ='" + ((sLOAD_SSN[1].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[1].SSN + "'");
                        }
                        if (mssn2.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn2 ='" + ((sLOAD_SSN[2].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[2].SSN + "'");
                        }
                        if (mssn3.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn3 ='" + ((sLOAD_SSN[3].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[3].SSN + "'");
                        }
                        if (mssn4.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn4 ='" + ((sLOAD_SSN[4].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[4].SSN + "'");
                        }
                        if (mssn5.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn5 ='" + ((sLOAD_SSN[5].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[5].SSN + "'");
                        }
                        if (mssn6.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn6 ='" + ((sLOAD_SSN[6].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[6].SSN + "'");
                        }
                        if (mssn7.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn7 ='" + ((sLOAD_SSN[7].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[7].SSN + "'");
                        }
                        if (mssn8.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn8 ='" + ((sLOAD_SSN[8].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[8].SSN + "'");
                        }
                        if (mssn9.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn9 ='" + ((sLOAD_SSN[9].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[9].SSN + "'");
                        }
                        if (mssn10.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn10 ='" + ((sLOAD_SSN[10].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[10].SSN + "'");
                        }
                        if (mssn11.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn11 ='" + ((sLOAD_SSN[11].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[11].SSN + "'");
                        }
                        if (mssn12.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn12 ='" + ((sLOAD_SSN[12].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[12].SSN + "'");
                        }
                        if (mmac1.Length == 0)
                        {
                            sb_Roku.Append(" ,mac1 ='" + ((sLOAD_MAC[1].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[1].MAC + "'");
                        }
                        if (mmac2.Length == 0)
                        {
                            sb_Roku.Append(" ,mac2 ='" + ((sLOAD_MAC[1].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[1].MAC + "'");
                        }
                        if (mmac3.Length == 0)
                        {
                            sb_Roku.Append(" ,mac3 ='" + ((sLOAD_MAC[3].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[3].MAC + "'");
                        }
                        if (mmac4.Length == 0)
                        {
                            sb_Roku.Append(" ,mac4 ='" + ((sLOAD_MAC[4].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[4].MAC + "'");
                        }
                        if (mmac5.Length == 0)
                        {
                            sb_Roku.Append(" ,mac5 ='" + ((sLOAD_MAC[5].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[5].MAC + "'");
                        }
                    }
                    else
                    {
                        if (mssn1.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn1 ='" + ((sLOAD_SSN[1].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[1].SSN + "'");
                        }
                        if (mssn2.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn2 ='" + ((sLOAD_SSN[2].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[2].SSN + "'");
                        }
                        if (mssn3.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn3 ='" + ((sLOAD_SSN[3].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[3].SSN + "'");
                        }
                        if (mssn4.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn4 ='" + ((sLOAD_SSN[4].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[4].SSN + "'");
                        }
                        if (mssn5.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn5 ='" + ((sLOAD_SSN[5].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[5].SSN + "'");
                        }
                        if (mssn6.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn6 ='" + ((sLOAD_SSN[6].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[6].SSN + "'");
                        }
                        if (mssn7.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn7 ='" + ((sLOAD_SSN[7].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[7].SSN + "'");
                        }
                        if (mssn8.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn8 ='" + ((sLOAD_SSN[8].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[8].SSN + "'");
                        }
                        if (mssn9.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn9 ='" + ((sLOAD_SSN[9].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[9].SSN + "'");
                        }
                        if (mssn10.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn10 ='" + ((sLOAD_SSN[10].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[10].SSN + "'");
                        }
                        if (mssn11.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn11 ='" + ((sLOAD_SSN[11].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[11].SSN + "'");
                        }
                        if (mssn12.Length == 0)
                        {
                            sb_Roku.Append(" ,ssn12 ='" + ((sLOAD_SSN[12].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[12].SSN + "'");
                        }
                        if (mmac1.Length == 0)
                        {
                            sb_Roku.Append(" ,mac1 ='" + ((sLOAD_MAC[1].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[1].MAC + "'");
                        }
                        if (mmac2.Length == 0)
                        {
                            sb_Roku.Append(" ,mac2 ='" + ((sLOAD_MAC[1].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[1].MAC + "'");
                        }
                        if (mmac3.Length == 0)
                        {
                            sb_Roku.Append(" ,mac3 ='" + ((sLOAD_MAC[3].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[3].MAC + "'");
                        }
                        if (mmac4.Length == 0)
                        {
                            sb_Roku.Append(" ,mac4 ='" + ((sLOAD_MAC[4].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[4].MAC + "'");
                        }
                        if (mmac5.Length == 0)
                        {
                            sb_Roku.Append(" ,mac5 ='" + ((sLOAD_MAC[5].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[5].MAC + "'");
                        }
                    }
                    sb_Roku.Append(" where serial_number = '" + editSerialNumber.Text + "'");
                    string strUpdate = sb_Roku.ToString();
                    try
                    {
                        var strUPdate = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strUpdate,
                            SfcCommandType = SfcCommandType.Text
                        });

                        // Backup r_custsn_t data
                        if (BOXID_FLAG == true)
                        {
                            string strInsert = "Insert into SFISM4.R_CUSTSN_T_BAK ( SELECT * FROM SFISM4.R_CUSTSN_T WHERE  serial_number in"
                                + " (select serial_number from sfism4.r_wip_tracking_t where tray_no = '" + BOXID + "' ))";
                            var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strInsert,
                                SfcCommandType = SfcCommandType.Text
                            });

                        }
                        else
                        {
                            string strInsert = "Insert into SFISM4.R_CUSTSN_T_BAK ( SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='" + editSerialNumber.Text + "')";
                            var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strInsert,
                                SfcCommandType = SfcCommandType.Text
                            });
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    mssn1 = qry_SerialNumber.Data["ssn1"]?.ToString() ?? "";
                    mssn2 = qry_SerialNumber.Data["ssn2"]?.ToString() ?? "";
                    mssn3 = qry_SerialNumber.Data["ssn3"]?.ToString() ?? "";
                    mssn4 = qry_SerialNumber.Data["ssn4"]?.ToString() ?? "";
                    mssn5 = qry_SerialNumber.Data["ssn5"]?.ToString() ?? "";
                    mssn6 = qry_SerialNumber.Data["ssn6"]?.ToString() ?? "";
                    mssn7 = qry_SerialNumber.Data["ssn7"]?.ToString() ?? "";
                    mssn8 = qry_SerialNumber.Data["ssn8"]?.ToString() ?? "";
                    mssn9 = qry_SerialNumber.Data["ssn9"]?.ToString() ?? "";
                    mssn10 = qry_SerialNumber.Data["ssn10"]?.ToString() ?? "";
                    mssn11 = qry_SerialNumber.Data["ssn11"]?.ToString() ?? "";
                    mssn12 = qry_SerialNumber.Data["ssn12"]?.ToString() ?? "";
                    mssn13 = qry_SerialNumber.Data["ssn13"]?.ToString() ?? "";
                    mssn14 = qry_SerialNumber.Data["ssn14"]?.ToString() ?? "";
                    mssn15 = qry_SerialNumber.Data["ssn15"]?.ToString() ?? "";
                    mssn16 = qry_SerialNumber.Data["ssn16"]?.ToString() ?? "";
                    mssn17 = qry_SerialNumber.Data["ssn17"]?.ToString() ?? "";
                    mssn18 = qry_SerialNumber.Data["ssn18"]?.ToString() ?? "";
                    mssn19 = qry_SerialNumber.Data["ssn19"]?.ToString() ?? "";
                    mssn20 = qry_SerialNumber.Data["ssn20"]?.ToString() ?? "";
                    mssn21 = qry_SerialNumber.Data["ssn21"]?.ToString() ?? "";
                    mssn22 = qry_SerialNumber.Data["ssn22"]?.ToString() ?? "";
                    mssn23 = qry_SerialNumber.Data["ssn23"]?.ToString() ?? "";
                    mssn24 = qry_SerialNumber.Data["ssn24"]?.ToString() ?? "";
                    mssn25 = qry_SerialNumber.Data["ssn25"]?.ToString() ?? "";
                    mssn26 = qry_SerialNumber.Data["ssn26"]?.ToString() ?? "";
                    mssn27 = qry_SerialNumber.Data["ssn27"]?.ToString() ?? "";
                    mssn28 = qry_SerialNumber.Data["ssn28"]?.ToString() ?? "";
                    mssn29 = qry_SerialNumber.Data["ssn29"]?.ToString() ?? "";
                    mssn30 = qry_SerialNumber.Data["ssn30"]?.ToString() ?? "";
                    mmac1 = qry_SerialNumber.Data["mac1"]?.ToString() ?? "";
                    mmac2 = qry_SerialNumber.Data["mac2"]?.ToString() ?? "";
                    mmac3 = qry_SerialNumber.Data["mac3"]?.ToString() ?? "";
                    mmac4 = qry_SerialNumber.Data["mac4"]?.ToString() ?? "";
                    mmac5 = qry_SerialNumber.Data["mac5"]?.ToString() ?? "";
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE");
                    sb.Append(" SFISM4.R_CUSTSN_T set in_station_time=sysdate, group_name = '"+mgroup+"', MO_NUMBER = '" + Edt_moNumber.Text + "'");
                    if (mssn1.Length == 0)
                    {
                        sb.Append(" ,ssn1 ='" + ((sLOAD_SSN[1].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[1].SSN + "'");
                    }
                    if (mssn2.Length == 0)
                    {
                        sb.Append(" ,ssn2 ='" + ((sLOAD_SSN[2].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[2].SSN + "'");
                    }
                    if (mssn3.Length == 0)
                    {
                        sb.Append(" ,ssn3 ='" + ((sLOAD_SSN[3].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[3].SSN + "'");
                    }
                    if (mssn4.Length == 0)
                    {
                        sb.Append(" ,ssn4 ='" + ((sLOAD_SSN[4].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[4].SSN + "'");
                    }
                    if (mssn5.Length == 0)
                    {
                        sb.Append(" ,ssn5 ='" + ((sLOAD_SSN[5].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[5].SSN + "'");
                    }
                    if (mssn6.Length == 0)
                    {
                        sb.Append(" ,ssn6 ='" + ((sLOAD_SSN[6].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[6].SSN + "'");
                    }
                    if (mssn7.Length == 0)
                    {
                        sb.Append(" ,ssn7 ='" + ((sLOAD_SSN[7].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[7].SSN + "'");
                    }
                    if (mssn8.Length == 0)
                    {
                        sb.Append(" ,ssn8 ='" + ((sLOAD_SSN[8].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[8].SSN + "'");
                    }
                    if (mssn9.Length == 0)
                    {
                        sb.Append(" ,ssn9 ='" + ((sLOAD_SSN[9].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[9].SSN + "'");
                    }
                    if (mssn10.Length == 0)
                    {
                        sb.Append(" ,ssn10 ='" + ((sLOAD_SSN[10].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[10].SSN + "'");
                    }
                    if (mssn11.Length == 0)
                    {
                        sb.Append(" ,ssn11 ='" + ((sLOAD_SSN[11].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[11].SSN + "'");
                    }
                    if (mssn12.Length == 0)
                    {
                        sb.Append(" ,ssn12 ='" + ((sLOAD_SSN[12].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[12].SSN + "'");
                    }
                    if (mssn13.Length == 0)
                    {
                        sb.Append(" ,ssn13 ='" + ((sLOAD_SSN[13].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[13].SSN + "'");
                    }
                    if (mssn14.Length == 0)
                    {
                        sb.Append(" ,ssn14 ='" + ((sLOAD_SSN[14].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[14].SSN + "'");
                    }
                    if (mssn15.Length == 0)
                    {
                        sb.Append(" ,ssn15 ='" + ((sLOAD_SSN[15].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[15].SSN + "'");
                    }
                    if (mssn16.Length == 0)
                    {
                        sb.Append(" ,ssn16 ='" + ((sLOAD_SSN[16].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[16].SSN + "'");
                    }
                    if (mssn17.Length == 0)
                    {
                        sb.Append(" ,ssn17 ='" + ((sLOAD_SSN[17].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[17].SSN + "'");
                    }
                    if (mssn18.Length == 0)
                    {
                        sb.Append(" ,ssn18 ='" + ((sLOAD_SSN[18].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[18].SSN + "'");
                    }
                    if (mssn19.Length == 0)
                    {
                        sb.Append(" ,ssn19 ='" + ((sLOAD_SSN[19].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[19].SSN + "'");
                    }
                    if (mssn20.Length == 0)
                    {
                        sb.Append(" ,ssn20 ='" + ((sLOAD_SSN[20].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[20].SSN + "'");
                    }
                    if (mssn21.Length == 0)
                    {
                        sb.Append(" ,ssn21 ='" + ((sLOAD_SSN[21].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[21].SSN + "'");
                    }
                    if (mssn22.Length == 0)
                    {
                        sb.Append(" ,ssn22 ='" + ((sLOAD_SSN[22].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[22].SSN + "'");
                    }
                    if (mssn23.Length == 0)
                    {
                        sb.Append(" ,ssn23 ='" + ((sLOAD_SSN[23].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[23].SSN + "'");
                    }
                    if (mssn24.Length == 0)
                    {
                        sb.Append(" ,ssn24 ='" + ((sLOAD_SSN[24].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[24].SSN + "'");
                    }
                    if (mssn25.Length == 0)
                    {
                        sb.Append(" ,ssn25 ='" + ((sLOAD_SSN[25].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[25].SSN + "'");
                    }
                    if (mssn26.Length == 0)
                    {
                        sb.Append(" ,ssn26 ='" + ((sLOAD_SSN[26].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[26].SSN + "'");
                    }
                    if (mssn27.Length == 0)
                    {
                        sb.Append(" ,ssn27 ='" + ((sLOAD_SSN[27].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[27].SSN + "'");
                    }
                    if (mssn28.Length == 0)
                    {
                        sb.Append(" ,ssn28 ='" + ((sLOAD_SSN[28].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[28].SSN + "'");
                    }
                    if (mssn29.Length == 0)
                    {
                        sb.Append(" ,ssn29 ='" + ((sLOAD_SSN[29].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[29].SSN + "'");
                    }
                    if (mssn30.Length == 0)
                    {
                        sb.Append(" ,ssn30 ='" + ((sLOAD_SSN[30].sSHIPPINGSN_CODE).Trim()).ToUpper() + sLOAD_SSN[30].SSN + "'");
                    }
                    if (mmac1.Length == 0)
                    {
                        sb.Append(" ,mac1 ='" + ((sLOAD_MAC[1].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[1].MAC + "'");
                    }
                    if (mmac2.Length == 0)
                    {
                        sb.Append(" ,mac2 ='" + ((sLOAD_MAC[2].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[2].MAC + "'");
                    }
                    if (mmac3.Length == 0)
                    {
                        sb.Append(" ,mac3 ='" + ((sLOAD_MAC[3].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[3].MAC + "'");
                    }
                    if (mmac4.Length == 0)
                    {
                        sb.Append(" ,mac4 ='" + ((sLOAD_MAC[4].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[4].MAC + "'");
                    }
                    if (mmac5.Length == 0)
                    {
                        sb.Append(" ,mac5 ='" + ((sLOAD_MAC[5].sMACID_CODE).Trim()).ToUpper() + sLOAD_MAC[5].MAC + "'");
                    }
                    sb.Append(" where serial_number = '" + editSerialNumber.Text + "'");
                    string strUpdate = sb.ToString();
                    try
                    {
                        var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strUpdate,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //Modify by ALIAN 20150316
                    if (M_modeltype.IndexOf("221") > -1)
                    {
                        string strGetCount = "SELECT count(*) FROM SFISM4.R107 A WHERE A.SHIPPING_SN IN ("
                            + " select KEY_PART_SN from sfism4.r_wip_keyparts_t B where B.serial_number='" + editSerialNumber.Text + "'"
                            + " and group_name='KEYPART' AND PART_MODE in"
                            + " ( select substr('" + Edt_modelName.Text + "',1,instr('" + Edt_modelName.Text + "','-')-1)  from dual))";
                        var qry_Count = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetCount,
                            SfcCommandType = SfcCommandType.Text
                        });

                        if (qry_Count.Data["count(*)"].ToString() == "0")
                        {
                            lbError.Text = "Not data link in R108";
                            ShowMessageForm _er = new ShowMessageForm(this,sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "Du lieu link khong ton tai trong R108";
                            _er.MessageEnglish = "Not data link in R108";
                            return;
                        }
                        else
                        {
                            string strGetDataKPSN = "SELECT SERIAL_NUMBER FROM SFISM4.R107 A WHERE A.SHIPPING_SN IN ("
                                + " select KEY_PART_SN from sfism4.r_wip_keyparts_t B where B.serial_number='" + editSerialNumber.Text + "' and group_name='KEYPART' 'AND PART_MODE in"
                                + " ( select substr('" + Edt_modelName.Text + "',1,instr('" + Edt_modelName.Text + "','-')-1)  from dual))";
                            var qry_DataKPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetDataKPSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            //debug
                        }
                    }
                    //Modify by ALIAN 20150316

                    //backup r_custsn_t data
                    try
                    {
                        string strInsert = "Insert into SFISM4.R_CUSTSN_T_BAK ( SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='" + editSerialNumber.Text + "')";
                        var InsertBak = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strInsert,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            string strGetSN = "select * from sfism4.r_wip_tracking_t"
                + " where serial_number = '" + editSerialNumber.Text + "'";
            var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetSN,
                SfcCommandType = SfcCommandType.Text
            });
            shippingsn = qry_SN.Data["shipping_sn"].ToString();
            if (shippingsn == "N/A")
            {
                shippingsn = "";
            }

            if (M_sThisGroup != "PACK_BOXII")
            {
                if (((sLOAD_SSN[1].sSHIPPINGSN_CODE).Trim().ToUpper() + sLOAD_SSN[1].SSN != "") && shippingsn == "")
                {
                    try
                    {
                        string strUpdate = "update sfism4.r_wip_tracking_t"
                            + " set shipping_sn = '" + (sLOAD_SSN[1].sSHIPPINGSN_CODE).Trim().ToUpper() + sLOAD_SSN[1].SSN + "',"
                            + " shipping_sn2 = '" + (sLOAD_MAC[1].sMACID_CODE).Trim().ToUpper() + sLOAD_MAC[1].MAC + "'"
                            + " where serial_number = '" + editSerialNumber.Text + "'";
                        var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strUpdate,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    if (shippingsn == "")
                    {
                        if (CTNSCAN_SHIPPINGSN == "SSN2")
                        {
                            shippingsn = (sLOAD_SSN[2].sSHIPPINGSN_CODE).Trim().ToUpper() + sLOAD_SSN[2].SSN;
                        }
                        else if (CTNSCAN_SHIPPINGSN == "SSN3")
                        {
                            shippingsn = (sLOAD_SSN[3].sSHIPPINGSN_CODE).Trim().ToUpper() + sLOAD_SSN[3].SSN;
                        }
                        else if (CTNSCAN_SHIPPINGSN == "SSN4")
                        {
                            shippingsn = (sLOAD_SSN[4].sSHIPPINGSN_CODE).Trim().ToUpper() + sLOAD_SSN[4].SSN;
                        }

                        if ((shippingsn == "") && (editSerialNumber.Text).Length > 7)
                        {
                            shippingsn = editSerialNumber.Text;
                        }
                        if (shippingsn.Trim() != "")
                        {
                            try
                            {
                                string strUpdate = "update sfism4.r_wip_tracking_t set"
                                    + " shipping_sn = '" + shippingsn + "' , "
                                    + " shipping_sn2 = '" + (sLOAD_MAC[1].sMACID_CODE).Trim().ToUpper() + sLOAD_MAC[1].MAC + "'"
                                    + " where serial_number = '" + editSerialNumber.Text + "'";
                                var Update = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = strUpdate,
                                    SfcCommandType = SfcCommandType.Text
                                });
                            }
                            catch (Exception)
                            {
                                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                    }
                }
            }
            try
            {
                string strDelete = "delete FROM SFISM4.R_TMP_ATEDATA_T"
                   + " where serial_number='" + editSerialNumber.Text + "' "
                   + " and group_name='" + mgroup + "'";
                var Delete = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strDelete,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            catch (Exception)
            {
                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        public async Task callTestInput()
        {
            string sWorkSection;
            string sDate, sTime;

            string strGetDateTime = "Select SysDate DateTime,To_Char(SysDate,'yyyymmdd') Mo_Date, "
                 + " To_Char(SysDate, 'hh24mi') Time "
                 + " From Dual";
            var qry_DateTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDateTime,
                SfcCommandType = SfcCommandType.Text
            });
            DateTime datetime = Convert.ToDateTime(qry_DateTime.Data["datetime"].ToString());
            sDate = qry_DateTime.Data["mo_date"].ToString();
            sTime = qry_DateTime.Data["time"].ToString();

            try
            {
                string strGetWorkTime = "select * from sfis1.c_work_desc_t where start_time <= :sTime and end_time >= :sTime and line_name = :sLine";
                var qry_WorkTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetWorkTime,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="sTime",Value=DateTime.Now.ToString("hhmm") },
                        new SfcParameter {Name="sLine",Value=Edt_lineName.Text }
                    }
                });
                if (qry_WorkTime.Data.Count() != 0)
                {
                    sWorkSection = qry_WorkTime.Data["work_section"].ToString();
                }
                else
                {
                    sWorkSection = "X";
                    return;
                }

                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.TEST_INPUT",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="EMP",Value=empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Line",Value=Edt_lineName.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Section",Value=M_sThisSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="W_Station",Value=M_sThisStation,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="DateTime",Value= datetime,SfcParameterDataType=SfcParameterDataType.Date,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="EC",Value="N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Mo_Date",Value=sDate,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="MyGroup",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="W_Section",Value=sWorkSection,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Data",Value=editSerialNumber.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
            }
            catch (Exception e)
            {
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + e.Message;
                _er.MessageEnglish = "Excute procedure have error:" + e.Message;
                _er.ShowDialog();
            }
        }
        private async void InputData_KeyDown(object sender, KeyEventArgs e)
        {
            if (_Inputdata == "UNDO")
            {
                CHECKSN_OK = false;
                editSerialNumber.Text = "";
                if ((cb_rework.IsChecked == true) && (cb_rework.IsVisible == true))
                {
                    _Inputdata = "";
                }
                for (int i = 1; i <= 30; i++)
                {
                    sLOAD_SSN[i].SSN_OK = false;
                    sLOAD_SSN[i].SSN = "";
                }

                for (int i = 1; i <= 5; i++)
                {
                    sLOAD_MAC[i].MAC_OK = false;
                    sLOAD_MAC[i].MAC = "";
                }

                Data_gridView.ItemsSource = null;
                sequenlist.Clear();
                SCAN_POS = 0;
                lbError.Text = "000 - Please Input Data";
                Next_Step = "";
                _Inputdata = "";
                InputData.Focus();
                return;
            }
            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(InputData.Text))
                {
                    if (item_ASN.IsChecked)
                    { 
                        if(Next_Step == "" || Next_Step ==null)
                        {
                            InputData.Text = "@" + InputData.Text;
                        }
                    }
                    if(item_ASSN1.IsChecked==true)
                    {
                        string val = "SN";
                        if (Next_Step != "" && Next_Step != null)
                        {
                            val = Next_Step.Substring(Next_Step.IndexOf('_') + 1);
                        }
                        if(val=="SSN1") InputData.Text = "@" + InputData.Text;
                    }
                    if (item_Nocheck.IsChecked == false)
                    {
                        if (item_Setsn.IsChecked == true)
                        {
                            string val = "SN";
                            if (Next_Step != "" && Next_Step != null)
                            {
                                val = Next_Step.Substring(Next_Step.IndexOf('_') + 1);
                            }
                            InputData.Text = getInputdataWithINI(InputData.Text, val);
                        }
                    }
                    if (item_showdatainput.IsChecked)
                    {
                        string strGetinputdata = $"select SFIS1.Z_PKG.get_chr('{InputData.Text}') from dual";
                        var qry_showdata = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetinputdata,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_showdata.Data != null)
                        {
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "Du lieu nhap vao: '" + qry_showdata.Data.Values.FirstOrDefault().ToString() + "' ";
                            _er.MessageEnglish = "Data input: '" + qry_showdata.Data.Values.FirstOrDefault().ToString() + "'";
                            _er.ShowDialog();
                        }

                    }

                    _Inputdata = InputData.Text;
                    InputData.Text = "";
                    if (cb_custsn.IsChecked == true && SCAN_POS == 0)
                    {
                        await EdmacInputData();
                        await mInputData();
                    }
                    else if (cb_custsn.IsChecked == true && SCAN_POS > 0)
                    {
                        await mInputData();
                    }
                    else if(item_labelcombine1.IsChecked==true)
                    {
                        var lst = _Inputdata.Split(',');
                        if (lst.Length > 4)
                        {
                            try
                            {
                                inQR = _Inputdata;
                                inPN = lst[0].ToString();
                                inSN = lst[1].ToString();
                                inSSN = lst[2].ToString();
                                if (Next_Step == "" || Next_Step is null)
                                {
                                    _Inputdata = inSN;
                                }
                                zExcute:
                                await mInputData();
                                if (inputFlag)
                                {
                                    if (Next_Step.EndsWith("MAC1"))
                                    {
                                        _Inputdata = inSN;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN1"))
                                    {
                                        _Inputdata = inSSN;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN2"))
                                    {
                                        _Inputdata = inQR;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN3"))
                                    {
                                        _Inputdata = inPN;
                                        goto zExcute;
                                    }
                                }
                            }
                            catch
                            {
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "Du lieu nhap vao sai dinh dang labelcombine: '" + _Inputdata + "' ";
                                _er.MessageEnglish = "Labelcombine input wrong format: '" + _Inputdata + "'";
                                _er.ShowDialog();
                            }
                        }
                        else
                        {
                            await mInputData();
                        }
                    }
                    else if (item_labelcombine2.IsChecked == true)
                    {
                        var lst = _Inputdata.Split(',');
                        if (lst.Length > 4)
                        {
                            try
                            {
                                inQR = _Inputdata;
                                inPN = lst[0].ToString();
                                inSSN = lst[1].ToString();
                                inSN = lst[2].ToString();
                                if (Next_Step == "" || Next_Step is null)
                                {
                                    _Inputdata = inSN;
                                }
                                zExcute:
                                await mInputData();
                                if (inputFlag)
                                {
                                    if (Next_Step.EndsWith("MAC1"))
                                    {
                                        _Inputdata = inSN;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN1"))
                                    {
                                        _Inputdata = inSSN;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN2"))
                                    {
                                        _Inputdata = inQR;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN3"))
                                    {
                                        _Inputdata = inPN;
                                        goto zExcute;
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "Du lieu nhap vao sai dinh dang labelcombine: '" + _Inputdata + "' ";
                                _er.MessageEnglish = "Labelcombine input wrong format: '" + _Inputdata + "'";
                                _er.ShowDialog();
                            }
                        }
                        else
                        {
                            await mInputData();
                        }
                    }
                    else if (item_labelcombine3.IsChecked == true)
                    {
                        var lst = _Inputdata.Split(',');
                        if (lst.Length > 4)
                        {
                            try
                            {
                                inQR = _Inputdata;
                                inPN = lst[0].ToString();
                                inSN = lst[1].ToString();
                                if (Next_Step == "" || Next_Step is null)
                                {
                                    _Inputdata = inSN;
                                }
                                zExcute:
                                await mInputData();
                                if (inputFlag)
                                {
                                    if (Next_Step.EndsWith("MAC1"))
                                    {
                                        _Inputdata = inSN;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN2"))
                                    {
                                        _Inputdata = inQR;
                                        goto zExcute;
                                    }
                                    if (Next_Step.EndsWith("SSN3"))
                                    {
                                        _Inputdata = inPN;
                                        goto zExcute;
                                    }
                                }
                            }
                            catch
                            {
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "Du lieu nhap vao sai dinh dang labelcombine: '" + _Inputdata + "' ";
                                _er.MessageEnglish = "Labelcombine input wrong format: '" + _Inputdata + "'";
                                _er.ShowDialog();
                            }
                        }
                        else
                        {
                            await mInputData();
                        }
                    }
                    //else if ((cb_chk1.IsChecked == true) && (cb_rework.IsChecked == true) && SCAN_POS == 0)
                    //{
                    //    await EdmacInputData();
                    //    await mInputData();
                    //}
                    //else if ((cb_chk1.IsChecked == true) && (cb_rework.IsChecked == true) && SCAN_POS > 0)
                    //{
                    //    await mInputData();
                    //}
                    else if ((cb_rework.IsChecked == true) && SCAN_POS == 0)
                    {
                        await EdmacInputData();
                        await mInputData();
                    }
                    else if ((cb_rework.IsChecked == true) && SCAN_POS > 0)
                    {
                        await mInputData();
                    }
                    else
                    {
                        await mInputData();
                    }
                }
                else
                {
                    lbError.Text = "Data not empty!";
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Vui long nhap du lieu!";
                    _er.MessageEnglish = "Data not empty!";
                    _er.ShowDialog();
                }
            }
        }
        public async Task mInputData()
        {
            string MYSN, camSN, hold_reason, TEMP2, myUPCEANDATA, MODEL_NAME, serial_number, MODEL_TYPE;
            string sMO_Number;
            int i, j, adb_I, K;
            bool isNeedCheckDupCam;
            inputFlag = false;

            if (CAPS_LOCK.IsChecked == true)
            {
                _Inputdata = _Inputdata.ToUpper();
            }

            if ((item_scan.IsChecked == true) && SCAN_POS == 0)
            {
                string strGetSSN1 = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SSN1='{_Inputdata}'";
                var qry_SSN1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetSSN1,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_SSN1.Data != null)
                {
                    _Inputdata = qry_SSN1.Data["serial_number"].ToString();
                }
                else
                {
                    lbError.Text = "Pls Scan SSN!";
                    ShowMessageForm _er = new ShowMessageForm();
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Vui long Scan SSN!";
                    _er.MessageEnglish = "Pls Scan SSN!";
                    _er.ShowDialog();
                    InputData.Focus();
                    return;
                }
            }

            string strGetShippingSN = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SHIPPING_SN='{_Inputdata}' and rownum=1";
            var qry_ShippingSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetShippingSN,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ShippingSN.Data != null)
            {
                MODEL_NAME = qry_ShippingSN.Data["model_name"].ToString();
                serial_number = qry_ShippingSN.Data["serial_number"].ToString();

                string strGetModel = $"SELECT * FROM SFIS1.C104 WHERE MODEL_NAME = '{MODEL_NAME}'";
                var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetModel,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Model.Data != null)
                {
                    MODEL_TYPE = qry_Model.Data["model_type"]?.ToString() ?? "";
                    if (MODEL_TYPE.IndexOf("245") > -1)
                    {
                        _Inputdata = serial_number;
                    }
                }
                else
                {
                    lbError.Text = "Model have not in DB. Call IE setup config 6!";
                    ShowMessageForm _er = new ShowMessageForm();
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Ten hang chua duoc thiet lap tren config 6,lien he IE!";
                    _er.MessageEnglish = "Model have not in DB. Call IE setup config 6!";
                    _er.ShowDialog();
                    InputData.Focus();
                    return;
                }
            }

            //Fix de phan biet ROKU
            string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
            var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strChkROKU,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ChkROKU.Data.Count() > 0)
            {
                if (item_automation.IsChecked == true)
                {
                    MessageBox.Show("Please Uncheck Automation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (_Inputdata.IndexOf(":") != -1)
                {
                    K = _Inputdata.IndexOf(":");
                    sn_temp = _Inputdata;
                    _Inputdata = _Inputdata.Substring(0, K);
                }
            }
            /*
            if ((_Inputdata.IndexOf("SN") != -1) && (_Inputdata.Length == 15))
            {
                K = _Inputdata.IndexOf("SN");
                _Inputdata = _Inputdata.Substring(K + 3, _Inputdata.Length);
            }*/
            MYSN = editSerialNumber.Text;
            if (_Inputdata == "UNDO")
            {
                CHECKSN_OK = false;
                editSerialNumber.Text = "";
                if ((cb_rework.IsChecked == true) && (cb_rework.IsVisible == true))
                {
                    InputData.Text = "";
                }
                for (i = 1; i <= 30; i++)
                {
                    sLOAD_SSN[i].SSN_OK = false;
                    sLOAD_SSN[i].SSN = "";
                }

                for (i = 1; i <= 5; i++)
                {
                    sLOAD_MAC[i].MAC_OK = false;
                    sLOAD_MAC[i].MAC = "";
                }

                Data_gridView.ItemsSource = null;
                sequenlist.Clear();
                SCAN_POS = 0;
                lbError.Text = "000 - Please Input Data";
                Next_Step = "";
                InputData.Text = "";
                InputData.Focus();
                return;
            }

            string strGetTrayNo = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='{_Inputdata}' AND ROWNUM=1";
            var qry_TrayNo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetTrayNo,
                SfcCommandType = SfcCommandType.Text
            });
            if ((qry_TrayNo.Data != null) && (M_sThisGroup == "PACK_BOXII"))
            {
                BOXID = _Inputdata;
                BOXID_FLAG = true;
                _Inputdata = qry_TrayNo.Data["serial_number"].ToString();
            }

            isNeedCheckDupCam = false;
            if (CHECKSN_OK == false)
            {
                if (await CHECKNOHHINPUT(_Inputdata) == false)
                {
                    return;
                }
                else
                {
                    Next_Step = "";
                    await CheckSP(_Inputdata);
                    Err_Msg = await CheckSN(_Inputdata, CHECKSN_OK);
                    if (CHECKSN_OK == true)
                    {
                        //sequenlist[SCAN_POS].SN = _Inputdata;
                        //SCAN_POS = SCAN_POS + 1;
                        //Data_gridView.Items.Refresh();
                        //editSerialNumber.Text = _Inputdata;

                        if (qry_ChkROKU.Data.Count() == 0)
                        {
                            //KhoaPD add for check duplicate camera in 2018-01-13
                            string strGetModelType = $"SELECT MODEL_TYPE FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='{Edt_modelName.Text}'";
                            var qry_ModelType = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetModelType,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_ModelType.Data["model_type"].ToString().IndexOf("G18") > -1)
                            {
                                isNeedCheckDupCam = true;
                            }
                            if (isNeedCheckDupCam == false)
                            {
                                string strGetKeyPart = $"SELECT * FROM SFIS1.C_BOM_KEYPART_T A,SFIS1.C_KEYPARTS_DESC_T B WHERE A.KEY_PART_NO=B.KEY_PART_NO AND B.KP_NAME LIKE'CAMERA%' AND A.BOM_NO='{Edt_modelName.Text}'";
                                var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetKeyPart,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_KeyPart.Data.Count() > 0)
                                {
                                    isNeedCheckDupCam = true;
                                }
                            }
                            if (isNeedCheckDupCam == true)
                            {
                                string strGetData = "SELECT B.KEY_PART_SN FROM SFISM4.R_WIP_TRACKING_T A, SFISM4.R_WIP_KEYPARTS_T B,SFIS1.C_KEYPARTS_DESC_T C,SFIS1.C_BOM_KEYPART_T D"
                                   + " WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER  AND C.KP_NAME LIKE 'CAMERA%' AND D.KEY_PART_NO =C.KEY_PART_NO"
                                   + " AND SUBSTR(B.KEY_PART_NO,1,LENGTH(D.KEY_PART_NO)) =D.KEY_PART_NO AND D.BOM_NO=A.MODEL_NAME"
                                   + $" AND (A.SERIAL_NUMBER='{_Inputdata}' OR A.SHIPPING_SN='{_Inputdata}' OR A.SHIPPING_SN2='{_Inputdata}')";
                                var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetData,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_Data.Data != null)
                                {
                                    camSN = qry_Data.Data["key_part_sn"].ToString();
                                    string strGetKeyPartSN = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_SN='{camSN}'";
                                    var qry_KeyPartSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetKeyPartSN,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_KeyPartSN.Data.Count() > 1)
                                    {
                                        sequenlist[SCAN_POS].SN = "";
                                        editSerialNumber.Text = "";
                                        CHECKSN_OK = false;
                                        MYSN = "";
                                        lbError.Text = "Keypart CAMERA '" + camSN + "' da duoc su dung trong ban khac. Hay check lai !";
                                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                        _er.CustomFlag = true;
                                        _er.MessageVietNam = "Keypart CAMERA '" + camSN + "' da duoc su dung trong ban khac. Hay check lai !";
                                        _er.MessageEnglish = "Keypart CAMERA '" + camSN + "' da duoc su dung trong ban khac. Hay check lai !";
                                        _er.ShowDialog();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        lbError.Text = Err_Msg;
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = Err_Msg;
                        _er.MessageVietNam = Err_Msg;
                        _er.ShowDialog();
                        return;
                    }
                }
            }
            if (qry_ChkROKU.Data.Count() > 0)
            {
                //Check cong lenh  hang pear city demo 579 2021-04-02
                if (Next_Step.IndexOf("SSN") != -1)
                {
                    string strGetParameter = $"select * from sfis1.C_PARAMETER_INI where prg_name = 'PACK_BOXII' and vr_value = '{Edt_modelName.Text}'";
                    var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetParameter,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (qry_Parameter.Data != null)
                    {
                        sMO_Number = "";
                        if ((Next_Step.IndexOf("SSN6") > -1) && (Next_Step.IndexOf("SSN8") > -1))
                        {
                            string strGetCustSNN1 = $"select mo_number from sfism4.R_CUSTSN_T where ssn1 = '{_Inputdata}'";
                            var qry_CustSNN1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetCustSNN1,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_CustSNN1.Data != null)
                            {
                                sMO_Number = qry_CustSNN1.Data["mo_number"].ToString();
                            }
                            string strGetParam = $"select * from sfis1.C_PARAMETER_INI where vr_item = 'SSN6' and vr_name = '{sMO_Number}'";
                            var qry_Param = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetParam,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_Param.Data == null)
                            {
                                ShowMessageForm _er = new ShowMessageForm();
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "KHONG PHAI BAN BEN TRAI";
                                _er.MessageEnglish = "Not the product on the Left";
                                _er.ShowDialog();
                                return;
                            }
                        }

                        if ((Next_Step.IndexOf("SSN7") > -1) && (Next_Step.IndexOf("SSN9") > -1))
                        {
                            string strGetCustSNN1 = $"select mo_number from sfism4.R_CUSTSN_T where ssn1 = '{_Inputdata}'";
                            var qry_CustSNN1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetCustSNN1,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_CustSNN1.Data != null)
                            {
                                sMO_Number = qry_CustSNN1.Data["mo_number"].ToString();
                            }
                            string strGetParam = $"select * from sfis1.C_PARAMETER_INI where vr_item = 'SSN7' and vr_name = '{sMO_Number}'";
                            var qry_Param = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetParam,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_Param.Data == null)
                            {
                                ShowMessageForm _er = new ShowMessageForm();
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "KHONG PHAI BAN BEN PHAI";
                                _er.MessageEnglish = "Not the product on the Right";
                                _er.ShowDialog();
                                return;
                            }
                        }
                    }

                    //QUAN CHE SCAN MA SSN1 CUA CAC HANG DUOC THIET LAP  ==>PQE YEU CAU
                    string strGetEtEConfig = $"SELECT * FROM SFIS1.C_ETE_CONFIG_T WHERE GROUP_NAME='PACK_BOX' AND TYPE in ('SCAN_1D','SCAN 1D','SCAN1D') AND MODEL_NAME ='{Edt_modelName.Text}'";
                    var qry_EtEConfig = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetEtEConfig,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_EtEConfig.Data.Count() > 0)
                    {
                        if (Next_Step.IndexOf("SSN1") != -1)
                        {
                            if (!string.IsNullOrEmpty(sn_temp))
                            {
                                if (sn_temp.IndexOf(":") != -1)
                                {
                                    lbError.Text = $"SSN1 CUA HANG {Edt_modelName.Text} PHAI SAO MA LABEL 1D";
                                    ShowMessageForm _er = new ShowMessageForm();
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = $"SSN1 CUA HANG {Edt_modelName.Text} PHAI SAO MA LABEL 1D";
                                    _er.MessageEnglish = $"SSN1 OF {Edt_modelName.Text} NEED SCAN 1D BARCODE";
                                    _er.ShowDialog();
                                    return;
                                }
                            }
                        }
                    }
                }

                //Begin check dup data,sua cho hang 559
                if (Next_Step.IndexOf("SSN") != -1)
                {
                    string strGetParameter = $"select * from sfis1.C_PARAMETER_INI where prg_name = 'PACK_BOX' and vr_value = '{Edt_modelName.Text}'";
                    var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetParameter,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Parameter.Data != null)
                    {
                        if (qry_Parameter.Data["vr_item"].ToString().IndexOf(Next_Step) > -1)
                        {
                            chk_ssn6 = _Inputdata;
                        }
                        if (qry_Parameter.Data["vr_name"].ToString().IndexOf(Next_Step) > -1)
                        {
                            chk_ssn7 = _Inputdata;
                        }

                        if ((chk_ssn6 != "") && (chk_ssn7 != "") && (chk_ssn6 == chk_ssn7))
                        {
                            ShowMessageForm _er = new ShowMessageForm();
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "Du lieu trung lap";
                            _er.MessageEnglish = "DATA SCAN DUP";
                            _er.ShowDialog();
                            return;
                        }
                    }
                }
            }
            
            if ((sOTHER_USE_ASSN[1].USED == true) && (sOTHER_USE_ASSN[1].checked1 = false))
            {
                if (sOTHER_USE_ASSN[1].checked1 == false)
                {
                    Err_Msg = await Chk_OTHER_USE_ASSN(_Inputdata, sOTHER_USE_ASSN[1].USEKEY);
                    if (Err_Msg != "OK")
                    {
                        lbError.Text = "CONFIG 44 SN=" + sOTHER_USE_ASSN[1].USEKEY + (sOTHER_USE_ASSN[1].indexid).ToString() + " ; " + Err_Msg;
                    }
                    else
                    {
                        sOTHER_USE_ASSN[1].checked1 = true;
                    }
                }
            }

            if (Next_Step.IndexOf("SSN") > -1)
            {
                SSNSTRING = Next_Step.Substring(8);
                if (await checksnchar() == false)
                {
                    InputData.SelectAll();
                    return;
                }

                //SSN10 - SSN30
                if ((Next_Step.Substring(Next_Step.Length - 2, 2) == "10")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "11")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "12")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "13")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "14")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "15")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "16")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "17")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "18")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "18")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "20")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "21")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "22")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "23")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "24")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "25")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "26")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "27")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "28")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "29")
                || (Next_Step.Substring(Next_Step.Length - 2, 2) == "30"))
                {
                    for (i = 10; i <= 30; i++)
                    {
                        if (Next_Step.IndexOf("SSN" + i.ToString().Trim()) > 0)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //SSN1 - SSN9
                    for (i = 1; i <= 9; i++)
                    {
                        if (Next_Step.IndexOf("SSN" + i.ToString().Trim()) > 0)
                        {
                            break;
                        }
                    }
                }

                if (sLOAD_SSN[i].SSN_OK == false)
                {
                    FindFlag();
                    Err_Msg = await ChkSSN(_Inputdata, "SSN" + i.ToString().Trim(), sLOAD_SSN[i].SSN_OK);
                    if (sLOAD_SSN[i].SSN_OK == true)
                    {
                        sequenlist[SCAN_POS].SN = _SSN;
                        SCAN_POS = SCAN_POS + 1;
                        Data_gridView.Items.Refresh();
                        sLOAD_SSN[i].SSN = _SSN;
                        sLOAD_SSN[i].SSN_OK = false;
                    }
                    else
                    {
                        lbError.Text = Err_Msg;
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = Err_Msg;
                        _er.MessageVietNam = Err_Msg;
                        _er.ShowDialog();
                        InputData.Text = "";
                        return;
                    }
                }
                else
                {
                    if ((sLOAD_SSN[i].sSHIPPINGSN_CODE != "") && (_Inputdata.Substring(1, (sLOAD_SSN[i].sSHIPPINGSN_CODE).Length) == sLOAD_SSN[i].sSHIPPINGSN_CODE))
                    {
                        _Inputdata = _Inputdata.Substring((sLOAD_SSN[i].sSHIPPINGSN_CODE).Length + 1, (_Inputdata).Length);
                    }

                    if (sLOAD_SSN[i].SSN == _Inputdata)
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                    }
                    else
                    {
                        if (await ChkCPEI() == false)
                        {
                            string strGetSerialNumber = $"SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER='{MYSN}'";
                            var qry_SerialNumbe = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetSerialNumber,
                                SfcCommandType = SfcCommandType.Text
                            });

                            if ((qry_SerialNumbe.Data["wip_group"].ToString()).Substring(0, 4) == "HOLD")
                            {
                                lbError.Text = "PCB HAVE HOLD!! " + qry_SerialNumbe.Data["wip_group"].ToString();
                                ShowMessageForm er = new ShowMessageForm(this, sfcClient);
                                er.CustomFlag = true;
                                er.MessageEnglish = "PCB HAS BEEN HOLD!! " + qry_SerialNumbe.Data["wip_group"].ToString();
                                er.MessageVietNam = "PCB da HOLD!! " + qry_SerialNumbe.Data["wip_group"].ToString();
                                er.ShowDialog();
                                return;
                            }
                            else
                            {
                                hold_reason = "SYSTEM AUTO HOLD WHEN SCAN DATA NOT MATCH AT PACK_BOX, '" + sLOAD_SSN[i].sSHIPPINGSN_CODE + "': '" + sLOAD_SSN[i].SSN + "' <> '" + _Inputdata + "'";
                                try
                                {
                                    string strUpdate = $"update sfism4.r107 set GROUP_NAME='HOLD-'||GROUP_NAME, NEXT_STATION='HOLD-'||NEXT_STATION, WIP_GROUP='HOLD-'||WIP_GROUP,EMP_NO='AUTO-HOLD' where serial_number='{MYSN}'";
                                    var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strUpdate,
                                        SfcCommandType = SfcCommandType.Text
                                    });

                                    string strInsert = "INSERT INTO SFISM4.R_SYSTEM_HOLD_T (SERIAL_NUMBER,MODEL_NAME,MAIN_DESC,HOLD_EMP_NO,HOLD_TIME,HOLD_REASON,HOLD_PROGRAM,FINISH_FLAG)"
                                        + " VALUES"
                                        + " ('" + MYSN + "','" + Edt_modelName.Text + "','N/A','SYSTEMHOLD',SYSDATE,'" + hold_reason + "','PACK_BOX','0')";
                                    var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strInsert,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                        }
                        lbError.Text = "SHIPPING_SN Not Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "SHIPPING_SN Not Match!!";
                        _er.MessageVietNam = "SHIPPING_SN khong khop!!";
                        _er.ShowDialog();

                        InputData.IsEnabled = false;
                        CHECKSN_OK = false;
                        editSerialNumber.Text = "";
                        if ((cb_rework.IsChecked == true) && (cb_rework.IsVisible == true))
                        {
                            InputData.Text = "";
                        }

                        for (i = 1; i <= 30; i++)
                        {
                            sLOAD_SSN[i].SSN_OK = false;
                            sLOAD_SSN[i].SSN = "";
                        }

                        for (i = 1; i <= 5; i++)
                        {
                            sLOAD_MAC[i].MAC_OK = false;
                            sLOAD_MAC[i].MAC = "";
                        }

                        for (i = 0; i <= 3; i++)
                        {
                            Data_gridView.Items.Clear();
                        }

                        Next_Step = "";
                        InputData.Text = "";
                        InputData.Focus();
                        lbError.Text = "000 - Please Input Data";
                    }
                }
            }

            if (Next_Step.IndexOf("MAC") > -1)
            {
                for (i = 1; i <= 5; i++)
                {
                    if (Next_Step.IndexOf("MAC" + Int32.Parse(i.ToString().Trim())) > -1)
                    {
                        break;
                    }
                }

                //if (item_Setsn.IsChecked == true)
                //{
                //    InputData.Text = getInputdataWithINI(InputData.Text, "MAC" + Int32.Parse(i.ToString().Trim()));
                //}

                FindFlag();
                if (M_modeltype.IndexOf("051") > -1)
                {
                    if ((_Inputdata).Length == 14)
                    {
                        _Inputdata = _Inputdata.Substring(0, 4) + _Inputdata.Substring(5, 4) + _Inputdata.Substring(10, 4);
                    }
                }

                if (M_modeltype.IndexOf("G67") > -1)
                {
                    if ((_Inputdata).Length == 53)
                    {
                        _Inputdata = _Inputdata.Substring(36, 2) + _Inputdata.Substring(39, 2) + _Inputdata.Substring(42, 2) + _Inputdata.Substring(45, 2) + _Inputdata.Substring(48, 2) + _Inputdata.Substring(51, 2);
                    }
                    else
                    {
                        if ((_Inputdata).Length == 54)
                        {
                            _Inputdata = _Inputdata.Substring(37, 2) + _Inputdata.Substring(40, 2) + _Inputdata.Substring(43, 2) + _Inputdata.Substring(46, 2) + _Inputdata.Substring(49, 2) + _Inputdata.Substring(52, 2);
                        }

                    }
                }

                if (sLOAD_MAC[i].MAC_OK == false)
                {
                    FindFlag();

                    Err_Msg = await ChkMAC(_Inputdata, "MAC" + i.ToString().Trim(), sLOAD_MAC[i].MAC_OK);
                    if (sLOAD_MAC[i].MAC_OK == true)
                    {
                        sequenlist[SCAN_POS].SN = _MAC;
                        SCAN_POS = SCAN_POS + 1;
                        Data_gridView.Items.Refresh();
                        sLOAD_MAC[i].MAC = _MAC;
                        sLOAD_MAC[i].MAC_OK = false;
                    }
                    else
                    {
                        lbError.Text = Err_Msg;
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = Err_Msg;
                        _er.MessageVietNam = Err_Msg;
                        _er.ShowDialog();
                        InputData.Text = "";
                        return;
                    }
                }
                else
                {
                    if ((sLOAD_MAC[i].sMACID_CODE != "") && (_Inputdata.Substring(1, sLOAD_MAC[i].sMACID_CODE.Length)) == sLOAD_MAC[i].sMACID_CODE)
                    {
                        _Inputdata = _Inputdata.Substring((sLOAD_MAC[i].sMACID_CODE).Length + 1, (_Inputdata).Length);
                    }

                    if (sLOAD_MAC[i].MAC == _Inputdata)
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                    }
                    else
                    {
                        lbError.Text = "MACID Not Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "MACID Not Match!!";
                        _er.MessageVietNam = "MACID khong khop!!";
                        _er.ShowDialog();
                        InputData.Text = "";
                        return;
                    }
                }
            }
            if (qry_ChkROKU.Data.Count() == 0)
            {
                // Lien modify add new function scan orbi small with orbi big
                if (Next_Step.IndexOf("ORBI_CODE") > -1)
                {
                    string strGetOrbiMAC = $"select SUBSTR('{_Inputdata}',instr('{_Inputdata}','MAC:')+4,12) ORBI_MAC from dual";
                    var qry_OrbiMAC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetOrbiMAC,
                        SfcCommandType = SfcCommandType.Text
                    });

                    _Inputdata = qry_OrbiMAC.Data["orbi_ma"].ToString();
                    string strGetKPCamera = $"select * from sfism4.r_wip_keyparts_t where serial_number='{_Inputdata}' and key_part_sn ='{sequenlist[0].SN}' and group_name='ASSY1'";
                    var qry_GetKPCamera = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKPCamera,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_GetKPCamera.Data.Count() == 0)
                    {
                        lbError.Text = "ORBI MAC NOT MATCH WITH THIS PRODUCT!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "ORBI MAC NOT MATCH WITH THIS PRODUCT!";
                        _er.MessageVietNam = "ORBI MAC khong khop voi san pham!";
                        _er.ShowDialog();
                        return;
                    }
                    sequenlist[SCAN_POS].SN = _Inputdata;
                    SCAN_POS = SCAN_POS + 1;
                }
                // Lien modify add new function scan orbi small with orbi big

                //   for negtear Pin_code add by shiliang 20110219  start
                if (Next_Step.IndexOf("PCODE") > -1)
                {
                    string strGetPinCode = $"select * from sfism4.r_wip_keyparts_t where serial_number = '{editSerialNumber.Text}' and carton_no = '{_Inputdata}'";
                    var qry_PinCode = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetPinCode,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_PinCode.Data.Count() != 1)
                    {
                        lbError.Text = "SN and Pin_Code Not Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "SN and Pin_Code Not Match!!";
                        _er.MessageVietNam = "SN and Pin_Code khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }
                //   for negtear  Pin_code  add by shiliang 20110219  end

                if (Next_Step.IndexOf("MCID") > -1)
                {
                    string strGetMACID = $"select * from sfism4.r_wip_keyparts_t where serial_number = '{editSerialNumber.Text}' and  KEY_PART_NO ='MACID' AND  KEY_PART_SN = '{_Inputdata}'";
                    var qry_MACID = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetMACID,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_MACID.Data.Count() != 1)
                    {
                        lbError.Text = "SN and MACID Not Match!!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "SN and MACID Not Match!!!";
                        _er.MessageVietNam = "SN and MACID khong khop!!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }

                // Modified by FIREBIRD 2011-10-13 15:06:25
                if (Next_Step.IndexOf("HDCP") > -1)
                {
                    string strGetHDCP = $"select * from sfism4.r_wip_keyparts_t where serial_number = '{editSerialNumber.Text}' AND KEY_PART_NO ='SSN' AND KEY_PART_SN = '{_Inputdata}'";
                    var qry_HDCP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetHDCP,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_HDCP.Data.Count() != 1)
                    {
                        lbError.Text = "SN and SSN Not Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "SN and SSN Not Match!!";
                        _er.MessageVietNam = "SN and SSN khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }

                // Modified by FIREBIRD 2011-10-13 15:06:37
                if (Next_Step.IndexOf("ADB") > -1)
                {
                    adb_I = Int32.Parse((Next_Step.Substring((Next_Step.IndexOf("ADB") + 3), Next_Step.Length)).Trim());
                    if (adb_I > 0)
                    {
                        string strGetADB = $"SELECT B.SERIAL_NUMBER FROM SFIS1.C_BOM_KEYPART_T A,SFISM4.R_WIP_KEYPARTS_T B WHERE"
                            + $" A.KEY_PART_NO=B.KEY_PART_NO AND A.BOM_NO='{Edt_modelName.Text}' AND A.KP_RELATION='{adb_I}' AND B.SERIAL_NUMBER='{editSerialNumber.Text}' AND B.KEY_PART_SN='{_Inputdata}'";
                        var qry_ADB = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetADB,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_ADB.Data.Count() != 1)
                        {
                            lbError.Text = "NO THIS KP_RELATION " + adb_I.ToString() + " CALL IE CONFIGURE AGAIN!";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "NO THIS KP_RELATION " + adb_I.ToString() + " CALL IE CONFIGURE AGAIN!";
                            _er.MessageVietNam = "Gia tri KP_RELATION " + adb_I.ToString() + " goi IE thiet lap lai!";
                            _er.ShowDialog();
                            return;
                        }
                        else
                        {
                            sequenlist[SCAN_POS].SN = _Inputdata;
                            SCAN_POS = SCAN_POS + 1;
                        }
                    }
                }

                if (Next_Step.IndexOf("SID_R108") > -1)
                {
                    string strGetSID_R108 = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND KEY_PART_NO = 'SSID' AND KEY_PART_SN='{_Inputdata}'";
                    var qry_SID_R108 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetSID_R108,
                        SfcCommandType = SfcCommandType.Text
                    });
                    TEMP2 = qry_SID_R108.Data["key_part_sn"].ToString();
                    if (TEMP2 != _Inputdata)
                    {
                        lbError.Text = "SSID NOT Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "SSID NOT Match!!";
                        _er.MessageVietNam = "SSID khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }

                if (Next_Step.IndexOf("SID2G_R108") > -1)
                {
                    //if (cb_chk1.IsChecked == false)
                    //{
                    //    string strGetSID2G_R108 = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND KEY_PART_NO = 'SSID_2G' AND KEY_PART_SN='{_Inputdata}'";
                    //    var qry_SID2G_R108 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = strGetSID2G_R108,
                    //        SfcCommandType = SfcCommandType.Text
                    //    });
                    //    TEMP2 = qry_SID2G_R108.Data["key_part_sn"].ToString();
                    //}
                    //else
                    //{
                        string strGetCustSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND ROWNUM=1";
                        var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCustSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        TEMP2 = qry_CustSN.Data["ssn3"].ToString();
                   // }
                    if (TEMP2 != _Inputdata)
                    {
                        lbError.Text = "SSID_2G NOT Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "SSID_2G NOT Match!!";
                        _er.MessageVietNam = "SSID_2G khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }

                if (Next_Step.IndexOf("SID5G_R108") > -1)
                {
                    //if (cb_chk1.IsChecked == false)
                    //{
                    //    string strGetSID5G_R108 = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND KEY_PART_NO = 'SSID_5G' AND KEY_PART_SN='{_Inputdata}'";
                    //    var qry_SID5G_R108 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = strGetSID5G_R108,
                    //        SfcCommandType = SfcCommandType.Text
                    //    });
                    //    TEMP2 = qry_SID5G_R108.Data["key_part_sn"].ToString();
                    //}
                    //else
                    //{
                        string strGetCustSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND ROWNUM=1";
                        var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCustSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        TEMP2 = qry_CustSN.Data["ssn7"].ToString();
                    //}
                    if (TEMP2 != _Inputdata)
                    {
                        lbError.Text = "SSID_5G NOT Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "SSID_5G NOT Match!!";
                        _er.MessageVietNam = "SSID_5G khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }

                if (Next_Step.IndexOf("PASSWORD2G_R108") > -1)
                {
                    //if (cb_chk1.IsChecked == false)
                    //{
                    //    string strGetPASSWORD2G_R108 = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND KEY_PART_NO = 'PASSWORD_2G' AND KEY_PART_SN='{_Inputdata}'";
                    //    var qry_PASSWORD2G_R108 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = strGetPASSWORD2G_R108,
                    //        SfcCommandType = SfcCommandType.Text
                    //    });
                    //    TEMP2 = qry_PASSWORD2G_R108.Data["key_part_sn"].ToString();
                    //}
                    //else
                    //{
                        string strGetCustSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND ROWNUM=1";
                        var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCustSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        TEMP2 = qry_CustSN.Data["ssn4"].ToString();
                    //}
                    if (TEMP2 != _Inputdata)
                    {
                        lbError.Text = "PASSWORD_2G NOT Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "PASSWORD_2G NOT Match!!";
                        _er.MessageVietNam = "PASSWORD_2G khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }

                if (Next_Step.IndexOf("PASSWORD5G_R108") > -1)
                {
                    //if (cb_chk1.IsChecked == false)
                    //{
                    //    string strGetPASSWORD5G_R108 = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND KEY_PART_NO = 'PASSWORD_5G' AND KEY_PART_SN='{_Inputdata}'";
                    //    var qry_PASSWORD5G_R108 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    //    {
                    //        CommandText = strGetPASSWORD5G_R108,
                    //        SfcCommandType = SfcCommandType.Text
                    //    });
                    //    TEMP2 = qry_PASSWORD5G_R108.Data["key_part_sn"].ToString();
                    //}
                    //else
                    //{
                        string strGetCustSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND ROWNUM=1";
                        var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCustSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        TEMP2 = qry_CustSN.Data["ssn8"].ToString();
                    //}
                    if (TEMP2 != _Inputdata)
                    {
                        lbError.Text = "PASSWORD5G NOT Match!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "PASSWORD5G NOT Match!!";
                        _er.MessageVietNam = "PASSWORD5G khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }

                if (Next_Step.IndexOf("CASN") > -1)
                {
                    string strGetCASN = "SELECT  * FROM SFISM4.R_WIP_KEYPARTS_T"
                        + $" WHERE SERIAL_NUMBER ='{sequenlist[0].SN}'  AND KEY_PART_SN ='{_Inputdata}' AND KEY_PART_NO='CASN'";
                    var qry_CASN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCASN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CASN.Data.Count() == 0)
                    {
                        lbError.Text = "CASN NOT MATCH!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "CASN NOT MATCH!!";
                        _er.MessageVietNam = "CASN khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    sequenlist[SCAN_POS].SN = _Inputdata;
                    SCAN_POS = SCAN_POS + 1;
                }

                if (Next_Step.IndexOf("CAMERA_SN") > -1)
                {
                    for (j = 0; j < SCAN_POS; j++)
                    {
                        if (sequenlist[j].SN == _Inputdata)
                        {
                            lbError.Text = "CAMERA_SCAN_DUP!!";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "CAMERA_SCAN_DUP!!";
                            _er.MessageVietNam = "CAMERA da Scan!!";
                            _er.ShowDialog();
                            return;
                        }
                    }
                    string strGetData = "SELECT COUNT(*) count FROM SFISM4.R108 A, SFIS1.C_BOM_KEYPART_T B, SFIS1.C_KEYPARTS_DESC_T C, SFISM4.R107 D"
                       + $" WHERE D.SERIAL_NUMBER=A.SERIAL_NUMBER AND D.BOM_NO=B.BOM_NO AND A.SERIAL_NUMBER='{sequenlist[0].SN}' AND"
                       + $" A.KEY_PART_SN='{_Inputdata}' AND B.GROUP_NAME=A.GROUP_NAME AND B.KEY_PART_NO=C.KEY_PART_NO"
                       + " AND C.KP_NAME LIKE 'CAMERA%'";
                    var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetData,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (Int32.Parse(qry_Data.Data["count"].ToString()) == 0)
                    {
                        lbError.Text = "CAMERA_SN NOT MATCH!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "CAMERA_SN NOT MATCH!!";
                        _er.MessageVietNam = "CAMERA_SN khong khop!!";
                        _er.ShowDialog();
                        return;
                    }
                    sequenlist[SCAN_POS].SN = _Inputdata;
                    SCAN_POS = SCAN_POS + 1;
                }

                if (Next_Step.IndexOf("ORBI_AX") > -1)
                {
                    if (is_check_orbi_ax == false)
                    {
                        for (j = 0; j <= SCAN_POS; j++)
                        {
                            if (sequenlist[j].SN == sequenlist[j + 1].SN)
                            {
                                lbError.Text = "ORBI AX SCAN_DUP!!";
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = "ORBI AX SCAN_DUP!!";
                                _er.MessageVietNam = "ORBI AX SCAN_DUP!!";
                                _er.ShowDialog();
                                return;
                            }
                        }
                        is_check_orbi_ax = true;
                        last_orbi_ax_val = _Inputdata;
                    }
                    else
                    {
                        if (_Inputdata != last_orbi_ax_val)
                        {
                            lbError.Text = "ORBI AX SCAN WRONG WITH FIRST";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "ORBI AX SCAN WRONG WITH FIRST";
                            _er.MessageVietNam = "ORBI AX SCAN sai voi gia tri dau tien";
                            _er.ShowDialog();
                            return;
                        }
                        else
                        {
                            is_check_orbi_ax = false;
                        }
                    }
                    string strGetKeyPart = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T"
                             + $" WHERE SERIAL_NUMBER = '{sequenlist[0].SN}' AND KEY_PART_SN = '{_Inputdata}' AND KEY_PART_NO IN('HFCMAC','HFCMAC1')";
                    var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPart,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_KeyPart.Data.Count() == 0)
                    {
                        lbError.Text = "HFCMAC NOT MATCH";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "HFCMAC NOT MATCH";
                        _er.MessageVietNam = "HFCMAC khong khop";
                        _er.ShowDialog();
                        return;
                    }
                    sequenlist[SCAN_POS].SN = _Inputdata;
                    SCAN_POS = SCAN_POS + 1;
                }

                if (Next_Step.IndexOf("LACN") > -1)
                {
                    string strGetLACN = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T"
                             + $" WHERE SERIAL_NUMBER ='{sequenlist[0].SN}' AND KEY_PART_SN ='{_Inputdata}' AND KEY_PART_NO = 'LACN'";
                    var qry_LACN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetLACN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_LACN.Data.Count() == 0)
                    {
                        lbError.Text = "LACN NOT MATCH";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "LACN NOT MATCH";
                        _er.MessageVietNam = "LACN khong khop";
                        _er.ShowDialog();
                        return;
                    }
                    sequenlist[SCAN_POS].SN = _Inputdata;
                    SCAN_POS = SCAN_POS + 1;
                }

                if (Next_Step.IndexOf("HFC") > -1)
                {
                    string strGetHFC = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T"
                             + $" WHERE SERIAL_NUMBER = '{sequenlist[0].SN}' AND KEY_PART_SN = '{_Inputdata}' AND KEY_PART_NO IN('HFCMAC','HFCMAC1')";
                    var qry_HFC = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetHFC,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_HFC.Data.Count() == 0)
                    {
                        lbError.Text = "HFCMAC NOT MATCH";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "HFCMAC NOT MATCH";
                        _er.MessageVietNam = "HFCMAC khong khop";
                        _er.ShowDialog();
                        return;
                    }
                    sequenlist[SCAN_POS].SN = _Inputdata;
                    SCAN_POS = SCAN_POS + 1;
                }

                if (Next_Step.IndexOf("MS_MESH") > -1)
                {
                    for (j = 0; j < SCAN_POS; j++)
                    {
                        if (sequenlist[j].SN == _Inputdata)
                        {
                            lbError.Text = "ORBI MESH_SCAN_DUP!!";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "ORBI MESH_SCAN_DUP!!";
                            _er.MessageVietNam = "ORBI MESH da xao!!";
                            _er.ShowDialog();
                            return;
                        }
                    }
                    string strGetMS_MESH = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T"
                            + $" WHERE SERIAL_NUMBER ='{sequenlist[0].SN}' AND KEY_PART_SN ='{_Inputdata}' AND KEY_PART_NO IN('HFCMAC','HFCMAC1')";
                    var qry_MS_MESH = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetMS_MESH,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_MS_MESH.Data.Count() == 0)
                    {
                        lbError.Text = "MS_MESH NOT MATCH IN SYSTEM!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "MS_MESH NOT MATCH IN SYSTEM!!";
                        _er.MessageVietNam = "MS_MESH khong khop voi he thong!!";
                        _er.ShowDialog();
                        return;
                    }
                    sequenlist[SCAN_POS].SN = _Inputdata;
                    SCAN_POS = SCAN_POS + 1;
                }

                if (Next_Step.IndexOf("IMEI") > -1)
                {
                    string strGetIMEI = $"select * from sfism4.r_wip_keyparts_t A,sfism4.r_net_3gmodule_t B where A.key_part_sn=B.sn and B.type='OUT' and A.serial_number='{editSerialNumber.Text}' and A.key_part_sn='{_Inputdata}' and A.key_part_no='IMEI'";
                    var qry_IMEI = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetIMEI,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_IMEI.Data.Count() == 0)
                    {
                        lbError.Text = "IMEI ERROR!!";
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "IMEI ERROR!!";
                        _er.MessageVietNam = "IMEI Loi!!";
                        _er.ShowDialog();
                        return;
                    }
                    else
                    {
                        sequenlist[SCAN_POS].SN = _Inputdata;
                        SCAN_POS = SCAN_POS + 1;
                    }
                }
            }

            if (CHECKSN_OK == false)
            {
                return;
            }

            if (SCAN_POS < sequenlist.Count)
            {
                Next_Step = sequenlist[SCAN_POS].STEP;
                InputData.Focus();
                lbError.Text = await GetPubMessage("00410") + " " + Next_Step;
                inputFlag = true;
                return;
            }
            else
            {
                try
                {
                    //tru lieu
                    string strquery = $"SELECT * FROM SFISM4.R117 where SERIAL_NUMBER ='{editSerialNumber.Text}' and MO_NUMBER ='{Edt_moNumber.Text}' and GROUP_NAME ='PACK_BOX'";
                    var qry_r117 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strquery,
                        SfcCommandType = SfcCommandType.Text
                    });

                    string strquery1 = $"SELECT * " +
                        $"from (SELECT TRIM(REGEXP_SUBSTR (group_Name,'[^,]+',1,LEVEL)) group_Name " +
                        $"FROM(select ATTRIBUTE_DESC1 as group_Name from SFIS1.C_MODEL_ATTR_CONFIG_T " +
                        $"where ATTRIBUTE_NAME = 'NO_CHECK_ALLPARTS' AND TYPE_VALUE = '{Edt_modelName.Text}' and ATTRIBUTE_VALUE = '{Edt_moNumber.Text}') T " +
                        $"CONNECT BY INSTR(group_Name,',',1,LEVEL - 1) > 0 ) where group_name = 'PACK_BOX'";
                    var qry_nocheckap = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strquery1,
                        SfcCommandType = SfcCommandType.Text
                    });

                    string strGetstation = $"SELECT * FROM (SELECT Trim(Regexp_substr(group_name, '[^,]+', 1, LEVEL)) group_name FROM(SELECT ATTRIBUTE_VALUE AS group_name FROM   SFIS1.C_MODEL_ATTR_CONFIG_T WHERE  ATTRIBUTE_NAME = 'NIC_CHECK_ALLPARTS' AND type_value = '{Edt_modelName.Text}') CONNECT BY Instr(group_name, ',', 1, LEVEL - 1) > 0 ORDER  BY group_name) WHERE group_name = 'PACK_BOX'";
                    var qry_checkap = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetstation,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (qry_checkap.Data.Count() != 0 && qry_r117.Data.Count() == 0 && qry_nocheckap.Data.Count() == 0)
                    {
                        try
                        {
                            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = "SFIS1.MODIFY_PTH_WIP_QTY_F12",
                                SfcCommandType = SfcCommandType.StoredProcedure,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="G_LINE",Value=Edt_lineName.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="G_WO",Value=Edt_moNumber.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="G_SN",Value=editSerialNumber.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="G_GROUP",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                }
                            });

                            dynamic ads = result.Data;
                            res = ads[0]["res"];
                            if (res.Substring(0, 2) != "OK")
                            {
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "ERROR: (AP) " + res;
                                _er.MessageEnglish = "ERROR: (AP) " + res;
                                _er.ShowDialog();

                                lbError.Text = res;
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + e.Message;
                            _er.MessageEnglish = "Excute procedure have error:" + e.Message;
                            _er.ShowDialog();
                        }
                    }

                    await updateR107();
                    if (BOXID_FLAG == true)
                    {
                        string strGetTray = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='{BOXID}'";
                        var qry_Tray = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetTrayNo,
                            SfcCommandType = SfcCommandType.Text
                        });
                        foreach (var row in qry_Tray.Data)
                        {
                            editSerialNumber.Text = row["serial_number"].ToString();
                            await callTestInput();
                        }
                    }
                    else
                    {
                        await callTestInput();
                    }

                    MYSN = editSerialNumber.Text;
                    lbError.Text = await GetPubMessage("00135");
                    Next_Step = "";
                    MACID = "";
                    CHECKSN_OK = false;
                    for (i = 1; i <= 30; i++)
                    {
                        sLOAD_SSN[i].SSN_OK = false;
                        sLOAD_SSN[i].SSN = "";
                    }

                    for (i = 1; i <= 5; i++)
                    {
                        sLOAD_MAC[i].MAC_OK = false;
                        sLOAD_MAC[i].MAC = "";
                    }
                    editSerialNumber.Text = "";
                    Data_gridView.ItemsSource = null;
                    sequenlist.Clear();
                    SCAN_POS = 0;
                    InputData.Focus();
                    BOXID_FLAG = false;
                    BOXID = "";
                    if ((cb_rework.IsChecked == true) && (cb_custsn.IsVisible == true))
                    {
                        InputData.Text = "";
                        InputData.Focus();
                    }
                    //Check Config19
                    string strGetC19 = "select  count(*) from sfis1.c_cust_snrule_t a,sfism4.r107 b where"
                                   + " a.model_name=b.model_name and a.version_code=b.version_code and a.cust_code=b.customer_no"
                                   + $" and b.serial_number='{MYSN}'";

                    var qry_c19 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetC19,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_c19.Data.Count() == 0)
                    {
                        lbError.Text = await GetPubMessageVN("00084");
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = await GetPubMessage("00084");
                        _er.MessageVietNam = await GetPubMessageVN("00084");
                        _er.ShowDialog();
                    }
                    else
                    {
                        string strGetUPC = "select a.UPCEANDATA from sfis1.c_cust_snrule_t a,sfism4.r107 b where"
                          + " a.model_name=b.model_name and a.version_code=b.version_code and a.cust_code=b.customer_no"
                          + $" and b.serial_number='{MYSN}'";
                        var qry_UPC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetUPC,
                            SfcCommandType = SfcCommandType.Text
                        });
                        myUPCEANDATA = qry_UPC.Data["upceandata"]?.ToString() ?? "";
                        if (myUPCEANDATA != "")
                        {
                            try
                            {
                                string strUpdate = "update sfism4.r107 set track_no=nvl(track_no,"
                                + " (select a.UPCEANDATA  from sfis1.c_cust_snrule_t a,sfism4.r107 b where"
                                + " a.model_name=b.model_name and a.version_code=b.version_code and a.cust_code=b.customer_no"
                                + " and b.serial_number=:sn)) where serial_number=:sn";
                                var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strUpdate,
                                    SfcCommandType = SfcCommandType.Text,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter {Name="sn",Value=MYSN }
                                    }
                                });
                            }
                            catch (Exception)
                            {
                                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            
                            if (M_modeltype.IndexOf("221") > -1)
                            {
                                string strGetTMP = "SELECT SERIAL_NUMBER FROM SFISM4.R107 A WHERE A.SHIPPING_SN IN ("
                                    + " select KEY_PART_SN from sfism4.r_wip_keyparts_t B where B.serial_number='" + MYSN + "' +  and group_name='KEYPART' AND PART_MODE in( "
                                    + " select substr('" + Edt_modelName.Text + "',1,instr('" + Edt_modelName.Text + "','-')-1)  from dual))";
                                var qry_TMP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetTMP,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_TMP.Data != null)
                                {
                                    try
                                    {
                                        string strUpdate = "update sfism4.r107 set track_no=nvl(track_no,"
                                            + " (select a.UPCEANDATA  from sfis1.c_cust_snrule_t a,sfism4.r107 b where"
                                            + " a.model_name=b.model_name and a.version_code=b.version_code and a.cust_code=b.customer_no"
                                            + " and b.serial_number=:sn)) where serial_number='" + qry_TMP.Data["serial_number"].ToString() + "'";
                                        var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strUpdate,
                                            SfcCommandType = SfcCommandType.Text,
                                            SfcParameters = new List<SfcParameter>()
                                            {
                                                new SfcParameter {Name="sn",Value=MYSN }
                                            }
                                        });
                                    }
                                    catch (Exception)
                                    {
                                        MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    if (item_automation.IsChecked == true)
                    {
                        FormKeyPart frm_keypart = new FormKeyPart(this, sfcClient);
                        frm_keypart.ShowDialog();
                        InputData.Text = "";
                        InputData.Focus();
                    }
                }
                catch
                {
                    lbError.Text = await GetPubMessageVN("00042");
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = await GetPubMessage("00042");
                    _er.MessageVietNam = await GetPubMessageVN("00042");
                    _er.ShowDialog();
                    InputData.Focus();
                    return;
                }
            }
            
        }
        private async Task<string> CheckSN(string SN, bool _CHECKSN_OK)
        {
            int i, macfound;
            string sMAC1, cVersion, tmp_str, KPNO;
            string sTime, sTime_Now, sModel_Name, sBomNo;
            sOTHER_USE_ASSN[1] = new OTHER_USE_ASSN();
            sOTHER_USE_ASSN[1].USED = false;
            sOTHER_USE_ASSN[1].checked1 = false;
            sOTHER_USE_ASSN[1].USEKEY = "";
            sOTHER_USE_ASSN[1].indexid = 0;
            cVersion = "";

            if (!string.IsNullOrEmpty(SN))
            {
                string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
                var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strChkROKU,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ChkROKU.Data.Count() == 0)
                {
                    string strGetSN = $"select * from sfism4.r_custsn_t where serial_number = '{SN}'";
                    var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_SN.Data != null)
                    {
                        if (qry_SN.Data["mac1"] is null) sMAC1 = "";
                        else
                        sMAC1 = qry_SN.Data["mac1"].ToString();
                        if ((!string.IsNullOrEmpty(sMAC1)) && (sMAC1 != "N/A"))
                        {
                            string strGetData = "SELECT sum(a)"
                               + $" AS C_COUNT FROM (select count(*) a from  SFISM4.R_CUSTSN_T WHERE MAC1 = '{sMAC1}'"
                               + $" AND SERIAL_NUMBER <> '{SN}'"
                               + " union"
                               + " select count(*) a from SFISM4.R_CUSTSN_T_BAK "
                               + $" WHERE MAC1 = '{sMAC1}' AND SERIAL_NUMBER <> '{SN}')";
                            var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetData,
                                SfcCommandType = SfcCommandType.Text
                            });
                            //if (Int32.Parse(qry_Data.Data["c_count"].ToString()) > 0)
                            //{
                            //    sMAC1 = sMAC1 + " MACID IS DUPLICATE IN CUSTSN. CALL TE-IT CHECK!";
                            //    try
                            //    {
                            //        string strInsert = "Insert into SFISM4.R_SYSTEM_LOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC, TIME)"
                            //         + " Values"
                            //         + $" ('{empNo}', 'PACK_BOX', 'DUP_DATA','{sMAC1}',sysdate)";
                            //        var insert_system_log = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            //        {
                            //            CommandText = strInsert,
                            //            SfcCommandType = SfcCommandType.Text
                            //        });
                            //    }
                            //    catch (Exception)
                            //    {
                            //        MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            //    }
                            //    return sMAC1;
                            //}
                        }
                    }
                    //else
                    //{
                    //    CHECKSN_OK = false;
                    //    return "00289 - " + await GetPubMessage("00289");
                    //}
                }
                else
                {
                    string strGetSerialNumber = $"select * from sfism4.r_wip_tracking_t where serial_number = '{SN}'";
                    var qry_SerialNumber = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetSerialNumber,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_SerialNumber.Data == null)
                    {
                        CHECKSN_OK = false;
                        return "00414 - " + await GetPubMessage("00414");
                    }
                }
            }

            if (!string.IsNullOrEmpty(SN))
            {
                string strGetDataSN = $"select * from sfism4.r_wip_tracking_t where serial_number = '{SN}' or shipping_sn='{SN}'";
                var qry_DataSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetDataSN,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                            new SfcParameter{Name="sSerN",Value=SN}
                    }
                });
                if (qry_DataSN.Data == null)
                {
                    CHECKSN_OK = false;
                    return "00414 - " + await GetPubMessage("00414");
                }

                Edt_modelName.Text = qry_DataSN.Data["model_name"].ToString();
                if ((qry_DataSN.Data["scrap_flag"].ToString()) != "0")
                {
                    CHECKSN_OK = false;
                    return "00401 - " + await GetPubMessage("00401");
                }

                if (await CheckRoute() == false)
                {
                    CHECKSN_OK = false;
                    return lbError.Text.ToString();
                }

                FindFlag();

                if (await CheckMO(MO_Status) == false)
                {
                    if (MO_Status == "1")
                    {
                        CHECKSN_OK = false;
                        return await GetPubMessage("00012");
                    }
                    else if (MO_Status == "3")
                    {
                        CHECKSN_OK = false;
                        return await GetPubMessage("00013");
                    }
                    else if (MO_Status == "5")
                    {
                        CHECKSN_OK = false;
                        return await GetPubMessage("00014");
                    }
                    else if (MO_Status == "6")
                    {
                        CHECKSN_OK = false;
                        return await GetPubMessage("00015");
                    }
                    else if (MO_Status == "")
                    {
                        CHECKSN_OK = false;
                        return await GetPubMessage("00632");
                    }
                }

                if ((Edt_moNumber.Text != "") && (Edt_moNumber.Text != qry_DataSN.Data["mo_number"].ToString()))
                {
                    if (CHECK_MO_FLAG == true)
                    {
                        if (M_modeltype.IndexOf("A") == -1)
                        {
                            return "00402 - " + await GetPubMessage("00402");
                        }
                    }
                }

                //Show data in display
                Edt_moNumber.Text = qry_DataSN.Data["mo_number"]?.ToString() ?? "";
                Edt_modelName.Text = qry_DataSN.Data["model_name"]?.ToString() ?? "";
                Edt_lineName.Text = M_sThisLine;
                Edt_versionCode.Text = qry_DataSN.Data["version_code"]?.ToString() ?? "";
                Edt_lastSection.Text = qry_DataSN.Data["section_name"]?.ToString() ?? "";
                Edt_lastGroup.Text = qry_DataSN.Data["group_name"]?.ToString() ?? "";
                Edt_lastStation.Text = qry_DataSN.Data["station_name"]?.ToString() ?? "";
                M_sSSN1 = qry_DataSN.Data["shipping_sn"]?.ToString() ?? "";
                sBomNo = qry_DataSN.Data["bom_no"]?.ToString() ?? "";

                string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
                var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strChkROKU,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ChkROKU.Data.Count() > 0)
                {
                    string strGetEtEConfig = $"select * from SFIS1.C_ETE_CONFIG_T where TYPE='LOCK' and GROUP_NAME ='PACK_BOX' and MODEL_NAME ='{Edt_modelName.Text}'";
                    var qry_EtEConfig = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetEtEConfig,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_EtEConfig.Data.Count() > 0)
                    {
                        //Check ma lieu 2022/01/06 start
                        try
                        {
                            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = "SFIS1.MATERIAL_CACULATE",
                                SfcCommandType = SfcCommandType.StoredProcedure,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="DATA", Value=Edt_modelName.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="LINE",Value=Edt_lineName.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                }
                            });

                            dynamic ads = result.Data;
                            res = ads[0]["res"];
                            if (res != "OK")
                            {
                                CHECKSN_OK = false;
                                return res;
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + ex.Message;
                            _er.MessageEnglish = "Excute procedure have error:" + ex.Message;
                            _er.ShowDialog();
                        }
                        //Check ma lieu 2022/01/06 end;

                        //2022/01/20 Check thiet lap ma lieu trong ngay - start
                        string strGetKeyPart = $"SELECT * FROM SFISM4.R_KEYPARTS_SKIP_T where BOM_NO ='{Edt_modelName.Text}' and DATA2 ='{Edt_lineName.Text}' order by IN_STATION_TIME DESC";
                        var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKeyPart,
                            SfcCommandType = SfcCommandType.Text
                        });
                        List<Material> rs = qry_KeyPart.Data.ToListObject<Material>().ToList();
                        if (qry_KeyPart.Data != null)
                        {
                            sTime = rs[0].INSTATION_TIME.ToString("yyyyMMdd");
                            sTime_Now = DateTime.Now.ToString("yyyyMMdd");
                            if (sTime != sTime_Now)
                            {
                                CHECKSN_OK = false;
                                return "Have not data material for today(Chua xao lieu ngay hom nay cho chuyen nay)" + Edt_lineName.Text;
                            }
                        }
                        //2022/01/20 Check thiet lap ma lieu trong ngay - end

                        //2022/01/25 CHECK ten hang dang xao va ten hang dang thiet lap - start
                        string strGetKeyPartSkip = $"SELECT * FROM SFISM4.R_KEYPARTS_SKIP_T where DATA2 ='{Edt_lineName.Text}' order by IN_STATION_TIME DESC";
                        var qry_KeyPartSkip = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKeyPartSkip,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_KeyPartSkip.Data != null)
                        {
                            sModel_Name = qry_KeyPartSkip.Data["bom_no"]?.ToString() ?? "";
                            if (sModel_Name != Edt_modelName.Text)
                            {
                                CHECKSN_OK = false;
                                return "Model name not same model_name in station USB(Ten hang ('" + Edt_lineName.Text + "')khac voi ten hang tai tram xao lieu -  '" + sModel_Name + "'";
                            }
                        }
                        //2022/01/25 CHECK ten hang dang xao va ten hang dang thiet lap - end
                    }
                }
                if (qry_DataSN.Data["finish_flag"].ToString() == "1")
                {
                    Keypart_OK = true;
                }
                else
                {
                    string strGetBOM = $"SELECT * FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO ='{sBomNo}' AND GROUP_NAME ='KEYPART'";
                    var qry_BOM = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetBOM,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_BOM.Data.Count() > 0)
                    {
                        CHECKSN_OK = false;
                        return "This Serial Number not scan Keypart!!Please scan keypart again!!";
                    }
                    else
                    {
                        KPNO = await CheckKeyPart(SN, Keypart_OK);
                        if (Keypart_OK == false)
                        {
                            CHECKSN_OK = false;
                            return "Have No KeyParts: '" + KPNO + "' !!";
                        }
                    }
                }

                KPNO = await ChecklinkKeyPart(SN, Keypart_OK);
                if (Keypart_OK == false)
                {
                    CHECKSN_OK = false;
                    return "Have No KeyParts: '" + KPNO + "' !!";
                }

                //Check Config44 items IE
                if (cVersion.Trim() == "")
                {
                    cVersion = qry_DataSN.Data["version_code"]?.ToString() ?? "";
                    string strGetConfig44 = "SELECT * FROM SFIS1.C_CUSTSN_RULE_T"
                        + $" WHERE  MODEL_NAME = '{Edt_modelName.Text}'"
                        + $" AND VERSION_CODE = '{cVersion}' AND MO_TYPE='{sMO_TYPE}'";
                    var qry_Config44 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetConfig44,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Config44.Data.Count() == 0)
                    {
                        CHECKSN_OK = false;
                        return "20102 - " + await GetPubMessage("20102");
                    }
                    else
                    {
                        foreach (var item in qry_Config44.Data)
                        {
                            sCUSTSN_CODE = item["custsn_code"].ToString();

                            if (qry_Config44.Data != null)
                            {
                                if (sCUSTSN_CODE.IndexOf("SSN") != -1)
                                {
                                    for (i = 30; i > 1; i--)
                                    {
                                        if (sCUSTSN_CODE.IndexOf("SSN" + i.ToString()) != -1)
                                        {
                                            break;
                                        }
                                    }
                                    sLOAD_SSN[i].SSN = "";
                                    sLOAD_SSN[i].SSN_OK = false;
                                    sLOAD_SSN[i].cSSN_PREFIX = item["custsn_prefix"]?.ToString() ?? "";
                                    sLOAD_SSN[i].cSSN_POSTFIX = item["custsn_postfix"]?.ToString() ?? "";
                                    if (qry_ChkROKU.Data.Count() == 0)
                                    {
                                        sLOAD_SSN[i].cSSN_STR = expandSpecialStr(item["custsn_str"]?.ToString()) ?? "";
                                    }
                                    else
                                    {
                                        sLOAD_SSN[i].cSSN_STR = item["custsn_str"]?.ToString() ?? "";
                                    }
                                    sLOAD_SSN[i].sSHIPPINGSN_CODE = item["shippingsn_code"]?.ToString().Trim() ?? "";
                                    if (sLOAD_SSN[i].sSHIPPINGSN_CODE != "")
                                    {
                                        if ((item["shippingsn_code"].ToString().Substring(0, 4) == "COMSN"))
                                        {
                                            sLOAD_SSN[i].sSHIPPINGSN_CODE2 = item["shippingsn_code"].ToString().Trim();
                                        }
                                    }
                                    sLOAD_SSN[i].sCHECK_SSN_RULE = item["check_rule_name"]?.ToString() ?? "";
                                    sLOAD_SSN[i].sCHECK_SSN_RANGE = item["check_range"]?.ToString() ?? "";
                                    sLOAD_SSN[i].sCHECK_SSN = item["check_ssn"].ToString();
                                    sLOAD_SSN[i].nSSN_LENGTH = Int32.Parse(item["custsn_leng"].ToString());
                                    if (sLOAD_SSN[i].cSSN_PREFIX.IndexOf(",") > -1)
                                    {
                                        sLOAD_SSN[i].nSSN_PREFIX_LEN = sLOAD_SSN[i].cSSN_PREFIX.IndexOf(",") - 1;
                                    }
                                    else
                                    {
                                        sLOAD_SSN[i].nSSN_PREFIX_LEN = sLOAD_SSN[i].cSSN_PREFIX.Length;
                                    }

                                    sLOAD_SSN[i].nSSN_POSTFIX_LEN = sLOAD_SSN[i].cSSN_POSTFIX.Length;
                                    sLOAD_SSN[i].sCOMPARE_SSN = item["compare_sn"]?.ToString() ?? "";
                                    sLOAD_SSN[i].nSSN_Self_StartDigit = Int32.Parse(item["custsn_start"].ToString());
                                    sLOAD_SSN[i].nSSN_Self_FlowNO = Int32.Parse(item["custsn_end"].ToString());
                                    sLOAD_SSN[i].nSSN_Compare_StartDigit = Int32.Parse(item["compare_sn_start"].ToString());
                                    sLOAD_SSN[i].nSSN_Compare_FlowNO = Int32.Parse(item["compare_sn_end"].ToString());

                                    if (sLOAD_SSN[i].sCOMPARE_SSN == "SN")
                                    {
                                        if ((sLOAD_SSN[i].nSSN_Self_StartDigit == sLOAD_SSN[i].nSSN_Compare_StartDigit) && (sLOAD_SSN[i].nSSN_Self_FlowNO == sLOAD_SSN[i].nSSN_Compare_FlowNO))
                                        {
                                            sOTHER_USE_ASSN[1].USED = true;
                                            sOTHER_USE_ASSN[1].USEKEY = "SSN";
                                            sOTHER_USE_ASSN[1].indexid = i;
                                        }
                                    }
                                }

                                if (sCUSTSN_CODE.IndexOf("MAC") != -1)
                                {
                                    for (i = 1; 1 <= 5; i++)
                                    {
                                        if (sCUSTSN_CODE.IndexOf("MAC" + i.ToString().Trim()) != -1)
                                        {
                                            break;
                                        }
                                    }
                                    sLOAD_MAC[i].MAC = "";
                                    sLOAD_MAC[i].MAC_OK = false;
                                    sLOAD_MAC[i].cMAC_PREFIX = item["custsn_prefix"]?.ToString() ?? "";
                                    sLOAD_MAC[i].cMAC_POSTFIX = item["custsn_postfix"]?.ToString() ?? "";
                                    if (qry_ChkROKU.Data.Count() == 0)
                                    {
                                        sLOAD_MAC[i].cMAC_STR = expandSpecialStr(item["custsn_str"].ToString());
                                    }
                                    else
                                    {
                                        sLOAD_MAC[i].cMAC_STR = item["custsn_str"]?.ToString() ?? "";
                                    }
                                    sLOAD_MAC[i].sMACID_CODE = item["shippingsn_code"]?.ToString() ?? "";
                                    sLOAD_MAC[i].sCHECK_MAC_RULE = item["check_rule_name"]?.ToString() ?? "";
                                    sLOAD_MAC[i].sCHECK_MAC = item["check_ssn"]?.ToString() ?? "";
                                    sLOAD_MAC[i].sCHECK_MAC_RANGE = item["check_range"]?.ToString() ?? "";
                                    sLOAD_MAC[i].nMAC_LENGTH = Int32.Parse(item["custsn_leng"].ToString());
                                    if (sLOAD_MAC[i].cMAC_PREFIX.IndexOf(",") > 0)
                                    {
                                        sLOAD_MAC[i].nMAC_PREFIX_LEN = sLOAD_MAC[i].cMAC_PREFIX.IndexOf(",") - 1;
                                    }
                                    else
                                    {
                                        sLOAD_MAC[i].nMAC_PREFIX_LEN = (sLOAD_MAC[i].cMAC_PREFIX).Length;
                                    }
                                    sLOAD_MAC[i].nMAC_POSTFIX_LEN = (sLOAD_MAC[i].cMAC_POSTFIX).Length;
                                    sLOAD_MAC[i].sCOMPARE_MAC = item["compare_sn"]?.ToString() ?? "";
                                    sLOAD_MAC[i].nMAC_Self_StartDigit = Int32.Parse(item["custsn_start"].ToString());
                                    sLOAD_MAC[i].nMAC_Self_FlowNO = Int32.Parse(item["custsn_end"].ToString());
                                    sLOAD_MAC[i].nMAC_Compare_StartDigit = Int32.Parse(item["compare_sn_start"].ToString());
                                    sLOAD_MAC[i].nMAC_Compare_FlowNO = Int32.Parse(item["compare_sn_end"].ToString());

                                    if (sLOAD_MAC[i].sCOMPARE_MAC == "SN")
                                    {
                                        if ((sLOAD_MAC[i].nMAC_Self_StartDigit == sLOAD_MAC[i].nMAC_Compare_StartDigit) && (sLOAD_MAC[i].nMAC_Self_FlowNO == sLOAD_MAC[i].nMAC_Compare_FlowNO))
                                        {
                                            sOTHER_USE_ASSN[1].USED = true;
                                            sOTHER_USE_ASSN[1].USEKEY = "MAC";
                                            sOTHER_USE_ASSN[1].indexid = i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //Check setup Config43 IE
                string strGetParameter = "SELECT count(*) FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME='BOXII' AND VR_VALUE='BOXII'";
                var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetParameter,
                    SfcCommandType = SfcCommandType.Text
                });
                if (Int32.Parse(qry_Parameter.Data["count(*)"].ToString()) > 0)
                {
                    if (qry_ChkROKU.Data.Count() == 0)
                    {
                        //Config 43
                        if (M_sThisGroup == "PACK_BOX")
                        {
                            string strGetConfig43 = $"SELECT * FROM SFIS1.C_PACK_SEQUENCE_T WHERE  MODEL_NAME = '{ Edt_modelName.Text.Trim()}'"
                                + $" AND VERSION_CODE = '{cVersion}' AND MO_TYPE='{sMO_TYPE}'"
                                + $" AND SUBSTR(CUSTSN_NAME, 1, 3) <> 'II_' order by SCAN_SEQ";
                            var qry_Config43 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetConfig43,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_Config43.Data.Count() == 0)
                            {
                                CHECKSN_OK = false;
                                return "20106 - " + await GetPubMessage("20106");
                            }
                            List<PACK_SEQUENCE> result = new List<PACK_SEQUENCE>();
                            result = qry_Config43.Data.ToListObject<PACK_SEQUENCE>().ToList();
                            SCAN_POS = 0;
                            if (result[0].CUSTSN_NAME != "HOUSING_SN")
                            {
                                sequenlist.Add(new SOURCE() { STEP = "HOUSING_SN" });
                            }
                            for (int j = 0; j < result.Count; j++)
                            {
                                sequenlist.Add(new SOURCE() { STEP = result[j].CUSTSN_NAME });
                                getSSNMAC_rule(j);
                                if (sOTHER_USE_ASSN[1].USED == true)
                                {
                                    if (sequenlist[j].STEP.IndexOf(sOTHER_USE_ASSN[1].USEKEY + (sOTHER_USE_ASSN[1].indexid).ToString()) > 0)
                                    {
                                        sOTHER_USE_ASSN[1].checked1 = true;
                                    }
                                }
                                if (sequenlist[j].STEP.IndexOf("MAC") > 0)
                                {
                                    CheckMacID1.IsChecked = true;
                                }
                                if (sequenlist[j].STEP.IndexOf("BOX_SSN") != -1)
                                {
                                    CTNSCAN_SHIPPINGSN = sequenlist[j].STEP.Substring(sequenlist[j].STEP.IndexOf("SSN"), 4);
                                }
                            }
                            Data_gridView.ItemsSource = sequenlist;
                        }
                    }
                    else
                    {
                        if (M_sThisGroup == "PACK_BOX")
                        {
                            string strGetConfig43 = $"SELECT * FROM SFIS1.C_PACK_SEQUENCE_T WHERE  MODEL_NAME = '{Edt_modelName.Text.Trim()}'"
                                + $" AND VERSION_CODE = '{cVersion}' AND MO_TYPE='{sMO_TYPE}'"
                                + " AND SUBSTR(CUSTSN_NAME, 1, 3) <> 'II_' AND instr(CUSTSN_NAME,':')=0 order by SCAN_SEQ";
                            var qry_Config43 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetConfig43,
                                SfcCommandType = SfcCommandType.Text
                            });
                            //Config 43
                            if (qry_Config43.Data.Count() == 0)
                            {
                                CHECKSN_OK = false;
                                return "20106 - " + await GetPubMessage("20106");
                            }
                            List<PACK_SEQUENCE> result = new List<PACK_SEQUENCE>();
                            result = qry_Config43.Data.ToListObject<PACK_SEQUENCE>().ToList();
                            SCAN_POS = 0;
                            if (result[0].CUSTSN_NAME != "HOUSING_SN")
                            {
                                sequenlist.Add(new SOURCE() { STEP = "HOUSING_SN" });
                                //j++;
                            }
                            for (int j = 0; j < result.Count; j++)
                            {
                                sequenlist.Add(new SOURCE() { STEP = result[j].CUSTSN_NAME });
                                getSSNMAC_rule(j);
                                if (sOTHER_USE_ASSN[1].USED == true)
                                {
                                    if (sequenlist[j].STEP.IndexOf(sOTHER_USE_ASSN[1].USEKEY + (sOTHER_USE_ASSN[1].indexid).ToString()) > 0)
                                    {
                                        sOTHER_USE_ASSN[1].checked1 = true;
                                    }
                                }
                                if (sequenlist[j].STEP.IndexOf("MAC") > 0)
                                {
                                    CheckMacID1.IsChecked = true;
                                }
                                if (sequenlist[j].STEP.IndexOf("BOX_SSN") != -1)
                                {
                                    CTNSCAN_SHIPPINGSN = sequenlist[j].STEP.Substring(sequenlist[j].STEP.IndexOf("SSN"), 4);
                                }
                            }
                            Data_gridView.ItemsSource = sequenlist;
                        }
                    }

                    if (M_sThisGroup != "PACK_BOX")
                    {
                        if (M_sThisGroup == "PACK_BOXII")
                        {
                            string strGetConfig43 = "SELECT * FROM SFIS1.C_PACK_SEQUENCE_T"
                                + $" WHERE  MODEL_NAME = '{Edt_modelName.Text}'"
                                + $" AND VERSION_CODE = '{cVersion}'"
                                + $" AND MO_TYPE='{sMO_TYPE}'"
                                + " AND SUBSTR(CUSTSN_NAME, 1, 3) = 'II_'  order by SCAN_SEQ";
                            var qry_Config43 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetConfig43,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_Config43.Data.Count() == 0)
                            {
                                CHECKSN_OK = false;
                                return "20106 - " + await GetPubMessage("20106");
                            }

                            List<PACK_SEQUENCE> result = new List<PACK_SEQUENCE>();
                            result = qry_Config43.Data.ToListObject<PACK_SEQUENCE>().ToList();
                            tmp_CUSTSN_NAME = result[0].CUSTSN_NAME.Substring(3, (result[0].CUSTSN_NAME.ToString().Length) - 3);
                            SCAN_POS = 0;
                            if (tmp_CUSTSN_NAME != "HOUSING_SN")
                            {
                                sequenlist.Add(new SOURCE() { STEP = "HOUSING_SN" });
                            }
                            for (int j = 0; j < result.Count; j++)
                            {
                                tmp_CUSTSN_NAME = result[j].CUSTSN_NAME.Substring(3, (result[j].CUSTSN_NAME.ToString().Length) - 3);
                                sequenlist.Add(new SOURCE() { STEP = tmp_CUSTSN_NAME });
                                getSSNMAC_rule(j + 1);
                                if (sOTHER_USE_ASSN[1].USED == true)
                                {
                                    if (tmp_CUSTSN_NAME.IndexOf(sOTHER_USE_ASSN[1].USEKEY + (sOTHER_USE_ASSN[1].indexid).ToString()) > 0)
                                    {
                                        sOTHER_USE_ASSN[1].checked1 = true;
                                    }
                                }
                                if (tmp_CUSTSN_NAME.IndexOf("MAC") > 0)
                                {
                                    CheckMacID1.IsChecked = true;
                                }
                                if (tmp_CUSTSN_NAME.IndexOf("BOX_SSN") != -1)
                                {
                                    CTNSCAN_SHIPPINGSN = sequenlist[j + 1].STEP.Substring(sequenlist[j + 1].STEP.IndexOf("SSN"), 4); 
                                }
                            }
                            Data_gridView.ItemsSource = sequenlist;
                        }
                        else
                        {
                            string strGetConfig43 = "SELECT * FROM SFIS1.C_PACK_SEQUENCE_T"
                             + $" WHERE  MODEL_NAME = '{Edt_modelName.Text}'"
                             + $" AND VERSION_CODE = '{cVersion}'"
                             + $" AND MO_TYPE='{sMO_TYPE}'"
                             + " AND  SUBSTR(CUSTSN_NAME,1,:L1)=:ST1  order by SCAN_SEQ";
                            var qry_Config43 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetConfig43,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="L1",Value=M_sThisGroup.Length + 1},
                                    new SfcParameter{Name="ST1",Value= ":"+M_sThisGroup}
                                }
                            });
                            if (qry_Config43.Data.Count() == 0)
                            {
                                CHECKSN_OK = false;
                                return "20106 - " + await GetPubMessage("20106");
                            }

                            List<PACK_SEQUENCE> result = new List<PACK_SEQUENCE>();
                            result = qry_Config43.Data.ToListObject<PACK_SEQUENCE>().ToList();
                            tmp_CUSTSN_NAME = result[0].CUSTSN_NAME.Substring(3, (result[0].CUSTSN_NAME.ToString().Length) - 3);
                            SCAN_POS = 0;
                            if (tmp_CUSTSN_NAME != "HOUSING_SN")
                            {
                                sequenlist.Add(new SOURCE() { STEP = "HOUSING_SN" });
                            }
                            for (int j = 0; j < result.Count; j++)
                            {
                                tmp_CUSTSN_NAME = result[j].CUSTSN_NAME.Substring(3, (result[j].CUSTSN_NAME.ToString().Length) - 3);
                                sequenlist.Add(new SOURCE() { STEP = tmp_CUSTSN_NAME });
                                getSSNMAC_rule(j + 1);
                                if (sOTHER_USE_ASSN[1].USED == true)
                                {
                                    if (tmp_CUSTSN_NAME.IndexOf(sOTHER_USE_ASSN[1].USEKEY + (sOTHER_USE_ASSN[1].indexid).ToString()) > 0)
                                    {
                                        sOTHER_USE_ASSN[1].checked1 = true;
                                    }
                                }
                                if (tmp_CUSTSN_NAME.IndexOf("MAC") > 0)
                                {
                                    CheckMacID1.IsChecked = true;
                                }
                                if (tmp_CUSTSN_NAME.IndexOf("BOX_SSN") != -1)
                                {
                                    CTNSCAN_SHIPPINGSN = sequenlist[j + 1].STEP.Substring(sequenlist[j + 1].STEP.IndexOf("SSN"), 4);
                                }
                            }
                            Data_gridView.ItemsSource = sequenlist;
                        }
                    }
                }
                else
                {
                    string strGetConfig43 = "SELECT * FROM SFIS1.C_PACK_SEQUENCE_T "
                          + $" WHERE  MODEL_NAME = '{Edt_modelName.Text}'"
                          + $" AND VERSION_CODE = '{cVersion}'"
                          + $" AND MO_TYPE='{sMO_TYPE}' order by SCAN_SEQ";
                    var qry_Config43 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetConfig43,
                        SfcCommandType = SfcCommandType.Text
                    });

                    //Config 43
                    if (qry_Config43.Data != null)
                    {
                        CHECKSN_OK = false;
                        return await GetPubMessage("20106");
                    }
                    i = 1;

                    List<PACK_SEQUENCE> result = new List<PACK_SEQUENCE>();
                    result = qry_Config43.Data.ToListObject<PACK_SEQUENCE>().ToList();
                    SCAN_POS = 0;
                    for (int j = 0; j < result.Count; j++)
                    {
                        if (result[0].CUSTSN_NAME != "HOUSING_SN")
                        {
                            sequenlist.Add(new SOURCE() { STEP = "HOUSING_SN" });
                            j++;
                        }
                        sequenlist.Add(new SOURCE() { STEP = result[j].CUSTSN_NAME });
                        getSSNMAC_rule(j);
                        if (sOTHER_USE_ASSN[1].USED == true)
                        {
                            if (sequenlist[j].STEP.IndexOf(sOTHER_USE_ASSN[1].USEKEY + (sOTHER_USE_ASSN[1].indexid).ToString()) > 0)
                            {
                                sOTHER_USE_ASSN[1].checked1 = true;
                            }
                        }
                        if (sequenlist[j].STEP.IndexOf("MAC") > 0)
                        {
                            CheckMacID1.IsChecked = true;
                        }
                        if (sequenlist[j].STEP.IndexOf("BOX_SSN") > 0)
                        {
                            CTNSCAN_SHIPPINGSN = sequenlist[j].STEP.Substring(sequenlist[j].STEP.IndexOf("SSN"), 4);
                        }
                    }
                    Data_gridView.ItemsSource = sequenlist;
                }

                if (qry_ChkROKU.Data.Count() > 0)
                {
                    string strGetSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{SN}' AND ROWNUM=1";
                    var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_SN.Data != null)
                    {
                        SMAC[1].CUSTSN_MAC = qry_SN.Data["mac1"]?.ToString() ?? "";
                        SMAC[2].CUSTSN_MAC = qry_SN.Data["mac2"]?.ToString() ?? "";
                        SMAC[3].CUSTSN_MAC = qry_SN.Data["mac3"]?.ToString() ?? "";
                        SMAC[4].CUSTSN_MAC = qry_SN.Data["mac4"]?.ToString() ?? "";
                        SMAC[5].CUSTSN_MAC = qry_SN.Data["mac5"]?.ToString() ?? "";

                        macfound = 0;
                        if (await FindMAC(SN, macfound) == 0)
                        {
                            if (await FindHMAC(SN, macfound) == 0)
                            {
                                CHECKSN_OK = false;
                                if (sLOAD_MAC[1].sCHECK_MAC != "")
                                {
                                    return SN + " " + await GetPubMessage("00403");
                                }
                            }
                        }
                        MACID = SMAC[1].MAC;
                    }
                }
                else
                {
                    if (sLOAD_MAC[1].sCHECK_MAC == "Y")
                    {
                        string strGetSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '{SN}' AND ROWNUM = 1";
                        var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetSN,
                            SfcCommandType = SfcCommandType.Text
                        });

                        SMAC[1].CUSTSN_MAC = qry_SN.Data["mac1"]?.ToString() ?? "";
                        SMAC[2].CUSTSN_MAC = qry_SN.Data["mac2"]?.ToString() ?? "";
                        SMAC[3].CUSTSN_MAC = qry_SN.Data["mac3"]?.ToString() ?? "";
                        SMAC[4].CUSTSN_MAC = qry_SN.Data["mac4"]?.ToString() ?? "";
                        SMAC[5].CUSTSN_MAC = qry_SN.Data["mac5"]?.ToString() ?? "";
                        macfound = 0;
                        if (await FindMAC(SN, macfound) == 0)
                        {
                            if (await FindHMAC(SN, macfound) == 0)
                            {
                                CHECKSN_OK = false;

                                if (sLOAD_MAC[1].sCHECK_MAC != "")
                                {
                                    return SN + " " + await GetPubMessage("00403");
                                }
                            }
                        }
                        MACID = SMAC[1].MAC;
                    }
                }

                if (item_Nocheck.IsChecked == false)
                {
                    if (await Checkcustsndata(SN) == false)
                    {
                        CHECKSN_OK = false;
                        return Err_Msg;
                    }
                }

                if (await Chk107flag(CHECKFLAG, SN) == false)
                {
                    CHECKSN_OK = false;
                    return "00633 - " + await GetPubMessage("00633");
                }

                if (item_Nocheck.IsChecked == false)
                {
                    string strGetKeyPart = $"select * from sfism4.R108 where serial_number = '{SN}' and KEY_PART_NO='MACID'";
                    var qry_KeyPartt = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPart,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_KeyPartt.Data != null)
                    {
                        tmp_str = qry_KeyPartt.Data["key_part_sn"]?.ToString() ?? "";
                        if (await Netgear_MoreMac_NoCheck_mac(SN) == false)
                        {
                            Mes_err = await CheckMAC_IDTYPE(tmp_str, Edt_modelName.Text);
                            if (Mes_err != "")
                            {
                                CHECKSN_OK = false;
                                return Mes_err;
                            }
                        }
                    }
                }
                CHECKSN_OK = true;
                sequenlist[SCAN_POS].SN = _Inputdata;
                SCAN_POS = SCAN_POS + 1;
                Data_gridView.Items.Refresh();
                editSerialNumber.Text = _Inputdata;
                return "";
            }
            return "";
        }

        public void getSSNMAC_rule(int mcolumn)
        {
            int j;
            string mStepdata = sequenlist[mcolumn].STEP;
            if (mStepdata.IndexOf("SSN") > -1)
            {
                //SSN10-SSN12
                if ((mStepdata.Substring(mStepdata.Length - 2, 2) == "10") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "11") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "12")
                    || (mStepdata.Substring(mStepdata.Length - 2, 2) == "13") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "14") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "15")
                    || (mStepdata.Substring(mStepdata.Length - 2, 2) == "16") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "17") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "18")
                    || (mStepdata.Substring(mStepdata.Length - 2, 2) == "19") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "20") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "21")
                    || (mStepdata.Substring(mStepdata.Length - 2, 2) == "22") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "23") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "24")
                    || (mStepdata.Substring(mStepdata.Length - 2, 2) == "25") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "26") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "27")
                    || (mStepdata.Substring(mStepdata.Length - 2, 2) == "28") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "29") || (mStepdata.Substring(mStepdata.Length - 2, 2) == "30")
                    )
                {
                    for (j = 10; j <= 30; j++)
                    {
                        if (mStepdata.IndexOf("SSN" + j.ToString().Trim()) > 0)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //SSN1-SSN9
                    for (j = 1; j <= 9; j++)
                    {
                        if (mStepdata.IndexOf("SSN" + j.ToString().Trim()) > 0)
                        {
                            break;
                        }
                    }
                }

                //if (item_Setsn.IsChecked == true)
                //{
                //    InputData.Text = getInputdataWithINI(InputData.Text, "SSN" + j.ToString().Trim());
                //}

                sequenlist[mcolumn].PREFIX = sLOAD_SSN[j].cSSN_PREFIX.ToString();
                sequenlist[mcolumn].LENGTH = sLOAD_SSN[j].nSSN_LENGTH.ToString();
            }

            if (mStepdata.IndexOf("MAC") > -1)
            {
                for (j = 1; j <= 5; j++)
                {
                    if (mStepdata.IndexOf("MAC" + j.ToString().Trim()) > 0)
                    {
                        break;
                    }
                }
                sequenlist[mcolumn].PREFIX = sLOAD_MAC[j].cMAC_PREFIX.ToString();
                sequenlist[mcolumn].LENGTH = sLOAD_MAC[j].nMAC_LENGTH.ToString();
            }

        }
        public async Task<string> ChkSSN(string SSN, string SSN_CODE, bool _CHECKSSN_OK)
        {
            int i, SSN_IDX;
            string CUST_SN, sIgnoreStr, MODEL_SERIAL_U_Z, tmp_str1;

            if (sCAPS_LOCK == true)
            {
                CUST_SN = SSN.ToUpper();
                _SSN = CUST_SN;
            }
            else
            {
                CUST_SN = SSN;
                _SSN = CUST_SN;
            }
            SSN_IDX = Int32.Parse(SSN_CODE.Substring(3));
            sIgnoreStr = sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE == null ? "" : (sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE).Trim();
            if ((sIgnoreStr != "") && (CUST_SN.Substring(0, sIgnoreStr.Length) == sIgnoreStr))
            {
                CUST_SN = CUST_SN.Substring((sIgnoreStr.Length) , CUST_SN.Length);
            }
            if (!((M_sSSN1 == "N/A") || (M_sSSN1 == "")) && (SSN_CODE == "SSN1"))
            {
                if (CUST_SN != M_sSSN1)
                {
                    SSN_OK = false;
                    return editSerialNumber.Text + " " + await GetPubMessage("00386") + " " + M_sSSN1;
                }
            }
            if (SSN_IDX.ToString() == "1")
            {
                if (await ChkDupSSN(SSN) == "EROR")
                {
                    SSN_OK = false;
                    return editSerialNumber.Text + " " + await GetPubMessage("80144");
                }
            }
            if ((CUST_SN.Length != (sLOAD_SSN[SSN_IDX].nSSN_LENGTH)) && sLOAD_SSN[SSN_IDX].nSSN_LENGTH != 0)
            {
                if (sLOAD_SSN[SSN_IDX].nSSN_LENGTH != 99)
                {
                    SSN_OK = false;
                    return "00366 - " + await GetPubMessage("00366");
                }
            }
            if ((sLOAD_SSN[SSN_IDX].cSSN_PREFIX != "") && (sLOAD_SSN[SSN_IDX].cSSN_PREFIX.IndexOf(CUST_SN.Substring(0, sLOAD_SSN[SSN_IDX].nSSN_PREFIX_LEN))) == -1)
            {
                SSN_OK = false;
                return "00387 - " + await GetPubMessage("00387");
            }
            //if (((CUST_SN.Substring(((CUST_SN.Length - 3) - sLOAD_SSN[SSN_IDX].nSSN_POSTFIX_LEN) + 1, sLOAD_SSN[SSN_IDX].nSSN_POSTFIX_LEN)) != sLOAD_SSN[SSN_IDX].cSSN_POSTFIX) && (CUST_SN.Length == 18))
            //{
            //    SSN_OK = false;
            //    return "00388 - " +  await GetPubMessage("00388");
            //}
            zTemp1 = CUST_SN;
            zSubstring(ref zTemp1, CUST_SN.Length - sLOAD_SSN[SSN_IDX].nSSN_POSTFIX_LEN, sLOAD_SSN[SSN_IDX].nSSN_POSTFIX_LEN);
            if ((zTemp1 != sLOAD_SSN[SSN_IDX].cSSN_POSTFIX) && (CUST_SN.Length != 18))
            {
                SSN_OK = false;
                return "00388 - " + await GetPubMessage("00388");
            }
            if (sLOAD_SSN[SSN_IDX].cSSN_STR != "")
            {
                for (i = 0; i < CUST_SN.Length; i++)
                {
                    if (sLOAD_SSN[SSN_IDX].cSSN_STR.IndexOf(CUST_SN[i]) == -1)
                    {
                        SSN_OK = false;
                        return "00389 - " + await GetPubMessage("00389");
                    }
                }
            }
            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "CONTROLKEY")
            {
                if (ControlKey(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "This SSN Discrepancy ControlKey Rule,Please Confirm With The LABEL ROOM!";
                }
            }
            else
            {
                if (M_modeltype.IndexOf("O") > -1)
                {
                    if (await U54Rule(CUST_SN, "SSN") == true)
                    {
                        SSN_OK = false;
                        return "The SSN Discrepancy U54 SSN Rule,Please Confirm With The LABEL ROOM!";
                    }
                }
                else
                {
                    if (M_modeltype.IndexOf("U") > -1)
                    {
                        if ((sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE).ToUpper() == "CHECKMODELSSN")
                        {
                            if (NetgearRule(CUST_SN) == true)
                            {
                                SSN_OK = false;
                                return "This SSN Discrepancy Netgear Rule,Please Confirm With The LABEL ROOM!!";
                            }
                        }
                    }
                    else
                    {
                        if ((sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "NETGEARRULE") && (M_modeltype != "U"))
                        {
                            if (NetgearRule(CUST_SN) == true)
                            {
                                SSN_OK = false;
                                return "This SSN Discrepancy Netgear Rule,Please Confirm With The LABEL ROOM!!";
                            }
                        }
                        else
                        {
                            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "CHECKMODELSSN")
                            {
                                if (CHECKMODELSSN(sLOAD_SSN[SSN_IDX].cSSN_PREFIX, CUST_SN) == true)
                                {
                                    SSN_OK = false;
                                    return "This SSN Discrepancy CHECKMODELSSN Rule,Please Confirm With The LABEL ROOM!";
                                }
                            }
                            else
                            {
                                if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "IBMMODULO10")
                                {
                                    if (IBMModulo10(CUST_SN) == true)
                                    {
                                        SSN_OK = false;
                                        return "This SSN Discrepancy IBMModulo10 Rule,Please Confirm With The LABEL ROOM!";
                                    }
                                    else
                                    {
                                        if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "SONYRULE")
                                        {
                                            if (await SonyRule(CUST_SN) == true)
                                            {
                                                SSN_OK = false;
                                                return "This SSN Discrepancy SonyRule Rule,Please Confirm With The LABEL ROOM!";
                                            }
                                        }
                                        else
                                        {
                                            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "PINCODEDATARULE")
                                            {
                                            }
                                            else
                                            {
                                                if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE != "")
                                                {
                                                    SSN_OK = false;
                                                    return "Config Item 44 SSN Check Rule Undefined,Please Call IE Defined!!";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (SSN_IDX == 1)
            {
                string strGetShippingSN = $"select * from sfism4.r_wip_tracking_t where shipping_sn = '{CUST_SN}'";
                var qry_ShippingSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetShippingSN,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ShippingSN.Data != null)
                {
                    if (qry_ShippingSN.Data["serial_number"].ToString() != editSerialNumber.Text)
                    {
                        SSN_OK = false;
                        return "Shipping SN Duplicate,R107 Has exist '" + CUST_SN + "' !!";
                    }
                }
            }
            if (sMO_TYPE == "RMA")
            {

            }
            else
            {
                if (SSN_IDX == 1)
                {
                    string strGetSerialNumber = "select * from sfism4.r_wip_tracking_t"
                        + $" where serial_number= '{editSerialNumber.Text}' and po_no = '{CUST_SN}'";
                    var qry_SerialNumber = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetSerialNumber,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_SerialNumber.Data.Count() == 0)
                    {
                        string strGetMAC = "select * from sfism4.r_wip_tracking_t"
                             + $" where serial_number= '{editSerialNumber.Text}'";
                        var qry_MAC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetMAC,
                            SfcCommandType = SfcCommandType.Text
                        });
                        string PO_NO = qry_MAC.Data["po_no"]?.ToString() ?? "";
                        if ((PO_NO != "") && (PO_NO != "N/A"))
                        {
                            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN == "Y")
                            {
                                SSN_OK = false;
                                return "R107 In Shipping SN: '" + CUST_SN + "' and  PO_NO Not Match !!";
                            }
                        }
                        else
                        {
                            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN == "Y")
                            {
                                string strGetMONumber = "select * from sfism4.r_wip_tracking_t"
                                    + $" where MO_NUMBER= '{Edt_moNumber.Text}' and length(po_no) ='{CUST_SN.Length}' and rownum<10";
                                var qry_MONumber = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetMONumber,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_MONumber.Data.Count() > 0)
                                {
                                    SSN_OK = false;
                                    return "R107 In Shipping SN: '" + CUST_SN + "' and  PO_NO No Value !!";
                                }
                            }
                        }
                    }

                    string strGetCustSN = "select * from SFISM4.R_CUSTSN_T"
                            + $" where serial_number= '{editSerialNumber.Text}'"
                            + $" and SSN" + SSN_IDX.ToString() + " IS NOT NULL";
                    var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CustSN.Data.Count() > 0)
                    {
                        if (LB_2D.IsVisible == true)
                        {
                            string strGetCustSN2D = "select * from SFISM4.R_CUSTSN_T"
                                + " where serial_number= '" + editSerialNumber.Text + "'"
                                + " and  SSN" + SSN_IDX.ToString() + " in (" + GET2DSTRING() + ")";
                            var qry_CustSN2D =  await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetCustSN2D,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_CustSN2D.Data.Count() == 0)
                            {
                                SSN_OK = false;
                                return "This + " + CUST_SN + " + Shipping SN ANd R_CUSTSN_T Not Consist";
                            }
                            else
                            {
                                if (LB_2D.IsVisible == true)
                                {
                                   await Save2DInfoAsync(editSerialNumber.Text, CUST_SN);
                                }
                            }
                        }
                        else
                        {
                            string strGetCustSN2D = "select * from SFISM4.R_CUSTSN_T"
                                + " where serial_number= '" + editSerialNumber.Text + "'"
                                + " and  SSN"+ SSN_IDX.ToString() + " = '" + CUST_SN + "'";
                            var qry_CustSN2D = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetCustSN2D,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_CustSN2D.Data.Count() == 0)
                            {
                                SSN_OK = false;
                                return "This + " + CUST_SN + " + Shipping SN ANd R_CUSTSN_T Not Consist";
                            }
                            else
                            {
                                if (LB_2D.IsVisible == true)
                                {
                                    await Save2DInfoAsync(editSerialNumber.Text, CUST_SN);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string strGetCustSN = "select * from SFISM4.R_CUSTSN_T"
                        + " where serial_number= '" + editSerialNumber.Text + "'"
                        + " and SSN" + SSN_IDX.ToString() + " IS NOT NULL";
                    var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CustSN.Data.Count() == 0)
                    {
                        //if (sLOAD_SSN[SSN_IDX].sCHECK_SSN == "Y")
                        //{
                        //    string strGetCustSNPN = "select * from sfism4.r_wip_tracking_t"
                        //        + " where serial_number= '" + editSerialNumber.Text + "'"
                        //        + " and po_no = '" + CUST_SN + "'";
                        //    var qry_CustSNPN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        //    {
                        //        CommandText = strGetCustSNPN,
                        //        SfcCommandType = SfcCommandType.Text
                        //    });
                        //    if (qry_CustSNPN.Data.Count() == 0)
                        //    {
                        //        SSN_OK = false;
                        //        return "R107 In Shipping SN: '" + CUST_SN + "' And PO_NO Not Match!!";
                        //    }
                        //}
                        if (sLOAD_SSN[SSN_IDX].sCHECK_SSN == "Y")
                        {
                            string strGetCustSNPN = "select * from sfism4.R108"
                                + " where serial_number= '" + editSerialNumber.Text + "'"
                                + " and KEY_PART_SN = '" + CUST_SN + "'"
                                + " and KEY_PART_NO='SSN" + SSN_IDX.ToString() + "'";
                            var qry_CustSNPN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetCustSNPN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_CustSNPN.Data.Count() == 0)
                            {
                                SSN_OK = false;
                                return "R108 In Shipping SN: '" + CUST_SN + "' And KEY_PART_SN Not Match!!";
                            }
                        }
                    }
                    else
                    {
                        if (qry_CustSN.Data.Count() > 0)
                        {
                            if (LB_2D.IsVisible == true)
                            {
                                string strGetCustSN2D = "select * from SFISM4.R_CUSTSN_T"
                                    + " where serial_number= '" + editSerialNumber.Text + "'"
                                    + " and  SSN" + SSN_IDX.ToString() + " in (" + GET2DSTRING() + ")";
                                var qry_CustSN2D = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetCustSN,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_CustSN2D.Data.Count() == 0)
                                {
                                    SSN_OK = false;
                                    return "This + " + CUST_SN + " + Shipping SN ANd R_CUSTSN_T Not Consist";
                                }
                                else
                                {
                                    if (LB_2D.IsVisible == true)
                                    {
                                        await Save2DInfoAsync(editSerialNumber.Text, CUST_SN);
                                    }
                                }
                            }
                            else
                            {
                                string strGetCustSN2D = "select * from SFISM4.R_CUSTSN_T"
                                    + " where serial_number= '" + editSerialNumber.Text + "'"
                                    + " and  SSN" + SSN_IDX.ToString() + " = '" + CUST_SN + "'";
                                var qry_CustSN2D = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetCustSN2D,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_CustSN2D.Data.Count() == 0)
                                {
                                    SSN_OK = false;
                                    return "This + " + CUST_SN + " + Shipping SN ANd R_CUSTSN_T Not Consist";
                                }
                                else
                                {
                                    if (LB_2D.IsVisible == true)
                                    {
                                        await Save2DInfoAsync(editSerialNumber.Text, CUST_SN);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "BOXCHECK")
            {
                string strGetCustSN = $"select * from SFISM4.R_CUSTSN_T where SERIAL_NUMBER ='{editSerialNumber.Text}'";
                var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustSN,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_CustSN.Data == null)
                {
                    SSN_OK = false;
                    return "This '" + editSerialNumber.Text + "' R_CUSTSN_T No Data !!";
                }
                else
                {
                    if (CUST_SN != (qry_CustSN.Data["SSN"].ToString() + SSN_IDX.ToString()))
                    {
                        SSN_OK = false;
                        return "This Shipping SN: '" + CUST_SN + "' AND R_CUSTSN_T" + qry_CustSN.Data["SSN"].ToString() + SSN_IDX.ToString() + " Not Match!";
                    }
                }
            }
            else
            {
                if ((sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE != "CHECKMODELSSN") && (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE != "VALENTINECHECK")
                   && (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE != "MODELPONODUPCHECK") && (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE != "NOCHECKDUP")
                   && (SSN_IDX < 5))
                {
                    string strGetCustSN = "select * from SFISM4.R_CUSTSN_T"
                        + " where SSN" + SSN_IDX.ToString() + " = '" + CUST_SN + "'"
                        + " and  SERIAL_NUMBER <>'" + editSerialNumber.Text + "'";
                    var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CustSN.Data != null)
                    {
                        SSN_OK = false;
                        return "Shipping SN: '" + CUST_SN + "' duplicate,R_CUSTSN_T Has This SSN!!";
                    }

                    //string strGetHCustSN = "select * from SFISM4.R_CUSTSN_T_BAK"
                    //    + " where SSN" + SSN_IDX.ToString() + " = '" + CUST_SN + "'"
                    //    + " and  SERIAL_NUMBER <>'" + editSerialNumber.Text + "'";
                    //var qry_HCustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    //{
                    //    CommandText = strGetHCustSN,
                    //    SfcCommandType = SfcCommandType.Text
                    //});
                    //if (qry_HCustSN.Data != null)
                    //{
                    //    SSN_OK = false;
                    //    return "Shipping SN: '" + CUST_SN + "' duplicate,R_CUSTSN_T Has This SSN!!";
                    //}
                }
                else if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "MODELPONODUPCHECK")
                {
                    string strGetDataWIP = "select Serial_Number from SFISM4.R_WIP_Tracking_T Where Model_Name='" + Edt_modelName.Text + "'"
                        + " AND Serial_Number<> '" + editSerialNumber.Text + "' AND PO_NO='" + CUST_SN + "'";
                    var qry_DataWIP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetDataWIP,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_DataWIP.Data.Count() > 0)
                    {
                        SSN_OK = false;
                        return "Cust SN: '" + CUST_SN + "' Within the same product Duplicate,R107.PO_NO Has This SN!!";
                    }
                }
            }

            if (sLOAD_SSN[SSN_IDX].sCOMPARE_SSN == "SN")
            {
                if (sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 != "" && sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 != null)
                {
                    int ds = sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit;
                    int f1 = sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO;
                    string sdf = CUST_SN.Substring(ds, f1);

                    string sds = sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2;
                    int shi = 0;
                        int com = sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO;
                    string sdfs = editSerialNumber.Text.Substring(shi-1, com);
                    if (ChkCompareSN(CUST_SN.Substring(0, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO), sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2, editSerialNumber.Text.Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO)) == false)
                    {
                        SSN_OK = false;
                        return sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 + "Compare Error !!";
                    }
                }
                else
                {
                    if (sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO == 0 || sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit == 0)
                    {
                        CHECKSN_OK = false;
                        return sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO + "IE chua setup COMPARE_SN_START,COMPARE_SN_END config 44! ";
                    }
                    if ((CUST_SN.Substring(sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO)) != (editSerialNumber.Text.Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO)))
                    {
                        CHECKSN_OK = false;
                        return sequenlist[SCAN_POS].STEP + " <> SN!!";
                    }
                }
            }

            if (sLOAD_SSN[SSN_IDX].sCOMPARE_SSN == "SSN2")
            {
                if (sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit == 0)
                {
                    CHECKSN_OK = false;
                    return "SSN2 CHECK STARtDigit=0 !";
                }
                if (sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 != "" && sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 != null)
                {
                    if (ChkCompareSN(CUST_SN.Substring(sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO), sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2, (sLOAD_SSN[2].sSHIPPINGSN_CODE.Trim().ToUpper() + sLOAD_SSN[2].SSN).Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO)) == false)
                    {
                        CHECKSN_OK = false;
                        return sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 + " Compare Error!!";
                    }
                }
                else
                {
                    if ((CUST_SN.Substring(0, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO)) != (sLOAD_SSN[2].sSHIPPINGSN_CODE.Trim().ToUpper() + sLOAD_SSN[2].SSN).Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO))
                    {
                        CHECKSN_OK = false;
                        return sequenlist[SCAN_POS].STEP + " <> SSN2!";
                    }
                }
            }

            if (await ChkModelType(M_modeltype, editSerialNumber.Text) == false)
            {
                CHECKSN_OK = false;
                return sequenlist[SCAN_POS + 1].STEP + " <> SSN2!";
            }

            if (sLOAD_SSN[SSN_IDX].sCOMPARE_SSN == "SSN1")
            {
                if (sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit == 0)
                {
                    CHECKSN_OK = false;
                    return "SSN1 CHECK STARtDigit = 0!";
                }
                if (sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 != "" && sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 != null)
                {
                    if (ChkCompareSN(CUST_SN.Substring(sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit-1, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO), sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2, (sLOAD_SSN[1].sSHIPPINGSN_CODE.Trim().ToUpper() + sLOAD_SSN[1].SSN).Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO)) == false)
                    {
                        CHECKSN_OK = false;
                        return sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE2 + " Compare Error!!";
                    }
                }
                else
                {
                    if ((CUST_SN.Substring(sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit-1, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO)) != (sLOAD_SSN[1].sSHIPPINGSN_CODE.Trim().ToUpper() + sLOAD_SSN[1].SSN).Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO))
                    {
                        CHECKSN_OK = false;
                        return sequenlist[SCAN_POS].STEP + " <> SSN1!";
                    }
                }
            }

            if (sLOAD_SSN[SSN_IDX].sCOMPARE_SSN == "MAC1")
            {
                if (CUST_SN.Substring(sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO) != (MACID.Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit - 1, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO)))
                {
                    CHECKSN_OK = false;
                    return sequenlist[SCAN_POS].STEP + " <> MAC1!";
                }
            }

            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RANGE == "Y")
            {
                string strGetModel = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='SFISSITE'";
                var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetModel,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Model.Data == null)
                {
                    CHECKSN_OK = false;
                    return "Can not data c104 model_name SFISSITE";
                }
                else
                {
                    MODEL_SERIAL_U_Z = qry_Model.Data["model_serial"]?.ToString() ?? "";
                }

                if ((M_modeltype.IndexOf("189") == -1) || MODEL_SERIAL_U_Z == "U")
                {
                    if ((sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "NetgearRule") || (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "NETGEARRULE"))
                    {
                        tmp_str1 = CUST_SN.Substring(0, 7) + CUST_SN.Substring(8, 5);

                        string strGetRange = $"SELECT * FROM SFISM4.R_MO_EXT3_T  WHERE '{tmp_str1}' <= SUBSTR(ITEM_2,1,7)||SUBSTR(ITEM_2,9,5) AND '{tmp_str1}'>=SUBSTR(ITEM_1,1,7)||SUBSTR(ITEM_1,9,5) AND MO_NUMBER='{Edt_moNumber.Text}'"
                            + $" AND LENGTH('{CUST_SN}')=length(item_1) AND LENGTH('{CUST_SN}')=length(item_2)";
                        var qry_Range = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetRange,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_Range.Data == null)
                        {
                            if (await ChkMMS(CUST_SN, "1") == false)
                            {
                                if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                {
                                    if (await ChkREWORKMO(CUST_SN, "SSN") == false)
                                    {
                                        SSN_OK = false;
                                        return "R_MO_EXT3_T Not This SSN Range!(REWORK)";
                                    }
                                }
                                else
                                {
                                    SSN_OK = false;
                                    return "R_MO_EXT3_T Not This SSN Range!(REWORK)";
                                }
                            }
                        }
                    }
                    else if (M_modeltype.IndexOf("035") > 0)
                    {
                        RESULT = await ChkRange(CUST_SN);
                        if (RESULT != "")
                        {
                            SSN_OK = false;
                            return RESULT;
                        }
                    }
                    else
                    {
                        string sSQL_str = "SELECT * FROM SFISM4.R_MO_EXT3_T WHERE '" + CUST_SN + "' <= item_2 and '" + CUST_SN + "' >=item_1 AND MO_NUMBER= '" + Edt_moNumber.Text + "'"
                            + " AND '" + CUST_SN.Length + "'=length(item_1) AND '" + CUST_SN.Length + "'=length(item_2)";
                        var query_moext3 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = sSQL_str,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if /*(query_moext3.Data.Count() == 0) ||*/ (query_moext3.Data == null)
                        {
                            if (await ChkMMS(CUST_SN, "1") == false)
                            {
                                if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                {
                                    if (await ChkREWORKMO(CUST_SN, "SSN") == false)
                                    {
                                        SSN_OK = false;
                                        return "R_MO_EXT3_T Not This SSN Range!(REWORK)";
                                    }
                                }
                                else
                                {
                                    SSN_OK = false;
                                    return "R_MO_EXT3_T Not This SSN Range!(Normal)";
                                }
                            }
                        }
                    }
                }
                else
                {
                    string strGetShippingSN = $"SELECT DISTINCT MO_NUMBER,MODEL_NAME FROM SFISM4.R_NETG_PRIN_ALL_T WHERE SHIPPING_SN='{CUST_SN}' AND ROWNUM=1";
                    var qry_ShippingSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetShippingSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_ShippingSN.Data["model_name"].ToString() == Edt_modelName.Text)
                    {
                        if (qry_ShippingSN.Data["mo_number"].ToString() != Edt_moNumber.Text)
                        {
                            string strGetMACSSN = $"SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE SSN =  '{CUST_SN}' AND MO_NUMBER = '{Edt_moNumber.Text}' AND '{CUST_SN.Length}'=length(SSN)";
                            var qry_MACSSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetMACSSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_MACSSN.Data.Count() == 0)
                            {
                                SSN_OK = false;
                                return "Diffrent Mo number, have not scan AddRW!";
                            }
                        }
                        else
                        {
                            if ((sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "NetgearRule") || (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "NETGEARRULE"))
                            {
                                tmp_str1 = CUST_SN.Substring(0, 7) + CUST_SN.Substring(8, 5);
                                string strGetRange = $"SELECT * FROM SFISM4.R_MO_EXT3_T WHERE '{tmp_str1}'<= SUBSTR(ITEM_2,1,7)||SUBSTR(ITEM_2,9,5) AND '{tmp_str1}'>=SUBSTR(ITEM_1,1,7)||SUBSTR(ITEM_1,9,5) AND MO_NUMBER='{Edt_moNumber.Text}'"
                                    + $" AND '{CUST_SN.Length}'=length(item_1) and '{CUST_SN.Length}'=length(item_2)";
                                var qry_Range = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetRange,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_Range.Data.Count() == 0)
                                {
                                    if (await ChkMMS(CUST_SN, "1") == false)
                                    {
                                        if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                        {
                                            if (await ChkREWORKMO(CUST_SN, "SSN") == false)
                                            {
                                                SSN_OK = false;
                                                return "R_MO_EXT3_T Not This SSN Range!(REWORK)";
                                            }
                                        }
                                        else
                                        {
                                            SSN_OK = false;
                                            return "R_MO_EXT3_T Not This SSN Range!(Normal)";
                                        }
                                    }
                                }
                            }
                            else if (M_modeltype.IndexOf("035") > -1)
                            {
                                RESULT = await ChkRange(CUST_SN);
                                if (RESULT != "")
                                {
                                    SSN_OK = false;
                                    return RESULT;
                                }
                            }
                            else
                            {
                                string strGetRangeExt3 = $"SELECT * FROM SFISM4.R_MO_EXT3_T WHERE '{CUST_SN}'<= item_2 AND '{CUST_SN}'>=item_1 AND MO_NUMBER='{Edt_moNumber.Text}'"
                                    + $" AND '{CUST_SN.Length}'=length(item_1) AND '{CUST_SN.Length}'=length(item_2)";
                                var qry_RangeExt3 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetRangeExt3,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_RangeExt3.Data.Count() == 0)
                                {
                                    if (await ChkMMS(CUST_SN, "1") == false)
                                    {
                                        if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                        {
                                            if (await ChkREWORKMO(CUST_SN, "SSN") == false)
                                            {
                                                SSN_OK = false;
                                                return "R_MO_EXT3_T Not This SSN Range!(REWORK)";
                                            }
                                        }
                                        else
                                        {
                                            SSN_OK = false;
                                            return "R_MO_EXT3_T Not This SSN Range!(REWORK)";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        SSN_OK = false;
                        return "Diffrent Model number, have not scan AddRW!";
                    }
                }
            }
            sLOAD_SSN[SSN_IDX].SSN_OK = true;
            return "";
        }

        public async Task<string> ChkMAC(string SSN, string SSN_CODE, bool CHECKMAC_OK)
        {
            int MAC_IDX, i;
            string CUST_SN, sIgnoreStr, c_MACID, MODEL_SERIAL_U_Z, tmpcustsn;

            tmpcustsn = "";
            if (sCAPS_LOCK == true)
            {
                CUST_SN = SSN.ToUpper();
                _MAC = CUST_SN;
            }
            else
            {
                CUST_SN = SSN;
                _MAC = CUST_SN;
            }

            if (await ChkDUPMAC(SSN) == "EROR")
            {
                SSN_OK = false;
                return "This MACID Already Shipping,Please Call SFC Engineer!";
            }
            MAC_IDX = Int32.Parse(SSN_CODE.Substring(3));
            sIgnoreStr = (sLOAD_MAC[MAC_IDX].sMACID_CODE) == null ? "" : (sLOAD_MAC[MAC_IDX].sMACID_CODE).Trim();
            if ((sIgnoreStr != "") && (CUST_SN.Substring(0, sIgnoreStr.Length) == sIgnoreStr))
            {
                CUST_SN = CUST_SN.Substring(sIgnoreStr.Length, CUST_SN.Length);
            }
            if ((CUST_SN.Length) != (sLOAD_MAC[MAC_IDX].nMAC_LENGTH))
            {
                SSN_OK = false;
                return "MACID Length Error !";
            }
            if (sLOAD_MAC[MAC_IDX].cMAC_PREFIX != "")
            {
                if (sLOAD_MAC[MAC_IDX].cMAC_PREFIX.IndexOf(CUST_SN.Substring(0, sLOAD_MAC[MAC_IDX].nMAC_PREFIX_LEN)) == -1)
                {
                    SSN_OK = false;
                    return "MACID Prefix Character Error!!";
                }
                if ((CUST_SN.Substring((sLOAD_MAC[MAC_IDX].nMAC_LENGTH - sLOAD_MAC[MAC_IDX].nMAC_POSTFIX_LEN), sLOAD_MAC[MAC_IDX].nMAC_POSTFIX_LEN)) != sLOAD_MAC[MAC_IDX].cMAC_POSTFIX)
                {
                    SSN_OK = false;
                    return "MACID Postfix Character Error!!";
                }
            }
            for (i = 0; i < sLOAD_MAC[MAC_IDX].nMAC_LENGTH; i++)
            {
                if (sLOAD_MAC[MAC_IDX].cMAC_STR.IndexOf(CUST_SN[i]) == -1)
                {
                    SSN_OK = false;
                    return "Shipping SN Format Error!";
                }
            }
            if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "ControlKey")
            {
                if (ControlKey(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "Control key error !!(2 digits)";
                }
            }
            else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "IBMModulo10")
            {
                if (IBMModulo10(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "Control key error !!(2 digits)";
                }
            }
            else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "ODDMAC")
            {
                if (ODDMAC(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "NOT ODDMAC !!";
                }
            }
            else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "MAC3ORMAC4")
            {
                if (MAC_IDX == 4)
                {
                    if (sLOAD_MAC[3].MAC == CUST_SN)
                    {
                        SSN_OK = false;
                        return "MAC3=MAC4 !!";
                    }
                }
                if (await ChkCust(CUST_SN, editSerialNumber.Text) == false)
                {
                    SSN_OK = false;
                    return "MAC NOT MATCH MAC3 OR MAC4 !!";
                }
            }
            else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "EVENMAC")
            {
                if (EVENMAC(CUST_SN))
                {
                    SSN_OK = false;
                    return "NOT EVENMAC !!";
                }
            }
            else if ((sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE != null) && (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE != ""))
            {
                if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE.Substring(0, 7) == "NEXTMAC")
                {
                    if (sLOAD_MAC[MAC_IDX - 1].MAC != "")
                    {
                        if (NEXTMACFULL(sLOAD_MAC[MAC_IDX - 1].MAC, CUST_SN, sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE))
                        {
                            SSN_OK = false;
                            return sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE;
                        }
                    }
                    else
                    {
                        SSN_OK = false;
                        return "PREMAC is NULL !!";
                    }
                }
                else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC1")
                {
                    MACID = SMAC[1].MAC;
                }
                else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC2")
                {
                    MACID = SMAC[2].MAC;
                }
                else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC3")
                {
                    MACID = SMAC[3].MAC;
                }
                else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC4")
                {
                    MACID = SMAC[4].MAC;
                }
                else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "CUSTMAC2")
                {
                    string strGetSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND ROWNUM=1";
                    var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_SN.Data != null)
                    {
                        if (_Inputdata != qry_SN.Data["mac2"].ToString())
                        {
                            SSN_OK = false;
                            return "IPUI NOT MATCH";
                        }
                        else
                        {
                            SSN_OK = false;
                            return "R_CUSTSN_T NO DATA.";
                        }
                    }
                }
            }

            if (SMAC[MAC_IDX].CUSTSN_MAC != "")
            {
                if (M_modeltype.IndexOf("G46") == -1)
                {
                    if (CUST_SN.IndexOf(".") != -1)
                    {
                        MyReplaceString(CUST_SN, ".", "", true);
                        return CUST_SN;
                    }
                    if (CUST_SN.IndexOf(":") != -1)
                    {
                        MyReplaceString(CUST_SN, ".", "", true);
                        return CUST_SN;
                    }
                }
                if (CUST_SN != SMAC[MAC_IDX].CUSTSN_MAC)
                {
                    if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE != "MAC3ORMAC4")
                    {
                        SSN_OK = false;
                        return "MAC:" + CUST_SN + "NOT MACTCH CUSTSN MAC: " + SMAC[MAC_IDX].CUSTSN_MAC + " !!";
                    }
                }
            }

            FindFlag();
            if (M_modeltype == "C")
            {
                c_MACID = await FindBomMAC(editSerialNumber.Text);
                if (c_MACID != "")
                {
                    MACID = c_MACID;
                }
            }
            else if (SMAC[MAC_IDX].CUSTSN_MAC != "")
            {
                MACID = SMAC[MAC_IDX].CUSTSN_MAC;
            }
            if ((MAC_IDX > 1) && (SMAC[MAC_IDX].CUSTSN_MAC == ""))
            {
                MACID = CUST_SN;
            }

            if (CUST_SN != MACID)
            {
                zTemp1 = sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE; 
                zSubstring(ref zTemp1,0, 7);
                if (zTemp1 != "NEXTMAC")
                {
                    if (CUST_SN.Substring(0, 3) == "23S")
                    {
                        if (CUST_SN.Substring(3, 12) != MACID)
                        {
                            SSN_OK = false;
                            return "MACID Not Match !!";
                        }
                    }
                    else
                    {
                        string strGetDataKeyPart = "SELECT a.* FROM SFISM4.R_WIP_KEYPARTS_T a,sfism4.r_wip_tracking_t b ,SFIS1.C_BOM_KEYPART_T c"
                            + $" where a.serial_number=b.serial_number and b.bom_no=c.bom_no  AND C.KEY_PART_NO='MACID' and  a.KEY_PART_NO = 'MACID' and  a.serial_number='{editSerialNumber.Text}'";
                        var qry_KeyPart = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetDataKeyPart,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_KeyPart.Data.Count() > 0)
                        {
                            MACID = qry_KeyPart.Data["key_part_sn"].ToString();
                        }
                        if (CUST_SN != MACID)
                        {
                            SSN_OK = false;
                            return "MACID Not Match !!";
                        }
                    }
                }
            }

            if (sLOAD_MAC[MAC_IDX].sCOMPARE_MAC == "SN")
            {
                zTemp1 = CUST_SN; zTemp2 = editSerialNumber.Text;
                zSubstring(ref zTemp1, sLOAD_MAC[MAC_IDX].nMAC_Self_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Self_FlowNO);
                zSubstring(ref zTemp2, sLOAD_MAC[MAC_IDX].nMAC_Compare_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Compare_FlowNO);
                if (zTemp1 != zTemp2)
                {
                    SSN_OK = false;
                    return "Compare " + sequenlist[SCAN_POS + 1].STEP + " with SN fail,Check with IE(" + zTemp1 + "<>" + zTemp2 + ")";
                }
            }
            if (sLOAD_MAC[MAC_IDX].sCOMPARE_MAC == "SSN1")
            {
                zTemp1 = CUST_SN; zTemp2 = sLOAD_SSN[1].SSN;
                zSubstring(ref zTemp1, sLOAD_MAC[MAC_IDX].nMAC_Self_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Self_FlowNO);
                zSubstring(ref zTemp2, sLOAD_MAC[MAC_IDX].nMAC_Compare_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Compare_FlowNO);
                if (zTemp1 != zTemp2)
                {
                    SSN_OK = false;
                    return "Compare "+ sequenlist[SCAN_POS + 1].STEP + " with SSN1 fail,Check with IE("+ zTemp1+"<>"+ zTemp2+")";
                }
            }
            if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "Y")
            {
                string strGetModel = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='SFISSITE'";
                var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetModel,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (qry_Model.Data == null)
                {
                    SSN_OK = false;
                    return "Can not data c104 model_name SFISSITE";
                }
                else
                {
                    MODEL_SERIAL_U_Z = qry_Model.Data["model_serial"].ToString();
                }

                if (M_modeltype.IndexOf("189") == -1 || MODEL_SERIAL_U_Z == "U")
                {
                    if (CUST_SN.Substring(0, 3) == "23S")
                    {
                        tmpcustsn = CUST_SN.Substring(3, 12);
                        string strGetRange = $"SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '{tmpcustsn}'<= item_2 AND '{tmpcustsn}'>=item_1 AND MO_NUMBER='{Edt_moNumber.Text}'"
                                         + " AND '{tmpcustsn.Length}'=length(item_1) AND '{tmpcustsn.Length}'=length(item_2)";
                        var qry_Range = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetRange,
                            SfcCommandType = SfcCommandType.Text,
                        });
                        if (qry_Range.Data.Count() == 0)
                        {
                            if (await ChkMMS(CUST_SN, "2") == false)
                            {
                                if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                {
                                    if (await ChkREWORKMO(tmpcustsn, "MAC") == false)
                                    {
                                        SSN_OK = false;
                                        return "R_MO_EXT4_T Not This MAC Range!(REWORK)";
                                    }
                                    else
                                    {
                                        SSN_OK = false;
                                        return "R_MO_EXT4_T Not This MAC Range!(Normal)";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string strGetRangeExt4 = $"SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '{CUST_SN}'<= item_2 AND '{CUST_SN}'>=item_1 AND MO_NUMBER='{Edt_moNumber.Text}'"
                                       + $" AND '{ CUST_SN.Length}'=length(item_1) AND '{CUST_SN.Length}'=length(item_2)";
                        var qry_RangeExt4 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetRangeExt4,
                            SfcCommandType = SfcCommandType.Text,
                        });
                        if (qry_RangeExt4.Data.Count() == 0)
                        {
                            if (await ChkMMS(CUST_SN, "2") == false)
                            {
                                if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                {
                                    if (await ChkREWORKMO(tmpcustsn, "MAC") == false)
                                    {
                                        SSN_OK = false;
                                        return "R_MO_EXT4_T Not This MAC Range!(REWORK)";
                                    }
                                    else
                                    {
                                        SSN_OK = false;
                                        return "R_MO_EXT4_T Not This MAC Range!(Normal)";
                                    }
                                }
                            }
                        }

                    }
                }
                else
                {
                    string strGetMACID = $"SELECT DISTINCT MO_NUMBER,model_name FROM SFISM4.R_NETG_PRIN_ALL_T WHERE MACID='{CUST_SN}' OR MACID1='{CUST_SN}' AND ROWNUM=1";
                    var qry_MACID = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetMACID,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (qry_MACID.Data["model_name"].ToString() == Edt_modelName.Text)
                    {
                        if (qry_MACID.Data["mo_number"].ToString() != Edt_moNumber.Text)
                        {
                            string strGetMACSSN = $"SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE MAC = '{CUST_SN}' OR MAC2='{CUST_SN}' AND MO_NUMBER='{Edt_moNumber.Text}'AND '{ CUST_SN.Length}'==length(MAC)";
                            var qry_MACSSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetMACSSN,
                                SfcCommandType = SfcCommandType.Text,
                            });
                            if (qry_MACSSN.Data.Count() == 0)
                            {
                                SSN_OK = false;
                                return "Diffrent Mo number, have not scan AddRW!";
                            }
                        }
                        else
                        {
                            if (CUST_SN.Substring(0, 3) == "23S")
                            {
                                tmpcustsn = CUST_SN.Substring(3, 12);
                                string strGetRangeExt4 = $"SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '{tmpcustsn}'<= item_2 AND '{tmpcustsn}'>=item_1 AND MO_NUMBER = '{Edt_moNumber.Text}' AND '{tmpcustsn.Length}'=length(item_1) AND '{tmpcustsn.Length}'=length(item_2)";
                                var qry_RangeExt4 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetRangeExt4,
                                    SfcCommandType = SfcCommandType.Text,
                                });
                                if (qry_RangeExt4.Data.Count() == 0)
                                {
                                    if (await ChkMMS(CUST_SN, "2") == false)
                                    {
                                        if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                        {
                                            if (await ChkREWORKMO(tmpcustsn, "MAC") == false)
                                            {
                                                SSN_OK = false;
                                                return "R_MO_EXT4_T Not This MAC Range!(REWORK)";
                                            }
                                        }
                                        else
                                        {
                                            SSN_OK = false;
                                            return "R_MO_EXT4_T Not This MAC Range!(Normal)";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                string strGetRangeExt4 = $"SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '{CUST_SN}'<= item_2 AND '{CUST_SN}'>=item_1 AND MO_NUMBER = '{Edt_moNumber.Text}' AND '{tmpcustsn.Length}'=length(item_1) AND '{tmpcustsn.Length}'=length(item_2)";
                                var qry_RangeExt4 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetRangeExt4,
                                    SfcCommandType = SfcCommandType.Text,
                                });
                                if (qry_RangeExt4.Data.Count() == 0)
                                {
                                    if (await ChkMMS(CUST_SN, "2") == false)
                                    {
                                        if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
                                        {
                                            if (await ChkREWORKMO(tmpcustsn, "MAC") == false)
                                            {
                                                SSN_OK = false;
                                                return "R_MO_EXT4_T Not This MAC Range!(REWORK)";
                                            }
                                        }
                                        else
                                        {
                                            SSN_OK = false;
                                            return "R_MO_EXT4_T Not This MAC Range!(Normal)";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        SSN_OK = false;
                        return "Diffrent Model number, have not scan AddRW!!";
                    }
                }
            }

            string strGetCustSN = "select * from SFISM4.R_CUSTSN_T"
                + $" where serial_number= '{editSerialNumber.Text}'"
                + $" and MAC" + MAC_IDX.ToString() + " IS NOT NULL";
            var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustSN,
                SfcCommandType = SfcCommandType.Text,
            });
            if (qry_CustSN.Data.Count() > 0)
            {
                string strGetCustMAC = "select * from SFISM4.R_CUSTSN_T"
                   + $" where serial_number= '{editSerialNumber.Text}'"
                   + $" and MAC" + MAC_IDX.ToString() + " = '" + CUST_SN + "'";
                var qry_CustMAC = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustMAC,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (qry_CustMAC.Data.Count() == 0)
                {
                    SSN_OK = false;
                    return "This '" + CUST_SN + "' MAC And R_CUSTSN_T Not Match";
                }
            }

            //if (sLOAD_SSN[MAC_IDX].sCHECK_SSN_RULE != "NOCHECKDUPMAC")
            //{
            //    string strGetCustMAC = "select * from SFISM4.R_CUSTSN_T"
            //      + " where MAC" + MAC_IDX.ToString() + " ='" + CUST_SN + "' AND SERIAL_NUMBER<>'" + editSerialNumber.Text + "'";
            //    var qry_CustMAC = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            //    {
            //        CommandText = strGetCustMAC,
            //        SfcCommandType = SfcCommandType.Text,
            //    });
            //    if (qry_CustMAC.Data.Count() > 0)
            //    {
            //        SSN_OK = false;
            //        return "Same mac found; Duplicate MAC '" + MAC_IDX.ToString() + "': '" + CUST_SN + "'!!";
            //    }


            //    string strGetHCustSN = "select * from SFISM4.R_CUSTSN_T_BAK"
            //      + " where MAC" + MAC_IDX.ToString() + " ='" + CUST_SN + "' AND SERIAL_NUMBER<>'" + editSerialNumber.Text + "'";
            //    var qry_HCustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            //    {
            //        CommandText = strGetHCustSN,
            //        SfcCommandType = SfcCommandType.Text,
            //    });
            //    if (qry_HCustSN.Data.Count() > 0)
            //    {
            //        SSN_OK = false;
            //        return "Same mac found; Duplicate MAC '" + MAC_IDX.ToString() + "': '" + CUST_SN + "'!!";
            //    }
            //}
            sLOAD_MAC[MAC_IDX].MAC_OK = true;
            return "";
        }
        public async Task<bool> CHECKNOHHINPUT(string _paramSN)
        {
            string strGetSN = $"SELECT a.group_name,b.model_name,a.mo_number,b.model_type FROM SFISM4.R_WIP_TRACKING_T A,SFIS1.C_MODEL_DESC_T B WHERE B.MODEL_NAME = A.MODEL_NAME AND A.WIP_GROUP='STOCKIN' AND A.SERIAL_NUMBER='{_paramSN}'";
            var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetSN,
                SfcCommandType = SfcCommandType.Text
            });

            if (qry_SN.Data != null)
            {
                string sModel_Type = qry_SN.Data["model_type"].ToString();
                if (sModel_Type.IndexOf("203") != -1)
                {
                    string strGetCountMAC = $"select count(*) aa from sfism4.r_wip_tracking_t where scrap_flag='0' and wip_group='PACK_BOXII' and serial_number like '{_paramSN}%' ";
                    var qry_CountMAC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetCountMAC,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (Int32.Parse(qry_CountMAC.Data["aa"].ToString()) == 1)
                    {
                        string strGetMAC = $"select * from sfism4.r_wip_tracking_t where scrap_flag='0' and wip_group='PACK_BOXII' and serial_number like'{_paramSN}%'";
                        var qry_MAC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetMAC,
                            SfcCommandType = SfcCommandType.Text
                        });

                        _Inputdata = qry_MAC.Data["serial_number"].ToString();

                        //Chk Substring(serial_number voi shipping_sn2 no same)
                        string strChkShipping_SN2 = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T where  serial_number = '{_Inputdata}' and SHIPPING_SN2 ='{_Inputdata.Substring(0, 12)}'";
                        var ChkShipping_SN2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strChkShipping_SN2,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (ChkShipping_SN2.Data.Count() == 0)
                        {
                            lbError.Text = "Du lieu MAC vs SHIPPING_SN2 khong giong nhau";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "Du lieu MAC vs SHIPPING_SN2 khong giong nhau";
                            _er.MessageEnglish = "Data MAC and SHIPPING_SN2 no same";
                            _er.ShowDialog();
                            return false;
                        }

                        //Chk du lieu trong CustSN va R108
                        string strGetCustSN = $"select mac1,mac2 from SFISM4.R_CUSTSN_T where serial_number='{_Inputdata}'";
                        var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetCustSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_CustSN.Data == null)
                        {
                            lbError.Text = "No data CUSTSN,Call IT Check";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "No data CUSTSN,Call IT Check";
                            _er.MessageVietNam = "Khong co du lieu CustSN,goi IT Check";
                            _er.ShowDialog();
                            return false;
                        }
                        else
                        {
                            string mac1 = qry_CustSN.Data["mac1"].ToString();
                            string mac2 = qry_CustSN.Data["mac2"].ToString();

                            string strGetKeyPart = $"select * from SFISM4.R_WIP_KEYPARTS_T where serial_number='{_Inputdata}' and key_part_sn='{mac1}'";
                            var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strGetKeyPart,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_KeyPart.Data.Count() == 0)
                            {
                                lbError.Text = "Data in CUSTSN and KeyPart no same,Call IT Check";
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = "Data in CUSTSN and KeyPart no same,Call IT Check";
                                _er.MessageVietNam = "Du lieu trong CustSN va KeyPart khac nhau,goi IT Check";
                                _er.ShowDialog();
                                return false;
                            }
                            else
                            {
                                string strGetKeyPart2 = $"select * from SFISM4.R_WIP_KEYPARTS_T where serial_number='{_Inputdata}' and key_part_sn='{mac2}'";
                                var qry_KeyPart2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = strGetKeyPart2,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_KeyPart2.Data.Count() == 0)
                                {
                                    lbError.Text = "Data in CUSTSN and KeyPart no same,Call IT Check";
                                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "Data in CUSTSN and KeyPart no same,Call IT Check";
                                    _er.MessageVietNam = "Du lieu trong CustSN va KeyPart khac nhau,goi IT Check";
                                    _er.ShowDialog();
                                    return false;
                                }
                            }
                        }
                        return true;
                    }
                    else
                    {
                        if (Int32.Parse(qry_CountMAC.Data["aa"].ToString()) > 1)
                        {
                            lbError.Text = "00693 - " + await GetPubMessage("00693");
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private async Task<string> ChkRange(string SSN)
        {
            string CUST_SN;
            CUST_SN = SSN;
            string strGetRangeExt4 = $"SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '{CUST_SN}' <= item_2 AND '{CUST_SN}' >=item_1 "
                + $" AND MO_NUMBER = '{Edt_moNumber.Text}' AND LENGTH('{CUST_SN}')=length(item_1)"
                + $" AND LENGTH('{CUST_SN}')=length(item_2)";
            var qry_RangeExt4 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRangeExt4,
                SfcCommandType = SfcCommandType.Text,
            });
            if (qry_RangeExt4.Data.Count() == 0)
            {
                string strGetRange2 = "SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '" + CUST_SN.Substring(1, CUST_SN.Length) + "' <= item_2 AND '" + CUST_SN.Substring(0, CUST_SN.Length - 1) + "' >=item_1"
                    + " AND MO_NUMBER = '" + Edt_moNumber.Text + "' AND"
                    + " '" + (CUST_SN.Substring(0, CUST_SN.Length - 1)).Length + "'=length(item_1) AND"
                    + " '" + (CUST_SN.Substring(0, CUST_SN.Length - 1)).Length + "'=length(item_2)";
                var qry_Range2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetRange2,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (qry_Range2.Data.Count() == 0)
                {
                    string strGetRangeExt3 = $"SELECT * FROM SFISM4.R_MO_EXT3_T WHERE '{CUST_SN}'<= item_2  and '{CUST_SN}'>=item_1 AND MO_NUMBER='{Edt_moNumber.Text}'"
                        + $" '{CUST_SN.Length}'=length(item_1) AND '{CUST_SN.Length}'=length(item_2)";
                    var qry_RangeExt3 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetRangeExt3,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (qry_RangeExt3.Data.Count() == 0)
                    {
                        return "Range Error!";
                    }

                }
            }
            else
            {
                return "";
            }
            return "";
        }

        private bool NEXTMACFULL(string Strtocheck1, string Strtocheck, string mactype)
        {
            if (Strtocheck1.StartsWith("23S")) Strtocheck1 = Strtocheck1.Substring(3, Strtocheck1.Length - 3);
            if (Strtocheck.StartsWith("23S")) Strtocheck = Strtocheck.Substring(3, Strtocheck.Length - 3);
            string str, retstr;
            int step;
            retstr = "";
            if (mactype == "NEXTMAC")
            {
                step = 1;
            }
            else
            {
                step = Int32.Parse(mactype.Substring(7, mactype.Length - 7));
            }
            str = "0123456789ABCDEF";
            if (string.IsNullOrEmpty(Strtocheck1))
            {
                return false;
            }

            for (int i = 0; i < step; i++)
            {
                retstr = F_DEFINE_SN_NEXT(Strtocheck1, str, 1, 12);
                Strtocheck1 = retstr;
            }
            if (retstr == Strtocheck)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private string F_DEFINE_SN_NEXT(string STR1, string DEFINE_STR, int INT2, int INT3)
        {
            string tmpstring_1; // STATIC STRING
            string tmpstring_2;// FLOW NUMBER
            string tmpstring_3;// FLOW NUMBER
            int k;
            string AlphabeticSets;
            string LASTSTR;
            bool FLAG;

            AlphabeticSets = DEFINE_STR;
            AlphabeticSets = AlphabeticSets + AlphabeticSets.Substring(0, 1);
            LASTSTR = AlphabeticSets.Substring(AlphabeticSets.Length-2, 1);
            tmpstring_1 = STR1.Substring(0, INT2 - 1);
            tmpstring_2 = STR1.Substring(INT2-1, INT3);
            tmpstring_3 = STR1.Substring(tmpstring_1.Length + tmpstring_2.Length-1, STR1.Length - tmpstring_1.Length - tmpstring_2.Length);
            FLAG = true;

            for (k = tmpstring_2.Length; k > 1; k--)
            {
                if (FLAG == true)
                {
                    if (tmpstring_2.Substring(k - 1, 1) == LASTSTR)
                    {
                        FLAG = true;
                    }
                    else
                    {
                        FLAG = false;
                    }
                    tmpstring_2 = tmpstring_2.Substring(0, k - 1) + F_NEXT_CHAR(AlphabeticSets, tmpstring_2.Substring(k - 1, 1)) + tmpstring_2.Substring(k, tmpstring_2.Length - k);
                }
            }
            return tmpstring_1 + tmpstring_2 + tmpstring_3;
        }
        private string MyReplaceString(string S, string Token,string NewToken,bool bCaseSensitive)
        {
            int I;
            string sFirstPart;
            sFirstPart = "";
            _CUSTSN_STR = S;
            if (bCaseSensitive == true)
            {
                I = _CUSTSN_STR.IndexOf(Token);
            }
            else
            {
                I = _CUSTSN_STR.ToUpper().IndexOf(Token.ToUpper());
            }
            if (I != -1)
            {
                sFirstPart = _CUSTSN_STR.Substring(0, I) + NewToken;
                _CUSTSN_STR = _CUSTSN_STR.Substring(I + Token.Length);
            }
            if (I != -1)
            {
                MyReplaceString(_CUSTSN_STR, Token, NewToken, bCaseSensitive);
                _CUSTSN_STR = sFirstPart + _CUSTSN_STR;
            }
            return _CUSTSN_STR;
        }
        private string F_NEXT_CHAR(string STR1, string STR2)
        {
            int m;
            for (m = 1; m < STR1.Length - 1; m++)
            {
                if (STR1.Substring(m - 1, 1) == STR2)
                {
                    break;
                }
                //return STR1.Substring(m, 1);
            }
            return STR1.Substring(m, 1);
        }
        private bool EVENMAC(string Strtocheck)
        {
            int retint, STRLENGTH, tmpj;
            string str;
            char ch1;
            retint = 0;
            tmpj = 0;
            STRLENGTH = Strtocheck.Length;
            str = Strtocheck.Substring(STRLENGTH, 1);
            ch1 = str[1];

            retint = tmpj;
            retint = tmpj % 2;
            if (retint == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void item_station_Click(object sender, RoutedEventArgs e)
        {
            Station_Setup formStationSetup = new PACK_BOX.Station_Setup(this, sfcClient);
            formStationSetup.ShowDialog();
            lbTitle.Content = M_sThisGroup;
        }
        private void menu_item_exit_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageBox.Show("Close Window!!", "System Message", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (message == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
        private void menu_item_help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Call IT SFC", "System Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void item_nocheckmo1_Cick(object sender, RoutedEventArgs e)
        {
            FormNo_Check_Mo frm_no_check = new FormNo_Check_Mo(this,sfcClient);
            frm_no_check.ShowDialog();
        }
        private void item_automation_Click(object sender, RoutedEventArgs e)
        {
            FormKeyPart frm_keypart = new FormKeyPart(this,sfcClient);
            frm_keypart.ShowDialog();
        }
        private void item_writeLog_Click(object sender, RoutedEventArgs e)
        {
            if (item_writeLog.IsChecked == true)
            {
                Writelog.WriteLog("Writelog open");
            }
            else
            {
                Writelog.WriteLog("Writelog close");
            }
        }
        private async void Input_Pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(Edt_pass.Password))
                {
                    string strGetEMP = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO='LOCALLABEL'";
                    var qry_EMP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetEMP,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_EMP.Data != null)
                    {
                        if (qry_EMP.Data["emp_pass"].ToString() == Edt_pass.Password)
                        {
                            item_Nocheck.IsChecked = true;
                            Edt_pass.Visibility = Visibility.Hidden;
                            lbinputPass.Visibility = Visibility.Hidden;
                            lbError.Text = "Pass Correct!!";
                        }
                        else
                        {
                            lbError.Text = "PASSWORD WRONG,NOT MATCH SFIS1.C_EMP_DESC_T WHERE EMP_NO=LOCALLABEL";
                            Edt_pass.SelectAll();
                            Edt_pass.Focus();
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "PASSWORD SAI,KHONG KHOP VOI SFIS1.C_EMP_DESC_T WHERE EMP_NO=LOCALLABEL";
                            _er.MessageEnglish = "PASSWORD WRONG,NOT MATCH SFIS1.C_EMP_DESC_T WHERE EMP_NO=LOCALLABEL";
                            _er.ShowDialog();
                            return;
                        }
                    }
                    else
                    {
                        lbError.Text = "KHONG CO DU LIEU TRONG BANG NAY!";
                        Edt_pass.SelectAll();
                        Edt_pass.Focus();
                        ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "KHONG CO DU LIEU TRONG BANG NAY!";
                        _er.MessageEnglish = "NO DATA TO TABLE!";
                        _er.ShowDialog();
                        return;
                    }
                }
                else
                {
                    lbError.Text = "Data is null!";
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Vui long nhap du lieu!";
                    _er.MessageEnglish = "Data is null!";
                    _er.ShowDialog();
                }
            }
        }
        private void item_NOCHECK_Click(object sender, RoutedEventArgs e)
        {
            if (item_Nocheck.IsChecked == true)
            {
                Edt_pass.Visibility = Visibility.Visible;
                lbinputPass.Visibility = Visibility.Visible;
                Edt_pass.Focus();
            }
            else
            {
                Edt_pass.Visibility = Visibility.Hidden;
                lbinputPass.Visibility = Visibility.Hidden;
                InputData.Focus();
            }
        }
        private void item_2Dinput_Click(object sender, RoutedEventArgs e)
        {
            if (edtpassword.Text != null)
            {
                FormScan formscan = new FormScan(Receiver);
                formscan.ShowDialog();
            }
            else
            {
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Password is Null!!";
                _er.MessageEnglish = "Password is Null!!";
                _er.ShowDialog();
            }
        }

        private void item_SETsn_Click(object sender, RoutedEventArgs e)
        {
            SetSN_Location formSetSNLocation = new PACK_BOX.SetSN_Location(this, sfcClient);
            formSetSNLocation.ShowDialog();
            //item_Setsn.IsChecked = !item_Setsn.IsChecked;
        }
        private void CAPS_LOCK_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            if (CAPS_LOCK.IsChecked == true)
            {
                sCAPS_LOCK = true;
                lbError.Text = "Caps Lock!";
            }
            else
            {
                sCAPS_LOCK = false;
                lbError.Text = "Unchecked Caps Lock!";
            }

            ini.Write("PrePacking", "CAPS_LOCK", sCAPS_LOCK.ToString());
        }
        private void item_scan_sn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(item_scan.IsChecked.ToString());
        }
        private async Task<bool> ChkREWORKMO(string Strtocheck, string SSN_IDX)
        {
            if ((sMO_TYPE == "Rework") || (sMO_TYPE == "RMA"))
            {
                if (SSN_IDX == "SSN")
                {
                    string strGetSSN = $"SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE SSN = '{Strtocheck}' AND MO_NUMBER = '{Edt_moNumber.Text}' AND LENGTH('{Strtocheck}')=length(SSN)";
                    var qry_SSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetSSN,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (qry_SSN.Data.Count() > 0)
                    {
                        return true;
                    }
                }
                else if (SSN_IDX == "MAC")
                {
                    string strGetMAC = $"SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE MAC =  '{Strtocheck}' AND MO_NUMBER = '{Edt_moNumber.Text}' AND LENGTH('{Strtocheck}')=length(MAC)";
                    var qry_MAC = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetMAC,
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (qry_MAC.Data.Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void item_cb_rework_Click(object sender, RoutedEventArgs e)
        {
            if (cb_rework.IsChecked == true)
            {
                if (InputData.IsEnabled == true)
                {
                    cb_witemac.Visibility = Visibility.Visible;
                    cb_custsn.Visibility = Visibility.Visible;
                    //cb_chk1.Visibility = Visibility.Visible;
                    InputData.Focus();
                }
            }
            else
            {
                lbinputPass.Visibility = Visibility.Hidden;
                cb_witemac.Visibility = Visibility.Hidden;
                cb_witemac.IsChecked = false;
                cb_custsn.Visibility = Visibility.Hidden;
                cb_custsn.IsChecked = false;
                //cb_chk1.Visibility = Visibility.Hidden;
                //cb_chk1.IsChecked = false;
            }
        }
        private async Task EdmacInputData()
        {
            string sMAC1, sMAC2, SSN_N3, tmp_str, tmp_serial, MODEL_N;
            try
            {
                if (CAPS_LOCK.IsChecked == true)
                {
                    _Inputdata = _Inputdata.ToUpper();
                }
                MODELSTR = "";
                if (_Inputdata.IndexOf("SERIAL NUMBER:") > -1)
                {
                    _Inputdata = _Inputdata.Substring((_Inputdata.IndexOf("MAC")) + 13, 12);
                }
                else
                {
                    if (_Inputdata.IndexOf(",") != -1)
                    {
                        sMAC1 = _Inputdata.Substring(14, _Inputdata.Length - 15);
                        sMAC2 = sMAC1.Substring(0, 12);

                        string strGetShippingSN2 = $"SELECT * FROM SFISM4.R107 WHERE SHIPPING_SN2={sMAC2}";
                        var qry_ShippingSN2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetShippingSN2,
                            SfcCommandType = SfcCommandType.Text,
                        });
                        if (qry_ShippingSN2.Data.Count() > 0)
                        {
                            _Inputdata = sMAC2;
                        }
                    }
                    else
                    {
                        if (_Inputdata.IndexOf(":") > -1)
                        {
                            sMAC1 = _Inputdata.Substring(_Inputdata.Length - 17, 17);
                            string strGetShippingSN2 = $"SELECT * FROM SFISM4.R107 WHERE  shipping_sn2=lower(replace('{sMAC1}',':',''))";
                            var qry_ShippingSN2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetShippingSN2,
                                SfcCommandType = SfcCommandType.Text,
                            });
                            if (qry_ShippingSN2.Data != null)
                            {
                                _Inputdata = qry_ShippingSN2.Data["shipping_sn2"].ToString();
                            }
                        }
                    }
                }

                if (cb_custsn.IsChecked == true)
                {
                    string strGetShippingSN = $"select * from sfism4.r_wip_tracking_t where shipping_sn = '{_Inputdata}'";
                    var qry_ShippingSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetShippingSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_ShippingSN.Data == null)
                    {
                        string strGetSSN1 = $"select * from sfism4.r_custsn_t where SSN1 = '{_Inputdata}'";
                        var qry_SSN1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetSSN1,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter{Name="sSerN",Value=_Inputdata}
                        }
                        });
                        if (qry_SSN1.Data == null)
                        {
                            lbError.Text = " Not Has SHIPPING SN !!";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "R107 khong ton tai SHIPPING SN!!";
                            _er.MessageEnglish = "R107 In Not Has SHIPPING SN!!";
                            _er.ShowDialog();
                            InputData.Focus();
                            InputData.SelectAll();
                            return;
                        }
                        else
                        {
                            _Inputdata = qry_SSN1.Data["serial_number"].ToString();
                            InputData.SelectAll();
                        }

                        string strGetSSN2 = "select (shipping_sn2) SSN_N2  from sfism4.r107"
                            + $" where SERIAL_NUMBER = '{_Inputdata}' or SHIPPING_SN='{_Inputdata}'";
                        var qry_SSN2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetSSN2,
                            SfcCommandType = SfcCommandType.Text
                        });
                        SSN_N3 = qry_SSN2.Data["ssn_n2"].ToString();

                        string strGetSerialNumber = "select *  from sfism4.r107 where serial_number in("
                        + " select serial_number  from sfism4.R_CUSTSN_T_BAK"
                        + $" where serial_number = '{SSN_N3}' or ssn1='{SSN_N3}' or mac1='{SSN_N3}')";
                        var qry_SerialNumber = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetSerialNumber,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_SerialNumber.Data.Count() != 0)
                        {
                            try
                            {
                                string Insert_Log = "insert into sfism4.r_system_log_t(emp_no,prg_name,action_type,action_desc)"
                                + " values"
                                + " (:emp,:prg_name,:action_type,:action_desc)";
                                var insert_sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = Insert_Log,
                                    SfcCommandType = SfcCommandType.Text,
                                    SfcParameters = new List<SfcParameter>
                                    {
                                        new SfcParameter{Name="emp",Value="V500304"},
                                        new SfcParameter{Name="prg_name",Value="PACK_BOX"},
                                        new SfcParameter{Name="action_type",Value="SCAN"},
                                        new SfcParameter{Name="action_desc",Value="DUPPLICATE : " + SSN_N3},
                                    }
                                });
                            }
                            catch (Exception)
                            {
                                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            lbError.Text = $"DUPPLICATE: '{SSN_N3}'!!Please check R_CUSTSN_T_BAK!!";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = $"DUPPLICATE: '{SSN_N3}' !!Please check R_CUSTSN_T_BAK !!";
                            _er.MessageEnglish = $"DUPPLICATE: '{SSN_N3}' !!Please check R_CUSTSN_T_BAK !!";
                            _er.ShowDialog();
                            return;
                        }
                    }
                    else
                    {
                        _Inputdata = qry_ShippingSN.Data["serial_number"].ToString();
                        InputData.Focus();
                        InputData.SelectAll();
                    }
                }
                else
                {
                    if ((cb_rework.IsChecked == true))
                    {
                        string strGetMAC1 = $"select * from sfism4.r_custsn_t where mac1 = '{_Inputdata}' and rownum = 1";
                        var qry_MAC1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetMAC1,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_MAC1.Data == null)
                        {
                            string strGetKeyPartSN = $"select * from sfism4.r_wip_keyparts_t where KEY_PART_SN = '{_Inputdata}'";
                            var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetKeyPartSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_KeyPartSN.Data == null)
                            {
                                string strGetHKeyPart = $"select * from sfism4.h_wip_keyparts_t where KEY_PART_SN = '{_Inputdata}'";
                                var qry_HKeyPart = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetHKeyPart,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_HKeyPart.Data == null)
                                {
                                    // The MAC LINK Serial Number not found
                                    lbError.Text = "R108 Not Found This KEY_PART_SN !!";
                                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = "R108 khong dung KEY_PART_SN !!";
                                    _er.MessageEnglish = "R108 Not Found This KEY_PART_SN !!";
                                    _er.ShowDialog();
                                    InputData.Focus();
                                    InputData.SelectAll();
                                    return;
                                }
                                tmp_str = qry_HKeyPart.Data["serial_number"].ToString();
                                _Inputdata = qry_HKeyPart.Data["serial_number"].ToString();
                            }
                            else
                            {
                                tmp_str = qry_KeyPartSN.Data["serial_number"].ToString();
                                _Inputdata = qry_KeyPartSN.Data["serial_number"].ToString();
                            }
                        }
                        else
                        {
                            tmp_str = qry_MAC1.Data["serial_number"].ToString();
                            _Inputdata = qry_MAC1.Data["serial_number"].ToString();
                        }
                    }
                    else
                    {
                        string strGetShippingSN = $"SELECT A.*,B.MODEL_TYPE MODELSTR FROM SFISM4.R107 A,SFIS1.C_MODEL_DESC_T B WHERE A.SHIPPING_SN='{_Inputdata}' AND A.MODEL_NAME=B.MODEL_NAME";
                        var qry_ShippingSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetShippingSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_ShippingSN.Data != null)
                        {
                            MODEL_N = qry_ShippingSN.Data["model_name"].ToString();
                            MODELSTR = qry_ShippingSN.Data["modelstr"].ToString();
                        }
                        if (MODELSTR.IndexOf("G34") > -1)
                        {
                            tmp_str = qry_ShippingSN.Data["serial_number"].ToString();
                        }
                        else
                        {
                            string strGetKeyPartSN = $"select * from sfism4.r_wip_keyparts_t where KEY_PART_SN = '{_Inputdata}'";
                            var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetKeyPartSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_KeyPartSN.Data == null)
                            {
                                string strGetHKeyPartSN = $"select * from sfism4.h_wip_keyparts_t where KEY_PART_SN = '{_Inputdata}'";
                                var qry_HKeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetHKeyPartSN,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_HKeyPartSN.Data == null)
                                {
                                    lbError.Text = "R108 Not Found This KEY_PART_SN !!";
                                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = "R108 khong dung KEY_PART_SN !!";
                                    _er.MessageEnglish = "R108 Not Found This KEY_PART_SN !!";
                                    _er.ShowDialog();
                                    InputData.Focus();
                                    InputData.SelectAll();
                                    return;
                                }
                                tmp_str = qry_HKeyPartSN.Data["serial_number"].ToString();
                                _Inputdata = qry_HKeyPartSN.Data["serial_number"].ToString();
                            }
                            else
                            {
                                tmp_str = qry_KeyPartSN.Data["serial_number"].ToString();
                                _Inputdata = qry_KeyPartSN.Data["serial_number"].ToString();
                            }
                        }
                    }

                    if (cb_witemac.IsChecked == true)
                    {
                        string strGetKeyPartSN = $"select * from sfism4.r_wip_keyparts_t where KEY_PART_SN = '{tmp_str}'";
                        var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKeyPartSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_KeyPartSN.Data == null)
                        {
                            string strGetHKeyPartSN = $"select * from sfism4.h_wip_keyparts_t where KEY_PART_SN = '{tmp_str}'";
                            var qry_HKeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetHKeyPartSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_HKeyPartSN.Data == null)
                            {
                                lbError.Text = "R108 Not Found This KEY_PART_SN !!";
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "R108 khong dung KEY_PART_SN !!";
                                _er.MessageEnglish = "R108 Not Found This KEY_PART_SN !!";
                                _er.ShowDialog();
                                InputData.Focus();
                                InputData.SelectAll();
                                return;
                            }
                            tmp_serial = qry_HKeyPartSN.Data["serial_number"].ToString();
                            _Inputdata = qry_HKeyPartSN.Data["serial_number"].ToString();
                        }
                        else
                        {
                            tmp_serial = qry_KeyPartSN.Data["serial_number"].ToString();
                            _Inputdata = qry_KeyPartSN.Data["serial_number"].ToString();
                        }
                        InputData.Focus();
                        _Inputdata = tmp_serial;
                        InputData.SelectAll();
                    }
                    else
                    {
                        InputData.Focus();
                        //InputData.SelectAll();
                    }
                }
            }
            catch (Exception e)
            {
                lbError.Text = e.Message;
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = e.Message;
                _er.MessageEnglish = e.Message;
                _er.ShowDialog();
                return;
            }

        }
        private async Task<bool> ChkCPEI()
        {
            string strGetModel = "SELECT MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME ='SFISSITE'";
            var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModel,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Model.Data == null)
            {
                return false;
            }
            if (qry_Model.Data["model_serial"].ToString() == "U")
            {
                return true;
            }
            return true;
        }
        public async Task<string> FindBomMAC(string SN)
        {
            string sBomNo;
            string strGetSN = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER=" + SN + "";
            var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetSN,
                SfcCommandType = SfcCommandType.Text
            });

            sBomNo = qry_SN.Data["bom_no"].ToString();

            string strGetBomNo = "SELECT * FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO=" + sBomNo + "";
            var qry_BomNo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetBomNo,
                SfcCommandType = SfcCommandType.Text
            });

            for (int i = 1; i < 4; i++)
            {
                if (SMAC[i].MODEL_NAME == qry_BomNo.Data["key_part_no"].ToString())
                {
                    return SMAC[i].MAC;
                }
            }
            return "";
        }
        private async Task<bool> ChkMMS(string Strtocheck, string SSN_IDX)
        {
            string strGetModel = $"SELECT * FROM SFIS1.C_MODEL_NOMMS_T WHERE MODEL_NAME = '{Edt_modelName.Text}'";
            var qry_Model = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModel,
                SfcCommandType = SfcCommandType.Text,
            });
            if (qry_Model.Data.Count() > 0)
            {
                string strGetRange = $"SELECT * FROM SFISM4.R_MO_EXT2_T WHERE '{Strtocheck}' <= item_2 AND '{Strtocheck}' >=item_1"
                    + $" AND MO_NUMBER = '{Edt_moNumber.Text}' AND VER_3 ='{SSN_IDX.Trim()}'"
                    + $" AND LENGTH('{Strtocheck}')=length(item_1) AND LENGTH('{Strtocheck}')=length(item_2)";
                var qry_Range = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetRange,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (qry_Range.Data.Count() == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        private async Task<bool> ChkModelType(string M_modeltype, string SN)
        {
            if (M_modeltype.IndexOf("007") > -1)
            {
                string strGetData = "SELECT count(*) mcount FROM SFISM4.R_custsn_T a, SFISM4.R_INVENTEL_DATA_T  b"
                    + $" WHERE a.serial_number ='{SN}' and substr(a.ssn1,2,length(a.ssn1))=b.WEPKEY"
                    + " and a.mac3=b.mac AND a.ssn3= b.SSN";
                var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text
                });
                if (Int32.Parse(qry_Data.Data["mcount"].ToString()) == 0)
                {
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "007 - R_CUSTSN_T And R_INVENTEL_DATA_T khac nhau!";
                    _er.MessageEnglish = "007 - R_CUSTSN_T And R_INVENTEL_DATA_T Data Discrepancy!";
                    _er.ShowDialog();
                    return false;
                }
            }

            if (M_modeltype.IndexOf("028") > -1)
            {
                string strGetData = "SELECT count(*) mcount FROM SFISM4.R_custsn_T a, SFISM4.R_INVENTEL_DATA_T  b"
                   + $" WHERE a.serial_number ='{SN}' and a.ssn1=B.SSN"
                   + " and a.mac1=b.mac AND a.MAC2= b.WEPKEY";
                var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text
                });
                if (Int32.Parse(qry_Data.Data["mcount"].ToString()) == 0)
                {
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "028 - R_CUSTSN_T And R_INVENTEL_DATA_T khac nhau!";
                    _er.MessageEnglish = "028 - R_CUSTSN_T And R_INVENTEL_DATA_T Data Discrepancy!";
                    _er.ShowDialog();
                    return false;
                }
            }
            return true;
        }
        private bool ChkCompareSN(string CUST_SN, string SHIPPINGSN_CODE2, string COMPareSSN)
        {
            int comsn;
            string str1, str2;
            try
            {
                comsn = Int32.Parse(SHIPPINGSN_CODE2.Substring(SHIPPINGSN_CODE2.IndexOf(":") + 1, (SHIPPINGSN_CODE2.IndexOf(",") - (SHIPPINGSN_CODE2.IndexOf(":"))) - 1));
                comsnchar = SHIPPINGSN_CODE2.Substring(SHIPPINGSN_CODE2.IndexOf(",") + 1, SHIPPINGSN_CODE2.Length - (SHIPPINGSN_CODE2.IndexOf(",")));
            }
            catch
            {
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = SHIPPINGSN_CODE2 + " dinh dang loi!!";
                _er.MessageEnglish = SHIPPINGSN_CODE2 + " Defined Format Error,Should,COMSN:Start the first few,Ignore the fixed characters !!";
                _er.ShowDialog();
                return false;
            }
            if (CUST_SN.Substring(comsn, comsnchar.Length) != comsnchar)
            {
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Khong dung !!";
                _er.MessageEnglish = "NOT MATCH !!";
                _er.ShowDialog();
                return false;
            }
            if (comsn == 1)
            {
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Defined dinh dang loi !!";
                _er.MessageEnglish = "Defined Format Error !!";
                _er.ShowDialog();
                return false;
            }
            else if (comsn > 1)
            {
                str1 = (CUST_SN.Substring(1, comsn - 1) + CUST_SN.Substring(comsn + comsnchar.Length, CUST_SN.Length - comsn - comsnchar.Length + 1));
                str2 = (COMPareSSN.Substring(1, comsn - 1) + COMPareSSN.Substring(comsn + comsnchar.Length, COMPareSSN.Length - comsn - comsnchar.Length + 1));
                if (str1 != str2)
                {
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Compare Fail !!";
                    _er.MessageEnglish = "Compare Fail !!";
                    _er.ShowDialog();
                    return false;
                }
            }
            return true;
        }
        private string GET2DSTRING()
        {
            int i, j;
            string TMP, RESULT;
            i = LB_2D.Items.Count;
            j = 0;
            RESULT = "";
            while (i<j)
            {
                TMP = LB_2D.Items[j].ToString();
                if (TMP != "")
                {
                    return TMP;
                }
                j = j + 1;
            }
            return RESULT.Substring(1,Int32.MaxValue);
        }

        private async Task Save2DInfoAsync(string _SN,string _BC)
        {
            string tmpa, tmpb;
            tmpa = _SN.Substring(0, 25);
            tmpb = _SN.Substring(0, 100);
            try
            {
                string strInsert = "insert into sfis1.C_2D_CODE_T(sn,bc,in_station_time) values ('" + tmpa + "','" + tmpb + "',sysdate)";
                var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strInsert,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            catch (Exception)
            {
                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private string expandSpecialStr(string _str)
        {
            _CUSTSN_STR = "";
            MyReplaceString(_str, "a-z", "abcdefghijklmnopqrstuvwxyz", true);
            MyReplaceString(_CUSTSN_STR, "A-Z", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", true);
            MyReplaceString(_CUSTSN_STR, "0-9", "0123456789", true);
            return _CUSTSN_STR;
        }
        public bool CHECKMODELSSN(string SSNPREFIX, string Strtocheck)
        {
            if (SSNPREFIX == Strtocheck)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<bool> SonyRule(string Strtocheck)
        {
            char chr;
            string Str1;

            if (Strtocheck.Length != 11)
            {
                lbError.Text = await GetPubMessage("00349");
            }

            for (int i = 1; i < Strtocheck.Length; i++)
            {
                chr = Strtocheck[i];

            }
            if ((editSerialNumber.Text).Substring(editSerialNumber.Text.Length - 5, 7) != (Strtocheck.Substring(3, 7)))
            {
                MessageBox.Show((editSerialNumber.Text).Substring(editSerialNumber.Text.Length - 5, 7) + "serial_number not same" + (Strtocheck.Substring(3, 7)));
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = (editSerialNumber.Text).Substring(editSerialNumber.Text.Length - 5, 7) + " serial_number khong giong " + (Strtocheck.Substring(3, 7));
                _er.MessageEnglish = (editSerialNumber.Text).Substring(editSerialNumber.Text.Length - 5, 7) + " serial_number not same " + (Strtocheck.Substring(3, 7));
                _er.ShowDialog();
            }
            Str1 = Strtocheck.Substring(0, 10);
            if (modulo43(Str1) != "N/A")
            {
                Str1 = modulo43(Str1);
            }
            else
            {
                return true;
            }
            if (Strtocheck == Str1)
            {
                return false;
            }
            return true;
        }
        public string modulo43(string TMPSTR)
        {
            string RETSTR, str;
            int STRLENGTH;
            char chl;
            RETSTR = "N/A";
            STRLENGTH = TMPSTR.Length;
            for (int i = 1; i < STRLENGTH; i++)
            {
                str = TMPSTR.Substring(STRLENGTH - i, 1);
                chl = str[1];
            }
            //RETSTR = TMPSTR + str;
            return RETSTR;
        }
        public bool NetgearRule(string Strtocheck)
        {
            int ChkPos, subtot, Sub1;
            string Str1, tmpStr, ChkDig;
            char char1;
            if (Strtocheck.Length != 13)
            {
                return false;
            }
            for (int i = 1; i < Strtocheck.Length; i++)
            {
                char1 = Strtocheck[i];
            }
            Str1 = Strtocheck.Substring(0, 7) + Strtocheck.Substring(8, 5);
            tmpStr = "123456789ABCDEFGHJKLMNPRSTUVWXY0";
            ChkPos = 0;
            subtot = 0;
            for (int j = 0; j < Str1.Length; j++)
            {
                if (Str1[j].ToString() != "0")
                {
                    Sub1 = (tmpStr.IndexOf(Str1[j].ToString()) + 1) * ((Str1.Length) - j);
                    subtot = subtot + Sub1;
                }
            }
            ChkPos = (subtot % 32);
            if (ChkPos == 0)
            {
                ChkPos = 32;
            }
            ChkDig = tmpStr[ChkPos - 1].ToString();
            if (Strtocheck.Substring(7, 1) == ChkDig)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> U54Rule(string Strtocheck, string SSN_IDX)
        {
            int strlenght;
            string str;
            char arr;

            strlenght = Strtocheck.Length;
            str = Strtocheck.Substring(strlenght - 1, 1);
            arr = str[1];

            if (SSN_IDX == "MAC")
            {
                string sSql = "SELECT * FROM SFISM4.R_RW_MAC_SSN_T"
                + " WHERE MAC =  '" + Strtocheck + "'"
                + " AND TYPE = '" + Edt_modelName.Text + "'"
                + " AND LENGTH('" + Strtocheck + "')=length(MAC)";
                var query_data = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSql,
                    SfcCommandType = SfcCommandType.Text
                });

                if (query_data.Data.Count() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (SSN_IDX == "SSN")
            {
                string sSql_str = "SELECT * FROM SFISM4.R_RW_MAC_SSN_T"
               + " WHERE MAC =  '" + Strtocheck + "'"
               + " AND TYPE = '" + Edt_modelName.Text + "'"
               + " AND LENGTH('" + Strtocheck + "')=length(SSN)";
                var query_data_str = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSql_str,
                    SfcCommandType = SfcCommandType.Text
                });

                if (query_data_str.Data.Count() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<string> ChkDUPMAC(string _MAC)
        {
            string strGetParameter = "SELECT count(*) a FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME='CHECKMACDUP' AND VR_VALUE='CHECK'";
            var qry_GetParameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetParameter,
                SfcCommandType = SfcCommandType.Text
            });

            if (Int32.Parse(qry_GetParameter.Data["a"].ToString()) > 0)
            {
                string strGetMACID = $"SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE MACID='{_MAC}'";
                var qry_MACID = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetMACID,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_MACID.Data.Count() > 0)
                {
                    lbError.Text = await GetPubMessageVN("80144");
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "80144 - " + await GetPubMessage("80144");
                    _er.MessageEnglish = "80144 - " + await GetPubMessageVN("80144");
                    _er.ShowDialog();
                    return "EROR";
                }
                else
                {
                    return "OK";
                }
            }
            else
            {
                return "OK";
            }
        }
        public async Task<string> ChkDupSSN(string SSN)
        {
            string strGetCount = "SELECT count(*) a FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME='CHECKMACDUP' AND  VR_VALUE='CHECK'";
            var qry_GetCount = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCount,
                SfcCommandType = SfcCommandType.Text
            });
            if (Int32.Parse(qry_GetCount.Data["a"].ToString()) > 0)
            {
                string strGetData = $"SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SHIPPING_SN='{SSN}'";
                var qry_GetData = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_GetData.Data.Count() > 0)
                {
                    lbError.Text = await GetPubMessageVN("80144");
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = await GetPubMessage("80144");
                    _er.MessageEnglish = await GetPubMessageVN("80144");
                    _er.ShowDialog();
                    return "EROR";
                }
                else
                {
                    return "OK";
                }
            }
            else
            {
                return "OK";
            }
        }

        private async Task<bool> ChkCust(string Strtocheck, string SN)
        {
            string strGetCustSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{SN}' AND MAC3 = '{Strtocheck}'";
            var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustSN,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_CustSN.Data.Count() > 0)
            {
                MACID = qry_CustSN.Data["mac3"].ToString();
                return true;
            }
            else
            {
                string strGetCustMAC = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER='{SN}' AND MAC4 = '{Strtocheck}'";
                var qry_CustMAC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustMAC,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_CustMAC.Data.Count() > 0)
                {
                    MACID = qry_CustMAC.Data["mac4"].ToString();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool ODDMAC(string Strtocheck)
        {
            int STRLENGTH, retint, tmpj;
            char chl;
            string str;

            retint = 0;
            STRLENGTH = Strtocheck.Length;
            str = Strtocheck.Substring(STRLENGTH, 1);
            chl = str[1];
            tmpj = (int)char.GetNumericValue(chl);
            retint = tmpj;
            retint = tmpj % 2;
            if (retint == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void item_item_labelcombine1_Click(object sender, RoutedEventArgs e)
        {
            item_labelcombine1.IsChecked = true;
            item_labelcombine2.IsChecked = false;
            item_labelcombine3.IsChecked = false;
        }

        private void item_item_labelcombine2_Click(object sender, RoutedEventArgs e)
        {
            item_labelcombine1.IsChecked = false;
            item_labelcombine2.IsChecked = true;
            item_labelcombine3.IsChecked = false;
        }

        private void item_item_labelcombine3_Click(object sender, RoutedEventArgs e)
        {
            item_labelcombine1.IsChecked = false;
            item_labelcombine2.IsChecked = false;
            item_labelcombine3.IsChecked = true;
        }

        public bool IBMModulo10(string Strtocheck)
        {
            double Sub1, SUBTOT;
            string Str1, Str2, ChkDig;
            Str1 = Strtocheck.Substring(1, (Strtocheck.Length) - 1);
            Str2 = "";
            Sub1 = 0;
            SUBTOT = 0;

            for (int k = 0; k < (Str1.Length) - 1; k++)
            {
                if (k % 2 == 0)
                {
                    Str2 = "2" + Str2;
                }
                else
                {
                    Str2 = "1" + Str2;
                }
            }

            for (int i = 1; i < Str1.Length; i++)
            {
                if (Str1[i] != '0')
                {
                    Sub1 = (Int32.Parse(Str1[i].ToString())) * (Int32.Parse(Str2[i].ToString()));
                    SUBTOT = SUBTOT + Math.Round(Sub1 / 10) + (Sub1 % 10);
                }
            }
            if ((SUBTOT % 10) == 0)
            {
                ChkDig = "0";
            }
            else
            {
                ChkDig = ((10 - (SUBTOT % 10)).ToString()).Trim();
            }

            if (Strtocheck.Substring(Strtocheck.Length - 1, 1) == ChkDig)
            {
                return false;
            }

            return true;
        }
        public bool ControlKey(string Strtocheck)
        {
            string Str1, Str2, ChkDig;

            Str1 = Strtocheck.Substring(Strtocheck.Length - 10, 10);
            Str2 = "";

            double number = Int32.Parse(Str1.Substring(0, 10));
            Str2 = ((((number % 100) + Math.Round(number / 100) % 23) % 100).ToString()).Trim();

            if ((Str2.Trim()).Length == 1)
            {
                ChkDig = "0" + Str2;
            }
            else
            {
                ChkDig = Str2;
            }

            if (Strtocheck.Substring(Strtocheck.Length, 2) == ChkDig)
            {
                return false;
            }

            return true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        public async Task<string> Chk_OTHER_USE_ASSN(string SN, string USEKEY)
        {
            int i;

            if (USEKEY == "SSN")
            {
                i = sOTHER_USE_ASSN[1].indexid;
                FindFlag();
                RESULT = await ChkSSN(SN, "SSN" + i.ToString(), sLOAD_SSN[i].SSN_OK);
            }

            if (USEKEY == "MAC")
            {
                i = sOTHER_USE_ASSN[1].indexid;
                FindFlag();
                RESULT = await ChkMAC(SN, "MAC" + i.ToString(), sLOAD_MAC[i].MAC_OK);
            }

            if (RESULT == "")
            {
                SCAN_POS = SCAN_POS - 1;
                RESULT = "OK";
            }

            return "OK";
        }
        public async Task<bool> Netgear_MoreMac_NoCheck_mac(string SN)
        {
            string strGetDataNETGEAR = "select model_type From sfis1.c_model_desc_t2 a,sfism4.r_wip_tracking_t b"
                + $" where a.model_name =b.model_name and b.serial_number='{SN}'"
                + " and model_serial='NETGEAR'";
            var qry_DataNETGEAR = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataNETGEAR,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_DataNETGEAR.Data.Count() == 0)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> checksnchar()
        {
            string STR2, tmp_str, p_str1, p_str2, STR3, STR4, STR5, STR9, STR6;
            int i_pos, j1, j2, j3, i, i1;
            j1 = 0;
            j3 = 0;
            p_str1 = ":";
            p_str2 = ",";
            tmp_str = "N/A";
            string strGetMO = $"SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER ='{Edt_moNumber.Text.Trim()}'";
            var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetMO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_MO.Data != null)
            {
                Wo_type = qry_MO.Data["mo_type"].ToString();
            }

            string strGetShippingCode = $"SELECT SHIPPING_CODE FROM SFIS1.C_CUSTSN_RULE_T  WHERE MODEL_NAME IN (SELECT MODEL_NAME FROM SFISM4.R107 WHERE SERIAL_NUMBER='{_Inputdata}')"
                 + $" AND CUSTSN_CODE ='{SSNSTRING}' AND MO_TYPE='{Wo_type}' AND SHIPPING_CODE IS NOT NULL ORDER BY CUSTSN_CODE";
            var qry_ShippingCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetShippingCode,
                SfcCommandType = SfcCommandType.Text
            });

            if (qry_ShippingCode.Data != null)
            {
                STR2 = qry_ShippingCode.Data["shipping_code"].ToString();
                while (STR2.IndexOf(";") > -1)
                {
                    i_pos = STR2.IndexOf(";");
                    j2 = STR2.IndexOf(",");
                    for (i = 1; i <= i_pos; i++)
                    {
                        tmp_str = STR2.Substring(i - 1, 1);
                        if (tmp_str == p_str1)
                        {
                            j1 = i;
                        }
                        if (tmp_str == p_str2)
                        {
                            j3 = i;
                        }
                    }
                    STR9 = _Inputdata;
                    STR3 = STR2.Substring(j1, j2 - j1 - 1);
                    STR4 = STR2.Substring(j2, j3 - j2 - 1);
                    STR5 = STR2.Substring(j3, i_pos - j3 - 1);
                    for (i1 = Int32.Parse(STR3); i1 <= Int32.Parse(STR4); i1++)
                    {
                        if (STR5.IndexOf(STR9.Substring(i1 - 1, 1)) > -1)
                        {
                        }
                        else
                        {
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "SN sai voi thiet lap!Goi Call IE xac nhan lai";
                            _er.MessageEnglish = "SN Character Discrepancy IE Defined Rule¡MPlease Call IE Confirm";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    STR6 = STR2.Substring(STR2.IndexOf(";"), STR2.Length);
                    STR2 = STR6;
                }
            }
            return true;
        }
        public async Task<string> CheckMAC_IDTYPE(string CUST_SN, string mo_number)
        {
            string id_type, mac_begin, check_result;
            int int_id_type;
            int_id_type = 0;

            string strGetRange = "SELECT * FROM SFISM4.R_MO_EXT4_T"
                + $" WHERE '{CUST_SN}' <= item_2"
                + $" AND '{CUST_SN}' >=item_1"
                + $" AND MO_NUMBER = '{mo_number}'"
                + $" AND LENGTH('{CUST_SN}')=length(item_1)"
                + $" AND LENGTH('{CUST_SN}')=length(item_2)";
            var qry_Range = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRange,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Range.Data != null)
            {
                try
                {
                    id_type = qry_Range.Data["item_4"].ToString();
                    if (id_type == "")
                    {
                        return null;
                    }
                    else
                    {
                        mac_begin = qry_Range.Data["item_1"].ToString();
                        if (id_type == "Continuous")
                        {
                            int_id_type = 1;
                        }
                        else if (id_type == "Odd")
                        {
                            int_id_type = 3;
                        }
                        else if (id_type == "Even")
                        {
                            int_id_type = 2;
                        }
                        else
                        {
                            try
                            {
                                int_id_type = Int32.Parse(id_type.Substring(1, 2).Trim());
                            }
                            catch
                            {

                            }
                        }

                        if (int_id_type > 1)
                        {
                            check_result = Check_macdate(int_id_type, mac_begin, CUST_SN);
                            if (check_result != "OK")
                            {
                                return check_result;
                            }
                        }
                    }
                }
                catch
                {
                    return await GetPubMessage("00458");
                }
            }
            return "";
        }
        public async Task<string> checku46daa(string SN)
        {
            string mtmpsn;

            try
            {
                mtmpsn = "";
                string strGetKeyPart = $"SELECT * FROM SFISM4.R108 WHERE serial_number='{SN}' and group_name in ('ASSY','ASSY_3')";
                var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetKeyPart,
                    SfcCommandType = SfcCommandType.Text
                });

                if (qry_KeyPart.Data.Count() == 0)
                {
                    return await GetPubMessage("00301");
                }

                foreach (var row in qry_KeyPart.Data)
                {
                    mtmpsn = row["key_part_sn"].ToString();
                    string strGetShippingSN = $"select shipping_sn,nvl(po_no,'--') po_no from sfism4.r107 where serial_number='{mtmpsn}'";
                    var qry_ShippingSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetShippingSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_ShippingSN.Data.Count() == 0)
                    {
                        string strGetSerialNumber = $"select shipping_sn,nvl(po_no,'--') po_no from sfism4.h107 where serial_number='{mtmpsn}'";
                        var qry_SerialNumber = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetSerialNumber,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_SerialNumber.Data.Count() == 0)
                        {
                            try
                            {
                                string strGetSerialNumberDB = $"select shipping_sn,nvl(po_no,'--') po_no from sfism4.r107@SFCODBH where serial_number='{mtmpsn}' and rownum=1";
                                var qry_SerialNumberDB = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = strGetSerialNumberDB,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_SerialNumberDB.Data.Count() == 0)
                                {
                                    string strGetSerialNumberDB2 = $"select shipping_sn,nvl(po_no,'--') po_no from sfism4.r107@SFCODBH where serial_number='{mtmpsn}' and rownum=1";
                                    var qry_SerialNumberDB2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                    {
                                        CommandText = strGetSerialNumberDB2,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_SerialNumberDB2.Data["shipping_sn"].ToString().Length < 10)
                                    {
                                        return mtmpsn + " " + await GetPubMessage("00478");
                                    }

                                    if (qry_SerialNumberDB2.Data["po_no"].ToString().Length < 14)
                                    {
                                        return mtmpsn + " " + await GetPubMessage("00478");
                                    }
                                }
                            }
                            catch
                            {
                                return mtmpsn + " " + await GetPubMessage("00478");
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return "OK";
        }
        public async Task<bool> Chk107flag(string CHECKFLAG, string SN)
        {
            if (CHECKFLAG == "TRACKNO")
            {
                string strGetSerialNumber = $"SELECT * FROM SFISm4.r107 WHERE serial_number='{SN}' and track_no<>'N/A'";
                var qry_SerialNumber = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetSerialNumber,
                    SfcCommandType = SfcCommandType.Text
                });

                if (qry_SerialNumber.Data.Count() == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> Checkcustsndata(string SN)
        {
            string mssn;
            int i, j;

            for (i = 1; i <= 5; i++)
            {
                sSSNR_CUSTDATE[i].THE_LENGTH = 0;
                sMACR_CUSTDATE[i].THE_LENGTH = 0;

            }

            string strGetConfig43 = $"SELECT * FROM SFIS1.C_PACK_SEQUENCE_T WHERE MODEL_NAME='{Edt_modelName.Text}' AND VERSION_CODE='{Edt_versionCode.Text}' AND MO_TYPE='{sMO_TYPE}'  order by SCAN_SEQ";
            var qry_Config43 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetConfig43,
                SfcCommandType = SfcCommandType.Text
            });
            foreach (var row in qry_Config43.Data)
            {
                if (row["model_name"].ToString() == "")
                {
                    return false;
                }
                mssn = row["custsn_name"].ToString();
                if (mssn.IndexOf("SSN1") > 0)
                {
                    sSSNR_CUSTDATE[1].CHECKED = false;
                }
                if (mssn.IndexOf("SSN2") > 0)
                {
                    sSSNR_CUSTDATE[2].CHECKED = false;
                }
                if (mssn.IndexOf("SSN3") > 0)
                {
                    sSSNR_CUSTDATE[3].CHECKED = false;
                }
                if (mssn.IndexOf("SSN4") > 0)
                {
                    sSSNR_CUSTDATE[4].CHECKED = false;
                }
                if (mssn.IndexOf("SSN5") > 0)
                {
                    sSSNR_CUSTDATE[5].CHECKED = false;
                }
                if (mssn.IndexOf("SSN6") > 0)
                {
                    sSSNR_CUSTDATE[6].CHECKED = false;
                }
                if (mssn.IndexOf("SSN7") > 0)
                {
                    sSSNR_CUSTDATE[7].CHECKED = false;
                }
                if (mssn.IndexOf("SSN8") > 0)
                {
                    sSSNR_CUSTDATE[8].CHECKED = false;
                }
                if (mssn.IndexOf("SSN9") > 0)
                {
                    sSSNR_CUSTDATE[9].CHECKED = false;
                }
                if (mssn.IndexOf("SSN10") > 0)
                {
                    sSSNR_CUSTDATE[10].CHECKED = false;
                }
                if (mssn.IndexOf("SSN11") > 0)
                {
                    sSSNR_CUSTDATE[11].CHECKED = false;
                }
                if (mssn.IndexOf("SSN12") > 0)
                {
                    sSSNR_CUSTDATE[12].CHECKED = false;
                }
                if (mssn.IndexOf("MAC1") > 0)
                {
                    sSSNR_CUSTDATE[1].CHECKED = false;
                }
                if (mssn.IndexOf("MAC2") > 0)
                {
                    sSSNR_CUSTDATE[2].CHECKED = false;
                }
                if (mssn.IndexOf("MAC3") > 0)
                {
                    sSSNR_CUSTDATE[3].CHECKED = false;
                }
                if (mssn.IndexOf("MAC4") > 0)
                {
                    sSSNR_CUSTDATE[4].CHECKED = false;
                }
                if (mssn.IndexOf("MAC5") > 0)
                {
                    sSSNR_CUSTDATE[5].CHECKED = false;
                }
                if (mssn.IndexOf("MAC6") > 0)
                {
                    sSSNR_CUSTDATE[6].CHECKED = false;
                }
                if (mssn.IndexOf("MAC7") > 0)
                {
                    sSSNR_CUSTDATE[7].CHECKED = false;
                }
                if (mssn.IndexOf("MAC8") > 0)
                {
                    sSSNR_CUSTDATE[8].CHECKED = false;
                }
                if (mssn.IndexOf("MAC9") > 0)
                {
                    sSSNR_CUSTDATE[9].CHECKED = false;
                }
                if (mssn.IndexOf("MAC10") > 0)
                {
                    sSSNR_CUSTDATE[10].CHECKED = false;
                }
                if (mssn.IndexOf("MAC11") > 0)
                {
                    sSSNR_CUSTDATE[11].CHECKED = false;
                }
                if (mssn.IndexOf("MAC12") > 0)
                {
                    sSSNR_CUSTDATE[12].CHECKED = false;
                }
            }

            string strGetMO = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE MO_NUMBER = (SELECT MO_NUMBER FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{_Inputdata}') AND ROWNUM<10";
            var qry_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetMO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_MO.Data.Count() > 0)
            {
                string strGetData = "select max(ssn1) lssn1,max(ssn2) lssn2,max(ssn3) lssn3,max(ssn4) lssn4,max(ssn5) lssn5,"
                    + " max(mac1) lmac1,max(mac2) lmac2,max(mac3) lmac3,max(mac4) lmac4,max(mac5) lmac5 from ("
                    + " SELECT length(ssn1) ssn1, length(ssn2) ssn2 ,length(ssn3) ssn3,length(ssn4) ssn4, length(ssn5) ssn5,"
                    + " length(mac1) mac1,length(mac2) mac2,length(mac3) mac3,length(mac4) mac4 ,length(mac5) mac5 FROM SFISM4.R_CUSTSN_T"
                    + " WHERE MO_NUMBER = (SELECT MO_NUMBER FROM SFISM4.R107 WHERE SERIAL_NUMBER = :SN )"
                    + " AND SERIAL_NUMBER IN (  SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T  WHERE  MO_NUMBER= (SELECT MO_NUMBER FROM SFISM4.R107 WHERE SERIAL_NUMBER = :SN ) AND WIP_GROUP IN ("
                    + " SELECT C.GROUP_NAME FROM  SFISM4.R107  A ,SFIS1.C_ROUTE_CONTROL_T B ,SFIS1.C_ROUTE_CONTROL_T C"
                    + " WHERE  A.SERIAL_NUMBER=:SN AND A.SPECIAL_ROUTE=B.ROUTE_CODE AND B.GROUP_NEXT='PACK_BOX'"
                    + " AND C.ROUTE_CODE=B.ROUTE_CODE  AND  C.STEP_SEQUENCE<=B.STEP_SEQUENCE"
                    + " )  AND ROWNUM<10) and GROUP_NAME<>'PACK_BOX')";
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SN",Value=_Inputdata}
                    }
                });

                sSSNR_CUSTDATE[1].THE_LENGTH = result.Data["lssn1"] == null ? 0 : Int32.Parse(result.Data["lssn1"].ToString());
                sSSNR_CUSTDATE[2].THE_LENGTH = result.Data["lssn2"] == null ? 0 : Int32.Parse(result.Data["lssn2"].ToString());
                sSSNR_CUSTDATE[3].THE_LENGTH = result.Data["lssn3"] == null ? 0 : Int32.Parse(result.Data["lssn3"].ToString());
                sSSNR_CUSTDATE[4].THE_LENGTH = result.Data["lssn4"] == null ? 0 : Int32.Parse(result.Data["lssn4"].ToString());
                sSSNR_CUSTDATE[5].THE_LENGTH = result.Data["lssn5"] == null ? 0 : Int32.Parse(result.Data["lssn5"].ToString());

                sMACR_CUSTDATE[1].THE_LENGTH = result.Data["lmac1"] == null ? 0 : Int32.Parse(result.Data["lmac1"].ToString());
                sMACR_CUSTDATE[2].THE_LENGTH = result.Data["lmac2"] == null ? 0 : Int32.Parse(result.Data["lmac2"].ToString());
                sMACR_CUSTDATE[3].THE_LENGTH = result.Data["lmac3"] == null ? 0 : Int32.Parse(result.Data["lmac3"].ToString());
                sMACR_CUSTDATE[4].THE_LENGTH = result.Data["lmac4"] == null ? 0 : Int32.Parse(result.Data["lmac4"].ToString());
                sMACR_CUSTDATE[5].THE_LENGTH = result.Data["lmac5"] == null ? 0 : Int32.Parse(result.Data["lmac5"].ToString());

                j = 0;
                for (i = 1; i <= 5; i++)
                {
                    if (sSSNR_CUSTDATE[i].THE_LENGTH == 0)
                    {
                        j = j + 1;
                    }
                    if (sMACR_CUSTDATE[i].THE_LENGTH == 0)
                    {
                        j = j + 1;
                    }
                }
                if (j == 10)
                {
                    return true;
                }

                string strGetCustSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '{_Inputdata}'";
                var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustSN,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SN",Value=_Inputdata}
                    }
                });
                if (qry_CustSN.Data == null)
                {
                    lbError.Text = await GetPubMessageVN("00311");
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = _Inputdata + " " + await GetPubMessage("00311");
                    _er.MessageEnglish = _Inputdata + " " + await GetPubMessageVN("00311");
                    _er.ShowDialog();
                    return false;
                }
                else
                {
                    if ((sSSNR_CUSTDATE[1].THE_LENGTH > 0) && sSSNR_CUSTDATE[1].CHECKED == true)
                    {
                        if (M_modeltype.IndexOf("027") != -1)
                        {
                            if ((qry_CustSN.Data["ssn1"].ToString()).Length == 0)
                            {
                                lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - SSN1";
                                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN1";
                                _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN1";
                                _er.ShowDialog();
                                return false;
                            }
                        }
                    }
                    if ((sSSNR_CUSTDATE[2].THE_LENGTH > 0) && sSSNR_CUSTDATE[2].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["ssn2"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - SSN2";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN2";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN2";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    if ((sSSNR_CUSTDATE[4].THE_LENGTH > 0) && sSSNR_CUSTDATE[4].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["ssn4"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - SSN4";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN4";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN4";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    if ((sSSNR_CUSTDATE[5].THE_LENGTH > 0) && sSSNR_CUSTDATE[5].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["ssn5"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - SSN5";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN5";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - SSN5";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    if ((sMACR_CUSTDATE[1].THE_LENGTH > 0) && sMACR_CUSTDATE[1].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["mac1"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - MAC1";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC1";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC1";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    if ((sMACR_CUSTDATE[2].THE_LENGTH > 0) && sMACR_CUSTDATE[2].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["mac2"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - MAC2";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC2";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC2";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    if ((sMACR_CUSTDATE[3].THE_LENGTH > 0) && sMACR_CUSTDATE[3].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["mac3"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - MAC3";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC3";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC3";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    if ((sMACR_CUSTDATE[4].THE_LENGTH > 0) && sMACR_CUSTDATE[4].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["mac4"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - MAC4";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC4";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC4";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    if ((sMACR_CUSTDATE[5].THE_LENGTH > 0) && sMACR_CUSTDATE[5].CHECKED == true)
                    {
                        if ((qry_CustSN.Data["mac5"].ToString()).Length == 0)
                        {
                            lbError.Text = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "' Loss Data - MAC5";
                            ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC5";
                            _er.MessageEnglish = "ER01,SFISM4.R_CUSTSN_T SERIALNUMBER:'" + _Inputdata + "'  Loss Data - MAC5";
                            _er.ShowDialog();
                            return false;
                        }
                    }
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        private async Task<int> FindMAC(string SN, int macfound)
        {
            int j;
            string[] mac = new string[35];
            string C_MO_NUMBER;
            string C_KPSN, C_KPT_NO, C_FINDMAC;
            bool mac_exit;

            j = 0;
            if ((cb_rework.IsChecked == true) && (cb_custsn.IsChecked == true))
            {
                string strGetSN = $"SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER ='{SN}' AND ROWNUM=1";
                var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetSN,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_SN.Data.Count() > 0)
                {
                    C_FINDMAC = qry_SN.Data["mac1"].ToString();
                    macfound = macfound + 1;
                    SMAC[macfound].MAC = C_FINDMAC;
                    C_MO_NUMBER = qry_SN.Data["mo_number"].ToString();

                    string strGetMO = $"SELECT MODEL_NAME FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER ='{C_MO_NUMBER}'";
                    var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetMO,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_MO.Data == null)
                    {
                        string strGetModel = $"SELECT MODEL_NAME FROM SFISM4.H_MO_BASE_T WHERE MO_NUMBER ='{C_MO_NUMBER}'";
                        var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetModel,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    else
                    {
                        SMAC[macfound].MODEL_NAME = qry_MO.Data["model_name"].ToString();
                    }
                }
                else
                {
                    macfound = 0;
                }
            }
            else
            {
                string strGetKPSN = "SELECT KEY_PART_SN,KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
                + $" WHERE SERIAL_NUMBER = '{SN}' ORDER BY KP_RELATION ";
                var qry_KPSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetKPSN,
                    SfcCommandType = SfcCommandType.Text
                });

                List<R108> data_r108 = new List<R108>();
                data_r108 = qry_KPSN.Data.ToListObject<R108>().ToList();
                if (qry_KPSN.Data.Count() > 0)
                {
                    for (j = 0; j < qry_KPSN.Data.Count(); j++)
                    {
                        mac[j] = data_r108[j].KEY_PART_SN.ToString();
                    }
                }
                while (j != 0)
                {
                    j = j - 1;
                    C_KPSN = mac[j];

                    string strGetKeyPartSN = "SELECT KEY_PART_SN,KEY_PART_NO,mo_number  FROM SFISM4.R_WIP_KEYPARTS_T"
                     + " WHERE SERIAL_NUMBER ='" + SN + "' AND KEY_PART_SN = '" + C_KPSN + "' ";
                    var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPartSN,
                        SfcCommandType = SfcCommandType.Text
                    });

                    C_KPT_NO = qry_KeyPartSN.Data["key_part_no"]?.ToString() ?? "";

                    if (C_KPT_NO == "MACID")
                    {
                        C_FINDMAC = qry_KeyPartSN.Data["key_part_sn"]?.ToString() ?? "";
                        macfound = macfound + 1;
                        SMAC[macfound].MAC = C_FINDMAC;

                        C_MO_NUMBER = qry_KeyPartSN.Data["mo_number"]?.ToString() ?? "";

                        string strGetModel = $"SELECT MODEL_NAME FROM SFISM4.R_MO_BASE_T WHERE  MO_NUMBER ='{C_MO_NUMBER}'";
                        var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetModel,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_MO.Data == null)
                        {
                            string strGetHModel = $"SELECT MODEL_NAME FROM SFISM4.H_MO_BASE_T WHERE MO_NUMBER ='{C_MO_NUMBER}'";
                            var qry_HMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetHModel,
                                SfcCommandType = SfcCommandType.Text
                            });
                            SMAC[macfound].MODEL_NAME = qry_HMO.Data["model_name"].ToString();
                        }
                        else
                        {
                            SMAC[macfound].MODEL_NAME = qry_MO.Data["model_name"].ToString();
                        }
                    }
                    else
                    {
                        C_FINDMAC = qry_KeyPartSN.Data["key_part_sn"]?.ToString() ?? "";
                        if (SN == C_KPSN)
                        {
                            C_KPSN = "";
                        }
                        string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
                        var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strChkROKU,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_ChkROKU.Data.Count() > 0)
                        {
                            string strGetSN = $"SELECT * FROM SFISM4.R_custsn_T WHERE SERIAL_NUMBER = '{C_KPSN}'";
                            var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_SN.Data != null)
                            {
                                string sMAC1 = qry_SN.Data["mac1"]?.ToString() ?? "";
                                string sMAC2 = qry_SN.Data["mac2"]?.ToString() ?? "";
                                string sMAC3 = qry_SN.Data["mac3"]?.ToString() ?? "";
                                if (!string.IsNullOrEmpty(sMAC1))
                                {
                                    mac_exit = false;
                                    for (int i = 0; i < macfound - 1; i++)
                                    {
                                        if (mac[i] == sMAC1)
                                        {
                                            mac_exit = true;
                                        }
                                    }
                                    if (mac_exit == false)
                                    {
                                        macfound = macfound + 1;
                                        SMAC[macfound].MAC = sMAC3;
                                    }
                                }
                                if (!string.IsNullOrEmpty(sMAC2))
                                {
                                    mac_exit = false;
                                    for (int i = 0; i < macfound - 1; i++)
                                    {
                                        if (mac[i] == sMAC2)
                                        {
                                            mac_exit = true;
                                        }
                                    }
                                    if (mac_exit == false)
                                    {
                                        macfound = macfound + 1;
                                        SMAC[macfound].MAC = sMAC3;
                                    }
                                }
                                if (!string.IsNullOrEmpty(sMAC3))
                                {
                                    mac_exit = false;
                                    for (int i = 0; i < macfound; i++)
                                    {
                                        if (mac[i] == sMAC3)
                                        {
                                            mac_exit = true;
                                        }
                                    }
                                    if (mac_exit == false)
                                    {
                                        macfound = macfound + 1;
                                        SMAC[macfound].MAC = sMAC3;
                                    }
                                }
                            }
                        }
                        string strGetWIPKPSN = $"SELECT KEY_PART_SN,KEY_PART_NO,mo_number  FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER ='{C_KPSN}'";
                        var qry_WIPKPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetWIPKPSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_WIPKPSN.Data != null)
                        {
                            if (qry_WIPKPSN.Data["key_part_sn"].ToString() == SN)
                            {
                                C_KPSN = "";
                            }
                            if (C_KPSN != "")
                            {
                                await FindMAC(C_KPSN, macfound);
                            }
                        }
                        else
                        {
                            if (C_KPSN != "")
                            {
                                await FindMAC(C_KPSN, macfound);
                            }
                        }
                    }
                }
            }
            return macfound;
        }
        public async Task<int> FindHMAC(string SN, int macfound)
        {
            string C_KPSN, C_KPT_NO, C_FINDMAC, C_mo_number;
            string[] mac = new string[20];
            int j;

            j = 1;

            string strGetWIPKP = $"SELECT KEY_PART_SN FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER = '{SN}' ORDER BY KP_RELATION";
            string strGetHWIPKP = $"SELECT KEY_PART_SN FROM SFISM4.H_WIP_KEYPARTS_T WHERE SERIAL_NUMBER = '{SN}' ORDER BY KP_RELATION";
            var qry_WIPKP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetWIPKP,
                SfcCommandType = SfcCommandType.Text
            });

            if (qry_WIPKP.Data == null)
            {
                var qry_HWIPKP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetHWIPKP,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_HWIPKP.Data == null)
                {
                }
                else
                {
                    mac[j] = qry_HWIPKP.Data["key_part_sn"]?.ToString() ?? "";
                    j = j + 1;
                    while (j != 0)
                    {
                        j = j - 1;
                        C_KPSN = mac[j];
                        string strGetHKPSN = $"SELECT * FROM SFISM4.H_WIP_KEYPARTS_T WHERE SERIAL_NUMBER ='{SN}' AND KEY_PART_SN = '{C_KPSN}'";
                        var qry_HKPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetHKPSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_HKPSN.Data != null)
                        {
                            C_KPT_NO = qry_HKPSN.Data["key_part_no"]?.ToString() ?? "";
                            if (C_KPT_NO == "MACID")
                            {
                                C_FINDMAC = qry_HKPSN.Data["key_part_sn"]?.ToString() ?? "";
                                C_mo_number = qry_HKPSN.Data["mo_number"]?.ToString() ?? "";

                                string strGetMO = $"SELECT MODEL_NAME  FROM SFISM4.R_MO_BASE_T WHERE  MO_NUMBER ='{C_mo_number}'";
                                var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetMO,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                macfound = macfound + 1;
                                SMAC[macfound].MAC = C_FINDMAC;
                                SMAC[macfound].MODEL_NAME = qry_MO.Data["model_name"]?.ToString() ?? "";
                            }
                            else
                            {
                                C_KPSN = qry_HKPSN.Data["model_name"]?.ToString() ?? "";
                                if (C_KPSN != "")
                                {
                                    await FindHMAC(C_KPSN, macfound);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                mac[j] = qry_WIPKP.Data["key_part_sn"]?.ToString() ?? "";
                j = j + 1;
                while (j != 0)
                {
                    j = j - 1;
                    C_KPSN = mac[j];
                    string strGetWIPKPSN = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER ='{SN}' AND KEY_PART_SN = '{C_KPSN}'";
                    var qry_WIPKPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetWIPKPSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_WIPKPSN.Data != null)
                    {
                        C_KPT_NO = qry_WIPKPSN.Data["key_part_no"]?.ToString() ?? "";
                        if (C_KPT_NO == "MACID")
                        {
                            C_FINDMAC = qry_WIPKPSN.Data["key_part_sn"]?.ToString() ?? "";
                            C_mo_number = qry_WIPKPSN.Data["mo_number"]?.ToString() ?? "";

                            string strGetMO = $"SELECT MODEL_NAME FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER ='{C_mo_number}'";
                            var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetMO,
                                SfcCommandType = SfcCommandType.Text
                            });
                            macfound = macfound + 1;
                            SMAC[macfound].MAC = C_FINDMAC;
                            SMAC[macfound].MODEL_NAME = qry_MO.Data["model_name"]?.ToString() ?? "";
                        }
                        else
                        {
                            C_KPSN = qry_WIPKPSN.Data["key_part_sn"]?.ToString() ?? "";
                            if (C_KPSN != "")
                            {
                                await FindHMAC(C_KPSN, macfound);
                            }
                        }
                    }
                }
            }
            return macfound;
        }
        private async Task<string> CheckKeyPart(string SN, bool _Keypart)
        {
            string KPNO;
            int nKP_RELATION;
            KPNO = "";
            string strGetKeyPart = "";
            Keypart_OK = true;

            string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
            var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strChkROKU,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ChkROKU.Data.Count() > 0)
            {
                if (await CheckBoxNext(SN) == false)
                {
                    strGetKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN"
                       + $" (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{SN}') and (group_name not in('{M_sThisGroup}') or group_name is null) AND KEY_PART_NO NOT IN"
                       + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
                       + $" WHERE SERIAL_NUMBER = '{SN}')";
                }
                else
                {
                    strGetKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN"
                     + $" (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{SN}') AND KEY_PART_NO NOT IN "
                     + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T "
                     + $" WHERE SERIAL_NUMBER = '{SN}') and ( group_name not IN ('{Bnext_group}') OR GROUP_NAME IS NULL)";
                }
            }
            else
            {
                if (await CheckBoxNext(SN) == false)
                {
                    strGetKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN"
                       + $" (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{SN}') AND KEY_PART_NO NOT IN"
                       + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
                       + $" WHERE SERIAL_NUMBER = '{SN}' AND KEY_PART_NO IS NOT NULL ) AND group_name not in ('PACK_CTN')";
                }
                else
                {
                    strGetKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN"
                     + $" (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{SN}') AND KEY_PART_NO NOT IN "
                     + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T "
                     + $" WHERE SERIAL_NUMBER = '{SN}') and ( group_name not IN ('{Bnext_group}') and group_name not in ('PACK_CTN'))";
                }
            }
            var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetKeyPart,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_KeyPart.Data.Count() > 0)
            {
                foreach (var row in qry_KeyPart.Data)
                {
                    nKP_RELATION = Int32.Parse(row["kp_relation"].ToString());
                    string strGetWIPKp = $"SELECT count(*) FROM SFISM4.R_WIP_KEYPARTS_T WHERE KP_RELATION = '{nKP_RELATION.ToString()}' AND SERIAL_NUMBER = '{SN}'";
                    var qry_WIPKp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetWIPKp,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (Int32.Parse(qry_WIPKp.Data["count(*)"].ToString()) == 0)
                    {
                        Keypart_OK = false;
                        KPNO = KPNO + " " + row["key_part_no"].ToString() + " , ";
                    }
                }
            }
            else
            {
                Keypart_OK = true;
                KPNO = "";
            }
            return KPNO;
        }

        public async Task<string> ChecklinkKeyPart(string SN, bool _Keypart)
        {
            string KPNO;
            int nKP_RELATION;
            KPNO = "";
            string strGetLinkKeyPart = "";
            Keypart_OK = true;
            string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
            var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strChkROKU,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ChkROKU.Data.Count() > 0)
            {
                if (await CheckBoxNext(SN) == false)
                {
                    strGetLinkKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN"
                       + $" (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{SN}') AND KEY_PART_NO NOT IN"
                       + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T "
                       + $" WHERE SERIAL_NUMBER = '{SN}') and group_name not in ('{M_sThisGroup}') and group_name is not null";
                }
                else
                {
                    strGetLinkKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN "
                         + $" (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{SN}') AND KEY_PART_NO NOT IN "
                         + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
                         + $" WHERE SERIAL_NUMBER = '{SN}') and  group_name  is not null and  group_name not IN ('{Bnext_group}')";
                }
            }
            else
            {
                if (await CheckBoxNext(SN) == false)
                {
                    strGetLinkKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN"
                       + $" (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{SN}') AND KEY_PART_NO NOT IN"
                       + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T "
                       + $" WHERE SERIAL_NUMBER = '{SN}' and group_name is not null) and group_name not in ('PACK_CTN')";
                }
                else
                {
                    strGetLinkKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN "
                         + " (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + SN + "') AND KEY_PART_NO NOT IN "
                         + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
                         + " WHERE SERIAL_NUMBER = '" + SN + "' AND KEY_PART_NO IS NOT NULL)   and   group_name  is not null and  group_name not IN ('" + Bnext_group + "')"
                         + " and group_name not in ('PACK_CTN')";
                }
            }
            var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetLinkKeyPart,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_KeyPart.Data.Count() > 0)
            {
                foreach (var row in qry_KeyPart.Data)
                {
                    nKP_RELATION = Int32.Parse(row["kp_relation"].ToString());
                    string strGetWIPKP = "SELECT COUNT(*) FROM SFISM4.R_WIP_KEYPARTS_T"
                        + $" WHERE KP_RELATION = '{nKP_RELATION.ToString()}' AND SERIAL_NUMBER = '{SN}'";
                    var qry_WIPKP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetWIPKP,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (Int32.Parse(qry_WIPKP.Data["count(*)"].ToString()) == 0)
                    {
                        Keypart_OK = false;
                        KPNO = KPNO + " " + row["key_part_no"].ToString() + " , ";
                    }
                }
            }
            else
            {
                Keypart_OK = true;
                KPNO = "";
            }
            return KPNO;
        }
        private string Check_macdate(int id_type, string start_mac,string check_mac)
        {
            int check;
            try
            {
                check = (Int32.Parse(CHENGE16TO10(check_mac.Substring(5, 7))) - Int32.Parse(CHENGE16TO10(start_mac.Substring(5, 7)))) % id_type;
                if (check != 0)
                {
                    return "MAC ID TYPE IS " + id_type.ToString() + ", but " + check_mac + "-" + start_mac + " mod " + id_type.ToString() + " not zero";
                }
            }
            catch
            {
                return "OK";
            }
            return "OK";
        }
        private string  CHENGE16TO10(string TMPSTR)
        {
            int tmpj, STRLENGTH, retint, tmpint;
            string str;
            char ch1;

            tmpj = 0;
            STRLENGTH = TMPSTR.Length;
            str = TMPSTR.Substring(STRLENGTH - 1, 1);
            ch1 = str[1];

            retint = tmpj;
            for (int i = 0;i< STRLENGTH -1;i++)
            {
                str = TMPSTR.Substring(STRLENGTH - i - 1, 1);
                ch1 = str[1];
                tmpint = 1;
                for (int j  = 0; j < i; j++)
                {
                    tmpint = tmpint * 16;
                }
                retint = retint + tmpj * tmpint;
            }
            return retint.ToString();
        }
        public async Task<bool> CheckBoxNext(string SN)
        {
            string str, t_str;
            int w_i, c_i;

            string strGetParameter = "SELECT count(*) a FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='BOX_NEXT' AND VR_VALUE='YES'";
            var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetParameter,
                SfcCommandType = SfcCommandType.Text
            });

            if (Int32.Parse(qry_Parameter.Data["a"].ToString()) > 0)
            {
                string strGetData = "SELECT STEP_SEQUENCE tt,route_code FROM SFIS1.C_ROUTE_CONTROL_T WHERE  GROUP_NEXT='PACK_BOX' AND GROUP_NEXT NOT LIKE 'R_%'"
                    + " AND GROUP_NAME NOT LIKE 'R_%' and route_code in("
                    + " select special_route tt from sfism4.r_wip_tracking_t where serial_number='{SN}') ORDER BY STEP_SEQUENCE";
                var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text
                });

                if (qry_Data.Data == null)
                {
                    return false;
                }
                w_i = Int32.Parse(qry_Data.Data["tt"].ToString());
                c_i = Int32.Parse(qry_Data.Data["route_code"].ToString());

                string strGetGN = $"SELECT  group_next FROM SFIS1.C_ROUTE_CONTROL_T WHERE  STEP_SEQUENCE>'{w_i}'"
                    + " AND GROUP_NEXT NOT LIKE 'R_%' AND GROUP_NAME NOT LIKE 'R_%'"
                    + $" and route_code='{c_i}' ORDER BY STEP_SEQUENCE";
                var qry_GN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetGN,
                    SfcCommandType = SfcCommandType.Text
                });

                if (qry_GN.Data == null) return false;
                str = "";
                while (qry_GN.Data != null)
                {
                    t_str = qry_GN.Data["group_next"].ToString();
                    t_str = t_str + "," + "";
                    str = str + t_str;
                }
                str = str.Substring(1, str.Length - 3);
                Bnext_group = str;
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task CheckSP(string _paramSN)
        {
            string sDB, sSendTo, sErr_Msg,sSTA,sSTB, sID, sMD5;
            string sMO, sMD, sMT, sRT, sCAP, sGN, sLocation;
            int i, j;

            sErr_Msg = "";
            sSTA = "";
            sSTB = "";
            sMD5 = "";
            try
            {
                string strGetParameter = "select vr_item,vr_value,vr_desc  from sfis1.c_parameter_ini where prg_name='RECORD_SP' and vr_name='PACK_BOX'";
                var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetParameter,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Parameter.Data == null)
                {
                    return;
                }
                else
                {
                    if (qry_Parameter.Data["vr_value"].ToString() != "Y")
                    {
                        return;
                    }
                    sDB = qry_Parameter.Data["vr_item"].ToString();
                    sSendTo = qry_Parameter.Data["vr_desc"].ToString();
                }

                string strGetData = $"select a.special_route,a.mo_number,b.model_name,upper(b.mo_type) type from sfism4.r107 a,sfism4.r105 b where a.mo_number=b.mo_number and a.serial_number='{_paramSN}'";
                var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter {Name="sn",Value=_paramSN }
                }
                });
                if (qry_Data.Data == null)
                {
                    return;
                }
                else
                {
                    sMO = qry_Data.Data["mo_number"].ToString();
                    sMD = qry_Data.Data["model_name"].ToString();
                    sMT = qry_Data.Data["type"].ToString();
                    sRT = qry_Data.Data["special_route"].ToString();
                    sCAP = sDB + $"Database Mode Monitor Abnormal('{sMD}','{sMO}')";
                }
                i = 1;

                string strGetDataParam = $"select group_name,location from sfism4.r117 where serial_number='{_paramSN}' and group_name in("
                 + " select group_next from sfis1.c_route_control_t where route_code='route_code' and step_sequence<( "
                 + $" select min(step_sequence) from sfis1.c_route_control_t where group_next like 'PACK%' and route_code='{sRT}'))";
                var qry_Param = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataParam,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Param.Data == null)
                {
                    return;
                }

                while (qry_Param.Data != null)
                {
                    sGN = qry_Param.Data["group_name"].ToString();
                    sLocation = qry_Param.Data["location"].ToString();
                    if (sLocation == "")
                    {
                        lbError.Text = "No record of station type!Need to modify SP";
                        Err_Msg = "No record of station type!Need to modify SP";
                        i = i + 1;
                    }
                    string sSQL_str = "select station_type from sfis1.c_smo_type_t where model_name=:md and mo_type=:mt and group_name=:gn order by port_type";
                    var query_smo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = sSQL_str,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="md",Value=sMD},
                        new SfcParameter{Name="mt",Value=sMT},
                        new SfcParameter{Name="gn",Value=sGN}
                    }
                    });
                    if (query_smo.Data == null)
                    {
                        sErr_Msg = sErr_Msg + i.ToString() + " : " + " Station CONFIG Item 52 Undefined.";
                        i = i + 1;
                    }
                    else
                    {
                        j = 0;
                        while ((query_smo.Data != null) && (j < 2))
                        {
                            if (j == 0)
                            {
                                sSTA = query_smo.Data["station_type"].ToString();
                            }
                            if (j == 1)
                            {
                                sSTB = query_smo.Data["station_type"].ToString();
                            }
                            j = j + 1;
                        }
                        if (j == 1)
                        {
                            if ((sLocation.Trim() == (sSTA + ",0").Trim()) || (sLocation.Trim() == (sSTA + "," + sSTA).Trim()))
                            {
                                //OK
                            }
                            else
                            {
                                sErr_Msg = sErr_Msg + i.ToString() + ":" + sGN + $"Station LOCATION And CONFIG Item 52 discrepancy.-->LOCATION='{sLocation}',CONFIG='" + (sSTA + ",0").Trim();
                                i = i + 1;
                            }
                        }
                        else
                        {
                            if (sLocation.Trim() != (sSTA + "," + sSTB))
                            {
                                sErr_Msg = sErr_Msg + i.ToString() + ":" + sGN + $"Station LOCATION And CONFIG Item 52 discrepancy.-->LOCATION='{sLocation}',CONFIG='" + (sSTA + ",0").Trim();
                                i = i + 1;
                            }
                        }
                    }
                }

                if (i == 0) return;
                sID = G_sBuildDate;

                string strGetMail = $"select 1 from sfis1.c_mail_t where mail_id='{sID}' and mail_from='{sMD5}'";
                var qry_Mail = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetMail,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Mail.Data == null)
                {
                    try
                    {
                        string strInsert = "insert into sfis1.c_mail_t(mail_id,mail_to,mail_from,MAIL_SUBJECT,mail_sequence,mail_content,mail_date,mail_flag,mail_program) " +
                            " values (:id,:to,:from,:SB,''0',:content,sysdate,'0','PACK_BOX')";
                        var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strInsert,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name="id",Value=sID },
                                new SfcParameter {Name="SB",Value=sCAP +"--" + sID + " (by PACK_BOX)"},
                                new SfcParameter {Name="to",Value=sSendTo },
                                new SfcParameter {Name="from",Value=sMD5 },
                                new SfcParameter {Name="content",Value=sErr_Msg},
                            }
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task<bool> CheckRoute()
        {
            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.CHECK_ROUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="DATA", Value=_Inputdata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MYGROUP",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter{Name="LINE",Value=sLine_name,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                }
            });

            dynamic ads = result.Data;
            res = ads[0]["res"];
            if (res != "OK")
            {
                lbError.Text = "Route Error : " + res;
                return false;
            }
            else
            {
                return true;
            }
        }
        public async void FindFlag()
        {
            string strGetModel = $"SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='{Edt_modelName.Text}'";
            var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetModel,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Model.Data != null)
            {
                M_modeltype = qry_Model.Data["model_type"]?.ToString() ?? "";
                CHECKFLAG = qry_Model.Data["checkflag"]?.ToString() ?? "";
            }
            else
            {
                M_modeltype = "";
                lbError.Text = "Model have not in DB. Call IE setup config 6!!";
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Model chua duoc thiet lap. Call IE thiet lap config 6!!";
                _er.MessageEnglish = "Model have not in DB. Call IE setup config 6!!";
                _er.ShowDialog();
                return;
            }
        }
        public async Task<bool> CheckMO(string _MO_Status)
        {
            string strGetMOStatus = $"SELECT MO_TYPE,CLOSE_FLAG FROM SFISM4.R105 WHERE MO_NUMBER = (SELECT MO_NUMBER FROM SFISM4.R107 WHERE SERIAL_NUMBER = '{_Inputdata}')";
            var qry_MOStatus = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetMOStatus,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_MOStatus.Data["close_flag"].ToString() != "2")
            {
                sMO_TYPE = qry_MOStatus.Data["mo_type"].ToString();
                MO_Status = qry_MOStatus.Data["close_flag"].ToString();
                return false;
            }
            else
            {
                sMO_TYPE = qry_MOStatus.Data["mo_type"].ToString();
                return true;
            }
        }
        public async Task<string> GetPubMessage(string PROMPT_CODE)
        {
            var query_lenguage = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT PROMPT_ENGLISH FROM SFIS1.C_PROMPT_CODE_T WHERE PROMPT_CODE = :prompt",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="prompt", Value=PROMPT_CODE }
                }
            });
            if (query_lenguage.Data != null)
            {
                string result = query_lenguage.Data["prompt_english"].ToString();
                return result;
            }
            return null;
        }
        public async Task<string> GetPubMessageVN(string PROMPT_CODE)
        {
            var query_lenguage = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT PROMPT_CHINESE FROM SFIS1.C_PROMPT_CODE_T WHERE PROMPT_CODE = :prompt",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="prompt", Value=PROMPT_CODE }
                }
            });
            if (query_lenguage.Data != null)
            {
                string result = query_lenguage.Data["prompt_chinese"].ToString();
                return result;
            }
            return null;
        }
        private async void mainWindow_Initialized(object sender, EventArgs e)
        {
            string[] Args = Environment.GetCommandLineArgs();

            if (Args.Length == 1)
            {
                //MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                //Environment.Exit(0);
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
            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //    Environment.Exit(0);
            //}

            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "PACKING",
                UserName = empNo,
                Password = empPass
            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            sfcClient = DBApi.sfcClient(loginDB, loginApiUri);
            try
            {
                await sfcClient.GetAccessTokenAsync(empNo, empPass);

                var result1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic ads = result1.Data;
                string Ok = ads[0]["output"];

                if (Ok.Substring(0, 2) == "OK")
                {

                    loginInfor = Ok.Substring(3, Ok.Length - 3).Trim();
                    string[] digits = Regex.Split(loginInfor, @";");
                    txt_Database.Text = digits[0].ToString();
                    edtpassword.Text = digits[1].ToString();
                    Lb_User.Text += empNo +"-"+  await getUser();
                    txtVerAp.Text += getRunningVersion().ToString();
                    txtIp.Text += localIP().ToString();
                }
                else
                {
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = "Excute procedure have error: " + Ok;
                    _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + Ok;
                    _er.ShowDialog();
                    this.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "Excute procedure have error:" + ex.Message;
                _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + ex.Message;
                _er.ShowDialog();
                this.Close();
                return;
            }
            InputData.Focus();
            edtpassword.IsEnabled = false;
            G_sBuildDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            FormCreate();
        }
        public void FormCreate()
        {
            string strTemp;
            INIFile ini = new INIFile("C:\\Windows\\SFIS.ini");
            M_sThisSection = ini.Read("PrePacking", "Section");
            M_sThisGroup = ini.Read("PrePacking", "Group");
            M_sThisStation = ini.Read("PrePacking", "Station");
            M_sThisLine = ini.Read("PrePacking", "Line");
            lbTitle.Content = M_sThisGroup;
            CHECK_MO_FLAG = true;
            if (string.IsNullOrEmpty(M_sThisGroup))
            {
                FormShow();
                ini.Write("PrePacking", "CAPS_LOCK", sCAPS_LOCK.ToString());
                ini.Write("PrePacking", "CBREWORK", cb_rework.IsChecked.ToString());
                ini.Write("PrePacking", "FORTMAC", cb_witemac.IsChecked.ToString());
                ini.Write("PrePacking", "Chcustsn", cb_custsn.IsChecked.ToString());
            }
            //
            if (ini.Read("PrePacking", "CAPS_LOCK") == "0")
            {
                sCAPS_LOCK = false;
            }
            else if (ini.Read("PrePacking", "CAPS_LOCK") == "1")
            {
                sCAPS_LOCK = true;
            }
            else
            {
                strTemp = ini.Read("PrePacking", "CAPS_LOCK");
                if (strTemp != "TRUE") strTemp = "FALSE";
                sCAPS_LOCK = bool.Parse(strTemp);
            }
            CAPS_LOCK.IsChecked = sCAPS_LOCK;
            if (ini.Read("PrePacking", "CBREWORK") == "0")
            {
                cb_rework.IsChecked = false;
            }
            else if(ini.Read("PrePacking", "CBREWORK") == "1")
            {
                cb_rework.IsChecked = true;
            }
            else
            {
                strTemp = ini.Read("PrePacking", "CBREWORK");
                if (strTemp != "TRUE") strTemp = "FALSE";
                cb_rework.IsChecked = bool.Parse(strTemp);
            }
            //
            if(ini.Read("PrePacking", "FORTMAC") == "0")
            {
                cb_witemac.IsChecked = false;
            }
            else if (ini.Read("PrePacking", "FORTMAC") == "1")
            {
                cb_witemac.IsChecked = true;
            }
            else
            {
                strTemp = ini.Read("PrePacking", "FORTMAC");
                if (strTemp != "TRUE") strTemp = "FALSE";
                cb_witemac.IsChecked = bool.Parse(strTemp);
            }
            //
            if (ini.Read("PrePacking", "Chcustsn") == "0")
            {
                cb_custsn.IsChecked = false;
            }
            else if (ini.Read("PrePacking", "Chcustsn") == "1")
            {
                cb_custsn.IsChecked = true;
            }
            else
            {
                strTemp = ini.Read("PrePacking", "Chcustsn");
                if (strTemp != "TRUE") strTemp = "FALSE";
                cb_custsn.IsChecked = bool.Parse(strTemp);
            }
            //
            //if (ini.Read("PrePacking", "CHECKCUSTSN") == "0")
            //{
            //    cb_chk1.IsChecked = false;
            //}
            //else if (ini.Read("PrePacking", "CHECKCUSTSN") == "1")
            //{
            //    cb_chk1.IsChecked = true;
            //}
            //else
            //{
            //    cb_chk1.IsChecked = bool.Parse(ini.Read("PrePacking", "CHECKCUSTSN"));
            //}
            //
            if (cb_rework.IsChecked == true)
            {
                cb_rework.Visibility = Visibility.Visible;
            }
            if (cb_witemac.IsChecked == true)
            {
                cb_witemac.Visibility = Visibility.Visible;
            }
            if (cb_custsn.IsChecked == true)
            {
                cb_custsn.Visibility = Visibility.Visible;
            }
            if (CAPS_LOCK.IsChecked == true)
            {
                lbError.Text = "Caps Lock!!";
            }
            sPrgName = "PACKBOX";
            G_sBuildDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            string G_Time = DateTime.Now.ToString("yyyy/MM/dd");
            this.Title = "SFIS : " + sPrgName + " Build Date : " + G_Time + " Version: " + txtVerAp.Text;
        }
        public void FormShow()
        {
            if (string.IsNullOrEmpty(M_sThisGroup))
            {
                Station_Setup formStationSetup = new PACK_BOX.Station_Setup(this, sfcClient);
                formStationSetup.ShowDialog();
            }
            lbTitle.Content = M_sThisGroup;
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
        private async Task<string> getUser()
        {
            string EMP;
            string sql = $"select * from SFIS1.C_EMP_DESC_T WHERE EMP_PASS = '{empPass}'";
            var query_model_sql = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            EMP = query_model_sql.Data["emp_name"].ToString();
            return EMP;
        }
        private async Task<string> getFTP()
        {
            string FTP;
            string sql = "SELECT * FROM SFIS1.C_model_desc_T where model_name ='Z_BOX_LH'";
            var query_model_sql = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            FTP = query_model_sql.Data["model_serial"].ToString();
            return FTP;
        }

        public async void upload_ftp()
        {
            string ftp_server;
            //Get ftp server username password   

            if (edtpassword.Text == "")
            {
                this.Close();
            }
            else
            {
                string sql_str = "SELECT * FROM SFIS1.C_model_desc_T where model_name = 'Z_BOX_LH'  AND CHECKFLAG = 'Y'";
                var query_model = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql_str,
                    SfcCommandType = SfcCommandType.Text
                });
                if (query_model.Data.Count() == 0)
                {
                    return;
                }
                string sql = "SELECT * FROM SFIS1.C_model_desc_T where model_name ='Z_BOX_LH'";
                var query_model_sql = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });

                ftp_server = query_model_sql.Data["model_serial"].ToString();

                FileInfo fi = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"LOG_PACK_BOX\" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                if (!fi.Exists)
                {
                    using (StreamWriter sw = fi.CreateText())
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ": Write Log Open");
                    }
                }
                string path = Directory.GetCurrentDirectory() + "\\LOG_PACK_BOX\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                string FileName = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                try
                {
                    FtpWeb ftpweb = new FtpWeb(ftp_server + "/Test_PACKBOX_Log/", null, "amsupload", "uploadap168!");
                    ftpweb.Upload(path);
                }
                catch (Exception e)
                {
                    ShowMessageForm _er = new ShowMessageForm(this, sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = e.Message;
                    _er.MessageVietNam = e.Message;
                    _er.ShowDialog();
                }
            }
        }
        public void FormClose(object sender, EventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            ini.Write("PrePacking", "CBREWORK", cb_rework.IsChecked.ToString());
            //ini.Write("PrePacking", "CHECKCUSTSN", cb_chk1.IsChecked.ToString());
            ini.Write("PrePacking", "FORTMAC", cb_witemac.IsChecked.ToString());
            ini.Write("PrePacking", "Chcustsn", cb_custsn.IsChecked.ToString());
        }
        public void zSubstring(ref string input,int start,int length)
        {
            try
            {
                if (start > input.Length)
                    input = "";
                else
                {
                    if (start == 1) start = 0;
                    if (length > input.Length) 
                        length = input.Length;
                    input = input.Substring(start, length);
                }
            }
            catch
            {

            }
        }

        // Add by V1038047 2023-03-25
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
            string tmpSnstart = ini.Read("PrePacking", SN_NAME).Trim();
            string tmpsnend = ini.Read("PrePacking", SN_NAME + "TO").Trim();
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
        // end
    }
}
