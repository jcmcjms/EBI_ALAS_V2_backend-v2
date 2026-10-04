using Microsoft.EntityFrameworkCore;
using EBI.ALAS.Api.Shared.Models;

namespace EBI.ALAS.Api.Features.Loans;

public sealed class LoanProductRepository(AppDbContext db) : ILoanProductRepository
{
    public Task<LoanProduct?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        db.LoanProducts.AsNoTracking().FirstOrDefaultAsync(p => p.ProductCode == code, ct);

    public Task<IReadOnlyList<LoanProduct>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default)
    {
        var query = db.LoanProducts.AsNoTracking();
        if (activeOnly) query = query.Where(p => p.IsActive);
        return query.ToListAsync(ct).ContinueWith(t => (IReadOnlyList<LoanProduct>)t.Result);
    }

    public async Task<LoanProduct> CreateAsync(LoanProduct product, CancellationToken ct = default)
    {
        db.LoanProducts.Add(product);
        await db.SaveChangesAsync(ct);
        return product;
    }

    public async Task UpdateAsync(LoanProduct product, CancellationToken ct = default)
    {
        db.LoanProducts.Update(product);
        await db.SaveChangesAsync(ct);
    }
}

public sealed class LoanProductSyncService : ILoanProductSyncService
{
    public Task SyncAsync(CancellationToken ct = default) => Task.CompletedTask;
}

public sealed class LoanProductService(
    ILoanProductRepository productRepository) : ILoanProductService
{
    public Task<LoanProduct?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        productRepository.GetByCodeAsync(code, ct);

    public Task<IReadOnlyList<LoanProduct>> GetAllAsync(bool activeOnly = true, CancellationToken ct = default) =>
        productRepository.GetAllAsync(activeOnly, ct);

    public Task<LoanProduct> CreateAsync(LoanProduct product, CancellationToken ct = default) =>
        productRepository.CreateAsync(product, ct);

    public async Task<LoanProduct> UpdateAsync(string code, LoanProduct product, CancellationToken ct = default)
    {
        var existing = await productRepository.GetByCodeAsync(code, ct);
        if (existing is null) throw new InvalidOperationException("Product not found");
        product.ProductCode = code;
        await productRepository.UpdateAsync(product, ct);
        return product;
    }
}

public sealed class LoanProductImportService : ILoanProductImportService
{
    public Task<ImportResult> ImportAsync(Stream excelStream, CancellationToken ct = default) =>
        Task.FromResult(new ImportResult(0, 0, 0, []));
}

public sealed class DocumentChecklistRepository(AppDbContext db) : IChecklistDocumentRepository
{
    public Task<DocumentChecklist?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.DocumentChecklists.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<IReadOnlyList<DocumentChecklist>> GetByLoanApplicationIdAsync(int loanApplicationId, CancellationToken ct = default) =>
        db.DocumentChecklists.AsNoTracking().Where(d => d.LoanApplicationId == loanApplicationId).ToListAsync(ct)
            .ContinueWith(t => (IReadOnlyList<DocumentChecklist>)t.Result);

    public async Task<DocumentChecklist> CreateAsync(DocumentChecklist document, CancellationToken ct = default)
    {
        db.DocumentChecklists.Add(document);
        await db.SaveChangesAsync(ct);
        return document;
    }

    public async Task UpdateAsync(DocumentChecklist document, CancellationToken ct = default)
    {
        db.DocumentChecklists.Update(document);
        await db.SaveChangesAsync(ct);
    }
}

public sealed class DocumentChecklistStore : IDocumentChecklistStore
{
    public Task<IReadOnlyList<DocumentChecklist>> GetChecklistAsync(int loanApplicationId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<DocumentChecklist>>([]);
}