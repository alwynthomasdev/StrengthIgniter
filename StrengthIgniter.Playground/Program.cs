using StrengthIgniter.EmailTemplates;
using StrengthIgniter.EmailTemplates.Models;
using System;

namespace StrengthIgniter.Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            EmailTemplateProvider templateProvider = new EmailTemplateProvider();
            UserAccountLockedTemplateModel model = new UserAccountLockedTemplateModel
            {
                Subject = "Your account is locked",
                Username = "Alwyn Thomas",
                LockoutMinuets = 2
            };
            Console.WriteLine(templateProvider.Generate(model));
            Console.Read();
        }
    }
}
