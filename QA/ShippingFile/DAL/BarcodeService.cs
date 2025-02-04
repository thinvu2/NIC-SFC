using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class BarcodeService
    {
        /// <summary>
        /// 检查是否存在BOX LABEL
        /// </summary>
        /// <param name="SSN">SHIPPING_SN</param>
        /// <returns>COUNT</returns>
        public static object CheckBOXLabel(string SSN)
        {
            object count;
            try
            {
                string sql = "select * from sfism4.r107 "
                          + "where shipping_sn = :SSN "
                          + "or serial_number=:SN "
                          + "and rownum = 1"; ;
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":SSN",SSN),
                    new OracleParameter(":SN",SSN)
                };
                count = DBHelper.ExecuteSclar(sql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return count;
        }

        /// <summary>
        /// 通过SHIPPING_SN获得SN
        /// </summary>
        /// <param name="SSN">SHIPPING_SN</param>
        /// <returns>SN</returns>
        public static DataTable GetR107(string SSN)
        {
            DataTable dt;
            try
            {
                string sql = "select serial_number,model_name,line_name,section_name,wip_group from sfism4.r107 "
                          + "where shipping_sn = :SSN "
                          + "or serial_number=:SN "
                          + "and rownum = 1";
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":SSN",SSN),
                    new OracleParameter(":SN",SSN)
                };
                dt = DBHelper.GetDataTable(sql, param);
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

        /// <summary>
        /// 检查R108的SN和KEYPARTSN
        /// </summary>
        /// <param name="serialNumber">SN</param>
        /// <param name="keyPartSn">KEY_PART_SN</param>
        /// <returns>COUNT</returns>
        public static object CheckFilterLabel(string serialNumber, string keyPartSn, int kpSnLength = 0)
        {
            object count;
            try
            {
                string sql = "select count(serial_number) from sfism4.r108 "
                          + "where serial_number = :serialNumber "
                          + "and key_part_sn = :keyPartSn";
                sql += kpSnLength == 0 ? "" : " and length(key_part_sn)=12";
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":serialNumber",serialNumber),
                    new OracleParameter(":keyPartSn",keyPartSn)
                };
                count = DBHelper.ExecuteSclar(sql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return count;
        }

        /// <summary>
        /// 检查sfism4.R_NOKIA_TEST_DATA_T的SN和BT LABEL
        /// </summary>
        /// <param name="serialNumber">SN</param>
        /// <param name="data1">BT LABEL</param>
        /// <returns>COUNT</returns>
        public static object CheckBTLabel(string serialNumber, string data1)
        {
            object count;
            try
            {
                string sql = "select count(serial_number) from sfism4.R_NOKIA_TEST_DATA_T "
                          + "where serial_number = :serialNumber "
                          + "and (DATA1 = :data1 or DATA10=:data1 or DATA2=:data1)";
                OracleParameter[] param = new OracleParameter[]
                {
                    new OracleParameter(":serialNumber",serialNumber),
                    new OracleParameter(":data1",data1)
                };
                count = DBHelper.ExecuteSclar(sql, param);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return count;
        }

        /// <summary>
        /// 檢查機種是否有FILTER LABEL
        /// </summary>
        /// <param name="model">機種名稱</param>
        /// <returns></returns>
        public static bool CheckFilterLabel(string model)
        {
            bool res;
            try
            {
                string sql = "SELECT COUNT(1) FROM SFIS1.C_MODEL_FILTER_SPECIAL_T "
                            + " WHERE MODEL_NAME=:modelName AND FILTER_FLAG='Y'";
                OracleParameter param = new OracleParameter(":modelName", model);
                res = Convert.ToInt16(DBHelper.ExecuteSclar(sql, param)) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return res;
        }

        /// <summary>
        /// 檢查機種是否有Wifi LABEL
        /// </summary>
        /// <param name="model">機種名稱</param>
        /// <returns></returns>
        public static bool CheckWifiLabel(string model)
        {
            bool res;
            try
            {
                string sql = "SELECT COUNT(1) FROM SFIS1.C_MODEL_FILTER_SPECIAL_T "
                            + " WHERE MODEL_NAME=:modelName AND WIFI_LABEL='Y'";
                OracleParameter param = new OracleParameter(":modelName", model);
                res = Convert.ToInt16(DBHelper.ExecuteSclar(sql, param)) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return res;
        }

        /// <summary>
        /// 执行NEW_TEST_INPUT存储过程
        /// </summary>
        /// <param name="line">线体</param>
        /// <param name="section">TEST</param>
        /// <param name="station">当前站位</param>
        /// <param name="sn">产品SN</param>
        /// <param name="myGroup">当前站位</param>
        /// <param name="emp">员工工号</param>
        /// <returns>执行结果</returns>
        public static string RunNewTestInputPro(string line, string section, string station, string sn, string myGroup, string emp)
        {
            string result = null;
            try
            {
                #region 配置parameter参数
                OracleParameter[] param = new OracleParameter[]
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

                for (int i = 0; i < param.Length - 1; i++)
                {
                    param[i].Direction = ParameterDirection.Input;
                }
                param[7].Direction = ParameterDirection.Output;

                param[0].Value = line;
                param[1].Value = section;
                param[2].Value = myGroup;
                param[3].Value = station;
                param[4].Value = "N/A";
                param[5].Value = sn;
                param[6].Value = emp;
                #endregion
                result = DBHelper.RunProcedure("NEW_TEST_INPUT", param);
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

    }
}
