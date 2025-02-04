using System;
using System.Data;
using System.Windows.Forms;
using PRINT_ALLPARTS._3Layer;
using System.Text.RegularExpressions;
using System.IO;
using LabelManager2;
using System.Reflection;
using System.Diagnostics;
using PRINT_ALLPARTS.Class;
using System.Net;
using System.Net.NetworkInformation;
using System.Linq;
using Sfc.Library.HttpClient;
using PRINT_ALLPART;
using System.Threading;
using System.Drawing;
using System.Collections.Generic;
using Label_Tracking;
using Sfc.Core.Parameters;

namespace PRINT_ALLPARTS
{
    public partial class Form1 : Form
    {
        SfcHttpClient _sfcHttpClient;
        private R105 _R105;
        private BarcodeModelRuleT _BarcodeModel;
        public static DataInputT _DataInputT;
        public VersionData _VersionData;
        public OtherData _OtherData;
        public GetNextsnAllpart _GetNextsnAllpart;
        public FtpData _FtpData;
        //private string _currenBU;
        public PrintALlpartsBLL PrintBLL;
        public static OracleClient _oracle = null;
        private string g_INIPath = System.IO.Directory.GetCurrentDirectory() + "\\PRINT_ALLPART.ini";
        public static string FileName = "ALLPART.LAB", labMode = "1";
        public static List<BuData> BuData;
        public static string val_prefix = "", val_model = "", endSN = "";
        public static int val_notPrintQty = 0, val_PrintedQty = 0;
        public static Boolean Recheck = true;
        public Form1()
        {
            InitializeComponent();
        }
        private async void loadBu()
        {
            PrintBLL = new PrintALlpartsBLL(_sfcHttpClient);
            await PrintBLL.setBu();
            if (BuData != null && BuData.Count > 0 && comboBU.Text.Trim() != "")
            {
                try
                {
                    FileName = BuData.First(p => p.Bu == comboBU.Text).LabName;
                    labMode = BuData.First(p => p.Bu == comboBU.Text).LabMode;
                    lblLabFileName.Text = "LabelFile : " + FileName;
                    txtLabMode.Text = labMode;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("loadBu exception:" + ex.Message);
                }
            }
        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            string loginInfor = "";
            string loginApiUri = "";
            string loginDB = "";
            string empNo = "";
            string empPass = "";
            string Plant = "";
            string inputLogin = "";
            string checkSum;
            string[] Args = Environment.GetCommandLineArgs();
            if (Args.Length == 1)
            {
                MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
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
            _sfcHttpClient = new SfcHttpClient(loginApiUri, loginDB, "helloApp", "123456");
            await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
            _oracle = new OracleClient(_sfcHttpClient);
            string strSQL = string.Empty;
            DataTable dt = new DataTable();
            PrintBLL = new PrintALlpartsBLL(_sfcHttpClient);
            string FilePath = System.Windows.Forms.Application.ExecutablePath;
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(FilePath);
            PrintAllpartsDTO.Instance.isfisVersion = string.Format("{0}.{1}.{2}.{3} ", myFileVersion.FileMajorPart,
            myFileVersion.FileMinorPart, myFileVersion.FileBuildPart, myFileVersion.FilePrivatePart);
            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "PRINT_ALLPART",
                UserName = empNo,
                Password = empPass
            };
            //------------------------------------
            lblIP.Text = "Ip:" + GetIP();
            lblMac.Text = "Mac:" + GetMacAddress();
            PrintAllpartsDTO.Instance.iBU = TIniFile.ReadINI(this.g_INIPath, "PRINT_ALLPART", "BU").ToString().Trim();
            lblVersion.Text = "Version:" + PrintAllpartsDTO.Instance.isfisVersion;
            lblBU.Text = PrintAllpartsDTO.Instance.iBU + " MODEL";
            comboBU.Text = PrintAllpartsDTO.Instance.iBU;
            //------------------------------------
            var res = await PrintBLL.CheckPrivilege(loginInfo);
            if (res.Substring(0, 2) != "OK")
            {
                DialogResult result;
                result = MessageBox.Show(res, "message", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }
            loadBu();
            var _res = await PrintBLL.CheckVersion("PRINT_ALLPART", PrintAllpartsDTO.Instance.isfisVersion);

            if (_res != "OK")
            {
                DialogResult result;
                result = MessageBox.Show(_res, "message", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                if (result == DialogResult.OK)
                {
                    this.Close();
                }
            }
            else
            {
                string FilePathName = System.Windows.Forms.Application.StartupPath.ToString();
                string FullFilePath = FilePathName + "\\" + FileName;

                #region xoa file label cu
                if (System.IO.File.Exists(FullFilePath))
                {
                    try
                    {
                        File.Delete(FullFilePath);
                    }
                    catch
                    {
                        List<Process> lstProcs = new List<Process>();
                        lstProcs = FileUtil.WhoIsLocking(FileName);

                        foreach (Process p in lstProcs)
                        {
                            try
                            {
                                localProcessKill(p.ProcessName);
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show("Cannot kill process " + p.ProcessName + " Exception: " + exc.Message);
                                //showMessage("Cannot kill process " + p.ProcessName + " Exception: " + exc.Message, "Không thể đóng process " + p.ProcessName);
                                return;
                            }
                        }
                        try
                        {
                            File.Delete(FileName);
                        }
                        catch (Exception ex_e)
                        {
                            MessageBox.Show("Cannot delete file " + FileName + ". Exception: " + ex_e.Message);
                            //showMessage("Cannot delete file " + FileName + ". Exception: " + ex_e.Message, "Không thể xóa file " + My_LabelFileName + ". Ngoại lệ: " + ex_e.Message);
                            return;
                        }
                    }
                }
                #endregion

                //progressBar1.Minimum = 0;
            }
        }
        public static bool IsValidData(string strdata)
        {
            string strRegex = "0*[1-9][0-9]*";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(strdata))
                return (true);
            else
                return (false);
        }

        private void TxtMoNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MO_EVENT();
            }
        }
        private async void MO_EVENT()
        {
            PrintBLL = new PrintALlpartsBLL(_sfcHttpClient);
            _R105 = await PrintBLL.CheckMonumberBLL(txtMoNumber.Text);
            if (_R105 == null)
            {
                MessageBox.Show("Công lệnh không tồn tại, kiểm tra lại");
                txtMoNumber.SelectAll();
                return;
            }
            if (!PrintAllpartsDTO.Instance.iBU.StartsWith(_R105.model_serial))
            {
                if (_R105.model_serial == "NIC" || _R105.model_serial == "ECD" || _R105.model_serial == "SUPERCAP"
                 || PrintAllpartsDTO.Instance.iBU == "NIC" || PrintAllpartsDTO.Instance.iBU == "ECD" || PrintAllpartsDTO.Instance.iBU == "SUPERCAP")
                {
                    MessageBox.Show("Công lệnh hàng " + _R105.model_serial + " không được chọn format " + PrintAllpartsDTO.Instance.iBU);
                    txtMoNumber.SelectAll();
                    return;
                }
            }
            _BarcodeModel = await PrintBLL.CheckPrefixLabel(PrintAllpartsDTO.Instance.iBU);
            if (_BarcodeModel == null)
            {
                MessageBox.Show("Chưa thiết lập prefix cho panel.!");
                txtMoNumber.SelectAll();
                return;
            }
            _DataInputT = await PrintBLL.GetLabelPrefix(txtMoNumber.Text, _BarcodeModel.YEAR_MONTH, _BarcodeModel.BARCODE_PREFIX, PrintAllpartsDTO.Instance.iBU);
            if (_DataInputT != null)
            {
                var _VersionData = await PrintBLL.GetVerSion(_R105.model_name, txtMoNumber.Text.Trim(), PrintAllpartsDTO.Instance.iBU);
                if (_VersionData == null)
                {
                    MessageBox.Show("Chưa thiết lập version code!");
                    txtMoNumber.SelectAll();
                    return;
                }
                if (Recheck)
                {
                    string res = await PrintBLL.ShowData(txtMoNumber.Text.Trim());
                    chkPanel.Visible = false;
                    chkSN.Visible = false;
                    if (res.IndexOf("|") > -1)
                    {
                        chkPanel.Visible = true;
                        chkPanel.Checked = false;
                        chkPanel.Text = res.Split('|')[0];
                        chkSN.Visible = true;
                        chkSN.Checked = false;
                        chkSN.Text = res.Split('|')[1];
                    }
                    else
                    {
                        chkPanel.Visible = true;
                        chkPanel.Checked = false;
                        chkPanel.Text = res;
                        chkSN.Visible = false;
                        chkSN.Checked = true;
                        chkSN.Text = "";
                    }
                    Recheck = false;
                }
                txtModelName.Text = _R105.model_name;
                txtTarget.Text = _R105.target_qty;
                txtPrinted.Text = val_PrintedQty.ToString();
                txtLastPrint.Text = _DataInputT.lastdata;
                txtStep.Text = _BarcodeModel.MACID_STEP;
                val_prefix = _DataInputT.prefix;
                txtVersion.Text = _VersionData.version;
                _FtpData = await PrintBLL.GetFtpLabelAsync();
                txtSNPrefix.Text = _BarcodeModel.BARCODE_PREFIX;
                txtTimePrefix.Text = _BarcodeModel.YEAR_MONTH;
                txtLength.Text = _BarcodeModel.BARCODE_LENGTH;
                txtValidChar.Text = _BarcodeModel.VALID_CHAR;
                txtStepPrint.Text = _BarcodeModel.MACID_STEP;
                comboBU.Text = PrintAllpartsDTO.Instance.iBU;
                txtQtyPrint.Text = "1";
                txtQtyPrint.Focus();
                RefreshGrid();
            }
        }
        private void TxtQtyPrint_TextChanged(object sender, EventArgs e)
        {

        }
        private async void RefreshGrid()
        {
            DataTable dt = await PrintBLL.ExecuteSQL("select row_number() over ( order by ssn1 ) as STT,ssn1 PANEL from SFISM4.R_data_INPUT_T where PRINT_FLAG<>'Y' and mo_number ='" + txtMoNumber.Text + "' order by ssn1 ");

            dataGridView1.ReadOnly = true;
            val_notPrintQty = dt.Rows.Count;
            //if (val_notPrintQty > 0) 
            dataGridView1.DataSource = dt;
            //progressBar1.Visible = true;
            //progressBar1.Maximum = val_notPrintQty;
            //progressBar1.Value = 0;
            lblNotprint.Text = "Total : " + val_notPrintQty + " panel";
            dataGridView1.Refresh();
        }
        private async void TxtQtyPrint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (IsValidData(txtQtyPrint.Text))
                {
                    txtQtyPrint.Text = txtQtyPrint.Text;
                }
                else
                {
                    MessageBox.Show("Hãy nhập đúng giá trị");
                    txtQtyPrint.Clear();
                    return;
                }
                if (chkPanel.Visible == true && chkPanel.Checked == false)
                {
                    MessageBox.Show("Hãy xác nhận định dạng panel trước khi release");
                    txtQtyPrint.Clear();
                    return;
                }
                if (chkSN.Visible == true && chkSN.Checked == false)
                {
                    MessageBox.Show("Hãy xác nhận định dạng SN trước khi release");
                    txtQtyPrint.Clear();
                    return;
                }
                PrintBLL = new PrintALlpartsBLL(_sfcHttpClient);
                string firtsnbeforeprint, sn1, lastsn = string.Empty;
                string res = string.Empty;
                int target, input = 0;
                int i = 1;
                input = Convert.ToInt32(txtQtyPrint.Text);
                target = Convert.ToInt32(txtTarget.Text);
                #region create table and coloums
                //dataGridView1.Rows.Clear();
                //dataGridView1.Refresh();
                #endregion

                if (txtQtyPrint.Text != "")
                {
                    _OtherData = await PrintBLL.GetFirstSnWhenPrint(val_prefix, Convert.ToInt32(txtLength.Text));

                    txtFrom.Text = await PrintBLL.GetNextSN(_OtherData.beforeprint, val_prefix, Convert.ToInt32(txtLength.Text), txtValidChar.Text, Convert.ToInt32(txtStep.Text));
                    if (target < val_PrintedQty + input + val_notPrintQty)
                    {
                        DialogResult result;
                        result = MessageBox.Show("Print QTY > Target QTY", "Continue release ?", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            Release();
                        }
                    }
                    else
                    {
                        DialogResult rep;
                        rep = MessageBox.Show("Start release " + txtQtyPrint.Text + " panel", "Release?", MessageBoxButtons.YesNo);
                        if (rep == DialogResult.Yes)
                        {
                            Release();
                        }
                    }
                }
            }
        }
        private async void Release()
        {
            System.DateTime dFrom = System.DateTime.Now;
            btnPrint.Enabled = false;
            PrintBLL = new PrintALlpartsBLL(_sfcHttpClient);
            if (txtFrom.Text == "")
            {
                MessageBox.Show("Please input data");
                return;
            }
            btnPrint.Enabled = false;
            #region check r_mo_ext
            //Removed<=>Check in SP
            //var _rescheck = await PrintBLL.CheckMoExtAsync(txtMoNumber.Text.Trim(), txtModelName.Text.Trim().ToUpper(), txtVersion.Text.Trim(), txtFrom.Text.Trim().ToUpper(), txtTo.Text.Trim().ToUpper(), txtLength.Text);
            //if (_rescheck != "OK")
            //{
            //    MessageBox.Show(_rescheck, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            //    btnPrint.Enabled = true;
            //    return;
            //}
            #endregion

            #region insert data_input_t & r_mo_ext
            string HH_SN = txtFrom.Text.Trim();
            var res = await PrintBLL.SaveInputDataAsync(txtMoNumber.Text.Trim(), HH_SN, Convert.ToInt32(txtQtyPrint.Text), txtVersion.Text.Trim());
            if (res != "OK")
            {
                MessageBox.Show(res, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                btnPrint.Enabled = true;
                return;
            }
            #endregion
            System.DateTime dTo = System.DateTime.Now;
            double rTime = Math.Round(TimeSpan.Parse((dTo - dFrom).ToString()).TotalSeconds, 2);
            MessageBox.Show("Release OK:" + txtQtyPrint.Text + " panel\r\nExecute time:" + rTime + "s");
            btnPrint.Enabled = true;
            MO_EVENT();
        }
        private async void BtnPrint_Click(object sender, EventArgs e)
        {
            System.DateTime dFrom = System.DateTime.Now;
            int printStep = int.Parse(txtLabMode.Text);
            string listPanel = "";
            MO_EVENT();
            if (val_notPrintQty == 0) return;

            string sqlcheck = $"select * from SFISM4.r_data_input_t where MO_NUMBER = '{txtMoNumber.Text}' and PRINT_FLAG = 'Y' ";
            var qry_count = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sqlcheck,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_count.Data.Count() >= 300)
            {
                int index = qry_count.Data.Count() / 300;
                for(int i = 1;i <= index;i++)
                {
                    int c_from = 300 * (i - 1) + 1;
                    int c_to = 300 * i;
                    string sqlchecked = $" select* from(SELECT ROW_NUMBER() OVER (ORDER BY print_time) AS STT, a.*from SFISM4.R_DATA_INPUT_T a " +
                        $" where mo_number = '{txtMoNumber.Text}' and print_Flag = 'Y'" +
                        $" ) where stt between '{c_from}' and '{c_to}' and ssn4 = 'Y' ";
                    var qry_count1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sqlchecked,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if(qry_count1.Data.Count() < 10)
                    {
                        MessageBox.Show($"Labels {c_from} => {c_to} need check 10 labels!", "Aleart!", MessageBoxButtons.OK);
                        Check_Label frm = new Check_Label(txtMoNumber.Text, comboBU.Text, _sfcHttpClient);
                        frm.ShowDialog();
                        return;
                    }

                }                
            }

            #region bat dau in label
            LabelManager2.ApplicationClass lbl = new LabelManager2.ApplicationClass();
            try
            {
                string labelAddress = _FtpData.url;

                string FilePath = System.Windows.Forms.Application.StartupPath.ToString();
                string FullFilePath = FilePath + "\\" + FileName;
                string LabelFtp = "http://" + labelAddress + "/";
                if (!System.IO.File.Exists(FullFilePath))
                {
                    string _result = PrintBLL.GetFileFormServer(LabelFtp, FileName, FullFilePath);
                    if (_result != "OK")
                    {
                        MessageBox.Show(_result + "," + FileName, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        btnPrint.Enabled = true;
                        return;
                    }
                }
                if (!System.IO.File.Exists(FullFilePath))
                {
                    MessageBox.Show("No label found:" + FullFilePath, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    btnPrint.Enabled = true;
                    return;
                }
                #region do label tracking pqe vs label
                //LabelTracking labelTracking = new LabelTracking();
                //if (!labelTracking.doLabelTracking(FileName.ToUpper(), 6))
                //{
                //    MessageBox.Show(labelTracking.LastError, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    btnPrint.Enabled = true;
                //    lbl.Quit();
                //    return;
                //}
                #endregion
                try
                {
                    lbl.Documents.Open(FullFilePath, false);
                    Document doc = lbl.ActiveDocument;
                    for (int i = 0; i < val_notPrintQty; i = i + printStep)
                    {
                        string Panel2 = "";
                        string Panel1 = dataGridView1.Rows[i].Cells[1].Value.ToString();
                        if (labMode == "2") Panel2 = dataGridView1.Rows[i + 1].Cells[1].Value.ToString();
                        try
                        {
                            for (int j = 1; j < doc.Variables.FormVariables.Count + 1; j++)
                            {
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "SN")
                                    doc.Variables.FormVariables.Item("SN").Value = Panel1.ToString();
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "SN1")
                                    doc.Variables.FormVariables.Item("SN1").Value = Panel1.ToString();
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "SN2")
                                    doc.Variables.FormVariables.Item("SN2").Value = Panel2.ToString();
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "MO")
                                    doc.Variables.FormVariables.Item("MO").Value = txtMoNumber.Text;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        #region do label tracking pqe vs value chuyen vao
                        //if (!labelTracking.doLabelTracking(FileName.ToUpper(), 6))
                        //{
                        //    MessageBox.Show(labelTracking.LastError, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    btnPrint.Enabled = true;
                        //    lbl.Quit();
                        //    return;
                        //}
                        #endregion
                        #region Print label
                        doc.PrintDocument(1);
                        dataGridView1.FirstDisplayedScrollingRowIndex = i;
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Green;
                        dataGridView1.Refresh();
                        #endregion


                        #region update data_input_t
                        var res = "";
                        if (i == 0)
                        {
                            if (printStep != 1 && printStep != 2)
                            {
                                MessageBox.Show("Print Step/LabMode error", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                btnPrint.Enabled = true;
                                return;
                            }
                            if (printStep == 2)
                                res = await PrintBLL.UpdateInputDataAsync(txtMoNumber.Text.Trim(), Panel1 + "|" + Panel2, 2);
                            else res = await PrintBLL.UpdateInputDataAsync(txtMoNumber.Text.Trim(), Panel1, 1);
                            if (res != "OK")
                            {
                                MessageBox.Show(res, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                btnPrint.Enabled = true;
                                return;
                            }
                        }
                        if (i == printStep)
                        {
                            res = await PrintBLL.UpdateInputDataAsync(txtMoNumber.Text.Trim(), Panel1, val_notPrintQty - printStep);
                            if (res != "OK")
                            {
                                MessageBox.Show(res, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                                btnPrint.Enabled = true;
                                return;
                            }
                        }
                        #endregion
                        //progressBar1.Value = progressBar1.Value + printStep;

                        //using (Graphics gr = progressBar1.CreateGraphics())
                        //{
                        //    gr.DrawString(progressBar1.Value.ToString() + "/" + val_notPrintQty,
                        //        SystemFonts.DefaultFont,
                        //        Brushes.Black,
                        //        new PointF(progressBar1.Width / 2 - (gr.MeasureString(progressBar1.Value.ToString() + "/" + val_notPrintQty,
                        //            SystemFonts.DefaultFont).Width / 2.0F),
                        //        progressBar1.Height / 2 - (gr.MeasureString(progressBar1.Value.ToString() + "/" + val_notPrintQty,
                        //            SystemFonts.DefaultFont).Height / 2.0F)));
                        //}
                        //xoa gia tri cua bien
                        try
                        {
                            for (int j = 1; j < doc.Variables.FormVariables.Count + 1; j++)
                            {
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "SN")
                                    doc.Variables.FormVariables.Item("SN").Value = "";
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "SN1")
                                    doc.Variables.FormVariables.Item("SN1").Value = "";
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "SN2")
                                    doc.Variables.FormVariables.Item("SN2").Value = "";
                                if (doc.Variables.FormVariables.Item(j).Name.ToUpper() == "MO")
                                    doc.Variables.FormVariables.Item("MO").Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            lbl.Quit();
                            btnPrint.Enabled = true;
                            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lbl.Quit();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    btnPrint.Enabled = true;
                    return;
                }
                finally
                {
                    lbl.Quit();
                    txtQtyPrint.Text = "";
                    txtFrom.Text = "";
                    MO_EVENT();
                    //progressBar1.Visible = false;
                    btnPrint.Enabled = true;
                    val_notPrintQty = 0;

                    System.DateTime dTo = System.DateTime.Now;
                    double rTime = Math.Round(TimeSpan.Parse((dTo - dFrom).ToString()).TotalSeconds, 2);
                    MessageBox.Show("In thành công\r\nExecute time:" + rTime + "s", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //dataGridView1.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                btnPrint.Enabled = true;
                return;
            }
            #endregion
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }
        private static string GetIP()
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }
        public static string GetMacAddress()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    // macAddresses += nic.GetPhysicalAddress().ToString();
                    macAddresses += String.Join(":", nic.GetPhysicalAddress()
                                    .GetAddressBytes()
                                    .Select(b => b.ToString("X2"))
                                    .ToArray());
                    break;
                }
            }
            return macAddresses;
        }

        private void chkPanel_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPanel.Checked == false) chkPanel.ForeColor = Color.Red;
            else chkPanel.ForeColor = Color.Black;
        }

        private void chkSN_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSN.Checked == false) chkSN.ForeColor = Color.Red;
            else chkSN.ForeColor = Color.Black;
        }

        private void txtMoNumber_TextChanged(object sender, EventArgs e)
        {
            Recheck = true;
        }

        private void checkLabelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMoNumber.Text))
            {
                Check_Label frm = new Check_Label(txtMoNumber.Text, comboBU.Text, _sfcHttpClient);
                frm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Vui long nhập công lệnh!");
                return;
            }
        }

        private async void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            txtMoNumber.Enabled = false;
            txtQtyPrint.Enabled = false;
            comboBU.Items.Clear();
            if (BuData != null)
            {
                foreach (var item in BuData)
                {
                    comboBU.Items.Add(item.Bu);
                }
            }
            comboBU.Enabled = true;
            button1.Enabled = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                int c = int.Parse(txtLabMode.Text);
                if (c < 1 || c > 2)
                {
                    MessageBox.Show("Số lượng SN trong 1 label chỉ có thể là 1 hoặc 2");
                    return;
                }
                txtMoNumber.Enabled = true;
                txtQtyPrint.Enabled = true;
                TIniFile.WriteINI(this.g_INIPath, "PRINT_ALLPART", "BU", comboBU.Text);
                PrintAllpartsDTO.Instance.iBU = comboBU.Text;
                lblBU.Text = PrintAllpartsDTO.Instance.iBU + " MODEL";
                comboBU.Enabled = false;
                button1.Enabled = false;
            }
            catch
            {
                MessageBox.Show("Số lượng SN trong 1 label chỉ có thể là 1 hoặc 2");
                return;
            }
        }

        private void comboBU_TextChanged(object sender, EventArgs e)
        {
            loadBu();
        }
        public void localProcessKill(string processName)
        {
            foreach (Process p in Process.GetProcessesByName(processName))
            {
                p.Kill();
            }
        }
    }
}
