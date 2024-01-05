using Aviant.Infrastructure.CrossCutting;
using CleanDDDArchitecture.Hosts.RestApi.Core.Features;
using Microsoft.FeatureManagement;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.Features;

/// <summary>
/// </summary>
internal static class FeatureExtension
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddFeatureFlags(this IServiceCollection services)
    {
        services.AddFeatureManagement(DependencyInjectionRegistry.ConfigurationWithDomains);

        var featureManager = services.BuildServiceProvider()
           .GetRequiredService<IFeatureManager>();

        services.AddControllersWithViews()
           .ConfigureApplicationPartManager(
                apm =>
                    apm.FeatureProviders.Add(
                        new CustomControllerFeatureProvider(featureManager)));

        return services;
    }
}
