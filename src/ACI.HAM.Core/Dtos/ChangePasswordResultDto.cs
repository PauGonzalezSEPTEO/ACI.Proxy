using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class ChangePasswordResultDto
    {
        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
