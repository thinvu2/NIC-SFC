using PACKINGBOXID_CFG.Model;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PACKINGBOXID_CFG
{
    /// <summary>
    /// Interaction logic for FormReprint.xaml
    /// </summary>
    public partial class FormReprint : Window
    {
        public DataTable dtParams_Reprint = new DataTable();
        SfcHttpClient sfcClient;
        public string pubftppath, labellocation;
        public string C19_CustModelName, M_sCustModelName, M_sCustCarton, M_sUPCEANData, M_sCustModelDesc;
        public int My_CartonQtyOnDB;
        public bool locationlabel;
        MainWindow formPackBox_Cfg;
        public FormReprint(MainWindow main, SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            this.formPackBox_Cfg = main;
            sfcClient = _sfcClient;
            //-----------
            Cb_CE.IsChecked = false;
            Cb_Address.IsChecked = false;
            Edt_Count.Text = "1";
            Edt_SN.Text = "";
            Edt_SN.Focus();
            //---------------
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Rbtn_SSN.IsChecked = false;
            Cb_CE.IsChecked = false;
            Cb_Address.IsChecked = false;
            ClearParams();
            Edt_Count.Text = "1";
            Edt_SN.Text = "";
            Edt_SN.Focus();
        }

        private void Edt_SN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Reprint();
            }
        }

        private void Rbtn_BOXID_Click(object sender, RoutedEventArgs e)
        {
            Cb_CE.IsChecked = false;
            Cb_Address.IsChecked = false;
            Edt_Count.Text = "1";
            Edt_SN.Text = "";
            Edt_SN.Focus();
        }

        private async void Reprint()
        {
            string sCartonNo;

            if ((Rbtn_SSN.IsChecked == false) && (Rbtn_BOXID.IsChecked == false) && (Rbtn_CustBOXID.IsChecked == false))
            {
                formPackBox_Cfg.lbError.Text = "00415 - " + await formPackBox_Cfg.GetPubMessageVN("00415");
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "00415 - " + await formPackBox_Cfg.GetPubMessageVN("00415");
                _er.MessageEnglish = "00415 - " + await formPackBox_Cfg.GetPubMessage("00415");
                _er.ShowDialog();
                Rbtn_SSN.Focus();
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(Edt_SN.Text))
                {
                    formPackBox_Cfg.lbError.Text = "00416 - " + await formPackBox_Cfg.GetPubMessageVN("00416");
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "00416 - " + await formPackBox_Cfg.GetPubMessageVN("00416");
                    _er.MessageEnglish = "00416 - " + await formPackBox_Cfg.GetPubMessage("00416");
                    _er.ShowDialog();
                    Edt_SN.Focus();
                    Edt_SN.SelectAll();
                    return;
                }
                else
                {
                    ClearParams();
                    sCartonNo = "";
                    string strGetDataWIP = "select * from sfism4.r_wip_tracking_t"
                        + " where (serial_number = :sSerN"
                        + " or  shipping_SN = :sSerN  or  shipping_SN2 = :sSerN or tray_no = :sSerN) and rownum = 1";
                    var qry_DataWIP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetDataWIP,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="sSerN",Value=Edt_SN.Text}
                        }
                    });
                    if (qry_DataWIP.Data != null)
                    {
                        Edt_SN.Text = qry_DataWIP.Data["tray_no"].ToString();
                        if (qry_DataWIP.Data["tray_no"].ToString() == "N/A")
                        {
                            formPackBox_Cfg.lbError.Text = "00417 - " + await formPackBox_Cfg.GetPubMessageVN("00417");
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "00417 - " + await formPackBox_Cfg.GetPubMessageVN("00417");
                            _er.MessageEnglish = "00417 - " + await formPackBox_Cfg.GetPubMessage("00417");
                            _er.ShowDialog();
                            Edt_SN.Focus();
                            Edt_SN.SelectAll();
                            return;
                        }
                        if (qry_DataWIP.Data["tray_no"].ToString() == "")
                        {
                            formPackBox_Cfg.lbError.Text = "00417 - " + await formPackBox_Cfg.GetPubMessageVN("00417");
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageVietNam = "00417 - " + await formPackBox_Cfg.GetPubMessageVN("00417");
                            _er.MessageEnglish = "00417 - " + await formPackBox_Cfg.GetPubMessage("00417");
                            _er.ShowDialog();
                            Edt_SN.Focus();
                            return;
                        }
                    }
                    else
                    {
                        formPackBox_Cfg.lbError.Text = "00414 - " + await formPackBox_Cfg.GetPubMessageVN("00414");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "00414 - " + await formPackBox_Cfg.GetPubMessageVN("00414");
                        _er.MessageEnglish = "00414 - " + await formPackBox_Cfg.GetPubMessage("00414");
                        _er.ShowDialog();
                        Edt_SN.Focus();
                        Edt_SN.SelectAll();
                        return;
                    }
                    sCartonNo = qry_DataWIP.Data["tray_no"].ToString();
                    formPackBox_Cfg.Edt_model.Text = qry_DataWIP.Data["model_name"].ToString();
                    if (await FindRPrint(sCartonNo) == false)
                    {
                        Edt_SN.SelectAll();
                        return;
                    }

                    if ((formPackBox_Cfg.Edt_BOXID.Text != sCartonNo) && (formPackBox_Cfg.Edt_BOXID.Text.Trim() != ""))
                    {
                        formPackBox_Cfg.lbError.Text = "00418 - " + await formPackBox_Cfg.GetPubMessageVN("00418");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "00418 - " + await formPackBox_Cfg.GetPubMessageVN("00418");
                        _er.MessageEnglish = "00418 - " + await formPackBox_Cfg.GetPubMessage("00418");
                        _er.ShowDialog();
                        return;
                    }

                    string strGetPallet = "select * from sfis1.c_pallet_t"
                        + " where PALLET_NO = '" + sCartonNo + "'";
                    var qry_Pallet = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetPallet,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Pallet.Data.Count() != 0)
                    {
                        formPackBox_Cfg.lbError.Text = "00419 - " + await formPackBox_Cfg.GetPubMessageVN("00419");
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "00419 - " + await formPackBox_Cfg.GetPubMessageVN("00419");
                        _er.MessageEnglish = "00419 - " + await formPackBox_Cfg.GetPubMessage("00419");
                        _er.ShowDialog();
                        return;
                    }

                    formPackBox_Cfg.PrintToCodeSoft();
                    Item_Name.Items.Clear();
                    Item_Data.Items.Clear();
                }
                Edt_SN.SelectAll();
                Edt_SN.Focus();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.formPackBox_Cfg.item_Reprint.IsChecked = false;
            this.Close();
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            Reprint();
        }

        private async Task<bool> FindRPrint(string _paramBoxID)
        {
            string sCustCode, sModelName, sVersion, sCarton;
            string SMO_NUMBER, strack_no;
            int ipos, i, BOXQTY, TRAYQTY;

            string indata = "PCMAC:" + formPackBox_Cfg.PCMAC.Replace(":", "").Replace("-", "").Trim() + "|PCIP:" + formPackBox_Cfg.PCIP + "|BOXID:" + _paramBoxID;
            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_PackingBoxID",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="AP_VER",Value=formPackBox_Cfg.ap_version,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="MYGROUP",Value=formPackBox_Cfg.M_sThisGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="LINE",Value=formPackBox_Cfg.M_sThisLineName,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_EMP",Value=formPackBox_Cfg.empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_FUNC",Value="REPRINT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=indata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
            });
            dynamic _ads = result.Data;
            string _RES = _ads[1]["res"];
            if (_RES.StartsWith("OK"))
            {
                var strOut = _ads[0]["out_data"];
                formPackBox_Cfg.dtParams.Clear();
                dtParams_Reprint.Clear();
                foreach (var rows in strOut.Split('|'))
                {
                    AddParams(rows.Split(':')[0].ToString() ?? "", rows.Split(':')[1].ToString() ?? "");
                }
                if(formPackBox_Cfg.PrintSN.IsChecked==true)
                {
                    string sql = "select serial_number from sfism4.r107 where tray_no='" + _paramBoxID + "'";
                    var SNParam = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (SNParam.Data.Count() != 0)
                    {
                        int k = 1;
                        foreach(var item in SNParam.Data)
                        {
                            dtParams_Reprint.Rows.Add(new object[] { "SN" + k, item["serial_number"].ToString() });
                            k++;
                        }
                    }
                }
                formPackBox_Cfg.LabelName = formPackBox_Cfg.Edt_model.Text.Trim().Replace(".", "_") + "_BOX.LAB";
                formPackBox_Cfg.model_NameBak = formPackBox_Cfg.Edt_model.Text.Trim() + "_BOX";
                formPackBox_Cfg.dtParams = dtParams_Reprint;
                formPackBox_Cfg.Count_ReprintLabel = Int32.Parse(Edt_Count.Text);
                return true;
            }
            else
            {
                formPackBox_Cfg.lbError.Text = _RES.Split('|')[0].ToString();
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = _RES.Split('|')[1].ToString();
                _er.MessageEnglish = _RES.Split('|')[0].ToString();
                _er.ShowDialog();
                return false;
            }
        }
        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            ClearParams();
            this.formPackBox_Cfg.item_Reprint.IsChecked = false;
            this.Close();
        }
        private void AddParams(string _name, string _value)
        {
            if ((_name != null) && (_value != null))
            {
                Item_Name.Items.Add(_name);
                Item_Data.Items.Add(_value);
                if (dtParams_Reprint.Columns.Count == 0)
                {
                    dtParams_Reprint.Columns.Add("Name");
                    dtParams_Reprint.Columns.Add("Value");
                }
                dtParams_Reprint.Rows.Add(new object[] { _name, _value });
            }
        }
        private void ClearParams()
        {
            Item_Name.Items.Clear();
            Item_Data.Items.Clear();
        }
    }
}
