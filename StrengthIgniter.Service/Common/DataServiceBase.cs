using Microsoft.Extensions.Logging;
using StrengthIgniter.Dal.AuditEvent;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.AuditEvent;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Common
{
    public abstract class DataServiceBase : ServiceBase
    {
        private readonly IDataAccessTransactionProvider _TransactionProvider;
        private readonly IAuditEventDataAccess _AuditEventDataAccess;

        public DataServiceBase(
            IDataAccessTransactionProvider transactionProvider,
            IAuditEventDataAccess auditEventDataAccess,
            ILogger logger
        )
            :base(logger)
        {
            _TransactionProvider = transactionProvider;
            _AuditEventDataAccess = auditEventDataAccess;
        }

        protected IDataAccessTransaction BeginTansaction()
        {
            return _TransactionProvider.BeginTansaction();
        }

        protected int CreateAuditEvent(
            string eventType, 
            string details, 
            IEnumerable<AuditEventItemModel> items, 
            Guid? userReference, 
            int? relatedAuditId = null)
        {
            AuditEventModel auditEvent = new AuditEventModel
            {
                AuditEventDateTimeUtc = DateTime.UtcNow,
                EventType = eventType,
                Details = details,
                RelatedUserReference = userReference,
                RelatedServiceName = GetServiceName(),
                RelatedAuditEventId = relatedAuditId,
                Items = items
            };
            return _AuditEventDataAccess.Insert(auditEvent);
        }

        protected int CreateAuditEvent(
            IDataAccessTransaction dataAccessTransaction,
            string eventType, 
            string details, 
            IEnumerable<AuditEventItemModel> items, 
            Guid? userReference, 
            int? relatedAuditId = null)
        {
            AuditEventModel auditEvent = new AuditEventModel
            {
                AuditEventDateTimeUtc = DateTime.UtcNow,
                EventType = eventType,
                Details = details,
                RelatedUserReference = userReference,
                RelatedServiceName = GetServiceName(),
                RelatedAuditEventId = relatedAuditId,
                Items = items
            };
            return _AuditEventDataAccess.Insert(auditEvent, dataAccessTransaction.DbTransaction);
        }

    }
}
