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

    public interface IRecordImportSchemaDataAccess
    {
        //TODO: modify to get schemas by user
        IEnumerable<RecordImportSchemaModel> GetAll();

        RecordImportSchemaModel GetByReference(Guid reference);
    }

    public class RecordImportSchemaDataAccess : DataAccessBase, IRecordImportSchemaDataAccess
    {
        #region CTOR
        public RecordImportSchemaDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public IEnumerable<RecordImportSchemaModel> GetAll()
        {
            #region SQL
            string sql = @"
SELECT [RecordImportSchemaId]
      ,[Reference]
      --,[UserId]
      ,[Name]
      ,[Delimiter]
FROM [RecordImportSchema]
WHERE
    [IsDeleted] = 0
".Trim();
            #endregion

            try
            {

                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();

                    RecordImportSchemaModel[] schemas = dbConnection.Query<RecordImportSchemaModel>(sql).TryToArray();
                    if(schemas.HasItems())
                    {
                        for(int i = 0; i<schemas.Length;i++)
                        {
                            int id = schemas[i].RecordImportSchemaId;
                            schemas[i].ColumnMap = GetSchemaColumnMap(dbConnection, id);
                            schemas[i].ExerciseMap = GetSchemaExerciseMap(dbConnection, id);
                        }
                    }
                    return schemas;
                }
            }
            catch(DataAccessException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql);
            }
        }

        public RecordImportSchemaModel GetByReference(Guid reference)
        {
            #region SQL
            string sql = @"
SELECT [RecordImportSchemaId]
      ,[Reference]
      --,[UserId]
      ,[Name]
      ,[Delimiter]
FROM [RecordImportSchema]
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
                    RecordImportSchemaModel schema = dbConnection.QueryFirstOrDefault<RecordImportSchemaModel>(sql, parameters);
                    if(schema!=null)
                    {
                        schema.ColumnMap = GetSchemaColumnMap(dbConnection, schema.RecordImportSchemaId);
                        schema.ExerciseMap = GetSchemaExerciseMap(dbConnection, schema.RecordImportSchemaId);
                    }
                    return schema;
                }
            }
            catch (DataAccessException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql);
            }
        }

        #region Private Methods

        private IEnumerable<RecordImportSchemaColumnMapModel> GetSchemaColumnMap(IDbConnection dbConnection, int schemaId)
        {
            #region SQL
            string sql = @"
SELECT [RecordImportSchemaColumnMapId]
      ,[RecordImportSchemaId]
      ,[HeaderName]
      ,[ColumnTypeCode]
FROM [RecordImportSchemaColumnMap]
WHERE [RecordImportSchemaId] = @RecordImportSchemaId
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

        private IEnumerable<RecordImportSchemaExerciseMapModel> GetSchemaExerciseMap(IDbConnection dbConnection, int schemaId)
        {
            #region SQL
            string sql = @"
SELECT [RecordImportSchemaExerciseMapId]
      ,[RecordImportSchemaId]
      ,[ExerciseId]
      ,[Text]
FROM [RecordImportSchemaExerciseMap]
WHERE [RecordImportSchemaId] = @RecordImportSchemaId
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

        #endregion


    }
}
