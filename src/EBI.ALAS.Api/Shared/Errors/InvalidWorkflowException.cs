namespace EBI.ALAS.Api.Shared.Errors;

public sealed class InvalidWorkflowException(string fromStatus, string toStatus, string role)
    : Exception($"Invalid workflow transition from '{fromStatus}' to '{toStatus}' for role '{role}'.")
{
    public string FromStatus { get; } = fromStatus;
    public string ToStatus { get; } = toStatus;
    public string Role { get; } = role;
}