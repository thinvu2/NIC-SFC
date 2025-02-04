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
using Path = System.IO.Path;
using REPAIR.Models;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Win32;
using System.IO;
using MES.OpINI;
using System.Windows.Threading;
using System.Diagnostics;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for frmModify.xaml
    /// </summary>
    public partial class frmCheckInOut : Window
    {
        public string FLAG, SN, MODEL_NAME , sqlstr , REempNo , PDempNo, C_Emp_No = "";
        public SfcHttpClient sfcHttpClient;
        public string[] RESArray = { "NULL" };
        private char charSub = IniUtil.CharSub;

        DataTable dtTable;
        public frmCheckInOut()
        {
            InitializeComponent();
        }

        private  async void CheckInOut_Loaded(object sender, RoutedEventArgs e)
        {
            REpassword.Focus();
            REpassword.Password = "";

            // For MBD1 LogIn 1 lan
            string sql = " SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='REPAIR' AND VR_CLASS ='MBD1' AND VR_ITEM ='CHECK_IN_OUT' AND VR_NAME ='TRUE' ";
            var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            if (result.Data != null)
            {
                if(!await Check_PRIVILEGE(C_Emp_No))
                {
                    this.Close();
                }
                else
                {
                    tbInputData.IsEnabled = true;
                    tbInputData.Focus();
                    tbInputData.Text = "";
                }
            }
        }

        public async Task<bool> Check_PRIVILEGE(string EmpNo)
        {
            if (await checkPrivilege(EmpNo, "REPAIR"))
            {
                REempNo = RESArray[1].ToString();
                tblREname.Text = RESArray[2].ToString();
                tblREname.Visibility = Visibility.Visible;

                if (await checkPrivilege(EmpNo, "PD"))
                {
                    PDempNo = RESArray[1].ToString();
                    tblPDname.Text = RESArray[2].ToString();
                    tblPDname.Visibility = Visibility.Visible;
                    return true;
                }
                else
                {
                    MessageError FrmMessage = new MessageError();
                    FrmMessage.CustomFlag = true;
                    FrmMessage.MessageVietNam = ""+ EmpNo + " - Employee No PRIVILEGE !\n FUN = PD";
                    FrmMessage.MessageEnglish = "Tài khoản không có quyền !";
                    FrmMessage.ShowDialog();
                    return false;
                }
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "" + EmpNo + " - Employee No PRIVILEGE !\n GROUP_NAME = REPAIR";
                FrmMessage.MessageEnglish = "Tài khoản không có quyền ! Chỉ RE mới có quyền";
                FrmMessage.ShowDialog();
                return false;
            }

            
        }

        private void itemCheckOut_Click(object sender, RoutedEventArgs e)
        {
            if (itemCheckOut.IsChecked == true)
            {
                if (tblStatus.Text != "CHECK OUT")
                {
                    btnUndo_Click(sender, e);
                    tblStatus.Text = "CHECK OUT";
                    itemCheckIn.IsChecked = false;
                }
            }
        }

        private void itemCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if (itemCheckIn.IsChecked == true)
            {
                if (tblStatus.Text != "CHECK IN")
                {
                    btnUndo_Click(sender, e);
                    tblStatus.Text = "CHECK IN";
                    itemCheckOut.IsChecked = false;
                }
            }
        }

        private async void tbInputData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (lstSN.Items.IndexOf(tbInputData.Text) >= 0)
                    {
                        showError("SN: " + tbInputData.Text + " has input!", "SN: " + tbInputData.Text + " đã được nhập !", true);
                        return;
                    }
                    else
                    {
                        if (!await checkRepairbySN(tbInputData.Text))
                        {
                        }
                        tbInputData.SelectAll();
                        tbInputData.Focus();
                    }
                }
                catch (Exception ex)
                {
                    showError("Input SN have error", ex.Message, true);
                    return ;
                }
            }
        }
        public async Task<bool>  checkRepairbySN(string DATA)
        {
            try
            {
                #region Product Roast time 0 hours<12h, should still roast!
                string sql1 = $@"SELECT * FROM sfism4.r109 a WHERE  item_desc IS NULL  AND repairer IS NULL AND test_time >= 
                            (SELECT MO_START_DATE + 7  FROM sfism4.r105 WHERE  mo_number = a.mo_number)  AND a.SERIAL_NUMBER = '{DATA}' ";
                var result1 = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel { CommandText = sql1, SfcCommandType = SfcCommandType.Text });
                if (result1.Data != null)
                {
                    MessageBoxResult res = MessageBox.Show("Product Roast time 0 hours<12h, should still roast! Continue CHECK_IN ???", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    if (res != MessageBoxResult.OK)
                    {
                        return false;
                    }
                }

                #endregion

                var logInfo = new
                {
                    OPTION = "checkRepairbySN",
                    DATA = DATA ,
                    FUN = tblStatus.Text
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
                        lstSN.Items.Add(RESArray[1].ToString());
                        tblStation.Text = RESArray[2].ToString();
                        tblErrorCode.Text = RESArray[3].ToString();
                        tblReasonCode.Text = RESArray[4].ToString();
                        tblRepairer.Text = RESArray[5].ToString();
                        tblRepairDate.Text = RESArray[6].ToString();
                        tblFailTime.Text = RESArray[7].ToString();
                        tblModelName.Text = RESArray[8].ToString();
                        return true;
                    }
                    else
                    {
                        if (RESArray[1] == "MS")
                        {
                            lstError.Items.Add( RESArray[2]);
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = RESArray[2];
                            frmMessage.ShowDialog();
                        }
                        else
                        {
                            lstError.Items.Add( RESArray[2]);
                        }
                        return false;
                    }
                }
                else
                {
                    lstError.Items.Add("SN:" + DATA + " " + " result data null ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                lstError.Items.Add("SN:" + DATA + " " +  ex.Message );
                return false;
            }

        }

        private void showError(string strVN, string strEng, bool Flag)
        {
            MessageError FrmMessage = new MessageError();
            FrmMessage.sfcHttpClient = sfcHttpClient;
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

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            int qtySN = lstSN.Items.Count;
            while (lstSN.Items.Count > 0)
            {
                try
                {
                    var logInfo = new
                    {
                        OPTION = "IN_OUT_EXECUTE",
                        DATA = lstSN.Items[0].ToString(),
                        FUN = tblStatus.Text,
                        PD_EMP = PDempNo,
                        RE_EMP = REempNo
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
                            lstSN.Items.Remove(RESArray[1].ToString());
                        }
                        else
                        {
                            lstSN.Items.Remove(RESArray[1].ToString());
                            lstError.Items.Add(RESArray[2].ToString());
                        }
                    }
                    else
                    {
                        lstError.Items.Add(lstSN.Items[0].ToString() + " execute result null");
                        lstSN.Items.Remove(lstSN.Items[0].ToString());
                    }
                }
                catch (Exception ex)
                {
                    lstError.Items.Add(lstSN.Items[0].ToString() + " " + ex.Message);
                    lstSN.Items.Remove(lstSN.Items[0].ToString());
                }

            }
            tbInputData.Text = "";
            tblErrorCode.Text = "";
            tblFailTime.Text = "";
            tblReasonCode.Text = "";
            tblRepairDate.Text = "";
            tblRepairer.Text = "";
            tblStation.Text = "";
            tblModelName.Text = "";
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            tbInputData.Text = "";
            lstSN.Items.Clear();
            lstError.Items.Clear();
            tblErrorCode.Text = "";
            tblFailTime.Text = "";
            tblPDname.Text = "";
            tblPDname.Visibility = Visibility.Hidden;
            PDpassword.IsEnabled = false;
            tblReasonCode.Text = "";
            tblREname.Text = "";
            tblREname.Visibility = Visibility.Hidden;
            REpassword.Password = "";
            REpassword.SelectAll();
            REpassword.Focus();
            tblRepairDate.Text = "";
            tblRepairer.Text = "";
            tblStation.Text = "";
            tblModelName.Text = "";
        }

        private async void lstSN_selection(object sender, SelectionChangedEventArgs e)
        {
            if (lstSN.Items.Count > 0)
            {
                string data = lstSN.SelectedValue.ToString();

                sqlstr = " SELECT * FROM SFISM4.R109 WHERE SERIAL_NUMBER  = '" + data + "' AND REPAIRER IS NULL ";
                var result = await sfcHttpClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlstr,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null && result.Data.Count() != 0)
                {
                    tblStation.Text = result.Data["test_group"]?.ToString() ?? "";
                    tblErrorCode.Text = result.Data["test_code"]?.ToString() ?? "";
                    tblReasonCode.Text = result.Data["reason_code"]?.ToString() ?? "";
                    tblRepairer.Text = result.Data["repairer"]?.ToString() ?? "";
                    tblRepairDate.Text = result.Data["repair_time"]?.ToString() ?? "";
                    tblFailTime.Text = result.Data["test_time"]?.ToString() ?? "";
                    tblModelName.Text = result.Data["model_name"]?.ToString() ?? "";
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            tbInputData.Text = "";
            tblErrorCode.Text = "";
            tblFailTime.Text = "";
            tblReasonCode.Text = "";
            tblRepairDate.Text = "";
            tblRepairer.Text = "";
            tblStation.Text = "";
            tblModelName.Text = "";
            lstSN.Items.Clear();
            lstError.Items.Clear();
        }

        private  async void btnExcell_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
           // openFileDialog.Filter = "Text files (*.txt)";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    txtPath.Text = Path.GetFileName(openFileDialog.FileNames.ToString());
                string[] dataSN = File.ReadAllLines(openFileDialog.FileName.ToString());
                if (dataSN != null)
                {
                    Prob1.Value = 0;
                    foreach (string sn in dataSN)
                    {
                        try
                        {
                            if (lstSN.Items.IndexOf(sn) >= 0)
                            {
                                lstError.Items.Add("SN: " + sn + " has input!");
                            }
                            else
                            {
                                if (!await checkRepairbySN(sn))
                                {

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            lstError.Items.Add("SN: " + sn + ex.Message);
                        }
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

        private async void CheckInOut_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow.DataInput = "";
        }

        private async void PDpassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (PDpassword.Password != string.Empty)
                {
                    if (await checkPrivilege(PDpassword.Password, "PD"))
                    {
                        PDempNo = RESArray[1].ToString();
                        tblPDname.Text = RESArray[2].ToString();
                        tblPDname.Visibility = Visibility.Visible;
                        tbInputData.IsEnabled = true;
                        tbInputData.Focus();
                        tbInputData.Text = "";
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "Password not match or Employee No PRIVILEGE !\n FUN = PD";
                        FrmMessage.MessageEnglish = "Mật khẩu không đúng hoặc Tài khoản không có quyền !";
                        FrmMessage.ShowDialog();
                        PDpassword.SelectAll();
                        PDpassword.Focus();
                    }
                }
            }
        }
        private async void REpassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (REpassword.Password != string.Empty)
                {
                    if (await checkPrivilege(REpassword.Password, "REPAIR"))
                    {
                        REempNo = RESArray[1].ToString();
                        tblREname.Text = RESArray[2].ToString();
                        tblREname.Visibility = Visibility.Visible;
                        PDpassword.IsEnabled = true;
                        PDpassword.Focus();
                        PDpassword.Password = "";
                    }
                    else
                    {
                        MessageError FrmMessage = new MessageError();
                        FrmMessage.CustomFlag = true;
                        FrmMessage.MessageVietNam = "Password not match or Employee No PRIVILEGE !\n FUN = REPAIR OR GROUP_NAME = REPAIR";
                        FrmMessage.MessageEnglish = "Mật khẩu không đúng hoặc Tài khoản không có quyền !";
                        FrmMessage.ShowDialog();
                        REpassword.SelectAll();
                        REpassword.Focus();
                    }
                }
            }
        }

        private void itemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void itemConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        public async Task<bool> checkPrivilege(string EmpPass, string FuntionCheck)
        {
            var logInfo = new
            {
                OPTION = "CHECKPRIVE",
                PASS = EmpPass,
                FUNTION = FuntionCheck
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            try
            {
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
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
    }
}
