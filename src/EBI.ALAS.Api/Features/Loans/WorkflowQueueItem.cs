namespace EBI.ALAS.Api.Features.Loans;

public enum QueueItemState
{
    Active,
    Completed,
    Released,
    Cancelled
}

public sealed class WorkflowQueueItem
{
    public int Id { get; init; }
    public int LoanApplicationId { get; set; }
    public string QueuePartition { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public int Position { get; set; }
    public QueueItemState State { get; set; } = QueueItemState.Active;
    public int? OwnerUserId { get; set; }
    public DateTime? LeasedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? OwnerUser { get; set; }
}