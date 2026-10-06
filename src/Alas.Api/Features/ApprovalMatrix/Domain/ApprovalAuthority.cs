namespace Alas.Api.Features.ApprovalMatrix.Domain;

public enum DeviationSeverity
{
    None = 0,
    Minor = 1,
    Major = 2
}

public enum AuthorityScope
{
    Branch = 0,
    Area = 1,
    Global = 2
}

public sealed class ApprovalAuthority
{
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int Tier { get; set; }
    public int Priority { get; set; }
    public bool AllowNew { get; set; } = true;
    public bool AllowRenewal { get; set; } = true;
    public DeviationSeverity MaxSeverity { get; set; }
    public decimal MaxTotalExposure { get; set; }
    public AuthorityScope ScopeType { get; set; }
}

public sealed class DeviationCatalogItem
{
    public int Id { get; init; }
    public string Description { get; set; } = string.Empty;
    public DeviationSeverity Severity { get; set; }
}