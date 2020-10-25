using StrengthIgniter.Core.Models.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class AuditEventModel : ModelBase
    {
        public int AuditEventId { get; internal set; }
        public DateTime AuditEventDateTimeUtc { get; internal set; }
        public string EventType { get; set; }
        public string Details { get; set; }

        public string RelatedServiceName { get; set; }
        public int? RelatedUserId { get; set; }
        public Guid? RelatedUserReference { get; set; }
        public int? RelatedAuditEventId { get; set; }

        public IEnumerable<AuditEventItemModel> Items { get; set; }
    }

    public class AuditEventItemModel : ModelBase
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

        public const string SecurityQuestionsUpdated = "SecurityQuestionsUpdated";

        public const string ExerciseInsert = "ExerciseInsert";
        public const string ExerciseUpdate = "ExerciseUpdate";
        public const string ExerciseDelete = "ExerciseDelete";

        public const string RecordUpdate = "RecordUpdate";
        public const string RecordInsert = "RecordInsert";
        public const string RecordDelete = "RecordDelete";

        public const string ImportNew = "ImportNew";
        public const string ImportProcessed = "ImportProcessed";
        public const string ImportRowProcessed = "ImportRowProcessed";
        public const string ImportRowUpdated = "ImportRowUpdated";
        public const string ImportDeleted = "ImportDeleted";
        public const string ImportRowDeleted = "ImportRowDeleted";
    }

}
