using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Core.Services
{

    public interface IRecordImportSchemaService
    {
        IEnumerable<RecordImportSchemaModel> GetAllSchemas();
    }

    public class RecordImportSchemaService : ServiceBase, IRecordImportSchemaService
    {
        #region CTOR
        private readonly IRecordImportSchemaDataAccess _RecordImportSchemeDal;

        public RecordImportSchemaService(
            IRecordImportSchemaDataAccess recordImportSchemeDal,
            //
            ILogger<RecordImportSchemaService> logger,
            IAuditEventDataAccess auditEventDataAccess,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDataAccess, logger, dbConnectionFactory.GetConnection)
        {
            _RecordImportSchemeDal = recordImportSchemeDal;
        }
        #endregion

        public IEnumerable<RecordImportSchemaModel> GetAllSchemas()
        {
            try
            {
                return _RecordImportSchemeDal.GetAll();
            }
            catch(Exception ex)
            {
                ServiceException serviceException = CreateServiceException(ex, MethodInfo.GetCurrentMethod().Name, null);
                throw serviceException;
            }
        }
    }

}
