using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Common.Utilities
{
    /// <summary>
    /// HIPAA-compliant AES-256 encryption helper for PHI data at rest
    /// Uses AES-256-GCM for authenticated encryption
    /// </summary>
    public static class EncryptionHelper
    {
        private const int KeySize = 256;
        private const int NonceSize = 12; // 96 bits for GCM
        private const int TagSize = 16; // 128 bits for GCM

        /// <summary>
        /// Encrypts data using AES-256-GCM
        /// </summary>
        public static string Encrypt(string plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
            
            if (key == null || key.Length != 32) // 256 bits
                throw new ArgumentException("Key must be 256 bits (32 bytes)", nameof(key));

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipherBytes = new byte[plainBytes.Length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonce);
            }

            using (var aes = new AesGcm(key))
            {
                aes.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }

            // Combine nonce + tag + ciphertext
            byte[] result = new byte[NonceSize + TagSize + cipherBytes.Length];
            Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
            Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Decrypts data using AES-256-GCM
        /// </summary>
        public static string Decrypt(string cipherText, byte[] key)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException(nameof(cipherText));
            
            if (key == null || key.Length != 32)
                throw new ArgumentException("Key must be 256 bits (32 bytes)", nameof(key));

            byte[] combined = Convert.FromBase64String(cipherText);
            
            if (combined.Length < NonceSize + TagSize)
                throw new ArgumentException("Invalid cipher text");

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] cipherBytes = new byte[combined.Length - NonceSize - TagSize];

            Buffer.BlockCopy(combined, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(combined, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(combined, NonceSize + TagSize, cipherBytes, 0, cipherBytes.Length);

            byte[] plainBytes = new byte[cipherBytes.Length];

            using (var aes = new AesGcm(key))
            {
                aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }

        /// <summary>
        /// Generates a cryptographically secure random encryption key
        /// </summary>
        public static byte[] GenerateKey()
        {
            byte[] key = new byte[32]; // 256 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }
            return key;
        }

        /// <summary>
        /// Hashes data using SHA-256 for indexing and searching
        /// </summary>
        public static string Hash(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
