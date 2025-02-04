using System;
using System.Collections.Generic;
using Sfc.Library.HttpClient;
using Newtonsoft.Json;
using Sfc.Core.Parameters;
using System.Reflection;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FG_CHECK
{
    public class ResouceClass
    {
        public SfcHttpClient sfcHttpClient { get; set; }

        public async Task<bool> Checkemp(string emp_no, string fun)
        {
            var _data = new
            {
                TYPE = "CHECKEMP",
                PRG_NAME = "CHECKIN",
                EMP_NO = emp_no,
                FUN = fun
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.API_EXECUTE ",
                    SfcCommandType = SfcCommandType.StoredProcedure,
                    SfcParameters = new List<SfcParameter>()
                    {
                        new SfcParameter{Name="DATA",Value=_jsonData,SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Input },
                        new SfcParameter{Name="OUTPUT",SfcParameterDataType=SfcParameterDataType.Varchar2,SfcParameterDirection=SfcParameterDirection.Output}
                    }
                });

                dynamic _ads = _result.Data;
                string _RES = _ads[0]["output"];
                string[] _RESArray = _RES.Split('#');

                if (_RESArray[0] == "OK")
                {
                    return true;
                }
                else
                {
                    if (_RESArray[0] == "NG")
                    {
                        return false;
                    }
                    else
                    {
                        MessageBox.Show("Excute procedure have error:" + _RESArray[0], "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Call procedure have exceptions:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public DataTable ToDataTable<T>(IEnumerable<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }

}
