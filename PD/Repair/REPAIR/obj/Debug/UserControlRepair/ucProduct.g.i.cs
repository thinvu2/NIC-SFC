﻿#pragma checksum "..\..\..\UserControlRepair\ucProduct.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "85950163AF8351979E6BB2CD632543C3C002C1D533C5C35F90B652DCD87219FE"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using IMS.UserControlIMS;
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


namespace IMS.UserControlIMS {
    
    
    /// <summary>
    /// ucProduct
    /// </summary>
    public partial class ucProduct : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\UserControlRepair\ucProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal IMS.UserControlIMS.ucProduct frmUCproduct;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\..\UserControlRepair\ucProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnDetail;
        
        #line default
        #line hidden
        
        
        #line 95 "..\..\..\UserControlRepair\ucProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnTree;
        
        #line default
        #line hidden
        
        
        #line 131 "..\..\..\UserControlRepair\ucProduct.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid panelProduct;
        
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
            System.Uri resourceLocater = new System.Uri("/REPAIR;component/usercontrolrepair/ucproduct.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UserControlRepair\ucProduct.xaml"
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
            this.frmUCproduct = ((IMS.UserControlIMS.ucProduct)(target));
            
            #line 11 "..\..\..\UserControlRepair\ucProduct.xaml"
            this.frmUCproduct.Loaded += new System.Windows.RoutedEventHandler(this.frmUCproduct_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnDetail = ((System.Windows.Controls.Button)(target));
            
            #line 70 "..\..\..\UserControlRepair\ucProduct.xaml"
            this.btnDetail.Click += new System.Windows.RoutedEventHandler(this.btnDetail_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnTree = ((System.Windows.Controls.Button)(target));
            
            #line 103 "..\..\..\UserControlRepair\ucProduct.xaml"
            this.btnTree.Click += new System.Windows.RoutedEventHandler(this.btnTree_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.panelProduct = ((System.Windows.Controls.Grid)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

