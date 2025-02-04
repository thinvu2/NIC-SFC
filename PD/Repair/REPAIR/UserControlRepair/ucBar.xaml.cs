using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using REPAIR.ViewModel;

namespace REPAIR.UserControlRepair
{
    /// <summary>
    /// Interaction logic for ucBar.xaml
    /// </summary>
    public partial class ucBar : UserControl
    {
        public ControlBarViewModel ViewModel { get; set; }
        public ucBar()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new ControlBarViewModel();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            string FrmSender;
            Process currentp = Process.GetCurrentProcess();
            FrmSender = currentp.MainWindowTitle;
            if (FrmSender.Contains( "Repair ver") )
            {
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
