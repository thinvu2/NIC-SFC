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
using System.Data;

namespace PACKINGBOXID_CFG
{
    /// <summary>
    /// Interaction logic for FormBOXID.xaml
    /// </summary>
    public partial class FormBOXID : Window
    {
        //public DataTable dtParams_Reprint = new DataTable();
        SfcHttpClient SfcHttpClient;
        MainWindow formPackBox_Cfg;
        public int My_CartonQtyOnDb;
        public string C19_CustModelName, M_sCustModelName, My_LabelFileName;
        public static string BOXID_CLOSE = "";
        public FormBOXID(MainWindow main,SfcHttpClient _sfcHttpClient)
        {
            InitializeComponent();
            this.formPackBox_Cfg = main;
            SfcHttpClient = _sfcHttpClient;
            Edt_BOXID.Focus();
        }

        private async void BOXID_Print()
        {
            string sCartonNo, boxid, c_tray_no, QTYSTR, MBOXID, QTYBOXID, oldboxid;
            int Startqty;
            boxid = "";
            QTYBOXID = "";
            if (string.IsNullOrEmpty(Edt_BOXID.Text))
            {
                formPackBox_Cfg.lbError.Text = "00416 - " + await formPackBox_Cfg.GetPubMessage("00416");
                ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "00416 - " + await formPackBox_Cfg.GetPubMessage("00416");
                _er.MessageVietNam = "00416 - " + await formPackBox_Cfg.GetPubMessage("00416");
                _er.ShowDialog();
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(formPackBox_Cfg.Edt_BOXID.Text))
                {
                    if (Edt_BOXID.Text != formPackBox_Cfg.Edt_BOXID.Text)
                    {
                        formPackBox_Cfg.lbError.Text = "00680 - " + await formPackBox_Cfg.GetPubMessage("00680");
                        ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00680 - " + await formPackBox_Cfg.GetPubMessage("00680");
                        _er.MessageVietNam = "00680 - " + await formPackBox_Cfg.GetPubMessage("00680");
                        _er.ShowDialog();
                        return;
                    }
                }

                //ClearParams();
                //sCartonNo = "";
                if (await FindBOXID(boxid) == false)
                {
                    Edt_BOXID.Focus();
                    return;
                }
                boxid = BOXID_CLOSE;
                MBOXID = BOXID_CLOSE;
                string strGetTrayNO = "select distinct model_name from sfism4.r107 where tray_no= '" + boxid + "'";
                var qry_TrayNO = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetTrayNO,
                    SfcCommandType = SfcCommandType.Text
                });
                c_tray_no = qry_TrayNO.Data["model_name"].ToString();

                string strGetParameter = "SELECT*FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOXID' AND VR_CLASS='" + c_tray_no + "' AND VR_ITEM='QTY'";
                var qry_Parameter = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetParameter,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Parameter.Data != null)
                {
                    QTYSTR = qry_Parameter.Data["vr_value"].ToString();
                    Startqty = Int32.Parse(qry_Parameter.Data["vr_name"].ToString());
                    MBOXID = boxid;

                    string strGetCountTray = "select TRIM(TO_CHAR(COUNT(*),'" + QTYSTR + "')) QTY from sfism4.r107 where TRAY_NO='" + MBOXID + "'";
                    var qry_CountTray = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCountTray,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_CountTray.Data != null)
                    {
                        QTYBOXID = qry_CountTray.Data["qty"].ToString();
                    }
                    oldboxid = MBOXID;
                    MBOXID = MBOXID.Substring(0, Startqty - 1) + QTYBOXID + MBOXID.Substring(Startqty + QTYBOXID.Length - 1, MBOXID.Length - (Startqty + MBOXID.Length) - 1);

                    try
                    {
                        string strUpdatePallet = "update SFIS1.C_PALLET_T set PALLET_NO=:newboxid where PALLET_NO=:oldboxid";
                        var Update = await SfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = strUpdatePallet,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="newboxid",Value=MBOXID},
                                new SfcParameter{Name="oldboxid",Value=oldboxid}
                            }
                        });

                        string strUpdateTray = "update sfism4.r_wip_tracking_t set TRAY_NO=:newboxid where TRAY_NO=:oldboxid";
                        var UpdateTray = await SfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = strUpdateTray,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="newboxid",Value=MBOXID},
                                new SfcParameter{Name="oldboxid",Value=oldboxid}
                            }
                        });
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"00042 - {await formPackBox_Cfg.GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var qry_Pallet = await SfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = "select * from sfis1.c_pallet_t where PALLET_NO = '" + boxid + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Pallet.Data.Count() == 0)
                {
                    formPackBox_Cfg.lbError.Text = "00412 - " + await formPackBox_Cfg.GetPubMessage("00412");
                    ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = "00412 - " + await formPackBox_Cfg.GetPubMessage("00412");
                    _er.MessageVietNam = "00412 - " + await formPackBox_Cfg.GetPubMessage("00412");
                    _er.ShowDialog();
                    return;
                }
                else
                {
                    var Delete = await SfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "delete from sfis1.c_pallet_t where PALLET_NO = '" + boxid + "'",
                        SfcCommandType = SfcCommandType.Text
                    });

                    var Update = await SfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "update sfism4.r_wip_tracking_t set pallet_full_flag='Y' where TRAY_NO = '" + boxid + "'",
                        SfcCommandType = SfcCommandType.Text
                    });

                    formPackBox_Cfg.lb_Count.Content = "0";
                    formPackBox_Cfg.lst_ListBox1.Items.Clear();
                }
                if (await FindPrint(MBOXID) == false)
                {
                    Edt_BOXID.SelectAll();
                    return;
                }
                //formPackBox_Cfg.PrintToCodeSoft();
                Item_Name.Items.Clear();
                Item_Data.Items.Clear();
            }
            Edt_BOXID.SelectAll();
            Edt_BOXID.Focus();
            formPackBox_Cfg.lb_Count.Content = "0";
        }

        private async Task<bool> FindPrint(string _paramCartonNo)
        {

            string indata = "LABELTYPE:PACKBOXID|BOXID:" + _paramCartonNo;
            var result = await SfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_GET_PARAMS",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="IN_DATA",Value=indata,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
            });
            dynamic _ads = result.Data;
            string _RES = _ads[1]["res"];
            if (_RES.StartsWith("OK"))
            {
                string strOut = _ads[0]["out_data"];
                await formPackBox_Cfg.zPrintBox(strOut, _paramCartonNo);
                return true;
            }
            else
            {
                formPackBox_Cfg.lbError.Text = _RES.Split('|')[0].ToString();
                ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                _er.CustomFlag = true;
                _er.MessageVietNam = _RES.Split('|')[1].ToString();
                _er.MessageEnglish = _RES.Split('|')[0].ToString();
                _er.ShowDialog();
                return false;
            }
            //---------------------------------------
        }
        private async Task<bool> FindBOXID(string paramboxid)
        {
            string strGetDataWIP = "select * from sfism4.r_wip_tracking_t"
                + " where tray_no = (select tray_no from sfism4.r_wip_tracking_t"
                + " where (serial_number = :sSerN"
                + " or  shipping_SN = :sSerN or tray_no = :sSerN) and rownum = 1)";
            var qury_DataWIP = await SfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetDataWIP,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="sSerN",Value=Edt_BOXID.Text}
                }
            });
            if (qury_DataWIP.Data == null)
            {
                formPackBox_Cfg.lbError.Text = "00414 - " + await formPackBox_Cfg.GetPubMessage("00414");
                ShowMessageForm _er = new ShowMessageForm(SfcHttpClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "00414 - " + await formPackBox_Cfg.GetPubMessage("00414");
                _er.MessageVietNam = "00414 - " + await formPackBox_Cfg.GetPubMessage("00414");
                _er.ShowDialog();
                return false;
            }
            else
            {
                paramboxid = qury_DataWIP.Data["tray_no"].ToString();
                BOXID_CLOSE = paramboxid;
                return true;
            }
        }
        /*
        private void ClearParams()
        {
            Item_Name.Items.Clear();
            Item_Data.Items.Clear();
        }

        private void AddParams(string _name, string _value)
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
        */
        private void BOXID_Print_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                BOXID_Print();
            }
        }

        private void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            BOXID_Print();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Edt_BOXID.Text = "";
            Edt_BOXID.Focus();
        }

        private void Btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Edt_BOXID.Text = "";
            this.Close();
        }
    }
}
