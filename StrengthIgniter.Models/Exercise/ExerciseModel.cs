using StrengthIgniter.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.Exercise
{
    public class ExerciseModel : ModelBase
    {
        public int ExerciseId { get; set; }
        public Guid Reference { get; set; }
        public Guid UserReference { get; set; }
        public string Name { get; set; }

        #region Methods

        public override ModelValidationResult Validate()
        {
            return new ModelValidationResult(new ExercieModelValidator().Validate(this));
        }

        #endregion
    }
}
