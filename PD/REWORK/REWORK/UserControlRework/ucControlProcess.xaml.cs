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
    /// Interaction logic for ucControlProcess.xaml
    /// </summary>
    public partial class ucControlProcess : UserControl
    {
        public string[] RESArray = { "NULL" };
        public string[] listSN = { "NULL" };
        string RouteCode;
        public ucControlProcess()
        {
            InitializeComponent();
        }

        private async void txtInputSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtInputSN.Text == "")
            {
                if (await checkSN(txtInputSN.Text))
                {
                    if (lstvSN_OK.Items.IndexOf(txtInputSN.Text) < 0)
                    {
                        lstvSN_OK.Items.Add(txtInputSN.Text);
                    }
                    else
                    {
                        lstvSN_NG.Items.Add(txtInputSN.Text + " Dup");
                    }
                }

                lblQtyOK.Content = lstvSN_OK.Items.Count.ToString();
                lblQtyNG.Content = lstvSN_NG.Items.Count.ToString();

                if (lstvSN_OK.Items.Count > 0)
                {
                    if (!await getRouteName(cbbRouteName.Text))
                    {
                        return;
                    }
                }
            }
        }

        private async void OpenFileSN_Click(object sender, MouseButtonEventArgs e)
        {
             OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                   txtSN.Text = Path.GetFileName(openFileDialog.FileNames.ToString());
                string[] dataSN = File.ReadAllLines(openFileDialog.FileName.ToString());
                if (dataSN != null)
                {
                    foreach (string sn in dataSN)
                    {
                        if (await checkSN(sn) )
                        {
                            if(lstvSN_OK.Items.IndexOf(sn) < 0)
                            {
                                lstvSN_OK.Items.Add(sn);
                            }
                            else
                            {
                                lstvSN_NG.Items.Add(sn + " Dup");
                            }
                        }
                    }

                    lblQtyOK.Content = lstvSN_OK.Items.Count.ToString();
                    lblQtyNG.Content = lstvSN_NG.Items.Count.ToString();
                    if (! await getRouteName(cbbRouteName.Text))
                    {
                        return;
                    }


                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "File no found data !";
                    frmMessage.MessageVietNam = "File không có dữ liệu !";
                    frmMessage.ShowDialog();
                }
            }
        }
        private async Task<Boolean> checkSN(String data)
        {
            var logInfo = new
            {
                OPTION = "TPROCESS_INPUTSN",
                DATA = data
            };
            string jsondata = JsonConvert.SerializeObject(logInfo).ToString();
            var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.REWORK_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter {Name="DATA" , Value =jsondata ,SfcParameterDataType = SfcParameterDataType.Varchar2,SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter {Name="OUTPUT" ,SfcParameterDataType= SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Output}
                }
            });

            if (result.Data != null)
            {
                dynamic output = result.Data;
                String Res = output[0]["output"];
                RESArray = Res.Split('#');
                if (RESArray[0] == "OK")
                {
                    if (cbbModelName.Items.Count > 0)
                    {
                        if (cbbModelName.Items.IndexOf(RESArray[1]) < 0)
                        {
                            cbbModelName.Items.Add(data + " Different model name !!" );
                            return false;
                        }
                    }
                    else
                    {
                        cbbModelName.Items.Add(RESArray[1]);
                    }
                    cbbModelName.SelectedIndex = 0;
                    return true;
                }
                else
                {
                    lstvSN_NG.Items.Add(data + " " +  RESArray[1]);
                    return false;
                }
            }
            else
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = "Call produe error !";
                frmMessage.MessageVietNam = "File không có dữ liệu !";
                frmMessage.ShowDialog();
                return false;
            }
        }

        private async Task<Boolean> getRouteName(String MODEL)
        {
            int i;
            var logInfo = new
            {
                OPTION = "TPROCESS_GETROUTENAME",
                DATA = MODEL
            };
            string jsondata = JsonConvert.SerializeObject(logInfo).ToString();
            var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
            {
                CommandText = "SFIS1.REWORK_API_EXECUTE",
                SfcCommandType = SfcCommandType.StoredProcedure,
                SfcParameters = new List<SfcParameter>
                {
                    new SfcParameter {Name="DATA" , Value =jsondata ,SfcParameterDataType = SfcParameterDataType.Varchar2,SfcParameterDirection = SfcParameterDirection.Input },
                    new SfcParameter {Name="OUTPUT" ,SfcParameterDataType= SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Output}
                }
            });

            if (result.Data != null)
            {
                dynamic output = result.Data;
                String Res = output[0]["output"];
                RESArray = Res.Split('#');
                if (RESArray[0] == "OK")
                {
                    for (i = 0; i < int.Parse(RESArray[1]) ; i++)
                    {
                        if (cbbRouteName.Items.IndexOf(RESArray[2 + i]) < 0)
                            cbbRouteName.Items.Add(RESArray[2 + i]);
                    }
                    return true;
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "REWORK_API_EXECUTE /TPROCESS_GETROUTENAME ";
                    frmMessage.MessageVietNam = RESArray[1];
                    frmMessage.ShowDialog();
                    return false;
                }
            }
            else
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = "REWORK_API_EXECUTE /TPROCESS_GETROUTENAME Result data null  !";
                frmMessage.MessageVietNam = "GET ROUTE NAME ERROR !";
                frmMessage.ShowDialog();
                return false;
            }
        }

        private async void txtInputMO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var result = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = " SELECT SERIAL_NUMBER  , MO_NUMBER , SHIPPING_SN  , SHIPPING_SN2 , MODEL_NAME  , GROUP_NAME  , WIP_GROUP ,SPECIAL_ROUTE FROM SFISM4.R107 A  " +
                                  " WHERE a.MO_NUMBER =  '"+txtInputMO.Text+"' " +
                                  " AND a.serial_number NOT IN(SELECT substr(b.serial_number, 2) FROM sfism4.r_wip_tracking_t b " +
                                  " WHERE b.serial_number = '@' || a.serial_number) " +
                                  " AND a.serial_number NOT IN(SELECT substr(b.serial_number, 2) FROM sfism4.r_wip_tracking_t@sfcodbh b " +
                                  " WHERE b.serial_number = '@' || a.serial_number) " +
                                  " AND LENGTH(a.serial_number) = '7' " +
                                  " AND a.scrap_flag = '0' ",
                    SfcCommandType = SfcCommandType.Text
                });

                if (result.Data.Count() > 0)
                {
                    var listCamera = result.Data.ToListObject<dataQuery>();
                    gridData.ItemsSource = listCamera;

                    foreach (var row in result.Data)
                    {
                        if (await checkSN(row["serial_number"].ToString() ))
                        {
                            if (lstvSN_OK.Items.IndexOf(row["serial_number"]) < 0)
                            {
                                lstvSN_OK.Items.Add(row["serial_number"]);
                            }
                            else
                            {
                                lstvSN_NG.Items.Add(row["serial_number"] + " Dup");
                            }
                        }
                        lblQtyOK.Content = lstvSN_OK.Items.Count.ToString();
                        lblQtyNG.Content = lstvSN_NG.Items.Count.ToString();
                        Prob1.Value = (lstvSN_OK.Items.Count) * 100 / gridData.Items.Count ;
                    }
       
                    if (!await getRouteName(cbbModelName.Text))
                    {
                        return;
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Query result data null ";
                    FrmMessage.MessageEnglish = "Check mo input : " + txtInputMO.Text;
                    FrmMessage.ShowDialog();
                    return;
                }
            }
        }
        private async void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            var result = await MainWindow.sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "SELECT DISTINCT ROUTE_CODE FROM SFIS1.C_ROUTE_CONTROL_T WHERE GROUP_NEXT LIKE 'ST_SMT%' " + 
                     "  AND ROUTE_CODE IN ( SELECT ROUTE_CODE FROM SFIS1.C_ROUTE_NAME_T WHERE ROUTE_NAME = '"+cbbRouteName.Text+"' ) " ,
                SfcCommandType = SfcCommandType.Text
            });

            if (result.Data.Count() > 0)
            {
                RouteCode = result.Data["ROUTE_CODE"].ToString();
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = " Luu trinh chon khong co tram ST_SMT ";
                FrmMessage.MessageEnglish = " Route name selected error ! ";
                FrmMessage.ShowDialog();
                return;
            }
            while(lstvSN_OK.Items.Count > 0)
            {
                var Excute = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = " UPDATE SFISM4.R_WIP_TRACKING_T SET SPECIAL_ROUTE='"+ RouteCode + "'   WHERE SERIAL_NUMBER = '"+lstvSN_OK.Items[0].ToString()+"' ",
                    SfcCommandType = SfcCommandType.Text
                });
                if (Excute.Result == "OK")
                {
                    lstvSN_OK.Items.RemoveAt(0);
                }
                else
                {
                    lstvSN_NG.Items.Add(lstvSN_OK.Items[0]);
                    lstvSN_OK.Items.RemoveAt(0);
                }
                lblQtyOK.Content = lstvSN_OK.Items.Count;
                lblQtyNG.Content = lstvSN_NG.Items.Count;
            }
            gridData.ItemsSource = null;
        }

        private void ControlProcessRefresh ()
        {
            lblQtyNG.Content = "0";
            lblQtyOK.Content = "0";
            lstvSN_NG.Items.Clear();
            lstvSN_OK.Items.Clear();
            gridData.ItemsSource = null;
            txtInputMO.Text = "";
            txtInputSN.Text = "";
            cbbModelName.Items.Clear();
            cbbRouteName.Items.Clear();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            List<dataQuery> datalistSI = gridData.ItemsSource as List<dataQuery>;
            if (datalistSI != null)
                ExportData(datalistSI, "SI");
        }
        private void ExportData(List<dataQuery> list, string fileName)
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
                dt.Rows.Add(items.SERIAL_NUMBER, items.MO_NUMBER, items.SHIPPING_SN, items.SHIPPING_SN2, items.MODEL_NAME, items.GROUP_NAME, items.WIP_GROUP);
            }
            dt.TableName = "DATA";

            string PathSaveFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString() + "\\CutinFW\\";

            if (!System.IO.Directory.Exists(PathSaveFile))
                System.IO.Directory.CreateDirectory(PathSaveFile);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save file after Excute";
            sfd.Filter = "Excel Files |*.xls";
            sfd.FileName = cbbModelName.Text + "_" + fileName;
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

    }
}
