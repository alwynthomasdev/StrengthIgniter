using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.AuditEvent;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.AuditEvent
{
    public interface IAuditEventDataAccess
    {
        int Insert(AuditEventModel auditEvent, IDbTransaction dbTransaction = null);
    }
}
