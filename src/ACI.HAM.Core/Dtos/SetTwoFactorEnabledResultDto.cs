using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class SetTwoFactorEnabledResultDto
    {
        public IEnumerable<IdentityError> Errors { get; set; }

        public bool IsSucceded { get; set; }
    }
}
