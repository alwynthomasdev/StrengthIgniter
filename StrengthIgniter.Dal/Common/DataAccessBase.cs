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

        protected TResult ManageConnection<TResult>(Func<IDbConnection, IDbTransaction, TResult> function, IDbTransaction dbTransaction = null)
        {
            if(dbTransaction == null)
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    return function(dbConnection, null);
                }
            }
            else
            {
                return function(dbTransaction.Connection, dbTransaction);
            }
        }

        protected void ManageConnection(Action<IDbConnection, IDbTransaction> action, IDbTransaction dbTransaction = null)
        {
            if (dbTransaction == null)
            {
                using (IDbConnection dbConnection = GetConnection())
                {
                    action(dbConnection, null);
                }
            }
            else
            {
                action(dbTransaction.Connection, dbTransaction);
            }
        }


    }
}
