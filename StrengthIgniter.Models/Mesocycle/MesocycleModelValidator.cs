using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.Mesocycle
{
    internal class MesocycleModelValidator : AbstractValidator<MesocycleModel>
    {
        public MesocycleModelValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty()
                .WithMessage("Reference is required.")
                .WithErrorCode("Reference.Required");

            RuleFor(x => x.UserReference)
                .NotEmpty()
                .WithMessage("UserReference is required.")
                .WithErrorCode("UserReference.Required");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("StartDate is required.")
                .WithErrorCode("StartDate.Required");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes can be no more than 1000 characters long.")
                .WithErrorCode("Notes.MaximumLength.1000");
        }
    }
}
