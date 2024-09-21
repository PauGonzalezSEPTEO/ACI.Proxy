namespace ACI.HAM.Settings
{
    public class JwtSettings
    {
        public string SecurityKey { get; set; }

        public string ValidAudience { get; set; }

        public string ValidIssuer { get; set; }

        public int TokenValidityInMinutes { get; set; }

        public int RefreshTokenValidityInDays { get; set; }
    }
}
