using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class UserService
    {
        public static string loginNO = null;

        public static string loginName = null;

        public static string userRank = "9";

        public static string CheckLogin(string emp,string LoginPwd)
        {
            string result = null;
            OracleDataReader reader = null;
            try
            {
                //string sql = "select emp_no,emp_name,emp_rank from SFIS1.C_EMP_DESC_T where emp_no=:emp and emp_bc = :emp_bc and sysdate<=to_date(quit_date)";
                string sql = "select emp_no,emp_name,emp_rank from SFIS1.C_EMP_DESC_T where emp_no= '"+emp+"' and emp_bc = '"+LoginPwd+"'";
                /* OracleParameter[] parameter = new OracleParameter[]
                     {
                         new OracleParameter(":EMP", emp),
                         new OracleParameter(":emp_bc", LoginPwd)
                     };*/
                reader = DBHelper.ExecuteReader(sql);
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        result = reader["emp_no"].ToString();
                        result += "-" + reader["emp_name"].ToString();
                        result += "-" + reader["emp_rank"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                reader.Close();
                DBHelper.getConnection.Close();
            }
            return result;
        }
        
        /// <summary>
        /// Check User is exist
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public static bool CheckUser(string emp)
        {
            OracleDataReader reader = null;
            try
            {
                string sql = "SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO=:EMP_NO";
                OracleParameter parameter = new OracleParameter(":EMP_NO", emp);
                reader = DBHelper.ExecuteReader(sql,parameter);
                if (reader.HasRows)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                reader.Close();
                DBHelper.getConnection.Close();
            }
            return true;
        }
        
        //僅僅輸入Emp Password查工號信息
        public static DataTable CheckUserByEmpBC(string InEmpPasswordBC)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @"SELECT EMP_NO,EMP_NAME,EMP_RANK,CLASS_NAME,STATION_NAME,EMAIL FROM SFIS1.C_EMP_DESC_T WHERE EMP_BC=:EmpBC AND ROWNUM=1 ";
                }
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":EmpBC", InEmpPasswordBC) 
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
        
        /// <summary>
        /// AddUser
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static int AddUser(string sEmpNo,string sEmpName, string sClassName, string sStationName, string sEmpPWD, string sEmpBC, string sEmail)
        {
            int result = 0;
            try
            {
                string sql = string.Format("INSERT INTO SFIS1.C_EMP_DESC_T VALUES ('{0}','{1}',0,'{2}','{3}',SYSDATE+1820,'{4}','{5}','{6}','memppwd')"
                    , sEmpNo, sEmpName, sClassName, sStationName, sEmpPWD, sEmpBC, sEmail);
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql,null));
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

        //Special Privilege of Reprint
        public static DataTable GetPrivilegeRePrint(string funName, string empno)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @"SELECT PRIVILEGE FROM SFIS1.C_PRIVILEGE  WHERE PRG_NAME='ShopFloor' AND FUN=:funName AND EMP=:empno ";
                }
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":funName", funName),
                     new OracleParameter(":empno", empno)
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

        //僅僅輸入Emp工號和站位信息
        public static DataTable CheckEmpGroupPrivilegeByEmpNoGroup(string InGroupName, string InEmpNo)
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = @" SELECT EMP_NO,GROUP_NAME  FROM SFIS1.C_EMP_2_GROUP_T WHERE EMP_NO=:EmpNo AND GROUP_NAME=:GroupName AND ROWNUM=1 ";
                }
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":GroupName", InGroupName), 
                     new OracleParameter(":EmpNo", InEmpNo)
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
       
        //僅僅輸入Emp工號和程序名稱和Fun信息
        public static DataTable CheckEmpPrivilegeByEmpNoAPNameFun(string InEmpNo,string InAPName,string InFunName )
        {
            DataTable dt = null;
            string sql = string.Empty;
            try
            {
                {
                    sql = string.Format(@" SELECT EMP,FUN,PRIVILEGE,PRG_NAME FROM SFIS1.C_PRIVILEGE WHERE EMP='{0}' AND PRG_NAME='{1}' AND FUN='{2}' AND ROWNUM=1 ", InEmpNo, InAPName, InFunName);
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
    }
}
