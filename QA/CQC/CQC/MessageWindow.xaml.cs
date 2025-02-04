using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
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

namespace CQC
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        SfcHttpClient httpclient;
        string errorcode = "";
        public bool CustomFlag = false;
        public string MessageVietNam = "";
        public string MessageEnglish = "";
        public MessageWindow()
        {
            InitializeComponent();
        }
        public MessageWindow(SfcHttpClient _httpclient, string _errorcode)
        {
            httpclient = _httpclient;
            errorcode = _errorcode;
            InitializeComponent();
        }
        private async void Window_Initialized(object sender, EventArgs e)
        {
            if (CustomFlag)
            {
                txbenglish.Text = MessageEnglish;
                txbvietnamese.Text = MessageVietNam;
            }
            else
            {
                if (httpclient != null){
                    var result = await httpclient.QuerySingleAsync(new QuerySingleParameterModel
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
                        txbenglish.Text = errorcode + " - " + result.Data["prompt_english"].ToString();
                        txbvietnamese.Text = errorcode + " - " + result.Data["prompt_chinese"].ToString();
                    }
                }
            }
            passwordBox.Password = string.Empty;
            passwordBox.Focus();
            passwordBox.SelectAll();
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.Return)
            {
                if (passwordBox.Password == "9999")
                {
                    this.Close();
                }
            }
        }

        //private void btnOK_Click(object sender, RoutedEventArgs e)
        //{
        //    if (passwordBox.Password == "9999")
        //    {
        //        this.Close();
        //    }
        //    else
        //    {
        //        MessageBox.Show("Error !!", "mtInformation",MessageBoxButton.OK);
        //        passwordBox.Focus();
        //        passwordBox.SelectAll();
        //    }
        //}
    }
}
