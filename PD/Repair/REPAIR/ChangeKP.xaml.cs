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

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for ItemCode.xaml
    /// </summary>
    public partial class ChangeKP : Window
    {
        public string sqlstr;
        public SfcHttpClient sfcHttpClient;
        DataTable dtTable;
        public ChangeKP()
        {
            InitializeComponent();
        }

        private async void ItemCode_Loaded(object sender, RoutedEventArgs e)
        {
            sqlstr = string.Format(sqlStr.qryR108,tbInput.Text);
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlstr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                string json = JsonConvert.SerializeObject(result.Data);
                dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                gridKeypart.DataContext = dtTable;
            }
        }

        private  async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (tbInput.Text != "")
            {
                sqlstr = string.Format(sqlStr.qryR108, tbInput.Text.ToUpper());
            }
            else
            {
                return;
            }

            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlstr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null)
            {
                string json = JsonConvert.SerializeObject(result.Data);
                dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                gridKeypart.DataContext = dtTable;
                gridKeypart.Items.Refresh();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            gridSetup.Height = 150;
        }

        private void tbInput_KeyUp(object sender, KeyEventArgs e)
        {
            btnSearch_Click(sender,e);
        }

        private void dgr_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = " " + e.PropertyName.ToUpper().Replace("_", " ") + "  ";
        }

  

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            gridSetup.Height = 0;
        }

        private void gridKeypart_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = gridKeypart.SelectedItem as DataRowView;
            tblKeypartSN.Text = row.Row.ItemArray[0].ToString();
            tblKeypartNO.Text = row.Row.ItemArray[1].ToString();
            tblKeypartDesc.Text = row.Row.ItemArray[2].ToString();
            tbKeypartSN.Text = row.Row.ItemArray[0].ToString();
            tbKeypartNO.Text = row.Row.ItemArray[1].ToString();
            tbKeypartDesc.Text = row.Row.ItemArray[2].ToString();
            //frmModify.ItemCode = row.Row.ItemArray[0].ToString();
        }

        private void tbKeypartSN_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
