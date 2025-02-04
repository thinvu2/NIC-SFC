using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace GemaltoRoast
{
    /// <summary>
    /// 封装数据库访问操作
    /// </summary>
    public class DBHelper
    {
        //数据库连接字符串
        private static string connString = "Data Source=(DESCRIPTION=(ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST =10.220.96.200)(PORT =1521)))(CONNECT_DATA =(SERVICE_NAME =vnsfc)));Persist Security Info=True;User ID=SFIS1;Password=vnsfis2014#!;";
        private static OracleConnection conn = null;

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public static OracleConnection getConnection
        {
            get
            {
                if (conn == null)
                {
                    conn = new OracleConnection(connString);
                }
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Broken)
                {
                    conn.Close();
                    conn.Open();
                }
                return conn;
            }
        }
        /// <summary>
        /// 构造参数
        /// </summary>
        public static void BuildCommand(OracleCommand comm, params OracleParameter[] parameter)
        {
            if (parameter != null && parameter.Length > 0)
            {
                foreach (OracleParameter p in parameter)
                {
                    comm.Parameters.Add(p);
                }
            }
        }
        /// <summary>
        /// 用字典动态添加参数
        /// </summary>
        public static OracleParameter[] GetParameter(Dictionary<string, object> dic)
        {
            OracleParameter[] parameter = null;
            if (dic != null)
            {
                parameter = new OracleParameter[dic.Count];
                int i = 0;
                foreach (string key in dic.Keys)
                {
                    parameter[i] = new OracleParameter(key, dic[key]);
                    i++;
                }
            }
            return parameter;
        }
        /// <summary>
        /// 执行增、删、改的操作方法
        /// </summary>
        public static int ExecuteNonQuery(string sql, params OracleParameter[] parameter)
        {
            int result = 0;
            using (OracleCommand comm = new OracleCommand(sql, getConnection))
            {
                BuildCommand(comm, parameter);
                result = comm.ExecuteNonQuery();
                comm.Dispose();
            }
            return result;
        }

        /// <summary>
        /// 执行查询操作返回单个数据
        /// </summary>
        public static object ExecuteSclar(string sql, params OracleParameter[] parameter)
        {
            object result = null;
            using (OracleCommand comm = new OracleCommand(sql, getConnection))
            {
                BuildCommand(comm, parameter);
                result = comm.ExecuteScalar();
                comm.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 执行查询操作方法返回一行数据
        /// </summary>
        public static OracleDataReader ExecuteReader(string sql, params OracleParameter[] parameter)
        {
            OracleDataReader reader = null;
            using (OracleCommand comm = new OracleCommand(sql, getConnection))
            {
                BuildCommand(comm, parameter);
                reader = comm.ExecuteReader(CommandBehavior.CloseConnection);
                comm.Dispose();
            }
            return reader;
        }
        /// <summary>
        /// 执行查询操作方法返回一组数据
        /// </summary>
        public static DataTable GetDataTable(string sql, params OracleParameter[] parameter)
        {
            DataSet ds = new DataSet();
            using (OracleCommand comm = new OracleCommand(sql, getConnection))
            {
                BuildCommand(comm, parameter);
                OracleDataAdapter adapter = new OracleDataAdapter(comm);
                adapter.Fill(ds);
                comm.Dispose();
            }
            return ds.Tables[0];
        }
        /// <summary>
        /// 执行多条增、删、改SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>
        /// <returns>影响的行数</returns>
        public static int ExecTransaction(List<String> list)
        {
            int count = 0;
            using (OracleCommand cmd = new OracleCommand(null, getConnection))
            {
                OracleTransaction stran = getConnection.BeginTransaction();
                cmd.Transaction = stran;
                try
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        string sql = list[i];
                        if (sql.Trim().Length > 1)
                        {
                            cmd.CommandText = sql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    stran.Commit();
                }
                catch
                {
                    stran.Rollback();
                    return 0;
                }
                finally
                {
                    cmd.Dispose();
                }
            }
            return count;
        }
        /// <summary>
        /// 执行查询一行数据的存储过程
        /// </summary>
        /// <param name="proName">名称</param>
        /// <param name="parameter">参数</param>
        /// <returns>object</returns>
        public static object ExecScalarProcedure(string proName, params OracleParameter[] parameter)
        {
            object result = null;
            using (OracleCommand cmd = new OracleCommand(proName, getConnection))
            {
                BuildCommand(cmd, parameter);
                cmd.CommandType = CommandType.StoredProcedure;
                result = cmd.ExecuteScalar();
                cmd.Dispose();
            }
            return result;
        }
        /// <summary>
        /// 执行查询一行数据的存储过程
        /// </summary>
        /// <param name="proName">名称</param>
        /// <param name="parameter">参数</param>
        /// <returns>SqlDataReader</returns>
        public static OracleDataReader ExecReaderProcedure(string proName, params OracleParameter[] parameter)
        {
            OracleDataReader reader = null;
            using (OracleCommand cmd = new OracleCommand(proName, getConnection))
            {
                BuildCommand(cmd, parameter);
                cmd.CommandType = CommandType.StoredProcedure;
                reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Dispose();
            }
            return reader;
        }
        /// <summary>
        ///  执行存储过程，返回输出参数值
        /// </summary>
        /// <param name="proName">名称</param>
        /// <param name="parameter">参数</param>
        /// <returns>影响的行数</returns>
        public static string RunProcedure(string proName, params OracleParameter[] parameter)
        {
            string result = null;
            using (OracleCommand cmd = new OracleCommand(proName, getConnection))
            {
                BuildCommand(cmd, parameter);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                object obj = cmd.Parameters["RES"].Value; //object obj=command.Parameters["@Output_Value"].Value;
                if (!(Object.Equals(obj, null)) && !(Object.Equals(obj, System.DBNull.Value)))
                {
                    result = obj.ToString();
                }
                cmd.Dispose();
            }
            return result;
        }
    }
}
