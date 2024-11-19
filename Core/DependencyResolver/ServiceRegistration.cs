using Core.Utilities.Security.Abstract;
using Core.Utilities.Security.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DependencyResolver
{
    public static class ServiceRegistration
    {
        public static void AddCoreService(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddScoped<ITokenService, TokenManager>();
            
            // FluentEmail
            #region FluentEmail
            var emailSettings = configuration.GetSection("EmailSettings");
            var defaultFromEmail = emailSettings["DefaultFromEmail"];
            var host = emailSettings["Host"];
            var port = emailSettings.GetValue<int>("Port");
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];
            services.AddFluentEmail(defaultFromEmail)
                .AddSmtpSender(host, port, username, password);
            #endregion
        }
    }
}
