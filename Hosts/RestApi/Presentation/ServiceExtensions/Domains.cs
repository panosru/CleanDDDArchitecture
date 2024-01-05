using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using CleanDDDArchitecture.Domains.Weather.CrossCutting;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Domains service extension
/// </summary>
public static class Domains
{
    /// <summary>
    ///  Add all domains services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomainsServices(this IServiceCollection services)
    {
        services
            .AddAccountDomain()
            .AddTodoDomain()
            .AddWeatherDomain();

        return services;
    }
}
