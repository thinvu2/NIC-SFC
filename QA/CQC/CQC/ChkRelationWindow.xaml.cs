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
using CQC.ViewModel;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CQC
{
    /// <summary>
    /// Interaction logic for FormChkRelationWindow.xaml
    /// </summary>
    public partial class ChkRelationWindow : Window
    {
        SfcHttpClient sfcClient;
        INIFile ini = new INIFile("SFIS.ini");
        private MainWindow formCQC;
        List<R107> snlist = new List<R107>();
        private string SNstr = string.Empty;
        public ChkRelationWindow()
        {
            InitializeComponent();
        }
        public ChkRelationWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            sfcClient = formCQC.sfcClient;
            InitializeComponent();
        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            //mcarton
            if (ini.Read("CQC", "Mcarton") == "Y")
            {
                itemMcarton.IsChecked = true;
            }
            else
                itemMcarton.IsChecked = false;

            txtSN.IsEnabled = true;
            txtSSN.IsEnabled = true;
            txtTray.IsEnabled = true;
            txtCarton.IsEnabled = true;


            lblSN.IsEnabled = true;
            lblSSN.IsEnabled = true;
            lblTray.IsEnabled = true;
            lblCarton.IsEnabled = true;

            txtSN.Text = string.Empty;
            txtSSN.Text = string.Empty;
            txtTray.Text = string.Empty;
            txtCarton.Text = string.Empty;

            if (await formCQC.Ischeckfinishgood(formCQC.str_model_name))
            {
                LblFinishGN.Visibility = Visibility.Visible;
                txtFinishGN.Visibility = Visibility.Visible;
                txtFinishGN.Focus();
                txtFinishGN.SelectAll();
                RefreshDataGrid();
            }
            else
            {
                LblFinishGN.Visibility = Visibility.Collapsed;
                txtFinishGN.Visibility = Visibility.Collapsed;
                RefreshDataGrid();
                txtSN.SelectAll();
                txtSN.Focus();
            }
        }

        async void RefreshDataGrid()
        {
            DataGrid.ItemsSource = null;
            string sql = " SELECT r.Serial_Number, r.Shipping_SN, r.Error_Flag, Carton_No, Pallet_No, r.model_name " +
                        " FROM SFISM4.R_QC_SN_T r, sfism4.r_wip_tracking_t r7 " +
                        " where lot_no = '" + formCQC.combLot.Text + "' and counter = 0 " +
                        " and(r.check_flag is null  or  r.check_flag <> 'Y') and r.serial_number = r7.serial_number";
            var qryCQCcheck = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (qryCQCcheck.Data.Count() == 0)
            {
                MessageBox.Show("no qa records", "CQC");
                return;
            }
            SNstr = string.Empty;
            foreach (R107 c in qryCQCcheck.Data.ToListObject<R107>().ToList())
            {
                SNstr += "'" + c.SERIAL_NUMBER + "'" + ",";
            }

            // remove last ','
            if (!string.IsNullOrEmpty(SNstr))
                SNstr = SNstr.Substring(0, SNstr.Length - 1);

            string tray = "", carton = "", last_tray = "", last_carton = "";
            bool same_tray = true, same_carton = true;


            //add snlist
            string sn = "", tray_no = "", carton_no = "", col5 = "", ssn = "";

            sql = "select SFISM4.R107.*,SFIS1.C_MODEL_DESC_T.MODEL_TYPE " +
                 "from sfism4.r107,SFIS1.C_MODEL_DESC_T " +
                 "where SFISM4.R107.MODEL_NAME=SFIS1.C_MODEL_DESC_T.MODEL_NAME AND " +
                 "SFISM4.R107.serial_number in (" + SNstr + ")  order by SFISM4.R107.serial_number ";

            var query = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            if (query.Data.Count() != 0)
            {
                foreach (R107 item in query.Data.ToListObject<R107>().ToList())
                {
                    tray = item.TRAY_NO;
                    if (itemMcarton.IsChecked)
                        carton = item.MCARTON_NO;
                    else
                        carton = item.CARTON_NO;

                    if (tray != last_tray && last_tray != "")
                        same_tray = false;
                    if (carton != last_carton && last_carton != "")
                        same_carton = false;
                    sn = item.SERIAL_NUMBER;
                    if (formCQC.itemCheckTray.IsChecked)
                    {
                        tray_no = item.TRAY_NO;
                        txtSSN.IsEnabled = false;
                        lblSSN.IsEnabled = false;
                    }
                    else
                    {
                        if (item.SHIPPING_SN == "N/A")
                            ssn = "";
                        else
                            ssn = item.SHIPPING_SN;
                        txtTray.IsEnabled = false;
                        lblTray.IsEnabled = false;
                    }
                    if (itemMcarton.IsChecked)
                    {
                        if (item.MCARTON_NO != "N/A" && item.MCARTON_NO != "")
                        {
                            carton_no = item.MCARTON_NO;
                        }
                        else
                        {
                            txtCarton.IsEnabled = false;
                            lblCarton.IsEnabled = false;
                        }
                    }
                    else
                    {
                        if (item.CARTON_NO != "N/A" && item.CARTON_NO != "")
                            carton_no = item.CARTON_NO;
                        else
                        {
                            txtCarton.IsEnabled = false;
                            lblCarton.IsEnabled = false;
                        }
                    }

                    col5 = "";
                    var tmpqury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SHIPPING_SN ='" + ssn + "' AND SCRAP_FLAG ='0' AND QA_RESULT ='5'",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (tmpqury.Data.Count() > 0)
                    {
                        col5 = "V";

                        var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                        {
                            CommandText = "update sfism4.r_qc_sn_t set check_flag= 'Y' where serial_number = '" + sn + "' ",
                            SfcCommandType = SfcCommandType.Text
                        });
                    }

                    // get last to check if all same carton
                    last_tray = item.TRAY_NO;
                    if (itemMcarton.IsChecked)
                        last_carton = item.MCARTON_NO;
                    else
                        last_carton = item.CARTON_NO;
                    if (item.MODEL_TYPE.IndexOf("F") > 0)
                    {
                        txtSSN.IsEnabled = false;
                        lblSSN.IsEnabled = false;
                    }
                    snlist.Add(new R107()
                    {
                        SERIAL_NUMBER = sn,
                        SHIPPING_SN = ssn,
                        TRAY_NO = tray_no,
                        MCARTON_NO = carton_no,
                        CARTON_NO = col5
                    });
                }
                DataGrid.ItemsSource = snlist;
                if (DataGrid.Items.Count > 0)
                {
                    lblTotal.Content = "Total Qty : " + DataGrid.Items.Count;
                    lblChecked.Content = "0";
                }
                if (same_tray)
                {
                    txtTray.IsEnabled = false;
                    if (last_tray == "N/A" || !formCQC.itemCheckTray.IsChecked)
                        txtTray.Text = "";
                    else
                        txtTray.Text = last_tray;
                }
                if (same_carton)
                {
                    txtCarton.IsEnabled = false;
                    if (last_carton == "N/A")
                        txtCarton.Text = "";
                    else
                        txtCarton.Text = last_carton;
                }
            }
        }

        private void txtSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                txtSN.Text = txtSN.Text.ToUpper();
                txtSSN.SelectAll();
                txtSSN.Focus();
            }
        }

        private void txtSSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtCarton.IsEnabled == false)
                    cmdOK_Click(new object(), new RoutedEventArgs());
                else
                {
                    txtCarton.SelectAll();
                    txtCarton.Focus();
                }
            }
        }

        private async void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            if (SNstr == "")
                return;
            string ssql = "SELECT * FROM SFIS1.C_MODEL_DESC_T  where MODEL_NAME =(SELECT MODEL_NAME FROM SFISM4.R_WIP_TRACKING_T WHERE serial_number in (" + SNstr + ") AND ROWNUM='1')";
            var tmpQry = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = ssql,
                SfcCommandType = SfcCommandType.Text
            });
            if (tmpQry.Data == null)
                return;
            if (tmpQry.Data["model_type"].ToString().IndexOf("F") <= 0)
                if (string.IsNullOrEmpty(txtSN.Text))
                    return;
            PanelMSG.Content = "";
            if (await formCQC.Ischeckfinishgood(formCQC.str_model_name))
            {
                tmpQry = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select finish_good2 from sfis1.c_cust_model_t where model_name ='" + formCQC.str_model_name + "'",
                    SfcCommandType = SfcCommandType.Text
                });
                string finish_good_no = "";
                if (tmpQry.Data == null)
                    return;
                if (tmpQry.Data != null)
                {
                    finish_good_no = tmpQry.Data["finish_good2"].ToString().Trim().ToUpper();
                }
                if (finish_good_no != txtFinishGN.Text.ToUpper())
                {
                    MessageBox.Show("FINISH GOOD NO not match:c_cust_model_t", "CQC");
                    return;
                }
            }

            foreach (R107 items in snlist)
            {
                if (items.SERIAL_NUMBER == txtSN.Text.Trim())
                {
                    if (items.SHIPPING_SN.Trim() == txtSSN.Text.Trim() || (items.SHIPPING_SN.Trim() == "N/A"
                       && string.IsNullOrEmpty(txtSSN.Text)) && items.TRAY_NO == txtTray.Text.Trim() && items.MCARTON_NO == txtCarton.Text.Trim())
                    {
                        PanelMSG.Content = txtSN.Text + " " + txtSSN.Text + " " + " " + txtTray.Text + " " + txtCarton.Text;
                        if (items.CARTON_NO != "V")
                        {
                            items.CARTON_NO = "V";
                            lblChecked.Content = (Int32.Parse(lblChecked.Content.ToString()) + 1).ToString();
                            PanelMSG.Content = PanelMSG.Content + " OK!";
                            if (Int16.Parse(lblChecked.Content.ToString()) == DataGrid.Items.Count)
                            {
                                // all checked
                                var update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = "update sfism4.r_qc_sn_t set check_flag= 'Y' where serial_number in (" + SNstr + ")",
                                    SfcCommandType = SfcCommandType.Text
                                });
                                formCQC.combDef.Focus();
                                formCQC.pass_chkRelation = true;
                                this.Close();
                            }
                        }
                        else
                        {
                            PanelMSG.Content = PanelMSG.Content + " Repeat Scan !";
                        }
                        DataGrid.Items.Refresh();
                        if (txtSN.IsEnabled)
                            txtSN.Text = "";
                        if (txtSSN.IsEnabled)
                            txtSSN.Text = "";
                        if (txtTray.IsEnabled)
                            txtTray.Text = "";
                        if (txtCarton.IsEnabled)
                            txtCarton.Text = "";
                        txtSN.Focus();
                        return;
                    }
                    else
                    {
                        PanelMSG.Content = "Not match !";
                        MessageWindow mes1 = new MessageWindow();
                        mes1.txbenglish.Text = "Not Match!";
                        mes1.txbvietnamese.Text = "không đúng!";
                        mes1.ShowDialog();
                        txtSN.SelectAll();
                        txtSN.Focus();
                        return;
                    }
                }
            }

            //wrong sn
            PanelMSG.Content = "Wrong SN !";
            MessageWindow mes = new MessageWindow();
            mes.txbenglish.Text = "Sai SN!";
            mes.txbvietnamese.Text = "Sai SN!";
            txtSN.Focus();
            txtSN.SelectAll();
            return;
        }

        private void cmdExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtFinishGN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                cmdOK_Click(new object(), new RoutedEventArgs());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            INIFile SFIS = new INIFile("SFIS.ini");
            if (itemMcarton.IsChecked)
            {
                SFIS.Write("CQC", "Mcarton", "Y");
            }
            else
                SFIS.Write("CQC", "Mcarton", "N");
        }

        private void txtCarton_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (txtFinishGN.IsVisible == false)
                    cmdOK_Click(new object(), new RoutedEventArgs());
                else
                {
                    txtFinishGN.SelectAll();
                    txtFinishGN.Focus();
                }
            }
        }
    }
}
