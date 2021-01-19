using StrengthIgniter.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.Record
{
    public class RecordModel : ModelBase
    {
        public int RecordId { get; set; }
        public Guid Reference { get; set; }
        public DateTime Date { get; set; }
        public int Reps { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? BodyweightKg { get; set; }
        public decimal? RPE { get; set; }
        public string Notes { get; set; }
        public Guid? SetReference { get; set; }
        public int? SetOrdinal { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        //
        public int ExerciseId { get; set; }
        public int? MicrocycleId { get; set; }
        public int? MesocycleId { get; set; }
        //
        public Guid UserReference { get; set; }
        public string ExerciseName { get; set; }

        #region Methods

        public override ModelValidationResult Validate()
        {
            return new ModelValidationResult(new RecordModelValidator().Validate(this));
        }

        #endregion

    }
}
