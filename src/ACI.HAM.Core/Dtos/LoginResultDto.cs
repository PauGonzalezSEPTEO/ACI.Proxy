using System.Text.Json.Serialization;

namespace ACI.HAM.Core.Dtos
{
    public class LoginResultDto
    {
        public int AccessFailedCount { get; set; }

        public string AccessToken { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public bool EmailNotConfirmed { get; set; }

        public bool IsLockedOut { get; set; }

        [JsonIgnore]
        public bool IsInvalidTwoFactorVerificationProvider { get; set; }

        public bool IsTwoFactorVerificationRequired { get; set; }

        [JsonIgnore]
        public bool IsUnauthorized { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public string TwoFactorProvider { get; set; }
    }
}
