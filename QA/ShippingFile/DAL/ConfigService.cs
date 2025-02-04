using System;
using System.Data;
using System.Management;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class ConfigService
    {
        #region Config 1 C_LINE_DESC_T
        public static DataTable GetAllLineName()
        {
            try
            {
                string sql = @"SELECT DISTINCT LINE_NAME FROM SFIS1.C_LINE_DESC_T ORDER BY LINE_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllLineInfo()
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_LINE_DESC_T ORDER BY LINE_NAME ASC";

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
        public static DataTable GetLineInfoByName(string sLineName)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format("SELECT * FROM SFIS1.C_LINE_DESC_T WHERE LINE_NAME LIKE '{0}%' ORDER BY LINE_NAME ASC", sLineName);

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
        public static DataTable GetNewLineInfoByName(string sLineName)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format("SELECT * FROM SFIS1.C_LINE_DESC_T WHERE LINE_NAME =:LN ORDER BY LINE_NAME ASC");

                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":LN",sLineName)
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
        #endregion

        #region Config 2 C_SECTION_CONFIG_T/C_GROUP_CONFIG_T
        public static DataTable GetAllSection()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_SECTION_CONFIG_T ORDER BY SECTION_CODE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetMaxSectionCode()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MAX(SECTION_CODE) SECTION_CODE FROM SFIS1.C_SECTION_CONFIG_T");
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
        public static DataTable GetSectionByname(string sSection)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_SECTION_CONFIG_T WHERE SECTION_NAME = '{0}' ORDER BY SECTION_CODE ASC", sSection);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllGroup()
        {
            try
            {
                string sql = @"SELECT DISTINCT GROUP_NAME FROM SFIS1.C_GROUP_CONFIG_T ORDER BY GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllPassGroup()
        {
            try
            {
                string sql = @"SELECT DISTINCT GROUP_NAME FROM SFIS1.C_GROUP_CONFIG_T WHERE GROUP_NAME NOT LIKE 'R_%' ORDER BY GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllFailGroup()
        {
            try
            {
                string sql = @"SELECT DISTINCT GROUP_NAME FROM SFIS1.C_GROUP_CONFIG_T WHERE GROUP_NAME LIKE 'R_%' ORDER BY GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetMaxGroupCode()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MAX(GROUP_CODE) GROUP_CODE FROM SFIS1.C_GROUP_CONFIG_T");
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
        public static DataTable GetAllSectionGroup()
        {
            try
            {
                string sql =  @" SELECT A.SECTION_NAME,
                                        A.SECTION_DESC,
                                        B.GROUP_NAME,
                                        B.GROUP_DESC,
                                        B.UPPER_LIMIT,
                                        B.LOWER_LIMIT
                                    FROM SFIS1.C_SECTION_CONFIG_T A, SFIS1.C_GROUP_CONFIG_T B
                                    WHERE A.SECTION_NAME = B.SECTION_NAME
                                    ORDER BY A.SECTION_CODE, B.GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetSectionGroup(string sInputType, string sInputValue)
        {
            try
            {
                string sql = string.Format(@"SELECT A.SECTION_NAME,
                                                    A.SECTION_DESC,
                                                    B.GROUP_NAME,
                                                    B.GROUP_DESC,
                                                    B.UPPER_LIMIT,
                                                    B.LOWER_LIMIT
                                               FROM SFIS1.C_SECTION_CONFIG_T A, SFIS1.C_GROUP_CONFIG_T B
                                              WHERE A.SECTION_NAME = B.SECTION_NAME
                                                AND B.{0} = '{1}'
                                              ORDER BY A.SECTION_CODE, B.GROUP_NAME ASC ", sInputType, sInputValue);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewSectionGroup(string sSection, string sGroup)
        {
            try
            {
                string sql = string.Format(@"SELECT A.SECTION_NAME,
                                                    A.SECTION_DESC,
                                                    B.GROUP_NAME,
                                                    B.GROUP_DESC,
                                                    B.UPPER_LIMIT,
                                                    B.LOWER_LIMIT
                                               FROM SFIS1.C_SECTION_CONFIG_T A, SFIS1.C_GROUP_CONFIG_T B
                                              WHERE A.SECTION_NAME = B.SECTION_NAME
                                                AND B.SECTION_NAME = '{0}'
                                                AND B.GROUP_NAME = '{1}'
                                              ORDER BY A.SECTION_CODE, B.GROUP_NAME ASC ", sSection, sGroup);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 3 C_STATION_CONFIG_T
        public static DataTable GetAllLineStation()
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT A.LINE_NAME, B.SECTION_NAME, B.GROUP_NAME, B.STATION_NAME 
                                 FROM SFIS1.C_LINE_DESC_T A, SFIS1.C_STATION_CONFIG_T B 
                                WHERE A.LINE_NAME = B.LINE_NAME AND B.HOSTID = -1 
                                ORDER BY A.LINE_NAME, B.SECTION_NAME, B.GROUP_NAME, B.STATION_NAME ASC";
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
        public static DataTable GetLineStation(string sLineName)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME 
                                               FROM SFIS1.C_STATION_CONFIG_T 
                                              WHERE LINE_NAME = '{0}' AND HOSTID = -1 
                                              ORDER BY LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME  ASC", sLineName);
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
        public static DataTable GetNewLineStation(string sLine, string sSection, string sGroup, string sStation)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME 
                                               FROM SFIS1.C_STATION_CONFIG_T 
                                              WHERE HOSTID = -1 AND LINE_NAME ='{0}' AND SECTION_NAME ='{1}' 
                                                AND GROUP_NAME ='{2}' AND STATION_NAME ='{3}'
                                              ORDER BY LINE_NAME, SECTION_NAME, GROUP_NAME, STATION_NAME  ASC", 
                                              sLine, sSection, sGroup, sStation);
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
        public static DataTable GetMaxStationNumber()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT MAX(STATION_NUMBER) STATION_NUMBER
                                               FROM SFIS1.C_STATION_CONFIG_T 
                                              WHERE HOSTID = -1 ");
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
        #endregion

        #region Config 4 C_WORK_DESC_T/C_CLASS_WORK_T
        public static DataTable GetAllClass()
        {
            try
            {
                string sql = @"SELECT LINE_NAME, 
                                      START_TIME, 
                                      END_TIME, 
                                      CLASS, SERIAL, 
                                      DAY_DISTINCT, 
                                      CREATE_DATE
                                 FROM SFIS1.C_CLASS_WORK_T 
                                ORDER BY LINE_NAME, SERIAL";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetClassByLine(string sLineName)
        {
            try
            {
                string sql = string.Format(@"SELECT LINE_NAME, 
                                                    START_TIME, 
                                                    END_TIME, 
                                                    CLASS, SERIAL, 
                                                    DAY_DISTINCT, 
                                                    CREATE_DATE
                                               FROM SFIS1.C_CLASS_WORK_T 
                                              WHERE LINE_NAME = '{0}'
                                              ORDER BY LINE_NAME, SERIAL",sLineName);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetWorkByLine(string sLineName)
        {
            try
            {
                string sql = string.Format(@"SELECT *
                                               FROM SFIS1.C_WORK_DESC_T 
                                              WHERE LINE_NAME = '{0}'
                                              ORDER BY LINE_NAME, WORK_SECTION", sLineName);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 5 C_ROUTE_NAME_T/C_ROUTE_CONTROL_T
        public static DataTable GetAllRouteName()
        {
            try
            {
                string sql = @"SELECT DISTINCT ROUTE_NAME FROM SFIS1.C_ROUTE_NAME_T ORDER BY ROUTE_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetRouteCodeByName(string routeName)
        {
            try
            {
                string sql = @"SELECT ROUTE_CODE FROM SFIS1.C_ROUTE_NAME_T WHERE ROUTE_NAME =:ROUTENAME ";
                OracleParameter parameter = new OracleParameter(":ROUTENAME", routeName);
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
        public static DataTable GetRouteNameByCode(string routeCode)
        {
            try
            {
                string sql = @"SELECT ROUTE_NAME FROM SFIS1.C_ROUTE_NAME_T WHERE ROUTE_CODE =:ROUTECODE ";
                OracleParameter parameter = new OracleParameter(":ROUTECODE", routeCode);
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
        public static DataTable GetRouteGroupByCode(string routeCode)
        {
            try
            {
                string sql = @"SELECT DISTINCT GROUP_NEXT FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE =:ROUTECODE ORDER BY GROUP_NEXT ";
                OracleParameter parameter = new OracleParameter(":ROUTECODE", routeCode);
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
        #endregion

        #region Config 6 C_MODEL_DESC_T
        public static DataTable GetAllModelInfo()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_MODEL_DESC_T ORDER BY MODEL_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllModelName()
        {
            try
            {
                string sql = @"SELECT DISTINCT MODEL_NAME FROM SFIS1.C_MODEL_DESC_T ORDER BY MODEL_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllModelSerial()
        {
            try
            {
                string sql = @"SELECT DISTINCT MODEL_SERIAL FROM SFIS1.C_MODEL_DESC_T ORDER BY MODEL_SERIAL ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllModelType()
        {
            try
            {
                string sql = @"SELECT DISTINCT MODEL_TYPE FROM SFIS1.C_MODEL_DESC_T ORDER BY MODEL_TYPE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetModelInfoByName(string sModelName)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME LIKE '{0}%' ORDER BY MODEL_NAME ASC", sModelName);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewModelInfoByName(string sModelName)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_MODEL_DESC_T WHERE MODEL_NAME = '{0}' ORDER BY MODEL_NAME ASC", sModelName);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 7 C_EMP_DESC_T/C_EMP_2_GROUP_T
        public static DataTable GetAllEmpInfo()
        {
            try
            {
                string sql = @"SELECT EMP_NO, EMP_NAME, CLASS_NAME, STATION_NAME, EMAIL, QUIT_DATE 
                                 FROM SFIS1.C_EMP_DESC_T
                                WHERE LENGTH(EMP_NO) >= 6 
                                  AND QUIT_DATE > SYSDATE 
                                  AND SUBSTR(EMP_NO, 1, 1) IN ('0','1','2','3','4','5','6','7','8','9',
                                                               'A','B','C','D','E','F','G','H','I','J','K','L','M',
                                                               'N','O','P','Q','R','S','T','U','V','W','X','Y','Z') 
                               ORDER BY EMP_NO ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetEmpBelongGroup(string sEmpNo)
        {
            try
            {
                string sql = string.Format(@"SELECT DISTINCT GROUP_NAME 
                                               FROM SFIS1.C_EMP_2_GROUP_T 
                                              WHERE EMP_NO = '{0}' ORDER BY GROUP_NAME ASC", sEmpNo);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetEmpNotBelongGroup(string sEmpNo)
        {
            try
            {
                string sql = string.Format(@"SELECT DISTINCT GROUP_NAME 
                                               FROM SFIS1.C_GROUP_CONFIG_T 
                                              WHERE SUBSTR(GROUP_NAME,1,2) !='R_'
                                              MINUS
                                             SELECT DISTINCT GROUP_NAME 
                                               FROM SFIS1.C_EMP_2_GROUP_T 
                                              WHERE EMP_NO = '{0}' ORDER BY GROUP_NAME ASC", sEmpNo);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetEmpInfoByEmpNo(string sEmpNo)
        {
            try
            {
                string sql = string.Format(@"SELECT EMP_NO, EMP_NAME, CLASS_NAME, STATION_NAME, EMAIL, QUIT_DATE  
                                               FROM SFIS1.C_EMP_DESC_T 
                                              WHERE EMP_NO LIKE '{0}%' ORDER BY EMP_NO ASC", sEmpNo);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewEmpInfoByEmpNo(string sEmpNo)
        {
            try
            {
                string sql = string.Format(@"SELECT EMP_NO, EMP_NAME, CLASS_NAME, STATION_NAME, EMAIL, QUIT_DATE  
                                               FROM SFIS1.C_EMP_DESC_T 
                                              WHERE EMP_NO = '{0}' ORDER BY EMP_NO ASC", sEmpNo);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetEmpPWDByEmpNo(string sEmpNo)
        {
            try
            {
                string sql = string.Format(@"SELECT EMP_NO, EMP_PASS, EMP_BC 
                                               FROM SFIS1.C_EMP_DESC_T 
                                              WHERE EMP_NO = '{0}' AND QUIT_DATE > SYSDATE ORDER BY EMP_NO ASC", sEmpNo);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 8 C_ERROR_CODE_T
        public static DataTable GetErrorCodeInfo(string errorCode)
        {
            DataTable dtError = null;
            try
            {
                string sql = "SELECT * FROM SFIS1.C_ERROR_CODE_T "
                          + "WHERE ERROR_CODE=:errorCode AND ROWNUM=1";
                OracleParameter para = new OracleParameter(":errorCode", errorCode);
                dtError = DBHelper.GetDataTable(sql, para);
            }
            catch
            {
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dtError;
        }
        #endregion

        #region Config 9 C_REASON_CODE_T
        public static DataTable GetAllReasonInfo()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_REASON_CODE_T ORDER BY REASON_CODE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetReasonByCode(string sReason)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_REASON_CODE_T WHERE REASON_CODE LIKE '{0}%' ORDER BY REASON_CODE ASC", sReason);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewReasonByCode(string sReason)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_REASON_CODE_T WHERE REASON_CODE = '{0}' ORDER BY REASON_CODE ASC", sReason);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 10 C_ITEM_DESC_T
        #endregion

        #region Config 11 C_KEYPARTS_DESC_T
        public static DataTable GetAllKeyPartNo()
        {
            try
            {
                string sql = @"SELECT KP_NAME,KEY_PART_NO,KP_DESC FROM SFIS1.C_KEYPARTS_DESC_T ORDER BY KP_NAME,KEY_PART_NO ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllKPName()
        {
            try
            {
                string sql = string.Format(@"SELECT DISTINCT KP_NAME FROM SFIS1.C_KEYPARTS_DESC_T ORDER BY KP_NAME ASC");
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetKeyPartNoByNo(string sKPNO)
        {
            try
            {
                string sql = string.Format(@"SELECT KP_NAME,KEY_PART_NO,KP_DESC FROM SFIS1.C_KEYPARTS_DESC_T WHERE KEY_PART_NO LIKE '{0}%' ORDER BY KEY_PART_NO ASC", sKPNO);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewKKPNOByNo(string sKPNO)
        {
            try
            {
                string sql = string.Format(@"SELECT KP_NAME,KEY_PART_NO,KP_DESC FROM SFIS1.C_KEYPARTS_DESC_T WHERE KEY_PART_NO = '{0}' ORDER BY KEY_PART_NO ASC", sKPNO);
                return DBHelper.GetDataTable(sql, null);
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


        #endregion

        #region Config 12 C_BOM_KEYPART_T
        public static DataTable GetAllBOMInfo()
        {
            try
            {
                string sql = @"SELECT A.BOM_NO, B.KP_NAME , A.KEY_PART_NO, A.KP_RELATION, A.KP_COUNT, A.GROUP_NAME, A.TYPE, A.WORK_DATE
                                 FROM SFIS1.C_BOM_KEYPART_T A, SFIS1.C_KEYPARTS_DESC_T B
                                WHERE A.KEY_PART_NO = B.KEY_PART_NO(+)
                                ORDER BY A.BOM_NO,A.KP_RELATION,A.GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetBomKPInfoByBomNo(string sBomNo)
        {
            try
            {
                string sql = string.Format(@"SELECT A.BOM_NO, B.KP_NAME, A.KEY_PART_NO, A.KP_RELATION, A.KP_COUNT, A.GROUP_NAME, A.TYPE, A.WORK_DATE
                                               FROM SFIS1.C_BOM_KEYPART_T A, SFIS1.C_KEYPARTS_DESC_T B
                                              WHERE A.BOM_NO LIKE '{0}%'
                                                AND A.KEY_PART_NO = B.KEY_PART_NO(+)
                                              ORDER BY A.BOM_NO, A.KP_RELATION ASC ", sBomNo);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetBomKPInfoByNewBomNo(string sBomNo)
        {
            try
            {
                string sql = string.Format(@"SELECT A.BOM_NO, B.KP_NAME, A.KEY_PART_NO, A.KP_RELATION, A.KP_COUNT, A.GROUP_NAME, A.TYPE, A.WORK_DATE
                                               FROM SFIS1.C_BOM_KEYPART_T A, SFIS1.C_KEYPARTS_DESC_T B
                                              WHERE A.BOM_NO = '{0}'
                                                AND A.KEY_PART_NO = B.KEY_PART_NO(+) 
                                              ORDER BY A.BOM_NO, A.KP_RELATION ASC ", sBomNo);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetBomKPByKPNO(string sBomNo, string sKeyPartNo)
        {
            try
            {
                string sql = string.Format(@"SELECT B.KP_NAME KP_TYPE, A.KEY_PART_NO KPNO, A.KP_COUNT QTY, A.GROUP_NAME, A.KP_RELATION RELATION
                                               FROM SFIS1.C_BOM_KEYPART_T A, SFIS1.C_KEYPARTS_DESC_T B
                                              WHERE A.BOM_NO = '{0}'
                                                AND A.KEY_PART_NO = B.KEY_PART_NO
                                                AND A.KEY_PART_NO = '{1}'
                                              ORDER BY A.KEY_PART_NO ASC", sBomNo, sKeyPartNo);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetBomSubKPByKpno(string sBomNo, string sRelation)
        {
            try
            {
                string sql = string.Format(@"SELECT B.KP_NAME KP_TYPE, A.KEY_PART_NO KPNO, A.KP_COUNT QTY, A.GROUP_NAME, A.KP_RELATION RELATION
                                               FROM SFIS1.C_BOM_KEYPART_T A, SFIS1.C_KEYPARTS_DESC_T B
                                              WHERE A.BOM_NO = '{0}'
                                                AND A.KEY_PART_NO = B.KEY_PART_NO
                                                AND A.KP_RELATION = '{1}'
                                              ORDER BY A.KP_RELATION ASC",sBomNo, sRelation);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetBomKPRelationByBomNo(string sBomNo)
        {
            try
            {
                string sql = string.Format(@"SELECT BOM_NO, KP_RELATION ,ROWNUM
                                               FROM (SELECT DISTINCT BOM_NO,KP_RELATION
                                                       FROM SFIS1.C_BOM_KEYPART_T
                                                      WHERE BOM_NO ='{0}' ORDER BY KP_RELATION ASC) ", sBomNo);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 13 C_DUTY_T
        public static DataTable GetAllDutyInfo()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_DUTY_T ORDER BY DUTY_TYPE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetDutyInfoByType(string sInputType, string sInputValue)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_DUTY_T WHERE {0} LIKE '{1}%' ORDER BY DUTY_TYPE ", sInputType, sInputValue);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewDutyInfo(string sInputType, string sInputValue)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_DUTY_T WHERE {0} = '{1}' ", sInputType, sInputValue);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 14 C_CUSTOMER_T
        public static DataTable GetAllCustomer()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_CUSTOMER_T WHERE PASSWD IS NULL ORDER BY CUST_CODE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetCustomerInfo(string sInputType, string sInputValue)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_CUSTOMER_T WHERE PASSWD IS NULL AND {0} LIKE '%{1}%' ORDER BY CUST_CODE ASC ", sInputType, sInputValue);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetCustomer(string sInputType, string sInputValue)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_CUSTOMER_T WHERE PASSWD IS NULL AND {0} = '{1}' ", sInputType, sInputValue);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewCustomer(string sCustCode)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_CUSTOMER_T WHERE CUST_CODE = '{0}' ", sCustCode);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 15 C_PACK_PARAM_T
        public static DataTable GetAllPackParam()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_PACK_PARAM_T ORDER BY MODEL_NAME, VERSION_CODE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllCreateName()
        {
            try
            {
                string sql = @"SELECT DISTINCT CREATE_NAME FROM SFIS1.UC_CREATE_NAME_T ORDER BY CREATE_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetPackParamByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_PACK_PARAM_T WHERE MODEL_NAME LIKE '{0}%' ORDER BY MODEL_NAME, VERSION_CODE ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewPackParamByModel(string sModel, string sVersion)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_PACK_PARAM_T 
                                              WHERE MODEL_NAME = '{0}' AND VERSION_CODE = '{1}' 
                                              ORDER BY MODEL_NAME, VERSION_CODE ASC", sModel, sVersion);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 19 C_CUST_SNRULE_T
        public static DataTable GetAllSnRule()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_CUST_SNRULE_T ORDER BY MODEL_NAME, VERSION_CODE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetSnRuleByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME LIKE '{0}%' ORDER BY MODEL_NAME, VERSION_CODE ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewSnRuleByModelVersion(string sModel, string sVersion)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_CUST_SNRULE_T 
                                              WHERE MODEL_NAME ='{0}' AND VERSION_CODE='{1}' 
                                              ORDER BY MODEL_NAME, VERSION_CODE ASC", sModel, sVersion);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 42 C_GROUP_SAP_MAPPING_T
        public static DataTable GetAllGroupMapping()
        {
            try
            {
                string sql = @"SELECT MODEL_NAME, ROUTE_TYPE, SEQUENCE_NO, GROUP_NAME FROM SFIS1.C_GROUP_SAP_MAPPING_T ORDER BY MODEL_NAME, ROUTE_TYPE, SEQUENCE_NO ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetGroupMappingByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT MODEL_NAME, ROUTE_TYPE, SEQUENCE_NO, GROUP_NAME 
                                               FROM SFIS1.C_GROUP_SAP_MAPPING_T 
                                              WHERE MODEL_NAME LIKE '{0}%'
                                              ORDER BY MODEL_NAME, ROUTE_TYPE, SEQUENCE_NO ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewGroupMapping(string sModel, string sRouteType, string sSequence)
        {
            try
            {
                string sql = string.Format(@"SELECT MODEL_NAME, ROUTE_TYPE, SEQUENCE_NO, GROUP_NAME 
                                               FROM SFIS1.C_GROUP_SAP_MAPPING_T 
                                              WHERE MODEL_NAME = '{0}' AND ROUTE_TYPE = '{1}' AND SEQUENCE_NO = '{2}' 
                                              ORDER BY MODEL_NAME, ROUTE_TYPE, SEQUENCE_NO ASC", sModel, sRouteType, sSequence);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 43 C_PACK_SEQUENCE_T
        public static DataTable GetAllPackSequence()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_PACK_SEQUENCE_T ORDER BY MODEL_NAME, VERSION_CODE, MO_TYPE, SCAN_SEQ ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetAllCustSnName()
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_CUSTSN_DESC_T  ORDER BY CUSTSN_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetPackSeqByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_PACK_SEQUENCE_T 
                                              WHERE MODEL_NAME LIKE '{0}%'
                                              ORDER BY MODEL_NAME, VERSION_CODE, MO_TYPE, SCAN_SEQ ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewPackSeq(string sModel, string sVersion, string sMoType)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_PACK_SEQUENCE_T 
                                              WHERE MODEL_NAME = '{0}' AND VERSION_CODE = '{1}' AND MO_TYPE = '{2}'
                                              ORDER BY MODEL_NAME, VERSION_CODE, MO_TYPE, SCAN_SEQ ASC", sModel, sVersion, sMoType);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 44 C_CUSTSN_RULE_T
        public static DataTable GetAllStepRule()
        {
            try
            {
                string sql = @"  SELECT B.MODEL_NAME,
                                        B.VERSION_CODE,
                                        B.MO_TYPE,
                                        B.CUSTSN_CODE,
                                        B.CUSTSN_PREFIX,
                                        B.CUSTSN_POSTFIX,
                                        B.CUSTSN_LENG,
                                        B.CUSTSN_STR,
                                        B.CHECK_SSN,
                                        B.CHECK_RANGE,
                                        B.CHECK_RULE_NAME,
                                        B.SHIPPINGSN_CODE,
                                        B.COMPARE_SN,
                                        B.COMPARE_SN_START,
                                        B.COMPARE_SN_END,
                                        B.CUSTSN_START,
                                        B.CUSTSN_END,
                                        B.CREATE_NAME,
                                        B.MODIFY_NAME,
                                        B.IN_STATION_TIME 
                                   FROM SFIS1.C_CUSTSN_RULE_T B 
                                  ORDER BY B.MODEL_NAME, B.VERSION_CODE, B.MO_TYPE ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetStepRuleByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT B.MODEL_NAME,
                                                    B.VERSION_CODE,
                                                    B.MO_TYPE,
                                                    B.CUSTSN_CODE,
                                                    B.CUSTSN_PREFIX,
                                                    B.CUSTSN_POSTFIX,
                                                    B.CUSTSN_LENG,
                                                    B.CUSTSN_STR,
                                                    B.CHECK_SSN,
                                                    B.CHECK_RANGE,
                                                    B.CHECK_RULE_NAME,
                                                    B.SHIPPINGSN_CODE,
                                                    B.COMPARE_SN,
                                                    B.COMPARE_SN_START,
                                                    B.COMPARE_SN_END,
                                                    B.CUSTSN_START,
                                                    B.CUSTSN_END,
                                                    B.CREATE_NAME,
                                                    B.MODIFY_NAME,
                                                    B.IN_STATION_TIME
                                               FROM SFIS1.C_CUSTSN_RULE_T B
                                              WHERE B.MODEL_NAME LIKE '{0}%'
                                              ORDER BY B.MODEL_NAME, B.VERSION_CODE, B.MO_TYPE, B.CUSTSN_CODE ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetStepRuleByModelVersion(string sModel, string sVersion, string sMoType)
        {
            try
            {
                string sql = string.Format(@"SELECT B.MODEL_NAME,
                                                    B.VERSION_CODE,
                                                    B.MO_TYPE,
                                                    B.CUSTSN_CODE,
                                                    B.CUSTSN_PREFIX,
                                                    B.CUSTSN_POSTFIX,
                                                    B.CUSTSN_LENG,
                                                    B.CUSTSN_STR,
                                                    B.CHECK_SSN,
                                                    B.CHECK_RANGE,
                                                    B.CHECK_RULE_NAME,
                                                    B.SHIPPINGSN_CODE,
                                                    B.COMPARE_SN,
                                                    B.COMPARE_SN_START,
                                                    B.COMPARE_SN_END,
                                                    B.CUSTSN_START,
                                                    B.CUSTSN_END,
                                                    B.CREATE_NAME,
                                                    B.MODIFY_NAME,
                                                    B.IN_STATION_TIME
                                               FROM SFIS1.C_CUSTSN_RULE_T B
                                              WHERE B.MODEL_NAME = '{0}' AND B.VERSION_CODE = '{1}' AND B.MO_TYPE = '{2}'
                                              ORDER BY B.MODEL_NAME, B.VERSION_CODE, B.MO_TYPE, B.CUSTSN_CODE ASC", sModel, sVersion, sMoType);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewStepRule(string sModel, string sVersion, string sMoType, string sCustsnCode)
        {
            try
            {
                string sql = string.Format(@"SELECT B.MODEL_NAME,
                                                    B.VERSION_CODE,
                                                    B.MO_TYPE,
                                                    B.CUSTSN_CODE,
                                                    B.CUSTSN_PREFIX,
                                                    B.CUSTSN_POSTFIX,
                                                    B.CUSTSN_LENG,
                                                    B.CUSTSN_STR,
                                                    B.CHECK_SSN,
                                                    B.CHECK_RANGE,
                                                    B.CHECK_RULE_NAME,
                                                    B.SHIPPINGSN_CODE,
                                                    B.COMPARE_SN,
                                                    B.COMPARE_SN_START,
                                                    B.COMPARE_SN_END,
                                                    B.CUSTSN_START,
                                                    B.CUSTSN_END,
                                                    B.CREATE_NAME,
                                                    B.MODIFY_NAME,
                                                    B.IN_STATION_TIME
                                               FROM SFIS1.C_CUSTSN_RULE_T B
                                              WHERE B.MODEL_NAME = '{0}' AND B.VERSION_CODE = '{1}' 
                                                AND B.MO_TYPE = '{2}' AND B.CUSTSN_CODE = '{3}'
                                              ORDER BY B.MODEL_NAME, B.VERSION_CODE, B.MO_TYPE ASC", sModel, sVersion, sMoType, sCustsnCode);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetCustsnCode(string sModel, string sVersion, string sMoType)
        {
            try
            {
                string sql = string.Format(@"SELECT CUSTSN_CODE FROM SFIS1.C_CUSTSN_DESC_T 
                                              WHERE CUSTSN_NAME IN (SELECT CUSTSN_NAME FROM SFIS1.C_PACK_SEQUENCE_T 
                                                                     WHERE MODEL_NAME ='{0}' 
                                                                       AND VERSION_CODE ='{1}' 
                                                                       AND MO_TYPE ='{2}') ", 
                                                                       sModel, sVersion, sMoType);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetSsnRuleName(string sCustsnCode, string sRuleType)
        {
            try
            {
                string sql = string.Format(@"SELECT RULL_NAME FROM SFIS1.C_SSN_RULE_NAME_T WHERE RULE_TYPE ='{0}' ", sRuleType);
                if (sCustsnCode == "MAC1")
                {
                    sql = sql + " AND RULL_NAME<>'NEXTMAC'";
                }
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 50 C_HOST_NAME_T
        public static DataTable GetAllHostName()
        {
            try
            {
                string sql = @" SELECT * FROM SFIS1.C_HOST_NAME_T ORDER BY HOSTID ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetHostByName(string sHostName)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_HOST_NAME_T
                                              WHERE HOST_NAME LIKE '{0}%'
                                              ORDER BY HOSTID ASC", sHostName);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewHostByName(string sHostName)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_HOST_NAME_T
                                              WHERE HOST_NAME = '{0}'
                                              ORDER BY HOSTID ASC", sHostName);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewHostByID(string sHostID)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_HOST_NAME_T
                                              WHERE HOSTID = '{0}'
                                              ORDER BY HOSTID ASC", sHostID);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 56 C_MODEL_ATE_SET_T
        public static DataTable GetAllModelATE()
        {
            try
            {
                string sql = @" SELECT A.MODEL_NAME,
                                       A.GROUP_NAME,
                                       A.PASS_LIMIT,
                                       A.RETEST_LIMIT,
                                       A.RETEST_MODEL,
                                       A.REPASS_MODEL 
                                  FROM SFIS1.C_MODEL_ATE_SET_T A 
                                 ORDER BY A.MODEL_NAME, A.GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetATEByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT A.MODEL_NAME,
                                                    A.GROUP_NAME,
                                                    A.PASS_LIMIT,
                                                    A.RETEST_LIMIT,
                                                    A.RETEST_MODEL,
                                                    A.REPASS_MODEL 
                                                FROM SFIS1.C_MODEL_ATE_SET_T A
                                              WHERE A.MODEL_NAME = '{0}'
                                              ORDER BY A.MODEL_NAME, A.GROUP_NAME ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetATEByModelGroup(string sModel, string sGroup)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_MODEL_ATE_SET_T 
                                              WHERE MODEL_NAME = '{0}' AND GROUP_NAME = '{1}'
                                              ORDER BY MODEL_NAME, GROUP_NAME ASC", sModel, sGroup);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 58 C_MODEL_WEIGHT_T
        public static DataTable GetAllModelWeight()
        {
            try
            {
                string sql = @" SELECT A.MODEL_NAME,
                                       A.VERSION_CODE,
                                       A.GROUP_NAME,
                                       A.MIN_WEIGHT,
                                       A.MAX_WEIGHT,
                                       A.MODIFY_NAME,
                                       A.MODIFY_DATE 
                                  FROM SFIS1.C_MODEL_WEIGHT_T A 
                                 ORDER BY MODEL_NAME, VERSION_CODE, GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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

        public static DataTable GetWeightByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT A.MODEL_NAME,
                                                    A.VERSION_CODE,
                                                    A.GROUP_NAME,
                                                    A.MIN_WEIGHT,
                                                    A.MAX_WEIGHT,
                                                    A.MODIFY_NAME,
                                                    A.MODIFY_DATE 
                                               FROM SFIS1.C_MODEL_WEIGHT_T A 
                                              WHERE A.MODEL_NAME = '{0}'
                                              ORDER BY MODEL_NAME, VERSION_CODE, GROUP_NAME ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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
        public static DataTable GetNewWeight(string sModel, string sVersion, string sGroup)
        {
            try
            {
                string sql = string.Format(@"SELECT A.MODEL_NAME,
                                                    A.VERSION_CODE,
                                                    A.GROUP_NAME,
                                                    A.MIN_WEIGHT,
                                                    A.MAX_WEIGHT,
                                                    A.MODIFY_NAME,
                                                    A.MODIFY_DATE 
                                               FROM SFIS1.C_MODEL_WEIGHT_T A 
                                              WHERE A.MODEL_NAME = '{0}'  AND A.VERSION_CODE = '{1}' AND A.GROUP_NAME = '{2}'
                                              ORDER BY MODEL_NAME, VERSION_CODE, GROUP_NAME ASC", sModel, sVersion, sGroup);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 59 C_MODEL_AUTOALARM_SET_T
        public static DataTable GetAllModelAlarm()
        {
            try
            {
                string sql = @" SELECT * FROM SFIS1.C_MODEL_AUTOALARM_SET_T ORDER BY MODEL_NAME, GROUP_NAME ASC";
                return DBHelper.GetDataTable(sql, null);
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

        public static DataTable GetAlarmByModel(string sModel)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_MODEL_AUTOALARM_SET_T 
                                              WHERE MODEL_NAME = '{0}'
                                              ORDER BY MODEL_NAME, GROUP_NAME ASC", sModel);
                return DBHelper.GetDataTable(sql, null);
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

        public static DataTable GetAlarmByModelGroup(string sModel, string sGroup)
        {
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_MODEL_AUTOALARM_SET_T 
                                              WHERE MODEL_NAME = '{0}' AND GROUP_NAME = '{1}'
                                              ORDER BY MODEL_NAME, GROUP_NAME ASC", sModel, sGroup);
                return DBHelper.GetDataTable(sql, null);
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
        #endregion

        #region Config 58 C_CUSTSN_RULE_T
        #endregion

        public static String GetDateCode()
        {
            try
            {
                string sql = "SELECT TO_CHAR(SYSDATE,'YYYYWW') FROM DUAL";
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
        public static DataTable GetRePrintPallet(string pallet)
        {
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_PACK_PACKUNITNO_T WHERE PACKUNIT_NO =:PALLET AND PACK_STATUS ='Close' ";
                OracleParameter parameter = new OracleParameter(":PALLET", pallet);
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

        public static DataTable CheckConfig19(string model, string version)
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_CUST_SNRULE_T WHERE MODEL_NAME =:MODEL AND VERSION_CODE =:VERSION  AND ROWNUM = 1";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":MODEL",model),
                    new OracleParameter(":VERSION",version)
                };
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
