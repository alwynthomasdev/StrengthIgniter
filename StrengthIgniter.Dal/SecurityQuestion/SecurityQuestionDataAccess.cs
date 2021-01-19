using Dapper;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.Models.SecurityQuestion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.SecurityQuestion
{
    public class SecurityQuestionDataAccess : DataAccessBase, ISecurityQuestionDataAccess
    {
        #region CTOR
        public SecurityQuestionDataAccess(DatabaseConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }
        #endregion

        public IEnumerable<SecurityQuestionModel> Select()
        {
            string sp = "dbo.spSecurityQuestionSelect";
            try
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return dbConnection.Query<SecurityQuestionModel>(sp, commandType: CommandType.StoredProcedure);
                }
            }
            catch(Exception ex)
            {
                throw new DataAccessException(ex, sp);
            }
        }

    }
}
