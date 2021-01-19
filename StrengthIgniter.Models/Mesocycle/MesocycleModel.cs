using FluentValidation;
using StrengthIgniter.Models.Common;
using System;

namespace StrengthIgniter.Models.Mesocycle
{
    public class MesocycleModel : ModelBase
    {
        public int MesocycleId { get; set; }
        public Guid Reference { get; set; }
        public Guid UserReference { get; set; }
        //
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        #region Methods

        public override ModelValidationResult Validate()
        {
            return new ModelValidationResult(new MesocycleModelValidator().Validate(this));
        }

        #endregion

        
    }
}
