using CleanDDDArchitecture.Hosts.RestApi.Presentation.Filters;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.Routing;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Add controllers services
/// </summary>
public static class Controllers
{
    /// <summary>
    ///   Add controllers services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddControllersServices(this IServiceCollection services)
    {
        services.AddControllersWithViews(
            options =>
            {
                options.Filters.Add(new ApiExceptionFilterAttribute());
                options.Filters.Add(new AuthorizeFilter());
                options.Conventions.Add(new CustomRouteConvention());
            });
        
        return  services;
    }
}
