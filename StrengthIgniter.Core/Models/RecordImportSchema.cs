using FluentValidation;
using StrengthIgniter.Core.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class RecordImportSchemaModel : ModelBase
    {
        public int RecordImportSchemaId { get; internal set; }
        public Guid Reference { get; internal set; }
        public Guid UserReference { get; set; }
        public string Name { get; set; }
        public string Delimiter { get; set; }

        public IEnumerable<RecordImportSchemaColumnMapModel> ColumnMap { get; set; }
        public IEnumerable<RecordImportSchemaExerciseMapModel> ExerciseMap { get; set; }
    }

    public class RecordImportSchemaColumnMapModel : ModelBase
    {
        public int RecordImportSchemaColumnMapId { get; internal set; }
        public int RecordImportSchemaId { get; set; }
        public string HeaderName { get; set; }
        public ColumnTypeCode? ColumnTypeCode { get; set; }
    }

    public class RecordImportSchemaExerciseMapModel : ModelBase
    {
        public int RecordImportSchemaExerciseMapId { get; internal set; }
        public int RecordImportSchemaId { get; set; }
        internal int ExerciseId { get; set; }
        public Guid ExerciseReference { get; set; }
        public string Text { get; set; }
    }

    public enum ColumnTypeCode
    {
        Exercise = 1,
        Date = 2,
        WeightKg = 3,
        WeightLb = 4,
        BodyweightKg = 5,
        BodyweightLb = 6,
        Sets = 7,
        Reps = 8,
        Rpe = 9,
        Notes = 10
    }

    public class RecordImportSchemaModelValidator : AbstractValidator<RecordImportSchemaModel>
    {
        public RecordImportSchemaModelValidator()
        {
            RuleFor(x => x.UserReference)
                .NotEqual(Guid.Empty)
                .WithMessage("UserReference is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 500)
                .WithMessage("Name is required and must be between 3 and 500 characters.");

            RuleFor(x => x.Delimiter)
                .NotEmpty()
                .Must(x=>new List<string> { ",", "|", "\t"}.Contains(x))
                .Length(1)
                .WithMessage("Delimiter must be either a comma (,) pipe (|) or tab (\t) character.");

            RuleForEach(x => x.ColumnMap)
                .SetValidator(new RecordImportSchemaColumnMapModelValidator());

            RuleForEach(x => x.ExerciseMap)
                .SetValidator(new RecordImportSchemaExerciseMapModelValidator());
        }
    }

    public class RecordImportSchemaColumnMapModelValidator : AbstractValidator<RecordImportSchemaColumnMapModel>
    {
        public RecordImportSchemaColumnMapModelValidator()
        {
            RuleFor(x => x.HeaderName)
               .NotEmpty() 
               .Length(2, 500)
               .WithMessage("HeaderName is required and must be between 2 and 500 characters.");

            RuleFor(x => x.ColumnTypeCode)
               .NotEmpty()
               .WithMessage("ColumnTypeCode is required.");
        }
    }

    public class RecordImportSchemaExerciseMapModelValidator : AbstractValidator<RecordImportSchemaExerciseMapModel>
    {
        public RecordImportSchemaExerciseMapModelValidator()
        {
            RuleFor(x => x.ExerciseReference)
               .NotEqual(Guid.Empty)
               .WithMessage("ExercieReference is required.");

            RuleFor(x => x.Text)
               .NotEmpty()
               .Length(2, 500)
               .WithMessage("Text is required and must be between 2 and 500 characters.");
        }
    }

}
