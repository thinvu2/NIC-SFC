using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Models;
using SFC_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADD_RWMAC
{
    public partial class Main_Prog : Form
    {
        OracleClientDBHelper db = new OracleClientDBHelper();
        DataTable dt;
        public string inputLogin, checkSum, baseUrl, loginDB, empNo, empPass, error_code, CK_DB;

        string sql, Query_Mo, othermodel_name, TA, TA_SN, S23SDATA, PLANT_CODE_MO, PLANT_CODE_SN, PREFIX_MO = "", PREFIX_SN = "";
        private ResponseModelSingle result_01;
        String Mmodel_name, C_SpecialRoute, C_VersionCode, C_FirstGroup; 
        Boolean errorflag, T23SFLAG;
        int SN_Length;
        public  SfcHttpClient _sfcHttpClient;
        public string varMAC = string.Empty;
        public string varSN = string.Empty;
        public string varSSN = string.Empty;
        public string varSSN2 = string.Empty;
        public string varMO = string.Empty;
        public Main_Prog()
        {
            AccessAPI();
            InitializeComponent();
          //  _sfcHttpClient = flogin._sfcHttpClient;
           // checkuser();
        }
        private async void AccessAPI()
        {
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
            baseUrl = argsInfor[1].ToString();
            loginDB = argsInfor[2].ToString();
            empNo = argsInfor[3].ToString();
            empPass = argsInfor[4].ToString();

            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButtons.OK);
            //    Environment.Exit(0);
            //}
            _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
            try
            {
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                sql = string.Format(@"SELECT * FROM SFIS1.C_EMP_DESC_T where EMP_NO = '{0}'", empNo);
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                
                lblManager.Text = string.Format("{0} - {1}", empNo, CK_DB = result.Data["emp_name"].ToString());
                tsLblConnectStatus.Text = string.Format("    Connected:  {0}", loginDB);
                sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME ='ADD_RWMAC' AND VR_VALUE = 'TRUE' ";
                var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_1.Data != null)
                {
                    CK_DB = result_1.Data["vr_name"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void checkuser()
        {
            tsLblConnectStatus.Text = string.Format("    Connected:  {0}", loginDB);
            lblManager.Text = string.Format("{0} - {1}", empNo, empPass);
        }

        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void Check_DB()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }
        private async Task<SfcHttpClient> test()
        {
            _sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
            try
            {
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
                sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME ='ADD_RWMAC' AND VR_VALUE = 'TRUE' ";
                var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_1.Data != null)
                {
                    CK_DB = result_1.Data["vr_name"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
            return _sfcHttpClient;
        }
        private async void Main_Prog_Load(object sender, EventArgs e)
        {
            string dir = @"C:\PACKING";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string folderName = Path.GetFullPath(@"C:\PACKING\addmac.txt");
            if (!File.Exists(folderName))
            {
                File.Create(folderName).Dispose();
            }

            string folderName_1 = Path.GetFullPath(@"C:\WINNT\SFIS.Ini");   // C:\Windows\SFIS.Ini
            if (File.Exists(folderName_1)) 
            {
                using (StreamReader sr = File.OpenText(@"C:\WINNT\SFIS.Ini"))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null) 
                    {
                        if (s == "[ADDMAC]")
                        {
                            if (sr.ReadLine() == "CHECKTXTMAC=TRUE")
                            {
                                Menu_CK_Mac.Checked = true;
                            }
                        }
                    }
                }
            }

            lblVersion.Text = " Version: " + Application.ProductVersion;

            Edit_SSN.Visible = false;
            Edit_SSN2.Visible = false;
            label4.Visible = false;
            label8.Visible = false;
        }

        private async void Edit_MO_KeyDown(object sender, KeyEventArgs e) 
        {
            if (e.KeyCode == Keys.Enter)
            {
                string SAP_MO_TYPE, MO_TYPE, C_FixSN;
                varMO = Edit_MO.Text;

                if (varMO.Length != 0)
                {
                    Edit_MO.SelectAll();
                    Edit_MO.Focus();
                    PREFIX_MO = "";

                    if (CK_DB == "CPEII") // New CPEII: Check SN Prefix 14/08/22
                    {
                        if (Ck_SNPrefix.Checked)
                        {
                            sql = " SELECT DISTINCT SUBSTR(SHIPPING_SN,0,4) C_PREFIX FROM SFISM4.R_NETG_PRIN_ALL_T WHERE MO_NUMBER = '" + varMO + "' ";
                            var result_5 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_5.Data.Count() == 0)
                            {
                                sql = " SELECT DISTINCT SUBSTR(ITEM_1,0,4) C_PREFIX FROM SFISM4.R_MO_EXT2_T WHERE MO_NUMBER = '" + varMO + "' AND LENGTH(ITEM_1) = 13 ";
                                result_5 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_5.Data.Count() == 0)
                                {
                                    sql = " SELECT DISTINCT SUBSTR(ITEM_1,0,4) C_PREFIX FROM SFISM4.R_MO_EXT3_T WHERE MO_NUMBER = '" + varMO + "' AND LENGTH(ITEM_1) = 13 ";
                                    result_5 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (result_5.Data.Count() == 0)
                                    {
                                        sql = " SELECT DISTINCT SUBSTR(ITEM_1,0,4) C_PREFIX FROM SFISM4.R_MO_EXT4_T WHERE MO_NUMBER = '" + varMO + "' AND LENGTH(ITEM_1) = 13 ";
                                        result_5 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                        if (result_5.Data.Count() == 0)
                                        {
                                            showMessage("" + varMO + " - Range label SSN of MO not found !\nR_NETG - EXT2 - EXT3 - EXT4", "" + varMO + " - Không tìm thấy dải SSN của MO. MO chưa lên dải !", true);
                                            return;
                                        }
                                        else if (result_5.Data.Count() > 1)
                                        {
                                            foreach (var row in result_5.Data)
                                            {
                                                PREFIX_MO = PREFIX_MO + row["c_prefix"].ToString() + ", ";
                                            }
                                            showMessage("" + varMO + " - There exist 2 range SSN label. Not link !\n" + PREFIX_MO + "", "Tồn tại 2 dải SSN. Không thể link !\nR_MO_EXT4_T", true);
                                            return;
                                        }
                                        else
                                        {
                                            sql = " SELECT DISTINCT SUBSTR(ITEM_1,0,4) C_PREFIX FROM SFISM4.R_MO_EXT4_T WHERE MO_NUMBER = '" + varMO + "' AND LENGTH(ITEM_1) = 13 ";
                                            var result_0 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                            PREFIX_MO = result_0.Data["c_prefix"].ToString();
                                        }
                                    }
                                    else if (result_5.Data.Count() > 1)
                                    {
                                        foreach (var row in result_5.Data)
                                        {
                                            PREFIX_MO = PREFIX_MO + row["c_prefix"].ToString() + ", ";
                                        }
                                        showMessage("" + varMO + " - There exist 2 range SSN label. Not link !\n" + PREFIX_MO + "", "Tồn tại 2 dải SSN. Không thể link !\nR_MO_EXT3_T", true);
                                        return;
                                    }
                                    else
                                    {
                                        sql = " SELECT DISTINCT SUBSTR(ITEM_1,0,4) C_PREFIX FROM SFISM4.R_MO_EXT3_T WHERE MO_NUMBER = '" + varMO + "' AND LENGTH(ITEM_1) = 13 ";
                                        var result_0 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                        PREFIX_MO = result_0.Data["c_prefix"].ToString();
                                    }
                                }
                                else if (result_5.Data.Count() > 1)
                                {
                                    foreach (var row in result_5.Data)
                                    {
                                        PREFIX_MO = PREFIX_MO + row["c_prefix"].ToString() + ", ";
                                    }
                                    showMessage("" + varMO + " - There exist 2 range SSN label. Not link !\n" + PREFIX_MO + "", "Tồn tại 2 dải SSN. Không thể link !\nR_MO_EXT2_T", true);
                                    return;
                                }
                                else
                                {
                                    sql = " SELECT DISTINCT SUBSTR(ITEM_1,0,4) C_PREFIX FROM SFISM4.R_MO_EXT2_T WHERE MO_NUMBER = '" + varMO + "' AND LENGTH(ITEM_1) = 13 ";
                                    var result_0 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    PREFIX_MO = result_0.Data["c_prefix"].ToString();
                                }
                            }
                            else if (result_5.Data.Count() > 1)
                            {
                                foreach (var row in result_5.Data)
                                {
                                    PREFIX_MO = PREFIX_MO + row["c_prefix"].ToString() + ", ";
                                }
                                showMessage("" + varMO + " - There exist 2 range SSN label. Not link !\n" + PREFIX_MO + "", "Tồn tại 2 dải SSN. Không thể link !\nR_NETG_PRIN_ALL_T", true);
                                return;
                            }
                            else
                            {
                                sql = " SELECT DISTINCT SUBSTR(SHIPPING_SN,0,4) C_PREFIX FROM SFISM4.R_NETG_PRIN_ALL_T WHERE MO_NUMBER = '" + varMO + "' ";
                                var result_0 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                PREFIX_MO = result_0.Data["c_prefix"].ToString();
                            }
                        }
                    }


                    if (!await CHECK_CHARACTER_0_9A_Z(varMO))
                    {
                        Edit_MO.SelectAll();
                        return;
                    }

                    sql = "SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T  WHERE   MO_NUMBER = '" + varMO + "' ";

                    var result = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data.Count() != 0)
                    {
                        Lab_Qty_RW1.Text = result.Data.Count().ToString();
                    }

                    if (Menu_SN_SSN.Checked)
                    {
                        Edit_SN.Enabled = false;
                        Edit_MAC.Enabled = false;
                        CheckB_Furu.Enabled = false;
                        CheckB_Home.Enabled = false;

                        sql = "SELECT B.* ,A.SAP_MO_TYPE  FROM SFISM4.R_BPCS_MOPLAN_T A, SFISM4.R_MO_BASE_T B  WHERE A.MO_NUMBER=B.MO_NUMBER AND A.MO_NUMBER = '" + varMO + "' ";
                        var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_1.Data == null)
                        {
                            showMessage("00012", "00012", false);
                            Edit_MO.SelectAll();
                            Edit_MO.Focus();
                            return;
                        }
                        else
                        {
                            PLANT_CODE_MO = (result_1.Data["site"]?.ToString()?? "");
                            Lab_Model.Text = result_1.Data["model_name"].ToString();
                            Lab_Qty_MO.Text = result_1.Data["target_qty"].ToString();
                            Lab_TA.Text = (result_1.Data["vender_part_no"]?.ToString()?? "");
                            Lab_Mo_type.Text = result_1.Data["mo_type"]?.ToString()?? "";

                            if (CK_DB != "NIC")
                            {
                                if (Lab_Mo_type.Text != "Rework")
                                {
                                    showMessage("80121", "80121", false);
                                    Edit_MO.SelectAll();
                                    Edit_MO.Focus();
                                    return;
                                }
                            }
                            else if (CK_DB == "NIC")
                            {
                                if ( (Lab_Mo_type.Text != "Rework") && (Lab_Mo_type.Text != "RMA") )
                                {
                                    showMessage("80121", "80121", false);
                                    Edit_MO.SelectAll();
                                    Edit_MO.Focus();
                                    return;
                                }
                            }

                            if ((result_1.Data["remark"]?.ToString()?? "") == "RMA")
                            {
                                CheckB_RMA.Checked = true;
                            }
                            else
                            if ((result_1.Data["sap_mo_type"]?.ToString()?? "") == "ZA13")
                            {
                                CheckB_RMA.Checked = true;
                            }
                            else
                            {
                                CheckB_RMA.Checked = false;
                            }

                            if (int.Parse(Lab_Qty_MO.Text) > int.Parse(Lab_Qty_RW1.Text))
                            {
                                Edit_MO.Enabled = false;
                                Edit_SN.Enabled = true;
                                Edit_MAC.Enabled = true;
                                Edit_SN.SelectAll();
                                Edit_SN.Focus();

                                if (CK_DB == "NIC")
                                {
                                    Edit_Length.Focus();
                                    Edit_Length.Enabled = true;
                                }
                            }
                            else
                            {
                                Edit_MO.SelectAll();
                                Edit_MAC.Enabled = false;
                            }

                            return;
                        }
                    }


                    Edit_MAC.Enabled = false;

                    Query_Mo = " SELECT  *  FROM SFISM4.R_BPCS_MOPLAN_T  WHERE MO_NUMBER = '" + varMO + "' ";
                    var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = Query_Mo, SfcCommandType = SfcCommandType.Text });
                    if (result_2.Data == null)
                    {
                        showMessage("80120", "80120", false);
                        Edit_MO.SelectAll();
                        Edit_MO.Focus();
                        return;
                    }
                    else
                    {
                        PLANT_CODE_MO = result_2.Data["site"]?.ToString()?? "";
                        Edit_MAC.Enabled = true;
                    }

                    SAP_MO_TYPE = result_2.Data["sap_mo_type"]?.ToString()?? "";
                    Lab_Qty_MO.Text = result_2.Data["target_qty"].ToString();
                    MO_TYPE = result_2.Data["mo_type"].ToString();
                    Lab_Model.Text = result_2.Data["model_name"].ToString();
                    Lab_Mo_type.Text = result_2.Data["mo_type"].ToString();

                    if(CK_DB == "ROKU") // ROKU
                    {
                        CheckB_Mac_Range.Checked = true;
                        Mmodel_name = result_2.Data["model_name"].ToString();

                        if (MO_TYPE != "Rework")
                        {
                            showMessage("80121", "80121", false);
                            Edit_MO.SelectAll();
                            Edit_MO.Focus();
                            return;
                        }
                    }

                    sql = " SELECT * FROM SFIS1.C_MODEL_DESC_T  WHERE MODEL_NAME = '" + Lab_Model.Text + "'  ";
                    var result_3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_3.Data == null)
                    {
                        showMessage("20104", "20104", false);
                        Edit_MO.SelectAll();
                        Edit_MO.Focus();
                        return;
                    }
                    else
                    {
                        Lab_Type.Text = result_3.Data["model_type"]?.ToString()?? "";
                    }

                    if (Lab_Type.Text.IndexOf("189") == -1)
                    {
                        if (CK_DB != "NIC")
                        {
                            if (MO_TYPE != "Rework")
                            {
                                showMessage("80121", "80121", false);
                                Edit_MO.SelectAll();
                                Edit_MO.Focus();
                                return;
                            }
                        }
                        else if (CK_DB == "NIC")
                        {
                            if ( (MO_TYPE != "Rework") && (MO_TYPE != "RMA") )
                            {
                                showMessage("80121", "80121", false);
                                Edit_MO.SelectAll();
                                Edit_MO.Focus();
                                return;
                            }
                        }
                    }

                    if (Lab_Type.Text.IndexOf("082") != -1)
                    {
                        CheckB_SN_MAC.Visible = true;
                        CheckB_SN_MAC.Checked = true;
                        CheckB_SN_MAC.Enabled = false;
                    }
                    else
                    {
                        CheckB_SN_MAC.Visible = false;
                        CheckB_SN_MAC.Checked = false;
                        CheckB_SN_MAC.Enabled = false;
                    }

                    if (Lab_Type.Text.IndexOf("H") != -1)
                    {
                        CheckB_Mac_Range.Checked = false;
                    }

                    sql = " SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + varMO + "' ";
                    var result_4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_4.Data == null)
                    {
                        showMessage("00012", "00012", false);
                        Edit_MO.SelectAll();
                        Edit_MO.Focus();
                        return;
                    }
                    else
                    {
                        if(CK_DB == "ROKU") //ROKU
                        {
                            C_SpecialRoute = result_4.Data["route_code"]?.ToString()?? "";
                            C_VersionCode = result_4.Data["version_code"]?.ToString()?? "";
                            C_FirstGroup = result_4.Data["default_group"]?.ToString()?? "";
                        }

                        if((CK_DB != "ROKU"))  //ROKU
                        {
                            if (Lab_Type.Text.IndexOf("189") == -1)
                            {
                                if ((result_4.Data["order_no"]?.ToString()?? "") == "NEW")
                                {
                                    showMessage("80122", "80122", false);
                                    Edit_MO.SelectAll();
                                    Edit_MO.Focus();
                                    return;
                                }
                                else if ((result_4.Data["order_no"]?.ToString()?? "") == "OTHER")
                                {
                                    othermodel_name = result_4.Data["cust_part_no"].ToString();
                                    if ((othermodel_name == "") || (othermodel_name is null))
                                    {
                                        showMessage("R105 CUST_PART_NO is null, OTHER model name not config ! ", "R105 CUST_PART_NO is null, OTHER model name not config ! ", true);
                                        return;
                                    }
                                    else
                                    {
                                        CheckB_Mac_Range.Checked = false;
                                    }
                                }
                                else if ((result_4.Data["order_no"]?.ToString()?? "") != "OLD")
                                {
                                    DialogResult result_9 = MessageBox.Show(" ORDER_NO in R105: " + result_4.Data["order_no"]?.ToString()?? "" + " have not station please use old MAC rework, disowning amc error !", "QUESTION", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                    if (result_9 == DialogResult.OK)
                                    {
                                        Edit_MAC.Enabled = true;
                                    }
                                    else
                                    {
                                        Edit_MO.SelectAll();
                                        Edit_MO.Focus();
                                        Edit_MAC.Enabled = false;
                                    }
                                }

                                if (int.Parse(Lab_Qty_MO.Text) > int.Parse(Lab_Qty_RW1.Text))
                                {
                                    Edit_MO.Enabled = false;
                                    Edit_MAC.Enabled = true;

                                    if (CK_DB == "NIC")
                                    {
                                        Edit_Length.Focus();
                                        Edit_Length.Enabled = true;
                                    }
                                }
                                else
                                {
                                    Edit_MO.SelectAll();
                                    Edit_MAC.Enabled = false;
                                }

                            }
                            else
                            {
                                if (((result_4.Data["vender_part_no"]?.ToString()?? "") == "") || ((result_4.Data["vender_part_no"]?.ToString()?? "") == "N/A"))
                                {
                                    showMessage("80389", "80389", false);
                                    Edit_MAC.Enabled = false;
                                    Edit_MO.SelectAll();
                                    Edit_MO.Focus();
                                    return;
                                }
                                else
                                {
                                    Lab_TA.Text = result_4.Data["vender_part_no"]?.ToString()?? "";
                                    TA = result_4.Data["vender_part_no"]?.ToString()?? "";
                                    Edit_MAC.Enabled = true;
                                    Edit_MAC.Focus();
                                    Edit_MO.Enabled = false;
                                }
                            }
                        }
                        else
                        {
                            if ((result_4.Data["order_no"]?.ToString()?? "") == "NEW")
                            {
                                showMessage("80122", "80122", false);
                                Edit_MO.SelectAll();
                                Edit_MO.Focus();
                                return;
                            }
                            else if ((result_4.Data["order_no"]?.ToString()?? "") == "OTHER")
                            {
                                othermodel_name = result_4.Data["cust_part_no"]?.ToString()?? "";
                                if ((othermodel_name == "") || (othermodel_name is null))
                                {
                                    showMessage("R105 CUST_PART_NO is null, OTHER model name not config ! ", "R105 CUST_PART_NO is null, OTHER model name not config ! ", true);
                                    return;
                                }
                                else
                                {
                                    CheckB_Mac_Range.Checked = false;
                                }
                            }
                            else if ((result_4.Data["order_no"]?.ToString()?? "") != "OLD")
                            {
                                DialogResult result_9 = MessageBox.Show(" ORDER_NO in R105: " + result_4.Data["order_no"]?.ToString()?? "" + " have not station please use old MAC rework, disowning amc error !", "QUESTION", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                if (result_9 == DialogResult.OK)
                                {
                                    Edit_MAC.Enabled = true;
                                }
                                else
                                {
                                    Edit_MO.SelectAll();
                                    Edit_MO.Focus();
                                    Edit_MAC.Enabled = false;
                                }
                            }
                        }
                    }

                }
            }
        }

        private async Task<Boolean> Check_History()   //ROKU
        {
            try
            {
                sql = " SELECT SYSDATE FROM DUAL@SFCODBH ";
                var result_2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_2.Data != null)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private async void Bt_Query_Click(object sender, EventArgs e)
        {    label15.Visible = true;
            Edit_MO_Qry.Visible = true;
            Lab_Qty_RW.Visible = true;
            Edit_MO_Qry.SelectAll();
            Edit_MO_Qry.Focus();
            await FindMo();
        }

        private async Task FindMo() 
        {
            if (Edit_MO_Qry.Text.Length != 0)
            {
               if ( !await CHECK_CHARACTER_0_9A_Z(Edit_MO_Qry.Text) )
                {
                    Edit_MO_Qry.SelectAll();
                    return;
                }

                Query_Mo = string.Format(@"SELECT  
                CASE WHEN B.WIP_GROUP IS NOT NULL THEN B.MO_NUMBER || '-' || B.WIP_GROUP ELSE 'NOT INPUT' END WIP_GROUP, A.*
                 FROM
                SFISM4.R_RW_MAC_SSN_T A
                LEFT JOIN SFISM4.R107 B
                ON A.SN = B.SERIAL_NUMBER
                 WHERE A.MO_NUMBER = '" + Edit_MO_Qry.Text + "'  ORDER BY  MAC");

                var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Query_Mo, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data.Count() == 0)
                {
                    Query_Mo = string.Format(@"SELECT  
                CASE WHEN B.WIP_GROUP IS NOT NULL THEN B.MO_NUMBER || '-' || B.WIP_GROUP ELSE 'NOT INPUT' END WIP_GROUP, A.*
                 FROM
                SFISM4.R_RW_MAC_SSN_T A
                LEFT JOIN SFISM4.R107 B
                ON A.SN = B.SERIAL_NUMBER
                 WHERE MAC =  '" + Edit_MO_Qry.Text + "' ");
                }

                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Query_Mo, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() != 0)
                {
                    var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoResizeColumns();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    Lab_Qty_RW.Text = result_list1.Data.Count().ToString();

                    Edit_MO_Qry.Focus();
                    Edit_MO_Qry.SelectAll();
                }
                else
                {
                    showMessage(" " + Edit_MO_Qry.Text + " - Data not fonud.Please check again!", "" + Edit_MO_Qry.Text + " - Dữ liệu không tồi tại. Vui lòng kiểm tra lại !", true);
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();

                    Lab_Qty_RW.Text = "0";

                    Edit_MO_Qry.Focus();
                    Edit_MO_Qry.SelectAll();
                    return;
                }
            }
        }

        private async void Main_Prog_Shown(object sender, EventArgs e)
        {
            Check_DB();

            label15.Visible = false;
            Edit_MO_Qry.Visible = false;
            Lab_Qty_RW.Visible = false;
            Edit_MO.Focus();

            errorflag = false;
            List_AddMac.Items.Clear();

            if (Menu_CK_Mac.Checked)
            {
                string path = Path.GetFullPath(@"C:\PACKING\addmac.txt");
                ResetText();

                using (StreamReader sr = File.OpenText(@"C:\PACKING\addmac.txt"))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                        List_AddMac.Items.Add(s);
                    }
                    sr.Close();
                }
            }

            if (CK_DB == "NIC")
            {
                label11.Visible = true;
                Edit_Length.Visible = true;
                Menu_Length.Visible = false;
            }

            if(CK_DB == "ROKU") //ROKU
            {
                Menu_16.Visible = false;
                Menu_19.Visible = false;
            }
        }

        private async void Edit_MAC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Edit_SN.Text == "UNDO" || Edit_MO.Text == "UNDO" || Edit_SSN.Text == "UNDO" || Edit_MAC.Text == "UNDO" || Edit_SSN2.Text == "UNDO")
                {
                    Edit_SN.Text = "";
                    Edit_Length.Text = "";
                    Edit_MO.Text = "";
                    Edit_SSN.Text = "";
                    Edit_MAC.Text = "";
                    Edit_SSN2.Text = "";
                    varMAC = "";
                    varMO = "";
                    varSN = "";
                    varSSN2 = "";
                    Edit_MO.Enabled = true;
                    Edit_MO.Focus();
                }
                else
                    if (Menu_IpQR.Checked == true)
                    {
                        var result1 = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.SP_GET_QRSTRING",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="IN_GROUP",Value="ADDRWMAC",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="IN_DATA",Value=Edit_MAC.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                    new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                }
                        });
                        dynamic ads = result1.Data;
                        string _RES = ads[1]["res"];
                        if (_RES.StartsWith("OK")) Edit_MAC.Text = ads[0]["out_data"];
                        varMAC = Edit_MAC.Text;
                        await Load_Mac_Key();
                }
                else
                {
                    varMAC = Edit_MAC.Text;
                    await Load_Mac_Key();
                }
            }
        }

        private async Task Load_Mac_Key()
        {
            string mo_number, tmp_str, tmp_model_name;
            string foundmac, MSERIAL_NUMBER = "", strmodel_name, ssn = "", pincode = "", password = "", mac1 = "", ssid = "", mac2 = "", webkey = "", macid = "";
            Boolean rmamo;
            string m_bom_no;
            string macrange = "";
            string model_name = "", s_ssn = "";
            PREFIX_SN = "";

            if (varMO.Length == 0) 
            {
                showMessage("Please input MO !", "Vui lòng nhập MO !", true);
                Edit_MO.Enabled = true;
                Edit_MO.Focus();
                return;
            }

            if ( (varMAC.Length == 0) || (varMAC.Length <= 2) )
            {
                showMessage("Please input MAC !", "Vui lòng nhập MAC !", true);
                Edit_MAC.Focus();
                Edit_MAC.SelectAll();
                return;
            }

            Edit_MAC.Text = varMAC.Trim();

            if (varMAC.Length > 50)  // For VSC1000
            {
                sql = " SELECT REPLACE( SUBSTR('" + varMAC + "',LENGTH('" + varMAC + "') - 16  ,17 ),':','') MAC  FROM DUAL ";
                var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                Edit_MAC.Text = result_1.Data["mac"].ToString();
            }

            if (Edit_MAC.Text.IndexOf(",") != -1 && Edit_MAC.Text.ToString().Length >= 27)
            {
                varMAC = varMAC.Substring(14, 12).Trim();
            }

            if (Lab_Type.Text.IndexOf("189") != -1)
            {
                Edit_SSN.Enabled = false;
            }

            if (Menu_Upper.Checked == true)
            {
                Edit_MAC.Text = (varMAC).ToUpper();
            }
            else if (Menu_Lower.Checked == true)
            {
                Edit_MAC.Text = (varMAC).ToLower();
            }

            if (varMAC.Substring(0, 3) == "23S" && varMAC.Length >= 15)
            {
                S23SDATA = varMAC;
                Edit_MAC.Text = varMAC.Substring(3, 12);
                varMAC = Edit_MAC.Text;
                T23SFLAG = true;
            }
            else
            {
                T23SFLAG = false;
                S23SDATA = "";
            }

            if (Menu_CK_Mac.Checked)
            {
                if (List_AddMac.Items.IndexOf(Edit_MAC.Text) == -1)
                {
                    showMessage("80124", "80124", false);
                    Edit_MAC.SelectAll();
                    return;
                }
            }

            sql = " SELECT Count(*) SN_COUNT FROM SFISM4.R_RW_MAC_SSN_T WHERE   MO_NUMBER = '" + varMO + "' ";
            var result_3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_3.Data != null)
            {
                if (Int32.Parse(result_3.Data["sn_count"].ToString()) >= Int32.Parse(Lab_Qty_MO.Text))
                {
                    showMessage("80125", "80125", false);
                    Edit_SN.SelectAll();
                    Edit_SN.Focus();
                    return;
                }
            }


            if (Menu_SN_SSN.Checked)
            {
                errorflag = false;
                await CHECKLINK_SN_SSN("OK");
                return;
            }

            sql = " SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE  MO_NUMBER = '" + varMO + "' AND ROWNUM = 1 ";
            result_3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_3.Data != null)
            {
                if ((result_3.Data["sap_mo_type"]?.ToString()?? "") == "ZA13")
                {
                    if (await CHECKRMALINK(Edit_SN.Text, varMAC, Edit_SSN.Text))
                    {
                        if (await MARK_HISTORY_DATA(Edit_SN.Text, varMAC, Edit_SSN.Text) == false)
                        {
                            showMessage("80126", "80126", false);
                            Edit_MAC.SelectAll();
                            Edit_MAC.Focus();
                            return;
                        }
                    }
                    else
                    {
                        showMessage("80127", "80127", false);
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                        return;
                    }
                }
            }

            rmamo = false;

            sql = " SELECT * FROM SFISM4.R105  WHERE MO_NUMBER = '" + varMO + "' AND ROWNUM = 1 ";
            var result_4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_4.Data == null)
            {
                showMessage("00012", "00012", false);
                Edit_MO.SelectAll();
                Edit_MO.Focus();
                return;
            }

            strmodel_name = result_4.Data["model_name"].ToString();
            m_bom_no = result_4.Data["bom_no"]?.ToString()?? "";

            if ((result_4.Data["remark"]?.ToString() ?? "") == "RMA")
            {
                rmamo = true;
            }

            if (CK_DB != "NIC")
            {
                if (Menu_10.Checked)
                {
                    if ((varMAC.Trim()).Length != 10)
                    {
                        showMessage("80128", "80128", false);
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                        return;
                    }
                }

                if (Menu_16.Checked)
                {
                    if ((varMAC.Trim()).Length != 16)
                    {
                        showMessage("80128", "80128", false);
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                        return;
                    }
                }

                if (Menu_19.Checked)
                {
                    if ((varMAC.Trim()).Length != 19)
                    {
                        showMessage("80128", "80128", false);
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                        return;
                    }
                }

                if (Menu_12.Checked)
                {
                    if ((varMAC.Trim()).Length != 12 && (varMAC.Trim()).Length != 13)
                    {
                        showMessage("80128", "80128", false);
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                        return;
                    }
                }
            }
            else
            {
                if (varMAC.Trim().Length != SN_Length)
                {
                    showMessage("Length MAC <> Length Setup !", "Độ dài MAC <> Độ dài đã thiết lập !", false);
                    Edit_MAC.SelectAll();
                    Edit_MAC.Focus();

                    if (Edit_Length.Text.Trim().Length == 0)
                    {
                        Edit_Length.Focus();
                        Edit_Length.SelectAll();
                    }
                    return;
                }
            }

            if (CK_DB == "CPEII") // New CPEII: Check SN Prefix 14/08/22
            {
                if (Ck_SNPrefix.Checked)
                {
                    sql = "SELECT  *  FROM SFISM4.R_NETG_PRIN_ALL_T WHERE  MACID = '" + varMAC + "'  OR SHIPPING_SN = '" + varMAC + "' AND ROWNUM=1 ";
                    var result_5 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    mo_number = result_5.Data["mo_number"].ToString();

                    sql = " SELECT DISTINCT SUBSTR(SHIPPING_SN,0,4) C_PREFIX FROM SFISM4.R_NETG_PRIN_ALL_T WHERE MO_NUMBER = '" + mo_number + "' ";
                    var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_list.Data.Count() > 1)
                    {
                        foreach (var row in result_list.Data)
                        {
                            PREFIX_SN = PREFIX_SN + row["c_prefix"].ToString() + ", ";
                        }
                        showMessage("" + mo_number + " - There exist 2 range SSN label. Not link !\nR_NETG: " + PREFIX_SN + "", "Tồn tại 2 dải SSN. Không thể link !", true);
                        return;
                    }
                    else
                    {
                        sql = " SELECT DISTINCT SUBSTR(SHIPPING_SN,0,4) C_PREFIX FROM SFISM4.R_NETG_PRIN_ALL_T WHERE MO_NUMBER = '" + mo_number + "' ";
                        result_5 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        PREFIX_SN = result_5.Data["c_prefix"].ToString();

                        if (PREFIX_MO != PREFIX_SN)
                        {
                            showMessage("Prefix of MO <> Prefix of SN. Not link !\nMO: " + PREFIX_MO + " - " + PREFIX_SN + " : SN", "Tiền tố của MO <> tiền tố của SN. Không thể link !", true);
                            Edit_MAC.Focus();
                            Edit_MAC.SelectAll();
                            return;
                        }
                    }
                }
            }

            if (CK_DB != "ROKU")  // ROKU
            {
                sql = " SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + varMAC + "' AND GD = 'BD2C' AND MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_SERIAL = 'NIC') ";
                var result_5 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_5.Data != null)
                {
                    showMessage("SN SAVED BD2C, CAN''T ADDMAC TO MO REWORK !", "SN SAVED BD2C, CAN''T ADDMAC TO MO REWORK ! ", true);
                    Edit_MAC.SelectAll();
                    return;
                }
            }

            if (!Menu_SN_SSN.Checked)
            {
                if (CheckB_Range.Checked)
                {
                    foundmac = "N";
                    macrange = "F";

                    if (CK_DB == "NIC")
                    {
                        if (SN_Length == 12)
                        {
                            sql = " SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '" + varMAC + "'  BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1) = 10) ";
                        }
                        else
                        {
                            sql = " SELECT * FROM SFISM4.R_MO_EXT2_T WHERE '" + varMAC + "'  BETWEEN ITEM_1 AND ITEM_2 ";
                        }
                    }
                    else
                    {
                        if (Menu_10.Checked || Menu_19.Checked || Menu_16.Checked)
                        {
                            sql = " SELECT  * FROM SFISM4.R_MO_EXT3_T WHERE  '" + varMAC + "'  BETWEEN  ITEM_1  AND  ITEM_2 ";
                        }
                        else
                        {
                            sql = " SELECT  * FROM SFISM4.R_MO_EXT4_T WHERE '" + varMAC + "' BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                        }
                    }

                    var result_6 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_6.Data == null)
                    {
                        if (Lab_Type.Text.IndexOf("189") == -1)
                        {
                            sql = "SELECT * FROM  SFISM4.R_INVENTEL_DATA_T WHERE MAC = '" + varMAC + "' ";
                            var result_7 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_7.Data != null)
                            {
                                mo_number = result_7.Data["mo_number"].ToString();

                                sql = " SELECT  *  FROM SFISM4.R_MO_BASE_T WHERE  MO_NUMBER = '" + mo_number + "' ";
                                var result_8 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_8.Data != null)
                                {
                                    model_name = result_8.Data["model_name"].ToString();

                                    if ((result_8.Data["CLOSE_FLAG"]?.ToString()?? "") == "2")
                                    {
                                        showMessage("80129", "80129", false);
                                        Edit_MAC.SelectAll();
                                        Edit_MAC.Focus();
                                        return;
                                    }
                                }
                                else
                                {
                                    sql = " SELECT  *  FROM SFISM4.H_MO_BASE_T WHERE  MO_NUMBER = '" + mo_number + "' ";
                                    var result_9 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    model_name = result_9.Data["model_name"].ToString();
                                }

                                macrange = "Y";
                            }
                            else
                            {
                                if (CK_DB == "NIC")
                                {
                                    if (SN_Length == 12)
                                    {
                                        sql = " SELECT* FROM SFISM4.H_MO_EXT4_T WHERE '" + varMAC + "' BETWEEN ITEM_1 AND ITEM_2 AND(LENGTH(ITEM_1) = 12 OR LENGTH(ITEM_1) = 10) "; 
                                    }
                                    else
                                    {
                                        sql = " SELECT * FROM SFISM4.H_MO_EXT3_T WHERE '" + varMAC + "' BETWEEN ITEM_1 AND ITEM_2  ";
                                    }
                                }
                                else
                                {
                                    if (Menu_10.Checked || Menu_19.Checked || Menu_16.Checked)
                                    {
                                        sql = " SELECT  * FROM SFISM4.H_MO_EXT3_T WHERE  '" + varMAC + "'  BETWEEN  ITEM_1  AND  ITEM_2 ";
                                    }
                                    else
                                    {
                                        sql = " SELECT  * FROM SFISM4.H_MO_EXT4_T WHERE '" + varMAC + "' BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                                    }
                                }

                                var result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_01.Data != null)
                                {
                                    macrange = "Y";
                                    model_name = result_01.Data["ver_4"]?.ToString()?? "";
                                }
                                else
                                {
                                    if(CK_DB == "ROKU")  // ROKU
                                    {
                                        if(await Check_History())
                                        {
                                            sql = " SELECT  * FROM SFISM4.R_MO_EXT4_T@SFCODBH WHERE '" + varMAC + "' BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                                        }
                                        else
                                        {
                                            sql = " SELECT  * FROM SFISM4.r_MO_EXT4_T WHERE '" + varMAC + "' BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                                        }
                                    }
                                    else
                                    {
                                        sql = " SELECT  * FROM SFISM4.R_MO_EXT4_T@SFCODBH WHERE '" + varMAC + "' BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                                    }
                                    
                                    var result_02 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (result_02.Data != null)
                                    {
                                        macrange = "Y";
                                        model_name = result_02.Data["ver_4"]?.ToString()?? "";
                                    }
                                }
                            }
                        }
                        else
                        {
                            sql = " SELECT  *  FROM  SFISM4.R_NETG_PRIN_ALL_T WHERE  MACID =  '" + varMAC + "' or SHIPPING_SN = '" + varMAC + "' AND ROWNUM=1 ";
                            var result_03 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_03.Data != null)
                            {
                                macid = result_03.Data["macid"].ToString();
                                mo_number = result_03.Data["mo_number"].ToString();
                                ssn = result_03.Data["shipping_sn"].ToString();
                                pincode = result_03.Data["pin_code"]?.ToString()?? "";
                                password = result_03.Data["temp4"]?.ToString() ?? "";
                                ssid = result_03.Data["ssid"]?.ToString() ?? "";
                                mac1 = result_03.Data["macid1"]?.ToString() ?? "";
                                mac2 = result_03.Data["macid2"]?.ToString() ?? "";
                                webkey = result_03.Data["webkey"]?.ToString() ?? "";
                                Edit_SSN.Text = ssn;

                                sql = " SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + mo_number + "' ";
                                var result_04 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_04.Data != null)
                                {
                                    PLANT_CODE_SN = (result_04.Data["site"]?.ToString() ?? "");
                                    model_name = result_04.Data["model_name"].ToString();

                                    if (PLANT_CODE_MO != PLANT_CODE_SN)  // For CPEII
                                    {
                                        showMessage("Plant_code MO<>SN: " + PLANT_CODE_MO + " <> " + PLANT_CODE_SN + " - difference. Not link ! ", "Plant_code MO<>SN:" + PLANT_CODE_MO + " <> " + PLANT_CODE_SN + " - Khác mã xưởng. Không thể Link ! ", true);
                                        Edit_MAC.Focus();
                                        Edit_MAC.SelectAll();
                                        return;
                                    }
                                }
                                else
                                {
                                    sql = " SELECT * FROM SFISM4.H_MO_BASE_T WHERE MO_NUMBER = '" + mo_number + "' ";
                                    var result_05 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    model_name = result_05.Data["model_name"].ToString();
                                }

                                macrange = "Y";

                            }
                            else
                            {
                                if (CK_DB == "NIC")
                                {
                                    if (SN_Length == 12)
                                    {
                                        sql = " SELECT * FROM SFISM4.H_MO_EXT4_T WHERE '" + varMAC + "' BETWEEN ITEM_1 AND ITEM_2 AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                                    }
                                    else
                                    {
                                        sql = " SELECT * FROM SFISM4.H_MO_EXT3_T WHERE '" + varMAC + "' BETWEEN ITEM_1 AND ITEM_2   ";
                                    }
                                }
                                else
                                {
                                    if (Menu_10.Checked || Menu_19.Checked || Menu_16.Checked)
                                    {
                                        sql = " SELECT  * FROM SFISM4.H_MO_EXT3_T WHERE  '" + varMAC + "'  BETWEEN  ITEM_1  AND  ITEM_2 ";
                                    }
                                    else
                                    {
                                        sql = " SELECT  * FROM SFISM4.H_MO_EXT4_T WHERE '" + varMAC + "' BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                                    }
                                }

                                var result_06 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_06.Data != null)
                                {
                                    macrange = "Y";
                                    model_name = result_06.Data["ver_4"]?.ToString()?? "";
                                }
                                else
                                {
                                    sql = " SELECT  * FROM SFISM4.r_MO_EXT4_T@SFCODBH WHERE '" + varMAC + "' BETWEEN  ITEM_1  AND  ITEM_2  AND (LENGTH(ITEM_1)=12 OR LENGTH(ITEM_1)=10) ";
                                    var result_07 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                    if (result_07.Data != null)
                                    {
                                        macrange = "Y";
                                        model_name = result_07.Data["ver_4"]?.ToString()?? "";
                                    }
                                }

                            }
                        }
                    }
                    else
                    {

                        model_name = result_6.Data["ver_4"]?.ToString()?? "";
                        macrange = "Y";
                        mo_number = result_6.Data["mo_number"].ToString();

                        sql = " SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '" + mo_number + "' ";
                        var result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_01.Data != null)
                        {
                            if (Lab_Type.Text.IndexOf("189") == -1)
                            {
                                if ((result_01.Data["close_flag"]?.ToString() ?? "") == "2")
                                {
                                    showMessage("80130", "80130", false);
                                    Edit_MAC.SelectAll();
                                    Edit_MAC.Focus();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            showMessage("MO "+ mo_number + " of range not exist in R105 !", "MO " + mo_number + " của dải không tồn tại trong R105 !", true);
                            Edit_MAC.SelectAll();
                            Edit_MAC.Focus();
                            return;
                        }
                    }


                    if (Lab_Type.Text.IndexOf("189") == -1)
                    {
                        if (macrange == "F")
                        {
                            sql = " SELECT * FROM  SFISM4.MMS_RANGE_USED  WHERE '" + varMAC + "' BETWEEN  FRONT_ID||BEGIN_ID  AND FRONT_ID||END_ID ";
                            var result_02 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_02.Data != null)
                            {
                                if ((result_02.Data["product_id"]?.ToString() ?? "") != "")
                                {
                                    macrange = "Y";
                                    model_name = result_02.Data["product_id"]?.ToString() ?? "";
                                }

                            }
                        }

                        if (Menu_CK_SEQ.Checked)
                        {
                            tmp_str = await CHECKMAC_IDTYPE(varMAC);

                            if (tmp_str != "OK")
                            {
                                showMessage(""+ tmp_str + "", "" + tmp_str + "", true);
                                return;
                            }
                        }

                        if (macrange == "F")
                        {
                            showMessage("80131", "80131", false);
                            Edit_MAC.SelectAll();
                            Edit_MAC.Focus();
                            return;
                        }

                    }

                    if ((CK_DB != "NIC") && (model_name == ""))
                    {
                        showMessage("MODEL of MAC not found !\nCheck Range or R_NETG_PRIN_ALL_T ", "MODEL của MAC không tim thấy !", true);
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                        return;
                    }

                    if (!Menu_Change_Mod.Checked)
                    {
                        if (Lab_Model.Text != model_name)
                        {
                            if (CheckB_Mac_Range.Checked)
                            {
                                showMessage("80132", "80132", false);
                                Edit_MAC.SelectAll();
                                Edit_MAC.Focus();
                                return;
                            }
                            else
                            {
                                if (othermodel_name != model_name)
                                {
                                    showMessage("R105 CUST_PART_NO: " + othermodel_name + " <> " + model_name + " - ERROR ", " R105 CUST_PART_NO: " + othermodel_name + " <> " + model_name + " - ERROR ", true);
                                    Edit_MAC.SelectAll();
                                    return;
                                }
                            }

                        }

                    }
                }

            }

            if (!Menu_SN_SSN.Checked)
            {
                if (!rmamo)
                {
                    if (CK_DB != "NIC")
                    {
                        if (CK_DB == "ROKU")  // ROKU
                        {
                            if(await Check_History())
                            {
                                sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE MACID =  '" + varMAC + "' ";
                            }
                            else
                            {
                                sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T WHERE MACID =  '" + varMAC + "' ";
                            }
                        }
                        else
                        {
                            sql = " SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE MACID =  '" + varMAC + "' ";
                        }
                         
                        var result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_01.Data != null)
                        {
                            showMessage("80133", "80133", false);
                            Edit_MAC.SelectAll();
                            Edit_MAC.Focus();
                            return;
                        }
                    }

                    if (Lab_Type.Text.IndexOf("189") == -1)
                    {
                        sql = " SELECT  *  FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_SN = '" + varMAC + "' ";
                        var result_02 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_02.Data == null)
                        {
                            sql = " SELECT  *  FROM SFISM4.H_WIP_KEYPARTS_T WHERE KEY_PART_SN = '" + varMAC + "' ";
                            var result_03 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_03.Data != null)
                            {
                                MSERIAL_NUMBER = result_03.Data["serial_number"].ToString();

                                sql = " SELECT  *  FROM SFISM4.H_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + MSERIAL_NUMBER + "' ";
                                var result_04 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                                if(result_04.Data != null)
                                {
                                    if ((result_04.Data["scrap_flag"]?.ToString() ?? "") == "0")
                                    {
                                        showMessage("80134", "80134", false);
                                        Edit_MAC.SelectAll();
                                        Edit_MAC.Focus();
                                        return;
                                    }
                                }
                                
                            }
                        }
                        else
                        {

                            tmp_str = result_02.Data["serial_number"].ToString();

                            sql = " SELECT  *  FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + tmp_str + "' ";
                            var result_05 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            tmp_model_name = result_05.Data["model_name"].ToString();

                            if ((result_05.Data["scrap_flag"]?.ToString() ?? "") != "0")
                            {
                                sql = " UPDATE SFISM4.R_WIP_KEYPARTS_T SET KEY_PART_SN =  'RW'|| '" + varMAC + "' ";
                                sql = sql + " WHERE SERIAL_NUMBER =  '" + tmp_str + "' AND KEY_PART_SN = '" + varMAC + "' ";
                                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                                if (a.Result != "OK")
                                {
                                    ReportError(a.Message.ToString());
                                    return;
                                }

                                sql = " UPDATE SFISM4.R_WIP_TRACKING_T  SET PO_NO = PO_NO||'RW', SHIPPING_SN = SHIPPING_SN||'RW', ";
                                sql = sql + " SHIPPING_SN2 = SHIPPING_SN2||'RW' WHERE SERIAL_NUMBER = '" + tmp_str + "' ";
                                var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                                if (b.Result != "OK")
                                {
                                    ReportError(b.Message.ToString());
                                    return;
                                }

                                sql = " DELETE  SFISM4.R_UNIQUE_SSN_T WHERE SERIAL_NUMBER = '" + tmp_str + "' ";
                                var c = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                                if (c.Result != "OK")
                                {
                                    ReportError(c.Message.ToString());
                                    return;
                                }

                                if ((tmp_model_name.Substring(0, 4) == "BB5G") ||
                                    (tmp_model_name.Substring(0, 4) == "BB7G") ||
                                    (tmp_model_name.Substring(0, 6) == "T60X98"))
                                {
                                    sql = " DELETE  SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '" + tmp_str + "' ";
                                    var d = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                                    if (d.Result != "OK")
                                    {
                                        ReportError(d.Message.ToString());
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                showMessage("80134", "80134", false);
                                Edit_MAC.SelectAll();
                                Edit_MAC.Focus();
                                return;
                            }
                        }

                        sql = " SELECT  * FROM  SFISM4.Z_WIP_TRACKING_T WHERE   SHIPPING_SN2 = '" + varMAC + "' ";
                        var result_06 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_06.Data != null)
                        {
                            showMessage("80135", "80135", false);
                            Edit_MAC.SelectAll();
                            Edit_MAC.Focus();
                            return;
                        }
                        sql = " SELECT  * FROM  SFISM4.r_WIP_TRACKING_T WHERE   serial_number = '" + varMAC + "' and wip_group not in ('BC2M','BC8M')";
                        var result_r = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_r.Data != null)
                        {
                            showMessage("Wip <> 'BC2M','BC8M' khong the add mac ", "Wip <> 'BC2M','BC8M' khong the add mac  ", true);
                            Edit_MAC.SelectAll();
                            Edit_MAC.Focus();
                            return;
                        }
                    }
                    else
                    {
                        sql = " SELECT  *  FROM SFISM4.R_NETG_PRIN_ALL_T WHERE  MACID = '" + varMAC + "'  OR SHIPPING_SN = '" + varMAC + "' AND ROWNUM=1  ";
                        var result_07 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_07.Data == null)
                        {
                            showMessage("No data R_NETG_PRIN_ALL_T. Call LABELROOM to check ! ", "Không có dữ liệu R_NETG_PRIN_ALL_T. Gọi LABELROOM kiểm tra ! ", true);
                            Edit_MAC.SelectAll();
                            return;
                        }
                        else
                        {
                            macid = result_07.Data["macid"].ToString();
                            mo_number = result_07.Data["mo_number"].ToString();
                            strmodel_name = result_07.Data["model_name"].ToString();
                            ssn = result_07.Data["shipping_sn"].ToString();
                            pincode = result_07.Data["pin_code"]?.ToString() ?? "";
                            password = result_07.Data["temp4"]?.ToString() ?? "";
                            ssid = result_07.Data["ssid"]?.ToString() ?? "";
                            mac1 = result_07.Data["macid1"]?.ToString() ?? "";
                            mac2 = result_07.Data["macid2"]?.ToString() ?? "";
                            webkey = result_07.Data["webkey"]?.ToString() ?? "";
                            Edit_SSN.Text = ssn;

                            sql = " SELECT  *  FROM SFISM4.R105 WHERE  MO_NUMBER = '" + mo_number + "'  ";
                            var result_08 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_08.Data == null)
                            {
                                showMessage("" + mo_number + " - No data R105.Please to check! ", "" + mo_number + " - Không có dữ liệu R105. Vui lòng kiểm tra ! ", true);
                                Edit_MAC.SelectAll();
                                return;
                            }
                            else
                            {
                                PLANT_CODE_SN = result_08.Data["site"]?.ToString()?? "";

                                if (PLANT_CODE_MO != PLANT_CODE_SN)  // For CPEII
                                {
                                    showMessage("Plant_code MO<>SN: " + PLANT_CODE_MO + " <> " + PLANT_CODE_SN + " - difference. Not link ! ", "Plant_code MO<>SN:" + PLANT_CODE_MO + " <> " + PLANT_CODE_SN + " - Khác mã xưởng. Không thể Link ! ", true);
                                    Edit_MAC.Focus();
                                    Edit_MAC.SelectAll();
                                    return;
                                }

                                if (Lab_Model.Text == strmodel_name)
                                {
                                    TA_SN = result_08.Data["vender_part_no"]?.ToString() ?? "";

                                    if (TA != TA_SN)
                                    {
                                        showMessage("" + TA + " <> " + TA_SN + " - TA_VERSION different.Please to check! ", "" + TA + " <> " + TA_SN + " - TA_VERSION khác nhau. Vui lòng kiểm tra ! ", true);
                                        Edit_MAC.SelectAll();
                                        return;
                                    }
                                    else
                                    {
                                        sql = " SELECT  *  FROM SFISM4.R_CUSTSN_T WHERE SSN1 = '" + ssn + "'  OR MAC1 = '" + varMAC + "'  ";
                                        var result_09 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                        if (result_09.Data == null)
                                        {
                                            sql = " SELECT  *  FROM SFISM4.R108 WHERE  KEY_PART_SN = '" + varMAC + "'  ";
                                            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                            if (result_1.Data == null)
                                            {
                                                sql = " SELECT  *  FROM SFISM4.R107 WHERE ( SHIPPING_SN = '" + ssn + "' OR PO_NO = '" + ssn + "' OR SHIPPING_SN2 = '" + varMAC + "'  ) ";
                                                var result_03 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                                if (result_03.Data != null)
                                                {
                                                    showMessage("SSN OR MACID exist in R107 . Please to check ! ", "SSN OR MACID đã tồn tại trong R107 . Vui lòng kiểm tra ! ", true);
                                                    Edit_MAC.SelectAll();
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                showMessage("MACID EXITSTS R108 . Please to check ! ", "MACID đã tồn tại trong R108 . Vui lòng kiểm tra ! ", true);
                                                Edit_MAC.SelectAll();
                                                return;
                                            }

                                        }
                                        else
                                        {
                                            showMessage("SSN OR MACID EXITSTS R_CUSTSN . Please to check ! ", "SSN OR MACID đã tồn tại trong R_CUSTSN . Vui lòng kiểm tra ! ", true);
                                            Edit_MAC.SelectAll();
                                            return;
                                        }
                                    }

                                }
                                else
                                {
                                    showMessage("MODEL_MO: " + Lab_Model.Text + " <> " + strmodel_name + " - MODEL_MAC different. Please to check ! ", "MODEL_MO: " + Lab_Model.Text + " <> " + strmodel_name + " - MODEL_MAC khác nhau. Vui lòng kiểm tra ! ", true);
                                    Edit_MAC.SelectAll();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            foundmac = "N";

            sql = " SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + varMAC + "' OR  SSN =  '" + varMAC + "'  ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                if (result.Data["mo_number"].ToString() != varMO)
                {
                    DialogResult result_05 = MessageBox.Show(" " + varMAC + " - " + result.Data["mo_number"].ToString() + " : MACID already accede. Can you need change now rework MO ?", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result_05 == DialogResult.OK)
                    {
                        foundmac = "Y";
                    }
                }
                else
                {
                    showMessage("MACID -  " + varMAC + "-  R_RW_MAC_SSN_T already accede ! ", "MACID -  " + varMAC + " - R_RW_MAC_SSN_T đã tồn tại ! ", true);
                    List_ErrorSN.Items.Add("" + varMAC + "   Exist");

                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();

                    sql = " SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + varMAC + "' OR  SSN =  '" + varMAC + "'  ";
                    var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_list1.Data.Count() != 0)
                    {
                        var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                        dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                        dataGridView1.DataSource = dt;
                        dataGridView1.AutoResizeColumns();
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    }
                    Edit_MAC.SelectAll();
                    return;
                }
            }

            if (Lab_Type.Text.IndexOf("189") == -1)  // B04
            {
                if (!Menu_SN_SSN.Checked)
                {
                    sql = " SELECT * FROM SFIS1.C_CUSTSN_RULE_T WHERE  MODEL_NAME = '" + model_name + "'  AND CUSTSN_CODE= 'SSN1' AND  COMPARE_SN = 'SN'  ";
                    var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_1.Data != null)
                    {
                        sql = " UPDATE  SFISM4.R_WIP_TRACKING_T SET SERIAL_NUMBER = MO_NUMBER||SERIAL_NUMBER,  ";
                        sql = sql + " SHIPPING_SN = MO_NUMBER||SHIPPING_SN, PO_NO = MO_NUMBER||PO_NO,  ";
                        sql = sql + " SHIPPING_SN2 = MO_NUMBER||SHIPPING_SN2 WHERE  SERIAL_NUMBER =  '" + MSERIAL_NUMBER + "' ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return;
                        }

                        sql = " DELETE  SFISM4.R_CUSTSN_T  WHERE  SERIAL_NUMBER = '" + MSERIAL_NUMBER + "' ";
                        var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (b.Result != "OK")
                        {
                            ReportError(b.Message.ToString());
                            return;
                        }

                        sql = " UPDATE SFISM4.R_SN_DETAIL_T SET SERIAL_NUMBER = SERIAL_NUMBER  WHERE   SERIAL_NUMBER = '" + MSERIAL_NUMBER + "' ";
                        var c = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (c.Result != "OK")
                        {
                            ReportError(c.Message.ToString());
                            return;
                        }
                    }
                }

                if (!Menu_SN_SSN.Checked)
                {
                    sql = " SELECT COUNT(*) COUNT_RW FROM SFISM4.R_RW_MAC_SSN_T WHERE MO_NUMBER = '" + varMO + "'  ";
                    var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (Int32.Parse(result1.Data["count_rw"].ToString()) >= (Int32.Parse(Lab_Qty_MO.Text) + 20))
                    {
                        showMessage("80136", "80136", false);
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                        return;
                    }
                }
            }
            else
            {
                sql = " SELECT  COUNT(*) COUNT_RW1 FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + varMAC + "' OR  SSN =  '" + varMAC + "'  ";
                result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                if (Int32.Parse(result.Data["count_rw1"].ToString()) == (Int32.Parse(Lab_Qty_MO.Text)))
                {
                    showMessage("80136", "80136", false);
                    Edit_MAC.SelectAll();
                    Edit_MAC.Focus();
                    return;
                }
            }

           
            if (Lab_Type.Text.IndexOf("189") == -1)
            {
                sql = " SELECT * FROM SFISM4.R_DATA_INPUT_T WHERE MAC1 =  '" + varMAC + "'  ";
                var result2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result2.Data != null)
                {
                    s_ssn = result2.Data["ssn1"]?.ToString() ?? "";
                    if (!Menu_SSN.Checked)
                    {
                        Menu_SSN.Checked = true;
                        
                    }
                }

                if (!Menu_SN_SSN.Checked)
                {
                    if (foundmac == "Y")
                    {
                        sql = " INSERT INTO SFISM4.R_RW_DETAIL_T (SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE MAC =  '" + varMAC + "' ) ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return;
                        }

                        sql = "  UPDATE  SFISM4.R_RW_MAC_SSN_T SET  MO_NUMBER = '" + varMO + "', EMP_NO= '" + empNo + "',  ";
                        sql = sql + " IN_STATION_TIME = SYSDATE WHERE MAC = '" + varMAC + "' ";
                        var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if (b.Result != "OK")
                        {
                            ReportError(b.Message.ToString());
                            return;
                        }
                    }
                    else
                    {
                        if (Menu_SSN.Checked)
                        {
                            Menu_SSN.Checked = true;
                            Edit_SSN.Visible = true;
                            label4.Visible = true;
                            Edit_SSN.SelectAll();
                            Edit_SSN.Focus();
                        }
                        else
                        {
                            sql = "  INSERT INTO SFISM4.R_RW_MAC_SSN_T (MO_NUMBER, MAC, TYPE, EMP_NO) ";
                            sql = sql + " VALUES ('" + varMO + "', '" + varMAC + "' , '" + Lab_Model.Text + "', '" + empNo + "') ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return;
                            }

                            if(CK_DB == "ROKU")  // ROKU
                            {
                                if(Mmodel_name.Substring(0, 2) == "J2")
                                {
                                    await REWORK_BYSN(varMO, C_SpecialRoute, C_VersionCode, C_FirstGroup, varMAC);
                                }
                            }

                            if (T23SFLAG)
                            {
                                if (CheckB_SN_MAC.Checked)
                                {
                                    await MARKR107R108(S23SDATA, varMO);
                                }
                            }
                            else
                            {
                                if (CheckB_SN_MAC.Checked)
                                {
                                    await MARKR107R108(varMAC, varMO);
                                }
                            }
                        }

                    }
                }

                errorflag = false;

                if (!Menu_SN_SSN.Checked)
                {
                    await DEL_USED();

                    if ((Menu_SSN.Checked == false) && (Menu_SSN2.Checked == false))
                    {
                        Edit_MAC.SelectAll();
                        Edit_MAC.Focus();
                    }
                    else if ((Menu_SSN.Checked == true) && (Menu_SSN2.Checked == false))
                    {
                        
                        Edit_SSN.SelectAll();
                        Edit_SSN.Focus();

                        if (s_ssn != "")
                        {
                            Edit_SSN.Text = s_ssn;
                            await Load_SSN_Key();
                        }
                    }
                    else if ((Menu_SSN.Checked == false) && (Menu_SSN2.Checked == true))
                    {
                        Edit_SSN2.SelectAll();
                        Edit_SSN2.Focus();
                    }
                }
            }
            else
            {
                if (macid == "N/A")
                {
                    macid = ssn;
                }

                sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T (MO_NUMBER, MAC, SSN, MAC2, MAC3, MAC5, SSN3, SSN4, SSN5, TYPE, IN_STATION_TIME, EMP_NO) VALUES   ";
                sql = sql + " ('" + varMO + "', '" + macid + "' , '" + ssn + "',  '" + mac1 + "',  '" + mac2 + "',  '" + pincode + "', ";
                sql = sql + " '" + webkey + "', '" + ssid + "', '" + password + "', '" + Lab_Model.Text + "', SYSDATE , '" + empNo + "' ) ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }

                Edit_MAC.SelectAll();

                sql = " SELECT COUNT(*) COUNT_SN FROM  SFISM4.R_RW_MAC_SSN_T WHERE MO_NUMBER =  '" + varMO + "'  ";
                var result3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result3.Data != null)
                {
                    Lab_Qty_RW1.Text = result3.Data["count_sn"].ToString();
                }

                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                sql = " SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + macid + "' OR  SSN =  '" + macid + "'  ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() != 0)
                {
                    var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoResizeColumns();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }

            }
        }
        private async void CheckB_Range_Click(object sender, EventArgs e)
        {
            if(CK_DB == "ROKU")
            {
                CheckB_Range.Checked = true;
                showMessage("80137", "80137", false);
                return;
            }
            else
            {
                sql = string.Format("SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO =  '" + empNo + "'  AND  CLASS_NAME LIKE '%5%' ");

                var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data.Count() == 0)
                {
                    CheckB_Range.Checked = true;
                    showMessage("80138", "80138", false);
                    return;
                }
            }
        }

        private void Menu_SSN_Click(object sender, EventArgs e)
        {
            if (Menu_SSN.Checked)
            {
                Menu_SSN.Checked = true;
                Edit_SSN.Visible = true;
                label4.Visible = true;
            }
            else
            {
                Menu_SSN.Checked = false;
                Edit_SSN.Visible = false;
                label4.Visible = false;
            }
        }


        private async void Edit_SSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Edit_SN.Text == "UNDO" || Edit_MO.Text == "UNDO" || Edit_SSN.Text == "UNDO" || Edit_MAC.Text == "UNDO" || Edit_SSN2.Text == "UNDO")
                {
                    Edit_SN.Text = "";
                    Edit_MO.Text = "";
                    Edit_SSN.Text = "";
                    Edit_Length.Text = "";
                    Edit_MAC.Text = "";
                    Edit_SSN2.Text = "";
                    varMAC = "";
                    varMO = "";
                    varSN = "";
                    varSSN2 = "";
                    Edit_MO.Enabled = true;
                    Edit_MO.Focus();
                }
                else
                {
                    varSSN= Edit_SSN.Text;
                    await Load_SSN_Key();
                }
            }
        }

        private async Task Load_SSN_Key( )
        {
            String  model_name = "", macrange ="",  mo_number, tmp_str, tmpssn = "", tmp_model_name;
            String foundmac, MSERIAL_NUMBER, MODELFLAG, mod_type, sModel_type;

            Edit_SSN.Text = varSSN.Trim();

            if (varMO.Length == 0)
            {
                showMessage("Please input MO !", "Vui lòng nhập MO !", true);
                Edit_MO.Enabled = true;
                Edit_MO.Focus();
                return;
            }

            if (Menu_Upper.Checked == true)
            {
                Edit_SSN.Text = (varSSN).ToUpper().Trim();
            }
            else if (Menu_Lower.Checked == true)
            {
                Edit_SSN.Text = (varSSN).ToLower().Trim();
            }

            if (Edit_SSN.Text.IndexOf(",") != -1)
            {
                Edit_SSN.Text = varSSN.Substring(0, 13).Trim();
            }

            if (!await CHECK_CHARACTER_0_9A_Z(varSSN))
            {
                Edit_SSN.SelectAll();
                return;
            }

            if(CK_DB == "ROKU")
            {
                sql = " SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + varSSN + "' AND GD = 'BD2C' AND MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_SERIAL = 'NIC') ";
                result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_01.Data != null)
                {
                    showMessage("SN SAVED BD2C, CAN'T ADDMAC TO MO REWORK ! ", "SN đã R_SCRAP_T GD = BD2C, không thể thêm vào MO làm lại ! ", true);
                    Edit_SSN.SelectAll();
                    return;
                }
            }


            if (Menu_SN_SSN.Checked)
            {
                if ((varSSN == Edit_MAC.Text) || (varSSN == Edit_SN.Text))
                {
                    showMessage("80139", "80139", false);
                    Edit_SSN.SelectAll();
                    return;
                }

                if (Edit_SSN.Text == "")
                {
                    showMessage("80140", "80140", false);
                    Edit_SSN.SelectAll();
                    return;
                }

                if (CheckB_RMA.Checked)
                {
                    sql = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '" + varSSN + "' OR SSN3 = '" + varSSN + "' ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        if (((result_01.Data["ssn3"]?.ToString() ?? "") != Edit_SSN.Text) || ((result_01.Data["serial_number"]?.ToString() ?? "") != Edit_SN.Text))
                        {
                            showMessage("80141", "80141", false);
                            Edit_SSN.SelectAll();
                            return;
                        }
                    }
                    else
                    {
                        sql = " SELECT * FROM SFISM4.H_CUSTSN_T WHERE SERIAL_NUMBER = '" + varSSN + "' OR SSN3 = '" + varSSN + "' ";
                        result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_01.Data != null)
                        {
                            if (((result_01.Data["ssn3"]?.ToString() ?? "") != varSSN) || ((result_01.Data["serial_number"]?.ToString() ?? "") != varSSN))
                            {
                                showMessage("80142", "80142", false);
                                Edit_SSN.SelectAll();
                                return;
                            }
                        }
                        else
                        {
                            sql = " SELECT A.MO_NUMBER FROM SFISM4.R_MO_EXT3_T A WHERE  '" + varSSN + "'  BETWEEN  A.ITEM_1  AND  A.ITEM_2   ";
                            sql = sql + " AND LENGTH( '" + varSSN + "' )=LENGTH(A.ITEM_1)  AND A.MO_NUMBER= '" + Lab_OLD_MO.Text + "'  ";
                            sql = sql + " UNION ALL   ";
                            sql = sql + " SELECT B.MO_NUMBER FROM SFISM4.H_MO_EXT4_T B WHERE  '" + varSSN + "'  BETWEEN  B.ITEM_1  AND  B.ITEM_2  ";
                            sql = sql + " AND  LENGTH( '" + varSSN + "' )=LENGTH(B.ITEM_1)  AND B.MO_NUMBER= '" + Lab_OLD_MO.Text + "'  ";
                            sql = sql + " UNION ALL  ";
                            sql = sql + " SELECT C.MO_NUMBER FROM SFISM4.R_RW_MAC_SSN_T C WHERE SSN= '" + varSSN + "'  ";
                            sql = sql + " AND SSN2 = '" + varSSN + "' AND C.MO_NUMBER= '" + Lab_OLD_MO.Text + "'  ";
                            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_01.Data == null)
                            {
                                showMessage("80143", "80143", false);
                                Edit_MAC.SelectAll();
                                return;
                            }
                        }
                    }

                    sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SHIPPING_SN = '" + varSSN + "'  ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        showMessage("80144", "80144", false);
                        Edit_SSN.SelectAll();
                        return;
                    }
                }

                foundmac = "N";

                sql = " SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T WHERE SSN = '" + varSSN + "' OR MAC = '" + Edit_MAC.Text + "' OR SSN2 = '" + varSSN + "'  ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() == 0)
                {
                    if (result_list1.Data.Count() > 1)
                    {
                        showMessage("80145", "80145", false);
                        Edit_SSN.SelectAll();
                        return;
                    }
                    else
                    {
                        if ((result_01.Data["ssn"].ToString() != Edit_SN.Text) &&
                            (result_01.Data["mac"].ToString() != Edit_MAC.Text) &&
                            (result_01.Data["ssn2"].ToString() != varSSN))
                        {
                            if (result_01.Data["mo_number"].ToString() != varMO)
                            {
                                DialogResult result_9 = MessageBox.Show(" SN or MAC already accede to rework mo: " + result_01.Data["mo_number"].ToString() + " , whether need change at present rework MO ", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                                if (result_9 == DialogResult.OK)
                                {
                                    foundmac = "Y";
                                }
                                else
                                {
                                    Edit_SN.SelectAll();
                                    Edit_SN.Focus();
                                    return;
                                }
                            }
                            else
                            {
                                showMessage("80146", "80146", false);
                                Edit_SN.SelectAll();
                                Edit_SN.Focus();
                                return;
                            }
                        }
                        else
                        {
                            showMessage("80147", "80147", false);
                            Edit_SSN.SelectAll();
                            return;
                        }
                    }
                }

                mod_type = "";

                sql = " SELECT B.MODEL_TYPE FROM SFISM4.R_MO_BASE_T A ,SFIS1.C_MODEL_DESC_T B   ";
                sql = sql + " WHERE A.MODEL_NAME = B.MODEL_NAME AND A.MO_NUMBER ='" + varMO + "'  ";
                result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_01.Data != null)
                {
                    mod_type = result_01.Data["mod_type"]?.ToString() ?? "";
                }

                if (!await Update_Table(varSSN, varMO))
                {
                    Edit_SN.SelectAll();
                    Edit_SN.Focus();
                    return;
                }

                if (CK_DB == "NIC")
                {
                    // Update updRepairInOut
                    sql = " UPDATE SFISM4.R_REPAIR_IN_OUT_T SET SERIAL_NUMBER = SERIAL_NUMBER|| '"+ varMO + "'  ";
                    sql = sql + " WHERE SERIAL_NUMBER= '" + Edit_SN.Text + "'  AND MO_NUMBER <> '" + varMO + "' ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        Edit_SN.SelectAll();
                        Edit_SN.Focus();
                        return;
                    }
                }


                if ((CheckB_RMA.Checked) && (mod_type.IndexOf("45") != -1))
                {
                    sql = " SELECT *FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER ='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL   ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        // Update HCustomer
                        sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER||'" + varMO + "',   ";
                        sql = sql + " MACID=MACID||'" + varMO + "', SHIPPING_SN=SHIPPING_SN||'" + varMO + "'  WHERE SHIPPING_SN= '" + Edit_SN.Text + "'   ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return;
                        }
                    }

                    sql = " SELECT *FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER ='" + Edit_SN.Text + "' AND SHIPPING_SN IS NULL   ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        // Update HCustomer1
                        sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER|| '" + varMO + "',   ";
                        sql = sql + " MACID=MACID|| '" + varMO + "' WHERE SERIAL_NUMBER= '" + Edit_SN.Text + "'  ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return;
                        }
                    }
                }
                else if ((CheckB_RMA.Checked) && (mod_type.IndexOf("75") != -1))
                {
                    sql = " SELECT *FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER ='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL   ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        // Update HCustomer
                        sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER||'" + varMO + "',   ";
                        sql = sql + " MACID=MACID||'" + varMO + "', SHIPPING_SN=SHIPPING_SN||'" + varMO + "'  WHERE SHIPPING_SN= '" + Edit_SN.Text + "'   ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return;
                        }
                    }

                    sql = " SELECT *FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER ='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL   ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        // Update HCustomer2
                        sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER|| '" + varMO + "',    ";
                        sql = sql + " SHIPPING_SN=SHIPPING_SN|| '" + varMO + "' WHERE SHIPPING_SN= '" + varSSN + "'  ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return;
                        }
                    }
                }
                else if (CheckB_RMA.Checked)
                {
                    // Update HCustomer
                    sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER||'" + varMO + "',   ";
                    sql = sql + " MACID=MACID||'" + varMO + "', SHIPPING_SN=SHIPPING_SN||'" + varMO + "'  WHERE SHIPPING_SN= '" + Edit_SN.Text + "'   ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        Edit_SN.SelectAll();
                        Edit_SN.Focus();
                        return;
                    }
                }

                if (foundmac == "Y")
                {
                    // Insert Rw_Detail
                    sql = " INSERT INTO SFISM4.R_RW_DETAIL_T (SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE SSN= '" + Edit_SN.Text + "' AND MAC= '" + Edit_MAC.Text + "' )   ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        Edit_SN.SelectAll();
                        Edit_SN.Focus();
                        return;
                    }

                    // Update Rw_SSN_Mac
                    sql = " UPDATE SFISM4.R_RW_MAC_SSN_T SET MO_NUMBER = '" + varMO + "' ,TYPE= '" + Lab_Model.Text + "',  ";
                    sql = sql + " EMP_NO= '" + empNo + "' WHERE SSN = '" + Edit_SN.Text + "' AND MAC= '" + Edit_MAC.Text + "' ";
                    a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        Edit_SN.SelectAll();
                        Edit_SN.Focus();
                        return;
                    }
                }
                else
                {
                    // InsertRwSSN3  (accede Exist)
                    sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T ( MO_NUMBER,SN,SSN,MAC,SSN2,TYPE ,EMP_NO )    ";
                    sql = sql + " VALUES ( '" + varMO + "' , '" + Edit_SN.Text + "', '" + Edit_SN.Text + "' , '" + Edit_MAC.Text + "'  , '" + varSSN + "' , '" + Lab_Model.Text + "'  , '" + empNo + "' ) ";
                    var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (a.Result != "OK")
                    {
                        ReportError(a.Message.ToString());
                        Edit_SN.SelectAll();
                        Edit_SN.Focus();
                        return;
                    }
                }
                Edit_SN.SelectAll();
                Edit_SN.Focus();
                return;
            }

            sql = " SELECT MODEL_TYPE FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME = '" + Lab_Model.Text + "'   ";
            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_01.Data != null)
            {
                sModel_type = result_01.Data["model_type"]?.ToString() ?? "";
            }
            else
            {
                showMessage("80271", "80271", false);
                return;
            }

            if (sModel_type.IndexOf("027") != -1) 
            {
                if (CheckB_Range.Checked)
                {
                    foundmac = "N";
                    macrange = "F";

                    sql = " SELECT * FROM SFISM4.R_MO_EXT3_T WHERE  '" + varSSN + "'  ";
                    sql = sql + " BETWEEN  ITEM_1  AND  ITEM_2  AND LENGTH(ITEM_1) = LENGTH('" + varSSN + "')  ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data == null)
                    {
                        sql = " SELECT * FROM SFISM4.H_MO_EXT3_T WHERE  '" + Edit_SSN.Text + "'  ";
                        sql = sql + " BETWEEN  ITEM_1  AND  ITEM_2  AND LENGTH(ITEM_1) = LENGTH('" + varSSN + "')  ";
                        result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_01.Data == null)
                        {
                            sql = " SELECT * FROM SFISM4.H_MO_EXT3_T@SFCODBH WHERE  '" + varSSN + "'  ";
                            sql = sql + " BETWEEN  ITEM_1  AND  ITEM_2  AND LENGTH(ITEM_1) = LENGTH('" + varSSN + "')  ";
                            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_01.Data == null)
                            {
                                sql = "  SELECT * FROM  SFISM4.CSM_BOOKED  WHERE '" + Edit_SSN.Text + "'   ";
                                sql = sql + " BETWEEN   CSN_BEGIN  AND CSN_END AND LENGTH(CSN_END)=LENGTH('" + varSSN + "')  ";
                                result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                                if (result_01.Data != null)
                                {
                                    if (result_01.Data["model_name"].ToString() != "")
                                    {
                                        model_name = result_01.Data["model_name"].ToString();

                                        if (!Menu_Change_Mod.Checked)
                                        {
                                            if (Lab_Model.Text != model_name)
                                            {
                                                if (!CheckB_Mac_Range.Checked)
                                                {
                                                    showMessage("80148", "80148", false);
                                                    return;
                                                }
                                                else
                                                {
                                                    showMessage("80132", "80132", false);
                                                    return;
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        showMessage("80149", "80149", false);
                                        return;
                                    }
                                }
                                else
                                {
                                    showMessage(" " + Edit_SSN.Text + " - DATA NOT IN SFISM4.CSM_BOOKED !", "" + Edit_SSN.Text + " - Dữ liệu không tồn tại trong SFISM4.CSM_BOOKED !", true);
                                    return;
                                }
                            }
                            else
                            {
                                model_name = result_01.Data["ver_4"]?.ToString() ?? "";
                            }
                        }
                    }
                    else
                    {
                        model_name = result_01.Data["ver_4"]?.ToString() ?? "";
                        mo_number = result_01.Data["mo_number"].ToString();


                        sql = " SELECT * FROM SFISM4.R_MO_BASE_T WHERE  MO_NUMBER=  '" + mo_number + "'  ";
                        result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        if ((result_01.Data["close_flag"]?.ToString()?? "") == "2")
                        {
                            showMessage("80129", "80129", false);
                            return;
                        }
                    }

                    if (!Menu_Change_Mod.Checked)
                    {
                        if (Lab_Model.Text != model_name)
                        {
                            if (!CheckB_Mac_Range.Checked)
                            {
                                showMessage("80148", "80148", false);
                                return;
                            }
                            else
                            {
                                showMessage("80132", "80132", false);
                                return;
                            }
                        }
                    }
                }

                if (CK_DB == "ROKU")  //ROKU
                {
                    if (await Check_History())
                    {
                        sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SHIPPING_SN = '"+ Edit_MAC.Text + "'   ";
                    }
                    else
                    {
                        sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T WHERE SHIPPING_SN = '" + Edit_MAC.Text + "' ";
                    }
                }
                else
                {
                    sql = " SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE   SHIPPING_SN =  '" + Edit_MAC.Text + "'    ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        showMessage(" " + Edit_MAC.Text + " - " + Edit_SSN.Text + " - SSN P(H)_TMP_CUSTOMER_T reelect, Can not use !", " " + Edit_MAC.Text + " - " + Edit_SSN.Text + " - SSN P(H)_TMP_CUSTOMER_T đã tồn tại, Không thê dử dụng !", true);
                        return;
                    }
                }
            }

            sql = " SELECT  *  FROM SFISM4.R_WIP_TRACKING_T WHERE   SHIPPING_SN =  '" + varSSN + "'  ";
            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_01.Data == null)
            {
                sql = " SELECT  *  FROM SFISM4.H_WIP_TRACKING_T WHERE   SHIPPING_SN =  '" + varSSN + "'  ";
                result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_01.Data != null)
                {
                    MSERIAL_NUMBER = result_01.Data["serial_number"].ToString();

                    sql = " SELECT * FROM SFISM4.H_WIP_TRACKING_T WHERE SERIAL_NUMBER =  '" + MSERIAL_NUMBER + "'  ";
                    result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_01.Data != null)
                    {
                        if ((result_01.Data["scrap_flag"]?.ToString() ?? "") == "0")
                        {
                            showMessage(" " + Edit_MAC.Text + " - " + varSSN + " - SSN H_WIP_TRACKING_T reelect, Can not use !", "" + Edit_MAC.Text + " - " + Edit_SSN.Text + " - SSN H_WIP_TRACKING_T đã tồn tại, Không thê dử dụng !", true);
                            return;
                        }
                    }
                }
            }
            else
            {
                tmp_str = result_01.Data["serial_number"].ToString();

                sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER =  '" + tmp_str + "'  ";
                result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_01.Data != null)
                {
                    tmp_model_name = result_01.Data["model_name"].ToString();

                    if ((result_01.Data["scrap_flag"]?.ToString() ?? "") != "0")
                    {
                        sql = " UPDATE   SFISM4.R_WIP_TRACKING_T  SET PO_NO  =  PO_NO||'RW',    ";
                        sql = sql + " SHIPPING_SN = 'RW'||'" + Edit_SSN.Text + "' WHERE   SERIAL_NUMBER =   '" + tmp_str + "'  ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return;
                        }

                        sql = " SELECT * FROM SFIS1.C_MODEL_DESC_T  WHERE  MODEL_NAME =  '" + tmp_model_name + "'  ";
                        result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                        MODELFLAG = result_01.Data["model_name"]?.ToString() ?? "";

                        sql = " SELECT  *  FROM SFISM4.R_CUSTSN_T   WHERE   SERIAL_NUMBER =  '" + tmp_str + "'  ";
                        result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_01.Data != null)
                        {
                            sql = " DELETE  SFISM4.R_CUSTSN_T WHERE   SERIAL_NUMBER =   '" + tmp_str + "'   ";
                            a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return;
                            }
                        }
                    }
                    else
                    {
                        showMessage(" " + Edit_MAC.Text + " - " + Edit_SSN.Text + " - SSN R_WIP_TRACKING_T SHIPPING_SN reelect, Can not use !", "" + Edit_MAC.Text + " - " + Edit_SSN.Text + " - SSN R_WIP_TRACKING_T  SHIPPING_SN đã tồn tại, Không thê dử dụng !", true);
                        return;
                    }
                }
            }

            sql = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE  SSN1 =  '" + Edit_SSN.Text + "'   ";
            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_01.Data != null)
            {
                tmpssn = result_01.Data["serial_number"].ToString();
            }

            sql = " SELECT  * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER = '" + tmpssn + "' AND SCRAP_FLAG <> 0  ";
            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_01.Data != null)
            {
                sql = " DELETE SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER = '" + tmpssn + "'  ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            }

            foundmac = "N";

            sql = " SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE SSN = '" + Edit_SSN.Text + "' ";
            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_01.Data != null)
            {
                if (result_01.Data["mo_number"].ToString() != varMO)
                {
                    DialogResult result_9 = MessageBox.Show(" SSN R_RW_MAC_SSN_T in already accede to " + result_01.Data["mo_number"].ToString() + " , whether need change at present rework MO ", "Message", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (result_9 == DialogResult.OK)
                    {
                        foundmac = "Y";
                        Edit_MAC.SelectAll();
                    }
                }
                else
                {
                    showMessage(" " + Edit_SSN.Text + " - SSN R_RW_MAC_SSN_T in already accede to !", " " + Edit_SSN.Text + " - SSN Đã tồn tại trong R_RW_MAC_SSN_T !", true);
                    List_ErrorSN.Items.Add("" + Edit_SSN.Text + "   Exist");
                    Edit_MAC.SelectAll();
                    return;
                }
            }

            sql = " SELECT * FROM SFISM4.Z_WIP_TRACKING_T WHERE SHIPPING_SN = '" + Edit_SSN.Text + "' ";
            result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_01.Data != null)
            {
                showMessage(" " + Edit_SSN.Text + " - Z_WIP_TRACKING_T SHIPPING_SN already exist, already ship, Can not use !", " " + Edit_SSN.Text + " - Z_WIP_TRACKING_T đã tồn tại, Không thể sử dụng !", true);
                return;
            }

            sql = " SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE MO_NUMBER = '" + varMO + "'  ";
            var result_list2 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list2.Data.Count() >= (Int32.Parse(Lab_Qty_MO.Text) + 20))
            {
                showMessage("80136", "80136", false);
                return;
            }

            if (foundmac == "Y")
            {
                sql = " UPDATE SFISM4.R_RW_MAC_SSN_T SET  MO_NUMBER  ='" + varMO + "' WHERE   SSN = '" + Edit_SSN.Text + "'  ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
            }
            else
            {
                sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T ( MO_NUMBER, SSN ,MAC, TYPE, EMP_NO) VALUES   ";
                sql = sql + " ('" + varMO + "', '" + Edit_SSN.Text + "', '" + Edit_MAC.Text + "' , '" + Lab_Model.Text + "', '" + empNo + "') ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }

                sql = " SELECT COUNT(*) COUNT_SN FROM  SFISM4.R_RW_MAC_SSN_T WHERE MO_NUMBER =  '" + varMO + "'  ";
                var result3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result3.Data != null)
                {
                    Lab_Qty_RW1.Text = result3.Data["count_sn"].ToString();
                }

                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                sql = " SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + Edit_SSN.Text + "' OR  SSN =  '" + Edit_SSN.Text + "'  ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() != 0)
                {
                    var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoResizeColumns();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }
            }

            if (CheckB_SN_MAC.Checked)
            {
                await MARKR107R108(Edit_MAC.Text, varMO);
            }

            if (Menu_SSN2.Checked)
            {
                Menu_SSN2.Checked = false;
                Edit_SSN2.Visible = false;
                label8.Visible = false;
                Edit_SSN2.SelectAll();
                Edit_SSN2.Focus();
            }
            else
            {
                Edit_MAC.SelectAll();
                Edit_MAC.Focus();
            }
        }

        private async void CheckB_SSN_Range_Click(object sender, EventArgs e)
        {
            sql = string.Format("SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO =  '" + empNo + "'  AND  CLASS_NAME LIKE '%5%' ");
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data.Count() == 0)
            {
                CheckB_SSN_Range.Checked = true;
                showMessage("80138", "80138", false);
                return;
            }
        }

        private void Menu_SSN2_Click(object sender, EventArgs e) 
        {
            if (Menu_SSN2.Checked)
            {
                Menu_SSN2.Checked = true;
                Edit_SSN2.Visible = true;
                label8.Visible = true;
                Edit_SSN2.Focus();
            }
            else
            {
                Menu_SSN2.Checked = false;
                Edit_SSN2.Visible = false;
                label8.Visible = false;
            }

        }
        private void Menu_10_Click(object sender, EventArgs e)
        {
            if (Menu_10.Checked)
            {
                Menu_10.Checked = true;
                Menu_12.Checked = false;
                Menu_19.Checked = false;
                Menu_16.Checked = false;
            }
            else
            {
                Menu_10.Checked = false;
                Menu_12.Checked = false;
                Menu_19.Checked = false;
                Menu_16.Checked = false;
            }

        }
        private void Menu_12_Click(object sender, EventArgs e)
        {
            if (Menu_12.Checked)
            {
                Menu_12.Checked = true;
                Menu_10.Checked = false;
                Menu_19.Checked = false;
                Menu_16.Checked = false;
            }
            else
            {
                Menu_12.Checked = false;
                Menu_10.Checked = false;
                Menu_19.Checked = false;
                Menu_16.Checked = false;
            }
        }

        private async Task DEL_USED()
        {
            string Temp_SN, S_Update = "";

            sql = " SELECT * FROM SFISM4.R_USED_KEYPARTS_T WHERE KEY_PART_SN = '" + Edit_MAC.Text + "' ";
            var result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_01.Data != null)
            {
                Temp_SN = result_01.Data["serial_number"].ToString();
            }
            else
            {
                return;
            }

            sql = (" SELECT * FROM SFISM4.R_USED_KEYPARTS_T WHERE SERIAL_NUMBER = '" + Temp_SN + "' AND KEY_PART_FLAG NOT IN('SN', 'MAC1')");
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data.Count() == 0)
            {
                string final = "";
                foreach (var row in result_list.Data)
                {
                    S_Update = S_Update + row["key_part_flag"].ToString() + "=" + row["key_part_sn"].ToString() + ",";
                    final = row["serial_number"].ToString();
                }

                S_Update = S_Update + " SN = "+ final;
                S_Update = S_Update.Replace("SSN1", "SSN");

                sql = " UPDATE SFISM4.R_RW_MAC_SSN_T SET "+ S_Update + " WHERE MAC = '"+Edit_MAC.Text+"'  ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
            }

            sql = " DELETE SFISM4.R_USED_KEYPARTS_T WHERE SERIAL_NUMBER = '" + Temp_SN + "' AND KEY_PART_FLAG<> 'SN' ";
            await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
        }

        private async void Edit_SN_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) && (Edit_SN.Text.Length != 0))
            {
                if(Edit_SN.Text == "UNDO" || Edit_MO.Text == "UNDO"  || Edit_SSN.Text == "UNDO" || Edit_MAC.Text == "UNDO" || Edit_SSN2.Text == "UNDO")
                {
                    Edit_SN.Text = "";
                    Edit_Length.Text = "";
                    Edit_MO.Text = "";
                    Edit_SSN.Text = ""; 
                    Edit_MAC.Text = ""; 
                    Edit_SSN2.Text = "";
                    varMAC = "";
                    varMO = "";
                    varSN = "";
                    varSSN2 = "";
                    Edit_MO.Enabled = true;
                    Edit_MO.Focus();
                }
                else
                {
                   varSN = Edit_SN.Text;
                    await Load_SN_Key();
                }
                
            }
        }
        private async Task Load_SN_Key()
        {
            string mo_number;
            string model_name = "";

            errorflag = true;

            Edit_SN.Text = (varSN).Trim();

            if (Edit_MO.Text.Length == 0)
            {
                showMessage("Please input MO !", "Vui lòng nhập MO !", true);
                Edit_MO.Enabled = true;
                Edit_MO.Focus();
                return;
            }

            if (Menu_Upper.Checked == true)
            {
                Edit_SN.Text = (varSN).ToUpper().Trim();
            }

            Lab_OLD_MO.Text = "oldmo";

            if (!await CHECK_CHARACTER_0_9A_Z(varSN))
            {
                Edit_SN.SelectAll();
                return;
            }

            sql = " SELECT B.MO_NUMBER FROM SFISM4.R_MO_EXT3_T A,SFISM4.R_MO_BASE_T B WHERE A.MO_NUMBER=B.MO_NUMBER AND B.CLOSE_FLAG='2'  ";
            sql = sql + " AND '" + varSN + "'  BETWEEN  A.ITEM_1  AND  A.ITEM_2 AND B.MO_NUMBER<> '" + Edit_MO.Text + "' AND LENGTH( '" + varSN + "' )=LENGTH(ITEM_1) ";
            sql = sql + " UNION ALL  ";
            sql = sql + " SELECT D.MO_NUMBER FROM SFISM4.R_RW_MAC_SSN_T C,SFISM4.R_MO_BASE_T D  ";
            sql = sql + " WHERE C.MO_NUMBER=D.MO_NUMBER AND D.CLOSE_FLAG='2' AND D.MO_NUMBER<> '" + Edit_MO.Text + "' AND C.SSN = '" + varSN + "'  ";
            var result_4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_4.Data != null)
            {
                mo_number = result_4.Data["mo_number"].ToString();
                showMessage("Former MO  " + mo_number + "  not close, Can not add rework MO ! ", "Công lệnh " + mo_number + " trước chưa đóng, không thể thêm MO làm lại ! ", true);
                Edit_SN.SelectAll();
                return;
            }

            sql = " SELECT * FROM SFIS1.C_CUSTSN_RULE_T WHERE  MODEL_NAME =  '" + Lab_Model.Text + "' AND  CUSTSN_CODE = 'SSN1'  AND  COMPARE_SN='SN' ";
            var result_5 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_5.Data == null)
            {
                showMessage("80150", "80150", false);
                Edit_SN.SelectAll();
                return;
            }

            sql = " SELECT COUNT(*) SN_COUNT FROM SFISM4.R_RW_MAC_SSN_T  WHERE   MO_NUMBER =  '" + Edit_MO.Text + "' ";
            var result_6 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (Int32.Parse(result_6.Data["sn_count"].ToString()) >= Int32.Parse(Lab_Qty_MO.Text))
            {
                showMessage("80125", "80125", false);
                Edit_SN.Text = "";
                Edit_SN.SelectAll();
                return;
            }

            if (CheckB_RMA.Checked == false)
            {
                sql = " SELECT A.MO_NUMBER,VER_4 FROM SFISM4.R_MO_EXT3_T A  ";
                sql = sql + " WHERE  '" + varSN + "'  BETWEEN  A.ITEM_1  AND  A.ITEM_2  AND LENGTH( '" + varSN + "' )=LENGTH(A.ITEM_1) ";
                sql = sql + " UNION ALL  ";
                sql = sql + " SELECT B.MO_NUMBER,VER_4 FROM SFISM4.H_MO_EXT3_T B  ";
                sql = sql + " WHERE  '" + varSN + "'  BETWEEN  B.ITEM_1  AND  B.ITEM_2   AND LENGTH( '" + varSN + "' )=LENGTH(B.ITEM_1) ";
                var result_7 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_7.Data == null)
                {
                    showMessage("80151", "80151", false);
                    Edit_SN.SelectAll();
                    return;
                }
                else
                {
                    model_name = result_7.Data["ver_4"]?.ToString() ?? "";
                }


                if (!Menu_Change_Mod.Checked)
                {
                    if (Lab_Model.Text != model_name)
                    {
                        if (!Menu_Change_Mod.Checked)
                        {
                            showMessage("80152", "80152", false);
                            Edit_SN.SelectAll();
                            return;
                        }
                    }
                }

                sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SHIPPING_SN= '" + varSN + "' ";
                var result_8 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_8.Data != null)
                {
                    showMessage("80153", "80153", false);
                    Edit_SN.SelectAll();
                    return;
                }

                sql = " SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER= '" + varSN + "' ";
                var result_9 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_9.Data != null)
                {
                    if ((result_9.Data["scrap_flag"]?.ToString() ?? "") == "0")
                    {
                        showMessage("80154", "80154", false);
                        Edit_SN.SelectAll();
                        return;
                    }
                    else
                    {
                        sql = " SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + varSN + "' AND GD = 'BD2C' AND MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_SERIAL = 'NIC') ";
                        var result_01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result_01.Data != null)
                        {
                            showMessage("SN SAVED BD2C, CAN'T ADDMAC TO MO REWORK ! ", "SN đã R_SCRAP_T GD = BD2C, không thể thêm vào MO làm lại ! ", true);
                            Edit_SN.SelectAll();
                            return;
                        }
                    }
                }
                else
                {
                    sql = " SELECT * FROM SFISM4.H_WIP_TRACKING_T WHERE SERIAL_NUMBER= '" + varSN + "' ";
                    var result_02 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_02.Data != null)
                    {
                        if ((result_02.Data["scrap_flag"]?.ToString() ?? "") == "0")
                        {
                            showMessage("80154", "80154", false);
                            Edit_SN.SelectAll();
                            return;
                        }
                        else
                        {
                            sql = " SELECT * FROM SFISM4.R_SCRAP_T WHERE SERIAL_NUMBER = '" + varSN + "' AND GD = 'BD2C' AND MODEL_NAME IN (SELECT MODEL_NAME FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_SERIAL = 'NIC') ";
                            var result_03 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_03.Data != null)
                            {
                                showMessage("SN SAVED BD2C, CAN'T ADDMAC TO MO REWORK ! ", "SN đã R_SCRAP_T GD = BD2C, không thể thêm vào MO làm lại ! ", true);
                                Edit_SN.SelectAll();
                                return;
                            }

                            Lab_OLD_MO.Text = result_02.Data["mo_number"].ToString();

                        }

                    }
                    else
                    {
                        showMessage("80155", "80155", false);
                        Edit_SN.SelectAll();
                        return;
                    }
                }
            }

            if (CheckB_Furu.Checked)
            {
                if (!await FIND_MAC(varSN))
                {
                    showMessage("80156", "80156", false);
                    Edit_MAC.Focus();
                    Edit_MAC.SelectAll();
                    return;
                }
                else
                {
                    Edit_MAC.Enabled = false;
                    await Load_Mac_Key();
                    errorflag = false;
                }
            }
            errorflag = false;
            Edit_MAC.SelectAll();
            Edit_MAC.Focus();

        }

        private void Menu_SN_SSN_Click(object sender, EventArgs e)
        {
            if (Menu_SN_SSN.Checked)
            {
                Menu_SSN.Checked = false;
                Menu_SSN.Enabled = false;
                Menu_SSN2.Checked = false;
                Menu_SSN2.Enabled = false;
                CheckB_Range.Visible = false;
                CheckB_Mac_Range.Visible = false;
                CheckB_SSN_Range.Visible = false;
                Edit_SSN.Visible = false;
                Edit_SSN2.Visible = false;
                label8.Visible = false;
                label4.Visible = false;
                label3.Visible = true;
                Edit_SN.Visible = true;
                Edit_MAC.Visible = true;
                Edit_MAC.Enabled = true;
                Menu_SN_SSN.Checked = true;
                CheckB_Furu.Visible = true;
                CheckB_Home.Visible = true;
            }
            else 
            {
                Menu_SSN.Enabled = true;
                Menu_SSN2.Enabled = true;
                CheckB_Range.Visible = true;
                CheckB_Mac_Range.Visible = true;
                CheckB_SSN_Range.Visible = true;
                label3.Visible = false;
                Edit_SN.Visible = false;
                CheckB_Furu.Visible = false;
                CheckB_Home.Visible = false;
            }
        }
        private void Menu_Change_Mod_Click(object sender, EventArgs e)
        {
            if (Menu_Change_Mod.Checked)
            {
                Menu_Change_Mod.Checked = true;
            }
            else
            {
                Menu_Change_Mod.Checked = false;
            }

        }
        private void Menu_Upper_CheckedChanged(object sender, EventArgs e)
        {
            if (Menu_Upper.Checked)
            {
                Menu_Upper.Checked = true;
                Menu_Lower.Checked = false;
            }
            else
            {
                Menu_Upper.Checked = false;
            }
        }
        private void Menu_Lower_CheckedChanged(object sender, EventArgs e)
        {
            if (Menu_Lower.Checked)
            {
                Menu_Lower.Checked = true;
                Menu_Upper.Checked = false;
            }
            else
            {
                Menu_Lower.Checked = false;
            }
        }
        private async Task<bool> FIND_MAC(String sn)
        {
            string t_mac;

            sql = " SELECT DISTINCT KEY_PART_SN FROM ( ";
            sql = sql + " SELECT KEY_PART_SN FROM SFISM4.R108 WHERE SERIAL_NUMBER = '" + Edit_SN.Text + "' AND KEY_PART_NO='MACID' ";
            sql = sql + " UNION ";
            sql = sql + " SELECT KEY_PART_SN FROM SFISM4.H108 WHERE SERIAL_NUMBER = '" + Edit_SN.Text + "' AND KEY_PART_NO='MACID' ) ";
            var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result1.Data == null)
            {
                return false;
            }

            t_mac = result1.Data["key_part_sn"].ToString();
            Edit_MAC.Text = t_mac;
            return true;
        }

        private async void Menu_DOA_Click(object sender, EventArgs e)
        {
            sql = " SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP = '" + empNo + "' AND FUN = 'VIEW' AND PRG_NAME = 'DOA' ";
            var result = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data.Count() != 0)
            {
                DOA_Form DOA_Form = new DOA_Form(_sfcHttpClient);
                DOA_Form._sfcHttpClient = _sfcHttpClient;
                DOA_Form.empNo = empNo;
                DOA_Form.CK_DB = CK_DB;
                DOA_Form.Show();
                this.Hide();
            }
            else
            {
                showMessage("You do not have access !", "Bạn không có quyền truy cập ! \nFUN = 'VIEW' AND PRG_NAME = 'DOA'", true);
                Menu_DOA.Checked = false;
                return;
            }
        }

        private async Task<bool> MARK_HISTORY_DATA(string SN, string MAC, string SSN)
        {
            string Q_R107 = "", Update_R107 = "", Q_Z107 = "";
            string Q_CUST = "", Update_CUST = "", Q_R108 = "", Update_R108 = "", Q_H108 = "";

            sql = " SELECT TO_CHAR(SYSDATE,'YYDDMM') TIME_T FROM DUAL ";
            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            String Doa_Time = "DOA" + result_1.Data["time_t"].ToString();

            if (Edit_MAC.Text != "")
            {
                Q_R107 = " SELECT * FROM SFISM4.R107 WHERE SHIPPING_SN2 = '" + MAC + "' ";
                Update_R107 = " SHIPPING_SN2 = '" + MAC + "' ";
                Q_Z107 = " SELECT * FROM SFISM4.Z107 WHERE SHIPPING_SN2 = '" + MAC + "' ";

                if (Edit_MAC.Text.Substring(0, 3) == "23S")
                {
                    Q_CUST = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE MAC1 LIKE '%" + MAC + "%' ";
                    Update_CUST = " MAC1 LIKE '%" + MAC + "%' ";
                }
                Q_CUST = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE MAC1 = '" + MAC + "' ";
                Update_CUST = " MAC1 = '" + MAC + "' ";
                Q_R108 = " SELECT * FROM SFISM4.R108 WHERE KEY_PART_SN = '" + MAC + "' ";
                Update_R108 = "  KEY_PART_SN = '" + MAC + "' ";
                Q_H108 = " SELECT * FROM SFISM4.H108 WHERE KEY_PART_SN = '" + MAC + "' ";
            }
            else if (Edit_SSN.Text != "")
            {
                Q_R107 = " SELECT * FROM SFISM4.R107 WHERE SHIPPING_SN = '" + SSN + "' ";
                Update_R107 = " SHIPPING_SN = '" + SSN + "' ";
                Q_Z107 = " SELECT * FROM SFISM4.Z107 WHERE SHIPPING_SN = '" + SSN + "' ";
                Q_CUST = " SELECT * FROM SFISM4.R_CUSTSN_T WHERE SSN1 = '" + SSN + "' ";
                Update_CUST = " SSN1 = '" + SSN + "' ";
            }

            try
            {
                if (Q_R107 != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_R107, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        sql = " UPDATE SFISM4.R107 SET SHIPPING_SN2 = SHIPPING_SN2||'" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN||'" + Doa_Time + "' WHERE '" + Update_R107 + "'  ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return false;
                        }
                    }
                }

                if (Q_Z107 != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_Z107, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        sql = " UPDATE SFISM4.Z107 SET SHIPPING_SN2 = SHIPPING_SN2||'" + Doa_Time + "', SHIPPING_SN = SHIPPING_SN||'" + Doa_Time + "' WHERE '" + Update_R107 + "' ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return false;
                        }
                    }
                }

                if (Q_CUST != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_CUST, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        sql = " UPDATE SFISM4.R_CUSTSN_T SET SSN1 = SSN1||'" + Doa_Time + "', SN2 = SSN2||'" + Doa_Time + "', MAC1 = MAC1||'" + Doa_Time + "' WHERE '" + Update_CUST + "' ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return false;
                        }
                    }
                }

                if (Q_R108 != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_R108, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        sql = " UPDATE SFISM4.R108 SET KEY_PART_SN = KEY_PART_SN||'" + Doa_Time + "' WHERE '" + Update_R108 + "' ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return false;
                        }
                    }
                }

                if (Q_H108 != "")
                {
                    var result1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = Q_H108, SfcCommandType = SfcCommandType.Text });
                    if (result1.Data.Count() != 0)
                    {
                        sql = " UPDATE SFISM4.H108 SET KEY_PART_SN = KEY_PART_SN||'" + Doa_Time + "' WHERE '" + Update_R108 + "' ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return false;
                        }
                    }
                }

                return true;

            }
            catch
            {
                return false;
            }
        }
        private async Task<bool> CHECKRMALINK(string sn, string macid, string ssn)
        {
            if (macid != "")
            {
                sql = " SELECT * FROM SFISM4.R_RMA_LINK_T WHERE MAC1=  '" + macid + "' ";
                var result_4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_4.Data != null)
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<string> CHECKMAC_IDTYPE(string CUST_SN)
        {
            string id_type, mac_begin;
            int int_id_type = 0;
            string Result1 = "OK";
            string check_result = "";

                sql = " SELECT * FROM SFISM4.R_MO_EXT4_T WHERE '" + CUST_SN + "' <= ITEM_2 AND '" + CUST_SN + "' >=ITEM_1 AND LENGTH('" + CUST_SN + "')=LENGTH(ITEM_1) AND LENGTH('" + CUST_SN + "')= LENGTH(ITEM_2) ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    id_type = result.Data["item_4"]?.ToString() ?? "";
                    mac_begin = result.Data["item_1"].ToString();

                    if (id_type == "")
                    {
                        if (mac_begin == "")
                        {
                            sql = " SELECT * FROM  SFISM4.MMS_RANGE_USED WHERE '" + CUST_SN + "' BETWEEN  FRONT_ID||BEGIN_ID  AND FRONT_ID||END_ID  ";
                            var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (result_1.Data != null)
                            {
                                id_type = result_1.Data["item_4"]?.ToString() ?? "";

                                if (id_type == "")
                                {
                                    if (result_1.Data["mo_no"].ToString() == "")
                                    {
                                        showMessage("" + CUST_SN + " - Range not found! ", "" + CUST_SN + " - Không tìm thấy dải ! ", true);
                                        return Result1;
                                    }

                                }
                                else
                                {
                                    mac_begin = result_1.Data["front_id"].ToString() + result_1.Data["begin_id"].ToString();
                                }
                            }
                        }
                    }

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
                            int_id_type = Int32.Parse(id_type.Substring(0, 2).Trim());
                        }
                        catch
                        {
                            return Result1;
                        }
                    }

                    if (int_id_type > 1)
                    {
                        check_result = await CHECK_MACDATE(int_id_type, mac_begin, CUST_SN);

                        if (check_result != "OK")
                        {
                            Result1 = check_result;
                            return Result1;
                        }

                    }
                }

            return "OK";
        }

        private async Task<int> CHENGE16TO10(string TMPSTR)  
        {
            int Temp_Value = 0;
            sql = " SELECT TO_NUMBER('" + TMPSTR + "', 'XXXXXXXXXXXXXXXX') C_VALUE FROM DUAL ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

            Temp_Value = int.Parse(result.Data["c_value"].ToString());

            return Temp_Value;
        }

        private async Task<bool> CHECK_CHARACTER_0_9A_F(string TMPSTR)
        {
            var regexItem = new Regex("^[0-9A-F]*$");
            if (!regexItem.IsMatch(TMPSTR))
            {
                showMessage("" + TMPSTR + " - Invalid characters(0_9 A_F) !", "" + TMPSTR + " - Ký tự không hợp lệ (0_9 A_F) !", true);
                return false;
            }
            return true;
        }


        private async Task<bool> CHECK_CHARACTER_0_9A_Z(string TMPSTR)
        {
            var regexItem = new Regex("^[0-9A-Za-z]*$");
            if (!regexItem.IsMatch(TMPSTR))
            {
                showMessage("" + TMPSTR + " - Invalid characters(0_9 A_Z) !", "" + TMPSTR + " - Ký tự không hợp lệ (0_9 A_Z) !", true);
                return false;
            }

            return true;
        }

        private async Task<bool> CHECK_CHARACTER_0_9(string TMPSTR)
        {
            TMPSTR = TMPSTR.Trim();

            var regexItem = new Regex("^[0-9]*$");
            if (!regexItem.IsMatch(TMPSTR))
            {
                showMessage("" + TMPSTR + " - Invalid characters(0_9) !", "" + TMPSTR + " - Ký tự không hợp lệ (0_9) !", true);
                return false;
            }

            return true;
        }

        private async Task<string> CHECKLINK_SN_SSN(string TMPSTR)
        {
            string   foundmac, mod_type;
            string model_name = "", MOD_TYPE = "";

            Edit_SN.Text = Edit_SN.Text.Trim();
            Edit_MAC.Text = Edit_MAC.Text.Trim();

            if (Menu_Upper.Checked == true)
            {
                Edit_MAC.Text = Edit_MAC.Text.ToUpper().Trim();
            }

            if (Edit_SN.Text.Trim() == "")
            {
                showMessage("80157", "80157", false);
                Edit_SN.SelectAll();
                return "FALSE";
            }

            if (Edit_SN.Text.Trim() == Edit_MAC.Text.Trim())
            {
                showMessage("80139", "80139", false);
                Edit_SN.SelectAll();
                return "FALSE";
            }

            if (((Edit_MAC.Text.Length) != 12) && (Menu_12.Checked == true))
            {
                showMessage("80158", "80158", false);
                Edit_MAC.SelectAll();
                return "FALSE";
            }


            if (CheckB_RMA.Checked == false)
            {
                sql = " SELECT * FROM SFISM4.R_WIP_KEYPARTS_T WHERE KEY_PART_NO='MACID' AND  (KEY_PART_SN = '" + Edit_MAC.Text + "' OR SERIAL_NUMBER = '" + Edit_SN.Text + "' ) ";
                var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result1.Data.Count() > 1)
                {
                    showMessage("80159", "80159", false);
                    Edit_MAC.SelectAll();
                    return "FALSE";
                }
                else if (result1.Data.Count() > 0)
                {
                    if ((result1.Data["key_part_sn"].ToString() != Edit_MAC.Text) || (result1.Data["serial_number"].ToString() != Edit_SN.Text))
                    {
                        showMessage("40112", "40112", false);
                        Edit_MAC.SelectAll();
                        return "FALSE";
                    }

                }
                else
                {
                    sql = " SELECT * FROM SFISM4.H_WIP_KEYPARTS_T WHERE KEY_PART_NO='MACID' AND  (KEY_PART_SN = '" + Edit_MAC.Text + "' OR SERIAL_NUMBER = '" + Edit_SN.Text + "' ) ";
                    var result2 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result2.Data.Count() > 1)
                    {
                        showMessage("80159", "80159", false);
                        Edit_MAC.SelectAll();
                        return "FALSE";
                    }
                    else if (result2.Data.Count() > 0)
                    {
                        if ((result2.Data["key_part_sn"].ToString() != Edit_MAC.Text) || (result2.Data["serial_number"].ToString() != Edit_SN.Text))
                        {
                            showMessage("40112", "40112", false);
                            Edit_MAC.SelectAll();
                            return "FALSE";
                        }
                    }
                    else
                    {
                        sql = " SELECT A.MO_NUMBER FROM SFISM4.R_MO_EXT4_T A ";
                        sql = sql + " WHERE  '" + Edit_MAC.Text + "'  BETWEEN  A.ITEM_1  AND  A.ITEM_2  AND LENGTH( '" + Edit_MAC.Text + "' )=LENGTH(A.ITEM_1)  AND A.MO_NUMBER= '" + Lab_OLD_MO.Text + "' ";
                        sql = sql + " UNION ALL ";
                        sql = sql + " SELECT B.MO_NUMBER FROM SFISM4.H_MO_EXT4_T B ";
                        sql = sql + " WHERE  '" + Edit_MAC.Text + "'  BETWEEN  B.ITEM_1  AND  B.ITEM_2  AND LENGTH( '" + Edit_MAC.Text + "' )=LENGTH(B.ITEM_1)  AND B.MO_NUMBER= '" + Lab_OLD_MO.Text + " ";
                        sql = sql + " UNION ALL ";
                        sql = sql + " SELECT C.MO_NUMBER FROM SFISM4.R_RW_MAC_SSN_T C WHERE MAC= '" + Edit_MAC.Text + "'  AND SSN = '" + Edit_SN.Text + "' AND C.MO_NUMBER= '" + Lab_OLD_MO.Text + " ";
                        var result3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result3.Data == null)
                        {
                            showMessage("80143", "80143", false);
                            Edit_MAC.SelectAll();
                            return "FALSE";
                        }
                    }
                }

                sql = " SELECT * FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE MACID= '" + Edit_MAC.Text + "' ";
                var result4 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result4.Data != null)
                {
                    showMessage("80133", "80133", false);
                    Edit_MAC.SelectAll();
                    return "FALSE";
                }
            }

            foundmac = "N";

            sql = " SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T WHERE SSN = '" + Edit_SN.Text + "' OR MAC= '" + Edit_MAC.Text + "' ";
            var result5 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result5.Data != null)
            {
                if (result5.Data.Count() > 1)
                {
                    showMessage("80160", "80160", false);
                    Edit_MAC.SelectAll();
                    return "FALSE";
                }
                else
                {
                    if (((result5.Data["ssn"]?.ToString() ?? "") == Edit_SN.Text) && ((result5.Data["mac"]?.ToString() ?? "") == Edit_MAC.Text))
                    {
                        if (result5.Data["mo_number"].ToString() != varMO)
                        {
                            DialogResult result_05 = MessageBox.Show(" SN or MAC already add rework MO: " + result5.Data["mo_number"].ToString() + " , whether need changs nonce rework MO ? ", "QUESTION", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (result_05 == DialogResult.OK)
                            {
                                foundmac = "Y";
                            }
                            else
                            {
                                Edit_SN.SelectAll();
                                Edit_SN.Focus();
                                return "FALSE";
                            }
                        }
                        else
                        {
                            showMessage("SN already exists in R_RW_MAC_SSN_T ! ", "SN đã tồn tại trong R_RW_MAC_SSN_T ! ", true);
                            List_ErrorSN.Items.Add("" + Edit_SN.Text + "   Exist");
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return "FALSE";
                        }
                    }
                    else
                    {
                        showMessage("80147", "80147", false);
                        Edit_MAC.SelectAll();
                        return "FALSE";
                    }
                }
            }

            if (CheckB_RMA.Checked)
            {
                sql = " SELECT * FROM SFISM4.Z_WIP_TRACKING_T  WHERE SERIAL_NUMBER = '" + Edit_SN.Text + "' ";
                var result6 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result6.Data != null)
                {
                    model_name = result6.Data["model_name"].ToString();
                }
                else
                {
                    showMessage("80161", "80161", false);
                    Edit_SN.SelectAll();
                    return "FALSE";
                }

                sql = " SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME = '" + model_name + "'  ";
                var result7 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result7.Data != null)
                {
                    mod_type = result7.Data["mod_type"]?.ToString() ?? "";
                }
                else
                {
                    showMessage("80162", "80162", false);
                    Edit_SN.SelectAll();
                    return "FALSE";
                }
            }

            sql = " SELECT B.MODEL_TYPE FROM SFISM4.R_MO_BASE_T A ,SFIS1.C_MODEL_DESC_T B WHERE A.MODEL_NAME=B.MODEL_NAME AND A.MO_NUMBER= '" + varMO + "'  ";
            var result8 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result8.Data != null)
            {
                if ((result8.Data["model_type"]?.ToString() ?? "").IndexOf("075") != -1)
                {
                    if ((CheckB_Home.Checked == false) || (Edit_SSN.Visible == false))
                    {
                        showMessage("80163", "80163", false);
                        Edit_SN.SelectAll();
                        return "FALSE";
                    }
                    else
                    {
                        Edit_SSN.SelectAll();
                        Edit_SSN.Focus();
                        return "FALSE";
                    }
                }
                else
                {
                    if ((CheckB_Home.Checked == true) || (Edit_SSN.Visible == true))
                    {
                        showMessage("80163", "80163", false);
                        Edit_SN.SelectAll();
                        return "FALSE";
                    }
                }
            }
            else
            {
                showMessage("80162", "80162", false);
                Edit_SN.SelectAll();
                return "FALSE";
            }

            if (!await Update_Table(varSN, varMO))
            {
                Edit_SN.SelectAll();
                Edit_SN.Focus();
                return "FALSE";
            }

            if (CK_DB == "NIC")
            {
                // Update updRepairInOut
                sql = " UPDATE SFISM4.R_REPAIR_IN_OUT_T SET SERIAL_NUMBER = SERIAL_NUMBER|| '" + varMO + "'  ";
                sql = sql + " WHERE SERIAL_NUMBER= '" + Edit_SN.Text + "'  AND MO_NUMBER <> '" + varMO + "' ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    Edit_SN.SelectAll();
                    Edit_SN.Focus();
                    return "FALSE";
                }
            }

            if ((CheckB_RMA.Checked) && (MOD_TYPE.IndexOf("045") != -1))
            {
                if (CK_DB == "ROKU")  // ROKU
                {
                    sql = " SELECT *FROM SFISM4.P_TMP_CUSTOMER_T@sfcoDBH WHERE SERIAL_NUMBER = '" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                }
                else
                {
                    sql = " SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER = '" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                }

                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    // Update HCustomer
                    sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER|| '" + varMO + "',    ";
                    sql = sql + " MACID=MACID||'" + varMO + "', SHIPPING_SN=SHIPPING_SN||'" + varMO + "' WHERE SHIPPING_SN= '" + Edit_SN.Text + "' ";
                    var y = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (y.Result != "OK")
                    {
                        ReportError(y.Message.ToString());
                        Edit_SN.SelectAll();
                        Edit_SN.Focus();
                        return "FALSE";
                    }
                }

                if (CK_DB == "ROKU")  //ROKU
                {
                    if (await Check_History())
                    {
                        sql = " SELECT *FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    }
                    else
                    {
                        sql = " SELECT *FROM SFISM4.P_TMP_CUSTOMER_T WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    }
                }
                else
                {
                    sql = " SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NULL ";
                    var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_1.Data != null)
                    {
                        // Update HCustomer
                        sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER||'" + varMO + "', ";
                        sql = sql + " MACID=MACID||'" + varMO + "' WHERE SERIAL_NUMBER= '" + Edit_SN.Text + "' ";
                        var z = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (z.Result != "OK")
                        {
                            ReportError(z.Message.ToString());
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return "FALSE";
                        }
                    }
                }
            }
            else if ((CheckB_RMA.Checked) && (MOD_TYPE.IndexOf("045") != -1))
            {
                if (CK_DB == "ROKU")  //ROKU
                {
                    if (await Check_History())
                    {
                        sql = " SELECT *FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    }
                    else
                    {
                        sql = " SELECT *FROM SFISM4.P_TMP_CUSTOMER_T WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    }
                }
                else
                {
                    sql = " SELECT *FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_1.Data != null)
                    {
                        // Update HCustomer
                        sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER||'" + varMO + "', ";
                        sql = sql + " MACID=MACID||'" + varMO + "' WHERE SERIAL_NUMBER= '" + Edit_SN.Text + "' ";
                        var z = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (z.Result != "OK")
                        {
                            ReportError(z.Message.ToString());
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return "FALSE";
                        }
                    }
                }

                if (CK_DB == "ROKU")  //ROKU
                {
                    if (await Check_History())
                    {
                        sql = " SELECT *FROM SFISM4.P_TMP_CUSTOMER_T@SFCODBH WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    }
                    else
                    {
                        sql = " SELECT *FROM SFISM4.P_TMP_CUSTOMER_T WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    }
                }
                else
                {
                    sql = " SELECT *FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER='" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                    var result_1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result_1.Data != null)
                    {
                        // Update HCustomer2
                        sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER||'" + varMO + "', ";
                        sql = sql + " SHIPPING_SN = SHIPPING_SN||'" + varMO + "'   WHERE SHIPPING_SN = '" + Edit_SSN.Text + "' ";
                        var z = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (z.Result != "OK")
                        {
                            ReportError(z.Message.ToString());
                            Edit_SN.SelectAll();
                            Edit_SN.Focus();
                            return "FALSE";
                        }
                    }
                }


            }


            else if (CheckB_RMA.Checked)
            {
                sql = " SELECT * FROM SFISM4.H_TMP_CUSTOMER_T WHERE SERIAL_NUMBER = '" + Edit_SN.Text + "' AND SHIPPING_SN IS NOT NULL ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data != null)
                {
                    // Update HCustomer
                    sql = " UPDATE SFISM4.P_TMP_CUSTOMER_T@SFCODBH SET SERIAL_NUMBER=SERIAL_NUMBER|| '" + varMO + "',    ";
                    sql = sql + " MACID=MACID||'" + varMO + "', SHIPPING_SN=SHIPPING_SN||'" + varMO + "' WHERE SHIPPING_SN= '" + Edit_SN.Text + "' ";
                    var y = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (y.Result != "OK")
                    {
                        ReportError(y.Message.ToString());
                        Edit_SN.SelectAll();
                        Edit_SN.Focus();
                        return "FALSE";
                    }
                }
            }

            if (foundmac == "Y")
            {
                // Insert RW_Detail
                sql = " INSERT INTO SFISM4.R_RW_DETAIL_T (SELECT * FROM SFISM4.R_RW_MAC_SSN_T WHERE SSN= '" + Edit_SN.Text + "' AND MAC= '" + Edit_MAC.Text + "' )   ";
                var q = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (q.Result != "OK")
                {
                    ReportError(q.Message.ToString());
                    Edit_SN.SelectAll();
                    Edit_SN.Focus();
                    return "FALSE";
                }

                // Update Rw_SSN_Mac
                sql = " UPDATE SFISM4.R_RW_MAC_SSN_T SET MO_NUMBER = '" + varMO + "' ,TYPE= '" + Lab_Model.Text + "',  ";
                sql = sql + " EMP_NO= '" + empNo + "' WHERE SSN = '" + Edit_SN.Text + "' AND MAC= '" + Edit_MAC.Text + "' ";
                var w = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (w.Result != "OK")
                {
                    ReportError(w.Message.ToString());
                    Edit_SN.SelectAll();
                    Edit_SN.Focus();
                    return "FALSE";
                }
            }
            else
            {
                // Add Rw_SSN_Mac
                sql = " INSERT INTO  SFISM4.R_RW_MAC_SSN_T ( MO_NUMBER,SN,SSN,MAC,TYPE ,EMP_NO )  ";
                sql = sql + " VALUES ( '" + varMO + "' , '" + Edit_SN.Text + "', '" + Edit_SN.Text + "' , '" + Edit_MAC.Text + "' , '" + Lab_Model.Text + "'  , '" + empNo + "' ) ";
                var w = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (w.Result != "OK")
                {
                    ReportError(w.Message.ToString());
                    Edit_SN.SelectAll();
                    Edit_SN.Focus();
                    return "FALSE";
                }
            }

            Edit_SN.SelectAll();
            Edit_SN.Focus();
            return "OK";
        }

        private async Task<String> CHECK_MACDATE(int ID_TYPE, string START_MAC, string CHECK_MAC) 
        {
            int Check = 0;
            string Result_Error = "MAC ID TYPE IS "+ ID_TYPE .ToString()+ ", But ("+ CHECK_MAC + " - "+ START_MAC + ") mod " + ID_TYPE.ToString() + "  Not ZERO(0)";
            try
            {
                if ( (!await CHECK_CHARACTER_0_9A_F(CHECK_MAC.Substring(5, 7))) || (!await CHECK_CHARACTER_0_9A_F(START_MAC.Substring(5, 7))) )
                {
                    return null;
                }

                Check = (( await CHENGE16TO10(CHECK_MAC.Substring(5,7)) - await CHENGE16TO10(START_MAC.Substring(5, 7))) % ID_TYPE);

                if (Check != 0)
                {
                    return Result_Error;
                }
                else
                {
                    return "OK";
                }
            }
            catch
            {
                return "OK";
            }
        }

        private void Main_Prog_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("ADD_RWMAC"))
            {
                process.Kill();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Menu_IpQR_Click(object sender, EventArgs e)
        {
            Menu_IpQR.Checked = true;
        }

        private void CheckB_Home_CheckStateChanged(object sender, EventArgs e)
        {
            if (CheckB_Home.Checked)
            {
                CheckB_Furu.Enabled = false;
                Edit_SSN.Visible = true;
                label4.Visible = true;
            }
            else
            {
                CheckB_Furu.Enabled = true;
                label4.Visible = false;
                Edit_SSN.Visible = false;
            }
        }

        private async void Ck_SNPrefix_Click(object sender, EventArgs e)
        {
            sql = string.Format("  SELECT * FROM SFIS1.C_PRIVILEGE  where FUN ='CHECK SN PREFIX' AND PRG_NAME = 'ADD_RWMAC' AND EMP = '" + empNo + "' ");

            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result_list.Data.Count() == 0)
            {
                Ck_SNPrefix.Checked = true;
                showMessage("You do not have permission to remove Check SN Prefix !\nFUN = CHECK SN PREFIX AND PRG_NAME = ADD_RWMAC", "Bạn không có quyền bỏ Check SN Prefix !", true);
                return;
            }
        }

        private void CheckB_Furu_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckB_Furu.Checked)
            {
                CheckB_Home.Enabled = false;
            }
            else
            {
                CheckB_Home.Enabled = true;
            }
        }

        private void Menu_Exit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to close the program ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Menu_About_Click(object sender, EventArgs e)
        {
            Info_AB info_About = new Info_AB();
            info_About.ShowDialog();
        }

        private async Task MARKR107R108(string SN, string MO)
        {
            if ((!CheckB_SN_MAC.Checked) && (!CheckB_SN_SSN.Checked))
            {
                showMessage("80163", "80163", false);
                return;
            }

            if (!await Update_Table(SN, MO))
            {
                return;
            }

            // Update Range
            sql = " UPDATE  SFISM4.R_MO_EXT_T SET ITEM_1=SUBSTR(ITEM_1,1,12)||'" + MO + "', ITEM_2=SUBSTR(ITEM_2,1,12)||'" + MO + "'  ";
            sql = sql + " WHERE '" + SN + "' BETWEEN ITEM_1 AND ITEM_2 AND MO_NUMBER<>'" + MO + "' ";
            var k = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (k.Result != "OK")
            {
                ReportError(k.Message.ToString());
                return;
            }

            if (CK_DB == "NIC")
            {
                // Update updRepairInOut
                sql = " UPDATE SFISM4.R_REPAIR_IN_OUT_T SET SERIAL_NUMBER = SERIAL_NUMBER|| '" + MO + "'  ";
                sql = sql + " WHERE SERIAL_NUMBER= '" + SN + "'  AND MO_NUMBER <> '" + MO + "' ";
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    Edit_SN.SelectAll();
                    Edit_SN.Focus();
                    return;
                }
            }

            if (CheckB_SN_MAC.Checked)
            {
                if (T23SFLAG)
                {
                    sql = " UPDATE SFISM4.R_RW_MAC_SSN_T SET SN = '" + SN + "' WHERE MAC ='" + SN.Substring(3, 12) + "'  ";
                    var s = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (s.Result != "OK")
                    {
                        ReportError(s.Message.ToString());
                        return;
                    }
                }
                else
                {
                    sql = " UPDATE SFISM4.R_RW_MAC_SSN_T SET SN = MAC WHERE MAC = '" + SN + "'  ";
                    var s = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (s.Result != "OK")
                    {
                        ReportError(s.Message.ToString());
                        return;
                    }

                }

                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();

                sql = " SELECT  * FROM  SFISM4.R_RW_MAC_SSN_T WHERE MAC = '" + SN + "' OR  SSN =  '" + SN + "'  ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() != 0)
                {
                    var stringListDic = JsonConvert.SerializeObject(result_list1.Data);
                    dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
                    dataGridView1.DataSource = dt;
                    dataGridView1.AutoResizeColumns();
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }

                sql = " SELECT  COUNT(*) COUNT_RW FROM  SFISM4.R_RW_MAC_SSN_T WHERE MO_NUMBER = '" + varMO + "'  ";
                var result01 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                Lab_Qty_RW1.Text = result01.Data["count_rw"].ToString();

            }
        }

        private void Menu_Upfile_Click(object sender, EventArgs e)
        {
            if (Menu_Upfile.Checked)
            {
                Picture_BackGr.Visible = false;
                Pane_Upfile.Visible = true;

                List_SN.Clear();
                List_MAC.Clear();
                List_SSN.Clear();
                List_SSN2.Clear();

                List_SN.Visible = Edit_SN.Visible;
                List_MAC.Visible = Edit_MAC.Visible;
                List_SSN.Visible = Edit_SSN.Visible;
                List_SSN2.Visible = Edit_SSN2.Visible;
            }
            else
            {
                Picture_BackGr.Visible = true;
                Pane_Upfile.Visible = false;
            }

        }

        private async void Button_Upload_Click(object sender, EventArgs e)
        {
            errorflag = false; 

            if (List_SN.Visible == true)
            {
                if (List_SN.Lines.Count() != List_MAC.Lines.Count())
                {
                    showMessage("Quantity SN: " + List_SN.Lines.Count().ToString() + " <> MAC: " + List_MAC.Lines.Count().ToString() + " ", "Số lượng SN: " + List_SN.Lines.Count().ToString() + "  <> MAC: " + List_MAC.Lines.Count().ToString() + " ", true);
                    return;
                }
            }

            if (List_SSN.Visible == true)
            {
                if (List_SSN.Lines.Count() != List_MAC.Lines.Count())
                {
                    showMessage("Quantity SSN: " + List_SSN.Lines.Count().ToString() + " <> MAC: " + List_MAC.Lines.Count().ToString() + " ", "Số lượng SSN: " + List_SSN.Lines.Count().ToString() + "  <> MAC: " + List_MAC.Lines.Count().ToString() + " ", true);
                    return;
                }
            }

            if (List_SSN2.Visible == true)
            {
                if (List_SSN2.Lines.Count() != List_MAC.Lines.Count())
                {
                    showMessage("Quantity SSN2: " + List_SSN2.Lines.Count().ToString() + " <> MAC: " + List_MAC.Lines.Count().ToString() + " ", "Số lượng SSN2: " + List_SSN2.Lines.Count().ToString() + "  <> MAC: " + List_MAC.Lines.Count().ToString() + " ", true);
                    return;
                }
            }

            for (int i = 0; i < List_MAC.Lines.Count(); i++)
            {
                if (errorflag)
                {
                    return;
                }

                if (List_SN.Visible == true)
                {
                    string var_SN;
                    Edit_SN.Text = List_SN.Lines[i].ToString();
                    await Load_SN_Key();
                    if (errorflag)
                    {
                        return;
                    }
                }

                if (List_MAC.Visible == true)
                {
                    Edit_MAC.Text = List_MAC.Lines[i].ToString();
                    await Load_Mac_Key();
                    if (errorflag)
                    {
                        return;
                    }
                }

                if (List_SSN.Visible == true)
                {
                    Edit_SSN.Text = List_SSN.Lines[i].ToString();
                    await Load_SSN_Key();
                    if (errorflag)
                    {
                        return;
                    }
                }

                if (List_SSN2.Visible == true)
                {
                    Edit_SSN2.Text = List_SSN2.Lines[i].ToString();
                    await Load_SSN2_Key();
                    if (errorflag)
                    {
                        return;
                    }
                }

            }
        }

        private async void Menu_Move_Mo_Click(object sender, EventArgs e)
        {
            sql = " SELECT * FROM SFIS1.C_PRIVILEGE  WHERE  EMP = '" + empNo + "'  AND FUN='CHANGE-MO' AND PRG_NAME='ADD-RWMAC'   ";
            var result1 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result1.Data == null)
            {
                showMessage("Only IPQC has permission to use this function\nFUN=CHANGE-MO AND PRG_NAME=ADD-RWMAC ", "IPQC mới có quyền sử dụng chức năng này ! ", true);
                return;
            }
            else
            {
                Change_Mo CHANGE_MO = new Change_Mo(_sfcHttpClient);
                CHANGE_MO._sfcHttpClient = _sfcHttpClient;
                CHANGE_MO.empNo = empNo;
                CHANGE_MO.Show();
                this.Hide();
            }
        }

        private async void Menu_Move_Range_Click(object sender, EventArgs e)
        {

            sql = " SELECT*FROM SFIS1.C_PRIVILEGE WHERE EMP = '" + empNo + "' AND PRG_NAME='ADDRWMAC' AND FUN='MOVE_RANGE' AND PRIVILEGE = 2  ";
            var result3 = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result3.Data != null)
            {
                Move_Range MOVE_RANGE = new Move_Range(_sfcHttpClient);
                MOVE_RANGE.empNo = empNo;
                MOVE_RANGE.Show();
                this.Hide();
            }
            else
            {
                showMessage("You do not have permission to use this function ! \n PRG_NAME = ADDRWMAC AND FUN = MOVE_RANGE AND PRIVILEGE = 2", "Bạn không có quyền sử dụng chức năng này ! ", true);
                return;
            }
        }

        private void Menu_19_Click(object sender, EventArgs e)
        {
            if (Menu_19.Checked)
            {
                Menu_19.Checked = true;
                Menu_12.Checked = false;
                Menu_10.Checked = false;
                Menu_16.Checked = false;
            }
            else
            {
                Menu_19.Checked = false;
                Menu_12.Checked = false;
                Menu_10.Checked = false;
                Menu_16.Checked = false;
            }
        }

        private void Menu_16_Click(object sender, EventArgs e)
        {
            if (Menu_16.Checked)
            {
                Menu_16.Checked = true;
                Menu_12.Checked = false;
                Menu_10.Checked = false;
                Menu_19.Checked = false;
            }
            else
            {
                Menu_16.Checked = false;
                Menu_12.Checked = false;
                Menu_10.Checked = false;
                Menu_19.Checked = false;
            }
        }

        private async void CheckB_Mac_Range_Click(object sender, EventArgs e)
        {
            if(CK_DB != "ROKU")
            {
                sql = string.Format("SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO =  '" + empNo + "'  AND  CLASS_NAME LIKE '%5%' ");
               
                var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list.Data.Count() == 0)
                {
                    CheckB_Mac_Range.Checked = true;
                    showMessage("80138", "80138", false);
                    return;
                }
            }
        }

        private async void Edit_SSN2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                if (Edit_SN.Text == "UNDO" || Edit_MO.Text == "UNDO" || Edit_SSN.Text == "UNDO" || Edit_MAC.Text == "UNDO" || Edit_SSN2.Text == "UNDO")
                {
                    Edit_SN.Text = "";
                    Edit_Length.Text = "";
                    Edit_MO.Text = "";
                    Edit_SSN.Text = "";
                    Edit_MAC.Text = "";
                    Edit_SSN2.Text = "";
                    varMAC = "";
                    varMO = "";
                    varSN = "";
                    varSSN2 = "";
                    Edit_MO.Enabled = true;
                    Edit_MO.Focus();
                }
                else
                {
                    varSSN2 = Edit_SSN2.Text;
                    await Load_SSN2_Key();
                }
            }
        }

        private async Task Load_SSN2_Key( )
        {
            if (Edit_SSN2.Text != "")
            {
                if (!await CHECK_CHARACTER_0_9A_Z(varSSN2))
                {
                    Edit_SSN2.SelectAll();
                    return;
                }

                sql = " SELECT *  FROM SFISM4.R_UNIQUE_SSN_T WHERE SSN = '" + varSSN2 + "'  ";
                var result_list1 = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result_list1.Data.Count() == 0)
                {
                    showMessage("Data not found in R_UNIQUE_SSN_T", "Dữ liệu không tìm thấy trong R_UNIQUE_SSN_T", true);
                    Edit_SSN2.SelectAll();
                    return;
                }

                sql = string.Format("DELETE SFISM4.R_UNIQUE_SSN_T WHERE SSN = '" + varSSN2 + "' ");
                var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

                if (a.Result != "OK")
                {
                    ReportError(a.Message.ToString());
                    return;
                }
                else
                {
                    Edit_MAC.SelectAll();
                    Edit_MAC.Focus();

                }
            }
        }

        private void showMessage(string MessageEnglish, string MessageVietNam, bool CustomFlag)
        {
            MessageError frmMessage = new MessageError();
            frmMessage._sfcHttpClient = _sfcHttpClient;
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

        private async Task<bool> Update_Table(string SN, string MO)
        {
            // Update R107
            sql = " UPDATE SFISM4.R_WIP_TRACKING_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "',  ";
            sql = sql + " SHIPPING_SN=SHIPPING_SN||'RW', SHIPPING_SN2=SHIPPING_SN2||'RW',  ";
            sql = sql + " PO_NO=PO_NO||'RW' WHERE SERIAL_NUMBER= '" + SN + "' AND MO_NUMBER<>'" + MO + "' ";
            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (a.Result != "OK")
            {
                ReportError(a.Message.ToString());
                return false;
            }

            // Update H107
            sql = " UPDATE SFISM4.H_WIP_TRACKING_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "',  ";
            sql = sql + " SHIPPING_SN=SHIPPING_SN||'RW', SHIPPING_SN2=SHIPPING_SN2||'RW',  ";
            sql = sql + " PO_NO=PO_NO||'RW' WHERE SERIAL_NUMBER= '" + SN + "' ";
            var b = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (b.Result != "OK")
            {
                ReportError(b.Message.ToString());
                return false;
            }

            // Update R108
            sql = " UPDATE SFISM4.R_WIP_KEYPARTS_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "', KEY_PART_NO='RMACID',  ";
            sql = sql + " KEY_PART_SN=KEY_PART_SN||'RW' WHERE SERIAL_NUMBER= '" + SN + "' AND KEY_PART_NO='MACID' AND MO_NUMBER<>'" + MO + "' ";
            var c = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (c.Result != "OK")
            {
                ReportError(c.Message.ToString());
                return false;
            }

            // Update H108
            sql = " UPDATE SFISM4.H_WIP_KEYPARTS_T SET SERIAL_NUMBER=SERIAL_NUMBER|| '" + MO + "',  ";
            sql = sql + " KEY_PART_SN=KEY_PART_SN||'RW' WHERE SERIAL_NUMBER= '" + SN + "' AND KEY_PART_NO='MACID' ";
            var d = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (d.Result != "OK")
            {
                ReportError(d.Message.ToString());
                return false;
            }

            // Update R108 SN
            sql = " UPDATE SFISM4.R_WIP_KEYPARTS_T SET SERIAL_NUMBER=SERIAL_NUMBER|| '" + MO + "'  ";
            sql = sql + " WHERE SERIAL_NUMBER= '" + SN + "' AND KEY_PART_NO<>'MACID' AND MO_NUMBER<>'" + MO + "' ";
            var e = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (e.Result != "OK")
            {
                ReportError(e.Message.ToString());
                return false;
            }

            // Update H108 SN
            sql = " UPDATE SFISM4.H_WIP_KEYPARTS_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "'  ";
            sql = sql + " WHERE SERIAL_NUMBER= '" + SN + "' AND KEY_PART_NO<>'MACID' ";
            var u = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (u.Result != "OK")
            {
                ReportError(u.Message.ToString());
                return false;
            }

            // Update R117
            sql = " UPDATE SFISM4.R_SN_DETAIL_T SET SERIAL_NUMBER = SERIAL_NUMBER ";
            sql = sql + " WHERE SERIAL_NUMBER= '" + SN + "' AND MO_NUMBER<>'" + MO + "' ";
            var f = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

            if (f.Result != "OK")
            {
                ReportError(f.Message.ToString());
                return false;
            }

            // Update Z107
            sql = "  UPDATE SFISM4.Z_WIP_TRACKING_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "',  ";
            sql = sql + " SHIPPING_SN=SHIPPING_SN||'RW', SHIPPING_SN2=SHIPPING_SN2||'RW',  ";
            sql = sql + " PO_NO=PO_NO||'RW' WHERE SERIAL_NUMBER= '" + SN + "' AND MO_NUMBER<>'" + MO + "' ";
            var g = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (g.Result != "OK")
            {
                ReportError(g.Message.ToString());
                return false;
            }

            // Update CustSn
            sql = " UPDATE SFISM4.R_CUSTSN_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "',   ";
            sql = sql + " SSN1=DECODE(SSN1,NULL,SSN1,SSN1||'RW'), SSN2=DECODE(SSN2,NULL,SSN2,SSN2||'RW'),  ";
            sql = sql + " SSN3=DECODE(SSN3,NULL,SSN3,SSN3||'RW'), SSN4=DECODE(SSN4,NULL,SSN4,SSN4||'RW'), ";
            sql = sql + " SSN5=DECODE(SSN5,NULL,SSN5,SSN5||'RW'), SSN6=DECODE(SSN6,NULL,SSN6,SSN6||'RW'), ";
            sql = sql + " MAC1=DECODE(MAC1,NULL,MAC1,MAC1||'RW'), MAC2=DECODE(MAC2,NULL,MAC2,MAC2||'RW'),  ";
            sql = sql + " MAC3=DECODE(MAC3,NULL,MAC3,MAC3||'RW'), MAC4=DECODE(MAC4,NULL,MAC4,MAC4||'RW'),  ";
            sql = sql + " MAC5=DECODE(MAC5,NULL,MAC5,MAC5||'RW') WHERE SERIAL_NUMBER= '" + SN + "' AND MO_NUMBER<>'" + MO + "' ";
            var h = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (h.Result != "OK")
            {
                ReportError(h.Message.ToString());
                return false;
            }

            // Update HCustSn
            sql = " UPDATE SFISM4.H_CUSTSN_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "',   ";
            sql = sql + " SSN1=DECODE(SSN1,NULL,SSN1,SSN1||'" + MO + "'), SSN2=DECODE(SSN2,NULL,SSN2,SSN2||'" + MO + "'),  ";
            sql = sql + " SSN3=DECODE(SSN3,NULL,SSN3,SSN3||'" + MO + "'), SSN4=DECODE(SSN4,NULL,SSN4,SSN4||'" + MO + "'),  ";
            sql = sql + " SSN5=DECODE(SSN5,NULL,SSN5,SSN5||'" + MO + "'), MAC1=DECODE(MAC1,NULL,MAC1,MAC1||'" + MO + "'),  ";
            sql = sql + " MAC2=DECODE(MAC2,NULL,MAC2,MAC2||'" + MO + "'), MAC3=DECODE(MAC3,NULL,MAC3,MAC3||'" + MO + "'),  ";
            sql = sql + " MAC4=DECODE(MAC4,NULL,MAC4,MAC4||'" + MO + "'), MAC5=DECODE(MAC5,NULL,MAC5,MAC5||'" + MO + "') WHERE SERIAL_NUMBER='" + SN + "' ";
            var m = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (m.Result != "OK")
            {
                ReportError(m.Message.ToString());
                return false;
            }

            // Update UpdR109
            sql = " UPDATE SFISM4.R_REPAIR_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "'  ";
            sql = sql + " WHERE SERIAL_NUMBER='" + SN + "' AND MO_NUMBER<>'" + MO + "' ";
            var n = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (n.Result != "OK")
            {
                ReportError(n.Message.ToString());
                return false;
            }

            // Update UpdH109
            sql = " UPDATE SFISM4.H_REPAIR_T SET SERIAL_NUMBER=SERIAL_NUMBER||'" + MO + "'   ";
            sql = sql + " WHERE SERIAL_NUMBER='" + SN + "'  ";
            var o = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (o.Result != "OK")
            {
                ReportError(o.Message.ToString());
                return false;
            }

            return true;
        }

        private async void Edit_MO_Qry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await FindMo();
            }
        }


        private async void Edit_Length_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (await CHECK_CHARACTER_0_9(Edit_Length.Text))
                {
                    if (Edit_Length.Text.Trim().Length > 2)
                    {
                        showMessage("Too long !", "Độ dài quá lớn !", true);
                        Edit_Length.SelectAll();
                        Edit_Length.Focus();
                        return;
                    }

                    SN_Length = Int32.Parse(Edit_Length.Text);

                    if (SN_Length < 7)
                    {
                        showMessage("Invalid length. Must > 7", "Độ dài không hợp lệ. Phải > 7 ", true);
                        Edit_Length.SelectAll();
                        Edit_Length.Focus();
                        return;
                    }
                    else
                    {
                        Edit_Length.Enabled = false;
                        Edit_MAC.Focus();
                    }
                }
                else
                {
                    Edit_Length.SelectAll();
                    Edit_Length.Focus();
                }
            }
        }

        private async Task REWORK_BYSN(string C_Mo_Number, string C_special_Route, string C_Version, string C_First_Group, string C_SN)
        {
            sql = " SELECT * FROM SFISM4.R107 WHERE  SERIAL_NUMBER = '" + C_SN + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                if (result.Data["model_name"].ToString() == Mmodel_name)
                {
                    if ((result.Data["error_flag"]?.ToString() ?? "") != "0")
                    {
                        if ((result.Data["error_flag"]?.ToString() ?? "") == "8")
                        {
                            showMessage("90055", "90055", false);
                        }
                        else
                        {
                            showMessage("90054", "90054", false);
                        }

                        sql = " DELETE SFISM4.R_RW_MAC_SSN_T WHERE (MAC = '" + C_SN + "' OR SN = '" + C_SN + "') AND MO_NUMBER = '" + varMO + "' ";
                        var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (a.Result != "OK")
                        {
                            ReportError(a.Message.ToString());
                            return;
                        }
                    }

                    if (((result.Data["wip_group"]?.ToString() ?? "").Substring(0, 2) == "SC") ||
                        ((result.Data["wip_group"]?.ToString() ?? "").Substring(0, 2) == "BC"))
                    {
                        sql = " SELECT * FROM SFISM4.R_REPAIR_T WHERE REPAIRER IS NULL AND SERIAL_NUMBER = '" + C_SN + "' ";
                        result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (result.Data == null)
                        {
                            sql = " UPDATE SFISM4.R107 SET MO_NUMBER = '" + C_Mo_Number + "', SPECIAL_ROUTE='" + C_special_Route + "', VERSION_CODE='" + C_Version + "', ";
                            sql = sql + "  WIP_GROUP ='" + C_First_Group  + "' ,NEXT_STATION='" + C_First_Group + "', GROUP_NAME='', SHIPPING_SN='N/A', TRAY_NO='N/A', ";
                            sql = sql + "  TRACK_NO='N/A', CARTON_NO='N/A', PALLET_FULL_FLAG='', MO_NUMBER_OLD='', MCARTON_NO='N/A', STOCK_NO='N/A', ";
                            sql = sql + "  PALLET_NO='N/A', IMEI='N/A', QA_NO='N/A', QA_RESULT='N/A', scrap_flag ='0' WHERE SERIAL_NUMBER='" + C_SN + "' ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return;
                            }
                        }
                        else
                        {
                            sql = " DELETE SFISM4.R_RW_MAC_SSN_T where (MAC= '" + C_SN + "' OR SN= '" + C_SN + "') AND MO_NUMBER='" + varMO + "' ";
                            var a = await _sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (a.Result != "OK")
                            {
                                ReportError(a.Message.ToString());
                                return;
                            }

                            showMessage("90054", "90054", false);
                            return;
                        }
                    }
                }


                showMessage("90055", "90055", false);
                Edit_SSN.SelectAll();
                return;
            }
        }



    }
}
