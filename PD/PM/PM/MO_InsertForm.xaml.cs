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
using PM.Model;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;

namespace PM
{
    /// <summary>
    /// Interaction logic for MO_InsertForm.xaml
    /// </summary>
    public partial class MO_InsertForm : Window
    {
        public SfcHttpClient sfcClient;
        public MO_ManageForm frm_MO_ManageForm;
        public LoadMOForm frm_LoadMOForm;
        public string OLD_MO, OLD_SN, OLD_END_SN;
        public string Mode, END_SN, MO, ship_addr, ship_index, ST;
        public int c_count, iSNLen, iMOLen, iCurrentNOLen, iCurrentNOStartDigit;
        public bool isNewMo, isUpdateMO, bSNFixFlag, bMOFixFlag, isS, isP;
        public string sCurrentNOType, sAllStrTmp, sErrMsgBuf;
        private string sTime = System.DateTime.Now.ToString("yyyy/MM/dd ");
        bool[] FieldFlag = new bool[10];
        string[] sMultiStr;

        //Define Edt
        TextBlock Edt_Option = new TextBlock();
        TextBlock Edt_FormAddPO = new TextBlock();
        TextBlock Edt_HWBOM = new TextBlock();
        TextBlock Edt_SWBOM = new TextBlock();

        public MO_InsertForm()
        {
            InitializeComponent();
        }
        public MO_InsertForm(MO_ManageForm _frm_MO_ManageForm, LoadMOForm _frm_LoadMOForm, SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            frm_MO_ManageForm = _frm_MO_ManageForm;
            frm_LoadMOForm = _frm_LoadMOForm;
            InitializeComponent();
            MO_InsertForm_FormShow();
        }
        private async void MO_InsertForm_FormShow()
        {
            int K, H;
            string MO_Type;
            bool The_Type;
            if (frm_MO_ManageForm.LOADFLAG == true)
            {
                Rb_Secondary.IsChecked = true;
                MOClass.IsEnabled = false;
                Edt_MO.IsEnabled = true;
                if (frm_MO_ManageForm.sPrivilige_ModifyMOQty)
                {
                    Edt_TargetQTY.IsEnabled = true;
                }
                else
                {
                    Edt_TargetQTY.IsEnabled = false;
                }
                if (frm_MO_ManageForm.sPrivilige_ModifyMOLink)
                {
                    Edt_LinkMO.IsEnabled = true;
                }
                else
                {
                    Edt_LinkMO.IsEnabled = false;
                }
                Edt_SN.IsEnabled = true;
                Cbb_Model.IsEnabled = true;
                Cbb_Line.IsEnabled = true;
                Time_PlanInput.IsEnabled = true;
                Title = "Add MO";
                Mode = "ADD";
                Edt_MO.Text = "";
                Edt_FormAddPO.Text = "";
                Edt_HWBOM.Text = "";
                Edt_HWBOM.Text = "";
                Edt_Option.Text = "";
                Edt_SN.Text = "";
                Edt_UPC.Text = "";
                Edt_Version.Text = "";
                string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =: MO_NUMBER and ROUTE_NAME not like '%HOLD%'"
                + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                + " ORDER BY MO_NUMBER";
                var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="CloseFlag",Value=frm_MO_ManageForm.MO_Status},
                        new SfcParameter{ Name="MO_NUMBER",Value=frm_MO_ManageForm.Edt_MO.Text}
                    }
                });
                OLD_MO = qry_DataMO.Data["mo_number"]?.ToString() ?? "";
                OLD_SN = qry_DataMO.Data["start_sn"]?.ToString() ?? "";
                OLD_END_SN = qry_DataMO.Data["end_sn"]?.ToString() ?? "";
            }
            else
            {
                if (frm_MO_ManageForm.sPrivilige_ModifyMOQty)
                {
                    Edt_TargetQTY.IsEnabled = true;
                }
                else
                {
                    Edt_TargetQTY.IsEnabled = false;
                }
                if (frm_MO_ManageForm.sPrivilige_ModifyMOLink)
                {
                    Edt_LinkMO.IsEnabled = true;
                }
                else
                {
                    Edt_LinkMO.IsEnabled = false;
                }
            }
            Grid_TA.Visibility = Visibility.Hidden;
            Lb_TA.Visibility = Visibility.Hidden;
            Cbb_SN_USE.Visibility = Visibility.Hidden;
            Cbb_SN_USE.IsEnabled = false;
            Cbb_MO.IsEnabled = true;
            Edt_MO.IsEnabled = false;
            Edt_SN.IsEnabled = Lb_StartSN.IsEnabled;
            Edt_PackLot.IsEnabled = true;
            Edt_PO.Text = "N/A";
            Edt_POITEM.Text = "N/A";

            Cbb_Model.IsEnabled = false;
            Cbb_Line.IsEnabled = true;
            Cbb_Route.IsEnabled = true;
            Cbb_FirstGroup.IsEnabled = false;
            Cbb_LastGroup.IsEnabled = false;
            Time_PlanInput.IsEnabled = true;
            Time_PlanFinish.IsEnabled = true;
            Cbb_PartNO.IsEnabled = false;
            Edt_Version.IsEnabled = true;
            Cbb_MoType.IsEnabled = true;
            Cbb_MOKind.IsEnabled = true;
            MOClass.IsEnabled = true;

            Lb_ScrapQty.Visibility = Visibility.Visible;
            Cbb_ScrapQty.Visibility = Visibility.Visible;
            Cbb_ScrapQty.IsEnabled = true;
            string strGetData_NBB = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PM' AND VR_ITEM='NBB' AND VR_CLASS='Default' AND VR_NAME='TITLE'";
            var qry_NBB = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetData_NBB,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_NBB.Data.Count() > 0)
            {
                Lb_CustPNo.Content = "Build Phase";
                Lb_CustPN.Content = "Device Config";
                Edt_CustPN.Text = "";
            }
            string strGetIni = "SELECT VR_CLASS FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = :sPRG GROUP BY VR_CLASS ORDER BY VR_CLASS";
            var qry_Ini = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetIni,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="sPRG",Value="PM"}
                }
            });
            Cbb_MO.Items.Clear();
            if (qry_Ini.Data.Count() == 0)
            {
                Cbb_MO.Items.Add("Default");
            }
            else
            {
                Cbb_MO.Items.Add("LOADFROMBPCS");
                foreach (var row in qry_Ini.Data)
                {
                    Cbb_MO.Items.Add(row["vr_class"]);
                }
            }
            if (frm_MO_ManageForm.LOADFLAG == true)
            {
                Cbb_MO.Text = "LOADFROMBPCS";
            }
            Cbb_MO.Text = Cbb_MO.Items[0].ToString();

            Edt_SN.Text = "";
            Edt_Version.Text = "";
            Edt_TargetQTY.Text = "10";
            Time_PlanInput.Text = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Time_PlanFinish.Text = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Edt_PackLot.Text = "";

            Edt_ReMark.Text = "";
            Edt_CustPN.Text = "";
            Edt_LinkMO.Text = "";
            Edt_SW_BOM.Text = "";

            try
            {
                Cbb_MOKind.Items.Clear();
                Cbb_MOKind.Items.Add("SFIS");
                Cbb_MOKind.Items.Add("LOT");
                Cbb_MOKind.Items.Add("HYBRID");

                Cbb_MAC.Items.Clear();
                Cbb_MAC.Items.Add("NONE");
                Cbb_MAC.Items.Add("NEW");
                Cbb_MAC.Items.Add("OLD");

                Cbb_ScrapQty.Items.Clear();
                Cbb_ScrapQty.Items.Add("1");
                Cbb_ScrapQty.Items.Add("2");
                Cbb_ScrapQty.Items.Add("3");
                Cbb_ScrapQty.Items.Add("4");
                Cbb_ScrapQty.Items.Add("5");
                Cbb_ScrapQty.Items.Add("6");
                Cbb_ScrapQty.Items.Add("7");
                Cbb_ScrapQty.Items.Add("8");
                Cbb_ScrapQty.Items.Add("9");
                Cbb_ScrapQty.Items.Add("10");
            }
            catch
            {

            }
            string strGetType = "select OQA_TYPE from SFIS1.C_OQA_SAMPLING_PLAN group by OQA_TYPE order by OQA_TYPE";
            var qry_Type = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetType,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_MoType.Items.Clear();
            if (qry_Type.Data.Count() > 0)
            {
                foreach (var row in qry_Type.Data)
                {
                    Cbb_MO.Items.Add(row["oqa_type"]);
                }
            }
            Cbb_MoType.Text = "";
            string strGetLine = "select LINE_NAME from SFIS1.C_LINE_DESC_T ORDER BY LINE_NAME";
            var qry_Line = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetLine,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_Line.Items.Clear();
            if (qry_Line.Data.Count() > 0)
            {
                foreach (var row in qry_Line.Data)
                {
                    Cbb_Line.Items.Add(row["line_name"]);
                }
                Cbb_Line.Text = Cbb_Line.Items[0].ToString();
            }

            string strGetModel = "SELECT a.MODEL_NAME,a.default_group,a.end_group,a.ROUTE_CODE ,a.STANDARD"
                + " FROM SFIS1.C_MODEL_DESC_T a ORDER BY MODEL_NAME";
            var qry_Model = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModel,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_Model.Items.Clear();
            if (qry_Model.Data.Count() > 0)
            {
                foreach (var row in qry_Model.Data)
                {
                    Cbb_Model.Items.Add(row["model_name"]);
                }
                //Cbb_Model.Text = Cbb_Model.Items[0].ToString();
            }

            string strGetRoute = "SELECT ROUTE_CODE,ROUTE_NAME FROM SFIS1.C_ROUTE_NAME_T ORDER BY ROUTE_NAME";
            var qry_Route = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRoute,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_Route.Items.Clear();
            if (qry_Route.Data.Count() > 0)
            {
                foreach (var row in qry_Route.Data)
                {
                    Cbb_Route.Items.Add(row["route_name"]);
                }
                Cbb_Route.Text = Cbb_Route.Items[0].ToString();
            }
            else
            {
                Cbb_Route.Text = "";
            }

            string strGetBOM = "SELECT BOM_NO FROM SFIS1.C_BOM_KEYPART_T GROUP BY BOM_NO order by BOM_NO";
            var qry_BOM = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetBOM,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_BOM.Items.Clear();
            if (qry_BOM.Data.Count() > 0)
            {
                foreach (var row in qry_BOM.Data)
                {
                    Cbb_BOM.Items.Add(row["bom_no"]);
                }
                Cbb_BOM.Items.Add("");
                Cbb_BOM.Text = Cbb_BOM.Items[0].ToString();
            }

            string strGetCustomer = "select CUSTOMER from SFIS1.C_CUSTOMER_T order by CUSTOMER";
            var qry_Customer = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustomer,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_Customer.Items.Clear();
            if (qry_Customer.Data.Count() > 0)
            {
                foreach (var row in qry_Customer.Data)
                {
                    if (row["customer"].ToString() != "ALL")
                    {
                        Cbb_Customer.Items.Add(row["customer"]);
                    }
                }
                Cbb_Customer.Text = Cbb_Customer.Items[0].ToString();
                Cbb_Customer.IsEnabled = false;
            }

            string strGetPartNO = "select KEY_PART_NO from SFIS1.C_KEYPARTS_DESC_T group by KEY_PART_NO order by KEY_PART_NO";
            var qry_PartNO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetPartNO,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_PartNO.Items.Clear();
            if (qry_PartNO.Data.Count() > 0)
            {
                foreach (var row in qry_PartNO.Data)
                {
                    Cbb_PartNO.Items.Add(row["key_part_no"]);
                }
                Cbb_PartNO.Text = "";
            }

            //string strGetGroupName = "SELECT DISTINCT GROUP_NAME FROM SFIS1.C_GROUP_CONFIG_T ORDER BY GROUP_NAME";
            //var qry_GroupName = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            //{
            //    CommandText = strGetGroupName,
            //    SfcCommandType = SfcCommandType.Text
            //});
            //Cbb_FirstGroup.Items.Clear();
            //Cbb_LastGroup.Items.Clear();
            //if (qry_GroupName.Data != null)
            //{
            //    foreach (var row in qry_GroupName.Data)
            //    {
            //        Cbb_FirstGroup.Items.Add(row["group_name"]);
            //        Cbb_LastGroup.Items.Add(row["group_name"]);
            //    }
            //}
            //Cbb_FirstGroup.Text = Cbb_FirstGroup.Items[0].ToString();

            if (Mode != "ADD")
            {
                string _strGetRoute = "";
                string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                    + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%' "
                    + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                    + " ORDER BY MO_NUMBER";
                var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="CloseFlag",Value=frm_MO_ManageForm.MO_Status},
                        new SfcParameter{ Name="MO_NUMBER",Value=frm_MO_ManageForm.Edt_MO.Text}
                    }
                });
                Cbb_Model.Text = qry_DataMO.Data["model_name"]?.ToString() ?? "";
                Edt_MO.Text = qry_DataMO.Data["mo_number"]?.ToString() ?? "";

                string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PM' AND VR_CLASS='AIR' AND VR_VALUE='Y'";
                var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strChkROKU,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ChkROKU.Data.Count() == 0)
                {
                    Cbb_Route.Items.Clear();
                    //if ((Edt_MO.Text.Substring(0, 2) == "00") && (await CheckCpeiii() == false))
                    //{
                    //    _strGetRoute = "select * from sfis1.c_route_name_t where route_name like '%SMT%' or route_name like 'KIT'";
                    //}
                    //else
                    //{
                    //    _strGetRoute = $"select * from sfis1.c_route_name_t where route_name like ('{Cbb_Model.Text}%')";
                    //}
                    _strGetRoute = $"SELECT route_code,route_name  FROM table(SFIS1.Z_PKG.get_route('{Edt_MO.Text}')) order by route_index";
                    var _qry_Route = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = _strGetRoute,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (_qry_Route.Data.Count() > 0)
                    {
                        foreach (var row in _qry_Route.Data)
                        {
                            Cbb_Route.Items.Add(row["route_name"]);
                        }

                        Cbb_Route.Text = Cbb_Route.Items[0].ToString();
                    }
                }
                else
                {
                    Cbb_Route.Items.Clear();
                    //string strGetRouteRoku = $"select * from sfis1.c_route_name_t where route_name like ('{Cbb_Model.Text}%') AND ROUTE_NAME NOT LIKE '%HOLD%'";
                    string strGetRouteRoku = $"SELECT route_code,route_name  FROM table(SFIS1.Z_PKG.get_route('{Edt_MO.Text}')) order by route_index";
                    var qry_RouteRoku = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetRouteRoku,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_RouteRoku.Data.Count() > 0)
                    {
                        foreach (var row in qry_RouteRoku.Data)
                        {
                            Cbb_Route.Items.Add(row["route_name"]);
                        }

                        Cbb_Route.Text = Cbb_Route.Items[0].ToString();
                    }
                }

                string _strGetBOM = $"SELECT * FROM table(SFIS1.Z_PKG.get_bomno('{Edt_MO.Text.Trim()}')) order by BOM_INDEX ";
                var _qry_BOM = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = _strGetBOM,
                    SfcCommandType = SfcCommandType.Text
                });
                Cbb_BOM.Items.Clear();
                if (_qry_BOM.Data.Count() > 0)
                {
                    foreach (var row in _qry_BOM.Data)
                    {
                        Cbb_BOM.Items.Add(row["bom_no"]);
                    }
                    Cbb_BOM.Items.Add("");
                    Cbb_BOM.Text = Cbb_BOM.Items[0].ToString();
                }
                else
                {
                    Cbb_BOM.Text = "";
                }

                string strGetRoute_Name = "SELECT * FROM SFIS1.C_ROUTE_NAME_T where ROUTE_CODE = :RouteCode";
                var qry_Route_Name = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetRoute_Name,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="RouteCode",Value= qry_DataMO.Data["route_code"].ToString()}
                    }
                });
                if (Cbb_Route.Items.Count < 0)
                {
                    Cbb_Route.Text = Cbb_Route.Items[0].ToString();
                }
                if (qry_Route_Name.Data["route_code"].ToString().IndexOf("HOLD") > -1)
                {
                    Cbb_Route.IsEnabled = false;
                }
                else
                {
                    Cbb_Route.IsEnabled = true;
                }

                MO = qry_DataMO.Data["mo_number"]?.ToString() ?? "";
                Edt_MO.Text = qry_DataMO.Data["mo_number"]?.ToString() ?? "";
                Edt_TargetQTY.Text = qry_DataMO.Data["target_qty"]?.ToString() ?? "";
                Cbb_Model.Text = qry_DataMO.Data["model_name"]?.ToString() ?? "";
                Cbb_Line.Text = qry_DataMO.Data["default_line"]?.ToString() ?? "";
                Cbb_FirstGroup.Text = qry_DataMO.Data["default_group"]?.ToString() ?? "";
                Cbb_LastGroup.Text = qry_DataMO.Data["end_group"]?.ToString() ?? "";
                Cbb_BOM.Text = qry_DataMO.Data["bom_no"]?.ToString() ?? "";
                Cbb_PartNO.Text = qry_DataMO.Data["key_part_no"]?.ToString() ?? "";
                Cbb_MoType.Items.Add(qry_DataMO.Data["mo_type"]?.ToString() ?? "");
                Cbb_MoType.Text = qry_DataMO.Data["mo_type"]?.ToString() ?? "";
                Edt_Version.Text = qry_DataMO.Data["version_code"]?.ToString() ?? "";
                Cbb_MOKind.Text = qry_DataMO.Data["lot_flag"]?.ToString() ?? "";
                Cbb_MAC.Text = qry_DataMO.Data["order_no"]?.ToString() ?? "";
                Cbb_ScrapQty.Text = qry_DataMO.Data["sign_emp"]?.ToString() ?? "";
                Edt_LinkMO.Text = qry_DataMO.Data["job_mo_option"]?.ToString() ?? "";
                Edt_TA.Text = qry_DataMO.Data["vender_part_no"]?.ToString() ?? "";
                Edt_CustPN.Text = qry_DataMO.Data["cust_part_no"]?.ToString() ?? "";
                Edt_SW_BOM.Text = qry_DataMO.Data["sw_bom"]?.ToString() ?? "";
                MOClass.IsEnabled = true;

                if (Edt_MO.Text.Substring(0, 2) != "00")
                {
                    if (await CheckWhs(MO))
                    {
                        Cbb_SN_USE.Visibility = Visibility.Visible;
                        if (await GetReuseSNvalue(MO))
                        {
                            Cbb_SN_USE.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Cbb_SN_USE.Visibility = Visibility.Hidden;
                        }
                    }
                }

                K = 0; H = 0;
                string strGetData = $"SELECT * from sfism4.r105 where mo_number='{Edt_MO.Text}'";
                var qry_Data = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text
                });
                List<ListMO> result_mo = qry_Data.Data.ToListObject<ListMO>().ToList();
                c_count = qry_Data.Data.Count();

                string strPrivilege = "SELECT * FROM SFIS1.C_PRIVILEGE A, SFIS1.C_EMP_DESC_T B WHERE B.EMP_BC = :sUser"
                    + " AND B.QUIT_DATE > SYSDATE AND B.EMP_NO = A.EMP AND A.PRG_NAME = 'PM'";
                var qry_Privilege = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strPrivilege,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="sUser",Value=frm_MO_ManageForm._EMP_PASS}
                    }
                });
                List<ListPrivilege> result = qry_Privilege.Data.ToListObject<ListPrivilege>().ToList();
                if ((c_count != 0) && (result_mo[0].CLOSE_FLAG == "5"))
                {
                    for (int j = 0; j < qry_Privilege.Data.Count(); j++)
                    {
                        if ((result[j].FUN == "MODIFY_MO_VER") && (result[j].PRIVILEGE == 2))
                        {
                            K = K + 1;
                        }
                        if ((result[j].FUN == "MODIFY_MO_QTY") && (result[j].PRIVILEGE == 2))
                        {
                            H = H + 1;
                        }
                    }
                }

                if (K > 0)
                {
                    Edt_Version.IsEnabled = true;
                }
                else
                {
                    Edt_Version.IsEnabled = false;
                }
                if (H > 0)
                {
                    Edt_TargetQTY.IsEnabled = true;
                }
                else
                {
                    Edt_TargetQTY.IsEnabled = false;
                }

                if (qry_DataMO.Data["master_flag"].ToString() == "N")
                {
                    Rb_Secondary.IsChecked = true;
                }
                else
                {
                    Rb_Primary.IsChecked = true;
                }

                MOClass.IsEnabled = false;
                string strGetCustName = "select CUSTOMER, CUST_CODE from SFIS1.C_CUSTOMER_T where CUST_CODE =:sCC";
                var qry_CustName = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCustName,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="sCC",Value=qry_DataMO.Data["cust_code"].ToString()}
                    }
                });
                Cbb_Customer.Text = qry_CustName.Data["customer"]?.ToString() ?? "";

                Time_PlanInput.Text = qry_DataMO.Data["mo_schedule_date"]?.ToString() ?? "";
                Time_PlanFinish.Text = qry_DataMO.Data["mo_due_date"]?.ToString() ?? "";
                Edt_SN.Text = qry_DataMO.Data["start_sn"]?.ToString() ?? "";
                Edt_FormAddPO.Text = qry_DataMO.Data["po_no"]?.ToString() ?? "";
                Edt_UPC.Text = qry_DataMO.Data["upc_co"]?.ToString() ?? "";
                Edt_Option.Text = qry_DataMO.Data["option_desc"]?.ToString() ?? "";
                Edt_HWBOM.Text = qry_DataMO.Data["hw_bom"]?.ToString() ?? "";
                Edt_SWBOM.Text = qry_DataMO.Data["sw_bom"]?.ToString() ?? "";

                string strGetData_MO = "SELECT * FROM sfism4.r_mo_base_t WHERE MO_NUMBER= :mo_number and close_flag <> '9' ORDER BY mo_number";
                var qry_Data_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData_MO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="mo_number",Value=MO}
                    }
                });
                Edt_TA.Text = qry_Data_MO.Data["vender_part_no"]?.ToString() ?? "";
                Edt_CustPN.Text = qry_Data_MO.Data["custpn"]?.ToString() ?? "";
                Edt_CustPNo.Text = qry_Data_MO.Data["cust_part_no"]?.ToString() ?? "";
                Edt_ReMark.Text = qry_Data_MO.Data["remark"]?.ToString() ?? "";
                Edt_SO.Text = qry_Data_MO.Data["so_number"]?.ToString() ?? "";
                Edt_SoLine.Text = qry_Data_MO.Data["so_line"]?.ToString() ?? "";
                Edt_PackLot.Text = qry_Data_MO.Data["pmcc"]?.ToString() ?? "";
                Edt_PO.Text = qry_Data_MO.Data["po_no"]?.ToString() ?? "";
                Edt_POITEM.Text = qry_Data_MO.Data["po_item"]?.ToString() ?? "";
                Cbb_MO.Text = qry_Data_MO.Data["mo_option"]?.ToString() ?? "";

                string strGetData_PO = "select PO_NUMBER,CUSTOMER,QTY from sfism4.R_PO_T where mo_number=:MO_NUMBER";
                var qry_DataPO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData_PO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text}
                    }
                });
                Cbb_MO.IsEnabled = true;
                if (Lb_CloseFlag.Content.ToString() == "2")
                {
                    Edt_MO.IsEnabled = false;
                    Edt_SN.IsEnabled = false;
                    Cbb_Model.IsEnabled = false;
                    Cbb_Line.IsEnabled = false;
                    Cbb_FirstGroup.IsEnabled = false;
                    Cbb_LastGroup.IsEnabled = false;
                    Time_PlanInput.IsEnabled = false;
                    Cbb_PartNO.IsEnabled = false;
                    Cbb_MoType.IsEnabled = false;
                    Cbb_MOKind.IsEnabled = false;
                    Cbb_Customer.IsEnabled = false;
                    Cbb_BOM.IsEnabled = false;
                    Edt_PackLot.IsEnabled = false;
                }
                else
                {
                    Edt_MO.IsEnabled = false;
                    Cbb_Model.IsEnabled = false;
                    Time_PlanInput.IsEnabled = false;
                    Time_PlanFinish.IsEnabled = false;
                    Edt_SN.IsEnabled = true;
                    Cbb_Line.IsEnabled = true;
                    Cbb_PartNO.IsEnabled = false;
                    Cbb_MoType.IsEnabled = true;
                    Cbb_MOKind.IsEnabled = true;
                    Cbb_BOM.IsEnabled = true;
                    Edt_PackLot.IsEnabled = true;
                }
                OLD_MO = qry_Data_MO.Data["mo_number"]?.ToString() ?? "";
                OLD_SN = qry_Data_MO.Data["start_sn"]?.ToString() ?? "";
                OLD_END_SN = qry_Data_MO.Data["end_sn"]?.ToString() ?? "";
                string strGetParam = "select * from sfis1.c_sql_param_t where sql_name='MO_ROUTE' and param_name =:mo";
                var qry_Param = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetParam,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="mo",Value=Edt_MO.Text}
                    }
                });
                if (qry_Param.Data.Count() > 0)
                {
                    Cbb_Route.Items.Clear();
                    foreach (var row in qry_Param.Data)
                    {
                        Cbb_Route.Items.Add(row["param_value"]);
                    }
                }
            }
            if (frm_MO_ManageForm.LOADFLAG == true)
            {
                string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                        + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER and ROUTE_NAME not like '%HOLD%' "
                        + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                        + " ORDER BY MO_NUMBER";
                var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{ Name="CloseFlag",Value=frm_MO_ManageForm.MO_Status},
                        new SfcParameter{ Name="MO_NUMBER",Value=frm_MO_ManageForm.Edt_MO.Text}
                    }
                });
                string strGetMO_Plan = "";
                if (!string.IsNullOrEmpty(frm_LoadMOForm.Edt_SearchMO.Text))
                {
                    strGetMO_Plan = "SELECT B.*,C.* from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C, SFIS1.C_CUSTOMER_T E"
                       + " WHERE B.MODEL_NAME=C.MODEL_NAME AND b.cust_code=e.cust_code(+)"
                       + " AND (B.MO_NUMBER NOT IN("
                       + $" SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A) AND MO_NUMBER = '{frm_LoadMOForm.Edt_SearchMO.Text}')";
                }
                else
                {
                    strGetMO_Plan = "SELECT B.*,C.* from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C, SFIS1.C_CUSTOMER_T E"
                        + " WHERE B.MODEL_NAME=C.MODEL_NAME AND b.cust_code=e.cust_code(+)"
                        + " AND (B.MO_NUMBER NOT IN("
                        + " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A))";
                }
                var qry_MO_Plan = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetMO_Plan,
                    SfcCommandType = SfcCommandType.Text
                });
                dynamic ads = qry_MO_Plan.Data;
                Edt_MO.Text = ads[0]["mo_number"];
                Edt_Version.Text = await frm_MO_ManageForm.GetVersion(Edt_MO.Text);
                Cbb_Model.Text = ads[0]["model_name"];
                Edt_TargetQTY.Text = Convert.ToString(ads[0]["target_qty"]);
                Cbb_MoType.Items.Add(ads[0]["mo_type"]);
                Cbb_MoType.Text = ads[0]["mo_type"];
                //await GetRoute_Group(Cbb_Model.Text);
                Time_PlanInput.Text = ads[0]["mo_schedule_date"];
                Time_PlanFinish.Text = ads[0]["mo_due_date"];
                Cbb_PartNO.Items.Add(ads[0]["key_part_no"]);
                Cbb_PartNO.Text = ads[0]["key_part_no"];
                Edt_CustPN.Text = ads[0]["custpn"];
                //Cbb_Route.Text = ads[0]["route_name"];
                Cbb_FirstGroup.Text = ads[0]["default_group"];
                Cbb_LastGroup.Text = ads[0]["end_group"];
                Cbb_BOM.Text = ads[0]["bom_no"];
                //Cbb_Customer.Text = ads[0]["customer"];
                Edt_SO.Text = ads[0]["so_number"];
                Edt_SoLine.Text = ads[0]["so_line"];
                Edt_PackLot.Text = qry_DataMO.Data["pmcc"]?.ToString() ?? "";
                Edt_TA.Text = qry_DataMO.Data["vender_part_no"]?.ToString() ?? "";
                if (Cbb_Customer.Text == null)
                {
                    Cbb_Customer.Text = Cbb_Customer.Items[0].ToString();
                }

                if ((ads[0]["whs"] == "BC3F") || (ads[0]["whs"] == ""))
                {
                    if (Edt_MO.Text.Substring(0, 2) != "00")
                    {
                        if (await CheckScrap())
                        {
                            Cbb_SN_USE.Visibility = Visibility.Visible;
                        }
                        else

                        {
                            Cbb_SN_USE.Visibility = Visibility.Hidden;
                        }
                    }
                }
                if ((ads[0]["whs"] == "BC4F") || (Cbb_MoType.Text == "Rework"))
                {
                    if (Edt_MO.Text.Substring(0, 2) != "00")
                    {
                        if (await CheckScrap())
                        {
                            if (await CheckMoModel(Cbb_Model.Text) == true)
                            {
                                Cbb_SN_USE.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }

                Cbb_BOM.Items.Clear();

                string strGetData_BOM = "";
                //if (Cbb_MoType.Text.Trim() == "Rework")
                //{
                //    strGetData_BOM = $"select distinct bom_no from sfis1.c_bom_keypart_t where bom_no Like '{Cbb_Model.Text.Trim()}-RW%' ORDER BY BOM_NO desc";
                //    var qry_BomNo = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                //    {
                //        CommandText = strGetData_BOM,
                //        SfcCommandType = SfcCommandType.Text
                //    });
                //    if (qry_BomNo.Data.Count() > 0)
                //    {
                //        foreach (var row in qry_BomNo.Data)
                //        {
                //            Cbb_BOM.Items.Add(row["bom_no"]);
                //        }
                //        Cbb_BOM.Items.Add("");
                //        Cbb_BOM.Text = Cbb_BOM.Items[0].ToString();
                //    }
                //    else
                //    {
                //        strGetData_BOM = $"select distinct bom_no from sfis1.c_bom_keypart_t where bom_no like '{Cbb_Model.Text.Trim()}RW%' ORDER BY BOM_NO desc";
                //        var _qry_BomNo = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                //        {
                //            CommandText = strGetData_BOM,
                //            SfcCommandType = SfcCommandType.Text
                //        });
                //        if (_qry_BomNo.Data.Count() > 0)
                //        {
                //            foreach (var row in _qry_BomNo.Data)
                //            {
                //                Cbb_BOM.Items.Add(row["bom_no"]);
                //            }
                //            Cbb_BOM.Items.Add("");
                //            Cbb_BOM.Text = Cbb_BOM.Items[0].ToString();
                //        }
                //        else
                //        {
                //            strGetData_BOM = $"select distinct bom_no from sfis1.c_bom_keypart_t where bom_no like '{Cbb_Model.Text.Trim()}%' ORDER BY BOM_NO desc";
                //            var qry_BomNo_ = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                //            {
                //                CommandText = strGetData_BOM,
                //                SfcCommandType = SfcCommandType.Text
                //            });
                //            if (qry_BomNo_.Data.Count() > 0)
                //            {
                //                foreach (var row in qry_BomNo_.Data)
                //                {
                //                    Cbb_BOM.Items.Add(row["bom_no"]);
                //                }
                //                Cbb_BOM.Items.Add("");
                //                Cbb_BOM.Text = Cbb_BOM.Items[0].ToString();
                //            }
                //            else
                //            {
                //                Cbb_BOM.Text = "";
                //            }
                //        }
                //    }
                //}
                //else
                //{
                strGetData_BOM = $"SELECT * FROM table(SFIS1.Z_PKG.get_bomno('{Edt_MO.Text.Trim()}')) order by BOM_INDEX ASC   ";
                var qry_BomNo = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetData_BOM,
                    SfcCommandType = SfcCommandType.Text,
                });
                if (qry_BomNo.Data.Count() > 0)
                {
                    foreach (var row in qry_BomNo.Data)
                    {
                        Cbb_BOM.Items.Add(row["bom_no"]);
                    }
                    Cbb_BOM.Items.Add("");
                    Cbb_BOM.Text = Cbb_BOM.Items[0].ToString();
                }
                else
                {
                    Cbb_BOM.Text = "";
                }


                string strGetMOType = $"select * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='{Cbb_Model.Text}'";
                var qry_MOType = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetMOType,
                    SfcCommandType = SfcCommandType.Text
                });
                MO_Type = qry_MOType.Data["model_type"]?.ToString() ?? "";

                Cbb_Route.Items.Clear();
                string strGetData_Param = "select * from sfis1.c_sql_param_t where sql_name='MO_ROUTE' and param_name=:mo";
                var qry_Param = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetData_Param,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="mo",Value=Edt_MO.Text}
                    }
                });
                if (qry_Param.Data != null)
                {
                    Cbb_Route.Items.Add(qry_Param.Data["param_value"].ToString());
                }
                else
                {
                    string strGetDataRoute = "";
                    string strGetData_RouteName = "";

                    strGetData_RouteName = $"SELECT route_code,route_name  FROM table(SFIS1.Z_PKG.get_route('{Edt_MO.Text}')) order by route_index";
                    var qry_RouteName = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetData_RouteName,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_RouteName.Data != null)
                    {
                        foreach (var row in qry_RouteName.Data)
                        {
                            Cbb_Route.Items.Add(row["route_name"]);
                        }
                    }
                    /*
                    strGetData_RouteName = $"select route_name from sfis1.c_route_name_t where route_name= '{Cbb_Model.Text}_{Cbb_MoType.Text}'";
                    var qry_RouteName = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetData_RouteName,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_RouteName.Data != null)
                    {
                        Cbb_Route.Text = qry_RouteName.Data["route_name"].ToString();
                        Cbb_Route.Items.Clear();
                        if ((Edt_MO.Text.Trim().Substring(0,2) == "00") && (await CheckCpeiii() == false))
                        {
                            if (MO_Type.IndexOf("215") > -1)
                            {
                                strGetDataRoute = "select * from sfis1.c_route_name_t where route_name in ('SMT_DAN')";
                                var qry_DataRoute = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetDataRoute,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_DataRoute.Data.Count() > 0)
                                {
                                    foreach (var row in qry_DataRoute.Data)
                                    {
                                        Cbb_Route.Items.Add(row["route_name"]);
                                    }
                                }
                            }
                            else
                            {
                                strGetDataRoute = "select * from sfis1.c_route_name_t where route_name like '%SMT%' OR route_name like 'KIT'";
                                var qry_DataRoute = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                {
                                    CommandText = strGetDataRoute,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_DataRoute.Data.Count() > 0)
                                {
                                    foreach (var row in qry_DataRoute.Data)
                                    {
                                        Cbb_Route.Items.Add(row["route_name"]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            strGetDataRoute = $"select * from sfis1.c_route_name_t where route_name like ('{Cbb_Model.Text}%')";
                            var qry_DataRoute = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetDataRoute,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_DataRoute.Data.Count() > 0)
                            {
                                foreach (var row in qry_DataRoute.Data)
                                {
                                    Cbb_Route.Items.Add(row["route_name"]);
                                }
                            }
                        }
                    }
                    else
                    {
                        Cbb_Route.Items.Clear();
                        if ((Edt_MO.Text.Trim().Substring(0,2) == "00") && (await CheckCpeiii() == false))
                        {
                            strGetDataRoute = "select * from sfis1.c_route_name_t where route_name like '%SMT%'";
                            var qry_DataRoute = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetDataRoute,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_DataRoute.Data.Count() > 0)
                            {
                                foreach (var row in qry_DataRoute.Data)
                                {
                                    Cbb_Route.Items.Add(row["route_name"]);
                                }
                                Cbb_Route.Text = Cbb_Route.Items[0].ToString();
                            }
                            else
                            {
                                Cbb_Route.Text = "";
                            }
                        }
                        else
                        {
                            strGetDataRoute = $"select * from sfis1.c_route_name_t where route_name like ('{Cbb_Model.Text}%')";
                            var qry_DataRoute = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetDataRoute,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_DataRoute.Data.Count() > 0)
                            {
                                foreach (var row in qry_DataRoute.Data)
                                {
                                    Cbb_Route.Items.Add(row["route_name"]);
                                }
                                Cbb_Route.Text = Cbb_Route.Items[0].ToString();
                            }
                            else
                            {
                                Cbb_Route.Text = "";
                            }
                        }
                    }*/
                }
                The_Type = await GetShipaddr();
                Lb_ShipTo.Content = "";
                Lb_ShipTo.Visibility = Visibility.Hidden;
                Cbb_ShipTo.Items.Clear();

                string strGetShipAdd = "SELECT * FROM SFIS1.C_SHIP_ADDR_T ORDER BY SHIP_INDEX";
                var qry_ShipAdd = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetShipAdd,
                    SfcCommandType = SfcCommandType.Text
                });
                Cbb_ShipTo.Items.Add("N/A");
                foreach (var row in qry_ShipAdd.Data)
                {
                    Cbb_ShipTo.Items.Add(row["ship_index"]);
                }
                if ((isNewMo == true) && (The_Type == false))
                {
                    Cbb_ShipTo.Text = Cbb_ShipTo.Items[0].ToString();
                }
                if ((isUpdateMO == true) && (The_Type == false))
                {
                    if ((ship_index != "") && (ship_index != "N/A"))
                    {
                        Cbb_ShipTo.Items.Clear();
                        Lb_ShipTo.Visibility = Visibility.Visible;
                        Lb_ShipTo.Content = ship_addr;
                    }
                    Cbb_ShipTo.Text = Cbb_ShipTo.Items[Cbb_ShipTo.Items.IndexOf(ship_index)].ToString();
                }
                if ((isUpdateMO == true) && (The_Type == true))
                {
                    Cbb_ShipTo.Text = Cbb_ShipTo.Items[Cbb_ShipTo.Items.IndexOf(ship_index)].ToString();
                    Lb_ShipTo.Visibility = Visibility.Visible;
                    Lb_ShipTo.Content = ship_addr;
                }
                if (ship_index == "")
                {
                    Cbb_ShipTo.Text = Cbb_ShipTo.Items[Cbb_ShipTo.Items.IndexOf("N/A")].ToString();
                }
                Edt_Option.Text = Cbb_ShipTo.Text.Trim();
            }
            if (Edt_MO.Text.Trim().Substring(0, 2) != "00")
            {
                Lb_ScrapQty.Visibility = Visibility.Hidden;
                Cbb_ScrapQty.Visibility = Visibility.Hidden;
            }
            if (await CheckCpeiii() == true)
            {
                await GetShipType(Edt_MO.Text);
            }
            if (await CheckTAUse(Edt_MO.Text))
            {
                Lb_TA.Visibility = Visibility.Visible;
                Grid_TA.Visibility = Visibility.Visible;
                Edt_TA.Text = "";
            }
        }
        private async Task<bool> CheckCpeiii()
        {
            string strGetData = "select * from sfis1.c_model_desc_t  where model_name='SFISSITE' and model_serial='Y'";
            var qry_Data = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });

            if (qry_Data.Data.Count() == 0)
            {
                return false;
            }
            else
            {
                string strGetmo_ao = "SELECT * FROM SFIS1.C_PARAMETER_INI where prg_name ='PM' and  VR_CLASS='T' and vr_value ='Y'";
                var strGet_data = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetmo_ao,
                    SfcCommandType = SfcCommandType.Text
                });
                if (strGet_data.Data.Count() == 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private async Task<bool> CheckWhs(string _MO)
        {
            string strGetData = "select * from sfis1.C_PARAMETER_INI WHERE VR_VALUE='Y' AND PRG_NAME='SCRAP'";
            var qry_Data = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Data.Data.Count() == 0)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> GetReuseSNvalue(string _MO)
        {
            string strGetData = $"select MSN_MO_OPTION from sfism4.r105 WHERE MO_NUMBER='{_MO}'";
            var qry_Data = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Data.Data.Count() == 0)
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
        private async Task<bool> GetRoute_Group(string _MO)
        {
            string strGetData = "SELECT C.ROUTE_NAME ,A.DEFAULT_GROUP,A.END_GROUP,A.STANDARD"
                + " FROM SFIS1.C_MODEL_DESC_T A ,SFIS1.C_ROUTE_CONTROL_T  B,SFIS1.C_ROUTE_NAME_T C"
                + " WHERE B.ROUTE_CODE=C.ROUTE_CODE AND A.ROUTE_CODE=B.ROUTE_CODE AND A.MODEL_NAME=:MODEL_NAME";
            var qry_Route_Group = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MODEL_NAME",Value=_MO}
                }
            });
            try
            {
                Cbb_Route.Text = qry_Route_Group.Data["route_name"]?.ToString() ?? "";
                Cbb_FirstGroup.Text = qry_Route_Group.Data["default_group"]?.ToString() ?? "";
                Cbb_LastGroup.Text = qry_Route_Group.Data["end_group"]?.ToString() ?? "";
                Cbb_MOKind.Text = qry_Route_Group.Data["standard"]?.ToString() ?? "";
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        private async Task<bool> CheckScrap()
        {
            string strGetData = "select * from sfis1.C_PARAMETER_INI WHERE VR_VALUE='Y' AND PRG_NAME='SCRAP'";
            var qry_Scrap = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Scrap.Data.Count() == 0)
            {
                return false;
            }
            return true;
        }
        private async Task<bool> CheckMoModel(string _MO)
        {
            string strGetData = "select * from sfis1.c_model_desc_t where model_name=:model and rownum=1";
            var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="model",Value=_MO}
                }
            });
            if (qry_Model.Data != null)
            {
                string sModel_type = qry_Model.Data["model_type"]?.ToString() ?? "";
                if (sModel_type.IndexOf("118") > -1)
                {
                    return true;
                }
            }
            return false;
        }
        private async Task<bool> GetShipaddr()
        {
            string tmpstr;
            ship_addr = "";
            ship_index = "";
            isNewMo = false;
            isUpdateMO = false;

            string strGetMO = $"select * from sfism4.r105 where mo_number='{Edt_MO.Text}'";
            var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetMO,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_MO.Data == null)
            {
                isNewMo = true;
                return false;
            }
            tmpstr = qry_MO.Data["option_desc"].ToString();
            if ((tmpstr.Trim() == "N/A") || (tmpstr.Trim() == ""))
            {
                isUpdateMO = true;
                return false;
            }

            string strGetShipIndex = $"select * from sfis1.c_ship_addr_t where ship_index='{tmpstr.Trim()}'";
            var qry_ShipIndex = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetShipIndex,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ShipIndex.Data == null)
            {
                MessageBox.Show("Not found data" + tmpstr, "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                ship_index = tmpstr;
                isUpdateMO = true;
                return false;
            }
            isUpdateMO = true;
            ship_index = tmpstr;
            ship_addr = qry_ShipIndex.Data["ship_address"]?.ToString() ?? "";
            return true;
        }
        private async Task<bool> GetShipType(string _MO)
        {
            string MO_WHS;
            string strGetShipType = $"SELECT IMEI_MO_OPTION FROM SFISM4.R105 WHERE MO_NUMBER = '{_MO}'";
            var qry_ShipType = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetShipType,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_ShipType.Data == null)
            {
                Rb_Ocean.IsChecked = false;
                Rb_Air.IsChecked = false;
                string strGetWHS = $"select WHS from sfism4.r_bpcs_moplan_t where mo_number='{_MO}'";
                var qry_WHS = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetWHS,
                    SfcCommandType = SfcCommandType.Text
                });
                MO_WHS = qry_WHS.Data["whs"].ToString();
                if (MO_WHS == "BC4F")
                {
                    GB_ShipType.Visibility = Visibility.Visible;
                }
                else
                {
                    GB_ShipType.Visibility = Visibility.Hidden;
                }
                return true;
            }
            else
            {
                MO_WHS = qry_ShipType.Data["imei_mo_option"]?.ToString() ?? "";
                if (MO_WHS.Trim() == "")
                {
                    GB_ShipType.Visibility = Visibility.Hidden;
                    return false;
                }
                GB_ShipType.Visibility = Visibility.Visible;
                if (MO_WHS == "S")
                {
                    Rb_Ocean.IsChecked = true;
                }
                else
                {
                    Rb_Air.IsChecked = true;
                }
                return true;
            }
        }
        private async Task<bool> CheckTAUse(string _MO)
        {
            string mo_type;
            string strGetTA = "select model_type from sfis1.c_model_desc_t where model_name in"
                + " ( select model_name from sfism4.r_bpcs_moplan_t where mo_number=:h_mo)";
            var qry_TA = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetTA,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="h_mo",Value=_MO}
                }
            });
            mo_type = qry_TA.Data["model_type"]?.ToString() ?? "";
            if ((mo_type.IndexOf("189") > -1) || (mo_type.IndexOf("G55") > -1))
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckTModel(string _Model)
        {
            if ((_Model.Substring(1, _Model.Length - 1)).IndexOf("T") > -1)
            {
                string strGetParam = $"select * from sfis1.C_PACK_PARAM_T where model_name='{_Model}'";
                var qry_Param = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetParam,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Param.Data.Count() == 0)
                {
                    return false;
                }
            }
            return true;
        }
        private bool CheckShipType()
        {
            isS = false;
            isP = false;
            if (GB_ShipType.IsVisible == false) return false;
            if (GB_ShipType.IsVisible == true)
            {
                if ((Rb_Ocean.IsChecked == false) && (Rb_Air.IsChecked == false))
                {
                    return false;
                }
            }
            else
            {
                if (Rb_Ocean.IsChecked == true)
                {
                    isS = true;
                }
                if (Rb_Air.IsChecked == true)
                {
                    isP = true;
                }
            }
            return true;
        }
        private void TrimAllField()
        {
            if (!string.IsNullOrEmpty(Cbb_MO.Text))
            {
                Cbb_MO.Text = Cbb_MO.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_MO.Text))
            {
                Edt_MO.Text = Edt_MO.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_Model.Text))
            {
                Cbb_Model.Text = Cbb_Model.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_TargetQTY.Text))
            {
                Edt_TargetQTY.Text = Edt_TargetQTY.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_Route.Text))
            {
                Cbb_Route.Text = Cbb_Route.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_Line.Text))
            {
                Cbb_Line.Text = Cbb_Line.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_FirstGroup.Text))
            {
                Cbb_FirstGroup.Text = Cbb_FirstGroup.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_LastGroup.Text))
            {
                Cbb_LastGroup.Text = Cbb_LastGroup.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_MoType.Text))
            {
                Cbb_MoType.Text = Cbb_MoType.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_MOKind.Text))
            {
                Cbb_MOKind.Text = Cbb_MOKind.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_Version.Text))
            {
                Edt_Version.Text = Edt_Version.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_BOM.Text))
            {
                Cbb_BOM.Text = Cbb_BOM.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Cbb_PartNO.Text))
            {
                Cbb_PartNO.Text = Cbb_PartNO.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_SN.Text))
            {
                Edt_SN.Text = Edt_SN.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_Shift.Text))
            {
                Edt_Shift.Text = Edt_Shift.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_ReMark.Text))
            {
                Edt_ReMark.Text = Edt_ReMark.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_CustPN.Text))
            {
                Edt_CustPN.Text = Edt_CustPN.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_SO.Text))
            {
                Edt_SO.Text = Edt_SO.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_SoLine.Text))
            {
                Edt_SoLine.Text = Edt_SoLine.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_UPC.Text))
            {
                Edt_UPC.Text = Edt_UPC.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_Option.Text))
            {
                Edt_Option.Text = Edt_Option.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_HWBOM.Text))
            {
                Edt_HWBOM.Text = Edt_HWBOM.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_SWBOM.Text))
            {
                Edt_SWBOM.Text = Edt_SWBOM.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_PackLot.Text))
            {
                Edt_PackLot.Text = Edt_PackLot.Text.Trim();
            }
            if (!string.IsNullOrEmpty(Edt_SW_BOM.Text))
            {
                Edt_SW_BOM.Text = Edt_SW_BOM.Text.Trim();
            }
        }
        private bool CheckFieldsValid()
        {
            string sErrMsgBuf;
            if (string.IsNullOrEmpty(Edt_MO.Text))
            {
                MessageBox.Show("00008 - Nhap cong lenh - Input MO number", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_MO.Focus();
                Edt_MO.SelectAll();
                return false;
            }
            if (string.IsNullOrEmpty(Cbb_Model.Text))
            {
                MessageBox.Show("00244 - Nhap ten dong san pham - Input model name", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Cbb_Model.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(Cbb_Route.Text))
            {
                MessageBox.Show("00245 - Chon lua luu trinh - Choose route", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Cbb_Route.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(Cbb_Line.Text))
            {
                MessageBox.Show("00179 - Nhap ten chuyen - Please input Line Name", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Cbb_Line.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(Cbb_FirstGroup.Text))
            {
                MessageBox.Show("00246 - Nhap tram dau tien cua cong lenh - Input a wo the first", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Cbb_FirstGroup.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(Cbb_LastGroup.Text))
            {
                MessageBox.Show("00247 - Nhap tram cuoi cung cua cong lenh - Input a wo the end", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Cbb_LastGroup.Focus();
                return false;
            }
            if ((string.IsNullOrEmpty(Edt_SN.Text)) && (Rb_Primary.IsChecked == true))
            {
                MessageBox.Show("Serial NO is null,Please input again!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_SN.Focus();
                Edt_SN.SelectAll();
                return false;
            }
            if (string.IsNullOrEmpty(Edt_TargetQTY.Text))
            {
                MessageBox.Show("This TARGET Quality is null,please input again!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_TargetQTY.Focus();
                Edt_TargetQTY.SelectAll();
                return false;
            }
            if (Edt_PackLot.Text.Length > 16)
            {
                MessageBox.Show("This Pack Lot NO. Length > 18 char,please input again!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_PackLot.Focus();
                Edt_PackLot.SelectAll();
                return false;
            }
            if (Rb_Primary.IsChecked == true)
            {
                if (bSNFixFlag)
                {
                    if (Edt_SN.Text.Length != iSNLen)
                    {
                        sErrMsgBuf = $"This length of serial number is not equal to '{iSNLen.ToString()}',please input again";
                        MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_SN.Focus();
                        Edt_SN.SelectAll();
                        return false;
                    }
                }
                else
                {
                    if (Edt_SN.Text.Length > iSNLen)
                    {
                        sErrMsgBuf = $"This length of serial number is greater than '{iSNLen.ToString()}', please input again";
                        MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_SN.Focus();
                        Edt_SN.SelectAll();
                        return false;
                    }
                    else if (Edt_SN.Text.Length < (iCurrentNOLen + iCurrentNOStartDigit - 1))
                    {
                        sErrMsgBuf = "This digit of current number is over than serial number length,please input again";
                        MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_SN.Focus();
                        Edt_SN.SelectAll();
                        return false;
                    }
                }
            }
            if (Cbb_MO.Text == "LOADFROMRMA")
            {
                if (Edt_MO.Text.Length > 25)
                {
                    sErrMsgBuf = $"This length of MO number is not equal to '{iMOLen}',please input again";
                    MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_MO.Focus();
                    Edt_MO.SelectAll();
                    return false;
                }
            }
            for (int i = 0; i < 9; i++)
            {
                if (FieldFlag[i])
                {
                    switch (i)
                    {
                        case 0:
                            if (string.IsNullOrEmpty(Edt_HWBOM.Text))
                            {
                                sErrMsgBuf = "HW Bom can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                            break;
                        case 1:
                            if (string.IsNullOrEmpty(Edt_Option.Text))
                            {
                                sErrMsgBuf = "Option can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                            break;
                        case 2:
                            if (string.IsNullOrEmpty(Edt_FormAddPO.Text))
                            {
                                sErrMsgBuf = "PO can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                            break;
                        case 3:
                            if (string.IsNullOrEmpty(Edt_SWBOM.Text))
                            {
                                sErrMsgBuf = "SW Bom can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                            break;
                        case 4:
                            if (string.IsNullOrEmpty(Edt_UPC.Text))
                            {
                                sErrMsgBuf = "UPC can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                Edt_UPC.Focus();
                                Edt_UPC.SelectAll();
                                return false;

                            }
                            break;
                        case 5:
                            if (string.IsNullOrEmpty(Edt_Version.Text))
                            {
                                sErrMsgBuf = "Version can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                Edt_Version.Focus();
                                Edt_Version.SelectAll();
                                return false;

                            }
                            break;
                        case 6:
                            if (string.IsNullOrEmpty(Cbb_BOM.Text))
                            {
                                sErrMsgBuf = "BOM NO can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                Cbb_BOM.Focus();
                                return false;

                            }
                            break;
                        case 7:
                            if (string.IsNullOrEmpty(Cbb_MoType.Text))
                            {
                                sErrMsgBuf = "MO Type can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                Cbb_MoType.Focus();
                                return false;
                            }
                            break;
                        case 8:
                            if (string.IsNullOrEmpty(Cbb_PartNO.Text))
                            {
                                sErrMsgBuf = "BOM NO can't be null value,please input again";
                                MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                Cbb_PartNO.Focus();
                                return false;
                            }
                            break;
                    }
                }
            }
            return true;
        }
        private async Task<bool> GetIsinputFW(string _Model)
        {
            string ModelType;
            string strGetModelType = "select model_type from sfis1.c_model_desc_t where model_name =:smodel_name";
            var qry_ModelType = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModelType,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="smodel_name",Value=_Model}
                }
            });
            ModelType = qry_ModelType.Data["model_type"]?.ToString() ?? "";
            if (ModelType.IndexOf("P") > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async void UpdateREuse(string _MO, string _Use)
        {
            string strUpdate = "update sfism4.r105 set MSN_MO_OPTION=:sn_use WHERE MO_NUMBER=:MO";
            var Update = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strUpdate,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO",Value=_MO},
                    new SfcParameter{Name="sn_use",Value=_Use}
                }
            });
        }
        private async Task<bool> CheckMolink(string _MO)
        {
            string lStr, lStr2;
            int lPos;
            lStr = _MO;
            lPos = lStr.IndexOf(",");
            if (lPos == -1)
            {
                string strGetMO = $"select * from sfism4.r_mo_base_t where mo_number='{lStr}'";
                var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetMO,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_MO.Data == null)
                {
                    return false;
                }
            }
            else
            {
                if (lPos > 0)
                {
                    lStr2 = lStr.Substring(0, lPos);
                    string strGetMO = $"select * from sfism4.r_mo_base_t where mo_number='{lStr2}'";
                    var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetMO,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_MO.Data == null)
                    {
                        return false;
                    }
                }
                lStr.Remove(0, lPos);
            }
            return true;
        }
        public async Task<bool> MailInsert(string _Sendwho, string _Mailtext1, string _Mailtext2)
        {
            string MailList;
            string strGetCustomer = "SELECT count(*) AA FROM  SFIS1.C_CUSTOMER_FTP_ACCOUNT_T WHERE CUSTOMER_CODE=:MCUSTOMER_CODE";
            var qry_Customer = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustomer,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MCUSTOMER_CODE",Value=_Sendwho}
                }
            });
            if (Int32.Parse(qry_Customer.Data["AA"].ToString()) == 0)
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
        private async void FillSectionGroup(string _RouteCode)
        {
            try
            {
                string strDelete = "Delete SFISM4.R_MO_TO_GROUP_T WHERE MO_NUMBER =:MO";
                var Delete = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strDelete,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="MO",Value=Edt_MO.Text}
                    }
                });
                string strGetData = "SELECT AA.ROUTE_CODE,AA.SECTION_NAME,BB.GROUP_NEXT INPUT_GROUP,CC.GROUP_NEXT OUTPUT_GROUP"
                    + " FROM (SELECT A.ROUTE_CODE,B.SECTION_NAME,MIN(A.STEP_SEQUENCE) INPUT_STEP,MAX(A.STEP_SEQUENCE) OUTPUT_STEP"
                    + " FROM SFIS1.C_ROUTE_CONTROL_T A,SFIS1.C_GROUP_CONFIG_T B"
                    + " WHERE A.GROUP_NEXT = B.GROUP_NAME"
                    + " AND A.STATE_FLAG = 0"
                    + " AND A.GROUP_NEXT NOT LIKE 'R_%'"
                    + " AND A.GROUP_NEXT <> '0'"
                    + " AND A.GROUP_NEXT <> 'X'"
                    + " GROUP BY A.ROUTE_CODE,B.SECTION_NAME) AA,"
                    + " SFIS1.C_ROUTE_CONTROL_T BB,SFIS1.C_ROUTE_CONTROL_T CC"
                    + " WHERE AA.ROUTE_CODE = BB.ROUTE_CODE"
                    + " AND AA.ROUTE_CODE = CC.ROUTE_CODE"
                    + $" AND AA.ROUTE_CODE = '{_RouteCode}'"
                    + " AND AA.INPUT_STEP = BB.STEP_SEQUENCE"
                    + " AND AA.OUTPUT_STEP = CC.STEP_SEQUENCE";
                var qry_Data = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetData,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Data.Data != null)
                {
                    dynamic ads = qry_Data.Data;
                    for (int i = 0; i < qry_Data.Data.Count(); i++)
                    {
                        string strInsert = "INSERT INTO SFISM4.R_MO_TO_GROUP_T"
                        + " (MO_NUMBER,SECTION_NAME,INPUT_GROUP,OUTPUT_GROUP)"
                        + " Values"
                        + " (:MO,:Section,:InputGroup,:OutputGroup)";
                        var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strInsert,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="MO",Value=Edt_MO.Text},
                                new SfcParameter{Name="Section",Value=ads[i]["section_name"]},
                                new SfcParameter{Name="InputGroup",Value=ads[i]["input_group"]},
                                new SfcParameter{Name="OutputGroup",Value=ads[i]["output_group"]}
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private async Task<bool> CheckSN_MOVE(string _START_SN, string _END_SN)
        {
            string strGetSNMode = "select COUNT(*) NUM FROM SFISM4.R_SN_MOVE_T WHERE LENGTH(:START_SN)=LENGTH(SERIAL_NUMBER)"
                 + " AND (SERIAL_NUMBER BETWEEN :START_SN AND :END_SN )";
            var qry_SNMode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetSNMode,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="START_SN",Value=_START_SN},
                    new SfcParameter{Name="END_SN",Value=_END_SN}
                }
            });
            if (Int32.Parse(qry_SNMode.Data["num"].ToString()) != 0)
            {
                return true;
            }
            return false;
        }
        private async void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            string Model_Type, Scarton_qty, MO_Version, sRouteCode, sCustCode, sVenderPN, Stbkey_num;
            string Model_Serial, Model_Product, Tmp_mo, Rma_mo_pre, Inputqty, sLog;
            string sStrTmpHead, sStrCurrentNO, sStrTmpTail;
            bool bFlag;
            int a_qty, b_qty, k_div;
            char tmp_char;
            Model_Product = "";
            sLog = "";
            if (Lb_CustPNo.Content.ToString() == "Build Phase")
            {
                if (string.IsNullOrEmpty(Edt_CustPNo.Text.Trim()))
                {
                    MessageBox.Show("Hay nhap Build Phase cua cong lenh", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_CustPNo.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(Edt_CustPN.Text.Trim()))
                {
                    MessageBox.Show("Hay nhap Device Config", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_CustPNo.Focus();
                    return;
                }
            }

            string strGetModel = $"select * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='{Cbb_Model.Text}'";
            var qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetModel,
                SfcCommandType = SfcCommandType.Text
            });
            Model_Type = qry_Model.Data["model_type"]?.ToString() ?? "";

            if (Model_Type.IndexOf("G06") > -1)
            {
                string strGetPackParam = $"select * FROM SFIS1.C_PACK_PARAM_T WHERE MODEL_NAME='{Cbb_Model.Text}' AND ROWNUM=1";
                var qry_PackParam = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetPackParam,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_PackParam.Data != null)
                {
                    Scarton_qty = qry_PackParam.Data["carton_qty"].ToString();
                    a_qty = Int32.Parse(Scarton_qty);
                    b_qty = Int32.Parse(Edt_TargetQTY.Text);
                    k_div = b_qty / a_qty;
                    if (k_div > 0)
                    {
                        for (int i = 0; i < k_div; i++)
                        {
                            string strGetData = $"select *  from SFISM4.R_PO_SN_DETAIL_T WHERE PO_ORDER='" + i.ToString() + "' AND PO_NO='{Cbb_Model.Text}' AND MO_NUMBER='{Edt_MO.Text}' AND MODEL_NAME='CARTON_NO'";
                            var qry_Data = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strGetData,
                                SfcCommandType = SfcCommandType.Text
                            });
                            if (qry_Data.Data.Count() == 0)
                            {
                                try
                                {
                                    string strInsert = $"Insert into SFISM4.R_PO_SN_DETAIL_T(PO_ORDER, PO_NO, MO_NUMBER, MODEL_NAME) Values ('{i.ToString()}', '{Cbb_Model.Text}', '{Edt_MO.Text}', 'CARTON_NO')";
                                    var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                    {
                                        CommandText = strInsert,
                                        SfcCommandType = SfcCommandType.Text
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Goi IE thiet lap config 15", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (Edt_Version.Text.Length > 1)
                {
                    MessageBox.Show("VERSION CUA CONG LENH PHAI LA 1 KY TU", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (Model_Type.IndexOf("189") > -1)
            {
                string strChkConfig20 = $"SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PM' AND VR_NAME='CHKCONFIG20' AND VR_VALUE='TRUE'";
                var qry_ChkConfig20 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strChkConfig20,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ChkConfig20.Data.Count() == 0)
                {
                    string strGetVersion = $"SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME='{Cbb_Model.Text }' and version_code='{Edt_Version.Text}'";
                    var qry_Version = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetVersion,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Version.Data != null)
                    {
                        string sUpceandata = qry_Version.Data["upceandata"]?.ToString() ?? "";
                        if ((qry_Version.Data == null) || (sUpceandata == ""))
                        {
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = "00084 - " + await GetPubMessage("00084");
                            _er.MessageVietNam = "00084 - " + await GetPubMessage("00084");
                            _er.ShowDialog();
                            Edt_TA.SelectAll();
                            Edt_TA.Focus();
                            Edt_TA.Background = Brushes.Yellow;
                            return;
                        }
                        else
                        {
                            if (Edt_TA.Text != qry_Version.Data["upceandata"].ToString())
                            {
                                MessageBoxResult result_ta = MessageBox.Show("XAC NHAN TA_VER MOI SE LA :" + Edt_TA.Text, "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                                if (result_ta == MessageBoxResult.Yes)
                                {
                                    try
                                    {
                                        string strUpdate = $"UPDATE SFIS1.C_CUST_SNRULE_T SET UPCEANDATA='{Edt_TA.Text}'"
                                            + $" WHERE MODEL_NAME='{Cbb_Model.Text}' and version_code='{Edt_Version.Text}'";
                                        var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                                        {
                                            CommandText = strUpdate,
                                            SfcCommandType = SfcCommandType.Text
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
                                                new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                                                new SfcParameter{Name="PRG_NAME",Value="PM"},
                                                new SfcParameter{Name="ACTION_TYPE",Value="MODIFY TA_VER"},
                                                new SfcParameter{Name="ACTION_DESC",Value= $"MO:{Edt_MO.Text},VER:{Edt_Version.Text},CHANGE TA_VER:From {qry_Version.Data["upceandata"].ToString()}, To:{Edt_TA.Text},IP Address:{frm_MO_ManageForm.local_strIP}"}
                                            }
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                        return;
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00084 - " + await GetPubMessage("00084");
                        _er.MessageVietNam = "00084 - " + await GetPubMessage("00084");
                        _er.ShowDialog();
                        Edt_TA.SelectAll();
                        Edt_TA.Focus();
                        Edt_TA.Background = Brushes.Yellow;
                        return;
                    }
                }
                else
                {
                    string strGetConfig20 = $"select * from sfis1.c_model_confirm_t where MODEL_TYPE='{Edt_MO.Text}' and TYPE_NAME='NPIMOCONFIG'";
                    var qry_Config20 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetConfig20,
                        SfcCommandType = SfcCommandType.Text
                    });
                    string sData = qry_Config20.Data["data"]?.ToString() ?? "";
                    if ((qry_Config20.Data == null) || (sData == ""))
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "PM no setup PO in  Config20";
                        _er.MessageVietNam = "PM chua setup PO trong Config20";
                        _er.ShowDialog();
                        Edt_TA.Focus();
                        Edt_TA.SelectAll();
                        Edt_TA.Background = Brushes.Red;
                        return;
                    }
                    else
                    {
                        if (Edt_TA.Text != sData)
                        {
                            ShowMessageForm _er = new ShowMessageForm(sfcClient);
                            _er.CustomFlag = true;
                            _er.MessageEnglish = $"PO not same PO {qry_Config20.Data["data"].ToString()} PM setup";
                            _er.MessageVietNam = $"PO  khac voi PO {qry_Config20.Data["data"].ToString()} PM thiet lap";
                            _er.ShowDialog();
                            Edt_TA.Background = Brushes.Red;
                            Edt_TA.Focus();
                            Edt_TA.SelectAll();
                            Edt_TA.Background = Brushes.Red;
                            return;
                        }
                    }
                }
            }

            /// Check target_qty
            string _strCountr107 = "SELECT count(*) as qty FROM SFISm4.r107 where mo_number = :mo_number";
            var _qry_Count107 = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = _strCountr107,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="mo_number",Value=Edt_MO.Text}
                }
            });
            int qty_r107 = int.Parse(_qry_Count107.Data["qty"]?.ToString() ?? "");
            int qty_r105 = int.Parse(Edt_TargetQTY.Text);
            if (qty_r107 > qty_r105)
            {
                MessageBox.Show("Số lượng công lệnh hiện tại đã vượt quá số lượng target_qty đã đặt! -- Pls Check", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string strGetParam = "select * from SFIS1.C_PARAMETER_INI where VR_CLASS='CHECK_RANGE' AND PRG_NAME='PM'";
            var qry_Param = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetParam,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Param.Data.Count() > 0)
            {
                string strGetRange = $"SELECT * FROM SFISM4.R_MO_EXT_T WHERE MO_NUMBER='{Edt_MO.Text} 'AND ROWNUM=1";
                var qry_Range = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetRange,
                    SfcCommandType = SfcCommandType.Text
                });
                MO_Version = qry_Range.Data["ver_1"]?.ToString() ?? "";
                if (qry_Range.Data == null)
                {
                    MessageBox.Show("Vung gia tri Mo trong EXT bang khong ton tai", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_Version.Focus();
                    return;
                }
                else
                {
                    if (MO_Version != Edt_Version.Text.Trim())
                    {
                        MessageBox.Show("LanID khong phai la LanID lon nhat", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_Version.Focus();
                        Edt_Version.SelectAll();
                        return;
                    }
                }
            }
            string strGetMO = $"SELECT * from sfism4.r105 where mo_number='{Edt_MO.Text}'";
            var qry_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetMO,
                SfcCommandType = SfcCommandType.Text
            });
            List<ListMO> ads_mo = qry_MO.Data.ToListObject<ListMO>().ToList();
            c_count = qry_MO.Data.Count();
            if (c_count > 0)
            {
                if (Edt_TargetQTY.Text != Convert.ToString(ads_mo[0].TARGET_QTY))
                {
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
                                new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                                new SfcParameter{Name="PRG_NAME",Value="PM"},
                                new SfcParameter{Name="ACTION_TYPE",Value="MODIFY QTY"},
                                new SfcParameter{Name="ACTION_DESC",Value= $"MO:{Edt_MO.Text},QTY: From {ads_mo[0].TARGET_QTY} To {Edt_TargetQTY.Text},IP Address:{frm_MO_ManageForm.local_strIP}"}
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
                if (Edt_Version.Text != ads_mo[0].VERSION_CODE)
                {
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
                                new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                                new SfcParameter{Name="PRG_NAME",Value="PM"},
                                new SfcParameter{Name="ACTION_TYPE",Value="MODIFY VERSION"},
                                new SfcParameter{Name="ACTION_DESC",Value= $"MO:{Edt_MO.Text},VERSION: From {ads_mo[0].VERSION_CODE} To {Edt_Version.Text},IP Address:{frm_MO_ManageForm.local_strIP}"}
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                }
            }
            if (Cbb_Route.Text == "")
            {
                MessageBox.Show("Chưa chọn lưu trình cho công lệnh- Not set route for MO", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string strGetRouteCode = "SELECT ROUTE_CODE FROM SFIS1.C_ROUTE_NAME_T where route_name = :RouteName";
            var qry_RouteCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetRouteCode,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="RouteName",Value=Cbb_Route.Text}
                }
            });
            if (qry_RouteCode.Data == null)
            {
                MessageBox.Show("Không tìm thấy lưu trình  - Route not found", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            sRouteCode = qry_RouteCode.Data["route_code"].ToString();
            if (string.IsNullOrEmpty(Edt_Version.Text.Trim()))
            {
                MessageBox.Show("00117 - Chua nhap thong tin phien ban - Do not input a version information", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_Version.Focus();
                return;
            }
            else
            {
                if ((Edt_Version.Text.Trim().ToUpper() == "N/A") || (Edt_Version.Text.Trim().ToUpper() == "NA"))
                {
                    MessageBox.Show("80180 - Phien ban trong MO_EXT cua cong lenh sai - Please check VERSION_CODE on R_MO_EXT_T", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_Version.Focus();
                    return;
                }
            }
            if (Cbb_MoType.Text == "Rework")
            {
                if ((string.IsNullOrEmpty(Cbb_MAC.Text)) && (Edt_MO.Text.Substring(0, 2) != "74"))
                {
                    MessageBox.Show("00238 - Phien ban trong MO_EXT cua cong lenh sai - Needs to choose MAC category on line of rework mo", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_Version.Focus();
                    return;
                }
                string strUpdate = $"UPDATE SFISM4.R105 SET ORDER_NO='{Cbb_MAC.Text}' WHERE MO_NUMBER='{Edt_MO.Text}'";
                var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strUpdate,
                    SfcCommandType = SfcCommandType.Text
                });
            }

            string _strGetModel = "select * from sfis1.c_model_desc_t where model_name='SFISSITE'";
            var _qry_Model = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = _strGetModel,
                SfcCommandType = SfcCommandType.Text
            });
            if (_qry_Model.Data["model_serial"].ToString() == "N")
            {
                string strGetCustCode = "select CUSTOMER, CUST_CODE from SFIS1.C_CUSTOMER_T where CUSTOMER =:Customer order by CUSTOMER";
                var qry_CustCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetCustCode,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="Customer",Value=Cbb_Customer.Text}
                    }
                });
                sCustCode = qry_CustCode.Data["cust_code"]?.ToString() ?? "";
            }
            else
            {
                sCustCode = "80190";
            }
            try
            {
                string strUpdate = $"UPDATE SFISM4.R107 SET CUSTOMER_NO='{sCustCode}', BOM_NO='{Cbb_BOM.Text}',VERSION_CODE='{Edt_Version.Text}',SPECIAL_ROUTE=:SPECIAL_ROUTE WHERE MO_NUMBER='{Edt_MO.Text}'";
                var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strUpdate,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SPECIAL_ROUTE",Value=sRouteCode}
                    }
                });

                string _strUpdate = $"UPDATE SFISM4.R_BPCS_MOPLAN_T SET TARGET_QTY='{Edt_TargetQTY.Text}' WHERE MO_NUMBER='{Edt_MO.Text}'";
                var _Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = _strUpdate,
                    SfcCommandType = SfcCommandType.Text
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            Lb_Mess.Content = "1";
            RouteGraphForm frm_RouteGraphForm = new RouteGraphForm(this, sfcClient);
            frm_RouteGraphForm.ShowDialog();
            MessageBoxResult result = MessageBox.Show("00239 - Thẻ lưu trình có chính xác không? -- Route whether exactitude?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                Lb_Mess.Content = "0";
                return;
            }
            CheckBOMForm frm_CheckBOMForm = new CheckBOMForm(this, sfcClient);
            frm_CheckBOMForm.ShowDialog();
            MessageBoxResult _result = MessageBox.Show("00240 - Bom có chính xác không? -- BOM whether exactitude?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (_result == MessageBoxResult.No)
            {
                Lb_Mess.Content = "0";
                return;
            }
            Lb_Mess.Content = "0";
            if (await CheckCpeiii() == true)
            {
                if (Mode == "ADD")
                {
                    if ((Edt_MO.Text.Substring(0, 2) != "00") && (await CheckTModel(Cbb_Model.Text)))
                    {
                        CheckConfigForm frm_CheckConfigForm = new CheckConfigForm(this, sfcClient, Cbb_BOM.Text, Edt_Version.Text, Cbb_MoType.Text);
                        frm_CheckConfigForm.ShowDialog();
                    }
                }
            }
            if (await CheckCpeiii() == true)
            {
                if (GB_ShipType.IsVisible == true)
                {
                    if (CheckShipType())
                    {
                        MessageBoxResult result_ship = MessageBox.Show("Xuat thuc xuat hang co dung khong? -- Ship_type is right?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result_ship == MessageBoxResult.Yes)
                        {
                            if (Rb_Ocean.IsChecked == true)
                            {
                                ST = "S";
                            }
                            else
                            {
                                ST = "P";
                            }
                            if (ST == "P")
                            {
                                string strGetPackParam = "select * from sfis1.c_pack_param_t where model_name =:model";
                                var qry_PackParam = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = strGetPackParam,
                                    SfcCommandType = SfcCommandType.Text,
                                    SfcParameters = new List<SfcParameter>()
                                    {
                                        new SfcParameter{Name="model",Value=Cbb_Model.Text.Trim()}
                                    }
                                });
                                if (qry_PackParam.Data != null)
                                {
                                    if (qry_PackParam.Data["coo"].ToString() == "")
                                    {
                                        MessageBox.Show("00570 -- Muc config15 chua thiet lap phuong thuc dong goi hang khong, vui long lien he IE -- Config[15] do not set pack Count by Air﹐Pls Call IE", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Lb_Mess.Content = "0";
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("00268 -- Vui long chon mot hinh thuc xuat hang -- Select a ship_type,Pls Check", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Lb_Mess.Content = "0";
                        return;
                    }
                }
            }

            TrimAllField();
            MOClass.IsEnabled = true;
            if (CheckFieldsValid() == false) return;
            if (await GetIsinputFW(Cbb_Model.Text))
            {
                if ((string.IsNullOrEmpty(Edt_PO.Text)) && (Edt_PO.Text == "N/A"))
                {
                    MessageBox.Show("00242 - Nhap F/W phien ban -- Input F/W version", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            string _strGetRouteCode = "SELECT ROUTE_CODE FROM SFIS1.C_ROUTE_NAME_T where route_name = :RouteName";
            var _qry_RouteCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = _strGetRouteCode,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="RouteName",Value=Cbb_Route.Text}
                }
            });
            sRouteCode = _qry_RouteCode.Data["route_code"]?.ToString() ?? "";

            string strGetCustPN = "SELECT VENDER_PART_NO FROM SFIS1.UC_CUST_PN_T WHERE VENDER_CODE =:CUST_CODE AND SELF_PART_NO= :SELF_PART_NO";
            var qry_CustPN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetCustPN,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="SELF_PART_NO",Value=Cbb_PartNO.Text},
                    new SfcParameter{Name="CUST_CODE",Value=sCustCode}
                }
            });
            if (qry_CustPN.Data != null)
            {
                sVenderPN = qry_CustPN.Data["vender_part_no"]?.ToString() ?? "";
            }
            else
            {
                sVenderPN = "";
            }
            if (Cbb_MOKind.Text.Trim() == "LOT")
            {
                string strGetRoute = "SELECT A.ROUTE_CODE FROM SFIS1.C_ROUTE_CONTROL_T A,SFIS1.C_ROUTE_NAME_T B"
                    + $" WHERE A.ROUTE_CODE=B.ROUTE_CODE AND A.GROUP_NAME='FQA' AND B.ROUTE_NAME='{Cbb_Route.Text}'";
                var qry_Route = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetRoute,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Route.Data.Count() > 0)
                {
                    MessageBox.Show("00022 - Loi luu trinh -- Route error||Cong len LOT khong the co tram FQA -- Cannot FQA station of LOT WO", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            try
            {
                //------ Save To Log ------//                
                if ((isUpdateMO == true) && (ship_index != Cbb_ShipTo.Text))
                {
                    string strInsert_Log = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                           + " VALUES ("
                           + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                    var Insert_Log = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsert_Log,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                            new SfcParameter{Name="PRG_NAME",Value="PM"},
                            new SfcParameter{Name="ACTION_TYPE",Value="SHIP_ADDR"},
                            new SfcParameter{Name="ACTION_DESC",Value= $"MO: {Edt_MO.Text}"}
                        }
                    });
                    return;
                }

                string strGetModelPri = "select * from sfis1.c_pirelli_stb_t where model_name=:model_name and mark='2'";
                var qry_ModelPri = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetModelPri,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="model_name",Value=Cbb_Model.Text}
                    }
                });
                if (qry_ModelPri.Data != null)
                {
                    string strUpdate = "update sfis1.c_model_desc_t set model_type='050' WHERE model_name=:model_name";
                    var Update = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strUpdate,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="model_name",Value=Cbb_Model.Text}
                        }
                    });
                }
                string strGetMoType = $"select * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME='{Cbb_Model.Text}'";
                var qry_MoType = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetMoType,
                    SfcCommandType = SfcCommandType.Text
                });
                Model_Type = qry_MoType.Data["model_type"]?.ToString() ?? "";

                //Insert dai tu ext4 => ext cho ROKU
                string strChkROKU = "SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PM' AND VR_CLASS='ROKU' AND VR_VALUE='Y'";
                var qry_ChkROKU = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strChkROKU,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ChkROKU.Data.Count() > 0)
                {
                    if (Model_Type.Contains("040"))
                    {
                        string strGetRangeExt4 = $"select * from sfism4.r_mo_ext4_t where mo_number='{Edt_MO.Text}' and ((item_6 is null ) or (item_6='IMEI'))";
                        var qry_RangeExt4 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetRangeExt4,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_RangeExt4.Data.Count() > 0)
                        {
                            foreach (var row in qry_RangeExt4.Data)
                            {
                                string strGetRangeExt = $"select * from sfism4.r_mo_ext_t where item_6='{(row["item_1"].ToString().Length + 1).ToString()}' and ('{row["item_1"].ToString()}' between item_1 and item_2) or ('{row["item_2"].ToString()}' between item_1 and item_2)";
                                var qry_RangeExt = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                {
                                    CommandText = strGetRangeExt,
                                    SfcCommandType = SfcCommandType.Text
                                });
                                if (qry_RangeExt.Data == null)
                                {
                                    string strInsertRange = "insert into sfism4.r_mo_ext_t(mo_number,item_1,ver_1,item_2,ver_2,item_3,ver_3,item_4,ver_4,item_5,ver_5,item_6,ver_6)"
                                        + " values"
                                        + " (:mo_number,:item_1,:ver_1,:item_2,:ver_2,:item_3,:ver_3,:item_4,:ver_4,:item_5,:ver_5,:item_6,:ver_6)";
                                    var InsertRange = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                                    {
                                        CommandText = strInsertRange,
                                        SfcCommandType = SfcCommandType.Text,
                                        SfcParameters = new List<SfcParameter>()
                                        {
                                            new SfcParameter{Name="mo_number",Value=Edt_MO.Text},
                                            new SfcParameter{Name="item_1",Value=row["item_1"].ToString()},
                                            new SfcParameter{Name="ver_1",Value=Edt_Version.Text},
                                            new SfcParameter{Name="item_2",Value=row["item_2"].ToString()},
                                            new SfcParameter{Name="ver_2",Value=Edt_Version.Text},
                                            new SfcParameter{Name="item_3",Value=System.DateTime.Now.ToString("yyyyMMddhhmmss")},
                                            new SfcParameter{Name="ver_3",Value=System.DateTime.Now.ToString("yyyyMMddhhmmss")},
                                            new SfcParameter{Name="item_4",Value=Edt_Version.Text},
                                            new SfcParameter{Name="ver_4",Value=Cbb_Model.Text},
                                            new SfcParameter{Name="item_5",Value="0"},
                                            new SfcParameter{Name="ver_5",Value=Cbb_Model.Text},
                                            new SfcParameter{Name="item_6",Value=""},
                                            new SfcParameter{Name="ver_6",Value=(row["item_1"].ToString().Length + 1).ToString()}
                                        }
                                    });
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No Data Range(MO_Ext4) Call IT remove motype(040),Please Check!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }

                if (Model_Type.IndexOf("049") > -1)
                {
                    string strGetRange = $"select * from sfism4.r_mo_ext3_t where mo_number='{Edt_MO.Text}'";
                    var qry_Range = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetRange,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Range.Data != null)
                    {
                        string strGetMOExt = $"select 1 from sfism4.r_mo_ext_t where item_6=:item_6 and (:start_sn between item_1 and item_2) or (:end_sn between iteM_1 and iteM_2)";
                        var qry_MOExt = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetMOExt,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="start_sn",Value=qry_Range.Data["item_1"].ToString()},
                                new SfcParameter{Name="end_sn",Value=qry_Range.Data["item_2"].ToString()},
                                new SfcParameter{Name="item_6",Value=(qry_Range.Data["item_1"].ToString().Length + 1).ToString()}
                            }
                        });
                        if (qry_MOExt.Data == null)
                        {
                            string strInsert_Range = "insert into sfism4.r_mo_ext_t(mo_number,item_1,ver_1,item_2,ver_2,item_3,ver_3,item_4,ver_4,item_5,ver_5,item_6,ver_6)"
                                + " values(:mo_number,:item_1,:ver_1,:item_2,:ver_2,:item_3,:ver_3,:item_4,:ver_4,:item_5,:ver_5,:item_6,:ver_6)";
                            var Insert_Range = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strInsert_Range,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="mo_number",Value=Edt_MO.Text},
                                    new SfcParameter{Name="item_1",Value=qry_Range.Data["item_1"].ToString()},
                                    new SfcParameter{Name="ver_1",Value=Edt_Version.Text},
                                    new SfcParameter{Name="item_2",Value=qry_Range.Data["item_2"].ToString()},
                                    new SfcParameter{Name="ver_2",Value=Edt_Version.Text},
                                    new SfcParameter{Name="item_3",Value=System.DateTime.Now.ToString()},
                                    new SfcParameter{Name="ver_3",Value=System.DateTime.Now.ToString()},
                                    new SfcParameter{Name="item_4",Value=Edt_Version.Text},
                                    new SfcParameter{Name="ver_4",Value=Cbb_Model.Text},
                                    new SfcParameter{Name="item_5",Value="0"},
                                    new SfcParameter{Name="ver_5",Value=Cbb_Model.Text},
                                    new SfcParameter{Name="item_6",Value=""},
                                    new SfcParameter{Name="ver_6",Value=(qry_Range.Data["item_1"].ToString().Length + 1).ToString()}
                                }
                            });
                        }
                    }
                }
                if (Model_Type.IndexOf("050") > -1)
                {
                    string strGetMo = $"select * from sfis1.c_pirelli_stb_t where mo_number='{Edt_MO.Text}'";
                    var qry_Mo = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetMo,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Mo.Data == null)
                    {
                        string strGetDataModel = $"select * from sfis1.c_pirelli_stb_t where model_name='{Cbb_Model.Text}' and mark='2'";
                        var qry_ModelName = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetDataModel,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if (qry_ModelName.Data != null)
                        {
                            string strInsert = "insert into sfis1.c_pirelli_stb_t values(:mo_number,:model_name,:stbkey_name,:stbkey_num,:mark)";
                            var Insert = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strInsert,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="mo_number",Value=Edt_MO.Text},
                                    new SfcParameter{Name="model_name",Value=Cbb_Model.Text},
                                    new SfcParameter{Name="stbkey_name",Value=qry_ModelName.Data["stbkey_name"].ToString()},
                                    new SfcParameter{Name="stbkey_num",Value=Edt_TargetQTY.Text},
                                    new SfcParameter{Name="mark",Value="3"}
                                }
                            });
                        }
                        string strGetStbKey = $"select * from sfis1.c_pirelli_stb_t where stbkey_name='{qry_ModelName.Data["stbkey_name"].ToString()}' and mark='1'";
                        var qry_StbKey = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetStbKey,
                            SfcCommandType = SfcCommandType.Text
                        });
                        Stbkey_num = qry_StbKey.Data["stbkey_num"].ToString();

                        string strGetStbKey_Name = $"select sum(stbkey_num) as aa from sfis1.c_pirelli_stb_t where stbkey_name='{qry_ModelName.Data["stbkey_name"].ToString()}' and mark='2'";
                        var qry_StbKey_Name = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetStbKey_Name,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if ((Int32.Parse(Stbkey_num) - Int32.Parse(qry_StbKey_Name.Data["aa"].ToString())) <= 10000)
                        {
                            string strInsertMail = "Insert into SFIS1.C_MAIL_T (MAIL_ID, MAIL_TO, MAIL_FROM, MAIL_SUBJECT, MAIL_SEQUENCE, MAIL_CONTENT, MAIL_FLAG, MAIL_PROGRAM)"
                                + " Values(:MAIL_ID,:MAIL_TO, :MAIL_FROM, :MAIL_SUBJECT, :MAIL_SEQUENCE, :MAIL_CONTENT, :MAIL_FLAG, :MAIL_PROGRAM)";
                            var InsertMai = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strInsertMail,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="mail_id",Value=System.DateTime.Now.ToString()},
                                    new SfcParameter{Name="mail_to",Value="Carl Y.J. Chan/NSG/FOXCONN, Penny X.P. Pan/NSG/FOXCONN, Whitney Q. Chao/NSG/FOXCONN, Emily F. Yan/NSG/FOXCONN, CPEI-Plan-PP/NSG/FOXCONN"},
                                    new SfcParameter{Name="mail_from",Value="pirelli"},
                                    new SfcParameter{Name="MAIL_SUBJECT",Value="The Pirelli StbKey is not Enough,Please Check it!"},
                                    new SfcParameter{Name="MAIL_SEQUENCE",Value="0"},
                                    new SfcParameter{Name="MAIL_CONTENT",Value="The Pirelli StbKey is not Enough,Please Check it!"},
                                    new SfcParameter{Name="MAIL_FLAG",Value="0"},
                                    new SfcParameter{Name="MAIL_PROGRAM",Value="PM"},
                                }
                            });
                        }
                    }
                    else
                    {
                        string strUpdate = "update sfis1.c_pirelli_stb_t set stbkey_num=:stbkey_num where mo_number=:mo_number'";
                        var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strUpdate,
                            SfcCommandType = SfcCommandType.Text,
                            SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="mo_number",Value=Edt_MO.Text},
                                new SfcParameter{Name="stbkey_num",Value=Edt_TargetQTY.Text}
                            }
                        });

                        string strGetStbkey_Name = $"select * from sfis1.c_pirelli_stb_t where stbkey_name='{qry_Mo.Data["stbkey_name"]}' and mark='1'";
                        var qry_Stbkey_Name = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetStbkey_Name,
                            SfcCommandType = SfcCommandType.Text
                        });
                        Stbkey_num = Edt_TargetQTY.Text;
                        string strGetStbKey_Name = $"select sum(stbkey_num) as aa from sfis1.c_pirelli_stb_t where stbkey_name='{qry_Mo.Data["stbkey_name"].ToString()}' and mark='2'";
                        var qry_StbKey_Name = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strGetStbKey_Name,
                            SfcCommandType = SfcCommandType.Text
                        });
                        if ((Int32.Parse(Stbkey_num) - Int32.Parse(qry_StbKey_Name.Data["aa"].ToString())) <= 10000)
                        {
                            string strInsertMail = "Insert into SFIS1.C_MAIL_T (MAIL_ID, MAIL_TO, MAIL_FROM, MAIL_SUBJECT, MAIL_SEQUENCE, MAIL_CONTENT, MAIL_FLAG, MAIL_PROGRAM)"
                                + " Values(:MAIL_ID,:MAIL_TO, :MAIL_FROM, :MAIL_SUBJECT, :MAIL_SEQUENCE, :MAIL_CONTENT, :MAIL_FLAG, :MAIL_PROGRAM)";
                            var InsertMai = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                            {
                                CommandText = strInsertMail,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="mail_id",Value=System.DateTime.Now.ToString()},
                                    new SfcParameter{Name="mail_to",Value="Carl Y.J. Chan/NSG/FOXCONN, Penny X.P. Pan/NSG/FOXCONN, Whitney Q. Chao/NSG/FOXCONN, Emily F. Yan/NSG/FOXCONN, CPEI-Plan-PP/NSG/FOXCONN"},
                                    new SfcParameter{Name="mail_from",Value="pirelli"},
                                    new SfcParameter{Name="MAIL_SUBJECT",Value="The Pirelli StbKey is not Enough,Please Check it!"},
                                    new SfcParameter{Name="MAIL_SEQUENCE",Value="0"},
                                    new SfcParameter{Name="MAIL_CONTENT",Value="The Pirelli StbKey is not Enough,Please Check it!"},
                                    new SfcParameter{Name="MAIL_FLAG",Value="0"},
                                    new SfcParameter{Name="MAIL_PROGRAM",Value="PM"},
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            string strGetModel_Name = "select * From sfis1.c_model_desc_t where model_name='SFISSITE'";
            var qry_Model_Name = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetModel_Name,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Model_Name.Data != null)
            {
                Model_Serial = qry_Model_Name.Data["model_serial"]?.ToString() ?? "";
                if (Model_Serial == "N")
                {
                    string _strGetModel_Name = "select * from sfis1.c_model_desc_t where model_name=:model_name";
                    var _qry_Model_Name = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = _strGetModel_Name,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                            {
                                new SfcParameter{Name="model_name",Value=Cbb_Model.Text.Trim()}
                            }
                    });
                    if (_qry_Model_Name.Data != null)
                    {
                        Model_Product = _qry_Model_Name.Data["product_name"]?.ToString() ?? "";
                    }
                    Tmp_mo = Edt_MO.Text.Trim();
                    Tmp_mo = Tmp_mo.Substring(0, 1);
                    if (Tmp_mo == "0")
                    {
                        if (!string.IsNullOrEmpty(Cbb_ScrapQty.Text))
                        {
                            for (int i = 0; i < Cbb_ScrapQty.Text.Trim().Length; i++)
                            {
                                //tmp_char = Convert.ToChar("");
                                tmp_char = Cbb_ScrapQty.Text[i];
                            }
                        }
                        if (string.IsNullOrEmpty(Cbb_ScrapQty.Text))
                        {
                            MessageBoxResult result_scrap = MessageBox.Show($"{Edt_MO.Text.Trim()} có bản báo phế hay không ?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result_scrap == MessageBoxResult.Yes)
                            {
                                MessageBox.Show("Chọn số lượng cần báo phế -- Please set scrapt qty", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                                Cbb_ScrapQty.Focus();
                                return;
                            }
                        }
                        else
                        {
                            if ((Int32.Parse(Model_Product)) <= Int32.Parse(Cbb_ScrapQty.Text.Trim()))
                            {
                                MessageBox.Show($"So luong {Cbb_ScrapQty.Text.Trim()} lon hon {Model_Product}");
                                Cbb_ScrapQty.Focus();
                                return;
                            }
                            MessageBoxResult result_scrap = MessageBox.Show($"MO : {Edt_MO.Text.Trim()} số bản trên 1 panel là {Model_Product}, số lượng báo phế {Cbb_ScrapQty.Text.Trim()}. Xác nh?", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result_scrap == MessageBoxResult.No)
                            {
                                Cbb_ScrapQty.Focus();
                                return;
                            }
                        }
                    }
                }
            }
            if (Cbb_SN_USE.IsVisible == true)
            {
                if (Cbb_SN_USE.IsChecked == true)
                {
                    UpdateREuse(Edt_MO.Text.Trim(), "1");
                }
                else
                {
                    UpdateREuse(Edt_MO.Text.Trim(), "");
                }
            }
            if (CheckShipType())
            {
                try
                {
                    string strUpdate = $"update sfism4.r105 set imei_mo_option='{ST}' where mo_number='{Edt_MO.Text}'";
                    var Update = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strUpdate,
                        SfcCommandType = SfcCommandType.Text
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (!string.IsNullOrEmpty(Edt_LinkMO.Text))
            {
                if (await CheckMolink(Edt_LinkMO.Text) == false)
                {
                    MessageBox.Show("T-MO not found,Please check", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (await CheckCpeiii() == true)
            {
                string strGetParameter = "select * from sfis1.c_parameter_ini where vr_item='MO' and prg_name='RMA'";
                var qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetParameter,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_Parameter.Data != null)
                {
                    Rma_mo_pre = qry_Parameter.Data["vr_value"]?.ToString() ?? "";
                    if (Rma_mo_pre.IndexOf(Edt_MO.Text.Trim().Substring(0, 4)) > -1)
                    {
                        MessageBox.Show("Please Set SMT(X5) MO Which Need to LINK With RMA(X8) MO", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_LinkMO.Focus();
                        Edt_LinkMO.SelectAll();
                        return;
                    }
                }
            }
            string _strGetParameter = "select * from sfis1.c_parameter_ini where vr_CLASS=:MO_NUMBER and prg_name='HOLD'";
            var _qry_Parameter = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = _strGetParameter,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text}
                }
            });
            if (_qry_Parameter.Data != null)
            {
                Edt_UPC.Text = "Y";
            }
            string strGetMOPlan = "SELECT * FROM SFISM4.R_BPCS_MOPLAN_T where MO_NUMBER=:MO_NUMBER and SAP_MO_TYPE='ZA13'";
            var qry_MOPlan = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetMOPlan,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text}
                }
            });
            if (qry_MOPlan.Data != null)
            {
                Edt_UPC.Text = "Y";
            }
            if ((Mode == "ADD") && (frm_MO_ManageForm.LOADFLAG == true))
            {
                string strGetMonumber = "SELECT MO_NUMBER FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = :sMO";
                var qry_Monumber = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetMonumber,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="sMO",Value=Edt_MO.Text}
                    }
                });
                if (qry_Monumber.Data.Count() > 0)
                {
                    MessageBox.Show("MO Exist", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO");
                    sb.Append(" SFISM4.R_MO_BASE_T");
                    sb.Append(" (MO_NUMBER,MO_TYPE,VERSION_CODE,TARGET_QTY,MODEL_NAME,");
                    sb.Append(" DEFAULT_LINE,DEFAULT_GROUP,END_GROUP,MO_SCHEDULE_DATE,MO_DUE_DATE,");
                    sb.Append(" START_SN,END_SN,MASTER_FLAG,CLOSE_FLAG,MO_CREATE_DATE,ROUTE_CODE,INPUT_QTY,OUTPUT_QTY,");
                    sb.Append(" BOM_NO,PO_NO,HW_BOM,SW_BOM,UPC_CO,OPTION_DESC,TOTAL_SCRAP_QTY,");
                    sb.Append(" CUST_CODE,CUST_PART_NO,KEY_PART_NO,TURN_OUT_QTY,MO_OPTION,SHIFT,REMARK,CUSTPN,so_number,so_line,");
                    sb.Append(" PO_ITEM,VENDER_PART_NO,PMCC,LOT_FLAG,ORDER_NO,SIGN_EMP,job_mo_option)");
                    sb.Append(" VALUES");
                    sb.Append($"('{Edt_MO.Text}','{Cbb_MoType.Text}','{Edt_Version.Text}','{Edt_TargetQTY.Text}','{Cbb_Model.Text}',");
                    sb.Append($" '{Cbb_Line.Text}','{Cbb_FirstGroup.Text}','{Cbb_LastGroup.Text}',TO_DATE('{Time_PlanInput.Text}', 'yy/MM/dd hh24:mi:ss'),TO_DATE('{Time_PlanFinish.Text}', 'yy/MM/dd hh24:mi:ss'),");
                    if (Rb_Secondary.IsChecked == true)
                    {
                        sb.Append($" 'N/A','N/A','N','1',");
                    }
                    sb.Append($" TO_DATE('{sTime}', 'yy/MM/dd hh24:mi:ss'),'{sRouteCode}','0','0','{Cbb_BOM.Text}',");
                    sb.Append($" '{Edt_PO.Text}','{Edt_HWBOM.Text}','{Edt_SW_BOM.Text}','{Edt_UPC.Text}','{Edt_Option.Text}','0',");
                    sb.Append($" '{sCustCode}','{Edt_CustPNo.Text}','{Cbb_PartNO.Text}','0','LOADFROMBPCS',");
                    sb.Append($" '{Edt_Shift.Text}','{Edt_ReMark.Text}','{Edt_CustPN.Text}','{Edt_SO.Text}','{Edt_SoLine.Text}',");
                    sb.Append($" '{Edt_POITEM.Text}',");
                    if ((Model_Type.IndexOf("189") > -1) || (Model_Type.IndexOf("G55") > -1))
                    {
                        sb.Append($" '{Edt_TA.Text}',");
                    }
                    else
                    {
                        sb.Append($" '{sVenderPN}',");
                    }
                    sb.Append($" '{Edt_PackLot.Text}',");
                    if (Cbb_MOKind.Text == "LOT")
                    {
                        sb.Append($" '{Cbb_MOKind.Text}',");
                    }
                    else
                    {
                        sb.Append($" '',");
                    }
                    sb.Append($" '{Cbb_MAC.Text}','{Cbb_ScrapQty.Text}','{Edt_LinkMO.Text}')");
                    string strInsert = sb.ToString();
                    var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strInsert,
                        SfcCommandType = SfcCommandType.Text
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
                            new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                            new SfcParameter{Name="PRG_NAME",Value="PM"},
                            new SfcParameter{Name="ACTION_TYPE",Value="New"},
                            new SfcParameter{Name="ACTION_DESC",Value= $"MO:{Edt_MO.Text},MODEL_NAME:{Cbb_Model.Text},VER:{Edt_Version.Text},ROUTE: {Cbb_Route.Text}({sRouteCode}),Link TMo: {Edt_LinkMO.Text},Bom: {Cbb_BOM.Text},IP Address:{frm_MO_ManageForm.local_strIP}"}
                        }
                    });
                    if (CheckShipType())
                    {
                        string strUpdate = $"update sfism4.r105 set imei_mo_option='{ST}' where mo_number='{Edt_MO.Text}'";
                        var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strUpdate,
                            SfcCommandType = SfcCommandType.Text
                        });
                    }
                    if (Cbb_SN_USE.IsVisible == true)
                    {
                        if (Cbb_SN_USE.IsChecked == true)
                        {
                            UpdateREuse(Edt_MO.Text.Trim(), "1");
                        }
                        else
                        {
                            UpdateREuse(Edt_MO.Text.Trim(), "");
                        }
                    }
                    FillSectionGroup(sRouteCode);
                    frm_MO_ManageForm.LOADFLAG = false;
                    this.Close();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            if ((Mode != "ADD") && (ads_mo[0].MO_OPTION == "LOADFROMBPCS"))
            {
                Edt_ReMark.Text = frm_MO_ManageForm._EMP;
                string strGetDataMO = $"SELECT* FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '{Edt_MO.Text}'";
                var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetDataMO,
                    SfcCommandType = SfcCommandType.Text
                });
                Inputqty = qry_DataMO.Data["input_qty"].ToString();
                if (Int32.Parse(Inputqty) < (Int32.Parse(Edt_TargetQTY.Text) + 1))
                {
                    try
                    {
                        Time_PlanInput.Text = qry_DataMO.Data["mo_schedule_date"]?.ToString() ?? "";

                        Time_PlanFinish.Text = qry_DataMO.Data["mo_due_date"]?.ToString() ?? "";
                        StringBuilder sb = new StringBuilder();
                        sb.Append("UPDATE");
                        sb.Append(" SFISM4.R_MO_BASE_T");
                        sb.Append($" set MO_NUMBER = '{Edt_MO.Text}',TARGET_QTY = '{Edt_TargetQTY.Text}',MODEL_NAME = '{Cbb_Model.Text}',MO_TYPE = '{Cbb_MoType.Text}',");
                        sb.Append($" VERSION_CODE = '{Edt_Version.Text}',DEFAULT_LINE='{Cbb_Line.Text}',DEFAULT_GROUP='{Cbb_FirstGroup.Text}',END_GROUP = '{Cbb_LastGroup.Text}',");
                        //  sb.Append($" MO_SCHEDULE_DATE=TO_DATE('{Time_PlanInput.Text}', 'yyyy/MM/dd hh24:mi:ss'),MO_DUE_DATE=TO_DATE('{Time_PlanFinish.Text}', 'yyyy/MM/dd hh24:mi:ss'),START_SN='{Edt_SN.Text}',END_SN='{END_SN}',");
                        sb.Append($" START_SN='{Edt_SN.Text}',END_SN='{END_SN}',");
                        sb.Append($" ROUTE_CODE='{sRouteCode}',BOM_NO='{Cbb_BOM.Text}',PO_NO='{Edt_PO.Text}',HW_BOM='{Edt_HWBOM.Text}',SW_BOM='{Edt_SW_BOM.Text}',UPC_CO='{Edt_UPC.Text}',");
                        sb.Append($" OPTION_DESC='{Edt_Option.Text}',CUST_CODE = '{sCustCode}',KEY_PART_NO = '{Cbb_PartNO.Text}',CUST_PART_NO ='{Edt_CustPNo.Text}',SHIFT = '{Edt_Shift.Text}',");
                        sb.Append($" REMARK ='{Edt_ReMark.Text}',CUSTPN ='{Edt_CustPN.Text}',SO_NUMBER='{Edt_SO.Text}',SO_LINE ='{Edt_SoLine.Text}',PO_ITEM = '{Edt_POITEM.Text}',");
                        if (Cbb_MOKind.Text == "LOT")
                        {
                            sb.Append($" LOT_FLAG='{Cbb_MOKind.Text}',");
                        }
                        else
                        {
                            sb.Append($" LOT_FLAG='',");
                        }
                        if ((Model_Type.IndexOf("189") > -1) || (Model_Type.IndexOf("G55") > -1))
                        {
                            sb.Append($" VENDER_PART_NO='{Edt_TA.Text}',");
                        }
                        else
                        {
                            sb.Append($" VENDER_PART_NO='{sVenderPN}',");
                        }
                        sb.Append($" PMCC = '{Edt_PackLot.Text}',SIGN_EMP='{Cbb_ScrapQty.Text}',job_mo_option='{Edt_LinkMO.Text}'  where MO_NUMBER='{Edt_MO.Text}'");
                        string strUpdate = sb.ToString();
                        var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                        {
                            CommandText = strUpdate,
                            SfcCommandType = SfcCommandType.Text
                        });
                        sLog = $"MO : {Edt_MO.Text}";
                        if (ads_mo[0].MO_TYPE != Cbb_MoType.Text)
                        {
                            sLog = sLog + $",MO_TYPE : {Cbb_MoType.Text.Trim()}";
                        }
                        if (ads_mo[0].LOT_FLAG != Cbb_MOKind.Text)
                        {
                            sLog = sLog + $",MO_KIND : {Cbb_MOKind.Text.Trim()}";
                        }
                        if (ads_mo[0].VERSION_CODE != Edt_Version.Text)
                        {
                            sLog = sLog + $",VERSION_CODE : {Edt_Version.Text.Trim()}";
                        }
                        if (ads_mo[0].DEFAULT_LINE != Cbb_Line.Text)
                        {
                            sLog = sLog + $",DEFAULT_LINE : {Cbb_Line.Text.Trim()}";
                        }
                        if (ads_mo[0].DEFAULT_GROUP != Cbb_FirstGroup.Text)
                        {
                            sLog = sLog + $",DEFAULT_GROUP : {Cbb_FirstGroup.Text.Trim()}";
                        }
                        if (ads_mo[0].END_GROUP != Cbb_LastGroup.Text)
                        {
                            sLog = sLog + $",END_GROUP : {Cbb_LastGroup.Text.Trim()}";
                        }
                        if (ads_mo[0].KEY_PART_NO != Cbb_PartNO.Text)
                        {
                            sLog = sLog + $",KEY_PART_NO : {Cbb_PartNO.Text.Trim()}";
                        }
                        if (ads_mo[0].CUST_PART_NO != Edt_CustPNo.Text)
                        {
                            sLog = sLog + $",CUST_PART_NO : {Edt_CustPNo.Text.Trim()}";
                        }
                        if (ads_mo[0].ROUTE_CODE != Cbb_Route.Text)
                        {
                            sLog = sLog + $",ROUTE_CODE : {Cbb_Route.Text.Trim()}({sRouteCode})";
                        }
                        if (ads_mo[0].BOM_NO != Cbb_BOM.Text)
                        {
                            sLog = sLog + $",BOM_NO : {Cbb_BOM.Text.Trim()}";
                        }
                        if (ads_mo[0].CUST_CODE != Cbb_Customer.Text)
                        {
                            sLog = sLog + $",CUST_CODE : {Cbb_Customer.Text.Trim()}";
                        }
                        if (ads_mo[0].PO_NO != Edt_PO.Text)
                        {
                            sLog = sLog + $",PO_NO : {Edt_PO.Text.Trim()}";
                        }
                        if (ads_mo[0].PO_ITEM != Edt_POITEM.Text)
                        {
                            sLog = sLog + $",PO_NO : {Edt_POITEM.Text.Trim()}";
                        }
                        if (ads_mo[0].REMARK != Edt_ReMark.Text)
                        {
                            sLog = sLog + $",REMARK : {Edt_ReMark.Text.Trim()}";
                        }
                        if ((ads_mo[0].SHIFT != Edt_Shift.Text) && (ads_mo[0].SHIFT != null))
                        {
                            sLog = sLog + $",SHIFT : {Edt_Shift.Text.Trim()}";
                        }
                        if ((ads_mo[0].PMCC != Edt_PackLot.Text) && (ads_mo[0].PMCC != null))
                        {
                            sLog = sLog + $",PMCC : {Edt_PackLot.Text.Trim()}";
                        }
                        if (ads_mo[0].SO_LINE != Edt_SoLine.Text)
                        {
                            sLog = sLog + $",SO_LINE : {Edt_SoLine.Text.Trim()}";
                        }
                        if (ads_mo[0].SO_NUMBER != Edt_SO.Text)
                        {
                            sLog = sLog + $",SO_NUMBER : {Edt_SO.Text.Trim()}";
                        }
                        if (ads_mo[0].CUSTPN != Edt_CustPN.Text)
                        {
                            sLog = sLog + $",CUSTPN : {Edt_CustPN.Text.Trim()}";
                        }
                        sLog = sLog.Substring(0, sLog.Length);
                        sLog = sLog.Trim();

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
                                new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                                new SfcParameter{Name="PRG_NAME",Value="PM"},
                                new SfcParameter{Name="ACTION_TYPE",Value="MODIFY"},
                                new SfcParameter{Name="ACTION_DESC",Value=$"{sLog}"}
                            }
                        });
                        if (qry_DataMO.Data["close_flag"].ToString() == "5")
                        {
                            string strUpdateStatusMO = "UPDATE SFISM4.R_MO_BASE_T SET MO_CLOSE_DATE = sysdate,CLOSE_FLAG = :CloseFlag WHERE MO_NUMBER= :MO_NUMBER";
                            var Update_MO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strUpdateStatusMO,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    //new SfcParameter{Name="CloseDate",Value=System.DateTime.Now},
                                    new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text},
                                    new SfcParameter{Name="CloseFlag",Value="2"}
                                }
                            });

                            //------ Save To Log ------//
                            string strInsert_Log2 = "INSERT INTO sfism4.r_system_log_t(EMP_NO, PRG_NAME, ACTION_TYPE, ACTION_DESC) "
                                    + " VALUES ("
                                    + " :EMP_NO, :PRG_NAME, :ACTION_TYPE, :ACTION_DESC)";
                            var Insert_Log2 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                            {
                                CommandText = strInsert_Log2,
                                SfcCommandType = SfcCommandType.Text,
                                SfcParameters = new List<SfcParameter>()
                                {
                                    new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                                    new SfcParameter{Name="PRG_NAME",Value="PM"},
                                    new SfcParameter{Name="ACTION_TYPE",Value="Determined"},
                                    new SfcParameter{Name="ACTION_DESC",Value= $"MO : {Edt_MO.Text}"}
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    FillSectionGroup(sRouteCode);
                    this.Close();
                    return;

                }
                else
                {
                    MessageBox.Show("00110 - So luong nhap khong duoc lon hon so luong cua MO - Put into number do not  can be bigger than a wo to measure", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            if (Mode == "ADD")
            {
                string strGetCheck = "select MO_NUMBER from SFISM4.R_MO_BASE_T where MO_NUMBER=:MoName";
                var qry_Check = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetCheck,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="MoName",Value=Edt_MO.Text}
                    }
                });
                if (qry_Check.Data.Count() > 0)
                {
                    MessageBox.Show($"This MO:{Edt_MO.Text} has exist,please input again!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    Edt_MO.Focus();
                    Edt_MO.SelectAll();
                    return;
                }
            }
            if (Rb_Primary.IsChecked == true)
            {
                string strGetSN = "select mo_number from sfism4.r_mo_base_t where End_sn >=:SN and start_sn <=:SN"
                    + " union select mo_number from sfism4.h_mo_base_t where End_sn >=:SN and start_sn <=:SN";
                var qry_CheckSN = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetSN,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SN",Value=Edt_SN.Text}
                    }
                });
                if (Mode == "ADD")
                {
                    if (qry_CheckSN.Data != null)
                    {
                        MessageBox.Show($"This SN : {Edt_SN.Text} has exist,please input again!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_SN.Focus();
                        Edt_SN.SelectAll();
                        return;
                    }
                }
                else
                {
                    foreach (var row in qry_CheckSN.Data)
                    {
                        if (row["mo_number"].ToString() == Edt_MO.Text)
                        {

                        }
                        else
                        {
                            MessageBox.Show($"This SN : {Edt_SN.Text} has exist,please input again!", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                            Edt_SN.Focus();
                            Edt_SN.SelectAll();
                            return;
                        }
                    }
                }

                sStrTmpHead = Edt_SN.Text.Substring(0, iCurrentNOStartDigit - 1);
                sStrCurrentNO = Edt_SN.Text.Substring(iCurrentNOStartDigit, iCurrentNOLen);
                bFlag = false;
                if ((sCurrentNOType != "decimal") && (sCurrentNOType != "hexadecimal"))
                {
                    if (sCurrentNOType == "all")
                    {
                        for (int i = 0; i < iCurrentNOLen - 1; i++)
                        {
                            bFlag = false;
                            for (int j = 0; j < sAllStrTmp.Length; j++)
                            {
                                if (sStrCurrentNO[iCurrentNOLen - i + 1] == sAllStrTmp[j])
                                {
                                    bFlag = true;
                                    break;
                                }
                            }
                            if (!bFlag)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < iCurrentNOLen - 1; i++)
                        {
                            bFlag = false;
                            for (int j = 0; j < sMultiStr[i - 1].Length; j++)
                            {
                                if (sStrCurrentNO[iCurrentNOLen - i + 1].ToString() == sMultiStr[i - 1].ToString())
                                {
                                    bFlag = true;
                                    break;
                                }
                            }
                            if (!bFlag)
                            {
                                break;
                            }
                        }
                    }
                    if (!bFlag)
                    {
                        sErrMsgBuf = $"Illegal current NO.{sStrCurrentNO},please input again";
                        MessageBox.Show($"{sErrMsgBuf}", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_SN.Focus();
                        Edt_SN.SelectAll();
                        return;
                    }
                }
                sStrTmpTail = Edt_SN.Text.Substring(iCurrentNOStartDigit + iCurrentNOLen - 1, iSNLen - sStrTmpHead.Length - iCurrentNOLen);
                if (sCurrentNOType == "decimal")
                {
                    sStrCurrentNO = string.Format("%." + iCurrentNOLen.ToString() + "d", Int32.Parse(sStrCurrentNO) + Int32.Parse(Edt_TargetQTY.Text) - 1);
                }
                else if (sCurrentNOType == "hexadecimal")
                {
                    sStrCurrentNO = string.Format("%." + iCurrentNOLen.ToString() + "x", Int32.Parse(sStrCurrentNO) + Int32.Parse(Edt_TargetQTY.Text) - 1);
                }
                else if (sCurrentNOType == "all")
                {
                }
                else if (sCurrentNOType == "other")
                {
                }

                if (sStrCurrentNO == "overflow")
                {
                    return;
                }
                else
                {
                    END_SN = sStrTmpHead + sStrCurrentNO + sStrTmpTail;
                }

                if (await CheckSN_MOVE(Edt_SN.Text, END_SN) == true)
                {
                    MessageBox.Show("Some SN has in the range", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string _strGetSN = "select mo_number from sfism4.r_mo_base_t"
                    + " where End_sn >=:SN and start_sn <=:SN"
                    + " union select mo_number from sfism4.h_mo_base_t"
                    + " where End_sn >=:SN and start_sn <=:SN";
                var qry_SN = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = _strGetSN,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="SN",Value=END_SN}
                    }
                });
                if (Mode == "ADD")
                {
                    if (qry_SN.Data != null)
                    {
                        MessageBox.Show($"This End SN:{END_SN} has exist,please input again.", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                        Edt_SN.Focus();
                        Edt_SN.SelectAll();
                        return;
                    }
                }
                else
                {
                    if (qry_SN.Data != null)
                    {
                        if (qry_SN.Data["mo_number"].ToString() == Edt_MO.Text)
                        {
                        }
                        else
                        {
                            MessageBox.Show($"This End SN:{Edt_SN.Text} has exist,please input again.", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                            Edt_SN.Focus();
                            Edt_SN.SelectAll();
                            return;
                        }
                    }
                }
            }
            var qry_Route_Code = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRouteCode,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="RouteName",Value=Cbb_Route.Text}
                }
            });
            sRouteCode = qry_Route_Code.Data["route_code"].ToString();
            string _strGetCustCode = "select CUSTOMER, CUST_CODE from SFIS1.C_CUSTOMER_T where CUSTOMER =:Customer order by CUSTOMER";
            var _qry_CustCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = _strGetCustCode,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="Customer",Value=Cbb_Customer.Text}
                }
            });
            sCustCode = _qry_CustCode.Data["cust_code"].ToString();

            if (Mode == "ADD")
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO");
                    sb.Append(" SFISM4.R_MO_BASE_T");
                    sb.Append(" (MO_NUMBER,MO_TYPE,VERSION_CODE,TARGET_QTY,MODEL_NAME,");
                    sb.Append(" DEFAULT_LINE,DEFAULT_GROUP,END_GROUP,MO_SCHEDULE_DATE,MO_DUE_DATE,");
                    sb.Append(" START_SN,END_SN,MASTER_FLAG,CLOSE_FLAG,MO_CREATE_DATE,ROUTE_CODE,INPUT_QTY,OUTPUT_QTY,");
                    sb.Append(" BOM_NO,PO_NO,HW_BOM,SW_BOM,UPC_CO,OPTION_DESC,TOTAL_SCRAP_QTY,");
                    sb.Append(" CUST_CODE,KEY_PART_NO,CUST_PART_NO,TURN_OUT_QTY,MO_OPTION,SHIFT,REMARK,CUSTPN,so_number,so_line,");
                    sb.Append(" PO_ITEM,VENDER_PART_NO,PMCC,LOT_FLAG,ORDER_NO,SIGN_EMP,job_mo_option)");
                    sb.Append(" VALUES");
                    sb.Append($"('{Edt_MO.Text}','{Cbb_MoType.Text}','{Edt_Version.Text}','{Edt_TargetQTY.Text}','{Cbb_Model.Text}',");
                    sb.Append($" '{Cbb_Line.Text}','{Cbb_FirstGroup.Text}','{Cbb_LastGroup.Text}',TO_DATE('{Time_PlanInput.Text}', 'yy/MM/dd hh24:mi:ss'),TO_DATE('{Time_PlanFinish.Text}', 'yy/MM/dd hh24:mi:ss'),");
                    if (Rb_Primary.IsChecked == true)
                    {
                        sb.Append($" '{Edt_SN.Text}','{END_SN}','Y','1',");
                    }
                    else
                    {
                        sb.Append($" 'N/A','N/A','N','2',");
                    }
                    sb.Append($" TO_DATE('{sTime}', 'yy/mm/dd hh24:mi:ss'),'{sRouteCode}','0','0','{Cbb_BOM.Text}',");
                    sb.Append($" '{Edt_PO.Text}','{Edt_HWBOM.Text}','{Edt_SW_BOM.Text}','{Edt_UPC.Text}','{Edt_Option.Text}','0',");
                    sb.Append($" '{sCustCode}','{Cbb_PartNO.Text}','0','{Time_PlanInput.Text}','LOADFROMBPCS',");
                    sb.Append($" '{Edt_Shift.Text}','{Edt_ReMark.Text}','{Edt_CustPN.Text}','{Edt_SO.Text}','{Edt_SoLine.Text}',");
                    sb.Append($" '{Edt_POITEM.Text}',");
                    sb.Append($" '{sVenderPN}',");
                    sb.Append($" '{Edt_PackLot.Text}',");
                    if (Cbb_MOKind.Text == "LOT")
                    {
                        sb.Append($" '{Cbb_MOKind.Text}',");
                    }
                    else
                    {
                        sb.Append($" '',");
                    }
                    sb.Append($" '{Cbb_MAC.Text}','{Cbb_ScrapQty.Text}','{Edt_LinkMO.Text}'");
                    string strInsert = sb.ToString();
                    var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strInsert,
                        SfcCommandType = SfcCommandType.Text
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
                            new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                            new SfcParameter{Name="PRG_NAME",Value="PM"},
                            new SfcParameter{Name="ACTION_TYPE",Value="New"},
                            new SfcParameter{Name="ACTION_DESC",Value= $"MO:{Edt_MO.Text},VER:{Edt_Version.Text},ROUTE: {Cbb_Route.Text},Link TMo: {Edt_LinkMO.Text},Bom: {Cbb_BOM.Text},IP Address:{frm_MO_ManageForm.local_strIP}"}
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE");
                    sb.Append(" SFISM4.R_MO_BASE_T");
                    sb.Append($" set MO_NUMBER = '{Edt_MO.Text}',TARGET_QTY = '{Edt_TargetQTY.Text}',MODEL_NAME = '{Cbb_Model.Text}',MO_TYPE = '{Cbb_MoType.Text}',");
                    sb.Append($" VERSION_CODE = '{Edt_Version.Text}',DEFAULT_LINE='{Cbb_Line.Text}',DEFAULT_GROUP='{Cbb_FirstGroup.Text}',END_GROUP = '{Cbb_LastGroup.Text}',");
                    //sb.Append($" c=TO_DATE('{Time_PlanInput.Text}', 'yyyy/MM/dd hh24:mi:ss'),MO_DUE_DATE=TO_DATE('{Time_PlanFinish.Text}', 'yyyy/MM/dd hh24:mi:ss'),START_SN='{Edt_SN.Text}',END_SN='{END_SN}',");
                    sb.Append($"START_SN='{Edt_SN.Text}',END_SN='{END_SN}',");
                    sb.Append($" ROUTE_CODE='{sRouteCode}',BOM_NO='{Cbb_BOM.Text}',PO_NO='{Edt_PO.Text}',HW_BOM='{Edt_HWBOM.Text}',SW_BOM='{Edt_SW_BOM.Text}',UPC_CO='{Edt_UPC.Text}',");
                    sb.Append($" OPTION_DESC='{Edt_Option.Text}',CUST_CODE = '{sCustCode}',KEY_PART_NO = '{Cbb_PartNO.Text}',CUST_PART_NO ='{Edt_CustPNo.Text}',SHIFT = '{Edt_Shift.Text}',");
                    sb.Append($" REMARK ='{Edt_ReMark.Text}',CUSTPN ='{Edt_CustPN.Text}',SO_NUMBER='{Edt_SO.Text}',SO_LINE ='{Edt_SoLine.Text}',PO_ITEM = '{Edt_POITEM.Text}',");
                    if (Cbb_MOKind.Text == "LOT")
                    {
                        sb.Append($" LOT_FLAG='{Cbb_MOKind.Text}',");
                    }
                    else
                    {
                        sb.Append($" LOT_FLAG='',");
                    }
                    sb.Append($" VENDER_PART_NO='{sVenderPN}',");
                    sb.Append($" PMCC = '{Edt_PackLot.Text}',SIGN_EMP='{Cbb_ScrapQty.Text}',job_mo_option='{Edt_LinkMO.Text}'  where MO_NUMBER='{Edt_MO.Text}'");
                    string strUpdate = sb.ToString();
                    var Update = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strUpdate,
                        SfcCommandType = SfcCommandType.Text
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
                            new SfcParameter{Name="EMP_NO",Value=frm_MO_ManageForm._EMP},
                            new SfcParameter{Name="PRG_NAME",Value="PM"},
                            new SfcParameter{Name="ACTION_TYPE",Value="MODIFY"},
                            new SfcParameter{Name="ACTION_DESC",Value=$"{Edt_MO.Text}"}
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            if (Model_Type.IndexOf("189") > -1)
            {
                string strChkConfig20 = $"SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='PM' AND VR_NAME='CHKCONFIG20' AND VR_VALUE='TRUE'";
                var qry_ChkConfig20 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = strChkConfig20,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ChkConfig20.Data.Count() == 0)
                {
                    string strGetCustSNRule = $"SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME='{Cbb_Model.Text}' and version_code='{Edt_Version.Text}'";
                    var qry_CustSNRule = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetCustSNRule,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if ((qry_CustSNRule.Data == null) || (qry_CustSNRule.Data["upceandata"].ToString() == ""))
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00084 - " + await GetPubMessage("00084");
                        _er.MessageVietNam = "00084 - " + await GetPubMessage("00084");
                        _er.ShowDialog();
                        return;
                    }
                }
                else
                {
                    string strGetConfig20 = $"SELECT * FROM sfis1.c_model_confirm_t WHERE MODEL_TYPE='{Edt_MO.Text}' and TYPE_NAME='NPIMOCONFIG'";
                    var qry_Config20 = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = strGetConfig20,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Config20.Data.Count() == 0)
                    {
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "PM no setup PO in  Config20";
                        _er.MessageVietNam = "PM chua setup PO trong Config20";
                        _er.ShowDialog();
                        return;
                    }
                }
            }
            FillSectionGroup(sRouteCode);
            this.Close();
            return;
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void Route_Changed(object sender, EventArgs e)
        {
            //Cbb_Route.Text = (sender as ComboBox).SelectedItem as string;
            string temp = Cbb_Route.Text;
            string strGetRouteCode = $"SELECT ROUTE_CODE FROM SFIS1.C_ROUTE_NAME_T where route_name = '{Cbb_Route.Text}'";
            var qry_RouteCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRouteCode,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_RouteCode.Data != null)
            {
                string strGetFirstStation = $"SELECT GROUP_NEXT FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE='{qry_RouteCode.Data["route_code"].ToString()}' AND STEP_SEQUENCE=1";
                var qry_FirstStation = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetFirstStation,
                    SfcCommandType = SfcCommandType.Text
                });
                Cbb_FirstGroup.Text = qry_FirstStation.Data["group_next"]?.ToString() ?? "";

                string strGetLastStation = $"SELECT GROUP_NEXT FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE='{qry_RouteCode.Data["route_code"].ToString()}' AND STATE_FLAG=0"
                    + $" AND STEP_SEQUENCE=(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE='{qry_RouteCode.Data["route_code"].ToString()}' AND STATE_FLAG=0 AND SUBSTR(GROUP_NAME,1,2)<>'R_')";
                var qry_LastStation = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetLastStation,
                    SfcCommandType = SfcCommandType.Text
                });
                Cbb_LastGroup.Text = qry_LastStation.Data["group_next"]?.ToString() ?? "";
            }
        }

        private void Edt_TargetQTY_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                // Ki?m tra xem gia tr? m?i trong TextBox co ph?i la m?t s? hay khong
                if (!int.TryParse(textBox.Text, out _))
                {
                    // N?u khong ph?i la s?, xoa gia tr? ?o kh?i TextBox
                    textBox.Text = "";
                }
            }
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            string strGetType = "select OQA_TYPE from SFIS1.C_OQA_SAMPLING_PLAN group by OQA_TYPE order by OQA_TYPE";
            var qry_Type = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetType,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_MoType.Items.Clear();
            if (qry_Type.Data != null)
            {
                foreach (var row in qry_Type.Data)
                {
                    Cbb_MO.Items.Add(row["oqa_type"]);
                }
            }
            Cbb_MoType.Text = "";

            string strGetLine = "select LINE_NAME from SFIS1.C_LINE_DESC_T ORDER BY LINE_NAME";
            var qry_Line = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetLine,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_Line.Items.Clear();
            if (qry_Line.Data != null)
            {
                foreach (var row in qry_Line.Data)
                {
                    Cbb_Line.Items.Add(row["line_name"]);
                }
                Cbb_Line.Text = Cbb_Line.Items[0].ToString();
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
                    new SfcParameter{ Name="CloseFlag",Value=frm_MO_ManageForm.MO_Status},
                    new SfcParameter{ Name="MO_NUMBER",Value=frm_MO_ManageForm.Edt_MO.Text}
                }
            });
            if (qry_DataMO.Data != null)
            {
                foreach (var row in qry_DataMO.Data)
                {
                    Cbb_Model.Items.Add(row["model_name"]);
                }
                Cbb_Model.Text = Cbb_Model.Items[0].ToString();
            }

            string strGetRoute = "SELECT ROUTE_CODE,ROUTE_NAME FROM SFIS1.C_ROUTE_NAME_T ORDER BY ROUTE_NAME";
            var qry_Route = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRoute,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_Route.Items.Clear();
            if (qry_Route.Data != null)
            {
                foreach (var row in qry_Route.Data)
                {
                    Cbb_Route.Items.Add(row["route_name"]);
                }
                Cbb_Route.Text = Cbb_Route.Items[0].ToString();
            }

            string strGetBOM = "SELECT BOM_NO FROM SFIS1.C_BOM_KEYPART_T GROUP BY BOM_NO order by BOM_NO";
            var qry_BOM = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetBOM,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_BOM.Items.Clear();
            if (qry_BOM.Data != null)
            {
                foreach (var row in qry_BOM.Data)
                {
                    Cbb_BOM.Items.Add(row["bom_no"]);
                }
                Cbb_BOM.Items.Add("");
                Cbb_BOM.Text = "";
            }

            string strGetCustomer = "select CUSTOMER from SFIS1.C_CUSTOMER_T order by CUSTOMER";
            var qry_Customer = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustomer,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_Customer.Items.Clear();
            if (qry_Customer.Data != null)
            {
                foreach (var row in qry_Customer.Data)
                {
                    if (row["customer"].ToString() != "ALL")
                    {
                        Cbb_Customer.Items.Add(row["customer"]);
                    }
                }
                Cbb_Customer.Text = Cbb_Customer.Items[0].ToString();
            }

            string strGetPartNO = "select KEY_PART_NO from SFIS1.C_KEYPARTS_DESC_T group by KEY_PART_NO order by KEY_PART_NO";
            var qry_PartNO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetPartNO,
                SfcCommandType = SfcCommandType.Text
            });
            Cbb_PartNO.Items.Clear();
            if (qry_PartNO.Data != null)
            {
                foreach (var row in qry_PartNO.Data)
                {
                    Cbb_PartNO.Items.Add(row["key_part_no"]);
                }
                Cbb_PartNO.Text = "";
            }
        }
        private async void btn_New_Click(object sender, RoutedEventArgs e)
        {
            POForm frm_POForm = new POForm();
            frm_POForm.Edt_PO.Text = "";
            frm_POForm.Edt_Customer.Text = "";
            frm_POForm.Edt_QTY.Text = "";
            frm_POForm.ShowDialog();
            try
            {
                string strInsert = "INSERT INTO SFISM4.R_PO_T (MO_NUMBER,PO_NUMBER,CUSTOMER,QTY,PO_COUNT)"
                + " VALUES (:MO_NUMBER,:PO_NUMBER,:CUSTOMER,:QTY,0)";
                var Insert = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strInsert,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text},
                    new SfcParameter{Name="PO_NUMBER",Value=frm_POForm.Edt_PO.Text},
                    new SfcParameter{Name="CUSTOMER",Value=frm_POForm.Edt_Customer.Text},
                    new SfcParameter{Name="QTY",Value=frm_POForm.Edt_QTY.Text},
                }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private async void btn_Modify_Click(object sender, RoutedEventArgs e)
        {
            POForm frm_POForm = new POForm();
            string strGetPO = "select PO_NUMBER,CUSTOMER,QTY from sfism4.R_PO_T where mo_number=:MO_NUMBER";
            var qry_PO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetPO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text}
                }
            });
            frm_POForm.Edt_PO.Text = qry_PO.Data["po_number"].ToString();
            frm_POForm.Edt_Customer.Text = qry_PO.Data["customer"].ToString();
            frm_POForm.Edt_QTY.Text = qry_PO.Data["qty"].ToString();
            frm_POForm.ShowDialog();

            try
            {
                string strUpdate = "UPDATE SFISM4.R_PO_T SET PO_NUMBER=:PO,CUSTOMER=:CUS,QTY=:COUNT WHERE MO_NUMBER = :MO_NUMBER AND PO_NUMBER = :PO_NUMBER";
                var Update = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strUpdate,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="PO",Value=frm_POForm.Edt_PO.Text},
                    new SfcParameter{Name="CUS",Value=frm_POForm.Edt_Customer.Text},
                    new SfcParameter{Name="COUNT",Value=frm_POForm.Edt_QTY.Text},
                    new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text},
                }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private async void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            POForm frm_POForm = new POForm();
            try
            {
                string strDelete = "DELETE FROM SFISM4.R_PO_T WHERE MO_NUMBER=:MO_NUMBER AND PO_NUMBER=:PO_NUMBER AND CUSTOMER=:CUSTOMER";
                var Delete = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strDelete,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="PO",Value=frm_POForm.Edt_PO.Text},
                        new SfcParameter{Name="CUS",Value=frm_POForm.Edt_Customer.Text},
                        new SfcParameter{Name="MO_NUMBER",Value=Edt_MO.Text},
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        private async void MO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string strGetMOData = "select * from SFIS1.C_MO_T where MO = :sMO";
                var qry_MOData = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
                {
                    CommandText = strGetMOData,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="sMO",Value=Edt_MO.Text}
                    }
                });
                Edt_TargetQTY.Text = qry_MOData.Data["qty"].ToString();
                Cbb_Model.Text = qry_MOData.Data["model_name"].ToString();
                Cbb_PartNO.Text = qry_MOData.Data["product"].ToString();
            }
        }
        private void StartSN_KeyDown(object sender, KeyEventArgs e)
        {
            DefaultSNForm frm_DefaultSNForm = new DefaultSNForm();
            frm_DefaultSNForm.ShowDialog();
        }
        private async void MoOption_Changed(object sender, SelectionChangedEventArgs e)
        {
            iSNLen = await GetIniIntValue("PM", Cbb_MO.Text, "LENGTH", "SN", 14);
            iCurrentNOLen = await GetIniIntValue("PM", Cbb_MO.Text, "LENGTH", "CurrentNO", 5);
            iCurrentNOStartDigit = await GetIniIntValue("PM", Cbb_MO.Text, "LENGTH", "CurrentNOStartDigit", (iSNLen - iCurrentNOLen + 1));
            iMOLen = await GetIniIntValue("PM", Cbb_MO.Text, "LENGTH", "MO", 6);
            bSNFixFlag = await GetIniBoolValue("PM", Cbb_MO.Text, "SN", "Fixed_LENGTH", true);
            bMOFixFlag = await GetIniBoolValue("PM", Cbb_MO.Text, "MO", "Fixed_LENGTH", true);

            FieldFlag[6] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "Bom NO", false);
            FieldFlag[0] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "HW Bom", false);
            FieldFlag[1] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "Option", false);
            FieldFlag[2] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "PO", false);
            FieldFlag[3] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "SW Bom", false);
            FieldFlag[4] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "UPC", false);
            FieldFlag[5] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "Version", false);
            FieldFlag[7] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "MO Type", false);
            FieldFlag[8] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "Part NO", false);
            FieldFlag[9] = await GetIniBoolValue("PM", Cbb_MO.Text, "Field", "PMCC", false);

            if (await GetIniStrValue("PM", Cbb_MO.Text, "Type", "CurrentNO", "Numerals") == "Numerals")
            {
                if (await GetIniStrValue("PM", Cbb_MO.Text, "Type", "Decimal", "decimal") == "decimal")
                {
                    sCurrentNOType = "decimal";
                }
                else
                {
                    if (await GetIniStrValue("PM", Cbb_MO.Text, "Type", "Decimal", "decimal") == "hexadecimal")
                    {
                        sCurrentNOType = "hexadecimal";
                    }
                }
            }
            else
            {
                if (await GetIniBoolValue("PM", Cbb_MO.Text, "Flag", "ALLNumber", true))
                {
                    sCurrentNOType = "all";
                    sAllStrTmp = await GetIniStrValue("PM", Cbb_MO.Text, "Number", "ALL", "0123456789");
                }
                else
                {
                    sCurrentNOType = "other";
                    for (int j = 0; j < iCurrentNOLen; j++)
                    {
                        sMultiStr[j - 1] = await GetIniStrValue("PM", Cbb_MO.Text, "Number", j.ToString(), "0123456789");
                    }
                }
            }

            for (int j = 0; j < 9; j++)
            {
                switch (j)
                {
                    case 0:
                        if (FieldFlag[j])
                        {
                            Edt_HWBOM.Background = Brushes.Yellow;
                        }
                        else
                        {
                            Edt_HWBOM.Background = Brushes.White;
                        }
                        break;
                    case 1:
                        if (FieldFlag[j])
                        {
                            Edt_Option.Background = Brushes.Yellow;
                        }
                        else
                        {
                            Edt_Option.Background = Brushes.White;
                        }
                        break;
                    case 2:
                        if (FieldFlag[j])
                        {
                            Edt_FormAddPO.Background = Brushes.Yellow;
                        }
                        else
                        {
                            Edt_FormAddPO.Background = Brushes.White;
                        }
                        break;
                    case 3:
                        if (FieldFlag[j])
                        {
                            Edt_SWBOM.Background = Brushes.Yellow;
                        }
                        else
                        {
                            Edt_SWBOM.Background = Brushes.White;
                        }
                        break;
                    case 4:
                        if (FieldFlag[j])
                        {
                            Edt_UPC.Background = Brushes.Yellow;
                        }
                        else
                        {
                            Edt_UPC.Background = Brushes.White;
                        }
                        break;
                    case 5:
                        if (FieldFlag[j])
                        {
                            Edt_Version.Background = Brushes.Yellow;
                            Lb_Version.Background = Brushes.Black;
                        }
                        else
                        {
                            Edt_Version.Background = Brushes.White;
                            Lb_Version.Background = (Brush)new BrushConverter().ConvertFromString("#008082");
                        }
                        break;
                    case 6:
                        if (FieldFlag[j])
                        {
                            Cbb_BOM.Background = Brushes.Yellow;
                            Lb_BOM.Background = Brushes.Black;
                        }
                        else
                        {
                            Lb_BOM.Background = (Brush)new BrushConverter().ConvertFromString("#008082");
                            Cbb_BOM.Background = Brushes.White;
                        }
                        break;
                    case 7:
                        if (FieldFlag[j])
                        {
                            Cbb_MoType.Background = Brushes.Yellow;
                            Lb_MOType.Background = Brushes.Black;
                        }
                        else
                        {
                            Lb_MOType.Background = (Brush)new BrushConverter().ConvertFromString("#008082");
                            Cbb_MoType.Background = Brushes.White;
                        }
                        break;
                    case 8:
                        if (FieldFlag[j])
                        {
                            Cbb_PartNO.Background = Brushes.Yellow;
                            Lb_PartNO.Background = Brushes.Black;
                        }
                        else
                        {
                            Lb_PartNO.Background = (Brush)new BrushConverter().ConvertFromString("#008082");
                            Cbb_PartNO.Background = Brushes.White;
                        }
                        break;
                }
            }
        }
        private async Task<int> GetIniIntValue(string _sPrg, string _sVClass, string _sVItem, string _sVName, int _iNum)
        {
            string strGetIniIntValue = $"SELECT VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = '{_sPrg}'"
                + $" AND VR_CLASS = '{_sVClass}' AND VR_ITEM = '{_sVItem}' AND VR_NAME = '{_sVName}'";
            var qry_IniIntValue = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetIniIntValue,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_IniIntValue.Data == null)
            {
                return _iNum;
            }
            else
            {
                return Int32.Parse(qry_IniIntValue.Data["vr_value"].ToString());
            }
        }
        private async Task<bool> GetIniBoolValue(string _sPrg, string _sVClass, string _sVItem, string _sVName, bool _bNum)
        {
            string strGetIniBoolValue = $"SELECT VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = '{_sPrg}'"
                + $" AND VR_CLASS = '{_sVClass}' AND VR_ITEM = '{_sVItem}' AND VR_NAME = '{_sVName}'";
            var qry_IniBoolValue = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetIniBoolValue,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_IniBoolValue.Data == null)
            {
                return _bNum;
            }
            else
            {
                if (qry_IniBoolValue.Data["vr_value"].ToString() == "TRUE")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private async Task<string> GetIniStrValue(string _sPrg, string _sVClass, string _sVItem, string _sVName, string _sNum)
        {
            string strGetIniStrValue = $"SELECT VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = '{_sPrg}'"
                + $" AND VR_CLASS = '{_sVClass}' AND VR_ITEM = '{_sVItem}' AND VR_NAME = '{_sVName}'";
            var qry_IniStrValue = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetIniStrValue,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_IniStrValue.Data == null)
            {
                return _sNum;
            }
            else
            {
                return qry_IniStrValue.Data["vr_value"].ToString();
            }
        }
        private async void Route_Changed(object sender, SelectionChangedEventArgs e)
        {
            //Cbb_Route.Text = (sender as ComboBox).SelectedItem as string;
            string temp = Cbb_Route.Text;
            string strGetRouteCode = $"SELECT ROUTE_CODE FROM SFIS1.C_ROUTE_NAME_T where route_name = '{Cbb_Route.Text}'";
            var qry_RouteCode = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetRouteCode,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_RouteCode.Data != null)
            {
                string strGetFirstStation = $"SELECT GROUP_NEXT FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE='{qry_RouteCode.Data["route_code"].ToString()}' AND STEP_SEQUENCE=1";
                var qry_FirstStation = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetFirstStation,
                    SfcCommandType = SfcCommandType.Text
                });
                Cbb_FirstGroup.Text = qry_FirstStation.Data["group_next"]?.ToString() ?? "";

                string strGetLastStation = $"SELECT GROUP_NEXT FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE='{qry_RouteCode.Data["route_code"].ToString()}' AND STATE_FLAG=0"
                    + $" AND STEP_SEQUENCE=(SELECT MAX(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE='{qry_RouteCode.Data["route_code"].ToString()}' AND STATE_FLAG=0 AND SUBSTR(GROUP_NAME,1,2)<>'R_')";
                var qry_LastStation = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetLastStation,
                    SfcCommandType = SfcCommandType.Text
                });
                Cbb_LastGroup.Text = qry_LastStation.Data["group_next"]?.ToString() ?? "";
            }
        }
        private async void ShipTo_Changed(object sender, SelectionChangedEventArgs e)
        {
            string tmpstr;
            if (Cbb_ShipTo.Text.Trim() != "N/A")
            {
                string strGetShipADD = $"SELECT SHIP_ADDRESS FROM SFIS1.C_SHIP_ADDR_T WHERE SHIP_INDEX='{Cbb_ShipTo.Text}'";
                var qry_ShipADD = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetShipADD,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_ShipADD.Data != null)
                {
                    tmpstr = qry_ShipADD.Data["ship_address"].ToString();
                }
                else
                {
                    MessageBox.Show("Khong tim thay dia chi -- Data not found", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Lb_ShipTo.Visibility = Visibility.Visible;
                Lb_ShipTo.Content = tmpstr;
            }
            else
            {
                Lb_ShipTo.Visibility = Visibility.Hidden;
            }
            Edt_Option.Text = Cbb_ShipTo.Text;
        }
        private void PO_Changed(object sender, RoutedEventArgs e)
        {
            if (Edt_PO.Text.Length > 12)
            {
                MessageBox.Show($"FW:{Edt_PO.Text} vuot qua 12 ki tu", "PM", MessageBoxButton.OK, MessageBoxImage.Error);
                Edt_PO.Text = "";
            }
        }
        private void Primary_Checked(object sender, RoutedEventArgs e)
        {
            Edt_SN.IsEnabled = true;
            Lb_StartSN.IsEnabled = true;
        }
        private void Secondary_Checked(object sender, RoutedEventArgs e)
        {
            Edt_SN.IsEnabled = false;
            Lb_StartSN.IsEnabled = false;
        }
        private void Btn_Route_Click(object sender, RoutedEventArgs e)
        {
            RouteGraphForm frm_RouteGraphForm = new RouteGraphForm(this, sfcClient);
            frm_RouteGraphForm.ShowDialog();
        }
        private void btn_BOM_Click(object sender, RoutedEventArgs e)
        {
            CheckBOMForm frm_CheckBOMForm = new CheckBOMForm(this, sfcClient);
            frm_CheckBOMForm.ShowDialog();
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
    }
}
