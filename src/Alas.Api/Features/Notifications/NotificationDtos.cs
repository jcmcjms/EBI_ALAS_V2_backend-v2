namespace Alas.Api.Features.Notifications;

public sealed record NotificationResponse(
    int Id,
    string Title,
    string Description,
    string? Link,
    bool IsRead,
    DateTimeOffset CreatedAt,
    string Type,
    DateTimeOffset? ReadAt);

public sealed record InboxPage(
    IReadOnlyList<NotificationResponse> Items,
    int TotalCount,
    int UnreadCount);

public sealed record InboxQuery(
    int Page,
    int PageSize,
    string Status,
    string? Type,
    string? Search);

public sealed record MarkAllReadResponse(int ChangedCount);

public sealed record NotificationDraft(
    int UserId,
    string Title,
    string Description,
    string? Link,
    string? Type = null);

public sealed record NotificationPush(
    string Title,
    string Description,
    string? Link,
    DateTimeOffset Timestamp);