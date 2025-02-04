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
using CQC.ViewModel;
using Sfc.Library.HttpClient.Helpers;

namespace CQC
{
    /// <summary>
    /// Interaction logic for TransferWindow.xaml
    /// </summary>
    public partial class TransferWindow : Window
    {
        private string sMO, sModel;
        private int M_iFailSSN5, M_iReplaceSN5;
        private string M_sModel;
        private SfcHttpClient sfcClient;
        bool chkbRemoveFailSSN, chkbInsertSNtoPallet, chkbTransferbyPiece;
        INIFile ini = new INIFile("SFIS.ini");
        private MainWindow formCQC;
        List<R107> quryCheckPrior = new List<R107>();
        List<R107> quryCheckFailSSN = new List<R107>();
        //binding formsetup group
        public string combPriorGroup = "";
        public TransferWindow()
        {
            InitializeComponent();
        }
        public TransferWindow(SfcHttpClient _sfcClient, bool _chkbRemoveFailSSN, bool _chkbInsertSNtoPallet, bool _chkbTransferbyPiece, MainWindow _formCQC)
        {
            sfcClient = _sfcClient;
            formCQC = _formCQC;
            InitializeComponent();
            chkbRemoveFailSSN = _chkbRemoveFailSSN;
            chkbInsertSNtoPallet = _chkbInsertSNtoPallet;
            chkbTransferbyPiece = _chkbTransferbyPiece;
        }
        public void WriteIniFile()
        {
            if(chkbRemoveFailSSN == true)
            {
                if(chkbPallet.IsChecked == true)
                {
                    ini.Write("CQC", "Transer Pallet", "N/A");
                }
                else
                {
                    ini.Write("CQC", "Transer Pallet", "");
                }
                if(chkbCarton.IsChecked == true)
                {
                    ini.Write("CQC", "Transer Carton", "N/A");
                }
                else
                {
                    ini.Write("CQC", "Transer Carton", "");
                }
                if(chkbSSN2.IsChecked == true)
                {
                    ini.Write("CQC", "Transer SSN", "N/A");
                }
                else
                {
                    ini.Write("CQC", "Transer SSN", "");
                }
            }//if1

            if (chkbInsertSNtoPallet == true)
            {
                if(chkbSSN.IsChecked == true)
                {
                    ini.Write("CQC", "New SSN equal SN", "Y");
                }
                else
                {
                    ini.Write("CQC", "New SSN equal SN", "N");
                }
            }//if2

            ini.Write("CQC", "Transer Prior Group", combPriorGroup);

            if(chkbTransferbyPiece == true)
            {
                if(chkbRepPallet.IsChecked == true)
                {
                    ini.Write("CQC", "Replaced Pallet", "Y");
                }
                else
                {
                    ini.Write("CQC", "Replaced Pallet", "N");
                }
                if(chkbRepCarton.IsChecked == true)
                {
                    ini.Write("CQC", "Replaced Carton", "Y");
                }
                else
                {
                    ini.Write("CQC", "Replaced Carton", "N");
                }
                if (chkbRepTray.IsChecked == true)
                {
                    ini.Write("CQC", "Replaced Tray", "Y");
                }
                else
                {
                    ini.Write("CQC", "Replaced Tray", "N");
                }
                if(chkbRepSSN.IsChecked == true)
                {
                    ini.Write("CQC", "Replaced SSN", "Y");
                }
                else
                {
                    ini.Write("CQC", "Replaced SSN", "N");
                }
            }//if3
        }

        private async void editFailSSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                editFailSSN.Text = editFailSSN.Text.ToUpper();
                editFailSSN.SelectAll();
                editFailSSN.Focus();

                if (ListbRewokSSN.Items.IndexOf(editFailSSN.Text) != -1)
                    return;

                string sql = "select * from SFISM4.R_WIP_TRACKING_T where ((group_name = '" + formCQC.sMyGroup + "' and error_flag=1) or error_flag=2) ";

                if (formCQC.itemSamplingSN.IsChecked)
                    sql += " and serial_number = '" + editFailSSN.Text + "'";
                else
                    sql += " and shipping_sn = '" + editFailSSN.Text + "'";

                var quryCheckFailSSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });
                if (quryCheckFailSSN.Data.Count() == 0)
                {
                    if (formCQC.itemSamplingSN.IsChecked)
                        MessageBox.Show("Illege Serial Number !!");
                    if (formCQC.itemSamplingSSN.IsChecked)
                        MessageBox.Show("Illege Shipping SN !!");
                }
                else
                {
                    List<R107> datalist = new List<R107>();
                    datalist = quryCheckFailSSN.Data.ToListObject<R107>().ToList();
                    if (formCQC.itemPallet.IsChecked && datalist[0].PALLET_NO == "N/A")
                    {
                        MessageBox.Show("The Pallet NO don''t Exist !!");
                        return;
                    }

                    if (formCQC.itemCarton.IsChecked && datalist[0].CARTON_NO == "N/A")
                    {
                        MessageBox.Show("The Carton NO don''t Exist !!");
                        return;
                    }
                    if (formCQC.itemTray.IsChecked && datalist[0].TRAY_NO == "N/A")
                    {
                        MessageBox.Show("The Tray NO don''t Exist !!");
                        return;
                    }
                    ListbRewokSSN.Items.Add(editFailSSN.Text);
                }
            }
        }

        private async void bbtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ListbRewokSSN.Items.Count == 0)
            {
                if (formCQC.itemSamplingSN.IsChecked)
                    MessageBox.Show("Please Input Remove SN");
                if (formCQC.itemSamplingSSN.IsChecked)
                    MessageBox.Show("Please Input Remove SSN");
                return;
            }

            if (MessageBox.Show("Are you sure Update ?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string sql = "";
                string sSQL = "";
                List<R107> snlist = new List<R107>();
                List<R107> snlist1 = new List<R107>();
                for (int i = 0; i <= ListbRewokSSN.Items.Count; i++)
                {
                    sql = "select * from SFISM4.R_WIP_TRACKING_T ";
                    if (formCQC.itemSamplingSN.IsChecked == true)
                        sql += "where serial_number= '" + ListbRewokSSN.Items[i] + "'";
                    else
                        sql += "where shipping_sn='" + ListbRewokSSN.Items[i] + "'";

                    var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    snlist = qury.Data.ToListObject<R107>().ToList();

                    var updateCQC = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "Update SFISM4.R_CQC_REC_T set FAIL_QTY = FAIL_QTY-1  where LOT_NO = '" + snlist[0].QA_NO + "'",
                        SfcCommandType = SfcCommandType.Text
                    });

                    sql = "SELECT R107.SPECIAL_ROUTE,GROUP_NEXT " +
                              "   FROM SFISM4.R_WIP_TRACKING_T R107 " +
                              "       ,SFIS1.C_ROUTE_CONTROL_T C " +
                              "  WHERE R107.SERIAL_NUMBER = '" + snlist[0].SERIAL_NUMBER + "' " +
                              "    AND R107.SPECIAL_ROUTE = C.ROUTE_CODE " +
                              "    AND C.GROUP_NAME = '" + formCQC.sMyGroup + "' " +
                              "    AND C.STATE_FLAG = '1' ";
                    var quryNext = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sql,
                        SfcCommandType = SfcCommandType.Text
                    });
                    snlist1 = quryNext.Data.ToListObject<R107>().ToList();

                    //clear pallet,carton,ssn
                    sSQL = "Update SFISM4.R_WIP_TRACKING_T set Pallet_Full_Flag = 'N',QA_NO = 'N/A',QA_RESULT ='N/A',next_station = '" + snlist1[0].GROUP_NEXT + "'";

                    if (chkbPallet.IsChecked == true)
                        sSQL += ", pallet_no = 'N/A' ";
                    if (chkbCarton.IsChecked == true)
                        sSQL += ", carton_no = 'N/A'";
                    if (chkbTray.IsChecked == true)
                        sSQL += ", tray_no = 'N/A'";
                    if (chkbSSN2.IsChecked == true)
                        sSQL += ", shipping_sn = 'N/A'";
                    if (formCQC.itemSamplingSN.IsChecked == true)
                        sSQL += " where serial_number = '" + snlist[0].SERIAL_NUMBER + "' ";
                    if (formCQC.itemSamplingSSN.IsChecked == true)
                        sSQL += " where shipping_sn = '" + snlist[0].SHIPPING_SN + "' ";

                    var quryUpdate = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });

                    updateCQC = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "Update SFISM4.R_QC_SN_T Set COUNTER=COUNTER+1 where serial_number = '" + snlist[0].SERIAL_NUMBER + "'",
                        SfcCommandType = SfcCommandType.Text
                    });
                }
                ListbRewokSSN.Items.Clear();
                editFailSSN.Text = "";
                MessageBox.Show("Update Finish !!");
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SetupGroupWindow formSetupGroup = new SetupGroupWindow(this, sfcClient);
        }

        private void bbtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void editUnpackSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !string.IsNullOrEmpty(editUnpackSN.Text))
            {
                editUnpackSN.SelectAll();
                editUnpackSN.Focus();
                if (combPriorGroup == "")
                {
                    MessageBox.Show("Please Input Prior Group !!");
                    return;
                }
                editUnpackSN.Text = editUnpackSN.Text.ToUpper();
                if (listUpdateSN.Items.IndexOf(editUnpackSN.Text) != -1)
                    return;

                string SSQL = " select * from SFISM4.R_WIP_TRACKING_T " +
                              " where serial_number='" + editUnpackSN.Text + "' and Group_name = '" + combPriorGroup + "' and next_station='N/A' " +
                              " and error_flag <> 1 and error_flag <> 2";
                var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = SSQL,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data.Count() == 0)
                {
                    MessageBox.Show("Illege Serial Number !!");
                    return;
                }
                else
                {
                    List<R107> datalist = new List<R107>();
                    datalist = qury.Data.ToListObject<R107>().ToList();
                    if ((formCQC.itemPallet.IsChecked == true && datalist[0].PALLET_NO != "N/A")
                        || (formCQC.itemCarton.IsChecked == true && datalist[0].CARTON_NO != "N/A")
                       || (formCQC.itemTray.IsChecked == true && datalist[0].TRAY_NO != "N/A")
                       || (formCQC.itemSSN.IsChecked == true && datalist[0].SHIPPING_SN != "N/A"))
                    {
                        MessageBox.Show("Illege Serial Number !!");
                        return;
                    }
                    if (string.IsNullOrEmpty(M_sModel))
                        M_sModel = datalist[0].MODEL_NAME;
                    if (M_sModel != datalist[0].MODEL_NAME)
                    {
                        MessageBox.Show("Different Model !!");
                        return;
                    }
                    listUpdateSN.Items.Add(editUnpackSN.Text);
                }
            }
        }

        private async void combPallet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFISM4.R_WIP_TRACKING_T where pallet_no='" + combPallet.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data.Count() == 0)
                {
                    MessageBox.Show("No Pallet Number!!");
                    combPallet.Text = "";
                    return;
                }
            }
        }

        private async void combCarton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFISM4.R_WIP_TRACKING_T where carton_no='" + combCarton.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data.Count() == 0)
                {
                    MessageBox.Show("No Carton Number!!");
                    combCarton.Text = "";
                    return;
                }
            }
        }

        private async void combTray_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFISM4.R_WIP_TRACKING_T where tray_no='" + combTray.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data.Count() == 0)
                {
                    MessageBox.Show("No Tray Number!!");
                    combTray.Text = "";
                    return;
                }
            }
        }

        private async void editSSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && chkbSSN.IsChecked == false)
            {
                var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFISM4.R_WIP_TRACKING_T where shipping_sn= '" + editSSN.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data.Count() > 0)
                {
                    MessageBox.Show("Duplicate Shipping SN");
                    editSSN.Text = "";
                    return;
                }
            }
        }

        private void BitBtn4_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bbtnClose3_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BitBtn3_Click(object sender, RoutedEventArgs e)
        {
            if (combPallet.Text == "")
                combPallet.SelectedIndex = combPallet.Items.IndexOf("N/A");
            if (combCarton.Text == "")
                combCarton.SelectedIndex = combCarton.Items.IndexOf("N/A");
            if (combTray.Text == "")
                combTray.SelectedIndex = combTray.Items.IndexOf("N/A");
            if (editSSN.Text == "" && chkbSSN.IsChecked == false)
                editSSN.Text = "N/A";

            if (editSSN.Text != "N/A")
            {
                var qury1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select * from SFISM4.R_WIP_TRACKING_T where shipping_sn= '" + editSSN.Text + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury1.Data.Count() > 0)
                {
                    MessageBox.Show("Duplicate Shipping SN");
                    editSSN.Text = "";
                    return;
                }
            }

            string SSQL = "Select QA_NO from SFISM4.R_WIP_TRACKING_T ";
            if (formCQC.itemPallet.IsChecked == true)
                SSQL += "where Pallet_NO = '" + combPallet.Text + "'";
            if (formCQC.itemCarton.IsChecked == true)
                SSQL += "where carton_no = '" + combCarton.Text + "'";
            if (formCQC.itemTray.IsChecked == true)
                SSQL += "where Tray_NO = '" + combTray.Text + "'";

            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = SSQL,
                SfcCommandType = SfcCommandType.Text
            });
            List<R107> datalist = new List<R107>();
            datalist = qury.Data.ToListObject<R107>().ToList();

            string sQA_NO = datalist[0].QA_NO;

            for (int i = 0; i <= listUpdateSN.Items.Count; i++)
            {
                if (chkbSSN.IsChecked == false)
                {
                    SSQL = "Update SFISM4.R_WIP_TRACKING_T " +
                  "  set pallet_no = '" + combPallet.Text + "' " +
                  " ,carton_no = '" + combCarton.Text + "' " +
                  " ,tray_no = '" + combTray.Text + "' " +
                  " ,shipping_sn = '" + editSSN.Text + "'     " +
                  " ,PALLET_FULL_FLAG='Y' " +
                  " ,NEXT_STATION ='" + formCQC.sMyGroup + "' " +
                  " ,QA_NO = '" + sQA_NO + "'  " +
                  " ,GROUP_NAME = '" + ini.Read("CQC", "CQC Prior Group") + "' " +
                  "  where serial_number = '" + listUpdateSN.Items[i] + "'";
                }
                else
                {
                    SSQL = "Update SFISM4.R_WIP_TRACKING_T " +
                  "  set pallet_no = '" + combPallet.Text + "' " +
                  " ,carton_no = '" + combCarton.Text + "' " +
                  " ,tray_no = '" + combTray.Text + "' " +
                  " ,shipping_sn = '" + listUpdateSN.Items[i] + "'     " +
                  " ,PALLET_FULL_FLAG='Y' " +
                  " ,NEXT_STATION ='" + formCQC.sMyGroup + "' " +
                  " ,QA_NO = '" + sQA_NO + "'  " +
                  " ,GROUP_NAME = '" + ini.Read("CQC", "CQC Prior Group") + "' " +
                  "  where serial_number = '" + listUpdateSN.Items[i] + "'";
                }
                var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                {
                    CommandText = SSQL,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            MessageBox.Show("Update Finish !!");
            M_sModel = "";
            listUpdateSN.Items.Clear();
            combPallet.Text = "";
            combCarton.Text = "";
            combTray.Text = "";
            editSSN.Text = "";
            editUnpackSN.Text = "";
        }

        private void editReplaceSN2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                checkReplaceSN();
        }

        async void checkReplaceSN()
        {

            if (editReplaceSN2.Text != "")
            {
                M_iReplaceSN5 = 0;
                if (string.IsNullOrEmpty(combPriorGroup))
                {
                    MessageBox.Show("Please Input Prior Group !!");
                    return;
                }
            }
            editReplaceSN2.Text = editReplaceSN2.Text.ToUpper();
            string sSQL = "select * from SFISM4.R_WIP_TRACKING_T " +
                          "  where serial_number='" + editReplaceSN2.Text + "' and Group_name = '" + combPriorGroup + "' " +
                          "  and next_station='N/A' " +
                          "  and error_flag <> 1 and error_flag <> 2";
            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });

            if (qury.Data.Count() == 0)
            {
                MessageBox.Show("Illege Serial Number !!");
                return;
            }
            else
            {
                quryCheckPrior = qury.Data.ToListObject<R107>().ToList();
                if ((formCQC.itemPallet.IsChecked == true && quryCheckPrior[0].PALLET_NO != "N/A") || (formCQC.itemCarton.IsChecked == true && quryCheckPrior[0].CARTON_NO != "N/A")
                    || (formCQC.itemTray.IsChecked == true && quryCheckPrior[0].TRAY_NO != "N/A") || (formCQC.itemSSN.IsChecked == true && quryCheckPrior[0].SHIPPING_SN != "N/A"))
                {
                    MessageBox.Show("Illege Serial Number !!");
                    return;
                }

                M_iReplaceSN5 = 1;
                if (formCQC.itemMO.IsChecked == true && sMO != quryCheckPrior[0].MO_NUMBER)
                {
                    MessageBox.Show("Different MO !!");
                    editReplaceSN2.Text = "";
                    editReplaceSN2.Focus();
                }
                if (formCQC.itemModel.IsChecked == true && sMO != quryCheckPrior[0].MODEL_NAME)
                {
                    MessageBox.Show("Different Model !!");
                    editReplaceSN2.Text = "";
                    editReplaceSN2.Focus();
                }
            }//not null
        }

        private async void bbtnUpdate3_Click(object sender, RoutedEventArgs e)
        {
            if (M_iFailSSN5 == 0)
                checkFailSSN();
            if (M_iReplaceSN5 == 0)
                checkReplaceSN();

            if (M_iFailSSN5 == 1 && M_iReplaceSN5 == 1)
            {
                if (quryCheckFailSSN.Count > 0 && quryCheckPrior.Count > 0)
                {
                    var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "Update SFISM4.R_CQC_REC_T set FAIL_QTY = FAIL_QTY-1 where LOT_NO = '" + quryCheckFailSSN[0].QA_NO + "'",
                        SfcCommandType = SfcCommandType.Text
                    });

                    update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "Update SFISM4.R_QC_SN_T Set COUNTER=COUNTER+1 where serial_number = '" + quryCheckFailSSN[0].SERIAL_NUMBER + "'",
                        SfcCommandType = SfcCommandType.Text
                    });

                    string sSQL = "Update SFISM4.R_WIP_TRACKING_T " +
                                 " Set QA_NO = '" + quryCheckFailSSN[0].QA_NO + "' , next_station = '" + formCQC.sMyGroup + "' ,PALLET_FULL_FLAG='Y' ,Group_Name='" + ini.Read("CQC", "CQC Prior Group") + "'";
                    if (chkbRepPallet.IsChecked == true)
                        sSQL += " ,Pallet_NO = '" + quryCheckFailSSN[0].PALLET_NO + "' ";
                    if (chkbRepCarton.IsChecked == true)
                        sSQL += " ,Carton_NO = '" + quryCheckFailSSN[0].CARTON_NO + "' ";
                    if (chkbRepTray.IsChecked == true)
                        sSQL += " ,Tray_NO = '" + quryCheckFailSSN[0].TRAY_NO + "' ";
                    if (chkbRepSSN.IsChecked == true)
                        sSQL += ", Shipping_SN = Serial_Number";
                    else
                        sSQL += ", Shipping_SN = '" + editRepSSN2.Text + "'";
                    sSQL += "where serial_number = '" + editRepSSN2.Text + "'";
                    var update1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    //search next group
                    sSQL = " SELECT R107.SPECIAL_ROUTE,GROUP_NEXT " +
                           "   FROM SFISM4.R_WIP_TRACKING_T R107, " +
                           "        SFIS1.C_ROUTE_CONTROL_T C " +
                           "  WHERE R107.SERIAL_NUMBER='" + quryCheckFailSSN[0].SERIAL_NUMBER + "' " +
                           "    AND R107.SPECIAL_ROUTE=C.ROUTE_CODE " +
                           "    AND C.GROUP_NAME='" + formCQC.sMyGroup + "' " +
                           "    AND C.STATE_FLAG='1' ";
                    var quryNextGroup = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sSQL,
                        SfcCommandType = SfcCommandType.Text
                    });
                    List<R107> datalist = new List<R107>();
                    datalist = quryNextGroup.Data.ToListObject<R107>().ToList();

                    sSQL = "Update SFISM4.R_WIP_TRACKING_T " +
                          " Set QA_NO='N/A' , QA_RESULT='N/A' " +
                          "   , next_station = '"+ datalist[0].GROUP_NEXT+ "' , Pallet_NO='N/A' " +
                          "   , CARTON_NO='N/A',TRAY_NO='N/A' , Shipping_SN='N/A' , PALLET_FULL_FLAG='N'";
                    if (formCQC.itemSamplingSN.IsChecked == true)
                        sSQL += "where serial_number = '"+ editFailSSN2.Text+ "' ";
                    if (formCQC.itemSamplingSSN.IsChecked == true)
                        sSQL += "where shipping_sn = '" + editFailSSN2.Text + "' ";

                    var update2 = sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                    {
                        CommandText=sSQL,
                        SfcCommandType=SfcCommandType.Text
                    });
                    MessageBox.Show("Replace OK !!");
                    M_iFailSSN5 = 0;
                    M_iReplaceSN5 = 0;
                    editFailSSN2.Text = "";
                    editReplaceSN2.Text = "";
                    editFailSSN2.Focus();

                }
                else
                {
                    MessageBox.Show("Different Model !!");
                    editReplaceSN2.SelectAll();
                    editReplaceSN2.Focus();
                }
            }
        }

        private void chkbRepSSN_Click(object sender, RoutedEventArgs e)
        {
            chkbRepSSN.IsChecked = !chkbRepSSN.IsChecked;
            if (chkbRepSSN.IsChecked == true)
            {
                editRepSSN2.IsEnabled = false;
                editRepSSN2.Text = "N/A";
            }
            else
                editRepSSN2.IsEnabled = false;
        }

        private void chkbSSN_Click(object sender, RoutedEventArgs e)
        {
            chkbSSN.IsChecked = !chkbSSN.IsChecked;
            if (chkbSSN.IsChecked == true)
            {
                editSSN.IsEnabled = false;
                editSSN.Text = "N/A";
            }
            else
                editSSN.IsEnabled=true;
        }

        private void editFailSSN2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                checkFailSSN();
        }

        async void checkFailSSN()
        {
            if (editFailSSN2.Text != "")
            {
                quryCheckFailSSN.Clear();
                sMO = "";
                sModel = "";
                M_iFailSSN5 = 0;
                editFailSSN2.Text = editFailSSN2.Text.ToUpper();
                editFailSSN2.SelectAll();
                editFailSSN2.Focus();

                string sSQL = "select * from SFISM4.R_WIP_TRACKING_T where ((group_name = '" + formCQC.sMyGroup + "' and error_flag=1) or error_flag=2)";

                if (formCQC.itemSamplingSN.IsChecked)
                    sSQL += " and serial_number = '" + editFailSSN2.Text + "' ";
                else
                    sSQL += " and shipping_sn = '" + editFailSSN2.Text + "' ";

                var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = sSQL,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data.Count() == 0)
                {
                    MessageBox.Show("Illege Serial Number or Shipping SN  !!");
                    return;
                }
                else
                {
                    quryCheckFailSSN = qury.Data.ToListObject<R107>().ToList();
                    if (formCQC.itemPallet.IsChecked && quryCheckFailSSN[0].PALLET_NO == "N/A")
                    {
                        MessageBox.Show("The Pallet NO don''t Exist !!");
                        return;
                    }
                    if (formCQC.itemCarton.IsChecked && quryCheckFailSSN[0].CARTON_NO == "N/A")
                    {
                        MessageBox.Show("The Carton NO don''t Exist !!");
                        return;
                    }
                    if (formCQC.itemTray.IsChecked && quryCheckFailSSN[0].TRAY_NO == "N/A")
                    {
                        MessageBox.Show("The Tray NO don''t Exist !!");
                        return;
                    }
                    editReplaceSN2.Focus();
                    editReplaceSN2.SelectAll();
                    if (formCQC.itemMO.IsChecked)
                        sMO = quryCheckFailSSN[0].MO_NUMBER;
                    if (formCQC.itemModel.IsChecked)
                        sModel = quryCheckFailSSN[0].MODEL_NAME;
                    M_iFailSSN5 = 1;
                }
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            WriteIniFile();
            this.Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!chkbRemoveFailSSN)
                grid1.Visibility = Visibility.Collapsed;
            if (!chkbInsertSNtoPallet)
                grid2.Visibility = Visibility.Collapsed;
            if (!chkbTransferbyPiece)
                grid3.Visibility = Visibility.Collapsed;
            M_sModel = "";
            ReadIniFile();
            if (formCQC.itemSamplingSN.IsChecked)
            {
                grpbFailSSN.Header = "Fail SN";
                grpbReworkSSN.Header = "Remove SN";
                grpbUnPack.Header = "Prior Group SN";

                TabSheetReworkSSN.Header = "Remove Fail SN";

                if (formCQC.itemPallet.IsChecked)
                    TabSheetAddSSN.Header = "Add SN to Pallet";

                if (formCQC.itemCarton.IsChecked)
                    TabSheetAddSSN.Header = "Add SN to Carton";
                if (formCQC.itemTray.IsChecked)
                    TabSheetAddSSN.Header = "Add SN to Tray";

                grpbFailSSN2.Header = "Failed SN";


            }
        }
        void ReadIniFile()
        {
            if (chkbRemoveFailSSN)
            {
                if (ini.Read("CQC", "Transer Pallet") == "N/A")
                    chkbPallet.IsChecked = true;
                else
                    chkbPallet.IsChecked = false;

                if (ini.Read("CQC", "Transer Carton") == "N/A")
                    chkbCarton.IsChecked = true;
                else
                    chkbCarton.IsChecked = false;
                if (ini.Read("CQC", "Transer SSN") == "N/A")
                    chkbSSN2.IsChecked = true;
                else
                    chkbSSN2.IsChecked = false;

                editRepSSN2.Text = "N/A";
            }

            if (chkbInsertSNtoPallet)
            {
                if (ini.Read("CQC", "New SSN equal SN") == "Y")
                    chkbSSN.IsChecked = true;
                else
                    chkbSSN.IsChecked = false;
            }

            if (chkbTransferbyPiece)
            {
                if (ini.Read("CQC", "Replaced Pallet") == "Y")
                    chkbRepPallet.IsChecked = true;
                else
                    chkbRepPallet.IsChecked = false;

                if (ini.Read("CQC", "Replaced Carton") == "Y")
                    chkbRepCarton.IsChecked = true;
                else
                    chkbRepCarton.IsChecked = false;

                if (ini.Read("CQC", "Replaced Tray") == "Y")
                    chkbRepTray.IsChecked = true;
                else
                    chkbRepTray.IsChecked = false;

            }

        }
    }
}
