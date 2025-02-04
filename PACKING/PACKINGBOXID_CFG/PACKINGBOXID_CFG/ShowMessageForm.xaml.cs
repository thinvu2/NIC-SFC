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

namespace PACKINGBOXID_CFG
{
    /// <summary>
    /// Interaction logic for ShowMessageForm.xaml
    /// </summary>
    public partial class ShowMessageForm : Window
    {
        public SfcHttpClient sfcClient;
        public string errorcode = "";
        public bool CustomFlag = false;
        public string MessageVietNam = "";
        public string MessageEnglish = "";
        public ShowMessageForm()
        {
            InitializeComponent();
        }

        public ShowMessageForm(SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            InitializeComponent();
        }
        public async void ShowMessageForm1_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.sendtoCom("NG,"+ MessageEnglish);
            if (CustomFlag)
            {
                txbenglish.Text = MessageEnglish;
                txbvietnamese.Text = MessageVietNam;
            }
            else
            {
                string sSql_str = "select * from SFIS1.C_PROMPT_CODE_T where PROMPT_CODE =:errorcode";
                var query_error = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sSql_str,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "errorcode", Value = errorcode }
                    }
                });

                if (query_error.Data != null)
                {
                    txbenglish.Text = errorcode + " - " + query_error.Data["prompt_english"].ToString();
                    txbvietnamese.Text = errorcode + " - " + query_error.Data["prompt_chinese"].ToString();
                }
            }

            passwordBox.Focus();
            passwordBox.SelectAll();

        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                passwordBox.SelectAll();
                if (passwordBox.Password == "9999")
                {
                    ShowMessageForm1.Close();
                }
            }
        }
    }
}
