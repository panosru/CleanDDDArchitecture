namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.AppBuilders;

/// <summary>
///  Endpoints app builder
/// </summary>
public static class Endpoints
{
    /// <summary>
    ///  Use application builders in development environment
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseEndpointsBuilder(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints => endpoints.MapRazorPages());

        return app;
    }
}
