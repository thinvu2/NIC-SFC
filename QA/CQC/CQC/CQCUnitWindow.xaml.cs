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
    /// Interaction logic for CQCUnitWindow.xaml
    /// </summary>
    public partial class CQCUnitWindow : Window
    {
        private MainWindow formCQC;
        public CQCUnitWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            InitializeComponent();
            editCQCUnit.Text = formCQC.editCQCUnit;
        }

        public void bbtnOK_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            ini.Write("CQC", "CQC Unit", editCQCUnit.Text);
            formCQC.grpbPalletNoNotChecked.Content = editCQCUnit.Text + " not been checked ";
            formCQC.grpbPalletNoInLot.Content = editCQCUnit.Text + " in this lot ";
            formCQC.been_check1.Header = editCQCUnit.Text;
            formCQC.this_lot1.Header = editCQCUnit.Text;
            this.Close();
        }

        private void bbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
