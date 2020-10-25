using CodeFluff.Extensions.IEnumerable;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
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

        void ProcessRow(Guid rowReference, Guid userReference);
        void ProcessRows(IEnumerable<Guid> rowReferences, Guid userReference);

        void UpdateRow(RecordImportRowModel row, Guid userReference);
        void DeleteImport(Guid reference, Guid userReference);
        void DeleteRow(Guid reference, Guid userReference);

        [Obsolete("This is a temporary method for processing all prepared rows", false)]
        void ProcessImport(Guid reference, Guid userReference);
    }

    public class RecordImportService : ServiceBase, IRecordImportService
    {
        #region CTOR
        private readonly IRecordImportSchemaDataAccess _RecordImportSchemaDal;
        private readonly IRecordImportDataAccess _RecordImportDal;
        private readonly IExerciseDataAccess _ExerciseDal;
        private readonly IRecordDataAccess _RecordDal;
        private readonly IPaginationUtility _PaginationUtility;


        public RecordImportService(
            IRecordImportDataAccess recordImportDal,
            IRecordImportSchemaDataAccess recordImportSchemaDal,
            IExerciseDataAccess exerciseDal,
            IRecordDataAccess recordDal,
            IPaginationUtility paginationUtility,
            //
            IAuditEventDataAccess auditEventDal,
            ILogger<RecordImportService> logger,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDal, logger, dbConnectionFactory.GetConnection)
        {
            _RecordImportDal = recordImportDal;
            _RecordImportSchemaDal = recordImportSchemaDal;
            _ExerciseDal = exerciseDal;
            _RecordDal = recordDal;
            _PaginationUtility = paginationUtility;
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
                RecordImportSchemaModel schema = GetSchema(request.SchemaReference, request.UserReference);
                import.Rows = ReadCsvRows(request.CsvFile, schema);
                import.Rows = ValidateRows(import.Rows, schema);

                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();

                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        _RecordImportDal.Insert(dbConnection, dbTransaction, import);
                        CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.ImportNew, request.UserReference, $"New record import '{import.Name}' with reference '{import.Reference}'.", new AuditEventItemModel[] { 
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

        //TODO: temporary method, replace with process multiple rows method
        [Obsolete("This is a temporary method for processing all prepared rows", false)]
        public void ProcessImport(Guid reference, Guid userReference)
        {
            try
            {
                RecordImportModel import = _RecordImportDal.Select(reference, userReference);
                if(import.Rows.HasItems())
                {
                    using (IDbConnection dbConnection = GetConnection())
                    {
                        dbConnection.Open();
                        using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                        {
                            foreach (RecordImportRowModel row in import.Rows)
                            {
                                ProcessRow(dbConnection, dbTransaction, row, userReference, true);
                            }
                            CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.ImportProcessed, userReference, $"Record import processed.", new AuditEventItemModel[] {
                                new AuditEventItemModel{ Key = "ImportReference", Value = import.Reference.ToString() },
                                new AuditEventItemModel{ Key = "ImportName", Value = import.Name }
                            });
                            dbTransaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference, userReference });
                throw serviceException;
            }

        }

        public void ProcessRow(Guid rowReference, Guid userReference)
        {
            try
            {
                RecordImportRowModel row = _RecordImportDal.SelectRow(rowReference, userReference);
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        ProcessRow(dbConnection, dbTransaction, row, userReference, false);
                        dbTransaction.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { rowReference, userReference });
                throw serviceException;
            }
        }

        public void ProcessRows(IEnumerable<Guid> rowReferences, Guid userReference)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        foreach(Guid reference in rowReferences)
                        {
                            RecordImportRowModel row = _RecordImportDal.SelectRow(reference, userReference);
                            ProcessRow(dbConnection, dbTransaction, row, userReference, false);
                        }
                        dbTransaction.Commit();

                    }
                }
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { rowReferences, userReference });
                throw serviceException;
            }
        }

        public RecordImportModel GetByReference(Guid reference, Guid userReference)
        {
            try
            {
                return _RecordImportDal.Select(reference, userReference);
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference, userReference });
                throw serviceException;
            }
        }

        public IEnumerable<RecordImportModel> GetUserImports(Guid userReference)
        {
            try
            {
                return _RecordImportDal.Select(userReference);
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { userReference });
                throw serviceException;
            }
        }

        public void DeleteImport(Guid reference, Guid userReference)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        _RecordImportDal.Delete(dbConnection, dbTransaction, reference, userReference);
                        CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.ImportDeleted, userReference, $"Import with reference {reference} deleted.", new AuditEventItemModel[] { 
                           new AuditEventItemModel{ Key = "ImportReference", Value = reference.ToString() } 
                        });

                        dbTransaction.Commit();
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference, userReference });
                throw serviceException;
            }
        }

        public ImportRowSearchResponse RowSearch(ImportRowSearchRequest request)
        {
            try
            {
                int? offset = _PaginationUtility.GetPageOffset(request.PageNo, request.PageLength);
                Tuple<IEnumerable<RecordImportRowModel>, int> result = _RecordImportDal.Filter(request.RecordImportReference, request.UserReference, offset, request.PageLength);
                return new ImportRowSearchResponse { ImportRows =result.Item1, TotalMatches = result.Item2 };
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        public void UpdateRow(RecordImportRowModel row, Guid userReference)
        {
            try
            {
                RecordImportSchemaModel schema = _RecordImportSchemaDal.Select(row.Reference, userReference);
                row = ValidateRow(row, schema);

                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        _RecordImportDal.UpdateRow(dbConnection, dbTransaction, row, userReference);
                        CreateAuditEvent(AuditEventType.ImportRowUpdated, userReference, $"Updated record import row.", new AuditEventItemModel[] { 
                            new AuditEventItemModel{ Key = "Reference", Value = row.Reference.ToString() }
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { row, userReference });
                throw serviceException;
            }
        }

        public void DeleteRow(Guid reference, Guid userReference)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        _RecordImportDal.DeleteRow(dbConnection, dbTransaction, reference, userReference);
                        CreateAuditEvent(AuditEventType.ImportRowDeleted, userReference, "Record import row deleted.", new AuditEventItemModel[] {
                            new AuditEventItemModel { Key = "Reference", Value = reference.ToString() }
                        });
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference, userReference });
                throw serviceException;
            }
        }

        #region Private Methods

        private void ProcessRow(IDbConnection dbConnection, IDbTransaction dbTransaction, RecordImportRowModel row, Guid userReference, bool suppressAudit = false)
        {
            if (row.StatusCode == ImportRowStatusCode.Ready)
            {
                RecordModel record = ConvertImportRowToRecord(row, userReference);
                int recordId = _RecordDal.Insert(dbConnection, dbTransaction, record);
                _RecordImportDal.UpdateRowStatus(dbConnection, dbTransaction, row.RecordImportRowId, userReference, ImportRowStatusCode.Processed);
                if (!suppressAudit)
                {
                    CreateAuditEvent(dbConnection, dbTransaction, AuditEventType.ImportRowProcessed, userReference, $"Record import row processed.", new AuditEventItemModel[] {
                    new AuditEventItemModel { Key = "RecordImportRowId", Value = row.RecordImportRowId.ToString(), },
                    new AuditEventItemModel { Key = "RecordImportId", Value = row.RecordImportId.ToString(), }
                });
                }
            }
            //TODO: something if the row is not ready
        }

        private RecordImportSchemaModel GetSchema(Guid schemaReference, Guid userReference)
        {
            RecordImportSchemaModel schema = _RecordImportSchemaDal.Select(schemaReference, userReference);
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
                //TODO: make case sensitivity configurable in schema ???
                RecordImportSchemaExerciseMapModel map = schema.ExerciseMap.Where(m => m.Text.ToLower() == row.ExerciseText.ToLower()).FirstOrDefault();
                if (map != null)
                {
                    ExerciseModel exercise = _ExerciseDal.Select(map.ExerciseId);
                    if (exercise != null)
                    {
                        row.ExerciseText = exercise.Name;
                        row.ExerciseId = map.ExerciseId;
                    }
                    else throw new Exception($"Unable to find exercise with id {map.ExerciseId}.");//TODO: should this be an exception ???
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

            //ensure only one weight is provided (kg or lb)
            if(!string.IsNullOrWhiteSpace(row.WeightKgText) && !string.IsNullOrWhiteSpace(row.WeightLbText))
            {
                errors.Add(ErrorCode.WeightDuplicate);
            }

            //validate WeightKg
            if (!string.IsNullOrWhiteSpace(row.WeightKgText))
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

            //ensure only one weight is provided (kg or lb)
            if (!string.IsNullOrWhiteSpace(row.BodyweightKgText) && !string.IsNullOrWhiteSpace(row.BodyweightLbText))
            {
                errors.Add(ErrorCode.WeightDuplicate);
            }

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

        private RecordModel ConvertImportRowToRecord(RecordImportRowModel row, Guid userReference)
        {
            //TODO: extra validation here ???

            RecordModel record = new RecordModel();

            record.UserReference = userReference;
            record.ExerciseId = row.ExerciseId;
            record.Date = DateTime.Parse(row.DateText);

            if(!string.IsNullOrWhiteSpace(row.SetText))
            {
                record.Sets = int.Parse(row.SetText);
            }
            record.Reps = int.Parse(row.RepText);

            if (!string.IsNullOrWhiteSpace(row.WeightLbText))
            {
                decimal lb = decimal.Parse(row.WeightLbText);
                record.WeightKg = WeightConversionHelper.ConvertLbToKg(lb);//TODO: round?
            }
            else if (!string.IsNullOrWhiteSpace(row.WeightKgText))
            {
                record.WeightKg = decimal.Parse(row.WeightKgText);
            }

            if (!string.IsNullOrWhiteSpace(row.BodyweightLbText))
            {
                decimal lb = decimal.Parse(row.BodyweightLbText);
                record.BodyweightKg = WeightConversionHelper.ConvertLbToKg(lb);//TODO: round?
            }
            else if (!string.IsNullOrWhiteSpace(row.BodyweightKgText))
            {
                record.BodyweightKg = decimal.Parse(row.BodyweightKgText);
            }

            if (!string.IsNullOrWhiteSpace(row.RpeText))
            {
                record.RPE = decimal.Parse(row.RpeText);
            }
            record.Notes = row.Notes;

            record.CreatedDateTimeUtc = DateTime.UtcNow;

            return record;
        }

        #endregion

    }

    #region RecordImportService Models

    public class NewImportRequest
    {
        public Guid UserReference { get; set; }
        public Stream CsvFile { get; set; }
        public string Name { get; set; }
        public Guid SchemaReference { get; set; }
    }

    public class ImportSearchRequest
    {
        public Guid UserReference { get; set; }
        public int PageNo { get; set; }
        public int PageLength { get; set; }
    }

    public class ImportSearchResponse
    {
        public IEnumerable<RecordImportRowModel> ImportRows { get; internal set; }
        public int TotalMatches { get; internal set; }
    }

    public class ImportRowSearchRequest
    {
        //put any extra search criteria here
        public Guid RecordImportReference { get; set; }
        public Guid UserReference { get; set; }
        public int PageNo { get; set; }
        public int PageLength { get; set; }
    }

    public class ImportRowSearchResponse
    {
        public IEnumerable<RecordImportRowModel> ImportRows { get; internal set; }
        public int TotalMatches { get; internal set; }
    }

    #endregion

}
