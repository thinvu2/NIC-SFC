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

namespace LOT_REPRINT
{
    /// <summary>
    /// Interaction logic for frmInputRoute.xaml
    /// </summary>
    public partial class frmInputRoute : Window
    {
        public frmInputRoute()
        {
            InitializeComponent();
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            txtRoute.Text = ini.IniReadValue("LOTREPRINT", "CHCEK_ROUTENAME");
            txtRoute.SelectAll();
            txtRoute.Focus();
        }

        private void txtRoute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                inputRoute();
                this.Close();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            inputRoute();
            this.Close();
        }

        private void inputRoute()
        {
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            ini.IniWriteValue("LOTREPRINT", "CHCEK_ROUTENAME", txtRoute.Text);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
