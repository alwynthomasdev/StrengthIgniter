using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Dal.Common
{
    public interface IDataAccessTransactionProvider
    {
        IDataAccessTransaction BeginTansaction();
    }
}
