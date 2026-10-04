using EBI.ALAS.Api.Infrastructure.Messaging;

namespace EBI.ALAS.Api.Infrastructure.Messaging.Events;

public sealed record LoanCreatedEvent(
    int LoanApplicationId,
    string LamId,
    string ApplicationGroupNo,
    string BranchCode,
    string ProductCode,
    decimal ProposedAmount,
    string Status,
    int CreatedById) : IntegrationEvent;

public sealed record LoanStatusChangedEvent(
    int LoanApplicationId,
    string LamId,
    string FromStatus,
    string ToStatus,
    int UserId,
    string? Remarks) : IntegrationEvent;

public sealed record LoanActionLoggedEvent(
    int LoanApplicationId,
    string Action,
    string FromStatus,
    string ToStatus,
    int UserId,
    string? Remarks,
    DateTime ActionDate) : IntegrationEvent;

public sealed record NotificationCreatedEvent(
    int NotificationId,
    int UserId,
    string Title,
    string Message,
    string? Link) : IntegrationEvent;

public sealed record DocumentRemarkAddedEvent(
    int DocumentRemarkId,
    int LoanApplicationId,
    int UserId,
    string RemarkType,
    string Content) : IntegrationEvent;