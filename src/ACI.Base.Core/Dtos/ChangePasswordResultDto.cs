using Microsoft.AspNetCore.Identity;

namespace ACI.Base.Core.Dtos
{
    public class ChangePasswordResultDto
    {
        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
