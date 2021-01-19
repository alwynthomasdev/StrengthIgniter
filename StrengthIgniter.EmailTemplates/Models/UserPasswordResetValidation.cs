using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.EmailTemplates.Models
{
    public class UserPasswordResetValidationTemplateModel
        : EmailTemplateModelBase
    {
        internal override string TemplateName { get { return " UserPasswordResetValidation"; } }

        public string Title { get; set; }
        public string Username { get; set; }
        public string ValidatePasswordResetUrl { get; set; }
    }
}
