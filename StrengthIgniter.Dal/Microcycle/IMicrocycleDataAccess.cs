using StrengthIgniter.Models.Microcycle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.Microcycle
{
    public interface IMicrocycleDataAccess
    {
        MicrocycleModel Select(int microcycleId, Guid userReference);
        Tuple<IEnumerable<MicrocycleModel>, int> Filter(
            Guid userReference, string searchString,
            DateTime? startDate, DateTime endDate,
            int? offset, int? fetch);
        int Insert(MicrocycleModel microcycle, IDbTransaction dbTransaction = null);
        void Update(MicrocycleModel microcycle, IDbTransaction dbTransaction = null);
        void Delete(int microcycleId, Guid userReference, bool deleteRecords = false, IDbTransaction dbTransaction = null);

    }
}
