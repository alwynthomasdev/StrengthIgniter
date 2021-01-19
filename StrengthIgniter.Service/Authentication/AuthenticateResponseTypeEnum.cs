using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Authentication
{
    public enum AuthenticateResponseTypeEnum
    {
        Success = 1,
        Failed = -1,
        Locked = -2
    }
}
