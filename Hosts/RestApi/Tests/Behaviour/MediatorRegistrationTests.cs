using System.Reflection;
using Aviant.Application.Extensions;
using Aviant.Application.UseCases;
using AwesomeAssertions;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CleanDDDArchitecture.Hosts.RestApi.Tests.Behaviour;

/// <summary>
///     Runs the API's startup check in CI: every command and query in the domains the API
///     references must have a registered handler. Leaving a domain out of
///     <see cref="Mediator.AddMediatorServices" /> fails this test, naming its requests.
/// </summary>
public sealed class MediatorRegistrationTests
{
    [Fact]
    public async Task EveryRequestTheApiCanSendHasAHandler()
    {
        var services = new ServiceCollection();
        services.AddMediatorServices();
        await using var provider = services.BuildServiceProvider();
        var validator = new CqrsHandlerValidator(
            [],
            provider.GetRequiredService<IServiceProviderIsService>(),
            rootAssembly: typeof(Mediator).Assembly);

        var act = () => validator.StartAsync(TestContext.Current.CancellationToken);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void EveryUseCaseTheApiReferencesIsRegisteredWithItsActivation()
    {
        var services = new ServiceCollection();
        services.AddMediatorServices();

        var unregistered = ReferencedAssemblies(typeof(Mediator).Assembly)
           .SelectMany(assembly => assembly.GetTypes())
           .Where(type => type is { IsClass: true, IsAbstract: false, ContainsGenericParameters: false }
                       && typeof(IUseCaseActivation).IsAssignableFrom(type))
           .Where(type => !services.Any(d => d.ServiceType == type && d.ImplementationFactory is not null))
           .Select(type => type.FullName)
           .ToList();

        unregistered.Should().BeEmpty("a use case must be registered through AddAviantUseCases so it is activated with its scope");
    }

    private static IEnumerable<Assembly> ReferencedAssemblies(Assembly root)
    {
        var seen    = new HashSet<string>(StringComparer.Ordinal);
        var pending = new Queue<Assembly>([root]);

        while (pending.TryDequeue(out var assembly))
        {
            if (!seen.Add(assembly.GetName().Name!))
                continue;

            yield return assembly;

            foreach (var reference in assembly.GetReferencedAssemblies()
                        .Where(name => name.Name!.StartsWith("CleanDDDArchitecture.", StringComparison.Ordinal)))
                pending.Enqueue(Assembly.Load(reference));
        }
    }
}
