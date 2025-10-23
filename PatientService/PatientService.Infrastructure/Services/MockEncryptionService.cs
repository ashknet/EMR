using System.Security.Cryptography;
using System.Text;

namespace PatientService.Infrastructure.Services;

/// <summary>
/// Mock encryption service for development - NOT for production use
/// Uses simple Base64 encoding instead of real encryption
/// </summary>
public class MockEncryptionService : IEncryptionService
{
    private readonly string _developmentKey = "DevelopmentKey123456789012345678901234"; // 32 bytes for AES-256

    public Task<string> EncryptAsync(string plainText, string keyId = "default")
    {
        if (string.IsNullOrEmpty(plainText))
            return Task.FromResult(plainText);

        // For development, just use Base64 encoding
        // WARNING: This is NOT secure and should never be used in production
        var bytes = Encoding.UTF8.GetBytes(plainText);
        return Task.FromResult(Convert.ToBase64String(bytes));
    }

    public Task<string> DecryptAsync(string cipherText, string keyId = "default")
    {
        if (string.IsNullOrEmpty(cipherText))
            return Task.FromResult(cipherText);

        try
        {
            // For development, just decode Base64
            // WARNING: This is NOT secure and should never be used in production
            var bytes = Convert.FromBase64String(cipherText);
            return Task.FromResult(Encoding.UTF8.GetString(bytes));
        }
        catch
        {
            // If it's not valid Base64, return as-is (might be plain text)
            return Task.FromResult(cipherText);
        }
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
}
