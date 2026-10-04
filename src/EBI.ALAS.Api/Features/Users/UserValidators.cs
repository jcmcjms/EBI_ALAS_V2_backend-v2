using FluentValidation;
using EBI.ALAS.Api.Common.Constants;

namespace EBI.ALAS.Api.Features.Users;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Must contain uppercase")
            .Matches("[a-z]").WithMessage("Must contain lowercase")
            .Matches("[0-9]").WithMessage("Must contain digit")
            .Matches("[!?*.]").WithMessage("Must contain special char (!?*.)");
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Role).NotEmpty().Must(Roles.All.Contains).WithMessage("Invalid role");
        RuleFor(x => x.BranchCode).NotEmpty().MaximumLength(10);
    }
}

public sealed class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Role).NotEmpty().Must(Roles.All.Contains).WithMessage("Invalid role");
        RuleFor(x => x.BranchCode).NotEmpty().MaximumLength(10);
    }
}