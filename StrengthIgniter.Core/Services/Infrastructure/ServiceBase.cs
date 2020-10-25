using CodeFluff.Extensions.IEnumerable;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Core.Services.Infrastructure
{
    public abstract class ServiceBase
    {
        #region CTOR
        private readonly IAuditEventDataAccess _AuditEventDal;
        private readonly ILogger _Logger;
        public readonly Func<IDbConnection> GetConnection;

        public ServiceBase(
            IAuditEventDataAccess auditEventDal,
            ILogger logger,
            Func<IDbConnection> fnGetConnection
        )
        {
            _AuditEventDal = auditEventDal;
            _Logger = logger;
            GetConnection = fnGetConnection;
        }
        #endregion

        private string GetServiceName()
        {
            return this.GetType().Name;
        }

        #region Logger Helpers ...

        protected void LogInfo(string message)
        {
            _Logger.Log(LogLevel.Information, message);
        }

        protected void LogWarning(string message)
        {
            _Logger.Log(LogLevel.Warning, message);
        }

        protected void LogDebug(string message)
        {
            _Logger.Log(LogLevel.Debug, message);
        }

        #endregion ... Logger Helpers

        #region Audit Helpers ...

        protected int CreateAuditEvent(string eventType, int? userId, string details, IEnumerable<AuditEventItemModel> items, int? relatedAuditId = null)
        {
            AuditEventModel auditEvent = new AuditEventModel
            {
                AuditEventDateTimeUtc = DateTime.UtcNow,
                EventType = eventType,
                Details = details,
                RelatedUserId = userId,
                RelatedServiceName = GetServiceName(),
                RelatedAuditEventId = relatedAuditId,
                Items = items
            };
            return _AuditEventDal.Insert(auditEvent);
        }
        protected int CreateAuditEvent(string eventType, Guid userReference, string details, IEnumerable<AuditEventItemModel> items, int? relatedAuditId = null)
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
            return _AuditEventDal.Insert(auditEvent);
        }

        protected int CreateAuditEvent(IDbConnection dbConnection, IDbTransaction dbTransaction, 
            string eventType, int? userId, string details, IEnumerable<AuditEventItemModel> items, int? relatedAuditId = null)
        {
            AuditEventModel auditEvent = new AuditEventModel
            {
                AuditEventDateTimeUtc = DateTime.UtcNow,
                EventType = eventType,
                Details = details,
                RelatedUserId = userId,
                RelatedServiceName = GetServiceName(),
                RelatedAuditEventId = relatedAuditId,
                Items = items
            };
            return _AuditEventDal.Insert(dbConnection, dbTransaction, auditEvent);
        }
        protected int CreateAuditEvent(IDbConnection dbConnection, IDbTransaction dbTransaction,
            string eventType, Guid userReference, string details, IEnumerable<AuditEventItemModel> items, int? relatedAuditId = null)
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
            return _AuditEventDal.Insert(dbConnection, dbTransaction, auditEvent);
        }

        protected int CreateEmailAuditEvent(EmailMessageModel emailModel, int? userId, int? relatedAuditEventId = null)
        {
            IEnumerable<AuditEventItemModel> auditItems = new AuditEventItemModel[]
            {
                new AuditEventItemModel { Key = "SendTo", Value = emailModel.To.ToCsvString() },
                new AuditEventItemModel { Key = "Subject", Value = emailModel.Subject },
                new AuditEventItemModel { Key = "MessageBody", Value = emailModel.Body },
            };
            return CreateAuditEvent(AuditEventType.EmailSent, userId, "Subject: " + emailModel.Subject, auditItems, relatedAuditEventId);
        }
        protected int CreateEmailAuditEvent(EmailMessageModel emailModel, Guid userReference, int? relatedAuditEventId = null)
        {
            IEnumerable<AuditEventItemModel> auditItems = new AuditEventItemModel[]
            {
                new AuditEventItemModel { Key = "SendTo", Value = emailModel.To.ToCsvString() },
                new AuditEventItemModel { Key = "Subject", Value = emailModel.Subject },
                new AuditEventItemModel { Key = "MessageBody", Value = emailModel.Body },
            };
            return CreateAuditEvent(AuditEventType.EmailSent, userReference, "Subject: " + emailModel.Subject, auditItems, relatedAuditEventId);
        }

        #endregion ... Audit Helpers

        #region ServiceException Methods ... 

        protected ServiceException CreateServiceException(Exception exception, string methodName, object requestObject, string message = "")
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = $"A '{exception.GetType().Name}' exception occured in '{GetServiceName()}.{methodName}'.";
            }
            _Logger.Log(LogLevel.Error, message, exception);
            return new ServiceException(message, exception, methodName, requestObject);
        }

        #endregion ... ServiceException Methods
    }
}
