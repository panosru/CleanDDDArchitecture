namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.AppBuilders;

/// <summary>
///  Health checks app builder
/// </summary>
public static class HealthChecks
{
    /// <summary>
    ///  Use health checks
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseHealthChecksBuilder(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");

        return app;
    }
}
