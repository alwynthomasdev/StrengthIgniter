using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface IAuthenticationService
    {
        AuthenticationResponse Authenticate(AuthenticationRequest request);
    }

    public class AuthenticationService : ServiceBase
    {
        #region CTOR
        private readonly AuthenticationServiceConfig _Config;
        private readonly IUserDataAccess _UserDal;
        private readonly IHashUtility _HashUtility;
        private readonly IEmailUtility _EmailUtility;
        private readonly ITemplateUtility _TemplateUtility;

        public AuthenticationService(
            AuthenticationServiceConfig config,
            IUserDataAccess userDal,
            IHashUtility hashUtility,
            IEmailUtility emailUtility,
            ITemplateUtility templateUtility,
            //
            IAuditEventDataAccess auditEventDal,
            ILogger<AuthenticationService> logger,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDal, logger, dbConnectionFactory.GetConnection)
        {
            _Config = config;
            _UserDal = userDal;
            _HashUtility = hashUtility;
            _EmailUtility = emailUtility;
            _TemplateUtility = templateUtility;
        }
        #endregion

        public AuthenticationResponse Authenticate(AuthenticationRequest request)
        {
            try
            {
                UserModel user = _UserDal.SelectByEmailAddress(request.EmailAddress);
                if (user != null)
                {
                    bool passwordIsValid = _HashUtility.Validate(request.Password, user.PasswordHash);

                    if (!user.IsRegistrationValidated)
                    {
                        return new AuthenticationResponse { ResponseType = AuthenticationResponseType.AccountNotValidated };
                        //TODO: possibly send an email, reminding user to validate email, possibly resend validation email...
                    }

                    if (!UserAccountIsLockedOut(user))
                    {
                        int? accountLockedAuditEventId = null;
                        AuthenticationResponse response = new AuthenticationResponse { ResponseType = AuthenticationResponseType.LoginAttemptFailed };

                        using (IDbConnection dbConnection = GetConnection())
                        {
                            dbConnection.Open();
                            using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                            {
                                if (passwordIsValid)
                                {
                                    user.FailedLoginAttemptCount = 0;
                                    user.LockoutEndDateTimeUtc = null;
                                    user.LastLoginDateTimeUtc = DateTime.UtcNow;

                                    response = new AuthenticationResponse
                                    {
                                        ResponseType = AuthenticationResponseType.Success,
                                        UserReference = user.Reference,
                                        Name = user.Name,
                                        UserType = user.UserTypeCode,
                                        EmailAddress = user.EmailAddress
                                    };

                                    CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.LoginSuccess, user.UserId, "", null);
                                }
                                else// password is not valid
                                {
                                    user.FailedLoginAttemptCount = user.FailedLoginAttemptCount + 1;
                                    if (user.FailedLoginAttemptCount >= _Config.MaxFailedAuthenticateAttempts)
                                    {
                                        user.FailedLoginAttemptCount = 0;
                                        user.LockoutEndDateTimeUtc = DateTime.UtcNow.AddMinutes(_Config.AccountLockoutTimeSpanMinutes);

                                        response = new AuthenticationResponse { ResponseType = AuthenticationResponseType.AccountLocked };
                                        accountLockedAuditEventId = CreateAuditEvent(AuditEventType.AccountLocked, user.UserId, "", null);
                                    }
                                }
                                _UserDal.UpdateLoginAttempt(dbConnection, dbTransaction, user);

                                dbTransaction.Commit();

                            }// end using db transaction
                        }// end using db connection

                        //if the account has just been locked out, send email (do this outside transaction scope)
                        if (accountLockedAuditEventId.HasValue)
                        {
                            SendAccountLockedEmail(user, accountLockedAuditEventId.Value);
                        }

                        return response;
                    }
                }
                else//if no user, always run the hash validator (computationally expensive)
                {
                    _HashUtility.Validate("RunFakeHashValidator", _HashUtility.GenerateFakeHash());
                }

                return new AuthenticationResponse { ResponseType = AuthenticationResponseType.LoginAttemptFailed };
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        #region Helpers

        private bool UserAccountIsLockedOut(UserModel user)
        {
            if (user.LockoutEndDateTimeUtc.HasValue)
            {
                if (DateTime.UtcNow < user.LockoutEndDateTimeUtc.Value)
                {
                    return true;
                }
            }
            return false;
        }

        private void SendAccountLockedEmail(UserModel user, int relatedAuditEventId)
        {
            string messageBody = _TemplateUtility.Parse(_Config.AccountLockoutEmailTemplatePath, new
            {
                Subject = _Config.AccountLockoutEmailSubject,
                Username = user.Name,
                LockoutMinutes = _Config.AccountLockoutTimeSpanMinutes
            });

            EmailMessageModel msg = new EmailMessageModel
            {
                To = new string[] { user.EmailAddress },
                Subject = _Config.AccountLockoutEmailSubject,
                Body = messageBody,
                IsHtml = true,
            };

            _EmailUtility.Send(msg);
            CreateEmailAuditEvent(msg, user.UserId, relatedAuditEventId);
        }

        #endregion

    }


    #region AuthenticationService Models

    public class AuthenticationServiceConfig
    {
        public int MaxFailedAuthenticateAttempts { get; set; }
        public int AccountLockoutTimeSpanMinutes { get; set; }

        //Account lockout email settings
        public string AccountLockoutEmailSubject { get; set; }
        public string AccountLockoutEmailTemplatePath { get; set; }
    }

    public sealed class AuthenticationRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public sealed class AuthenticationResponse
    {
        //return relevant login data
        public AuthenticationResponseType ResponseType { get; internal set; }
        public Guid UserReference { get; internal set; }
        public string Name { get; internal set; }
        public string EmailAddress { get; internal set; }
        public UserType UserType { get; set; }
    }

    public enum AuthenticationResponseType
    {
        Success = 1,
        LoginAttemptFailed = -1,
        AccountLocked = -2,
        AccountNotValidated = -3
    }

    #endregion

}
