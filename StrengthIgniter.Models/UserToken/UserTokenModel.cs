using FluentValidation;
using StrengthIgniter.Models.Common;
using System;

namespace StrengthIgniter.Models.UserToken
{
    public class UserTokenModel : ModelBase
    {
        public int UserTokenId { get; set; }
        public Guid Reference { get; set; }
        public Guid UserReference { get; set; }
        public string PurposeCode { get; set; }
        public DateTime IssuedDateTimeUtc { get; set; }
        public DateTime ExpiryDateTimeUtc { get; set; }

        #region Methods

        public override ModelValidationResult Validate()
        {
            return new ModelValidationResult(new UserTokenModelValidator()
                .Validate(this));
        }

        public bool HasExpired()
        {
            return this.ExpiryDateTimeUtc < DateTime.UtcNow;
        }

        #endregion

        
    }
}
