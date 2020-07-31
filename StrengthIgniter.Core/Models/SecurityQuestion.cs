using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class SecurityQuestionModel
    {
        public int SecurityQuestionId { get; internal set; }
        public string QuestionText { get; internal set; }
    }

    //used for registration and user security question reset
    public class SecurityQuestionAnswerModel
    {
        //[Obsolete]//TODO: switch to question text
        //public int SecurityQuestionId { get; set; }
        public string QuestionText { get; set; }
        public string Answer { get; set; }

        public Guid? Reference { get; internal set; }
    }
    public class SecurityQuestionAnswerModelValidator : AbstractValidator<SecurityQuestionAnswerModel>
    {
        public SecurityQuestionAnswerModelValidator(int securityQuestionAnswerMinLength, int securityQuestionAnswerMaxLength)
        {
            RuleFor(x => x.QuestionText)
                .NotEmpty()
                .WithMessage("Security question is required.");

            RuleFor(x => x.Answer)
                .NotEmpty()
                .Length(securityQuestionAnswerMinLength, securityQuestionAnswerMaxLength)
                .WithMessage($"An answer to security questions must be between {securityQuestionAnswerMinLength} and {securityQuestionAnswerMaxLength} characters long.");
        }
    }
}
