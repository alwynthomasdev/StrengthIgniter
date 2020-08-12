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
    public interface IRecordImportDataAccess
    {
        RecordImportModel GetByReference(Guid reference, Guid userReference);
        IEnumerable<RecordImportModel> GetByUserReference(Guid userReference);

        void Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportModel import);

        void DeleteByReference(Guid reference);
    }

    public class RecordImportDataAccess : DataAccessBase, IRecordImportDataAccess
    {
        #region CTOR
        public RecordImportDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion


        public RecordImportModel GetByReference(Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 [RecordImportId]
      ,[ri].[Reference]
      ,[ri].[UserId]
      ,[ri].[RecordImportSchemaId]
      ,[ri].[Name]
      ,[ri].[ImportDateTimeUtc]
FROM [RecordImport] [ri]
    INNER JOIN [User] u
        ON [ri].[UserId] = [u].[UserId]
WHERE
    [ri].[IsDeleted] = 0 AND
    [ri].[Reference] = @Reference AND
    [u]. [Reference] = @UserReference
".Trim();
            #endregion

            object parameters = new { Reference = reference, UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    RecordImportModel import = dbConnection.QueryFirstOrDefault<RecordImportModel>(sql, parameters);
                    if(import != null)
                    {
                        import.Rows = GetImportRows(dbConnection, import.RecordImportId);
                    }
                    return import;
                }
            }
            catch(DataAccessException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }

        }

        public IEnumerable<RecordImportModel> GetByUserReference(Guid userReference)
        {
            throw new NotImplementedException();
        }

        public void Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportModel import)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 @UserId = [UserId] FROM [User] WHERE [Reference] = @UserReference

INSERT INTO [RecordImport]
    ([Reference]
    ,[UserId]
    ,[RecordImportSchemaId]
    ,[Name]
    ,[ImportDateTimeUtc])
VALUES
    (@Reference
    ,@UserId
    ,@RecordImportSchemaId
    ,@Name
    ,@ImportDateTimeUtc);

SELECT TOP 1 [RecordImportId] FROM [RecordImport] WHERE [Reference] = @Reference
".Trim();
            if(import.RecordImportSchemaReference.HasValue)
            {
                sql = @"
SELECT TOP 1 @RecordImportSchemaId = [RecordImportSchemaId] FROM [RecordImportSchema] WHERE [Reference] = @RecordImportSchemaReference

"+sql;
            }
            #endregion

            try
            {
                int? importId = dbConnection.QueryFirstOrDefault<int?>(sql, import, dbTransaction);
                if (importId.HasValue)
                {
                    InsertImportRows(dbConnection, dbTransaction, importId.Value, import.Rows);
                }
            }
            catch (DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, import);
            }
        }

        public void DeleteByReference(Guid reference)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private void InsertImportRows(IDbConnection dbConnection, IDbTransaction dbTransaction, int importId, IEnumerable<RecordImportRowModel> rows)
        {
            #region SQL
            string sql = @"
INSERT INTO [RecordImportRow]
    ([RecordImportId]
    ,[StatusCode]
    ,[ExerciseText]
    ,[DateText]
    ,[WeightKgText]
    ,[WeightLbText]
    ,[BodyweightKgText]
    ,[BodyweightLbText]
    ,[SetText]
    ,[RepText]
    ,[RpeText]
    ,[Notes])
VALUES
    (@RecordImportId
    ,@StatusCode
    ,@ExerciseText
    ,@DateText
    ,@WeightKgText
    ,@WeightLbText
    ,@BodyweightKgText
    ,@BodyweightLbText
    ,@SetText
    ,@RepText
    ,@RpeText
    ,@Notes);

SELECT SCOPE_IDENTITY();
".Trim();
            #endregion

            RecordImportRowModel[] aryRows = rows.TryToArray();
            if (aryRows.HasItems())
            {
                for (int i = 0; i < aryRows.Length; i++)
                {
                    RecordImportRowModel row = aryRows[i];
                    row.RecordImportId = importId;

                    try
                    {
                        int? rowId = dbConnection.QueryFirst<int?>(sql, row, dbTransaction);
                        if (rowId.HasValue)
                        {
                            InsertImportRowErrors(dbConnection, dbTransaction, rowId.Value, row.Errors);
                        }
                    }
                    catch (DataAccessException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        throw new DataAccessException(ex, sql, row);
                    }
                }
            }
        }

        private void InsertImportRowErrors(IDbConnection dbConnection, IDbTransaction dbTransaction, int rowId, IEnumerable<RecordImportRowErrorModel> errors)
        {
            #region SQL
            string sql = @"
INSERT INTO [RecordImportRowError]
    ([RecordImportRowId]
    ,[ErrorCode])
VALUES
    (@RecordImportRowId
    ,@ErrorCode)
".Trim();
            #endregion

            if (errors.HasItems())
            {
                foreach (RecordImportRowErrorModel error in errors)
                {
                    try
                    {
                        dbConnection.Execute(sql, error, dbTransaction);
                    }
                    catch (Exception ex)
                    {
                        throw new DataAccessException(ex, sql, error);
                    }
                }
            }
        }

        private IEnumerable<RecordImportRowModel> GetImportRows(IDbConnection dbConnection, int importId)
        {
            #region SQL
            string sql = @"
SELECT [RecordImportRowId]
      ,[RecordImportId]
      ,[StatusCode]
      ,[ExerciseText]
      ,[DateText]
      ,[WeightKgText]
      ,[WeightLbText]
      ,[BodyweightKgText]
      ,[BodyweightLbText]
      ,[SetText]
      ,[RepText]
      ,[RpeText]
      ,[Notes]
      ,[ExerciseId]
FROM [RecordImportRow]
WHERE 
    [RecordImportId] = @RecordImportId
".Trim();
            #endregion

            object parameters = new { RecordImportId = importId };

            try
            {
                RecordImportRowModel[] rows = dbConnection.Query<RecordImportRowModel>(sql, parameters).TryToArray(); 
                if(rows.HasItems())
                {
                    for(int i = 0; i < rows.Length; i++)
                    {
                        rows[i].Errors = GetImportRowErrors(dbConnection, rows[i].RecordImportRowId);
                    }
                }
                return rows;
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

        private IEnumerable<RecordImportRowErrorModel> GetImportRowErrors(IDbConnection dbConnection, int rowId)
        {
            #region SQL
            string sql = @"
SELECT [RecordImportRowErrorId]
      ,[RecordImportRowId]
      ,[ErrorCode]
FROM [RecordImportRowError]
WHERE
    [RecordImportRowId] = @RecordImportRowId
".Trim();
            #endregion

            object parameters = new { RecordImportRowId = rowId };

            try
            {
                return dbConnection.Query<RecordImportRowErrorModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        #endregion

    }
}
