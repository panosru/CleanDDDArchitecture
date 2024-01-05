using Hangfire;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.AppBuilders;

/// <summary>
///  Hangfire app builder
/// </summary>
public static class Hangfire
{
    /// <summary>
    ///  Use Hangfire
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseHangfireBuilder(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard(
            "/jobs",
            new DashboardOptions
            {
                DashboardTitle = "Jobs",
                Authorization = new[]
                {
                    new HangfireAuthorizationFilter()
                },
                IgnoreAntiforgeryToken = true
            });

        return app;
    }
}
