using PACKINGBOXID_CFG.Model;
using PACKINGBOXID_CFG.Resource;
using Sfc.Library.HttpClient;
using System;
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
using System.Management;
using System.Data;
using System.Diagnostics;
using LabelManager2;
using PACKINGBOXID_CFG.Resources;
using System.IO.Ports;

namespace PACKINGBOXID_CFG
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
        public string empName = "";
        public string inputLogin = "";

        public DataTable dtParams = new DataTable();
        LabelManager2.Application labApp = null;
        LabelManager2.Document doc = null;
        public string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\";
        public static string strDataInput { get; set; }

        public string LabelName = "";
        public string model_NameBak = "";
        public static string FilePath = "";
        public static string UrlLabelFile = "";
        public static string Next_Step = "";
        public static string MBOXID = "";
        public int Count_ReprintLabel = 1;
        public string local_strIP, G_sBuildDate, G_Time, sPrgName;
        public string M_sThisSection, M_sThisGroup, M_sThisStation,M_sThisLineName;
        public string ap_version, PCMAC, PCIP;
        public bool CHECKSN_OK, CHECKTRAY_OK, MacPrefix_OK, FCD_OK, BomAndOSFW_OK,Keypart_OK, MACID_OK,SN_OK;
        public string sCUSTSN_CODE, My_LabelFileName, Result_MO_Status;
        public string MODEL_TYPE, Mmodel_type, M_SITE,MACID, MO_TYPE, MO_BOM_NO, sMO_TYPE, MAC1, MAC2, MAC3, CIS_MAC, CIS_SSN, BOM_NO, CheckPri;
        public string Err_Msg;
        bool FoundMAC;
        public bool IsPackingByMoElseByModel, BOM_HAS_MAC, NO_MAC, BOM_HAS_WIRELESS, BOM_HAS_WAN, BOM_HAS_LAN,comEvent;
        public int M_iTrayCount, sQty, SCAN_POS, tmpj, macfound, RETEST_NUM, ERROR_NUM;
        public string MO, MO_R102, AMPMODELTYPE, smodel_name, MACSTRING, SCAN_MAC, Wo_type, mo_ver, retstr;
        public string S_YYWW, S_YM, modelsite, res,t_version_code,t_mo_number, M_sSSN, M_sSSN1, M_sSSN2;
        public int Time,_Time;
        public static string ComData = "";

        //Array
        TSSN_RULE[] sLOAD_SSN = new TSSN_RULE[16];
        TMAC_RULE[] sLOAD_MAC = new TMAC_RULE[16];
        T_TESTSN[] sLOAD_SN = new T_TESTSN[6];
        string[] SMAC = new string[16];

        //TextBox
        TextBox editSerialNumber = new TextBox();

        //List
        List<SOURCE> sequenlist = new List<SOURCE>();
        List<CUST_RULE> CUST_SN_MODEL = new List<CUST_RULE>();
        public static SerialPort Z14 = new SerialPort();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {

            InitializeComponent();
            getComdata();
        }
        public void MainWindow_Load()
        {

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
            result = result.Replace(":", "");
            PCMAC = result;
            return PCMAC;
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
                CommandText = $"SELECT prompt_chinese FROM SFIS1.C_PROMPT_CODE_T WHERE PROMPT_CODE = '{PROMPT_CODE}'",
                SfcCommandType = SfcCommandType.Text
            });
            if (query_lenguage.Data != null)
            {
                string result = query_lenguage.Data["prompt_chinese"].ToString();
                return result;
            }
            return null;
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            InitForm frm_InitForm = new InitForm();
            frm_InitForm.ShowDialog();
        }
        private void item_Exit_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageBox.Show("00276 - Close the Program!!", "Message", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (message == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private void item_Station_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FormStationSetup formStation = new PACKINGBOXID_CFG.FormStationSetup(this, sfcClient);
                formStation.ShowDialog();
                lbTitle.Content = M_sThisLineName + ":" + M_sThisGroup;
                Edt_linename.Text = M_sThisLineName;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void item_Assy_input_Click(object sender, RoutedEventArgs e)
        {
            Input_Monumber.IsEnabled = item_Assy_input.IsChecked;
            Input_Monumber.Focus();
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
            PCIP = local_strIP;
            return local_strIP;
        }
        private  async void CloseBOXID_Click(object sender, RoutedEventArgs e)
        {
            await CloseBOXID();
        }
        private void item_Label_lh_Click(object sender, RoutedEventArgs e)
        {
            item_Label_gl.IsChecked = false;
            item_Label_local.IsChecked = false;
        }

        private void item_Label_gl_Click(object sender, RoutedEventArgs e)
        {
            item_Label_lh.IsChecked = false;
            item_Label_local.IsChecked = false;
        }

        private void item_CheckMacID_Click(object sender, RoutedEventArgs e)
        {
            //
        }
        private void item_Tori_2G_Click(object sender, RoutedEventArgs e)
        {
            if (item_Tori_2G.IsChecked == true)
            {
                item_Label_lh.IsChecked = true;
            }
            else
            {
                item_Label_local.IsChecked = true;
            }
        }
        private async void item_Controlrun_Click(object sender, RoutedEventArgs e)
        {
            if (item_Controlrun.IsChecked == true)
            {
                FormInputPassword frm_InputPass = new FormInputPassword(this,sfcClient,"");
                frm_InputPass.ShowDialog();
                if (CheckPri == "OK")
                {
                    item_Pilotrun.IsChecked = true;
                    string strInsertLog = "Insert into SFISM4.R_SYSTEM_LOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC)"
                        + " Values"
                        + " (:EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                    var Insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsertLog,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="EMP_NO",Value=frm_InputPass.EMP_NO},
                            new SfcParameter{Name="PRG_NAME",Value="PACKING"},
                            new SfcParameter{Name="ACTION_TYPE",Value="PILOT_RUN"},
                            new SfcParameter{Name="ACTION_DESC",Value="PACK_BOXID:CHECKPILOTRUN(Consigned)--->NOCHECKPILOTRUN(Consigned);IP:" + localIP().ToString()}
                        }
                    });
                }
            }
            else
            {
                //Writelog.WriteLog("SETTING:CHECK PIRUN->Consigned:FALSE;CHECK PIRUN->PILOT_RUN:TRUE");
            }
        }

        private void Cb_Rework_Click(object sender, RoutedEventArgs e)
        {
            if (Cb_Rework.IsChecked == true)
            {
                if (Inputdata.IsEnabled == true)
                {
                    Cb_Fortmac.Visibility = Visibility.Visible;
                    Cb_custsn.Visibility = Visibility.Visible;
                    Input_Edtmac.Visibility = Visibility.Visible;
                    Edtmac.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Cb_Fortmac.Visibility = Visibility.Hidden;
                Cb_custsn.Visibility = Visibility.Hidden;
                Input_Edtmac.Visibility = Visibility.Hidden;
                Edtmac.Visibility = Visibility.Hidden;
            }
        }

        private void Cb_Fortmac_Click(object sender, RoutedEventArgs e)
        {
            if (Cb_Fortmac.IsChecked == true)
            {
                Cb_custsn.IsChecked = false;
                Cb_Rework.IsChecked = true;
            }
        }
        private async void item_Pilotrun_Click(object sender, RoutedEventArgs e)
        {
            if (item_Pilotrun.IsChecked == true)
            {
                FormInputPassword frm_InputPass = new FormInputPassword(this, sfcClient,"");
                frm_InputPass.ShowDialog();
                if (CheckPri == "OK")
                {
                    item_Controlrun.IsChecked = true;
                    string strInsertLog = "Insert into SFISM4.R_SYSTEM_LOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC)"
                        + " Values"
                        + " (:EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                    var Insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsertLog,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="EMP_NO",Value=frm_InputPass.EMP_NO},
                            new SfcParameter{Name="PRG_NAME",Value="PACKING"},
                            new SfcParameter{Name="ACTION_TYPE",Value="PILOT_RUN"},
                            new SfcParameter{Name="ACTION_DESC",Value="PACK_BOXID:CHECKPILOTRUN(Pilot Run)--->NOCHECKPILOTRUN(Pilot Run);IP:" + localIP().ToString()}
                        }
                    });
                    //Writelog.WriteLog("SETTING:CHECK PIRUN->Consigned:TRUE;CHECK PIRUN->PILOT_RUN:FALSE");
                }
            }
            else
            {
                //Writelog.WriteLog("SETTING:CHECK PIRUN->Consigned:TRUE;CHECK PIRUN->PILOT_RUN:FALSE");
            }
        }

        private void item_Merge_Click(object sender, RoutedEventArgs e)
        {
            FormLotcarton frm_Lotcarton = new FormLotcarton(this, sfcClient);
            frm_Lotcarton.ShowDialog();
        }
        private async Task Checkprivilege()
        {
            string strGetPrivilege = "select count(*) aa from sfis1.c_emp_desc_t a,sfis1.c_privilege b where a.emp_no=b.emp"
            + " and b.PRG_NAME='PACKING' AND b.FUN='MERGE_BOX' and a.emp_no='" + empNo + "'";
            var qry_Privilege = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetPrivilege,
                SfcCommandType = SfcCommandType.Text
            });
            if (Int32.Parse(qry_Privilege.Data["aa"].ToString()) > 0)
            {
                item_Merge.IsEnabled = true;
            }
            else
            {
                item_Merge.IsEnabled = false;
            }
        }

        private void item_Closeboxid_Click(object sender, RoutedEventArgs e)
        {
            FormBOXID frm_BOXID = new FormBOXID(this,sfcClient);
            frm_BOXID.ShowDialog();
        }



        private void item_Label_local_Click(object sender, RoutedEventArgs e)
        {
            item_Label_lh.IsChecked = false;
            item_Label_gl.IsChecked = false;
            FormInputPassword frm_InputPass = new FormInputPassword(this,sfcClient,"");
            frm_InputPass.ShowDialog();
        }
        private void ShowParam(object sender, RoutedEventArgs e)
        {
            ShowParamForm frm_ShowParam = new ShowParamForm();
            frm_ShowParam.dataGrid.DataContext = dtParams;
            if (frm_ShowParam.dataGrid.DataContext == null)
            {
                return;
            }
            frm_ShowParam.sUrlFile = UrlLabelFile;
            frm_ShowParam.ShowDialog();
        }
        public async Task CloseBOXID()
        {
            string QTYBOXID, QTYSTR, oldboxid;
            int Startqty;
            QTYBOXID = "";
            string strGetParameter = $"SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOXID' AND VR_CLASS='{Edt_model.Text}' AND VR_ITEM='QTY'";
            var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetParameter,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Parameter.Data != null)
            {
                QTYSTR = qry_Parameter.Data["vr_value"].ToString();
                Startqty = Int32.Parse(qry_Parameter.Data["vr_name"].ToString());

                string strGetCount = $"select TRIM(TO_CHAR(COUNT(*),'{QTYSTR}')) QTY from sfism4.r107 where TRAY_NO='{MBOXID}'";
                var qry_Count = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCount,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Count.Data != null)
                {
                    QTYBOXID = qry_Count.Data["qty"].ToString();
                }
                oldboxid = MBOXID;
                MBOXID = MBOXID.Substring(0, Startqty - 1) + QTYBOXID + MBOXID.Substring(Startqty + QTYBOXID.Length, MBOXID.Length - (Startqty + QTYBOXID.Length - 1));
                Edt_BOXID.Text = MBOXID;

                try
                {
                    string strUpdatePallet = $"update SFIS1.C_PALLET_T set PALLET_NO='{MBOXID}' where PALLET_NO='{oldboxid}'";
                    var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = strUpdatePallet,
                        SfcCommandType = SfcCommandType.Text
                    });

                    string strUpdateWIP = $"update sfism4.r_wip_tracking_t set TRAY_NO='{MBOXID}' where TRAY_NO='{oldboxid}'";
                    var UpdateWIP = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = strUpdateWIP,
                        SfcCommandType = SfcCommandType.Text
                    });
                }
                catch (Exception)
                {
                    MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            string strGetPalletNO = $"select * from sfis1.c_pallet_t where pallet_no='{Edt_BOXID.Text}'";
            var qry_PalletNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetPalletNO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_PalletNO.Data == null)
            {
                lbError.Text = "00412 - " +  await GetPubMessageVN("00412");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "00412 - " + await GetPubMessageVN("00412");
                _er.MessageEnglish = "00412 - " + await GetPubMessage("00412");
                _er.ShowDialog();
                return;
            }
            if (Int32.Parse(lb_Count.Content.ToString()) < Int32.Parse(lb_Capacity.Content.ToString()))
            {
                var result = MessageBox.Show(await GetPubMessage("00062"), "Message", MessageBoxButton.OK, MessageBoxImage.Question);
                if (result != MessageBoxResult.OK)
                {
                    return;
                }
            }
            Lb_SendListBox_Data.Items.Clear();
            Lb_SendListBox_ItemName.Items.Clear();
            await delete_pallet_ini(Edt_BOXID.Text);
            await updatePalletFull();
            lb_Count.Content = "0";
            lst_ListBox1.Items.Clear();

            await P_BOXIDInformationForCodeSoft(Edt_BOXID.Text);

            Inputdata.Focus();
        }
        private bool killprocess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
            {
                process.Kill();
            }
            return true;
        }

        private void item_Reprint_Click(object sender, RoutedEventArgs e)
        {
            FormInputPassword frm_InputPass = new FormInputPassword(this, sfcClient, "REPRINT");
            frm_InputPass.ShowDialog();
            if (CheckPri == "OK")
            {
                FormReprint formReprint = new FormReprint(this, sfcClient);
                formReprint.ShowDialog();
            }
        }

        private void item_Visible_Click(object sender, RoutedEventArgs e)
        {
            VisibleLabel();
        }
        public void VisibleLabel()
        {
            if (File.Exists(MainWindow.FilePath))
            {
                ApplicationClass labApp = new ApplicationClass();
                try
                {
                    labApp.Documents.Open(MainWindow.FilePath, false);
                    Document doc = labApp.ActiveDocument;
                    doc.Application.Visible = true;
                }
                catch (Exception ex)
                {
                    ShowMessageForm _er = new ShowMessageForm();
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Mở file label có lỗi ngoại lệ: " + ex.Message;
                    _er.MessageEnglish = "Have exception when open label file: " + ex.Message;
                    _er.ShowDialog();
                }
            }
            else if (File.Exists(FilePath))
            {
                ApplicationClass labApp = new ApplicationClass();
                try
                {
                    labApp.Documents.Open(FilePath, false);
                    Document doc = labApp.ActiveDocument;
                    doc.Application.Visible = true;
                }
                catch (Exception ex)
                {
                    ShowMessageForm _er = new ShowMessageForm();
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Mở file label có lỗi ngoại lệ: " + ex.Message;
                    _er.MessageEnglish = "Have exception when open label file: " + ex.Message;
                    _er.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Can not find Label file!", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        public async void PrintToCodeSoft()
        {
            if (Not_Print.IsChecked==true) return;
            string _labelNameBak =  model_NameBak + ".bak";
            FilePath = _DirPath + LabelName;
            string _param_Name = "";

            //Down file label
            if (!File.Exists(FilePath))
            {
                try
                {
                    WebClient wc = new WebClient();
                    UrlLabelFile = await GetAddressLabel();
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
            if (File.Exists(FilePath))
            {
                try
                {
                    labApp.Documents.Open(FilePath, false);
                    doc = labApp.ActiveDocument;
                   
                    foreach (DataRow param in dtParams.Rows)
                    {
                        _param_Name = param["Name"].ToString().ToUpper();
                        try
                        {
                            for (int j = 1; j < doc.Variables.FormVariables.Count + 1; j++)
                            {
                                if (_param_Name == doc.Variables.FormVariables.Item(j).Name.ToString().ToUpper())
                                {
                                    doc.Variables.FormVariables.Item(_param_Name).Value = param["Value"].ToString();
                                }
                            }
                        }
                        catch
                        { }
                    }
                    //Call label tracking
                    if (!item_Reprint.IsChecked)
                    {
                        LabelTracking _lbt = new LabelTracking();
                        _lbt.sfcHttpClient = this.sfcClient;
                        try
                        {
                            doc.PrintDocument(0);
                            if (!await _lbt.doLabelTracking(LabelName, 0, labApp, doc))
                            {
                                ShowMessageForm _sh = new ShowMessageForm();
                                _sh.CustomFlag = true;
                                _sh.MessageVietNam = _lbt.LastError;
                                _sh.MessageEnglish = _lbt.LastError;
                                _sh.ShowDialog();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageForm _sh = new ShowMessageForm();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message;
                            _sh.MessageEnglish = "Check label tracking have exception: " + ex.Message;
                            _sh.ShowDialog();
                            return;
                        }
                        doc.PrintDocument(Count_ReprintLabel);
                    }
                    else
                    {
                        doc.PrintDocument(Count_ReprintLabel);
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message;
                    _sh.MessageEnglish = "Check label tracking have exception: " + ex.Message;
                    _sh.ShowDialog();
                    return;
                }
                finally
                {
                    doc = null;
                }
            }
        }
        private async Task<bool> P_BOXIDInformationForCodeSoft(string paramBOXID)
        {
            if(Not_Print.IsChecked==true)
            {
                return true;
            }

            bool new_flag = true;
            if (new_flag)
            {
                string indata = "LABELTYPE:PACKBOXID|BOXID:" + paramBOXID;
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.SP_GET_PARAMS",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="IN_DATA",Value=indata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                });
                dynamic _ads = result.Data;
                string _RES = _ads[1]["res"];
                if (_RES.StartsWith("OK"))
                {
                    string strOut = _ads[0]["out_data"];
                    dtParams.Clear();
                    foreach (var rows in strOut.Split('|'))
                    {
                        AddParams(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "");
                    }
                    LabelName = Edt_model.Text.Trim().Replace(".", "_") + "_BOX.LAB";
                    model_NameBak = Edt_model.Text.Trim() + "_BOX";
                    PrintToCodeSoft();
                    return true;
                }
                else
                {
                    lbError.Text = _RES.Split('|')[0].ToString();
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = _RES.Split('|')[1].ToString();
                    _er.MessageEnglish = _RES.Split('|')[0].ToString();
                    _er.ShowDialog();
                    return false;
                }
            }

            string C_MODEL_NAME, C_VERSION, C_MO, c_serial_number, strack_no;
            int Qty, BOXQTY,i, ipos;
            string sModelName, sVersion, sCustCode, C19_CustModelName, M_sCustModelDesc, M_sUPCEANData;

            string strGetDateTime = "SELECT TO_CHAR(sysdate, 'YYYYMMDD') date_time FROM dual";
            var qry_DateTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDateTime,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_DateTime.Data != null)
            {
                AddParams("date_time", qry_DateTime.Data["date_time"].ToString());
            }

            string strGetPackDate = "select TO_CHAR (MAX (in_station_time), 'YYYYMMDD') PackDate from sfism4.r117 where serial_number in(select serial_number from sfism4.r107 where tray_no='"+ paramBOXID + "') and group_name ='PACK_BOX'";
            var qry_PackDate = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetPackDate,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_PackDate.Data != null)
            {
                AddParams("PackBoxDate", qry_PackDate.Data["packdate"].ToString());
            }

            string strGetTrayNO = $"select * from sfism4.r_wip_tracking_t where tray_no='{paramBOXID}'";
            var qry_TrayNO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetTrayNO,
                SfcCommandType = SfcCommandType.Text
            });

            List<R107> result_r107 = new List<R107>();
            result_r107 = qry_TrayNO.Data.ToListObject<R107>().ToList();
            C_MO = result_r107[0].MO_NUMBER;
            c_serial_number = result_r107[0].SERIAL_NUMBER;
            C_MODEL_NAME = result_r107[0].MODEL_NAME;
            C_VERSION = result_r107[0].VERSION_CODE;
            strack_no = result_r107[0].TRACK_NO;

            string strGetQty = $"select count(*) qty from sfism4.r_wip_tracking_t where tray_no='{paramBOXID}'";
            var qry_Qty = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetQty,
                SfcCommandType = SfcCommandType.Text
            });
            Qty = Int32.Parse(qry_Qty.Data["qty"].ToString());

            string strGetModelVer = $"SELECT * FROM SFIS1.C_PACK_PARAM_T WHERE MODEL_NAME='{C_MODEL_NAME}' AND VERSION_CODE='{C_VERSION}' AND ROWNUM=1";
            var qry_ModelVer = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModelVer,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ModelVer.Data != null)
            {
                BOXQTY = Int32.Parse(qry_ModelVer.Data["tray_qty"].ToString());
            }
            else
            {
                BOXQTY = 0;
                lbError.Text = "00111 - " + await GetPubMessageVN("00111");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "00111 - " + await GetPubMessageVN("00111");
                _er.MessageEnglish = "00111 - " + await GetPubMessage("00111");
                _er.ShowDialog();
                return false;
            }

            string strGetCustSN = $"select * from sfism4.r_CUSTSN_T where serial_number in (select serial_number from sfism4.r_wip_tracking_t where tray_no='{paramBOXID}')";
            var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustSN,
                SfcCommandType = SfcCommandType.Text
            });
            if ((MODEL_TYPE.IndexOf("190") > -1) || (MODEL_TYPE.IndexOf("188") > -1) || (item_Tori_2G.IsChecked == true))
            {

            }
            if (qry_CustSN.Data.Count() > 0)
            {
                for (i = 1; i <= qry_CustSN.Data.Count(); i++)
                {
                    List<R_CUSTSN> result_custsn = new List<R_CUSTSN>();
                    result_custsn = qry_CustSN.Data.ToListObject<R_CUSTSN>().ToList();
                    AddParams("MACA" + i.ToString(), result_custsn[0].MAC1);
                    AddParams("MACB" + i.ToString(), result_custsn[0].MAC2);
                    AddParams("MACC" + i.ToString(), result_custsn[0].MAC3);
                    AddParams("MACD" + i.ToString(), result_custsn[0].MAC4);
                    AddParams("MACE" + i.ToString(), result_custsn[0].MAC5);
                    AddParams("MACF" + i.ToString(), result_custsn[0].MAC6);
                    AddParams("MACG" + i.ToString(), result_custsn[0].MAC7);
                    AddParams("MACH" + i.ToString(), result_custsn[0].MAC8);
                    AddParams("MACI" + i.ToString(), result_custsn[0].MAC9);
                    AddParams("MACJ" + i.ToString(), result_custsn[0].MAC10);

                    AddParams("MSNA" + i.ToString(), result_custsn[0].SSN1);
                    AddParams("MSNB" + i.ToString(), result_custsn[0].SSN2);
                    AddParams("MSNC" + i.ToString(), result_custsn[0].SSN3);
                    AddParams("MSND" + i.ToString(), result_custsn[0].SSN4);
                    AddParams("MSNE" + i.ToString(), result_custsn[0].SSN5);
                    AddParams("MSNF" + i.ToString(), result_custsn[0].SSN6);
                    AddParams("MSNG" + i.ToString(), result_custsn[0].SSN7);

                    AddParams("SN" + i.ToString(), result_custsn[0].SERIAL_NUMBER);
                }
            }

            string strGetCust = "select * from sfism4.r_CUSTSN_T where serial_number in (select key_part_sn from sfism4.R_WIP_KEYPARTS_T where serial_number in"
                + $" (select serial_number from sfism4.r_wip_tracking_t where tray_no='{paramBOXID}'))";
            var qry_Cust = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCust,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Cust.Data.Count() > 0)
            {
                for (i = 1;i<= qry_Cust.Data.Count();i++)
                {
                    List<R_CUSTSN> result_custsn = new List<R_CUSTSN>();
                    result_custsn = qry_Cust.Data.ToListObject<R_CUSTSN>().ToList();
                    AddParams("T_MACA" + i.ToString(), result_custsn[i-1].MAC1);
                    AddParams("T_MACB" + i.ToString(), result_custsn[i-1].MAC2);
                    AddParams("T_MACC" + i.ToString(), result_custsn[i-1].MAC3);
                    AddParams("T_MACD" + i.ToString(), result_custsn[i-1].MAC4);
                    AddParams("T_MACE" + i.ToString(), result_custsn[i-1].MAC5);

                    AddParams("T_MSNA" + i.ToString(), result_custsn[i-1].SSN1);
                    AddParams("T_MSNB" + i.ToString(), result_custsn[i-1].SSN2);
                    AddParams("T_MSNC" + i.ToString(), result_custsn[i-1].SSN3);
                    AddParams("T_MSND" + i.ToString(), result_custsn[i-1].SSN4);
                    AddParams("T_MSNE" + i.ToString(), result_custsn[i-1].SSN5);
                    AddParams("T_MSNF" + i.ToString(), result_custsn[i-1].SSN6);
                    AddParams("T_MSNG" + i.ToString(), result_custsn[i-1].SSN7);
                    AddParams("T_MSNH" + i.ToString(), result_custsn[i-1].SSN8);
                    AddParams("T_MSNI" + i.ToString(), result_custsn[i-1].SSN9);
                    AddParams("T_MSNJ" + i.ToString(), result_custsn[i-1].SSN10);
                    AddParams("T_MSNK" + i.ToString(), result_custsn[i-1].SSN11);
                    AddParams("T_MSNL" + i.ToString(), result_custsn[i-1].SSN12);

                    AddParams("T_MO" + i.ToString(), result_custsn[i-1].MO_NUMBER);
                    AddParams("T_SN" + i.ToString(), result_custsn[i-1].SERIAL_NUMBER);
                }
            }

            string MODEL_NAME = await GETMODELTYPE();
            if ((MODEL_NAME.IndexOf("190") > -1) && (item_Tori_2G.IsChecked == true))
            {
                string strGetDateCode = $"select MAX(SSN6) DATE_CODE from sfism4.r_CUSTSN_T where serial_number in (select serial_number from sfism4.r_wip_tracking_t where tray_no='{paramBOXID}')";
                var qry_DateCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetDateCode,
                    SfcCommandType = SfcCommandType.Text
                });
                AddParams("DATA_CODE", qry_DateCode.Data["data_code"].ToString());
            }
            AddParams("ModelName", C_MODEL_NAME);
            AddParams("ModelName", C_MO);
            AddParams("VERSION", C_VERSION);
            AddParams("QTY", Qty.ToString());
            AddParams("BOXID", paramBOXID);
            AddParams("TRACK_NO", strack_no);

            string strGetDataWIP = "select * from sfism4.r_wip_tracking_t"
                + " where tray_no = (select tray_no from sfism4.r_wip_tracking_t"
                + " where (serial_number = :sSerN"
                + " or  shipping_SN = :sSerN or tray_no = :sSerN) and rownum = 1)";
            var qry_DataWIP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataWIP,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="sSerN",Value=paramBOXID}
                }
            });
            if (qry_DataWIP.Data.Count() == 0)
            {
                MessageBox.Show(await GetPubMessage("00414"), "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            List<R107> result_r107_ = new List<R107>();
            result_r107_ = qry_DataWIP.Data.ToListObject<R107>().ToList();
            sCustCode = result_r107_[0].CUSTOMER_NO;
            sModelName = result_r107_[0].MODEL_NAME;
            sVersion = result_r107_[0].VERSION_CODE;

            string strGetC19 = "select * from sfis1.c_cust_snrule_t"
                + " where cust_code = '" + sCustCode + "'"
                + " and model_name = '" + sModelName + "'"
                + " and version_code = '" + sVersion + "'";
            var qry_C19 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetC19,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_C19.Data.Count() == 0)
            {
                lbError.Text = "00084 - " + await GetPubMessageVN("00084");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "00084 - " + await GetPubMessageVN("00084");
                _er.MessageEnglish = "00084 - " + await GetPubMessage("00084");
                _er.ShowDialog();
                return false;
            }
            else
            {
                List<CUST_SNRULE> resutl_cust_snrule = new List<CUST_SNRULE>();
                resutl_cust_snrule = qry_C19.Data.ToListObject<CUST_SNRULE>().ToList();
                C19_CustModelName = resutl_cust_snrule[0].CUST_MODEL_NAME;
                AddParams("CustModelName", C19_CustModelName);
                M_sCustModelDesc = resutl_cust_snrule[0].CUST_MODEL_DESC;
                AddParams("Modeldesc", M_sCustModelDesc);
                M_sUPCEANData = resutl_cust_snrule[0].UPCEANDATA;
                AddParams("UPCEANDATA", M_sUPCEANData);
            }
            try
            {
                //ipos = Edt_model.Text.IndexOf(".");
                //if (ipos == -1)
                //{
                //    LabelName = Edt_model.Text.Trim() + "_BOX.LAB";
                //    model_NameBak = Edt_model.Text.Trim() + "_BOX";
                //    PrintToCodeSoft();
                //}
                //else
                //{
                //    LabelName= Edt_model.Text.Trim().Replace(".","_") + "_BOX.LAB";
                //    model_NameBak = Edt_model.Text.Trim() + "_BOX";
                //    PrintToCodeSoft();
                //}
                LabelName = Edt_model.Text.Trim().Replace(".", "_") + "_BOX.LAB";
                model_NameBak = Edt_model.Text.Trim() + "_BOX";
                PrintToCodeSoft();
            }
            catch (Exception e)
            {
                lbError.Text = e.Message;
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Có lỗi phát sinh khi xác định tên hàng: " + e.Message;
                _er.MessageEnglish = "Have exceptions when call model: " + e.Message;
                _er.ShowDialog();
                return false;
            }
            return true;
        }

        private async Task<string> GetAddressLabel()
        {
            string ftplabelpath, ftpaddress, AddressLabel;
            try
            {
                ftplabelpath = "";
                if (item_Label_lh.IsChecked == true)
                {
                    string strGetModelName = $"SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='Z_BOX_LH'";
                    var qry_ModelName = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetModelName,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_ModelName.Data != null)
                    {
                        if (qry_ModelName.Data["standard"].ToString() == "Y")
                        {
                            ftplabelpath = qry_ModelName.Data["customer"].ToString();
                        }
                        //Get address file label
                        ftpaddress = qry_ModelName.Data["model_serial"].ToString();
                        AddressLabel = "http://"+ ftpaddress + ftplabelpath + "/";
                        return AddressLabel;
                    }
                    else
                    {
                        lbError.Text = await GetPubMessageVN("00172");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = await GetPubMessageVN("00172");
                        _er.MessageEnglish = await GetPubMessage("00172");
                        _er.ShowDialog();
                        return null;
                    }
                }
                else if (item_Label_gl.IsChecked == true || item_Label_local.IsChecked == true)
                {
                    string strGetModelName = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='Z_BOXID_GL'";
                    var qry_ModelName = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetModelName,
                        SfcCommandType = SfcCommandType.Text
                    });
                }
                else if (item_Label_local.IsChecked == true)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                lbError.Text = e.Message;
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Get Address Label Error!";
                _er.MessageEnglish = e.Message;
                _er.ShowDialog();
                return null;
            }
            return null;
        }
        public void AddParams(string _name, string _value)
        {
            if (_name.ToUpper() == "MODELNAME") Edt_model.Text = _value;
            if ((_name != null) && (_value != null))
            {
                Lb_SendListBox_ItemName.Items.Add(_name);
                Lb_SendListBox_Data.Items.Add(_value);
                if (dtParams.Columns.Count == 0)
                {
                    dtParams.Columns.Add("Name");
                    dtParams.Columns.Add("Value");
                }
                dtParams.Rows.Add(new object[] { _name, _value });
            }
        }


        private async Task afterProcess()
        {
            int iCapacity, iTrayCount;

            if (lb_Count.Content.ToString() == "0")
            {
                btn_CloseBoxID.IsEnabled = false;
                lst_ListBox1.Items.Clear();
            }
            else
            {
                btn_CloseBoxID.IsEnabled = true;

                iCapacity = Int32.Parse(lb_Capacity.Content.ToString());
                iTrayCount = Int32.Parse(lb_Count.Content.ToString());
                if (iTrayCount >= iCapacity)
                {
                    await CloseTray();
                    if (await checkMoTarget() == true)
                    {
                        lbError.Text = "00120 - " +  await GetPubMessageVN("00120");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "00120 - " + await GetPubMessageVN("00120");
                        _er.MessageEnglish = "00120 - " + await GetPubMessage("00120");
                        _er.ShowDialog();
                        return;
                    }
                }
                else
                {
                    if (await checkMoTarget() == true)
                    {
                        await CloseTray();
                        lbError.Text = "00120 - " + await GetPubMessageVN("00120");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "00120 - " + await GetPubMessageVN("00120");
                        _er.MessageEnglish = "00120 - " + await GetPubMessage("00120");
                        _er.ShowDialog();
                        return;
                    }
                }
            }
            await getMoInformation();
        }
        private async Task CloseTray()
        {
            int i;
            await updateClassQty();
            await updatePalletFull();
            CHECKSN_OK = false;
            editSerialNumber.Text = "";
            for ( i = 1; i <= 15; i++)
            {
                sLOAD_SSN[i].SSN_OK = false;
                sLOAD_MAC[i].MAC_OK = false;
                sLOAD_SSN[i].SSN = "";
                sLOAD_MAC[i].MAC = "";
            }
            sequenlist.Clear();
            Data_gridView.ItemsSource = null;
            Next_Step = "";
            strDataInput = "";
            lbError.Text = await GetPubMessageVN("00116");
            btn_CloseBoxID.IsEnabled = false;
        }
        private async Task updateClassQty()
        {
            string Class_Type, Class_Date;
            int WorkSection;

            Class_Type = "";
            Class_Date = "";
            WorkSection = 0;
            await GetClass(Class_Type, Class_Date, WorkSection);

            string strGetTotalQty = "SELECT SUM(PASS_QTY+FAIL_QTY+REPASS_QTY+REFAIL_QTY) TOTAL_QTY FROM SFISM4.R_STATION_REC_T"
                + " WHERE GROUP_NAME = :GroupName"
                + " AND CLASS = :Class"
                + " AND CLASS_DATE = :ClassDate"
                + " AND LINE_NAME = :LineName"
                + " AND SECTION_NAME = :SectionName";
            var qry_TotalQty = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetTotalQty,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="GroupName",Value=M_sThisGroup},
                    new SfcParameter{Name="Class",Value=Class_Type},
                    new SfcParameter{Name="ClassDate",Value=Class_Date},
                    new SfcParameter{Name="LineName",Value=Edt_linename.Text},
                    new SfcParameter{Name="SectionName",Value=M_sThisSection}                    
                }
            });
            if (qry_TotalQty.Data.Count() == 0)
            {
                Lb_ClassQty.Content = "0";
            }
            else
            {
                List<R_STATION_REC> result_station = new List<R_STATION_REC>();
                result_station = qry_TotalQty.Data.ToListObject<R_STATION_REC>().ToList();
                Lb_ClassQty.Content = result_station[0].TOTAL_QTY;
            }
        }

        private async Task GetClass(string Class_Type,string Class_Date,int WorkSection)
        {
            int LOC_WORK_SECTION;

            LOC_WORK_SECTION = await FindWorkSection();
            string strGetWorkDesc = "SELECT CLASS,DAY_DISTINCT"
                + " FROM SFIS1.C_WORK_DESC_T"
                + $" WHERE LINE_NAME = '{Edt_linename.Text}' AND SECTION_NAME = 'Default' AND WORK_SECTION ='{LOC_WORK_SECTION}'";
            var qry_WorkDesc = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetWorkDesc,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_WorkDesc.Data.Count() > 0)
            {
                List<C_WORK_DESC> result_work_desc = new List<C_WORK_DESC>();
                result_work_desc = qry_WorkDesc.Data.ToListObject<C_WORK_DESC>().ToList();

                if (result_work_desc[0].DAY_DISTINCT == "TODAY")
                {
                    Class_Date = System.DateTime.Now.ToString("YYYMMDD");
                }
                if (result_work_desc[0].DAY_DISTINCT == "TOMORROW")
                {
                    Class_Date = System.DateTime.Now.AddDays(-1).ToString("YYYMMDD");
                }
                if (result_work_desc[0].DAY_DISTINCT == "YESTERDAY")
                {
                    Class_Date = System.DateTime.Now.AddDays(+1).ToString("YYYMMDD");
                }
            }
            else
            {
                lbError.Text = "00378 - " + await GetPubMessageVN("00378");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "00378 - " + await GetPubMessageVN("00378");
                _er.MessageEnglish = "00378 - " + await GetPubMessage("00378");
                _er.ShowDialog();
                return;
            }
        }

        private async Task<int> FindWorkSection()
        {
            string strGetWorkSection = "SELECT WORK_SECTION"
                + " FROM SFIS1.C_WORK_DESC_T"
                + " WHERE START_TIME <= TO_CHAR(SYSDATE ,'HH24MI')"
                + " AND END_TIME > TO_CHAR(SYSDATE ,'HH24MI')"
                + " AND LINE_NAME ='" + Edt_linename.Text + "'";
            var qry_WorkSection = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetWorkSection,
                SfcCommandType = SfcCommandType.Text
            });
            return Int32.Parse(qry_WorkSection.Data["work_section"].ToString());
        }

        private async Task getMoInformation()
        {
            string strGetMOIn4 = "select r5.target_qty, r7.packing_qty"
                + " from sfism4.r_mo_base_t r5,"
                + " (select count(serial_number) packing_qty, mo_number"
                + " from sfism4.r_wip_tracking_t"
                + " where mo_number = '" + Edt_monumber.Text + "'"
                + " group by mo_number) r7"
                + " where r5.mo_number = r7.mo_number"
                + " and r5.mo_number = '" + Edt_monumber.Text + "'";
            var qry_MOIn4 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetMOIn4,
                SfcCommandType = SfcCommandType.Text
            });
            Lb_MOTarget.Content = qry_MOIn4.Data["target_qty"]?.ToString() ?? "";
            Lb_PackingQty.Content = qry_MOIn4.Data["packing_qty"]?.ToString() ?? "";
        }
        public async void editEmployee_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                editSerialNumber.IsEnabled = false;
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " select * from SFIS1.c_emp_desc_t where emp_pass = '" + edtpassword.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });

                if (result.Data != null)
                {
                    empNo = result.Data["emp_no"].ToString();
                    empPass = result.Data["emp_bc"].ToString();
                }

                var loginInfo = new
                {
                    TYPE = "LOGIN",
                    PRG_NAME = "PACKING",
                    UserName = empNo,
                    Password = empPass
                };

                //Tranform it to Json object
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
                        Edt_Database.Text = digits[0].ToString();
                        edtpassword.Text = digits[1].Substring(10, digits[1].ToString().Length - 10).ToString();
                        Edt_User.Text = empNo + " - " + await getUser();
                        Edt_Ver.Text = getRunningVersion().ToString();
                        Edt_IP.Text = localIP().ToString();
                        Edt_local.Text = GetHostMacAddress().ToString();
                        Inputdata.Focus();
                        Inputdata.IsEnabled = true;
                        edtpassword.IsEnabled = false;
                    }
                    else
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = Ok;
                        _er.MessageVietNam = Ok;
                        _er.ShowDialog();
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = ex.Message;
                    _er.MessageVietNam = ex.Message;
                    _er.ShowDialog();
                    this.Close();
                }

                for (int i = 1; i <= 15; i++)
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

                for (int i = 1; i <= 15; i++)
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

                item_Reprint.IsEnabled = true;
                if (item_Tori_2G.IsChecked == false)
                {
                    lbError.Text = await GetPubMessage("00008");
                }
                Inputdata.Focus();
            }
            
        }

        private async Task updateR107()
        {
            string next_group, date_code, ULOGOSTR;
            next_group = "";

            string MODEL_NAME = await GETMODELTYPE();
            string MODEL_SERIAL = await getmodelserial();
            try
            {
                if ((MODEL_NAME.IndexOf("190") > -1) && (item_Assy_input.IsChecked = false) || item_autotray.IsChecked == true)
                {
                    string strGetTrayNO = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE  TRAY_NO = '{editSerialNumber.Text}'";
                    var qry_TrayNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetTrayNO,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_TrayNO.Data != null)
                    {
                        string strUpdateWIP = $"update sfism4.r_wip_tracking_t set TRACK_NO = tray_no where TRAY_NO = '{editSerialNumber.Text}'";
                        var UpdateWIP = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = strUpdateWIP,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }

                    string strGetData = "select distinct group_next from sfis1.c_route_control_t a,sfism4.r_wip_tracking_t b where a.route_code"
                        + $" =b.special_route and a.state_flag='0' and a.group_name='{M_sThisGroup}' and step_SEQUENCE in ("
                        + $" select min(step_sequence) from sfis1.c_route_control_t where group_name='{M_sThisGroup}' and state_flag='0'"
                        + $" and route_code=b.special_route) and   track_no='{editSerialNumber.Text}'";
                    var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetData,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Data.Data != null)
                    {
                        next_group = qry_Data.Data["group_next"].ToString();
                    }

                    string strUpdateR107 = "update sfism4.r_wip_tracking_t"
                        + $" set tray_no = '{MBOXID}'"
                        + $" where TRACK_NO = '{editSerialNumber.Text}'";
                    var UpdateR107 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = strUpdateR107,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (MODEL_NAME.IndexOf("187") > -1)
                    {
                        date_code = await GetDateCode_All();
                        var UpdateCustSN = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = $"update SFISM4.R_CUSTSN_T set SSN6 = '{date_code}' where serial_number IN ( SELECT SERIAL_NUMBER FROM SFISM4.R107  WHERE TRACK_NO = '{editSerialNumber.Text}')",
                            SfcCommandType = SfcCommandType.Text
                        });

                        var InsertCustBak = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = $"INSERT INTO SFISM4.R_CUSTSN_T_BAK SELECT * FROM  SFISM4.R_CUSTSN_T  where serial_number IN ( SELECT SERIAL_NUMBER FROM SFISM4.R107  WHERE TRACK_NO = '{editSerialNumber.Text}')",
                            SfcCommandType = SfcCommandType.Text
                        });
                    }

                    //var Insert_r117 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    //{
                    //    CommandText = $"INSERT INTO SFISM4.R_SN_DETAIL_T SELECT * FROM SFISM4.R107  WHERE TRACK_NO = '{editSerialNumber.Text}'",
                    //    SfcCommandType = SfcCommandType.Text
                    //});
                }
                else
                {
                    string strGetCustSN = $"Select * from SFISM4.R_CUSTSN_T where  SERIAL_NUMBER ='{editSerialNumber.Text}'";
                    var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CustSN.Data == null)
                    {   // CHO NAY SON SUA NHUNG CHI LIEN REMARK LAI VI CHUYEN SANG CHECK DOAN KHAC 2022-09-23
                        //string strChkDupH_CustSN = $"select * from SFISM4.H_CUSTSN_T where mac1 ='{sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC}' or mac1 ='{sLOAD_MAC[2].sMACID_CODE + sLOAD_MAC[2].MAC}' or mac2 ='{sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC}' or mac2 ='{sLOAD_MAC[2].sMACID_CODE + sLOAD_MAC[2].MAC}'";
                        //var qry_ChkDupH_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        //{
                        //    CommandText = strChkDupH_CustSN,
                        //    SfcCommandType = SfcCommandType.Text
                        //});
                        //if (qry_ChkDupH_CustSN.Data != null)
                        //{
                        //    lbError.Text = "Dup data in H_CUSTSN(Call IT Check)!!";
                        //    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        //    _er.CustomFlag = true;
                        //    _er.MessageVietNam = "Trung lap du lieu trong R_CUSTSN!";
                        //    _er.MessageEnglish = "Dup data in H_CUSTSN(Call IT Check)!!";
                        //    _er.ShowDialog();
                        //    return;
                        //}
                        //string strChkDupCustSN = $"select * from SFISM4.R_CUSTSN_T where mac1 ='{sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC}' or mac1 ='{sLOAD_MAC[2].sMACID_CODE + sLOAD_MAC[2].MAC}' or mac2 ='{sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC}' or mac2 ='{sLOAD_MAC[2].sMACID_CODE + sLOAD_MAC[2].MAC}'";
                        //var qry_ChkDupCustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        //{
                        //    CommandText = strChkDupCustSN,
                        //    SfcCommandType = SfcCommandType.Text
                        //});
                        //if (qry_ChkDupCustSN.Data != null)
                        //{
                        //    lbError.Text = "Dup data in R_CUSTSN(Call IT Check)!!";
                        //    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        //    _er.CustomFlag = true;
                        //    _er.MessageVietNam = "Trung lap du lieu trong R_CUSTSN!";
                        //    _er.MessageEnglish = "Dup data in R_CUSTSN(Call IT Check)!!";
                        //    _er.ShowDialog();
                        //    return;
                        //}
                        // CHO NAY SON SUA NHUNG CHI LIEN REMARK LAI VI CHUYEN SANG CHECK DOAN KHAC 2022-09-23
                        try
                        {
                            string strInsertCustSN = "Insert into SFISM4.R_CUSTSN_T (MO_NUMBER,SERIAL_NUMBER,GROUP_NAME,SSN1,SSN2,SSN3,SSN4,SSN5,SSN6,SSN7,SSN8,SSN9,SSN10,SSN11,SSN12,MAC1,MAC2,MAC3,MAC4,MAC5,MAC6,MACID7,MAC8,MAC9,MAC10,MAC11)"//,MAC6,MAC7,MAC8,MAC9,MAC10,MAC11,MAC12,MAC13,MAC14,MAC15
                        + " VALUES"
                        + " ('" + Edt_monumber.Text + "','" + editSerialNumber.Text + "','" + M_sThisGroup + "',"
                        + " '" + sLOAD_SSN[1].sSHIPPINGSN_CODE + sLOAD_SSN[1].SSN + "',"
                        + " '" + sLOAD_SSN[2].sSHIPPINGSN_CODE + sLOAD_SSN[2].SSN + "',"
                        + " '" + sLOAD_SSN[3].sSHIPPINGSN_CODE + sLOAD_SSN[3].SSN + "',"
                        + " '" + sLOAD_SSN[4].sSHIPPINGSN_CODE + sLOAD_SSN[4].SSN + "',"
                        + " '" + sLOAD_SSN[5].sSHIPPINGSN_CODE + sLOAD_SSN[5].SSN + "',"
                        + " '" + sLOAD_SSN[6].sSHIPPINGSN_CODE + sLOAD_SSN[6].SSN + "',"
                        + " '" + sLOAD_SSN[7].sSHIPPINGSN_CODE + sLOAD_SSN[7].SSN + "',"
                        + " '" + sLOAD_SSN[8].sSHIPPINGSN_CODE + sLOAD_SSN[8].SSN + "',"
                        + " '" + sLOAD_SSN[9].sSHIPPINGSN_CODE + sLOAD_SSN[9].SSN + "',"
                        + " '" + sLOAD_SSN[10].sSHIPPINGSN_CODE + sLOAD_SSN[10].SSN + "',"
                        + " '" + sLOAD_SSN[11].sSHIPPINGSN_CODE + sLOAD_SSN[11].SSN + "',"
                        + " '" + sLOAD_SSN[12].sSHIPPINGSN_CODE + sLOAD_SSN[12].SSN + "',"
                        + " '" + sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC + "',"
                        + " '" + sLOAD_MAC[2].sMACID_CODE + sLOAD_MAC[2].MAC + "',"
                        + " '" + sLOAD_MAC[3].sMACID_CODE + sLOAD_MAC[3].MAC + "',"
                        + " '" + sLOAD_MAC[4].sMACID_CODE + sLOAD_MAC[4].MAC + "',"
                        + " '" + sLOAD_MAC[5].sMACID_CODE + sLOAD_MAC[5].MAC + "',"
                        + " '" + sLOAD_MAC[6].sMACID_CODE + sLOAD_MAC[6].MAC + "',"
                        + " '" + sLOAD_MAC[7].sMACID_CODE + sLOAD_MAC[7].MAC + "',"
                        + " '" + sLOAD_MAC[8].sMACID_CODE + sLOAD_MAC[8].MAC + "',"
                        + " '" + sLOAD_MAC[9].sMACID_CODE + sLOAD_MAC[9].MAC + "',"
                        + " '" + sLOAD_MAC[10].sMACID_CODE + sLOAD_MAC[10].MAC + "',"
                        + " '" + sLOAD_MAC[15].sMACID_CODE + sLOAD_MAC[11].MAC + "')";
                            var Insert_CustSN = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = strInsertCustSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Insert cust have exception" + ex.Message);
                            return;
                        }
                        
                    }
                    else
                    {
                        string strInsertCustBak = $"INSERT INTO SFISM4.R_CUSTSN_T_BAK SELECT * FROM  SFISM4.R_CUSTSN_T  where  serial_number = '{editSerialNumber.Text}'";
                        var Insert_rcust_bak = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = strInsertCustBak,
                            SfcCommandType = SfcCommandType.Text
                        });
                        string dataupdatecust = string.Empty;
                       
                        if(sLOAD_SSN[1].SSN != "" && sLOAD_SSN[1].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN1 = '" + sLOAD_SSN[1].sSHIPPINGSN_CODE + sLOAD_SSN[1].SSN + "',";
                        }
                        if (sLOAD_SSN[2].SSN != "" && sLOAD_SSN[2].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN2 = '" + sLOAD_SSN[2].sSHIPPINGSN_CODE + sLOAD_SSN[2].SSN + "',";
                        }
                        if (sLOAD_SSN[3].SSN != "" && sLOAD_SSN[3].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN3 = '" + sLOAD_SSN[3].sSHIPPINGSN_CODE + sLOAD_SSN[3].SSN + "',";
                        }
                        if (sLOAD_SSN[4].SSN != "" && sLOAD_SSN[4].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN4 = '" + sLOAD_SSN[4].sSHIPPINGSN_CODE + sLOAD_SSN[4].SSN + "',";
                        }
                        if (sLOAD_SSN[5].SSN != "" && sLOAD_SSN[5].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN5 = '" + sLOAD_SSN[5].sSHIPPINGSN_CODE + sLOAD_SSN[5].SSN + "',";
                        }
                        if (sLOAD_SSN[6].SSN != "" && sLOAD_SSN[6].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN6 = '" + sLOAD_SSN[6].sSHIPPINGSN_CODE + sLOAD_SSN[6].SSN + "',";
                        }
                        if (sLOAD_SSN[7].SSN != "" && sLOAD_SSN[7].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN7 = '" + sLOAD_SSN[7].sSHIPPINGSN_CODE + sLOAD_SSN[7].SSN + "',";
                        }
                        if (sLOAD_SSN[8].SSN != "" && sLOAD_SSN[8].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN8 = '" + sLOAD_SSN[8].sSHIPPINGSN_CODE + sLOAD_SSN[8].SSN + "',";
                        }
                        if (sLOAD_SSN[9].SSN != "" && sLOAD_SSN[9].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN9 = '" + sLOAD_SSN[9].sSHIPPINGSN_CODE + sLOAD_SSN[9].SSN + "',";
                        }
                        if (sLOAD_SSN[10].SSN != "" && sLOAD_SSN[10].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN10 = '" + sLOAD_SSN[10].sSHIPPINGSN_CODE + sLOAD_SSN[10].SSN + "',";
                        }
                        if (sLOAD_SSN[11].SSN != "" && sLOAD_SSN[11].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN11 = '" + sLOAD_SSN[11].sSHIPPINGSN_CODE + sLOAD_SSN[11].SSN + "',";
                        }
                        if (sLOAD_SSN[12].SSN != "" && sLOAD_SSN[12].SSN != null)
                        {
                            dataupdatecust = dataupdatecust + " SSN12 = '" + sLOAD_SSN[12].sSHIPPINGSN_CODE + sLOAD_SSN[12].SSN + "',";
                        }
                        if(sLOAD_MAC[1].MAC != "" && sLOAD_MAC[1].MAC != null)
                        {
                            dataupdatecust = dataupdatecust + " mac1 = '" + sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC + "',";
                        }
                        if (sLOAD_MAC[2].MAC != "" && sLOAD_MAC[2].MAC != null)
                        {
                            dataupdatecust = dataupdatecust + " mac2 = '" + sLOAD_MAC[2].sMACID_CODE + sLOAD_MAC[2].MAC + "',";
                        }
                        dataupdatecust = dataupdatecust + " IN_STATION_TIME =SYSDATE,"
                               + " GROUP_NAME ='" + M_sThisGroup + "'";
                        string strUpdateCustSN = "update SFISM4.R_CUSTSN_T"
                              + " set"
                              + dataupdatecust
                              + " where   serial_number = '" + editSerialNumber.Text + "'";
                        var Update_CustSN = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = strUpdateCustSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }

                    
                    

                    if (MODEL_NAME.IndexOf("187") > -1)
                    {
                        date_code = await GetDateCode_All();
                        var strUpdateSSN6 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = $"update SFISM4.R_CUSTSN_T set SSN6 = '{date_code}' where  ssn1 = '{editSerialNumber.Text}' or serial_number = '{editSerialNumber.Text}'",
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    //Add get LotID by Z 2023/05/15
                    var resultLot = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.FIND_LOTID_RULE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name="MODEL",Value=editSerialNumber.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                            new SfcParameter {Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                        }
                    });
                    dynamic dyn = resultLot.Data;
                    string OutPut = dyn[0]["res"];
                    string updateLotID = " ";
                    if (OutPut != null)
                    {
                        if (OutPut.IndexOf("BADSN") > -1)
                        {
                            lbError.Text = "LOT_ID ERR!";
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "Sinh ma LOTID loi:" + OutPut + "";
                            _er.MessageEnglish = "LOT_ID ERR " + OutPut + "!";
                            _er.ShowDialog();
                            return;
                        }

                        if (OutPut.Trim() != "")
                        {
                            updateLotID = ",mo_number_old='" + OutPut + "' ";
                        }
                    }
                    //End get LotID by Z 2023/05/15

                    if (MODEL_NAME.IndexOf("204") > -1)
                    {
                        string strUpdateShippingSN = "update sfism4.r_wip_tracking_t"
                            + " set"
                            + " shipping_sn = '" + sLOAD_SSN[1].sSHIPPINGSN_CODE + sLOAD_SSN[1].SSN + "',"
                            + " MSN = '" + sLOAD_SSN[2].sSHIPPINGSN_CODE + sLOAD_SSN[2].SSN + "'"
                            + updateLotID 
                            + " where  serial_number = '" + editSerialNumber.Text + "'";
                        var UpdateShippingSN = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = strUpdateShippingSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    else
                    {
                        if (sLOAD_SSN[1].SSN != "")
                        {
                            string strUpdateShippingSN = "update sfism4.r_wip_tracking_t set "
                            + " tray_no = '" + MBOXID + "', location='Cfg',"
                            + " shipping_sn = '" + sLOAD_SSN[1].sSHIPPINGSN_CODE + sLOAD_SSN[1].SSN + "',"
                            + " ATE_STATION_NO='"+ Edt_local.Text.Trim().Replace(":","") + "' ,"
                            //+ " shipping_sn2 = '" + sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC + "',"
                            + " MSN = '" + sLOAD_SSN[2].sSHIPPINGSN_CODE + sLOAD_SSN[2].SSN + "'"
                            + updateLotID 
                            + " where  serial_number = '" + editSerialNumber.Text + "'";
                            var UpdateShippingSN = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = strUpdateShippingSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                        }
                        else
                        {
                            string strUpdateShippingSN = "update sfism4.r_wip_tracking_t set "
                            + " tray_no = '" + MBOXID + "', location='Cfg',"
                            + " ATE_STATION_NO='" + Edt_local.Text.Trim().Replace(":", "") + "' ,"
                            //+ " shipping_sn2 = '" + sLOAD_MAC[1].sMACID_CODE + sLOAD_MAC[1].MAC + "',"
                            + " MSN = '" + sLOAD_SSN[2].sSHIPPINGSN_CODE + sLOAD_SSN[2].SSN + "'"
                            + updateLotID
                            + " where serial_number = '" + editSerialNumber.Text + "'";
                            var UpdateShippingSN = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = strUpdateShippingSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                        }
                    }

                    //Chk Substring(serial_number voi shipping_sn2 no same)
                    //string strChkShipping_SN2 = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T where  serial_number = '{editSerialNumber.Text}' and SHIPPING_SN2 ='{editSerialNumber.Text.Substring(0, 12)}'";
                    //var ChkShipping_SN2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    //{
                    //    CommandText = strChkShipping_SN2,
                    //    SfcCommandType = SfcCommandType.Text
                    //});
                    //if (ChkShipping_SN2.Data.Count() == 0)
                    //{
                    //    lbError.Text = "Du lieu MAC vs SHIPPING_SN2 khong giong nhau";
                    //    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    //    _er.CustomFlag = true;
                    //    _er.MessageVietNam = "Du lieu MAC vs SHIPPING_SN2 khong giong nhau";
                    //    _er.MessageEnglish = "Data MAC and SHIPPING_SN2 no same";
                    //    _er.ShowDialog();
                    //    return;
                    //}

                    //Chk Substring(serial_number voi MAC1 no same)
                    //string strChkMac1 = $"SELECT * FROM SFISM4.R_CUSTSN_T where  serial_number = '{editSerialNumber.Text}' and MAC1 ='{editSerialNumber.Text.Substring(0, 12)}'";
                    //var ChkMac1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    //{
                    //    CommandText = strChkMac1,
                    //    SfcCommandType = SfcCommandType.Text
                    //});
                    //if (ChkShipping_SN2.Data.Count() == 0)
                    //{
                    //    lbError.Text = "Du lieu MAC vs MAC1 khong giong nhau";
                    //    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    //    _er.CustomFlag = true;
                    //    _er.MessageVietNam = "Du lieu MAC vs MAC1 khong giong nhau";
                    //    _er.MessageEnglish = "Data MAC and MAC1 no same";
                    //    _er.ShowDialog();
                    //    return;
                    //}
                    //if (MODEL_NAME.IndexOf("207") > -1)
                    //{
                    //    ULOGOSTR = sLOAD_SSN[5].sSHIPPINGSN_CODE + sLOAD_SSN[5].SSN;
                    //    if ((ULOGOSTR == "609-00116-03") || (ULOGOSTR == "609-00114-03") || (ULOGOSTR == "609-00115-02") || (ULOGOSTR == "609-00113-02"))
                    //    {
                    //        string strGetSN = $"SELECT * FROM SFISM4.R_KEYPART_COLOR_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}'";
                    //        var qry_GetSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    //        {
                    //            CommandText = strGetSN,
                    //            SfcCommandType = SfcCommandType.Text
                    //        });
                    //        if (qry_GetSN.Data == null)
                    //        {
                    //            string strUpdateColor = "update SFISM4.R_KEYPART_COLOR_T"
                    //                + $" set serial_number='{editSerialNumber.Text}',group_name='{M_sThisGroup}',in_station_time=sysdate"
                    //                + $" WHERE SERIAL_NUMBER IS NULL  AND KEYPART_NAME='{ULOGOSTR}'  AND MO_NUMBER='{Edt_monumber.Text}' and rownum=1";
                    //            var UpdateColor = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    //            {
                    //                CommandText = strUpdateColor,
                    //                SfcCommandType = SfcCommandType.Text
                    //            });
                    //        }
                    //    }
                    //}
                }
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
            string StrMo_Date, StrTime;

            string strGetSysDateTime = "Select SysDate DateTime,To_Char(SysDate,'yyyymmdd') Mo_Date, "
                 + "  To_Char(SysDate, 'hh24mi') Time "
                 + " From Dual";
            var qry_SysDateTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetSysDateTime,
                SfcCommandType = SfcCommandType.Text
            });
            System.DateTime datetime = Convert.ToDateTime(qry_SysDateTime.Data["datetime"].ToString());
            StrMo_Date = qry_SysDateTime.Data["mo_date"].ToString();
            StrTime = qry_SysDateTime.Data["time"].ToString();

            try
            {
                var query_work = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "select * from sfis1.c_work_desc_t"
                    + " where start_time <= :sTime"
                    + " and end_time >= :sTime"
                    + " and line_name = :sLine",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="sTime",Value=System.DateTime.Now.ToString("hhmm") },
                        new SfcParameter {Name="sLine",Value=Edt_linename.Text }
                    }
                });
                if (query_work.Data != null)
                {
                    sWorkSection = query_work.Data["work_section"].ToString();
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
                        new SfcParameter {Name="Line",Value=Edt_linename.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Section",Value=M_sThisSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="W_Station",Value=M_sThisStation,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="DateTime",Value= datetime,SfcParameterDataType=SfcParameterDataType.Date,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="EC",Value="N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Mo_Date",Value=StrMo_Date,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="MyGroup",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="W_Section",Value=sWorkSection,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Data",Value=editSerialNumber.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
            }
            catch (Exception e)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = e.Message;
                _er.MessageEnglish = e.Message;
                _er.ShowDialog();
                return;
            }
        }
        private async void InputMo_number_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(Input_Monumber.Text.ToString()))
                {
                    await mInputMo_number();
                }
                else
                {
                    lbError.Text = "Data is null!";
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Vui long nhap du lieu!";
                    _er.MessageEnglish = "Data is null!";
                    _er.ShowDialog();
                    return;
                }
            }
        }

        private async Task mInputMo_number()
        {
            string close_flag, bom_no, MODEL_SITE;
            MODEL_SITE = "";

            string strGerDataMO = $"select * from sfism4.r_mo_base_t where mo_number='{Input_Monumber.Text.Trim()}' and rownum=1";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGerDataMO,
                SfcCommandType = SfcCommandType.Text
            });
            List<R105> result_r105 = new List<R105>();
            result_r105 = qry_DataMO.Data.ToListObject<R105>().ToList();
            if (qry_DataMO.Data.Count() > 0)
            {
                close_flag = result_r105[0].CLOSE_FLAG;
                if (close_flag == "3")
                {
                    lbError.Text = "00013 - " + await GetPubMessageVN("00013");
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "00013 - " + await GetPubMessageVN("00013");
                    _er.MessageEnglish = "00013 - " + await GetPubMessage("00013");
                    _er.ShowDialog();
                    Input_Monumber.Focus();
                    Input_Monumber.SelectAll();
                    return;
                }
                else if (close_flag == "5")
                {
                    lbError.Text = "00014 - " + await GetPubMessageVN("00014");
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "00014 - " + await GetPubMessageVN("00014");
                    _er.MessageEnglish = "00014 - " + await GetPubMessage("00014");
                    _er.ShowDialog();
                    Input_Monumber.Focus();
                    Input_Monumber.SelectAll();
                    return;
                }

                Edt_model.Text = result_r105[0].MODEL_NAME;
                bom_no = result_r105[0].BOM_NO;
                Lb_PackingQty.Content = result_r105[0].INPUT_QTY;
                Lb_MOTarget.Content = result_r105[0].TARGET_QTY;
                Edt_Version.Text = result_r105[0].VERSION_CODE;
                MO_TYPE = result_r105[0].MO_TYPE;
                Mmodel_type = result_r105[0].MODEL_NAME;

                string strGetModelName = $"select * from SFIS1.C_MODEL_DESC_T A,SFIS1.C_MODEL_SITE_T B where A.MODEL_NAME=B.MODEL_NAME AND A.MODEL_NAME='{Mmodel_type}' and rownum=1";
                var qry_ModelName = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetModelName,
                    SfcCommandType = SfcCommandType.Text
                });
                List<C104> result_model_name = new List<C104>();
                result_model_name = qry_ModelName.Data.ToListObject<C104>().ToList();
                Mmodel_type = result_model_name[0].MODEL_TYPE;
                MODEL_SITE = result_model_name[0].SITE;

                string strGetBOMNO = $"select * from sfis1.c_bom_keypart_t where group_name='{M_sThisGroup}' and bom_no='{bom_no}'";
                var qry_BOMNO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetBOMNO,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_BOMNO.Data.Count() > 0)
                {
                    List<BOM> result_bom = new List<BOM>();
                    result_bom = qry_BOMNO.Data.ToListObject<BOM>().ToList();
                    if (Edt_Keyparts.Text == "")
                    {
                        Edt_Keyparts.Text = result_bom[0].KEY_PART_NO;
                    }
                    else
                    {
                        Edt_Keyparts.Text = Edt_Keyparts.Text +" - "+ result_bom[0].KEY_PART_NO;
                    }
                    if (MODEL_SITE == "UBNT")
                    {
                        Input_Monumber.IsEnabled = false;
                    }
                    else
                    {
                        Input_Monumber.IsEnabled = false;
                        Inputdata.IsEnabled = true;
                        Inputdata.Focus();
                        lbError.Text = "000 - Please Input Data";
                    }
                }
                else
                {
                    lbError.Text = "40316 - " + await GetPubMessageVN("40316");
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "40316 - " + await GetPubMessageVN("40316");
                    _er.MessageEnglish = "40316 - " + await GetPubMessage("40316");
                    _er.ShowDialog();
                    Input_Monumber.SelectAll();
                    Input_Monumber.Focus();
                    return;
                }
            }
            else
            {
                lbError.Text = "00004 - " + await GetPubMessageVN("00004");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "00004 - " + await GetPubMessageVN("00004");
                _er.MessageEnglish = "00004 - " + await GetPubMessage("00004");
                _er.ShowDialog();
                Input_Monumber.SelectAll();
                Input_Monumber.Focus();
                return;
            }
        }

        private async void Edtmac_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (!string.IsNullOrEmpty(Edtmac.Text.ToString()))
                {
                    await Edmacinputdata();
                }
                else
                {
                    lbError.Text = "Data is null!";
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Vui long nhap du lieu!";
                    _er.MessageEnglish = "Data is null!";
                    _er.ShowDialog();
                    return;
                }
            }
        }
        private async void InputData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                startInput();
            }
        }
        private async void startInput()
        {
            strDataInput = Inputdata.Text.Trim();
            Inputdata.Text = "";
            if (!string.IsNullOrEmpty(strDataInput.ToString()))
            {
                await mInputData();
            }
            else
            {
                lbError.Text = "Data is null!";
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "Vui long nhap du lieu!";
                _er.MessageEnglish = "Data is null!";
                _er.ShowDialog();
                return;
            }
            
        }
        public async Task<bool> Check_dup_cust_sn(string mac)
        {
            if (!string.IsNullOrEmpty(mac))
            {
                //string check_r_cust = $"select * from SFISM4.r_CUSTSN_T where mac1='{mac}' or mac2='{mac}' and rownum=1";
                //var qry_check_r_cust = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                //{
                //    CommandText = check_r_cust,
                //});
                //if (qry_check_r_cust.Data != null)
                //{
                //    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                //    _er.CustomFlag = true;
                //    _er.MessageVietNam = "Trung lap du lieu trong R_CUSTSN! MAC: "+mac+" of serial_number: "+qry_check_r_cust.Data["serial_number"];
                //    _er.MessageEnglish = "Dup data in  R_CUSTSN! (Call IT Check)!!";
                //    _er.ShowDialog();
                //    return false;
                //}
                //else
                //{
                    string check_h_cust= $"select * from SFISM4.H_CUSTSN_T where mac1='{mac}' or mac2='{mac}' and rownum=1";
                    var qry_check_H_cust = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = check_h_cust,
                    });
                    if (qry_check_H_cust.Data != null)
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "Trung lap du lieu trong H_CUSTSN! MAC: " + mac + " of serial_number: " + qry_check_H_cust.Data["serial_number"];
                        _er.MessageEnglish = "Dup data in  H_CUSTSN! (Call IT Check)!!";
                        _er.ShowDialog();
                        return false;
                    }
              }


            //}
            return true;
        }
        private void showMessage(string content)
        {
            string MessageEnglish = "", MessageVietNam = "";
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
            lbError.Text = MessageEnglish;
            ShowMessageForm frmMessage = new ShowMessageForm(sfcClient);
            frmMessage.CustomFlag = true;
            frmMessage.MessageVietNam = MessageVietNam;
            frmMessage.MessageEnglish = MessageEnglish;
            frmMessage.ShowDialog();
        }
        public async Task zPrintBox(string strParams,string SN)
        {
            dtParams.Clear();
            foreach (var rows in strParams.Split('|'))
            {
                AddParams(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "");
            }

            if (PrintSN.IsChecked == true)
            {
                string sql = "select serial_number from sfism4.r107 where tray_no = (select tray_no from sfism4.r107 where serial_number ='"+ SN + "' or tray_no ='" + SN + "')";
                var SNParam = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (SNParam.Data.Count() != 0)
                {
                    int k = 1;  
                    foreach (var item in SNParam.Data)
                    {
                        dtParams.Rows.Add(new object[] { "SN" + k, item["serial_number"].ToString() });
                        k++;
                    }
                }
            }
            

            LabelName = Edt_model.Text.Trim().Replace(".", "_") + "_BOX.LAB";
            model_NameBak = Edt_model.Text.Trim() + "_BOX";
            PrintToCodeSoft();
        }
        private async Task mInputData()
        {
            string TRAY_STR, TMP_SN, TMPMODEL, ControlTime, OutTime, temptsn, checkfcdver, checkbomosver, checkmacprestr, temp_Next_Step;
            string MSN_MO_OPTION, last_mo, checkfcdbysite, checkbomosbysite, checkmacprebysite, tmpsn, temkp, temmodel, kprelation;
            bool REC;
            int i, iCapacity, TRACK_QTY, bom_count_all, link_count_all, bom_count_kp, link_count_kp, ssn_start;

            try
            {
                if (string.IsNullOrEmpty(Edt_linename.Text.ToString()))
                {
                    lbError.Text = "00673 - " + await GetPubMessageVN("00673");
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "00673 - " + await GetPubMessageVN("00673");
                    _er.MessageEnglish = "00673 - " + await GetPubMessage("00673");
                    _er.ShowDialog();
                    return;
                }
                if (strDataInput == "UNDO")
                {
                    CHECKSN_OK = false;
                    editSerialNumber.Text = "";
                    for (i = 1; i <= 15;i++)
                    {
                        sLOAD_SSN[i] = new TSSN_RULE();
                        sLOAD_SSN[i].SSN_OK = false;
                        sLOAD_SSN[i].SSN = "";

                        sLOAD_MAC[i] = new TMAC_RULE();
                        sLOAD_MAC[i].MAC_OK = false;
                        sLOAD_MAC[i].MAC = "";
                    }
                    Data_gridView.ItemsSource = null;
                    sequenlist.Clear();
                    Next_Step = "";
                    strDataInput = "";
                    Input_Monumber.IsEnabled = false;
                    Inputdata.Focus();
                    lbError.Text = "00230 - " + await GetPubMessageVN("00230");
                    return;
                }
                if (strDataInput == "CLEAR")
                {
                    CHECKSN_OK = false;
                    editSerialNumber.Text = "";
                    Data_gridView.ItemsSource = null;
                    Next_Step = "";
                    strDataInput = "";
                    Inputdata.IsEnabled = true;
                    Inputdata.Focus();
                    lbError.Text = "00024 - " + await GetPubMessageVN("00024");
                    return;
                }
                //------------------Add by Z : 2023/05/25 Apply for model just scan SN to BOX/TRAY, no check keypart
                if (item_NoKeyPart.IsChecked)
                {
                    string indata = "PCMAC:" + Edt_local.Text.Replace(":","").Replace("-","").Trim() + "|PCIP:" + Edt_IP.Text.Trim() + "|SN:" + strDataInput;
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.SP_PackingBoxID",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="AP_VER",Value=ap_version,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="MYGROUP",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="LINE",Value=M_sThisLineName,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_EMP",Value=empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_FUNC",Value="SN EXECUTE",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=indata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                    });
                    dynamic _ads = result.Data;
                    string _RES = _ads[1]["res"];
                    if (_RES.StartsWith("OK"))
                    {
                        string strOut = _ads[0]["out_data"];
                        if (_RES=="OK,Print label")
                        {
                            try
                            {
                                lb_Count.Content = (int.Parse(lb_Count.Content.ToString()) + 1).ToString();
                            }
                            catch { }
                            await zPrintBox(strOut, strDataInput);
                        }
                        else
                        {
                            Edt_model.Text = strOut.Split('|')[0].ToString();
                            Edt_monumber.Text = strOut.Split('|')[1].ToString();
                            Edt_section.Text = "-";
                            Edt_group.Text = "-";
                            Edt_station.Text = "-";
                            Edt_BOXID.Text = strOut.Split('|')[2].ToString();
                            lb_Capacity.Content = strOut.Split('|')[3].ToString();
                            lb_Count.Content = strOut.Split('|')[4].ToString();
                        }
                        lbError.Text = _RES;
                        btn_CloseBoxID.IsEnabled = true;
                    }
                    else
                    {
                        showMessage(_RES);
                    }
                    sendtoCom(_RES);
                    return;
                }
                //------------------End by Z : 2023/05/25

                if(item_A_sn.IsChecked && Next_Step == "")
                {
                    strDataInput ="@"+ strDataInput;
                }

                // USE TRAY_NO TO PACK BOX --2013/11/11-- LANMIAO
                string strGetData = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='{strDataInput}' OR SHIPPING_SN='{strDataInput}' OR TRAY_NO='{strDataInput}' AND ROWNUM=1";
                var qry_DataR107 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData,
                });
                MODEL_TYPE = "";
                if (qry_DataR107.Data != null && Next_Step=="")
                {
                    editSerialNumber.Text = qry_DataR107.Data["serial_number"].ToString();
                    string strGetMoType = "SELECT A.MODEL_TYPE,B.SITE  FROM SFIS1.C_MODEL_DESC_T A,SFIS1.C_MODEL_SITE_T B"
                        + $" WHERE A.MODEL_NAME=B.MODEL_NAME(+) AND A.MODEL_NAME='{qry_DataR107.Data["model_name"].ToString()}'";
                    var qry_MoType = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetMoType,
                    });
                    MODEL_TYPE = qry_MoType.Data["model_type"].ToString();
                    Mmodel_type = MODEL_TYPE;
                }
                if (qry_DataR107.Data == null && Next_Step == "")
                {
                    SN_OK = false;
                    showMessage("SN/SSN not found|Không tìm thấy SN/SSN");
                    return;
                }

                if ((MODEL_TYPE.IndexOf("190") > -1) && (item_autotray.IsChecked == false))
                {
                    //Modeltype 199 auto tray
                    TRAY_STR = strDataInput;
                    string strGetTrayNO = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='{TRAY_STR}' OR TRACK_NO='{TRAY_STR}' or serial_number ='{TRAY_STR}' AND ROWNUM=1";
                    var qry_TrayNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetTrayNO,
                    });
                    if (qry_TrayNO.Data == null)
                    {
                        lbError.Text = "40001 - " + await GetPubMessageVN("40001");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "40001 - " + await GetPubMessageVN("40001");
                        _er.MessageEnglish = "40001 - " + await GetPubMessage("40001");
                        _er.ShowDialog();
                        Inputdata.Focus();
                        return;
                    }
                    else
                    {
                        TMP_SN = qry_TrayNO.Data["serial_number"].ToString();
                        TMPMODEL = qry_TrayNO.Data["model_name"].ToString();
                        smodel_name = TMPMODEL;
                        Err_Msg = await CheckTRAY(TRAY_STR, CHECKTRAY_OK);
                        string strGetRouteCode = "select * from sfis1.c_route_control_t where group_name='VACUUM' AND"
                            + $" ROUTE_CODE IN(SELECT SPECIAL_ROUTE FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='{TMP_SN}')";
                        var qry_RouteCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetRouteCode,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_RouteCode.Data != null)
                        {
                            string strGetControlTime = $"select type_desc/24 Control_time from sfis1.c_model_confirm_t where  model_type='CHECKTIME' AND TYPE_NAME='{TMPMODEL}'";
                            var qry_ControlTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetControlTime,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_ControlTime.Data != null)
                            {
                                ControlTime = qry_ControlTime.Data["control_time"].ToString();
                                string strGetOutTime = $"SELECT ('{ControlTime}'-(SYSDATE-OUT_LINE_TIME))*24*60 C_TIME FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='{TMP_SN}')";
                                var qry_OutTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetOutTime,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                OutTime = qry_OutTime.Data["c_time"].ToString();
                                if (Int32.Parse(OutTime) < 0)
                                {
                                    OutTime = OutTime.Substring(1, OutTime.Length - 1);
                                    lbError.Text = OutTime + "Error";
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = OutTime + "Error";
                                    _er.MessageEnglish = OutTime + "Error";
                                    _er.ShowDialog();
                                    return;
                                }
                                else
                                {
                                    lbError.Text = OutTime + "Error";
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = OutTime + "Error";
                                    _er.MessageEnglish = OutTime + "Error";
                                    _er.ShowDialog();
                                    return;
                                }
                            }
                        }
                        //editSerialNumber.Text = strDataInput;
                        if (CHECKTRAY_OK == true)
                        {
                            if (Int32.Parse(lb_Capacity.Content.ToString()) == Int32.Parse(lb_Count.Content.ToString()))
                            {
                                lb_Count.Content = "0";
                            }
                            if (Int32.Parse(lb_Count.Content.ToString()) == 0)
                            {
                                editSerialNumber.Text = TMP_SN;
                                if (await FGetBoxIDBySP(MBOXID) == false)
                                {
                                    lbError.Text = "40001 - " + await GetPubMessageVN("40001") + "-" + "00084 - " + await GetPubMessageVN("00084");
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "40001 - " + await GetPubMessage("40001") + "-" + "00084 - " + await GetPubMessage("00084");
                                    _er.MessageVietNam = "40001 - " + await GetPubMessageVN("40001") + "-" + "00084 - " + await GetPubMessageVN("00084");
                                    _er.ShowDialog();
                                    return;
                                }
                                else
                                {
                                    Edt_BOXID.Text = MBOXID;
                                }
                            }
                            REC = true;
                            //editSerialNumber.Text = TRAY_STR;
                            try
                            {
                                iCapacity = 0;
                                strDataInput = TRAY_STR;
                                if ((strDataInput != "") && (REC == true))
                                {
                                    lst_ListBox1.Items.Add(strDataInput);
                                }
                                if (item_autotray.IsChecked == true)
                                {
                                    sQty = 1;
                                }
                                else
                                {
                                    var qry_sQty = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = $"SELECT COUNT(*) qty FROM SFISM4.R107 WHERE tray_no='{TRAY_STR}'",
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    sQty = Int32.Parse(qry_sQty.Data["qty"].ToString());
                                }
                                M_iTrayCount = sQty;
                                sQty = Int32.Parse(lb_Count.Content.ToString()) + M_iTrayCount;
                                if (sQty > await getCapacity())
                                {
                                    lst_ListBox1.Items.Remove(lst_ListBox1.Items.IndexOf(TRAY_STR));
                                    lbError.Text = "00185 - " + await GetPubMessageVN("00185");
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "00185 - " + await GetPubMessage("00185");
                                    _er.MessageVietNam = "00185 - " + await GetPubMessageVN("00185");
                                    return;
                                }
                                else
                                {
                                    lb_Count.Content = sQty.ToString();
                                }

                                string strGetCount = $"SELECT COUNT(*) T_QTY FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='{TRAY_STR}'";
                                var qry_strGetCount = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetCount,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                TRACK_QTY = Int32.Parse(qry_strGetCount.Data["track_qty"].ToString());

                                try
                                {
                                    string strUpdate = $"UPDATE sfis1.c_pallet_t SET QTY=QTY+:TRACK_QTY  where pallet_no = '{MBOXID}' and close_flag = 'BOXID'";
                                    var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strUpdate,
                                        SfcCommandType = SfcCommandType.Text,
                                        SfcParameters = new List<SfcParameter>()
                                        {
                                            new SfcParameter{Name="TRACK_QTY",Value=TRACK_QTY}
                                        }
                                    });
                                    await updateR107();
                                    await InsertR102ByTray(TRACK_QTY);
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                
                                //await callTestInput();
                                lbError.Text = "00135 - " + await GetPubMessage("00135");
                                if (sQty == await getCapacity())
                                {
                                    await CloseBOXID();
                                }
                                editSerialNumber.Text = "";
                                strDataInput = "";
                            }
                            catch
                            {
                                lbError.Text = await GetPubMessageVN("00042");
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = await GetPubMessageVN("00042");
                                _er.MessageEnglish = await GetPubMessage("00042");
                                _er.ShowDialog();
                                strDataInput = "";
                                Inputdata.Focus();
                                return;
                            }
                            await afterProcess();
                            CHECKTRAY_OK = false;
                            editSerialNumber.Text = "";
                            sequenlist.Clear();
                            Next_Step = "";
                            Data_gridView.ItemsSource = null;
                            strDataInput = "";
                            Inputdata.Focus();
                        }
                    }
                }
                else
                {
                    if (item_Assy_input.IsChecked == false)
                    {
                        REC = false;
                        if (!string.IsNullOrEmpty(Edt_model.Text.ToString()))
                        {
                            if (editSerialNumber.Text == "")
                            {
                                editSerialNumber.Text = strDataInput;
                            }
                            string strGetSN = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE   SERIAL_NUMBER='{editSerialNumber.Text}' AND ROWNUM=1";
                            var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetSN,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_SN.Data != null)
                            {
                                t_version_code = qry_SN.Data["version_code"].ToString();
                                t_mo_number = qry_SN.Data["mo_number"].ToString();
                                if (Edt_model.Text != qry_SN.Data["model_name"].ToString())
                                {
                                    lbError.Text = "00108 - " + await GetPubMessageVN("00108");
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = "00108 - " + await GetPubMessageVN("00108");
                                    _er.MessageEnglish = "00108 - " + await GetPubMessage("00108");
                                    _er.ShowDialog();
                                    return;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(Edt_BOXID.Text.ToString()))
                        {
                            string strGetTrayNO = $"select *from sfism4.r_wip_tracking_t where tray_no='{Edt_BOXID.Text}' and rownum=1";
                            var qry_TrayNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetTrayNO,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_TrayNO.Data != null)
                            {
                                if (qry_TrayNO.Data["version_code"].ToString() != t_version_code)
                                {
                                    lbError.Text = "00108 - " + await GetPubMessageVN("00108");
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = "00108 - " + await GetPubMessageVN("00108");
                                    _er.MessageEnglish = "00108 - " + await GetPubMessage("00108");
                                    _er.ShowDialog();
                                    return;
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(Edt_monumber.Text))
                        {
                            CHECKMOTYPE();
                            if (Edt_monumber.Text != t_mo_number)
                            {
                                if (IsPackingByMoElseByModel == true)
                                {
                                    lbError.Text = "00402 - " + await GetPubMessageVN("00402");
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageVietNam = "00402 - " + await GetPubMessageVN("00402");
                                    _er.MessageEnglish = "00402 - " + await GetPubMessage("00402");
                                    _er.ShowDialog();
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        //BEGIN//UPDATE BY HWC//20140630
                        if ((strDataInput.Length == 19) && (strDataInput.Substring(12,1) == "-"))
                        {
                            strDataInput = strDataInput.Substring(0, 12);
                        }
                        string strGetDataWIP = $"select * from sfism4.r_wip_tracking_t where  serial_number='{strDataInput}' and rownum=1";
                        var qry_DataWIP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetDataWIP,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_DataWIP.Data != null)
                        {
                            last_mo = qry_DataWIP.Data["mo_number"].ToString();
                            string strGetDataMO = $"select * from sfism4.r_mo_base_t where  mo_number='{last_mo}' and rownum=1";
                            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetDataMO,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_DataMO.Data != null)
                            {
                                List<R105> result_r105 = new List<R105>();
                                result_r105 = qry_DataMO.Data.ToListObject<R105>().ToList();
                                MSN_MO_OPTION = result_r105[0].MSN_MO_OPTION;
                                if (!string.IsNullOrEmpty(MSN_MO_OPTION))
                                {
                                    if (MSN_MO_OPTION != Input_Monumber.Text)
                                    {
                                        lbError.Text = last_mo + " - " + MSN_MO_OPTION;
                                        Inputdata.Focus();
                                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                        _er.CustomFlag = true;
                                        _er.MessageEnglish = last_mo + " - "+ MSN_MO_OPTION;
                                        _er.MessageVietNam = last_mo + " - " + MSN_MO_OPTION;
                                        _er.ShowDialog();
                                        return;
                                    }
                                }
                                else
                                {
                                    string strGetMoOption = $"select * from sfism4.r_mo_base_t where  MSN_MO_OPTION='{Input_Monumber.Text.ToUpper()}' and rownum=1";
                                    var qry_MoOption = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetMoOption,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_MoOption.Data != null)
                                    {
                                        MSN_MO_OPTION = qry_MoOption.Data["mo_number"].ToString();
                                        lbError.Text = Input_Monumber.Text.ToUpper() + " - " + MSN_MO_OPTION;
                                        Inputdata.Focus();
                                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                        _er.CustomFlag = true;
                                        _er.MessageEnglish = Input_Monumber.Text.ToUpper() + " - " + MSN_MO_OPTION;
                                        _er.MessageVietNam = Input_Monumber.Text.ToUpper() + " - " + MSN_MO_OPTION;
                                        _er.ShowDialog();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    // LIEN SUA NGAY 2022-09-23 KIEM TRA TRUNG LAP TRONG CUST MAC1, MAC2
                    if (!await Check_dup_cust_sn(strDataInput))
                    {
                        return;
                    }
                    // LIEN SUA NGAY 2022-09-23 KIEM TRA TRUNG LAP TRONG CUST MAC1, MAC2
                    if (CHECKSN_OK == false)
                    {
                        Err_Msg = await CheckSN(strDataInput, CHECKSN_OK);
                        REC = true;
                        if (SN_OK == true)
                        {
                            sequenlist[SCAN_POS].SN = strDataInput;
                            Data_gridView.Items.Refresh();
                            CHECKSN_OK = true;
                            SCAN_POS = SCAN_POS + 1;
                            strDataInput = "";
                            Inputdata.Focus();
                            if (Int32.Parse(lb_Capacity.Content.ToString()) == (Int32.Parse(lb_Count.Content.ToString())))
                            {
                                lb_Count.Content = "0";
                            }
                            if (Int32.Parse(lb_Count.Content.ToString()) == 0)
                            {
                                if (await FGetBoxIDBySP(MBOXID) == false)
                                {
                                        //lbError.Text = await GetPubMessageVN("40001") + "||" + await GetPubMessageVN("00084");
                                    lbError.Text = "Create new BoxID fail";
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                        _er.CustomFlag = true;
                                        //_er.MessageEnglish = await GetPubMessage("40001") + "||" + await GetPubMessage("00084");
                                        //_er.MessageVietNam = await GetPubMessageVN("40001") + "||" + await GetPubMessageVN("00084");
                                    _er.MessageEnglish = "Create new BoxID fail";
                                    _er.MessageVietNam = "Tạo mã BoxID thất bại";
                                    _er.ShowDialog();
                                    //---------
                                    CHECKSN_OK = false;
                                    editSerialNumber.Text = "";
                                    Data_gridView.ItemsSource = null;
                                    Next_Step = "";
                                    strDataInput = "";
                                    Inputdata.IsEnabled = true;
                                    Inputdata.Focus();
                                    lbError.Text = "00024 - " + await GetPubMessageVN("00024");
                                    return;
                                    return;
                                }
                                else
                                {
                                    Edt_BOXID.Text = MBOXID;
                                }
                            }
                        }
                        else
                        {
                            lbError.Text = Err_Msg;
                            strDataInput = "";
                            Inputdata.Focus();
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = Err_Msg;
                            _er.MessageVietNam = Err_Msg;
                            _er.ShowDialog();
                            return;
                        }

                        if (M_SITE == "NOKIA")
                        {
                        }
                        else
                        {
                            //check mac prefix ,not match,can not pack together ---added by lanmiao 20131231
                            Err_Msg = await CheckMacPrefix(strDataInput, MBOXID, MacPrefix_OK);
                            if ((MacPrefix_OK == false) && (Mmodel_type.IndexOf("194") > -1))
                            {
                                lbError.Text = Err_Msg;
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = Err_Msg;
                                _er.MessageVietNam = Err_Msg;
                                _er.ShowDialog();
                                try
                                {
                                    CHECKSN_OK = false;
                                    MacPrefix_OK = false;
                                    for (i = 1; i < 12; i++)
                                    {
                                        sLOAD_SSN[i].SSN_OK = false;
                                        sLOAD_MAC[i].MAC_OK = false;
                                        sLOAD_SSN[i].SSN = "";
                                        sLOAD_MAC[i].MAC = "";
                                    }
                                    editSerialNumber.Text = "";
                                    Data_gridView.ItemsSource = null;
                                    sequenlist.Clear();
                                    strDataInput = "";
                                    Inputdata.Focus();
                                }
                                catch
                                {
                                    lbError.Text = "00042 - " + await GetPubMessageVN("00042");
                                    ShowMessageForm er = new ShowMessageForm(sfcClient);
                                    er.CustomFlag = true;
                                    er.MessageEnglish = "00042 - " + await GetPubMessage("00042");
                                    er.MessageVietNam = "00042 - " + await GetPubMessageVN("00042");
                                    er.ShowDialog();
                                    strDataInput = "";
                                    Inputdata.Focus();
                                    return;
                                }
                            }

                            FCD_OK = true;
                            Err_Msg = await CheckFCD(strDataInput, MBOXID, FCD_OK);
                            if (FCD_OK == false)
                            {
                                lbError.Text = Err_Msg;
                                ShowMessageForm er = new ShowMessageForm(sfcClient);
                                er.CustomFlag = true;
                                er.MessageEnglish = Err_Msg;
                                er.MessageVietNam = Err_Msg;
                                er.ShowDialog();
                                try
                                {
                                    CHECKSN_OK = false;
                                    BomAndOSFW_OK = false;
                                    for (i = 1; i <= 12; i++)
                                    {
                                        sLOAD_SSN[i].SSN_OK = false;
                                        sLOAD_MAC[i].MAC_OK = false;
                                        sLOAD_SSN[i].SSN = "";
                                        sLOAD_MAC[i].MAC = "";
                                    }
                                    editSerialNumber.Text = "";
                                    Data_gridView.ItemsSource = null;
                                    sequenlist.Clear();
                                    strDataInput = "";
                                    Inputdata.Focus();
                                }
                                catch
                                {
                                    lbError.Text = "00042 - " + await GetPubMessageVN("00042");
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "00042 - " + await GetPubMessage("00042");
                                    _er.MessageVietNam = "00042 - " + await GetPubMessageVN("00042");
                                    _er.ShowDialog();
                                    strDataInput = "";
                                    Inputdata.Focus();
                                    return;
                                }
                            }
                            //END  check FCD ,not match,can not pack together ---added by lanmiao 20140531
                        }
                    }
                    iCapacity = 0;
                    if (item_Assy_input.IsChecked == true)
                    {
                        //20160324
                        if (lb_Capacity.Content.ToString() == "1")
                        {
                            if ((Next_Step.IndexOf("MAC") > -1) || ((strDataInput.Length > 9) && (strDataInput.Substring(0,1) == "K")))
                            {
                                if (strDataInput.Length == 22)
                                {
                                    temptsn = strDataInput.Substring(0, 12);
                                }
                                else
                                {
                                    temptsn = strDataInput;
                                }

                                if ((strDataInput.Length == 19) && (strDataInput.Substring(12,1) != "-"))
                                {
                                    temptsn = strDataInput.Substring(0, 9);
                                }

                                string strGetKeyPartNO = "select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' AND key_part_sn='" + temptsn + "' and rownum=1";
                                var qry_KeyPartNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetKeyPartNO,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_KeyPartNO.Data != null)
                                {
                                    temptsn = qry_KeyPartNO.Data["serial_number"].ToString();
                                }

                                string strGetCustSN = "SELECT * FROM SFISM4.R_custsn_t where serial_number ='" + temptsn + "'";
                                var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetCustSN,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                List<R_CUSTSN> result_rcustsn = new List<R_CUSTSN>();
                                result_rcustsn = qry_CustSN.Data.ToListObject<R_CUSTSN>().ToList();
                                if (qry_CustSN.Data.Count() == 0)
                                {
                                    lbError.Text = "50040 - " + await GetPubMessageVN("50040") + "-" + temptsn;
                                    Inputdata.Focus();
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "50040 - " + await GetPubMessage("50040") + "-" + temptsn;
                                    _er.MessageVietNam = "50040 - " + await GetPubMessageVN("50040") + "-" + temptsn;
                                    _er.ShowDialog();
                                    return;
                                }
                                else
                                {
                                    if ((editSerialNumber.Text.Substring(0,strDataInput.Length) == strDataInput) && lb_Count.Content.ToString() == "0")
                                    {
                                        Edt_Old_FCD.Text = result_rcustsn[0].SSN10;
                                        Edt_Old_Bom.Text = result_rcustsn[0].SSN11;
                                        Edt_Old_Os.Text = result_rcustsn[0].SSN12;
                                    }
                                    else
                                    {
                                        Edt_New_FCD.Text = result_rcustsn[0].SSN10;
                                        Edt_New_Bom.Text = result_rcustsn[0].SSN11;
                                        Edt_New_Os.Text = result_rcustsn[0].SSN12;

                                        string strGetParameter = "SELECT * FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME='PACK_BOX' AND  VR_CLASS='UBNT' AND VR_NAME='CHECK MAC PREFIX/BOM/OSFW/FCD'";
                                        var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strGetParameter,
                                            SfcCommandType = SfcCommandType.Text
                                        });
                                        if (qry_Parameter.Data != null)
                                        {
                                            checkfcdbysite = qry_Parameter.Data["vr_item"].ToString();
                                            checkbomosbysite = qry_Parameter.Data["vr_desc"].ToString();
                                            checkmacprebysite = qry_Parameter.Data["vr_value"].ToString();
                                        }
                                        else
                                        {
                                            checkfcdbysite = "Y";
                                            checkbomosbysite = "Y";
                                            checkmacprebysite = "Y";
                                        }

                                        string strGetParameterINI = "SELECT * FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='UBNT'  AND VR_NAME='" + Edt_model.Text + "'";
                                        var qry_ParameterINI = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strGetParameterINI,
                                            SfcCommandType = SfcCommandType.Text
                                        });
                                        if (qry_ParameterINI.Data != null)
                                        {
                                            checkfcdver = qry_ParameterINI.Data["vr_item"].ToString();
                                            checkbomosver = qry_ParameterINI.Data["vr_desc"].ToString();
                                            checkmacprestr = qry_ParameterINI.Data["vr_value"].ToString();
                                        }
                                        else
                                        {
                                            checkfcdver = "Y";
                                            checkbomosver = "Y";
                                            checkmacprestr = "Y";
                                        }

                                        if (checkfcdbysite == "Y")
                                        {
                                            if (checkfcdver == "Y")
                                            {
                                                if (Edt_Old_FCD.Text != Edt_New_FCD.Text)
                                                {
                                                    lbError.Text = "00687 - " + await GetPubMessageVN("00687") + "-" + temptsn;
                                                    Inputdata.Focus();
                                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                    _er.CustomFlag = true;
                                                    _er.MessageEnglish = "00687 - " + await GetPubMessage("00687") + "-" + temptsn;
                                                    _er.MessageVietNam = "00687 - " + await GetPubMessageVN("00687") + "-" + temptsn;
                                                    _er.ShowDialog();
                                                    return;
                                                }
                                            }
                                        }

                                        if (checkbomosbysite == "Y")
                                        {
                                            if (checkbomosver == "Y")
                                            {
                                                if (Edt_Old_Bom.Text != Edt_New_Bom.Text)
                                                {
                                                    lbError.Text = "00685 - " + await GetPubMessageVN("00685") + "-" + temptsn;
                                                    Inputdata.Focus();
                                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                    _er.CustomFlag = true;
                                                    _er.MessageEnglish = "00685 - " + await GetPubMessage("00685") + "-" + temptsn;
                                                    _er.MessageVietNam = "00685 - " + await GetPubMessageVN("00685") + "-" + temptsn;
                                                    _er.ShowDialog();
                                                    return;
                                                }
                                                if (Edt_Old_Os.Text != Edt_New_Os.Text)
                                                {
                                                    lbError.Text = "00685 - " + await GetPubMessageVN("00686") + "-" + temptsn;
                                                    Inputdata.Focus();
                                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                    _er.CustomFlag = true;
                                                    _er.MessageEnglish = "00685 - " + await GetPubMessage("00686") + "-" + temptsn;
                                                    _er.MessageVietNam = "00685 - " + await GetPubMessageVN("00686") + "-" + temptsn;
                                                    _er.ShowDialog();
                                                    return;
                                                }
                                            }
                                        }

                                        if (checkmacprebysite == "Y")
                                        {
                                            if (checkmacprestr == "Y")
                                            {
                                                string strGetDataPrefix = "SELECT DISTINCT SUBSTR(KEY_PART_SN,1,6) MACPREFIX FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_NO='MACID' AND  SERIAL_NUMBER IN ("
                                                    + " SELECT KEY_PART_SN FROM SFISM4.R108 WHERE SERIAL_NUMBER ='" + editSerialNumber.Text + "' )";
                                                var qry_DataPrefix = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                                {
                                                    CommandText = strGetDataPrefix,
                                                    SfcCommandType = SfcCommandType.Text
                                                });
                                                List<R108> result_keypart = new List<R108>();
                                                result_keypart = qry_DataPrefix.Data.ToListObject<R108>().ToList();
                                                if ((qry_DataPrefix.Data.Count() > 1) || (result_keypart[0].MACPREFIX.Substring(0,6) != (strDataInput.Substring(0, 6))))
                                                {
                                                    lbError.Text = "00682 - " + await GetPubMessageVN("00682") + "-" + strDataInput;
                                                    Inputdata.Focus();
                                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                    _er.CustomFlag = true;
                                                    _er.MessageEnglish = "00682 - " + await GetPubMessage("00682") + "-" + strDataInput;
                                                    _er.MessageVietNam = "00682 - " + await GetPubMessageVN("00682") + "-" + strDataInput;
                                                    _er.ShowDialog();
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        string strGetKeyPartSN = $"select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' and key_part_sn='{strDataInput}' and rownum=1";
                        var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKeyPartSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_KeyPartSN.Data != null)
                        {
                            tmpsn = qry_KeyPartSN.Data["serial_number"].ToString();

                            string strGetKeyPartMACID = $"select * from sfism4.r_wip_keyparts_t where key_part_no<>'MACID' AND key_part_sn='{tmpsn}' and rownum=1";
                            var qry_KeyPartMACID = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetKeyPartMACID,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_KeyPartMACID.Data != null)
                            {
                                if (qry_KeyPartMACID.Data["serial_number"].ToString() != editSerialNumber.Text)
                                {
                                    lbError.Text = "00677 - " + await GetPubMessageVN("00677") + "-" + qry_KeyPartMACID.Data["serial_number"].ToString();
                                    Inputdata.Focus();
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "00677 - " + await GetPubMessage("00677") + "-" + qry_KeyPartMACID.Data["serial_number"].ToString();
                                    _er.MessageVietNam = "00677 - " + await GetPubMessageVN("00677") + "-" + qry_KeyPartMACID.Data["serial_number"].ToString();
                                    _er.ShowDialog();
                                    return;
                                }
                                else
                                {
                                    if (Next_Step.IndexOf("MAC1") == -1)
                                    {
                                        lbError.Text = "00677 - " + await GetPubMessage("00677") + "-" + qry_KeyPartMACID.Data["serial_number"].ToString();
                                        Inputdata.Focus();
                                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                        _er.CustomFlag = true;
                                        _er.MessageEnglish = "00677 - " + await GetPubMessage("00677") + "-" + qry_KeyPartMACID.Data["serial_number"].ToString();
                                        _er.MessageVietNam = "00677 - " + await GetPubMessageVN("00677") + "-" + qry_KeyPartMACID.Data["serial_number"].ToString();
                                        _er.ShowDialog();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                string srtGetDataWIP = $"select * from sfism4.r_wip_tracking_t where serial_number='{tmpsn}' and rownum=1";
                                var qry_DataWIP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = srtGetDataWIP,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_DataWIP.Data != null)
                                {
                                    temmodel = qry_DataWIP.Data["model_name"].ToString();
                                    if (qry_DataWIP.Data["wip_group"].ToString() == "STOCKIN")
                                    {
                                        temkp = qry_DataWIP.Data["model_name"].ToString();

                                        if (Edt_Keyparts.Text.IndexOf(temkp) > -1)
                                        {
                                            string strGetDataMO = $"select * from sfism4.r_mo_base_t where mo_number='{Edt_monumber.Text}' and rownum=1";
                                            var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strGetDataMO,
                                                SfcCommandType = SfcCommandType.Text
                                            });
                                            if (qry_DataMO.Data != null)
                                            {
                                                BOM_NO = qry_DataMO.Data["bom_no"].ToString();
                                            }

                                            string strGetKPCount = $"select sum(kp_count) kpcount from sfis1.c_bom_keypart_t where bom_no='{BOM_NO}' and group_name='{M_sThisGroup}'";
                                            var qry_KPCount = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strGetKPCount,
                                                SfcCommandType = SfcCommandType.Text
                                            });
                                            if (qry_KPCount.Data != null)
                                            {
                                                bom_count_all = Int32.Parse(qry_KPCount.Data["kpcount"].ToString());

                                                string strGetCountKP = $"select count(*) cout from sfism4.r_wip_keyparts_t where serial_number='{editSerialNumber.Text}' and group_name='{M_sThisGroup}'";
                                                var qry_CountKP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                                {
                                                    CommandText = strGetCountKP,
                                                    SfcCommandType = SfcCommandType.Text
                                                });
                                                link_count_all = Int32.Parse(qry_CountKP.Data["cout"].ToString());

                                                if ((bom_count_all >= 2)  && (bom_count_all > link_count_all))
                                                {
                                                    if (link_count_all != 0)
                                                    {
                                                        if ((link_count_all == 1) && (Next_Step.IndexOf("MAC1") > -1))
                                                        {
                                                            string strGetKeyPartNO = "select * from sfism4.r_wip_keyparts_t where key_part_NO='MACID' AND  serial_number in ( select key_part_sn from sfism4.r_wip_keyparts_t where serial_number=:serial_number and group_name=:group_name )";
                                                            var qry_KeyPartNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                                            {
                                                                CommandText = strGetKeyPartNO,
                                                                SfcCommandType = SfcCommandType.Text,
                                                                SfcParameters = new List<SfcParameter>()
                                                                {
                                                                    new SfcParameter{Name="serial_number",Value= editSerialNumber.Text},
                                                                    new SfcParameter{Name="group_name",Value=M_sThisGroup}
                                                                }
                                                            });
                                                            if (qry_KeyPartNO.Data["key_part_sn"].ToString() != strDataInput)
                                                            {
                                                                lbError.Text = "MAC NOT MATCH HH!";
                                                                Inputdata.Focus();
                                                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                                _er.CustomFlag = true;
                                                                _er.MessageEnglish = "MAC NOT MATCH HH!";
                                                                _er.MessageVietNam = "MAC khong trung khop!";
                                                                _er.ShowDialog();
                                                                return;
                                                            }
                                                        }
                                                        string strGetBOMNO = $"select * from sfis1.c_bom_keypart_t where bom_no='{BOM_NO}' and group_name='{M_sThisGroup}' and key_part_no='{temkp}'";
                                                        var qry_BOMNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                                        {
                                                            CommandText = strGetBOMNO,
                                                            SfcCommandType = SfcCommandType.Text
                                                        });
                                                        if (qry_BOMNO.Data != null)
                                                        {
                                                            bom_count_kp = Int32.Parse(qry_BOMNO.Data["kp_count"].ToString());
                                                            kprelation = qry_BOMNO.Data["kp_relation"].ToString();

                                                            string strGetCount = $"select count(*) cout from sfism4.r_wip_keyparts_t where serial_number=:serial_number and group_name=:group_name and key_part_no=:temkp";
                                                            var qry_Count = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                                            {
                                                                CommandText = strGetCount,
                                                                SfcCommandType = SfcCommandType.Text,
                                                                SfcParameters = new List<SfcParameter>()
                                                                {
                                                                    new SfcParameter{Name="serial_number",Value= editSerialNumber.Text},
                                                                    new SfcParameter{Name="group_name",Value= M_sThisGroup},
                                                                    new SfcParameter{Name="temkp",Value=temkp}
                                                                }
                                                            });
                                                            link_count_kp = Int32.Parse(qry_Count.Data["cout"].ToString());
                                                            if (bom_count_kp > link_count_kp)
                                                            {
                                                                try
                                                                {
                                                                    string strInsertKeyPart = "Insert into SFISM4.R_WIP_KEYPARTS_T"
                                                                    + " (EMP_NO, SERIAL_NUMBER, KEY_PART_NO, KEY_PART_SN,"
                                                                    + " KP_RELATION, GROUP_NAME, CARTON_NO, WORK_TIME,"
                                                                    + " VERSION, KP_CODE, MO_NUMBER)"
                                                                    + " Values"
                                                                    + " (:EMP_NO, :SERIAL_NUMBER, :KEY_PART_NO, :KEY_PART_SN,"
                                                                    + " :KP_RELATION, :GROUP_NAME, :CARTON_NO, SYSDATE,"
                                                                    + " :VERSION, :KP_CODE, :MO_NUMBER)";
                                                                    var InsertKeyPart = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                                    {
                                                                        CommandText = strInsertKeyPart,
                                                                        SfcCommandType = SfcCommandType.Text,
                                                                        SfcParameters = new List<SfcParameter>()
                                                                        {
                                                                            new SfcParameter{Name="EMP_NO",Value= empNo},
                                                                            new SfcParameter{Name="SERIAL_NUMBER",Value= editSerialNumber.Text},
                                                                            new SfcParameter{Name="KEY_PART_NO",Value=temkp},
                                                                            new SfcParameter{Name="KEY_PART_SN",Value=tmpsn},
                                                                            new SfcParameter{Name="KP_RELATION",Value=kprelation},
                                                                            new SfcParameter{Name="GROUP_NAME",Value=M_sThisGroup},
                                                                            new SfcParameter{Name="CARTON_NO",Value="N/A"},
                                                                            new SfcParameter{Name="VERSION",Value="N/A"},
                                                                            new SfcParameter{Name="KP_CODE",Value="N/A"},
                                                                            new SfcParameter{Name="MO_NUMBER",Value=Input_Monumber.Text}
                                                                        }
                                                                    });

                                                                    string strUpdateShipNO = "update SFISM4.R_WIP_TRACKING_T set ship_no=:newssn where serial_number=:serial_number";
                                                                    var UpdateShipNO = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                                    {
                                                                        CommandText = strUpdateShipNO,
                                                                        SfcCommandType = SfcCommandType.Text,
                                                                        SfcParameters = new List<SfcParameter>()
                                                                        {
                                                                            new SfcParameter{Name="newssn",Value=editSerialNumber.Text},
                                                                            new SfcParameter{Name="serial_number",Value=tmpsn}
                                                                        }
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
                                                                lbError.Text = "90035 - " + await GetPubMessageVN("90035") + "-" + temkp;
                                                                Inputdata.Focus();
                                                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                                _er.CustomFlag = true;
                                                                _er.MessageEnglish = "90035 - " + await GetPubMessage("90035") + "-" + temkp;
                                                                _er.MessageVietNam = "90035 - " + await GetPubMessageVN("90035") + "-" + temkp;
                                                                _er.ShowDialog();
                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lbError.Text = "00498 - " + await GetPubMessageVN("00498") + "-" + tmpsn;
                                                        Inputdata.Focus();
                                                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                        _er.CustomFlag = true;
                                                        _er.MessageEnglish = "00498 - " + await GetPubMessage("00498") + "-" + tmpsn;
                                                        _er.MessageVietNam = "00498 - " + await GetPubMessageVN("00498") + "-" + tmpsn;
                                                        _er.ShowDialog();
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    string strGetKeyPartNO = $"select * from sfism4.r_wip_keyparts_t where serial_number='{tmpsn}' and key_part_no='MACID' AND ROWNUM=1";
                                                    var qry_KeyPartNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                                    {
                                                        CommandText = strGetKeyPartNO,
                                                        SfcCommandType = SfcCommandType.Text
                                                    });
                                                    if (qry_KeyPartNO.Data != null)
                                                    {
                                                        if (qry_KeyPartNO.Data["key_part_sn"].ToString() != strDataInput)
                                                        {
                                                            lbError.Text = "00329 - " + await GetPubMessageVN("00329") + "-" + tmpsn;
                                                            Inputdata.Focus();
                                                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                                            _er.CustomFlag = true;
                                                            _er.MessageEnglish = "00329 - " + await GetPubMessage("00329") + "-" + tmpsn;
                                                            _er.MessageVietNam = "00329 - " + await GetPubMessageVN("00329") + "-" + tmpsn;
                                                            _er.ShowDialog();
                                                            return;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            lbError.Text = "00499 - " + await GetPubMessageVN("00499") + "-" + tmpsn;
                                            Inputdata.Focus();
                                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                            _er.CustomFlag = true;
                                            _er.MessageEnglish = "00499 - " + await GetPubMessage("00499") + "-" + tmpsn;
                                            _er.MessageVietNam = "00499 - " + await GetPubMessageVN("00499") + "-" + tmpsn;
                                            _er.ShowDialog();
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        lbError.Text = "00022 - " + await GetPubMessageVN("00022") + "-" + tmpsn + "-" + qry_DataWIP.Data["wip_group"].ToString();
                                        Inputdata.Focus();
                                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                        _er.CustomFlag = true;
                                        _er.MessageEnglish = "00022 - " + await GetPubMessage("00022") + "-" + tmpsn + "-" + qry_DataWIP.Data["wip_group"].ToString();
                                        _er.MessageVietNam = "00022 - " + await GetPubMessageVN("00022") + "-" + tmpsn + "-" + qry_DataWIP.Data["wip_group"].ToString();
                                        _er.ShowDialog();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    if (Next_Step.IndexOf("SSN") > -1)
                    {
                        for (i = 1; i <= 12; i++)
                        {
                            ssn_start = Next_Step.IndexOf("_");
                            temp_Next_Step = Next_Step.Substring(ssn_start +1  , Next_Step.Length - ssn_start -1);

                            ssn_start = temp_Next_Step.IndexOf("_");
                            temp_Next_Step = temp_Next_Step.Substring(ssn_start +1 , temp_Next_Step.Length - ssn_start -1);

                            if ("SSN" + i.ToString().Trim() == temp_Next_Step)
                            {
                                break;
                            }
                        }

                        if (sLOAD_SSN[i].SSN_OK == false)
                        {
                            ExecuteResult exeRes = new ExecuteResult();
                            exeRes = await CheckSSN(strDataInput, "SSN" + i.ToString(),  sLOAD_SSN[i].SSN_OK);

                            if (exeRes.Status == true)
                            {
                                sequenlist[SCAN_POS].SN = strDataInput;
                                sLOAD_SSN[i].SSN = strDataInput;
                                SCAN_POS = SCAN_POS + 1;
                            }
                            else
                            {
                                lbError.Text = exeRes.Message;
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = exeRes.Message; 
                                _er.MessageVietNam = exeRes.Message;
                                _er.ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            if ((sLOAD_SSN[i].sSHIPPINGSN_CODE != "") && (strDataInput.Substring(0, (sLOAD_SSN[i].sSHIPPINGSN_CODE).Length) == sLOAD_SSN[i].sSHIPPINGSN_CODE))
                            {
                                strDataInput = strDataInput.Substring((sLOAD_SSN[i].sSHIPPINGSN_CODE).Length, strDataInput.Length);
                            }
                            if (sLOAD_SSN[i].SSN == strDataInput)
                            {
                                sequenlist[SCAN_POS].SN = strDataInput;
                            }
                            else
                            {
                                lbError.Text = "00371 - " + await GetPubMessageVN("00371");
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = "00371 - " + await GetPubMessage("00371");
                                _er.MessageVietNam = "00371 - " + await GetPubMessageVN("00371");
                                _er.ShowDialog();
                                return;
                            }
                        }
                    }
                    if (Next_Step.IndexOf("MAC") > -1)
                    {
                        for (i = 1; i <= 15; i++)
                        {
                            if (Next_Step.IndexOf("MAC" + i.ToString()) > -1)
                            {
                                MACSTRING = "MAC" + i.ToString();
                                break;
                            }
                        }
                        if (await Checksnchar() == false)
                        {
                            Inputdata.Focus();
                            return;
                        }
                        if (sLOAD_MAC[i].MAC_OK == false)
                        {
                            Err_Msg = await CheckMAC(strDataInput, "MAC" + i.ToString(), sLOAD_MAC[i].MAC_OK);
                            if (sLOAD_MAC[i].MAC_OK == true)
                            {
                                sequenlist[SCAN_POS].SN = strDataInput;
                                sLOAD_MAC[i].MAC = strDataInput;
                                SCAN_POS = SCAN_POS + 1;
                                sLOAD_MAC[i].MAC_OK = false;
                            }
                            else
                            {
                                lbError.Text = Err_Msg;
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = Err_Msg;
                                _er.MessageVietNam = Err_Msg;
                                _er.ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            if ((sLOAD_MAC[i].sMACID_CODE != "") && (strDataInput.Substring(0, (sLOAD_MAC[i].sMACID_CODE).Length) == sLOAD_MAC[i].sMACID_CODE))
                            {
                                strDataInput = strDataInput.Substring(sLOAD_MAC[i].sMACID_CODE.Length, strDataInput.Length);
                            }
                            if (sLOAD_MAC[i].MAC == strDataInput)
                            {
                                sequenlist[SCAN_POS].SN = strDataInput;
                            }
                            else
                            {
                                lbError.Text = "00329 - " + await GetPubMessageVN("00329");
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = "00329 - " + await GetPubMessage("00329");
                                _er.MessageVietNam = "00329 - " + await GetPubMessageVN("00329");
                                _er.ShowDialog();
                                return;
                            }
                        }
                    }
                    if (SCAN_POS < sequenlist.Count)
                    {
                        Next_Step = sequenlist[SCAN_POS].STEP;
                        Inputdata.Focus();
                        lbError.Text = "00410 - " + await GetPubMessageVN("00410") + " " + Next_Step;
                        Data_gridView.Items.Refresh();
                        return;
                    }
                    else
                    {
                        try
                        {
                            if (MBOXID != "" && MBOXID != null)
                            {
                                await processC_Pallet();
                            }
                            await updateR107();
                            await callTestInput();
                            lbError.Text = "00135 - " + await GetPubMessageVN("00135");
                            CHECKSN_OK = false;
                            for (i = 1; i <= 12; i++)
                            {
                                sLOAD_SSN[i].SSN_OK = false;
                                sLOAD_SSN[i].SSN = "";
                                sLOAD_MAC[i].MAC_OK = false;
                                sLOAD_MAC[i].MAC = "";
                            }
                           
                            sequenlist.Clear();
                            Next_Step = "";
                            Data_gridView.ItemsSource = null;
                            strDataInput = "";
                            Inputdata.Focus();
                        }
                        catch
                        {
                            lbError.Text = "00042 - " + await GetPubMessageVN("00042");
                            strDataInput = "";
                            Inputdata.Focus();
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "00042 - " + await GetPubMessage("00042");
                            _er.MessageVietNam = "00042 - " + await GetPubMessageVN("00042");
                            _er.ShowDialog();
                            return;
                        }
                        string strGetCountTrayNo = "";
                        if (MBOXID != "" && MBOXID != null)
                        {
                            strGetCountTrayNo = $"select count(*) qty from sfism4.r_wip_tracking_t where  tray_no='{MBOXID}'";
                        }
                        else
                        {
                            strGetCountTrayNo = $"select count(*) qty from sfism4.r_wip_tracking_t where  serial_number = '" + editSerialNumber.Text + "'";
                        }

                        var qry_CountTrayNo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCountTrayNo,
                            SfcCommandType = SfcCommandType.Text
                        });
                        sQty = Int32.Parse(qry_CountTrayNo.Data["qty"].ToString());

                        M_iTrayCount = sQty;
                        lb_Count.Content = M_iTrayCount.ToString();

                        iCapacity = await getCapacity();
                        if (MBOXID != "" && MBOXID != null)
                        {
                            if (sQty >= iCapacity)
                            {
                                await CloseBOXID();
                            }
                            await afterProcess();
                        }
                        
                        editSerialNumber.Text = "";

                        if (lb_Count.Content.ToString() == "0")
                        {
                            Edt_Old_Bom.Text = "";
                            Edt_Old_FCD.Text = "";
                            Edt_Old_Os.Text = "";
                            Edt_New_Bom.Text = "";
                            Edt_New_FCD.Text = "";
                            Edt_New_Os.Text = "";
                        }
                        
                    }

                    if (Edtmac.IsVisible == true && Edtmac.IsEnabled == true)
                    {
                        Edtmac.Focus();
                        Edtmac.SelectAll();
                        Inputdata.IsEnabled = true;
                    }
                    else
                    {
                        Inputdata.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                lbError.Text = ex.Message;
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = ex.Message;
                _er.MessageVietNam = ex.Message;
                _er.ShowDialog();
                return;
            }
            sendtoCom("OK");
        }
        
        private async Task Edmacinputdata()
        {
            string tmp_str, tmp_serial;

            if (Cb_custsn.IsChecked == true)
            {
                string strGetShippingSN = $"select * from sfism4.r_wip_tracking_t where shipping_sn = '{strDataInput}'";
                var qry_ShippingSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetShippingSN,
                    SfcCommandType = SfcCommandType.Text
                });

                if (Cb_Fortmac.IsChecked == true)
                {
                    string strGetKeyPartSN = "select * from sfism4.r_wip_keyparts_t"
                        + " where KEY_PART_SN =(select serial_number from sfism4.r_custsn_t where ssn5=:sSerN and rownum=1)";
                    var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPartSN,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="sSerN",Value=strDataInput}
                        }
                    });
                    if (qry_KeyPartSN.Data == null)
                    {
                        lbError.Text = "00311 - " + await GetPubMessageVN("00311");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00311 - " + await GetPubMessage("00311");
                        _er.MessageVietNam = "00311 - " + await GetPubMessageVN("00311");
                        _er.ShowDialog();
                        Inputdata.Focus();
                    }
                    else
                    {
                        Inputdata.Focus();
                        strDataInput = qry_KeyPartSN.Data["serial_number"].ToString();
                    }
                }
                else
                {
                    if (qry_ShippingSN.Data.Count() == 0)
                    {
                        string strGetCustSN = "select * from sfism4.r_custsn_t"
                        + " where SSN1 = :sSerN or ssn3= :sSerN and rownum=1";
                        var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCustSN,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="sSerN",Value=strDataInput}
                            }
                        });
                        if (qry_CustSN.Data.Count() == 0)
                        {
                            lbError.Text = "00311 - " + await GetPubMessageVN("00311");
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "00311 - " + await GetPubMessage("00311");
                            _er.MessageVietNam = "00311 - " + await GetPubMessageVN("00311");
                            _er.ShowDialog();
                            Inputdata.Focus();
                        }
                        else
                        {
                            List<R_CUSTSN> result_rcustsn = new List<R_CUSTSN>();
                            result_rcustsn = qry_CustSN.Data.ToListObject<R_CUSTSN>().ToList();
                            Inputdata.Focus();
                            strDataInput = result_rcustsn[0].SERIAL_NUMBER;
                        }
                    }
                    else
                    {
                        List<R107> result_r107 = new List<R107>();
                        result_r107 = qry_ShippingSN.Data.ToListObject<R107>().ToList();
                        Inputdata.Focus();
                        strDataInput = result_r107[0].SERIAL_NUMBER;
                    }
                }
            }
            else
            {
                if (strDataInput.Length != 12)
                {
                    string strGetKeyPartSN = "select * from sfism4.r_wip_keyparts_t"
                        + " where KEY_PART_SN = :sSerN";
                    var qry_KeyPartSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPartSN,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="sSerN",Value=strDataInput}
                        }
                    });
                    if (qry_KeyPartSN.Data.Count() == 1)
                    {
                        List<R108> result_r108 = new List<R108>();
                        result_r108 = qry_KeyPartSN.Data.ToListObject<R108>().ToList();
                        tmp_serial = result_r108[0].SERIAL_NUMBER;
                        Inputdata.Focus();
                        strDataInput = tmp_serial;
                    }
                    else
                    {
                        lbError.Text = "00502 - " + await GetPubMessageVN("00502");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00502 - " + await GetPubMessage("00502");
                        _er.MessageVietNam = "00502 - " + await GetPubMessageVN("00502");
                        _er.ShowDialog();
                        Inputdata.Focus();
                    }
                }
                else
                {
                    string strGetKeyPart = "select * from sfism4.r_wip_keyparts_t"
                       + $" where key_part_no='MACID' AND  KEY_PART_SN = '{strDataInput}'";
                    var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPart,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_KeyPart.Data.Count() == 0)
                    {
                        string strGetHKeyPart = "select * from sfism4.h_wip_keyparts_t"
                            + $" where KEY_PART_SN = '{strDataInput}'";
                        var qry_HKeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetHKeyPart,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_HKeyPart.Data.Count() == 0)
                        {
                            string strGetHKeyPart2 = "select * from sfism4.h_wip_keyparts_t"
                                + $" where KEY_PART_SN = '{strDataInput}'";
                            var qry_HKeyPart2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetHKeyPart2,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_HKeyPart2.Data.Count() == 0)
                            {
                                lbError.Text = "00502 - " + await GetPubMessageVN("00502");
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = "00502 - " + await GetPubMessage("00502");
                                _er.MessageVietNam = "00502 - " + await GetPubMessageVN("00502");
                                _er.ShowDialog();
                                Inputdata.Focus();
                            }
                            else if (qry_HKeyPart2.Data.Count() == 1)
                            {
                                List<H108> result_H108 = new List<H108>();
                                result_H108 = qry_HKeyPart2.Data.ToListObject<H108>().ToList();
                                tmp_str = result_H108[0].SERIAL_NUMBER;
                            }
                            else
                            {
                                lbError.Text = "00502 - " + await GetPubMessageVN("00502");
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = "00502 - " + await GetPubMessage("00502");
                                _er.MessageVietNam = "00502 - " + await GetPubMessageVN("00502");
                                _er.ShowDialog();
                                Inputdata.Focus();
                            }
                        }
                        List<H108> _H108 = new List<H108>();
                        _H108 = qry_HKeyPart.Data.ToListObject<H108>().ToList();
                        tmp_str = _H108[0].SERIAL_NUMBER;
                    }
                    else
                    {
                        List<R108> _R108 = new List<R108>();
                        _R108 = qry_KeyPart.Data.ToListObject<R108>().ToList();
                        tmp_str = _R108[0].SERIAL_NUMBER;
                    }

                    if (Cb_Fortmac.IsChecked == true)
                    {
                        string strGetKeyPartSN = "select * from sfism4.r_wip_keyparts_t"
                            + $" where KEY_PART_NO<>'MACID' AND  KEY_PART_SN = '{tmp_str}'";
                        var qry_KeyPartSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKeyPartSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_KeyPartSN.Data.Count() == 0)
                        {
                            string strGetKeyPartSN2 = "select * from sfism4.h_wip_keyparts_t"
                                + " where KEY_PART_SN = :sSerN";
                            var qry_KeyPartSN2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetKeyPartSN2,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="sSerN",Value=tmp_str}
                                }
                            });
                            if (qry_KeyPartSN2.Data.Count() == 0)
                            {
                                lbError.Text = "00502 - " + await GetPubMessageVN("00502");
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = "00502 - " + await GetPubMessage("00502");
                                _er.MessageVietNam = "00502 - " + await GetPubMessageVN("00502");
                                _er.ShowDialog();
                                Inputdata.Focus();
                            }
                            List<H108> _H108 = new List<H108>();
                            _H108 = qry_KeyPartSN2.Data.ToListObject<H108>().ToList();
                            tmp_serial = _H108[0].SERIAL_NUMBER;
                        }
                        else
                        {
                            List<R108> _r108 = new List<R108>();
                            _r108 = qry_KeyPartSN.Data.ToListObject<R108>().ToList();
                            tmp_serial = _r108[0].SERIAL_NUMBER;
                        }

                        string strGetKPSN = "select * from sfism4.r_wip_keyparts_t "
                               + " where KEY_PART_SN = :sSerN";
                        var qry_KPSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKPSN,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="sSerN",Value=tmp_serial}
                            }
                        });
                        if (qry_KPSN.Data.Count() != 0)
                        {
                            List<R108> _r108 = new List<R108>();
                            _r108 = qry_KPSN.Data.ToListObject<R108>().ToList();
                            tmp_serial = _r108[0].SERIAL_NUMBER;
                        }
                        Inputdata.Focus();
                        strDataInput = tmp_serial;
                    }
                    else
                    {
                        List<R108> _r108 = new List<R108>();
                        _r108 = qry_KeyPart.Data.ToListObject<R108>().ToList();
                        Inputdata.Focus();
                        strDataInput = _r108[0].SERIAL_NUMBER;
                    }
                }
            }
        }
        private async Task<string> CheckMAC(string SSN,string SSN_CODE,bool SSN_OK)
        {
            string CUST_SN, TEMP_SN, sIgnoreStr;
            int MAC_IDX, i, iLoop;

            CUST_SN = SSN.ToUpper();
            SCAN_MAC = CUST_SN;
            MACID = SMAC[1];
            TEMP_SN = editSerialNumber.Text;
            MAC_IDX = Int32.Parse(SSN_CODE.Substring(3));
            sIgnoreStr = sLOAD_MAC[MAC_IDX].sMACID_CODE;
            if ((sIgnoreStr!= "") && (CUST_SN.Substring(0,sIgnoreStr.Length) == sIgnoreStr))
            {
                CUST_SN = CUST_SN.Substring(sIgnoreStr.Length, CUST_SN.Length);
            }
            if (CUST_SN.Length != sLOAD_MAC[MAC_IDX].nMAC_LENGTH)
            {
                SSN_OK = false;
                await updateR107_hold2("3");
                return "40102 - " + await GetPubMessage("40102");
            }
            if (sLOAD_MAC[MAC_IDX].cMAC_PREFIX == "")
            {
                SSN_OK = false;
                return "20101 - " + await GetPubMessage("20101");
            }
            if ((sLOAD_MAC[MAC_IDX].cMAC_PREFIX != "") && sLOAD_MAC[MAC_IDX].cMAC_PREFIX.IndexOf(CUST_SN.Substring(0, sLOAD_MAC[MAC_IDX].nMAC_PREFIX_LEN)) == -1)
            {
                SSN_OK = false;
                return "20101 - " + await GetPubMessage("20101");
            }
            if ((CUST_SN.Substring(sLOAD_MAC[MAC_IDX].nMAC_LENGTH - sLOAD_MAC[MAC_IDX].nMAC_POSTFIX_LEN, sLOAD_MAC[MAC_IDX].nMAC_POSTFIX_LEN)) != sLOAD_MAC[MAC_IDX].cMAC_POSTFIX)
            {
                SSN_OK = false;
                return "00393 - " + await GetPubMessage("00393");
            }
            if (sLOAD_MAC[MAC_IDX].nMAC_PREFIX_LEN == 0)
            {
                for (i = 0; i < sLOAD_MAC[MAC_IDX].nMAC_LENGTH; i++)
                {
                    if (sLOAD_MAC[MAC_IDX].cMAC_STR.IndexOf(CUST_SN[i]) == -1)
                    {
                        SSN_OK = false;
                        return "00394 - " + await GetPubMessage("00394");
                    }
                }
            }
            else
            {
                for (i = sLOAD_MAC[MAC_IDX].nMAC_PREFIX_LEN - 1;i< sLOAD_MAC[MAC_IDX].nMAC_LENGTH;i++)
                {
                    if (sLOAD_MAC[MAC_IDX].cMAC_STR.IndexOf(CUST_SN[i]) == -1)
                    {
                        SSN_OK = false;
                        return "00394 - " + await GetPubMessage("00394");
                    }
                }
            }
            if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "ControlKey")
            {
                if (ControlKey(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "00395 - " + await GetPubMessage("00395");
                }
            }
            else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "IBMModulo10")
            {
                if (IBMModulo10(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "00390 - " + await GetPubMessage("00390");
                }
            }else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "ODDMAC")
            {
                if (ODDMAC(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "00396 - " + await GetPubMessage("00396");
                }
            }else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "EVENMAC")
            {
                if (EVENMAC(CUST_SN) == true)
                {
                    SSN_OK = false;
                    return "00396 - " + await GetPubMessage("00396");
                }
            }else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "NEXTMAC")
            {
                if (sLOAD_MAC[MAC_IDX - 1].MAC != "")
                {
                    if (NEXTMAC(sLOAD_MAC[MAC_IDX - 1].MAC, CUST_SN) == true)
                    {
                        SSN_OK = false;
                        return "00398 - " + await GetPubMessage("00398");
                    }
                }
                else
                {
                    SSN_OK = false;
                    return "00399 - " + await GetPubMessage("00399");
                }
            }else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC1")
            {
                MACID = SMAC[1];
            }else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC2")
            {
                MACID = SMAC[2];
            }else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC3")
            {
                MACID = SMAC[3];
            }else if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "FINDMAC4")
            {
                MACID = SMAC[4];
            }

            if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE != "")
            {
                SSN_OK = false;
                return "00391 - " + await GetPubMessage("00391");
            }
            for (iLoop = 1; i <= 5;i++)
            {
                if (CUST_SN == SMAC[iLoop])
                {
                    MACID_OK = true;
                }
            }
            if (sLOAD_MAC[MAC_IDX].sCHECK_MAC == "Y")
            {
                if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE != "NEXTMAC")
                {
                    if (MACID_OK == false)
                    {
                        SSN_OK = false;
                        return "00329 - " + await GetPubMessage("00329");
                    }
                }
            }

            // CHECK MAC ------20121210
            if (MACID_OK == false)
            {
                if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE == "")
                {

                }
                else
                {
                    if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RULE.Substring(0, 7) != "NEXTMAC")
                    {
                        if (CUST_SN.Substring(0, 3) == "23S")
                        {
                            if (CUST_SN.Substring(4, 12) != MACID)
                            {
                                SSN_OK = false;
                                return "00329 - " + await GetPubMessage("00329");
                            }
                        }
                        else
                        {
                            string strGetDataKP = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T a,sfism4.r_wip_tracking_t b ,SFIS1.C_BOM_KEYPART_T c"
                                + " where a.serial_number=b.serial_number and b.bom_no=c.bom_no AND C.KEY_PART_NO='MACID'"
                                + " and a.KEY_PART_NO = 'MACID' and a.serial_number=:SERIAL_NUMBER";
                            var qry_DataKP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetDataKP,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="SERIAL_NUMBER",Value=editSerialNumber.Text}
                                }
                            });
                            if (qry_DataKP.Data != null)
                            {
                                MACID = qry_DataKP.Data["key_part_sn"].ToString();
                                if (CUST_SN == MACID)
                                {
                                    MACID_OK = true;
                                }
                            }
                            else
                            {
                                string strGetDataKPSN = "select * from sfism4.r_wip_keyparts_t a,sfism4.r108 b where"
                                   + " a.serial_number=:serial_number and a.key_part_sn =b.serial_number"
                                   + " and b.key_part_sn=:macid";
                                var qry_DataKPSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetDataKPSN,
                                    SfcCommandType = SfcCommandType.Text,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter{Name="SERIAL_NUMBER",Value=editSerialNumber.Text},
                                        new SfcParameter{Name="macid",Value=CUST_SN}
                                    }
                                });
                                if (qry_DataKPSN.Data.Count() > 0)
                                {
                                    MACID_OK = true;
                                }
                            }
                            if (MACID_OK == false)
                            {
                                SSN_OK = true;
                                try
                                {
                                    updateR107_hold("2");
                                    lbError.Text = "00135 - " + await GetPubMessage("00135");
                                    CHECKSN_OK = false;
                                    for (i = 1; i <= 15; i++)
                                    {
                                        sLOAD_SSN[i].SSN_OK = false;
                                        sLOAD_MAC[i].MAC_OK = false;
                                        sLOAD_SSN[i].SSN = "";
                                        sLOAD_MAC[i].MAC = "";
                                    }
                                    editSerialNumber.Text = "";
                                    Data_gridView.ItemsSource = null;
                                    sequenlist.Clear();
                                    Next_Step = "";
                                }
                                catch
                                {
                                    strDataInput = "";
                                    Inputdata.Focus();
                                    lbError.Text = "00042 - " + await GetPubMessageVN("00042");
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "00042 - " + await GetPubMessage("00042");
                                    _er.MessageVietNam = "00042 - " + await GetPubMessageVN("00042");
                                    _er.ShowDialog();
                                }
                                return "00329 - " + await GetPubMessage("00329");
                            }
                        }
                    }
                }

                }
            if (sLOAD_MAC[MAC_IDX].sCOMPARE_MAC == "SN")
            {
                if ((CUST_SN.Substring(sLOAD_MAC[MAC_IDX].nMAC_Self_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Self_FlowNO)) != (editSerialNumber.Text.Substring(sLOAD_MAC[MAC_IDX].nMAC_Compare_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Compare_FlowNO)))
                {
                    SSN_OK = false;
                    return sequenlist[SCAN_POS + 1].STEP + " <> SN!!";
                }
            }
            if (sLOAD_MAC[MAC_IDX].sCOMPARE_MAC == "SSN1")
            {
                if ((CUST_SN.Substring(sLOAD_MAC[MAC_IDX].nMAC_Self_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Self_FlowNO)) != (editSerialNumber.Text.Substring(sLOAD_MAC[MAC_IDX].nMAC_Compare_StartDigit, sLOAD_MAC[MAC_IDX].nMAC_Compare_FlowNO)))
                {
                    SSN_OK = false;
                    return sequenlist[SCAN_POS + 1].STEP + " <> SSN1!!";
                }
            }
            if (sLOAD_MAC[MAC_IDX].sCHECK_MAC_RANGE == "Y")
            {
                string strGetRangeExt4 = "SELECT * FROM SFISM4.R_MO_EXT4_T"
                    + " WHERE '" + CUST_SN + "' <= item_2"
                    + " AND '" + CUST_SN + "' >=item_1"
                    + " AND MO_NUMBER = '" + Edt_monumber.Text + "'"
                    + " AND VER_3 = '" + MAC_IDX.ToString() + "'"
                    + " AND LENGTH('" + CUST_SN + "')=length(item_1)"
                    + " AND LENGTH('" + CUST_SN + "')=length(item_2)";
                var qry_RangeExt4 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetRangeExt4,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_RangeExt4.Data.Count() == 0)
                {
                    SSN_OK = false;
                    return "40108 - " + await GetPubMessage("40108");
                }
            }

            string strGetCustSN = "select * from SFISM4.R_CUSTSN_T"
                + " where serial_number= '" + editSerialNumber.Text + "'"
                + "  and MAC" + MAC_IDX.ToString() + " IS NOT NULL";
            var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustSN,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_CustSN.Data.Count() > 0)
            {
                string strGetCustSN2 = "select * from SFISM4.R_CUSTSN_T"
                   + " where serial_number= '" + editSerialNumber.Text + "'"
                   + "  and MAC" + MAC_IDX.ToString() + "='" + CUST_SN + "'";
                var qry_CustSN2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustSN2,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_CustSN2.Data.Count() == 0)
                {
                    SSN_OK = false;
                    return CUST_SN + " - 40114 - " + await GetPubMessage("40114");
                }
            }

            strDataInput = CUST_SN;
            sLOAD_MAC[MAC_IDX].MAC_OK = true;
            return "";
        }

        public async Task<DataTable> ExecuteSQL(string sql)
        {
            DataTable data;
            data = null;
            var datacust = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });

            if (datacust.Data != null)
            {
                var vardatatabel = JsonConvert.SerializeObject(datacust.Data);
                data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
            }
           return data;
        }

        private async Task<ExecuteResult> CheckSSN(string SSN,string SSN_CODE, bool SSN_OK)
        {
            ExecuteResult exeRes = new ExecuteResult();
            int SSN_IDX,i;
            string CUST_SN, sIgnoreStr, temgessn;
            CUST_SN = SSN;
            if (SSN_CODE.Length < 5)
            {
                SSN_IDX = Int32.Parse(SSN_CODE.Substring(3, 1));
            }
            else
            {
                SSN_IDX = Int32.Parse(SSN_CODE.Substring(3, 2));
            }
            sIgnoreStr = sLOAD_SSN[SSN_IDX].sSHIPPINGSN_CODE;
            string modelSerial = await getmodelserial();
            if (modelSerial == "NIC" || modelSerial == "SUPERCAP")
            {
                string sqlPN = string.Format(@"select broadcom_pn from SFIS1.C_BRCM_PN_T where model_name = '{0}'", smodel_name);
                var dataPN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlPN,
                    SfcCommandType = SfcCommandType.Text
                });

                if (dataPN.Data != null)
                {
                    string BROADCOM_PN = dataPN.Data["broadcom_pn"].ToString();
                    string BrcmPN_Temp = "";
                    if (modelSerial == "SUPERCAP")
                    {
                        BrcmPN_Temp = CUST_SN.Substring(0, CUST_SN.Length - 3);
                    }
                    else if (smodel_name.Equals("_"))
                    {
                        BrcmPN_Temp = CUST_SN.Substring(0, CUST_SN.Length - 3);
                    }
                    else
                    {
                        BrcmPN_Temp = CUST_SN.Substring(0, CUST_SN.Length - 2);
                    }
                    if (SSN_IDX.ToString() == "3")
                    {
                        if (BROADCOM_PN == BrcmPN_Temp)
                        {
                            SSN_OK = true;
                        }
                        else
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message = "Thiết Lập BRCM PN sai, tìm IE và PM!";
                        }
                        if (smodel_name.Equals("_"))
                        {
                            BrcmPN_Temp = CUST_SN.Substring(1, CUST_SN.Length - 3);
                        }

                    }
                    else
                    {
                        if (BROADCOM_PN == BrcmPN_Temp)
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message = "Chỉ có SSN3 mới sảo BRCM PN!";
                        }
                    }

                }
            }
            if ((sIgnoreStr != "") && (CUST_SN.Substring(0,sIgnoreStr.Length) == sIgnoreStr))
            {
                CUST_SN = CUST_SN.Substring(sIgnoreStr.Length, CUST_SN.Length);
            }
            if(await getmodelserial() != "ECD" && await getmodelserial() != "SUPERCAP")
            {
                if (((M_sSSN1 != "N/A") || (M_sSSN1 != "")) && (SSN_CODE != "SSN1"))
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message = editSerialNumber.Text + "-" + await GetPubMessage("00070") + "-" + M_sSSN1;
                }
            }
            // check dup cust:chua test Z
            string sqlcustdup = "SELECT*FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER ='"+ editSerialNumber.Text+ "'";
            DataTable dt =await ExecuteResult.ExecuteSQL(sqlcustdup, sfcClient);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows.Count > 1)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message = "Cust data(SN) is duplicate";
                    return exeRes;
                }
                if (dt.Rows.Count == 1 )
                {
                    string varssn = SSN_CODE.ToLower();
                    string vardatasncust =  dt.Rows[0][varssn].ToString();

                    if (CUST_SN != vardatasncust && vardatasncust != "" && vardatasncust != "N/A")
                    {
                        SSN_OK = false;
                        exeRes.Status = false;
                        exeRes.Message = CUST_SN + "<>" + vardatasncust + " plz check!";
                        return exeRes;
                    }
                }
            }
            if (CUST_SN.Length != (sLOAD_SSN[SSN_IDX].nSSN_LENGTH))
            {
                SSN_OK = false;
                exeRes.Status = false;
                exeRes.Message =  "00366 - " + await GetPubMessage("00366");
                return exeRes;
            }
            if (sLOAD_SSN[SSN_IDX].cSSN_PREFIX == "")
            {
                SSN_OK = false;
                exeRes.Status = false;
                exeRes.Message = "00387 - " + await GetPubMessage("00387");
            }
            if ((sLOAD_SSN[SSN_IDX].cSSN_PREFIX != "") && (sLOAD_SSN[SSN_IDX].cSSN_PREFIX.IndexOf(CUST_SN.Substring(0,sLOAD_SSN[SSN_IDX].nSSN_PREFIX_LEN)) == -1))
            {
                SSN_OK = false;
                exeRes.Status = false;
                exeRes.Message = "00387 - " + await GetPubMessage("00387");
            }
            if (CUST_SN.Substring(sLOAD_SSN[SSN_IDX].nSSN_LENGTH - sLOAD_SSN[SSN_IDX].nSSN_POSTFIX_LEN, sLOAD_SSN[SSN_IDX].nSSN_POSTFIX_LEN) != sLOAD_SSN[SSN_IDX].cSSN_POSTFIX)
            {
                SSN_OK = false;
                exeRes.Status = false;
                exeRes.Message ="00388 - " + await GetPubMessage("00388");
            }
            for (i = sLOAD_SSN[SSN_IDX].nSSN_PREFIX_LEN; i < sLOAD_SSN[SSN_IDX].nSSN_LENGTH - sLOAD_SSN[SSN_IDX].nSSN_POSTFIX_LEN; i++)
            {
                if (sLOAD_SSN[SSN_IDX].cSSN_STR.IndexOf(CUST_SN[i]) == -1)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message =  "00389 - " + await GetPubMessage("00389");
                }
            }
            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "ControlKey")
            {
                if (ControlKey(CUST_SN) == true)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message = await GetPubMessage("00390") + " (ControlKey)";
                }
            }
            else if(sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "IBMModulo10")
            {
                if (IBMModulo10(CUST_SN) == true)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message = "00390 - " + await GetPubMessage("00390") + " (IBMModulo10)";
                }
            }else if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "CHECKMODELSSN")
            {
                if (CHECKMODELSSN(sLOAD_SSN[SSN_IDX].cSSN_PREFIX, CUST_SN) == true)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message = "00390 - " + await GetPubMessage("00390") + " (CHECKMODELSSN)";
                }
            }else if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE != "")
            {
                SSN_OK = false;
                exeRes.Status = false;
                exeRes.Message = "00391 - " + await GetPubMessage("00391");
            }
            //top coment
            //if (SSN_IDX == 1)
            //{
            //    string strGetShippingSN = "select * from sfism4.r_wip_tracking_t"
            //        + $" where shipping_sn = '{CUST_SN}' AND ROWNUM=1";
            //    var qry_ShippingSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            //    {
            //        CommandText = strGetShippingSN,
            //        SfcCommandType = SfcCommandType.Text
            //    });
            //    if (qry_ShippingSN.Data.Count() > 0)
            //    {
            //        if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "CHECKMODELSSN")
            //        {
            //            SSN_OK = true;
            //        }
            //        else
            //        {
            //            SSN_OK = false;
            //            return "00375 - " + await GetPubMessage("00375") + "-" + CUST_SN;
            //        }
            //    }
            //}
            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN == "Y")
            {
                if (sMO_TYPE.ToUpper() == "RMA")
                {
                    if (CIS_SSN == "")
                    {
                        string strGetDataRMA = "select * from sfism4.R_RMA_NETSYS_T"
                            + $" where serial_number = '{editSerialNumber.Text}'";
                        var qry_DataRMA = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetDataRMA,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_DataRMA.Data != null)
                        {
                            exeRes.Status = false;
                            exeRes.Message = "00316 - " + await GetPubMessage("00316");
                        }
                        CIS_MAC = qry_DataRMA.Data["macid"].ToString();
                        CIS_SSN = qry_DataRMA.Data["ssn"].ToString();
                    }
                    if (CUST_SN != CIS_SSN)
                    {
                        SSN_OK = false;
                        exeRes.Status = false;
                        exeRes.Message =  "00371 - " + await GetPubMessage("00371") + "-" + CIS_SSN;
                    }
                }
                else
                {
                    if (SSN_IDX == 1)
                    {
                        string strGetDataWIP = "select * from sfism4.r_wip_tracking_t"
                            + $" where  serial_number= '{editSerialNumber.Text}' and po_no = '{CUST_SN}'";
                        var qry_DataWIP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetDataWIP,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_DataWIP.Data.Count() == 0)
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message =  "00392 - " + await GetPubMessage("00392");
                        }
                    }
                    else
                    {
                        string strGetCustSN = "select * from SFISM4.R_CUSTSN_T"
                            + " where serial_number= '" + editSerialNumber.Text + "'"
                            + " and SSN" + SSN_IDX.ToString() + " = '" + CUST_SN + "'";
                        var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCustSN,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_CustSN.Data.Count() == 0)
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message = "00392 - " + await GetPubMessage("00392");
                        }
                    }
                }
            }
            else
            {
                if (Mmodel_type.Equals("194"))
                {

                    string strGetCustSN = "select * from SFISM4.R_CUSTSN_T"
                           + " where  SSN" + SSN_IDX.ToString() + " = '" + CUST_SN + "' AND ROWNUM=1";
                    var qry_CustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if ((qry_CustSN.Data.Count() != 0) && (Mmodel_type.IndexOf("194") == -1))
                    {
                        if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE == "NOCHECKDUP")
                        {
                            SSN_OK = true;
                        }
                        else
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message = "00375 - " + await GetPubMessage("00375") + "-" + CUST_SN;
                        }
                    }
                }
                
            }

            string strGetCustSSN = "select * from SFISM4.R_CUSTSN_T"
                + " where serial_number= '" + editSerialNumber.Text + "'"
                + " and SSN" + SSN_IDX.ToString() + " IS NOT NULL";
            var qry_CustSSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustSSN,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_CustSSN.Data.Count() > 0)
            {
                string strGetCustSSN2 = "select * from SFISM4.R_CUSTSN_T"
                   + " where serial_number= '" + editSerialNumber.Text + "'"
                   + " and SSN" + SSN_IDX.ToString() + " = '" + CUST_SN + "'";
                var qry_CustSSN2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustSSN2,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_CustSSN2.Data.Count() == 0)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message =  CUST_SN + " - 00629 - " + await GetPubMessage("00629");
                }
            }
            if (sLOAD_SSN[SSN_IDX].sCOMPARE_SSN == "SN")
            {
                if (CUST_SN.Substring(sLOAD_SSN[SSN_IDX].nSSN_Self_StartDigit, sLOAD_SSN[SSN_IDX].nSSN_Self_FlowNO) != MACID.Substring(sLOAD_SSN[SSN_IDX].nSSN_Compare_StartDigit, sLOAD_SSN[SSN_IDX].nSSN_Compare_FlowNO))
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message =sequenlist[SCAN_POS].STEP + " != MAC1!!";
                }
            }
            if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RANGE == "Y")
            {
                string strGetRange = "SELECT * FROM SFISM4.R_MO_EXT3_T"
                    + " WHERE '" + CUST_SN + "' <= item_2"
                    + " AND '" + CUST_SN + "' >=item_1"
                    + " AND MO_NUMBER = '" + Edt_monumber.Text + "'"
                    //+ " AND VER_3 = " + SSN_IDX.ToString() + ""
                    + " AND LENGTH('" + CUST_SN + "')=length(item_1)"
                    + " AND LENGTH('" + CUST_SN + "')=length(item_2)";
                var qry_Range = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetRange,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Range.Data.Count() == 0)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message = "40308 - " + await GetPubMessage("40308");
                }
            }
            if (Mmodel_type.IndexOf("194") > -1)
            {
                if (SSN_IDX == 1)
                {
                    string strGetCustSSN3 = $"select * from SFISM4.R_CUSTSN_T SSN" + SSN_IDX.ToString() + " where SSN='{CUST_SN}' and ROWNUM=1";
                    var qry_CustSSN3 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSSN3,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CustSSN3.Data.Count() > 0)
                    {
                        SSN_OK = false;
                        exeRes.Status = false;
                        exeRes.Message =  "90013 - " + await GetPubMessage("90013");
                    }
                }
                if (sLOAD_SSN[SSN_IDX].sCHECK_SSN_RULE != "NOCHECKDUP")
                {
                    string strGetCustSSN4 = "select * from SFISM4.R_CUSTSN_T SSN" + SSN_IDX.ToString() + " where SSN=:tcust_sn AND  SERIAL_NUMBER<>:editNumber and ROWNUM=1";
                    var qry_CustSSN4 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSSN4,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name = "tcust_sn",Value=CUST_SN},
                            new SfcParameter{Name = "editNumber",Value=editSerialNumber.Text}
                        }
                    });
                    if (qry_CustSSN4.Data.Count() > 0)
                    {
                        SSN_OK = false;
                        exeRes.Status = false;
                        exeRes.Message =  "90013 - " + await GetPubMessage("90013");
                    }
                }
                if (sLOAD_SSN[SSN_IDX].sCOMPARE_SSN.Substring(0,2) == "T_")
                {
                    string strGetCustSNKP = "select * from SFISM4.R_CUSTSN_T where SERIAL_NUMBER=(SELECT KEY_PART_SN FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER=:editNumber and rownum=1 ) and ROWNUM=1";
                    var qry_CustSNKP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSNKP,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name = "editNumber",Value=editSerialNumber.Text}
                        }
                    });
                    if (qry_CustSNKP.Data == null)
                    {
                        SSN_OK = false;
                        exeRes.Status = false;
                        exeRes.Message = " 00311- " + await GetPubMessage("00311");
                    }
                    temgessn = qry_CustSNKP.Data[sLOAD_SSN[SSN_IDX].sCOMPARE_SSN.Substring(2, 5)].ToString();

                    string strGetCust = "select * from sfism4.r_custsn_t where "+ sLOAD_SSN[SSN_IDX].sCOMPARE_SSN + "=:tssn and rownum=1";
                    var qry_Cust = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCust,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name = "tssn",Value=CUST_SN}
                        }
                    });
                    if (qry_Cust.Data == null)
                    {
                        SSN_OK = false;
                        exeRes.Status = false;
                        exeRes.Message =  "00311 - " + await GetPubMessage("00311") +"-"+ sLOAD_SSN[SSN_IDX].sCOMPARE_SSN;
                    }
                    if (SCAN_POS == 1)
                    {
                        if (CUST_SN != temgessn)
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message = "00368 - " + await GetPubMessage("00368") + "-" + sLOAD_SSN[SSN_IDX].sCOMPARE_SSN;
                        }
                    }
                    else
                    {
                        if (CUST_SN == sequenlist[SCAN_POS].SN)
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message = "Has been input data--" + sequenlist[SCAN_POS].SN;
                        }
                        if (await CheckNOMAC(SSN_IDX,""))
                        {
                            exeRes.Status = false;
                            exeRes.Message = "";
                        }
                    }
                }
                else
                {
                    if (await CheckSSNEXT3(CUST_SN) == true)
                    {
                        SSN_OK = false;
                        exeRes.Status = false;
                        exeRes.Message = "00336 - " + await GetPubMessage("00336") + "--T_SSN (NO COMPARE_SSN)-" + sLOAD_SSN[SSN_IDX].sCOMPARE_SSN.Substring(2, 5);
                    }
                }
            }

            if (Mmodel_type.IndexOf("207") > -1)
            {
                if ((CUST_SN  == "609-00116-03") || (CUST_SN == "609-00114-03") || (CUST_SN == "609-00115-02") || (CUST_SN == "609-00113-02"))
                {
                    string strGetKPColor = $"SELECT * FROM SFISM4.R_KEYPART_COLOR_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}'";
                    var qry_KPColor = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKPColor,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_KPColor.Data == null)
                    {
                        string strGetKPColor2 = $"SELECT * FROM SFISM4.R_KEYPART_COLOR_T WHERE SERIAL_NUMBER IS NULL  AND KEYPART_NAME='{CUST_SN}' AND MO_NUMBER='{Edt_monumber.Text}'";
                        var qry_KPColor2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKPColor2,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_KPColor2.Data == null)
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message ="90033 - " + await GetPubMessage("90033");
                        }
                    }
                    else
                    {
                        if (qry_KPColor.Data["keypart_name"].ToString() != CUST_SN)
                        {
                            SSN_OK = false;
                            exeRes.Status = false;
                            exeRes.Message ="90034 - " + await GetPubMessage("90034") +"-"+ qry_KPColor.Data["keypart_name"].ToString();
                        }
                    }
                }
            }

            if (M_SITE == "NOKIA")
            {
                if (CUST_SN == sequenlist[SCAN_POS].SN)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message =  "Has been input data--" + sequenlist[SCAN_POS].SN;
                }
                string strGetKPSN = $"SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE SERIAL_NUMBER='{editSerialNumber.Text}' AND KEY_PART_SN='{SSN}'";
                var qry_KPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetKPSN,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_KPSN.Data == null)
                {
                    SSN_OK = false;
                    exeRes.Status = false;
                    exeRes.Message = "50010 - " + await GetPubMessage("50010");
                }
            }

            strDataInput = CUST_SN;
            return exeRes;
        }
        
        private async Task processC_Pallet()
        {
            try
            {
                string strUpdate = "UPDATE sfis1.c_pallet_t SET QTY=QTY+1"
                + " where pallet_no = '" + MBOXID + "'"
                + " and close_flag = 'BOXID'";
                var Updatet = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
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
        private async Task<bool> CheckSSNEXT3(string SN)
        {
            string temkp;
            string strGetRangeExt3 = "select * from sfism4.r_mo_ext3_t where :teminput between item_1 and item_2 and ROWNUM=1";
            var qry_RangeExt3 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRangeExt3,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="teminput",Value=SN}
                }
            });
            if (qry_RangeExt3.Data != null)
            {
                temkp = qry_RangeExt3.Data["ver_4"].ToString();
                if (Edt_Keyparts.Text.IndexOf(temkp) > -1)
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
        private async Task<bool> CheckNOMAC(int SSN_IDX,string info)
        {
            string tmpsn, temmodel, temkp;
            int bomcout;

            if (sLOAD_SSN[SSN_IDX].sCOMPARE_SSN.Substring(2,3) != "SSN")
            {
                info = await GetPubMessage("00336") + "--NO COMPARE_SSN " + sLOAD_SSN[SSN_IDX].sCOMPARE_SSN.Substring(2,5);
                Inputdata.Focus();
                return false;
            }
            if (item_Assy_input.IsChecked == true)
            {
                string strGetCustSN = "select * from sfism4.r_custsn_t where " + sLOAD_SSN[SSN_IDX].sCOMPARE_SSN.Substring(2, 5) + "=:t_ssn and rownum=1";
                var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustSN,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="t_ssn",Value=strDataInput}
                    }
                });
                if (qry_CustSN.Data != null)
                {
                    tmpsn = qry_CustSN.Data["serial_number"].ToString();
                    string strGetKeyPart = "select * from sfism4.r_wip_keyparts_t where key_part_no<>'MACID' AND key_part_sn=:key_part_sn and rownum=1";
                    var qry_KeyPart = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPart,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="key_part_sn",Value=tmpsn}
                        }
                    });
                    if (qry_KeyPart.Data != null)
                    {
                        if (qry_KeyPart.Data["serial_number"].ToString() != editSerialNumber.Text)
                        {
                            info = "00677 - " + await GetPubMessage("00677") + "--" + qry_KeyPart.Data["serial_number"].ToString();
                            Inputdata.Focus();
                            return false;
                        }
                    }
                    else
                    {
                        string strGetDataWIP = $"select * from sfism4.r_wip_tracking_t where  serial_number='{tmpsn}' and rownum=1";
                        var qry_DataWIP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetDataWIP,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_DataWIP.Data != null)
                        {
                            temmodel = qry_DataWIP.Data["model_name"].ToString();
                            if (qry_DataWIP.Data["model_name"].ToString() == "STOCKIN")
                            {
                                temkp = qry_DataWIP.Data["model_name"].ToString();
                                if (Edt_Keyparts.Text.IndexOf(qry_DataWIP.Data["model_name"].ToString()) > -1)
                                {
                                    string strGetDataMO = $"select * from sfism4.r_mo_base_t where mo_number='{Input_Monumber.Text}' and rownum=1";
                                    var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetDataMO,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_DataMO.Data != null)
                                    {
                                        BOM_NO = qry_DataMO.Data["bom_no"].ToString();
                                    }
                                    string strGetBOMNO = "select * from sfis1.c_bom_keypart_t where bom_no=:BOM_NO and group_name=:group_name and KEY_PART_NO=:KEY_PART_NO";
                                    var qry_BOMNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetBOMNO,
                                        SfcCommandType = SfcCommandType.Text,
                                        SfcParameters = new List<SfcParameter>()
                                        {
                                            new SfcParameter{Name="BOM_NO",Value=BOM_NO},
                                            new SfcParameter{Name="group_name",Value=M_sThisGroup},
                                            new SfcParameter{Name="KEY_PART_NO",Value=temmodel}
                                        }
                                    });
                                    bomcout = Int32.Parse(qry_BOMNO.Data["kp_count"].ToString());
                                    if (qry_BOMNO.Data != null)
                                    {
                                        if (bomcout == 2)
                                        {
                                            string strGetKPSN = "select * from sfism4.r_wip_keyparts_t where serial_number=:serial_number and group_name=:group_name";
                                            var qry_KPSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strGetKPSN,
                                                SfcCommandType = SfcCommandType.Text,
                                                SfcParameters = new List<SfcParameter>()
                                                {
                                                    new SfcParameter{Name="serial_number",Value=editSerialNumber.Text},
                                                    new SfcParameter{Name="group_name",Value=M_sThisGroup}
                                                }
                                            });
                                            List<R108> result_keypart = new List<R108>();
                                            result_keypart = qry_KPSN.Data.ToListObject<R108>().ToList();
                                            if ((qry_KPSN.Data.Count() == 1) && (bomcout == 2 || result_keypart[0].KEY_PART_NO != temmodel))
                                            {
                                                string strInsert = "Insert into SFISM4.R_WIP_KEYPARTS_T"
                                                    + " (EMP_NO, SERIAL_NUMBER, KEY_PART_NO, KEY_PART_SN,"
                                                    + " KP_RELATION, GROUP_NAME, CARTON_NO, WORK_TIME,"
                                                    + " VERSION, KP_CODE, MO_NUMBER)"
                                                    + " Values"
                                                    + " (:EMP_NO, :SERIAL_NUMBER, :KEY_PART_NO, :KEY_PART_SN,"
                                                    + " :KP_RELATION, :GROUP_NAME, :CARTON_NO, SYSDATE,"
                                                    + " :VERSION, :KP_CODE, :MO_NUMBER)";
                                                var Insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                {
                                                    CommandText = strInsert,
                                                    SfcCommandType = SfcCommandType.Text,
                                                    SfcParameters = new List<SfcParameter>()
                                                    {
                                                        new SfcParameter{Name="EMP_NO",Value=empNo},
                                                        new SfcParameter{Name="SERIAL_NUMBER",Value=editSerialNumber.Text},
                                                        new SfcParameter{Name="KEY_PART_NO",Value=temkp},
                                                        new SfcParameter{Name="KEY_PART_SN",Value=tmpsn},
                                                        new SfcParameter{Name="KP_RELATION",Value="1"},
                                                        new SfcParameter{Name="GROUP_NAME",Value=M_sThisGroup},
                                                        new SfcParameter{Name="CARTON_NO",Value="N/A"},
                                                        new SfcParameter{Name="VERSION",Value="N/A"},
                                                        new SfcParameter{Name="KP_CODE",Value="N/A"},
                                                        new SfcParameter{Name="VERSIONMO_NUMBER",Value=Input_Monumber.Text},
                                                    }
                                                });

                                                string strUpdate = "update SFISM4.R_WIP_TRACKING_T set ship_no=:newssn where serial_number=:serial_number";
                                                var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                                {
                                                    CommandText = strUpdate,
                                                    SfcCommandType = SfcCommandType.Text,
                                                    SfcParameters = new List<SfcParameter>()
                                                    {
                                                        new SfcParameter{Name="newssn",Value=editSerialNumber.Text},
                                                        new SfcParameter{Name="serial_number",Value=tmpsn}
                                                    }
                                                });
                                            }
                                            else
                                            {
                                                info = "00498 - " + await GetPubMessage("00498") +"-"+ tmpsn;
                                                Inputdata.Focus();
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            string strGetCustSN2 = "select * from sfism4.r_custsn_t where serial_number=:serial_number ROWNUM=1";
                                            var qry_CustSN2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strGetCustSN2,
                                                SfcCommandType = SfcCommandType.Text,
                                                SfcParameters = new List<SfcParameter>()
                                                {
                                                     new SfcParameter{Name="serial_number",Value=tmpsn}
                                                }
                                            });
                                            if (qry_CustSN2.Data != null)
                                            {
                                                if (qry_CustSN2.Data[sLOAD_SSN[SSN_IDX].sCOMPARE_SSN.Substring(2,5)].ToString() != strDataInput)
                                                {
                                                    info = "00368 - " + await GetPubMessage("00368") + "CUSTSN--" + tmpsn;
                                                    Inputdata.Focus();
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    info = "00499 - " + await GetPubMessage("00499") + "--" + tmpsn;
                                    Inputdata.Focus();
                                    return false;
                                }
                            }
                            else
                            {
                                info = "00022 - " + await GetPubMessage("00022") + "--" + tmpsn;
                                Inputdata.Focus();
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    info = "00311 - " + await GetPubMessage("00311") + "--" + sLOAD_SSN[SSN_IDX].sCOMPARE_SSN;
                    return false;
                }
            }
            return true;
        }
        private bool EVENMAC(string Strtocheck)
        {
            int retint, STRLENGTH;
            string str;
            char ch1;
            retint = 0;
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
        private bool NEXTMAC(string Strtocheck1, string Strtocheck)
        {
            string str;
            str = "0123456789ABCDEF";
            if (!string.IsNullOrEmpty(Strtocheck1))
            {
                return false;
            }

            retstr = F_DEFINE_SN_NEXT(Strtocheck1, str, 1, 12);
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
            LASTSTR = AlphabeticSets.Substring(AlphabeticSets.Length, 1);
            tmpstring_1 = STR1.Substring(0, INT2 - 1);
            tmpstring_2 = STR1.Substring(INT2, INT3);
            tmpstring_3 = STR1.Substring(tmpstring_1.Length + tmpstring_2.Length, STR1.Length - tmpstring_1.Length - tmpstring_2.Length + 1);
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

                    tmpstring_2 = tmpstring_2.Substring(0, k - 1) + F_NEXT_CHAR(AlphabeticSets, tmpstring_2.Substring(k - 1, 1))
                        + tmpstring_2.Substring(k, tmpstring_2.Length - k);
                }
            }
            return tmpstring_1 + tmpstring_2 + tmpstring_3;
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
                return STR1.Substring(m, 1);
            }
            return STR1.Substring(m, 1);
        }
        private async Task<bool> Checksnchar()
        {
            string str2,SN;
            string DATA, data2, data3, data4, a, d, SerialSN;
            int i, j, k, m, n, b, c;

            DATA = "";

            string SQL_STR = "SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + Edt_monumber.Text + "'";
            var query_mo_number = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = SQL_STR,
                SfcCommandType = SfcCommandType.Text
            });
            if (query_mo_number.Data != null)
            {
                Wo_type = query_mo_number.Data["mo_type"].ToString();
                mo_ver = query_mo_number.Data["version_code"].ToString();
            }

            string SQL_str = "SELECT SHIPPING_CODE FROM SFIS1.C_CUSTSN_RULE_T  WHERE MODEL_NAME ='" + Edt_model.Text + "'"
                + " AND CUSTSN_CODE =:SSNSTRING AND MO_TYPE=:WO_TYPE AND version_code=:VERSION_CODE AND SHIPPING_CODE IS NOT NULL  ORDER BY CUSTSN_CODE";
            var query_model_name = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = SQL_str,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="SSNSTRING",Value = MACSTRING},
                    new SfcParameter{Name="WO_TYPE",Value = Wo_type},
                    new SfcParameter{Name="VERSION_CODE",Value = mo_ver}
                }
            });
            if (query_model_name.Data != null)
            {
                str2 = query_model_name.Data["shipping_code"].ToString();
                if (str2.IndexOf("CTR:") > -1)
                {
                    i = 0;
                    j = DATA.Length;
                    i = DATA.IndexOf("CTR:");
                    data2 = DATA.Substring(i - 1, j - i + 1);
                    k = data2.IndexOf(";");
                    data3 = DATA.Substring(i - 1, k);
                    m = data3.IndexOf(",");
                    n = data3.IndexOf(":");
                    a = DATA.Substring(n, m - n - 1);
                    b = data3.Length;
                    data4 = data3.Substring(m, b - m);
                    c = data4.IndexOf(",");
                    d = data4.Substring(0, c - 1);
                    SN = strDataInput;
                    SerialSN = SN.Substring(Int32.Parse(a), Int32.Parse(d));
                    strDataInput = SerialSN;
                    if (SerialSN != "")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private async Task<string> GetDateCode_All()
        {
            string tmpstr, s_y, s_m, s_d;

            string strGetDateTime = "select to_char(sysdate,'YYMMDDWW') AS TT from dual";
            var qry_DateTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDateTime,
                SfcCommandType = SfcCommandType.Text
            });
            tmpstr = qry_DateTime.Data["TT"].ToString();
            s_y = tmpstr.Substring(1, 1);
            s_m = tmpstr.Substring(2, 2);
            s_d = tmpstr.Substring(4, 2);
            S_YM = tmpstr.Substring(1, 1);
            S_YYWW = tmpstr.Substring(0, 2) + tmpstr.Substring(6, 2);

            string strGetModelSite = $"select site from sfis1.c_model_site_t where model_name in (select model_name from sfism4.r_wip_tracking_t where TRACK_NO = '{editSerialNumber.Text}')";
            var qry_ModelSite = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModelSite,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ModelSite.Data["site"].ToString() == "Soney")
            {
                return s_y + s_m + s_d;
            }
            else
            {
                return s_y + s_m + s_d;
            }
        }
        private bool CHECKMODELSSN(string SSNPREFIX, string Strtocheck)
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
        private bool IBMModulo10(string Strtocheck)
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
        private bool ControlKey(string Strtocheck)
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
        private async Task<string> CheckFCD(string SN,string BOXID, bool _FCD_OK)
        {
            string temp_modelname, temp_sn, temp_site, UBNTCheckFcd, ModelCheckFcd, temp_kp, temp_fcd;
            string sn_kp, sn_fcd;

            sn_kp = "";

            string strGetTrayNO = $"select * from sfism4.r_wip_tracking_t where tray_no='{BOXID}' and rownum=1";
            var qry_TrayNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetTrayNO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_TrayNO.Data != null)
            {
                temp_modelname = qry_TrayNO.Data["model_name"].ToString();
                temp_sn = qry_TrayNO.Data["serial_number"].ToString();

                string strGetModelSite = $"select * from sfis1.c_model_site_t where model_name='{temp_modelname}'";
                var qry_ModelSite = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetModelSite,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ModelSite.Data != null)
                {
                    temp_site = qry_ModelSite.Data["site"].ToString();
                
                    string strGetParameter = $"SELECT * FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='{temp_site}' AND VR_NAME='CHECK MAC PREFIX/BOM/OSFW/FCD'";
                    var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetParameter,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Parameter.Data != null)
                    {
                        UBNTCheckFcd = qry_Parameter.Data["vr_item"].ToString();
                        ModelCheckFcd = "Y";

                        string strGetParameterINI = $"SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = 'PACK_BOX' AND VR_CLASS ='{temp_site}' AND VR_NAME = '{temp_modelname}'";
                        var qry_ParameterINI = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetParameterINI,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_ParameterINI.Data != null)
                        {
                            ModelCheckFcd = qry_ParameterINI.Data["vr_item"].ToString();
                        }
                        if (UBNTCheckFcd == "Y")
                        {
                            if (ModelCheckFcd == "N")
                            {
                                //need not check Fcd BY MODEL
                                FCD_OK = true;
                            }
                            else
                            {
                                string strGetKeyPart = $"select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' and serial_number='{temp_sn}' and rownum=1";
                                var qry_KeyPart = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetKeyPart,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_KeyPart.Data == null)
                                {
                                    FCD_OK = false;
                                    Inputdata.Focus();
                                    return "00301 - " + await GetPubMessage("00301") + "-" + temp_sn;
                                }
                                else
                                {
                                    temp_kp = qry_KeyPart.Data["key_part_sn"].ToString();

                                    string strGetCustSN = $"select * from sfism4.r_custsn_t where serial_number='{temp_kp}' and ssn10 is not null and rownum=1";
                                    var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetCustSN,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_CustSN.Data == null)
                                    {
                                        FCD_OK = false;
                                        Inputdata.Focus();
                                        return await GetPubMessage("50043") + "-" + temp_sn;
                                    }
                                    else
                                    {
                                        temp_fcd = qry_CustSN.Data["ssn10"].ToString();
                                    }
                                }

                                string strGetKeyPartSN = $"select * from sfism4.r_wip_keyparts_t where key_part_no<>'MACID' and serial_number='{SN}' and rownum=1";
                                var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetKeyPartSN,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_KeyPartSN.Data != null)
                                {
                                    sn_kp = qry_KeyPartSN.Data["key_part_sn"].ToString();

                                    string strGetCustnSNN10 = $"select * from sfism4.r_custsn_t where serial_number='{sn_kp}' and ssn10 is not null and rownum=1";
                                    var qry_CustnSNN10 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetCustnSNN10,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_CustnSNN10.Data == null)
                                    {
                                        FCD_OK = false;
                                        Inputdata.Focus();
                                        return "50043 - " + await GetPubMessage("50043") + "-" + sn_kp;
                                    }
                                    else
                                    {
                                        sn_fcd = qry_CustnSNN10.Data["ssn10"].ToString();
                                        if (temp_fcd != sn_fcd)
                                        {
                                            FCD_OK = false;
                                            Inputdata.Focus();
                                            return "00687 - " + await GetPubMessage("00687");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    FCD_OK = false;
                    Inputdata.Focus();
                    return "00683 - " + await GetPubMessage("00683") + "-"+ temp_modelname;
                }
            }
            else
            {
                FCD_OK = true;
                return "";
            }
            return "";
        }
        private async Task<string> CheckMacPrefix(string SN,string BOXID,bool _MacPrefix_OK)
        {
            string temp_modelname, temp_mac, temp_sn, temp_site, CheckMacPrefix, ModelCheckMacPrefix;

            //check mac prefix ,not match,can not pack together ---added by lanmiao 20131231
            string strGetDataTrayNO = $"select * from sfism4.r_wip_tracking_t where tray_no='{BOXID}' and rownum=1";
            var qry_TrayNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataTrayNO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_TrayNO.Data != null)
            {
                temp_modelname = qry_TrayNO.Data["model_name"].ToString();
                temp_sn = qry_TrayNO.Data["serial_number"].ToString();

                string strGetModelSite = $"select * from sfis1.c_model_site_t where model_name='{temp_modelname}'";
                var qry_ModelSite = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetModelSite,
                    SfcCommandType = SfcCommandType.Text
                });
                
                if (qry_ModelSite.Data != null)
                {
                    temp_site = qry_ModelSite.Data["site"].ToString();
                    string strGetParameter = $"SELECT * FROM SFIS1.C_PARAMETER_INI  WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='{temp_site}' AND VR_NAME='CHECK MAC PREFIX/BOM/OSFW/FCD'";
                    var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetParameter,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Parameter.Data != null)
                    {
                        CheckMacPrefix = qry_Parameter.Data["vr_value"].ToString();
                        ModelCheckMacPrefix = "Y";

                        string strGetParmeter = $"SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = 'PACK_BOX' AND VR_CLASS ='{temp_site}' AND VR_NAME = '{temp_modelname}'";
                        var qry_GetParmeter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetParmeter,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_GetParmeter.Data != null)
                        {
                            modelsite = qry_GetParmeter.Data["vr_value"].ToString();
                        }
                        if (CheckMacPrefix == "Y")
                        {
                            if (ModelCheckMacPrefix == "N")
                            {
                                //need not check mac prefix by model
                                MacPrefix_OK = true;
                            }
                            else
                            {
                                string strGetKeyPartNO = $"select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' and serial_number='{temp_sn}' and rownum=1";
                                var qry_KeyPartNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetKeyPartNO,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_KeyPartNO.Data == null)
                                {
                                    string strGetKeyPartMACID = "select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' and  serial_number in ("
                                        + $" select key_part_sn from sfism4.r_wip_keyparts_t where serial_number='{temp_sn}')";
                                    var qry_KeyPartMACID = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetKeyPartMACID,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_KeyPartMACID.Data != null)
                                    {
                                        temp_mac = qry_KeyPartMACID.Data["key_part_sn"].ToString();
                                    }
                                    else
                                    {
                                        MacPrefix_OK = false;
                                        Inputdata.Focus();
                                        return "00684 - " + await GetPubMessage("00684") + "-" + temp_sn;
                                    }
                                }
                                else
                                {
                                    temp_mac = qry_KeyPartNO.Data["key_part_sn"].ToString();
                                }

                                string strGetKP = $"select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' and serial_number='{SN}' and rownum=1";
                                var qry_KP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetKP,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_KP.Data != null)
                                {
                                    if (qry_KP.Data["key_part_sn"].ToString().Substring(0,6) != temp_mac.Substring(0,6))
                                    {
                                        MacPrefix_OK = false;
                                        Inputdata.Focus();
                                        return await GetPubMessage("00682") + "-" + temp_sn;
                                    }
                                }
                                else
                                {
                                    string strGetKPSN = "select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' and  serial_number in ("
                                        + $" select key_part_sn from sfism4.r_wip_keyparts_t where serial_number='{SN}')";
                                    var qry_KPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetKPSN,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_KPSN.Data != null)
                                    {
                                        if (qry_KPSN.Data["key_part_sn"].ToString().Substring(0,6) != temp_mac.Substring(0,6))
                                        {
                                            MacPrefix_OK = false;
                                            Inputdata.Focus();
                                            return "00682 - " + await GetPubMessage("00682") + "-" + temp_sn;
                                        }
                                    }
                                    else
                                    {
                                        MacPrefix_OK = false;
                                        Inputdata.Focus();
                                        return "00684 - " + await GetPubMessage("00684") + "-" + temp_sn;
                                    }
                                }
                            }
                        }
                    }
                    else { MacPrefix_OK = true; }
                }
                else
                {
                    MacPrefix_OK = false;
                    Inputdata.Focus();
                    return "00683 - " + await GetPubMessage("00683") + "-" + temp_modelname;
                }
            }
            else
            {
                MacPrefix_OK = true;
                return "";
            }
            return "";
        }
        private async Task<string> CheckSN(string SN,bool _CHECKSN_OK)
        {
            string MO_Status, TMPWIP, printmaclabelcheck, temjopqty, cVersion, KPNO;
            string tmphh,newhh, tmpkeyparts;
            int C_REALTIME, C_CHECKTIME, iCapacity;
            int i;
            cVersion = "";
            MO_Status = "";
            MACID = "";
            if (!string.IsNullOrEmpty(SN))
            {
                if (item_Assy_input.IsChecked == true)
                {
                    if (((MO_TYPE == "Rework") || (MO_TYPE == "RMA")) && (SN.Length == 12))
                    {
                        string strGetKeyPart = $"select * from sfism4.r_wip_keyparts_t where key_part_no='MACID' and key_part_sn='{SN}' and rownum=1";
                        var qry_KeyPart = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetKeyPart,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_KeyPart.Data != null)
                        {
                            SN = qry_KeyPart.Data["serial_number"].ToString();
                        }
                    }
                    tmphh = SN;
                    
                    newhh = SN + Input_Monumber.Text;
                    string strGetDataWIP = $"select * from sfism4.r_wip_tracking_t where serial_number='{tmphh}' and rownum=1";
                    var qry_DataWIP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetDataWIP,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_DataWIP.Data.Count() > 0)
                    {
                        List<R107> result_r107 = new List<R107>();
                        result_r107 = qry_DataWIP.Data.ToListObject<R107>().ToList();
                        tmpkeyparts = result_r107[0].MODEL_NAME;
                        TMPWIP = result_r107[0].WIP_GROUP;
                        printmaclabelcheck = result_r107[0].GROUP_NAME_CQC;
                        if (TMPWIP != "STOCKIN")
                        {
                            Inputdata.Focus();
                            SN_OK = false;
                            return "00022 - " + await GetPubMessage("00022") + " - " + TMPWIP;
                        }
                        if (printmaclabelcheck == "LABELREPRINT")
                        {
                            Inputdata.Focus();
                            SN_OK = false;
                            return "90030 - " + await GetPubMessage("90030") +"-"+ tmphh;
                        }

                        // 20160525
                        //begin check AMPFI
                        string strGetModeltype = $"select MODEL_TYPE  from  SFIS1.C_MODEL_DESC_T where model_name='{Edt_model.Text}'";
                        var qry_Modeltype = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetModeltype,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_Modeltype.Data != null)
                        {
                            AMPMODELTYPE = qry_Modeltype.Data["model_type"].ToString();
                        }
                        if (((Edt_model.Text.Substring(0,3) == "AFILR") || (AMPMODELTYPE.IndexOf("221") > -1)) && (Edt_model.Text.Substring(8,1) == "."))
                        {
                            string strGetBomNo = "select b.* from sfism4.r_mo_base_t a,sfis1.c_bom_keypart_t b"
                                + $" where  a.bom_no=b.bom_no and b.kp_count=1 and mo_number='{Input_Monumber.Text}'";
                            var qry_BomNo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetBomNo,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_BomNo.Data != null)
                            {
                                if (qry_BomNo.Data.Count() > 1)
                                {
                                    Inputdata.Focus();
                                    SN_OK = false;
                                    return "90041 - " + await GetPubMessage("90041");
                                }
                                else
                                {
                                    if (tmpkeyparts != qry_BomNo.Data["key_part_no"].ToString())
                                    {
                                        Inputdata.Focus();
                                        SN_OK = false;
                                        return "900036 - " + await GetPubMessage("900036");
                                    }
                                }
                            }
                        }
                        string stGetDataWIP = $"select * from sfism4.r_wip_tracking_t where SERIAL_NUMBER='{newhh}' and rownum=1";
                        var qry_stGetDataWIP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = stGetDataWIP,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_stGetDataWIP.Data == null)
                        {
                            string strGetData = "select B.LABEL_CONTENT,A.END_MSN,A.JOB_QTY"
                                        + " from sfism4.R_MO_BASE_T A,SFIS1.C_PACK_PARAM_T B"
                                        + " where B.LABEL_CONTENT IS NOT NULL AND  B.VERSION_CODE=A.VERSION_CODE AND B.MODEL_NAME=A.MODEL_NAME AND  A.MO_NUMBER='" + Input_Monumber.Text + "'";
                            var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetData,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_Data.Data != null)
                            {
                                temjopqty = qry_Data.Data["job_qty"].ToString();
                                if (qry_Data.Data["end_msn"].ToString() != "")
                                {
                                    Inputdata.Focus();
                                    SN_OK = false;
                                    return "90028 - " + await GetPubMessage("90028");
                                }
                                if ((temjopqty != "") && (temjopqty != "0"))
                                {
                                    string strGetCount = $"select COUNT(*) as CUT from sfism4.r_wip_tracking_t where MO_NUMBER='{Input_Monumber.Text}'";
                                    var qry_Count = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetCount,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_Count.Data != null)
                                    {
                                        if (Int32.Parse(qry_Count.Data["cut"].ToString()) >= Int32.Parse(temjopqty))
                                        {
                                            Inputdata.Focus();
                                            SN_OK = false;
                                            return "90027 - " + await GetPubMessage("90027");
                                        }
                                    }
                                }
                                else
                                {
                                    Inputdata.Focus();
                                    SN_OK = false;
                                    return "90027 - " + await GetPubMessage("90027");
                                }
                            }
                        }
                        if (Edt_Keyparts.Text.IndexOf(tmpkeyparts) < -1)
                        {
                            Inputdata.Focus();
                            SN_OK = false;
                            return "00499 - " + await GetPubMessage("00499") + "-" + tmpkeyparts;
                        }
                        else
                        {
                            string strGetKeyPart = $"select * from sfism4.r_wip_keyparts_t where key_part_no<>'MACID' AND  key_part_sn='{tmphh}' and rownum=1";
                            var qry_KeyPart = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetKeyPart,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_KeyPart.Data != null)
                            {
                                Inputdata.Focus();
                                SN_OK = false;
                                return "00677 - " + await GetPubMessage("00677") + "- IN R108 " + qry_KeyPart.Data["serial_number"].ToString();
                            }

                            //Chk dup trong H_Keypart
                            string strGetHKeyPart = $"select * from sfism4.h_wip_keyparts_t where key_part_no<>'MACID' AND  key_part_sn='{tmphh}' and rownum=1";
                            var qry_HKeyPart = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetHKeyPart,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_HKeyPart.Data != null)
                            {
                                Inputdata.Focus();
                                SN_OK = false;
                                return "00677 - " + await GetPubMessage("00677") + "- IN H108 " + qry_HKeyPart.Data["serial_number"].ToString();
                            }

                            string strGetKeyPartSFCODBH = $"select * from sfism4.r_wip_keyparts_t@SFCODBH where key_part_no<>'MACID' AND  key_part_sn='{tmphh}' and rownum=1";
                            var qry_KeyPartSFCODBH = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetKeyPartSFCODBH,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_KeyPartSFCODBH.Data != null)
                            {
                                Inputdata.Focus();
                                SN_OK = false;
                                return "00677 - " + await GetPubMessage("00677") + "- IN R108 HISTORY " + qry_KeyPartSFCODBH.Data["serial_number"].ToString();
                            }

                            string strGetKeyPartGroup = $"select * from sfism4.r_wip_keyparts_t where serial_number='{newhh}' and group_name='{M_sThisGroup}' and rownum=1";
                            var qry_KeyPartGroup = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetKeyPartGroup,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_KeyPartGroup.Data != null)
                            {
                                try
                                {
                                    string strInsertR108 = "insert into sfism4.r_wip_keyparts_undo_t select * from sfism4.r_wip_keyparts_t"
                                    + $" WHERE serial_number='{newhh}' and group_name='{M_sThisGroup}' and rownum=1";
                                    var InsertR108 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strInsertR108,
                                        SfcCommandType = SfcCommandType.Text
                                    });

                                    string strDelete = $"delete from sfism4.r_wip_keyparts_t where serial_number='{newhh}' and group_name='{M_sThisGroup}' and rownum=1";
                                    var deleteR108 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strDelete,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return null;
                                }
                            }

                            //Roku
                            string strGetESOP = "select SFIS1.CHECK_ESOP_STOP_F(:LINE,:MYGROUP,:SN,:MODEL_NAME ) RESULT FROM DUAL";
                            var qry_ESOP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetESOP,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="LINE",Value=Edt_linename.Text.Trim()},
                                    new SfcParameter{Name="MYGROUP",Value=M_sThisGroup},
                                    new SfcParameter{Name="SN",Value=newhh},
                                    new SfcParameter{Name="MODEL_NAME",Value=Edt_model.Text.Trim()},
                                }
                            });
                            if (qry_ESOP.Data["result"].ToString() != "OK")
                            {
                                Inputdata.Focus();
                                return qry_ESOP.Data["result"].ToString();
                            }
                            try
                            {
                                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                {
                                    CommandText = "SFIS1.INPUT_SN_FIRST",
                                    SfcCommandType = SfcCommandType.StoredProcedure,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter {Name="LINE",Value=Edt_linename.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                        new SfcParameter {Name="SECTION",Value="PRINT_INPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                        new SfcParameter {Name="MYGROUP",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                        new SfcParameter {Name="W_STATION",Value=M_sThisStation,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                        new SfcParameter {Name="DATA",Value=newhh,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                        new SfcParameter {Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                    }
                                });
                                dynamic ads = result.Data;
                                string res = ads[0]["res"];
                                if (res != "OK")
                                {
                                    Inputdata.Focus();
                                    SN_OK = false;
                                    return res;
                                }
                            }
                            catch(Exception e)
                            {
                                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageVietNam = e.Message;
                                _er.MessageEnglish = "Call procedure have exceptions:" + e.Message;
                                _er.ShowDialog();
                                strDataInput = "";
                                Inputdata.Focus();
                            }

                            try
                            {
                                string strUpdate = $"update sfism4.r_wip_tracking_t set WIP_GROUP='{M_sThisGroup}'  where serial_number='{newhh}'";
                                var UpdateWIP = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strUpdate,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                TMPWIP = "";

                                // INSERT KEYPART AND CUSTSN
                                string strInsert = "Insert into SFISM4.R_WIP_KEYPARTS_T"
                                    + " (EMP_NO, SERIAL_NUMBER, KEY_PART_NO, KEY_PART_SN,"
                                    + " KP_RELATION, GROUP_NAME, CARTON_NO, WORK_TIME,"
                                    + " VERSION, KP_CODE, MO_NUMBER)"
                                    + " Values"
                                    + " (:EMP_NO, :SERIAL_NUMBER, :KEY_PART_NO, :KEY_PART_SN,"
                                    + " :KP_RELATION, :GROUP_NAME, :CARTON_NO, SYSDATE,"
                                    + " :VERSION, :KP_CODE, :MO_NUMBER)";
                                var Insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strInsert,
                                    SfcCommandType = SfcCommandType.Text,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter{Name="EMP_NO",Value=empNo},
                                        new SfcParameter{Name="SERIAL_NUMBER",Value=newhh},
                                        new SfcParameter{Name="KEY_PART_NO",Value=tmpkeyparts},
                                        new SfcParameter{Name="KEY_PART_SN",Value=tmphh},
                                        new SfcParameter{Name="KP_RELATION",Value="1"},
                                        new SfcParameter{Name="GROUP_NAME",Value=M_sThisGroup},
                                        new SfcParameter{Name="CARTON_NO",Value="N/A"},
                                        new SfcParameter{Name="VERSION",Value="N/A"},
                                        new SfcParameter{Name="KP_CODE",Value="N/A"},
                                        new SfcParameter{Name="MO_NUMBER",Value=Input_Monumber.Text},
                                    }
                                });

                                // UPDATE SHIP_NO
                                string strUpdateShipNO = $"update sfism4.r_wip_tracking_t set ship_no=:newhh where serial_number=:serial_number";
                                var UpdateShipNO = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strUpdateShipNO,
                                    SfcCommandType = SfcCommandType.Text,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter{Name="serial_number",Value=tmphh},
                                        new SfcParameter{Name="newhh",Value=newhh}
                                    }
                                });
                            }
                            catch (Exception)
                            {
                                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                return null;
                            }
                            

                            string strGetDataMO = $"select * from sfism4.r_mo_base_t where mo_number='{Input_Monumber.Text}' and rownum=1";
                            var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetDataMO,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_DataMO.Data != null)
                            {
                                Lb_PackingQty.Content = qry_DataMO.Data["input_qty"].ToString();
                            }

                            strDataInput = newhh;
                            SN = newhh;
                        }
                    }
                    else
                    {
                        Inputdata.Focus();
                        SN_OK = false;
                        return "40001 - " + await GetPubMessage("40001");
                    }
                }

                if ((Cb_Rework.IsChecked == true) && (Cb_Fortmac.IsChecked == true))
                {
                    string strGetCustSN = $"select serial_number from sfism4.r_custsn_t  where ssn1='{SN}'";
                    var qry_CustSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCustSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CustSN.Data != null)
                    {
                        SN = qry_CustSN.Data["serial_number"].ToString();
                        strDataInput = SN;
                    }
                    else
                    {
                        string strGetCustSNN2 = $"select serial_number from sfism4.r_custsn_t  where ssn2='{SN}'";
                        var qry_CustSNN2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetCustSNN2,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_CustSNN2.Data != null)
                        {
                            SN = qry_CustSNN2.Data["serial_number"].ToString();
                            strDataInput = SN;
                        }
                        else
                        {
                            string strGetCustSNN3 = $"select serial_number from sfism4.r_custsn_t  where ssn3='{SN}'";
                            var qry_CustSNN3 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetCustSNN3,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_CustSNN3.Data != null)
                            {
                                SN = qry_CustSNN3.Data["serial_number"].ToString();
                                strDataInput = SN;
                            }
                            else
                            {
                                string strGetCustMAC1 = $"select serial_number from sfism4.r_custsn_t where mac1='{SN}'";
                                var qry_CustMAC1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetCustMAC1,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_CustMAC1.Data != null)
                                {
                                    SN = qry_CustMAC1.Data["serial_number"].ToString();
                                    strDataInput = SN;
                                }
                                else
                                {
                                    string strGetCustMAC2 = $"select serial_number from sfism4.r_custsn_t where mac2='{SN}'";
                                    var qry_CustMAC2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetCustMAC2,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_CustMAC2.Data != null)
                                    {
                                        SN = qry_CustMAC2.Data["serial_number"].ToString();
                                        strDataInput = SN;
                                    }
                                    else
                                    {
                                        string strGetCustMAC3 = $"select serial_number from sfism4.r_custsn_t  where mac3='{SN}'";
                                        var qry_CustMAC3 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strGetCustMAC3,
                                            SfcCommandType = SfcCommandType.Text
                                        });
                                        if (qry_CustMAC3.Data != null)
                                        {
                                            SN = qry_CustMAC3.Data["serial_number"].ToString();
                                            strDataInput = SN;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if ((Cb_Rework.IsChecked == true) && (Cb_Fortmac.IsChecked == false) && (Cb_custsn.IsChecked == false))
                {
                    string strGetKeyPartSN = $"select serial_number from sfism4.r_wip_keyparts_t where KEY_PART_NO<>'MACID' AND key_part_sn='{SN}'";
                    var qry_KeyPartSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetKeyPartSN,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_KeyPartSN.Data != null)
                    {
                        SN = qry_KeyPartSN.Data["serial_number"].ToString();
                        strDataInput = SN;
                    }
                }

                //--//
                string strGetDataSN = "select * from sfism4.r_wip_tracking_t"
                    + $" where shipping_sn =  '{SN}' or serial_number = '{SN}' OR Tray_No= '{SN}'";
                var qry_DataSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetDataSN,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_DataSN.Data.Count() == 0)
                {
                    SN_OK = false;
                    return "00347 - " + await GetPubMessage("00347");
                }

                List<R107> result_datar107 = new List<R107>();
                result_datar107 = qry_DataSN.Data.ToListObject<R107>().ToList();
                if ((result_datar107[0].TRAY_NO != "N/A") && (result_datar107[0].TRAY_NO != ""))
                {
                    if (MODEL_TYPE.IndexOf("190") > -1)
                    {
                        if ((result_datar107[0].TRACK_NO != "N/A") && (result_datar107[0].TRACK_NO != ""))
                        {
                            SN_OK = false;
                            return await GetPubMessage("00400") + "-" + result_datar107[0].TRAY_NO;
                        }
                    }
                    else
                    {
                        SN_OK = false;
                        return "00400 - " + await GetPubMessage("00400") + result_datar107[0].TRAY_NO;
                    }
                }
                // check scrap
                if (result_datar107[0].SCRAP_FLAG == "1")
                {
                    SN_OK = false;
                    return "00401 - " + await GetPubMessage("00401");
                }

                // check route
                if (await checkRoute() == false)
                {
                    SN_OK = false;
                    return lbError.Text.ToString();
                }
                MO = result_datar107[0].MO_NUMBER;

                //check mo status
                if (await chkMO(MO_Status) == true)
                {
                    if (Result_MO_Status == "1")
                    {
                        SN_OK = false;
                        return await GetPubMessage("00012");
                    }
                    if (Result_MO_Status == "3")
                    {
                        SN_OK = false;
                        return await GetPubMessage("00013");
                    }
                    if (Result_MO_Status == "5")
                    {
                        SN_OK = false;
                        return await GetPubMessage("00014");
                    }
                    if (Result_MO_Status == "6")
                    {
                        SN_OK = false;
                        return await GetPubMessage("00015");
                    }
                    if (Result_MO_Status == "")
                    {
                        SN_OK = false;
                        return await GetPubMessage("40001");
                    }
                }
                CHECKMOTYPE();
                if ((Edt_monumber.Text != "") && (Edt_monumber.Text != result_datar107[0].MO_NUMBER))
                {
                    if (IsPackingByMoElseByModel == true)
                    {
                        SN_OK = false;
                        return await GetPubMessage("00402");
                    }
                }

                Edt_monumber.Text = result_datar107[0].MO_NUMBER;
                Edt_model.Text = result_datar107[0].MODEL_NAME;
                smodel_name = result_datar107[0].MODEL_NAME;
                Edt_section.Text = result_datar107[0].SECTION_NAME;
                Edt_group.Text = result_datar107[0].GROUP_NAME;
                Edt_station.Text =  result_datar107[0].STATION_NAME;
                M_sSSN = result_datar107[0].SHIPPING_SN;
                M_sSSN1 = result_datar107[0].SHIPPING_SN;
                M_sSSN2 = result_datar107[0].SHIPPING_SN2;
                macfound = 0;
                string getmodelserial_ = await getmodelserial();

                string strGetRO = $"SELECT count(*) as count  FROM SFISM4.r117 WHERE SERIAL_NUMBER = '{SN}' AND GROUP_NAME = 'ROAST_OUT'";
                var qry_RO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetRO,
                    SfcCommandType = SfcCommandType.Text
                });
                var C_RO = Int32.Parse(qry_RO.Data["count"].ToString());

                if ((M_sThisGroup == "PACK_BOX") && (getmodelserial_.IndexOf("Cinterion") > -1) && C_RO > 0)
                {
                    string strGetTRUNC = $"SELECT TRUNC ( (SYSDATE - MAX (IN_STATION_TIME)) * 24*60) as C_REALTIME  FROM SFISM4.R_SN_DETAIL_T WHERE SERIAL_NUMBER = '{SN}' AND GROUP_NAME = 'ROAST_OUT'";
                    var qry_TRUNC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetTRUNC,
                        SfcCommandType = SfcCommandType.Text
                    });
                    C_REALTIME = Int32.Parse(qry_TRUNC.Data["c_realtime"].ToString());

                    string strGetControlTime = $"SELECT CONTROL_TIME * 60  as C_CONTROLTIME FROM SFIS1.C_ROAST_TIME_CONTROL_T WHERE MODEL_NAME = '{Edt_model.Text}' AND"
                        + " DEFAULT_GROUP = 'ROAST_OUT' AND END_GROUP = 'PACK_BOX' and CONTROL_TIME is not null AND ROWNUM = 1";
                    var qry_ControlTime = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetControlTime,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_ControlTime.Data != null)
                    {
                        C_CHECKTIME = Int32.Parse(qry_ControlTime.Data["c_controltime"].ToString());
                        if ((C_REALTIME - C_CHECKTIME) >= 0)
                        {
                            SN_OK = false;
                            return "RoastTime Over ControlTime (ROAST_OUT -> PACK_BOX), Please Roast Again! " + SN;
                        }
                    }
                }

                if (M_SITE == "NOKIA")
                {
                    if (MO_BOM_NO != "")
                    {
                        if (await FindNokiaSn(SN,macfound,MO_BOM_NO) == 0)
                        {
                            MACID = "N/A";
                            MAC1 = "N/A";
                            MAC2 = "N/A";
                            MAC3 = "N/A";
                            FoundMAC = false;
                        }
                        else
                        {
                            FoundMAC = true;
                        }
                    }
                }
                else
                {
                    if (BOM_HAS_MAC == true)
                    {
                        macfound = 0;
                        if (await FindMAC(SN,macfound) == 0)
                        {
                            MACID = "N/A";
                            MAC1 = "N/A";
                            MAC2 = "N/A";
                            MAC3 = "N/A";
                            FoundMAC = false;
                        }
                        else
                        {
                            FoundMAC = true;
                        }
                    }
                    else
                    {
                        if (MO_BOM_NO != "" && MO_BOM_NO != null)
                        {
                            if (await FindMAC(SN,macfound) == 0)
                            {
                                MACID = "N/A";
                                MAC1 = "N/A";
                                MAC2 = "N/A";
                                MAC3 = "N/A";
                                FoundMAC = false;
                            }
                            else
                            {
                                FoundMAC = true;
                            }
                        }
                        else
                        {
                            NO_MAC = true;
                        }
                    }

                    if ((NO_MAC == false) && (Mmodel_type.IndexOf("194") > -1))
                    {
                        if (FoundMAC == false)
                        {
                            SN_OK = false;
                            return SN + "-" + await GetPubMessage("00403");
                        }
                    }
                }

                iCapacity = await getCapacity();
                if ((iCapacity == 0) || (iCapacity == -1))
                {
                    SN_OK = false;
                    return "00111 - " + await GetPubMessage("00111");
                }
                lb_Capacity.Content = iCapacity.ToString();

                //check config 43
                if (cVersion == "")
                {
                    cVersion = result_datar107[0].VERSION_CODE;
                    //check config 44
                    string strGetConfig44 = "SELECT * FROM SFIS1.C_CUSTSN_RULE_T"
                           + $" WHERE  MODEL_NAME = '{Edt_model.Text}'"
                           + $" AND VERSION_CODE = '{cVersion}' AND MO_TYPE='{sMO_TYPE}'";
                    var qry_onfig44 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetConfig44,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_onfig44.Data.Count() == 0)
                    {
                        SN_OK = false;
                        return "20102 - " + await GetPubMessage("20102");
                    }
                    else
                    {
                        CUST_SN_MODEL = qry_onfig44.Data.ToListObject<CUST_RULE>().ToList();

                        foreach (CUST_RULE items in CUST_SN_MODEL)
                        {
                            sCUSTSN_CODE = items.CUSTSN_CODE;

                            if (sCUSTSN_CODE.IndexOf("SSN") != -1)
                            {
                                for (i= 1;i <= 12; i++)
                                {
                                    if (("SSN"+i.ToString()) == sCUSTSN_CODE)
                                    {
                                        break;
                                    }
                                }
                                sLOAD_SSN[i].SSN = "";
                                sLOAD_SSN[i].SSN_OK = false;
                                sLOAD_SSN[i].cSSN_PREFIX = items.CUSTSN_PREFIX == null ? "" : items.CUSTSN_PREFIX;
                                sLOAD_SSN[i].cSSN_POSTFIX = items.CUSTSN_POSTFIX == null ? "" : items.CUSTSN_POSTFIX;
                                sLOAD_SSN[i].cSSN_STR = items.CUSTSN_STR == null ? "" : items.CUSTSN_STR;
                                sLOAD_SSN[i].sSHIPPINGSN_CODE = items.SHIPPINGSN_CODE == null ? "" : items.SHIPPINGSN_CODE;
                                sLOAD_SSN[i].sCHECK_SSN_RULE = items.CHECK_RULE_NAME == null ? "" : items.CHECK_RULE_NAME;
                                sLOAD_SSN[i].sCHECK_SSN_RANGE = items.CHECK_RANGE == null ? "" : items.CHECK_RANGE;
                                sLOAD_SSN[i].sCHECK_SSN = items.CHECK_SSN == null ? "" : items.CHECK_SSN;
                                sLOAD_SSN[i].nSSN_LENGTH = Int32.Parse(items.CUSTSN_LENG == null ? "" : items.CUSTSN_LENG);
                                if (sLOAD_SSN[i].cSSN_PREFIX.IndexOf(",") > -1)
                                {
                                    sLOAD_SSN[i].nSSN_PREFIX_LEN = sLOAD_SSN[i].cSSN_PREFIX.IndexOf(",") - 1;
                                }
                                else
                                {
                                    sLOAD_SSN[i].nSSN_PREFIX_LEN = (sLOAD_SSN[i].cSSN_PREFIX).Length;
                                }
                                sLOAD_SSN[i].nSSN_POSTFIX_LEN = (sLOAD_SSN[i].cSSN_POSTFIX).Length;
                                sLOAD_SSN[i].sCOMPARE_SSN = items.COMPARE_SN == null ? "" : items.COMPARE_SN;
                                sLOAD_SSN[i].nSSN_Self_StartDigit = items.CUSTSN_START;
                                sLOAD_SSN[i].nSSN_Self_FlowNO = items.CUSTSN_END;
                                sLOAD_SSN[i].nSSN_Self_StartDigit = items.COMPARE_SN_START;
                                sLOAD_SSN[i].nSSN_Compare_FlowNO = items.COMPARE_SN_END;
                            }
                            if (sCUSTSN_CODE.IndexOf("MAC") > -1)
                            {
                                for (i = 1;i <= 15; i++)
                                {
                                    if (sCUSTSN_CODE.IndexOf("MAC"+i.ToString()) > -1)
                                    {
                                        break;
                                    }
                                }
                                sLOAD_MAC[i].MAC = "";
                                sLOAD_MAC[i].MAC_OK = false;
                                sLOAD_MAC[i].cMAC_PREFIX = items.CUSTSN_PREFIX == null ? "" : items.CUSTSN_PREFIX;
                                sLOAD_MAC[i].cMAC_POSTFIX = items.CUSTSN_POSTFIX == null ? "" : items.CUSTSN_POSTFIX;
                                sLOAD_MAC[i].cMAC_STR = items.CUSTSN_STR == null ? "" : items.CUSTSN_STR;
                                sLOAD_MAC[i].sMACID_CODE = items.SHIPPINGSN_CODE == null ? "" : items.SHIPPINGSN_CODE;
                                sLOAD_MAC[i].sCHECK_MAC_RULE = items.CHECK_RULE_NAME == null ? "" : items.CHECK_RULE_NAME;
                                sLOAD_MAC[i].sCHECK_MAC = items.CHECK_SSN == null ? "" : items.CHECK_SSN;
                                sLOAD_MAC[i].sCHECK_MAC_RANGE = items.CHECK_RANGE == null ? "" : items.CHECK_RANGE;
                                sLOAD_MAC[i].nMAC_LENGTH = Int32.Parse(items.CUSTSN_LENG);
                                if (sLOAD_MAC[i].cMAC_PREFIX.IndexOf(",") > -1)
                                {
                                    sLOAD_MAC[i].nMAC_PREFIX_LEN = sLOAD_MAC[i].cMAC_PREFIX.IndexOf(",");
                                }
                                else
                                {
                                    sLOAD_MAC[i].nMAC_PREFIX_LEN = (sLOAD_MAC[i].cMAC_PREFIX).Length;
                                }
                                sLOAD_MAC[i].nMAC_POSTFIX_LEN = (sLOAD_MAC[i].cMAC_POSTFIX).Length;
                                sLOAD_MAC[i].sCOMPARE_MAC = items.COMPARE_SN == null ? "" : items.COMPARE_SN;
                                sLOAD_MAC[i].nMAC_Self_StartDigit = items.CUSTSN_START;
                                sLOAD_MAC[i].nMAC_Self_FlowNO = items.CUSTSN_END;
                                sLOAD_MAC[i].nMAC_Compare_StartDigit = items.COMPARE_SN_START;
                                sLOAD_MAC[i].nMAC_Compare_FlowNO = items.COMPARE_SN_END;
                                if (sLOAD_MAC[i].sCHECK_MAC == "Y")
                                {
                                    if (MACID == "")
                                    {
                                        if (await FindMACID(SN,MACID) == false)
                                        {
                                            MACID = "N/A";
                                            MAC1 = "N/A";
                                            MAC2 = "N/A";
                                            MAC3 = "N/A";
                                            SN_OK = false;
                                            return SN + "-" + await GetPubMessage("00403");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //check config43
                    var qry_Config43 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFIS1.C_PACK_SEQUENCE_T"
                           + " WHERE    SUBSTR(CUSTSN_NAME,1,3)<>'II_'  AND MODEL_NAME = '" + Edt_model.Text + "'"
                           + " AND VERSION_CODE = '" + cVersion + "' "
                           + " AND MO_TYPE='" + sMO_TYPE + "'"
                           + " order by SCAN_SEQ",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Config43.Data.Count() == 0)
                    {
                        SN_OK = false;
                        return "20106 - " + await GetPubMessage("20106");
                    }
                    List<PACK_SEQUENCE> datalist = new List<PACK_SEQUENCE>();
                    datalist = qry_Config43.Data.ToListObject<PACK_SEQUENCE>().ToList();
                    SCAN_POS = 0;
                    if (datalist[0].CUSTSN_NAME != "HOUSING_SN")
                    {
                        sequenlist.Add(new SOURCE() { STEP = "HOUSING_SN" });
                    }
                    for (int j = 0; j < datalist.Count; j++)
                    {
                        sequenlist.Add(new SOURCE { STEP = datalist[j].CUSTSN_NAME });
                        getSSNMAC_rule(j);
                        if (sequenlist[j].STEP.IndexOf("MAC") > -1)
                        {
                            item_CheckMacID.IsChecked = true;
                        }
                    }
                    Data_gridView.ItemsSource = sequenlist;
                }

                if (await GETMODELTYPE() == "S")
                {
                    try
                    {
                        var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.PACK_TIMEOUT",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="SN",Value=SN,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                        }
                        });
                        dynamic ads = result.Data;
                        res = ads[0]["res"];
                        if (float.Parse(res) > 0.25)
                        {
                            PackFail("VI01",SN);
                            SN_OK = false;
                            lbError.Text = "00404 - " + await GetPubMessageVN("00404");
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "00404 - " + await GetPubMessage("00404");
                            _er.MessageVietNam = "00404 - " + await GetPubMessageVN("00404");
                            _er.ShowDialog();
                        }
                    }
                    catch (Exception e)
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = e.Message;
                        _er.MessageEnglish = "Call procedure have exceptions:" + e.Message;
                        _er.ShowDialog();
                        strDataInput = "";
                        Inputdata.Focus();
                    }
                }

                if ((sLOAD_MAC[1].sCHECK_MAC == "Y") || (sLOAD_MAC[1].sCOMPARE_MAC == "Y"))
                {
                    if (sMO_TYPE.ToUpper() == "RMA")
                    {
                        CIS_MAC = "";
                        CIS_SSN = "";
                        string strGetNetSys = "select * from sfism4.R_RMA_NETSYS_T"
                            + $" where serial_number = '{SN}'";
                        var qry_NetSys = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText= strGetNetSys,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_NetSys.Data == null)
                        {
                            SN_OK = false;
                            return "00405 - " + await GetPubMessage("00405");
                        }

                        CIS_MAC = qry_NetSys.Data["macid"].ToString();
                        CIS_SSN = qry_NetSys.Data["ssn"].ToString();
                        if (MACID != CIS_MAC)
                        {
                            SN_OK = false;
                            return "00329 - " + await GetPubMessage("00329");
                        }
                    }
                }

                if (result_datar107[0].FINISH_FLAG == "1")
                {
                    Keypart_OK = true;
                }
                else
                {
                    KPNO = await CheckKeyPart(SN,Keypart_OK);
                    if (Keypart_OK == false)
                    {
                        if (item_Assy_input.IsChecked == false)
                        {
                            SN_OK = false;
                            return "00406 - " + await GetPubMessage("00406") + KPNO;
                        }
                    }
                }
            }
            SN_OK = true;
       //     editSerialNumber.Text = SN;
            return "";
        }

        private async Task<string> CheckKeyPart(string SN,bool _Keypart_OK)
        {
            int nKP_RELATION;
            string KPNO;
            Keypart_OK = true;
            KPNO = "";
            string strGetKeyPart = "SELECT KEY_PART_NO,KP_RELATION FROM SFIS1.C_BOM_KEYPART_T WHERE BOM_NO IN"
                + " (SELECT BOM_NO FROM SFISM4.R107 WHERE SERIAL_NUMBER = '" + SN + "') AND KEY_PART_NO NOT IN"
                + " (SELECT KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
                + " WHERE SERIAL_NUMBER = '" + SN + "')";
            var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetKeyPart,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_KeyPart.Data.Count() != 0)
            {
                List<BOM> result_bom = new List<BOM>();
                result_bom = qry_KeyPart.Data.ToListObject<BOM>().ToList();
                nKP_RELATION = result_bom[0].KP_RELATION;

                string strGetCountKP = "SELECT COUNT(*) count FROM SFISM4.R_WIP_KEYPARTS_T"
                    + " WHERE KP_RELATION = '" + nKP_RELATION.ToString() + "' AND SERIAL_NUMBER = '" + SN + "'";
                var qry_CountKP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCountKP,
                    SfcCommandType = SfcCommandType.Text
                });
                if (Int32.Parse(qry_CountKP.Data["count"].ToString()) == 0)
                {
                    Keypart_OK = false;
                    KPNO = KPNO + qry_CountKP.Data["key_part_no"].ToString();
                }
            }
            else
            {
                Keypart_OK = true;
                KPNO = "";
            }
            return KPNO;
        }
        public async Task<string> GETMODELTYPE()
        {
            string strGetModel = $"SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='{smodel_name}'";
            var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText  = strGetModel,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Model.Data == null)
            {
                lbError.Text = "20104 - " + await GetPubMessageVN("20104");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "20104 - " + await GetPubMessage("20104");
                _er.MessageVietNam = "20104 - " + await GetPubMessageVN("20104");
                _er.ShowDialog();
                return "";
            }
            else
            {
                return qry_Model.Data["model_type"].ToString();
            }
        }
        private async Task<bool> FindMACID(string SN,string MACID)
        {
            string LastKPSN;
            bool ContinueFindMACID;

            ContinueFindMACID = false;

            string SQL_STR = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T"
                + " WHERE KEY_PART_NO = 'MACID' AND SERIAL_NUMBER = :SERIAL_NUMBER";
            var query_keypart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = SQL_STR,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="SERIAL_NUMBER",Value=SN}
                }
            });
            if (query_keypart.Data.Count() > 1)
            {
                string SQL_str = "SELECT A.SERIAL_NUMBER,(A.SHIPPING_SN)SHIPPING_SN,(B.KEY_PART_SN)LAN,"
                    + " (C.KEY_PART_SN)WAN,(D.KEY_PART_SN)WIRELESS FROM (SELECT SERIAL_NUMBER,SHIPPING_SN"
                    + " FROM SFISM4.R_WIP_TRACKING_T where (serial_number= :SERIAL_NUMBER) AND ROWNUM<2) A,"
                    + " (SELECT  SERIAL_NUMBER,KEY_PART_SN,KP_CODE FROM  SFISM4.R108"
                    + " WHERE (serial_number= :SERIAL_NUMBER) AND KP_CODE=''LAN'' AND ROWNUM<2)B,"
                    + " (SELECT  SERIAL_NUMBER,KEY_PART_SN,KP_CODE FROM  SFISM4.R108"
                    + " WHERE (serial_number= :SERIAL_NUMBER) AND KP_CODE=''WAN'' AND ROWNUM<2)C,"
                    + " (SELECT  SERIAL_NUMBER,KEY_PART_SN,KP_CODE FROM  SFISM4.R108"
                    + " WHERE (serial_number= :SERIAL_NUMBER) AND KP_CODE=''WIRELESS'' AND ROWNUM<2)D"
                    + " WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER AND A.SERIAL_NUMBER=C.SERIAL_NUMBER"
                    + " AND A.SERIAL_NUMBER=D.SERIAL_NUMBER";
                var query_SQL_str = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = SQL_str,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SERIAL_NUMBER",Value=SN}
                    }
                });
                if (query_SQL_str.Data == null)
                {
                    string SQL = "SELECT A.SERIAL_NUMBER,(A.SHIPPING_SN)SHIPPING_SN,(B.KEY_PART_SN)LAN,"
                        + " (C.KEY_PART_SN)WAN FROM (SELECT SERIAL_NUMBER,SHIPPING_SN"
                        + " FROM SFISM4.R_WIP_TRACKING_T where (serial_number= :SERIAL_NUMBER) AND ROWNUM<2) A,"
                        + " (SELECT  SERIAL_NUMBER,KEY_PART_SN,KP_CODE FROM"
                        + " SFISM4.R108  WHERE (serial_number= :SERIAL_NUMBER)"
                        + " AND KP_CODE=''LAN'' AND ROWNUM<2)B,(SELECT  SERIAL_NUMBER,KEY_PART_SN,KP_CODE"
                        + " FROM  SFISM4.R108  WHERE (serial_number= :SERIAL_NUMBER)"
                        + " AND KP_CODE=''WAN'' AND ROWNUM<2)C  WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER"
                        + " AND A.SERIAL_NUMBER=C.SERIAL_NUMBER";
                    var query_SQL = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = SQL,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="SERIAL_NUMBER",Value=SN}
                        }
                    });
                }
                else
                {
                    MAC3 = query_SQL_str.Data["wireless"].ToString();
                    BOM_HAS_WIRELESS = true;
                }
                if (query_SQL_str.Data != null)
                {
                    MAC1 = query_SQL_str.Data["wan"].ToString();
                    MAC2 = query_SQL_str.Data["lan"].ToString();
                    BOM_HAS_WAN = true;
                    BOM_HAS_LAN = true;
                    BOM_HAS_WIRELESS = false;
                    return true;
                }
            }

            if (query_keypart.Data.Count() == 0)
            {
                string SQL_STr = "SELECT * FROM SFISM4.R_WIP_KEYPARTS_T"
                    + " WHERE KEY_PART_NO <> KEY_PART_SN AND SERIAL_NUMBER = :SERIAL_NUMBER";
                var query_keypart_STr = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = SQL_STr,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SERIAL_NUMBER",Value=SN}
                    }
                });
                if (ContinueFindMACID == true)
                {
                    LastKPSN = query_keypart_STr.Data["key_part_sn"].ToString();
                    if (query_keypart_STr.Data == null)
                    {
                        ContinueFindMACID = false;
                        return false;
                    }
                    else
                    {
                        if (query_keypart_STr.Data["key_part_no"].ToString() == "MACID")
                        {
                            MACID = LastKPSN;
                            BOM_HAS_WAN = false;
                            BOM_HAS_LAN = false;
                            BOM_HAS_WIRELESS = false;
                            ContinueFindMACID = false;
                            return true;
                        }
                        else
                        {
                            SN = LastKPSN;
                        }
                    }
                }
            }
            return false;
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
                        if (mStepdata.IndexOf("SSN" + j.ToString().Trim()) > -1)
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
                        if (mStepdata.IndexOf("SSN" + j.ToString().Trim()) > -1)
                        {
                            break;
                        }
                    }
                }

                sequenlist[mcolumn].PREFIX = sLOAD_SSN[j].cSSN_PREFIX.ToString();
                sequenlist[mcolumn].LENGTH = sLOAD_SSN[j].nSSN_LENGTH.ToString();
            }

            if (mStepdata.IndexOf("MAC") > -1)
            {
                for (j = 1; j <= 5; j++)
                {
                    if (mStepdata.IndexOf("MAC" + j.ToString().Trim()) > -1)
                    {
                        break;
                    }
                }
                sequenlist[mcolumn].PREFIX = sLOAD_MAC[j].cMAC_PREFIX.ToString();
                sequenlist[mcolumn].LENGTH = sLOAD_MAC[j].nMAC_LENGTH.ToString();
            }

        }
        private async Task<int> FindNokiaSn(string SN,int _macfound,string bomkey)
        {
            string C_KPSN, C_KPT_NO, C_FINDMAC;
            string[] mac = new string[21];
            int  j;

            j = 0;

            string strGetKeyPart = "SELECT KEY_PART_SN,KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
               + $" WHERE SERIAL_NUMBER = '{SN}' ORDER BY KP_RELATION ";
            var qry_KeyPart = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetKeyPart,
                SfcCommandType = SfcCommandType.Text
            });
            List<R108> data_r108 = new List<R108>();
            data_r108 = qry_KeyPart.Data.ToListObject<R108>().ToList();
            if (qry_KeyPart.Data.Count() > 0)
            {
                for (j = 0; j < qry_KeyPart.Data.Count(); j++)
                {
                    mac[j] = data_r108[j].KEY_PART_SN.ToString();
                }
            }
            while (j != 0)
            {
                j = j - 1;
                C_KPSN = mac[j];
                string strGetKeyPartSN = "SELECT KEY_PART_SN,KEY_PART_NO,mo_number  FROM SFISM4.R_WIP_KEYPARTS_T"
                     + $" WHERE SERIAL_NUMBER ='{SN}'  AND KEY_PART_SN =  '{C_KPSN}'";
                var qry_KPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetKeyPart,
                    SfcCommandType = SfcCommandType.Text
                });
                C_KPT_NO = qry_KPSN.Data["key_part_no"].ToString();
                if (C_KPT_NO == bomkey)
                {
                    C_FINDMAC = qry_KPSN.Data["key_part_sn"].ToString();
                    macfound = macfound + 1;
                    SMAC[macfound] = C_FINDMAC;
                }
                else
                {
                    C_KPSN = qry_KPSN.Data["key_part_sn"].ToString();
                    if (C_KPSN != "")
                    {
                       await FindNokiaSn(C_KPSN, macfound, bomkey);
                    }
                }
            }
            return macfound;
        }

        private async Task<int> FindMAC(string SN,int _macfound)
        {
            string C_KPSN, C_KPT_NO, C_FINDMAC;
            string[] mac = new string[21];
            int j;

            j = 0;

            string strGetKeyPartSN = "SELECT KEY_PART_SN,KEY_PART_NO FROM SFISM4.R_WIP_KEYPARTS_T"
               + $" WHERE SERIAL_NUMBER = '{SN}'  ORDER BY KP_RELATION ";
            var qry_KeyPartSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetKeyPartSN,
                SfcCommandType = SfcCommandType.Text
            });
            List<R108> data_r108 = new List<R108>();
            data_r108 = qry_KeyPartSN.Data.ToListObject<R108>().ToList();
            if (qry_KeyPartSN.Data.Count() > 0)
            {
                for (j = 0; j < qry_KeyPartSN.Data.Count(); j++)
                {
                    mac[j] = data_r108[j].KEY_PART_SN.ToString();
                }
            }
            while (j != 0)
            {
                j = j - 1;
                C_KPSN = mac[j];
                string strGetKPSN = "SELECT KEY_PART_SN,KEY_PART_NO,mo_number  FROM SFISM4.R_WIP_KEYPARTS_T"
                     + $" WHERE SERIAL_NUMBER ='{SN}'  AND KEY_PART_SN = '{C_KPSN}'";
                var qry_KPSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetKPSN,
                    SfcCommandType = SfcCommandType.Text
                });
                C_KPT_NO = qry_KPSN.Data["key_part_no"].ToString();
                if (C_KPT_NO == "MACID")
                {
                    C_FINDMAC = qry_KPSN.Data["key_part_sn"].ToString();
                    macfound = macfound + 1;
                    SMAC[macfound] = C_FINDMAC;
                }
                else
                {
                    C_KPSN = qry_KPSN.Data["key_part_sn"].ToString();
                    if (C_KPSN != "")
                    {
                        await FindMAC(C_KPSN, macfound);
                    }
                }
            }
            return macfound;
        }
        private async Task<string>  getmodelserial()
        {
            string strGetModel = $"SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='{smodel_name}'";
            var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModel,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Model.Data == null)
            {
                lbError.Text = "20104 - " + await GetPubMessageVN("20104");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "20104 - " + await GetPubMessage("20104");
                _er.MessageVietNam = "20104 - " + await GetPubMessageVN("20104");
                _er.ShowDialog();
            }
            else
            {
                return qry_Model.Data["model_serial"].ToString();
            }
            return qry_Model.Data["model_serial"].ToString();
        }
        private async Task<bool> chkMO(string _MO_Status)
        {
            string sBOM_NO;
            string strGetDataMO = "SELECT MO_TYPE,CLOSE_FLAG,BOM_NO FROM SFISM4.R105"
                + $" WHERE MO_NUMBER = (SELECT MO_NUMBER FROM SFISM4.R107 WHERE shipping_sn = '{strDataInput}' or SERIAL_NUMBER = '{strDataInput}')";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text
            });
            List<R105> result_r105 = qry_DataMO.Data.ToListObject<R105>().ToList();
            sBOM_NO = result_r105[0].BOM_NO;
            MO_BOM_NO = sBOM_NO;
            sMO_TYPE = result_r105[0].MO_TYPE;
            if (result_r105[0].CLOSE_FLAG != "2")
            {
                Result_MO_Status = result_r105[0].CLOSE_FLAG;
                return true;
            }
            string strGetBOMNO = "SELECT KEY_PART_NO FROM SFIS1.C_BOM_KEYPART_T"
                + $" WHERE BOM_NO = '{sBOM_NO}' AND KEY_PART_NO='MACID'";
            var qry_BOMNO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetBOMNO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_BOMNO.Data.Count() > 0)
            {
                List<BOM> result_bom = new List<BOM>();
                result_bom = qry_BOMNO.Data.ToListObject<BOM>().ToList();
                if (result_bom[0].KEY_PART_NO == "MACID")
                {
                    BOM_HAS_MAC = true;
                }
            }
            return false;
        }
        public async Task<bool> checkRoute()
        {
            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.CHECK_ROUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="DATA", Value=strDataInput,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MYGROUP",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter{Name="LINE",Value=Edt_linename.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
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
        private async Task<bool> FGetBoxIDBySP(string paramBoxID1)
        {
            string BOXID,  cust_code, model_name, version_code, yww_str, ubntboxid;
            string box_prefix, box_str, box_last_boxid;
            bool Exist_Flag, NEW_FLAG, GETBOXID_FLAG;
            int iCapacity, box_leng, trayqty, Qty;

            BOXID = "";
            Exist_Flag = false;
            box_prefix = "";
            box_str = "";
            box_leng = 0;
            model_name = "";
            version_code = "";

            string strGetDataWIP = "select * from sfism4.r_wip_tracking_t"
                + " where shipping_sn = '" + editSerialNumber.Text + "' or serial_number= '" + editSerialNumber.Text + "'";
            var qry_DataWIP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataWIP,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_DataWIP.Data != null)
            {
                cust_code = qry_DataWIP.Data["customer_no"].ToString();
                model_name = qry_DataWIP.Data["model_name"].ToString();
                version_code = qry_DataWIP.Data["version_code"].ToString();
            }
            else
            {
                return false;
            }

            string strGetModelSite = "select * from sfis1.c_model_site_t"
                + " where model_name= '" + model_name + "'";
            var qry_ModelSite = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModelSite,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ModelSite.Data != null)
            {
                modelsite = qry_ModelSite.Data["site"].ToString();
            }

            string strGetC19 = "select * from sfis1.C_Cust_SNRule_t"
               + " where cust_code = '" + cust_code + "'"
               + " and model_name = '" + model_name + "'"
               + " and version_code = '" + version_code + "'";
            var qry_C19 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetC19,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_C19.Data != null)
            {
                
                box_prefix = qry_C19.Data["cust_box_prefix"]?.ToString() ?? "";

                box_str = qry_C19.Data["cust_box_str"]?.ToString()?? "";
                box_leng = Int32.Parse(qry_C19.Data["cust_box_leng"]?.ToString()??"");
                box_last_boxid = qry_C19.Data["cust_last_box"]?.ToString()??"";
                if ((box_prefix != "") && (box_leng > 0))
                {
                    if (box_str == "")
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            iCapacity = await getCapacity();
            if ((MODEL_TYPE.IndexOf("190") > -1))
            {
                string strGetTrayQty = "select tray_qty from sfis1.c_pack_param_t c, sfism4.r_mo_base_t r"
                    + " where c.model_name = r.model_name"
                    + " and c.version_code = r.version_code"
                    + " and r.mo_number = '" + Edt_monumber.Text + "' and rownum = 1";
                var qry_TrayQty = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetTrayQty,
                    SfcCommandType = SfcCommandType.Text
                });
                List<PACK_PARAM> result_pack_param = new List<PACK_PARAM>();
                result_pack_param = qry_TrayQty.Data.ToListObject<PACK_PARAM>().ToList();
                if ((qry_TrayQty.Data.Count() == 0) || (result_pack_param[0].TRAY_QTY.ToString() == "") || (result_pack_param[0].TRAY_QTY == 0))
                {
                    lbError.Text = "Set tray_qty(config 15)";
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = "Set tray_qty(config 15)";
                    _er.MessageVietNam = "Thiet lap tray_qty trong Config 15";
                    _er.ShowDialog();
                    return false; 
                }
                trayqty = result_pack_param[0].TRAY_QTY;
                iCapacity = iCapacity * trayqty;
            }

            Qty = 0;
            NEW_FLAG = false;
            string strGetPallet = "select * from sfis1.c_pallet_t where  CLOSE_FLAG='BOXID'"
                            + " and LINE_NAME='" + Edt_linename.Text + "' AND  MO_NUMBER='" + MO + "'";
            var qry_Pallet = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetPallet,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Pallet.Data != null)
            {
                BOXID = qry_Pallet.Data["pallet_no"].ToString();
                if (box_prefix.Substring(box_prefix.Length -3,3) == "YWW")
                {
                    if (box_prefix.Substring(box_prefix.Length -4,4) == "YYMM")
                    {
                        yww_str = BOXID.Substring(box_prefix.Length - 4, 4);
                        if (yww_str != S_YYWW)
                        {
                            NEW_FLAG = true;
                            await delete_pallet_ini(BOXID);
                        }
                    }
                    else
                    {
                        yww_str = BOXID.Substring(box_prefix.Length - 3, 3);
                        if (yww_str != await GetSysWeek())
                        {
                            NEW_FLAG = true;
                            await delete_pallet_ini(BOXID);
                        }
                    }
                }
                else if (box_prefix.Substring(box_prefix.Length - 4, 4) == "YYMM")
                {
                    yww_str = BOXID.Substring(box_prefix.Length - 4, 4);
                    if (yww_str != S_YM)
                    {
                        NEW_FLAG = true;
                        await delete_pallet_ini(BOXID);
                    }
                }
                if (NEW_FLAG == false)
                {
                    string strGetQtyTrayNo = "select count(*) qty from sfism4.r_wip_tracking_t where tray_no='" + BOXID + "'";
                    var qry_QtyTrayNo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetQtyTrayNo,
                        SfcCommandType = SfcCommandType.Text
                    });
                    Qty = Int32.Parse(qry_QtyTrayNo.Data["qty"].ToString());
                    if (Qty > 0)
                    {
                        if (Qty >= iCapacity)
                        {
                            NEW_FLAG = true;
                            await delete_pallet_ini(BOXID);
                        }
                        else
                        {
                            Exist_Flag = true;
                        }
                    }
                    else
                    {
                        NEW_FLAG = true;
                        await delete_pallet_ini(BOXID);
                    }
                }
            }
            else
            {
                NEW_FLAG = true;
            }

            if (Exist_Flag == true)
            {
                if (modelsite == "UBTT")
                {
                    ubntboxid = BOXID + "_B";
                    string strGetRPallet = "select * from SFISM4.R_PALLET_T  WHERE CLOSE_FLAG='BOXID' AND  CARTON_NO='" + ubntboxid + "' and macid='" + Edt_local.Text + "'";
                    var qry_RPallet = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetRPallet,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_RPallet.Data != null)
                    {
                        MBOXID = BOXID;
                        return true;
                    }
                    else
                    {
                        NEW_FLAG = true;
                    }
                }
                else
                {
                    string strGetRPallet = "select * from SFISM4.R_PALLET_T  WHERE CLOSE_FLAG='BOXID' AND  CARTON_NO='" + BOXID + "' and macid='" + Edt_local.Text + "'";
                    var qry_RPallet = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetRPallet,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_RPallet.Data != null)
                    {
                        MBOXID = BOXID;
                        return true;
                    }
                    else
                    {
                        NEW_FLAG = true;
                    }
                }
            }

            GETBOXID_FLAG = false;
            if (NEW_FLAG == true)
            {
                if (GETBOXID_FLAG == false)
                {
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.FIND_BOXID_RULE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name="CUST",Value=cust_code,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                            new SfcParameter {Name="MODEL",Value=model_name,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                            new SfcParameter {Name="VER",Value=version_code,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                            new SfcParameter {Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                        }
                    });
                    dynamic ads = result.Data;
                    string STRTEMP = ads[0]["res"];
                    if (STRTEMP == "BADSN")
                    {
                        lbError.Text = "00084 - " + await GetPubMessageVN("00084");
                        Inputdata.Focus();
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00084 - " + await GetPubMessage("00084");
                        _er.MessageVietNam = "00084 - " + await GetPubMessageVN("00084");
                        _er.ShowDialog();
                        return false;
                    }
                    else
                    {
                        if (STRTEMP == "BADSN1")
                        {
                            lbError.Text = "00681 - " + await GetPubMessageVN("00681");
                            Inputdata.Focus();
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "00681 - " + await GetPubMessage("00681");
                            _er.MessageVietNam = "00681 - " + await GetPubMessageVN("00681");
                            _er.ShowDialog();
                            return false;
                        }
                        else
                        {
                            BOXID = STRTEMP;
                        }
                    }
                    if (BOXID != "" && BOXID != null)
                    {
                        string strGetTrayNO = "select * from sfism4.r_wip_tracking_t where tray_no='" + BOXID + "'";
                        var qry_TrayNO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetTrayNO,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_TrayNO.Data != null)
                        {
                            GETBOXID_FLAG = false;
                            lbError.Text = BOXID+" - 00374 - " + await GetPubMessage("00374");
                            Inputdata.Focus();
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = BOXID + " - 00374 - " + await GetPubMessage("00374");
                            _er.MessageVietNam = BOXID + " - 00374 - " + await GetPubMessage("00374");
                            _er.ShowDialog();
                            return false;
                        }
                        else
                        {
                            if (modelsite == "UBNT")
                            {
                                ubntboxid = BOXID + "_B";
                                string strGetPallet2 = "select * from SFISM4.R_PALLET_T  WHERE CLOSE_FLAG='BOXID' AND  carton_no='" + ubntboxid + "'";
                                var qry_Pallet2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetPallet2,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_Pallet2.Data != null)
                                {
                                    GETBOXID_FLAG = false;
                                    lbError.Text = "00374 - " + await GetPubMessageVN("00374");
                                    Inputdata.Focus();
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "00374 - " + await GetPubMessage("00374");
                                    _er.MessageVietNam = "00374 - " + await GetPubMessageVN("00374");
                                    _er.ShowDialog();
                                    return false;
                                }
                                else
                                {
                                    try
                                    {
                                        if (modelsite == "UBTT")
                                        {
                                            string strInsert = "Insert into sfism4.r_pallet_t(CARTON_NO,MCARTON_NO,CLOSE_FLAG,MACID,IN_STATION_TIME) values(:boxid,:boxid,:close_flag,:macid,sysdate)";
                                            var Insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strInsert,
                                                SfcCommandType = SfcCommandType.Text,
                                                SfcParameters = new List<SfcParameter>()
                                                {
                                                    new SfcParameter{Name="boxid",Value=BOXID+"_B"},
                                                    new SfcParameter{Name="close_flag",Value="BOXID"},
                                                    new SfcParameter{Name="macid",Value=Edt_local.Text},
                                                }
                                            });
                                        }
                                        else
                                        {
                                            string strInsert2 = "Insert into sfism4.r_pallet_t(CARTON_NO,MCARTON_NO,CLOSE_FLAG,MACID,IN_STATION_TIME) values(:boxid,:boxid,:close_flag,:macid,sysdate)";
                                            var Insert2 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strInsert2,
                                                SfcCommandType = SfcCommandType.Text,
                                                SfcParameters = new List<SfcParameter>()
                                                {
                                                    new SfcParameter{Name="boxid",Value=BOXID},
                                                    new SfcParameter{Name="close_flag",Value="BOXID"},
                                                    new SfcParameter{Name="macid",Value=Edt_local.Text}
                                                }
                                            });
                                        }
                                        string strInsert_CPallet = "Insert into sfis1.c_pallet_t(LINE_NAME,MO_NUMBER,MODEL_NAME,PALLET_NO,CUST_PALLET_NO,TARGET,QTY,CLOSE_FLAG)"
                                            + " values(:line,:mo,:model,:boxid,:boxid,:tqty,:qty,:close_flag)";
                                        var Insert_CPallet = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strInsert_CPallet,
                                            SfcCommandType = SfcCommandType.Text,
                                            SfcParameters = new List<SfcParameter>()
                                            {
                                                new SfcParameter{Name="boxid",Value=BOXID},
                                                new SfcParameter{Name="close_flag",Value="BOXID"},
                                                new SfcParameter{Name="qty",Value=0},
                                                new SfcParameter{Name="line",Value=Edt_linename.Text},
                                                new SfcParameter{Name="mo",Value=MO},
                                                new SfcParameter{Name="model",Value=model_name},
                                                new SfcParameter{Name="tqty",Value=iCapacity}
                                            }
                                        });
                                        GETBOXID_FLAG = true;
                                        MBOXID = BOXID;
                                        return true;
                                    }
                                    catch
                                    {
                                        GETBOXID_FLAG = false;
                                    }
                                }
                            }
                            else
                            {
                                string strGetRPallet2 = "select * from SFISM4.R_PALLET_T  WHERE CLOSE_FLAG='BOXID' AND  carton_no='" + BOXID + "'";
                                var qry_RPallet2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetRPallet2,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_RPallet2.Data != null)
                                {
                                    GETBOXID_FLAG = false;
                                    lbError.Text = "00374 - " + await GetPubMessageVN("00374");
                                    Inputdata.Focus();
                                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                                    _er.CustomFlag = true;
                                    _er.MessageEnglish = "00374 - " + await GetPubMessage("00374");
                                    _er.MessageVietNam = "00374 - " + await GetPubMessageVN("00374");
                                    _er.ShowDialog();
                                    return false;
                                }
                                else
                                {
                                    try
                                    {
                                        if (modelsite == "UBTT")
                                        {
                                            string strInsert = "Insert into sfism4.r_pallet_t(CARTON_NO,MCARTON_NO,CLOSE_FLAG,MACID,IN_STATION_TIME) values(:boxid,:boxid,:close_flag,:macid,sysdate)";
                                            var Insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strInsert,
                                                SfcCommandType = SfcCommandType.Text,
                                                SfcParameters = new List<SfcParameter>()
                                                {
                                                    new SfcParameter{Name="boxid",Value=BOXID+"_B"},
                                                    new SfcParameter{Name="close_flag",Value="BOXID"},
                                                    new SfcParameter{Name="macid",Value=Edt_local.Text},
                                                }
                                            });
                                        }
                                        else
                                        {
                                            string strInsert2 = "Insert into sfism4.r_pallet_t(CARTON_NO,MCARTON_NO,CLOSE_FLAG,MACID,IN_STATION_TIME) values(:boxid,:boxid,:close_flag,:macid,sysdate)";
                                            var Insert2 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strInsert2,
                                                SfcCommandType = SfcCommandType.Text,
                                                SfcParameters = new List<SfcParameter>()
                                                {
                                                    new SfcParameter{Name="boxid",Value=BOXID},
                                                    new SfcParameter{Name="close_flag",Value="BOXID"},
                                                    new SfcParameter{Name="macid",Value=Edt_local.Text}
                                                }
                                            });
                                        }
                                        string strInsertCPallet = "Insert into sfis1.c_pallet_t(LINE_NAME,MO_NUMBER,MODEL_NAME,PALLET_NO,CUST_PALLET_NO,TARGET,QTY,CLOSE_FLAG)"
                                            + " values(:line,:mo,:model,:boxid,:boxid,:tqty,:qty,:close_flag)";
                                        var InsertCPallet = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strInsertCPallet,
                                            SfcCommandType = SfcCommandType.Text,
                                            SfcParameters = new List<SfcParameter>()
                                            {
                                                new SfcParameter{Name="boxid",Value=BOXID},
                                                new SfcParameter{Name="close_flag",Value="BOXID"},
                                                new SfcParameter{Name="qty",Value=0},
                                                new SfcParameter{Name="line",Value=Edt_linename.Text},
                                                new SfcParameter{Name="mo",Value=MO},
                                                new SfcParameter{Name="model",Value=model_name},
                                                new SfcParameter{Name="tqty",Value=iCapacity}
                                            }
                                        });
                                        GETBOXID_FLAG = true;
                                        MBOXID = BOXID;
                                        return true;
                                    }
                                    catch
                                    {
                                        GETBOXID_FLAG = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return true;
        }
        private async Task updateR107_hold2(string _hold_flag2)
        {
            string TEMP_SN;

            TEMP_SN = editSerialNumber.Text;
            string strGetReTestLiMit = "select retest_limit from sfis1.c_model_ate_set_t"
                + " where model_name='" + Edt_model.Text + "' and group_name='PACK_BOX'";
            var qry_ReTestLiMit = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetReTestLiMit,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ReTestLiMit.Data != null)
            {
                RETEST_NUM = Int32.Parse(qry_ReTestLiMit.Data["retest_limit"].ToString());
                string strGetRTCount = "SELECT retest_count FROM SFISM4.R_TMP_ATEDATA_T"
                    + " where serial_number='" + editSerialNumber.Text + "'"
                    + " and group_name='" + lbTitle.Content.ToString() + "'  and  station_name='" + Next_Step + "'";
                var qry_RTCount = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetRTCount,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_RTCount.Data != null)
                {
                    ERROR_NUM = Int32.Parse(qry_RTCount.Data["retest_count"].ToString());
                    ERROR_NUM = ERROR_NUM + 1;
                    if (ERROR_NUM == RETEST_NUM)
                    {
                        updateR107_hold(_hold_flag2);
                        await callTestInput();
                        lbError.Text = "00135 - " + await GetPubMessage("00135");
                        CHECKSN_OK = false;
                        for (int i = 1; i <= 15; i++)
                        {
                            sLOAD_SSN[i].SSN_OK = false;
                            sLOAD_MAC[i].MAC_OK = false;
                            sLOAD_SSN[i].SSN = "";
                            sLOAD_MAC[i].MAC = "";
                        }
                        editSerialNumber.Text = "";
                        sequenlist.Clear();
                        Data_gridView.ItemsSource = null;
                        Next_Step = "";
                        lst_ListBox1.Items.Remove(lst_ListBox1.Items.IndexOf(TEMP_SN));
                    }
                }
                else
                {
                    try
                    {
                        var Insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "Insert into SFISM4.R_TMP_ATEDATA_T"
                            + " (EMP_NO, MO_NUMBER, MODEL_NAME, SERIAL_NUMBER, LINE_NAME,"
                            + " SECTION_NAME, GROUP_NAME, STATION_NAME, RETEST_COUNT, IN_STATION_TIME)"
                            + " Values"
                            + " ('" + empNo + "', '" + Edt_monumber.Text + "', '" + Edt_model.Text + "', '" + editSerialNumber.Text + "', '" + Edt_linename.Text + "',"
                            + " '" + Edt_section.Text + "', '" + lbTitle.Content.ToString() + "', '" + Next_Step + "', 1, :time)",
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

        private async void updateR107_hold(string _hold_flag)
        {
            try
            {
                var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "update sfism4.r_wip_tracking_t set group_name='HOLD'||group_name,wip_group='HOLD'||wip_group,"
                    + " next_station='HOLD'||next_station,pmcc='" + _hold_flag + "',in_station_time = :STATION_TIME"
                    + " where serial_number = '" + editSerialNumber.Text + "'",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter{Name="STATION_TIME",Value=G_sBuildDate}
                        }
                });

                var Insert_Log = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "insert into SFISM4.R_SYSTEM_LOG_T"
                    + " (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC, TIME)"
                    + " Values"
                    + " ('" + empNo + "','" + lbTitle.Content.ToString() + "','" + Next_Step + "'"
                    + " '" + editSerialNumber.Text + "'||':'||'" + SCAN_MAC + "'||''; PMCC='||'" + _hold_flag + "'||"
                    + " '(PMCC-->1:timehold; 2:mac not match; 3:mac length error 3times; 4:box id not match mac 3times); IP:''||'" + local_strIP + "',:time)",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{Name="time",Value=G_sBuildDate}
                    }
                });

                var Delete = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "delete FROM SFISM4.R_TMP_ATEDATA_T "
                    + " where serial_number='" + editSerialNumber.Text + "'"
                    + " and group_name='" + lbTitle.Content.ToString() + "'",
                    SfcCommandType = SfcCommandType.Text
                });
            }
            catch (Exception)
            {
                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private async void PackFail(string _EC,string _SN)
        {
            string sWorkSection;
            try
            {
                var query_work = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "select * from sfis1.c_work_desc_t"
                    + " where start_time <= :sTime"
                    + " and end_time >= :sTime"
                    + " and line_name = :sLine",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="sTime",Value=System.DateTime.Now.ToString("hhmm") },
                        new SfcParameter {Name="sLine",Value=Edt_linename.Text }
                    }
                });
                if (query_work.Data.Count() != 0)
                {
                    sWorkSection = query_work.Data["work_section"].ToString();
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
                        new SfcParameter {Name="Line",Value=Edt_linename.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Section",Value=M_sThisSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="W_Station",Value=M_sThisStation,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="DateTime",Value= G_sBuildDate,SfcParameterDataType=SfcParameterDataType.Date,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="EC",Value="N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Mo_Date",Value=System.DateTime.Now.ToString("yyyymmdd"),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="MyGroup",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="W_Section",Value=sWorkSection,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="Data",Value=editSerialNumber.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter {Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
            }
            catch (Exception e)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = e.Message;
                _er.MessageEnglish = e.Message;
                _er.ShowDialog();
                return;
            }
        }
        private async Task InsertR102ByTray(int _TRAY_QTY)
        {
            string sWorkSection;

            string strGetTime = "select * from sfis1.c_work_desc_t"
                + " where start_time <= :sTime"
                + " and end_time >= :sTime"
                + " and line_name = :sLine";
            var qry_Time = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetTime,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="sTime",Value=G_sBuildDate},
                    new SfcParameter{Name="sLine",Value=Edt_linename.Text}
                }
            });
            if (qry_Time.Data != null)
            {
                sWorkSection = qry_Time.Data["work_section"].ToString();
            }
            else
            {
                sWorkSection = "X";
            }
            try
            {
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.INSERT_R102_QTY",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="LINE",Value=Edt_linename.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="SECTION",Value=M_sThisSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value=M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MO",Value=MO_R102,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MO_DATE",Value=G_sBuildDate,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="W_SECTION",Value=sWorkSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="C_QTY",Value=_TRAY_QTY,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="F_FLAG",Value="0",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    }
                });
                //var result1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                //{
                //    CommandText = "SFIS1.STN_REC ",
                //    SfcCommandType = SfcCommandType.StoredProcedure,
                //    SfcParameters = new List<SfcParameter>()
                //        {
                //            new SfcParameter{Name="line",Value=Edt_linename.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                //            new SfcParameter{Name="section",Value=M_sThisSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                //            new SfcParameter{Name="mygroup",Value="COMPARE_LABEL",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                //            new SfcParameter{Name="w_station",Value=item.W_STATION,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                //            new SfcParameter{Name="mo",Value="",SfcParameterDataType=SfcParameterDataType.Date,SfcParameterDirection=SfcParameterDirection.Input },
                //            new SfcParameter{Name="sn",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                //            new SfcParameter{Name="mo_date",Value=G_sBuildDate,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                //            new SfcParameter{Name="w_section",Value=sWorkSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            
                //            new SfcParameter{Name="f_flag",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                //        }
                //});
            }
            catch (Exception e)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = e.Message;
                _er.MessageEnglish = e.Message;
                _er.ShowDialog();
                return;
            }
           
        }
        private async Task<bool> checkMoTarget()
        {
            int iTarget, iMoCount;

            string strGetTargetQty = "select target_qty from sfism4.r_mo_base_t"
                + " where mo_number = '" + Edt_monumber.Text + "'";
            var qry_TargetQty = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetTargetQty,
                SfcCommandType = SfcCommandType.Text
            });
            iTarget = Int32.Parse(qry_TargetQty.Data["target_qty"].ToString());

            string strGetCountSN = "select count(serial_number) sncount"
               + " from sfism4.r_wip_tracking_t"
               + $" where mo_number = '{Edt_monumber.Text}'"
               + " and tray_no <> 'N/A'";
            var qry_CountSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCountSN,
                SfcCommandType = SfcCommandType.Text
            });
            iMoCount = Int32.Parse(qry_CountSN.Data["sncount"].ToString());

            if (iMoCount >= iTarget)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<string> GetSysWeek()
        {
            string strGetSysWeek = "select to_char(sysdate, 'iiw') sysweek from dual";
            var qry_SysWeek = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetSysWeek,
                SfcCommandType = SfcCommandType.Text
            });
            return qry_SysWeek.Data["sysweek"].ToString();
        }
        private async Task updatePalletFull()
        {
            try
            {
                string strUpdate = "update sfism4.r_wip_tracking_t"
                               + " set pallet_full_flag = 'Y'"
                               + " where tray_no = '" + MBOXID + "'";
                var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
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
        private async Task delete_pallet_ini(string pallet)
        {
            try
            {
                string strDelete = $"delete from sfis1.c_pallet_t where pallet_no='{pallet}'";
                var Delete = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = strDelete,
                    SfcCommandType = SfcCommandType.Text
                });
                btn_CloseBoxID.IsEnabled = false;
            }
            catch (Exception)
            {
                MessageBox.Show($"00042 - {await GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private async Task<string> CheckTRAY(string TRAY,bool _CHECKTRAY_OK)
        {
            string MO_Status;
            int iCapacity;
            //MO_Status = "";
            if (!string.IsNullOrEmpty(TRAY))
            {
                string strGetTrayNO = "select DISTINCT WIP_GROUP from sfism4.r_wip_tracking_t"
                    + $" where TRAY_NO = '{TRAY}' OR TRACK_NO='{TRAY}'";
                var query_r107 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetTrayNO,
                    SfcCommandType = SfcCommandType.Text
                });
                if (query_r107.Data.Count() != 1)
                {
                    CHECKTRAY_OK = false;
                    return "00678 - " + await GetPubMessage("00678");
                }
                else
                {
                    List<R107> result_r107 = new List<R107>();
                    result_r107 = query_r107.Data.ToListObject<R107>().ToList();
                    if (result_r107[0].WIP_GROUP != M_sThisGroup)
                    {
                        CHECKTRAY_OK = false;
                        return await GetPubMessage("00679") + result_r107[0].WIP_GROUP;
                    }
                }

                // check scrap
                string strGetScrap = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE (TRAY_NO='{TRAY}' OR TRACK_NO='{TRAY}') AND SCRAP_FLAG<>'0'";
                var qry_Scrap = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetScrap,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Scrap.Data.Count() > 0)
                {
                    CHECKTRAY_OK = false;
                    return "00401 - " + await GetPubMessage("00401");
                }

                // check ERROR
                string strGetError = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE (TRAY_NO='{TRAY}' OR TRACK_NO='{TRAY}') AND ERROR_FLAG<>'0'";
                var qry_Error = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetError,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Error.Data.Count() > 0)
                {
                    CHECKTRAY_OK = false;
                    return "00401 - " + await GetPubMessage("00401");
                }

                string strGetDataTray = $"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE  TRAY_NO='{TRAY}' OR TRACK_NO='{TRAY}' AND ROWNUM=1";
                var qry_DataTray = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetDataTray,
                    SfcCommandType = SfcCommandType.Text
                });
                MO = qry_DataTray.Data["mo_number"].ToString();
                MO_R102 = MO;
                if ((qry_DataTray.Data["tray_no"].ToString() != "N/A") && (qry_DataTray.Data["tray_no"].ToString() != ""))
                {
                    if ((qry_DataTray.Data["track_no"].ToString() != "N/A") && (qry_DataTray.Data["track_no"].ToString() != ""))
                    {
                        CHECKTRAY_OK = false;
                        return "00400 - " + await GetPubMessage("00400") + qry_DataTray.Data["tray_no"].ToString();
                    }
                }
                CHECKMOTYPE();
                // check the same MO
                if ((Edt_monumber.Text != "") && (Edt_monumber.Text != MO))
                {
                    if (IsPackingByMoElseByModel == true)
                    {
                        CHECKTRAY_OK = false;
                        return "00402 - " + await GetPubMessage("00402");
                    }
                }

                Edt_monumber.Text = qry_DataTray.Data["mo_number"].ToString();
                Edt_model.Text = qry_DataTray.Data["model_name"].ToString();
                smodel_name = qry_DataTray.Data["model_name"].ToString();
                Edt_section.Text = qry_DataTray.Data["section_name"].ToString();
                Edt_group.Text = qry_DataTray.Data["group_name"].ToString();
                Edt_station.Text = qry_DataTray.Data["station_name"].ToString();

                //check mo status
                string strGetStatusMO = $"SELECT MO_TYPE,CLOSE_FLAG FROM SFISM4.R105 WHERE MO_NUMBER='{MO}'";
                var qry_StatusMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetStatusMO,
                    SfcCommandType = SfcCommandType.Text
                });
                MO_Status = qry_StatusMO.Data["close_flag"].ToString();
                if (MO_Status == "1")
                {
                    CHECKTRAY_OK = false;
                    return "00012 - " + await GetPubMessage("00012");
                }
                if (MO_Status == "3")
                {
                    CHECKTRAY_OK = false;
                    return "00013 - " + await GetPubMessage("00013");
                }
                if (MO_Status == "5")
                {
                    CHECKTRAY_OK = false;
                    return "00014 - " + await GetPubMessage("00014");
                }
                if (MO_Status == "6")
                {
                    CHECKTRAY_OK = false;
                    return "00015 - " + await GetPubMessage("00015");
                }
                if (MO_Status == "")
                {
                    CHECKTRAY_OK = false;
                    return "40001 - " + await GetPubMessage("40001");
                }
                iCapacity = await getCapacity();
                if ((iCapacity == 0) || (iCapacity == -1))
                {
                    CHECKTRAY_OK = false;
                    return "00111 - " + await GetPubMessage("00111");
                }
                lb_Capacity.Content = iCapacity.ToString();
                CHECKTRAY_OK = true;
            }
            return "";
        }
        private async Task<int> getCapacity()
        {
            string strGetParam = "select tray_qty,coo from sfis1.c_pack_param_t c, sfism4.r_mo_base_t r"
                + " where c.model_name = r.model_name"
                + " and c.version_code = r.version_code"
                + $" and r.mo_number = '{Edt_monumber.Text}' and rownum=1";
            var qry_GetParam = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetParam,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_GetParam.Data == null)
            {
                return -1;
            }
            else
            {
                if (lbTitle.Content.ToString().Contains("PACK_BOX") && Edt_model.Text.Contains("J20H100"))
                {
                    return int.Parse(qry_GetParam.Data["coo"].ToString());
                }
                return  int.Parse(qry_GetParam.Data["tray_qty"].ToString());
            }
        }
        private async void CHECKMOTYPE()
        {
            string SAPMO_NUMBER, SAPMO_TYPE, MO_TYPE_FRONT;
            string strGetDataWIP = "Select * From SFISM4.R_wip_tracking_T"
                + $" Where SERIAL_NUMBER='{strDataInput}' OR SHIPPING_SN2='{strDataInput}' OR SHIPPING_SN='{strDataInput}' OR TRAY_NO='{strDataInput}' AND ROWNUM=1";
            var qry_DataWIP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataWIP,
                SfcCommandType = SfcCommandType.Text
            });

            if(qry_DataWIP.Data != null)
            {
                SAPMO_NUMBER = qry_DataWIP.Data["mo_number"].ToString();

                string strGetDataMO = "Select b.vr_class From SFISM4.R_BPCS_MOPLAN_T A,sfis1.C_PARAMETER_INI B"
                    + $" where a.sap_mo_type=b.vr_item and b.PRG_NAME='WORKTYPE' and a.MO_NUMBER='{SAPMO_NUMBER}' AND A.SITE=B.VR_NAME";
                var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_DataMO.Data != null)
                {
                    SAPMO_TYPE = qry_DataMO.Data["vr_class"].ToString();
                }

                string strGetDataMO2 = "select b.vr_class From SFISM4.R_BPCS_MOPLAN_T  A,sfis1.C_PARAMETER_INI B"
                    + $" where a.sap_mo_type=b.vr_item and b.PRG_NAME='WORKTYPE' and  a.MO_NUMBER='{Edt_monumber.Text}' AND A.SITE=B.VR_NAME";
                var qry_DataMO2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetDataMO2,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_DataMO2.Data != null)
                {
                    MO_TYPE_FRONT = qry_DataMO2.Data["vr_class"].ToString();
                }
                if (MODEL_TYPE.IndexOf("A") > -1)
                {
                    IsPackingByMoElseByModel = false;
                }
                else
                {
                    IsPackingByMoElseByModel = true;
                }
            } 
        }
        private async void mainWindow_Initialized(object sender, EventArgs e)
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
            loginApiUri = argsInfor[1].ToString();
            loginDB = argsInfor[2].ToString();
            empNo = argsInfor[3].ToString();
            empPass = argsInfor[4].ToString();
            string temp = GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location);
            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //    //Environment.Exit(0);
            //}
            sfcClient = DBApi.sfcClient(loginDB, loginApiUri);
            await sfcClient.GetAccessTokenAsync(empNo, empPass);
            edtpassword.Focus();
            edtpassword.IsEnabled = true;
            Inputdata.IsEnabled = false;
            FormCreate();

            killprocess();
            labApp = new LabelManager2.Application();
            foreach (string sFile in System.IO.Directory.GetFiles(_DirPath, "*.LAB"))
            {
                try
                {
                    File.Delete(sFile);
                }
                catch (Exception ex)
                {
                    string errorMessage = "Cannot kill process " + " Exception: " + ex.Message + Environment.NewLine + "Please Close program and try again";
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Không thể xóa đóng process { 0}. Lỗi ngoại lệ: { 1} Hãy đóng chương trình và thử lại ";
                    _sh.MessageEnglish = errorMessage + ex.Message;
                    _sh.ShowDialog();
                    return;
                }
            }
            //------------------------Start get Emp 
            
            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "PACKING",
                UserName = empNo,
                Password = empPass
            };

            //Tranform it to Json object
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
                    empName = digits[1].Split('-')[1].ToString().Trim();
                    Edt_Database.Text = digits[0].ToString();
                    edtpassword.Text = empName;
                    Edt_User.Text = empNo + " - " + empName;
                    Edt_Ver.Text = getRunningVersion().ToString();
                    Edt_IP.Text = localIP().ToString();
                    Edt_local.Text = GetHostMacAddress().ToString();
                    Inputdata.Focus();
                    Inputdata.IsEnabled = true;
                    edtpassword.IsEnabled = false;
                }
                else
                {
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = Ok;
                    _er.MessageVietNam = Ok;
                    _er.ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = ex.Message;
                _er.MessageVietNam = ex.Message;
                _er.ShowDialog();
                this.Close();
            }

            for (int i = 1; i <= 15; i++)
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

            for (int i = 1; i <= 15; i++)
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

            item_Reprint.IsEnabled = true;
            if (item_Tori_2G.IsChecked == false)
            {
                lbError.Text = await GetPubMessage("00008");
            }
            Inputdata.Focus();
            
        }
        private async void FormCreate()
        {
            if (!Directory.Exists("C:\\PACKING"))
            {
                Directory.CreateDirectory("C:\\PACKING");
            }
            ap_version= getRunningVersion().ToString();
            INIFile ini = new INIFile("C:\\PACKING\\Packing.ini");
            M_sThisSection = ini.Read("PrePacking", "Section");
            M_sThisGroup = ini.Read("PrePacking", "Group");
            M_sThisStation = ini.Read("PrePacking", "Station");
            M_sThisLineName = ini.Read("PACKY", "LINE");
            if (string.IsNullOrEmpty(M_sThisGroup))
            {
                ini.Write("PACKY", "IsChangeLine", "false");
                FormShow();
                ini.Write("PrePacking", "CBREWORK", Cb_Rework.IsChecked.ToString());
                ini.Write("PrePacking", "FORTMAC", Cb_Fortmac.IsChecked.ToString());
                ini.Write("PrePacking", "CUSTSN", Cb_custsn.IsChecked.ToString());
            }
            Cb_Rework.IsChecked = bool.Parse(ini.Read("PrePacking", "CBREWORK"));
            Cb_Fortmac.IsChecked = bool.Parse(ini.Read("PrePacking", "FORTMAC"));
            Cb_custsn.IsChecked = bool.Parse(ini.Read("PrePacking", "CUSTSN"));
            G_sBuildDate = System.DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            G_Time = System.DateTime.Now.ToString("yyyy/MM/dd");
            sPrgName = "PackingBOXID";
            this.Title = "SFIS : " + sPrgName + " Build Date : " + G_Time + " Version: " + ap_version;
            lbTitle.Content = M_sThisLineName + ":" + M_sThisGroup;
            Edt_linename.Text = M_sThisLineName;
            await Checkprivilege();
        }
        public void FormClose(object sender, EventArgs e)
        {
            string Path = System.AppDomain.CurrentDomain.BaseDirectory;
            try
            {
                labApp.Quit();
            }
            catch { }
            killprocess();
            try
            {
                string[] listlink = Directory.GetFiles(Path, "*.lab");
                foreach (string link in listlink)
                {
                    File.Delete(link);
                }

                labApp.Quit();
            }
            catch(Exception ex)
            { }
            finally
            {
                INIFile ini = new INIFile("C:\\PACKING\\Packing.ini");
                ini.Write("PrePacking", "CBREWORK", Cb_Rework.IsChecked.ToString());
                ini.Write("PrePacking", "FORTMAC", Cb_Fortmac.IsChecked.ToString());
                ini.Write("PrePacking", "CUSTSN", Cb_custsn.IsChecked.ToString());
            }
        }
        public void FormShow()
        {
            if (string.IsNullOrEmpty(M_sThisGroup))
            {
                FormStationSetup formStationSetup = new PACKINGBOXID_CFG.FormStationSetup(this, sfcClient);
                formStationSetup.ShowDialog();
            }
            Edt_linename.Text = M_sThisLineName;
            lbTitle.Content = Edt_linename.Text + ":" + M_sThisGroup;
        }
        #region zCom
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Z14.IsOpen)
            {
                try
                {
                    if (comEvent)
                    {
                        Inputdata.Text = ComData;
                        startInput();
                        comEvent = false;
                    }
                }
                catch { }
            }
        }
        void getComdata()
        {
            Z14.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
        }
        void serialPort_DataReceived(object s, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = Z14.ReadLine().Trim();
                if (!string.IsNullOrEmpty(data))
                {
                    ComData = data;
                    comEvent = true;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:Lỗi truyền dữ liệu từ cổng COM: " + exc.Message + "");
                return;
            }
        }
        private void Com1_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void useComPort_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!Inputdata.IsEnabled)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Nhập mã thẻ trước";
                    _sh.MessageEnglish = "Input employee ID first";
                    _sh.ShowDialog();
                    useComPort.IsChecked = false;
                    return;
                }
                Z14.Close();
                Z14.PortName = "COM1";
                Z14.BaudRate = Convert.ToInt32(9600);
                Z14.Handshake = System.IO.Ports.Handshake.None;
                Z14.Parity = Parity.None;
                Z14.DataBits = 8;
                Z14.StopBits = StopBits.One;
                Z14.ReadTimeout = 200;
                Z14.WriteTimeout = 50;
                Z14.Open();
            }
            catch (Exception exc)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = "Không thể mở kết nối cổng COM1. Kiểm tra kết nối cổng COM !";
                _sh.MessageEnglish = "Can't open scale's COM1 port \nERROR: " + exc.Message + "";
                _sh.ShowDialog();
                useComPort.IsChecked=false;
                return;
            }
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1 / 2);
            dispatcherTimer.Start();
            useComPort.IsChecked = true;
        }

        private void useComPort_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                Z14.Close();
                dispatcherTimer.Stop();
            }
            catch
            {
                //nothing
            }
        }
        public static void sendtoCom(string text)
        {
            if(Z14.IsOpen) Z14.WriteLine(text);
        }
        #endregion
    }
}
