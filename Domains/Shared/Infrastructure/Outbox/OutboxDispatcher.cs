using CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.Outbox;

/// <summary>
///     Publishes the outbox of <typeparamref name="TDbContext" />: unprocessed messages, oldest
///     first, each marked processed once published. A failure is recorded and retried on the next
///     cycle, up to <see cref="OutboxMessage.MaxAttempts" />. Delivery is therefore at least once.
/// </summary>
public sealed partial class OutboxDispatcher<TDbContext>(
    IServiceScopeFactory                  scopes,
    IOptions<IntegrationEventsOptions>    options,
    TimeProvider                          time,
    ILogger<OutboxDispatcher<TDbContext>> logger) : BackgroundService
    where TDbContext : DbContext
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Value.OutboxPollingInterval, time);

        do
        {
            try
            {
                await DispatchPendingAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogCycleFailed(exception);
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }

    /// <summary>Publishes one batch; returns how many messages were published.</summary>
    public async Task<int> DispatchPendingAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopes.CreateAsyncScope();
        var database  = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();
        var outbox    = database.Set<OutboxMessage>();

        List<OutboxMessage> pending = await outbox
           .AsNoTracking()
           .Where(message => message.ProcessedAtUtc == null && message.Attempts < OutboxMessage.MaxAttempts)
           .OrderBy(message => message.OccurredAtUtc)
           .Take(options.Value.OutboxBatchSize)
           .ToListAsync(cancellationToken)
           .ConfigureAwait(false);

        var published = 0;

        foreach (var message in pending)
        {
            try
            {
                await publisher.PublishAsync(IntegrationEventSerializer.Deserialize(message.Type, message.Payload), cancellationToken)
                   .ConfigureAwait(false);

                var now = time.GetUtcNow().UtcDateTime;
                await outbox.Where(m => m.Id == message.Id)
                   .ExecuteUpdateAsync(set => set.SetProperty(m => m.ProcessedAtUtc, now), cancellationToken)
                   .ConfigureAwait(false);

                published++;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                LogPublishFailed(exception, message.Id, message.Type, message.Attempts + 1);

                var error = exception.Message.Length > 2000 ? exception.Message[..2000] : exception.Message;
                await outbox.Where(m => m.Id == message.Id)
                   .ExecuteUpdateAsync(
                        set => set
                           .SetProperty(m => m.Attempts, m => m.Attempts + 1)
                           .SetProperty(m => m.LastError, error),
                        cancellationToken)
                   .ConfigureAwait(false);
            }
        }

        return published;
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Outbox dispatch cycle failed")]
    private partial void LogCycleFailed(Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Publishing outbox message {MessageId} ({Type}) failed, attempt {Attempt}")]
    private partial void LogPublishFailed(Exception exception, Guid messageId, string type, int attempt);
}
