using System.Security.Cryptography;


namespace AppSecPracticalAssignment_223981B.Models
{
    public class EncryptionSettings
    {
        private static readonly EncryptionSettings instance = new EncryptionSettings();

        public byte[] Key { get; }
        public byte[] IV { get; }

        private EncryptionSettings()
        {
            using (Aes aesAlg = Aes.Create())
            {
                Key = aesAlg.Key;
                IV = aesAlg.IV;
            }
        }

        public static EncryptionSettings Instance => instance;
    }
}
