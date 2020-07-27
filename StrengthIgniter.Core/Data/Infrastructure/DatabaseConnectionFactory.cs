﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StrengthIgniter.Core.Data.Infrastructure
{
    public sealed class DatabaseConnectionFactory
    {
        public readonly Func<IDbConnection> GetConnection;
        public DatabaseConnectionFactory(Func<IDbConnection> fnGetConnection)
        {
            GetConnection = fnGetConnection;
        }
    }
}
