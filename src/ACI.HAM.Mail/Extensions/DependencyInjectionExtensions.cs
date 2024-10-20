using ACI.HAM.Mail.Helpers;
using ACI.HAM.Mail.Services;
using ACI.HAM.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace ACI.HAM.Mail.Extensions
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
