using Newtonsoft.Json;
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

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for RfidForm.xaml
    /// </summary>
    public partial class RfidForm : Window
    {
        public string[] _RESArray = { "NULL" };
        MainWindow frmMain = new MainWindow();
        public RfidForm()
        {
            InitializeComponent();
            txtRFIDNO.Focus();
        }

        private async void txtRFIDNO_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtRFIDNO.Text))
                {
                    if (!await LINKRFID(txtRFIDNO.Text))
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Data null or white sprace:", "Error", MessageBoxButton.OK);
                    txtRFIDNO.SelectAll();
                    txtRFIDNO.Focus();
                    return;
                }
                this.Close();
            }
        }

        private async Task<bool> LINKRFID(string rfid_no)
        {
            var logInfo = new
            {
                OPTION = "RFID_LABEL",
                MODELNAME = MainWindow.MODEL_NAME,
                MCARTON_NO = MainWindow.MCARTON_NO,
                RFIDNO = rfid_no
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

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
            if (result.Data != null)
            {
                dynamic ads = result.Data;
                string _RES = ads[0]["output"];
                _RESArray = _RES.Split('#');

                if (_RESArray[0] != "OK")
                {
                    MessageError FrmMessage = new MessageError();
                    string _erro = _RESArray[1];
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageEnglish = string.Format("Link RFID label erro: " + _erro);
                    FrmMessage.MessageVietNam = string.Format("Link RFID label lỗi: " + _erro);
                    FrmMessage.ShowDialog();
                    return false;
                }
            }
            return true;
        }

        private void RfidName_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
