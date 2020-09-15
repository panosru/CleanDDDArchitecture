namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Hosts.RestApi.Application
{
    #region

    using Aviant.DDD.Application.Orchestration;
    using Microsoft.Extensions.DependencyInjection;
    using Todo.Infrastructure.Persistence.Contexts;
    using ApiControllerCore = CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController;

    #endregion

    public abstract class ApiController : ApiControllerCore
    {
        protected new IOrchestrator<TodoDbContextWrite> Orchestrator => 
            HttpContext.RequestServices.GetRequiredService<IOrchestrator<TodoDbContextWrite>>();
    }
}