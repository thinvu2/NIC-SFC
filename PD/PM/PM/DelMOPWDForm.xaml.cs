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
    /// Interaction logic for DelMOPWDForm.xaml
    /// </summary>
    public partial class DelMOPWDForm : Window
    {
        public DelMOPWDForm()
        {
            InitializeComponent();
            Edt_DelMOPwd.Focus();
        }

        private void Edt_DelMOPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.Close();
            }
        }

        private void Edt_DelMOPwd_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
