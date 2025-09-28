namespace EMR.Infrastructure.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync(string domain, string eventType, string payload, Guid messageId, Guid? correlationId, string? idempotencyKey, CancellationToken cancellationToken);
}

public class ConsoleMessagePublisher : IMessagePublisher
{
    public Task PublishAsync(string domain, string eventType, string payload, Guid messageId, Guid? correlationId, string? idempotencyKey, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[PUBLISH] Domain={domain} EventType={eventType} MessageId={messageId} CorrelationId={correlationId} IdempotencyKey={idempotencyKey}\nPayload={payload}");
        return Task.CompletedTask;
    }
}

