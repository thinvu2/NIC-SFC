﻿#pragma checksum "..\..\ShowMessageForm.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "3CCD9ED7CFF632230D3796474E2C1D0D66AC8BEF6F856EA5C13C71AE3AB3DD6C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PACKINGBOXID_CFG;
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


namespace PACKINGBOXID_CFG {
    
    
    /// <summary>
    /// ShowMessageForm
    /// </summary>
    public partial class ShowMessageForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\ShowMessageForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PACKINGBOXID_CFG.ShowMessageForm ShowMessageForm1;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\ShowMessageForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txbenglish;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\ShowMessageForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txbvietnamese;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\ShowMessageForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox passwordBox;
        
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
            System.Uri resourceLocater = new System.Uri("/PACKINGBOXID_NIC_NEW;component/showmessageform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ShowMessageForm.xaml"
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
            this.ShowMessageForm1 = ((PACKINGBOXID_CFG.ShowMessageForm)(target));
            
            #line 16 "..\..\ShowMessageForm.xaml"
            this.ShowMessageForm1.Loaded += new System.Windows.RoutedEventHandler(this.ShowMessageForm1_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txbenglish = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.txbvietnamese = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.passwordBox = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 47 "..\..\ShowMessageForm.xaml"
            this.passwordBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.passwordBox_KeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

