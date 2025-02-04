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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PACK_PALT.ViewModel;
using System.IO;
using System.Diagnostics;
using PACK_PALT.Resource;

namespace PACK_PALT.UserControlCTN
{
    /// <summary>
    /// Interaction logic for ControlBarUC.xaml
    /// </summary>
    public partial class ControlBarUC : UserControl
    {
        public ControlBarViewModel ViewModel { get; set; }
        public ControlBarUC()
        {
            InitializeComponent();
            this.DataContext = ViewModel = new ControlBarViewModel();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            string _DirPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\";

            foreach (string file in Directory.GetFiles(_DirPath, "*.lab"))
            {

                
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    List<Process> lstProcs = new List<Process>();
                    lstProcs = FileUtil.WhoIsLocking(file);

                    foreach (Process p in lstProcs)
                    {
                        try
                        {
                            ProcessHandler.localProcessKill(p.ProcessName);
                        }
                        catch 
                        {
                           

                        }
                    }
                    try
                    {
                        File.Delete(file);
                    }
                    catch 
                    {
                        
                    }
                }
            }
            foreach (string file in Directory.GetFiles(_DirPath, "*.bak"))
            {

                File.Delete(file);
            }
            Application.Current.Shutdown();
        }
    }
}
