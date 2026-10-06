using FluentValidation;

namespace Alas.Api.Features.Account;

public sealed class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    private const string PhonePattern = @"^\+?[0-9\s\-()]{7,20}$";

    public UpdateProfileRequestValidator()
    {
        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email!)
                .NotEmpty().WithMessage("Email must be a valid address or omitted.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");
        });

        When(x => x.Phone != null, () =>
        {
            RuleFor(x => x.Phone!)
                .NotEmpty().WithMessage("Phone must be a valid number or omitted.")
                .Matches(PhonePattern).WithMessage("Phone may contain digits, spaces and + - ( ) only (7-20 characters).");
        });
    }
}