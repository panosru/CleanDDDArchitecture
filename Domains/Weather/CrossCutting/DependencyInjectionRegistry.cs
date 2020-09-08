namespace CleanDDDArchitecture.Domains.Weather.CrossCutting
{
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddWeather(
            this IServiceCollection services,
            IConfiguration          configuration)
        {
            services.AddScoped<IOrchestrator, Orchestrator>();
            
            return services;
        }
    }
}