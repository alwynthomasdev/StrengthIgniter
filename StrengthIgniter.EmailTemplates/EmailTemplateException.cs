using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.EmailTemplates
{
    public class EmailTemplateException : Exception
    {
        public EmailTemplateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
