using Microsoft.AspNetCore.Identity;

namespace ACI.HAM.Core.Dtos
{
    public class GenerateApiKeyResultDto
    {
        public string ApiKey { get; set; }

        public bool HasErrors { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
