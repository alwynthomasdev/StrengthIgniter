using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.EmailTemplates.Models
{
    public class UserRegistrationValidationTemplateModel
        : EmailTemplateModelBase
    {
        internal override string TemplateName { get { return "ValidateRegistration"; } }
        public string Title { get; set; }
        public string Username { get; set; }
        public string ValidateRegistrationUrl { get; set; }
    }
}
