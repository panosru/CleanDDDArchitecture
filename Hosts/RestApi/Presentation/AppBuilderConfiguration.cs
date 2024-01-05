using Aviant.Core.Services;
using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.AppBuilders;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation;

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
        ServiceLocator.Initialise(serviceProvider);

        app.UseInDevelopmentBuilder(environment);
        app.UseNotInDevelopmentBuilder(environment);
        app.UseSerilogBuilder();
        app.UseHangfireBuilder();
        app.UseHealthChecksBuilder();
        app.UseSwaggerBuilder();
        app.UseHttpsRedirection();
        app.UseStaticFilesBuilder();
        app.UseSession();
        app.UseRouting();
        app.UseAccountAuth();
        app.UseEndpointsBuilder();
    }
}
