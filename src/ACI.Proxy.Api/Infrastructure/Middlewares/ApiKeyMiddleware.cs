using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using ACI.Proxy.Core.Data;
using ACI.Proxy.Core.Extensions;
using ACI.Proxy.Core.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace ACI.Proxy.Core.Infrastructure.Middlewares
{
    public class ApiKeyMiddleware
    {
        public const string X_API_KEY = "x-api-key";

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
            if (context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader) &&
    authorizationHeader.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            if (!context.Request.Headers.TryGetValue(X_API_KEY, out var apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(_messages["API Key not provided"].Value);
                return;
            }
            if (!context.Request.Headers.TryGetValue("email", out var email))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(_messages["User id not provided"].Value);
                return;
            }
            using (var scope = _serviceProvider.CreateScope())
            {
                var baseContext = scope.ServiceProvider.GetRequiredService<BaseContext>();
                baseContext.IsApiKeyRequest = true;
                var userApiKeys = await baseContext.UserApiKeys
                    .Include(x => x.User)
                    .ThenInclude(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .Where(x => (x.User.Email == email.ToString()) && x.IsActive && (x.Expiration > DateTimeOffset.Now))
                    .ToListAsync();
                if (userApiKeys == null || !userApiKeys.Any())
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(_messages["Invalid API Key"].Value);
                    return;
                }
                bool isValidApiKey = userApiKeys.Any(userApiKey => ApiKeyExtension.ValidateApiKey(apiKey.ToString(), userApiKey.HashedApiKey, userApiKey.Salt));
                if (!isValidApiKey)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(_messages["Invalid API Key"].Value);
                    return;
                }
                var userApiKey = userApiKeys.First();
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userApiKey.User.Email),
                    new Claim(ClaimTypes.NameIdentifier, userApiKey.UserId.ToString())
                };
                var roles = userApiKey.User.UserRoles.ToList();
                if (roles != null)
                {
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
                    }
                }
                var identity = new ClaimsIdentity(claims, "ApiKey");
                var principal = new ClaimsPrincipal(identity);
                context.User = principal;
                var userApiUsageStatistic = new UserApiUsageStatistic
                {
                    UserId = userApiKey.User.Id,
                    ApiRoute = path,
                    RequestTime = DateTimeOffset.Now
                };
                baseContext.UserApiUsageStatistics.Add(userApiUsageStatistic);
                await baseContext.SaveChangesAsync();
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
