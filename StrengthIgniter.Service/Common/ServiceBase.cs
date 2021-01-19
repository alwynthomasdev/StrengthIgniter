using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Common
{
    public abstract class ServiceBase
    {
        private readonly ILogger _Logger;

        public ServiceBase(ILogger logger)
        {
            _Logger = logger;
        }

        //

        protected string GetServiceName()
        {
            return this.GetType().Name;
        }

        //

        protected void LogInfo(string message)
        {
            _Logger.Log(LogLevel.Information, message);
        }

        protected void LogWarning(string message)
        {
            _Logger.Log(LogLevel.Warning, message);
        }

        protected void LogDebug(string message)
        {
            _Logger.Log(LogLevel.Debug, message);
        }

        //

        protected ServiceException CreateServiceException(Exception exception, string methodName, object parameters, string message = "")
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = $"A '{exception.GetType().Name}' exception occured in '{GetServiceName()}.{methodName}'.";
            }
            _Logger.Log(LogLevel.Error, message, exception);
            return new ServiceException(message, exception, methodName, parameters);
        }

        protected void ThrowServiceException(Exception exception, string methodName, object parameters, string message = "")
        {
            ServiceException serviceException = CreateServiceException(exception, methodName, parameters, message);
            throw serviceException;
        }

    }
}
