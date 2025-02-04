using CodeSoft_9Codes.Resources;
using LabelManager2;
using Newtonsoft.Json;
using Sfc.Core.Extentsions;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;

namespace CodeSoft_9Codes.View
{
    /// <summary>
    /// Interaction logic for LabelHHVN.xaml
    /// </summary>
    public partial class LabelHHVN : UserControl
    {
        int minvalue = 0,
        startvalue = 0;
        private string sMo, sModel, sModelType, sEmpReprint;
        private int sTagetMo;
        bool isPrintFlag;
        public static bool isPrinting;
        private SfcHttpClient sfcClient;
        public static DataTable dtParams = new DataTable();
        public static string LabelName = "";
        public static string FilePath = "";
        private static string UrlLabelFile;
        public string sTablePrint = " SFISM4.R_PRINT_INPUT_T ";
        public string sTableBPCS = " SFISM4.R_BPCS_MOPLAN_T ";
        public string sTableC6 = " SFIS1.C_MODEL_DESC_T ";
        private bool _ChkMD5Flag = true;
        private string sEmpNo, sSSN;
        int test = 0;
        DataTable dt;
        LabelTracking _lbt = new LabelTracking();
        //ApplicationClass LabApp;
        LabelManager2.Application LabApp;
        Document doctemp;
        DAL fDal;
        public LabelHHVN(SfcHttpClient _sfc, string _empname, string _empno)
        {
            InitializeComponent();
            txtSnCount.Text = startvalue.ToString();
            sfcClient = _sfc;
            txtEmp.Text = _empname;
            sEmpNo = _empno;
            fDal = new DAL();
            dt =  new DataTable();
            ((INotifyCollectionChanged)lvList.Items).CollectionChanged += ListBox_CollectionChanged;
            ((INotifyCollectionChanged)lvPrint.Items).CollectionChanged += ListBox2_CollectionChanged;
        }


        private void ListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (VisualTreeHelper.GetChildrenCount(lvList) > 0)
                {
                    Border border = (Border)VisualTreeHelper.GetChild(lvList, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scrollViewer.ScrollToBottom();
                }
                //lbList.SelectedIndex = lbList.Items.Count - 1;
                //lbList.ScrollIntoView(lbList.SelectedItem);
            }
        }

        private void ListBox2_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (VisualTreeHelper.GetChildrenCount(lvPrint) > 0)
                {
                    Border border = (Border)VisualTreeHelper.GetChild(lvPrint, 0);
                    ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                    scrollViewer.ScrollToBottom();
                }
            }
        }
        private void BtnCheck_MouseEnter(object sender, MouseEventArgs e)
        {
            //btnCheck.Background = (Brush)new BrushConverter().ConvertFrom("#0889A6");
        }

        private void btnCheck_MouseLeave(object sender, MouseEventArgs e)
        {
            btnCheck.Background = (Brush)new BrushConverter().ConvertFrom("#034E70");
        }


        private void NUDButtonUP_Click(object sender, RoutedEventArgs e)
        {
            int number;
            if (string.IsNullOrEmpty(cbLabelQty.Text))
            {
                MessageBox.Show("Please choose Label Qty | Chưa chọn số lượng in label", "Warning");
                return;
            }
            if (txtSnCount.Text != "") number = Convert.ToInt32(txtSnCount.Text);
            else number = 0;
            //if (number < maxvalue)
            txtSnCount.Text = Convert.ToString(number + int.Parse(cbLabelQty.Text));
        }

        private void NUDButtonDown_Click(object sender, RoutedEventArgs e)
        {
            int number;
            if (string.IsNullOrEmpty(cbLabelQty.Text))
            {
                MessageBox.Show("Please choose Label Qty | Chưa chọn số lượng in label", "Warning");
                return;
            }
            if (txtSnCount.Text != "") number = Convert.ToInt32(txtSnCount.Text);
            else number = 0;
            if (number > minvalue)
                txtSnCount.Text = Convert.ToString(number - int.Parse(cbLabelQty.Text));
        }

        private void NUDTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Up)
            {
                NUDButtonUP.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonUP, new object[] { true });
            }


            if (e.Key == Key.Down)
            {
                NUDButtonDown.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonDown, new object[] { true });
            }
        }

        private void dtg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = e.PropertyName.ToUpper().Replace("_", "__");
        }

        private void NUDTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonUP, new object[] { false });

            if (e.Key == Key.Down)
                typeof(Button).GetMethod("set_IsPressed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(NUDButtonDown, new object[] { false });
        }

        private void btnCheck_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (chkReprint.IsChecked == false && txtSnCount.Text == "0")
            {
                MessageBox.Show("Please check, Print QTY cannot be 0 ! | Hãy kiểm tra lại, Số lượng in không thể là 0!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DoBusy(true);
        }

        private void txtMo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnCheck_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {

        }

        private async void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dt = null;
                if (string.IsNullOrEmpty(txtMo.Text))
                {
                    MessageBox.Show("Please input MO ! | Hãy nhập công lệnh!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!(bool)chkReprint.IsChecked && txtSnCount.Text == "0")
                {
                    MessageBox.Show("Please check, Print QTY cannot be 0 ! | Hãy kiểm tra lại, Số lượng in không thể là 0!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DoBusy(true);
                if (!(bool)chkReprint.IsChecked)
                {
                    if (Have_print_too_much() && !(bool)chkPrintOver.IsChecked)
                    {
                        MessageBox.Show("Have print to much! | In vượt quá số lượng công lệnh", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (!await Prepare_Grid())
                    {
                        MessageBox.Show("No have data to print! | Không còn dữ liệu để in, Hãy kiểm tra lại!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (int.Parse(txtSnCount.Text) != dt.Rows.Count)
                    {
                        MessageBox.Show("Số lượng dữ liệu có thể in là " + dt.Rows.Count.ToString() + " khác " + " với số lượng cần in là " + txtSnCount.Text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                }
                else
                {
                    if (!await Prepare_Grid_Reprint())
                    {
                        MessageBox.Show("No have data to re print! | Không còn dữ liệu để in bù, Hãy kiểm tra lại!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (int.Parse(txtSnCount.Text) % 2 == 0 && cbLabelQty.Text != "2")
                    {
                        MessageBox.Show("QTY is even number but label QTY not even ..");
                        return;
                    }
                }
                btnOK.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                DoBusy(false);
            }
        }

        private async void havePrintQty()
        {
            string sql = $"select count(1) bb from  {sTablePrint} where mo_number='{sMo}' and print_flag='Y' and group_name is not null";
            dt =await fDal.ExcuteSelectSQL(sql, sfcClient);
            if (dt.Rows.Count > 0)
            {
                txtPrinted.Text = dt.Rows[0]["bb"].ToString();
            }
        }
        private void FitToContent()
        {
            // where dg is my data grid's name...
            foreach (DataGridColumn column in dtg.Columns)
            {
                //if you want to size your column as per the cell content
                column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToCells);
                //if you want to size your column as per the column header
                column.Width = new DataGridLength(1.0, DataGridLengthUnitType.SizeToHeader);
                //if you want to size your column as per both header and cell content
                column.Width = new DataGridLength(1.0, DataGridLengthUnitType.Auto);
            }
        }
        private bool Have_print_too_much()
        {
            // RESULT:=(STRTOINT(motarget.Text)<STRTOINT(alreadyprint.Text)+STRTOINT(ssncount.Text));
            return int.Parse(txtTarget.Text) < int.Parse(txtPrinted.Text) + int.Parse(txtSnCount.Text);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtMo.Focus();
        }

        private async  void txtMo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMo.Text = txtMo.Text.Trim();
                if (string.IsNullOrEmpty(txtMo.Text)) return;
                string sql = $"SELECT A.MO_NUMBER,A.TARGET_QTY,B.MODEL_NAME,B.MODEL_TYPE  FROM {sTableBPCS} A, {sTableC6} B    WHERE A.MODEL_NAME=B.MODEL_NAME AND A.MO_NUMBER =  '{txtMo.Text}'";
                dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                if (dt.Rows.Count > 0)
                {
                    sMo = txtMo.Text;
                    txtModel.Text = dt.Rows[0]["model_name"].ToString();
                    txtModelType.Text = dt.Rows[0]["model_type"].ToString();
                    txtTarget.Text = dt.Rows[0]["target_qty"].ToString();
                    havePrintQty();
                }
                else
                {
                    MessageBox.Show("THIS " + txtMo.Text + " MO_NUMBER NOT FOUND OR NOT DOWNLOAD !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                txtMo.IsEnabled = false;
            }
        }

        private async Task< bool> Prepare_Grid()
        {
            string sql = $"SELECT * FROM  ( select SSN1,SSN2,SSN3,SSN4,SSN5,SSN6,SSN7,SSN8,SSN9,SSN10, SSN11,SSN12,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5  from  {sTablePrint} where mo_number='{sMo}' and print_flag='N'  ORDER BY SSN1,SSN2,SSN3,MAC1  ) WHERE ROWNUM<= '{txtSnCount.Text}' ORDER BY SSN1,SSN2,SSN3,SSN4,MAC1,MAC2,MAC3,MAC4,MAC5";
            dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
            if (dt.Rows.Count >  0)
            {
                txtStartSN.Text = dt.Rows[0]["ssn1"].ToString();
                txtEndSN.Text = dt.Rows[dt.Rows.Count - 1]["ssn1"].ToString();
                lblQty.Content = "Total print Qty: " + dt.Rows.Count.ToString() + " (pcs)";
            }
            else
            {
                return false;
            }
            return true;
        }

        private async Task<bool> Prepare_Grid_Reprint()
        {
            string select = (bool)chkLabelNN.IsChecked ? "SSN1,SSN1 as SSN2,SSN1 as SSN3,SSN4,SSN2 as SSN5,SSN6,SSN7,SSN8,SSN9,SSN10,SSN3 as SSN11,SSN12,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5" : "SSN1,SSN2,SSN3,SSN4,SSN5,SSN6,SSN7,SSN8,SSN9,SSN10, SSN11,SSN12,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5";
            
            string sql = $"SELECT {select}  FROM {sTablePrint}  where (SSN1 BETWEEN '{txtStartSN.Text}'  AND '{txtEndSN.Text}' OR SSN2 BETWEEN '{txtStartSN.Text}'  AND '{txtEndSN.Text}'  OR MAC1 BETWEEN '{txtStartSN.Text}'  AND '{txtEndSN.Text}') and mo_number = '{sMo}' and print_flag='Y' and group_name is NOT null ORDER BY SSN1,SSN2,SSN3,SSN4,SSN5,MAC1,MAC2,MAC3";
            dt =await fDal.ExcuteSelectSQL(sql, sfcClient);
            if (dt.Rows.Count > 0)
            {
                txtStartSN.Text = dt.Rows[0]["ssn1"].ToString();
                txtEndSN.Text = dt.Rows[dt.Rows.Count - 1]["ssn1"].ToString();
                lblQty.Content = "Total print Qty: " + dt.Rows.Count.ToString() + " (pcs)";
                txtSnCount.Text = dt.Rows.Count.ToString();
            }
            else
            {
                return false;
            }
            return true;
        }

        private void btnShowParam_Click(object sender, RoutedEventArgs e)
        {
            ShowParam();
        }

        private void lv1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private void lb1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (cbLabelQty.SelectedIndex == -1)
                {
                    MessageBox.Show("Please choose label qty | Chưa chọn số lượng label", "Warning");
                    return;
                }
                lvList.Items.Clear();
                //btnOK.IsEnabled = false;
                isPrintFlag = false;
                lblNotify.Content = "Đang in | Printing!!!";
                await Normal_Print();
                //self.Menul.Checked:= false;
                havePrintQty();
            }
            finally
            {
                isPrinting = false;
                btnOK.IsEnabled = false;
                lblNotify.Content = "Đã in xong!!! | Print done!!!";
            }
        }
        private async Task<string> Normal_Print()
        {
            try
            {
                bool isNeedBreak;
                string s = "";
                await killprocess();
                LabelName = (bool)chkLabelNN.IsChecked ? txtModel.Text + "_NN.LAB" : txtModel.Text + "_HH.LAB";

                List<string> lstSSN = new List<string>();

                if (isPrinting)
                {
                    lblNotify.Content = "Printing | Chương trình đang in !!!";
                    //MessageBox.Show("Printing ! Chương trình đang in !!!");
                    return "NG";
                }
                isPrinting = true;
                int i = 1;
                int t = 1;
                string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory;
                FilePath = _DirPath + LabelName;
                //string FilePathBak = _DirPath + _labelNameBak;
                //remove old file label
                bool sflag = true;
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
                                return "";

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

                            return "";
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
                            MessageBox.Show("Không tìm thấy file label, gọi Labelroom kiểm tra." + " | " + "Label file not found, call Labelroom check." + Environment.NewLine + "Url: " + Environment.NewLine + UrlLabelFile + LabelName);
                            return "";
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy file label, gọi Labelroom kiểm tra. | Can not download label file. Url: " + UrlLabelFile + exc.Message);
                            return "";
                        }
                    }
                }
                _lbt.sfcHttpClient = sfcClient;
                
                try
                {
                    //LabApp = new ApplicationClass();
                    LabApp = new LabelManager2.Application();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("LOI CODESOFT " + ex.ToString(), "Warning");
                    return "";
                }
                LabApp.Documents.Open(FilePath, false);
                doctemp = LabApp.ActiveDocument;
                //Check MD5 of label file
                if (_ChkMD5Flag)
                {
                    if (!await _lbt.doMD5Label(LabelName, 6, true))
                    {
                        return "";
                    }
                    _ChkMD5Flag = false;
                }
                DataTable dtPqe = new DataTable();
                dtPqe = await _lbt.getPQESetup(LabelName, 6);
                string sPercent = await _lbt.getLabelSetupPercent();
                string sStatus = await _lbt.getLabeStatus(LabelName, 6);

                for (int k = 0; k < dt.Rows.Count; k++)
                {
                    //this.Dispatcher.Invoke(new Action(async() => {
                    // code của bạn
                    if (cbLabelQty.Text == "1")
                    {
                        dtParams.Clear();
                        t = 1;
                        s = string.Join("|", dt.Rows[k].ItemArray.Select(p => p?.ToString() == "" ? "*" : p.ToString()).ToArray());
                        //SSN1,SSN2,SSN3,SSN4,SSN5,SSN6,SSN7,SSN8,SSN9,SSN10, SSN11,SSN12,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5
                        lvList.Items.Add(s);
                        AddParams("MO", txtMo.Text);
                        sSSN = dt.Rows[k]["ssn1"]?.ToString();
                        AddParams("SSN" + t.ToString() + "1", dt.Rows[k]["ssn1"]?.ToString());
                        //AddParams("SSN" + t.ToString() + "2", dt.Rows[k]["ssn2"]?.ToString());
                        DoEvents();
                    }
                    else
                    {
                        dtParams.Clear();
                        AddParams("MO", txtMo.Text);
                        lstSSN.Clear();
                        for (t = 1; t <= int.Parse(cbLabelQty.Text); t++)
                        {
                            s = string.Join("|", dt.Rows[k].ItemArray.Select(p => p?.ToString() == "" ? "*" : p.ToString()).ToArray());
                            //SSN1,SSN2,SSN3,SSN4,SSN5,SSN6,SSN7,SSN8,SSN9,SSN10, SSN11,SSN12,MO_NUMBER,MODEL_NAME,MAC1,MAC2,MAC3,MAC4,MAC5
                            lvList.Items.Add(s);
                            lstSSN.Add(dt.Rows[k]["ssn1"]?.ToString());
                            AddParams("SSN" + t.ToString(), dt.Rows[k]["ssn1"]?.ToString());
                            //AddParams("SSN" + t.ToString(), dt.Rows[k]["ssn2"]?.ToString());
                            if (t < int.Parse(cbLabelQty.Text))
                                if (k < dt.Rows.Count - 1) k++; else break;
                            DoEvents();
                        }
                        sSSN = string.Join("','", lstSSN);
                    }
                    lvList.Items.Add("---------------------------------------" + k);

                    //bool sflag = await PrintToCodeSoft("M", txtModel.Text);
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
                            catch(Exception e)
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

                        try
                        {
                            //doctemp.Save();
                            //doctemp.Close();
                            //LabApp.Documents.Open(FilePath, false);
                            //doctemp = LabApp.ActiveDocument;
                            //chon
                            if (!await _lbt.doLabelTracking_AddParamValue(LabelName, 6, dtParams, doctemp, dtbVarName,dtPqe,sPercent, sStatus, true))
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
                           
                        
                        //LabApp.Documents.CloseAll();
                        //LabApp.Quit();
                        //doctemp = null;
                        //LabApp = null;
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
                    if (sflag && !(bool)chkReprint.IsChecked && !(bool)chkLabelNN.IsChecked)
                    {

                        foreach (DataRow param in dtParams.Rows)
                        {
                            if (param["Name"].ToString().ToUpper().Contains("SSN"))
                            {
                                string sql =string.Format(@"update SFISM4.R_PRINT_INPUT_T set in_station_time = sysdate, print_flag = 'Y' where ssn1 ='{0}'", param["Value"].ToString());
                                await fDal.ExcuteNonQuerySQL(sql, sfcClient);
                            }
                        }
                    }
                    else
                    {
                        if (!sflag)
                        {
                            prLoading.Value = 100;
                            lblLoading.Content = "100%";

                            DoEvents();
                            break;
                        }
                        
                    }; 
                    await prLoading.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
                    {
                        float prValue = (((float)k + 1) / (float)dt.Rows.Count) * 100;
                        prLoading.Value = prValue;
                        lblLoading.Content = Math.Round(prValue) + "%";
                        return null;
                    }), null);
                    DoEvents();
                    // }));

                    //Thread.Sleep(5);
                }
            }
            finally
            {
                LabApp.Documents.CloseAll();
                LabApp.Quit();
                doctemp = null;
                LabApp = null;
                await killprocess();
            }


            if ((bool)chkReprint.IsChecked) saveLog("REPRINT"); else saveLog("NORMAL");
            txtStartSN.Clear();
            txtEndSN.Clear();
            return "OK";
        }


        private async void saveLog(string _type)
        {
            //string sql = "INSERT INTO SFISM4.R_PRINT_REPRINT_T (MODEL_NAME,MO_NUMBER,PRINTED_ST,PRINTED_ED,DATA_TYPE,SPEDIT_ID,PRINTED_QTY,REPRINT_REASON,EMP_NO,CREATE_DATE,PPID_ST,PPID_ED)  VALUES ('{0}','{1}','{2}','{3}','{4}','',{5},'{6}','{7}',sysdate,'{8}','{9}')";
            //var updatePrint = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            //{
            //    CommandText = string.Format(sql, txtModel.Text, txtMo.Text, txtStartSN.Text, txtEndSN.Text, _type, dt.Rows.Count, "LY DO IN BU", sEmpNo, MainWindow.GetLocalIPAddress(), MainWindow.GetMACAddress()),
            //    SfcCommandType = SfcCommandType.Text
            //});
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

        public void AddParams(string _name, string _value)
        {
            if (dtParams.Columns.Count == 0)
            {
                dtParams.Columns.Add("Name");
                dtParams.Columns.Add("Value");
            }
            dtParams.Rows.Add(new object[] { _name, _value });
        }

        private async Task<bool> killprocess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
            {
                process.Kill();
            }
            return true;            
        }
        public async Task<bool> PrintToCodeSoft(string ParamKind, string model_name)
        {
            await killprocess();

            string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory;
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
                        MessageBox.Show("Không tìm thấy file label, gọi Labelroom kiểm tra." + " | " + "Label file not found, call Labelroom check." + Environment.NewLine + "Url: " + Environment.NewLine + UrlLabelFile + LabelName);
                        return false;
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy file label, gọi Labelroom kiểm tra. | Can not download label file. Url: " + UrlLabelFile + exc.Message);
                        return false;
                    }
                }
            }

            // ApplicationClass LabApp = new ApplicationClass();
            //LabApp.Documents.Open(FilePath, false);
            // Document doc = LabApp.ActiveDocument;

            bool sFlag = await CallCodesoftToPrint(/*doc,*/ LabelName, dtParams);

            return sFlag;
        }
        public static void DoEvents()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        private async void chkVisible_Checked(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkVisible.IsChecked) return;
            PasswordForm pw = new PasswordForm();
            pw.ShowDialog();
            if (string.IsNullOrEmpty(pw.password)) { chkVisible.IsChecked = false; return; }
            if (await CheckPrivilege("VISIBLE", pw.password))
            {
                VisibleLabel();
            }
            else
            {
                chkVisible.IsChecked = false;
            }
        }

        private async Task<bool> CheckPrivilege(string permission, string password)
        {
            string sql=string.Format(@"SELECT count(*)
              FROM SFIS1.C_EMP_DESC_T A,
                   SFIS1.C_PRIVILEGE B
                     WHERE A.EMP_BC = '{0}'
              AND B.PRG_NAME = 'CODESOFT'
                     AND B.FUN = 'REPRINT'
                     AND PRIVILEGE = '2'
                     AND A.EMP_NO = B.EMP",password);

            dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
            if (dt.Rows.Count > 0)
            {
                return true;
            }  
            
            return false;

            //var _data = new
            //{
            //    TYPE = permission,
            //    PRG_NAME = "CODESOFT",
            //    UserName = "V0958187",
            //    Password = password
            //};
            //try
            //{
            //    string _jsondata = JsonConvert.SerializeObject(_data).ToString();
            //    var _result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            //    {
            //        CommandText = "SFIS1.API_EXECUTE",
            //        SfcCommandType = SfcCommandType.StoredProcedure,
            //        SfcParameters = new List<SfcParameter>()
            //        {
            //            new SfcParameter{Name="DATA",Value=_jsondata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
            //            new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
            //        }
            //    });

            //    dynamic _ads = _result.Data;
            //    string _RES = _ads[0]["output"];
            //    string[] _RESArray = _RES.Split('#');

            //    if (_RESArray[0] == "OK")
            //    {
            //        sEmpReprint = _RESArray[1];
            //        return true;
            //    }
            //    else
            //    {

            //        ShowMessageForm _sh = new ShowMessageForm();
            //        _sh.CustomFlag = true;
            //        _sh.MessageVietNam = _RESArray[1];
            //        _sh.MessageEnglish = _RESArray[2];
            //        _sh.ShowDialog();
            //        return false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ShowMessageForm _sh = new ShowMessageForm();
            //    _sh.CustomFlag = true;
            //    _sh.MessageVietNam = ex.Message.ToString();
            //    _sh.MessageEnglish = "Call procedure have exceptions:";
            //    _sh.ShowDialog();
            //    return false;
            //}
        }

        private async void chkReprint_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkReprint.IsChecked) {
                txtSnCount.IsEnabled = true;
                NUDButtonUP.IsEnabled = true;
                NUDButtonDown.IsEnabled = true;
                return;
            };
            PasswordForm pw = new PasswordForm();
            pw.ShowDialog();
            if (string.IsNullOrEmpty(pw.password)) { chkReprint.IsChecked = false; return; };
            if (!await CheckPrivilege("REPRINT", pw.password))
            {
                chkReprint.IsChecked = false;
                return;
            }
            txtSnCount.IsEnabled = false;
            NUDButtonUP.IsEnabled = false;
            NUDButtonDown.IsEnabled = false;
        }

        private async void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (isPrinting)
            {
                MessageBox.Show("Đang in | Printing!!!", "Warning!!!");
                return;
            }
            if (MessageBox.Show("Do you want to close this window?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            //string sql = "SELECT * FROM SFISM4.R_WPAKEY_WPSPIN_T WHERE  mo_number ='2638005368' order by serial_number asc";
            //string sql1 = "UPDATE SFISM4.R_WPAKEY_WPSPIN_T set item= {0} WHERE  mo_number ='2638005368' and serial_number = '{1}' ";
            //var print = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            //{
            //    CommandText = sql,
            //    SfcCommandType = SfcCommandType.Text
            //});
            //int i = 1;
            //if (print.Data.Count() > 0)
            //{
            //    foreach (var rows in print.Data)
            //    {
            //        var updatePrint = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            //        {
            //            CommandText = string.Format(sql1,i, rows["serial_number"].ToString()),
            //            SfcCommandType = SfcCommandType.Text
            //        });

            //        await prLoading.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(delegate
            //        {
            //            float prValue = (((float)i + 1) / (float)print.Data.Count()) * 100;
            //            prLoading.Value = prValue;
            //            lblLoading.Content = Math.Round(prValue) + "%";
            //            return null;
            //        }), null);
            //        i++;
            //        DoEvents();
            //    }
            //}


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

        private async void chkPrintOver_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkPrintOver.IsChecked) return;
            PasswordForm pw = new PasswordForm();
            pw.ShowDialog();
            if (string.IsNullOrEmpty(pw.password)) { chkPrintOver.IsChecked = false; return; }
            if (await CheckPrivilege("PRINT_OVER", pw.password))
            {
              //  VisibleLabel();
            }
            else
            {
                chkPrintOver.IsChecked = false;
            }
        }

        private void chkLabelNN_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkLabelNN.IsChecked)
            {
                sTablePrint = " SFISM4.R_PRINT_INPUT_T ";
                sTableBPCS = " SFISM4.R_BPCS_MOPLAN_T ";
                sTableC6 = " SFIS1.C_MODEL_DESC_T ";
            }
            else
            {
                sTablePrint = " SFISM4.R_PRINT_INPUT_T@VNSFC_NN ";
                sTableBPCS = " SFISM4.R_BPCS_MOPLAN_T@VNSFC_NN ";
                sTableC6 = " SFIS1.C_MODEL_DESC_T@VNSFC_NN ";
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (isPrinting)
            {
                MessageBox.Show("Đang in | Printing!!!", "Warning!!!");
                lblNotify.Content = "Đang in | Printing!!!";
                return;
            }
            txtMo.IsEnabled = true;
            txtMo.Clear();
            txtModel.Clear();
            txtModelType.Clear();
            txtPrinted.Clear();
            txtTarget.Clear();
            dt.Clear();
            BindingOperations.ClearAllBindings(dtg);
            //dtg.Items.Clear();
            txtSnCount.Text = "0";
            lblQty.Content = "";
            lblNotify.Content = "Reset done!!!";
            dtg.Items.Refresh();
            txtStartSN.Clear();
            txtEndSN.Clear();
            txtMo.Focus();
        }

        private void DoBusy(bool _flag)
        {
            if (_flag) Mouse.OverrideCursor = Cursors.Wait; else Mouse.OverrideCursor = Cursors.Arrow;
        }

        public async Task<bool> CallCodesoftToPrint(/*Document doctemp,*/ string LabelName, DataTable dtParams)
        {
            string _param_Name = "";

            try
            {

                _lbt.sfcHttpClient = sfcClient;

                LabApp.Documents.Open(FilePath, false);
                Document doctemp = LabApp.ActiveDocument;
                //Check MD5 of label file
                if (_ChkMD5Flag)
                {
                    if (!await _lbt.doMD5Label(LabelName, 6, true))
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
                    //if (!await _lbt.doLabelTracking_AddParamValue(LabelName, 6, dtParams, doctemp, dtbVarName, true))
                    //{
                    //    return false;
                    //}
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

                doctemp.PrintDocument(1);
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

        private bool After_Prepare_Grid()
        {
            if (int.Parse(txtSnCount.Text) != dt.Rows.Count)
            {
                MessageBox.Show("Số lượng dữ liệu có thể in là " + dt.Rows.Count.ToString() + " khác " + " với số lượng cần in là " + txtSnCount.Text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void NUDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int number = 0;
            if (txtSnCount.Text != "")
                if (!int.TryParse(txtSnCount.Text, out number)) txtSnCount.Text = startvalue.ToString();
            //if (number > maxvalue) txtSnCount.Text = maxvalue.ToString();
            if (number < minvalue) txtSnCount.Text = minvalue.ToString();
            txtSnCount.SelectionStart = txtSnCount.Text.Length;
        }
    }
}
