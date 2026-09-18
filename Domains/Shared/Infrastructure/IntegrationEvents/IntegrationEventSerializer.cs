using System.Collections.Frozen;
using System.Text.Json;
using CleanDDDArchitecture.Domains.Shared.Core.IntegrationEvents;

namespace CleanDDDArchitecture.Domains.Shared.Infrastructure.IntegrationEvents;

/// <summary>
///     Serialises integration events as JSON and resolves them by contract name. Contracts live in
///     Shared.Core, which every context references, so every publisher and consumer knows them.
/// </summary>
public static class IntegrationEventSerializer
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static readonly FrozenDictionary<string, Type> Contracts = typeof(IIntegrationEvent).Assembly
       .GetTypes()
       .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IIntegrationEvent).IsAssignableFrom(type))
       .ToFrozenDictionary(type => type.FullName!, StringComparer.Ordinal);

    public static string TypeName(IIntegrationEvent integrationEvent)
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        return integrationEvent.GetType().FullName!;
    }

    public static string Serialize(IIntegrationEvent integrationEvent) =>
        JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType(), Json);

    public static IIntegrationEvent Deserialize(string typeName, string payload)
    {
        if (!Contracts.TryGetValue(typeName, out var type))
            throw new InvalidOperationException($"Unknown integration event type \"{typeName}\".");

        return (IIntegrationEvent)(JsonSerializer.Deserialize(payload, type, Json)
         ?? throw new InvalidOperationException($"Integration event \"{typeName}\" deserialised to null."));
    }
}
