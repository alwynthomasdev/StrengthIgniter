using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Common
{
    public class ServiceException : Exception
    {
        public string MethodName
        {
            get
            {
                return Data["MethodName"].ToString();
            }
        }

        public object RequestData
        {
            get
            {
                return Data["RequestData"];
            }
        }

        public ServiceException(string message, Exception exception, string methodName, object parameters)
            : this(message, exception, methodName)
        {
            Data["RequestData"] = parameters;
        }

        public ServiceException(string message, Exception exception, string methodName)
            : base(message, exception)
        {
            Data["MethodName"] = methodName;
        }
    }
}
