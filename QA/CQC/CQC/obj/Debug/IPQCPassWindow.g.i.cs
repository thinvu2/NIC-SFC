﻿#pragma checksum "..\..\IPQCPassWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "E1C73358986B622F597BD186B00227F755368115426ED57352AFE2CDBC19FE79"
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
    /// IPQCPassWindow
    /// </summary>
    public partial class IPQCPassWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 55 "..\..\IPQCPassWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Edit1;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\IPQCPassWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Edit2;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\IPQCPassWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Edit3;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\IPQCPassWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox Edit4;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\IPQCPassWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BitBtn2;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\IPQCPassWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BitBtn1;
        
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
            System.Uri resourceLocater = new System.Uri("/CQC;component/ipqcpasswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\IPQCPassWindow.xaml"
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
            
            #line 17 "..\..\IPQCPassWindow.xaml"
            ((CQC.IPQCPassWindow)(target)).Initialized += new System.EventHandler(this.Window_Initialized);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Edit1 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.Edit2 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.Edit3 = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.Edit4 = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 6:
            this.BitBtn2 = ((System.Windows.Controls.Button)(target));
            
            #line 87 "..\..\IPQCPassWindow.xaml"
            this.BitBtn2.Click += new System.Windows.RoutedEventHandler(this.BitBtn2_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BitBtn1 = ((System.Windows.Controls.Button)(target));
            
            #line 103 "..\..\IPQCPassWindow.xaml"
            this.BitBtn1.Click += new System.Windows.RoutedEventHandler(this.BitBtn1_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

