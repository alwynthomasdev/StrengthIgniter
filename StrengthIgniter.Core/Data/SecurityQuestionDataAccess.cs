using Dapper;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace StrengthIgniter.Core.Data
{
    public interface ISecurityQuestionDataAccess
    {
        IEnumerable<SecurityQuestionModel> Select();
    }

    public class SecurityQuestionDataAccess : DataAccessBase, ISecurityQuestionDataAccess
    {
        #region CTOR
        public SecurityQuestionDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory.GetConnection)
        {
        }
        #endregion

        public IEnumerable<SecurityQuestionModel> Select()
        {
            #region SQL
            string sql = @"
SELECT
    [SecurityQuestionId],
    [QuestionText]
FROM [SecurityQuestion]
".Trim();
            #endregion

            try
            {
                using (var con = GetConnection())
                {
                    return con.Query<SecurityQuestionModel>(sql);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex, sql);
            }
        }
    }
}
