using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Make_Weight.DataObject;
namespace Make_Weight
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
        public async Task<string> CheckVersion(string programname, string version)
        {
            strSQL = string.Format(@"SELECT ap_name,ap_version FROM SFIS1.C_AMS_PATTERN_T     WHERE 1=1     AND AP_NAME ='{0}'", programname);
            var result = await _oracle.GetObj<AMS_AP>(strSQL);
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


        public async Task<string> checkClose(string ctn)
        {
            try
            {
                var obj = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select PALLET_NO from SFIS1.C_PALLET_T WHERE PALLET_NO='" + ctn + "' OR   CUST_PALLET_NO= '" + ctn + "' ",
                    SfcCommandType = SfcCommandType.Text,
                });
                if (obj.Data != null && obj.Data.Count() > 0)
                {
                    return "NG";
                }
                else
                {
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return "NG";
            }
        }


        public async Task<string> checkDifWIP(string ctn)
        {
            try
            {
                var obj = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select distinct wip_group,mcarton_no from sfism4.r107 where mcarton_no= '" + ctn + "' ",
                    SfcCommandType = SfcCommandType.Text,
                });
                if (obj.Data != null && obj.Data.Count() > 0)
                {
                    if (obj.Data.Count() > 1)
                    {
                        return "NG";
                    }
                    else
                    {
                        return "OK";
                    }
                }
                else
                {
                    return "NG";
                }
            }
            catch (Exception ex)
            {
                return "NG";
            }
        }




        public async Task<string> GetVersionMatch(string apName)
        {
            strSQL = string.Format(@" SELECT AP_VERSION FROM   SFIS1.C_AMS_PATTERN_T WHERE  AP_NAME = '{0}'", apName);
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


        //public async Task<string> IsSupercap(string ctn)
        //{
        //    try
        //    {
        //        var obj = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
        //        {
        //            CommandText = "select DISTINCT  ATTRIBUTE_VALUE from SFIS1.C_MODEL_ATTR_CONFIG_T a,SFISM4.R107 b where b.MODEL_NAME = a.TYPE_VALUE and a.TYPE_NAME = 'MODEL_NAME' and a.ATTRIBUTE_NAME = 'CONFIRM_AT_CTNWEIGHT' and b.MCARTON_NO = '"+ctn+"' ",
        //            SfcCommandType = SfcCommandType.Text,
        //        });
        //        if (obj.Data != null && obj.Data.Count() > 0)
        //        {
        //                return obj.Data.ElementAt(0)["attribute_value"].ToString().Trim();
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}

       

        public async Task<string> CheckLogin(string empNO,string empPW,string mygroup,string func,string apver)
        {
            string res = "";
            var obj_check = await _oracle.ExecuteProcAsync("SFIS1.SP_MAKEWEIGHT", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="AP_VER",Value=apver.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_EMP",Value=empNO.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value=mygroup.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_DATA",Value=empPW.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_FUNC",Value=func.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="CARTON",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj_check != null)
            {
                dynamic result = obj_check;
                return (result[0]["res"]);
            }
            else
            {
                return res;
            }
        }


        public async Task<string> GetMOData(string monumber,string func,string apver)
        {
            string res = "";
            var obj_check = await _oracle.ExecuteProcAsync("SFIS1.SP_MAKEWEIGHT", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="AP_VER",Value=apver.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_EMP",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_DATA",Value=monumber.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_FUNC",Value=func.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="CARTON",Value="",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj_check != null)
            {
                dynamic result = obj_check;
                return (result[0]["res"]);
            }
            else
            {
                return res;
            }
        }        

       


        public async Task<string> CheckCartonNo(string empNO,string ctno, string mygroup,string apver,string inData)
        {
            try
            {
                string res = "";
                var obj_check = await _oracle.ExecuteProcAsync("SFIS1.SP_MAKEWEIGHT", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="AP_VER",Value=apver.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_EMP",Value=empNO.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="MYGROUP",Value=mygroup.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_DATA",Value=inData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="IN_FUNC",Value="CHECK_CARTON",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="CARTON",Value=ctno.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });

                if (obj_check != null)
                {
                    dynamic result = obj_check;
                    res = result[0]["res"];
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


        

        //public async Task<string> CheckSN(string empNO,string ctno, string mygroup,string inData,string apver)
        //{
        //    try
        //    {
        //        string res = "";
        //        var obj_check = await _oracle.ExecuteProcAsync("SFIS1.SP_MAKEWEIGHT", SfcCommandType.StoredProcedure, new List<SfcParameter>()
        //            {
        //                new SfcParameter{Name="AP_VER",Value=apver.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
        //                new SfcParameter{Name="IN_EMP",Value=empNO.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
        //                new SfcParameter{Name="MYGROUP",Value=mygroup.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
        //                new SfcParameter{Name="IN_DATA",Value=inData.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
        //                new SfcParameter{Name="IN_FUNC",Value="CHECK_CARTON",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
        //                new SfcParameter{Name="CARTON",Value=ctno.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
        //                new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
        //            });

        //        if (obj_check != null)
        //        {
        //            dynamic result = obj_check;
        //            res = result[0]["res"];
        //            if (!"OK".Equals(res) || res != "OK")
        //            {
        //                return res;
        //            }
        //            else
        //            {
        //                return "OK";
        //            }
        //        }
        //        else
        //        {
        //            return res;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error";
        //    }
        //}


        public async Task<List<string>> checkPrint(string input)
        {
            List<string> lstout = new List<string>();
            try
            {
                string res = "";
                string output = "";
                var obj_check = await _oracle.ExecuteProcAsync("SFIS1.SP_GET_PARAMS", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="IN_DATA",Value=input.ToUpper(),SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUT_DATA",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });

                if (obj_check != null)
                {
                    dynamic result = obj_check;
                    output = result[0]["out_data"];
                    res = result[1]["res"];
                    lstout.Add(res.Trim());
                    lstout.Add(output.Trim());
                    return lstout;
                }
                else
                {
                    lstout.Add(res.Trim());
                    lstout.Add(output.Trim());
                    return lstout;
                }

            }
            catch (Exception ex)
            {
                lstout.Add(ex.Message);
                return lstout;
            }
        }


        public async Task<List<R107>> GetCartonNo(string ctno)
        {
            List<R107> lstP = new List<R107>();
            try
            {
                var obj = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                {
                    CommandText = "select serial_number,mo_number,wip_group,mcarton_no,carton_no from sfism4.r107 where mcarton_no='" + ctno + "'  ",
                    SfcCommandType = SfcCommandType.Text,
                });
                if (obj.Data != null && obj.Data.Count() > 0)
                {
                    for (int i = 0; i < obj.Data.Count(); i++)
                    {
                        R107 r107 = new R107();
                        if (obj.Data.ElementAt(i)["serial_number"] != null)
                        {
                            r107.SN = obj.Data.ElementAt(i)["serial_number"].ToString();
                        }
                        else
                        {
                            r107.SN = "";
                        }

                        if (obj.Data.ElementAt(i)["mo_number"] != null)
                        {
                            r107.MO_NUMEBR = obj.Data.ElementAt(i)["mo_number"].ToString();
                        }
                        else
                        {
                            r107.MO_NUMEBR = "";
                        }

                        if (obj.Data.ElementAt(i)["mcarton_no"] != null)
                        {
                            r107.MCARTON_NO = obj.Data.ElementAt(i)["mcarton_no"].ToString();
                        }
                        else
                        {
                            r107.MCARTON_NO = "";
                        }

                        if (obj.Data.ElementAt(i)["wip_group"] != null)
                        {
                            r107.WIP_GROUP = obj.Data.ElementAt(i)["wip_group"].ToString();
                        }
                        else
                        {
                            r107.WIP_GROUP = "";
                        }

                        if (obj.Data.ElementAt(i)["carton_no"] != null)
                        {
                            r107.CARTON = obj.Data.ElementAt(i)["carton_no"].ToString();
                        }
                        else
                        {
                            r107.CARTON = "";
                        }


                        lstP.Add(r107);
                    }
                    return lstP;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<string> CheckSNExist(string sn,string fun)
        {
            try
            {
                if ("CTN_WEIGHT".Equals(fun) || fun == "CTN_WEIGHT")
                {
                    var obj = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFISM4.R_WIP_TRACKING_T where ( serial_number = '"+ sn + "' or shipping_sn2='" + sn + "' or shipping_sn='" + sn + "' ) and carton_no<>'N/A' AND ROWNUM=1  ",
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (obj.Data != null && obj.Data.Count() > 0)
                    {
                        return "OK";
                    }
                    else
                    {
                        return "NG";
                    }
                }
                else if("FQA_WEIGHT".Equals(fun) || fun == "FQA_WEIGHT")
                {
                    var obj = await _sfcHttpClient.QueryListAsync(new QuerySingleParameterModel()
                    {
                        CommandText = "SELECT * FROM SFISM4.R_WIP_TRACKING_T where (MCARTON_NO ='" + sn + "' or serial_number ='" + sn + "' or shipping_sn2='" + sn + "' or shipping_sn='" + sn + "' ) and carton_no<>''N/A'' AND ROWNUM=1  ",
                        SfcCommandType = SfcCommandType.Text,
                    });
                    if (obj.Data != null && obj.Data.Count() > 0)
                    {
                        return "OK";
                    }
                    else
                    {
                        return "NG";
                    }
                }
                else
                {
                    return "NG";
                }
                
            }
            catch(Exception ex)
            {
                return "NG";
            }
            
        }

        
       
        public async Task<string> GetPubMessage(string prompt_code)
        {
            var obj = await _oracle.ExecuteProcAsync("sfis1.GET_PROMPT_MESSAGE_NEW", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="P_PROMPT_CODE",Value=prompt_code,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="RES",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
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

              
        
        public async Task<DBConnect> GetApiInfor()
        {
            strSQL = string.Format(@"select VR_NAME as LINK_API,VR_VALUE as DB_CONNECT from SFIS1.C_PARAMETER_INI where PRG_NAME ='ALLPARTS' and VR_DESC ='WEB_API'");
            var result = await _oracle.GetObj<DBConnect>(strSQL);
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        public async Task<string> DoExcuseSMTAPIProc(object input)
        {
            string jsonData = JsonConvert.SerializeObject(input).ToString();

            var obj = await _oracle.ExecuteProcAsync("SFIS1.SMT_API_GETDATA", SfcCommandType.StoredProcedure, new List<SfcParameter>()
                    {
                        new SfcParameter{Name="data",Value=jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="output",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    });
            if (obj != null)
            {
                dynamic result = obj;
                return (result[0]["output"]);
            }
            else
            {
                return "FALSE";
            }
        }
    }
}
