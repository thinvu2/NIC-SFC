using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Drawing.Printing;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for SetupStation.xaml
    /// </summary>
    public partial class SetupPrinter : Window
    {
        public string strINIPath  = "C:\\PACKING\\PACKING.INI";
        public SetupPrinter()
        {
            InitializeComponent();
        }

        private async void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbbPrinterName.Text != "")
            {
                MES.OpINI.IniUtil.WriteINI(strINIPath, "PRINTER", "PrinterName", cbbPrinterName.Text );
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select printer name !!");
                return;
            }
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public ObservableCollection<string> list = new ObservableCollection<string>();
        

        private void PrinterSetup_Loaded(object sender, RoutedEventArgs e)
        {
            if (PrinterSettings.InstalledPrinters.Count <= 0)
            {
                MessageBox.Show("Printer not found!");
                return;
            }

            //Get all available printers and add them to the combo box  
            foreach (String printer in PrinterSettings.InstalledPrinters)
            {
                list.Add(printer.ToString());
            }
            cbbPrinterName.ItemsSource = list;
     
            string PrinterName = MES.OpINI.IniUtil.ReadINI(strINIPath, "PRINTER", "PrinterName", "");
            if (PrinterName != "")
            {
                cbbPrinterName.Text = PrinterName ;
            }
        }

        private void CbbPrinterName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOK_Click(sender, e);
            }
        }
    }
}
