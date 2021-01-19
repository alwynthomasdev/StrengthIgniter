using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace StrengthIgniter.Dal.Common
{
    public abstract class DataAccessBase
    {
        private const string SYSTEM_USER_REFERENCE = "00000000-0000-0000-0000-000000000000";

        protected readonly Func<IDbConnection> GetConnection;
        public DataAccessBase(DatabaseConnectionFactory databaseConnectionFactory)
        {
            GetConnection = databaseConnectionFactory.GetConnection;
        }
    }
}
