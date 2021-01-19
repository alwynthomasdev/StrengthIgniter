using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.UserToken
{
    public enum ValidateTokenResponseTypeEnum
    {
        Valid = 1,
        NotFound = -1,
        Expired = -2,
        InvalidPurpose = -3
    }
}
