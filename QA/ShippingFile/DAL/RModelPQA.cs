using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class RModelPQA
    {
        public static DataTable GetOBAData()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT WORK_DATE AS 日期, WORK_SECTION AS 時間, MODEL_NAME AS 機種,
                           GROUP_NAME AS 站位, STATION_NAME AS 測試機台, PASS_QTY AS 測試數量, PQA_QTY AS 檢測數量,
                  FAIL_QTY AS 不良數量 FROM SFISM4.R_MODEL_PQA_T WHERE WORK_DATE >= TO_CHAR(SYSDATE - 3, 'YYYYMMDD')
                  AND PASS_QTY>10 ORDER BY WORK_DATE DESC,WORK_SECTION DESC ");
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
        //Get R107 Info By SSN
        public static DataTable GetOBADataByWorkDate(string InWorkDate)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT WORK_DATE AS 日期,WORK_SECTION AS 時間, MODEL_NAME AS 機種,
                           GROUP_NAME AS 站位,STATION_NAME AS 測試機台, PASS_QTY AS 測試數量,PQA_QTY AS 檢測數量
                          FROM SFISM4.R_MODEL_PQA_T WHERE WORK_DATE = '{0}' ORDER BY MODEL_NAME, GROUP_NAME ", InWorkDate);
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

        public static DataTable GetWorkDate()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT DISTINCT WORK_DATE FROM SFISM4.R_MODEL_PQA_T WHERE WORK_DATE >= TO_CHAR(SYSDATE - 300, 'YYYYMMDD') ORDER BY WORK_DATE ");
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
        public static DataTable GetModelNameByWorkDate(string InWorkDate)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT DISTINCT MODEL_NAME FROM SFISM4.R_MODEL_PQA_T WHERE WORK_DATE= '{0}' ORDER BY MODEL_NAME ", InWorkDate);
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
        public static DataTable GetModelName()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT DISTINCT MODEL_NAME FROM SFISM4.R_MODEL_PQA_T WHERE OBA_QTY = 0 ORDER BY MODEL_NAME ");
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

        public static DataTable GetGroupName(string InModelName)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT DISTINCT GROUP_NAME FROM SFISM4.R_MODEL_PQA_T WHERE MODEL_NAME = '{0}' AND OBA_QTY = 0 ", InModelName);
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

        public static DataTable GetStationNameByWorkDateModel(string InWorkDate,string InModelName)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT DISTINCT STATION_NAME FROM SFISM4.R_MODEL_PQA_T WHERE  
               WORK_DATE= '{0}' AND MODEL_NAME = '{1}' ORDER BY STATION_NAME", InWorkDate,InModelName);
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

        public static DataTable GetNotOBAData(string InModelName, string InGroupName, string InATEStation, string InTrayNO, string InCartonNO, string InPalletNO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT SERIAL_NUMBER,SHIPPING_SN,MO_NUMBER,MODEL_NAME,TRAY_NO,CARTON_NO,PALLET_NO,WIP_GROUP FROM SFISM4.R107
                 WHERE SERIAL_NUMBER IN (SELECT SERIAL_NUMBER FROM SFISM4.R117 WHERE SERIAL_NUMBER IN
                 (SELECT SERIAL_NUMBER  FROM SFISM4.R107 WHERE TRAY_NO = '{3}'  AND MODEL_NAME = '{0}' )
                 AND GROUP_NAME = '{1}'  AND ATE_STATION_NO = '{2}'  AND ERROR_FLAG = '0') ", InModelName, InGroupName, InATEStation, InTrayNO, InCartonNO, InPalletNO);
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

        public static DataTable GetOBADataByWhere(string InWorkDate, string InModelName, string InATEStation )
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@" SELECT A.WORK_DATE AS 日期,A.MODEL_NAME AS 機種,A.STATION_NAME AS 測試棧台,A.WORK_SECTION AS 時間,A.GROUP_NAME AS 站位,
                              A.PASS_QTY AS 測試數量,A.PQA_QTY AS 檢測數量 ,FAIL_QTY AS 不良數量  from SFISM4.R_MODEL_PQA_T A WHERE A.WORK_DATE='{0}' ", InWorkDate);
                if (!string.IsNullOrEmpty(InModelName))
                {
                    sql += string.Format(" AND MODEL_NAME='{0}' ", InModelName);
                }
                if (!string.IsNullOrEmpty(InATEStation))
                {
                    sql += string.Format(" AND STATION_NAME='{0}' ", InATEStation);
                }
                sql += " ORDER BY WORK_DATE DESC,WORK_SECTION DESC,MODEL_NAME DESC ";
                //, InWorkDate,InModelName, InATEStation);
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
