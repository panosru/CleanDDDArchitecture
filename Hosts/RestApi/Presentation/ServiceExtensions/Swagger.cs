using CleanDDDArchitecture.Hosts.RestApi.Presentation.Swagger;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

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
