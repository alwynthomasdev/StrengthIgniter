﻿using FluentValidation;
using StrengthIgniter.Core.Data;
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
    public interface IRegistrationService
    {
        RegistrationResponseType Register(RegistrationModel request);
    }

    public class RegistrationService : ServiceBase
    {
        #region CTOR
        private readonly RegistrationServiceConfig _Config;
        private readonly IUserDataAccess _UserDal;
        private readonly IHashUtility _HashUtility;
        private readonly IEmailUtility _EmailUtility;
        private readonly ITemplateUtility _TemplateUtility;
        private readonly IEnumerable<SecurityQuestionModel> _SecurityQuestions;

        public RegistrationService(
            RegistrationServiceConfig config,
            IUserDataAccess userDal,
            ISecurityQuestionDataAccess securityQuestionDal,
            IHashUtility hashUtility,
            IEmailUtility emailUtility,
            ITemplateUtility templateUtility,
            //
            IAuditEventDataAccess auditEventDal,
            ILogUtility logger,
            Func<IDbConnection> fnGetConnection
        )
            : base(auditEventDal, logger, fnGetConnection)
        {
            _Config = config;
            _UserDal = userDal;
            _HashUtility = hashUtility;
            _EmailUtility = emailUtility;
            _TemplateUtility = templateUtility;

            _SecurityQuestions = securityQuestionDal.GetQuestions();
        }
        #endregion

        public RegistrationResponseType Register(RegistrationModel registrationModel)
        {
            try
            {
                ValidateRegistrationModel(registrationModel);
                UserModel user = _UserDal.GetByEmailAddress(registrationModel.EmailAddress);
                if (user == null)
                {
                    user = ConvertRegistrationToUser(registrationModel);
                    UserTokenModel tkn = CreateRegistrationToken();
                    user.Tokens = new UserTokenModel[] { tkn };

                    int auditId = 0;

                    using (IDbConnection dbConnection = GetConnection())
                    {
                        using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                        {
                            int newUserId = _UserDal.CreateNewUser(dbConnection, dbTransaction, user);
                            auditId = CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.NewUserRegistration, newUserId, "", null);

                            dbTransaction.Commit();
                        }
                    }

                    string url = string.Format(_Config.ValidateRegistrationBaseUrl, tkn.Reference);
                    SendRegistrationValidationEmail(user, url, auditId);

                    return RegistrationResponseType.Success;
                }
                else// user with email already exists
                {
                    SendAccountExistsEmail(user);
                    return RegistrationResponseType.Exists;
                }
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, registrationModel);
                throw serviceException;
            }
        }

        #region Private Methods

        private void ValidateRegistrationModel(RegistrationModel model)
        {
            RegistrationModelValidatorConfig config = new RegistrationModelValidatorConfig
            {
                PassswordMinLength = _Config.PasswordMaxLength,
                PasswordMaxLength = _Config.PasswordMaxLength,
                NumberOfSecurityQuestionsRequired = _Config.NumberOfSecretQuestionsRequired,
                SecurityQuestionAnswerMinLength = _Config.SecretQuestionAnswerMinLength,
                SecurityQuestionAnswerMaxLength = _Config.SecretQuestionAnswerMaxLength
            };
            RegistrationModelValidator validatior = new RegistrationModelValidator(config);
            validatior.ValidateAndThrow(model);
            ValidateSsecurityQuestionIds(model.SecurityQuestionAnswers);
        }
        private void ValidateSsecurityQuestionIds(IEnumerable<SecurityQuestionAnswerModel> securityQuestionAnswers)
        {
            foreach (SecurityQuestionAnswerModel securityQuestionAnswer in securityQuestionAnswers)
            {
                SecurityQuestionModel securityQuestion = GetSecurityQuestionById(securityQuestionAnswer.SecurityQuestionId);
                if (securityQuestion == null)
                {
                    throw new ValidationException($"No security question found with id '{securityQuestionAnswer.SecurityQuestionId}'.");
                }
            }
        }

        private UserModel ConvertRegistrationToUser(RegistrationModel registration)
        {
            UserModel newUser = new UserModel
            {
                Reference = Guid.NewGuid(),
                Name = registration.Name,
                EmailAddress = registration.EmailAddress,
                PasswordHash = _HashUtility.Generate(registration.Password),
                RegisteredDateTimeUtc = DateTime.UtcNow
            };

            List<UserSecurityQuestionAnswerModel> userSecretQuestionAnswers = new List<UserSecurityQuestionAnswerModel>();
            foreach (SecurityQuestionAnswerModel qa in registration.SecurityQuestionAnswers)
            {
                SecurityQuestionModel securityQuestion = GetSecurityQuestionById(qa.SecurityQuestionId);
                if (securityQuestion == null)
                {
                    throw new Exception($"No security question found with id '{qa.SecurityQuestionId}'.");
                }

                userSecretQuestionAnswers.Add(new UserSecurityQuestionAnswerModel
                {
                    Reference = Guid.NewGuid(),
                    AnswerHash = _HashUtility.Generate(qa.Answer),
                    QuestionText = securityQuestion.Question,
                });
            }
            newUser.SecurityQuestions = userSecretQuestionAnswers;

            return newUser;
        }

        private UserTokenModel CreateRegistrationToken()
        {
            return new UserTokenModel
            {
                Reference = Guid.NewGuid(),
                PurposeCode = "Registration",
                IssuedDateTimeUtc = DateTime.UtcNow,
                ExpiryDateTimeUtc = DateTime.UtcNow.AddHours(_Config.RegistrationTokenExpiryHours)
            };
        }

        private void SendRegistrationValidationEmail(UserModel  user, string url, int relatedAuditId)
        {
            string messageBody = _TemplateUtility.Parse(_Config.RegistrationEmailTemplatePath, new
            {
                Username = user.Name,
                Url = url
            });

            EmailMessageModel msg = new EmailMessageModel
            {
                To = new string[] { user.EmailAddress },
                Subject = _Config.RegistrationEmailSubject,
                Body = messageBody,
                IsHtml = true,
            };

            _EmailUtility.Send(msg);
            CreateEmailAuditEvent(msg, user.UserId, relatedAuditId);
        }

        private void SendAccountExistsEmail(UserModel user)
        {
            string messageBody = _TemplateUtility.Parse(_Config.AccountExistsEmailTemplatePath, new
            {
                Username = user.Name,
            });

            EmailMessageModel msg = new EmailMessageModel
            {
                To = new string[] { user.EmailAddress },
                Subject = _Config.AccountExistsEmailSubject,
                Body = messageBody,
                IsHtml = true,
            };

            _EmailUtility.Send(msg);
            CreateEmailAuditEvent(msg, user.UserId);
        }

        private SecurityQuestionModel GetSecurityQuestionById(int id)
        {
            return _SecurityQuestions.Where(q => q.SecurityQuestionId == id).SingleOrDefault();
        }

        #endregion

    }

    #region Models

    public class RegistrationServiceConfig
    {
        //validation config
        public int PassswordMinLength { get; set; }
        public int PasswordMaxLength { get; set; }
        public int NumberOfSecretQuestionsRequired { get; set; }
        public int SecretQuestionAnswerMinLength { get; set; }
        public int SecretQuestionAnswerMaxLength { get; set; }

        //email verification config
        public int RegistrationTokenExpiryHours { get; set; }
        public string RegistrationEmailTemplatePath { get; set; }
        public string RegistrationEmailSubject { get; set; }
        public string ValidateRegistrationBaseUrl { get; set; }
        //
        public string AccountExistsEmailTemplatePath { get; set; }
        public string AccountExistsEmailSubject { get; set; }
    }

    public enum RegistrationResponseType
    {
        Success = 1,
        Exists = 0,
    }

    #endregion

}
