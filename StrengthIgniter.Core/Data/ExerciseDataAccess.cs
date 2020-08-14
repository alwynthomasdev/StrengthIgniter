using CodeFluff.Extensions.IEnumerable;
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

        /// <summary>
        /// Pageable search for exercises
        /// </summary>
        /// <param name="exerciseName">Search string</param>
        /// <param name="offset">offset the search (based on page)</param>
        /// <param name="fetch">number or exercises to return (how many per page)</param>
        /// <returns>A tuple containing the exercies and total number of results found (used to determine number of pages)</returns>
        Tuple<IEnumerable<ExerciseModel>, int> Search(string exerciseName, int? offset, int? fetch);
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

        public Tuple<IEnumerable<ExerciseModel>, int> Search(string exerciseName, int? offset, int? fetch)
        {
            string sql = GenerateSearchSql(exerciseName, offset, fetch);
            object parameters = new { SearchString = exerciseName, Offset = offset, Fetch = fetch };
            ExerciseModel[] exercises = null;
            int total = 0;

            try
            { 
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (SqlMapper.GridReader reader = dbConnection.QueryMultiple(sql, parameters))
                    {
                        if (reader != null)
                        {
                            exercises = reader.Read<ExerciseModel>().TryToArray();
                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<ExerciseModel>, int>(exercises, total);
                }
            }
            catch (DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        #region Private Helpers

        private string GenerateSearchSql(string exerciseName, int? offset, int? fetch)
        {
            string sql = @"
SELECT 
    [ex].[ExerciseId],
    [ex].[Reference],
    [ex].[Name]
FROM
    [Exercise] [ex]
WHERE
    [ex].[IsDeleted] = 0
    {1}
ORDER BY [ex].[Name]
{2};

SELECT 
	Count(*) [Count]
FROM
	[Exercise] [ex]
WHERE
    [ex].[IsDeleted] = 0
    {1}";

            string whereClause = "";
            if (!string.IsNullOrWhiteSpace(exerciseName))
            {
                whereClause = "AND [ex].[Name] LIKE '%'+@SearchString+'%'";
            }

            //add offset/fetch if parameters set...
            if (offset.HasValue && fetch.HasValue)
            {
                sql = string.Format(sql, "", whereClause, "OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY");
            }
            else if (fetch.HasValue)
            {
                sql = string.Format(sql, "TOP @Fetch", whereClause, "");
            }
            else
            {
                sql = string.Format(sql, "", whereClause, "");
            }

            return sql.Trim();
        }

        #endregion




    }

}
