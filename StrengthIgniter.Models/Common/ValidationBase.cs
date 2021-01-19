using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.Common
{
    public abstract class ValidationBase
    {
        public virtual ModelValidationResult Validate()
        {
            // no validation logic, validation succeeds by default
            return new ModelValidationResult();
        }

        public bool IsValid()
        {
            ModelValidationResult validationResult = Validate();
            return validationResult.IsValid;
        }

        public void ValidateAndThrow()
        {
            ModelValidationResult validationResult = Validate();
            if (!validationResult.IsValid)
            {
                throw new ModelValidationException(validationResult.Errors);
            }
        }

    }
}
