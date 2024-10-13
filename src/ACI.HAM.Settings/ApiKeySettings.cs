namespace ACI.HAM.Settings
{
    public class ApiKeySettings
    {
        public string CertificateFile { get; set; } = "/app/certificates/certificate.pfx";

        public string CertificatePassword { get; set; } = "x9w\\(6<EXe1P";

        public int DaysBeforeExpirationWarning { get; set; } = 7;

        public string EncryptionKey { get; set; } = string.Empty;

        public string HMACKey { get; set; } = string.Empty;

        public string PersistKeysDirectory { get; set; } = "/app/certificates";
    }
}
