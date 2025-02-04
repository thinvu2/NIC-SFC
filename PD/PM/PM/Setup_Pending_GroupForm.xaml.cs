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
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Sfc.Library.HttpClient.Helpers;
using PM.Model;

namespace PM
{
    /// <summary>
    /// Interaction logic for Setup_Pending_GroupForm.xaml
    /// </summary>
    public partial class Setup_Pending_GroupForm : Window
    {
        public SfcHttpClient sfcClient;
        public MO_ManageForm MO_ManageForm;
        public Setup_Pending_GroupForm()
        {
            InitializeComponent();
        }
        public Setup_Pending_GroupForm(MO_ManageForm _MO_ManageForm,SfcHttpClient _sfcClient)
        {
            sfcClient = _sfcClient;
            MO_ManageForm = _MO_ManageForm;
            InitializeComponent();
            ShowTree();
        }
        private void Setup_Pending_GroupForm_Initialized(object sender, EventArgs e)
        {
            LstGroupName.Items.Clear();
            LstSectionName.Items.Clear();
        }
        private async void ShowTree()
        {
            string sSection, sGroup;
            sGroup = "";
            sSection = "";
            string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                   + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =:MO_NUMBER"
                   + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                   + " ORDER BY MO_NUMBER";
            var qry_DataMO = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="CloseFlag",Value=MO_ManageForm.MO_Status},
                    new SfcParameter{ Name="MO_NUMBER",Value=MO_ManageForm.Edt_MO.Text}
                }
            });
            Root root = new Root();
            Family family = new Family();
            List<Family> _List = new List<Family>();
            root = new Root { TitleRoot = $"{qry_DataMO.Data["default_line"].ToString()}" };
            string strGetGroup = "SELECT D.SECTION_NAME, D.GROUP_NAME FROM"
                + " (SELECT B.SECTION_NAME, A.GROUP_NEXT GROUP_NAME FROM"
                + " (select group_next from sfis1.c_route_control_t"
                + $" where route_code = '{qry_DataMO.Data["route_code"]}'"
                + " and state_flag = 0"
                + " and group_name not like 'R_%'"
                + " order by step_sequence) A, SFIS1.C_GROUP_CONFIG_T B"
                + " WHERE A.GROUP_NEXT = B.GROUP_NAME) D";
            var qry_Group = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetGroup,
                SfcCommandType = SfcCommandType.Text
            });
            dynamic ads = qry_Group.Data;
            for (int i = 0; i < qry_Group.Data.Count(); i++)
            {
                if (ads[i]["section_name"] != sSection)
                {
                    family = new Family { Title = $"{ads[i]["section_name"]}" };
                    _List.Add(family);
                    sSection = ads[i]["section_name"];
                }
                if (ads[i]["group_name"] != sGroup)
                {
                    foreach (Family _fm in _List)
                    {
                        _fm.Members.Add(new FamilyMemebr() { Name = $"{ads[i]["group_name"]}" });
                        sGroup = ads[i]["group_name"];
                    }
                }
                LstSectionName.Items.Add(ads[i]["section_name"]);
                LstGroupName.Items.Add(ads[i]["group_name"]);
            }
            foreach (Family fm in _List)
            {
                root.ItemsRoot.Add(fm);
            }
            Treeview_Group.Items.Add(root);
        }
        private void btn_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btn_OK(object sender, RoutedEventArgs e)
        {
            if (LstSectionName.Items.Count == 0)
            {
                MessageBoxResult result = MessageBox.Show("You had not choose any group to pending!", "PM", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
            this.Close();
        }
    }
}
