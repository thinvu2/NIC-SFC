using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PRINT_ALLPART;
using PRINT_ALLPARTS._3Layer;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System.Windows.Forms;

namespace PRINT_ALLPARTS._3Layer
{
    public class PrintALlpartsBLL
    {
        public SfcHttpClient _sfcHttpClient;
        public PrintALlpartsBLL(SfcHttpClient sfcHttpClient )
        {
            this._sfcHttpClient = sfcHttpClient;
        }
        public string strSQL = string.Empty;
        public async Task<R105> CheckMonumberBLL(string moNumber)
        {
            strSQL = "select a.model_name,a.target_qty,a.mo_number,model_serial from sfism4.r_bpcs_moplan_t a,sfis1.c104 b where a.model_name=b.model_name and a.mo_number='" + moNumber + "' and SUBSTR(a.MO_NUMBER,0,2)='00'";

            var obj = await Form1._oracle.GetObj<R105>(strSQL);

            if (obj != null)
            {
                return obj;
            }
            return null;
        }
        public async Task<BarcodeModelRuleT> CheckPrefixLabel(string bu)
        {
            strSQL = "select BARCODE_PREFIX,YEAR_MONTH,BARCODE_LENGTH,VALID_CHAR,MACID_STEP from SFIS1.C_BARCODE_MODEL_RULE_T where model_name ='PRINT_ALLPART' AND SKU_NAME ='" + bu + "'";
            var obj = await Form1._oracle.GetObj<BarcodeModelRuleT>(strSQL);

            if (obj != null)
            {
                return obj;
            }
            return null;
        }
        public async Task<DataInputT> GetLabelPrefix(string monumber, string yearmonth, string barcodeprefix, string bu)
        {
            string YY = string.Empty;
            if (yearmonth!="YMD")
            {
                strSQL = "select to_char (sysdate,'" + yearmonth + "') YY from dual ";
            }
            else
            {
                strSQL = @"SELECT    TO_CHAR (SYSDATE, 'Y')   
                            || CASE 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '01' THEN '1' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '02' THEN '2' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '03' THEN '3'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '04' THEN '4' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '05' THEN '5' 
                            WHEN TO_CHAR (SYSDATE, 'MM') = '06' THEN '6'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '07' THEN '7'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '08' THEN '8'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '09' THEN '9'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '10' THEN 'A'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '11' THEN 'B'  
                            WHEN TO_CHAR (SYSDATE, 'MM') = '12' THEN 'C'  
                            ELSE TO_CHAR (SYSDATE, 'MM') 
                             END
                             || CASE 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '01' THEN '1' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '02' THEN '2' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '03' THEN '3'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '04' THEN '4'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '05' THEN '5' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '06' THEN '6'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '07' THEN '7'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '08' THEN '8' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '09' THEN '9' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '10' THEN 'A'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '11' THEN 'B' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '12' THEN 'C' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '13' THEN 'D'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '14' THEN 'E' 
                            WHEN TO_CHAR (SYSDATE, 'DD') = '15' THEN 'F'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '16' THEN 'G'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '17' THEN 'H'  
                            WHEN TO_CHAR (SYSDATE, 'DD') = '18' THEN 'I'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '19' THEN 'J'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '20' THEN 'K'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '21' THEN 'L'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '22' THEN 'M'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '23' THEN 'N'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '24' THEN 'O'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '25' THEN 'P'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '26' THEN 'Q'    
                            WHEN TO_CHAR (SYSDATE, 'DD') = '27' THEN 'R'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '28' THEN 'S'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '29' THEN 'T'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '30' THEN 'U'   
                            WHEN TO_CHAR (SYSDATE, 'DD') = '31' THEN 'V'   
                            ELSE TO_CHAR (SYSDATE, 'DD') 
                             END 
                            AS YY  
                            FROM DUAL ";
            }
            var data = await Form1._oracle.GetObj<DataInputT>(strSQL);
            YY = data.YY;
            strSQL = "select '" + barcodeprefix + "'||'" + YY + "' prefix,nvl(max(ssn1),'No data') lastdata, count(case PRINT_FLAG when 'Y' then 1 else null end) PrintedQty from sfism4.r_data_input_t where mo_number='" + monumber + "'";
            var obj = await Form1._oracle.GetObj<DataInputT>(strSQL);

            if (obj != null)
            {
                Form1.val_PrintedQty = obj.PrintedQty;
                return obj;
            }
            return null;
        }
        public async Task<VersionData> GetVerSion(string modelname, string monumber, string bu)
        {
            bu = bu.ToUpper().Trim();
            //strSQL = "select version From SFIS1.tblBSProductData Where ProductNo = '" + modelname + "'";
            //if (bu.StartsWith("NIC") || bu.StartsWith("ECD") || bu.StartsWith("CINTERION") || bu.StartsWith("PS5") || bu.StartsWith("HPP") || bu.StartsWith("SUPERCAP") || bu.StartsWith("SEQUANS"))
                strSQL = "SELECT HH_VER as version FROM SFIS1.C_MODEL_BRCM_VER_T WHERE MO_NUMBER =SUBSTR ('" + monumber + "',3) AND MODEL_NAME ='" + modelname + "'";
            var obj = await Form1._oracle.GetObj<VersionData>(strSQL);

            if (obj != null)
            {
                return obj;
            }
            return null;
        }
        public async Task<string> GetNextSN(string firstsnbeforeprint, string constprefix, int barcodelength, string validchar, int step)
        {
            var obj = await Form1._oracle.ExecuteProcAsync("sfis1.get_nextsn_allpart", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="firstsn",Value=firstsnbeforeprint,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="constprefix",Value=constprefix,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="barcodelength",Value=barcodelength,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="validchar",Value=validchar,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="macid_step",Value=step.ToString(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj != null)
            {
                dynamic result = obj;
                return result[0]["res"];
            }
            else
            {
                return null;
            }
        }
        public async Task<OtherData> GetFirstSnWhenPrint(string constprefix, int barcodelength)
        {
            string var_change = string.Empty;
            string totalsn = string.Empty;
            int i, j = 0;
            j = barcodelength;
            i = j - constprefix.Length;
            while (0 < i)
            {
                var_change = var_change + "0";
                i--;
            }
            totalsn = constprefix + var_change;
            strSQL = "select nvl(max(ssn1),'" + totalsn + "') beforeprint from sfism4.r_data_input_t where ssn1 like '" + constprefix + "%' ";
            var obj = await Form1._oracle.GetObj<OtherData>(strSQL);

            if (obj != null)
            {
                return obj;
            }
            return null;
        }
        public string GetFileFormServer(string UrlLabelFile, string LabelName, string FilePath)
        {

            string a = UrlLabelFile + LabelName;
            WebClient wc = new WebClient();
            try
            {
                wc.DownloadFile(a, FilePath);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<FtpData> GetFtpLabelAsync()
        {
            strSQL = "SELECT MODEL_SERIAL || CUSTOMER as url  FROM  SFIS1.C_MODEL_DESC_t WHERE UPPER (MODEL_NAME) = 'Z_LABELROOM_LH'";
            var obj = await Form1._oracle.GetObj<FtpData>(strSQL);
            if (obj != null)
            {
                return  obj;
            }
            else
            {
                return null;
            }
        }
        //public async Task<string> CheckMoExtAsync(string monumber, string modelname, string version, string beginsn, string endsn, string lengthsn)
        //{
        //    string res = string.Empty;
        //    strSQL = "select 1 from sfism4.r_mo_ext_t where ('" + beginsn + "' between item_1 and item_2) or ('" + endsn + "' between item_1 and item_2)";
        //    var obj = await Form1._oracle.GetObj<object>(strSQL);
        //    if (obj != null)
        //    {
        //        res = "DUP range SN. Please check again!";
        //    }
        //    else
        //    {
        //        strSQL = "select 1 from sfism4.R_DATA_INPUT_T where ssn1 between '" + beginsn + "' and '" + endsn + "' ";
        //        var _obj = await Form1._oracle.GetObj<object>(strSQL);
        //        if (_obj != null)
        //        {
        //            res = "DUP range SN. Please check again!";
        //        }
        //        else
        //        {
        //            res = "OK";
        //        }
        //    }
           
        //    return res;
        //}
        public async Task<string> CheckVersion(string programname, string version)
        {
            strSQL = string.Format(@"SELECT * FROM SFISM4.AMS_AP     WHERE 1=1     AND AP_NAME ='{0}'", programname);
            var result = await Form1._oracle.GetObj<ApplicationConfig>(strSQL);
            try
            {
                if (result == null)
                {
                    return "Function not exist";
                }
                else
                {
                    string DBversion = result.ap_version;
                    if (version.CompareTo(DBversion) < 1)
                    {
                        return "Wrong version, system is " + DBversion + ",program is " + version;
                    }
                    else
                    {
                        return "OK";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<string> setBu()
        {
            strSQL = "select BU,LabName,LabMode from(select distinct vr_name,VR_ITEM  as BU,vr_value as LabName,VR_DESC AS LabMode from SFIS1.C_PARAMETER_INI where prg_name = 'PRINT_ALLPART' AND VR_CLASS = 'BU' order by vr_name)";
            Form1.BuData = await Form1._oracle.GetList<BuData>(strSQL);
            return "";
        }
        public async Task<string> SaveInputDataAsync(string monumber, string sn,int qty,string ver)
        {
            var obj = await Form1._oracle.ExecuteProcAsync("sfis1.sp_print_allparts", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="step",Value="INSERTDATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="C_TYPE",Value=PrintAllpartsDTO.Instance.iBU,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="monumber",Value=monumber,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_QTY",Value=qty,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="version",Value=ver,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="beginsn",Value=sn,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="endsn",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.InputOutput },
                        new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj != null)
            {
                dynamic result = obj;
                Form1.endSN = result[0]["endsn"];
                return result[1]["res"];
            }
            else
            {
                return null;
            }
        }
        public async Task<string> ShowData(string monumber)
        {
            var obj = await Form1._oracle.ExecuteProcAsync("sfis1.sp_print_allparts", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="step",Value="SHOWDATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="C_TYPE",Value=PrintAllpartsDTO.Instance.iBU,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="monumber",Value=monumber,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_QTY",Value=0,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="version",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="beginsn",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="endsn",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.InputOutput },
                        new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj != null)
            {
                dynamic result = obj;
                Form1.endSN = result[0]["endsn"];
                return result[1]["res"];
            }
            else
            {
                return null;
            }
        }
        public async Task<string> UpdateInputDataAsync(string monumber, string panel,int qty)
        {
            var obj = await Form1._oracle.ExecuteProcAsync("sfis1.sp_print_allparts", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="step",Value="UPDATEDATAINPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="C_TYPE",Value=PrintAllpartsDTO.Instance.iBU,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="monumber",Value=monumber,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_QTY",Value=qty,SfcParameterDataType=SfcParameterDataType.Int32,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="version",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="beginsn",Value=panel,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="endsn",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.InputOutput },
                        new SfcParameter{Name="res",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj != null)
            {
                dynamic result = obj;
                return result[1]["res"];
            }
            else
            {
                return null;
            }
        }

        public async Task<string> CheckPrivilege(object loginInfo)
        {
            string jsonData = JsonConvert.SerializeObject(loginInfo).ToString();
            var obj = await Form1._oracle.ExecuteProcAsync("SFIS1.API_EXECUTE", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj != null)
            {
                dynamic result = obj;
                string a = result[0]["output"];
                return result[0]["output"];
            }
            else
            {
                return null;
            }
        }
        public async Task<DataTable> ExecuteSQL(string sql)
        {
            DataTable data;
            data = null;
            try
            {
                var datacust = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });


                if (datacust.Data != null)
                {
                    var vardatatabel = JsonConvert.SerializeObject(datacust.Data);
                    data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
                }
            }
            catch (Exception ex)
            {

            }
            return data;
        }
    }
}
