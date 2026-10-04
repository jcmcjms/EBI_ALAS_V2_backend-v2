namespace EBI.ALAS.Api.Features.Loans;

public sealed class SignatureChainService : ISignatureChainService
{
    public Task<bool> VerifyChainAsync(int loanApplicationId, CancellationToken ct = default) => Task.FromResult(true);
    public Task AddSignatureAsync(int loanApplicationId, int userId, string role, string action, CancellationToken ct = default) => Task.CompletedTask;
}