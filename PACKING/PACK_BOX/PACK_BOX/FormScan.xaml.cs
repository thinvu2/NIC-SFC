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
using PACK_BOX.Model;

namespace PACK_BOX
{
    /// <summary>
    /// Interaction logic for FormScan.xaml
    /// </summary>
    public partial class FormScan : Window
    {
        MainWindow.getFormScan _get;
        string total_step, mstep;
        public delegate void sentToformMain(string output);
        public FormScan(MainWindow.getFormScan get)
        {
            InitializeComponent();
            _get = get;
            txt_Scan.Focus();
        }
        public void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            _get(lst_output.Text);
            this.Close();
        }

        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            INIFile ini = new INIFile("GetSCANdata.ini");
            ini.Write("TOTAL_SCAN", "TOTAL_STEP", total_step);

            for (int i = 1; i <= 9; i++)
            {
                ini.Write(mstep, "STEP_" + i.ToString(), "NULL");
            }
            if (txt_Scan.Text.Contains(":"))
            {
                string[] arrListCarton = txt_Scan.Text.Split(new char[] { ':' });
                lst_output.Text = arrListCarton[0].ToString();
            }
        }
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            INIFile ini = new INIFile("GetSCANdata.ini");
            ini.Write("TOTAL_SCAN", "TOTAL_STEP", total_step);

            for (int i = 1; i <= 9; i++)
            {
                ini.Write(mstep, "STEP_" + i.ToString(), "NULL");
            }
            if (txt_Scan.Text.Contains(":"))
            {
                string[] arrList = txt_Scan.Text.Split(new char[] { ':' });
                lst_output.Text = arrList[0].ToString();
            }
        }
    }
}
