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
    /// Interaction logic for formWarehouseNOWindow.xaml
    /// </summary>
    public partial class formWarehouseNOWindow : Window
    {
        private MainWindow formCQC;
        public formWarehouseNOWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(editWarehouseNO.Text))
            {
                MessageBox.Show("Please input Warehouse No !!", "CQC", MessageBoxButton.OK);
                return;
            }
            formCQC.editWarehouseNO = editWarehouseNO.Text;
            this.Close();
        }
    }
}
