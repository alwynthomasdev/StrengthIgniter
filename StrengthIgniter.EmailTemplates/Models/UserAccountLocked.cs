using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.EmailTemplates.Models
{
    public class UserAccountLockedTemplateModel
        : EmailTemplateModelBase
    {
        internal override string TemplateName { get { return "UserAccountLocked"; } }

        public string Title { get; set; }
        public string Username { get; set; }
        public int LockoutMinuets { get; set; }

    }
}
