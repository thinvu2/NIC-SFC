using Sfc.Library.HttpClient;
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
using Sfc.Core.Parameters;
using CQC.ViewModel;
using Sfc.Library.HttpClient.Helpers;
namespace CQC
{
    /// <summary>
    /// Interaction logic for oobafrmWindow.xaml
    /// </summary>
    public partial class oobafrmWindow : Window
    {
        public oobafrmWindow(string _QA, List<OOBA> _Source)
        {
            InitializeComponent();
            Panel1.Content = _QA;
            ColorStringGrid1.ItemsSource = _Source;
        }
    }
}
