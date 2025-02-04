using Oracle;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADD_RWMAC
{
    public partial class flogin : Form
    {
       // public DB _oracle;
        public string inputLogin, checkSum, baseUrl, empNo, empPass, error_code, CK_DB;
        public static string empNO;
        public static string empName;
        public static string empBC;
        public static string loginDB;
        public static SfcHttpClient _sfcHttpClient;

        private void flogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("ADD_RWMAC"))
            {
                process.Kill();
            }
        }

        public flogin()
        {
            InitializeComponent();
            lblVersion.Text = " Version: " + Application.ProductVersion;
            sfcconnect sfcconnec = new sfcconnect();
            connect();


        }
        private async void flogin_Load(object sender, EventArgs e)
        {
            
        }
        public async Task connect()
        {
            string[] Args = Environment.GetCommandLineArgs();
            if (Args.Length == 1)
            {
                //MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
                //Environment.Exit(0);
            }
            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }
            sfcconnect conn = new sfcconnect();
            _sfcHttpClient = await conn.AccessAPI();
            if (_sfcHttpClient.AutoRefreshToken == null)
            {
                MessageBox.Show("Connect SFC filed! plz call IT");
                Application.Exit();
            }
            string sql = "select*from SFISM4.AMS_AP where ap_name = 'ADD_RWMAC' and AP_VERSION ='" + Application.ProductVersion + "'";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data == null)
            {
                MessageBox.Show("Plz Update new program");
                Application.Exit();
            }
        }

        private void txt_emp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txt_emp.Text != null || txt_emp.Text != "")
                {
                    txt_pass.SelectAll();
                    txt_pass.Focus();
                }
            }
        }

        private void txt_pass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txt_emp.Text != null || txt_emp.Text != null)
                {
                    btn_login_Click(sender, e);
                }
            }
        }
        private async void btn_login_Click(object sender, EventArgs e)
        {
            if (txt_emp.Text == "" || txt_emp.Text =="PD" || txt_pass.Text == "" || txt_pass.Text == "PD")
            {
                showMessage("Bạn phải sử dụng mã thẻ và mật khẩu để đăng nhập !", "You must use the card code and password to login !", true);
                return;
            }
            string sql = " SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO ='" + txt_emp.Text + "' AND EMP_BC='" + txt_pass.Text + "' AND QUIT_DATE > SYSDATE ";
            var result = await _sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
          
            if (result.Data == null)
            {
                showMessage("You're not privilege to use this program !\n\nQUIT_DATE < SYSDATE", "Bạn không có quyền sử dụng chương trình này !\nLiên hệ IT-SFC để được hỗ trợ !", true);
                return;
            }
            else
            {
                empName = result.Data["emp_name"]?.ToString() ?? "";
                empNO = result.Data["emp_no"]?.ToString() ?? "";
                empBC = result.Data["emp_bc"]?.ToString() ?? "";
                Main_Prog main = new Main_Prog();
                this.Visible = false;
                main.Show();
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
    }
}
