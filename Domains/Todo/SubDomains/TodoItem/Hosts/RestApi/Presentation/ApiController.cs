using Aviant.Application.UseCases;
using CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Core;
using CleanDDDArchitecture.Hosts.RestApi.Core.Routing;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Presentation;

/// <inheritdoc />
/// <summary>
///     Todo Item endpoints
/// </summary>
[RouteSegment(ITodoItemDomainConfiguration.RouteSegment)]
public abstract class ApiController : Todo.Hosts.RestApi.Core.ApiController;

/// <inheritdoc />
/// <summary>
///     Todo Item endpoint
/// </summary>
/// <typeparam name="TUseCase"></typeparam>
/// <typeparam name="TUseCaseOutput"></typeparam>
[RouteSegment(ITodoItemDomainConfiguration.RouteSegment)]
public abstract class ApiController<TUseCase, TUseCaseOutput>
    : Todo.Hosts.RestApi.Core.ApiController<TUseCase, TUseCaseOutput>
    where TUseCase : class, IUseCase<TUseCaseOutput>
    where TUseCaseOutput : class, IUseCaseOutput
{
    /// <inheritdoc />
    protected ApiController(TUseCase useCase)
        : base(useCase)
    { }
}
