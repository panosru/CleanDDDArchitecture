using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using CleanDDDArchitecture.Domains.Weather.CrossCutting;
using FluentValidation;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///   Validator service extension
/// </summary>
public static class Validator
{
    /// <summary>
    ///  Add Validator services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddValidatorServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblies(
            TodoCrossCutting.ValidatorAssemblies()
                .Union(AccountCrossCutting.ValidatorAssemblies())
                .Union(WeatherCrossCutting.ValidatorAssemblies())
                .ToArray());

        return services;
    }
}
