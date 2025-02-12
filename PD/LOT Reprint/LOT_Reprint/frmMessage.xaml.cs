﻿using Sfc.Core.Parameters;
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

namespace LOT_REPRINT
{
    /// <summary>
    /// Interaction logic for frmMessage.xaml
    /// </summary>
    public partial class frmMessage : Window
    {
        public SfcHttpClient sfcClient;
        public string errorcode = "";
        public bool CustomFlag = false;
        public string MessageVietNam = "";
        public string MessageEnglish = "";
        public frmMessage()
        {
            InitializeComponent();
        }

        private async void frmMessage_Loaded(object sender, RoutedEventArgs e)
        {
            if (CustomFlag)
            {
                txtEnglish.Text = MessageEnglish;
                txtVietnamese.Text = MessageVietNam;
            }
            else
            {
                var result = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = "select*from SFIS1.C_PROMPT_CODE_T where PROMPT_CODE =:errorcode",
                    SfcCommandType = SfcCommandType.Text,
                    SfcParameters = new List<SfcParameter>
                    {
                        new SfcParameter{ Name = "errorcode", Value = errorcode }
                    }
                });

                if (result.Data != null)
                {
                    txtEnglish.Text = errorcode + " - " + result.Data["prompt_english"]?.ToString() ?? "";
                    txtVietnamese.Text = errorcode + " - " + result.Data["prompt_chinese"]?.ToString() ?? "";
                }
                else
                {
                    txtEnglish.Text = "ERROR: " + errorcode;
                    txtVietnamese.Text = "LỖI: " + errorcode;
                }
            }

            txtPassWord.Focus();
            txtPassWord.SelectAll();
        }

        private void txtPassWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                if (txtPassWord.Password == "9999")
                {
                    this.Close();
                }
                else
                {
                    txtEnglish.Text = "Please enter key: 9999";
                    txtVietnamese.Text = "Hãy nhập: 9999";
                    txtPassWord.Focus();
                    txtPassWord.SelectAll();
                }
            }
        }
    }
}
