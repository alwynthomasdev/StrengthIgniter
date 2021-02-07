using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web
{
    public static class ConfigureUtilities
    {
        public static void AddContentRootPath(this IServiceCollection services, string path)
        {
            services.TryAddSingleton<ContentRootPathFactory>(new ContentRootPathFactory(() => path));
        }

        public static void AddUtilities(this IServiceCollection services)
        {
            services.TryAddSingleton<IEmailUtility>(sp => new EmailUtility(
                sp.GetRequiredService<IOptions<EmailConfiguration>>().Value, 
                sp.GetRequiredService<ILoggerFactory>()
            ));
            services.TryAddSingleton<IHashUtility>(new HashUtility());
            services.TryAddSingleton<ITemplateUtility>(sp => new TemplateUtility(sp.GetRequiredService<ContentRootPathFactory>().GetPath()));
            services.TryAddSingleton<IPaginationUtility>(new PaginationUtility());

        }
    }

    public class ContentRootPathFactory
    {
        public Func<string> GetPath;
        public ContentRootPathFactory(Func<string> fnGetPath)
        {
            GetPath = fnGetPath;
        }
    }

}
