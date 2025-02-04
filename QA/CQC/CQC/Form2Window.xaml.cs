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
    /// Interaction logic for Form2Window.xaml
    /// </summary>
    public partial class Form2Window : Window
    {
        private MainWindow formCQC;
        public Form2Window(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (edtpass.Password != "123456")
            {
                MessageBox.Show("Error !!", "CQC", MessageBoxButton.OK, MessageBoxImage.Error);
                edtpass.SelectAll();
                edtpass.Focus();
            }
            else
            {
                formCQC.ispass = true;
                this.Close();
            }
        }
    }
}
