using Newtonsoft.Json;
using Sfc.Core.Parameters;
using Sfc.Library.HttpClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PACK_PALT.Resource
{
    public class ResouceClass
    {
        public SfcHttpClient sfcHttpClient { get; set; }

        public async Task<bool> Checkemp(string emp_no, string fun)
        {
            var _data = new
            {
                TYPE = "CHECKEMP",
                PRG_NAME = "PRINT_LABEL",
                EMP_NO = emp_no,
                FUN = fun
            };

            //Tranform it to Json object
            string _jsonData = JsonConvert.SerializeObject(_data).ToString();
            try
            {
                var _result = await sfcHttpClient.ExecuteAsync(new QuerySingleParameterModel
                {
                    CommandText = "SFIS1.PRINT_LABEL_API_EXECUTE ",
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
                        ShowMessageForm _sh = new ShowMessageForm();
                        _sh.CustomFlag = true;
                        _sh.MessageVietNam = _RESArray[0];
                        _sh.MessageEnglish = "Excute procedure have error:";
                        _sh.ShowDialog();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessageForm _sh = new ShowMessageForm();
                _sh.CustomFlag = true;
                _sh.MessageVietNam = ex.Message.ToString();
                _sh.MessageEnglish = "Call procedure have exceptions:";
                _sh.ShowDialog();
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
