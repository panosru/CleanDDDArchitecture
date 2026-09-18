using AwesomeAssertions;
using CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.Outbox;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace CleanDDDArchitecture.Domains.Shared.Tests.Unit;

public sealed class OutboxDispatcherTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    private readonly RecordingPublisher _publisher = new();

    private ServiceProvider _services = null!;

    public async ValueTask InitializeAsync()
    {
        await _connection.OpenAsync(TestContext.Current.CancellationToken);

        var services = new ServiceCollection();
        services.AddDbContext<OutboxContext>(options => options.UseSqlite(_connection));
        services.AddSingleton<IIntegrationEventPublisher>(_publisher);
        _services = services.BuildServiceProvider();

        await using var scope = _services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<OutboxContext>().Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _services.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task PublishesPendingMessagesOldestFirstAndMarksThemProcessed()
    {
        var first  = Event(minute: 1);
        var second = Event(minute: 2);
        await AddAsync(second, first);

        var published = await Dispatcher().DispatchPendingAsync(TestContext.Current.CancellationToken);

        published.Should().Be(2);
        _publisher.Published.Should().Equal(first, second);
        (await Messages()).Should().OnlyContain(message => message.ProcessedAtUtc != null);
    }

    [Fact]
    public async Task AProcessedMessageIsNotPublishedAgain()
    {
        await AddAsync(Event(minute: 1));
        await Dispatcher().DispatchPendingAsync(TestContext.Current.CancellationToken);

        var published = await Dispatcher().DispatchPendingAsync(TestContext.Current.CancellationToken);

        published.Should().Be(0);
        _publisher.Published.Should().HaveCount(1);
    }

    [Fact]
    public async Task AFailedPublishIsRecordedAndRetriedNextCycle()
    {
        await AddAsync(Event(minute: 1));
        _publisher.FailNext = true;

        await Dispatcher().DispatchPendingAsync(TestContext.Current.CancellationToken);

        var failed = (await Messages()).Single();
        failed.ProcessedAtUtc.Should().BeNull();
        failed.Attempts.Should().Be(1);
        failed.LastError.Should().Be("broker unavailable");

        await Dispatcher().DispatchPendingAsync(TestContext.Current.CancellationToken);

        (await Messages()).Single().ProcessedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public async Task InProcessPublishingReachesTheTypedHandler()
    {
        var services = new ServiceCollection();
        services.AddSingleton<RecordingHandler>();
        services.AddSingleton<INotificationHandler<IntegrationEventReceived<AccountDeletedIntegrationEvent>>>(p => p.GetRequiredService<RecordingHandler>());
        services.AddTransient<IPublisher, Mediator>();
        await using var provider = services.BuildServiceProvider();
        var deleted = Event(minute: 1);

        await new InProcessIntegrationEventPublisher(provider.GetRequiredService<IPublisher>())
           .PublishAsync(deleted, TestContext.Current.CancellationToken);

        provider.GetRequiredService<RecordingHandler>().Received.Should().ContainSingle().Which.Should().Be(deleted);
    }

    private static AccountDeletedIntegrationEvent Event(int minute) =>
        AccountDeletedIntegrationEvent.For(Guid.NewGuid(), new DateTime(2026, 9, 18, 10, minute, 0, DateTimeKind.Utc));

    private OutboxDispatcher<OutboxContext> Dispatcher() => new(
        _services.GetRequiredService<IServiceScopeFactory>(),
        Options.Create(new IntegrationEventsOptions()),
        TimeProvider.System,
        NullLogger<OutboxDispatcher<OutboxContext>>.Instance);

    private async Task AddAsync(params IIntegrationEvent[] events)
    {
        await using var scope = _services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<OutboxContext>();
        database.AddRange(events.Select(OutboxMessage.From));
        await database.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private async Task<List<OutboxMessage>> Messages()
    {
        await using var scope = _services.CreateAsyncScope();

        return await scope.ServiceProvider.GetRequiredService<OutboxContext>().Set<OutboxMessage>()
           .AsNoTracking().ToListAsync(TestContext.Current.CancellationToken);
    }

    public sealed class OutboxContext(DbContextOptions<OutboxContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyOutbox();
    }

    private sealed class RecordingPublisher : IIntegrationEventPublisher
    {
        public List<IIntegrationEvent> Published { get; } = [];

        public bool FailNext { get; set; }

        public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            if (FailNext)
            {
                FailNext = false;

                throw new InvalidOperationException("broker unavailable");
            }

            Published.Add(integrationEvent);

            return Task.CompletedTask;
        }
    }

    private sealed class RecordingHandler : INotificationHandler<IntegrationEventReceived<AccountDeletedIntegrationEvent>>
    {
        public List<AccountDeletedIntegrationEvent> Received { get; } = [];

        public Task Handle(IntegrationEventReceived<AccountDeletedIntegrationEvent> notification, CancellationToken cancellationToken)
        {
            Received.Add(notification.Event);

            return Task.CompletedTask;
        }
    }
}
