using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Shared.Models;
using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Users;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<PagedResult<UserListItemDto>> GetPagedAsync(
        int page, int pageSize, string? search, string? role, string? branchCode, bool? isActive, CancellationToken ct = default)
    {
        var query = db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(u =>
                u.Username.Contains(s) ||
                u.Email.Contains(s) ||
                u.FirstName.Contains(s) ||
                u.LastName.Contains(s));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(branchCode))
        {
            query = query.Where(u => u.BranchCode == branchCode);
        }

        if (isActive.HasValue)
        {
            query = query.Where(u => u.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(u => u.Username)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserListItemDto(
                u.Id, u.Username, u.Email, u.FirstName, u.MiddleName, u.LastName, u.Suffix,
                u.Role, u.BranchCode, u.JobTitle, u.IsActive, u.CreatedAt, u.LastLoginAt))
            .ToListAsync(ct);

        return new PagedResult<UserListItemDto>(items, totalCount, page, pageSize);
    }

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }

    public Task<bool> UsernameExistsAsync(string username, int? excludeId = null, CancellationToken ct = default) =>
        db.Users.AsNoTracking().AnyAsync(u => u.Username == username && u.Id != (excludeId ?? 0), ct);

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null, CancellationToken ct = default) =>
        db.Users.AsNoTracking().AnyAsync(u => u.Email == email && u.Id != (excludeId ?? 0), ct);

    public Task<IReadOnlyList<User>> GetByRoleAndBranchAsync(string role, string branchCode, CancellationToken ct = default) =>
        db.Users
            .AsNoTracking()
            .Where(u => u.Role == role && u.BranchCode == branchCode && u.IsActive)
            .ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<User>)t.Result);
}