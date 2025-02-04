using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Management;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using PM.Model;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using Microsoft.Win32;
using System.Data;

namespace PM
{
    /// <summary>
    /// Interaction logic for MO_ManageForm.xaml
    /// </summary>
    public partial class MO_ManageForm : Window
    {
        public SfcHttpClient sfcClient;
        public MainWindow frm_main;
        public LoadMOForm frm_LoadMOForm;
        public MO_InsertForm frm_MO_InsertForm = new MO_InsertForm();
        Grid momentGrid = new Grid();
        public Setup_Pending_GroupForm frm_Setup_Pending_GroupForm = new Setup_Pending_GroupForm();
        public ObservableCollection<ListMO> _ListMO { get; set; }
        public ObservableCollection<ListRange> _ListRange { get; set; }
        public bool LOADFLAG;
        public bool sPrivilige_ModifyMOQty = false;
        public bool sPrivilige_ModifyMOLink = false;
        public string MO_Status = "";
        public string local_strIP,_EMP,_EMP_PASS;
        public MO_ManageForm()
        {
            InitializeComponent();
        }
        public static ObservableCollection<ListMO> Convert(IEnumerable original)
        {
            return new ObservableCollection<ListMO>(original.Cast<ListMO>());
        }
        public static ObservableCollection<ListRange> Convert_Range(IEnumerable original)
        {
            return new ObservableCollection<ListRange>(original.Cast<ListRange>());
        }
        public MO_ManageForm(MainWindow main, SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            frm_main = main;
            FormShow_CheckPrivilege();
            MO_ManageForm_FormShow();
            InitializeComponent();
            Edt_SearchMO.Focus();
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            StackPanel stack = grid.Children.Cast<StackPanel>().ToList()[0];
            var childStack = stack.Children.Cast<Control>().ToList();
            PackIcon icon = childStack[0] as PackIcon;
            icon.Height = 30;
            icon.Width = 30;
            Label label = childStack[1] as Label;
            label.FontSize = 15;

            grid.Cursor = Cursors.Hand;
        }
        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;
            StackPanel stack = grid.Children.Cast<StackPanel>().ToList()[0];
            var childStack = stack.Children.Cast<Control>().ToList();
            PackIcon icon = childStack[0] as PackIcon;
            icon.Height = 25;
            icon.Width = 25;
            Label label = childStack[1] as Label;
            label.FontSize = 14;
        }
        private async void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            StackPanel stack = grid.Children.Cast<StackPanel>().ToList()[0];
            var childStack = stack.Children.Cast<Control>().ToList();
            Label label = childStack[1] as Label;
            grid.Background = (Brush)new BrushConverter().ConvertFromString("#0000FF");
            label.FontWeight = FontWeights.Bold;
            switch (label.Content.ToString())
            {
                case "Go Top":
                    this.Close();
                    break;
                case "New":
                    _NEW();
                    break;
                case "On Line":
                    await _OnLine();
                    break;
                case "Un Online":
                    await _UnOnLine();
                    break;
                case "MO Finish":
                    await _MOFinish();
                    break;
                case "Delete MO":
                    await _Delete();
                    break;
                case "Pending":
                    await _Pending();
                    break;
                case "Save":
                    _Print();
                    break;
                case "Modify MO":
                    await _Modify();
                    break;
                case "KP Completed":
                    _KPCompleted();
                    break;
                case "Refresh MO":
                    _Refresh();
                    break;
                case "System":
                    _System();
                    break;
                case "Default":
                    break;
            }
            grid.Background = (Brush)new BrushConverter().ConvertFromString("#008082");
        }

        public void _NEW()
        {
            LOADFLAG = false;
            LoadMOForm frm_LoadMOForm = new LoadMOForm(this,sfcClient);
            frm_LoadMOForm.ShowDialog();
            if (LOADFLAG)
            {
                MO_InsertForm frm_MO_InsertForm = new MO_InsertForm(this, frm_LoadMOForm, sfcClient);
                frm_MO_InsertForm.ShowDialog();
            }
            else
            {
                return;
            }
        }
        private async Task _OnLine()
        {
            string sVersion, sMo_Kind, sMailsub, sKey;
            string sModelType, sMo_Type;
            string sStartMSN, sEndMSN, sStartIMEI, sEndIMEI;
            sStartMSN = "";
            sEndMSN = "";
            sStartIMEI = "";
            sEndIMEI = "";
            string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                    + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                    + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                    + " ORDER BY MO_NUMBER";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                    new SfcParameter{ Name="MO_NUMBER",Value=Edt_MO.Text}
                }
            });
            if (qry_DataMO.Data.Count() > 0)
            {
                List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
                if (result[0].MO_OPTION == "LOADFROMBPCS")
                {
                    if (Int32.Parse(result[0].CLOSE_FLAG) >= 2)
                    {
                        MessageBox.Show("Công lệnh đã Online -- Mo has Online", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if ((await GetVersion(result[0].MO_NUMBER) == "N/A") || (await GetVersion(result[0].MO_NUMBER) == "NA"))
                    {
                        MessageBox.Show("80180 - Phiên bản trong MO_EXT của công lệnh sai -- Please check VERSION_CODE on R_mo_ext_t", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else 
                    {
                        //sVersion = await GetVersion(result[0].MO_NUMBER);
                        sMo_Kind = result[0].LOT_FLAG;
                    }
                    string strGetMO = "Select A.Model_Type,B.Mo_Type,b.version_code From SFIS1.C_Model_Desc_T A,SFISM4.R_Mo_Base_T B"
                        + " Where A.Model_Name=B.Model_Name AND B.Mo_Number=:Mo_Number";
                    var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetMO,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="Mo_Number",Value=result[0].MO_NUMBER}
                        }
                    });
                    sModelType = qry_MO.Data["model_type"].ToString() == null ? qry_MO.Data["model_type"].ToString():"";
                    sMo_Type = qry_MO.Data["mo_type"].ToString();
                    sVersion = qry_MO.Data["version_code"].ToString();
                    if (sModelType.IndexOf("333") > -1)
                    {
                        sVersion = qry_MO.Data["version_code"].ToString();
                    }
                    if (await GetReuseSNvalue(result[0].MO_NUMBER))
                    {
                        sVersion = qry_MO.Data["version_code"].ToString();
                    }
                    if (sModelType.IndexOf("015") > -1)
                    {
                        if (await CheckMORANGE(result[0].MO_NUMBER) == false)
                        {
                            return;
                        }
                    }
                    if ((sMo_Type == "Rework") || (sMo_Type == "Consigned") || (sMo_Type == "Pilot Run"))
                    {
                        sMailsub = System.DateTime.Now.ToString("MM/DD") + $"MO : {result[0].MO_NUMBER }" + " Online,Vui long xac nhan lai";
                        await MailInsert("NPIMOCONFIG", "sMailsub", "");
                    }
                    if (sModelType.IndexOf("R") > -1)
                    {
                        if (await CheckMOPackRange(result[0].MO_NUMBER, sStartMSN, sEndMSN, sStartIMEI, sEndIMEI) == false)
                        {
                            return;
                        }
                    }
                    if ((sMo_Type == "Rework") && (string.IsNullOrEmpty(sVersion)))
                    {
                        string strGetRULE = "Select A.* FROM SFIS1.C_CUSTSN_RULE_T A,SFISM4.R_MO_BASE_T B"
                            + " WHERE A.MODEL_NAME=B.MODEL_NAME AND A.CUSTSN_CODE LIKE 'SSN%' AND A.COMPARE_SN='SN' AND B.Mo_Number=:Mo_Number";
                        var qry_RULE = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetRULE,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="Mo_Number",Value=result[0].MO_NUMBER}
                        }
                        });
                        if (qry_RULE.Data.Count() > 0)
                        {
                            if ((sVersion == "N/A") || (sVersion == "NA"))
                            {
                                MessageBox.Show("80180 - Phiên bản trong MO_EXT của công lệnh sai -- Please check VERSION_CODE on R_mo_ext_t", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
                    }
                    if ((!string.IsNullOrEmpty(sVersion)) || (sMo_Kind == "LOT") || (result[0].MO_NUMBER.Substring(0, 4) == "2088") || (await GetReuseSNvalue(result[0].MO_NUMBER)) || (sModelType.IndexOf("333") > -1))
                    {
                        await UpdateCloseFlag(result[0].MO_NUMBER, sVersion);
                        await UpdateMOOnline(result[0].MO_NUMBER);
                        if (sModelType.IndexOf("R") > -1)
                        {
                            await UpdateMOPackRange(result[0].MO_NUMBER, sStartMSN, sEndMSN, sStartIMEI, sEndIMEI);
                        }
                        try
                        {
                            //------ Save To Log ------//
                            string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                                    + " VALUES ("
                                    + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                            var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strInsert_Log,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="EMP_NO",Value=frm_main.empNo},
                                    new SfcParameter{Name="PRG_NAME",Value="PM"},
                                    new SfcParameter{Name="ACTION_TYPE",Value="On Line"},
                                    new SfcParameter{Name="ACTION_DESC",Value= $"MO:{result[0].MO_NUMBER}"}
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        MessageBox.Show($"Online MO:{result[0].MO_NUMBER} Successful","PM",MessageBoxButton.OK,MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        MessageBox.Show("Vung gia tri Mo trong EXT bang khong ton tai -- The ext table wo range is nonexistent", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    return;
                }
                sKey = result[0].MO_NUMBER;
                if (Int32.Parse(result[0].CLOSE_FLAG) >= 2)
                {
                    MessageBox.Show("00231 - Cong lenh da duoc nhap -- Mo has input", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MO_InputForm frm_MO_InputForm = new MO_InputForm(this, sfcClient);
                    frm_MO_InputForm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("00237 - Khong tim thay du lieu -- Not found data. Search from first record", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private async Task _UnOnLine()
        {
            string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                + " ORDER BY MO_NUMBER";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                    new SfcParameter{ Name="MO_NUMBER",Value=Edt_MO.Text}
                }
            });
            if (qry_DataMO.Data.Count() > 0)
            {
                List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
                if (result[0].MO_OPTION == "LOADFROMBPCS")
                {
                    if (Int32.Parse(result[0].CLOSE_FLAG) >= 2)
                    {
                        string strGetMO = "SELECT SERIAL_NUMBER FROM  SFISM4.R_WIP_TRACKING_T WHERE MO_NUMBER=:MO_NUMBER";
                        var qry_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strGetMO,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="MO_NUMBER",Value=result[0].MO_NUMBER}
                            }
                        });
                        if (qry_MO.Data.Count() > 0)
                        {
                            MessageBox.Show($"This MO: { result[0].MO_NUMBER} has input","PM",MessageBoxButton.OK,MessageBoxImage.Error);
                            return;
                        }
                        else
                        {
                            //------ Update CLOSE_FLAG------//
                            string strUpdate = "UPDATE SFISM4.R_MO_BASE_T SET CLOSE_FLAG=1,VERSION_CODE ='' WHERE MO_NUMBER=:MO_NUMBER";
                            var Update_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strUpdate,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="MO_NUMBER",Value=result[0].MO_NUMBER}
                                }
                            });

                            //------ Save To Log ------//
                            string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                                    + " VALUES ("
                                    + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                            var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strInsert_Log,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="EMP_NO",Value=frm_main.empNo},
                                    new SfcParameter{Name="PRG_NAME",Value="PM"},
                                    new SfcParameter{Name="ACTION_TYPE",Value="UNOn Line"},
                                    new SfcParameter{Name="ACTION_DESC",Value= $"MO : {result[0].MO_NUMBER}"}
                                }
                            });
                            //Lb_CountMO.Content = qry_DataMO.Data.Count();
                            if (qry_DataMO.Data.Count() == 0)
                            {
                                Save.IsEnabled = false;
                            }
                            else
                            {
                                if (frm_main.bCanPrint)
                                {
                                    Save.IsEnabled = true;
                                }
                                else
                                {
                                    Save.IsEnabled = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        private async Task _MOFinish()
        {
            string MO_Finish;

            DelMOPWDForm frm_DelMOPWDForm = new DelMOPWDForm();
            frm_DelMOPWDForm.ShowDialog();
            if (!string.IsNullOrEmpty(frm_DelMOPWDForm.Edt_DelMOPwd.Password))
            {
                if (frm_DelMOPWDForm.Edt_DelMOPwd.Password == frm_main.empPass)
                {
                    string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                    + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                    + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                    + " ORDER BY MO_NUMBER";
                    var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetDataMO,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                            new SfcParameter{ Name="MO_NUMBER",Value=Edt_MO.Text}
                        }
                    });
                    if (qry_DataMO.Data.Count() > 0)
                    {
                        List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
                        MO_Finish = result[0].MO_NUMBER;
                        MessageBoxResult _result = MessageBox.Show($"Close MO : {MO_Finish} ?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (_result == MessageBoxResult.No)
                        {
                            frm_DelMOPWDForm.Close();
                            return;
                        }
                        else
                        {
                            string strUpdate = "UPDATE SFISM4.R_MO_BASE_T SET MO_CLOSE_DATE = SYSDATE,CLOSE_FLAG = :CloseFlag WHERE MO_NUMBER= :MO_NUMBER";
                            var Update_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strUpdate,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                            {
                               // new SfcParameter{Name="CloseDate",Value= System.DateTime.Now},
                                new SfcParameter{Name="MO_NUMBER",Value=MO_Finish},
                                new SfcParameter{Name="CloseFlag",Value="3"}
                            }
                            });

                            //------ Save To Log ------//
                            string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                                    + " VALUES ("
                                    + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                            var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strInsert_Log,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="EMP_NO",Value=frm_main.empNo},
                                    new SfcParameter{Name="PRG_NAME",Value="PM"},
                                    new SfcParameter{Name="ACTION_TYPE",Value="Finish"},
                                    new SfcParameter{Name="ACTION_DESC",Value= $"MO : {result[0].MO_NUMBER}"}
                                }
                            });

                            //Lb_CountMO.Content = qry_DataMO.Data.Count();
                            if (qry_DataMO.Data.Count() == 0)
                            {
                                Save.IsEnabled = false;
                            }
                            else
                            {
                                if (frm_main.bCanPrint)
                                {
                                    Save.IsEnabled = true;
                                }
                                else
                                {
                                    Save.IsEnabled = false;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nhap sai mat khau -- Password error", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Vui long nhap mat khau -- Please input PASSWORD", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private async Task _Delete()
        {
            string MO_Delete;

            DelMOPWDForm frm_DelMOPWDForm = new DelMOPWDForm();
            frm_DelMOPWDForm.ShowDialog();
            if (frm_DelMOPWDForm.Edt_DelMOPwd.Password == frm_main.empPass)
            {
                string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                + " ORDER BY MO_NUMBER";
                var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                        new SfcParameter{ Name="MO_NUMBER",Value=Edt_MO.Text}
                    }
                });
                if (qry_DataMO.Data.Count() > 0)
                {
                    List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
                    MO_Delete = result[0].MO_NUMBER;
                    MessageBoxResult _result = MessageBox.Show($"Delete MO : {MO_Delete} ?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (_result == MessageBoxResult.No)
                    {
                        frm_DelMOPWDForm.Close();
                        return;
                    }
                    else
                    {
                        try
                        {

                        }
                        catch (Exception ex)
                        {
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = ex.Message;
                            _er.MessageVietNam = ex.Message;
                            _er.ShowDialog();
                            return;
                        }
                        ////------ Save To Log ------//
                        //string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                        //        + " VALUES ("
                        //        + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                        //var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        //{
                        //    CommandText = strInsert_Log,
                        //    SfcCommandType = SfcCommandType.Text,
                        //    SfcParameters = new List<SfcParameter>()
                        //{
                        //    new SfcParameter{Name="EMP_NO",Value=frm_main.empNo},
                        //    new SfcParameter{Name="PRG_NAME",Value="PM"},
                        //    new SfcParameter{Name="ACTION_TYPE",Value="Finish"},
                        //    new SfcParameter{Name="ACTION_DESC",Value= $"MO : {result[0].MO_NUMBER}"}
                        //}
                        //});

                        ////Lb_CountMO.Content = qry_DataMO.Data.Count();
                        //if (qry_DataMO.Data.Count() == 0)
                        //{
                        //    Save.IsEnabled = false;
                        //}
                        //else
                        //{
                        //    if (frm_main.bCanPrint)
                        //    {
                        //        Save.IsEnabled = true;
                        //    }
                        //    else
                        //    {
                        //        Save.IsEnabled = false;
                        //    }
                        //}
                    }
                }
            }
            else
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = "00087 - " + await frm_main.GetPubMessage("00087");
                _er.MessageVietNam = "00087 - " + await frm_main.GetPubMessage("00087");
                _er.ShowDialog();
                return;
            }
        }
        private async Task _Pending()
        {
            string MO_Pending;

            DelMOPWDForm frm_DelMOPWDForm = new DelMOPWDForm();
            string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                    + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                    + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                    + " ORDER BY MO_NUMBER";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                    new SfcParameter{ Name="MO_NUMBER",Value=Edt_MO.Text}
                }
            });

            List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
            if (result[0].CLOSE_FLAG == "1")
            {
                MessageBox.Show("Choose Online MO -- Chon Online MO", "PM", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (result[0].CLOSE_FLAG == "5")
            {
                frm_DelMOPWDForm.ShowDialog();
                if (frm_DelMOPWDForm.Edt_DelMOPwd.Password == frm_main.empPass)
                {
                    MO_Pending = result[0].MO_NUMBER;
                    MessageBoxResult _result = MessageBox.Show($"Determined MO : {MO_Pending} ?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (_result == MessageBoxResult.No)
                    {
                        frm_DelMOPWDForm.Close();
                        return;
                    }
                    else
                    {
                        string strUpdate = "UPDATE SFISM4.R_MO_BASE_T SET MO_CLOSE_DATE = :CloseDate,CLOSE_FLAG = :CloseFlag WHERE MO_NUMBER= :MO_NUMBER";
                        var Update_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strUpdate,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="CloseDate",Value=System.DateTime.Now},
                                new SfcParameter{Name="MO_NUMBER",Value=MO_Pending},
                                new SfcParameter{Name="CloseFlag",Value="2"}
                            }
                        });

                        //------ Save To Log ------//
                        string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                                + " VALUES ("
                                + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                        var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                        {
                            CommandText = strInsert_Log,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="EMP_NO",Value=frm_main.empNo},
                                new SfcParameter{Name="PRG_NAME",Value="PM"},
                                new SfcParameter{Name="ACTION_TYPE",Value="Determined"},
                                new SfcParameter{Name="ACTION_DESC",Value= $"MO : {result[0].MO_NUMBER}"}
                            }
                        });

                        if (qry_DataMO.Data.Count() == 0)
                        {
                            Save.IsEnabled = false;
                        }
                        else
                        {
                            if (frm_main.bCanPrint)
                            {
                                Save.IsEnabled = true;
                            }
                            else
                            {
                                Save.IsEnabled = false;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Nhap sai mat khau -- Password error", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                frm_DelMOPWDForm.ShowDialog();
                if (frm_DelMOPWDForm.Edt_DelMOPwd.Password != frm_main.empPass)
                {
                    MessageBox.Show("Password is not accurate!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MO_Pending = result[0].MO_NUMBER;
                MessageBoxResult _result = MessageBox.Show($"Pending MO : {MO_Pending}", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (_result == MessageBoxResult.No)
                {
                    frm_DelMOPWDForm.Close();
                    return;
                }
                else
                {
                    Setup_Pending_GroupForm frm_Setup_Pending_GroupForm = new Setup_Pending_GroupForm(this,sfcClient);
                    frm_Setup_Pending_GroupForm.ShowDialog();
                    if (frm_Setup_Pending_GroupForm.LstSectionName.Items.Count > 0)
                    {
                        await Insert_Pending_Group(MO_Pending);
                    }

                    string strUpdate = "UPDATE SFISM4.R_MO_BASE_T SET MO_CLOSE_DATE = sysdate,CLOSE_FLAG = :CloseFlag WHERE MO_NUMBER= :MO_NUMBER";

                    var Update_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strUpdate,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                          //  new SfcParameter{Name="CloseDate",Value=System.DateTime.Now},
                            new SfcParameter{Name="MO_NUMBER",Value=MO_Pending},
                            new SfcParameter{Name="CloseFlag",Value="5"}
                        }
                    });

                    //------ Save To Log ------//
                    string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                            + " VALUES ("
                            + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                    var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsert_Log,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="EMP_NO",Value=frm_main.empNo},
                            new SfcParameter{Name="PRG_NAME",Value="PM"},
                            new SfcParameter{Name="ACTION_TYPE",Value="Pending"},
                            new SfcParameter{Name="ACTION_DESC",Value= $"MO : {result[0].MO_NUMBER}"}
                        }
                    });

                    if (qry_DataMO.Data.Count() == 0)
                    {
                        Save.IsEnabled = false;
                    }
                    else
                    {
                        if (frm_main.bCanPrint)
                        {
                            Save.IsEnabled = true;
                        }
                        else
                        {
                            Save.IsEnabled = false;
                        }
                    }
                }
            }
        }
        private async void _Print()
        {
            string FileName = "";
            string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag"
                + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                + " ORDER BY MO_NUMBER";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="CloseFlag",Value=MO_Status}
                }
            });
            List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
            int count = qry_DataMO.Data.Count();
            string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\";
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook workbook;
            Microsoft.Office.Interop.Excel.Worksheet worksheet;
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;

                workbook = excel.Workbooks.Add(Type.Missing);
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets["Sheet1"];
                worksheet.Name = "ListMO";

                //header
                worksheet.Cells[1, 1] = "CLOSE_FLAG";
                worksheet.Cells[1, 2] = "MO_NUMBER";
                worksheet.Cells[1, 3] = "DEFAULT_LINE";
                worksheet.Cells[1, 4] = "KEY_PART_NO";
                worksheet.Cells[1, 5] = "TARGET_QTY";

                //export content
                for (int i = 2; i <= count + 1; i++)
                {
                    worksheet.Cells[i, 1] = result[i - 2].CLOSE_FLAG;
                    worksheet.Cells[i, 2] = result[i - 2].MO_NUMBER;
                    worksheet.Cells[i, 3] = result[i - 2].DEFAULT_LINE;
                    worksheet.Cells[i, 4] = result[i - 2].KEY_PART_NO;
                    worksheet.Cells[i, 5] = result[i - 2].TARGET_QTY;
                }
                //FileName 
                switch (MO_Status)
                {
                    case "1":
                        FileName = "_NotIn_MO.xlsx";
                        break;
                    case "2":
                        FileName = "_Input_MO.xlsx";
                        break;
                    case "3":
                        FileName = "_Finish_MO.xlsx";
                        break;
                    case "5":
                        FileName = "_Pending_MO.xlsx";
                        break;
                    case "%":
                        FileName = "_All_MO.xlsx";
                        break;
                }
                //Save
                workbook.SaveAs($"D:\\{FileName}");
                workbook.Close();
                excel.Quit();
                MessageBox.Show($"Export successful - Access Drive D:\\{FileName}", "PM",MessageBoxButton.OK,MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel file already exists or {ex.Message}");
                return;
            }
            finally
            {
                workbook = null;
                worksheet = null;
            }
        }
        private async Task _Modify()
        {
            string sErrMsgBuf;
            DelMOPWDForm frm_DelMOPWDForm = new DelMOPWDForm();
            frm_DelMOPWDForm.ShowDialog();
            if (!string.IsNullOrEmpty(frm_DelMOPWDForm.Edt_DelMOPwd.Password))
            {
                if (frm_DelMOPWDForm.Edt_DelMOPwd.Password != frm_main.empPass)
                {
                    MessageBox.Show("00087 - Nhap sai mat khau -- Password error", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                        + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                        + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                        + " ORDER BY MO_NUMBER";
                var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                        new SfcParameter{ Name="MO_NUMBER",Value=Edt_MO.Text}
                    }
                });
                List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
                if (result[0].CLOSE_FLAG == "2")
                {
                    sErrMsgBuf = "Công lệnh :" + result[0].MO_NUMBER + " đang online, Cần sửa vui lòng pending trước";
                    MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (result[0].CLOSE_FLAG == "3")
                {
                    sErrMsgBuf = "Công lệnh :" + result[0].MO_NUMBER + " đã đóng, The MO already closed";
                    MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LOADFLAG = false;
               MO_InsertForm frm_MO_InsertForm1 = new MO_InsertForm(this,frm_LoadMOForm, sfcClient);
                frm_MO_InsertForm1.Mode = "Modify MO";
                frm_MO_InsertForm1.Title = "MODIFY MO";
                frm_MO_InsertForm1.ShowDialog();
                frm_MO_InsertForm1.Cbb_MAC.Text = result[0].ORDER_NO;
            }
            else
            {
                MessageBox.Show("Nhập mật khẩul -- Please input PASSWORD", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
        private void _KPCompleted()
        {
            KPCompletedForm frm_KPCompletedForm = new KPCompletedForm();
            frm_KPCompletedForm.ShowDialog();
        }
        private void _Refresh()
        {
            RefreshMOForm frm_RefreshMOForm = new RefreshMOForm(this,sfcClient);
            frm_RefreshMOForm.ShowDialog();
        }
        private void _System()
        {
            MO_InputOemForm frm_MO_InputOemForm = new MO_InputOemForm();
            frm_MO_InputOemForm.ShowDialog();
        }
        public async void MO_ManageForm_FormShow()
        {
            string strGetData = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PM' AND VR_ITEM='NBB' AND VR_CLASS='Default' AND VR_NAME='TITLE'";
            var qry_Data = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Data.Data.Count() > 0)
            {
                Lb_Cust_PNo.Content = "Build Phase :";
                Lb_DeviceConfig.Visibility = Visibility.Visible;
                Edt_Device_Config.Visibility = Visibility.Visible;
            }
            Rb_All.IsChecked = true;
            Edt_User.Text += frm_main.empNo + " - " + await getUser();
            Edt_Version.Text += getRunningVersion().ToString();
            Edt_IP.Text += localIP().ToString();
            Edt_DB.Text = frm_main.DB;
            _EMP = frm_main.empNo;
            _EMP_PASS = frm_main.empPass;
        }
        private async void FormShow_CheckPrivilege()
        {
            string strPrivilege = "SELECT * FROM SFIS1.C_PRIVILEGE A, SFIS1.C_EMP_DESC_T B"
                + " WHERE B.EMP_BC = :sUser AND B.QUIT_DATE > SYSDATE AND B.EMP_NO = A.EMP AND A.PRG_NAME = 'PM'";
            var qry_privilege = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strPrivilege,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="sUser",Value=frm_main.empPass}
                }
            });
            List<ListPrivilege> result = qry_privilege.Data.ToListObject<ListPrivilege>().ToList();
            for (int i = 0; i < qry_privilege.Data.Count(); i++)
            {
                if (result[i].FUN == "MO_MODIFY")
                {
                    if (result[i].PRIVILEGE == 2)
                    {
                        ModifyMO.IsEnabled = true;
                    }
                    else
                    {
                        ModifyMO.IsEnabled = false;
                    }
                }
                else if (result[i].FUN == "MO_NEW")
                {
                    if (result[i].PRIVILEGE == 2)
                    {
                        New.IsEnabled = true;
                    }
                    else
                    {
                        New.IsEnabled = false;
                    }
                }
                else if (result[i].FUN == "MO_ON_LINE")
                {
                    if (result[i].PRIVILEGE == 2)
                    {
                        Online.IsEnabled = true;
                    }
                    else
                    {
                        Online.IsEnabled = false;
                    }
                }
                else if (result[i].FUN == "MO_UNON_LINE")
                {
                    if (result[i].PRIVILEGE == 0)
                    {
                        UnOnline.IsEnabled = false;
                    }
                    else
                    {
                        UnOnline.IsEnabled = true;
                    }
                }
                else if (result[i].FUN == "MO_FINISH")
                {
                    if (result[i].PRIVILEGE == 2)
                    {
                        Mofinish.IsEnabled = true;
                    }
                    else
                    {
                        Mofinish.IsEnabled = false;
                    }
                }
                else if (result[i].FUN == "MO_DELETE")
                {

                }
                else if (result[i].FUN == "MO_PENDING")
                {
                    if (result[i].PRIVILEGE == 2)
                    {
                        Pending.IsEnabled = true;
                    }
                    else
                    {
                        Pending.IsEnabled = false;
                    }
                }
                else if (result[i].FUN == "MO_SAVE")
                {
                    if (result[i].PRIVILEGE == 0)
                    {
                        Save.IsEnabled = false;
                    }
                    else
                    {
                        Save.IsEnabled = true;
                    }
                }
                else if (result[i].FUN == "KP_COMPLETED")
                {
                    if (result[i].PRIVILEGE == 2)
                    {
                        KPCompleted.IsEnabled = true;
                    }
                    else
                    {
                        KPCompleted.IsEnabled = false;
                    }
                }
                else if (result[i].FUN == "MO_MODIFY_QTY")
                {
                    if (result[i].PRIVILEGE == 2)
                    {
                        sPrivilige_ModifyMOQty = true;
                    }
                    else
                    {
                        sPrivilige_ModifyMOQty = false;
                    }
                }
                else if (result[i].FUN == "MODIFY_MO_LINK")
                {
                    string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PM' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
                    var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strChkROKU,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_ChkROKU.Data.Count() > 0)
                    {
                        if (result[i].PRIVILEGE == 2)
                        {
                            sPrivilige_ModifyMOLink = true;
                        }
                        else
                        {
                            sPrivilige_ModifyMOLink = false;
                        }
                    }
                    else
                    {
                        sPrivilige_ModifyMOLink = true;
                    }
                }
            }
        }
        private async Task<string> getUser()
        {
            string EMP;
            string sql = $"select * from SFIS1.C_EMP_DESC_T WHERE EMP_BC = '{frm_main.empPass}'";
            var query_model_sql = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            EMP = query_model_sql.Data["emp_name"].ToString();
            return EMP;
        }
        private async Task<string> getFTP()
        {
            string FTP;
            string sql = "SELECT * FROM SFIS1.C_model_desc_T where model_name ='Z_BOX_LH'";
            var query_model_sql = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = sql,
                SfcCommandType = SfcCommandType.Text
            });
            FTP = query_model_sql.Data["model_serial"].ToString();
            return FTP;
        }
        private Version getRunningVersion()
        {
            try
            {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
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
        public string GetHostMacAddress()
        {
            string result = "";
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection instances = managementClass.GetInstances();
            foreach (ManagementObject item in instances)
            {
                if (item["IPEnabled"].ToString() == "True")
                {
                    result = item["MacAddress"].ToString();
                }
            }
            return result;
        }
        private void MO_ManageForm_Closed(object sender, EventArgs e)
        {
            Mofinish.IsEnabled = false;
            System.Windows.Application.Current.Shutdown();
        }
        private void radioButtons_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                if (rb.IsChecked == true)
                {
                    Edt_SearchMO.Focus();
                    switch (rb.Name)
                    {
                        case "Rb_NotIn":
                            MO_Status = "1";
                            GetDataMO();
                            break;
                        case "Rb_Input":
                            MO_Status = "2";
                            GetDataMO();
                            break;
                        case "Rb_Finish":
                            MO_Status = "3";
                            GetDataMO();
                            break;
                        case "Rb_Pending":
                            MO_Status = "5";
                            GetDataMO();
                            break;
                        case "Rb_All":
                            MO_Status = "%";
                            GetDataMO();
                            break;
                    }
                }
            }
        }
        private async void GetDataMO()
        {
            string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag"
                + " and r105.route_code = crn.route_code AND close_flag <> '9' and rownum < 50"
                + " ORDER BY MO_NUMBER";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="CloseFlag",Value=MO_Status}
                }
            });
            if (qry_DataMO.Data.Count() == 0)
            {
                Save.IsEnabled = false;
                _ListMO = null;
            }
            else
            {
                _ListMO = Convert(qry_DataMO.Data.ToListObject<ListMO>());
                await GetRange(_ListMO[0].MO_NUMBER);
                if (frm_main.bCanPrint)
                {
                    Save.IsEnabled = true;
                }
                else
                {
                    Save.IsEnabled = false;
                }
            }
            Lv_DataMO.ItemsSource = _ListMO;
            //Lb_CountMO.Content = qry_DataMO.Data.Count();
            _Grid_ShowData.DataContext = _ListMO;
        }
        private async void SearchMO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =: MO_NUMBER "
                + " and r105.route_code = crn.route_code(+) AND close_flag <> '9'"
                + " ORDER BY MO_NUMBER";
                var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                        new SfcParameter{ Name="MO_NUMBER",Value=Edt_SearchMO.Text}
                    }
                });
                if (qry_DataMO.Data.Count() > 0)
                {
                    _ListMO = Convert(qry_DataMO.Data.ToListObject<ListMO>());
                    string sqlCheckRoute = "select * from sfis1.c_route_name_t where route_code=:var1  ";
                    var qry_Route = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = sqlCheckRoute,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="var1",Value=_ListMO[0].ROUTE_CODE}
                    }
                    });
                    if (qry_Route.Data.Count() == 0)
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "Route not found, check with IE";
                        _er.MessageVietNam = "Không tìm thấy lưu trình, kiểm tra lại với IE";
                        _er.ShowDialog();
                        return;
                    }

                    Lv_DataMO.ItemsSource = _ListMO;
                    _Grid_ShowData.DataContext = _ListMO;
                    if (_ListMO[0].MO_TYPE != "Rework"  && _ListMO[0].MO_TYPE != "RMA")
                        await GetRange(Edt_SearchMO.Text);
                }
                else
                {
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = "00237 - " + await frm_main.GetPubMessage("00237");
                    _er.MessageVietNam = "00237 - " + await frm_main.GetPubMessage("00237");
                    _er.ShowDialog();
                }
                Edt_SearchMO.Focus();
                Edt_SearchMO.SelectAll();
            }
        }
        private async void Lv_DataMO_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Lv_DataMO.SelectedItems == null)
            {
                return;
            }
            else
            {
                var item = Lv_DataMO.SelectedItem as ListMO;
                string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =: MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                + " ORDER BY MO_NUMBER";
                var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="CloseFlag",Value=MO_Status},
                        new SfcParameter{ Name="MO_NUMBER",Value=item.MO_NUMBER}
                    }
                });
                if (qry_DataMO.Data.Count() > 0)
                {
                    _ListMO = Convert(qry_DataMO.Data.ToListObject<ListMO>());
                    _Grid_ShowData.DataContext = _ListMO;
                    await GetRange(item.MO_NUMBER);
                }
                else
                {
                    ShowMessageForm _er = new ShowMessageForm(sfcClient);
                    _er.CustomFlag = true;
                    _er.MessageEnglish = "00237 - " + await frm_main.GetPubMessage("00237");
                    _er.MessageVietNam = "00237 - " + await frm_main.GetPubMessage("00237");
                    _er.ShowDialog();
                }
                Edt_SearchMO.Focus();
                Edt_SearchMO.SelectAll();
            }
        }
        private async Task GetRange(string _param)
        {
            _ListRange = null;
            string strGetDataRange = "SELECT  * FROM SFISM4.R_MO_EXT_T WHERE MO_NUMBER=:MO_NUMBER ORDER BY ITEM_1 DESC";
            var qry_DataRange = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataRange,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="MO_NUMBER",Value=_param}
                }
            });
            if (qry_DataRange.Data.Count() > 0)
            {
                _ListRange = Convert_Range(qry_DataRange.Data.ToListObject<ListRange>());
            }
            Lv_DataRange.ItemsSource = _ListRange;
        }
        private async Task Insert_Pending_Group(string _MO)
        {
            string strDelete = "DELETE SFISM4.R_MO_PENDING_GROUP_T WHERE MO_NUMBER = :MO_NUMBER";
            var Delete_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strDelete,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO_NUMBER",Value=_MO}
                }
            });
            try
            {
                for (int i = 0;i < frm_Setup_Pending_GroupForm.LstSectionName.Items.Count - 1;i++)
                {
                    string strInsert = "INSERT INTO SFISM4.R_MO_PENDING_GROUP_T"
                    + " (MO_NUMBER, SECTION_NAME, GROUP_NAME)"
                    + " VALUES"
                    + " (:MO,:SECTION,:GROUP)";
                    var Insert_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsert,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="MO_NUMBER",Value=_MO},
                            new SfcParameter{Name="SECTION",Value=frm_Setup_Pending_GroupForm.LstSectionName.Items[i]},
                            new SfcParameter{Name="GROUP",Value=frm_Setup_Pending_GroupForm.LstGroupName.Items[i]}
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _er = new ShowMessageForm(sfcClient);
                _er.CustomFlag = true;
                _er.MessageEnglish = ex.Message;
                _er.MessageVietNam = ex.Message;
                _er.ShowDialog();
                return;
            }
        }
        public async Task<bool> GetReuseSNvalue(string _MO)
        {
            string strGetData = $"select MSN_MO_OPTION from sfism4.r105 WHERE MO_NUMBER='{_MO}'";
            var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Data.Data == null)
            {
                return true;
            }
            else
            {
                string sMOOption = qry_Data.Data["msn_mo_option"]?.ToString() ?? "";
                if (sMOOption != "1")
                {
                    return false;
                }
            }
            return false;
        }
        public async Task<string> GetVersion(string _MO)
        {
            string strGetRange = "SELECT ITEM_3,VER_1 FROM SFISM4.R_MO_EXT_T WHERE MO_NUMBER=:MO_NUMBER ORDER BY ITEM_1 DESC";
            var qry_Range = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRange,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO_NUMBER",Value=_MO}
                }
            });
            try
            {
                return qry_Range.Data["ver_1"].ToString();
            }
            catch
            {
                return "";
            }
        }
        private async Task<bool> CheckMORANGE(string _MO)
        {
            string item1, item2, sLastSN, sLastSN_1, sLastSN_2, sLastSN_3;
            string strGetRange = "SELECT ITEM_1,ITEM_2 FROM  SFISM4.R_MO_EXT4_T WHERE MO_NUMBER=:MO_NUMBER AND ROWNUM='1'";
            var qry_Range = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetRange,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO_NUMBER",Value=_MO}
                }
            });
            if (qry_Range.Data == null)
            {
                MessageBox.Show("00140 - Vung gia tri Mo trong EXT4 bang khong ton tai -- The ext4 table wo range is nonexistent", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            item1 = qry_Range.Data["item_1"].ToString();
            item2 = qry_Range.Data["item_2"].ToString();

            string strGetScanData = $"SELECT FIRST_SN,LAST_SN FROM BARCODE.BAR_SCAN_DATA_T WHERE '{item2}' BETWEEN FIRST_SN AND LAST_SN AND RESCAN_FLAG='N'";
            var qry_ScanData = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetScanData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ScanData.Data.Count() == 0)
            {
                MessageBox.Show("MAC khong duoc quet -- MAC dont scan", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string _strGetScanData = $"SELECT FIRST_SN,LAST_SN FROM BARCODE.BAR_SCAN_DATA_T WHERE '{item1}' BETWEEN FIRST_SN AND LAST_SN AND RESCAN_FLAG='N'";
            var _qry_ScanData = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = _strGetScanData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ScanData.Data.Count() == 0)
            {
                MessageBox.Show("MAC khong duoc quet -- MAC dont scan", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            sLastSN = qry_Range.Data["last_sn"].ToString();
            if (sLastSN.Length >= item2.Length)
            {
                return true;
            }
            else
            {
                sLastSN_1 = "";
            }

            string strGetScanLastSN = $"SELECT FIRST_SN,LAST_SN FROM BARCODE.BAR_SCAN_DATA_T WHERE '{sLastSN_1}' BETWEEN FIRST_SN AND LAST_SN AND RESCAN_FLAG='N'";
            var qry_ScanLastSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetScanLastSN,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ScanLastSN.Data.Count() == 0)
            {
                MessageBox.Show("MAC khong duoc quet -- MAC dont scan", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            sLastSN_2 = qry_Range.Data["last_sn"].ToString();
            if (sLastSN_2.Length >= item2.Length)
            {
                return true;
            }
            else
            {
                sLastSN_3 = "";
            }

            string _strGetScanLastSN = $"SELECT FIRST_SN,LAST_SN FROM BARCODE.BAR_SCAN_DATA_T WHERE '{sLastSN_3}' BETWEEN FIRST_SN AND LAST_SN AND RESCAN_FLAG='N'";
            var _qry_ScanLastSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = _strGetScanLastSN,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ScanLastSN.Data.Count() == 0)
            {
                MessageBox.Show("MAC khong duoc quet -- MAC dont scan", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> CheckMOPackRange(string _MO, string _sSTARTMSN,string _sEndMsn,string _sStartImei,string sEndImei)
        {
            string sModelName, sVersionCode;
            int PalletQty, CartonQty, iCurrentNumber;
            string strGetMO = "SELECT MODEL_NAME,VERSION_CODE,Start_MSN,End_MSN,Start_IMEI,End_IMEI"
                + $" FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER='{_MO}'";
            var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                 CommandText = strGetMO,
                 SfcCommandType = SfcCommandType.Text
            });
            sModelName = qry_MO.Data["model_name"].ToString();
            sVersionCode = qry_MO.Data["version_code"].ToString();
            if (!string.IsNullOrEmpty(qry_MO.Data["start_msn"].ToString()))
            {
                MessageBox.Show("Du lieu trung lap Carton/Pallet ID", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string strGetMcaton_no = "SELECT MAX(END_MSN) AS MCARTON_NO,Max(End_Imei) AS Imei_No"
                + " FROM (SELECT END_MSN,End_Imei"
                + " FROM SFISM4.R105 WHERE Model_Name=:ModelName AND"
                + " End_Msn IS NOT NULL AND End_IMEI IS NOT NULL"
                + " UNION ALL"
                + " SELECT END_MSN,End_Imei FROM SFISM4.H105"
                + " WHERE Model_Name=:ModelName AND End_Msn IS NOT NULL AND End_IMEI IS NOT NULL)";
            var qry_Mcaton_no = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetMcaton_no,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="ModelName",Value=sModelName}
                }
            });
            _sSTARTMSN = qry_Mcaton_no.Data["mcarton_no"].ToString();
            _sStartImei = qry_Mcaton_no.Data["imei_no"].ToString();
            if ((_sSTARTMSN == "") || (_sStartImei == ""))
            {
                MessageBox.Show("Khong tim thay du lieu Carton ID or Pallet ID", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string strGetTotal = "Select Trunc(Target_Qty/Carton_Qty) AS intCartonTSQ,"
               + " (Target_Qty/Carton_Qty) AS RealCartonTSQ,"
               + " Trunc(Target_Qty/(Carton_Qty*Pallet_Qty*Tray_Qty)) AS intPalletTSQ,"
               + " (Target_Qty/(Carton_Qty*Pallet_Qty*Tray_Qty)) AS RealPalletTSQ"
               + " From SFIS1.C_Pack_Param_T C,SFISM4.R_Mo_Base_T R"
               + " Where C.Model_Name=R.Model_Name AND C.Version_Code=R.Version_Code AND"
               + " R.Mo_Number=:Mo_Number AND"
               + " (Carton_Qty IS NOT NULL) AND (Pallet_Qty IS NOT NULL ) AND"
               + " (Tray_Qty IS NOT NULL)";
            var qry_Total = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetTotal,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="Mo_Number",Value=_MO}
                }
            });
            if (qry_Total.Data == null)
            {
                MessageBox.Show("00111 - IE chua thiet lap muc config -- The IE did not  set 15 configs", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                if ((float.Parse(qry_Total.Data["realcartontsq"].ToString()) - float.Parse(qry_Total.Data["intcartontsq"].ToString())) > 0)
                {
                    CartonQty = Int32.Parse(qry_Total.Data["intcartontsq"].ToString()) + 1;
                }
                else
                {
                    CartonQty = Int32.Parse(qry_Total.Data["intcartontsq"].ToString());
                }

                if ((float.Parse(qry_Total.Data["realpallettsq"].ToString()) - float.Parse(qry_Total.Data["intpallettsq"].ToString())) > 0)
                {
                    PalletQty = Int32.Parse(qry_Total.Data["intpallettsq"].ToString()) + 1;
                }
                else
                {
                    PalletQty = Int32.Parse(qry_Total.Data["intpallettsq"].ToString());
                }
            }
            iCurrentNumber = Int32.Parse(_sSTARTMSN.Substring(2, _sSTARTMSN.Length - 2));
            _sSTARTMSN = _sSTARTMSN.Substring(0, 2) + string.Format("00000000", iCurrentNumber + 1);
            _sEndMsn = _sSTARTMSN.Substring(0,2) + string.Format("00000000", iCurrentNumber + CartonQty);
            iCurrentNumber = Int32.Parse(_sStartImei.Substring(2, _sStartImei.Length - 2));
            _sStartImei = _sStartImei.Substring(0,2) + string.Format("00000000", iCurrentNumber + 1);
            sEndImei = _sStartImei.Substring(0,2) + string.Format("00000000", iCurrentNumber + PalletQty);
            string strGetData = "Select * From SFISM4.R_MO_BASE_T"
               + " Where Model_Name=:ModelName AND"
               + " (END_MSN Between :StartMSN AND :ENDMSN) OR"
               + " (Start_IMEI Between :StartIMEI AND :ENDIMEI) OR"
               + " (END_MSN Between :StartIMEI AND :ENDIMEI) OR"
               + " (Start_MSN<:StartMSN AND END_MSN>:ENDMSN) OR"
               + " (Start_IMEI<:StartIMEI AND END_IMEI>:ENDIMEI)"
               + " ) AND Start_MSN IS NOT NULL AND ROWNUM=1";
            var qry_Data = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MODELNAME",Value=sModelName},
                    new SfcParameter{Name="STARTMSN",Value=_sSTARTMSN},
                    new SfcParameter{Name="ENDMSN",Value=_sEndMsn},
                    new SfcParameter{Name="STARTIMEI",Value=_sStartImei},
                    new SfcParameter{Name="ENDIMEI",Value=sEndImei}
                }
            });
            if (qry_Data.Data.Count() > 0)
            {
                MessageBox.Show("40007 - Vung gia tri SN trung lap -- SN Range DUP", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private async Task<bool> CheckENCList(string _MO,string _Model)
        {
            string strGetENC = "SELECT Model_name,Sys_no,Desp,to_char(begin_date,'YYYY-MM-DD HH24:MI:SS') TT,rowidtochar(rowid) idd"
                + $" FROM sfis1.c_ecn_desc_t WHERE model_name = '{_Model}'"
                + $" AND begin_date IN (SELECT MAX (begin_date)"
                + $" FROM sfis1.c_ecn_desc_t WHERE model_name = '{_Model}')"
                + " AND begin_date < SYSDATE AND end_date IS NULL AND ROWNUM = 1";
            var qry_ENC = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetENC,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ENC.Data != null)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        private async Task<bool> UpdateCloseFlag(string _MO,string _Version)
        {
            string Rma;
            try
            {
                Rma = frm_main.empNo;
                string strUpdate = "UPDATE SFISM4.R_MO_BASE_T"
                    + " SET CLOSE_FLAG=2,VERSION_CODE =:VERSION_CODE,remark=:rma"
                    + " WHERE MO_NUMBER=:MO_NUMBER";
                var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strUpdate,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="rma",Value=Rma},
                        new SfcParameter{Name="MO_NUMBER",Value=_MO},
                        new SfcParameter{Name="VERSION_CODE",Value=_Version}
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        private async Task<bool> UpdateMOOnline(string _MO)
        {
            try
            {
                string strUpdate = "UPDATE SFISM4.R_MO_BASE_T"
                    + " SET MO_START_DATE=SYSDATE"
                    + " WHERE MO_NUMBER=:MO_NUMBER";
                var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strUpdate,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="MO_NUMBER",Value=_MO}
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        private async Task<bool> UpdateMOPackRange(string _MO,string sStartMsn,string sEndMsn,string sStartImei,string sEndImei)
        {
            try
            {
                string strUpdate = "UPDATE SFISM4.R_MO_BASE_T"
                    + " SET Start_MSN=:Start_MSN,End_MSN=:End_MSN,Start_IMEI=:Start_IMEI,End_IMEI=:End_IMEI"
                    + " WHERE MO_NUMBER=:MO_NUMBER";
                var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strUpdate,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="MO_NUMBER",Value=_MO},
                        new SfcParameter{Name="Start_MSN",Value=sStartMsn},
                        new SfcParameter{Name="End_MSN",Value=sEndMsn},
                        new SfcParameter{Name="Start_IMEI",Value=sStartImei},
                        new SfcParameter{Name="End_IMEI",Value=sEndImei}
                    }
                });
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }
        public async Task<bool> MailInsert(string _Sendwho, string _Mailtext1, string _Mailtext2)
        {
            string MailList;
            string strGetCustomer = "SELECT * FROM  SFIS1.C_CUSTOMER_FTP_ACCOUNT_T WHERE CUSTOMER_CODE=:MCUSTOMER_CODE";
            var qry_Customer = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustomer,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MCUSTOMER_CODE",Value=_Sendwho}
                }
            });
            if (qry_Customer.Data.Count() == 0)
            {
                return false;
            }
            string strGetCustomer_FTP = "SELECT * FROM SFIS1.C_CUSTOMER_FTP_ACCOUNT_T WHERE CUSTOMER_CODE=:MCUSTOMER_CODE";
            var qry_Customer_FTP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustomer_FTP,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MCUSTOMER_CODE",Value=_Sendwho}
                }
            });
            foreach (var row in qry_Customer_FTP.Data)
            {
                MailList = row["email_list"].ToString();
                try
                {
                    //------ Save To Mail ------//
                    string strInsert_Mail = "Insert into SFIS1.C_MAIL_T "
                            + " (MAIL_ID, MAIL_TO, MAIL_FROM,  MAIL_SUBJECT, MAIL_SEQUENCE,"
                            + " MAIL_CONTENT, MAIL_FLAG, MAIL_PROGRAM)"
                            + " VALUES ("
                            + " :MAIL_ID,:MAIL_TO,:MAIL_FROM,:MAIL_SUBJECT,:MAIL_SEQUENCE,:MAIL_CONTENT,:MAIL_FLAG,:MAIL_PROGRAM)";
                    var Insert_Mail = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsert_Mail,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="MAIL_ID",Value=System.DateTime.Now.ToString("yyyymmddhhnnsszzz")},
                            new SfcParameter{Name="MAIL_TO",Value=MailList},
                            new SfcParameter{Name="MAIL_FROM",Value="SFIS_CONFIG"},
                            new SfcParameter{Name="MAIL_SUBJECT",Value=_Mailtext1},
                            new SfcParameter{Name="MAIL_SEQUENCE",Value="0"},
                            new SfcParameter{Name="MAIL_CONTENT",Value=_Mailtext2},
                            new SfcParameter{Name="MAIL_FLAG",Value="0"},
                            new SfcParameter{Name="MAIL_PROGRAM",Value="PM"}
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return true;
        }
    }
}
