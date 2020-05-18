using HandlebarsDotNet;
using System;
using System.IO;

namespace StrengthIgniter.Core.Utils
{
    public interface ITemplateUtility
    {
        string Parse(string templatePath, object model);
    }

    public class TemplateUtility : ITemplateUtility
    {
        public string Parse(string templatePath, object model)
        {
            string template = ReadTemplate(templatePath);
            Func<object, string> fn = Handlebars.Compile(template);
            string output = fn(model);
            return output;
        }

        private string ReadTemplate(string templatePath)
        {
            if (File.Exists(templatePath))
            {
                return File.ReadAllText(templatePath);
            }
            else throw new FileNotFoundException(string.Format("Could not find template at path '{0}'", templatePath));
        }

    }
}
