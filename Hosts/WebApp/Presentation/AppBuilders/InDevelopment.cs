using Westwind.AspNetCore.LiveReload;

namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.AppBuilders;

/// <summary>
///  In development environment app builder
/// </summary>
public static class InDevelopment
{
    /// <summary>
    ///   Use application builders in development environment
    /// </summary>
    /// <param name="app"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseInDevelopmentBuilder(
        this IApplicationBuilder app,
        IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseLiveReload();
            app.UseDeveloperExceptionPage();
        }
        
        return app;
    }
}
