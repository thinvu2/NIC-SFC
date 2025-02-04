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
using Sfc.Core.Parameters;
using PM.Model;

namespace PM
{
    /// <summary>
    /// Interaction logic for CheckConfigForm.xaml
    /// </summary>
    public partial class CheckConfigForm : Window
    {
        public SfcHttpClient sfcClient;
        MO_InsertForm frm_MO_InsertForm;
        List<ListDepartment> _ListDepartment = new List<ListDepartment>();
        public string Model, Version, Motype;
        public CheckConfigForm()
        {
            InitializeComponent();
        }
        public CheckConfigForm(MO_InsertForm _frm_MO_InsertForm,SfcHttpClient _sfcClient,string _Model,string _Version,string _MoType)
        {
            frm_MO_InsertForm = _frm_MO_InsertForm;
            sfcClient = _sfcClient;
            Model = _Model;
            Version = _Version;
            Motype = _MoType;
            InitializeComponent();
            CheckConfigForm_FormShow();
        }
        private async void CheckConfigForm_FormShow()
        {
            string sLanguage;
            INIFile ini = new INIFile("SFIS.ini");
            Lv_DataContent.ItemsSource = null;
            _ListDepartment.Clear();
            sLanguage = ini.Read("LANGUAGES", "LANGUAGE");
            if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
            {
                Column_1.Header = "Config Item";
                Column_2.Header = "Status";
                Column_3.Header = "Department";
            }
            else
            {
                Column_1.Header = "Config Item";
                Column_2.Header = "Condition";
                Column_3.Header = "Responsibility Unit";
            }
            string strGetCust_SnRule = $"select * from sfis1.c_cust_snrule_t where model_name='{Model}' and version_code='{Version}'";
            var qry_Cust_SnRule = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCust_SnRule,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Cust_SnRule.Data == null)
            {
                if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config19", ITEM_2 = "Not Setup", ITEM_3 = "IT" });
                }
                else
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config19", ITEM_2 = "No Set", ITEM_3 = "IT" });
                }
                //await MailInsert("CONFIG19", $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time!!",
                //    $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time,prevent production delay");
            }

            string strGetBOM = "";
            if (Motype == "Rework")
            {
                strGetBOM = $"select * from sfis1.c_bom_keypart_t where bom_no='{Model}RW'";
                var qry_BOM = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetBOM,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_BOM.Data == null)
                {
                    if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
                    {
                        _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config12", ITEM_2 = "Not Setup", ITEM_3 = "IE" });
                    }
                    else
                    {
                        _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config12", ITEM_2 = "No Set", ITEM_3 = "IE" });
                    }
                }
            }
            else
            {
                strGetBOM = $"select * from sfis1.c_bom_keypart_t where bom_no='{Model}'";
                var qry_BOM = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
                {
                    CommandText = strGetBOM,
                    SfcCommandType = SfcCommandType.Text
                });
                if (qry_BOM.Data == null)
                {
                    if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
                    {
                        _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config12", ITEM_2 = "Not Setup", ITEM_3 = "IE" });
                    }
                    else
                    {
                        _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config12", ITEM_2 = "No Set", ITEM_3 = "IE" });
                    }
                }
            }
            //await MailInsert("CONFIG19", $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time!!",
            //    $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time,prevent production delay");

            string strGetParam = $"select * from sfis1.C_PACK_PARAM_T where model_name='{Model}' and version_code='{Version}'";
            var qry_Param = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetParam,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Param.Data == null)
            {
                if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config15", ITEM_2 = "Not Setup", ITEM_3 = "IE" });
                }
                else
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config15", ITEM_2 = "No Set" , ITEM_3 = "IE" });
                }
                //await MailInsert("CONFIG19", $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time!!",
                //    $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time,prevent production delay");
            }

            string strGetSAP = $"select * from sfis1.c_group_sap_mapping_t where model_name='{Model}'";
            var qry_SAP = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetSAP,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_SAP.Data == null)
            {
                if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config42", ITEM_2 = "Not Setup", ITEM_3 = "IE" });
                }
                else
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config42", ITEM_2 = "No Set", ITEM_3 = "IE" });
                }
                //await MailInsert("CONFIG19", $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time!!",
                //    $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time,prevent production delay");
            }

            string strGetSequence = $"select * from sfis1.c_pack_sequence_t where model_name='{Model}'"
                 + $" and version_code='{Version}' and mo_type='{Motype}'";
            var qry_Sequence = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetSequence,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_Sequence.Data == null)
            {
                if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config43", ITEM_2 = "Not Setup", ITEM_3 = "IE" });
                }
                else
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config43", ITEM_2 = "No Se", ITEM_3 = "IE" });
                }
                //await MailInsert("CONFIG19", $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time!!",
                //    $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time,prevent production delay");
            }

            string strGetCustSNRule = $"select * from sfis1.c_custsn_rule_t where model_name='{Model}'"
                + $" and version_code='{Version}' and mo_type='{Motype}'";
            var qry_CustSNRule = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustSNRule,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_CustSNRule.Data == null)
            {
                if ((string.IsNullOrEmpty(sLanguage)) || (sLanguage == "C"))
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config44", ITEM_2 = "Not Setup", ITEM_3 = "IE" });
                }
                else
                {
                    _ListDepartment.Add(new ListDepartment { ITEM_1 = "Config44", ITEM_2 = "No Set", ITEM_3 = "IE" });
                }
                //await MailInsert("CONFIG19", $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time!!",
                //    $"Model: {Model} ;Version: {Version};Mo_Type: {Motype} -- Config 19 have not configure, please set in time,prevent production delay");
            }
            Lv_DataContent.ItemsSource = _ListDepartment;
        }
        public async Task<bool> MailInsert(string _Sendwho, string _Mailtext1, string _Mailtext2)
        {
            string MailList;
            string strGetCustomer = "SELECT count(*) AA FROM  SFIS1.C_CUSTOMER_FTP_ACCOUNT_T WHERE CUSTOMER_CODE=:MCUSTOMER_CODE";
            var qry_Customer = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustomer,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MCUSTOMER_CODE",Value=_Sendwho}
                }
            });
            if (Int32.Parse(qry_Customer.Data["AA"].ToString()) == 0)
            {
                return false;
            }
            string strGetCustomer_FTP = "SELECT * FROM SFIS1.C_CUSTOMER_FTP_ACCOUNT_T WHERE CUSTOMER_CODE=:MCUSTOMER_CODE";
            var qry_Customer_FTP = await sfcClient.QueryListAsync(new QuerySingleParameterModel
            {
                CommandText = strGetCustomer_FTP,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{Name="MCUSTOMER_CODE",Value=_Sendwho}
                }
            });
            foreach (var row in qry_Customer_FTP.Data)
            {
                MailList = row["email_list"].ToString();
                try
                {
                    //------ Save To Mail ------//
                    string strInsert_Mail = "Insert into SFIS1.C_MAIL_T "
                            + " (MAIL_ID, MAIL_TO, MAIL_FROM,  MAIL_SUBJECT, MAIL_SEQUENCE,"
                            + " MAIL_CONTENT, MAIL_FLAG, MAIL_PROGRAM)"
                            + " VALUES ("
                            + " :MAIL_ID,:MAIL_TO,:MAIL_FROM,:MAIL_SUBJECT,:MAIL_SEQUENCE,:MAIL_CONTENT,:MAIL_FLAG,:MAIL_PROGRAM)";
                    var Insert_Mail = await sfcClient.QueryListAsync(new QuerySingleParameterModel
                    {
                        CommandText = strInsert_Mail,
                        SfcCommandType = SfcCommandType.Text,
                        SfcParameters = new List<SfcParameter>()
                        {
                            new SfcParameter{Name="MAIL_ID",Value=System.DateTime.Now.ToString("yyyymmddhhnnsszzz")},
                            new SfcParameter{Name="MAIL_TO",Value=MailList},
                            new SfcParameter{Name="MAIL_FROM",Value="SFIS_CONFIG"},
                            new SfcParameter{Name="MAIL_SUBJECT",Value=_Mailtext1},
                            new SfcParameter{Name="MAIL_SEQUENCE",Value="0"},
                            new SfcParameter{Name="MAIL_CONTENT",Value=_Mailtext2},
                            new SfcParameter{Name="MAIL_FLAG",Value="0"},
                            new SfcParameter{Name="MAIL_PROGRAM",Value="PM"}
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return true;
        }
        private void CheckConfigForm_Close(object sender, EventArgs e)
        {
            MessageBox.Show("00623 - Du lieu chua thiet lap, vui long lien he IE va SFC -- Having data didn't set, please notify IE and SFC", "PM", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
    }
}
