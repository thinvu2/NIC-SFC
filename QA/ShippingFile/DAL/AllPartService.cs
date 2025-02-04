using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class AllPartService
    {
        //SMT相关的SQL
        //MES1.GET_TRCODE ,返回两个参数0是v_trcode,1:Res提示信息 
        /*
         传进5个参数,传出2个参数
        真正有用的我认为就三个TRSN WO,MAC,但为什么是MAC,而不是主机名或IP呢?
        并且提示,还不够清楚明了
        用了多个CURSOR
        1.用Data1= 'SMTLOADING'查mes4.r_ap_temp是否有值
        如果没有,则赋给Sequence为1,插入到mes4.r_ap_temp(data1, data2, data3)
        values('SMTLOADING',MAC,'1')
        2.用Data1= 'SMTLOADING' 和 data2 = mac_address查mes4.r_ap_temp是否有值
        如果没有,则查系统中的data3最大值+1赋给Sequence,插入到mes4.r_ap_temp(data1, data2, data3)
        values('SMTLOADING',MAC,Sequence)
        Station= 'LOADING' ||Sequence;
        TraceCode= 'S0SMT'||Sequence||TO_CHAR (SYSDATE,'YYMMDDHH24MISS');
        3.SELECT location_flag  FROM mes4.r_tr_sn WHERE tr_sn 
        如果查不到,肯定报异常,如果空也异常,如果不是2,则报不在产线,可能KIT没有Checkout
        4.select *FROM mes4.r_tr_sn_wip WHERE tr_sn
         如果查不到数据或料号为空,报异常:可能KIT没有Checkout或用玩了
        5.SELECT p_no, p_version FROM mes4.r_wo_base WHERE wo = g_wo 
        如果机种料号为空,报异常:工单不存在
        Insert mes4.r_tr_code_list 
        mes4.r_station_wip 
        mes4.r_tr_code_detail 
        6.用tr_sn=TRSN查mes4.r_kp_list是否有值
        如果查不到数据则插入mes4.r_kp_list
        */
        //AllPartDBHelper AllPartsDB = new AllPartDBHelper("SHCPEAPS");
        public static string[] RunAPSGetTraceCode(string InTRSN, string InMO, string InPCMacid, string InEMPNO, string InBoardSide)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("G_TRSN",OracleDbType.Varchar2,25),
                    new OracleParameter("G_WO",OracleDbType.Varchar2,20),
                    new OracleParameter("MAC_ADDRESS",OracleDbType.Varchar2,25),
                    new OracleParameter("G_EMP_NO",OracleDbType.Varchar2,20),
                    new OracleParameter("G_PROCESS",OracleDbType.Varchar2,20),
                    new OracleParameter("V_TRCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("RES",OracleDbType.Varchar2,200)
                };

                for (int i = 0; i < parameter.Length - 5; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 5; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = InTRSN;
                parameter[1].Value = InMO;
                parameter[2].Value = InPCMacid;
                parameter[3].Value = InEMPNO;
                parameter[4].Value = InBoardSide;

                result = AllPartDBHelper.RunProcedure("MES1.GET_TRCODE", 2, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return result;
        }

        //MES1.Z_INSERT_PANEL_SNLINK ,返回两个参数0是v_ext_qty TRSN剩余数,1:Res提示信息 
        public static string[] RunAPSGetTRSNExtQty(string InTRSN, string InMO, string InPCMacid, string InEMPNO, string InTraceCode, string InPanelNo, int InLinkQty, string InBoardSide)
        {
            string[] result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("G_TRSN",OracleDbType.Varchar2,25),
                    new OracleParameter("G_WO",OracleDbType.Varchar2,20),
                    new OracleParameter("MAC_ADDRESS",OracleDbType.Varchar2,25),
                    new OracleParameter("G_EMP_NO",OracleDbType.Varchar2,20),
                    new OracleParameter("G_TRCODE",OracleDbType.Varchar2,50),
                    new OracleParameter("G_PANELNO",OracleDbType.Varchar2,50),
                    new OracleParameter("G_LINK_QTY",OracleDbType.Int32,20),
                    new OracleParameter("G_PROCESS",OracleDbType.Varchar2,20),
                    new OracleParameter("V_EXT_QTY",OracleDbType.Varchar2,20),
                    new OracleParameter("RES",OracleDbType.Varchar2,200)
                };

                for (int i = 0; i < parameter.Length - 8; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 8; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = InTRSN;
                parameter[1].Value = InMO;
                parameter[2].Value = InPCMacid;
                parameter[3].Value = InEMPNO;
                parameter[4].Value = InTraceCode;
                parameter[5].Value = InPanelNo;
                parameter[6].Value = InLinkQty;
                parameter[7].Value = InBoardSide;

                result = AllPartDBHelper.RunProcedure("MES1.INSERT_PANEL_SNLINK", 2, parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return result;
        }
        //MES1.CHECK_WO_MATERIAL_PROCESS这个SP应该是有些问题的
        //SUBSTR (PROGRAM_NAME, INSTR (PROGRAM_NAME, '-') - 1, 1) 這個取面別的方法不對
        //例如:ELS61-E2-RMT-S9G1A,
        //應該 SUBSTR (PROGRAM_NAME,INSTR(PROGRAM_NAME,'-' || SUBSTR (STATION, 4, 2) || SUBSTR(STATION, 7, 2))- 1,1)
        //PROGRAM_NAME的定義是:機種名稱+面別+'-'+機台編號的SUBSTR(STATION,4,2)||SUBSTR(STATION,7,2)
        public static string RunAPSCheckWOMaterialProcess(string InAPSLine, string InMO, string InBoardSide)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("G_LINE",OracleDbType.Varchar2,25),
                    new OracleParameter("G_WO",OracleDbType.Varchar2,20),
                    new OracleParameter("G_PROCESS",OracleDbType.Varchar2,25),
                    new OracleParameter("RES",OracleDbType.Varchar2,200)
                };

                for (int i = 0; i < parameter.Length - 3; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 3; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = InAPSLine;
                parameter[1].Value = InMO;
                parameter[2].Value = InBoardSide;

                result = AllPartDBHelper.RunProcedure("MES1.CHECK_WO_MATERIAL", parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return result;
        }
        //MES1.CHECK_PSN_MATERIAL检查并补料
        public static string RunAPSCheckPSNMaterial(string InPanelNo, string InMO, string InBoardSide, string InAPSLine)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("G_PSN",OracleDbType.Varchar2,25),
                    new OracleParameter("G_WO",OracleDbType.Varchar2,20),
                    new OracleParameter("G_PROCESS",OracleDbType.Varchar2,6),
                    new OracleParameter("G_LINE",OracleDbType.Varchar2,20),
                    new OracleParameter("RES",OracleDbType.Varchar2,200)
                };

                for (int i = 0; i < parameter.Length - 4; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 4; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = InPanelNo;
                parameter[1].Value = InMO;
                parameter[2].Value = InBoardSide;
                parameter[3].Value = InAPSLine;

                result = AllPartDBHelper.RunProcedure("MES1.CHECK_PSN_MATERIAL", parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return result;
        }
        //MES1.CHECK_TRSN_KITTING
        //G_MOVETYPE: CHECK-IN CHECK-OUT TO-WHS TO-MRB SCRAP BUFFER-WO RETURN MODIFY WO-WO WO-KITTING ADJUST-CHECK ADJUST
        //G_FMLOCATION:WHS,KITTING,工单
        //G_TOLOCATION:WHS,KITTING,工单,LINE,SETUP
        //G_REASON: a:001 b:003 c:999 d:997 e:996
        /*  WHS-->KITTING  a:001
            KITTING-->WO b:003
            WO-->LINE c:999
            WO-->SETUP d:997
            WO-->KITTING e:996*/
        public static string RunAPSCheckTRSNKitting(string InTRSN, string InMoveType, string InFromLocation, string InToLocation, string InReason, string InEMPNo)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter("G_TRSN",OracleDbType.Varchar2,25),
                    new OracleParameter("G_MOVETYPE",OracleDbType.Varchar2,20),
                    new OracleParameter("G_FMLOCATION",OracleDbType.Varchar2,20),
                    new OracleParameter("G_TOLOCATION",OracleDbType.Varchar2,20),
                    new OracleParameter("G_REASON",OracleDbType.Varchar2,20),
                    new OracleParameter("G_EMP",OracleDbType.Varchar2,20),
                    new OracleParameter("RES",OracleDbType.Varchar2,200)
                };

                for (int i = 0; i < parameter.Length - 6; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                for (int i = 6; i < parameter.Length; i++)
                {
                    parameter[i].Direction = ParameterDirection.Output;
                }

                parameter[0].Value = InTRSN;
                parameter[1].Value = InMoveType;
                parameter[2].Value = InFromLocation;
                parameter[3].Value = InToLocation;
                parameter[4].Value = InReason;
                parameter[5].Value = InEMPNo;

                result = AllPartDBHelper.RunProcedure("MES1.CHECK_TRSN_KITTING", parameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return result;
        }
        //获取R_WO_BASE的信息
        public static DataTable GetAllPartsMOInfoByMO(string InMo)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM MES4.R_WO_BASE WHERE WO='{0}' AND ROWNUM=1 ", InMo);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //EXT_QTY 是剩余数
        public static DataTable GetTRSNExtQtyByWoTRSN(string InMo, string InTRSN)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM MES4.R_TR_SN_WIP WHERE WO='{0}' AND TR_SN = '{1}' AND ROWNUM=1 ", InMo, InTRSN);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        public static DataTable GetTRSNWIPInfoByMOStation(string InMo)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM MES4.R_TR_SN_WIP WHERE WO='{0}' AND STATION LIKE '%AP1' AND ROWNUM=1 ", InMo);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //SELECT* FROM mes1.c_kp_time_control where cust_kp_no='W30960Q4400A7' 
        public static DataTable GetPNControlTimeByKPNO(string InKPNO)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM MES1.C_KP_TIME_CONTROL WHERE CUST_KP_NO='{0}' AND ROWNUM=1 ", InKPNO);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //(INSERT INTO MES4.R_TR_SN_DETAIL(TR_SN, CUST_KP_NO, MFR_KP_NO, MFR_CODE, DATE_CODE, LOT_CODE, QTY, EXT_QTY, LOCATION_FLAG, 
        //WORK_FLAG,WO,STATION,MFG_EMP_NO, WORK_TIME, EMP_NO,REMARK) VALUES('K19071800350', '202.00004.015', 'GRM155R71C103KA01D',
        //'YE00000681', '90709', 'WG9620AHK', '10000', '6322', '2', '1', '005100079652', 'A12SEAH2', 'Z3011', SYSDATE, 'Z3011', '')
        // )
        public static int InsertTRSNDetailInfo(string InTRSN, string InPN, string InMFRPN, string InMFRCode, string InDateCode, 
            string InLotCode,string InQty, string InExtQty, string InLocationFlag, string InWorkFlag, string InWO, 
            string InStation, string InUserID, string InDocNo)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_TR_SN_DETAIL(TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE, 
                                             QTY,EXT_QTY,LOCATION_FLAG,WORK_FLAG,WO,STATION,MFG_EMP_NO,WORK_TIME,EMP_NO,REMARK,DOC_NO)  
                                          VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',SYSDATE,'{12}','','{13}')",
                                          InTRSN, InPN, InMFRPN, InMFRCode, InDateCode,InLotCode,InQty,InExtQty,InLocationFlag,InWorkFlag,InWO,InStation,InUserID,InDocNo);
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
        //插入MES4.R_TR_SN_CHANGE
        /*INSERT INTO MES4.R_TR_SN_CHANGE(TR_SN, DOC_NO, DOC_FLAG, CUST_KP_NO, MFR_KP_NO, MFR_CODE, DATE_CODE, LOT_CODE, QTY, EXT_QTY, 
        LOCATION_FLAG, WORK_FLAG, START_TIME, END_TIME, CHANGE_FLAG, WORK_TIME, EMP_NO) VALUES('K19071800350','5025408542','0',
        '202.00004.015','GRM155R71C103KA01D','YE00000681','90709','WG9620AHK','10000','7837','1','2', TO_DATE('20190718095323',
        'YYYYMMDDHH24MISS'),TO_DATE('20200505101012','YYYYMMDDHH24MISS'),'0',SYSDATE,'Z3011' )*/
        public static int InsertTRSNChangeInfo(string InTRSN, string InPN, string InMFRPN, string InMFRCode, string InDateCode,
            string InLotCode, string InQty, string InExtQty, string InLocationFlag, string InWorkFlag, string InStartTime, string InUserID, string InDocNo)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_TR_SN_CHANGE(TR_SN,DOC_NO,DOC_FLAG,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,
                                          LOT_CODE,QTY,EXT_QTY,LOCATION_FLAG,WORK_FLAG,START_TIME,END_TIME, CHANGE_FLAG, WORK_TIME, EMP_NO)  
                                          VALUES('{0}','{1}','0','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{1}',TO_DATE('{11}','YYYYMMDDHH24MISS'),SYSDATE,'0',SYSDATE,'{12}')",
                                          InTRSN, InDocNo, InPN, InMFRPN, InMFRCode, InDateCode, InLotCode, InQty, InExtQty, InLocationFlag, InWorkFlag, InStartTime, InUserID);
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
        //插入MES4.R_KITTING_SCAN_DETAIL
        /*INSERT INTO MES4.R_KITTING_SCAN_DETAIL(TR_SN, CUST_KP_NO, QTY, FROM_LOCATION, TO_LOCATION, MOVE_TYPE, MOVE_REASON, MOVE_EMP,
         MOVE_TIME) VALUES('K19071800350', '202.00004.015', 6322, '005100079652', 'KITTING', 'e', '996', 'Z3011', SYSDATE)*/
        public static int InsertKittingScanDetailInfo(string InTRSN, string InPN, string InExtQty, string InFromLocation, 
                                            string InToLocation, string InMoveType, string InMoveReason, string InUserID)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_KITTING_SCAN_DETAIL(TR_SN,CUST_KP_NO,QTY,FROM_LOCATION,TO_LOCATION,MOVE_TYPE,MOVE_REASON,MOVE_EMP,MOVE_TIME)  
                                          VALUES('{0}','{1}','0','{2}','{3}','{4}','{5}','{6}','{7}',SYSDATE)",
                                          InTRSN, InPN, InExtQty, InFromLocation, InToLocation, InMoveType, InMoveReason, InUserID);
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
        public static DataTable GetTRSNControlTimeByTRSN(string InTRSN)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT ROUND ((SYSDATE - WORK_TIME) * 24)  DIFF_TIME FROM MES4.R_TR_SN_DETAIL WHERE TR_SN= '{0}' AND LOCATION_FLAG = '2' AND WORK_FLAG = '0' ", InTRSN);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
            /*SELECT ROUND((SYSDATE-START_TIME)*1440) DIFF_TIME,STENCIL_SN FROM 
            MES4.R_STENCIL_WIP WHERE WO='005200155098' AND LINE_NAME = 'A12S9'(AllParts Line)
            如果查不到数据,则说明:鋼網未上線，請上線鋼網
            有值则需要检查DIFF_TIME是否大于LIMIT_WASH_TIME,
            SELECT LIMIT_WASH_TIME FROM MES1.C_STENCIL_BASE WHERE  STENCIL_SN='WW1805TT1170115'
            如果大于,则需要:鋼網超過規定使用時間，請更換鋼網*/
        public static DataTable GetStencilWIPInfoByWoLine(string InMo, string InAPSLineName)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format(" SELECT ROUND((SYSDATE-START_TIME)*1440) DIFF_TIME,STENCIL_SN FROM " +
                                    " MES4.R_STENCIL_WIP WHERE WO='{0}' AND LINE_NAME = '{1}' AND ROWNUM=1 ", InMo, InAPSLineName);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //LIMIT_WASH_TIME一般设定是360分钟
        public static DataTable GetStencilBaseInfoByStencilSN(string InStencilSN)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format(" SELECT * FROM MES1.C_STENCIL_BASE WHERE STENCIL_SN='{0}' AND ROWNUM=1 ", InStencilSN);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //根据机台查锡膏数据
        public static DataTable GetSolderWipInfoBySolderStation(string InSolderStation)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format(" SELECT TR_CODE,TR_SN,ROUND((SYSDATE-START_TIME)*24,2) DIFF_TIME,LIMIT_WASH_TIME FROM MES4.R_SOLDER_WIP WHERE STATION ='{0}' AND ROWNUM=1 ", InSolderStation);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        /*INSERT INTO MES4.R_SOLDER_WIP(TR_CODE, TR_SN, STATION, LINK_STATION, LIMIT_WASH_TIME, START_TIME)
            VALUES('A12S7AP1200508095516', 'K20042101784', 'A12S7AP1', 'A12S7AH2', '1200', SYSDATE)*/
        public static int InsertRSolderWIPInfo(string InTrCode, string InTRSN, string InStation, string InLinkStation, string InLimitTimes)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_SOLDER_WIP(TR_CODE, TR_SN, STATION, LINK_STATION, LIMIT_WASH_TIME, START_TIME )  
                                          VALUES('{0}','{1}','{2}','{3}','{4}', SYSDATE)",
                                          InTrCode, InTRSN, InStation, InLinkStation, InLimitTimes);
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
        //DELETE FROM MES4.R_SOLDER_WIP WHERE STATION = 'A62S1AP1'
        public static int DeleteRSolderWIPByStation(string InStation)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"DELETE MES4.R_SOLDER_WIP WHERE STATION ='{0}' ", InStation);
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        //获取PCB 清洗的数据,一般都是没有值的
        /*
        MES4.R_PCBWASH_CONTROL 
        WORK_FLAG='0' 正在清洗中
        WORK_FLAG='1' 結束成功
        WORK_FLAG='2' 已經報廢 清洗超过4个小时報廢
        SELECT TRUNC((SYSDATE-START_TIME)*24,2) AS WASHTIME FROM MES4.R_PCBWASH_CONTROL WHERE P_SN='" + strP_SN + "' AND WORK_FLAG='0' AND END_TIME IS NULL
        清洗必须大于10分钟,超过4个小时報廢
         */
        public static DataTable GetPCBWashInfoByPanelNo(string InPanelNo)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM MES4.R_PCBWASH_CONTROL WHERE P_SN= '{0}' AND (WORK_FLAG = '0' OR WORK_FLAG = '2') ", InPanelNo);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //获取正在清洗中的Panel信息
        public static DataTable GetAllPCBWashingInfo()
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT WO,P_SN,START_TIME,START_EMP,WORK_FLAG,END_TIME,END_EMP FROM MES4.R_PCBWASH_CONTROL WHERE WORK_FLAG = '0' ORDER BY WO,P_SN ");

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        public static DataTable GetPCBWashingInfoByPanelNo(string InPanelNo)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT A.*,TRUNC((SYSDATE-START_TIME)*24,2) AS WASH_HOUR FROM MES4.R_PCBWASH_CONTROL A WHERE P_SN= '{0}' AND WORK_FLAG = '0' ", InPanelNo);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        public static DataTable GetPCBWashInfoOnlyByPanelNo(string InPanelNo)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT A.* FROM MES4.R_PCBWASH_CONTROL A WHERE P_SN= '{0}' ", InPanelNo);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //插入清洗数据
        public static int InsertPCBWashInfo(string InWO, string InPanelNo, string InEmpNo)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_PCBWASH_CONTROL(WO,P_SN,WORK_FLAG,START_TIME,START_EMP )  
                                          VALUES('{0}','{1}','0',SYSDATE,'{2}')",
                                          InWO, InPanelNo,InEmpNo );
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
        //更新状态
        //WO,P_SN,WORK_FLAG,START_TIME,START_EMP,END_TIME,END_EMP
        public static int UpdatePCBWashInfo(string InPanelNo, string InWashOldStatus, string InWashStatus, string InEmpNo)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE MES4.R_PCBWASH_CONTROL SET WORK_FLAG='{2}',END_TIME=SYSDATE,END_EMP='{3}' WHERE P_SN ='{0}' AND WORK_FLAG ='{1}' ",
                                           InPanelNo, InWashOldStatus, InWashStatus, InEmpNo);
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        //SELECT *  FROM MES4.R_TR_PRODUCT_DETAIL  where p_sn='P011751020395'
        public static DataTable GetTRProductDetailInfoByPanelNo(string InPanelNo)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT A.* FROM MES4.R_TR_PRODUCT_DETAIL A WHERE P_SN= '{0}' ", InPanelNo);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //根据TRSN获取相关信息
        public static DataTable GetTRSNInfoByTRSN(string InTRSN)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT A.*,TO_CHAR(A.START_TIME,'YYYYMMDDHH24MISS') STARTTIME FROM MES4.R_TR_SN A WHERE TR_SN= '{0}' ", InTRSN);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //Update R_TR_SN for Return Material
        //UPDATE MES4.R_TR_SN SET EXT_QTY='6322',LOCATION_FLAG='1',WORK_FLAG='0',kitting_flag='a',END_TIME=SYSDATE  WHERE TR_SN='K19071800350'
        public static int UpdateRTRSNForReturn(string InTRSN, string InExtQty, string InLocationFlag, string InWorkFlag, string InKittingFlag)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE MES4.R_TR_SN SET EXT_QTY='{1}',LOCATION_FLAG='{2}',WORK_FLAG='{3}',KITTING_FLAG='{4}',END_TIME=SYSDATE WHERE TR_SN ='{0}' ",
                                           InTRSN, InExtQty, InLocationFlag, InWorkFlag, InKittingFlag);
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        //再更新 R_WO_REQUEST 
        //Update MES4.R_WO_REQUEST Set RETURN_QTY = RETURN_QTY + '6322' WHERE WO = '005100079652' AND CUST_KP_NO = '202.00004.015'
        public static int UpdateRWORequestForReturn(string InWO, string InPN , Int16 InExtQty)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE MES4.R_WO_REQUEST SET RETURN_QTY=RETURN_QTY+{2} WHERE WO='{0}' AND CUST_KP_NO ='{1}' ",
                                           InWO,InPN,InExtQty );
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        //再更新 R_KP_LIST   WIP_ExtQTY.Text
        //UPDATE MES4.R_KP_LIST SET EXT_QTY = '6322' WHERE TR_SN = 'K19071800350' AND EXT_QTY = '7837'
        public static int UpdateRKPListForReturn(string InTRSN, string InWIPQty, string InExtQty)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE MES4.R_KP_LIST SET EXT_QTY='{2}' WHERE TR_SN='{0}' AND EXT_QTY ='{1}' ",
                                           InTRSN, InWIPQty, InExtQty);
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        //UPDATE MES4.R_KP_LIST SET END_TIME = SYSDATE WHERE TR_SN = 'K20042101767' AND END_TIME IS NULL
        public static int UpdateRKPListEndTime(string InTRSN)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE MES4.R_KP_LIST SET END_TIME = SYSDATE WHERE TR_SN='{0}' AND END_TIME IS NULL ",InTRSN );
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        public static DataTable GetTRSNWIPInfoByTRSN(string InTRSN)
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format("SELECT * FROM MES4.R_TR_SN_WIP WHERE TR_SN= '{0}' ", InTRSN);

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //DELETE FROM MES4.R_TR_SN_WIP WHERE TR_SN='K19071800350'
        public static int DeleteTRSNWIPByTRSN(string InTRSN)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"DELETE MES4.R_TR_SN_WIP WHERE TR_SN ='{0}' ", InTRSN);
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        //Update MES4.R_TR_SN_WIP For Solder
        //UPDATE MES4.R_TR_SN_WIP SET STATION = 'A12S7AP1',WO = '005200155187' ,START_TIME = SYSDATE ,WORK_FLAG = '1'  WHERE TR_SN = 'K20042101784'
        public static int UpdateRTRSNWIPForSolder(string InTRSN, string InWO, string InStation, string InWorkFlag)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE MES4.R_TR_SN_WIP SET WO='{1}',STATION='{2}',START_TIME = SYSDATE ,WORK_FLAG = '{3}' WHERE TR_SN='{0}' AND ROWNUM=1 ",
                                           InTRSN, InWO, InStation, InWorkFlag);
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        public static DataTable GetSolderAllOnlineInfo()
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format(@"SELECT DISTINCT R.TR_CODE,R.TR_SN, C.LINE_NAME,R.STATION, R.LIMIT_WASH_TIME, R.START_TIME,  
                                      ROUND(TO_NUMBER(LIMIT_WASH_TIME) - TO_NUMBER((SYSDATE - R.START_TIME) * 24 * 60)) EXT_MINUTES,
                                      ROUND(TO_NUMBER(SYSDATE - R.START_TIME) * 24 * 60) ONLINE_MINUTES FROM
                                      MES4.R_SOLDER_WIP R, MES1.C_LINE_STATION C WHERE R.STATION = C.STATION_NAME ORDER BY STATION");

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }
        //
        public static DataTable GetMaterialKittingOutInfoByMO()
        {
            DataTable dt = null;
            try
            {
                string sql = null;
                sql = string.Format(@"SELECT * FROM MES4.R_STATION_WIP WHERE WO = '005200155272' AND SMT_CODE IS NOT NULL AND REPLACEKP_FLAG= 0 ORDER BY STATION");

                dt = AllPartDBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                AllPartDBHelper.getConnection.Close();
            }
            return dt;
        }

        /*INSERT INTO MES4.R_TR_CODE_LIST(TR_CODE, WO, STATION, START_TIME, EMP_NO)
         VALUES('A12S7AP1200508095516', '005200155187', 'A12S7AH2', SYSDATE, 'F6850755')*/
        public static int InsertRTRCodeListInfo(string InTRCode, string InWO, string InStation, string InUserID)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_TR_CODE_LIST(TR_CODE, WO, STATION, START_TIME, EMP_NO)  
                                          VALUES('{0}','{1}','{2}',SYSDATE,'{3}')",
                                          InTRCode, InWO, InStation, InUserID);
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
        //UPDATE mes4.R_TR_CODE_LIST SET END_TIME = SYSDATE WHERE STATION='A12S7AH2' and SUBSTR(TR_CODE,1,8)='A12S7AP1' and END_TIME IS NULL
        public static int UpdateRTRCodeList(string InLinkStation,string InStation)
        {
            int result = 0;
            try
            {
                string sql = string.Format(@"UPDATE MES4.R_TR_CODE_LIST SET END_TIME=SYSDATE WHERE STATION ='{0}' AND SUBSTR(TR_CODE,1,8)='{1}' AND  END_TIME IS NULL ",
                                           InLinkStation, InStation);
                result = DBHelper.ExecuteNonQuery(sql, null);
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
        /*
         INSERT INTO MES4.R_TR_CODE_DETAIL(TR_CODE, STATION, STATION_FLAG, WO, P_NO, P_VERSION, TR_SN, KP_NO, MFR_KP_NO, MFR_CODE,
        DATE_CODE, LOT_CODE, STANDARD_QTY, PROCESS_FLAG, REPLACEKP_FLAG, EMP_NO, WORK_TIME)
         VALUES('A12S7AP1200508095516', 'A12S7AH2', '1', '005200155187', 'EHS5-US-R3', '07', 'K20042101784', '591.00046.005', 'S3X58-M500',
         'YE00004840', '200212 ', 'L002121', '1', 'D', '0', 'F6850755', SYSDATE) */
        public static int InsertRTRCodeDetailInfo(string InTRCode,string InStation, string InStationFlag, string InWO, string InModelName,
            string InPCBVersion,string InTRSN, string InKPNO, string InMFRKPNO, string InMFRCode, string InDateCode, string InLotCode, 
            string InStandardQty, string InProcessFlag, string InReplaceFlag, string InUserID)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_TR_CODE_DETAIL(TR_CODE, STATION, STATION_FLAG, WO, P_NO, P_VERSION,TR_SN, KP_NO, 
                                          MFR_KP_NO, MFR_CODE,DATE_CODE, LOT_CODE, STANDARD_QTY, PROCESS_FLAG, REPLACEKP_FLAG, EMP_NO, WORK_TIME)  
                                          VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}',SYSDATE)",
                                          InTRCode, InStation, InStationFlag, InWO, InModelName, InPCBVersion, InTRSN, InKPNO, InMFRKPNO,
                                          InMFRCode, InDateCode, InLotCode,InStandardQty, InProcessFlag, InReplaceFlag, InUserID);
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
        /*INSERT INTO MES4.R_KP_LIST(TR_SN, WO, KP_NO, MFR_KP_NO, MFR_CODE, DATE_CODE, LOT_CODE, ADD_QTY, EXT_QTY, START_TIME, STATION, EMP_NO)
          VALUES('K20042101784', '005200155187', '591.00046.005', 'S3X58-M500', 'YE00004840', '200212', 'L002121', '1', '1', SYSDATE, 'A12S7AP1', 'F6850755')*/
        public static int InsertRKPListInfo(string InTRSN, string InWO,string InKPNO, string InMFRKPNO, string InMFRCode, string InDateCode, string InLotCode,
            string InAddQty,  string InExtQty, string InStation, string InUserID)
        {
            int result = 0;

            try
            {
                string sql = string.Format(@"INSERT INTO MES4.R_KP_LIST(TR_SN, WO, KP_NO, MFR_KP_NO, MFR_CODE, DATE_CODE, LOT_CODE, ADD_QTY, EXT_QTY, START_TIME, STATION, EMP_NO)  
                                          VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',SYSDATE,'{9}','{10}')",
                                          InTRSN, InWO, InKPNO, InMFRKPNO, InMFRCode, InDateCode, InLotCode, InAddQty, InExtQty, InStation, InUserID);
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

        //Query 查產品SN是否存在 
        public static DataTable QueryTSNInfo(string VarTSN)
        {
            DataTable dtQuery = null;

            try
            {
                string sql = "SELECT P_SN,SN_CODE,WO,PROCESS_FLAG,WORK_TIME,EMP_NO FROM MES4.R_SN_LINK WHERE P_SN =:TSN";
                OracleParameter[] parameter = new OracleParameter[]
                {
                    new OracleParameter(":TSN", VarTSN)
                 };
                dtQuery = DBHelper.GetDataTable(sql, parameter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dtQuery;
        }
        //Query 
        public static DataTable QueryMaterialInfoByTsn(string VarTSN)
        {
            DataTable dtQuery = null;
            try
            {
                string sql = string.Format(@" SELECT STATION,TR_SN,KP_NO,MFR_KP_NO,MFR_NAME,DATE_CODE,LOT_CODE,WORK_TIME,SERIAL_NUMBER,PANELNO,MO_NUMBER,MODEL_NAME FROM 
                           (SELECT A.P_SN SERIAL_NUMBER,A.SN_CODE PANELNO,A.WO MO_NUMBER,C.STATION,C.P_NO MODEL_NAME,C.TR_SN,KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,WORK_TIME FROM 
                           (SELECT P_SN,SN_CODE,WO FROM MES4.R_SN_LINK WHERE P_SN ='{0}' ) A, 
                           (SELECT TR_CODE,P_SN  FROM MES4.R_TR_PRODUCT_DETAIL WHERE P_SN IN (SELECT SN_CODE  FROM MES4.R_SN_LINK WHERE P_SN ='{0}' )) B,
                           (SELECT * FROM MES4.R_TR_CODE_DETAIL  WHERE TR_CODE IN ( 
                           SELECT TR_CODE FROM MES4.R_TR_PRODUCT_DETAIL WHERE P_SN IN (SELECT SN_CODE  FROM MES4.R_SN_LINK WHERE P_SN ='{0}' )) ) C 
                           WHERE A.SN_CODE=B.P_SN AND B.TR_CODE=C.TR_CODE  ) AA,MES1.C_MFR_CONFIG BB 
                           WHERE AA.MFR_CODE=BB.MFR_CODE ORDER BY WORK_TIME  ", VarTSN);

                dtQuery = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dtQuery;
        }
        public static DataTable QueryMaterialInfoByTsnPN(string VarTSN, string VarPN)
        {
            DataTable dtQuery = null;
            try
            {
                string sql = string.Format(@" SELECT STATION,TR_SN,KP_NO,MFR_KP_NO,MFR_NAME,DATE_CODE,LOT_CODE,WORK_TIME,SERIAL_NUMBER,PANELNO,MO_NUMBER,MODEL_NAME FROM 
                            (SELECT A.P_SN SERIAL_NUMBER,A.SN_CODE PANELNO,A.WO MO_NUMBER,C.STATION,C.P_NO MODEL_NAME,C.TR_SN,KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,WORK_TIME FROM 
                            (SELECT P_SN,SN_CODE,WO FROM MES4.R_SN_LINK WHERE P_SN ='{0}') A, 
                            (SELECT TR_CODE,P_SN  FROM MES4.R_TR_PRODUCT_DETAIL WHERE P_SN IN (SELECT SN_CODE  FROM MES4.R_SN_LINK WHERE P_SN ='{0}' )) B,
                            (SELECT * FROM MES4.R_TR_CODE_DETAIL  WHERE TR_CODE IN ( 
                            SELECT TR_CODE FROM MES4.R_TR_PRODUCT_DETAIL WHERE P_SN IN (SELECT SN_CODE  FROM MES4.R_SN_LINK WHERE P_SN ='{0}' ))  AND  KP_NO='{1}') C 
                            WHERE A.SN_CODE=B.P_SN AND B.TR_CODE=C.TR_CODE  ) AA,MES1.C_MFR_CONFIG BB 
                            WHERE AA.MFR_CODE=BB.MFR_CODE ORDER BY WORK_TIME ", VarTSN, VarPN);

                dtQuery = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dtQuery;
        }
    }
}
