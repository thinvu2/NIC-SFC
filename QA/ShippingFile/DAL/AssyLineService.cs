using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class AssyLineService
    {
        public static DataTable dt = null;
        public static string result = null;

        /// <summary>
        /// 获取所有的线体名称
        /// </summary>
        /// <returns>线体名称</returns>
        public static DataTable GetAllLine()
        {
            try
            {
                string sql = "SELECT DISTINCT LINE_NAME FROM SFIS1.C_LINE_DESC_T ORDER BY 1";
                dt = DBHelper.GetDataTable(sql, null);
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
        /// 获取所有ASSY_LINE站
        /// </summary>
        /// <returns>dt</returns>
        public static DataTable GetAssyStation()
        {
            try
            {
                string sql = "SELECT DISTINCT SECTION_NAME,GROUP_NAME,STATION_NAME "
                            +"FROM SFIS1.C_STATION_CONFIG_T "
                            +"WHERE SECTION_NAME='TEST' "
                            +"AND GROUP_NAME LIKE 'ASSY%' "
                            +"ORDER BY GROUP_NAME,STATION_NAME";
                dt = DBHelper.GetDataTable(sql, null);
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
        /// 执行NEW_ASSY_SNINPUT存储过程
        /// </summary>
        /// <param name="line">线名</param>
        /// <param name="group">当前站位</param>
        /// <param name="station">站名</param>
        /// <param name="data">sn</param>
        /// <returns></returns>
        public static string RunNewAssySnInputPro(string line,string group,string station,string data)
        {
            try
            {
                #region 配置parameter参数
                OracleParameter[] paras = new OracleParameter[]
                {
                    new OracleParameter("STATION_NUM",OracleDbType.Varchar2,25),
                    new OracleParameter("LINE",OracleDbType.Varchar2,25),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,25),
                    new OracleParameter("W_STATION",OracleDbType.Varchar2,60),
                    new OracleParameter("DATA",OracleDbType.Varchar2,25),
                    new OracleParameter("RES",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < paras.Length - 1; i++)
                {
                    paras[i].Direction = ParameterDirection.Input;
                }
                paras[6].Direction = ParameterDirection.Output;

                paras[0].Value = "";
                paras[1].Value = line;
                paras[2].Value = "TEST";
                paras[3].Value = group;
                paras[4].Value = station;
                paras[5].Value = data;
                #endregion

                result = DBHelper.RunProcedure("SFIS1.NEW_ASSY_SNINPUT", paras);
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

        /// <summary>
        /// 执行SFIS1.NEW_ASSY_CHECK_KEYPART存储过程
        /// </summary>
        /// <param name="line">线名</param>
        /// <param name="group">当前站位</param>
        /// <param name="station">站名</param>
        /// <param name="kpSn">key_part_sn</param>
        /// <param name="emp">工号</param>
        /// <param name="sn">sn</param>
        /// <param name="mo">工单</param>
        /// <param name="model">机种</param>
        /// <param name="bom">bom_no</param>
        /// <returns></returns>
        public static string RunNewAssyCheckKeypartPro(string line,string group,string station,string kpSn,string emp,string sn,string mo,string model,string bom)
        {
            try
            {
                #region 配置parameter参数
                OracleParameter[] paras = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,25),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,25),
                    new OracleParameter("W_STATION",OracleDbType.Varchar2,25),
                    new OracleParameter("KPN",OracleDbType.Varchar2,25),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,25),
                    new OracleParameter("SN",OracleDbType.Varchar2,25),
                    new OracleParameter("MO",OracleDbType.Varchar2,25),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("BOMNO",OracleDbType.Varchar2,25),
                    new OracleParameter("RES",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < paras.Length - 1; i++)
                {
                    paras[i].Direction = ParameterDirection.Input;
                }
                paras[10].Direction = ParameterDirection.Output;

                paras[0].Value = line;
                paras[1].Value = "TEST";
                paras[2].Value = group;
                paras[3].Value = station;
                paras[4].Value = kpSn;
                paras[5].Value = emp;
                paras[6].Value = sn;
                paras[7].Value = mo;
                paras[8].Value = model;
                paras[9].Value = bom;
                #endregion
                result = DBHelper.RunProcedure("SFIS1.NEW_ASSY_CHECK_KEYPART", paras);
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

        /// <summary>
        /// 通过SN获得R107数据
        /// </summary>
        /// <param name="sn">sn</param>
        /// <returns></returns>
        public static DataTable GetR107BySN(string sn)
        {
            try
            {
                string sql = "SELECT MO_NUMBER,MODEL_NAME,BOM_NO FROM SFISM4.R107 "
                          + "WHERE SERIAL_NUMBER=:sn";
                OracleParameter para = new OracleParameter(":sn", sn);
                dt = DBHelper.GetDataTable(sql, para);
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
        /// 通过KEYPARTSN获得KEYPARTNO
        /// </summary>
        /// <param name="kpSn">key_part_sn</param>
        /// <returns>key_part_no</returns>
        public static string GetKeyPartNo(string kpSn)
        {
            try
            {
                result = null;
                string sql = "SELECT KEY_PART_NO FROM SFISM4.R107 "
                          + "WHERE SERIAL_NUMBER=:sn";
                OracleParameter para = new OracleParameter(":sn",kpSn);
                result = DBHelper.ExecuteSclar(sql, para).ToString();
            }
            catch 
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            if (string.IsNullOrEmpty(result))
            {
                result = "KeyPartNo Not Find";
            }
            return result;
        }
    }
}
