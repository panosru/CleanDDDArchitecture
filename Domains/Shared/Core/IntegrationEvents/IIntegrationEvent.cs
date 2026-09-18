namespace CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;

/// <summary>
///     A fact one bounded context publishes for others. Unlike a domain event, it is a public
///     contract: its shape is versioned deliberately, because other contexts depend on it.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>Unique per occurrence; consumers use it to recognise redelivery.</summary>
    public Guid Id { get; }

    public DateTime OccurredAtUtc { get; }
}
