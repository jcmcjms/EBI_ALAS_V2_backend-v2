using FluentValidation;

namespace Alas.Api.Features.AuditLogs;

public sealed class AuditLogQueryParametersValidator : AbstractValidator<AuditLogQueryParameters>
{
    private static readonly string[] ValidActions = ["Create", "Update", "StatusChange", "Delete"];
    private static readonly string[] ValidEntityTypes = ["LoanApplication", "User", "Branch", "Role", "LoanProduct"];

    public AuditLogQueryParametersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.Action)
            .Must(a => string.IsNullOrEmpty(a) || ValidActions.Contains(a))
            .WithMessage($"Action must be one of: {string.Join(", ", ValidActions)}");

        RuleFor(x => x.EntityType)
            .Must(e => string.IsNullOrEmpty(e) || ValidEntityTypes.Contains(e))
            .WithMessage($"EntityType must be one of: {string.Join(", ", ValidEntityTypes)}");
    }
}