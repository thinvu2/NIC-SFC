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
using Sfc.Library.HttpClient.Helpers;
using CQC.ViewModel;
using Sfc.Library.HttpClient;

namespace CQC
{
    /// <summary>
    /// Interaction logic for Form_checkboxssn.xaml
    /// </summary>
    public partial class CheckboxssnWindow : Window
    {
        bool flagKP, flagSSN, CHECK_CUST;
        string M_MODEL, M_TYPE, mo;
        private SfcHttpClient sfcClient;

        private void SetupScanCustSN1_Click(object sender, RoutedEventArgs e)
        {
            SetupScanCustSN1.IsChecked = !SetupScanCustSN1.IsChecked;
            if (SetupScanCustSN1.IsChecked)
            {
                SetupScanCustSN1.IsChecked = false;
                Label5.Visibility = Visibility.Collapsed;
                lb_dataCust.Visibility = Visibility.Collapsed;
            }
            else
            {
                Form4Window Form4 = new Form4Window(this);
                Form4.Owner = this;
                Form4.ShowDialog();
            }
        }

        private void LOGIN1_Click(object sender, RoutedEventArgs e)
        {
            frloginWindow frlogin = new frloginWindow(this, formCQC, this.sfcClient);
            frlogin.Owner = this;
            frlogin.ShowDialog();
        }

        private MainWindow formCQC;
        private async void checkssn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string tmpKeypart, tmpboxssn, res;
                string SSQL = "select station_number,line_name,section_name,station_name,sysdate,  " +
                               " to_char(sysdate,'YYYYMMDD') ww,to_char(sysdate,'HH24') zz,group_name " +
                               " from sfis1.c_station_config_t where line_name='L1' and group_name='FQA' and rownum=1  ";
                var qurySelect = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = SSQL,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qurySelect.Data == null)
                {
                    MessageBox.Show("Line config error", "CQC");
                    return;
                }

                if (checkssn.Text.Length > 40 && !SetupScanCustSN1.IsChecked)
                {
                    if (checkssn.Text.Substring(0, 5) == "WIFI:" && checkssn.Text.Contains("SN:") && checkssn.Text.Contains("SK:"))
                        checkssn.Text = checkssn.Text.Substring(checkssn.Text.IndexOf("SN:") + 3, checkssn.Text.IndexOf(";SK") - checkssn.Text.IndexOf("SN:") - 3);
                    else
                        checkssn.Text = checkssn.Text.Substring(0, checkssn.Text.IndexOf(","));
                    SSQL = "SELECT SSN13,SSN1 FROM SFISM4.R_CUSTSN_T WHERE SSN1='" + checkssn.Text + "' OR SSN13='" + checkssn.Text + "' OR MAC2='" + checkssn.Text + "'";
                    var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = SSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qury.Data == null)
                        return;
                    tmpKeypart = qury.Data["ssn1"].ToString();
                }


                if (checkssn.Text == "UNDO" && lb_hoursing.Content.ToString() != "N/A" && chkMBD.IsChecked == false)
                {
                    var quryDeleteKP = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "DELETE SFIS1.C_TEMP_KEYPART_T WHERE SERIAL_NUMBER IN (SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE SHIPPING_SN='" + lb_hoursing.Content + "')",
                        SfcCommandType = SfcCommandType.Text
                    });

                    quryDeleteKP = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "UPDATE SFISM4.R107 SET SECTION_FLAG='' WHERE SHIPPING_SN= '" + lb_hoursing.Content + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    lb_hoursing.Content = "N/A";
                    lb_box.Content = "N/A";
                    label_pass.Content = "Ready";
                    label_pass.Foreground = Brushes.Olive;
                    pnl1.Visibility = Visibility.Collapsed;
                    flagKP = true;
                    flagSSN = true;
                    memoRec.Text = "             Cmd:    HOUSING SSN ?";
                    checkssn.SelectAll();
                    return;
                }
                if (SetupScanCustSN1.IsChecked)
                {
                    if (lb_dataCust.Content.ToString() != "N/A")
                        CHECK_CUST = true;
                    else
                        CHECK_CUST = false;
                }
                else
                    CHECK_CUST = true;

                if ((lb_hoursing.Content.ToString() == "N/A" || lb_box.Content.ToString() != "N/A") && flagSSN)
                {
                    if (lb_box.Content.ToString() != "N/A" && flagKP == false)
                    {
                        lb_hoursing.Content = "N/A";
                        lb_box.Content = "N/A";
                        lb_KP.Content = "N/A";
                        lblKPN.Content = "N/A";
                    }
                    label_pass.Content = "TEST";
                    label_pass.Foreground = Brushes.Yellow;
                    var tmpQry2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE  SHIPPING_SN ='" + checkssn.Text + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (tmpQry2.Data.Count() == 0)
                    {
                        MessageBox.Show(await formCQC.GetPubMessage("00612"), "CQC");
                        label_pass.Content = "FAIL";
                        label_pass.Foreground = Brushes.Red;
                        checkssn.SelectAll();
                        Memo1.Text += checkssn.Text;
                        memoRec.Text = "             Cmd:    HOUSING SSN ?";
                        return;
                    }
                    else
                    {
                        List<R107> datalist = new List<R107>();
                        datalist = tmpQry2.Data.ToListObject<R107>().ToList();
                        M_MODEL = datalist[0].MODEL_NAME;
                        if (datalist[0].QA_RESULT == "4")
                        {
                            MessageBox.Show(await formCQC.GetPubMessage("80173"), "CQC");
                            label_pass.Content = "FAIL";
                            label_pass.Foreground = Brushes.Red;
                            checkssn.SelectAll();
                            Memo1.Text += checkssn.Text;
                            memoRec.Text = "             Cmd:    HOUSING SSN ?";
                            return;
                        }
                        else
                        {
                            if (!(datalist[0].SCRAP_FLAG == "0" && datalist[0].QA_RESULT == "N/A"))
                            {
                                MessageBox.Show(await formCQC.GetPubMessage("80174"), "CQC");
                                label_pass.Content = "FAIL";
                                label_pass.Foreground = Brushes.Red;
                                checkssn.SelectAll();
                                Memo1.Text += checkssn.Text;
                                memoRec.Text = "             Cmd:    HOUSING SSN ?";
                                return;
                            }
                            else
                            {
                                lb_hoursing.Content = checkssn.Text;
                                checkssn.SelectAll();
                                Memo1.Text += checkssn.Text;
                                memoRec.Text = "Msg: HOUSING SSN  OK ";
                                memoRec.Text += "             Cmd:    BOX SSN ?";
                            }
                        }
                    }


                }
                else if (lb_box.Content.ToString() == "N/A")
                {
                    if (lb_KP.Content.ToString() == "N/A")
                    {
                        var tmpQuery2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "SELECT MODEL_TYPE FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='" + M_MODEL + "'",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (tmpQuery2.Data == null)
                            return;
                        M_TYPE = tmpQuery2.Data["model_type"].ToString();
                        if (tmpQuery2.Data.Count > 0 && !string.IsNullOrEmpty(M_TYPE))
                        {
                            if (M_TYPE.Contains("185"))
                            {
                                tmpQuery2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = "SELECT SSN12 FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER IN(SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE  SHIPPING_SN = '" + lb_hoursing.Content.ToString().Trim() + "')",
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (tmpQuery2.Data == null)
                                    return;
                                tmpKeypart = tmpQuery2.Data["ssn12"].ToString();
                                if (string.IsNullOrEmpty(tmpKeypart) && tmpKeypart != checkssn.Text)
                                {
                                    MessageWindow mes = new MessageWindow();
                                    mes.txbenglish.Text = "Adapter PN :" + checkssn.Text + " not Correct!";
                                    mes.txbvietnamese.Text = "Sạc:" + checkssn.Text + "không đúng!";
                                    mes.Owner = this;
                                    mes.ShowDialog();
                                    Memo1.Text += "KP: " + checkssn.Text;
                                    checkssn.SelectAll();
                                    label_pass.Content = "FAIL";
                                    label_pass.Foreground = Brushes.Red;
                                    lb_hoursing.Content = "N/A";
                                    lb_box.Content = "N/A";
                                    lb_KP.Content = "N/A";
                                    memoRec.Text = "             Cmd:    HOUSING SSN?";
                                    return;
                                }
                                else
                                {
                                    lb_KP.Content = checkssn.Text;
                                    label_pass.Foreground = Brushes.Aqua;
                                    Memo1.Text += "KP: " + checkssn.Text;
                                    checkssn.SelectAll();
                                    memoRec.Text = "       Cmd:    ADAPTER PN  OK ";
                                    memoRec.Text += "               Cmd:    BOX SSN ?";
                                    return;
                                }
                            }
                            if (M_TYPE.Contains("G05"))
                            {
                                tmpQuery2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = "SELECT SSN13 FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER IN(SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE  SHIPPING_SN ='" + lb_hoursing.Content.ToString().Trim() + "')",
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (tmpQuery2.Data == null)
                                    return;
                                tmpboxssn = tmpQuery2.Data["ssn13"].ToString();
                                if (string.IsNullOrEmpty(tmpboxssn) && tmpboxssn != checkssn.Text)
                                {
                                    MessageWindow mes = new MessageWindow();
                                    mes.txbenglish.Text = "DNI PN :" + checkssn.Text + " not Correct!";
                                    mes.txbvietnamese.Text = "DNI PN :" + checkssn.Text + "không đúng!";
                                    mes.Owner = this;
                                    mes.ShowDialog();
                                    Memo1.Text += "KP: " + checkssn.Text;
                                    checkssn.SelectAll();
                                    label_pass.Content = "FAIL";
                                    label_pass.Foreground = Brushes.Red;
                                    lb_hoursing.Content = "N/A";
                                    lb_box.Content = "N/A";
                                    lb_KP.Content = "N/A";
                                    memoRec.Text = "             Cmd:    HOUSING SSN?";
                                    return;
                                }
                                else
                                {
                                    flagSSN = false;
                                    Memo1.Text += checkssn.Text;
                                    memoRec.Text = "Msg: BOX SSN  OK ";
                                    memoRec.Text += "             Cmd:    KPN ?";
                                    checkssn.SelectAll();
                                }
                            }

                        }
                    }

                    if (M_TYPE.Contains("G05"))
                    {
                        var tmpqury2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "SELECT * FROM SFISM4.R_CUSTSN_T where SSN13='" + checkssn.Text + "' OR MAC2='" + checkssn.Text + "' OR SSN1='" + checkssn.Text + "'",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (tmpqury2.Data == null)
                            return;
                        checkssn.Text = tmpqury2.Data["ssn1"].ToString();
                    }

                    if (lb_hoursing.Content.ToString() == checkssn.Text && lb_box.Content.ToString() == "N/A")
                    {
                        var tmpQry2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "SELECT SECTION_FLAG FROM SFISM4.R_WIP_TRACKING_T  WHERE  SHIPPING_SN ='" + checkssn.Text + "'",
                            SfcCommandType = SfcCommandType.Text
                        });
                        string section_flag = string.Empty;
                        if (tmpQry2.Data.Count() > 0)
                        {
                            List<R107> secList = new List<R107>();
                            secList = tmpQry2.Data.ToListObject<R107>().ToList();
                            section_flag = secList[0].SECTION_FLAG;
                        }
                        if (section_flag == "1" && chkMBD.IsChecked == false)
                        {
                            label_pass.Content = "FAIL";
                            label_pass.Foreground = Brushes.Red;
                            checkssn.SelectAll();
                            Memo1.Text += checkssn.Text;
                            memoRec.Text = "Msg: KPS INPUT FINISH";
                            memoRec.Text += "             Cmd:    HOUSING SSN ?";
                            lb_hoursing.Content = "N/A";
                            lb_box.Content = "N/A";
                            return;
                        }
                        else if (chkMBD.IsChecked == false)
                        {
                            var checkdup = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "SFIS1.CHECK_SN_DUP",
                                SfcCommandType = SfcCommandType.StoredProcedure,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="DATA" , Value=checkssn.Text , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="SN" , Value=lb_hoursing.Content.ToString() , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="STATION_NUM" , Value = qurySelect.Data["station_number"].ToString(), SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="RES" , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Output }
                                }
                            });
                            dynamic res1 = checkdup.Data;
                            res = res1[0]["res"];
                            if (res.Substring(0, 2) != "OK")
                            {
                                label_pass.Content = "FAIL";
                                label_pass.Foreground = Brushes.Red;
                                checkssn.SelectAll();
                                Memo1.Text += checkssn.Text;
                                lb_hoursing.Content = "N/A";
                                lb_box.Content = "N/A";
                                return;
                            }

                            var quryMo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "select MO_NUMBER FROM SFISM4.R107 WHERE SERIAL_NUMBER='" + lb_hoursing.Content + "' OR SHIPPING_SN='" + lb_hoursing.Content + "' AND ROWNUM=1",
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (quryMo.Data == null)
                                return;
                            mo = quryMo.Data["mo_number"].ToString();
                            lb_box.Content = checkssn.Text;
                            flagSSN = false;
                            Memo1.Text += checkssn.Text;
                            memoRec.Text = "Msg: BOX SSN  OK ";
                            if (SetupScanCustSN1.IsChecked)
                                memoRec.Text += Label5.Content + "?";
                            else
                                memoRec.Text += "             Cmd:    KPN ?";
                            checkssn.SelectAll();
                            return;
                        }
                        else
                        {
                            lb_box.Content = checkssn.Text;
                            Memo1.Text += checkssn.Text;
                            memoRec.Text = "Msg: BOX SSN  OK ";
                            memoRec.Text += "            Cmd:   HOUSING SSN ?";
                            label_pass.Content = "PASS";
                            label_pass.Foreground = Brushes.Blue;
                            label_pass.Background = Brushes.Lime;
                            SSQL = " UPDATE  SFISM4.R_WIP_TRACKING_T   SET   QA_RESULT = '4' " +
                                        "WHERE  SHIPPING_SN ='" + lb_hoursing.Content.ToString() + "'  AND SCRAP_FLAG ='0' AND QA_RESULT ='N/A' ";
                            var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SSQL,
                                SfcCommandType = SfcCommandType.Text
                            });
                            flagKP = false;
                            flagSSN = true;
                            checkssn.SelectAll();
                        }
                    }
                    else
                    {
                        label_pass.Content = "FAIL";
                        label_pass.Foreground = Brushes.Red;
                        lb_box.Content = "N/A";
                        lb_hoursing.Content = "N/A";
                        Memo1.Text += checkssn.Text;
                        memoRec.Text = "             Cmd:    HOUSING SSN ?";
                        checkssn.SelectAll();
                        flagKP = false;
                        flagSSN = true;
                        return;
                    }
                }
                else if (SetupScanCustSN1.IsChecked && lb_dataCust.Content.ToString() == "N/A")
                {
                    if (lb_hoursing.Content.ToString() == "N/A" && lb_box.Content.ToString() != "N/A" && checkssn.Text != "END" && chkMBD.IsChecked == false && lb_dataCust.Content.ToString() != "N/A")
                    {
                        var qryCustSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "SELECT * FROM SFISM4.R_CUSTSN_T WHERE " + Label5.Content + " = '" + checkssn.Text + "' and ssn1  ='" + lb_hoursing.Content.ToString() + "'",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qryCustSN.Data.Count() == 0)
                        {
                            MessageBox.Show(await formCQC.GetPubMessage("80174"), "CQC");
                            label_pass.Content = "FAIL";
                            label_pass.Foreground = Brushes.Red;
                            checkssn.SelectAll();
                            Memo1.Text += checkssn.Text;
                            memoRec.Text = "             Cmd:    CUST SSN ?";
                            return;
                        }
                        else
                        {
                            lb_dataCust.Content = checkssn.Text;
                            checkssn.SelectAll();
                            Memo1.Text += checkssn.Text;
                            memoRec.Text = "Msg: CUST SSN  OK ";
                            memoRec.Text += "             Cmd:    KPN ?";
                        }
                    }
                }

                if (lb_hoursing.Content.ToString() != "N/A" && lb_box.Content.ToString() != "N/A" && checkssn.Text != "END" && chkMBD.IsChecked == false && CHECK_CUST)
                {
                    Memo1.Text += checkssn.Text;
                    try
                    {
                        var excuteproc = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "SFIS1.CHECK_LOTKP_TO_NETGER",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="STATION_NUM" , Value="" , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="DATA" , Value=checkssn.Text , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="SN" , Value = lb_hoursing.Content.ToString(), SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="EMP" , Value = formCQC.sEmpNO, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="RES" , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Output }
                                }
                        });

                        dynamic res1 = excuteproc.Data;
                        res = res1[0]["res"];
                        if (res.Substring(0, 2) != "OK")
                        {
                            memoRec.Text = "Msg: " + res;
                            memoRec.Text += "                Cmd: KPN?";
                            pnl1.Visibility = Visibility.Visible;
                            pnl1.Content = "NG";
                            pnl1.Foreground = Brushes.Red;
                            checkssn.SelectAll();
                            return;
                        }
                        else
                        {
                            lblKPN.Content = checkssn.Text;
                            memoRec.Text = "Msg: KPS OK " + res;
                            memoRec.Text += "               Cmd: KPN?";
                            memoRec.Text += "END?";
                            pnl1.Visibility = Visibility.Visible;
                            pnl1.Content = "OK";
                            pnl1.Foreground = Brushes.Lime;
                            checkssn.SelectAll();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("excute SFIS1.CHECK_LOTKP_TO_NETGER proc error", "CQC");
                    }
                }

                if ((checkssn.Text == "END") && (lb_hoursing.Content.ToString() != "N/A") && (lb_box.Content.ToString() != "N/A") && (chkMBD.IsChecked == false) && (lblKPN.Content.ToString() != "N/A"))
                {
                    Memo1.Text += checkssn.Text;
                    var sprocInsertKps = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SFIS1.INSERT_R108_KP_LOTKP_OBA",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="EMP" , Value=formCQC.sEmpNO , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="LINE" ,Value= qurySelect.Data["line_name"].ToString() , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="SECTION" , Value = qurySelect.Data["section_name"].ToString(), SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="MYGROUP" , Value = qurySelect.Data["group_name"].ToString(), SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="STATION_NUM" , Value=qurySelect.Data["station_number"].ToString() , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="W_STATION" , Value=qurySelect.Data["station_name"].ToString() , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="DATA" , Value = checkssn.Text, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="SN" , Value = lb_hoursing.Content.ToString(), SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="KPN" , Value = "", SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="DATETIME" , Value = DateTime.Parse(qurySelect.Data["sysdate"].ToString()), SfcParameterDataType=SfcParameterDataType.Date, SfcParameterDirection=SfcParameterDirection.Input},
                                    new SfcParameter {Name="RES" , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Output }
                                }
                    });
                    dynamic res1 = sprocInsertKps.Data;
                    res = res1[0]["res"];
                    if (res.Substring(0, 2) != "OK")
                    {
                        memoRec.Text = "Msg: " + res;
                        memoRec.Text += "               Cmd: KPN?";
                        memoRec.Text += "END?";
                        pnl1.Visibility = Visibility.Visible;
                        pnl1.Content = "NG";
                        pnl1.Foreground = Brushes.Red;
                        checkssn.SelectAll();
                    }
                    else
                    {
                        memoRec.Text = "Msg: END OK " + res;
                        memoRec.Text += "        Cmd: HOUSING SSN?";
                        label_pass.Content = "PASS";
                        //label_pass.Foreground = Brushes.Lime;
                        pnl1.Visibility = Visibility.Collapsed;
                        var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "UPDATE  SFISM4.R_WIP_TRACKING_T   SET   QA_RESULT = '4' WHERE  SHIPPING_SN ='" + lb_hoursing.Content.ToString() + "'  AND SCRAP_FLAG ='0' AND QA_RESULT ='N/A'",
                            SfcCommandType = SfcCommandType.Text
                        });
                        flagKP = false;
                        flagSSN = true;
                        checkssn.SelectAll();
                    }
                }
            }

        }

        public CheckboxssnWindow(MainWindow _formCQC, SfcHttpClient _sfcClient)
        {
            formCQC = _formCQC;
            sfcClient = _sfcClient;
            InitializeComponent();
        }
        public CheckboxssnWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            flagKP = true;
            flagSSN = true;
            memoRec.Text = "  Cmd:    HOUSING SSN ?";
        }
    }
}
