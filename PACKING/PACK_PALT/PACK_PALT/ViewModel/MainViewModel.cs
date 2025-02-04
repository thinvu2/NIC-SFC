using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System.IO;
using LabelManager2;
using System.Diagnostics;
using PACK_PALT.Resource;

namespace PACK_PALT.ViewModel
{
    public class MainViewModel :BaseViewModel 
    {
        private static SfcHttpClient _sfcHttpClient;
        private const string baseUrl = "http://10.224.44.162:8080/Help";
        public bool Isloaded = false;
        //public string Password  {get => _Password; set { _Password = value; OnPropertyChanged(); } }
        public bool _CartonLabelCheck { get; set; }
        public bool CartonLabelCheck { get { return _CartonLabelCheck; } set { _CartonLabelCheck = value;OnPropertyChanged("CartonLabelCheck"); } }
        public bool _SsnLabelCheck { get; set; }
        public bool SsnLabelCheck { get { return _SsnLabelCheck; } set { _SsnLabelCheck = value;OnPropertyChanged("SsnLabelCheck"); } }
        public bool _PageQtyCheck { get; set; }
        public bool PageQtyCheck { get { return _PageQtyCheck; }set { _PageQtyCheck = value;OnPropertyChanged("PageQtyCheck"); } }
        public bool _A4LabelCheck { get; set; }
        public bool A4LabelCheck { get { return _A4LabelCheck; } set { _A4LabelCheck = value;OnPropertyChanged("A4LabelCheck"); } }

        public bool _Pallet_Label3Check { get; set; }
        public bool Pallet_Label3Check { get { return _Pallet_Label3Check; } set { _Pallet_Label3Check = value;OnPropertyChanged("Pallet_Label3Check"); } }
        public bool _Pallet_Label4Check { get; set; }
        public bool Pallet_Label4Check { get { return _Pallet_Label4Check; } set { _Pallet_Label4Check = value;OnPropertyChanged("Pallet_Label4Check"); } }
        public bool _Ambit_labelCheck { get; set; }
        public bool Ambit_labelCheck { get { return _Ambit_labelCheck; } set { _Ambit_labelCheck = value;OnPropertyChanged("Ambit_labelCheck"); } }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand ClosepalletCommand { get; set; }
        public ICommand ButtomClosePalletCommand { get; set; }
        public ICommand ReprintCommand { get; set; }
        public ICommand ClosedWindowCommand { get; set; }
        public ICommand ShowParamCommand { get; set; }
        public ICommand VisibleLabelCommand { get; set; }
        public ICommand Pallet_label3Commad { get; set; }
        public ICommand Ambit_labelCommad { get; set; }
        public ICommand Label_QtyCommad { get; set; }
        public MainViewModel()
        {
            //RunnerAsync();
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => {
                Isloaded = true;
                if (p == null)
                    return;
                p.Hide();

                }
              );
            
            ClosepalletCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { ShowClosePallet(p); });
            ReprintCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { ShowReprintlabel(p); });
            ShowParamCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { ShowParam(p); });
            VisibleLabelCommand = new RelayCommand<Window>(p => { return true; }, p => { VisibleLabel(p); });
            //Pallet_label3Commad = new RelayCommand<Window>(p => { return true; }, p => { loadfileini(p); });
            //Ambit_labelCommad = new RelayCommand<Window>(p => { return true; }, p => { loadfileini(p); });
            Label_QtyCommad = new RelayCommand<Window>(p => { return true; }, p => { ShowQtyPallet(p); });
            ClosedWindowCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { CloseWindow(p); });
        }
        private void ShowQtyPallet(Window p)
        {
            if (p == null) return;
            LabelQty _lq = new LabelQty(MainWindow.strINIPath);
            //_lq.strINIPath = MainWindow.strINIPath;
            _lq.ShowDialog();
        }
        private void loadfileini(Window p)
        {
            if (p == null) return;

            if (_Pallet_Label3Check)
            {
                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "Label3");
                _Ambit_labelCheck = false;
            }
            if (_Ambit_labelCheck)
            {
                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK3", "Pallet Type", "Ambit");
                _Pallet_Label3Check = false;
            }
        }
        private async void CloseWindow(Window p)
        {
            //Kill all process before print
            await killprocess();
            ReprintForm _rep = new ReprintForm();
            var _getrep = _rep.DataContext as ReprintPalletViewModel;
            if (File.Exists(MainWindow.FilePath))
            {
                try
                {
                    File.Delete(MainWindow.FilePath);
                }
                catch (Exception )
                {
                    List<Process> lstProcs = new List<Process>();
                    lstProcs = FileUtil.WhoIsLocking(MainWindow.LabelName);

                    foreach (Process pr in lstProcs)
                    {
                        try
                        {
                            ProcessHandler.localProcessKill(pr.ProcessName);
                        }
                        catch (Exception)
                        {


                        }
                    }
                    try
                    {
                        File.Delete(MainWindow.FilePath);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            if (File.Exists(_getrep.FilePath))
            {
                try
                {
                    File.Delete(_getrep.FilePath);
                }
                catch (Exception)
                {
                    List<Process> lstProcs = new List<Process>();
                    lstProcs = FileUtil.WhoIsLocking(_getrep.LabelName);

                    foreach (Process pr in lstProcs)
                    {
                        try
                        {
                            ProcessHandler.localProcessKill(pr.ProcessName);
                        }
                        catch (Exception)
                        {
                            

                        }
                    }
                    try
                    {
                        File.Delete(_getrep.FilePath);
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
            Environment.Exit(0);
        }
        private void ShowParam(Window p)
        {
            if (p == null)
                return;
            ShowParam _sp = new ShowParam();
            ReprintForm _rep = new ReprintForm();
            _sp.dataGrid.DataContext = MainWindow.dtParams.DefaultView;

            if (_sp.DataContext == null)
            {
                
                    return;
                
            }
            var _spVM = _sp.DataContext as ShowParamViewModel;
            _spVM.URLLabel = MainWindow.UrlLabelFile + MainWindow.LabelName;
            if (string.IsNullOrWhiteSpace(_spVM.URLLabel))
            {
                var getrep = _rep.DataContext as ReprintPalletViewModel;
                _sp.dataGrid.DataContext = getrep.dtParam.DefaultView;
                _spVM.URLLabel = getrep.UrlLabelFile + getrep.LabelName;
            }
            _sp.ShowDialog();
        }
        private void VisibleLabel(Window p)
        {
            ReprintForm _rep = new ReprintForm();
            var _getrep = _rep.DataContext as ReprintPalletViewModel;
            if (File.Exists(MainWindow.FilePath))
            {
                ApplicationClass labApp = new ApplicationClass();
                try
                {
                    labApp.Documents.Open(MainWindow.FilePath, false);
                    Document doc = labApp.ActiveDocument;
                    doc.Application.Visible = true;
                }
                catch (Exception ex)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Mở file label có lỗi ngoại lệ: " + ex.Message;
                    _sh.MessageEnglish = "Have exception when open label file: " + ex.Message;
                    _sh.ShowDialog();
                }
            }
            else if (File.Exists(_getrep.FilePath))
            {
                ApplicationClass labApp = new ApplicationClass();
                try
                {
                    labApp.Documents.Open(_getrep.FilePath, false);
                    Document doc = labApp.ActiveDocument;
                    doc.Application.Visible = true;
                }
                catch (Exception ex)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Mở file label có lỗi ngoại lệ: " + ex.Message;
                    _sh.MessageEnglish = "Have exception when open label file: " + ex.Message;
                    _sh.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Can not find Label file!", "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private async Task<bool> killprocess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcessesByName("lppa"))
            {
                process.Kill();
            }
            return true;
        }
        public void ShowReprintlabel(Window p)
        {
            if (p == null) return;
            MainWindow.UrlLabelFile = "";
            MainWindow.FilePath = "";
            MainWindow.LabelName = "";
            PasswordForm _pf = new PasswordForm();
            _pf.sfcHttpClient = MainWindow._sfcHttpClient;
            _pf.permission = "REPRINT";
            _pf.ShowDialog();
             
        }
        public void ShowClosePallet(Window p)
        {
            if (p == null) { return; }
            ClosePaletForm clp = new ClosePaletForm();
            var _setcloseVM = clp.DataContext as ClosePalletViewModel;
            _setcloseVM.sfcHttpClient = MainWindow._sfcHttpClient;
            clp.ShowDialog();
        }
       

    }

}
