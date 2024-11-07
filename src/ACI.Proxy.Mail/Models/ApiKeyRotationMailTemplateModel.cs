namespace ACI.Proxy.Mail.Models
{
    public class ApiKeyRotation
    {
        public DateTimeOffset? Expiration { get; set; }

        public string? FormattedExpiration => Expiration?.ToString("dd/MM/yyyy HH:mm:ss");

        public string? Url { get; set; }
    }
}
