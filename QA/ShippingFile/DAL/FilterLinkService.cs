using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class FilterLinkService
    {
        public static string result = null;

        public static DataTable GetR107(string sn)
        {
            try
            {
                string sql = @"SELECT SERIAL_NUMBER,
                                      LINE_NAME,
                                      MODEL_NAME,
                                      MO_NUMBER,
                                      BOM_NO,
                                      WIP_GROUP 
                                 FROM SFISM4.R_WIP_TRACKING_T 
                                WHERE SHIPPING_SN= :sn OR SERIAL_NUMBER=:sn";
                OracleParameter parameter = new OracleParameter(":sn", sn);
                return DBHelper.GetDataTable(sql, parameter);
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return null;
        }

        public static string RunCheckRoute(string line, string wip, string sn)
        {
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,25),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,25),
                    new OracleParameter("DATA",OracleDbType.Varchar2,50),
                    new OracleParameter("RES",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[3].Direction = ParameterDirection.Output;

                parameter[0].Value = line;
                parameter[1].Value = wip;
                parameter[2].Value = sn;

                result = DBHelper.RunProcedure("SFIS1.CHECK_ROUTE", parameter);
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return result;
        }

        public static bool CheckBomKeypart(string bom, string keypart, string group)
        {
            try
            {
                string sql = @"SELECT COUNT(*) FROM SFIS1.C_BOM_KEYPART_T 
                                WHERE BOM_NO = :bom 
                                  AND KEY_PART_NO = :keypartno 
                                  AND GROUP_NAME = :groupname";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":bom",bom),
                    new OracleParameter(":keypartno",keypart),
                    new OracleParameter(":groupname",group)
                };
                int res = int.Parse(DBHelper.ExecuteSclar(sql, parameter).ToString());
                if (res > 0)
                {
                    return true;
                }     
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return false;
        }

        public static DataTable GetAssyModel(string model)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_ASSY_MODEL_T A,SFIS1.C_BOM_KEYPART_T B
                                WHERE A.MODEL_NAME = :MODEL_NAME 
                                  AND A.MODEL_NAME = B.BOM_NO 
                                  AND A.KP_NO = B.KEY_PART_NO  
                                  AND B.GROUP_NAME = 'ASSY6'";
                OracleParameter parameter = new OracleParameter(":MODEL_NAME", model);
                dt = DBHelper.GetDataTable(sql, parameter);
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dt;
        }

        public static bool CheckAssyModel(string modelName, string kp, string kpsn)
        {
            try
            {
                string sql = @"SELECT COUNT(1) FROM SFIS1.C_ASSY_MODEL_T 
                                WHERE MODEL_NAME = :modelName 
                                  AND KP_NO = :kpNo 
                                  AND KPS_LENGTH = LENGTH(:sn) 
                                  AND PREFIX = SUBSTR(:sn, 1, LENGTH(PREFIX))";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":modelName",modelName),
                    new OracleParameter(":kpNo",kp),
                    new OracleParameter(":sn",kpsn)
                };
                int res = int.Parse(DBHelper.ExecuteSclar(sql, parameter).ToString());
                if (res > 0)
                    return true;
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return false;
        }

        public static bool GetFilterQty(string modelName)
        {
            try
            {
                string sql = @"SELECT COUNT(1) FROM SFIS1.C_MODEL_FILTER_SPECIAL_T 
                                WHERE MODEL_NAME=:modelName AND FILTER_QTY='1'";
                OracleParameter parameter = new OracleParameter(":modelName", modelName);
                int res = int.Parse(DBHelper.ExecuteSclar(sql, parameter).ToString());
                if (res > 0)
                    return true;
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return false;
        }

        public static void RunNewStnRecPcs(string group, string mo, string sn, string line = "W217", string flag = "0")
        {
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,20),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,20),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,20),
                    new OracleParameter("W_STATION",OracleDbType.Varchar2,20),
                    new OracleParameter("MO",OracleDbType.Varchar2,20),
                    new OracleParameter("SN",OracleDbType.Varchar2,50),
                    new OracleParameter("F_FLAG",OracleDbType.Varchar2,20)
                };

                for (int i = 0; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }

                parameter[0].Value = line;
                parameter[1].Value = "TEST";
                parameter[2].Value = group;
                parameter[3].Value = group + "1";
                parameter[4].Value = mo;
                parameter[5].Value = sn;
                parameter[6].Value = flag;

                DBHelper.RunProcedure("SFIS1.NEW_STN_REC_PCS", parameter);
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
        }

        public static void RunNewUpdateR107(string group, string mo, string sn, string emp, string line = "W217", string flag = "0", string station = "ShopFloor")
        {
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,20),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,20),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,20),
                    new OracleParameter("W_STATION",OracleDbType.Varchar2,20),
                    new OracleParameter("MO",OracleDbType.Varchar2,20),
                    new OracleParameter("SN",OracleDbType.Varchar2,50),
                    new OracleParameter("F_FLAG",OracleDbType.Varchar2,20),
                    new OracleParameter("EMP",OracleDbType.Varchar2,30),
                    new OracleParameter("STATION_PCNAME",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }

                parameter[0].Value = line;
                parameter[1].Value = "TEST";
                parameter[2].Value = group;
                parameter[3].Value = group + "1";
                parameter[4].Value = mo;
                parameter[5].Value = sn;
                parameter[6].Value = flag;
                parameter[7].Value = emp;
                parameter[8].Value = station;

                DBHelper.RunProcedure("SFIS1.NEW_UPDATE_R107", parameter);
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
        }

        public static bool CheckR108(string kpSn)
        {
            try
            {
                string sql = @"SELECT COUNT(1) FROM SFISM4.R108 WHERE KEY_PART_SN=:kpsn";
                OracleParameter parameter = new OracleParameter(":kpsn", kpSn);
                if (int.Parse(DBHelper.ExecuteSclar(sql, parameter).ToString()) > 0)
                {
                    return false;
                }
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return true;
        }

        public static int InsertR108(string emp, string sn, string kpNo, string kpSn, string wip, string mo)
        {
            try
            {
                string sql = @"INSERT INTO SFISM4.R108 
                               SELECT :emp,:sn,:kpNo,:kpSn,'',:wip,'',SYSDATE,'','','',:mo FROM DUAL 
                                WHERE NOT EXISTS (SELECT * FROM SFISM4.R108 WHERE KEY_PART_SN IN(:kpSn))";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":emp",emp),
                    new OracleParameter(":sn",sn),
                    new OracleParameter(":kpNo",kpNo),
                    new OracleParameter(":kpSn",kpSn),
                    new OracleParameter(":wip",wip),
                    new OracleParameter(":mo",mo)
                };

                return DBHelper.ExecuteNonQuery(sql, parameter);
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return 0;
        }
    }
}
