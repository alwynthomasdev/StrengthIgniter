using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Core.Data;
using StrengthIgniter.Core.Data.Infrastructure;
using StrengthIgniter.Core.Services;
using StrengthIgniter.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StrengthIgniter.Web
{
    public static class ConfigureCoreServicesExtentions
    {
        public static void AddDatabaseConnectionFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseConnectionFactory("");//TODO: get connection string from configuration
        }
        public static void AddDatabaseConnectionFactory(this IServiceCollection services, string connectionString)
        {
            services.TryAddSingleton<DatabaseConnectionFactory>(new DatabaseConnectionFactory(() => new SqlConnection(connectionString)));
        }

        public static void AddDataAccessServices(this IServiceCollection services)
        {
            services.TryAddTransient<IAuditEventDataAccess, AuditEventDataAccess>();
            services.TryAddTransient<IUserDataAccess, UserDataAccess>();
            services.TryAddTransient<ISecurityQuestionDataAccess, SecurityQuestionDataAccess>();
        }

        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLoginService(null);//TODO: get configuration
            services.AddRegistrationService(null);//TODO: get configuration
            services.AddPasswordResetService(null);//TODO: get configuration
            services.AddUserSecurityQuestionResetService(null);//TODO: get configuration
        }

        #region Individual services

        public static void AddLoginService(this IServiceCollection services, LoginServiceConfig configuration)
        {
            services.TryAddTransient<ILoginService>(sp => new LoginService(
                configuration,
                sp.GetRequiredService<IUserDataAccess>(),
                sp.GetRequiredService<IHashUtility>(),
                sp.GetRequiredService<IEmailUtility>(),
                sp.GetRequiredService<ITemplateUtility>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        public static void AddRegistrationService(this IServiceCollection services, RegistrationServiceConfig configuration)
        {
            services.TryAddTransient<IRegistrationService>(sp => new RegistrationService(
                configuration,
                sp.GetRequiredService<IUserDataAccess>(),
                sp.GetRequiredService<ISecurityQuestionDataAccess>(),
                sp.GetRequiredService<IHashUtility>(),
                sp.GetRequiredService<IEmailUtility>(),
                sp.GetRequiredService<ITemplateUtility>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        public static void AddPasswordResetService(this IServiceCollection services, PasswordResetServiceConfig configuration)
        {
            services.TryAddTransient<IPasswordResetService>(sp => new PasswordResetService(
                configuration,
                sp.GetRequiredService<IUserDataAccess>(),
                sp.GetRequiredService<IHashUtility>(),
                sp.GetRequiredService<IEmailUtility>(),
                sp.GetRequiredService<ITemplateUtility>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        public static void AddUserSecurityQuestionResetService(this IServiceCollection services, UserSecurityQuestionResetServiceConfig configuration)
        {
            services.TryAddTransient<IUserSecurityQuestionResetService>(sp => new UserSecurityQuestionResetService(
                configuration,
                sp.GetRequiredService<IUserDataAccess>(),
                sp.GetRequiredService<ISecurityQuestionDataAccess>(),
                sp.GetRequiredService<IHashUtility>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        #endregion

    }
}
