namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Application
{
    #region

    using Account.Application.Aggregates;
    using Aviant.DDD.Application.Orchestration;
    using Microsoft.Extensions.DependencyInjection;
    using ApiControllerCore = CleanDDDArchitecture.Hosts.RestApi.Core.Controllers.ApiController;

    #endregion

    public abstract class ApiController : ApiControllerCore
    {
        private IOrchestrator<AccountAggregate, AccountAggregateId>? _orchestrator;

        protected new IOrchestrator<AccountAggregate, AccountAggregateId> Orchestrator =>
            _orchestrator ??= HttpContext.RequestServices
               .GetService<IOrchestrator<AccountAggregate, AccountAggregateId>>();
    }
}