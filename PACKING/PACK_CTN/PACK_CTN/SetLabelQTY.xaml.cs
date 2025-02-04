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

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for SetLabelQTY.xaml
    /// </summary>
    public partial class SetLabelQTY : Window
    {
        public string strINIPath = "C:\\PACKING\\PACKING.INI";
        public SetLabelQTY()
        {
            InitializeComponent();
        }

        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            MES.OpINI.IniUtil.WriteINI(strINIPath, "SLabelQTY", "Default",cbbsnQty.Text);
            MES.OpINI.IniUtil.WriteINI(strINIPath, "CLabelQTY", "Default", cbbLabelQty.Text);
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PLabelQTY", "Default", cbbMaxQty.Text);
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PRINT_LEFT", "Default", cbbMarginLeft.Text);
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PRINT_TOP", "Default", cbbMarginTop.Text);
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PRINT_Vertical", "Default", cbbVertical.Text);
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PRINT_Horizontal", "Default", cbbHorixontal.Text);
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public ObservableCollection<string> list = new ObservableCollection<string>();
        private void SetLabelQTY_Loaded(object sender, RoutedEventArgs e)
        {
            
            int i;
            for (i = 1; i <= 50 ; i++)
            {
                //foreach (string s in str)
                list.Add(i.ToString());
               // cbbLabelQty.Items.Add(i.ToString() );
            }
            cbbLabelQty.ItemsSource = list ;
            cbbLabelQty.SelectedIndex = 0;
            cbbMaxQty.ItemsSource = list;
            cbbMaxQty.SelectedIndex = 0;
            cbbsnQty.ItemsSource = list;
            cbbsnQty.SelectedIndex = 0;
            cbbVertical.ItemsSource = list;
            cbbVertical.SelectedIndex = 0;
            cbbMarginTop.ItemsSource = list;
            cbbMarginTop.SelectedIndex = 0;
            cbbMarginLeft.ItemsSource = list;
            cbbMarginLeft.SelectedIndex = 0;
            cbbHorixontal.ItemsSource = list;
            cbbHorixontal.SelectedIndex = 0;
            
            cbbsnQty.Text = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "SLabelQTY", "Default", 1).ToString();
            cbbLabelQty.Text = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "CLabelQTY", "Default", 1).ToString();
            cbbMaxQty.Text = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PLabelQTY", "Default", 1).ToString();
            cbbVertical.Text = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PRINT_Vertical", "Default", 0).ToString();
            cbbMarginTop.Text = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PRINT_TOP", "Default", 0).ToString();
            cbbMarginLeft.Text = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PRINT_LEFT", "Default", 0).ToString();
            cbbHorixontal.Text = MES.OpINI.IniUtil.ReadINI_Int(strINIPath, "PRINT_Horizontal", "Default", 0).ToString();
        }
    }
}
