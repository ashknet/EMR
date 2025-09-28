using System.Text.Json;
using EMR.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EMR.Infrastructure.Messaging;

public class OutboxDispatcher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxDispatcher> _logger;

    public OutboxDispatcher(IServiceProvider serviceProvider, ILogger<OutboxDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Polling loop suitable for local development. In production, a more robust scheduler is advised.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<EMRDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

                var batch = await db.OutboxMessages
                    .Where(m => m.PublishedAt == null)
                    .OrderBy(m => m.OccurredAt)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in batch)
                {
                    try
                    {
                        await publisher.PublishAsync(message.Domain, message.EventType, message.Payload, message.MessageId, message.CorrelationId, message.IdempotencyKey, stoppingToken);
                        message.PublishedAt = DateTime.UtcNow;
                        message.Error = null;
                    }
                    catch (Exception ex)
                    {
                        message.RetryCount += 1;
                        message.Error = ex.Message;
                        _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.MessageId);
                    }
                }

                if (batch.Count > 0)
                {
                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox dispatcher loop error");
            }

            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}

