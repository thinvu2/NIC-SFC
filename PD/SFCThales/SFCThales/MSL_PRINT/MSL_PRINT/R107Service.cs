using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Newtonsoft.Json;
using Sfc.Library.HttpClient;

namespace MSL_PRINT
{
    class R107Service
    {
        public async Task<DataTable>  ExecSNFormatProcedure(string sn, SfcHttpClient sfcHttpClient)
        {
            string[] result = null;
            DataTable dt = null;
            try
            {
                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "data", Value = sn, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "sn",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output },
                                new SfcParameter { Name = "ssn",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output },
                                new SfcParameter { Name = "mac", SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output },
                                new SfcParameter { Name = "ec",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output },
                                };

                dt = await ExcuteSP("sfis1.FORMAT_INPUTSN", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {

            }
            return dt;
        }

        public async Task<string> ExecCheckRouteProcedure(string line, string myGroup, string sn, SfcHttpClient sfcHttpClient)
        {
            
            DataTable dt = null;
            try
            {

                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "line", Value = line, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "mygroup", Value = myGroup, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "data", Value = sn, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                            };

                dt = await ExcuteSP("sfis1.NEW_CHECK_ROUTE", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt.Rows[0]["res"].ToString();
        }

        public async Task<string> ExecCheckSNPackTimeProcedure(string sn, string myGroup, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "sn", Value = sn, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "mygroup",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                                };

                dt = await ExcuteSP("sfis1.PACK_TIMEOUT_BGS3", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt.Rows[0]["res"].ToString();
        }


        public async Task<string> ExecR107Procedure(string line, string section, string myGroup, string station, string sn, SfcHttpClient sfcHttpClient)
        {

            DataTable dt = null;
            try
            {
                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "line", Value = line, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "section", Value = section, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "mygroup", Value = myGroup, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "w_station", Value =station, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "ec", Value = "N/A", SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "data", Value = sn, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "emp", Value = MSL_PRINT.EMP_NO, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                            };

                dt = await ExcuteSP("sfis1.NEW_TEST_INPUT", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt.Rows[0]["res"].ToString();
        }

        //public static string[] ExecLdbCheckProcedure(string invoice, SfcHttpClient sfcHttpClient)
        //{
        //    string[] result = null;
        //    try
        //    {
        //        OracleParameter[] parameter = new OracleParameter[7]
        //        {
        //            new OracleParameter("INVOICE",OracleDbType.Varchar2, 25),
        //            new OracleParameter("MODELNAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("VTCOM",OracleDbType.Varchar2, 25),
        //            new OracleParameter("SHIPQTY",OracleDbType.Int16, 25),
        //            new OracleParameter("CUSTPO",OracleDbType.Varchar2, 25),
        //            new OracleParameter("CUSTOMERNAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("STATUS",OracleDbType.Varchar2, 100)
        //        };


        //        parameter[0].Direction = ParameterDirection.Input;

        //        for (int i = 1; i < parameter.Length; i++)
        //        {
        //            parameter[i].Direction = ParameterDirection.Output;
        //        }

        //        parameter[0].Value = invoice;

        //        result = DBHelper.RunProcedure("LDBCHECK", 6, parameter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //    finally
        //    {
                
        //    }
        //    return result;
        //}

        //public static string ExecLdbCheckUploadProcedure(string invoice, string modelName, string custName, string tcom, string shipQty, SfcHttpClient sfcHttpClient)
        //{
        //    string result = null;
        //    try
        //    {
        //        OracleParameter[] parameter = new OracleParameter[6]
        //        {
        //            new OracleParameter("INVOICE",OracleDbType.Varchar2, 25),
        //            new OracleParameter("MODELNAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("CUSTOMERNAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("VTCOM",OracleDbType.Varchar2, 25),
        //            new OracleParameter("SHIPQTY",OracleDbType.Varchar2, 25),
        //            new OracleParameter("STATUS",OracleDbType.Varchar2, 100)
        //        };

        //        for (int i = 0; i < parameter.Length - 1; i++)
        //        {
        //            parameter[i].Direction = ParameterDirection.Input;
        //        }
        //        parameter[5].Direction = ParameterDirection.Output;

        //        parameter[0].Value = invoice;
        //        parameter[1].Value = modelName;
        //        parameter[2].Value = custName;
        //        parameter[3].Value = tcom;
        //        parameter[4].Value = shipQty;

        //        result = DBHelper.RunProcedure("LDBCHECK_UPLOAD", parameter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //    finally
        //    {
                
        //    }
        //    return result;
        //}
        //public static string ExecCheckRouteProcedureq(string sn, string wip, string group, SfcHttpClient sfcHttpClient)
        //{
        //    string result = null;
        //    try
        //    {
        //        OracleParameter[] parameter = new OracleParameter[4]
        //        {
        //            new OracleParameter("DATA",OracleDbType.Varchar2, 25),
        //            new OracleParameter("SN",OracleDbType.Varchar2, 25),
        //            new OracleParameter("MYGROUP",OracleDbType.Varchar2, 60),
        //            new OracleParameter("RES",OracleDbType.Varchar2, 25)
        //        };

        //        for (int i = 0; i < parameter.Length - 1; i++)
        //        {
        //            parameter[i].Direction = ParameterDirection.Input;
        //        }
        //        parameter[3].Direction = ParameterDirection.Output;

        //        parameter[0].Value = sn;
        //        parameter[1].Value = wip;
        //        parameter[2].Value = group;

        //        result = DBHelper.RunProcedure("Check_VI6", parameter);
        //        //result = DBHelper.RunProcedure()
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //    finally
        //    {
                
        //    }
        //    return result;
        //}

        //public static string ExecCreate856Procedure(string carrier, string trans, string hawb, string type, string area, string country, string dn, SfcHttpClient sfcHttpClient)
        //{
        //    string result = null;
        //    try
        //    {
        //        OracleParameter[] parameter = new OracleParameter[8]
        //        {
        //            new OracleParameter("PCARRIER",OracleDbType.Varchar2, 25),
        //            new OracleParameter("PTRANS",OracleDbType.Varchar2, 25),
        //            new OracleParameter("PHAWB",OracleDbType.Varchar2, 25),
        //            new OracleParameter("PASFIS",OracleDbType.Varchar2, 25),
        //            new OracleParameter("PATYPE",OracleDbType.Varchar2, 25),
        //            new OracleParameter("PPOE",OracleDbType.Varchar2, 25),
        //            new OracleParameter("PDN",OracleDbType.Varchar2, 25),
        //            new OracleParameter("RES",OracleDbType.Varchar2, 100)
        //        };

        //        for (int i = 0; i < parameter.Length - 1; i++)
        //        {
        //            parameter[i].Direction = ParameterDirection.Input;
        //        }
        //        parameter[7].Direction = ParameterDirection.Output;

        //        parameter[0].Value = carrier;
        //        parameter[1].Value = trans;
        //        parameter[2].Value = hawb;
        //        parameter[3].Value = type;
        //        parameter[4].Value = area;
        //        parameter[5].Value = country;
        //        parameter[6].Value = dn;

        //        result = DBHelper.RunProcedure("EERO_CREATE_856", parameter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //    finally
        //    {
                
        //    }
        //    return result;
        //}
        ////SMTLOADING程序各种检查OK后的Insert过站
        //public static string ExecSPSMTLoadingInsertSNlinkR107STNRec(string InLine, string InSection, string InMyGroup, string InWipGroup, string InStationName,
        //    string InEmpNo, string InMO, string InModel, string InVersion, string InRouteCode, string InTRSN, string InTraceCode, string InSN, string InLinkQty)
        //{
        //    string result = null;
        //    try
        //    {
        //        OracleParameter[] parameter = new OracleParameter[15]
        //        {
        //            new OracleParameter("LINENAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("SECTIONNAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("MYGROUP",OracleDbType.Varchar2, 25),
        //            new OracleParameter("WIPGROUP",OracleDbType.Varchar2, 25),
        //            new OracleParameter("STATIONAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("EMPNO",OracleDbType.Varchar2, 25),
        //            new OracleParameter("MONUMBER",OracleDbType.Varchar2, 25),
        //            new OracleParameter("MODELNAME",OracleDbType.Varchar2, 25),
        //            new OracleParameter("VERSIONCODE",OracleDbType.Varchar2, 25),
        //            new OracleParameter("ROUTECODE",OracleDbType.Varchar2, 25),
        //            new OracleParameter("TRSN",OracleDbType.Varchar2, 25),
        //            new OracleParameter("TRACECODE",OracleDbType.Varchar2, 25),
        //            new OracleParameter("SN",OracleDbType.Varchar2, 25),
        //            new OracleParameter("LINKQTY",OracleDbType.Varchar2, 25),
        //            new OracleParameter("RES",OracleDbType.Varchar2, 100)
        //        };

        //        for (int i = 0; i < parameter.Length - 1; i++)
        //        {
        //            parameter[i].Direction = ParameterDirection.Input;
        //        }
        //        parameter[14].Direction = ParameterDirection.Output;

        //        parameter[0].Value = InLine;
        //        parameter[1].Value = InSection;
        //        parameter[2].Value = InMyGroup;
        //        parameter[3].Value = InWipGroup;
        //        parameter[4].Value = InStationName;
        //        parameter[5].Value = InEmpNo;
        //        parameter[6].Value = InMO;
        //        parameter[7].Value = InModel;
        //        parameter[8].Value = InVersion;
        //        parameter[9].Value = InRouteCode;
        //        parameter[10].Value = InTRSN;
        //        parameter[11].Value = InTraceCode;
        //        parameter[12].Value = InSN;
        //        parameter[13].Value = InLinkQty;

        //        result = DBHelper.RunProcedure("SFIS1.SMTLOADING_INSERT_SNLINK_R107", parameter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.ToString());
        //    }
        //    finally
        //    {
                
        //    }
        //    return result;
        //}

        //public static string[] RunFailLocationErrorSnInput(string line, string group, string section, string station, string pcname, string emp, string location, string errorCode, string sn)
        //{
        //    string[] result = null;
        //    try
        //    {
        //        OracleParameter[] parameter = new OracleParameter[]
        //        {
        //            new OracleParameter("LINENAME",OracleDbType.Varchar2,25),
        //            new OracleParameter("MYGROUP",OracleDbType.Varchar2,25),
        //            new OracleParameter("SECTIONNAME",OracleDbType.Varchar2,25),
        //            new OracleParameter("MYSTATION",OracleDbType.Varchar2,25),
        //            new OracleParameter("PCNAME",OracleDbType.Varchar2,25),
        //            new OracleParameter("EMPNO",OracleDbType.Varchar2,25),
        //            new OracleParameter("ERRORLOCATION",OracleDbType.Varchar2,25),
        //            new OracleParameter("ERRORCODE",OracleDbType.Varchar2,25),
        //            new OracleParameter("ERRORSN",OracleDbType.Varchar2,70),
        //            new OracleParameter("MONUMBER",OracleDbType.Varchar2,25),
        //            new OracleParameter("MODELNAME",OracleDbType.Varchar2,25),
        //            new OracleParameter("STATUS",OracleDbType.Varchar2,60)
        //        };

        //        for (int i = 0; i < parameter.Length - 3; i++)
        //        {
        //            parameter[i].Direction = ParameterDirection.Input;
        //        }
        //        for (int i = parameter.Length - 3; i < parameter.Length; i++)
        //        {
        //            parameter[i].Direction = ParameterDirection.Output;
        //        }

        //        parameter[0].Value = line;
        //        parameter[1].Value = group;
        //        parameter[2].Value = section;
        //        parameter[3].Value = station;
        //        parameter[4].Value = pcname;
        //        parameter[5].Value = emp;
        //        parameter[6].Value = location;
        //        parameter[7].Value = errorCode;
        //        parameter[8].Value = sn;

        //        result = DBHelper.RunProcedure("SFIS1.FAIL_LOCATION_ERROR_SN_INPUT", 3, parameter);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
                
        //    }
        //    return result;
        //}
        public async Task<DataTable> GetR107BySNERICSSON(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {

                string sql =string.Format(@"SELECT DISTINCT A.SERIAL_NUMBER,
                                                A.MO_NUMBER,
                                                A.MODEL_NAME,
                                                A.VERSION_CODE,
                                                A.SHIPPING_SN,
                                                A.SHIPPING_SN2,
                                                A.BILL_NO,
                                                A.TRACK_NO,
                                                A.PO_NO,
                                                A.PMCC,
                                                A.MSN, 
                                                '1' QTY, 
                                                'DU5' FACTORYCODE,                                                         
                                                (SELECT TO_CHAR(PRINT_TIME, 'YYYYMMDD') FROM SFISM4.R_ERICSSON_CSN_T
                                                        WHERE SERIAL_NUMBER = A.SERIAL_NUMBER) DATECODE
                                    FROM SFISM4.R_WIP_TRACKING_T A WHERE A.SERIAL_NUMBER = '{0}' OR A.SHIPPING_SN = '{0}' OR A.SHIPPING_SN2 = '{0}'
                                            AND ROWNUM = 1", sn);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> GetR107BySN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format(@"SELECT DISTINCT A.SERIAL_NUMBER, 
                                               A.MO_NUMBER, 
                                               A.MODEL_NAME,
                                               A.VERSION_CODE,
                                               A.SHIPPING_SN,
                                               A.SHIPPING_SN2, 
                                               A.BILL_NO, 
                                               A.TRACK_NO, 
                                               A.PO_NO, 
                                               A.PMCC, 
                                               A.MSN, 
                                               B.DATA1, 
                                               B.DATA2, 
                                               B.DATA3,
                                               B.DATA4, 
                                               B.DATA5, 
                                               B.DATA6, 
                                               B.DATA7, 
                                               B.DATA8, 
                                               B.DATA9, 
                                               B.DATA10, 
                                               TO_CHAR(SYSDATE,'YYYYMMWWDD') DATECODE
                                          FROM (SELECT * FROM SFISM4.R_WIP_TRACKING_T
                                                 WHERE (SERIAL_NUMBER = '{0}' OR SHIPPING_SN = '{0}' OR SHIPPING_SN2 = '{0}')         
                                                 ORDER BY IN_STATION_TIME DESC) A,
                                               (SELECT * FROM SFISM4.R_NOKIA_TEST_DATA_T 
                                                 WHERE GROUP_NAME IN ('ATO','ASSY','OTA','FT2','FT','RI_TMO','RI_R2','RI_RAKUTEN')   
                                                   AND STATUS ='PASS') B
                                         WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER(+) 
                                           AND ROWNUM = 1", sn);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        //Get R107 Info By SSN
        public async Task<DataTable> GetR107SNInfoBySSN(string InSSN, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format(@"SELECT * FROM SFISM4.R_WIP_TRACKING_T 
                               WHERE SHIPPING_SN=:SSN AND ROWNUM=1 ", InSSN);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        //Get R107 Info By SN
        public async Task<DataTable> GetR107SNInfoBySN(string InSN, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT * FROM SFISM4.R_WIP_TRACKING_T 
                               WHERE SERIAL_NUMBER='{0}' AND ROWNUM=1 ", InSN);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> GetR107ByIMEI(string IMEI, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,VERSION_CODE,SHIPPING_SN,SHIPPING_SN2 FROM SFISM4.R_WIP_TRACKING_T 
                               WHERE SHIPPING_SN=:IMEI ";
              
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> GetR107ByReelNo(string reelNo, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO ='{0}' ", reelNo);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> GetR108BySN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT A.*,B.MODEL_NAME MODEL_NAME FROM SFISM4.R_WIP_KEYPARTS_T A,SFISM4.R_WIP_TRACKING_T B 
                                WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER 
                                AND (B.SERIAL_NUMBER='{0}' OR B.SHIPPING_SN ='{0}') 
                                AND A.GROUP_NAME='ASSY5'", sn);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> GetStockNoQty(string VarStockNo, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT COUNT(SERIAL_NUMBER) QTY FROM SFISM4.R_WIP_TRACKING_T WHERE STOCK_NO = '{0}' ", VarStockNo);
               
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> Get_WPON_BySN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT A.SERIAL_NUMBER,
                                       A.MO_NUMBER,
                                       A.MODEL_NAME,
                                       A.VERSION_CODE,
                                       A.SHIPPING_SN,
                                       B.KEY_PART_SN,
                                       C.MAC1,
                                       C.MAC2
                                  FROM SFISM4.R_WIP_TRACKING_T A,
                                       SFISM4.R_WIP_KEYPARTS_T B,
                                       SFISM4.R_PRINT_INPUT_T  C
                                 WHERE (A.SERIAL_NUMBER = '{0}' OR A.SHIPPING_SN = '{0}')
                                   AND A.SERIAL_NUMBER = B.SERIAL_NUMBER
                                   AND B.GROUP_NAME = 'ASSY1'
                                   AND B.KEY_PART_SN LIKE 'EB%'
                                   AND B.KEY_PART_SN = C.SERIAL_NUMBER", sn);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> Get_NORKIA_BySN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT A.SERIAL_NUMBER,
                                        A.MO_NUMBER,
                                        A.MODEL_NAME,
                                        A.VERSION_CODE,
                                        A.SHIPPING_SN,
                                        A.SHIPPING_SN2,
                                        A.BILL_NO,
                                        A.TRACK_NO,
                                        A.PO_NO, 
                                        A.PMCC, 
                                        A.MSN, 
                                        B.DATA1,
                                        B.DATA2,
                                        B.DATA3,
                                        B.DATA4,
                                        B.DATA5,
                                        B.DATA6,
                                        B.DATA7,
                                        B.DATA8,
                                        B.DATA9,
                                        B.DATA10
                                FROM SFISM4.R_WIP_TRACKING_T A, SFISM4.R_NOKIA_TEST_DATA_T B
                            WHERE A.SERIAL_NUMBER = B.SERIAL_NUMBER
                                AND (A.SERIAL_NUMBER='{0}' OR A.SHIPPING_SN='{0}' OR A.SHIPPING_SN2=:SN)
                                AND B.GROUP_NAME IN('SWDL','RC')
                                AND B.STATUS = 'PASS' ", sn);
                
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> Get_EERO_BOX_BySN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT A.SERIAL_NUMBER,
                                      A.MO_NUMBER,
                                      A.MODEL_NAME,
                                      A.VERSION_CODE,
                                      B.KEY_PART_SN
                                 FROM SFISM4.R_WIP_TRACKING_T A,
                                      SFISM4.R_WIP_KEYPARTS_T B
                                WHERE A.SERIAL_NUMBER = '{0}' 
                                  AND A.SERIAL_NUMBER = B.SERIAL_NUMBER
                                  AND B.KEY_PART_NO LIKE '810%' 
                                  AND LENGTH(B.KEY_PART_SN) = 16
                                  AND ROWNUM = 1", sn);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> GetModelType(string model, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT UPPER(MODEL_SERIAL) MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T 
                                WHERE MODEL_NAME ='{0}' AND ROWNUM = 1 ", model);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable>GetT77BySN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT A.SERIAL_NUMBER, 
                                      A.MO_NUMBER, 
                                      A.MODEL_NAME,
                                      A.VERSION_CODE,
                                      A.SHIPPING_SN,
                                      A.SHIPPING_SN2, 
                                      A.BILL_NO, 
                                      A.TRACK_NO, 
                                      A.PO_NO, 
                                      A.PMCC, 
                                      A.MSN,
                                      TO_CHAR(B.FT_TIME,'YYYYMMDD') SFIS_DATE
                                 FROM SFISM4.R_WIP_TRACKING_T A, SFISM4.R_T77T943_CSNMAC_T B
                                WHERE A.SERIAL_NUMBER = B.SERIAL_NUMBER
                                  AND (A.SERIAL_NUMBER='{0}' OR A.SHIPPING_SN=:SN OR A.SHIPPING_SN2=:SN)", sn);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> CheckT77SN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format( @"SELECT * FROM SFISM4.R_T77T943_CSNMAC_T
                                WHERE CUST_SN = (SELECT SUBSTR('{0}', 1, 9) || (SUBSTR(:SN, 10, 5) + 21450) NEWSN 
                                                    FROM DUAL)", sn);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> CheckRePrintLog(SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_REPRINT_LOG_T 
                                WHERE REPRINT_TIME IS NULL OR CHECKOUT_TIME IS NULL";

                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> CheckRePrintLog(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_REPRINT_LOG_T 
                                WHERE SERIAL_NUMBER <>:SN 
                                  AND (REPRINT_TIME IS NULL OR CHECKOUT_TIME IS NULL)";
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> CheckRePrintStatus(string sn, string status, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql =string.Format(@"SELECT * FROM SFISM4.R_REPRINT_LOG_T 
                                WHERE SERIAL_NUMBER ='{0}'", sn);

                if (status == "REPRINT")
                {
                    sql += " AND REPRINT_TIME IS NULL AND PRINTER IS NULL";
                }
                if (status == "CHECKOUT")
                {
                    sql += " AND REPRINT_TIME IS NOT NULL AND CHECKOUT_TIME IS NULL";
                }

                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }
        //Update R107 
        public async Task<int> UpdateR107ScrapBySN(string InSN, string InEmpNo, string InStockNo, string InStorage, SfcHttpClient sfcHttpClient)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_WIP_TRACKING_T SET SCRAP_FLAG='1',EMP_NO='{1}',STOCK_NO='{2}',WIP_GROUP='{3}',IN_STATION_TIME=SYSDATE
                                             WHERE SERIAL_NUMBER ='{0}' AND ROWNUM=1 ", InSN, InEmpNo, InStockNo, InStorage);
                await ExcuteNonQuerySQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return result;
        }
        public async Task<DataTable> QueryR117LimitColumn(string InSN, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;

            try
            {
                string sql = string.Format(@" SELECT SERIAL_NUMBER,GROUP_NAME,WIP_GROUP,IN_STATION_TIME,ATE_STATION_NO 
                            FROM SFISM4.R117 WHERE SERIAL_NUMBER='{0}' ORDER BY IN_STATION_TIME DESC", InSN);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }
        public async Task<DataTable> GetWip(string sField, string sValue, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;

            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE {0} = '{1}' ", sField, sValue);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }
        //根據指定SN和指定站位和當前時間當前站位比較的分鐘數
        public async Task<DataTable> GetOverTimeMMinutesBySNandGroup(string InSN, string InGroup, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT TRUNC( (SYSDATE- MAX(IN_STATION_TIME) )*24*60) OVER_MINUTES FROM SFISM4.R_SN_DETAIL_T WHERE SERIAL_NUMBER='{0}'  AND GROUP_NAME='{1}'  ", InSN, InGroup);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }
        //SELECT CONTROL_TIME FROM SFIS1.C_ROAST_TIME_CONTROL_T WHERE MODEL_NAME = 'T77H459.03' AND DEFAULT_GROUP = 'ROAST_IN'  AND END_GROUP = 'ROAST_OUT';
        public async Task<DataTable> GetOverTimeInfoByModelGroup(string InModel, string InStartGroup, string InEndGroup, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT CONTROL_TIME FROM SFIS1.C_ROAST_TIME_CONTROL_T WHERE MODEL_NAME = '{0}' AND DEFAULT_GROUP = '{1}' AND END_GROUP ='{2}'  ", InModel, InStartGroup, InEndGroup);
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }
        public async Task<DataTable> ExcuteSelectSQL(string sql, SfcHttpClient sfcHttpClient)
        {
            DataTable data=new DataTable();
            try
            {
                var datacust = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
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
                throw new Exception(ex.ToString());
            }
            return data;
        }
        public async Task<bool> ExcuteNonQuerySQL(string sql, SfcHttpClient sfcHttpClient)
        {
            try
            {
                await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return true;
        }
        public async Task<DataTable> ExcuteSP(string procename, List<SfcParameter> ListPara, SfcHttpClient sfcHttpClient)
        {
            DataTable data = new DataTable();
            try
            {
                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = procename,
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = ListPara
                });
                dynamic ads = result.Data;
                var vardatatabel = JsonConvert.SerializeObject(result.Data);
                data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
            }
            catch (Exception ex)
            {
                return null;
            }
            return data;
        }
    }
}
