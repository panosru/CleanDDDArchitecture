namespace CleanDDDArchitecture.Domains.Weather.CrossCutting
{
    #region

    using Aviant.DDD.Application.Orchestration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    #endregion

    public static class WeatherDependencyInjectionRegistry
    {
        public static IServiceCollection AddWeather(this IServiceCollection services)
        {
            services.AddScoped<IOrchestrator, Orchestrator>();

            return services;
        }
    }
}