namespace EMR.Infrastructure.Entities;

public class OutboxMessage
{
    public Guid OutboxMessageId { get; set; }
    public Guid MessageId { get; set; }
    public Guid? CorrelationId { get; set; }
    public string Domain { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public int RetryCount { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? Error { get; set; }
    public string? IdempotencyKey { get; set; }
}

