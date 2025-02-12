﻿#pragma checksum "..\..\Setup_Station.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "31577AE554773C68A6776C7D8E43CD24E8EDCE98FCCE4F08A052F5247C1D50E8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using PACK_PALT;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PACK_PALT {
    
    
    /// <summary>
    /// Setup_Station
    /// </summary>
    public partial class Setup_Station : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PACK_PALT.Setup_Station setup_station;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox MaterialDesignFloatingHintComboBoxEnabledComboBox;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbb_Line_name;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_section;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_group_name;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txt_station_name;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgr_group;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ok_station;
        
        #line default
        #line hidden
        
        
        #line 130 "..\..\Setup_Station.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Cancel_station;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PACK_PALT_NEW;component/setup_station.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Setup_Station.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.setup_station = ((PACK_PALT.Setup_Station)(target));
            
            #line 18 "..\..\Setup_Station.xaml"
            this.setup_station.Closed += new System.EventHandler(this.setup_station_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.MaterialDesignFloatingHintComboBoxEnabledComboBox = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 3:
            this.cbb_Line_name = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 4:
            this.txt_section = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.txt_group_name = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.txt_station_name = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.dgr_group = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 8:
            this.ok_station = ((System.Windows.Controls.Button)(target));
            
            #line 125 "..\..\Setup_Station.xaml"
            this.ok_station.Click += new System.Windows.RoutedEventHandler(this.ok_station_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Cancel_station = ((System.Windows.Controls.Button)(target));
            
            #line 133 "..\..\Setup_Station.xaml"
            this.Cancel_station.Click += new System.Windows.RoutedEventHandler(this.Cancel_station_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

