namespace CleanDDDArchitecture.Hosts.RestApi.Application.Features
{
    using Aviant.DDD.Infrastructure.CrossCutting;
    using Core.Features;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.FeatureManagement;

    /// <summary>
    /// </summary>
    internal static class FeatureExtension
    {
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddFeatureFlags(this IServiceCollection services)
        {
            services.AddFeatureManagement(DependencyInjectionRegistry.ConfigurationWithDomains);

            var featureManager = services.BuildServiceProvider()
               .GetRequiredService<IFeatureManager>();

            services.AddControllersWithViews()
               .ConfigureApplicationPartManager(
                    apm =>
                        apm.FeatureProviders.Add(
                            new CustomControllerFeatureProvider(featureManager)));

            return services;
        }
    }
}
