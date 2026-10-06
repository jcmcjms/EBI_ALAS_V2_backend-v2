namespace Alas.Api.Infrastructure.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class;
}