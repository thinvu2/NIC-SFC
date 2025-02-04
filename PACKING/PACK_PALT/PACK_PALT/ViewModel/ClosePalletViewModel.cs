using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using PACK_PALT.Model;
using PACK_PALT.ViewModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace PACK_PALT.ViewModel
{
    public class ClosePalletViewModel:BaseViewModel
    {
        private string _txtCartonorPallet;
        public string txtCartonorPallet { get { return _txtCartonorPallet; } set { _txtCartonorPallet = value; OnPropertyChanged(txtCartonorPallet); } }
        public SfcHttpClient sfcHttpClient { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand editdataKeyEnterCommand { get; set; }
        public ICommand OKButtomCommad { get; set; }
        public ClosePalletViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, p => { });
            editdataKeyEnterCommand = new RelayCommand<TextBox>((p) => { return true;  }, (p) => {  ClosePalletClass(p.ToString());p.SelectAll();p.Focus(); });
            OKButtomCommad = new RelayCommand<Window>(p => { return true; }, p => { ClosePalletClass(); });
        }
        private async void ClosePalletClass(string p)
        {
            
            if (string.IsNullOrWhiteSpace(_txtCartonorPallet))
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.httpclient = sfcHttpClient;
                _sh.CustomFlag = true;
                _sh.MessageEnglish = " Please input carton or pallet";
                _sh.MessageVietNam = "Nhập mã carton hoặc pallet";
                _sh.ShowDialog();
                return;
            }
            ClosePalletAll(_txtCartonorPallet);
            txtCartonorPallet = "";
        }
        private async void ClosePalletClass()
        {
            if (string.IsNullOrWhiteSpace(_txtCartonorPallet))
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.httpclient = sfcHttpClient;
                _sh.CustomFlag = true;
                _sh.MessageEnglish = " Please input carton or pallet";
                _sh.MessageVietNam = "Nhập mã carton hoặc pallet";
                _sh.ShowDialog();
                return;
            }
            ClosePalletAll(_txtCartonorPallet);
            txtCartonorPallet = "";
        }
        private async void ClosePalletAll(string data)
        {
            data = data.ToUpper();
            var _data = new
            {
                TYPE = "CLOSEPALLET",
                PRG_NAME = "PACK_PALT",
                PALLET_NO= data
            };
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
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
                    MessageBox.Show("THIS PALLET NO ALREADY CLOSED!!");
                    txtCartonorPallet = "";
                    return;
                    
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _msf = new ShowMessageForm();
                        _msf.httpclient = this.sfcHttpClient;
                        _msf.CustomFlag = true;
                        _msf.MessageVietNam = _RESArray[2];
                        _msf.MessageEnglish = _RESArray[1];
                        _msf.ShowDialog();
                        return;
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error: CLOSEPALLET";
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
                _sh.MessageEnglish = "Call procedure have exceptions: CLOSEPALLET";
                _sh.ShowDialog();
                return;
            }
        }
    }
}
