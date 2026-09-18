using Aviant.Application.Extensions;
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
}
