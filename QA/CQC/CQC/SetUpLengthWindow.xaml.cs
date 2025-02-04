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
    /// Interaction logic for SetUpLengthWindow.xaml
    /// </summary>
    public partial class SetUpLengthWindow : Window
    {
        private MainWindow formCQC;
        public SetUpLengthWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            InitializeComponent();           
        }        

        private void Window_Initialized(object sender, EventArgs e)
        {
            BitBtn1.Focus();
            if(formCQC.itemSamplingSN.IsChecked)
            {
                Label2.Content = "Serial Number";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            SNEdit.Text = "12";
        }

        private void BitBtn1_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            ini.Write("CQC", "SSN", SNEdit.Text);
            formCQC.SNEdit = SNEdit.Text;
            this.Close();
        }

        private void BitBtn2_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            SNEdit.Text = "12";
            formCQC.SNEdit = "12";
            this.Close();
        }
    }
}
