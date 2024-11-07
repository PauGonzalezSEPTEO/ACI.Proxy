using Microsoft.AspNetCore.Identity;

namespace ACI.Proxy.Core.Dtos
{
    public class VerifyEmailAndSetPasswordResultDto
    {
        public IEnumerable<IdentityError> Errors { get; set; }

        public bool IsSucceded { get; set; }
    }
}
