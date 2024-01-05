namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Session service extension
/// </summary>
public static class Session
{
    /// <summary>
    ///  Add Session services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSessionServices(this IServiceCollection services)
    {
        services
            .AddDistributedMemoryCache()
            .AddSession(
                options =>
                {
                    options.IdleTimeout        = TimeSpan.FromMinutes(30);
                    options.Cookie.HttpOnly    = true;
                    options.Cookie.IsEssential = true;
                });
        
        return services;
    }
}
