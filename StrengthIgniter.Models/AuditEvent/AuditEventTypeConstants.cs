using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Models.AuditEvent
{
    public static class AuditEventTypeConstants
    {
        public const string EmailSent = "EmailSent";

        public const string UserTokenCreated = "UserTokenCreated";

        public const string UserAuthenticationSuccess = "UserAuthenticationSuccess";
        public const string UserAuthenticationLocked = "UserAuthenticationLocked";

        public const string UserPasswordResetSuccess = "UserPasswordResetSuccess";
        public const string UserPasswordResetAttemptFailed = "UserPasswordResetAttemptFailed";

        public const string UserRegistration = "UserRegistration";
        public const string UserRegistrationValidated = "UserRegistrationValidated";

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
