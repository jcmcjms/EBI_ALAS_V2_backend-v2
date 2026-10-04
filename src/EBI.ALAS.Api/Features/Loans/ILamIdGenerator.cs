namespace EBI.ALAS.Api.Features.Loans;

public interface ILamIdGenerator
{
    Task<string> GenerateGroupNumberAsync(CancellationToken ct = default);
    Task<IReadOnlyList<string>> GenerateLamIdsAsync(int count, CancellationToken ct = default);
}