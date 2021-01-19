using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Email
{
    public class EmailServiceConfiguration
    {
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        //
        public string DefaultFromName { get; set; }
        public string DefaultFromAddress { get; set; }
    }
}
