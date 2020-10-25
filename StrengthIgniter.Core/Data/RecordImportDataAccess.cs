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
    public interface IRecordImportDataAccess
    {
        RecordImportModel Select(Guid reference, Guid userReference);
        IEnumerable<RecordImportModel> Select(Guid userReference);
        RecordImportRowModel SelectRow(Guid rowReference, Guid userReference);
        Tuple<IEnumerable<RecordImportRowModel>, int> Filter(Guid recordImportReference, Guid userReference, int? offset, int? fetch);

        void Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportModel import);

        void UpdateRowStatus(IDbConnection dbConnection, IDbTransaction dbTransaction, int rowId, Guid userReference, ImportRowStatusCode status);
        void UpdateRow(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportRowModel row, Guid userReference);

        void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference);
        void DeleteRow(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference);
    }

    public class RecordImportDataAccess : DataAccessBase, IRecordImportDataAccess
    {
        #region CTOR
        public RecordImportDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public RecordImportModel Select(Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 
    ri.RecordImportId,
    ri.Reference,
    ri.UserId,
    ri.RecordImportSchemaId,
    ri.[Name],
    ri.ImportDateTimeUtc,
    u.Reference AS UserReference,
    ris.Reference AS RecordImportSchemaReference
FROM RecordImport ri
INNER JOIN [User] u
    ON ri.UserId = u.UserId
INNER JOIN RecordImportSchema ris
    ON ri.RecordImportSchemaId = ris.RecordImportSchemaId
WHERE
    ri.IsDeleted = 0 AND
    ri.Reference = @Reference AND
    u. Reference = @UserReference
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
                        import.Rows = SelectImportRows(dbConnection, import.RecordImportId);
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

        public IEnumerable<RecordImportModel> Select(Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT 
    ri.RecordImportId,
    ri.Reference,
    ri.UserId,
    ri.RecordImportSchemaId,
    ri.[Name],
    ri.ImportDateTimeUtc,
    u.Reference AS UserReference,
    ris.Reference AS RecordImportSchemaReference
FROM RecordImport ri
INNER JOIN [User] u
    ON ri.UserId = u.UserId
INNER JOIN RecordImportSchema ris
    ON ri.RecordImportSchemaId = ris.RecordImportSchemaId
WHERE
    ri.IsDeleted = 0 AND
    u. Reference = @UserReference
";
            #endregion

            object parameters = new { UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    RecordImportModel[] imports = dbConnection.Query<RecordImportModel>(sql, parameters).TryToArray();
                    if(imports.HasItems())
                    {
                        for(int i = 0; i<imports.Length; i++)
                        {
                            imports[i].Rows = SelectImportRows(dbConnection, imports[i].RecordImportId);
                        }
                    }
                    return imports;
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

        public RecordImportRowModel SelectRow(Guid rowReference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 
    riw.RecordImportRowId,
    riw.Reference,
    riw.RecordImportId,
    riw.StatusCode,
    riw.ExerciseText,
    riw.DateText,
    riw.WeightKgText,
    riw.WeightLbText,
    riw.BodyweightKgText,
    riw.BodyweightLbText,
    riw.SetText,
    riw.RepText,
    riw.RpeText,
    riw.Notes,
    riw.ExerciseId
FROM RecordImportRow riw
INNER JOIN RecordImport ri
	ON riw.RecordImportId = ri.RecordImportId
INNER JOIN [User] u
	ON ri.UserId = u.UserId
WHERE
    riw.Reference = @rowReference AND
	u.Reference = @UserReference AND
	ri.IsDeleted = 0
".Trim();
            #endregion

            object parameters = new { RecordImportRowId = rowReference, UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    RecordImportRowModel row = dbConnection.QueryFirstOrDefault<RecordImportRowModel>(sql, parameters);
                    if (row != null)
                    {
                        row.Errors = SelectImportRowErrors(dbConnection, row.RecordImportRowId);
                    }
                    return row;
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

        public void Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportModel import)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 @UserId = UserId 
FROM [User] 
WHERE Reference = @UserReference

INSERT INTO RecordImport
    (Reference
    ,UserId
    ,RecordImportSchemaId
    ,[Name]
    ,ImportDateTimeUtc)
VALUES
    (@Reference
    ,@UserId
    ,@RecordImportSchemaId
    ,@Name
    ,@ImportDateTimeUtc);

SELECT TOP 1 
RecordImportId F
ROM RecordImport 
WHERE Reference = @Reference
".Trim();
            if(import.RecordImportSchemaReference.HasValue)
            {
                sql = @"
SELECT TOP 1 @RecordImportSchemaId = RecordImportSchemaId 
FROM RecordImportSchema 
WHERE Reference = @RecordImportSchemaReference

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

        public void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
UPDATE RecordImport
SET
	IsDeleted = 1,
	DeletedDateTimeUtc = GETUTCDATE()
WHERE
	Reference = @Reference AND
	UserId IN (
		SELECT TOP 1 UserId FROM [User] WHERE Reference = @UserReference
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

        public void UpdateRowStatus(IDbConnection dbConnection, IDbTransaction dbTransaction, int rowId, Guid userReference, ImportRowStatusCode status)
        {
            #region SQL
            string sql = @"
UPDATE
    RecordImportRow
SET
    RecordImportRow.StatusCode = @StatusCode
WHERE 
	RecordImportRowId IN (
		SELECT TOP 1 RecordImportRowId
		FROM RecordImportRow riw
			INNER JOIN RecordImport ri
				ON riw.RecordImportId = ri.RecordImportId
			INNER JOIN [User] u
				ON ri.UserId = u.UserId
		WHERE
			riw.RecordImportRowId = @RecordImportRowId AND
			u.Reference = @UserReference AND
			ri.IsDeleted = 0 AND
			u.IsDeleted = 0 
	)
".Trim();
            #endregion

            object parameters = new { RecordImportRowId = rowId, UserReference = userReference, StatusCode = (int)status };

            try
            {
                dbConnection.Execute(sql, parameters, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }

        }

        public Tuple<IEnumerable<RecordImportRowModel>, int> Filter(Guid recordImportReference, Guid userReference, int? offset, int? fetch)
        {
            string sql = GenerateRowFilterSql(offset, fetch);
            object parameters = new { ImportReference = recordImportReference, UserReference = userReference, Offset=offset, Fetch=fetch };
            RecordImportRowModel[] rows = null;
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
                            rows = reader.Read<RecordImportRowModel>().TryToArray();

                            for(int i = 0; i < rows.Length; i++)
                            {
                                rows[i].Errors = SelectImportRowErrors(dbConnection, rows[i].RecordImportRowId);
                            }

                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<RecordImportRowModel>, int>(rows, total);
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

        public void UpdateRow(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportRowModel row, Guid userReference)
        {
            #region SQL
            string sql = @"
DECLARE @RecordImportRowId INTEGER
SELECT @RecordImportRowId = RecordImportRowId FROM RecordImportRow WHERE Reference = @Reference

DELETE FROM RecordImportRowError
WHERE RecordImportRowId = @RecordImportRowId

UPDATE RecordImportRow
SET
	ExerciseText = @ExerciseText,
	DateText = @DateText,
	WeightKgText = @WeightKgText,
	WeightLbText = @WeightLbText,
	BodyweightKgText = @BodyweightKgText,
	BodyweightLbText = @BodyweightLbText,
	SetText = @SetText,
	RepText = @RepText,
	RpeText = @RpeText,
	Notes = @Notes
WHERE
	Reference = @Reference

SELECT @RecordImportRowId
".Trim();
            #endregion

            try
            {
                if(CanUpdateRow(dbConnection, row.Reference, userReference))
                {
                    int? importId = dbConnection.QueryFirstOrDefault<int?>(sql, row, dbTransaction);
                    if (importId.HasValue)
                    {
                        InsertImportRowErrors(dbConnection, dbTransaction, importId.Value, row.Errors);
                    }
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

        public void DeleteRow(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
DECLARE @RecordImportRowId INTEGER
SELECT @RecordImportRowId = RecordImportRowId FROM RecordImportRow WHERE Reference = @Reference

DELETE FROM RecordImportRowError
WHERE RecordImportRowId = @RecordImportRowId

DELETE FROM RecordImportRow WHERE Reference = @Reference
".Trim();
            #endregion

            object parameters = new { Reference = reference };

            try
            {
                if (CanUpdateRow(dbConnection, reference, userReference))
                {
                    dbConnection.QueryFirstOrDefault<int?>(sql, parameters, dbTransaction);
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

        #region Private Methods

        private void InsertImportRows(IDbConnection dbConnection, IDbTransaction dbTransaction, int importId, IEnumerable<RecordImportRowModel> rows)
        {
            #region SQL
            string sql = @"
INSERT INTO [RecordImportRow]
    ([RecordImportId]
    ,[Reference]
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
    ,[ExerciseId])
VALUES
    (@RecordImportId
    ,@Reference
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
    ,@Notes
    ,@ExerciseId);

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
                    row.Reference = Guid.NewGuid();

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
                    object parameters = new { RecordImportRowId = rowId, ErrorCode = error.ErrorCode };

                    try
                    {
                        dbConnection.Execute(sql, parameters, dbTransaction);
                    }
                    catch (Exception ex)
                    {
                        throw new DataAccessException(ex, sql, parameters);
                    }
                }
            }
        }

        private IEnumerable<RecordImportRowModel> SelectImportRows(IDbConnection dbConnection, int importId)
        {
            #region SQL
            string sql = @"
SELECT [RecordImportRowId]
      ,[Reference]
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
                        rows[i].Errors = SelectImportRowErrors(dbConnection, rows[i].RecordImportRowId);
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

        private IEnumerable<RecordImportRowErrorModel> SelectImportRowErrors(IDbConnection dbConnection, int rowId)
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

        private bool CanUpdateRow(IDbConnection dbConnection, Guid rowReference, Guid userRefernece)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 rir.Reference 
FROM RecordImportRow rir
	INNER JOIN RecordImport ri
		ON rir.RecordImportId = ri.RecordImportId
	INNER JOIN [User] u
		ON ri.UserId = u.UserId
WHERE 
	u.Reference = @UserReference AND
	rir.Reference = @Reference
".Trim();
            #endregion

            object parameters = new { Reference = rowReference, UserReference = userRefernece };

            try
            {
                Guid? reference = dbConnection.QueryFirstOrDefault<Guid?>(sql, parameters);
                return reference.HasValue;
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        private string GenerateRowFilterSql(int? offset, int? fetch)
        {
            string offsetFetch = "";
            string top = "";

            if (offset.HasValue && fetch.HasValue)
            {
                offsetFetch = "OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY";
            }
            else if (fetch.HasValue)
            {
                top = "TOP @Fetch";
            }

            return $@"
SELECT {top}
    rir.RecordImportRowId,
    rir.Reference,
    rir.RecordImportId,
    rir.StatusCode,
    rir.ExerciseText,
    rir.DateText,
    rir.WeightKgText,
    rir.WeightLbText,
    rir.BodyweightKgText,
    rir.BodyweightLbText,
    rir.SetText,
    rir.RepText,
    rir.RpeText,
    rir.Notes,
    rir.ExerciseId
FROM RecordImportRow rir
INNER JOIN RecordImport ri
	ON rir.RecordImportId = ri.RecordImportId
INNER JOIN [User] u
	ON ri.UserId = u.UserId
WHERE 
    ri.Reference = @ImportReference AND
	u.Reference = @UserReference AND
    ri.IsDeleted = 0
{offsetFetch}

SELECT COUNT(*) [Count]
FROM RecordImportRow rir
INNER JOIN RecordImport ri
	ON rir.RecordImportId = ri.RecordImportId
INNER JOIN [User] u
	ON ri.UserId = u.UserId
WHERE 
    ri.Reference = @ImportReference AND
	u.Reference = @UserReference AND
    ri.IsDeleted = 0
".Trim();
            
        }

        #endregion

    }
}
