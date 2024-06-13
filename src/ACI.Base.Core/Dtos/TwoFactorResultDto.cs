using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace ACI.Base.Core.Dtos
{
    public class TwoFactorResultDto
    {
        public string AccessToken { get; set; }

        public IEnumerable<IdentityError> Errors { get; set; }

        public bool HasErrors { get; set; }

        [JsonIgnore]
        public bool IsInvalidToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
