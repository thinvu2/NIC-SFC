using PACK_PALT;
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
    public partial class RFIDForm : Window
    {
        public static SfcHttpClient sfcHttpClient;
        private MainWindow _mainprogram;
        public RFIDForm(MainWindow mainprogram, SfcHttpClient _sfcHttpClient)
        {
            InitializeComponent();
            this._mainprogram = mainprogram;
            sfcHttpClient = _sfcHttpClient;
            txtRFID.Focus();
        }
        private void txtRFID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtRFID.Text))
                {
                    MainWindow.zFlag = MainWindow.LINKRFID(txtRFID.Text);
                    if (!MainWindow.zFlag) return;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Data null or white sprace:", "Error", MessageBoxButton.OK);
                    txtRFID.SelectAll();
                    txtRFID.Focus();
                    return;
                }
            }
        }
    }
}
