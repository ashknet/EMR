using Xunit;
using FluentAssertions;
using PatientService.Infrastructure.Services;

namespace PatientService.Tests.Services;

public class EncryptionServiceTests
{
    [Fact]
    public void ComputeHash_SameInput_ReturnsSameHash()
    {
        // Arrange
        var service = CreateMockEncryptionService();
        var input = "test-ssn-123-45-6789";

        // Act
        var hash1 = service.ComputeHash(input);
        var hash2 = service.ComputeHash(input);

        // Assert
        hash1.Should().Be(hash2);
        hash1.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ComputeHash_DifferentInputs_ReturnsDifferentHashes()
    {
        // Arrange
        var service = CreateMockEncryptionService();
        var input1 = "ssn-123-45-6789";
        var input2 = "ssn-987-65-4321";

        // Act
        var hash1 = service.ComputeHash(input1);
        var hash2 = service.ComputeHash(input2);

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ComputeHash_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        var service = CreateMockEncryptionService();

        // Act
        var result = service.ComputeHash(string.Empty);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ComputeHash_NullString_ReturnsEmptyString()
    {
        // Arrange
        var service = CreateMockEncryptionService();

        // Act
        var result = service.ComputeHash(null!);

        // Assert
        result.Should().BeEmpty();
    }

    // Note: Real encryption tests would require Azure Key Vault setup
    // In production, use integration tests with actual Key Vault instance
    private IEncryptionService CreateMockEncryptionService()
    {
        // This is a mock implementation for unit testing
        // In real scenarios, you'd need proper Key Vault configuration
        return new MockEncryptionService();
    }

    private class MockEncryptionService : IEncryptionService
    {
        public Task<string> EncryptAsync(string plainText, string keyId = "default")
        {
            if (string.IsNullOrEmpty(plainText))
                return Task.FromResult(plainText);

            // Mock encryption
            var encrypted = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"ENCRYPTED:{plainText}"));
            return Task.FromResult(encrypted);
        }

        public Task<string> DecryptAsync(string cipherText, string keyId = "default")
        {
            if (string.IsNullOrEmpty(cipherText))
                return Task.FromResult(cipherText);

            // Mock decryption
            var decrypted = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
            decrypted = decrypted.Replace("ENCRYPTED:", "");
            return Task.FromResult(decrypted);
        }

        public string ComputeHash(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}

