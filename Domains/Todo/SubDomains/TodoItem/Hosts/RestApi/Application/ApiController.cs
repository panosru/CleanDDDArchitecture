namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application
{
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Application.Persistence;

    /// <inheritdoc />
    /// <summary>
    ///     Todo Item endpoints
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
    ///     Todo Item endpoint
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
}