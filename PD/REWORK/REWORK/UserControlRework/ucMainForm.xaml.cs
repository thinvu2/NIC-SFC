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

namespace REWORK.UserControlRework
{
    /// <summary>
    /// Interaction logic for ucMainForm.xaml
    /// </summary>
    public partial class ucMainForm : System.Windows.Controls.UserControl
    {
        public string[] RESArray = { "NULL" };
        public string[] listSN = { "NULL" };
        public static String ResultCheck108, StepGroup, ROUTE_CODE, strGroupKP, strItemChecked, REWORK_NO;
        public bool checkLinked;
        public string str_pmemo, str_sn, ModelSelected;
        int i;

        public static List<listModelName> itemsModel = new List<listModelName>();
        public MainWindow frmMain = new MainWindow();



        public ucMainForm()
        {
            InitializeComponent();
            txtInputReworkNO.IsEnabled = false;
            OpenFileReworkNO.IsEnabled = false;

        }
        private void ucMainForm_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            txtInputSN.Focus();
            StepGroup = "";
            txtReworkNO.Text = MainWindow.empNo;
        }

        public async Task<bool> GetReworkNO(String EMP_NO)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "GETREWORKNO",
                    EMP = MainWindow.empNo
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="DATA" , Value = jsonData ,SfcParameterDataType = SfcParameterDataType.Varchar2,SfcParameterDirection = SfcParameterDirection.Input},
                        new SfcParameter {Name="OUTPUT",SfcParameterDataType= SfcParameterDataType.Varchar2,SfcParameterDirection = SfcParameterDirection.Output}
                    }
                });

                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "OK")
                    {
                        REWORK_NO = RESArray[1];
                        return true;
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "Get rework NO error !";
                        frmMessage.MessageVietNam = "Sinh m?rework no l?i !";
                        frmMessage.ShowDialog();
                        return false;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Get rework NO error !";
                    frmMessage.MessageVietNam = "Sinh m?rework no l?i !";
                    frmMessage.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.Message.ToString();
                frmMessage.MessageVietNam = "Call procedure have exceptions:";
                frmMessage.ShowDialog();
                return false;
            }
        }

        private async void txtInputSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtInputCartonNO.IsEnabled = false;
                OpenFileCarton.IsEnabled = false;
                txtInputImei.IsEnabled = false;
                btnOpenFileImei.IsEnabled = false;

                ucMainFormRefreshNOT();
                //Add by Cuong 2024-02-02
                if (chkQR.IsChecked == true)
                {
                    var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.SP_GET_QRSTRING",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="IN_GROUP",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="IN_DATA",Value=txtInputSN.Text,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                                new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                            }
                    });
                    dynamic ads = result.Data;
                    string _RES = ads[1]["res"];
                    if (_RES.StartsWith("OK")) txtInputSN.Text = ads[0]["out_data"];
                };
                //End by Cuong


                if (txtInputSN.Text != null)
                {
                    if (await CheckExitR108(txtInputSN.Text))
                    {
                        txtInputSN.Text = ResultCheck108;
                    }

                    if (lstvSN.Items.IndexOf(txtInputSN.Text.ToString()) < 0)
                    {
                        if (await checkSNSSN(txtInputSN.Text.ToString()))
                        {
                            getGroupNext(txtInputSN.Text.ToString());
                        }
                    }
                    else
                    {
                        lstvSNDescription.Items.Add(txtInputSN.Text + " DUP");
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Please input data sn/ssn/mac !";
                    frmMessage.MessageVietNam = "Nh?p vào d? li?u sn/ssn/mac !";
                    frmMessage.ShowDialog();
                }
                txtInputSN.SelectAll();
                txtInputSN.Focus();
            }
        }
        private async Task<Boolean> CheckExitR108(string DATA)
        {
            var logInfo = new
            {
                OPTION = "CHECK_R108",
                DATA = DATA
            };
            string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
            try
            {
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new querySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter {Name="DATA" , Value = jsonData ,SfcParameterDataType = SfcParameterDataType.Varchar2,SfcParameterDirection = SfcParameterDirection.Input},
                        new SfcParameter {Name="OUTPUT",SfcParameterDataType= SfcParameterDataType.Varchar2,SfcParameterDirection = SfcParameterDirection.Output}
                    }

                });
                dynamic output = result.Data;
                string Res = output[0]["output"];
                RESArray = Res.Split('#');
                if (RESArray[0] == "OK")
                {
                    ResultCheck108 = RESArray[1];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.Message.ToString();
                frmMessage.MessageVietNam = "Call procedure have exceptions:";
                frmMessage.ShowDialog();
                return false;
            }
        }

        private async Task<Boolean> checkSNSSN(String data)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "checkSNSSN",
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
                        if (lstvSN.Items.IndexOf(RESArray[1]) < 0)
                        {
                            lstvSN.Items.Add(RESArray[1]);
                            lstvSNDescription.Items.Add(RESArray[1] + " ,Valid, " + RESArray[2]);
                            lblQTY.Content = lstvSN.Items.Count;

                            if (cbbModelName.Items.IndexOf(RESArray[2]) < 0)
                            {
                                if ((cbbModelName.Items.Count > 0) & (txtInputSN.Text != ""))
                                {
                                    MessageError frmMessage = new MessageError();
                                    frmMessage.CustomFlag = true;
                                    frmMessage.MessageEnglish = " Different model name , can't rework !";
                                    frmMessage.MessageVietNam = " Hai tên hàng khác nhau , không th? cùng rework !";
                                    frmMessage.ShowDialog();
                                    lstvSNDescription.Items.Add(txtInputSN.Text + " Different model_name ");
                                    lstvSN.Items.Remove(txtInputSN.Text);
                                    return false;
                                }

                                cbbModelName.Items.Add(RESArray[2]);
                            }

                            if (cbbModelName.Items.Count > 1)
                            {
                                cbbModelName.Visibility = Visibility.Visible;

                                // btnExecute.IsEnabled = false;
                            }
                            else
                            {
                                cbbModelName.Visibility = Visibility.Hidden;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        if (lstvSN.Items.IndexOf(RESArray[1]) != -1)
                        {
                            lstvSN.Items.Remove(RESArray[1]);
                        }

                        lstvSNDescription.Items.Add(RESArray[1] + " ,InValid, " + RESArray[2]);
                        return false;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Call produe error !";
                    frmMessage.MessageVietNam = "File không c?d? li?u !";
                    frmMessage.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.Message.ToString();
                frmMessage.MessageVietNam = "Call procedure have exceptions:";
                frmMessage.ShowDialog();
                return false;
            }
        }

        private async void btnOpenFileSN_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            //  openFileDialog.Filter = "Text files (*.txt)";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                txtInputCartonNO.IsEnabled = false;
                OpenFileCarton.IsEnabled = false;
                txtInputImei.IsEnabled = false;
                btnOpenFileImei.IsEnabled = false;

                foreach (string filename in openFileDialog.FileNames)
                    txtSN.Text = Path.GetFileName(openFileDialog.FileNames.ToString());
                string[] dataSN = File.ReadAllLines(openFileDialog.FileName.ToString());
                if (dataSN != null)
                {
                    Prob1.Value = 0;
                    foreach (string sn in dataSN)
                    {
                        if (!await CheckExitR108(sn))
                        {
                            ResultCheck108 = sn;
                        }
                        if (lstvSN.Items.IndexOf(ResultCheck108) < 0)
                        {
                            if (await checkSNSSN(ResultCheck108))
                            {
                                getGroupNext(ResultCheck108);
                            }
                        }
                        else
                        {
                            lstvSNDescription.Items.Add(ResultCheck108 + " DUP!!");
                        }
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "File no found data !";
                    frmMessage.MessageVietNam = "File không c?d? li?u !";
                    frmMessage.ShowDialog();
                }
            }
        }

        private async void btnAutoClick_Click(object sender, RoutedEventArgs e)
        {
            string serial, tmpgroup, res, data;
            if (await GetReworkNO(MainWindow.empNo))
            {
                txtReworkNO.Text = REWORK_NO;
            }
            else
            {
                return;
            }

            if (lstvSN.Items.Count > 0)
            {
                if (cbbModelName.Items.Count > 1)
                {
                    if (lstvSNbyModel.Items.Count > 0)
                    {
                        serial = lstvSNbyModel.Items[0].ToString();
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    serial = lstvSN.Items[0].ToString();
                }

                chkTrackNO.IsChecked = false;
                chkCartonNO.IsChecked = false;
                chkQaNo.IsChecked = false;
                chkTrayNO.IsChecked = false;
                chkCustSN.IsChecked = false;
                chkPalletNO.IsChecked = false;
                chkStockNO.IsChecked = false;
                chkShippingSN.IsChecked = false;
                chkShippingSN2.IsChecked = false;
                chkMSN.IsChecked = false;
                res = "";
                if ((cbbGroup.Text == "KEYPART") || (cbbGroup.Text == "LOT_REPRINT"))
                {
                    tmpgroup = "PACK_BOX";
                }
                else
                {
                    tmpgroup = cbbGroup.Text;
                }
                try
                {
                    var logInfo = new
                    {
                        OPTION = "getItemRemove",
                        DATA = serial,
                        GROUP = tmpgroup
                    };
                    String jsondata = JsonConvert.SerializeObject(logInfo).ToString();
                    var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.REWORK_API_EXECUTE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter {Name="DATA" ,Value=jsondata,SfcParameterDataType= SfcParameterDataType.Varchar2 , SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }

                    });
                    if (result.Data != null)
                    {
                        dynamic output = result.Data;
                        String a = output[0]["output"];
                        RESArray = a.Split('#');
                        if (RESArray[0] == "OK")
                        {
                            res = RESArray[1];
                            if (res != "")
                            {
                                string dddd = res.Substring(0, 2);
                                if (res.Substring(0, 2) == "Y-")
                                {
                                    data = res.Substring(2, res.Length - 2);
                                    if (data.Length != 12)
                                    {
                                        MessageError frmMessage = new MessageError();
                                        frmMessage.CustomFlag = true;
                                        frmMessage.MessageEnglish = "SP_107 result: " + res;
                                        frmMessage.MessageVietNam = " Th?c thi SP_107 l?i !";
                                        frmMessage.ShowDialog();
                                    }
                                    else
                                    {
                                        strItemChecked = data;
                                        if (data.Substring(0, 1) == "N")
                                        {
                                            if (chkQR.IsChecked == true)
                                            {
                                                chkCustSN.IsChecked = false;
                                                chkShippingSN.IsChecked = false;
                                                //chkShippingSN.IsEnabled = false;
                                            }
                                            else
                                            {
                                                chkCustSN.IsChecked = true;
                                                chkShippingSN.IsChecked = true;
                                            }
                                        }

                                        if (data.Substring(1, 1) == "N")
                                        {
                                            chkShippingSN2.IsChecked = true;
                                        }
                                        if (data.Substring(2, 1) == "N")
                                        {
                                            chkPoNo.IsChecked = true;
                                        }
                                        if (data.Substring(3, 1) == "N")
                                        {
                                            chkPalletNO.IsChecked = true;
                                        }
                                        if (data.Substring(4, 1) == "N")
                                        {
                                            chkCartonNO.IsChecked = true;
                                        }
                                        if (data.Substring(5, 1) == "N")
                                        {
                                            chkTrayNO.IsChecked = true;
                                        }
                                        if (data.Substring(6, 1) == "N")
                                        {
                                            chkQaNo.IsChecked = true;
                                            chkStockNO.IsChecked = true;
                                        }
                                        if (cbbGroup.Text != "LOT_REPRINT")
                                        {
                                            if (cbbGroup.Text != "PACK_BOX")
                                            {
                                                if (data.Substring(7, 1) == "Y")
                                                {
                                                    chkTrackNO.IsChecked = true;
                                                }
                                            }
                                            else
                                            {
                                                chkTrackNO.IsChecked = false;
                                            }

                                        }
                                        else
                                        {
                                            chkTrackNO.IsChecked = true;
                                            chkFinishFlag.IsChecked = true;
                                        }

                                        if (cbbGroup.Text != "KEYPART")
                                        {
                                            if (cbbGroup.Text != "PACK_BOX")
                                            {
                                                if (data.Substring(8, 1) == "Y")
                                                {
                                                    chkFinishFlag.IsChecked = true;
                                                }
                                            }
                                            else
                                            {
                                                chkFinishFlag.IsChecked = false;
                                            }

                                        }
                                        else
                                        {
                                            chkFinishFlag.IsChecked = true;
                                            chkTrackNO.IsChecked = false;
                                        }
                                        if (data.Substring(9, 1) == "Y")
                                        {
                                            chkRemoveCamera.IsChecked = true;
                                        }
                                        else
                                        {
                                            chkRemoveCamera.IsChecked = false;
                                        }
                                        if (data.Substring(10, 1) == "Y")
                                        {
                                            chkWeight.IsChecked = true;
                                        }
                                        else
                                        {
                                            chkWeight.IsChecked = false;
                                        }
                                        if (data.Substring(11, 1) == "Y")
                                        {
                                            chklicense.IsChecked = true;
                                        }
                                        else
                                        {
                                            chklicense.IsChecked = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (RESArray[2] != "0")
                                {
                                    chkShippingSN2.IsChecked = true;
                                    chkPoNo.IsChecked = true;
                                    chkPalletNO.IsChecked = true;
                                    chkCartonNO.IsChecked = true;
                                    chkTrayNO.IsChecked = true;
                                    chkQaNo.IsChecked = true;
                                    chkStockNO.IsChecked = true;
                                    chkFinishFlag.IsChecked = true;
                                    chkTrackNO.IsChecked = true;
                                    chkShippingSN.IsChecked = true;
                                }
                            }
                            btnAutoClick.IsEnabled = false;
                        }
                        else
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = RESArray[1];
                            frmMessage.MessageVietNam = "getItemRemove phát sinh l?i !";
                            frmMessage.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "getItemRemove result data null";
                        frmMessage.MessageVietNam = "getItemRemove không c?d? li?u !";
                        frmMessage.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = ex.Message;
                    frmMessage.MessageVietNam = "Have exceptions when auto click getItemRemove";
                    frmMessage.ShowDialog();
                }
            }
        }

        private async void btnRemoveKP_Click(object sender, RoutedEventArgs e)
        {
            if (cbbGroup.Text == "")
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = "Group Next null , Please select group next";
                frmMessage.MessageVietNam = "Group next không c?gi?tr?,Hãy Ch?n group next";
                frmMessage.ShowDialog();
                return;
            }
            if ((ROUTE_CODE == "") || (ROUTE_CODE is null))
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = "ROUTE CODE null";
                frmMessage.MessageVietNam = "ROUTE CODE Không c?d? li?u";
                frmMessage.ShowDialog();
                return;
            }
            try
            {
                var logInfo = new
                {
                    OPTION = "getGroupKP",
                    GROUP = cbbGroup.Text,
                    ROUTE = ROUTE_CODE
                };
                String jsonData = JsonConvert.SerializeObject(logInfo).ToString();
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="DATA" ,Value =jsonData ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Input},
                    new SfcParameter {Name="OUTPUT" ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                }
                });
                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "OK")
                    {
                        int i;
                        strGroupKP = "'1'";
                        for (i = 0; i < int.Parse(RESArray[1]); i++)
                        {
                            strGroupKP = strGroupKP + ",'" + RESArray[2 + i] + "'";
                            lstvGroupKP.Items.Add("_" + RESArray[2 + i]);
                        }
                        btnRemoveKP.IsEnabled = false;
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = RESArray[1];
                        frmMessage.MessageVietNam = "getItemRemove phát sinh l?i !";
                        frmMessage.ShowDialog();
                        return;
                    }


                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "getItemRemove result data null";
                    frmMessage.MessageVietNam = "getItemRemove không c?d? li?u !";
                    frmMessage.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.Message;
                frmMessage.MessageVietNam = "Have exceptions when auto click getItemRemove";
                frmMessage.ShowDialog();
                return;
            }
        }

        private async void getGroupNext(string data)
        {
            //lstvSN.Visibility = Visibility.Visible;
            //lstvSNbyModel.Visibility = Visibility.Hidden;
            try
            {
                var logInfo = new
                {
                    OPTION = "checkRouter",
                    STEPGROUP = StepGroup,
                    DATA = data
                };
                string jsondata = JsonConvert.SerializeObject(logInfo).ToString();
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter { Name="DATA" ,Value = jsondata,SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter { Name="OUTPUT",SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                    }
                });
                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string Res = output[0]["output"];
                    RESArray = Res.Split('#');
                    if (RESArray[0] == "OK")
                    {
                        StepGroup = RESArray[4];
                        if (cbbModelName.Items.IndexOf(RESArray[1]) < 0)
                        {
                            cbbModelName.Items.Add(RESArray[1]);
                        }

                        if (cbbModelName.Items.Count > 1)
                        {
                            cbbModelName.Visibility = Visibility.Visible;
                            //btnExecute.IsEnabled = false;
                        }
                        else
                        {
                            cbbModelName.Visibility = Visibility.Hidden;
                        }

                        if (cbbRouteName.Items.Count > 0)
                        {
                            if (cbbRouteName.Items.IndexOf("[" + RESArray[3] + "] " + RESArray[2]) < 0)
                            {
                                if (cbbModelName.Items.Count == 1)
                                {
                                    int k;
                                    for (k = 0; k < int.Parse(RESArray[5]); k++)
                                    {
                                        var a = cbbGroup.Items[k].ToString().Trim();
                                        var b = RESArray[6 + k].ToString().Trim();
                                        if (a != b)
                                        {
                                            lstvSNDescription.Items.Add(data + " Different route code !");
                                            lstvSN.Items.Remove(data);
                                            return;
                                        }
                                    }
                                    //ROUTE_CODE = RESArray[3];
                                    //cbbRouteName.Items.Add("[" + RESArray[3] + "] " + RESArray[2]);
                                }
                            }
                        }
                        else
                        {
                            ROUTE_CODE = RESArray[3];
                            cbbRouteName.Items.Add("[" + RESArray[3] + "] " + RESArray[2]);
                        }

                        //ROUTE_CODE = RESArray[3];
                        //cbbRouteName.Items.Add("[" + RESArray[3] + "] " + RESArray[2]);

                        int j;
                        cbbGroup.Items.Clear();
                        for (j = 0; j < int.Parse(RESArray[5]); j++)
                        {
                            cbbGroup.Items.Add(RESArray[6 + j]);
                        }
                        cbbGroup.SelectedIndex = 0;
                        if (cbbModelName.Items.Count == 1)
                        {
                            cbbModelName.SelectedIndex = 1;
                        }
                        else if (cbbModelName.Items.Count > 1)
                        {
                            cbbModelName.SelectedIndex = 2;
                        }
                    }
                    else
                    {
                        lstvError.Items.Add(data + " : " + RESArray[1]);
                        lstvSN.Items.Remove(data);
                        lblQTY.Content = lstvSN.Items.Count;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Excute Procedure SFIS1.REWORK_API_EXECUTE/checkRouter return null !";
                    frmMessage.MessageVietNam = " !";
                    frmMessage.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = "Have exceptions when getGroupNext " + ex.Message;
                frmMessage.MessageVietNam = "getGroupNext phát sinh l?i !";
                frmMessage.ShowDialog();
                return;
            }

        }
        private void ucMainReresh()
        {
            chkitem(false);
            txtReason.Text = "";
            cbbGroup.Items.Clear();
            cbbModelName.Items.Clear();
            cbbRouteName.Items.Clear();
        }

        private void CbbGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chkitem(false);
            btnAutoClick.IsEnabled = true;
            btnRemoveKP.IsEnabled = true;
        }
        private static string Converbool(Boolean data)
        {
            if (data)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
        private async void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            int i;

            if (lstvSN.Items.Count == 0)
            {
                return;
            }

            if (cbbModelName.Visibility == Visibility.Hidden)
            {
                cbbModelName.SelectedIndex = 0;
            }

            if (btnAutoClick.IsEnabled == true)
            {
                btnAutoClick_Click(sender, e);
            }
            if (btnRemoveKP.IsEnabled == true)
            {
                btnRemoveKP_Click(sender, e);
            }
            if (txtReason.Text.ToString().Trim() != "")
            {
                checkLinked = false;
                for (i = 0; i < lstvSN.Items.Count; i++)
                {
                    var logInfor = new
                    {
                        OPTION = "CheckSNLinked",
                        SN = lstvSN.Items[i].ToString()
                    };
                    string jsonData1 = JsonConvert.SerializeObject(logInfor).ToString();
                    var result1 = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                    {
                        CommandText = "SFIS1.REWORK_API_EXECUTE",
                        SfcCommandType = SfcCommandType.StoredProcedure,
                        SfcParameters = new List<SfcParameter>
                        {
                            new SfcParameter {Name="DATA",Value= jsonData1 ,SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection =SfcParameterDirection.Input},
                            new SfcParameter {Name="OUTPUT" , SfcParameterDataType =SfcParameterDataType.Varchar2 , SfcParameterDirection= SfcParameterDirection.Output}
                        }
                    });
                    if (result1.Data != null)
                    {
                        dynamic output = result1.Data;
                        string RES = output[0]["output"];
                        RESArray = RES.Split('#');
                        if (RESArray[0] == "NG")
                        {
                            lstvSN.Items.Remove(RESArray[1]);
                            checkLinked = true;
                        }
                    }
                }
                if (checkLinked)
                {
                    MessageBoxResult dlrs = MessageBox.Show("have SN linked, YES to continue", "Waiting", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (dlrs == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                if (cbbGroup.Text == "NOTINPUT")
                {
                    if (cbbModelName.Text.Substring(0, 4) == "U12H")
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "MODEL U12% CAN NOT REWORK TO NOT INPUT ";
                        frmMessage.MessageVietNam = "Hàng U12H Không th? rework v? NOT INPUT !";
                        frmMessage.ShowDialog();
                        InitializeComponent();
                        return;
                    }
                }
                MessageBoxResult dlr = MessageBox.Show("Rework to: " + cbbGroup.Text, "Waiting", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (dlr == MessageBoxResult.No)
                {
                    return;
                }
                if ((txtReworkNO.Text == "")
                    || (txtReworkNO.Text is null))
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Rework No Can''t Be Blank!! ";
                    frmMessage.MessageVietNam = "Rework No Can''t Be Blank!!!";
                    frmMessage.ShowDialog();
                    return;
                }

                if ((cbbGroup.Text == "")
                 || (cbbGroup.Text is null))
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "GROUP_NEXT Can''t Be NULL!! ";
                    frmMessage.MessageVietNam = "GROUP_NEXT Can''t Be NULL!!";
                    frmMessage.ShowDialog();
                    return;
                }

                if (lstvSN.Items.Count == 0)
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "No Data to Rework ";
                    frmMessage.MessageVietNam = "Không c?d? li?u rework ";
                    frmMessage.ShowDialog();
                    return;
                }
                str_pmemo = chkCartonNO.Content + "=" + Converbool(chkCartonNO.IsChecked.Value) + ";"
                           + chkCustSN.Content + "=" + Converbool(chkCustSN.IsChecked.Value) + ";"
                           + chkFinishFlag.Content + "=" + Converbool(chkFinishFlag.IsChecked.Value) + ";"
                           + chkMSN.Content + "=" + Converbool(chkMSN.IsChecked.Value) + ";"
                           + chkPalletNO.Content + "=" + Converbool(chkPalletNO.IsChecked.Value) + ";"
                           + chkPoNo.Content + "=" + Converbool(chkPoNo.IsChecked.Value) + ";"
                           + chkQaNo.Content + "=" + Converbool(chkQaNo.IsChecked.Value) + ";"
                           + chkRemoveCamera.Content + "=" + Converbool(chkRemoveCamera.IsChecked.Value) + ";"
                           + chkShipNo.Content + "=" + Converbool(chkShipNo.IsChecked.Value) + ";"
                           + chkShippingSN.Content + "=" + Converbool(chkShippingSN.IsChecked.Value) + ";"
                           + chkShippingSN2.Content + "=" + Converbool(chkShippingSN2.IsChecked.Value) + ";"
                           + chkStockNO.Content + "=" + Converbool(chkStockNO.IsChecked.Value) + ";"
                           + chkTrackNO.Content + "=" + Converbool(chkTrackNO.IsChecked.Value) + ";"
                           + chkTrayNO.Content + "=" + Converbool(chkTrayNO.IsChecked.Value) + ";"
                           + chkWeight.Content + "=" + Converbool(chkWeight.IsChecked.Value) + ";"
                           + chklicense.Content + "=" + Converbool(chklicense.IsChecked.Value);

                var logInfo = new
                {
                    OPTION = "CHECK_REWORK_NO",
                    EMP = MainWindow.empNo,
                    REWORK_NO = txtReworkNO.Text,
                    ADDRESS = "; MAC ADDRESS: " + MainWindow.MACAddress + " ; IP: " + MainWindow.IP,
                    PMEMO = str_pmemo,
                    MODEL = cbbModelName.Text,
                    GROUP = cbbGroup.Text,
                    REASON = txtReason.Text,
                    QTY = lstvSN.Items.Count.ToString()
                };
                string jsonData = JsonConvert.SerializeObject(logInfo).ToString();
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter {Name="DATA" , Value=jsonData, SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT" , SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection= SfcParameterDirection.Output}
                    }
                });
                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "NG")
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = RESArray[1];
                        frmMessage.MessageVietNam = "ERROR";
                        frmMessage.ShowDialog();
                        txtReason.Focus();
                        return;
                    }
                }
                getItemChecked();



                if (cbbModelName.Visibility == Visibility.Visible)
                {
                    int qtySN = lstvSNbyModel.Items.Count;
                    while (lstvSNbyModel.Items.Count > 0)
                    {
                        i = 0;
                        var logInfo2 = new
                        {
                            OPTION = "EXECUTE",
                            SN = lstvSNbyModel.Items[i],
                            MODEL = cbbModelName.Text,
                            GROUP_KP = strGroupKP,
                            GROUP = cbbGroup.Text,
                            REMOVE = strItemChecked,
                            REWOEK_NO = txtReworkNO.Text
                        };
                        string jsonData2 = JsonConvert.SerializeObject(logInfo2).ToString();
                        var result2 = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.REWORK_API_EXECUTE",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>
                        {
                        new SfcParameter {Name="DATA" , Value=jsonData2, SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT" , SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection= SfcParameterDirection.Output}
                        }
                        });
                        if (result2.Data != null)
                        {
                            dynamic output = result2.Data;
                            string RES = output[0]["output"];
                            RESArray = RES.Split('#');
                            if (RESArray[0] == "NG")
                            {
                                lstvError.Items.Add(lstvSN.Items[i] + " ," + RESArray[1]);
                            }
                        }

                        lstvSNbyModel.Items.Remove(lstvSNbyModel.Items[i]);
                        lblQTY.Content = lstvSNbyModel.Items.Count;
                        Prob1.Value = (qtySN - lstvSNbyModel.Items.Count) * 100 / qtySN;
                        int data = (qtySN - lstvSNbyModel.Items.Count) * 100 / qtySN;
                    }
                    if (lblQTY.Content.ToString() == "0")
                    {
                        chkitem(false);
                        cbbModelName.Items.Remove(cbbModelName.Text);
                        if (cbbModelName.Items.Count > 0)
                        {
                            cbbModelName.SelectedIndex = 0;
                            CbbModelName_Changed();
                        }
                        else
                        {
                            chkitem(false);
                            ucMainFormRefreshALL();
                        }
                    }
                }
                else
                {

                    int qtySN = lstvSN.Items.Count;
                    cbbModelName.SelectedIndex = 0;

                    while (lstvSN.Items.Count > 0)
                    {
                        i = 0;
                        var logInfo2 = new
                        {
                            OPTION = "EXECUTE",
                            SN = lstvSN.Items[i],
                            MODEL = cbbModelName.Text,
                            GROUP_KP = strGroupKP,
                            GROUP = cbbGroup.Text,
                            REMOVE = strItemChecked,
                            REWOEK_NO = txtReworkNO.Text
                        };
                        string jsonData2 = JsonConvert.SerializeObject(logInfo2).ToString();
                        var result2 = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SFIS1.REWORK_API_EXECUTE",
                            SfcCommandType = SfcCommandType.StoredProcedure,
                            SfcParameters = new List<SfcParameter>
                        {
                        new SfcParameter {Name="DATA" , Value=jsonData2, SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT" , SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection= SfcParameterDirection.Output}
                        }
                        });
                        if (result2.Data != null)
                        {
                            dynamic output = result2.Data;
                            string RES = output[0]["output"];
                            RESArray = RES.Split('#');
                            if (RESArray[0] == "NG")
                            {
                                lstvError.Items.Add(lstvSN.Items[i] + " ," + RESArray[1]);
                            }
                        }

                        lstvSN.Items.Remove(lstvSN.Items[i]);
                        lblQTY.Content = lstvSN.Items.Count;
                        Prob1.Value = (qtySN - lstvSN.Items.Count) * 100 / qtySN;
                        int data = (qtySN - lstvSN.Items.Count) * 100 / qtySN;

                    }
                    if (lblQTY.Content.ToString() == "0")
                    {
                        chkitem(false);
                        ucMainFormRefreshALL();
                    }
                }
            }
            else
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = "Insert Reason, Please ! ";
                frmMessage.MessageVietNam = "Nh?p l?do Rework !";
                frmMessage.ShowDialog();
                txtReason.Focus();
            }
        }

        public async void ucMainFormRefreshALL()
        {

            btnNotInput.IsEnabled = true;
            btnRemoveKP.IsEnabled = true;
            cbbGroup.IsEnabled = true;
            btnAutoClick.IsEnabled = true;
            cbbGroup.Items.Clear();
            cbbModelName.Items.Clear();
            cbbRouteName.Items.Clear();
            lstvGroupKP.Items.Clear();
            lstvSNDescription.Items.Clear();
            Prob1.Value = 0;
            txtReason.Text = "";
            StepGroup = "";
            txtInputImei.Text = "";
            txtInputReworkNO.Text = "";
            txtInputSN.Text = "";
            txtInputCartonNO.Text = "";
            chkitem(false);
            groupItems.IsEnabled = true;
            lstvSN.Items.Clear();
            lstvSNbyModel.Items.Clear();
            lstvSN.Visibility = Visibility.Visible;
            lstvSNbyModel.Visibility = Visibility.Hidden;
            if (await GetReworkNO(MainWindow.empNo))
            {
                txtReworkNO.Text = REWORK_NO;
            }
            else
            {
                return;
            }
        }

        private void ucMainFormRefreshNOT()
        {
            if (btnNotInput.IsEnabled == false)
            {
                btnNotInput.IsEnabled = true;
                btnRemoveKP.IsEnabled = true;
                cbbGroup.IsEnabled = true;
                btnAutoClick.IsEnabled = true;
                cbbGroup.Items.Clear();
                cbbModelName.Items.Clear();
                cbbRouteName.Items.Clear();
                lstvGroupKP.Items.Clear();
                lstvSNDescription.Items.Clear();
                Prob1.Value = 0;
                txtReason.Text = "";
                StepGroup = "";
                chkitem(false);
                groupItems.IsEnabled = true;
            }
        }

        private async void btnOpenFileCarton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                txtInputSN.IsEnabled = false;
                btnOpenFileSN.IsEnabled = false;
                txtInputImei.IsEnabled = false;
                btnOpenFileImei.IsEnabled = false;

                foreach (string filename in openFileDialog.FileNames)
                    txtSN.Text = Path.GetFileName(openFileDialog.FileNames.ToString());
                string[] dataCarton = File.ReadAllLines(openFileDialog.FileName.ToString());
                if (dataCarton != null)
                {
                    foreach (string CartonNo in dataCarton)
                    {
                        if (await inPutCartonNO(CartonNo))
                        {

                        }
                    }
                    for (i = 0; i < lstvSN.Items.Count; i++)
                    {
                        getGroupNext(lstvSN.Items[i].ToString());
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "File no found data !";
                    frmMessage.MessageVietNam = "File không c?d? li?u !";
                    frmMessage.ShowDialog();
                }
            }
        }

        private async void BtnNotInput_Click(object sender, RoutedEventArgs e)
        {
            if (await frmMain.checkPrivilege(MainWindow.empNo, "GOTO_BEGIN"))
            {
                cbbGroup.Items.Clear();
                cbbGroup.Items.Add("NOTINPUT");
                cbbGroup.SelectedIndex = 0;
                chkitem(true);
                groupItems.IsEnabled = false;
                btnAutoClick.IsEnabled = false;
                btnExecute.IsEnabled = true;
                btnNotInput.IsEnabled = false;
                btnRemoveKP.IsEnabled = false;
                cbbGroup.IsEnabled = false;
                if (await GetReworkNO(MainWindow.empNo))
                {
                    txtReworkNO.Text = REWORK_NO;
                }
            }
            else
            {
                MessageError FrmMessage = new MessageError();
                FrmMessage.CustomFlag = true;
                FrmMessage.MessageVietNam = "M?TH? KHÔNG C?QUYÊN REWORK NOTINPUT";
                FrmMessage.MessageEnglish = "EMP NO PRIVILEGE";
                FrmMessage.ShowDialog();
                return;
            }
        }

        private void chkitem(Boolean value)
        {

            chkCartonNO.IsChecked = value;
            chkCustSN.IsChecked = value;
            chkFinishFlag.IsChecked = value;
            chkMSN.IsChecked = value;
            chkPalletNO.IsChecked = value;
            chkPoNo.IsChecked = value;
            chkQaNo.IsChecked = value;
            chkRemoveCamera.IsChecked = value;
            chkShipNo.IsChecked = value;
            chkShippingSN.IsChecked = value;
            chkShippingSN2.IsChecked = value;
            chkStockNO.IsChecked = value;
            chkTrackNO.IsChecked = value;
            chkTrayNO.IsChecked = value;
            chkUpdateGroup.IsChecked = value;
            chkWeight.IsChecked = value;
        }

        private void getItemChecked()
        {
            if (chkShippingSN.IsChecked == true)  /// 1
            {
                strItemChecked = "Y";
            }
            else
            {
                strItemChecked = "N";
            }
            if (chkCartonNO.IsChecked == true)  /// 2
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }

            if (chkQaNo.IsChecked == true)  /// 3
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkTrackNO.IsChecked == true)  /// 4
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkShipNo.IsChecked == true)  /// 5
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkUpdateGroup.IsChecked == true)  /// 6
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkFinishFlag.IsChecked == true)  /// 7
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkRemoveCamera.IsChecked == true)  /// 8
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkTrayNO.IsChecked == true)  /// 9
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkPalletNO.IsChecked == true)  /// 10
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkStockNO.IsChecked == true)  /// 11
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkPoNo.IsChecked == true)  /// 12
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkMSN.IsChecked == true)  /// 13
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkShippingSN2.IsChecked == true)  /// 14
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkCustSN.IsChecked == true)  /// 15
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chkWeight.IsChecked == true)  /// 16
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
            if (chklicense.IsChecked == true)  /// 17
            {
                strItemChecked = strItemChecked + "Y";
            }
            else
            {
                strItemChecked = strItemChecked + "N";
            }
        }
        private async Task<bool> inPutCartonNO(string CARTON_NO)
        {
            int i = 0;
            try
            {
                var logInfo2 = new
                {
                    CARTON = CARTON_NO,
                    OPTION = "INPUTCARTON"
                };
                string jsonData2 = JsonConvert.SerializeObject(logInfo2).ToString();
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                        {
                        new SfcParameter {Name="DATA" , Value=jsonData2, SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT" , SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection= SfcParameterDirection.Output}
                        }
                });

                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "OK")
                    {
                        var getSN = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SELECT serial_number FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO = '" + CARTON_NO + "'  OR TRAY_NO ='" + CARTON_NO + "' ",
                            SfcCommandType = SfcCommandType.Text
                        });

                        if (getSN.Data != null && getSN.Data.Count() > 0)
                        {
                            foreach (var row in getSN.Data)
                            {
                                if (lstvSN.Items.IndexOf(row["serial_number"].ToString()) < 0)
                                {
                                    lstvSN.Items.Add(row["serial_number"].ToString());
                                    lstvSNDescription.Items.Add(row["serial_number"].ToString() + ", Valid");
                                    lblQTY.Content = lstvSN.Items.Count;
                                }
                                else
                                {
                                    lstvSNDescription.Items.Add(row["serial_number"].ToString() + ", DUP");
                                }
                            }
                        }
                        if (lstvSN.Items.Count > 0)
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
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = RESArray[1];
                        frmMessage.MessageVietNam = "ERROR";
                        frmMessage.ShowDialog();
                        txtReason.Focus();
                        return false;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "SFIS1.REWORK_API_EXECUTE / INPUTCARTON Result null";
                    frmMessage.MessageVietNam = "INPUT CARTON ERROR ";
                    frmMessage.ShowDialog();
                    txtReason.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.ToString();
                frmMessage.MessageVietNam = "INPUT CARTON ERROR ";
                frmMessage.ShowDialog();
                txtReason.Focus();
                return false;
            }
        }
        private async void txtInputCartonNO_Keydown(object sender, KeyEventArgs e)
        {
            int i = 0;
            if (e.Key == Key.Enter)
            {
                txtInputSN.IsEnabled = false;
                btnOpenFileSN.IsEnabled = false;
                txtInputImei.IsEnabled = false;
                btnOpenFileImei.IsEnabled = false;

                if (txtInputCartonNO.Text != "")
                {
                    if (await inPutCartonNO(txtInputCartonNO.Text))
                    {
                        for (i = 0; i < lstvSN.Items.Count; i++)
                        {
                            getGroupNext(lstvSN.Items[i].ToString());
                        }
                    }
                }

                txtInputCartonNO.Focus();
                txtInputCartonNO.SelectAll();
            }
        }

        private async Task<bool> inPutImei(string imei)
        {
            int i = 0;
            try
            {
                var logInfo2 = new
                {
                    IMEI = imei,
                    OPTION = "INPUTIMEI"
                };
                string jsonData2 = JsonConvert.SerializeObject(logInfo2).ToString();
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                        {
                        new SfcParameter {Name="DATA" , Value=jsonData2, SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT" , SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection= SfcParameterDirection.Output}
                        }
                });

                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "OK")
                    {

                        var resultR107 = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE IMEI ='" + imei + "' ",
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (resultR107.Data != null)
                        {

                            foreach (var row in resultR107.Data)
                            {
                                if (lstvSN.Items.IndexOf(row["serial_number"].ToString()) < 0)
                                {
                                    lstvSN.Items.Add(row["serial_number"].ToString());
                                    lstvSNDescription.Items.Add(row["serial_number"].ToString() + ", Valid");
                                    lblQTY.Content = lstvSN.Items.Count;
                                }
                                else
                                {
                                    lstvSNDescription.Items.Add(row["serial_number"].ToString() + ", DUP");
                                }
                            }
                        }

                        if (lstvSN.Items.Count > 0)
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
                        //lstvSNDescription.Items.Add(imei + ", " + RESArray[1].ToString());
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = RESArray[1];
                        frmMessage.MessageVietNam = "ERROR";
                        frmMessage.ShowDialog();
                        txtReason.Focus();
                        return false;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "SFIS1.REWORK_API_EXECUTE / INPUTIMEI Result null";
                    frmMessage.MessageVietNam = "INPUT IMEI ERROR ";
                    frmMessage.ShowDialog();
                    txtReason.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.ToString();
                frmMessage.MessageVietNam = "INPUT IMEI ERROR ";
                frmMessage.ShowDialog();
                txtReason.Focus();
                return false;
            }
        }

        private async void txtInputImei_Keydown(object sender, KeyEventArgs e)
        {
            int i = 0;
            if (e.Key == Key.Enter)
            {
                txtInputSN.IsEnabled = false;
                btnOpenFileSN.IsEnabled = false;
                txtInputCartonNO.IsEnabled = false;
                OpenFileCarton.IsEnabled = false;

                if (txtInputImei.Text != "")
                {
                    if (await inPutImei(txtInputImei.Text))
                    {
                        for (i = 0; i < lstvSN.Items.Count; i++)
                        {
                            getGroupNext(lstvSN.Items[i].ToString());
                        }
                    }
                }

                txtInputImei.Focus();
                txtInputImei.SelectAll();
            }
        }

        private async void btnOpenFileImei_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                txtInputSN.IsEnabled = false;
                btnOpenFileSN.IsEnabled = false;
                txtInputCartonNO.IsEnabled = false;
                OpenFileCarton.IsEnabled = false;

                foreach (string filename in openFileDialog.FileNames)
                    txtSN.Text = Path.GetFileName(openFileDialog.FileNames.ToString());
                string[] dataCarton = File.ReadAllLines(openFileDialog.FileName.ToString());
                if (dataCarton != null)
                {
                    foreach (string CartonNo in dataCarton)
                    {
                        if (await inPutImei(CartonNo))
                        {

                        }
                    }
                    for (i = 0; i < lstvSN.Items.Count; i++)
                    {
                        getGroupNext(lstvSN.Items[i].ToString());
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "File no found data !";
                    frmMessage.MessageVietNam = "File không c?d? li?u !";
                    frmMessage.ShowDialog();
                }
            }
        }

        private void lstvSNDescription_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            showDataSelected frmDataSelected = new showDataSelected();
            frmDataSelected.txtData.Text = lstvSNDescription.SelectedValue.ToString();
            frmDataSelected.ShowDialog();
        }

        private void cbbRouteName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async Task<bool> inPutReworkNo(string ReworkNo)
        {
            int i = 0;
            try
            {
                var logInfo2 = new
                {
                    REWORKNO = ReworkNo,
                    OPTION = "InputReworkNo"
                };
                string jsonData2 = JsonConvert.SerializeObject(logInfo2).ToString();
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter {Name="DATA" , Value=jsonData2, SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT" , SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection= SfcParameterDirection.Output}
                    }
                });


                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string RES = output[0]["output"];
                    RESArray = RES.Split('#');
                    if (RESArray[0] == "OK")
                    {

                        var getSN = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SELECT serial_number FROM SFISM4.R_WIP_TRACKING_T WHERE rework_no = '" + ReworkNo + "' ",
                            SfcCommandType = SfcCommandType.Text
                        });

                        if (getSN.Data != null && getSN.Data.Count() > 0)
                        {
                            foreach (var row in getSN.Data)
                            {
                                if (lstvSN.Items.IndexOf(row["serial_number"].ToString()) < 0)
                                {
                                    lstvSN.Items.Add(row["serial_number"].ToString());
                                    lstvSNDescription.Items.Add(row["serial_number"].ToString() + ", Valid");
                                    lblQTY.Content = lstvSN.Items.Count;
                                }
                                else
                                {
                                    lstvSNDescription.Items.Add(row["serial_number"].ToString() + ", DUP");
                                }
                            }
                        }
                        if (lstvSN.Items.Count > 0)
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
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = RESArray[1];
                        frmMessage.MessageVietNam = "ERROR";
                        frmMessage.ShowDialog();
                        txtReason.Focus();
                        return false;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "SFIS1.REWORK_API_EXECUTE / InputReworkNo Result null";
                    frmMessage.MessageVietNam = "INPUT REWORK_NO ERROR ";
                    frmMessage.ShowDialog();
                    txtReason.Focus();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.ToString();
                frmMessage.MessageVietNam = "INPUT REWORK_NO ERROR ";
                frmMessage.ShowDialog();
                txtReason.Focus();
                return false;
            }
        }
        private async void TxtInputReworkNO_KeyDown(object sender, KeyEventArgs e)
        {
            int i = 0;
            if (e.Key == Key.Enter)
            {
                if (txtInputReworkNO.Text != "")
                {
                    if (await inPutReworkNo(txtInputReworkNO.Text))
                    {
                        for (i = 0; i < lstvSN.Items.Count; i++)
                        {
                            getGroupNext(lstvSN.Items[i].ToString());
                        }
                    }
                }

                txtInputReworkNO.Focus();
                txtInputReworkNO.SelectAll();
            }
        }
        private async void OpenFileReworkNO_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    txtSN.Text = Path.GetFileName(openFileDialog.FileNames.ToString());
                string[] dataReworkNo = File.ReadAllLines(openFileDialog.FileName.ToString());
                if (dataReworkNo != null)
                {
                    foreach (string reworkno in dataReworkNo)
                    {
                        if (await inPutReworkNo(reworkno))
                        {
                        }
                    }
                    for (i = 0; i < lstvSN.Items.Count; i++)
                    {
                        getGroupNext(lstvSN.Items[i].ToString());
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "File data not found !";
                    frmMessage.MessageVietNam = "File không c?d? li?u !";
                    frmMessage.ShowDialog();
                    return;
                }
            }
        }
        private async void CbbModelName_Changed()
        {
            int i;
            string SN;
            ModelSelected = cbbModelName.SelectedValue.ToString();
            cbbRouteName.Items.Clear();
            cbbGroup.Items.Clear();
            lstvSNbyModel.Items.Clear();
            lstvSNbyModel.Visibility = Visibility.Visible;
            lstvSN.Visibility = Visibility.Hidden;
            lstvGroupKP.Items.Clear();
            btnExecute.IsEnabled = true;
            if (txtInputSN.Text == "")
            {
                if (lstvSN.Items.Count > 0)
                {
                    for (i = 0; i < lstvSN.Items.Count; i++)
                    {
                        SN = lstvSN.Items[i].ToString();
                        var result = await MainWindow.sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = "SELECT * FROM SFISM4.R107 WHERE SERIAL_NUMBER  = '" + SN + "' AND MODEL_NAME  = '" + ModelSelected + "' ",
                            SfcCommandType = SfcCommandType.Text
                        });

                        if (result.Data.Count() > 0)
                        {
                            lstvSNbyModel.Items.Add(SN);
                            getGroupNextbyModel(SN);
                        }
                        lblQTY.Content = lstvSNbyModel.Items.Count;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "List data not found !";
                    frmMessage.MessageVietNam = "List không c?d? li?u !";
                    frmMessage.ShowDialog();
                    return;
                }
            }
            else
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = " Different model name , can't rework !";
                frmMessage.MessageVietNam = " Hai tên hàng khác nhau , không th? cùng rework !";
                frmMessage.ShowDialog();
                lstvSNDescription.Items.Add(txtInputSN.Text + " Different model_name ");
                lstvSN.Items.Remove(txtInputSN.Text);
                return;
            }
        }

        private void CbbModelName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbModelName.Items.Count > 1)
            {
                CbbModelName_Changed();
            }
        }

        private async void getGroupNextbyModel(string data)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "checkRouter",
                    STEPGROUP = StepGroup,
                    DATA = data
                };
                string jsondata = JsonConvert.SerializeObject(logInfo).ToString();
                var result = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter { Name="DATA" ,Value = jsondata,SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter { Name="OUTPUT",SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection = SfcParameterDirection.Output}
                    }
                });
                if (result.Data != null)
                {
                    dynamic output = result.Data;
                    string Res = output[0]["output"];
                    RESArray = Res.Split('#');
                    if (RESArray[0] == "OK")
                    {
                        StepGroup = RESArray[4];

                        if (cbbRouteName.Items.Count > 0)
                        {
                            if (cbbRouteName.Items.IndexOf("[" + RESArray[3] + "] " + RESArray[2]) < 0)
                            {
                                return;
                            }
                        }
                        else
                        {
                            ROUTE_CODE = RESArray[3];
                            cbbRouteName.Items.Add("[" + RESArray[3] + "] " + RESArray[2]);
                        }

                        int j;
                        cbbGroup.Items.Clear();
                        for (j = 0; j < int.Parse(RESArray[5]); j++)
                        {
                            cbbGroup.Items.Add(RESArray[6 + j]);
                        }
                        cbbGroup.SelectedIndex = 0;
                        cbbRouteName.SelectedIndex = 0;
                    }
                    else
                    {
                        lstvError.Items.Add(data + " : " + RESArray[1]);
                        lstvSNbyModel.Items.Remove(data);
                        lblQTY.Content = lstvSNbyModel.Items.Count;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Excute Procedure SFIS1.REWORK_API_EXECUTE/checkRouter return null !";
                    frmMessage.MessageVietNam = " !";
                    frmMessage.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = "Have exceptions when getGroupNext " + ex.Message;
                frmMessage.MessageVietNam = "getGroupNext phát sinh l?i !";
                frmMessage.ShowDialog();
                return;
            }
        }
        private async Task<Boolean> checkSNbyModel(String DATA, string MODEL)
        {
            try
            {
                var logInfo = new
                {
                    OPTION = "checkSNbyModel",
                    MODEL_NAME = MODEL,
                    DATA = DATA
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
                        if (lstvSNbyModel.Items.IndexOf(RESArray[1]) < 0)
                        {
                            lstvSNbyModel.Items.Add(RESArray[1]);
                            lblQTY.Content = lstvSNbyModel.Items.Count;
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = "Call produe error !";
                    frmMessage.MessageVietNam = "File không c?d? li?u !";
                    frmMessage.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.Message.ToString();
                frmMessage.MessageVietNam = "Call procedure have exceptions:";
                frmMessage.ShowDialog();
                return false;
            }
        }
    }
}
