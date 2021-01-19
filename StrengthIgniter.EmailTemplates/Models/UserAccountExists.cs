using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.EmailTemplates.Models
{
    public class UserAccountExistsTemplateModel
        : EmailTemplateModelBase
    {
        internal override string TemplateName { get { return " UserAccountExists"; } }

        public string Title { get; set; }
        public string PasswordResetUrl { get; set; }
    }
}
