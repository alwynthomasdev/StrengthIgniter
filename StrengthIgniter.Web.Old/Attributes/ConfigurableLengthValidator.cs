using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web.Attributes
{
    public class ConfigurableMinLength : ValidationAttribute
    {
        private readonly string _Key;
        public ConfigurableMinLength(string key)
        {
            _Key = key;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IConfiguration configuration = validationContext.GetService<IConfiguration>();
            int.TryParse(configuration[_Key], out int minLength);

            if(value.ToString().Length < minLength)
            {
                return new ValidationResult($"Please enter at least {minLength} characters.");
            }

            return ValidationResult.Success;
        }
    }

    public class ConfigurableMaxLength : ValidationAttribute
    {
        private readonly string _Key;
        public ConfigurableMaxLength(string key)
        {
            _Key = key;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IConfiguration configuration = validationContext.GetService<IConfiguration>();
            int.TryParse(configuration[_Key], out int maxLength);

            if (value.ToString().Length > maxLength)
            {
                return new ValidationResult($"Please enter less than {maxLength} characters.");
            }

            return ValidationResult.Success;
        }
    }

}   
