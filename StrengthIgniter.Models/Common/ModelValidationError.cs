using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.Common
{
    public class ModelValidationError
    {
        public string PropertyName { get; internal set; }
        public string ErrorCode { get; internal set; }
        public string ErrorMessage { get; internal set; }
    }
}
