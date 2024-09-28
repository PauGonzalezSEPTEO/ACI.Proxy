using ACI.HAM.Core.Data;
using ACI.HAM.Core.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace ACI.HAM.Core.Infrastructure.Middlewares
{
    public class ApiKeyMiddleware
    {
        private const string APIKEYNAME = "X-API-KEY";
        private readonly IDataProtector _dataProtector;
        private readonly IStringLocalizer<ApiKeyMiddleware> _messages;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public ApiKeyMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IDataProtectionProvider dataProtectionProvider, IStringLocalizer<ApiKeyMiddleware> messages)
        {
            _next = next;
            _dataProtector = dataProtectionProvider.CreateProtector("ApiKeyProtector");
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(_messages["API Key not provided"].Value);
                return;
            }
            using (var scope = _serviceProvider.CreateScope())
            {
                var baseContext = scope.ServiceProvider.GetRequiredService<BaseContext>();
                var user = await baseContext.Users.FirstOrDefaultAsync(x => ApiKeyExtension.ValidateApiKey(apiKey, x.HashedApiKey, x.Salt));
                if (user == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(_messages["Invalid API Key"].Value);
                    return;
                }
            }

            //ToDo

            await _next(context);
        }
    }
}
