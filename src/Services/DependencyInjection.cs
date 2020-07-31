using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AutoMapper;
using CleanArchitecture.Services.v1_0;
using CleanArchitecture.Services.v1_0.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace CleanArchitecture.Services
{
    public static class DependencyInjection
    {
        private static List<Assembly> _assemblies { get; set; }
        
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // services.AddScoped<IWeatherForcast, WeatherForcast>();

            
            Console.WriteLine("@£$@£%@£$@%@%£");

            services.RegisterAssemblyPublicNonGenericClasses(
                    Assembly.GetExecutingAssembly())
                .Where(x =>
                    x.Name.EndsWith("Service"))
                    //typeof(IService).IsAssignableFrom(x))
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            Console.WriteLine("@£$@£%@£$@%@%£");

            // services.AddTransient(typeof(IWeatherForcast), typeof(WeatherForcast));
            
            return services;
        }
    }
}