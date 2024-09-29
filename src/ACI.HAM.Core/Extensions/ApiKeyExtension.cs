using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

namespace ACI.HAM.Core.Extensions
{
    public static class ApiKeyExtension
    {
        private static string _hMACKey;

        public static string EncryptApiKey(string apiKey, string encryptionKey)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aes.GenerateIV();
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(apiKey);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string GenerateApiKey(int size = 32)
        {
            byte[] apiKey = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(apiKey);
            }
            return Convert.ToBase64String(apiKey);
        }

        public static string GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        public static (string hashedApiKey, string salt) HashApiKey(string apiKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hMACKey)))
            {
                var salt = GenerateSalt();
                var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey + salt);
                var hashedApiKey = hmac.ComputeHash(apiKeyBytes);
                return (Convert.ToBase64String(hashedApiKey), salt);
            }
        }

        public static void Initialize(IDataProtectionProvider provider, IConfiguration configuration)
        {
            var protector = provider.CreateProtector("ApiKey.ApiKeyProtector");
            var encryptedHmacKey = configuration["ApiKey:HMACKey"];
            _hMACKey = protector.Unprotect(encryptedHmacKey);
        }

        public static bool ValidateApiKey(string apiKey, string storedHashedApiKey, string salt)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_hMACKey)))
            {
                var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey + salt);
                var hashedKey = hmac.ComputeHash(apiKeyBytes);
                var computedHashedApiKey = Convert.ToBase64String(hashedKey);
                return storedHashedApiKey == computedHashedApiKey;
            }
        }
    }
}
