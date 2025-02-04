using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CHECK_SYS.DataObject;
namespace CHECK_SYS
{
    public class DAL
    {
        public SfcHttpClient _sfcHttpClient;
        private OracleClient _oracle;
        public DAL(SfcHttpClient sfcHttpClient)
        {
            this._sfcHttpClient = sfcHttpClient;
            _oracle = new OracleClient(sfcHttpClient);
        }
        public string strSQL = string.Empty;
       

        public async Task<string> GetVersionMatch(string apName)
        {
            strSQL = string.Format(@" SELECT AP_VERSION FROM SFIS1.C_AMS_PATTERN_T WHERE  AP_NAME = '{0}'", apName);
            var result = await _oracle.GetObj<AMS_AP>(strSQL);
            try
            {
                if (result == null)
                {
                    return "Program not exist|Chương trình không tồn tại!";
                }
                else
                {
                    return result.ap_version.Trim();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

       

        public async Task<string> CheckLogin(string inData)
        {
            string res = "";
            var obj_check = await _oracle.ExecuteProcAsync("SFIS1.SP_CHECKSYS", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="IN_DATA",Value=inData.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj_check != null)
            {
                dynamic result = obj_check;
                return (result[1]["res"]);
            }
            else
            {
                return res;
            }
        }


         

        public async Task<string> CheckCartonNo(string inData)
        {
            try
            {
                string res = "";
                var obj_check = await _oracle.ExecuteProcAsync("SFIS1.SP_CHECKSYS", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="IN_DATA",Value=inData.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output},
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });

                if (obj_check != null)
                {
                    dynamic result = obj_check;
                    res = result[1]["res"];
                    if (!"OK".Equals(res) || res != "OK")
                    {
                        return res;
                    }
                    else
                    {
                        return "OK";
                    }
                }
                else
                {
                    return res;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }

    }
}
