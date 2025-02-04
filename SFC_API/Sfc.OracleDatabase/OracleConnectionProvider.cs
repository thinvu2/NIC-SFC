using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;

namespace Sfc.OracleDatabase
{
    public class OracleConnectionProvider
    {
        public OracleConnectionProvider(string dbName)
        {
            try
            {
                _ConnectionString = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;
            }catch(Exception ex)
            {
                if(!IsGoodConnectionString(dbName))
                {
                    throw new Exception(string.Format("The key {0} don't exist in web.config. Please contact with Administrator to add them.", dbName));
                }
            }
        }
        private string _ConnectionString;
        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }
        }
        public bool IsGoodConnectionString(string dbName)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[dbName].ConnectionString != null;
            }
            catch
            {
                return false;
            }
        }
        //private string GetConnectionString()
        //{
        //    //var builder = new OracleConnectionStringBuilder();
        //    //var address = "10.225.35.61";
        //    //var port = 1521;
        //    //var serviceName = "nbbdbtest";
        //    //string dataSource = string.Format("(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})))", address, port, serviceName);
        //    //builder.UserID = "sfis1";
        //    //builder.Password = "vnsfis2014#!";
        //    //builder.DataSource = dataSource;
        //    //builder.Pooling = true;
        //    //return builder.ToString();
        //    string connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
        //    return connectionString;
        //}
        public IDbConnection Create(string connectionString)
        {
            return new OracleConnection(connectionString);
        }
        public IDbConnection Create()
        {
            return new OracleConnection(ConnectionString);
        }
    }
}
