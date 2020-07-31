using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace StrengthIgniter.Core.Models
{
    public class RegistrationModel
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public List<SecurityQuestionAnswerModel> SecurityQuestionAnswers { get; set; }
    }

    public class RegistrationModelValidatorConfig
    {
        public int PasswordMinLength { get; set; }
        public int PasswordMaxLength { get; set; }
        public int NumberOfSecurityQuestionsRequired { get; set; }
        public int SecurityQuestionAnswerMinLength { get; set; }
        public int SecurityQuestionAnswerMaxLength { get; set; }
    }

    public class RegistrationModelValidator : AbstractValidator<RegistrationModel>
    {
        public RegistrationModelValidator(RegistrationModelValidatorConfig validatorConfig)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 256)
                .WithMessage("PersonalName must be between 3 and 256 characters.");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("EmailAddress must be a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(validatorConfig.PasswordMinLength, validatorConfig.PasswordMaxLength)
                .WithMessage($"Password must be between {validatorConfig.PasswordMinLength} and {validatorConfig.PasswordMaxLength} characters.");

            RuleFor(x => x.SecurityQuestionAnswers)
                .Must(x => x.Count == validatorConfig.NumberOfSecurityQuestionsRequired)
                .WithMessage($"{validatorConfig.NumberOfSecurityQuestionsRequired} secret questions are required.");

            RuleForEach(x => x.SecurityQuestionAnswers)
                .SetValidator(new SecurityQuestionAnswerModelValidator(validatorConfig.SecurityQuestionAnswerMinLength, validatorConfig.SecurityQuestionAnswerMaxLength));
        }
    }
}