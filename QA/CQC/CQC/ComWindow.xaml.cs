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
using System.IO.Ports;

namespace CQC
{
    /// <summary>
    /// Interaction logic for ComWindow.xaml
    /// </summary>
    public partial class ComWindow : Window
    {
        SerialPort _serialPort = new SerialPort("Com1", 9600, Parity.None, 8, StopBits.One);
        private MainWindow formCQC;
        public ComWindow(MainWindow _formCQC)
        {
            formCQC = _formCQC;
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_serialPort.IsOpen)
            {
                _serialPort = new SerialPort(ComboBox1.Text, 9600, Parity.None, 8, StopBits.One);
                _serialPort.Handshake = Handshake.None;
                try
                {
                    _serialPort.Open();
                    ComboBox1.IsEnabled = false;
                    formCQC._serialPort = _serialPort;
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("can not open port " + ComboBox1.Text,"CQC");
                }
            }
            else
            {
                try
                {
                    _serialPort.Close();
                    ComboBox1.IsEnabled = true;
                }
                catch
                {
                    MessageBox.Show("can not close port1 ","CQC");
                }
            }
        }
    }
}
