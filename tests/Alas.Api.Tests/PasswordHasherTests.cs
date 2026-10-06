using Alas.Api.Features.Auth;
using FluentAssertions;

namespace Alas.Api.Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void HashPassword_ShouldReturnNonEmptyHash()
    {
        var hash = _hasher.HashPassword("TestPassword123!");
        hash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        var password = "TestPassword123!";
        var hash = _hasher.HashPassword(password);
        _hasher.VerifyPassword(password, hash).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        var hash = _hasher.HashPassword("TestPassword123!");
        _hasher.VerifyPassword("WrongPassword", hash).Should().BeFalse();
    }

    [Fact]
    public void HashPassword_WithDifferentWorkFactors_ShouldProduceDifferentHashes()
    {
        var password = "TestPassword123!";
        var hash1 = _hasher.HashPassword(password, 4);
        var hash2 = _hasher.HashPassword(password, 12);
        hash1.Should().NotBe(hash2);
    }
}