using StrengthIgniter.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Registration
{
    public class RegisterRequest : RequestBase
    {
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }

        public IEnumerable<SecurityQuestionAnswer> SecurityQuestionAnswer { get; set; }
    }
}
