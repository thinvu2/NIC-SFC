﻿#pragma checksum "..\..\..\View\QueryData.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "DEB538FDCE16995FA23EE160DE83975CFC21A20AA26C39AD1CAD726DD1FF52C8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CodeSoft_9Codes.View;
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


namespace CodeSoft_9Codes.View {
    
    
    /// <summary>
    /// QueryData
    /// </summary>
    public partial class QueryData : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 63 "..\..\..\View\QueryData.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtMo;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\..\View\QueryData.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkUnPrint;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\View\QueryData.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkOverFlow;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\View\QueryData.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tblNotify;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\View\QueryData.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border bdInput;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\View\QueryData.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dtg;
        
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
            System.Uri resourceLocater = new System.Uri("/CodeSoft_New;component/view/querydata.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\QueryData.xaml"
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
            
            #line 8 "..\..\..\View\QueryData.xaml"
            ((CodeSoft_9Codes.View.QueryData)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.txtMo = ((System.Windows.Controls.TextBox)(target));
            
            #line 63 "..\..\..\View\QueryData.xaml"
            this.txtMo.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.txtMo_PreviewTextInput);
            
            #line default
            #line hidden
            
            #line 63 "..\..\..\View\QueryData.xaml"
            this.txtMo.KeyDown += new System.Windows.Input.KeyEventHandler(this.txtMo_KeyDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.chkUnPrint = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.chkOverFlow = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.tblNotify = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.bdInput = ((System.Windows.Controls.Border)(target));
            return;
            case 7:
            this.dtg = ((System.Windows.Controls.DataGrid)(target));
            
            #line 83 "..\..\..\View\QueryData.xaml"
            this.dtg.AutoGeneratingColumn += new System.EventHandler<System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs>(this.dtg_AutoGeneratingColumn);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

