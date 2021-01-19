using StrengthIgniter.Models.Record;
using StrengthIgniter.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.RecordFilter
{
    public interface IRecordFilterService
    {
        FilterResponse<RecordModel> Filter(RecordFilterRequest request);
    }
}
