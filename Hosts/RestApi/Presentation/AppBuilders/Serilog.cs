using Serilog;
using Serilog.Events;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.AppBuilders;

/// <summary>
///  Serilog app builder
/// </summary>
public static class Serilog
{
    /// <summary>
    ///   Use Serilog
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSerilogBuilder(this IApplicationBuilder app)
    {
        //TODO: Revisit the logging configuration
        app.UseSerilogRequestLogging(
            options =>
            {
                // Customize the message template
                options.MessageTemplate = "Handled {RequestPath}";

                // Emit debug-level events instead of the defaults
                options.GetLevel = (
                    _ /* httpContext */,
                    _ /* elapsed */,
                    _ /* Exception */) => LogEventLevel.Debug;

                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost",   httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                };
            });

        return app;
    }
}
