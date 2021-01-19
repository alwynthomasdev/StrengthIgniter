using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Dal.Common
{
    public class DataAccessTransactionProvider : IDataAccessTransactionProvider
    {
        private readonly DatabaseConnectionFactory _DatabaseConnectionFactory;
        public DataAccessTransactionProvider(DatabaseConnectionFactory databaseConnectionFactory)
        {
            _DatabaseConnectionFactory = databaseConnectionFactory;
        }

        public IDataAccessTransaction BeginTansaction()
        {
            return new DataAccessTransaction(_DatabaseConnectionFactory);
        }
    }
}
