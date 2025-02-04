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

namespace CQC
{
    /// <summary>
    /// Interaction logic for Form4Window.xaml
    /// </summary>
    public partial class Form4Window : Window
    {
        private CheckboxssnWindow Fm_checkboxssn;
        public Form4Window(CheckboxssnWindow _Fm_checkboxssn)
        {
            Fm_checkboxssn = _Fm_checkboxssn;
            InitializeComponent();
        }

        private void btnok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(edt1.Text))
            {
                MessageBox.Show("Data scan null ,Please input data scan !","CQC");
            }
            else
            {
                Fm_checkboxssn.SetupScanCustSN1.IsChecked = true;
                Fm_checkboxssn.Label5.Visibility= Visibility.Visible;
                Fm_checkboxssn.lb_dataCust.Visibility= Visibility.Visible;
                Fm_checkboxssn.Label5.Content= edt1.Text;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Fm_checkboxssn.SetupScanCustSN1.IsChecked = false;
            Fm_checkboxssn.Label5.Visibility = Visibility.Collapsed;
            Fm_checkboxssn.lb_dataCust.Visibility = Visibility.Collapsed;
            this.Close();
        }

        private void edt1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnok_Click(new object(), new RoutedEventArgs());
            }
        }
    }
}
