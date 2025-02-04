using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Ports;
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

namespace LOT_REPRINT
{
    /// <summary>
    /// Interaction logic for frmCheckAcce.xaml
    /// </summary>
    public partial class frmCheckAcce : Window
    {
        public SfcHttpClient sfcClient;
        string sql, mGroup, mMo, mLine, mySection, mStation, SSN;
        int dem, kpCount;
        public SerialPort serialPort3;
        public frmCheckAcce()
        {
            InitializeComponent();
        }

        private void itemSetStation_Click(object sender, RoutedEventArgs e)
        {
            frmSetupStation setupStation = new frmSetupStation();
            setupStation.sfcClient = sfcClient;
            setupStation.ShowDialog();
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            ini.IniWriteValue("LOTREPRINT", "GROUP", setupStation.M_Group);
            ini.IniWriteValue("LOTREPRINT", "LINE", setupStation.M_Line);
            ini.IniWriteValue("LOTREPRINT", "STATION", setupStation.M_Station);
            ini.IniWriteValue("LOTREPRINT", "SECTION", setupStation.My_Section);
            mGroup = ini.IniReadValue("LOTREPRINT", "GROUP");
            mLine = ini.IniReadValue("LOTREPRINT", "LINE");
            mStation = ini.IniReadValue("LOTREPRINT", "STATION");
            mySection = ini.IniReadValue("LOTREPRINT", "SECTION");
            lblStation.Content = mLine + "-" + mGroup;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            mGroup = ini.IniReadValue("LOTREPRINT", "GROUP");
            mLine = ini.IniReadValue("LOTREPRINT", "LINE");
            mStation = ini.IniReadValue("LOTREPRINT", "STATION");
            mySection = ini.IniReadValue("LOTREPRINT", "SECTION");
            lblStation.Content = mLine + "-" + mGroup;
            txtSend.Focus();
        }

        private void showMessage(string MessageEnglish, string MessageVietNam, bool CustomFlag)
        {
            frmMessage frmMessage = new frmMessage();
            frmMessage.sfcClient = sfcClient;
            if (CustomFlag)
            {
                frmMessage.MessageEnglish = MessageEnglish;
                frmMessage.MessageVietNam = MessageVietNam;
                frmMessage.CustomFlag = CustomFlag;
            }
            else
            {
                frmMessage.errorcode = MessageEnglish;
                frmMessage.CustomFlag = CustomFlag;
            }
            frmMessage.ShowDialog();
        }
        private async Task<DateTime> ServerDateTime()
        {
            sql = "SELECT TO_CHAR(SYSDATE,'YYYY/MM/DD HH24:MI:SS') AS TIME_SERVER FROM DUAL";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            DateTime timeServer = Convert.ToDateTime(_result.Data["time_server"].ToString());
            return timeServer;
        }
        private async Task<string> ServerMoDate()
        {
            sql = "SELECT To_Char(SysDate,'yyyymmdd') Mo_Date FROM DUAL";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            string Mo_Date = _result.Data["mo_date"].ToString();
            return Mo_Date;
        }
        private async Task<bool> TestInput(string SN)
        {
            string wSection;
            sql = "SELECT WORK_SECTION WRKSEC FROM SFIS1.C_WORK_DESC_T WHERE START_TIME <= TO_CHAR(SYSDATE,'HH24MI') AND END_TIME >TO_CHAR(SYSDATE,'HH24MI')  AND LINE_NAME ='" + mLine + "' AND SECTION_NAME = 'Default' AND SHIFT = '1'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if(_result.Data != null)
            {
                wSection = _result.Data["wrksec"]?.ToString() ?? "";
            }
            else
            {
                showMessage("Cannot get WORK_SECTION, sql: " + sql, "Không thể lấy WORK_SECTION", true);
                return false;
            }
            var procedure = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.TEST_INPUT",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="Emp",Value="Lot_reprint",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="DATA",Value=SN,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="LINE",Value=mLine,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="SECTION",Value=mySection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="W_STATION",Value=mStation,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="DATETIME",Value=await ServerDateTime(),SfcParameterDataType=SfcParameterDataType.Date,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="EC",Value="N/A",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MO_DATE",Value=await ServerMoDate(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="W_SECTION",Value=wSection,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value=mGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
            });
            dynamic ads = procedure.Data;
            string Ok = ads[0]["res"];
            if(Ok != "OK")
            {
                showMessage("SFIS1.TEST_INPUT reponse " + Ok, "Bản chưa qua trạm!", true);
                return false;
            }
            return true;
        }
        private async void refresh(string data)
        {
            data = data.ToUpper();
            if(data == "UNDO")
            {
                lblSerial.Content = "";
                lstLong.Items.Clear();
                lstShost.Items.Clear();
                lblRec.Content = "Ready ...";
                lblRec.Foreground = Brushes.Yellow;
                mMo = "";
                dem = 0;
                txtSend.Text = "UNDO";
                txtSend.SelectAll();
                txtSend.Focus();
                return;
            }
            if (data == "END")
            {
                txtSend.Text = "END";
                await Submit();
                //txtSend_KeyDown(new object(), new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Enter));
                return;
            }
        }
        private async Task InputSN(string dataInput)
        {
            sql = "select * from sfism4.r_wip_tracking_t where serial_number ='" + dataInput + "'";
            var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (_result.Data == null)
            {
                lblRec.Content = "Not found data in R107 SN: " + dataInput;
                return;
            }
            mMo = _result.Data["mo_number"]?.ToString() ?? "";
            SSN = _result.Data["shipping_sn"]?.ToString() ?? "";
            lblRec.Content = "OK OK";
            lblRec.Foreground = Brushes.Yellow;
            txtSend.SelectAll();
            txtSend.Focus();
            return;
        }
        private async void txtSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await Submit();
            }
        }
        private async Task Submit()
        {
            bool mbYes = true;
            dynamic ads;
            string txtAss, data, kpn, kpn_sn, cModel, cInner, dataPro, res, kpLong;


            txtSend.Text = txtSend.Text.Trim().ToUpper();
            txtAss = txtSend.Text.Trim().ToUpper();
            if (txtAss == "END")
            {
                sql = "select A.KP_COUNT FROM sfis1.c_bom_keypart_t a,sfism4.r_wip_tracking_t b "
                    + "WHERE a.bom_no=b.bom_no AND b.serial_number='" + lblSerial.Content + "' "
                    + "AND a.group_name ='" + mGroup + "'";
                var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                if (_result.Data != null)
                {
                    if (Int32.Parse(_result.Data["kp_count"].ToString()) != (lstLong.Items.Count + lstShost.Items.Count))
                    {
                        lblRec.Content = "NOT ENOUGHT KEYPART";
                        lblRec.Foreground = Brushes.Red;
                        showMessage("NOT ENOUGHT KEYPART", "KHÔNG ĐỦ KEYPART", true);
                        txtSend.SelectAll();
                        txtSend.Focus();
                        return;
                    }
                    else if (lstLong.Items.Count != 0)
                    {
                        for (int i = 0; i < lstLong.Items.Count; i++)
                        {
                            data = lstLong.Items[i].ToString();
                            cModel = "N/A";
                            cInner = "N/A";
                            try
                            {
                                kpn = data.Substring(0, data.IndexOf("|"));
                                kpn_sn = data.Substring(data.IndexOf("|") + 1);
                            }
                            catch
                            {
                                kpn = data;
                                kpn_sn = data;
                            }

                            sql = "INSERT INTO SFISM4.R_WIP_KEYPARTS_T "
                                + "(SERIAL_NUMBER,KEY_PART_NO,KEY_PART_SN,KP_RELATION,GROUP_NAME,EMP_NO,WORK_TIME,MO_NUMBER,CARTON_NO,PART_MODE,KP_CODE) "
                                + "VALUES "
                                + "('" + lblSerial.Content + "','" + kpn_sn + "','" + kpn + "','','" + mGroup + "','LotRe',SYSDATE,'" + mMo + "','N/A','" + cModel + "','" + cInner + "') ";
                            var insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                            if (insert.Result.ToString() != "OK")
                            {
                                showMessage("Exception: " + insert.Message.ToString() + "\nsql: " + sql, "Exception: " + insert.Message.ToString() + "\nsql: " + sql, true);
                                return;
                            }
                        }
                        if (!await TestInput(lblSerial.Content.ToString()))
                        {
                            return;
                        }
                        serialPort3.Write(SSN + "\r\n");

                        this.Close();
                    }
                    else
                    {
                        refresh("UNDO");
                        this.Close();
                    }
                }
                else
                {
                    refresh("UNDO");
                    this.Close();
                }
            }
            else if (txtAss == "UNDO")
            {
                refresh(txtAss);
                txtSend.SelectAll();
                txtSend.Focus();
                return;
            }
            else if (string.IsNullOrEmpty(txtAss))
            {
                lblRec.Content = "INPUT SN";
                lblRec.Foreground = Brushes.Red;
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(mGroup))
                {
                    lblRec.Content = "Group not been selected yet";
                    lblRec.Foreground = Brushes.Red;
                    showMessage("Group not been selected yet", "Chưa cài đặt trạm kìa", true);
                    txtSend.SelectAll();
                    txtSend.Focus();
                    return;
                }
                if (string.IsNullOrEmpty((lblSerial.Content?.ToString() ?? "")))
                {
                    var procedure = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.INPUT_SN_FIRST_LOT",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="LINE",Value=mLine,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="MYGROUP",Value=mGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="DATA",Value=txtAss,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.InputOutput },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                    });
                    ads = procedure.Data;
                    res = ads[1]["res"];
                    dataPro = ads[0]["data"];
                    if (res != "OK")
                    {
                        lblRec.Content = res;
                        lblRec.Foreground = Brushes.Red;
                        showMessage("SFIS1.INPUT_SN_FIRST_LOT reponse " + res, res, true);
                        txtSend.SelectAll();
                        txtSend.Focus();
                        return;
                    }
                    else
                    {
                        procedure = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.CHECK_ROUTE",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="LINE",Value=mLine,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="MYGROUP",Value=mGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="DATA",Value=dataPro,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.InputOutput },
                                    new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                }
                        });
                        ads = procedure.Data;
                        dataPro = ads[0]["data"];
                        res = ads[1]["res"];
                        if (res != "OK")
                        {
                            lblRec.Content = res;
                            lblRec.Foreground = Brushes.Red;
                            showMessage("SFIS1.CHECK_ROUTE reponse " + res, res, true);
                            txtSend.SelectAll();
                            txtSend.Focus();
                            return;
                        }
                        lblSerial.Content = dataPro;

                        sql = "select A.KP_COUNT FROM sfis1.c_bom_keypart_t a,sfism4.r_wip_tracking_t b "
                            + "WHERE a.bom_no=b.bom_no AND b.serial_number='" + dataPro + "' "
                            + "AND a.group_name ='" + mGroup + "'";
                        var _result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_result.Data != null)
                        {
                            kpCount = Int32.Parse(_result.Data["kp_count"].ToString());
                        }
                        await InputSN(lblSerial.Content.ToString());
                    }
                }
                else
                {
                    var procedure = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.CHECK_DUP_KPSN_ASSYKPS_LOT",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="DATA",Value=txtAss,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.InputOutput },
                                    new SfcParameter{Name="SN",Value=lblSerial.Content.ToString(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="MYGROUP",Value=mGroup,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                    new SfcParameter{Name="COMMAND1",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                    new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                                }
                    });
                    ads = procedure.Data;
                    dataPro = ads[0]["data"];
                    res = ads[2]["res"];
                    if (res != "OK")
                    {
                        lblRec.Content = res;
                        lblRec.Foreground = Brushes.Red;
                        showMessage("SFIS1.CHECK_DUP_KPSN_ASSYKPS_LOT reponse: " + res, res, true);
                        txtSend.SelectAll();
                        txtSend.Focus();
                        return;
                    }
                    else
                    {
                        kpLong = ads[1]["command1"];
                        sql = "select a.BOM_NO FROM sfis1.c_bom_keypart_t a,sfism4.r_wip_tracking_t b "
                            + "WHERE a.bom_no=b.bom_no AND b.serial_number='" + lblSerial.Content.ToString() + "'"
                            + "AND a.KEY_PART_NO ='" + kpLong + "'";
                        var _resulthh = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
                        if (_resulthh.Data == null)
                        {
                            lblRec.Content = "Key part " + kpLong + " not in BOM";
                            lblRec.Foreground = Brushes.Red;
                            showMessage("Key part " + kpLong + " not in BOM", "Key part " + kpLong + " not in BOM", true);
                            txtSend.SelectAll();
                            txtSend.Focus();
                            return;
                        }
                        if (kpLong != txtAss)
                        {
                            if (lstLong.Items.Count != 0)
                            {
                                for (int i = 0; i < lstLong.Items.Count; i++)
                                {
                                    if (lstLong.Items[i].ToString().IndexOf(txtAss + "|" + kpLong) == -1)
                                    {
                                        lstLong.Items.Add(txtAss + "|" + kpLong);
                                        lblRec.Content = "OK OK";
                                        lblRec.Foreground = Brushes.Yellow;
                                        dem++;
                                        if (dem == kpCount)
                                        {
                                            refresh("END");
                                            return;
                                        }
                                        txtSend.SelectAll();
                                        txtSend.Focus();
                                        return;
                                    }
                                    else
                                    {
                                        lblRec.Content = "DUPLICATE " + txtAss;
                                        lblRec.Foreground = Brushes.Red;
                                        showMessage("DUPLICATE " + txtAss, "Trùng lặp " + txtAss, true);
                                        txtSend.SelectAll();
                                        txtSend.Focus();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                lstLong.Items.Add(txtAss + "|" + kpLong);
                                lblRec.Content = "OK OK";
                                lblRec.Foreground = Brushes.Yellow;
                                dem++;
                                if (dem == kpCount)
                                {
                                    refresh("END");
                                    return;
                                }
                            }
                            txtSend.SelectAll();
                            txtSend.Focus();
                            return;
                        }
                        else
                        {
                            if (lstShost.Items.Count != 0)
                            {
                                for (int i = 0; i < lstShost.Items.Count; i++)
                                {
                                    if (lstLong.Items[i].ToString().IndexOf(txtAss) == -1)
                                    {
                                        lstShost.Items.Add(txtAss);
                                        lblRec.Content = "OK OK";
                                        lblRec.Foreground = Brushes.Yellow;
                                        txtSend.SelectAll();
                                        txtSend.Focus();
                                        return;
                                    }
                                    else
                                    {
                                        lblRec.Content = "DUPLICATE " + txtAss;
                                        lblRec.Foreground = Brushes.Red;
                                        showMessage("DUPLICATE " + txtAss, "Trùng lặp " + txtAss, true);
                                        txtSend.SelectAll();
                                        txtSend.Focus();
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                lstShost.Items.Add(txtAss);
                                lblRec.Content = "OK OK";
                                lblRec.Foreground = Brushes.Yellow;
                                txtSend.SelectAll();
                                txtSend.Focus();
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
