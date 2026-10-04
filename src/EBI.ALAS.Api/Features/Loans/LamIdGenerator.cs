using Microsoft.EntityFrameworkCore;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class LamIdGenerator(AppDbContext db) : ILamIdGenerator
{
    public async Task<string> GenerateGroupNumberAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var prefix = $"LAM{now:yyyyMMdd}";
        var lastGroup = await db.LoanApplications
            .AsNoTracking()
            .Where(l => l.ApplicationGroupNo.StartsWith(prefix))
            .OrderByDescending(l => l.ApplicationGroupNo)
            .Select(l => l.ApplicationGroupNo)
            .FirstOrDefaultAsync(ct);

        int sequence = 1;
        if (!string.IsNullOrEmpty(lastGroup) && lastGroup.Length > prefix.Length)
        {
            if (int.TryParse(lastGroup[prefix.Length..], out var lastSeq))
            {
                sequence = lastSeq + 1;
            }
        }

        return $"{prefix}{sequence:D4}";
    }

    public async Task<IReadOnlyList<string>> GenerateLamIdsAsync(int count, CancellationToken ct = default)
    {
        var groupNo = await GenerateGroupNumberAsync(ct);
        var result = new List<string>();

        for (int i = 1; i <= count; i++)
        {
            result.Add($"{groupNo}-{i:D3}");
        }

        return result;
    }
}