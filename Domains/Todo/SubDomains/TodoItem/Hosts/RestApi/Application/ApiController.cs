namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application
{
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Application.Persistence;

    public abstract class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
    {
        protected new IOrchestrator<ITodoDbContextWrite> Orchestrator =>
            HttpContext.RequestServices.GetRequiredService<IOrchestrator<ITodoDbContextWrite>>();
    }

    public class ApiController<TUseCase> : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase>
        where TUseCase : class, IUseCase
    {
        public ApiController(TUseCase useCase)
            : base(useCase)
        { }
    }
}