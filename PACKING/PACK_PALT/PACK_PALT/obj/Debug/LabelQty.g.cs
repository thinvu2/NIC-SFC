﻿#pragma checksum "..\..\LabelQty.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "CD764A5BE46F6D17F9DFA40D7AB70A2AD758A96BC010FBC4A349FAE574915450"
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
    /// LabelQty
    /// </summary>
    public partial class LabelQty : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 45 "..\..\LabelQty.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtNum;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\LabelQty.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private System.Windows.Controls.Button cmdUp;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\LabelQty.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private System.Windows.Controls.Button cmdDown;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\LabelQty.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bbtnOK;
        
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
            System.Uri resourceLocater = new System.Uri("/PACK_PALT_NEW;component/labelqty.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\LabelQty.xaml"
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
            this.txtNum = ((System.Windows.Controls.TextBox)(target));
            
            #line 46 "..\..\LabelQty.xaml"
            this.txtNum.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.TxtNum_TextChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cmdUp = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\LabelQty.xaml"
            this.cmdUp.Click += new System.Windows.RoutedEventHandler(this.CmdUp_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cmdDown = ((System.Windows.Controls.Button)(target));
            
            #line 65 "..\..\LabelQty.xaml"
            this.cmdDown.Click += new System.Windows.RoutedEventHandler(this.CmdDown_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.bbtnOK = ((System.Windows.Controls.Button)(target));
            
            #line 71 "..\..\LabelQty.xaml"
            this.bbtnOK.Click += new System.Windows.RoutedEventHandler(this.BbtnOK_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

