using FluentValidation;

namespace Alas.Api.Features.ApprovalMatrix;

public sealed class CreateApprovalAuthorityRequestValidator : AbstractValidator<CreateApprovalAuthorityRequest>
{
    public CreateApprovalAuthorityRequestValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Key is required")
            .MaximumLength(50).WithMessage("Key must not exceed 50 characters")
            .Matches(@"^[A-Z0-9_]+$").WithMessage("Key must contain only uppercase letters, numbers, and underscores");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters");

        RuleFor(x => x.Tier)
            .GreaterThan(0).WithMessage("Tier must be greater than 0");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority must be non-negative");

        RuleFor(x => x.MaxTotalExposure)
            .GreaterThanOrEqualTo(0).WithMessage("Max total exposure must be non-negative");
    }
}

public sealed class UpdateApprovalAuthorityRequestValidator : AbstractValidator<UpdateApprovalAuthorityRequest>
{
    public UpdateApprovalAuthorityRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters");

        RuleFor(x => x.Tier)
            .GreaterThan(0).WithMessage("Tier must be greater than 0");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority must be non-negative");

        RuleFor(x => x.MaxTotalExposure)
            .GreaterThanOrEqualTo(0).WithMessage("Max total exposure must be non-negative");
    }
}

public sealed class CreateDeviationCatalogRequestValidator : AbstractValidator<CreateDeviationCatalogRequest>
{
    public CreateDeviationCatalogRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}

public sealed class UpdateDeviationCatalogRequestValidator : AbstractValidator<UpdateDeviationCatalogRequest>
{
    public UpdateDeviationCatalogRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}