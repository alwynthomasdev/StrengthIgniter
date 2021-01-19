using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.Common
{

    public class DataAccessTransaction : IDataAccessTransaction
    {
        public DataAccessTransaction(DatabaseConnectionFactory databaseConnectionFactory)
        {
            DbConnection = databaseConnectionFactory.GetConnection();
            DbConnection.Open();
            DbTransaction = DbConnection.BeginTransaction();
        }

        public IDbConnection DbConnection { get; private set; }

        public IDbTransaction DbTransaction { get; private set; }

        public void Commit()
        {
            DbTransaction.Commit();
        }

        public void Rollback()
        {
            DbTransaction.Rollback();
        }

        public void Dispose()
        {
            DbTransaction.Dispose();
            DbConnection.Close();
            DbConnection.Dispose();

            DbTransaction = null;
            DbConnection = null;
        }
    }
}
