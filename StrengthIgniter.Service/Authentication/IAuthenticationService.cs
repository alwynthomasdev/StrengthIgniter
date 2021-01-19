using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Authentication
{
    public interface IAuthenticationService
    {
        AuthenticateResponse Authenticate(string emailAddress, string password);
    }
}
