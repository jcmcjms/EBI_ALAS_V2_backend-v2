using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Features.Auth;

namespace EBI.ALAS.Api.Features.Account;

public sealed class AccountRepository(AppDbContext db) : IAccountRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }
}