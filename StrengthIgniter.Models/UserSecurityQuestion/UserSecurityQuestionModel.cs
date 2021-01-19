using FluentValidation;
using StrengthIgniter.Models.Common;
using System;

namespace StrengthIgniter.Models.UserSecurityQuestion
{
    public class UserSecurityQuestionModel : ModelBase
    {
        public int UserSecurityQuestionId { get; set; }
        public Guid Reference { get; set; }
        public Guid UserReference { get; set; }
        public string QuestionText { get; set; }
        public string AnswerHash { get; set; }
        public int? FailedAnswerAttemptCount { get; set; }

        #region Methods

        public override ModelValidationResult Validate()
        {
            return new ModelValidationResult(new UserSecurityQuestionModelValidator()
                .Validate(this));
        }

        #endregion

       
    }
}
