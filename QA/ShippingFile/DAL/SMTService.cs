using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class SMTService
    {
        //SMT相关的SQL
        //VR_VALUE是AllParts先别名称
        public static DataTable GetAllPartsLineName(string InLineName)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME='ALLPARTS' AND VR_ITEM='SMT' AND VR_NAME = '{0}' ", InLineName);

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
        //SELECT * FROM SFISM4.R_SN_TRSN_LINK_T  WHERE MO_NUMBER = :MO AND TRSN = :TRSN
        public static DataTable GetTRSNLinkInfoByMoTRSN(string InMo, string InTRSN)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM SFISM4.R_SN_TRSN_LINK_T WHERE MO_NUMBER='{0}' AND TRSN = '{1}' AND ROWNUM=1 ", InMo, InTRSN);

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
       //获取当前班别信息
        public static DataTable GetCurrentClassWorkDateInfo()
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format(" SELECT WORK_SECTION,DAY_DISTINCT,CLASS, TO_CHAR(SYSDATE,'YYYYMMDD') WORK_DATE, "
                                  + " DECODE(DAY_DISTINCT,'TOMORROW',TO_CHAR(SYSDATE-1,'YYYYMMDD'),'YESTERDAY', "
                                  + " TO_CHAR(SYSDATE+1,'YYYYMMDD'), TO_CHAR(SYSDATE,'YYYYMMDD')) CLASS_DATE "
                                  + " FROM SFIS1.C_WORK_DESC_T WHERE START_TIME <= TO_CHAR(SYSDATE,'HH24MI') "
                                  + " AND END_TIME > TO_CHAR(SYSDATE,'HH24MI') AND LINE_NAME = 'Default' ");
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
        //SELECT DECODE(TOTAL_QTY,NULL,'0', TOTAL_QTY) TOTAL_QTY FROM ( SELECT SUM(PASS_QTY+FAIL_QTY+REPASS_QTY+REFAIL_QTY) TOTAL_QTY FROM 
        public static DataTable GetClassProductQtyByWOLineGroupClass(string InWO, string InGroup, string InClassType, string InClassDate, string InLineName, string InSectionName)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format(" SELECT DECODE(TOTAL_QTY,NULL,'0', TOTAL_QTY) TOTAL_QTY FROM ( "
                                  + " SELECT SUM(PASS_QTY+FAIL_QTY+REPASS_QTY+REFAIL_QTY) TOTAL_QTY FROM "
                                  + " SFISM4.R_STATION_REC_T WHERE MO_NUMBER='{0}' AND GROUP_NAME = '{1}' AND CLASS = '{2}' "
                                  + " AND CLASS_DATE = '{3}' AND LINE_NAME = '{4}' AND SECTION_NAME = '{5}')  ",
                                  InWO, InGroup, InClassType, InClassDate, InLineName, InSectionName);

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
