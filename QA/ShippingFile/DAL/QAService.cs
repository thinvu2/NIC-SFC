using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class QAService
    {
        //Get TrayQty OR BoxQty
        public static string GetTrayQty(string TrayNo)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) TrayQty FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO=:TrayNo";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":TrayNo", TrayNo)
                };
                QTY = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return QTY;
        }
        //Get CartonQty
        public static string GetCartonQty(string CartonNo)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) CartonQty FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO=:CartonNo";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":CartonNo", CartonNo)      
                };
                QTY = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return QTY;
        }
        //Get PalletSNQty
        public static string GetPalletQty(string PalletNo)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) PalletSNQty FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI=:PalletNo";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":PalletNo", PalletNo)      
                };
                QTY = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return QTY;
        }
        //Get OBA Check SN List By PKGID
        public static DataTable GetOBASNByLotNo(string LotNo)
        {
            try
            {
                string sql = " SELECT MODEL_NAME,SERIAL_NUMBER,DECODE(ERROR_FLAG,'1','Fail','0','Pass','Pass') TEST_STATUS,TESTER, "
                          + " GROUP_NAME,STATION_NAME,TEST_TIME FROM SFISM4.R_QC_SN_T WHERE LOT_NO=:LotNo ORDER BY SERIAL_NUMBER ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":LotNo", LotNo)      
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
        //Get LotNo Fail
        public static string GetOBAFailByLotNo(string LotNo)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) FAILQTY  FROM SFISM4.R_QC_SN_T WHERE LOT_NO=:LotNo AND ERROR_FLAG='1' ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":LotNo", LotNo)      
                };
                QTY = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return QTY;
        }
        //Get PalletCartonQty
        public static string GetModelOBABaseQty(string ModelName)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @" SELECT OBA_BASEQTY FROM SFIS1.C_OBA_MODEL_T WHERE MODEL_NAME=:ModelName";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":ModelName", ModelName)      
                };
                QTY = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return QTY;
        } 
        //Get Model OBA Setup Info
        public static DataTable GetModelOBABaseInfo()
        {

            try
            {
                string sql = @" SELECT MODEL_NAME,CHECK_UNIT,OBA_BASEQTY,EMP_NAME,IN_STATION_TIME FROM SFIS1.C_OBA_MODEL_T ORDER BY MODEL_NAME ";
                
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
        //Get Model OBA Setup Info By Model
        public static DataTable GetModelOBABaseInfoByModel(string ModelName)
        {
            
            try
            {
                string sql = @" SELECT MODEL_NAME,CHECK_UNIT,OBA_BASEQTY,EMP_NAME,IN_STATION_TIME FROM SFIS1.C_OBA_MODEL_T WHERE MODEL_NAME=:ModelName";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":ModelName", ModelName)      
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
        //Insert into SFIS1.C_MODEL_OBA_T
        public static int InsertModelOBAInfo(string InputModel, string InputGroup, string InputCheckUit, string InputOBAQty, string empName)
        {
            int result = 0;
            try
            {
                string sql = " INSERT INTO SFIS1.C_OBA_MODEL_T (MODEL_NAME,GROUP_NAME,CHECK_UNIT,OBA_BASEQTY,EMP_NAME ) "
                            + " Values(:InputModel,:InputGroup,:InputCheckUit,:InputOBAQty,:empName) ";

                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":InputModel", InputModel), 
                    new OracleParameter(":InputGroup", InputGroup), 
                    new OracleParameter(":InputCheckUit", InputCheckUit), 
                    new OracleParameter(":InputOBAQty", InputOBAQty), 
                    new OracleParameter(":empName", empName) 
                };
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, parameter));
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
        //Update SFIS1.C_MODEL_OBA_T
        public static int UpdateModelOBAInfo(string InputModel, string InputGroup, string InputCheckUit, string InputOBAQty, string empName)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFIS1.C_OBA_MODEL_T SET GROUP_NAME='{1}',CHECK_UNIT='{2}',
                              OBA_BASEQTY='{3}', EMP_NAME='{4}',IN_STATION_TIME=SYSDATE WHERE MODEL_NAME= '{0}' ", 
                              InputModel, InputGroup, InputCheckUit, InputOBAQty, empName);
              
                result = Convert.ToInt32(DBHelper.ExecuteNonQuery(sql, null));
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
        public static string[] RunSfcFQAProc(string lineName, string section, string groupName, string stationName, string empNo, string pcName, string PKGType, string FQANo, string PKGData, string FQATYPE)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINENAME",OracleDbType.Varchar2,20),
                    new OracleParameter("SECTIONNAME",OracleDbType.Varchar2,20),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,20),
                    new OracleParameter("STATIONAME",OracleDbType.Varchar2,20),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,20),
                    new OracleParameter("PCNAME",OracleDbType.Varchar2,30),
                    new OracleParameter("PKGTYPE",OracleDbType.Varchar2,30),//--Carton Pallet
                    new OracleParameter("FQANO",OracleDbType.Varchar2,20),
                    new OracleParameter("PKGDATA",OracleDbType.Varchar2,20),//--CartonNo or Pallet No
                    new OracleParameter("FQATYPE",OracleDbType.Varchar2,20),//--Pass or Reject
                    new OracleParameter("DBSTATUS",OracleDbType.Varchar2,50)
                }; 
                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 10; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = lineName;
                parameter[1].Value = section;
                parameter[2].Value = groupName;
                parameter[3].Value = stationName;
                parameter[4].Value = empNo;
                parameter[5].Value = pcName;
                parameter[6].Value = PKGType;
                parameter[7].Value = FQANo;
                parameter[8].Value = PKGData;
                parameter[9].Value = FQATYPE; 

                result = DBHelper.RunProcedure("SFIS1.SFC_FQA", 1, parameter);
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
        //FQA Reject 把產品打回到指定工站,並清除相關數據
        public static string[] RunSfcFQARejectProc(string groupName,  string empNo, string pcName,  string FQANo)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,20),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,20),
                    new OracleParameter("PCNAME",OracleDbType.Varchar2,30),
                    new OracleParameter("FQANO",OracleDbType.Varchar2,20),
                    new OracleParameter("DBSTATUS",OracleDbType.Varchar2,50)
                };
                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 4; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = groupName;
                parameter[1].Value = empNo;
                parameter[2].Value = pcName;
                parameter[3].Value = FQANo;

                result = DBHelper.RunProcedure("SFIS1.SFC_FQA_REJECT", 1, parameter);
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
        //Get LotNo Fail
        public static string CheckRejectOBAByLotNo(string LotNo)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @" SELECT COUNT(LOT_NO) LOTQTY FROM SFISM4.R_CQC_REC_T WHERE LOT_NO=:LotNo AND QA_RESULT='1' ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":LotNo", LotNo)      
                };
                QTY = DBHelper.ExecuteSclar(sql, parameter).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return QTY;
        }
        //Get OBA Check SN List By PKGID
        public static DataTable GetRejectGroupBySN(string strSN)
        {
            try
            {
                string sql = " SELECT DISTINCT GROUP_NEXT FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE IN ( "
                           + " SELECT SPECIAL_ROUTE FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER=:strSN) "
                           + " AND STATE_FLAG='0' ORDER BY GROUP_NEXT ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":strSN", strSN)      
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
        //Get route Temp
        public static DataTable GetRouteInfoByRoutecode(string varRouteCode)
        {
            
            try
            {
                string sql = @" SELECT GROUP_NAME,GROUP_NEXT,STATE_FLAG,STEP_SEQUENCE FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE=:RouteCode ORDER BY STEP_SEQUENCE ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":RouteCode", varRouteCode)      
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
        //Get route Temp
        public static DataTable GetRouteInfoByMO(string varRouteCode)
        {

            try
            {
                string sql = @" SELECT GROUP_NAME,GROUP_NEXT ,STATE_FLAG,STEP_SEQUENCE FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE=:RouteCode AND STATE_FLAG='0' AND STEP_SEQUENCE<(SELECT MIN(STEP_SEQUENCE) FROM SFIS1.C_ROUTE_CONTROL_T WHERE ROUTE_CODE=:RouteCode AND STATE_FLAG='1') ORDER BY STEP_SEQUENCE ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":RouteCode", varRouteCode)      
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
        //根据流程代码和当前工站获取NextGroup
        public static DataTable GetNextGroupByRouteCodeMyGroup(string varRouteCode,string InMyGroup)
        {

            try
            {
                string sql = @" SELECT GROUP_NAME,GROUP_NEXT,STATE_FLAG,STEP_SEQUENCE FROM SFIS1.C_ROUTE_CONTROL_T 
                        WHERE ROUTE_CODE=:RouteCode AND GROUP_NAME = :MyGroup AND STATE_FLAG = '0' AND ROWNUM = 1 ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":RouteCode", varRouteCode),
                    new OracleParameter(":MyGroup", InMyGroup)
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
        public static DataTable GetMoBase(string varMONumber)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = " SELECT MODEL_NAME,VERSION_CODE,ROUTE_CODE FROM SFISM4.R_MO_BASE_T WHERE MO_NUMBER=:MONumber";
                OracleParameter parameter = new OracleParameter(":MONumber", varMONumber);
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
   }
}
