﻿#pragma checksum "..\..\SetupStation.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "99D81EE8CC22BCF523C3DA04888C16E561E31A9071A3BFE37081C54DC92398BF"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Make_Weight;
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


namespace Make_Weight {
    
    
    /// <summary>
    /// SetupStation
    /// </summary>
    public partial class SetupStation : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\SetupStation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Make_Weight.SetupStation staionsetup;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\SetupStation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox cbChangeLine;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\SetupStation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbbLineName;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\SetupStation.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox IputPassword;
        
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
            System.Uri resourceLocater = new System.Uri("/Make_Weight;component/setupstation.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SetupStation.xaml"
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
            this.staionsetup = ((Make_Weight.SetupStation)(target));
            
            #line 18 "..\..\SetupStation.xaml"
            this.staionsetup.Loaded += new System.Windows.RoutedEventHandler(this.StationSetup_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.cbChangeLine = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 3:
            this.cbbLineName = ((System.Windows.Controls.ComboBox)(target));
            
            #line 40 "..\..\SetupStation.xaml"
            this.cbbLineName.KeyUp += new System.Windows.Input.KeyEventHandler(this.CbbLineName_KeyUp);
            
            #line default
            #line hidden
            return;
            case 4:
            this.IputPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 51 "..\..\SetupStation.xaml"
            this.IputPassword.KeyUp += new System.Windows.Input.KeyEventHandler(this.IputPassword_KeyUp);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 62 "..\..\SetupStation.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnOK_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 65 "..\..\SetupStation.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.btnCancle_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

