using System.Reflection;
using Aviant.Application.Extensions;
using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using CleanDDDArchitecture.Domains.Weather.CrossCutting;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Mediator service extension
/// </summary>
public static class Mediator
{
    /// <summary>
    ///   Registers the CQRS pipeline for every domain the monolith hosts.
    /// </summary>
    /// <remarks>
    ///   Startup fails, naming the requests, if a domain the API references is missing here.
    /// </remarks>
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
        IEnumerable<Assembly> assemblies = new List<Assembly> { typeof(Program).Assembly }
           .Union(TodoCrossCutting.MediatorAssemblies())
           .Union(AccountCrossCutting.MediatorAssemblies())
           .Union(WeatherCrossCutting.MediatorAssemblies());

        return services.AddAviantCqrs(assemblies);
    }
}
