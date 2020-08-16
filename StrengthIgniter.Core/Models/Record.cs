using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class RecordModel
    {
        public int RecordId { get; set; }
        public int UserId { get; internal set; }
        public int ExerciseId { get; set; }
        public DateTime Date { get; set; }
        public int? Sets { get; set; }
        public int Reps { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? BodyweightKg { get; set; }
        public decimal? RPE { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDateTimeUtc { get; internal set; }

        //
        public Guid? UserReference { get; set; }
        public string ExerciseName { get; set; }
        public Guid? ExerciseReference { get; set; }

        #region Calculated Properties

        public decimal? e1RM
        {
            get
            {
                if (this.WeightKg.HasValue)
                {
                    decimal x = EOneRepMaxHelper.CalculateOneRepMax(this.Reps, this.WeightKg.Value);
                    return WeightConversionHelper.RoundToZeroPointFive(x);
                }
                else return null;
            }
        }

        public decimal? RPEMax
        {
            get
            {
                if (this.WeightKg.HasValue && this.RPE.HasValue)
                {
                    decimal x = RpeHelper.CalculateRpeMax(this.Reps, this.WeightKg.Value, this.RPE.Value);
                    return WeightConversionHelper.RoundToZeroPointFive(x);
                }
                else return null;
            }
        }

        public int? VolumeKg
        {
            get
            {
                if (this.WeightKg.HasValue)
                {
                    int sets = 1;
                    if (this.Sets.HasValue)
                        sets = this.Sets.Value;

                    return (int)(sets * this.Reps * this.WeightKg.Value);
                }
                else return null;
            }
        }

        #endregion

    }
}
