using System;
using System.Data;
using System.Management;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class CartonService
    {
        public static DataTable GetR107(string sn)
        {
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER =:SN OR SHIPPING_SN =:SN";
                OracleParameter parameter = new OracleParameter(":SN", sn);
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

        public static DataTable GetRePrintPackNo(string input, string packType)
        {
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE SERIAL_NUMBER =:INPUT OR SHIPPING_SN =:INPUT OR TRAY_NO =:INPUT ";

                if (packType == "CartonRePrint")
                {
                    sql = sql + " OR CARTON_NO =:INPUT OR MCARTON_NO =:INPUT ";
                }
                if (packType == "PalletRePrint")
                {
                    sql = sql + " OR CARTON_NO =:INPUT OR MCARTON_NO =:INPUT OR PALLET_NO =:INPUT OR IMEI =:INPUT ";
                }
                OracleParameter parameter = new OracleParameter(":INPUT", input);
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

        public static DataTable GetR107ByCarton(string carton)
        {
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE (CARTON_NO=:CARTON OR MCARTON_NO=:CARTON)";
                OracleParameter parameter = new OracleParameter(":CARTON", carton);
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

        public static DataTable GetNokiaTestDataByCartonAndStation(string carton,string station)
        {
            try
            {
                string sql = $@"select * from sfism4.r_nokia_test_data_t where serial_number in(select serial_number from sfism4.r_wip_tracking_t where carton_no='{carton}') and group_name='{station}' and status='PASS'";
                return DBHelper.GetDataTable(sql, null);
            }
            catch (Exception e)
            {

                throw e;
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
        }

        public static DataTable GetReelIDByCarton(string carton)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT DISTINCT MCARTON_NO, TRAY_NO 
                                               FROM SFISM4.R_WIP_TRACKING_T 
                                              WHERE CARTON_NO=:CARTON OR MCARTON_NO=:CARTON ");
                OracleParameter[] parameter = new OracleParameter[]
                {
                     new OracleParameter(":CARTON", carton)
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

        public static DataTable GetPackCarton(string carton)
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_PACK_CARTONNO_T WHERE CARTON_NO =:carton ";
                OracleParameter parameter = new OracleParameter(":carton", carton);
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

        public static string GetPcMacAddress()
        {
            string varResult = null;
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        varResult = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
            }
            catch
            {
                varResult = "NG_Mac_Addres獲取失敗,請重新開啟程式！";
            }
            return varResult;
        }

        public static string GetReprintDateCode(string carton)
        {
            try
            {
                string sql = @"SELECT TO_CHAR(IN_STATION_TIME,'YYYYWW') FROM SFISM4.R_PACK_CARTONNO_T WHERE CARTON_NO =:CARTON  OR  MCARTON_NO=:CARTON ";
                OracleParameter para = new OracleParameter(":CARTON", carton);
                return DBHelper.ExecuteSclar(sql, para).ToString();
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

        public static DataTable GetR107ByBox(string reelNo)
        {
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO=:BOXNO ";
                OracleParameter parameter = new OracleParameter(":BOXNO", reelNo);
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
        public static DataTable GetZ107ByBox(string reelNo)
        {
            try
            {
                string sql = @"SELECT * FROM SFISM4.Z_WIP_TRACKING_T WHERE TRAY_NO=:BOXNO ";
                OracleParameter parameter = new OracleParameter(":BOXNO", reelNo);
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

        public static DataTable GetPackBox(string reelNo)
        {
            try
            {
                string sql = @"SELECT * FROM SFIS1.C_PALLET_T WHERE PALLET_NO =:BOXNO ";
                OracleParameter parameter = new OracleParameter(":BOXNO", reelNo);
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

        /// <summary>
        /// 調用SFIS1.PACK_BOX,
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="stationName"></param>
        /// <param name="empNo"></param>
        /// <param name="pcName"></param>
        /// <param name="SN"></param>
        /// <param name="lineName"></param>
        /// <param name="section"></param>
        /// <param name="moNumber"></param>
        /// <param name="modelName"></param>
        /// <param name="moType"></param>
        /// <param name="cartonNo"></param>
        /// <param name="versionCode"></param>
        /// <param name="isFinish"></param>
        /// <returns></returns>
        public static string[] RunPackBoxProc(string lineName, string section, string groupName, string stationName, string empNo, string pcName, string SN, string moNumber = "NA", string moType = "NA", string modelName = "NA", string versionCode = "NA", string boxNo = "NA", string isFinish = "NA")
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,50),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,50),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,50),
                    new OracleParameter("STATIONAME",OracleDbType.Varchar2,50),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,50),
                    new OracleParameter("STATION_PCNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("SERIALNUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("MONUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("MOTYPE",OracleDbType.Varchar2,50),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("VERSIONCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("BOXNO",OracleDbType.Varchar2,50),
                    new OracleParameter("BOXFINISH",OracleDbType.Varchar2,50),
                    new OracleParameter("DBSERIALNUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("DBBOXNO",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMONUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMOTYPE",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMODELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("DBVERSIONCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("DBPACKPARAMQTY",OracleDbType.Varchar2,50),
                    new OracleParameter("DBBOXQTY",OracleDbType.Varchar2,50),
                    new OracleParameter("DBSTATUS",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < parameter.Length - 9; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 13; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = lineName;
                parameter[1].Value = section;
                parameter[2].Value = groupName;
                parameter[3].Value = stationName;
                parameter[4].Value = empNo;
                parameter[5].Value = pcName;
                parameter[6].Value = SN;
                parameter[7].Value = moNumber;
                parameter[8].Value = moType;
                parameter[9].Value = modelName;
                parameter[10].Value = versionCode;
                parameter[11].Value = boxNo;
                parameter[12].Value = isFinish;

                result = DBHelper.RunProcedure("SFIS1.PACK_BOX", 9, parameter);
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
        /// 調用SFIS1.PACK_CARTON,
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="stationName"></param>
        /// <param name="empNo"></param>
        /// <param name="pcName"></param>
        /// <param name="SN"></param>
        /// <param name="lineName"></param>
        /// <param name="section"></param>
        /// <param name="moNumber"></param>
        /// <param name="modelName"></param>
        /// <param name="moType"></param>
        /// <param name="cartonNo"></param>
        /// <param name="versionCode"></param>
        /// <param name="isFinish"></param>
        /// <returns></returns>
        public static string[] RunPackCartonProc(string lineName,string section,string groupName,string stationName,string empNo,string pcName,string SN,string isFinish="NA", string moNumber="NA",string modelName="NA",string moType="NA",string cartonNo="NA",string versionCode="NA")
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,50),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,50),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,50),
                    new OracleParameter("STATIONAME",OracleDbType.Varchar2,50),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,50),
                    new OracleParameter("STATION_PCNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("SERIALNUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("MONUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("MOTYPE",OracleDbType.Varchar2,50),
                    new OracleParameter("CARTONNO",OracleDbType.Varchar2,50),
                    new OracleParameter("VERSIONCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("CARTONFINISH",OracleDbType.Varchar2,50),
                    new OracleParameter("DBSERIALNUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("DBCARTONNO",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMONUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMODELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("DBVERSIONCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMOTYPE",OracleDbType.Varchar2,50),
                    new OracleParameter("DBPACKPARAMQTY",OracleDbType.Varchar2,50),
                    new OracleParameter("DBLABELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("DBCARTONQTY",OracleDbType.Varchar2,50),
                    new OracleParameter("DBSTATUS",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < parameter.Length - 10; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 13; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = lineName;
                parameter[1].Value = section;
                parameter[2].Value = groupName;
                parameter[3].Value = stationName;
                parameter[4].Value = empNo;
                parameter[5].Value = pcName;
                parameter[6].Value = SN;
                parameter[7].Value = moNumber;
                parameter[8].Value = modelName;
                parameter[9].Value = moType;
                parameter[10].Value = cartonNo;
                parameter[11].Value = versionCode;
                parameter[12].Value = isFinish;

                result = DBHelper.RunProcedure("SFIS1.PACK_CARTON", 10, parameter);
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
        
        //Get MCartonNo
        public static string[] RunPackGetMCartonNoProc(string lineName, string Custno, string modelName,  string versionCode)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,25),
                    new OracleParameter("CUSTNO",OracleDbType.Varchar2,25),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,25),
                    new OracleParameter("VERSIONCODE",OracleDbType.Varchar2,25),
                    new OracleParameter("MCARTONNO",OracleDbType.Varchar2,50),
                    new OracleParameter("RES_STATUS",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < parameter.Length - 2; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 4; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = lineName;
                parameter[1].Value = Custno;
                parameter[2].Value = modelName;
                parameter[3].Value = versionCode; 

                result = DBHelper.RunProcedure("SFIS1.GET_NEW_CARTONNO", 2, parameter);
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

        //Get IMEI
        public static string[] RunPackGetIMEIProc(string lineName, string Custno, string modelName, string versionCode)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,25),
                    new OracleParameter("CUSTNO",OracleDbType.Varchar2,25),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,25),
                    new OracleParameter("VERSIONCODE",OracleDbType.Varchar2,25),
                    new OracleParameter("PALLETNO",OracleDbType.Varchar2,50),
                    new OracleParameter("RES_STATUS",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < parameter.Length - 2; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 4; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = lineName;
                parameter[1].Value = Custno;
                parameter[2].Value = modelName;
                parameter[3].Value = versionCode;

                result = DBHelper.RunProcedure("SFIS1.GET_NEW_PALLETNO", 2, parameter);
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

        public static DataTable GetR107ByPallet(string pallet)
        {
            try
            {
                string sql = "SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI=:PALLET ORDER BY SERIAL_NUMBER";
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

        public static DataTable GetPackPallet(string pallet)
        {
            try
            {
                string sql = @"SELECT * FROM SFISM4.R_PACK_PACKUNITNO_T WHERE PACKUNIT_NO =:PALLET AND PACK_STATUS ='Open' ";
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

        /// <summary>
        /// 調用SFIS1.PACK_PALLET
        /// </summary>
        /// <param name="lineName"></param>
        /// <param name="section"></param>
        /// <param name="groupName"></param>
        /// <param name="stationName"></param>
        /// <param name="empNo"></param>
        /// <param name="pcName"></param>
        /// <param name="carton"></param>
        /// <param name="isFinish"></param>
        /// <param name="moNumber"></param>
        /// <param name="modelName"></param>
        /// <param name="moType"></param>
        /// <param name="palletno"></param>
        /// <param name="versionCode"></param>
        /// <returns></returns>
        public static string[] RunPackPalletProc(string lineName, string section, string groupName, string stationName, string empNo, string pcName, string carton, string isFinish = "NA", string moNumber = "NA", string modelName = "NA", string moType = "NA", string palletno = "NA", string versionCode = "NA")
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,50),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,50),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,50),
                    new OracleParameter("STATIONAME",OracleDbType.Varchar2,50),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,50),
                    new OracleParameter("STATION_PCNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("CARTONNO",OracleDbType.Varchar2,50),
                    new OracleParameter("MONUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("MOTYPE",OracleDbType.Varchar2,50),
                    new OracleParameter("PALLETNO",OracleDbType.Varchar2,50),
                    new OracleParameter("VERSIONCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("PALLETFINISH",OracleDbType.Varchar2,50),
                    new OracleParameter("DBCARTONNO",OracleDbType.Varchar2,50),
                    new OracleParameter("DBPALLETNO",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMONUMBER",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMODELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("DBVERSIONCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("DBMOTYPE",OracleDbType.Varchar2,50),
                    new OracleParameter("DBPACKPARAMQTY",OracleDbType.Varchar2,50),
                    new OracleParameter("DBLABELNAME",OracleDbType.Varchar2,50),
                    new OracleParameter("DBPALLETQTY",OracleDbType.Varchar2,50),
                    new OracleParameter("DBCARTONQTY",OracleDbType.Varchar2,50),
                    new OracleParameter("DBSTATUS",OracleDbType.Varchar2,100)
                };

                for (int i = 0; i < parameter.Length - 11; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 13; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = lineName;
                parameter[1].Value = section;
                parameter[2].Value = groupName;
                parameter[3].Value = stationName;
                parameter[4].Value = empNo;
                parameter[5].Value = pcName;
                parameter[6].Value = carton;
                parameter[7].Value = moNumber;
                parameter[8].Value = modelName;
                parameter[9].Value = moType;
                parameter[10].Value = palletno;
                parameter[11].Value = versionCode;
                parameter[12].Value = isFinish;

                result = DBHelper.RunProcedure("SFIS1.PACK_PALLET", 11, parameter);
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

        public static DataTable GetPackNoInfo(string stockType, string packNo)
        {
            DataTable dt = null;
            string sql;
            try
            {
                if (stockType == "CARTON")
                {
                     sql = @"SELECT MCARTON_NO PACKNO,STOCK_NO  FROM  SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO=:PACKNO AND ROWNUM=1";
                     OracleParameter[] parameter = new OracleParameter[]
                     {              
                         new OracleParameter(":PACKNO", packNo)
                     };
                     dt = DBHelper.GetDataTable(sql, parameter);
                }
                else if (stockType == "PALLET")
                {
                    sql = @"SELECT IMEI PACKNO, STOCK_NO  FROM  SFISM4.R_WIP_TRACKING_T WHERE  IMEI=:PACKNO  AND ROWNUM=1";
                   OracleParameter[] parameter = new OracleParameter[]
                   {              
                       new OracleParameter(":PACKNO", packNo)
                   };
                   dt = DBHelper.GetDataTable(sql, parameter);
                }
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

        public static DataTable GetStockNoQty(string stockNo)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) QTY FROM  SFISM4.R_WIP_TRACKING_T WHERE  STOCK_NO = :STOCKNO ";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":STOCKNO", stockNo)      
                };
                dt = DBHelper.GetDataTable(sql, parameter);
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

        public static string GetCartonQty(string carton)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) FROM SFISM4.R107 WHERE MCARTON_NO=:CARTON";
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":CARTON", carton)      
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
        
    }
}
