namespace ACI.HAM.Settings
{
    public class JwtSettings
    {
        public string SecurityKey { get; set; } = string.Empty;

        public string ValidAudience { get; set; } = "http://localhost:4200";

        public string ValidIssuer { get; set; } = "http://localhost:5000";

        public int TokenValidityInMinutes { get; set; } = 5;

        public int RefreshTokenValidityInDays { get; set; } = 7;
    }
}
