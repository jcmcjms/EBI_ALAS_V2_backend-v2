using FluentValidation;

namespace EBI.ALAS.Api.Common.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain a digit")
            .Matches("[!?*.]").WithMessage("Password must contain a special character (! ? * .)");
    }

    public static IRuleBuilderOptions<T, string?> OptionalPassword<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must((_, value) => string.IsNullOrEmpty(value) || IsValidPassword(value))
            .WithMessage("Password must be at least 8 characters with uppercase, lowercase, digit, and special character (! ? * .)");
    }

    private static bool IsValidPassword(string password)
    {
        return password.Length >= 8
            && password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(char.IsDigit)
            && password.Any(c => "!?*.".Contains(c));
    }
}