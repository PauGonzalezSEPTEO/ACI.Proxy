using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class RegistrationResultDto
    {
        public IEnumerable<IdentityError> Errors { get; set; }

        public bool HasErrors { get; set; }

        public string Id { get; set; }

        public AccountDto User { get; set; }
    }
}
