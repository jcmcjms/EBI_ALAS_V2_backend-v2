using Alas.Api.Features.Auth;
using Alas.Api.Features.Users;
using Alas.Api.Features.Users.Domain;
using Alas.Api.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Tests;

public class UserServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repository;
    private readonly PasswordHasher _passwordHasher;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);
        _passwordHasher = new PasswordHasher();
        _userService = new UserService(_repository, _passwordHasher, TimeProvider.System);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser()
    {
        var request = new CreateUserRequest(
            "testuser",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var result = await _userService.CreateUserAsync(request);

        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.FirstName.Should().Be("Test");
        result.LastName.Should().Be("User");
        result.BranchId.Should().Be("BR001");
        result.Role.Should().Be("Staff");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateUsername_ShouldThrow()
    {
        var request = new CreateUserRequest(
            "testuser",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        await _userService.CreateUserAsync(request);

        var act = () => _userService.CreateUserAsync(request);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Username already exists");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        var request = new CreateUserRequest(
            "testuser",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var created = await _userService.CreateUserAsync(request);
        var result = await _userService.GetUserByIdAsync(created.Id);

        result.Should().NotBeNull();
        result!.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var result = await _userService.GetUserByIdAsync(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUserStatusAsync_ShouldToggleStatus()
    {
        var request = new CreateUserRequest(
            "testuser",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var created = await _userService.CreateUserAsync(request);
        var result = await _userService.UpdateUserStatusAsync(created.Id, false);

        result.Should().BeTrue();

        var user = await _userService.GetUserByIdAsync(created.Id);
        user!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldResetPassword()
    {
        var request = new CreateUserRequest(
            "testuser",
            "TestPassword123!",
            "Test",
            null,
            "User",
            "BR001",
            "Staff");

        var created = await _userService.CreateUserAsync(request);
        var result = await _userService.ResetPasswordAsync(created.Id, "NewPassword123!");

        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.MustChangePassword.Should().BeTrue();
    }
}