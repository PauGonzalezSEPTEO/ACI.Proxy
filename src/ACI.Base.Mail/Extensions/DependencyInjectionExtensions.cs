using ACI.Base.Mail.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ACI.Base.Mail.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMailComponents(this IServiceCollection services)
        {
            services.AddSingleton<IMailService, MailService>();
            return services;
        }
    }
}
