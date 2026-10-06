using Alas.Api.Composition;
using Alas.Api.Features.Users.Domain;
using Alas.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alas.Api.Features.Users;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<PagedResult<UserResponse>> GetUsersAsync(UserQueryParameters parameters, CancellationToken ct = default)
    {
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search;
            query = query.Where(u =>
                u.Username.Contains(search) ||
                u.FirstName.Contains(search) ||
                u.LastName.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Role))
            query = query.Where(u => u.Role == parameters.Role);

        if (!string.IsNullOrWhiteSpace(parameters.BranchId))
            query = query.Where(u => u.BranchId == parameters.BranchId);

        if (parameters.IsActive.HasValue)
            query = query.Where(u => u.IsActive == parameters.IsActive.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .ThenBy(u => u.Id)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(u => new UserResponse(
                u.Id, u.Username, u.FirstName, u.MiddleName, u.LastName,
                u.BranchId, u.Role, u.IsActive, u.CreatedAt, u.JobTitle, u.Email, u.Phone))
            .ToListAsync(ct);

        return new PagedResult<UserResponse>(items, totalCount, parameters.PageNumber, parameters.PageSize);
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken ct = default) =>
        await _context.Users.FindAsync([id], ct);

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken ct = default) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<bool> UsernameExistsAsync(string username, int? excludeId = null, CancellationToken ct = default)
    {
        var query = _context.Users.Where(u => u.Username == username);
        if (excludeId.HasValue) query = query.Where(u => u.Id != excludeId.Value);
        return await query.AnyAsync(ct);
    }

    public async Task AddUserAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateUserAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}