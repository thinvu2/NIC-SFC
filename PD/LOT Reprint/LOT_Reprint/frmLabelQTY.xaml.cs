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
    /// Interaction logic for frmLabelQTY.xaml
    /// </summary>
    public partial class frmLabelQTY : Window
    {
        public frmLabelQTY()
        {
            InitializeComponent();
            getValue();
        }
        void getValue()
        {
            Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
            SNQty.Text = ini.IniReadValue("SLabelQTY", "Default");
            CartonQty.Text = ini.IniReadValue("CLabelQTY", "Default");
            PalletQty.Text = ini.IniReadValue("PLabelQTY", "Default");
            se_top.Text = ini.IniReadValue("PRINT_TOP", "Default");
            se_left.Text = ini.IniReadValue("PRINT_LEFT", "Default");
            se_Vertical.Text = ini.IniReadValue("PRINT_Vertical", "Default");
            se_Horizontal.Text = ini.IniReadValue("PRINT_Horizontal", "Default");
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ini ini = new Ini(@"C:\PACKING\PACKING.Ini");
                ini.IniWriteValue("SLabelQTY", "Default", SNQty.Text);
                ini.IniWriteValue("CLabelQTY", "Default", CartonQty.Text);
                ini.IniWriteValue("PLabelQTY", "Default", PalletQty.Text);

                ini.IniWriteValue("PRINT_TOP", "Default", se_top.Text);
                ini.IniWriteValue("PRINT_LEFT", "Default", se_left.Text);
                ini.IniWriteValue("PRINT_Vertical", "Default", se_Vertical.Text);
                ini.IniWriteValue("PRINT_Horizontal", "Default", se_Horizontal.Text);
                //ini.IniWriteValue("PRINT_TOP", "Default", se_top.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
