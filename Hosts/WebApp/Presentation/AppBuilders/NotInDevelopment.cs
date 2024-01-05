namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.AppBuilders;

/// <summary>
///  Not in development environment app builder
/// </summary>
public static class NotInDevelopment
{
    /// <summary>
    ///   Use application builders not in development environment
    /// </summary>
    /// <param name="app"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseNotInDevelopmentBuilder(
        this IApplicationBuilder app,
        IHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios,
            // see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        return app;
    }
}
