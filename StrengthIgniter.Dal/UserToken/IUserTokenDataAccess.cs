using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.UserToken;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.UserToken
{
    public interface IUserTokenDataAccess
    {
        IEnumerable<UserTokenModel> Select(Guid userReference);
        UserTokenModel Select(int userTokenId, Guid userReference);
        UserTokenModel SelectByReference(Guid reference);
        void Insert(UserTokenModel token, IDbTransaction dbTransaction = null);
        void Delete(Guid tokenReference, IDbTransaction dbTransaction = null);
    }
}
