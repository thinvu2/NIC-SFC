using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ShippingFile

{
    public partial class LoginForm : Form
    {
        DAL dal;

        private string checkSum;
        public static SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUri = "";
        public string loginDB = "";
        public static string empNo = "", PCIP, PCMAC, APVersion;
        public string empPass = "";
        public string Plant = "";
        public string inputLogin = "";
        public string sEmpName = "";
        public static string UrlLabelFile = "";
        public LoginForm()
        {
            InitializeComponent();
            dal = new DAL();
            string[] Args = Environment.GetCommandLineArgs();

            if (Args.Length == 1)
            {
                MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING");
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
            APVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //    Environment.Exit(0);
            //}
            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "ShippingFile",
                UserName = empNo,
                Password = empPass
            };
            //Tranform it to Json object
            ApiConnect(loginInfo);
        }
        public async void ApiConnect(object loginInfo)
        {
            sfcClient = new SfcHttpClient(loginApiUri, loginDB, "helloApp", "123456");
            await sfcClient.GetAccessTokenAsync(empNo, empPass);
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            try
            {
                var result1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                });

                dynamic ads = result1.Data;
                string Ok = ads[0]["output"];

                if (Ok.Substring(0, 2) != "OK")
                {
                    MessageBox.Show(Ok);
                    this.Close();
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
                this.Close();
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           // String str = "Username"
            
        }

        public void RemoveText(object sender, EventArgs e)
        {
            if (this.textboxUsername.Text == "Username")
            {
                this.textboxUsername.Text = "";
            }
            

        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textboxUsername.Text))
                this.textboxUsername.Text = "Username";

        }

        public void RemoveTextPass(object sender, EventArgs e)
        {
            if (this.textboxPassword.Text == "Password")
            {
                this.textboxPassword.Text = "";
            }

        }

        public void AddTextPass(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textboxPassword.Text))
                this.textboxPassword.Text = "Password";
        }

        private async void pictureboxLogin_Click(object sender, EventArgs e)
        {
            DAL fDal = new DAL();
            if (textboxUsername.Text.Equals("Username") || textboxUsername.Text.Equals(""))
            {
                MessageBox.Show("Username is null! Please enter Username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textboxUsername.Focus();
                return;
            }
            if (textboxPassword.Text.Equals("Password") || textboxPassword.Text.Equals(""))
            {
                MessageBox.Show("Password is null! Please enter Password","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textboxPassword.Focus();
                return;
            }
            else
            {
                string result = string.Empty;
                result = await fDal.CheckLogin(textboxUsername.Text.Trim(), textboxPassword.Text.Trim(), sfcClient);

                if (result != null)
                {
                    string[] login = result.Split('-');
                    fDal.loginNO = login[0];
                    fDal.loginName = login[1];
                    fDal.userRank = login[2];

                    if (fDal.userRank.Contains("9"))
                    {
                        ShipingFile shippingFileForm = new ShipingFile(sfcClient);
                        this.Visible = false;
                        shippingFileForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("Emp have not privilege!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        textboxPassword.Focus();
                    }

                }
                else
                {
                    MessageBox.Show("No emp or emp has expired！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textboxPassword.Text = "";
                    textboxPassword.Focus();
                }
            }
            
        }


        private void CheckLogin()
        {
            //验证版本信息
            string preName, preVersion;
            preName = this.ProductName;
            preVersion = this.ProductVersion;

            /*UpdVersion uv = new UpdVersion();
            bool result = uv.CheckVersion(preName, preVersion);
            if (result == true)
            {
                if (DialogResult.Yes == MessageBox.Show("使用版本" + preVersion + "低於系統版本,是否需要更新?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    string path = Environment.CurrentDirectory+"\\AutoUpdate\\AutoUpdate.exe";
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(path);
                    System.Diagnostics.Process.Start(psi);
                    Application.Exit();
                }
            }*/
        }

        private void textboxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                pictureboxLogin_Click(sender, e);
            }
        }

        private void textboxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                pictureboxLogin_Click(sender, e);
            }
        }

        private  void textboxPassword_TextChanged(object sender, EventArgs e)
        {
            

        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
           
        }

        //public async Task<SfcHttpClient> connec()
        //{
        //    sfcconnect sfc = new sfcconnect();

        //    sfcClient = await sfc.AccessAPI();
        //    return sfcClient;
        //}
       

    }
}
