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

namespace PM
{
    /// <summary>
    /// Interaction logic for PO_FORM.xaml
    /// </summary>
    public partial class POForm : Window
    {
        public POForm()
        {
            InitializeComponent();
        }

        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_OK(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
