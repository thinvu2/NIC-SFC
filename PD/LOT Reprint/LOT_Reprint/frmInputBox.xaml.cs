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
    /// Interaction logic for frmInputBox.xaml
    /// </summary>
    public partial class frmInputBox : Window
    {
        public static string inputBox;
        public static string _Title;
        public frmInputBox()
        {
            InitializeComponent();
        }
        public frmInputBox(string Title) : this()
        {
            _Title = Title;
            lblTitle.Text = Title;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            inputBox = txtInputBox.Text;
            this.Close();
        }

        private void txtInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOk_Click(sender, e);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //lblTitle.Text = Title;
        }
    }
}
