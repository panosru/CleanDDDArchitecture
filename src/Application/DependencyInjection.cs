namespace CleanDDDArchitecture.Application
{
    using System.Reflection;
    using AutoMapper;
    using Aviant.DDD.Application.Behaviour;
    using Aviant.DDD.Application.Behaviour.Request;
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(Performance<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(Validation<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>),
                typeof(UnhandledException<,>));

            return services;
        }
    }
}