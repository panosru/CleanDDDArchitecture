using AwesomeAssertions;
using CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.RemoveDeletedAccountTodos;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CleanDDDArchitecture.Domains.Todo.Tests.Unit;

public sealed class RemoveDeletedAccountTodosTests
{
    [Fact]
    public async Task RemovesEverythingTheDeletedAccountOwnedAsOfTheDeletion()
    {
        var repository = new RecordingRepository();
        var handler    = new RemoveDeletedAccountTodos(repository, NullLogger<RemoveDeletedAccountTodos>.Instance);
        var deleted    = AccountDeletedIntegrationEvent.For(Guid.NewGuid(), new DateTime(2026, 9, 18, 10, 0, 0, DateTimeKind.Utc));

        await handler.Handle(
            new IntegrationEventReceived<AccountDeletedIntegrationEvent>(deleted),
            TestContext.Current.CancellationToken);

        repository.Calls.Should().ContainSingle().Which.Should().Be((deleted.AccountId, deleted.OccurredAtUtc));
    }

    [Fact]
    public async Task HandlingTheSameEventTwiceIsHarmless()
    {
        var repository = new RecordingRepository();
        var handler    = new RemoveDeletedAccountTodos(repository, NullLogger<RemoveDeletedAccountTodos>.Instance);
        var received   = new IntegrationEventReceived<AccountDeletedIntegrationEvent>(
            AccountDeletedIntegrationEvent.For(Guid.NewGuid(), DateTime.UtcNow));

        await handler.Handle(received, TestContext.Current.CancellationToken);
        var act = () => handler.Handle(received, TestContext.Current.CancellationToken);

        await act.Should().NotThrowAsync("delivery is at least once, so the handler must be idempotent");
    }

    private sealed class RecordingRepository : ITodoListOwnerCleanup
    {
        public List<(Guid OwnerId, DateTimeOffset DeletedAtUtc)> Calls { get; } = [];

        public Task<int> SoftDeleteOwnedByAsync(Guid ownerId, DateTimeOffset deletedAtUtc, CancellationToken cancellationToken = default)
        {
            var removed = Calls.Any(call => call.OwnerId == ownerId) ? 0 : 2;
            Calls.Add((ownerId, deletedAtUtc));

            return Task.FromResult(removed);
        }
    }
}
