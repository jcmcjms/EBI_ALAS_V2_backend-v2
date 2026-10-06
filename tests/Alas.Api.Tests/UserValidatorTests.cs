using Alas.Api.Features.Users;
using FluentAssertions;

namespace Alas.Api.Tests;

public class UserValidatorTests
{
    private readonly CreateUserRequestValidator _createValidator = new();
    private readonly UpdateUserRequestValidator _updateValidator = new();

    [Fact]
    public async Task CreateUserRequest_ValidRequest_ShouldPass()
    {
        var request = new CreateUserRequest(
            "testuser",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var result = await _createValidator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUserRequest_EmptyUsername_ShouldFail()
    {
        var request = new CreateUserRequest(
            "",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var result = await _createValidator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Fact]
    public async Task CreateUserRequest_WeakPassword_ShouldFail()
    {
        var request = new CreateUserRequest(
            "testuser",
            "weak",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var result = await _createValidator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task CreateUserRequest_InvalidEmail_ShouldFail()
    {
        var request = new CreateUserRequest(
            "testuser",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff",
            Email: "invalid-email");

        var result = await _createValidator.ValidateAsync(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public async Task UpdateUserRequest_ValidRequest_ShouldPass()
    {
        var request = new UpdateUserRequest(
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var result = await _updateValidator.ValidateAsync(request);
        result.IsValid.Should().BeTrue();
    }
}