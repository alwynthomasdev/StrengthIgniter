using StrengthIgniter.Models.Common;
using System;
using System.Collections.Generic;

namespace StrengthIgniter.Models.AuditEvent
{
    public class AuditEventModel : ModelBase
    {
        //public int AuditEventId { get; set; }
        public DateTime AuditEventDateTimeUtc { get; set; }
        public string EventType { get; set; }
        public string Details { get; set; }

        public string RelatedServiceName { get; set; }
        public Guid? RelatedUserReference { get; set; }
        public int? RelatedAuditEventId { get; set; }

        public IEnumerable<AuditEventItemModel> Items { get; set; }

        #region Methods



        #endregion

    }
}
