﻿#pragma checksum "..\..\CheckLabelForm.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "6F989F00B37E6C84746CD68916FFC5FC15532DC1708D3BE4FAC21D71B2835FB4"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CodeSoft_9Codes;
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


namespace CodeSoft_9Codes {
    
    
    /// <summary>
    /// CheckLabelForm
    /// </summary>
    public partial class CheckLabelForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CodeSoft_9Codes.CheckLabelForm Passform;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox txtPassword;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblName;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblMO;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblLabelType;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtLabel;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtMsgV;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\CheckLabelForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtMsgE;
        
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
            System.Uri resourceLocater = new System.Uri("/CodeSoft_New;component/checklabelform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CheckLabelForm.xaml"
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
            this.Passform = ((CodeSoft_9Codes.CheckLabelForm)(target));
            return;
            case 2:
            this.txtPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 40 "..\..\CheckLabelForm.xaml"
            this.txtPassword.KeyUp += new System.Windows.Input.KeyEventHandler(this.txtPassword_KeyUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lblName = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblMO = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.lblLabelType = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.txtLabel = ((System.Windows.Controls.TextBox)(target));
            
            #line 83 "..\..\CheckLabelForm.xaml"
            this.txtLabel.KeyUp += new System.Windows.Input.KeyEventHandler(this.txtLabel_KeyUp);
            
            #line default
            #line hidden
            return;
            case 7:
            this.txtMsgV = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.txtMsgE = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

