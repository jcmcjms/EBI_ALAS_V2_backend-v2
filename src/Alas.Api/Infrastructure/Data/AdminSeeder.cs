using System.ComponentModel.DataAnnotations;
using Alas.Api.Features.Auth;
using Alas.Api.Features.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Alas.Api.Infrastructure.Data;

public sealed class AdminSeeder
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AdminSettings _adminSettings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AdminSeeder> _logger;

    public AdminSeeder(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        IOptions<AdminSettings> adminSettings,
        TimeProvider timeProvider,
        ILogger<AdminSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _adminSettings = adminSettings.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // Ensure database exists
        await _context.Database.EnsureCreatedAsync(ct);

        var username = _adminSettings.Username;

        if (await _context.Users.AnyAsync(u => u.Username == username, ct))
        {
            _logger.LogInformation("Admin user '{Username}' already exists, skipping seed", username);
            return;
        }

        var now = _timeProvider.GetUtcNow();

        var admin = new User
        {
            Username = username,
            PasswordHash = _passwordHasher.HashPassword(_adminSettings.Password),
            FirstName = _adminSettings.FirstName,
            LastName = _adminSettings.LastName,
            BranchId = _adminSettings.BranchId,
            Role = "Admin",
            IsActive = true,
            MustChangePassword = false,
            CreatedAt = now,
            Email = _adminSettings.Email
        };

        _context.Users.Add(admin);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Admin user '{Username}' seeded successfully", username);
    }
}

public sealed class AdminSettings
{
    [Required]
    public string Username { get; init; } = string.Empty;
    [Required]
    public string Password { get; init; } = string.Empty;
    [Required]
    public string FirstName { get; init; } = string.Empty;
    [Required]
    public string LastName { get; init; } = string.Empty;
    [Required]
    public string BranchId { get; init; } = string.Empty;
    public string? Email { get; init; }
}