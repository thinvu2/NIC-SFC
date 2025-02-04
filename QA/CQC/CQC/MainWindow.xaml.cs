using AMSLabel;
using CQC.ViewModel;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;

namespace CQC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string checkSum;
        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUri = "";
        public string loginDB = "";
        public string empNo = "";
        public string empPass = "";
        public string inputLogin = "";
        INIFile ini = new INIFile("SFIS.ini");
        public bool onProcess = false;
        private DispatcherTimer dispatcherTimer;
        public bool pass_chkRelation, bLogin, CPEIIIFLAG, NOcheckcontrolrun, isMcarton;
        public string str_model_name = "" , PALET_NO  ="";
        public string G_sReasonCode, G_sType, G_sLocation, G_sDutycode, G_VERSION_SHOW, Sn_pre, Sn_Post, s_ModelType = "", sMyLine, sMySection, sMyGroup, sMyStation, sIinLine, G_sTester;
        public string sPO, sPOLine, sCompany, sErrorMessage, sSelectPallet, sSelectPallet2, sEmpNO, cust_carton_postfix, upceandata;
        private string sSerialNO;
        ComboBox comb1 = new ComboBox();
        public List<string> ssn_Stringlist = new List<string>();
        public List<string> mac_Stringlist = new List<string>();
        public int iChangeLine, SSNLENGTH;
        private string ATE_FLAG, G_sLotNo, G_sPalletNo, G_sRL, G_sModelName, G_sMO, G_Class, G_Class_Date;
        private int G_iComboItem, G_iCritical, G_iMajor, G_iMinor, G_Work_Section, G_iWSection;
        public string C_pallet_no;
        public bool po_flag, isPassword;
        public bool RoKuCheck;
        string sClass, sDay_Distinct;
        int mo_needQty1, slpallet, mo_qty1, qtyneed1, slpalletreal;
        double qtyneed;
        List<R107> RoKuList = new List<R107>();
        //binding formRejectReason
        RejectReasonWindow formRejectReason;
        string RejectReason = string.Empty;
        //public ComboBox combNextGroup = new ComboBox();
        //public RichTextBox memo1 = new RichTextBox();

        //biding formFromGroup.combGroup.text 
        string combGroup = "";

        //biding UnitCountWindow.lablUnitCount
        string lablUnitCount = "";
        string editUnitCount = "";

        //biding formCQCUnit.editCQCUnit.Text
        public string editCQCUnit = "";
        //binding formSystem
        bool chkbRejectReason, chkbCheckRoute, chkbUpdateQANoResult, chkbRemoveFailSSN, chkbInsertSNtoPallet, chkbTransferbyPiece, chkbWarehouseNO, chkbPO, chkbClearPallet, chkbClearCarton;
        public bool chkbSamplingPlan, chkbInsertQCSN;


        string editPalletFullFlag, editCompany;

        //binding formDefault
        public bool logic1;
        public bool ModalResult;

        public List<R107> qurySerN = new List<R107>();
        List<R107> CQCErr_List = new List<R107>();

        // list form dutytypeWindow
        ListBox ListBoxLocation = new ListBox();
        ListBox ListBoxReasonCode = new ListBox();
        ListBox ListBoxDutyCode = new ListBox();

        //binding formSetupLine
        public ListView listSelectLine = new ListView();

        //binding formsetup length
        public string SNEdit = "12";
        public string EditStation = "";

        //binding form2
        public bool ispass;

        //binding formWarehouseNo
        public string editWarehouseNO;

        //binding formYes
        public bool MrOK = false;

        //com
        public SerialPort _serialPort;


        private async void bbtnReject_Click(object sender, RoutedEventArgs e)
        {
            string sSQL;
            int iFail, iPass, iLotSize, iTotal, iCheck;

            if (itemFullSampling.IsChecked == false)
                await bbtnRefreshClick();
            sSQL = "select SERIAL_NUMBER from SFISM4.R_WIP_TRACKING_T where qa_no=:lot_no" +
                        " AND((Tray_NO  IN(Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag = 'Tray')) " +
                        " OR(Mcarton_NO  IN(Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag = 'Carton')) " +
                        " OR(IMEI  IN(Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag = 'Pallet')))  AND RowNum = 1 ";

            var sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name = "lot_no" , Value=combLot.Text }
                }
            });
            if (sql.Data.Count() != 0)
            {
                lablMsg.Foreground = Brushes.Red;
                lablMsg.Content = "This Pallet/Carton is not complete!!";
                MessageBox.Show("This Pallet/Carton is not complete!!", "CQC");
                return;
            }
            sSQL = " Select Serial_Number from sfism4.r_wip_Tracking_t " +
                 " Where qa_No =:Qa_no ";
            sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name = "Qa_no" , Value=combLot.Text }
                }
            });
            iTotal = sql.Data.Count();
            sSQL = " Select Serial_Number from sfism4.r_QC_SN_T " +
                   " Where Lot_no=:lot_No and Counter=0 ";
            sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name = "lot_No" , Value=combLot.Text }
                }
            });
            iCheck = iTotal / Int32.Parse(editCheckQty.Text);
            if (iTotal % Int32.Parse(editCheckQty.Text) != 0)
            {
                iCheck = iCheck + 1;
            }
            if (iCheck > sql.Data.Count())
            {
                if (chkbCheckQty.IsChecked == true)
                {
                    MessageBox.Show(await GetPubMessage("00588") + " " + (iCheck - sql.Data.Count()) + " PCS");
                    return;
                }
                else
                    MessageBox.Show(await GetPubMessage("00588") + " " + (iCheck - sql.Data.Count()) + " PCS");
            }
            if (combLot.Text != "")
            {
                YesDlgWindow formYesDlg = new YesDlgWindow(this);
                formYesDlg.Title = "REJECT";
                formYesDlg.lablMesg.Content = "REJECT?";
                formYesDlg.Owner = this;
                formYesDlg.ShowDialog();

                if (MrOK)
                {
                    if (chkbRejectReason)
                    {
                        if (formRejectReason == null)
                            formRejectReason = new RejectReasonWindow(this);
                        formRejectReason.meno1.Document.Blocks.Clear();
                        formRejectReason.Owner = this;
                        formRejectReason.ShowDialog();
                        TextRange richText = new TextRange(formRejectReason.meno1.Document.ContentStart, formRejectReason.meno1.Document.ContentEnd);
                        RejectReason = richText.Text;
                    }
                    //Reject1  Update qa_result , in_station_time
                    sSQL = " update sfism4.r_wip_tracking_t set  qa_result = '1' ,IN_STATION_TIME =sysdate where qa_no = '" + combLot.Text + "'";
                    var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });

                    //Reject2  Update next_station
                    if (formRejectReason.combNextGroup.Text == "")
                    {
                        sSQL = " update sfism4.r_wip_tracking_t set  next_station='" + sMyGroup + "' " +
                          " where qa_no ='" + combLot.Text + "' and error_flag <> '1' ";
                    }
                    else
                    {
                        sSQL = " update sfism4.r_wip_tracking_t set  next_station='" + formRejectReason.combNextGroup.Text + "' " +
                          " where qa_no ='" + combLot.Text + "' and error_flag <> '1' ";
                    }
                    var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });

                    //lot size
                    sSQL = " select serial_number from SFISM4.R_WIP_TRACKING_T where qa_no = :qa_no ";
                    sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name = "qa_no" , Value=combLot.Text }
                        }
                    });
                    iLotSize = sql.Data.Count();

                    //pass count
                    sSQL = " select serial_number from SFISM4.R_QC_SN_T where lot_no = '" + combLot.Text + "' and ERROR_FLAG=0 AND COUNTER=0 ";
                    if (chkbInsertQCSN == true)
                    {
                        sSQL = sSQL + " and Group_name='" + sMyGroup + "' ";
                    }
                    sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    iPass = sql.Data.Count();

                    sSQL = " select serial_number from SFISM4.R_QC_SN_T where lot_no = '" + combLot.Text + "' and ERROR_FLAG=1 AND COUNTER=0 ";
                    if (chkbInsertQCSN == true)
                    {
                        sSQL = sSQL + " and Group_name='" + sMyGroup + "' ";
                    }
                    sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    iFail = sql.Data.Count();

                    await FindWorkSection(sMyLine);
                    if (sDay_Distinct == "TODAY")
                        sDay_Distinct = DateTime.Now.ToString("yyyyMMdd");
                    if (sDay_Distinct == "TOMORROW")
                        sDay_Distinct = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                    if (sDay_Distinct == "YESTERDAY")
                        sDay_Distinct = DateTime.Now.AddDays(+1).ToString("yyyyMMdd");

                    sSQL = " update sfism4.r_cqc_rec_t " +
                            " set  qa_result = '1', ng_num = ng_num + 1, lot_size = :lot_size " +
                            "     ,PASS_QTY = :pass , Fail_Qty = :Fail " +
                            "     ,END_TIME = sysdate  " +
                            "     ,Class = :Class " +
                            "     ,Class_Date = :Class_Date ,PO_NUMBER  =:PO_NUMBER ";
                    if (chkbRejectReason == true)
                    {
                        sSQL = sSQL + " ,REJECT_REASON  ='" + RejectReason + "' ";
                    }
                    if (chkbSamplingPlan == true)
                    {
                        sSQL = sSQL + " ,MAJOR_FAIL_QTY=0,MINOR_FAIL_QTY=0,CRITICAL_FAIL_QTY=0 ";
                    }
                    sSQL = sSQL + " where Lot_No = :Lot_No ";

                    update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{ Name = "lot_size", Value =iLotSize, SfcParameterDataType = SfcParameterDataType.Varchar2},
                            new SfcParameter{ Name = "pass", Value =iPass, SfcParameterDataType = SfcParameterDataType.Varchar2 },
                            new SfcParameter{ Name = "Fail", Value =iFail, SfcParameterDataType = SfcParameterDataType.Varchar2 },
                            new SfcParameter{ Name = "Class", Value =sClass, SfcParameterDataType = SfcParameterDataType.Varchar2 },
                            new SfcParameter{ Name = "Class_Date", Value =sDay_Distinct, SfcParameterDataType = SfcParameterDataType.Varchar2 },
                            new SfcParameter{ Name = "PO_NUMBER", Value =ComboBox1.Text, SfcParameterDataType = SfcParameterDataType.Varchar2 },
                            new SfcParameter{ Name = "Lot_No", Value =combLot.Text, SfcParameterDataType = SfcParameterDataType.Varchar2 }
                        }
                    });

                    update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "update sfism4.r_qc_sn_t set counter = counter+1 where lot_no = '" + combLot.Text + "' and COUNTER <> 99",
                        SfcCommandType = SfcCommandType.Text
                    });//update QC_SN

                    //insert R_REJECT_CQC_REC_T
                    sSQL = " insert into SFISM4.R_REJECT_CQC_REC_T " +
                            "   select * from SFISM4.R_CQC_REC_T " +
                            "    where lot_no='" + combLot.Text + "' ";
                    update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });

                    dbgridSSN.ItemsSource = null; dbgridErrorCode.ItemsSource = null; qurySerN = null;
                    lablMsg.Foreground = Brushes.Purple;
                    lablMsg.Content = "Reject " + combLot.Text + " completed.";
                    //clstrgridInLot.RowCount = 2;
                    editCheckedQty.Content = "0"; editFailedQty.Content = "0"; editCount.Content = "0"; G_sLotNo = "";
                    editLimitQty.Content = "0";
                    editPassRate.Content = "";
                    G_sModelName = string.Empty;
                    G_sMO = string.Empty;
                    editMoNo.Text = "";
                    editModel.Text = "";
                    //clstrgridNotChecked.RowCount = 2;
                    //clstrgridInLot.RowCount = 2;
                    clstrgridInLot.ItemsSource = null;
                    clstrgridNotChecked.ItemsSource = null;
                    comb1.Items.Clear();
                    combLot.Text = "";
                    //combEmp.Text = "";
                    combDef.Text = "";
                    editCritical.Text = "0";
                    editMajor.Text = "0";
                    editMinor.Text = "0";
                    editSampleQty.Text = "0";
                    editOQAType.Text = "";
                    combDef.Focus();
                    G_iComboItem = -1;
                    await displayCombLot();
                }
            }
            combDef.Focus();
        }

        private void cmdCheck_Click(object sender, RoutedEventArgs e)
        {
            chkRelation1_Click(new object(), new RoutedEventArgs());
        }

        async Task combLot_SelectionChanged1()
        {
            if (itemSamplingSN.IsChecked == false && itemSamplingSSN.IsChecked == false)
            {
                MessageBox.Show("Please Choose Sampling Unit !!", "CQC");
                combDef.Text = "";
                return;
            }
            if (itemSSN.IsChecked == false && itemTray.IsChecked == false && itemCarton.IsChecked == false && itemPallet.IsChecked == false)
            {
                MessageBox.Show("Please Choose CQC by Unit !!", "CQC");
                combDef.Text = "";
                return;
            }
            if (itemModel.IsChecked == false && itemMO.IsChecked == false && itemPO.IsChecked == false)
            {
                MessageBox.Show("Please Choose MO / Model !!", "CQC");
                combDef.Text = "";
                return;
            }

            if (combLot.SelectedIndex != -1 && G_iComboItem != combLot.SelectedIndex)
            {
                await inquireLot();
                if ((itemModel.IsChecked && !string.IsNullOrEmpty(G_sModelName)) || (itemMO.IsChecked && !string.IsNullOrEmpty(G_sMO)) || (itemPO.IsChecked && !string.IsNullOrEmpty(sPO)))
                {
                    if (itemRight.IsChecked)
                        await DisplayRight();
                    if (itemLeft.IsChecked)
                        await DisplayLeft();
                    await displayCqcSn();
                    if (chkbSamplingPlan && !await CheckSamplingPlan_sn())
                        return;
                    if (chkbSamplingPlan)
                        await qurySamplingPlan();
                }
                else
                {
                    clstrgridNotChecked.ItemsSource = null;
                    clstrgridInLot.ItemsSource = null;
                    editCount.Content = "0";
                    editCheckedQty.Content = "0";
                    editFailedQty.Content = "0";
                }
                await bbtnRefreshClick();
                G_iComboItem = combLot.SelectedIndex;
                comb1.Items.Clear();
                lablMsg.Foreground = Brushes.Blue;
                if (itemSamplingSN.IsChecked)
                    lablMsg.Content = "Please input Error code or Serial Number.";
                else
                    lablMsg.Content = "Please input Error code or Shipping S/N.";
            }
            combDef.Focus();
            chkTA.IsChecked = true;
        }

        async Task bbtnRefreshClick()
        {
            if (clstrgridInLot.Items.Count > 0)
            {
                foreach (R107 items in clstrgridInLot.Items)
                {
                    if (itemSSN.IsChecked && string.IsNullOrEmpty(items.SHIPPING_SN) && items.SHIPPING_SN == "N/A")
                    {
                        MessageBox.Show("SN or SSN is null,can not do FQA", "CQC");
                        return;
                    }
                    if (itemTray.IsChecked && string.IsNullOrEmpty(items.TRAY_NO) && items.TRAY_NO == "N/A")
                    {
                        MessageBox.Show("Tray NO is null,can not do FQA", "CQC");
                        return;
                    }
                    if (itemCarton.IsChecked && string.IsNullOrEmpty(items.CARTON_NO) && items.CARTON_NO == "N/A")
                    {
                        MessageBox.Show("Carton NO is null,can not do FQA", "CQC");
                        return;
                    }
                    if (itemPallet.IsChecked && string.IsNullOrEmpty(items.PALLET_NO) && items.PALLET_NO == "N/A")
                    {
                        MessageBox.Show("Pallet NO is null,can not do FQA", "CQC");
                        return;
                    }
                    string SQL = "";
                    SQL = "update sfism4.r_wip_tracking_t set qa_no = '" + combLot.Text + "' ";
                    if (itemSSN.IsChecked)
                    {
                        if (itemSamplingSN.IsChecked)
                            SQL += " where serial_number = '" + items.SERIAL_NUMBER + "'";
                        if (itemSamplingSSN.IsChecked)
                            SQL += " where shipping_sn = '" + items.SHIPPING_SN + "'";
                    }
                    if (itemTray.IsChecked)
                        SQL += " where Tray_NO = '" + items.TRAY_NO + "'";
                    if (itemCarton.IsChecked)
                        SQL += " where Carton_NO = '" + items.CARTON_NO + "'";
                    if (itemPallet.IsChecked)
                        SQL += " where Pallet_NO = '" + items.PALLET_NO + "'";

                    try
                    {
                        var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                        {
                            CommandText = SQL,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    catch
                    {
                        MessageBox.Show("Update QA_NO error!!!", "CQC");
                    }

                }
                if (itemRight.IsChecked)
                    await DisplayRight();
                if (itemLeft.IsChecked)
                    await DisplayLeft();
                await displayCqcSn();
            }
        }
        async Task DisplayRight()
        {
            int i = 0;
            string sSQL = "";
            if (itemSSN.IsChecked)
            {
                if (itemSamplingSN.IsChecked)
                {
                    sSQL = " SELECT SERIAL_NUMBER,QA_NO,QA_RESULT,COUNT(*) COUNT ";
                }
                else
                {
                    sSQL = " SELECT SHIPPING_SN,QA_NO,QA_RESULT,COUNT(*) COUNT ";
                }
            }
            else if (itemTray.IsChecked)
            {
                sSQL = " SELECT TRAY_NO,QA_NO,QA_RESULT,COUNT(*) COUNT ";
            }
            else if (itemCarton.IsChecked)
            {
                sSQL = " SELECT Carton_No,QA_NO,QA_RESULT,COUNT(*) COUNT ";
            }
            else if (itemPallet.IsChecked)
            {
                sSQL = " SELECT pallet_No,QA_NO,QA_RESULT,COUNT(*) COUNT ";
            }

            if (CheckSSN1.IsChecked)
            {
                sSQL = sSQL + " FROM SFISM4.R_WIP_TRACKING_T " +
                 " WHERE QA_NO='" + combLot.Text + "' AND ((SHIPPING_SN <> 'N/A') OR (SHIPPING_SN IS NOT NULL))";
            }
            else
            {
                sSQL = sSQL + " FROM SFISM4.R_WIP_TRACKING_T WHERE QA_NO='" + combLot.Text + "'";
            }

            if (itemSSN.IsChecked)
            {
                if (itemSamplingSN.IsChecked)
                {
                    sSQL = sSQL + " group by Serial_Number, qa_no, qa_result " +
                      " order by Serial_Number, qa_result ";
                }
                else
                {
                    sSQL = sSQL + " group by Shipping_SN, qa_no, qa_result " +
                    " order by Shipping_SN, qa_result ";
                }
            }
            else if (itemTray.IsChecked)
            {
                sSQL = sSQL + " group by TRAY_NO, qa_no,qa_result " +
                   " order by TRAy_No, qa_result ";
            }
            else if (itemCarton.IsChecked)
            {
                sSQL = sSQL + " group by Carton_no, qa_no, qa_result " +
                   " order by Carton_no, qa_result ";
            }
            else if (itemPallet.IsChecked)
            {
                sSQL = sSQL + " group by pallet_no, qa_no, qa_result " +
                      " order by pallet_no, qa_result ";
            }

            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });

            if (quryTemp != null && quryTemp.Data.Count() > 0)
            {
                List<R107> rightlist = new List<R107>();
                rightlist = quryTemp.Data.ToListObject<R107>().ToList();
                clstrgridInLot.ItemsSource = rightlist;
                if (itemTray.IsChecked)
                    this_lot1.Binding = new Binding("TRAY_NO");
                if (itemCarton.IsChecked)
                    this_lot1.Binding = new Binding("CARTON_NO");
                if (itemPallet.IsChecked)
                    this_lot1.Binding = new Binding("PALLET_NO");

                foreach (R107 items in rightlist)
                {
                    i += Int32.Parse(items.COUNT);
                }
            }
            editCount.Content = i.ToString();
            getLotAql();
        }

        async Task DisplayLeft()
        {
            string sSQL = "";
            if (itemSSN.IsChecked)
            {
                if (itemSamplingSN.IsChecked)
                {
                    sSQL = " SELECT SERIAL_NUMBER,QA_NO,QA_RESULT,COUNT(*) COUNT " +
                          " FROM SFISM4.R_WIP_TRACKING_T A,SFIS1.C_ROUTE_CONTROL_T B " +
                          " WHERE A.SPECIAL_ROUTE = B.ROUTE_CODE " +
                          " AND B.GROUP_NEXT = '" + sMyGroup + "' " +
                          " AND A.GROUP_NAME = B.GROUP_NAME " +
                          " AND A.SERIAL_NUMBER <>'N/A' ";
                    sSQL = sSQL + " AND (A.QA_NO = '' OR A.QA_NO='N/A') ";
                }
                else
                {
                    sSQL = " SELECT SHIPPING_SN,QA_NO,QA_RESULT,COUNT(*) COUNT " +
                          " FROM SFISM4.R_WIP_TRACKING_T A,SFIS1.C_ROUTE_CONTROL_T B " +
                          " WHERE A.SPECIAL_ROUTE = B.ROUTE_CODE " +
                          " AND B.GROUP_NEXT = '" + sMyGroup + "' " +
                          " AND A.GROUP_NAME = B.GROUP_NAME " +
                          " AND A.SHIPPING_SN <>'N/A' ";
                    sSQL = sSQL + " AND (A.QA_NO='' OR A.QA_NO='N/A') ";
                }
            }
            else if (itemTray.IsChecked)
            {
                sSQL = " SELECT Tray_No,QA_NO,QA_RESULT,COUNT(*)  COUNT" +
                       " FROM SFISM4.R_WIP_TRACKING_T A,SFIS1.C_ROUTE_CONTROL_T B " +
                       " WHERE A.SPECIAL_ROUTE = B.ROUTE_CODE " +
                       " AND B.GROUP_NEXT = '" + sMyGroup + "' " +
                       " AND A.GROUP_NAME = B.GROUP_NAME " +
                       " AND A.Tray_No <>'N/A' ";
                sSQL = sSQL + " AND (A.QA_NO='' OR A.QA_NO='N/A') ";
            }
            else if (itemCarton.IsChecked)
            {
                sSQL = " SELECT Carton_No,QA_NO,QA_RESULT,COUNT(*) COUNT " +
                       " FROM SFISM4.R_WIP_TRACKING_T A,SFIS1.C_ROUTE_CONTROL_T B " +
                       " WHERE A.SPECIAL_ROUTE = B.ROUTE_CODE " +
                       " AND B.GROUP_NEXT = '" + sMyGroup + "' " +
                       " AND A.GROUP_NAME = B.GROUP_NAME " +
                       " AND A.Carton_No <>'N/A' ";
                sSQL = sSQL + " AND (A.QA_NO='' OR A.QA_NO='N/A') ";
            }
            else if (itemPallet.IsChecked)
            {
                sSQL = " SELECT Pallet_No,QA_NO,QA_RESULT,COUNT(*) COUNT" +
                       " FROM SFISM4.R_WIP_TRACKING_T A,SFIS1.C_ROUTE_CONTROL_T B " +
                       " WHERE A.SPECIAL_ROUTE = B.ROUTE_CODE " +
                       " AND B.GROUP_NEXT = '" + sMyGroup + "' " +
                       " AND A.GROUP_NAME = B.GROUP_NAME " +
                       " AND A.Pallet_No <>'N/A' ";
                sSQL = sSQL + " AND (A.QA_NO='' OR A.QA_NO='N/A') ";
            }
            if (itemFullSampling.IsChecked)
            {
                sSQL = sSQL + " AND (Tray_NO NOT IN (Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag='Tray')) " +
                    " AND (Mcarton_NO NOT IN (Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag='Carton')) " +
                    " AND (IMEI NOT IN (Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag='Pallet')) ";
            }
            if (itemMO.IsChecked && !string.IsNullOrEmpty(G_sMO))
            {
                sSQL = sSQL + " and A.mo_number = '" + G_sMO + "' ";
            }
            else
            {
                if (itemPO.IsChecked)
                {
                    sSQL = sSQL + " and A.po_no = '" + sPO + "' and A.po_line='" + sPOLine + "' ";
                }
                else
                {
                    if (itemModel.IsChecked && !string.IsNullOrEmpty(G_sModelName))
                    {
                        sSQL = sSQL + " and A.model_name = '" + G_sModelName + "' ";
                    }
                }
            }
            if (itemLineSingle.IsChecked && listSelectLine.Items.Count == 1)
            {
                sSQL = sSQL + " and A.line_name='" + listSelectLine.Items[0].ToString() + "'";
            }
            else
            {
                if (itemLineSingle.IsChecked && listSelectLine.Items.Count > 1)
                {
                    sSQL = sSQL + " and (A.line_name='" + listSelectLine.Items[0].ToString() + "'";
                    for (int i = 1; i <= listSelectLine.Items.Count - 1; i++)
                    {
                        sSQL = sSQL + " or A.line_name='" + listSelectLine.Items[i].ToString() + "'";
                    }
                    sSQL = sSQL + " ) ";
                }
            }
            if (itemSSN.IsChecked)
            {
                if (itemSamplingSN.IsChecked)
                {
                    sSQL = sSQL + " group by Serial_Number, qa_no, qa_result " +
                     " order by Serial_Number, qa_result ";
                }
                else
                {
                    sSQL = sSQL + " group by Shipping_SN, qa_no, qa_result " +
                     " order by Shipping_SN, qa_result ";
                }
            }
            else if (itemTray.IsChecked)
            {
                sSQL = sSQL + " group by Tray_no, qa_no, qa_result " +
                      " order by Tray_no, qa_result ";
            }
            else if (itemCarton.IsChecked)
            {
                sSQL = sSQL + " group by Carton_no, qa_no, qa_result " +
                      " order by Carton_no, qa_result ";
            }
            else if (itemPallet.IsChecked)
            {
                sSQL = sSQL + " group by pallet_no, qa_no, qa_result " +
                      " order by pallet_no, qa_result ";
            }

            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            displayTitle(ini.Read("CQC", "CQC Unit"));
            if (quryTemp != null && quryTemp.Data.Count() > 0)
            {
                List<R107> leftlist = new List<R107>();
                leftlist = quryTemp.Data.ToListObject<R107>().ToList();
                clstrgridNotChecked.ItemsSource = leftlist;
                if (itemTray.IsChecked)
                    been_check1.Binding = new Binding("TRAY_NO");
                if (itemCarton.IsChecked)
                    been_check1.Binding = new Binding("CARTON_NO");
                if (itemPallet.IsChecked)
                    been_check1.Binding = new Binding("PALLET_NO");
            }
        }
        private async void itemCQCUnit_Click(object sender, RoutedEventArgs e)
        {
            CQCUnitWindow formCQCUnit = new CQCUnitWindow(this);
            if (string.IsNullOrEmpty(formCQCUnit.editCQCUnit.Text))
            {
                if (itemRight.IsChecked)
                    await DisplayRight();
                if (itemLeft.IsChecked)
                    await DisplayLeft();
            }
            else
                itemCQCUnit.IsEnabled = false;
        }

        private void itemUnitCount_Click(object sender, RoutedEventArgs e)
        {
            UnitCountWindow formUnitCount = new UnitCountWindow(lablUnitCount, editUnitCount);
        }



        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Login formLogin = new Login(this.sfcClient, chkbRejectReason, chkbInsertQCSN, chkbCheckRoute, chkbUpdateQANoResult, chkbRemoveFailSSN, chkbInsertSNtoPallet, chkbTransferbyPiece, chkbWarehouseNO, chkbPO, chkbClearPallet, chkbClearCarton, chkbSamplingPlan, editPalletFullFlag, editCompany);
            formLogin.Owner = this;
            formLogin.ShowDialog();
        }

        private void chkRelation1_Click(object sender, RoutedEventArgs e)
        {
            if (!Check_relation())
            {
                MessageBox.Show("Not check SN-Box-Tray-Carton yet!", "CQC");
                return;
            }
        }

        bool Check_relation()
        {
            pass_chkRelation = false;
            ChkRelationWindow formChkRelation = new ChkRelationWindow(this);
            formChkRelation.Owner = this;
            formChkRelation.ShowDialog();
            if (!pass_chkRelation)
            {
                return false;
            }
            return true;
        }

        //private DateTime G_dtTimeTemp;

        List<PARAMETER> stationlist = new List<PARAMETER>();

        private void itemStationName_Click(object sender, RoutedEventArgs e)
        {
            bLogin = true;
            ConfigDlgWindow formConfigDlg = new ConfigDlgWindow(this);
            formConfigDlg.Owner = this;
            formConfigDlg.ShowDialog();
        }


        private void itemCheckCarton_Click(object sender, RoutedEventArgs e)
        {
            itemCheckCarton.IsChecked = true;
            itemCheckPallet.IsChecked = false;
            itemCheckTray.IsChecked = false;
            itemCarton_Click(new object(), new RoutedEventArgs());
            editCQCUnit = "Carton";
            CQCUnitWindow formCQCUnit = new CQCUnitWindow(this);
            formCQCUnit.bbtnOK_Click(new Object(), new RoutedEventArgs());
            IniCQCBY();
        }
        void IniCQCBY()
        {
            if (itemSSN.IsChecked)
                ini.Write("CQC", "CQC BY", "SSN");
            if (itemTray.IsChecked)
                ini.Write("CQC", "CQC BY", "Tray");
            if (itemCarton.IsChecked)
                ini.Write("CQC", "CQC BY", "Carton");
            if (itemPallet.IsChecked)
                ini.Write("CQC", "CQC BY", "Pallet");

            //SAMPLING UNIT
            if (itemSamplingSN.IsChecked)
                ini.Write("CQC", "Sampling Unit", "SN");
            if (itemSamplingSSN.IsChecked)
                ini.Write("CQC", "Sampling Unit", "SSN");

            // MO/Model
            if (itemPO.IsChecked)
                ini.Write("CQC", "MO/Model", "PO");
            if (itemMO.IsChecked)
                ini.Write("CQC", "MO/Model", "MO");
            if (itemModel.IsChecked)
                ini.Write("CQC", "MO/Model", "Model");

            //Check Method
            if (itemCheckCarton.IsChecked)
                ini.Write("CQC", "Check Carton", "Y");
            else
                ini.Write("CQC", "Check Carton", "N");

            if (itemCheckPallet.IsChecked)
                ini.Write("CQC", "Check Pallet", "Y");
            else
                ini.Write("CQC", "Check Pallet", "N");

            if (itemCheckTray.IsChecked)
                ini.Write("CQC", "Check Tray", "Y");
            else
                ini.Write("CQC", "Check Tray", "N");


            //Full Sampling
            if (itemFullSampling.IsChecked)
                ini.Write("CQC", "Full Sampling", "Y");
            else
                ini.Write("CQC", "Full Sampling", "N");
        }

        private void itemCheckPallet_Click(object sender, RoutedEventArgs e)
        {
            itemCheckPallet.IsChecked = true;
            itemCheckCarton.IsChecked = false;
            itemCheckTray.IsChecked = false;
            itemPallet_Click(new object(), new RoutedEventArgs());
            editCQCUnit = "Pallet";
            CQCUnitWindow formCQCUnit = new CQCUnitWindow(this);
            formCQCUnit.bbtnOK_Click(new Object(), new RoutedEventArgs());
            IniCQCBY();
        }

        private void itemCheckTray_Click(object sender, RoutedEventArgs e)
        {
            itemCheckTray.IsChecked = true;
            itemCheckPallet.IsChecked = false;
            itemCheckCarton.IsChecked = false;
            itemTray_Click(new object(), new RoutedEventArgs());
            editCQCUnit = "Tray";
            CQCUnitWindow formCQCUnit = new CQCUnitWindow(this);
            formCQCUnit.bbtnOK_Click(new Object(), new RoutedEventArgs());
            IniCQCBY();
        }

        private void itemSSNLength_Click(object sender, RoutedEventArgs e)
        {
            SetUpLengthWindow formSetup = new SetUpLengthWindow(this);
            formSetup.Owner = this;
            formSetup.ShowDialog();
        }

        private void itemSamplingSN_Click(object sender, RoutedEventArgs e)
        {
            itemSamplingSN.IsChecked = !itemSamplingSN.IsChecked;
            if (itemSamplingSN.IsChecked == false)
            {
                itemSamplingSN.IsChecked = true;
                itemSamplingSSN.IsChecked = false;
                grpbInputEcSsn.Content = "Input EC / SN";
                grpbSSNchecked.Content = "Serial Number been checked";
                itemSSN.Header = "Serial Number";


                itemSampleUnit.Header = "Sampling Unit - SN";
                //   itemSampleUnit.Enabled:=false;
                //  lablInputMsg.Caption:='OK->SN  ,  NG->EC+SN / RC+LOC+SN';
                lablInputMsg.Text = "OK->SN,NG->EC+SN/RC+Duty+Loc+SN";
                itemSSNLength.Header = "SN &Length";
                if (string.IsNullOrEmpty(editCQCUnit))
                {
                    grpbPalletNoNotChecked.Content = " Serial Number not been checked ";
                    grpbPalletNoInLot.Content = " Serial Number in this lot ";
                    this_lot1.Header = " Serial Number ";
                    been_check1.Header = " Serial Number ";
                }
            }
            IniCQCBY();
        }

        private void itemSamplingSSN_Click(object sender, RoutedEventArgs e)
        {
            itemSamplingSSN.IsChecked = !itemSamplingSSN.IsChecked;
            if (itemSamplingSSN.IsChecked == false)
            {
                itemSamplingSSN.IsChecked = true;
                itemSamplingSN.IsChecked = false;
                grpbInputEcSsn.Content = "Input EC / S-SN";
                grpbSSNchecked.Content = "Shipping SN been checked";
                itemSSN.Header = "Shipping SN";


                itemSampleUnit.Header = "Sampling Unit - SSN";
                //   itemSampleUnit.Enabled:=false;
                //  lablInputMsg.Caption:='OK->SN  ,  NG->EC+SN / RC+LOC+SN';
                lablInputMsg.Text = "OK->SSN,NG->EC+SSN/RC+Duty+Loc+SSN";
                itemSSNLength.Header = "SN &Length";
                if (string.IsNullOrEmpty(editCQCUnit))
                {
                    grpbPalletNoNotChecked.Content = " Shipping SN not been checked ";
                    grpbPalletNoInLot.Content = " Shipping SN in this lot ";
                    this_lot1.Header = " Shipping SN ";
                    been_check1.Header = " Shipping SN ";
                }
            }
            IniCQCBY();
        }

        private void itemMO_Click(object sender, RoutedEventArgs e)
        {
            itemMO.IsChecked = !itemMO.IsChecked;
            if (itemMO.IsChecked == false)
            {
                itemMO.IsChecked = true;
                itemModel.IsChecked = false;
                itemPO.IsChecked = false;
                itemMOModel.Header = "*MO/Model";
            }
            else
                itemMO.IsChecked = false;
            IniCQCBY();
        }

        private void itemModel_Click(object sender, RoutedEventArgs e)
        {
            itemModel.IsChecked = !itemModel.IsChecked;
            if (itemModel.IsChecked == false)
            {
                itemMO.IsChecked = false;
                itemModel.IsChecked = true;
                itemPO.IsChecked = false;
                itemMOModel.Header = "*MO/Model";
            }
            else
                itemModel.IsChecked = false;
            IniCQCBY();
        }

        private void itemPO_Click(object sender, RoutedEventArgs e)
        {
            itemPO.IsChecked = !itemPO.IsChecked;
            if (itemPO.IsChecked == false)
            {
                itemMO.IsChecked = false;
                itemModel.IsChecked = false;
                itemPO.IsChecked = true;
                itemMOModel.Header = "*PO/*Model";
            }
            else
                itemPO.IsChecked = false;
            IniCQCBY();
        }

        private void itemEnabledLotNO_Click(object sender, RoutedEventArgs e)
        {
            itemEnabledLotNO.IsChecked = !itemEnabledLotNO.IsChecked;
            if (itemEnabledLotNO.IsChecked)
            {
                combLot.IsEnabled = true;
                ini.Write("CQC", "Enabled Lot No", "Y");
            }
            else
            {
                combLot.IsEnabled = false;
                ini.Write("CQC", "Enabled Lot No", "N");
            }
        }

        private async void itemChangeLine_Click(object sender, RoutedEventArgs e)
        {
            if (itemLineSingle.IsChecked)
            {
                iChangeLine = 1;
                SetupLineWindow formSetupLine = new SetupLineWindow(this);
                formSetupLine.Owner = this;
                formSetupLine.ShowDialog();
                PanelTitle.Content = "";
                for (int j = 0; j <= listSelectLine.Items.Count - 1; j++)
                {
                    if (j != listSelectLine.Items.Count - 1)
                        PanelTitle.Content += listSelectLine.Items[j].ToString() + ",";
                    else
                        PanelTitle.Content += listSelectLine.Items[j].ToString() + " ";
                }
                PanelTitle.Content += sMyGroup;
                await displayCombLot();
                iChangeLine = 0;
            }
        }

        private void AutoSampling_Click(object sender, RoutedEventArgs e)
        {
            if (AutoSampling.IsChecked)
                ini.Write("CQC", "Auto Sampling", "Y");
            else
                ini.Write("CQC", "Auto Sampling", "N");
        }

        private void ShowRejectLot_Click(object sender, RoutedEventArgs e)
        {
            if (ShowRejectLot.IsChecked)
                ini.Write("CQC", "Auto Sampling", "Y");
            else
                ini.Write("CQC", "Auto Sampling", "N");
        }

        private void ATETEST1_Click(object sender, RoutedEventArgs e)
        {
            if (ATETEST1.IsChecked)
            {
                ComWindow frmComWindow = new ComWindow(this);
                frmComWindow.Owner = this;
                frmComWindow.ShowDialog();
            }
            if (_serialPort.IsOpen)
            {
                _serialPort.DataReceived += _serialPort_DataReceived;
                //_serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            }
        }

        private async void N0401_Click(object sender, RoutedEventArgs e)
        {
            N0401.IsChecked = !N0401.IsChecked;
            if (N0401.IsChecked)
            {
                N0401.IsChecked = false;
                chkbCheckQty.IsEnabled = true;
                editCheckQty.IsEnabled = true;
                if (N0651.IsChecked == false)
                {
                    if (await ischeckfile())
                    {
                        N0651.IsChecked = true;
                        chkbCheckQty.IsEnabled = false;
                        editCheckQty.IsEnabled = false;
                    }
                    else
                    {
                        editCheckQty.IsEnabled = true;
                        editCheckQty.Text = "50";
                    }
                }
            }
            else
            {
                N0401.IsChecked = true;
                if (N0651.IsChecked)
                    N0651.IsChecked = false;

                chkbCheckQty.IsChecked = true;
                editCheckQty.IsEnabled = false;
                getLotAql();
            }
        }

        async void getLotAql()
        {
            if (string.IsNullOrEmpty(editCount.Content.ToString()))
                return;

            if (await ischeckfile())
            {
                if (N0401.IsChecked)
                {
                    if (int.Parse(editCount.Content.ToString()) >= 2 && int.Parse(editCount.Content.ToString()) <= 32)
                        editCheckQty.Text = editCount.Content.ToString();
                    else if (int.Parse(editCount.Content.ToString()) > 32 && int.Parse(editCount.Content.ToString()) <= 280)
                        editCheckQty.Text = "32";
                    else if (int.Parse(editCount.Content.ToString()) >= 281 && int.Parse(editCount.Content.ToString()) <= 500)
                        editCheckQty.Text = "48";
                    else if (int.Parse(editCount.Content.ToString()) >= 501 && int.Parse(editCount.Content.ToString()) <= 3200)
                        editCheckQty.Text = "73";
                    else if (int.Parse(editCount.Content.ToString()) >= 3201 && int.Parse(editCount.Content.ToString()) <= 10000)
                        editCheckQty.Text = "86";
                    else if (int.Parse(editCount.Content.ToString()) >= 10001 && int.Parse(editCount.Content.ToString()) <= 35000)
                        editCheckQty.Text = "108";
                    else if (int.Parse(editCount.Content.ToString()) >= 35001 && int.Parse(editCount.Content.ToString()) <= 15000)
                        editCheckQty.Text = "123";
                    else if (int.Parse(editCount.Content.ToString()) >= 150001 && int.Parse(editCount.Content.ToString()) <= 500000)
                        editCheckQty.Text = "156";
                    else if (int.Parse(editCount.Content.ToString()) >= 500001)
                        editCheckQty.Text = "189";
                }
                else if (N0651.IsChecked)
                {
                    if (int.Parse(editCount.Content.ToString()) >= 2 && int.Parse(editCount.Content.ToString()) <= 16)
                        editCheckQty.Text = editCount.Content.ToString();
                    else if (int.Parse(editCount.Content.ToString()) >= 17 && int.Parse(editCount.Content.ToString()) <= 280)
                        editCheckQty.Text = "20";
                    else if (int.Parse(editCount.Content.ToString()) >= 281 && int.Parse(editCount.Content.ToString()) <= 1200)
                        editCheckQty.Text = "47";
                    else if (int.Parse(editCount.Content.ToString()) >= 1201 && int.Parse(editCount.Content.ToString()) <= 3200)
                        editCheckQty.Text = "53";
                    else if (int.Parse(editCount.Content.ToString()) >= 3201 && int.Parse(editCount.Content.ToString()) <= 10000)
                        editCheckQty.Text = "68";
                    else if (int.Parse(editCount.Content.ToString()) >= 10001 && int.Parse(editCount.Content.ToString()) <= 35000)
                        editCheckQty.Text = "77";
                    else if (int.Parse(editCount.Content.ToString()) >= 35001 && int.Parse(editCount.Content.ToString()) <= 15000)
                        editCheckQty.Text = "96";
                    else if (int.Parse(editCount.Content.ToString()) >= 150001 && int.Parse(editCount.Content.ToString()) <= 500000)
                        editCheckQty.Text = "119";
                    else if (int.Parse(editCount.Content.ToString()) >= 500001)
                        editCheckQty.Text = "143";
                }
                else if (N1001.IsChecked)
                {
                    if (int.Parse(editCount.Content.ToString()) >= 2 && int.Parse(editCount.Content.ToString()) <= 16)
                        editCheckQty.Text = editCount.Content.ToString();
                    else if (int.Parse(editCount.Content.ToString()) >= 17 && int.Parse(editCount.Content.ToString()) <= 150)
                        editCheckQty.Text = "13";
                    else if (int.Parse(editCount.Content.ToString()) >= 151 && int.Parse(editCount.Content.ToString()) <= 280)
                        editCheckQty.Text = "20";
                    else if (int.Parse(editCount.Content.ToString()) >= 281 && int.Parse(editCount.Content.ToString()) <= 500)
                        editCheckQty.Text = "29";
                    else if (int.Parse(editCount.Content.ToString()) >= 501 && int.Parse(editCount.Content.ToString()) <= 1200)
                        editCheckQty.Text = "34";
                    else if (int.Parse(editCount.Content.ToString()) >= 1201 && int.Parse(editCount.Content.ToString()) <= 3200)
                        editCheckQty.Text = "42";
                    else if (int.Parse(editCount.Content.ToString()) >= 3201 && int.Parse(editCount.Content.ToString()) <= 10000)
                        editCheckQty.Text = "50";
                    else if (int.Parse(editCount.Content.ToString()) >= 10001 && int.Parse(editCount.Content.ToString()) <= 35000)
                        editCheckQty.Text = "60";
                    else if (int.Parse(editCount.Content.ToString()) >= 35001 && int.Parse(editCount.Content.ToString()) <= 150000)
                        editCheckQty.Text = "74";
                    else if (int.Parse(editCount.Content.ToString()) >= 150001 && int.Parse(editCount.Content.ToString()) <= 500000)
                        editCheckQty.Text = "90";
                    else if (int.Parse(editCount.Content.ToString()) >= 500001)
                        editCheckQty.Text = "102";
                }
            }
            else
                return;

        }

        private void itemTransfer_Click(object sender, RoutedEventArgs e)
        {
            TransferWindow formTransfer = new TransferWindow(sfcClient, chkbRemoveFailSSN, chkbInsertSNtoPallet, chkbTransferbyPiece, this);
            formTransfer.Owner = this;
            formTransfer.ShowDialog();
        }

        private void itemLeft_Click(object sender, RoutedEventArgs e)
        {
            ini.Write("CQC", "Show QC Right Data", "N");
        }

        private void itemRight_Click(object sender, RoutedEventArgs e)
        {

            if (itemRight.IsChecked)
                ini.Write("CQC", "Show QC Right Data", "Y");
            else
                ini.Write("CQC", "Show QC Right Data", "N");
        }

        private void btnRC_Click(object sender, RoutedEventArgs e)
        {
            DutyTypeWindow formDutyType = new DutyTypeWindow(ListBoxReasonCode, ListBoxDutyCode, ListBoxLocation);
            formDutyType.Owner = this;
            formDutyType.ShowDialog();
        }

        private void Runing1_Click(object sender, RoutedEventArgs e)
        {
            ModelFileWindow frmModelFile = new ModelFileWindow(sfcClient);
            frmModelFile.Owner = this;
            frmModelFile.ShowDialog();
        }

        private void OOBA1_Click(object sender, RoutedEventArgs e)
        {
            qryoobaWindow qryooba = new qryoobaWindow(sfcClient);
            qryooba.Owner = this;
            qryooba.ShowDialog();
        }

        private void CHECKBOXSSN1_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(combEmp.Text))
            {
                CheckboxssnWindow Fm_checkboxssn = new CheckboxssnWindow(this, sfcClient);
                Fm_checkboxssn.Owner = this;
                Fm_checkboxssn.ShowDialog();
            }
            else
                MessageBox.Show("NO EMP", "CQC");
        }

        private void Chip_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Login formLogin = new Login(sfcClient, chkbRejectReason, chkbInsertQCSN, chkbCheckRoute, chkbUpdateQANoResult, chkbRemoveFailSSN, chkbInsertSNtoPallet, chkbTransferbyPiece, chkbWarehouseNO, chkbPO, chkbClearPallet, chkbClearCarton, chkbSamplingPlan, editPalletFullFlag, editCompany);
            formLogin.Owner = this;
            formLogin.ShowDialog();
        }

        private async void N0651_Click(object sender, RoutedEventArgs e)
        {
            N0651.IsChecked = !N0651.IsChecked;
            if (N0651.IsChecked)
            {
                N0651.IsChecked = false;
                chkbCheckQty.IsEnabled = true;
                editCheckQty.IsEnabled = true;
                if (N0401.IsChecked == false)
                {
                    if (await ischeckfile())
                    {
                        N0401.IsChecked = true;
                        chkbCheckQty.IsEnabled = false;
                        editCheckQty.IsEnabled = false;
                    }
                    else
                    {
                        editCheckQty.IsEnabled = true;
                        editCheckQty.Text = "50";
                    }
                }
            }
            else
            {
                N0651.IsChecked = true;
                if (N0401.IsChecked)
                    N0401.IsChecked = false;

                chkbCheckQty.IsChecked = true;
                editCheckQty.IsEnabled = false;
                getLotAql();
            }
        }

        private void N1001_Click(object sender, RoutedEventArgs e)
        {
            N1001.IsChecked = !N1001.IsChecked;
            if (N1001.IsChecked)
            {
                N1001.IsChecked = false;
                chkbCheckQty.IsEnabled = true;
                editCheckQty.IsEnabled = true;
            }
            else
            {
                N1001.IsChecked = true;
                if (N0401.IsChecked)
                    N0401.IsChecked = false;
                else if (N0651.IsChecked)
                    N0651.IsChecked = false;

                chkbCheckQty.IsChecked = true;
                editCheckQty.IsEnabled = false;
                getLotAql();
            }
        }

        private void Ensky_Click(object sender, RoutedEventArgs e)
        {
            if (Ensky.IsChecked)
            {
                editMoNo.IsEnabled = true;
                editMoNo.Text = "";
                combDef.IsEnabled = false;
            }
            else
            {
                editMoNo.IsEnabled = false;
                editMoNo.Text = "";
                combDef.IsEnabled = true;
            }
        }

        private async void bbtnRight_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(combLot.Text))
            {
                MessageBox.Show("Please choose Lot NO !!", "CQC");
                return;
            }
            if (string.IsNullOrEmpty(combGroup))
            {
                string sSelData = sSelectPallet2;
                string sSQL = "select special_route from sfism4.r_wip_tracking_t ";
                if (itemSSN.IsChecked)
                {
                    if (itemSamplingSSN.IsChecked)
                        sSQL += " where shipping_sn = '" + sSelData + "' ";
                    else
                        sSQL += " where serial_number = '" + sSelData + "' ";
                }
                if (itemTray.IsChecked)
                    sSQL += " Where Tray_no = '" + sSelData + "' ";
                if (itemCarton.IsChecked)
                    sSQL += " where carton_no = '" + sSelData + "' ";

                if (itemPallet.IsChecked)
                    sSQL += " Where Pallet_no = '" + sSelData + "' ";
                sSQL += " and rownum=1 ";

                var quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSQL,
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryTemp.Data == null)
                    return;
                var quryRoute = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select step_sequence,group_Name from SFIS1.C_ROUTE_CONTROL_T where route_code ='" + quryTemp.Data["special_route"].ToString() + "' and group_next ='" + sMyGroup + "' order by step_sequence",
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryRoute.Data == null)
                    return;
                combGroup = quryRoute.Data["group_name"].ToString();

            }
            if (clstrgridNotChecked.Items.Count == 0)
            {
                lablMsg.Foreground = Brushes.Red;
                if (!string.IsNullOrEmpty(editCQCUnit))
                    lablMsg.Content = "Please select " + editCQCUnit;
                else
                {
                    if (itemSSN.IsChecked)
                        lablMsg.Content = "Please select Shipping SN";
                    else if (itemTray.IsChecked)
                        lablMsg.Content = "Please select Tray No";
                    else if (itemCarton.IsChecked)
                        lablMsg.Content = "Please select Carton No";
                    else
                        lablMsg.Content = "Please select Pallet No";
                }
            }
            else
            {
                if (clstrgridNotChecked.SelectedIndex == -1)
                    return;
                YesDlgWindow formYesDlg = new YesDlgWindow(this);
                formYesDlg.lablMesg.Content = "MOVE " + sSelectPallet + " UNIT?";
                formYesDlg.Owner = this;
                formYesDlg.ShowDialog();
                if (MrOK)
                {
                    if (clstrgridNotChecked.SelectedIndex < 0)
                        return;
                    R107 tmp = clstrgridNotChecked.Items[clstrgridNotChecked.SelectedIndex] as R107;
                    if (itemSSN.IsChecked)
                    {
                        if (itemSamplingSN.IsChecked)
                            sSelectPallet = tmp.SERIAL_NUMBER;
                        if (itemSSN.IsChecked)
                            sSelectPallet = tmp.SHIPPING_SN;
                    }
                    if (itemTray.IsChecked)
                        sSelectPallet = tmp.TRAY_NO;
                    if (itemCarton.IsChecked)
                        sSelectPallet = tmp.CARTON_NO;
                    if (itemPallet.IsChecked)
                        sSelectPallet = tmp.PALLET_NO;
                    lablMsg.Foreground = Brushes.Maroon;
                    clstrgridInLot.Items.Add(tmp);
                    List<R107> datalist = clstrgridNotChecked.ItemsSource as List<R107>;
                    datalist.RemoveAt(clstrgridNotChecked.SelectedIndex);
                    clstrgridNotChecked.ItemsSource = datalist;
                    clstrgridNotChecked.Items.Refresh();
                    lablMsg.Content = sSelectPallet + " has been moved.";
                    await updateR107(sSelectPallet);
                    if (itemRight.IsChecked)
                        await DisplayRight();
                    if (itemLeft.IsChecked)
                        await DisplayLeft();
                    if (chkbSamplingPlan)
                        await qurySamplingPlan();
                }
            }
            sSelectPallet = "";

        }

        private async void bbtnLeft_Click(object sender, RoutedEventArgs e)
        {
            int iFlag = 0;
            string sSQL = "";
            if (string.IsNullOrEmpty(sSelectPallet))
            {
                lablMsg.Foreground = Brushes.Red;
                if (editCQCUnit != "")
                    lablMsg.Content = "Please select " + editCQCUnit;
                else
                {
                    if (itemSSN.IsChecked)
                        lablMsg.Content = "Please select Shipping SN";
                    else if (itemTray.IsChecked)
                        lablMsg.Content = "Please select Tray No";
                    else if (itemCarton.IsChecked)
                        lablMsg.Content = "Please select Carton No";
                    else
                        lablMsg.Content = "Please select Pallet No";
                }
                iFlag = 1;
            }
            if (iFlag != 1)
            {
                if (itemSSN.IsChecked)
                    sSQL = " select * from SFISM4.R_WIP_TRACKING_T where  shipping_sn='" + sSelectPallet + "'" +
                          " and ((qa_result='1') or (group_name='" + sMyGroup + "')) ";
                if (itemTray.IsChecked)
                    sSQL = " select * from SFISM4.R_WIP_TRACKING_T where  Tray_no='" + sSelectPallet + "'" +
                          " and ((qa_result='1') or (group_name=:sMyGroup)) ";
                if (itemCarton.IsChecked)
                    sSQL = " select * from SFISM4.R_WIP_TRACKING_T where  Carton_no='" + sSelectPallet + "'" +
                          " and ((qa_result='1') or (group_name=:sMyGroup)) ";
                if ((!itemSSN.IsChecked && !itemTray.IsChecked && !itemCarton.IsChecked) || (itemPallet.IsChecked))
                    sSQL = " select * from SFISM4.R_WIP_TRACKING_T where  Pallet_no='" + sSelectPallet + "'" +
                          " and ((qa_result='1') or (group_name=:sMyGroup)) ";
                var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSQL,
                    SfcCommandType = SfcCommandType.Text
                });

                if (quryTemp.Data.Count() != 0)
                    lablMsg.Content = sSelectPallet + " illegal Move .";
                sSelectPallet = "a";
                YesDlgWindow formYesDlg = new YesDlgWindow(this);
                formYesDlg.Owner = this;
                formYesDlg.ShowDialog();
                if (MrOK)
                {
                    lablMsg.Foreground = Brushes.Maroon;
                    lablMsg.Content = sSelectPallet + " has been moved.";
                    await updateR107(sSelectPallet);
                    List<R107> datalist = clstrgridInLot.ItemsSource as List<R107>;
                    for (int i = 0; i < datalist.Count; i++)
                    {
                        if (itemSSN.IsChecked)
                        {
                            if (itemSSN.IsChecked)
                            {
                                if (datalist[i].SHIPPING_SN == sSelectPallet)
                                {
                                    datalist.RemoveAt(i);
                                    clstrgridNotChecked.Items.Add(datalist[i]);
                                }
                            }
                            else
                            {
                                if (datalist[i].SERIAL_NUMBER == sSelectPallet)
                                {
                                    datalist.RemoveAt(i);
                                    clstrgridNotChecked.Items.Add(datalist[i]);
                                }
                            }
                        }
                        if (itemTray.IsChecked)
                            if (datalist[i].TRAY_NO == sSelectPallet)
                            {
                                datalist.RemoveAt(i);
                                clstrgridNotChecked.Items.Add(datalist[i]);
                            }
                        if (itemCarton.IsChecked)
                            if (datalist[i].CARTON_NO == sSelectPallet)
                            {
                                datalist.RemoveAt(i);
                                clstrgridNotChecked.Items.Add(datalist[i]);
                            }
                        if (itemPallet.IsChecked)
                            if (datalist[i].PALLET_NO == sSelectPallet)
                            {
                                datalist.RemoveAt(i);
                                clstrgridNotChecked.Items.Add(datalist[i]);
                            }
                    }
                    clstrgridInLot.ItemsSource = datalist;
                    clstrgridInLot.Items.Refresh();
                    if (itemRight.IsChecked)
                        await DisplayRight();
                    if (itemLeft.IsChecked)
                        await DisplayLeft();
                    if (chkbSamplingPlan)
                        await qurySamplingPlan();
                }

            }
            sSelectPallet = "";
        }

        private void clstrgridNotChecked_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            InquireWindow formInquire = new InquireWindow(this, sfcClient, "3");
            formInquire.Owner = this;
            formInquire.ShowDialog();
        }

        private void clstrgridInLot_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            InquireWindow formInquire = new InquireWindow(this, sfcClient, "3");
            formInquire.Owner = this;
            formInquire.ShowDialog();
        }

        private void clstrgridInLot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clstrgridInLot.SelectedIndex > -1)
            {
                R107 tmp = clstrgridInLot.Items[clstrgridInLot.SelectedIndex] as R107;
                if (itemSSN.IsChecked)
                {
                    if (itemSamplingSN.IsChecked)
                        sSelectPallet = tmp.SERIAL_NUMBER;
                    if (itemSSN.IsChecked)
                        sSelectPallet = tmp.SHIPPING_SN;
                }
                if (itemTray.IsChecked)
                    sSelectPallet = tmp.TRAY_NO;
                if (itemCarton.IsChecked)
                    sSelectPallet = tmp.CARTON_NO;
                if (itemPallet.IsChecked)
                    sSelectPallet = tmp.PALLET_NO;
            }
        }

        private void clstrgridNotChecked_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clstrgridNotChecked.SelectedIndex > -1)
            {
                R107 tmp = clstrgridNotChecked.Items[clstrgridNotChecked.SelectedIndex] as R107;
                if (itemSSN.IsChecked)
                {
                    if (itemSamplingSN.IsChecked)
                        sSelectPallet2 = tmp.SERIAL_NUMBER;
                    if (itemSSN.IsChecked)
                        sSelectPallet2 = tmp.SHIPPING_SN;
                }
                if (itemTray.IsChecked)
                    sSelectPallet2 = tmp.TRAY_NO;
                if (itemCarton.IsChecked)
                    sSelectPallet2 = tmp.CARTON_NO;
                if (itemPallet.IsChecked)
                    sSelectPallet2 = tmp.PALLET_NO;
            }
        }

        string sprocCheckRoute;

        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            mainWindow.Title = "SHOP FLOOR INTEGRATED SYSTEM (SFIS) : FQA -- " + "Version: " + getRunningVersion().ToString();
        }

        //private void DataReceivedHandler( object sender,SerialDataReceivedEventArgs e)
        //{
        //    if (_serialPort.IsOpen)
        //    {
        //        string value = _serialPort.ReadLine();
        //        Check_str(value);
        //    }
        //}
        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                string value = _serialPort.ReadLine();
                if (value.Contains("\r"))
                {
                    value = value.Substring(0, value.IndexOf("\r"));
                }
                Check_str(value);
            }
        }

        async void Check_str(string TE_STR)
        {
            if (TE_STR.Length == 49)
            {
                string sn = TE_STR.Substring(0, 6);
                var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfism4.r_wip_tracking_t where serial_number ='" + sn + "' or shipping_sn ='" + sn + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data == null)
                {
                    MessageBox.Show("can not found this SN or Shipping_sn", "ERROR");
                    return;
                }
                else
                {
                    if (itemSamplingSSN.IsChecked)
                        sn = qury.Data["shipping_sn"].ToString();
                    else
                        sn = qury.Data["serial_number"].ToString();
                }
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    combDef.Text = sn;
                    ATE_FLAG = "OK";
                    combDef_KeyDown(new object(), new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return));
                    return;
                }));
            }
            if (TE_STR.Length == 53)
            {
                string sn = TE_STR.Substring(0, 6);
                ATE_FLAG = "";
                return;
            }
            if (TE_STR.Length == 56)
                return;
            Application.Current.Dispatcher.Invoke(new Action(() => { lablMsg.Content = "SFC:format error"; }));
            //MessageBox.Show("SFC:format error");
        }
        public static IPAddress GetIPAddress()
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses("");

            foreach (IPAddress hostAddress in hostAddresses)
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(hostAddress) &&  // ignore loopback addresses
                    !hostAddress.ToString().StartsWith("169.254."))  // ignore link-local addresses
                    return hostAddress;
            }
            return null; // or IPAddress.None if you prefer
        }
        public static string GetChecksum(HashingAlgoTypes hashingAlgoType, string filename)
        {
            using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString()))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }

        public enum HashingAlgoTypes
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }

        private void itemTray_Click(object sender, RoutedEventArgs e)
        {
            if (itemTray.IsChecked == false)
            {
                itemSSN.IsChecked = false;
                itemTray.IsChecked = true;
                itemCarton.IsChecked = false;
                itemPallet.IsChecked = false;
            }
            IniCQCBY();
        }

        private void itemCarton_Click(object sender, RoutedEventArgs e)
        {
            if (itemCarton.IsChecked == false)
            {
                itemSSN.IsChecked = false;
                itemTray.IsChecked = false;
                itemCarton.IsChecked = true;
                itemPallet.IsChecked = false;
            }
            IniCQCBY();
        }

        private void itemPallet_Click(object sender, RoutedEventArgs e)
        {
            if (itemPallet.IsChecked == false)
            {
                itemSSN.IsChecked = false;
                itemTray.IsChecked = false;
                itemCarton.IsChecked = false;
                itemPallet.IsChecked = true;
            }
            IniCQCBY();
        }

        private void itemSSN_Click(object sender, RoutedEventArgs e)
        {
            if (itemSSN.IsChecked == false)
            {
                itemSSN.IsChecked = true;
                itemTray.IsChecked = false;
                itemCarton.IsChecked = false;
                itemPallet.IsChecked = false;
            }
            IniCQCBY();
        }

        private void combDef_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            InquireWindow formInquire = new InquireWindow(this, sfcClient, "1");
            formInquire.Owner = this;
            formInquire.ShowDialog();
        }

        private void itemIsCheckSSN_Click(object sender, RoutedEventArgs e)
        {
            itemIsCheckSSN.IsEnabled = false;
            combDef.Focus();
        }

        public async Task<bool> CheckPrivilege(string Emp_NO, string Group)
        {
            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from sfis1.c_emp_2_group_t where emp_no = '" + empNo + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data.Count() > 0)
            {
                List<EMP_2_GROUP_T> emp2List = new List<EMP_2_GROUP_T>();
                emp2List = quryTemp.Data.ToListObject<EMP_2_GROUP_T>().ToList();
                if (emp2List.Exists(c => c.GROUP_NAME == "ALL"))
                    return true;
                else if (sMyGroup.Substring(0, 2) == "R_" && emp2List.Exists(c => c.GROUP_NAME == "REPAIR"))
                    return true;
                else if (emp2List.Count(c => c.GROUP_NAME == Group) > 0)
                    return true;
            }
            return false;

        }
        async Task<int> GetDbAndType()
        {
            combDef.Text = combDef.Text.ToUpper();
            var qrysetup = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from sfis1.c_parameter_ini where  prg_name='CQC' and  vr_class='shownotice' and vr_item='cqacpe2' and vr_name='cpe2' and vr_value='FQA' and rownum=1",
                SfcCommandType = SfcCommandType.Text
            });
            if (qrysetup.Data.Count() != 0) //cpeii
            {
                chknotooba.IsEnabled = true;
                chkairau.IsEnabled = true;
                chkairhk.IsEnabled = true;
                chkair.IsEnabled = true;
                chksea.IsEnabled = true;

                var qrycheckmodel = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfism4.r_custsn_t where SSN1='" + combDef.Text + "' OR SSN13='" + combDef.Text + "' OR MAC2='" + combDef.Text + "' ",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qrycheckmodel.Data != null)
                {
                    combDef.Text = qrycheckmodel.Data["ssn1"].ToString();
                }
                var querycount = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select  model_name from sfism4.r107 where shipping_sn='" + combDef.Text + "' and model_name like 'VMA%'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (querycount.Data.Count() == 0)//khong phai hang VMA tra 
                {
                    if (chknotooba.IsChecked == false)//neu khong check OBA tra ve 1
                    {
                        return 1;
                    }
                    else //chknobita ==true
                    {
                        return 2;
                    }
                }
                else //la hang VMA tra ve 3
                {
                    return 3;
                }
            }
            else //cpei tra ve 4
            {
                return 4;
            }
        }

        private void lablMsg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Meditinput_password.Visibility = Visibility.Visible;
            Meditinput_password.Focus();
        }

        private async void Meditinput_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (await Input_Password(Meditinput_password.Password))
                {
                    itemIsCheckSSN.IsEnabled = true;
                    MessageBox.Show(await GetPubMessage("00607"), "CQC");
                    Meditinput_password.Password = string.Empty;
                    Meditinput_password.Visibility = Visibility.Collapsed;
                    await displayCombLot();
                }
                else
                {
                    MessageBox.Show(await GetPubMessage("00608"), "CQC");
                    itemIsCheckSSN.IsEnabled = false;
                    Meditinput_password.Password = string.Empty;
                    Meditinput_password.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async void CombLot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combLot.SelectedIndex != -1)
            {
                combLot.Text = combLot.Items[combLot.SelectedIndex].ToString();
                await combLot_SelectionChanged1();
            }
        }

        private async void combDef_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                try
                {
                    if (await CheckRoku())
                    {
                        if (await getSNRoKu())
                        {
                            RoKuCheck = true;
                            foreach (R107 item in RoKuList)
                            {
                                try
                                {
                                    if (itemSamplingSN.IsChecked)
                                        combDef.Text = item.SERIAL_NUMBER;
                                    else
                                        combDef.Text = item.SHIPPING_SN;
                                    if (string.IsNullOrEmpty(combDef.Text) || combDef.Text.ToUpper() == "N/A")
                                    {
                                        MessageBox.Show("SN or SSN is null or N/A | Bản có dữ liệu SN hoặc SSN rỗng hoặc N/A ");
                                        break;
                                    }
                                    await InputSN();
                                    if (!RoKuCheck)
                                        break;
                                }
                                catch (Exception )
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Data not found!!!");
                            combDef.Focus();
                            return;
                        }
                    }
                    else
                    {
                        await InputSN();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }

        }

        async Task<bool> CheckRoku()
        {
            var quryRoKu = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from sfis1.c_parameter_ini where  prg_name='CQC' and  vr_class='shownotice' and vr_item='isroku' and vr_name='isroku' and vr_value='FQA' and rownum=1",
                SfcCommandType = SfcCommandType.Text
            });
            if (quryRoKu.Data.Count() > 0)
                return true;
            return false;
        }
        async Task<bool> getSNRoKu()
        {
            RoKuList = null;
            string[] argsIMEI = Regex.Split(combDef.Text, @":");
            string IMEI = argsIMEI[0].ToString();
            if (string.IsNullOrEmpty(IMEI))
                return false;
            else
                combDef.Text = IMEI;
            string strSQL = string.Empty;
            if (itemPallet.IsChecked)
                strSQL = "select a.* from sfism4.r107 a, sfism4.r117 b where a.imei = '" + combDef.Text + "'  and a.serial_number = b.serial_number and b.group_name = 'OBA2' and a.wip_group = 'FQA' and rownum <11";
            if (itemCarton.IsChecked)
                strSQL = "select a.* from sfism4.r107 a, sfism4.r117 b where a.mcarton_no = '" + combDef.Text + "'  and a.serial_number = b.serial_number and b.group_name = 'OBA2' and a.wip_group = 'FQA' and rownum <11";
            if (itemTray.IsChecked)
                strSQL = "select a.* from sfism4.r107 a, sfism4.r117 b where a.mcarton_no = '" + combDef.Text + "'  and a.serial_number = b.serial_number and b.group_name = 'OBA2' and a.wip_group = 'FQA' and rownum <11";

            var quryRoKu = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryRoKu.Data.Count() > 0)
            {
                RoKuList = quryRoKu.Data.ToListObject<R107>().ToList();
                return true;
            }
            return false;
        }
        private async Task InputSN()
        {
            int type;
            string sModel_name, sSN, C_RMA_PO, STR;
            bool Isbypallet = false;
            //THUY For CPEII ---check sn need pass station ooba1 or ooba2 , can test pass CQC
            if (itemSamplingSN.IsChecked == false && itemSamplingSSN.IsChecked == false)
            {
                MessageBox.Show("Please Choose Sampling Unit !!", "CQC");
                combDef.Text = "";
            }
            else
            {
                if (itemSSN.IsChecked == false && itemTray.IsChecked == false && itemCarton.IsChecked == false && itemPallet.IsChecked == false)
                {
                    MessageBox.Show("Please Choose CQC by Unit !!", "CQC");
                    combDef.Text = "";
                }
                else if (itemModel.IsChecked == false && itemMO.IsChecked == false && itemPO.IsChecked == false)
                {
                    MessageBox.Show("Please Choose MO / Model !!", "CQC");
                    combDef.Text = "";
                }
            }
            if (string.IsNullOrEmpty(editCheckQty.Text))
            {
                MessageBox.Show("Please Input Check Qty !!");
                editCheckQty.Focus();
                editCheckQty.Text = "";
            }
            combDef.Text = combDef.Text.ToUpper();
            combDef.Focus();

            type = await GetDbAndType();
            if (type == 1)
            {
                var qrycheckstation = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFISM4.R_EDI_AEDI03F where group_name in ('OOBA2','OOBA1','OOBA3') and Y3INVN ='" + combDef.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qrycheckstation.Data.Count() == 0)
                {
                    MessageBox.Show("Shipping sn: '" + combDef.Text + "' have not pass station OOBA1 and OOBA2.Pls check and input again", "CQC");
                    ATE_FLAG = "";
                    await getsnLotno(combDef.Text);
                    RoKuCheck = false;
                    return;
                }

            }
            if (type == 2)
            {
                Form3Window Form3 = new Form3Window(this);
                Form3.Owner = this;
                Form3.ShowDialog();
                if (!logic1)
                {
                    RoKuCheck = false;
                    return;
                }
            }
            if (combDef.Text == "UNDO")
            {
                combDef.Items.Clear();
                ListBoxLocation.Items.Clear();
                ListBoxReasonCode.Items.Clear();
                ListBoxDutyCode.Items.Clear();
                G_sType = "";
                G_sReasonCode = "";
                G_sDutycode = "";
                G_sLocation = "";
                editMoNo.Text = "";
                editModel.Text = "";
                lablMsg.Foreground = Brushes.Blue;
                G_iCritical = 0;
                G_iMajor = 0;
                G_iMinor = 0;
                if (itemSamplingSSN.IsChecked)
                    lablMsg.Content = "UNDO Ok,input Error code or Shipping SN.";
                else
                    lablMsg.Content = "UNDO Ok,input Error code or Serial Number.";
                if (Ensky.IsChecked)
                {
                    lablMsg.Content = "UNDO Ok,input Mo_Number.";
                    editMoNo.IsEnabled = true;
                    editMoNo.Text = "";
                    combDef.IsEnabled = false;
                }
                else
                {
                    editMoNo.IsEnabled = false;
                    combDef.IsEnabled = true;
                    editMoNo.Text = "";
                }
                return;
            }

            //kyo add
            //±NSN §ó·s¬°R107ªí¤¤°O¿ýªºSN
            if (Ensky.IsChecked)
            {
                combDef.Text = await CutSNforEnsky(combDef.Text.ToUpper());
                if (string.IsNullOrEmpty(combDef.Text))
                {
                    MessageWindow mes = new MessageWindow(this.sfcClient, "00585");
                    mes.Owner = this;
                    mes.ShowDialog();
                    RoKuCheck = false;
                    return;
                }
            }

            //end add
            // formCQC.tmpQry2

            var sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from sfism4.r_wip_tracking_t where imei='" + combDef.Text.ToString() + "'  and  WIP_GROUP='FQA'  AND  QA_RESULT='5'",
                SfcCommandType = SfcCommandType.Text
            });
            if (sql.Data.Count() > 0)
            {
                List<R107> snlist = new List<R107>();
                snlist = sql.Data.ToListObject<R107>().ToList();
                foreach (R107 item in snlist)
                {
                    combDef.Items.Add(item.SHIPPING_SN);
                }
                Isbypallet = true;
            }
            else
            {
                var tmpsql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfism4.r_wip_tracking_t where imei='" + combDef.Text.ToString() + "'  and  WIP_GROUP='FQA'  AND  QA_RESULT='4' ",
                    SfcCommandType = SfcCommandType.Text
                });
                if (tmpsql.Data.Count() > 0 && tmpsql.Data != null)
                {
                    lablMsg.Content = await GetPubMessage("00586");
                    RoKuCheck = false;
                    return;
                }
            }

            //    //----for tmm fqa ate test
            if (ATETEST1.IsChecked && ATE_FLAG != "OK")
            {
                sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfism4.r_wip_tracking_t where serial_number =:sn or shipping_sn =:sn",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name = "sn" , Value=combDef.Text.ToUpper() }
                    }
                });
                if (sql.Data.Count() > 0)
                {
                    List<R107> snlist = new List<R107>();
                    snlist = sql.Data.ToListObject<R107>().ToList();
                    sModel_name = snlist[0].MODEL_NAME;
                    sSN = snlist[0].SERIAL_NUMBER;
                }
                else
                {
                    lablMsg.Content = GetPubMessage("00587");
                    RoKuCheck = false;
                    return;
                }
                sModel_name = ateformat(sModel_name, 12);
                sSN = ateformat(sSN, 25);

                send_str(sSN + sModel_name + "PASS");
                return;
            }
            //--------------CHECK_SN_MOVE_T

            var tmpQry2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT MODEL_TYPE FROM SFIS1.C104 WHERE MODEL_NAME IN( select MODEL_NAME from sfism4.r_wip_tracking_t where serial_number ='" + combDef.Text + "' or shipping_sn ='" + combDef.Text + "')",
                SfcCommandType = SfcCommandType.Text
            });
            if (tmpQry2.Data.Count() > 0 && tmpQry2.Data != null)
            {
                List<C104> modeltypelist = new List<C104>();
                modeltypelist = tmpQry2.Data.ToListObject<C104>().ToList();
                s_ModelType = modeltypelist[0].MODEL_TYPE;
                if (string.IsNullOrEmpty(s_ModelType))
                {
                    MessageBox.Show("Call SFIS setup Model_type");
                    RoKuCheck = false;
                    return;
                }
            }
            if (s_ModelType.Contains("170"))
            {
                tmpQry2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "SELECT SERIAL_NUMBER FROM SFISM4.R107 where serial_number = '" + combDef.Text.ToUpper() + "' or shipping_sn ='" + combDef.Text.ToUpper() + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                List<R107> snlist = new List<R107>();
                snlist = tmpQry2.Data.ToListObject<R107>().ToList();
                STR = snlist[0].SERIAL_NUMBER;

                tmpQry2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "SELECT POSTITION_FLAG  serial_number from  SFISM4.R_SN_MOVE_T WHERE SERIAL_NUMBER= '" + STR + "' ",
                    SfcCommandType = SfcCommandType.Text
                });

                if (tmpQry2.Data.Count() <= 0)
                {
                    lablMsg.Foreground = Brushes.Green;
                    lablMsg.Content = combDef.Text + " no weight";
                    combDef.Focus();
                    RoKuCheck = false;
                    return;
                }
                else
                {
                    List<R107> chklist = new List<R107>();
                    chklist = tmpQry2.Data.ToListObject<R107>().ToList();
                    if (chklist[0].SERIAL_NUMBER == "0")
                    {
                        lablMsg.Foreground = Brushes.Green;
                        lablMsg.Content = combDef.Text + " no weight";
                        combDef.Focus();
                        RoKuCheck = false;
                        return;
                    }
                }
            }

            if (G_sType == "")
            {
                List<ERROR> errorlist = new List<ERROR>();
                List<REASON> reasonlist = new List<REASON>();
                var quryDef = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfis1.c_error_code_t where error_code = '" + combDef.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryDef.Data.Count() > 0)
                {
                    errorlist = quryDef.Data.ToListObject<ERROR>().ToList();
                    lablMsg.Foreground = Brushes.Green;
                    lablMsg.Content = errorlist[0].ERROR_DESC;
                    combDef.Items.Add(combDef.Text);
                    G_sType = "EC";
                    combDef.Focus();
                }
                else
                {
                    quryDef = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "select * from SFIS1.C_REASON_CODE_T where reason_code = '" + combDef.Text + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (quryDef.Data.Count() > 0)
                    {
                        lablMsg.Foreground = Brushes.Green;
                        reasonlist = quryDef.Data.ToListObject<REASON>().ToList();
                        lablMsg.Content = reasonlist[0].REASON_DESC;
                        G_sReasonCode = combDef.Text;
                        G_sType = "RC";
                        combDef.Focus();
                    }
                    else
                    {
                        //ÀË¬d¬O§_¬°sn
                        po_flag = false;
                        //input macid
                        if (itemInputMac.IsChecked && await FindSerialNO(combDef.Text))
                        {
                            combDef.Text = sSerialNO;
                        }
                        var querydef = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "select * from sfism4.r_wip_tracking_t where serial_number =:sn or shipping_sn =:sn",
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="sn", Value=combDef.Text }
                                }
                        });
                        if (querydef.Data.Count() > 0)
                        {
                            List<R107> modellist = new List<R107>();
                            modellist = querydef.Data.ToListObject<R107>().ToList();
                            str_model_name = modellist[0].MODEL_NAME;
                            C_pallet_no = modellist[0].PALLET_NO;
                            string ssql = "select vr_value from SFIS1.C_PARAMETER_INI where VR_item ='" + str_model_name + "' and prg_name ='CQC' AND  vr_class ='PO CONTROL' and vr_name ='RMA PO' ";
                            var qryRMAPO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = ssql,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qryRMAPO.Data.Count() > 0)
                            {
                                List<PARAMETER> palist = new List<PARAMETER>();
                                palist = qryRMAPO.Data.ToListObject<PARAMETER>().ToList();
                                C_RMA_PO = palist[0].VR_VALUE;

                                var qryRMAPO2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = "select * from sfism4.r_po_sn_detail_t where serial_number ='" + combDef.Text.ToUpper() + "'  and PO_NO ='" + C_RMA_PO + "'",
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qryRMAPO2.Data.Count() > 0)
                                {
                                    RMAPOWindow RMAPO = new RMAPOWindow(this);
                                    RMAPO.Owner = this;
                                    RMAPO.ShowDialog();
                                    if (po_flag != true)
                                    {
                                        po_flag = false;
                                        return;
                                    }
                                }
                            }

                            var quryParameter = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "SELECT PRG_NAME FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME ='TA_VERSION' AND VR_NAME IN(SELECT MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME = '" + str_model_name + "')",
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (quryParameter.Data.Count() == 0)
                                chkTA.IsChecked = false;
                            if (chkTA.IsChecked == true)
                            {
                                if (!await CheckTAVer(C_pallet_no))
                                {
                                    MessageBox.Show("PLEASE CALL PQE CONFIRM!");
                                    RoKuCheck = false;
                                    return;
                                }
                            }

                            if (type == 2 && logic1)//truong hop tra ve 2 se check logic1
                            {
                                await checkSerN();
                            }
                            else
                                await checkSerN();
                            if (Isbypallet)
                            {
                                if (tmpQry2.Data.Count() > 0)
                                {
                                    List<R107> list1 = new List<R107>();
                                    list1 = tmpQry2.Data.ToListObject<R107>().ToList();
                                    combDef.Text = list1[0].SHIPPING_SN;
                                    await checkSerN();
                                }
                            }
                            combDef.Focus();
                        }
                        else
                        {
                            if (SNEdit != "" && combDef.Text.Length != Int32.Parse(SNEdit))
                            {
                                MessageBox.Show("Length Error !!", "CQC");
                                RoKuCheck = false;
                                return;
                            }
                        }

                    }
                }
                if (editLimitQty.Content != null && editCount.Content != null && editCheckQty.Text != null)
                {
                    editLimitQty.Content = (Int32.Parse(editCount.Content.ToString()) / Int32.Parse(editCheckQty.Text)).ToString();

                    if (Int32.Parse(editCount.Content.ToString()) % Int32.Parse(editCheckQty.Text) != 0)
                    {
                        editLimitQty.Content = (Int32.Parse(editLimitQty.Content.ToString()) + 1).ToString();
                    }
                    if (Int32.Parse(editCheckedQty.Content.ToString()) >= Int32.Parse(editLimitQty.Content.ToString()) && editLimitQty.Content.ToString() != "0")
                    {
                        if (itemIsCheckSSN.IsChecked == false)
                        {
                            cmdCheck_Click(new object(), new RoutedEventArgs());
                        }
                    }
                }
                return;
            }
            if (G_sType == "EC")
            {
                List<ERROR> errorlist = new List<ERROR>();
                if (combDef.Items.IndexOf(combDef.Text) != -1)
                {
                    lablMsg.Foreground = Brushes.Red;
                    lablMsg.Content = "Duplicate Error code !";
                    RoKuCheck = false;
                    combDef.Focus();
                    return;
                }
                var quryErrorcode = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfis1.c_error_code_t where error_code ='" + combDef.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryErrorcode.Data.Count() > 0)
                {
                    lablMsg.Foreground = Brushes.Green;
                    errorlist = quryErrorcode.Data.ToListObject<ERROR>().ToList();
                    lablMsg.Content = errorlist[0].ERROR_DESC;
                    combDef.Items.Add(combDef.Text);
                }
                else
                {
                    if (SNEdit != "" && combDef.Text.Length != Int32.Parse(SNEdit))
                    {
                        MessageBox.Show("Length Error !!");
                        RoKuCheck = false;
                        return;
                    }
                    await checkSerN();
                }
            }
            if (G_sType == "RC")
            {
                List<REASON> reasonlist = new List<REASON>();
                List<DUTY> dutylist = new List<DUTY>();
                var quryErrorcode = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfis1.c_error_code_t where error_code ='" + combDef.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryErrorcode.Data.Count() > 0)
                {
                    MessageBox.Show("Please input Duty");
                    lablMsg.Foreground = Brushes.Red;
                    lablMsg.Content = "Please input Duty!";
                    combDef.Focus();
                    RoKuCheck = false;
                    return;
                }
                var quryreasoncode = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfis1.C_REASON_CODE_T where reason_code ='" + combDef.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryreasoncode.Data.Count() > 0)
                {
                    if (G_sDutycode == "" && G_sLocation == "")
                    {
                        MessageBox.Show("Please input Duty");
                        lablMsg.Foreground = Brushes.Red;
                        lablMsg.Content = "Please input Duty!";
                        combDef.Focus();
                        RoKuCheck = false;
                        return;
                    }
                    else
                    {
                        lablMsg.Foreground = Brushes.Green;
                        reasonlist = quryreasoncode.Data.ToListObject<REASON>().ToList();
                        lablMsg.Content = reasonlist[0].REASON_DESC;
                        G_sReasonCode = combDef.Text;
                        G_sDutycode = "";
                        G_sLocation = "";
                        combDef.Focus();
                        RoKuCheck = false;
                        return;
                    }
                }
                var quryduty = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFIS1.C_DUTY_T where DUTY_TYPE ='" + combDef.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryduty.Data.Count() > 0)
                {
                    if (G_sDutycode == "" && G_sLocation == "")
                    {
                        lablMsg.Foreground = Brushes.Green;
                        dutylist = quryduty.Data.ToListObject<DUTY>().ToList();
                        lablMsg.Content = dutylist[0].DUTY_DESC;
                        G_sDutycode = combDef.Text;
                        combDef.Focus();
                        RoKuCheck = false;
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Please input Location");
                        lablMsg.Foreground = Brushes.Red;
                        lablMsg.Content = "Please input Location!";
                        combDef.Focus();
                        RoKuCheck = false;
                        return;
                    }
                }
                else
                {
                    if (G_sDutycode == "" && G_sLocation == "")
                    {
                        MessageBox.Show("Duty Code Error!");
                        lablMsg.Foreground = Brushes.Red;
                        lablMsg.Content = "Please input Duty!";
                        combDef.Focus();
                        RoKuCheck = false;
                        return;
                    }
                }

                //input macid
                if (itemInputMac.IsChecked && await FindSerialNO(combDef.Text))
                {
                    combDef.Text = sSerialNO;
                }
                var querydef = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfism4.r_wip_tracking_t where serial_number =:sn or shipping_sn =:sn",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                                                {
                                                    new SfcParameter {Name="sn", Value=combDef.Text }
                                                }
                });
                if (querydef.Data.Count() > 0)
                {
                    if (G_sLocation == "")
                    {
                        MessageBox.Show("Please input Location");
                        lablMsg.Foreground = Brushes.Red;
                        lablMsg.Content = "Please input Location!";
                        combDef.Focus();
                        RoKuCheck = false;
                        return;
                    }
                    else
                    {
                        await checkSerN();
                        combDef.Focus();
                        ListBoxReasonCode.Items.Clear();
                        ListBoxDutyCode.Items.Clear();
                        ListBoxLocation.Items.Clear();
                        return;
                    }
                }
                ListBoxReasonCode.Items.Add(G_sReasonCode);
                ListBoxDutyCode.Items.Add(G_sDutycode);
                ListBoxLocation.Items.Add(combDef.Text);
                G_sLocation = combDef.Text;
                combDef.Focus();
            }
            editLimitQty.Content = (Int32.Parse(editCount.Content.ToString()) / Int32.Parse(editCheckQty.Text)).ToString();
            if (Int32.Parse(editCount.Content.ToString()) % Int32.Parse(editCheckQty.Text) != 0)
            {
                editLimitQty.Content = (Int32.Parse(editLimitQty.Content.ToString()) + 1).ToString();
            }
            combDef.Focus();
            if (Int32.Parse(editCheckedQty.Content.ToString()) >= Int32.Parse(editLimitQty.Content.ToString()) && editLimitQty.Content.ToString() != "0")
            {
                if (itemIsCheckSSN.IsChecked == false)
                    cmdCheck_Click(new object(), new RoutedEventArgs());
            }
            ATE_FLAG = "";
            await getsnLotno(combDef.Text);
        }

        public async Task<bool> checkQA(string QA)
        {

            try
            {
                ColorStringGrid1.ItemsSource = null;
                if (!await isOOBAControlNeed())
                    return true;
                var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select model_name from SFISM4.R_WIP_TRACKING_T WHERE QA_NO='" + QA + "' and rownum=1",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data == null)
                    return true;
                if (!await isNetGear(qury.Data["model_name"].ToString()))
                    return true;
                string mysql = "SELECT MO_NUMBER,model_name,version_code,imei,count(*) count FROM SFISM4.R_WIP_TRACKING_T WHERE QA_NO='" + QA + "' group by MO_NUMBER,model_name,version_code,imei  order by mo_number";
                var Query2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = mysql,
                    SfcCommandType = SfcCommandType.Text
                });
                List<R107> qurylist = new List<R107>();

                qurylist = Query2.Data.ToListObject<R107>().ToList();
                int mo_needQty;
                int mo_qty;
                int packqty;
                double qtypallet;
                int mypackqty;
                double pack_needqty;
                double c;
                string stt = string.Empty;
                List<OOBA> datalist = new List<OOBA>();
                for (int i = 0; i < qurylist.Count; i++)
                {
                    mypackqty = int.Parse(qurylist[i].COUNT);
                    mo_qty = await getMOQTY(qurylist[i].MO_NUMBER, 0);
                    packqty = await getPackQTY(qurylist[i].MODEL_NAME, qurylist[i].VERSION_CODE, 0);
                    mo_needQty = getOOBASampleSize(mo_qty, 0);
                    qtypallet = Math.Round((double)(mo_qty / mypackqty));
                    pack_needqty = Math.Round((double)(mo_needQty / qtypallet));
                    c = await getSampleQty(QA, qurylist[i].IMEI, 0);

                    if  (mo_qty < pack_needqty)
                    {
                        pack_needqty = mo_qty;
                    }
                    else if (mypackqty < pack_needqty)
                    {
                        pack_needqty = mypackqty;
                    }

                    if ((pack_needqty - c) > 0)
                    {
                        stt = "*";
                        datalist.Add(new OOBA()
                        {
                            STT = stt,
                            MO_NUMBER = qurylist[i].MO_NUMBER,
                            MODEL_NAME = qurylist[i].MODEL_NAME,
                            VERSION_CODE = qurylist[i].VERSION_CODE,
                            PA_NO = qurylist[i].IMEI,
                            QTY = mypackqty.ToString(),
                            MO_QTY = mo_qty.ToString(),
                            AQL = "0.65",
                            PACK_PARAM = packqty.ToString(),
                            MO_NEED = mo_needQty.ToString(),
                            PA_NEED = pack_needqty.ToString(),
                            PA_HAVE = c.ToString(),
                            DIF = (pack_needqty - (c)).ToString()
                        });
                        ColorStringGrid1.ItemsSource = datalist;
                        return false;
                    }
                    else
                    {
                        stt = "";
                    }
                    datalist.Add(new OOBA()
                    {
                        STT = stt,
                        MO_NUMBER = qurylist[i].MO_NUMBER,
                        MODEL_NAME = qurylist[i].MODEL_NAME,
                        VERSION_CODE = qurylist[i].VERSION_CODE,
                        PA_NO = qurylist[i].IMEI,
                        QTY = mypackqty.ToString(),
                        MO_QTY = mo_qty.ToString(),
                        AQL = "0.65",
                        PACK_PARAM = packqty.ToString(),
                        MO_NEED = mo_needQty.ToString(),
                        PA_NEED = pack_needqty.ToString(),
                        PA_HAVE = c.ToString(),
                        DIF = (pack_needqty - (c)).ToString()
                    });
                }
                ColorStringGrid1.ItemsSource = datalist;
                return true;
            }
            catch
            {
                return false;
            }
        }

        async Task<int> getSampleQty(string qa, string imei, int result)
        {
            string mysql = "select count(1) bb from sfism4.r_qc_sn_t a where lot_no ='" + qa + "' and exists" +
                          " (select 1 from sfism4.r107 where serial_number=a.serial_number and imei='" + imei + "')";
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = mysql,
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data == null)
                result = 0;
            else
                result = int.Parse(qury.Data["bb"].ToString());
            return result;
        }
        int getOOBASampleSize(int qty, int result)
        {
            if (1 <= qty && qty <= 15)
                result = qty;
            else if (16 <= qty && qty <= 280)
                result = 20;
            else if (281 <= qty && qty <= 1200)
                result = 47;
            else if (1201 <= qty && qty <= 3200)
                result = 53;
            else if (qty % 3000 == 0)
            {
                result = (qty / 3000) * 53;
            }
            else
            {
                int result1 = qty - ((qty / 3200) * 3200);
                if (1 <= result1 && result1 <= 15)
                    result = result1 + ((qty / 3200) * 53);
                else if (16 <= result1 && result1 <= 280)
                    result = 20 + ((qty / 3200) * 53);
                else if (281 <= result1 && result1 <= 1200)
                    result = 47 + ((qty / 3200) * 53);
                else if (1201 <= result1 && result1 <= 3200)
                    result = 53 + ((qty / 3200) * 53);
            }
            return result;
        }
        async Task<int> getPackQTY(string model, string ver, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select pallet_qty from sfis1.c_pack_param_t where model_name='" + model + "' and version_code='" + ver + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data == null)
                result = 0;
            else
                result = int.Parse(qury.Data["pallet_qty"].ToString());
            return result;
        }

        async Task<int> getMOQTY(string mo, int result)
        {
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT target_qty from sfism4.r105 where mo_number='" + mo + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data == null)
                result = 0;
            else
                result = int.Parse(qury.Data["target_qty"].ToString());
            return result;
        }
        public async Task<bool> isNetGear(string model)
        {
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select 1 from sfis1.c_model_desc_t where model_name='" + model + "' and  model_type like '%201%'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data.Count() > 0)
                return true;
            return false;
        }

        private void ColorStringGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            List<OOBA> datalist = ColorStringGrid1.ItemsSource as List<OOBA>;
            oobafrmWindow oobafrm = new oobafrmWindow(combLot.Text, datalist);
            oobafrm.Owner = this;
            oobafrm.ShowDialog();
        }

        public async Task<bool> isOOBAControlNeed()
        {
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select vr_value from sfis1.c_parameter_ini where prg_name='CQC' and vr_name='OOBA'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data.Count() > 0)
            {
                return true;
            }
            return false;
        }
        async Task getsnLotno(string _SN)
        {
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT LOT_NO FROM SFISM4.R_QC_SN_T WHERE SERIAL_NUMBER='" + _SN + "' ",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data == null)
                return;
            if (qury.Data.Count() > 0)
            {
                List<R_QC_SN_T> lotnolist = new List<R_QC_SN_T>();
                lotnolist = qury.Data.ToListObject<R_QC_SN_T>().ToList();
                combLot.Text = lotnolist[0].LOT_NO;
            }
        }
        async Task<bool> CheckTAVer(string sPallet)
        {
            var quryTA = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT DISTINCT TRACK_NO FROM SFISM4.R107 WHERE PALLET_NO = '" + sPallet + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTA.Data.Count() != 1)
            {
                MessageBox.Show("1 Pallet have 2 'TA ver' or 'TA ver' is null");
                inputpasswordWindow forminputpassword = new inputpasswordWindow(this);
                forminputpassword.Owner = this;
                forminputpassword.ShowDialog();
                if (isPassword)
                {
                    chkTA.IsChecked = false;
                    return true;
                }
                else
                {
                    chkTA.IsChecked = true;
                    return false;
                }
            }
            else
            {
                List<R107> tracknolist = new List<R107>();
                tracknolist = quryTA.Data.ToListObject<R107>().ToList();
                string NewTA = tracknolist[0].TRACK_NO;
                string sql = " SELECT MAX(TRACK_NO) TRACK_NO FROM SFISM4.Z107 WHERE WIP_GROUP = 'FG' AND MODEL_NAME IN " +
                             " (SELECT MODEL_NAME FROM SFISM4.R107 WHERE PALLET_NO = '" + sPallet + "' AND ROWNUM = 1)";
                quryTA = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                tracknolist = quryTA.Data.ToListObject<R107>().ToList();
                string OldTA = tracknolist[0].TRACK_NO;
                if (string.IsNullOrEmpty(OldTA))
                    return true;
                if (int.Parse(NewTA) < int.Parse(OldTA))
                {
                    isPassword = false;
                    if (chkTA.IsChecked == true)
                    {
                        inputpasswordWindow forminputpassword = new inputpasswordWindow(this);
                        forminputpassword.Owner = this;
                        forminputpassword.ShowDialog();
                        if (isPassword)
                        {
                            chkTA.IsChecked = false;
                            return true;
                        }
                        else
                        {
                            chkTA.IsChecked = true;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        async Task checkSerN()
        {
            try
            {
                int iI;
                int iFlag = 0;
                int iFlag2 = 0;
                int iError_F = 0;
                string sSQL;
                string sCarton = "";
                if (CheckSSN1.IsChecked)
                {
                    sSQL = " select * from sfism4.r_wip_tracking_t ";
                    if (itemSamplingSSN.IsChecked)
                        sSQL += " where shipping_sn = '" + combDef.Text + "' ";
                    else
                        sSQL += " where serial_number = '" + combDef.Text + "' ";
                    sSQL += " and rownum=1 ";

                    var query107 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query107.Data.Count() > 0)
                    {
                        qurySerN = query107.Data.ToListObject<R107>().ToList();
                        if (itemCheckTray.IsChecked)
                            sCarton = qurySerN[0].TRAY_NO;
                        if (itemCheckCarton.IsChecked)
                            sCarton = qurySerN[0].CARTON_NO;
                        if (itemCheckPallet.IsChecked)
                            sCarton = qurySerN[0].PALLET_NO;
                    }

                    sSQL = " select * from sfism4.r_wip_tracking_t ";
                    if (itemCheckTray.IsChecked)
                        sSQL += "where Tray_NO = '" + sCarton + "' and ((SHIPPING_SN = 'N/A') or (SHIPPING_SN IS NULL))";
                    if (itemCheckCarton.IsChecked)
                        sSQL += "where Carton_No = '" + sCarton + "' and ((SHIPPING_SN = 'N/A') or (SHIPPING_SN IS NULL))";
                    if (itemCheckPallet.IsChecked)
                        sSQL += "where Pallet_No = '" + sCarton + "' and ((SHIPPING_SN = 'N/A') or (SHIPPING_SN IS NULL))";

                    sSQL += " and rownum=1 ";

                    query107 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (query107.Data.Count() == 0)
                    {
                        MessageBox.Show("SHIPPING_SN ERROR.");
                        RoKuCheck = false;
                        return;
                    }
                }
                sSQL = " select * from sfism4.r_wip_tracking_t ";
                if (itemSamplingSSN.IsChecked)
                    sSQL += " where shipping_sn = '" + combDef.Text + "' ";
                else
                    sSQL += " where serial_number = '" + combDef.Text + "' ";
                sSQL += " and rownum=1 ";
                var sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSQL,
                    SfcCommandType = SfcCommandType.Text
                });


                if (sql.Data.Count() > 0)
                {
                    qurySerN = sql.Data.ToListObject<R107>().ToList();
                    if (string.IsNullOrEmpty(combGroup))
                    {
                        var queryRoute = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "select step_sequence,group_Name from SFIS1.C_ROUTE_CONTROL_T where route_code=:route_code and group_next=:group_name order by step_sequence",
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name="route_code", Value=qurySerN[0].SPECIAL_ROUTE },
                            new SfcParameter {Name="group_name", Value=sMyGroup }
                        }
                        });
                        if (queryRoute.Data != null)
                            combGroup = queryRoute.Data["group_name"].ToString();
                    }

                    //-------------------ADD BY YHQ 06.03.10----------------------------
                    //Tray,Carton,Pallet
                    if (combGroup == "PACK_TRAY")
                    {
                        if (qurySerN[0].TRAY_NO != "N/A")
                        {
                            var query1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "Select * From SFIS1.C_Pallet_T Where Close_Flag= 'Tray' AND Pallet_No='" + qurySerN[0].TRAY_NO + "'",
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (query1.Data.Count() != 0)
                            {
                                lablMsg.Foreground = Brushes.Red;
                                lablMsg.Content = "Tray NO " + qurySerN[0].TRAY_NO + "not closed yet!!";
                                combDef.Focus();
                                iFlag = 1;
                                RoKuCheck = false;
                                return;
                            }
                        }
                        else
                        {
                            lablMsg.Foreground = Brushes.Red;
                            lablMsg.Content = "No tray no";
                            combDef.Focus();
                            iFlag = 1;
                            RoKuCheck = false;
                            return;
                        }
                    }
                    else if (combGroup == "PACK_CTN")
                    {
                        if (qurySerN[0].MCARTON_NO != "N/A")
                        {
                            var query1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "Select * From SFIS1.C_Pallet_T Where Close_Flag= 'Carton' AND Pallet_No='" + qurySerN[0].MCARTON_NO + "'",
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (query1.Data.Count() != 0)
                            {
                                lablMsg.Foreground = Brushes.Red;
                                lablMsg.Content = "Carton NO " + qurySerN[0].MCARTON_NO + "not closed yet!!";
                                combDef.Focus();
                                iFlag = 1;
                                RoKuCheck = false;
                                return;
                            }
                        }
                        else
                        {
                            lablMsg.Foreground = Brushes.Red;
                            lablMsg.Content = "No Carton NO!!";
                            combDef.Focus();
                            iFlag = 1;
                            RoKuCheck = false;
                            return;
                        }
                    }
                    else if (combGroup == "PACK_PALT")
                    {
                        if (qurySerN[0].IMEI != "N/A")
                        {
                            var query1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "Select * From SFIS1.C_Pallet_T Where Close_Flag= 'Pallet' AND Pallet_No='" + qurySerN[0].IMEI + "'",
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (query1.Data.Count() != 0)
                            {
                                lablMsg.Foreground = Brushes.Red;
                                lablMsg.Content = "PALLET NO " + qurySerN[0].IMEI + "not closed yet!!";
                                combDef.Focus();
                                iFlag = 1;
                                RoKuCheck = false;
                                return;
                            }
                        }
                        else
                        {
                            lablMsg.Foreground = Brushes.Red;
                            lablMsg.Content = "No PALLET NO!!";
                            combDef.Focus();
                            iFlag = 1;
                            RoKuCheck = false;
                            return;
                        }
                    }

                    //-------------------ADD BY YHQ 06.03.10----------------------------
                    sMyLine = qurySerN[0].LINE_NAME;
                    if (comb1.Items.IndexOf(combDef.Text) != -1)
                    {
                        if (qurySerN[0].NEXT_STATION == sMyGroup)
                        {
                            if (itemSamplingSSN.IsChecked)
                                MessageBox.Show("Duplicate Shipping S/N.");
                            else
                                MessageBox.Show("Duplicate Serial Number.");
                        }
                        else
                            MessageBox.Show("Duplicate Sampling !!");
                        RoKuCheck = false;
                        return;
                    }

                    //SHOW INFORMATION
                    editModel.Text = qurySerN[0].MODEL_NAME;
                    editMoNo.Text = qurySerN[0].MO_NUMBER;


                    //add by syant 20130911
                    if (await isOOBAControlNeed())
                    {
                        if (await isNetGear(editModel.Text))
                        {
                            if (!string.IsNullOrEmpty(combLot.Text))
                            {
                                panelOOBA.Visibility = Visibility.Visible;
                                await checkQA(combLot.Text);
                            }
                        }
                    }
                    else
                    {
                        panelOOBA.Visibility = Visibility.Collapsed;
                    }

                    //Check Route
                    if (chkbCheckRoute == true)
                    {
                        var check_route = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "SFIS1.CHECK_ROUTE",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name="LINE", Value = qurySerN[0].LINE_NAME, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter {Name="MYGROUP", Value = sMyGroup, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter {Name="DATA", Value = qurySerN[0].SERIAL_NUMBER, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter {Name="RES", SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Output }
                        }
                        });
                        dynamic result = check_route.Data;
                        sprocCheckRoute = result[0]["res"];
                    }
                    if (chkbCheckRoute == false || sprocCheckRoute == "OK")
                    {
                        if (itemLineSingle.IsChecked)
                        {
                            if (listSelectLine.Items.IndexOf(qurySerN[0].LINE_NAME) == -1)
                            {
                                lablMsg.Foreground = Brushes.Red;
                                lablMsg.Content = "Different Line !!";
                                combDef.Focus();
                                iFlag = 1;
                                RoKuCheck = false;
                                return;
                            }
                            //differ line
                        }

                        //Check if No Tray Number
                        if (itemCheckTray.IsChecked && qurySerN[0].TRAY_NO == "N/A")
                        {
                            lablMsg.Background = Brushes.Red;
                            lablMsg.Content = "No Tray Number!!";
                            combDef.Focus();
                            iFlag = 1;
                            RoKuCheck = false;
                            return;
                        }

                        //check if No Carton Number
                        if (itemCheckCarton.IsChecked && qurySerN[0].CARTON_NO == "N/A")
                        {
                            lablMsg.Background = Brushes.Red;
                            lablMsg.Content = "No Carton Number!!";
                            combDef.Focus();
                            iFlag = 1;
                            RoKuCheck = false;
                            return;
                        }

                        //check if No Pallet Number
                        if (itemCheckPallet.IsChecked && qurySerN[0].PALLET_NO == "N/A")
                        {
                            lablMsg.Background = Brushes.Red;
                            lablMsg.Content = "No Pallet Number!!";
                            combDef.Focus();
                            iFlag = 1;
                            return;
                        }

                        //check if Passed
                        if (iFlag != 1 && (qurySerN[0].QA_RESULT == "0" || qurySerN[0].QA_RESULT == "2") && qurySerN[0].GROUP_NAME == sMyGroup)
                        {
                            lablMsg.Background = Brushes.Red;
                            lablMsg.Content = "This unit has has verified PASS !";
                            combDef.Focus();
                            iFlag = 1;
                            RoKuCheck = false;
                            return;
                        }


                        int iMO_Status = await CheckMO_Pending(qurySerN[0].MO_NUMBER);
                        if (iMO_Status != 0)
                        {
                            if (iMO_Status == 1)
                            {
                                lablMsg.Background = Brushes.Red;
                                lablMsg.Content = "MO:" + qurySerN[0].MO_NUMBER + " Not Exist! !";
                            }
                            if (iMO_Status == 3)
                            {
                                lablMsg.Background = Brushes.Red;
                                lablMsg.Content = "MO:" + qurySerN[0].MO_NUMBER + " Close !";
                            }
                            if (iMO_Status == 3)
                            {
                                lablMsg.Background = Brushes.Red;
                                lablMsg.Content = "MO:" + qurySerN[0].MO_NUMBER + " Pending !";
                            }
                            combDef.Focus();
                            RoKuCheck = false;
                            return;
                        }

                        //Sampling Plan
                        if (chkbSamplingPlan == true)
                        {
                            if (!await CheckSamplingPlan_sn())
                            {
                                RoKuCheck = false;
                                return;
                            }
                        }

                        //change mo_number when MyGroup = Reject Group(Rework mo -> Orgin MO)
                        //Change Lot NO
                        if (iFlag != 1 && qurySerN[0].QA_NO != "N/A" && !string.IsNullOrEmpty(qurySerN[0].QA_NO) && qurySerN[0].QA_NO != combLot.Text)
                        {
                            combLot.SelectedIndex = combLot.Items.IndexOf(qurySerN[0].QA_NO);
                            await inquireLot();

                            if (comb1.Items.IndexOf(combDef.Text) != -1)
                            {
                                if (itemSamplingSSN.IsChecked)
                                    MessageBox.Show("Duplicate Shipping SN.");
                                else
                                    MessageBox.Show("Duplicate Serial Number.");
                                if (itemRight.IsChecked)
                                    await DisplayRight();
                                if (itemLeft.IsChecked)
                                    await DisplayLeft();
                                await displayCqcSn();
                                if (chkbSamplingPlan == true)
                                    await qurySamplingPlan();
                                G_iComboItem = combLot.SelectedIndex;
                                return;
                            }

                        }

                        if (itemModel.IsChecked && qurySerN[0].MODEL_NAME != G_sModelName && !string.IsNullOrEmpty(G_sModelName))
                            iFlag2 = 1;
                        else
                        {
                            if (qurySerN[0].MODEL_NAME != G_sModelName && !string.IsNullOrEmpty(G_sModelName))
                                iFlag2 = 1;
                            if (itemMO.IsChecked && !string.IsNullOrEmpty(G_sMO))
                            {
                                if (qurySerN[0].MO_NUMBER.Substring(0, 2) == "46")
                                {
                                    if (G_sMO.Substring(0, 2) != "46")
                                    {
                                        MessageWindow mes = new MessageWindow(sfcClient, "00605");
                                        mes.Owner = this;
                                        mes.ShowDialog();
                                        RoKuCheck = false;
                                        return;
                                    }
                                    else
                                        iFlag2 = 1;
                                }
                                else if (G_sMO.Substring(0, 2) == "46")
                                {
                                    if (qurySerN[0].MO_NUMBER.Substring(0, 2) != "46")
                                    {
                                        MessageWindow mes = new MessageWindow(sfcClient, "00605");
                                        mes.Owner = this;
                                        mes.ShowDialog();
                                        RoKuCheck = false;
                                        return;
                                    }
                                    else
                                        iFlag2 = 1;
                                }
                            }
                            else
                            {
                                if (editCompany == "ASUS")
                                {
                                    if ((itemPO.IsChecked && qurySerN[0].PO_NO != sPO && !string.IsNullOrEmpty(sPO)) || (qurySerN[0].PO_LINE != sPOLine && !string.IsNullOrEmpty(sPOLine)))
                                    {
                                        iFlag2 = 1;
                                    }
                                }
                            }
                        }

                        //Check if Same Model_Name
                        if (iFlag != 1 && iFlag2 == 1)
                        {
                            if (combLot.Items.IndexOf(qurySerN[0].QA_NO) == -1)
                            {
                                if (!insertLot())
                                    return;
                                comb1.Items.Clear();
                                comb1.Items.Add(combDef.Text);
                                G_sRL = "Right";

                                //CQC By SSN
                                if (itemSSN.IsChecked)
                                {
                                    if (itemSamplingSN.IsChecked)
                                    {
                                        await updateR107(qurySerN[0].SERIAL_NUMBER);
                                    }
                                    else
                                    {
                                        await updateR107(qurySerN[0].SHIPPING_SN);
                                    }
                                }

                                //CQC By Tray
                                if (itemTray.IsChecked)
                                {
                                    await updateR107(qurySerN[0].TRAY_NO);
                                }

                                //CQC By Carton
                                if (itemCarton.IsChecked)
                                {
                                    await updateR107(qurySerN[0].CARTON_NO);
                                }

                                //CQC By Pallet
                                if ((itemSSN.IsChecked == false && itemTray.IsChecked == false && itemCarton.IsChecked == false) || itemPallet.IsChecked)
                                {
                                    await updateR107(qurySerN[0].PALLET_NO);
                                }
                            }
                            else
                            {
                                combLot.SelectedIndex = combLot.Items.IndexOf(qurySerN[0].QA_NO);
                                await inquireLot();
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(combLot.Text))
                            {
                                if (!insertLot())
                                    return;
                            }
                        }
                        comb1.Items.Add(combDef.Text);
                        G_sRL = "Right";

                        //CQC by SSN/SN
                        if (itemSSN.IsChecked)
                        {
                            if (itemSamplingSN.IsChecked)
                            {
                                await updateR107(qurySerN[0].SERIAL_NUMBER);
                            }
                            else
                            {
                                await updateR107(qurySerN[0].SHIPPING_SN);
                            }
                        }

                        //CQC by Tray
                        if (itemTray.IsChecked)
                            await updateR107(qurySerN[0].TRAY_NO);

                        //CQC by Carton
                        if (itemCarton.IsChecked)
                            await updateR107(qurySerN[0].CARTON_NO);

                        //CQC by Pallet
                        if ((itemSSN.IsChecked == false && itemTray.IsChecked == false && itemCarton.IsChecked == false) || itemPallet.IsChecked)
                            await updateR107(qurySerN[0].PALLET_NO);

                        if (combDef.Items.Count == 0)
                        {
                            iError_F = 0;
                        }
                        else
                        {
                            iError_F = 1;
                        }
                        if (combDef.Text is null || combDef.Text.ToUpper() == "N/A")
                        {
                            MessageBox.Show("SN or SSN is null or N/A | Bản có dữ liệu SN hoặc SSN rỗng hoặc N/A ");
                            RoKuCheck = false;
                            return;
                        }
                        //Update R107
                        sSQL = " update sfism4.r_wip_tracking_t " +
                            " set station_name = '" + sMyStation + "' " +
                            " , group_name ='" + sMyGroup + "', section_name ='" + sMySection + "' " +
                            " , emp_no ='" + G_sTester + "' , " +
                            " IN_STATION_TIME =sysdate ";

                        if (G_sType == "RC")
                            sSQL = sSQL + " ,error_flag ='0' ";
                        else
                            sSQL = sSQL + " ,error_flag ='" + iError_F.ToString() + "' ";

                        if (iError_F == 0)
                            sSQL = sSQL + " ,next_Station ='" + sMyGroup + "' ";
                        else
                            sSQL = sSQL + " ,next_Station ='N/A' ";

                        if (itemSamplingSN.IsChecked)
                            sSQL = sSQL + " where (Serial_Number ='" + combDef.Text + "') ";
                        else
                            sSQL = sSQL + " where (shipping_sn ='" + combDef.Text + "') ";

                        sSQL = sSQL + " and (next_Station ='N/A' or next_Station ='" + sMyGroup + "') ";

                        var sbUpdate = new StringBuilder();
                        sbUpdate.Append(sSQL);
                        var resultUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = sbUpdate.ToString()
                        });
                        G_sModelName = qurySerN[0].MODEL_NAME;
                        G_sMO = qurySerN[0].MO_NUMBER;
                        editModel.Text = G_sModelName;
                        editMoNo.Text = qurySerN[0].MO_NUMBER;
                        combDef.Focus();
                        lablMsg.Foreground = Brushes.Blue;
                        if (itemSamplingSN.IsChecked)
                            lablMsg.Content = "Please input Error code or Serial Number.";
                        else
                            lablMsg.Content = "Please input Error code or Shipping SN.";

                        //G_dtTimeTemp = DateTime.Now;
                        await CQCIns(iError_F);
                        await insertQCSN(iError_F);

                        // modify by Tim 2002/06/15
                        if (AutoSampling.IsChecked && iError_F == 0 && G_sType != "RC")
                            await QC_AutoSampling();
                        //---------------------------------
                        await combLot_SelectionChanged1();
                        if (G_sType == "RC")
                        {
                            for (iI = 0; iI <= ListBoxReasonCode.Items.Count - 1; iI++)
                            {
                                await insertR109("ZZZZ", iI);
                                await CheckItem(iI);
                            }
                        }
                        else
                        {
                            for (iI = 0; iI <= combDef.Items.Count - 1; iI++)
                            {
                                await insertR109(combDef.Items[iI].ToString(), iI);
                            }
                        }
                        if (itemSSN.IsChecked)
                        {
                            if (itemSamplingSN.IsChecked)
                            {
                                G_sPalletNo = qurySerN[0].SERIAL_NUMBER;
                            }
                            else
                            {
                                G_sPalletNo = qurySerN[0].SHIPPING_SN;
                            }
                        }
                        if (itemTray.IsChecked)
                        {
                            G_sPalletNo = qurySerN[0].TRAY_NO;
                        }
                        if (itemCarton.IsChecked)
                        {
                            G_sPalletNo = qurySerN[0].CARTON_NO;
                        }
                        if ((itemSSN.IsChecked == false && itemTray.IsChecked == false && itemCarton.IsChecked == false) || itemPallet.IsChecked)
                        {
                            G_sPalletNo = qurySerN[0].PALLET_NO;
                        }
                        //if (G_sLotNo == combLot.Text && clstrgridInLot.Items[0] != null)
                        if (G_sLotNo == combLot.Text && clstrgridInLot.Items != null)
                        {
                            {
                                sSelectPallet = G_sPalletNo;
                                if ((itemLeft.IsChecked) && (itemRight.IsChecked))
                                {
                                    displayDataNoChange();
                                }
                                else if (itemRight.IsChecked)
                                {
                                    await DisplayRight();
                                }
                                else if (itemLeft.IsChecked)
                                {
                                    await DisplayLeft();
                                }
                            }
                        }
                        else
                        {
                            if (itemRight.IsChecked)
                            {
                                await DisplayRight();
                            }
                            if (itemLeft.IsChecked)
                            {
                                await DisplayLeft();
                            }
                        }
                        await displayCqcSn();

                        //show Sampling Plan
                        if (chkbSamplingPlan == true)
                        {
                            await qurySamplingPlan();
                        }
                        if (itemSamplingSN.IsChecked)
                        {
                            string SQL_Error = " select test_code,error_desc " +
                            " from sfism4.r_repair_t r9, sfism4.r_qc_sn_t r7, " +
                            "      sfis1.c_error_code_t ec " +
                            " where r9.serial_number = r7.serial_number " +
                            "   and r7.counter='0' " +
                            "   and r9.test_time = r7.test_time " +
                            "   and r9.test_code = ec.error_code " +
                            "   and r7.lot_no = '" + combLot.Text + "' and r7.Serial_Number = '" + combDef.Text + "' ";
                            var quryError = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQL_Error,
                                SfcCommandType = SfcCommandType.Text
                            });
                            dbgridErrorCode.ItemsSource = quryError.Data.ToListObject<ERROR>().ToList();

                        }
                        if (itemSamplingSSN.IsChecked)
                        {
                            string SQL_Error = " select test_code,error_desc " +
                           " from sfism4.r_repair_t r9, sfism4.r_qc_sn_t r7, " +
                           "      sfis1.c_error_code_t ec " +
                           " where r9.serial_number = r7.serial_number " +
                           "   and r7.counter='0' " +
                           "   and r9.test_time = r7.test_time " +
                           "   and r9.test_code = ec.error_code " +
                           "   and r7.lot_no = '" + combLot.Text + "' and r7.shipping_Sn = '" + combDef.Text + "' ";
                            var quryError = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQL_Error,
                                SfcCommandType = SfcCommandType.Text
                            });
                            dbgridErrorCode.ItemsSource = quryError.Data.ToListObject<ERROR>().ToList();
                        }

                        G_sLotNo = combLot.Text;
                        combDef.Items.Clear();
                        ListBoxLocation.Items.Clear();
                        ListBoxReasonCode.Items.Clear();
                        ListBoxDutyCode.Items.Clear();
                        G_sType = string.Empty;
                        G_sReasonCode = string.Empty;
                        G_sDutycode = string.Empty;
                        G_sLocation = string.Empty;
                        combDef.Focus();
                    }
                    else
                    {
                        if (chkbCheckRoute == true)
                        {
                            MessageBox.Show("Route Error - " + sprocCheckRoute.ToString(), "CQC");
                            RoKuCheck = false;
                            return;
                        }
                    }
                }
                else
                {
                    lablMsg.Foreground = Brushes.Red;
                    if (itemSamplingSN.IsChecked == true)
                    {
                        lablMsg.Content = "Serial Number does not exist !";
                    }
                    else
                    {
                        lablMsg.Content = "Shipping SN does not exist !";
                    }
                    combDef.Focus();
                }
            }
            catch
            {
                MessageBox.Show("Loi thuc thi ham CheckSerN() ");
                RoKuCheck = false;
            }
        }

        void displayDataNoChange()
        {
            List<R107> datalist = clstrgridInLot.ItemsSource as List<R107>;
            {
                if (itemSSN.IsChecked)
                {
                    if (itemSSN.IsChecked)
                    {
                        if (datalist.Count(c => c.SHIPPING_SN == sSelectPallet) > 0)
                            return;
                    }
                    else
                    {
                        if (datalist.Count(c => c.SERIAL_NUMBER == sSelectPallet) > 0)
                            return;
                    }
                }
                if (itemTray.IsChecked)
                    if (datalist.Count(c => c.TRAY_NO == sSelectPallet) > 0)
                        return;
                if (itemCarton.IsChecked)
                    if (datalist.Count(c => c.CARTON_NO == sSelectPallet) > 0)
                        return;
                if (itemPallet.IsChecked)
                    if (datalist.Count(c => c.PALLET_NO == sSelectPallet) > 0)
                        return;
            }
            //xuan tam thoi chua biet viet doan nay the nao
            // truong hop nay khi tich chon ca 2 left va ringth
            //string sClearTemp;
            //List<R107> tmplist = clstrgridNotChecked.ItemsSource as List<R107>;
            //for(int i=0; i<tmplist.Count; i++)
            //{
            //    if (itemSSN.IsChecked)
            //    {
            //        if (itemSSN.IsChecked)
            //            if (sSelectPallet == datalist[i].SHIPPING_SN)
            //            {
            //                sClearTemp = datalist[i].COUNT;
            //                if (tmplist.Count == 1 && tmplist[0].SHIPPING_SN!="")
            //                    tmplist.RemoveAt(0);
            //                datalist[i] = datalist[i + 1];
            //                if(datalist.Count==1 && datalist[0].SHIPPING_SN == "")
            //                {
            //                    datalist[0].SHIPPING_SN = sSelectPallet;
            //                    datalist[0].COUNT = sClearTemp;
            //                }
            //                else
            //                {
            //                    datalist[datalist.Count - 1].SHIPPING_SN = sSelectPallet;
            //                    datalist[datalist.Count - 1].COUNT = sClearTemp;
            //                }
            //                editCount.Content = (int.Parse(sClearTemp) + int.Parse(editCount.Content.ToString())).ToString();
            //            }
            //        else
            //            if (sSelectPallet == datalist[i].SERIAL_NUMBER)
            //            {

            //            }
            //    }
            //    if (itemTray.IsChecked)
            //        if (datalist.Count(c => c.TRAY_NO == sSelectPallet) > 0)
            //            return;
            //    if (itemCarton.IsChecked)
            //        if (datalist.Count(c => c.CARTON_NO == sSelectPallet) > 0)
            //            return;
            //    if (itemPallet.IsChecked)
            //        if (datalist.Count(c => c.PALLET_NO == sSelectPallet) > 0)
            //}
            sSelectPallet = "";
        }

        async Task CheckItem(int iI)
        {
            string sSQL = "";
            sSQL = " SELECT MODEL_NAME from SFIS1.C_ITEM_DESC_T  " +
                   " where model_Name =:model_Name " +
                   " and item_code =:item_code ";

            var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = " SELECT MODEL_NAME from SFIS1.C_ITEM_DESC_T  " +
                   " where model_Name =:model_Name " +
                   " and item_code =:item_code ",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "model_Name", Value = qurySerN[0].MODEL_NAME },
                        new SfcParameter{ Name = "item_code", Value = ListBoxLocation.Items[iI].ToString() }
                    }
            });
            if (result.Data == null)
            {
                sSQL = "insert into SFIS1.C_ITEM_DESC_T " +
                        " (model_Name,item_code,item_name) " +
                        " values('" + qurySerN[0].MODEL_NAME + "','" + ListBoxLocation.Items[iI].ToString() + "',' ') ";
                var sbInsert = new StringBuilder();
                sbInsert.Append(sSQL);
                var resultInsert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbInsert.ToString()
                });
            }

        }

        async Task insertR109(string _Test_Code, int _Index)
        {
            if (editCompany == "DBTEL" && !await F_WORK_SECTION(qurySerN[0].LINE_NAME, DateTime.Now))
                return;
            await FindWorkSection(qurySerN[0].LINE_NAME);
            if (sDay_Distinct == "TODAY")
                sDay_Distinct = DateTime.Now.ToString("yyyyMMdd");
            if (sDay_Distinct == "TOMORROW")
                sDay_Distinct = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            if (sDay_Distinct == "YESTERDAY")
                sDay_Distinct = DateTime.Now.AddDays(+1).ToString("yyyyMMdd");
            string sSQL = " insert into SFISM4.R_Repair_T " +
                          " ( serial_number, mo_number, model_name " +
                          "  , test_time, test_code, test_station " +
                          "  , test_line, record_type,TESTER, test_group, test_section " +
                          "  , T_Work_Section,T_Class,T_Class_Date ";
            if (_Test_Code == "ZZZZ")
            {
                sSQL += "  ,REASON_CODE, ERROR_ITEM_CODE , DUTY_TYPE  " +
                   "  ,R_WORK_SECTION ,R_CLASS ,R_CLASS_DATE  " +
                   "  ,REPAIR_STATION ,REPAIR_GROUP ,REPAIR_SECTION  " +
                   "  ,REPAIR_TIME , REPAIRER  )";
            }
            else
                sSQL += ")";
            sSQL += " select serial_number, mo_number, model_name, sysdate " +
                "       , " + _Test_Code + "," + sMyStation + ", line_name, 'T',EMP_NO " +
                "       , " + sMyGroup + ", " + sMySection + " " +
                "       , " + G_iWSection + "," + sClass + "," + sDay_Distinct + " ";
            if (_Test_Code == "ZZZZ")
            {
                sSQL = sSQL + " , " + ListBoxReasonCode.Items[_Index].ToString() + "," + ListBoxLocation.Items[_Index].ToString() + ", " + ListBoxDutyCode.Items[_Index].ToString() + " " +
                   " , " + G_iWSection + "," + sClass + "," + sDay_Distinct + " " +
                   " , " + sMyStation + "," + sMyGroup + "," + sMySection +
                   " , sysdate,EMP_NO ";
            }
            sSQL = sSQL + " from sfism4.r_wip_tracking_t ";
            if (itemSamplingSN.IsChecked)
            {
                sSQL = sSQL + " where serial_number ='" + combDef.Text + "'";
            }
            else
                sSQL = sSQL + " where shipping_sn ='" + combDef.Text + "' ";

            var sbInsert = new StringBuilder();
            sbInsert.Append(sSQL);
            var resultInsert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = sbInsert.ToString()
            });
        }

        async Task<bool> F_WORK_SECTION(string _sLine, DateTime _TDATETIME)
        {
            string sSQL = "  SELECT CLASS,DAY_DISTINCT ,WORK_SECTION  " +
                         "  FROM SFIS1.C_WORK_DESC_T " +
                         "  WHERE LINE_NAME= '" + _sLine + "' AND SECTION_NAME='Default' " +
                         "  AND TO_CHAR('" + _TDATETIME + "','HH24MI')>=START_TIME AND TO_CHAR('" + _TDATETIME + "','HH24MI')<END_TIME ";
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
            {
                G_Class = qury.Data["class"].ToString();
                G_Work_Section = int.Parse(qury.Data["work_section"].ToString());
                if (qury.Data["day_distinct"].ToString() == "TODAY")
                    G_Class_Date = DateTime.Now.ToString("yyyyMMdd");
                if (qury.Data["day_distinct"].ToString() == "TOMORROW")
                    G_Class_Date = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                if (qury.Data["day_distinct"].ToString() == "YESTERDAY")
                    G_Class_Date = DateTime.Now.AddDays(+1).ToString("yyyyMMdd");
                return true;
            }
            else
            {
                MessageBox.Show("You must setup CLASS!");
            }
            return false;
        }

        async Task insertQCSN(int _Error_Flag)
        {
            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select SERIAL_NUMBER from  sfism4.r_qc_sn_t where SERIAL_NUMBER ='" + qurySerN[0].SERIAL_NUMBER + "' and COUNTER=0 AND ROWNUM= 1",
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data.Count() > 0)
            {
                try
                {
                    var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "update sfism4.r_qc_sn_t set counter = counter+1 where serial_number = '" + qurySerN[0].SERIAL_NUMBER + "' and COUNTER <> 99"
                    });
                }
                catch
                {
                    MessageBox.Show("update error!!");
                }
            }
            await FindWorkSection(qurySerN[0].LINE_NAME);
            if (sDay_Distinct == "TODAY")
                sDay_Distinct = DateTime.Now.ToString("yyyyMMdd");
            if (sDay_Distinct == "TOMORROW")
                sDay_Distinct = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            if (sDay_Distinct == "YESTERDAY")
                sDay_Distinct = DateTime.Now.AddDays(+1).ToString("yyyyMMdd");

            string SQL = "";
            if (chkbInsertQCSN)
            {
                SQL = "insert into sfism4.r_qc_sn_t " +
                      "(lot_no, model_name, serial_number,shipping_sn, error_flag, test_time, tester, counter,SECTION_NAME,group_name,line_name,Station_name,Condition_Flag,Class,Class_Date)" +
                      "values ('" + combLot.Text + "','" + qurySerN[0].MODEL_NAME + "','" + qurySerN[0].SERIAL_NUMBER + "', '" + qurySerN[0].SHIPPING_SN + "','" + _Error_Flag + "',sysdate,'" + G_sTester + "',0,'" + sMySection + "','" + sMyGroup + "','" + sMyLine + "','" + sMyStation + "','" + editConFlag.Text + "','" + sClass + "','" + sDay_Distinct + "')";
            }
            else
            {
                SQL = "insert into sfism4.r_qc_sn_t " +
                     "(lot_no, model_name, serial_number,shipping_sn, error_flag, test_time, tester, counter,Condition_Flag,Class,Class_Date)" +
                     "values ('" + combLot.Text + "','" + qurySerN[0].MODEL_NAME + "','" + qurySerN[0].SERIAL_NUMBER + "', '" + qurySerN[0].SHIPPING_SN + "','" + _Error_Flag + "',sysdate,'" + G_sTester + "',0,'" + editConFlag.Text + "','" + sClass + "','" + sDay_Distinct + "')";
            }
            var insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
            {
                CommandText = SQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (insert.Result != "OK")
            {
                MessageBox.Show("insert sfism4.r_qc_sn_t error", "CQC");
                return;
            }
        }

        async Task QC_AutoSampling()
        {
            await FindWorkSection(qurySerN[0].LINE_NAME);
            if (sDay_Distinct == "TODAY")
                sDay_Distinct = DateTime.Now.ToString("yyyyMMdd");
            if (sDay_Distinct == "TOMORROW")
                sDay_Distinct = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            if (sDay_Distinct == "YESTERDAY")
                sDay_Distinct = DateTime.Now.AddDays(+1).ToString("yyyyMMdd");

            string sSQL = "SELECT SERIAL_NUMBER, SHIPPING_SN FROM SFISM4.R_WIP_TRACKING_T " +
                          "WHERE QA_NO = '" + combLot.Text + "' AND SERIAL_NUMBER NOT IN " +
                          "(SELECT SERIAL_NUMBER FROM SFISM4.R_QC_SN_T WHERE LOT_NO = '" + combLot.Text + "' )";
            var quryR109 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryR109.Data.Count() > 0)
            {
                try
                {
                    List<R107> updatelist = new List<R107>();
                    updatelist = quryR109.Data.ToListObject<R107>().ToList();
                    foreach (R107 items in updatelist)
                    {
                        await Auto_CQCIns(items.SERIAL_NUMBER, items.SHIPPING_SN, sClass, sDay_Distinct);
                        await Auto_insertQCSN(items.SERIAL_NUMBER, items.SHIPPING_SN, sClass, sDay_Distinct);
                    }
                }
                catch
                {
                    MessageBox.Show("AutoInsert DataBase Error!");
                }
            }
        }

        async Task Auto_insertQCSN(string _SN, string _SSN, string _sClass, string _sDAy_Distinct)
        {
            string _Error_Flag = "0";
            if (G_sType == "RC")
                _Error_Flag = "1";
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select SERIAL_NUMBER from  sfism4.r_qc_sn_t where SERIAL_NUMBER ='" + _SN + "' and COUNTER=0 AND ROWNUM= 1",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null /*&& qury.Data.Count > 0*/)
            {
                try
                {
                    var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "update sfism4.r_qc_sn_t set counter = counter+1 where serial_number = '" + _SN + "' and COUNTER <> 99"
                    });
                }
                catch
                {
                    MessageBox.Show("update error!!");
                }
            }

            string SQL = "";
            if (chkbInsertQCSN)
            {
                SQL = "insert into sfism4.r_qc_sn_t " +
                      "(lot_no, model_name, serial_number,shipping_sn, error_flag, test_time, tester, counter,SECTION_NAME,group_name,line_name,Station_name,Condition_Flag,Class,Class_Date)" +
                      "values ('" + combLot.Text + "','" + qurySerN[0].MODEL_NAME + "','" + _SN + "', '" + _SSN + "','" + _Error_Flag + "',sysdate,'" + G_sTester + "',0,'" + sMySection + "','" + sMyGroup + "','" + sMyLine + "','" + sMyStation + "','" + editConFlag.Text + "','" + _sClass + "','" + _sDAy_Distinct + "')";
            }
            else
            {
                SQL = "insert into sfism4.r_qc_sn_t " +
                     "(lot_no, model_name, serial_number,shipping_sn, error_flag, test_time, tester, counter,Condition_Flag,Class,Class_Date)" +
                     "values ('" + combLot.Text + "','" + qurySerN[0].MODEL_NAME + "','" + _SN + "', '" + _SSN + "','" + _Error_Flag + "',sysdate,'" + G_sTester + "',0,'" + editConFlag.Text + "','" + _sClass + "','" + _sDAy_Distinct + "')";
            }
            try
            {
                var insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = SQL,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            catch
            {
                MessageBox.Show("insert error!!!");
            }
        }
        async Task Auto_CQCIns(string _SN, string _SSN, string _sClass, string _sDAy_Distinct)
        {
            int iPass = 0, iFail = 0;
            string sql1 = "select * from SFISM4.R_QC_SN_T ";
            if (itemSamplingSN.IsChecked)
                sql1 += " where Serial_Number = '" + _SN + "' and lot_no = '" + combLot.Text + "'";
            else
                sql1 += " where SHIPPING_SN = '" + _SSN + "' and lot_no = '" + combLot.Text + "'";

            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sql1,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data.Count() == 0)
            {
                iPass = 1;
                iFail = 0;
            }
            else
            {
                iPass = 0;
                iFail = 0;
            }

            string sSQL = "update SFISM4.R_cqc_rec_T " +
                          "set tester = '" + G_sTester + "', pass_qty = pass_qty + " + iPass + ", " +
                          "fail_qty = fail_qty + " + iFail + ", target_qty = target_qty + 1 ,Class ='" + _sClass + "', Class_Date = '" + _sDAy_Distinct + "' ";

            if (chkbSamplingPlan)
                sSQL += ", CRITICAL_FAIL_QTY = CRITICAL_FAIL_QTY + " + G_iCritical + " , MAJOR_FAIL_QTY = MAJOR_FAIL_QTY + " + G_iMajor + " , MINOR_FAIL_QTY = MINOR_FAIL_QTY + " + G_iMinor + " ";
            sSQL += " where lot_no = '" + combLot.Text + "'";

            try
            {
                var quryIns = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSQL,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            catch
            {
                MessageBox.Show("Update error!!!");
            }
            G_iCritical = 0;
            G_iMajor = 0;
            G_iMinor = 0;
        }
        async Task CQCIns(int Test)
        {
            int iPass = 0, iFail = 0;
            string start_time;
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from  sfism4.r_cqc_rec_t where lot_no ='" + combLot.Text + "' and start_time is not null",
                SfcCommandType = SfcCommandType.Text
            });

            if (qury.Data.Count() > 0)
            {
                List<CQCREC> cqclist = new List<CQCREC>();
                cqclist = qury.Data.ToListObject<CQCREC>().ToList();
                start_time = cqclist[0].START_TIME;
            }
            else
                start_time = DateTime.Now.ToString("yyyy/MM/dd HH:mm");

            await FindWorkSection(qurySerN[0].LINE_NAME);
            if (sDay_Distinct == "TODAY")
                sDay_Distinct = DateTime.Now.ToString("yyyyMMdd");
            if (sDay_Distinct == "TOMORROW")
                sDay_Distinct = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            if (sDay_Distinct == "YESTERDAY")
                sDay_Distinct = DateTime.Now.AddDays(+1).ToString("yyyyMMdd");
            string sql1 = "select * from SFISM4.R_QC_SN_T ";
            if (itemSamplingSN.IsChecked)
                sql1 += " where Serial_Number = '" + combDef.Text + "' and lot_no = '" + combLot.Text + "'";
            else
                sql1 += " where SHIPPING_SN = '" + combDef.Text + "' and lot_no = '" + combLot.Text + "'";

            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sql1,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data.Count() == 0)
            {
                if (Test == 0)
                {
                    iPass = 1;
                    iFail = 0;
                }
                else
                {
                    iPass = 0;
                    iFail = 1;
                }
            }
            else
            {
                iPass = 0;
                iFail = 0;
            }

            string sSQL = "update SFISM4.R_cqc_rec_T " +
                          "set tester = '" + G_sTester + "', start_time = TO_DATE('" + start_time + "','YYYY/MM/DD HH24:MI:SS'),model_name = '" + G_sModelName + "', pass_qty = pass_qty + " + iPass + ", " +
                          "fail_qty = fail_qty + " + iFail + ", target_qty = target_qty + 1 ,Class ='" + sClass + "', Class_Date = '" + sDay_Distinct + "', PO_NUMBER = '" + ComboBox1.Text + "' ";

            if (chkbSamplingPlan)
                sSQL += ", CRITICAL_FAIL_QTY = CRITICAL_FAIL_QTY + " + G_iCritical + " , MAJOR_FAIL_QTY = MAJOR_FAIL_QTY + " + G_iMajor + " , MINOR_FAIL_QTY = MINOR_FAIL_QTY + " + G_iMinor + " ";
            sSQL += "where lot_no = '" + combLot.Text + "'";

            var quryIns = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryIns.Result != "OK")
            {
                MessageBox.Show("update SFISM4.R_cqc_rec_T error", "CQC");
                return;
            }
            G_iCritical = 0;
            G_iMajor = 0;
            G_iMinor = 0;
        }

        async Task FindWorkSection(string Line)
        {
            string ssql = " SELECT Work_Section,Class,Day_Distinct FROM SFIS1.C_WORK_DESC_T " +
                         " WHERE START_TIME <= TO_CHAR(SYSDATE ,'HH24MI') AND END_TIME > TO_CHAR(SYSDATE ,'HH24MI') " +
                        " AND SECTION_NAME = 'Default' AND SHIFT = '1' AND LINE_NAME ='" + Line + "' ";
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data.Count() > 0)
            {
                List<WORK_DESC_T> timelist = new List<WORK_DESC_T>();
                timelist = qury.Data.ToListObject<WORK_DESC_T>().ToList();
                sClass = timelist[0].CLASS;
                sDay_Distinct = timelist[0].DAY_DISTINCT;
                G_iWSection = Int32.Parse(timelist[0].WORK_SECTION);
            }
        }

        async Task updateR107(string sPalletNo)
        {
            string sSQL = "";
            if ((sPalletNo == "" || sPalletNo == "N/A") && itemSSN.IsChecked)
            {
                MessageBox.Show("SN or SSN is null, can not do FQA");
                return;
            }
            if ((sPalletNo == "" || sPalletNo == "N/A") && itemTray.IsChecked)
            {
                MessageBox.Show("Tray NO is null, can not do FQA");
                return;
            }
            if ((sPalletNo == "" || sPalletNo == "N/A") && itemCarton.IsChecked)
            {
                MessageBox.Show("Carton NO is null,can not do FQA");
                return;
            }
            if ((sPalletNo == "" || sPalletNo == "N/A") && itemPallet.IsChecked)
            {
                MessageBox.Show("Pallet NO is null, can not do FQA");
                return;
            }

            //CQC by SSN
            if (itemSSN.IsChecked)
            {
                sSQL = "update sfism4.r_wip_tracking_t set qa_no =: qa_no ";
                if (itemSamplingSN.IsChecked)
                {
                    sSQL = sSQL + " where Serial_number ='" + sPalletNo + "' ";
                }
                else
                {
                    sSQL = sSQL + " where Shipping_SN ='" + sPalletNo + "' ";
                }
            }

            //CQC by Tray
            if (itemTray.IsChecked)
            {
                sSQL = " update sfism4.r_wip_tracking_t set qa_no =: qa_no where Tray_no ='" + sPalletNo + "' ";
            }

            //CQC by Carton
            if (itemCarton.IsChecked)
            {
                sSQL = " update sfism4.r_wip_tracking_t set qa_no =: qa_no where Carton_no ='" + sPalletNo + "' ";
            }

            //CQC by Pallet
            if (itemPallet.IsChecked)
            {
                sSQL = " update sfism4.r_wip_tracking_t set qa_no =: qa_no where pallet_no ='" + sPalletNo + "' ";
            }

            if (itemLineSingle.IsChecked && listSelectLine.Items.Count == 1)
            {
                sSQL = sSQL + " and line_name='" + listSelectLine.Items[0].ToString() + "'";
            }
            else
            {
                if (itemLineSingle.IsChecked && listSelectLine.Items.Count > 1)
                {
                    sSQL = sSQL + " and (line_name='" + listSelectLine.Items[0].ToString() + "'";
                    for (int i = 1; i <= (listSelectLine.Items.Count - 1); i++)
                    {
                        sSQL = sSQL + " or line_name='" + listSelectLine.Items[i].ToString() + "'";
                    }
                    sSQL = sSQL + " )";
                }
            }
            sSQL = sSQL + " and (group_name ='" + combGroup + "' or NEXT_STATION = '" + sMyGroup + "' or WIP_GROUP = '" + sMyGroup + "'   ) ";

            var sbUpdate = new StringBuilder();
            sbUpdate.Append(sSQL);
            if (G_sRL == "Right")
            {
                var resultUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString(),
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name = "qa_no", Value =combLot.Text, SfcParameterDataType = SfcParameterDataType.Varchar2}
                }
                });
            }
            else
            {
                var resultUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = sbUpdate.ToString(),
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name = "qa_no", Value ="N/A", SfcParameterDataType = SfcParameterDataType.Varchar2}
                }
                });
            }

        }

        bool insertLot()
        {
            DefaultWindow formDefault = new DefaultWindow(this, sfcClient);
            formDefault.Owner = this;
            formDefault.ShowDialog();
            if (ModalResult)
                return true;
            return false;
        }

        async Task qurySamplingPlan()
        {
            string sSamplingPlan = "";
            int iSNCount = 0;
            if (string.IsNullOrEmpty(combLot.Text))
                return;
            string sSQL = "select * from SFISM4.R_CQC_REC_T where LOT_NO = '" + combLot.Text + "'";
            var quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data.Count() > 0)
            {
                List<CQCREC> cqclist = new List<CQCREC>();
                cqclist = quryTemp.Data.ToListObject<CQCREC>().ToList();
                editCritical.Text = cqclist[0].CRITICAL_FAIL_QTY;
                editMajor.Text = cqclist[0].MAJOR_FAIL_QTY;
                editMinor.Text = cqclist[0].MINOR_FAIL_QTY;
            }
            sSQL = "select type,count(distinct serial_number) Count from SFISM4.R_WIP_TRACKING_T where QA_NO= '" + combLot.Text + "' Group by TYPE,QA_NO";
            quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data.Count() > 0)
            {
                List<R107> countlist = new List<R107>();
                countlist = quryTemp.Data.ToListObject<R107>().ToList();
                if (!string.IsNullOrEmpty(countlist[0].TYPE))
                {
                    sSamplingPlan = countlist[0].TYPE;
                    iSNCount = int.Parse(countlist[0].COUNT);
                }
            }
            else
            {
                MessageBox.Show("NO Sampling Plan !!");
                editCritical.Text = "0";
                editMajor.Text = "0";
                editMinor.Text = "0";
                editSampleQty.Text = "0";
                editOQAType.Text = "";
                return;
            }

            sSQL = "SELECT * FROM SFIS1.C_OQA_SAMPLING_PLAN  WHERE OQA_TYPE= '" + sSamplingPlan + "' AND LOT_SIZE_MIN <= '" + iSNCount + "' AND LOT_SIZE_MAX >= '" + iSNCount + "' and GROUP_NAME ='" + sMyGroup + "'";
            quryTemp = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryTemp.Data.Count() == 0)
            {
                MessageBox.Show("NO Sampling Plan !!");
                sSamplingPlan = "";
                iSNCount = 0;
                editCritical.Text = "0";
                editMajor.Text = "0";
                editMinor.Text = "0";
                editSampleQty.Text = "0";
                editOQAType.Text = "";
                return;
            }
            editOQAType.Text = sSamplingPlan;
            List<OQA> oqa_list = new List<OQA>();
            oqa_list = quryTemp.Data.ToListObject<OQA>().ToList();
            if (quryTemp.Data.Count() > 0)
                editSampleQty.Text = oqa_list[0].SAMPLE_SIZE;
            else
                editSampleQty.Text = "0";
            if (int.Parse(editCheckedQty.Content.ToString()) >= int.Parse(editSampleQty.Text.ToString()) && editCheckQty.Text != "0")
            {
                if (int.Parse(oqa_list[0].MAJOR_REJECT_QTY) <= int.Parse(editMajor.Text))
                {
                    MessageBox.Show("MAJOR REJECT !!");
                    return;
                }

                if (int.Parse(oqa_list[0].MINOR_REJECT_QTY) <= int.Parse(editMinor.Text))
                {
                    MessageBox.Show("MINOR REJECT !!");
                    return;
                }
                MessageBox.Show("PASS !!");
            }

        }

        async Task displayCqcSn()
        {
            int iFail_Qty = 0;
            string sSQL;
            //quryCQCErr
            if (itemSamplingSN.IsChecked)
            {
                sSQL = " SELECT r.Serial_Number SN, r.Error_Flag flag, Carton_No Carton, Pallet_No Pallet, r.model_name model " +
                      " FROM   SFISM4.R_QC_SN_T r, sfism4.r_wip_tracking_t r7 " +
                      " where  lot_no = '" + combLot.Text + "' and counter = 0    and  r.serial_number = r7.serial_number";
            }
            else
            {
                sSQL = " SELECT r.Serial_Number SN,r.Shipping_SN SSN, r.Error_Flag flag, Carton_No Carton, Pallet_No Pallet, r.model_name model" +
                      " FROM   SFISM4.R_QC_SN_T r, sfism4.r_wip_tracking_t r7 " +
                      " where  lot_no = '" + combLot.Text + "' and counter = 0 and  r.serial_number = r7.serial_number";
            }
            if (chkbInsertQCSN == true)
                sSQL += " and r.group_name = '" + sMyGroup + "' ";
            sSQL += " order by r.test_time ";

            var quryCQCErr = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryCQCErr.Data.Count() != 0)
            {
                CQCErr_List = quryCQCErr.Data.ToListObject<R107>().ToList();
                iFail_Qty = CQCErr_List.Count(c => c.FLAG == "1");
                dbgridSSN.ItemsSource = CQCErr_List;
            }

            editFailedQty.Content = iFail_Qty.ToString();
            editCheckedQty.Content = quryCQCErr.Data.Count().ToString();
            if (float.Parse(editCount.Content.ToString()) != 0)
            {
                editPassRate.Content = ((1 - float.Parse(editFailedQty.Content.ToString()) / float.Parse(editCount.Content.ToString())) * 100).ToString();
            }
        }
        async Task inquireLot()
        {
            string sSQL;
            if (itemMO.IsChecked)
            {
                sSQL = " select  r2.mo_number,qa_no, r.Tester " +
                      "       , r.line_name, r.customer, c.emp_name " +
                      " from sfism4.r_cqc_rec_t r,sfis1.c_emp_desc_t c " +
                      "     ,SFISM4.R_WIP_TRACKING_T r2 " +
                      "where Lot_No = '" + combLot.Text + "' and r.Tester = c.emp_no and r2.qa_no=lot_no " +
                      "group by r2.mo_number,qa_no,r.Tester, r.line_name, r.customer, c.emp_name ";
            }
            else
            {
                sSQL = " select Model_Name, Tester, line_name, customer, emp_name, PO_NUMBER ,PO_LINE " +
                      " from sfism4.r_cqc_rec_t r, sfis1.c_emp_desc_t c " +
                      " where Lot_No = '" + combLot.Text + "' and r.Tester = c.emp_no " +
                      " group by Model_Name, r.lot_no, r.Tester, r.line_name, r.customer, c.emp_name ";
            }
            var quryModel = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (quryModel.Data != null)
            {
                //if (formDefault != null)
                //{
                //    if(formDefault.combLot.Items.IndexOf(combLot.Text)==-1)
                //    formDefault.combLot.Text = combLot.Text;
                //    if (formDefault.combLine.Items.IndexOf(quryModel.Data["line_name"].ToString()) == -1)
                //        formDefault.combLine.Items.Add(quryModel.Data["line_name"].ToString());

                //    formDefault.combLine.SelectedIndex = formDefault.combLine.Items.IndexOf(quryModel.Data["line_name"].ToString());

                //    formDefault.combCus.SelectedIndex = formDefault.combCus.Items.IndexOf(quryModel.Data["customer"].ToString());

                //    if (formDefault.combEmp.Items.IndexOf(quryModel.Data["emp_name"].ToString()) == -1)
                //        formDefault.combEmp.Items.Add(quryModel.Data["emp_name"].ToString());

                //    formDefault.combEmp.SelectedIndex = formDefault.combEmp.Items.IndexOf(quryModel.Data["emp_name"].ToString());
                //}

                if (itemModel.IsChecked)
                    G_sModelName = quryModel.Data["model_name"].ToString();
                else
                    G_sModelName = string.Empty;

                if (itemMO.IsChecked)
                    G_sMO = quryModel.Data["mo_number"].ToString();
                else
                    G_sMO = string.Empty;

                if (itemPO.IsChecked)
                {
                    sPO = quryModel.Data["po_number"].ToString();
                    sPOLine = quryModel.Data["po_line"].ToString();
                }
                else
                {
                    sPO = string.Empty;
                    sPOLine = string.Empty;
                }
                sMyLine = quryModel.Data["line_name"].ToString();

                var qurySn = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "SELECT serial_number,Shipping_SN FROM SFISM4.R_QC_SN_T where lot_no = '" + combLot.Text + "' and counter = 0 order by serial_number,Shipping_SN",
                    SfcCommandType = SfcCommandType.Text
                });
                comb1.Items.Clear();

                if (itemSamplingSN.IsChecked)
                {
                    foreach (R107 items in qurySn.Data.ToListObject<R107>().ToList())
                        comb1.Items.Add(items.SERIAL_NUMBER);
                }
                else
                {
                    foreach (R107 items in qurySn.Data.ToListObject<R107>().ToList())
                        comb1.Items.Add(items.SHIPPING_SN);
                }
            }
            else
            {

            }
        }
        async Task<bool> CheckSamplingPlan_sn()
        {
            string sSamplePlan;
            string sql = "select * from SFISM4.R_WIP_TRACKING_T ";
            if (itemSamplingSN.IsChecked)
            {
                sql += "where serial_number= '" + combDef.Text + "'";
            }
            else
                sql += "where shipping_sn= '" + combDef.Text + "'";
            var quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (string.IsNullOrEmpty(quryTemp.Data["type"].ToString()))
            {
                MessageBox.Show("NO Sampling Plan !!");
                editCritical.Text = "0";
                editMajor.Text = "0";
                editMinor.Text = "0";
                editSampleQty.Text = "0";
                editOQAType.Text = string.Empty;
                sSamplePlan = " ";
                return false;
            }
            sSamplePlan = quryTemp.Data["type"].ToString();

            sql = "SELECT * FROM SFIS1.C_OQA_SAMPLING_PLAN WHERE OQA_TYPE= '" + sSamplePlan + "'  and GROUP_NAME ='" + sMyGroup + "'";
            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data.Count() == 0)
            {
                MessageBox.Show("NO Sampling Plan !!");
                editCritical.Text = "0";
                editMajor.Text = "0";
                editMinor.Text = "0";
                editSampleQty.Text = "0";
                editOQAType.Text = "";
                return false;
            }
            return true;
        }

        async Task<int> CheckMO_Pending(string _mo)
        {
            var quryMoPending = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "Select mo_number,Close_Flag from sfism4.r_mo_base_t where mo_number = '" + _mo + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (quryMoPending.Data == null)
            {
                return 1;
            }
            if (quryMoPending.Data["close_flag"].ToString() == "3")
            {
                return 3;
            }
            if (quryMoPending.Data["close_flag"].ToString() == "5")
            {
                var qurytemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select count(Group_Name) C_COUNT from SFISM4.R_MO_PENDING_GROUP_T WHERE MO_NUMBER = '" + _mo + "' AND SECTION_NAME = '" + sMySection + "' AND GROUP_NAME = '" + sMyGroup + "' ",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qurytemp.Data.Count != 0)
                    return 5;
            }
            return 0;
        }
        async Task<bool> FindSerialNO(string macid)
        {
            var quryFindSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select serial_number from sfism4.r_wip_keyparts_t where key_part_no='MACID' and key_part_sn=:key_part_sn and rownum=1",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="key_part_sn", Value=macid.ToUpper() }
                }
            });
            if (quryFindSN.Data != null)
            {
                string temp_SerialNO = quryFindSN.Data["serial_number"].ToString();
                quryFindSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select serial_number from sfism4.r_wip_keyparts_t where key_part_sn=:key_part_sn",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="key_part_sn", Value=temp_SerialNO }
                }
                });
                if (quryFindSN.Data == null)
                {
                    sSerialNO = temp_SerialNO;
                    return true;
                }
                else
                {
                    string temp_SerialNO1 = quryFindSN.Data["serial_number"].ToString();
                    quryFindSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "select serial_number from sfism4.r_wip_keyparts_t where key_part_sn=:key_part_sn",
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()

                        {
                            new SfcParameter {Name="key_part_sn", Value=temp_SerialNO1 }
                        }
                    });
                    if (quryFindSN.Data == null)
                    {
                        sSerialNO = temp_SerialNO1;
                        return true;
                    }
                    else
                    {
                        string temp_SerialNO2 = quryFindSN.Data["serial_number"].ToString();
                        quryFindSN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "select serial_number from sfism4.r_wip_keyparts_t where key_part_sn=:key_part_sn",
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()

                        {
                            new SfcParameter {Name="key_part_sn", Value=temp_SerialNO2 }
                        }
                        });
                        if (quryFindSN.Data == null)
                        {
                            sSerialNO = temp_SerialNO2;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        void send_str(string str)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.WriteLine(str + "\n");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        public string ateformat(string _STR, int _LEN)
        {
            string temp = _STR;
            for (int i = 1; i >= _LEN - _STR.Trim().Length; i++)
            {
                temp += " ";
            }
            return temp;
        }

        public async Task<string> GetPubMessage(string PROMPT_CODE)
        {
            var sql = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT PROMPT_ENGLISH FROM SFIS1.C_PROMPT_CODE_T WHERE PROMPT_CODE = :prompt",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="prompt", Value=PROMPT_CODE }
                }
            });
            if (sql.Data != null)
            {
                string result = sql.Data["prompt_english"].ToString();
                return result;
            }
            return null;
        }
        public async Task<string> CutSNforEnsky(string SN)
        {
            string tmpstr = SN.Substring(3, 7);
            var sql = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT serial_Number FROM SFISM4.R_WIP_TRACKING_T WHERE Po_No=:sn2 and Mo_Number=:mo ",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="sn2", Value=tmpstr },
                    new SfcParameter {Name="mo" , Value=editMoNo.Text }
                }
            });
            if (sql.Data != null)
            {
                string result = sql.Data["serial_Number"].ToString();
                return result;
            }
            return null;
        }
        public async Task<string> GetMac(string SN)
        {
            try
            {
                var result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = "SFIS1.SP_SPECIAL_SN_DEAL",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="station_name" , Value="CQC" , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter {Name="DATA" , Value=SN , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter {Name="cMac" , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Output},
                    new SfcParameter {Name="res" , SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Output }
                }
                });
                dynamic res = result.Data;
                string MAC = res[0]["cmac"];
                return MAC;
            }
            catch (Exception)
            {
                MessageBox.Show("SP_SPECIAL_SN_DEAL Error");
            }
            return null;
        }

        private async void bttnPass_Click(object sender, RoutedEventArgs e)
        {

            ispass = false;
            bool Pass, PassWD;
            string sSQL, sWip_Group ;
            int totalqty, TOTALQTY1, iTotal, iCheck, iPass, iFail;
            string str = string.Empty;

            if (string.IsNullOrEmpty(combLot.Text))
            {
                MessageWindow mes = new MessageWindow(sfcClient, "00598");
                mes.Owner = this;
                mes.ShowDialog();
                return;
            }
            if (!await checkQA(combLot.Text))
            {
                List<OOBA> datalist = ColorStringGrid1.ItemsSource as List<OOBA>;
                oobafrmWindow oobafrm = new oobafrmWindow(combLot.Text, datalist);
                oobafrm.Owner = this;
                oobafrm.ShowDialog();
                return;
            }
            //CHECK ²£½u¬O§_Ã±®Ö­º¥ó   modify by yixianglin
            if (editModel.Text != "")
            {
                await GETCPEIIIFLAG();
                if (!NOcheckcontrolrun)
                {
                    if (!CPEIIIFLAG)
                    {
                        if (!await CHECKMOONLINEVER(editModel.Text))
                        {
                            MessageWindow mes = new MessageWindow(sfcClient, "00600");
                            mes.Owner = this;
                            mes.ShowDialog();
                            return;
                        }
                    }
                }
                var tmpqry = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {

                    CommandText = "select * from sfis1.c_model_confirm_t where type_name='CQC' AND model_type='" + editModel.Text.Trim() + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (tmpqry.Data.Count() > 0)
                {
                    sSQL = "SELECT distinct serial_number FROM SFISM4.R_REPAIR_T WHERE SERIAL_NUMBER IN( " +
                                "SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE TRAY_NO=:DATA OR CARTON_NO=:DATA OR PALLET_NO=:DATA)";
                    if (CQCErr_List[0].PALLET.Length > 5)
                    {
                        tmpqry = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = sSQL,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name = "DATA", Value =CQCErr_List[0].PALLET}
                            }
                        });
                    }
                    else
                    {
                        tmpqry = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = sSQL,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name = "DATA", Value =CQCErr_List[0].CARTON}
                            }
                        });
                    }
                    if (tmpqry.Data.Count() != 0)
                    {
                        int k = 0;
                        foreach (R107 items in tmpqry.Data.ToListObject<R107>().ToList())
                        {
                            if (CQCErr_List.Count(c => c.SN == items.SERIAL_NUMBER) != 0)
                                k = 1;
                            if (k == 0)
                            {
                                MessageBox.Show("sn error!");
                            }
                        }

                    }
                }

                //------------------
                if (string.IsNullOrEmpty(editMoNo.Text))
                {
                    MessageBox.Show("Please scan product first!");
                    return;
                }

                if (await ischeckfile()) // modify by yixianglin
                {
                    if (!await checkModelFile(combLot.Text))
                        return;
                }

                if (await ISCHECKSNMACPREFIX() == true)
                {
                    var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE QA_NO='" + combLot.Text + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    totalqty = qury.Data.Count();
                    List<R107> datalist = new List<R107>();
                    datalist = qury.Data.ToListObject<R107>().ToList();
                    editMoNo.Text = datalist[0].MO_NUMBER;
                    editModel.Text = datalist[0].MODEL_NAME;

                    qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE (SHIPPING_SN='N/A' OR SHIPPING_SN IS NULL) and QA_NO='" + combLot.Text + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    TOTALQTY1 = qury.Data.Count();
                    if (TOTALQTY1 == 0)
                    {
                        if (await CHECKPQESSN(combLot.Text) == false)
                        {
                            return;
                        }
                    }
                    else if (TOTALQTY1 != 0 && TOTALQTY1 < totalqty)
                    {
                        MessageWindow mes = new MessageWindow(sfcClient, "00603");
                        mes.Owner = this;
                        mes.ShowDialog();
                        return;
                    }
                }

                if (itemFullSampling.IsChecked == false)
                {
                    await bbtnRefreshClick();
                }
                if (!string.IsNullOrEmpty(combLot.Text))
                {
                    INIFile SFIS = new INIFile("SFIS.ini");
                    if (string.IsNullOrEmpty(SFIS.Read("CQC", "CHECKPASSWORD")))
                        PassWD = false;
                    else
                        PassWD = true;
                    Pass = false;
                    sSQL = " select serial_number from SFISM4.R_WIP_TRACKING_T " +
                          " where  qa_no ='" + combLot.Text + "' and ((error_flag=1) or (error_flag=2) " +
                          "    or  ((next_station<>'N/A') and (next_station<>'" + sMyGroup + "')) )" +
                          "  and RowNum=1 ";
                    var quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (quryTemp.Data != null)
                    {
                        lablMsg.Content = quryTemp.Data["serial_number"].ToString() + "Units Still don''t Repair / Scrap or Next Group is not N/A !!";
                        List<R107> re_list = new List<R107>();
                        re_list.Add(new R107 { SERIAL_NUMBER = quryTemp.Data["serial_number"].ToString() });
                        ErrorListWindow formErrorList = new ErrorListWindow(re_list);
                        formErrorList.Owner = this;
                        formErrorList.ShowDialog();
                        return;
                    }
                    sSQL = "select serial_number from SFISM4.R_WIP_TRACKING_T " +
                          " where  qa_no ='" + combLot.Text + "' and group_name LIKE '%O%'  " +
                          "    AND ( qa_result <> '5') " +
                          "  and RowNum=1 ";
                    quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (quryTemp.Data != null)
                    {
                        lablMsg.Content = quryTemp.Data["serial_number"].ToString() + "check not enough keypart/CHECKCTN";
                        List<R107> re_list = new List<R107>();
                        re_list.Add(new R107 { SERIAL_NUMBER = quryTemp.Data["serial_number"].ToString() });
                        ErrorListWindow formErrorList = new ErrorListWindow(re_list);
                        formErrorList.Owner = this;
                        formErrorList.ShowDialog();
                        return;
                    }

                    //check route

                    //******­Y¬OCPeiii ¾÷ºØ¨ú®ø¦¹¬q¥N½XCheck¥\¯à ******add by huang-jun-wu 2010-10-27*******
                    if (CPEIIIFLAG == false)
                    {
                        sSQL = " select serial_number from SFISM4.R_WIP_TRACKING_T  " +
                                "  where qa_no=:lot_no  and wip_group NOT IN (:mygroup1,:mygroup2,:mygroup3,:mygroup4) ";
                        quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = sSQL,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name="lot_no", Value = combLot.Text },
                                new SfcParameter {Name="mygroup1", Value = "FQA" },
                                new SfcParameter {Name="mygroup2", Value = "F_FQA" },
                                new SfcParameter {Name="mygroup3", Value = "U_FQA" },
                                new SfcParameter {Name="mygroup4 ", Value = "OBA_BI" }
                            }
                        });
                        if (quryTemp.Data != null)
                        {
                            lablMsg.Foreground = Brushes.Red;
                            lablMsg.Content = quryTemp.Data["serial_number"].ToString() + "SERIAL NUMBER Group is not QA !!";
                            List<R107> re_list = new List<R107>();
                            re_list.Add(new R107 { SERIAL_NUMBER = quryTemp.Data["serial_number"].ToString() });
                            ErrorListWindow formErrorList = new ErrorListWindow(re_list);
                            formErrorList.Owner = this;
                            formErrorList.ShowDialog();
                            return;
                        }
                    }
                    //*******************************end **********************************************
                    //check route
                    //check pallet full flag=Y

                    sSQL = " select serial_number from SFISM4.R_WIP_TRACKING_T where qa_no=:lot_no ";

                    sSQL = sSQL + " AND ((Tray_NO  IN (Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag='Tray')) " +
                            " OR (Mcarton_NO  IN (Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag='Carton')) " +
                            " OR (IMEI  IN (Select Pallet_No From SFIS1.C_Pallet_T Where Close_Flag='Pallet')) " +
                            ") ";
                    sSQL = sSQL + " AND RowNum=1 ";
                    quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name="lot_no", Value = combLot.Text }
                            }
                    });
                    if (quryTemp.Data != null)
                    {
                        lablMsg.Foreground = Brushes.Red;
                        lablMsg.Content = "This Pallet/Carton is not complete!!";
                        MessageBox.Show("This Pallet/Carton is not complete!!");
                        return;
                    }
                    sSQL = " Select SERIAL_NUMBER Total from sfism4.r_wip_Tracking_t Where qa_No =:Qa_no ";
                    var quryTemp1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name="Qa_no", Value = combLot.Text }
                            }
                    });
                    iTotal = quryTemp1.Data.Count();
                    sSQL = " Select Serial_Number ,Model_name from sfism4.r_QC_SN_T Where Lot_no=:lot_No and Counter=0 ";
                    quryTemp1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name="lot_No", Value = combLot.Text }
                            }
                    });
                    if ((await ischeckfile()) || (N0401.IsChecked && N0651.IsChecked == false))
                    {
                        iCheck = Int32.Parse(editCheckQty.Text);
                    }
                    else
                    {
                        iCheck = iTotal / Int32.Parse(editCheckQty.Text);
                        if (iTotal % Int32.Parse(editCheckQty.Text) != 0)
                        {
                            iCheck = iCheck + 1;
                        }
                    }
                    if (iCheck > quryTemp1.Data.Count())
                    {
                        if (chkbCheckQty.IsChecked == true)
                        {
                            MessageBox.Show(await GetPubMessage("00588") + " " + (iCheck - quryTemp1.Data.Count()) + " PCS");
                            return;
                        }
                        else
                            MessageBox.Show(await GetPubMessage("00588") + " " + (iCheck - quryTemp1.Data.Count()) + " PCS");
                    }
                    if (PassWD)
                    {
                        //if (PasswordDLg(str) == true)
                        //{
                        //    if (str == G_sPassword || str == G_sDefaultPW)
                        //    {
                        //        Pass = true;
                        //    }
                        //}
                    }
                    else
                    {
                        Pass = true;
                    }

                    //2005/01/22 SN improvement
                    // Louis
                    // check all sn passed : check_flag = 'Y' sfism4.r_qc_sn_t

                    string SQLstr = " select * from  sfism4.r_qc_sn_t " +
                         "  where  lot_no = '" + combLot.Text + "' " +
                         "  and (check_flag is null or check_flag <> 'Y') ";
                    var tmpQry = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = SQLstr,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (itemIsCheckSSN.IsChecked == false)
                    {
                        if (tmpQry.Data != null)
                        {
                            MessageBox.Show("Not check SN-Box-Tray-Cartion yet!");
                            return;
                        }
                    }

                    //THUY  qty need  check  in 1 pallet match with  qty must check
                    var qrysetup = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "select * from sfis1.c_parameter_ini where  prg_name='CQC' and  vr_class='shownotice' and vr_item='cqacpe2' and vr_name='cpe2' and vr_value='FQA' and rownum=1",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qrysetup.Data.Count() > 0)
                    {
                        if (chksea.IsChecked == false && chkairhk.IsChecked == false && chkairau.IsChecked == false && chkair.IsChecked == false)
                        {
                            MessageBox.Show("Please chose the way export goods, SEA, AIR, AIR HK, AIR AUS");
                            return;
                        }
                        if (chkair.IsChecked == true)
                        {
                            SQLstr = "select round(qtymo / qtypallet) slpallet , qtymo  from (select  a.pallet_qty*a.carton_qty qtypallet ,b.target_Qty qtymo " +
                                " from sfis1.c_pack_param_t a, sfism4.r105 b where b.mo_number='" + editMoNo.Text + "'  and a.model_name=b.model_name and a.version_code ='AI')";
                            var qrycheckqty = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qrycheckqty.Data == null)
                            {
                                slpallet = 0;
                                mo_qty1 = 0;
                            }
                            else
                            {
                                slpallet = Int32.Parse(qrycheckqty.Data["slpallet"].ToString());
                                mo_qty1 = Int32.Parse(qrycheckqty.Data["qtymo"].ToString());
                            }
                            mo_needQty1 = getOOBASampleSize(mo_qty1);
                            if (slpallet == 0)
                            {
                                qtyneed1 = 0;
                            }
                            else
                            {
                                qtyneed = mo_needQty1 / slpallet;
                                if ((qtyneed - Convert.ToInt32(qtyneed)) >= 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed) + 1);
                                }
                                if ((qtyneed - Convert.ToInt32(qtyneed)) < 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed));
                                }
                            }
                            SQLstr = " select COUNT(*) qtyreal from  sfism4.r_qc_sn_t   where  lot_no = '" + combLot.Text + "'";
                            var qryscanreal = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            slpalletreal = Int32.Parse(qryscanreal.Data["qtyreal"].ToString());
                            if (slpalletreal != qtyneed1)
                            {
                                MessageBox.Show("AIR Mo number: " + editMoNo.Text + "  need check " + qtyneed1.ToString() + " pcs but really qty check only " + slpalletreal.ToString() + "pcs.Pls check again");
                                Form2Window Form2 = new Form2Window(this);
                                Form2.Owner = this;
                                Form2.ShowDialog();
                                if (!ispass)
                                    return;
                            }
                        }

                        if (chksea.IsChecked == true)
                        {
                            SQLstr = "select round(qtymo / qtypallet) slpallet , qtymo  from (select  a.pallet_qty*a.carton_qty qtypallet ,b.target_Qty qtymo " +
                                " from sfis1.c_pack_param_t a, sfism4.r105 b where b.mo_number='" + editMoNo.Text + "'  and a.model_name=b.model_name and a.version_code not in ('HK','AU','AI'))";
                            var qrycheckqty = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qrycheckqty.Data == null)
                            {
                                slpallet = 0;
                                mo_qty1 = 0;
                            }
                            else
                            {
                                slpallet = Int32.Parse(qrycheckqty.Data["slpallet"].ToString());
                                mo_qty1 = Int32.Parse(qrycheckqty.Data["qtymo"].ToString());
                            }
                            mo_needQty1 = getOOBASampleSize(mo_qty1);
                            if (slpallet == 0)
                            {
                                qtyneed1 = 0;
                            }
                            else
                            {
                                qtyneed = mo_needQty1 / slpallet;
                                if ((qtyneed - Convert.ToInt32(qtyneed)) >= 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed) + 1);
                                }
                                if ((qtyneed - Convert.ToInt32(qtyneed)) < 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed));
                                }
                            }
                            SQLstr = " select COUNT(*) qtyreal from  sfism4.r_qc_sn_t   where  lot_no = '" + combLot.Text + "'";
                            var qryscanreal = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            slpalletreal = Int32.Parse(qryscanreal.Data["qtyreal"].ToString());
                            if (slpalletreal != qtyneed1)
                            {
                                MessageBox.Show("SEA Mo number: " + editMoNo.Text + "  need check " + qtyneed1.ToString() + " pcs but really qty check only " + slpalletreal.ToString() + "pcs.Pls check again");
                                Form2Window Form2 = new Form2Window(this);
                                Form2.Owner = this;
                                Form2.ShowDialog();
                                if (!ispass)
                                    return;
                            }
                        }

                        if (chkairhk.IsChecked == true)
                        {
                            SQLstr = "select round(qtymo / qtypallet) slpallet , qtymo  from (select  a.pallet_qty*a.carton_qty qtypallet ,b.target_Qty qtymo " +
                                " from sfis1.c_pack_param_t a, sfism4.r105 b where b.mo_number='" + editMoNo.Text + "'  and a.model_name=b.model_name and a.version_code='HK')";
                            var qrycheckqty = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qrycheckqty.Data == null)
                            {
                                slpallet = 0;
                                mo_qty1 = 0;
                            }
                            else
                            {
                                slpallet = Int32.Parse(qrycheckqty.Data["slpallet"].ToString());
                                mo_qty1 = Int32.Parse(qrycheckqty.Data["qtymo"].ToString());
                            }
                            mo_needQty1 = getOOBASampleSize(mo_qty1);
                            if (slpallet == 0)
                            {
                                qtyneed1 = 0;
                            }
                            else
                            {
                                qtyneed = mo_needQty1 / slpallet;
                                if ((qtyneed - Convert.ToInt32(qtyneed)) >= 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed) + 1);
                                }
                                if ((qtyneed - Convert.ToInt32(qtyneed)) < 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed));
                                }
                            }
                            SQLstr = " select COUNT(*) qtyreal from  sfism4.r_qc_sn_t   where  lot_no = '" + combLot.Text + "'";
                            var qryscanreal = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            slpalletreal = Int32.Parse(qryscanreal.Data["qtyreal"].ToString());
                            if (slpalletreal != qtyneed1)
                            {
                                MessageBox.Show("AIR HONG KONG Mo number: " + editMoNo.Text + "  need check " + qtyneed1.ToString() + " pcs but really qty check only " + slpalletreal.ToString() + "pcs.Pls check again");
                                Form2Window Form2 = new Form2Window(this);
                                Form2.Owner = this;
                                Form2.ShowDialog();
                                if (!ispass)
                                    return;
                            }
                        }

                        if (chkairau.IsChecked == true)
                        {
                            SQLstr = "select round(qtymo / qtypallet) slpallet , qtymo  from (select  a.pallet_qty*a.carton_qty qtypallet ,b.target_Qty qtymo " +
                                " from sfis1.c_pack_param_t a, sfism4.r105 b where b.mo_number='" + editMoNo.Text + "'  and a.model_name=b.model_name and a.version_code='AU')";
                            var qrycheckqty = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qrycheckqty.Data == null)
                            {
                                slpallet = 0;
                                mo_qty1 = 0;
                            }
                            else
                            {
                                slpallet = Int32.Parse(qrycheckqty.Data["slpallet"].ToString());
                                mo_qty1 = Int32.Parse(qrycheckqty.Data["qtymo"].ToString());
                            }
                            mo_needQty1 = getOOBASampleSize(mo_qty1);
                            if (slpallet == 0)
                            {
                                qtyneed1 = 0;
                            }
                            else
                            {
                                qtyneed = mo_needQty1 / slpallet;
                                if ((qtyneed - Convert.ToInt32(qtyneed)) >= 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed) + 1);
                                }
                                if ((qtyneed - Convert.ToInt32(qtyneed)) < 0.1)
                                {
                                    qtyneed1 = Convert.ToInt32(Math.Truncate(qtyneed));
                                }
                            }
                            SQLstr = " select COUNT(*) qtyreal from  sfism4.r_qc_sn_t   where  lot_no = '" + combLot.Text + "'";
                            var qryscanreal = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = SQLstr,
                                SfcCommandType = SfcCommandType.Text
                            });
                            slpalletreal = Int32.Parse(qryscanreal.Data["qtyreal"].ToString());
                            if (slpalletreal != qtyneed1)
                            {
                                MessageBox.Show("AIR AUSTRALIA Mo number: " + editMoNo.Text + " need check " + qtyneed1.ToString() + " pcs but really qty check only " + slpalletreal.ToString() + "pcs.Pls check again");
                                Form2Window Form2 = new Form2Window(this);
                                Form2.Owner = this;
                                Form2.ShowDialog();
                                if (!ispass)
                                    return;
                            }
                        }
                    }
                    if (s_ModelType.IndexOf("170") > 0)
                    {
                        quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "SELECT DISTINCT A.SERIAL_NUMBER ,B.SHIPPING_SN SN,A.POSTITION_FLAG WEIGHT FROM SFISM4.R_SN_MOVE_T A " +
                                    "INNER JOIN SFISM4.R_QC_SN_T B ON A.SERIAL_NUMBER=B.SERIAL_NUMBER WHERE A.POSTITION_FLAG='0' AND B.LOT_no =:lot_no",
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter {Name="lot_no", Value = combLot.Text }
                            }
                        });
                        if (quryTemp.Data != null)
                        {
                            lablMsg.Foreground = Brushes.Red;
                            lablMsg.Content = quryTemp.Data["sn"].ToString() + " NO WEIGHT!!";
                            List<R107> re_list = new List<R107>();
                            re_list.Add(new R107 { SERIAL_NUMBER = quryTemp.Data["serial_number"].ToString() });
                            ErrorListWindow formErrorList = new ErrorListWindow(re_list);
                            formErrorList.Owner = this;
                            formErrorList.ShowDialog();
                            return;
                        }
                    }

                    if (Pass == true) //cuong
                    {
                        YesDlgWindow formYesDlg = new YesDlgWindow(this);
                        formYesDlg.Owner = this;
                        formYesDlg.ShowDialog();
                        if (MrOK)
                        {
                            var quryCount1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "SELECT serial_number FROM SFISM4.R_wip_tracking_T WHERE qa_no like :lot_no ",
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="lot_no", Value = combLot.Text }
                                }
                            });

                            //pass count
                            sSQL = " select serial_number from SFISM4.R_QC_SN_T " +
                                  " where LOT_NO = :qa_no and ERROR_FLAG=0 AND COUNTER=0 ";
                            if (chkbInsertQCSN == true)
                            {
                                sSQL = sSQL + " and Group_name='" + sMyGroup + "'";
                            }
                            quryTemp1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = sSQL,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="qa_no", Value = combLot.Text }
                                }
                            });
                            iPass = quryTemp1.Data.Count();
                            sSQL = " select serial_number from SFISM4.R_QC_SN_T where LOT_NO = :qa_no and ERROR_FLAG=1 AND COUNTER=0";
                            if (chkbInsertQCSN == true)
                            {
                                sSQL = sSQL + " and Group_name='" + sMyGroup + "' ";
                            }
                            quryTemp1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = sSQL,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="qa_no", Value = combLot.Text }
                                }
                            });
                            iFail = quryTemp1.Data.Count();
                            if (chkbWarehouseNO == true)
                            {
                                formWarehouseNOWindow formWarehouseNO = new formWarehouseNOWindow(this);
                                formWarehouseNO.Owner = this;
                                formWarehouseNO.ShowDialog();
                                if (string.IsNullOrEmpty(editWarehouseNO))
                                    return;
                            }
                            // cuong
                            var quryTocken = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME  ='CQC' AND VR_CLASS ='ACTIVE_TOKEN' AND VR_ITEM ='TRUE'",
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (quryTocken.Data.Count() != 0)
                            {
                                if (!await LoadTocken(combLot.Text))
                                {
                                    MessageWindow mes = new MessageWindow();
                                    mes.txbenglish.Text = " Loaded file token error !";
                                    mes.txbvietnamese.Text = "Loaded file token error!";
                                    mes.Owner = this;
                                    mes.ShowDialog();
                                    return;
                                }

                            }

                            await FindWorkSection(sMyLine);
                            if (sDay_Distinct == "TODAY")
                                sDay_Distinct = DateTime.Now.ToString("yyyyMMdd");
                            if (sDay_Distinct == "TOMORROW")
                                sDay_Distinct = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
                            if (sDay_Distinct == "YESTERDAY")
                                sDay_Distinct = DateTime.Now.AddDays(+1).ToString("yyyyMMdd");

                            sSQL = " update sfism4.r_cqc_rec_t " +
                                 " set  qa_result = '0' " +
                                 "    , lot_size = :lot_size " +
                                 "    , PASS_QTY = :pass " +
                                 "    , Fail_Qty = :Fail " +
                                 "    , end_time = sysdate " +
                                 "    , Class =:Class " +
                                 "    , Class_Date =:Class_Date ";
                            sSQL = sSQL + " ,PO_NUMBER=:PO_NUMBER ";
                            if (chkbWarehouseNO == true)
                            {
                                sSQL = sSQL + " ,WAREHOUSE_NO='" + editWarehouseNO + "' ";
                            }
                            sSQL = sSQL + " where lot_no = :lot_no ";
                            var quryP = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                            {
                                CommandText = sSQL,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="lot_no", Value = combLot.Text },
                                    new SfcParameter {Name="lot_size", Value = quryCount1.Data.Count() },
                                    new SfcParameter {Name="pass", Value = iPass },
                                    new SfcParameter {Name="Fail", Value = iFail },
                                    new SfcParameter {Name="Class", Value = sClass },
                                    new SfcParameter {Name="Class_Date", Value = sDay_Distinct },
                                    new SfcParameter {Name="PO_NUMBER", Value = ComboBox1.Text }
                                }
                            });
                            quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "SELECT B.END_GROUP End_Group FROM SFISM4.R_WIP_TRACKING_T A,SFISM4.R_MO_BASE_T B "
                                            + " WHERE A.MO_NUMBER=B.MO_NUMBER AND A.QA_NO=:QA_NO and B.END_GROUP=:MYGROUP",
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="QA_NO", Value = combLot.Text },
                                    new SfcParameter {Name="MYGROUP", Value = sMyGroup }
                                }
                            });
                            if (quryTemp.Data != null)
                            {
                                sWip_Group = quryTemp.Data["end_group"].ToString();
                            }
                            else
                            {
                                quryTemp = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = "SELECT B.GROUP_NEXT GROUP_NEXT FROM SFISM4.R_WIP_TRACKING_T A, SFIS1.C_ROUTE_CONTROL_T B "
                                              + "  WHERE  A.QA_NO=:QA_NO AND B.ROUTE_CODE=A.SPECIAL_ROUTE AND B.GROUP_NAME=A.GROUP_NAME AND A.GROUP_NAME=:MYGROUP "
                                              + "   AND ROWNUM=1 and B.STATE_FLAG ='0' ",
                                    SfcCommandType = SfcCommandType.Text,
                                    SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="QA_NO", Value = combLot.Text },
                                    new SfcParameter {Name="MYGROUP", Value = sMyGroup }
                                }
                                });
                                if (quryTemp.Data != null)
                                {
                                    sWip_Group = quryTemp.Data["group_next"].ToString();
                                }
                                else
                                {
                                    sWip_Group = sMyGroup;
                                }
                            }

                            sSQL = " update SFISM4.R_WIP_TRACKING_T  " +
                                 " set QA_RESULT = '0', " +
                                 "     SECTION_NAME = :section_name, " +
                                 "     GROUP_NAME = :group_name, " +
                                 "     STATION_NAME = :station_name, " +
                                 "     IN_STATION_TIME = sysdate, " +
                                 "     EMP_NO =:emp_no, " +
                                 "     NEXT_STATION = 'N/A', " +
                                 "     WIP_GROUP =:WIP_GROUP, ";
                            if (editPalletFullFlag == "Y")
                            {
                                sSQL = sSQL + "     PALLET_FULL_FLAG ='Y' ";
                            }
                            else
                            {
                                sSQL = sSQL + "     PALLET_FULL_FLAG ='N' ";
                            }
                            if (CheckSSN1.IsChecked)
                            {
                                sSQL = sSQL + " where  QA_NO = :QA_NO  AND ((SHIPPING_SN <> 'N/A') OR (SHIPPING_SN IS NOT NULL))" +
                                        " and (NEXT_STATION='N/A' or NEXT_STATION=:group_name) ";
                            }
                            else
                            {
                                sSQL = sSQL + " where  QA_NO = :QA_NO and (NEXT_STATION='N/A' or NEXT_STATION=:group_name) ";
                            }
                            var quryOK = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                            {
                                CommandText = sSQL,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter {Name="QA_NO", Value = combLot.Text },
                                    new SfcParameter {Name="section_name", Value = sMySection },
                                    new SfcParameter {Name="group_name", Value = sMyGroup },
                                    new SfcParameter {Name="station_name", Value = sMyStation },
                                    new SfcParameter {Name="emp_no", Value = G_sTester },
                                    new SfcParameter {Name="WIP_GROUP", Value = sWip_Group }
                                }
                            });

                            //update r105,r102
                            //update R105

                            await UpdateR105();

                            //update R102

                            /*
                            sSQL = "select R.serial_number,R.Line_name,R107.MO_NUMBER from SFISM4.R_QC_SN_T R,SFISM4.R_WIP_TRACKING_T R107 " +
                               "  where R.lot_no='" + combLot.Text + "' and counter='0' and R.error_flag='1' " +
                               "  and R107.Error_flag='0' and R.Serial_Number = R107.Serial_Number " +
                               "  AND R.lot_no=R107.Qa_no";
                            var query = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = sSQL,
                                SfcCommandType = SfcCommandType.Text
                            });

                            foreach (R107 r in query.Data.ToListObject<R107>().ToList())
                            {
                                if (await CheckR117(r.SERIAL_NUMBER) == 1)
                                {
                                    await ReinsertR102("1", r.LINE_NAME, r.MO_NUMBER, sMyGroup);
                                }
                                else
                                    await insertR102("1", r.MO_NUMBER, sMyGroup, r.LINE_NAME, r.SERIAL_NUMBER);
                            }

                            var query117 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "select serial_number,mo_number,line_name from SFISM4.R_WIP_TRACKING_T where qa_no='" + combLot.Text + "'",
                                SfcCommandType = SfcCommandType.Text
                            });
                            foreach (R107 r in query117.Data.ToListObject<R107>().ToList())
                            {
                                if (await CheckR117(r.SERIAL_NUMBER) == 1)
                                {
                                    await ReinsertR102("0", r.LINE_NAME, r.MO_NUMBER, sMyGroup);
                                }
                                else
                                    await insertR102("0", r.MO_NUMBER, sMyGroup, r.LINE_NAME, r.SERIAL_NUMBER);
                            }
                            */
                            var SqlR102_ER = new
                            {
                                TYPE = "UPDATE_R102_ERROR",
                                LOT_NO = combLot.Text,
                                MY_GROUP = sMyGroup,
                                MY_SECTION = sMySection
                            };

                            //Tranform it to Json object
                            string jsonR102_ER = JsonConvert.SerializeObject(SqlR102_ER).ToString();
                            var resultR102_ER = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = "SFIS1.CQC_EXECUTE",
                                SfcCommandType = SfcCommandType.StoredProcedure,
                                SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter{Name="DATA",Value=jsonR102_ER,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}

                                    }
                            });

                            //dynamic adsR102_ER = resultR102_ER.Data;
                            //string R102status_ER = adsR102_ER[0]["output"];

                            //if (R102status_ER.Substring(0, 2) != "OK")
                            //{
                            //    MessageBox.Show(R102status_ER.Substring(0, 2), "CQC");
                            //    return;
                            //}

                            var SqlR102 = new
                            {
                                TYPE = "UPDATE_R102",
                                LOT_NO = combLot.Text,
                                MY_GROUP = sMyGroup,
                                MY_SECTION = sMySection
                            };

                            //Tranform it to Json object
                            string jsonR102 = JsonConvert.SerializeObject(SqlR102).ToString();
                            var resultR102 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                            {
                                CommandText = "SFIS1.CQC_EXECUTE",
                                SfcCommandType = SfcCommandType.StoredProcedure,
                                SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter{Name="DATA",Value=jsonR102,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}

                                    }
                            });

                            //dynamic adsR102 = resultR102.Data;
                            //string R102status = adsR102[0]["output"];

                            //if (R102status.Substring(0, 2) != "OK")
                            //{
                            //    MessageBox.Show(R102status.Substring(0, 2), "CQC");
                            //    return;
                            //}

                            await insertR117();

                            sSQL = "Update SFISM4.R_WIP_TRACKING_T Set SECTION_FLAG='' ";
                            if (chkbUpdateQANoResult)
                                sSQL += " , QA_NO=''N/A'',QA_RESULT='N/A' ";
                            if (chkbClearCarton)
                                sSQL += " , Carton_NO='N/A' ";
                            sSQL += " Where QA_NO='" + combLot.Text + "' ";
                            quryOK = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                            {
                                CommandText = sSQL,
                                SfcCommandType = SfcCommandType.Text
                            });

                            // Insert system log

                            var _Result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                            {
                                CommandText = "SELECT DISTINCT IMEI   FROM SFISM4.R107 WHERE QA_NO ='" + combLot.Text + "' ",
                                SfcCommandType = SfcCommandType.Text

                            });
                            if (_Result.Data != null)
                            {
                                PALET_NO = _Result.Data["imei"].ToString();
                            }

                            var SaveLog = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                            {
                                CommandText = " Insert into SFISM4.R_SYSTEM_LOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC,TIME) "
                            + " Values ('" + empNo + "', 'CQC', 'PASS_FQA', 'QA_NO: " + combLot.Text + " ,IMEI: "+ PALET_NO + " ' , SYSDATE) "
                            });

                            dbgridSSN.ItemsSource = null; qurySerN = null; dbgridErrorCode.ItemsSource = null;
                            lablMsg.Content = combLot.Text + " has verified PASS.";
                            clstrgridInLot.ItemsSource = null;
                            clstrgridNotChecked.ItemsSource = null;
                            editCheckedQty.Content = "0";
                            editFailedQty.Content = "0";
                            editCount.Content = "0";
                            G_sLotNo = "";
                            editLimitQty.Content = "0";
                            G_sModelName = string.Empty;
                            G_sMO = string.Empty;
                            editMoNo.Text = "";
                            editModel.Text = "";
                            sPO = "";
                            sPOLine = "";
                            combDef.Text = "";
                            editPassRate.Content = "";
                            combLot.Text = "";
                            //combEmp.Text = "";
                            comb1.Items.Clear();
                            G_iComboItem = -1;
                            editCritical.Text = "0";
                            editMajor.Text = "0";
                            editMinor.Text = "0";
                            editSampleQty.Text = "0";
                            editOQAType.Text = "";
                            panelOOBA.Visibility = Visibility.Collapsed;
                            await displayCombLot();
                        }
                    }
                }
                combDef.Focus();
            }
        }

        async Task UpdateR105()
        {
            var query = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select distinct Mo_number,group_name, count(mo_number) count from SFISM4.R_WIP_TRACKING_T where qa_no= '" + combLot.Text + "' group by mo_number , group_name",
                SfcCommandType = SfcCommandType.Text
            });
            foreach (R107 item in query.Data.ToListObject<R107>().ToList())
            {
                var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = "SFIS1.UPDATE_R105",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="MO",Value=item.MO_NUMBER,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                            new SfcParameter{Name="GNAME",Value=item.GROUP_NAME, SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                            new SfcParameter {Name="C_QTY", Value=item.COUNT, SfcParameterDataType=SfcParameterDataType.Int32, SfcParameterDirection=SfcParameterDirection.Input }
                        }
                });
            }
        }

        public async Task<bool> LoadTocken(string _QA_no)
        {
            string TK_MODEL_NAME, sql_token;
            Double QTYA;
            var quryTocken = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT * FROM SFIS1.C_MODEL_NOMMS_T WHERE MODEL_NAME IN ( SELECT DISTINCT  MODEL_NAME FROM SFISM4.R107 WHERE QA_NO ='" + _QA_no + "') AND MODEL_DESC ='TOKEN' ",
                SfcCommandType = SfcCommandType.Text
            });

            if (quryTocken.Data.Count() > 0)
            {
                var _Result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "SELECT DISTINCT IMEI, COUNT(*) COUNT, MODEL_NAME  FROM SFISM4.R107 WHERE QA_NO ='" + _QA_no + "' GROUP BY IMEI, MODEL_NAME",
                    SfcCommandType = SfcCommandType.Text

                });
                if (_Result.Data != null)
                {
                    dynamic r = _Result.Data;
                    TK_MODEL_NAME = r["model_name"];
                    QTYA = r["count"];
                    PALET_NO = r["imei"];

                    string path = Directory.GetCurrentDirectory() + "\\NetgearShippingFile\\" + TK_MODEL_NAME + "\\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    path += "PTM_" + PALET_NO + "_TOKEN.txt";

                    sql_token = " SELECT A.SHIPPING_SN,B.MAC8 REQUEST_ID,B.MAC6 TOKEN_ID " +
                                " FROM SFISM4.R_WIP_TRACKING_T A, SFISM4.R_CUSTSN_T B" +
                                " WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER AND A.QA_NO= '" + _QA_no + "'";

                    var Result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql_token,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (Result.Data != null)
                    {
                        if (Result.Data.Count() != int.Parse(QTYA.ToString()))
                        {
                            MessageWindow mes = new MessageWindow();
                            mes.txbenglish.Text = "So dong khong trung voi so luong he thong!";
                            mes.txbvietnamese.Text = "So dong khong trung voi so luong he thong!";
                            mes.Owner = this;
                            mes.ShowDialog();
                            return false;
                        }
                        try
                        {
                            using (StreamWriter SW = new StreamWriter(path))
                            {
                                SW.WriteLine("SHIPPING_SN\tREQUEST_ID\tTOKEN_ID");
                                foreach (var row in Result.Data)
                                {
                                    SW.WriteLine(row["shipping_sn"] + "\t" + row["request_id"] + "\t" + row["token_id"]);
                                }
                            }
                            string path2 = Directory.GetCurrentDirectory();

                            path2 = path2.Substring(0, path2.LastIndexOf("\\")) + "\\testconnectArloCQC.exe";
                            //path2 = @"C:\CPEII\testconnectArloCQC.exe";
                            ProcessStartInfo pProcess = new ProcessStartInfo();
                            pProcess.FileName = path2;
                            pProcess.WorkingDirectory = System.IO.Path.GetDirectoryName(path2);
                            pProcess.Arguments = path;
                            Process pRoc = Process.Start(pProcess);
                            pRoc.WaitForExit();

                            var SaveLog = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                            {
                                CommandText = " Insert into SFISM4.R_SYSTEM_LOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC,TIME) "
                                    + " Values ('" + empNo + "', 'CQC', 'UPLOAD', '" + PALET_NO + "' , SYSDATE) "
                            });

                            /// Kiem tra system_log testconnectArloCQC.exe
                            var rSystemLog = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = "select * from SFISM4.R_SYSTEM_LOG_T  where prg_name  ='CQC' AND ACTION_TYPE ='" + PALET_NO + "' AND ACTION_DESC LIKE  '%other error: 0%' ",
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (rSystemLog.Data.Count() != 0)
                            {
                                var updateErpMo = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = " UPDATE SFISM4.R107 SET ERP_MO = 'ACTIVED' WHERE IMEI = '" + PALET_NO + "' "
                                });

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageWindow mes = new MessageWindow();
                            mes.txbenglish.Text = ex.Message.ToString();
                            mes.txbvietnamese.Text = ex.Message.ToString();
                            mes.Owner = this;
                            mes.ShowDialog();
                            return false;
                        }
                    }
                    else
                    {
                        MessageWindow mes = new MessageWindow();
                        mes.txbenglish.Text = "Loaded file token error !";
                        mes.txbvietnamese.Text = "Loaded file token error!";
                        mes.Owner = this;
                        mes.ShowDialog();
                        return false;
                    }
                }
                else
                {
                    MessageWindow mes = new MessageWindow();
                    mes.txbenglish.Text = " Not found data QA_NO : " + _QA_no + " IN R107 ";
                    mes.txbvietnamese.Text = " Not found data QA_NO : " + _QA_no + " IN R107 ";
                    mes.Owner = this;
                    mes.ShowDialog();
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        async Task insertR117()
        {
            var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
            {
                CommandText = "INSERT INTO SFISM4.R_SN_DETAIL_T select * from sfism4.r_wip_tracking_t where qa_no = '" + combLot.Text + "'",
                SfcCommandType = SfcCommandType.Text
            });
        }
        public async Task ReinsertR102(string _F_FLAG, string _LINE, string _MO, string _MYGROUP)
        {
            var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
            {
                CommandText = "SFIS1.REINSERT_R102",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="LINE",Value= _LINE,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="SECTION",Value=sMySection, SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter {Name="MYGROUP", Value=_MYGROUP, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MO",Value=_MO,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MO_DATE",Value=DateTime.Now.ToString("MMddyyyy"), SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter {Name="W_SECTION", Value=G_iWSection, SfcParameterDataType=SfcParameterDataType.Int32, SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter {Name="F_FLAG", Value=_F_FLAG, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                }
            });
        }
        public async Task insertR102(string _F_Flag, string _MO, string _Group, string _Line, string _SN)
        {
            var Update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
            {
                CommandText = "SFIS1.INSERT_R102",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="LINE",Value= _Line,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="SECTION",Value=sMySection, SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter {Name="MYGROUP", Value=_Group, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MO",Value=_MO,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MO_DATE",Value=DateTime.Now.ToString("MMddyyyy"), SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter {Name="W_SECTION", Value=G_iWSection, SfcParameterDataType=SfcParameterDataType.Int32, SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter {Name="F_FLAG", Value=_F_Flag, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                }
            });
            var Update1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
            {
                CommandText = "SFIS1.CACULATE_LEADTIME",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="LINE",Value= _Line,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="SECTION",Value=sMySection, SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input},
                    new SfcParameter {Name="MYGROUP", Value=_Group, SfcParameterDataType=SfcParameterDataType.Varchar2, SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="MO",Value=_MO,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="SN",Value=_SN, SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input}
                }
            });
        }

        async Task<int> CheckR117(string _sn)
        {
            var quryR117 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT SERIAL_NUMBER from SFISM4.R_SN_DETAIL_T WHERE SERIAL_NUMBER='" + _sn + "' AND GROUP_NAME ='" + sMyGroup + "'"
            });
            if (quryR117.Data.Count() > 0)
                return 1;
            return 0;
        }

        int getOOBASampleSize(int qty)
        {
            if (1 <= qty && qty <= 15)
                mo_needQty1 = qty;
            else if (16 <= qty && qty <= 280)
                mo_needQty1 = 20;
            else if (281 <= qty && qty <= 1200)
                mo_needQty1 = 47;
            else if (1201 <= qty && qty <= 3200)
                mo_needQty1 = 53;
            else if (qty % 3000 == 0)
            {
                mo_needQty1 = (qty / 3000) * 53;
            }
            else
            {
                int result1 = qty - ((qty / 3200) * 3200);
                if (1 <= result1 && result1 <= 15)
                    mo_needQty1 = result1 + ((qty / 3200) * 53);
                else if (16 <= result1 && result1 <= 280)
                    mo_needQty1 = 20 + ((qty / 3200) * 53);
                else if (281 <= result1 && result1 <= 1200)
                    mo_needQty1 = 47 + ((qty / 3200) * 53);
                else if (1201 <= result1 && result1 <= 3200)
                    mo_needQty1 = 53 + ((qty / 3200) * 53);
            }
            return mo_needQty1;
        }

        //bool PasswordDLg(string Value)
        //{
        //    formPasswdWindow FormPasswd = new formPasswdWindow();
        //    FormPasswd.ShowDialog();
        //    if (DialogResult != null && DialogResult.Value)
        //    {
        //        Value = FormPasswd.meditPassword.ToString();
        //        return true;
        //    }
        //    else
        //        return false;

        //}

        async Task<bool> CHECKPQESSN(string LOT_NO)
        {
            string tmpmodel, tmpver, tmpmotype, tmpmo, STRSQL;
            string strsql = "";
            strsql = "Select distinct mo_number,model_name,version_code from sfism4.r_wip_Tracking_t Where  qa_No='" + LOT_NO + "'";
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strsql,
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null && qury.Data.Count() > 0)
            {
                var a = qury.Data.ToListObject<R107>();
                List<R107> list = a.Cast<R107>().ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    tmpmodel = list[i].MODEL_NAME.ToString();
                    tmpver = list[i].VERSION_CODE.ToString();
                    tmpmo = list[i].MO_NUMBER.ToString();
                    var QUERY1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "select mo_type from sfism4.r_mo_base_t where mo_number='" + tmpmo + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (QUERY1.Data != null)
                        tmpmotype = QUERY1.Data["mo_type"].ToString();
                    else
                        tmpmotype = string.Empty;
                    STRSQL = "SELECT A.CUSTSN_PREFIX,B.MO_TYPE FROM SFIS1.C_CUSTSN_RULE_CHECK_T A ,SFISM4.R105 B WHERE A.MODEL_NAME=B.MODEL_NAME " +
                      " AND A.VERSION_CODE=B.VERSION_CODE AND A. CUSTSN_CODE='SSN1' AND A.MO_TYPE=B.MO_TYPE AND A.WAIT_CHECK='PQE' AND " +
                      " B.MO_NUMBER='" + list[i].MO_NUMBER.ToString() + "'";
                    QUERY1 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = STRSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (QUERY1.Data != null)
                    {
                        CHECKPQESSNPREFIX(QUERY1.Data["custsn_prefix"].ToString(), QUERY1.Data["mo_type"].ToString());
                    }
                    else
                    {
                        MessageBox.Show("PQE do not setup SSN prefix, MODEL:" + tmpmodel + " VERSION:" + tmpver + " MO_TYPE:" + tmpmotype);
                        return false;
                    }
                }
            }

            strsql = "SELECT SUBSTR (a.shipping_sn, 1, :LENGTH) AS PREFIX, A.MODEL_NAME,A.VERSION_CODE,B.MO_TYPE " +
                    " FROM sfism4.r_wip_tracking_t a, sfism4.r_mo_base_t b " +
                    " WHERE a.mo_number = b.mo_number " +
                    " AND a.qa_no = '" + LOT_NO + "'" +
                    " AND SUBSTR (a.shipping_sn, 1, :LENGTH) || UPPER(b.mo_type) NOT IN (";
            for (int i = 0; i < ssn_Stringlist.Count; i++)
            {
                if (i == 0)
                {
                    strsql = strsql + " '" + ssn_Stringlist[i].ToString() + "' ";
                }
                else
                {
                    strsql = strsql + "," + " '" + ssn_Stringlist[i].ToString() + "' "; ;
                }
            }
            strsql = strsql + "') AND ROWNUM = 1";
            var qury2 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strsql,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="LENGTH", Value = SSNLENGTH }
                }
            });
            if (qury2.Data != null)
            {
                MessageBox.Show(qury2.Data["model_name"].ToString()
                + " " + qury2.Data["version_code"].ToString()
                + " " + qury2.Data["mo_type"].ToString()
                + "  prefix: " + qury2.Data["prefix"].ToString()
                + "  error ");
                return false;
            }
            else
            {
                return true;
            }
        }

        void CHECKPQESSNPREFIX(string SSNPREFIX, string MO_TYPE)
        {
            string temp;
            temp = SSNPREFIX;
            int i = temp.IndexOf(",");
            if (i == 0)
            {
                ssn_Stringlist.Add(temp + MO_TYPE.ToUpper());
                SSNLENGTH = temp.Length;
            }
            else
            {
                while (i != 0)
                {
                    SSNLENGTH = temp.Substring(0, i - 1).Length;
                    ssn_Stringlist.Add(temp.Substring(0, i - 1) + MO_TYPE.ToUpper());
                    temp = temp.Substring(i, temp.Length - i);
                    i = temp.IndexOf(",");
                }
            }
        }

        async Task<bool> ISCHECKSNMACPREFIX()
        {
            string strsql = "";
            strsql = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='CHECK_SSN_MAC_PREFIX'";
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strsql,
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
            {
                if (qury.Data["checkflag"].ToString() == "Y")
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }

        }

        async Task<bool> checkModelFile(string _LOT_NO)
        {
            string MO_NUMBER, LINE_NAME, MODEL_NAME, VERSION_CODE, PASSDATETIME2, PASSDATETIME1;

            string strhour = DateTime.Now.ToString("HH");
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select MO_NUMBER,LINE_NAME,MODEL_NAME,VERSION_CODE from SFISM4.R_WIP_TRACKING_T where  QA_NO ='" + _LOT_NO + "' AND ROWNUM=1",
                SfcCommandType = SfcCommandType.Text
            });
            MO_NUMBER = qury.Data["mo_number"].ToString();
            LINE_NAME = qury.Data["line_name"].ToString();
            MODEL_NAME = qury.Data["model_name"].ToString();
            VERSION_CODE = qury.Data["version_code"].ToString();
            string STRTIME = DateTime.Now.ToString("HHmmss");
            if (int.Parse(STRTIME) < 80000) //neu nho hon 8h sang
            {
                PASSDATETIME2 = DateTime.Now.ToString("MM/dd/yyyy") + " " + "08:00:00";
                PASSDATETIME1 = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy") + " " + "20:00:00";
            }
            else if (int.Parse(STRTIME) > 200000) //neu lon hon 8h toi
            {
                PASSDATETIME1 = DateTime.Now.ToString("MM/dd/yyyy") + " " + "20:00:00";
                PASSDATETIME2 = DateTime.Now.AddDays(+1).ToString("MM/dd/yyyy") + " " + "08:00:00";
            }
            else  //neu lon hon 8h sang va nho hon 20h toi
            {
                PASSDATETIME1 = DateTime.Now.ToString("MM/dd/yyyy") + " " + "08:00:00";
                PASSDATETIME2 = DateTime.Now.AddDays(+1).ToString("MM/dd/yyyy") + " " + "20:00:00";
            }

            string STRSQL = "SELECT * FROM SFISM4.R_MODELFILE_CHECK_T WHERE " +
                           " FIRST_INSTATION_TIME > TO_DATE (:PASSDATETIME1, 'MM/DD/YYYY HH24:MI:SS')" +
                           " AND FIRST_INSTATION_TIME < TO_DATE (:PASSDATETIME2, 'MM/DD/YYYY HH24:MI:SS')" +
                           " AND LINE_NAME=:LINE_NAME AND PASS_FLAG IS NOT NULL AND  MO_NUMBER=:MO_NUMBER AND ROWNUM=1";
            var qurycheck = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = STRSQL,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="PASSDATETIME1", Value = PASSDATETIME1 },
                    new SfcParameter {Name="PASSDATETIME2", Value = PASSDATETIME2 },
                    new SfcParameter {Name="LINE_NAME", Value = LINE_NAME },
                    new SfcParameter {Name="LINE_NAME", Value = MO_NUMBER }
                }
            });
            if (qurycheck.Data != null)
            {
                if (qurycheck.Data["pass_flag"].ToString() != "2")
                {
                    MessageWindow mes = new MessageWindow();
                    mes.txbenglish.Text = "IPQC not click pass!";
                    mes.txbvietnamese.Text = "IPQC chưa check pass!";
                    mes.Owner = this;
                    mes.ShowDialog();
                    return false;
                }
            }
            else
            {
                var check = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from sfism4.r_modelfile_check_t where  LINE_NAME='" + LINE_NAME + "' AND MO_NUMBER='" + MO_NUMBER + "' and pass_flag='2'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (check.Data == null)
                {
                    STRSQL = "INSERT INTO SFISM4.R_MODELFILE_CHECK_T(MO_NUMBER, MODEL_NAME, VERSION_CODE, LINE_NAME, PASS_FLAG) " +
                             " VALUES('" + MO_NUMBER + "', '" + MODEL_NAME + "','" + VERSION_CODE + "' ,'" + LINE_NAME + "','0') ";
                    var SQL_insert = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = STRSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    MessageWindow mes = new MessageWindow(sfcClient, "00591");
                    mes.Owner = this;
                    mes.ShowDialog();
                    return false;
                }
            }
            return true;
        }

        async Task<bool> CHECKMOONLINEVER(string _MODELNAME)
        {
            string G_VERSION_SHOW = "";
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT DISTINCT VERSION_CODE FROM SFISM4.R_MO_BASE_T WHERE CLOSE_FLAG='2' AND  MODEL_NAME = '" + _MODELNAME + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data.Count() > 1)
            {
                List<R107> verlist = new List<R107>();
                verlist = qury.Data.ToListObject<R107>().ToList();
                foreach (R107 items in verlist)
                {
                    G_VERSION_SHOW += items.VERSION_CODE;
                }
                return false;
            }
            return true;
        }
        async Task GETCPEIIIFLAG()
        {
            var sql = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "Select * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='CQC' AND VR_VALUE='NOCHECK'",
                SfcCommandType = SfcCommandType.Text
            });
            if (sql.Data.Count() > 0)
                NOcheckcontrolrun = false;
            else
                NOcheckcontrolrun = true;
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "Select Model_Serial From SFIS1.C_Model_Desc_T where model_name='SFISSITE' and model_serial = 'N'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                CPEIIIFLAG = true;
            else
                CPEIIIFLAG = false;

        }

        private async void mainWindow_Initialized(object sender, EventArgs e)
        {

            string[] Args = Environment.GetCommandLineArgs();

            if (Args.Length == 1)
            {
                //MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                //Environment.Exit(0);
            }
            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }
            string[] argsInfor = Regex.Split(inputLogin, @";");
            checkSum = argsInfor[0].ToString();
            loginApiUri = argsInfor[1].ToString();
            loginDB = argsInfor[2].ToString();
            empNo = argsInfor[3].ToString();
            empPass = argsInfor[4].ToString();

            G_sTester = empNo;

            //if (checkSum != GetChecksum(HashingAlgoTypes.SHA256, System.Reflection.Assembly.GetExecutingAssembly().Location))
            //{
            //    MessageBox.Show("THE CHECKSUM OF FILE NOT MATCH WITH SYSTEM", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            //    Environment.Exit(0);
            //}


            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "CQC",
                UserName = empNo,
                Password = empPass
            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            sfcClient = DBApi.sfcClient(loginDB, loginApiUri);
            try
            {
                await sfcClient.GetAccessTokenAsync(empNo, empPass);

                var result1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                    new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}

                }

                });

                dynamic ads = result1.Data;
                string Ok = ads[0]["output"];

                if (Ok.Substring(0, 2) == "OK")
                {

                    loginInfor = Ok.Substring(3, Ok.Length - 3).Trim();
                    string[] digits = Regex.Split(loginInfor, @";");
                    txt_Database.Text = digits[0].ToString();
                    txt_loginInfor.Text = digits[1].ToString();
                    txtVerAp.Text += getRunningVersion().ToString();
                    combEmp.Text = digits[1].Substring(10, digits[1].ToString().Length - 10).ToString();
                }
                else
                {
                    MessageBox.Show(Ok, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                this.Close();
            }

            //create form delphi
            string sCompany = ini.Read("CQC", "Company");
            if (String.IsNullOrEmpty(sCompany))
            {
                CompanyWindow formSetupCompany = new CompanyWindow();
                formSetupCompany.txbCompany.Focus();
                formSetupCompany.Owner = this;
                formSetupCompany.ShowDialog();
                sCompany = formSetupCompany.txbCompany.Text.ToString().ToUpper();
                ini.Write("CQC", "Company", sCompany);
            }
            if (N0651.IsChecked || N0401.IsChecked)
            {
                editCheckQty.Text = "0";
            }
            //create form delphi

            combDef.Focus();
            int iFlag, iLine, i;
            bLogin = false;
            G_sType = "";
            G_sLocation = "";
            G_sReasonCode = "";
            G_sDutycode = "";

            G_Class = "";
            G_Class_Date = "";
            G_Work_Section = 0;

            iChangeLine = 0;
            if (string.IsNullOrEmpty(ini.Read("CQC", "Station")))
            {
                ConfigDlgWindow formConfigDlg = new ConfigDlgWindow(this);
                formConfigDlg.Owner = this;
                formConfigDlg.ShowDialog();
            }
            //smysection
            sMySection = ini.Read("CQC", "Section");
            //mygroup
            sMyGroup = ini.Read("CQC", "Group");
            //station
            sMyStation = ini.Read("CQC", "Station");

            //siinline
            if (!string.IsNullOrEmpty(ini.Read("CQC", "Line Count")) && ini.Read("CQC", "Line Count") != "0")
            {
                sIinLine = "Single";
            }
            else
                sIinLine = "All";

            if (string.IsNullOrEmpty(ini.Read("CQC", "Line Count")))
            {
                iLine = 0;
            }
            else
                iLine = int.Parse(ini.Read("CQC", "Line Count"));

            listSelectLine.Items.Clear();

            string tmpS = "";
            for (i = 1; i <= iLine; i++)
            {
                tmpS = ini.Read("CQC", "Line" + i.ToString());
                if (!string.IsNullOrEmpty(tmpS))
                {
                    listSelectLine.Items.Add(tmpS);
                }
            }

            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' AND VR_ITEM='Line_Single'",
                SfcCommandType = SfcCommandType.Text
            });
            stationlist = result.Data.ToListObject<PARAMETER>().ToList();
            iLine = 0;
            if (sIinLine == "Single" && result.Data.Count() != 0)
            {
                foreach (string Item in listSelectLine.Items)
                {
                    if (stationlist.Count(c => c.VR_NAME == Item) != 0)
                    {
                        if (stationlist.Count(c => c.VR_NAME == sMyGroup) != 0)
                        {
                            iLine = 1;
                            break;
                        }
                    }
                }
            }
            if (sIinLine != "ALL")
            {
                if (iLine != 1)
                {
                    SetupLineWindow formSetupLine = new SetupLineWindow(this);
                    formSetupLine.btnOK_Click(new object(), new RoutedEventArgs());
                    sIinLine = "single";
                    itemLineAll.IsChecked = false;
                    itemLineSingle.IsChecked = true;
                    itemLineAll.IsEnabled = false;
                    itemLineSingle.IsEnabled = false;
                    itemChangeLine.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show(sIinLine + " is Using by Other PC!!");
                    SetupLineWindow formSetupLine = new SetupLineWindow(this);
                    formSetupLine.Owner = this;
                    formSetupLine.ShowDialog();
                    itemLineSingle.IsChecked = true;
                    itemLineAll.IsEnabled = true;
                    itemLineSingle.IsEnabled = false;
                    itemChangeLine.IsEnabled = true;
                }
            }
            else
            {
                if (await checkLineAll() == true)
                {
                    if (stationlist.Count(c => c.VR_CLASS != GetIPAddress().ToString()) != 0)
                    {
                        sIinLine = "All";
                        MessageBox.Show("Other Line Using CQC AP");
                        ConfigDlgWindow formConfigDlg = new ConfigDlgWindow(this);
                        formConfigDlg.Owner = this;
                        formConfigDlg.ShowDialog();
                        itemLineAll.IsEnabled = false;
                        itemLineSingle.IsEnabled = false;
                        itemChangeLine.IsEnabled = true;
                    }
                }

                if (await checkOtherLine() == true)
                {
                    itemLineSingle.IsChecked = true;
                    itemLineAll.IsChecked = false;
                    itemLineAll.IsEnabled = false;
                    itemLineSingle.IsEnabled = false;
                    itemChangeLine.IsEnabled = true;
                    SetupLineWindow formSetupLine = new SetupLineWindow(this);
                    formSetupLine.Owner = this;
                    formSetupLine.ShowDialog();
                }
                else
                {
                    itemLineAll.IsChecked = true;
                    itemChangeLine.IsChecked = false;
                    itemChangeLine.IsEnabled = false;
                    itemLineAll.IsEnabled = false;
                    await InsPARAMETER();
                }
            }

            //CQC Unit
            editCQCUnit = ini.Read("CQC", "CQC Unit");

            if (editCQCUnit != "")
            {
                itemCQCUnit.IsEnabled = false;
            }
            else
                itemCQCUnit.IsEnabled = true;

            // CQC BY
            iFlag = 0;
            if (iFlag == 0 && ini.Read("CQC", "CQC BY") == "SSN")
            {
                itemSSN.IsChecked = true;
                itemTray.IsChecked = false;
                itemCarton.IsChecked = false;
                itemPallet.IsChecked = false;
                iFlag = 1;
                itemCQCby.Header = "CQC By - S";
            }
            if (iFlag == 0 && ini.Read("CQC", "CQC BY") == "TRAY")
            {
                itemTray.IsChecked = true;
                itemSSN.IsChecked = false;
                itemCarton.IsChecked = false;
                itemPallet.IsChecked = false;
                iFlag = 1;
                itemCQCby.Header = "CQC By - T";
            }
            if (iFlag == 0 && ini.Read("CQC", "CQC BY") == "Carton")
            {
                itemSSN.IsChecked = false;
                itemTray.IsChecked = false;
                itemCarton.IsChecked = true;
                itemPallet.IsChecked = false;
                itemCQCby.Header = "CQC By - C";
                itemUnitCount.Header = "? Carton / 1 LOT";
                lablUnitCount = "Carton Count =";
                iFlag = 1;
            }
            if (iFlag == 0 && ini.Read("CQC", "CQC BY") == "Pallet")
            {
                itemSSN.IsChecked = false;
                itemTray.IsChecked = false;
                itemCarton.IsChecked = false;
                itemPallet.IsChecked = true;
                itemCQCby.Header = "CQC By - P";
                itemUnitCount.Header = "? Pallet / 1 LOT";
                lablUnitCount = "Pallet Count =";
                iFlag = 1;
            }
            if (iFlag == 0)
            {
                itemSSN.IsChecked = false;
                itemTray.IsChecked = false;
                itemCarton.IsChecked = false;
                itemPallet.IsChecked = false;
                iFlag = 1;
                itemCQCby.IsEnabled = true;
            }

            //Sampling Unit
            iFlag = 0;
            if (ini.Read("CQC", "Sampling Unit") == "SN")
            {
                itemSamplingSN.IsChecked = true;
                itemSamplingSSN.IsChecked = false;
                itemSSN.Header = "Serial Number";
                itemSampleUnit.Header = itemSampleUnit.Header + "- SN";
                iFlag = 1;
                itemSSNLength.Header = "SN Length";
                lablInputMsg.Text = "OK->SN,NG->EC+SN/RC+Duty+Loc+SN";
                if (itemSSN.IsChecked)
                {
                    itemUnitCount.Header = "? SN / 1 LOT";
                    lablUnitCount = "SN Count =";
                }
            }


            if (iFlag == 0 && ini.Read("CQC", "Sampling Unit") == "SSN")
            {
                itemSamplingSN.IsChecked = false;
                itemSamplingSSN.IsChecked = true;
                itemSSN.Header = "Shipping SN";
                //   lablInputMsg.Caption:='OK->SSN  ,  NG->EC+SSN / RC+LOC+SSN';
                lablInputMsg.Text = "OK->SSN,NG->EC+SSN/RC+Duty+Loc+SSN";
                itemSampleUnit.Header = itemSampleUnit.Header + "- SSN";
                //   itemSampleUnit.Enabled:=false;
                iFlag = 1;
                if (itemSSN.IsChecked)
                {
                    itemUnitCount.Header = "? SSN / 1 LOT";
                    lablUnitCount = "SSN Count =";
                }
            }


            if (iFlag == 0)
            {
                itemSamplingSN.IsChecked = false;
                itemSamplingSSN.IsChecked = false;
                itemSSN.Header = "???";
                itemSampleUnit.IsEnabled = true;
            }
            //Check Method
            if (ini.Read("CQC", "Check Carton") == "Y")
            {
                itemCheckCarton.IsChecked = true;
            }
            if (ini.Read("CQC", "Check Pallet") == "Y")
            {
                itemCheckPallet.IsChecked = true;
            }
            if (ini.Read("CQC", "Check Tray") == "Y")
            {
                itemCheckTray.IsChecked = true;
            }

            //PO NO + PO LINE
            if (ini.Read("CQC", "BY PO NO+PO Line") == "Y")
                chkbPO = true;
            else
                chkbPO = false;

            //MO/Model
            if (ini.Read("CQC", "MO/Model") == "MO")
            {
                itemMO.IsChecked = true;
                itemMOModel.Header = "*MO/Model";
            }
            if (ini.Read("CQC", "MO/Model") == "Model")
            {
                itemModel.IsChecked = true;
                itemMOModel.Header = "MO/*Model";
            }
            if (ini.Read("CQC", "MO/Model") == "PO" && chkbPO == true)
            {
                itemPO.IsChecked = true;
                itemMOModel.Header = "*PO/Model";
            }
            editPalletFullFlag = ini.Read("CQC", "Pallet Full Flag");
            //check route
            if (ini.Read("CQC", "Check Route") == "Y")
                chkbCheckRoute = true;
            else
                chkbCheckRoute = true;

            //full Samplging
            if (ini.Read("CQC", "Full Sampling") == "Y")

                itemFullSampling.IsChecked = true;
            else
                itemFullSampling.IsChecked = false;

            //company
            editCompany = sCompany;
            //Insert group,section,station
            if (ini.Read("CQC", "Insert Sec&Grp") == "Y")
                chkbInsertQCSN = true;
            else
                chkbInsertQCSN = true;

            //reject reason
            if (ini.Read("CQC", "Reject Reason") == "N")
                chkbRejectReason = false;
            else
                chkbRejectReason = true;

            //Update R107 QA_NO,QA_RESULT='N/A'
            if (ini.Read("CQC", "Update R107 QA_NO&Result") == "Y")
                chkbUpdateQANoResult = true;
            else
                chkbUpdateQANoResult = false;

            //Remove SSN
            if (ini.Read("CQC", "Remove Fail SSN/SN") == "Y")
                chkbRemoveFailSSN = true;
            else
                chkbRemoveFailSSN = false;

            //Insert new SSN to Pallet
            if (ini.Read("CQC", "Insert New SN to Pallet / Carton") == "Y")
                chkbInsertSNtoPallet = true;
            else
                chkbInsertSNtoPallet = false;

            //Transfer by piece
            if (ini.Read("CQC", "Transfer by Piece") == "Y")
                chkbTransferbyPiece = true;
            else
                chkbTransferbyPiece = false;

            if (ini.Read("CQC", "Show QC Left Data") == "Y")
                itemLeft.IsChecked = true;
            else
                itemLeft.IsChecked = false;

            if (ini.Read("CQC", "Show QC Right Data") == "Y")
                itemRight.IsChecked = true;
            else
                itemRight.IsChecked = false;

            //Show Lot NO
            if (ini.Read("CQC", "Enabled Lot No") == "Y")
            {
                itemEnabledLotNO.IsChecked = true;
                combLot.IsEnabled = true;
            }
            else
            {
                itemEnabledLotNO.IsChecked = false;
                combLot.IsEnabled = false;
            }


            //ADVN Warehouse NO
            if (ini.Read("CQC", "Warehouse NO") == "Y")
                chkbWarehouseNO = true;
            else
                chkbWarehouseNO = false;

            if (chkbPO)
                itemPO.Visibility = Visibility.Visible;
            else
                itemPO.Visibility = Visibility.Collapsed;

            // clear pallet no
            if (ini.Read("CQC", "CLEAR PALLET NO") == "Y")
                chkbClearPallet = true;
            else
                chkbClearPallet = false;

            // clear carton no
            if (ini.Read("CQC", "CLEAR CARTON NO") == "Y")
                chkbClearCarton = true;
            else
                chkbClearCarton = false;

            //auto sampling modify by Tim
            if (ini.Read("CQC", "Auto Sampling") == "Y")
                AutoSampling.IsChecked = true;
            else
                AutoSampling.IsChecked = false;

            //show reject lot modify by Tim
            if (ini.Read("CQC", "Show Reject Lot") == "Y")
                ShowRejectLot.IsChecked = true;
            else
                ShowRejectLot.IsChecked = false;

            if (itemLineAll.IsChecked)
            {
                PanelTitle.Content = sMyGroup;
            }
            else
            {
                PanelTitle.Content = "";
                for (int j = 0; j <= listSelectLine.Items.Count - 1; j++)
                {
                    if (j != listSelectLine.Items.Count - 1)
                        PanelTitle.Content += listSelectLine.Items[j].ToString() + ",";
                    else
                        PanelTitle.Content += listSelectLine.Items[j].ToString() + " ";
                }
                PanelTitle.Content += sMyGroup;
            }
            editUnitCount = ini.Read("CQC", "Unit Count");

            if (ini.Read("CQC", "OQA SAMPLING PLAN") == "Y")
            {
                chkbSamplingPlan = true;
                panelSamplingPlan.Visibility = Visibility.Visible;
            }
            else
            {
                chkbSamplingPlan = false;
                panelSamplingPlan.Visibility = Visibility.Collapsed;

            }

            //check qty
            if (ini.Read("CQC", "Check Qty") == "T")
                chkbCheckQty.IsChecked = true;
            else
                chkbCheckQty.IsChecked = false;

            if (string.IsNullOrEmpty(ini.Read("CQC", "Check (1)Qty/(N)Qty")))
            {
                editCheckQty.Text = "1";
            }
            else
                editCheckQty.Text = ini.Read("CQC", "Check (1)Qty/(N)Qty");

            editConFlag.Text = ini.Read("CQC", "Condition Flag");


            G_iCritical = 0;
            G_iMajor = 0;
            G_iMinor = 0;
            if (itemLineSingle.IsChecked)
                itemChangeLine.IsEnabled = true;
            else
                itemChangeLine.IsEnabled = false;

            G_sLotNo = "";

            //display comblot
            await displayCombLot();

            G_iComboItem = -1;
            G_sPalletNo = "";
            bttnPass.IsEnabled = true;
            bbtnReject.IsEnabled = true;
            if (combLot.IsEnabled == true)
                combLot.Focus();


            lablMsg.Foreground = Brushes.Blue;
            //displayTitle
            displayTitle(ini.Read("CQC", "CQC Unit"));
            editMoNo.Text = "";
            editModel.Text = "";
            sPO = "";
            sPOLine = "";

            G_sRL = "Right";
            //editCount.Content = "0";


            if (await ischeckfile())
                N0651.IsChecked = true;

            if (N0651.IsChecked || N0401.IsChecked)
                editCheckQty.Text = "1";
            combDef.Focus();
        }

        async Task<bool> Input_Password(string emp_pass)
        {
            string str_emp_pass = "";
            var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "select class_name from sfis1.c_emp_desc_t where emp_pass ='" + emp_pass + "'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data != null)
                str_emp_pass = qury.Data["class_name"].ToString();
            if (str_emp_pass.Contains("6"))
                return true;
            return false;
        }
        async Task<bool> checkctnsn(string sn, string mctn)
        {
            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from sfism4.r107 where (serial_number=:sn or Tray_no=:sn) and mcarton_No=:ctn",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter { Name="sn", Value=sn },
                    new SfcParameter { Name="ctn", Value=mctn }
                }
            });
            if (result.Data.Count() != 0)
                return true;
            return false;
        }

        public async Task<bool> Ischeckfinishgood(string smodel_name)
        {
            var querymodel = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from sfis1.c_model_desc_t where model_name = '" + smodel_name + "' and model_type like '%Y%'",
                SfcCommandType = SfcCommandType.Text
            });
            if (querymodel.Data != null && querymodel.Data.Count() > 0)
                return true;
            return false;
        }
        public async Task<bool> ischeckfile()
        {
            var qryfilepass = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='CHECK_FILE' and CHECKFLAG='Y'",
                SfcCommandType = SfcCommandType.Text
            });
            if (qryfilepass.Data.Count() > 0)
            {
                return true;
            }
            else
                return false;
        }
        void displayTitle(string _Title)
        {
            int iFlag = 0;
            if (_Title != "")
            {
                grpbPalletNoNotChecked.Content = _Title + " not been checked ";
                grpbPalletNoInLot.Content = _Title + " in this lot ";
                been_check1.Header = _Title;
                this_lot1.Header = _Title;
                iFlag = 1;
            }
            if (iFlag == 0 && itemSSN.IsChecked)
            {
                grpbPalletNoNotChecked.Content = " Shipping SN not been checked ";
                grpbPalletNoInLot.Content = " Shipping SN in this lot ";
                iFlag = 1;
            }
            if (iFlag == 0 && itemTray.IsChecked)
            {
                grpbPalletNoNotChecked.Content = " Tray No not been checked ";
                grpbPalletNoInLot.Content = " Tray No in this lot ";
                iFlag = 1;
            }
            if (iFlag == 0 && itemCarton.IsChecked)
            {
                grpbPalletNoNotChecked.Content = " Carton No not been checked ";
                grpbPalletNoInLot.Content = " Carton No in this lot ";
                iFlag = 1;
            }
            if (iFlag == 0 && itemPallet.IsChecked)
            {
                grpbPalletNoNotChecked.Content = " Pallet No not been checked ";
                grpbPalletNoInLot.Content = " Pallet No in this lot ";
                iFlag = 1;
            }

            if (iFlag == 0)
            {
                grpbPalletNoNotChecked.Content = " ??? not been checked ";
                grpbPalletNoInLot.Content = " ??? in this lot ";
                iFlag = 1;
            }
            if (_Title == "")
            {
                if (itemSSN.IsChecked)
                {
                    if (itemSamplingSN.IsChecked)
                    {
                        grpbPalletNoNotChecked.Content = " Serial Number not been checked ";
                        grpbPalletNoInLot.Content = " Serial Number in this lot ";
                        been_check1.Header = "Serial Number";
                        this_lot1.Header = "Serial Number";
                    }
                    if (itemSamplingSSN.IsChecked)
                    {
                        grpbPalletNoNotChecked.Content = " Shipping SN not been checked ";
                        grpbPalletNoInLot.Content = " Shipping SN in this lot ";
                        been_check1.Header = " Shipping SN ";
                        this_lot1.Header = " Shipping SN ";
                    }
                    if (itemSamplingSN.IsChecked == false && itemSamplingSSN.IsChecked == false)
                    {
                        grpbPalletNoNotChecked.Content = " ??? not been checked ";
                        grpbPalletNoInLot.Content = " ??? in this lot ";
                        been_check1.Header = "??? ";
                        this_lot1.Header = "???";
                    }
                }
                if (itemTray.IsChecked)
                {
                    been_check1.Header = "Tray NO";
                    this_lot1.Header = "Tray NO";
                }
                if (itemCarton.IsChecked)
                {
                    been_check1.Header = "Carton NO";
                    this_lot1.Header = "Carton NO";
                }
                if (itemPallet.IsChecked)
                {
                    been_check1.Header = "Pallet NO";
                    this_lot1.Header = "Pallet NO";
                }
                if (itemPallet.IsChecked == false && itemCarton.IsChecked == false && itemTray.IsChecked == false && itemSSN.IsChecked == false)
                {
                    been_check1.Header = "???";
                    this_lot1.Header = "???";
                }
            }


            //Sampling Unit
            iFlag = 0;
            if (itemSamplingSN.IsChecked)
            {
                itemSSN.Header = "Serial Number";
                grpbInputEcSsn.Content = " Input EC / SN ";
                grpbSSNchecked.Content = " Serial Number been checked ";
                iFlag = 1;
            }
            if (iFlag == 0 && itemSamplingSSN.IsChecked)
            {
                itemSSN.Header = "Shipping SN";
                grpbInputEcSsn.Content = " Input EC / S-SN ";
                grpbSSNchecked.Content = " Shipping SN been checked ";
                iFlag = 1;
            }
            if (iFlag == 0)
            {
                itemSSN.Header = "???";
                grpbInputEcSsn.Content = " Input EC / ??? ";
                grpbSSNchecked.Content = " ??? been checked ";
            }

            if (itemSamplingSSN.IsChecked)
                lablMsg.Content = "Please input Error code or Shipping S/N.";
            else if (itemSamplingSN.IsChecked)
                lablMsg.Content = "Please input Error code or Serial Number.";
            else
                lablMsg.Content = "Please input Error code or ???.";
        }

        async Task displayCombLot()
        {
            string SQL = "";
            if (ShowRejectLot.IsChecked)
            {
                SQL = "SELECT lot_no " +
                      "FROM sfism4.r_cqc_rec_t  " +
                      "WHERE START_TIME>SYSDATE-90 AND (qa_result = 'N/A' OR qa_result = '1') ";
            }
            else
            {
                SQL = "SELECT lot_no " +
           " FROM sfism4.r_cqc_rec_t " +
           " WHERE START_TIME>SYSDATE-90 AND qa_result = 'N/A'  ";
            }
            if (itemLineSingle.IsChecked && listSelectLine.Items.Count == 1)
            {
                SQL += "and line_name= '" + listSelectLine.Items[0].ToString() + "'";
            }
            else if (itemLineSingle.IsChecked && listSelectLine.Items.Count > 1)
            {
                SQL += "and line_name in ('" + listSelectLine.Items[0].ToString() + "'";
                for (int i = 1; i <= listSelectLine.Items.Count - 1; i++)
                {
                    SQL += "," + "'" + listSelectLine.Items[i].ToString() + "'";
                }
                SQL += " ) ";
            }
            if (chkbInsertQCSN == true)
            {
                SQL += " and group_name='FQA' ";
            }
            SQL += " order by start_time DESC ";

            var querylot = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = SQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (querylot.Data.Count() > 0)
            {
                combLot.Items.Clear();
                foreach (CQCREC items in querylot.Data.ToListObject<CQCREC>().ToList())
                {
                    combLot.Items.Add(items.LOT_NO);
                }
            }
            combLot.Items.Add("");
            combLot.IsEnabled = true;
        }
        public async Task InsPARAMETER()
        {
            List<PARAMETER> stationlist1 = new List<PARAMETER>();
            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' AND VR_ITEM='Line_All' AND VR_VALUE=:MyGroup ",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter {Name = "MyGroup" , Value=sMyGroup }
                        }
            });
            stationlist1 = result.Data.ToListObject<PARAMETER>().ToList();
            if (result.Data.Count() == 0)
            {
                var sql = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "insert into SFIS1.C_PARAMETER_INI (prg_name, vr_class, vr_item, vr_name, vr_value) values (:prg_name, :vr_class, :vr_item, :vr_name, :vr_value)",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter {Name = "prg_name", Value="CQC" },
                            new SfcParameter {Name = "vr_class" , Value=GetIPAddress().ToString()},
                            new SfcParameter {Name = "vr_item", Value="Line_All" },
                            new SfcParameter {Name = "vr_name", Value="All" },
                            new SfcParameter {Name = "vr_value", Value=sMyGroup }
                        }
                });
            }
            else
            {
                if (stationlist1.Count(c => c.VR_CLASS != GetIPAddress().ToString()) != 0)
                    MessageBox.Show("System Error");
            }
        }
        public async Task<bool> checkOtherLine()
        {
            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select* from SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' AND VR_ITEM='Line_Single' ",
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data.Count() != 0)
                return true;
            else
                return false;
        }
        public async Task<bool> checkLineAll()
        {
            var sqlCheckLineAll = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = "select * from SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' AND VR_ITEM='Line_All' and VR_VALUE=:sMyGroup",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter {Name = "sMyGroup" , Value=sMyGroup }
                        }
            });
            if (sqlCheckLineAll.Data.Count() != 0)
                return true;
            else
                return false;
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            // Updating the Label which displays the current second
            txtTime.Text = DateTime.Now.ToString();
            // Forcing the CommandManager to raise the RequerySuggested event
            CommandManager.InvalidateRequerySuggested();
        }

        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        private async void mainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                combGroup = "";
                if (sMyLine != "ALL")
                {
                    if (await checkLineAll() == true)
                    {
                        var delete = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = "delete SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' AND VR_ITEM='Line_All' and VR_NAME='All' and VR_VALUE=:MyGroup",
                            SfcParameters = new List<SfcParameter> { new SfcParameter { Name = "MyGroup", Value = sMyGroup } }
                        });
                    }
                    else
                    {
                        var delete = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = "delete SFIS1.C_PARAMETER_INI where PRG_NAME='CQC' AND VR_ITEM='Line_Single' and VR_VALUE=:MyGroup and VR_CLASS=:MyIP",
                            SfcParameters = new List<SfcParameter> {
                                new SfcParameter { Name = "MyGroup", Value = sMyGroup },
                                new SfcParameter {Name="MyIP", Value= GetIPAddress().ToString() }
                            }
                        });
                    }

                    Application.Current.Shutdown();
                }

            }
            catch (Exception)
            {


            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //string sCompany = ini.Read("CQC", "Company");
            //if (String.IsNullOrEmpty(sCompany))
            //{
            //    CompanyWindow formSetupCompany = new CompanyWindow();
            //    formSetupCompany.txbCompany.Focus();
            //    formSetupCompany.ShowDialog();
            //    sCompany = formSetupCompany.txbCompany.Text.ToString().ToUpper();
            //    ini.Write("CQC", "Company", sCompany);
            //}
            //if (N0651.IsChecked == true || N0401.IsChecked == true)
            //{
            //    editCheckQty.Text = "0";
            //}
        }
    }

}
