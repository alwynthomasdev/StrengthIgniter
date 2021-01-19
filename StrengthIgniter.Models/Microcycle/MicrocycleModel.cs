using FluentValidation;
using StrengthIgniter.Models.Common;
using System;

namespace StrengthIgniter.Models.Microcycle
{
    public class MicrocycleModel : ModelBase
    {
        public int MicrocycleId { get; set; }
        public Guid Reference { get; set; }
        public int? MesocycleId { get; set; }
        public Guid UserReference { get; set; }
        //
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Notes { get; set; }
        public string Description { get; set; }
        //
        //public IEnumerable<RecordModel> Records { get; set }

        #region Methods

        public override ModelValidationResult Validate()
        {
            return new ModelValidationResult(new MicrocycleModelValidator().Validate(this));
        }

        #endregion

        
    }
}
