using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StrengthIgniter.Core.Services;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web
{
    public static class ConfigureConfigurationOptions
    {
        public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfiguration"));

            services.Configure<LoginServiceConfig>(configuration.GetSection("CoreServiceConfiguration:LoginServiceConfig"));
            services.Configure<RegistrationServiceConfig>(configuration.GetSection("CoreServiceConfiguration:RegistrationServiceConfig"));
            services.Configure<PasswordResetServiceConfig>(configuration.GetSection("CoreServiceConfiguration:PasswordResetServiceConfig"));
            services.Configure<UserSecurityQuestionResetServiceConfig>(configuration.GetSection("CoreServiceConfiguration:UserSecurityQuestionResetServiceConfig"));
        }
    }
}
