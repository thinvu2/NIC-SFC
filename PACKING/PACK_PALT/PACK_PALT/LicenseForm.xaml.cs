using PACK_PALT;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PACK_PALT_NEW
{
    /// <summary>
    /// Interaction logic for RFIDForm.xaml
    /// </summary>
    public partial class LicenseForm : Window
    {
        public static SfcHttpClient sfcHttpClient;
        public LicenseForm(MainWindow mainprogram, SfcHttpClient _sfcHttpClient)
        {
            InitializeComponent();
            sfcHttpClient = _sfcHttpClient;
            txtLISENCENO.Focus();
        }
        private void txtLISENCENO_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtLISENCENO.Text))
                {
                    string str_sql_query = "SELECT * FROM SFISM4.R_SEC_LIC_LINK_T WHERE LICENSE_NO = '" + txtLISENCENO.Text + "' AND CARTON_NO = '" + MainWindow.txt_mcarton_no + "' ";
                    var select_license = sfcHttpClient.QuerySingle(new QuerySingleParameterModel
                    {
                        CommandText = str_sql_query,
                        SfcCommandType = SfcCommandType.Text
                    });
                    if (select_license.Data == null)
                    {
                        MessageBox.Show("License_no not match with carton", "Error", MessageBoxButton.OK);
                        txtLISENCENO.SelectAll();
                        txtLISENCENO.Focus();
                        return;
                    }
                    else
                    {
                        MainWindow.zFlag = true;
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Data null or white sprace:", "Error", MessageBoxButton.OK);
                    return;
                }
            }
        }
    }
}
