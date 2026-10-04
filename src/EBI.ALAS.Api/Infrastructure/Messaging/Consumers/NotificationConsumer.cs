using MassTransit;
using EBI.ALAS.Api.Infrastructure.Messaging.Events;

namespace EBI.ALAS.Api.Infrastructure.Messaging.Consumers;

public sealed class NotificationConsumer(
    ILogger<NotificationConsumer> logger) : IConsumer<NotificationCreatedEvent>
{
    public Task Consume(ConsumeContext<NotificationCreatedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Notification queued | NotificationId: {NotificationId} | User: {UserId} | Title: {Title}",
            evt.NotificationId, evt.UserId, evt.Title);
        // In a real implementation, this would push to SignalR
        return Task.CompletedTask;
    }
}