using CleanDDDArchitecture.Hosts.RestApi.Presentation.Features;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Features service extension
/// </summary>
public static class Features
{
    /// <summary>
    ///  Add Features services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddFeaturesServices(this IServiceCollection services)
    {
        services.AddFeatureFlags();

        return services;
    }
}
