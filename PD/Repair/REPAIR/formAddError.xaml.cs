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
    public partial class formAddError : Window
    {
        public string FLAG, SN, MODEL_NAME , sqlstr;
        public SfcHttpClient sfcHttpClient;
        DataTable dtTable;
        public formAddError()
        {
            InitializeComponent();
        }

        private async void AddError_Loaded(object sender, RoutedEventArgs e)
        {
           if ( !await getError("") )
           {

           }
        }


        private async Task<Boolean> getError(string ER)
        {
            try
            {
                if (ER != "")
                {
                    sqlstr = " SELECT * FROM SFIS1.C_ERROR_CODE_T WHERE ERROR_CODE LIKE  '" + ER + "%'  ";
                }
                else
                {
                    sqlstr = " SELECT * FROM SFIS1.C_ERROR_CODE_T  ";
                }
                var result = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sqlstr,
                    SfcCommandType = SfcCommandType.Text
                });
                if (result.Data != null && result.Data.Count() > 0)
                {
                    string json = JsonConvert.SerializeObject(result.Data);
                    dtTable = JsonConvert.DeserializeObject<DataTable>(json);
                    gridErrorCode.DataContext = dtTable;
                }
                else
                {
                    gridErrorCode.DataContext = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                showError("Get error code have exception ", ex.Message, true);
                return false;

            }
        }

        private async void tbErrorCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbErrorCode.Text != string.Empty)
            {
                if (!await getError(tbErrorCode.Text))
                {

                }
            }
        }
        private void showError(string strVN, string strEng, bool Flag)
        {
            MessageError FrmMessage = new MessageError();
            FrmMessage.sfcHttpClient = this.sfcHttpClient;
            if (Flag)
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.MessageVietNam = strVN;
                FrmMessage.MessageEnglish = strEng;
            }
            else
            {
                FrmMessage.CustomFlag = Flag;
                FrmMessage.errorcode = strEng;
            }

            FrmMessage.ShowDialog();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void gridResonCode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataRowView row = gridErrorCode.SelectedItem as DataRowView;
            tbErrorCode.Text = row.Row.ItemArray[0].ToString();
            tbDescription.Text = row.Row.ItemArray[3].ToString();
        }

  
        private void dgr_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = " " + e.PropertyName.ToUpper().Replace("_", " ") + "  ";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.fAddEC = true;
            MainWindow.EC_ERROR_CODE = tbErrorCode.Text;
            this.Close();
        }
    }
}
