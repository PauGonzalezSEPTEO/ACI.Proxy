using Microsoft.AspNetCore.Identity;

namespace ACI.Base.Core.Dtos
{
    public class DeactivateResultDto
    {
        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
