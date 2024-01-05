using Westwind.AspNetCore.LiveReload;

namespace CleanDDDArchitecture.Hosts.WebApp.Presentation.ServiceExtensions;

/// <summary>
///  This class contains common services for the application.
/// </summary>
public static class Common
{
    /// <summary>
    ///   This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration      configuration,
        IHostEnvironment    environment)
    {
        services.AddOptions();
        services.AddSingleton(configuration);

        if (environment.IsDevelopment())
            services.AddLiveReload();
        
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        return  services;
    }
}
