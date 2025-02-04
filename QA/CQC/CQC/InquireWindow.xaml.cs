using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Sfc.Core.Parameters;
using CQC.ViewModel;
using Sfc.Library.HttpClient.Helpers;
using Sfc.Library.HttpClient;
using Microsoft.Win32;

namespace CQC
{
    /// <summary>
    /// Interaction logic for InquireWindow.xaml
    /// </summary>
    public partial class InquireWindow : Window
    {
        SfcHttpClient sfcClient;

        private MainWindow formCQC;
        string sKind;
        
        public InquireWindow()
        {
            InitializeComponent();
        }
        public InquireWindow(MainWindow _formCQC, SfcHttpClient _sfcClient, string _sKind)
        {
            sKind = _sKind;
            formCQC = _formCQC;
            sfcClient = _sfcClient;
            InitializeComponent();

        }

        private async void Window_Initialized(object sender, EventArgs e)
        {
            if (formCQC.itemCheckTray.IsChecked)
            {
                lablTitle.Content = "Tray NO";
                pallet.Header = "Tray NO";
            }
            if (formCQC.itemCheckCarton.IsChecked)
            {
                lablTitle.Content = "CARTON_NO";
                pallet.Header = "CARTON_NO";
            }
            if (formCQC.itemCheckPallet.IsChecked)
            {
                lablTitle.Content = "Pallet NO";
                pallet.Header = "Pallet NO";
            }
            if (formCQC.editCQCUnit != "")
            {
                lablTitle.Content = formCQC.editCQCUnit;
                pallet.Header = formCQC.editCQCUnit;
            }
            if (sKind == "1")
            {
                string SSQL = "select * from SFISM4.R_WIP_TRACKING_T";
                if (formCQC.itemSamplingSN.IsChecked)
                    SSQL += " where serial_number = '" + formCQC.combDef.Text + "' ";
                if (formCQC.itemSamplingSSN.IsChecked)
                    SSQL += " where shipping_sn = '" + formCQC.combDef.Text + "' ";
                editUnit.Text = formCQC.combDef.Text;

                var qury = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = SSQL,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qury.Data != null /*&& qury.Data.Count != 0*/)
                {
                    if (formCQC.itemCheckTray.IsChecked)
                        editUnit.Text = qury.Data["tray_no"].ToString();
                    if (formCQC.itemCheckCarton.IsChecked)
                        editUnit.Text = qury.Data["carton_no"].ToString();
                    if (formCQC.itemCheckPallet.IsChecked)
                        editUnit.Text = qury.Data["pallet_no"].ToString();
                }
                else
                    editUnit.Text = "N/A";
            }
            if (sKind == "2")
                editUnit.Text = formCQC.sSelectPallet;
            if (sKind == "2")
                editUnit.Text = formCQC.sSelectPallet2;
            displayData();
        }

        async void displayData()
        {
            if (editUnit.Text == "N/A" || string.IsNullOrEmpty(editUnit.Text))
                return;
            string sSQL = "";
            if (formCQC.itemCheckTray.IsChecked)
                sSQL = "select tray_no ";

            if (formCQC.itemCheckCarton.IsChecked)
                sSQL = "select carton_no ";

            if (formCQC.itemCheckPallet.IsChecked)
                sSQL = "select Pallet_no ";

            sSQL += ", serial_number , shipping_sn from SFISM4.R_WIP_TRACKING_T";
            if (formCQC.itemCheckTray.IsChecked)
            {
                sSQL += " where Tray_No = '" + editUnit.Text + "'";
                pallet.Binding = new Binding("TRAY_NO");
            }
            if (formCQC.itemCheckCarton.IsChecked)
            {
                sSQL += " where Carton_No = '" + editUnit.Text + "'";
                pallet.Binding = new Binding("CARTON_NO");
            }
            if (formCQC.itemCheckPallet.IsChecked)
            {
                sSQL += " where Pallet_No = '" + editUnit.Text + "'";
                pallet.Binding = new Binding("PALLET_NO");
            }

            var qury = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = sSQL,
                SfcCommandType = SfcCommandType.Text
            });
            if (qury.Data == null || qury.Data.Count() == 0)
                return;

            if (qury.Data.Count() > 0)
            {
                List<R107> datalist = new List<R107>();
                datalist = qury.Data.ToListObject<R107>().ToList();
                DBGrid1.ItemsSource = datalist;
            }
        }

        private void editUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                displayData();
        }

        private void bbtnPrint_Click(object sender, RoutedEventArgs e)
        {
            List<R107> datalist = DBGrid1.ItemsSource as List<R107>;
            if (datalist.Count == 0)
            {
                MessageBox.Show("No records", "CQC");
                return;
            }
            List<R107> snlist = new List<R107>();
            foreach (R107 items in datalist)
            {
                if (formCQC.itemCheckTray.IsChecked)
                {
                    snlist.Add(new R107() { CARTON_NO = items.TRAY_NO, SERIAL_NUMBER = items.SERIAL_NUMBER, SHIPPING_SN = items.SHIPPING_SN });
                }
                if (formCQC.itemCheckCarton.IsChecked)
                {
                    snlist.Add(new R107() { CARTON_NO = items.CARTON_NO, SERIAL_NUMBER = items.SERIAL_NUMBER, SHIPPING_SN = items.SHIPPING_SN });
                }
                if (formCQC.itemCheckPallet.IsChecked)
                {
                    snlist.Add(new R107() { CARTON_NO = items.PALLET_NO, SERIAL_NUMBER = items.SERIAL_NUMBER, SHIPPING_SN = items.SHIPPING_SN });
                }
            }

            ExportDgvToXML(snlist);

        }
        public void ExportDgvToXML(List<R107> list)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (formCQC.itemCheckTray.IsChecked)
            {
                dt.Columns.Add("Tray_No", typeof(string));
            }
            if (formCQC.itemCheckCarton.IsChecked)
            {
                dt.Columns.Add("Carton_No", typeof(string));
            }
            if (formCQC.itemCheckPallet.IsChecked)
            {
                dt.Columns.Add("Pallet_No", typeof(string));
            }

            dt.Columns.Add("Serial_Number", typeof(string));
            dt.Columns.Add("Shipping_SN", typeof(string));
            foreach (R107 items in list)
            {
                dt.Rows.Add(items.CARTON_NO, items.SERIAL_NUMBER, items.SHIPPING_SN);
            }
            dt.TableName = "DATA";
            SaveFileDialog sfd = new SaveFileDialog();
            //sfd.Filter = "XML|*.xml";
            sfd.Filter = "Excel Files |*.xls";
            // Show save file dialog box
            Nullable<bool> result = sfd.ShowDialog();
            if (result == true)
            {
                try
                {
                    dt.WriteXml(sfd.FileName);
                    MessageBox.Show("Export Successfully", "Finish");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }


        private void bbtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
