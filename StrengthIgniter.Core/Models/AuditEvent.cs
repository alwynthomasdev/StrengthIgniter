using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class AuditEventModel
    {
        public int AuditEventId { get; internal set; }
        public DateTime AuditEventDateTimeUtc { get; internal set; }
        public string EventType { get; set; }
        public string Details { get; set; }

        public string RelatedServiceName { get; set; }
        public int? RelatedUserId { get; set; }
        public int? RelatedAuditEventId { get; set; }

        public IEnumerable<AuditEventItemModel> Items { get; set; }
    }

    public class AuditEventItemModel
    {
        public int AuditEventItemId { get; internal set; }
        public int AuditEventId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public static class AuditEventType
    {
        public const string EmailSent = "EmailSent";
        public const string TokenCreated = "TokenCreated";

        public const string LoginSuccess = "LoginSuccess";
        public const string AccountLocked = "AccountLocked";

        public const string PasswordResetSuccess = "PasswordResetSuccess";
        public const string PasswordResetAttemptFailed = "PasswordResetAttemptFailed";

        public const string NewUserRegistration = "NewUserRegistration";
        public const string ValidatedRegistration = "ValidatedRegistration";
    }

}
