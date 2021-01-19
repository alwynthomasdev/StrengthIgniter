using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Dal.Common
{
    public class DataAccessException : Exception
    {
        //TODO: generate sql statment using parameters and sql???

        private const string DF_MSG = "An error occurred when trying to access the database.";

        public string Statement
        {
            get
            {
                return Data["Statement"].ToString();
            }
        }
        public object Parameters
        {
            get
            {
                return Data["Parameters"];
            }
        }

        internal DataAccessException(string sqlStatement, object parameters = null)
            : this(DF_MSG, sqlStatement, parameters)
        {
        }
        internal DataAccessException(string message, string sqlStatement, object parameters = null)
            : base(message)
        {
            Data["Statement"] = sqlStatement;
            Data["Parameters"] = parameters;
        }

        internal DataAccessException(Exception exception, string sqlStatement, object parameters = null)
            : this(DF_MSG, exception, sqlStatement, parameters)
        {
        }
        internal DataAccessException(string message, Exception exception, string sqlStatement, object parameters = null)
            : base(message, exception)
        {
            Data["Statement"] = sqlStatement;
            Data["Parameters"] = parameters;
        }
    }
}
