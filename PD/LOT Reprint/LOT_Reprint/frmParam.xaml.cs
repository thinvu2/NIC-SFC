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
    /// Interaction logic for frmParam.xaml
    /// </summary>
    public partial class frmParam : Window
    {
        public string sUrlFile;
        public frmParam()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            urlFile.Text = sUrlFile;
        }
    }
}
