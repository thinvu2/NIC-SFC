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
    /// Interaction logic for UnitCountWindow.xaml
    /// </summary>
    public partial class UnitCountWindow : Window
    {
        public UnitCountWindow(string _lablUnitCount, string _editUnitCount)
        {
            InitializeComponent();
            editUnitCount.Text = _editUnitCount;
            lablUnitCount.Content = _lablUnitCount;
        }

        private void bbtnOk_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("SFIS.ini");
            ini.Write("CQC", "Unit Count", editUnitCount.Text);
        }

        private void bbtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
