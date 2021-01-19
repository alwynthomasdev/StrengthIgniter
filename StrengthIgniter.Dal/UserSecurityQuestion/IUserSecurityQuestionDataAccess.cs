using StrengthIgniter.Models.UserSecurityQuestion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.UserSecurityQuestion
{
    public interface IUserSecurityQuestionDataAccess
    {
        IEnumerable<UserSecurityQuestionModel> Select(Guid userReference);
        int Insert(UserSecurityQuestionModel userSecurityQuestion, IDbTransaction dbTransaction = null);
        void Update(UserSecurityQuestionModel userSecurityQuestion, IDbTransaction dbTransaction = null);
        void Delete(int UserSecurityQuestionId, Guid userReference, IDbTransaction dbTransaction = null);
    }
}
