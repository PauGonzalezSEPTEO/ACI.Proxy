using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ACI.Proxy.Api.Services
{
    public class UserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }
    }
}
