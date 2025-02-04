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
using System.Windows.Shapes;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System.Net;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for MessageError.xaml
    /// </summary>
    public partial class MessageError : Window
    {
        public string errorcode = "";
        public bool CustomFlag = false;
        public string MessageVietNam = "";
        public string MessageEnglish = "";
        public SfcHttpClient sfcHttpClient;

        public MessageError()
        {
            InitializeComponent();
        }
        private async void MessageError_Loaded(object sender, RoutedEventArgs e)
        {
            password.Focus();
            password.Password = "";

            if (CustomFlag)
            {
                lblEnglish.Text = MessageEnglish;
                lblVietNamese.Text = MessageVietNam;
            }
            else
            {
                var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "select*from SFIS1.C_PROMPT_CODE_T where PROMPT_CODE =:errorcode",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "errorcode", Value = errorcode }
                    }
                });

                if (result.Data != null)
                {
                    lblEnglish.Text = errorcode + " - " + result.Data["prompt_english"].ToString();
                    lblVietNamese.Text = errorcode + " - " + result.Data["prompt_chinese"].ToString();
                }
            }
            Label_IP.Content = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        }
        private void btnOK_click(object sender, RoutedEventArgs e)
        {
            MessageFrm.Close();
        }
        private void txtPassword_keyup(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_click(sender, e);
            }
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (password.Password == "9999")
                {
                    btnOK_click(sender,e);
                }
                else
                {
                    MessageBox.Show("Nhập 9999 để thoát !!" ,"Error" , MessageBoxButton.OK,MessageBoxImage.Information);
                    return;
                }
            }
        }
    }
}
