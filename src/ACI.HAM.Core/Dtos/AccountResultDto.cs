using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class AccountResultDto
    {
        public AccountDto Account { get; set; }

        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
