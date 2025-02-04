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

namespace PACK_PALT
{
    /// <summary>
    /// Interaction logic for LabelQty.xaml
    /// </summary>
    public partial class LabelQty : Window
    {
        public static SfcHttpClient sfcHttpClient;
        private int _numValue = 0;
        public int _numtemp = 0;
        public string strINIPath;
        public LabelQty(string trINIPath)
        {
            InitializeComponent();
            this.strINIPath = trINIPath;
            //string temp = MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "QTYLABEL");
            Int32.TryParse(MES.OpINI.IniUtil.ReadINI(strINIPath, "PACK3", "QTYLABEL"), out _numValue);
            //_numValue = _numtemp;
            txtNum.Text = _numValue.ToString();
        }
        public int NumValue
        {
            get { return _numValue; }
            set
            {
                if (value >= 0)
                {
                    _numValue = value;
                    txtNum.Text = value.ToString();
                }
            }
        }
        private void CmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void CmdDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void TxtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (!int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();
        }

        private void BbtnOK_Click(object sender, RoutedEventArgs e)
        {
            MES.OpINI.IniUtil.WriteINI(strINIPath, "PACK3", "QTYLABEL", txtNum.Text);
            //MainWindow mn = new MainWindow();
            MainWindow.int_label_qty = Int32.Parse(txtNum.Text);
            this.Close();
        }
    }
}
