using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EBI.ALAS.Api.Features.Auth;
using EBI.ALAS.Api.Features.Branches;
using EBI.ALAS.Api.Features.Loans;
using EBI.ALAS.Api.Features.ApprovalMatrix;
using EBI.ALAS.Api.Features.SystemSettings;
using EBI.ALAS.Api.Common.Constants;
using EBI.ALAS.Api.Shared.Time;

namespace EBI.ALAS.Api.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext db, IServiceProvider services, CancellationToken ct = default)
    {
        // Apply migrations
        await db.Database.MigrateAsync(ct);

        // Seed branches
        await SeedBranchesAsync(db, ct);

        // Seed default admin user
        await SeedAdminUserAsync(db, services, ct);

        // Seed loan products
        await SeedLoanProductsAsync(db, ct);

        // Seed approval matrix
        await SeedApprovalMatrixAsync(db, ct);

        // Seed deviation catalog
        await SeedDeviationCatalogAsync(db, ct);

        // Seed workflow configuration
        await SeedWorkflowConfigAsync(db, ct);
    }

    private static async Task SeedBranchesAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.Branches.AnyAsync(ct)) return;

        var branches = new[]
        {
            new Branch { Code = "011", Name = "Head Office", AreaCode = "HO", Region = "NCR", Address = "Makati City", IsActive = true },
            new Branch { Code = "001", Name = "Manila", AreaCode = "NCR", Region = "NCR", Address = "Manila", IsActive = true },
            new Branch { Code = "002", Name = "Quezon City", AreaCode = "NCR", Region = "NCR", Address = "Quezon City", IsActive = true },
            new Branch { Code = "003", Name = "Caloocan", AreaCode = "NCR", Region = "NCR", Address = "Caloocan City", IsActive = true },
            new Branch { Code = "004", Name = "Pasig", AreaCode = "NCR", Region = "NCR", Address = "Pasig City", IsActive = true },
            new Branch { Code = "005", Name = "Makati", AreaCode = "NCR", Region = "NCR", Address = "Makati City", IsActive = true },
            new Branch { Code = "006", Name = "Mandaluyong", AreaCode = "NCR", Region = "NCR", Address = "Mandaluyong City", IsActive = true },
            new Branch { Code = "007", Name = "San Juan", AreaCode = "NCR", Region = "NCR", Address = "San Juan City", IsActive = true },
            new Branch { Code = "008", Name = "Marikina", AreaCode = "NCR", Region = "NCR", Address = "Marikina City", IsActive = true },
            new Branch { Code = "009", Name = "Cebu", AreaCode = "VIS", Region = "Visayas", Address = "Cebu City", IsActive = true },
            new Branch { Code = "010", Name = "Davao", AreaCode = "MIN", Region = "Mindanao", Address = "Davao City", IsActive = true },
        };

        db.Branches.AddRange(branches);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedAdminUserAsync(AppDbContext db, IServiceProvider services, CancellationToken ct)
    {
        if (await db.Users.AnyAsync(u => u.Username == "admin", ct)) return;

        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var timeProvider = services.GetRequiredService<ITimeProvider>();

        var admin = new User
        {
            Username = "admin",
            Email = "admin@ebi.com.ph",
            PasswordHash = passwordHasher.HashPassword("admin123"),
            FirstName = "System",
            LastName = "Administrator",
            Role = Roles.Admin,
            BranchCode = "011",
            IsActive = true,
            MustChangePassword = true,
            CreatedAt = timeProvider.UtcNow
        };

        db.Users.Add(admin);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedLoanProductsAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.LoanProducts.AnyAsync(ct)) return;

        var products = new[]
        {
            new LoanProduct
            {
                ProductCode = "PL",
                ProductName = "Personal Loan",
                Description = "Personal Loan Product",
                InterestRate = 0.035m,
                MinTermDays = 30,
                MaxTermDays = 365,
                MinAmount = 10000,
                MaxAmount = 500000,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new LoanProduct
            {
                ProductCode = "HL",
                ProductName = "Housing Loan",
                Description = "Housing Loan Product",
                InterestRate = 0.045m,
                MinTermDays = 365,
                MaxTermDays = 7300,
                MinAmount = 500000,
                MaxAmount = 10000000,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new LoanProduct
            {
                ProductCode = "CL",
                ProductName = "Car Loan",
                Description = "Car Loan Product",
                InterestRate = 0.055m,
                MinTermDays = 365,
                MaxTermDays = 2190,
                MinAmount = 200000,
                MaxAmount = 3000000,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        db.LoanProducts.AddRange(products);
        await db.SaveChangesAsync(ct);

        // Seed checklists
        var checklists = products.SelectMany(p => new[]
        {
            new LoanProductChecklist { ProductCode = p.ProductCode, DocumentCode = "ID", DocumentName = "Valid ID", IsRequired = true, SortOrder = 1 },
            new LoanProductChecklist { ProductCode = p.ProductCode, DocumentCode = "POI", DocumentName = "Proof of Income", IsRequired = true, SortOrder = 2 },
            new LoanProductChecklist { ProductCode = p.ProductCode, DocumentCode = "POB", DocumentName = "Proof of Billing", IsRequired = true, SortOrder = 3 },
        });

        db.LoanProductChecklists.AddRange(checklists);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedApprovalMatrixAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.ApprovalAuthorities.AnyAsync(ct)) return;

        var authorities = new[]
        {
            new ApprovalAuthority
            {
                LoanType = "New",
                MinExposure = 0,
                MaxExposure = 500000,
                DeviationSeverity = DeviationSeverity.None,
                Tier = 1,
                Priority = 1,
                ApproverRole = Roles.Approver,
                BranchCode = null,
                AreaCode = null,
                IsActive = true
            },
            new ApprovalAuthority
            {
                LoanType = "New",
                MinExposure = 500001,
                MaxExposure = 2000000,
                DeviationSeverity = DeviationSeverity.None,
                Tier = 2,
                Priority = 1,
                ApproverRole = Roles.Approver,
                BranchCode = null,
                AreaCode = null,
                IsActive = true
            },
            new ApprovalAuthority
            {
                LoanType = "New",
                MinExposure = 2000001,
                MaxExposure = 5000000,
                DeviationSeverity = DeviationSeverity.None,
                Tier = 3,
                Priority = 1,
                ApproverRole = Roles.Approver,
                BranchCode = null,
                AreaCode = null,
                IsActive = true
            },
            new ApprovalAuthority
            {
                LoanType = "Renewal",
                MinExposure = 0,
                MaxExposure = 1000000,
                DeviationSeverity = DeviationSeverity.None,
                Tier = 1,
                Priority = 1,
                ApproverRole = Roles.Approver,
                BranchCode = null,
                AreaCode = null,
                IsActive = true
            }
        };

        db.ApprovalAuthorities.AddRange(authorities);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedDeviationCatalogAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.DeviationCatalog.AnyAsync(ct)) return;

        var deviations = new[]
        {
            new DeviationCatalogItem { Code = "DEV001", Description = "Exceeds maximum loanable amount", Severity = DeviationSeverity.Major, IsActive = true },
            new DeviationCatalogItem { Code = "DEV002", Description = "Incomplete documents", Severity = DeviationSeverity.Minor, IsActive = true },
            new DeviationCatalogItem { Code = "DEV003", Description = "NTHP below minimum", Severity = DeviationSeverity.Major, IsActive = true },
            new DeviationCatalogItem { Code = "DEV004", Description = "Collateral deficiency", Severity = DeviationSeverity.Major, IsActive = true },
            new DeviationCatalogItem { Code = "DEV005", Description = "Fee deviation", Severity = DeviationSeverity.Minor, IsActive = true }
        };

        db.DeviationCatalog.AddRange(deviations);
        await db.SaveChangesAsync(ct);
    }

    private static async Task SeedWorkflowConfigAsync(AppDbContext db, CancellationToken ct)
    {
        if (await db.WorkflowConfigurations.AnyAsync(ct)) return;

        var config = new WorkflowConfiguration
        {
            RequireRecommendation = false,
            SlaForRecommendationHours = 4,
            SlaForCheckingHours = 8,
            SlaForApprovalHours = 8,
            SlaForRevisionHours = 24,
            SlaForDisbursementHours = 24,
            MinimumNthp = 5000,
            CreatedAt = DateTime.UtcNow
        };

        db.WorkflowConfigurations.Add(config);
        await db.SaveChangesAsync(ct);
    }
}