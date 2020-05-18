using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Services.Infrastructure
{
    public class ServiceException : Exception
    {
        public string MethodName
        {
            get
            {
                return Data["Method_Name"].ToString();
            }
        }
        public string RequestJson
        {
            get
            {
                return Data["RequestData"].ToString();
            }
        }

        public ServiceException(string message, Exception exception, string methodName, object requestData)
            : this(message, exception, methodName)
        {
            Data["RequestData"] = requestData;
        }

        public ServiceException(string message, Exception exception, string methodName)
            : base(message, exception)
        {
            Data["Method_Name"] = methodName;
        }

    }
}
