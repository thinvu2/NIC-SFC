using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class StationService
    {
        /// <summary>
        /// Get Station
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllStation()
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT DISTINCT LINE_NAME,SECTION_NAME,GROUP_NAME,STATION_NAME FROM SFIS1.C_STATION_CONFIG_T 
                            WHERE HOSTID = '-1' ORDER BY LINE_NAME,GROUP_NAME";
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

        public static DataTable GetModel_labeldefine(string model, string group_Name)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_MODEL_LABELDEFINE_T WHERE MODEL_NAME=:MODEL_NAME AND GROUP_NAME=:GROUP_NAME";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":MODEL_NAME", model),
                     new OracleParameter(":GROUP_NAME", group_Name)
   
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
    }
}
