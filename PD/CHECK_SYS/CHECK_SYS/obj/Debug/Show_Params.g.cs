﻿#pragma checksum "..\..\Show_Params.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "71576BE805E3A07ABE0E8C994F6221E76F282756"
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
    /// Show_Params
    /// </summary>
    public partial class Show_Params : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 45 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border borderFrame;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid containerFrame;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid homeHeader;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblTitle;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblTitleFunc;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnExit;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid homeContent;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\Show_Params.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dtgShowparams;
        
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
            System.Uri resourceLocater = new System.Uri("/Make_Weight;component/show_params.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Show_Params.xaml"
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
            
            #line 9 "..\..\Show_Params.xaml"
            ((Make_Weight.Show_Params)(target)).Closed += new System.EventHandler(this.Show_Params_Closed);
            
            #line default
            #line hidden
            return;
            case 2:
            this.borderFrame = ((System.Windows.Controls.Border)(target));
            return;
            case 3:
            this.containerFrame = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.homeHeader = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.lblTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.lblTitleFunc = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.btnExit = ((System.Windows.Controls.Button)(target));
            
            #line 75 "..\..\Show_Params.xaml"
            this.btnExit.Click += new System.Windows.RoutedEventHandler(this.Show_Params_Closed);
            
            #line default
            #line hidden
            return;
            case 8:
            this.homeContent = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.dtgShowparams = ((System.Windows.Controls.DataGrid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

