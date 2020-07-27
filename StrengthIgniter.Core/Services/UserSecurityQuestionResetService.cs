using FluentValidation;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface IUserSecurityQuestionResetService
    {
        IEnumerable<UserSecurityQuestionAnswerModel> GetFailedQuestionsForUser(Guid userReference);
        void UpdateSecretQuestions(Guid userReference, IEnumerable<SecurityQuestionAnswerModel> questions);
        IEnumerable<SecurityQuestionModel> GetSecurityQuestions();
        /*
         * TODO: 
         *  Get questions that need resetting for user
         *  Reset questions
         */
    }

    public class UserSecurityQuestionResetService : ServiceBase, IUserSecurityQuestionResetService
    {
        #region CTOR
        private readonly UserSecurityQuestionResetServiceConfig _Config;
        private readonly IUserDataAccess _UserDal;
        private readonly IHashUtility _HashUtility;
        private readonly IEnumerable<SecurityQuestionModel> _SecurityQuestions;

        public UserSecurityQuestionResetService(
            UserSecurityQuestionResetServiceConfig config,
            IUserDataAccess userDal,
            ISecurityQuestionDataAccess securityQuestionDal,
            IHashUtility hashUtility,
            //
            IAuditEventDataAccess auditEventDal,
            ILoggerFactory loggerFactory,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDal, loggerFactory.CreateLogger(typeof(UserSecurityQuestionResetService)), dbConnectionFactory.GetConnection)
        {
            _Config = config;
            _UserDal = userDal;
            _HashUtility = hashUtility;

            _SecurityQuestions = securityQuestionDal.GetQuestions();
        }
        #endregion

        public IEnumerable<UserSecurityQuestionAnswerModel> GetFailedQuestionsForUser(Guid userReference)
        {
            try
            {
                return _UserDal.GetFailedQuestionsForUser(userReference);
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { userReference = userReference });
                throw serviceException;
            }
        }

        public void UpdateSecretQuestions(Guid userReference, IEnumerable<SecurityQuestionAnswerModel> questions)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        UserModel user = _UserDal.GetByReference(userReference);
                        if (user != null)
                        {
                            List<AuditEventItemModel> auditItems = new List<AuditEventItemModel>();

                            foreach (SecurityQuestionAnswerModel question in questions)
                            {
                                ValidateSecurityQuestion(question);
                                if(user.SecurityQuestions.Where(x=>x.Reference == question.Reference).SingleOrDefault() == null)
                                {
                                    throw new Exception($"Security question with reference '{question.Reference}' does not belong to user with reference '{userReference}'.");
                                }

                                UserSecurityQuestionAnswerModel userSecurityQuestionAnswerModel = new UserSecurityQuestionAnswerModel
                                {
                                    Reference = question.Reference.Value,
                                    QuestionText = GetSecurityQuestionTextById(question.SecurityQuestionId),
                                    AnswerHash = _HashUtility.Generate(question.Answer),
                                    FailedAnswerAttemptCount = null
                                };

                                auditItems.Add(new AuditEventItemModel { Key = "UserSecurityQuestionAnswerReference", Value = question.Reference.ToString() });
                            }
                            
                            CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.SecurityQuestionsUpdated, user.UserId, "", auditItems);
                            dbTransaction.Commit();
                        }
                        else
                        {
                            throw new Exception($"No user found with reference '{userReference}'.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, questions);
                throw serviceException;
            }
        }

        public IEnumerable<SecurityQuestionModel> GetSecurityQuestions()
        {
            try
            {
                return _SecurityQuestions;
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, null);
                throw serviceException;
            }
        }

        #region Private Methods

        private void ValidateSecurityQuestion(SecurityQuestionAnswerModel question)
        {
            SecurityQuestionAnswerModelValidator validator = new SecurityQuestionAnswerModelValidator(_Config.SecretQuestionAnswerMinLength, _Config.SecretQuestionAnswerMaxLength);
            validator.ValidateAndThrow(question);
            if(!question.Reference.HasValue)
            {
                throw new ValidationException("Cannot update user security question answer without reference");
            }
        }

        private string GetSecurityQuestionTextById(int securityQuestionId)
        {
            SecurityQuestionModel securityQuestion = GetSecurityQuestionById(securityQuestionId);
            if (securityQuestion == null)
            {
                throw new Exception($"No security question found with id '{securityQuestionId}'.");
            }
            return securityQuestion.Question;
        }

        private SecurityQuestionModel GetSecurityQuestionById(int id)
        {
            return _SecurityQuestions.Where(q => q.SecurityQuestionId == id).SingleOrDefault();
        }

        #endregion

    }

    #region Models

    public class UserSecurityQuestionResetServiceConfig
    {
        //public int NumberOfSecretQuestionsRequired { get; set; }
        public int SecretQuestionAnswerMinLength { get; set; }
        public int SecretQuestionAnswerMaxLength { get; set; }
    }

    #endregion

}
