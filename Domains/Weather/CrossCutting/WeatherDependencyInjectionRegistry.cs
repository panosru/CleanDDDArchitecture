using CleanDDDArchitecture.Domains.Weather.Application.Services;
using CleanDDDArchitecture.Domains.Weather.Application.UseCases.AddCity;
using CleanDDDArchitecture.Domains.Weather.Application.UseCases.Forecast;
using CleanDDDArchitecture.Domains.Weather.Application.UseCases.SyncWeatherService;
using Aviant.Application.Interceptors;
using Aviant.Application.Orchestration;
using Aviant.Infrastructure.CrossCutting;
using CleanDDDArchitecture.Domains.Weather.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDDDArchitecture.Domains.Weather.CrossCutting;

public static class WeatherDependencyInjectionRegistry
{
    private const string CurrentDomain = "Weather";

    static WeatherDependencyInjectionRegistry() => Configuration =
        DependencyInjectionRegistry.GetDomainConfiguration(
            CurrentDomain.ToLower());

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static IConfiguration Configuration { get; }

    public static IServiceCollection AddWeatherDomain(this IServiceCollection services)
    {
        services.AddScoped(_ => new WeatherDomainConfiguration(Configuration));

        services.AddScoped<IOrchestrator, Orchestrator>();

        services.AddScoped(typeof(AddCityUseCase));
        services.AddScoped(typeof(ForecastUseCase));
        services.AddScoped(typeof(Application.UseCases.ForecastV1_1.ForecastUseCase));
        services.AddScoped(typeof(SyncWeatherServiceUseCase));

        services.RegisterProxied<IWeatherForecastService, WeatherForecastService>(ProxyInterceptorLifetime.Scoped);

        return services;
    }
}
