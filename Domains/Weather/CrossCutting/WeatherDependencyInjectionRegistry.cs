namespace CleanDDDArchitecture.Domains.Weather.CrossCutting
{
    using Application.UseCases.AddCity;
    using Application.UseCases.Forecast;
    using Application.UseCases.SyncWeatherService;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.Extensions.DependencyInjection;

    public static class WeatherDependencyInjectionRegistry
    {
        public static IServiceCollection AddWeatherDomain(this IServiceCollection services)
        {
            services.AddScoped<IOrchestrator, Orchestrator>();

            services.AddScoped(typeof(AddCityUseCase));
            services.AddScoped(typeof(ForecastUseCase));
            services.AddScoped(typeof(Application.UseCases.ForecastV1_1.ForecastUseCase));
            services.AddScoped(typeof(SyncWeatherServiceUseCase));

            return services;
        }
    }
}