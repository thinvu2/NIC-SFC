using Sfc.Core.Extentsions;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for QueryData.xaml
    /// </summary>
    public partial class QueryData : UserControl
    {
        private SfcHttpClient sfcClient;
        private string sEmp;
        DataTable dt;
        DAL fDal;
        public QueryData(SfcHttpClient _sfc, string _emp)
        {
            InitializeComponent();
            sfcClient = _sfc;
            sEmp = _emp;
            fDal = new DAL();
            dt = new DataTable();

        }

        private async void txtMo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string sql = $"SELECT * FROM SFISM4.R_BPCS_MOPLAN_T WHERE  MO_NUMBER = '{txtMo.Text}'";
                dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu công lệnh" + Environment.NewLine + "MO have not data", "Error");
                    return;
                }
                string ssql = "";
                if ((bool)chkUnPrint.IsChecked) ssql += " AND PRINT_FLAG='N' ";
                if ((bool)chkOverFlow.IsChecked) ssql += " AND GROUP_NAME <> 'LABELROOM' ";
                sql = $"SELECT * FROM SFISM4.R_PRINT_INPUT_T WHERE  MO_NUMBER = '{txtMo.Text}' " + ssql;
                dt = await fDal.ExcuteSelectSQL(sql, sfcClient);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu release label công lệnh" + Environment.NewLine + "MO have not data release!!!", "Error");
                    return;
                }
                if (dt.Rows.Count > 0)
                {
                    dtg.DataContext = dt;
                }
                var sPrinted = dt.AsEnumerable().Where(myRow => myRow.Field<string>("print_flag") == "Y");   //Select(p => new { print_flag = p.Field<string>("print_flag") =="Y" }).ToList();
                var sNOtPrint = dt.AsEnumerable().Where(myRow => myRow.Field<string>("print_flag") == "N");
                var sOver = dt.AsEnumerable().Where(myRow => myRow.Field<string>("group_name") == "OverFlow");
                tblNotify.Text = string.Format("Total: {0}, Printed: {1}, Not Print:{2},OverFlow:{3}",dt.Rows.Count,sPrinted.Count(),sNOtPrint.Count(),sOver.Count());
            }
        }

        private void txtMo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dtg_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = e.PropertyName.ToUpper().Replace("_", "__");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtMo.Focus();
        }
    }
}
