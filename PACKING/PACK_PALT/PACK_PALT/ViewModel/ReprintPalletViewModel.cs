using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using PACK_PALT.Model;
using PACK_PALT.ViewModel;
using System.Data;
using LabelManager2;
using PACK_PALT.Resource;
using System.IO;
using System.Diagnostics;
using System.Net;
using Sfc.Library.HttpClient.Helpers;
using System.Windows.Forms;

namespace PACK_PALT.ViewModel
{
    public class ReprintPalletViewModel: BaseViewModel
    {
        public SfcHttpClient sfcHttpClient { get; set; }

        public string strINIPath = "C:\\Windows\\SFIS.ini";
        
        private string mtable { get; set; }
        private string str_sql { get; set; }
        private string ModelName { get; set; }
        public string emp_no { get; set; }
        public string model_type { get; set; }
        public string mo_number { get; set; }
        private bool _ChkMD5Flag = true;
        public string FilePath { get; set; }
        public bool rma_flag { get; set; }
        public string UrlLabelFile { get; set; }
        public string LabelName { get; set; }
        public bool label_pallet_3;
        public bool label_pallet_4;
        public bool a4label;
        public bool label_pallet_3_4;
        private bool _ZWipTrackingCheck;
        public bool ZWipTrackingCheck { get { return _ZWipTrackingCheck; } set { _ZWipTrackingCheck = value; OnPropertyChanged("ZWipTrackingCheck"); } }
        //private bool _ReprintEnable;
        //public bool ReprintEnable { get { return _ReprintEnable; }set { _ReprintEnable = value;OnPropertyChanged("ReprintEnable"); } }

        private string _TxtQtyLable;
        public string TxtQtyLable { get { return _TxtQtyLable; } set { _TxtQtyLable = value;OnPropertyChanged("TxtQtyLable"); } }
        private string _txtdata;
        public string txtdata { get { return _txtdata; }set { _txtdata = value; OnPropertyChanged(txtdata); } }
        public ICommand LabelqtyComboxCommand { get; set; }
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand txt_dataKeyEnterCommand { get; set; }
        public ICommand ClosedWindowCommand { get; set; }
        public ICommand ShowParamCommand { get; set; }
        public ICommand OKButtomCommad { get; set; }
        public ICommand CloseButtomCommad { get; set; }
        public DataTable dtParam = new DataTable();

        public ReprintPalletViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => { return true; }, p => { TxtQtyLable = "1"; });
            //LabelqtyComboxCommand = new RelayCommand<ComboBox>((p) => { return true; }, (p) => { });
            txt_dataKeyEnterCommand = new RelayCommand<System.Windows.Controls.TextBox>((p) => { return true; }, p => { ReprintlabelClass(); p.SelectAll(); p.Focus(); });
            OKButtomCommad = new RelayCommand<Window>(p => { return true; }, p => { ReprintlabelClass(); });
            //ClosedWindowCommand = new RelayCommand<Window>((p) => { return true; }, p => { CloseWindowClass(p); });
            ShowParamCommand = new RelayCommand<Window>((p) => { return true; }, (p) => { ShowParam(p); });
            CloseButtomCommad = new RelayCommand<Window>(p => { return true; }, p => { CloseClass(p); });
        }

        public void CloseClass(Window p)
        {
            if (p == null) return;
            p.Close();
        }
        private void ShowParam(Window p)
        {
            if (p == null)
                return;
            ShowParam _sp = new ShowParam();
            _sp.dataGrid.DataContext = dtParam.DefaultView;
            if (_sp.DataContext == null)
            {
                return;
            }
            var _spVM = _sp.DataContext as ShowParamViewModel;
            _spVM.URLLabel = UrlLabelFile + LabelName;
            _sp.ShowDialog();
        }
        public async void ReprintlabelClass()
        {
            
            string temp = _TxtQtyLable;
            if (string.IsNullOrWhiteSpace(_TxtQtyLable))
            {
                _TxtQtyLable = "1";
            }

            check_file_ini();

            string[] array = { "NA", "N/A" };
            //if (_ReprintCheck)
            //{
            //    mtable = "SFISM4.Z_WIP_TRACKING_T";
            //}
            //else
            //{
            //    mtable = "SFISM4.R_WIP_TRACKING_T";
            //}
            if (string.IsNullOrWhiteSpace(_txtdata)|| array.Contains(_txtdata))
            {
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = sfcHttpClient;
                _smf.CustomFlag = true;
                _smf.MessageEnglish = "Input data is invalid" + _txtdata;
                _smf.MessageVietNam = "Dữ liệu đầu vào không hợp lệ " + _txtdata;
                _smf.ShowDialog();
                return;
            }
            
            if (_txtdata.Contains(":"))
            {
                string[] arr = _txtdata.Split(new char[] { ':' });
                _txtdata = arr[0].ToString();
            }
            
            if (await FindPalletReprint(_txtdata) == false) return;

            bool tempa = model_type.Contains("224");

            MainWindow mw = new MainWindow();
            //var getmainVM = mw.DataContext as MainViewModel;
            string wip_group = "";
            if (_ZWipTrackingCheck)
            {
                wip_group = await CheckWipGroup(_txtdata);
            }



            if (model_type.Contains("224") && _ZWipTrackingCheck && wip_group== "SHIPPING")
            {
                string dn = ShowDialog("INPUT DN", "DN");
                if (string.IsNullOrWhiteSpace(dn))
                {
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.CustomFlag = true;
                    _smf.MessageEnglish = "DN NULL!!";
                    _smf.MessageVietNam = "NHAP DU LIEU RONG!";
                    _smf.ShowDialog();
                    return;
                }
                if (await PrintPalletShipping(dn) == false) return;
            }
            else
            {
                InsertSystemLog("REPRINT", "PALLET OR CARTON:" + _txtdata + " ,IP:" + MainWindow.G_sMyIP);
            }
            //MainWindow main = new MainWindow();
            //main.dtParams = dtParam;
            //main.PrintToCodeSoft("P", ModelName);
            if (string.IsNullOrWhiteSpace(LabelName))
            {
                if (ModelName.IndexOf(".") >= 0)
                {
                    string temp_model_name = ModelName.Replace(".", "_");
                    LabelName = "P_" + temp_model_name + ".LAB";
                }
                
            }
            
            if(mw.loginDB=="ROKU" && !_ZWipTrackingCheck)
            {
                LabelName = "P_" + ModelName + "_3.LAB";
            }
            

            if (label_pallet_3)
                LabelName = "P_" + ModelName + "_3.LAB";
            else if(label_pallet_4)
                LabelName= "P_" + ModelName + "_4.LAB";
             
            if (await FindRmaMO(mo_number) == true)
            {
                LabelName = LabelName.ToUpper();
                int charPos = LabelName.IndexOf(".LAB");
                LabelName = LabelName.Replace(LabelName.Substring(charPos), "_RMA.LAB");
            }

            if (a4label)
            {
                PageA4LabelPrint();
            }else if (label_pallet_3_4)
            {
                LabelName = "P_" + ModelName + "_3.LAB";
                 PrintToCodeSoft("P");
                LabelName = "P_" + ModelName + "_4.LAB";
                 PrintToCodeSoft("P");
            }
            else
            {
                
                 PrintToCodeSoft("P");
            }
            
            

            txtdata = "";
            ZWipTrackingCheck = false;
            label_pallet_3 = false;
            label_pallet_4 = false;
            a4label = false;
        }
        public async Task<bool> FindRmaMO(string mo_number)
        {
            

            var _data = new
            {
                TYPE = "FINDRMAMO",
                PRG_NAME = "PACK_PALT",
                MO_NUMBER = mo_number
            };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
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
                if (_RESArray[0] == "TRUE")
                {
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "FAIL")
                    {
                        return  false;
                    }
                    else
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = sfcHttpClient;
                        _smf.CustomFlag = true;
                        _smf.MessageVietNam = _RESArray[0].ToString();
                        _smf.MessageEnglish = "procedure have exceptions:";
                        _smf.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = sfcHttpClient;
                _smf.CustomFlag = true;
                _smf.MessageVietNam = ex.Message;
                _smf.MessageEnglish = "Call procedure have exceptions:";
                _smf.ShowDialog();
                return false;
            }
        }
        public async Task<string> CheckWipGroup(string imei)
        {
            string wip_group = "";
            str_sql = $"SELECT DISTINCT WIP_GROUP FROM SFISM4.Z107 WHERE IMEI='{imei}' ";
            var _result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = str_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (_result.Data.Count() > 0)
            {
                wip_group = _result.Data["wip_group"].ToString();
            }
            return wip_group;
        }
        private string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 200,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                TopMost = true
            };
            System.Windows.Forms.Label textLabel = new System.Windows.Forms.Label() { Left = 50, Top = 30,  Text = text, Dock = DockStyle.Top,Size= new System.Drawing.Size(56, 19),
              Font=  new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Margin = new System.Windows.Forms.Padding(5, 0, 5, 0)
            };
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox() { Left = 20, Top = 30, Width = 150 ,
                Font = new System.Drawing.Font("Cambria", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
            Size= new System.Drawing.Size(150, 26) };
            System.Windows.Forms.Button confirmation = new System.Windows.Forms.Button() { Text = "Ok", Left = 70, Width = 70, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
        private async void InsertSystemLog(string action_type,string action_desc)
        {
            var _data = new
            {
                TYPE = "INSERTLOG",
                PRG_NAME = "PACK_PALT",
                EMP_NO = emp_no,
                PROGRAM = "PACK_PALT",
                ACTION_TYPE= action_type,
                ACTION_DESC= action_desc
            };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
            }
            catch
            {

            }
        }
        private async Task<bool> PrintPalletShipping(string dn)
        {
            try
            {
                var _result_pro = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REPRINTLABEL",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                {
                     new SfcParameter{Name="MYGROUP",Value="PACK_PALT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                     new SfcParameter{Name="DN",Value=dn,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                     new SfcParameter{Name="DATA",Value=_txtdata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                     new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}

                }
                });
                dynamic ads = _result_pro.Data;
                string res = ads[0]["res"];
                if (res.Substring(0, 2) == "OK")
                {
                    string label_name = res.Substring(2, res.Length - 2);
                    str_sql = "SELECT EMP_NO,NVL(MEMO,'N/A') MEMO FROM SFISM4.R_FQA_CHECKLABEL_T WHERE LABEL_NAME='" + label_name + "' ";
                    var _result_param = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = str_sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    dtParam.Rows.Clear();
                    if (_result_param.Data != null)
                    {
                        foreach (var rows in _result_param.Data)
                        {
                            if (!string.IsNullOrEmpty(rows["memo"].ToString()))
                            {
                                AddParams(rows["emp_no"].ToString(), rows["memo"].ToString());
                            }
                        }
                        str_sql = "DELETE  SFISM4.R_FQA_CHECKLABEL_T WHERE LABEL_NAME='" + label_name + "'";
                        try
                        {
                            var _result_delete = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = str_sql,
                                SfcCommandType = SfcCommandType.Text
                            });
                        }
                        catch
                        {

                        }
                        InsertSystemLog("PRINTSHIPPINGLABEL", "PALLET OR CARTON:" + _txtdata + " OF DN: "+dn+" ,IP:" + MainWindow.G_sMyIP);
                    }
                    return true;
                }
                else
                {
                    ShowMessageForm _smf = new ShowMessageForm();
                    _smf.MessageVietNam = res;
                    _smf.MessageEnglish = res;
                    _smf.ShowDialog();
                    return false;
                }

            }
            catch(Exception ex)
            {
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = sfcHttpClient;
                _smf.CustomFlag = true;
                _smf.MessageVietNam = ex.Message;
                _smf.MessageEnglish = "Call procedure have exceptions:";
                _smf.ShowDialog();
                return false;
            }
        }
        public void AddParams(string _name, string _value)
        {
            if (dtParam.Columns.Count == 0)
            {
                dtParam.Columns.Add("Name");
                dtParam.Columns.Add("Value");
            }
            dtParam.Rows.Add(new object[] { _name, _value });
        }
        private async Task<bool> FindPalletReprint(string pallet_no)
        {
            string table_check = "0";
            await CheckExistZ107(_txtdata);
            if (_ZWipTrackingCheck)
            {
                table_check = "1";
            }
            var _data = new
            {
                TYPE = "FINDPALLETREPRINT",
                PRG_NAME = "PACK_PALT",
                IMEI = pallet_no,
                TABLE=table_check
            };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
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
                    string label_name = _RESArray[1].ToString();
                    ModelName = _RESArray[2].ToString();
                    _txtdata = _RESArray[3].ToString();
                    model_type = _RESArray[4].ToString();
                    LabelName = _RESArray[5].ToString();
                    mo_number = _RESArray[6].ToString();
                    str_sql = "SELECT EMP_NO,MEMO FROM SFISM4.R_FQA_CHECKLABEL_T WHERE LABEL_NAME='" + label_name + "' ";
                    var _result_param = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = str_sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    dtParam.Clear();
                    if (_result_param.Data != null)
                    {
                        foreach(var rows in _result_param.Data)
                        {
                            if (!string.IsNullOrEmpty(rows["memo"].ToString()))
                            {
                                AddParams(rows["emp_no"].ToString(), rows["memo"].ToString());
                            }
                        }
                        try
                        {
                            str_sql = "DELETE  SFISM4.R_FQA_CHECKLABEL_T WHERE LABEL_NAME='" + label_name + "'";
                            var _result_delete = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = str_sql,
                                SfcCommandType = SfcCommandType.Text
                            });
                        }
                        catch
                        {

                        }
                    }
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = sfcHttpClient;
                        _smf.errorcode = _RESArray[1].ToString();
                        _smf.ShowDialog();
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _smf = new ShowMessageForm();
                        _smf.httpclient = sfcHttpClient;
                        _smf.CustomFlag = true;
                        _smf.MessageVietNam = _RESArray[0].ToString();
                        _smf.MessageEnglish = "procedure have exceptions:";
                        _smf.ShowDialog();
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                ShowMessageForm _smf = new ShowMessageForm();
                _smf.httpclient = sfcHttpClient;
                _smf.CustomFlag = true;
                _smf.MessageVietNam = ex.Message;
                _smf.MessageEnglish = "Call procedure have exceptions:";
                _smf.ShowDialog();
                return false;
            }
        }
        private async Task<bool> CheckExistZ107(string datainput)
        {
            
            var _data = new
            {
                TYPE = "CHECKEXISTZ107",
                PRG_NAME = "PACK_PALT",
                DATAINPUT = datainput
            };
            ShowMessageForm _msf = new ShowMessageForm();
            _msf.httpclient = this.sfcHttpClient;
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
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
                    if (_RESArray[1] == "ZCHECK")
                    {
                        ZWipTrackingCheck = true;
                    }
                    else
                    {
                        ZWipTrackingCheck = false;
                    }

                }
                else if(_RESArray[0] == "NG")
                {
                    _msf.CustomFlag = true;
                    _msf.MessageVietNam = _RESArray[2];
                    _msf.MessageEnglish = _RESArray[1];
                    _msf.ShowDialog();
                    return false;
                }
            }
            catch(Exception ex)
            {
                _msf.CustomFlag = true;
                _msf.MessageVietNam = ex.Message.ToString();
                _msf.MessageEnglish = "Call procedure have exceptions:";
                _msf.ShowDialog();
                return false;
            }
            return true;
        }

        public bool PrintToCodeSoft(string ParamKind)
        {
             
            //await killprocess();
            string Loc_num = MES.OpINI.IniUtil.ReadINI(strINIPath, ParamKind + "LabelQTY", "Default");
            if (string.IsNullOrWhiteSpace(Loc_num))
            {
                Loc_num = "1";
            }
            
            //string _labelNameBak = ParamKind + "_" + model_name + ".BAK";
            string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\";
            FilePath = _DirPath + LabelName;
            //string FilePathBak = _DirPath + _labelNameBak;
            //remove old file label
            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch (Exception ex)
                {
                    List<Process> lstProcs = new List<Process>();
                    lstProcs = FileUtil.WhoIsLocking(LabelName);

                    foreach (Process p in lstProcs)
                    {
                        try
                        {
                            ProcessHandler.localProcessKill(p.ProcessName);
                        }
                        catch (Exception exc)
                        {
                            string errorMessage = "Cannot kill process " + p.ProcessName + " Exception: " + exc.Message + Environment.NewLine + "Please Close program and try again";
                            ShowMessageForm _sh = new ShowMessageForm();
                            _sh.CustomFlag = true;
                            _sh.MessageVietNam = "Không thể xóa đóng process { 0}. Lỗi ngoại lệ: { 1} Hãy đóng chương trình và thử lại ";
                            _sh.MessageEnglish = errorMessage + ex.Message;
                            _sh.ShowDialog();
                            return false;

                        }
                    }
                    try
                    {
                        File.Delete(FilePath);
                    }
                    catch (Exception ex_e)
                    {
                        string errorMessage = "Cannot delete file " + FilePath + " Exception: " + ex_e.Message;
                        //MessageBoxHelper.Display(errorMessage);
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = string.Format("Không thể xóa file {0}. Lỗi ngoại lệ: {1}", FilePath, ex_e.Message);
                        _sh.MessageEnglish = string.Format("Cannot delete file {0}. Exception: {1}", FilePath, ex_e.Message);
                        _sh.ShowDialog();
                        return false;
                    }

                }
            }
            //down file label
            if (!File.Exists(FilePath))
            {
                try
                {
                    WebClient wc = new WebClient();
                    UrlLabelFile =  GetUrlLabelFile();
                    wc.DownloadFile(UrlLabelFile + LabelName, FilePath);
                }
                catch (Exception exc)
                {
                    if (exc.Message.Equals("The remote server returned an error: (404) Not Found."))
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "Không tìm thấy file label, gọi Labelroom kiểm tra.";
                        _sh.MessageEnglish = "Label file not found, call Labelroom check." + Environment.NewLine + "Url: " + Environment.NewLine + UrlLabelFile + LabelName;
                        _sh.ShowDialog();
                        return false;
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = "Không tìm thấy file label, gọi Labelroom kiểm tra.";
                        _sh.MessageEnglish = "Can not download label file. Url: " + UrlLabelFile + exc.Message;
                        _sh.ShowDialog();
                        return false;
                    }
                }
            }

            //ApplicationClass LabApp = new ApplicationClass();
            //LabApp.Documents.Open(FilePath, false);
            //Document doc = LabApp.ActiveDocument;

            bool _chkprint = CallCodesoftToPrint(/*doc,*/ LabelName, dtParam);
            if (!_chkprint)
            {
                return false;
            }
            return true;
        }
        public bool CallCodesoftToPrint(/*Document doctemp,*/ string LabelName, DataTable dtParams)
        {
            string _param_Name = "";

            try
            {
                LabelTracking _lbt = new LabelTracking();
                _lbt.sfcHttpClient = sfcHttpClient;
                //Call labelfile and print
                ApplicationClass LabApp = new ApplicationClass();
                LabApp.Documents.Open(FilePath, false);
                Document doctemp = LabApp.ActiveDocument;
                //Check MD5 of label file
                if (_ChkMD5Flag)
                {
                    if (! _lbt.doMD5Label(LabelName, 4, true))
                    {
                        return false;
                    }
                    _ChkMD5Flag = false;
                }

                // Set value into label file
                foreach (DataRow param in dtParams.Rows)
                {
                    _param_Name = param["Name"].ToString();
                    try
                    {
                        doctemp.Variables.FormVariables.Item(_param_Name).CounterUse = 0;
                        doctemp.Variables.FormVariables.Item(_param_Name).Value = param["Value"].ToString();
                    }
                    catch
                    { }

                }

                //THEM DOAN NAY
                try
                {
                    doctemp.Save();
                    doctemp.Close();
                    LabApp.Documents.Open(FilePath, false);
                    doctemp = LabApp.ActiveDocument;
                    /*if (!await _lbt.doLabelTracking_AddParamValue_1(doctemp, LabelName, 4, dtParams, true))
                    {
                        return false;
                    }*/
                }
                catch (Exception ex)
                {
                    ShowMessageForm _sh = new ShowMessageForm();
                    _sh.CustomFlag = true;
                    _sh.MessageVietNam = "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message;
                    _sh.MessageEnglish = "Check label tracking have exception: " + ex.Message;
                    _sh.ShowDialog();
                    LabApp.Documents.CloseAll();
                    LabApp.Quit();
                    doctemp = null;
                    LabApp = null;
                    return false;
                }
                doctemp.PrintDocument(Int32.Parse(_TxtQtyLable));
                doctemp.FormFeed();
                LabApp.Documents.CloseAll();
                LabApp.Quit();
                doctemp = null;
                LabApp = null;
                //END 

                // Get value of label items to compare with label tracking
                //DataTable dtbVarName = new DataTable();
                //dtbVarName.Columns.Add("VAR_NAME", typeof(string));
                //dtbVarName.Columns.Add("VALUE", typeof(string));
                //dtbVarName.Columns.Add("TYPE", typeof(string));
                //int TotalBarcode = doctemp.DocObjects.Barcodes.Count;
                //int TotalText = doctemp.DocObjects.Texts.Count;
                //int TotalVar = doctemp.Variables.FormVariables.Count;
                //int TotalFreeVar = doctemp.Variables.FreeVariables.Count;
                //int TotalFomula = doctemp.Variables.Formulas.Count;

                //for (int i = 1; i <= TotalText; i++)
                //{
                //    var _name = doctemp.DocObjects.Texts.Item(i).Name;
                //    if (_name != null)
                //    {
                //        var _var = doctemp.DocObjects.Texts.Item(i).VariableName;
                //        var _data = doctemp.DocObjects.Texts.Item(i).Value;
                //        var _font = doctemp.DocObjects.Texts.Item(i).Font.Name;
                //        dtbVarName.Rows.Add(new object[] { _name, _data, _font });
                //    }
                //}

                //for (int i = 1; i <= TotalBarcode; i++)
                //{
                //    var _name = doctemp.DocObjects.Barcodes.Item(i).Name;
                //    if (_name != null)
                //    {
                //        var _var = doctemp.DocObjects.Barcodes.Item(i).VariableName;
                //        var _bar = doctemp.DocObjects.Barcodes.Item(i).Symbology.ToString().Replace("lppx", "").Trim();
                //        var _data = doctemp.DocObjects.Barcodes.Item(i).Value;
                //        dtbVarName.Rows.Add(new object[] { _name, _data, _bar });
                //    }
                //}
                ////Call label tracking

                //    try
                //    {
                //        if (!await _lbt.doLabelTracking_AddParamValue(LabelName, 4, dtParams, doctemp, dtbVarName, true))
                //        {
                //            return false;
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        ShowMessageForm _sh = new ShowMessageForm();
                //        _sh.CustomFlag = true;
                //        _sh.MessageVietNam = "Kiểm tra label tracking có lỗi ngoại lệ: " + ex.Message;
                //        _sh.MessageEnglish = "Check label tracking have exception: " + ex.Message;
                //        _sh.ShowDialog();
                //        return false;
                //    }

                //doctemp.PrintDocument(Int32.Parse(_TxtQtyLable));
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = "Phát sinh lỗi khi in label: " + ex.Message;
                _sh.MessageEnglish = "Have exceptions when pint document: " + ex.Message;
                _sh.ShowDialog();
                return false;
            }
            return true;
        }
        public string GetUrlLabelFile()
        {
            var _data = new
            {
                TYPE = "GETURL",
                PRG_NAME = "PACK_PALT"
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result =  sfcHttpClient.Execute(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_PALT_API_EXE_10",
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
                    return "http://" + _RESArray[1] + "/";
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        return "";
                    }
                    else
                    {
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error:";
                        _sh.ShowDialog();
                        return "";
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
                return "";
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
        private void check_file_ini()
        {
            string temp = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "Pallet Type");
            if (temp == "A4page")
                a4label = true;
            else if (temp == "Label3")
                label_pallet_3 = true;
            else if (temp == "Label4")
                label_pallet_4 = true;
            else if (temp == "Label3_4")
                label_pallet_3_4 = true;

        }
        public async void PageA4LabelPrint()
        {
            int pageSum = 0;
            bool PRINTOTHERFLAG;
            string tmpssn = "";
            MainWindow mw = new MainWindow();
            var getmainVM = mw.DataContext as MainViewModel;
            if (!string.IsNullOrWhiteSpace(MES.OpINI.IniUtil.ReadINI(strINIPath, "PALLETPRINT", "PAGEQTY")))
                pageSum = Int32.Parse(MES.OpINI.IniUtil.ReadINI(strINIPath, "PALLETPRINT", "PAGEQTY"));

            if (_ZWipTrackingCheck)
            {
                mtable = "SFISM4.Z_WIP_TRACKING_T";
            }
            else
            {
                mtable = "SFISM4.R_WIP_TRACKING_T";
            }
            string query_sql = "SELECT COUNT(*) QTY FROM " + mtable + " WHERE IMEI='" + _txtdata + "'";

            var _result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            int sQTY = Int32.Parse(_result.Data["qty"].ToString());
            query_sql = "sELECT COUNT(DISTINCT serial_number) QTY  FROM " + mtable + " WHERE IMEI='" + _txtdata + "'";
            var _result_count = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            int sQTY_SN = Int32.Parse(_result_count.Data["qty"].ToString());
            query_sql = "SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME= "
                + "SELECT MODEL_NAME FROM " + mtable + " WHERE IMEI='" + _txtdata + "' AND ROWNUM=1) AND "
                + " VERSION_CODE=(SELECT VERSION_CODE FROM '" + mtable + "' WHERE IMEI=:SN AND ROWNUM=1) ";
            var _result_cust = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            string Modeldesc = _result_cust.Data["cust_model_desc"].ToString();
            string M_MODEL_NAME = _result_count.Data["model_name"].ToString();
            query_sql = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE "
                + "PRG_NAME='" + M_MODEL_NAME + "' AND VR_CLASS='LOT_PRINT' AND VR_ITEM='PALTLIST' AND ROWNUM=1";
            var _result_parameter = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = query_sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (_result_parameter.Data != null)
            {
                PRINTOTHERFLAG = true;
                tmpssn = _result_parameter.Data["vr_name"].ToString();
            }
            else
            {
                PRINTOTHERFLAG = false;
            }
            if (getmainVM._SsnLabelCheck)
            {
                if (PRINTOTHERFLAG)
                {
                    if (_ZWipTrackingCheck)
                    {
                        query_sql = "select a.serial_number,a.pallet_no,a.track_no,b.ssn4 SHIPPING_SN,a.MODEL_NAME,b.mac1,b.ssn3 from sfism4.z_wip_tracking_t a,sfism4.r_custsn_t b"
                            + " where a.imei='" + _txtdata + "' and a.serial_number=b.serial_number";
                    }
                    else
                    {
                        query_sql = "select a.serial_number,a.pallet_no,a.track_no,b.ssn4 SHIPPING_SN,a.MODEL_NAME,b.mac1,b.ssn3 from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b"
                            + " where a.imei='" + _txtdata + "' and a.serial_number=b.serial_number";
                    }
                }
                else
                {
                    if (_ZWipTrackingCheck)
                    {
                        query_sql = "select distinct a.serial_number, a.SHIPPING_SN,a.model_name,a.pallet_no,a.track_no,b.mac1,b.ssn3 from sfism4.z_wip_tracking_t a,sfism4.r_custsn_t b "
                            + " where imei='" + _txtdata + "' and a.serial_number=b.serial_number ";
                    }
                    else
                    { 
                        query_sql = "select distinct a.serial_number, a.SHIPPING_SN,a.model_name,a.pallet_no,a.track_no,b.mac1,b.ssn3 from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b "
                            + " where imei='" + _txtdata + "' and a.serial_number=b.serial_number ";
                    }
                }
                var _result_wip_track = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = query_sql,
                    SfcCommandType = SfcCommandType.Text
                });
                ResouceClass rc = new ResouceClass();
                DataTable dt = new DataTable();
                if (_result_wip_track.Data != null)
                {
                    dtParam.Clear();
                    dt = rc.ToDataTable<List_infomation_pallet>(_result_wip_track.Data.ToListObject<List_infomation_pallet>());
                    AddParams("AMBIT_PALLET_NO", dt.Rows[0]["pallet_no"].ToString());
                    AddParams("CUST_PALLET_NO", _txtdata);
                    AddParams("SN_QTY", sQTY.ToString());
                    AddParams("PMODELDESC", Modeldesc);
                    AddParams("MODELNAME", M_MODEL_NAME);
                    AddParams("P_NO", dt.Rows[0]["track_no"].ToString());
                    AddParams("PAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                }
                else return;

                if (pageSum == 0)
                {
                    pageSum = sQTY_SN;
                }
                
                int allqty = 0;
                int tmppage = 0;
                int IntX = 0;
                //foreach(var row in _result_wip_track.Data)
                //{
                //    IntX = IntX + 1;
                //    allqty = allqty + 1;
                //    AddParams("MSN" + IntX, row["shipping_sn"].ToString());
                //    AddParams("PMSN" + IntX, row["serial_number"].ToString());
                //    AddParams("MAC" + IntX, row["mac1"].ToString());
                //    AddParams("MSNK", row["ssn3"].ToString());
                //    if(IntX==pageSum || allqty == sQTY)
                //    {
                //        tmppage = tmppage + 1;
                //        AddParams("ALLPAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                //        AddParams("PAGEQTY", tmppage.ToString());
                //        LabelName = "P_" + M_MODEL_NAME + "_PMSN_4.LAB";
                //         PrintToCodeSoft("P");
                //        dtParam.Clear();
                //        IntX = 0;
                //    }
                //}
            }
            else if (getmainVM._CartonLabelCheck)
            {
                if (PRINTOTHERFLAG)
                {
                    if (_ZWipTrackingCheck)
                    {
                        query_sql = "select distinct a.MCARTON_NO,a.model_name,a.pallet_no,a.track_no from sfism4.z_wip_tracking_t a,sfism4.r_custsn_t b"
                            + " where a.imei='" + _txtdata + "' and a.serial_number=b.serial_number";
                    }
                    else
                    {
                        query_sql = "select distinct a.MCARTON_NO,a.model_name,a.pallet_no,a.track_no from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b"
                            + " where a.imei='" + _txtdata + "' and a.serial_number=b.serial_number";
                    }
                }
                else
                {
                    if (_ZWipTrackingCheck)
                    {
                        query_sql = "select distinct a.MCARTON_NO,a.model_name,a.pallet_no,a.track_no from sfism4.z_wip_tracking_t a,sfism4.r_custsn_t b "
                            + " where imei='" + _txtdata + "' and a.serial_number=b.serial_number ";
                    }
                    else
                    {
                        query_sql = "select distinct a.MCARTON_NO,a.model_name,a.pallet_no,a.track_no from sfism4.r_wip_tracking_t a,sfism4.r_custsn_t b "
                            + " where imei='" + _txtdata + "' and a.serial_number=b.serial_number ";
                    }
                }
                var _result_wip_track = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = query_sql,
                    SfcCommandType = SfcCommandType.Text
                });
                ResouceClass rc = new ResouceClass();
                DataTable dt = new DataTable();
                if (_result_wip_track.Data != null)
                {
                    dtParam.Clear();
                    dt = rc.ToDataTable<List_info_carton_check>(_result_wip_track.Data.ToListObject<List_info_carton_check>());
                    AddParams("AMBIT_PALLET_NO", dt.Rows[0]["pallet_no"].ToString());
                    AddParams("CUST_PALLET_NO", _txtdata);
                    AddParams("SN_QTY", sQTY.ToString());
                    AddParams("PMODELDESC", Modeldesc);
                    AddParams("MODELNAME", M_MODEL_NAME);
                    AddParams("P_NO", dt.Rows[0]["track_no"].ToString());
                    AddParams("PAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                }
                else return;

                if (pageSum == 0)
                {
                    pageSum = sQTY_SN;
                }

                int allqty = 0;
                int tmppage = 0;
                int IntX = 0;
                foreach (var row in _result_wip_track.Data)
                {
                    IntX = IntX + 1;
                    allqty = allqty + 1;
                    AddParams("MCTN" + IntX, row["mcarton_no"].ToString());
                    if (IntX == pageSum || allqty == sQTY)
                    {
                        tmppage = tmppage + 1;
                        AddParams("ALLPAGE", Math.Ceiling(sQTY / (float)pageSum).ToString());
                        AddParams("PAGEQTY", tmppage.ToString());
                        LabelName = "P_" + M_MODEL_NAME + "_PMSN_4.LAB";
                        PrintToCodeSoft("P");
                        dtParam.Clear();
                        IntX = 0;
                    }
                }
            }
        }
    }
}
