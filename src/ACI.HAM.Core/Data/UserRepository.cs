using Microsoft.AspNetCore.Http;

namespace ACI.HAM.Api.Services
{
    public class UserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.Name;
        }
    }
}
