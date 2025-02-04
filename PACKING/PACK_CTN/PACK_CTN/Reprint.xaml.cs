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
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PACK_CTN.Models;
using System.Collections.ObjectModel;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for Reprint.xaml
    /// </summary>
    /// 

    public partial class Reprint : Window
    {

        // public SfcHttpClient _sfcHttpClient;
        public static String mCartonNo, CartonNo , MODEL_TYPE , label_name;
        public  MainWindow _frmmain = new MainWindow() ;
        public String KindLabel, table107 , strsql;
        public string[] _RESArray = { "NULL" };
        public string carton_no , _Check , colums , EMP_NO;
        string str , strSN;
        public int i, j, itemScanInput;
        public static bool CheckSN = false;
        public Reprint()
        {
            InitializeComponent();
        }

        private async Task<bool> BtnOK (string _JSONDATA)
        {
            try
            {
                var result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_CTN_PRINT_LABEL",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_JSONDATA,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                });
                if (result.Data != null)
                {
                    dynamic ads = result.Data;
                    string _RES = ads[0]["output"];
                    _RESArray = _RES.Split('#');

                    if (_RESArray[0] == "OK")
                    {
                        mCartonNo = _RESArray[2];
                        CartonNo  = _RESArray[1];
                        MODEL_TYPE = _RESArray[4];
                        if ( cbCartonLabel.IsChecked == true)
                        {
                            carton_no = _RESArray[1];
                        }
                        else
                        {
                            carton_no = _RESArray[2];
                        }
                      
                        MainWindow.MODEL_TYPE = _RESArray[4];
                        return true;
                    }
                    else
                    {
                        string _erro = _RESArray[2];
                        //MainWindow.FrmMessage.CustomFlag = true;
                        //FrmMessage.MessageEnglish = string.Format("change Mcarton erro : " + _erro, PasswordBox.Password);
                        //FrmMessage.MessageVietNam = string.Format("Thay đổi mã mcarton phát sinh lỗi  ", PasswordBox.Password);
                        //FrmMessage.ShowDialog();
                        return false;

                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Data null";
                    FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                    FrmMessage.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = ex.Message.ToString();
                FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                FrmMessage.ShowDialog();
                return false;
            }
        
        }
        private void CbCustCartonLabel_Click(object sender, RoutedEventArgs e)
        {
            listDataSN.Items.Clear();
            tbInputData.Clear();
            tbInputData.Focus();

            if (cbCustCartonLabel.IsChecked == true)
            {
                cbCartonLabel.IsChecked = false;
                KindLabel = "2";
            }
        }

        private void ComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        public ObservableCollection<string> list = new ObservableCollection<string>();
        private async void Reprint_Loaded(object sender, RoutedEventArgs e)
        {
            int i;
            for (i = 1; i <= 20 ; i++)
            {
                list.Add(i.ToString());
            }
            cbbLbQty.ItemsSource = list;
            cbbLbQty.SelectedIndex = 0;

            var result = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = "  select * from SFIS1.C_PARAMETER_INI  where prg_name  ='PACK_CTN' and vr_name  ='CUST_CARTON' and vr_value ='TRUE' ",
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count > 0)
            {
                cbCustCartonLabel.IsChecked = true;
                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK2", "CB_Carton_LB", "False");
                KindLabel = "2";
            }
            else
            {
                cbCartonLabel.IsChecked = MES.OpINI.IniUtil.ReadINI_B(MainWindow.strINIPath, "PACK2", "CB_Carton_LB", false);
            }

            cbbLbQty.Text = MES.OpINI.IniUtil.ReadINI(MainWindow.strINIPath, "CLabelQTY", "Default", "1");
            tbInputData.Focus();
            itemScanInput = MES.OpINI.IniUtil.ReadINI_Int(MainWindow.strINIPath, "PACK2", "Scan_Input", 0);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnTable_Click(object sender, RoutedEventArgs e)
        {
            dataGridsn.ItemsSource = null;
            listDataSN.Items.Clear();
            tbInputData.Clear();
            tbInputData.Focus();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
       
        }

        private async void BtnOK_Click(object sender, RoutedEventArgs e)
        {

            CheckSN = false;

            if ((cbCustCartonLabel.IsChecked == false) && (cbCartonLabel.IsChecked == false))
            {
                MessageBox.Show("Please choose the label kind", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                return;
            }
            if (tbInputData.Text == "")
            {
                if (itemScanInput == 1)
                {
                    MessageBox.Show("Please input serial number", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    return;
                }
                else if (itemScanInput == 3)
                {
                    
                    MessageBox.Show("Please input TRAY_NO", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBox.Show("Please input shipping sn", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    return;
                }
            }

            if (cbtabelZ.IsChecked == true)
            {
                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK2", "RB_Carton", "True");
                table107 = "SFISM4.Z107";
            }
            else
            {
                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK2", "RB_Carton", "False");
                table107 = "SFISM4.R107";
            }

            if (cbCartonLabel.IsChecked == true)
            {
                str = " CARTON_NO ";
            }
            else
            {
                str = " MCARTON_NO ";
            }

            var logInfo = new
            {
                OPTION = "PRINT_LABEL",
                DATA = tbInputData.Text,
                EMP = EMP_NO,
                TABLE = table107,
                ITEM = MainWindow.ITEMS_PRINT,
                MACIP = MainWindow.MACIP
                
            };

            string JsBtnOK = JsonConvert.SerializeObject(logInfo).ToString();

            try
            {
                var result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PACK_CTN_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=JsBtnOK,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                });
                if (result.Data != null)
                {
                    dynamic ads = result.Data;
                    string _RES = ads[0]["output"];
                    _RESArray = _RES.Split('#');

                    if (_RESArray[0] == "OK")
                    {
                        strSN = strSN + "," + tbInputData.Text;
                        if (cbCartonLabel.IsChecked == true)
                        {
                            carton_no = _RESArray[1];
                        }
                        else
                        {
                            carton_no = _RESArray[2];
                        }
                        //if (carton_no == tbInputData.Text && listDataSN.Items.Count == 0)
                        //{
                        //    CheckSN = true;
                        //}
                        tbInputData.Text = _RESArray[3];
                        MODEL_TYPE = _RESArray[4];
                        MainWindow.MODEL_TYPE = _RESArray[4];
                        MainWindow.LabelName = _RESArray[5];
                    }
                    else
                    {
                        if (_RESArray[1] =="ER")
                        {
                            MessageError FrmMessage = new MessageError();
                            FrmMessage.CustomFlag = false;
                            FrmMessage.errorcode = _RESArray[2];
                            FrmMessage.ShowDialog();
                            tbInputData.Focus();
                            tbInputData.SelectAll();
                            return;
                        }
                        else if (_RESArray[1] == "MS")
                        {
                            MessageError FrmMessage = new MessageError();
                            FrmMessage.CustomFlag = true;
                            FrmMessage.MessageEnglish = _RESArray[2];
                            FrmMessage.MessageVietNam = "In bù phát sinh lỗi!!";
                            FrmMessage.ShowDialog();
                            tbInputData.Focus();
                            tbInputData.SelectAll();
                            return;
                        }
                        else
                        {
                            MessageError FrmMessage = new MessageError();
                            FrmMessage.CustomFlag = true;
                            FrmMessage.MessageVietNam = "In bù phát sinh lỗi!!";
                            FrmMessage.MessageEnglish = _RESArray[2];
                            FrmMessage.ShowDialog();
                            tbInputData.Focus();
                            tbInputData.SelectAll();
                            return;
                        }
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Data null";
                    FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                    FrmMessage.ShowDialog();
                    tbInputData.Focus();
                    tbInputData.SelectAll();
                    return ;
                }
            }
            catch (Exception ex)
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = ex.Message.ToString();
                FrmMessage.MessageEnglish = "Call procedure have exceptions:";
                FrmMessage.ShowDialog();
                tbInputData.Focus();
                tbInputData.SelectAll();
                return ;
            }
            string sqlParamater = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='REPRINT_SCAN' AND VR_VALUE='YES' AND ROWNUM=1 ";
            var SnInCarton = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sqlParamater,
                SfcCommandType = SfcCommandType.Text
            });

            if ( SnInCarton.Data != null && SnInCarton.Data.Count() > 0 && cbtabelZ.IsChecked == false) 
            {
                //lvListSN.Items.Clear()

                #region check sn reprint old format
                /*
                if (cbCartonLabel.IsChecked == true)
                {
                    str = " CARTON_NO ";
                }
                else
                {
                    str = " MCARTON_NO ";
                }

                var _SnInCarton = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = "select distinct SHIPPING_SN ,EMP_NO from  " + table107  
                                 +" WHERE " + str + " = '"+ carton_no +"' ",
                    SfcCommandType = SfcCommandType.Text
                });

                if (_SnInCarton.Data != null)
                {
                    i = _SnInCarton.Data.Count();
                    j = 0;
                    List<ReprintListSN> items = new List<ReprintListSN>();
                    foreach (var row in _SnInCarton.Data)
                    {
                        if (row["emp_no"].ToString() == "")
                        {
                            _Check = "";
                        }
                        else
                        {
                            if (row["emp_no"].ToString() == "REPRINT")
                            {
                                _Check = "V";
                                j = j + 1;
                            }
                            else
                            {
                                _Check = "";
                            }
                        }
                        items.Add(new ReprintListSN() { SHIPPING_SN = row["shipping_sn"].ToString(), CHECK = _Check });
                    }
                    dataGridsn.ItemsSource = items;
                    if (i == j)
                    {
                        String _SQL = "UPDATE  " + table107
                                      + " SET EMP_NO ='"+MainWindow.empNo+"' WHERE " + str + " = '" + carton_no + "' ";
                        var _Result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = _SQL
                        });
                        CheckSN = true;
                    }
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = " ";
                    FrmMessage.MessageEnglish = "Query SN by carton no error :";
                    FrmMessage.ShowDialog();
                    return;
                } */
                #endregion

                if (listDataSN.Items.Count == 0)
                {
                    
                    if (itemScanInput == 1)
                    {
                        colums = "serial_number";
                    }
                    else if (itemScanInput == 2)
                    {
                        colums = "shipping_sn";
                    }
                    else if (itemScanInput == 3)
                    {
                        colums = "tray_no";
                    }
                    else
                    {
                        colums = "mac2";
                    }

                    if (itemScanInput == 4)
                    {
                        strsql = "select distinct " + colums + " from  SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER IN ( SELECT SERIAL_NUMBER FROM   " + table107
                                       + " WHERE " + str + " = '" + carton_no + "' ) ";
                    }
                    else
                    {
                        strsql = "select distinct " + colums + " from  " + table107
                                       + " WHERE " + str + " = '" + carton_no + "' ";
                    }
                        
                    var _SnInCarton = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strsql ,
                        SfcCommandType = SfcCommandType.Text
                    });

                    if (_SnInCarton.Data != null)
                    {
                        i = _SnInCarton.Data.Count();
                        j = 0;
                        listDataSN.Items.Clear();

                        foreach (var row in _SnInCarton.Data)
                        {
                            listDataSN.Items.Add(row[colums].ToString());
                        }
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = " ";
                        FrmMessage.MessageEnglish = "Query SN by carton no error :";
                        FrmMessage.ShowDialog();
                        return;
                    }
                }

                if (listDataSN.Items.IndexOf(tbInputData.Text) >= 0)
                {
                    j = j + 1;
                    listDataSN.Items.RemoveAt(listDataSN.Items.IndexOf(tbInputData.Text));
                    listDataSN.Items.Insert(0, new ListBoxItem { Content = tbInputData.Text + "            V", Foreground = Brushes.Green  });
              
                   //listDataSN.Style.Setters.Add(listDataSN.Background.)
                }
               
                if (i==j) // check du so luong  sn
                {
                    CheckSN = true;
                }
            }
            else
            {
                CheckSN = true;
            }
            

            if (CheckSN)
            {
                dataGridsn.ItemsSource = null;
                strsql = "SELECT distinct lower( b.CARTON_LAB_NAME) label_name FROM " + table107 + " A ,SFIS1.C_CUST_SNRULE_T B WHERE  A." + str + " = '" + carton_no + "' " +
                          " AND A.MODEL_NAME  = B.MODEL_NAME  AND A.VERSION_CODE = B.VERSION_CODE";
                var labelName = await MainWindow._sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel {
                    CommandText = strsql,
                    SfcCommandType = SfcCommandType.Text
                });

                if (labelName.Data != null && labelName.Data.Count() > 0)
                {
                    label_name = labelName.Data["label_name"].ToString();
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = "Reprint error";
                    FrmMessage.MessageEnglish = "Can not get label name , " + str +  " : " + carton_no;
                    FrmMessage.ShowDialog();
                    tbInputData.SelectAll();
                    tbInputData.Focus();
                    return;
                }

                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "CLabelQTY", "Default", cbbLbQty.Text );

                string action_desc = carton_no + "; MAC: " + MainWindow.MACIP  + "; IP:" + MainWindow.IP;

                if ( await _frmmain.Reprint(carton_no, table107, label_name , str))
                {
                    String sql = " Insert into SFISM4.R_SYSTEM_PRGLOG_T (EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                            + " Values ('" + EMP_NO + "', 'PACK_CTN', 'REPRINT', '" + action_desc + "') ";
                    var _insert = MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = sql
                    });
                }
                listDataSN.Items.Clear();
                tbInputData.Clear();
                tbInputData.Focus();
                this.Close();
            }
            else
            {
                tbInputData.SelectAll();
                tbInputData.Focus();
            }
 
        }


        //private async void ReprintCheckSNinCarton(string carton)
        //{
        //    //lvListSN.Items.Clear();
        //    string str;
        //    if (cbCartonLabel.IsChecked == true)
        //    {
        //        str = " CARTON_NO ";
        //    }
        //    else
        //    {
        //        str = " MCARTON_NO ";
        //    }

        //    string sql_str = "select distinct SHIPPING_SN ,EMP_NO from  sfism4.r_wip_tracking_t "
        //        + " WHERE " + str + " = '" + carton + "' ";
        //    var SnInCarton = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
        //    {
        //        CommandText = sql_str,
        //        SfcCommandType = SfcCommandType.Text
        //    });

        //    if (SnInCarton.Data != null)
        //    {
        //        i = SnInCarton.Data.Count();
        //        j = 0;
        //        List<ReprintListSN> items = new List<ReprintListSN>();
        //        foreach (var row in SnInCarton.Data)
        //        {
        //            if (row["emp_no"] is null )
        //            {
        //                _Check = "";
        //            }
        //            else
        //            {
        //                if (row["emp_no"].ToString() == "REPRINT")
        //                {
        //                    _Check = "V";
        //                    j = j + 1;
        //                }
        //            }
        //            items.Add(new ReprintListSN() { SHIPPING_SN = row["shipping_sn"].ToString() , CHECK = _Check });
        //        }
        //        dataGridsn.ItemsSource = items;
        //        if (i==j)
        //        {
        //            String _SQL = " UPDATE SFISM4.R107 SET EMP_NO ='' WHERE MCARTON_NO = '"+carton+"' OR CARTON_NO ='"+carton+"' ";
        //            var _Result = await MainWindow._sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
        //            {
        //                CommandText = _SQL
        //            });
        //            CheckSN = true;
        //            return ;
        //        }
        //    }
        //    else
        //    {
        //        MessageError FrmMessage = new MessageError();
        //        FrmMessage.CustomFlag = true;
        //        FrmMessage.MessageVietNam = " ";
        //        FrmMessage.MessageEnglish = "Query SN by carton no error :";
        //        FrmMessage.ShowDialog();
        //        return;
        //    }
        //}
        private void BtnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TbInputData_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOK_Click(sender, e);
            }
        }

        private void DataGridsn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CbCartonLabel_Click(object sender, RoutedEventArgs e)
        {
            listDataSN.Items.Clear();
            tbInputData.Clear();
            tbInputData.Focus();

            if (cbCartonLabel.IsChecked == true)
            {
                cbCustCartonLabel.IsChecked = false;
                KindLabel = "1";
                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK2", "CB_Carton_LB", "True");
            }
            else
            {
                MES.OpINI.IniUtil.WriteINI(MainWindow.strINIPath, "PACK2", "CB_Carton_LB", "False");
            }
        }
      
    }
}
