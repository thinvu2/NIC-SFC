﻿#pragma checksum "..\..\IsBracket.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "BDD98B995C3A7E8C81C90DEAC8CC6BDCA9CB2E71D88B674FE8019ADB85C0E0E2"
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
using PACK_CTN;
using PACK_CTN.UserControlCTN;
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


namespace PACK_CTN {
    
    
    /// <summary>
    /// IsBracket
    /// </summary>
    public partial class IsBracket : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PACK_CTN.IsBracket IsBracketName;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lbltitle;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCartonNo1;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCartonNo;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblmessage;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtKPNo;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCount;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\IsBracket.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstBK;
        
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
            System.Uri resourceLocater = new System.Uri("/PACK_CTN;component/isbracket.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\IsBracket.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.IsBracketName = ((PACK_CTN.IsBracket)(target));
            
            #line 20 "..\..\IsBracket.xaml"
            this.IsBracketName.Loaded += new System.Windows.RoutedEventHandler(this.IsBracket_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lbltitle = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.lblCartonNo1 = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblCartonNo = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.lblmessage = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.txtKPNo = ((System.Windows.Controls.TextBox)(target));
            
            #line 63 "..\..\IsBracket.xaml"
            this.txtKPNo.KeyUp += new System.Windows.Input.KeyEventHandler(this.txtKPNo_KeyUp);
            
            #line default
            #line hidden
            
            #line 63 "..\..\IsBracket.xaml"
            this.txtKPNo.ContextMenuOpening += new System.Windows.Controls.ContextMenuEventHandler(this.txtKPNo_ContextMenuOpening);
            
            #line default
            #line hidden
            return;
            case 7:
            this.lblCount = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.lstBK = ((System.Windows.Controls.ListBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

