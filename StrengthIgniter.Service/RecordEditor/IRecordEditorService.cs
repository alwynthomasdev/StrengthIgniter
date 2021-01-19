using StrengthIgniter.Models.Record;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.RecordEditor
{
    public interface IRecordEditorService
    {
        RecordModel Save(RecordModel record);
        void Delete(int recordId, Guid userReference);
    }
}
