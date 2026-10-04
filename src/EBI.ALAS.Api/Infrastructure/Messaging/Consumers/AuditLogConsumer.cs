using MassTransit;
using EBI.ALAS.Api.Infrastructure.Messaging.Events;

namespace EBI.ALAS.Api.Infrastructure.Messaging.Consumers;

public sealed class AuditLogConsumer(
    ILogger<AuditLogConsumer> logger) : IConsumer<LoanCreatedEvent>, IConsumer<LoanStatusChangedEvent>, IConsumer<LoanActionLoggedEvent>
{
    public Task Consume(ConsumeContext<LoanCreatedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Audit: Loan created | LoanId: {LoanId} | LamId: {LamId} | Group: {Group} | User: {UserId}",
            evt.LoanApplicationId, evt.LamId, evt.ApplicationGroupNo, evt.CreatedById);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<LoanStatusChangedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Audit: Loan status changed | LoanId: {LoanId} | {From} -> {To} | User: {UserId}",
            evt.LoanApplicationId, evt.FromStatus, evt.ToStatus, evt.UserId);
        return Task.CompletedTask;
    }

    public Task Consume(ConsumeContext<LoanActionLoggedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Audit: Loan action | LoanId: {LoanId} | Action: {Action} | {From} -> {To} | User: {UserId}",
            evt.LoanApplicationId, evt.Action, evt.FromStatus, evt.ToStatus, evt.UserId);
        return Task.CompletedTask;
    }
}