using CodeFluff.Extensions.IEnumerable;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface IRecordImportService
    {
        Guid Import(NewImportRequest request);
        RecordImportModel GetByReference(Guid reference, Guid userReference);
        IEnumerable<RecordImportModel> GetUserImports(Guid userReference);
    }

    public class RecordImportService : ServiceBase, IRecordImportService
    {
        #region CTOR
        IRecordImportSchemaDataAccess _RecordImportSchemaDal;
        IRecordImportDataAccess _RecordImportDal;
        IExerciseDataAccess _ExerciseDal;

        public RecordImportService(
            IRecordImportDataAccess recordImportDal,
            IRecordImportSchemaDataAccess recordImportSchemaDal,
            IExerciseDataAccess exerciseDal,
            //
            IAuditEventDataAccess auditEventDal,
            ILoggerFactory loggerFactory,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDal, loggerFactory.CreateLogger(typeof(RecordImportService)), dbConnectionFactory.GetConnection)
        {
            _RecordImportDal = recordImportDal;
            _RecordImportSchemaDal = recordImportSchemaDal;
            _ExerciseDal = exerciseDal;
        }
        #endregion

        public Guid Import(NewImportRequest request)
        {
            try
            {
                RecordImportModel import = new RecordImportModel
                {
                    Reference = Guid.NewGuid(),
                    Name = request.Name,
                    UserReference = request.UserReference,
                    RecordImportSchemaReference = request.SchemaReference,
                    ImportDateTimeUtc = DateTime.UtcNow
                };
                RecordImportSchemaModel schema = GetSchema(request.SchemaReference);
                import.Rows = ReadCsvRows(request.CsvFile, schema);
                import.Rows = ValidateRows(import.Rows, schema);

                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();

                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        _RecordImportDal.Insert(dbConnection, dbTransaction, import);
                        CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.NewImport, request.UserReference, $"New record import '{import.Name}' with reference '{import.Reference}'.", new AuditEventItemModel[] { 
                            new AuditEventItemModel { Key = "ImportReference", Value = import.Reference.ToString() },
                            new AuditEventItemModel { Key = "ImportName", Value = import.Name },
                        });

                        dbTransaction.Commit();
                        
                        return import.Reference;
                    }
                }

            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        public RecordImportModel GetByReference(Guid reference, Guid userReference)
        {
            return _RecordImportDal.GetByReference(reference, userReference);
        }

        public IEnumerable<RecordImportModel> GetUserImports(Guid userReference)
        {
            return _RecordImportDal.GetByUserReference(userReference);
        }


        #region Private Methods

        private RecordImportSchemaModel GetSchema(Guid schemaReference)
        {
            RecordImportSchemaModel schema = _RecordImportSchemaDal.GetByReference(schemaReference);
            if (schema == null)
            {
                throw new Exception($"Unable to find Record Import Schema with unique id '{schemaReference}'.");
            }
            else return schema;
        }

        private List<RecordImportRowModel> ReadCsvRows(Stream csvFile, RecordImportSchemaModel schema)
        {
            using (StreamReader streamReader = new StreamReader(csvFile))
            {
                using (CsvReader csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture))
                {
                    csvReader.Configuration.Delimiter = schema.Delimiter;
                    csvReader.Configuration.RegisterClassMap(new RecordImportMap(schema));
                    return csvReader.GetRecords<RecordImportRowModel>().ToList();//write to memory (potentialy expensive)
                }
            }
        }

        private IEnumerable<RecordImportRowModel> ValidateRows(IEnumerable<RecordImportRowModel> rows, RecordImportSchemaModel schema)
        {
            RecordImportRowModel[] aryRows = rows.TryToArray();
            for(int i = 0; i<aryRows.Length;i++)
            {
                aryRows[i] = ValidateRow(aryRows[i], schema);
            }
            return aryRows;
        }

        private RecordImportRowModel ValidateRow(RecordImportRowModel row, RecordImportSchemaModel schema)
        {
            List<ErrorCode> errors = new List<ErrorCode>();

            //Validate exercise
            if (!string.IsNullOrEmpty(row.ExerciseText))
            {
                RecordImportSchemaExerciseMapModel map = schema.ExerciseMap.Where(m => m.Text == row.ExerciseText).FirstOrDefault();
                if (map != null)
                {
                    ExerciseModel exercise = _ExerciseDal.GetById(map.ExerciseId);
                    if (exercise != null)
                    {
                        row.ExerciseText = exercise.Name;
                        row.ExerciseId = map.ExerciseId;
                    }
                    else throw new Exception($"Unable to find exercise with id {map.ExerciseId}.");//TODO: should this be an exception
                }
                else errors.Add(ErrorCode.ExerciseCannotMap);
            }
            else errors.Add(ErrorCode.ExerciseRequired);

            //validate date
            if (!string.IsNullOrWhiteSpace(row.DateText))
            {
                //TODO: more thorough check !
                if (!DateTime.TryParse(row.DateText, out DateTime dt))
                {
                    errors.Add(ErrorCode.DateInvalid);
                }
            }
            else errors.Add(ErrorCode.DateRequired);

            //validate WeightKg
            if(!string.IsNullOrWhiteSpace(row.WeightKgText))
            {
                if(!decimal.TryParse(row.WeightKgText, out decimal d))
                {
                    errors.Add(ErrorCode.WeightKgInvalid);
                }
            }
            //else not required...

            //validate WeightLb
            if (!string.IsNullOrWhiteSpace(row.WeightLbText))
            {
                if (!decimal.TryParse(row.WeightLbText, out decimal d))
                {
                    errors.Add(ErrorCode.WeightLbInvalid);
                }
            }
            //else not required...

            //validate BodyweightKg
            if (!string.IsNullOrWhiteSpace(row.BodyweightKgText))
            {
                if (!decimal.TryParse(row.BodyweightKgText, out decimal d))
                {
                    errors.Add(ErrorCode.BodyweightKgInvalid);
                }
            }
            //else not required...

            //validate BodyweightLbInvalid
            if (!string.IsNullOrWhiteSpace(row.BodyweightLbText))
            {
                if (!decimal.TryParse(row.BodyweightLbText, out decimal d))
                {
                    errors.Add(ErrorCode.BodyweightLbInvalid);
                }
            }
            //else not required...

            //validate Set
            if (!string.IsNullOrWhiteSpace(row.SetText))
            {
                if (!int.TryParse(row.SetText, out int i))
                {
                    errors.Add(ErrorCode.SetInvalid);
                }
            }
            //else not required...

            //validate rep
            if (!string.IsNullOrWhiteSpace(row.RepText))
            {
                if (!int.TryParse(row.RepText, out int i))
                {
                    errors.Add(ErrorCode.RepInvalid);
                }
            }
            else errors.Add(ErrorCode.RepRequired);

            //validate rpe
            if (!string.IsNullOrWhiteSpace(row.RpeText))
            {
                if (!decimal.TryParse(row.RpeText, out decimal d))
                {
                    errors.Add(ErrorCode.RpeInvalid);
                }
            }
            //else not required

            if (errors.HasItems())
            {
                row.StatusCode = ImportRowStatusCode.Error;
            }
            else row.StatusCode = ImportRowStatusCode.Ready;

            row.Errors = errors.Select(err => new RecordImportRowErrorModel { ErrorCode = err });

            return row;
        }

        #endregion

    }

    #region Models

    public class NewImportRequest
    {
        public Guid UserReference { get; set; }
        public Stream CsvFile { get; set; }
        public string Name { get; set; }
        public Guid SchemaReference { get; set; }
    }

    #endregion

}
