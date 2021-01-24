using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.UserToken;
using System;
using System.Collections.Generic;
using System.Data;

namespace StrengthIgniter.Dal.UserToken
{
    public class UserTokenDataAccess : DataAccessBase, IUserTokenDataAccess
    {
        #region CTOR
        public UserTokenDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public IEnumerable<UserTokenModel> Select(Guid userReference)
        {
            string sp = "dbo.spUserTokenSelect";
            object parameters = new
            {
                UserReference = userReference
            };
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.Query<UserTokenModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public UserTokenModel Select(int userTokenId, Guid userReference)
        {
            string sp = "dbo.spUserTokenSelectById";
            object parameters = new
            {
                UserTokenId = userTokenId,
                UserReference = userReference
            };
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QuerySingleOrDefault<UserTokenModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public UserTokenModel SelectByReference(Guid reference)
        {
            string sp = "dbo.spUserTokenSelectByReference";
            object parameters = new
            {
                Reference = reference
            };
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<UserTokenModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex,sp, parameters);
            }
        }

        public void Insert(UserTokenModel token, IDbTransaction dbTransaction = null)
        {
            string sp = "dbo.spUserTokenInsert";
            object parameters = new
            {
                Reference = token.Reference,
                UserReference = token.UserReference,
                PurposeCode = token.PurposeCode,
                ExpiryDateTimeUtc = token.ExpiryDateTimeUtc
            };
            try
            {
                ManageConnection((con, trn) => {

                    con.Execute(sp, parameters, trn, commandType: CommandType.StoredProcedure);

                }, dbTransaction);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Delete(Guid tokenReference, IDbTransaction dbTransaction = null)
        {
            string sp = "dbo.spUserTokenDelete";
            object parameters = new
            {
                Reference = tokenReference,
            };
            try
            {
                ManageConnection((con, trn) => {

                    con.Execute(sp, parameters, trn, commandType: CommandType.StoredProcedure);

                }, dbTransaction);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }
    }
}
