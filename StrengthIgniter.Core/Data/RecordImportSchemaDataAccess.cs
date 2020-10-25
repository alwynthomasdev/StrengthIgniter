using CodeFluff.Extensions.IEnumerable;
using Dapper;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StrengthIgniter.Core.Data
{

    public interface IRecordImportSchemaDataAccess
    {
        IEnumerable<RecordImportSchemaModel> Select(Guid userReference);
        RecordImportSchemaModel Select(Guid reference, Guid userReference);
        RecordImportSchemaModel SelectForRow(Guid rowReference, Guid userReference);
        Tuple<IEnumerable<RecordImportSchemaModel>, int> Filter(string schemaName, Guid userReference, int? offset, int? fetch);
        RecordImportSchemaModel Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportSchemaModel schema);
        void Update(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportSchemaModel schema);
        void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid schemaId, Guid userReference);
    }

    public class RecordImportSchemaDataAccess : DataAccessBase, IRecordImportSchemaDataAccess
    {
        #region CTOR
        public RecordImportSchemaDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public IEnumerable<RecordImportSchemaModel> Select(Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT RecordImportSchemaId
      ,ris.Reference
      ,u.Reference AS UserReference
      ,ris.[Name]
      ,ris.Delimiter
FROM RecordImportSchema ris
    INNER JOIN [User] u
WHERE
    ris.IsDeleted = 0 AND
    u.Reference = @UserReference
    
".Trim();
            #endregion

            object parameters = new { UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();

                    RecordImportSchemaModel[] schemas = dbConnection.Query<RecordImportSchemaModel>(sql, parameters).TryToArray();
                    if(schemas.HasItems())
                    {
                        for(int i = 0; i<schemas.Length;i++)
                        {
                            int id = schemas[i].RecordImportSchemaId;
                            schemas[i].ColumnMap = SelectSchemaColumnMap(dbConnection, id);
                            schemas[i].ExerciseMap = SelectSchemaExerciseMap(dbConnection, id);
                        }
                    }
                    return schemas;
                }
            }
            catch(DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql);
            }
        }

        public RecordImportSchemaModel Select(Guid reference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1
     ris.RecordImportSchemaId
    ,ris.Reference
    ,u.Reference AS UserReference
    ,ris.[Name]
    ,ris.Delimiter
FROM RecordImportSchema ris
    INNER JOIN [User] u
        ON ris.UserId = u.UserId
WHERE
    ris.IsDeleted = 0 AND
    u.Reference = @UserReference AND
    ris.Reference = @Reference
".Trim();
            #endregion

            object parameters = new { Reference = reference, UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    RecordImportSchemaModel schema = dbConnection.QueryFirstOrDefault<RecordImportSchemaModel>(sql, parameters);
                    if(schema!=null)
                    {
                        schema.ColumnMap = SelectSchemaColumnMap(dbConnection, schema.RecordImportSchemaId);
                        schema.ExerciseMap = SelectSchemaExerciseMap(dbConnection, schema.RecordImportSchemaId);
                    }
                    return schema;
                }
            }
            catch (DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql);
            }
        }

        public RecordImportSchemaModel SelectForRow(Guid rowReference, Guid userReference)
        {
            #region SQL
            string sql = @"
SELECT TOP 1 
	 ris.RecordImportSchemaId
    ,ris.Reference
    ,u.Reference AS UserReference
    ,ris.[Name]
    ,ris.Delimiter
FROM RecordImportRow rir
	INNER JOIN RecordImport ri
		ON rir.RecordImportId = ri.RecordImportId
	INNER JOIN RecordImportSchema ris
		ON ri.RecordImportSchemaId = ris.RecordImportSchemaId
	INNER JOIN [User] u
		ON ris.UserId = u.UserId
WHERE
    ris.IsDeleted = 0 AND
	ri.IsDeleted = 0 AND
    u.Reference = @UserReference AND
    rir.Reference = @RecordImportRowReference
".Trim();
            #endregion

            object parameters = new { RecordImportRowReference = rowReference, UserReference = userReference };

            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    RecordImportSchemaModel schema = dbConnection.QueryFirstOrDefault<RecordImportSchemaModel>(sql, parameters);
                    if (schema != null)
                    {
                        schema.ColumnMap = SelectSchemaColumnMap(dbConnection, schema.RecordImportSchemaId);
                        schema.ExerciseMap = SelectSchemaExerciseMap(dbConnection, schema.RecordImportSchemaId);
                    }
                    return schema;
                }
            }
            catch (DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql);
            }

        }

        public Tuple<IEnumerable<RecordImportSchemaModel>, int> Filter(string schemaName, Guid userReference, int? offset, int? fetch)
        {
            string sql = GenerateFilterSql(schemaName, offset, fetch);
            object parameters = new { SearchString = schemaName, Offset = offset, Fetch = fetch };
            RecordImportSchemaModel[] schemas = null;
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
                            schemas = reader.Read<RecordImportSchemaModel>().TryToArray();

                            if (schemas.HasItems())
                            {
                                for (int i = 0; i < schemas.Length; i++)
                                {
                                    int id = schemas[i].RecordImportSchemaId;
                                    schemas[i].ColumnMap = SelectSchemaColumnMap(dbConnection, id);
                                    schemas[i].ExerciseMap = SelectSchemaExerciseMap(dbConnection, id);
                                }
                            }

                            total = reader.ReadSingle<int>();
                        }
                    }
                    return new Tuple<IEnumerable<RecordImportSchemaModel>, int>(schemas, total);
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

        public RecordImportSchemaModel Insert(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportSchemaModel schema)
        {
            #region SQL
            string sql = @"
DECLAER @UserId INTEGER
SELECT TOP 1 @UserId = UserId FROM [User] WHERE Reference = @UserReference

INSERT INTO RecordImportSchema
    (Reference
    ,UserId
    ,Name
    ,Delimiter)
VALUES
    (@Reference
    ,@UserId
    ,@Name
    ,@Delimiter)

SELECT SCOPE_INDETITIY()
".Trim();
            #endregion

            try
            {
                int? schemaId = dbConnection.QueryFirstOrDefault<int?>(sql, schema, dbTransaction);
                if (schemaId.HasValue)
                {
                    schema.RecordImportSchemaId = schemaId.Value;
                    InsertColumnMap(dbConnection, dbTransaction, schemaId.Value, schema.ColumnMap.TryToArray());
                    InsertExerciseMap(dbConnection, dbTransaction, schemaId.Value, schema.ExerciseMap.TryToArray());
                    return schema;
                }
                else throw new Exception("Unable to insert new schema");
            }
            catch(DataAccessException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, schema);
            }
        }

        public void Update(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportSchemaModel schema)
        {
            #region SQL
            string sql = @"
UPDATE RecordImportSchema
SET
    Name = @Name,
    Delimiter = @Delimiter
FROM 
	RecordImportSchema ris
	INNER JOIN [User] u
		ON ris.UserId = u.UserId
WHERE
	ris.Reference = @Reference
	AND u.Reference = @UserReference

SELECT TOP 1 RecordImportSchemaId FROM RecordImportSchema WHERE Reference = @Reference
".Trim();
            #endregion

            try
            {
                int? schemaId = dbConnection.QueryFirstOrDefault<int?>(sql, schema, dbTransaction);
                if (schemaId.HasValue)
                {
                    ClearSchemaMaps(dbConnection, dbTransaction, schema.RecordImportSchemaId);
                    InsertColumnMap(dbConnection, dbTransaction, schemaId.Value, schema.ColumnMap.TryToArray());
                    InsertExerciseMap(dbConnection, dbTransaction, schemaId.Value, schema.ExerciseMap.TryToArray());
                }
                else throw new Exception("Failed to get updated schemas id.");
            }
            catch(DataAccessException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, schema);
            }
        }

        public void Delete(IDbConnection dbConnection, IDbTransaction dbTransaction, Guid schemaReference, Guid userReference)
        {
            #region SQL
            string sql = @"
UPDATE RecordImportSchema
SET
    IsDeleted = 1,
    DeletedDateTimeUtc = GETUTCDATE()
FROM 
	RecordImportSchema ris
	INNER JOIN [User] u
		ON ris.UserId = u.UserId
WHERE
	ris.Reference = @Reference
	AND u.Reference = @UserReference
".Trim();
            #endregion

            object parameters = new { Reference = schemaReference, UserReference = userReference };

            try
            {
                dbConnection.Execute(sql, parameters, dbTransaction);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        #region Private Methods

        private IEnumerable<RecordImportSchemaColumnMapModel> SelectSchemaColumnMap(IDbConnection dbConnection, int schemaId)
        {
            #region SQL
            string sql = @"
SELECT  RecordImportSchemaColumnMapId,
        RecordImportSchemaId,
        HeaderName,
        ColumnTypeCode,
FROM    RecordImportSchemaColumnMap 
WHERE   RecordImportSchemaId = @RecordImportSchemaId
".Trim();
            #endregion

            object parameters = new { RecordImportSchemaId = schemaId };

            try
            {
                return dbConnection.Query<RecordImportSchemaColumnMapModel>(sql, parameters);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        private IEnumerable<RecordImportSchemaExerciseMapModel> SelectSchemaExerciseMap(IDbConnection dbConnection, int schemaId)
        {
            #region SQL
            string sql = @"
SELECT      RecordImportSchemaExerciseMapId,
            RecordImportSchemaId,
            ExerciseId,
            ExercieReference,
            [Text]
FROM        RecordImportSchemaExerciseMap map
INNER JOIN  Exercise ex ON map.ExerciseId = ex.ExerciseId
WHERE       RecordImportSchemaId = @RecordImportSchemaId
".Trim() ;
            #endregion

            object parameters = new { RecordImportSchemaId = schemaId };

            try
            {
                return dbConnection.Query<RecordImportSchemaExerciseMapModel>(sql, parameters);
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        private void InsertColumnMap(IDbConnection dbConnection, IDbTransaction dbTransaction, int schemaId, RecordImportSchemaColumnMapModel[] maps)
        {
            #region SQL
            string sql = @"
INSERT INTO RecordImportSchemaColumnMap
    (RecordImportSchemaId
    ,HeaderName
    ,ColumnTypeCode)
VALUES
    (@RecordImportSchemaId
    ,@HeaderName
    ,@ColumnTypeCode)
".Trim();
            #endregion

            for(int i = 0; i < maps.Length; i++)
            {
                maps[i].RecordImportSchemaId = schemaId;
                try
                {
                    dbConnection.Execute(sql, maps[i], dbTransaction);
                }
                catch (Exception ex)
                {
                    throw new DataAccessException(ex, sql, maps[i]);
                }
            }
        }

        private void InsertExerciseMap(IDbConnection dbConnection, IDbTransaction dbTransaction, int schemaId, RecordImportSchemaExerciseMapModel[] maps)
        {
            #region SQL
            string sql = @"
INSERT INTO RecordImportSchemaExerciseMap
    (RecordImportSchemaId
    ,ExerciseId
    ,Text)
VALUES
    (@RecordImportSchemaId
    ,@ExerciseId
    ,@Text)
".Trim();
            #endregion

            for(int i = 0; i<maps.Length; i++)
            {
                RecordImportSchemaExerciseMapModel map = maps[i];
                map.RecordImportSchemaId = schemaId;

                try
                {
                    dbConnection.Execute(sql, map, dbTransaction);
                }
                catch(Exception ex)
                {
                    throw new DataAccessException(ex, sql, map);
                }
            }
        }

        private void ClearSchemaMaps(IDbConnection dbConnection, IDbTransaction dbTransaction, int schemaId)
        {
            #region SQL
            string sql = @"
DELETE FROM RecordImportSchemaColumnMap WHERE RecordImportSchemaId = @RecordImportSchemaId
DELETE FROM RecordImportSchemaExerciseMap WHERE RecordImportSchemaId = @RecordImportSchemaId
".Trim();
            #endregion

            object parameters = new { RecordImportSchemaId = schemaId };

            try
            {
                dbConnection.Execute(sql, parameters, dbTransaction);
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sql, parameters);
            }
        }

        private string GenerateFilterSql(string schemaName, int? offset, int? fetch)
        {
            // define variables
            string top = "";
            string where = "";
            string offsetFetchRows = "";

            // set the where clause if there is an exercise search string
            where = !string.IsNullOrWhiteSpace(schemaName) ? @"AND  ex.[Name] LIKE '%'+@SearchString+'%'" : "";

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
SELECT  {top}
        ris.RecordImportSchemaId,
        ris.Reference,
        u.Reference AS UserReference,
        ris.[Name],
        ris.Delimiter
FROM        RecordImportSchema ris
INNER JOIN  [User] u ON ris.UserId = u.UserId
WHERE ris.IsDeleted = 0
AND u.Reference = @UserReference
{where}
ORDER BY    ex.[Name]
{offsetFetchRows};

SELECT      COUNT(*) AS Count
FROM        RecordImportSchema ris
INNER JOIN  [User] u
WHERE ris.IsDeleted = 0
AND u.Reference = @UserReference
{where}
".Trim();
        }

        #endregion

    }
}
