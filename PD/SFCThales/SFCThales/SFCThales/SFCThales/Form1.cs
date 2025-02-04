using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CHECK_MSL;
using MSL_PRINT;
using GemaltoRoast;
using SFC_Library;
using Sfc.Library.HttpClient;
using System.Text.RegularExpressions;
using Sfc.Core.Parameters;
using Newtonsoft.Json;

namespace SFCThales
{
    public partial class Form1 : Form
    {
        public EmployeeInfomation empInfo = new EmployeeInfomation();
        public OracleClientDBHelper db = null;
        public static string inputLogin, checkSum, baseUrl, loginDB, empNo, empPass, error_code, CK_DB;
        public static SfcHttpClient sfcHttpClient;
        public Form1()
        {
            InitializeComponent();
            lblVersion.Text = "Version: " + Application.ProductVersion;
            AccessAPI();
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

            if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            {
                //MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButtons.OK);
                //Environment.Exit(0);
            }
            sfcHttpClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
            try
            {
                await sfcHttpClient.GetAccessTokenAsync(empNo, empPass);
                DataTable dt;
                dt = await ExcuteSelectSQL("select b.EMP_no, b.emp_name, b.EMP_RANK from SFIS1.C_PRIVILEGE a, SFIS1.C_EMP_DESC_T b where a.emp =b.emp_no and PRG_NAME ='SFCTHALES' and emp ='" + empNo+"'", sfcHttpClient);

                if (dt.Rows.Count > 0)
                    {
                        GemaltoRoast.MainForm.empno = dt.Rows[0]["EMP_no"].ToString();
                        GemaltoRoast.MainForm.empname = dt.Rows[0]["EMP_name"].ToString();
                    GemaltoRoast.MainForm.emprank = dt.Rows[0]["EMP_RANK"].ToString();
                    MSL_PRINT.MSL_PRINT.EMP_NO = dt.Rows[0]["EMP_no"].ToString();
                    CHECK_MSL.CHECK_MSL.empno_msl = dt.Rows[0]["EMP_no"].ToString();
                }
                    else
                    {
                        SFCMessage.Show("Check Privilege", "You're not privilege to use this program.", "Bạn chưa được cấp quyền để sử dụng chương trình này." + Environment.NewLine + "PRG: SFCTHALES, FUN LOGIN", MessageBoxButtons.OK, MessageBoxDefaultButton.Button1, MessageBoxIconType.Stop);
                        Environment.Exit(0);
                    }
                
                this.WindowState = FormWindowState.Normal;
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

        private void button3_Click(object sender, EventArgs e)
        {
            CHECK_MSL.CHECK_MSL checkmsl = new CHECK_MSL.CHECK_MSL();
            checkmsl.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GemaltoRoast.MainForm login = new GemaltoRoast.MainForm();
            login.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MSL_PRINT.MSL_PRINT mm = new MSL_PRINT.MSL_PRINT();
            mm.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
        public async Task<DataTable> ExcuteSelectSQL(string sql, SfcHttpClient sfcHttpClient)
        {
            DataTable data;
            data = null;
            try
            {
                var datacust = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });


                if (datacust.Data != null)
                {
                    var vardatatabel = JsonConvert.SerializeObject(datacust.Data);
                    data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return data;
        }
    }
}
