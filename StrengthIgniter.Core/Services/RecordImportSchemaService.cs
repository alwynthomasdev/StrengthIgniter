using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{

    public interface IRecordImportSchemaService
    {
        RecordImportSchemaModel GetRecordImportSchema(Guid reference, Guid userReference);
        IEnumerable<RecordImportSchemaModel> GetRecordImportSchemaList(Guid userReference);
        FilterResponse<RecordImportSchemaModel> FilterRecordImportSchemas(FilterRequest request);

        RecordImportSchemaModel SaveRecordImportSchema(RecordImportSchemaModel schema);
        void DeleteRecordImportSchema(Guid reference, Guid userReference);
    }

    public class RecordImportSchemaService : ServiceBase, IRecordImportSchemaService
    {
        #region CTOR
        private readonly IRecordImportSchemaDataAccess _RecordImportSchemeDal;
        private readonly IPaginationUtility _PaginationUtility;

        public RecordImportSchemaService(
            IRecordImportSchemaDataAccess recordImportSchemeDal,
            IPaginationUtility paginationUtility,
            //
            ILogger<RecordImportSchemaService> logger,
            IAuditEventDataAccess auditEventDataAccess,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDataAccess, logger, dbConnectionFactory.GetConnection)
        {
            _RecordImportSchemeDal = recordImportSchemeDal;
            _PaginationUtility = paginationUtility;
        }
        #endregion

        public RecordImportSchemaModel GetRecordImportSchema(Guid reference, Guid userReference)
        {
            try
            {
                return _RecordImportSchemeDal.Select(reference, userReference);
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { reference, userReference });
                throw serviceException;
            }
        }

        public IEnumerable<RecordImportSchemaModel> GetRecordImportSchemaList(Guid userReference)
        {
            try
            {
                return _RecordImportSchemeDal.Select(userReference);
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, new { userReference });
                throw serviceException;
            }
        }

        public FilterResponse<RecordImportSchemaModel> FilterRecordImportSchemas(FilterRequest request)
        {
            try
            {
                int? offset = _PaginationUtility.GetPageOffset(request.PageNo, request.PageLength);
                Tuple<IEnumerable<RecordImportSchemaModel>, int> result = _RecordImportSchemeDal.Filter(request.SearchString, request.UserReference, offset, request.PageLength);

                if (result != null)
                {
                    return new FilterResponse<RecordImportSchemaModel>(result.Item1, result.Item2);
                }
                else return new FilterResponse<RecordImportSchemaModel>();
            }
            catch (Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, request);
                throw serviceException;
            }
        }

        public RecordImportSchemaModel SaveRecordImportSchema(RecordImportSchemaModel schema)
        {
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    dbConnection.Open();
                    using (IDbTransaction dbTransaction = dbConnection.BeginTransaction())
                    {
                        if(schema.Reference == Guid.Empty)
                        {
                            //TODO: insert
                        }
                        else
                        {
                            //TODO: update
                        }
                        return schema;
                    }
                }
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, schema);
                throw serviceException;
            }
        }

        public void DeleteRecordImportSchema(Guid reference, Guid userReference)
        {
            //TODO: create the delete method for recrod import schema on the service
        }

    }

}
