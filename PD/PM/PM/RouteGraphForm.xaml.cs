using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using Sfc.Core.Parameters;
using PM.Model;
using System.Collections.ObjectModel;
using System.Reflection;

namespace PM
{
    /// <summary>
    /// Interaction logic for RouteGraphForm.xaml
    /// </summary>
    public partial class RouteGraphForm : Window
    {
        public SfcHttpClient sfcClient;
        public MO_InsertForm frm_MO_InsertForm;
        public RouteGraphForm()
        {
            InitializeComponent();
        }
        public RouteGraphForm(MO_InsertForm _frm_MO_InsertForm,SfcHttpClient _sfcClient)
        {
            frm_MO_InsertForm = _frm_MO_InsertForm;
            sfcClient = _sfcClient;
            InitializeComponent();
        }
        private async void RouteGraphForm_Initialized(object sender, EventArgs e)
        {
            string sGroupName;
            bool bBackFlag;

            string strGetRouteName = "SELECT ROUTE_CODE FROM SFIS1.C_ROUTE_NAME_T where route_name = :RouteName";
            var qry_RouteName = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetRouteName,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="RouteName",Value=frm_MO_InsertForm.Cbb_Route.Text}
                }
            });

            if(qry_RouteName.Data == null)
            {
                this.Close();
                return;
            }

            string strGetRouteCode = $"select * from sfis1.c_route_control_t where route_code='{qry_RouteName.Data["route_code"].ToString()}' order by step_sequence";
            var qry_RouteCode = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetRouteCode,
                SfcCommandType = SfcCommandType.Text
            });
            sGroupName = "";
            bBackFlag = false;
            dynamic ads = qry_RouteCode.Data;
            Family root = new Family();
            List<Family> _List = new List<Family>();
            for (int i = 0; i < qry_RouteCode.Data.Count(); i++)
            {
                if ((ads[i]["state_flag"] == 0) && (ads[i]["group_name"] == "0"))
                {
                    if (bBackFlag == true)
                    {
                    }
                    else
                    {
                        if (ads[i]["group_next"].ToString() != sGroupName)
                        {
                            root = new Family { Title = $"{ads[i]["group_next"]}" };
                            _List.Add(root);
                        }
                    }
                }
                if ((ads[i]["state_flag"] == 0) && (ads[i]["group_name"] != "0") && (ads[i]["group_name"].Substring(0, 2) != "R_") )
                {
                    if (bBackFlag == true)
                    {
                        for (int j = 0; j < TreeView_Route.Items.Count; j++)
                        {
                        }
                    }
                    else
                    {
                        if (ads[i]["group_next"].ToString() != sGroupName)
                        {
                            root = new Family() { Title = $"{ads[i]["group_next"]}" };
                            _List.Add(root);
                        }
                    }
                }
                if (ads[i]["state_flag"] == 1)
                {
                    //add branch to root
                    foreach (Family _fm in _List)
                    {
                        if (_fm.Title == ads[i]["group_name"])
                        {
                            _fm.Members.Add(new FamilyMemebr() { Name = $"{ads[i]["group_next"]}" });
                        }
                        bBackFlag = true;
                    }
                }
                sGroupName = ads[i]["group_next"];
            }
            //add all item in List to treeview
            foreach (Family fm in _List)
            {
                TreeView_Route.Items.Add(fm);
            }
        }
        private void RouteGraphForm_Close(object sender, EventArgs e)
        {
            if (frm_MO_InsertForm.Lb_Mess.Content.ToString() == "0")
            {
                MessageBox.Show("Xac nhan lai Luu trinh -- Please check Route", "PM", MessageBoxButton.OK, MessageBoxImage.Question);
            }
        }
    }
}
