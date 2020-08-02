using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CleanArchitecture.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), 
                typeof(Aviant.DDD.Application.Behaviour.Request.Performance<,>));
            
            services.AddTransient(typeof(IPipelineBehavior<,>), 
                typeof(Aviant.DDD.Application.Behaviour.Request.Validation<,>));
            
            services.AddTransient(typeof(IPipelineBehavior<,>), 
                typeof(Aviant.DDD.Application.Behaviour.UnhandledException<,>));

            return services;
        }
    }
}
