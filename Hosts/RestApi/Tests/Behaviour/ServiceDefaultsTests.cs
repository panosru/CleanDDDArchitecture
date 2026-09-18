using System.Net;
using AwesomeAssertions;
using CleanDDDArchitecture.Hosts.ServiceDefaults.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Xunit;

namespace CleanDDDArchitecture.Hosts.RestApi.Tests.Behaviour;

/// <summary>
///     What every host gets from <c>AddServiceDefaults</c> / <c>MapDefaultEndpoints</c>.
/// </summary>
public sealed class ServiceDefaultsTests
{
    [Fact]
    public async Task AliveReportsHealthyWithoutRunningDependencyChecks()
    {
        await using var app = await StartAsync(failingDependency: true);

        using var response = await app.GetTestClient().GetAsync(new Uri("/alive", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK, "liveness only checks the process itself");
    }

    [Fact]
    public async Task HealthRunsEveryCheck()
    {
        await using var app = await StartAsync(failingDependency: true);

        using var response = await app.GetTestClient().GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable, "readiness includes the failing dependency");
    }

    [Fact]
    public async Task TracingAndMetricsAreRegistered()
    {
        await using var app = await StartAsync(failingDependency: false);

        app.Services.GetService<TracerProvider>().Should().NotBeNull();
        app.Services.GetService<MeterProvider>().Should().NotBeNull();
    }

    private static async Task<WebApplication> StartAsync(bool failingDependency)
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.AddServiceDefaults();

        if (failingDependency)
            builder.Services.AddHealthChecks().AddCheck("database", () => HealthCheckResult.Unhealthy("down"));

        var app = builder.Build();
        app.MapDefaultEndpoints();
        await app.StartAsync(TestContext.Current.CancellationToken);

        return app;
    }
}
