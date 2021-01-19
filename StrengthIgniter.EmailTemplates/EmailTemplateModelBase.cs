using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.EmailTemplates
{
    public abstract class EmailTemplateModelBase
    {
        internal abstract string TemplateName { get; }
    }
}
