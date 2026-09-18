using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace CleanDDDArchitecture.Hosts.ServiceDefaults.Core;

/// <summary>
///     What every host in the solution shares: OpenTelemetry, health endpoints, resilient HTTP
///     clients and service discovery. Under Aspire the telemetry appears in the dashboard;
///     elsewhere set <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> to send it to any OTLP collector.
/// </summary>
public static class ServiceDefaultsExtensions
{
    private const string LivenessTag = "live";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureOpenTelemetry();

        builder.Services.AddHealthChecks()
           .AddCheck("self", () => HealthCheckResult.Healthy(), [LivenessTag]);

        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(
            http =>
            {
                http.AddStandardResilienceHandler();
                http.AddServiceDiscovery();
            });

        return builder;
    }

    /// <summary>
    ///     Maps <c>/health</c> (readiness: every registered check, including databases) and
    ///     <c>/alive</c> (liveness: only whether the process responds).
    /// </summary>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapHealthChecks("/health").AllowAnonymous();
        app.MapHealthChecks(
                "/alive",
                new HealthCheckOptions { Predicate = check => check.Tags.Contains(LivenessTag) })
           .AllowAnonymous();

        return app;
    }

    private static void ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(
            logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes           = true;
            });

        builder.Services.AddOpenTelemetry()
           .WithMetrics(
                metrics => metrics
                   .AddAspNetCoreInstrumentation()
                   .AddHttpClientInstrumentation()
                   .AddRuntimeInstrumentation())
           .WithTracing(
                tracing => tracing
                   .AddSource(builder.Environment.ApplicationName)
                   .AddSource("Npgsql")
                   .AddAspNetCoreInstrumentation(
                        options => options.Filter = context =>
                            !context.Request.Path.StartsWithSegments("/health", StringComparison.Ordinal)
                         && !context.Request.Path.StartsWithSegments("/alive", StringComparison.Ordinal))
                   .AddHttpClientInstrumentation());

        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
    }
}
