using FluentValidation;
using StrengthIgniter.Core.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class ExerciseModel : ModelBase
    {
        public int ExerciseId { get; internal set; }
        public Guid Reference { get; internal set; }
        public Guid UserReference { get; set; }
        public string Name { get; set; }
    }

    public class ExercieModelValidator : AbstractValidator<ExerciseModel>
    {
        public ExercieModelValidator()
        {
            RuleFor(x => x.UserReference)
                .NotEqual(Guid.Empty)
                .WithMessage("UserReference is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 500)
                .WithMessage("Name is required and must be between 3 and 500 characters.");
        }
    }

}
