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

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public string PASS = "";
        public InputBox()
        {
            InitializeComponent();
        }

        private void  InputPass_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender , e );
            }

        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.PassInput = inputPass.Password.ToString();
            PASS = inputPass.Password.ToString();
            this.Close();
        }

        private void InputBox_Loaded(object sender, RoutedEventArgs e)
        {
            inputPass.Password = "";
            inputPass.Focus();
        }
    }
}
