using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class ResendVerifyEmailResultDto
    {
        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
