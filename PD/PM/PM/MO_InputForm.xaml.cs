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
    /// Interaction logic for MO_InputForm.xaml
    /// </summary>
    public partial class MO_InputForm : Window
    {
        public SfcHttpClient sfcClient;
        public MO_ManageForm frm_MO_ManageForm;
        public int iRouteCode, iMaxQTY, iSNLen, iCurrentNOLen, iCurrentNOStartDigit;
        public string sVersionCode, sKPNO, sBomNO, sCusCode, MO_OPTION, sCurrentNOType;
        public MO_InputForm()
        {
            InitializeComponent();
        }
        public MO_InputForm(MO_ManageForm _frm_MO_ManageForm, SfcHttpClient _sfcClient)
        {
            frm_MO_ManageForm = _frm_MO_ManageForm;
            sfcClient = _sfcClient;
            InitializeComponent();
            MO_InputForm_FormShow();
        }
        private async void MO_InputForm_FormShow()
        {
            //int iSNLen, iCurrentNOLen, iCurrentNOStartDigit;
            string strGetDataMO = "SELECT r105.*,crn.route_name FROM SFISM4.R_MO_BASE_T r105, SFIS1.C_ROUTE_NAME_T crn"
                + " WHERE CLOSE_FLAG LIKE :CloseFlag and MO_NUMBER =: MO_NUMBER"
                + " and r105.route_code = crn.route_code AND close_flag <> '9'"
                + " ORDER BY MO_NUMBER";
            var qry_DataMO = await sfcClient.QueryListAsync(new QuerySingleParameterModel()
            {
                CommandText = strGetDataMO,
                SfcCommandType = SfcCommandType.Text,
                SfcParameters = new List<SfcParameter>()
                {
                    new SfcParameter{ Name="CloseFlag",Value=frm_MO_ManageForm.MO_Status},
                    new SfcParameter{ Name="MO_NUMBER",Value=frm_MO_ManageForm.Edt_MO.Text}
                }
            });
            List<ListMO> result = qry_DataMO.Data.ToListObject<ListMO>().ToList();
            Edt_MO.Text = result[0].MO_NUMBER;
            Edt_MoDel.Text = result[0].MODEL_NAME;
            Edt_Line.Text = result[0].DEFAULT_LINE;
            Edt_TargetQty.Text = result[0].TARGET_QTY.ToString();
            Edt_InputQty.Text = result[0].INPUT_QTY.ToString();
            Edt_NotInQty.Text = (float.Parse(result[0].TARGET_QTY.ToString()) - float.Parse(result[0].INPUT_QTY.ToString())).ToString();
            Edt_StartSN.Text = result[0].START_SN;
            Edt_EndSN.Text = result[0].END_SN;
            Edt_InGroup.Text = result[0].DEFAULT_GROUP;
            Edt_OutGroup.Text = result[0].END_GROUP;
            iRouteCode = Int32.Parse(result[0].ROUTE_CODE);
            sVersionCode = result[0].VERSION_CODE;
            sKPNO = result[0].KEY_PART_NO;
            sBomNO = result[0].BOM_NO;
            sCusCode = result[0].CUST_CODE;
            MO_OPTION = result[0].MO_OPTION;

            iSNLen = await GetIniIntValue("PM", MO_OPTION, "LENGTH", "SN",14);
            iCurrentNOLen = await GetIniIntValue("PM", MO_OPTION, "LENGTH", "CurrentNO", 5);
            iCurrentNOStartDigit = await GetIniIntValue("PM", MO_OPTION, "LENGTH", "CurrentNOStartDigit", iSNLen - iCurrentNOLen + 1);
            if (await GetIniStrValue("PM", MO_OPTION, "Type", "CurrentNO", "Numerals") == "Numerals")
            {
                if (await GetIniStrValue("PM", MO_OPTION, "Type", "Decimal", "decimal") == "decimal")
                {
                    sCurrentNOType = "decimal";
                }else if (await GetIniStrValue("PM", MO_OPTION, "Type", "Decimal", "decimal") == "hexadecimal")
                {
                    sCurrentNOType = "hexadecimal";
                }
            }
            else
            {

            }
        }
        private async Task<int> GetIniIntValue(string _sPrg, string _sVClass, string _sVItem, string _sVName, int _iNum)
        {
            string strGetIniIntValue = $"SELECT VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = '{_sPrg}'"
                + $" AND VR_CLASS = '{_sVClass}' AND VR_ITEM = '{_sVItem}' AND VR_NAME = '{_sVName}'";
            var qry_IniIntValue = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetIniIntValue,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_IniIntValue.Data == null)
            {
                return _iNum;
            }
            else
            {
                return Int32.Parse(qry_IniIntValue.Data["vr_value"].ToString());
            }
        }
        private async Task<string> GetIniStrValue(string _sPrg, string _sVClass, string _sVItem, string _sVName, string _sNum)
        {
            string strGetIniStrValue = $"SELECT VR_VALUE FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = '{_sPrg}'"
                + $" AND VR_CLASS = '{_sVClass}' AND VR_ITEM = '{_sVItem}' AND VR_NAME = '{_sVName}'";
            var qry_IniStrValue = await sfcClient.QuerySingleAsync(new QuerySingleParameterModel
            {
                CommandText = strGetIniStrValue,
                SfcCommandType = SfcCommandType.Text
            });
            if (qry_IniStrValue.Data == null)
            {
                return _sNum;
            }
            else
            {
                return qry_IniStrValue.Data["vr_value"].ToString();
            }
        }
    }
}
