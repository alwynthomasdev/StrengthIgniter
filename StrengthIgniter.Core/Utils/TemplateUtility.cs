using HandlebarsDotNet;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace StrengthIgniter.Core.Utils
{
    public interface ITemplateUtility
    {
        string Parse(string templatePath, object model);
    }

    public class TemplateUtility : ITemplateUtility
    {

        private readonly string _RootPath;
        public TemplateUtility(string rootPath)
        {
            _RootPath = rootPath;
        }

        public string Parse(string templatePath, object model)
        {
            string template = ReadTemplate(templatePath);
            Func<object, string> fn = Handlebars.Compile(template);
            string output = fn(model);
            return output;
        }

        private string ReadTemplate(string templatePath)
        {
            string fullPath = string.Concat(_RootPath, templatePath);

            if (File.Exists(fullPath))
            {
                return File.ReadAllText(fullPath);
            }
            else throw new FileNotFoundException(string.Format("Could not find template at path '{0}'", fullPath));
        }

    }
}
