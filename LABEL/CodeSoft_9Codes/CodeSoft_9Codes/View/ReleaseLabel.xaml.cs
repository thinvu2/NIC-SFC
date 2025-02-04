using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Sfc.Core.Extentsions;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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

namespace CodeSoft_9Codes.View
{
    /// <summary>
    /// Interaction logic for ReleaseLabel.xaml
    /// </summary>
    public partial class ReleaseLabel : UserControl
    {
        private SfcHttpClient sfcClient;
        private string sEmp;
        DAL fDal;
        DataTable dt;
        public ReleaseLabel(SfcHttpClient _sfc, string _empname, string _empno)
        {
            InitializeComponent();
            sfcClient = _sfc;
            sEmp = _empno;
            fDal = new DAL();
            dt = new DataTable();
        }

        private void txtMo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void txtMo_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(txtMo.Text))
                {
                    txtMo.Text = txtMo.Text.Trim();
                    string sql = $"SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE  MO_NUMBER = '{txtMo.Text}'";
                    dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                    if (dt.Rows.Count > 0)
                    {
                        txtModel.Text = dt.Rows[0]["model_name"].ToString();
                        txtTargetQty.Text = dt.Rows[0]["target_qty"].ToString();
                    }
                    else
                    {
                        showMessage("Không có dữ liệu công lệnh", "MO have not data");
                        return;
                    }
                    // string yearstr = getYearMonthDay(0).Substring(0, 8);
                    //string datestr = getHHdatecode(yearstr);
                    //getMOInfor(txtMo.Text,datestr);
                    sql = $"select * from sfism4.r_mo_ext2_t where mo_number= '{txtMo.Text}' and ver_5='PrintData'";
                    dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                    if (dt.Rows.Count > 0)
                    {
                        lvListMORange.Items.Clear();
                        for(int i = 0; i<dt.Rows.Count; i++)
                        {
                            lvListMORange.Items.Add(dt.Rows[i]["item_1"].ToString() + " ~ " + dt.Rows[i]["item_2"].ToString());
                        }
                    }
                    sql = "select * from  sfism4.r_mo_ext2_t where mo_number='" + txtMo.Text + "' and ver_5='PrintData' and item_6='PRINT' ";

                    dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                    if (dt.Rows.Count > 0)
                    {
                        sql = "select * from  sfism4.r_print_input_t where mo_number='" + txtMo.Text + "' and print_flag='N' ";
                        dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                        if (dt.Rows.Count == 0)
                        {
                            if (MessageBox.Show("Công lệnh đã in xong, bạn chắc chắn muốn in bù???" + Environment.NewLine + "Work order  has print completed, whether to overflow brought?", "PrintDataInput", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                            {
                                DataTable dt1 = new DataTable();
                                PasswordForm pass = new PasswordForm();
                                pass.ShowDialog();
                                sql = "select * from sfis1.C_PRIVILEGE where emp in (select emp_no from sfis1.C_EMP_DESC_T where emp_bc = '{0}' or emp_pass = '{0}') and privilege = '2' and fun = 'OVERFLOW' and prg_name ='PrintDataInput' ";
                                dt1 =await fDal.ExcuteSelectSQL(string.Format(sql, pass.password), sfcClient);
                                if (dt1.Rows.Count > 0)
                                {
                                    sEmp = dt1.Rows[0]["emp"].ToString();
                                    gbOverFlow.Visibility = Visibility.Visible;
                                    txtMo.IsEnabled = false;
                                    btnInput.IsEnabled = false;
                                    txtOverFlow.Text = Math.Round((double)int.Parse(txtTargetQty.Text) * 0.003).ToString();
                                    return;
                                }
                                else
                                {
                                    MessageBox.Show("You no have privilege Print Ovew Flow" + Environment.NewLine + "Bạn không có quyền hạn để release thêm dải", "PrintDataInput", MessageBoxButton.OK, MessageBoxImage.Stop);
                                    return;
                                }
                            }
                            else return;
                            
                        }
                        else
                        {
                            MessageBox.Show("There are data not print out, unable to do overflow lalbel!?" + Environment.NewLine + "VẪN CÒN DỮ LIỆU CHƯA IN, KHÔNG THỂ RELEASE THÊM", "PrintDataInput", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                    }
                    
                    txtMo.IsEnabled = false;
                    btnInput.IsEnabled = true;
                }

            }
        }
        public async Task<bool> getMOInfor(string mo_number,string datecode,string flag,string emp,string qty)
        {
            var _data = new
            {
                TYPE = "GETMOINFOR",
                PRG_NAME = "PRINTDATAINPUT",
                MO_NUMBER = mo_number,
                DATECODE = datecode,
                TARGETQTY = qty,
                EMPNO = emp,
                OVERFLOW = flag
            };
            try
            {
                string _jsonData = JsonConvert.SerializeObject(_data).ToString();
                var _result = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE_PRINT_INPUT",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });
                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');
                if (_RESArray[0] == "OK")
                {
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        showMessage(_RESArray[1].ToString(), _RESArray[2].ToString());
                        return false;
                    }
                    else
                    {
                        showMessage(_RESArray[0].ToString(), "procedure have exceptions");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                showMessage(ex.Message, " Call procedure have exceptions");
                return false;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtMo.Focus();
        }
        public async Task<string> zgetYearMonthDay(int addday)
        {
            string strSQL = " select to_char(sysdate-138+" + addday + ",'YYYYMMDD') sdate from dual ";
            DataTable dt = new DataTable();
            dt = await fDal.ExcuteSelectSQL(strSQL, sfcClient);
            return dt.Rows[0]["sdate"].ToString();
        }
        public async Task<string> getYearMonthDay(int addday)
        {
            string strSQL = " select to_char(sysdate+" + addday + ",'YYYYMMDDIW') sdate from dual ";
            DataTable dt = new DataTable();
            dt = await fDal.ExcuteSelectSQL(strSQL, sfcClient);
            return dt.Rows[0]["sdate"].ToString();
        }
        
        private void showMessage(string _messVN,string _messEN)
        {
            ShowMessageForm _sh = new ShowMessageForm();
            _sh.CustomFlag = true;
            _sh.MessageVietNam = _messVN;
            _sh.MessageEnglish = _messEN;
            _sh.ShowDialog();
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private async Task<bool> checkInput(string mo, string target_qty)
        {
            
            string ssql = "select * from  sfism4.r_print_input_t where mo_number='" + mo + "' ";
            dt = await fDal.ExcuteSelectSQL(ssql, sfcClient);
            if (dt.Rows.Count == 0)
            {
                return true;
            }
            if (gbOverFlow.IsVisible)
            {
                return true;
            }
            if (dt.Rows.Count < Convert.ToInt32(target_qty))
            {
                return true;
            }
            return false;
        }

        public string getNextSn(string sn, string mystring)
        {
            char laststr;
            string last;
            string tempsn;
            int snlength, strlength;
            strlength = mystring.Length;
            snlength = sn.Length;
            laststr = Convert.ToChar(sn.Substring(snlength - 1, 1));
            if (mystring[strlength - 1] == laststr)
            {
                if (snlength != 1)
                {
                    laststr = mystring[0];
                    last = Convert.ToString(laststr);
                    tempsn = getNextSn(sn.Substring(0, snlength - 1), mystring) + last;
                    return tempsn;
                }
                else
                {
                    return "--";
                }
            }
            else
            {
                tempsn = sn.Substring(0, snlength - 1) + mystring[mystring.IndexOf(laststr) + 1];
                return tempsn;
            }
        }

      
        private async void btnInput_Click(object sender, RoutedEventArgs e)
        {
            /*
            string zsql = "";
            int z = 0;
            while ( z < 3650)
            {
                string YMD = await zgetYearMonthDay(z);
                string datestr = WEEKCODE.YearWeek(DateTime.ParseExact(YMD, "yyyyMMdd",CultureInfo.InvariantCulture));
                zsql = "INSERT INTO SFISM4.R_SYSTEM_LOG_T VALUES ('Z','GETDC','"+ YMD + "','"+ datestr + "',SYSDATE)";
                var query_update = await sfcClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = zsql,
                    SfcCommandType = SfcCommandType.Text
                });
                z ++;
            }
            return;
            */
            
            if (string.IsNullOrEmpty(txtMo.Text)) return;
            if (await checkInput(txtMo.Text, txtTargetQty.Text))
            {
                string yearstr =await getYearMonthDay(0);
                yearstr = yearstr.Substring(0, 8);
                string datestr = WEEKCODE.YearWeek(System.DateTime.Today);
                bool rs = false;
                if(gbOverFlow.Visibility == Visibility.Visible)
                {
                   rs =await getMOInfor(txtMo.Text, datestr,"Y",sEmp,txtOverFlow.Text.Trim());
                }
                else rs =await getMOInfor(txtMo.Text, datestr,"N",MainWindow.empNo,txtTargetQty.Text);
                
                
                if (!rs) return;
                string sql = $"select * from sfism4.r_mo_ext2_t where mo_number= '{txtMo.Text}' and ver_5='PrintData'";
                dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                if (dt.Rows.Count > 0)
                {
                    lvListMORange.Items.Clear();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        lvListMORange.Items.Add(dt.Rows[i]["item_1"].ToString() + " ~ " + dt.Rows[i]["item_2"].ToString());
                    }
                    
                }
                btnClear.IsEnabled = true;
                btnInput.IsEnabled = false;
                MessageBox.Show(" Nhập dữ liệu thành công | Input data OK","OK",MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtMo.IsEnabled = true;
            btnInput.IsEnabled = false;
            txtMo.Clear();
            txtModel.Clear();
            txtTargetQty.Clear();
            gbOverFlow.Visibility = Visibility.Collapsed;
            lvListMORange.Items.Clear();
        }

        private void txtOverFlow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnInput.IsEnabled = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            BG.Children.Clear();
            if (miQuery.IsChecked)
            {
                BG.Children.Add(new QueryData(sfcClient,sEmp));
            }
        }

        private void miSetmodel_Checked(object sender, RoutedEventArgs e)
        {
            BG.Children.Clear();
            if (miSetmodel.IsChecked)
            {
                 BG.Children.Add(new SetModel(sfcClient, sEmp));
            }
        }

        private void miSetPrint_Click(object sender, RoutedEventArgs e)
        {
            BG.Children.Clear();
            if (miSetPrint.IsChecked)
            {
                BG.Children.Add(new SetPrintData(sfcClient, sEmp));
            }
        }
    }

    
    public static class WEEKCODE
    {
        public static string F36(int n)
        {
            if (n < 10) return n.ToString();
            else
            {
                char s = (char)(n - 9 + 64);
                return s.ToString();
            }
        }
        public static string C36(int YW)
        {
            int YW1 = 0;
            int YW2 = 0;
            string TempString = "";
            do
            {
                YW1 = (int)(YW / 36);
                YW2 = YW - YW1 * 36;
                TempString = F36(YW2) + TempString;
                YW = YW1;
            }
            while (YW1 != 0);
            return TempString.PadLeft(2,'0');
        }
        public static string YearWeek(DateTime Thedate) 
        {
            int J = Thedate.Year - 2024;
            int M = 0;
            int YW = 0;
            if (J >= 0)
            {
                for (int i = 1; i <= J; i++)
                {
                    string TmpDate = "12/31/" + (i + 2023).ToString();
                    M = M + DatePart("ww", DateTime.Parse(TmpDate));
                }
                YW = M + DatePart("ww", Thedate);

            }
            else
                YW = 0;
            return C36(YW);
        }
        public static int DatePart(string Interval, DateTime date)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            System.Globalization.Calendar cal = dfi.Calendar;
            int week = cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            return week;
        }
        public static int GetTotal(int StardNo, int EndNo, int Total)
        {
            return (int)((EndNo - StardNo + Total) / Total);
        }

        public static int TempLong(int StardNO, int i, int j,int totalinrow)
        {
            return StardNO + (i - 1) * totalinrow + j - 1;
        }
    
       
    }
}
