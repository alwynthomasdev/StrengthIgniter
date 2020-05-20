using Dapper;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StrengthIgniter.Core.Data
{
    public interface IUserDataAccess
    {
        UserModel GetByEmailAddress(string emailAddress);
        void UpdateUserLoginAttempt(IDbConnection con, IDbTransaction transaction, UserModel user);

        int CreateNewUser(IDbConnection con, IDbTransaction transaction, UserModel user);
        void CreateUserToken(IDbConnection con, IDbTransaction transaction, Guid userReference, UserTokenModel token);
        void CreateSecurityQuestions(IDbConnection con, IDbTransaction transaction, Guid userReference, IEnumerable<UserSecurityQuestionAnswerModel> questions);
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

        public int CreateNewUser(IDbConnection con, IDbTransaction transaction, UserModel user)
        {
            #region SQL
            string sql = @"
INSERT INTO [User]
    ([Reference]
    ,[Name]
    ,[EmailAddress]
    ,[PasswordHash]
    ,[UserTypeCode]
    ,[RegisteredDateTimeUtc])
VALUES
    (@Reference
    ,@Name
    ,@EmailAddress
    ,@PasswordHash
    ,@UserTypeCode
    ,@RegisteredDateTimeUtc);
".Trim();
            #endregion

            try
            {
                int? userId = con.QueryFirstOrDefault<int>(sql, user, transaction);
                if (userId.HasValue)
                {
                    foreach(UserTokenModel token in user.Tokens)
                    {
                        CreateUserToken(con, transaction, user.Reference, token);
                    }
                    CreateSecurityQuestions(con, transaction, user.Reference, user.SecurityQuestions);
                    return userId.Value;
                }
                else
                {
                    throw new DataAccessException("Failed to create user.", sql, user);
                }
            }
            catch (DataAccessException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, user);
            }
        }

        public void CreateSecurityQuestions(IDbConnection con, IDbTransaction transaction, Guid userReference, IEnumerable<UserSecurityQuestionAnswerModel> questions)
        {
            #region SQL
            string sql = @"
DECLARE @UserId INTEGER
SELECT TOP 1 @UserId = [UserId] FROM [User] WHERE [Reference] = @UserReference

INSERT INTO [UserSecurityQuestionAnswer]
    ([Reference]
    ,[UserId]
    ,[QuestionText]
    ,[AnswerHash])
VALUES
    (@Reference
    ,@UserId
    ,@QuestionText
    ,@AnswerHash)
".Trim();
//            string updateSecurityQuestion = @"
//UPDATE [UserSecurityQuestionAnswer]
//SET
//    [QuestionText] = @QuestionText,
//    [AnswerHash] = @AnswerHash,
//    [FailedAnswerAttemptCount] = @FailedAnswerAttemptCount
//WHERE [Reference] = @Reference
//".Trim();
            #endregion

            foreach(UserSecurityQuestionAnswerModel question in questions)
            {
                object parameters = new
                {
                    UserReference = userReference,
                    Reference = question.Reference,
                    QuestionText = question.QuestionText,
                    AnswerHash = question.AnswerHash
                };

                try
                {
                    con.Execute(sql, question, transaction);
                }
                catch (Exception ex)
                {
                    throw new DataAccessException(ex, sql, question);
                }
            }
        }

        public void CreateUserToken(IDbConnection con, IDbTransaction transaction, Guid userReference, UserTokenModel token)
        {
            #region SQL
            string sql = @"
DECLARE @UserId INTEGER
SELECT TOP 1 @UserId = [UserId] FROM [User] WHERE [Reference] = @UserReference

DELETE FROM [UserToken] WHERE [UserId] = @UserId AND [PurposeCode] = @PurposeCode;

INSERT INTO [UserToken]
    ([UserId]
    ,[Reference]
    ,[PurposeCode]
    ,[IssuedDateTimeUtc]
    ,[ExpiryDateTimeUtc])
VALUES
    (@UserId
    ,@Reference
    ,@PurposeCode
    ,@IssuedDateTimeUtc
    ,@ExpiryDateTimeUtc)
".Trim();
            #endregion

            object parameters = new
            {
                UserReference = userReference,
                Reference = token.Reference,
                PurposeCode = token.PurposeCode,
                IssuedDateTimeUtc = token.IssuedDateTimeUtc,
                ExpiryDateTimeUtc = token.ExpiryDateTimeUtc
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

        #region Private Methods



        #endregion

    }
}
