using StrengthIgniter.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.RecordFilter
{
    public class RecordFilterRequest : FilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? ExerciseId { get; set; }
        public int? MesocycleId { get; set; }
        public int? MicrocycleId { get; set; }

        //TODO: pass order by value ???

        // prevent use of this property, not used for RecordFilterRequest
        new public string SearchString
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
