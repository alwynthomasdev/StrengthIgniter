using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.UserToken;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.UserToken
{
    public interface IUserTokenService
    {
        Guid Create(Guid userReference, string purposeCode);
        Guid Create(Guid userReference, string purposeCode, IDataAccessTransaction dataAccessTransaction);
        ValidateTokenResponse Validate(Guid tokenReference, string purposeCode);
        void DeleteToken(Guid tokenReference);
    }
}
