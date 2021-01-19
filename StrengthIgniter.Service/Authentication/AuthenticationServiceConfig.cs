using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Authentication
{
    public class AuthenticationServiceConfig
    {
        public int MaxFailedLoginAttemptCount { get; set; }
        public int LockoutMinuets { get; set; }
    }
}
