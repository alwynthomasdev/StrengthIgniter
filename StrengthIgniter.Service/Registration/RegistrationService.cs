using Microsoft.Extensions.Logging;
using StrengthIgniter.Dal.AuditEvent;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Dal.User;
using StrengthIgniter.Dal.UserSecurityQuestion;
using StrengthIgniter.EmailTemplates.Models;
using StrengthIgniter.Models.AuditEvent;
using StrengthIgniter.Models.User;
using StrengthIgniter.Models.UserSecurityQuestion;
using StrengthIgniter.Models.UserToken;
using StrengthIgniter.Service.Common;
using StrengthIgniter.Service.Email;
using StrengthIgniter.Service.Hash;
using StrengthIgniter.Service.UserToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Registration
{
    public class RegistrationService : DataServiceBase, IRegistrationService
    {
        private readonly RegistrationServiceConfig _Config;
        private readonly IUserTokenService _UserTokenService;
        private readonly IEmailService _EmailService;
        private readonly IHashService _HashService;
        private readonly IUserDataAccess _UserDataAccess;
        private readonly IUserSecurityQuestionDataAccess _UserSecurityQuestionDataAccess;

        public RegistrationService(
            RegistrationServiceConfig config,
            IUserTokenService userTokenService,
            IEmailService emailService,
            IHashService hashService,
            IUserDataAccess userDataAccess,
            IUserSecurityQuestionDataAccess userSecurityQuestionDataAccess,
            //
            IDataAccessTransactionProvider transactionProvider,
            IAuditEventDataAccess auditEventDataAccess,
            //
            ILogger logger
        )
            : base(transactionProvider, auditEventDataAccess, logger)
        {
            _UserTokenService = userTokenService;
            _EmailService = emailService;
            _HashService = hashService;
            _UserDataAccess = userDataAccess;
            _UserSecurityQuestionDataAccess = userSecurityQuestionDataAccess;
        }

        public RegistrationResultEnum Register(RegisterRequest request)
        {
            try
            {
                request.ValidateAndThrow();
                UserModel existingUser = _UserDataAccess.Select(request.EmailAddress);

                if (existingUser == null)
                {
                    UserModel newUser = CreateUserModel(request);
                    IEnumerable<UserSecurityQuestionModel> userSecurityQuestions = CreateSecurityQuestionModelList(newUser.Reference, request.SecurityQuestionAnswer);

                    int registrationAuditId = 0;
                    Guid validateRegistrationToken = Guid.Empty;

                    using (IDataAccessTransaction trn = BeginTansaction())
                    {
                        _UserDataAccess.Insert(newUser, trn.DbTransaction);
                        foreach(UserSecurityQuestionModel userSecurityQuestion in userSecurityQuestions)
                        {
                            _UserSecurityQuestionDataAccess.Insert(userSecurityQuestion, trn.DbTransaction);
                        }
                        validateRegistrationToken = _UserTokenService.Create(newUser.Reference, UserTokenPurposeConstants.Registration, trn);
                        registrationAuditId = CreateAuditEvent(
                            trn, AuditEventTypeConstants.UserRegistration, 
                            "", null, newUser.Reference);

                        trn.Commit();
                    }

                    SendValidateRegistrationEmail(newUser, validateRegistrationToken, registrationAuditId);
                    return RegistrationResultEnum.Success;
                }
                else // user already exists
                {
                    SendAccountExistsEmail(existingUser);
                    return RegistrationResultEnum.UserExists;
                }
            }
            catch (ServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, "Register", request);
                throw serviceException;
            }
        }

        public ValidateTokenResponseTypeEnum Validate(Guid tokenReference)
        {
            try
            {
                ValidateTokenResponse validateTokenResponse = _UserTokenService.Validate(tokenReference, UserTokenPurposeConstants.Registration);

                UserModel user = _UserDataAccess.Select(validateTokenResponse.UserReference);
                user.IsRegistrationValidated = true;
                
                using(IDataAccessTransaction trn = BeginTansaction())
                {
                    _UserDataAccess.Update(user, trn.DbTransaction);
                    CreateAuditEvent(trn, AuditEventTypeConstants.UserRegistrationValidated,
                        "", null, user.Reference);
                }

                return validateTokenResponse.ResponseTpe;
            }
            catch(ServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, "Validate", new { tokenReference });
                throw serviceException;
            }
        }

        // Private Methods

        private UserModel CreateUserModel(RegisterRequest request)
        {
            return new UserModel
            {
                Reference = Guid.NewGuid(),
                EmailAddress = request.EmailAddress,
                PasswordHash = _HashService.Generate(request.Password),
                Name = request.FullName,
                //
                UserTypeCode = UserTypeEnum.Basic,
                RegisteredDateTimeUtc = DateTime.UtcNow,
                //
                FailedLoginAttemptCount = 0,
                IsRegistrationValidated = false,
                LastLoginDateTimeUtc = null,
                LockoutEndDateTimeUtc = null,
            };
        }

        private IEnumerable<UserSecurityQuestionModel> CreateSecurityQuestionModelList(Guid userReference, IEnumerable<SecurityQuestionAnswer> securityQuestionAnswers)
        {
            List<UserSecurityQuestionModel> lstUserSecurityQuestionModel = new List<UserSecurityQuestionModel>();

            foreach(SecurityQuestionAnswer securityQuestionAnswer in securityQuestionAnswers)
                lstUserSecurityQuestionModel.Add(CreateSecurityQuestionModel(userReference, securityQuestionAnswer));

            return lstUserSecurityQuestionModel;
        }

        private UserSecurityQuestionModel CreateSecurityQuestionModel(Guid userReference, SecurityQuestionAnswer securityQuestionAnswer)
        {
            return new UserSecurityQuestionModel
            {
                Reference = Guid.NewGuid(),
                UserReference = userReference,
                QuestionText = securityQuestionAnswer.Question,
                AnswerHash = _HashService.Generate(securityQuestionAnswer.Answer),
                //
                FailedAnswerAttemptCount = 0
            };
        }

        private void SendAccountExistsEmail(UserModel existingUser)
        {
            UserAccountExistsTemplateModel template = new UserAccountExistsTemplateModel
            {
                Title = "Account Exists",
                PasswordResetUrl = _Config.PasswordResetUrl
            };
            _EmailService.SendEmail(new SendEmailRequest {
                ToAddress = existingUser.EmailAddress,
                Subject = template.Title,
                Template = template,
                UserReference = existingUser.Reference
            });
        }

        private void SendValidateRegistrationEmail(UserModel user, Guid token, int auditId)
        {
            UserRegistrationValidationTemplateModel template = new UserRegistrationValidationTemplateModel
            {
                Title = "Validate Registration",
                Username = user.Name,
                ValidateRegistrationUrl = string.Format(_Config.PasswordResetUrl, token),
            };

            _EmailService.SendEmail(new SendEmailRequest { 
                ToAddress = user.EmailAddress,
                Subject = template.Title,
                Template = template,
                UserReference = user.Reference,
                RelatedAuditEventId = auditId
            });
        }

    }
}
