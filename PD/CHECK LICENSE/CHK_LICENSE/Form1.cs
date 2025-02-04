using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CHK_LICENSE;
using Microsoft.VisualBasic;
using System.Diagnostics;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using System.Linq;
using System.Threading.Tasks;
using Oracle;
using Sfc.Core.Models;
using System.Reflection;
using System.Net;
using Newtonsoft.Json;

namespace CHK_LICENSE
{
    
    public partial class Form1 : Form
    {
        DataTable dt = new DataTable();
        public String line;
        public String routecode;
        public String empno;
        public string inputLogin, checkSum, baseUrl, empNo, empPass, error_code, CK_DB;
        public string val_carton, val_security1, val_security2, val_license,val_CartonQty,r_License;
        public static string val_empNo=string.Empty, val_empName, loginDB,val_emppwd;
        public static Boolean F_visible = false,F_reprint=false,F_LSSC=false,F_License=true;
        public static SfcHttpClient _sfcHttpClient;
        public DB _oracle;
        public static string lang = "", ipAddress="", macAddress="";//VNI/ENG
        public static DataTable dtParams = new DataTable();
        private Ini ini;
        public string mygroup = "CHK_LICENSE";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ipAddress = Ini.GetLocalIPAddress();
            macAddress = Ini.GetMacAddress();
            
            connect();
            lblGroup.Text = mygroup;
            this.Text = "CHECK LICENSE Version: " + System.Windows.Forms.Application.ProductVersion;
            try
            {
                ini = new Ini(System.Windows.Forms.Application.StartupPath + "\\SFIS_AMS.INI");
                lang = ini.IniReadValue("MainSection", "LANG");
                if (lang == "ENG")
                {
                    englishToolStripMenuItem.Checked = true;
                    tiếngViệtToolStripMenuItem.Checked = false;
                }
                else
                {
                    englishToolStripMenuItem.Checked = false;
                    tiếngViệtToolStripMenuItem.Checked = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task connect()
        {
            if (!await AccessAPI())
            {
                return;
                Application.Exit();
            }
            if(_sfcHttpClient.AutoRefreshToken == null)
            {
                MessageBox.Show("Connect SFC fail! plz call IT");
                System.Windows.Forms.Application.Exit();
            }
            //string sql = "select*from SFIS1.C_AMS_PATTERN_T  where ap_name = 'CHK_LICENSE' and AP_VERSION ='" + System.Windows.Forms.Application.ProductVersion + "'";
            //var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            //if (result.Data == null)
            //{
            //    MessageBox.Show("Please update new version");
            //    System.Windows.Forms.Application.Exit();
            //}
        }

        private async Task<Boolean> AccessAPI()
        {
            try
            {
                string[] Args = Environment.GetCommandLineArgs();
                if (Args.Length == 1)
                {
                    MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButtons.OKCancel,MessageBoxIcon.Stop);
                    Environment.Exit(0);
                }
                foreach (string s in Args)
                {
                    inputLogin = s.ToString();
                }
                string[] argsInfor = System.Text.RegularExpressions.Regex.Split(inputLogin, @";");
                checkSum = argsInfor[0].ToString();
                baseUrl = argsInfor[1].ToString();
                loginDB = argsInfor[2].ToString();
                empNo = argsInfor[3].ToString();
                empPass = argsInfor[4].ToString();

                //itemDbConnected.Content = "DB: " + loginDB.ToUpper();

                _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = "select 1 from dual", SfcCommandType = SfcCommandType.Text });
                /*
                oracle = new Oracle(_sfcHttpClient);
                
                if (!await CheckAppVersion(prgname, Appver))
                {
                    Environment.Exit(0);
                }*/
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not connect to DB:"+ ex.Message);
                return false;
            }
        }
        private async void employee_KeyPress(object sender, KeyPressEventArgs e)
        {
            employeeno.PasswordChar = '*';
            val_empNo = ""; val_empName = "";
            if (e.KeyChar == 13)
            {
                val_empName = "";
                val_emppwd = employeeno.Text.Trim();
                if (val_emppwd == "") return;
                _oracle = new DB(_sfcHttpClient);
                string sql = string.Format(@"SELECT * FROM SFIS1.C_EMP_DESC_T where  EMP_PASS ='{0}'", val_emppwd);

                 var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    showMessage("No employee password", "Không tìm thấy mã nhân viên");
                }
                else
                {
                    if (result.Data["class_name"].ToString().ToUpper().IndexOf("V") != -1) F_visible = true;
                    else F_visible = false;
                    employeeno.PasswordChar = (char)0;
                    val_empName = result.Data["emp_name"]?.ToString() ?? "";
                    val_empNo = result.Data["emp_no"]?.ToString() ?? "";
                    var result2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel {
                        CommandText = string.Format("select * from SFIS1.C_PRIVILEGE where emp='{0}' and privilege='2' and prg_name='CHK_LICENSE' and fun='REPRINT'", val_empNo), SfcCommandType = SfcCommandType.Text });
                    if (result2.Data != null) F_reprint = true;
                    else F_reprint = false;
                    employeeno.Text = val_empName + "(" + val_empNo + ")";
                    cartonno.Enabled = true;
                    cartonno.Focus();
                }
            }
        }
        private async void GetLSSCFlag()
        {
            string zSql ="select * from SFIS1.C_TMM_CONFIG_T where CUSTOMER_NAME='LABEL LSSC' and temp5='1' ";
            zSql += " and model_name in(select model_name from sfism4.r107 where mcarton_no='{0}') ";
            zSql = string.Format(zSql, val_carton);
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = zSql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
                F_LSSC = true;
            else F_LSSC = false;
            zSql = "select count(*) qty from sfism4.r107 where mcarton_no='" + val_carton + "'";
            result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = zSql, SfcCommandType = SfcCommandType.Text });
            val_CartonQty = result.Data["qty"]?.ToString() ?? "";
        }
        private async void carton_KeyPress(object sender, KeyPressEventArgs e)
        {
            string sql = "select*from SFIS1.C_AMS_PATTERN_T  where ap_name = 'CHK_LICENSE' and AP_VERSION ='" + System.Windows.Forms.Application.ProductVersion + "'";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                showMessage("CHK_LICENSE have new version,Please re-login SFIS_AMS to update", "CHK_LICENSE có phiên bản mới,Vui lòng đăng nhập lại SFIS_AMS để cập nhật");
                return;
            }
            //----------------------------------------------------------------------
            F_LSSC = false;F_License = true;
            val_carton = "";
            val_security1 = "";
            val_security2 = "";
            val_license = ""; r_License = "";
            errorMessage.Text = "";
            if (val_empNo == string.Empty || val_empNo.Trim() == "")
            {
                showMessage("Please input password first", "Vui lòng nhập mật khẩu trước");
                return;
            }
            if (e.KeyChar == 13)
            {
                F_LSSC = true;
                val_carton = cartonno.Text.Trim();
                if (val_carton == "") return;
                
                sql = "select distinct mcarton_no,mo_number,version_code,line_name,SPECIAL_ROUTE,wip_group,model_name from sfism4.r107 where mcarton_no='" + val_carton + "' ";
                var R107 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (R107.Data == null)
                {
                    showMessage("Carton " + val_carton + " not exist", "Carton " + cartonno.Text + " không tồn tại");
                    cartonno.Focus();
                    cartonno.SelectAll();
                    return;
                }
                sql = "select carton_no,license_no,SECURITY_1_NO,SECURITY_2_NO from SFISM4.R_SEC_LIC_LINK_T where carton_no='" + val_carton + "' and LICENSE_NO is not null";
                var RSEC = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (RSEC.Data == null)
                {
                    showMessage("Carton " + val_carton + " not link with License,Rework from CHK_SYS", "Carton " + cartonno.Text + " chưa link với License,làm lại từ CHK_SYS");
                    cartonno.Focus();
                    cartonno.SelectAll();
                    return;
                }
                string se1 = RSEC.Data["security_1_no"]?.ToString().Trim() ;
                string se2 = RSEC.Data["security_2_no"]?.ToString().Trim() ;

                //if (RSEC.Data["security_1_no"].ToString().Trim()!= ""|| RSEC.Data["security_2_no"].ToString().Trim() != "" || RSEC.Data["security_1_no"].ToString().Trim() != null || RSEC.Data["security_2_no"].ToString().Trim() != null)
                if(se1!= null || se2!=null )
                {
                    showMessage("Carton " + val_carton + " has linked with Security label", "Carton " + cartonno.Text + " đã link với label Security");
                    cartonno.Focus();
                    cartonno.SelectAll();
                    return;
                }
                r_License = RSEC.Data["license_no"].ToString().Trim();
                if (r_License == val_carton) 
                {
                    F_License = false;
                    licenseno.Text = val_carton;
                    mygroup = "CHK_SCR";
                    licenseno.ReadOnly = true;
                }
                else
                {
                    licenseno.ReadOnly = false;
                    mygroup = "CHK_LICENSE";
                }
                lblGroup.Text = mygroup;
                GetLSSCFlag();
                string wipgroup = R107.Data["wip_group"]?.ToString() ?? "";                
                string modelname = R107.Data["model_name"]?.ToString() ?? "";                
                string monumber = R107.Data["mo_number"]?.ToString() ?? "";                
                string version = R107.Data["version_code"]?.ToString() ?? "";                
                routecode = R107.Data["special_route"]?.ToString() ?? "";                
                line = R107.Data["line_name"]?.ToString() ?? "";                
                if (wipgroup == mygroup)
                {
                    errorMessage.Text = "";                    
                    moNumber.Text = monumber;                    
                    modelName.Text = modelname;                    
                    versionCode.Text = version;                    
                    sec1.Enabled = true;                    
                    sec1.Focus();
                }
                else
                {
                    showMessage("Route error,wip=" + wipgroup, "Lỗi lưu trình,wip=" + wipgroup);                    
                    cartonno.Clear();                    
                    cartonno.Focus();                    
                }
            }
        }
        private async void sec1_KeyPress(object sender, KeyPressEventArgs e)
        {
            errorMessage.Text = "";
            if (val_empNo == string.Empty || val_empNo.Trim() == "")
            {
                showMessage("Please input password first", "Vui lòng nhập mật khẩu trước");
                return;
            }
            if (e.KeyChar == 13)
            {
                val_security1 = sec1.Text.Trim();
                if (val_security1 == "") return;
                if (await checkDup(1))
                {
                    if (val_security1.Length == 16)
                    {
                        if (await CheckSec(moNumber.Text, val_security1, modelName.Text))
                        {
                            errorMessage.Text = "";
                            sec2.Enabled = true;
                            sec2.Focus();
                        }
                        else
                        {
                            sec1.Clear();
                            sec1.Focus();
                        }
                    }
                    else
                    {
                        showMessage("Security label1" + val_security1 + " length error", "Lỗi độ dài label Security1:"+ val_security1);
                        sec1.Clear();
                        sec1.Focus();
                    }
                }
                else
                {
                    showMessage("Security label1  " + val_security1 + " has check license", "Label Security1 " + val_security1 + " đã check license");
                    sec1.Clear();
                    sec1.Focus();
                }

            }
        }
        private async Task<Boolean> CheckSec(string mo, string security, string model)//by wenchun check sec
        {
            DataTable dt1 = new DataTable();
            string str = @"SELECT * FROM SFIS1.C_MODEL_BRCM_VER_T WHERE MO_NUMBER = '" + mo + "' AND MODEL_NAME = '" + model + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = str, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                str = @"SELECT * FROM SFIS1.C_MODEL_BRCM_VER_T WHERE MO_NUMBER = '" + mo + "' AND MODEL_NAME = '" + model + "' and CONFIRM_STATUS = 'Y' ";
                var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = str, SfcCommandType = SfcCommandType.Text });
                if (result1.Data != null)
                {
                    string site = result1.Data["site"]?.ToString() ?? "";
                    if (site == "FG")//bac giang
                    {
                        if (!security.Contains("FG")) //BRCMFG0000000039
                        {
                            showMessage("Security " + security + " does not match with SiteCode GZ(FG).Please find Label Room, PQE, and Line Leader check!", "Security " + security + " không đúng với SiteCode Quang Châu(FG).Tìm Labelroom, PQE, và chuyền trưởng kiểm tra lại!");
                            return false;
                        }
                    }
                    if (site == "FV") //bac ninh
                    {
                        if (!security.Contains("BF")) //BRCMBF0000000992
                        {
                            showMessage("Security " + security + " does not match with SiteCode GW(BF).Please find Label Room, PQE, and Line Leader check!", "Security " + security + " không đúng với SiteCode Quế Võ(BF).Tìm Labelroom, PQE, và chuyền trưởng kiểm tra lại!");
                            return false;
                        }
                    }
                    if (site == "CQ") //trung khanh
                    {
                        if (!security.Contains("QF")) //BRCMQF0000000992
                        {
                            showMessage("Security " + security + " does not match with SiteCode CQ(QF).Please find Label Room, PQE, and Line Leader check!", "Security " + security + " không đúng với SiteCode Trùng Khánh(QF).Tìm Labelroom, PQE, và chuyền trưởng kiểm tra lại!");
                            return false;
                        }
                    }
                }
                else
                {
                    showMessage("Find PQE confirm CustVersion " + mo, "Tìm PQE xác nhận CustVersion " + mo);
                    return false;
                }
            }
            else
            {
                showMessage("Find LabelRoom setup CustVersion for " + mo, "Tìm LabelRoom thiết lập Cust Version cho công lệnh " + mo);
                return false;
            }
            return true;
        }

        private void cartonno_TextChanged(object sender, EventArgs e)
        {

        }

        private async void sec2_KeyPress(object sender, KeyPressEventArgs e)
        {
            errorMessage.Text = "";
            if (val_empNo == string.Empty || val_empNo.Trim() == "")
            {
                showMessage("Please input password first", "Vui lòng nhập mật khẩu trước");
                return;
            }
            if (e.KeyChar == 13)
            {
                val_security2 = sec2.Text.Trim();
                if (val_security2 == "") return;
                if (val_security2 == val_security1)
                {
                    showMessage("Security label2 dup with Security label1 " + val_security1,"Security2 trùng lặp với Security1 " + val_security1);
                    sec2.Clear();
                    sec2.Focus();
                }
                else
                {
                    if (await checkDup(2))
                    {
                        if (val_security2.Length == 16)
                        {
                            if (await CheckSec(moNumber.Text, val_security2, modelName.Text))
                            {
                                errorMessage.Text = "";
                                licenseno.Enabled = true;
                                licenseno.Focus();
                                if (!F_License) licenseno_KeyPress(sender,e);
                            }
                            else
                            {
                                sec2.Clear();
                                sec2.Focus();
                            }

                        }
                        else
                        {
                            showMessage("Security2 " + val_security2 + " length error <> 16", "Lỗi độ dài Security2:" + val_security2+" <> 16");
                            sec2.Clear();
                            sec2.Focus();

                        }
                    }
                    else
                    {
                        showMessage("Security2 " + val_security2 + " has check license", "Security2 " + val_security2 + "đã check license");
                        sec2.Clear();
                        sec2.Focus();
                    }
                }
            }
        }

        private async void licenseno_KeyPress(object sender, KeyPressEventArgs e)
        {
            string sql = "" ;
            errorMessage.Text = "";
            DataTable data = new DataTable();
            if (val_empNo == string.Empty || val_empNo.Trim() == "")
            {
                showMessage("Please input password first", "Vui lòng nhập mật khẩu trước");
                return;
            }
            if (e.KeyChar == 13)
            {
                val_license = licenseno.Text.Trim();
                if (val_license == "") return;
                if(F_LSSC)
                {
                    #region checkQR
                    try
                    {
                        List<string> strQR = val_license.Split(';').ToList();
                        if(strQR.Count ==1)
                        {
                            showMessage("Please scan QR code in LSSC label", "Vui lòng sảo mã QR trên label LSSC");
                            return;
                        }
                        if (strQR.Count != 9)
                        {
                            showMessage("QR format error", "Lỗi định dạng QR");
                            return;
                        }
                        string qrCarton = strQR[0].ToString().Substring(1);//cut Q character
                        string qrLicence = strQR[3].ToString().Substring(2);//cut 1T character
                        string qrWeight = strQR[6].ToString().Substring(1);//cut G character
                        string qrQty = strQR[7].ToString().Substring(1);//cut Q character
                        if (qrCarton != val_carton)
                        {
                            showMessage("CartonNO in QR code not match", "Mã Carton  trong QR không đúng");
                            return;
                        }
                        if (Int32.Parse(qrQty) != Int32.Parse(val_CartonQty))
                        {
                            showMessage("Qty in QR code not match", "Số lượng trong QR không đúng");
                            return;
                        }
                        val_license = qrLicence;
                    }
                    catch(Exception ex)
                    {
                        showMessage("Get QR content error:"+ex.Message,"Lấy nội dung mã QR xảy ra lỗi:"+ex.Message);
                        return;
                    }
                    #endregion
                }
                if(r_License!=val_license)
                {
                    showMessage(val_license + " not match with License link from CHK_SYS", val_license + " không khớp với Licence link từ trạm CHK_SYS");
                    licenseno.Clear();
                    licenseno.Focus();
                    return;
                }
                if (await checkDup(3))
                {
                    if (await checkLicenseRange(val_carton,val_license))
                    {
                        //xuan begin a binh QA yeu cau sao ma license xong sao mot ma sn tren label packing
                        sql = "select * from SFIS1.C_MODEL_ATTR_CONFIG_T where ATTRIBUTE_NAME='LPN SCAN SN' AND TYPE_VALUE = '" + modelName.Text + "' ";
                        var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result.Data != null)
                        {
                            string sn_input = Interaction.InputBox("INPUT SN BARCODE.", "INPUT SN BARCODE.", "", -1, -1);

                            var intfr = result.Data["attribute_desc1"]?.ToString() == null ? "" : result.Data["attribute_desc1"]?.ToString();
                            var intto = result.Data["attribute_desc2"]?.ToString() == null ? "" : result.Data["attribute_desc2"]?.ToString();

                            if (intfr != "" && intto != "")
                            {
                                int fr = int.Parse(result.Data["attribute_desc1"]?.ToString());
                                int to = int.Parse(result.Data["attribute_desc2"]?.ToString());

                                sn_input = sn_input.Substring(fr-1, to);
                            }

                            sql = "SELECT * FROM SFISM4.R107 WHERE MCARTON_NO = '" + val_carton  + "' AND SERIAL_NUMBER IN " +
                                          "(SELECT SERIAL_NUMBER FROM SFISM4.R108 WHERE KEY_PART_SN ='" + sn_input + "' AND KEY_PART_NO <> 'MACID') ";
                            if (!F_License)
                            {
                                sql = "SELECT * FROM SFISM4.R107 WHERE MCARTON_NO = '" + val_carton + "' and shipping_sn='" + sn_input + "' ";
                            }
                            var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result1.Data == null)
                            {
                                showMessage("SN:" + sn_input + " not in carton " + val_carton + ".Please check.", "SN:" + sn_input + " không nằm trong carton " + val_carton + ".Kiểm tra lại");
                                if (!F_License)
                                {
                                    licenseno_KeyPress(sender, e);
                                }
                                else
                                {
                                    licenseno.Clear();
                                    licenseno.Focus();
                                }
                                return;
                            }
                            if (dt.Rows.Count >= 2)
                            {
                                showMessage("SN:" + sn_input + " in 2 carton.Please check.", "SN:" + sn_input + " trong 2 carton.Kiểm tra lại");
                                if (!F_License)
                                {
                                    licenseno_KeyPress(sender, e);
                                }
                                else
                                {
                                    licenseno.Clear();
                                    licenseno.Focus();
                                }
                                return;
                            }
                        }
                        //xuan end
                        errorMessage.Text = "";
                        string sqllicense = "select * from sfism4.R_SEC_LIC_LINK_T where carton_no ='" + val_carton + "' and license_no='"+val_license+"' ";
                        var result2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sqllicense, SfcCommandType = SfcCommandType.Text });
                        if (result2.Data == null)
                        {
                            showMessage("Not found data link carton"+ val_carton+" with license "+val_license, "Không tìm thấy dữ liệu carton " + val_carton + " link với license " + val_license);
                            licenseno.Clear();
                            licenseno.Focus();
                            return;
                        }
                        string update = "update SFISM4.R_SEC_LIC_LINK_T set MODEL_NAME=nvl(MODEL_NAME,'" + modelName.Text + "') ,SECURITY_1_NO=nvl(SECURITY_1_NO,'" + val_security1 + "'),SECURITY_2_NO= nvl(SECURITY_2_NO,'" + val_security2 + "'),EMP_NO= '" + val_empNo + "',LINE = '" + line + "', LINK_TIME = sysdate where carton_no= '" + val_carton + "'";
                        var d = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = update, SfcCommandType = SfcCommandType.Text });
                        string nextStation =await getNextStation(routecode);

                        string str2 = "update sfism4.r107 set NEXT_STATION='N/A',location='PRG',group_name='"+mygroup+ "',station_name='" + mygroup + "',in_station_time=sysdate ,wip_group='" + nextStation + "',PMCC= '" + val_license + "' where mcarton_no='" + val_carton + "'";
                      
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = str2, SfcCommandType = SfcCommandType.Text });

                        string str3 = "Insert into SFISM4.R117 SELECT*FROM SFISM4.R107 WHERE mcarton_no='" + val_carton + "'";
                        var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = str3, SfcCommandType = SfcCommandType.Text });
                        
                        cartonno.Clear();
                        sec1.Clear();
                        sec1.Enabled = false;
                        sec2.Clear();
                        sec2.Enabled = false;
                        licenseno.Clear();
                        licenseno.Enabled = false;
                        cartonno.Focus();
                    }
                    else
                    {
                        showMessage(val_license+":License range not found!", val_license + ":Không tìm thấy dải label license!");
                        licenseno.Clear();
                        licenseno.Focus();
                        return;
                    }

                }
                else
                {
                    showMessage("LICENSE NO:" + val_license + " has check license", "Mã license:" + val_license + " đã được sử dụng");
                    licenseno.Clear();
                    licenseno.Focus();
                    return;
                }
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tiếngViệtToolStripMenuItem.Checked = false;
            englishToolStripMenuItem.Checked = true;
            lang = "ENG";
            try{ini.IniWriteValue("MainSection", "LANG", "ENG");}catch { }
        }

        private void tiếngViệtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tiếngViệtToolStripMenuItem.Checked = true;
            englishToolStripMenuItem.Checked = false;
            lang = "VNI";
            try { ini.IniWriteValue("MainSection", "LANG", "VNI"); } catch { }
            
        }
        private void cToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 main = new Form2();
            this.Visible = false;
            main.Show();
        }
        private async Task<Boolean> checkDup(int index)   
        {
            try
            {
                if(index!=1 && index != 2 && index != 3) return true;
                string sql = "select * from sfism4.R_SEC_LIC_LINK_T where carton_no !='" + val_carton + "' ";
                if (index == 1) sql = sql + " and SECURITY_1_NO='" + val_security1 + "' ";
                if (index == 2) sql = sql + " and SECURITY_2_NO='" + val_security2 + "' ";
                if (index == 3) sql = sql + " and license_no='" + val_license + "' ";
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data == null)
                {
                    return true;
                }
                else return false;
            }
            catch(Exception ex)
            {
                showMessage("Check duplicate license_no exception:" + ex.ToString(), "Lỗi ngoại lệ khi kiểm tra trùng lặp mã license:" + ex.ToString());
                return false;
            }

            return false;
        }
        public async Task<Boolean> checkLicenseRange(string parms1, string parms2)
        {
            if (!F_License) return true;
            string sql = "select * from sfism4.r_mo_ext3_T where MO_NUMBER IN( SELECT MO_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO='" + parms1 + "') AND  '" + parms2 + "' between ITEM_1 AND ITEM_2 and length('" + parms2 + "') = 10";
            var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_2.Data == null)
            {
                return false;
            }
            else
            {
                sql = "select * from sfism4.R_DATA_INPUT_T where MO_NUMBER IN( SELECT MO_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO='" + parms1 + "') AND MAC1 = '" + parms2 + "' AND LENGTH(MAC1) = 10 ";
                var result_3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_3.Data == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public async Task<string> getNextStation(String parms)
        {
            String str = "select GROUP_NEXT from sfis1.c_route_control_t where route_code='" + parms + "' and  GROUP_NAME='" + mygroup + "' AND STATE_FLAG='0' ";
            str += " and STEP_SEQUENCE=(select max(STEP_SEQUENCE) from sfis1.c_route_control_t where route_code='" + parms + "' and  GROUP_NAME='" + mygroup + "' AND STATE_FLAG='0')";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = str, SfcCommandType = SfcCommandType.Text });
            return result.Data["group_next"]?.ToString() ?? "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 main = new Form2();
            this.Visible = false;
            main.Show();
        }
        

        private void Form1_FormClosing(object sender, FormClosedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("CHK_LICENSE"))
            {
                process.Kill();
            }
        }
        private void showMessage(string MessageEnglish, string MessageVietNam)
        {
            string msgText;
            if (lang == "VNI")
            {
                msgText = MessageVietNam;
            }else msgText = MessageEnglish;
            errorMessage.Text = msgText;
            MessageBox.Show(msgText);
        }
        
        
    }
}
