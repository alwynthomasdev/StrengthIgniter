using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.UserSecurityQuestion
{
    internal class UserSecurityQuestionModelValidator : AbstractValidator<UserSecurityQuestionModel>
    {
        public UserSecurityQuestionModelValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty()
                .WithMessage("Reference is required.")
                .WithErrorCode("Reference.Required");

            RuleFor(x => x.UserReference)
                .NotEmpty()
                .WithMessage("UserReference is required.")
                .WithErrorCode("UserReference.Required");

            RuleFor(x => x.QuestionText)
                .NotEmpty()
                .WithMessage("QuestionText is required.")
                .WithErrorCode("QuestionText.Required");

            RuleFor(x => x.QuestionText)
                .MaximumLength(1000)
                .WithMessage("QuestionText can be no more than 1000 characters long..")
                .WithErrorCode("QuestionText.MaximumLength.0.1000");

            RuleFor(x => x.AnswerHash)
                .NotEmpty()
                .WithMessage("AnswerHash is required.")
                .WithErrorCode("AnswerHash.Required");
        }
    }
}
