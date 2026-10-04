using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EBI.ALAS.Api.Infrastructure.SignalR;

public sealed class JwtUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext context)
    {
        return context.User?.FindFirst("uid")?.Value
            ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}