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
    /// Interaction logic for RMAPOWindow.xaml
    /// </summary>
    public partial class RMAPOWindow : Window
    {
        private MainWindow formCQC;
        public RMAPOWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            formCQC.po_flag = false;
        }

        private void MaskEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return)
            {
                pnl1.Content = string.Empty;
                if(MaskEdit1.Password== "FQACHECK2011")
                {
                    MessageBox.Show("success!,ADD GREEN DOT!","Done");
                    formCQC.po_flag = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("password Error !!", "ERROR");
                    pnl1.Content = "input password error!";
                    pnl1.Foreground = Brushes.Red;
                    formCQC.po_flag = false;
                }
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            pnl1.Content = string.Empty;
            if (MaskEdit1.Password == "FQACHECK2011")
            {
                MessageBox.Show("success!,ADD GREEN DOT!", "Done");
                formCQC.po_flag = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("password Error !!", "ERROR");
                pnl1.Content = "input password error!";
                pnl1.Foreground = Brushes.Red;
                formCQC.po_flag = false;
            }
        }

        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            formCQC.po_flag = false;
            this.Close();
        }
    }
}
