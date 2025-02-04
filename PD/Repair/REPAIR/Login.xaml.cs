using Sfc.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using Newtonsoft.Json;
using MES.OpINI;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        MainWindow frmMain = new MainWindow();
        public SfcHttpClient sfcHttpClient;
        public string[] RESArray = { "NULL" };
        private char charSub = IniUtil.CharSub;
        public Login()
        {
            InitializeComponent();
        }

        private void PasswordBox_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (PasswordBox.Password != "")
                {
                    login();
                }
            }
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != "")
            {
                login();
            }
        }
        public async void login()
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "CHECK_EMP",
                    PASS = PasswordBox.Password.ToString()
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            
                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPAIR_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter {Name = "DATA" ,Value = jsonData, SfcParameterDataType = SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter {Name = "OUTPUT" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                }
                });

                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split(charSub);
                    if (RESArray[0] == "OK")
                    {
                        if (await frmMain.checkPrivilege(RESArray[1], "REPAIR_NEW"))
                        {
                            MainWindow.empNo = RESArray[1].ToString();
                            MainWindow.empName = RESArray[2].ToString();
                            this.Close();
                        }
                        else
                        {
                            MessageError FrmMessage = new MessageError();
                            FrmMessage.CustomFlag = true;
                            FrmMessage.MessageVietNam = "Employee No PRIVILEGE !\n FUN = REPAIR OR GROUP_NAME = REPAIR";
                            FrmMessage.MessageEnglish = "Tài khoản không có quyền !";
                            FrmMessage.ShowDialog();
                            return;
                        }
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "SFIS1.REPAIR_API_EXECUTE/CHECK_EMP have error ";
                        FrmMessage.MessageEnglish = RESArray[1].ToString();
                        FrmMessage.ShowDialog();
                        return;
                    }
                    
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "SFIS1.REPAIR_API_EXECUTE/CHECK_EMP result data null";
                    FrmMessage.MessageEnglish = " Error !";
                    FrmMessage.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CheckLogin = false;
            Process.GetCurrentProcess().Kill();
        }

        private async void LoginUC_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordBox.Password = "";
            PasswordBox.Focus();

        }
    }
}
