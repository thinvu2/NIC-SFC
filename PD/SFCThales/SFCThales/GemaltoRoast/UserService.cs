using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Sfc.Library.HttpClient;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Newtonsoft.Json;

namespace GemaltoRoast
{
    public class UserService
    {
        //public static string loginNO = null;

        //public static string loginName = null;

        //public static string userRank = null;

        public async Task <string> CheckLogin(string emp,string LoginPwd, SfcHttpClient _sfcHttpClient)
        {
            string result = null;
            DataTable reader = null;
            try
            {
               // string sql = "select emp_no,emp_name,emp_rank from SFIS1.C_EMP_DESC_T where emp_no=:emp and emp_bc = :emp_bc and sysdate<=to_date(quit_date)";
                string sql =string.Format( @"SELECT A.EMP_NO, A.EMP_NAME,A.EMP_RANK 
                                             FROM SFIS1.C_EMP_DESC_T A,SFIS1.C_PRIVILEGE B 
                                            WHERE A.EMP_NO=B.EMP  
                                            AND A.EMP_NO='{0}' 
                                            AND A.EMP_BC='{1}'
                                            AND upper(B.FUN) IN('ADMIN','LOGIN')  AND B.PRG_NAME='GemaltoRoast' 
                                            AND SYSDATE <= A.QUIT_DATE" , emp, LoginPwd);

                
                reader = await ExcuteSelectSQL(sql, _sfcHttpClient);
                result = reader.Rows[0]["emp_no"].ToString();
                        result += "-" + reader.Rows[0]["emp_name"].ToString();
                        result += "-" + reader.Rows[0]["emp_rank"].ToString();
                    

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

        public async Task<string> CheckPrivilege(string emp, SfcHttpClient _sfcHttpClient)
        {
            string result = null;
            DataTable reader = null;
            try
            {
               string sql =string.Format( @"SELECT A.EMP_NO, A.EMP_NAME,A.EMP_RANK
                                            FROM SFIS1.C_EMP_DESC_T A,SFIS1.C_PRIVILEGE B 
                                            WHERE A.EMP_NO=B.EMP
                                            AND A.EMP_NO ='{0}'
                                            AND B.FUN IN ('Admin','VI')
                                            AND B.PRG_NAME='GemaltoRoast' ", emp);

                
                reader = await ExcuteSelectSQL(sql, _sfcHttpClient);
               
                result = reader.Rows[0]["emp_no"].ToString();
                result += "-" + reader.Rows[0]["emp_name"].ToString();
                result += "-" + reader.Rows[0]["emp_rank"].ToString();
                    

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
        /// Check User is exist
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public async Task<bool> CheckUser(string emp, SfcHttpClient _sfcHttpClient)
        {
            DataTable reader = null;
            try
            {
                string sql =string.Format(@"SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO=:EMP_NO", emp);
                reader = await ExcuteSelectSQL(sql, _sfcHttpClient);
                if (reader.Rows.Count == 0)
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
            }
            return true;
        }
        /// <summary>
        /// AddUser
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public async Task< int> AddUser(string emp,string name,string pwd, SfcHttpClient _sfcHttpClient)
        {
            int result = 0;
            DataTable dt = null;
            try
            {
                string sql = string.Format("INSERT INTO SFIS1.C_EMP_DESC_T VALUES ('{0}',{1},0,'','','',SYSDATE,'{2}','{3}','','memppwd')"
                    , emp, name, pwd, pwd);
                dt = await ExcuteSelectSQL(sql, _sfcHttpClient);
                result = dt.Rows.Count;
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
        public async Task<DataTable> ExcuteSP(string procename, List<SfcParameter> ListPara, SfcHttpClient sfcHttpClient)
        {
            DataTable data = new DataTable();
            try
            {
                var result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = procename,
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = ListPara
                });
                dynamic ads = result.Data;
                var vardatatabel = JsonConvert.SerializeObject(result.Data);
                data = JsonConvert.DeserializeObject<DataTable>(vardatatabel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return data;
        }
    }
}
