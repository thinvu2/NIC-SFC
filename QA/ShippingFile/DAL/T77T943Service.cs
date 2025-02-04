using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class T77T943Service
    {
        //Pallet
        public static DataTable GetPackingDataByPallet(string InPallet)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT AA.*,BB.REEL_NUM,BB.ACTIVDATE FROM "
                + " (SELECT SERIAL_NUMBER,CUST_SN, UIM, IMEI,TELEPHONE_NUMBER,ETHERNET_MAC FROM SFISM4.R_T77T943_CSNMAC_T WHERE SERIAL_NUMBER IN ( "
                + " SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE IMEI='{0}') ) AA, SFISM4.R_T77T943_CSN_REEL_T BB "
                + " WHERE AA.TELEPHONE_NUMBER=BB.TELEPHONE_NUMBER ORDER BY SERIAL_NUMBER ", InPallet);
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
        //Carton
        public static DataTable GetPackingDataByCarton(string InCarton)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT AA.*,BB.REEL_NUM,BB.ACTIVDATE FROM "
                + " (SELECT SERIAL_NUMBER,CUST_SN, UIM, IMEI,TELEPHONE_NUMBER,ETHERNET_MAC FROM SFISM4.R_T77T943_CSNMAC_T WHERE SERIAL_NUMBER IN ( "
                + " SELECT SERIAL_NUMBER FROM SFISM4.R_WIP_TRACKING_T WHERE MCARTON_NO='{0}') ) AA, SFISM4.R_T77T943_CSN_REEL_T BB "
                + " WHERE AA.TELEPHONE_NUMBER=BB.TELEPHONE_NUMBER ORDER BY SERIAL_NUMBER ", InCarton);
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
        //CSN
        public static DataTable GetPackingDataByCSN(string InCSN)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT AA.*,BB.REEL_NUM,BB.ACTIVDATE FROM "
                + " (SELECT SERIAL_NUMBER,CUST_SN, UIM, IMEI,TELEPHONE_NUMBER,ETHERNET_MAC FROM SFISM4.R_T77T943_CSNMAC_T "
                + "  WHERE CUST_SN='{0}' ) AA, SFISM4.R_T77T943_CSN_REEL_T BB "
                + " WHERE AA.TELEPHONE_NUMBER=BB.TELEPHONE_NUMBER ORDER BY SERIAL_NUMBER ", InCSN);
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
        //SN
        public static DataTable GetPackingDataBySN(string InSN)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT AA.*,BB.REEL_NUM,BB.ACTIVDATE FROM "
                + " (SELECT SERIAL_NUMBER,CUST_SN, UIM, IMEI,TELEPHONE_NUMBER,ETHERNET_MAC FROM SFISM4.R_T77T943_CSNMAC_T "
                + "  WHERE SERIAL_NUMBER='{0}' ) AA, SFISM4.R_T77T943_CSN_REEL_T BB "
                + " WHERE AA.TELEPHONE_NUMBER=BB.TELEPHONE_NUMBER ORDER BY SERIAL_NUMBER ", InSN);
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
        //AX
        public static DataTable GetPackingDataByAX(string InAX)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT AA.*,BB.REEL_NUM,BB.ACTIVDATE FROM "
                + " (SELECT SERIAL_NUMBER,CUST_SN, UIM, IMEI,TELEPHONE_NUMBER,ETHERNET_MAC FROM SFISM4.R_T77T943_CSNMAC_T "
                + "  WHERE UIM='{0}' ) AA, SFISM4.R_T77T943_CSN_REEL_T BB "
                + " WHERE AA.TELEPHONE_NUMBER=BB.TELEPHONE_NUMBER ORDER BY SERIAL_NUMBER ", InAX);
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
    }
}
