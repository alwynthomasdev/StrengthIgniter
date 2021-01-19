using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.EmailTemplates.Models
{
    public class UserAccountNotFoundTemplateModel
        : EmailTemplateModelBase
    {
        internal override string TemplateName { get { return "UserAccountNotFound"; } }
        public string Title { get; set; }
    }
}
