namespace EBI.ALAS.Api.Features.Loans;

public interface ISignatureChainService
{
    Task<bool> VerifyChainAsync(int loanApplicationId, CancellationToken ct = default);
    Task AddSignatureAsync(int loanApplicationId, int userId, string role, string action, CancellationToken ct = default);
}