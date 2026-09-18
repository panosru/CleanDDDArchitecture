using System.Reflection;
using CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;

public static class IntegrationEventsServiceCollectionExtensions
{
    /// <summary>
    ///     Registers the integration event transport chosen by <c>IntegrationEvents:Transport</c>:
    ///     in-process (the default, for the monolith) or Kafka (for separate services, using
    ///     <c>ConnectionStrings:kafka</c>). Call once per host.
    /// </summary>
    public static IServiceCollection AddIntegrationEvents(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(IntegrationEventsOptions.Section);
        services.AddOptions<IntegrationEventsOptions>().Bind(section);
        services.TryAddSingleton(TimeProvider.System);

        var options = section.Get<IntegrationEventsOptions>() ?? new IntegrationEventsOptions();

        if (options.Transport == IntegrationEventTransport.InProcess)
        {
            services.TryAddScoped<IIntegrationEventPublisher, InProcessIntegrationEventPublisher>();

            return services;
        }

        var bootstrapServers = configuration.GetConnectionString("kafka")
         ?? throw new InvalidOperationException(
                "IntegrationEvents:Transport is Kafka, but ConnectionStrings:kafka is not set.");
        var consumerGroup = options.ConsumerGroup
         ?? Assembly.GetEntryAssembly()?.GetName().Name
         ?? "cleanddd";

        services.TryAddSingleton<IIntegrationEventPublisher>(
            _ => new KafkaIntegrationEventPublisher(bootstrapServers, options.Topic));
        services.AddHostedService(
            provider => new KafkaIntegrationEventConsumer(
                bootstrapServers,
                options.Topic,
                consumerGroup,
                provider.GetRequiredService<IServiceScopeFactory>(),
                provider.GetRequiredService<ILogger<KafkaIntegrationEventConsumer>>()));

        return services;
    }

    /// <summary>Publishes the outbox table of <typeparamref name="TDbContext" /> in the background.</summary>
    public static IServiceCollection AddOutboxDispatcher<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddOptions<IntegrationEventsOptions>();
        services.AddHostedService<OutboxDispatcher<TDbContext>>();

        return services;
    }
}
