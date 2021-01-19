using StrengthIgniter.Service.UserToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Registration
{
    public interface IRegistrationService
    {
        RegistrationResultEnum Register(RegisterRequest request);
        ValidateTokenResponseTypeEnum Validate(Guid tokenReference);
    }
}
