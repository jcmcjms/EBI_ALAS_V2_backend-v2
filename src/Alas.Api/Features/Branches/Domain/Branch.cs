namespace Alas.Api.Features.Branches.Domain;

public sealed class Branch
{
    public int Id { get; init; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; init; }
    public string? AreaCode { get; set; }
}