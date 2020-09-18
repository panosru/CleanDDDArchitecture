namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application
{
    using Aviant.DDD.Application.Orchestration;
    using Aviant.DDD.Application.UseCases;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Infrastructure.Persistence.Contexts;

    public abstract class ApiController : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController
    {
        protected new IOrchestrator<TodoDbContextWrite> Orchestrator => 
            HttpContext.RequestServices.GetRequiredService<IOrchestrator<TodoDbContextWrite>>();
    }
    
    public class ApiController<TUseCase> : CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController<TUseCase>
        where TUseCase : class, IUseCase
    {
        public ApiController(TUseCase useCase)
            : base(useCase)
        { }
    }
}