using Dapper;
using Dapper.Oracle;
using Sfc.Core.Models;
using Sfc.Core.Parameters;
using Sfc.OracleDatabase.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.OracleDatabase.Repository
{
    public class SfcRepository : IDisposable
    {
        #region Fields
        
        private OracleConnectionManager _ConnectionManager;
        private IDbTransaction _Transaction;


  
        private const string DATA = "DATA";
        private const string OK = "OK";
        private const string RES = "RES";
        private const string PROC_KEY = "proc";
        private const string TRANSACTION_KEY = "transaction";

        #endregion

        #region Properties
        #endregion

        #region Ctor
        public SfcRepository(string dbKey)
        {
            _ConnectionManager = new OracleConnectionManager(dbKey);
        }

        ~SfcRepository()
        {
            Dispose(false);
        }
        #endregion

        #region Methods
    

        public async Task<ResponseModelList> ExecuteAsync(QuerySingleParameterModel model)
        {

            try
            {


                _ConnectionManager.Open();

                if (_ConnectionManager.IsConnected)
                {
                    _Transaction = _ConnectionManager.Connection.BeginTransaction();

                    try
                    {
                        int rowEffected = 0;
                        if (model.SfcCommandType == SfcCommandType.StoredProcedure)
                        {


                      
                            OracleDynamicParameters parameters = OracleDatabaseHelper.GetOracleDynamicParameters(model);
                            if (parameters.ParameterNames.Count() > 0)
                            {
                                rowEffected = await _ConnectionManager.Connection.ExecuteAsync
                                    (
                                        sql: model.CommandText,
                                        param: parameters,
                                        transaction: _Transaction,
                                        commandType: (CommandType)model.SfcCommandType,
                                        commandTimeout: _ConnectionManager.CommandTimeout
                                    );
                            }
                            else
                            {
                                rowEffected = await _ConnectionManager.Connection.ExecuteAsync
                                     (
                                         sql: model.CommandText,
                                         transaction: _Transaction,
                                         commandType: (CommandType)model.SfcCommandType,
                                         commandTimeout: _ConnectionManager.CommandTimeout
                                     );
                            }
                            _Transaction.Commit();
                            List<Dictionary<string, object>> paramResResults = new List<Dictionary<string, object>>();
                            foreach (var sfcParam in model.SfcParameters)
                            {
                                if (sfcParam.SfcParameterDirection == SfcParameterDirection.InputOutput || sfcParam.SfcParameterDirection == SfcParameterDirection.Output)
                                {
                                    dynamic res = parameters.Get<dynamic>(sfcParam.Name)?.ToString();
                                    paramResResults.Add
                                    (
                                        new Dictionary<string, object>
                                        {
                                        { sfcParam.Name, res}
                                        }
                                    );
                                }
                            }

                            var responseModelList = new ResponseModelList(ResponseModelResult.OK, string.Format("OK", rowEffected), paramResResults);
                            return responseModelList;
                        } else
                        {

                            OracleDynamicParameters parameters = OracleDatabaseHelper.GetOracleDynamicParameters(model);
                            if (parameters.ParameterNames.Count() > 0)
                            {
                                rowEffected = await _ConnectionManager.Connection.ExecuteAsync
                                    (
                                        sql: model.CommandText,
                                        param: parameters,
                                        transaction: _Transaction,
                                        commandType: (CommandType)model.SfcCommandType,
                                        commandTimeout: _ConnectionManager.CommandTimeout
                                    );
                            }
                            else
                            {
                                rowEffected = await _ConnectionManager.Connection.ExecuteAsync
                                     (
                                         sql: model.CommandText,
                                         transaction: _Transaction,
                                         commandType: (CommandType)model.SfcCommandType,
                                         commandTimeout: _ConnectionManager.CommandTimeout
                                     );
                            }
                            _Transaction.Commit();
                            var responseModelList = new ResponseModelList(ResponseModelResult.OK, string.Format("{0} row effected", rowEffected), null);
                            return responseModelList;
                        }

                    }
                    catch (Exception ex)
                    {
                        _Transaction.Rollback();
                        throw ex;
                    }
                }
                else
                {
                    throw new Exception(string.Format("Cannot open database "));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task<ResponseModelSingle> QuerySingleAsync(QuerySingleParameterModel model)
        {

                try
                {



                    _ConnectionManager.Open();

                    if (_ConnectionManager.IsConnected)
                    {

                        if(model.SfcCommandType != SfcCommandType.Text)
                    {
                        throw new Exception("Only support CommandType.Text");
                    }
                        try
                        {
            

                            IDictionary<string, object> result = new Dictionary<string, object>();
                            OracleDynamicParameters parameters = OracleDatabaseHelper.GetOracleDynamicParameters(model);
                            if(parameters.ParameterNames.Count() > 0) 
                            {
                               result = _ConnectionManager.Connection.QueryFirstOrDefault
                                    (
                                        sql: model.CommandText,
                                        param: parameters,
                                        commandType: (CommandType)model.SfcCommandType,
                                        commandTimeout: _ConnectionManager.CommandTimeout
                                    );
                        } else
                            {
                                 result = _ConnectionManager.Connection.QueryFirstOrDefault
                                     (
                                         sql: model.CommandText,
                                         commandType: (CommandType)model.SfcCommandType,
                                         commandTimeout: _ConnectionManager.CommandTimeout
                                     );
                       
                        }
                        var responseModelSingle = new ResponseModelSingle(ResponseModelResult.OK, ResponseModelResult.OK, result);
                        return responseModelSingle;
                        //var row1 = row.ToDictionary(d => d.Key, d => d.Value);
                        //var result = _ConnectionManager.Connection.QueryFirstOrDefault(commandText) as Dictionary<string, object>;
                        //var rows = result.Select(r => r.Distinct().ToDictionary(d => d.Key.ToLower(), d => d.Value));


                        /*IList<IDictionary<string, object>> rows = (from row in _ConnectionManager.Connection.Query(commandText)
                                                                   select (IDictionary<string, object>)row).ToList();*/

                        //// var rows = _ConnectionManager.Connection.Query(commandText);
                        //var settings = new JsonSerializerSettings
                        //{
                        //    Formatting = Formatting.Indented,
                        //    DateFormatString = "yyyy/MM/dd hh:mm:ss",
                        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        //};
                        ///*
                        // * var settings = new JsonSerializerSettings
                        //    {
                        //        Formatting = Formatting.Indented,
                        //        NullValueHandling = NullValueHandling.Ignore,
                        //        ReferenceLoopHandling=ReferenceLoopHandling.Ignore,
                        //        PreserveReferencesHandling = PreserveReferencesHandling.Arrays
                        //    };
                        // */

                        //var toJsonString = JsonConvert.SerializeObject(rows, settings);
                        //JArray jArray = JArray.Parse(toJsonString);

                        //res = ResponseModel.CreateResponse(ResponseModel.OK, ResponseModel.OK, code: 200, jArray);
                        //return res;
                    }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Cannot open database "));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            
        }
      
        public async Task<ResponseModelList> QueryListAsync(QuerySingleParameterModel model)
        {

            try
            {


                _ConnectionManager.Open();

                if (_ConnectionManager.IsConnected)
                {
                    if (model.SfcCommandType != SfcCommandType.Text)
                    {
                        throw new Exception("Only support CommandType.Text");
                    }
                    try
                    {

                        IEnumerable<IDictionary<string, object>> results = new List<Dictionary<string, object>>();

                        OracleDynamicParameters parameters = OracleDatabaseHelper.GetOracleDynamicParameters(model);
                        if (parameters.ParameterNames.Count() > 0)
                        {
                            results = _ConnectionManager.Connection.Query
                             (
                                 sql: model.CommandText,
                                 param: parameters,
                                 commandType: (CommandType)model.SfcCommandType,
                                 commandTimeout: _ConnectionManager.CommandTimeout
                             ) as IEnumerable<IDictionary<string, object>>;
                        }
                        else
                        {
                            results = _ConnectionManager.Connection.Query
                                 (
                                     sql: model.CommandText,
                                     commandType: (CommandType)model.SfcCommandType,
                                     commandTimeout: _ConnectionManager.CommandTimeout
                                 ) as IEnumerable<IDictionary<string, object>>;
                        }


                        var responseModelList = new ResponseModelList(ResponseModelResult.OK, ResponseModelResult.OK, results);
                        return responseModelList;

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw new Exception(string.Format("Cannot open database "));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        #endregion
        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {

                    if(_ConnectionManager != null)
                    {
                        _ConnectionManager.Dispose();
                        _ConnectionManager = null;
                    }
                    if(_Transaction != null)
                    {
                        _Transaction.Dispose();
                        _Transaction = null;
                    }
            }
        }
        #endregion
    }
}
