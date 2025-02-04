using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class R107Service
    {

        public static string[] ExecSNFormatProcedure(string sn)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[5]
                {
                    new OracleParameter("DATA",OracleDbType.Varchar2, 200),
                    new OracleParameter("SN",OracleDbType.Varchar2, 25),
                    new OracleParameter("SSN",OracleDbType.Varchar2, 100),
                    new OracleParameter("MAC",OracleDbType.Varchar2, 100),
                    new OracleParameter("RES",OracleDbType.Varchar2, 50)
                };
                
                parameter[0].Direction = ParameterDirection.Input;

                for (int i = 1; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = sn;

                result = DBHelper.RunProcedure("SFIS1.FORMAT_INPUTSN", 4, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static string ExecCheckRouteProcedure(string line, string myGroup, string sn)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[4]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2, 25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2, 25),
                    new OracleParameter("DATA",OracleDbType.Varchar2, 60),
                    new OracleParameter("RES",OracleDbType.Varchar2, 25)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[3].Direction = ParameterDirection.Output;

                parameter[0].Value = line;
                parameter[1].Value = myGroup;
                parameter[2].Value = sn;

                result = DBHelper.RunProcedure("SFIS1.NEW_CHECK_ROUTE", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static string ExecCheckSNPackTimeProcedure(string sn, string myGroup)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[3]
                {
                    new OracleParameter("SN",OracleDbType.Varchar2, 25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2, 25),
                    new OracleParameter("RES",OracleDbType.Varchar2, 100)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[2].Direction = ParameterDirection.Output;

                parameter[0].Value = sn;
                parameter[1].Value = myGroup;

                result = DBHelper.RunProcedure("SFIS1.PACK_TIMEOUT_BGS3", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }
        
        public static string ExecR107ProcedureEricsson(string line, string section, string myGroup, string station, string sn)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[11]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2, 25),
                    new OracleParameter("SECTION",OracleDbType.Varchar2, 25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2, 25),
                    new OracleParameter("W_STATION",OracleDbType.Varchar2, 25),
                    new OracleParameter("EC",OracleDbType.Varchar2, 25),
                    new OracleParameter("DATA",OracleDbType.Varchar2, 60),
                    new OracleParameter("EMP",OracleDbType.Varchar2, 25),                    
                    new OracleParameter("DATETIME",OracleDbType.Date),
                    new OracleParameter("MO_DATE",OracleDbType.Varchar2, 25),
                    new OracleParameter("W_SECTION",OracleDbType.Int16, 25),
                    new OracleParameter("RES",OracleDbType.Varchar2, 25),
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[10].Direction = ParameterDirection.Output;

                parameter[0].Value = line;
                parameter[1].Value = section;
                parameter[2].Value = myGroup;
                parameter[3].Value = station;
                parameter[4].Value = "N/A";
                parameter[5].Value = sn;
                parameter[6].Value = UserService.loginNO;
                parameter[7].Value = System.DateTime.Now;
                parameter[8].Value = System.DateTime.Now.ToString("yyyMMdd");
                parameter[9].Value = Convert.ToInt16(System.DateTime.Now.ToString("hh"));

                result = DBHelper.RunProcedure("SFIS1.TEST_INPUT_MAN_ONLY", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static string ExecR107Procedure(string line, string section, string myGroup, string station, string sn)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[8]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2, 25),
                    new OracleParameter("SECTION",OracleDbType.Varchar2, 25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2, 25),
                    new OracleParameter("W_STATION",OracleDbType.Varchar2, 25),
                    new OracleParameter("EC",OracleDbType.Varchar2, 25),
                    new OracleParameter("DATA",OracleDbType.Varchar2, 60),
                    new OracleParameter("EMP",OracleDbType.Varchar2, 25),
                    new OracleParameter("RES",OracleDbType.Varchar2, 25)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[7].Direction = ParameterDirection.Output;

                parameter[0].Value = line;
                parameter[1].Value = section;
                parameter[2].Value = myGroup;
                parameter[3].Value = station;
                parameter[4].Value = "N/A";
                parameter[5].Value = sn;
                parameter[6].Value = UserService.loginNO;

                result = DBHelper.RunProcedure("SFIS1.NEW_TEST_INPUT", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static string[] ExecLdbCheckProcedure(string invoice)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[7]
                {
                    new OracleParameter("INVOICE",OracleDbType.Varchar2, 25),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("VTCOM",OracleDbType.Varchar2, 25),
                    new OracleParameter("SHIPQTY",OracleDbType.Int16, 25),
                    new OracleParameter("CUSTPO",OracleDbType.Varchar2, 25),
                    new OracleParameter("CUSTOMERNAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("STATUS",OracleDbType.Varchar2, 100)
                };


                parameter[0].Direction = ParameterDirection.Input;

                for (int i = 1; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = invoice;

                result = DBHelper.RunProcedure("LDBCHECK", 6, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static string ExecLdbCheckUploadProcedure(string invoice, string modelName,string custName, string tcom, string shipQty)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[6]
                {
                    new OracleParameter("INVOICE",OracleDbType.Varchar2, 25),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("CUSTOMERNAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("VTCOM",OracleDbType.Varchar2, 25),
                    new OracleParameter("SHIPQTY",OracleDbType.Varchar2, 25),
                    new OracleParameter("STATUS",OracleDbType.Varchar2, 100)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[5].Direction = ParameterDirection.Output;

                parameter[0].Value = invoice;
                parameter[1].Value = modelName;
                parameter[2].Value = custName;
                parameter[3].Value = tcom;
                parameter[4].Value = shipQty;

                result = DBHelper.RunProcedure("LDBCHECK_UPLOAD", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static string ExecCreate856Procedure(string carrier, string trans, string hawb, string type, string area, string country, string dn)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[8]
                {
                    new OracleParameter("PCARRIER",OracleDbType.Varchar2, 25),
                    new OracleParameter("PTRANS",OracleDbType.Varchar2, 25),
                    new OracleParameter("PHAWB",OracleDbType.Varchar2, 25),
                    new OracleParameter("PASFIS",OracleDbType.Varchar2, 25),
                    new OracleParameter("PATYPE",OracleDbType.Varchar2, 25),
                    new OracleParameter("PPOE",OracleDbType.Varchar2, 25),
                    new OracleParameter("PDN",OracleDbType.Varchar2, 25),
                    new OracleParameter("RES",OracleDbType.Varchar2, 100)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[7].Direction = ParameterDirection.Output;

                parameter[0].Value = carrier;
                parameter[1].Value = trans;
                parameter[2].Value = hawb;
                parameter[3].Value = type;
                parameter[4].Value = area;
                parameter[5].Value = country;
                parameter[6].Value = dn;

                result = DBHelper.RunProcedure("EERO_CREATE_856", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }
        //SMTLOADING程序各种检查OK后的Insert过站
        public static string ExecSPSMTLoadingInsertSNlinkR107STNRec(string InLine, string InSection, string InMyGroup, string InWipGroup, string InStationName,
            string InEmpNo, string InMO, string InModel, string InVersion, string InRouteCode, string InTRSN, string InTraceCode, string InSN, string InLinkQty)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[15]
                {
                    new OracleParameter("LINENAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("SECTIONNAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2, 25),
                    new OracleParameter("WIPGROUP",OracleDbType.Varchar2, 25),
                    new OracleParameter("STATIONAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2, 25),
                    new OracleParameter("MONUMBER",OracleDbType.Varchar2, 25),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2, 25),
                    new OracleParameter("VERSIONCODE",OracleDbType.Varchar2, 25),
                    new OracleParameter("ROUTECODE",OracleDbType.Varchar2, 25),
                    new OracleParameter("TRSN",OracleDbType.Varchar2, 25),
                    new OracleParameter("TRACECODE",OracleDbType.Varchar2, 25),
                    new OracleParameter("SN",OracleDbType.Varchar2, 25),
                    new OracleParameter("LINKQTY",OracleDbType.Varchar2, 25),
                    new OracleParameter("RES",OracleDbType.Varchar2, 100)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[14].Direction = ParameterDirection.Output;

                parameter[0].Value = InLine;
                parameter[1].Value = InSection;
                parameter[2].Value = InMyGroup;
                parameter[3].Value = InWipGroup;
                parameter[4].Value = InStationName;
                parameter[5].Value = InEmpNo;
                parameter[6].Value = InMO;
                parameter[7].Value = InModel;
                parameter[8].Value = InVersion;
                parameter[9].Value = InRouteCode;
                parameter[10].Value = InTRSN;
                parameter[11].Value = InTraceCode;
                parameter[12].Value = InSN;
                parameter[13].Value = InLinkQty;

                result = DBHelper.RunProcedure("SFIS1.SMTLOADING_INSERT_SNLINK_R107", parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static string[] RunFailLocationErrorSnInput(string line, string group, string section, string station, string pcname, string emp, string location, string errorCode, string sn)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {   
                    new OracleParameter("LINENAME",OracleDbType.Varchar2,25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,25),
                    new OracleParameter("SECTIONNAME",OracleDbType.Varchar2,25),
                    new OracleParameter("MYSTATION",OracleDbType.Varchar2,25),
                    new OracleParameter("PCNAME",OracleDbType.Varchar2,25),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,25),
                    new OracleParameter("ERRORLOCATION",OracleDbType.Varchar2,25),
                    new OracleParameter("ERRORCODE",OracleDbType.Varchar2,25),
                    new OracleParameter("ERRORSN",OracleDbType.Varchar2,70),
                    new OracleParameter("MONUMBER",OracleDbType.Varchar2,25),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,25),
                    new OracleParameter("STATUS",OracleDbType.Varchar2,60)
                };

                for (int i = 0; i < parameter.Length - 3; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = parameter.Length - 3; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = line;
                parameter[1].Value = group;
                parameter[2].Value = section;
                parameter[3].Value = station;
                parameter[4].Value = pcname;
                parameter[5].Value = emp;
                parameter[6].Value = location;
                parameter[7].Value = errorCode;
                parameter[8].Value = sn;

                result = DBHelper.RunProcedure("SFIS1.FAIL_LOCATION_ERROR_SN_INPUT", 3, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }
        public static DataTable GetR107BySNERICSSON(string sn)
        {
            DataTable dt = null;
            try
            {

                string sql = @"SELECT DISTINCT A.SERIAL_NUMBER,
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
                                    FROM SFISM4.R_WIP_TRACKING_T A WHERE A.SERIAL_NUMBER = :SN OR A.SHIPPING_SN = :SN OR A.SHIPPING_SN2 = :SN
                                            AND ROWNUM = 1";

                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn),
   
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetR107BySN(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT DISTINCT A.SERIAL_NUMBER, 
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
                                                 WHERE (SERIAL_NUMBER = :SN OR SHIPPING_SN = :SN OR SHIPPING_SN2 = :SN)         
                                                 ORDER BY IN_STATION_TIME DESC) A,
                                               (SELECT * FROM SFISM4.R_NOKIA_TEST_DATA_T 
                                                 WHERE GROUP_NAME IN ('ATO','ASSY','OTA','FT2','FT','RI_TMO','RI_R2','RI_RAKUTEN')   
                                                   AND STATUS ='PASS') B
                                         WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER(+) 
                                           AND ROWNUM = 1";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn),
   
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        //Get R107 Info By SSN
        public static DataTable GetR107SNInfoBySSN(string InSSN)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T 
                               WHERE SHIPPING_SN=:SSN AND ROWNUM=1 ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SSN", InSSN)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        //Get R107 Info By SN
        public static DataTable GetR107SNInfoBySN(string InSN)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T 
                               WHERE SERIAL_NUMBER=:SN AND ROWNUM=1 ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", InSN)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetR107ByIMEI(string IMEI)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,VERSION_CODE,SHIPPING_SN,SHIPPING_SN2 FROM SFISM4.R_WIP_TRACKING_T 
                               WHERE SHIPPING_SN=:IMEI ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":IMEI", IMEI)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetR107ByReelNo(string reelNo)
        {
            DataTable dt = null;
            try
            {
                string  sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO =:REELNO ";
                OracleParameter[] parameter = new OracleParameter[]
                     {
                       new OracleParameter(":REELNO",reelNo),
                     };
                dt = DBHelper.GetDataTable(sql, parameter);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetR108BySN(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT A.*,B.MODEL_NAME MODEL_NAME FROM SFISM4.R_WIP_KEYPARTS_T A,SFISM4.R_WIP_TRACKING_T B 
                                WHERE A.SERIAL_NUMBER=B.SERIAL_NUMBER 
                                AND (B.SERIAL_NUMBER=:SN OR B.SHIPPING_SN =:SN) 
                                AND A.GROUP_NAME='ASSY5'";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn),
   
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetStockNoQty(string VarStockNo)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) QTY FROM SFISM4.R_WIP_TRACKING_T WHERE STOCK_NO = :STOCKNO ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":STOCKNO", VarStockNo)      
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable Get_WPON_BySN(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT A.SERIAL_NUMBER,
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
                                 WHERE (A.SERIAL_NUMBER = :SN OR A.SHIPPING_SN = :SN)
                                   AND A.SERIAL_NUMBER = B.SERIAL_NUMBER
                                   AND B.GROUP_NAME = 'ASSY1'
                                   AND B.KEY_PART_SN LIKE 'EB%'
                                   AND B.KEY_PART_SN = C.SERIAL_NUMBER";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable Get_NORKIA_BySN(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT A.SERIAL_NUMBER,
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
                                AND (A.SERIAL_NUMBER=:SN OR A.SHIPPING_SN=:SN OR A.SHIPPING_SN2=:SN)
                                AND B.GROUP_NAME IN('SWDL','RC')
                                AND B.STATUS = 'PASS' ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable Get_EERO_BOX_BySN(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT A.SERIAL_NUMBER,
                                      A.MO_NUMBER,
                                      A.MODEL_NAME,
                                      A.VERSION_CODE,
                                      B.KEY_PART_SN
                                 FROM SFISM4.R_WIP_TRACKING_T A,
                                      SFISM4.R_WIP_KEYPARTS_T B
                                WHERE A.SERIAL_NUMBER = :SN 
                                  AND A.SERIAL_NUMBER = B.SERIAL_NUMBER
                                  AND B.KEY_PART_NO LIKE '810%' 
                                  AND LENGTH(B.KEY_PART_SN) = 16
                                  AND ROWNUM = 1";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetModelType(string model)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT UPPER(MODEL_SERIAL) MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T 
                                WHERE MODEL_NAME =:MODEL AND ROWNUM = 1 ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":MODEL", model)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable GetT77BySN(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT A.SERIAL_NUMBER, 
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
                                  AND (A.SERIAL_NUMBER=:SN OR A.SHIPPING_SN=:SN OR A.SHIPPING_SN2=:SN)";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable CheckT77SN(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_T77T943_CSNMAC_T
                                WHERE CUST_SN = (SELECT SUBSTR(:SN, 1, 9) || (SUBSTR(:SN, 10, 5) + 21450) NEWSN 
                                                    FROM DUAL)";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable CheckRePrintLog()
        {
            DataTable dt = null;
            try
            {   
                string sql = @"SELECT * FROM SFISM4.R_REPRINT_LOG_T 
                                WHERE REPRINT_TIME IS NULL OR CHECKOUT_TIME IS NULL";

                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable CheckRePrintLog(string sn)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_REPRINT_LOG_T 
                                WHERE SERIAL_NUMBER <>:SN 
                                  AND (REPRINT_TIME IS NULL OR CHECKOUT_TIME IS NULL)";

                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
  
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static DataTable CheckRePrintStatus(string sn, string status)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_REPRINT_LOG_T 
                                WHERE SERIAL_NUMBER =:SN";

                if (status == "REPRINT")
                {
                    sql += " AND REPRINT_TIME IS NULL AND PRINTER IS NULL";
                }
                if (status == "CHECKOUT")
                {
                    sql += " AND REPRINT_TIME IS NOT NULL AND CHECKOUT_TIME IS NULL";
                }

                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
                };
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }
        //Update R107 
        public static int UpdateR107ScrapBySN(string InSN, string InEmpNo, string InStockNo, string InStorage)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_WIP_TRACKING_T SET SCRAP_FLAG='1',EMP_NO='{1}',STOCK_NO='{2}',WIP_GROUP='{3}',IN_STATION_TIME=SYSDATE
                                             WHERE SERIAL_NUMBER ='{0}' AND ROWNUM=1 ", InSN, InEmpNo, InStockNo, InStorage);
                result = DBHelper.ExecuteNonQuery(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }
        public static  DataTable QueryR117LimitColumn(string InSN)
        {
            DataTable dtQueryWorkTypeSPName = null;

            try
            {
                string sql = " SELECT SERIAL_NUMBER,GROUP_NAME,WIP_GROUP,IN_STATION_TIME,ATE_STATION_NO "
                            + " FROM SFISM4.R117 WHERE SERIAL_NUMBER=:InSN ORDER BY IN_STATION_TIME DESC";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":InSN", InSN)
                 };
                dtQueryWorkTypeSPName = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dtQueryWorkTypeSPName;
        }
        public static DataTable GetWip(string sField, string sValue)
        {
            DataTable dt = null;

            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE {0} = '{1}' ", sField, sValue);
                dt = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }
        //根據指定SN和指定站位和當前時間當前站位比較的分鐘數
        public static DataTable GetOverTimeMMinutesBySNandGroup(string InSN,string InGroup)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT TRUNC( (SYSDATE- MAX(IN_STATION_TIME) )*24*60) OVER_MINUTES FROM SFISM4.R_SN_DETAIL_T WHERE SERIAL_NUMBER='{0}'  AND GROUP_NAME='{1}'  ", InSN, InGroup);
                dt = DBHelper.GetDataTable(sql, null); 

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }
        //SELECT CONTROL_TIME FROM SFIS1.C_ROAST_TIME_CONTROL_T WHERE MODEL_NAME = 'T77H459.03' AND DEFAULT_GROUP = 'ROAST_IN'  AND END_GROUP = 'ROAST_OUT';
        public static DataTable GetOverTimeInfoByModelGroup(string InModel, string InStartGroup, string InEndGroup)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT CONTROL_TIME FROM SFIS1.C_ROAST_TIME_CONTROL_T WHERE MODEL_NAME = '{0}' AND DEFAULT_GROUP = '{1}' AND END_GROUP ='{2}'  ", InModel, InStartGroup, InEndGroup);
                dt = DBHelper.GetDataTable(sql, null);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }
    }
}
