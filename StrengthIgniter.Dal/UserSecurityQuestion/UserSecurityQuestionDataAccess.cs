using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.UserSecurityQuestion;
using System;
using System.Collections.Generic;
using System.Data;

namespace StrengthIgniter.Dal.UserSecurityQuestion
{
    

    public class UserSecurityQuestionDataAccess : DataAccessBase, IUserSecurityQuestionDataAccess
    {
        #region CTOR
        public UserSecurityQuestionDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public IEnumerable<UserSecurityQuestionModel> Select(Guid userReference)
        {
            string sp = "dbo.spUserSecurityQuestionSelect";
            object parameters = new
            {
                UserReference = userReference
            };
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.Query<UserSecurityQuestionModel>(sp, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

        public int Insert(UserSecurityQuestionModel userSecurityQuestion, IDbTransaction dbTransaction = null)
        {
            userSecurityQuestion.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spUserSecurityQuestionInsert";
            object parameter = new
            {
                Reference = userSecurityQuestion.Reference,
                UserReference = userSecurityQuestion.UserReference,
                QuestionText = userSecurityQuestion.QuestionText,
                AnswerHash = userSecurityQuestion.AnswerHash,
                FailedAnswerAttemptCount = userSecurityQuestion.FailedAnswerAttemptCount
            };
            try
            {
                int? id = dbConnection.QueryFirstOrDefault<int?>(sp, parameter, commandType: CommandType.StoredProcedure);
                if (id.HasValue)
                {
                    return id.Value;
                }
                else throw new Exception("Failed to insert UserSecurityQuestion.");
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp, parameter);
            }
        }

        public void Update(UserSecurityQuestionModel userSecurityQuestion, IDbTransaction dbTransaction = null)
        {
            userSecurityQuestion.ValidateAndThrow();

            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spUserSecurityQuestionUpdate";
            object parameter = new
            {
                UserSecurityQuestionId = userSecurityQuestion.UserSecurityQuestionId,
                UserReference = userSecurityQuestion.UserReference,
                QuestionText = userSecurityQuestion.QuestionText,
                AnswerHash = userSecurityQuestion.AnswerHash,
                FailedAnswerAttemptCount = userSecurityQuestion.FailedAnswerAttemptCount
            };
            try
            {
                 dbConnection.Execute(sp, parameter, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameter);
            }
        }

        public void Delete(int userSecurityQuestionId, Guid userReference, IDbTransaction dbTransaction = null)
        {
            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spUserSecurityQuestionDelete";
            object parameter = new
            {
                UserSecurityQuestionId = userSecurityQuestionId,
                UserReference = userReference
            };
            try
            {
                dbConnection.Execute(sp, parameter, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameter);
            }
        }
    }
}
