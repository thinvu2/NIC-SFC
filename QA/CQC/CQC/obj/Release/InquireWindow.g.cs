﻿#pragma checksum "..\..\InquireWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "421181600F79E70BC77087B5BF3391556CA3D75AAABAE2B651F6C95AA8400BC0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CQC;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
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


namespace CQC {
    
    
    /// <summary>
    /// InquireWindow
    /// </summary>
    public partial class InquireWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 32 "..\..\InquireWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lablTitle;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\InquireWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox editUnit;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\InquireWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bbtnPrint;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\InquireWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bbtnClose;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\InquireWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid DBGrid1;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\InquireWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGridTextColumn pallet;
        
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
            System.Uri resourceLocater = new System.Uri("/CQC;component/inquirewindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\InquireWindow.xaml"
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
            
            #line 17 "..\..\InquireWindow.xaml"
            ((CQC.InquireWindow)(target)).Initialized += new System.EventHandler(this.Window_Initialized);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lablTitle = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.editUnit = ((System.Windows.Controls.TextBox)(target));
            
            #line 37 "..\..\InquireWindow.xaml"
            this.editUnit.KeyDown += new System.Windows.Input.KeyEventHandler(this.editUnit_KeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.bbtnPrint = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\InquireWindow.xaml"
            this.bbtnPrint.Click += new System.Windows.RoutedEventHandler(this.bbtnPrint_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.bbtnClose = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\InquireWindow.xaml"
            this.bbtnClose.Click += new System.Windows.RoutedEventHandler(this.bbtnClose_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.DBGrid1 = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.pallet = ((System.Windows.Controls.DataGridTextColumn)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

