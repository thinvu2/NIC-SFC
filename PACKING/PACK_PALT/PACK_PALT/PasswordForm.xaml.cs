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
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using PACK_PALT.ViewModel;

namespace PACK_PALT
{
    /// <summary>
    /// Interaction logic for PasswordForm.xaml
    /// </summary>
    public partial class PasswordForm : Window
    {
        public SfcHttpClient sfcHttpClient;
        public string permission { get; set; }
        public string password { get; set; }
        public PasswordForm()
        {
            InitializeComponent();
            txtPassword.Focus();
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                password = txtPassword.Password;
                if (!string.IsNullOrWhiteSpace(password))
                {
                    if(permission== "REPRINT")
                    {
                        CheckReprintClass();
                    }
                    //ReprintForm _rf = new ReprintForm();
                    //this.Close();
                    //_rf.ShowDialog();
                }
            }
        }
        private async void CheckReprintClass()
        {
            var _data = new
            {
                TYPE = permission,
                PRG_NAME = "PACK_PALT",
                EMP_PASS = password
            };
            try
            { 
            string _jsondata= JsonConvert.SerializeObject(_data).ToString();
            var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.PACK_PALT_API_EXECUTE ",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsondata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
            });

            dynamic _ads = _result.Data;
            string _RES = _ads[0]["output"];
            string[] _RESArray = _RES.Split('#');

            if (_RESArray[0] == "OK")
            {
                
                    //ReprintForm _rf = new ReprintForm();

                    //this.Close();
                    //_rf.ShowDialog();
                    ReprintForm _rf = new ReprintForm();
                    var _setreprint = _rf.DataContext as ReprintPalletViewModel;
                    _setreprint.sfcHttpClient = sfcHttpClient;
                    _setreprint.emp_no = _RESArray[1].ToString();
                    this.Close();
                    _rf.ShowDialog();
                return;

            }
            else
            {
                if (_RESArray[0] == "EMP_PASS_NG" || _RESArray[0]== "PERMISSION_NG")
                {
                    ShowMessageForm _msf = new ShowMessageForm();
                    _msf.httpclient = this.sfcHttpClient;
                    _msf.errorcode = _RESArray[1];
                    _msf.ShowDialog();
                        txtPassword.SelectAll();
                        txtPassword.Focus();
                        txtPassword.Password = "";
                    return;
                }
                else
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = _RESArray[0];
                    _sh.MessageEnglish = "Excute procedure have error:";
                    _sh.ShowDialog();
                    return;
                }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions:";
                _sh.ShowDialog();
                return;
            }
        }
    }
}
