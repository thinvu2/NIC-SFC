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
using System.Windows.Threading;
using System.Diagnostics;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for frmModify.xaml
    /// </summary>
    public partial class frmModify : Window
    {
        public static string ReasonCode, ReasonDesc ,ItemCode;
        public string FLAG, SN, MODEL_NAME , sqlstr , M_iFlag , SECTION , lbGroup , sRepairType, REPAIR_GROUP ,LINE_NAME , tReasonCode="",
            F_ROWID;
        public SfcHttpClient sfcHttpClient;
        DataTable dtTable;
        public string[] RESArray = { "NULL" };
        private char charSub = IniUtil.CharSub;
        public frmModify()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        }



        private async void modify_Loaded(object sender, RoutedEventArgs e)
        {
            tbDC.IsEnabled = true;
            tbLC.IsEnabled = true;
            sqlstr = string.Format(sqlStr.qryTopReason, lblErrorCode.Content.ToString(),MODEL_NAME);
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel 
            {
                CommandText = sqlstr ,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0 )
            {
                string json = JsonConvert.SerializeObject(result.Data);
                dtTable = JsonConvert.DeserializeObject<DataTable>(json);

                gridDataReason.DataContext = dtTable;
            }

            sqlstr = string.Format(sqlStr.qryTableDuty);
            result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlstr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                foreach (var row in result.Data)
                {
                    cbbDutyDesc.Items.Add(row["duty_desc"]);
                    cbbDuty.Text = row["duty_type"].ToString();
                }
            }
            tblFixTime.Text = DateTime.Now.ToString();
            tbFixReasonCode.Text = tReasonCode;
            tbFixReasonCode.SelectAll();
            tbFixReasonCode.Focus();
        }

        private async void cbbDutyDesc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbDutyDesc.SelectedValue != null)
            {
                sqlstr = string.Format(sqlStr.qryDuty, cbbDutyDesc.SelectedValue.ToString());
                /*
                var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlstr,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null && result.Data.Count() > 0)
                {
                    foreach (var row in result.Data)
                    {
                        tblDutyType.Text = row["duty_type"].ToString();
                    }
                }
                */
                //Hau sua
                var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlstr,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null)
                {
                    tblDutyType.Text = result.Data["duty_type"]?.ToString() ?? "";
                }
            }
        }

        private void Modifyfrm_Keydown(object sender, KeyEventArgs e)
        {
           

            _altModifierPressed = (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));

            if (_altModifierPressed && Keyboard.IsKeyDown(Key.O) && btnOK.IsEnabled == true)
            {
                btnOK_Click(sender, e);
            }

            if (_altModifierPressed && Keyboard.IsKeyDown(Key.E))
            {
                btnCancel_Click(sender, e);
            }

            if (_altModifierPressed && Keyboard.IsKeyDown(Key.M))
            {
                tbTempAllpart.Text = "";
                chkbChangeMaterial.IsChecked = !chkbChangeMaterial.IsChecked;
                if (chkbChangeMaterial.IsChecked == true)
                {
                    btnOK.IsEnabled = false;
                }
                else
                {
                    btnOK.IsEnabled = true;
                }
            }
        }

        private bool _altModifierPressed = false;

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private  void TblLocation_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Location_Keydown();
            }
        }
        public async void Location_Keydown ()
        {
           
            tbDC.IsEnabled = true;
            string infor = await getAllpartInfor(SN, tblLocation.Text, "NULL");
            if (infor == "")
            {
                cbbVender.Text = "";
                tbLC.Text = "";
                tbDC.Text = "";
                cbbVender.IsEnabled = false;
                  tbLC.IsEnabled = false;
                  tbDC.IsEnabled = false;
              
            }
            else
            {
                RESArray = infor.Split(charSub);
                cbbVender.IsEnabled = true;
                tbLC.IsEnabled = true;
                tbDC.IsEnabled = true;
                cbbVender.Text = RESArray[1].ToString();
                tbLC.Text = RESArray[2].ToString();
                tbDC.Text = RESArray[3].ToString();
                //tbTempAllpart.IsEnabled = true;
                tbTempAllpart.SelectAll();
                tbTempAllpart.Focus();
            }
            
        }

        private async void TbTempAllpart_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string infor = await getAllpartInfor(SN, tblLocation.Text, tbTempAllpart.Text);
                if (infor=="")
                {
                    tbNewVendor.Text = "";
                    tbNewLC.Text = "";
                    tbNewDC.Text = "";
                    tbNewVendor.IsEnabled = false;
                    tbNewLC.IsEnabled = false;
                    tbNewDC.IsEnabled = false;
                    tbTempAllpart.SelectAll();
                    tbTempAllpart.Focus();
                }
                else
                {
                    btnOK.IsEnabled = true;
                    RESArray = infor.Split(charSub);
                    tbNewVendor.IsEnabled = true;
                    tbNewLC.IsEnabled = true;
                    tbNewDC.IsEnabled = true;
                    tbNewVendor.Text = RESArray[1].ToString();
                    tbNewLC.Text = RESArray[2].ToString();
                    tbNewDC.Text = RESArray[3].ToString();
                    tbPartNumber.Text = RESArray[4].ToString();
                }
            }
        }

        private void chkbChangeMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (chkbChangeMaterial.IsChecked == true)
            {
                btnOK.IsEnabled = false;
            }
            else
            {
                btnOK.IsEnabled = true;
            }
        }

        private void ChkbtypeDCLC_Click(object sender, RoutedEventArgs e)
        {
            if (chkbtypeDCLC.IsChecked == true)
            {
                frmCheckEmp checkEmp = new frmCheckEmp();
                checkEmp.sfcHttpClient = sfcHttpClient;
                checkEmp.ShowDialog();
                if (!checkEmp.returnCheck)
                {
                   
                    tbDC.IsEnabled = false;
                    tbLC.IsEnabled = false;
                    chkbtypeDCLC.IsChecked = false;
                    return;
                   
                }
                else
                {
                    string empppp = checkEmp.empNoIpqc;
                    tbDC.IsEnabled = true;
                    tbLC.IsEnabled = true;
                }
            }
            else
            {
              //  chkbtypeDCLC.IsChecked = true;
                tbDC.IsEnabled = false;
                tbLC.IsEnabled = false;
            }

           
        }

        public async Task<string> getAllpartInfor (string SN , string DATA1 , string DATA2)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "ALLPART_INFOR",
                    DATA = SN,
                    LOCATION = DATA1,
                    ALLPART_LB = DATA2
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
                        return RES;
                    }
                    else
                    {
                        if (RESArray[1] == "MS")
                        {
                            showError("SFIS1.REPAIR_API_EXECUTE / ALLPART_INFOR", RESArray[2], true);
                            return "";
                        }
                        else
                        {
                            showError("SFIS1.REPAIR_API_EXECUTE / ALLPART_INFOR", RESArray[2], false);
                            return "";
                        }
                    }
                }
                else
                {
                    showError("SFIS1.REPAIR_API_EXECUTE / ALLPART_INFOR", " result data null ", true);
                    return "";
                }
            }
            catch (Exception ex)
            {
                showError("Exception!!", ex.ToString(), true);
                return "";
            }
        }

        private async void tblDutyType_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tblDutyType.Text != string.Empty)
            {
                sqlstr = string.Format(sqlStr.qryDuty,tblDutyType.Text);
                var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlstr,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null && result.Data.Count() > 0)
                {
                    foreach (var row in result.Data)
                    {
                        cbbDutyDesc.Text =  row["duty_desc"]?.ToString() ?? "N/A";
                    }
                }
            }
        }

        private async void tblFixReasonCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (! await checkReasonCode(tbFixReasonCode.Text))
                {
                    MessageBox.Show("Reason Code not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    tbFixReasonCode.SelectAll();
                    tbFixReasonCode.Focus();
                    return;
                }
                else
                {
                    tblLocation.Focus();
                }
            }
        }

        public async Task<bool> checkReasonCode (string reasoncode )
        {
            var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = string.Format(sqlStr.qryCheckReason, reasoncode),
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                tblFixDescription.Text = result.Data["reason_desc"]?.ToString() ?? "";
                tblDutyType.Text = result.Data["reason_type"]?.ToString() ?? "";
                cbbDutyStation.Text = result.Data["duty_station"]?.ToString() ?? "";
               // cbbDuty.Text = result.Data["duty_desc"]?.ToString() ?? "";
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnFindReasonCode_Click(object sender, RoutedEventArgs e)
        {
            formReasonCode frmReason = new formReasonCode();
            frmReason.sfcHttpClient = this.sfcHttpClient;
            frmReason.ShowDialog();
            tbFixReasonCode.Text = ReasonCode;
            tblFixDescription.Text = ReasonDesc;

        }

        private  void gridDataReason_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = gridDataReason.SelectedItem as DataRowView;
            if (row != null)
            {
                tbFixReasonCode.Text = row.Row.ItemArray[0].ToString();
                tblLocation.Text = row.Row.ItemArray[1].ToString();
       
            }
            //var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
            //{
            //    CommandText = string.Format(sqlStr.qryItemCode2, MODEL_NAME, tblLocation.Text.ToUpper()),
            //    SfcCommandType = SfcCommandType.Text
            //});
            //if (result.Data != null && result.Data.Count() > 0)
            //{
            //    lblReasonCode.Content = result.Data["item_name"]?.ToString() ?? "";
            //}
            //else
            //{
            //    MessageBox.Show("Reason Code not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    tbFixReasonCode.SelectAll();
            //    tbFixReasonCode.Focus();
            //    return ;
            //}
        }

        private void cbChangeKeypart_Click(object sender, RoutedEventArgs e)
        {
            if (cbChangeKeypart.IsChecked == true)
            {
                tbPCBsn.Visibility = Visibility.Visible;
            }
            else
            {
                tbPCBsn.Visibility = Visibility.Hidden;
            }
        }

        private async void tbFixReasonCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbFixReasonCode.SelectionStart = tbFixReasonCode.Text.Length;
            tbFixReasonCode.Text = tbFixReasonCode.Text.ToUpper();
            if (tbFixReasonCode.Text == "I018")
            {
                sqlstr = string.Format(sqlStr.qryR108, SN);
                var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel 
                { 
                    CommandText = sqlstr ,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null && result.Data.Count() > 0)
                {
                    cbChangeKeypart.Visibility = Visibility.Visible;
                }
                else
                {
                    cbChangeKeypart.Visibility = Visibility.Visible;
                    cbChangeKeypart.IsChecked = true;
                    tbPCBsn.Visibility = Visibility.Visible;
                }
            }
            else
            {
                cbChangeKeypart.Visibility = Visibility.Hidden;
                tbPCBsn.Visibility = Visibility.Hidden;
                if (!await checkReasonCode(tbFixReasonCode.Text))
                {
                    return;
                }
            }
        }

        private void btnFindItemCoede_Click(object sender, RoutedEventArgs e)
        {
            ItemCode frmItemCode = new ItemCode();
            frmItemCode.sfcHttpClient = this.sfcHttpClient;
            frmItemCode.lblModel.Text = MODEL_NAME;
            frmItemCode.ShowDialog();
            tblLocation.Text = ItemCode;
            Location_Keydown();
        }
        private async void btnChangeKP_Click(object sender, RoutedEventArgs e)
        {
            sqlstr = string.Format(sqlStr.qryR108 , SN);
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel 
            {
                CommandText = sqlstr ,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0 )
            {
                ChangeKP frmChangeKP = new ChangeKP();
                frmChangeKP.tbInput.Text = SN;
                frmChangeKP.sfcHttpClient = this.sfcHttpClient;
                frmChangeKP.ShowDialog();
            }
            else
            {
                showError("This SN: " + SN + " no have KeyParts", "", true);
                return;
            }
        }

        private void dgr_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = " " + e.PropertyName.ToUpper().Replace("_", " ") + "  ";
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String RemoveMaterial = "";
                if (cbChangeKeypart.IsChecked == true)
                {
                    if (tbPCBsn.Text == "")
                    {
                        showError("Reason code: I018 ,Please input original keypart sn", "", true);
                        tbPCBsn.Focus();
                        M_iFlag = "0";
                        return;
                    }
                }
                if (chkbRemoveMaterial.IsChecked == true)
                {
                    RemoveMaterial = "Y";
                }

                var logInfo = new
                {
                    OPTION = "MODIFY_EXECUTE",
                    DATA = SN,
                    REPAIR_STATUS = MainWindow.RepairStatus,
                    MODEL = MODEL_NAME,
                    REASON_CODE = tbFixReasonCode.Text,
                    REASON_DESC = tblFixDescription.Text,
                    ITEM_CODE = tblLocation.Text,
                    ITEM_NAME = tblDescription2.Text,
                    SODER_QTY = tbSoderQTY.Text,
                    DUTY_TYPE = tblDutyType.Text,
                    DUTY_STATION = cbbDutyStation.Text,
                    VENDOR = cbbVender.Text,
                    DC = tbDC.Text,
                    LC = tbLC.Text,
                    TEMP_ALLPART = tbTempAllpart.Text,
                    NEW_VENDOR = tbNewVendor.Text,
                    NEW_DC = tbNewDC.Text,
                    NEW_LC = tbNewLC.Text,
                    PART_NUMBER = tbPartNumber.Text,
                    EC_EXT = tbEcExt.Text,
                    MEMO = tbMemo.Text,
                    KEYPART = tbPCBsn.Text,
                    MSECTION = SECTION,
                    LB_GROUP = lbGroup,
                    EMP = MainWindow.empNo,
                    REPAIR_TYPE = sRepairType,
                    RP_GROUP = REPAIR_GROUP,
                    LINE = LINE_NAME,
                    ROWID = F_ROWID,
                    SUPPLIER_MODEL = "",
                    //MACHINE = MainWindow.strMACHINE,
                    EXCEL_FILE = MainWindow.ExcelFile,
                    REMOVE_MATERIAL = RemoveMaterial
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
                        MainWindow.Modify_Status = true;
                        this.Close();
                    }
                    else
                    {
                        if (RESArray[1] == "MS")
                        {
                            showError("SFIS1.REPAIR_API_EXECUTE / MODIFY_EXECUTE", RESArray[2], true);
                            return;
                        }
                        else
                        {
                            showError("SFIS1.REPAIR_API_EXECUTE / MODIFY_EXECUTE", RESArray[2], false);
                            return;
                        }
                    }
                }
                else
                {
                    showError("SFIS1.REPAIR_API_EXECUTE / ModifyExecute result data null ","Error", true);
                    return;
                }

            }
            catch (Exception ex)
            {
                showError("", ex.Message,true);
                return;
            }
        }

        private void showError(string strVN, string strEng, bool Flag)
        {
            MessageError FrmMessage = new MessageError();
            FrmMessage.sfcHttpClient = this.sfcHttpClient;
            if (Flag)
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.MessageVietNam = strVN;
                FrmMessage.MessageEnglish = strEng;
            }
            else
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.errorcode = strEng;
            }

            FrmMessage.ShowDialog();
        }
    }
}
