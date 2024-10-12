using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class GenerateUserApiKeyResultDto
    {
        public string ApiKey { get; set; }

        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
