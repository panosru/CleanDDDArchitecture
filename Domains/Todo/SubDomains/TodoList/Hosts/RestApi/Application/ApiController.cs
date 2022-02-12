namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Hosts.RestApi.Application;

using Aviant.Foundation.Application.Orchestration;
using Aviant.Foundation.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Persistence;

/// <inheritdoc />
/// <summary>
///     Todo Lists endpoints
/// </summary>
public abstract class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
{
    /// <summary>
    /// </summary>
    protected new IOrchestrator<ITodoDbContextWrite> Orchestrator =>
        HttpContext.RequestServices.GetRequiredService<IOrchestrator<ITodoDbContextWrite>>();
}

/// <inheritdoc />
/// <summary>
///     Todo Lists endpoints
/// </summary>
/// <typeparam name="TUseCase"></typeparam>
/// <typeparam name="TUseCaseOutput"></typeparam>
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
