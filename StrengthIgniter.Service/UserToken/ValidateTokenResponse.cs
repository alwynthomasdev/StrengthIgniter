using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.UserToken
{
    public class ValidateTokenResponse
    {
        public ValidateTokenResponseTypeEnum ResponseTpe { get; internal set; }
        public Guid UserReference { get; internal set; }
    }

    

}
