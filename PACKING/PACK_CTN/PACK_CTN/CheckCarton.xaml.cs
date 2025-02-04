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


namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CheckCarton : Window
    {
        private MainWindow main = new MainWindow();
        public string[] _RESArray = { "NULL" };
        public CheckCarton()
        {
            InitializeComponent();
        }

        private void txtCartonNo_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtShippingSN.Focus();
                var bc = new BrushConverter();
                lbMess.Background = (Brush)bc.ConvertFrom("#FF8DEA58");
                lblMess.Content = "TEST";
                lblMess.Foreground = Brushes.OrangeRed;
            }
        }

        private async void txtShippingSN_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var logInfo = new
                {
                    OPTION = "CHECKCARTON",
                    CARTON_NO = txtCartonNo.Text,
                    SHIPPING_SN = txtShippingSN.Text 
                };

                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
                try
                {
                    var result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                        {
                                new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                        }
                    });
                    dynamic ads = result.Data;
                    string _RES = ads[0]["output"];
                    _RESArray = _RES.Split('#');

                    if (_RESArray[0] == "PASSED")
                    {
                        lstSN.Items.Add(_RESArray[1].ToString());
                        tbMess.Text = _RESArray[2].ToString();
                        lblMess.Content = "PASSED";
                        var bc = new BrushConverter();
                        tbMess.Foreground = Brushes.Blue;
                        lbMess.Background = (Brush)bc.ConvertFrom("#0889a6");
                        lblMess.Foreground = Brushes.Blue;
                        txtShippingSN.SelectAll();
                        txtShippingSN.Focus();
                    }
                    else if (_RESArray[0] == "PASS")
                    {
                        lstSN.Items.Add(_RESArray[1].ToString());
                        tbMess.Text = _RESArray[2].ToString();
                        lblMess.Content = "PASS";
                        var bc = new BrushConverter();
                        tbMess.Foreground = Brushes.Blue;
                        lbMess.Background = (Brush)bc.ConvertFrom("#0889a6");
                        lblMess.Foreground = Brushes.Blue;
                        txtShippingSN.SelectAll();
                        txtShippingSN.Focus();
                    }
                    else if  (_RESArray[0] == "NG")
                    {
                        tbMess.Text = _RESArray[2].ToString();
                        lblMess.Content = "FAIL";
                        var bc = new BrushConverter();
                        tbMess.Foreground = Brushes.Red;
                        lbMess.Background = Brushes.Red;
                        lblMess.Foreground = Brushes.White;
                        txtShippingSN.SelectAll();
                        txtShippingSN.Focus();
                    }
                }
                catch (Exception ex)
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = ex.Message.ToString();
                    FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                    FrmMessage.ShowDialog();
                    txtShippingSN.SelectAll();
                    txtShippingSN.Focus();
                    return;
                }
            }
        }

        private void CheckCartonName_Loaded(object sender, RoutedEventArgs e)
        {
            txtShippingSN.Text = "";
            txtCartonNo.Text = "";
            txtCartonNo.Focus();
        }
    }
}
