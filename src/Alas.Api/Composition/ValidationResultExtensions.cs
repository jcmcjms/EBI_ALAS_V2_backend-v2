namespace Alas.Api.Composition;

/// <summary>
/// Shared extension for converting FluentValidation results to Problem Details format.
/// Used by all endpoint groups that validate input.
/// </summary>
internal static class ValidationResultExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this FluentValidation.Results.ValidationResult result) =>
        result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
}