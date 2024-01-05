using Hangfire;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.AppBuilders;

/// <summary>
///  Endpoints app builder
/// </summary>
public static class Endpoints
{
    /// <summary>
    ///  Use endpoints
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseEndpointsBuilder(this IApplicationBuilder app)
    {
        app.UseEndpoints(
            endpoints =>
            {
                endpoints
                    .MapDefaultControllerRoute()
                    .RequireAuthorization();

                endpoints.MapHangfireDashboard();
            });

        return app;
    }
}
