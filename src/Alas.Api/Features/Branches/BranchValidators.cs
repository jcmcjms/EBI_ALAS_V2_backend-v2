using FluentValidation;

namespace Alas.Api.Features.Branches;

public sealed class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Branch code is required")
            .MaximumLength(20).WithMessage("Branch code must not exceed 20 characters")
            .Matches(@"^[A-Z0-9]+$").WithMessage("Branch code must contain only uppercase letters and numbers");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Branch name is required")
            .MaximumLength(100).WithMessage("Branch name must not exceed 100 characters");

        RuleFor(x => x.AreaCode)
            .MaximumLength(10).WithMessage("Area code must not exceed 10 characters");
    }
}

public sealed class UpdateBranchRequestValidator : AbstractValidator<UpdateBranchRequest>
{
    public UpdateBranchRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Branch name is required")
            .MaximumLength(100).WithMessage("Branch name must not exceed 100 characters");

        RuleFor(x => x.AreaCode)
            .MaximumLength(10).WithMessage("Area code must not exceed 10 characters");
    }
}