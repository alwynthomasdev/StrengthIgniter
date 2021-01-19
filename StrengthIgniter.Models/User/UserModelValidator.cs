using FluentValidation;

namespace StrengthIgniter.Models.User
{
    internal class UserModelValidator : AbstractValidator<UserModel>
    {
        public UserModelValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty()
                .WithMessage("Reference is required.")
                .WithErrorCode("Reference.Required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .WithErrorCode("Name.Required");

            RuleFor(x => x.Name)
               .Length(3, 256)
               .WithMessage("Name must be between 3 and 256 characters.")
               .WithErrorCode("Name.Length.3.256");

            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage("EmailAddress is required.")
                .WithErrorCode("EmailAddress.Required");

            RuleFor(x => x.EmailAddress)
                .EmailAddress()
                .WithMessage("EmailAddress must be a valid email address.")
                .WithErrorCode("EmailAddress.Invalid");

            RuleFor(x => x.PasswordHash)
                .NotEmpty()
                .WithMessage("PasswordHash is required.")
                .WithErrorCode("PasswordHash.Required");

            RuleFor(x => x.UserTypeCode)
                .NotNull()
                .WithMessage("UserTypeCode is required.")
                .WithErrorCode("UserTypeCode.Required");
        }
    }
}
