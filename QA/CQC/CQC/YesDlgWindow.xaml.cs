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
    /// Interaction logic for YesDlgWindow.xaml
    /// </summary>
    public partial class YesDlgWindow : Window
    {
        private MainWindow formCQC;
        public YesDlgWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            formCQC.MrOK = false;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            BitBtn1.Focus();
        }

        private void BitBtn1_Click(object sender, RoutedEventArgs e)
        {
            formCQC.MrOK = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
