using System;
using FluentValidation;
using StrengthIgniter.Models.Common;

namespace StrengthIgniter.Models.User
{
    public class UserModel : ModelBase
    {
        public int UserId { get; set; }
        public Guid Reference { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public UserTypeEnum UserTypeCode { get; set; }

        public DateTime? LastLoginDateTimeUtc { get; set; }
        public DateTime? LockoutEndDateTimeUtc { get; set; }
        public int? FailedLoginAttemptCount { get; set; }
        public bool IsRegistrationValidated { get; set; }

        //
        public DateTime RegisteredDateTimeUtc { get; set; }

        #region Methods

        public bool IsLockedOut()
        {
            if (this.LockoutEndDateTimeUtc.HasValue)
                return DateTime.UtcNow < this.LockoutEndDateTimeUtc.Value;
            else return false;
        }

        public override ModelValidationResult Validate()
        {
            UserModelValidator validator = new UserModelValidator();
            return new ModelValidationResult(validator.Validate(this));
        }

        #endregion
    }

   

    

}
