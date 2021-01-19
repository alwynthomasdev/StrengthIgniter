using Microsoft.Extensions.Logging;
using StrengthIgniter.Dal.AuditEvent;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Dal.User;
using StrengthIgniter.EmailTemplates.Models;
using StrengthIgniter.Models.AuditEvent;
using StrengthIgniter.Models.User;
using StrengthIgniter.Service.Common;
using StrengthIgniter.Service.Email;
using StrengthIgniter.Service.Hash;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Authentication
{
    public class AuthenticationService : DataServiceBase, IAuthenticationService
    {
        private readonly AuthenticationServiceConfig _Config;
        private readonly IEmailService _EmailService;
        private readonly IHashService _HashService;
        private readonly IUserDataAccess _UserDataAccess;

        public AuthenticationService(
            AuthenticationServiceConfig config,
            IEmailService emailService,
            IHashService hashService,
            IUserDataAccess userDataAccess,
            //
            IDataAccessTransactionProvider transactionProvider,
            IAuditEventDataAccess auditEventDataAccess,
            //
            ILogger logger
        )
            : base(transactionProvider, auditEventDataAccess, logger)
        {
            _Config = config;
            _EmailService = emailService;
            _HashService = hashService;
            _UserDataAccess = userDataAccess;
        }

        public AuthenticateResponse Authenticate(string emailAddress, string password)
        {
            try
            {
                // failed by default
                AuthenticateResponse response = new AuthenticateResponse { ResponseType = AuthenticateResponseTypeEnum.Failed };

                UserModel user = _UserDataAccess.Select(emailAddress);
                if(user != null)
                {
                    // audit event type depends on password validation result
                    string auditEventType = "";

                    if (_HashService.Validate(password, user.PasswordHash))
                    {
                        // reset any failed login attempts
                        user.FailedLoginAttemptCount = 0;
                        user.LockoutEndDateTimeUtc = DateTime.UtcNow;

                        // return user data
                        response.UserReference = user.Reference;
                        response.Username = user.Name;
                        response.UserType = user.UserTypeCode;
                        response.ResponseType = AuthenticateResponseTypeEnum.Success;

                        auditEventType = AuditEventTypeConstants.UserAuthenticationSuccess;
                    }
                    else // password incorect, increment lockout
                    {
                        user.FailedLoginAttemptCount = user.FailedLoginAttemptCount + 1;
                        // if the max login attempts have been reached, lock the account for 'n' minuets
                        if(user.FailedLoginAttemptCount >= _Config.MaxFailedLoginAttemptCount)
                        {
                            user.FailedLoginAttemptCount = 0;
                            user.LockoutEndDateTimeUtc = DateTime.UtcNow.AddMinutes(_Config.LockoutMinuets);

                            // return account locked as response type
                            response.ResponseType = AuthenticateResponseTypeEnum.Locked;
                            auditEventType = AuditEventTypeConstants.UserAuthenticationLocked;
                        }
                    }

                    // update the user data with above changes
                    using (IDataAccessTransaction trn = BeginTansaction())
                    {
                        _UserDataAccess.Update(user, trn.DbTransaction);
                        CreateAuditEvent(trn, auditEventType, "", null, user.Reference);
                    }

                    // if the login attempt resulted in a lockout, email the user
                    if(response.ResponseType == AuthenticateResponseTypeEnum.Locked)
                    {
                        SendAccountLockedEmail(user);
                    }
                }
                else // no user found with email address
                {
                    // run encryption so there is no time difference
                    _HashService.Validate(password, _HashService.GenerateFakeHash());
                }

                return response;
            }
            catch (ServiceException)
            {
                throw;
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, "Authenticate", new { emailAddress, password="*****" });
                throw serviceException;
            }
        }

        private void SendAccountLockedEmail(UserModel user)
        {

            UserAccountLockedTemplateModel template = new UserAccountLockedTemplateModel
            {
                Title = "Account Locked",
                LockoutMinuets = _Config.LockoutMinuets,
                Username = user.Name
            };
            _EmailService.SendEmail(new SendEmailRequest
            {
                ToAddress = user.EmailAddress,
                Subject = template.Title,
                Template = template,
                UserReference = user.Reference
            });
        }

    }
}
