using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ACI.HAM.Core.Extensions
{
    public static class ApiKeyExtension
    {
        public static string DecryptApiKey(string encryptedApiKey, string encryptionKey)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedApiKey);
            using (Aes aes = Aes.Create())
            {
                byte[] iv = new byte[aes.BlockSize / 8];
                byte[] cipherText = new byte[fullCipher.Length - iv.Length];
                Array.Copy(fullCipher, iv, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);
                aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

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

        public static (string HashedApiKey, string Salt) HashApiKey(string apiKey)
        {
            using (var hmac = new HMACSHA256())
            {
                var salt = GenerateSalt();
                var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey + salt);
                var hashedApiKey = hmac.ComputeHash(apiKeyBytes);
                return (Convert.ToBase64String(hashedApiKey), salt);
            }
        }

        public static bool ValidateApiKey(string apiKey, string storedHashedApiKey, string salt)
        {
            using (var hmac = new HMACSHA256())
            {
                var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey + salt);
                var hashedKey = hmac.ComputeHash(apiKeyBytes);
                var computedHashedApiKey = Convert.ToBase64String(hashedKey);
                return storedHashedApiKey == computedHashedApiKey;
            }
        }
    }
}
