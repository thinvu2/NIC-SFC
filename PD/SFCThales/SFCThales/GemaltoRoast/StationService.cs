using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using Newtonsoft.Json;

namespace GemaltoRoast
{
    public class StationService
    {
        /// <summary>
        /// Get Station
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task< DataTable > GetAllStation(SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT DISTINCT LINE_NAME FROM SFIS1.C_STATION_CONFIG_T ORDER BY LINE_NAME";
                dt =await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task< DataTable > GetGetLineStation(string LineName, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
               /* string sql = @"SELECT  LINE_NAME,SECTION_NAME,GROUP_NAME,STATION_NAME 
                                          FROM SFIS1.C_STATION_CONFIG_T WHERE LINE_NAME=:LINE_NAME
                                         ORDER BY GROUP_NAME ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":LINE_NAME", LineName)
                };
                dt = DBHelper.GetDataTable(sql, parameter);*/
                string sql = string.Format(@"select 'SI' as SECTION_NAME , group_name, group_name as station_name from                               
                                            (SELECT 'VI' AS GROUP_NAME FROM DUAL UNION 
                                            SELECT 'ROAST_OUT' AS GROUP_NAME FROM DUAL UNION 
                                            SELECT 'PACK_TRAY' AS GROUP_NAME FROM DUAL UNION
                                            SELECT 'PACK_TRAYII' AS GROUP_NAME FROM DUAL UNION  
                                            SELECT 'DUST_BLOWING' AS GROUP_NAME FROM DUAL UNION 
                                            SELECT 'ROAST_IN' AS GROUP_NAME FROM DUAL UNION 
                                            SELECT 'TAPPING' AS GROUP_NAME FROM DUAL UNION 
                                            SELECT 'VI1' AS GROUP_NAME FROM DUAL)");

                /* string.Format("SELECT  LINE_NAME,SECTION_NAME,GROUP_NAME,STATION_NAME "
                           + " FROM SFIS1.C_STATION_CONFIG_T WHERE LINE_NAME='{0}' "
                           + " ORDER BY GROUP_NAME",LineName );*/

                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task<DataTable> GetTrayNo(string TrayNo, string GroupName,SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {   string sql = string.Format("SELECT A.SERIAL_NUMBER,A.MO_NUMBER,A.MODEL_NAME,A.VERSION_CODE,A.TRAY_NO,A.LINE_NAME,A.GROUP_NAME,"
                             + " A.NEXT_STATION, A.WIP_GROUP,B.GROUP_NEXT "
                             + " FROM (SELECT * FROM SFISM4.R_WIP_TRACKING_T  WHERE TRAY_NO ='{0}' "
                             + " UNION SELECT * FROM SFISM4.H_WIP_TRACKING_T  WHERE TRAY_NO ='{0}') A,"
                             + " SFIS1.C_ROUTE_CONTROL_T B WHERE A.SPECIAL_ROUTE = B.ROUTE_CODE "
                             + " AND A.GROUP_NAME = B.GROUP_NAME AND A.ERROR_FLAG = B.STATE_FLAG "
                             + " AND B.GROUP_NEXT ='{1}'", TrayNo, GroupName);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt;
        }

        public async Task<DataTable> GetTrayNo1(string TrayNo, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format("SELECT SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,VERSION_CODE,TRAY_NO,LINE_NAME,GROUP_NAME,"
                             + " NEXT_STATION, WIP_GROUP "
                             + " FROM SFISM4.R_WIP_TRACKING_T  WHERE TRAY_NO ='{0}' AND GROUP_NAME_CQC IS NULL ", TrayNo);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> getR107 (string SSN, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format("SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE shipping_sn='{0}' ", SSN);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {

            }
            return dt;
        }

        public async Task<DataTable> SelectTraySn(string TrayNo, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
               /* string sql = @"select * from sfism4.r_wip_tracking_t where TRAY_NO=:TRAY_NO and rownum=1";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":TRAY_NO", tray_no)
                };
                dt = DBHelper.GetDataTable(sql, parameter);*/
                string sql = string.Format("SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='{0}' AND ROWNUM=1", TrayNo);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> SelectTraySn1(string TrayNo, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                /* string sql = @"select * from sfism4.r_wip_tracking_t where TRAY_NO=:TRAY_NO and rownum=1";
                 OracleParameter[] parameter = new OracleParameter[]
                 {
                      new OracleParameter(":TRAY_NO", tray_no)
                 };
                 dt = DBHelper.GetDataTable(sql, parameter);*/
                string sql = string.Format("SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='{0}' ", TrayNo);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<DataTable> SelectQTYTray(string model,string ver, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                /* string sql = @"select * from sfism4.r_wip_tracking_t where TRAY_NO=:TRAY_NO and rownum=1";
                 OracleParameter[] parameter = new OracleParameter[]
                 {
                      new OracleParameter(":TRAY_NO", tray_no)
                 };
                 dt = DBHelper.GetDataTable(sql, parameter);*/
                string sql = string.Format("SELECT VR_CLASS AS MODEL_NAME,VR_VALUE AS TRAY_QTY FROM SFIS1.C_PARAMETER_INI WHERE PRG_NAME = 'PACKTRAYQTY' AND VR_CLASS = '{0}' AND VR_ITEM = 'OFFSET'", model, ver);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
                if (dt.Rows.Count >0 )
                {
                    return dt;
                }
                sql = string.Format("SELECT * FROM SFIS1.C_PACK_PARAM_T WHERE model_name='{0}' AND version_code = '{1}' and rownum=1", model,ver);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }


        public async Task<DataTable> SelectSN(string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                /*string sql = @"select SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,VERSION_CODE,LINE_NAME,GROUP_NAME,NEXT_STATION,WIP_GROUP,ERROR_FLAG,SCRAP_FLAG,TRAY_NO  
                 from sfism4.r_wip_tracking_t where SERIAL_NUMBER=:SN OR SHIPPING_SN=:SN ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn)
                };
                dt = DBHelper.GetDataTable(sql, parameter);*/
                string sql = string.Format(" SELECT SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,VERSION_CODE,LINE_NAME,GROUP_NAME,NEXT_STATION,"
                + " WIP_GROUP,ERROR_FLAG,SCRAP_FLAG,TRAY_NO FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER='{0}' OR SHIPPING_SN='{0}'", sn);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                
            }
            return dt;
        }

        public async Task<int> UpdateSN(string SN, string TrayNo, SfcHttpClient sfcHttpClient)
        {
            int result = 0;
            DataTable dt;
            try
            {
                /*string sql = "Update sfism4.r_wip_tracking_t set TRAY_NO=:TRAY_NO  where SERIAL_NUMBER=:SN OR SHIPPING_SN=:SN ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn),
                     new OracleParameter(":TRAY_NO", Tray_No)
                };
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, parameter));*/
                string sql = string.Format(" UPDATE SFISM4.R_WIP_TRACKING_T  SET TRAY_NO='{1}' WHERE SERIAL_NUMBER='{0}' OR SHIPPING_SN='{0}'", SN, TrayNo);
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
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

        public async Task<int> UpdateSN1(string SN, SfcHttpClient sfcHttpClient)
        {
            int result = 0;
            DataTable dt;
            try
            {
                /*string sql = "Update sfism4.r_wip_tracking_t set TRAY_NO=:TRAY_NO  where SERIAL_NUMBER=:SN OR SHIPPING_SN=:SN ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":SN", sn),
                     new OracleParameter(":TRAY_NO", Tray_No)
                };
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, parameter));*/
                string sql = string.Format(" UPDATE SFISM4.R_WIP_TRACKING_T  SET GROUP_NAME_CQC='{1}' WHERE SERIAL_NUMBER='{0}' OR SHIPPING_SN='{0}'", SN,"Y");
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
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

        /**
         * add by champion 2016.06.24
         * 檢查電性是否過期:FT站到PACK_TRAY站的時間間隔
         * true:電性過期
         * false:電性沒過期
         */
        public async Task<bool> checkElectricalExpired(string sn, SfcHttpClient sfcHttpClient)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string max_ft_time = string.Empty;

            sql = "select * from SFIS1.C_MODEL_ATTR_CONFIG_T where ATTRIBUTE_NAME='NO_CHECK_FT' and type_value=(select model_name from sfism4.r107 where serial_number='"+sn+ "')";
            dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            if (dt.Rows.Count > 0)
            {
                return false;
            }

            sql = @"select * from (
                    select * from sfism4.r_sn_detail_t where serial_number = '" + sn + @"'  and group_name = 'FT' 
                    union
                    select * from sfism4.h_sn_detail_t where serial_number = '" + sn + @"'  and group_name = 'FT') ";
            dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            if (dt.Rows.Count == 0)
            {
                return true;
            }
            sql = @"select to_char(max(in_station_time),'YYYYMMDD HH24:MI:SS') time from (
                    select * from sfism4.r_sn_detail_t where serial_number = '" + sn + @"' and group_name = 'FT'
                    union
                    select * from sfism4.h_sn_detail_t where serial_number = '" + sn + @"' and group_name = 'FT') ";
            dt = await ExcuteSelectSQL(sql, sfcHttpClient);
            if (dt.Rows.Count > 0)
            {
                sql = "select * from dual where round(sysdate - to_date('" + dt.Rows[0]["time"].ToString() + @"','YYYYMMDD HH24:MI:SS'),2)>180";
                dt = await ExcuteSelectSQL(sql, sfcHttpClient);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public async Task<string> CheckRouteProcedure(string line, string myGroup, string sn, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {

                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "line", Value = line, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "mygroup", Value = myGroup, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "data", Value = sn, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                            };

                dt = await ExcuteSP("sfis1.NEW_CHECK_ROUTE", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt.Rows[0]["res"].ToString();
        }

        public async Task<string> CheckBakeProcedure(string bakeNo, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {

                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "bakeno", Value = bakeNo, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                            };

                dt = await ExcuteSP("sfis1.GEMALTO_ROAST_CHECK_BAKE", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt.Rows[0]["res"].ToString();
        }

        public async Task<string> UpTrayStationProcedure(SfcHttpClient sfcHttpClient,string line, string section, string myGroup, string station, string trayNo, string bakeNo ="" )
        {
            DataTable dt = null;
            try
            {
                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "line", Value = line, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "section", Value = section, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "mygroup", Value = myGroup, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "w_station", Value = station, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "trayno", Value = trayNo, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "bakeno", Value = bakeNo, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "empno", Value = SFCThales.Form1.empNo, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                            };

                dt = await ExcuteSP("GEMALTO_ROAST_BAKETRAY", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt.Rows[0]["res"].ToString();
        }

        public async Task<string>  MakeNGProcedure(string line, string section, string myGroup, string station, string sn, string ec, SfcHttpClient sfcHttpClient)
        {
            DataTable dt = null;
            try
            {
                List<SfcParameter> ListPara;
                ListPara = new List<SfcParameter>()
                            {
                                new SfcParameter { Name = "line", Value = line, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "section", Value = section, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "mygroup", Value = myGroup, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "w_station", Value = station, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "data", Value = sn, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "empno", Value = SFCThales.Form1.empNo, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "ec", Value = ec, SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Input },
                                new SfcParameter { Name = "res",  SfcParameterDataType = SfcParameterDataType.Varchar2, SfcParameterDirection = SfcParameterDirection.Output }
                            };

                dt = await ExcuteSP("GEMALTO_ROAST_BAKETRAY", ListPara, sfcHttpClient);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
            }
            return dt.Rows[0]["res"].ToString();
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
