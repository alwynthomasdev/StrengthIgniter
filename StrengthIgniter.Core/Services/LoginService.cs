using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface ILoginService
    {
        LoginResponse Login(LoginRequest request);
    }

    public class LoginService
    {

    }

    #region LoginService Models

    public class LoginServiceConfig
    {
        public int MaxFailedAttempts { get; set; }
        public int LockoutTimeSpanMinuets { get; set; }

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
        //TODO: return relevant login data
        public LoginResponseType ResponseType { get; internal set; }
        public Guid UserUniqueId { get; internal set; }
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
