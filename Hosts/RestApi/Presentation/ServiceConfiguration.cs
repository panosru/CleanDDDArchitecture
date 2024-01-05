using Aviant.Infrastructure.CrossCutting;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation;

/// <summary>
///  Service configuration
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    ///   Add services to the program.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    public static void ConfigureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddSingleton(configuration);
        DependencyInjectionRegistry.CurrentEnvironment = environment;

        services.AddAutoMapperServices();
        services.AddValidatorServices();
        services.AddDataProtectionServices();
        services.AddMediatorServices();
        services.AddSessionServices();
        services.AddHangfireServices(configuration);
        services.AddDomainsServices();
        services.AddFeaturesServices();
        services.AddScopedServices();
        services.AddSingletonServices();
        services.AddSwaggerServices();
        services.AddHttpContextAccessor();
        services.AddHealthCheckServices();
        services.AddRoutingServices();

        if (environment.IsDevelopment())
            services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddControllersServices();
    }
}
