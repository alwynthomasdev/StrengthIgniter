using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Services
{
    public interface IUserSecurityQuestionResetService
    {
        IEnumerable<UserSecurityQuestionAnswerModel> GetFailedQuestionsForUser(Guid userReference);
        void UpdateSecretQuestions

        /*
         * TODO: 
         *  Get questions that need resetting for user
         *  Reset questions
         */
    }

    public class UserSecurityQuestionResetService
    {
    }
}
