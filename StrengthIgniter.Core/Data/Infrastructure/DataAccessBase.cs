using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Core.Data.Infrastructure
{
    public abstract class DataAccessBase
    {
        protected const string SYSTEM_USER_REFERENCE = "00000000-0000-0000-0000-000000000000";

        #region CTOR
        protected readonly Func<IDbConnection> GetConnection;
        public DataAccessBase(Func<IDbConnection> fnGetConnection)
        {
            GetConnection = fnGetConnection;
        }
        #endregion
    }
}
