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

namespace Make_Weight
{
    /// <summary>
    /// Interaction logic for SetupStation.xaml
    /// </summary>
    public partial class SetupStation : Window
    {
        public string strINIPath  = "C:\\PACKING\\PACKING.INI";
        public static string lang = "";
        private Ini ini;
        public SetupStation()
        {
            InitializeComponent();
            ini = new Ini(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\SFIS_AMS.INI");
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
                        try {
                            ini.IniWriteValue("MainSection", "LINE", cbbLineName.Text);
                        } catch { }
                    }
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
            this.Close();
        }
        public ObservableCollection<string> list = new ObservableCollection<string>();

        private async void StationSetup_Loaded(object sender, RoutedEventArgs e)
        {
            cbbLineName.IsEnabled = false;
            cbChangeLine.IsEnabled = false;
            IputPassword.IsEnabled = true;
            IputPassword.Password = "";


            var result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.SP_MAKEWEIGHT",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>()
                        {
                        new SfcParameter{Name="AP_VER",Value=MainWindow.apver,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_EMP",Value=MainWindow.empNo,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value="CTN_WEIGHT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_DATA",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_FUNC",Value="GET_LINE",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="CARTON",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                        }
            });
            dynamic ads = result.Data;
            string linename = ads[0]["res"];

            List<string> List = linename.Split('|').ToList();
            cbbLineName.ItemsSource = List;

        }

        private void IputPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ( (IputPassword.Password is null) || (IputPassword.Password  == "") )
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
