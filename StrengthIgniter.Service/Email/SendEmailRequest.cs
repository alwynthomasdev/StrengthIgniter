using StrengthIgniter.EmailTemplates;
using StrengthIgniter.Service.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Email
{
    public class SendEmailRequest : RequestBase
    {
        public string ToAddress { get; set; }
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public EmailTemplateModelBase Template { get; set; }

        //
        public Guid? UserReference { get; set; }
        public int? RelatedAuditEventId { get; set; }

    }
}
