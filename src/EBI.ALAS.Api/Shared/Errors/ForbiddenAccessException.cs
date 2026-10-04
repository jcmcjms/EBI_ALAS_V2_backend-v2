namespace EBI.ALAS.Api.Shared.Errors;

public sealed class ForbiddenAccessException(string message) : Exception(message);