using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class SnReleaseService
    {
        /// <summary>
        /// 根據工單號從工單表查看工單是否有上線
        /// </summary>
        /// <param name="mo">工單</param>
        public static DataTable GetMoBaseByMo(string mo)
        {
            try
            {
                string sql = "SELECT * FROM SFISM4.R_MO_BASE_T "
                            + "WHERE MO_NUMBER=:mo";
                OracleParameter parameter = new OracleParameter(":mo", mo);
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

        /// <summary>
        /// 根據工單獲得該筆工單的機種和套數
        /// </summary>
        /// <param name="mo">工單</param>
        public static DataTable GetMoPlanByMo(string mo)
        {
            try
            {
                string sql = "SELECT MODEL_NAME,TARGET_QTY FROM SFISM4.R_BPCS_MOPLAN_T "
                        + "WHERE MO_NUMBER=:mo";
                OracleParameter parameter = new OracleParameter(":mo", mo);
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

        /// <summary>
        /// 工單RELEASE信息
        /// </summary>
        /// <param name="mo">工單</param>
        public static DataTable MoRelease(string mo)
        {
            try
            {
                string sql = "SELECT * FROM SFISM4.R_ERICSSON_CSN_T WHERE MO_NUMBER=:mo";
                OracleParameter paremeter = new OracleParameter(":mo", mo);
                return DBHelper.GetDataTable(sql, paremeter);
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

        /// <summary>
        /// 根據機種獲取該機種SN規則
        /// </summary>
        /// <param name="model">機種名稱</param>
        public static DataTable GetSnRuleByModel(string model)
        {
            try
            {
                string sql = "SELECT * FROM SFIS1.C_SNMAC_RULE_T WHERE MODEL_NAME=:model";
                OracleParameter parameter = new OracleParameter(":model", model);
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

        public static string GetDateCode()
        {
            try
            {
                string sql = "select TO_CHAR(sysdate,'YYYYMMDD') from dual";
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

        public static string GetDateCodeEricssion()
        {
            try
            {
                string sql = "select TO_CHAR(sysdate - 7/24,'YYYYMMDD') from dual";
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

        /// <summary>
        /// 從ERICSSONCSN表獲取最大的SN
        /// </summary>
        /// <returns>MAX SN</returns>
        public static string GetMaxSnFromEricssonCsn()
        {
            try
            {
                string sql = "SELECT MAX(SERIAL_NUMBER) FROM SFISM4.R_ERICSSON_CSN_T ";
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

        /// <summary>
        /// 獲取一個工單的最大SN
        /// </summary>
        /// <param name="mo">工單號</param>
        /// <returns>此工單最大的SN</returns>
        public static string GetMaxSnFromEricssonCsn(string mo)
        {
            try
            {
                string sql = "SELECT MAX(SERIAL_NUMBER) FROM SFISM4.R_ERICSSON_CSN_T "
                        + "WHERE MO_NUMBER=:mo";
                OracleParameter parameter = new OracleParameter(":mo", mo);
                return DBHelper.ExecuteSclar(sql, parameter).ToString();
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

        /// <summary>
        /// 從ERICSSON表獲取最小的SN和最大的SN
        /// </summary>
        /// <param name="mo">工單</param>
        /// <returns>區間</returns>
        public static DataTable GetSnRangeByMo(string mo)
        {
            try
            {
                string sql = "SELECT MIN(SERIAL_NUMBER) MIN,MAX(SERIAL_NUMBER) MAX FROM SFISM4.R_ERICSSON_CSN_T "
                            + "WHERE MO_NUMBER=:mo ";
                OracleParameter parameter = new OracleParameter(":mo", mo);
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

        /// <summary>
        /// RELEASE工單,就是把SN插入到R_ERICSSON_CSN_T
        /// </summary>
        /// <param name="mo">工單</param>
        /// <param name="sn">SN</param>
        /// <param name="ver">版本</param>
        /// <param name="emp">工號</param>
        /// <param name="model">機種</param>
        public static void InsertEricssonCsn(string mo, string sn, string ver, string emp, string model)
        {
            try
            {
                string sql = "INSERT INTO SFISM4.R_ERICSSON_CSN_T "
                     + "VALUES(:mo,:sn,SYSDATE,SYSDATE,:ver,'N',:emp,:model)";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":mo",mo),
                    new OracleParameter(":sn",sn),
                    new OracleParameter(":ver",ver),
                    new OracleParameter(":emp",emp),
                    new OracleParameter(":model",model)
                };
                DBHelper.ExecuteNonQuery(sql, parameter);
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

        /// <summary>
        /// 打印成功向R_M2B_B_RECORD_T插入一筆數據
        /// </summary>
        /// <param name="sn">SN</param>
        /// <param name="model">機種</param>
        /// <param name="ver">版本</param>
        public static void InsertM2bBRecord(string sn, string model, string ver)
        {
            try
            {
                string sql = "INSERT INTO SFISM4.R_M2T_B_RECORD "
                    + "VALUES('B',:sn,'DU5',:model,:ver,SYSDATE,'DU5','Y','SFIS','DU5','CN','','N',SYSDATE,'')";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":sn",sn),
                    new OracleParameter(":model",model),
                    new OracleParameter(":ver",ver)
                };
                DBHelper.ExecuteNonQuery(sql, parameter);
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

        /// <summary>
        /// 打印成功向R_M2B_C_RECORD_T插入一筆數據
        /// </summary>
        /// <param name="sn">SN</param>
        /// <param name="model">機種</param>
        /// <param name="ver">版本</param>
        public static void InsertM2bCRecord(string sn, string model, string ver)
        {
            try
            {
                string sql = "INSERT INTO SFISM4.R_M2T_C_RECORD "
                        + "VALUES('C',:sn,'DU5',:model,:ver,'DU5','Y','DU5','SFIS','DU5','CN','N',SYSDATE,'')";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":sn",sn),
                    new OracleParameter(":model",model),
                    new OracleParameter(":ver",ver)
                };
                DBHelper.ExecuteNonQuery(sql, parameter);
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

        /// <summary>
        /// 把工單區間插入到區間表里
        /// </summary>
        /// <param name="mo">工單號</param>
        /// <param name="minSn">最小的SN</param>
        /// <param name="ver">版本</param>
        /// <param name="maxSn">最大的SN</param>
        /// <param name="time">插入的時間</param>
        /// <param name="model">幾種</param>
        /// <param name="length">SN的長度</param>
        public static void InsertMoExt(string mo, string minSn, string ver, string maxSn, string time, string model, string length)
        {
            try
            {
                string sql = "INSERT INTO SFISM4.R_MO_EXT_T "
                    + "VALUES(:mo,:min,:ver,:max,:ver,:time,:time,'ShopFloor',:model,'0',:model,'',:length)";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":mo",mo),
                    new OracleParameter(":min",minSn),
                    new OracleParameter(":ver",ver),
                    new OracleParameter(":max",maxSn),
                    new OracleParameter(":time",time),
                    new OracleParameter(":model",model),
                    new OracleParameter(":length",length)
                };
                DBHelper.ExecuteNonQuery(sql, parameter);
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

        /// <summary>
        /// 從R_ERICSSON_CSN_T獲取已經列印的數量
        /// </summary>
        /// <param name="mo">工單</param>
        /// <returns>已經列印的數量</returns>
        public static string GetPrintQtyByMo(string mo)
        {
            try
            {
                string sql = "SELECT count(1) FROM SFISM4.R_ERICSSON_CSN_T "
                            + "WHERE MO_NUMBER=:mo AND PRINT_FLAG='Y'";
                OracleParameter parameter = new OracleParameter(":mo", mo);
                return DBHelper.ExecuteSclar(sql, parameter).ToString();
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

        /// <summary>
        /// 判斷區間是否重複
        /// </summary>
        /// <param name="sn">sn</param>
        public static DataTable SnRangeRepeat(string sn)
        {
            try
            {
                string sql = "SELECT * FROM SFISM4.R_MO_EXT_T "
                        + "WHERE :sn BETWEEN ITEM_1 AND ITEM_2";
                OracleParameter parameter = new OracleParameter(":sn", sn);
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
    }
}
