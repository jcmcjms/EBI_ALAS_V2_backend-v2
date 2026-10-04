using EBI.ALAS.Api.Features.Loans;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Loans;

public interface IChecklistDocumentRepository
{
    Task<DocumentChecklist?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DocumentChecklist>> GetByLoanApplicationIdAsync(int loanApplicationId, CancellationToken ct = default);
    Task<DocumentChecklist> CreateAsync(DocumentChecklist document, CancellationToken ct = default);
    Task UpdateAsync(DocumentChecklist document, CancellationToken ct = default);
}

public interface IDocumentChecklistStore
{
    static readonly IReadOnlyList<string> UnresolvedStatuses = ["Pending", "Rejected"];
    Task<IReadOnlyList<DocumentChecklist>> GetChecklistAsync(int loanApplicationId, CancellationToken ct = default);
}

public interface ILoanProductRepository
{
    Task<LoanProduct?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<LoanProduct>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default);
    Task<LoanProduct> CreateAsync(LoanProduct product, CancellationToken ct = default);
    Task UpdateAsync(LoanProduct product, CancellationToken ct = default);
}

public interface ILoanProductSyncService
{
    Task SyncAsync(CancellationToken ct = default);
}

public interface ILoanProductService
{
    Task<LoanProduct?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<LoanProduct>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default);
    Task<LoanProduct> CreateAsync(LoanProduct product, CancellationToken ct = default);
    Task<LoanProduct> UpdateAsync(string code, LoanProduct product, CancellationToken ct = default);
}

public interface ILoanProductImportService
{
    Task<ImportResult> ImportAsync(Stream excelStream, CancellationToken ct = default);
}

public sealed record ImportResult(int Created, int Updated, int Errors, IReadOnlyList<string> ErrorMessages);