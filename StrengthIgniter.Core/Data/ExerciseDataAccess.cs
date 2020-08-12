using Dapper;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Core.Data
{

    public interface IExerciseDataAccess
    {
        ExerciseModel GetById(int id);
        ExerciseModel GetByReference(Guid reference);
    }

    public class ExerciseDataAccess : DataAccessBase, IExerciseDataAccess
    {
        #region CTOR
        public ExerciseDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public ExerciseModel GetById(int id)
        {
            #region SQL
            string sql = @"
SELECT TOP 1
    [ExerciseId],
    [Reference],
    [Name]
FROM
    [Exercise]
WHERE
    [ExerciseId] = @ExerciseId AND
    [IsDeleted] = 0
".Trim();
            #endregion

            object parameters = new { ExerciseId = id };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<ExerciseModel>(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        public ExerciseModel GetByReference(Guid reference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1
    [ExerciseId],
    [Reference],
    [Name]
FROM
    [Exercise]
WHERE
    [Reference] = @Reference AND
    [IsDeleted] = 0
".Trim();
            #endregion

            object parameters = new { Reference = reference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.QueryFirstOrDefault<ExerciseModel>(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }
    }
}
