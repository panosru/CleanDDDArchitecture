using System.Text;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;

/// <summary>
///     Receives integration events from Kafka and hands them to this process's handlers. The
///     offset is committed only after the handlers succeed, so a crash means redelivery, not loss.
/// </summary>
public sealed partial class KafkaIntegrationEventConsumer(
    string                                 bootstrapServers,
    string                                 topic,
    string                                 consumerGroup,
    IServiceScopeFactory                   scopes,
    ILogger<KafkaIntegrationEventConsumer> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        Task.Factory.StartNew(() => Consume(stoppingToken), stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
           .Unwrap();

    private async Task Consume(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(
                new ConsumerConfig
                {
                    BootstrapServers    = bootstrapServers,
                    GroupId             = consumerGroup,
                    AutoOffsetReset     = AutoOffsetReset.Earliest,
                    EnableAutoCommit    = false,
                    AllowAutoCreateTopics = true
                })
           .Build();

        consumer.Subscribe(topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result;

                try
                {
                    result = consumer.Consume(stoppingToken);
                }
                catch (ConsumeException exception)
                {
                    LogConsumeFailed(exception, topic);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken).ConfigureAwait(false);
                    continue;
                }

                if (result?.Message is null)
                    continue;

                var type = Encoding.UTF8.GetString(result.Message.Headers.GetLastBytes(KafkaIntegrationEventPublisher.TypeHeader));

                await using (var scope = scopes.CreateAsyncScope())
                {
                    var local = ActivatorUtilities.CreateInstance<InProcessIntegrationEventPublisher>(scope.ServiceProvider);
                    await local.PublishAsync(IntegrationEventSerializer.Deserialize(type, result.Message.Value), stoppingToken)
                       .ConfigureAwait(false);
                }

                consumer.Commit(result);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Shutting down.
        }
        finally
        {
            consumer.Close();
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Consuming integration events from {Topic} failed")]
    private partial void LogConsumeFailed(Exception exception, string topic);
}
