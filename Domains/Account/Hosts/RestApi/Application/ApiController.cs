namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application
{
    using Account.Application.Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.Extensions.DependencyInjection;
    using ApiControllerCore = CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController;

    public abstract class ApiController : ApiControllerCore
    {
        protected new IOrchestrator<AccountAggregate, AccountAggregateId> Orchestrator =>
            HttpContext.RequestServices.GetRequiredService<IOrchestrator<AccountAggregate, AccountAggregateId>>();
    }
}