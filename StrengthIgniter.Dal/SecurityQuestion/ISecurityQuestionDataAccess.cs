using StrengthIgniter.Models.SecurityQuestion;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Dal.SecurityQuestion
{
    public interface ISecurityQuestionDataAccess
    {
        IEnumerable<SecurityQuestionModel> Select();
    }
}
