using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DAL
{
    public class ReworkService
    {
        //Query Invoice(DN) Info
        public static DataTable QueryInvoiceInfo(string InInvoiceNo)
        {
            DataTable dtQueryInvoiceInfo = null;

            try
            {
                string sql = string.Format(" SELECT SO_QTY,MODEL_NAME,FINISH_DATE FROM SFISM4.R_BPCS_INVOICE_T WHERE INVOICE='{0}' AND ROWNUM=1 ", InInvoiceNo);

                dtQueryInvoiceInfo = DBHelper.GetDataTable(sql, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                DBHelper.getConnection.Close();
            }
            return dtQueryInvoiceInfo;
        }
        //通過調用SP:SFIS1.CANCEL_DN_INVOICE把產品打回到待出貨狀態或Rework到產線
        //InReturnFlag Y代表是Rework到產線,否則就是Z107的更新而已,z107的reworkNo=invoice對應的tcom
        //r_bpcs_invoice_t則會清除,可以重新Ship狀態
        public static string ExecCancelDNProcedure(string InInvoice, string InReturnGroup, string InReturnFlag)
        {
            string result = null;
            try
            {
                OracleParameter[] parameter = new OracleParameter[4]
                {
                    new OracleParameter("INVOICENO",OracleDbType.Varchar2, 20),
                    new OracleParameter("RETURNGROUP",OracleDbType.Varchar2, 20),
                    new OracleParameter("RETURNFLAG",OracleDbType.Varchar2, 2),
                    new OracleParameter("RETURNSTRING",OracleDbType.Varchar2, 100)
                };

                for (int i = 0; i < parameter.Length - 1; i++)
                {
                    parameter[i].Direction = ParameterDirection.Input;
                }
                parameter[3].Direction = ParameterDirection.Output;

                parameter[0].Value = InInvoice;
                parameter[1].Value = InReturnGroup;
                parameter[2].Value = InReturnFlag;

                result = DBHelper.RunProcedure("SFIS1.CANCEL_DN_INVOICE", parameter);
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
    }
}
