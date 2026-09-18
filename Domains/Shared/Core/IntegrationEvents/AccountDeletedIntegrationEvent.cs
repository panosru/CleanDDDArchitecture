namespace CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;

/// <summary>
///     Published by the Account context when an account is deleted, so other contexts can
///     remove what they hold for that user.
/// </summary>
public sealed record AccountDeletedIntegrationEvent(Guid AccountId, Guid Id, DateTime OccurredAtUtc) : IIntegrationEvent
{
    public static AccountDeletedIntegrationEvent For(Guid accountId, DateTime occurredAtUtc) =>
        new(accountId, Guid.NewGuid(), occurredAtUtc);
}
