using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography;
using System.Text;

namespace PatientService.Infrastructure.Services;

/// <summary>
/// Provides AES-256 encryption using envelope encryption with Azure Key Vault
/// </summary>
public interface IEncryptionService
{
    Task<string> EncryptAsync(string plainText, string keyId = "default");
    Task<string> DecryptAsync(string cipherText, string keyId = "default");
    string ComputeHash(string input);
}

public class EncryptionService : IEncryptionService
{
    private readonly SecretClient _keyVaultClient;
    private readonly Dictionary<string, byte[]> _keyCache = new();
    private readonly SemaphoreSlim _cacheLock = new(1, 1);

    public EncryptionService(string keyVaultUri)
    {
        _keyVaultClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
    }

    public async Task<string> EncryptAsync(string plainText, string keyId = "default")
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        var key = await GetEncryptionKeyAsync(keyId);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Combine IV and ciphertext
        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public async Task<string> DecryptAsync(string cipherText, string keyId = "default")
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        var key = await GetEncryptionKeyAsync(keyId);
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Extract IV
        var iv = new byte[aes.IV.Length];
        var cipher = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }

    public string ComputeHash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private async Task<byte[]> GetEncryptionKeyAsync(string keyId)
    {
        await _cacheLock.WaitAsync();
        try
        {
            if (_keyCache.TryGetValue(keyId, out var cachedKey))
                return cachedKey;

            var secretName = $"PatientDataEncryptionKey-{keyId}";
            var secret = await _keyVaultClient.GetSecretAsync(secretName);
            var key = Convert.FromBase64String(secret.Value.Value);

            if (key.Length != 32) // AES-256 requires 32 bytes
                throw new InvalidOperationException($"Encryption key must be 32 bytes for AES-256. Key: {keyId}");

            _keyCache[keyId] = key;
            return key;
        }
        finally
        {
            _cacheLock.Release();
        }
    }
}

