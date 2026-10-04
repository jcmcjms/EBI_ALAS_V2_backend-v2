using Microsoft.AspNetCore.Http;

namespace EBI.ALAS.Api.Infrastructure.Security;

public sealed class BankingSecurityValidator
{
    private readonly IpAllowlistOptions? _ipAllowlistOptions;

    public BankingSecurityValidator(IpAllowlistOptions? ipAllowlistOptions = null)
    {
        _ipAllowlistOptions = ipAllowlistOptions;
    }

    public bool IsAdminIpAllowed(HttpContext context)
    {
        if (_ipAllowlistOptions?.AllowedIps is not { Count: > 0 })
        {
            return true; // No restriction configured
        }

        var remoteIp = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(remoteIp))
        {
            return false;
        }

        return _ipAllowlistOptions.AllowedIps.Contains(remoteIp);
    }

    public void ValidateNoSecretsInLog(object? obj)
    {
        // Placeholder for secret detection logic
        // In production, would scan for patterns like passwords, tokens, keys
    }
}

public sealed class IpAllowlistOptions
{
    public HashSet<string> AllowedIps { get; init; } = [];
}