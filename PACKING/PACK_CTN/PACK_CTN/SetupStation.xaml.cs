using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SetupStation.xaml
    /// </summary>
    public partial class SetupStation : Window
    {
        public string strINIPath  = "C:\\PACKING\\PACKING.INI";
        public SetupStation()
        {
            InitializeComponent();
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbChangeLine.IsChecked == true)
            {
                var _C_Line = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "Select line_Name  From SFIS1.C_Line_Desc_T where line_name ='" + cbbLineName.Text + "' ",
                    SfcCommandType = SfcCommandType.Text
                });
                if (_C_Line.Data != null)
                {
                    if (cbChangeLine.IsChecked == true)
                    {
                        MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LINE_CHANGE", "TRUE");
                    }
                    else
                    {
                        MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LINE_CHANGE", "FALSE");
                    }
                    MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK2", "LINE", cbbLineName.Text);
                    Close();
                }
                else
                {
                    MessageBox.Show("Line Name '" + cbbLineName.Text + "' not found , Can not setup this line name ", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    cbbLineName.Focus();
                    return;
                }
            }
            else
            {
                this.Close();
            }
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            string LineName = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK2", "LINE", "");
            if (LineName != "")
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Not setup line name can not close!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public ObservableCollection<string> list = new ObservableCollection<string>();

        private async void StationSetup_Loaded(object sender, RoutedEventArgs e)
        {
            cbbLineName.IsEnabled = false;
            cbChangeLine.IsEnabled = false;
            IputPassword.IsEnabled = true;
            IputPassword.Password = "";
            
        }

        private async void IputPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ( (IputPassword.Password == null) || (IputPassword.Password  == "") )
                {
                    MessageBox.Show("Please input pass ", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    IputPassword.Focus();
                    return;
                }
                if (IputPassword.Password == "AMBITCTN")
                {
                    cbbLineName.IsEnabled = true;
                    cbChangeLine.IsEnabled = true;
                    IputPassword.IsEnabled = false;

                    var _R107 = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = " Select line_Name  From SFIS1.C_Line_Desc_T order by line_name ",
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (_R107.Data != null)
                    {
                        int i = 0;
                        foreach (var row in _R107.Data)
                        {
                            list.Add(row["line_name"].ToString());
                        }
                        cbbLineName.ItemsSource = list;
                    }
                    string LineName = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK2", "LINE", "");
                    if (LineName != "")
                    {
                        cbbLineName.Text = LineName;
                    }
                }
                else
                {
                    MessageBox.Show("Pass not right , Input pass again !!", "ERROR", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    IputPassword.Focus();
                    return;
                }
            }
        }

        private void CbbLineName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key ==  Key.Enter)
            {
                btnOK_Click(sender,e);
            }
        }
    }
}
