using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class ReportWIP
    {
        public static DataTable GetSysDate()
        {
            DataTable dt = null;
            try
            {
                string sql = @" SELECT TO_CHAR(SYSDATE,'YYYY/MM/DD') WORKSTART,
                            TO_CHAR(SYSDATE+30,'YYYY/MM/DD') WORKEND FROM DUAL ";
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
        //查询C_LINE的All LineName List
        public static DataTable GetLineNameList()
        {
            DataTable dt = null;
            try
            {
                string sql = @" SELECT 'ALL' LINE_NAME FROM DUAL  
                                UNION
                                SELECT DISTINCT LINE_NAME FROM SFIS1.C_LINE_DESC_T 
                                ORDER BY LINE_NAME ";
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
        //查询C_GROUP的All GroupName List
        public static DataTable GetGroupNameList()
        {
            DataTable dt = null;
            try
            {
                string sql = @" SELECT 'ALL' GROUP_NAME FROM DUAL  
                                UNION
                                SELECT DISTINCT GROUP_NAME FROM SFIS1.C_GROUP_CONFIG_T 
                                ORDER BY GROUP_NAME ";
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

        //查询C_WORK_CLASS的All GroupName List
        public static DataTable GetWorkShiftList()
        {
            DataTable dt = null;
            try
            {
                string sql = @" SELECT 'ALL' WORK_SHIFT FROM DUAL ";
                /*UNION
                                SELECT DISTINCT SHIFT WORK_SHIFT FROM SFIS1.C_WORK_CLASS_T 
                                ORDER BY SHIFT ";*/
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
        
        //查询C_MODEL的All ModelSeries(产品系列) List
        public static DataTable GetModelSeriesList()
        {
            DataTable dt = null;
            try
            {
                string sql = @" SELECT 'ALL' MODEL_SERIES FROM DUAL  
                                UNION
                                SELECT DISTINCT MODEL_SERIAL MODEL_SERIES FROM SFIS1.C_MODEL_DESC_T 
                                ORDER BY MODEL_SERIES ";
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
        //查询C_MODEL的All CustomerName List
        public static DataTable GetCustomerNameList()
        {
            DataTable dt = null;
            try
            {
                string sql = @" SELECT 'ALL' CUSTOMER_NAME FROM DUAL  
                                UNION
                                SELECT CUSTOMER_NAME FROM SFIS1.SFIS1.C_CUSTOMER_T ORDER BY CUSTOMER_NAME ";
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
        
        //按照CustomerName查ModelName 从数据库中查询
        public static DataTable GetModelNameListByCustomer(string VarCustomerName)
        {
            DataTable dt = null;
            try
            {
                string sql = @" SELECT 'ALL' MODEL_NAME FROM DUAL  
                                UNION
                                SELECT DISTINCT MODEL_NAME FROM SFIS1.C_MODEL_DESC_T WHERE CUSTOMER =:CustomerName
                                ORDER BY MODEL_NAME ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":CustomerName", VarCustomerName)
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
       
        //get 
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
    }

}
