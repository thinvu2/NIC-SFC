using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class WarehouseService
    {
        public static DataTable GetZ107SNByCarton(string CartonNo)
        {
            try
            {
                string sql = " SELECT MODEL_NAME,VERSION_CODE,SERIAL_NUMBER,'1' QTY FROM SFISM4.Z_WIP_TRACKING_T "
                + " WHERE MCARTON_NO =:CartonNo ORDER BY SERIAL_NUMBER ";
                OracleParameter parameter = new OracleParameter(":CartonNo", CartonNo);
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

        public static DataTable GetZ107CartonByPallet(string PalletNo)
        {
            try
            {
                string sql = " SELECT MODEL_NAME,VERSION_CODE,MCARTON_NO,COUNT(SERIAL_NUMBER) QTY FROM SFISM4.Z_WIP_TRACKING_T "
                + " WHERE IMEI =:PalletNo GROUP BY MODEL_NAME,VERSION_CODE,MCARTON_NO ORDER BY MCARTON_NO ";
                OracleParameter parameter = new OracleParameter(":PalletNo", PalletNo);
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

        public static int UpdateZ107CartonBySN(string CartonNo, string SerialNumber)
        {
            int result = 0;
            try
            {
                string sql = "UPDATE SFISM4.Z_WIP_TRACKING_T SET MCARTON_NO=:CartonNo,CARTON_NO=:CartonNo,IN_STATION_TIME=SYSDATE WHERE SERIAL_NUMBER=:SerialNumber AND ROWNUM=1 ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":CartonNo", CartonNo),
                    new OracleParameter(":SerialNumber", SerialNumber)
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

        public static int UpdateZ107PalletByCarton(string PalletNo, string CartonNo)
        {
            int result = 0;
            try
            {
                string sql = "UPDATE SFISM4.Z_WIP_TRACKING_T SET IMEI=:PalletNo,PALLET_NO=:PalletNo,IN_STATION_TIME=SYSDATE WHERE MCARTON_NO=:CartonNo ";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":PalletNo", PalletNo),
                    new OracleParameter(":CartonNo", CartonNo)
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
        //比较特殊,PM and QA 应客户要求从产线直接拿产品快递给客户,出货资料需要
        public static int UpdateZ107ShipNoByTray(string InTrayNo, string InShipNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE SFISM4.Z_WIP_TRACKING_T SET SHIP_NO='{1}' ,WIP_GROUP='SHIPPING',IN_STATION_TIME=SYSDATE,
                        IMEI='PEFGNodefin',MCARTON_NO='VPFNodefine',OUT_LINE_TIME=SYSDATE WHERE TRAY_NO='{0}'  ", InTrayNo, InShipNo); 
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

        public static int InsertZ107FromR107ByTray(string InTrayNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.Z_WIP_TRACKING_T SELECT * FROM SFISM4.R_WIP_TRACKING_T WHERE TRAY_NO='{0}' ", InTrayNo);
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

        public static DataTable GetRePrintPackNo(string ScanData, string packType)
        {
            try
            {
                string sql = @"SELECT A.*,TO_CHAR(A.IN_STATION_TIME,'YYYYWW') DATE_CODE 
                                 FROM SFISM4.Z_WIP_TRACKING_T A 
                                WHERE A.SERIAL_NUMBER =:ScanData OR A.SHIPPING_SN =:ScanData OR A.MCARTON_NO =:ScanData";

                if (packType == "PalletRePrint")
                {
                    sql = sql + " OR A.IMEI =:ScanData ";
                }

                OracleParameter parameter = new OracleParameter(":ScanData", ScanData);
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
        
        //Get CartonQty
        public static string GetCartonQty(string CartonNo)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @"SELECT COUNT(SERIAL_NUMBER) CartonQty FROM SFISM4.Z_WIP_TRACKING_T WHERE MCARTON_NO=:CartonNo";
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
                string sql = @"SELECT COUNT(SERIAL_NUMBER) PalletSNQty FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI=:PalletNo";
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

        //Get PalletCartonQty
        public static string GetPalletCartonQty(string PalletNo)
        {
            string QTY = string.Empty;
            try
            {
                string sql = @"SELECT COUNT(distinct MCARTON_NO) PalletCartonQty FROM SFISM4.Z_WIP_TRACKING_T WHERE IMEI=:PalletNo";
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

        public static DataTable GetNoShipDNInfoByDN()
        {
            try
            {
                string sql = " SELECT DN_NO,DN_ITEM_NO,TO_NO,MODEL_NAME,SHIPPING_QTY SHIPQTY,SO_NUMBER,CUST_PO,SHIP_CODE, "
                           + " CUSTOMER_CODE,MODEL_DESC,CUST_NAME,SHIP_ADDRESS,COUNTRY,HUB_FLAG FROM SFISM4.R_SAP_DN_DETAIL_T "
                           + " WHERE SHOW_FLAG='Y' ORDER BY DN_NO,DN_ITEM_NO ";
                 
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

        public static DataTable GetNoShipDNInfoByDN(string InputDN, string InputDNItem)
        {
            try
            {
                string sql = " SELECT DN_NO,DN_ITEM_NO,TO_NO,MODEL_NAME,SHIPPING_QTY SHIPQTY,SO_NUMBER,CUST_PO,SHIP_CODE, "
                           + " CUSTOMER_CODE,MODEL_DESC,CUST_NAME,SHIP_ADDRESS,COUNTRY,HUB_FLAG FROM SFISM4.R_SAP_DN_DETAIL_T "
                           + " WHERE DN_NO=:InputDN AND DN_ITEM_NO=:InputDNItem AND ROWNUM=1 ";         
                 
                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":InputDN", InputDN), 
                    new OracleParameter(":InputDNItem", InputDNItem)     
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

        public static int InsertDNInfoToBPCSInvoice(string InputDNItem, string InputDN, string InputModelName, string InputDNQty, string InputTcom, string InpuCustPo)
        {
            int result = 0;
            try
            {
                string sql =string.Format(@" INSERT INTO SFISM4.R_BPCS_INVOICE_T (INV_NO,INVOICE,SO_NUMBER,SO_LINE,MODEL_NAME,SO_QTY,SHIPPING_QTY,FINISH_DATE,EMP_NO,TCOM,CUST_PO) "
                            + " VALUES('{0}','{1}','{1}','{0}','{2}','{3}','{3}',SYSDATE,'SFIS','{4}','{5}') ", InputDNItem,InputDN, InputModelName, InputDNQty, InputTcom, InpuCustPo);

                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":InputDN", InputDN),
                    new OracleParameter(":InputDNItem", InputDNItem)
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

        public static int InsertDNInfoToBPCSInvoiceByDN(string InputDN, string InputDNItem)
        {
            int result = 0;
            try
            {
                string sql = " INSERT INTO SFISM4.R_BPCS_INVOICE_T (INV_NO,INVOICE,SO_NUMBER,SO_LINE,SO_QTY, "
                            +" UNIT_NW,UNIT_GW,DIMEN,CUST_PO, MODEL_NAME,CUST_NAME,CUST_ID,MO_NO ) "
                            +" SELECT DN_ITEM_NO INV_NO,DN_NO INVOICE,SO_NUMBER,SO_LINE,SHIPPING_QTY SO_QTY, "
                            +" '0' UNIT_NW,'0' UNIT_GW,'10X10X10' DIMEN,CUST_PO,MODEL_NAME,CUST_NAME,CUST_ID,'N/A' MO_NO "
                            +" FROM SFISM4.R_SAP_DN_DETAIL_T "
                            +" WHERE DN_NO=:InputDN AND DN_ITEM_NO=:InputDNItem AND ROWNUM=1 ";

                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":InputDN", InputDN), 
                    new OracleParameter(":InputDNItem", InputDNItem)     
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

        public static int UpdateDNInfoAtSAPDNByDN(string InputDN, string InputDNItem)
        {
            int result = 0;
            try
            {
                string sql = " UPDATE SFISM4.R_SAP_DN_DETAIL_T SET SHOW_FLAG='N' "
                           + " WHERE DN_NO=:InputDN AND DN_ITEM_NO=:InputDNItem ";

                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":InputDN", InputDN), 
                    new OracleParameter(":InputDNItem", InputDNItem)     
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

        public static DataTable QueryDNInfoAtBPCSInvoiceByDN(string InputDN, string InputDNItem)
        {
            try
            {
                string sql = " SELECT INV_NO,INVOICE,SO_NUMBER,SO_LINE,SO_QTY,CUST_PO,MODEL_NAME,CUST_NAME, FINISH_DATE,TCOM "
                            + " FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE=:InputDN and INV_NO=:InputDNItem AND ROWNUM=1 ";

                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":InputDN", InputDN), 
                    new OracleParameter(":InputDNItem", InputDNItem)      
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
        
        //處理BTS DN
        //Query BTS DN 是否有Ship Data
        public static DataTable QueryBTSDNInfoByDN(string InputDN)
        {
            try
            {
                string sql = " SELECT COUNT(P_SN) DNQTY,P_NO FROM SFCRUNTIME.R_SHIPPING_DETAIL@A6BTSSFC.WORLD  "
                            + " WHERE  DN_NO=:InputDN  GROUP BY P_NO ";

                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":InputDN", InputDN)       
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
        
        //Query BTS DN 對應的數據是否已經在CPE
        public static DataTable QueryBTSDNInfoAtCPEByDN(string InputDN)
        {
            try
            {
                string sql = " SELECT COUNT(SERIAL_NUMBER) DNQTY FROM SFISM4.Z_WIP_TRACKING_T WHERE SERIAL_NUMBER IN ( "
                          + " SELECT P_SN FROM SFCRUNTIME.R_SHIPPING_DETAIL@A6BTSSFC.WORLD WHERE  DN_NO=:InputDN ) ";

                OracleParameter[] parameter = new OracleParameter[]
                {              
                    new OracleParameter(":InputDN", InputDN)       
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

        public static int InsertDNInfoToCPEFromBTSByBTSDN(string InputDN)
        {
            int result = 0;//Insert OK,Result=Inset Rownum,Else=0
            try
            {
                string sql = string.Format(@" INSERT INTO SFISM4.Z_WIP_TRACKING_T(SERIAL_NUMBER,MO_NUMBER,MODEL_NAME,VERSION_CODE, 
                             CUSTOMER_NO,LINE_NAME,SECTION_NAME,GROUP_NAME,STATION_NAME,ERROR_FLAG,IN_STATION_TIME,   
                             SHIPPING_SN,PALLET_NO,SCRAP_FLAG,KEY_PART_NO,CARTON_NO,WARRANTY_DATE,EMP_NO,PALLET_FULL_FLAG, 
                             ATE_STATION_NO,IMEI,MCARTON_NO,SO_NUMBER,SO_LINE,STOCK_NO,WIP_GROUP) 
                             SELECT SYSSERIALNO SERIAL_NUMBER,WORKORDERNO MO_NUMBER,P_NO MODEL_NAME,'00' VERSION_CODE,'80190' CUSTOMER_NO,  
                             PRODUCTIONLINE LINE_NAME,'TEST' SECTION_NAME,'STOCKIN' GROUP_NAME,'BTS' STATION_NAME, '0' ERROF_FLAG, 
                             SHIPDATE IN_STATION_TIME,SYSSERIALNO SHIPPING_SN,PALLET_NO,'0' SCRAP_FLAG,P_NO KEY_PART_NO,CARTON_NO, 
                             SHIPDATE WARRANTY_DATE,'BTS' EMP_NO,'Y' PALLET_FULL_FLAG,'BTS' ATE_STATION_NO,PALLET_NO IMEI,CARTON_NO MCARTON_NO, 
                             'N/A' SO_NUMBER,'N/A' SO_LINE,DN_NO STOCK_NO ,'F101' WIP_GROUP FROM  
                             (SELECT SYSSERIALNO,WORKORDERNO,SHIPDATE ,PRODUCTIONLINE  FROM SFCRUNTIME.MFWORKSTATUS@A6BTSSFC.WORLD WHERE SYSSERIALNO IN ( 
                             SELECT P_SN FROM SFCRUNTIME.R_SHIPPING_DETAIL@A6BTSSFC.WORLD WHERE  DN_NO='{0}' ) )A, 
                             (SELECT DN_NO,P_NO,P_SN,'3S'||CARTON_NO CARTON_NO,PALLET_NO FROM SFCRUNTIME.R_SHIPPING_DETAIL@A6BTSSFC.WORLD WHERE  DN_NO='{0}' ) B 
                             WHERE A.SYSSERIALNO=B.P_SN ORDER BY SYSSERIALNO ", InputDN);
                 
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
        
        //Insert Reprint Log
        public static int InsertRePrintLog(string EMPNo,string APName,  string FunctionName, string ActionDesc)
        {
            int result = 0;
            
            try
            {
                string sql = string.Format(@"INSERT INTO SFISM4.R_SYSTEM_LOG_T (EMP_NO,PRG_NAME,ACTION_TYPE,ACTION_DESC,TIME )  
                             VALUES('{0}','{1}','{2}','{3}',SYSDATE)", EMPNo, APName,FunctionName,ActionDesc) ;
                 

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

        //Query ModelName List
        public static DataTable QueryModelNameList()
        {
            try
            {
                string sql = " SELECT DISTINCT TRIM(UPPER(MODEL_NAME)) MODEL_NAME FROM SFIS1.C_MODEL_DESC_T ORDER BY MODEL_NAME ";

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
        
        //Query Inventory By Model
        public static DataTable GetModelDataByModelName(string Model)
        {
            DataTable dt = null;
            try
            {
                string sql = @"SELECT A.MODEL_NAME,COUNT(A.SERIAL_NUMBER) QTY
                              FROM SFISM4.Z_WIP_TRACKING_T A WHERE A.WIP_GROUP != 'SHIPPING' ";
                if (!Model.Equals("ALL"))
                {
                    sql += string.Format("AND A.MODEL_NAME='{0}' ", Model);
                }

                sql += "Group by A.MODEL_NAME Order by A.MODEL_NAME ";
                dt = DBHelper.GetDataTable(sql);
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
        
        //Query Inventory By Date
        public static DataTable GetModelDataByDate(string BeginDate, string EndDate)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT A.MODEL_NAME, COUNT(A.SERIAL_NUMBER) QTY
                              FROM SFISM4.Z_WIP_TRACKING_T A WHERE A.WIP_GROUP != 'SHIPPING'  AND A.IN_STATION_TIME 
                              BETWEEN to_date('{0}','YYYY/MM/DD') AND to_date('{1}','YYYY/MM/DD')
                              Group by A.MODEL_NAME Order by A.MODEL_NAME ", BeginDate, EndDate);
                dt = DBHelper.GetDataTable(sql);
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
        
        //Query Inventory Carton By Model/Version
        public static DataTable GetModelCartonData(string Model)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT A.MODEL_NAME, A.MCARTON_NO,COUNT(A.SERIAL_NUMBER) QTY
                              FROM SFISM4.Z_WIP_TRACKING_T A WHERE A.WIP_GROUP != 'SHIPPING' AND A.MODEL_NAME='{0}' 
                              Group by A.MODEL_NAME,A.MCARTON_NO 
                              ORDER BY A.MCARTON_NO ",Model);
                dt = DBHelper.GetDataTable(sql);
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
        
        //Query Inventory Carton By Model/Version/Date
        public static DataTable GetModelCartonDataAddDate(string Model,string BeginDate, string EndDate)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT A.MODEL_NAME, A.MCARTON_NO,COUNT(A.SERIAL_NUMBER) QTY
                              FROM SFISM4.Z_WIP_TRACKING_T A WHERE A.WIP_GROUP != 'SHIPPING' AND A.MODEL_NAME='{0}' 
                              AND A.IN_STATION_TIME BETWEEN to_date('{1}','YYYY/MM/DD') AND to_date('{2}','YYYY/MM/DD')
                              Group by A.MODEL_NAME,A.MCARTON_NO ORDER BY A.MCARTON_NO ", Model,BeginDate, EndDate);
                dt = DBHelper.GetDataTable(sql);
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
        
        //Stock
        /// <summary>
        /// 調用SP SFIS1.SFC_STOCKIN
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="stationName"></param>
        /// <param name="empNo"></param>
        /// <param name="pcName"></param>
        /// <param name="stockType"></param>
        /// <param name="stockNo"></param>
        /// <param name="stockData"></param>
        /// <param name="isFinish"></param>
        /// <param name="lineName"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string[] RunSfcStockInProc(string lineName, string section, string groupName, string stationName, string empNo, string pcName, string stockType, string stockNo, string stockData, string isFinish = "NA")
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINE",OracleDbType.Varchar2,20),
                    new OracleParameter("SECTION",OracleDbType.Varchar2,20),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,20),
                    new OracleParameter("STATIONAME",OracleDbType.Varchar2,20),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,20),
                    new OracleParameter("STATION_PCNAME",OracleDbType.Varchar2,30),
                    new OracleParameter("STOCKTYPE",OracleDbType.Varchar2,30),
                    new OracleParameter("STOCKNO",OracleDbType.Varchar2,20),
                    new OracleParameter("STOCKDATA",OracleDbType.Varchar2,20),
                    new OracleParameter("STOCKFINISH",OracleDbType.Varchar2,20),
                    new OracleParameter("DBCARTONNO",OracleDbType.Varchar2,20),
                    new OracleParameter("DBMODELNAME",OracleDbType.Varchar2,20),
                    new OracleParameter("DBVERSIONCODE",OracleDbType.Varchar2,30),
                    new OracleParameter("DBCARTONQTY",OracleDbType.Varchar2,20),
                    new OracleParameter("DBSTOCKNO",OracleDbType.Varchar2,20),
                    new OracleParameter("DBSTATUS",OracleDbType.Varchar2,50)
                };

                for (int i = 0; i < parameter.Length - 6; i++)
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
                parameter[6].Value = stockType;
                parameter[7].Value = stockNo;
                parameter[8].Value = stockData;
                parameter[9].Value = isFinish;

                result = DBHelper.RunProcedure("SFIS1.SFC_STOCKIN", 6, parameter);
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

        public static string[] RunSFCShipmentProc(string linename, string groupname, string stationname, string pcname, string empNo, string scantype, string dnno, string modelname, string shipno, string scandata, string finishtype)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("LINENAME",OracleDbType.Varchar2,20),
                    new OracleParameter("MYGROUP",OracleDbType.Varchar2,20),
                    new OracleParameter("STATIONAME",OracleDbType.Varchar2,20),
                    new OracleParameter("PCNAME",OracleDbType.Varchar2,20),
                    new OracleParameter("EMPNO",OracleDbType.Varchar2,30),
                    new OracleParameter("SCANTYPE",OracleDbType.Varchar2,30),
                    new OracleParameter("DNNO",OracleDbType.Varchar2,20),
                    new OracleParameter("MODELNAME",OracleDbType.Varchar2,20),
                    new OracleParameter("SHIPNO",OracleDbType.Varchar2,20),
                    new OracleParameter("SCANDATA",OracleDbType.Varchar2,20),
                    new OracleParameter("FINISHTYPE",OracleDbType.Varchar2,20),
                    new OracleParameter("DBPACKNO",OracleDbType.Varchar2,20),
                    new OracleParameter("DBMODELNAME",OracleDbType.Varchar2,20),
                    new OracleParameter("DBVERSIONCODE",OracleDbType.Varchar2,30),
                    new OracleParameter("DBPACKQTY",OracleDbType.Varchar2,20),
                    new OracleParameter("DBSHIPQTY",OracleDbType.Varchar2,20),                    
                    new OracleParameter("DBSHIPNO",OracleDbType.Varchar2,20),
                    new OracleParameter("DBSTATUS",OracleDbType.Varchar2,80)
                };

                for (int i = 0; i < parameter.Length - 7; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 11; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = linename;
                parameter[1].Value = groupname;
                parameter[2].Value = stationname;
                parameter[3].Value = pcname;
                parameter[4].Value = empNo;
                parameter[5].Value = scantype;
                parameter[6].Value = dnno;
                parameter[7].Value = modelname;
                parameter[8].Value = shipno;
                parameter[9].Value = scandata;
                parameter[10].Value = finishtype;

                result = DBHelper.RunProcedure("SFIS1.SFC_SHIPMENT", 7, parameter);
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
   }
}
