using System;
using System.Data;

namespace DAL
{
    public class AOIService
    {
        //SELECT* FROM SFISM4.AOI_TRI_DEFECT  具體不良數據
        //SELECT* FROM SFISM4.AOI_TRI_BOARD   具體PanelSN狀態數據
        //導入從AOI系統中讀數據到SFC中
        public static DataTable GetPanelDataByPanelSN(string InPanelSN)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT CMODEL,BOARDSN,TOPBTM,IMULTI,FDATE,IDSTATION,STATUS,TOTALPAD,FAILCOMP,CYCLETIME,TOTALCOMP FROM SFISM4.AOI_TRI_BOARD WHERE BOARDSN='{0}' ORDER BY IMULTI ", InPanelSN);
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
        public static DataTable GetPanelDefectByPanelSN(string InPanelSN)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT SERIAL_NUMBER BOARDSN,BOARD_SEQNO,INSPECT_TIME,STATION_NAME,COMP_LOCATION,COMP_PNNAME,ERROR_DESC FROM SFISM4.AOI_TRI_DEFECT WHERE SERIAL_NUMBER='{0}' ORDER BY BOARD_SEQNO ", InPanelSN);
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
