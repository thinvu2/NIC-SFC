using System;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;

namespace GemaltoRoast
{
    public class UpdVersion
    {
        /// <summary>
        /// 检查系统当前版本是否是最新版本
        /// </summary>
        /// <param name="prgName">系统名称</param>
        /// <param name="tempVersion">系统版本</param>
        /// <returns>是否需要更新版本</returns>
        public async Task<bool> CheckVersion(string prgName, string tempVersion,SfcHttpClient sfcHttpClient)
        {
            try
            {
                string newVersion =await FindVersion(prgName, sfcHttpClient);
                if (newVersion == null)
                {
                    int result = await AddVersion(prgName, tempVersion, sfcHttpClient);
                    if (result<=0)
                    {
                        return true;
                    }
                    newVersion =await FindVersion(prgName, sfcHttpClient);
                }

                if (tempVersion.CompareTo(newVersion) < 0)
                {
                    return true;
                }
                else if(tempVersion.CompareTo(newVersion) > 0)
                {
                   await UpdateVersion(prgName, tempVersion, sfcHttpClient);
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
        private async Task< string> FindVersion(string prgName, SfcHttpClient sfcHttpClient)
        {
            string result = null;
            DataTable dt = null;
            try
            {
                string sql =string.Format(@"select AP_VERSION from sfism4.ams_ap where AP_NAME='{0}'", prgName);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
                if(dt.Rows.Count > 0)
                {
                    result = dt.Rows[0]["AP_VERSION"].ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return result;
        }
        /// <summary>
        /// 更新系统版本信息
        /// </summary>
        /// <param name="prgName">系统名称</param>
        /// <param name="tempVersion">系统版本</param>
        /// <returns>系统最新版本</returns>
        private async Task<int> UpdateVersion(string prgName, string tempVersion, SfcHttpClient sfcHttpClient)
        {
            int r = 0;
            try
            {
                string sql =string.Format(@"update sfism4.ams_ap  set AP_VERSION='{0}' where AP_NAME='{1}'", tempVersion,prgName);
             
                await ExcuteNonQuerySQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                r = 1;
            }
            finally
            {
                r = 0;
            }
            return r;
        }

        private async Task<int> AddVersion(string prgName, string tempVersion, SfcHttpClient sfcHttpClient)
        
        {
            int r = 0;
            try
            {
                string sql = string.Format(@"insert into  sfism4.ams_ap (ap_name,ap_version,ap_path,ap_desc,
                ap_type,file_name,updae_time,ap_group)
                values('{0}','{1}','{0}','{0}','FILE','{0}',sysdate,'Online')", prgName, tempVersion);

                await ExcuteNonQuerySQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
                r =  1;
            }
            finally
            {
                r =  0;
            }
            return r;
            
        }
        public async Task<bool> ExcuteNonQuerySQL(string sql, SfcHttpClient sfcHttpClient)
        {
            try
            {
                await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel { CommandText = sql, SfcCommandType = SfcCommandType.Text });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return true;
        }
        public async Task<DataTable> ExcuteSelectSQL(string sql, SfcHttpClient sfcHttpClient)
        {
            DataTable data;
            data = null;
            try
            {
                var datacust = await sfcHttpClient.QueryListAsync(new QuerySingleParameterModel
                {
                    CommandText = sql,
                    SfcCommandType = SfcCommandType.Text
                });


                if (datacust.Data != null)
                {
                    var vardatatabel = JsonConvert.SerializeObject(datacust.Data);
                    data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return data;
        }
    }
}
