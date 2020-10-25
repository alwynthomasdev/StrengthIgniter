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
    public interface IRecordDataAccess
    {
        RecordModel Select(Guid reference, Guid userReference);
        IEnumerable<RecordModel> SelectByExercise(Guid exerciseReference, Guid userReference);

        Tuple<IEnumerable<RecordModel>, int> FilterByExercise(Guid exerciseReference, Guid userReference, int? offset, int? fetch);

        int Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordModel record);
        void Update(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordModel record);
        void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference);
    }

    public class RecordDataAccess : DataAccessBase, IRecordDataAccess
    {
        #region CTOR
        public RecordDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public RecordModel Select(Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 
	r.RecordId,
    r.Reference,
	r.UserId,
	r.ExerciseId,
	r.[Date],
    r.[Sets],
    r.Reps,
    r.WeightKg,
    r.BodyweightKg,
    r.RPE,
    r.Notes,
    r.CreatedDateTimeUtc,
    e.[Name] AS ExerciseName,
    u.Reference AS UserReference,
    e.Reference AS ExerciseReference
FROM Record r
INNER JOIN [User] u 
    ON r.UserId = u.UserId
INNER JOIN Exercise e 
    ON r.ExerciseId = e.ExerciseId
WHERE 
	r.Reference = @Reference AND
	u.Reference = @UserReference
".Trim();

            #endregion

            object parameters = new { Reference = reference, UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    return dbConnection.QueryFirstOrDefault<RecordModel>(sql, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }

        }

        public IEnumerable<RecordModel> SelectByExercise(Guid exerciseReference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT 
	r.RecordId,
	r.Reference,
    r.UserId,
    r.ExerciseId,
	e.[Name] AS ExerciseName,
    r.[Date],
    r.[Sets],
    r.Reps,
    r.WeightKg,
    r.BodyweightKg,
    r.RPE,
    r.Notes,
    r.CreatedDateTimeUtc,
    u.Reference AS UserReference,
    e.Reference AS ExerciseReference
FROM Record r
INNER JOIN [User] u
	ON r.UserId = u.UserId
INNER JOIN Exercise e
	ON r.ExerciseId = e.ExerciseId
WHERE
	e.Reference = @ExerciseReference AND
	u.Reference = @UserReference
".Trim();
            #endregion

            object parameters = new { ExerciseReference = exerciseReference, UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    return dbConnection.Query<RecordModel>(sql, parameters);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        public Tuple<IEnumerable<RecordModel>, int> FilterByExercise(Guid exerciseReference, Guid userReference, int? offset, int? fetch)
        {
            string sql = GenerateFilterByExerciseSql(offset, fetch);
            object parameters = new { ExerciseReference = exerciseReference, UserReference = userReference, Offset = offset, Fetch = fetch };
            RecordModel[] records = null;
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
                            records = reader.Read<RecordModel>().TryToArray();
                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<RecordModel>, int>(records, total);
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

        public int Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordModel record)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 @UserId = [UserId] 
FROM [User] 
WHERE [Reference] = @UserReference

INSERT INTO [Record]
    ([UserId]
    ,[Reference]
    ,[ExerciseId]
    ,[Date]
    ,[Sets]
    ,[Reps]
    ,[WeightKg]
    ,[BodyweightKg]
    ,[RPE]
    ,[Notes]
    ,[CreatedDateTimeUtc])
VALUES
    (@UserId
    ,@Reference
    ,@ExerciseId
    ,@Date
    ,@Sets
    ,@Reps
    ,@WeightKg
    ,@BodyweightKg
    ,@RPE
    ,@Notes
    ,@CreatedDateTimeUtc)

SELECT SCOPE_IDENTITY()
".Trim();
            #endregion

            try
            {
                return dbConnection.QueryFirstOrDefault<int>(sql, record, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, record);
            }
        }

        public void Update(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordModel record)
        {
            #region SQL
            string sql = @"
IF EXISTS(
	SELECT TOP 1 [u].[UserId] 
	FROM [Record] [r]
		INNER JOIN [User] [u]
			ON [r].[UserId] = [u].[UserId]
	WHERE [u].[Reference] = @UserReference AND [r].[RecordId] = @RecordId
)
 BEGIN
	UPDATE [dbo].[Record]
	   SET [Date] = @Date
		  ,[Sets] = @Sets
		  ,[Reps] = @Reps
		  ,[WeightKg] = @WeightKg
		  ,[BodyweightKg] = @BodyweightKg
		  ,[RPE] = @RPE
		  ,[Notes] = @Notes
	WHERE 
		[Reference] = @Reference
 END
".Trim();

            try
            {
                dbConnection.Execute(sql, record, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, record);
            }

            #endregion
        }

        public void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
DELETE 
FROM [Record] 
WHERE 
	[Reference] = @Reference AND 
	[UserId] IN (
		SELECT TOP 1 [u].[UserId]
		FROM [Record] [r]
			INNER JOIN [User] [u]
				ON [r].[UserId] = [u].[UserId]
		WHERE [u].[Reference] = @UserReference
	)
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

        private string GenerateFilterByExerciseSql(int? offset, int? fetch)
        {
            // define variables
            string top = "";
            string offsetFetchRows = "";

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

            return $@"
SELECT {top}
	r.RecordId,
    r.Reference,
    r.UserId,
    r.ExerciseId,
	e.[Name] AS ExerciseName,
    r.[Date],
    r.[Sets],
    r.Reps,
    r.WeightKg,
    r.BodyweightKg,
    r.RPE,
    r.Notes,
    r.CreatedDateTimeUtc,
    u.Reference AS UserReference,
    e.Reference AS ExerciseReference
FROM Record r
INNER JOIN [User] u
	ON r.UserId = u.UserId
INNER JOIN Exercise e
	ON r.ExerciseId = e.ExerciseId
WHERE
	e.Reference = @ExerciseReference AND
	u.Reference = @UserReference
ORDER BY  r.[Date] DESC
{offsetFetchRows}

SELECT Count(*) Count
FROM Record r
INNER JOIN [User] u
	ON r.UserId = u.UserId
INNER JOIN Exercise e
	ON r.ExerciseId = e.ExerciseId
WHERE
	e.Reference = @ExerciseReference AND
	u.Reference = @UserReference
".Trim();

        }

        #endregion
    }
}
