using Sfc.Library.HttpClient;
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
using Sfc.Core.Parameters;
using PACK_BOX.Model;
using Sfc.Library.HttpClient.Helpers;

namespace PACK_BOX
{
    /// <summary>
    /// Interaction logic for FormKeyPart.xaml
    /// </summary>
    public partial class FormKeyPart : Window
    {
        public MainWindow frm_main;
        public SfcHttpClient sfcClient;
        public string res, r_data, c_data;
        public string kpn, kpn_sn, kplong;
        public FormKeyPart()
        {
            InitializeComponent();
        }
        public FormKeyPart(MainWindow main, SfcHttpClient _sfcClient)
        {
            InitializeComponent();
            frm_main = main;
            sfcClient = _sfcClient;
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            Edt_scan_keypart.Focus();
        }

        private void FormKeyPart_Close(object sender, EventArgs e)
        {
            frm_main.item_automation.IsChecked = false;
            this.Close();
        }

        public async Task InputSN(string SN)
        {
            string strGetSerialNumber = $"select * from sfism4.r_wip_tracking_t where serial_number ='{lb_serial.Content}'";
            var qry_SerialNumber = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetSerialNumber,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_SerialNumber.Data["finish_flag"].ToString()== "1")
            {
                lb_serial.Content = null;
                lb_mmorec.Content = "KPS INPUT FINISH";
                ShowMessageForm _er = new ShowMessageForm(frm_main,sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "KPS INPUT FINISH !";
                _er.MessageVietNam = "KPS INPUT FINISH !";
                _er.ShowDialog();
                _er.passwordBox.Password = "9999";
                Edt_scan_keypart.Focus();
                Edt_scan_keypart.SelectAll();
                return;
            }
            else
            {
                Edt_monumber.Text = qry_SerialNumber.Data["mo_number"].ToString();
                lb_mmorec.Content = "OK OK";
                Edt_scan_keypart.SelectAll();
                Edt_scan_keypart.Focus();
            }
        }
        public async void Key_Part()
        {
            string txtass, data, check221, c_model, c_inner;

            Edt_scan_keypart.Text = Edt_scan_keypart.Text.ToUpper();
            txtass = Edt_scan_keypart.Text.ToUpper();

            //Fix de phan biet vs ROKU
            string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PACK_BOX' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
            var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strChkROKU,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ChkROKU.Data.Count() != 0)
            {
                if (txtass == "END")
                {
                    string strGetData = "select count(*) from sfis1.c_bom_keypart_t a,sfism4.r_wip_tracking_t b"
                       + $" where a.bom_no=b.bom_no and b.serial_number='{lb_serial.Content}' and (a.group_name is null or a.group_name ='KEYPART')";
                    var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetData,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (Int32.Parse(qry_Data.Data["count(*)"].ToString()) != (lstlong.Items.Count + lstshost.Items.Count))
                    {
                        lb_mmorec.Content = "NOT ENOUGHT KEYPART";
                        ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "NOT ENOUGHT KEYPART!";
                        _er.MessageVietNam = "NOT ENOUGHT KEYPART!";
                        _er.ShowDialog();
                        _er.passwordBox.Password = "9999";
                        Edt_scan_keypart.SelectAll();
                        Edt_scan_keypart.Focus();
                        return;
                    }
                    else
                    {
                        if (lstlong.Items.Count != 0)
                        {
                            for (int i = 0; i <= lstlong.Items.Count - 1; i++)
                            {
                                data = lstlong.Items[i].ToString();
                                kpn = data.Substring(0, data.IndexOf("|"));
                                kpn_sn = data.Substring(data.IndexOf("|") + 1);

                                string strGetDataSN = "select nvl(model_type,'-') model_type,model_name from sfis1.c_model_desc_t where model_name in ( "
                                   + $" SELECT model_name FROM sfism4.r_wip_tracking_t WHERE serial_number='{lb_serial.Content}')";
                                var qry_DataSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetDataSN,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                check221 = qry_DataSN.Data["model_type"].ToString();
                                c_model = qry_DataSN.Data["model_name"].ToString();

                                if (check221.IndexOf("221") != -1)
                                {
                                    string strGetCount = $"select count(*) from sfism4.r_wip_tracking_t where shipping_sn='{kpn}' and rownum=1";
                                    var qry_Count = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetCount,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    if (qry_Count.Data.Count() != 0)
                                    {
                                        try
                                        {
                                            string strUpdate = $"UPDATE sfism4.r_wip_tracking_t SET finish_flag=1 WHERE shipping_sn= '{kpn}'";
                                            var Update = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                            {
                                                CommandText = strUpdate,
                                                SfcCommandType = SfcCommandType.Text
                                            });
                                            c_model = kpn_sn;
                                        }
                                        catch (Exception)
                                        {
                                            MessageBox.Show($"00042 - {await frm_main.GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        lb_mmorec.Content = "NOT SHIPPING SN --CAN''T UPDATE!";
                                        ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                                        _er.CustomFlag = true;
                                        _er.MessageEnglish = "NOT SHIPPING SN --CAN''T UPDATE!";
                                        _er.MessageVietNam = "NOT SHIPPING SN --CAN''T UPDATE!";
                                        _er.ShowDialog();
                                        _er.passwordBox.Password = "9999";
                                        return;
                                    }
                                }
                                else
                                {
                                    c_model = "N/A";
                                }

                                string strGetSerialNumber = $"SELECT * FROM SFISM4.R_POWERSN_T WHERE SERIAL_NUMBER='{kpn}'";
                                var qry_SerialNumber = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetSerialNumber,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_SerialNumber.Data.Count() != 0)
                                {
                                    string strGetFlag = $"SELECT * FROM SFISM4.R_POWERSN_T WHERE SERIAL_NUMBER='{kpn}'";
                                    var qry_Flag = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strGetFlag,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    c_inner = qry_Flag.Data["flag"].ToString();
                                }
                                else
                                {
                                    c_inner = "N/A";
                                }

                                try
                                {
                                    string strInsert = "INSERT INTO SFISM4.R_WIP_KEYPARTS_T" +
                                       " (SERIAL_NUMBER,KEY_PART_NO,KEY_PART_SN,KP_RELATION,GROUP_NAME,EMP_NO,WORK_TIME,MO_NUMBER,CARTON_NO,PART_MODE,KP_CODE)" +
                                       " VALUES" +
                                       " ('" + lb_serial.Content + "','" + kpn_sn + "','" + kpn + "','','KEYPART','CCD',SYSDATE,'" + Edt_monumber.Text + "','N/A','" + c_model + "','" + c_inner + "')";
                                    var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strInsert,
                                        SfcCommandType = SfcCommandType.Text
                                    });

                                    string strUpdate = "UPDATE sfism4.r_wip_tracking_t SET finish_flag=1 WHERE serial_number= '" + lb_serial.Content + "'";
                                    var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strUpdate,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    Refresh("UNDO");
                                    this.Close();
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                        else
                        {
                            if (lstshost.Items.Count == 0)
                            {
                                Refresh("UNDO");
                                this.Close();
                            }
                            else
                            {
                                try
                                {
                                    string strUpdate = "UPDATE sfism4.r_wip_tracking_t SET finish_flag=1 WHERE serial_number= '" + lb_serial.Content + "'";
                                    var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strUpdate,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                    Refresh("UNDO");
                                    this.Close();
                                }
                                catch (Exception)
                                {
                                    MessageBox.Show($"00042 - {await frm_main.GetPubMessage("00042")}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (txtass == "UNDO")
                {
                    Refresh(txtass);
                    Edt_scan_keypart.SelectAll();
                    Edt_scan_keypart.Focus();
                    return;
                }
                else if (txtass == "")
                {
                    lb_mmorec.Content = "INPUT SN";
                    return;
                }
                else
                {
                    if (lb_serial.Content == null)
                    {
                        try
                        {
                            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = "SFIS1.CHECK_SN",
                                SfcCommandType = SfcCommandType.StoredProcedure,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="DATA", Value=txtass,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                                }
                            });

                            dynamic ads = result.Data;
                            res = ads[0]["res"];
                            if (res != "OK")
                            {
                                lb_mmorec.Content = res;
                                ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = res;
                                _er.MessageVietNam = res;
                                _er.passwordBox.Password = "9999";
                                _er.ShowDialog();
                                Edt_scan_keypart.SelectAll();
                                Edt_scan_keypart.Focus();
                                return;
                            }
                            else
                            {
                                string strGetShippingSN = $"SELECT * FROM SFISM4.R107 WHERE SHIPPING_SN='{txtass}' OR SHIPPING_SN2='{txtass}'";
                                var qry_ShippingSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetShippingSN,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                lb_serial.Content = qry_ShippingSN.Data["serial_number"].ToString();
                                await InputSN(lb_serial.Content.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "Excute procedure have error: " + ex.Message;
                            _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + ex.Message;
                            _er.passwordBox.Password = "9999";
                            _er.ShowDialog();
                            Edt_scan_keypart.SelectAll();
                            Edt_scan_keypart.Focus();
                            return;
                        }

                    }
                    else
                    {
                        try
                        {
                            var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = "SFIS1.CHECK_LOTKP_FOR_ALL",
                                SfcCommandType = SfcCommandType.StoredProcedure,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="DATA", Value=txtass,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="SN",Value=lb_serial.Content,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                                    new SfcParameter{Name="command1",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                }
                            });

                            dynamic ads = result.Data;
                            res = ads[0]["res"];
                            if (res != "OK")
                            {
                                lb_mmorec.Content = res;
                                ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                                _er.CustomFlag = true;
                                _er.MessageEnglish = res;
                                _er.MessageVietNam = res;
                                _er.ShowDialog();
                                _er.passwordBox.Password = "9999";
                                Edt_scan_keypart.SelectAll();
                                Edt_scan_keypart.Focus();
                                return;
                            }
                            else
                            {
                                dynamic ads_long = result.Data;
                                kplong = ads_long[1]["command1"];
                                if (kplong != txtass)
                                {
                                    if (lstlong.Items.Count != 0)
                                    {
                                        for (int j = 0; j <= lstlong.Items.Count - 1; j++)
                                        {
                                            if (lstshost.Items.IndexOf(txtass + "|" + kplong) == -1)
                                            {
                                                lstlong.Items.Add(txtass + "|" + kplong);
                                                lb_mmorec.Content = "OK OK";
                                                Edt_scan_keypart.SelectAll();
                                                Edt_scan_keypart.Focus();
                                                return;
                                            }
                                            else
                                            {
                                                lb_mmorec.Content = "DUPLICATE " + txtass;
                                                ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                                                _er.CustomFlag = true;
                                                _er.MessageEnglish = "DUPLICATE " + txtass;
                                                _er.MessageVietNam = "DUPLICATE " + txtass;
                                                _er.ShowDialog();
                                                _er.passwordBox.Password = "9999";
                                                Edt_scan_keypart.SelectAll();
                                                Edt_scan_keypart.Focus();
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lstlong.Items.Add(txtass + "|" + kplong);
                                        lb_mmorec.Content = "OK OK";
                                        Edt_scan_keypart.SelectAll();
                                        Edt_scan_keypart.Focus();
                                        return;
                                    }
                                }
                                else
                                {
                                    if (lstshost.Items.Count != 0)
                                    {
                                        for (int j = 0; j <= lstshost.Items.Count - 1; j++)
                                        {
                                            if (lstshost.Items.IndexOf(txtass) == -1)
                                            {
                                                lstshost.Items.Add(txtass);
                                                lb_mmorec.Content = "OK OK";
                                                Edt_scan_keypart.SelectAll();
                                                Edt_scan_keypart.Focus();
                                                return;
                                            }
                                            else
                                            {
                                                lb_mmorec.Content = "DUPLICATE " + txtass;
                                                ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                                                _er.CustomFlag = true;
                                                _er.MessageEnglish = "DUPLICATE " + txtass;
                                                _er.MessageVietNam = "DUPLICATE " + txtass;
                                                _er.ShowDialog();
                                                _er.passwordBox.Password = "9999";
                                                Edt_scan_keypart.SelectAll();
                                                Edt_scan_keypart.Focus();
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        lstshost.Items.Add(txtass);
                                        lb_mmorec.Content = "OK OK";
                                        Edt_scan_keypart.SelectAll();
                                        Edt_scan_keypart.Focus();
                                        return;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ShowMessageForm _er = new ShowMessageForm(frm_main, sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "Excute procedure have error: " + ex.Message;
                            _er.MessageVietNam = _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + ex.Message;
                            _er.ShowDialog();
                            _er.passwordBox.Password = "9999";
                            Edt_scan_keypart.SelectAll();
                            Edt_scan_keypart.Focus();
                            return;
                        }
                    }
                }
            }
        }
        private void Refresh(string data)
        {
            c_data = data.ToUpper();
            if (c_data == "UNDO")
            {
                lb_serial.Content = null;
                lstlong.Items.Clear();
                lstshost.Items.Clear();
                lb_mmorec.Content = "";
                Edt_monumber.Text = "";
                Edt_scan_keypart.SelectAll();
                return;
            }
        }
        public void Edt_scan_keypart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Key_Part();
            }
        }
    }
}