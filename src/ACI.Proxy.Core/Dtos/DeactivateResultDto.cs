using Microsoft.AspNetCore.Identity;

namespace ACI.Proxy.Core.Dtos
{
    public class DeactivateResultDto
    {
        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
