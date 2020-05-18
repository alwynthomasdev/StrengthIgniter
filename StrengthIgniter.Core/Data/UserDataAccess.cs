using Dapper;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using System;
using System.Data;

namespace StrengthIgniter.Core.Data
{
    public interface IUserDataAccess
    {
        UserModel GetByEmailAddress(string emailAddress);
        void UpdateUserLoginAttempt(IDbConnection con, IDbTransaction transaction, UserModel user);
    }

    public class UserDataAccess : DataAccessBase, IUserDataAccess
    {
        #region CTOR
        public UserDataAccess(Func<IDbConnection> fnGetConnection) : base(fnGetConnection)
        {
        }
        #endregion

        public UserModel GetByEmailAddress(string emailAddress)
        {
            #region sql
            string sql = @"
SELECT TOP 1
    [u].[UserId],
    [u].[Reference],
    [u].[Name],
    [u].[EmailAddress],
    [u].[PasswordHash],
    [u].[UserTypeCode],
    [u].[LastLoginDateTimeUtc],
    [u].[LockoutEndDateTimeUtc],
    [u].[FailedLoginAttemptCount],
    [u].[IsRegistrationValidated],
    [u].[RegisteredDateTimeUtc]
FROM [User] [u]
WHERE 
    [u].[EmailAddress] = @EmailAddress AND
    [u].[IsDeleted] = 0".Trim();
            #endregion

            object parameters = new { EmailAddress = emailAddress };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QuerySingleOrDefault<UserModel>(sql, parameters);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        public void UpdateUserLoginAttempt(IDbConnection con, IDbTransaction transaction, UserModel user)
        {
            #region SQL
            string sql = @"
UPDATE [User] SET
    [LastLoginDateTimeUtc] = @LastLoginDateTimeUtc,
    [LockoutEndDateTimeUtc] = @LockoutEndDateTimeUtc,
    [FailedLoginAttemptCount] = @FailedLoginAttemptCount
WHERE [UserId] = @UserId
".Trim();
            #endregion

            object parameters = new
            {
                UserId = user.UserId,
                LastLoginDateTimeUtc = user.LastLoginDateTimeUtc,
                LockoutEndDateTimeUtc = user.LockoutEndDateTimeUtc,
                FailedLoginAttemptCount = user.FailedLoginAttemptCount
            };

            try
            {
                con.Execute(sql, parameters, transaction);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

    }
}
