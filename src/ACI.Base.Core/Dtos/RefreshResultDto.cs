using System.Text.Json.Serialization;

namespace ACI.Base.Core.Dtos
{
    public class RefreshResultDto
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public bool IsInvalidToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
