using AwesomeAssertions;
using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;
using CleanDDDArchitecture.Domains.Shared.Infrastructure.Outbox;
using Xunit;

namespace CleanDDDArchitecture.Domains.Shared.Tests.Unit;

public sealed class IntegrationEventSerializerTests
{
    private static readonly AccountDeletedIntegrationEvent Deleted =
        AccountDeletedIntegrationEvent.For(Guid.Parse("6d1f1d4e-0b53-4b9a-9d7e-0a1c5c2b3e44"), new DateTime(2026, 9, 18, 10, 0, 0, DateTimeKind.Utc));

    [Fact]
    public void AnEventSurvivesTheRoundTrip()
    {
        var restored = IntegrationEventSerializer.Deserialize(
            IntegrationEventSerializer.TypeName(Deleted),
            IntegrationEventSerializer.Serialize(Deleted));

        restored.Should().Be(Deleted);
    }

    [Fact]
    public void AnUnknownContractIsRefused()
    {
        var act = () => IntegrationEventSerializer.Deserialize("Nope.SomethingHappened", "{}");

        act.Should().Throw<InvalidOperationException>().WithMessage("*Nope.SomethingHappened*");
    }

    [Fact]
    public void AnOutboxMessageCarriesTheEventIdTypeAndTime()
    {
        var message = OutboxMessage.From(Deleted);

        message.Id.Should().Be(Deleted.Id);
        message.Type.Should().Be(typeof(AccountDeletedIntegrationEvent).FullName);
        message.OccurredAtUtc.Should().Be(Deleted.OccurredAtUtc);
        message.ProcessedAtUtc.Should().BeNull();
        IntegrationEventSerializer.Deserialize(message.Type, message.Payload).Should().Be(Deleted);
    }
}
