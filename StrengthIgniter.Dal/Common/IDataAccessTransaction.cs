using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Dal.Common
{
    public interface IDataAccessTransaction : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
        void Commit();
        void Rollback();
    }
}
