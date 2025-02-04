using Sfc.Core.Parameters;
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

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class frmInputReason : Window
    {
        MainWindow frmMain = new MainWindow();
        public frmInputReason()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tbReason.Text.Length == 0 )
            {
                MessageBox.Show("Reason close can not null , Please input reason!!","Error", MessageBoxButton.OK,MessageBoxImage.Error );

                tbReason.Focus();
                return;
            }
            MainWindow.DataReasonClose = tbReason.Text;
            this.Close();
        }
  

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.DataReasonClose = "";
            this.Close();
        }

        private void frmInputData_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.DataReasonClose = "";
            tbReason.SelectAll();
            tbReason.Focus();
        }

        private void tbInputData_Keydown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender, e);
            }   
        }
    }
}
