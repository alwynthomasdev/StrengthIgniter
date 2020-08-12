using Dapper;
using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using StrengthIgniter.Core.Data.Infrastructure;
using CodeFluff.Extensions.IEnumerable;

namespace StrengthIgniter.Core.Data
{
    public interface IAuditEventDataAccess
    {
        int InsertAuditEvent(AuditEventModel auditEvent);
        int InsertAuditEvent(IDbConnection connection, IDbTransaction transaction, AuditEventModel auditEvent);
    }

    public class AuditEventDataAccess : DataAccessBase, IAuditEventDataAccess
    {
        #region CTOR
        public AuditEventDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public int InsertAuditEvent(AuditEventModel auditEvent)
        {
            using (IDbConnection con = GetConnection())
            {
                con.Open();
                using (IDbTransaction trn = con.BeginTransaction())
                {
                    return InsertAuditEvent(con, trn, auditEvent);
                }
            }
        }

        public int InsertAuditEvent(IDbConnection connection, IDbTransaction transaction, AuditEventModel auditEvent)
        {
            #region SQL
            string sql = @"
INSERT INTO [AuditEvent]
    ([AuditEventDateTimeUtc]
    ,[EventType]
    ,[Details]
    ,[RelatedServiceName]
    ,[RelatedUserId]
    ,[RelatedAuditEventId])
VALUES
    (@AuditEventDateTimeUtc
    ,@EventType
    ,@Details
    ,@RelatedServiceName
    ,@RelatedUserId
    ,@RelatedAuditEventId);

SELECT SCOPE_IDENTITY()
".Trim();
            //add code to get real user id using reference
            if (auditEvent.RelatedUserReference.HasValue)
            {
                sql = @"
--DECALRE @RelatedUserId INTEGER
SELECT TOP 1 @RelatedUserId = [UserId] FROM [User] WHERE [Reference] = @RelatedUserReference

" + sql;
            }
            #endregion

            try
            {
                int? aid = connection.QueryFirstOrDefault<int>(sql, auditEvent, transaction);
                if (!aid.HasValue)
                {
                    throw new Exception("Failed to create audit event.");
                }
                if(auditEvent.Items.HasItems())
                {
                    InsertAuditEventItems(connection, transaction, aid.Value, auditEvent.Items);
                }
                return aid.Value;
            }
            catch (DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql, auditEvent);
            }
        }

        private void InsertAuditEventItems(IDbConnection connection, IDbTransaction transaction, int auditEventId, IEnumerable<AuditEventItemModel> items)
        {
            #region SQL
            string sql = @"
INSERT INTO [AuditEventItem]
    ([AuditEventId]
    ,[Key]
    ,[Value])
VALUES
    (@AuditEventId
    ,@Key
    ,@Value)
".Trim();
            #endregion

            AuditEventItemModel[] aryItems = items.ToArray();
            for (int i = 0; i < aryItems.Length; i++)
            {
                AuditEventItemModel item = aryItems[i];
                item.AuditEventId = auditEventId;
                try
                {
                    connection.Execute(sql, item, transaction);
                }
                catch (Exception ex)
                {
                    throw new DataAccessException(ex, sql, item);
                }
            }
        }

    }
}
