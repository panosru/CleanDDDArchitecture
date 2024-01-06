using Aviant.Application.Persistence.Orchestration;
using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Todo.Application.Persistence;
using CleanDDDArchitecture.Domains.Todo.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CleanDDDArchitecture.Domains.Todo.Hosts.RestApi.Core;

/// <inheritdoc />
/// <summary>
///     Todo endpoints
/// </summary>
[RouteSegment(ITodoDomainConfiguration.RouteSegment)]
public abstract class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
{
    /// <summary>
    /// </summary>
    protected new IOrchestrator<ITodoDbContextWrite> Orchestrator =>
        HttpContext.RequestServices.GetRequiredService<IOrchestrator<ITodoDbContextWrite>>();
}

/// <inheritdoc />
/// <summary>
///     Todo endpoint
/// </summary>
/// <typeparam name="TUseCase"></typeparam>
/// <typeparam name="TUseCaseOutput"></typeparam>
[RouteSegment(ITodoDomainConfiguration.RouteSegment)]
public abstract class ApiController<TUseCase, TUseCaseOutput>
    : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase, TUseCaseOutput>
    where TUseCase : class, IUseCase<TUseCaseOutput>
    where TUseCaseOutput : class, IUseCaseOutput
{
    /// <inheritdoc />
    protected ApiController(TUseCase useCase)
        : base(useCase)
    { }
}