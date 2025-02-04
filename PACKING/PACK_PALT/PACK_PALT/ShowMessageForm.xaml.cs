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

namespace PACK_PALT
{
    /// <summary>
    /// Interaction logic for ShowMessageForm.xaml
    /// </summary>
    public partial class ShowMessageForm : Window
    {
        public SfcHttpClient httpclient;
        public string errorcode = "";
        public bool CustomFlag = false;
        public string MessageVietNam = "";
        public string MessageEnglish = "";
        public ShowMessageForm()
        {
            InitializeComponent();
        }

        private async void ShowMessageForm1_Loaded(object sender, RoutedEventArgs e)
        {
            if (CustomFlag)
            {
                txbenglish.Text = MessageEnglish;
                txbvietnamese.Text = MessageVietNam;
            }
            else
            {
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

            passwordBox.Focus();
            passwordBox.SelectAll();
            
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                if (passwordBox.Password == "9999")
                {
                    this.Close();
                }
            }
        }
    }
}
