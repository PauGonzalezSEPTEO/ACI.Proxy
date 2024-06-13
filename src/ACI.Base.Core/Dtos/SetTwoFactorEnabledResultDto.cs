using Microsoft.AspNetCore.Identity;

namespace ACI.Base.Core.Dtos
{
    public class SetTwoFactorEnabledResultDto
    {
        public IEnumerable<IdentityError> Errors { get; set; }

        public bool IsSucceded { get; set; }
    }
}
