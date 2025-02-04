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
using System.Collections.ObjectModel;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for frmModify.xaml
    /// </summary>
    public partial class formReport : Window
    {
        public string ALLFlag, filename, SN, MODEL_NAME , sqlstr ,strsql1 , strsql2; 

        public SfcHttpClient sfcHttpClient;
        public ObservableCollection<string> listTimeStart = new ObservableCollection<string>();
        public ObservableCollection<string> listTimeEnd = new ObservableCollection<string>();
        DataTable dtTable;

    
        public formReport()
        {
            InitializeComponent();
        }

 
        private async void Report_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime DT =  DateTime.Now;
            filename = DT.ToString("yymmdd");
            ALLFlag = "ALL";
            var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = " SELECT START_TIME, END_TIME FROM  SFIS1.C_WORK_DESC_T WHERE UPPER(LINE_NAME) = 'DEFAULT' ",
                SfcCommandType = SfcCommandType.Text
            });
            listTimeStart.Add("ALL");
            if (result.Data != null && result.Data.Count() > 0 )
            {
                foreach( var row in result.Data)
                {
                    listTimeStart.Add(row["start_time"].ToString());
                    listTimeEnd.Add(row["end_time"].ToString());
                }
            }
            cboTimeFrom.ItemsSource = listTimeStart;
            cboTimeTo.ItemsSource = listTimeEnd;
        }

        private void chkFuntion2_Click(object sender, RoutedEventArgs e)
        {
            if (chkFuntion2.IsChecked == true)
            {
                chkFuntion1.IsChecked = false;
            }
            else
            {
                chkFuntion1.IsChecked = true;
            }
        }
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            if (chkModel.IsChecked == true)
            {
                if (txtModel.Text  == string.Empty)
                {
                    MessageBox.Show("Model Name is Null!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    txtModel.Text = txtModel.Text.ToUpper();
                }
                strsql1 = " AND MODEL_NAME  IN (" + getModelName(txtModel.Text) + ") ";
                if (chkOut.IsChecked == true || chkIn.IsChecked == true || chkAll.IsChecked == true)
                {
                    strsql1 = " AND A.SERIAL_NUMBER IN (SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE C.MODEL_NAME IN (" + getModelName(txtModel.Text) + ")) ";
                }

            }

            if (chkRepairer.IsChecked == true)
            {
                if (txtRepairer.Text == string.Empty)
                {
                    MessageBox.Show("Repairer is Null!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
         
                if (chkIn.IsChecked == true)
                {
                    strsql2 = " and a.in_datetime is not null ";
                    strsql1 = strsql1 + "  and a.r_receiver = '" + txtRepairer + "' ";
                }
                else if (chkOut.IsChecked == true)
                {
                    strsql2 = " and a.out_datetime is not null ";
                    strsql1 = strsql1 + "  and a.r_sender = '" + txtRepairer + "' ";
                }
                else if (chkAll.IsChecked == true)
                {
                    strsql2 = " and (  a.out_datetime is not null or   in_datetime is not null  ) ";
                    strsql1 = strsql1 + "  and ( a.r_sender = '" + txtRepairer + "'  ora.r_receiver =  '" + txtRepairer + "' " ;
                }
                else
                {
                    strsql2 = "";
                    strsql1 = strsql1 + "  and repairer = '" + txtRepairer + "' ";
                }
            }

            if (cboSection.Text != string.Empty)
            {
                strsql1 = strsql1 + " and test_section = '"+cboSection.Text+"' ";
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
        public string getModelName (string strModel)
        {
            strModel = "'" + strModel.Replace(",", "','") + "'";
            return strModel;
        }
    }
}
