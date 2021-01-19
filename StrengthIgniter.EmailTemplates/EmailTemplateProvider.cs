using HandlebarsDotNet;
using System;
using System.IO;
using System.Reflection;

namespace StrengthIgniter.EmailTemplates
{
    public interface IEmailTemplateProvider
    {
        string Generate<TModel>(TModel model) where TModel : EmailTemplateModelBase;
    }

    public class EmailTemplateProvider : IEmailTemplateProvider
    {
        public string Generate<TModel>(TModel model) where TModel : EmailTemplateModelBase
        {
            try
            {
                string template = GetEmailTemplate(model.TemplateName);
                string emailBody = ParseTemplate(template, model);
                return emailBody;
            }
            catch(Exception ex)
            {
                throw new EmailTemplateException(string.Format("Unable to get generate email with template '{0}'", model.TemplateName), ex);
            }
        }

        #region Private Helpers

        private string GetEmailTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var template = string.Format("StrengthIgniter.EmailTemplates.Templates.{0}.html", templateName);

            using (Stream stream = assembly.GetManifestResourceStream(template))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string ParseTemplate<TModel>(string template, TModel model)
            where TModel : EmailTemplateModelBase
        {
            var handlebarsTemplate = Handlebars.Compile(template);
            string output = handlebarsTemplate(model);
            return output;
        }

        #endregion

    }





}
