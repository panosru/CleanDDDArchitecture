using CleanDDDArchitecture.Hosts.RestApi.Application.Swagger;

namespace CleanDDDArchitecture.Hosts.RestApi.Application.ServiceExtensions;

/// <summary>
///  Swagger service extension
/// </summary>
public static class Swagger
{
    /// <summary>
    ///  Add Swagger services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services
            .AddApiVersionWithExplorer()
            .AddSwaggerOptions()
            .AddSwaggerGen();

        return services;
    }
}
