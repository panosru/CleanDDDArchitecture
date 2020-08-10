namespace CleanDDDArchitecture.Services
{
    using System.Collections.Generic;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using NetCore.AutoRegisterDi;

    public static class DependencyInjection
    {
        private static List<Assembly> _assemblies { get; set; }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // services.AddScoped<IWeatherForcast, WeatherForcast>();

            services.RegisterAssemblyPublicNonGenericClasses(
                    Assembly.GetExecutingAssembly())
                .Where(
                    x =>
                        x.Name.EndsWith("Service"))
                //typeof(IIdentityService).IsAssignableFrom(x))
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            // services.AddTransient(typeof(IWeatherForcast), typeof(WeatherForcast));

            return services;
        }
    }
}