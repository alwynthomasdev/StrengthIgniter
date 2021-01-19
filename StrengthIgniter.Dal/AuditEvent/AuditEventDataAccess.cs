using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.AuditEvent;
using System;
using System.Data;

namespace StrengthIgniter.Dal.AuditEvent
{
    

    public class AuditEventDataAccess : DataAccessBase, IAuditEventDataAccess
    {
        #region CTOR
        public AuditEventDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public int Insert(AuditEventModel auditEvent, IDbTransaction dbTransaction)
        {
            IDbConnection dbConnection = dbTransaction != null ? dbTransaction.Connection : GetConnection();
            string sp = "dbo.spAuditEventInsert";
            var parameters = new
            {
                EventType = auditEvent.EventType,
                Details = auditEvent.Details,
                RelatedServiceName = auditEvent.RelatedServiceName,
                UserReference = auditEvent.RelatedUserReference,
                RelatedAuditEventId = auditEvent.RelatedAuditEventId,
                AuditEventItems = auditEvent.Items.ToDataTable().AsTableValuedParameter("dbo.uttKeyValuePair")
            };

            try
            {
                int? aid = dbConnection.QueryFirstOrDefault<int>(sp, parameters, dbTransaction, commandType: CommandType.StoredProcedure);
                if (!aid.HasValue)
                {
                    throw new Exception("Failed to create audit event.");
                }
                return aid.Value;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sp, parameters);
            }
        }

    }
}
