﻿using System;
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

namespace PACKINGBOXID_CFG
{
    /// <summary>
    /// Interaction logic for ShowParamForm.xaml
    /// </summary>
    public partial class ShowParamForm : Window
    {
        public string sUrlFile;
        public ShowParamForm()
        {
            InitializeComponent();
        }
        private void PACKINGBOXID_CFG_Loaded(object sender, RoutedEventArgs e)
        {
            UrlLabel.Text = sUrlFile;
        }
    }
}
