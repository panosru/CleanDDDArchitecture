using Aviant.Application.Identity;
using Aviant.Application.Services;
using Aviant.Core.Services;
using CleanDDDArchitecture.Hosts.RestApi.Application.Services;

namespace CleanDDDArchitecture.Hosts.RestApi.Application.ServiceExtensions;

/// <summary>
///  Singleton services service extension
/// </summary>
public static class Singleton
{
    /// <summary>
    ///  Add all singleton services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSingletonServices(this IServiceCollection services)
    {
        services.AddSingleton<IServiceContainer, HttpContextServiceProviderProxy>();
        services.AddSingleton<ICurrentUserService, CurrentUser>();
        
        return services;
    }
}
