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
    /// Interaction logic for frmModify.xaml
    /// </summary>
    public partial class formReasonCode : Window
    {
        public string FLAG, SN, MODEL_NAME , sqlstr;
        public SfcHttpClient sfcHttpClient;
        DataTable dtTable;
        public formReasonCode()
        {
            InitializeComponent();
        }

        private async void ReasonCode_Loaded(object sender, RoutedEventArgs e)
        {
            sqlstr = string.Format(sqlStr.qryReasonCode);
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlstr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null && result.Data.Count() > 0)
            {
                string json = JsonConvert.SerializeObject(result.Data);
                dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                gridResonCode.DataContext = dtTable;
            }
        }

 

        private async void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (tbInput.Text != "")
            {
                sqlstr = string.Format(sqlStr.qryReasonCode2, tbInput.Text.ToUpper());
            }
            else
            {
                sqlstr = string.Format(sqlStr.qryReasonCode);
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
                gridResonCode.DataContext = dtTable;
                gridResonCode.Items.Refresh();
            }
        }

        private void gridResonCode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = gridResonCode.SelectedItem as DataRowView;
            frmModify.ReasonCode = row.Row.ItemArray[0].ToString();
            frmModify.ReasonDesc = row.Row.ItemArray[2].ToString();
            this.Close();
        }

        private void tbInput_KeyUp(object sender, KeyEventArgs e)
        {
            btnSelect_Click(sender,e);
        }

        private void dgr_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = " " + e.PropertyName.ToUpper().Replace("_", " ") + "  ";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
