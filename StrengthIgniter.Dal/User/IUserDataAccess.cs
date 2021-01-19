using StrengthIgniter.Models.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.User
{
    public interface IUserDataAccess
    {
        UserModel Select(Guid userReference);
        UserModel Select(string emailAddress);
        void Insert(UserModel user, IDbTransaction dbTransaction = null);
        void Update(UserModel user, IDbTransaction dbTransaction = null);
    }
}
