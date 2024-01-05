using AutoMapper;
using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using CleanDDDArchitecture.Domains.Weather.CrossCutting;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///    AutoMapper service extension
/// </summary>
public static class AutoMapper
{
    /// <summary>
    ///   Add AutoMapper services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
    {
        services.AddAutoMapper(
            cfg =>
            {
                IEnumerable<Profile> profiles = TodoCrossCutting.AutoMapperProfiles()
                    .Union(AccountCrossCutting.AutoMapperProfiles())
                    .Union(WeatherCrossCutting.AutoMapperProfiles())
                    .ToList();

                foreach (var profile in profiles)
                    cfg.AddProfile(profile);
            });

        return services;
    }
}
