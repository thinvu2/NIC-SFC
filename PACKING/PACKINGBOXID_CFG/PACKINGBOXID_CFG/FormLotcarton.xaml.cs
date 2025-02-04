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
using Sfc.Library.HttpClient;
using Sfc.Core.Parameters;
using PACKINGBOXID_CFG.Model;
using Sfc.Library.HttpClient.Helpers;

namespace PACKINGBOXID_CFG
{
    /// <summary>
    /// Interaction logic for FormLotcarton.xaml
    /// </summary>
    public partial class FormLotcarton : Window
    {
        public string M_flag, packqty;
        public MainWindow formPackBox_Cfg;
        SfcHttpClient SfcHttpClient;
        List<SOURCE_BOXID> sequenlist = new List<SOURCE_BOXID>();
        public FormLotcarton(MainWindow main,SfcHttpClient _sfcHttpClient)
        {
            InitializeComponent();
            this.formPackBox_Cfg = main;
            SfcHttpClient = _sfcHttpClient;
            Edt_BOXID.Focus();
        }

        private void Btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            Edt_BOXID.Clear();
            Edt_BOXID.Focus();
            Edt_ModelName.Clear();
            Edt_MO.Clear();
            Edt_Version.Clear();
            Edt_Qty.Clear();
            ListView.ItemsSource = null;
        }

        private void FormClose(object sender, EventArgs e)
        {
            Edt_BOXID.Clear();
            Edt_BOXID.Focus();
            Edt_ModelName.Clear();
            Edt_MO.Clear();
            Edt_Version.Clear();
            Edt_Qty.Clear();
            ListView.ItemsSource = null;
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Edt_BOXID.Clear();
            Edt_BOXID.Focus();
            Edt_ModelName.Clear();
            Edt_MO.Clear();
            Edt_Version.Clear();
            Edt_Qty.Clear();
        }

        private async void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            int i;
            string smonumber;
            smonumber = "";
            if (string.IsNullOrEmpty(Edt_Qty.Text))
            {
                Edt_Qty.Text = "0";
            }
            if (string.IsNullOrEmpty(Edt_TotalQty.Text))
            {
                Edt_TotalQty.Text = "0";
            }
            if (string.IsNullOrEmpty(Edt_BOXID.Text))
            {
                if (await Getboxidmessage(Edt_BOXID.Text) == false)
                {
                    Edt_BOXID.Clear();
                    Edt_BOXID.Focus();
                    return;
                }
                for (i = 1; i <= sequenlist.Count - 1; i++)
                {
                    if (sequenlist[i].BOX_ID == Edt_BOXID.Text)
                    {
                        formPackBox_Cfg.lbError.Text = "DUP";
                        ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "Trung Lap";
                        _er.MessageEnglish = "DUP";
                        _er.ShowDialog();
                        return;
                    }
                }
                if (Edt_Qty.Text == packqty)
                {
                    formPackBox_Cfg.lbError.Text = "BOX ERROR";
                    ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "BOX phat sinh loi";
                    _er.MessageEnglish = "BOX ERROR";
                    _er.ShowDialog();
                    return;
                }

                string strGetDataWIP = "select * from sfism4.r_wip_tracking_t where group_name<>'PACK_BOX' AND  tray_no='" + Edt_BOXID.Text + "' AND MO_NUMBER='" + Edt_MO.Text + "'";
                var qry_DataWIP = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetDataWIP,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_DataWIP.Data != null)
                {
                    formPackBox_Cfg.lbError.Text = "BOX ERROR";
                    ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "BOX loi";
                    _er.MessageEnglish = "BOX ERROR";
                    _er.ShowDialog();
                    Edt_BOXID.Clear();
                    Edt_BOXID.Focus();
                    Edt_ModelName.Clear();
                    Edt_MO.Clear();
                    Edt_Version.Clear();
                    Edt_Qty.Clear();
                    return;
                }

                for (i = 1; i <= sequenlist.Count - 1;i++)
                {
                    if (sequenlist[i].MO_NUMBER != Edt_MO.Text)
                    {
                        smonumber = sequenlist[i].MO_NUMBER;
                    }
                    if (await Checkmodelver(Edt_MO.Text,smonumber) == false)
                    {
                        formPackBox_Cfg.lbError.Text = "80084 - " + await formPackBox_Cfg.GetPubMessage("80084");
                        ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "80084 - " + await formPackBox_Cfg.GetPubMessage("80084");
                        _er.MessageEnglish = "80084 - " + await formPackBox_Cfg.GetPubMessage("80084");
                        _er.ShowDialog();
                        Edt_BOXID.Clear();
                        Edt_BOXID.Focus();
                        Edt_ModelName.Clear();
                        Edt_MO.Clear();
                        Edt_Version.Clear();
                        Edt_Qty.Clear();
                        return;
                    }

                    if (await CheckMotype(Edt_MO.Text,smonumber) == false)
                    {
                        formPackBox_Cfg.lbError.Text = "00689 - " + await formPackBox_Cfg.GetPubMessage("00689");
                        ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                        _er.CustomFlag = true;
                        _er.MessageVietNam = "00689 - " + await formPackBox_Cfg.GetPubMessage("00689");
                        _er.MessageEnglish = "00689 - " + await formPackBox_Cfg.GetPubMessage("00689");
                        _er.ShowDialog();
                        Edt_BOXID.Clear();
                        Edt_BOXID.Focus();
                        Edt_ModelName.Clear();
                        Edt_MO.Clear();
                        Edt_Version.Clear();
                        Edt_Qty.Clear();
                        return;
                    }
                }

                if (Int32.Parse(Edt_TotalQty.Text) + Int32.Parse(Edt_Qty.Text) > Int32.Parse(packqty))
                {
                    formPackBox_Cfg.lbError.Text = "Pls Check Count!";
                    ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "Check so luong!";
                    _er.MessageEnglish = "Pls Check Count!";
                    _er.ShowDialog();
                    Edt_BOXID.Focus();
                    Edt_BOXID.SelectAll();
                    return;
                }

                string strGetData = "select count(serial_number),a.version_code,a.group_name,b.model_name from sfism4.r_wip_tracking_t a,sfism4.r_mo_base_t b"
                    + " where a.mo_number=b.mo_number and A.tray_NO='" + Edt_BOXID.Text + "' group by a.version_code,a.group_name,b.model_name";
                var qry_Data = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text
                });
                i = sequenlist.Count;
                sequenlist[i].NO = i.ToString();
                sequenlist[i].MODEL_NAME = Edt_ModelName.Text.Trim();
                sequenlist[i].MO_NUMBER = Edt_MO.Text.Trim();
                sequenlist[i].VERSION_CODE = Edt_Version.Text.Trim();
                sequenlist[i].BOX_ID = Edt_BOXID.Text.Trim();
                Edt_TotalQty.Text = (Int32.Parse(Edt_TotalQty.Text) + Int32.Parse(Edt_Qty.Text)).ToString();
                Edt_BOXID.Clear();
                Edt_BOXID.Focus();
                Edt_ModelName.Clear();
                Edt_MO.Clear();
                Edt_Version.Clear();
                Edt_Qty.Clear();
            }
        }
        
        private async Task<bool> CheckMotype(string mo_number1,string mo_number2)
        {
            string motype, mo_type;
            string strGetDataMO = "select * from sfism4.r_bpcs_moplan_t a,sfis1.C_PARAMETER_INI  b  where a.sap_mo_type=b.vr_item  and b.prg_name='WORKTYPE' AND a.mo_number='" + mo_number1 + "'";
            var qry_DataMO = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text
            });
            motype = qry_DataMO.Data["sap_mo_type"].ToString();

            string strGetDataMO2 = "select * from sfism4.r_bpcs_moplan_t a,sfis1.C_PARAMETER_INI  b  where a.sap_mo_type=b.vr_item  and b.prg_name='WORKTYPE' AND a.mo_number='" + mo_number2 + "'";
            var qry_DataMO2 = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataMO2,
                SfcCommandType = SfcCommandType.Text
            });
            mo_type = qry_DataMO2.Data["sap_mo_type"].ToString();
            if (mo_type == motype)
            {
                if (M_flag.IndexOf("A") > -1)
                {
                    if (motype == "Consigned")
                    {
                        if (item_ControlRun.IsCheckable == false)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (motype == "Pilot Run")
                        {
                            if (item_PilotRun.IsChecked == false)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (M_flag.IndexOf("A") > -1)
                {
                    if ((motype == "Pilot Run") || (motype== "Consigned") || (mo_type == "Pilot Run") || (mo_type == "Consigned"))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        private async Task<bool> Checkmodelver(string mo1,string mo2)
        {
            string mo1model, mo2model, mo1ver, mo2ver;

            string strGetDataMO = "select * from sfism4.r_mo_base_t where mo_number='" + mo1 + "'";
            var qry_DataMO = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text
            });
            mo1model = qry_DataMO.Data["model_name"].ToString();
            mo1ver = qry_DataMO.Data["version_code"].ToString();

            string strGetDataMO2 = "select * from sfism4.r_mo_base_t where mo_number='" + mo2 + "'";
            var qry_DataMO2 = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataMO2,
                SfcCommandType = SfcCommandType.Text
            });
            mo2model = qry_DataMO2.Data["model_name"].ToString();
            mo2ver = qry_DataMO2.Data["version_code"].ToString();

            if (mo1model != mo2model)
            {
                return false;
            }
            else
            {
                if (mo1ver != mo2ver)
                {
                    return false;
                }
            }
            return true;
        }
        private async Task<bool> Getboxidmessage(string boxid)
        {
            string sMcarton;

            string strGetDataTrayNO = "select * from sfism4.r_wip_tracking_t where tray_no='" + Edt_BOXID.Text + "' and rownum=1";
            var qry_DataTrayNO = await SfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataTrayNO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_DataTrayNO.Data == null)
            {
                formPackBox_Cfg.lbError.Text = "00417 - " +  await formPackBox_Cfg.GetPubMessage("00417");
                ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = "00417 - " + await formPackBox_Cfg.GetPubMessage("00417");
                _er.MessageEnglish = "00417 - " + await formPackBox_Cfg.GetPubMessage("00417");
                _er.ShowDialog();
                return false;
            }
            else
            {
                List<R107> result = new List<R107>();
                result = qry_DataTrayNO.Data.ToListObject<R107>().ToList();
                sMcarton = result[0].MCARTON_NO;

                if ((!string.IsNullOrEmpty(sMcarton)) && (sMcarton != "N/A"))
                {
                    formPackBox_Cfg.lbError.Text = "00095 - " + await formPackBox_Cfg.GetPubMessage("00095");
                    ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "00095 - " + await formPackBox_Cfg.GetPubMessage("00095");
                    _er.MessageEnglish = "00095 - " + await formPackBox_Cfg.GetPubMessage("00095");
                    _er.ShowDialog();
                    return false;
                }
            }

            string strGetTrayNO = "select * from sfism4.r_wip_tracking_t where tray_no='" + Edt_BOXID.Text + "' and rownum=1";
            var qry_TrayNO = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetTrayNO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_TrayNO.Data != null)
            {
                Edt_ModelName.Text = qry_TrayNO.Data["model_name"].ToString();
                await FindMotype(Edt_ModelName.Text);
                if (M_flag.IndexOf("195") == -1)
                {
                    formPackBox_Cfg.lbError.Text = "60140 - " + await formPackBox_Cfg.GetPubMessage("60140");
                    ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "60140 - " + await formPackBox_Cfg.GetPubMessage("60140");
                    _er.MessageEnglish = "60140 - " + await formPackBox_Cfg.GetPubMessage("60140");
                    _er.ShowDialog();
                    return false;
                }
                Edt_MO.Text = qry_TrayNO.Data["mo_number"].ToString();
                Edt_Version.Text = qry_TrayNO.Data["version_code"].ToString();

                string strGetTrayNOQty = "select count(*) qty from sfism4.r_wip_tracking_t where tray_no='" + Edt_BOXID.Text + "'";
                var qry_TrayNOQty = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetTrayNOQty,
                    SfcCommandType = SfcCommandType.Text
                });
                Edt_Qty.Text = qry_TrayNOQty.Data["qty"].ToString();
                if (string.IsNullOrEmpty(Edt_TotalQty.Text))
                {
                    Edt_TotalQty.Text = "0";
                }

                string strGetParam = "select * from SFIS1.C_PACK_PARAM_T where version_code='" + Edt_Version.Text + "' and model_name='" + Edt_ModelName.Text + "'";
                var qry_Param = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetParam,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Param.Data != null)
                {
                    packqty = qry_Param.Data["box_qty"].ToString();
                }
                else
                {
                    formPackBox_Cfg.lbError.Text = "00111 - " + await formPackBox_Cfg.GetPubMessage("00111");
                    ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                    _er.CustomFlag = true;
                    _er.MessageVietNam = "00111 - " + await formPackBox_Cfg.GetPubMessage("00111");
                    _er.MessageEnglish = "00111 - " + await formPackBox_Cfg.GetPubMessage("00111");
                    _er.ShowDialog();
                    return false;
                }
            }
            return true;
        }

        private async void Btn_Pass_Click(object sender, RoutedEventArgs e)
        {
            string trayno;
            int i;

            trayno = sequenlist[1].VERSION_CODE;
            for (i = 1;i < sequenlist.Count - 1;i++)
            {
                if (sequenlist[1].VERSION_CODE != "")
                {
                    string strUpdate = "update sfism4.r_wip_tracking_t set tray_no='" + trayno + "',ERP_MO='" + sequenlist[1].VERSION_CODE + "',in_station_time=sysdate,emp_no='" + formPackBox_Cfg.empNo + "' where tray_no='" + sequenlist[1].VERSION_CODE + "'";
                    var Update = await SfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = strUpdate,
                        SfcCommandType = SfcCommandType.Text
                    });
                }

                string strInsert = "insert into sfism4.r_sn_detail_t select * from sfism4.r_wip_tracking_t where tray_no='" + trayno + "'";
                var Insert = await SfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = strInsert,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            MessageBox.Show("OK - " + trayno);
        }

        private async void BOXID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (await Getboxidmessage(Edt_BOXID.Text) == false)
                {
                    Edt_BOXID.Clear();
                    Edt_BOXID.Focus();
                }
            }
        }

        private async Task FindMotype(string smodel)
        {
            string strGetModelName = "select * from sfis1.c_model_desc_t where model_name='" + smodel + "'";
            var qry_ModelName = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModelName,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ModelName.Data != null)
            {
                M_flag = qry_ModelName.Data["model_type"].ToString();
            }
        }
    }
}
