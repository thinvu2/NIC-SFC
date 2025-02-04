using Sfc.Core.Parameters;
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

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for IsCheckBox_Tray.xaml
    /// </summary>
    public partial class IsCheckBox_Tray : Window
    {
        public static string carton;
        private string sqlStr;
        public string KP;
        public ListBox lstSN = new ListBox();
        public ListBox lstSSN = new ListBox();
        public int Total = 0;
        public IsCheckBox_Tray()
        {
            InitializeComponent();
        }
        public string RES { get; set; }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblCartonNoSN_SSN1.Content = lblCartonNoSN_SSN.Content;
            var tray_no = lblCartonNoSN_SSN1.Content.ToString();
            //sqlStr = "select * from sfism4.r107 Where MCARTON_NO='" + lblCartonNoSN_SSN.Content + "' OR CARTON_NO='" + lblCartonNoSN_SSN.Content + "'";
            sqlStr = "select SERIAL_NUMBER kp,SHIPPING_SN kp1 from sfism4.r107 Where TRAY_NO='" + tray_no + "'";
            var result = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlStr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null)
            {
                if (result.Data.Count() != 0)
                {
                    lblCountSN_SSN.Content = result.Data.Count();
                    foreach (var row in result.Data)
                    {
                        lstSN.Items.Add(row["kp"].ToString());
                        lstSSN.Items.Add(row["kp1"].ToString());
                    }
                }
                txtSN_SSN.SelectAll();
                txtSN_SSN.Focus();
            }
        }
        void addLstView(ListBox inlst)
        {
            Total = inlst.Items.Count;
            foreach (var item in inlst.Items)
            {
                lstView.Items.Add(item);
            }
        }
        private async void txtSN_SSN_KeyUp(object sender, KeyEventArgs e)
        {
            string inputData;
            if (e.Key == Key.Enter)
            {
                bool isSuccess = false;
                inputData = txtSN_SSN.Text;
                int index = 0;

                if (lstView.Items.Count == 0)
                {
                    if (lstSN.Items.Contains(inputData)) addLstView(lstSN);
                    else if (lstSSN.Items.Contains(inputData)) addLstView(lstSSN);
                    else
                    {
                        lblmessageSN_SSN.Content = KP + " SN/SSN NOT FOUND";
                        txtSN_SSN.Focus();
                        txtSN_SSN.SelectAll();
                        txtSN_SSN.Clear();
                    }
                    lstView.Items.Refresh();
                }

                foreach (var temp in lstView.Items)
                {
                    if (inputData == temp.ToString())
                    {
                        lstView.Items.Remove(temp);
                        lblCountSN_SSN.Content ="QTY : "+ lstView.Items.Count.ToString() + " / " + Total.ToString();
                        lblmessageSN_SSN.Content = "OK";
                        txtSN_SSN.Focus();
                        txtSN_SSN.SelectAll();
                        txtSN_SSN.Clear();
                        isSuccess = true;
                        break;
                    }
                    else
                    {
                        isSuccess = false;
                    }

                }

                if (!isSuccess)
                {
                    lblmessageSN_SSN.Content = KP + " SN/SSN NOT FOUND";
                    txtSN_SSN.Focus();
                    txtSN_SSN.SelectAll();
                    txtSN_SSN.Clear();
                }
                if (lstView.Items.Count == 0)
                {
                    RES = "OK";
                    lblCartonNoSN_SSN1.Content = "N/A";
                    this.Close();
                }
            }
        }


    }
}
