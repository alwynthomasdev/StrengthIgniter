using StrengthIgniter.Models.Mesocycle;
using System;
using System.Collections.Generic;
using System.Data;

namespace StrengthIgniter.Dal.Mesocycle
{
    public interface IMesocycleDataAccess
    {
        MesocycleModel Select(int mesocycleId, Guid userReference);
        Tuple<IEnumerable<MesocycleModel>, int> Filter(
            Guid userReference, string searchString,
            DateTime? startDate, DateTime endDate,
            int? offset, int? fetch);
        int Insert(MesocycleModel mesocycle, IDbTransaction dbTransaction = null);
        void Update(MesocycleModel mesocycle, IDbTransaction dbTransaction = null);
        void Delete(int mesocycleId, Guid userReference, bool deleteMicrocycles = false, bool deleteRecords = false, IDbTransaction dbTransaction = null);
    }
}
