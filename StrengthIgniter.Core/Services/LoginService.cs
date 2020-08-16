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
    public interface ILoginService
    {
        LoginResponse Login(LoginRequest request);
    }

    public class LoginService : ServiceBase, ILoginService
    {
        #region CTOR
        private readonly LoginServiceConfig _Config;
        private readonly IUserDataAccess _UserDal;
        private readonly IHashUtility _HashUtility;
        private readonly IEmailUtility _EmailUtility;
        private readonly ITemplateUtility _TemplateUtility;

        public LoginService(
            LoginServiceConfig config,
            IUserDataAccess userDal,
            IHashUtility hashUtility,
            IEmailUtility emailUtility,
            ITemplateUtility templateUtility,
            //
            IAuditEventDataAccess auditEventDal,
            ILogger<LoginService> logger,
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

        public LoginResponse Login(LoginRequest request)
        {
            try
            {
                UserModel user = _UserDal.GetByEmailAddress(request.EmailAddress);
                if (user != null)
                {
                    bool passwordIsValid = _HashUtility.Validate(request.Password, user.PasswordHash);

                    if (!user.IsRegistrationValidated)
                    {
                        return new LoginResponse { ResponseType = LoginResponseType.AccountNotValidated };
                        //TODO: possibly send an email, reminding user to validate email, possibly resend validation email...
                    }

                    if (!UserAccountIsLockedOut(user))
                    {
                        int? accountLockedAuditEventId = null;
                        LoginResponse response = new LoginResponse { ResponseType = LoginResponseType.LoginAttemptFailed };

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

                                    response = new LoginResponse { 
                                        ResponseType = LoginResponseType.Success, 
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
                                    if (user.FailedLoginAttemptCount >= _Config.MaxFailedAttempts)
                                    {
                                        user.FailedLoginAttemptCount = 0;
                                        user.LockoutEndDateTimeUtc = DateTime.UtcNow.AddMinutes(_Config.LockoutTimeSpanMinutes);

                                        response = new LoginResponse { ResponseType = LoginResponseType.AccountLocked };
                                        accountLockedAuditEventId = CreateAuditEvent(AuditEventType.AccountLocked, user.UserId, "", null);
                                    }
                                }
                                _UserDal.UpdateUserLoginAttempt(dbConnection, dbTransaction, user);

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
                else// always run the hash validator (computationally expensive)
                {
                    _HashUtility.Validate("RunFakeHashValidator", _HashUtility.GenerateFakeHash());
                }

                return new LoginResponse { ResponseType = LoginResponseType.LoginAttemptFailed };
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        #region Private Methods

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
                LockoutMinutes = _Config.LockoutTimeSpanMinutes
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

    #region LoginService Models

    public class LoginServiceConfig
    {
        public int MaxFailedAttempts { get; set; }
        public int LockoutTimeSpanMinutes { get; set; }

        public string AccountLockoutEmailSubject { get; set; }
        public string AccountLockoutEmailTemplatePath { get; set; }
    }

    public sealed class LoginRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public sealed class LoginResponse
    {
        //return relevant login data
        public LoginResponseType ResponseType { get; internal set; }
        public Guid UserReference { get; internal set; }
        public string Name { get; internal set; }
        public string EmailAddress { get; internal set; }
        public UserType UserType { get; set; }
    }

    public enum LoginResponseType
    {
        Success = 1,
        LoginAttemptFailed = -1,
        AccountLocked = -2,
        AccountNotValidated = -3
    }

    #endregion

}
