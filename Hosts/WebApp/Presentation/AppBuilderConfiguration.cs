using CleanDDDArchitecture.Hosts.WebApp.Presentation.AppBuilders;

namespace CleanDDDArchitecture.Hosts.WebApp.Presentation;

/// <summary>
///  App builder configuration
/// </summary>
public static class AppBuilderConfiguration
{
    /// <summary>
    ///  Configure the application builder
    /// </summary>
    /// <param name="app"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="environment"></param>
    public static void ConfigureAppBuilder(
        this IApplicationBuilder app, 
        IServiceProvider serviceProvider,
        IHostEnvironment environment)
    {
        app.UseInDevelopmentBuilder(environment);
        app.UseNotInDevelopmentBuilder(environment);
        app.UsePagesAndLocaleBuilder();
        app.UseSecurityBuilder(environment);
        app.UseEndpointsBuilder();
    }
}
