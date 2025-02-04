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
    /// Interaction logic for ucTReworkForm.xaml
    /// </summary>
    public partial class ucTReworkForm : UserControl
    {
        public string[] RESArray = { "NULL" };
        public string[] listSN = { "NULL" };
        public static String ResultCheck108, StepGroup, ROUTE_CODE, strGroupKP, strItemChecked;
        public bool checkLinked;
        public string str_pmemo, str_sn;
        int qty, i;
        public ucMainForm frmucMain = new ucMainForm();
        public MainWindow frmMain = new MainWindow ();
        

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            TReworkRefresh();
        }

        private async void btnOpenFileSN_Click(object sender, RoutedEventArgs e)
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
                                lstvSN_NG.Items.Add(sn + " Dup!");
                            }
                        }
                    }

                    lblQtyOK.Content = lstvSN_OK.Items.Count.ToString();
                    lblQtyNG.Content = lstvSN_NG.Items.Count.ToString();
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
                OPTION = "TREWORK_INPUTSN",
                SN = data
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
                    if (lblWipGoup.Content is null )
                    {
                        lblWipGoup.Content = RESArray[1].Replace("_", "__");
                    }
                    else
                    {
                        if ( lblWipGoup.Content.ToString() != RESArray[1].Replace("_", "__") )
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = "Have more than one WIP_GROUP  ";
                            frmMessage.MessageVietNam = "Có nhiều hơn một Wip_Group !";
                            frmMessage.ShowDialog();
                            TReworkRefresh();
                            return false;
                        }
                    }

                    if (cbbRouteName.Items.IndexOf(RESArray[2]) < 0)
                    {
                        cbbRouteName.Items.Add(RESArray[2]);
                    }

                    if (cbbRouteName.Items.Count > 1)
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "Have more than one route name ";
                        frmMessage.MessageVietNam = "Có nhiều hơn một tên lưu trình !";
                        frmMessage.ShowDialog();
                        TReworkRefresh();
                        return false;
                    }
                    int i = 0;
                    for (i = 0; i < int.Parse(RESArray[3].ToString()); i++)
                    {
                        if (cbbGroupName.Items.IndexOf(RESArray[4 + i]) < 0 )
                        {
                            cbbGroupName.Items.Add(RESArray[4 + i]);
                        }
                    }
                    return true;
                }
                else
                {
                    lstvSN_NG.Items.Add(data + RESArray[1]);
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
        private void TReworkRefresh ()
        {
            lblQtyNG.Content = "0";
            lblQtyOK.Content = "0";
            lstvSN_NG.Items.Clear();
            lstvSN_OK.Items.Clear();
            txtErrorCode.Text = "";
            txtReason.Text = "";
            cbbGroupName.Items.Clear();
            cbbRouteName.Items.Clear();
        }

        private void ucTRework_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime dateTime = DateTime.UtcNow.Date;

            txtReworkNo.Text = MainWindow.empNo + dateTime.ToString("yyMMdd");
        }

        private void cbOne_Click(object sender, RoutedEventArgs e)
        {
            if (cbOne.IsChecked == true)
            {
                cbAll.IsChecked = false;
                btnOpenFileSN.Visibility = Visibility.Hidden;
                txtInputSN.Focus();
                txtInputSN.SelectAll();

            }
            TReworkRefresh();
        }

        private void cbAll_Click(object sender, RoutedEventArgs e)
        {
            if (cbAll.IsChecked == true)
            {
                cbOne.IsChecked = false;
                btnOpenFileSN.Visibility = Visibility.Visible;
            }
            TReworkRefresh();
        }
        private async void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            string FUN;
            if (txtReason.Text == "")
            {
                MessageBox.Show("You aren't input reason", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (cbbGroupName.Text == "")
            { 
                MessageBox.Show(" Group next is null ", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (cbbRouteName.Text  == "" )
            {
                MessageBox.Show(" Route name is null ", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (cbOne.IsChecked == true)
            {
                FUN = "1";
            }
            else
                FUN = "2";

            try
            {
                Prob1.Value = 0;
                if (await frmucMain.GetReworkNO(MainWindow.empNo))
                {
                    txtReworkNo.Text = ucMainForm.REWORK_NO ;
                }
                else
                {
                    return;
                }
                qty = lstvSN_OK.Items.Count;

                var logInfo1 = new
                {
                    OPTION = "CHECK_REWORK_NO",
                    EMP = MainWindow.empNo,
                    REWORK_NO = txtReworkNo.Text,
                    ADDRESS = "; MAC ADDRESS: " + MainWindow.MACAddress + " ; IP: " + MainWindow.IP,
                    PMEMO = str_pmemo,
                    MODEL = cbbRouteName.Text,
                    GROUP = cbbGroupName.Text,
                    REASON = txtReason.Text,
                    QTY = qty.ToString()
                };
                string jsonData1 = JsonConvert.SerializeObject(logInfo1).ToString();
                var result1 = await MainWindow.sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.REWORK_API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter {Name="DATA" , Value=jsonData1, SfcParameterDataType= SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                        new SfcParameter {Name="OUTPUT" , SfcParameterDataType = SfcParameterDataType.Varchar2 ,SfcParameterDirection= SfcParameterDirection.Output}
                    }
                });
                if (result1.Data != null)
                {
                    dynamic output = result1.Data;
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

                while (lstvSN_OK.Items.Count > 0)
                {
                    i = i + 1;
                    var logInfo = new
                    {
                        OPTION = "TREWORK_EXECUTE",
                        SN = lstvSN_OK.Items[0].ToString(),
                        GROUP_NEXT = cbbGroupName.Text,
                        ROUTE_NAME = cbbRouteName.Text,
                        ERROR_CODE = txtErrorCode.Text,
                        EMP = MainWindow.empNo,
                        FUNTION = FUN,
                        REWORK_NO = txtReworkNo.Text,
                        MACIP = "IP: " + MainWindow.IP + "; MAC: " + MainWindow.MACAddress ,
                        QTY = lblQtyOK.Content,
                        REASON = txtReason.Text 
                    };
                    string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

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

                    if (result.Data != null)
                    {
                        dynamic output = result.Data;
                        string RES = output[0]["output"];
                        RESArray = RES.Split('#');
                        if (RESArray[0] == "OK")
                        {
                            if (lstvSN_OK.Items.Count == 0)
                            {
                                TReworkRefresh();
                                MessageBox.Show("Delete R_ success : " + qty + " pcs", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = RESArray[1];
                            frmMessage.MessageVietNam = "SFIS1.REWORK_API_EXECUTE / TREWORK_EXECUTE error !";
                            frmMessage.ShowDialog();
                            return;
                        }
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "Input sn error !";
                        frmMessage.MessageVietNam = "SFIS1.REWORK_API_EXECUTE / TREWORK_EXECUTE result null !";
                        frmMessage.ShowDialog();
                        return;
                    }

                    lstvSN_OK.Items.Remove(lstvSN_OK.Items[0]);
                    Prob1.Value = (i * 100 ) / qty ;
                    if (lstvSN_OK.Items.Count == 0)
                    {
                        TReworkRefresh();
                        MessageBox.Show("Delete R_ success : " + qty + " pcs", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageError frmMessage = new MessageError();
                frmMessage.CustomFlag = true;
                frmMessage.MessageEnglish = ex.Message.ToString();
                frmMessage.MessageVietNam = "Call procedure have exceptions:";
                frmMessage.ShowDialog();
                return;
            }
        }

        public static List<listModelName> itemsModel = new List<listModelName>();
        public ucTReworkForm()
        {
            InitializeComponent();
        }

        private async void txtInputSN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cbbGroupName.Items.Clear();
                cbbRouteName.Items.Clear();
                lblWipGoup.Content = "";
                try
                {
                    var logInfo = new
                    {
                        OPTION = "TREWORK_INPUTSN",
                        SN = txtInputSN.Text 
                    };
                    string jsonData = JsonConvert.SerializeObject(logInfo).ToString();

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

                    if (result.Data != null)
                    {
                        dynamic output = result.Data;
                        string RES = output[0]["output"];
                        RESArray = RES.Split('#');
                        if (RESArray[0] == "OK")
                        {
                            lblWipGoup.Content =  RESArray[1].Replace("_" ,"__");
                            lstvSN_OK.Items.Add(txtInputSN.Text);

                            if (cbbRouteName.Items.IndexOf(RESArray[2]) < 0)
                            {
                                cbbRouteName.Items.Add(RESArray[2]);
                            }

                            if (cbbRouteName.Items.Count > 1)
                            {
                                MessageError frmMessage = new MessageError();
                                frmMessage.CustomFlag = true;
                                frmMessage.MessageEnglish = "Have more than one route name ";
                                frmMessage.MessageVietNam = "Có nhiều hơn một tên lưu trình !";
                                frmMessage.ShowDialog();
                                TReworkRefresh();
                                return;
                            }
                            int i = 0;
                            for (i = 0; i < int.Parse(RESArray[3].ToString()); i++ )
                            {
                                cbbGroupName.Items.Add(RESArray[4 + i]);
                            }
                            cbbGroupName.SelectedIndex = 0;
                            cbbRouteName.SelectedIndex = 0;
                            lblQtyOK.Content = lstvSN_OK.Items.Count.ToString();
                            txtInputSN.SelectAll();
                            txtInputSN.Focus();
                            
                        }
                        else
                        {
                            MessageError frmMessage = new MessageError();
                            frmMessage.CustomFlag = true;
                            frmMessage.MessageEnglish = RESArray[1];
                            frmMessage.MessageVietNam = "SFIS1.REWORK_API_EXECUTE / InputSN_R_ error !";
                            frmMessage.ShowDialog();
                            return;
                        }
                    }
                    else
                    {
                        MessageError frmMessage = new MessageError();
                        frmMessage.CustomFlag = true;
                        frmMessage.MessageEnglish = "Input sn error !";
                        frmMessage.MessageVietNam = "SFIS1.REWORK_API_EXECUTE / InputSN_R_ result null !";
                        frmMessage.ShowDialog();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageError frmMessage = new MessageError();
                    frmMessage.CustomFlag = true;
                    frmMessage.MessageEnglish = ex.Message.ToString();
                    frmMessage.MessageVietNam = "Call procedure have exceptions:";
                    frmMessage.ShowDialog();
                    return;
                }
            }
        }
    }
}
