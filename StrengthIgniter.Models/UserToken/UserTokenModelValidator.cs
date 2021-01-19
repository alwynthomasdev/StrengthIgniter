using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.UserToken
{
    internal class UserTokenModelValidator : AbstractValidator<UserTokenModel>
    {
        public UserTokenModelValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty()
                .WithMessage("Reference is required.")
                .WithErrorCode("Reference.Required");

            RuleFor(x => x.UserReference)
                .NotEmpty()
                .WithMessage("UserReference is required.")
                .WithErrorCode("UserReference.Required");

            RuleFor(x => x.PurposeCode)
                .NotNull()
                .WithMessage("PurposeCode is required.")
                .WithErrorCode("PurposeCode.Required");

            RuleFor(x => x.IssuedDateTimeUtc)
                .NotEmpty()
                .WithMessage("IssuedDateTimeUtc is required.")
                .WithErrorCode("IssuedDateTimeUtc.Required");

            RuleFor(x => x.ExpiryDateTimeUtc)
                .NotEmpty()
                .WithMessage("ExpiryDateTimeUtc is required.")
                .WithErrorCode("ExpiryDateTimeUtc.Required");
        }
    }
}
