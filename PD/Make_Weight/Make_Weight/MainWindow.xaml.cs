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
using Make_Weight.DataObject;
using System.IO.Ports;
using System.Windows.Threading;
using LabelManager2;
using System.IO;
using Make_Weight.Resource;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows;
using DateTime = System.DateTime;


namespace Make_Weight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow openWD { get; private set; }
        public static SfcHttpClient _sfcHttpClient;
        SfcHttpClient _sfcHttpClientAllparts;
        public DAL dal;
        string loginApiUri = "";
        string loginDB = "";
        public static string empNo = "";
        string empPass = "";
        string inputLogin = "";
        string checkSum;
        string checkSetup = "",upcValue = "";
        string strProcess = "",downloadfail = "";
        private string _res;
        dynamic I_stuff;
        string mac = "", ip = "", G_sLabl_Name = "";
        string empPW = "",moNumber = "";
        public static int check_role = 0;
        int txtMOPress = 0;
        string My_LabelFileName, LabelFileName_ftp, publicfilepath = "";
        string ap_version_server = "";
        public static string apname ="", localVer = "",apver = "";
        private SerialPort _serialPort = null;
        public static string isVisible = "N",isReprint = "N",reprint_checked = "N";
        string cartonNo = "", SourceString = "",sn = "";
        public static DataTable dtParams = new DataTable();
        public Boolean PrintOK = true, print;
        public static string lang = "EN";
        string isPrintLabel = "N";
        string prvW = "";
        bool _ChkMD5Flag = true,afterPrint = false;
        string sql = "";
        List<WeightTable> lst = new List<WeightTable>();
        private Ini ini;
        TextBox txtTemp = new TextBox();
        [DllImport("SfcScann.dll")]
        public static extern int SfcSubclass(IntPtr h);
        
        public MainWindow()
        {
            IntPtr handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            InitializeComponent();
            Loaded += MainWindow_Load;
            this.dtgRS.AutoGeneratingColumn += dtgRS_AutoGeneratingColumn;
            ini = new Ini(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) +"\\SFIS_AMS.INI");
            lLine.Text = ini.IniReadValue("MainSection", "LINE");
        }
        private void CHECKLINE_Click(object sender, RoutedEventArgs e)
        {
            SetupStation frmStation = new SetupStation();
            frmStation.ShowDialog();
            lLine.Text = ini.IniReadValue("MainSection", "LINE");
        }
        
        private void CTN_CHECK_Click(object sender, RoutedEventArgs e)
        {
            if (CTN_CHECK.IsChecked)
            {
                check_role = 0;
                isVisible = "N";
                isReprint = "N";
                clearAll();
                txtEmpNo.Clear();
                lblTitleFunc.Text = "CTN_WEIGHT";
                FQA_WEIGHT.IsChecked = false;
                lblSN.Content = "SN/SSN:";
                lblLimit.Visibility = Visibility.Hidden;
                txtLimit.Visibility = Visibility.Hidden;
                lblLineWeight.Visibility = Visibility.Hidden;
                txtLineWeight.Visibility = Visibility.Hidden;
            }
            
        }
            

        private delegate void UpdateUiTextDelegate(string text);
        void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           string recieved_data = _serialPort.ReadExisting();
            Dispatcher.Invoke(DispatcherPriority.Send,
            new UpdateUiTextDelegate(setWeight), recieved_data);
            System.Threading.Thread.Sleep(50);
        }

        private void dtgRS_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "ExtensionData")
            {
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }

        private void setWeight(string text)
        {
            try
            {
                string txt = "";
                //text = Regex.Replace(text, @"\s", "");
                text = text.Substring(0, text.Length - 2).Trim();
                    if (text.Length > 8)
                    {
                        if (!prvW.Equals(text.Substring(7, text.Length - 7)))
                        {
                            txt = text.Substring(7, text.Length - 7);
                            prvW = txt;
                            Double sbw = Double.Parse(txt);
                            txtCartonWeight.Text = sbw.ToString("F3") + "KG";
                        }                                       
                    }                
            }
            catch(Exception ex)
            {             
            }
        }

        private void FQA_WEIGHT_Click(object sender, RoutedEventArgs e)
        {
            if (FQA_WEIGHT.IsChecked == true)
            {
                check_role = 0;
                isVisible = "N";
                isReprint = "N";
                clearAll();
                txtEmpNo.Clear();
                lblTitleFunc.Text = "FQA_WEIGHT";
                CTN_CHECK.IsChecked = false;
                lblSN.Content = "CARTON_NO/SN/SSN:";
                lblLimit.Visibility = Visibility.Visible;
                txtLimit.Visibility = Visibility.Visible;
                lblLineWeight.Visibility = Visibility.Visible;
                txtLineWeight.Visibility = Visibility.Visible;
            }           
        }

        private void clearError()
        {
            txtError.Text = "";
        }
        private void clearAll()
        {
            txtMoNumber.Clear();
            txtModelName.Clear();
            txtUpWeight.Clear();
            txtDownWeight.Clear();
            txtStandardWeight.Clear();
            txtLimit.Clear();
            txtMCarton.Clear();
            txtCartonNo.Clear();
            txtSN.Clear();
            txtQty.Clear();
            txtPrintLabel.Clear();
            txtLineWeight.Clear();
            txtCOMP.Clear();
            txtCartonWeight.Clear();
            dtgRS.ItemsSource = null;
            prvW = "";
        }

       
        private void txtEmpNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            check_role = 0;
            isVisible = "N";
            isReprint = "N";
            clearAll();
        }

        public void setMessage(string msg)
        {
            txtError.Foreground = Brushes.Red;
            string Emsg = "", Vmsg = "";
            int aa = msg.IndexOf("|");
            if (msg.IndexOf("|") != -1)
            {
                Emsg= msg.Split('|')[0];
                Vmsg = msg.Split('|')[1];
            }
            if (EN.IsChecked)
            {
                txtError.Text = Emsg;
                lang = "EN";
                if (Emsg.ToUpper().StartsWith("OK")) txtError.Foreground = Brushes.Green;
            }
            else
            {
                txtError.Text = Vmsg;
                lang = "VI";
                if (Vmsg.ToUpper().StartsWith("OK")) txtError.Foreground = Brushes.Green;
            }
            
        }


       
         private async Task<string> txtMoNumber_Keypress(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Return)
            {
                float std = 0, vrName = 0 ; 
                clearError();
                if (check_role != 1)
                {
                    setMessage("Login first!|Nhập mã thẻ và mật khẩu trước!");
                    return "FAIL";
                }
                moNumber = txtMoNumber.Text.ToString().Trim();
                if ("".Equals(moNumber) || moNumber.Length == 0 )
                {
                    setMessage("MO cannot be blank|MO không được để trống");
                    return "FAIL"; ;
                }                
                var getMoData = new
                {
                    TYPE = "GET_MODATA",
                    MO = moNumber
                };

                _res = await dal.GetMOData(moNumber, "CHECK_MO",apver);            
                if (_res.Contains("OK"))
                {
                    /* string[] arrData = _message.Split('|');
                     txtModelName.Text = arrData[0].ToString().Trim();
                     if ("".Equals(arrData[1]) || "0".Equals(arrData[1]))
                     {
                         txtLimit.Text =arrData[4].ToString().Trim();
                     }
                     else
                     {
                         txtLimit.Text = arrData[1].ToString().Trim();
                     }
                     txtStandardWeight.Text = arrData[2].ToString().Trim();
                     txtModelName.IsEnabled = false;
                     std = float.Parse(arrData[2].ToString().Trim());
                     vrName = float.Parse(arrData[3].ToString().Trim());
                     txtUpWeight.Text = (std + (std * vrName)).ToString("#.####").Trim();
                     txtDownWeight.Text = (std - (std * vrName)).ToString("#.####").Trim(); */
                    //txtSN.IsEnabled = true;
                    //txtSN.Clear();
                    //txtSN.Focus();
                     return "OK";
                }
                else
                {
                    setMessage(_res.ToString());
                    //txtModelName.IsEnabled = true;
                    //txtModelName.Clear();
                    //txtLimit.Clear();
                    //txtStandardWeight.Clear();
                    //txtUpWeight.Clear();
                    //txtDownWeight.Clear();                 
                    //txtCartonNo.Clear();
                    //txtCartonNo.Focus();
                    return "FAIL";
                }
            }
            else
            {
                setMessage("NOT ENTER|Khong phai nhan enter ");
                return "NOTENTER";
            }
        }
        private async void txtCatxtCartonNo_Keypress(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                //DateTime now = DateTime.Now;
                //if (now.Subtract(_dt).Milliseconds > 50)
                //{
                //    (sender as TextBox).Text = "";
                //}
                //_dt = now;
            }
            //if (sSpecialScan == "FALSE")
            //{
            //    cforKeyDown = ToChar(e.Key);
            //    if (cforKeyDown != '\u0010')
            //        scannerText(e, txtCartonNo, cforKeyDown);
            //}
            string[] arrData;
            txtTemp.Text = "";
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
                    try
                      {
                        cartonNo = txtCartonNo.Text.ToString().Trim();
                        if ("".Equals(cartonNo) || cartonNo == "")
                        {
                            setMessage("Mcarton No cannot be blank|Mcarton No không được để trống");
                            return;
                        }
                        else
                        {
                            clearError();
                            cartonNo = cartonNo.ToUpper();
                            txtCartonNo.Text = cartonNo;                           
                            if (reprint_checked == "Y" || "Y".Equals(reprint_checked))
                            {
                                if ("Y".Equals(isReprint) || isReprint == "Y")
                                {
                                    await printLabel();
                                    if (PrintOK)
                                    {
                                        isReprint = "N";
                                        isVisible = "N";
                                        string actionDesc = "Reprint MCartonNo:" + cartonNo + ";" + ip + ";" + mac;
                                        string sqlSaveLog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time) values ('" + empNo + "','MAKE_WEIGHT','REPRINT','" + actionDesc + "',sysdate)";
                                        var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                                        { CommandText = sqlSaveLog, SfcCommandType = SfcCommandType.Text });
                                        reprint.IsChecked = false;
                                        clearAll();
                                        txtCartonNo.Text = cartonNo;
                                        txtCartonNo.SelectAll();
                                        txtCartonNo.Focus();
                                        setMessage("Reprint OK|Reprint OK");
                                    }
                                    else
                                    {
                                        clearAll();
                                        txtCartonNo.Text = cartonNo;
                                        txtCartonNo.SelectAll();
                                        txtCartonNo.Focus();
                                        return;
                                    }
                                }
                                else
                                {
                                    setMessage("No reprint privilege,Login again!|Không có quyền in bù,Đăng nhập lại!");
                                    return;
                                }
                                
                            }
                            else
                            {
                                upcValue = "";
                                string mygroup = lblTitleFunc.Text.ToString().Trim().ToUpper();
                                if ("CTN_WEIGHT".Equals(mygroup) || mygroup == "CTN_WEIGHT")
                                {
                                    //upcValue = await dal.IsSupercap(cartonNo);
                                    upcValue = "";
                                }

                                if (!"".Equals(upcValue) || upcValue.Length > 0)
                                {
                                    lblSN.Visibility = Visibility.Visible;
                                    txtSN.Visibility = Visibility.Visible;
                                    txtSN.IsEnabled = true;
                                    txtSN.Focus();
                                }
                                else
                                {
                                    //lblSN.Visibility = Visibility.Hidden;
                                    //txtSN.Visibility = Visibility.Hidden;
                                    //txtSN.IsEnabled = false;
                                    if (txtCartonWeight.Text.ToString().Trim() == "") txtCartonWeight.Text = "0.00KG";
                                    string strW = txtCartonWeight.Text.ToString().Trim();
                                    string weight = strW.Substring(0, strW.Length - 2);
                                    Double dbweight = Double.Parse(weight);
                                    string actual_weight = dbweight.ToString("F3");
                                    string res = await dal.CheckCartonNo(empNo, cartonNo, mygroup, apver, lLine.Text+"|"+actual_weight);
                                    WeightTable tbl = new WeightTable();
                                    tbl.CARTON_NO = cartonNo;
                                    tbl.CARTON_WEIGHT = strW;
                                    if (EN.IsChecked) tbl.NOTE = res.Split('|')[0];
                                    else tbl.NOTE = res.Split('|')[1];
                                    if(res.ToUpper().StartsWith("NEEDSN"))
                                    {
                                        txtCOMP.Text = "";
                                        arrData = res.Split('|');
                                        lblSN.Content = "SN : " + arrData[8].ToString().Trim();
                                        txtSN.SelectAll();
                                        txtSN.Focus();
                                        setMessage("OK carton,Scan SN/SSN to confirm|OK carton,Sảo mã SN hoặc SSN để xác nhận ");
                                        txtMoNumber.Clear();
                                        txtStandardWeight.Text = arrData[4].ToString().Trim();
                                        txtModelName.Text = arrData[1].Trim();
                                        txtModelName.IsEnabled = false;
                                        txtMCarton.Text = cartonNo;
                                        txtUpWeight.Text = arrData[5].ToString().Trim();
                                        txtDownWeight.Text = arrData[3].ToString().Trim();
                                        txtPrintLabel.Text = arrData[6].ToString().Trim();
                                        txtQty.Text = arrData[7].ToString().Trim();
                                        return;
                                    }
                                    if (res.StartsWith("OK") || res.Contains("OK"))
                                    {
                                        arrData = res.Split('|');
                                        txtMoNumber.Text = arrData[2].ToString().Trim();
                                        string checkMO = await txtMoNumber_Keypress(sender, e);
                                        if (!"OK".Equals(checkMO) || checkMO != "OK")
                                        {
                                            txtPrintLabel.Clear();
                                            txtMCarton.Clear();
                                            txtQty.Clear();
                                            txtCartonNo.SelectAll();
                                            txtCartonNo.Focus();
                                            return;
                                        }
                                        txtMoNumber.Clear();
                                        txtStandardWeight.Text = arrData[4].ToString().Trim();
                                        txtModelName.Text = arrData[1].Trim();
                                        txtModelName.IsEnabled = false;
                                        txtMCarton.Text = cartonNo;
                                        txtUpWeight.Text = arrData[5].ToString().Trim();
                                        txtDownWeight.Text = arrData[3].ToString().Trim();
                                        txtPrintLabel.Text = arrData[6].ToString().Trim();
                                        txtQty.Text = arrData[7].ToString().Trim();
                                        if ("FQA_WEIGHT".Equals(mygroup) || mygroup == "FQA_WEIGHT")
                                        {
                                            txtLimit.Text = arrData[8].ToString().Trim();
                                            txtLineWeight.Text = arrData[9].ToString().Trim();
                                        }
                                        else
                                        {
                                            //res = await dal.CheckSN(empNo, cartonNo, mygroup, sn+"|"+actual_weight, apver);                               
                                            if ("Y".Equals(isPrintLabel) || isPrintLabel == "Y")
                                            {
                                                await printLabel();
                                                if (PrintOK)
                                                {
                                                    isVisible = "N";
                                                    isReprint = "N";
                                                    reprint.IsChecked = false;
                                                }
                                                else
                                                {
                                                    txtCOMP.Text = "";
                                                    txtCartonNo.SelectAll();
                                                    txtCartonNo.Focus();
                                                    tbl.RESULT = "FAIL";
                                                    txtCOMP.Text = "FAIL";
                                                    txtCOMP.Foreground = Brushes.Red;
                                                    lst.Add(tbl);
                                                    dtgRS.ItemsSource = null;
                                                    dtgRS.ItemsSource = lst;
                                                    txtPrintLabel.Clear();
                                                    txtMCarton.Clear();
                                                    txtQty.Clear();
                                                    return;
                                                }

                                            }
                                        }
                                        txtSN.Text="";
                                        lblSN.Content = "SN : 0/0 ";
                                        txtCOMP.Text = "PASS";
                                        tbl.RESULT = "PASS";
                                        txtCOMP.Foreground = Brushes.Green;
                                        lst.Add(tbl);
                                        dtgRS.ItemsSource = null;
                                        dtgRS.ItemsSource = lst;
                                        setMessage("OK|OK");
                                        txtCartonNo.SelectAll();
                                        txtCartonNo.Focus();

                                    }
                                    else
                                    {
                                        txtCOMP.Text = "";
                                        setMessage(res);
                                        txtCartonNo.SelectAll();
                                        txtCartonNo.Focus();
                                        tbl.RESULT = "FAIL";
                                        txtCOMP.Text = "FAIL";
                                        txtCOMP.Foreground = Brushes.Red;
                                        lst.Add(tbl);
                                        dtgRS.ItemsSource = null;
                                        dtgRS.ItemsSource = lst;
                                        txtPrintLabel.Clear();
                                        txtMCarton.Clear();
                                        txtQty.Clear();
                                        return;
                                    }
                                }
                                
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        setMessage(ex.Message.ToString() + "|" + ex.Message.ToString());
                        return;
                    }                   

                }
                }
                   
        }

        public void AddParams(string str)
        {
            string[] params1;
            List<string> lstcheckDup = new List<string>();
            try
            {
                dtParams.Clear();

                if (dtParams.Columns.Count == 0)
                {
                    dtParams.Columns.Add("Name");
                    dtParams.Columns.Add("Value");
                }
                params1 = str.Split('|');
                for (int i = 0; i < params1.Length; i ++)
                {
                    string name = params1.ElementAt(i).Split(':')[0].ToString().Trim();
                    string value = params1.ElementAt(i).Split(':')[1].ToString().Trim();                   
                    if (!lstcheckDup.Contains(name))
                    {
                        lstcheckDup.Add(name);
                        dtParams.Rows.Add(new object[] { name,value });
                        if ("ModelName".Equals(name) || name == "ModelName")
                        {
                            G_sLabl_Name = value + "_LSSC.LAB";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                setMessage(ex.Message.ToString() + "|"+ ex.Message.ToString());
                return;
            }
        }

        private async Task printLabel()
        {
            PrintOK = false;
            try
            {
                DataTable dt = new DataTable();             
                string input = "LABELTYPE:LSSC|MCARTON_NO:" + cartonNo;
                List<string> lstRes = await dal.checkPrint(input);
                string strPrm = lstRes.ElementAt(1).Trim();
                if (lstRes.ElementAt(0).Trim().StartsWith("OK"))
                {
                    AddParams(strPrm);
                    await P_PrintToCodeSoft(G_sLabl_Name);
                }
                else
                {
                    setMessage(lstRes.ElementAt(0).Trim());
                    return;
                }
                
            }
            catch (Exception ex)
            {
                setMessage("printLabel function exception:" + ex.Message + "|" + "printLabel function exception:" + ex.Message);
                return;
            }
            
        }

        private async Task<bool> killprocess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
            {
                process.Kill();
            }
            return true;
        }

        public async Task P_PrintToCodeSoft(string paramLabelFile)
        {
            try
            {
                await killprocess();               
                //Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                LabelFileName_ftp = paramLabelFile;
                My_LabelFileName = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + @"\" + paramLabelFile;
                if (publicfilepath != My_LabelFileName || File.Exists(My_LabelFileName))
                {
                    //Remove label file old
                    if (File.Exists(My_LabelFileName))
                    {
                        try
                        {
                            File.Delete(My_LabelFileName);
                        }
                        catch (Exception ex)
                        {
                            List<Process> lstProcs = new List<Process>();
                            lstProcs = FileUtil.WhoIsLocking(My_LabelFileName);

                            foreach (Process p in lstProcs)
                            {
                                try
                                {
                                    ProcessHandler.localProcessKill(p.ProcessName);
                                }
                                catch (Exception exc)
                                {
                                    setMessage("Cannot kill process " + p.ProcessName + " Exception: " + exc.Message+ "|Không thể đóng process " + p.ProcessName);
                                    return;
                                }
                            }
                            try
                            {
                                File.Delete(My_LabelFileName);
                            }
                            catch (Exception ex_e)
                            {
                                setMessage("Cannot delete file " + My_LabelFileName + ". Exception: " + ex_e.Message+ "|Không thể xóa file " + My_LabelFileName + ". Ngoại lệ: " + ex_e.Message);
                                return;
                            }
                        }
                    }
                    if (!await downloadlabel(paramLabelFile))
                    {
                        setMessage("Error during Download: " + downloadfail + "|Lỗi khi tải xuống: " + downloadfail);
                        return;
                    }
                }
                
                publicfilepath = My_LabelFileName;

                if (!File.Exists(My_LabelFileName))
                {
                    setMessage("The label file could not find: " + My_LabelFileName+ "|Không tìm thấy file label: " + My_LabelFileName);
                    return;
                }

                if (string.IsNullOrEmpty(G_sLabl_Name))
                {
                    G_sLabl_Name = LabelFileName_ftp;
                    if (G_sLabl_Name == "")
                    {
                        setMessage("Not Find Label Name Or Station Name|Không tìm thấy tên label hoặc tên trạm");
                        return;
                    }
                }
                    ApplicationClass LabApp = new ApplicationClass();
                    LabApp.Documents.Open(My_LabelFileName, false);
                    Document doc = LabApp.ActiveDocument;
                    bool _chkprint = await CallCodesoftToPrint(doc, LabelFileName_ftp, dtParams);
                if (_chkprint)
                {
                    PrintOK = true;
                }
            }
            catch (Exception ex)
            {
                setMessage(ex.Message.ToString() + "|" + ex.Message.ToString());
                return;
            }
            #region delete label file
            //finally
            //{
            //    //Remove label file old
            //    await killprocess();
            //    if (File.Exists(My_LabelFileName))
            //    {
            //        try
            //        {
            //            File.Delete(My_LabelFileName);
            //        }
            //        catch (Exception ex)
            //        {
            //            List<Process> lstProcs = new List<Process>();
            //            lstProcs = FileUtil.WhoIsLocking(My_LabelFileName);

            //            foreach (Process p in lstProcs)
            //            {
            //                try
            //                {
            //                    ProcessHandler.localProcessKill(p.ProcessName);
            //                }
            //                catch (Exception exc)
            //                {
            //                }
            //            }
            //            try
            //            {
            //                File.Delete(My_LabelFileName);
            //            }
            //            catch (Exception ex_e)
            //            {
            //            }
            //        }
            //    }
            //   

            //}
            #endregion
        }


        public async Task<bool> CallCodesoftToPrint(Document doctemp, string LabelName, DataTable dtParams)
        {
            string _param_Name = string.Empty, _labName;
            string cTEST = string.Empty;
            try
            {
                LabelTracking _lbt = new LabelTracking(openWD);
                //_lbt.sfcHttpClient = SfcHttpClient;
                _lbt.sfcHttpClient = _sfcHttpClient;

                //Check MD5 of label file
                if (_ChkMD5Flag)
                {
                    if (!await _lbt.doMD5Label(LabelName, 1, true))
                    {
                        return false;
                    }
                    _ChkMD5Flag = false;
                }
                for (int i = 1; i < doctemp.Variables.FormVariables.Count + 1; i++)
                {
                    _labName = doctemp.Variables.FormVariables.Item(i).Name;
                    //doctemp.Variables.FormVariables.Item(_labName).CounterUse = 0;
                    doctemp.Variables.FormVariables.Item(_labName).Value = "";
                }
                // Set value into label file
                foreach (DataRow param in dtParams.Rows)
                {
                    _param_Name = param["Name"].ToString().ToUpper();
                    string vl = param["Value"].ToString();
                    try
                    {
                        for (int i = 1; i < doctemp.Variables.FormVariables.Count + 1; i++)
                        {
                            if (doctemp.Variables.FormVariables.Item(i).Name.ToUpper() == _param_Name)
                            {
                                doctemp.Variables.FormVariables.Item(_param_Name).CounterUse = 0;
                                doctemp.Variables.FormVariables.Item(_param_Name).Value = param["Value"]?.ToString() ?? "";                                
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        setMessage(ex.Message.ToString() + "|" + ex.Message.ToString());
                        return false;
                    }
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
                    doctemp.Save();
                    //Call label tracking
                    try
                    {
                        if (!await _lbt.doLabelTracking_AddParamValue(LabelName, 1, dtParams, doctemp, dtbVarName, true))
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        setMessage("Check label tracking have exception: " + ex.Message + "|Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message);
                        return false;
                    }
                    int printQTY = 1;
                    doctemp.PrintDocument(printQTY);
                
            }
            catch (Exception ex)
            {
                setMessage("Have exceptions when pint document: " + ex.Message+"|Phát sinh lỗi khi in label: " + ex.Message);
                return false;
            }
            return true;
        }


        public async Task<bool> downloadlabel(string labelname)
        {
            string ftpaddress, TMPDIR, ftpuser, ftppassword, LocalDir;
            string DestinationString, sModel_name = "Z_CARTON_LH";
            LocalDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\" + labelname;
            string ftplabelpath;

            try
            {
                if (_sfcHttpClient == null)
                {
                    MessageBox.Show("Can't connect to DB ");
                    return false;
                }
                string sql = string.Format("SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME ='{0}'", sModel_name);
                var _result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });

                if (_result.Data != null)
                {
                    ftpaddress = _result.Data["model_serial"].ToString();
                    ftplabelpath = _result.Data["customer"].ToString();
                    SourceString = "http://" + ftpaddress + ftplabelpath + "//" + labelname;
                    if (!await DownLoadInternetFile(SourceString, LocalDir))
                    {
                        downloadfail = SourceString;
                        setMessage("Error during Download " + SourceString+ "|Lỗi khi tải xuống " + SourceString);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
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
                    downloadfail = Source;
                    setMessage(ex.Message+"|"+ ex.Message);
                    return false;
                }
            }
            return false;
        }
        string sSpecialScan = "FALSE";
        char cforKeyDown = '\0';
        DateTime _dt = DateTime.Now;
        private async void txtSN_Keypress(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
            {
                //DateTime now = DateTime.Now;
                //if (now.Subtract(_dt).Milliseconds > 50)
                //{
                //    (sender as TextBox).Text = "";
                //}
                //_dt = now;
            }
                //if (sSpecialScan == "FALSE")
                //{
                //    cforKeyDown = ToChar(e.Key); 
                //    if (cforKeyDown != '\u0010')
                //        scannerText(e, txtSN, cforKeyDown);
                //}
                string[] arrData;
            if (e.Key == Key.Return)
            {
                try
                {
                    clearError();
                    if (check_role == 0)
                    {
                        setMessage("Login first!|Nhập mã thẻ và mật khẩu trước!");
                        return;
                    }
                    if (txtCartonWeight.Text == null || "".Equals(txtCartonWeight.Text))
                    {
                        setMessage("No weight data!|Không có dữ liệu cân!");
                        return;
                    }
                    sn = txtSN.Text.ToString().Trim().ToUpper();
                    if ("".Equals(sn) || sn == "")
                    {
                        setMessage("SN cannot be blank|SN không được để trống");
                        txtSN.Clear();
                        txtSN.Focus();
                        return;
                    }
                    else
                    {
                            if (txtCartonWeight.Text.ToString().Trim() == "") txtCartonWeight.Text = "0.00KG";
                            string mygroup = lblTitleFunc.Text.ToString().Trim().ToUpper();
                            string strW = txtCartonWeight.Text.ToString().Trim();
                            string weight = strW.Substring(0, strW.Length - 2);
                            Double dbweight = Double.Parse(weight);
                            string actual_weight = dbweight.ToString("F3");
                            string LISTSN = txtSN.Text.Trim();
                        int a = txtTemp.Text.Trim().IndexOf(txtSN.Text.Trim());
                            if (txtTemp.Text.Trim().IndexOf(txtSN.Text.Trim()) > -1)
                        {
                            setMessage("SN had scan|SN đã sảo");
                            txtSN.Clear();
                            txtSN.Focus();
                            return;
                        }
                            if(txtTemp.Text.Trim()!="") LISTSN = txtSN.Text.Trim() + "," + txtTemp.Text.Trim();
                            string res = await dal.CheckCartonNo(empNo, cartonNo, mygroup, apver, lLine.Text+ "|"+actual_weight+"|"+ LISTSN);
                            WeightTable tbl = new WeightTable();
                            tbl.CARTON_NO = cartonNo;
                            tbl.CARTON_WEIGHT = strW;
                            if (EN.IsChecked) tbl.NOTE = res.Split('|')[0];
                            else tbl.NOTE = res.Split('|')[1];
                            if (res.ToUpper().StartsWith("NEEDSN"))
                            {
                                arrData = res.Split('|');
                                lblSN.Content = "SN : " + arrData[8].ToString().Trim();
                                txtTemp.Text = arrData[9].ToString().Trim();
                                txtSN.SelectAll();
                                txtSN.Focus();
                                setMessage("OK,Scan next SN/SSN to confirm|OK,Sảo mã SN hoặc SSN tiếp theo");
                                txtMoNumber.Clear();
                                txtStandardWeight.Text = arrData[4].ToString().Trim();
                                txtModelName.Text = arrData[1].Trim();
                                txtModelName.IsEnabled = false;
                                txtMCarton.Text = cartonNo;
                                txtUpWeight.Text = arrData[5].ToString().Trim();
                                txtDownWeight.Text = arrData[3].ToString().Trim();
                                txtPrintLabel.Text = arrData[6].ToString().Trim();
                                txtQty.Text = arrData[7].ToString().Trim();
                                return;
                            }
                            if (res.StartsWith("OK") || res.Contains("OK"))
                            {
                                arrData = res.Split('|');
                                txtMoNumber.Text = arrData[2].ToString().Trim();
                                string checkMO = await txtMoNumber_Keypress(sender, e);
                                if (!"OK".Equals(checkMO) || checkMO != "OK")
                                {
                                    txtPrintLabel.Clear();
                                    txtMCarton.Clear();
                                    txtQty.Clear();
                                    txtCartonNo.SelectAll();
                                    txtCartonNo.Focus();
                                    return;
                                }
                                txtMoNumber.Clear();
                                txtStandardWeight.Text = arrData[4].ToString().Trim();
                                txtModelName.Text = arrData[1].Trim();
                                txtModelName.IsEnabled = false;
                                txtMCarton.Text = cartonNo;
                                txtUpWeight.Text = arrData[5].ToString().Trim();
                                txtDownWeight.Text = arrData[3].ToString().Trim();
                                txtPrintLabel.Text = arrData[6].ToString().Trim();
                                txtQty.Text = arrData[7].ToString().Trim();
                                if ("FQA_WEIGHT".Equals(mygroup) || mygroup == "FQA_WEIGHT")
                                {
                                    txtLimit.Text = arrData[8].ToString().Trim();
                                    txtLineWeight.Text = arrData[9].ToString().Trim();
                                }
                                else
                                {
                                    //res = await dal.CheckSN(empNo, cartonNo, mygroup, sn+"|"+actual_weight, apver);                               
                                    if ("Y".Equals(isPrintLabel) || isPrintLabel == "Y")
                                    {
                                        await printLabel();
                                        if (PrintOK)
                                        {
                                            isVisible = "N";
                                            isReprint = "N";
                                            reprint.IsChecked = false;
                                        }
                                        else
                                        {
                                            txtCOMP.Text = "";
                                            txtCartonNo.SelectAll();
                                            txtCartonNo.Focus();
                                            tbl.RESULT = "FAIL";
                                            txtCOMP.Text = "FAIL";
                                            txtCOMP.Foreground = Brushes.Red;
                                            lst.Add(tbl);
                                            dtgRS.ItemsSource = null;
                                            dtgRS.ItemsSource = lst;
                                            txtPrintLabel.Clear();
                                            txtMCarton.Clear();
                                            txtQty.Clear();
                                            return;
                                        }

                                    }
                                }
                            txtSN.Text = "";
                            lblSN.Content = "SN : 0/0 ";
                            txtCOMP.Text = "PASS";
                            tbl.RESULT = "PASS";
                            txtCOMP.Foreground = Brushes.Green;
                            lst.Add(tbl);
                            dtgRS.ItemsSource = null;
                                dtgRS.ItemsSource = lst;
                                setMessage("OK|OK");
                                txtCartonNo.SelectAll();
                                txtCartonNo.Focus();

                            }
                            else
                            {
                                txtCOMP.Text = "";
                                setMessage(res);
                                txtSN.SelectAll();
                                txtSN.Focus();
                                tbl.RESULT = "FAIL";
                                txtCOMP.Text = "FAIL";
                                txtCOMP.Foreground = Brushes.Red;
                                lst.Add(tbl);
                                dtgRS.ItemsSource = null;
                                dtgRS.ItemsSource = lst;
                                txtPrintLabel.Clear();
                                txtMCarton.Clear();
                                txtQty.Clear();
                                return;
                            }
                    }
                }
                catch(Exception ex)
                {
                    setMessage(ex.ToString() + "|"+ex.ToString());
                    return;
                }
                
            }
        }
        public enum MapType :uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }
        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out,MarshalAs(UnmanagedType.LPWStr,SizeParamIndex =4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);
        public static char ToChar(Key key)
        {
            char ch = ' ';
            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            var stringBuilder = new StringBuilder(2);
            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch(result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }

        private void txtEmpNO_Keypress(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.Return)
            {
                empNo = txtEmpNo.Text.ToString().Trim();
                if ("".Equals(empNo) || empNo.Length == 0)
                {
                    setMessage("Emp No cannot be blank|Mã thẻ không được để trống");
                    return;
                }

                    txtEmpPwd.SelectAll();
                    txtEmpPwd.Focus();
            }
        }

        private async void txtEmpPwd_Keypress(object sender, KeyEventArgs e)
        {
            empPW = txtEmpPwd.Password.ToString().Trim();
            empNo = txtEmpNo.Text.Trim();
            if (e.Key == Key.Return)
            {
                clearError();
                if ("".Equals(empPW) || empPW.Length == 0 || empNo.Length == 0)
                {
                    setMessage("EmpNo and Emp pass cannot be blank|Mã thẻ và mật khẩu không được để trống");
                    return;
                }
                empNo = txtEmpNo.Text.Trim();

                _res = await dal.CheckLogin(empNo, empPW, lblTitleFunc.Text.ToString(), "CHECK_EMP", apver);
                if (_res.ToString().Contains("OK"))
                {
                    Status.Text = "Employee: " + empNo + "       |       " + "IP: " + ip + "       |       " + "MAC: " + mac;
                    check_role = 1;
                    setMessage(_res.ToString());
                    isVisible = _res.Split('|')[1];
                    isReprint = _res.Split('|')[2];
                    txtCartonNo.IsEnabled = true;
                    txtCartonNo.Focus();
                }
                else
                {
                    setMessage(_res.ToString());
                    txtEmpPwd.SelectAll();
                    txtEmpPwd.Focus();
                    txtCartonNo.IsEnabled = false;
                    check_role = 0;
                    isVisible = "N";
                    isReprint = "N";
                    return;
                }                   
            }                     
        }


        private void reprint_Click(object sender, RoutedEventArgs e)
        {
            if (reprint.IsChecked == true)
            {
                if ("N".Equals(isReprint) || isReprint == "N" || check_role == 0)
                {
                    setMessage("No reprint privilege,Login again!|Không có quyền in bù,Đăng nhập lại!");
                    reprint.IsChecked = false;
                    return;
                }
                reprint_checked = "Y";
                lblTitleFunc.Text = "REPRINT";
            }
        }

        private void txtCartonNo_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Cancel paste from mouse/keyboard
            if (txtEmpNo.Text != "V1019802")
            {
                if (e.Command == ApplicationCommands.Paste) e.Handled = true;
            }
        }

        private void lblSN_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            //Cancel paste from mouse/keyboard
            if (e.Command == ApplicationCommands.Paste) e.Handled = true;
        }

        private void reprint_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CTN_CHECK.IsChecked)
            {
                check_role = 0;
                isVisible = "N";
                isReprint = "N";
                reprint_checked = "N";
                clearAll();
                txtEmpNo.Clear();
                lblTitleFunc.Text = "CTN_WEIGHT";
                FQA_WEIGHT.IsChecked = false;
                lblSN.Content = "SN/SSN:";
                lblLimit.Visibility = Visibility.Hidden;
                txtLimit.Visibility = Visibility.Hidden;
                lblLineWeight.Visibility = Visibility.Hidden;
                txtLineWeight.Visibility = Visibility.Hidden;
            }
            if (FQA_WEIGHT.IsChecked)
            {
                check_role = 0;
                isVisible = "N";
                isReprint = "N";
                reprint_checked = "N";
                clearAll();
                txtEmpNo.Clear();
                lblTitleFunc.Text = "FQA_WEIGHT";
                CTN_CHECK.IsChecked = false;
                lblSN.Content = "CARTON_NO/SN/SSN:";
                lblLimit.Visibility = Visibility.Visible;
                txtLimit.Visibility = Visibility.Visible;
                lblLineWeight.Visibility = Visibility.Visible;
                txtLineWeight.Visibility = Visibility.Visible;
            }
        }

        private void txtPrintLabel_TextChanged(object sender, TextChangedEventArgs e)
        {
            isPrintLabel = txtPrintLabel.Text;
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void visible_Click(object sender, RoutedEventArgs e)
        {
            if ("N".Equals(isVisible) || isVisible == "N" || check_role == 0)
            {
                setMessage("No visible privilege,Login again!|Không có quyền visible,Đăng nhập lại!");
                return;
            }
            if (File.Exists(My_LabelFileName))
            {
                ApplicationClass labApp = new ApplicationClass();
                try
                {
                    string actionDesc = "Visible MCartonNo:" + cartonNo + ";" + ip + ";" + mac;
                    string sqlSaveLog = "insert into SFISM4.R_SYSTEM_PRGLOG_T(emp_no,prg_name,action_type,action_desc,time) values ('" + empNo + "','MAKE_WEIGHT','VISIBLE','" + actionDesc + "',sysdate)";
                    var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    { CommandText = sqlSaveLog, SfcCommandType = SfcCommandType.Text });
                    //---------------
                    empNo = ""; txtEmpNo.Text = "";
                    labApp.Documents.Open(My_LabelFileName, false);
                    Document doc = labApp.ActiveDocument;
                    doc.Application.Visible = true;
                    isVisible = "N";
                }
                catch (Exception ex)
                {
                    setMessage(ex.Message + "|" + ex.Message);
                    return;
                }
            }
            else
            {
                setMessage("Can not find Label file!|Không tìm thấy tệp label!");
                return;
            }

        }

        private void Showparams_Click(object sender, RoutedEventArgs e)
        {
            if ("N".Equals(isVisible) || isVisible == "N" || check_role == 0)
            {
                setMessage("No privilege show param,Login again!|Không có quyền show param,Đăng nhập lại!");
                return;
            }
            empNo = ""; txtEmpNo.Text = "";
            Show_Params frm = new Show_Params(dtParams, openWD);
            frm.Show();
        }

        private void EN_Click(object sender, RoutedEventArgs e)
        {
            if (EN.IsChecked == true)
            {
                VI.IsChecked = false;
            }
        }

        private void VI_Click(object sender, RoutedEventArgs e)
        {
            if (VI.IsChecked == true)
            {
                EN.IsChecked = false;
            }
        }


        private void txtMoNumber_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        private void closePort()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                }
            }
            
        }

        private async void MainWindow_Closed(object sender, EventArgs e)
        {
            string dir = "C:/NIC/Make_Weight";
            string[] listlink = Directory.GetFiles(dir, "*.lab");
            foreach (string link in listlink)
            {
                File.Delete(link);
            }

            closePort();
            await killprocess();
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
                    closePort();
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
                loginDB = argsInfor[2].ToString();
                empNo = argsInfor[3].ToString();
                empPass = argsInfor[4].ToString();
                //lblSN.Visibility = Visibility.Hidden;
                //txtSN.Visibility = Visibility.Hidden;
                string fun = lblTitleFunc.Text.Trim();
                _sfcHttpClient = new SfcHttpClient(loginApiUri, loginDB, "helloApp", "123456");
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
                dal = new DAL(_sfcHttpClient);

                apname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                _res = await dal.GetVersionMatch(apname);
                if (!_res.Contains("Program not exist"))
                {
                    ap_version_server = _res;
                }
                else
                {
                    MessageBox.Show(_res, "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                    closePort();
                    Environment.Exit(0);
                }
                string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                var ver_local = new Version(ver);
                var ver_server = new Version(ap_version_server);
                var result = ver_local.CompareTo(ver_server);
                if (result < 0)
                {
                    MessageBox.Show("Application is running have new version on AMS-SERVER is: " + ap_version_server + " ,Please use SFC_AMS program download new version!", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    closePort();
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
                    CTN_CHECK.IsChecked = true;
                    openWD.Title = apname + " (Version: " + ver_local + " )";
                    lblTitle.Text = apname + " (Version: " + ver_local + " )";
                     Status.Text = "Employee: " + empNo + "       |       " + "IP: " + ip + "       |       " + "MAC: "+mac;
                    _serialPort = new SerialPort("COM1", 9600, Parity.None, 8);
                    _serialPort.Handshake = System.IO.Ports.Handshake.None;
                    _serialPort.StopBits = StopBits.One;                   
                    _serialPort.Open();
                    _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
                    lblLimit.Visibility = Visibility.Hidden;
                    txtLimit.Visibility = Visibility.Hidden;
                    lblLineWeight.Visibility = Visibility.Hidden;
                    txtLineWeight.Visibility = Visibility.Hidden;
                    visible.IsCheckable = false;
                    Showparams.IsCheckable = false;
                    openWD.Show();
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                closePort();
                Environment.Exit(0);
            }

        }

        static int _lastKeystroke = DateTime.Now.Millisecond;
        static List<char> _barcode = new List<char>(1);
        static bool UseKeyboard = false;

        public static void scannerText(KeyEventArgs e, TextBox txtText, char cforKeyDown)
        {
            
            if (cforKeyDown != ToChar(e.Key) || cforKeyDown == '\0')
            {
                cforKeyDown = '\0';
                _barcode.Clear();
                txtText.Text = "";
                return;
            }
            var temp = (char)KeyInterop.VirtualKeyFromKey(e.Key);
            // getting the time difference between 2 keys
            int elapsed = (DateTime.Now.Millisecond - _lastKeystroke);

            /*
             * Barcode scanner usually takes less than 17 milliseconds to read, increase this if neccessary of your barcode scanner is slower
             * also assuming human can not type faster than 17 milliseconds
             */
            if (elapsed > 100)
            {
                txtText.Text = "";
                _barcode.Clear();
            }


            // Do not push in array if Enter/Return is pressed, since it is not any Character that need to be read
            if (ToChar(e.Key) != (char)Key.Return)
            {
                _barcode.Add(ToChar(e.Key));
            }

            // Barcode scanner hits Enter/Return after reading barcode
            if (ToChar(e.Key) == (char)Key.Return && _barcode.Count > 0)
            {
                txtText.Text = new String(_barcode.ToArray());

                _barcode.Clear();
            }

            // update the last key press strock time
            _lastKeystroke = DateTime.Now.Millisecond;
        }
    }
}
