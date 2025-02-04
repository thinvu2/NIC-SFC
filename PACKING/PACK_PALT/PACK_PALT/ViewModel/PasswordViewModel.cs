using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sfc.Library.HttpClient;
using PACK_PALT.ViewModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Sfc.Core.Parameters;

namespace PACK_PALT.ViewModel
{
    public class PasswordViewModel : BaseViewModel
    {
        public SfcHttpClient sfcHttpClient;
        private string _txtpassword { get; set; }
        public string permission { get; set; }
        public string txtpassword { get { return _txtpassword; } set { _txtpassword = value; OnPropertyChanged(txtpassword); } }
        public ICommand txtpasswordCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public PasswordViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, p => { });
            //txtpasswordCommand = new RelayCommand<PasswordBox>((p) => { return true; }, p => { CheckPassword(p.ToString()); });

        }
        public async void CheckPassword(string p)
        {
            if (p == null) return;



            Window _wdMain = new Window();
            
            if (string.IsNullOrWhiteSpace(_txtpassword))
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.httpclient = sfcHttpClient;
                _sh.CustomFlag = true;
                _sh.MessageEnglish = "Data input wrong!!";
                _sh.MessageVietNam = "Dữ liệu nhập vào không hợp lệ";
                _sh.ShowDialog();
                return;
            }
            
            var _data = new
                {
                    TYPE = "PERMISSION",
                    PRG_NAME = "PACK_PALT",
                    DATA = _txtpassword,
                    PERMISSION = permission
                };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXECUTE ",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    if (_RESArray[1] == "REPRINT")
                    {
                        ReprintForm _rf = new ReprintForm();
                        var _setreprint = _rf.DataContext as ReprintPalletViewModel;
                        _setreprint.sfcHttpClient = sfcHttpClient;
                        _wdMain.Close();
                        
                        _rf.ShowDialog();
                        
                    }
                    return;

                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _msf = new ShowMessageForm();
                        _msf.httpclient = this.sfcHttpClient;
                        _msf.errorcode = _RESArray[1];
                        _msf.ShowDialog();
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