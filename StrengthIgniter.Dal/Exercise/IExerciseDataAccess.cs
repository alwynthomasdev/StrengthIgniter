using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.Exercise;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.Exercise
{
    public interface IExerciseDataAccess
    {
        ExerciseModel Select(int id, Guid userReference);

        Tuple<IEnumerable<ExerciseModel>, int> Filter(string searchString, Guid userReference, int? offset, int? fetch);

        int Insert(ExerciseModel exercise, IDbTransaction dbTransaction = null);

        void Update(ExerciseModel exercise, IDbTransaction dbTransaction = null);

        void Delete(int id, Guid userReference, IDbTransaction dbTransaction = null);
    }
}
