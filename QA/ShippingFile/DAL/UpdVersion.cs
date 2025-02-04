using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class UpdVersion
    {
        /// <summary>
        /// 检查系统当前版本是否是最新版本
        /// </summary>
        /// <param name="prgName">系统名称</param>
        /// <param name="tempVersion">系统版本</param>
        /// <returns>是否需要更新版本</returns>
        public bool CheckVersion(string prgName, string tempVersion)
        {
            try
            {
                string newVersion = FindVersion(prgName);
                if (newVersion == null)
                {
                    int result = AddVersion(prgName, tempVersion);
                    if (result<=0)
                    {
                        return true;
                    }
                    newVersion = FindVersion(prgName);
                }

                if (tempVersion.CompareTo(newVersion) < 0)
                {
                    return true;
                }
                else if(tempVersion.CompareTo(newVersion) > 0)
                {
                    UpdateVersion(prgName, tempVersion);
                } 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return false;
        }
        
        /// <summary>
        /// 查找系统版本信息
        /// </summary>
        /// <param name="prgName">系统名称</param>
        /// <returns>系统最新版本</returns>
        private string FindVersion(string prgName)
        {
            string result = null;
            try
            {
                string sql = "select AP_VERSION from sfism4.ams_ap where AP_NAME=:AP_NAME";
                OracleParameter parameter = new OracleParameter(":AP_NAME", prgName);
                object ver = DBHelper.ExecuteSclar(sql, parameter);
                if (ver != null)
                {
                    result = ver.ToString();
                }
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
       
        /// <summary>
        /// 更新系统版本信息
        /// </summary>
        /// <param name="prgName">系统名称</param>
        /// <param name="tempVersion">系统版本</param>
        /// <returns>系统最新版本</returns>
        private int UpdateVersion(string prgName, string tempVersion)
        {
            int result = 0;
            try
            {
                string sql = "update sfism4.ams_ap  set AP_VERSION=:AP_VERSION where AP_NAME=:AP_NAME";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":AP_VERSION", tempVersion),
                    new OracleParameter(":AP_NAME", prgName)
                };
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, parameter));
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

        private int AddVersion(string prgName, string tempVersion)
        
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"insert into  sfism4.ams_ap (ap_name,ap_version,ap_path,ap_desc,
                ap_type,file_name,updae_time,ap_group)
                values('{0}','{1}','{0}','{0}','FILE','{0}',sysdate,'Online')", prgName, tempVersion);

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
