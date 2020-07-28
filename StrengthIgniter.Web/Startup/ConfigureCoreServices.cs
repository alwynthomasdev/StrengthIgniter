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
            services.AddDatabaseConnectionFactory(configuration["DatabaseConnectionString"]);
        }
        public static void AddDatabaseConnectionFactory(this IServiceCollection services, string connectionString)
        {
            services.TryAddSingleton<DatabaseConnectionFactory>(new DatabaseConnectionFactory(() => new SqlConnection(connectionString)));
        }

        public static void AddDataAccess(this IServiceCollection services)
        {
            services.TryAddTransient<IAuditEventDataAccess, AuditEventDataAccess>();
            services.TryAddTransient<IUserDataAccess, UserDataAccess>();
            services.TryAddTransient<ISecurityQuestionDataAccess, SecurityQuestionDataAccess>();
        }

        public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataAccess();

            //get configuration
            LoginServiceConfig loginServiceConfig = GetLoginServiceConfig(configuration);
            RegistrationServiceConfig registrationServiceConfig = GetRegistrationServiceConfig(configuration);
            PasswordResetServiceConfig passwordResetServiceConfig = GetPasswordResetServiceConfig(configuration);
            UserSecurityQuestionResetServiceConfig userSecurityQuestionResetConfig = GetUserSecurityQuestionResetServiceConfig(configuration);

            services.AddLoginService(loginServiceConfig);
            services.AddRegistrationService(registrationServiceConfig);
            services.AddPasswordResetService(passwordResetServiceConfig);
            services.AddUserSecurityQuestionResetService(userSecurityQuestionResetConfig);
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

        #region Private Methods

        private static LoginServiceConfig GetLoginServiceConfig(IConfiguration configuration)
        {
            LoginServiceConfig config = new LoginServiceConfig();
            configuration.Bind("CoreServiceConfiguration:LoginServiceConfig", config);
            return config;
        }

        private static RegistrationServiceConfig GetRegistrationServiceConfig(IConfiguration configuration)
        {
            RegistrationServiceConfig config = new RegistrationServiceConfig();
            configuration.Bind("CoreServiceConfiguration:RegistrationServiceConfig", config);
            return config;
        }

        private static PasswordResetServiceConfig GetPasswordResetServiceConfig(IConfiguration configuration)
        {
            PasswordResetServiceConfig config = new PasswordResetServiceConfig();
            configuration.Bind("CoreServiceConfiguration:PasswordResetServiceConfig", config);
            return config;
        }

        private static UserSecurityQuestionResetServiceConfig GetUserSecurityQuestionResetServiceConfig(IConfiguration configuration)
        {
            UserSecurityQuestionResetServiceConfig config = new UserSecurityQuestionResetServiceConfig();
            configuration.Bind("CoreServiceConfiguration:UserSecurityQuestionResetServiceConfig", config);
            return config;
        }

        #endregion

    }
}
