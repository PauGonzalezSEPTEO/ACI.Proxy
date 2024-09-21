using ACI.HAM.Mail.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ACI.HAM.Mail.Extensions
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
