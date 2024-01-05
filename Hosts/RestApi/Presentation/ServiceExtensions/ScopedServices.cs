using Aviant.Application.ApplicationEvents;
using Aviant.Core.Messages;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Scoped services service extension
/// </summary>
public static class ScopedServices
{
    /// <summary>
    ///  Add all scoped services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddScopedServices(this IServiceCollection services)
    {
        services.AddScoped<IMessages, Messages>();
        services.AddScoped<IApplicationEventDispatcher, ApplicationEventDispatcher>();

        return services;
    }
}
