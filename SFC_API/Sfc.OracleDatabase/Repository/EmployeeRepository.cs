using Dapper;
using Dapper.Oracle;
using Newtonsoft.Json.Linq;
using System;
using System.Data;


namespace Sfc.OracleDatabase.Repository
{
    public class EmployeeRepository : IDisposable
    {
        private OracleConnectionManager _ConnectionManager;
        private IDbTransaction _Transaction;
        private const string DATA = "DATA";
        private const string OK = "OK";
        private const string RES = "RES";
        private const string PROC_KEY = "proc";

        private const string SP_NAME = "CHECK_EMP_API";
        #region Properties
        public object _Lock = new object();
        #endregion

        #region Ctor
        public EmployeeRepository(string dbKey)
        {
            _ConnectionManager = new OracleConnectionManager(dbKey);
        }
        ~EmployeeRepository()
        {
            Dispose(false);
        }
        #endregion

        public string Check_Employee(string jsonString)
        {
            string res = string.Empty;

            lock (_Lock)
            {
                try
                {
                    _ConnectionManager.Open();

                    if (_ConnectionManager.IsConnected)
                    {
                        _Transaction = _ConnectionManager.Connection.BeginTransaction();

                        try
                        {
                            JObject jObject = JObject.Parse(jsonString);


                            var dynamicParams = new OracleDynamicParameters();
                            dynamicParams.Add(DATA, jsonString, dbType: OracleMappingType.Clob, direction: ParameterDirection.InputOutput);
                            dynamicParams.Add(RES, dbType: OracleMappingType.Clob, direction: ParameterDirection.Output, size: 4000);
                            _ConnectionManager.Connection.Execute(
                                SP_NAME,
                                param: dynamicParams,
                                commandType: CommandType.StoredProcedure,
                                commandTimeout: _ConnectionManager.CommandTimeout);
                            _Transaction.Commit();

                            res = dynamicParams.Get<string>(RES);
                            return res;
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
                    throw new Exception(string.Format("Exception: {0}", ex.Message));
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_Lock)
                {
                    if (_ConnectionManager != null)
                    {
                        _ConnectionManager.Dispose();
                        _ConnectionManager = null;
                    }
                    if (_Transaction != null)
                    {
                        _Transaction.Dispose();
                        _Transaction = null;
                    }
                }
            }
        }
    }
}
