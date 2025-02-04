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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for CheckPrivilege.xaml
    /// </summary>
    public partial class CheckPrivilege : Window
    {
        public CheckPrivilege()
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

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password.ToString() != null)
            {
                Privilege();
            }
        }
        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public async void Privilege()
        {
            try
            {
                var result = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = " SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC ='" + PasswordBox.Password.ToString() + "' AND CLASS_NAME LIKE '%V%'",
                    SfcCommandType = SfcCommandType.Text
                });

                if (result.Data != null)
                {
                    MainWindow.CheckPri = true;
                    this.Close();
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Tài khoản không có quyền visible! ";
                    FrmMessage.MessageEnglish = "EMP NOT PRIVILEGE VISIBLE";
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

        private void CheckPrivilegeUC_Loaded(object sender, RoutedEventArgs e)
        {
            PasswordBox.Clear();
            PasswordBox.Focus();
            MainWindow.CheckPri = false;
        }
    }
}
