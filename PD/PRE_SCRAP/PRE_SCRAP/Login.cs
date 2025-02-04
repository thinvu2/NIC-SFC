
using Oracle;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using SFC_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using PRE_SCRAP.Resources;
using Newtonsoft.Json;

namespace PRE_SCRAP
{
    public partial class Login : Form
    {
        public DB _oracle;
        public string inputLogin, checkSum, baseUrl, loginDB, empNo, empPass, error_code, sql, Temp_Station, Emp_Manager, C_Empno, C_Connect;
        DataTable dt;
        string C_line, C_section, C_group, C_StationName;
        public SfcHttpClient _sfcHttpClient;
        MessageError FrmMessage = new MessageError();

        public Login()
        {
            AccessAPI();
            CheckLogin();
            InitializeComponent();
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

            _sfcHttpClient = DBAPI.sfcClient(loginDB, baseUrl);
            try
            { 
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
                _oracle = new DB(_sfcHttpClient);
                DBAPI._sfcHttpClient = _sfcHttpClient;
                Get_Group();

                EmployeeModel emp = await _oracle.GetObj<EmployeeModel>($"SELECT * FROM SFIS1.C_EMP_DESC_T where EMP_NO = '{empNo}' AND EMP_PASS = '{empPass}'");
                Emp_Manager = string.Format("{0} - {1}", empNo, emp.EMP_NAME);
                C_Empno = empNo;
                C_Connect = string.Format("   Connected:  {0}", loginDB);
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

        private async void Get_Group()
        {
            sql = " SELECT LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME FROM SFIS1.C_STATION_CONFIG_T  ";
            sql = sql + " WHERE STATION_NAME LIKE '%PRE_%' OR GROUP_NAME LIKE '%PRE_%' ORDER BY LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME  ";
            var result_list = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });

            var stringListDic = JsonConvert.SerializeObject(result_list.Data);
            dt = JsonConvert.DeserializeObject<DataTable>(stringListDic);
            Txt_Group.DataSource = dt.Copy();
            Txt_Group.DisplayMember = "group_name";
            Txt_Group.ValueMember = "group_name";
            Txt_Group.Text = "";
        }

        async void CheckLogin()  //API
        {
            try
            {
                await _sfcHttpClient.GetAccessTokenAsync(empNo, empPass);

                if (empNo.Length < 7)
                {
                    showMessage("Please re-login AMS with card code and password !", "Vui lòng đăng nhập lại AMS bằng mã thẻ và mật khẩu !\nKhông được sử dụng tài khoản: "+ empNo + "", true);
                    this.Close();
                }

                sql = " SELECT EMP_NO FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO ='" + empNo + "' AND EMP_BC ='" + empPass + "' AND SYSDATE < QUIT_DATE ";
                var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (result.Data == null)
                {
                    showMessage("" + empNo + " :Expired card code !\n(SYSDATE > QUIT_DATE or EMP_RANK = 0)", "Mã thẻ: " + empNo + "  hết hạn !", true);
                    this.Close();
                }
                else
                {
                    sql = " SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO ='" + empNo + "' AND EMP_BC ='" + empPass + "' ";
                    result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if (result.Data != null)
                    {
                        if((result.Data["emp_rank"]?.ToString()?? "") == "1")
                        {
                            showMessage("00075", "00075", false);
                            this.Close();
                        }
                        if((result.Data["email"]?.ToString() ?? "") == "OFFWORK")
                        {
                            showMessage(""+ empNo + " :Employee card number has retired !", "" + empNo + ": Mã thẻ nhân viên đã nghỉ việc !", true);
                            this.Close();
                        }
                    }

                    sql = " SELECT * FROM SFIS1.C_EMP_2_GROUP_T WHERE EMP_NO = '" + empNo + "' AND ( GROUP_NAME = 'ALL' OR GROUP_NAME IN  ";
                    sql = sql + " (SELECT GROUP_NAME FROM SFIS1.C_STATION_CONFIG_T WHERE STATION_NAME LIKE '%PRE_%' OR GROUP_NAME LIKE '%PRE_%') )  ";
                    result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                    if(result.Data == null)
                    { 
                        showMessage("EMP not have premission in Group: C_STATION_CONFIG_T and EMP_RANK <> 0 !\nC_EMP_2_GROUP_T", "Mã thẻ không có quyền trong nhóm: C_STATION_CONFIG_T and EMP_RANK <> 0 !", true);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
            }
        }
        private async void Btn_Login_Click(object sender, EventArgs e)
        {
            sql = " SELECT * FROM SFIS1.C_STATION_CONFIG_T WHERE (STATION_NAME LIKE '%PRE_%' OR GROUP_NAME LIKE '%PRE_%') AND GROUP_NAME = '"+ Txt_Group.Text + "' ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                showMessage("" + Txt_Group.Text + " Not in List !", "" + Txt_Group.Text + " Không nằm trong List !", true);
                Txt_Group.Focus();
                Txt_Group.SelectAll();
                return;
            }

            if (Txt_Group.Text == "")
            {
                showMessage("You Must Select Group Name !", "Bạn phải chọn tên nhóm !", true);
                Txt_Group.Focus();
                return;
            }

            if ((await Check_Privilege(Txt_Group.Text)) != "OK")
            {
                return;
            }
            else
            {
                Get_Station.sStation = C_StationName;
                Get_Station.eEmp_Manager = Emp_Manager;
                Get_Station.eEmp_No = C_Empno;

                Get_Station.lLine = C_line;
                Get_Station.sSection_name = C_section;
                Get_Station.gGroup_name = C_group;

                Get_Station.cConnectStatus = C_Connect;


                Main_Prescrap main_Prescrap = new Main_Prescrap(this);
                main_Prescrap._sfcHttpClient = _sfcHttpClient;


                Ini ini = new Ini(@"C:\Windows\SFIS.ini");
                ini.IniWriteValue("PreScrap", "line", C_line);
                ini.IniWriteValue("PreScrap", "section", C_section);
                ini.IniWriteValue("PreScrap", "group", C_group);
                ini.IniWriteValue("PreScrap", "station", C_StationName);

                Option option = new Option(this);
                this.Hide();
                option.ShowDialog();
            }


        }
        private void ReportError(string Message)
        {
            StackFrame CallStack = new StackFrame(1, true);
            MessageBox.Show("Error: " + Message + ",\n\rFile: " + CallStack.GetFileName() + ", Line: " + CallStack.GetFileLineNumber(), "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void Txt_Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_DataStation();
        }

        private async void Get_DataStation()
        {
            sql = " SELECT LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME FROM SFIS1.C_STATION_CONFIG_T  ";
            sql = sql + " WHERE GROUP_NAME = '" + Txt_Group.Text + "' ORDER BY LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME   ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                C_line = result.Data["line_name"]?.ToString() ?? "";
                C_section = result.Data["section_name"]?.ToString() ?? "";
                C_group = result.Data["group_name"]?.ToString() ?? "";
                C_StationName = result.Data["station_name"]?.ToString() ?? "";
            }
        }
            
        private async Task<string> Check_Privilege(string TMPSTR)
        {
           if(Txt_Group.Text.Substring(0,2) == "R_")
           {
                sql = " SELECT COUNT(A.EMP_NO) COUNT2   FROM SFIS1.C_EMP_DESC_T A, SFIS1.C_EMP_2_GROUP_T B  ";
                sql = sql + "  WHERE A.EMP_RANK <> '0' AND  A.EMP_NO = B.EMP_NO AND A.EMP_BC = '"+TxT_Pass.Text+"'  ";
                sql = sql + "  AND (B.GROUP_NAME = '"+Txt_Group.Text+"'  or B.GROUP_NAME = 'ALL' OR B.GROUP_NAME ='REPAIR') ";
           }
           else
           {
                sql = "  SELECT COUNT(A.EMP_NO) COUNT2 FROM SFIS1.C_EMP_DESC_T A, SFIS1.C_EMP_2_GROUP_T B  WHERE A.EMP_RANK = '0'  ";
                sql = sql + "  AND A.EMP_NO = B.EMP_NO AND A.EMP_BC = '" + TxT_Pass.Text + "'  AND (B.GROUP_NAME = '" + Txt_Group.Text + "' OR B.GROUP_NAME ='ALL' )  ";
           }

            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if( (result.Data == null) || Int32.Parse(result.Data["count2"].ToString()) == 0 )
            {
                showMessage("EMP not have premission in Group: (" + Txt_Group.Text + " or REPAIR or ALL) and EMP_RANK<>0 !\nC_EMP_2_GROUP_T", "Mã thẻ không có quyền trong nhóm: (" + Txt_Group.Text + " or REPAIR or ALL ) and EMP_RANK<>0 !", true);
                Txt_Group.Focus();
                return "FALSE";
            }
            return "OK";
        }
        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
