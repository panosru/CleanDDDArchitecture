using Aviant.Infrastructure.CrossCutting;
using CleanDDDArchitecture.Domains.Shared.Core;
using CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

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
        
        // Bind App Settings
        var appSettings = configuration.GetSection("AppSettings");
        services.Configure<AppSettings>(appSettings);

        services.AddEmailService(configuration);
        services.AddAutoMapperServices();
        services.AddValidatorServices();
        services.AddDataProtectionServices();
        services.AddMediatorServices();
        services.AddSessionServices();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("public-auth", context => BuildIpPolicy(context, 5, TimeSpan.FromMinutes(1)));
            options.AddPolicy("public-recovery", context => BuildIpPolicy(context, 3, TimeSpan.FromMinutes(5)));
            options.AddPolicy("public-registration", context => BuildIpPolicy(context, 2, TimeSpan.FromMinutes(10)));
            options.AddPolicy("authenticated-sensitive", context => BuildIpPolicy(context, 10, TimeSpan.FromMinutes(1)));
        });
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

    private static RateLimitPartition<string> BuildIpPolicy(HttpContext context, int permitLimit, TimeSpan window)
    {
        var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window,
                QueueLimit = 0,
                AutoReplenishment = true
            });
    }
}
