using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class HHLabelService
    {
        //根據工單查從SAP Download的MO數據
        public static DataTable GetBPCSMOPlanDataByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM  SFISM4.R_BPCS_MOPLAN_T WHERE MO_NUMBER = '{0}' ", InMO);

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
        
        //根據年周獲取HHLabel YW SELECT HH_YW  FROM SFIS1.C_HHLABEL_YW WHERE YEARWEEK='202001'   
        public static DataTable GetHHLabelYWByYYYYWW(string InYYYYWW)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT HH_YW FROM SFIS1.C_HHLABEL_YW WHERE YEARWEEK='{0}' ", InYYYYWW);
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
        
        //獲取All HHLabelModelInfo
        public static DataTable GetHHLabelALLModelInfo()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_HHLABEL_MODEL ORDER BY MODEL_NAME ");
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
        
        //根據機種名稱獲取HHLabelModelInfo
        public static DataTable GetHHLabelModelInfoByModelName(string InModelName)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_HHLABEL_MODEL WHERE MODEL_NAME='{0}' ", InModelName);
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
       
        public static DataTable GetHHLabelPrivilege(string InAPName,string InFun,string InEmpNo)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_PRIVILEGE WHERE EMP='{0}' AND PRG_NAME='{1}' AND FUN='{2}' ",InEmpNo,InAPName,InFun);
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
        
        //根據SN獲取HHLabelPrintInfo   
        public static DataTable GetHHLabelPrintInfoBySN(string InSN)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_HHLABEL_DETAIL WHERE SERIAL_NUMBER='{0}' ", InSN);
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
        
        //根據MO獲取HHLabelPrintInfo   
        public static DataTable GetHHLabelPrintInfoByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_HHLABEL_DETAIL WHERE MO_NUMBER='{0}' ", InMO);
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
        
        //根據MO獲取HHLabelPrintRange
        public static DataTable GetHHLabelPrintRangeByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MO_NUMBER,VER_5 MODEL_NAME,VER_1 VERSION_CODE,ITEM_1 START_RANGE,ITEM_2 END_RANGE,ITEM_3 PRINT_TIME FROM SFISM4.R_MO_EXT_T WHERE MO_NUMBER='{0}' ORDER BY ITEM_1 ", InMO);
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
       
        //根據MO獲取MO最大打印次序   
        public static DataTable GetMOMaxPrintNoInfoByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT DECODE(MAX(PRINT_NO),NULL,'0',MAX(PRINT_NO)) PRINT_NO,TO_CHAR(SYSDATE,'YYYYWW') YYYY_WW FROM SFISM4.R_HHLABEL_DETAIL WHERE MO_NUMBER='{0}' ", InMO);
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

        //根據MO和打印次序獲取HHLabel最大最小SN  
        public static DataTable GetMOPrintRangeInfoByMOPrintNo(string InMO, int InPrintNo)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MIN(SERIAL_NUMBER) START_RANGE,MAX(SERIAL_NUMBER) END_RANGE,TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') DB_TIME FROM SFISM4.R_HHLABEL_DETAIL WHERE MO_NUMBER='{0}' AND PRINT_NO={1} ", InMO, InPrintNo);
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
        
        //根據MO和打印次序獲取HHLabel Detail Info  
        public static DataTable GetMOSNDetailInfoByMOPrintNo(string InMO, int InPrintNo)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_HHLABEL_DETAIL WHERE MO_NUMBER='{0}' AND PRINT_NO={1} ORDER BY SERIAL_NUMBER", InMO, InPrintNo);
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
        
        //根據產地BUDB_FLAG和YEAR_WEEK獲取HHLabel最大SN  
        public static DataTable GetMOMaxSNInfoByDBIDYW(string InBUDBID, string InYearWeek)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MAX(SERIAL_NUMBER) MAX_SN,MAX(SEQ_NO) SEQ_NO FROM SFISM4.R_HHLABEL_DETAIL WHERE BUDB_FLAG='{0}' AND YEAR_WEEK='{1}' ", InBUDBID, InYearWeek);
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
        
        //Insert
        //Insert SFIS1.C_HHLABEL_MODEL 
        public static int InsertHHLabelModelDefineInfo(string InModel, string InCustName, string InCustModel, string InRohsFlag, string InVersion,
            string InSiteFlag, string InBUDBFlag, int InRowQty, string InLabelName, string InEmpNo, string InLabelRemark, string InCustVersion, string InHWVersion, string InFWVersion, string InSWVersion)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO SFIS1.C_HHLABEL_MODEL(MODEL_NAME,CUST_NAME,CUST_MODELNAME,ROHS_FLAG,VERSION_CODE,SITE_FLAG,
                                           BUDB_FLAG,ROW_QTY,LABEL_NAME,EDIT_EMPNO,EDIT_TIME,LABEL_REMARK,CUST_VERSION,HW_VERSION,FW_VERSION,SW_VERSION )  
                                          VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8}','{9}',SYSDATE,'{10}','{11}','{12}','{13}','{14}')",
                                          InModel, InCustName, InCustModel, InRohsFlag, InVersion, InSiteFlag, InBUDBFlag, 
                                          InRowQty, InLabelName,InEmpNo, InLabelRemark, InCustVersion, InHWVersion, InFWVersion,InSWVersion);
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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
        
        //Insert SFISM4.R_HHLABEL_DETAIL
        public static int InsertHHLabelDetailInfo(string InMO, string InModel, string InVersion,
           int InPrintNo, string InBUDBFlag, string InYearWeek,string InSNSeq, string InSN, string InEmpNo)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.R_HHLABEL_DETAIL(MO_NUMBER,MODEL_NAME,VERSION_CODE,PRINT_NO,
                                            BUDB_FLAG,YEAR_WEEK,SEQ_NO,SERIAL_NUMBER,PRINT_EMPNO,PRINT_TIME )  
                                            VALUES('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}',SYSDATE)",
                                            InMO, InModel, InVersion, InPrintNo, InBUDBFlag, InYearWeek, InSNSeq,InSN, InEmpNo);
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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
        //SELECT MO_NUMBER, VER_5 MODEL_NAME,VER_1 VERSION_CODE, ITEM_1 START_RANGE,ITEM_2 END_RANGE, ITEM_3 PRINT_TIME
        public static DataTable CheckSNRange(string InMO, string InSNRange)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT MO_NUMBER,VER_5 MODEL_NAME FROM SFISM4.R_MO_EXT_T WHERE MO_NUMBER<>:MONumber AND LENGTH(:SNRange)=LENGTH(ITEM_1) AND :SNRange BETWEEN ITEM_1 AND ITEM_2  ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":MONumber", InMO),
                    new OracleParameter(":SNRange", InSNRange)
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
        public static DataTable CheckSNRangeInMO(string InMO, string InSNRange)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT MO_NUMBER,VER_5 MODEL_NAME FROM SFISM4.R_MO_EXT_T WHERE MO_NUMBER=:MONumber AND LENGTH(:SNRange)=LENGTH(ITEM_1) AND :SNRange BETWEEN ITEM_1 AND ITEM_2  ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":MONumber", InMO),
                    new OracleParameter(":SNRange", InSNRange)
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
        public static DataTable CheckSNRangeBySN(string InSNRange)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT MO_NUMBER,VER_5 MODEL_NAME FROM SFISM4.R_MO_EXT_T WHERE LENGTH(:SNRange)=LENGTH(ITEM_1) AND :SNRange BETWEEN ITEM_1 AND ITEM_2  ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":SNRange", InSNRange)
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
        public static DataTable GetMoSNInfoByMORangeStartEnd(string InMO, string InRangeStart, string InRangeEnd)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT MO_NUMBER, VER_5 MODEL_NAME,VER_1 VERSION_CODE, ITEM_1 START_RANGE,ITEM_2 END_RANGE, ITEM_3 PRINT_TIME FROM SFISM4.R_MO_EXT_T WHERE MO_NUMBER=:MONumber AND ITEM_1=:RangeStart AND ITEM_2=:RangeEnd ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":MONumber", InMO),
                    new OracleParameter(":RangeStart", InRangeStart),
                    new OracleParameter(":RangeEnd", InRangeEnd)
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
        //Insert SFISM4.R_MO_EXT_T
        public static int InsertHHLabelRangeInfo(string InMO, string InModel, string InModelRohs, string InVersion,
          string InBUDBFlag, string InPrintTime, string InSNLength, string InBeginRange, string InEndRange, string InEmpNo)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.R_MO_EXT_T(MO_NUMBER,ITEM_1,VER_1,ITEM_2,VER_2,
                                             ITEM_3,VER_3,ITEM_4,VER_4,ITEM_5,VER_5,VER_6)
                                            VALUES('{0}','{1}','{2}','{3}','{2}','{4}','{4}','{5}','{6}','{7}','{8}','{9}')",
                                            InMO, InBeginRange, InVersion, InEndRange, InPrintTime, InEmpNo, InModelRohs, InBUDBFlag,InModel, InSNLength);
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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
        //Insert SFISM4.R_MO_EXT2_T
        public static int InsertLabelRangeR1052Info(string InMO, string InModel, string InVersion,string InBeginRange, string InEndRange, string InEmpNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.R_MO_EXT2_T(MO_NUMBER,ITEM_1,VER_1,ITEM_2,VER_2, ITEM_3,VER_4,VER_5)
                                            VALUES('{0}','{1}','{2}','{3}','{2}',TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'{4}','{5}')",
                                            InMO, InBeginRange, InVersion, InEndRange, InModel, InEmpNo);
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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
        //update SFIS1.C_HHLABEL_MODEL 
        public static int UpdateHHLabelDefineByModelName(string InModel, string InCustName, string InCustModel, string InRohsFlag, string InVersion, string InSiteFlag,
             string InBUDBFlag, int InRowQty, string InLabelName, string InEmpNo,string InLabelRemark, string InCustVersion, string InHWVersion, string InFWVersion, string InSWVersion)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFIS1.C_HHLABEL_MODEL SET CUST_NAME='{1}',CUST_MODELNAME='{2}',
                                           ROHS_FLAG='{3}',VERSION_CODE='{4}',SITE_FLAG='{5}',BUDB_FLAG='{6}',ROW_QTY='{7}',
                                           LABEL_NAME='{8}',EDIT_EMPNO='{9}',EDIT_TIME=SYSDATE,LABEL_REMARK='{10}',
                                           CUST_VERSION='{11}',HW_VERSION='{12}',FW_VERSION='{13}',SW_VERSION='{14}' WHERE MODEL_NAME ='{0}' ",
                                           InModel, InCustName, InCustModel, InRohsFlag, InVersion, InSiteFlag, InBUDBFlag, InRowQty,
                                           InLabelName,InEmpNo, InLabelRemark, InCustVersion, InHWVersion, InFWVersion,InSWVersion);
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
        
        //update SFISM4.R_HHLABEL_DETAIL Only ReprintDate ReprintEMPNO 發生在補印時
        public static int UpdateHHLabelReprintBySN(string InSN, string InEmpNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_HHLABEL_DETAIL SET REPRINT_EMPNO='{1}',REPRINT_TIME=SYSDATE WHERE SERIAL_NUMBER ='{0}' ",
                                           InSN, InEmpNo);
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
       
        //SFISM4.R_MO_EXT_T不存在Update
        //Delete SFIS1.C_HHLABEL_MODEL
        public static int DeleteHHLabelDefineByModelName(string InModelName)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"DELETE SFIS1.C_HHLABEL_MODEL WHERE MODEL_NAME ='{0}' ",InModelName);
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
        //系统时间的YYYYMMDDHHMISS
        public static DataTable GetSysdateYYYYMMDDHHMISS()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') YMDHMS FROM DUAL  ");
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
        public static DataTable GetModelBoardQty(string InModelName)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT DECODE(PRODUCT_NAME,NULL,'0',PRODUCT_NAME) BOARD_QTY,DECODE(VERSION_CODE,NULL,'B',VERSION_CODE) BOARD_TYPE FROM SFIS1.C_MODEL_DESC_T
                               WHERE MODEL_NAME =:MODEL AND ROWNUM = 1 ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":MODEL", InModelName)

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
        public static int UpdateModelBoardQty(string InModelName, string InBoardQty, string InBoardSide)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFIS1.C_MODEL_DESC_T SET PRODUCT_NAME='{1}',VERSION_CODE='{2}' WHERE MODEL_NAME ='{0}' ",
                                           InModelName, InBoardQty, InBoardSide);
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
        public static int InsertVirtualMO(string InMO, string InModel,string InTargetQty,string InWHS,string InSite,string InSapType,string InMOtype)
        {
            int result = 0;
            try
            {
                string sql = string.Format("INSERT INTO SFISM4.R_BPCS_MOPLAN_T (MO_NUMBER, MO_TYPE, MODEL_NAME, TARGET_QTY, MO_SCHEDULE_DATE, "
                                          + "MO_DUE_DATE,KEY_PART_NO,CUST_CODE,WHS,SITE,REFERENCE_MO,SAP_MODEL_NAME,SAP_MO_TYPE,SO_NUMBER,SO_LINE,CUSTPN ) "
                                          + "VALUES('{0}','{1}','{2}','{3}',sysdate,sysdate,'{2}','80190','{4}','{5}','{6}','{2}','{7}','N/A','N/A','N/A') ",
                                          InMO, InMOtype, InModel, InTargetQty, InWHS, InSite,InMO.Substring(3), InSapType);
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

        //根據MO獲取PanelNoPrintInfo   
        public static DataTable GetPanelNoPrintInfoByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_PANELNO_DETAIL WHERE MO_NUMBER='{0}' ", InMO);
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
        //根據MO獲取Panel最大打印次序   
        public static DataTable GetMOPanelMaxPrintNoInfoByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT DECODE(MAX(PRINT_NO),NULL,'0',MAX(PRINT_NO)) PRINT_NO,TO_CHAR(SYSDATE,'YYYYWW') YYYYWW, TO_CHAR(SYSDATE,'YYYYMMDD') YYYYMMDD 
                                             FROM SFISM4.R_PANELNO_DETAIL WHERE MO_NUMBER='{0}' ", InMO);
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

        //根據產地BUDB_FLAG和YEAR_WEEK獲取HHLabel最大SN  
        public static DataTable GetPanelNoMaxSNInfoByPanelPrefixYW(string InPanelPrefix, string InYearWeek)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MAX(SERIAL_NUMBER) MAX_SN,MAX(SEQ_NO) SEQ_NO FROM SFISM4.R_PANELNO_DETAIL WHERE PANEL_PREFIX='{0}' AND YEAR_WEEK='{1}' ", InPanelPrefix, InYearWeek);
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

        //Insert SFISM4.R_HHLABEL_DETAIL
        public static int InsertPanelNoDetailInfo(string InMO, string InModel, string InVersion,
           int InPrintNo, string InPanelPrefix, string InYearWeek, string InSNSeq, string InSN, string InEmpNo)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.R_PANELNO_DETAIL(MO_NUMBER,MODEL_NAME,VERSION_CODE,PRINT_NO,
                                            PANEL_PREFIX,YEAR_WEEK,SEQ_NO,SERIAL_NUMBER,PRINT_EMPNO,PRINT_TIME )  
                                            VALUES('{0}','{1}','{2}',{3},'{4}','{5}','{6}','{7}','{8}',SYSDATE)",
                                            InMO, InModel, InVersion, InPrintNo, InPanelPrefix, InYearWeek, InSNSeq, InSN, InEmpNo);
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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

        public static DataTable GetPanelPrintRangeInfoByMOPrintNo(string InMO, int InPrintNo)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MIN(SERIAL_NUMBER) START_RANGE,MAX(SERIAL_NUMBER) END_RANGE,TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') DB_TIME FROM SFISM4.R_PANELNO_DETAIL WHERE MO_NUMBER='{0}' AND PRINT_NO={1} ", InMO, InPrintNo);
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
        public static DataTable GetMOPanelInfoByMOPrintNo(string InMO, int InPrintNo)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_PANELNO_DETAIL WHERE MO_NUMBER='{0}' AND PRINT_NO={1} ORDER BY SERIAL_NUMBER", InMO, InPrintNo);
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

        //根據SN獲取HHLabelPrintInfo   
        public static DataTable GetPanelNoPrintInfoBySN(string InSN)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_PANELNO_DETAIL WHERE SERIAL_NUMBER='{0}' ", InSN);
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

        public static int UpdatePanelReprintInfoBySN(string InSN, string InEmpNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_PANELNO_DETAIL SET REPRINT_EMPNO='{1}',REPRINT_TIME=SYSDATE WHERE SERIAL_NUMBER ='{0}' ",
                                           InSN, InEmpNo);
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
        //IMEI
        //根據工單獲取最大SeqNo  
        public static DataTable GetMOIMEIMaxSeqNo(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MAX(SN) MAX_SN,MAX(SEQ) SEQ_NO FROM SFISM4.R_BGS3_CSN_T WHERE MO_NUMBER='{0}' ", InMO);
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
        //根據工單獲取已經打印的信息
        public static DataTable GetIMEILabelPrintedInfoByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MO_NUMBER,SN,SEQ,HW,CUST_VERSION,VERSION_CODE,RELEASE_TIME,PRINT_TIME 
                                             FROM SFISM4.R_BGS3_CSN_T WHERE MO_NUMBER='{0}' AND  PRINT_FLAG='Y' ", InMO);
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
        //根據工單和選定的區間,查Release信息
        public static DataTable CheckIMEIRangeIsNotReleaseByMo(string InMO, string InRangeBegin, string InRangeEnd)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MO_NUMBER,SN,SEQ,HW,CUST_VERSION,VERSION_CODE,RELEASE_TIME,PRINT_TIME 
                            FROM SFISM4.R_BGS3_CSN_T WHERE MO_NUMBER='{0}' AND SUBSTR(SN,1,14) BETWEEN '{1}' AND '{2}' ",
                            InMO, InRangeBegin, InRangeEnd);
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
        //IMS.MMS_RANGE_USED @IMS.WORLD =SFISM4.IMS_RANGE_USED
        //從IMS系統中獲取IMEI信息  FRONT_ID||BEGIN_ID IMEI_BEGIN,FRONT_ID||END_ID IMEI_END
        public static DataTable GetIMEIRangeFromIMS(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT FRONT_ID||BEGIN_ID||'~'||FRONT_ID||END_ID||'~'||QUANTITY IMEI_RANGE
                              FROM SFISM4.IMS_RANGE_USED WHERE MO_NO='{0}' ORDER BY FRONT_ID||BEGIN_ID ", InMO);                 
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
        public static DataTable GetIMEILabelMayPrintInfoByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MO_NUMBER,SN,SEQ,HW,CUST_VERSION,VERSION_CODE,RELEASE_TIME,PRINT_TIME 
                                             FROM SFISM4.R_BGS3_CSN_T WHERE MO_NUMBER='{0}' AND  PRINT_FLAG='0' ", InMO);
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
        //通過傳入的SequenceName,得到Next SeqNo
        public static DataTable GetSeqNoBySequenceName(string InSequenceName)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT {0}.NEXTVAL SEQ_NO FROM DUAL", InSequenceName);
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
        //更新將要打印的IMEI
        public static int UpdateWillPrintIMEIInfo(string InMO, string InSeqNo,int InWillPrintQty)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"
                UPDATE SFISM4.R_BGS3_CSN_T SET PRINT_FLAG = 'Y',PRINT_TIME = SYSDATE,
                PRINT_EMP = '{1}' WHERE SN IN (SELECT SN FROM (SELECT SN FROM
                SFISM4.R_BGS3_CSN_T WHERE MO_NUMBER = '{0}' AND PRINT_FLAG = '0'
                ORDER BY SN) WHERE ROWNUM <= {2}) ", InMO, InSeqNo, InWillPrintQty);
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
        //獲取具體將要打印的IMEI
        public static DataTable GetWillPrintIMEIInfoByMO(string InMO,string InSeqNo)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MO_NUMBER,SEQ,SN,HW,CUST_VERSION,VERSION_CODE,MODEL_NAME,
                DECODE(TO_CHAR(SYSDATE,'YY'),'20','M','21','N','22','P','23','Q','23','R','24','S','25','T','26','U','27','V')|| 
                DECODE(TO_CHAR(SYSDATE,'MM'),'10','O', '11', 'N', '12','D',SUBSTR(TO_CHAR(SYSDATE, 'MM'), 2)) YM 
                FROM SFISM4.R_BGS3_CSN_T WHERE MO_NUMBER = '{0}' AND PRINT_EMP = '{1}' ORDER BY SN", InMO, InSeqNo);
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
        //根據MO和打印次序獲取HHLabel最大最小SN  
        public static DataTable GetIMEIPrintRangeInfoByMOSeqNo(string InMO, string InSeqNo)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MIN(SN) START_RANGE,MAX(SN) END_RANGE,TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS') DB_TIME 
                                             FROM SFISM4.R_BGS3_CSN_T WHERE MO_NUMBER='{0}' AND PRINT_EMP='{1}' ", InMO, InSeqNo);
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

        public static int InsertIMEIDetailInfo(string InMO, string InIMEI, string InIMEIPrefix, string InCheckSum, 
            string InSNSeq,string InFWVersion,string InCustVersion,string InVersion, string InModel)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.R_BGS3_CSN_T(MO_NUMBER,SN,SN_PREFIX,CHECKSUM,
                                             RELEASE_TIME,SEQ,HW,CUST_VERSION,VERSION_CODE,MODEL_NAME)  
                                            VALUES('{0}','{1}','{2}',{3},SYSDATE,'{4}','{5}','{6}','{7}','{8}')",
                                            InMO, InIMEI, InIMEIPrefix, InCheckSum, InSNSeq, InFWVersion, InCustVersion, InVersion,InModel);
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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
        public static string GetNextSn(string InSN, string myString)
        {
            char laststr;
            string last;
            string tempsn;
            int snLength, strLength;
            strLength = myString.Length;
            snLength = InSN.Length;
            laststr = Convert.ToChar(InSN.Substring(snLength - 1, 1));
            if (myString[strLength - 1] == laststr)
            {
                if (snLength != 1)
                {
                    laststr = myString[0];
                    last = Convert.ToString(laststr);
                    tempsn = GetNextSn(InSN.Substring(0, snLength - 1), myString) + last;
                    return tempsn;
                }
                else
                {
                    return "--";
                }
            }
            else
            {
                tempsn = InSN.Substring(0, snLength - 1) + myString[myString.IndexOf(laststr) + 1];
                return tempsn;
            }
        }
        public static string getdateYMD(string InYYYYMMDD)
        {
            string VarYear, VarMonth, VarDay, VarYYMD;
            string VaildChar = "123456789ABCDEFGHJKLMNPRSTUVWXY";
            VarYear = InYYYYMMDD.Substring(3, 1);
            VarMonth = InYYYYMMDD.Substring(4, 2);
            VarDay = InYYYYMMDD.Substring(6, 2);
            VarMonth = VaildChar[Convert.ToInt16(VarMonth) - 1].ToString();
            VarDay = VaildChar[Convert.ToInt16(VarDay) - 1].ToString();
            VarYYMD = VarYear + VarMonth + VarDay;
            return VarYYMD;
        }
        //根據傳入的IMEI計算出檢驗碼
        public static string getCheckSumByIMEI(string InIMEI)
        {
            int sum, temp, modulo;
            bool alt;
            string CheckSum = "";
            sum = 0;
            alt = true;
            for (int i = InIMEI.Length - 1; i >= 0; i--)
            {
                if (alt)
                {
                    temp = Convert.ToByte(InIMEI[i]) - 48;
                    temp = temp * 2;
                    if (temp > 9)
                    {
                        temp = temp - 9;
                    }
                    sum = sum + temp;
                }
                else
                {
                    sum = sum + Convert.ToByte(InIMEI[i]) - 48;
                }
                alt = !alt;
            }
            modulo = sum % 10;
            if (modulo > 0)
            {
                CheckSum = Convert.ToChar(10 - modulo + 48).ToString();
            }
            else
            {
                CheckSum = Convert.ToChar(48).ToString();
            }
            return CheckSum;
        }
    }
}
