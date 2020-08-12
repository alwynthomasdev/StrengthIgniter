using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Services.Infrastructure;
using System;
using System.Collections.Generic;
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
            ILoggerFactory loggerFactory,
            IAuditEventDataAccess auditEventDataAccess,
            DatabaseConnectionFactory dbConnectionFactory
        )
            : base(auditEventDataAccess, loggerFactory.CreateLogger(typeof(RecordImportSchemaService)), dbConnectionFactory.GetConnection)
        {
            _RecordImportSchemeDal = recordImportSchemeDal;
        }
        #endregion

        public IEnumerable<RecordImportSchemaModel> GetAllSchemas()
        {
            return _RecordImportSchemeDal.GetAll();
        }
    }

}
