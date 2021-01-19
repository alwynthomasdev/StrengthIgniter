using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.User;
using System;
using System.Data;

namespace StrengthIgniter.Dal.User
{
    public class UserDataAccess : DataAccessBase, IUserDataAccess
    {
        #region CTOR
        public UserDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public UserModel Select(Guid userReference)
        {
            string sp = "dbo.spUserSelectByReference";
            object parameters = new
            {
                Reference = userReference
            };
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<UserModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public UserModel Select(string emailAddress)
        {
            string sp = "dbo.spUserSelectByEmailAddress";
            object parameters = new
            {
                EmailAddress = emailAddress
            };
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<UserModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Insert(UserModel user, IDbTransaction dbTransaction = null)
        {
            user.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spUserInsert";
            object parameters = new
            {
                Reference = user.Reference,
                Name = user.Name,
                EmailAddress = user.EmailAddress,
                PasswordHash = user.PasswordHash,
                UserTypeCode = user.UserTypeCode
            };
            try
            {
                dbConnection.Execute(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public void Update(UserModel user, IDbTransaction dbTransaction = null)
        {
            user.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spUserUpdate";
            object parameters = new
            {
                Reference = user.Reference,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
                UserTypeCode = user.UserTypeCode,
                LastLoginDateTimeUtc = user.LastLoginDateTimeUtc,
                LockoutEndDateTimeUtc = user.LockoutEndDateTimeUtc,
                FailedLoginAttemptCount = user.FailedLoginAttemptCount,
                IsRegistrationValidated = user.IsRegistrationValidated
            };
            try
            {
                dbConnection.Execute(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

    }
}
