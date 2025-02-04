using System;
using System.Data;

namespace DAL
{
    public class PanelSNService
    {
        //SELECT * FROM SFISM4.R_PANEL_OVERTIME  具體在TurnPCB Or Insp 作業時發現的Panel超時數據 
        // 
        public static DataTable GetPanelOverData()
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT MO_NUMBER,MODEL_NAME,GROUP_NAME,PANEL_NO,OVER_HOUR,WORK_DATE,WORK_SHIFT FROM SFISM4.R_PANEL_OVERTIME ORDER BY WORK_DATE,MO_NUMBER ");
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
        public static DataTable GetPanelOverDataByPanelSN(string InPanelSN)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT MO_NUMBER,MODEL_NAME,GROUP_NAME,PANEL_NO,OVER_HOUR,WORK_DATE,WORK_SHIFT FROM SFISM4.R_PANEL_OVERTIME WHERE PANEL_NO='{0}' ", InPanelSN);
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
        public static DataTable GetPanelOverDataByMO(string InMO)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT MO_NUMBER,MODEL_NAME,GROUP_NAME,PANEL_NO,OVER_HOUR,WORK_DATE,WORK_SHIFT FROM SFISM4.R_PANEL_OVERTIME WHERE MO_NUMBER='{0}' ORDER BY WORK_DATE ", InMO);
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

        public static DataTable GetPanelOverDataByWorkDate(string InWorkDate)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT MO_NUMBER,MODEL_NAME,GROUP_NAME,PANEL_NO,OVER_HOUR,WORK_DATE,WORK_SHIFT FROM SFISM4.R_PANEL_OVERTIME WHERE WORK_DATE='{0}' ORDER BY MO_NUMBER ", InWorkDate);
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

        public static DataTable GetPanelOverDataByGroup(string InGroup)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(" SELECT MO_NUMBER,MODEL_NAME,GROUP_NAME,PANEL_NO,OVER_HOUR,WORK_DATE,WORK_SHIFT FROM SFISM4.R_PANEL_OVERTIME WHERE GROUP_NAME='{0}' ORDER BY MO_NUMBER ", InGroup);
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
