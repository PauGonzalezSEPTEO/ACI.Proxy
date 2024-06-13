using Microsoft.AspNetCore.Identity;

namespace ACI.Base.Core.Dtos
{
    public class AccountResultDto
    {
        public AccountDto Account { get; set; }

        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
