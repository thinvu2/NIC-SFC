using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DAL
{
    public class MenuService
    {
        public static DataTable GetAllMenu(string bu)
        {
            DataTable dt = null;
            try
            {
                string sql = string.Format(@"SELECT * FROM SFIS1.C_NAVIGATE_MENU_T WHERE 
                            NODE_NAME IN (SELECT FUN FROM SFIS1.C_PRIVILEGE WHERE PRG_NAME='ShopFloor' AND EMP='{0}')
                            OR NODE_URL IS NULL ", UserService.loginNO);
                if (bu == "SFC_NIC")
                {
                    sql += " OR PARENT_NODE_ID IN ('Q03','Q08') ";
                }
                if (UserService.userRank == "9")
                {
                    sql = "SELECT * FROM SFIS1.C_NAVIGATE_MENU_T ORDER BY NODE_ID";
                }
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
    }
}
