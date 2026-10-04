using EBI.ALAS.Api.Features.ApprovalMatrix;

namespace EBI.ALAS.Api.Features.ApprovalMatrix;

public sealed class DeviationCatalogItem
{
    public int Id { get; init; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DeviationSeverity Severity { get; set; }
    public bool IsActive { get; set; } = true;
}