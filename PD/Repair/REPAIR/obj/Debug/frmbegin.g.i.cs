// Updated by XamlIntelliSenseFileGenerator 11/21/2022 8:18:42 AM
#pragma checksum "..\..\frmbegin.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B575F39485421A5ED1BBA8BF8B5CA2E936D44F2225669EC4F8C563241E63AD02"
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
using REPAIR;
using REPAIR.UserControlRepair;
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
using XamlAnimatedGif;


namespace REPAIR
{


    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class begin : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/REPAIR;component/frmbegin.xaml", System.UriKind.Relative);

#line 1 "..\..\frmbegin.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler)
        {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.mainWindow = ((REPAIR.MainWindow)(target));

#line 17 "..\..\frmbegin.xaml"
                    this.mainWindow.Loaded += new System.Windows.RoutedEventHandler(this.MainWindow_Loaded);

#line default
#line hidden
                    return;
                case 2:
                    this.itemConfig = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 3:
                    this.itemRework = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 4:
                    this.itemSSN = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 5:
                    this.itemSN = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 6:
                    this.itemSN2 = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 7:
                    this.itemAll = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 8:
                    this.itemSSN2 = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 9:
                    this.itemPoNo = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 10:
                    this.itemInputSN = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 11:
                    this.itemInputSNbyXLS = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 12:
                    this.itemAddErrorCode = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 13:
                    this.itemDeleteErrorCode = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 14:
                    this.itemAddRepairRecord = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 15:
                    this.itemModifyWip = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 16:
                    this.itemDeleteRepairRecord = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 17:
                    this.itemFinishRepair = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 18:
                    this.itemChangeKeyPart = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 19:
                    this.itemRepairReport = ((System.Windows.Controls.MenuItem)(target));
                    return;
                case 20:
                    this.lblLeadFree = ((System.Windows.Controls.Label)(target));
                    return;
                case 21:
                    this.tblWipGroup = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 22:
                    this.lblEmpName = ((System.Windows.Controls.Label)(target));
                    return;
                case 23:
                    this.tblSN = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 24:
                    this.tblS_SN = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 25:
                    this.tblMO = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 26:
                    this.tblLine = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 27:
                    this.tblModel = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 28:
                    this.tblPartNO = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 29:
                    this.tblVersion = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 30:
                    this.tblGroup = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 31:
                    this.tblInLine = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 32:
                    this.tblStation = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 33:
                    this.tblPMCC = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 34:
                    this.lblQtyError = ((System.Windows.Controls.Label)(target));
                    return;
                case 35:
                    this.listErrorCode = ((System.Windows.Controls.ListBox)(target));
                    return;
                case 36:
                    this.tblDefectTime = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 37:
                    this.tblDfstation = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 38:
                    this.tbErroCode = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 39:
                    this.tbDescription = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 40:
                    this.gridDataRepair = ((System.Windows.Controls.DataGrid)(target));

#line 370 "..\..\frmbegin.xaml"
                    this.gridDataRepair.AutoGeneratingColumn += new System.EventHandler<System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs>(this.dgr_AutoGeneratingColumn);

#line default
#line hidden
                    return;
                case 41:
                    this.btnNew = ((System.Windows.Controls.Button)(target));
                    return;
                case 42:
                    this.btnRemove = ((System.Windows.Controls.Button)(target));
                    return;
                case 43:
                    this.btnAdd = ((System.Windows.Controls.Button)(target));
                    return;
                case 44:
                    this.btnModify = ((System.Windows.Controls.Button)(target));
                    return;
                case 45:
                    this.btnDelete = ((System.Windows.Controls.Button)(target));
                    return;
                case 46:
                    this.btnClose = ((System.Windows.Controls.Button)(target));

#line 537 "..\..\frmbegin.xaml"
                    this.btnClose.Click += new System.Windows.RoutedEventHandler(this.btnClose_Click);

#line default
#line hidden
                    return;
                case 47:
                    this.btnFinish = ((System.Windows.Controls.Button)(target));
                    return;
                case 48:
                    this.btnExit = ((System.Windows.Controls.Button)(target));

#line 589 "..\..\frmbegin.xaml"
                    this.btnExit.Click += new System.Windows.RoutedEventHandler(this.btnExit_Click_1);

#line default
#line hidden
                    return;
                case 49:
                    this.lblVerson = ((System.Windows.Controls.Label)(target));
                    return;
                case 50:
                    this.lblDBName = ((System.Windows.Controls.Label)(target));
                    return;
                case 51:
                    this.lblIP = ((System.Windows.Controls.Label)(target));
                    return;
                case 52:
                    this.lblMAC = ((System.Windows.Controls.Label)(target));
                    return;
            }
            this._contentLoaded = true;
        }

        internal System.Windows.Window mainWindow;
    }
}

