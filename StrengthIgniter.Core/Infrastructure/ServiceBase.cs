using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Core.Infrastructure
{
    public abstract class ServiceBase
    {
        #region CTOR

        public ServiceBase(
        )
        {
        }
        #endregion

        private string GetServiceName()
        {
            return this.GetType().Name;
        }

        #region Logger Helpers ...

        protected void LogInfo(string message)
        {
            throw new NotImplementedException();
        }

        protected void LogWarning(string message)
        {
            throw new NotImplementedException();
        }

        #endregion ... Logger Helpers

        #region Audit Helpers ...

        protected int CreateAuditEvent(string eventName, int? userId, string details, IEnumerable<object> items, int? relatedAuditId = null)
        {
            throw new NotImplementedException();
        }

        protected int CreateEmailAuditEvent(object emailModel, int? userId, int? relatedAuditEventId = null)
        {
            throw new NotImplementedException();
        }

        #endregion ... Audit Helpers

        #region ServiceException Methods ... 

        protected ServiceException CreateServiceException(Exception exception, string methodName, string requestObject, string message = "")
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = $"A '{exception.GetType().Name}' exception occured in '{GetServiceName()}.{methodName}'.";
            }
            //TODO: write to log

            return new ServiceException(message, exception, methodName, requestObject);
        }

        #endregion ... ServiceException Methods
    }
}
