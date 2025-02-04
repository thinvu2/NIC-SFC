using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class RepairService
    {
        
        public static DataTable GetR109SNInfoRecentOneBySN(string InSN)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @" SELECT * FROM (SELECT * FROM SFISM4.R_REPAIR_T WHERE SERIAL_NUMBER=:SN ORDER BY TEST_TIME DESC) WHERE ROWNUM=1 ";
                }
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

        public static DataTable GetR109NotRepairInfoBySN(string InSN)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @" SELECT * FROM SFISM4.R_REPAIR_T WHERE SERIAL_NUMBER=:SN AND REPAIR_TIME IS NULL AND ROWNUM=1 ";
                }
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

        //Below is Check In And Out
         public static DataTable GetR109CheckInOutInfoRecentOneBySN(string InSN)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @" SELECT * FROM (SELECT SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,TEST_GROUP,TO_CHAR(TEST_TIME,'YYYY/MM/DD HH24:MI:SS') FAIL_TIME,
                             TEST_CODE FAIL_CODE,REPAIRER RE_ENGINEER,TO_CHAR(REPAIR_TIME,'YYYY/MM/DD HH24:MI:SS') REPAIR_TIME,REASON_CODE,
                             ERROR_ITEM_CODE FAIL_LOCATION,CHECKIN_TIME,CHECKOUT_TIME,CHECK_FLAG FROM SFISM4.R_REPAIR_T 
                             WHERE SERIAL_NUMBER=:SN ORDER BY TEST_TIME DESC) WHERE ROWNUM=1 ";
                }
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
        
        //Update CheckIn at R109
        public static int UpdateRepairCheckInInfoBySN(string InSN)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_REPAIR_T SET MOVE_FLAG='N',CHECK_FLAG='I',CHECKIN_TIME=SYSDATE
                                             WHERE SERIAL_NUMBER ='{0}' AND CHECKIN_TIME IS NULL ", InSN);
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
        
        //Update CheckOut at R109
        public static int UpdateRepairCheckOutInfoBySN(string InSN)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_REPAIR_T SET MOVE_FLAG='O',CHECK_FLAG='O',CHECKOUT_TIME=SYSDATE
                                             WHERE SERIAL_NUMBER ='{0}' AND CHECKOUT_TIME IS NULL ", InSN);
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
        
        //Update R109 Scrap
        public static int UpdateRepairScrapBySN(string InSN,string InRepairer,string InReasonCode)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_REPAIR_T SET REPAIRER='{1}',REASON_CODE='{2}',REPAIR_TIME=SYSDATE
                                             WHERE SERIAL_NUMBER ='{0}' AND REPAIR_TIME IS NULL ", InSN,InRepairer,InReasonCode);
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

        //Insert CheckIn at Repair_CheckInout Total Table
        public static int InsertRepairCheckInInfo(string InSN, string InMO, string InModelName, string InGroup, string InFailCode,
            string InFailTime, string InSender, string InReceiver, string InPCName, string InCheckInNo)
        {
           
            int result = 0;
            try
            {
                string sql = string.Format(@" INSERT INTO SFISM4.R_REPAIR_CHECKINOUT (SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,GROUP_NAME,FAIL_CODE, 
                                            FAIL_TIME,CHECKIN_TIME, CHECKIN_SENDER,CHECKIN_RECEIVER,CHECKIN_PCNAME,CHECKIN_NO,UPLOAD_FLAG)
                                            VALUES('{0}','{1}','{2}','{3}','{4}',to_date('{5}','YYYY/MM/DD HH24:MI:SS'),SYSDATE,'{6}','{7}','{8}','{9}','N') ",
                                            InSN, InMO, InModelName, InGroup, InFailCode, InFailTime, InSender, InReceiver, InPCName, InCheckInNo);
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
        
        //Update CheckOut at Repair_CheckInout Total Table
        public static int UpdateRepairCheckOutInfo(string InSN, string InRepairTime, string InReasonCode, string InFailLocation,
             string InREEngineer, string InSender, string InReceiver, string InPCName, string InCheckOutNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_REPAIR_CHECKINOUT SET REPAIR_TIME=to_date('{1}','YYYY/MM/DD HH24:MI:SS'), REASON_CODE='{2}',FAIL_LOCATION='{3}',
                                             RE_ENGINEER='{4}', CHECKOUT_TIME=SYSDATE,CHECKOUT_SENDER='{5}',CHECKOUT_RECEIVER='{6}',CHECKOUT_PCNAME='{7}',CHECKOUT_NO='{8}'
                                             WHERE SERIAL_NUMBER ='{0}' AND CHECKOUT_TIME IS NULL ", InSN, InRepairTime, InReasonCode,
                                               InFailLocation, InREEngineer, InSender, InReceiver, InPCName, InCheckOutNo);
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
        
        //Get YYWW
        public static string GetYearWeekYYYYWW()
        {
            try
            {
                string sql = "SELECT TO_CHAR (SYSDATE, 'YYYYWW') YearWeekYYWW FROM DUAL";
                return DBHelper.ExecuteSclar(sql, null).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
        }
        
        //NO_DETAIL需要是Number,否則Max永遠都是10
        public static DataTable GetMaxNoByNotypePre(string VarNotype, string VarPrefix)
        {
            try
            {
                string sql = "SELECT DECODE(MAX(NO_DETAIL),NULL,0,MAX(NO_DETAIL)) + 1 NEWNO FROM SFISM4.R_NO_T WHERE NO_TYPE=:Notype AND TYPE_PREFIX=:Prefix ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":Notype", VarNotype), 
                    new OracleParameter(":Prefix", VarPrefix)
                };
                return DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
        }

        public static int InsertRNoLog(string VarNotype, string VarPrefix, string VarNewNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.R_NO_T(NO_TYPE,TYPE_PREFIX,NO_DETAIL,EDIT_TIME)
               VALUES('{0}','{1}','{2}',sysdate )", VarNotype, VarPrefix, VarNewNo);

                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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
        //Above is Check In And Out

        public static int InsertScrapInfo(string InScrapNo,string InScrapStorage,string InModel,string InMO,
                                          string InSN,string InGroupName,string InWipGroup,string EmpNo,string InScrapReason)
        {

            int result = 0;
            try
            {
                string sql = string.Format(@" INSERT INTO SFISM4.R_SCRAP_DETAIL (SCRAP_NO,SCRAP_STORAGE,MODEL_NAME,MO_NUMBER,
                                              SERIAL_NUMBER,GROUP_NAME,WIP_GROUP,SCRAP_TIME,SCRAP_EMPNO,SCRAP_REASON)
                                            VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',SYSDATE,'{7}','{8}') ",
                                            InScrapNo, InScrapStorage, InModel, InMO, InSN, InGroupName, InWipGroup, EmpNo, InScrapReason);
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
    }
}
