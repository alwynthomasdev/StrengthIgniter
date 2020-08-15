﻿using CodeFluff.Extensions.IEnumerable;
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
        RecordModel GetByIdAndUser(int id, Guid userReference);
        IEnumerable<RecordModel> GetByExerciseAndUser(Guid exerciseReference, Guid userReference);
        Tuple<IEnumerable<RecordModel>, int> GetPagedByExerciseAndUser(Guid exerciseReference, Guid userReference, int? offset, int? fetch);
        int Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordModel record);
        void Update(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordModel record);
        void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, int recordId, Guid userReference);
    }

    public class RecordDataAccess : DataAccessBase, IRecordDataAccess
    {
        #region CTOR
        public RecordDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public IEnumerable<RecordModel> GetByExerciseAndUser(Guid exerciseReference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT [r].[RecordId]
      ,[r].[UserId]
      ,[r].[ExerciseId]
	  ,[e].[Name] AS [ExerciseName]
      ,[r].[Date]
      ,[r].[Sets]
      ,[r].[Reps]
      ,[r].[WeightKg]
      ,[r].[BodyweightKg]
      ,[r].[RPE]
      ,[r].[Notes]
      ,[r].[CreatedDateTimeUtc]
FROM [Record] [r]
	INNER JOIN [User] [u]
		ON [r].[UserId] = [u].[UserId]
	INNER JOIN [Exercise] [e]
		ON [r].[ExerciseId] = [e].[ExerciseId]
WHERE
	[e].[Reference] = @ExerciseReference AND
	[u].[Reference] = @UserReference
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

        public Tuple<IEnumerable<RecordModel>, int> GetPagedByExerciseAndUser(Guid exerciseReference, Guid userReference, int? offset, int? fetch)
        {
            #region SQL
            string sql = @"
SELECT {0}
       [r].[RecordId]
      ,[r].[UserId]
      ,[r].[ExerciseId]
	  ,[e].[Name] AS [ExerciseName]
      ,[r].[Date]
      ,[r].[Sets]
      ,[r].[Reps]
      ,[r].[WeightKg]
      ,[r].[BodyweightKg]
      ,[r].[RPE]
      ,[r].[Notes]
      ,[r].[CreatedDateTimeUtc]
FROM [Record] [r]
	INNER JOIN [User] [u]
		ON [r].[UserId] = [u].[UserId]
	INNER JOIN [Exercise] [e]
		ON [r].[ExerciseId] = [e].[ExerciseId]
WHERE
	[e].[Reference] = @ExerciseReference AND
	[u].[Reference] = @UserReference
ORDER BY  [r].[Date] DESC
{1}

SELECT Count(*) [Count]
FROM [Record] [r]
	INNER JOIN [User] [u]
		ON [r].[UserId] = [u].[UserId]
	INNER JOIN [Exercise] [e]
		ON [r].[ExerciseId] = [e].[ExerciseId]
WHERE
	[e].[Reference] = @ExerciseReference AND
	[u].[Reference] = @UserReference
".Trim();

            if (offset.HasValue && fetch.HasValue)
            {
                sql = string.Format(sql, "", "OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY");
            }
            else if (fetch.HasValue)
            {
                sql = string.Format(sql, "TOP @Fetch", "");
            }
            else
            {
                sql = string.Format(sql, "", "");
            }

            #endregion

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
INSERT INTO [Record]
    ([UserId]
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
            if(record.UserReference.HasValue)
            {
                sql = @"SELECT TOP 1 @UserId = [UserId] FROM [User] WHERE [Reference] = @UserReference
" + sql;
            }
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
		[RecordId] = @RecordId
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

        public void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, int recordId, Guid userReference)
        {
            #region SQL
            string sql = @"
DELETE 
FROM [Record] 
WHERE 
	[RecordId] = @RecordId AND 
	[UserId] IN (
		SELECT TOP 1 [u].[UserId]
		FROM [Record] [r]
			INNER JOIN [User] [u]
				ON [r].[UserId] = [u].[UserId]
		WHERE [u].[Reference] = @UserReference
	)
".Trim();
            #endregion

            object parameters = new { RecordId = recordId, UserReference = userReference };

            try
            {
                dbConnection.Execute(sql, parameters, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }

        }

        public RecordModel GetByIdAndUser(int id, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 [r].[RecordId]
      ,[r].[UserId]
      ,[r].[ExerciseId]
      ,[r].[Date]
      ,[r].[Sets]
      ,[r].[Reps]
      ,[r].[WeightKg]
      ,[r].[BodyweightKg]
      ,[r].[RPE]
      ,[r].[Notes]
      ,[r].[CreatedDateTimeUtc]
      ,[e].[Name] AS [ExerciseName]
      ,[e].[Reference] AS [ExerciseReference]
FROM [Record] [r]
	INNER JOIN [User] [u]
		ON [r].[UserId] = [u].[UserId]
    INNER JOIN [Exercise] [e]
        ON [r].[ExerciseId] = [e].[ExerciseId]
WHERE 
	[r].[RecordId] = @RecordId AND
	[u].[Reference] = @UserReference
".Trim();

            #endregion

            object parameters = new { RecordId = id, UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    return dbConnection.QueryFirstOrDefault<RecordModel>(sql, parameters);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }

        }

    }
}