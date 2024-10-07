namespace ACI.HAM.Core.Dtos
{
    public class UserApiKeyDto
    {
        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset Expiration { get; set; }

        public string HashedApiKey { get; set; }

        public int Id { get; set; }

        public bool IsActive { get; set; }

        public string Salt { get; set; }
    }
}
