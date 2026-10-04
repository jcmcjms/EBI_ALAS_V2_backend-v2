using Microsoft.EntityFrameworkCore;

namespace EBI.ALAS.Api.Features.Auth;

public sealed class AuthRepository(AppDbContext db) : IAuthRepository
{
    public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        db.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User> CreateAsync(User user, CancellationToken ct = default)
    {
        db.Set<User>().Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        db.Set<User>().Update(user);
        await db.SaveChangesAsync(ct);
    }
}