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
    /// Interaction logic for DutyTypeWindow.xaml
    /// </summary>
    public partial class DutyTypeWindow : Window
    {
        MainWindow formCQC = new MainWindow();
        public DutyTypeWindow()
        {
            InitializeComponent();
        }
        public DutyTypeWindow(ListBox _ListBoxReasonCode,ListBox _ListBoxDutyCode,ListBox _ListBoxLocation)
        {
            InitializeComponent();
            ListBoxReasonCode = _ListBoxReasonCode;
            ListBoxDutyCode = _ListBoxDutyCode;
            ListBoxLocation = _ListBoxLocation;
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            ListBoxLocation.Items.Clear();
            ListBoxReasonCode.Items.Clear();
            ListBoxDutyCode.Items.Clear();
            formCQC.G_sType = "";
            formCQC.G_sReasonCode = "";
            formCQC.G_sDutycode = "";
            formCQC.G_sLocation = "";
        }
    }
}
