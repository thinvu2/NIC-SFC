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
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PACK_CTN.Models;
using System.Runtime.InteropServices;

namespace PACK_CTN
{
    /// <summary>
    /// Interaction logic for IsBracket.xaml
    /// </summary>
    public partial class IsBracket : Window
    {
        private MainWindow main = new MainWindow();
        public static string carton;
        private string sqlStr;
        public string KP;
        public static string version;
        DateTime _dt = DateTime.Now;
        string sSpecialScan = "FALSE";
        char cforKeyDown = '\0';
        public IsBracket()
        {
            InitializeComponent();
            
        }
        public string RES { get; set; }
        private async void IsBracket_Loaded(object sender, RoutedEventArgs e)
        {
            lblCartonNo1.Content = lblCartonNo.Content;
            if (KP == "BRACKET")
            {
                sqlStr = "SELECT NVL(SSN7,'N/A') KP FROM SFISM4.R_CUSTSN_T WHERE SERIAL_NUMBER IN (SELECT SERIAL_NUMBER FROM SFISM4.R107 WHERE MCARTON_NO = '" + lblCartonNo.Content + "' OR CARTON_NO = '" + lblCartonNo.Content + "')";
                
            }
            if(KP=="ESD")
            {
                lblmessage.Content = "Please Scan ESD";
                sqlStr = "select ATTRIBUTE_VALUE KP from SFIS1.C_MODEL_ATTR_CONFIG_T where ATTRIBUTE_NAME='PACKCTN_CONFIRM_KP' and TYPE_VALUE='"+ lblCartonNo.Content + "' ";
            }
            if (KP == "DES")
            {
                lbltitle.Content = "MODEL_NAME";
                lblmessage.Content = "Please Scan Desiccant";
                sqlStr = "select ATTRIBUTE_VALUE KP from SFIS1.C_MODEL_ATTR_CONFIG_T where ATTRIBUTE_NAME='PACKCTN_CONFIRM_DES' and TYPE_VALUE='" + lblCartonNo.Content + "' and (VERSION ='"+ version + "' OR VERSION = 'ALL') ";
            }
            var result = await MainWindow._sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = sqlStr,
                SfcCommandType = SfcCommandType.Text
            });
            if (result.Data != null)
            {
                if (result.Data.Count() != 0)
                {
                    lblCount.Content = result.Data.Count();
                    foreach (var row in result.Data)
                    {
                        lstBK.Items.Add(row["kp"] != null? row["kp"].ToString():"");
                    }
                }
                txtKPNo.SelectAll();
                txtKPNo.Focus();
            }
        }

        private void txtKPNo_KeyUp(object sender, KeyEventArgs e)
        {
            //                 if (e.Key != Key.Return)
            //            {
            //                DateTime now = DateTime.Now;
            //                if (now.Subtract(_dt).Milliseconds > 80)
            //                {
            //                    (sender as TextBox).Text = "";
            //                }
            //    _dt = now;
            //            }
            //if (sSpecialScan == "FALSE")
            //{
            //    cforKeyDown = ToChar(e.Key);
            //    if (cforKeyDown != '\u0010')
            //        scannerText(e, txtKPNo, cforKeyDown);
            //}

            string inputData;
            if (e.Key == Key.Enter)
            {
                bool isSuccess = false;
                inputData = txtKPNo.Text;
                foreach (var temp in lstBK.Items)
                {
                    if (inputData == temp.ToString())
                    {
                        lstBK.Items.Remove(temp);
                        lblCount.Content = Convert.ToString(Convert.ToInt16(lblCount.Content) - 1);
                        lblmessage.Content = "OK";
                        txtKPNo.Focus();
                        txtKPNo.SelectAll();
                        txtKPNo.Clear();
                        isSuccess = true;
                        break;
                    }
                    else
                    {
                        isSuccess = false;
                    }

                }
                if (!isSuccess)
                {
                    lblmessage.Content = KP + " KP NOT FOUND";
                    lblmessage.Foreground = new SolidColorBrush(Colors.Red);
                    txtKPNo.Focus();
                    txtKPNo.SelectAll();
                    txtKPNo.Clear();
                }
                if (lblCount.Content.ToString() == "0")
                {
                    RES = "OK";
                    lblCartonNo1.Content = "N/A";
                    this.Close();
                }
            }
        }

        public enum MapType : uint
        {
            MAPVK_VK_TO_VSC = 0x0,
            MAPVK_VSC_TO_VK = 0x1,
            MAPVK_VK_TO_CHAR = 0x2,
            MAPVK_VSC_TO_VK_EX = 0x3,
        }

        [DllImport("user32.dll")]
        public static extern int ToUnicode(
            uint wVirtKey,
            uint wScanCode,
            byte[] lpKeyState,
            [Out,MarshalAs(UnmanagedType.LPWStr,SizeParamIndex =4)]
            StringBuilder pwszBuff,
            int cchBuff,
            uint wFlags);
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

        public static char ToChar(Key key)
        {
            char ch = ' ';
            int virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var keyboardState = new byte[256];
            GetKeyboardState(keyboardState);
            uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
            var stringBuilder = new StringBuilder(2);
            int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
            switch (result)
            {
                case -1:
                    break;
                case 0:
                    break;
                case 1:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
                default:
                    {
                        ch = stringBuilder[0];
                        break;
                    }
            }
            return ch;
        }

        static int _lastKeystroke = DateTime.Now.Millisecond;
        static List<char> _barcode = new List<char>(1);
        public static void scannerText(KeyEventArgs e, TextBox txtText, char cforKeyDown)
        {

            if (cforKeyDown != ToChar(e.Key) || cforKeyDown == '\0')
            {
                cforKeyDown = '\0';
                _barcode.Clear();
                txtText.Text = "";
                return;
            }
            var temp = (char)KeyInterop.VirtualKeyFromKey(e.Key);
            // getting the time difference between 2 keys
            int elapsed = (DateTime.Now.Millisecond - _lastKeystroke);

            /*
             * Barcode scanner usually takes less than 17 milliseconds to read, increase this if neccessary of your barcode scanner is slower
             * also assuming human can not type faster than 17 milliseconds
             */
            if (elapsed > 100)
            {
                txtText.Text = "";
                _barcode.Clear();
            }


            // Do not push in array if Enter/Return is pressed, since it is not any Character that need to be read
            if (ToChar(e.Key) != (char)Key.Return)
            {
                _barcode.Add(ToChar(e.Key));
            }

            // Barcode scanner hits Enter/Return after reading barcode
            if (ToChar(e.Key) == (char)Key.Return && _barcode.Count > 0)
            {
                txtText.Text = new String(_barcode.ToArray());

                _barcode.Clear();
            }

            // update the last key press strock time
            _lastKeystroke = DateTime.Now.Millisecond;
        }

        private void txtKPNo_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }
    }
}
