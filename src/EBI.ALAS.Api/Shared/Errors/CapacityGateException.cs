namespace EBI.ALAS.Api.Shared.Errors;

public sealed class CapacityGateException(string message) : Exception(message);