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
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using REPAIR.Models;
using Newtonsoft.Json;
using System.Data;
using MES.OpINI;
using System.Diagnostics;
using System.Windows.Threading;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for ItemCode.xaml
    /// </summary>
    public partial class RemoveKP : Window
    {
        public string sqlstr;
        public string EMP_NO, MACIP;
        public SfcHttpClient sfcHttpClient;
        DataTable dtTable;
        public string[] RESArray = { "NULL" };
        private char charSub = IniUtil.CharSub;
        public RemoveKP()
        {
            InitializeComponent();
        }



        private async void frmRemoveKP_Loaded(object sender, RoutedEventArgs e)
        {
            tbInput.Focus();
            sqlstr = string.Format(sqlStr.qryR108, tbInput.Text);
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlstr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                string json = JsonConvert.SerializeObject(result.Data);
                dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                gridKeypart.DataContext = dtTable;
                
            }
        }

        private  async void getDataKeypart()
        {
            tbInput.Text = tbInput.Text.Replace("'","");

            if (tbInput.Text != "")
            {
                sqlstr = string.Format(sqlStr.qryR108, tbInput.Text.ToUpper());
            }
            else
            {
                MessageBox.Show("Please Input Serial Number","Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlstr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() != 0)
            {
                string json = JsonConvert.SerializeObject(result.Data);
                dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                gridKeypart.DataContext = dtTable;
                gridKeypart.Items.Refresh();
                gridKeypart.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("The Serial number not found data keypart. Please intput data again", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            gridSetup.Height = 150;
        }

        private void tbInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                getDataKeypart();
            }
        }

        private void dgr_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = " " + e.PropertyName.ToUpper().Replace("_", " ") + "  ";
        }

  

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            gridSetup.Height = 0;
        }

        private void gridKeypart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
   
        }


        private async void btnDeleteKP_Click(object sender, RoutedEventArgs e)
        {
            if (gridKeypart.ItemsSource != null)
            {
                DataRowView row = gridKeypart.SelectedItem as DataRowView;

                string mess = "Delete " + tbInput.Text + " , keypart no: " + row.Row.ItemArray[0].ToString() + ", keypart sn: " + row.Row.ItemArray[1].ToString();
                MessageBoxResult dlr = MessageBox.Show(mess, "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (dlr == MessageBoxResult.Cancel)
                {
                    return;
                }
                if (await excuteRemoveKP("DELETE", tbInput.Text, row.Row.ItemArray[1].ToString(), row.Row.ItemArray[0].ToString()))
                {
                    return;
                    //MessageBox.Show(mess + ". Thành công !! ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } 
            else
            {
                MessageBox.Show("1.Vui lòng nhập dữ liệu !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tbInput.Focus();
                return;
            }
        }

        public async Task<Boolean> excuteRemoveKP (string FUN , string SN , string KeypartSN ,string KeypartNO)
        {
            var logInfo = new
            {
                OPTION = "REMOVE_KP",
                FUNTION = FUN,
                DATA = SN ,
                KP_SN = KeypartSN,
                KP_NO = KeypartNO,
                EMP = EMP_NO ,
                MAC = MACIP
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

            var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.REPAIR_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter {Name = "DATA" ,Value = jsonData, SfcParameterDataType = SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter {Name = "OUTPUT" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                }
            });
            if (result.Data != null)
            {
                dynamic output = result.Data;
                string RES = output[0]["output"];
                RESArray = RES.Split(charSub);
                if (RESArray[0] == "OK")
                {
                    MessageBox.Show(RESArray[1].ToString(), "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    var result1 = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = sqlstr,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (result.Data != null)
                    {
                        string json = JsonConvert.SerializeObject(result1.Data);
                        dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                        gridKeypart.DataContext = dtTable;
                        gridKeypart.Items.Refresh();
                        gridKeypart.SelectedIndex = 0;
                    }
                    else
                    {
                        gridKeypart.DataContext = null;
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show(RESArray[2].ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("SFIS1.REPAIR_API_EXECUTE / REMOVE_KP result null ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (gridKeypart.ItemsSource != null)
            {
                DataRowView row = gridKeypart.SelectedItem as DataRowView;

                string mess = "Delete All keypart of SN: " + tbInput.Text;
                MessageBoxResult dlr = MessageBox.Show(mess, "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (dlr == MessageBoxResult.Cancel)
                {
                    return;
                }
                if (await excuteRemoveKP("DELETE_ALL", tbInput.Text, row.Row.ItemArray[1].ToString(), row.Row.ItemArray[0].ToString()))
                {
                    return;
                    //MessageBox.Show(mess + ". Thành công !! ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("2.Vui lòng nhập dữ liệu !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tbInput.Focus();
                return;
            }

        }
    }
}
