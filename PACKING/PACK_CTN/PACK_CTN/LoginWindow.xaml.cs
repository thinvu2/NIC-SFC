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


namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        MainWindow frmMain = new MainWindow();
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOK_Click(sender, e);
            }
        }

        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.CheckLogin = false;
            Process.GetCurrentProcess().Kill();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.ToString() != null)
            {
                login();
            }
        }
        public async void login()
        {
            try
            {
                var result = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " select * from SFIS1.c_emp_desc_t where emp_pass = '" + PasswordBox.Password.ToString() + "' and EMP_RANK <> '3' ",
                    SfcCommandType = SfcCommandType.Text
                });

                if (result.Data != null)
                {
                    MainWindow.empNo = result.Data["emp_no"].ToString();
                    MainWindow.empPass = result.Data["emp_pass"].ToString();
                    MainWindow.EMP_NAME = result.Data["emp_name"].ToString();
                    MainWindow.CheckLogin = true;
                    this.Close();
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "EMP NOT EXITS ";
                    FrmMessage.MessageEnglish = "Tài khoản không đúng hoặc không tồn tại !";
                    FrmMessage.ShowDialog();
                    PasswordBox.Clear();
                    PasswordBox.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                PasswordBox.Clear();
                PasswordBox.Focus();
            }
        }

        private void LoginUC_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordBox.Clear();
            PasswordBox.Focus();
            MainWindow.CheckLogin = false;

        }
    }
}
