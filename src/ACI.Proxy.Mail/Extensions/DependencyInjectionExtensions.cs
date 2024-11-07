using ACI.Proxy.Mail.Helpers;
using ACI.Proxy.Mail.Services;
using ACI.Proxy.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace ACI.Proxy.Mail.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMailComponents(this IServiceCollection services)
        {
            services.AddSingleton<UISettings>();
            services.AddSingleton<MailTemplateHelper>();
            services.AddSingleton<IMailService, MailService>();
            return services;
        }
    }
}
