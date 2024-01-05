using System.Reflection;
using Aviant.Application.Behaviours;
using Aviant.Application.Extensions;
using Aviant.Application.Interceptors;
using Aviant.Application.Processors;
using CleanDDDArchitecture.Domains.Account.CrossCutting;
using CleanDDDArchitecture.Domains.Todo.CrossCutting;
using CleanDDDArchitecture.Domains.Weather.CrossCutting;
using MediatR;
using MediatR.Pipeline;

namespace CleanDDDArchitecture.Hosts.RestApi.Presentation.ServiceExtensions;

/// <summary>
///  Mediator service extension
/// </summary>
public static class Mediator
{
    /// <summary>
    ///   Add Mediator services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMediatorServices(this IServiceCollection services)
    {
       services.AddTransient<IMediator, MediatR.Mediator>();

        services.Scan(
            scan => scan.FromAssemblies(
                    new List<Assembly>
                        {
                            typeof(Program).Assembly,
                            typeof(LoggerBehaviour<>).Assembly
                        }
                       .Union(TodoCrossCutting.MediatorAssemblies())
                       .Union(AccountCrossCutting.MediatorAssemblies())
                       .Union(WeatherCrossCutting.MediatorAssemblies())
                       .ToArray())
               .RegisterHandlers(typeof(IRequestHandler<>))
               .RegisterHandlers(typeof(IRequestHandler<,>))
               .RegisterHandlers(typeof(InterceptorBase<>))
               .RegisterHandlers(typeof(INotificationHandler<>))
               .RegisterHandlers(typeof(IRequestPreProcessor<>))
               .RegisterHandlers(typeof(IRequestPostProcessor<,>))
               .RegisterHandlers(typeof(IRequestExceptionHandler<,,>))
               .RegisterHandlers(typeof(IRequestExceptionAction<,>)));

        services.Decorate(
            typeof(IRequestHandler<,>),
            typeof(RetryRequestProcessor<,>));

        services.Decorate(
            typeof(INotificationHandler<>),
            typeof(RetryEventProcessor<>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(PerformanceBehaviour<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehaviour<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(UnhandledExceptionBehaviour<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestPreProcessorBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestPostProcessorBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestExceptionActionProcessorBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(RequestExceptionProcessorBehavior<,>));

        return services;
    }
}
