using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class ChangeEmailResultDto
    {
        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
