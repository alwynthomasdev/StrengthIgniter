using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrengthIgniter.Models.Common
{
    public class ModelValidationResult
    {
        public readonly bool IsValid;
        public readonly IEnumerable<ModelValidationError> Errors;

        public ModelValidationResult()
        {
            // no validation result, no possible errors, result is valid
            // used for models that dont require validation logic
            IsValid = true;
        }

        public ModelValidationResult(FluentValidation.Results.ValidationResult result)
        {
            IsValid = result.IsValid;
            Errors = result.Errors.Select(failure =>
                new ModelValidationError
                {
                    ErrorMessage = failure.ErrorMessage,
                    PropertyName = failure.PropertyName,
                    ErrorCode = failure.ErrorCode
                }
            );
        }
    }
}
