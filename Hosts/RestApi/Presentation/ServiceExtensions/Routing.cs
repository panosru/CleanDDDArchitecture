namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Routing service extension
/// </summary>
public static class Routing
{
    /// <summary>
    ///  Add Routing services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRoutingServices(this IServiceCollection services)
    {
        services.AddRouting(
            options => options.LowercaseUrls = true);

        return services;
    }
}
