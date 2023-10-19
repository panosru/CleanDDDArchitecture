using CleanDDDArchitecture.Hosts.WebApp.Application.ServiceExtensions;

namespace CleanDDDArchitecture.Hosts.WebApp.Application;

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
        services.AddCommonServices(configuration, environment);
        services.AddSecurityServices();
        services.AddPagesAndLocaleServices();
    }
}
