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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Microsoft.Win32;
using System.IO;
using Path = System.IO.Path;
using REWORK.Models;
using REWORK.ViewModel;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using Sfc.Library.HttpClient.Helpers;

namespace REWORK.UserControlRework
{
    /// <summary>
    /// Interaction logic for ucCutInFW.xaml
    /// </summary>
    public partial class ucCutInFW : UserControl
    {
        public string[] RESArray = { "NULL" };
        string D_MO, T_MO;
        public ucCutInFW()
        {
            InitializeComponent();
        }

        private void tbModelName_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private async void BtnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (txtModelName.Text == "")
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Model Name is null, Please input Model Name ";
                FrmMessage.MessageEnglish = "Nhập tên hàng trước khi Query";
                FrmMessage.ShowDialog();
                return;
            }
            MessageBoxResult dlr = MessageBox.Show("Model need cut in FW " + txtModelName.Text, "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (dlr == MessageBoxResult.Cancel)
            {
                txtModelName.SelectAll();
                txtModelName.Focus();
                return;
            }
            if (txtMoNumberT.Text == "")
            {
                txtMoNumberT.Text = "1";
            }

            if (txtMoNumberD.Text =="")
            {
                txtMoNumberD.Text = "1";
            }

            T_MO = "'" + txtMoNumberT.Text.Replace(",", "','") + "'";
            D_MO = "'" + txtMoNumberD.Text.Replace(",", "','") + "'";
         
   
            var RouteName = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT * FROM SFIS1.C_ROUTE_NAME_T WHERE SUBSTR(ROUTE_NAME,1,LENGTH('"+txtModelName.Text+"')) ='"+txtModelName.Text+"' ",
                SfcCommandType = SfcCommandType.Text
            });
            if (RouteName.Data.Count() > 0)
            {
                foreach (var row in RouteName.Data)
                {
                    if (cbbRouteName.Items.IndexOf(row["route_name"]) < 0 )
                    cbbRouteName.Items.Add(row["route_name"]);
                }
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Query route name result data null ";
                FrmMessage.MessageEnglish = "Query Error !!";
                FrmMessage.ShowDialog();
                return;
            }

            gridLoading.Visibility = Visibility.Visible;
            var result = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT A.SERIAL_NUMBER,a.mo_number,a.shipping_sn,a.shipping_sn2,A.MODEL_NAME,A.GROUP_NAME,A.WIP_GROUP FROM SFISM4.R107 A " +
                          " WHERE a.model_name = '"+txtModelName.Text+"' " +
                          " AND a.MO_NUMBER  not in ("+T_MO+") " +
                          " AND a.serial_number NOT IN(SELECT substr(b.serial_number, 2) FROM sfism4.r_wip_tracking_t b " +
                          " WHERE b.serial_number = '@' || a.serial_number) " +
                          " AND a.serial_number NOT IN(SELECT substr(b.serial_number, 2) FROM sfism4.r_wip_tracking_t@sfcodbh b " +
                          " WHERE b.serial_number = '@' || a.serial_number) " +
                          " AND LENGTH(a.serial_number) = '7' " +
                          " AND a.scrap_flag = '0' ",
                SfcCommandType = SfcCommandType.Text
            });
            gridLoading.Visibility = Visibility.Hidden;
            if (result.Data.Count() > 0 )
            {
                 var listCamera = result.Data.ToListObject<ViewModel.dataQuery>();
                 gridData.ItemsSource = listCamera;
                 
            }
            else
            {
                
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "Query result data null ";
                FrmMessage.MessageEnglish = "Nhập tên hàng trước khi Query";
                FrmMessage.ShowDialog();
                return;
            }
            lblQTY.Content = ( gridData.Items.Count -1).ToString();
         

            var result1 = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = " SELECT SERIAL_NUMBER,MO_NUMBER,SHIPPING_SN,SHIPPING_SN2,MO_NUMBER,MODEL_NAME,GROUP_NAME,WIP_GROUP FROM SFISM4.R107 WHERE " +
                              " SERIAL_NUMBER IN(SELECT '@' || SERIAL_NUMBER FROM SFISM4.R107 WHERE MODEL_NAME = '"+txtModelName+"' " +
                              " AND MO_NUMBER not in ("+ T_MO + ") AND WIP_GROUP = 'STOCKIN' ) AND MO_NUMBER NOT IN("+D_MO+") AND WIP_GROUP <> 'FG' " , 
                SfcCommandType = SfcCommandType.Text
            });

            if (result1.Data.Count() > 0 )
            {
                var listCamera = result1.Data.ToListObject<ViewModel.dataQuery>();
                gridDataSI.ItemsSource = listCamera;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            // Export data SMT
            List<dataQuery> datalist = gridData.ItemsSource as List<dataQuery>;
            if (datalist != null)
                ExportData(datalist,"SMT");

            // Export data SI
            List<dataQuery> datalistSI = gridDataSI.ItemsSource as List<dataQuery>;
            if (datalistSI != null )
            ExportData(datalistSI ,"SI");
        }

        private void ExportData(List<dataQuery> list ,string fileName)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("SERIAL_NUMBER");
            dt.Columns.Add("MO_NUMBER");
            dt.Columns.Add("SHIPPING_SN");
            dt.Columns.Add("SHIPPING_SN2");
            dt.Columns.Add("MODEL_NAME");
            dt.Columns.Add("GROUP_NAME");
            dt.Columns.Add("WIP_GROUP");
            foreach (dataQuery items in list)
            {
                dt.Rows.Add(items.SERIAL_NUMBER,items.MO_NUMBER, items.SHIPPING_SN,items.SHIPPING_SN2,items.MODEL_NAME,items.GROUP_NAME, items.WIP_GROUP);
            }
            dt.TableName = "DATA";

            string PathSaveFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString() + "\\CutinFW\\";
  
            if (!System.IO.Directory.Exists(PathSaveFile))
                System.IO.Directory.CreateDirectory(PathSaveFile);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save file after Excute";
            sfd.Filter = "Excel Files |*.xls";
            sfd.FileName =  txtModelName.Text + "_" + fileName;
            sfd.InitialDirectory = PathSaveFile;
            Nullable<bool> result = sfd.ShowDialog();
            if (result == true)
            {
                try
                {
                    dt.WriteXml(sfd.FileName);
                    MessageBox.Show("Export Successfully", "Notice");
                    btnExecute.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private async void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            var logInfo = new
            {
                OPTION = "CUTIN_FW_EXECUTE",
                MODEL = txtModelName.Text,
                MO1 = D_MO ,
                MO2 = T_MO ,
                ROUTE = cbbRouteName.Text 
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            try
            {
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
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
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "OK")
                    {
                        MessageBox.Show("CUTIN FW SUCCESS !!", "Information", MessageBoxButton.OK);
                        ucCutinFWRefresh();
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "SFIS1.REWORK_API_EXECUTE / CUTIN_FW_EXECUTE ";
                        FrmMessage.MessageEnglish = RESArray[1];
                        FrmMessage.ShowDialog();
                        return;
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "SFIS1.REWORK_API_EXECUTE / CUTIN_FW_EXECUTE  : Result data null";
                    FrmMessage.MessageEnglish = "ERROR" ;
                    FrmMessage.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "SFIS1.REWORK_API_EXECUTE / CUTIN_FW_EXECUTE have exception";
                FrmMessage.MessageEnglish = ex.Message.ToString(); 
                FrmMessage.ShowDialog();
                return;
            }
        }

        private void Card_KeyDown(object sender, KeyEventArgs e)
        {
           btnExecute.IsEnabled = false;
        }

        private void TxtModelName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ucCutinFWRefresh();
        }

        private void ucCutinFWRefresh ()
        {
            //txtModelName.Text = "";
            txtMoNumberD.Text = "";
            txtMoNumberT.Text = "";
            cbbRouteName.Items.Clear();
            gridData.ItemsSource = null;
            btnExecute.IsEnabled = false;
            gridDataSI.ItemsSource = null;
            lblQTY.Content = "0";
        }
    } 
}
