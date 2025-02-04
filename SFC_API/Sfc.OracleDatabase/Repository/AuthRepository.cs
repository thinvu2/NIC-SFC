using Dapper;
using Sfc.Core.Entities;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Sfc.OracleDatabase.Repository
{
    public class AuthRepository : IDisposable
    {
        private OracleConnectionManager _ConnectionManager;
        private IDbTransaction _Transaction;

        #region Ctor
        public AuthRepository(string dbKey)
        {
            _ConnectionManager = new OracleConnectionManager(dbKey);
        }

        ~AuthRepository()
        {
            Dispose(false);
        }
        #endregion
        public async Task<EmployeeEntity> FindEmployeeAsync(string username, string password)
        {


                try
                {



                    _ConnectionManager.Open();

                    if (_ConnectionManager.IsConnected)
                    {

                        try
                        {
                            var query = string.Format("SELECT * FROM SFIS1.C_EMP_DESC_T WHERE EMP_NO = '{0}' AND (EMP_PASS ='{1}' OR EMP_BC = '{1}')", username, password);
                            var result = _ConnectionManager.Connection.QueryFirstOrDefault<EmployeeEntity>(query);
                            return result;
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
                    throw ex;
                }

            

        }
        public ClientEntity FindClient(string clienId)
        {



                try
                {



                    _ConnectionManager.Open();

                    if (_ConnectionManager.IsConnected)
                    {

                        try
                        {
                            var query = string.Format("SELECT * FROM SFIS1.C_API_CLIENT_T WHERE ID = '{0}'", clienId);
                            var result = _ConnectionManager.Connection.QueryFirstOrDefault<ClientEntity>(query);
                            return result;
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
                    throw ex;
                }

            
          
        }

        public  async Task<bool> AddRefreshTokenAsync(RefreshTokenEntity refreshTokenEntity)
        {

                try
                {
                    _ConnectionManager.Open();

                    if (_ConnectionManager.IsConnected)
                    {
                        _Transaction = _ConnectionManager.Connection.BeginTransaction();

                        try
                        {
                            var query = string.Format("SELECT * FROM SFIS1.C_API_REFRESHTOKEN_T WHERE SUBJECT = '{0}' AND CLIENTID = '{1}'", refreshTokenEntity.Subject, refreshTokenEntity.ClientId);
                            var result = await _ConnectionManager.Connection.QueryFirstOrDefaultAsync<RefreshTokenEntity>(query);
                            if(result != null)
                            {
                                var delete_stmt = string.Format("DELETE FROM SFIS1.C_API_REFRESHTOKEN_T WHERE ID = '{0}'", result.Id);
                                await _ConnectionManager.Connection.ExecuteAsync(query);
                            }

                            var sbSql = new StringBuilder();
                            sbSql.Append("INSERT INTO SFIS1.C_API_REFRESHTOKEN_T(ID,SUBJECT,CLIENTID,ISSUED,EXPIRES,PROTECTEDTICKET) ");
                            sbSql.Append("VALUES");
                            sbSql.Append("(");
                            sbSql.AppendFormat("'{0}',", refreshTokenEntity.Id);
                            sbSql.AppendFormat("'{0}',", refreshTokenEntity.Subject);
                            sbSql.AppendFormat("'{0}',", refreshTokenEntity.ClientId);
                            sbSql.AppendFormat("TO_DATE('{0}','YYYY/MM/DD HH24:MI:SS'),", refreshTokenEntity.Issued.ToString("yyyy/MM/dd hh:mm:ss"));
                            sbSql.AppendFormat("TO_DATE('{0}','YYYY/MM/DD HH24:MI:SS'),", refreshTokenEntity.Expires.ToString("yyyy/MM/dd hh:mm:ss"));
                            sbSql.AppendFormat("'{0}'", refreshTokenEntity.ProtectedTicket);
                            sbSql.Append(")");
                            var stmt_execute = sbSql.ToString();

                            var isSuccess = await _ConnectionManager.Connection.ExecuteAsync(stmt_execute);
                            _Transaction.Commit();
                            return isSuccess > 0;
                            
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
                throw ex;
                }
        }
        public async Task<bool> RemoveRefreshTokenAsync(RefreshTokenEntity refreshTokenEntity)
        {

                try
                {

                    _ConnectionManager.Open();

                    if (_ConnectionManager.IsConnected)
                    {
                        _Transaction = _ConnectionManager.Connection.BeginTransaction();

                        try
                        {
                            var query = string.Format("DELETE FROM SFIS1.C_API_REFRESHTOKEN_T WHERE ID = '{0}'", refreshTokenEntity.Id);
                            var result = await _ConnectionManager.Connection.ExecuteAsync(query);
                            _Transaction.Commit();
                            return result > 0;


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
                    throw ex;
                }
        }

        public async Task<RefreshTokenEntity> FindRefreshTokenAsync(string refreshTokenId)
        {
            try
            {
                _ConnectionManager.Open();

                if (_ConnectionManager.IsConnected)
                {
                    _Transaction = _ConnectionManager.Connection.BeginTransaction();

                    try
                    {
                        string query = string.Format("SELECT * FROM C_API_REFRESHTOKEN_T WHERE ID ='{0}'", refreshTokenId);

                        var refreshToken = await _ConnectionManager.Connection.QueryFirstOrDefaultAsync<RefreshTokenEntity>(query);
                        _Transaction.Commit();
                        return refreshToken;
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
                throw ex;
            }

        }

        public async Task<bool> RemoveRefreshTokenAsync(string refreshTokenId)
        {
            try
            {
                _ConnectionManager.Open();

                if (_ConnectionManager.IsConnected)
                {
                    _Transaction = _ConnectionManager.Connection.BeginTransaction();

                    try
                    {
                        string query = string.Format("SELECT * FROM SFIS1.C_API_REFRESHTOKEN_T WHERE ID ='{0}'", refreshTokenId);

                        var refreshToken = await _ConnectionManager.Connection.QueryFirstOrDefaultAsync<RefreshTokenEntity>(query);
                        int result = 0;
                        if(refreshToken != null)
                        {
                            string delete_stmt = string.Format("DELETE FROM SFIS1.C_API_REFRESHTOKEN_T WHERE ID = '{0}'", refreshToken.Id);
                            result = await _ConnectionManager.Connection.ExecuteAsync(delete_stmt);
                            _Transaction.Commit();
                            //await RemoveRefreshToken(refreshToken);


                        }
                        return result > 0;
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
                throw ex;
            }

        }
        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
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
        #endregion
    }
}
