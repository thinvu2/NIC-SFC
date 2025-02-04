using Sfc.Core.Parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace REPAIR
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class frmInputData : Window
    {
        MainWindow frmMain = new MainWindow();
        public frmInputData()
        {
            InitializeComponent();
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbbFuntion.Text == "REPAIR")
            {
                if (tbInputData.Text.Length == 0)
                {
                    MessageBox.Show("Nhập SN cần sửa chữa !!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    tbInputData.Focus();
                    return;
                }
                MainWindow.DataInput = tbInputData.Text;
            }
            else if (cbbFuntion.Text == "CHECK_IN" || cbbFuntion.Text == "CHECK_OUT")
            {
                MainWindow.DataInput = cbbFuntion.Text;
            }
            this.Close();
        }
  

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            //Environment.Exit(0);
            this.Close();

        }

        private void frmInputData_Loaded(object sender, RoutedEventArgs e)
        {
            tbInputData.SelectAll();
            tbInputData.Focus();
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
