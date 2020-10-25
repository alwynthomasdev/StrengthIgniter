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
        ExerciseModel Select(int id);

        ExerciseModel Select(Guid reference, Guid userReference);

        Tuple<IEnumerable<ExerciseModel>, int> Filter(string exerciseName, Guid userReference, int? offset, int? fetch);

        ExerciseModel Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, ExerciseModel exercise);

        void Update(IDbConnection dbConnection, IDbTransaction dbTransaction, ExerciseModel exercise);

        void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference);
    }

    public class ExerciseDataAccess : DataAccessBase, IExerciseDataAccess
    {
        #region CTOR
        public ExerciseDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public ExerciseModel Select(int id)
        {
            #region SQL
            string sql = @"
SELECT TOP 1
    e.ExerciseId,
    e.Reference,
    u.Reference AS UserReference,
    e.[Name]
FROM
    Exercise e
    INNER JOIN [User] u
        ON e.UserId = u.UserId
WHERE
    e.ExerciseId = @ExerciseId AND
    e.IsDeleted = 0
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

        public ExerciseModel Select(Guid reference, Guid userReference)
        {
            #region SQL
            string sql = $@"
SELECT TOP 1
    e.ExerciseId,
    e.Reference,
    u.Reference AS UserReference,
    e.[Name]
FROM
    Exercise e
    INNER JOIN [User] u 
        ON e.UserId = u.UserId
WHERE
    e.Reference = @Reference AND
    (u.Reference = @UserReference OR u.Reference = '{SYSTEM_USER_REFERENCE}') AND
    e.IsDeleted = 0
".Trim();
            #endregion

            object parameters = new { Reference = reference, UserReference = userReference };

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

        public Tuple<IEnumerable<ExerciseModel>, int> Filter(string exerciseName, Guid userReference, int? offset, int? fetch)
        {
            string sql = GenerateFilterSql(exerciseName, offset, fetch);
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

        public ExerciseModel Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, ExerciseModel exercise)
        {
            #region SQL
            string sql = @"
DECLARE @UserId INTEGER
SELECT @UserId = UserId FROM [User] WHERE Reference = @UserReference

INSERT INTO [dbo].[Exercise]
	(Reference
	,[Name]
	,UserId
	,CreatedDateTimeUtc)
VALUES
	(@Reference
	,@Name
	,@UserId
	,GETUTCDATE())

SELECT SCOPE_IDENTITY()
".Trim();
            #endregion

            try
            {
                int? id = dbConnection.QuerySingleOrDefault<int?>(sql, dbTransaction);
                if (id.HasValue)
                {
                    exercise.ExerciseId = id.Value;
                    return exercise;
                }
                else throw new Exception("failed to insert exercise.");
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, exercise);
            }
        }

        public void Update(IDbConnection dbConnection, IDbTransaction dbTransaction, ExerciseModel exercise)
        {
            #region SQL
            string sql = @"
UPDATE 
	Exercise
SET 
	Exercise.[Name] = @Name
FROM 
	Exercise e
	INNER JOIN [User] u
		ON e.UserId = u.UserId
WHERE
	e.Reference = @Reference
	AND u.Reference = @UserReference
".Trim();
            #endregion

            try
            {
                dbConnection.Execute(sql, exercise, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, exercise);
            }

        }

        public void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
UPDATE 
	Exercise
SET 
	Exercise.IsDeleted = 1
    Exercise.DeletedDateTimeUtc = GETUTCDATE()
FROM 
	Exercise e
	INNER JOIN [User] u
		ON e.UserId = u.UserId
WHERE
	e.Reference = @Reference
	AND u.Reference = @UserReference
".Trim();
            #endregion

            object parameters = new { Reference = reference, UserReference = userReference };

            try
            {
                dbConnection.Execute(sql, parameters, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }

        }

        #region Private Helpers

        // Generates sql based on filter parameters
        private string GenerateFilterSql(string exerciseName, int? offset, int? fetch)
        {
            // define variables
            string top = "";
            string offsetFetchRows = "";

            // set the where clause if there is an exercise search string
            string where = !string.IsNullOrWhiteSpace(exerciseName) ? "AND ex.[Name] LIKE '%'+@SearchString+'%'" : "";

            // set the top or offset depending on configuration of offset and fetch parameters
            if (offset.HasValue && fetch.HasValue)
            {
                offsetFetchRows = "OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY";
            }
            else if (fetch.HasValue)
            {
                top = "TOP @Fetch";
            }
            else
            {
                // do nothing
            }

            // generate and return sql string
            return $@"
SELECT {top}
    ex.ExerciseId,
    ex.Reference,
    u.Reference AS UserReference,
    ex.[Name]
FROM
    Exercise ex
    INNER JOIN [User] u
        ON ex.UserId = u.UserId
WHERE
    (u.Reference = @UserReference OR u.Reference = '{SYSTEM_USER_REFERENCE}')
    AND ex.IsDeleted = 0
    {where}
ORDER BY ex.[Name]
{offsetFetchRows};

SELECT 
	COUNT(*) [Count]
FROM
	Exercise ex
    INNER JOIN [User] u
        ON ex.UserId = u.UserId
WHERE
    ex.IsDeleted = 0
    {where}
".Trim();
        }

        #endregion
    }

}
