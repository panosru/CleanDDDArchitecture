﻿namespace CleanDDDArchitecture.Application
{
    using System.Reflection;
    using AutoMapper;
    using Aviant.DDD.Application.Behaviours;
    using Aviant.DDD.Application.Mappings;
    using FluentValidation;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(
                cfg => { cfg.AddProfile(new MappingProfile(Assembly.GetExecutingAssembly())); });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            //services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(PerformanceBehaviour<,>));

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehaviour<,>));

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(UnhandledExceptionBehaviour<,>));


            // services.RegisterApplication(typeof(ITodoDbContext).Assembly);

            return services;
        }
    }
}