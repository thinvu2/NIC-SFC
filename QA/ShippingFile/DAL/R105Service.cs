using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class R105Service
    {
        public static DataTable GetR105POInfoByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER = '{0}' ORDER BY MO_NUMBER ", InMO);
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
        public static DataTable GetR105POInfoByPO(string InPO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT PO_NO,MO_NUMBER,MODEL_NAME,TARGET_QTY, OUTPUT_QTY 
                                               FROM SFISM4.R_MO_BASE_T  
                                              WHERE PO_NO = '{0}' ORDER BY MO_NUMBER ", InPO);
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
        public static int UpdatePONoByMO(string InPO, string InMO)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@" UPDATE SFISM4.R_MO_BASE_T SET PO_NO='{0}' WHERE MO_NUMBER='{1}' AND ROWNUM=1  ", InPO, InMO);

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

        public static int UpdatePONoByMO(string InMO)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@" UPDATE SFISM4.R_MO_BASE_T  SET PO_NO='' WHERE MO_NUMBER='{0}' AND ROWNUM=1  ", InMO);

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

        public static int UpdateR105VersionCodeByMO(string InMO, string InVersion)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@" UPDATE SFISM4.R_MO_BASE_T  SET VERSION_CODE='{1}' WHERE MO_NUMBER='{0}' AND ROWNUM=1  ", InMO,InVersion);

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

    }
}
