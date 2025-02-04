using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PACK_BOX
{
    /// <summary>
    /// Interaction logic for ShowMessageForm.xaml
    /// </summary>
    public partial class ShowMessageForm : Window
    {
        public MainWindow frm_main;
        public SfcHttpClient sfcClient;
        public string errorcode = "";
        public bool CustomFlag = false;
        public string MessageVietNam = "";
        public string MessageEnglish = "";
        private DispatcherTimer dispatcherTimer;
        public ShowMessageForm()
        {
            InitializeComponent();
        }
        public ShowMessageForm(MainWindow _frm_main, SfcHttpClient _sfcClient)
        {
            frm_main = _frm_main;
            sfcClient = _sfcClient;
            InitializeComponent();
        }
        public async void ShowMessageForm1_Loaded(object sender, RoutedEventArgs e)
        {
            if (CustomFlag)
            {
                txbenglish.Text = MessageEnglish;
                txbvietnamese.Text = MessageVietNam;
            }
            else
            {
                string strGetError = "select * from SFIS1.C_PROMPT_CODE_T where PROMPT_CODE =:errorcode";
                var qry_Error = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetError,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "errorcode", Value = errorcode }
                    }
                });

                if (qry_Error.Data != null)
                {
                    txbenglish.Text = errorcode + " - " + qry_Error.Data["prompt_english"].ToString();
                    txbvietnamese.Text = errorcode + " - " + qry_Error.Data["prompt_chinese"].ToString();
                }
            }
            passwordBox.Focus();
            passwordBox.SelectAll();

            // Timer to Close App
            if (frm_main != null)
            {
                if (frm_main.item_automation.IsChecked == true)
                {
                    dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();
                }
            }
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
        public void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (frm_main.item_automation.IsChecked == true)
            {
                this.Close();
            }
        }
    }
}
