using Sfc.Core.Parameters;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace REWORK.UserControlRework
{
    /// <summary>
    /// Interaction logic for ucLogin.xaml
    /// </summary>
    public partial class ucLogin : UserControl
    {
        MainWindow frmMain = new MainWindow();
        public ucLogin()
        {
            InitializeComponent();
        }

        private void password_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (PasswordBox.Password != "")
                {
                    Login();
                }
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != "")
            {
                Login();
            }
        }
        public async void Login ()
        {
            try
            {
                var result = await MainWindow.sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " select * from SFIS1.c_emp_desc_t where emp_pass = '"+PasswordBox.Password+"' and EMP_RANK <> '3' ",
                    SfcCommandType = SfcCommandType.Text
                });

                if (result.Data != null)
                {
                    MainWindow.empNo = result.Data["emp_no"].ToString();
                    MainWindow.empPass = result.Data["emp_pass"].ToString();
                    frmMain.lblEmpName.Content = result.Data["emp_name"];
                    frmMain.itemNameEmp.Content = "Hi: " + result.Data["emp_name"];
                    frmMain.panelMain.Children.Clear();
                    frmMain.panelMain.Children.Add(new ucMainForm());
                    return;
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "EMP NOT EXITS ";
                    FrmMessage.MessageEnglish = "Tài khoản không đúng hoặc không tồn tại !";
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
  
        }
    }
}
