using System;
using System.Collections.Generic;
using System.Linq;
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
using Sfc.Library.HttpClient;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Sfc.Core.Parameters;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using CodeSoft_9Codes.Resources;
using System.Net;
using System.Windows.Threading;

namespace CodeSoft_9Codes.View
{
    /// <summary>
    /// Interaction logic for CodeSoft_NIC.xaml
    /// </summary>
    /// 
    //internal sealed class p1
    //{
    //    string product { get; set; }
    //    string kp { get; set; }
    //}
    public partial class CodeSoft_NIC : UserControl
    {
        public DataTable dt, dtParams, dtPqe;
        DAL fDal;
        string sql = "",strMO="", options="",thisKP,thisProgram;
        string res, output;
        public bool releaseAvaiable = false,PrintAvailable=false;

        public LabelManager2.Application labApp = null;
        public LabelManager2.Document doc = null;
        string My_LabelFileName, LabelFileName_ftp, pubftppath, G_sLabl_Name, publicfilepath = "", SourceString="";
        public Boolean  printOK = false,ShowCheck=false;



        public static bool isPrinting;
        public static string FilePath = "";
        private static string UrlLabelFile;
        LabelManager2.Application LabApp;
        LabelTracking _lbt = new LabelTracking();
        Document doctemp;
        private bool _ChkMD5Flag = true, sflag=false;
        public int sQty = 0;
        public string LabelName = "Default.Lab";
        public string thisSN = "", sPercent, sStatus;
        public CodeSoft_NIC()
        {
            InitializeComponent();
            fDal = new DAL();
            dt = new DataTable();
            txtLabelMode.Text = _numValue.ToString();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private async void txtMo_KeyUp(object sender, KeyEventArgs e)
        {
            await clear();
            if(e.Key==Key.Enter)
            {
                await call_SP("", "CheckEMP");
                if (res.StartsWith("OK"))
                {
                    if (getVal(output, "Visible") == "Y") btnVisible.IsEnabled = true;
                    if (getVal(output, "ShowParam") == "Y") btnShowParam.IsEnabled = true;
                    if (getVal(output, "PrintOver") == "Y") chkPrintOver.IsEnabled = true;
                    if (getVal(output, "Reprint") == "Y") chkReprint.IsEnabled = true;
                }
                strMO = txtMo.Text.Trim();
                if (string.IsNullOrEmpty(strMO)) return;
                sql = string.Format("select model_name,target_qty from SFISM4.R_BPCS_MOPLAN_T where mo_number ='{0}'", strMO);
                dt = await fDal.ExcuteSelectSQL(sql,MainWindow.sfcClient);
                if (dt.Rows.Count > 0)
                {
                    txtModelName.Text = dt.Rows[0]["model_name"].ToString();
                    txtMoQty.Text = dt.Rows[0]["target_qty"].ToString();
                    sql = string.Format("select distinct rownum as ID,product_name,cust_kp from SFIS1.C_MODEL_DESC_T2 where model_name ='{0}' and trim(upper(PRODUCT_NAME)) not in('NIC','HPE','LICEN','LICENSE')", txtModelName.Text);
                    dt = await fDal.ExcuteSelectSQL(sql, MainWindow.sfcClient);
                    if (dt.Rows.Count == 0)
                    {
                        showMessage("Not setup in ModelDesc2|Chưa thiết lập trong ModelDesc2");
                        return;
                    }
                    RadioButtonList_PN.DisplayMemberPath = dt.Columns[1].ColumnName;
                    RadioButtonList_PN.ItemsSource = dt.DefaultView;

                    RadioButtonList_Program.DisplayMemberPath = dt.Columns[0].ColumnName;
                    RadioButtonList_Program.ItemsSource = dt.DefaultView;
                }
                else
                {
                    showMessage("MO not found,Download MO first|Không tìm thấy công lệnh,Download công lệnh trước");
                    return;
                }
                //txtMo.IsEnabled = false;
            }
        }

        private void txtMo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPrintQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        

        private void RadioKPClick(object sender, RoutedEventArgs e)
        {
            thisKP = (e.Source as RadioButton).Content.ToString();
            RadioButton rd = e.Source as RadioButton;
        }

        public object lastSender;
        public RoutedEventArgs lastE;
        private async void RadioPRGClick(object sender, RoutedEventArgs e)
        {
            lastSender = sender;
            lastE = e;
            GridMain.ItemsSource = null;
            thisProgram = (e.Source as RadioButton).Content.ToString();
            await radioEvent(lastSender, lastE);
            dtTemp = new DataTable();
            dtTemp.Columns.Add("Label", typeof(string));
            dtTemp.Columns.Add("Flag", typeof(int));
            dtTemp.Columns.Add("Status", typeof(string));

            ShowCheck = true;
        }
        private async Task radioEvent(object sender, RoutedEventArgs e)
        {
            if (chkReprint.IsChecked == false)
            {
                await call_SP("", "RadioPRGClick");
                if (res.StartsWith("OK"))
                {
                    txtVersionCode.Text = getVal(output, "VersionCode");
                    txtAlreadyQty.Text = getVal(output, "PrintedQty");
                    txtSNfirst.Text = getVal(output, "SNFirst");
                    txtSNnext.Text = getVal(output, "SNNext");
                    LabelName = getVal(output, "LabelName");
                    txtSNend.Text = "";
                    lblMsg.Content = "";
                    sql = "";
                    sql = getVal(output, "SQL");
                    if (sql != "")
                    {
                        dt = await fDal.ExcuteSelectSQL(sql, MainWindow.sfcClient);
                        GridMain.ItemsSource = dt.DefaultView;
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            PrintAvailable = true;
                            sQty = dt.Rows.Count;
                            btnPrint.Content = "Print " + sQty;
                        }
                        else
                        {
                            PrintAvailable = false;
                            btnPrint.Content = "Print";
                            sQty = 0;
                        }
                    }
                }
            }
        }
        private string getVal(string input,string column)
        {
            string res = "";
            foreach(string arr in input.Split('|'))
            {
                if(arr.StartsWith(column+":"))
                {
                    for(int i=1;i < arr.Split(':').Count();i++)
                    {
                        res += arr.Split(':')[i]+":";
                    }
                    return res.Substring(0,res.Length-1);
                }
            }
            return "";
        }
        private async void txtPrintQty_KeyUp(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)
            {
                if (chkReprint.IsChecked == true)
                {
                    showMessage("Reprint mode enable|Chế độ in bù đang mở");
                    return;
                }
                lblMsg.Content="";
                try
                {
                    if(int.Parse(txtMoQty.Text) < int.Parse(txtPrintQty.Text)+int.Parse(txtAlreadyQty.Text))
                    {
                        if(chkPrintOver.IsChecked==false)
                        {
                            showMessage("Print QTY over MO QTY|Số lượng in vượt quá số lượng công lệnh");
                            return;
                        }
                    }
                }
                catch
                {
                    showMessage("Exception when convert QTY");
                    return;
                }
                string indata = "SNFirst:" + txtSNnext.Text.Trim();
                await call_SP(indata, "QtyEvent");
                if (res.StartsWith("OK"))
                {
                    txtSNend.Text = getVal(output, "SNEnd");
                    lblMsg.Content = "Get SNEND OK,Next to release ";
                    releaseAvaiable = true;
                    btnRelease.Focus();
                }
            }
        }
        private async void btnRelease_Click(object sender, RoutedEventArgs e)
        {
            if(!releaseAvaiable)
            {
                showMessage("Please input data first|Vui lòng nhập dữ liệu trước");
                return;
            }
            if (chkReprint.IsChecked == true)
            {
                showMessage("Reprint mode enable|Chế độ in bù đang mở");
                return;
            }
            lblMsg.Content = "";
            try
            {
                if (int.Parse(txtMoQty.Text) < int.Parse(txtPrintQty.Text) + int.Parse(txtAlreadyQty.Text))
                {
                    if (chkPrintOver.IsChecked == false)
                    {
                        showMessage("Print QTY over MO QTY|Số lượng in vượt quá số lượng công lệnh");
                        return;
                    }
                }
            }
            catch
            {
                showMessage("Exception when convert QTY");
                return;
            }
            string indata = "SNFirst:" + txtSNnext.Text.Trim() +"|SNEnd:"+txtSNend.Text.Trim();
            await call_SP(indata,"Release");
            if (res.StartsWith("OK"))
            {
                PrintAvailable = true;
                await radioEvent(lastSender, lastE);
                btnPrint.Focus();
                lblMsg.Content = "Release OK";
            }
            txtPrintQty.Text = "";
        }
        private async void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            if(chkReprint.IsChecked==true)
            {
                int qty = dtTemp.Select("Flag=0").Count();
                
                if(qty==0)
                {
                    showMessage("No data to re-print|Không có dữ liệu để in bù");
                    return;
                }
                await ReprintLabel();
            }
            else
            {
                if (!PrintAvailable)
                {
                    showMessage("No data to print label|Không có dữ liệu để in");
                    return;
                }
                await PrintLabel();
            }
        }

        private void txtPrintQty_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

        private int _numValue = 1;
        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtLabelMode.Text = value.ToString();
            }
        }
        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            if(NumValue==1)
            NumValue++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            if (NumValue == 2)
                NumValue--;
        }

        private void txtLabelMode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtLabelMode == null)
            {
                return;
            }
            if (!int.TryParse(txtLabelMode.Text, out _numValue))
                txtLabelMode.Text = _numValue.ToString();
        }

        private async void btnVisible_Click(object sender, RoutedEventArgs e)
        {
            PasswordForm pw = new PasswordForm();
            pw.ShowDialog();
            if (string.IsNullOrEmpty(pw.password)) return;
            if (await CheckPrivilege("VISIBLE", pw.password))
            {
                VisibleLabel();
            }
            else
            {
                showMessage("No privilege|Không có quyền hạn");
                return;
            }
        }
        private async void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if(!ShowCheck)
            {
                showMessage("Input MO and select label type first|Nhập công lệnh và chọn loại label trước");
                return;
            }
            CheckLabelForm pw = new CheckLabelForm(txtMo.Text,thisProgram);
            pw.ShowDialog();
        }
        private async Task<bool> CheckPrivilege(string permission, string password)
        {
            string sql = string.Format(@"SELECT count(*)
              FROM SFIS1.C_EMP_DESC_T A,
                   SFIS1.C_PRIVILEGE B
                     WHERE A.EMP_BC = '{0}'
              AND B.PRG_NAME = 'CODESOFT'
                     AND B.FUN = 'REPRINT'
                     AND PRIVILEGE = '2'
                     AND A.EMP_NO = B.EMP", password);

            dt = await fDal.ExcuteSelectSQL(sql, MainWindow.sfcClient);
            if (dt.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }

        private void chkReprint_Checked(object sender, RoutedEventArgs e)
        {
            lblSNEnd.Visibility = Visibility.Hidden;
            txtSNend.Visibility = Visibility.Hidden;

            lblSNNext.Visibility = Visibility.Hidden;
            txtSNnext.Visibility = Visibility.Hidden;

            lblSNLast.Content = "SN Reprint:";
            txtSNfirst.IsReadOnly = false;
            btnPrint.Content = "Re-Print";
        }

        private void chkReprint_Unchecked(object sender, RoutedEventArgs e)
        {
            lblSNEnd.Visibility = Visibility.Visible;
            txtSNend.Visibility = Visibility.Visible;

            lblSNNext.Visibility = Visibility.Visible;
            txtSNnext.Visibility = Visibility.Visible;
            lblSNLast.Content = "SN Last :";
            txtSNfirst.IsReadOnly = true;
            btnPrint.Content = "Print";
        }
        DataTable dtTemp;
        private async  void txtSNfirst_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && chkReprint.IsChecked==true)
            {
                dtTemp.Rows.Add(new object[] { txtSNfirst.Text,0, "Waiting reprint"});
                GridMain.ItemsSource = dtTemp.DefaultView;
            }
        }

        private void VisibleLabel()
        {
            if (File.Exists(FilePath))
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
                    ShowMessageForm _sh = new ShowMessageForm();
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
        private void btnShowParam_Click(object sender, RoutedEventArgs e)
        {
            ShowParam();
        }
        private void ShowParam()
        {
            ShowParam _sp = new ShowParam();
            _sp.dataGrid.DataContext = dtParams.DefaultView;

            if (_sp.dataGrid.DataContext == null)
            {

                return;

            }
            _sp.txtURLLabel.Text = UrlLabelFile + LabelName;
            _sp.ShowDialog();
        }
        private void showMessage(string content)
        {
            string _messVN= content,  _messEN = content;
            if(content.IndexOf('|') != -1 )
            {
                _messEN = content.Split('|')[0].Trim();
                _messVN = content.Split('|')[1].Trim();
            }
            ShowMessageForm _sh = new ShowMessageForm();
            _sh.CustomFlag = true;
            _sh.MessageVietNam = _messVN;
            _sh.MessageEnglish = _messEN;
            _sh.ShowDialog();
            lblMsg.Content = _messVN;
            lblMsg.Foreground = new System.Windows.Media.SolidColorBrush(Colors.Red);
        }
        private void showMessage(string MessageEnglish, string MessageVietNam, bool CustomFlag)
        {
            ShowMessageForm frmMessage = new ShowMessageForm();
            frmMessage.httpclient = MainWindow.sfcClient;
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
        public async Task call_SP(string in_data,string in_func)
        {
            var result = await MainWindow.sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_CODESOFT_NIC",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="AP_VER",Value=MainWindow.APVersion,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_EMP",Value=MainWindow.empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="PCMAC",Value=MainWindow.PCMAC,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="PCIP",Value=MainWindow.PCIP,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_FUNC",Value=in_func,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_DATA",Value=in_data,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_LABELTYPE",Value=thisProgram,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_MO",Value=txtMo.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_QTY",Value=txtPrintQty.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
 
            });
            if (result.Data != null)
            {
                dynamic ads = result.Data;
                output = ads[0]["out_data"];
                res = ads[1]["res"];
                if (!res.StartsWith("OK"))
                {
                    showMessage(res);
                    return;
                }
                if(in_func=="PRINT") await AddParams(output);
            }
        }
        private async Task clear()
        {
            GridMain.ItemsSource = null;
            btnVisible.IsEnabled = false;
            btnShowParam.IsEnabled = false;
            chkPrintOver.IsEnabled = false;
            chkReprint.IsEnabled = false;
            txtMoQty.Text = "";
            txtAlreadyQty.Text = "";
            txtModelName.Text = "";
            txtVersionCode.Text = "";
            txtSNfirst.Text = "";
            txtSNnext.Text = "";
            txtSNend.Text = "";
            RadioButtonList_Program.ItemsSource = null;
            RadioButtonList_PN.ItemsSource = null;
            releaseAvaiable = false;
            PrintAvailable = false;
        }
        public void AddParams(string _name, string _value)
        {
            if (dtParams.Columns.Count == 0)
            {
                dtParams.Columns.Add("Name");
                dtParams.Columns.Add("Value");
            }
            dtParams.Rows.Add(new object[] { _name, _value });
        }
        public async Task AddParams(string _Param )
        {
            if (dtParams == null) dtParams = new DataTable();
            if (dtParams.Columns.Count == 0)
            {
                dtParams.Columns.Add("Name");
                dtParams.Columns.Add("Value");
            }
            foreach (var rows in _Param.Split('|'))
            {
                try
                {
                    if (rows.Split(':')[0].ToString() != "LabelName")
                        dtParams.Rows.Add(new object[] { rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "" });
                }
                catch { }
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
        public async Task<bool> downloadLabel()
        {
            string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory;
            FilePath = _DirPath + LabelName;
            //string FilePathBak = _DirPath + _labelNameBak;
            //remove old file label
            sflag = true;
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
                            MessageBox.Show("Không thể xóa đóng process { 0}. Lỗi ngoại lệ: { 1} Hãy đóng chương trình và thử lại  |" + errorMessage);
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
                        //MessageBoxHelper.Display(errorMessage);

                        MessageBox.Show(string.Format("Không thể xóa file {0}. Lỗi ngoại lệ: {1}", FilePath, ex_e.Message) + " | " + string.Format("Cannot delete file {0}. Exception: {1}", FilePath, ex_e.Message));

                        return false;
                    }

                }
            }
            //down file label
            if (!File.Exists(FilePath))
            {
                try
                {
                    WebClient wc = new WebClient();
                    UrlLabelFile = MainWindow.UrlLabelFile;
                    wc.DownloadFile(UrlLabelFile + LabelName, FilePath);
                }
                catch (Exception exc)
                {
                    if (exc.Message.Equals("The remote server returned an error: (404) Not Found."))
                    {
                        MessageBox.Show("Không tìm thấy file label,kiểm tra lại." + " | " + "Label file not found, call Labelroom check." + Environment.NewLine + "Url: " + Environment.NewLine + UrlLabelFile + LabelName);
                        return false;
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy file label,kiểm tra lại | Can not download label file. Url: " + UrlLabelFile + exc.Message);
                        return false;
                    }
                }
            }
            _lbt.sfcHttpClient = MainWindow.sfcClient;

            try
            {
                LabApp = new LabelManager2.Application();
                LabApp.Documents.Open(FilePath, false);
                doctemp = LabApp.ActiveDocument;
                //Check MD5 of label file
                if (_ChkMD5Flag)
                {
                    if (!await _lbt.doMD5Label(LabelName, 6, true))
                    {
                        return false;
                    }
                    _ChkMD5Flag = false;
                }
                dtPqe = new DataTable();
                dtPqe = await _lbt.getPQESetup(LabelName, 6);
                sPercent = await _lbt.getLabelSetupPercent();
                sStatus = await _lbt.getLabeStatus(LabelName, 6);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LOI CODESOFT " + ex.ToString(), "Warning");
                return false;
            }
        }

        public async Task<bool> PrintLabel()
        {
            int QTY = dt.Rows.Count;
            try
            {
                bool isNeedBreak;
                await killprocess();

                List<string> lstSSN = new List<string>();

                if (isPrinting)
                {
                    showMessage("Printing|Chương trình đang in");
                    return false;
                }
                if (await downloadLabel() == false) return false;
                isPrinting = true;
                
                for (int k = 0; k < QTY; k = k + NumValue)
                {
                    if (dtParams != null)  dtParams.Clear();

                    string indata = "LabelMode:" + NumValue.ToString();
                    await call_SP(indata, "PRINT");
                    if (!res.StartsWith("OK"))
                    {
                        return false;
                    }
                    await radioEvent(lastSender, lastE);
                    DoEvents();

                    #region CallCodesoftToPrint
                    string _param_Name = "";

                    try
                    {
                        
                        for (int j = 1; j < doctemp.Variables.FormVariables.Count + 1; j++)
                        {
                            string lbLabelName = doctemp.Variables.FormVariables.Item(j).Name;
                            doctemp.Variables.FormVariables.Item(lbLabelName).Value = "";
                        }
                        // Set value into label file
                        foreach (DataRow param in dtParams.Rows)
                        {
                            _param_Name = param["Name"].ToString().ToUpper();
                            string vl = param["Value"].ToString();

                            try
                            {
                                for (int z = 1; z < doctemp.Variables.FormVariables.Count + 1; z++)
                                {
                                    if (doctemp.Variables.FormVariables.Item(z).Name.ToUpper() == _param_Name)
                                    {
                                        doctemp.Variables.FormVariables.Item(_param_Name).CounterUse = 0;
                                        doctemp.Variables.FormVariables.Item(_param_Name).Value = param["Value"]?.ToString() ?? "";
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string a = e.Message.ToString();
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

                        for (int w = 1; w <= TotalText; w++)
                        {
                            var _name = doctemp.DocObjects.Texts.Item(w).Name;
                            if (_name != null)
                            {
                                var _var = doctemp.DocObjects.Texts.Item(w).VariableName;
                                var _data = doctemp.DocObjects.Texts.Item(w).Value;
                                var _font = doctemp.DocObjects.Texts.Item(w).Font.Name;
                                dtbVarName.Rows.Add(new object[] { _name, _data, _font });
                            }
                        }

                        for (int q = 1; q <= TotalBarcode; q++)
                        {
                            var _name = doctemp.DocObjects.Barcodes.Item(q).Name;
                            if (_name != null)
                            {
                                var _var = doctemp.DocObjects.Barcodes.Item(q).VariableName;
                                var _bar = doctemp.DocObjects.Barcodes.Item(q).Symbology.ToString().Replace("lppx", "").Trim();
                                var _data = doctemp.DocObjects.Barcodes.Item(q).Value;
                                dtbVarName.Rows.Add(new object[] { _name, _data, _bar });
                            }
                        }
                        //Call label tracking
                        doctemp.PrintDocument(0);
                        try
                        {
                            if (!await _lbt.doLabelTracking_AddParamValue(LabelName, 6, dtParams, doctemp, dtbVarName, dtPqe, sPercent, sStatus, true))
                            {
                                sflag = false;
                                goto PRINTERROR;
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageForm _sh = new ShowMessageForm();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message;
                            _sh.MessageEnglish = "Check label tracking have exception: " + ex.Message;
                            _sh.ShowDialog();

                            sflag = false;
                            goto PRINTERROR;
                        }

                        doctemp.PrintDocument(1);
                        doctemp.FormFeed();
                    }
                    catch (Exception ex)
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "Phát sinh lỗi khi in label: " + ex.Message;
                        _sh.MessageEnglish = "Have exceptions when pint document: " + ex.Message;
                        _sh.ShowDialog();
                        sflag = false;
                        goto PRINTERROR;
                    }
                #endregion
                // bool sFlag = await CallCodesoftToPrint(/*doc,*/ LabelName, dtParams);

                PRINTERROR:
                    if (!sflag)
                    {
                        prLoading.Value = 100;
                        lblLoading.Content = "100%";

                        DoEvents();
                        break;
                    }
                    await prLoading.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                    {
                        float prValue = (((float)k + NumValue) / (float)QTY) * 100;
                        prLoading.Value = prValue;
                        lblLoading.Content = Math.Round(prValue) + "%";
                        return null;
                    }), null);
                    DoEvents();
                }
            }
            finally
            {
                isPrinting = false;
                try
                {
                    LabApp.Documents.CloseAll();
                    LabApp.Quit();
                    doctemp = null;
                    LabApp = null;
                    await killprocess();
                }
                catch { }
            }
            return true;
        }
        public async Task ReprintLabel()
        {
            try
            {
                bool isNeedBreak;
                await killprocess();

                List<string> lstSSN = new List<string>();

                if (isPrinting)
                {
                    showMessage("Printing|Chương trình đang in");
                    return ;
                }
                if (await downloadLabel() == false) return ;
                isPrinting = true;
                int i = 1,k=0;
                int QTY = dtTemp.Select("Flag=0").Count();
                foreach (DataRow row in dtTemp.Select("Flag=0"))
                {
                    k++;
                    DoEvents();
                    row[1] = 1;
                    string indata = "index:" + i + "|data:" + row[0];
                    await call_SP(indata, "Reprint");
                    if (res.StartsWith("OK"))
                    {
                        i++;
                        AddParams(output);
                        if (i.ToString() == txtLabelMode.Text)
                        {
                            #region CallCodesoftToPrint
                            string _param_Name = "";

                            try
                            {

                                for (int j = 1; j < doctemp.Variables.FormVariables.Count + 1; j++)
                                {
                                    string lbLabelName = doctemp.Variables.FormVariables.Item(j).Name;
                                    doctemp.Variables.FormVariables.Item(lbLabelName).Value = "";
                                }
                                // Set value into label file
                                foreach (DataRow param in dtParams.Rows)
                                {
                                    _param_Name = param["Name"].ToString().ToUpper();
                                    string vl = param["Value"].ToString();

                                    try
                                    {
                                        for (int z = 1; z < doctemp.Variables.FormVariables.Count + 1; z++)
                                        {
                                            if (doctemp.Variables.FormVariables.Item(z).Name.ToUpper() == _param_Name)
                                            {
                                                doctemp.Variables.FormVariables.Item(_param_Name).CounterUse = 0;
                                                doctemp.Variables.FormVariables.Item(_param_Name).Value = param["Value"]?.ToString() ?? "";
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        string a = e.Message.ToString();
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

                                for (int w = 1; w <= TotalText; w++)
                                {
                                    var _name = doctemp.DocObjects.Texts.Item(w).Name;
                                    if (_name != null)
                                    {
                                        var _var = doctemp.DocObjects.Texts.Item(w).VariableName;
                                        var _data = doctemp.DocObjects.Texts.Item(w).Value;
                                        var _font = doctemp.DocObjects.Texts.Item(w).Font.Name;
                                        dtbVarName.Rows.Add(new object[] { _name, _data, _font });
                                    }
                                }

                                for (int q = 1; q <= TotalBarcode; q++)
                                {
                                    var _name = doctemp.DocObjects.Barcodes.Item(q).Name;
                                    if (_name != null)
                                    {
                                        var _var = doctemp.DocObjects.Barcodes.Item(q).VariableName;
                                        var _bar = doctemp.DocObjects.Barcodes.Item(q).Symbology.ToString().Replace("lppx", "").Trim();
                                        var _data = doctemp.DocObjects.Barcodes.Item(q).Value;
                                        dtbVarName.Rows.Add(new object[] { _name, _data, _bar });
                                    }
                                }
                                //Call label tracking
                                doctemp.PrintDocument(0);
                                try
                                {
                                    if (!await _lbt.doLabelTracking_AddParamValue(LabelName, 6, dtParams, doctemp, dtbVarName, dtPqe, sPercent, sStatus, true))
                                    {
                                        sflag = false;
                                        goto PRINTERROR;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ShowMessageForm _sh = new ShowMessageForm();
                                    _sh.CustomFlag = true;
                                    _sh.MessageVietNam = "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message;
                                    _sh.MessageEnglish = "Check label tracking have exception: " + ex.Message;
                                    _sh.ShowDialog();

                                    sflag = false;
                                    goto PRINTERROR;
                                }

                                doctemp.PrintDocument(1);
                                doctemp.FormFeed();
                                dtParams.Clear();
                            }
                            catch (Exception ex)
                            {
                                ShowMessageForm _sh = new ShowMessageForm();
                                _sh.CustomFlag = true;
                                _sh.MessageVietNam = "Phát sinh lỗi khi in label: " + ex.Message;
                                _sh.MessageEnglish = "Have exceptions when pint document: " + ex.Message;
                                _sh.ShowDialog();
                                sflag = false;
                                goto PRINTERROR;
                            }
                        #endregion
                        PRINTERROR:
                            if (!sflag)
                            {
                                prLoading.Value = 100;
                                lblLoading.Content = "100%";

                                DoEvents();
                                break;
                            }
                            await prLoading.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                            {
                                float prValue = (((float)k + NumValue) / (float)QTY) * 100;
                                prLoading.Value = prValue;
                                lblLoading.Content = Math.Round(prValue) + "%";
                                return null;
                            }), null);
                            DoEvents();
                            PrintAvailable = false;
                            i = 1;
                        }
                    }
                }
            }
            finally
            {
                isPrinting = false;
                try
                {
                    LabApp.Documents.CloseAll();
                    LabApp.Quit();
                    doctemp = null;
                    LabApp = null;
                    await killprocess();
                }
                catch { }
            }
        }
        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
    }
}
