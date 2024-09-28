using System.Linq;
using System.Text.RegularExpressions;
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
        private const string API_KEY_NAME = "x-api-key";
        private readonly IDataProtector _dataProtector;
        private readonly List<string> _excludedRoutes = new List<string>
        {
            "/authentication/forgot-password",
            "/authentication/login",
            "/authentication/refresh-token",
            "/authentication/register",
            "/authentication/resend-verify-email",
            "/authentication/reset-password",
            "/authentication/two-factor",
            "/authentication/verify-email",
            "/authentication/verify-email-and-set-password",
            "/authentication/verify-new-email"
        };
        private readonly IStringLocalizer<ApiKeyMiddleware> _messages;
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public ApiKeyMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IDataProtectionProvider dataProtectionProvider, IStringLocalizer<ApiKeyMiddleware> messages)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _dataProtector = dataProtectionProvider.CreateProtector("ApiKeyProtector");
            _messages = messages ?? throw new ArgumentNullException(nameof(messages));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (_excludedRoutes.Any(route => IsExcludedRoute(path, route)))
            {
                await _next(context);
                return;
            }
            if (context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }
            if (!context.Request.Headers.TryGetValue(API_KEY_NAME, out var apiKey))
            {
                context.Response.StatusCode = 401;
                //ToDo: Add localization
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
            await _next(context);
        }

        private bool IsExcludedRoute(string path, string route)
        {
            var regex = new Regex($"/api/v\\d+{route}", RegexOptions.IgnoreCase);
            return regex.IsMatch(path);
        }
    }
}
