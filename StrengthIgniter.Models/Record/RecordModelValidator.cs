using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.Record
{
    internal class RecordModelValidator : AbstractValidator<RecordModel>
    {
        public RecordModelValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty()
                .WithMessage("Reference is required.")
                .WithErrorCode("Reference.Required");

            RuleFor(x => x.UserReference)
                .NotEmpty()
                .WithMessage("UserReference is required.")
                .WithErrorCode("UserReference.Required");

            RuleFor(x => x.ExerciseId)
                .NotEmpty()
                .WithMessage("ExerciseId is required.")
                .WithErrorCode("ExerciseId.Required");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Date is required.")
                .WithErrorCode("Date.Required");

            RuleFor(x => x.Reps)
                .NotEmpty()
                .WithMessage("Reps is required (more than 0).")
                .WithErrorCode("Reps.Required");

            RuleFor(x => x.RPE)
                .Must(isRpeRangeValid)
                .WithMessage("RPE must be between 6 and 10.")
                .WithErrorCode("RPE.Range.6.10");

            RuleFor(x => x.RPE)
                .Must(isRpeIntervalValid)
                .WithMessage("RPE must be an interval of 0.5.")
                .WithErrorCode("RPE.Interval");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes can be no more than 1000 characters long.")
                .WithErrorCode("Notes.MaximumLength.1000");
        }

        private bool isRpeRangeValid(decimal? rpe)
        {
            bool isValid = false;

            if (rpe.HasValue)
                isValid = (rpe > 6 && rpe < 10);// is between 6 and 10
            else isValid = true;                // an null rpe is valid

            return isValid;
        }

        private bool isRpeIntervalValid(decimal? rpe)
        {
            bool isValid = false;

            if (rpe.HasValue)
                isValid = (rpe % 0.5m == 0);    // is an interval of 0.5
            else isValid = true;                // an null rpe is valid

            return isValid;
        }

    }
}
