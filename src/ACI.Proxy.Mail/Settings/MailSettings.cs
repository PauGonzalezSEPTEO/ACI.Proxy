namespace ACI.Proxy.Mail.Settings
{
    public class MailSettings
    {
        public string? DisplayName { get; set; }

        public string? From { get; set; }

        public string? Host { get; set; }

        public string? Password { get; set; }

        public int Port { get; set; }

        public string? UserName { get; set; }

        public bool UseSsl { get; set; }

        public bool UseStartTls { get; set; }
    }
}
