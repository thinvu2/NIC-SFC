using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    /*SFISM4.R_SMARTCARD_INFO
     ( SMARTCARD_SN       VARCHAR2 (25 BYTE),
       SMARTCARD_STATUS VARCHAR2(25 BYTE),
       CHECKIN_DATE VARCHAR2(15 BYTE),
       CHECKIN_TIME DATE,
       CHECKIN_EMPNO      VARCHAR2(25 BYTE),
       CHECKIN_EMPNAME VARCHAR2(25 BYTE),
       CHECKIN_PCNAME VARCHAR2(25 BYTE),
       CHECKIN_NO NUMBER,
       CHECKOUT_MO        VARCHAR2(25 BYTE),
       CHECKOUT_MODEL VARCHAR2(25 BYTE),
       CHECKOUT_DATE VARCHAR2(15 BYTE),
       CHECKOUT_TIME DATE,
       CHECKOUT_EMPNO     VARCHAR2(25 BYTE),
       CHECKV_EMPNAME VARCHAR2(25 BYTE),
       CHECKOUT_PCNAME VARCHAR2(25 BYTE),
       CHECKOUT_NO NUMBER,
       LINK_DATE          VARCHAR2(15 BYTE),
       SERIAL_NUMBER VARCHAR2(25 BYTE),
       ACTUAL_MO VARCHAR2(25 BYTE),
       ACTUAL_MODEL VARCHAR2(25 BYTE),
       LINK_TIME DATE,
       LINK_EMPNO         VARCHAR2(25 BYTE),
       LINK_PCNAME VARCHAR2(25 BYTE)
       SmartCard来到工厂后,库房扫描入G101贵重物料仓,记录哪天入库的是谁入库的以及具体时间,第几次入库的
       领用时:以工单为单位领用,但实际使用可能不是此工单,所以会记录实际工单
         */

    public class SmartCardService
    {
        
        public static DataTable GetSmartCardAllInfo()
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @" SELECT SMARTCARD_SN,SMARTCARD_STATUS,CHECKIN_TIME,CHECKOUT_TIME,LINK_TIME,SERIAL_NUMBER FROM SFISM4.R_SMARTCARD_INFO ORDER BY SMARTCARD_SN  ";
                } 
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

        public static DataTable GetSmartCardAllInfoBySmartSN(string InSmartCardSN)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @" SELECT SMARTCARD_SN,SMARTCARD_STATUS,CHECKIN_TIME,CHECKIN_NO,CHECKOUT_TIME,CHECKOUT_NO,LINK_TIME,SERIAL_NUMBER FROM SFISM4.R_SMARTCARD_INFO WHERE SMARTCARD_SN=:SN AND ROWNUM=1 ";
                }
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SmartCardSN", InSmartCardSN)
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

        public static DataTable GetSmartCardAllInfoBySN(string InSN)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @" SELECT * FROM SFISM4.R_SMARTCARD_INFO WHERE SERIAL_NUMBER=:SN AND ROWNUM=1 ";
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
        //
        public static DataTable GetTotalSummaryData()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT SMARTCARD_STATUS,COUNT(SMARTCARD_SN) QTY ,CHECKOUT_MO,CHECKOUT_DATE,ACTUAL_MO,CHECKOUT_NO 
                                              FROM SFISM4.R_SMARTCARD_INFO 
                                              GROUP BY SMARTCARD_STATUS,CHECKOUT_MO,CHECKOUT_DATE,CHECKOUT_NO,ACTUAL_MO
                                              ORDER BY SMARTCARD_STATUS,CHECKOUT_MO,CHECKOUT_DATE,CHECKOUT_NO,ACTUAL_MO ");
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
        public static DataTable GetSummaryData(string InStartDate, string InEndDate)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT SMARTCARD_STATUS,COUNT(SMARTCARD_SN) QTY ,CHECKOUT_MO,CHECKOUT_DATE,ACTUAL_MO,CHECKOUT_NO 
                                              FROM SFISM4.R_SMARTCARD_INFO WHERE CHECKOUT_DATE BETWEEN '{0}' AND '{1}' 
                                              GROUP BY SMARTCARD_STATUS,CHECKOUT_MO,CHECKOUT_DATE,CHECKOUT_NO,ACTUAL_MO
                                              ORDER BY SMARTCARD_STATUS,CHECKOUT_MO,CHECKOUT_DATE,CHECKOUT_NO,ACTUAL_MO ", InStartDate, InEndDate);
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
        public static DataTable GetSmartCardSNData(string InStartDate, string InEndDate,string InSmartCardStatus, string InCheckOutMo, string InCheckOutDate, string InCheckOutNo, string InActualMo)
        {
            DataTable dt = null;
            try
            {//SmartCardStatus, CheckOutMo, CheckOutDate, CheckOutNo, ActualMo
                string sql = string.Format(@" SELECT SMARTCARD_STATUS,SMARTCARD_SN,CEIL((SYSDATE-NVL(CHECKOUT_TIME,SYSDATE))*24) OUT_HOURS,CHECKIN_TIME,CHECKOUT_TIME, LINK_TIME,CHECKOUT_MO,CHECKOUT_MODEL,SERIAL_NUMBER,ACTUAL_MO,ACTUAL_MODEL
                                              FROM SFISM4.R_SMARTCARD_INFO WHERE CHECKOUT_DATE  BETWEEN '{0}' AND '{1}' AND SMARTCARD_STATUS='{2}' AND 
                                             CHECKOUT_MO='{3}' AND CHECKOUT_DATE ='{4}' AND CHECKOUT_NO='{5}' AND ACTUAL_MO='{6}' 
                                             ORDER BY OUT_HOURS, SMARTCARD_SN ", InStartDate, InEndDate,InSmartCardStatus, 
                                            InCheckOutMo, InCheckOutDate, InCheckOutNo, InActualMo);
                
                dt = DBHelper.GetDataTable(sql,null);
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
        //Insert CheckIn at Repair_CheckInout Total Table
        public static int InsertSmartCardCheckInInfo(string InSmartCardSN, string InSmartStatus, string InCheckDate, string InCheckEmpno, string InCheckEmpName, string InPCName, string InCheckInNo)
        {
           
            int result = 0;
            try
            {
                string sql = string.Format(@" INSERT INTO SFISM4.R_SMARTCARD_INFO (SMARTCARD_SN,SMARTCARD_STATUS,
                                              CHECKIN_DATE,CHECKIN_TIME,CHECKIN_EMPNO,CHECKIN_EMPNAME,CHECKIN_PCNAME,CHECKIN_NO)
                                            VALUES('{0}','{1}','{2}',SYSDATE,'{3}','{4}','{5}','{6}') ",
                                            InSmartCardSN, InSmartStatus, InCheckDate, InCheckEmpno, InCheckEmpName,InPCName, InCheckInNo);
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

        //Update CHECKOUT_MO,CHECKOUT_MODEL,CHECKOUT_DATE,CHECKOUT_TIME,CHECKOUT_EMPNO,CHECKV_EMPNAME,CHECKOUT_PCNAME,CHECKOUT_NO
         public static int UpdateSmartCardCheckOutInfo(string InSmartCardSN, string InMO, string InModel, string InSmartStatus, string InCheckDate, string InCheckEmpno, string InCheckEmpName, string InPCName, string InCheckOutNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.R_SMARTCARD_INFO SET CHECKOUT_MO='{1}', CHECKOUT_MODEL='{2}',SMARTCARD_STATUS='{3}',ACTUAL_MO='NoLink',
                                             CHECKOUT_DATE='{4}', CHECKOUT_TIME=SYSDATE,CHECKOUT_EMPNO='{5}',CHECKOUT_EMPNAME='{6}',CHECKOUT_PCNAME='{7}',CHECKOUT_NO='{8}'
                                             WHERE SMARTCARD_SN ='{0}' AND ROWNUM=1 ", InSmartCardSN, InMO, InModel, InSmartStatus,
                                               InCheckDate, InCheckEmpno, InCheckEmpName, InPCName, InCheckOutNo);
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
        public static string GetYearWeekYYYYMMDD()
        {
            try
            {
                string sql = "SELECT TO_CHAR (SYSDATE, 'YYYYMMDD') YearMonthDay FROM DUAL";
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

        //CHECKIN_NO栏位需要是Number,否則Max永遠都是10
        public static DataTable GetMaxCheckInNo()
        {
            try
            {
                string sql = "SELECT DECODE(MAX(CHECKIN_NO),NULL,0,MAX(CHECKIN_NO)) + 1 NEWNO FROM SFISM4.R_SMARTCARD_INFO "; 
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetMaxCheckOutNo()
        {
            try
            {
                string sql = "SELECT DECODE(MAX(CHECKOUT_NO),NULL,0,MAX(CHECKOUT_NO)) + 1 NEWNO FROM SFISM4.R_SMARTCARD_INFO ";
                return DBHelper.GetDataTable(sql, null);
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

        public static DataTable GetDataByTableNameColumnAndTerms(string InTableName, string InColumnName, string InWhereColumnName,string InTermData)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT {1} FROM {0} WHERE {2} IN {3}   ", InTableName, InColumnName, InWhereColumnName, InTermData);
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

        public static DataTable GetDataForNokiaByDN(string InTermData)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT C.INVOICE, A.MODEL_NAME PART_NUMBER,A.SERIAL_NUMBER,B.DATA4 MACID,B.DATA5 IMEI, B.DATA6 SSID,
                 B.DATA8 PASSWORD,B.DATA9 USERNAME,SUBSTR(A.SERIAL_NUMBER, 3, 2) YERA_CODE,SUBSTR(A.SERIAL_NUMBER, 5, 2) WEEK_CODE,B.UNITLEVEL_VER SW
                  FROM SFISM4.Z107 A, SFISM4.R_NOKIA_TEST_DATA_T B, 
                   (SELECT INVOICE,TCOM FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE  in {0} ) C
                 WHERE C.TCOM=A.SHIP_NO AND A.SERIAL_NUMBER = B.SERIAL_NUMBER
                   AND B.GROUP_NAME = 'FT' AND B.STATUS = 'PASS'  ORDER BY 1,3 ", InTermData);
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

        public static DataTable GetDataForNokiaTMOByDN(string InTermData)
        {
            DataTable dt = null;
            try
            {//还需要修改,没有最终确认,因为这是5张表的关联,我想的是缩减到三张表,并且有些是固定值,这也是不对的
                string sql = string.Format(@"SELECT A.MODEL_NAME CPE_PART_NUMBER,A.SERIAL_NUMBER CPE_SERIAL_NUMBER, '1' CPE_ICS,'2' CPE_MREV,A.MODEL_NAME KIT_PART_NUMBER,'1' KIT_ICS,'2' KIT_MREV,
                        D.DATA4 MACID,'ZZZ260R060' SIM_CARD_PART_NUMBER,'3TG01445AAAA' NOKIA_SIM_CARD_PART_NUMBER,E.SMARTCARD_SN SIM_CARD_SERIAL_NUMBER,
                        D.DATA5 IMEI, A.CARTON_NO,A.PALLET_NO,'610214664655' SKU_NUMBER,D.DATA7 SSID,D.DATA8  SSID_PW,D.DATA9 USERNAME,B.DATA5 SW_VERSION  
                        FROM SFISM4.Z107 A, (SELECT * FROM SFISM4.R_NOKIA_TEST_DATA_T WHERE GROUP_NAME = 'DL_TMO' AND STATUS = 'PASS'  ) B ,
                   (SELECT * FROM SFISM4.R_NOKIA_TEST_DATA_T WHERE GROUP_NAME = 'RI_TMO' AND STATUS = 'PASS'  ) D, 
                   (SELECT INVOICE,TCOM FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE  in {0} ) C,SFISM4.R_SMARTCARD_INFO E
                 WHERE C.TCOM=A.SHIP_NO   AND A.SERIAL_NUMBER = D.SERIAL_NUMBER  AND A.SERIAL_NUMBER = B.SERIAL_NUMBER AND A.SERIAL_NUMBER = E.SERIAL_NUMBER(+)
                ORDER BY CPE_SERIAL_NUMBER ", InTermData);
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
