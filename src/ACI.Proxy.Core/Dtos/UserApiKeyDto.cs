namespace ACI.Proxy.Core.Dtos
{
    public class UserApiKeyDto
    {
        public string ApiKeyLast6 { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset Expiration { get; set; }

        public string HashedApiKey { get; set; }

        public int Id { get; set; }

        public bool IsActive { get; set; }

        public bool IsValid => IsActive && (Expiration > DateTimeOffset.Now);

        public string Salt { get; set; }
    }
}
