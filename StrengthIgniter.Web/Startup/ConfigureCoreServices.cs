using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        public static void AddDatabaseConnectionFactory(this IServiceCollection services, string connectionString)
        {
            services.TryAddSingleton<DatabaseConnectionFactory>(new DatabaseConnectionFactory(() => new SqlConnection(connectionString)));
        }

        public static void AddDataAccess(this IServiceCollection services)
        {
            services.TryAddTransient<IAuditEventDataAccess, AuditEventDataAccess>();
            services.TryAddTransient<IUserDataAccess, UserDataAccess>();
            services.TryAddTransient<ISecurityQuestionDataAccess, SecurityQuestionDataAccess>();

            services.TryAddTransient<IExerciseDataAccess, ExerciseDataAccess>();

            services.TryAddTransient<IRecordImportSchemaDataAccess, RecordImportSchemaDataAccess>();
            services.TryAddTransient<IRecordImportDataAccess, RecordImportDataAccess>();
        }

        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddDataAccess();

            services.AddLoginService();
            services.AddRegistrationService();
            services.AddPasswordResetService();
            services.AddUserSecurityQuestionResetService();

            services.AddRecordImportSchemaService();
            services.AddRecordImportService();
        }

        #region Individual services

        public static void AddLoginService(this IServiceCollection services)
        {
            services.TryAddTransient<ILoginService>(sp => new LoginService(
                sp.GetRequiredService<IOptions<LoginServiceConfig>>().Value,
                sp.GetRequiredService<IUserDataAccess>(),
                sp.GetRequiredService<IHashUtility>(),
                sp.GetRequiredService<IEmailUtility>(),
                sp.GetRequiredService<ITemplateUtility>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        public static void AddRegistrationService(this IServiceCollection services)
        {
            services.TryAddTransient<IRegistrationService>(sp => new RegistrationService(
                sp.GetRequiredService<IOptions<RegistrationServiceConfig>>().Value,
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

        public static void AddPasswordResetService(this IServiceCollection services)
        {
            services.TryAddTransient<IPasswordResetService>(sp => new PasswordResetService(
                sp.GetRequiredService<IOptions<PasswordResetServiceConfig>>().Value,
                sp.GetRequiredService<IUserDataAccess>(),
                sp.GetRequiredService<IHashUtility>(),
                sp.GetRequiredService<IEmailUtility>(),
                sp.GetRequiredService<ITemplateUtility>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        public static void AddUserSecurityQuestionResetService(this IServiceCollection services)
        {
            services.TryAddTransient<IUserSecurityQuestionResetService>(sp => new UserSecurityQuestionResetService(
                sp.GetRequiredService<IOptions<UserSecurityQuestionResetServiceConfig>>().Value,
                sp.GetRequiredService<IUserDataAccess>(),
                sp.GetRequiredService<ISecurityQuestionDataAccess>(),
                sp.GetRequiredService<IHashUtility>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        public static void AddRecordImportSchemaService(this IServiceCollection services)
        {
            services.TryAddTransient<IRecordImportSchemaService>(sp => new RecordImportSchemaService(
                sp.GetRequiredService<IRecordImportSchemaDataAccess>(),
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<IAuditEventDataAccess>(),
                sp.GetRequiredService<DatabaseConnectionFactory>()
            ));
        }

        public static void AddRecordImportService(this IServiceCollection services)
        {
            services.TryAddTransient<IRecordImportService>(sp => new RecordImportService(
                sp.GetRequiredService<IRecordImportDataAccess>(),
                sp.GetRequiredService<IRecordImportSchemaDataAccess>(),
                sp.GetRequiredService<IExerciseDataAccess>(),
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
