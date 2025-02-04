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
using System.Collections.ObjectModel;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PM.Model;
using System.Collections;
using System.Data;
using Microsoft.Win32;
using System.Reflection;

namespace PM
{
    /// <summary>
    /// Interaction logic for LoadMOForm.xaml
    /// </summary>
    public partial class LoadMOForm : Window
    {
        public SfcHttpClient sfcClient;
        public MO_ManageForm frm_MO_ManageForm;
        public MainWindow frm_main;
        List<STEP> list_Status = new List<STEP>();
        public ObservableCollection<LoadMO> _MO { get; set; }
        public bool Search;
        public LoadMOForm()
        {
            InitializeComponent();
        }
        public LoadMOForm(MO_ManageForm _frm_MO_ManageForm, SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            frm_MO_ManageForm = _frm_MO_ManageForm;
            FormShow_CheckPrivilege();
            LoadMOForm_FormShow();
            ReasonMO();
            InitializeComponent();
            Edt_SearchMO.Focus();
        }
        public static ObservableCollection<LoadMO> Convert_Reason(IEnumerable original)
        {
            return new ObservableCollection<LoadMO>(original.Cast<LoadMO>());
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
                    new SfcParameter{Name="sUser",Value=frm_MO_ManageForm._EMP_PASS}
                }
            });
            List<ListPrivilege> result = qry_privilege.Data.ToListObject<ListPrivilege>().ToList();
            for (int i = 0; i < qry_privilege.Data.Count(); i++)
            {
                if (result[i].FUN == "MO_BPCS_SAVE")
                {
                    if (result[i].PRIVILEGE == 0)
                    {
                        btnPrint.IsEnabled = false;
                    }
                    else
                    {
                        btnPrint.IsEnabled = true;
                    }
                }
            }
        }
        private void LoadMOForm_FormShow()
        {
            Search = false;
            GetListMO();
            GetCount();
        }
        private async void GetListMO()
        {
            string strGetCount = "SELECT B.*,C.*,D.ROUTE_NAME from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C,SFIS1.C_ROUTE_NAME_T D, SFIS1.C_CUSTOMER_T E"
                    + " WHERE B.MODEL_NAME=C.MODEL_NAME AND C.ROUTE_CODE=D.ROUTE_CODE and b.cust_code=e.cust_code(+)"
                    + " AND (B.MO_NUMBER NOT IN("
                    + " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A)) AND ROWNUM < 50";
            var qry_Count = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCount,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Count.Data.Count() == 0)
            {
                btnPrint.IsEnabled = false;
            }
            else
            {
                _MO = Convert_Reason(qry_Count.Data.ToListObject<LoadMO>().ToList());
                Lv_DataMO.ItemsSource = _MO;
            }
        }
        private async void GetCount()
        {
            string strGetCount = "SELECT B.*,C.*,D.ROUTE_NAME from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C,SFIS1.C_ROUTE_NAME_T D, SFIS1.C_CUSTOMER_T E"
                    + " WHERE B.MODEL_NAME=C.MODEL_NAME AND C.ROUTE_CODE=D.ROUTE_CODE and b.cust_code=e.cust_code(+)"
                    + " AND (B.MO_NUMBER NOT IN("
                    + " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A))";
            var qry_Count = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCount,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Count.Data.Count() == 0)
            {
                btnPrint.IsEnabled = false;
            }
            else
            {
                Lb_Count.Content = "Total Record : " + qry_Count.Data.Count().ToString();
            }
        }
        private async void ReasonMO()
        {
            //string strGetReasonMO = "select mo_number from SFISM4.R_BPCS_MOPLAN_T"
            //    + " where mo_number not in ("
            //    + " SELECT B.mo_number from"
            //    + " SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T  C,SFIS1.C_ROUTE_NAME_T D"
            //    + " ,SFIS1.C_CUSTOMER_T E"
            //    + " WHERE B.MODEL_NAME=C.MODEL_NAME"
            //    + " AND C.ROUTE_CODE=D.ROUTE_CODE and b.cust_code=e.cust_code(+)"
            //    + " AND (B.MO_NUMBER NOT IN (SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A)))"
            //    + " and mo_number not in (SELECT MO_NUMBER FROM SFISM4.R_MO_BASE_T ) AND ROWNUM < 50"
            //    + " order by mo_number";
            string strGetReasonMO = "select mo_number from SFISM4.R_BPCS_MOPLAN_T"
                    + " where mo_number not in ("
                    + " SELECT B.mo_number from"
                    + " SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T  C"
                    + " ,SFIS1.C_CUSTOMER_T E"
                    + " WHERE B.MODEL_NAME=C.MODEL_NAME"
                    + " AND b.cust_code=e.cust_code(+)"
                    + " AND (B.MO_NUMBER NOT IN (SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A)))"
                    + " and mo_number not in (SELECT MO_NUMBER FROM SFISM4.R_MO_BASE_T ) AND ROWNUM < 50"
                    + " order by mo_number";
            var qry_ReasonMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetReasonMO,
                SfcCommandType = SfcCommandType.Text
            });
            Lv_ReasonMO.ItemsSource = null;
            list_Status.Clear();
            if (qry_ReasonMO.Data.Count() > 0)
            {
                for (int i = 0; i < qry_ReasonMO.Data.Count(); i++)
                {
                    List<ListMO> result = qry_ReasonMO.Data.ToListObject<ListMO>().ToList();
                    list_Status.Add(new STEP { MO_NUMBER = "TESTING" });
                    list_Status[i].MO_NUMBER = result[i].MO_NUMBER;
                    list_Status[i].REASON = await ChkStatus(result[i].MO_NUMBER);
                }
            }
            Lv_ReasonMO.ItemsSource = list_Status;
        }
        private async Task<String> ChkStatus(string _MO)
        {
            string ModelName, CustCode, RouteCode;

            string strGetStatus = $"SELECT MODEL_NAME, CUST_CODE from SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '{_MO}'";
            var qry_Status = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetStatus,
                SfcCommandType = SfcCommandType.Text
            });
            dynamic ads = qry_Status.Data;
            ModelName = ads[0]["model_name"];
            CustCode = ads[0]["cust_code"];

            string strGetMODEL= $"SELECT MODEL_NAME,ROUTE_CODE from SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME = '{ModelName}'";
            var qry_MODEL = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetMODEL,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_MODEL.Data.Count() == 0)
            {
                return "Model Not Define";
            }
            // bỏ check route_error
            //else
            //{
            //    dynamic _ads = qry_MODEL.Data;
            //    RouteCode = Convert.ToString(_ads[0]["route_code"]);
            //    string strGetRoute_Code = $"SELECT ROUTE_CODE from SFIS1.C_ROUTE_NAME_T WHERE ROUTE_CODE = '{RouteCode}'";
            //    var qry_Route_Code = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            //    {
            //        CommandText = strGetRoute_Code,
            //        SfcCommandType = SfcCommandType.Text
            //    });
            //    if (qry_Route_Code.Data.Count() == 0)
            //    {
            //        return "Route Error";
            //    }
            //    else
            //    {
            //        string strGetCust_Code = $"SELECT cust_code from SFIS1.C_CUSTOMER_T cust_code = '{CustCode}'";
            //        var qry_Cust_Code = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            //        {
            //            CommandText = strGetCust_Code,
            //            SfcCommandType = SfcCommandType.Text
            //        });
            //        if (qry_Cust_Code.Data.Count() == 0)
            //        {
            //            return "Customer Not Define";
            //        }
            //    }
            //}
            return null;
        }
        private async void SearchMO_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if(Edt_SearchMO.Text.Trim() == "")
                {
                    string strGetCount1 = "SELECT B.*,C.* from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C, SFIS1.C_CUSTOMER_T E"
                    + " WHERE B.MODEL_NAME=C.MODEL_NAME AND b.cust_code=e.cust_code(+)"
                    + " AND (B.MO_NUMBER NOT IN("
                    + " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A)) AND ROWNUM < 50";
                    // Kiệm bỏ check route
//                    string strGetCount1 = "SELECT B.*,C.*,D.ROUTE_NAME from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C,SFIS1.C_ROUTE_NAME_T D, SFIS1.C_CUSTOMER_T E"
//+ " WHERE B.MODEL_NAME=C.MODEL_NAME AND C.ROUTE_CODE=D.ROUTE_CODE and b.cust_code=e.cust_code(+)"
//+ " AND (B.MO_NUMBER NOT IN("
//+ " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A)) AND ROWNUM < 50";
                    var qry_Count1 = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetCount1,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_Count1.Data.Count() == 0)
                    {
                        btnPrint.IsEnabled = false;
                        return;
                    }
                    else
                    {
                        _MO = Convert_Reason(qry_Count1.Data.ToListObject<LoadMO>().ToList());
                        Lv_DataMO.ItemsSource = _MO;
                        return;
                    }
                }

                string strGetCount = "SELECT B.*,C.*,'' as ROUTE_NAME from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C, SFIS1.C_CUSTOMER_T E"
                    + " WHERE B.MODEL_NAME=C.MODEL_NAME AND b.cust_code=e.cust_code(+)"
                    + " AND (B.MO_NUMBER NOT IN("
                    + " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A) and MO_NUMBER =: MO_NUMBER)";
                //Kiệm bỏ check route
                //string strGetCount = "SELECT B.*,C.*,D.ROUTE_NAME from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C,SFIS1.C_ROUTE_NAME_T D, SFIS1.C_CUSTOMER_T E"
                //    + " WHERE B.MODEL_NAME=C.MODEL_NAME AND C.ROUTE_CODE=D.ROUTE_CODE and b.cust_code=e.cust_code(+)"
                //    + " AND (B.MO_NUMBER NOT IN("
                //    + " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A) and MO_NUMBER =: MO_NUMBER)";
                var qry_Count = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetCount,
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="MO_NUMBER",Value=Edt_SearchMO.Text}
                    }
                });
                if (qry_Count.Data.Count() == 0)
                {
                    string strGetMO = $"SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER='{Edt_SearchMO.Text}'";
                    var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                    {
                        CommandText = strGetMO,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (qry_MO.Data != null)
                    {
                        string sClose_Flag = qry_MO.Data["close_flag"]?.ToString() ?? "";
                        MessageBox.Show($"MO can not create(Close_Flag = {sClose_Flag}),Cong lenh khong the tao moi(Close_Flag = {sClose_Flag})", "PM", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        btnPrint.IsEnabled = false;
                        ShowMessageForm _er = new ShowMessageForm(sfcClient);
                        _er.CustomFlag = true;
                        _er.MessageEnglish = "00237 - " + await GetPubMessage("00237");
                        _er.MessageVietNam = "00237 - " + await GetPubMessage("00237");
                        _er.ShowDialog();
                        return;
                    }
                }
                else
                {
                    _MO = Convert_Reason(qry_Count.Data.ToListObject<LoadMO>().ToList());
                    Lv_DataMO.ItemsSource = _MO;
                }
                Edt_SearchMO.Focus();
                Edt_SearchMO.SelectAll();
            }
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

        private async void Btn_OK_Click(object sender, RoutedEventArgs e)
        {
            if (await CheckMoHold(Edt_SearchMO.Text) == true)
            {
                frm_MO_ManageForm.LOADFLAG = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("MO has been holded,please Call PIE(IE)!", "PM", MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }
        }
        private async Task<bool> CheckMoHold(string _mo)
        {
            string strGetData = $"select * from sfism4.r_bpcs_moplan_t where mo_number='{_mo}'";
            var qry_MO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetData,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_MO.Data != null)
            {
                string sLoc = qry_MO.Data["loc"]?.ToString() ?? "";
                if (sLoc == "HOLD")
                {
                    return false;
                }
            }
            return true;
        }
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            frm_MO_ManageForm.LOADFLAG = false;
            this.Close();
        }
        private async void btn_Print(object sender, RoutedEventArgs e)
        {
            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook workbook;
            Microsoft.Office.Interop.Excel.Worksheet worksheet;
            string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\";
            DataTable tbl = new DataTable(); 
            
            string strGetCount = "SELECT B.*,C.*,D.ROUTE_NAME from SFISM4.R_BPCS_MOPLAN_T B,SFIS1.C_MODEL_DESC_T C,SFIS1.C_ROUTE_NAME_T D, SFIS1.C_CUSTOMER_T E"
                    + " WHERE B.MODEL_NAME=C.MODEL_NAME AND C.ROUTE_CODE=D.ROUTE_CODE and b.cust_code=e.cust_code(+)"
                    + " AND (B.MO_NUMBER NOT IN("
                    + " SELECT A.MO_NUMBER FROM SFISM4.R_MO_BASE_T A))";
            var qry_Count = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCount,
                SfcCommandType = SfcCommandType.Text
            });
            tbl = ToDataTable<LoadMO>(qry_Count.Data.ToListObject<LoadMO>().ToList());
            try
            {
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;

                workbook = excel.Workbooks.Add(Type.Missing);
                worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets["Sheet1"];
                worksheet.Name = "ListMO";

                //header
                for (var i = 0; i < tbl.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = tbl.Columns[i].ColumnName;
                }

                //export content
                for (var i = 0; i < tbl.Rows.Count; i++)
                {
                    // to do: format datetime values before printing
                    for (var j = 0; j < tbl.Columns.Count; j++)
                    {
                        String VALUE = tbl.Rows[i][j].ToString();
                        if ((tbl.Rows[i][j].ToString() == "" || tbl.Rows[i][j].ToString() == null) && tbl.Columns[j].ColumnName != "IntelliVision License" && tbl.Columns[j].ColumnName != "INTERFACE_VERSION")
                        {
                            MessageBox.Show("ERROR: Export data have null! Can not export to excel file");
                        }
                        else
                        {
                            worksheet.Cells[i + 2, j + 1] = "'" + tbl.Rows[i][j].ToString();
                        }
                    }
                }

                //Save
                try
                {
                    worksheet.SaveAs($"D:\\_New_MO.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlCSVMSDOS);
                    excel.Quit();
                    MessageBox.Show("Excel file successful!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ExportToExcel: Excel file could not be saved! Check filepath.\n"
                                        + ex.Message);
                }
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
        public DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
