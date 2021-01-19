using StrengthIgniter.Models.Record;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.Record
{
    public interface IRecordDataAccess
    {
        IEnumerable<RecordModel> Select(int recordId, Guid userReference);
        Tuple<IEnumerable<RecordModel>, int> Filter(
            Guid userReference, int? exerciseId,
            DateTime? startDate, DateTime endDate,
            int? mesocycleId, int? microcycleId,
            int? offset, int? fetch);
        int Insert(RecordModel record, IDbTransaction dbTransaction = null);
        void Update(RecordModel record, IDbTransaction dbTransaction = null);
        void Delete(int recordId, Guid userReference, IDbTransaction dbTransaction = null);
    }
}
