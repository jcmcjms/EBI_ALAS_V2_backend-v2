using Alas.Api.Features.AuditLogs;
using Alas.Api.Infrastructure.Messaging.Events;
using MassTransit;

namespace Alas.Api.Infrastructure.Messaging.Consumers;

public sealed class AuditLogConsumer : IConsumer<AuditLogRecordedEvent>
{
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditLogConsumer> _logger;

    public AuditLogConsumer(IAuditLogService auditLogService, ILogger<AuditLogConsumer> logger)
    {
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AuditLogRecordedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation(
            "Processing audit log: {Action} on {EntityType}/{EntityId} by {UserName}",
            msg.Action, msg.EntityType, msg.EntityId, msg.UserName);

        await _auditLogService.LogAsync(
            msg.UserId,
            msg.UserName,
            msg.Action,
            msg.EntityType,
            msg.EntityId,
            msg.EntityLabel,
            msg.Summary,
            msg.RawChanges,
            msg.IpAddress,
            msg.UserAgent,
            context.CancellationToken);
    }
}