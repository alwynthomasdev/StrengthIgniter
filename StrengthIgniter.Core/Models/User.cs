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
        internal string PasswordHash { get; set; }
        public UserType UserTypeCode { get; set; }
        public DateTime? LastLoginDateTimeUtc { get; internal set; }
        public DateTime? LockoutEndDateTimeUtc { get; internal set; }
        public int? FailedLoginAttemptCount { get; internal set; }
        public bool IsRegistrationValidated { get; internal set; }
        public DateTime RegisteredDateTimeUtc { get; internal set; }

        public IEnumerable<UserSecurityQuestionAnswerModel> SecurityQuestions { get; internal set; }
        internal IEnumerable<UserTokenModel> Tokens { get; set; }
    }

    public class UserSecurityQuestionAnswerModel
    {
        public Guid Reference { get; internal set; }
        public string QuestionText { get; set; }
        internal string AnswerHash { get; set; }
        public int? FailedAnswerAttemptCount { get; internal set; }
    }

    public class UserTokenModel
    {
        public Guid Reference { get; internal set; }
        public string PurposeCode { get; internal set; }
        public DateTime IssuedDateTimeUtc { get; internal set; }
        public DateTime ExpiryDateTimeUtc { get; internal set; }
    }

    public enum UserType
    {
        Basic = 1
    }

}
