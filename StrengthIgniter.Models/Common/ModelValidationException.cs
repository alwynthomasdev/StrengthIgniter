using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrengthIgniter.Models.Common
{
    public class ModelValidationException : Exception
    {
        public readonly IEnumerable<ModelValidationError> _Errors;

        public IEnumerable<ModelValidationError> Errors
        {
            get
            {
                return _Errors;
            }
        }

        public ModelValidationException(IEnumerable<ModelValidationError> errors)
        {
            _Errors = errors;
        }
    }
}

