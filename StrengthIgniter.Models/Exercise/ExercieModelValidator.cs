using FluentValidation;

namespace StrengthIgniter.Models.Exercise
{
    internal class ExercieModelValidator : AbstractValidator<ExerciseModel>
    {
        public ExercieModelValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty()
                .WithMessage("Reference is required.")
                .WithErrorCode("Reference.Required");

            RuleFor(x => x.UserReference)
                .NotEmpty()
                .WithMessage("UserReference is required.")
                .WithErrorCode("UserReference.Required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .WithErrorCode("Name.Required");

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 500)
                .WithMessage("Name must be between 3 and 500 characters.")
                .WithErrorCode("Name.Length.3.500");
        }
    }
}
