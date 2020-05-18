using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Core.Data.Infrastructure
{
    public abstract class DataAccessBase
    {
        #region CTOR
        protected readonly Func<IDbConnection> GetConnection;
        public DataAccessBase(Func<IDbConnection> fnGetConnection)
        {
            GetConnection = fnGetConnection;
        }
        #endregion
    }
}
