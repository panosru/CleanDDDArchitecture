using CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.RemoveDeletedAccountTodos;

/// <summary>
///     When the Account context reports an account deleted, remove that user's todo lists.
/// </summary>
/// <remarks>
///     The only link between the two contexts is the <see cref="AccountDeletedIntegrationEvent" />
///     contract in Shared: Todo never references Account. The removal is timestamped with the
///     deletion itself, so a redelivered event produces the same result.
/// </remarks>
public sealed partial class RemoveDeletedAccountTodos(
    ITodoListOwnerCleanup              lists,
    ILogger<RemoveDeletedAccountTodos> logger)
    : INotificationHandler<IntegrationEventReceived<AccountDeletedIntegrationEvent>>
{
    public async Task Handle(
        IntegrationEventReceived<AccountDeletedIntegrationEvent> notification,
        CancellationToken                                        cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var deleted = notification.Event;
        var removed = await lists.SoftDeleteOwnedByAsync(deleted.AccountId, deleted.OccurredAtUtc, cancellationToken)
           .ConfigureAwait(false);

        LogRemoved(removed, deleted.AccountId);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Removed {Count} todo lists of deleted account {AccountId}")]
    private partial void LogRemoved(int count, Guid accountId);
}
