namespace EBI.ALAS.Api.Shared.Errors;

public sealed class NotFoundException(string message) : Exception(message)
{
    public NotFoundException(string entityName, object key)
        : this($"{entityName} with key '{key}' was not found.")
    { }
}