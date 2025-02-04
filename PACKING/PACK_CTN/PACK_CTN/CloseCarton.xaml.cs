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
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PACK_CTN.Models;
using System.Collections.ObjectModel;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for CloseCarton.xaml
    /// </summary>
    public partial class CloseCarton : Window
    {
        private MainWindow _main = new MainWindow();
        public String MainLbCartonCount , CARTON_NO ;
        public string[] _RESArray = { "NULL" };
        public CloseCarton()
        {
            InitializeComponent();
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {

            String ERR = await _main.GetPubMessage("00062");
            MessageBoxResult dlr =   MessageBox.Show (ERR, "WARRING", MessageBoxButton.OKCancel,MessageBoxImage.Question);
            if (dlr  == MessageBoxResult.Cancel)
            {
                return;
            }
            if (InputData.Text.ToString() == "")
            {
                MessageBox.Show("Please inpute serial number.", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                InputData.Focus();
                return;
            }
            else
            {
                if (MainLbCartonCount == "0")
                {
                    String _er = await _main.GetPubMessage("00062");
                    MessageBox.Show( _er , "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    return;
                }
                var logInfo = new
                {
                    OPTION = "F_CLOSE_CARTON" ,
                    DATA  = InputData.Text
                };
                string Jsdata = JsonConvert.SerializeObject(logInfo).ToString();

                var result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value= Jsdata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                });
                if (result.Data != null)
                {
                    dynamic ads = result.Data;
                    string _RES = ads[0]["output"];
                    _RESArray = _RES.Split('#');

                    if (_RESArray[0] == "OK")
                    {
                        CARTON_NO = _RESArray[1];
                        MainWindow.LabelName = _RESArray[3];
                        MainWindow.MODEL_TYPE = _RESArray[4];

                        MessageBox.Show("This Carton NO Already Closed!!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        _main.loadIniFile();
                        InputData.Focus();
                        return;

                    }
                    else
                    {
                        if (_RESArray[1] == "ER")
                        {
                            MessageError FrmMessage = new MessageError();
                            FrmMessage.errorcode = _RESArray[2];
                            FrmMessage.CustomFlag = false;
                            FrmMessage.ShowDialog();
                        }
                        else
                        {
                            MessageError FrmMessage = new MessageError();
                            FrmMessage.MessageEnglish = " Close carton error !! ";
                            FrmMessage.MessageVietNam = _RESArray[2];
                            FrmMessage.CustomFlag = false;
                            FrmMessage.ShowDialog();
                        }
                        
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Data null";
                    FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                    FrmMessage.ShowDialog();
                }
            }
        }

        private void InputData_Keyup(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender, e);
            }
        }
        public ObservableCollection<string> list = new ObservableCollection<string>();
        private void CloseCarton_Loaded(object sender, RoutedEventArgs e)
        {
            int i;
            for (i = 1; i <= 20; i++)
            {
                //foreach (string s in str)
                list.Add(i.ToString());
                // cbbLabelQty.Items.Add(i.ToString() );
            }
            cbbLbQTY.ItemsSource = list;
            cbbLbQTY.SelectedIndex = 0;
            InputData.Focus();
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }
    }
}
