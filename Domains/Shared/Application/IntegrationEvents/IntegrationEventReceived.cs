using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using MediatR;

namespace CleanDDDArchitecture.Domains.Shared.Application.IntegrationEvents;

/// <summary>
///     How an integration event reaches its handlers inside a process. Handle
///     <c>INotificationHandler&lt;IntegrationEventReceived&lt;TEvent&gt;&gt;</c>; handlers must be
///     idempotent, because delivery is at least once.
/// </summary>
public sealed record IntegrationEventReceived<TEvent>(TEvent Event) : INotification
    where TEvent : IIntegrationEvent;
