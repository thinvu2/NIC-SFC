using System;
using System.Data;

namespace Sfc.OracleDatabase
{
    public class OracleConnectionManager : IDisposable
    {

        #region Fields
        public OracleConnectionProvider Provider;
        private IDbConnection _Connection;
        #endregion
        #region Properties

        private string _ConnectionString;
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        private int? _CommandTimeout;
        public int? CommandTimeout 
        { 
            get
            {
                return _CommandTimeout;
            } 
            set
            {
                _CommandTimeout = value;
            }
        }
        private object _Lock = new object();
        public IDbConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }

        public bool IsConnected
        {
            get { return _Connection != null && _Connection.State == ConnectionState.Open; }
        }
        #endregion

        #region Ctor
        public OracleConnectionManager(string dbName)
        {
            
            Provider = new OracleConnectionProvider(dbName);
        }

        public bool IsGoodConnectionString(string dbName)
        {
            return Provider.IsGoodConnectionString(dbName);
        }

        ~OracleConnectionManager()
        {
            Dispose(false);
        }
        #endregion

        #region Methods
        public void Open()
        {
            lock(_Lock)
            {
                if(_Connection == null)
                {

                        var connection = Provider.Create();
                        _ConnectionString = connection.ConnectionString;
                        _CommandTimeout = connection.ConnectionTimeout;
                        connection?.Open();
                  
                        _Connection = connection.State == ConnectionState.Open ? connection : null;
                

                }
            }
        }
        public void Close()
        {
            lock(_Lock)
            {
                if(_Connection != null)
                {
                    _Connection.Dispose();
                    _Connection = null;
                }
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
                Close();
            }
        }

        #endregion


    }
}
