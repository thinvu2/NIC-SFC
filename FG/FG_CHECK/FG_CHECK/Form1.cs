using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SFC_Library;
using Sfc.Library.HttpClient;
using Microsoft.VisualBasic;
using Sfc.Core.Parameters;
using System.Text.RegularExpressions;
using System.IO;

namespace FG_CHECK
{
    public partial class Form1 : Form
    {
        public static SfcHttpClient sfcClient;
        public static string empName;
        public static string empNO;
        public static string lang = "";
        public string inputLogin, checkSum, baseUrl, loginDB, empNo, empPass, error_code, CK_DB;
        public Form1()
        {
            InitializeComponent();
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
            sfcClient = new SfcHttpClient(baseUrl, loginDB, "helloApp", "123456");
            try
            {
                await sfcClient.GetAccessTokenAsync(empNo, empPass);
                string sql = string.Format(@"select*from SFIS1.C_PRIVILEGE A,  SFIS1.C_EMP_DESC_T B where A.FUN ='LOGIN' and A.PRG_NAME = 'FG_CHECK' AND A.EMP = B.EMP_NO AND emp_no = '{0}' AND ROWNUM <2", empNo);
                var qry_Data_emp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Data_emp.Data == null)
                {
                    MessageBox.Show("Không có quyền hạn");
                }
                else
                {
                    empName = qry_Data_emp.Data["emp_name"]?.ToString() ?? "";
                    empNO = qry_Data_emp.Data["emp_no"]?.ToString() ?? "";
                    btnShippinglable.Visible = true;
                    btnShipping_Notice.Visible = true;
                    btnChecIN.Visible = true;
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
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        LabelManager2.Application labApp = null;
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                killprocess();
                labApp = new LabelManager2.Application();
                String _DirPath = System.Windows.Forms.Application.StartupPath + @"\";
                foreach (string sFile in System.IO.Directory.GetFiles(_DirPath, "*.LAB"))
                {
                    File.Delete(sFile);
                }
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                Environment.Exit(0);
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
        private void btnShipping_Notice_Click(object sender, EventArgs e)
        {
            fMainNotice fMain = new fMainNotice(sfcClient);
            fMain.ShowDialog();
        }

        private void btnShippinglable_Click(object sender, EventArgs e)
        {
            fMainShippingLabel fMain = new fMainShippingLabel(sfcClient);
            fMain.ShowDialog();
        }

        private void btnCheckin_Click(object sender, EventArgs e)
        {
            fMainInOutRevert fMain = new fMainInOutRevert(sfcClient);
            fMain.ShowDialog();
        }
        private void btnFQA_Click(object sender, EventArgs e)
        {
            FFQA fmain = new FFQA();
            fmain.ShowDialog();
        }
    }
}

