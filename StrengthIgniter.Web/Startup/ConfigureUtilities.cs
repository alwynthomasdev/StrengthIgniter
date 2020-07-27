using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web
{
    public static class ConfigureUtilities
    {
        public static void AddEmailUtility(this IServiceCollection services, EmailConfiguration config)
        {
            services.TryAddSingleton<IEmailUtility>(sp => new EmailUtility(config, sp.GetRequiredService<ILoggerFactory>()));
        }

        public static void AddUtilities(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IHashUtility>(new HashUtility());
            services.TryAddSingleton<ITemplateUtility>(new TemplateUtility());
            services.AddEmailUtility(null); //TODO: get email configuration
        }
    }
}
