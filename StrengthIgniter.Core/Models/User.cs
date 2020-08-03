using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Models
{
    public class UserModel
    {
        internal int UserId { get; set; }
        public Guid Reference { get; internal set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; internal set; }
        public UserType UserTypeCode { get; set; }
        public DateTime? LastLoginDateTimeUtc { get; internal set; }
        public DateTime? LockoutEndDateTimeUtc { get; internal set; }
        public int? FailedLoginAttemptCount { get; internal set; }
        public bool IsRegistrationValidated { get; internal set; }
        public DateTime RegisteredDateTimeUtc { get; internal set; }

        internal IEnumerable<UserSecurityQuestionAnswerModel> SecurityQuestions { get; set; }
        internal IEnumerable<UserTokenModel> Tokens { get; set; }
    }

    public class UserSecurityQuestionAnswerModel
    {
        public Guid Reference { get; internal set; }
        public string QuestionText { get; set; }
        public string AnswerHash { get; internal set; }
        public int? FailedAnswerAttemptCount { get; internal set; }
    }

    public class UserTokenModel
    {
        public Guid TokenReference { get; internal set; }
        public string PurposeCode { get; internal set; }
        public DateTime IssuedDateTimeUtc { get; internal set; }
        public DateTime ExpiryDateTimeUtc { get; internal set; }
    }

    public enum UserType
    {
        [Description("Basic")]
        Basic = 0
    }

}
