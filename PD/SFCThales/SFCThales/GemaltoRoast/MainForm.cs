using System;
using System.Collections.Generic;
using System.ComponentModel;
using LabelManager2;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Windows.Forms;
using Sfc.Library.HttpClient;
using System.Threading.Tasks;
using SFCThales;


namespace GemaltoRoast
{
    public partial class MainForm : Form
    {
        
        string line_Name = Comm.GetConfigValue("line_Name");
        string section_Name = Comm.GetConfigValue("section_Name");
        string group_Name = Comm.GetConfigValue("group_Name");
        string station_Name = Comm.GetConfigValue("station_Name");
        public static string trayprefix;
        public static string traylength;
        public static string empno ;
        public static string empname;
        public static string emprank;
        public static Boolean English = false;
        SfcHttpClient sfcClient;
        string checkSum, loginApiUrl, loginDB, empNo, empPass, inputLogin;
        //public  string trayprefix;
        //public  int traylength;
        public string g_INIPath = System.IO.Directory.GetCurrentDirectory() + "\\CheckTray.ini";
        OperateINI openIni = new OperateINI();
        UserService fdal;
        StationService sdal;
        public MainForm()
        {
            fdal = new UserService();
            sdal = new StationService();
            sfcClient = Form1.sfcHttpClient;
            InitializeComponent();
            lblStation.Text = Comm.GetConfigValue("line_Name") + " " + Comm.GetConfigValue("group_Name");
            if (group_Name == "VI" || group_Name == "DUST_BLOWING" || group_Name == "VI1")
            {
                
            }
        }
        public async Task connect()
        {
            sfcClient = SFCThales.Form1.sfcHttpClient;
            if (sfcClient.AutoRefreshToken == null)
            {
                showMessage("Connect to SFC fail, Please call IT","Không thể kết nối tới SFC, Tìm IT");
                Environment.Exit(0);
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
        public void showMessage(string eMsg,string vMsg)
        {
            if (English) MessageBox.Show(eMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(vMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void showMessage(string Msg)
        {
            MessageBox.Show(Msg);
        }
        private async void MainForm_Load(object sender, EventArgs e)
        {
            await connect();
            //UserService.loginNO = empno;
            //UserService.loginName = empname;
            //UserService.userRank = emprank;
            textBox1.Text = "";
            readini();
            if ((group_Name == "PACK_TRAY")
                || (group_Name == "ROAST_IN")
                || (group_Name == "PACK_TRAYII")
                || (group_Name == "ROAST_OUT")
                || (group_Name == "VI")
                || (group_Name == "VI1")
                || (group_Name == "TAPPING")
                || (group_Name == "DUST_BLOWING"))
            {

                lblts.Text = "Please Input Tray_No";
                if(!English) lblts.Text = "Nhập mã Tray_No";
                lblBakeNo.Visible = false;
                txbBakeNo.Visible = false;

                groupScan.Visible = true;
                groupSNLIST.Visible = false;
                groupNG.Visible = false;
                groupSM.Visible = false;
                sERIALNUMBERToolStripMenuItem.Enabled = false;
                sHIPPINGSNToolStripMenuItem.Enabled = false;

                if (group_Name == "PACK_TRAY" || group_Name == "PACK_TRAYII")
                {
                    groupSNLIST.Visible = true;
                    ckbNG.Visible = false;
                }
                else if ((group_Name == "ROAST_IN") || (group_Name == "ROAST_OUT"))
                {
                    groupSNLIST.Visible = false;
                    groupNG.Visible = false;
                    ckbNG.Visible = false;
                    groupSM.Visible = true;
                }
                else if (group_Name == "VI"||group_Name == "DUST_BLOWING" || group_Name == "VI1")
                {
                    ckbNG.Visible = true;
                    groupSM.Visible = true;
                }
                else if (group_Name == "TAPPING")
                {
                    groupSNLIST.Visible = true;
                    ckbNG.Visible = false;
                    label2.Visible = false;
                    textBox1.Visible = false;
                    btnClose.Visible = false;
                }
                if (group_Name == "ROAST_IN")
                {
                    lblts.Text = "Please Input BAKE_NO!";
                    if (!English) lblts.Text = "Nhập mã BAKE_NO";
                    lblBakeNo.Visible = true;
                    txbBakeNo.Visible = true;
                }
            }
            else
            {
                groupScan.Visible = false;
                groupSNLIST.Visible = false;
                groupNG.Visible = false;
                groupSM.Visible = true;
                sERIALNUMBERToolStripMenuItem.Enabled = false;
                sHIPPINGSNToolStripMenuItem.Enabled = false;
            }
            string emp = SFCThales.Form1.empNo;
            string result =await fdal.CheckPrivilege(emp, sfcClient);
            if (result == null)
            {
                showMessage("This EmpNo no have privilege pass VI", "Mã thẻ này không có quyền qua trạm VI");
                textScan.Enabled = false;
                ckbNG.Enabled = false;
            }
        }

        public void writeini(string prefix, string length)
        {
            openIni.WriteINI(g_INIPath, "TRAY", "PREFIX", prefix);
            openIni.WriteINI(g_INIPath, "TRAY", "LENGTH", length);
        }

        public  void readini()
        {
            trayprefix = openIni.ReadINI(g_INIPath, "TRAY", "PREFIX").ToString().Trim();
            traylength = openIni.ReadINI(g_INIPath, "TRAY", "LENGTH").ToString().Trim();
        }
        private void sERIALNUMBERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textTrayNo.Text != "")
            {
                sERIALNUMBERToolStripMenuItem.Checked = true;
                sHIPPINGSNToolStripMenuItem.Checked = false;
                lblts.Text = "Please Input " + sERIALNUMBERToolStripMenuItem.Text.ToString() + " !";
                if(!English) lblts.Text = "Vui lòng nhập " + sERIALNUMBERToolStripMenuItem.Text.ToString() + " !";
            }
        }

        private void sHIPPINGSNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (textTrayNo.Text != "")
            {
                sHIPPINGSNToolStripMenuItem.Checked = true;
                sERIALNUMBERToolStripMenuItem.Checked = false;
                lblts.Text = "Please Input " + sHIPPINGSNToolStripMenuItem.Text.ToString() + " !";
                if (!English) lblts.Text = "Vui lòng nhập " + sHIPPINGSNToolStripMenuItem.Text.ToString() + " !";
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //System.Windows.Forms.Application.Exit();
        }

        private void tsmiStation_Click(object sender, EventArgs e)
        {
            StationSetup Form = new StationSetup(sfcClient);
            Form.Show();
            this.Hide();
        }

        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            if(English)
            MessageBox.Show("Ext :31526\n This program currently only applies to PACK_TRAY,PACK_TRAYII,\n ROAST_IN, ROAST_OUT, VI1 and VI and DUST_BLOWING stations",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show("Số máy lẻ :31526\n Chương trình chỉ áp dụng cho các trạm PACK_TRAY,PACK_TRAYII,\n ROAST_IN, ROAST_OUT, VI1 và VI and DUST_BLOWING",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void textScan_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (group_Name == "PACK_TRAY" || group_Name == "PACK_TRAYII")
            {
                if (e.KeyChar == '\r' && textScan.Text.Trim() != "")
                {
                    if (string.IsNullOrEmpty(traylength) || string.IsNullOrEmpty(trayprefix))
                    {
                        showMessage("Please set up Tray Prefix and tray length", "Hãy thiết lập tray prefix và độ dài của Tray");
                        lblts.Text = " " + textScan.Text.Trim() + "  Please set up Tray Prefix and tray length!!! !";
                        textScan.SelectAll();
                        return;
                    }
                    string sn = textScan.Text.Trim();

                    if (sHIPPINGSNToolStripMenuItem.Checked == true)
                    {
                        DataTable dtsn = await sdal.getR107(textScan.Text.Trim(), sfcClient);
                        if (dtsn.Rows.Count == 1 )
                        {
                            sn = dtsn.Rows[0]["serial_number"].ToString();
                        }
                    }

                    

                    //第一槍刷入TRAY_NO
                    if (textTrayNo.Text.Trim() == "")
                    {
                        if (textScan.Text.Trim().Length == int.Parse(traylength) && textScan.Text.Trim().Substring(0, trayprefix.Length) == trayprefix)
                        {
                            string Tray_No = textScan.Text.Trim();
                            DataTable dt = await sdal.SelectTraySn(textScan.Text.Trim(), sfcClient);
                            DataTable dt2 =await sdal.GetTrayNo(Tray_No, group_Name, sfcClient);
                            if (dt.Rows.Count == 0)
                            {
                                textTrayNo.Text = textScan.Text.Trim();
                                lblts.Text = "Please Input Serial_Number";
                                if(!English) lblts.Text = "Nhập mã Serial_Number";
                                sERIALNUMBERToolStripMenuItem.Enabled = true;
                                sHIPPINGSNToolStripMenuItem.Enabled = true;
                                sERIALNUMBERToolStripMenuItem.Checked = true;
                                textCount.Text = dt2.Rows.Count.ToString();
                                dgv.DataSource = dt2;
                                textScan.SelectAll();
                            }
                            else
                            {
                                if (dt2.Rows.Count == 0)
                                {
                                    showMessage("The TRAY_NO has been used ", "Mã TRAY đã được sử dụng");
                                    lblts.Text = " " + textScan.Text.Trim() + "   Scan  NG !";
                                    textScan.SelectAll();
                                }
                                else if (dt2.Rows[0]["GROUP_NEXT"].ToString() == "PACK_TRAY" || dt2.Rows[0]["GROUP_NEXT"].ToString() == "PACK_TRAYII")
                                {
                                    textTrayNo.Text = textScan.Text.Trim();
                                    lblts.Text = "Please Input Serial_Number";
                                    if(!English) lblts.Text = "Nhập mã Serial_Number";
                                    sERIALNUMBERToolStripMenuItem.Enabled = true;
                                    sHIPPINGSNToolStripMenuItem.Enabled = true;
                                    sERIALNUMBERToolStripMenuItem.Checked = true;
                                    textCount.Text = dt2.Rows.Count.ToString();
                                    dgv.DataSource = dt2;

                                    textModel.Text = dt2.Rows[0]["model_name"].ToString();
                                    textVersion.Text = dt2.Rows[0]["version_code"].ToString();
                                    if (textBox1.Text == "")
                                    {
                                        DataTable dt3 = await sdal.SelectQTYTray(textModel.Text, textVersion.Text, sfcClient);
                                        if (dt3.Rows.Count != 0)
                                        {
                                            textBox1.Text = dt3.Rows[0]["tray_qty"].ToString();
                                        }
                                        else
                                        {
                                            showMessage("IE not set up Config15", "IE chưa thiết lập config15");
                                            lblts.Text = " " + textScan.Text.Trim() + "   Scan  NG !";
                                            textScan.SelectAll();
                                        }
                                    }

                                    textScan.SelectAll();
                                }
                            }

                        }
                        else
                        {
                            showMessage("TRAY_NO is illegal", "Mã TRAY không hợp lệ");
                            lblts.Text = " " + textScan.Text.Trim() + "   Scan  NG !";
                            textScan.SelectAll();
                        }
                    }
                    //刷入SN
                    else
                    {
                        //判斷電性是否過期
                        if (await sdal.checkElectricalExpired(sn, sfcClient))
                        {
                            showMessage("The electrical properties of the product have expired, please re-test", "Đặc tính điện của sản phẩm đã hết hạn, vui lòng kiểm tra lại");
                            textScan.SelectAll();
                        }
                        else
                        {
                            string result = await sdal.CheckRouteProcedure(line_Name, group_Name, textScan.Text.Trim(), sfcClient);

                            if (!result.Contains("OK"))
                            {
                                lblts.Text = " " + textScan.Text.Trim() + "   Scan NG ! ";
                                textScan.SelectAll();
                                MessageBox.Show(result, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            else
                            {
                                //判斷機種版本是否一致
                                DataTable dt =await sdal.SelectSN(sn, sfcClient);

                                if (dt.Rows.Count > 0)
                                {
                                    if (textModel.Text == "" && textVersion.Text == "")
                                    {
                                        textModel.Text = dt.Rows[0]["model_name"].ToString();
                                        textVersion.Text = dt.Rows[0]["version_code"].ToString();

                                        string Tray_No = textTrayNo.Text.Trim();
                                        int result1 = Convert.ToInt32(await sdal.UpdateSN(sn, Tray_No, sfcClient));
                                        DataTable dt2 = await sdal.GetTrayNo(Tray_No, group_Name, sfcClient);
                                        textCount.Text = dt2.Rows.Count.ToString();
                                        dgv.DataSource = dt2;
                                        lblts.Text = "" + textScan.Text.Trim() + "   Scan  OK !";
                                        if (textBox1.Text == "")
                                        {
                                            DataTable dt3 = await sdal.SelectQTYTray(textModel.Text, textVersion.Text, sfcClient);
                                            if (dt3.Rows.Count != 0)
                                            {
                                                textBox1.Text = dt3.Rows[0]["tray_qty"].ToString();
                                            }
                                            else
                                            {
                                                showMessage("IE not set up Config15", "IE chưa thiết lập config15");
                                                lblts.Text = " " + textScan.Text.Trim() + "   Scan  NG !";
                                                textScan.SelectAll();
                                            }
                                        }
                                        textScan.SelectAll();
                                    }
                                    else
                                    {
                                        if ((textModel.Text != dt.Rows[0]["model_name"].ToString()) || (textVersion.Text != dt.Rows[0]["version_code"].ToString()))
                                        {
                                            showMessage("Different  Model/Version", "Khác tên hàng hoặc phiên bản");
                                            lblts.Text = "" + textScan.Text.Trim() + "   Scan  NG !";
                                            textScan.SelectAll();
                                        }
                                        else
                                        {
                                            string Tray_No = textTrayNo.Text.Trim();
                                            int result1 = Convert.ToInt32(await sdal.UpdateSN(sn, Tray_No, sfcClient));
                                            DataTable dt2 = await sdal.GetTrayNo(Tray_No, group_Name, sfcClient);
                                            textCount.Text = dt2.Rows.Count.ToString();
                                            dgv.DataSource = dt2;
                                            lblts.Text = "" + textScan.Text.Trim() + "   Scan  OK !";
                                            if (textBox1.Text == "")
                                            {
                                                DataTable dt3 = await sdal.SelectQTYTray(textModel.Text, textVersion.Text, sfcClient);
                                                textBox1.Text = dt3.Rows[0]["tray_qty"].ToString();
                                            }
                                            if (int.Parse(textCount.Text) == int.Parse(textBox1.Text))
                                            {
                                                btnClose_Click(sender, e);
                                            }
                                            else
                                            {
                                                textScan.SelectAll();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    showMessage("SN not found","Không tìm thấy SN");
                                    textScan.SelectAll();
                                }
                            }
                        }
                    }
                }
            }
            else if (group_Name == "TAPPING")
            {
                if (e.KeyChar == '\r' && textScan.Text.Trim() != "")
                {
                    string sn = textScan.Text.Trim();

                    //第一槍刷入TRAY_NO
                    if (textTrayNo.Text.Trim() == "")
                    {
                        if (textScan.Text != "")
                        {
                            string Tray_No = textScan.Text.Trim();
                            DataTable dt = await sdal.SelectTraySn(textScan.Text.Trim(), sfcClient);
                            
                            if (dt.Rows.Count == 0)
                            {
                                MessageBox.Show("The TRAY_NO No data, Please check.. !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                showMessage("The TRAY_NO no data, Please check", "Mã TRAY không có dữ liệu,Vui lòng kiểm tra lại");
                                lblts.Text = " " + textScan.Text.Trim() + "   no data !!!!";
                                textScan.SelectAll();
                            }
                            else
                            {
                                if (dt.Rows[0]["WIP_GROUP"].ToString() == "TAPPING")
                                {
                                    textTrayNo.Text = textScan.Text.Trim();
                                    lblts.Text = "Please Input Serial_Number";
                                    if(!English) lblts.Text = "Nhập mã Serial_Number";
                                    sERIALNUMBERToolStripMenuItem.Enabled = true;
                                    sHIPPINGSNToolStripMenuItem.Enabled = true;
                                    sERIALNUMBERToolStripMenuItem.Checked = true;
                                    textModel.Text = dt.Rows[0]["model_name"].ToString();
                                    textVersion.Text = dt.Rows[0]["version_code"].ToString();
                                    DataTable dt2 = await sdal.GetTrayNo1(Tray_No, sfcClient);
                                    dgv.DataSource = dt2;
                                    textCount.Text = dt2.Rows.Count.ToString();

                                    textScan.SelectAll();
                                }
                                else
                                {
                                    showMessage("The TRAY_NO have WIP_GROUP <> TAPPING station, Please check", "Mã TRAY có bản lỗi lưu trình,WIP <> TAPPING");
                                    lblts.Text = " " + textScan.Text.Trim() + "   No data !!!!";
                                    textScan.SelectAll();

                                }
                            }

                        }
                        else
                        {
                            showMessage("TRAY_NO is illegal", "Mã TRAY không hợp lệ");
                            lblts.Text = " " + textScan.Text.Trim() + "   Scan  NG !";
                            textScan.SelectAll();
                        }
                    }
                    //刷入SN
                    else
                    {
                        //判斷電性是否過期

                            string result = await sdal.CheckRouteProcedure(line_Name, group_Name, textScan.Text.Trim(), sfcClient);
                            
                            if (!result.Contains("OK"))
                            {
                                lblts.Text = " " + textScan.Text.Trim() + "   Scan NG ! ";
                                textScan.SelectAll();
                                MessageBox.Show(result, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            else
                            {
                                //判斷機種版本是否一致
                                DataTable dt = await sdal.SelectSN(sn, sfcClient);

                                if (dt.Rows.Count > 0)
                                {
                                    if (textModel.Text == "" && textVersion.Text == "")
                                    {
                                        textModel.Text = dt.Rows[0]["model_name"].ToString();
                                        textVersion.Text = dt.Rows[0]["version_code"].ToString();

                                        string Tray_No = textTrayNo.Text.Trim();
                                        int result1 = Convert.ToInt32(await sdal.UpdateSN1(sn, sfcClient));
                                        DataTable dt2 = await sdal.GetTrayNo1(Tray_No, sfcClient);
                                        textCount.Text = dt2.Rows.Count.ToString();
                                        dgv.DataSource = dt2;
                                        lblts.Text = "" + textScan.Text.Trim() + "   Scan  OK !";
                                        if (int.Parse(textCount.Text) == 0)
                                        {
                                            btnClose_Click(sender, e);
                                        }
                                        else
                                        {
                                            textScan.SelectAll();
                                        }
                                }
                                    else
                                    {
                                        if ((textModel.Text != dt.Rows[0]["model_name"].ToString()) || (textVersion.Text != dt.Rows[0]["version_code"].ToString()))
                                        {
                                            showMessage("Different  Model/Version", "Khác tên hàng hoặc phiên bản");
                                            lblts.Text = "" + textScan.Text.Trim() + "   Scan  NG !";
                                            textScan.SelectAll();
                                        }
                                        else
                                        {
                                            string Tray_No = textTrayNo.Text.Trim();
                                            int result1 = Convert.ToInt32(sdal.UpdateSN1(sn, sfcClient));
                                            DataTable dt2 = await sdal.GetTrayNo1(Tray_No, sfcClient);
                                            textCount.Text = dt2.Rows.Count.ToString();
                                            dgv.DataSource = dt2;
                                            lblts.Text = "" + textScan.Text.Trim() + "   Scan  OK !";
                                            if (int.Parse(textCount.Text) == 0)
                                            {
                                                btnClose_Click(sender, e);
                                            }
                                            else
                                            {
                                                textScan.SelectAll();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    showMessage("SN not found", "Không tìm thấy SN");
                                    textScan.SelectAll();
                                }
                            }
                        
                    }
                }
            }
            else if (group_Name == "ROAST_IN")
            {
                if (e.KeyChar == '\r' && textScan.Text.Trim() != "")
                {
                    if (string.IsNullOrEmpty(txbBakeNo.Text))
                    {
                        string result = await sdal.CheckBakeProcedure(textScan.Text, sfcClient);
                        if (result.Contains("OK"))
                        {
                            txbBakeNo.Text = textScan.Text;
                            lblts.Text = "BakeID: " + textScan.Text.Trim() + " Scan OK! Please Scan Tray_No！";
                            textScan.SelectAll();
                        }
                        else
                        {
                            lblts.Text = result + "Please Scan Other BakeID ！";
                            textScan.SelectAll();
                        }
                    }
                    else
                    {
                        if (textScan.Text.Trim().Length == int.Parse(traylength) && textScan.Text.Trim().Substring(0, trayprefix.Length) == trayprefix)
                        {
                            DataTable dt = await sdal.SelectTraySn(textScan.Text.Trim(), sfcClient);
                            if (dt.Rows.Count > 0)
                            {
                                string traySn = dt.Rows[0]["SERIAL_NUMBER"].ToString();
                                string result = await sdal.CheckRouteProcedure(line_Name, group_Name, traySn, sfcClient);
                                if (!result.Contains("OK"))
                                {
                                    lblts.Text = " " + textScan.Text.Trim() + " : " + result + " ";
                                    textScan.SelectAll();
                                    MessageBox.Show(result, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                                else
                                {
                                    string result1 = await sdal.UpTrayStationProcedure(sfcClient,line_Name, section_Name, group_Name, station_Name, textScan.Text.Trim(), txbBakeNo.Text.Trim());
                                    if (!result1.Contains("OK"))
                                    {
                                        lblts.Text = " " + textScan.Text.Trim() + " : " + result1 + " ";
                                        textScan.SelectAll();
                                        MessageBox.Show(result1, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        return;
                                    }
                                    else
                                    {
                                        txbBakeNo.Text = "";
                                        lblts.Text = "" + textScan.Text.Trim() + " PASS OK ! Please Input Next BakeID!";
                                        textScan.SelectAll();
                                    }
                                }
                            }
                            else
                            {
                                lblts.Text = "TrayID :  " + textScan.Text.Trim() + " is no data !";
                                MessageBox.Show("TRAY_NO is no data !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                textScan.SelectAll();
                            }

                        }
                        else
                        {
                            lblts.Text = "TrayID :  " + textScan.Text.Trim() + " is illegal !";
                            showMessage("TRAY_NO is illegal", "Mã TRAY không hợp lệ");
                            textScan.SelectAll();
                        }
                    }
                }
            }
            else if ((group_Name == "ROAST_OUT") || (group_Name == "VI") || (group_Name == "DUST_BLOWING") || (group_Name == "VI1"))
            {
                if (e.KeyChar == '\r' && textScan.Text.Trim() != "")
                {
                    if (textScan.Text.Trim().Length == int.Parse(traylength) && textScan.Text.Trim().Substring(0, trayprefix.Length) == trayprefix)
                    {
                        DataTable dt = await sdal.SelectTraySn(textScan.Text.Trim(), sfcClient);
                        if (dt.Rows.Count > 0)
                        {
                            string traySn = dt.Rows[0]["SERIAL_NUMBER"].ToString();
                            string result = await sdal.CheckRouteProcedure(line_Name, group_Name, traySn, sfcClient);
                            if (!result.Contains("OK"))
                            {
                                lblts.Text = " " + textScan.Text.Trim() + " : " + result + " ";
                                textScan.SelectAll();
                                MessageBox.Show(result, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            else
                            {
                                string result1 = await sdal.UpTrayStationProcedure(sfcClient,line_Name, section_Name, group_Name, station_Name, textScan.Text.Trim());
                                if (!result1.Contains("OK"))
                                {
                                    lblts.Text = " " + textScan.Text.Trim() + " : " + result1 + " ";
                                    textScan.SelectAll();
                                    MessageBox.Show(result1, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                                else
                                {
                                    lblts.Text = "" + textScan.Text.Trim() + " PASS OK ! Please Input Next TrayNo!";
                                    textScan.SelectAll();
                                }
                            }
                        }
                    }
                    else
                    {
                        lblts.Text = "TrayID :  " + textScan.Text.Trim() + " 不合法 !";
                        showMessage("TRAY_NO is illegal", "Mã TRAY không hợp lệ");
                        textScan.SelectAll();
                    }
                }
            }
        }

        private async void btnClose_Click(object sender, EventArgs e)
        {
            string result = await sdal.UpTrayStationProcedure(sfcClient,line_Name, section_Name, group_Name, station_Name, textTrayNo.Text.Trim());
            if (!result.Contains("OK"))
            {
                lblts.Text = result;
                textScan.SelectAll();
                MessageBox.Show(result, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                lblts.Text = "" + textTrayNo.Text.Trim() + " PASS OK ! Please Input Next TrayNo!";
                textScan.Text = "";
                textScan.SelectAll();
                textTrayNo.Text = "";
                textCount.Text = "";
                textModel.Text = "";
                textVersion.Text = "";
                textBox1.Text = "";
                string Tray_No = textTrayNo.Text.Trim();
                DataTable dt2 = await sdal.GetTrayNo(Tray_No, group_Name, sfcClient);
                dgv.DataSource = dt2;
            }
        }

        private void ckbNG_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbNG.Checked == true)
            {
                textScan.Enabled = false;
                groupNG.Visible = true;
                groupSM.Visible = false;
                textSN.Text = "";
                textEC.Text = "";
                lblts.Text = "";
            }
            else
            {
                groupNG.Visible = false;
                groupSM.Visible = true;
                textScan.Enabled = true;
                textSN.Text = "";
                textEC.Text = "";
                lblts.Text = "Please Input Tray_No!";
            }
        }

        private async void btnOK_Click(object sender, EventArgs e)
        {
            string result = await sdal.CheckRouteProcedure(line_Name, group_Name, textSN.Text.Trim(), sfcClient);

            if (!result.Contains("OK"))
            {
                textSN.SelectAll();
                MessageBox.Show(result, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                string result1 = await sdal.MakeNGProcedure(line_Name, section_Name, group_Name, station_Name, textSN.Text.Trim(), textEC.Text.Trim(), sfcClient);
                if (result1 != null)
                {
                    textSN.SelectAll();
                    MessageBox.Show(result1, "Point", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    MessageBox.Show("" + textSN.Text.Trim() + " Make NG OK ! Please Input Next SN !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textSN.SelectAll();
                    textSN.Text = "";
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            textSN.Text = "";
            textEC.Text = "";
            groupNG.Visible = false;
            groupSM.Visible = true;
            ckbNG.Checked = false;
            textScan.Enabled = true;
            lblts.Text = "Please Input Tray_No!";
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {

        }

        private void tsmiExit_Click_1(object sender, EventArgs e)
        {
            TraySetup Form1 = new TraySetup(sfcClient);
            Form1.ShowDialog();
            //string prefix = Form1.t
            //this.Hide();
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            englishToolStripMenuItem.Checked = true;
            tiếngViệtToolStripMenuItem.Checked = false;
            English = true;
        }

        private void tiếngViệtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            englishToolStripMenuItem.Checked = false;
            tiếngViệtToolStripMenuItem.Checked = true;
            English = false;
        }
    }
}
