using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using PM.Model;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;

namespace PM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string checkSum;
        public SfcHttpClient sfcClient;
        public string loginInfor = "";
        public string loginApiUri = "";
        public string loginDB = "";
        public string empNo = "";
        public string empPass = "";
        public string inputLogin = "";

        public OptionForm frm_OptionForm = new OptionForm();
        //public LoadMOForm frm_LoadMOForm = new LoadMOForm();
        public TravelCardForm frm_TravelCardForm = new TravelCardForm();
        public CapacityForm frm_CapacityForm = new CapacityForm();
        public string local_strIP, IP,DB;
        public bool bCanPrint;
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Get localIP addresses
        public string localIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    local_strIP = ip.ToString();
                }
            }
            return local_strIP;
        }
        #endregion

        #region Open the TravelCardForm Screen
        private void btnTravelCard(object sender, MouseButtonEventArgs e)
        {
            //TravelCardForm frm_TravelCard = new TravelCardForm();
            //frm_TravelCard.ShowDialog();
        }
        #endregion

        #region Open the MO_ManageForm Screen
        public void btnMOManage(object sender, MouseButtonEventArgs e)
        {
            MO_ManageForm frm_MO_ManageForm = new MO_ManageForm(this, sfcClient);
            frm_MO_ManageForm.ShowDialog();
        }
        #endregion

        #region Open the InformationForm Screen
        private void btnWIPQuery(object sender, MouseButtonEventArgs e)
        {
            //InformationForm frm_InformationForm = new InformationForm();
            //frm_InformationForm.ShowDialog();
        }
        #endregion

        #region Open the RManageForm Screen
        private void btnRouteManage(object sender, MouseButtonEventArgs e)
        {
            RManageForm frm_RManageForm = new RManageForm();
            frm_RManageForm.ShowDialog();
        }
        #endregion

        #region Open the CapacityForm Screen
        private void btnCapacityQuery(object sender, MouseButtonEventArgs e)
        {
            //CapacityForm frm_CapacityForm = new CapacityForm();
            //frm_CapacityForm.ShowDialog();
        }
        #endregion

        #region Open the StbKeyForm Screen
        private void btnPirelliStbKey(object sender, MouseButtonEventArgs e)
        {
            //StbKeyForm frm_StbKeyForm = new StbKeyForm();
            //frm_StbKeyForm.ShowDialog();
        }
        #endregion

        #region Open the MoLinkForm Screen
        private void btnMOLINK(object sender, MouseButtonEventArgs e)
        {
            //MoLinkForm frm_MoLinkForm = new MoLinkForm();
            //frm_MoLinkForm.ShowDialog();
        }
        #endregion

        #region Open the MoLinkPIForm Screen
        private void btnMOLINKPI(object sender, MouseButtonEventArgs e)
        {
            //MoLinkPIForm frm_MoLinkPIForm = new MoLinkPIForm();
            //frm_MoLinkPIForm.ShowDialog();
        }
        #endregion

        #region Open the OrderForm Screen
        private void btnMOQuery(object sender, MouseButtonEventArgs e)
        {
            //OrderForm frm_OrderForm = new OrderForm();
            //frm_OrderForm.ShowDialog();
        }
        #endregion

        #region Open the InitForm Screen
        private void btnAboutPM(object sender, MouseButtonEventArgs e)
        {
            InitForm frm_InitForm = new InitForm();
            frm_InitForm.ShowDialog();
        }
        #endregion

        #region Exit Program
        private void btnLogout(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        #region Open the ConfirmDlgForm Screen
        private void item_QuitPM_Click(object sender, RoutedEventArgs e)
        {
            ConfirmDlg frm_ConfirmDlg = new ConfirmDlg();
            frm_ConfirmDlg.ShowDialog();
        }
        #endregion

        #region Exit Program
        private void item_Logout_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        private void item_SetCompany_Click(object sender, RoutedEventArgs e)
        {
            InitForm frm_InitForm = new InitForm();
            CompanyForm frm_CompanyForm = new CompanyForm();
            frm_CompanyForm.ShowDialog();
            try
            {
                INIFile ini = new INIFile("SFIS.ini");
                ini.Write("PM", "Company", frm_CompanyForm.Edt_CompanyName.Text);
                frm_InitForm.sCompanyName = frm_CompanyForm.Edt_CompanyName.Text;
            }
            catch (Exception ex)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "Excute have error:" + ex.Message;
                _er.MessageVietNam = "Có lỗi phát sinh: " + ex.Message;
                _er.ShowDialog();
                this.Close();
            }
        }
        private void item_Chinese_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            item_Chinese.IsChecked = true;
            item_English.IsChecked = false;
            ini.Write("LANGUAGES", "LANGUAGES", "C");
        }
        private void item_English_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            item_Chinese.IsChecked = false;
            item_English.IsChecked = true;
            ini.Write("LANGUAGES", "LANGUAGES", "E");
        }
        private void item_MOManage_Click(object sender, RoutedEventArgs e)
        {
            MO_ManageForm frm_MO_ManageForm = new MO_ManageForm(this,sfcClient);
            frm_MO_ManageForm.ShowDialog();
        }
        private async void item_MOModify_Click(object sender, RoutedEventArgs e)
        {
            string sErrMsgBuf;
            DelMOForm frm_DelMOForm = new DelMOForm(this,sfcClient);
            frm_DelMOForm.Title = "Modify MO";
            frm_DelMOForm.ShowDialog();
            string strGetMO = "SELECT r105.*, crn.route_name"
                + " FROM sfism4.r_mo_base_t r105, sfis1.c_route_name_t crn"
                + " WHERE close_flag LIKE :closeflag"
                + " AND crn.route_code = r105.route_code"
                + " AND close_flag <> '9' ORDER BY mo_number";
            var qry_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="CloseFlag",Value="%"}
                }
            });
            dynamic ads = qry_MO.Data;
            for (int i = 0;i < qry_MO.Data.Count(); i++)
            {
                if (ads[i]["mo_number"] == frm_DelMOForm.Cbb_MO.Text)
                {
                    break;
                }
            }
            if (ads[0]["close_flag"] == "3")
            {
                sErrMsgBuf = $"MO {ads[0]["mo_number"]} had closed, can't modify!";
                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MO_InsertForm frm_MO_InsertForm = new MO_InsertForm();
            frm_MO_InsertForm.Mode = "Modify MO";
            frm_MO_InsertForm.Title = "MODIFY MO";
            frm_MO_InsertForm.ShowDialog();
        }
        private void item_HistoryMO_Click(object sender, RoutedEventArgs e)
        {
            HistoryMOForm frm_HistoryMOForm = new HistoryMOForm();
            frm_HistoryMOForm.ShowDialog();
        }
        private void item_AboutPM_Click(object sender, RoutedEventArgs e)
        {
            InitForm frm_InitForm = new InitForm();
            frm_InitForm.ShowDialog();
        }
        public static string GetChecksum(HashingAlgoTypes hashingAlgoType, string filename)
        {
            using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgoType.ToString()))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    var hash = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "");
                }
            }
        }
        public enum HashingAlgoTypes
        {
            MD5,
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }
        private async void mainWindow_Initialized(object sender, EventArgs e)
        {
            string[] Args = Environment.GetCommandLineArgs();

            if (Args.Length == 1)
            {
                MessageBox.Show("ONLY OPEN FROM SFIS_5.0 PROGRAM", "WARRING", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                Environment.Exit(0);
            }
            foreach (string s in Args)
            {
                inputLogin = s.ToString();
            }
            string[] argsInfor = Regex.Split(inputLogin, @";");
            checkSum = argsInfor[0].ToString();
            loginApiUri = argsInfor[1].ToString();
            loginDB = argsInfor[2].ToString();
            empNo = argsInfor[3].ToString();
            empPass = argsInfor[4].ToString();

            var loginInfo = new
            {
                TYPE = "LOGIN",
                PRG_NAME = "PM",
                UserName = empNo,
                Password = empPass
            };

            //Tranform it to Json object
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            sfcClient = DBApi.sfcClient(loginDB, loginApiUri);
            try
            {
                await sfcClient.GetAccessTokenAsync(empNo, empPass);

                var result1 = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic ads = result1.Data;
                string Ok = ads[0]["output"];

                if (Ok.Substring(0, 2) == "OK")
                {
                    CheckPrivilege();
                    loginInfor = Ok.Substring(3, Ok.Length - 3).Trim();
                    string[] digits = Regex.Split(loginInfor, @";");
                    DB = digits[0].ToString();
                }
                else
                {
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = "Excute procedure have error: " + Ok;
                    _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + Ok;
                    _er.ShowDialog();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "Excute procedure have error:" + ex.Message;
                _er.MessageVietNam = "Có lỗi phát sinh trong thủ tục: " + ex.Message;
                _er.ShowDialog();
                this.Close();
            }
            await UpdateLastVerSetting();
            INIFile ini = new INIFile("SFIS.ini");
            string strTmp = ini.Read("LANGUAGES", "LANGUAGES");
            if (strTmp == "C")
            {
                item_Chinese.IsChecked = true;
            }
            else if (strTmp == "E")
            {
                item_English.IsChecked = true;
            }
        }
        public async Task UpdateLastVerSetting()
        {
            string strGetData = "SELECT * FROM SFIS1.C_PARAMETER_INI";
            var result = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });

            if (result.Data.Count() > 0) return;

            string strUpdate = "UPDATE SFISM4.R_MO_BASE_T SET MO_OPTION = 'Default' WHERE MO_OPTION IS NULL";
            var performed = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strUpdate,
                SfcCommandType = SfcCommandType.Text
            });

            INIFile ini = new INIFile("SFIS.ini");

            try
            {
                WriteIniValue("PM", "Default", "LENGTH", "SN", ini.Read("PM", "SN_LENGTH"),"");
                WriteIniValue("PM", "Default", "LENGTH", "MO", ini.Read("PM", "MO_LENGTH"), "");
                WriteIniValue("PM", "Default", "LENGTH", "CurrentNO", ini.Read("PM", "CurrentNO_LENGTH"), "");
                WriteIniValue("PM", "Default", "LENGTH", "CurrentNOStartDigit", ini.Read("PM", "CurrentNOStartDigit"), "");
                if (bool.Parse(ini.Read("PM", "SN_Fixed_LENGTH")) == true)
                {
                    WriteIniValue("PM", "Default", "SN", "Fixed_LENGTH", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "SN", "Fixed_LENGTH", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "MO_Fixed_LENGTH")) == true)
                {
                    WriteIniValue("PM", "Default", "MO", "Fixed_LENGTH", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "MO", "Fixed_LENGTH", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_Bom NO")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "Bom NO", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "Bom NO", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_HW Bom")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "HW Bom", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "HW Bom", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_MO Type")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "MO Type", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "MO Type", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_Option")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "Option", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "Option", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_Part NO")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "Part NO", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "Part NO", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_PO")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "PO", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "PO", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_SW Bom")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "SW Bom", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "SW Bom", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_UPC")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "UPC", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "UPC", "FALSE", "");
                }

                if (bool.Parse(ini.Read("PM", "Field_Version")) == true)
                {
                    WriteIniValue("PM", "Default", "Field", "Version", "TRUE", "");
                }
                else
                {
                    WriteIniValue("PM", "Default", "Field", "Version", "FALSE", "");
                }

                if (ini.ReadString("PM", "CurrentNOType", "Numerals") == "Numerals")
                {
                    WriteIniValue("PM", "Default", "Type", "CurrentNO", "Numerals", "");
                    WriteIniValue("PM", "Default", "Type", "Decimal", ini.Read("PM", "DecimalType"), "");
                }
                else
                {
                    if (ini.ReadString("PM", "CurrentNOType", "Other") == "Numerals")
                    {
                        WriteIniValue("PM", "Default", "Type", "CurrentNO", "Numerals", "");
                        if (bool.Parse(ini.Read("PM", "ALLNumberFlag")) == true)
                        {
                            WriteIniValue("PM", "Default", "Flag", "ALLNumber", "TRUE", "");
                            WriteIniValue("PM", "Default", "Number", "ALL", ini.ReadString("PM", "ALLNumber", "0123456789"), "");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void WriteIniValue(string _sPrg,string _sVClass,string _sVItem,string _sVName,string _sVValue,string _sVDesc)
        {
            string strInsert = " INSERT INTO SFIS1.C_PARAMETER_INI"
                + " (PRG_NAME,VR_CLASS,VR_ITEM,VR_NAME,VR_VALUE,VR_DESC)"
                + " VALUES (:sPRG,:sVClass,:sVItem,:sVName,:sVValue,:sVDesc)";
            var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strInsert,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="sPRG",Value=_sPrg},
                    new SfcParameter{Name="sPRG",Value=_sVClass},
                    new SfcParameter{Name="sPRG",Value=_sVItem},
                    new SfcParameter{Name="sPRG",Value=_sVName},
                    new SfcParameter{Name="sPRG",Value=_sVValue},
                    new SfcParameter{Name="sPRG",Value=_sVDesc}
                }
            });
        }
        private async void CheckPrivilege()
        {
            FormShow_CheckPrivilege();
            string strPrivilege = "SELECT * FROM SFIS1.C_PRIVILEGE A, SFIS1.C_EMP_DESC_T B"
                + " WHERE B.EMP_BC = :sUser AND B.QUIT_DATE > SYSDATE AND B.EMP_NO = A.EMP AND A.PRG_NAME = 'PM'";
            var qry_privilege = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strPrivilege,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="sUser",Value=empPass}
                }
            });
            if (qry_privilege.Data.Count() > 0)
            {
                if (empNo == "ambitsfis")
                {
                    this.menu_Option.Visibility = Visibility.Visible;
                }
                else
                {
                    this.menu_Option.Visibility = Visibility.Visible;
                }
                //Assesment privilege
                List<ListPrivilege> result = qry_privilege.Data.ToListObject<ListPrivilege>().ToList();
                for (int i = 0; i < qry_privilege.Data.Count(); i++)
                {
                    if (result[i].FUN == "MO_MANAGE")
                    {
                        if (result[i].PRIVILEGE == 0)
                        {
                            this.Lb_MOManage.IsEnabled = false;
                            this.MOManage.IsEnabled = false;
                            this.item_MOManage.IsEnabled = false;
                        }
                        else
                        {
                            //Value is 1 or 2
                            this.Lb_MOManage.IsEnabled = true;
                            this.MOManage.IsEnabled = true;
                            this.item_MOManage.IsEnabled = true;
                        }
                    }
                    else if (result[i].FUN == "MO_LINK")
                    {
                        if (result[i].PRIVILEGE == 2)
                        {
                            this.Lb_MOLINK.Visibility = Visibility.Visible;
                            this.MOLINK.Visibility = Visibility.Visible;
                            this.Lb_MOLINK.IsEnabled = true;
                            this.MOLINK.IsEnabled = true;
                        }
                        else
                        {
                            this.Lb_MOLINK.Visibility = Visibility.Hidden;
                            this.MOLINK.Visibility = Visibility.Hidden;
                            this.Lb_MOLINK.IsEnabled = false;
                            this.MOLINK.IsEnabled = false;
                        }
                    }
                    else if (result[i].FUN == "ROUTE_MANAGE")
                    {
                        if (result[i].PRIVILEGE == 2)
                        {
                            this.Lb_RouteManage.Visibility = Visibility.Visible;
                            this.RouteManage.Visibility = Visibility.Visible;
                            this.Lb_MOLINK.IsEnabled = true;
                            this.MOQuery.IsEnabled = true;
                        }
                        else
                        {
                            this.Lb_RouteManage.Visibility = Visibility.Hidden;
                            this.RouteManage.Visibility = Visibility.Hidden;
                            this.Lb_MOLINK.IsEnabled = false;
                            this.MOQuery.IsEnabled = false;
                        }
                    }
                    else if (result[i].FUN == "MO_MODIFY")
                    {
                        if (result[i].PRIVILEGE == 2)
                        {
                            this.item_MOModify.IsEnabled = true;
                        }
                        else
                        {
                            this.item_MOModify.IsEnabled = false;
                        }
                    }
                    else if (result[i].FUN == "MO_DELETE")
                    {
                       
                    }
                    else if (result[i].FUN == "MO_SAVE")
                    {
                        if (result[i].PRIVILEGE == 0)
                        {
                            bCanPrint = false;
                        }
                        else
                        {
                            bCanPrint = true;
                        }
                    }
                    else if (result[i].FUN == "MO_HISTORY")
                    {
                        if (result[i].PRIVILEGE == 0)
                        {
                            this.item_HistoryMO.IsEnabled = false;
                        }
                        else
                        {
                            this.item_HistoryMO.IsEnabled = true;
                        }
                    }
                    else if (result[i].FUN == "TRAVEL")
                    {
                        if (result[i].PRIVILEGE == 0)
                        {
                            this.Lb_TravelCard.IsEnabled = false;
                            this.TravelCard.IsEnabled = false;
                            frm_TravelCardForm.WIPDetail.IsEnabled = false;
                        }
                        else
                        {
                            this.Lb_TravelCard.IsEnabled = true;
                            this.TravelCard.IsEnabled = true;
                            frm_TravelCardForm.WIPDetail.IsEnabled = true;
                        }
                    }
                    else if (result[i].FUN == "TRAVEL_WIP_DETAIL")
                    {
                        if (result[i].PRIVILEGE == 0)
                        {
                            frm_TravelCardForm.WIPDetail.IsEnabled = false;
                        }
                        else
                        {
                            frm_TravelCardForm.WIPDetail.IsEnabled = true;
                        }
                    }
                    else if (result[i].FUN == "CAPACITY")
                    {
                        if (result[i].PRIVILEGE == 0)
                        {
                            frm_CapacityForm.btnShipdetail.IsEnabled = false;
                            this.Lb_CapacityQuery.IsEnabled = false;
                            this.CapacityQuery.IsEnabled = false;
                        }
                        else
                        {
                            frm_CapacityForm.btnShipdetail.IsEnabled = false;
                            this.Lb_CapacityQuery.IsEnabled = true;
                            this.CapacityQuery.IsEnabled = true;
                        }
                    }
                    else if (result[i].FUN == "CAPACITY_SHIP_DETAIL")
                    {
                        if (result[i].PRIVILEGE == 0)
                        {
                            frm_CapacityForm.btnShipdetail.IsEnabled = false;
                        }
                        else
                        {
                            frm_CapacityForm.btnShipdetail.IsEnabled = true;
                        }
                    }
                    else if (result[i].FUN == "MO_LINK_PI")
                    {
                        if (result[i].PRIVILEGE == 2)
                        {
                            this.Lb_MOLINKPI.Visibility = Visibility.Visible;
                            this.MOLINKPI.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.Lb_MOLINKPI.Visibility = Visibility.Hidden;
                            this.MOLINKPI.Visibility = Visibility.Hidden;
                        }
                    }
                    else if (result[i].FUN == "PIRELLISTBKEY")
                    {
                        if (result[i].PRIVILEGE == 2)
                        {
                            this.Lb_PirelliStbKey.Visibility = Visibility.Visible;
                            this.PirelliStbKey.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.Lb_PirelliStbKey.Visibility = Visibility.Hidden;
                            this.PirelliStbKey.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            else
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "00003 - " + await GetPubMessage("00003");
                _er.MessageVietNam = "00003 - " + await GetPubMessage("00003");
                _er.ShowDialog();
                this.Close();
                return;
            }
        }
        public void FormShow_CheckPrivilege()
        {
            this.menu_Option.Visibility = Visibility.Hidden;
            this.Lb_MOManage.IsEnabled = false;
            this.MOManage.IsEnabled = false;
            this.item_MOManage.IsEnabled = false;
            this.item_MOModify.IsEnabled = false;
            this.item_HistoryMO.IsEnabled = false;
            this.Lb_TravelCard.IsEnabled = false;
            this.TravelCard.IsEnabled = false;
            this.Lb_CapacityQuery.IsEnabled = false;
            this.CapacityQuery.IsEnabled = false;
            this.Lb_MOLINK.IsEnabled = false;
            this.MOLINK.IsEnabled = false;
        }
        public async Task<string> GetPubMessage(string PROMPT_CODE)
        {
            var query_lenguage = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = "SELECT PROMPT_ENGLISH FROM SFIS1.C_PROMPT_CODE_T WHERE PROMPT_CODE = :prompt",
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter {Name="prompt", Value=PROMPT_CODE }
                }
            });
            if (query_lenguage.Data != null)
            {
                string result = query_lenguage.Data["prompt_english"].ToString();
                return result;
            }
            return null;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
